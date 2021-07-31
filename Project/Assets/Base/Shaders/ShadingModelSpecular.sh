// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

struct Material_SpecularSM_Inputs
{
	vec3 diffuseColor;
	vec3 specularColor;
	float shininess;
	float AO;
};

vec3 shading_normal_sm;
vec3 shading_L_sm;
vec3 shading_V_sm;

//!!!!
void prepare_SpecularSM(vec3 normal, vec3 toLight, vec3 toCamera)
{
	shading_normal_sm = normal;
	shading_L_sm = toLight;
	shading_V_sm = toCamera;	
}

vec3 surfaceShading_SpecularSM(Material_SpecularSM_Inputs material, vec3 lightColor)
{
	float NoL = dot(shading_normal_sm, shading_L_sm);

	vec3 diffuseReflection = material.diffuseColor * max(0.0, NoL);

	vec3 specularReflection;
	if (NoL < 0.0) // light source on the wrong side?
	{
		specularReflection = vec3_splat(0); // no specular reflection
	}
	else // light source on the right side
	{
		specularReflection = material.specularColor *
			pow(max(0.0, dot(reflect(-shading_L_sm, shading_normal_sm), shading_V_sm)), material.shininess);
	}

	return (diffuseReflection + specularReflection) * lightColor * material.AO;
}

vec3 iblEnvironment_SpecularSM(vec3 replaceSpecularIrradianceValue, float replaceSpecularIrradianceFactor, samplerCube environmentTexture, EnvironmentTextureData environmentTextureData, vec3 lightColor)
{  
	vec3 r = reflect(-shading_V_sm, shading_normal_sm);

	vec3 reflectionCM = getEnvironmentValue(environmentTexture, environmentTextureData, r);

	vec3 envReflection = mix(reflectionCM, replaceSpecularIrradianceValue, replaceSpecularIrradianceFactor);

	return envReflection * lightColor;
}
