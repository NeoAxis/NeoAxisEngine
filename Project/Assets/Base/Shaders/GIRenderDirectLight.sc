// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#define GI_DIRECT_LIGHT
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
//UIMAGE3D_RO(s_gbufferGrid, rg32ui, 1);
//IMAGE3D_RO(s_lightingGridCopy, rgba16f, 2);
//IMAGE3D_RW(s_lightingGrid, rgba16f, 1);

#ifdef GLOBAL_LIGHT_MASK_SUPPORT
	#ifdef LIGHT_TYPE_POINT
		SAMPLERCUBE(s_lightMask, 4);
	#elif defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_DIRECTIONAL)
		SAMPLER2D(s_lightMask, 4);
	#endif
#endif

#ifdef SHADOW_MAP
	#ifdef LIGHT_TYPE_POINT
		#ifdef SHADOW_TEXTURE_FORMAT_BYTE4 //GLOBAL_SHADOW_TECHNIQUE_EVSM
			SAMPLERCUBE(s_shadowMapShadow, 5);
		#else
			SAMPLERCUBESHADOW(s_shadowMapShadow, 5);
		#endif
	#else
		#ifdef SHADOW_TEXTURE_FORMAT_BYTE4 //GLOBAL_SHADOW_TECHNIQUE_EVSM
			SAMPLER2DARRAY(s_shadowMapShadow, 5);
		#else
			SAMPLER2DARRAYSHADOW(s_shadowMapShadow, 5);
		#endif
	#endif
#endif

SAMPLER2D(s_brdfLUT, 6);

#ifdef SHADOW_MAP
	#include "ShadowReceiverFunctions.sh"
#endif

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
	float directLighting = giRenderLightParameters[ 2 ].w;
	vec3 giGridIndexesMax = giRenderLightParameters[ 3 ].xyz;

	ivec3 giGridIndexOffset = ivec3( giGridIndexesMin.xyz );
	ivec3 giGridIndexSize = ivec3( giGridIndexesMax.xyz ) - giGridIndexOffset + ivec3(1,1,1);
	
	if( gl_GlobalInvocationID.x >= giGridIndexSize.x || gl_GlobalInvocationID.y >= giGridIndexSize.y || gl_GlobalInvocationID.z >= giGridIndexSize.z )
		return;
	
	ivec3 giGridIndex = giGridIndexOffset + ivec3( gl_GlobalInvocationID );
	
	vec4 currentLighting = texelFetch( s_lightingGridTextureCopy, giGridIndex, 0 );
	//vec4 currentLighting = imageLoad( s_lightingGridCopy, giGridIndex );

	//skip if empty voxel
	BRANCH
	if( currentLighting.a < 0.5 )
		return;
	
	vec3 worldPosition = gridPosition.xyz + ( vec3( giGridIndex ) + vec3_splat( 0.5 ) ) * giCellSize;

	//objectLightAttenuation
	float objectLightAttenuation = 1;
	#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT)
		float lightDistance = length(u_lightPosition.xyz - worldPosition);
		objectLightAttenuation = getLightAttenuation(u_lightAttenuation, (float)lightDistance);
	#endif

	BRANCH
	if( objectLightAttenuation <= 0.0 )
		return;

	//lightWorldDirection
	vec3 lightWorldDirection;
	#ifdef LIGHT_TYPE_DIRECTIONAL
		lightWorldDirection = -u_lightDirection;
	#else
		lightWorldDirection = (vec3)normalize(u_lightPosition.xyz - worldPosition);
	#endif
	
	#ifdef LIGHT_TYPE_SPOTLIGHT
		// factor in spotlight angle
		float rho0 = saturate(dot(-u_lightDirection, lightWorldDirection));
		// factor = (rho - cos(outer/2)) / (cos(inner/2) - cos(outer/2)) ^ falloff 
		float spotFactor0 = saturate(pow(saturate(rho0 - u_lightSpot.y) / (u_lightSpot.x - u_lightSpot.y), u_lightSpot.z));
		objectLightAttenuation *= spotFactor0;
	#endif

	BRANCH
	if( objectLightAttenuation <= 0.0 )
		return;


	ivec3 giGridIndexWithOffset = giGridIndex;
	giGridIndexWithOffset.x += int( gridResolution * currentLevel );
	
	uvec4 gbufferValue = texelFetch( s_gbufferGridTexture, giGridIndexWithOffset, 0 );
	//uvec4 gbufferValue = imageLoad( s_gbufferGrid, giGridIndexWithOffset );
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

	
	//!!!!?
	vec3 toCamera = -lightWorldDirection;
	
	//vec3 toCamera = u_viewportOwnerCameraPosition - worldPosition.xyz;
	//float cameraDistance = length(toCamera);
	//toCamera = normalize(toCamera);
	

	//lightMaskMultiplier
	vec3 lightMaskMultiplier = vec3(1,1,1);
