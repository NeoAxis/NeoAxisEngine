$input v_texCoord01, v_worldPosition, v_worldNormal, v_depth, v_tangent, v_bitangent, v_fogFactor, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_position, v_previousPosition, v_texCoord23, v_colorParameter

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "UniformsFragment.sh"
#include "FragmentFunctions.sh"

uniform vec4 u_lightDataFragment[LIGHTDATA_FRAGMENT_SIZE];
uniform vec4 u_renderOperationData[5];

SAMPLER2D(s_materials, 1);

vec4 materialStandardFragment[MATERIAL_STANDARD_FRAGMENT_SIZE];

#ifdef DISPLACEMENT_CODE_PARAMETERS
	DISPLACEMENT_CODE_PARAMETERS
#endif
#ifdef FRAGMENT_CODE_PARAMETERS
	FRAGMENT_CODE_PARAMETERS
#endif

// environment cubemaps. second set of cubemaps is used for reflection probes blending.
SAMPLERCUBE(s_environmentTexture1, 2);
SAMPLERCUBE(s_environmentTextureIBL1, 3);
SAMPLERCUBE(s_environmentTexture2, 4);
SAMPLERCUBE(s_environmentTextureIBL2, 5);

uniform mat3 u_environmentTexture1Rotation;
uniform mat3 u_environmentTexture2Rotation;
uniform vec4 /*vec3*/ u_environmentTexture1Multiplier;
uniform vec4 /*vec3*/ u_environmentTexture2Multiplier;
uniform vec4 u_environmentBlendingFactor;//.x. factor of the environment1 for reflection probes blending.

SAMPLER2D(s_brdfLUT, 6);

#include "PBRFilament\common_types.sh"
#include "PBRFilament\common_math.sh"
#include "PBRFilament\brdf.sh"
#include "PBRFilament\PBRFilament.sh"

#ifdef SHADING_MODEL_SPECULAR
	#include "ShadingModelSpecular.sh"
#endif

#ifdef GLOBAL_LIGHT_MASK_SUPPORT
	#ifdef LIGHT_TYPE_POINT
		SAMPLERCUBE(s_lightMask, 7);
	#elif defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_DIRECTIONAL)
		SAMPLER2D(s_lightMask, 7);
	#endif
#endif

#ifdef SHADOW_MAP
	#ifdef LIGHT_TYPE_POINT
		SAMPLERCUBESHADOW(s_shadowMapShadow, 8);
	#else
		SAMPLER2DARRAYSHADOW(s_shadowMapArrayShadow, 8);
	#endif
#endif

#ifdef SHADOW_MAP
	#include "ShadowReceiverFunctions.sh"
#endif

#ifdef DISPLACEMENT_CODE_SAMPLERS
	DISPLACEMENT_CODE_SAMPLERS
#endif
#ifdef DISPLACEMENT_CODE_SHADER_SCRIPTS
	DISPLACEMENT_CODE_SHADER_SCRIPTS
#endif
#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
	#include "ParallaxOcclusionMapping.sh"
#endif

#ifdef FRAGMENT_CODE_SAMPLERS
	FRAGMENT_CODE_SAMPLERS
#endif
#ifdef FRAGMENT_CODE_SHADER_SCRIPTS
	FRAGMENT_CODE_SHADER_SCRIPTS
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	bool isLayer = u_renderOperationData[3].z != 0;
	
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		smoothLOD(gl_FragCoord, u_renderOperationData[2].w);
	#endif
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)
	if(isLayer)
		smoothLOD(gl_FragCoord, u_renderOperationData[2].w);
	#endif
		
	//get material data
	int materialIndex = (int)u_renderOperationData[0].x;
	for(int n=0;n<MATERIAL_STANDARD_FRAGMENT_SIZE;n++)
		materialStandardFragment[n] = texelFetch(s_materials, ivec2((int)(materialIndex % 64) * 8 + n, (int)(materialIndex / 64)), 0);
	
	vec3 worldPosition = v_worldPosition;
	vec3 inputWorldNormal = normalize(v_worldNormal);

	vec3 toCamera = u_viewportOwnerCameraPosition - worldPosition.xyz;
	float cameraDistance = length(toCamera);
	toCamera = normalize(toCamera);

	//displacement
	vec2 displacementOffset = vec2_splat(0);
