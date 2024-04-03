$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"
#include "../UniformsFragment.sh"
#include "../FragmentFunctions.sh"

#ifndef SAMPLE_COUNT
#define SAMPLE_COUNT 15
#endif

//#ifdef BLUR_SAMPLE2D_FROM_ARRAY
//	SAMPLER2DARRAY(s_sourceTexture, 0);
//#else
SAMPLER2D(s_sourceTexture, 0);
//#endif

#ifdef DEPTH_CHECK
	SAMPLER2D(s_depthTexture, 2);
#endif
#ifdef NORMAL_CHECK
	SAMPLER2D(s_normalTexture, 3);
#endif

uniform vec4/*vec2*/ u_blurSampleOffsets[15];//SAMPLE_COUNT];
uniform vec4 u_blurSampleWeights[15];//SAMPLE_COUNT];

uniform vec4 u_blurParameters;
#define u_depthThreshold u_blurParameters.x
#define u_normalThreshold u_blurParameters.y


#ifdef BLEND_WITH_TEXTURE
	SAMPLER2D(s_blendWithTexture, 1);
	uniform vec4/*float*/ intensity;
#endif

void main()
{
	vec4 color = vec4_splat( 0 );
	float weightsAdded = 0.0;
	
#if defined( DEPTH_CHECK ) || defined( NORMAL_CHECK )
	//with depth, normal check
	
	#ifdef DEPTH_CHECK
	float rawDepth = texture2DLod( s_depthTexture, v_texCoord0, 0 ).r;
	float sourceDepth = getDepthValue( rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance );
	#endif

	#ifdef NORMAL_CHECK
	vec3 sourceNormal = expand( texture2DLod( s_normalTexture, v_texCoord0, 0 ).xyz );
	#endif
		
	for( int n = 0; n < SAMPLE_COUNT; n++ )
	{
		vec2 texCoord = v_texCoord0 + u_blurSampleOffsets[ n ].xy;
		
		bool acceptByDepth = true;
		bool acceptByNormal = true;
		
	#ifdef DEPTH_CHECK
		float rawDepth2 = texture2DLod( s_depthTexture, texCoord, 0 ).r;
		float depth = getDepthValue( rawDepth2, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance );
		acceptByDepth = ( depth / sourceDepth < u_depthThreshold ) && ( sourceDepth / depth < u_depthThreshold );
	#endif
	
	#ifdef NORMAL_CHECK
		vec3 normal = expand( texture2DLod( s_normalTexture, texCoord, 0 ).xyz );
		float angle = vectorsAngle( sourceNormal, normal );
		acceptByNormal = angle < u_normalThreshold;
	#endif

		if( acceptByDepth && acceptByNormal )
		{
			vec4 value = texture2D( s_sourceTexture, texCoord );
			
			float weight = u_blurSampleWeights[ n ].x;
//#ifdef ALPHA_AS_WEIGHT
//			weight *= value.a;
//#endif
			
			color += weight * value;
			weightsAdded += weight;
			
			//color += u_blurSampleWeights[ n ].x * value;
			//weightsAdded += u_blurSampleWeights[ n ].x;
		}
	}

	//normalize
	color /= weightsAdded;

#else
	//without depth, normal check

	for( int n = 0; n < SAMPLE_COUNT; n++ )
	{
		vec4 value = texture2D( s_sourceTexture, v_texCoord0 + u_blurSampleOffsets[ n ].xy );
		
		float weight = u_blurSampleWeights[ n ].x;
//#ifdef ALPHA_AS_WEIGHT
//			weight *= value.a;
//#endif
		
		color += weight * value;
		//weightsAdded += weight;
		
		//color += u_blurSampleWeights[ n ] * texture2D( s_sourceTexture, v_texCoord0 + u_blurSampleOffsets[ n ].xy );
	}

//#ifdef ALPHA_AS_WEIGHT
//	//normalize
//	color /= weightsAdded;
//#endif
	
	//		vec4 value;		
	//		#ifdef BLUR_SAMPLE2D_FROM_ARRAY
	//			value = texture2DArray(s_sourceTexture, vec3(v_texCoord0 + sampleOffsets[n].xy, float(BLUR_SAMPLE2D_FROM_ARRAY)));
	//		#else
	//			value = texture2D(s_sourceTexture, v_texCoord0 + sampleOffsets[n].xy);
	//		#endif
	//		color += sampleWeights[n] * value;
	
#endif
	
	
#ifdef BLEND_WITH_TEXTURE
	vec4 v1 = texture2D(s_blendWithTexture, v_texCoord0);
	gl_FragColor = lerp(v1, color, intensity.x);
#else	
	gl_FragColor = color;
#endif
}
