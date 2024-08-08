$input v_texCoord01, v_worldPosition_depth, v_worldNormal_materialIndex, v_tangent, v_fogFactor, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_position, v_previousPosition, v_texCoord23, v_colorParameter, v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define FORWARD 1
#include "Common.sh"
#include "UniformsFragment.sh"

////!!!!test oit
//#ifndef GLSL
//#include "Common/bgfx_compute.sh"
//#endif

uniform vec4 u_renderOperationData[8];
uniform vec4 u_materialCustomParameters[2];
uniform vec4 u_objectInstanceParameters[2];

SAMPLER2D(s_materials, 1);
#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
	SAMPLER2D(s_voxelData, 2);
#endif
//#ifdef GLOBAL_VIRTUALIZED_GEOMETRY
//	SAMPLER2D(s_virtualizedData, 11);
//#endif

#include "CustomFunctions.sh"

#ifdef DISPLACEMENT_CODE_PARAMETERS
	DISPLACEMENT_CODE_PARAMETERS
#endif
#ifdef FRAGMENT_CODE_PARAMETERS
	FRAGMENT_CODE_PARAMETERS
#endif
#ifdef MATERIAL_INDEX_CODE_PARAMETERS
	MATERIAL_INDEX_CODE_PARAMETERS
#endif
#ifdef OPACITY_CODE_PARAMETERS
	OPACITY_CODE_PARAMETERS
#endif

//environment cubemaps
//IMAGE2D_RW( s_oitScreen, rgba8, 3 );
////UIMAGE2D_RW( s_oitScreen, r32ui, 3 );
////IMAGE2D_WO( s_oitLists, rgba32f, 4 );

////BUFFER_RW( s_oitScreen, uint, 3 );
////BUFFER_RW( s_oitLists, vec4, 4 );
SAMPLERCUBE(s_environmentTexture1, 3);
SAMPLERCUBE(s_environmentTexture2, 4);

uniform vec4 u_forwardEnvironmentData[5];
#define u_forwardEnvironmentDataRotation1 u_forwardEnvironmentData[0]
#define u_forwardEnvironmentDataMultiplierAndAffect1 u_forwardEnvironmentData[1]
#define u_forwardEnvironmentDataRotation2 u_forwardEnvironmentData[2]
#define u_forwardEnvironmentDataMultiplierAndAffect2 u_forwardEnvironmentData[3]
#define u_forwardEnvironmentDataBlendingFactor u_forwardEnvironmentData[4].x
uniform vec4 u_forwardEnvironmentIrradiance1[9];
uniform vec4 u_forwardEnvironmentIrradiance2[9];

SAMPLER2D(s_colorDepthTextureCopy, 5);
SAMPLER2D(s_brdfLUT, 6);
SAMPLER2D(s_lightsTexture, 7);
#ifdef GLOBAL_LIGHT_GRID
	SAMPLER3D(s_lightGrid, 8);
#endif

//!!!!not need? use from s_brdfLUT? wrap, clamp?
#ifndef GLSL
	SAMPLER2D(s_linearSamplerFragment, 9);
#endif

#ifdef SHADOW_MAP
	#ifdef GLSL
		//mobile specific. light grid is disabled, reused by shadows and masks because of samplers limit

		#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
			SAMPLER2DARRAY(s_shadowMapShadowDirectional, 8);
		#else
			SAMPLER2DARRAYSHADOW(s_shadowMapShadowDirectional, 8);
		#endif
		#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
			SAMPLER2DARRAY(s_shadowMapShadowSpot, 9);
		#else
			SAMPLER2DARRAYSHADOW(s_shadowMapShadowSpot, 9);
		#endif
		#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
			SAMPLERCUBEARRAY(s_shadowMapShadowPoint, 10);
		#else
			SAMPLERCUBEARRAYSHADOW(s_shadowMapShadowPoint, 10);
		#endif
	
	#else	

		#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
			SAMPLER2DARRAY(s_shadowMapShadowDirectional, 10);
		#else
			SAMPLER2DARRAYSHADOW(s_shadowMapShadowDirectional, 10);
		#endif
		#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
			SAMPLER2DARRAY(s_shadowMapShadowSpot, 11);
		#else
			SAMPLER2DARRAYSHADOW(s_shadowMapShadowSpot, 11);
		#endif
		#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
			SAMPLERCUBEARRAY(s_shadowMapShadowPoint, 12);
		#else
			SAMPLERCUBEARRAYSHADOW(s_shadowMapShadowPoint, 12);
		#endif
		
	#endif	
#endif

//mobile specific. on limited devices masks and shadow maps are managed inside one shadow map array
#ifndef GLSL
#ifdef GLOBAL_LIGHT_MASK
	SAMPLER2DARRAY(s_lightMaskDirectional, 13);
	SAMPLER2DARRAY(s_lightMaskSpot, 14);
	SAMPLERCUBEARRAY(s_lightMaskPoint, 15);
#endif
#endif

