$input a_position, a_texcoord0
$output v_texCoord0

// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "Common.sh"

void main()
{
	gl_Position = vec4(a_position, 1.0);
	v_texCoord0 = a_texcoord0;
}
