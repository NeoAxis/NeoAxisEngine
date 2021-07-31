$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

SAMPLER2D(s_sourceTexture, 0);

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	//color.rgb = ToneMap(color.rgb);  // linear color output

	float luma = dot(sqrt(sourceColor.rgb), vec3(0.299, 0.587, 0.114)); // compute luma
	//color.rgb = sqrt(color.rgb);
	//float luma = dot(color.rgb, float3(0.299, 0.587, 0.114)); // compute luma

	gl_FragColor = vec4(sourceColor.rgb, luma);
}
