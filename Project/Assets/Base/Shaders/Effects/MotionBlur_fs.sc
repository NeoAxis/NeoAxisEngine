$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_motionTexture, 1);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ motionBlurMultiplier;
uniform vec4 viewportSize;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	vec2 motionVector = texture2D(s_motionTexture, v_texCoord0).xy;

	vec2 velocity = motionVector * motionBlurMultiplier.x;	
	velocity.y *= -1;

	const int MAX_SAMPLES = 32;
	vec2 texelSize = viewportSize.zw;
	float speed = length(velocity / texelSize);
	float samples = clamp(int(speed), 1, MAX_SAMPLES);
	
	vec4 color = sourceColor;
	for (int n = 1; n < samples; n++)
	{
		vec2 offset = velocity * (float(n) / float(samples - 1) - 0.5);
		color += texture2D(s_sourceTexture, v_texCoord0 + offset);
	}
	color /= float(samples);

	vec3 color2 = lerp(sourceColor, color, intensity.x);
	//!!!!alpha?
	gl_FragColor = float4(color2, 1.0);
}
