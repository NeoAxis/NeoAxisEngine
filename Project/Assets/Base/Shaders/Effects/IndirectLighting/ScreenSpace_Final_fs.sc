$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "../../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_blurTexture, 1);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ showIndirectLighting;
uniform vec4/*float*/ multiplier;

void main()
{
	vec4 sceneColor = texture2D(s_sourceTexture, v_texCoord0);
	vec4 lighting = texture2D(s_blurTexture, v_texCoord0);
	
	vec4 color = vec4(sceneColor.rgb + lighting.rgb * multiplier.x, sceneColor.a);
	if(showIndirectLighting.x > 0.0)
		color = vec4(lighting.rgb * multiplier.x, 1);
	
	gl_FragColor = lerp(sceneColor, color, intensity.x);
}
