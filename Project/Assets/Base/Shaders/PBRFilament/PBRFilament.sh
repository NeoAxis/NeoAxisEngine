//#define GEOMETRIC_SPECULAR_AA_ROUGHNESS //not best on car wheels

#define MATERIAL_HAS_AMBIENT_OCCLUSION

#define MIN_ROUGHNESS              0.045
#define MIN_LINEAR_ROUGHNESS       0.002025
#define MAX_CLEAR_COAT_ROUGHNESS   0.6
#define MIN_N_DOT_V                1e-4

#define IBL_SPECULAR_OCCLUSION
#define IBL_OFF_SPECULAR_PEAK

#define IBL_LUMINANCE 1.0

//#define USE_MULTIPLE_SCATTERING_COMPENSATION 1

#define CLOTH_DFG                  CLOTH_DFG_CHARLIE

struct MaterialInputs {
	MEDIUMP vec4  baseColor;
	MEDIUMP float roughness;
	MEDIUMP float metallic;
	MEDIUMP float reflectance;
	MEDIUMP float ambientOcclusion;

#ifdef MATERIAL_HAS_ANISOTROPY
	MEDIUMP float anisotropy;
	MEDIUMP vec3 anisotropyDirection;
#endif

#ifdef MATERIAL_HAS_CLEAR_COAT
	MEDIUMP float clearCoat;
	MEDIUMP float clearCoatRoughness;
	MEDIUMP vec3 clearCoatNormal;
#endif	

#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
	MEDIUMP float thickness;
	MEDIUMP float subsurfacePower;
	MEDIUMP vec3 subsurfaceColor;
#endif

#if defined(SHADING_MODEL_CLOTH)
	MEDIUMP vec3 sheenColor;
	MEDIUMP vec3 subsurfaceColor;
#endif

};

struct PixelParams {
	MEDIUMP vec3 diffuseColor;
	MEDIUMP float roughness;
	MEDIUMP vec3 f0;
	MEDIUMP float linearRoughness;
	MEDIUMP vec3 dfg;
	MEDIUMP vec3 energyCompensation;

#ifdef MATERIAL_HAS_CLEAR_COAT
	MEDIUMP float clearCoat;
	MEDIUMP float clearCoatRoughness;
	MEDIUMP float clearCoatLinearRoughness;
#endif

#ifdef MATERIAL_HAS_ANISOTROPY
	MEDIUMP float anisotropy;
	MEDIUMP vec3 anisotropicT;
	MEDIUMP vec3 anisotropicB;
#endif	

#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
	MEDIUMP float thickness;
	MEDIUMP vec3 subsurfaceColor;
	MEDIUMP float subsurfacePower;
#endif

#if defined(SHADING_MODEL_CLOTH)
	MEDIUMP vec3 subsurfaceColor;
#endif
};

struct ShadingParams {
	HIGHP mat3 tangentToWorld;
	MEDIUMP vec3 normal;
	//vec3 input_normal;
#ifdef MATERIAL_HAS_CLEAR_COAT
	MEDIUMP vec3 clearCoatNormal;
#endif	
	MEDIUMP vec3 view;
	MEDIUMP vec3 L;
	MEDIUMP vec3 H;
	MEDIUMP vec3 reflected;
	MEDIUMP float NoV;// dot(normal, view), always strictly >= MIN_N_DOT_V
	MEDIUMP float NoL;
	MEDIUMP float NoH;
	MEDIUMP float LoH;
	MEDIUMP float ToL;
};

MEDIUMP float clampNoV(float NoV)
{
	// Neubelt and Pettineo 2013, "Crafting a Next-gen Material Pipeline for The Order: 1886"
	return max(NoV, MIN_N_DOT_V);
}

MEDIUMP vec3 PrefilteredDFG_LUT(float roughness, float NoV)
{
	return texture2DLod(s_brdfLUT, vec2(NoV, 1.0-roughness), 0.0).rgb;
}

