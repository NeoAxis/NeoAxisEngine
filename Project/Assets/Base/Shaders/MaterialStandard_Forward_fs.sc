$input v_texCoord01, v_worldPosition_depth, v_worldNormal, v_tangent, v_bitangent, v_fogFactor, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_position, v_previousPosition, v_texCoord23, v_colorParameter, v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles, v_billboardRotation

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define FORWARD 1
#include "Common.sh"
#include "UniformsFragment.sh"
#include "FragmentFunctions.sh"

uniform vec4 u_lightDataFragment[LIGHTDATA_FRAGMENT_SIZE];
uniform vec4 u_renderOperationData[5];
uniform vec4 u_materialCustomParameters[2];

SAMPLER2D(s_materials, 1);
#ifdef GLOBAL_BILLBOARD_DATA
	SAMPLER2DARRAY(s_billboardData, 2);
#endif

#ifdef DISPLACEMENT_CODE_PARAMETERS
	DISPLACEMENT_CODE_PARAMETERS
#endif
#ifdef FRAGMENT_CODE_PARAMETERS
	FRAGMENT_CODE_PARAMETERS
#endif

//environment cubemaps
SAMPLERCUBE(s_environmentTexture1, 3);
SAMPLERCUBE(s_environmentTexture2, 4);

uniform vec4 u_forwardEnvironmentData[5];
#define u_forwardEnvironmentDataRotation1 u_forwardEnvironmentData[0]
#define u_forwardEnvironmentDataMultiplierAndAffect1 u_forwardEnvironmentData[1]
#define u_forwardEnvironmentDataRotation2 u_forwardEnvironmentData[2]
#define u_forwardEnvironmentDataMultiplierAndAffect2 u_forwardEnvironmentData[3]
#define u_forwardEnvironmentDataBlendingFactor u_forwardEnvironmentData[4].x
uniform vec4 u_forwardEnvironmentIrradiance1[9];
uniform vec4 u_forwardEnvironmentIrradiance2[9];

SAMPLER2D(s_colorDepthTextureCopy, 5);
SAMPLER2D(s_brdfLUT, 6);

#include "PBRFilament/common_math.sh"
#include "PBRFilament/brdf.sh"
#include "PBRFilament/PBRFilament.sh"

#ifdef GLOBAL_LIGHT_MASK_SUPPORT
	#ifdef LIGHT_TYPE_POINT
		SAMPLERCUBE(s_lightMask, 7);
	#elif defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_DIRECTIONAL)
		SAMPLER2D(s_lightMask, 7);
	#endif
#endif

#ifdef SHADOW_MAP
	#ifdef LIGHT_TYPE_POINT
		#ifdef SHADOW_TEXTURE_FORMAT_BYTE4 //GLOBAL_SHADOW_TECHNIQUE_EVSM
				SAMPLERCUBE(s_shadowMapShadow, 8);
		#else
				SAMPLERCUBESHADOW(s_shadowMapShadow, 8);
		#endif		
	#else
		//!!!!shadow2DArray works wrong on mobile. no cascades for directional light
		#ifdef GLSL
			#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
				SAMPLER2D(s_shadowMapShadow, 8);
			#else
				SAMPLER2DSHADOW(s_shadowMapShadow, 8);
			#endif
		#else
			#ifdef SHADOW_TEXTURE_FORMAT_BYTE4 //GLOBAL_SHADOW_TECHNIQUE_EVSM
				SAMPLER2DARRAY(s_shadowMapShadow, 8);
			#else
				SAMPLER2DARRAYSHADOW(s_shadowMapShadow, 8);
			#endif
		#endif
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
	vec4 fragCoord = getFragCoord();

	//lod for opaque
#ifdef GLOBAL_SMOOTH_LOD
	float lodValue = v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.x;
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		smoothLOD(fragCoord, lodValue);
	#endif
