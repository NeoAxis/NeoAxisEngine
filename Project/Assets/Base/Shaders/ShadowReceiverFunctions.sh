// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Directional and Spot lights

#if defined(LIGHT_TYPE_DIRECTIONAL) || defined(LIGHT_TYPE_SPOTLIGHT)

//default for Low
float getShadowValueSimple(sampler2DArrayShadow shadowMapArray, int cascadeIndex, vec4 shadowUV)
{
	float compareDepth = shadowUV.z / u_lightShadowMapFarClipDistance;
	vec2 uv = shadowUV.xy / shadowUV.w;

#ifdef GLSL
	float shadowFactor = shadow2DArray(shadowMapArray, vec4(uv, cascadeIndex, compareDepth));
#else
	float shadowFactor = shadow2DArray(shadowMapArray, vec4(uv, cascadeIndex, compareDepth)).r;
#endif
	return (1.0 - shadowFactor);
}

//default for High
float getShadowValuePCF8TapFixedDisk4x(sampler2DArrayShadow shadowMapArray, int cascadeIndex, vec4 shadowUV)
{
	float compareDepth = shadowUV.z / u_lightShadowMapFarClipDistance;
	vec2 shadowUVScaled = shadowUV.xy / shadowUV.w;

	float scale = 2.0f / u_lightShadowTextureSize;
	//float scale = u_lightShadowSoftness * 2.0f / u_lightShadowTextureSize;
	//const float scale = 2.0f / u_lightShadowTextureSize;

#ifdef GLSL
	const vec2 poisson[8] = vec2[8]
	(
		vec2(     0.0,      0.0),
		vec2( -0.18, -0.816),
		vec2(-0.126,    0.8),
		vec2(-0.854, -0.166),
		vec2( 0.856,  -0.13),
		vec2(-0.394,  0.032),
		vec2( 0.178,   0.33),
		vec2( 0.186, -0.324)
	);
#else
	const vec2 poisson[8] =
	{
		vec2(     0.0,      0.0),
		vec2( -0.18, -0.816),
		vec2(-0.126,    0.8),
		vec2(-0.854, -0.166),
		vec2( 0.856,  -0.13),
		vec2(-0.394,  0.032),
		vec2( 0.178,   0.33),
		vec2( 0.186, -0.324),
	};
#endif

	//8 tap filter

	float shadow = 0.0;
	for(int n = 0; n < 8; n++)
	{
		vec2 texCoord = shadowUVScaled.xy + poisson[n] * scale;
#ifdef GLSL
		float shadowFactor = shadow2DArray(shadowMapArray, vec4(texCoord, cascadeIndex, compareDepth));
#else
		float shadowFactor = shadow2DArray(shadowMapArray, vec4(texCoord, cascadeIndex, compareDepth)).r;
#endif		
		shadow += shadowFactor;
	}
	return (1.0 - shadow / 8.0);
}

