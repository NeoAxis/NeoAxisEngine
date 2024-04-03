$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"
#ifdef HLSL
	#include "../GICommon.sh"
#endif
#include "../UniformsFragment.sh"
#include "../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);

#ifdef GI_GRID
	USAMPLER3D(s_giGrid1, 1);
	SAMPLER3D(s_giGrid2, 2);
#elif defined(LIGHT_GRID)
	SAMPLER3D(s_lightGrid, 1);
	SAMPLER2D(s_lightsTexture, 2);
#elif defined(SHADOW_POINT_LIGHT)
	SAMPLERCUBEARRAY(s_showTexture, 1); //SAMPLERCUBE(s_showTexture, 1);
#elif defined(SHADOW_DIRECTIONAL_LIGHT)
	SAMPLER2DARRAY(s_showTexture, 1);
#elif defined(SHADOW_SPOT_LIGHT)
	SAMPLER2DARRAY(s_showTexture, 1);
#else
	SAMPLER2D(s_showTexture, 1);
#endif

//!!!!rename
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ nearClipDistance;
uniform vec4/*float*/ farClipDistance;
uniform vec4/*int*/ mode;
uniform vec4/*float*/ multiplier;

#ifdef GI_GRID
	uniform vec4 giRayCastInfo[ 7 ];
	#include "../GIGrid.sh"	
#endif

//////////////////////////////////////////////////

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec4 showColor;

#ifdef GI_GRID

	//!!!!ortho camera

	//get world position of pixel
	float rawDepth = 0.5;//texture2D( s_depthTexture, v_texCoord0 ).x;
	vec3 worldPosition = reconstructWorldPosition( u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, v_texCoord0, rawDepth );
	
	//get world ray
	vec3 rayStart = u_viewportOwnerCameraPosition;
	vec3 rayDirection = normalize( worldPosition - u_viewportOwnerCameraPosition );

	//get color
	vec3 rayCastColor;
	//float rayCastDistance;
	bool rayCastResult = giRayCast( rayStart, rayDirection, rayCastColor, true, mode.x == 12.0 );//, rayCastDistance );	
	//normal
	if( rayCastResult && mode.x == 12.0 )
		rayCastColor = rayCastColor * 0.5 + vec3_splat( 0.5 );
	showColor = vec4( rayCastColor, 1 );
	//showColor = vec4( giRayCast( rayStart, rayDirection ), 1 );

#elif defined( LIGHT_GRID )

	vec3 worldPosition = vec3( ( v_texCoord0 * 2.2 - vec2_splat( 1.1 ) ) * farClipDistance.x, 0 );
	//vec3 worldPosition = vec3( ( v_texCoord0 * 2.0 - vec2_splat( 1 ) ) * farClipDistance.x, 0 );

	vec2 lightGridIndex = ( worldPosition.xy - vec2_splat( d_lightGridStart ) ) / d_lightGridCellSize;
	vec4 lightGrid0 = texelFetch( s_lightGrid, uvec3( uvec2( lightGridIndex ), 0 ), 0 );
	bool lightGridEnabled2 = d_lightGridEnabled > 0.0 && lightGridIndex.x >= 0.0 && lightGridIndex.x < d_lightGridSize && lightGridIndex.y >= 0.0 && lightGridIndex.y < d_lightGridSize && lightGrid0.x >= 0.0;

	if( lightGridEnabled2 )
	{
		float factor = saturate( lightGrid0.x / 10.0 );
		showColor = vec4( 1.0 - factor, 1, 1.0 - factor, 1 );
	}
	else
		showColor = vec4( 0, 0, 0, 1 );
	
