$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "ASSAO_Helpers.sh"

uniform vec4 NDCToViewMul;
uniform vec4 NDCToViewAdd;
uniform vec4 viewportPixelSize;
uniform vec4 viewport2xPixelSize;
uniform vec4 halfViewportPixelSize;
uniform vec4 effectRadiusParams;
uniform vec4 patternRotScaleMatrices[5];
uniform vec4 perPassFullResCoordOffset;
uniform vec4 effectFadeOutMul;
uniform vec4 effectFadeOutAdd;
uniform vec4 effectShadowStrength;
uniform vec4 effectShadowClamp;
uniform vec4 effectShadowPow;
uniform vec4 detailAOStrength;
uniform mat4 itViewMatrix;

SAMPLER2D(s_depthTextureL0, 0);
SAMPLER2D(s_depthTextureL1, 1);
SAMPLER2D(s_depthTextureL2, 2);
SAMPLER2D(s_depthTextureL3, 3);
SAMPLER2D(s_depthTextureM, 4);
SAMPLER2D(s_normalTexture, 5);

#define SSAO_ADAPTIVE_TAP_BASE_COUNT 5
#define SSAO_HORIZON_ANGLE_THRESHOLD 0.1
#define SSAO_HALOING_REDUCTION_AMOUNT 0.6
#define SSAO_DEPTH_PRECISION_OFFSET_MOD 0.9992
#define SSAO_NORMAL_BASED_EDGES_DOT_THRESHOLD 0.5
#define SSAO_DEPTH_MIPS_GLOBAL_OFFSET -6.0//-4.3
#define MAIN_DISK_SAMPLE_COUNT 32
#define SSAO_TILT_SAMPLES_AMOUNT 0.4

static const vec4 g_samplePatternMain[MAIN_DISK_SAMPLE_COUNT] =
{
     vec4(0.78488064,  0.56661671,  1.500000, -0.126083),     vec4(0.26022232, -0.29575172,  1.500000, -1.064030),     vec4(0.10459357,  0.08372527,  1.110000, -2.730563),    vec4(-0.68286800,  0.04963045,  1.090000, -0.498827),
    vec4(-0.13570161, -0.64190155,  1.250000, -0.532765),    vec4(-0.26193795, -0.08205118,  0.670000, -1.783245),    vec4(-0.61177456,  0.66664219,  0.710000, -0.044234),     vec4(0.43675563,  0.25119025,  0.610000, -1.167283),
     vec4(0.07884444,  0.86618668,  0.640000, -0.459002),    vec4(-0.12790935, -0.29869005,  0.600000, -1.729424),    vec4(-0.04031125,  0.02413622,  0.600000, -4.792042),     vec4(0.16201244, -0.52851415,  0.790000, -1.067055),
    vec4(-0.70991218,  0.47301072,  0.640000, -0.335236),     vec4(0.03277707, -0.22349690,  0.600000, -1.982384),     vec4(0.68921727,  0.36800742,  0.630000, -0.266718),     vec4(0.29251814,  0.37775412,  0.610000, -1.422520),
    vec4(-0.12224089,  0.96582592,  0.600000, -0.426142),     vec4(0.11071457, -0.16131058,  0.600000, -2.165947),     vec4(0.46562141, -0.59747696,  0.600000, -0.189760),    vec4(-0.51548797,  0.11804193,  0.600000, -1.246800),
     vec4(0.89141309, -0.42090443,  0.600000,  0.028192),    vec4(-0.32402530, -0.01591529,  0.600000, -1.543018),     vec4(0.60771245,  0.41635221,  0.600000, -0.605411),     vec4(0.02379565, -0.08239821,  0.600000, -3.809046),
     vec4(0.48951152, -0.23657045,  0.600000, -1.189011),    vec4(-0.17611565, -0.81696892,  0.600000, -0.513724),    vec4(-0.33930185, -0.20732205,  0.600000, -1.698047),    vec4(-0.91974425,  0.05403209,  0.600000,  0.062246),
    vec4(-0.15064627, -0.14949332,  0.600000, -1.896062),     vec4(0.53180975, -0.35210401,  0.600000, -0.758838),     vec4(0.41487166,  0.81442589,  0.600000, -0.505648),    vec4(-0.24106961, -0.32721516,  0.600000, -1.665244)
};

vec3 NDCToViewspace(vec2 pos, float viewspaceDepth)
{
    vec3 ret;

    ret.xy = (NDCToViewMul.xy * pos.xy + NDCToViewAdd.xy) * viewspaceDepth;
    ret.z = viewspaceDepth;

    return ret;
}

