$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ levels;

float posterize(float v)
{
	return floor(v * levels.x) / levels.x;
}

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	
	vec4 color;
	color.r = posterize(sourceColor.r);
	color.g = posterize(sourceColor.g);
	color.b = posterize(sourceColor.b);
	color.a = sourceColor.a;
	
	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
