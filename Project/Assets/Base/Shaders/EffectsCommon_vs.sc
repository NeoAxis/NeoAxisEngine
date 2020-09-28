$input a_position, a_texcoord0
$output v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

void main()
{
#ifdef GLSL
	gl_Position = vec4(a_position.x, -a_position.y, 0.0, 1.0);
#else
	gl_Position = vec4(a_position, 1.0);
#endif	
	
	//gl_Position = vec4(a_position.x, -a_position.y, 0.0, 1.0);
	//gl_Position = mul(u_viewProj, vec4(a_position.x, 1.0 - a_position.y, 0.0, 1.0) );
	//gl_Position = mul(u_viewProj, vec4(a_position.x, 1.0 - a_position.y, 0.0, 1.0) );
	//gl_Position = mul(u_viewProj, vec4(a_position.xy, 0.0, 1.0) );

/*	
	mat4 worldMatrix = u_model[0];
	vec4 worldPosition = mul(worldMatrix, vec4(a_position, 1.0));
	//vec4 worldPosition = mul(u_model[0], vec4(a_position, 1.0));

	gl_Position = mul(u_modelViewProj, vec4(a_position, 1.0));
*/

	
	
	//gl_Position = vec4(a_position, 1.0);
	
	v_texCoord0 = a_texcoord0;
}
