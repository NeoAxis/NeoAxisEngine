$input v_texCoord01, v_worldPosition_depth, v_worldNormal_materialIndex, v_tangent, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_texCoord23, v_colorParameter, v_lodValue_visibilityDistance_receiveDecals

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define DEFERRED_DECAL 1
#include "Common.sh"
#include "UniformsFragment.sh"

uniform vec4 u_renderOperationData[8];
uniform vec4 u_materialCustomParameters[2];
uniform vec4 u_objectInstanceParameters[2];
SAMPLER2D(s_materials, 1);
/*
#if defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)
	SAMPLER2D(s_virtualizedData, 11);
#endif
*/
SAMPLER2D(s_linearSamplerFragment, 9);
//SAMPLER2D(s_linearSamplerFragment, 10);

uniform mat4 u_decalMatrix;
uniform vec4 u_decalNormalTangent[2];

SAMPLER2D(s_depthTexture, 3);
SAMPLER2D(s_gBuffer1TextureCopy, 4);
SAMPLER2D(s_gBuffer4TextureCopy, 5);
SAMPLER2D(s_gBuffer5TextureCopy, 6);

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

#ifdef MULTI_MATERIAL_COMBINED_PASS
	uniform vec4 u_multiMaterialCombinedInfo;
	uniform vec4 u_multiMaterialCombinedMaterials[4];
#endif

#include "FragmentFunctions.sh"
#include "FragmentVoxel.sh"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	vec4 fragCoord = getFragCoord();
	
	vec2 screenPosition = fragCoord.xy * u_viewTexel.xy;
	float rawDepth = texture2D(s_depthTexture, screenPosition).r;
	vec3 worldPosition = reconstructWorldPosition(u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, screenPosition, rawDepth);
	//vec3 worldPosition = reconstructWorldPosition(u_invViewProj, screenPosition, rawDepth);

	if( cutVolumes( worldPosition ) )
		discard;

	float cameraDistance = length(worldPosition - u_viewportOwnerCameraPosition);
	vec3 cameraPosition = u_viewportOwnerCameraPosition;

	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals.y;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
	smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
#endif

	vec2 decalTexCoord;
	{
		vec4 objectPosition = mul(u_decalMatrix, vec4(worldPosition, 1));
		
		decalTexCoord = vec2(0.5 - objectPosition.y, objectPosition.x + 0.5);
		//decalTexCoord = objectPosition.xy + 0.5;
		
		//reject anything outside.
		clip(0.5 - abs(objectPosition.xyz));
	}
	
	vec3 sourceNormal;
	vec4 sourceTangent;
	
	//get source normal
	{
		vec4 normal_and_reflectance = texture2D(s_gBuffer1TextureCopy, screenPosition);
		sourceNormal = normalize(normal_and_reflectance.rgb * 2 - 1);
	}
	
	//get source tangent
	vec4 gBuffer4DataCopy = texture2D(s_gBuffer4TextureCopy, screenPosition);
	sourceTangent = gBuffer4DataCopy * 2.0 - 1.0;
	sourceTangent.xyz = normalize(sourceTangent.xyz);
	
	//get receive decals, discard
	vec4 gBuffer5DataCopy = texture2D(s_gBuffer5TextureCopy, screenPosition);
	bool receiveDecals2 = gBuffer5DataCopy.y == 1.0;
	if(!receiveDecals2)
		discard;

	//NormalsMode.VectorOfDecal
	if(any(u_decalNormalTangent[0]))
	{
		sourceNormal = u_decalNormalTangent[0].xyz;
		sourceTangent = u_decalNormalTangent[1];
	}
	
	vec2 texCoord0 = decalTexCoord;
	vec2 texCoord1 = decalTexCoord;
	vec2 texCoord2 = decalTexCoord;
	vec2 unwrappedUV = decalTexCoord;
	vec3 inputWorldNormal = normalize(v_worldNormal_materialIndex.xyz);
	vec4 color0 = v_color0;
	int materialIndex = int(round(v_worldNormal_materialIndex.w));
	float depthOffset = 0.0;

	//end of geometry stage
	
	//shading

	//multi material