MEDIUMP vec3 PrefilteredDFG(float roughness, float NoV)
{
	// PrefilteredDFG_LUT() takes a coordinate, which is sqrt(linear_roughness) = roughness
	return PrefilteredDFG_LUT(roughness, NoV);
}

MEDIUMP vec3 diffuseIrradiance(const vec3 n, vec4 environmentIrradiance[9], EnvironmentTextureData environmentTextureIBLData)
{
	return getEnvironmentValue(environmentIrradiance, environmentTextureIBLData, n);	
}

MEDIUMP vec3 diffuseIrradianceTexture( const vec3 n, samplerCube environmentTextureIBL, EnvironmentTextureData environmentTextureIBLData, float lod )
{
	return getEnvironmentValueLod( environmentTextureIBL, environmentTextureIBLData, n, lod );
	//return getEnvironmentValueTexture(environmentTextureIBL, environmentTextureIBLData, n);
}

MEDIUMP vec3 specularIrradiance(const vec3 r, float roughness, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	MEDIUMP float lod;
#ifdef GLSL
	lod = pow(float(textureSize(environmentTexture, 0)), 0.5) * roughness;
	//lod = float(textureQueryLevels(environmentTexture.m_texture)) * roughness;
	//lod = 8.0 * roughness;
#else
	vec3 texDim = vec3(0.0, 0.0, 0.0);
	environmentTexture.m_texture.GetDimensions(0, texDim.x, texDim.y, texDim.z);
	lod = texDim.z * roughness;
#endif	
	return getEnvironmentValueLod(environmentTexture, environmentTextureData, r, lod);
}

MEDIUMP vec3 specularIrradiance_Offset(const vec3 r, float roughness, float offset, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	MEDIUMP float lod;
#ifdef GLSL
	lod = pow(float(textureSize(environmentTexture, 0)), 0.5) * roughness;
	//lod = float(textureQueryLevels(environmentTexture.m_texture)) * roughness * roughness;
	//lod = 8.0 * roughness * roughness;
#else
	vec3 texDim = vec3(0.0, 0.0, 0.0);
	environmentTexture.m_texture.GetDimensions(0, texDim.x, texDim.y, texDim.z);
	lod = texDim.z * roughness * roughness;
#endif
	return getEnvironmentValueLod(environmentTexture, environmentTextureData, r, lod + offset);
}

MEDIUMP vec3 getSpecularDominantDirection(vec3 n, vec3 r, float linearRoughness)
{
#if defined(IBL_OFF_SPECULAR_PEAK)
	MEDIUMP float s = 1.0 - linearRoughness;
	return mix(n, r, s * (sqrt(s) + linearRoughness));
#else
	return r;
#endif
}

MEDIUMP vec3 specularDFG(const PixelParams pixel)
{
#if defined(SHADING_MODEL_CLOTH)
	return pixel.f0 * pixel.dfg.z;
#elif !defined(USE_MULTIPLE_SCATTERING_COMPENSATION)
	return pixel.f0 * pixel.dfg.x + pixel.dfg.y;
#else
	return mix(pixel.dfg.xxx, pixel.dfg.yyy, pixel.f0);
#endif
}

MEDIUMP float computeSpecularAO(float NoV, float ao, float roughness)
{
#if defined(IBL_SPECULAR_OCCLUSION) && defined(MATERIAL_HAS_AMBIENT_OCCLUSION)
	return saturate(pow(NoV + ao, exp2(-16.0 * roughness - 1.0)) - 1.0 + ao);
#else
	return 1.0;
#endif
}

