$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

SAMPLER2D(s_colorTexture, 0);
SAMPLER2D(s_depthTexture, 1);

void main()
{
	gl_FragColor = vec4( texture2D( s_colorTexture, v_texCoord0 ).rgb, texture2D( s_depthTexture, v_texCoord0 ).r );

	//not enough color precision
	
	//vec3 color = texture2D( s_colorTexture, v_texCoord0 ).rgb;
	//float depth =  texture2D( s_depthTexture, v_texCoord0 ).r;
	//gl_FragColor = vec4( packColor( color ), depth, 0, 0 );
}
