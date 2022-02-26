$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"
#include "../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);
//SAMPLER2D(s_patternTexture, 1);

uniform vec4/*float*/ intensity;
uniform vec4 bokehParameters;

bool isInsideHexagon(float x0, float y0, float d, float x, float y)
{
    float dx = abs(x - x0)/d;
    float dy = abs(y - y0)/d;
    float a = 0.25 * sqrt(3.0);
    return (dy <= a) && (a*dx + 0.25*dy <= 0.5*a);
}

void main()
{
	vec2 radius = bokehParameters.xy;
	float power = bokehParameters.z;
	
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	
	vec3 acc = vec3_splat(0.0);
	vec3 div = vec3_splat(0.0);

	
#ifdef PATTERN_VOGELDISK
	for (int n = 0; n < SAMPLES; n++)
	{
		vec2 offset = vec2_splat(0);
		offset = vogelDiskSample(n, SAMPLES, 0.0);

		vec3 col = texture2D(s_sourceTexture, v_texCoord0 + offset * radius).rgb;
		vec3 bokeh = pow(col, vec3_splat(power));
		acc += col * bokeh;
		div += bokeh;
	}	
#endif
	
	
#ifdef PATTERN_HEXAGON	

	//!!!!is not perfect implemetation
	
	
	for (int n = 0; n < SAMPLES; n++)
	{
		vec2 offset = vec2_splat(0);
		offset = vogelDiskSample(n, SAMPLES, 0.0);

		if(!isInsideHexagon(0,0,2,offset.y, offset.x))
			continue;
		
		vec3 col = texture2D(s_sourceTexture, v_texCoord0 + offset * radius).rgb;
		vec3 bokeh = pow(col, vec3_splat(power));
		acc += col * bokeh;
		div += bokeh;
	}

	
/*
	int samplesByAxis = int(sqrt(float(SAMPLES) * 1.27));
	
	for(int y=0; y<samplesByAxis; y++)
	{
		for(int x=0; x<samplesByAxis; x++)
		{
			//!!!!
			vec2 offset2 = vec2(float(x) / float(samplesByAxis), float(y) / float(samplesByAxis));
			vec2 offset = vec2(float(x) / float(samplesByAxis) - 0.5, float(y) / float(samplesByAxis) - 0.5) * 2.0;
			
			float mask = texture2D(s_patternTexture, offset2).r;
			
			vec3 col = texture2D(s_sourceTexture, v_texCoord0 + offset * radius).rgb;
			vec3 bokeh = pow(col, vec3_splat(power)) * mask;
			acc += col * bokeh;
			div += bokeh;
		}
	}
*/

/*
	int samplesByAxis = int(sqrt(float(SAMPLES) * 1.27));
	
	for(int y=0; y<samplesByAxis; y++)
	{
		for(int x=0; x<samplesByAxis; x++)
		{
			vec2 offset = vec2(float(x) / float(samplesByAxis) - 0.5, float(y) / float(samplesByAxis) - 0.5) * 2.0;
			if(isInsideHexagon(0,0,2,offset.y, offset.x))
			{
				vec3 col = texture2D(s_sourceTexture, v_texCoord0 + offset * radius).rgb;
				vec3 bokeh = pow(col, vec3_splat(power));
				acc += col * bokeh;
				div += bokeh;
			}
		}
	}
*/	
		
#endif
	
	
	vec4 color = vec4(acc / div, 1.0);
	
	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