MEDIUMP vec3 getReflectedVectorExt(const PixelParams pixel, const vec3 v, const vec3 n)
{
	MEDIUMP vec3 r;
#if defined(MATERIAL_HAS_ANISOTROPY)
	MEDIUMP vec3 anisotropyDirection = pixel.anisotropy >= 0.0 ? pixel.anisotropicB : pixel.anisotropicT;
	MEDIUMP vec3 anisotropicTangent = cross(anisotropyDirection, v);
	MEDIUMP vec3 anisotropicNormal = cross(anisotropicTangent, anisotropyDirection);
	MEDIUMP float bendFactor = abs(pixel.anisotropy) * saturate(5.0 * pixel.roughness);
	MEDIUMP vec3  bentNormal = normalize(mix(n, anisotropicNormal, bendFactor));
	r = reflect(-v, bentNormal);
#else
	r = reflect(-v, n);
#endif
	return r;
}

MEDIUMP vec3 getReflectedVector(const PixelParams pixel, const ShadingParams shading, const vec3 n)
{
	MEDIUMP vec3 r;
#if defined(MATERIAL_HAS_ANISOTROPY)
	r = getReflectedVectorExt(pixel, shading.view, n);
#else
	r = shading.reflected;
#endif
	return getSpecularDominantDirection(n, r, pixel.linearRoughness);
}

void evaluateClothIndirectDiffuseBRDF(const PixelParams pixel, const ShadingParams shading, inout float diffuse)
{
#if defined(SHADING_MODEL_CLOTH)
	// Simulate subsurface scattering with a wrap diffuse term
	diffuse *= Fd_Wrap(shading.NoV, 0.5);
#endif
}

#ifdef MATERIAL_HAS_CLEAR_COAT
void iblClearCoatDiffuse(const PixelParams pixel, const ShadingParams shading, inout vec3 Fd)
{
	MEDIUMP float clearCoatNoV = clampNoV(dot(shading.clearCoatNormal, shading.view));
	// The clear coat layer assumes an IOR of 1.5 (4% reflectance)
	MEDIUMP float Fc = F_Schlick(0.04, 1.0, clearCoatNoV) * pixel.clearCoat;
	MEDIUMP float attenuation = 1.0 - Fc;
	Fd *= attenuation;
}

void iblClearCoatSpecular(const PixelParams pixel, const ShadingParams shading, float specularAO, inout vec3 Fr, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData, vec3 replaceSpecularIrradianceValue, float replaceSpecularIrradianceFactor)
{
	MEDIUMP float clearCoatNoV = clampNoV(dot(shading.clearCoatNormal, shading.view));
	MEDIUMP vec3 clearCoatR = reflect(-shading.view, shading.clearCoatNormal);
	MEDIUMP float Fc = F_Schlick(0.04, 1.0, clearCoatNoV) * pixel.clearCoat;
	MEDIUMP float attenuation = 1.0 - Fc;
	Fr *= sq(attenuation);	
	Fr += mix( specularIrradiance( clearCoatR, pixel.clearCoatRoughness, environmentTexture, environmentTextureData ), replaceSpecularIrradianceValue, replaceSpecularIrradianceFactor ) * ( specularAO * Fc );	
	//Fr += specularIrradiance(clearCoatR, pixel.clearCoatRoughness, environmentTexture, environmentTextureData) * (specularAO * Fc);
}
#endif

#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
void iblSubsurfaceDiffuse(const PixelParams pixel, const ShadingParams shading, const vec3 diffuseIrradiance, inout vec3 Fd, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	MEDIUMP vec3 viewIndependent = diffuseIrradiance;
	MEDIUMP vec3 viewDependent = specularIrradiance_Offset(-shading.view, pixel.roughness, 1.0 + pixel.thickness, environmentTexture, environmentTextureData);
	MEDIUMP float attenuation = (1.0 - pixel.thickness) / (2.0 * PI);
	Fd += pixel.subsurfaceColor * (viewIndependent + viewDependent) * attenuation;
}
#endif

#if defined(SHADING_MODEL_CLOTH)
void iblClothDiffuse(const PixelParams pixel, const ShadingParams shading, const vec3 diffuseIrradiance, inout vec3 Fd, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	Fd *= saturate(pixel.subsurfaceColor + shading.NoV);
}
#endif

