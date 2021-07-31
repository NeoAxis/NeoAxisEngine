$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ amount;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec2 uv = v_texCoord0;
	float amount2 = amount.x * 0.004;
	
	vec4 color;
	color.r = texture2D(s_sourceTexture, vec2(uv.x+amount2,uv.y)).r;
	color.g = texture2D(s_sourceTexture, uv).g;
	color.b = texture2D(s_sourceTexture, vec2(uv.x-amount2,uv.y)).b;
	color.a = sourceColor.a;

	color.rgb *= (1.0 - amount2 * 0.5);

	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
