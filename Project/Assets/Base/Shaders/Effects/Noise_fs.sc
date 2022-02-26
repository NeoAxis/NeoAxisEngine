$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_noiseTexture, 1);
uniform vec4/*float*/ intensity;
uniform vec4/*vec2*/ multiply;
uniform vec4/*vec2*/ add;
uniform vec4 viewportSize;
uniform vec4 noiseTextureSize;
uniform vec4 seeds;

//https://stackoverflow.com/questions/5149544/can-i-generate-a-random-number-inside-a-pixel-shader
//Output interval: 0 - 1
float random2( vec2 p )
{
	vec2 k1 = vec2(
		23.14069263277926, // e^pi (Gelfond's constant)
		2.665144142690225 // 2^sqrt(2) (Gelfondaˆ“Schneider constant)
	);
	return frac( cos( dot( p, k1 ) ) * 12345.6789 );
}

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec2 noiseUV = v_texCoord0 * viewportSize.xy * noiseTextureSize.zw;
	noiseUV += seeds.xy;
	vec4 noise = texture2D(s_noiseTexture, noiseUV);

	float m = (random2(noise.xy) + 1.0) / 2.0;
	m = lerp(multiply.x, multiply.y, m);

	float a = (random2(noise.yz) + 1.0) / 2.0;
	a = lerp(add.x, add.y, a);

//!!!!!clamp? optionally?

	vec4 color = sourceColor * m + vec4(a, a, a, 0);
		
	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
