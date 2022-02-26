// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

/*
#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM

//!!!!не тут
float2 GetEVSMExponents(float positiveExponent, float negativeExponent)
{
    const float maxExponent = 42.0f;

    float2 lightSpaceExponents = float2(positiveExponent, negativeExponent);

    // Clamp to maximum range of fp32/fp16 to prevent overflow/underflow
    return min(lightSpaceExponents, maxExponent);
}

//!!!!не тут
// Applies exponential warp to shadow map depth, input depth should be in [0, 1]
float2 WarpDepth(float depth, float2 exponents)
{
	// Rescale depth into [-1, 1]
	depth = 2.0f * depth - 1.0f;
	float pos = exp( exponents.x * depth);
	float neg = -exp(-exponents.y * depth);
	return float2(pos, neg);
}

vec2 shadowCasterEVSM(vec2 depth, float lightFarClipDistance)
{
	//!!!!

	float PositiveExponent = 40.0f;
	float NegativeExponent = 5.0f;

	//!!!!
	PositiveExponent *= 10.0;
	
	
	float2 exponents = GetEVSMExponents(PositiveExponent, NegativeExponent);

	//!!!!
	float d = depth.x / lightFarClipDistance;
	//float d = depth.x;
	
	float2 vsmDepth = WarpDepth(d, exponents);

	
	
	return vec2(vsmDepth.x, vsmDepth.x * vsmDepth.x);
	


	//!!!!

//	float m1 = depth.x / lightFarClipDistance.x;
//	//float m1 = depth.x;//getDepthValue(depth.x, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);

//	//!!!!
/	//m1 /= 1000.0;

//	float m2 = m1*m1;

//	//    float dx = dFdx(m1);
//	//    float dy = dFdy(m1);
//	//    m2 += 0.25*(dx*dx+dy*dy);

//	return vec2(m1, m2);
	
}
#endif
*/
