$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

uniform vec4 quarterViewportPixelSize;

SAMPLER2D(s_importanceMap, 0);

void main()
{
    static const float cSmoothenImportance = 1.0;

    float centre = texture2D(s_importanceMap, v_texCoord0).x;

    vec2 halfPixel = quarterViewportPixelSize.xy * 0.5;

    vec4 vals;
    vals.x = texture2D(s_importanceMap, v_texCoord0 + vec2(-halfPixel.x * 3.0, -halfPixel.y)).x;
    vals.y = texture2D(s_importanceMap, v_texCoord0 + vec2( halfPixel.x, -halfPixel.y * 3.0)).x;
    vals.z = texture2D(s_importanceMap, v_texCoord0 + vec2( halfPixel.x * 3.0,  halfPixel.y)).x;
    vals.w = texture2D(s_importanceMap, v_texCoord0 + vec2(-halfPixel.x,  halfPixel.y * 3.0)).x;

    float avgVal = dot(vals, vec4(0.25, 0.25, 0.25, 0.25));
    vals.xy = max(vals.xy, vals.zw);
    float maxVal = max(centre, max(vals.x, vals.y));

    gl_FragColor = vec4(mix(maxVal, avgVal, cSmoothenImportance), 0.0, 0.0, 0.0);
}