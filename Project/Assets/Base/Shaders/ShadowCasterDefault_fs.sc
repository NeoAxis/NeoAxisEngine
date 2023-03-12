$input v_worldPosition, v_lodValue_visibilityDistance_receiveDecals, v_texCoord0, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, glPositionZ

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define SHADOW_CASTER 1
#include "Common.sh"
#include "FragmentFunctions.sh"

uniform vec4 u_renderOperationData[7];
uniform vec4/*float*/ u_farClipDistance;
uniform vec4/*float*/ u_nearClipDistance;
uniform vec4/*vec3*/ u_cameraPosition;

#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)
	SAMPLER2D(s_voxelData, 2);
#endif
#if defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)
	SAMPLER2D(s_virtualizedData, 11);
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	vec4 fragCoord = getFragCoord();
	float voxelDataMode = u_renderOperationData[1].w;
	float virtualizedDataMode = u_renderOperationData[3].w;

	//lod
#ifdef GLOBAL_SMOOTH_LOD
	float lodValue = v_lodValue_visibilityDistance_receiveDecals.x;
	smoothLOD(fragCoord, lodValue);
#endif

	MEDIUMP vec2 texCoord0 = v_texCoord0;
	MEDIUMP vec2 texCoord1 = vec2_splat(0);
	MEDIUMP vec2 texCoord2 = vec2_splat(0);
	MEDIUMP vec3 inputWorldNormal = vec3_splat(0);
	MEDIUMP vec4 tangent = vec4_splat(0);
	MEDIUMP vec4 color0 = vec4_splat(0);
	vec3 fromCameraDirection = normalize(v_worldPosition - u_cameraPosition.xyz);// - u_viewportOwnerCameraPosition
	vec3 worldPosition = v_worldPosition;

	int materialIndex = 0;
	float depthOffset = 0.0;
#if defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)
	voxelDataModeCalculateParametersF(voxelDataMode, s_voxelData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, fromCameraDirection, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex);
#elif defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)
	virtualizedDataModeCalculateParametersF(virtualizedDataMode, s_virtualizedData, fragCoord, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, u_renderOperationData, gl_PrimitiveID, inputWorldNormal, tangent, texCoord0, texCoord1, texCoord2, color0, depthOffset, materialIndex);
#endif
	worldPosition += fromCameraDirection * depthOffset;

	float ownerCameraDistance = length(worldPosition - u_viewportOwnerCameraPosition);
	float cameraDistance = length(worldPosition - u_cameraPosition.xyz);
	
	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals.y * u_shadowObjectVisibilityDistanceFactor;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, ownerCameraDistance);
	smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
#endif	

	if( cutVolumes( worldPosition ) )
		discard;
	
	//end of geometry stage

	//shading

	
	//vec2 depth = v_depth;
	////depth.x += u_shadowBias.x + u_shadowBias.y * fwidth(depth.x);

/*#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM
	vec2 rg = shadowCasterEVSM(depth, u_farClipDistance.x);
	gl_FragColor = vec4(rg.x, rg.y, 0, 1);
#else*/


	float newFragCoordZ = fragCoord.z;

//!!!!GLSL
#ifndef GLSL
#if (defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)) || (defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED)) || defined(DEPTH_OFFSET_MODE_GREATER_EQUAL)
	#ifdef LIGHT_TYPE_DIRECTIONAL
		float depth2 = fragCoord.z * u_farClipDistance.x;
		//!!!!
		depth2 += depthOffset;
		gl_GreaterEqualFragDepth = newFragCoordZ = depth2 / u_farClipDistance.x;
	#else
		float depth2 = getDepthValue(fragCoord.z, u_nearClipDistance.x, u_farClipDistance.x);
		//!!!!
		depth2 += depthOffset;
		gl_GreaterEqualFragDepth = newFragCoordZ = getRawDepthValue(depth2, u_nearClipDistance.x, u_farClipDistance.x);
	#endif
#elif defined(DEPTH_OFFSET_MODE_LESS_EQUAL)
	#ifdef LIGHT_TYPE_DIRECTIONAL
		float depth2 = fragCoord.z * u_farClipDistance.x;
		//!!!!
		depth2 += depthOffset;
		gl_LessEqualFragDepth = newFragCoordZ = depth2 / u_farClipDistance.x;
	#else
		float depth2 = getDepthValue(fragCoord.z, u_nearClipDistance.x, u_farClipDistance.x);
		//!!!!
		depth2 += depthOffset;
		gl_LessEqualFragDepth = newFragCoordZ = getRawDepthValue(depth2, u_nearClipDistance.x, u_farClipDistance.x);
	#endif
#endif
#endif


	float depth;
#ifdef LIGHT_TYPE_POINT
	depth = length(worldPosition - u_cameraPosition.xyz);
#elif LIGHT_TYPE_SPOTLIGHT
	depth = getDepthValue(newFragCoordZ, u_nearClipDistance.x, u_farClipDistance.x);	
#else
	depth = newFragCoordZ;
#endif

	//!!!!special for mobile
#ifdef GLSL
	#ifndef LIGHT_TYPE_POINT
		depth = glPositionZ;
	#endif
#endif


	float normalizedDepth = depth / u_farClipDistance.x;
#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
	gl_FragColor = packFloatToRgba(normalizedDepth);
#else
	gl_FragColor = vec4(normalizedDepth, 0, 0, 1);
#endif

//#endif

}
