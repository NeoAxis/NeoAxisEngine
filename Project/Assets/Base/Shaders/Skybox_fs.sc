$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

#ifdef USE_2D
	SAMPLER2D(s_skyboxTexture, 0);
#else
	SAMPLERCUBE(s_skyboxTexture, 0);
#endif

uniform vec4/*vec3*/ multiplier;
uniform mat3 rotation;
uniform vec4/*float*/ stretch;

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
	vec3 texCoord = flipCubemapCoords2(mul(rotation, v_texCoord0));
	#ifdef USE_2D
		vec2 r = toRadialCoords(texCoord);
		r.y /= stretch.x;
		color = texture2DLod(s_skyboxTexture, r, 0).rgb;
	#else
		color = textureCubeLod(s_skyboxTexture, texCoord, 0).rgb;	
	#endif
	
	gl_FragColor = vec4(color * multiplier.rgb, 1);
}