//!!!!sampler limit
/*
//ssr
#ifndef MOBILE
	//SAMPLER2DARRAY( s_ssrTexture, 12 );
	//SAMPLER2D_TEXTUREONLY( s_voxelData, 16 );
	uniform vec4 u_environmentLightParams;
	#define u_ssrEnabled ( u_environmentLightParams.w != 0.0 )
#endif
*/

#ifdef DISPLACEMENT_CODE_SAMPLERS
	DISPLACEMENT_CODE_SAMPLERS
#endif
#ifdef FRAGMENT_CODE_SAMPLERS
	FRAGMENT_CODE_SAMPLERS
#endif
#ifdef MATERIAL_INDEX_CODE_SAMPLERS
	MATERIAL_INDEX_CODE_SAMPLERS
#endif
#ifdef OPACITY_CODE_SAMPLERS
	OPACITY_CODE_SAMPLERS
#endif

#ifdef DISPLACEMENT_CODE_SHADER_SCRIPTS
	DISPLACEMENT_CODE_SHADER_SCRIPTS
#endif
#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
	#include "ParallaxOcclusionMapping.sh"
#endif
#ifdef FRAGMENT_CODE_SHADER_SCRIPTS
	FRAGMENT_CODE_SHADER_SCRIPTS
#endif
#ifdef MATERIAL_INDEX_CODE_SHADER_SCRIPTS
	MATERIAL_INDEX_CODE_SHADER_SCRIPTS
#endif
#ifdef OPACITY_CODE_SHADER_SCRIPTS
	OPACITY_CODE_SHADER_SCRIPTS
#endif

#include "FragmentFunctions.sh"
#include "FragmentVoxel.sh"
#include "PBRFilament/common_math.sh"
#include "PBRFilament/brdf.sh"
#include "PBRFilament/PBRFilament.sh"
#ifdef SHADOW_MAP
	#include "ShadowReceiverFunctions.sh"
#endif

//uniform vec4 u_forwardOIT;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if defined( BLEND_MODE_TRANSPARENT ) || defined( BLEND_MODE_ADD )
EARLY_DEPTH_STENCIL
#endif
void main()
{
	vec3 shadingView = vec3_splat( 0 );
	float shadingNoV = 0.0;
	
	vec4 fragCoord = getFragCoord();
	
#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
	float voxelDataMode = u_renderOperationData[ 1 ].w;
#else
	const float voxelDataMode = 0.0;
#endif

	//float virtualizedDataMode = u_renderOperationData[3].w;

	//lod for opaque
#ifdef GLOBAL_SMOOTH_LOD
	float lodValue = v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.x;
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		smoothLOD(fragCoord, lodValue);
	#endif
#endif

	vec2 texCoord0 = v_texCoord01.xy;
	vec2 texCoord1 = v_texCoord01.zw;
	vec2 texCoord2 = v_texCoord23.xy;
	vec2 unwrappedUV = getUnwrappedUV(texCoord0, texCoord1, texCoord2, u_renderOperationData[3].x);
	vec3 inputWorldNormal = v_worldNormal_materialIndex.xyz;
	vec4 tangent = v_tangent;
	vec4 color0 = v_color0;
	vec3 fromCameraDirection = normalize(v_worldPosition_depth.xyz - u_viewportOwnerCameraPosition);
	vec3 worldPosition = v_worldPosition_depth.xyz;

	int materialIndex = int(round(v_worldNormal_materialIndex.w));
	float depthOffset = 0.0;
#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
	voxelDataModeCalculateParametersF(voxelDataMode, s_voxelData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, fromCameraDirection, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex, v_colorParameter);
#endif
//#ifdef GLOBAL_VIRTUALIZED_GEOMETRY
//	virtualizedDataModeCalculateParametersF(virtualizedDataMode, s_virtualizedData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, gl_PrimitiveID, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex);
//#endif
	worldPosition += fromCameraDirection * depthOffset;

	float cameraDistance = length(worldPosition - u_viewportOwnerCameraPosition);
	vec3 cameraPosition = u_viewportOwnerCameraPosition;

	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y;
	MEDIUMP float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
	#endif
#endif

#if GLOBAL_CUT_VOLUME_MAX_AMOUNT > 0
	if( cutVolumes( worldPosition ) )
		discard;
#endif

	inputWorldNormal = normalize(inputWorldNormal);
	tangent.xyz = normalize(tangent.xyz);

	//end of geometry stage
	
	//shading

	//multi material
#ifdef MATERIAL_INDEX_CODE_BODY
	#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )	
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
	#else
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling, u_mipBias)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
	#endif
	
	/*
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling, voxelDataMode != 0.0 ? -16.0 : u_mipBias)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, voxelDataMode != 0.0 ? -16.0 : u_mipBias)
	*/
	
	{
		MATERIAL_INDEX_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	int frameMaterialIndex = int( u_renderOperationData[ 0 ].x );
	////get material data
	//vec4 materialStandardFragment[ MATERIAL_STANDARD_FRAGMENT_SIZE ];
	//getMaterialData( s_materials, uint( u_renderOperationData[ 0 ].x ), materialStandardFragment );

	//multi material
#ifdef MULTI_MATERIAL_SEPARATE_PASS
	if(materialIndex != u_materialMultiSubMaterialSeparatePassIndex)
		discard;
#endif
	
	//displacement
	vec2 displacementOffset = vec2_splat(0);
#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
	BRANCH
	if(u_displacementScale != 0.0 && voxelDataMode == 0.0)
		displacementOffset = getParallaxOcclusionMappingOffset(texCoord0, v_eyeTangentSpace, v_normalTangentSpace, u_materialDisplacementScale * u_displacementScale, u_displacementMaxSteps);
#endif //DISPLACEMENT_CODE_BODY
	
	//get material parameters
	vec3 normal = vec3_splat(0);
	vec3 baseColor = u_materialBaseColor;
	float opacity = u_materialOpacity;
	float opacityMaskThreshold = u_materialOpacityMaskThreshold;
	float metallic = u_materialMetallic;
	float roughness = u_materialRoughness;
	float reflectance = u_materialReflectance;
	float clearCoat = u_materialClearCoat;
	float clearCoatRoughness = u_materialClearCoatRoughness;
	vec3 clearCoatNormal = vec3_splat(0);
	float anisotropy = u_materialAnisotropy;
	vec3 anisotropyDirection = vec3_splat(0);// = u_materialAnisotropyDirection;
	float anisotropyDirectionBasis = u_materialAnisotropyDirectionBasis;
	float thickness = u_materialThickness;
	float subsurfacePower = u_materialSubsurfacePower;
	vec3 sheenColor = u_materialSheenColor;
	vec3 subsurfaceColor = u_materialSubsurfaceColor;
	float ambientOcclusion = 1.0;
	//float rayTracingReflection = u_materialRayTracingReflection;
	vec3 emissive = u_materialEmissive;
	float softParticlesDistance = u_materialSoftParticlesDistance;
	vec4 customParameter1 = u_materialCustomParameters[0];
	vec4 customParameter2 = u_materialCustomParameters[1];
	vec4 instanceParameter1 = u_objectInstanceParameters[0];
	vec4 instanceParameter2 = u_objectInstanceParameters[1];

	//get material parameters (procedure generated code)
	vec2 texCoord0BeforeDisplacement = texCoord0;
	vec2 texCoord1BeforeDisplacement = texCoord1;
	vec2 texCoord2BeforeDisplacement = texCoord2;
	vec2 unwrappedUVBeforeDisplacement = unwrappedUV;
	texCoord0 -= displacementOffset;
	texCoord1 -= displacementOffset;
	texCoord2 -= displacementOffset;
	unwrappedUV -= displacementOffset;

	#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
	#else
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling, u_mipBias)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
	#endif	
	#ifdef FRAGMENT_CODE_BODY
	{
		FRAGMENT_CODE_BODY
	}
	#endif
	#ifdef OPACITY_CODE_BODY
	{
		OPACITY_CODE_BODY
	}
	#endif	
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D

