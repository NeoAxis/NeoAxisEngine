$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

#ifdef SOURCE_IS_2D_ARRAY
	SAMPLER2DARRAY(s_sourceTexture, 0);
	uniform vec4 u_copyArrayIndex;
#else
	SAMPLER2D(s_sourceTexture, 0);
#endif

void main()
{
#ifdef SOURCE_IS_2D_ARRAY
	gl_FragColor = texture2DArray( s_sourceTexture, vec3( v_texCoord0, u_copyArrayIndex.x ) );
#else
	gl_FragColor = texture2D( s_sourceTexture, v_texCoord0 );
#endif
}