float CalculatePixelObscurance(vec3 pixelNormal, vec3 hitDelta, float falloffCalcMulSq)
{
  float lengthSq = dot(hitDelta, hitDelta);
  float NdotD = dot(pixelNormal, hitDelta) / sqrt(lengthSq);

  float falloffMult = max(0.0, lengthSq * falloffCalcMulSq + 1.0);

  return max(0, NdotD - SSAO_HORIZON_ANGLE_THRESHOLD) * falloffMult;
}

float SampleDepthMipMapped(vec2 TexCoords, float mipLevel)
{
    mipLevel = clamp(mipLevel, 0.0, 3.0);
    float d0, d1, mixFactor;

    if (mipLevel <= 1.0) { // [0..1]
        d0 = texture2D(s_depthTextureL0, TexCoords).x;
        d1 = texture2D(s_depthTextureL1, TexCoords).x;
        mixFactor = mipLevel;
    } else
    if (mipLevel <= 2.0) { // [1..2]
        d0 = texture2D(s_depthTextureL1, TexCoords).x;
        d1 = texture2D(s_depthTextureL2, TexCoords).x;
        mixFactor = mipLevel - 1.0;
    } else
    if (mipLevel <= 3.0) { // [2..3]
        d0 = texture2D(s_depthTextureL2, TexCoords).x;
        d1 = texture2D(s_depthTextureL3, TexCoords).x;
        mixFactor = mipLevel - 2.0;
    }

    return mix(d0, d1, mixFactor);
}

vec3 LoadNormal(vec2 TexCoords)
{
    vec3 normal = texture2D(s_normalTexture, TexCoords).xyz * 2.0 - 1.0;
    
    vec3 tnormal = normalize(mul((mat3)itViewMatrix, normal));
    tnormal.z = -tnormal.z;
    tnormal = normalize(tnormal);

    return tnormal;
}

void SSAOTapInner(inout float obscuranceSum, inout float weightSum, const vec2 samplingUV, const float mipLevel, const vec3 pixCenterPos, const vec3 negViewspaceDir, vec3 pixelNormal, const float falloffCalcMulSq, const float weightMod)
{
	float viewspaceSampleZ = SampleDepthMipMapped(samplingUV, mipLevel);

	vec3 hitPos = NDCToViewspace(samplingUV, viewspaceSampleZ);

	vec3 hitDelta = hitPos - pixCenterPos;

	float obscurance = CalculatePixelObscurance(pixelNormal, hitDelta, falloffCalcMulSq);
	float weight = 1.0;

	float reduct = max(0.0, -hitDelta.z);
	reduct = saturate(reduct * effectRadiusParams.y + 2.0);
	weight = SSAO_HALOING_REDUCTION_AMOUNT * reduct + (1.0 - SSAO_HALOING_REDUCTION_AMOUNT);

	weight *= weightMod;
	obscuranceSum += obscurance * weight;
	weightSum += weight;
}

void SSAOTap(inout float obscuranceSum, inout float weightSum, const int tapIndex, const mat2 rotScale, const vec3 pixCenterPos, const vec3 negViewspaceDir, vec3 pixelNormal, const vec2 normalizedScreenPos, const float mipOffset, const float falloffCalcMulSq, float weightMod, vec2 normXY, float normXYLength)
{
	vec2 sampleOffset;
	float samplePow2Len;

	vec4 newSample = g_samplePatternMain[tapIndex];
	sampleOffset = mul(rotScale, newSample.xy);
	samplePow2Len = newSample.w;
	weightMod *= newSample.z;

	sampleOffset = round(sampleOffset);

	float mipLevel = samplePow2Len + mipOffset;

	vec2 samplingUV = sampleOffset * viewport2xPixelSize.xy + normalizedScreenPos;

	SSAOTapInner(obscuranceSum, weightSum, samplingUV, mipLevel, pixCenterPos, negViewspaceDir, pixelNormal, falloffCalcMulSq, weightMod);

	vec2 sampleOffsetMirroredUV = -sampleOffset;
    
	//float dotNorm = dot(sampleOffsetMirroredUV, normXY);
	//sampleOffsetMirroredUV -= dotNorm * normXYLength * normXY;
	//sampleOffsetMirroredUV = round(sampleOffsetMirroredUV);
    
	vec2 samplingMirroredUV = sampleOffsetMirroredUV * viewport2xPixelSize.xy + normalizedScreenPos;

	SSAOTapInner(obscuranceSum, weightSum, samplingMirroredUV, mipLevel, pixCenterPos, negViewspaceDir, pixelNormal, falloffCalcMulSq, weightMod);
}

