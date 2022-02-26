$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ intensity;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec4 color = float4(1,1,1,1);

	gl_FragColor = lerp(sourceColor, color, intensity.x);
	//!!!!alpha?
	//float3 color2 = lerp(sourceColor, color, intensity);
	//return float4(color2, 1.0);
}
