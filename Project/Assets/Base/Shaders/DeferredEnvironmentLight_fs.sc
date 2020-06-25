$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "UniformsFragment.sh"

uniform vec4 u_lightDataFragment[LIGHTDATA_FRAGMENT_SIZE];

uniform vec4 u_reflectionProbeData;
#define u_reflectionProbePosition u_reflectionProbeData.xyz
#define u_reflectionProbeRadius u_reflectionProbeData.w
//uniform vec4 u_reflectionProbeDataFragment[REFLECTION_PROBE_DATA_FRAGMENT_SIZE];

uniform mat3 u_environmentTextureRotation;
uniform mat3 u_environmentTextureIBLRotation;
uniform vec4 /*vec3*/ u_environmentTextureMultiplier;
uniform vec4 /*vec3*/ u_environmentTextureIBLMultiplier;

SAMPLER2D(s_sceneTexture, 0);
SAMPLER2D(s_normalTexture, 1);
SAMPLER2D(s_gBuffer2Texture, 2);
SAMPLER2D(s_gBuffer3Texture, 9);
SAMPLER2D(s_depthTexture, 3);
SAMPLER2D(s_gBuffer4Texture, 10);

SAMPLERCUBE(s_environmentTexture, 6);
SAMPLERCUBE(s_environmentTextureIBL, 7);
SAMPLER2D(s_brdfLUT, 8);

#include "PBRFilament\common_types.sh"
#include "PBRFilament\common_math.sh"
#include "PBRFilament\brdf.sh"
#include "PBRFilament\PBRFilament.sh"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	float rawDepth = texture2D(s_depthTexture, v_texCoord0).r;
	vec3 worldPosition = reconstructWorldPosition(u_invViewProj, v_texCoord0, rawDepth);

	// discard if probe pass and pixel outside of probe
	float distanceToProbe = 0;
	BRANCH
	if(u_reflectionProbeRadius > 0)
	{
		distanceToProbe = length(u_reflectionProbePosition - worldPosition);
		BRANCH
		if(distanceToProbe > u_reflectionProbeRadius)
			discard;
	}

	vec3 baseColor = decodeRGBE8(texture2D(s_sceneTexture, v_texCoord0));	
	vec4 normal_and_reflectance = texture2D(s_normalTexture, v_texCoord0);
	vec3 normal = normalize(normal_and_reflectance.rgb * 2 - 1);

	vec4 gBuffer2Data = texture2D(s_gBuffer2Texture, v_texCoord0);
	vec4 gBuffer3Data = texture2D(s_gBuffer3Texture, v_texCoord0);

	//get GBuffer 4
	vec4 tangent;
	bool shadingModelSimple;
	bool receiveDecals;
	vec4 gBuffer4Data = texture2D(s_gBuffer4Texture, v_texCoord0);
	decodeGBuffer4(gBuffer4Data, tangent, shadingModelSimple, receiveDecals);

	//vec4 tangent = texture2D(s_tangentTexture, v_texCoord0) * 2 - 1;
	//tangent.xyz = normalize(tangent.xyz);
	vec3 bitangent = normalize(cross(tangent.xyz, normal) * tangent.w);

	vec3 toCamera = u_viewportOwnerCameraPosition - worldPosition.xyz;
	//float cameraDistance = length(toCamera);
	toCamera = normalize(toCamera);

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
		//Lit shading model
		
		//!!!!
		// дл€ relfection probe надо тут использовать собственный параметр
		// чтобы у них могла быть €ркость отлична€ от ambient

		float metallic = gBuffer2Data.x;
		float roughness = gBuffer2Data.y;
		float ambientOcclusion = gBuffer2Data.z;
		float reflectance = normal_and_reflectance.a;

		//toLight
		vec3 toLight = normal; // For ambient light dot(N, L) == 1, i.e. we can do L = N;

		MaterialInputs material;

		material.baseColor        = vec4(baseColor, 0.0);
		material.roughness        = roughness;
		material.metallic         = metallic;
		material.reflectance      = reflectance;
		material.ambientOcclusion = ambientOcclusion;
		//material.emissive         = vec4(emissive, 0.0);

		material.anisotropyDirection = vec3_splat(0);
		material.anisotropy = 0;

		#if defined(SHADING_MODEL_SUBSURFACE)
			material.thickness = 0;
			material.subsurfacePower = 0;
			material.subsurfaceColor = vec3_splat(0);
		#endif

		material.clearCoat = 0;
		material.clearCoatRoughness = 0;
		material.clearCoatNormal = vec3_splat(0);

		#if defined(SHADING_MODEL_CLOTH)
			material.sheenColor = vec3_splat(0);
		
			#if defined(MATERIAL_HAS_SUBSURFACE_COLOR)
				material.subsurfaceColor = vec3_splat(0);
			#endif

		#endif

		setupPBRFilamentParams(material, tangent.xyz, bitangent, normal, vec3_splat(0), toLight, toCamera, gl_FrontFacing);

		PixelParams pixel;
		getPBRFilamentPixelParams(material, pixel);

		EnvironmentTextureData data1;
		data1.rotation = u_environmentTextureRotation;
		data1.multiplier = u_environmentTextureMultiplier;
		
		EnvironmentTextureData dataIBL;
		dataIBL.rotation = u_environmentTextureIBLRotation;
		dataIBL.multiplier = u_environmentTextureIBLMultiplier;
			
		resultColor.rgb = iblDiffuse(material, pixel, s_environmentTextureIBL, dataIBL, s_environmentTexture, data1) +
			iblSpecular(material, pixel, vec3_splat(0), 0, s_environmentTexture, data1);

		resultColor.rgb *= lightColor;
	}
		
	//emissive
	resultColor.rgb += decodeRGBE8(gBuffer3Data) * u_emissiveMaterialsFactor;//gBuffer3Data.rgb

	BRANCH
	if(u_reflectionProbeRadius > 0)
	{
		float blendingStartsAt = 0.8;
		// позже лучше сделать отдельный входной параметр вместо 0.8
		float alpha = (u_reflectionProbeRadius - distanceToProbe) / ((1 - blendingStartsAt) * u_reflectionProbeRadius);
		resultColor.a = saturate(alpha);
	}

	gl_FragColor = resultColor;
}