#endif

	bool isLayer = u_renderOperationData[3].z != 0.0;
	
	//get material data
	vec4 materialStandardFragment[MATERIAL_STANDARD_FRAGMENT_SIZE];
	getMaterialData(s_materials, u_renderOperationData, materialStandardFragment);
	
	vec3 worldPosition = v_worldPosition_depth.xyz;
	MEDIUMP vec3 inputWorldNormal = normalize(v_worldNormal);

	cutVolumes(worldPosition);

	vec3 toCamera = u_viewportOwnerCameraPosition - worldPosition.xyz;
	float cameraDistance = length(toCamera);
	toCamera = normalize(toCamera);

	float billboardDataMode = u_renderOperationData[1].w;
	
	//displacement
	vec2 displacementOffset = vec2_splat(0);
#if defined(DISPLACEMENT_CODE_BODY) && defined(DISPLACEMENT)
	BRANCH
	if(u_displacementScale != 0.0 && billboardDataMode == 0.0)
		displacementOffset = getParallaxOcclusionMappingOffset(v_texCoord01.xy, v_eyeTangentSpace, v_normalTangentSpace, u_materialDisplacementScale * u_displacementScale, u_displacementMaxSteps);
#endif //DISPLACEMENT_CODE_BODY
	
	vec3 lightPositionMinusWorldPosition = u_lightPosition.xyz - worldPosition;
	
	//lightWorldDirection
	MEDIUMP vec3 lightWorldDirection;
	#ifdef LIGHT_TYPE_DIRECTIONAL
		lightWorldDirection = -u_lightDirection;
	#else
		lightWorldDirection = normalize(lightPositionMinusWorldPosition);//u_lightPosition.xyz - worldPosition);
	#endif

	//objectLightAttenuation
	MEDIUMP float objectLightAttenuation = 1.0;
	#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT)
		float lightDistance = length(lightPositionMinusWorldPosition);//u_lightPosition.xyz - worldPosition);
		objectLightAttenuation = getLightAttenuation(u_lightAttenuation, lightDistance);
	#endif
	#ifdef LIGHT_TYPE_SPOTLIGHT
		// factor in spotlight angle
		MEDIUMP float rho0 = saturate(dot(-u_lightDirection, lightWorldDirection));
		// factor = (rho - cos(outer/2)) / (cos(inner/2) - cos(outer/2)) ^ falloff 
		MEDIUMP float spotFactor0 = saturate(pow(saturate(rho0 - u_lightSpot.y) / (u_lightSpot.x - u_lightSpot.y), u_lightSpot.z));
		objectLightAttenuation *= spotFactor0;
	#endif

	MEDIUMP vec3 lightMaskMultiplier = vec3(1,1,1);
	MEDIUMP float shadowMultiplier = 1.0;
	
	//!!!!
	//BRANCH
	//if(objectLightAttenuation != 0 )
	{		
		//lightMaskMultiplier
	#ifdef GLOBAL_LIGHT_MASK_SUPPORT
		{
		#ifdef LIGHT_TYPE_POINT
			vec3 dir = normalize(-lightPositionMinusWorldPosition);//worldPosition - u_lightPosition.xyz);
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
			shadowMultiplier = getShadowMultiplier(worldPosition, cameraDistance, v_worldPosition_depth.w, lightWorldDirection, v_worldNormal, vec2_splat(0.0), fragCoord);
		#endif
	}

	//light color
	MEDIUMP vec3 lightColor = vec3(1,1,1);
	#ifndef SHADING_MODEL_UNLIT
		lightColor = u_lightPower.rgb * u_cameraExposure * objectLightAttenuation * lightMaskMultiplier * shadowMultiplier;
	#endif

	//get material parameters
	MEDIUMP vec3 normal = vec3_splat(0);
	MEDIUMP vec3 baseColor = u_materialBaseColor;
	MEDIUMP float opacity = u_materialOpacity;
	MEDIUMP float opacityMaskThreshold = u_materialOpacityMaskThreshold;
	MEDIUMP float metallic = u_materialMetallic;
	MEDIUMP float roughness = u_materialRoughness;
	MEDIUMP float reflectance = u_materialReflectance;
	MEDIUMP float clearCoat = u_materialClearCoat;
	MEDIUMP float clearCoatRoughness = u_materialClearCoatRoughness;
	MEDIUMP vec3 clearCoatNormal = vec3_splat(0);
	MEDIUMP float anisotropy = u_materialAnisotropy;
	MEDIUMP vec3 anisotropyDirection = vec3_splat(0);// = u_materialAnisotropyDirection;
	MEDIUMP float anisotropyDirectionBasis = u_materialAnisotropyDirectionBasis;
	MEDIUMP float thickness = u_materialThickness;
	MEDIUMP float subsurfacePower = u_materialSubsurfacePower;
	MEDIUMP vec3 sheenColor = u_materialSheenColor;
	MEDIUMP vec3 subsurfaceColor = u_materialSubsurfaceColor;
	MEDIUMP float ambientOcclusion = 1.0;
	MEDIUMP float rayTracingReflection = u_materialRayTracingReflection;
	MEDIUMP vec3 emissive = u_materialEmissive;
	float softParticlesDistance = u_materialSoftParticlesDistance;
	vec4 customParameter1 = u_materialCustomParameters[0];
	vec4 customParameter2 = u_materialCustomParameters[1];

	//get material parameters (procedure generated code)
	MEDIUMP vec2 texCoord0 = v_texCoord01.xy - displacementOffset;
	MEDIUMP vec2 texCoord1 = v_texCoord01.zw - displacementOffset;
	MEDIUMP vec2 texCoord2 = v_texCoord23.xy - displacementOffset;
	//vec2 texCoord3 = v_texCoord23.zw - displacementOffset;
	
	//billboard with geometry data mode
#ifdef GLOBAL_BILLBOARD_DATA
	billboardDataModeCalculateParameters(billboardDataMode, s_billboardData, fragCoord, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles, v_billboardRotation, inputWorldNormal, texCoord0, texCoord1, texCoord2);
#endif
	
	MEDIUMP vec2 unwrappedUV = getUnwrappedUV(texCoord0, texCoord1, texCoord2/*, texCoord3*/, u_renderOperationData[3].x);
	MEDIUMP vec4 color0 = v_color0;
#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(_sampler, _uv, u_renderOperationData[3].z, 0/*gl_PrimitiveID*/)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(_sampler, _uv, u_removeTextureTiling)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(_sampler, _uv, u_mipBias)
	FRAGMENT_CODE_BODY
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	//apply color parameter

	baseColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0.0)
		baseColor *= color0.xyz;
	baseColor = max(baseColor, vec3_splat(0));
	
	opacity *= v_colorParameter.w;
	if(u_materialUseVertexColor != 0.0)
		opacity *= color0.w;
	opacity = saturate(opacity);
	
