$input v_texCoord01, v_color0, v_texCoord23, v_colorParameter, v_worldPosition_depth, v_worldNormal_materialIndex, v_lodValue_visibilityDistance_receiveDecals, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, glPositionZ

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define SHADOW_CASTER 1
#include "Common.sh"
#include "UniformsFragment.sh"

uniform vec4 u_renderOperationData[8];
uniform vec4 u_materialCustomParameters[2];
uniform vec4 u_objectInstanceParameters[2];

SAMPLER2D(s_materials, 1);
#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)
	SAMPLER2D(s_voxelData, 2);
#endif
//#if defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)
//	SAMPLER2D(s_virtualizedData, 11);
//#endif
#ifndef GLSL
	SAMPLER2D(s_linearSamplerFragment, 9);
	//SAMPLER2D(s_linearSamplerFragment, 10);
#endif

#ifdef FRAGMENT_CODE_PARAMETERS
	FRAGMENT_CODE_PARAMETERS
#endif
#ifdef MATERIAL_INDEX_CODE_PARAMETERS
	MATERIAL_INDEX_CODE_PARAMETERS
#endif

#ifdef FRAGMENT_CODE_SAMPLERS
	FRAGMENT_CODE_SAMPLERS
#endif
#ifdef MATERIAL_INDEX_CODE_SAMPLERS
	MATERIAL_INDEX_CODE_SAMPLERS
#endif

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

#include "FragmentFunctions.sh"
#include "FragmentVoxel.sh"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	vec4 fragCoord = getFragCoord();

#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
	float voxelDataMode = u_renderOperationData[ 1 ].w;
#else
	const float voxelDataMode = 0.0;
#endif

	//float virtualizedDataMode = u_renderOperationData[3].w;
	
	//lod
#ifdef GLOBAL_SMOOTH_LOD
	float lodValue = v_lodValue_visibilityDistance_receiveDecals.x;
	smoothLOD(fragCoord, lodValue);
#endif

	vec2 texCoord0 = v_texCoord01.xy;
	vec2 texCoord1 = v_texCoord01.zw;
	vec2 texCoord2 = v_texCoord23.xy;
	vec2 unwrappedUV = getUnwrappedUV(texCoord0, texCoord1, texCoord2, u_renderOperationData[3].x);
	MEDIUMP vec3 inputWorldNormal = v_worldNormal_materialIndex.xyz;
	MEDIUMP vec4 tangent = vec4_splat(0);
	MEDIUMP vec4 color0 = v_color0;
	vec3 fromCameraDirection = normalize(v_worldPosition_depth.xyz - u_cameraPosition);// - u_viewportOwnerCameraPosition
	vec3 worldPosition = v_worldPosition_depth.xyz;

	int materialIndex = 0;
	float depthOffset = 0.0;
#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)
	voxelDataModeCalculateParametersF(voxelDataMode, s_voxelData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, fromCameraDirection, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex, v_colorParameter);
//#elif defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)
//	virtualizedDataModeCalculateParametersF(virtualizedDataMode, s_virtualizedData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, gl_PrimitiveID, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex);
#else
	materialIndex = int(round(v_worldNormal_materialIndex.w));
#endif
	worldPosition += fromCameraDirection * depthOffset;
	
	float ownerCameraDistance = length(worldPosition - u_viewportOwnerCameraPosition);
	float cameraDistance = length(worldPosition - u_cameraPosition);
	vec3 cameraPosition = u_cameraPosition;

	//start distance
#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT)
	if(cameraDistance < u_startDistance)
		discard;
#endif

/*!!!!new
	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals.y * u_shadowObjectVisibilityDistanceFactor;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, ownerCameraDistance);
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
	#endif
#endif
*/
	
	if( cutVolumes( worldPosition ) )
		discard;

	inputWorldNormal = normalize(inputWorldNormal);

	//end of geometry stage
		
	//shading

	//multi material
