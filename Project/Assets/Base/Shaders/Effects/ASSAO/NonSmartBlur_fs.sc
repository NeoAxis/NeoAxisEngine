$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

uniform vec4 halfViewportPixelSize;

SAMPLER2D(s_SSAOMap, 0);

void main()
{
    vec2 halfPixel = halfViewportPixelSize * 0.5;

    vec2 centre = texture2D(s_SSAOMap, v_texCoord0).xy;
    
    vec4 vals;

    vals.x = texture2D(s_SSAOMap, v_texCoord0 + vec2(-halfPixel.x * 3.0, -halfPixel.y      )).x;
    vals.y = texture2D(s_SSAOMap, v_texCoord0 + vec2( halfPixel.x,       -halfPixel.y * 3.0)).x;
    vals.z = texture2D(s_SSAOMap, v_texCoord0 + vec2(-halfPixel.x,        halfPixel.y * 3.0)).x;
    vals.w = texture2D(s_SSAOMap, v_texCoord0 + vec2( halfPixel.x * 3.0,  halfPixel.y      )).x;

    gl_FragColor = vec4(dot(vals, 0.2.xxxx) + centre.x * 0.2, centre.y, 0.0, 0.0);
}