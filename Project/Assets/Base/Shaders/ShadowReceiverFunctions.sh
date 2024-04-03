// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#ifdef SHADOW_CONTACT

/*
float getDepth(vec2 uv)
{
	float rawDepth = texture2D(s_depthTexture, uv).r;
	return getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);	
}
*/

float getCameraDistance( vec2 uv )
{
	float rawDepth = texture2DLod(s_depthTexture, uv, 0.0).r;
	vec3 worldPosition = reconstructWorldPosition(u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, uv, rawDepth);
	//vec3 worldPosition = reconstructWorldPosition(invViewProj, uv, rawDepth);
	return length(u_viewportOwnerCameraPosition.xyz - worldPosition);	
}

float screenFade( vec2 uv )
{
	vec2 fade = max(vec2_splat(0.0f), 12.0f * abs(uv - vec2_splat(0.5f)) - vec2_splat(5.0f));
	return saturate(1.0f - dot(fade, fade));
}

float screenSpaceShadows( vec3 surfaceWorldPosition, vec3 directionFromPixelToLight, vec2 surfaceUV, int nLight )
{
	const int stepCount = 12;
	const float stepCountInv = 1.0 / float(stepCount);
		
	vec3 worldFrom = surfaceWorldPosition;
	vec3 worldTo = surfaceWorldPosition + directionFromPixelToLight * d_lightShadowContactLength;
	
	vec2 uvFrom = surfaceUV;
	vec2 uvTo = projectToScreenCoordinates(u_viewportOwnerViewProjection, worldTo);
	//vec2 uvTo = projectToScreenCoordinates(viewProj, worldTo);
	
	vec2 uvStep = (uvTo - uvFrom) * stepCountInv;
	vec3 worldPositionStep = (worldTo - worldFrom) * stepCountInv;
	
	
	vec2 uvRay = uvFrom;
	vec3 worldPositionRay = worldFrom;
	
	float result = 0.0;	

	//LOOP
	for(int n=0;n<stepCount;n++)
	{
		uvRay += uvStep;
		worldPositionRay += worldPositionStep;
		
		float cameraDistanceRay = length(u_viewportOwnerCameraPosition.xyz - worldPositionRay);

		vec2 uv = uvRay;
		//another way to use world position. it is slowler, however can be better in some cases
		//vec2 uv = projectToScreenCoordinates(viewProj, worldPositionRay);

		if(uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
			break;
		
		float depthBufferCameraDistance = getCameraDistance(uv);
		
		float depthDelta = cameraDistanceRay - depthBufferCameraDistance;
		
		//!!!!
		float thickness = d_lightShadowContactLength;
		
		//!!!!
		if( depthDelta > 0.02 && abs(depthDelta) < thickness )
		//if( depthDelta > 0.0 && abs(depthDelta) < thickness )
		//if( depthDelta > 0.0 && depthDelta < thickness )
		{
			return screenFade(uv);
			//return 1.0;
		}


		


	//float depthFrom = getDepth( uvFrom );
	//float depthTo = getDepth( uvTo );
	//float depthStep = (depthTo - depthFrom) * 0.1;

	//vec2 depthCurrent = depthFrom;
	
	//float cameraDistanceOriginal = length(u_viewportOwnerCameraPosition.xyz - worldFrom);
	
	//{
		
		//vec2 uv = uvFrom + uvStep * float(n);
		//vec2 depth = depthFrom + depthStep * float(n);
	
		//float d = getDepth(uvCurrent);
		
		/*
		float cameraDistance = getCameraDistance(uvRay);
		//float rawDepth = texture2D(s_depthTexture, uvRay).r;
		//vec3 worldPosition = reconstructWorldPosition(u_invViewProj, uvRay, rawDepth);
		//float cameraDistance = length(u_viewportOwnerCameraPosition.xyz - worldPosition);
		
		//if(n == 0)
		//	cameraDistanceOriginal = cameraDistance;
		
		float depthDelta = cameraDistanceRay - cameraDistance;
		
		//!!!!
		bool occluded_by_the_original_pixel = abs(cameraDistanceRay - cameraDistanceOriginal) < 0.005;// g_sss_max_delta_from_original_depth;
		
		//!!!!
		float const_thickness = u_lightShadowContactLength;
		
		if( depthDelta > 0.0 && depthDelta < const_thickness && occluded_by_the_original_pixel )
		//if( depthDelta > 0.0 && depthDelta < const_thickness )//&& occluded_by_the_original_pixel )
		//if( depthDelta > 0.0 )//!!!!&& depthDelta < const_thickness )//&& occluded_by_the_original_pixel )
		{
			return 1.0;
		}
		*/
		
		//if( cameraDistance < cameraDistanceCurrent )
		//	return 0.0;
				
		//if( d > depthCurrent )
		//	return 0.0;		
		
		//depthCurrent += depthStep;
	}
	
	return 0.0;
}

#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Directional and Spot lights

//#ifdef GLOBAL_SHADOW_TECHNIQUE_SIMPLE

float getShadowValueSimpleDirectional( int cascadeIndex, vec4 shadowUV, int nLight )
{
	float compareDepth = shadowUV.z / d_lightShadowMapFarClipDistance;
	vec2 texCoord = shadowUV.xy / shadowUV.w;

	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		vec4 shadowValue = texture2DArrayLod( s_shadowMapShadowDirectional, vec3( texCoord, cascadeIndex ), 0 );
		MEDIUMP float shadowFactor = compareDepth < unpackRgbaToFloat( shadowValue ) ? 1.0 : 0.0;
	#else	
		MEDIUMP float shadowFactor = shadow2DArray( s_shadowMapShadowDirectional, vec4( texCoord, cascadeIndex, compareDepth ) ).r;		
	#endif
	
//#ifdef REVERSEDZ
//	return shadowFactor;
//#else
	return 1.0 - shadowFactor;
//#endif
}

float getShadowValueSimpleSpot( float shadowMapIndex, vec4 shadowUV, int nLight )
{
	float compareDepth = shadowUV.z / d_lightShadowMapFarClipDistance;
	vec2 texCoord = shadowUV.xy / shadowUV.w;

//!!!!
//#ifdef GLSL
//	float shadowFactor = 0.0;
//#else	
	
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		vec4 shadowValue = texture2DArrayLod( s_shadowMapShadowSpot, vec3( texCoord, shadowMapIndex ), 0 );
		MEDIUMP float shadowFactor = compareDepth < unpackRgbaToFloat( shadowValue ) ? 1.0 : 0.0;
	#else	
		MEDIUMP float shadowFactor = shadow2DArray( s_shadowMapShadowSpot, vec4( texCoord, shadowMapIndex, compareDepth ) ).r;
	#endif
	
//#endif

//#ifdef REVERSEDZ
//	return shadowFactor;
//#else
	return 1.0 - shadowFactor;
//#endif
}

//#endif

#if defined(GLOBAL_SHADOW_TECHNIQUE_PCF4) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF8) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF12) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF16) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF22) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF32)

//!!!!

//!!!!
/*float avgBlockersDepthToPenumbra(float shadowMapView, float avgBlockersDepth)
{
  float penumbra = (shadowMapView - avgBlockersDepth) / avgBlockersDepth;
  penumbra *= penumbra;
  return saturate(80.0f * penumbra);
}*/

/*
//!!!!original
float avgBlockersDepthToPenumbra(float lightSize, float shadowMapView, float avgBlockersDepth)
{
  float penumbra = lightSize * (shadowMapView - avgBlockersDepth) / avgBlockersDepth;
  return penumbra;
}

float calculatePenumbra(sampler2DArrayShadow shadowMapArray, vec2 shadowMapUV, int cascadeIndex, float shadowMapView, int samplesCount)
{
	float avgBlockersDepth = 0.0f;
	float blockersCount = 0.0f;

	
	//!!!!
	float penumbraFilterMaxSize = 4.0 / 1024.0;
	//float penumbraFilterMaxSize = 8.0 / 1024.0;
	//float penumbraFilterMaxSize = 1.0 * u_lightSourceRadiusOrAngle;
	
	
	//!!!!UNROLL
	for(int i = 0; i < samplesCount; i ++)
	{
		//!!!!noise
		vec2 sampleUV = vogelDiskSample(i, samplesCount, 0.0);// gradientNoise);
		//vec2 sampleUV = VogelDiskOffset(i, samplesCount, gradientNoise);
		sampleUV = shadowMapUV + penumbraFilterMaxSize * sampleUV;

		//!!!!GLSL
		
		//!!!!
				
#ifdef DEFERRED_DIRECT_LIGHT
		float sampleDepth = texture2DArray(s_shadowMapShadow2, vec3(sampleUV, cascadeIndex)).x;
#else
		float sampleDepth = 0;
#endif
		
		//return _sampler.m_texture.Sample(_sampler.m_sampler, _coord);
		//float sampleDepth = shadowMapArray.m_texture.Sample(shadowMapArray.m_sampler, vec3(sampleUV, cascadeIndex)).x;
		//float sampleDepth = texture2DArray(shadowMapArray, vec3(sampleUV, cascadeIndex)).x;
		//float shadowFactor = shadow2DArray(shadowMapArray, vec4(texCoord, cascadeIndex, compareDepth)).r;
		
		//float sampleDepth = shadowMapTexture.SampleLevel(pointClampSampler, sampleUV, 0).x;

		if(sampleDepth < shadowMapView)
		{
			avgBlockersDepth += sampleDepth;
			blockersCount += 1.0f;
		}
	}

	if(blockersCount > 0.0f)
	{
		avgBlockersDepth /= blockersCount;
		
		//float penumbra = shadowMapView - avgBlockersDepth;
		
		//penumbra *= u_lightShadowMapFarClipDistance;
		
		//penumbra *= u_lightSourceRadiusOrAngle;

//!!!!		
//#if defined(USE_FALLOFF)
//	penumbra = 1.0 - pow(1.0 - penumbra, SoftnessFalloff);
//#endif
		
		return avgBlockersDepthToPenumbra(u_lightSourceRadiusOrAngle, shadowMapView, avgBlockersDepth);
		
		//return penumbra;
		
		//return avgBlockersDepthToPenumbra(shadowMapView, penumbra);//avgBlockersDepth
		
		//return penumbra;
		//return saturate(penumbra);
		//return 1.0 - saturate(penumbra);
		
		//return avgBlockersDepthToPenumbra(shadowMapView, avgBlockersDepth);
	}
	else
	{
		//!!!!
		return 1.0;
		//return 0.0f;
	}
}
*/

//!!!!
/*
float calculatePenumbra(sampler2DArrayShadow shadowMapArray, vec2 shadowMapUV, int cascadeIndex, float shadowMapView, int samplesCount)
{
	float avgBlockersDepth = 0.0f;
	float blockersCount = 0.0f;

	
	//!!!!
	float penumbraFilterMaxSize = 4.0 / 2048.0;
	//float penumbraFilterMaxSize = 8.0 / 1024.0;
	//float penumbraFilterMaxSize = 1.0 * u_lightSourceRadiusOrAngle;
	
	
	//!!!!UNROLL
	for(int i = 0; i < samplesCount; i ++)
	{
		//!!!!noise
		vec2 sampleUV = vogelDiskSample(i, samplesCount, 0.0);// gradientNoise);
		//vec2 sampleUV = VogelDiskOffset(i, samplesCount, gradientNoise);
		sampleUV = shadowMapUV + penumbraFilterMaxSize * sampleUV;

		//!!!!GLSL
		
		//!!!!
				
#ifdef DEFERRED_DIRECT_LIGHT
		float sampleDepth = texture2DArray(s_shadowMapShadow2, vec3(sampleUV, cascadeIndex)).x;
#else
		float sampleDepth = 0;
#endif
		
		//return _sampler.m_texture.Sample(_sampler.m_sampler, _coord);
		//float sampleDepth = shadowMapArray.m_texture.Sample(shadowMapArray.m_sampler, vec3(sampleUV, cascadeIndex)).x;
		//float sampleDepth = texture2DArray(shadowMapArray, vec3(sampleUV, cascadeIndex)).x;
		//float shadowFactor = shadow2DArray(shadowMapArray, vec4(texCoord, cascadeIndex, compareDepth)).r;
		
		//float sampleDepth = shadowMapTexture.SampleLevel(pointClampSampler, sampleUV, 0).x;

		if(sampleDepth < shadowMapView)
		{
			avgBlockersDepth += sampleDepth;
			blockersCount += 1.0f;
		}
	}

	if(blockersCount > 0.0f)
	{
		avgBlockersDepth /= blockersCount;
		
		//!!!!
		//float shadowMapViewD = shadowMapView;
		//float avgBlockersDepthD = avgBlockersDepth;
		float shadowMapViewD = getDepthValue(shadowMapView * u_lightShadowMapFarClipDistance, u_viewportOwnerNearClipDistance, u_lightShadowMapFarClipDistance);
		float avgBlockersDepthD = getDepthValue(avgBlockersDepth * u_lightShadowMapFarClipDistance, u_viewportOwnerNearClipDistance, u_lightShadowMapFarClipDistance);
		
		//float lightSize = 1.0;
		//float penumbra = lightSize * (shadowMapView - avgBlockersDepth) / avgBlockersDepth;
		
		float diff = shadowMapViewD - avgBlockersDepthD;
		
		//diff *= 100000;
		
		//penumbra *= u_lightShadowMapFarClipDistance;

		float penumbra;
		
		//if(diff > 2.0 && diff < 3.0)
		if(diff > 1.0)
			penumbra = 3.0;
		else
			penumbra = 1.0;
			
//		if(abs(avgBlockersDepth) < 0.001)
//			penumbra = 5.0;			
			
		//penumbra = 3.0;
		
		//if(penumbra > 3.0)
		//	penumbra = 3.0;
			
		return penumbra;
		
		//penumbra *= u_lightSourceRadiusOrAngle;

//!!!!		
//#if defined(USE_FALLOFF)
//	penumbra = 1.0 - pow(1.0 - penumbra, SoftnessFalloff);
//#endif
		
		//return avgBlockersDepthToPenumbra(u_lightSourceRadiusOrAngle, shadowMapView, avgBlockersDepth);
		
		//return penumbra;
		
		//return avgBlockersDepthToPenumbra(shadowMapView, penumbra);//avgBlockersDepth
		
		//return penumbra;
		//return saturate(penumbra);
		//return 1.0 - saturate(penumbra);
		
		//return avgBlockersDepthToPenumbra(shadowMapView, avgBlockersDepth);
	}
	else
	{
		//!!!!
		return 1.0;
		//return 0.0f;
	}
}
*/



float getShadowValuePCFDirectional( int cascadeIndex, vec4 shadowUV, vec4 fragCoord, int nLight )
{
	float compareDepth = shadowUV.z / d_lightShadowMapFarClipDistance;
	vec2 shadowUVScaled = shadowUV.xy / shadowUV.w;

	MEDIUMP float softness = d_lightShadowSoftness;
	softness /= float(cascadeIndex + 1);
	//no linear interpolation when packed format is used. make softness smaller
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		softness *= 0.75;
	#endif
	
	MEDIUMP float scale = 2.0f / d_lightShadowTextureSize * softness;//d_lightShadowSoftness;
	
	//!!!!
	//scale *= d_lightSourceRadiusOrAngle;

#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF4
	const int sampleCount = 4;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF8
	const int sampleCount = 8;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF12
	const int sampleCount = 12;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF16
	const int sampleCount = 16;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF22
	const int sampleCount = 22;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF32
	const int sampleCount = 32;
#endif

	//!!!!	
	MEDIUMP float penumbra = 1.0;
	//!!!!multiple to Softness
	
	/*
	//!!!!
	float shadowMapView = compareDepth;
	//!!!!
	penumbra = calculatePenumbra(s_shadowMapShadow, shadowUVScaled, cascadeIndex, shadowMapView, 16);
	if(penumbra < 1.0)
		penumbra = 1.0;
	*/

	
	MEDIUMP float shadow = 0.0;
	UNROLL
	for(int n = 0; n < sampleCount; n++)
	{
		vec2 texCoord = shadowUVScaled.xy + vogelDiskSample(n, sampleCount, 0.0) * scale * penumbra;

	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		vec4 shadowValue = texture2DArrayLod( s_shadowMapShadowDirectional, vec3( texCoord, cascadeIndex ), 0 );
		MEDIUMP float shadowFactor = compareDepth < unpackRgbaToFloat( shadowValue ) ? 1.0 : 0.0;	
	#else		
		MEDIUMP float shadowFactor = shadow2DArray( s_shadowMapShadowDirectional, vec4( texCoord, cascadeIndex, compareDepth ) ).r;
	#endif
		
		shadow += shadowFactor;
	}

//#ifdef REVERSEDZ
//	return shadow / float(sampleCount);
//#else
	return 1.0 - shadow / float(sampleCount);
//#endif
}

float getShadowValuePCFSpot( float shadowMapIndex, vec4 shadowUV, vec4 fragCoord, int nLight )
{
	float compareDepth = shadowUV.z / d_lightShadowMapFarClipDistance;
	vec2 shadowUVScaled = shadowUV.xy / shadowUV.w;

	MEDIUMP float softness = d_lightShadowSoftness;
	//no linear interpolation when packed format is used. make softness smaller
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		softness *= 0.75;
	#endif
	
	MEDIUMP float scale = 2.0f / d_lightShadowTextureSize * softness;//d_lightShadowSoftness;
	
	//!!!!
	//scale *= d_lightSourceRadiusOrAngle;

#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF4
	const int sampleCount = 4;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF8
	const int sampleCount = 8;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF12
	const int sampleCount = 12;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF16
	const int sampleCount = 16;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF22
	const int sampleCount = 22;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF32
	const int sampleCount = 32;
#endif

	//!!!!	
	MEDIUMP float penumbra = 1.0;
	//!!!!multiple to Softness
	
	/*
	//!!!!
	float shadowMapView = compareDepth;
	//!!!!
	penumbra = calculatePenumbra(s_shadowMapShadow, shadowUVScaled, cascadeIndex, shadowMapView, 16);
	if(penumbra < 1.0)
		penumbra = 1.0;
	*/

	
	MEDIUMP float shadow = 0.0;
	UNROLL
	for(int n = 0; n < sampleCount; n++)
	{
		vec2 texCoord = shadowUVScaled.xy + vogelDiskSample(n, sampleCount, 0.0) * scale * penumbra;

	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		vec4 shadowValue = texture2DArrayLod( s_shadowMapShadowSpot, vec3( texCoord, shadowMapIndex ), 0 );
		MEDIUMP float shadowFactor = compareDepth < unpackRgbaToFloat( shadowValue ) ? 1.0 : 0.0;
	#else		
		MEDIUMP float shadowFactor = shadow2DArray( s_shadowMapShadowSpot, vec4( texCoord, shadowMapIndex, compareDepth ) ).r;
	#endif
	
		shadow += shadowFactor;
	}

//#ifdef REVERSEDZ
//	return shadow / float(sampleCount);
//#else
	return 1.0 - shadow / float(sampleCount);
//#endif
}

#endif

/*
#ifdef GLOBAL_SHADOW_TECHNIQUE_CHS

//!!!!
// technique - all
static const uint   g_shadow_samples                 = 3;
static const float  g_shadow_filter_size             = 3.0f;
static const float  g_shadow_cascade_blend_threshold = 0.8f;
// technique - vogel
static const uint   g_penumbra_samples                = 8;
static const float  g_penumbra_filter_size            = 128.0f;
// technique - pre-calculated
static const float g_pcf_filter_size    = (sqrt((float)g_shadow_samples) - 1.0f) / 2.0f;
static const float g_shadow_samples_rpc = 1.0f / (float)g_shadow_samples;

static const float FLT_MIN             = 0.00000001f;
static const float FLT_MAX_16          = 32767.0f;

float compute_penumbra(sampler2DArrayShadow shadowMapArray, float vogel_angle, vec2 uv, int cascadeIndex, float compare)
//float compute_penumbra(float vogel_angle, float3 uv, int cascadeIndex, float compare)
{
	float penumbra = 1.0f;
	float blocker_depth_avg = 0.0f;
	uint blocker_count = 0;

	//!!!!
	float g_shadow_texel_size = 1.0f / u_lightShadowTextureSize;
	
	for(uint i = 0; i < g_penumbra_samples; i ++)
	{
		vec2 offset = vogelDiskSample(i, g_penumbra_samples, vogel_angle) * g_shadow_texel_size * g_penumbra_filter_size;

		//!!!!GLSL
		float depth = shadow2DArray(shadowMapArray, vec4(uv + offset, cascadeIndex, compare)).r;
		//float depth = shadow_sample_depth(uv + float3(offset, 0.0f));

		if(depth > compare)
		{
			blocker_depth_avg += depth;
			blocker_count++;
		}
	}

	if (blocker_count != 0)
	{
		blocker_depth_avg /= (float)blocker_count;

		// Compute penumbra
		penumbra = (compare - blocker_depth_avg) / (blocker_depth_avg + FLT_MIN);
		penumbra *= penumbra;
		penumbra *= 10.0f;
	}

	return clamp(penumbra, 1.0f, FLT_MAX_16);
}


float getShadowValueCHS(sampler2DArrayShadow shadowMapArray, int cascadeIndex, vec4 shadowUV)
{
	float compareDepth = shadowUV.z / u_lightShadowMapFarClipDistance;
	vec2 shadowUVScaled = shadowUV.xy / shadowUV.w;
	
	//!!!!
	float g_shadow_texel_size = 1.0f / u_lightShadowTextureSize;
	
	
	//!!!!
	vec2 uv = shadowUVScaled;
	float compare = compareDepth;

	
	float shadow = 0.0f;
	//!!!!!surface.uv?
	//!!!!!!
	//!!!!!!
	//!!!!!!
	//!!!!!!
	//!!!!!!
	//!!!!!!
	//!!!!!!
	float temporal_offset = get_noise_interleaved_gradient();//uv * g_resolution_rt);
	//float temporal_offset = get_noise_interleaved_gradient(surface.uv * g_resolution_rt);
	float temporal_angle  = temporal_offset * PI * 2.0;

	float penumbra;
	#ifdef LIGHT_TYPE_DIRECTIONAL
		penumbra = 1.0;
	#else
		penumbra = compute_penumbra(shadowMapArray, temporal_angle, uv, cascadeIndex, compare);
	#endif

	for (uint i = 0; i < g_shadow_samples; i++)
	{
		vec2 offset = vogelDiskSample(i, g_shadow_samples, temporal_angle) * g_shadow_texel_size * g_shadow_filter_size * penumbra;

		//!!!!GLSL
		shadow += shadow2DArray(shadowMapArray, vec4(uv + offset, cascadeIndex, compare)).r;
		//shadow += shadow_compare_depth(uv + float3(offset, 0.0f), compare);
	} 

	return 1.0 - saturate(shadow * g_shadow_samples_rpc);
}
#endif
*/


/*#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM

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

//!!!!не тут

float Linstep(float a, float b, float v)
{
	return saturate((v - a) / (b - a));
}

// Reduces VSM light bleedning
float ReduceLightBleeding(float pMax, float amount)
{
	// Remove the [0, amount] tail and linearly rescale (amount, 1].
	return Linstep(amount, 1.0f, pMax);
}

float ChebyshevUpperBound(float2 moments, float mean, float minVariance, float lightBleedingReduction)
{
	// Compute variance
	float variance = moments.y - (moments.x * moments.x);
	variance = max(variance, minVariance);

	// Compute probabilistic upper bound
	float d = mean - moments.x;
	float pMax = variance / (variance + (d * d));

	pMax = ReduceLightBleeding(pMax, lightBleedingReduction);

	// One-tailed Chebyshev
	return (mean <= moments.x ? 1.0f : pMax);
}

float getShadowValueEVSM(sampler2DArray shadowMapArray, int cascadeIndex, vec4 shadowUV)
{
	float compareDepth = shadowUV.z / u_lightShadowMapFarClipDistance;
	vec2 uv = shadowUV.xy / shadowUV.w;

	//!!!!
	
	float VSMBias = 0.01f;
	float LightBleedingReduction = 0.5;//0.0f;
	
	//!!!!
	
	float PositiveExponent = 40.0f;
	float NegativeExponent = 5.0f;
	
	//!!!!
	PositiveExponent *= 10.0;
	
	
	float2 exponents = GetEVSMExponents(PositiveExponent, NegativeExponent);
	float2 warpedDepth = WarpDepth(compareDepth, exponents);

	//!!!!ddx
	
	float4 occluder = texture2DArray(shadowMapArray, vec3(uv, cascadeIndex));
	//float4 occluder = ShadowMap.SampleGrad(VSMSampler, float3(shadowPos.xy, cascadeIdx), shadowPosDX.xy, shadowPosDY.xy);

	// Derivative of warping at depth
	float2 depthScale = VSMBias * 0.01f * exponents * warpedDepth;
	float2 minVariance = depthScale * depthScale;

	// Positive only
	float r = ChebyshevUpperBound(occluder.xy, warpedDepth.x, minVariance.x, LightBleedingReduction);
   // #endif

   
   return 1.0 - saturate( r );
}
#endif
*/

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Point light

//#ifdef GLOBAL_SHADOW_TECHNIQUE_SIMPLE
float getShadowValuePointSimple( float shadowMapIndex, vec4 shadowUV, int nLight )
{
	float compareDepth = shadowUV.w / d_lightShadowMapFarClipDistance;

	//flipped cubemaps. conversion already done in the vertex shader.	
#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
	vec4 shadowValue = textureCubeArrayLod( s_shadowMapShadowPoint, vec4( shadowUV.xyz, shadowMapIndex ), 0.0 );
	MEDIUMP float shadowFactor = compareDepth < unpackRgbaToFloat( shadowValue ) ? 1.0 : 0.0;
#else	
	MEDIUMP float shadowFactor = shadowCubeArray( s_shadowMapShadowPoint, vec4( shadowUV.xyz, shadowMapIndex ), compareDepth ).r;
#endif

//#ifdef REVERSEDZ
//	return shadowFactor;
//#else
	return 1.0 - shadowFactor;
//#endif

}
//#endif

#if defined(GLOBAL_SHADOW_TECHNIQUE_PCF4) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF8) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF12) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF16) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF22) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF32)
float getShadowValuePointPCF( float shadowMapIndex, vec4 shadowUV, int nLight )
{
	vec3 ray = normalize(shadowUV.xyz);

	vec3 absRay = abs(ray);

	MEDIUMP vec3 planeNormal;
	bool planeX = false;
	bool planeY = false;
	bool planeZ = false;

	if(absRay.x > absRay.y && absRay.x > absRay.z)
	{
		planeX = true;
		if(ray.x > 0.0)
			planeNormal = vec3(1.0, 0.0, 0.0);
		else
			planeNormal = vec3(-1.0, 0.0, 0.0);
	}
	else if(absRay.y > absRay.z)
	{
		planeY = true;
		if(ray.y > 0.0)
			planeNormal = vec3(0.0, 1.0, 0.0);
		else
			planeNormal = vec3(0.0, -1.0, 0.0);
	}
	else
	{
		planeZ = true;
		if(ray.z > 0.0)
			planeNormal = vec3(0.0, 0.0, 1.0);
		else
			planeNormal = vec3(0.0, 0.0, -1.0);
	}
	
	//detecting plane intersection point
	MEDIUMP float fraction = .5f / dot( planeNormal.rgb, ray );
	vec3 intersectionPoint = ray * fraction;

	vec3 texPos = intersectionPoint * d_lightShadowTextureSize;

	MEDIUMP float softness = d_lightShadowSoftness;
	//no linear interpolation when packed format is used. make softness smaller
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		softness *= 0.75;
	#endif
	
	MEDIUMP float scale = 1.5 * softness;
	//const float scale = 1.5;// = scale / u_lightShadowTextureSize;

#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF4
	const int sampleCount = 4;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF8
	const int sampleCount = 8;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF12
	const int sampleCount = 12;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF16
	const int sampleCount = 16;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF22
	const int sampleCount = 22;
#endif
#ifdef GLOBAL_SHADOW_TECHNIQUE_PCF32
	const int sampleCount = 32;
#endif

	float compareDepth = shadowUV.w / d_lightShadowMapFarClipDistance;

	MEDIUMP float shadow = 0.0;
	UNROLL
	for(int n = 0; n < sampleCount; n++)
	{
		vec2 offset2 = vogelDiskSample(n, sampleCount, 0.0) * scale;

		vec3 offset;
		if(planeX)
			offset = vec3(0.0, offset2.x, offset2.y);
		else if(planeY)
			offset = vec3(offset2.x, 0.0, offset2.y);
		else
			offset = vec3(offset2.x, offset2.y, 0.0);

		vec3 texCoord = texPos + offset;

		//flipped cubemaps. conversion already done in the vertex shader.		
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		vec4 shadowValue = textureCubeArrayLod( s_shadowMapShadowPoint, vec4( texCoord, shadowMapIndex ), 0.0 );
		MEDIUMP float shadowFactor = compareDepth < unpackRgbaToFloat( shadowValue ) ? 1.0 : 0.0;
	#else
		MEDIUMP float shadowFactor = shadowCubeArray( s_shadowMapShadowPoint, vec4( texCoord, shadowMapIndex ), compareDepth ).r;
	#endif

		shadow += shadowFactor;
	}
	
//#ifdef REVERSEDZ
//	return shadow / float(sampleCount);
//#else
	return 1.0 - shadow / float(sampleCount);
//#endif
}
#endif

/*#ifdef GLOBAL_SHADOW_TECHNIQUE_CHS
float getShadowValuePointCHS(vec4 shadowUV)
{
	//!!!!
	
	return 0;
}
#endif*/

/*#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM
float getShadowValuePointEVSM(vec4 shadowUV)
{
	return 0;
}
#endif*/

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//getShadowMultiplierMulti

float calcWorldTexelSize( float worldDistanceToSample, int cascadeIndex, int lightType, int nLight )
{
	float distScale;
	float unitTexelSize;
	if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
	{
		distScale = 1.0;
		//unitTexelSize = d_lightShadowUnitDistanceTexelSizes[ cascadeIndex ];
	}
	else
	{
		distScale = worldDistanceToSample;
		//unitTexelSize = d_lightShadowUnitDistanceTexelSizes[ 0 ];
	}
	unitTexelSize = d_lightShadowUnitDistanceTexelSizes[ cascadeIndex ];
	return distScale * unitTexelSize;
}

float getShadowMultiplierMulti( vec3 worldPosition, float cameraDistance, float cascadeDepth, MEDIUMP vec3 lightWorldDirection, MEDIUMP vec3 worldNormal, vec2 texCoord, vec4 fragCoord, int lightType, float shadowMapIndex, int nLight )
{
	MEDIUMP float final;
	MEDIUMP float finalAdd;

	MEDIUMP int cascadeIndex = 0;
#ifndef LIGHT_DIRECTIONAL_AMBIENT_ONLY
	BRANCH
	if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
#endif
	{
		vec4 lightShadowCascades = d_lightShadowCascades;//texelFetch( s_lightsTexture, ivec2( 28, nLight ), 0 );

		BRANCH
		if(cascadeDepth >= lightShadowCascades.y)
		{
			if(cascadeDepth < lightShadowCascades.z)
				cascadeIndex = 1;
			else if(cascadeDepth < lightShadowCascades.w)
				cascadeIndex = 2;
			else
				cascadeIndex = 3;
			
//#ifndef MOBILE
			//overlap cascades
			float from = lightShadowCascades[cascadeIndex];
			float to = from * 1.2;//d_lightShadowCascadeOverlapping;
			if(cascadeDepth < to)
			{
				float d = (cascadeDepth - from) / (to - from);
				if(!getDitherBoolean(fragCoord, d))
					cascadeIndex--;
			}
			int cascadeCount = int(lightShadowCascades.x) - 1;
			cascadeIndex = min(cascadeIndex, cascadeCount);
//#endif
		}
	}

	worldNormal = normalize(worldNormal);
	vec3 shadowTexelNormal = normalize( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT ? -d_lightDirection : lightWorldDirection );
	//if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
	//	shadowTexelNormal = normalize(-d_lightDirection);
	//else
	//	shadowTexelNormal = normalize(lightWorldDirection);

	MEDIUMP float NdotL = dot(shadowTexelNormal, worldNormal);//float NdotL = dot(lightWorldDirection, worldNormal);
	MEDIUMP float sine = sqrt(1.0 - NdotL * NdotL);
	MEDIUMP float tan = abs(NdotL) > 0.0 ? sine / NdotL : 0.0;
   	MEDIUMP float worldTexelSize = calcWorldTexelSize(length(worldPosition - d_lightPosition), cascadeIndex, lightType, nLight );

	vec3 worldPositionFixed = worldPosition;
#ifndef SHADOW_MAP_MICROPARTICLES_IN_AIR
	worldPositionFixed += 2.0 * lightWorldDirection * d_lightShadowBias * clamp(tan, 0.9, 10.0) * worldTexelSize;
	worldPositionFixed += 2.5 * (worldNormal * d_lightShadowNormalBias) * clamp(sine, 0.1, 1.0) * worldTexelSize;
#endif
	
#ifndef LIGHT_DIRECTIONAL_AMBIENT_ONLY
	BRANCH
	if( lightType == ENUM_LIGHT_TYPE_POINT )
	{
		//////////////////////////////////////////////////
		//Point

		vec4 shadowUV;
		shadowUV.xyz = worldPositionFixed - d_lightPosition;
		shadowUV.w = length(shadowUV.xyz);
		shadowUV.xyz = flipCubemapCoords(shadowUV.xyz);

		#ifdef GLSL
			mat4 toImageSpace = mat4(
				0.5, 0, 0, 0,
				0, -0.5, 0, 0,
				0, 0, 0.5, 0,
				0, 0, 0, 1 );
			shadowUV = mul(toImageSpace, shadowUV);	
		#endif
		
		#ifdef GLOBAL_SHADOW_TECHNIQUE_SIMPLE
			final = getShadowValuePointSimple( shadowMapIndex, shadowUV, nLight );
		#endif
		#if defined(GLOBAL_SHADOW_TECHNIQUE_PCF4) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF8) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF12) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF16) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF22) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF32)
			#if defined( SHADOW_MAP_MICROPARTICLES_IN_AIR ) || defined( SHADOW_MAP_FORCE_SIMPLE )
				final = getShadowValuePointSimple( shadowMapIndex, shadowUV, nLight );
			#else
				final = getShadowValuePointPCF( shadowMapIndex, shadowUV, nLight );
			#endif
		#endif
		
		//	#ifdef GLOBAL_SHADOW_TECHNIQUE_CHS
		//		final = getShadowValuePointCHS(shadowUV);
		//	#endif
		//	#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM
		//		final = getShadowValuePointEVSM(shadowUV);
		//	#endif

		finalAdd = saturate((cameraDistance + u_viewportOwnerShadowPointSpotlightDistance.y) * u_viewportOwnerShadowPointSpotlightDistance.z);
	}
	else if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
#endif		
	{
		//////////////////////////////////////////////////
		//Directional

		vec4 position4 = vec4(worldPositionFixed, 1);
		
		int readIndex = 12 + cascadeIndex * 4;
		mat4 lightShadowTextureViewProjMatrix = mtxFromRows(
			texelFetch( s_lightsTexture, ivec2( readIndex + 0, nLight ), 0 ), 
			texelFetch( s_lightsTexture, ivec2( readIndex + 1, nLight ), 0 ), 
			texelFetch( s_lightsTexture, ivec2( readIndex + 2, nLight ), 0 ), 
			texelFetch( s_lightsTexture, ivec2( readIndex + 3, nLight ), 0 ) );
			
		//mat4 lightShadowTextureViewProjMatrix;
		//switch(cascadeIndex)
		//{
		//	case 1: lightShadowTextureViewProjMatrix = d_lightShadowTextureViewProjMatrix1; break;
		//	case 2: lightShadowTextureViewProjMatrix = d_lightShadowTextureViewProjMatrix2; break;
		//	case 3: lightShadowTextureViewProjMatrix = d_lightShadowTextureViewProjMatrix3; break;
		//	default: lightShadowTextureViewProjMatrix = d_lightShadowTextureViewProjMatrix0; break;
		//}
		
		vec4 shadowUV = mul(lightShadowTextureViewProjMatrix, position4);
		
		#ifdef GLOBAL_SHADOW_TECHNIQUE_SIMPLE
			final = getShadowValueSimpleDirectional( cascadeIndex, shadowUV, nLight );
		#endif
		#if defined(GLOBAL_SHADOW_TECHNIQUE_PCF4) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF8) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF12) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF16) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF22) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF32)
			#if defined( SHADOW_MAP_MICROPARTICLES_IN_AIR ) || defined( SHADOW_MAP_FORCE_SIMPLE )
				final = getShadowValueSimpleDirectional( cascadeIndex, shadowUV, nLight );
			#else
				final = getShadowValuePCFDirectional( cascadeIndex, shadowUV, fragCoord, nLight );
			#endif		
		#endif
		
		//	#ifdef GLOBAL_SHADOW_TECHNIQUE_CHS
		//		final = getShadowValueCHS(cascadeIndex, shadowUV);
		//	#endif
		//	#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM
		//		final = getShadowValueEVSM(cascadeIndex, shadowUV);
		//	#endif

		//visualize cascades
		#ifdef GLOBAL_DEBUG_MODE
			if( d_lightShadowCascadesVisualize > 0.0 )
				final += 0.8 - float( cascadeIndex ) * 0.2;
		#endif
		
		finalAdd = saturate((cameraDistance + u_viewportOwnerShadowDirectionalDistance.y) * u_viewportOwnerShadowDirectionalDistance.z);
	}
#ifndef LIGHT_DIRECTIONAL_AMBIENT_ONLY
	else
	{
		//////////////////////////////////////////////////
		//Spot

		vec4 position4 = vec4(worldPositionFixed, 1);
		vec4 shadowUV = mul( d_lightShadowTextureViewProjMatrix0, position4 );
		
		#ifdef GLOBAL_SHADOW_TECHNIQUE_SIMPLE
			final = getShadowValueSimpleSpot( shadowMapIndex, shadowUV, nLight );
		#endif
		#if defined(GLOBAL_SHADOW_TECHNIQUE_PCF4) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF8) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF12) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF16) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF22) || defined(GLOBAL_SHADOW_TECHNIQUE_PCF32)
			#if defined( SHADOW_MAP_MICROPARTICLES_IN_AIR ) || defined( SHADOW_MAP_FORCE_SIMPLE )
				final = getShadowValueSimpleSpot( shadowMapIndex, shadowUV, nLight );
			#else
				final = getShadowValuePCFSpot( shadowMapIndex, shadowUV, fragCoord, nLight );
			#endif		
		#endif
		
		//	#ifdef GLOBAL_SHADOW_TECHNIQUE_CHS
		//		final = getShadowValueCHS(cascadeIndex, shadowUV);
		//	#endif
		//	#ifdef GLOBAL_SHADOW_TECHNIQUE_EVSM
		//		final = getShadowValueEVSM(cascadeIndex, shadowUV);
		//	#endif

		finalAdd = saturate((cameraDistance + u_viewportOwnerShadowPointSpotlightDistance.y) * u_viewportOwnerShadowPointSpotlightDistance.z);
	}
#endif	

	//contact shadows
	#ifdef SHADOW_CONTACT
		BRANCH
		if( final < 1.0 && d_lightShadowContactLength != 0.0 )
			final = max( final, screenSpaceShadows( worldPosition, lightWorldDirection, texCoord, nLight ) );
	#endif

	//////////////////////////////////////////////////

	//shadow intensity
	final = 1.0 - final * d_lightShadowIntensity;

	//fading by distance
	//vec3 u_viewportOwnerShadowDirectionalDistance, u_viewportOwnerShadowPointSpotlightDistance:
	//x: far distance
	//y: shadowFarDistance - shadowFadeMinDistance * 2
	//z: 1 / (shadowFarDistance - shadowFadeMinDistance)
	
	final += finalAdd;
	
	//if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
	//	final += saturate((cameraDistance + d_viewportOwnerShadowDirectionalDistance.y) * d_viewportOwnerShadowDirectionalDistance.z);
	//else
	//	final += saturate((cameraDistance + d_viewportOwnerShadowPointSpotlightDistance.y) * d_viewportOwnerShadowPointSpotlightDistance.z);

	final = saturate( final );

	return final;
}