/*
#ifdef GLOBAL_VOXEL_LOD
	BRANCH
	if( voxelDataMode != 0.0 )
	{
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
		#ifdef FRAGMENT_CODE_BODY
		{
			FRAGMENT_CODE_BODY
		}
		#endif
		#ifdef OPACITY_CODE_BODY
		{
			OPACITY_CODE_BODY
		}
		#endif
		#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
		#undef CODE_BODY_TEXTURE2D
	}
	else	
#endif //GLOBAL_VOXEL_LOD
	{
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling, u_mipBias)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
		//#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling, voxelDataMode != 0.0 ? -16.0 : u_mipBias)	
		//#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, voxelDataMode != 0.0 ? -16.0 : u_mipBias)
		#ifdef FRAGMENT_CODE_BODY
		{
			FRAGMENT_CODE_BODY
		}
		#endif
		#ifdef OPACITY_CODE_BODY
		{
			OPACITY_CODE_BODY
		}
		#endif
		#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
		#undef CODE_BODY_TEXTURE2D
	}
*/

	//apply color parameter

	baseColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0.0)
		baseColor *= color0.xyz;
	baseColor = max(baseColor, vec3_splat(0));
	
	opacity *= v_colorParameter.w;
	if(u_materialUseVertexColor != 0.0)
		opacity *= color0.w;
	opacity = saturate(opacity);
	
#ifdef SHADING_MODEL_CLOTH
	sheenColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0.0)
		sheenColor *= color0.xyz;
	sheenColor = max(sheenColor, vec3_splat(0));
#endif	

#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE) || defined(SHADING_MODEL_CLOTH)
	subsurfaceColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0.0)
		subsurfaceColor *= color0.xyz;
	subsurfaceColor = max(subsurfaceColor, vec3_splat(0));	
#endif

	//opacity dithering
#ifdef OPACITY_DITHERING
	opacity = dither(fragCoord, opacity);
#endif

	//opacity masked clipping
