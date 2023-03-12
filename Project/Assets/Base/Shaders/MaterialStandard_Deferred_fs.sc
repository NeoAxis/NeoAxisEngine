$input v_texCoord01, v_worldPosition_depth, v_worldNormal_materialIndex, v_tangent, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_position, v_previousPosition, v_texCoord23, v_colorParameter, v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define DEFERRED 1
#include "Common.sh"
#include "UniformsFragment.sh"
#include "FragmentFunctions.sh"

uniform vec4 u_renderOperationData[7];
uniform vec4 u_materialCustomParameters[2];
SAMPLER2D(s_materials, 1);
#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)
	SAMPLER2D(s_voxelData, 2);
#endif
#if defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)
	SAMPLER2D(s_virtualizedData, 11);
#endif
SAMPLER2D(s_linearSamplerFragment, 10);

#ifdef DISPLACEMENT_CODE_PARAMETERS
	DISPLACEMENT_CODE_PARAMETERS
#endif
#ifdef FRAGMENT_CODE_PARAMETERS
	FRAGMENT_CODE_PARAMETERS
#endif
#ifdef MATERIAL_INDEX_CODE_PARAMETERS
	MATERIAL_INDEX_CODE_PARAMETERS
#endif

#ifdef DISPLACEMENT_CODE_SAMPLERS
	DISPLACEMENT_CODE_SAMPLERS
#endif
#ifdef FRAGMENT_CODE_SAMPLERS
	FRAGMENT_CODE_SAMPLERS
#endif
#ifdef MATERIAL_INDEX_CODE_SAMPLERS
	MATERIAL_INDEX_CODE_SAMPLERS
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

#ifdef MULTI_MATERIAL_COMBINED_PASS
	uniform vec4 u_multiMaterialCombinedInfo;
	uniform vec4 u_multiMaterialCombinedMaterials[8];
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	vec4 fragCoord = getFragCoord();
	float voxelDataMode = u_renderOperationData[1].w;
	//float virtualizedDataMode = u_renderOperationData[3].w;

	//lod
#ifdef GLOBAL_SMOOTH_LOD
	float lodValue = v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.x;
	smoothLOD(fragCoord, lodValue);
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

	int materialIndex = 0;
	float depthOffset = 0.0;
#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)
	voxelDataModeCalculateParametersF(voxelDataMode, s_voxelData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, fromCameraDirection, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex);
//#elif defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)
//	virtualizedDataModeCalculateParametersF(virtualizedDataMode, s_virtualizedData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, gl_PrimitiveID, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex);
#else
	materialIndex = int(round(v_worldNormal_materialIndex.w));
#endif
	worldPosition += fromCameraDirection * depthOffset;

	float cameraDistance = length(worldPosition - u_viewportOwnerCameraPosition);
	vec3 cameraPosition = u_viewportOwnerCameraPosition;

	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
	smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
#endif

	if( cutVolumes( worldPosition ) )
		discard;

	inputWorldNormal = normalize(inputWorldNormal);
	tangent.xyz = normalize(tangent.xyz);
	
	//end of geometry stage
	
	//shading
	
	//multi material
#ifdef MATERIAL_INDEX_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(makeSampler(s_linearSamplerFragment, _sampler), _uv, 0, 0)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)	
	//#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(_sampler, _uv, 0, 0)
	//#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DBias(_sampler, _uv, u_mipBias)
	//#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(_sampler, _uv, u_mipBias)
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
		discard;
	frameMaterialIndex = uint( u_multiMaterialCombinedMaterials[ localGroupMaterialIndex / 4 ][ localGroupMaterialIndex % 4 ] );
#endif
	vec4 materialStandardFragment[ MATERIAL_STANDARD_FRAGMENT_SIZE ];
	getMaterialData( s_materials, frameMaterialIndex, materialStandardFragment );

	//multi material
#ifdef MULTI_MATERIAL_SEPARATE_PASS
	if( materialIndex != u_materialMultiSubMaterialSeparatePassIndex )
		discard;
#endif
	
	//displacement
	vec2 displacementOffset = vec2_splat(0);
#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
	BRANCH
	if(u_displacementScale != 0.0 && voxelDataMode == 0.0)
		displacementOffset = getParallaxOcclusionMappingOffset(texCoord0, v_eyeTangentSpace, v_normalTangentSpace, u_materialDisplacementScale * u_displacementScale, u_displacementMaxSteps);
#endif

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
	bool receiveDecals = u_materialReceiveDecals != 0.0 && v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.z != 0.0;
	bool useVertexColor = u_materialUseVertexColor != 0.0;
	bool opacityDithering = false;
