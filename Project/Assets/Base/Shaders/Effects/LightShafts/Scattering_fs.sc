$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

SAMPLER2D(s_depthTexture, 0);
uniform vec4/*vec2*/ screenLightPosition;
uniform vec4/*float*/ decay;
uniform vec4/*float*/ density;

void main()
{
	//constants
	const int iterations = 20;

	//calculate vector from pixel to light source in screen space.  
	vec2 deltaTexCoord = screenLightPosition.xy - v_texCoord0;
	deltaTexCoord *= density.x / float(iterations);

	float scatter = 0.0;
	vec2 newTexCoord = v_texCoord0;
	float fallOff = 1.0;
	for( int n = 0; n < iterations; n++ )
	{		
		float rawDepth = texture2D(s_depthTexture, newTexCoord).x;
		if(rawDepth >= 1.0) // detect the sky pixel
			scatter += fallOff;
		fallOff *= decay.x;

		newTexCoord += deltaTexCoord;
	}
	scatter /= float(iterations);
	
	gl_FragColor = vec4(scatter, 0, 0, 0);
}
