$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

SAMPLER2D(s_source1Texture, 0);
SAMPLER2D(s_source2Texture, 1);
uniform vec4/*float*/ intensity;

void main()
{
	vec4 v1 = texture2D(s_source1Texture, v_texCoord0);
	vec4 v2 = texture2D(s_source2Texture, v_texCoord0);
	gl_FragColor = lerp(v1, v2, intensity.x);
}
