void CalculateRadiusParameters(float startEffectRadius, float effectSamplingRadiusNearLimitRec, const float pixCenterLength, const vec2 pixelDirRBViewspaceSizeAtCenterZ, out float pixLookupRadiusMod, out float effectRadius, out float falloffCalcMulSq)
{
    effectRadius = startEffectRadius;

    const float tooCloseLimitMod = saturate(pixCenterLength * effectSamplingRadiusNearLimitRec) * 0.8 + 0.2;
    
    effectRadius *= tooCloseLimitMod;

    pixLookupRadiusMod = (0.85 * effectRadius) / pixelDirRBViewspaceSizeAtCenterZ.x;

    falloffCalcMulSq= -1.0 / (effectRadius * effectRadius);
}

vec4 UnpackEdges(float _packedVal, float invSharpness)
{
    uint packedVal = (uint)(_packedVal * 255.5);

    vec4 edgesLRTB;
    edgesLRTB.x = float((packedVal >> 6) & 0x03) / 3.0;
    edgesLRTB.y = float((packedVal >> 4) & 0x03) / 3.0;
    edgesLRTB.z = float((packedVal >> 2) & 0x03) / 3.0;
    edgesLRTB.w = float((packedVal >> 0) & 0x03) / 3.0;

    return saturate(edgesLRTB + invSharpness);
}

void AddSample(float ssaoValue, float edgeValue, inout float sum, inout float sumWeight)
{
    float weight = edgeValue;    

    sum += (weight * ssaoValue);
    sumWeight += weight;
}