
// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "Common.sh"

uniform vec4/*vec3*/ backgroundColor;

void main()
{
	gl_FragColor = vec4(backgroundColor.rgb, 1);
}