#ifdef MATERIAL_INDEX_CODE_BODY
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling, u_mipBias)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
	{
		MATERIAL_INDEX_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	//get material data
	int frameMaterialIndex = int( u_renderOperationData[ 0 ].x );
#ifdef MULTI_MATERIAL_COMBINED_PASS
	uint localGroupMaterialIndex = uint( materialIndex ) - uint( u_multiMaterialCombinedInfo.x );
	BRANCH
	if( localGroupMaterialIndex < 0 || localGroupMaterialIndex >= uint( u_multiMaterialCombinedInfo.y ) )
		discard;
	frameMaterialIndex = int( u_multiMaterialCombinedMaterials[ localGroupMaterialIndex / 4 ][ localGroupMaterialIndex % 4 ] );
#endif
	//vec4 materialStandardFragment[ MATERIAL_STANDARD_FRAGMENT_SIZE ];
	//getMaterialData( s_materials, frameMaterialIndex, materialStandardFragment );

	//multi material
#ifdef MULTI_MATERIAL_SEPARATE_PASS
	if( materialIndex != u_materialMultiSubMaterialSeparatePassIndex )
		discard;
#endif
	
	/*
	//get source tangent, check receive decals flag
	{
		//vec4 tangentDummy;
		bool shadingModelSimpleDummy;
		bool receiveDecals;
		vec4 gBuffer4DataCopy = texture2D(s_gBuffer4TextureCopy, screenPosition);
		decodeGBuffer4(gBuffer4DataCopy, sourceTangent, shadingModelSimpleDummy, receiveDecals);
		if(!receiveDecals)
			discard;
		sourceTangent.xyz = normalize(sourceTangent.xyz);
	}
	*/
	
	//displacement
	vec2 displacementOffset = vec2_splat(0);
#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
	BRANCH
	if(u_displacementScale != 0.0)
		displacementOffset = getParallaxOcclusionMappingOffset(decalTexCoord/*v_texCoord01.xy*/, v_eyeTangentSpace, v_normalTangentSpace, u_materialDisplacementScale * u_displacementScale, u_displacementMaxSteps);
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
	vec3 anisotropyDirection = u_materialAnisotropyDirection;
	float thickness = u_materialThickness;
	float subsurfacePower = u_materialSubsurfacePower;
	vec3 sheenColor = u_materialSheenColor;
	vec3 subsurfaceColor = u_materialSubsurfaceColor;
	float ambientOcclusion = 1;
	//float rayTracingReflection = u_materialRayTracingReflection;
	vec3 emissive = u_materialEmissive;
	vec4 customParameter1 = u_materialCustomParameters[0];
	vec4 customParameter2 = u_materialCustomParameters[1];
	vec4 instanceParameter1 = u_objectInstanceParameters[0];
	vec4 instanceParameter2 = u_objectInstanceParameters[1];

	vec2 texCoord0BeforeDisplacement = texCoord0;
	vec2 texCoord1BeforeDisplacement = texCoord1;
	vec2 texCoord2BeforeDisplacement = texCoord2;
	vec2 unwrappedUVBeforeDisplacement = unwrappedUV;
	texCoord0 -= displacementOffset;
	texCoord1 -= displacementOffset;
	texCoord2 -= displacementOffset;
	unwrappedUV -= displacementOffset;

	int shadingModel = SHADING_MODEL_INDEX;
	bool receiveDecals = false;
	bool useVertexColor = u_materialUseVertexColor != 0.0;
	bool opacityDithering = false;
#ifdef OPACITY_DITHERING
	opacityDithering = true;
#endif
	
#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling, u_mipBias)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
	{
		FRAGMENT_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

#ifdef OPACITY_CODE_BODY
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling, u_mipBias)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
	{
		OPACITY_CODE_BODY
	}
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

	sheenColor *= v_colorParameter.xyz;
	if(useVertexColor)
		sheenColor *= color0.xyz;
	sheenColor = max(sheenColor, vec3_splat(0));
	
	subsurfaceColor *= v_colorParameter.xyz;
	if(useVertexColor)
		subsurfaceColor *= color0.xyz;
	subsurfaceColor = max(subsurfaceColor, vec3_splat(0));	
	
	//opacity dithering
#ifdef OPACITY_DITHERING
	if(opacityDithering)
		opacity = dither(fragCoord, opacity);
#endif

	//opacity masked threshold
#ifdef BLEND_MODE_MASKED
	clip(opacity - opacityMaskThreshold);
#endif

	//get result tangent
	vec4 tangent;
	tangent.xyz = normalize(sourceTangent.xyz);
	tangent.w = sourceTangent.w;
	
	//get result world normal
#if defined(GLOBAL_NORMAL_MAPPING) && GLOBAL_MATERIAL_SHADING != GLOBAL_MATERIAL_SHADING_SIMPLE
	BRANCH
	if(any2(normal))
	{
		vec3 bitangent = cross(sourceTangent.xyz, sourceNormal) * sourceTangent.w;		
		mat3 tbn = transpose(mtxFromRows(tangent.xyz, bitangent, sourceNormal));
		normal = expand(normal);
		normal.z = sqrt(max(1.0 - dot(normal.xy, normal.xy), 0.0));
		normal = normalize(mul(tbn, normal));
	}
	else
		normal = sourceNormal;
#else
	normal = sourceNormal;
#endif


#if !defined(SHADING_MODEL_SUBSURFACE) && !defined(SHADING_MODEL_FOLIAGE) //#ifndef SHADING_MODEL_SUBSURFACE
	thickness = 0;
	subsurfacePower = 0;
	subsurfaceColor = vec3_splat(0);
#endif

#ifdef MULTI_MATERIAL_COMBINED_PASS
	if(shadingModel != 1 && shadingModel != 2)//Subsurface, Foliage
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
	//rayTracingReflection = 0;
#endif
#ifdef MULTI_MATERIAL_COMBINED_PASS
	if(shadingModel == 4)//3)//Simple
	{
		reflectance = 0;
		metallic = 0;
		roughness = 0;
		ambientOcclusion = 0;
		//rayTracingReflection = 0;
	}
#endif

	receiveDecals = false;	
	
	gl_FragData[0] = encodeRGBE8(baseColor);
	gl_FragData[1] = vec4(normal * 0.5 + 0.5, reflectance);
	gl_FragData[2] = vec4(metallic, roughness, ambientOcclusion, 0/*rayTracingReflection*/);
#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
	gl_FragData[3] = encodeRGBE8(subsurfaceColor);
#else
	gl_FragData[3] = encodeRGBE8(emissive);
#endif
	gl_FragData[4] = tangent * 0.5 + 0.5;//gl_FragData[4] = encodeGBuffer4(tangent, shadingModelSimple, false/*receiveDecals*/);
	gl_FragData[5] = vec4(float(SHADING_MODEL_INDEX) / 8.0, receiveDecals ? 1.0 : 0.0, thickness, subsurfacePower / 15.0);

	vec2 velocity = vec2_splat( 0 );
	gl_FragData[6] = vec4(velocity.x,velocity.y,0,0);
	
/*
	//!!!!check
#if defined(GLOBAL_MOTION_VECTOR) || defined(GLOBAL_INDIRECT_LIGHTING_FULL_MODE)

	//motion vector
	vec2 velocity = vec2_splat( 0 );
#ifdef GLOBAL_MOTION_VECTOR

	//!!!!copy from copy texture
	
#endif
	
	//object id
	float objectId = u_renderOperationData[3].w; !!!! + gl_InstanceID 

	//!!!!pack objectId
	gl_FragData[6] = vec4(velocity.x,velocity.y,objectId,0);

#endif
*/
	
}


	/*vec3 normal;
	{
		mat3 tbn = transpose(mtxFromRows(tangent.xyz, bitangent, sourceNormal));
		normal = normalize(mul(tbn, normal));
	}*/
	