/*
void iblSubsurfaceDiffuse(const PixelParams pixel, const vec3 diffuseIrradiance, inout vec3 Fd, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
#if defined(SHADING_MODEL_SUBSURFACE)
	vec3 viewIndependent = diffuseIrradiance;
	vec3 viewDependent = specularIrradiance_Offset(-shading.view, pixel.roughness, 1.0 + pixel.thickness, environmentTexture, environmentTextureData);
	float attenuation = (1.0 - pixel.thickness) / (2.0 * PI);
	Fd += pixel.subsurfaceColor * (viewIndependent + viewDependent) * attenuation;
#elif defined(SHADING_MODEL_CLOTH)
	Fd *= saturate(pixel.subsurfaceColor + shading.NoV);
#endif
}
*/

MEDIUMP vec3 iblDiffuse(const MaterialInputs material, const PixelParams pixel, const ShadingParams shading, vec4 environmentIrradiance[9], EnvironmentTextureData environmentTextureIBLData, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData, bool shadingModelSubsurface, bool shadingModelFoliage)
//vec3 iblDiffuse(const MaterialInputs material, const PixelParams pixel, samplerCube environmentTextureIBL, EnvironmentTextureData environmentTextureIBLData, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData, bool shadingModelSubsurface)
{
	MEDIUMP vec3 n = shading.normal;

	MEDIUMP float ao = material.ambientOcclusion;

	// diffuse indirect
	MEDIUMP float diffuseBRDF = ao; // Fd_Lambert() is baked in the SH below
	//float diffuseBRDF = Fd_Lambert() * ao;
	evaluateClothIndirectDiffuseBRDF(pixel, shading, diffuseBRDF);

	MEDIUMP vec3 diffuseIrr;
	BRANCH
	if( environmentIrradiance[ 0 ].x < -9999.0 )
		diffuseIrr = diffuseIrradianceTexture( n, environmentTexture, environmentTextureData, environmentIrradiance[ 0 ].y );
	else
		diffuseIrr = diffuseIrradiance( n, environmentIrradiance, environmentTextureIBLData );
		
	//MEDIUMP vec3 diffuseIrr = diffuseIrradiance( n, environmentIrradiance, environmentTextureIBLData );
	//diffuseIrr *= diffuseIrradianceTexture( n, environmentTexture, environmentTextureData );	
	////vec3 diffuseIrr = diffuseIrradiance(n, environmentTextureIBL, environmentTextureIBLData);
	
	MEDIUMP vec3 Fd = pixel.diffuseColor * diffuseIrr * diffuseBRDF;

#ifdef MATERIAL_HAS_CLEAR_COAT
	iblClearCoatDiffuse(pixel, shading, Fd);
#endif

#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
	BRANCH
	if(shadingModelSubsurface || shadingModelFoliage)
		iblSubsurfaceDiffuse(pixel, shading, diffuseIrr, Fd, environmentTexture, environmentTextureData);
#endif

#if defined(SHADING_MODEL_CLOTH)
	iblClothDiffuse(pixel, shading, diffuseIrr, Fd, environmentTexture, environmentTextureData);
#endif

	return Fd * IBL_LUMINANCE;
}

MEDIUMP vec3 getSpecularIrradiance(const PixelParams pixel, const ShadingParams shading, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	MEDIUMP vec3 n = shading.normal;
	MEDIUMP vec3 r = getReflectedVector(pixel, shading, n);
	MEDIUMP vec3 specIrradiance = specularIrradiance(r, pixel.roughness, environmentTexture, environmentTextureData);
	return specIrradiance;
}

