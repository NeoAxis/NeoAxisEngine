//#define GEOMETRIC_SPECULAR_AA_ROUGHNESS

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
	vec4  baseColor;
	float roughness;
	float metallic;
	float reflectance;
	float ambientOcclusion;

	float anisotropy;
	vec3 anisotropyDirection;

	float clearCoat;
	float clearCoatRoughness;
	vec3 clearCoatNormal;

#if defined(SHADING_MODEL_SUBSURFACE)
	float thickness;
	float subsurfacePower;
	vec3 subsurfaceColor;
#endif

#if defined(SHADING_MODEL_CLOTH)
	vec3 sheenColor;
	vec3 subsurfaceColor;
#endif

};

struct PixelParams {
	vec3 diffuseColor;
	float roughness;
	vec3 f0;
	float linearRoughness;
	vec3 dfg;
	vec3 energyCompensation;

	float clearCoat;
	float clearCoatRoughness;
	float clearCoatLinearRoughness;

	float anisotropy;
	vec3 anisotropicT;
	vec3 anisotropicB;

#if defined(SHADING_MODEL_SUBSURFACE)
	float thickness;
	vec3 subsurfaceColor;
	float subsurfacePower;
#endif

#if defined(SHADING_MODEL_CLOTH)
	vec3 subsurfaceColor;
#endif
};

// Filament Params:
// These variables should be in a struct but some GPU drivers ignore the
// precision qualifier on individual struct members
HIGHP mat3 shading_tangentToWorld;
vec3 shading_normal;
//vec3 shading_input_normal;
vec3 shading_clearCoatNormal;
vec3 shading_view;
vec3 shading_L;
vec3 shading_H;
vec3 shading_reflected;
float shading_NoV;              // dot(normal, view), always strictly >= MIN_N_DOT_V
float shading_NoL;
float shading_NoH;
float shading_LoH;

float shading_ToL;

float clampNoV(float NoV)
{
	// Neubelt and Pettineo 2013, "Crafting a Next-gen Material Pipeline for The Order: 1886"
	return max(NoV, MIN_N_DOT_V);
}

vec3 PrefilteredDFG_LUT(float roughness, float NoV)
{
	return texture2DLod(s_brdfLUT, vec2(NoV, 1.0-roughness), 0.0).rgb;
}

vec3 PrefilteredDFG(float roughness, float NoV)
{
	// PrefilteredDFG_LUT() takes a coordinate, which is sqrt(linear_roughness) = roughness
	return PrefilteredDFG_LUT(roughness, NoV);
}

vec3 diffuseIrradiance(const vec3 n, vec4 environmentIrradiance[9], EnvironmentTextureData environmentTextureIBLData)
{
	return getEnvironmentValue(environmentIrradiance, environmentTextureIBLData, n);	
}

//vec3 diffuseIrradiance(const vec3 n, samplerCube environmentTextureIBL, EnvironmentTextureData environmentTextureIBLData)
//{
//	return getEnvironmentValue(environmentTextureIBL, environmentTextureIBLData, n);	
//}