#if !defined(GLOBAL_VOXEL_LOD) || !defined(VOXEL)
#ifdef BLEND_MODE_MASKED
	//if(voxelDataMode == 0.0)
	clip(opacity - opacityMaskThreshold);
#endif
#endif


	MEDIUMP vec4 resultColor = vec4_splat(0);
	

#ifndef SHADING_MODEL_UNLIT

#ifdef MATERIAL_HAS_ANISOTROPY
	if(any2(anisotropyDirection))
		anisotropyDirection = expand(anisotropyDirection);
	else
   		anisotropyDirection = u_materialAnisotropyDirection;
	anisotropyDirection.z = 0.0;
	anisotropyDirection = normalize(anisotropyDirection);
#endif	

	MEDIUMP vec3 bitangent = cross(tangent.xyz, inputWorldNormal) * tangent.w;
	
#if defined(GLOBAL_NORMAL_MAPPING) && GLOBAL_MATERIAL_SHADING != GLOBAL_MATERIAL_SHADING_SIMPLE
	BRANCH
	if(any2(normal))
	{
		MEDIUMP mat3 tbn = transpose(mtxFromRows(tangent.xyz, bitangent, inputWorldNormal));
		normal = expand(normal);
		normal.z = sqrt(max(1.0 - dot(normal.xy, normal.xy), 0.0));
		normal = normalize(mul(tbn, normal));
	}
	else
		normal = inputWorldNormal;
#else
		normal = inputWorldNormal;
#endif

#ifdef TWO_SIDED_FLIP_NORMALS
	if(gl_FrontFacing)
		normal = -normal;
#endif

	//clearCoatNormal
#ifdef MATERIAL_HAS_CLEAR_COAT
	if(any2(clearCoatNormal))
	{
		clearCoatNormal = expand(clearCoatNormal);
		clearCoatNormal.z = sqrt(max(1.0 - dot(clearCoatNormal.xy, clearCoatNormal.xy), 0.0));
	}	
	else
		clearCoatNormal = normal;
#endif

	//ambient light
	{
		//moved to bottom, because compilation bug. happens on Scripting C#.scene. to test switch deferred and forward.
	}
	
#ifdef GLOBAL_DEBUG_MODE
	if( u_viewportOwnerDebugMode <= DebugMode_Wireframe )
#endif
	{
		//iterate lights

		#ifdef GLOBAL_LIGHT_GRID
			vec2 lightGridIndex = ( worldPosition.xy - vec2_splat( d_lightGridStart ) ) / d_lightGridCellSize;
			vec4 lightGrid0 = texelFetch( s_lightGrid, ivec3( ivec2( lightGridIndex ), 0 ), 0 );
			bool lightGridEnabled2 = d_lightGridEnabled > 0.0 && lightGridIndex.x >= 0 && lightGridIndex.x < d_lightGridSize && lightGridIndex.y >= 0 && lightGridIndex.y < d_lightGridSize && lightGrid0.x >= 0.0;
			int lightCount = lightGridEnabled2 ? ( int( lightGrid0.x ) + 1 ) : d_lightCount;
		#else
			int lightCount = d_lightCount;
		#endif

#ifdef LIGHT_DIRECTIONAL_AMBIENT_ONLY
		const int lightCounter = 1;
		BRANCH
		if( lightCount > 1 )
#else
		LOOP
		for( int lightCounter = 1; lightCounter < lightCount; lightCounter++ )
#endif			
		{
			int nLight;
			#ifdef GLOBAL_LIGHT_GRID
				if( lightGridEnabled2 )
					nLight = int( texelFetch( s_lightGrid, ivec3( ivec2( lightGridIndex ), lightCounter / 4 ), 0 )[ lightCounter % 4 ] );
				else
			#endif
					nLight = lightCounter;
					
#ifndef LIGHT_DIRECTIONAL_AMBIENT_ONLY
			//cull by bounding sphere
			float lightDistance = length( d_lightPosition - worldPosition );
			if( lightDistance > d_lightBoundingRadius )
				continue;
#endif			

			int lightType = int(d_lightType);
			
			//objectLightAttenuation and start distance
			MEDIUMP float objectLightAttenuation = 1.0;
#ifndef LIGHT_DIRECTIONAL_AMBIENT_ONLY
			BRANCH
			if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT || lightType == ENUM_LIGHT_TYPE_POINT )
			{
				objectLightAttenuation = getLightAttenuation2(d_lightAttenuation, lightDistance);
				if(objectLightAttenuation == 0.0 || lightDistance < d_lightStartDistance)
					continue;
			}
#endif			

			//lightWorldDirection
			MEDIUMP vec3 lightWorldDirection;
#ifndef LIGHT_DIRECTIONAL_AMBIENT_ONLY
			if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
				lightWorldDirection = -d_lightDirection;
			else
				lightWorldDirection = normalize( d_lightPosition - worldPosition );
#else
				lightWorldDirection = -d_lightDirection;
#endif

#ifndef LIGHT_DIRECTIONAL_AMBIENT_ONLY
			BRANCH
			if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
			{
				// factor in spotlight angle
				MEDIUMP float rho0 = saturate(dot(-d_lightDirection, lightWorldDirection));
				// factor = (rho - cos(outer/2)) / (cos(inner/2) - cos(outer/2)) ^ falloff 
				MEDIUMP float spotFactor0 = saturate(pow(saturate(rho0 - d_lightSpot.y) / (d_lightSpot.x - d_lightSpot.y), d_lightSpot.z));
				if(spotFactor0 == 0.0)
					continue;
				objectLightAttenuation *= spotFactor0;
			}
#endif			

			//lightMaskMultiplier
			MEDIUMP vec3 lightMaskMultiplier = vec3(1,1,1);
		#ifdef GLOBAL_LIGHT_MASK
			MEDIUMP float lightMaskIndex = d_lightMaskIndex;
			BRANCH
			if( lightMaskIndex >= 0.0 )
			{				
#ifndef LIGHT_DIRECTIONAL_AMBIENT_ONLY
				BRANCH
				if( lightType == ENUM_LIGHT_TYPE_POINT )
				{
					vec3 dir = normalize( worldPosition - d_lightPosition );
					dir = mul( d_lightMaskMatrix, vec4( dir, 0 ) ).xyz;
					//flipped cubemaps, already applied in lightMaskTextureMatrixArray.
					//dir = float3(-dir.y, dir.z, dir.x);
					#ifdef GLSL
						#ifdef SHADOW_MAP
							lightMaskMultiplier = textureCubeArrayLod( s_shadowMapShadowPoint, vec4( dir, lightMaskIndex ), 0 ).rgb;
						#endif
					#else
						lightMaskMultiplier = textureCubeArrayLod( s_lightMaskPoint, vec4( dir, lightMaskIndex ), 0 ).rgb;
					#endif
				}
				else if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
				{
					vec4 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) );
					#ifdef GLSL
						#ifdef SHADOW_MAP
							lightMaskMultiplier = texture2DArrayLod( s_shadowMapShadowSpot, vec3( texCoord.xy / texCoord.w, lightMaskIndex ), 0 ).rgb;
						#endif
					#else
						lightMaskMultiplier = texture2DArrayLod( s_lightMaskSpot, vec3( texCoord.xy / texCoord.w, lightMaskIndex ), 0 ).rgb;
					#endif
					//lightMaskMultiplier = texture2DProj( s_lightMask, texCoord ).rgb;
				}
				else //if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