MEDIUMP vec3 iblSpecular(const MaterialInputs material, const PixelParams pixel, const ShadingParams shading, vec3 replaceSpecularIrradianceValue, float replaceSpecularIrradianceFactor, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	MEDIUMP vec3 n = shading.normal;
	MEDIUMP vec3 r = getReflectedVector(pixel, shading, n);

	MEDIUMP float ao = material.ambientOcclusion;
	MEDIUMP float specularAO = computeSpecularAO(shading.NoV, ao, pixel.roughness);

	// specular indirect
	MEDIUMP vec3 specIrradiance = mix(specularIrradiance(r, pixel.roughness, environmentTexture, environmentTextureData),
		replaceSpecularIrradianceValue, replaceSpecularIrradianceFactor);

	MEDIUMP vec3 Fr = specularDFG(pixel) * specIrradiance * specularAO;
	Fr *= pixel.energyCompensation;

#ifdef MATERIAL_HAS_CLEAR_COAT
	iblClearCoatSpecular( pixel, shading, specularAO, Fr, environmentTexture, environmentTextureData, replaceSpecularIrradianceValue, replaceSpecularIrradianceFactor );
#endif
	
	return Fr * IBL_LUMINANCE;

	//return vec3(prefilteredDFG_LUT(pixel.roughness, shading.NoV), 0.0);
}

void getPBRFilamentShadingParams(const MaterialInputs material, vec3 tangent, vec3 bitangent, vec3 normal, vec3 inputNormal, vec3 toLight, vec3 toCamera, bool frontFacing, out ShadingParams shading)//, bool specularOnly = false)
{
	shading.normal = normal;
	//shading.input_normal = inputNormal;

	shading.view = toCamera;

	//if (!specularOnly)
	//{
	shading.L = toLight;
	shading.H = normalize(shading.view + shading.L);

	shading.NoL = saturate(dot(shading.normal, shading.L));
	shading.NoH = saturate(dot(shading.normal, shading.H));
	shading.LoH = saturate(dot(shading.L, shading.H));
	//}

	shading.ToL = saturate(dot(normal, shading.L));

	shading.tangentToWorld = transpose(mtxFromRows(tangent, bitangent, normal));

	shading.NoV = clampNoV(dot(shading.normal, shading.view));
	shading.reflected = reflect(-shading.view, normal);

#ifdef MATERIAL_HAS_CLEAR_COAT
	mat3 tangentToWorld_ClearCoat = transpose(mtxFromRows(tangent, bitangent, inputNormal));
	shading.clearCoatNormal = normalize(mul(tangentToWorld_ClearCoat, material.clearCoatNormal));
	#ifdef TWO_SIDED_FLIP_NORMALS
		if(frontFacing)
			shading.clearCoatNormal = -shading.clearCoatNormal;
	#endif	
//#else
//	shading.clearCoatNormal = vec3_splat(0);
#endif
	
}

