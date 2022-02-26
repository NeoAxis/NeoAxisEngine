$input v_depth, v_texCoord01, v_color0, v_texCoord23, v_colorParameter, v_worldPosition_depth, v_worldNormal, v_lodValue_visibilityDistance_receiveDecals, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles, v_billboardRotation

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define SHADOW_CASTER 1
#include "Common.sh"
#include "UniformsFragment.sh"
#include "FragmentFunctions.sh"
//#include "Sha_dowCasterFunctions.sh"

uniform vec4 u_renderOperationData[5];
uniform vec4 u_materialCustomParameters[2];
uniform vec4/*float*/ u_farClipDistance;
uniform vec4/*float*/ u_shadowMaterialOpacityMaskThresholdFactor;
//uniform vec4/*vec2*/ u_shadowBias;

SAMPLER2D(s_materials, 1);
#ifdef BILLBOARD	
#ifdef GLOBAL_BILLBOARD_DATA
	SAMPLER2DARRAY(s_billboardData, 2);
#endif
#endif

#ifdef FRAGMENT_CODE_PARAMETERS
	FRAGMENT_CODE_PARAMETERS
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
	vec4 fragCoord = getFragCoord();

	//get material parameters (procedure generated code)
	vec2 texCoord0 = v_texCoord01.xy;
	vec2 texCoord1 = v_texCoord01.zw;
	vec2 texCoord2 = v_texCoord23.xy;
	//vec2 texCoord3 = v_texCoord23.zw;

	//billboard with geometry data mode
	float billboardDataMode = u_renderOperationData[1].w;
	vec3 inputWorldNormal = vec3_splat(0);
#ifdef BILLBOARD	
#ifdef GLOBAL_BILLBOARD_DATA
	billboardDataModeCalculateParameters(billboardDataMode, s_billboardData, fragCoord, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles, v_billboardRotation, inputWorldNormal, texCoord0, texCoord1, texCoord2);
#endif
#endif	
	
	//lod
#ifdef GLOBAL_SMOOTH_LOD
	float lodValue = v_lodValue_visibilityDistance_receiveDecals.x;
	smoothLOD(fragCoord, lodValue);
#endif

	cutVolumes(v_worldPosition_depth.xyz);
	
	//get material data
	vec4 materialStandardFragment[MATERIAL_STANDARD_FRAGMENT_SIZE];
	getMaterialData(s_materials, u_renderOperationData, materialStandardFragment);	
	
	vec2 depth = v_depth;
	//depth.x += u_shadowBias.x + u_shadowBias.y * fwidth(depth.x);

	//get material parameters
	float opacity = u_materialOpacity;
	float opacityMaskThreshold = u_materialOpacityMaskThreshold;
	opacityMaskThreshold *= u_shadowMaterialOpacityMaskThresholdFactor.x;
	
	vec2 unwrappedUV = getUnwrappedUV(texCoord0, texCoord1, texCoord2/*, texCoord3*/, u_renderOperationData[3].x);
	vec4 color0 = v_color0;
	vec4 customParameter1 = u_materialCustomParameters[0];
	vec4 customParameter2 = u_materialCustomParameters[1];
#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(_sampler, _uv, u_renderOperationData[3].z, 0/*gl_PrimitiveID*/)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2D(_sampler, _uv)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2D(_sampler, _uv)
	FRAGMENT_CODE_BODY
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	//apply color parameter
	opacity *= v_colorParameter.w;
	if(u_materialUseVertexColor != 0.0)
		opacity *= color0.w;
	opacity = saturate(opacity);

	float cameraDistance = length(u_viewportOwnerCameraPosition - v_worldPosition_depth.xyz);

	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals.y * u_shadowObjectVisibilityDistanceFactor;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
	#endif
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)	
		opacity *= visibilityDistanceFactor;
	#endif
#endif
	
//	//opacity dithering
//#ifdef OPACITY_DITHERING
//	opacity = dither(fragCoord, opacity);
//#endif

//opacity masked clipping
#ifdef BLEND_MODE_MASKED
	if(billboardDataMode == 0.0)
		clip(opacity - opacityMaskThreshold);
#endif

//special for shadow caster
#ifdef BLEND_MODE_TRANSPARENT
	if(billboardDataMode == 0.0)
		clip(opacity - 0.5);
#endif

/*#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM
	vec2 rg = shadowCasterEVSM(depth, u_farClipDistance.x);
	gl_FragColor = vec4(rg.x, rg.y, 0, 1);
#else */

	float normalizedDepth = depth.x / u_farClipDistance.x;	
#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
	gl_FragColor = packFloatToRgba(normalizedDepth);
#else
	gl_FragColor = vec4(normalizedDepth, 0, 0, 1);
#endif

//#endif
}
