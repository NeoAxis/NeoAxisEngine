// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#define GI 1
#include "Common/bgfx_compute.sh"
#include "Common.sh"
#include "GICommon.sh"
#include "UniformsFragment.sh"
#include "FragmentFunctions.sh"

uniform vec4 giRenderToGridParameters[2];

//!!!!instancing
uniform vec4 giObjectData[6];

UIMAGE3D_WR(s_gbufferGrid, rg32ui, 0);
IMAGE3D_WR(s_lightingGrid, rgba16f, 1);

uniform vec4 u_renderOperationData[7];
uniform vec4 u_materialCustomParameters[2];
//!!!!
SAMPLER2D(s_materials, 3);
//SAMPLER2D(s_materials, 1);
#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)
	SAMPLER2D(s_voxelData, 2);
#endif
SAMPLER2D(s_linearSamplerFragment, 10);

//#ifdef DISPLACEMENT_CODE_PARAMETERS
//	DISPLACEMENT_CODE_PARAMETERS
//#endif
#ifdef FRAGMENT_CODE_PARAMETERS
	FRAGMENT_CODE_PARAMETERS
#endif
#ifdef MATERIAL_INDEX_CODE_PARAMETERS
	MATERIAL_INDEX_CODE_PARAMETERS
#endif

//#ifdef DISPLACEMENT_CODE_SAMPLERS
//	DISPLACEMENT_CODE_SAMPLERS
//#endif
#ifdef FRAGMENT_CODE_SAMPLERS
	FRAGMENT_CODE_SAMPLERS
#endif
#ifdef MATERIAL_INDEX_CODE_SAMPLERS
	MATERIAL_INDEX_CODE_SAMPLERS
#endif

//#ifdef DISPLACEMENT_CODE_SHADER_SCRIPTS
//	DISPLACEMENT_CODE_SHADER_SCRIPTS
//#endif
//#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
//	#include "Paral_laxOcclusionMapping.sh"
//#endif
#ifdef FRAGMENT_CODE_SHADER_SCRIPTS
	FRAGMENT_CODE_SHADER_SCRIPTS
#endif
#ifdef MATERIAL_INDEX_CODE_SHADER_SCRIPTS
	MATERIAL_INDEX_CODE_SHADER_SCRIPTS
#endif

#ifdef MULTI_MATERIAL_COMBINED_PASS
	uniform vec4 u_multiMaterialCombinedInfo;
	uniform vec4 u_multiMaterialCombinedMaterials[4];
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*
void giVoxelDataModeCalculateParametersF( float voxelDataMode, sampler2D voxelData, vec4 fragCoord, vec3 voxelObjectSpacePosition, vec3 voxelCameraPositionObjectSpace, vec4 worldMatrixRotation, vec3 worldMatrixScale, vec4 renderOperationData[ 7 ], inout vec3 inputWorldNormal, inout vec4 tangent, inout vec2 texCoord0, inout vec2 texCoord1, inout vec2 texCoord2, inout vec4 color0, inout float voxelLengthInside, inout int materialIndex )
{
}
*/

