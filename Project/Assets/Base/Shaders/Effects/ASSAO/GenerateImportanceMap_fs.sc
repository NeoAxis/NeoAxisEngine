$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

uniform vec4 halfViewportPixelSize;
uniform vec4 effectShadowStrength;
uniform vec4 effectShadowPow;

SAMPLER2DARRAY(s_halfSSAOMapArray, 0);

vec4 FetchSSAOMapArray(vec2 TexCoords, int layer)
{
    vec4 res;

    float l = (float)layer;

    res.x = texture2DArray(s_halfSSAOMapArray, vec3(TexCoords + vec2(0.0,                     halfViewportPixelSize.y), l)).x;
    res.y = texture2DArray(s_halfSSAOMapArray, vec3(TexCoords + vec2(halfViewportPixelSize.x, halfViewportPixelSize.y), l)).x;
    res.z = texture2DArray(s_halfSSAOMapArray, vec3(TexCoords + vec2(halfViewportPixelSize.x, 0.0                    ), l)).x;
    res.w = texture2DArray(s_halfSSAOMapArray, vec3(TexCoords + vec2(0.0,                     0.0                    ), l)).x;

    return res;
}

void main()
{
    uvec2 basePos = ((uvec2)getFragCoord().xy) * 2;

    vec2 gatherUV = (vec2(basePos) + vec2(1.0, 1.0)) * halfViewportPixelSize.xy;

    float avg = 0.0;
    float minV = 1.0;
    float maxV = 0.0;

    for(int i = 0; i < 4; i++)
    {
        vec4 vals = FetchSSAOMapArray(gatherUV, i);

        // apply the same modifications that would have been applied in the main shader:

        vals = vals * effectShadowStrength.x;

        vals = 1.0 - vals;

        vals = pow(saturate(vals), effectShadowPow.x);

        avg += dot(vec4(vals.x, vals.y, vals.z, vals.w), vec4(1.0 / 16.0, 1.0 / 16.0, 1.0 / 16.0, 1.0 / 16.0));

        maxV = max(maxV, max(max(vals.x, vals.y), max(vals.z, vals.w)));
        minV = min(minV, min(min(vals.x, vals.y), min(vals.z, vals.w)));
    }

    float minMaxDiff = maxV - minV;

    gl_FragColor = vec4(pow(saturate(minMaxDiff * 2.0), 0.8), 0.0, 0.0, 0.0);
}