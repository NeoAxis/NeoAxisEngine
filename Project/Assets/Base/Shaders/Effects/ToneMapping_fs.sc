$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4 u_tonemapping_parameters;
#define intensity u_tonemapping_parameters.x
#define gammaInput u_tonemapping_parameters.y
#define exposure u_tonemapping_parameters.z
#define gammaOutput u_tonemapping_parameters.w

//////////////////////////////////////////////////////////
//Linear

vec3 method_linear(vec3 x)
{
	return x;
}

//////////////////////////////////////////////////////////
//ACES

vec3 RRTAndODTFit(vec3 v)
{
    vec3 a = v * (v + 0.0245786f) - 0.000090537f;
    vec3 b = v * (0.983729f * v + 0.4329510f) + 0.238081f;
    return a / b;
}

vec3 method_ACES(vec3 x)
{
	// sRGB => XYZ => D65_2_D60 => AP1 => RRT_SAT
	mat3 ACESInputMat = mtxFromRows(
	    0.59719, 0.35458, 0.04823,
	    0.07600, 0.90834, 0.01566,
	    0.02840, 0.13383, 0.83777
	);

	// ODT_SAT => XYZ => D60_2_D65 => sRGB
	mat3 ACESOutputMat = mtxFromRows(
	     1.60475, -0.53108, -0.07367,
	    -0.10208,  1.10813, -0.00605,
	    -0.00327, -0.07276,  1.07602
	);
	
	vec3 result = mul(ACESInputMat, x);
	result = RRTAndODTFit(result);
	result = mul(ACESOutputMat, result);
	result = saturate(result);
	return result;
}

//////////////////////////////////////////////////////////
//Custom

#ifdef CUSTOM_CODE
CUSTOM_CODE
#endif

//////////////////////////////////////////////////////////

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec3 color = sourceColor.rgb;

	// Gamma input
	color = pow(color,vec3_splat(1.0/gammaInput));
		
	// Exposure Adjustment
	color *= exposure;

	// Tone mapping
#ifdef TONEMAPPING_METHOD_LINEAR
	color = method_linear(color);
#elif defined(TONEMAPPING_METHOD_ACES)
	color = method_ACES(color);
#elif defined(TONEMAPPING_METHOD_CUSTOM)
	color = method_custom(color);
#endif
    
	// Gamma output
	color = pow(color,vec3_splat(1.0/gammaOutput));

	gl_FragColor = lerp(sourceColor, vec4(color, sourceColor.w), intensity);
}
