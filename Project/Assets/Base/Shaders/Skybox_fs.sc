$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

SAMPLERCUBE(s_skyboxTexture, 0);
uniform vec4/*vec3*/ multiplier;
uniform mat3 rotation;

void main()
{
	vec3 texCoord = mul(rotation, v_texCoord0);
	vec3 color = textureCubeLod(s_skyboxTexture, flipCubemapCoords(texCoord), 0).rgb;	
	gl_FragColor = vec4(color * multiplier.rgb, 1);
}