#endif
				{
 					vec2 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) ).xy;
					#ifdef GLSL
						#ifdef SHADOW_MAP
							lightMaskMultiplier = texture2DArrayLod( s_shadowMapShadowDirectional, vec3( texCoord, lightMaskIndex ), 0 ).rgb;
						#endif
					#else
						lightMaskMultiplier = texture2DArrayLod( s_lightMaskDirectional, vec3( texCoord, lightMaskIndex ), 0 ).rgb;
					#endif
					//lightMaskMultiplier = texture2DLod( s_lightMaskDirectional, texCoord, 0 ).rgb;
					////lightMaskMultiplier = texture2D( s_lightMaskDirectional, texCoord ).rgb;
				}
			}
		#endif

			//shadows
			MEDIUMP float shadowMultiplier = 1.0;
		#ifdef SHADOW_MAP
			MEDIUMP float shadowMapIndex = d_shadowMapIndex;
			BRANCH
			if( shadowMapIndex >= 0.0 )
			{
				//no contact shadows for forward because can't read from depth buffer
				vec2 uvScreen = vec2_splat(0);//fragCoord.xy * u_viewTexel.xy; flip y?
				
				//use inputWorldNormal instead normal?
				shadowMultiplier = getShadowMultiplierMulti( worldPosition, cameraDistance, v_worldPosition_depth.w, lightWorldDirection, normal, uvScreen, fragCoord, lightType, shadowMapIndex, nLight );
			}		
		#endif

			//light color
			vec3 lightColor = ( d_lightPower * 10000.0 ) * u_cameraExposure * objectLightAttenuation * lightMaskMultiplier * shadowMultiplier;

			#ifdef SHADING_MODEL_SIMPLE
				//Simple model
				resultColor.rgb += baseColor * lightColor;
			#else
				//PBR

				#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE
					resultColor.rgb += baseColor * lightColor * saturate(dot(normal, lightWorldDirection) / PI);
				#else
			
					MaterialInputs material;

					material.baseColor = vec4(baseColor, 0.0);
					material.roughness = roughness;
					material.metallic = metallic;
					material.reflectance = reflectance;
					material.ambientOcclusion = ambientOcclusion;
					//material.emissive = vec4(emissive, 0.0);

					#ifdef MATERIAL_HAS_ANISOTROPY
						material.anisotropy = anisotropy * anisotropyDirectionBasis;
						material.anisotropyDirection = anisotropyDirection;
					#endif

					#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
						material.thickness = thickness;
						material.subsurfacePower = subsurfacePower;
						material.subsurfaceColor = subsurfaceColor;
					#endif

					#ifdef MATERIAL_HAS_CLEAR_COAT
						material.clearCoat = clearCoat;
						material.clearCoatRoughness = clearCoatRoughness;
						material.clearCoatNormal = clearCoatNormal;
					#endif

					#if defined(SHADING_MODEL_CLOTH)
						material.sheenColor = sheenColor;
						material.subsurfaceColor = subsurfaceColor;
					#endif

					ShadingParams shading;
					getPBRFilamentShadingParams(material, tangent.xyz, bitangent, normal, inputWorldNormal, lightWorldDirection/*toLight*/, -fromCameraDirection, gl_FrontFacing, shading);
					
					PixelParams pixel;
					getPBRFilamentPixelParams(material, shading, pixel);

					resultColor.rgb += surfaceShading(pixel, shading) * lightColor;
					
				#endif //GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE
			#endif //SHADING_MODEL_SIMPLE
			
		}
	}

	//ambient light
	{
		vec3 ambientLightPower = texelFetch( s_lightsTexture, ivec2( 2, 0 ), 0 ).xyz * 10000.0;
		
		//light color
		MEDIUMP vec3 lightColor = ambientLightPower * u_cameraExposure;
		
		#ifdef SHADING_MODEL_SIMPLE
			//Simple model
			resultColor.rgb += baseColor * lightColor;
		#else
			//PBR

			#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE
				resultColor.rgb += baseColor * lightColor;
			#else
		
				MaterialInputs material;

				material.baseColor = vec4(baseColor, 0.0);
				material.roughness = roughness;
				material.metallic = metallic;
				material.reflectance = reflectance;
				material.ambientOcclusion = ambientOcclusion;
				//material.emissive = vec4(emissive, 0.0);

				#ifdef MATERIAL_HAS_ANISOTROPY
					material.anisotropy = anisotropy * anisotropyDirectionBasis;
					material.anisotropyDirection = anisotropyDirection;
				#endif

				#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
					material.thickness = thickness;
					material.subsurfacePower = subsurfacePower;
					material.subsurfaceColor = subsurfaceColor;
				#endif

				#ifdef MATERIAL_HAS_CLEAR_COAT
					material.clearCoat = clearCoat;
					material.clearCoatRoughness = clearCoatRoughness;
					material.clearCoatNormal = clearCoatNormal;
				#endif

				#if defined(SHADING_MODEL_CLOTH)
					material.sheenColor = sheenColor;
					material.subsurfaceColor = subsurfaceColor;
				#endif

				ShadingParams shading;
				getPBRFilamentShadingParams(material, tangent.xyz, bitangent, normal, inputWorldNormal, normal/*toLight*/, -fromCameraDirection/*toCamera*/, gl_FrontFacing, shading);
				
				shadingView = shading.view;
				shadingNoV = shading.NoV;

				PixelParams pixel;
				getPBRFilamentPixelParams(material, shading, pixel);

				EnvironmentTextureData data1;
				data1.rotation = u_forwardEnvironmentDataRotation1;
				data1.multiplierAndAffect = u_forwardEnvironmentDataMultiplierAndAffect1;
				
				EnvironmentTextureData data2;
				data2.rotation = u_forwardEnvironmentDataRotation2;
				data2.multiplierAndAffect = u_forwardEnvironmentDataMultiplierAndAffect2;

				vec4 screenSpaceReflection = vec4_splat( 0 );		
				//!!!!sampler limit
				/*
			//#ifdef SSR
			#ifndef MOBILE 
				BRANCH
				if( u_ssrEnabled )
				{
					vec2 texCoord = fragCoord.xy * u_viewportSizeInv;
					screenSpaceReflection = texture2DArrayLod( s_ssrTexture, vec3( texCoord, roughness * 3.0 ), 0 );
				}
			#endif
			//#endif
				*/
				
				vec3 color1 = 
					iblDiffuse(material, pixel, shading, u_forwardEnvironmentIrradiance1, data1, s_environmentTexture1, data1, true, true) +
					iblSpecular(material, pixel, shading, screenSpaceReflection.xyz, screenSpaceReflection.w/*vec3_splat(0), 0.0*/, s_environmentTexture1, data1);
#ifdef GLOBAL_ENVIRONMENT_MAP_MIXING
				BRANCH
				if( u_forwardEnvironmentDataBlendingFactor < 1.0 )
				{
					vec3 color2 = 
						iblDiffuse(material, pixel, shading, u_forwardEnvironmentIrradiance2, data2, s_environmentTexture2, data2, true, true) + 
						iblSpecular(material, pixel, shading, screenSpaceReflection.xyz, screenSpaceReflection.w/*vec3_splat(0), 0.0*/, s_environmentTexture2, data2);					
					resultColor.rgb += mix( color2, color1, u_forwardEnvironmentDataBlendingFactor ) * lightColor;
				}
				else
					resultColor.rgb += color1 * lightColor;
#else
				resultColor.rgb += color1 * u_forwardEnvironmentDataBlendingFactor * lightColor;
#endif
				

				//can split blending factor to two, blending factor diffuse and blending factor specular
				//{				
				//	vec3 color1Diffuse = iblDiffuse(material, pixel, shading, u_forwardEnvironmentIrradiance1, data1, s_environmentTexture1, data1, true);
				//	vec3 color1Specular = iblSpecular(material, pixel, shading, vec3_splat(0), 0.0, s_environmentTexture1, data1);

				//	vec3 color2Diffuse = iblDiffuse(material, pixel, shading, u_forwardEnvironmentIrradiance2, data2, s_environmentTexture2, data2, true);
				//	vec3 color2Specular = iblSpecular(material, pixel, shading, vec3_splat(0), 0.0, s_environmentTexture2, data2);
					
				//	float diffuseFactor = u_forwardEnvironmentDataBlendingFactor;
				//	float specularFactor = u_forwardEnvironmentDataBlendingFactor;
						
				//	resultColor.rgb += mix(color2Diffuse, color1Diffuse, diffuseFactor) * lightColor;
				//	resultColor.rgb += mix(color2Specular, color1Specular, specularFactor) * lightColor;
				//}
				
				//resultColor.rgb = u_envFactor * color1 + (1 - u_envFactor) * color2;
				//resultColor.rgb *= lightColor;
				
			#endif //GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE
		#endif //SHADING_MODEL_SIMPLE
	}
	