void getPBRFilamentPixelParams(const MaterialInputs material, const ShadingParams shading, out PixelParams pixel)
{
	MEDIUMP vec4 baseColor = material.baseColor;

#if !defined(SHADING_MODEL_CLOTH)
	MEDIUMP float metallic = material.metallic;
	MEDIUMP float reflectance = material.reflectance;

	pixel.diffuseColor = (1.0 - metallic) * baseColor.rgb;
	// Assumes an interface from air to an IOR of 1.5 for dielectrics
	pixel.f0 = 0.16 * reflectance * reflectance * (1.0 - metallic) + baseColor.rgb * metallic;
#else
	pixel.diffuseColor = baseColor.rgb;
	pixel.f0 = material.sheenColor;
	pixel.subsurfaceColor = material.subsurfaceColor;
#endif

	// Clamp the roughness to a minimum value to avoid divisions by 0 in the
	// lighting code
	MEDIUMP float roughness = material.roughness;
	roughness = clamp(roughness, MIN_ROUGHNESS, 1.0);

//#if defined(GEOMETRIC_SPECULAR_AA_ROUGHNESS)
//	// Increase the roughness based on the curvature of the geometry to reduce
//	// shading aliasing. The curvature is approximated using the derivatives
//	// of the geometric normal
//	vec3 ndFdx = dFdx(shading.normal);
//	vec3 ndFdy = dFdy(shading.normal);
//	float geometricRoughness = pow(saturate(max(dot(ndFdx, ndFdx), dot(ndFdy, ndFdy))), 0.333);
//	roughness = max(roughness, geometricRoughness);
//#endif

#ifdef MATERIAL_HAS_CLEAR_COAT
	pixel.clearCoat = material.clearCoat;

	// Clamp the clear coat roughness to avoid divisions by 0
	MEDIUMP float clearCoatRoughness = material.clearCoatRoughness;
	clearCoatRoughness = mix(MIN_ROUGHNESS, MAX_CLEAR_COAT_ROUGHNESS, clearCoatRoughness);

//#if defined(GEOMETRIC_SPECULAR_AA_ROUGHNESS)
//	clearCoatRoughness = max(clearCoatRoughness, geometricRoughness);
//#endif

	// Remap the roughness to perceptually linear roughness
	pixel.clearCoatRoughness = clearCoatRoughness;
	pixel.clearCoatLinearRoughness = clearCoatRoughness * clearCoatRoughness;
#endif	

	//!!!!is not used
//#if defined(MATERIAL_HAS_CLEAR_COAT_ROUGHNESS)
//	// This is a hack but it will do: the base layer must be at least as rough
//	// as the clear coat layer to take into account possible diffusion by the
//	// top layer
//	roughness = max(roughness, pixel.clearCoatRoughness);
//#endif

#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
	pixel.subsurfacePower = material.subsurfacePower;
	pixel.subsurfaceColor = material.subsurfaceColor;
	pixel.thickness = saturate(material.thickness);
#endif

#ifdef MATERIAL_HAS_ANISOTROPY
	pixel.anisotropy = material.anisotropy;
//	pixel.anisotropicT = vec3_splat(0);
//	pixel.anisotropicB = vec3_splat(0);
//#if defined(MATERIAL_HAS_ANISOTROPY)
	pixel.anisotropicT = normalize(mul(shading.tangentToWorld, material.anisotropyDirection));
	pixel.anisotropicB = normalize(cross(shading.normal, pixel.anisotropicT));
#endif

	// Remaps the roughnes to a perceptually linear roughness (roughness^2)
	// TODO: the base layer's roughness should not be higher than the clear coat layer's
	pixel.roughness = roughness;
	pixel.linearRoughness = roughness * roughness;

	// Pre-filtered DFG term used for image-based lighting
	pixel.dfg = PrefilteredDFG(pixel.roughness, shading.NoV);

#if defined(USE_MULTIPLE_SCATTERING_COMPENSATION) && !defined(SHADING_MODEL_CLOTH)
	// Energy compensation for multiple scattering in a microfacet model
	// See "Multiple-Scattering Microfacet BSDFs with the Smith Model"
	pixel.energyCompensation = 1.0 + pixel.f0 * (1.0 / pixel.dfg.y - 1.0);
#else
	pixel.energyCompensation = vec3_splat(1);
#endif

}

MEDIUMP vec3 isotropicLobe(const PixelParams pixel, const ShadingParams shading)
{
	MEDIUMP float D = distribution(pixel.linearRoughness, shading.NoH, shading.H);
	MEDIUMP float V = visibility(pixel.linearRoughness, shading.NoV, shading.NoL, shading.LoH);
	MEDIUMP vec3  F = fresnel(pixel.f0, shading.LoH);

	return (D * V) * F;

	//return dFdx(normalize(shading.normal));
}

