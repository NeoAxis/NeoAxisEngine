$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);

uniform vec4/*vec2*/ antialiasing_multiplier;

void main()
{
#ifdef SSAAX2
	const int count = 8;
	const vec2 offsets[8] =
	{
		vec2(-0.1,-0.37) * 0.3 + vec2(-0.4,-0.4),
		vec2(0.1,0.37) * 0.3 + vec2(-0.4,-0.4),

		vec2(-0.37,0.1) * 0.3 + vec2(0.4,-0.4),
		vec2(0.37,-0.1) * 0.3 + vec2(0.4,-0.4),

		vec2(-0.1,-0.37) * 0.3 + vec2(0.4,0.4),
		vec2(0.1,0.37) * 0.3 + vec2(0.4,0.4),

		vec2(-0.37,0.1) * 0.3 + vec2(-0.4,0.4),
		vec2(0.37,-0.1) * 0.3 + vec2(-0.4,0.4),
	};
/*
	const int count = 4;
	const vec2 offsets[4] =
	{
		vec2(-0.35,-0.25),
		vec2(0.35,0.25),
		vec2(0.25,-0.35),
		vec2(-0.25,0.35)
//		vec2(-0.35,-0.35),
//		vec2(-0.35,0.35),
//		vec2(0.35,-0.35),
//		vec2(0.35,0.35)
	};
*/
#endif

#ifdef SSAAX3
	const int count = 8;
	const vec2 offsets[8] =
	{
		vec2(-0.1,-0.37) * 0.7 + vec2(-0.5,-0.5),
		vec2(0.1,0.37) * 0.7 + vec2(-0.5,-0.5),

		vec2(-0.37,0.1) * 0.7 + vec2(0.5,-0.5),
		vec2(0.37,-0.1) * 0.7 + vec2(0.5,-0.5),

		vec2(-0.1,-0.37) * 0.7 + vec2(0.5,0.5),
		vec2(0.1,0.37) * 0.7 + vec2(0.5,0.5),

		vec2(-0.37,0.1) * 0.7 + vec2(-0.5,0.5),
		vec2(0.37,-0.1) * 0.7 + vec2(-0.5,0.5),
	};
/*
	const int count = 4;
	const vec2 offsets[4] =
	{
		vec2(-0.4,-0.1),
		vec2(0.4,0.1),
		vec2(0.1,-0.4),
		vec2(-0.1,0.4)
	};
	*/
#endif

#ifdef SSAAX4
	const int count = 16;
	const vec2 offsets[16] =
	{
		vec2(-0.1,-0.37) * 0.7 + vec2(-0.5,-0.5),
		vec2(0.37,-0.1) * 0.7 + vec2(-0.5,-0.5),
		vec2(0.1,0.37) * 0.7 + vec2(-0.5,-0.5),
		vec2(-0.37,0.1) * 0.7 + vec2(-0.5,-0.5),

		vec2(-0.1,-0.37) * 0.7 + vec2(0.5,-0.5),
		vec2(0.37,-0.1) * 0.7 + vec2(0.5,-0.5),
		vec2(0.1,0.37) * 0.7 + vec2(0.5,-0.5),
		vec2(-0.37,0.1) * 0.7 + vec2(0.5,-0.5),

		vec2(-0.1,-0.37) * 0.7 + vec2(0.5,0.5),
		vec2(0.37,-0.1) * 0.7 + vec2(0.5,0.5),
		vec2(0.1,0.37) * 0.7 + vec2(0.5,0.5),
		vec2(-0.37,0.1) * 0.7 + vec2(0.5,0.5),

		vec2(-0.1,-0.37) * 0.7 + vec2(-0.5,0.5),
		vec2(0.37,-0.1) * 0.7 + vec2(-0.5,0.5),
		vec2(0.1,0.37) * 0.7 + vec2(-0.5,0.5),
		vec2(-0.37,0.1) * 0.7 + vec2(-0.5,0.5)
	};
#endif

	vec4 result = vec4_splat(0);
	for(int n=0;n<count;n++)
		result += texture2D(s_sourceTexture, v_texCoord0 + offsets[n] * antialiasing_multiplier.xy);
	result /= count;
	
	gl_FragColor = result;
}
