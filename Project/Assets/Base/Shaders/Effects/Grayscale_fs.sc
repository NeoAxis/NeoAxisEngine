$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ intensity;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	float value = dot(sourceColor.xyz, vec3(0.3, 0.59, 0.11));
	gl_FragColor = lerp(sourceColor, vec4_splat(value), intensity.x);
}
