$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_noiseTexture, 1);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ radius;
uniform vec4 color;
uniform vec4/*vec2*/ noiseRange;
uniform vec4 viewportSize;
uniform vec4 noiseTextureSize;

//https://stackoverflow.com/questions/5149544/can-i-generate-a-random-number-inside-a-pixel-shader
float random2( vec2 p )
{
	vec2 k1 = vec2(
		23.14069263277926, // e^pi (Gelfond's constant)
		2.665144142690225 // 2^sqrt(2) (Gelfonda€“Schneider constant)
	);
	return frac( cos( dot( p, k1 ) ) * 12345.6789 );
}

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec2 dist = v_texCoord0 - vec2(0.5,0.5);
	float powX = 1.0 - dot(dist, dist);
	float effect = saturate(pow(powX, radius.x));
	//float color = saturate(pow(powX, radius) + (1 - intensity));

	//noise
	BRANCH
	if(noiseRange.x != 1.0 || noiseRange.y != 1.0)
	{
		vec2 noiseUV = v_texCoord0 * viewportSize.xy * noiseTextureSize.zw;
		//noiseUV += seeds.xy;
		vec4 noise = texture2D(s_noiseTexture, noiseUV);
		
		float m = saturate((random2(noise.xy) + 1.0) / 2.0);
		float m2 = lerp(noiseRange.x, noiseRange.y, m);
		
		effect = saturate(effect * m2);
	}
	
//!!!!alpha
	vec4 color2 = lerp(color, sourceColor, effect);
//!!!!alpha?
//	float4 color2 = float4(sourceColor.rgb * effect, sourceColor.a);

	//!!!!
//	float2 range = float2(.8, 1.2);

//!!!!!do clamp or no

	//float2 range = float2(.7, 1.3);
//	float n = (random(input.uv) + 1) / 2;
//	n = lerp(range.x, range.y, n);
//	color2.rgb *= n;

	gl_FragColor = lerp(sourceColor, color2, intensity.x);
}