vec3 specularIrradiance(const vec3 r, float roughness, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	float lod;
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

vec3 specularIrradiance_Offset(const vec3 r, float roughness, float offset, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	float lod;
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

vec3 getSpecularDominantDirection(vec3 n, vec3 r, float linearRoughness)
{
#if defined(IBL_OFF_SPECULAR_PEAK)
	float s = 1.0 - linearRoughness;
	return mix(n, r, s * (sqrt(s) + linearRoughness));
#else
	return r;
#endif
}

vec3 specularDFG(const PixelParams pixel)
{
#if defined(SHADING_MODEL_CLOTH)
	return pixel.f0 * pixel.dfg.z;
#elif !defined(USE_MULTIPLE_SCATTERING_COMPENSATION)
	return pixel.f0 * pixel.dfg.x + pixel.dfg.y;
#else
	return mix(pixel.dfg.xxx, pixel.dfg.yyy, pixel.f0);
#endif
}

float computeSpecularAO(float NoV, float ao, float roughness)
{
#if defined(IBL_SPECULAR_OCCLUSION) && defined(MATERIAL_HAS_AMBIENT_OCCLUSION)
	return saturate(pow(NoV + ao, exp2(-16.0 * roughness - 1.0)) - 1.0 + ao);
#else
	return 1.0;
#endif
}

vec3 getReflectedVectorExt(const PixelParams pixel, const vec3 v, const vec3 n)
{
	vec3 r;
#if defined(MATERIAL_HAS_ANISOTROPY)
	vec3  anisotropyDirection = pixel.anisotropy >= 0.0 ? pixel.anisotropicB : pixel.anisotropicT;
	vec3  anisotropicTangent = cross(anisotropyDirection, v);
	vec3  anisotropicNormal = cross(anisotropicTangent, anisotropyDirection);
	float bendFactor = abs(pixel.anisotropy) * saturate(5.0 * pixel.roughness);
	vec3  bentNormal = normalize(mix(n, anisotropicNormal, bendFactor));
	r = reflect(-v, bentNormal);
#else
	r = reflect(-v, n);
#endif
	return r;
}

vec3 getReflectedVector(const PixelParams pixel, const vec3 n)
{
	vec3 r;
#if defined(MATERIAL_HAS_ANISOTROPY)
	r = getReflectedVectorExt(pixel, shading_view, n);
#else
	r = shading_reflected;
#endif
	return getSpecularDominantDirection(n, r, pixel.linearRoughness);
}

void evaluateClothIndirectDiffuseBRDF(const PixelParams pixel, inout float diffuse)
{
#if defined(SHADING_MODEL_CLOTH)
	// Simulate subsurface scattering with a wrap diffuse term
	diffuse *= Fd_Wrap(shading_NoV, 0.5);
#endif
}

void iblClearCoatDiffuse(const PixelParams pixel, inout vec3 Fd)
{
	float clearCoatNoV = clampNoV(dot(shading_clearCoatNormal, shading_view));
	// The clear coat layer assumes an IOR of 1.5 (4% reflectance)
	float Fc = F_Schlick(0.04, 1.0, clearCoatNoV) * pixel.clearCoat;
	float attenuation = 1.0 - Fc;
	Fd *= attenuation;
}

void iblClearCoatSpecular(const PixelParams pixel, float specularAO, inout vec3 Fr, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	float clearCoatNoV = clampNoV(dot(shading_clearCoatNormal, shading_view));
	vec3 clearCoatR = reflect(-shading_view, shading_clearCoatNormal);
	float Fc = F_Schlick(0.04, 1.0, clearCoatNoV) * pixel.clearCoat;
	float attenuation = 1.0 - Fc;
	Fr *= sq(attenuation);
	Fr += specularIrradiance(clearCoatR, pixel.clearCoatRoughness, environmentTexture, environmentTextureData) * (specularAO * Fc);
}

#if defined(SHADING_MODEL_SUBSURFACE)
void iblSubsurfaceDiffuse(const PixelParams pixel, const vec3 diffuseIrradiance, inout vec3 Fd, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	vec3 viewIndependent = diffuseIrradiance;
	vec3 viewDependent = specularIrradiance_Offset(-shading_view, pixel.roughness, 1.0 + pixel.thickness, environmentTexture, environmentTextureData);
	float attenuation = (1.0 - pixel.thickness) / (2.0 * PI);
	Fd += pixel.subsurfaceColor * (viewIndependent + viewDependent) * attenuation;
}
#endif

#if defined(SHADING_MODEL_CLOTH)
void iblClothDiffuse(const PixelParams pixel, const vec3 diffuseIrradiance, inout vec3 Fd, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	Fd *= saturate(pixel.subsurfaceColor + shading_NoV);
}
#endif

/*
void iblSubsurfaceDiffuse(const PixelParams pixel, const vec3 diffuseIrradiance, inout vec3 Fd, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
#if defined(SHADING_MODEL_SUBSURFACE)
	vec3 viewIndependent = diffuseIrradiance;
	vec3 viewDependent = specularIrradiance_Offset(-shading_view, pixel.roughness, 1.0 + pixel.thickness, environmentTexture, environmentTextureData);
	float attenuation = (1.0 - pixel.thickness) / (2.0 * PI);
	Fd += pixel.subsurfaceColor * (viewIndependent + viewDependent) * attenuation;
#elif defined(SHADING_MODEL_CLOTH)
	Fd *= saturate(pixel.subsurfaceColor + shading_NoV);
#endif
}
*/

vec3 iblDiffuse(const MaterialInputs material, const PixelParams pixel, vec4 environmentIrradiance[9], EnvironmentTextureData environmentTextureIBLData, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData, bool shadingModelSubsurface)
//vec3 iblDiffuse(const MaterialInputs material, const PixelParams pixel, samplerCube environmentTextureIBL, EnvironmentTextureData environmentTextureIBLData, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData, bool shadingModelSubsurface)
{
	vec3 n = shading_normal;

	float ao = material.ambientOcclusion;

	// diffuse indirect
	float diffuseBRDF = ao; // Fd_Lambert() is baked in the SH below
	//float diffuseBRDF = Fd_Lambert() * ao;
	evaluateClothIndirectDiffuseBRDF(pixel, diffuseBRDF);

	vec3 diffuseIrr = diffuseIrradiance(n, environmentIrradiance, environmentTextureIBLData);
	//vec3 diffuseIrr = diffuseIrradiance(n, environmentTextureIBL, environmentTextureIBLData);
	vec3 Fd = pixel.diffuseColor * diffuseIrr * diffuseBRDF;

#ifdef MATERIAL_HAS_CLEAR_COAT
	iblClearCoatDiffuse(pixel, Fd);
#endif

#if defined(SHADING_MODEL_SUBSURFACE)
	BRANCH
	if(shadingModelSubsurface)
		iblSubsurfaceDiffuse(pixel, diffuseIrr, Fd, environmentTexture, environmentTextureData);
#endif

#if defined(SHADING_MODEL_CLOTH)
	iblClothDiffuse(pixel, diffuseIrr, Fd, environmentTexture, environmentTextureData);
#endif

	return Fd * IBL_LUMINANCE;
}

vec3 getSpecularIrradiance(const PixelParams pixel, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	vec3 n = shading_normal;
	vec3 r = getReflectedVector(pixel, n);
	vec3 specIrradiance = specularIrradiance(r, pixel.roughness, environmentTexture, environmentTextureData);
	return specIrradiance;
}

vec3 iblSpecular(const MaterialInputs material, const PixelParams pixel, vec3 replaceSpecularIrradianceValue, 
	float replaceSpecularIrradianceFactor, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData)
{
	vec3 n = shading_normal;
	vec3 r = getReflectedVector(pixel, n);

	float ao = material.ambientOcclusion;
	float specularAO = computeSpecularAO(shading_NoV, ao, pixel.roughness);

	// specular indirect
	vec3 specIrradiance = mix(specularIrradiance(r, pixel.roughness, environmentTexture, environmentTextureData),
		replaceSpecularIrradianceValue, replaceSpecularIrradianceFactor);

	vec3 Fr = specularDFG(pixel) * specIrradiance * specularAO;
	Fr *= pixel.energyCompensation;

#ifdef MATERIAL_HAS_CLEAR_COAT
	iblClearCoatSpecular(pixel, specularAO, Fr, environmentTexture, environmentTextureData);
#endif
	
	return Fr * IBL_LUMINANCE;

	//return vec3(prefilteredDFG_LUT(pixel.roughness, shading_NoV), 0.0);
}

void setupPBRFilamentParams(const MaterialInputs material, vec3 tangent, vec3 bitangent, vec3 normal, vec3 inputNormal, vec3 toLight, vec3 toCamera, bool frontFacing)//, bool specularOnly = false)
{
	shading_normal = normal;
	//shading_input_normal = inputNormal;

	shading_view = toCamera;

	//if (!specularOnly)
	//{
	shading_L = toLight;
	shading_H = normalize(shading_view + shading_L);

	shading_NoL = saturate(dot(shading_normal, shading_L));
	shading_NoH = saturate(dot(shading_normal, shading_H));
	shading_LoH = saturate(dot(shading_L, shading_H));
	//}

	shading_ToL = saturate(dot(normal, shading_L));

	shading_tangentToWorld = transpose(mtxFromRows(tangent, bitangent, normal));

	shading_NoV = clampNoV(dot(shading_normal, shading_view));
	shading_reflected = reflect(-shading_view, normal);

#ifdef MATERIAL_HAS_CLEAR_COAT
	mat3 tangentToWorld_ClearCoat = transpose(mtxFromRows(tangent, bitangent, inputNormal));
	shading_clearCoatNormal = normalize(mul(tangentToWorld_ClearCoat, material.clearCoatNormal));
	#ifdef TWO_SIDED_FLIP_NORMALS
		if(frontFacing)
			shading_clearCoatNormal = -shading_clearCoatNormal;
	#endif	
#else
	shading_clearCoatNormal = vec3_splat(0);
#endif
	
}

void getPBRFilamentPixelParams(const MaterialInputs material, out PixelParams pixel)
{
	vec4 baseColor = material.baseColor;

#if !defined(SHADING_MODEL_CLOTH)
	float metallic = material.metallic;
	float reflectance = material.reflectance;

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
	float roughness = material.roughness;
	roughness = clamp(roughness, MIN_ROUGHNESS, 1.0);

#if defined(GEOMETRIC_SPECULAR_AA_ROUGHNESS)
	// Increase the roughness based on the curvature of the geometry to reduce
	// shading aliasing. The curvature is approximated using the derivatives
	// of the geometric normal
	//!!!!?
	vec3 ndFdx = dFdx(shading_normal);
	vec3 ndFdy = dFdy(shading_normal);
	float geometricRoughness = pow(saturate(max(dot(ndFdx, ndFdx), dot(ndFdy, ndFdy))), 0.333);
	roughness = max(roughness, geometricRoughness);
#endif

	pixel.clearCoat = material.clearCoat;

	// Clamp the clear coat roughness to avoid divisions by 0
	float clearCoatRoughness = material.clearCoatRoughness;
	clearCoatRoughness = mix(MIN_ROUGHNESS, MAX_CLEAR_COAT_ROUGHNESS, clearCoatRoughness);

#if defined(GEOMETRIC_SPECULAR_AA_ROUGHNESS)
	clearCoatRoughness = max(clearCoatRoughness, geometricRoughness);
#endif

	// Remap the roughness to perceptually linear roughness
	pixel.clearCoatRoughness = clearCoatRoughness;
	pixel.clearCoatLinearRoughness = clearCoatRoughness * clearCoatRoughness;

#if defined(MATERIAL_HAS_CLEAR_COAT_ROUGHNESS)
	// This is a hack but it will do: the base layer must be at least as rough
	// as the clear coat layer to take into account possible diffusion by the
	// top layer
	roughness = max(roughness, pixel.clearCoatRoughness);
#endif

#if defined(SHADING_MODEL_SUBSURFACE)
	pixel.subsurfacePower = material.subsurfacePower;
	pixel.subsurfaceColor = material.subsurfaceColor;
	pixel.thickness = saturate(material.thickness);
#endif

	pixel.anisotropy = material.anisotropy;
	pixel.anisotropicT = vec3_splat(0);
	pixel.anisotropicB = vec3_splat(0);
#if defined(MATERIAL_HAS_ANISOTROPY)
	pixel.anisotropicT = normalize(mul(shading_tangentToWorld, material.anisotropyDirection));
	pixel.anisotropicB = normalize(cross(shading_normal, pixel.anisotropicT));
#endif

	// Remaps the roughnes to a perceptually linear roughness (roughness^2)
	// TODO: the base layer's roughness should not be higher than the clear coat layer's
	pixel.roughness = roughness;
	pixel.linearRoughness = roughness * roughness;

	// Pre-filtered DFG term used for image-based lighting
	pixel.dfg = PrefilteredDFG(pixel.roughness, shading_NoV);

#if defined(USE_MULTIPLE_SCATTERING_COMPENSATION) && !defined(SHADING_MODEL_CLOTH)
	// Energy compensation for multiple scattering in a microfacet model
	// See "Multiple-Scattering Microfacet BSDFs with the Smith Model"
	pixel.energyCompensation = 1.0 + pixel.f0 * (1.0 / pixel.dfg.y - 1.0);
#else
	pixel.energyCompensation = vec3_splat(1);
#endif

}

vec3 isotropicLobe(const PixelParams pixel)
{
	float D = distribution(pixel.linearRoughness, shading_NoH, shading_H);
	float V = visibility(pixel.linearRoughness, shading_NoV, shading_NoL, shading_LoH);
	vec3  F = fresnel(pixel.f0, shading_LoH);

	return (D * V) * F;

	//return dFdx(normalize(shading_normal));
}

vec3 anisotropicLobe(const PixelParams pixel)
{
	vec3 l = shading_L;
	vec3 t = pixel.anisotropicT;
	vec3 b = pixel.anisotropicB;
	vec3 v = shading_view;
	vec3 h = shading_H;

	float ToV = dot(t, v);
	float BoV = dot(b, v);
	float ToL = dot(t, l);
	float BoL = dot(b, l);
	float ToH = dot(t, h);
	float BoH = dot(b, h);

	// Anisotropic parameters: at and ab are the roughness along the tangent and bitangent
	// to simplify materials, we derive them from a single roughness parameter
	// Kulla 2017, "Revisiting Physically Based Shading at Imageworks"

	float at = max(pixel.linearRoughness * (1.0 + pixel.anisotropy), MIN_LINEAR_ROUGHNESS);
	float ab = max(pixel.linearRoughness * (1.0 - pixel.anisotropy), MIN_LINEAR_ROUGHNESS);

	// specular anisotropic BRDF
	float D = distributionAnisotropic(at, ab, ToH, BoH, shading_NoH);
	float V = visibilityAnisotropic(pixel.linearRoughness, at, ab, ToV, BoV, ToL, BoL, shading_NoV, shading_NoL);
	vec3  F = fresnel(pixel.f0, shading_LoH);

	return (D * V) * F;
}

float clearCoatLobe(const PixelParams pixel, out float Fcc)
{
	// If the material has a normal map, we want to use the geometric normal
	// instead to avoid applying the normal map details to the clear coat layer

	float clearCoatNoH = saturate(dot(shading_clearCoatNormal, shading_H));

	// clear coat specular lobe
	float D = distributionClearCoat(pixel.clearCoatLinearRoughness, clearCoatNoH, shading_H);
	float V = visibilityClearCoat(pixel.clearCoatRoughness, pixel.clearCoatLinearRoughness, shading_LoH);
	float F = F_Schlick(0.04, 1.0, shading_LoH) * pixel.clearCoat; // fix IOR to 1.5

	Fcc = F;
	return D * V * F;
}

vec3 specularLobe(const PixelParams pixel)
{
#if defined(MATERIAL_HAS_ANISOTROPY)
	return anisotropicLobe(pixel);
#else
	return isotropicLobe(pixel);
#endif
}

vec3 diffuseLobe(const PixelParams pixel)
{
    return pixel.diffuseColor * diffuse(pixel.linearRoughness, shading_NoV, shading_NoL, shading_LoH);
}

vec3 surfaceShadingStandard(const PixelParams pixel)
{
	vec3 Fr = specularLobe(pixel);
	vec3 Fd = diffuseLobe(pixel);

	vec3 color;

#ifdef MATERIAL_HAS_CLEAR_COAT
	{
		float Fcc;
		float clearCoat = clearCoatLobe(pixel, Fcc);

		// Energy compensation and absorption; the clear coat Fresnel term is
		// squared to take into account both entering through and exiting through
		// the clear coat layer
		float attenuation = 1.0 - Fcc;
		color = (Fd + Fr * (pixel.energyCompensation * attenuation)) * attenuation * shading_NoL;

		// If the material has a normal map, we want to use the geometric normal
		// instead to avoid applying the normal map details to the clear coat layer
		float clearCoatNoL = saturate(dot(shading_clearCoatNormal, shading_L));
		color += clearCoat * clearCoatNoL;

		// Early exit to avoid the extra multiplication by NoL
		return color;
	}
#endif

	color = Fd + Fr * pixel.energyCompensation;

	return color * shading_NoL;
}

#if defined(SHADING_MODEL_SUBSURFACE)
vec3 surfaceShadingSubSurface(const PixelParams pixel)
{
	vec3 Fr = vec3_splat(0);

	if (shading_NoL > 0.0)
	{
		// specular BRDF
		float D = distribution(pixel.linearRoughness, shading_NoH, shading_H);
		float V = visibility(pixel.linearRoughness, shading_NoV, shading_NoL, shading_LoH);
		vec3  F = fresnel(pixel.f0, shading_LoH);
		Fr = (D * V) * F * pixel.energyCompensation;
	}

	// diffuse BRDF
	vec3 Fd = pixel.diffuseColor * diffuse(pixel.linearRoughness, shading_NoV, shading_NoL, shading_LoH);

	// NoL does not apply to transmitted light
	vec3 color = (Fd + Fr) * shading_NoL;

	// subsurface scattering
	// Use a spherical gaussian approximation of pow() for forwardScattering
	// We could include distortion by adding shading_normal * distortion to light.l

	float scatterVoH = saturate(dot(shading_view, -shading_L));
	float forwardScatter = exp2(scatterVoH * pixel.subsurfacePower - pixel.subsurfacePower);
	float backScatter = saturate(shading_NoL * pixel.thickness + (1.0 - pixel.thickness)) * 0.5;
	float subsurface = mix(backScatter, 1.0, forwardScatter) * (1.0 - pixel.thickness);
	color += pixel.subsurfaceColor * (subsurface * Fd_Lambert());

	return color;
}
#endif

#if defined(SHADING_MODEL_CLOTH)
vec3 surfaceShadingCloth(const PixelParams pixel)
{
	// specular BRDF
	float D = distributionCloth(pixel.linearRoughness, shading_NoH);
	float V = visibilityCloth(shading_NoV, shading_NoL);
	vec3 F = pixel.f0;//vec3  F = fresnel(pixel.f0, shading_LoH);
	// Ignore pixel.energyCompensation since we use a different BRDF here
	vec3 Fr = (D * V) * F;

	// diffuse BRDF
	float diffuseC = diffuse(pixel.linearRoughness, shading_NoV, shading_NoL, shading_LoH);
	// Energy conservative wrap diffuse to simulate subsurface scattering
	diffuseC *= Fd_Wrap(dot(shading_normal, shading_L), 0.5);

	// We do not multiply the diffuse term by the Fresnel term as discussed in
	// Neubelt and Pettineo 2013, "Crafting a Next-gen Material Pipeline for The Order: 1886"
	// The effect is fairly subtle and not deemed worth the cost for mobile
	vec3 Fd = diffuseC * pixel.diffuseColor;

	// Cheap subsurface scatter
	Fd *= saturate(pixel.subsurfaceColor + shading_NoL);
	// We need to apply NoL separately to the specular lobe since we already took
	// it into account in the diffuse lobe
	vec3 color = Fd + Fr * shading_NoL;

	return color;
}
#endif

vec3 surfaceShading(const PixelParams pixel)
{
#if defined(SHADING_MODEL_SUBSURFACE)
	return surfaceShadingSubSurface(pixel);
#elif defined(SHADING_MODEL_CLOTH)
	return surfaceShadingCloth(pixel);
#else
	return surfaceShadingStandard(pixel);
#endif
}
