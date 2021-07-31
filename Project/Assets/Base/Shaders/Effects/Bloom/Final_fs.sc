$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_bloomTexture, 1);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ scale;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	vec4 bloomColor = texture2D(s_bloomTexture, v_texCoord0);

	// Scene color
	vec3 sceneColor = sourceColor.rgb;

	//half adaptedLuminance = tex2D(adaptedLuminanceTexture, half2(.5f, .5f)).r;
	//sceneColor *= adaptationMiddleBrightness / (adaptedLuminance + .001f);
	////sceneColor /= (1.0f + sceneColor);

	vec3 color = sceneColor + bloomColor.rgb * scale.x;
	vec4 color4 = vec4(color, sourceColor.w);

	gl_FragColor = lerp(sourceColor, color4, intensity.x);
}