#ifdef MATERIAL_HAS_ANISOTROPY
MEDIUMP vec3 anisotropicLobe(const PixelParams pixel, const ShadingParams shading)
{
	MEDIUMP vec3 l = shading.L;
	MEDIUMP vec3 t = pixel.anisotropicT;
	MEDIUMP vec3 b = pixel.anisotropicB;
	MEDIUMP vec3 v = shading.view;
	MEDIUMP vec3 h = shading.H;

	MEDIUMP float ToV = dot(t, v);
	MEDIUMP float BoV = dot(b, v);
	MEDIUMP float ToL = dot(t, l);
	MEDIUMP float BoL = dot(b, l);
	MEDIUMP float ToH = dot(t, h);
	MEDIUMP float BoH = dot(b, h);

	// Anisotropic parameters: at and ab are the roughness along the tangent and bitangent
	// to simplify materials, we derive them from a single roughness parameter
	// Kulla 2017, "Revisiting Physically Based Shading at Imageworks"

	MEDIUMP float at = max(pixel.linearRoughness * (1.0 + pixel.anisotropy), MIN_LINEAR_ROUGHNESS);
	MEDIUMP float ab = max(pixel.linearRoughness * (1.0 - pixel.anisotropy), MIN_LINEAR_ROUGHNESS);

	// specular anisotropic BRDF
	MEDIUMP float D = distributionAnisotropic(at, ab, ToH, BoH, shading.NoH);
	MEDIUMP float V = visibilityAnisotropic(pixel.linearRoughness, at, ab, ToV, BoV, ToL, BoL, shading.NoV, shading.NoL);
	MEDIUMP vec3  F = fresnel(pixel.f0, shading.LoH);

	return (D * V) * F;
}
#endif

#ifdef MATERIAL_HAS_CLEAR_COAT
MEDIUMP float clearCoatLobe(const PixelParams pixel, const ShadingParams shading, out float Fcc)
{
	// If the material has a normal map, we want to use the geometric normal
	// instead to avoid applying the normal map details to the clear coat layer

	MEDIUMP float clearCoatNoH = saturate(dot(shading.clearCoatNormal, shading.H));

	// clear coat specular lobe
	MEDIUMP float D = distributionClearCoat(pixel.clearCoatLinearRoughness, clearCoatNoH, shading.H);
	MEDIUMP float V = visibilityClearCoat(pixel.clearCoatRoughness, pixel.clearCoatLinearRoughness, shading.LoH);
	MEDIUMP float F = F_Schlick(0.04, 1.0, shading.LoH) * pixel.clearCoat; // fix IOR to 1.5

	Fcc = F;
	return D * V * F;
}
#endif

MEDIUMP vec3 specularLobe(const PixelParams pixel, const ShadingParams shading)
{
#if defined(MATERIAL_HAS_ANISOTROPY)
	return anisotropicLobe(pixel, shading);
#else
	return isotropicLobe(pixel, shading);
#endif
}

MEDIUMP vec3 diffuseLobe(const PixelParams pixel, const ShadingParams shading)
{
    return pixel.diffuseColor * diffuse(pixel.linearRoughness, shading.NoV, shading.NoL, shading.LoH);
}

MEDIUMP vec3 surfaceShadingStandard(const PixelParams pixel, const ShadingParams shading)
{
	MEDIUMP vec3 Fr = specularLobe(pixel, shading);
	MEDIUMP vec3 Fd = diffuseLobe(pixel, shading);

	MEDIUMP vec3 color;

#ifdef MATERIAL_HAS_CLEAR_COAT
	{
		MEDIUMP float Fcc;
		MEDIUMP float clearCoat = clearCoatLobe(pixel, shading, Fcc);

		// Energy compensation and absorption; the clear coat Fresnel term is
		// squared to take into account both entering through and exiting through
		// the clear coat layer
		MEDIUMP float attenuation = 1.0 - Fcc;
		color = (Fd + Fr * (pixel.energyCompensation * attenuation)) * attenuation * shading.NoL;

		// If the material has a normal map, we want to use the geometric normal
		// instead to avoid applying the normal map details to the clear coat layer
		MEDIUMP float clearCoatNoL = saturate(dot(shading.clearCoatNormal, shading.L));
		color += clearCoat * clearCoatNoL;

		// Early exit to avoid the extra multiplication by NoL
		return color;
	}
#endif

	color = Fd + Fr * pixel.energyCompensation;

	return color * shading.NoL;
}

