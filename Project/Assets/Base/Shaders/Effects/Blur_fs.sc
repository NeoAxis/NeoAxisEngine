$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

#ifndef SAMPLE_COUNT
#define SAMPLE_COUNT 15
#endif

//#ifdef BLUR_SAMPLE2D_FROM_ARRAY
//	SAMPLER2DARRAY(s_sourceTexture, 0);
//#else
	SAMPLER2D(s_sourceTexture, 0);
//#endif

uniform vec4/*vec2*/ sampleOffsets[15];//SAMPLE_COUNT];
uniform vec4 sampleWeights[15];//SAMPLE_COUNT];

#ifdef BLEND_WITH_TEXTURE
	SAMPLER2D(s_blendWithTexture, 1);
	uniform vec4/*float*/ intensity;
#endif

void main()
{
	vec4 color = vec4_splat(0);
	for(int n = 0; n < SAMPLE_COUNT; n++)
	{
		vec4 value;		
//		#ifdef BLUR_SAMPLE2D_FROM_ARRAY
//			value = texture2DArray(s_sourceTexture, vec3(v_texCoord0 + sampleOffsets[n].xy, float(BLUR_SAMPLE2D_FROM_ARRAY)));
//		#else
			value = texture2D(s_sourceTexture, v_texCoord0 + sampleOffsets[n].xy);
//		#endif
		
		color += sampleWeights[n] * value;
	}
	
#ifdef BLEND_WITH_TEXTURE
	vec4 v1 = texture2D(s_blendWithTexture, v_texCoord0);
	gl_FragColor = lerp(v1, color, intensity.x);
#else	
	gl_FragColor = color;
#endif
}
