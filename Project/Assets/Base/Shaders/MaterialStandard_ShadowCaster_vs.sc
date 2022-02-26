$input a_position, a_normal, a_tangent, a_indices, a_weight, a_texcoord0, a_texcoord1, a_texcoord2, a_color0, i_data0, i_data1, i_data2, i_data3, i_data4
$output v_depth, v_texCoord01, v_color0, v_texCoord23, v_colorParameter, v_worldPosition_depth, v_worldNormal, v_lodValue_visibilityDistance_receiveDecals, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles, v_billboardRotation

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define SHADOW_CASTER 1
#include "Common.sh"
#include "VertexFunctions.sh"

uniform vec4 u_renderOperationData[5];
uniform vec4 u_materialCustomParameters[2];
#ifdef GLOBAL_SKELETAL_ANIMATION
	SAMPLER2D(s_bones, 0);
#endif

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
#ifdef GLOBAL_SKELETAL_ANIMATION
	getAnimationData(u_renderOperationData[0], s_bones, a_indices, a_weight, positionLocal, normalLocal, tangentLocal);
#endif
	
	mat4 worldMatrix;
	if(u_renderOperationData[0].y < 0.0)
	{
		//instancing
		worldMatrix = mtxFromRows(i_data0, i_data1, i_data2, vec4(0,0,0,1));
		uint data = asuint(i_data3.w);
		v_colorParameter.w = float((data & uint(0xff000000)) >> 24);
		v_colorParameter.z = float((data & uint(0x00ff0000)) >> 16);
		v_colorParameter.y = float((data & uint(0x0000ff00)) >> 8);
		v_colorParameter.x = float((data & uint(0x000000ff)) >> 0);
		v_colorParameter = pow2(v_colorParameter / 255.0, 2.0) * 10.0;
		v_lodValue_visibilityDistance_receiveDecals = i_data4;
		if(v_lodValue_visibilityDistance_receiveDecals.y < 0.0)
			v_lodValue_visibilityDistance_receiveDecals.y = u_renderOperationData[1].y;
	}
	else
	{
		worldMatrix = u_model[0];
		v_colorParameter = u_renderOperationData[4];
		v_lodValue_visibilityDistance_receiveDecals = vec4(u_renderOperationData[2].w, u_renderOperationData[1].y, u_renderOperationData[1].x, 0);
	}
	
	vec3 worldObjectPositionBeforeChanges = vec3(worldMatrix[0][3], worldMatrix[1][3], worldMatrix[2][3]);

#ifdef BILLBOARD	
	vec4 billboardRotation;
	billboardRotateWorldMatrix(u_renderOperationData, worldMatrix, true, u_cameraPosition.xyz, billboardRotation);
#endif
	vec4 worldPosition = mul(worldMatrix, vec4(positionLocal, 1.0));

	vec2 texCoord0 = a_texcoord0;
	vec2 texCoord1 = a_texcoord1;
	vec2 texCoord2 = a_texcoord2;
	//vec2 texCoord3 = a_texcoord3;
	vec2 unwrappedUV = getUnwrappedUV(texCoord0, texCoord1, texCoord2/*, texCoord3*/, u_renderOperationData[3].x);
	vec4 color0 = (u_renderOperationData[3].y > 0.0) ? a_color0 : vec4_splat(1);
	vec3 positionOffset = vec3(0,0,0);
	vec4 customParameter1 = u_materialCustomParameters[0];
	vec4 customParameter2 = u_materialCustomParameters[1];
#ifdef VERTEX_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(_sampler, _uv, 0, 0)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2D(_sampler, _uv)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2D(_sampler, _uv)
	VERTEX_CODE_BODY
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif
#ifdef GLOBAL_BILLBOARD_DATA
	float billboardDataMode = u_renderOperationData[1].w;
	/* PSSM is broken
	//billboard with geometry data mode: add position offset
	BRANCH
	if(billboardDataMode != 0.0)
	{
		//works only for square billboards
		float vertexDistance = length(positionLocal);		
		float meshGeometryIndex = u_renderOperationData[3].w;
		float offset = vertexDistance * 0.25 + meshGeometryIndex * vertexDistance * 0.05;
		positionOffset += normalize(u_viewportOwnerCameraPosition - worldObjectPositionBeforeChanges) * offset;
	}
	*/
#endif
	worldPosition.xyz += positionOffset;

	mat3 worldMatrix3 = toMat3(worldMatrix);

	gl_Position = mul(u_viewProj, worldPosition);
	gl_Position.xy += u_shadowTexelOffsets.xy * gl_Position.w; //output.position.xy += texelOffsets.zw * output.position.w;

	#ifdef LIGHT_TYPE_POINT
		v_depth = vec2(length(worldPosition.xyz - u_cameraPosition.xyz), 0);
	#else
		v_depth = vec2(gl_Position.z, gl_Position.w);
	#endif

	v_texCoord01.xy = texCoord0;
	v_texCoord01.zw = texCoord1;
	v_texCoord23.xy = texCoord2;
	v_texCoord23.zw = vec2_splat(0);//texCoord3;
	v_color0 = color0;
	
	v_worldPosition_depth.xyz = worldPosition.xyz;
	v_worldPosition_depth.w = 0.0;
	v_worldNormal = normalize(mul(toMat3(worldMatrix), normalLocal));
	
#ifdef BILLBOARD	
#ifdef GLOBAL_BILLBOARD_DATA
	//billboard with geometry data mode
	billboardDataModeCalculateParameters(billboardDataMode, worldObjectPositionBeforeChanges, billboardRotation, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles);
	v_billboardRotation = mat3ToQuat(worldMatrix3);
#endif
#endif
}
