$input v_depth, v_worldPosition, v_lodValueVisibilityDistanceReceiveDecals

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "FragmentFunctions.sh"

uniform vec4 u_renderOperationData[5];
uniform vec4/*float*/ u_farClipDistance;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	float cameraDistance = length(u_viewportOwnerCameraPosition - v_worldPosition);
	
	//lod
	float lodValue = v_lodValueVisibilityDistanceReceiveDecals.x;
	smoothLOD(getFragCoord(), lodValue);
	
	//fading by visibility distance
	float visibilityDistance = v_lodValueVisibilityDistanceReceiveDecals.y;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
	smoothLOD(getFragCoord(), 1.0f - visibilityDistanceFactor);

	cutVolumes(v_worldPosition);
	
	vec2 depth = v_depth;
	//!!!!
	//depth.x += u_shadowBias.x + u_shadowBias.y * fwidth(depth.x);

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
