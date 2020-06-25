$input a_position, a_normal, a_tangent, a_texcoord0, a_texcoord1, a_texcoord2, a_texcoord3, a_color0, a_indices, a_weight, i_data0, i_data1, i_data2, i_data3
$output v_texCoord01, v_worldPosition, v_worldNormal, v_depth, v_tangent, v_bitangent, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_texCoord23, v_colorParameter

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "UniformsVertex.sh"

uniform vec4 u_renderOperationData[5];
SAMPLER2D(s_bones, 0);

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
	
	billboardRotateWorldMatrix(u_renderOperationData[0], worldMatrix, false, vec3_splat(0));
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
	v_texCoord01.xy = a_texcoord0;
	v_texCoord01.zw = a_texcoord1;
	v_texCoord23.xy = a_texcoord2;
	v_texCoord23.zw = a_texcoord3;
	v_worldPosition = worldPosition.xyz;
	v_worldNormal = normalize(mul((mat3)worldMatrix, normalLocal));
	v_depth = gl_Position.z;

	v_tangent.xyz = normalize(mul((mat3)worldMatrix, tangentLocal.xyz));
	v_tangent.w = tangentLocal.w;

	v_bitangent = cross(v_tangent.xyz, v_worldNormal) * tangentLocal.w;

	//v_reflectionVector = v_worldPosition - u_viewportOwnerCameraPosition;
	
	v_color0 = (u_renderOperationData[3].y > 0) ? a_color0 : vec4_splat(1);
	
	//displacement
#ifdef DISPLACEMENT
	{
		vec3 eyeWorldSpace = worldPosition.xyz - u_viewportOwnerCameraPosition;

		vec3 normalizedNormal = normalize(normalLocal);
		vec3 normalizedTangent = normalize(tangentLocal.xyz);
		vec3 binormal = normalize(cross( normalizedNormal, normalizedTangent ) * tangentLocal.w);
	
		mat3 tangentToWorldSpace;
		tangentToWorldSpace[0] = mul( worldMatrix, normalizedTangent );
		tangentToWorldSpace[1] = mul( worldMatrix, binormal );
		tangentToWorldSpace[2] = mul( worldMatrix, normalizedNormal );	
		mat3 worldToTangentSpace = transpose(tangentToWorldSpace);
		
		v_eyeTangentSpace = mul( eyeWorldSpace, worldToTangentSpace );
		v_normalTangentSpace = mul( v_worldNormal, worldToTangentSpace );
	}
#endif
	
}