vec4 CalculateEdges(const float centerZ, const float leftZ, const float rightZ, const float topZ, const float bottomZ)
{
    vec4 edgesLRTB = vec4(leftZ, rightZ, topZ, bottomZ) - centerZ;
    vec4 edgesLRTBSlopeAdjusted = edgesLRTB + edgesLRTB.yxwz;
    edgesLRTB = min(abs(edgesLRTB), abs(edgesLRTBSlopeAdjusted));
    return saturate((1.3 - edgesLRTB / (centerZ * 0.040)));
}

void GenerateSSAOShadowsInternal(out float outShadowTerm, out vec4 outEdges, out float outWeight, const vec2 SVPos, const vec2 TexCoord)
{
    vec2 SVPosRounded = trunc(SVPos);

    const int numberOfTaps = SSAO_ADAPTIVE_TAP_BASE_COUNT;
   
    float  pixZ = texture2D(s_depthTextureM, TexCoord).x;
    float pixLZ = texture2D(s_depthTextureM, TexCoord + vec2(-halfViewportPixelSize.x, 0.0)).x;
    float pixRZ = texture2D(s_depthTextureM, TexCoord + vec2( halfViewportPixelSize.x, 0.0)).x;
    float pixTZ = texture2D(s_depthTextureM, TexCoord + vec2(0.0, -halfViewportPixelSize.y)).x;
    float pixBZ = texture2D(s_depthTextureM, TexCoord + vec2(0.0,  halfViewportPixelSize.y)).x;

    vec2 fullResCoord = TexCoord + (perPassFullResCoordOffset.xy * viewportPixelSize.xy);

    vec3 pixelNormal = LoadNormal(fullResCoord);

    vec3 pixCenterPos = NDCToViewspace(TexCoord, pixZ);

    const vec2 pixelDirRBViewspaceSizeAtCenterZ = NDCToViewMul.xy * viewport2xPixelSize.xy * pixCenterPos.zz;

    float pixLookupRadiusMod;
    float falloffCalcMulSq;
    float effectViewspaceRadius;

    CalculateRadiusParameters(effectRadiusParams.x, effectRadiusParams.z, length(pixCenterPos), pixelDirRBViewspaceSizeAtCenterZ, pixLookupRadiusMod, effectViewspaceRadius, falloffCalcMulSq);

    //float nearScreenBorder = min(min(TexCoord.x, 1.0 - TexCoord.x), min(TexCoord.y, 1.0 - TexCoord.y));
    //nearScreenBorder = saturate(10.0 * nearScreenBorder + 0.6);
    //pixLookupRadiusMod *= nearScreenBorder;
    
    uint pseudoRandomIndex = uint(SVPosRounded.y * 2 + SVPosRounded.x) % 5;

    vec4 rs = patternRotScaleMatrices[pseudoRandomIndex];

#ifdef GLSL
	zz mtxFromRows;
#endif

    mat2 rotScale = mat2(rs.x * pixLookupRadiusMod, rs.y * pixLookupRadiusMod,
                         rs.z * pixLookupRadiusMod, rs.w * pixLookupRadiusMod);

    float obscuranceSum = 0.0;
    float weightSum = 0.0;

    pixCenterPos *= SSAO_DEPTH_PRECISION_OFFSET_MOD;

    float4 edgesLRTB = vec4(1.0, 1.0, 1.0, 1.0);

    float mipOffset = log2(pixLookupRadiusMod) + SSAO_DEPTH_MIPS_GLOBAL_OFFSET;

    vec2 normXY = vec2(pixelNormal.x, pixelNormal.y);
    float normXYLength = length(normXY);
    normXY /= vec2(normXYLength, -normXYLength);
    normXYLength *= SSAO_TILT_SAMPLES_AMOUNT;

    const vec3 negViewspaceDir = -normalize(pixCenterPos);

    for(int i = 0; i < numberOfTaps; i++)
    {
        SSAOTap(obscuranceSum, weightSum, i, rotScale, pixCenterPos, negViewspaceDir, pixelNormal, TexCoord, mipOffset, falloffCalcMulSq, 1.0, normXY, normXYLength);
    }

    float obscurance = obscuranceSum / weightSum;

    outShadowTerm = obscurance;
    outEdges = 0;
    outWeight = weightSum;
}

void main()
{
    float outShadowTerm;
    float outWeight;
    vec4 outEdges;

    GenerateSSAOShadowsInternal(outShadowTerm, outEdges, outWeight, getFragCoord().xy, v_texCoord0);

    gl_FragColor = vec4(outShadowTerm, outWeight / ((float)SSAO_ADAPTIVE_TAP_BASE_COUNT * 4.0), 0.0, 0.0);
}