#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
	displacementOffset = getParallaxOcclusionMappingOffset(v_texCoord01.xy, v_eyeTangentSpace, v_normalTangentSpace);
#endif //DISPLACEMENT_CODE_BODY
	
	//lightWorldDirection
	vec3 lightWorldDirection;
	#ifdef LIGHT_TYPE_DIRECTIONAL
		lightWorldDirection = -u_lightDirection;
	#else
		lightWorldDirection = normalize(u_lightPosition.xyz - worldPosition);
	#endif

	//objectLightAttenuation
	float objectLightAttenuation = 1;
	#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT)
		float lightDistance = length(u_lightPosition.xyz - worldPosition);
		objectLightAttenuation = getLightAttenuation(u_lightAttenuation, lightDistance);
	#endif
	#ifdef LIGHT_TYPE_SPOTLIGHT
		// factor in spotlight angle
		float rho0 = saturate(dot(-u_lightDirection, lightWorldDirection));
		// factor = (rho - cos(outer/2)) / (cos(inner/2) - cos(outer/2)) ^ falloff 
		float spotFactor0 = saturate(pow(saturate(rho0 - u_lightSpot.y) / (u_lightSpot.x - u_lightSpot.y), u_lightSpot.z));
		objectLightAttenuation *= spotFactor0;
	#endif

	vec3 lightMaskMultiplier = vec3(1,1,1);
	float shadowMultiplier = 1;
	
	//!!!!
	//BRANCH
	//if(objectLightAttenuation != 0 )
	{		
		//lightMaskMultiplier
	#ifdef GLOBAL_LIGHT_MASK_SUPPORT
		{
		#ifdef LIGHT_TYPE_POINT
			vec3 dir = normalize(worldPosition - u_lightPosition.xyz);
			dir = mul(u_lightMaskMatrix, vec4(dir, 0)).xyz;
			//!!!!flipped cubemaps, already applied in lightMaskTextureMatrixArray.
			//dir = float3(-dir.y, dir.z, dir.x);
			lightMaskMultiplier = textureCube(s_lightMask, dir).rgb;
		#elif defined(LIGHT_TYPE_SPOTLIGHT)
			vec4 texCoord = mul( u_lightMaskMatrix, vec4( worldPosition, 1 ) );
			lightMaskMultiplier = texture2DProj( s_lightMask, texCoord ).rgb;
		#elif defined(LIGHT_TYPE_DIRECTIONAL)
			vec2 texCoord = mul( u_lightMaskMatrix, vec4( worldPosition, 1 ) ).xy;
			lightMaskMultiplier = texture2D( s_lightMask, texCoord ).rgb;
		#endif
		}
	#endif

		//shadows
		#ifdef SHADOW_MAP
			shadowMultiplier = getShadowMultiplier(worldPosition, cameraDistance, v_depth, lightWorldDirection, v_worldNormal);
		#endif
	}

	//light color
	vec3 lightColor = vec3(1,1,1);
	#ifndef SHADING_MODEL_UNLIT
		lightColor = u_lightPower.rgb * u_cameraExposure * objectLightAttenuation * lightMaskMultiplier * shadowMultiplier;
	#endif

	//get material parameters
	vec3 normal = vec3_splat(0);
	vec3 baseColor = u_materialBaseColor;
	float opacity = u_materialOpacity;
	float opacityMaskThreshold = u_materialOpacityMaskThreshold;
	float metallic = u_materialMetallic;
	float roughness = u_materialRoughness;
	float reflectance = u_materialReflectance;
	float clearCoat = u_materialClearCoat;
	float clearCoatRoughness = u_materialClearCoatRoughness;
	vec3 clearCoatNormal = vec3_splat(0);
	float anisotropy = u_materialAnisotropy;
	vec3 anisotropyDirection = vec3_splat(0);// = u_materialAnisotropyDirection;
	float anisotropyDirectionBasis = u_materialAnisotropyDirectionBasis;
	float thickness = u_materialThickness;
	float subsurfacePower = u_materialSubsurfacePower;
	vec3 sheenColor = u_materialSheenColor;
	vec3 subsurfaceColor = u_materialSubsurfaceColor;
	float ambientOcclusion = 1;
	float rayTracingReflection = u_materialRayTracingReflection;
	vec3 emissive = u_materialEmissive;
	float shininess = u_materialShininess;

	//get material parameters (procedure generated code)
	vec2 c_texCoord0 = v_texCoord01.xy - displacementOffset;
	vec2 c_texCoord1 = v_texCoord01.zw - displacementOffset;
	vec2 c_texCoord2 = v_texCoord23.xy - displacementOffset;
	vec2 c_texCoord3 = v_texCoord23.zw - displacementOffset;
	vec2 c_unwrappedUV = getUnwrappedUV(c_texCoord0, c_texCoord1, c_texCoord2, c_texCoord3, u_renderOperationData[3].x);
	vec4 c_color0 = v_color0;