/*
float getShadowValuePCF4x4(sampler2DArray shadowMapArray, int cascadeIndex, vec4 shadowUV)
{
	float compareDepth = shadowUV.z / u_lightShadowMapFarClipDistance;
	vec2 shadowUVScaled = shadowUV.xy / shadowUV.w;

	//2x2 filter
	
	//transform to texel space
	vec2 texelPos = u_lightShadowTextureSize * shadowUVScaled.xy;
	//determine the lerp amounts
	vec2 lerps = frac( texelPos );

	float pixelOffset = ( 1.0f / u_lightShadowTextureSize ) * .99f;

	vec4 depths;
	depths.x = texture2DArray(shadowMapArray, vec3(shadowUVScaled.xy, cascadeIndex)).r;
	depths.y = texture2DArray(shadowMapArray, vec3(shadowUVScaled.xy + vec2(pixelOffset, 0), cascadeIndex)).r;
	depths.z = texture2DArray(shadowMapArray, vec3(shadowUVScaled.xy + vec2(0, pixelOffset), cascadeIndex)).r;
	depths.w = texture2DArray(shadowMapArray, vec3(shadowUVScaled.xy + vec2(pixelOffset, pixelOffset), cascadeIndex)).r;
	//depths.x = texture2D(shadowMap, shadowUVScaled.xy ).r;
	//depths.y = texture2D(shadowMap, shadowUVScaled.xy + vec2(pixelOffset, 0) ).r;
	//depths.z = texture2D(shadowMap, shadowUVScaled.xy + vec2(0, pixelOffset) ).r;
	//depths.w = texture2D(shadowMap, shadowUVScaled.xy + vec2(pixelOffset, pixelOffset) ).r;

	vec4 depthFlags = step( depths, compareDepth );

	return (float)lerp( 
		lerp( depthFlags.x, depthFlags.y, lerps.x ),
		lerp( depthFlags.z, depthFlags.w, lerps.x ),
		lerps.y );

//#endif

}

//!!!!no fetch4, no post blur
float getShadowValuePCF16TapFixedDisk(sampler2DArray shadowMapArray, int cascadeIndex, vec4 shadowUV)
{
	float compareDepth = shadowUV.z / u_lightShadowMapFarClipDistance;
	vec2 shadowUVScaled = shadowUV.xy / shadowUV.w;

	const float scale = 2.0f / u_lightShadowTextureSize;

	const vec2 poisson[16] =
	{
		vec2(     0,      0),
		vec2(-0.488, -0.748),
		vec2( 0.704,   0.55),
		vec2(-0.916, -0.002),
		vec2( 0.724, -0.526),
		vec2( -0.62, -0.336),
		vec2( 0.114,  0.668),
		vec2( 0.116, -0.694), 
		vec2(-0.568,   0.41),
		vec2(  0.69, -0.008),
		vec2(-0.324,  0.866), 
		vec2(-0.116, -0.308), 
		vec2( 0.304,   0.31),
		vec2(-0.432, -0.008),
		vec2( 0.328,   -0.2),
		vec2(-0.118,  0.376),
	};

	//16 tap filter

	float shadow = 0;
	for(int n = 0; n < 16; n++)
	{
		vec2 texCoord = shadowUVScaled.xy + poisson[n] * scale;

		float depth = texture2DArray(shadowMapArray, vec3(texCoord, cascadeIndex)).r;
		//float depth = texture2D(shadowMap, texCoord ).r;
		float depthFlag = step( depth, compareDepth );
		
		shadow += (float)depthFlag;
	}
	return shadow / 16;
}
*/

#endif //LIGHT_TYPE_DIRECTIONAL || LIGHT_TYPE_SPOTLIGHT


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Point light

#ifdef LIGHT_TYPE_POINT

//default for Low
float getShadowValuePointSimple(samplerCubeShadow shadowMap, vec4 shadowUV)
{
	//flipped cubemaps. conversion already done in the vertex shader.
	float compareDepth = shadowUV.w / u_lightShadowMapFarClipDistance;
	float shadowFactor = shadowCube(shadowMap, vec4(shadowUV.xyz, compareDepth));
	return (1.0 - shadowFactor);
}

//default for High
float getShadowValuePointPCF8TapFixedDisk4x(samplerCubeShadow shadowMap, vec4 shadowUV)
{
	vec3 ray = normalize(shadowUV.xyz);

	vec3 absRay = abs(ray);

	vec3 planeNormal;
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
	float fraction = .5f / dot( planeNormal.rgb, ray );
	vec3 intersectionPoint = ray * fraction;

	vec3 texPos = intersectionPoint * u_lightShadowTextureSize;

	const float scale = 1.5;
	//float scale = u_lightShadowSoftness * 1.5f;
	//const float scale = 1.5;// = scale / u_lightShadowTextureSize;

#ifdef GLSL
	const vec2 poisson[8] = vec2[8]
	(
		vec2(     0.0,      0.0),
		vec2( -0.18, -0.816),
		vec2(-0.126,    0.8),
		vec2(-0.854, -0.166),
		vec2( 0.856,  -0.13),
		vec2(-0.394,  0.032),
		vec2( 0.178,   0.33),
		vec2( 0.186, -0.324)
	);
#else
	const vec2 poisson[8] =
	{
		vec2(     0.0,      0.0),
		vec2( -0.18, -0.816),
		vec2(-0.126,    0.8),
		vec2(-0.854, -0.166),
		vec2( 0.856,  -0.13),
		vec2(-0.394,  0.032),
		vec2( 0.178,   0.33),
		vec2( 0.186, -0.324),
	};
#endif

	//8 tap filter

	float compareDepth = shadowUV.w / u_lightShadowMapFarClipDistance;

	float shadow = 0.0;
	for(int n = 0; n < 8; n++)
	{
		vec2 offset2 = poisson[n] * scale;

		vec3 offset;
		if(planeX)
			offset = vec3(0.0, offset2.x, offset2.y);
		else if(planeY)
			offset = vec3(offset2.x, 0.0, offset2.y);
		else
			offset = vec3(offset2.x, offset2.y, 0.0);

		vec3 texCoord = texPos + offset;

		//flipped cubemaps. conversion already done in the vertex shader.
		float shadowFactor = shadowCube(shadowMap, vec4(texCoord, compareDepth));

		shadow += shadowFactor;
	}
	return (1.0 - shadow / 8.0);
}

