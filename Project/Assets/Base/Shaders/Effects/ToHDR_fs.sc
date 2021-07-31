$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	gl_FragColor = sourceColor;
}
