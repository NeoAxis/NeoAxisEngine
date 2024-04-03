$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

#ifdef USE_2D
	SAMPLER2D(s_skyboxTexture, 0);
#else
	SAMPLERCUBE(s_skyboxTexture, 0);
#endif

//atmosphere
uniform vec4 u_skyParams1;
#define u_atmosphereIntensity u_skyParams1.x
#define u_atmospherePreventBanding (u_skyParams1.y > 0.0)
//#define u_skyReflectionCubemapGeneration (u_skyParams1.z > 0.0)
uniform vec4 u_skyParams2;
uniform vec4/*vec3*/ u_skyLightDirection;
uniform vec4 u_skyPerezCoeff[5];
uniform vec4/*vec3*/ u_skyColorxyY;

//cubemap
uniform vec4/*vec3*/ u_skyCubemapMultiplier;
uniform vec4 u_skyCubemapRotation;
uniform vec4/*float*/ u_skyCubemapStretch;

vec2 toRadialCoords(vec3 coords)
{
	vec3 normalizedCoords = normalize(coords);
	float latitude = acos(normalizedCoords.y);
	float longitude = atan2(-normalizedCoords.z, normalizedCoords.x);
	vec2 sphereCoords = vec2(longitude, latitude) * vec2(0.5/PI, 1.0/PI);
	return vec2(0.25 - sphereCoords.x, sphereCoords.y);
}

vec3 Perez(vec3 A,vec3 B,vec3 C,vec3 D, vec3 E,float costeta, float cosgamma)
{
	float _1_costeta = 1.0 / costeta;
	float cos2gamma = cosgamma * cosgamma;
	float gamma = acos(cosgamma);
	vec3 f = (vec3_splat(1.0) + A * exp(B * _1_costeta)) * (vec3_splat(1.0) + C * exp(D * gamma) + E * cos2gamma);
	return f;
}

vec3 convertXYZ2RGB(vec3 _xyz)
{
	vec3 rgb;
	rgb.x = dot(vec3( 3.2404542, -1.5371385, -0.4985314), _xyz);
	rgb.y = dot(vec3(-0.9692660,  1.8760108,  0.0415560), _xyz);
	rgb.z = dot(vec3( 0.0556434, -0.2040259,  1.0572252), _xyz);
	return rgb;
}

vec3 toGamma(vec3 _rgb)
{
	return pow(abs(_rgb), vec3_splat(1.0/2.2) );
}

//uniformly distributed, normalized rand, [0, 1)
float nrand(vec2 n)
{
	return fract(sin(dot(n.xy, vec2(12.9898, 78.233)))* 43758.5453);
}

float n4rand_ss(vec2 n, vec4 parameters)
{
	float nrnd0 = nrand( n + 0.07*fract( parameters.w ) );
	float nrnd1 = nrand( n + 0.11*fract( parameters.w + 0.573953 ) );
	return 0.23*sqrt(-log(nrnd0+0.00001))*cos(2.0*3.141592*nrnd1)+0.5;
}

void main()
{
	vec3 texCoord0 = v_texCoord0;	
	////flip by Y for reflection cubemap. strange
	//if(u_skyReflectionCubemapGeneration)
	//	texCoord0.y = -texCoord0.y;
	
	vec3 color;
	BRANCH
	if(u_atmosphereIntensity < 1.0)
	{
		vec3 texCoord = flipCubemapCoords(mulQuat(u_skyCubemapRotation, texCoord0));
		//vec3 texCoord = flipCubemapCoords2(mulQuat(u_skyCubemapRotation, texCoord0));
		#ifdef USE_2D
			vec2 r = toRadialCoords(texCoord);
			r.y /= u_skyCubemapStretch.x;
			color = texture2DLod(s_skyboxTexture, r, 0).rgb;
		#else
			color = textureCubeLod(s_skyboxTexture, texCoord, 0).rgb;	
		#endif		
		color *= u_skyCubemapMultiplier.rgb;
	}
	else
		color = vec3_splat(0);		
	
	BRANCH
	if(u_atmosphereIntensity > 0.0)
	{		
		// x - sun size, y - sun bloom, z - exposition
		vec4 u_parameters = u_skyParams2;//vec4(0.02, 3.0, 0.1, 0.0);

		vec3 v_viewDir = normalize(texCoord0);

		vec3 lightDir = normalize(u_skyLightDirection.xyz);
		vec3 skyDir = vec3(0.0, 0.0, 1.0);//vec3(0.0, 1.0, 0.0);

		// Perez coefficients.
		vec3 A = u_skyPerezCoeff[0].xyz;
		vec3 B = u_skyPerezCoeff[1].xyz;
		vec3 C = u_skyPerezCoeff[2].xyz;
		vec3 D = u_skyPerezCoeff[3].xyz;
		vec3 E = u_skyPerezCoeff[4].xyz;

		float costeta = max(dot(v_viewDir, skyDir), 0.001);
		float cosgamma = clamp(dot(v_viewDir, lightDir), -0.9999, 0.9999);
		float cosgammas = dot(skyDir, lightDir);

		vec3 P = Perez(A,B,C,D,E, costeta, cosgamma);
		vec3 P0 = Perez(A,B,C,D,E, 1.0, cosgammas);

		vec3 skyColorxyY = u_skyColorxyY.xyz;
		//vec3 skyColorxyY = vec3(
		//	  u_skyLuminanceXYZ.x / (u_skyLuminanceXYZ.x+u_skyLuminanceXYZ.y + u_skyLuminanceXYZ.z)
		//	, u_skyLuminanceXYZ.y / (u_skyLuminanceXYZ.x+u_skyLuminanceXYZ.y + u_skyLuminanceXYZ.z)
		//	, u_skyLuminanceXYZ.y
		//	);

		vec3 Yp = skyColorxyY * P / P0;
		vec3 skyColorXYZ = vec3(Yp.x * Yp.z / Yp.y,Yp.z, (1.0 - Yp.x- Yp.y)*Yp.z/Yp.y);
		vec3 v_skyColor = convertXYZ2RGB(skyColorXYZ * u_parameters.z);

		
		float size2 = u_parameters.x * u_parameters.x;

		float dist = 2.0 * (1.0 - dot(normalize(v_viewDir), lightDir));
		float sun  = exp(-dist/ u_parameters.y / size2) + step(dist, size2);
		float sun2 = min(sun * sun, 1.0);
		vec3 color2 = v_skyColor + sun2;
		
		color2 = toGamma(color2);

		if(u_atmospherePreventBanding)
		{
			vec4 fragCoord = getFragCoord();
			
			float r = n4rand_ss(vec2(fragCoord.x, fragCoord.y), u_parameters);
			color2 += vec3(r, r, r) / 40.0;
		}

		color = mix( color, color2, u_atmosphereIntensity );
	}
	
	gl_FragColor = vec4(color, 1);
	//gl_FragColor = vec4(color * multiplier.rgb, 1);
}
