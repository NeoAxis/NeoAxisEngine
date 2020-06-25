// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common/bgfx_shader.sh"
#include "Common/shaderlib.sh"
#include "GlobalSettings.sh"
#include "Constants.sh"
#include "UniformsGeneral.sh"
#include "Extension.sh"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef PI
#define PI 3.14159265359
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define DebugMode_None 0
#define DebugMode_Wireframe 1
#define DebugMode_Geometry 2
#define DebugMode_Surface 3
#define DebugMode_BaseColor 4
#define DebugMode_Metallic 5
#define DebugMode_Roughness 6
#define DebugMode_Reflectance 7
#define DebugMode_Emissive 8
#define DebugMode_Normal 9
//#define DebugMode_NormalMap 7
//#define DebugMode_AmbientOcclusionMap 8
//#define DebugMode_SpecialF0 9

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//lightAttenuation: x = near; y = far; z = power; w = far - near.
float getLightAttenuation(vec4 lightAttenuation, float distance)
{
	return saturate(pow(1 - min( (distance - lightAttenuation.x) / lightAttenuation.w, 1), lightAttenuation.z));
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec3 expand(vec3 v)
{
	return v * 2 - 1;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec4 blendOpaque(vec4 srcColor, vec4 destColor)
{
	return srcColor;
}

vec4 blendAdd(vec4 srcColor, vec4 destColor)
{
	return srcColor + destColor;
}

vec4 blendModulate(vec4 srcColor, vec4 destColor)
{
	return srcColor * destColor;
}

vec4 blendColorBlend(vec4 srcColor, vec4 destColor)
{
	return srcColor * srcColor + destColor * (1.0 - srcColor);
}

vec4 blendAlphaBlend(vec4 srcColor, vec4 destColor)
{
	return srcColor * srcColor.a + destColor * (1.0 - srcColor.a);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec3 flipCubemapCoords(vec3 uvw)
{
	return vec3(-uvw.y, uvw.z, uvw.x);
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
//!!!!new

	float clipDepth = toClipSpaceDepth(depthTextureValue);
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	return near * far / (far - clipDepth * (far - near));
#else
	return 2.0 * near * far / (far + near - clipDepth * (far - near));
#endif // BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL

//	float value2 = toClipSpaceDepth(depthTextureValue);
//	return 2.0 * near * far / (far + near - value2 * (far - near));
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef GLOBAL_FOG_SUPPORT
float getFogFactor(vec3 worldPosition)
{
	float fog = 0;

	int distanceMode = (int)u_fogDistanceMode;
	int heightMode = (int)u_fogHeightMode;
	//!!!!BRANCH?
	if(distanceMode != 0 || heightMode !=0)
	{
		fog = 1;

		//Exp, Exp2
		bool modeExp = distanceMode == 1;
		bool modeExp2 = distanceMode == 2;
		if(modeExp || modeExp2)
		{
			float cameraDistance = length(worldPosition - u_viewportOwnerCameraPosition);

			float distance = cameraDistance - u_fogStartDistance;
			if(distance < 0) distance = 0;
			float m = distance * u_fogDensity;
			if( modeExp2 )
				m *= m;
			fog = 1.0 - saturate(1.0 / exp( m * log( 2.718281828 )));
		}

		//Height
		if(heightMode != 0)
			fog *= saturate((u_fogHeight - worldPosition.z) / u_fogHeightScale);

		fog *= u_fogColor.w;
	}

	return 1.0 - fog;
}
#endif

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

mat4 getBoneTransform(sampler2D bones, int index)
{
	vec4 item0 = texelFetch(bones, ivec2(0, index), 0);
	vec4 item1 = texelFetch(bones, ivec2(1, index), 0);
	vec4 item2 = texelFetch(bones, ivec2(2, index), 0);
	vec4 item3 = texelFetch(bones, ivec2(3, index), 0);
	return transpose(mat4(item0, item1, item2, item3));
}

void getAnimationData(vec4 renderOperationData, sampler2D bones, uvec4 indices, vec4 weight, inout vec3 position, inout vec3 normal, inout vec4 tangent)
{
	BRANCH
	if(renderOperationData.y > 0)
	{
		mat4 transform = 
			getBoneTransform(bones, indices.x) * weight.x +
			getBoneTransform(bones, indices.y) * weight.y +
			getBoneTransform(bones, indices.z) * weight.z +
			getBoneTransform(bones, indices.w) * weight.w;
		mat3 transform3 = (mat3)transform;		
		position = (mul(transform, vec4(position, 1.0))).xyz;
		normal = normalize(mul(transform3, normal));
		tangent.xyz = normalize(mul(transform3, tangent.xyz));
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

mat3 makeRotationMatrix(float angle, vec3 axis)
{
    float c, s;
	sincos(angle, s, c);

    float t = 1 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    return mat3(
        t * x * x + c, t * x * y - s * z, t * x * z + s * y,
        t * x * y + s * z, t * y * y + c, t * y * z - s * x,
        t * x * z - s * y, t * y * z + s * x, t * z * z + c
    );
}

vec3 getScaleFromMatrix(mat3 m)
{
    float sx = length(vec3(m[0][0], m[0][1], m[0][2]));
    float sy = length(vec3(m[1][0], m[1][1], m[1][2]));
    float sz = length(vec3(m[2][0], m[2][1], m[2][2]));
    return vec3(sx, sy, sz);
}

void billboardRotateWorldMatrix(vec4 renderOperationData, inout mat4 worldMatrix, bool shadowCaster, vec3 shadowCasterCameraPosition)
{
	BRANCH
	if(renderOperationData.z != 0)
	{
		//get rotation value and restore matrix
		float rotation = worldMatrix[1][0];
		worldMatrix[1][0] = 0;
		
		//face to camera
		{
			int mode = (int)renderOperationData.z;
			
			mat3 rotationMatrix2;
			switch(mode)
			{
			case 1:
				rotationMatrix2 = makeRotationMatrix(-PI/2, vec3(1,0,0));
				break;
			case 2:
				rotationMatrix2 = mul(makeRotationMatrix(PI, vec3(0,0,1)), makeRotationMatrix(PI/2, vec3(1,0,0)));
				break;
			case 3:
				rotationMatrix2 = mul(makeRotationMatrix(-PI/2, vec3(0,0,1)), makeRotationMatrix(-PI/2, vec3(0,1,0)));
				break;
			case 4:
			default:
				rotationMatrix2 = mul(makeRotationMatrix(PI/2, vec3(0,0,1)), makeRotationMatrix(PI/2, vec3(0,1,0)));
				break;				
			}

			//apply rotation parameter
			rotationMatrix2 = mul(rotationMatrix2, makeRotationMatrix(-rotation, vec3(0,1,0)));
			
			mat3 rotationMatrix = mul(transpose((mat3)u_view), rotationMatrix2);
			
			mat3 m = mul(rotationMatrix, (mat3)worldMatrix);
			for(int y=0;y<3;y++)
				for(int x=0;x<3;x++)
					worldMatrix[x][y] = m[x][y];
		}
			
		//add offset to shadow caster
		BRANCH
		if(shadowCaster)
		{
			vec3 worldPosition;
			for(int n=0;n<3;n++)
				worldPosition[n] = worldMatrix[n][3];
			
			vec3 scale = getScaleFromMatrix((mat3)worldMatrix);
			float scaleFloat = max(max(scale.x, scale.y), scale.z);
			
			vec3 direction = normalize(worldPosition - shadowCasterCameraPosition);
			vec3 offset = direction * renderOperationData.w * scaleFloat;
			worldPosition += offset;
			
			for(int n2=0;n2<3;n2++)
				worldMatrix[n2][3] = worldPosition[n2];
		}
	}	
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
	float v = source * 2 - 1;
	v1 = abs(v) > 0.5;
	v2 = v > 0;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec4 encodeGBuffer4(vec4 tangent, bool shadingModelSimple, bool receiveDecals)
{
	vec4 result;
	
	result.xyz = tangent.xyz * 0.5 + 0.5;
	if(shadingModelSimple)
		result.xyz = vec3_splat(0);
	
	result.w = encodeBitsToByte(tangent.w > 0, receiveDecals);
	
	return result;
}

void decodeGBuffer4(vec4 source, out vec4 tangent, out bool shadingModelSimple, out bool receiveDecals)
{
	bool tangentW;
	decodeBitsFromByte(source.w, tangentW, receiveDecals);
	
	tangent = vec4(normalize(source.xyz * 2 - 1), tangentW ? 1 : -1);
	
	shadingModelSimple = !any(source.xyz);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec2 getUnwrappedUV(vec2 texCoord0, vec2 texCoord1, vec2 texCoord2, vec2 texCoord3, float unwrappedUV)
{
	if(unwrappedUV == 1)return texCoord0;
	if(unwrappedUV == 2)return texCoord1;
	if(unwrappedUV == 3)return texCoord2;
	if(unwrappedUV == 4)return texCoord3;
	return vec2_splat(0);
}
