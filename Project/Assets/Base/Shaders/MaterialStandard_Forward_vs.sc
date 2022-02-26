$input a_position, a_normal, a_tangent, a_texcoord0, a_texcoord1, a_texcoord2, a_color0, a_indices, a_weight, i_data0, i_data1, i_data2, i_data3, i_data4
$output v_texCoord01, v_worldPosition_depth, v_worldNormal, v_tangent, v_bitangent, v_fogFactor, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_position, v_previousPosition, v_texCoord23, v_colorParameter, v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles, v_billboardRotation

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define FORWARD 1
#include "Common.sh"
#include "UniformsVertex.sh"
#include "VertexFunctions.sh"

uniform vec4 u_renderOperationData[5];
uniform vec4 u_materialCustomParameters[2];
uniform mat4 u_viewProjPrevious;
#ifdef GLOBAL_SKELETAL_ANIMATION
	SAMPLER2D(s_bones, 0);
#endif

//uniform vec4 u_lightDataVertex[LIGHTDATA_VERTEX_SIZE];

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
	vec3 previousWorldPosition;
	if(u_renderOperationData[0].y < 0.0)
	{
		//instancing
		worldMatrix = mtxFromRows(i_data0, i_data1, i_data2, vec4(0,0,0,1));
		previousWorldPosition = i_data3.xyz;
		uint data = asuint(i_data3.w);
		v_colorParameter.w = float((data & uint(0xff000000)) >> 24);
		v_colorParameter.z = float((data & uint(0x00ff0000)) >> 16);
		v_colorParameter.y = float((data & uint(0x0000ff00)) >> 8);
		v_colorParameter.x = float((data & uint(0x000000ff)) >> 0);
		v_colorParameter = pow(v_colorParameter / 255.0, vec4_splat(2.0)) * 10.0;
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor = i_data4;
		if(v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y < 0.0)
			v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y = u_renderOperationData[1].y;
	}
	else
	{
		worldMatrix = u_model[0];
		previousWorldPosition = u_renderOperationData[2].xyz;
		v_colorParameter = u_renderOperationData[4];
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor = vec4(u_renderOperationData[2].w, u_renderOperationData[1].y, u_renderOperationData[1].x, u_renderOperationData[1].z);
	}

#ifdef GLSL
	vec3 worldObjectPositionBeforeChanges = vec3(worldMatrix[3][0], worldMatrix[3][1], worldMatrix[3][2]);
#else
	vec3 worldObjectPositionBeforeChanges = vec3(worldMatrix[0][3], worldMatrix[1][3], worldMatrix[2][3]);
#endif
	
	vec4 billboardRotation;
	billboardRotateWorldMatrix(u_renderOperationData, worldMatrix, false, vec3_splat(0), billboardRotation);
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
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(_sampler, _uv, u_removeTextureTiling)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(_sampler, _uv, u_mipBias)
	VERTEX_CODE_BODY
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif
#ifdef GLOBAL_BILLBOARD_DATA
	float billboardDataMode = u_renderOperationData[1].w;
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
#endif
	worldPosition.xyz += positionOffset;

	mat3 worldMatrix3 = toMat3(worldMatrix);
	
	gl_Position = mul(u_viewProj, worldPosition);
	v_texCoord01.xy = texCoord0;
	v_texCoord01.zw = texCoord1;
	v_texCoord23.xy = texCoord2;
	v_texCoord23.zw = vec2_splat(0);//texCoord3;
	v_worldPosition_depth.xyz = worldPosition.xyz;
	v_worldNormal = normalize(mul(toMat3(worldMatrix), normalLocal));
	v_worldPosition_depth.w = gl_Position.z;
	v_tangent = normalize(mul(toMat3(worldMatrix), tangentLocal.xyz));
	v_bitangent = cross(v_tangent.xyz, v_worldNormal) * tangentLocal.w;

	//v_reflectionVector = v_worldPosition - u_viewportOwnerCameraPosition;

	//fog
#ifdef GLOBAL_FOG
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)
		v_fogFactor = getFogFactor(v_worldPosition_depth.xyz, 1.0);
	#endif
#endif

	v_color0 = color0;

	//displacement
#ifdef DISPLACEMENT
	{
		vec3 eyeWorldSpace = worldPosition.xyz - u_viewportOwnerCameraPosition;

		vec3 normalizedNormal = normalize(normalLocal);
		vec3 normalizedTangent = normalize(tangentLocal.xyz);
		vec3 binormal = normalize(cross( normalizedNormal, normalizedTangent ) * tangentLocal.w);
	
		mat3 tangentToWorldSpace;
		tangentToWorldSpace[0] = mul( worldMatrix3, normalizedTangent );
		tangentToWorldSpace[1] = mul( worldMatrix3, binormal );
		tangentToWorldSpace[2] = mul( worldMatrix3, normalizedNormal );	
#ifdef GLSL
		mat3 worldToTangentSpace = tangentToWorldSpace;
#else
		mat3 worldToTangentSpace = transpose(tangentToWorldSpace);
#endif
		
		v_eyeTangentSpace = mul( eyeWorldSpace, worldToTangentSpace );
		v_normalTangentSpace = mul( v_worldNormal, worldToTangentSpace );
	}
#endif

	//motion vector
#ifdef GLOBAL_MOTION_VECTOR
	v_position = gl_Position;
	mat4 previousWorldMatrix = worldMatrix;
	#ifdef GLSL
		previousWorldMatrix[3][0] = previousWorldPosition[0];
		previousWorldMatrix[3][1] = previousWorldPosition[1];
		previousWorldMatrix[3][2] = previousWorldPosition[2];
	#else	
		previousWorldMatrix[0][3] = previousWorldPosition[0];
		previousWorldMatrix[1][3] = previousWorldPosition[1];
		previousWorldMatrix[2][3] = previousWorldPosition[2];
	#endif
	vec4 previousPosition = mul(previousWorldMatrix, vec4(positionLocal, 1));
	previousPosition.xyz += positionOffset;
	v_previousPosition = mul(u_viewProjPrevious, vec4(previousPosition.xyz,1));
#endif
	
	//billboard with geometry data mode
#ifdef GLOBAL_BILLBOARD_DATA
	billboardDataModeCalculateParameters(billboardDataMode, worldObjectPositionBeforeChanges, billboardRotation, v_billboardDataIndexes, v_billboardDataFactors, v_billboardDataAngles);
	v_billboardRotation = mat3ToQuat(worldMatrix3);
#endif
}
