$input v_depth, v_texCoord01, v_color0, v_texCoord23, v_colorParameter, v_worldPosition, v_lodValueVisibilityDistanceReceiveDecals

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "UniformsFragment.sh"
#include "FragmentFunctions.sh"

uniform vec4 u_renderOperationData[5];
uniform vec4/*float*/ u_farClipDistance;
//uniform vec4/*vec2*/ u_shadowBias;

SAMPLER2D(s_materials, 1);

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
	//lod
	float lodValue = v_lodValueVisibilityDistanceReceiveDecals.x;
	smoothLOD(getFragCoord(), lodValue);

	cutVolumes(v_worldPosition);
	
	//get material data
	vec4 materialStandardFragment[MATERIAL_STANDARD_FRAGMENT_SIZE];
	getMaterialData(s_materials, u_renderOperationData, materialStandardFragment);	
	
	vec2 depth = v_depth;
	//!!!!
	//depth.x += u_shadowBias.x + u_shadowBias.y * fwidth(depth.x);

	//get material parameters
	float opacity = u_materialOpacity;
	float opacityMaskThreshold = u_materialOpacityMaskThreshold;
	
	//get material parameters (procedure generated code)
	vec2 c_texCoord0 = v_texCoord01.xy;
	vec2 c_texCoord1 = v_texCoord01.zw;
	vec2 c_texCoord2 = v_texCoord23.xy;
	//vec2 c_texCoord3 = v_texCoord23.zw;
	vec2 c_unwrappedUV = getUnwrappedUV(c_texCoord0, c_texCoord1, c_texCoord2/*, c_texCoord3*/, u_renderOperationData[3].x);
	vec4 c_color0 = v_color0;
#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(_sampler, _uv, u_removeTextureTiling)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2D(_sampler, _uv)
	FRAGMENT_CODE_BODY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	//apply color parameter
	opacity *= v_colorParameter.w;
	if(u_materialUseVertexColor != 0.0)
		opacity *= v_color0.w;
	opacity = saturate(opacity);

	float cameraDistance = length(u_viewportOwnerCameraPosition - v_worldPosition);

	//fading by visibility distance
	float visibilityDistance = v_lodValueVisibilityDistanceReceiveDecals.y;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		smoothLOD(getFragCoord(), 1.0f - visibilityDistanceFactor);
	#endif
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)	
		opacity *= visibilityDistanceFactor;
	#endif
	
//	//opacity dithering
//#ifdef OPACITY_DITHERING
//	opacity = dither(getFragCoord(), opacity);
//#endif

//opacity masked clipping
#ifdef BLEND_MODE_MASKED
	clip(opacity - opacityMaskThreshold);
#endif

//special for shadow caster
#ifdef BLEND_MODE_TRANSPARENT
	clip(opacity - 0.5);
#endif


/*
	#ifdef NVIDIA_HARDWARE_SHADOWS
		depth.x += bias.x + bias.y * fwidth(depth.x);
	#elif defined(ATI_HARDWARE_SHADOWS)
		depth.x += bias.x + bias.y * fwidth(depth.x);
	#else
		#if defined(DIRECT3D)
			depth.x += bias.x + bias.y * fwidth(depth.x);
		#else
			depth.x += bias.x;
		#endif
	#endif
*/

	//color, depth
/*
#ifdef NVIDIA_HARDWARE_SHADOWS
	oColor = float4(0, 0, 0, 1);
	oDepth = depth.x / depth.y;
#elif defined(ATI_HARDWARE_SHADOWS)
	float normalizedDepth = depth.x / farClipDistance;
	oColor = float4(normalizedDepth, 0, 0, 1);
	oDepth = normalizedDepth;
#else
*/
	float normalizedDepth = depth.x / u_farClipDistance.x;
	gl_FragColor = vec4(normalizedDepth, 0, 0, 1);
//#endif

}

