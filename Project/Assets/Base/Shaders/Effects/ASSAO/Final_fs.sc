$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_blurTexture, 1);

uniform vec4 intensity;
uniform vec4 showAO;
uniform vec4 maxValue;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	float occlusion = 1.0 - texture2D(s_blurTexture, v_texCoord0).x;
	occlusion = clamp(occlusion, 0.0, maxValue.x);
	
	float coef = saturate(occlusion * intensity.x);	
	
	vec3 color;

	if(showAO.x > 0)
		color = lerp(vec3(1, 1, 1), vec3(0, 0, 0), coef);
	else
		color = lerp(sourceColor.rgb, vec3(0, 0, 0), coef);

	gl_FragColor = lerp(sourceColor, vec4(color, sourceColor.w), intensity.x);
}