NUM_THREADS(8, 8, 8)
void main()
{
	float giCellSize = giRenderToGridParameters[ 0 ].x;
	float giVoxelizationFactor = giRenderToGridParameters[ 0 ].y;
	float gridResolution = giRenderToGridParameters[ 0 ].z;
	float currentLevel = giRenderToGridParameters[ 0 ].w;
	vec3 gridPosition = giRenderToGridParameters[ 1 ].xyz;
	float emissiveLighting = giRenderToGridParameters[ 1 ].w;

	mat4 objectTransform = mtxFromRows( giObjectData[ 0 ], giObjectData[ 1 ], giObjectData[ 2 ], vec4(0,0,0,1) );
	vec4 v_colorParameter = giObjectData[ 3 ];
	vec4 giGridIndexesMin = giObjectData[ 4 ];
	vec4 giGridIndexesMax = giObjectData[ 5 ];
	
	ivec3 giGridIndexOffset = ivec3( giGridIndexesMin.xyz );
	ivec3 giGridIndexSize = ivec3( giGridIndexesMax.xyz ) - giGridIndexOffset + ivec3(1,1,1);

	//!!!!BRANCH? where else
	if( gl_GlobalInvocationID.x >= giGridIndexSize.x || gl_GlobalInvocationID.y >= giGridIndexSize.y || gl_GlobalInvocationID.z >= giGridIndexSize.z )
		return;
	
	ivec3 giGridIndex = giGridIndexOffset + ivec3( gl_GlobalInvocationID );
	vec3 worldPosition = gridPosition.xyz + ( vec3( giGridIndex ) + vec3_splat( 0.5 ) ) * giCellSize;
	//vec3 worldPosition = gridCenter.xyz - vec3_splat( gridSizeOfLevel / 2 ) + ( vec3( giGridIndex ) + vec3_splat( 0.5 ) ) * giCellSize;
	
	//float voxelDataMode = u_renderOperationData[1].w;
	
	
//	//lod
//#ifdef GLOBAL_SMOOTH_LOD
//	float lodValue = v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.x;
//	smoothLOD(fragCoord, lodValue);
//#endif

	vec2 texCoord0 = vec2_splat(0);//v_texCoord01.xy;
	vec2 texCoord1 = vec2_splat(0);//v_texCoord01.zw;
	vec2 texCoord2 = vec2_splat(0);//v_texCoord23.xy;
	vec2 unwrappedUV = vec2_splat(0);//getUnwrappedUV(texCoord0, texCoord1, texCoord2, u_renderOperationData[3].x);
	vec3 inputWorldNormal = vec3_splat(0);//v_worldNormal_materialIndex.xyz;
	vec4 tangent = vec4_splat(0);//v_tangent;
	vec4 color0 = vec4_splat(1);//v_color0;
	//vec3 fromCameraDirection = normalize(v_worldPosition_depth.xyz - u_viewportOwnerCameraPosition);

	int materialIndex = 0;
	float depthOffset = 0.0;
//#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)

	//make function
	{
		//get voxel data info
		vec3 gridSizeF = u_renderOperationData[ 5 ].xyz;
		float fillHolesDistanceAndFormat = u_renderOperationData[ 5 ].w;
		float fillHolesDistance = abs( fillHolesDistanceAndFormat );
		bool fullFormat = fillHolesDistanceAndFormat < 0.0;
		vec3 boundsMin = u_renderOperationData[ 6 ].xyz;
		float cellSize = u_renderOperationData[ 6 ].w;
		ivec3 gridSize = ivec3( gridSizeF );
		vec3 boundsMax = boundsMin + gridSizeF * cellSize;

		//!!!!slowly. no sense to calculate it for each texel
		mat4 objectTransformInv = matInverse( objectTransform );
		vec3 localPosition = mul( objectTransformInv, vec4( worldPosition, 1 ) ).xyz;

		//convert to grid space
		vec3 currentPosition = localPosition;
		currentPosition -= boundsMin;
		currentPosition /= ( boundsMax - boundsMin );
		currentPosition = clamp( currentPosition, vec3_splat( 0 ), vec3_splat( 0.9999 ) ); //to have valid grid index
		currentPosition *= gridSizeF;

		ivec3 currentIndex = ivec3( currentPosition );
		
		//currentIndex = clamp( currentIndex, vec3_splat( 0 ), gridSize );
		////za predelami toshe moshno nearest yusat
		//if( currentIndex.x < 0 || currentIndex.y < 0 || currentIndex.z < 0 )
		//	return;
		//if( currentIndex.x >= gridSize.x || currentIndex.y >= gridSize.y || currentIndex.z >= gridSize.z )
		//	return;

		int voxelTextureSize = textureSize( s_voxelData, 0 ).x;
		float voxelValue = getVoxelValue( s_voxelData, voxelTextureSize, getVoxelBufferIndexOfVoxel( gridSize, currentIndex ) );
		//get nearest voxel
		if( voxelValue <= 0.0 )
		{
			currentIndex = getVoxelNearestIndexFromValue256( voxelValue );
			voxelValue = getVoxelValue( s_voxelData, voxelTextureSize, getVoxelBufferIndexOfVoxel( gridSize, currentIndex ) );
		}

		
		//clip too far cells
		
		//get local position of the voxel by the index
		vec3 voxelLocalPosition = vec3( currentIndex ) + vec3_splat( 0.5 );
		voxelLocalPosition /= gridSizeF;
		voxelLocalPosition *= ( boundsMax - boundsMin );
		voxelLocalPosition += boundsMin;
		
		//get closest position to gi world position inside bounds of the voxel
		vec3 halfCellSize3 = vec3_splat( cellSize / 2 );
		vec3 voxelLocalPositionMin = voxelLocalPosition - halfCellSize3;
		vec3 voxelLocalPositionMax = voxelLocalPosition + halfCellSize3;
		voxelLocalPosition = clamp( localPosition, voxelLocalPositionMin, voxelLocalPositionMax );
		
		vec3 voxelWorldPosition = mul( objectTransform, vec4( voxelLocalPosition, 1 ) ).xyz;		
		vec3 absDistances = abs( voxelWorldPosition - worldPosition );
		float compareValue = giCellSize * 0.5 * giVoxelizationFactor;
		if( absDistances.x > compareValue || absDistances.y > compareValue || absDistances.z > compareValue )
			return;
		
		
		//read voxel data
		
		int dataStartIndex = int( voxelValue );
		
		vec2 data0 = unpackHalf2x16( asuint( getVoxelValue( s_voxelData, voxelTextureSize, dataStartIndex + 0 ) ) );
		vec2 data1 = unpackHalf2x16( asuint( getVoxelValue( s_voxelData, voxelTextureSize, dataStartIndex + 1 ) ) );
		vec2 data2 = unpackHalf2x16( asuint( getVoxelValue( s_voxelData, voxelTextureSize, dataStartIndex + 2 ) ) );
		vec2 data3 = unpackHalf2x16( asuint( getVoxelValue( s_voxelData, voxelTextureSize, dataStartIndex + 3 ) ) );

		materialIndex = int( data0.x );

		vec2 normalSpherical;
		vec2 data0c = ( ( data0 % 1.0 ) - 0.5 ) * PI;
		normalSpherical.x = data0c.x * 2.0;
		normalSpherical.y = data0c.y;

		vec3 normalObjectSpace = sphericalDirectionGetVector( normalSpherical );
		inputWorldNormal = mul( objectTransform, vec4( normalObjectSpace, 0 ) ).xyz;
		//inputWorldNormal = mulQuat( worldMatrixRotation, normalObjectSpace );
		
		texCoord0 = data1;

		tangent.xy = data2;
		tangent.zw = data3;
		tangent.xyz = mul( objectTransform, vec4( tangent.xyz, 0 ) ).xyz;
		//tangent.xyz = mulQuat( worldMatrixRotation, tangent.xyz );
		
		BRANCH
		if( fullFormat )
		{
			vec2 data4 = unpackHalf2x16( asuint( getVoxelValue( s_voxelData, voxelTextureSize, dataStartIndex + 4 ) ) );
			vec2 data5 = unpackHalf2x16( asuint( getVoxelValue( s_voxelData, voxelTextureSize, dataStartIndex + 5 ) ) );
			vec2 data6 = unpackHalf2x16( asuint( getVoxelValue( s_voxelData, voxelTextureSize, dataStartIndex + 6 ) ) );
			vec2 data7 = unpackHalf2x16( asuint( getVoxelValue( s_voxelData, voxelTextureSize, dataStartIndex + 7 ) ) );

			texCoord1 = data4;
			texCoord2 = data5;
			color0.xy = data6;
			color0.zw = data7;
		}	
	}

	
//#else
//	materialIndex = round(v_worldNormal_materialIndex.w);
//#endif
	//worldPosition += fromCameraDirection * depthOffset;

	//float cameraDistance = length(worldPosition - u_viewportOwnerCameraPosition);

	//!!!!temp
	float cameraDistance = 0.0;
	vec3 cameraPosition = vec3_splat(0);

//	//fading by visibility distance
//#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
//	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y;
//	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
//	smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
//#endif

	if( cutVolumes( worldPosition ) )
		return;

	inputWorldNormal = normalize( inputWorldNormal );
	tangent.xyz = normalize( tangent.xyz );
	
	//end of geometry stage

	//shading
	
	//!!!!compute lod = 0. gde escho

	//multi material
#ifdef MATERIAL_INDEX_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, 0)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, 0)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, 0)
	//#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(makeSampler(s_linearSamplerFragment, _sampler), _uv, 0, 0)
	//#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
	//#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)	
	{
		MATERIAL_INDEX_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	//get material data
	uint frameMaterialIndex = uint( u_renderOperationData[ 0 ].x );
#ifdef MULTI_MATERIAL_COMBINED_PASS
	uint localGroupMaterialIndex = uint( materialIndex ) - uint( u_multiMaterialCombinedInfo.x );
	BRANCH
	if( localGroupMaterialIndex < 0 || localGroupMaterialIndex >= uint( u_multiMaterialCombinedInfo.y ) )
		return;
	frameMaterialIndex = uint( u_multiMaterialCombinedMaterials[ localGroupMaterialIndex / 4 ][ localGroupMaterialIndex % 4 ] );
#endif
	vec4 materialStandardFragment[ MATERIAL_STANDARD_FRAGMENT_SIZE ];
	getMaterialData( s_materials, frameMaterialIndex, materialStandardFragment );

	//multi material
#ifdef MULTI_MATERIAL_SEPARATE_PASS
	if( materialIndex != u_materialMultiSubMaterialSeparatePassIndex )
		return;
#endif

	//displacement
	vec2 displacementOffset = vec2_splat(0);
//#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
//	BRANCH
//	if(u_displacementScale != 0.0 && voxelDataMode == 0.0)
//		displacementOffset = getParallaxOcclusionMappingOffset(texCoord0, v_eyeTangentSpace, v_normalTangentSpace, u_materialDisplacementScale * u_displacementScale, u_displacementMaxSteps);
//#endif

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
	vec3 anisotropyDirection = u_materialAnisotropyDirection;
	float thickness = u_materialThickness;
	float subsurfacePower = u_materialSubsurfacePower;
	vec3 sheenColor = u_materialSheenColor;
	vec3 subsurfaceColor = u_materialSubsurfaceColor;
	float ambientOcclusion = 1;
	float rayTracingReflection = u_materialRayTracingReflection;
	vec3 emissive = u_materialEmissive;
	vec4 customParameter1 = u_materialCustomParameters[0];
	vec4 customParameter2 = u_materialCustomParameters[1];
	
	//get material parameters (procedure generated code)
	vec2 texCoord0BeforeDisplacement = texCoord0;
	vec2 texCoord1BeforeDisplacement = texCoord1;
	vec2 texCoord2BeforeDisplacement = texCoord2;
	vec2 unwrappedUVBeforeDisplacement = unwrappedUV;
	texCoord0 -= displacementOffset;
	texCoord1 -= displacementOffset;
	texCoord2 -= displacementOffset;
	unwrappedUV -= displacementOffset;
	
	int shadingModel = SHADING_MODEL_INDEX;
	//!!!!
	bool receiveDecals = false;
	//bool receiveDecals = u_materialReceiveDecals != 0.0 && v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.z != 0.0;
	bool useVertexColor = u_materialUseVertexColor != 0.0;
	bool opacityDithering = false;
#ifdef OPACITY_DITHERING
	opacityDithering = true;
#endif

//!!!!moshet lod ne 0 brat

#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, 0)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, 0)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, 0)
	{
		FRAGMENT_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif
	//#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_renderOperationData[3].z, 0)
	//#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling)
	//#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)


	//apply color parameter
	
	baseColor *= v_colorParameter.xyz;
	if(useVertexColor)
		baseColor *= color0.xyz;
	baseColor = max(baseColor, vec3_splat(0));
	
	opacity *= v_colorParameter.w;
	if(useVertexColor)
		opacity *= color0.w;
	opacity = saturate(opacity);
	
#ifdef SHADING_MODEL_SUBSURFACE
	subsurfaceColor *= v_colorParameter.xyz;
	if(useVertexColor)
		subsurfaceColor *= color0.xyz;
	subsurfaceColor = max(subsurfaceColor, vec3_splat(0));
#endif

//!!!!need?
	//get result world normal
#if defined(GLOBAL_NORMAL_MAPPING) && GLOBAL_MATERIAL_SHADING != GLOBAL_MATERIAL_SHADING_SIMPLE
	BRANCH
	if(any2(normal))
	{
		vec3 bitangent = cross(tangent.xyz, inputWorldNormal) * tangent.w;
		mat3 tbn = transpose(mtxFromRows(tangent.xyz, bitangent, inputWorldNormal));
		normal = expand(normal);
		normal.z = sqrt(max(1.0 - dot(normal.xy, normal.xy), 0.0));
		normal = normalize(mul(tbn, normal));
	}
	else
		normal = inputWorldNormal;
#else
	normal = inputWorldNormal;
#endif

//#ifdef TWO_SIDED_FLIP_NORMALS
//	if(gl_FrontFacing)
//		normal = -normal;
//#endif

#ifndef SHADING_MODEL_SUBSURFACE
	thickness = 0;
	subsurfacePower = 0;
	subsurfaceColor = vec3_splat(0);
#endif
#ifdef MULTI_MATERIAL_COMBINED_PASS
	if(shadingModel != 1)//Subsurface
	{
		thickness = 0;
		subsurfacePower = 0;
		subsurfaceColor = vec3_splat(0);
	}
#endif

#ifdef SHADING_MODEL_SIMPLE
	reflectance = 0;
	metallic = 0;
	roughness = 0;
	ambientOcclusion = 0;
	rayTracingReflection = 0;
#endif
#ifdef MULTI_MATERIAL_COMBINED_PASS
	if(shadingModel == 3)//Simple
	{
		reflectance = 0;
		metallic = 0;
		roughness = 0;
		ambientOcclusion = 0;
		rayTracingReflection = 0;
	}
#endif


	//write result
	
//!!!!? reflectance. shader model. what else

	uvec4 gbuffer = uvec4_splat( 0 );
	gbuffer.x = convertVec4ToRGBA8( vec4( normal * 0.5 + 0.5, roughness ) * 255.0 );
	gbuffer.y = convertVec4ToRGBA8( vec4( baseColor, metallic ) * 255.0 );
	
	giGridIndex.x += int( gridResolution * currentLevel );
	
	imageStore( s_gbufferGrid, giGridIndex, gbuffer );
	//write opacity anyway even when emissive is zero
	imageStore( s_lightingGrid, giGridIndex, vec4( emissive * emissiveLighting, 1.0 ) );

	
/*	
	result.z = convertVec4ToRGBA8( vec4( metallic, roughness, 1.0, rayTracingReflection ) * 255.0 );
	//result.z = convertVec4ToRGBA8( vec4( metallic, roughness, ambientOcclusion, rayTracingReflection ) * 255.0 );
#ifdef SHADING_MODEL_SUBSURFACE
	//gl_FragData[3] = encodeRGBE8(subsurfaceColor);
#endif
	//gl_FragData[4] = tangent * 0.5 + 0.5;//gl_FragData[4] = encodeGBuffer4(tangent, shadingModelSimple, receiveDecals);
	//gl_FragData[5] = vec4(float(shadingModel) / 8.0, receiveDecals ? 1.0 : 0.0, thickness, subsurfacePower / 15.0);
*/

		
}
