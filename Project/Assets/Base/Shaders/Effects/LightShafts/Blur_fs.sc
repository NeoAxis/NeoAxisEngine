$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

SAMPLER2D(s_scatteringTexture, 0);
uniform vec4/*vec3*/ color;
uniform vec4/*vec2*/ screenLightPosition;
uniform vec4/*float*/ blurFactor;

void main()
{
	//constants
	const int iterations = 20;

	//calculate vector from pixel to light source in screen space.
	vec2 deltaTexCoord =  screenLightPosition.xy - v_texCoord0;
	deltaTexCoord *= blurFactor.x / float(iterations);

	float scatter = 0.0;
	vec2 newTexCoord = v_texCoord0;
	for( int n = 0; n < iterations; n++ )
	{		
		scatter += texture2D(s_scatteringTexture, newTexCoord).r;
		newTexCoord += deltaTexCoord;
	}
	scatter /= float(iterations);
	
	vec3 scatteringColor = color.rgb * scatter;

	gl_FragColor = vec4(scatteringColor, 1);
}
