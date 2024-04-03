// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#ifdef GLSL
void clip(float v)
{
	if(v < 0.0)
		discard;
}
#endif

void smoothLOD(vec4 fragCoord, float lodValue)
{
	BRANCH
	if(lodValue != 0.0)
	{
		uint x = uint(fragCoord.x);
		uint y = uint(fragCoord.y);
		uint xx = x % uint(4);
		uint yy = y % uint(4);
		uint v = xx * uint(4) + yy;
#ifdef GLSL
		const float thresholds[16] = float[16](0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625);
#else
		const float thresholds[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
#endif
		float threshold = thresholds[v];
		if(lodValue > 0.0)
			clip(threshold - lodValue);
		else
			clip(-threshold - lodValue);
	}
}

float dither(vec4 fragCoord, float value)
{
	BRANCH
	if(value > 0.02 && value < 0.98)
	{
		uint x = uint(fragCoord.x);
		uint y = uint(fragCoord.y);
		uint xx = x % uint(4);
		uint yy = y % uint(4);
		uint v = xx * uint(4) + yy;
#ifdef GLSL
		const float array[16] = float[16](0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625);
#else
		const float array[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
#endif
		float vv = array[v];
		value -= vv * 1.0 - 0.5;
		//value -= vv * 0.5 - 0.25;
		//value -= vv * 0.25 - 0.125;
		value = saturate(value);
	}	
	return value;
}

bool getDitherBoolean(vec4 fragCoord, float value)
{
	uint x = uint(fragCoord.x);
	uint y = uint(fragCoord.y);
	uint xx = x % uint(4);
	uint yy = y % uint(4);
	uint v = xx * uint(4) + yy;
#ifdef GLSL
	const float thresholds[16] = float[16](0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625);
#else
	const float thresholds[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
#endif
	float threshold = thresholds[v];
	return threshold - value < 0.0;
}

int ditherArray4(vec4 fragCoord, vec3 factors)
{
	uint x = uint(fragCoord.x);
	uint y = uint(fragCoord.y);
	uint xx = x % uint(4);
	uint yy = y % uint(4);
	uint v = xx * uint(4) + yy;
#ifdef GLSL
	const float thresholds[16] = float[16](0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625);
#else
	const float thresholds[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
#endif
	float threshold = thresholds[v];
	
	if(threshold < factors[0])
		return 0;
	threshold -= factors[0];
	if(threshold < factors[1])
		return 1;
	threshold -= factors[1];
	if(threshold < factors[2])
		return 2;
	return 3;
}

vec2 vogelDiskSample(int sampleIndex, int sampleCount, float angle)
{
	const float goldenAngle = 2.399963f;
	float r = sqrt(float(sampleIndex) + 0.5f) / sqrt(float(sampleCount));
	float theta = float(sampleIndex) * goldenAngle + angle;
	float sine, cosine;
	sincos(theta, sine, cosine);
	return vec2(cosine, sine) * r;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//lightAttenuation: x = near; y = far; z = power; w = far - near.
float getLightAttenuation(vec4 lightAttenuation, float distance)
{
	return saturate(pow(1.0 - min( (distance - lightAttenuation.x) / lightAttenuation.w, 1.0), lightAttenuation.z));
}

//lightAttenuation: x = near; y = power; z = far - near.
float getLightAttenuation2(vec3 lightAttenuation, float distance)
{
	return saturate(pow(1.0 - min( (distance - lightAttenuation.x) / lightAttenuation.z, 1.0), lightAttenuation.y));
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

float toClipSpaceDepth(float depthTextureValue)
{
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	return depthTextureValue;
#else
	return depthTextureValue * 2.0 - 1.0;
#endif
}

float getDepthFromClipSpaceDepth(float depthTextureValue)
{
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	return depthTextureValue;
#else
	return (depthTextureValue + 1.0) * 0.5;
#endif
}

float getDepthValue(float depthTextureValue, float near, float far)
{
	//!!!!
//#ifdef REVERSEDZ	
//	depthTextureValue = 1.0 - depthTextureValue;
//#endif


	float clipDepth = toClipSpaceDepth(depthTextureValue);
	
	float result;
	
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	result = near * far / (far - clipDepth * (far - near));
#else
	result = near * far / (far + near - clipDepth * (far - near));
	//return 2.0 * near * far / (far + near - clipDepth * (far - near));
#endif

	return result;
}

//!!!!not works
/*
float getDepthValue2(vec2 texCoord, float rawDepth, float near, float far, mat4 invProj)
{
	vec3 clip = vec3(texCoord * 2.0 - 1.0, toClipSpaceDepth(rawDepth));
	#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
		clip.y = -clip.y;
	#endif	
	vec4 posVS = mul(invProj, vec4(clip, 1.0));

	float depth = (posVS.z - near) / (far - near);
	
	return depth;
}
*/

float getRawDepthValue(float depth, float near, float far)
{
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	float clipDepth = (far - (near * far) / depth) / (far - near);
#else
	float clipDepth = (far + near - (near * far) / depth) / (far - near);
#endif
	
	float rawDepth = getDepthFromClipSpaceDepth(clipDepth);
	
	//!!!!
//#ifdef REVERSEDZ
//	rawDepth = 1.0 - rawDepth;
//#endif
	
	return rawDepth;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec3 reconstructWorldPosition(mat4 invView, mat4 invProj, vec2 texCoord, float rawDepth)
{
	//!!!!
//#ifdef REVERSEDZ
//	rawDepth = 1.0 - rawDepth;
//#endif
	
	vec3 clip = vec3(texCoord * 2.0 - 1.0, toClipSpaceDepth(rawDepth));
	#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
		clip.y = -clip.y;
	#endif	
	vec4 posVS = mul(invProj, vec4(clip, 1.0));
	vec3 posNDC = posVS.xyz / posVS.w;
	return mul(invView, vec4(posNDC, 1.0)).xyz;
}

/*
vec3 reconstructWorldPosition(mat4 invViewProj, vec2 texCoord, float rawDepth)
{
	vec3 clip = vec3(texCoord * 2.0 - 1.0, toClipSpaceDepth(rawDepth));
	#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
		clip.y = -clip.y;
	#endif
	vec4 wpos = mul(invViewProj, vec4(clip, 1.0));
	return wpos.xyz / wpos.w;
}
*/

vec2 projectToScreenCoordinates(mat4 viewProj, vec3 worldPosition)
{
	// get homogeneous clip space coordinates
	vec4 clipSpace = mul(viewProj, vec4( worldPosition, 1.0 ));
	
	// apply perspective divide to get normalized device coordinates
	vec3 ndc = clipSpace.xyz / clipSpace.w;
	
	// do viewport transform
	vec2 screenSpace = (ndc.xy * 0.5 + vec2_splat( 0.5 ));
	screenSpace.y = 1.0 - screenSpace.y;
	
	return screenSpace;
	
	
	/*
	vec3 eyeSpacePos = mul(viewMatrix, vec4(worldPosition, 1.0)).xyz;
	
	//!!!!
	//if( backsideReturnFalse )

	mat4 m = projMatrix;
	vec3 v = eyeSpacePos;	
	
	//!!!!	
	float invW = 1.0 / ( m[3][0] * v.x + m[3][1] * v.y + m[3][2] * v.z + m[3][3] );
	//float invW = 1.0 / ( m[0][3] * v.x + m[1][3] * v.y + m[2][3] * v.z + m[3][3] );
	//float invW = 1.0 / ( m.Item0.W * v.X + m.Item1.W * v.Y + m.Item2.W * v.Z + m.Item3.W );
	vec2 v2 = mul(m, vec4(v, 1) ) * invW;

	vec2 screenPosition = vec2( ( v2.x + 1.0 ) * 0.5, 1.0 - ( v2.y + 1.0 ) * 0.5 );	

	return screenPosition;
	*/	
	
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*
float encodeBitsToByte(bool v1, bool v2)
{
	float v = v1 ? 0.75 : 0.25;
	if(!v2)
		v = -v;
	return v * 0.5 + 0.5;
}

void decodeBitsFromByte(float source, out bool v1, out bool v2)
{
	float v = source * 2.0 - 1.0;
	v1 = abs(v) > 0.5;
	v2 = v > 0.0;
}
*/

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*
vec4 encodeGBuffer4(vec4 tangent, bool shadingModelSimple, bool receiveDecals)
{
	vec4 result;
	
	result.xyz = tangent.xyz * 0.5 + 0.5;
	if(shadingModelSimple)
		result.xyz = vec3_splat(0);
	
	result.w = encodeBitsToByte(tangent.w > 0.0, receiveDecals);
	
	return result;
}

void decodeGBuffer4(vec4 source, out vec4 tangent, out bool shadingModelSimple, out bool receiveDecals)
{
	bool tangentW;
	decodeBitsFromByte(source.w, tangentW, receiveDecals);
	
	tangent = vec4(normalize(source.xyz * 2.0 - 1.0), tangentW ? 1.0 : -1.0);

	shadingModelSimple = !any2(source.xyz);
}
*/

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct EnvironmentTextureData
{
	vec4 rotation;//mat3 rotation;
	vec4 multiplierAndAffect;
};

vec3 irradianceSphericalHarmonics( vec4 irradiance[9], const vec3 n )
{
    return max(
          irradiance[0]
        + irradiance[1] * (n.y)
        + irradiance[2] * (n.z)
        + irradiance[3] * (n.x)
#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_QUALITY
		+ irradiance[4] * (n.y * n.x)
        + irradiance[5] * (n.y * n.z)
        + irradiance[6] * (3.0 * n.z * n.z - 1.0)
        + irradiance[7] * (n.z * n.x)
        + irradiance[8] * (n.x * n.x - n.y * n.y)
#endif
        , vec4_splat(0.0)).xyz;
}

vec3 getEnvironmentValue( vec4 environmentIrradiance[9], EnvironmentTextureData data, vec3 texCoord )
{
	vec3 v = irradianceSphericalHarmonics(environmentIrradiance, flipCubemapCoords(mulQuat(data.rotation, texCoord))).rgb * data.multiplierAndAffect.xyz;
	//vec3 v = irradianceSphericalHarmonics(environmentIrradiance, flipCubemapCoords2(mulQuat(data.rotation, texCoord))).rgb * data.multiplierAndAffect.xyz;
	return lerp(vec3_splat(1), v, data.multiplierAndAffect.w);
	
	/*
	BRANCH
	if( environmentIrradiance[ 0 ].w == 1.0 )
	{
		//white harmonics detected
		return vec3_splat( 1 );
	}
	else
	{
		vec3 v = irradianceSphericalHarmonics(environmentIrradiance, flipCubemapCoords(mulQuat(data.rotation, texCoord))).rgb * data.multiplierAndAffect.xyz;
		//vec3 v = irradianceSphericalHarmonics(environmentIrradiance, flipCubemapCoords2(mulQuat(data.rotation, texCoord))).rgb * data.multiplierAndAffect.xyz;
		return lerp(vec3_splat(1), v, data.multiplierAndAffect.w);
	}*/
}

/*
vec3 getEnvironmentValueTexture(samplerCube tex, EnvironmentTextureData data, vec3 texCoord)
{
	vec3 v = textureCube(tex, flipCubemapCoords(mul(data.rotation, texCoord))).rgb * data.multiplierAndAffect.xyz;
	//vec3 v = textureCube(tex, flipCubemapCoords2(mul(data.rotation, texCoord))).rgb * data.multiplierAndAffect.xyz;
	return lerp(vec3_splat(1), v, data.multiplierAndAffect.w);
}
*/

vec3 getEnvironmentValueLod(samplerCube tex, EnvironmentTextureData data, vec3 texCoord, float lod)
{
	//!!!!
	vec3 v = textureCubeLod(tex, flipCubemapCoords(mulQuat(data.rotation, texCoord)), lod).rgb * data.multiplierAndAffect.xyz;
	//vec3 v = textureCubeLod(tex, flipCubemapCoords2(mulQuat(data.rotation, texCoord)), lod).rgb * data.multiplierAndAffect.xyz;
	return lerp(vec3_splat(1), v, data.multiplierAndAffect.w);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool cutVolumes( vec3 worldPosition )
{
#if GLOBAL_CUT_VOLUME_MAX_AMOUNT > 0

	BRANCH
	if(u_viewportCutVolumeSettings.x > 0.0)
	{
		int count = int(u_viewportCutVolumeSettings.x);
		//BRANCH
		LOOP
		for(int n = 0; n < count; n++)
		{
			mat4 m = u_viewportCutVolumeData[n];
			float shape = m[3][3];
			m[3][3] = 1.0;

			vec3 p = abs(mul(m, vec4(worldPosition, 1.0)).xyz);
			bool invert = shape < 0.0;
			float shapeAbs = abs(shape);
			
			bool clip = false;
			
			if(shapeAbs == 1.0)
			{
				//Box
				clip = p.x < 0.5 && p.y < 0.5 && p.z < 0.5;
			}
			else if(shapeAbs == 2.0)
			{
				//Sphere
				clip = length(p) < 0.5;
			}
			else// if(shapeAbs == 3.0)
			{
				//Cylinder
				clip = p.x < 0.5 && length(p.yz) < 0.5;
			}
/*			else
			{
				//Plane
				vec4 plane = m[0];
				clip = dot(plane, vec4(worldPosition, 1.0));
			}*/
			
			if(invert)
				clip = !clip;
			
			if(clip)
				return true;
		}
	}

#endif

	return false;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef GLSL
void rayAABBIntersect(vec3 ray_origin, vec3 ray_dir, vec3 minpos, vec3 maxpos, out bool intersects, out float intersectScale)
{
	vec3 inverse_dir = 1.0 / ray_dir;
	vec3 tbot = inverse_dir * (minpos - ray_origin);
	vec3 ttop = inverse_dir * (maxpos - ray_origin);
	vec3 tmin = min(ttop, tbot);
	vec3 tmax = max(ttop, tbot);
	vec2 traverse = max(tmin.xx, tmin.yz);
	float traverselow = max(traverse.x, traverse.y);
	traverse = min(tmax.xx, tmax.yz);
	float traversehi = min(traverse.x, traverse.y);

	intersects = traversehi > max(traverselow, 0.0);
	intersectScale = traverselow;
	//return vec3(float(traversehi > max(traverselow, 0.0)), traversehi, traverselow);
}
#endif
