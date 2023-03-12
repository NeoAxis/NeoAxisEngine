$input a_position, a_normal, a_tangent, a_texcoord0, a_texcoord1, a_texcoord2, a_color0, a_color3, a_indices, a_weight, i_data0, i_data1, i_data2, i_data3, i_data4
$output v_texCoord01, v_worldPosition_depth, v_worldNormal_materialIndex, v_tangent, v_fogFactor, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_position, v_previousPosition, v_texCoord23, v_colorParameter, v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define FORWARD 1
#include "Common.sh"
#include "UniformsVertex.sh"
#include "VertexFunctions.sh"

uniform vec4 u_renderOperationData[7];
uniform vec4 u_materialCustomParameters[2];
uniform mat4 u_viewProjPrevious;
#ifdef GLOBAL_SKELETAL_ANIMATION
	SAMPLER2D(s_bones, 0);
#endif
#ifndef GLSL
SAMPLER2D(s_linearSamplerVertex, 9);
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
	uint cullingByCameraDirectionData = uint(0);
	BRANCH
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
		
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.xy = i_data4.xy;
		uint data2 = asuint(i_data4.z);
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.z = float((data2 & uint(0x000000ff)) >> 0) / 255.0;
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.w = float((data2 & uint(0x0000ff00)) >> 8) / 255.0;
		//v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor = i_data4;
		
		if(v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y < 0.0)
			v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y = u_renderOperationData[1].y;
		
		cullingByCameraDirectionData = asuint( i_data4.w );
	}
	else
	{
		worldMatrix = u_model[0];
		previousWorldPosition = u_renderOperationData[2].xyz;
		v_colorParameter = u_renderOperationData[4];
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor = vec4(u_renderOperationData[2].w, u_renderOperationData[1].y, u_renderOperationData[1].x, u_renderOperationData[1].z);
		//!!!!?
		//cullingByCameraDirectionData = 0;
	}

	//mat4 worldMatrixBeforeChanges = worldMatrix;
	//vec3 worldObjectPositionBeforeChanges = getTranslate(worldMatrix);
	
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
	vec3 cameraPosition = u_viewportOwnerCameraPosition;
#ifdef VERTEX_CODE_BODY
	#define CODE_BODY_TEXTURE2D_MASK_OPACITY(_sampler, _uv) texture2DMaskOpacity(makeSampler(s_linearSamplerVertex, _sampler), _uv, 0, 0)
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerVertex, _sampler), _uv, u_removeTextureTiling)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerVertex, _sampler), _uv, u_mipBias)
	{
		VERTEX_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_MASK_OPACITY
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif
	worldPosition.xyz += positionOffset;

	mat3 worldMatrix3 = toMat3(worldMatrix);
	
	gl_Position = mul(u_viewProj, worldPosition);
	v_texCoord01.xy = texCoord0;
	v_texCoord01.zw = texCoord1;
	v_texCoord23.xy = texCoord2;
	v_texCoord23.zw = vec2_splat(0);//texCoord3;
	v_worldPosition_depth.xyz = worldPosition.xyz;
	v_worldNormal_materialIndex.xyz = normalize(mul(toMat3(worldMatrix), normalLocal));
	v_worldNormal_materialIndex.w = a_color3;
	v_worldPosition_depth.w = gl_Position.z;
	v_tangent.xyz = normalize(mul(worldMatrix3, tangentLocal.xyz));
	v_tangent.w = tangentLocal.w;

	//!!!!GLSL
#ifndef GLSL
	BRANCH
	if( cullingByCameraDirectionData != 0 )
	{
		vec4 data;
		data.x = float((cullingByCameraDirectionData & uint(0x000000ff)) >> 0);
		data.y = float((cullingByCameraDirectionData & uint(0x0000ff00)) >> 8);
		data.z = float((cullingByCameraDirectionData & uint(0x00ff0000)) >> 16);
		data.w = float((cullingByCameraDirectionData & uint(0xff000000)) >> 24);
		data = data / 255.0;
		
		vec3 cullingNormal = normalize( expand( data.xyz ) );
		vec3 dir = normalize( u_viewportOwnerCameraPosition - worldPosition.xyz );
		float _cos = dot( dir, cullingNormal );
		float _acos = acos( clamp( _cos, -1.0, 1.0 ) );
		if( _acos > PI / 2.0 + data.w * PI / 2.0 )
			gl_Position.x = 0.0 / 0.0;
	}
#endif
	
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
		v_normalTangentSpace = mul( v_worldNormal_materialIndex.xyz, worldToTangentSpace );
	}
#endif

	//motion vector
#ifdef GLOBAL_MOTION_VECTOR
	v_position = gl_Position;
	mat4 previousWorldMatrix = worldMatrix;
	setTranslate(previousWorldMatrix, previousWorldPosition);
	vec4 previousPosition = mul(previousWorldMatrix, vec4(positionLocal, 1));
	previousPosition.xyz += positionOffset;
	v_previousPosition = mul(u_viewProjPrevious, vec4(previousPosition.xyz,1));
#endif
	
	//geometry with voxel data
#if defined(GLOBAL_VOXEL_LOD) || defined(GLOBAL_VIRTUALIZED_GEOMETRY)
	v_objectSpacePosition = positionLocal;
	voxelOrVirtualizedDataModeCalculateParametersV(u_renderOperationData, worldMatrix, u_viewportOwnerCameraPosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2);
#endif
}
