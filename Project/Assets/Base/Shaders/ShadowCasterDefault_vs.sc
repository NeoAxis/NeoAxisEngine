$input a_position, a_indices, a_weight, i_data0, i_data1, i_data2
$output v_depth

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

uniform vec4 u_renderOperationData[5];
SAMPLER2D(s_bones, 0);

uniform vec4/*vec2*/ u_shadowTexelOffsets;
uniform vec4/*vec3*/ u_cameraPosition;

void main()
{
	vec3 positionLocal = a_position;
	vec3 normalLocal = vec3_splat(0);
	vec4 tangentLocal = vec4_splat(0);
	getAnimationData(u_renderOperationData[0], s_bones, a_indices, a_weight, positionLocal, normalLocal, tangentLocal);

	mat4 worldMatrix;
	if(u_renderOperationData[0].y < 0)
	{
		//instancing
		worldMatrix[0] = i_data0;
		worldMatrix[1] = i_data1;
		worldMatrix[2] = i_data2;
		worldMatrix[3] = vec4(0,0,0,1);
	}
	else
		worldMatrix = u_model[0];
	
	billboardRotateWorldMatrix(u_renderOperationData[0], worldMatrix, true, u_cameraPosition.xyz);
	vec4 worldPosition = mul(worldMatrix, vec4(positionLocal, 1.0));

	gl_Position = mul(u_viewProj, worldPosition);
	//!!!!
	gl_Position.xy += u_shadowTexelOffsets.xy * gl_Position.w; //output.position.xy += texelOffsets.zw * output.position.w;

	#ifdef LIGHT_TYPE_POINT
		v_depth = vec2(length(worldPosition.xyz - u_cameraPosition.xyz), 0);
	#else
		v_depth = vec2(gl_Position.z, gl_Position.w);
	#endif
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
