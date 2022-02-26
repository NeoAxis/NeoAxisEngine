$input a_position, a_indices, a_weight, a_texcoord0, i_data0, i_data1, i_data2, i_data3, i_data4
$output v_depth, v_texCoord0, v_worldPosition, v_lodValue_visibilityDistance_receiveDecals, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles, v_billboardRotation

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define SHADOW_CASTER 1
#include "Common.sh"
#include "VertexFunctions.sh"

uniform vec4 u_renderOperationData[5];
#ifdef GLOBAL_SKELETAL_ANIMATION
	SAMPLER2D(s_bones, 0);
#endif

uniform vec4/*vec2*/ u_shadowTexelOffsets;
uniform vec4/*vec3*/ u_cameraPosition;

void main()
{
	vec3 positionLocal = a_position;
	vec3 normalLocal = vec3_splat(0);
	vec4 tangentLocal = vec4_splat(0);
#ifdef GLOBAL_SKELETAL_ANIMATION
	getAnimationData(u_renderOperationData[0], s_bones, a_indices, a_weight, positionLocal, normalLocal, tangentLocal);
#endif

	mat4 worldMatrix;
	if(u_renderOperationData[0].y < 0.0)
	{
		//instancing
		worldMatrix = mtxFromRows(i_data0, i_data1, i_data2, vec4(0,0,0,1));
		v_lodValue_visibilityDistance_receiveDecals = i_data4;
		if(v_lodValue_visibilityDistance_receiveDecals.y < 0.0)
			v_lodValue_visibilityDistance_receiveDecals.y = u_renderOperationData[1].y;
	}
	else
	{
		worldMatrix = u_model[0];
		v_lodValue_visibilityDistance_receiveDecals = vec4(u_renderOperationData[2].w, u_renderOperationData[1].y, u_renderOperationData[1].x, 0);
	}
	
	vec3 worldObjectPositionBeforeChanges = vec3(worldMatrix[0][3], worldMatrix[1][3], worldMatrix[2][3]);
	
#ifdef BILLBOARD
	vec4 billboardRotation;
	billboardRotateWorldMatrix(u_renderOperationData, worldMatrix, true, u_cameraPosition.xyz, billboardRotation);
#endif
	vec4 worldPosition = mul(worldMatrix, vec4(positionLocal, 1.0));

	vec3 positionOffset = vec3(0,0,0);
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

	v_texCoord0 = a_texcoord0;
	
	v_worldPosition = worldPosition.xyz;

#ifdef BILLBOARD
#ifdef GLOBAL_BILLBOARD_DATA
	//billboard with geometry data mode
	billboardDataModeCalculateParameters(billboardDataMode, worldObjectPositionBeforeChanges, billboardRotation, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles);
	v_billboardRotation = mat3ToQuat(worldMatrix3);
	//v_billboardRotation = billboardRotation;
#endif
#endif
}