#ifdef SHADING_MODEL_CLOTH
	sheenColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0.0)
		sheenColor *= color0.xyz;
	sheenColor = max(sheenColor, vec3_splat(0));
#endif	

#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_CLOTH)
	subsurfaceColor *= v_colorParameter.xyz;
	if(u_materialUseVertexColor != 0.0)
		subsurfaceColor *= color0.xyz;
	subsurfaceColor = max(subsurfaceColor, vec3_splat(0));	
#endif

	//opacity dithering
#ifdef OPACITY_DITHERING
	opacity = dither(fragCoord, opacity);
#endif

	//opacity masked clipping
#ifdef BLEND_MODE_MASKED
	if(billboardDataMode == 0.0)
		clip(opacity - opacityMaskThreshold);
#endif

	MEDIUMP vec4 resultColor = vec4_splat(0);

#ifndef SHADING_MODEL_UNLIT

	if(any2(anisotropyDirection))
		anisotropyDirection = expand(anisotropyDirection);
	else
   		anisotropyDirection = u_materialAnisotropyDirection;
	anisotropyDirection.z = 0.0;
	anisotropyDirection = normalize(anisotropyDirection);

	//get result world normal
	MEDIUMP vec3 tangent = normalize(v_tangent);
	MEDIUMP vec3 bitangent = normalize(v_bitangent);

#if defined(GLOBAL_NORMAL_MAPPING) && GLOBAL_MATERIAL_SHADING != GLOBAL_MATERIAL_SHADING_SIMPLE
	if(any2(normal) && billboardDataMode == 0.0)
	{
		mat3 tbn = transpose(mtxFromRows(tangent, bitangent, inputWorldNormal));
		normal = expand(normal);
		normal.z = sqrt(max(1.0 - dot(normal.xy, normal.xy), 0.0));
		normal = normalize(mul(tbn, normal));
	}
	else
		normal = inputWorldNormal;
