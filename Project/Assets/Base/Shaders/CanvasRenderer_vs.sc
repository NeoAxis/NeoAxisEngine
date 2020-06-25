$input a_position, a_color0, a_texcoord0
$output v_color0, v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

void main()
{
	//gl_Position = vec4(a_position.x * 2 - 1, (a_position.y * 2 - 1) * -1, 0, 1.0);
	gl_Position = vec4(a_position, 1.0);

	//gl_Position = mul(u_modelViewProj, vec4(a_position, 1.0) );
	v_texCoord0 = a_texcoord0;
	v_color0 = a_color0;
}