#else //!SHADING_MODEL_UNLIT
	//Unlit
	resultColor.rgb = baseColor;
#endif //SHADING_MODEL_UNLIT
	
		
	//emissive
	resultColor.rgb += emissive * u_emissiveMaterialsFactor;

	//blend mode Add. apply alpha
	#ifdef BLEND_MODE_ADD
		resultColor.rgb *= opacity;
	#endif

	//resultColor.a
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		//#ifndef LIGHT_TYPE_AMBIENT
			resultColor.a = 1.0;
		//#else
		//	resultColor.a = 0.0;
		//#endif
	#endif
	#ifdef BLEND_MODE_TRANSPARENT
		resultColor.a = opacity;
	#endif
	
	//lod for transparent
#ifdef GLOBAL_SMOOTH_LOD
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)
		bool isLayer = u_renderOperationData[3].z != 0.0;
		if(isLayer)
			smoothLOD(fragCoord, lodValue);
		else
		{
			if(lodValue != 0.0)
			{
				MEDIUMP float factor;
				if(lodValue > 0.0)
					factor = 1.0 - lodValue;
				else
					factor = -lodValue;

				#if defined(BLEND_MODE_TRANSPARENT)
					resultColor.a *= factor;
				#endif
				#if defined(BLEND_MODE_ADD)	
					resultColor.rgb *= factor;
				#endif
			}
		}
	#endif
