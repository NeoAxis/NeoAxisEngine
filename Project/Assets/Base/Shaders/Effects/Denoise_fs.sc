$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

// Source:
// BSD-2-Clause license
// https://github.com/BrutPitt/glslSmartDeNoise

#include "../Common.sh"

SAMPLER2D( s_sourceTexture, 0 );
uniform vec4 u_denoiseParameters[ 2 ];
#define radius u_denoiseParameters[ 0 ].x
#define radQ u_denoiseParameters[ 0 ].y
#define invSigmaQx2 u_denoiseParameters[ 0 ].z
#define invSigmaQx2PI u_denoiseParameters[ 0 ].w
#define invThresholdSqx2 u_denoiseParameters[ 1 ].x
#define invThresholdSqrt2PI u_denoiseParameters[ 1 ].y

//#define sigma u_denoiseParameters.x
//#define kSigma u_denoiseParameters.y
//#define threshold u_denoiseParameters.z

//#define INV_SQRT_OF_2PI 0.39894228040143267793994605993439  // 1.0 / SQRT_OF_2PI
//#define INV_PI 0.31830988618379067153776752674503

void main()
{
	//float radius = round( kSigma * sigma );
	//float radQ = radius * radius;

	//float invSigmaQx2 = 0.5 / ( sigma * sigma );      // 1.0 / (sigma^2 * 2.0)
	//float invSigmaQx2PI = INV_PI * invSigmaQx2;    // // 1/(2 * PI * sigma^2)

	//float invThresholdSqx2 = 0.5 / ( threshold * threshold );     // 1.0 / (sigma^2 * 2.0)
	//float invThresholdSqrt2PI = INV_SQRT_OF_2PI / threshold;   // 1.0 / (sqrt(2*PI) * sigma)

	vec4 centrPx = texture2D( s_sourceTexture, v_texCoord0 );

	float zBuff = 0;
	vec4 aBuff = vec4_splat( 0 );
	vec2 size = vec2( textureSize( s_sourceTexture, 0 ) );

	vec2 d;
	for( d.x = -radius; d.x <= radius; d.x++ )
	{
		float pt = sqrt( radQ - d.x * d.x );       // pt = yRadius: have circular trend		
		for (d.y=-pt; d.y <= pt; d.y++)
		{
			float blurFactor = exp( -dot( d , d ) * invSigmaQx2 ) * invSigmaQx2PI;

			vec4 walkPx = texture2D( s_sourceTexture, v_texCoord0 + d / size );

			vec4 dC = walkPx - centrPx;
			float deltaFactor = exp( -dot( dC.rgb, dC.rgb ) * invThresholdSqx2 ) * invThresholdSqrt2PI * blurFactor;

			zBuff += deltaFactor;
			aBuff += deltaFactor * walkPx;
		}
	}
	gl_FragColor = aBuff / zBuff;
}