#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2D(_sampler, _uv)
	FRAGMENT_CODE_BODY
	#undef CODE_BODY_TEXTURE2D
#endif

	//apply color parameter
	baseColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0)
		baseColor *= v_color0.xyz;
	baseColor = max(baseColor, vec3_splat(0));
	opacity *= v_colorParameter.w;
	if(u_materialUseVertexColor != 0)
		opacity *= v_color0.w;
	opacity = saturate(opacity);

	//opacity dithering
#ifdef OPACITY_DITHERING
	opacity = dither(gl_FragCoord, opacity);
#endif

	//opacity masked clipping
#ifdef BLEND_MODE_MASKED
	clip(opacity - opacityMaskThreshold);
#endif

	vec4 resultColor = vec4_splat(0);

#ifndef SHADING_MODEL_UNLIT

	if(any(anisotropyDirection))
		anisotropyDirection = expand(anisotropyDirection);
	else
   		anisotropyDirection = u_materialAnisotropyDirection;
	anisotropyDirection.z = 0.0;
	anisotropyDirection = normalize(anisotropyDirection);

	//get result world normal
	vec3 tangent = normalize(v_tangent);
	vec3 bitangent = normalize(v_bitangent);

	if(any(normal))
	{
		mat3 tbn = transpose(mat3(tangent, bitangent, inputWorldNormal));
		normal = expand(normal);
		normal.z = sqrt(max(1.0 - dot(normal.xy, normal.xy), 0.0));
		normal = normalize(mul(tbn, normal));
	}
	else
		normal = inputWorldNormal;
#ifdef TWO_SIDED_FLIP_NORMALS
	if(gl_FrontFacing)//if(!gl_FrontFacing)
		normal = -normal;
#endif

	//clearCoatNormal
#ifdef MATERIAL_HAS_CLEAR_COAT
	if(any(clearCoatNormal))
	{
		clearCoatNormal = expand(clearCoatNormal);
		clearCoatNormal.z = sqrt(max(1.0 - dot(clearCoatNormal.xy, clearCoatNormal.xy), 0.0));
	}	
	else
		clearCoatNormal = normal;
