$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"
#include "../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ intensity;
uniform vec4 u_paramColor;

#ifdef FRAGMENT_CODE_PARAMETERS
	FRAGMENT_CODE_PARAMETERS
#endif
#ifdef FRAGMENT_CODE_SAMPLERS
	FRAGMENT_CODE_SAMPLERS
#endif
#ifdef FRAGMENT_CODE_SHADER_SCRIPTS
	FRAGMENT_CODE_SHADER_SCRIPTS
#endif

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec2 c_texCoord0 = v_texCoord0;
	vec4 c_color0 = sourceColor;
	vec4 color = u_paramColor;
#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(_sampler, _uv, u_removeTextureTiling)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2D(_sampler, _uv)
	FRAGMENT_CODE_BODY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	gl_FragColor = lerp(sourceColor, color, intensity.x);

	//alpha?
	//float3 color2 = lerp(sourceColor, color, intensity);
	//return float4(color2, 1.0);
}
