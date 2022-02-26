$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"
#include "../FragmentFunctions.sh"

SAMPLER2D(s_depthTexture, 0);
uniform vec4/*float*/ affectBackground;
//uniform mat4 invViewProj;

void main()
{
	float rawDepth = texture2D(s_depthTexture, v_texCoord0).x;

	float backgroundFactor = 1.0;
	if(rawDepth >= 1.0)
		backgroundFactor = affectBackground.x;
	
	vec3 worldPosition = reconstructWorldPosition(u_invViewProj, v_texCoord0, rawDepth);
	float value = 1.0 - getFogFactor(worldPosition, backgroundFactor);
	gl_FragColor = vec4(u_fogColor.rgb, value);
}
