$input v_texCoord0

// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	gl_FragColor = sourceColor;
}
