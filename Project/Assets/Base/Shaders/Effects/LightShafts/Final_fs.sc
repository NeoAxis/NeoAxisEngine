$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_blurTexture, 1);
uniform vec4/*float*/ intensity;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	vec4 blurColor = texture2D(s_blurTexture, v_texCoord0);
	
	vec4 color = sourceColor;
	color.rgb += blurColor.rgb * intensity.x;

	gl_FragColor = color;
}
