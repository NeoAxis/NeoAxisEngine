// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#define LIGHTDATA_FRAGMENT_SIZE 29
//#define LIGHTDATA_FRAGMENT_SIZE 28
//vec4 u_lightDataFragment[LIGHTDATA_FRAGMENT_SIZE]

#define u_lightPosition u_lightDataFragment[0]
#define u_lightDirection u_lightDataFragment[1].xyz
#define u_lightPower u_lightDataFragment[2].xyz
#define u_lightAttenuation u_lightDataFragment[3]
#define u_lightSpot u_lightDataFragment[4]
#define u_lightShadowIntensity u_lightDataFragment[5].x
#define u_lightShadowTextureSize u_lightDataFragment[5].y
#define u_lightShadowMapFarClipDistance u_lightDataFragment[5].z
#define u_lightShadowCascadesVisualize u_lightDataFragment[5].w
#define u_lightShadowTextureViewProjMatrix0 mat4(u_lightDataFragment[6], u_lightDataFragment[7], u_lightDataFragment[8], u_lightDataFragment[9])
#define u_lightShadowTextureViewProjMatrix1 mat4(u_lightDataFragment[10], u_lightDataFragment[11], u_lightDataFragment[12], u_lightDataFragment[13])
#define u_lightShadowTextureViewProjMatrix2 mat4(u_lightDataFragment[14], u_lightDataFragment[15], u_lightDataFragment[16], u_lightDataFragment[17])
#define u_lightShadowTextureViewProjMatrix3 mat4(u_lightDataFragment[18], u_lightDataFragment[19], u_lightDataFragment[20], u_lightDataFragment[21])
#define u_lightShadowCascades u_lightDataFragment[22]
#define u_lightMaskMatrix mat4(u_lightDataFragment[23], u_lightDataFragment[24], u_lightDataFragment[25], u_lightDataFragment[26])
#define u_lightShadowUnitDistanceTexelSizes u_lightDataFragment[27]
#define u_lightShadowBias u_lightDataFragment[28].x
#define u_lightShadowNormalBias u_lightDataFragment[28].y

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//#define REFLECTION_PROBE_DATA_FRAGMENT_SIZE 2
//#define u_reflectionProbePosition u_reflectionProbeDataFragment[0]
//#define u_reflectionProbeRadius u_reflectionProbeDataFragment[1].x

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#define MATERIAL_STANDARD_FRAGMENT_SIZE 8

#define u_materialBaseColor materialStandardFragment[0].xyz
#define u_materialOpacity materialStandardFragment[0].w
#define u_materialAnisotropyDirection materialStandardFragment[1].xyz
#define u_materialOpacityMaskThreshold materialStandardFragment[1].w
#define u_materialSubsurfaceColor materialStandardFragment[2].xyz
#define u_materialMetallic materialStandardFragment[2].w
#define u_materialSheenColor materialStandardFragment[3].xyz
#define u_materialRoughness materialStandardFragment[3].w
#define u_materialEmissive materialStandardFragment[4].xyz
#define u_materialReflectance materialStandardFragment[4].w
#define u_materialClearCoat materialStandardFragment[5].x
#define u_materialClearCoatRoughness materialStandardFragment[5].y
#define u_materialAnisotropy materialStandardFragment[5].z
#define u_materialThickness materialStandardFragment[5].w
#define u_materialSubsurfacePower materialStandardFragment[6].x
#define u_materialRayTracingReflection materialStandardFragment[6].y
#define u_materialAnisotropyDirectionBasis materialStandardFragment[6].z
#define u_materialShininess materialStandardFragment[6].w
#define u_materialDisplacementScale materialStandardFragment[7].x
#define u_materialReceiveDecals materialStandardFragment[7].y
#define u_materialUseVertexColor materialStandardFragment[7].z


//#if defined(USAGEMODE_MATERIALBLENDFIRST) || defined(USAGEMODE_MATERIALBLENDNOTFIRST)
//	uniform vec4/*float*/ param_materialBlendMask;
//#endif
