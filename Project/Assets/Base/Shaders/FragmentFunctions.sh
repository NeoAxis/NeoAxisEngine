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
		int x = int(fragCoord.x);
		int y = int(fragCoord.y);
		int xx = x % 4;
		int yy = y % 4;
		int v = xx * 4 + yy;
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
		int x = int(fragCoord.x);
		int y = int(fragCoord.y);
		int xx = x % 4;
		int yy = y % 4;
		int v = xx * 4 + yy;
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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//lightAttenuation: x = near; y = far; z = power; w = far - near.
float getLightAttenuation(vec4 lightAttenuation, float distance)
{
	return saturate(pow(1.0 - min( (distance - lightAttenuation.x) / lightAttenuation.w, 1.0), lightAttenuation.z));
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

float toClipSpaceDepth(float depthTextureValue)
{
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	return depthTextureValue;
#else
	return depthTextureValue * 2.0 - 1.0;
#endif // BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
}

float getDepthValue(float depthTextureValue, float near, float far)
{
	float clipDepth = toClipSpaceDepth(depthTextureValue);
	
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	return near * far / (far - clipDepth * (far - near));
#else
	return near * far / (far + near - clipDepth * (far - near));
	//return 2.0 * near * far / (far + near - clipDepth * (far - near));
#endif // BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec3 reconstructWorldPosition(mat4 invViewProj, vec2 texCoord, float rawDepth)
{
	vec3 clip = vec3(texCoord * 2.0 - 1.0, toClipSpaceDepth(rawDepth));
	#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
		clip.y = -clip.y;
	#endif
	vec4 wpos = mul(invViewProj/*u_invViewProj*/, vec4(clip, 1.0));
	return wpos.xyz / wpos.w;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct EnvironmentTextureData
{
	//samplerCube texture;
	mat3 rotation;
	vec3 multiplier;
};

vec3 getEnvironmentValue(samplerCube tex, EnvironmentTextureData data, vec3 texCoord)
{
	return textureCube(tex, flipCubemapCoords(mul(data.rotation, texCoord))).rgb * data.multiplier;
}

vec3 getEnvironmentValueLod(samplerCube tex, EnvironmentTextureData data, vec3 texCoord, float lod)
{
	return textureCubeLod(tex, flipCubemapCoords(mul(data.rotation, texCoord)), lod).rgb * data.multiplier;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void cutVolumes(vec3 worldPosition)
{
#ifdef GLOBAL_CUT_VOLUME_SUPPORT

	BRANCH
	if(u_viewportCutVolumeSettings.x > 0.0)
	{
		int count = int(u_viewportCutVolumeSettings.x);
		for(int n = 0; n < count; n++)
		{
			mat4 m = u_viewportCutVolumeData[n];
			float shape = m[3][3];
			m[3][3] = 1.0;

			vec3 p = abs(mul(m, vec4(worldPosition, 1.0)).xyz);
			
			if(shape == 0.0)
			{
				//Box
				if(p.x < 0.5 && p.y < 0.5 && p.z < 0.5)
					discard;
			}
			else if(shape == 1.0)
			{
				//Sphere
				if(length(p) < 0.5)
					discard;
			}
			else
			{
				//Cylinder
				if(p.x < 0.5 && length(p.yz) < 0.5)
					discard;
			}
		}
	}

#endif
}