#endif
	
	//!!!!
	//BRANCH
	//if(any(lightColor) && ambientOcclusion != 0 )
	{
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

		#ifdef SHADING_MODEL_SPECULAR
			//Specular model

			Material_SpecularSM_Inputs material;

			material.diffuseColor = baseColor;
			material.specularColor = vec3_splat(roughness);
			material.shininess = shininess;//u_materialShininess;
			material.AO = ambientOcclusion;

			prepare_SpecularSM(normal, toLight, toCamera);

			#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT) || defined(LIGHT_TYPE_DIRECTIONAL)
				resultColor.rgb = surfaceShading_SpecularSM(material, lightColor);
			#else				
				EnvironmentTextureData data1;
				data1.rotation = u_environmentTexture1Rotation;
				data1.multiplier = u_environmentTexture1Multiplier;
				
				EnvironmentTextureData data2;
				data2.rotation = u_environmentTexture2Rotation;
				data2.multiplier = u_environmentTexture2Multiplier;
				
				vec3 envColor1 = iblEnvironment_SpecularSM(vec3_splat(0), 0, s_environmentTexture1, data1, lightColor);
				vec3 envColor2 = iblEnvironment_SpecularSM(vec3_splat(0), 0, s_environmentTexture2, data2, lightColor);
				
				//vec3 envColor1 = iblEnvironment_SpecularSM(vec3_splat(0), 0, s_environmentTexture1, lightColor);
				//vec3 envColor2 = iblEnvironment_SpecularSM(vec3_splat(0), 0, s_environmentTexture2, lightColor);

				resultColor.rgb = mix(envColor2, envColor1, u_environmentBlendingFactor.x);
			#endif

		#elif SHADING_MODEL_SIMPLE
			//Simple model
			resultColor.rgb = baseColor * lightColor;			
		#else
			//PBR

			MaterialInputs material;

			material.baseColor        = vec4(baseColor, 0.0);
			material.roughness        = roughness;
			material.metallic         = metallic;
			material.reflectance      = reflectance;
			material.ambientOcclusion = ambientOcclusion;
			//material.emissive         = vec4(emissive, 0.0);

			material.anisotropy = anisotropy * anisotropyDirectionBasis;
			material.anisotropyDirection = anisotropyDirection;

			#if defined(SHADING_MODEL_SUBSURFACE)
				material.thickness = thickness;
				material.subsurfacePower = subsurfacePower;
				material.subsurfaceColor = subsurfaceColor;
			#endif

			material.clearCoat = clearCoat;
			material.clearCoatRoughness = clearCoatRoughness;
			material.clearCoatNormal = clearCoatNormal;

			#if defined(SHADING_MODEL_CLOTH)
				material.sheenColor = sheenColor;

				#if defined(MATERIAL_HAS_SUBSURFACE_COLOR)
					material.subsurfaceColor = subsurfaceColor;
				#endif

			#endif

			setupPBRFilamentParams(material, tangent, bitangent, normal, inputWorldNormal, toLight, toCamera, gl_FrontFacing);

			PixelParams pixel;
			getPBRFilamentPixelParams(material, pixel);

			#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT) || defined(LIGHT_TYPE_DIRECTIONAL)
				resultColor.rgb = surfaceShading(pixel);
			#else
				
				EnvironmentTextureData data1;
				data1.rotation = u_environmentTexture1Rotation;
				data1.multiplier = u_environmentTexture1Multiplier;
				
				EnvironmentTextureData data2;
				data2.rotation = u_environmentTexture2Rotation;
				data2.multiplier = u_environmentTexture2Multiplier;

				vec3 color1 = iblDiffuse(material, pixel, s_environmentTextureIBL1, data1, s_environmentTexture1, data1) + 
					iblSpecular(material, pixel, vec3_splat(0), 0, s_environmentTexture1, data1);
				vec3 color2 = iblDiffuse(material, pixel, s_environmentTextureIBL2, data2, s_environmentTexture2, data2) + 
					iblSpecular(material, pixel, vec3_splat(0), 0, s_environmentTexture2, data2);
			
				//vec3 color1 = iblDiffuse(material, pixel, s_environmentTextureIBL1, s_environmentTexture1) +
				//	iblSpecular(material, pixel, vec3_splat(0), 0, s_environmentTexture1);

				//vec3 color2 = iblDiffuse(material, pixel, s_environmentTextureIBL2, s_environmentTexture2) +
				//	iblSpecular(material, pixel, vec3_splat(0), 0, s_environmentTexture2);
				

				resultColor.rgb = mix(color2, color1, u_environmentBlendingFactor.x);
				//resultColor.rgb = u_envFactor * color1 + (1 - u_envFactor) * color2;
			#endif

			resultColor.rgb *= lightColor;

		#endif //PBR

	}
	
