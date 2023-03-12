$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "../../FragmentFunctions.sh"

uniform mat4 invViewProj;
uniform vec4 cameraPosition;
uniform vec4 intensity;
uniform vec4 ambientLightPower;

SAMPLER2D(s_srcTexture, 0);
SAMPLER2D(s_blurRoughnessMin, 1);
SAMPLER2D(s_blurRoughnessMax, 2);
//SAMPLER2D(s_reflectionTexture, 1);
//SAMPLER2D(s_bluredReflectionTexture, 2);

SAMPLER2D(s_depthTexture, 3);
SAMPLER2D(s_normalTexture, 4);

SAMPLERCUBE(s_environmentTexture, 5);
//SAMPLERCUBE(s_environmentTextureIBL, 6);
SAMPLER2D(s_brdfLUT, 7);

SAMPLER2D(s_gBuffer2Texture, 8);

SAMPLER2D(s_gBuffer0Texture, 9);

#include "../../PBRFilament/common_math.sh"
#include "../../PBRFilament/brdf.sh"
#include "../../PBRFilament/PBRFilament.sh"

void main()
{
	vec3 resultColor = vec3_splat(0);

	BRANCH
	if(any(ambientLightPower.rgb)) //if ((ambientLightPower.r +  ambientLightPower.g + ambientLightPower.b) > 0.0)
	{
		vec3 sourceColor = texture2D(s_srcTexture, v_texCoord0).rgb;

		vec3 blurReflectionMin = texture2D(s_blurRoughnessMin, v_texCoord0).rgb;
		vec3 blurReflectionMax = texture2D(s_blurRoughnessMax, v_texCoord0).rgb;

		vec3 baseColor = decodeRGBE8(texture2D(s_gBuffer0Texture, v_texCoord0));
		vec4 normal_and_reflectance = texture2D(s_normalTexture, v_texCoord0);

		vec3 normal = normalize(normal_and_reflectance.rgb * 2 - 1);

		vec4 gBufferData = texture2D(s_gBuffer2Texture, v_texCoord0);

		float metallic             = gBufferData.x;
		float roughness            = gBufferData.y;
		float ambientOcclusion     = gBufferData.z;
		float rayTracingReflection = gBufferData.w;//1.0;
		float reflectance          = normal_and_reflectance.a;

		float rawDepth = texture2D(s_depthTexture, v_texCoord0).r;
		vec3 worldPosition = reconstructWorldPosition(invViewProj, v_texCoord0, rawDepth);
		vec3 toCamera = normalize(cameraPosition.xyz - worldPosition);
		vec3 toLight = normal;

		MaterialInputs material;

		material.baseColor = vec4(baseColor, 0.0);
		material.roughness = roughness;
		material.metallic = metallic;
		material.reflectance = reflectance;
		material.ambientOcclusion = ambientOcclusion;

		material.anisotropy = 0.0f;
		material.anisotropyDirection = vec3_splat(0);

		material.clearCoat = 0;
		material.clearCoatRoughness = 0;
		material.clearCoatNormal = vec3_splat(0);

		PixelParams pixel;
		getPBRFilamentPixelParams(material, pixel);
		
		setupPBRFilamentParams(material, vec3_splat(0), vec3_splat(0), normal, normal, toLight, toCamera, false);//gl_FrontFacing);

		EnvironmentTextureData data;
		data.rotation = vec4(0,0,0,1);
		data.multiplierAndAffect = vec4_splat(1);

		vec3 specularUsual;
		{
			specularUsual = iblSpecular(material, pixel, vec3_splat(0), 0, s_environmentTexture, data) * ambientLightPower.rgb * u_cameraExposure;
		}

		vec3 specularSSR;
		{
			vec3 reflectionColor = mix(blurReflectionMin, blurReflectionMax, roughness);
			specularSSR = iblSpecular(material, pixel, reflectionColor, 1, s_environmentTexture, data) * ambientLightPower.rgb * u_cameraExposure;
		}
		
		resultColor = sourceColor - specularUsual; //all minus ambient specular
		
		float factor = saturate(intensity.x * rayTracingReflection);
		resultColor += mix(specularUsual, specularSSR, factor); //add ambient specular
		
	}

	gl_FragColor = vec4(resultColor, 0.0);
	
	//vec3 finalColor = lerp(sourceColor, specIBL, factor);
	//gl_FragColor = vec4(finalColor, 0.0);
	//vec3 final_color = specIBL * reflectionKoeff.x + sourceColor * (1.0 - reflectionKoeff.x);
	//gl_FragColor = vec4(lerp(sourceColor, final_color, intensity.x).xyz, 0.0);
}
