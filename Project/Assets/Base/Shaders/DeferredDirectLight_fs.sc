$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#ifdef GI_GRID
	#include "GICommon.sh"
#endif
#include "UniformsFragment.sh"
#include "FragmentFunctions.sh"

SAMPLER2D(s_sceneTexture, 0);
SAMPLER2D(s_normalTexture, 1);
SAMPLER2D(s_gBuffer2Texture, 2);
//using one sampler for s_gBuffer2Texture, s_gBuffer3Texture, s_gBuffer4Texture because samplers limit
SAMPLER2D_TEXTUREONLY(s_gBuffer3Texture, 16); //SAMPLER2D(s_gBuffer3Texture, 9);
SAMPLER2D(s_depthTexture, 3);
SAMPLER2D_TEXTUREONLY(s_gBuffer4Texture, 17); //SAMPLER2D(s_gBuffer4Texture, 10);
SAMPLER2D(s_gBuffer5Texture, 11);
SAMPLER2D(s_lightsTexture, 4);
SAMPLER2D(s_brdfLUT, 8);
#ifdef GLOBAL_LIGHT_GRID
	SAMPLER3D(s_lightGrid, 15);
#endif

#ifdef GI_GRID
	USAMPLER3D(s_giGrid1, 9);
	SAMPLER3D(s_giGrid2, 10);
	uniform vec4 giRayCastInfo[ 7 ];
//	uniform vec4 showRenderTargetGI0;
//	uniform vec4 showRenderTargetGI1;	
	#include "GIGrid.sh"
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
	#include "ShadowReceiverFunctions.sh"
#endif

#define SHADING_MODEL_SUBSURFACE
#define SHADING_MODEL_FOLIAGE

