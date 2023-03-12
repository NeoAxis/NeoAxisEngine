$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ viewportSize;
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ sharpStrength;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	
	vec3 col = texture2D(s_sourceTexture, v_texCoord0).xyz;

	// CAS algorithm
	float max_g = col.y;
	float min_g = col.y;
	vec4 uvoff = vec4(1.0,0.0,1.0,-1.0) * viewportSize.zwzw;
	vec3 colw;
	vec3 col1 = texture2D(s_sourceTexture, v_texCoord0+uvoff.yw).xyz;
	max_g = max(max_g, col1.y);
	min_g = min(min_g, col1.y);
	colw = col1;
	col1 = texture2D(s_sourceTexture, v_texCoord0+uvoff.xy).xyz;
	max_g = max(max_g, col1.y);
	min_g = min(min_g, col1.y);
	colw += col1;
	col1 = texture2D(s_sourceTexture, v_texCoord0+uvoff.yz).xyz;
	max_g = max(max_g, col1.y);
	min_g = min(min_g, col1.y);
	colw += col1;
	col1 = texture2D(s_sourceTexture, v_texCoord0-uvoff.xy).xyz;
	max_g = max(max_g, col1.y);
	min_g = min(min_g, col1.y);
	colw += col1;
	float d_min_g = min_g;
	float d_max_g = 1.0-max_g;
	float A;
	max_g = max(0.0, max_g);
	if (d_max_g < d_min_g)
		A = d_max_g / max_g;
	else
		A = d_min_g / max_g;
	A = sqrt(max(0.0, A));
	A *= mix(-0.125, -0.2, sharpStrength.x);
	vec3 col_out = (col + colw * A) / (1.0+4.0*A);

	gl_FragColor = lerp(sourceColor, vec4(col_out,1.0), intensity.x);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


/*

//HLSL implementation from FSR
#ifdef HLSL

// FidelityFX Super Resolution Sample
// Copyright (c) 2021 Advanced Micro Devices, Inc. All rights reserved.
// MIT

uniform vec4 fsrParameters[5];
#define Const0 floatBitsToUint(fsrParameters[0])
#define Const1 floatBitsToUint(fsrParameters[1])
#define Const2 floatBitsToUint(fsrParameters[2])
#define Const3 floatBitsToUint(fsrParameters[3])
#define Sample fsrParameters[4]

#define A_GPU 1

#ifdef GLSL
#define A_GLSL 1
#define A_SKIP_EXT 1
#else
#define A_HLSL 1
#endif

//!!!!
// You may encounter image corruption issues on NVIDIA RTX 3000 series when FP16 FSR shaders are used with DirectX 11. If this issue manifests itself then the FP32 version of FSR should be used on these GPUs until NVIDIA issues a driver fix. This issue does not affect DirectX 12 or Vulkan.
#define SAMPLE_SLOW_FALLBACK 1

//#if SAMPLE_SLOW_FALLBACK

#include "FSR/ffx_a.sh"

#define FSR_RCAS_F
AF4 FsrRcasLoadF(ASU2 p) { return texelFetch(s_sourceTexture, p, 0); }
//AF4 FsrRcasLoadF(ASU2 p) { return InputTexture.Load(int3(ASU2(p), 0)); }
void FsrRcasInputF(inout AF1 r, inout AF1 g, inout AF1 b) {}		

//#else
//	
//	#define A_HALF
//	#include "ffx_a.sh"
//	Texture2D<AH4> InputTexture : register(t0);
//	RWTexture2D<AH4> OutputTexture : register(u0);
//	#if SAMPLE_RCAS
//		#define FSR_RCAS_H
//		AH4 FsrRcasLoadH(ASW2 p) { return InputTexture.Load(ASW3(ASW2(p), 0)); }
//		void FsrRcasInputH(inout AH1 r,inout AH1 g,inout AH1 b){}
//	#endif
//#endif

#include "FSR/ffx_fsr1.sh"

vec4 CurrFilter(uvec2 pos)
{
//#if SAMPLE_SLOW_FALLBACK

	AF3 c;
	FsrRcasF(c.r, c.g, c.b, pos, Const0);
	//if (Sample.x > 0.0)
	//	c *= c;
	return vec4(c, 1.0);
	
//#else
//	AH3 c;
//	FsrRcasH(c.r, c.g, c.b, pos, Const0);
//	if( Sample.x > 0.0)
//		c *= c;
//	OutputTexture[pos] = AH4(c, 1);
//#endif

}

void main()
{	
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	vec4 color = CurrFilter(uvec2(v_texCoord0 * viewportSize.xy));	
	gl_FragColor = lerp(sourceColor, color, intensity.x);
}

#endif

*/


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//uniform vec4/*float*/ sharpStrength;
////uniform vec4/*float*/ sharpClamp;
////uniform vec4/*float*/ offsetBias;

/*
void main()
{
	float sharpClamp = 0.035;
	float offsetBias = 1.0;
	
	
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec3 coef_luma = vec3(0.2126, 0.7152, 0.0722);      // BT.709 & sRBG luma coefficient (Monitors and HD Television)
	//vec3 coef_luma = vec3(0.299, 0.587, 0.114);       // BT.601 luma coefficient (SD Television)
	//vec3 coef_luma = vec3(1.0/3.0, 1.0/3.0, 1.0/3.0); // Equal weight coefficient

	vec3 sharp_strength_luma = coef_luma * sharpStrength.x;

	// -- Gaussian filter --
	//   [ .25, .50, .25]     [ 1 , 2 , 1 ]
	//   [ .50,   1, .50]  =  [ 2 , 4 , 2 ]
 	//   [ .25, .50, .25]     [ 1 , 2 , 1 ]

	float px = viewportSize.z;
	float py = viewportSize.w;

	vec3 blur_ori = texture2D(s_sourceTexture, v_texCoord0 + vec2(px,-py) * 0.5 * offsetBias).rgb; // South East
	blur_ori += texture2D(s_sourceTexture, v_texCoord0 + vec2(-px,-py) * 0.5 * offsetBias).rgb;  // South West    
	blur_ori += texture2D(s_sourceTexture, v_texCoord0 + vec2(px,py) * 0.5 * offsetBias).rgb; // North East
	blur_ori += texture2D(s_sourceTexture, v_texCoord0 + vec2(-px,py) * 0.5 * offsetBias).rgb; // North West
	blur_ori *= 0.25;  // ( /= 4) Divide by the number of texture fetches


	// -- Calculate the sharpening --  
	vec3 sharp = sourceColor.rgb - blur_ori;  //Subtracting the blurred image from the original image

	// -- Adjust strength of the sharpening --
	float sharp_luma = dot(sharp, sharp_strength_luma); //Calculate the luma and adjust the strength

	// -- Clamping the maximum amount of sharpening to prevent halo artifacts --
	sharp_luma = clamp(sharp_luma, -sharpClamp, sharpClamp);  //TODO Try a curve function instead of a clamp

	// -- Combining the values to get the final sharpened pixel	--
	vec4 done = vec4(sourceColor.rgb + sharp_luma, sourceColor.w);    // Add the sharpening to the original.
	//vec4 done = inputcolor + sharp_luma;    // Add the sharpening to the input color.


	//#if show_sharpen == 1
	//  //vec3 chroma = ori - luma;
	//  //done = abs(sharp * 4).rrr;
	//  done = saturate(0.5 + (sharp_luma * 4)).rrrr;
	//#endif

	
	gl_FragColor = lerp(sourceColor, done, intensity.x);
	//vec4 done2 = saturate(done);
	//return lerp(sourceColor, done2, intensity);
}
*/