#else //!SHADING_MODEL_UNLIT
	//Unlit
	resultColor.rgb = baseColor;	
#endif
	
	//emissive
	#ifdef LIGHT_TYPE_AMBIENT
		resultColor.rgb += emissive * u_emissiveMaterialsFactor;
	#endif

	//blend mode Add. apply alpha
	#ifdef BLEND_MODE_ADD
		resultColor.rgb *= opacity;
	#endif
	
	//resultColor.a
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		#ifndef LIGHT_TYPE_AMBIENT
			resultColor.a = 1;
		#else
			resultColor.a = 0;
		#endif
	#endif
	#ifdef BLEND_MODE_TRANSPARENT
		resultColor.a = opacity;
	#endif
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)
		if(!isLayer)
		{
			float lodValue = u_renderOperationData[2].w;
			if(lodValue != 0)
			{
				if(lodValue > 0)
					resultColor.a *= 1 - lodValue;
				else
					resultColor.a *= -lodValue;
			}
		}
	#endif

/*
	//_MaterialBlend mask
	#if defined(USAGEMODE_MATERIALBLENDFIRST) || defined(USAGEMODE_MATERIALBLENDNOTFIRST)
		float blendMask;
		#ifdef USAGEMODE_MATERIALBLENDFIRST
			blendMask = 1.0 - materialBlendMask;
		#else
			blendMask = materialBlendMask;
		#endif

		#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
			resultColor.rgb *= blendMask;
		#endif
		#ifdef BLEND_MODE_TRANSPARENT
			//!!!!
		#endif
	#endif
*/

	//fog
#ifdef GLOBAL_FOG_SUPPORT
	//#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
	//	resultColor.rgb *= v_fogFactor;
	//	#ifdef LIGHT_TYPE_AMBIENT
	//		resultColor.rgb += u_fogColor.rgb * (1.0 - v_fogFactor);
	//	#endif
	//#endif
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)
	//#ifdef BLEND_MODE_TRANSPARENT
		resultColor.rgb *= v_fogFactor;
		resultColor.rgb += u_fogColor.rgb * (1.0 - v_fogFactor);
	#endif
#endif

	//debug mode
	BRANCH
	if(u_viewportOwnerDebugMode != DebugMode_None)
	{
		#ifdef LIGHT_TYPE_AMBIENT
			switch(u_viewportOwnerDebugMode)
			{
			case DebugMode_Geometry: resultColor.rgb = vec3_splat(abs(dot(inputWorldNormal, shading_view))); break;
			case DebugMode_Surface: resultColor.rgb = vec3_splat(shading_NoV); break;				
			case DebugMode_BaseColor: resultColor.rgb = baseColor; break;
			case DebugMode_Metallic: resultColor.rgb = vec3_splat(metallic); break;
			case DebugMode_Roughness: resultColor.rgb = vec3_splat(roughness); break;
			case DebugMode_Reflectance: resultColor.rgb = vec3_splat(reflectance); break;
			case DebugMode_Emissive: resultColor.rgb = emissive; break;
			case DebugMode_Normal: resultColor.rgb = normal * 0.5 + 0.5; break;
			}
		#else
			if(u_viewportOwnerDebugMode != DebugMode_Wireframe)
				resultColor = vec4_splat(0);
		#endif
	}

	gl_FragData[0] = resultColor;
	#ifdef LIGHT_TYPE_AMBIENT
		gl_FragData[1] = vec4(normal * 0.5 + 0.5, resultColor.a);
	#else
		gl_FragData[1] = vec4(0,0,0,0);
	#endif
	
	//motion vector
	#ifdef LIGHT_TYPE_AMBIENT
		vec2 aa = (v_position.xy / v_position.w) * 0.5 + 0.5;
		vec2 bb = (v_previousPosition.xy / v_previousPosition.w) * 0.5 + 0.5;
		vec2 velocity = aa - bb;
		gl_FragData[2] = vec4(velocity.x,velocity.y,0,resultColor.a);
	#else
		gl_FragData[2] = vec4(0,0,0,0);
	#endif
}
