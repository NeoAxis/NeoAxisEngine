
// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

uniform vec4/*vec3*/ backgroundColor;

void main()
{
	gl_FragColor = vec4(backgroundColor.rgb, 1);
}