#ifdef GLOBAL_LIGHT_MASK_SUPPORT
	{
	#ifdef LIGHT_TYPE_POINT
		vec3 dir = normalize(worldPosition - u_lightPosition.xyz);
		dir = mul(u_lightMaskMatrix, vec4(dir, 0)).xyz;
		//!!!!flipped cubemaps, already applied in lightMaskTextureMatrixArray.
		//dir = float3(-dir.y, dir.z, dir.x);
		lightMaskMultiplier = textureCubeLod(s_lightMask, dir, 0).rgb;
	#elif defined(LIGHT_TYPE_SPOTLIGHT)
		vec4 texCoord = mul( u_lightMaskMatrix, vec4( worldPosition, 1 ) );
		lightMaskMultiplier = texture2DProjLod( s_lightMask, texCoord, 0 ).rgb;
	#elif defined(LIGHT_TYPE_DIRECTIONAL)
		vec2 texCoord = mul( u_lightMaskMatrix, vec4( worldPosition, 1 ) ).xy;
		lightMaskMultiplier = texture2DLod( s_lightMask, texCoord, 0 ).rgb;
	#endif
	}
#endif

	//shadows
	float shadowMultiplier = 1;
	#ifdef SHADOW_MAP
	
		//!!!!
		float cascadeDepth = 0;
		
		vec4 fragCoord = vec4( giGridIndex.x, giGridIndex.y, 0, 0 );		
		shadowMultiplier = getShadowMultiplier( worldPosition, 0, cascadeDepth, lightWorldDirection, normal, vec2_splat( 0 ), fragCoord );
	#endif
	
	//light color
	vec3 lightColor = u_lightPower.rgb * u_cameraExposure * objectLightAttenuation * lightMaskMultiplier * shadowMultiplier;

	
	vec3 resultColor = vec3_splat(0);

	if(shadingModelSimple)
	{
		//Simple shading model
		resultColor.rgb = baseColor * lightColor;
	}
	else
	{
		//Lit, Subsurface shading models

#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE || !defined(GI_SHADING_MODEL_FULL)
		resultColor.rgb = baseColor * lightColor * saturate(dot(normal, lightWorldDirection) / PI);
#else
		
		//toLight
		vec3 toLight;
		#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT)
			toLight = u_lightPosition.xyz - worldPosition;
		#elif LIGHT_TYPE_DIRECTIONAL
			toLight = -u_lightDirection;
		#else
			toLight = normal; // For ambient light dot(N, L) == 1, i.e. we can do L = N;
		#endif
		toLight = normalize(toLight);

		MaterialInputs material;

		material.baseColor = vec4(baseColor, 0.0);
		material.roughness = roughness;
		material.metallic = metallic;
		material.reflectance = reflectance;
		material.ambientOcclusion = ambientOcclusion;

		material.anisotropyDirection = vec3_splat(0);
		material.anisotropy = 0;

		material.thickness = thickness;
		material.subsurfacePower = subsurfacePower;
		material.subsurfaceColor = subsurfaceColor;

		material.clearCoat = 0;
		material.clearCoatRoughness = 0;
		material.clearCoatNormal = vec3_splat(0);

		setupPBRFilamentParams(material, tangent.xyz, bitangent, normal, vec3_splat(0), toLight, toCamera, false/*gl_FrontFacing*/);

		PixelParams pixel;
		getPBRFilamentPixelParams(material, pixel);

		vec3 shadingResult;
		BRANCH
		if(shadingModelSubsurface)
			shadingResult = surfaceShadingSubSurface(pixel);
		else
			shadingResult = surfaceShadingStandard(pixel);
		resultColor.rgb = shadingResult * lightColor;
		
#endif
	}

	
	//write to output image
	
	//!!!!can't image load from rgba16f. so make copy before it

	currentLighting.xyz += resultColor * directLighting;
	currentLighting.w = 1.0;
	
	imageStore( s_lightingGrid, giGridIndexWithOffset, currentLighting );
}
