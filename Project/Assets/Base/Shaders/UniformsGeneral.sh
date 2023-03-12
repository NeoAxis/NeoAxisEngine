// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

//!!!! later maybe split to vertex, fragment
uniform vec4 u_viewportOwnerSettings[7];
#define u_viewportOwnerCameraPosition u_viewportOwnerSettings[0].xyz
#define u_viewportOwnerNearClipDistance u_viewportOwnerSettings[0].w
#define u_viewportOwnerFarClipDistance u_viewportOwnerSettings[1].x
#define u_viewportOwnerFieldOfView u_viewportOwnerSettings[1].y
#define u_viewportOwnerDebugMode int(u_viewportOwnerSettings[1].z)
#define u_emissiveMaterialsFactor u_viewportOwnerSettings[1].w
//#define u_cameraEv100 u_viewportOwnerSettings[1].w
#define u_viewportOwnerShadowFarDistance u_viewportOwnerSettings[2].xyz
#define u_cameraExposure u_viewportOwnerSettings[2].w
#define u_displacementScale u_viewportOwnerSettings[3].x
#define u_displacementMaxSteps int(u_viewportOwnerSettings[3].y)
#define u_removeTextureTiling u_viewportOwnerSettings[3].z
#define u_provideColorDepthTextureCopy u_viewportOwnerSettings[3].w
#define u_viewportOwnerCameraDirection u_viewportOwnerSettings[4].xyz
#define u_engineTime u_viewportOwnerSettings[4].w
#define u_viewportOwnerCameraUp u_viewportOwnerSettings[5].xyz
#define u_mipBias u_viewportOwnerSettings[5].w
#define u_windSpeed u_viewportOwnerSettings[6].xy
#define u_shadowObjectVisibilityDistanceFactor u_viewportOwnerSettings[6].z

uniform vec4 u_viewportSettings[1];
#define u_viewportSize u_viewportSettings[0].xy
#define u_viewportSizeInv u_viewportSettings[0].zw

#ifdef GLOBAL_FOG
uniform vec4 u_fogSettings[3];
#define u_fogColor u_fogSettings[0]
#define u_fogDistanceMode u_fogSettings[1].x
#define u_fogStartDistance u_fogSettings[1].y
#define u_fogDensity u_fogSettings[1].z
#define u_fogHeightMode u_fogSettings[1].w
#define u_fogHeight u_fogSettings[2].x
#define u_fogHeightScale u_fogSettings[2].y
#endif

#if GLOBAL_CUT_VOLUME_MAX_AMOUNT > 0
uniform vec4 u_viewportCutVolumeSettings;
uniform mat4 u_viewportCutVolumeData[GLOBAL_CUT_VOLUME_MAX_AMOUNT];
#endif

#ifdef GLSL
	#define getFragCoord() vec4(gl_FragCoord.x, u_viewportSize.y - gl_FragCoord.y, gl_FragCoord.z, gl_FragCoord.w)
#else
	#define getFragCoord() gl_FragCoord
#endif