/*
float getShadowValueForPointLight2x2(samplerCube shadowMap, vec4 shadowUV)
{
	vec3 ray = normalize(shadowUV.xyz);

	vec3 absRay = abs(ray);

	vec3 planeNormal;
	bool planeX = false;
	bool planeY = false;
	bool planeZ = false;

	if(absRay.x > absRay.y && absRay.x > absRay.z)
	{
		planeX = true;
		if(ray.x > 0)
			planeNormal = vec3(1, 0, 0);
		else
			planeNormal = vec3(-1, 0, 0);
	}
	else if(absRay.y > absRay.z)
	{
		planeY = true;
		if(ray.y > 0)
			planeNormal = vec3(0, 1, 0);
		else
			planeNormal = vec3(0, -1, 0);
	}
	else
	{
		planeZ = true;
		if(ray.z > 0)
			planeNormal = vec3(0, 0, 1);
		else
			planeNormal = vec3(0, 0, -1);
	}
	
	//detecting plane intersection point
	float fraction = .5f / dot( planeNormal.rgb, ray );
	vec3 intersectionPoint = ray * fraction;

	vec3 texPos = intersectionPoint * u_lightShadowTextureSize;

	//determine the lerp amounts
	vec2 lerps;
	{
		vec2 v;
		if(planeX)
			v = texPos.yz;
		else if(planeY)
			v = texPos.xz;
		else
			v = texPos.xy;
		lerps = frac(v);
	}

	float compareDepth = shadowUV.w / u_lightShadowMapFarClipDistance;

	vec3 offset1, offset2;
	if(planeX)
	{
		offset1 = vec3(0, 1, 0);
		offset2 = vec3(0, 0, 1);
	}
	else if(planeY)
	{
		offset1 = vec3(1, 0, 0);
		offset2 = vec3(0, 0, 1);
	}
	else
	{
		offset1 = vec3(1, 0, 0);
		offset2 = vec3(0, 1, 0);
	}

	vec4 depths;
	//flipped cubemaps. conversion already done in the vertex shader.
	depths.x = textureCube(shadowMap, texPos ).r;
	depths.y = textureCube(shadowMap, texPos + offset1 ).r;
	depths.z = textureCube(shadowMap, texPos + offset2 ).r;
	depths.w = textureCube(shadowMap, texPos + offset1 + offset2 ).r;

	vec4 depthFlags = step( depths, compareDepth );
	return lerp( 
		lerp( depthFlags.x, depthFlags.y, lerps.x ),
		lerp( depthFlags.z, depthFlags.w, lerps.x ),
		lerps.y );
}

float getShadowValueForPointLightPoisson16(samplerCube shadowMap, vec4 shadowUV)
{
	vec3 ray = normalize(shadowUV.xyz);

	vec3 absRay = abs(ray);

	vec3 planeNormal;
	bool planeX = false;
	bool planeY = false;
	bool planeZ = false;

	if(absRay.x > absRay.y && absRay.x > absRay.z)
	{
		planeX = true;
		if(ray.x > 0)
			planeNormal = vec3(1, 0, 0);
		else
			planeNormal = vec3(-1, 0, 0);
	}
	else if(absRay.y > absRay.z)
	{
		planeY = true;
		if(ray.y > 0)
			planeNormal = vec3(0, 1, 0);
		else
			planeNormal = vec3(0, -1, 0);
	}
	else
	{
		planeZ = true;
		if(ray.z > 0)
			planeNormal = vec3(0, 0, 1);
		else
			planeNormal = vec3(0, 0, -1);
	}
	
	//detecting plane intersection point
	float fraction = .5f / dot( planeNormal.rgb, ray );
	vec3 intersectionPoint = ray * fraction;

	vec3 texPos = intersectionPoint * u_lightShadowTextureSize;

	const float scale = 1.5;// = scale / u_lightShadowTextureSize;

	const vec2 poisson[16] =
	{
		vec2(     0,      0),
		vec2(-0.488, -0.748),
		vec2( 0.704,   0.55),
		vec2(-0.916, -0.002),
		vec2( 0.724, -0.526),
		vec2( -0.62, -0.336),
		vec2( 0.114,  0.668),
		vec2( 0.116, -0.694), 
		vec2(-0.568,   0.41),
		vec2(  0.69, -0.008),
		vec2(-0.324,  0.866), 
		vec2(-0.116, -0.308), 
		vec2( 0.304,   0.31),
		vec2(-0.432, -0.008),
		vec2( 0.328,   -0.2),
		vec2(-0.118,  0.376),
	};

	//16 tap filter

	float compareDepth = shadowUV.w / u_lightShadowMapFarClipDistance;

	float shadow = 0;
	for(int n = 0; n < 16; n++)
	{
		vec2 offset2 = poisson[n] * scale;

		vec3 offset;
		if(planeX)
			offset = vec3(0, offset2.x, offset2.y);
		else if(planeY)
			offset = vec3(offset2.x, 0, offset2.y);
		else
			offset = vec3(offset2.x, offset2.y, 0);

		vec3 texCoord = texPos + offset;
		
		//flipped cubemaps. conversion already done in the vertex shader.
		float depth = textureCube( shadowMap, texCoord ).r;
		float depthFlag = step( depth, compareDepth );
		
		shadow += depthFlag;
	}
	return shadow / 16;
}
*/