#include "PBRFilament/common_math.sh"
#include "PBRFilament/brdf.sh"
#include "PBRFilament/PBRFilament.sh"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	vec4 fragCoord = getFragCoord();
	float rawDepth = texture2D(s_depthTexture, v_texCoord0).r;
	vec3 worldPosition = reconstructWorldPosition(u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, v_texCoord0, rawDepth);
	float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);

	
	vec3 baseColor = decodeRGBE8(texture2D(s_sceneTexture, v_texCoord0));
	vec4 normal_and_reflectance = texture2D(s_normalTexture, v_texCoord0);	
	vec3 normal = normalize(normal_and_reflectance.rgb * 2 - 1);
	
	vec4 gBuffer2Data = texture2D(s_gBuffer2Texture, v_texCoord0);
	vec4 gBuffer3Data = texture2D( makeSampler( s_gBuffer2Texture, s_gBuffer3Texture ), v_texCoord0 ); //texture2D(s_gBuffer3Texture, v_texCoord0);
	vec4 gBuffer4Data = texture2D( makeSampler( s_gBuffer2Texture, s_gBuffer4Texture ), v_texCoord0 ); //texture2D(s_gBuffer4Texture, v_texCoord0);
	vec4 gBuffer5Data = texture2D(s_gBuffer5Texture, v_texCoord0);

	vec4 tangent = gBuffer4Data * 2.0 - 1.0;
	tangent.xyz = normalize(tangent.xyz);

	int shadingModel = int(round(gBuffer5Data.x * 8.0));
	bool receiveDecals = gBuffer5Data.y == 1.0;
	float thickness = gBuffer5Data.z;
	float subsurfacePower = gBuffer5Data.w * 15.0;
	
	bool shadingModelSubsurface = shadingModel == 1;
	bool shadingModelFoliage = shadingModel == 2;
	bool shadingModelSimple = shadingModel == 4;//3;
	
	vec3 subsurfaceColor = vec3_splat(0);
	{
		vec3 v = decodeRGBE8(gBuffer3Data);
		if(shadingModelSubsurface || shadingModelFoliage)
			subsurfaceColor = v;
	}
	
	////get GBuffer 4
	//vec4 tangent;
	//bool shadingModelSimple;
	//bool receiveDecals;
	//vec4 gBuffer4Data = texture2D(s_gBuffer4Texture, v_texCoord0);	
	//decodeGBuffer4(gBuffer4Data, tangent, shadingModelSimple, receiveDecals);
	
	vec3 bitangent = normalize(cross(tangent.xyz, normal) * tangent.w);

	vec3 toCamera = u_viewportOwnerCameraPosition - worldPosition;
	float cameraDistance = length(toCamera);
	toCamera = normalize(toCamera);
	
	
	vec3 resultColor = vec3_splat(0);

	
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
			//mat4 lightMaskMatrix = mtxFromRows(
			//	texelFetch( s_lightsTexture, ivec2( 5, nLight ), 0 ), 
			//	texelFetch( s_lightsTexture, ivec2( 6, nLight ), 0 ), 
			//	texelFetch( s_lightsTexture, ivec2( 7, nLight ), 0 ), 
			//	texelFetch( s_lightsTexture, ivec2( 8, nLight ), 0 ) );
				
			BRANCH
			if( lightType == ENUM_LIGHT_TYPE_POINT )
			{
				vec3 dir = normalize( worldPosition - d_lightPosition );
				dir = mul( d_lightMaskMatrix, vec4( dir, 0 ) ).xyz;
				//flipped cubemaps, already applied in lightMaskTextureMatrixArray.
				//dir = float3(-dir.y, dir.z, dir.x);
				lightMaskMultiplier = textureCubeArrayLod( s_lightMaskPoint, vec4( dir, lightMaskIndex ), 0.0 ).rgb;
			}
			else if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
			{
				vec4 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) );
				lightMaskMultiplier = texture2DArrayLod( s_lightMaskSpot, vec3( texCoord.xy / texCoord.w, lightMaskIndex ), 0 ).rgb;				
				//lightMaskMultiplier = texture2DProj( s_lightMask, texCoord ).rgb;
			}
			else //if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
			{
				vec2 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) ).xy;				
				lightMaskMultiplier = texture2DArrayLod( s_lightMaskDirectional, vec3( texCoord, lightMaskIndex ), 0 ).rgb;
				//lightMaskMultiplier = texture2DLod( s_lightMaskDirectional, texCoord, 0 ).rgb;
				////lightMaskMultiplier = texture2D( s_lightMaskDirectional, texCoord ).rgb;
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
			shadowMultiplier = getShadowMultiplierMulti( worldPosition, cameraDistance, depth, lightWorldDirection, normal, v_texCoord0, fragCoord, lightType, shadowMapIndex, nLight );
		}		
	#endif
	
		//light color
		vec3 lightColor = ( d_lightPower.rgb * 10000.0 ) * u_cameraExposure * objectLightAttenuation * lightMaskMultiplier * shadowMultiplier;
		
		if(shadingModelSimple)
		{
			//Simple shading model
			resultColor += baseColor * lightColor;
		}
		else
		{
			//Lit, Subsurface, Foliage shading models

	#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE
			resultColor += baseColor * lightColor * saturate(dot(normal, lightWorldDirection) / PI);
	#else
			
			float metallic = gBuffer2Data.x;
			float roughness = gBuffer2Data.y;
			float ambientOcclusion = gBuffer2Data.z;
			float reflectance = normal_and_reflectance.a;

			//toLight
			//vec3 toLight = lightWorldDirection;
			//vec3 toLight;
			//if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT || lightType == ENUM_LIGHT_TYPE_POINT )
			//	toLight = d_lightPosition.xyz - worldPosition;
			//else if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
			//	toLight = -d_lightDirection;
			//else
			//	toLight = normal; // For ambient light dot(N, L) == 1, i.e. we can do L = N;
			//toLight = normalize(toLight);

			MaterialInputs material;

			material.baseColor = vec4(baseColor, 0.0);
			material.roughness = roughness;
			material.metallic = metallic;
			material.reflectance = reflectance;
			material.ambientOcclusion = ambientOcclusion;

#ifdef MATERIAL_HAS_ANISOTROPY
			material.anisotropyDirection = vec3_splat(0);
			material.anisotropy = 0;
#endif

			//#if defined(SHADING_MODEL_SUBSURFACE)
			material.thickness = thickness;
			material.subsurfacePower = subsurfacePower;
			material.subsurfaceColor = subsurfaceColor;
			//#endif

#ifdef MATERIAL_HAS_CLEAR_COAT
			material.clearCoat = 0;
			material.clearCoatRoughness = 0;
			material.clearCoatNormal = vec3_splat(0);
#endif

			//#if defined(SHADING_MODEL_CLOTH)
			//	material.sheenColor = vec3_splat(0);
			//	material.subsurfaceColor = vec3_splat(0);
			//#endif
			
			ShadingParams shading;
			getPBRFilamentShadingParams(material, tangent.xyz, bitangent, normal, vec3_splat(0), lightWorldDirection/*toLight*/, toCamera, gl_FrontFacing, shading);

			PixelParams pixel;
			getPBRFilamentPixelParams(material, shading, pixel);

			vec3 shadingResult;
			BRANCH
			if(shadingModelSubsurface || shadingModelFoliage)
				shadingResult = surfaceShadingSubSurface(pixel, shading);
			else
				shadingResult = surfaceShadingStandard(pixel, shading);
			resultColor += shadingResult * lightColor;
			//resultColor.rgb = surfaceShading(pixel) * lightColor;
			
	#endif
		}
	}

	//!!!!

