$input a_position, a_normal, a_tangent, a_indices, a_weight, a_texcoord0, a_texcoord1, a_texcoord2, a_color0, a_color3, i_data0, i_data1, i_data2, i_data3, i_data4
$output v_texCoord01, v_color0, v_texCoord23, v_colorParameter, v_worldPosition_depth, v_worldNormal_materialIndex, v_lodValue_visibilityDistance_receiveDecals, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2, glPositionZ

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define SHADOW_CASTER 1
#include "Common.sh"
#include "VertexFunctions.sh"

uniform vec4 u_renderOperationData[8];
uniform vec4 u_materialCustomParameters[2];
uniform vec4 u_objectInstanceParameters[2];
#ifdef GLOBAL_SKELETAL_ANIMATION
	SAMPLER2D(s_bones, 0);
#endif
#ifndef GLSL
	SAMPLER2D(s_linearSamplerVertex, 9);
#endif

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
	uint cullingByCameraDirectionData = uint(0);
	BRANCH
	if(u_renderOperationData[0].y < 0.0)
	{
		//instancing
		worldMatrix = mtxFromRows(i_data0, i_data1, i_data2, vec4(0,0,0,1));
		addTranslate(worldMatrix, u_renderOperationData[7].xyz);
		
		v_lodValue_visibilityDistance_receiveDecals.xy = i_data4.xy;
		uint data2 = asuint(i_data4.z);
		v_lodValue_visibilityDistance_receiveDecals.z = float((data2 & uint(0x000000ff)) >> 0) / 255.0;
		v_lodValue_visibilityDistance_receiveDecals.w = 0.0;//float((data2 & uint(0x0000ff00)) >> 8) / 255.0;
		uint colorExp = ( data2 & uint( 0x00ff0000 ) ) >> 16;
		//v_lodValue_visibilityDistance_receiveDecals = i_data4;
		
		v_colorParameter = decodePackedInstanceColor( i_data3.w, colorExp );
		
		if(v_lodValue_visibilityDistance_receiveDecals.y < 0.0)
			v_lodValue_visibilityDistance_receiveDecals.y = u_renderOperationData[1].y;

		cullingByCameraDirectionData = asuint( i_data4.w );
	}
	else
	{
		worldMatrix = u_model[0];
		v_colorParameter = u_renderOperationData[4];
		v_lodValue_visibilityDistance_receiveDecals = vec4(u_renderOperationData[2].w, u_renderOperationData[1].y, u_renderOperationData[1].x, 0);
		cullingByCameraDirectionData = asuint( u_renderOperationData[3].w );
	}

	//mat4 worldMatrixBeforeChanges = worldMatrix;
	//vec3 worldObjectPositionBeforeChanges = getTranslate(worldMatrix);

#ifdef BILLBOARD	
	vec4 billboardRotation;
	billboardRotateWorldMatrix(u_renderOperationData, worldMatrix, true, u_cameraPosition, billboardRotation);
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
	vec4 instanceParameter1 = u_objectInstanceParameters[0];
	vec4 instanceParameter2 = u_objectInstanceParameters[1];
	vec3 cameraPosition = u_cameraPosition;
#ifdef VERTEX_CODE_BODY
	#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerVertex, _sampler), _uv, u_mipBias)
	#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerVertex, _sampler), _uv, u_mipBias)
	{
		VERTEX_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif
	worldPosition.xyz += positionOffset;

	mat3 worldMatrix3 = toMat3(worldMatrix);

	gl_Position = mul(u_viewProj, worldPosition);
	gl_Position.xy += vec2(u_shadowTexelOffset, u_shadowTexelOffset) * gl_Position.w; //output.position.xy += texelOffsets.zw * output.position.w;

	//!!!!special for mobile
#ifdef GLSL
	glPositionZ = gl_Position.z;
#endif
	//#ifdef LIGHT_TYPE_POINT
	//	v_depth = vec2(length(worldPosition.xyz - u_cameraPosition), 0);
	//#else
	//	v_depth = vec2(gl_Position.z, gl_Position.w);
	//#endif

	v_texCoord01.xy = texCoord0;
	v_texCoord01.zw = texCoord1;
	v_texCoord23.xy = texCoord2;
	v_texCoord23.zw = vec2_splat(0);//texCoord3;
	v_color0 = color0;
	
	v_worldPosition_depth.xyz = worldPosition.xyz;
	v_worldPosition_depth.w = 0.0;
	v_worldNormal_materialIndex.xyz = normalize(mul(toMat3(worldMatrix), normalLocal));
	v_worldNormal_materialIndex.w = a_color3;

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
		vec3 dir = normalize( u_cameraPosition - worldPosition.xyz );
		float _cos = dot( dir, cullingNormal );
		float _acos = acos( clamp( _cos, -1.0, 1.0 ) );
		if( _acos > PI / 2.0 + data.w * PI / 2.0 )
			gl_Position.x = 0.0 / 0.0;
	}
#endif
	
	//geometry with voxel data
#if (defined(GLOBAL_VOXEL_LOD) && defined(VOXEL)) || (defined(GLOBAL_VIRTUALIZED_GEOMETRY) && defined(VIRTUALIZED))
	v_objectSpacePosition = positionLocal;
	voxelOrVirtualizedDataModeCalculateParametersV(u_renderOperationData, worldMatrix, u_cameraPosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2);
#endif
}
