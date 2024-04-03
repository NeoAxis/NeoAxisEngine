// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

//!!!!cbuffer
uniform vec4 u_viewportOwnerSettings[10];
#define u_viewportOwnerCameraPositionSinglePrecision u_viewportOwnerSettings[0].xyz
//camera position is always zero. relative camera rendering
#define u_viewportOwnerCameraPosition vec3(0.0,0.0,0.0)
//#define u_viewportOwnerCameraPosition u_viewportOwnerSettings[0].xyz
#define u_viewportOwnerNearClipDistance u_viewportOwnerSettings[0].w
#define u_viewportOwnerFarClipDistance u_viewportOwnerSettings[1].x
#define u_viewportOwnerFieldOfView u_viewportOwnerSettings[1].y
#define u_viewportOwnerDebugMode int(u_viewportOwnerSettings[1].z)
#define u_emissiveMaterialsFactor u_viewportOwnerSettings[1].w
//#define u_cameraEv100 u_viewportOwnerSettings[1].w
#define u_viewportOwnerShadowDirectionalDistance u_viewportOwnerSettings[2].xyz
#define u_cameraExposure u_viewportOwnerSettings[2].w
#define u_displacementScale u_viewportOwnerSettings[3].x
#define u_displacementMaxSteps int(u_viewportOwnerSettings[3].y)
#define u_removeTextureTiling u_viewportOwnerSettings[3].z
#define u_provideColorDepthTextureCopy u_viewportOwnerSettings[3].w
#define u_viewportOwnerCameraDirection u_viewportOwnerSettings[4].xyz
#define u_engineTime u_viewportOwnerSettings[4].w
#define u_viewportOwnerCameraUp u_viewportOwnerSettings[5].xyz
//mip bias is disabled
#define u_mipBias 0.0
//#define u_mipBias u_viewportOwnerSettings[5].w
#define u_windSpeed u_viewportOwnerSettings[6].xy
#define u_shadowObjectVisibilityDistanceFactor u_viewportOwnerSettings[6].z
#define u_debugCapsLock u_viewportOwnerSettings[6].w
#define u_viewportOwnerCameraPositionPreviousFrameChange u_viewportOwnerSettings[7].xyz
#define u_temperature u_viewportOwnerSettings[7].w
#define u_viewportOwnerCameraPositionDivide987654Remainder u_viewportOwnerSettings[8].xyz
#define u_precipitationFalling u_viewportOwnerSettings[8].w
#define u_viewportOwnerShadowPointSpotlightDistance u_viewportOwnerSettings[9].xyz

#define u_precipitationFallen unpackHalf2x16( asuint( u_viewportOwnerSettings[9].w ) ).x
#define u_timeOfDay unpackHalf2x16( asuint( u_viewportOwnerSettings[9].w ) ).y
//#define u_precipitationFallen u_viewportOwnerSettings[9].w

#define capsLock (u_debugCapsLock != 0.0)

uniform mat4 u_viewportOwnerViewProjection;
uniform mat4 u_viewportOwnerView;
uniform mat4 u_viewportOwnerProjection;
uniform mat4 u_viewportOwnerViewProjectionPrevious;
uniform mat4 u_viewportOwnerViewInverse;
uniform mat4 u_viewportOwnerProjectionInverse;
uniform mat4 u_viewportOwnerViewProjectionInverse;
/*
uniform mat4 u_viewportOwnerMatrices[7];
#define u_viewportOwnerViewProjection u_viewportOwnerMatrices[0]
#define u_viewportOwnerView u_viewportOwnerMatrices[1]
#define u_viewportOwnerProjection u_viewportOwnerMatrices[2]
#define u_viewportOwnerViewProjectionPrevious u_viewportOwnerMatrices[3]
#define u_viewportOwnerViewInverse u_viewportOwnerMatrices[4]
#define u_viewportOwnerProjectionInverse u_viewportOwnerMatrices[5]
#define u_viewportOwnerViewProjectionInverse u_viewportOwnerMatrices[6]
*/

uniform vec4 u_viewportSettings[1];
#define u_viewportSize u_viewportSettings[0].xy
#define u_viewportSizeInv u_viewportSettings[0].zw

//!!!!cbuffer: forward only
#ifdef GLOBAL_FOG
uniform vec4 u_fogSettings[3];
#define u_fogColor u_fogSettings[0]
#define u_fogDistanceMode u_fogSettings[1].x
#define u_fogStartDistance u_fogSettings[1].y
#define u_fogDensity u_fogSettings[1].z
#define u_fogHeightMode u_fogSettings[1].w
#define u_fogHeight u_fogSettings[2].x
#define u_fogHeightScale u_fogSettings[2].y
#define u_fogAffectBackground u_fogSettings[2].z
#endif

//!!!!cbuffer
#if GLOBAL_CUT_VOLUME_MAX_AMOUNT > 0
uniform vec4 u_viewportCutVolumeSettings;
uniform mat4 u_viewportCutVolumeData[GLOBAL_CUT_VOLUME_MAX_AMOUNT];
#endif

//!!!!cbuffer
#ifdef SHADOW_CASTER
uniform vec4 u_prepareShadowsSettings[2];
#define u_cameraPosition u_prepareShadowsSettings[0].xyz
#define u_startDistance u_prepareShadowsSettings[0].w
#define u_farClipDistance u_prepareShadowsSettings[1].x
#define u_nearClipDistance u_prepareShadowsSettings[1].y
#define u_shadowMaterialOpacityMaskThresholdFactor u_prepareShadowsSettings[1].z
#define u_shadowTexelOffset u_prepareShadowsSettings[1].w
#endif

#ifdef GLSL
	#define getFragCoord() vec4(gl_FragCoord.x, u_viewportSize.y - gl_FragCoord.y, gl_FragCoord.z, gl_FragCoord.w)
#else
	#define getFragCoord() gl_FragCoord
#endif
