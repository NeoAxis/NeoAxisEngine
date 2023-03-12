$input a_position, a_color0, a_color1
$output v_worldPosition_depth, v_colorVisible, v_colorInvisible

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "VertexFunctions.sh"

uniform vec4/*vec3*/ u_cameraPosition;

uniform vec4 u_simple3DRendererVertex[3];
#define u_color u_simple3DRendererVertex[0]
#define u_colorInvisibleBehindObjects u_simple3DRendererVertex[1]
#define u_useColorFromUniform u_simple3DRendererVertex[2].x
#define u_depthTextureAvailable u_simple3DRendererVertex[2].y
#define u_billboardMode u_simple3DRendererVertex[2].z

void main()
{
	mat4 worldMatrix = u_model[0];
	vec4 renderOperationData[7];	
	renderOperationData[0] = vec4(0,0,u_billboardMode,0);
	renderOperationData[1] = vec4_splat(0);
	renderOperationData[2] = vec4_splat(0);
	renderOperationData[3] = vec4_splat(0);
	renderOperationData[4] = vec4_splat(0);	
	renderOperationData[5] = vec4_splat(0);	
	renderOperationData[6] = vec4_splat(0);	
	//vec4 renderOperationData = vec4(0,0,u_billboardMode,0);
	vec4 billboardRotation;
	billboardRotateWorldMatrix(renderOperationData, worldMatrix, false, vec3_splat(0), billboardRotation);
	
	vec4 worldPosition = mul(worldMatrix, vec4(a_position, 1.0));

	//const float depthOffset = -.05f;

	//vec3 dir = normalize(worldPosition.xyz - u_cameraPosition.xyz);
	//worldPosition.xyz += dir * depthOffset;
	gl_Position = mul(u_viewProj, worldPosition);
	v_worldPosition_depth.w = gl_Position.z;

	//gl_Position = mul(u_modelViewProj, vec4(a_position, 1.0));

	if(u_useColorFromUniform > 0.0)
	{
		v_colorVisible = u_color;
		v_colorInvisible = u_colorInvisibleBehindObjects;
	}
	else
	{
		v_colorVisible = a_color0;
		v_colorInvisible = a_color1;
	}
	
	v_worldPosition_depth.xyz = worldPosition.xyz;
}