#else
		normal = inputWorldNormal;
#endif
#ifdef TWO_SIDED_FLIP_NORMALS
	if(gl_FrontFacing)//if(!gl_FrontFacing)
		normal = -normal;
#endif

	//clearCoatNormal
#ifdef MATERIAL_HAS_CLEAR_COAT
	if(any2(clearCoatNormal))
	{
		clearCoatNormal = expand(clearCoatNormal);
		clearCoatNormal.z = sqrt(max(1.0 - dot(clearCoatNormal.xy, clearCoatNormal.xy), 0.0));
	}	
	else
		clearCoatNormal = normal;
#endif
	
	//!!!!
	//BRANCH
	//if(any2(lightColor))
	{
		//toLight
		vec3 toLight;
		#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT)
			toLight = lightPositionMinusWorldPosition;//u_lightPosition.xyz - worldPosition;
		#elif LIGHT_TYPE_DIRECTIONAL
			toLight = -u_lightDirection;
		#else
			toLight = normal; // For ambient light dot(N, L) == 1, i.e. we can do L = N;
		#endif
		toLight = normalize(toLight);

		#ifdef SHADING_MODEL_SIMPLE
			//Simple model
			resultColor.rgb = baseColor * lightColor;
		#else
			//PBR

#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE
			resultColor.rgb = baseColor * lightColor;
			#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT) || defined(LIGHT_TYPE_DIRECTIONAL)
				resultColor.rgb *= saturate(dot(normal, lightWorldDirection) / PI);
			#endif
#else
		
			MaterialInputs material;

			material.baseColor = vec4(baseColor, 0.0);
			material.roughness = roughness;
			material.metallic = metallic;
			material.reflectance = reflectance;
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
				material.subsurfaceColor = subsurfaceColor;
			#endif

			setupPBRFilamentParams(material, tangent, bitangent, normal, inputWorldNormal, toLight, toCamera, gl_FrontFacing);

			PixelParams pixel;
			getPBRFilamentPixelParams(material, pixel);

			#if defined(LIGHT_TYPE_SPOTLIGHT) || defined(LIGHT_TYPE_POINT) || defined(LIGHT_TYPE_DIRECTIONAL)
				resultColor.rgb = surfaceShading(pixel);
			#else

				EnvironmentTextureData data1;
				data1.rotation = u_forwardEnvironmentDataRotation1;
				data1.multiplierAndAffect = u_forwardEnvironmentDataMultiplierAndAffect1;
				
				EnvironmentTextureData data2;
				data2.rotation = u_forwardEnvironmentDataRotation2;
				data2.multiplierAndAffect = u_forwardEnvironmentDataMultiplierAndAffect2;

				vec3 color1 = iblDiffuse(material, pixel, u_forwardEnvironmentIrradiance1, data1, s_environmentTexture1, data1, true) + 
					iblSpecular(material, pixel, vec3_splat(0), 0.0, s_environmentTexture1, data1);
				vec3 color2 = iblDiffuse(material, pixel, u_forwardEnvironmentIrradiance2, data2, s_environmentTexture2, data2, true) + 
					iblSpecular(material, pixel, vec3_splat(0), 0.0, s_environmentTexture2, data2);
			
				resultColor.rgb = mix(color2, color1, u_forwardEnvironmentDataBlendingFactor);
				//resultColor.rgb = u_envFactor * color1 + (1 - u_envFactor) * color2;
				
			#endif
			
			resultColor.rgb *= lightColor;
#endif
			
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
			resultColor.a = 1.0;
		#else
			resultColor.a = 0.0;
		#endif
	#endif
	#ifdef BLEND_MODE_TRANSPARENT
		resultColor.a = opacity;
	#endif
	
	//lod for transparent
#ifdef GLOBAL_SMOOTH_LOD
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)
		if(isLayer)
			smoothLOD(fragCoord, lodValue);
		else
		{
			if(lodValue != 0.0)
			{
				MEDIUMP float factor;
				if(lodValue > 0.0)
					factor = 1.0 - lodValue;
				else
					factor = -lodValue;

				#if defined(BLEND_MODE_TRANSPARENT)
					resultColor.a *= factor;
				#endif
				#if defined(BLEND_MODE_ADD)	
					resultColor.rgb *= factor;
				#endif
			}
		}
	#endif
