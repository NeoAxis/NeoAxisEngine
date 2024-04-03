$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "../../FragmentFunctions.sh"

SAMPLER2D( s_ssrDebug, 0 );
SAMPLER2DARRAY( s_ssrTexture, 1 );

uniform vec4 u_ssrDebugParameters;

void main()
{
	vec4 value;

	if( u_ssrDebugParameters.x >= 0.0 )
		value = texture2DArray( s_ssrTexture, vec3( v_texCoord0, u_ssrDebugParameters.x ) );
	else
		value = texture2D( s_ssrDebug, v_texCoord0 );		

	vec3 v2 = mix( vec3( 0, 0, 0 ), value.xyz, value.w );
	if( u_ssrDebugParameters.y != 0.0 )
		v2 = value.xyz;
	
	gl_FragColor = vec4( v2, 1.0 );
}