#ifdef GI_GRID
	//if(false)
	//if( capsLock )
	{
		//!!!!

		//!!!!отступ от центра вокселя


		/*
		vec3 directions[512];
		for (int i = 0; i < 512; i++)
		{
			float seed = i * 0.1;// + u_engineTime;
			directions[i] = normalize(vec3(sin(seed * 3.1415), cos(seed * 2.7182), sin(seed * 1.4142)));
		}
		const int DIFFUSE_CONE_COUNT = 512;
		*/
		
		/*
		vec3 directions[128];
		for (int i = 0; i < 128; i++)
		{
			//!!!!randomize per voxel index. where else
			float seed = i * 0.1;// + u_engineTime;
		
			//!!!!
			//if(capsLock)
			//	seed += fragCoord.x + fragCoord.y * 2.1;
			//seed += float( gridIndex.x ) + float( gridIndex.y ) * 2.1 + float( gridIndex.z ) * 3.42;
		
			directions[i] = normalize(vec3(sin(seed * 3.1415), cos(seed * 2.7182), sin(seed * 1.4142)));
		}
		const int DIFFUSE_CONE_COUNT = 128;
		*/
		

		vec4 indirectContribution = vec4_splat( 0 );

		int cascade = giDetectCascade( worldPosition );
		
		vec4 cascadeInfo = giRayCastInfo[ 1 + cascade ];
		vec3 gridPosition = cascadeInfo.xyz;
		float cellSize = cascadeInfo.w;

		int gridSize = int( giRayCastInfo[ 0 ].y );
		
		//!!!!что по стыкам каскадов
		
		ivec3 gridIndex = ivec3( ( worldPosition - gridPosition ) / cellSize );
		//!!!!sense?
		//gridIndex = clamp( 
		
		//ivec3 cellPositionMin = ivec3( ( worldBoundsMin - gridPosition ) / cellSize ) - ivec3( 1, 1, 1 );


	

		//!!!!
		ivec3 cellIndexWithCascadeOffset = gridIndex;
		cellIndexWithCascadeOffset.x += gridSize * cascade;
		indirectContribution.rgb += texelFetch( s_giGrid2, cellIndexWithCascadeOffset, 0 ).rgb;
		//indirectContribution.rgb += texture3DLod( s_giGrid2, cellIndexWithCascadeOffset / vec3( gridSize * cascadeCount, gridSize, gridSize ), 0 ).rgb;
		
		

		//!!!!
	
/*


		//!!!!
		//vec3 from = worldPosition + normal * cellSize * 1.01 * sqrt( 2.0 );
		//vec3 from = worldPosition + normal * cellSize * 0.51 * sqrt( 2.0 );
		
		ivec3 nearCellIndexes[ 6 ];
		nearCellIndexes[ 0 ] = ivec3( -1, 0, 0 );
		nearCellIndexes[ 1 ] = ivec3( 1, 0, 0 );
		nearCellIndexes[ 2 ] = ivec3( 0, -1, 0 );
		nearCellIndexes[ 3 ] = ivec3( 0, 1, 0 );
		nearCellIndexes[ 4 ] = ivec3( 0, 0, -1 );
		nearCellIndexes[ 5 ] = ivec3( 0, 0, 1 );

		for( int nSide = 0; nSide < 6; nSide++ )
		{
			ivec3 gridIndex2 = gridIndex + nearCellIndexes[ nSide ];
			
			if( any( gridIndex2 < ivec3_splat( 0 ) ) || any( gridIndex2 >= ivec3_splat( gridSize ) ) )
				continue;
			
			ivec3 giGridIndex2WithCascadeOffset = gridIndex2;
			giGridIndex2WithCascadeOffset.x += gridSize * cascade;
			
			bool nearCellFilled = convertRGBA8ToUVec4( texelFetch( s_giGrid1, giGridIndex2WithCascadeOffset, 0 ).r ).w == 255u;			
			if( !nearCellFilled )
			{	
				//!!!!randomize?
				vec3 normal2 = vec3( nearCellIndexes[ nSide ] );
				
				//!!!!
				vec3 from = worldPosition + normal2 * cellSize * 1.51;
				//vec3 from = worldPosition + normal2 * cellSize * 0.51;
				
				
				//!!!!
				//indirectContribution += vec4(0,0,1,0);
				
				
				vec4 indirectContribution2 = vec4_splat( 0 );
				int count = 0;
				for( int vv = 0; vv < 8; vv++ )
				{
					for( int hh = 0; hh < 16; hh++ )
					{
						float h = float( hh ) * PI * 2 / 16; // v_texCoord0.x * PI * 2.0;				
						float v = float( vv ) * PI / 8; //float v = ( float( vv ) - 3.5 ) * PI / 8; //( 0.5 - v_texCoord0.y ) * PI;

						h += float( gridIndex.x + gridIndex.z * 3 ) / 16 / 5;
						v += float( gridIndex.y + gridIndex.z * 2 ) / 8 / 5;
						
						//!!!!
						//h += u_engineTime;
						
						//h += fragCoord.x / 16;
						//v += fragCoord.y / 8;
						
						v = ( v % PI ) - PI / 2;
						
						vec3 dir = sphericalDirectionGetVector( vec2( h, v ) );

						//!!!!
						//float cosTheta = dot( normal, dir );
						float cosTheta = dot( normal2, dir );
						if( cosTheta > 0.01 )
						{
							count++;

							//!!!!нормаль тут есть, как влиять будет
							
							vec3 rayCastColor;
							//float rayCastDistance;
							bool rayCastResult = giRayCast( from, dir, rayCastColor, false, false );//, rayCastDistance );

							if( rayCastResult )
							{
								vec4 contrib = vec4( rayCastColor, 1 );
								indirectContribution += contrib * cosTheta;
							}
							else
							{
								//!!!!
								//vec4 contrib = vec4( 1,1,1, 1 );
								//indirectContribution += contrib * cosTheta;
							}
							
							//coneTraceCount += 1.0;
							//indirectContribution += castCone( startPos, dir, DIFFUSE_CONE_APERTURE, MAX_TRACE_DISTANCE, minLevel ) * cosTheta;
						}
					}
				}
				
				if( count > 0 )
				{
					indirectContribution2 /= float( count );
					indirectContribution += indirectContribution2;
				}
			}
		}
		
//!!!!		
		indirectContribution /= 30;
		//indirectContribution /= 3;
	

*/	




		


/*	
		
		
		vec4 indirectContribution = vec4_splat( 0 );

//!!!!cascades
		vec4 giDataTemp = giRayCastInfo[ 1 + 0 ];
		float cellSizeTemp = giDataTemp.w;
		vec3 from = worldPosition + normal * cellSizeTemp * 0.51 * sqrt( 2.0 );				

		int count = 0;
		for( int vv = 0; vv < 8; vv++ )
		{
			for( int hh = 0; hh < 16; hh++ )
			{
				//float h = ( float( hh ) + fragCoord.x / 16 ) * PI * 2 / 16;
				//float v = ( float( vv ) + fragCoord.y / 8 ) * PI / 8;
				
				float h = float( hh ) * PI * 2 / 16; // v_texCoord0.x * PI * 2.0;				
				float v = float( vv ) * PI / 8; //float v = ( float( vv ) - 3.5 ) * PI / 8; //( 0.5 - v_texCoord0.y ) * PI;

				//!!!!
				//h += u_engineTime;
				
				//h += fragCoord.x / 16;
				//v += fragCoord.y / 8;
				
				v = ( v % PI ) - PI / 2;
				
				vec3 dir = sphericalDirectionGetVector( vec2( h, v ) );

				float cosTheta = dot( normal, dir );
				if( cosTheta > 0.01 )
				{
					count++;
					
					
					//!!!!нормаль тут есть, как влиять будет
					
					
					vec3 rayCastColor;
					//float rayCastDistance;
					bool rayCastResult = giRayCast( from, dir, rayCastColor, false, false );//, rayCastDistance );

					if( rayCastResult )
					{
						vec4 contrib = vec4( rayCastColor, 1 );
						indirectContribution += contrib * cosTheta;
					}
					else
					{
						//!!!!
						//vec4 contrib = vec4( 1,1,1, 1 );
						//indirectContribution += contrib * cosTheta;
					}
					
					//coneTraceCount += 1.0;
					//indirectContribution += castCone( startPos, dir, DIFFUSE_CONE_APERTURE, MAX_TRACE_DISTANCE, minLevel ) * cosTheta;
				}
			}
		}
		
		indirectContribution /= float( count );
		//!!!!
		//indirectContribution.a *= u_ambientOcclusionFactor;

		//!!!!
		//indirectContribution.rgb *= diffuse * u_indirectDiffuseIntensity;
		//indirectContribution = clamp(indirectContribution, 0.0, 1.0);

//!!!!		
		indirectContribution *= 2;//4;

*/		
		
		
		/*		
		//float coneTraceCount = 0.0;
		float cosSum = 0.0;
		for( int i = 0; i < DIFFUSE_CONE_COUNT; i++ )
		{
			vec3 dir = normalize( directions[ i ] );
			//vec3 dir = normalize( DIFFUSE_CONE_DIRECTIONS[ i ] );
			
			float cosTheta = dot( normal, dir );
			if( cosTheta > 0.0 )
			{
				//!!!! * 0.1
				//vec3 from = worldPosition + dir * 1.0;

				vec3 rayCastColor;
				float rayCastDistance;
				bool rayCastResult = giRayCast( from, dir, rayCastColor, rayCastDistance );

				if( rayCastResult )
				{
					vec4 contrib = vec4( rayCastColor, 1 );
					indirectContribution += contrib * cosTheta;
				}
				else
				{
					//!!!!
					//vec4 contrib = vec4( 1,1,1, 1 );
					//indirectContribution += contrib * cosTheta;
				}
				
				//coneTraceCount += 1.0;
				//indirectContribution += castCone( startPos, dir, DIFFUSE_CONE_APERTURE, MAX_TRACE_DISTANCE, minLevel ) * cosTheta;
			}
		}

		// DIFFUSE_CONE_COUNT includes cones to integrate over a sphere - on the hemisphere there are on average ~half of these cones
		indirectContribution /= DIFFUSE_CONE_COUNT * 0.5;
		//!!!!
		//indirectContribution.a *= u_ambientOcclusionFactor;

		//!!!!
		//indirectContribution.rgb *= diffuse * u_indirectDiffuseIntensity;
		//indirectContribution = clamp(indirectContribution, 0.0, 1.0);

//!!!!		
		indirectContribution *= 2;//4;
		*/
		
		//indirectContribution *= 100;
	
		resultColor += indirectContribution.rgb;
		
		
		
		/*
		vec3 reflected = normalize( reflect( -toCamera, normal ) );

		//!!!!reflected * 0.1
		vec3 from = worldPosition + reflected * 1.0;

		vec3 rayCastColor;
		float rayCastDistance;
		bool rayCastResult = giRayCast( from, reflected, rayCastColor, rayCastDistance );
		
		if( rayCastResult )
		{
			resultColor = rayCastColor;
			//resultColor = lerp(resultColor, giResultColor, 0.5);
		}
		*/
	}
#endif
	
	
	/*
	//!!!!tempvmsj
#ifdef GI_GRID
	//if( capsLock )
	if( false )
	{
		//!!!!
		
		vec3 reflected = normalize( reflect( -toCamera, normal ) );

		//!!!!reflected * 0.1
		vec3 from = worldPosition + reflected * 1.0;

		//!!!!
		vec3 giResultPosition;
		vec4 giResultColor;
		bool result = giGetReflection( s_giBasicGrid, showRenderTargetGI0, showRenderTargetGI1, from, reflected, giResultColor );
		//, giResultPosition
		
		if( result )
		{
			resultColor = giResultColor;
			//resultColor = lerp(resultColor, giResultColor, 0.5);
		}
	}
#endif
*/
			
	gl_FragColor = vec4(resultColor, 1.0);
}
