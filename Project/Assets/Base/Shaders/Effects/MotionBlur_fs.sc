$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"
#include "../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_motionTexture, 1);
SAMPLER2D(s_depthTexture, 2);

uniform vec4/*float*/ intensity;
uniform vec4 motionBlurParameters;//x - multiplier, y - depth threshold
uniform vec4 viewportSize;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	vec2 motionVector = texture2D(s_motionTexture, v_texCoord0).xy;

	float rawDepth = texture2D(s_depthTexture, v_texCoord0).r;
	float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
	
	
	vec4 color = sourceColor;
	
	vec2 velocity = motionVector * motionBlurParameters.x;	
	velocity.y *= -1;

	const int MAX_SAMPLES = 32;
	vec2 texelSize = viewportSize.zw;
	float speed = length(velocity / texelSize);
	float samples = clamp(int(speed), 1, MAX_SAMPLES);

	int count = 1;
	for (int n = 1; n < MAX_SAMPLES && n < samples; n++)
	{
		vec2 offset = velocity * (float(n) / float(samples - 1) - 0.5);
		
		//vec2 motionVector2 = texture2D(s_motionTexture, v_texCoord0 + offset).xy;
		//if(any(motionVector2))
		//{
		float rawDepth2 = texture2D(s_depthTexture, v_texCoord0 + offset).r;
		float depth2 = getDepthValue(rawDepth2, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
		if(abs(depth - depth2) < motionBlurParameters.y)
		{
			color += texture2D(s_sourceTexture, v_texCoord0 + offset);
			count++;
		}
		//}
	}
	color /= float(count);
	
	vec3 color2 = lerp(sourceColor, color, intensity.x);
	gl_FragColor = float4(color2, 1.0);
}