#endif //LIGHT_TYPE_POINT

float calcWorldTexelSize(float worldDistanceToSample, int cascadeIndex)
{
	#ifdef LIGHT_TYPE_DIRECTIONAL
		float distScale = 1.0;
		float unitTexelSize = u_lightShadowUnitDistanceTexelSizes[cascadeIndex];
	#else
		float distScale = worldDistanceToSample;
		float unitTexelSize = u_lightShadowUnitDistanceTexelSizes[0];
	#endif
	return distScale * unitTexelSize;
	//return 1.414213562f * (2.0f * distScale) / (u_proj[0][0] * u_lightShadowTextureSize.x);
	//return 1.414213562f * (2.0f * distScale) / (u_lightShadowTextureViewProjMatrix0[0] * u_lightShadowTextureSize.x);

	// наверное лучше world размер тексел€ на near clip plane посчитать на cpu и прокинуть в юниформ
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//getShadowMultiplier

float getShadowMultiplier(vec3 worldPosition, float cameraDistance, float cascadeDepth, vec3 lightWorldDirection, vec3 worldNormal)
{
	float final;

	int cascadeIndex = 0;
	#ifdef LIGHT_TYPE_DIRECTIONAL		
		if(cascadeDepth < u_lightShadowCascades.y)
			cascadeIndex = 0;
		else if(cascadeDepth < u_lightShadowCascades.z)
			cascadeIndex = 1;
		else if(cascadeDepth < u_lightShadowCascades.w)
			cascadeIndex = 2;
		else
			cascadeIndex = 3;
		int cascadeCount = int(u_lightShadowCascades.x) - 1;
		cascadeIndex = min(cascadeIndex, cascadeCount);
	#endif


	worldNormal = normalize(worldNormal);
	#ifdef LIGHT_TYPE_SPOTLIGHT
		// дл€ spotlight shadowmap тексели направлены вдоль направлени€ источника света 
		// (т.е. все тексели смотр€т в одну сторону как у directional light)
		vec3 shadowTexelNormal = normalize(-u_lightDirection);
	#else
		vec3 shadowTexelNormal = normalize(lightWorldDirection);
	#endif

	float NdotL = dot(shadowTexelNormal, worldNormal);//float NdotL = dot(lightWorldDirection, worldNormal);
	float sine = sqrt(1.0 - NdotL * NdotL);
	float tan = abs(NdotL) > 0.0 ? sine / NdotL : 0.0;
   	float worldTexelSize = calcWorldTexelSize(length(worldPosition - u_lightPosition.xyz), cascadeIndex); 

	worldPosition.xyz += 2.0 * lightWorldDirection * u_lightShadowBias * clamp(tan, 0.9, 10.0) * worldTexelSize;
	worldPosition.xyz += 2.5 * (worldNormal * u_lightShadowNormalBias) * clamp(sine, 0.1, 1.0) * worldTexelSize;
	// clamp нужен, чтобы избежать артефактов, когда тексел с пола заезжает на стенку и недостаточно сильно
	// выправл€етс€ bias-ом, 0.1 и 0.9 подобраны эмпирически, надо подумать, нельз€ ли их прив€зать к
	// более осмысленным константам
	// множитель 2 стоит из тех же соображений. ¬ идеале должно 1 хватить (если бы не артефакты на острых стыках граней)
	
#ifdef LIGHT_TYPE_POINT
	//////////////////////////////////////////////////
	//Point

	vec4 shadowUV;
	shadowUV.xyz = worldPosition - u_lightPosition.xyz;
	shadowUV.w = length(shadowUV.xyz);
	shadowUV.xyz = flipCubemapCoords(shadowUV.xyz);

	#ifdef SHADOW_MAP_HIGH
		final = getShadowValuePointPCF8TapFixedDisk4x(s_shadowMapShadow, shadowUV);
	#else
		final = getShadowValuePointSimple(s_shadowMapShadow, shadowUV);
	#endif
		
#else
	//////////////////////////////////////////////////
	//Directional, Spot

/*
	int cascadeIndex = 0;	
	#ifdef LIGHT_TYPE_DIRECTIONAL		
		if(cascadeDepth < u_lightShadowCascades.y)
			cascadeIndex = 0;
		else if(cascadeDepth < u_lightShadowCascades.z)
			cascadeIndex = 1;
		else if(cascadeDepth < u_lightShadowCascades.w)
			cascadeIndex = 2;
		else
			cascadeIndex = 3;
		int cascadeCount = int(u_lightShadowCascades.x) - 1;
		cascadeIndex = min(cascadeIndex, cascadeCount);
	#endif
*/

	vec4 position4 = vec4(worldPosition, 1);

	mat4 lightShadowTextureViewProjMatrix;	
	switch(cascadeIndex)
	{
		case 1: lightShadowTextureViewProjMatrix = u_lightShadowTextureViewProjMatrix1; break;
		case 2: lightShadowTextureViewProjMatrix = u_lightShadowTextureViewProjMatrix2; break;
		case 3: lightShadowTextureViewProjMatrix = u_lightShadowTextureViewProjMatrix3; break;
		default: lightShadowTextureViewProjMatrix = u_lightShadowTextureViewProjMatrix0; break;
	}
	vec4 shadowUV = mul(lightShadowTextureViewProjMatrix, position4);

	#ifdef SHADOW_MAP_HIGH
		final = getShadowValuePCF8TapFixedDisk4x(s_shadowMapArrayShadow, cascadeIndex, shadowUV);
	#else
		final = getShadowValueSimple(s_shadowMapArrayShadow, cascadeIndex, shadowUV);
	#endif

	//visualize cascades
	#ifdef LIGHT_TYPE_DIRECTIONAL
	if(u_lightShadowCascadesVisualize > 0.0 && u_lightShadowCascades.x > 1.0)
	{
		if(cascadeDepth < u_lightShadowCascades.y)
			final += .8f;
		else if(cascadeDepth < u_lightShadowCascades.z)
			final += .6f;
		else if(cascadeDepth < u_lightShadowCascades.w)
			final += .4f;
		else
			final += .2f;
	}
	#endif
	
#endif

	//////////////////////////////////////////////////

	//shadow intensity
	final = 1.0 - final * u_lightShadowIntensity;

	//fading by distance
	//vec3 u_viewportOwnerShadowFarDistance:
	//x: far distance
	//y: shadowFarDistance - shadowFadeMinDistance * 2
	//z: 1 / (shadowFarDistance - shadowFadeMinDistance)
	final += saturate((cameraDistance + u_viewportOwnerShadowFarDistance.y) * u_viewportOwnerShadowFarDistance.z);
	if(final > 1.0)
		final = 1.0;

	return final;
}
