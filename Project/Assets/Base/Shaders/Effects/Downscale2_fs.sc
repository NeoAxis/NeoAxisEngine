$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*vec2*/ sourceSizeInv;

void main()
{
	vec4 v00 = texture2D(s_sourceTexture, v_texCoord0 + vec2(-0.5,-0.5) * sourceSizeInv.xy);
	vec4 v01 = texture2D(s_sourceTexture, v_texCoord0 + vec2(-0.5,0.5) * sourceSizeInv.xy);
	vec4 v10 = texture2D(s_sourceTexture, v_texCoord0 + vec2(0.5,-0.5) * sourceSizeInv.xy);
	vec4 v11 = texture2D(s_sourceTexture, v_texCoord0 + vec2(0.5,0.5) * sourceSizeInv.xy);
	gl_FragColor= (v00 + v01 + v10 + v11) * 0.25;
}