#ifdef OPACITY_DITHERING
	opacityDithering = true;
#endif

#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_renderOperationData[3].z, 0/*gl_PrimitiveID*/)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
//	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(_sampler, _uv, u_renderOperationData[3].z, 0/*gl_PrimitiveID*/)
//	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(_sampler, _uv, u_removeTextureTiling)
//	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(_sampler, _uv, u_mipBias)
	{
		FRAGMENT_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	//apply color parameter
	
	baseColor *= v_colorParameter.xyz;
	if(useVertexColor)
		baseColor *= color0.xyz;
	baseColor = max(baseColor, vec3_splat(0));
	
	opacity *= v_colorParameter.w;
	if(useVertexColor)
		opacity *= color0.w;
	opacity = saturate(opacity);

	//sheenColor *= v_colorParameter.xyz;
	//if(useVertexColor)
	//	sheenColor *= color0.xyz;
	//sheenColor = max(sheenColor, vec3_splat(0));

#ifdef SHADING_MODEL_SUBSURFACE
	subsurfaceColor *= v_colorParameter.xyz;
	if(useVertexColor)
		subsurfaceColor *= color0.xyz;
	subsurfaceColor = max(subsurfaceColor, vec3_splat(0));
#endif
	
	//opacity dithering	
#if !defined(GLOBAL_VOXEL_LOD) || !defined(VOXEL)
#ifdef OPACITY_DITHERING
	if(opacityDithering)
		opacity = dither(fragCoord, opacity);
#endif
#endif

	//opacity masked threshold
#if !defined(GLOBAL_VOXEL_LOD) || !defined(VOXEL)
#ifdef BLEND_MODE_MASKED
	//if(voxelDataMode == 0.0)
	clip(opacity - opacityMaskThreshold);
#endif
#endif

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

#ifdef TWO_SIDED_FLIP_NORMALS
	if(gl_FrontFacing)
		normal = -normal;
#endif

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

	gl_FragData[0] = encodeRGBE8(baseColor);
	gl_FragData[1] = vec4(normal * 0.5 + 0.5, reflectance);
	gl_FragData[2] = vec4(metallic, roughness, ambientOcclusion, rayTracingReflection);
#ifdef SHADING_MODEL_SUBSURFACE
	gl_FragData[3] = encodeRGBE8(subsurfaceColor);
#else
	gl_FragData[3] = encodeRGBE8(emissive);
#endif
	gl_FragData[4] = tangent * 0.5 + 0.5;//gl_FragData[4] = encodeGBuffer4(tangent, shadingModelSimple, receiveDecals);
	gl_FragData[5] = vec4(float(shadingModel/*SHADING_MODEL_INDEX*/) / 8.0, receiveDecals ? 1.0 : 0.0, thickness, subsurfacePower / 15.0);


	/*
#if defined(GLOBAL_MOTION_VECTOR) || defined(GLOBAL_INDIRECT_LIGHTING_FULL_MODE)
	
	//motion vector
	vec2 velocity = vec2_splat( 0 );
#ifdef GLOBAL_MOTION_VECTOR
	vec2 aa = (v_position.xy / v_position.w) * 0.5 + 0.5;
	vec2 bb = (v_previousPosition.xy / v_previousPosition.w) * 0.5 + 0.5;
	velocity = (aa - bb) * v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.w;
#endif

	//object id
	float objectId = u_renderOperationData[3].w; !!!! + gl_InstanceID

//!!!!pack objectId
	gl_FragData[6] = vec4(velocity.x,velocity.y,objectId,0);
	
#endif
*/

	//motion vector
#ifdef GLOBAL_MOTION_VECTOR
	vec2 aa = (v_position.xy / v_position.w) * 0.5 + 0.5;
	vec2 bb = (v_previousPosition.xy / v_previousPosition.w) * 0.5 + 0.5;
	vec2 velocity = (aa - bb) * v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.w;
	gl_FragData[6] = vec4(velocity.x,velocity.y,0,0);
#endif

#if (defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)) || (defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)) || defined(DEPTH_OFFSET_MODE_GREATER_EQUAL)
	float depth = getDepthValue(fragCoord.z, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
	depth += depthOffset;
	gl_GreaterEqualFragDepth = getRawDepthValue(depth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
#elif defined(DEPTH_OFFSET_MODE_LESS_EQUAL)
	float depth = getDepthValue(fragCoord.z, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
	depth += depthOffset;
	gl_LessEqualFragDepth = getRawDepthValue(depth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
#endif

}
