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
#define u_proceduralAtmosphere u_skyParams1.z
//#define u_skyReflectionCubemapGeneration (u_skyParams1.z > 0.0)
uniform vec4 u_skyParams2;
uniform vec4/*vec3*/ u_skyLightDirection;
uniform vec4 u_skyPerezCoeff[5];
uniform vec4/*vec3*/ u_skyColorxyY;
uniform vec4 u_skyStars1;
uniform vec4 u_skyStars2;

uniform vec4 u_skyAuroraParams;
#define u_proceduralAurora u_skyAuroraParams.x
#define u_proceduralAuroraFrequency u_skyAuroraParams.y
uniform vec4 u_skyAuroraColor1;
uniform vec4 u_skyAuroraColor2;

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

//!!!!

//source: https://stackblitz.com/edit/starry-skydome?file=StarrySkyShader.js

vec4 permute(vec4 x)
{
	return mod(((x*34.0)+1.0)*x, 289.0);
}
vec4 taylorInvSqrt(vec4 r)
{
	return 1.79284291400159 - 0.85373472095314 * r;
}
vec3 fade(vec3 t)
{
	return t*t*t*(t*(t*6.0-15.0)+10.0);
}

float cnoise(vec3 P)
{
	vec3 Pi0 = floor(P); // Integer part for indexing
	vec3 Pi1 = Pi0 + vec3_splat(1.0); // Integer part + 1
	Pi0 = mod(Pi0, 289.0);
	Pi1 = mod(Pi1, 289.0);
	vec3 Pf0 = fract(P); // Fractional part for interpolation
	vec3 Pf1 = Pf0 - vec3_splat(1.0); // Fractional part - 1.0
	vec4 ix = vec4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
	vec4 iy = vec4(Pi0.yy, Pi1.yy);
	vec4 iz0 = Pi0.zzzz;
	vec4 iz1 = Pi1.zzzz;

	vec4 ixy = permute(permute(ix) + iy);
	vec4 ixy0 = permute(ixy + iz0);
	vec4 ixy1 = permute(ixy + iz1);

	vec4 gx0 = ixy0 / 7.0;
	vec4 gy0 = fract(floor(gx0) / 7.0) - 0.5;
	gx0 = fract(gx0);
	vec4 gz0 = vec4_splat(0.5) - abs(gx0) - abs(gy0);
	vec4 sz0 = step(gz0, vec4_splat(0.0));
	gx0 -= sz0 * (step(0.0, gx0) - 0.5);
	gy0 -= sz0 * (step(0.0, gy0) - 0.5);

	vec4 gx1 = ixy1 / 7.0;
	vec4 gy1 = fract(floor(gx1) / 7.0) - 0.5;
	gx1 = fract(gx1);
	vec4 gz1 = vec4_splat(0.5) - abs(gx1) - abs(gy1);
	vec4 sz1 = step(gz1, vec4_splat(0.0));
	gx1 -= sz1 * (step(0.0, gx1) - 0.5);
	gy1 -= sz1 * (step(0.0, gy1) - 0.5);

	vec3 g000 = vec3(gx0.x,gy0.x,gz0.x);
	vec3 g100 = vec3(gx0.y,gy0.y,gz0.y);
	vec3 g010 = vec3(gx0.z,gy0.z,gz0.z);
	vec3 g110 = vec3(gx0.w,gy0.w,gz0.w);
	vec3 g001 = vec3(gx1.x,gy1.x,gz1.x);
	vec3 g101 = vec3(gx1.y,gy1.y,gz1.y);
	vec3 g011 = vec3(gx1.z,gy1.z,gz1.z);
	vec3 g111 = vec3(gx1.w,gy1.w,gz1.w);

	vec4 norm0 = taylorInvSqrt(vec4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
	g000 *= norm0.x;
	g010 *= norm0.y;
	g100 *= norm0.z;
	g110 *= norm0.w;
	vec4 norm1 = taylorInvSqrt(vec4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
	g001 *= norm1.x;
	g011 *= norm1.y;
	g101 *= norm1.z;
	g111 *= norm1.w;

	float n000 = dot(g000, Pf0);
	float n100 = dot(g100, vec3(Pf1.x, Pf0.yz));
	float n010 = dot(g010, vec3(Pf0.x, Pf1.y, Pf0.z));
	float n110 = dot(g110, vec3(Pf1.xy, Pf0.z));
	float n001 = dot(g001, vec3(Pf0.xy, Pf1.z));
	float n101 = dot(g101, vec3(Pf1.x, Pf0.y, Pf1.z));
	float n011 = dot(g011, vec3(Pf0.x, Pf1.yz));
	float n111 = dot(g111, Pf1);

	vec3 fade_xyz = fade(Pf0);
	vec4 n_z = mix(vec4(n000, n100, n010, n110), vec4(n001, n101, n011, n111), fade_xyz.z);
	vec2 n_yz = mix(n_z.xy, n_z.zw, fade_xyz.y);
	float n_xyz = mix(n_yz.x, n_yz.y, fade_xyz.x);
	return 2.2 * n_xyz;
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

		//stars, aurora
		BRANCH
		if( u_proceduralAtmosphere < 1.0 )
		{
			//!!!!to properties?
			vec3 noiseOffset = vec3( 100.01, 100.01, 100.01 );
			
			//!!!!when u_proceduralStars == 1 no sense to calculate day sky
			
			//float skyRadius = 1;//500.01;
	
			float starSize = u_skyStars1.x; //0.01;
			float starDensity = u_skyStars1.y; //0.09;
			float clusterStrength = u_skyStars1.z; //0.1;//0.2;
			float clusterSize = u_skyStars1.w; //0.2;

			vec3 auroraColor = vec3_splat( 0 );
			BRANCH
			if( u_proceduralAurora > 0.0 )
			{
				float freq = u_proceduralAuroraFrequency; //1.1;// / skyRadius;
				float noise = cnoise( v_viewDir * freq );
				auroraColor = mix( u_skyAuroraColor1.rgb, u_skyAuroraColor2.rgb, noise ) * u_proceduralAurora;
			}

			float scaledClusterSize = ( 1.0 / clusterSize ); // / skyRadius;
			float scaledStarSize = ( 1.0 / starSize ); // / skyRadius;

			float cs = pow( abs( cnoise( scaledClusterSize * v_viewDir + noiseOffset ) ), 1.0 / clusterStrength ) + cnoise( scaledStarSize * v_viewDir );
			float c = clamp( pow( abs( cs ), 1.0 / starDensity ), 0.0, 1.0 );
			vec4 starColor = vec4( c, c, c, 1.0 ) * u_skyStars2;

			color2 = mix( auroraColor + starColor.rgb, color2, u_proceduralAtmosphere );
		}		
		
		color = mix( color, color2, u_atmosphereIntensity );
	}
	
	gl_FragColor = vec4(color, 1);
	//gl_FragColor = vec4(color * multiplier.rgb, 1);
}
