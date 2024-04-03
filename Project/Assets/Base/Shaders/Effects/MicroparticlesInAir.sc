// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common/bgfx_compute.sh"
#include "../Common.sh"
#include "../UniformsFragment.sh"
#include "../FragmentFunctions.sh"

//!!!!when ldr
IMAGE2D_WO( s_addTexture, rgba16f, 0 );
//IMAGE2D_RW( s_sceneTexture, rgba16f, 0 );

uniform vec4 u_microparticlesInAirColor;
uniform vec4 u_microparticlesInAirParams;
#define u_debugEdges ( u_microparticlesInAirParams.z > 0.0 )
#define u_depthThreshold u_microparticlesInAirParams.w

SAMPLER2D(s_depthTexture, 3);
SAMPLER2D(s_lightsTexture, 4);
#ifdef GLOBAL_LIGHT_GRID
	SAMPLER3D(s_lightGrid, 15);
#endif

#ifdef SHADOW_MAP
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		SAMPLER2DARRAY(s_shadowMapShadowDirectional, 5);
	#else
		SAMPLER2DARRAYSHADOW(s_shadowMapShadowDirectional, 5);
	#endif
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		SAMPLER2DARRAY(s_shadowMapShadowSpot, 6);
	#else
		SAMPLER2DARRAYSHADOW(s_shadowMapShadowSpot, 6);
	#endif
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		SAMPLERCUBEARRAY(s_shadowMapShadowPoint, 7);
	#else
		SAMPLERCUBEARRAYSHADOW(s_shadowMapShadowPoint, 7);
	#endif
#endif

#ifdef GLOBAL_LIGHT_MASK
	SAMPLER2DARRAY(s_lightMaskDirectional, 12);
	SAMPLER2DARRAY(s_lightMaskSpot, 13);
	SAMPLERCUBEARRAY(s_lightMaskPoint, 14);
#endif

#ifdef SHADOW_MAP
	#define SHADOW_MAP_MICROPARTICLES_IN_AIR 1
	#include "../ShadowReceiverFunctions.sh"
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef QUALITY_OPTIMIZED

