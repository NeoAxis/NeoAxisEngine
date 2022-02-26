$input v_texCoord01, v_worldPosition_depth, v_worldNormal, v_tangent, v_bitangent, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_texCoord23, v_colorParameter, v_lodValue_visibilityDistance_receiveDecals

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define DEFERRED_DECAL 1
#include "Common.sh"
#include "UniformsFragment.sh"
#include "FragmentFunctions.sh"

uniform vec4 u_renderOperationData[5];
uniform vec4 u_materialCustomParameters[2];
SAMPLER2D(s_materials, 1);

uniform mat4 u_decalMatrix;
uniform vec4 u_decalNormalTangent[2];

SAMPLER2D(s_depthTexture, 3);
SAMPLER2D(s_gBuffer1TextureCopy, 4);
SAMPLER2D(s_gBuffer4TextureCopy, 5);
SAMPLER2D(s_gBuffer5TextureCopy, 6);

#ifdef DISPLACEMENT_CODE_PARAMETERS
	DISPLACEMENT_CODE_PARAMETERS
#endif
#ifdef FRAGMENT_CODE_PARAMETERS
	FRAGMENT_CODE_PARAMETERS
#endif

#ifdef DISPLACEMENT_CODE_SAMPLERS
	DISPLACEMENT_CODE_SAMPLERS
#endif
#ifdef DISPLACEMENT_CODE_SHADER_SCRIPTS
	DISPLACEMENT_CODE_SHADER_SCRIPTS
#endif
#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
	#include "ParallaxOcclusionMapping.sh"
#endif

#ifdef FRAGMENT_CODE_SAMPLERS
	FRAGMENT_CODE_SAMPLERS
#endif
#ifdef FRAGMENT_CODE_SHADER_SCRIPTS
	FRAGMENT_CODE_SHADER_SCRIPTS
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	//get material data
	vec4 materialStandardFragment[MATERIAL_STANDARD_FRAGMENT_SIZE];
	getMaterialData(s_materials, u_renderOperationData, materialStandardFragment);	
	
	//vec3 worldPosition = v_worldPosition_depth.xyz;
	vec3 inputWorldNormal = normalize(v_worldNormal);

	//decal specified code
	
	vec2 screenPosition = getFragCoord().xy * u_viewTexel.xy;
	float rawDepth = texture2D(s_depthTexture, screenPosition).r;
	vec3 worldPosition = reconstructWorldPosition(u_invViewProj, screenPosition, rawDepth);

	cutVolumes(worldPosition);

	vec3 toCamera = u_viewportOwnerCameraPosition - worldPosition.xyz;
	float cameraDistance = length(toCamera);
	toCamera = normalize(toCamera);
	
	vec2 decalTexCoord;
	{
		vec4 objectPosition = mul(u_decalMatrix, vec4(worldPosition, 1));
		
		decalTexCoord = vec2(0.5 - objectPosition.y, objectPosition.x + 0.5);
		//decalTexCoord = objectPosition.xy + 0.5;
		
		//  Reject anything outside.
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
	
	//NormalsMode.VectorOfDecal
	if(any(u_decalNormalTangent[0]))
	{
		sourceNormal = u_decalNormalTangent[0].xyz;
		sourceTangent = u_decalNormalTangent[1];
	}

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
	float rayTracingReflection = u_materialRayTracingReflection;
	vec3 emissive = u_materialEmissive;
	vec4 customParameter1 = u_materialCustomParameters[0];
	vec4 customParameter2 = u_materialCustomParameters[1];
	
	//get material parameters (procedure generated code)
	vec2 texCoord0 = decalTexCoord/*v_texCoord01.xy*/ - displacementOffset;
	vec2 texCoord1 = decalTexCoord/*v_texCoord01.zw*/ - displacementOffset;
	vec2 texCoord2 = decalTexCoord/*v_texCoord23.xy*/ - displacementOffset;
	//vec2 texCoord3 = decalTexCoord/*v_texCoord23.zw*/ - displacementOffset;
	vec2 unwrappedUV = getUnwrappedUV(texCoord0, texCoord1, texCoord2/*, texCoord3*/, u_renderOperationData[3].x);	
	vec4 color0 = v_color0;
#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(_sampler, _uv, u_renderOperationData[3].z, 0/*gl_PrimitiveID*/)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(_sampler, _uv, u_removeTextureTiling)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(_sampler, _uv, u_mipBias)
	FRAGMENT_CODE_BODY
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	//apply color parameter
	
	baseColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0.0)
		baseColor *= color0.xyz;
	baseColor = max(baseColor, vec3_splat(0));
	
	opacity *= v_colorParameter.w;
	if(u_materialUseVertexColor != 0.0)
		opacity *= color0.w;
	opacity = saturate(opacity);

	sheenColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0.0)
		sheenColor *= color0.xyz;
	sheenColor = max(sheenColor, vec3_splat(0));
	
	subsurfaceColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0.0)
		subsurfaceColor *= color0.xyz;
	subsurfaceColor = max(subsurfaceColor, vec3_splat(0));	
	
	//opacity dithering
#ifdef OPACITY_DITHERING
	opacity = dither(getFragCoord(), opacity);
#endif

	//opacity masked threshold
#ifdef BLEND_MODE_MASKED
	clip(opacity - opacityMaskThreshold);
#endif

	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals.y;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
	smoothLOD(getFragCoord(), 1.0f - visibilityDistanceFactor);
#endif

	//get result tangent
	vec4 tangent;
	tangent.xyz = normalize(sourceTangent.xyz);
	tangent.w = sourceTangent.w;

	//get result world normal
#if defined(GLOBAL_NORMAL_MAPPING) && GLOBAL_MATERIAL_SHADING != GLOBAL_MATERIAL_SHADING_SIMPLE
	//vec3 bitangent = normalize(v_bitangent);
	if(any(normal))
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
		if(gl_FrontFacing)//if(!gl_FrontFacing)
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

	bool receiveDecals = false;// = u_materialReceiveDecals != 0 && u_renderOperationData[1].x != 0;
	//bool shadingModelSimple = false;
	
#ifndef SHADING_MODEL_SUBSURFACE
	thickness = 0;
	subsurfacePower = 0;
#endif
#ifdef SHADING_MODEL_SIMPLE
	//shadingModelSimple = true;
	reflectance = 0;
	metallic = 0;
	roughness = 0;
	ambientOcclusion = 0;
	rayTracingReflection = 0;
#endif

	gl_FragData[0] = encodeRGBE8(baseColor);
	gl_FragData[1] = vec4(normal * 0.5 + 0.5, reflectance);
	gl_FragData[2] = vec4(metallic, roughness, ambientOcclusion, rayTracingReflection);
#ifdef SHADING_MODEL_SUBSURFACE
	gl_FragData[3] = encodeRGBE8(subsurfaceColor);
#else
	gl_FragData[3] = encodeRGBE8(emissive);
#endif
	gl_FragData[4] = tangent * 0.5 + 0.5;//gl_FragData[4] = encodeGBuffer4(tangent, shadingModelSimple, false/*receiveDecals*/);
	gl_FragData[5] = vec4(float(SHADING_MODEL_INDEX) / 8.0, receiveDecals ? 1.0 : 0.0, thickness, subsurfacePower / 15.0);
}
