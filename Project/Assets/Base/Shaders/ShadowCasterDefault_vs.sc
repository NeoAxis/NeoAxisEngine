$input a_position, a_indices, a_weight, i_data0, i_data1, i_data2, i_data3, i_data4
$output v_depth, v_worldPosition, v_lodValueVisibilityDistanceReceiveDecals

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "VertexFunctions.sh"

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
	if(u_renderOperationData[0].y < 0.0)
	{
		//instancing
		worldMatrix = mtxFromRows(i_data0, i_data1, i_data2, vec4(0,0,0,1));
		v_lodValueVisibilityDistanceReceiveDecals = i_data4;
	}
	else
	{
		worldMatrix = u_model[0];
		v_lodValueVisibilityDistanceReceiveDecals = vec4(u_renderOperationData[2].w, u_renderOperationData[1].y, u_renderOperationData[1].x, 0);
	}
	
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
	
	v_worldPosition = worldPosition.xyz;
}