NUM_THREADS(16, 16, 1)
void main()
{
	uvec2 pixelSize = uvec2( imageSize( s_addTexture ) );
		
	uvec2 blockIndex = gl_GlobalInvocationID.xy;

	//fetch without v_texCoord0? but depth buffer can have different size
	
	float minDepth = 1000000.0;
	float maxDepth = 0.0;
	bool foundPixel = false;
	
	for( uint y = 0u; y < 4u; y++ )
	{
		for( uint x = 0u; x < 4u; x++ )
		{
			uvec2 pixelIndex = blockIndex * 4u + uvec2( x, y );			
			if( pixelIndex.x < pixelSize.x && pixelIndex.y < pixelSize.y )
			{
				//with add 0.5 - equal same as pixel shader
				vec2 v_texCoord0 = ( vec2( pixelIndex ) + vec2_splat( 0.5 ) ) / vec2( pixelSize );
				//vec2 v_texCoord0 = vec2( pixelIndex ) / vec2( pixelSize );
				
				//vec4 fragCoord = vec4( pixelIndex.x, pixelIndex.y, 0, 0 );
				
				float rawDepth = texture2DLod(s_depthTexture, v_texCoord0, 0).r;
				//vec3 worldPosition2 = reconstructWorldPosition(u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, v_texCoord0, rawDepth);
				//float cameraDistance2 = length( worldPosition2 );
				float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
				
				minDepth = min( minDepth, depth );
				maxDepth = max( maxDepth, depth );
				foundPixel = true;
			}
		}
	}
	
	if( !foundPixel )
		return;
	
	bool onEdge = maxDepth / minDepth > u_depthThreshold;

	
	//debug edges
#ifdef GLOBAL_DEBUG_MODE
	BRANCH
	if( u_debugEdges )
	{
		vec4 c = onEdge ? vec4( 1, 0, 0, 0 ) : vec4( 0, 1, 0, 0 );
		UNROLL
		for( uint y = 0u; y < 4u; y++ )
		{
			UNROLL
			for( uint x = 0u; x < 4u; x++ )
			{
				uvec2 pixelIndex = blockIndex * 4u + uvec2( x, y );
				imageStore( s_addTexture, pixelIndex, c );
			}
		}	
		return;
	}
#endif	


	vec4 previousResult = vec4_splat( 0 );
	float previousDepth = 0.0;
	
	LOOP
	for( uint y = 0u; y < 4u; y++ )
	{
		LOOP
		for( uint x = 0u; x < 4u; x++ )
		{
			uvec2 pixelIndex = blockIndex * 4u + uvec2( x, y );
			if( pixelIndex.x < pixelSize.x && pixelIndex.y < pixelSize.y )
			{
				//move closer to center
				if( !onEdge )
					pixelIndex += uvec2( 1u, 1u );
		
				//with add 0.5 - equal same as pixel shader
				vec2 v_texCoord0 = ( vec2( pixelIndex ) + vec2_splat( 0.5 ) ) / vec2( pixelSize );
				//vec2 v_texCoord0 = vec2( pixelIndex ) / vec2( pixelSize );
				
				float rawDepth = texture2DLod(s_depthTexture, v_texCoord0, 0).r;
				float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);

				//reuse previous pixel value if their depths are close
				BRANCH
				if( onEdge )
				{
					if( x % 2 != 0 )
					{
						if( previousDepth / depth < u_depthThreshold || depth / previousDepth < u_depthThreshold )
						{
							imageStore( s_addTexture, pixelIndex, previousResult );
							continue;
						}							
					}
				}

				vec3 worldPosition2 = reconstructWorldPosition(u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, v_texCoord0, rawDepth);
				float cameraDistance2 = length( worldPosition2 );
				
				vec4 fragCoord = vec4( pixelIndex.x, pixelIndex.y, 0, 0 );
				
				vec3 fromCamera = normalize( worldPosition2 - u_viewportOwnerCameraPosition );
				
				vec3 resultColor = vec3_splat(0);

				const int distanceSteps = STEPS;//64;

				float stepSize = u_microparticlesInAirParams.x;//0.2;
				float multiply = u_microparticlesInAirParams.y;//1.05;
				
				//!!!!not best
				float maxBySteps = stepSize * float( distanceSteps );
				if( maxBySteps > cameraDistance2 )
				{
					stepSize *= cameraDistance2 / maxBySteps;
					multiply = 1.0;
				}
				
				vec2 rand = random( fragCoord.xy );
				//randomize start
				float currentDistance = rand.x * rand.y * stepSize; //float currentDistance = stepSize;
				vec3 previousPosition = vec3_splat(0);
				
				LOOP
				for( int nDistanceStep = 0; nDistanceStep < distanceSteps; nDistanceStep++ )
				{
					vec3 worldPosition = fromCamera * min( currentDistance, cameraDistance2 );
					
					//iterate lights
					
					#ifdef GLOBAL_LIGHT_GRID
						vec2 lightGridIndex = ( worldPosition.xy - vec2_splat( d_lightGridStart ) ) / d_lightGridCellSize;
						vec4 lightGrid0 = texelFetch( s_lightGrid, ivec3( ivec2( lightGridIndex ), 0 ), 0 );
						bool lightGridEnabled2 = d_lightGridEnabled > 0.0 && lightGridIndex.x >= 0 && lightGridIndex.x < d_lightGridSize && lightGridIndex.y >= 0 && lightGridIndex.y < d_lightGridSize && lightGrid0.x >= 0.0;
						int lightCount = lightGridEnabled2 ? ( int( lightGrid0.x ) + 1 ) : d_lightCount;
					#else
						int lightCount = d_lightCount;
					#endif
					
					LOOP
					for( int lightCounter = 1; lightCounter < lightCount; lightCounter++ )
					{
						int nLight;
						#ifdef GLOBAL_LIGHT_GRID
							if( lightGridEnabled2 )
								nLight = int( texelFetch( s_lightGrid, ivec3( ivec2( lightGridIndex ), lightCounter / 4 ), 0 )[ lightCounter % 4 ] );
							else
						#endif
							nLight = lightCounter;

						//!!!!skip by bool flag
						
						//cull by bounding sphere
						float lightDistance = length( d_lightPosition - worldPosition );
						if( lightDistance > d_lightBoundingRadius )
							continue;

						int lightType = int(d_lightType);
						
						//objectLightAttenuation and start distance
						float objectLightAttenuation = 1.0;
						BRANCH
						if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT || lightType == ENUM_LIGHT_TYPE_POINT )
						{
							objectLightAttenuation = getLightAttenuation2(d_lightAttenuation, lightDistance);
							if(objectLightAttenuation == 0.0 || lightDistance < d_lightStartDistance)
								continue;
						}

						//lightWorldDirection
						vec3 lightWorldDirection;
						if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
							lightWorldDirection = -d_lightDirection;
						else
							lightWorldDirection = normalize(d_lightPosition - worldPosition);

						BRANCH
						if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
						{
							// factor in spotlight angle
							float rho0 = saturate(dot(-d_lightDirection, lightWorldDirection));
							// factor = (rho - cos(outer/2)) / (cos(inner/2) - cos(outer/2)) ^ falloff 
							float spotFactor0 = saturate(pow(saturate(rho0 - d_lightSpot.y) / (d_lightSpot.x - d_lightSpot.y), d_lightSpot.z));
							if(spotFactor0 == 0.0)
								continue;
							objectLightAttenuation *= spotFactor0;
						}
						
						//lightMaskMultiplier
						vec3 lightMaskMultiplier = vec3(1,1,1);
					#ifdef GLOBAL_LIGHT_MASK
						float lightMaskIndex = d_lightMaskIndex;
						BRANCH
						if( lightMaskIndex >= 0.0 )
						{
							BRANCH
							if( lightType == ENUM_LIGHT_TYPE_POINT )
							{
								vec3 dir = normalize( worldPosition - d_lightPosition );
								dir = mul( d_lightMaskMatrix, vec4( dir, 0 ) ).xyz;
								lightMaskMultiplier = textureCubeArrayLod( s_lightMaskPoint, vec4( dir, lightMaskIndex ), 0.0 ).rgb;
							}
							else if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
							{
								vec4 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) );
								lightMaskMultiplier = texture2DArrayLod( s_lightMaskSpot, vec3( texCoord.xy / texCoord.w, lightMaskIndex ), 0 ).rgb;				
							}
							else
							{
								vec2 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) ).xy;				
								lightMaskMultiplier = texture2DArrayLod( s_lightMaskDirectional, vec3( texCoord, lightMaskIndex ), 0 ).rgb;
							}
						}
					#endif

						//shadows
						float shadowMultiplier = 1.0;
					#ifdef SHADOW_MAP
						float shadowMapIndex = d_shadowMapIndex;
						BRANCH
						if( shadowMapIndex >= 0.0 )
						{
							vec3 normalDummy = vec3_splat(0);//lightWorldDirection;

							vec3 plane = u_viewportOwnerCameraDirection;
							//easy to calculate projective depth
							float cascadeDepth = dot( plane, worldPosition );
							
							//vec3 left = cross( u_viewportOwnerCameraDirection, u_viewportOwnerCameraUp );
							//vec3 plane;//vec4 plane;
							//plane.xyz = normalize( cross( u_viewportOwnerCameraUp, left ) );
							////camera position is equal zero plane.w = -( dot( plane.xyz, vec3_splat( 0 ) ) );
							//float cascadeDepth = dot( plane, vec4( worldPosition, 1.0 ) );
							
							//!!!!
							//for fading by distance
							float cameraDistance3 = 0.0;//currentDistance
							
							shadowMultiplier = getShadowMultiplierMulti( worldPosition, cameraDistance3, cascadeDepth, lightWorldDirection, normalDummy, v_texCoord0, fragCoord, lightType, shadowMapIndex, nLight );
						}		
					#endif
					
						//light color
						vec3 lightColor = ( d_lightPower.rgb * 10000.0 ) * u_cameraExposure * objectLightAttenuation * lightMaskMultiplier * shadowMultiplier;
					
						float stepDistance = length( worldPosition - previousPosition );

						//Simple shading model
						resultColor += lightColor * stepDistance;
					}

					if( currentDistance > cameraDistance2 )
						break;
					currentDistance = currentDistance * multiply + stepSize;
					previousPosition = worldPosition;
				}
				
				vec4 result = vec4( resultColor * u_microparticlesInAirColor.xyz, 0.0 );

				BRANCH
				if( !onEdge )
				{
					UNROLL
					for( uint yy = 0u; yy < 4u; yy++ )
					{
						UNROLL
						for( uint xx = 0u; xx < 4u; xx++ )
						{
							uvec2 pixelIndex = blockIndex * 4u + uvec2( xx, yy );
							imageStore( s_addTexture, pixelIndex, result );
						}
					}
					return;
				}
				else
				{
					imageStore( s_addTexture, pixelIndex, result );
					previousResult = result;
					previousDepth = depth;
				}
			}
		}
	}
}