#endif

	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	#if defined(BLEND_MODE_TRANSPARENT)
		resultColor.a *= visibilityDistanceFactor;
	#endif
	#if defined(BLEND_MODE_ADD)	
		resultColor.rgb *= visibilityDistanceFactor;
	#endif
#endif

	//soft particles
#ifdef SOFT_PARTICLES
	BRANCH
	if(u_provideColorDepthTextureCopy > 0.0)
	{
		//get depth
		vec2 texCoord = fragCoord.xy * u_viewportSizeInv;
		float rawDepth = texture2D(s_colorDepthTextureCopy, texCoord).w; //.g;
		float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);

		MEDIUMP float softParticlesFactor = saturate((depth - v_worldPosition_depth.w) / softParticlesDistance);
		
		#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
			smoothLOD(fragCoord, 1.0f - softParticlesFactor);
		#endif
		#if defined(BLEND_MODE_TRANSPARENT)
			resultColor.a *= softParticlesFactor;
		#endif
		#if defined(BLEND_MODE_ADD)	
			resultColor.rgb *= softParticlesFactor;
		#endif
	}
#endif

	//fog
#ifdef GLOBAL_FOG
	#ifdef BLEND_MODE_TRANSPARENT
		resultColor *= v_fogFactor;
		//#ifdef LIGHT_TYPE_AMBIENT
			resultColor.rgb += u_fogColor.rgb * (1.0 - v_fogFactor);
		//#endif
	#endif
	#ifdef BLEND_MODE_ADD
		resultColor *= v_fogFactor;
	#endif
#endif

	//debug mode
