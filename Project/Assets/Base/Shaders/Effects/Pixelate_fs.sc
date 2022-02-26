$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ cells;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	float count = cells.x - 1.0;
	float countX = count * u_viewportSize.x / u_viewportSize.y;
	float countY = count;
	
	vec2 texCoord;
	texCoord.x = round(v_texCoord0.x * countX) / countX;
	texCoord.y = round(v_texCoord0.y * countY) / countY;
	
	vec4 color = texture2D(s_sourceTexture, texCoord);
	
	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
