$input a_position
$output v_texCoord0

// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "Common.sh"

void main()
{      
	gl_Position = mul(u_modelViewProj, vec4(a_position, 1.0)).xyww;
	// thus z will be 1.0 after perspective divide

	v_texCoord0 = a_position.xyz;
}