#ifdef GLOBAL_DEBUG_MODE
	BRANCH
	if(u_viewportOwnerDebugMode != DebugMode_None)
	{
		//#ifdef LIGHT_TYPE_AMBIENT
			switch(u_viewportOwnerDebugMode)
			{
			case DebugMode_Geometry: resultColor.rgb = vec3_splat(abs(dot(inputWorldNormal, shadingView))); break;
			case DebugMode_Surface: resultColor.rgb = vec3_splat(shadingNoV); break;
			case DebugMode_BaseColor: resultColor.rgb = baseColor; break;
			case DebugMode_Metallic: resultColor.rgb = vec3_splat(metallic); break;
			case DebugMode_Roughness: resultColor.rgb = vec3_splat(roughness); break;
			case DebugMode_Reflectance: resultColor.rgb = vec3_splat(reflectance); break;
			case DebugMode_Emissive: resultColor.rgb = emissive; break;
			case DebugMode_Normal: resultColor.rgb = normal * 0.5 + 0.5; break;//inputWorldNormal * 0.5 + 0.5; break
			case DebugMode_SubsurfaceColor: resultColor.rgb = subsurfaceColor; break;
			case DebugMode_TextureCoordinate0: resultColor.rgb = vec3(v_texCoord01.xy, 0); break;
			case DebugMode_TextureCoordinate1: resultColor.rgb = vec3(v_texCoord01.zw, 0); break;
			case DebugMode_TextureCoordinate2: resultColor.rgb = vec3(v_texCoord23.xy, 0); break;
			}
		//#else
		//	if(u_viewportOwnerDebugMode != DebugMode_Wireframe)
		//		resultColor = vec4_splat(0);
		//#endif
	}
#endif


	//color
	gl_FragData[0] = resultColor;

	//normal
#ifndef MOBILE
	//#ifdef LIGHT_TYPE_AMBIENT
		gl_FragData[1] = vec4(normal * 0.5 + 0.5, resultColor.a);
	//#else
	//	gl_FragData[1] = vec4(0,0,0,0);
	//#endif
#endif

	/*
	//oit
#ifndef MOBILE
	BRANCH
	if( u_forwardOIT.x > 0.0 )
	{
		vec4 color = resultColor;

		//!!!!good?
		float depth = v_worldPosition_depth.w; //gl_FragCoord.z;//v_pos.z / v_pos.w;
		
		//float depth = gl_FragCoord.z / gl_FragCoord.w;//v_pos.z / v_pos.w;
		
		//!!!!good formula? add configurable parameters?
		float weight = color.a * clamp( 0.03 / ( 1e-5 + pow( depth / 200.0, 5.0 ) ), 0.01, 3000.0 );
		
		//float weight = max(min(1.0, max(max(color.r, color.g), color.b) * color.a), color.a) * clamp(0.03 / (1e-5 + pow(gl_FragCoord.z / 200.0, 4.0)), 1e-2, 3e3);

		gl_FragData[0] = vec4( color.rgb * color.a, color.a ) * weight;
		gl_FragData[1] = vec4( color.a, 0, 0, 0 );
	}
#endif
	*/

	//motion vector
#ifdef GLOBAL_MOTION_VECTOR
	//#ifdef LIGHT_TYPE_AMBIENT
		vec2 aa = (v_position.xy / v_position.w) * 0.5 + 0.5;
		vec2 bb = (v_previousPosition.xy / v_previousPosition.w) * 0.5 + 0.5;
		vec2 velocity = (aa - bb) * v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.w;
		gl_FragData[2] = vec4(velocity.x,velocity.y,0.0,resultColor.a);
	//#else
	//	gl_FragData[2] = vec4(0,0,0,0);
	//#endif
#endif

	//geometry with voxel data
	
//!!!!GLSL
#ifndef GLSL
#if (defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )) // || defined(DEPTH_OFFSET_MODE_GREATER_EQUAL)	
//#if defined(GLOBAL_VOXEL_LOD) || defined(GLOBAL_VIRTUALIZED_GEOMETRY)// || defined(DEPTH_OFFSET_MODE_GREATER_EQUAL)
	BRANCH
	if(depthOffset != 0.0)
	{
		float depth = getDepthValue(fragCoord.z, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
		depth += depthOffset;
#ifdef REVERSEDZ
		gl_LessEqualFragDepth = getRawDepthValue(depth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
#else
		gl_GreaterEqualFragDepth = getRawDepthValue(depth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
#endif
	}
	else
	{
#ifdef REVERSEDZ
		gl_LessEqualFragDepth = fragCoord.z;
#else
		gl_GreaterEqualFragDepth = fragCoord.z;
#endif
	}
	
/*
#elif defined(DEPTH_OFFSET_MODE_LESS_EQUAL)
	BRANCH
	if(depthOffset != 0.0)
	{
		float depth = getDepthValue(fragCoord.z, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
		depth += depthOffset;
		gl_LessEqualFragDepth = getRawDepthValue(depth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
	}
	else
		gl_LessEqualFragDepth = fragCoord.z;
*/

#endif
#endif

/*
	//!!!!test
	
#if defined( BLEND_MODE_TRANSPARENT ) || defined( BLEND_MODE_ADD )
	imageStore( s_oitScreen, ivec2( fragCoord.xy ), vec4(1,0,0,1) );
	//imageStore( s_oitScreen, ivec2( fragCoord.xy ), 1u );

	//imageStore( s_oitScreen, ivec2( 10,10 ), 1u );

	//imageStore( s_oitScreen, ivec2( fragCoord.xy ), uvec4( 1, 0, 0, 0 ) );		
#endif
*/

}
