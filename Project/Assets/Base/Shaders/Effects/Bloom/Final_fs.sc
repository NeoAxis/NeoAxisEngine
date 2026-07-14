$input v_texCoord0

// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "../../Common.sh"

SAMPLER2D(s_sourceTexture_0, 0);
SAMPLER2D(s_bloomTexture_1, 1);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ scale;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture_0, v_texCoord0);
	vec4 bloomColor = texture2D(s_bloomTexture_1, v_texCoord0);

	// Scene color
	vec3 sceneColor = sourceColor.rgb;

	//half adaptedLuminance = tex2D(adaptedLuminanceTexture, half2(.5f, .5f)).r;
	//sceneColor *= adaptationMiddleBrightness / (adaptedLuminance + .001f);
	////sceneColor /= (1.0f + sceneColor);

	vec3 color = sceneColor + bloomColor.rgb * scale.x;
	vec4 color4 = vec4(color, sourceColor.w);

	gl_FragColor = lerp(sourceColor, color4, intensity.x);
}
