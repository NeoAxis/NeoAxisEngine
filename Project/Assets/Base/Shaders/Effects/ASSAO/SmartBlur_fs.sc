$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "ASSAO_Helpers.sh"

uniform vec4 halfViewportPixelSize;
uniform vec4 invSharpness;

SAMPLER2D(s_SSAOMap, 0);

void main()
{
    vec2 vC = texture2D(s_SSAOMap, v_texCoord0 + vec2( 0.0,                            0.0)).xy;
    vec2 vL = texture2D(s_SSAOMap, v_texCoord0 + vec2(-1.0 * halfViewportPixelSize.x,  0.0)).xy;
    vec2 vT = texture2D(s_SSAOMap, v_texCoord0 + vec2( 0.0, -1.0 * halfViewportPixelSize.y)).xy;
    vec2 vR = texture2D(s_SSAOMap, v_texCoord0 + vec2( 1.0 * halfViewportPixelSize.x,  0.0)).xy;
    vec2 vB = texture2D(s_SSAOMap, v_texCoord0 + vec2( 0.0,  1.0 * halfViewportPixelSize.y)).xy;

    float packedEdges = vC.y;
    vec4 edgesLRTB = UnpackEdges(packedEdges, invSharpness.x);

    float ssaoValue  = vC.x;
    float ssaoValueL = vL.x;
    float ssaoValueT = vT.x;
    float ssaoValueR = vR.x;
    float ssaoValueB = vB.x;

    float sumWeight = 0.5f;
    float sum = ssaoValue * sumWeight;

    AddSample(ssaoValueL, edgesLRTB.x, sum, sumWeight);
    AddSample(ssaoValueR, edgesLRTB.y, sum, sumWeight);
    AddSample(ssaoValueT, edgesLRTB.z, sum, sumWeight);
    AddSample(ssaoValueB, edgesLRTB.w, sum, sumWeight);

    float ssaoAvg = sum / sumWeight;

    ssaoValue = ssaoAvg;

    gl_FragColor = vec4(ssaoValue, packedEdges, 0.0, 0.0);
}