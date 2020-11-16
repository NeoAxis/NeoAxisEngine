$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

#ifdef USE_2D
	SAMPLER2D(s_skyboxTexture, 0);
#else
	SAMPLERCUBE(s_skyboxTexture, 0);
#endif

uniform vec4/*vec3*/ multiplier;
uniform mat3 rotation;
uniform vec4/*float*/ flipCubemap;

vec2 toRadialCoords(vec3 coords)
{
	vec3 normalizedCoords = normalize(coords);
	float latitude = acos(normalizedCoords.y);
	float longitude = atan2(-normalizedCoords.z, normalizedCoords.x);
	vec2 sphereCoords = vec2(longitude, latitude) * vec2(0.5/PI, 1.0/PI);
	return vec2(0.25 - sphereCoords.x, sphereCoords.y);
}

void main()
{
	vec3 color;
	
	#ifdef USE_2D
		vec3 texCoord = flipCubemapCoords(mul(rotation, v_texCoord0));
		vec2 texCoord2 = toRadialCoords(texCoord);
		color = texture2DLod(s_skyboxTexture, texCoord2, 0).rgb;
	#else
		vec3 texCoord = mul(rotation, v_texCoord0);
		texCoord.y *= flipCubemap.x;
		color = textureCubeLod(s_skyboxTexture, flipCubemapCoords(texCoord), 0).rgb;	
	#endif

	gl_FragColor = vec4(color * multiplier.rgb, 1);
}