#ifdef MATERIAL_INDEX_CODE_BODY
	#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
	#else
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
	#endif
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
	
	//vec2 depth = v_depth;
	////depth.x += u_shadowBias.x + u_shadowBias.y * fwidth(depth.x);

	//get material parameters
	MEDIUMP float opacity = u_materialOpacity;
	MEDIUMP float opacityMaskThreshold = u_materialOpacityMaskThreshold;
	//MEDIUMP float rayTracingReflection = u_materialRayTracingReflection;
	
	int shadingModel = 0;
	bool receiveDecals = false;
	bool useVertexColor = u_materialUseVertexColor != 0.0;
	bool opacityDithering = false;
	
	vec2 texCoord0BeforeDisplacement = texCoord0;
	vec2 texCoord1BeforeDisplacement = texCoord1;
	vec2 texCoord2BeforeDisplacement = texCoord2;
	vec2 unwrappedUVBeforeDisplacement = unwrappedUV;
	//vec2 texCoord0 = geometryTexCoord0;// - displacementOffset;
	//vec2 texCoord1 = geometryTexCoord1;// - displacementOffset;
	//vec2 texCoord2 = geometryTexCoord2;// - displacementOffset;
	//vec2 unwrappedUV = getUnwrappedUV(texCoord0, texCoord1, texCoord2, u_renderOperationData[3].x);
	vec4 customParameter1 = u_materialCustomParameters[0];
	vec4 customParameter2 = u_materialCustomParameters[1];
	vec4 instanceParameter1 = u_objectInstanceParameters[0];
	vec4 instanceParameter2 = u_objectInstanceParameters[1];
	
