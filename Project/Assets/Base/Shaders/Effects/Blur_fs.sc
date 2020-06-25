$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

#ifndef SAMPLE_COUNT
#define SAMPLE_COUNT 15
#endif

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*vec2*/ sampleOffsets[15];//SAMPLE_COUNT];
uniform vec4 sampleWeights[15];//SAMPLE_COUNT];

void main()
{
	vec4 color = vec4_splat(0);
	for(int n = 0; n < SAMPLE_COUNT; n++)
		color += sampleWeights[n] * texture2D(s_sourceTexture, v_texCoord0 + sampleOffsets[n].xy);
	gl_FragColor = color;
}
