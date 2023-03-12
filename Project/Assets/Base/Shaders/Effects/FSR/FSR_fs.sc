$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4 viewportSize;
uniform vec4 fsrParameters[5];

////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef HLSL
//HLSL original implementation


#define Const0 floatBitsToUint(fsrParameters[0])
#define Const1 floatBitsToUint(fsrParameters[1])
#define Const2 floatBitsToUint(fsrParameters[2])
#define Const3 floatBitsToUint(fsrParameters[3])
#define Sample fsrParameters[4]

#define A_GPU 1
#define A_HLSL 1

// You may encounter image corruption issues on NVIDIA RTX 3000 series when FP16 FSR shaders are used with DirectX 11. If this issue manifests itself then the FP32 version of FSR should be used on these GPUs until NVIDIA issues a driver fix. This issue does not affect DirectX 12 or Vulkan.
#define SAMPLE_SLOW_FALLBACK 1

//#if SAMPLE_SLOW_FALLBACK

#include "ffx_a.sh"

#define FSR_EASU_F 1
AF4 FsrEasuRF(AF2 p) { AF4 res = s_sourceTexture.m_texture.GatherRed(s_sourceTexture.m_sampler, p, ivec2(0,0) ); return res; }
AF4 FsrEasuGF(AF2 p) { AF4 res = s_sourceTexture.m_texture.GatherGreen(s_sourceTexture.m_sampler, p, ivec2(0,0) ); return res; }
AF4 FsrEasuBF(AF2 p) { AF4 res = s_sourceTexture.m_texture.GatherBlue(s_sourceTexture.m_sampler, p, ivec2(0,0) ); return res; }
		
/*	
#else
	
	#define A_HALF
	#include "ffx_a.sh"
	Texture2D<AH4> InputTexture : register(t0);
	RWTexture2D<AH4> OutputTexture : register(u0);
	#define FSR_EASU_H 1
	AH4 FsrEasuRH(AF2 p) { AH4 res = InputTexture.GatherRed(samLinearClamp, p, int2(0, 0)); return res; }
	AH4 FsrEasuGH(AF2 p) { AH4 res = InputTexture.GatherGreen(samLinearClamp, p, int2(0, 0)); return res; }
	AH4 FsrEasuBH(AF2 p) { AH4 res = InputTexture.GatherBlue(samLinearClamp, p, int2(0, 0)); return res; }	
#endif
*/

#include "ffx_fsr1.sh"

vec3 CurrFilter(ivec2 pos)
{
	//#if SAMPLE_SLOW_FALLBACK
	
	AF3 c;
	FsrEasuF(c, pos, Const0, Const1, Const2, Const3);
	//if (Sample.x > 0.0)
	//	c *= c;
	return c;
	
	/*
	#else
		AH3 c;
		FsrEasuH(c, pos, Const0, Const1, Const2, Const3);
		if (Sample.x > 0.0)
			c *= c;
		OutputTexture[pos] = AH4(c, 1);
	#endif
	*/
}

void main()
{
	vec3 color = CurrFilter(ivec2(v_texCoord0 * viewportSize.xy));
	gl_FragColor = vec4(color, 1);
}


#else
////////////////////////////////////////////////////////////////////////////////////////////////
//GLSL implementation for mobile

//!!!!impl


/*

#define Const0 fsrParameters[0]
#define Const1 fsrParameters[1]
#define Const2 fsrParameters[2]
#define Const3 fsrParameters[3]
#define Sample fsrParameters[4]

#define A_HALF
#define A_GPU 1
#define A_GLSL 1

#include "ffx_a_mobile.sh"

AH3 FsrEasuSampleH(AF2 p) { return texture2D(s_sourceTexture, p).xyz; }
#include "ffx_fsr1_mobile.sh"

vec3 CurrFilter(vec2 pos)
{
	AF3 c;
	FsrEasuF(c, pos, Const0, Const1, Const2, Const3);
	//if (Sample.x > 0.0)
	//	c *= c;
	return c;
}

void main()
{
	vec3 color = CurrFilter(v_texCoord0 * viewportSize.xy);
	gl_FragColor = vec4(color, 1);
}

*/

#endif
