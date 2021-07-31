$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

uniform vec4 quarterViewportPixelSize;

SAMPLER2D(s_importanceMap, 0);

void main()
{
    vec2 uv = vec2(0.0, 0.0);
    float sum = 0.0;

    while (uv.y <= 1.0) {

        while (uv.x <= 1.0) {
            sum += texture2D(s_importanceMap, uv).x;
            uv.x += quarterViewportPixelSize.x;
        }

        uv.y += quarterViewportPixelSize.y;
    }

    gl_FragColor = vec4(sum / quarterViewportPixelSize.z, 0.0, 0.0, 0.0);
}