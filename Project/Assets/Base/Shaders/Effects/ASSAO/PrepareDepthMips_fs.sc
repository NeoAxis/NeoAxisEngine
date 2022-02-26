$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "ASSAO_Helpers.sh"

uniform vec4 prevMipParams;
uniform vec4 effectRadiusParams;

SAMPLER2D(s_halfDepthTexture0, 0);
SAMPLER2D(s_halfDepthTexture1, 1);
SAMPLER2D(s_halfDepthTexture2, 2);
SAMPLER2D(s_halfDepthTexture3, 3);

void main()
{
    vec4 depthsArr[4];
    float depthsOutArr[4];

    depthsArr[0].x = texture2D(s_halfDepthTexture0, v_texCoord0 + vec2(0.0,             0.0            )).x;
    depthsArr[0].y = texture2D(s_halfDepthTexture0, v_texCoord0 + vec2(prevMipParams.x, 0.0            )).x;
    depthsArr[0].z = texture2D(s_halfDepthTexture0, v_texCoord0 + vec2(0.0,             prevMipParams.y)).x;
    depthsArr[0].w = texture2D(s_halfDepthTexture0, v_texCoord0 + vec2(prevMipParams.x, prevMipParams.y)).x;

    depthsArr[1].x = texture2D(s_halfDepthTexture1, v_texCoord0 + vec2(0.0,             0.0            )).x;
    depthsArr[1].y = texture2D(s_halfDepthTexture1, v_texCoord0 + vec2(prevMipParams.x, 0.0            )).x;
    depthsArr[1].z = texture2D(s_halfDepthTexture1, v_texCoord0 + vec2(0.0,             prevMipParams.y)).x;
    depthsArr[1].w = texture2D(s_halfDepthTexture1, v_texCoord0 + vec2(prevMipParams.x, prevMipParams.y)).x;

    depthsArr[2].x = texture2D(s_halfDepthTexture2, v_texCoord0 + vec2(0.0,             0.0            )).x;
    depthsArr[2].y = texture2D(s_halfDepthTexture2, v_texCoord0 + vec2(prevMipParams.x, 0.0            )).x;
    depthsArr[2].z = texture2D(s_halfDepthTexture2, v_texCoord0 + vec2(0.0,             prevMipParams.y)).x;
    depthsArr[2].w = texture2D(s_halfDepthTexture2, v_texCoord0 + vec2(prevMipParams.x, prevMipParams.y)).x;

    depthsArr[3].x = texture2D(s_halfDepthTexture3, v_texCoord0 + vec2(0.0,             0.0            )).x;
    depthsArr[3].y = texture2D(s_halfDepthTexture3, v_texCoord0 + vec2(prevMipParams.x, 0.0            )).x;
    depthsArr[3].z = texture2D(s_halfDepthTexture3, v_texCoord0 + vec2(0.0,             prevMipParams.y)).x;
    depthsArr[3].w = texture2D(s_halfDepthTexture3, v_texCoord0 + vec2(prevMipParams.x, prevMipParams.y)).x;

    float falloffCalcMulSq, falloffCalcAdd;
    float dummyUnused1, dummyUnused2;
 
    for(int i = 0; i < 4; i++)
    {
        vec4 depths = depthsArr[i];

        float closest = min(min(depths.x, depths.y), min(depths.z, depths.w));

        CalculateRadiusParameters(effectRadiusParams.x, effectRadiusParams.z, abs(closest), 1.0, dummyUnused1, dummyUnused2, falloffCalcMulSq);

        vec4 dists = depths - closest.xxxx;

        vec4 weights = saturate(dists * dists * falloffCalcMulSq + 1.0);

        float smartAvg = dot(weights, depths) / dot(weights, vec4(1.0, 1.0, 1.0, 1.0));

        depthsOutArr[i] = smartAvg;
    }

    gl_FragData[0] = vec4(depthsOutArr[0], 0.0, 0.0, 0.0);
    gl_FragData[1] = vec4(depthsOutArr[1], 0.0, 0.0, 0.0);
    gl_FragData[2] = vec4(depthsOutArr[2], 0.0, 0.0, 0.0);
    gl_FragData[3] = vec4(depthsOutArr[3], 0.0, 0.0, 0.0);
}