#else
	
//another than Optimized quality

	
NUM_THREADS(16, 16, 1)
void main()
{
	uvec2 pixelSize = uvec2( imageSize( s_addTexture ) );
	
	uvec2 pixelIndex = gl_GlobalInvocationID.xy;
	if( pixelIndex.x >= pixelSize.x || pixelIndex.y >= pixelSize.y )
		return;

	//with add 0.5 - equal same as pixel shader
	vec2 v_texCoord0 = ( vec2( pixelIndex ) + vec2_splat( 0.5 ) ) / vec2( pixelSize );
	//vec2 v_texCoord0 = vec2( pixelIndex ) / vec2( pixelSize );
	
	vec4 fragCoord = vec4( pixelIndex.x, pixelIndex.y, 0, 0 );
	
	float rawDepth = texture2DLod(s_depthTexture, v_texCoord0, 0).r;
	vec3 worldPosition2 = reconstructWorldPosition(u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, v_texCoord0, rawDepth);
	float cameraDistance2 = length( worldPosition2 );
	float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);

	vec3 fromCamera = normalize( worldPosition2 - u_viewportOwnerCameraPosition );
	
	vec3 resultColor = vec3_splat(0);

	const int distanceSteps = STEPS;//64;

	float stepSize = u_microparticlesInAirParams.x;//0.2;
	float multiply = u_microparticlesInAirParams.y;//1.05;
	
	//!!!!not best
	float maxBySteps = stepSize * float( distanceSteps );
	if( maxBySteps > cameraDistance2 )
	{
		stepSize *= cameraDistance2 / maxBySteps;
		multiply = 1.0;
	}
	
	vec2 rand = random( fragCoord.xy );
	//randomize start
	float currentDistance = rand.x * rand.y * stepSize; //float currentDistance = stepSize;
	vec3 previousPosition = vec3_splat(0);
	
	LOOP
	for( int nDistanceStep = 0; nDistanceStep < distanceSteps; nDistanceStep++ )
	{
		vec3 worldPosition = fromCamera * min( currentDistance, cameraDistance2 );
		
		//iterate lights
		
		#ifdef GLOBAL_LIGHT_GRID
			vec2 lightGridIndex = ( worldPosition.xy - vec2_splat( d_lightGridStart ) ) / d_lightGridCellSize;
			vec4 lightGrid0 = texelFetch( s_lightGrid, ivec3( ivec2( lightGridIndex ), 0 ), 0 );
			bool lightGridEnabled2 = d_lightGridEnabled > 0.0 && lightGridIndex.x >= 0 && lightGridIndex.x < d_lightGridSize && lightGridIndex.y >= 0 && lightGridIndex.y < d_lightGridSize && lightGrid0.x >= 0.0;
			int lightCount = lightGridEnabled2 ? ( int( lightGrid0.x ) + 1 ) : d_lightCount;
		#else
			int lightCount = d_lightCount;
		#endif
		
		LOOP
		for( int lightCounter = 1; lightCounter < lightCount; lightCounter++ )
		{
			int nLight;
			#ifdef GLOBAL_LIGHT_GRID
				if( lightGridEnabled2 )
					nLight = int( texelFetch( s_lightGrid, ivec3( ivec2( lightGridIndex ), lightCounter / 4 ), 0 )[ lightCounter % 4 ] );
				else
			#endif
				nLight = lightCounter;

			//!!!!skip by bool flag
			
			//cull by bounding sphere
			float lightDistance = length( d_lightPosition - worldPosition );
			if( lightDistance > d_lightBoundingRadius )
				continue;

			int lightType = int(d_lightType);
			
			//objectLightAttenuation and start distance
			float objectLightAttenuation = 1.0;
			BRANCH
			if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT || lightType == ENUM_LIGHT_TYPE_POINT )
			{
				objectLightAttenuation = getLightAttenuation2(d_lightAttenuation, lightDistance);
				if(objectLightAttenuation == 0.0 || lightDistance < d_lightStartDistance)
					continue;
			}

			//lightWorldDirection
			vec3 lightWorldDirection;
			if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
				lightWorldDirection = -d_lightDirection;
			else
				lightWorldDirection = normalize(d_lightPosition - worldPosition);

			BRANCH
			if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
			{
				// factor in spotlight angle
				float rho0 = saturate(dot(-d_lightDirection, lightWorldDirection));
				// factor = (rho - cos(outer/2)) / (cos(inner/2) - cos(outer/2)) ^ falloff 
				float spotFactor0 = saturate(pow(saturate(rho0 - d_lightSpot.y) / (d_lightSpot.x - d_lightSpot.y), d_lightSpot.z));
				if(spotFactor0 == 0.0)
					continue;
				objectLightAttenuation *= spotFactor0;
			}
			
			//lightMaskMultiplier
			vec3 lightMaskMultiplier = vec3(1,1,1);
		#ifdef GLOBAL_LIGHT_MASK
			float lightMaskIndex = d_lightMaskIndex;
			BRANCH
			if( lightMaskIndex >= 0.0 )
			{
				BRANCH
				if( lightType == ENUM_LIGHT_TYPE_POINT )
				{
					vec3 dir = normalize( worldPosition - d_lightPosition );
					dir = mul( d_lightMaskMatrix, vec4( dir, 0 ) ).xyz;
					lightMaskMultiplier = textureCubeArrayLod( s_lightMaskPoint, vec4( dir, lightMaskIndex ), 0.0 ).rgb;
				}
				else if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
				{
					vec4 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) );
					lightMaskMultiplier = texture2DArrayLod( s_lightMaskSpot, vec3( texCoord.xy / texCoord.w, lightMaskIndex ), 0 ).rgb;				
				}
				else
				{
					vec2 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) ).xy;				
					lightMaskMultiplier = texture2DArrayLod( s_lightMaskDirectional, vec3( texCoord, lightMaskIndex ), 0 ).rgb;
				}
			}
		#endif

			//shadows
			float shadowMultiplier = 1.0;
		#ifdef SHADOW_MAP
			float shadowMapIndex = d_shadowMapIndex;
			BRANCH
			if( shadowMapIndex >= 0.0 )
			{
				vec3 normalDummy = vec3_splat(0);//lightWorldDirection;

				vec3 plane = u_viewportOwnerCameraDirection;
				//easy to calculate projective depth
				float cascadeDepth = dot( plane, worldPosition );
				
				//vec3 left = cross( u_viewportOwnerCameraDirection, u_viewportOwnerCameraUp );
				//vec3 plane;//vec4 plane;
				//plane.xyz = normalize( cross( u_viewportOwnerCameraUp, left ) );
				////camera position is equal zero plane.w = -( dot( plane.xyz, vec3_splat( 0 ) ) );
				//float cascadeDepth = dot( plane, vec4( worldPosition, 1.0 ) );
				
				/*
				Plane result;
				Vector3 normal;
				Vector3.Cross( ref dir1, ref dir2, out normal );
				normal.Normalize();
				result.A = normal.X;
				result.B = normal.Y;
				result.C = normal.Z;
				result.D = -( normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z );
				return result;
				*/
				
				//!!!!
				//for fading by distance
				float cameraDistance3 = 0.0;//currentDistance
				
				shadowMultiplier = getShadowMultiplierMulti( worldPosition, cameraDistance3, cascadeDepth, lightWorldDirection, normalDummy, v_texCoord0, fragCoord, lightType, shadowMapIndex, nLight );
			}		
		#endif
		
			//light color
			vec3 lightColor = ( d_lightPower.rgb * 10000.0 ) * u_cameraExposure * objectLightAttenuation * lightMaskMultiplier * shadowMultiplier;
		
			float stepDistance = length( worldPosition - previousPosition );

			//Simple shading model
			resultColor += lightColor * stepDistance;
		}

		if( currentDistance > cameraDistance2 )
			break;
		currentDistance = currentDistance * multiply + stepSize;
		previousPosition = worldPosition;
	}

	imageStore( s_addTexture, ivec2( pixelIndex ), vec4( resultColor * u_microparticlesInAirColor.xyz, 0.0 ) );
	
	//vec4 q = imageLoad( s_sceneTexture, pixelIndex );
	//q += vec4( resultColor * u_microparticlesInAirColor.xyz, 0.0 );
	//imageStore( s_sceneTexture, pixelIndex, q );	
	
}

#endif
