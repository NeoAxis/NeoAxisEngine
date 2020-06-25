$input a_position, a_normal, a_tangent, a_indices, a_weight, a_texcoord0, a_texcoord1, a_texcoord2, a_texcoord3, a_color0, i_data0, i_data1, i_data2, i_data3
$output v_depth, v_texCoord01, v_color0, v_texCoord23, v_colorParameter

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

uniform vec4 u_renderOperationData[5];
SAMPLER2D(s_bones, 0);

uniform vec4/*vec2*/ u_shadowTexelOffsets;
uniform vec4/*vec3*/ u_cameraPosition;

#ifdef VERTEX_CODE_PARAMETERS
	VERTEX_CODE_PARAMETERS
#endif
#ifdef VERTEX_CODE_SAMPLERS
	VERTEX_CODE_SAMPLERS
#endif
#ifdef VERTEX_CODE_SHADER_SCRIPTS
	VERTEX_CODE_SHADER_SCRIPTS
#endif

void main()
{
	vec3 positionLocal = a_position;
	vec3 normalLocal = a_normal;
	vec4 tangentLocal = a_tangent;
	getAnimationData(u_renderOperationData[0], s_bones, a_indices, a_weight, positionLocal, normalLocal, tangentLocal);
	
	mat4 worldMatrix;
	if(u_renderOperationData[0].y < 0)
	{
		//instancing
		worldMatrix[0] = i_data0;
		worldMatrix[1] = i_data1;
		worldMatrix[2] = i_data2;
		worldMatrix[3] = vec4(0,0,0,1);
		uint data = asuint(i_data3.w);
		v_colorParameter.w = float((data & uint(0xff000000)) >> 24);
		v_colorParameter.z = float((data & uint(0x00ff0000)) >> 16);
		v_colorParameter.y = float((data & uint(0x0000ff00)) >> 8);
		v_colorParameter.x = float((data & uint(0x000000ff)) >> 0);
		v_colorParameter = pow(v_colorParameter / 255.0, 2) * 10;
	}
	else
	{
		worldMatrix = u_model[0];
		v_colorParameter = u_renderOperationData[4];
	}
	
	billboardRotateWorldMatrix(u_renderOperationData[0], worldMatrix, true, u_cameraPosition.xyz);
	vec4 worldPosition = mul(worldMatrix, vec4(positionLocal, 1.0));

	vec2 c_texCoord0 = a_texcoord0;
	vec2 c_texCoord1 = a_texcoord1;
	vec2 c_texCoord2 = a_texcoord2;
	vec2 c_texCoord3 = a_texcoord3;
	vec2 c_unwrappedUV = getUnwrappedUV(c_texCoord0, c_texCoord1, c_texCoord2, c_texCoord3, u_renderOperationData[3].x);
	vec4 c_color0 = (u_renderOperationData[3].y > 0) ? a_color0 : vec4_splat(1);
	vec3 positionOffset = vec3(0,0,0);
#ifdef VERTEX_CODE_BODY
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2D(_sampler, _uv)
	VERTEX_CODE_BODY
	#undef CODE_BODY_TEXTURE2D
#endif
	worldPosition.xyz += positionOffset;


	gl_Position = mul(u_viewProj, worldPosition);
	//!!!!
	//gl_Position.xy += u_shadowTexelOffsets.xy * gl_Position.w; //output.position.xy += texelOffsets.zw * output.position.w;

	#ifdef LIGHT_TYPE_POINT
		v_depth = vec2(length(worldPosition.xyz - u_cameraPosition.xyz), 0);
	#else
		v_depth = vec2(gl_Position.z, gl_Position.w);
	#endif

	v_texCoord01.xy = a_texcoord0;
	v_texCoord01.zw = a_texcoord1;
	v_texCoord23.xy = a_texcoord2;
	v_texCoord23.zw = a_texcoord3;
	v_color0 = (u_renderOperationData[3].y > 0) ? a_color0 : vec4_splat(1);
}





//!!!!был инстансинг

/*
struct PixelOutput
{
	float4 color : SV_TARGET0;
//use real depth buffer
	//float4 depth : SV_TARGET1;
};
*/


/*

void main_vp(
	uniform float4x4 worldMatrix, //instancing specific: initialized from instance data.
	uniform float4x4 viewProjMatrix,
	uniform float4 texelOffsets,
	uniform float3 cameraPosition,

#ifdef INSTANCING
	uniform float instancing,
	//hardware instancing. instance data
	float4 instancingWorldMatrix0 : TEXCOORD5,
	float4 instancingWorldMatrix1 : TEXCOORD6,
	float4 instancingWorldMatrix2 : TEXCOORD7,
#endif

	float4 position : POSITION,

	out float2 oDepth : TEXCOORD0,
	out float4 oPosition : POSITION
	)
{
#ifdef INSTANCING
	//hardware instancing
	if(instancing > 0)
	{
		worldMatrix = float4x4(
			instancingWorldMatrix0,
			instancingWorldMatrix1,
			instancingWorldMatrix2,
			float4(0,0,0,1));
	}
#endif

	float4 worldPosition = mul(worldMatrix, position);
	oPosition = mul(viewProjMatrix, worldPosition);
	oPosition.xy += texelOffsets.zw * oPosition.w;

	#ifdef LIGHT_TYPE_POINT
		oDepth = float2(length(worldPosition.xyz - cameraPosition), 0);
	#else
		oDepth = float2(oPosition.z, oPosition.w);
	#endif
}

void main_fp(
	uniform float farClipDistance,
	uniform float2 shadowDirectionalLightBias,
	uniform float2 shadowSpotLightBias,
	uniform float2 shadowPointLightBias,

	float2 depth : TEXCOORD0,

#if defined(ATI_HARDWARE_SHADOWS) || defined(NVIDIA_HARDWARE_SHADOWS)
	out float oDepth : DEPTH,
#endif
	out float4 oColor : COLOR )
{

	//bias
	float2 bias;
	#ifdef LIGHT_TYPE_DIRECTIONAL
		bias = shadowDirectionalLightBias,
	#endif
	#ifdef LIGHT_TYPE_SPOTLIGHT
		bias = shadowSpotLightBias,
	#endif
	#ifdef LIGHT_TYPE_POINT
		bias = shadowPointLightBias,
	#endif

	#ifdef NVIDIA_HARDWARE_SHADOWS
		depth.x += bias.x + bias.y * fwidth(depth.x);
	#elif defined(ATI_HARDWARE_SHADOWS)
		depth.x += bias.x + bias.y * fwidth(depth.x);
	#else
		#if defined(DIRECT3D)
			depth.x += bias.x + bias.y * fwidth(depth.x);
		#else
			depth.x += bias.x;
		#endif
	#endif

	//oColor, oDepth
#ifdef NVIDIA_HARDWARE_SHADOWS
	oColor = float4(0, 0, 0, 1);
	oDepth = depth.x / depth.y;
#elif defined(ATI_HARDWARE_SHADOWS)
	float normalizedDepth = depth.x / farClipDistance;
	oColor = float4(normalizedDepth, 0, 0, 1);
	oDepth = normalizedDepth;
#else
	float normalizedDepth = depth.x / farClipDistance;
	oColor = float4(normalizedDepth, 0, 0, 1);
#endif

}
*/
