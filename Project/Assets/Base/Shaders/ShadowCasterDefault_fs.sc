$input v_depth, v_worldPosition, v_lodValue_visibilityDistance_receiveDecals, v_texCoord0, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles, v_billboardRotation

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define SHADOW_CASTER 1
#include "Common.sh"
#include "FragmentFunctions.sh"
//#include "Sh_adowCasterFunctions.sh"

uniform vec4 u_renderOperationData[5];
uniform vec4/*float*/ u_farClipDistance;

#ifdef GLOBAL_BILLBOARD_DATA
	SAMPLER2DARRAY(s_billboardData, 2);
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	vec4 fragCoord = getFragCoord();

	vec2 texCoord0 = v_texCoord0;
	
	//billboard with geometry data mode
#ifdef GLOBAL_BILLBOARD_DATA
	float billboardDataMode = u_renderOperationData[1].w;
	vec3 inputWorldNormal = vec3_splat(0);
	vec2 texCoord1 = vec2_splat(0);
	vec2 texCoord2 = vec2_splat(0);
	billboardDataModeCalculateParameters(billboardDataMode, s_billboardData, fragCoord, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles, v_billboardRotation, inputWorldNormal, texCoord0, texCoord1, texCoord2);
#endif
	
	float cameraDistance = length(u_viewportOwnerCameraPosition - v_worldPosition);
	
	//lod
#ifdef GLOBAL_SMOOTH_LOD
	float lodValue = v_lodValue_visibilityDistance_receiveDecals.x;
	smoothLOD(fragCoord, lodValue);
#endif
	
	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals.y * u_shadowObjectVisibilityDistanceFactor;
	float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
	smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
#endif	

	cutVolumes(v_worldPosition);
	
	vec2 depth = v_depth;
	//depth.x += u_shadowBias.x + u_shadowBias.y * fwidth(depth.x);

/*#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM
	vec2 rg = shadowCasterEVSM(depth, u_farClipDistance.x);
	gl_FragColor = vec4(rg.x, rg.y, 0, 1);
#else*/

	float normalizedDepth = depth.x / u_farClipDistance.x;
#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
	gl_FragColor = packFloatToRgba(normalizedDepth);
#else
	gl_FragColor = vec4(normalizedDepth, 0, 0, 1);
#endif

//#endif
}
