$input v_worldPosition, v_lodValue_visibilityDistance_receiveDecals, v_texCoord0, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, glPositionZ, v_colorParameter

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define SHADOW_CASTER 1
#define SHADOW_CASTER_DEFAULT 1
#include "Common.sh"

uniform vec4 u_renderOperationData[8];

#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)
	SAMPLER2D(s_voxelData, 2);
#endif
//#if defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)
//	SAMPLER2D(s_virtualizedData, 11);
//#endif

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

	vec2 texCoord0 = v_texCoord0;
	vec2 texCoord1 = vec2_splat(0);
	vec2 texCoord2 = vec2_splat(0);
	MEDIUMP vec3 inputWorldNormal = vec3_splat(0);
	MEDIUMP vec4 tangent = vec4_splat(0);
	MEDIUMP vec4 color0 = vec4_splat(0);
	vec3 fromCameraDirection = normalize(v_worldPosition - u_cameraPosition);// - u_viewportOwnerCameraPosition
	vec3 worldPosition = v_worldPosition;

	int materialIndex = 0;
	float depthOffset = 0.0;
#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)
	voxelDataModeCalculateParametersF(voxelDataMode, s_voxelData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, fromCameraDirection, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex, v_colorParameter);
//#elif defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)
//	virtualizedDataModeCalculateParametersF(virtualizedDataMode, s_virtualizedData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, gl_PrimitiveID, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex);
#endif
	worldPosition += fromCameraDirection * depthOffset;

	float ownerCameraDistance = length(worldPosition - u_viewportOwnerCameraPosition);
	float cameraDistance = length(worldPosition - u_cameraPosition);

	//start distance
#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT)
	if(cameraDistance < u_startDistance)
		discard;
#endif
	
	//fading by visibility
/*!!!!new	
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals.y * u_shadowObjectVisibilityDistanceFactor;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, ownerCameraDistance);
	smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
#endif
*/

	if( cutVolumes( worldPosition ) )
		discard;
	
	//end of geometry stage

	//shading

	
	//vec2 depth = v_depth;
	////depth.x += u_shadowBias.x + u_shadowBias.y * fwidth(depth.x);

/*#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM
	vec2 rg = shadowCasterEVSM(depth, u_farClipDistance);
	gl_FragColor = vec4(rg.x, rg.y, 0, 1);
#else*/


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
