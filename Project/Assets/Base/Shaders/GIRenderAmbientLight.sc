// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#define GI_AMBIENT_LIGHT
#include "Common/bgfx_compute.sh"
#include "Common.sh"
#include "GICommon.sh"
#include "UniformsFragment.sh"
#include "FragmentFunctions.sh"

uniform vec4 giRenderLightParameters[4];
uniform vec4 u_lightDataFragment[LIGHTDATA_FRAGMENT_SIZE];

IMAGE3D_WR(s_lightingGrid, rgba16f, 0);
USAMPLER3D(s_gbufferGridTexture, 1);
SAMPLER3D(s_lightingGridTextureCopy, 2);

uniform vec4 u_deferredEnvironmentData[4];
#define u_deferredEnvironmentDataRotation u_deferredEnvironmentData[0]
#define u_deferredEnvironmentDataMultiplierAndAffect u_deferredEnvironmentData[1]
#define u_deferredEnvironmentDataIBLRotation u_deferredEnvironmentData[2]
#define u_deferredEnvironmentDataIBLMultiplierAndAffect u_deferredEnvironmentData[3]
uniform vec4 u_deferredEnvironmentIrradiance[9];

SAMPLERCUBE(s_environmentTexture, 8);
SAMPLER2D(s_brdfLUT, 6);

#define SHADING_MODEL_SUBSURFACE

#include "PBRFilament/common_math.sh"
#include "PBRFilament/brdf.sh"
#include "PBRFilament/PBRFilament.sh"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Returns a vector that is orthogonal to u.
vec3 orthogonal(vec3 u)
{
	u = normalize(u);
	vec3 v = vec3(0.99146, 0.11664, 0.05832); // Pick any normalized vector.
	return abs(dot(u, v)) > 0.99999f ? cross(u, vec3(0, 1, 0)) : cross(u, v);
}

