$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"
#include "../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ intensity;
uniform vec4 u_paramColor;
#ifndef GLSL
SAMPLER2D(s_linearSamplerFragment, 10);
#endif

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

	vec2 texCoord0 = v_texCoord0;
	vec4 color0 = sourceColor;
	vec4 color = u_paramColor;
#ifdef FRAGMENT_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(makeSampler(s_linearSamplerFragment, _sampler), _uv, 0, 0)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_removeTextureTiling)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2D(makeSampler(s_linearSamplerFragment, _sampler), _uv)
	FRAGMENT_CODE_BODY
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif

	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