#ifdef FRAGMENT_CODE_BODY
	#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
	#else
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
	#endif
	{
		FRAGMENT_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	opacityMaskThreshold *= u_shadowMaterialOpacityMaskThresholdFactor;

	//apply color parameter	
	opacity *= v_colorParameter.w;
	if(useVertexColor)
		opacity *= color0.w;
	opacity = saturate(opacity);
/*!!!!new
	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)	
		opacity *= visibilityDistanceFactor;
	#endif
#endif
*/


//!!!!	
//	//opacity dithering
//#ifdef OPACITY_DITHERING
//	opacity = dither(fragCoord, opacity);
//#endif

//opacity masked clipping
#ifdef BLEND_MODE_MASKED
	if(voxelDataMode == 0.0)
	{
		//!!!!apply when OpacityDithering is enabled in material?
		
		clip(opacity - opacityMaskThreshold);
		//clip(dither(fragCoord, opacity) - opacityMaskThreshold);
	}
#endif

//special for shadow caster
#ifdef BLEND_MODE_TRANSPARENT
	if(voxelDataMode == 0.0)
	{
		clip(opacity - 0.5);
		
		//!!!!
		
		
		/*
		vec3 viewPosition = mul(u_viewportOwnerView, vec4(worldPosition, 1)).xyz;
		
		mat4 proj = u_viewportOwnerProj;
		//!!!!
		//float del = proj[3][0] * viewPosition.x + proj[3][1] * viewPosition.y + proj[3][2] * viewPosition.z + proj[3][3];
		float del = proj[0][3] * viewPosition.x + proj[1][3] * viewPosition.y + proj[2][3] * viewPosition.z + proj[3][3];
		if( del == 0 )
			del = 0.000001;
		float invW = 1.0 / del;
		vec4 screenSpacePos = mul(proj, vec4(viewPosition, 1) ) * invW;
		
		//MultiplyProjectWTo1( ref projectionMatrix, ref eyeSpacePos, out var screenSpacePos );
		
		vec2 screenPosition = vec2( ( screenSpacePos.x + 1.0 ) * 0.5, 1.0 - ( screenSpacePos.y + 1.0 ) * 0.5 );

		vec2 qq = screenPosition.xy * vec2(1920 * 1.4,1080 * 1.4);

		vec4 fragCoordOwner = vec4(qq.x, qq.y, 0, 0);
		clip(dither(fragCoordOwner, opacity) - 0.5);
		*/
		
		
		/*
		vec2 screenPosition = projectToScreenCoordinates(u_viewportOwnerViewProjection, worldPosition);
		
		//vec4 screenPosition = mul(u_viewportOwnerViewProjection, vec4(worldPosition, 1));
		//screenPosition.xy /= screenPosition.w;
		
		//vec2 bb = (v_previousPosition.xy / v_previousPosition.w) * 0.5 + 0.5;

		
		//!!!-1, 1
		
		//!!!!
		vec2 qq = screenPosition.xy * vec2(1920 * 1.41,1080 * 1.41);
		//vec2 qq = screenPosition.xz * vec2(6000,4000);
		
		vec4 fragCoordOwner = vec4(qq.x, qq.y, 0, 0);
		//vec4 fragCoordOwner = vec4(qq.x, qq.y, 0, 0);
		
		//if(!getDitherBoolean(fragCoordOwner, opacity))
		//	discard;
		clip(dither(fragCoordOwner, opacity) - 0.5);
		*/

		
		//clip(dither(fragCoord, opacity) - 0.5);	
		
		
		
		
		/*
	#ifdef LIGHT_TYPE_DIRECTIONAL
	
		clip(dither(fragCoord, opacity) - 0.5);	
		//only for directional
		//vec4 fragCoordEmulation = vec4((worldPosition.x + worldPosition.z) * 10.0, (worldPosition.y + worldPosition.z) * 10.0, 0,0);
		//clip(dither(fragCoordEmulation, opacity) - 0.5);
	
	#else
		clip(dither(fragCoord, opacity) - 0.5);	
	#endif
		*/
		
		//clip(opacity - 0.5);
	}
#endif

/*#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM
	vec2 rg = shadowCasterEVSM(depth, u_farClipDistance);
	gl_FragColor = vec4(rg.x, rg.y, 0, 1);
#else */


	float newFragCoordZ = fragCoord.z;

//!!!!GLSL
#ifndef GLSL
#if (defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)) || (defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)) || defined(DEPTH_OFFSET_MODE_GREATER_EQUAL)
	#ifdef LIGHT_TYPE_DIRECTIONAL
		float depth2 = fragCoord.z * u_farClipDistance;
		//!!!!
		depth2 += depthOffset;
		gl_GreaterEqualFragDepth = newFragCoordZ = depth2 / u_farClipDistance;
	#else
		float depth2 = getDepthValue(fragCoord.z, u_nearClipDistance, u_farClipDistance);
		//!!!!
		depth2 += depthOffset;
		gl_GreaterEqualFragDepth = newFragCoordZ = getRawDepthValue(depth2, u_nearClipDistance, u_farClipDistance);
	#endif
#elif defined(DEPTH_OFFSET_MODE_LESS_EQUAL)
	#ifdef LIGHT_TYPE_DIRECTIONAL
		float depth2 = fragCoord.z * u_farClipDistance;
		//!!!!
		depth2 += depthOffset;
		gl_LessEqualFragDepth = newFragCoordZ = depth2 / u_farClipDistance;
	#else
		float depth2 = getDepthValue(fragCoord.z, u_nearClipDistance, u_farClipDistance);
		//!!!!
		depth2 += depthOffset;
		gl_LessEqualFragDepth = newFragCoordZ = getRawDepthValue(depth2, u_nearClipDistance, u_farClipDistance);
	#endif
#endif
#endif


	float depth;
#ifdef LIGHT_TYPE_POINT
	depth = length(worldPosition - u_cameraPosition);
#elif defined( LIGHT_TYPE_SPOTLIGHT )
	depth = getDepthValue(newFragCoordZ, u_nearClipDistance, u_farClipDistance);	
#else
	depth = newFragCoordZ;
#endif

	//!!!!special for mobile
#ifdef GLSL
	#ifndef LIGHT_TYPE_POINT
		depth = glPositionZ;
	#endif
#endif


	float normalizedDepth = depth / u_farClipDistance;
		
#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
	gl_FragColor = packFloatToRgba(normalizedDepth);
#else
	gl_FragColor = vec4(normalizedDepth, 0, 0, 1);
#endif

//#endif

}