#elif defined( SHADOW_POINT_LIGHT )

	float h = v_texCoord0.x * PI * 2.0;
	float v = (0.5 - v_texCoord0.y) * PI;
	vec3 coord = flipCubemapCoords(sphericalDirectionGetVector(vec2(h,v)));
	
	float shadowMapIndex = 0.0;
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		vec4 shadowValue = textureCubeArrayLod( s_showTexture, vec4( coord, shadowMapIndex ), 0.0 );
		showColor = vec4_splat( unpackRgbaToFloat( shadowValue ) );
	#else
		float shadowFactor = textureCubeArrayLod( s_showTexture, vec4( coord, shadowMapIndex ), 0.0 ).r;
		showColor = vec4_splat( shadowFactor );
	#endif
	/*
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		showColor = unpackRgbaToFloat( textureCube(s_showTexture, coord) ).xxxx;
	#else
		showColor = textureCube(s_showTexture, coord).xxxx;
	#endif
	*/
	
#elif defined( SHADOW_DIRECTIONAL_LIGHT )

	float index = 0.0;
	if(mode.x >= 3.0 && mode.x <= 6.0)
		index = float(round(mode.x - 3.0));

	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		showColor = vec4_splat( unpackRgbaToFloat( texture2DArray( s_showTexture, vec3( v_texCoord0, index ) ) ) );
	#else
		showColor = texture2DArray( s_showTexture, vec3( v_texCoord0, index ) ).xxxx;
	#endif
	
#elif defined( SHADOW_SPOT_LIGHT )

	float shadowMapIndex = 0.0;
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		showColor = vec4_splat( unpackRgbaToFloat( texture2DArray(s_showTexture, vec3( v_texCoord0, shadowMapIndex ) ) ) );
	#else
		showColor = texture2DArray( s_showTexture, vec3( v_texCoord0, shadowMapIndex ) ).xxxx;
	#endif

	/*
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		showColor = unpackRgbaToFloat( texture2D(s_showTexture, v_texCoord0) ).xxxx;
	#else
		showColor = texture2D(s_showTexture, v_texCoord0).xxxx;
	#endif
	*/

#else
	
	showColor = texture2D(s_showTexture, v_texCoord0);

#endif	
	
	vec4 color;

	BRANCH
	if(mode.x == 0.0)
	{
		//normal
		color = showColor;
	}
	else if(mode.x == 1.0)
	{
		//depth
		float depth = getDepthValue(showColor.r, nearClipDistance.x, farClipDistance.x);		
		float v = saturate(depth / farClipDistance.x * multiplier.x);
		//float v = saturate(depth * multiplier.x);
		color = vec4(v, v, v, 1.0);
	}
	else if(mode.x == 2.0)
	{
		//motion vector
		color = showColor * multiplier.x;
	}
	else if(mode.x == 9.0 || mode.x == 10.0 || mode.x == 11.0 || mode.x == 12.0) //LightGrid, GlobalIlluminationGridAccelerated, GlobalIlluminationGrid, GlobalIlluminationNormal
	{
		color = showColor;
	}	
	/*else if( mode.x == 9.0 )
	{
		//object id		

#ifdef GLSL
		const float colors[ 8 ] = float[ 8 ] (
			vec3( 1, 1, 0 ),
			vec3( 1, 0, 0 ),
			vec3( 0, 1, 0 ),
			vec3( 0.75, 0.75, 0.75 ),
			vec3( 1, 1, 1 ),
			vec3( 0, 1, 1 ),
			vec3( 1, 0, 1 ),
			vec3( 0.8, 0.4, 0.1 ) );
#else
		const vec3 colors[ 8 ] = {
			vec3( 1, 1, 0 ),
			vec3( 1, 0, 0 ),
			vec3( 0, 1, 0 ),
			vec3( 0.75, 0.75, 0.75 ),
			vec3( 1, 1, 1 ),
			vec3( 0, 1, 1 ),
			vec3( 1, 0, 1 ),
			vec3( 0.8, 0.4, 0.1 ) };
#endif
		
		int objectId = int( showColor.z );
		color = vec4( colors[ objectId % 8 ], 1 );
	}*/
	else
	{
		//shadow
		color = vec4(showColor.x, showColor.x, showColor.x, 1.0) * multiplier.x;
	}

	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
