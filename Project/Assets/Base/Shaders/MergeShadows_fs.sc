$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

uniform vec4 u_mergeShadowsParams;

//#ifdef POINT_LIGHT
SAMPLER2DARRAY(s_sourceTexture1, 0);
SAMPLER2DARRAY(s_sourceTexture2, 1);
//#else
//SAMPLER2D(s_sourceTexture1, 0);
//SAMPLER2D(s_sourceTexture2, 1);
//#endif

void main()
{
#ifdef SHADOW_TEXTURE_FORMAT_BYTE4

	//#ifdef POINT_LIGHT
		float index = u_mergeShadowsParams[ 0 ];
		float v1 = unpackRgbaToFloat( texture2DArray( s_sourceTexture1, vec3( v_texCoord0, index ) ) );
		float v2 = unpackRgbaToFloat( texture2DArray( s_sourceTexture2, vec3( v_texCoord0, index ) ) );
		gl_FragColor = packFloatToRgba( min( v1, v2 ) );
		/*
	#else	
		float v1 = unpackRgbaToFloat( texture2D( s_sourceTexture1, v_texCoord0 ) );
		float v2 = unpackRgbaToFloat( texture2D( s_sourceTexture2, v_texCoord0 ) );
		gl_FragColor = packFloatToRgba( min( v1, v2 ) );
	#endif
		*/
	
#else
	
	//#ifdef POINT_LIGHT
		float index = u_mergeShadowsParams[ 0 ];
		float v1 = texture2DArray( s_sourceTexture1, vec3( v_texCoord0, index ) ).x;
		float v2 = texture2DArray( s_sourceTexture2, vec3( v_texCoord0, index ) ).x;
		gl_FragColor = vec4( min( v1, v2 ), 0, 0, 0 );	
		/*
	#else	
		float v1 = texture2D( s_sourceTexture1, v_texCoord0 ).x;
		float v2 = texture2D( s_sourceTexture2, v_texCoord0 ).x;
		gl_FragColor = vec4( min( v1, v2 ), 0, 0, 0 );	
	#endif
		*/

#endif
}
