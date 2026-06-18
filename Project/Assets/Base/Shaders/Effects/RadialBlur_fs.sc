$input v_texCoord0

// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ intensity;
uniform vec4/*float2*/ center;
uniform vec4/*float*/ blurFactor;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	const int samples = 32;
	vec4 color = vec4_splat(0);
	for(int n = 0; n < samples; n++) 
	{ 
		float scale = 1.0f - blurFactor.x * (float(n) / float(samples - 1));
		color += texture2D(s_sourceTexture, (v_texCoord0 - center.xy) * scale + center.xy);
	} 
	color /= float(samples);

	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