/*
	//get result tangent
	vec4 tangent;
	tangent.xyz = normalize(v_tangent.xyz);
	tangent.w = v_tangent.w;
	
	//get result world normal
	vec3 bitangent = normalize(v_bitangent);
	if(any(normal))
	{
		mat3 tbn = transpose(mtxFromRows(tangent.xyz, bitangent, inputWorldNormal));
		normal = expand(normal);
		normal.z = sqrt(max(1.0 - dot(normal.xy, normal.xy), 0.0));
		normal = normalize(mul(tbn, normal));
	}
	else
		normal = inputWorldNormal;
	#ifdXXef TWO_SIDED
		if(gl_FrontFacing)
			normal = -normal;
	#endif
*/

/*	
	{
		vec3 pixelTangent = tangent.xyz;
		vec3 pixelBinormal = bitangent;
		vec3 pixelNormal = normal;
		
		//Get values across and along the surface
		vec3 ddxWp = ddx(worldPosition);
		vec3 ddyWp = ddy(worldPosition);

		//Determine the normal
		vec3 normal2 = normalize(cross(ddyWp, ddxWp));

		//Normalizing things is cool
		bitangent = normalize(ddxWp);
		//!!!!
		tangent.xyz = normalize(ddyWp);
		//!!!!need?
		//tangent.w = xx

		//Create a matrix transforming from tangent space to view space
		float3x3 tangentToView;
		//tangentToView[0] = mul(pixelTangent, transpose(u_view));
		//tangentToView[1] = mul(pixelBinormal, transpose(u_view));
		//tangentToView[2] = mul(pixelNormal, transpose(u_view));
		tangentToView[0] = mul(pixelTangent, u_view);
		tangentToView[1] = mul(pixelBinormal, u_view);
		tangentToView[2] = mul(pixelNormal, u_view);

		//Transform normal from tangent space into view space
		normal = mul(normal2, tangentToView);
	}
	*/