#endif

	//fading by visibility distance
#ifdef GLOBAL_FADE_BY_VISIBILITY_DISTANCE
	float visibilityDistance = v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y;
	MEDIUMP float visibilityDistanceFactor = getVisibilityDistanceFactor(visibilityDistance, cameraDistance);
	#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
		smoothLOD(fragCoord, 1.0f - visibilityDistanceFactor);
	#endif
	#if defined(BLEND_MODE_TRANSPARENT)
		resultColor.a *= visibilityDistanceFactor;
	#endif
	#if defined(BLEND_MODE_ADD)	
		resultColor.rgb *= visibilityDistanceFactor;
	#endif
#endif

	//soft particles
#ifdef SOFT_PARTICLES
	BRANCH
	if(u_provideColorDepthTextureCopy > 0.0)
	{
		//get depth
		MEDIUMP vec2 texCoord = fragCoord.xy * u_viewportSizeInv;
		//float rawDepth = texture2D(s_colorDepthTextureCopy, texCoord).g;
		float rawDepth = texture2D(s_colorDepthTextureCopy, texCoord).w;
		float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);

		MEDIUMP float softParticlesFactor = saturate((depth - v_worldPosition_depth.w) / softParticlesDistance);
		
		#if defined(BLEND_MODE_OPAQUE) || defined(BLEND_MODE_MASKED)
			smoothLOD(fragCoord, 1.0f - softParticlesFactor);
		#endif
		#if defined(BLEND_MODE_TRANSPARENT)
			resultColor.a *= softParticlesFactor;
		#endif
		#if defined(BLEND_MODE_ADD)	
			resultColor.rgb *= softParticlesFactor;
		#endif
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
#ifdef GLOBAL_FOG
	#ifdef BLEND_MODE_TRANSPARENT
		resultColor *= v_fogFactor;
		resultColor.rgb += u_fogColor.rgb * (1.0 - v_fogFactor);
	#endif
	#ifdef BLEND_MODE_ADD
		resultColor *= v_fogFactor;
	#endif
#endif

	//debug mode
#ifdef GLOBAL_DEBUG_MODE
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
			case DebugMode_Normal: resultColor.rgb = inputWorldNormal * 0.5 + 0.5; break;//resultColor.rgb = normal * 0.5 + 0.5; break;
			case DebugMode_SubsurfaceColor: resultColor.rgb = subsurfaceColor; break;
			case DebugMode_TextureCoordinate0: resultColor.rgb = vec3(v_texCoord01.xy, 0); break;
			case DebugMode_TextureCoordinate1: resultColor.rgb = vec3(v_texCoord01.zw, 0); break;
			case DebugMode_TextureCoordinate2: resultColor.rgb = vec3(v_texCoord23.xy, 0); break;
			}
		#else
			if(u_viewportOwnerDebugMode != DebugMode_Wireframe)
				resultColor = vec4_splat(0);
		#endif
	}
#endif

	gl_FragData[0] = resultColor;
#ifndef MOBILE
	#ifdef LIGHT_TYPE_AMBIENT
		gl_FragData[1] = vec4(normal * 0.5 + 0.5, resultColor.a);
	#else
		gl_FragData[1] = vec4(0,0,0,0);
	#endif
#endif
	
	//motion vector
#ifdef GLOBAL_MOTION_VECTOR
	#ifdef LIGHT_TYPE_AMBIENT
		vec2 aa = (v_position.xy / v_position.w) * 0.5 + 0.5;
		vec2 bb = (v_previousPosition.xy / v_previousPosition.w) * 0.5 + 0.5;
		vec2 velocity = (aa - bb) * v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.w;
		gl_FragData[2] = vec4(velocity.x,velocity.y,0.0,resultColor.a);
	#else
		gl_FragData[2] = vec4(0,0,0,0);
	#endif
#endif
	
}