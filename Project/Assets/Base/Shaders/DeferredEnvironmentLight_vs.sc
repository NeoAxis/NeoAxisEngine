$input a_position, a_texcoord0
$output v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "UniformsVertex.sh"

void main()
{
	gl_Position = vec4(a_position, 1.0);

	v_texCoord0 = a_texcoord0;
}
