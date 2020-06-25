$input v_depth

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "FragmentFunctions.sh"

uniform vec4 u_renderOperationData[5];
uniform vec4/*float*/ u_farClipDistance;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	smoothLOD(gl_FragCoord, u_renderOperationData[2].w);

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