#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
MEDIUMP vec3 surfaceShadingSubSurface(const PixelParams pixel, const ShadingParams shading)
{
	MEDIUMP vec3 Fr = vec3_splat(0);

	if (shading.NoL > 0.0)
	{
		// specular BRDF
		MEDIUMP float D = distribution(pixel.linearRoughness, shading.NoH, shading.H);
		MEDIUMP float V = visibility(pixel.linearRoughness, shading.NoV, shading.NoL, shading.LoH);
		MEDIUMP vec3  F = fresnel(pixel.f0, shading.LoH);
		Fr = (D * V) * F * pixel.energyCompensation;
	}

	// diffuse BRDF
	MEDIUMP vec3 Fd = pixel.diffuseColor * diffuse(pixel.linearRoughness, shading.NoV, shading.NoL, shading.LoH);

	// NoL does not apply to transmitted light
	MEDIUMP vec3 color = (Fd + Fr) * shading.NoL;

	// subsurface scattering
	// Use a spherical gaussian approximation of pow() for forwardScattering
	// We could include distortion by adding shading.normal * distortion to light.l

	MEDIUMP float scatterVoH = saturate(dot(shading.view, -shading.L));
	MEDIUMP float forwardScatter = exp2(scatterVoH * pixel.subsurfacePower - pixel.subsurfacePower);
	MEDIUMP float backScatter = saturate(shading.NoL * pixel.thickness + (1.0 - pixel.thickness)) * 0.5;
	MEDIUMP float subsurface = mix(backScatter, 1.0, forwardScatter) * (1.0 - pixel.thickness);
	color += pixel.subsurfaceColor * (subsurface * Fd_Lambert());

	return color;
}
#endif

#if defined(SHADING_MODEL_CLOTH)
MEDIUMP vec3 surfaceShadingCloth(const PixelParams pixel, const ShadingParams shading)
{
	// specular BRDF
	MEDIUMP float D = distributionCloth(pixel.linearRoughness, shading.NoH);
	MEDIUMP float V = visibilityCloth(shading.NoV, shading.NoL);
	MEDIUMP vec3 F = pixel.f0;//vec3  F = fresnel(pixel.f0, shading.LoH);
	// Ignore pixel.energyCompensation since we use a different BRDF here
	MEDIUMP vec3 Fr = (D * V) * F;

	// diffuse BRDF
	MEDIUMP float diffuseC = diffuse(pixel.linearRoughness, shading.NoV, shading.NoL, shading.LoH);
	// Energy conservative wrap diffuse to simulate subsurface scattering
	diffuseC *= Fd_Wrap(dot(shading.normal, shading.L), 0.5);

	// We do not multiply the diffuse term by the Fresnel term as discussed in
	// Neubelt and Pettineo 2013, "Crafting a Next-gen Material Pipeline for The Order: 1886"
	// The effect is fairly subtle and not deemed worth the cost for mobile
	MEDIUMP vec3 Fd = diffuseC * pixel.diffuseColor;

	// Cheap subsurface scatter
	Fd *= saturate(pixel.subsurfaceColor + shading.NoL);
	// We need to apply NoL separately to the specular lobe since we already took
	// it into account in the diffuse lobe
	MEDIUMP vec3 color = Fd + Fr * shading.NoL;

	return color;
}
#endif

MEDIUMP vec3 surfaceShading(const PixelParams pixel, const ShadingParams shading)
{
#if defined(SHADING_MODEL_SUBSURFACE) || defined(SHADING_MODEL_FOLIAGE)
	return surfaceShadingSubSurface(pixel, shading);
#elif defined(SHADING_MODEL_CLOTH)
	return surfaceShadingCloth(pixel, shading);
#else
	return surfaceShadingStandard(pixel, shading);
#endif
}