NUM_THREADS(8, 8, 8)
void main()
{
	float giCellSize = giRenderLightParameters[ 0 ].x;
	float giVoxelizationFactor = giRenderLightParameters[ 0 ].y;
	float gridResolution = giRenderLightParameters[ 0 ].z;
	float currentLevel = giRenderLightParameters[ 0 ].w;
	vec3 gridPosition = giRenderLightParameters[ 1 ].xyz;	
	vec3 giGridIndexesMin = giRenderLightParameters[ 2 ].xyz;
	float ambientLighting = giRenderLightParameters[ 2 ].w;
	vec3 giGridIndexesMax = giRenderLightParameters[ 3 ].xyz;

	ivec3 giGridIndexOffset = ivec3( giGridIndexesMin.xyz );
	ivec3 giGridIndexSize = ivec3( giGridIndexesMax.xyz ) - giGridIndexOffset + ivec3(1,1,1);
	
	if( gl_GlobalInvocationID.x >= giGridIndexSize.x || gl_GlobalInvocationID.y >= giGridIndexSize.y || gl_GlobalInvocationID.z >= giGridIndexSize.z )
		return;
	
	ivec3 giGridIndex = giGridIndexOffset + ivec3( gl_GlobalInvocationID );
	
	vec4 currentLighting = texelFetch( s_lightingGridTextureCopy, giGridIndex, 0 );
	
	//skip if empty voxel
	BRANCH
	if( currentLighting.a < 0.5 )
		return;
	
	vec3 worldPosition = gridPosition.xyz + ( vec3( giGridIndex ) + vec3_splat( 0.5 ) ) * giCellSize;


	ivec3 giGridIndexWithOffset = giGridIndex;
	giGridIndexWithOffset.x += int( gridResolution * currentLevel );
	
	uvec4 gbufferValue = texelFetch( s_gbufferGridTexture, giGridIndexWithOffset, 0 );
	vec4 gbuffer0 = convertRGBA8ToVec4( gbufferValue.x ) / 255.0;
	
	bool filledVoxel = any( gbuffer0.xyz );
	
	BRANCH
	if( !filledVoxel )
		return;

	vec4 gbuffer1 = convertRGBA8ToVec4( gbufferValue.y ) / 255.0;
	vec4 gbuffer2 = convertRGBA8ToVec4( gbufferValue.z ) / 255.0;
	vec4 gbuffer3 = convertRGBA8ToVec4( gbufferValue.w ) / 255.0;
	
	vec3 normal = normalize( expand( gbuffer0.xyz ) );
	float roughness = gbuffer0.w;
	vec3 baseColor = gbuffer1.xyz;
	float metallic = gbuffer1.w;

	
	//!!!!
	float reflectance = 0.5;
	
	//!!!!
	float ambientOcclusion = 1;
	
	
	//!!!!
	vec4 tangent = vec4( orthogonal( normal ), -1.0 );
	//vec4 tangent = gBuffer4Data * 2.0 - 1.0;
	//tangent.xyz = normalize(tangent.xyz);


	//!!!!	
	int shadingModel = 0;//int(round(gBuffer5Data.x * 8.0));
	bool receiveDecals = false;//gBuffer5Data.y == 1.0;
	float thickness = 0;//gBuffer5Data.z;
	float subsurfacePower = 0;//gBuffer5Data.w * 15.0;
	
	bool shadingModelSubsurface = shadingModel == 1;
	bool shadingModelSimple = shadingModel == 3;
	
	vec3 subsurfaceColor = vec3_splat(0);
	{
		//!!!!
		//vec3 v = decodeRGBE8(gBuffer3Data);
		//if(shadingModelSubsurface)
		//	subsurfaceColor = v;
	}
	
	vec3 bitangent = normalize(cross(tangent.xyz, normal) * tangent.w);
	

	vec3 toCamera = normal;

	//light color
	vec3 lightColor = u_lightPower.rgb * u_cameraExposure;

	
	vec4 resultColor = vec4_splat(0);

	if(shadingModelSimple)
	{
		//Simple shading model
		resultColor.rgb = baseColor * lightColor;
	}
	else
	{
		//Lit, Subsurface shading models

#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE || !defined(GI_SHADING_MODEL_FULL)
		resultColor.rgb = baseColor * lightColor;
#else

		//toLight
		vec3 toLight = normal; // For ambient light dot(N, L) == 1, i.e. we can do L = N;

		MaterialInputs material;

		material.baseColor = vec4(baseColor, 0.0);
		material.roughness = roughness;
		material.metallic = metallic;
		material.reflectance = reflectance;
		material.ambientOcclusion = ambientOcclusion;

		material.anisotropyDirection = vec3_splat(0);
		material.anisotropy = 0;

		//#if defined(SHADING_MODEL_SUBSURFACE)
		material.thickness = thickness;
		material.subsurfacePower = subsurfacePower;
		material.subsurfaceColor = subsurfaceColor;
		//#endif

		material.clearCoat = 0;
		material.clearCoatRoughness = 0;
		material.clearCoatNormal = vec3_splat(0);

		/*
		#if defined(SHADING_MODEL_CLOTH)
			material.sheenColor = vec3_splat(0);		
			material.subsurfaceColor = vec3_splat(0);
		#endif
		*/

		setupPBRFilamentParams(material, tangent.xyz, bitangent, normal, vec3_splat(0), toLight, toCamera, false/*gl_FrontFacing*/);

		PixelParams pixel;
		getPBRFilamentPixelParams(material, pixel);
		
		EnvironmentTextureData data1;
		data1.rotation = u_deferredEnvironmentDataRotation;//u_environmentTextureRotation;
		data1.multiplierAndAffect = u_deferredEnvironmentDataMultiplierAndAffect;//u_environmentTextureMultiplierAndAffect;
		
		EnvironmentTextureData dataIBL;
		dataIBL.rotation = u_deferredEnvironmentDataIBLRotation;//u_environmentTextureIBLRotation;
		dataIBL.multiplierAndAffect = u_deferredEnvironmentDataIBLMultiplierAndAffect;//u_environmentTextureIBLMultiplierAndAffect;

		resultColor.rgb = iblDiffuse(material, pixel, u_deferredEnvironmentIrradiance, dataIBL, s_environmentTexture, data1, shadingModelSubsurface) +
			iblSpecular(material, pixel, vec3_splat(0), 0, s_environmentTexture, data1);

		resultColor.rgb *= lightColor;
#endif
	}
	

	//write to output image
	
	//!!!!can't image load from rgba16f. so make copy before it

	currentLighting.xyz += resultColor.xyz * ambientLighting;
	currentLighting.w = 1.0;
	
	imageStore( s_lightingGrid, giGridIndexWithOffset, currentLighting );
}
