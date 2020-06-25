$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_depthTexture, 0);
uniform vec4/*float*/ affectBackground;
//uniform mat4 invViewProj;

void main()
{
#ifdef GLOBAL_FOG_SUPPORT

	float rawDepth = texture2D(s_depthTexture, v_texCoord0).x;
	
	float value;
	if(rawDepth < 1.0)
	{
		vec3 worldPosition = reconstructWorldPosition(u_invViewProj, v_texCoord0, rawDepth);
		value = 1.0 - getFogFactor(worldPosition);
	}
	else
		value = affectBackground.x;
	
	gl_FragColor = vec4(u_fogColor.rgb, value);

#else
	gl_FragColor = vec4(0,0,0,0);
#endif
}
