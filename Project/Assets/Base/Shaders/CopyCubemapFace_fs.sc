$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

SAMPLERCUBE(s_sourceTexture, 0);
uniform vec4 u_copyCubemapFaceIndex;

void main()
{
	int face = int( u_copyCubemapFaceIndex.x );
	vec2 texCoord = v_texCoord0 * 2.0 - 1.0;
	
	vec3 coords = vec3_splat( 0 );
	switch( face )
	{
		case 0: coords = vec3( 1, -texCoord.y, -texCoord.x ); break;
		case 1: coords = vec3( -1, -texCoord.y, texCoord.x ); break;
		case 2: coords = vec3( texCoord.x, 1, texCoord.y ); break;
		case 3: coords = vec3( texCoord.x, -1, -texCoord.y ); break;
		case 4: coords = vec3( texCoord.x, -texCoord.y, 1 ); break;
		case 5: coords = vec3( -texCoord.x, -texCoord.y, -1 ); break;
	}
	
	gl_FragColor = textureCubeLod( s_sourceTexture, coords, 0 );
}
