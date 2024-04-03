$input a_position, a_color0, a_texcoord0
$output v_color0, v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

uniform vec4 u_canvasColorMultiplier;
uniform vec4 u_canvasOcclusionDepthCheck;

#ifndef MOBILE
	SAMPLER2D( s_depthTexture, 1 );
	#include "FragmentFunctions.sh"
#endif

void main()
{
	//!!!!
	//gl_Position = vec4(a_position.x, -a_position.y, 0.0, 1.0);
	//gl_Position = mul(u_viewProj, vec4(a_position.x, 1.0 - a_position.y, 0.0, 1.0) );
	
	gl_Position = vec4(a_position, 1.0);
	////gl_Position = vec4(a_position.x * 2 - 1, (a_position.y * 2 - 1) * -1, 0, 1.0);

	//gl_Position = mul(u_modelViewProj, vec4(a_position, 1.0) );
	v_texCoord0 = a_texcoord0;
	v_color0 = a_color0 * u_canvasColorMultiplier;

	//depth texture check. for lens flares
#ifndef MOBILE
	BRANCH
	if( u_canvasOcclusionDepthCheck.w > 0.0 )
	{
		float screenSize = u_canvasOcclusionDepthCheck.z;
		
		vec2 depthTextureSize = vec2( textureSize( s_depthTexture, 0 ) );
		float aspectRatio = depthTextureSize.x / depthTextureSize.y;
		vec2 scale = vec2( screenSize / aspectRatio, screenSize );
		
		const int sampleCount = 128;

		int passed = 0;
		//UNROLL
		for( int n = 0; n < sampleCount; n++ )
		{
			vec2 texCoord = u_canvasOcclusionDepthCheck.xy + vogelDiskSample( n, sampleCount, 0.0 ) * scale;
			
			float rawDepth = texelFetch( s_depthTexture, ivec2( texCoord * depthTextureSize ), 0 ).r;
			vec3 worldPosition = reconstructWorldPosition( u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, u_canvasOcclusionDepthCheck.xy, rawDepth );
			
			float distanceToCamera = length( worldPosition - u_viewportOwnerCameraPosition );
			
			if( u_canvasOcclusionDepthCheck.w < distanceToCamera )
				passed++;
		}
		
		float factor = float( passed ) / float( sampleCount );
		
		//apply factor to color
		v_color0.w *= factor;

		//discard
		if( factor <= 0.001 )
			gl_Position.x = 0.0 / 0.0;
	}		
#endif
	
}
