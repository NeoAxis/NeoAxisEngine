$input a_position, a_normal, a_tangent, a_texcoord0, a_texcoord1, a_texcoord2, a_color0, a_color2, a_color3, a_indices, a_weight, i_data0, i_data1, i_data2, i_data3, i_data4
$output v_texCoord01, v_worldPosition_depth, v_worldNormal_materialIndex, v_tangent, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_position, v_previousPosition, v_texCoord23, v_colorParameter, v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#define DEFERRED 1
#include "Common.sh"
#include "UniformsVertex.sh"
#include "VertexFunctions.sh"

uniform vec4 u_renderOperationData[8];
uniform vec4 u_materialCustomParameters[2];
uniform vec4 u_objectInstanceParameters[2];
#ifdef GLOBAL_SKELETAL_ANIMATION
	SAMPLER2D(s_bones, 0);
#endif
SAMPLER2D(s_linearSamplerVertex, 9);

#ifdef VERTEX_CODE_PARAMETERS
	VERTEX_CODE_PARAMETERS
#endif
#ifdef VERTEX_CODE_SAMPLERS
	VERTEX_CODE_SAMPLERS
#endif
#ifdef VERTEX_CODE_SHADER_SCRIPTS
	VERTEX_CODE_SHADER_SCRIPTS
#endif

//TAA jittering test
//uniform vec4 jitteringTest;

void main()
{
	vec3 positionLocal = a_position;
	vec3 normalLocal = a_normal;
	vec4 tangentLocal = a_tangent;
#ifdef GLOBAL_SKELETAL_ANIMATION
	getAnimationData(u_renderOperationData[0], s_bones, a_indices, a_weight, positionLocal, normalLocal, tangentLocal);
#endif
	
	mat4 worldMatrix;
	vec3 previousFramePositionChange;
	uint cullingByCameraDirectionData = uint(0);
	BRANCH
	if(u_renderOperationData[0].y < 0.0)
	{
		//instancing
		worldMatrix = mtxFromRows(i_data0, i_data1, i_data2, vec4(0,0,0,1));
		addTranslate(worldMatrix, u_renderOperationData[7].xyz);
		previousFramePositionChange = i_data3.xyz;

		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.xy = i_data4.xy;
		uint data2 = asuint(i_data4.z);
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.z = float((data2 & uint(0x000000ff)) >> 0) / 255.0;
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.w = float((data2 & uint(0x0000ff00)) >> 8) / 255.0;
		uint colorExp = ( data2 & uint( 0x00ff0000 ) ) >> 16;
		//v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor = i_data4;

		v_colorParameter = decodePackedInstanceColor( i_data3.w, colorExp );
		
		if(v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y < 0.0)
			v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y = u_renderOperationData[1].y;
		
		cullingByCameraDirectionData = asuint( i_data4.w );
	}
	else
	{
		worldMatrix = u_model[0];
		vec4 renderOperationData1 = u_renderOperationData[1];
		vec4 renderOperationData2 = u_renderOperationData[2];
		previousFramePositionChange = renderOperationData2.xyz;
		v_colorParameter = u_renderOperationData[4];
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor = vec4(renderOperationData2.w, renderOperationData1.y, renderOperationData1.x, renderOperationData1.z);
		cullingByCameraDirectionData = asuint( u_renderOperationData[3].w );
	}
	
#ifdef BILLBOARD
	vec4 billboardRotation;
	billboardRotateWorldMatrix(u_renderOperationData, worldMatrix, false, vec3_splat(0), billboardRotation);
#endif
	vec4 worldPosition = mul(worldMatrix, vec4(positionLocal, 1.0));

	vec2 texCoord0 = a_texcoord0;
	vec2 texCoord1 = a_texcoord1;
	vec2 texCoord2 = a_texcoord2;
	vec2 unwrappedUV = getUnwrappedUV(texCoord0, texCoord1, texCoord2, u_renderOperationData[3].x);
	vec4 color0 = (u_renderOperationData[3].y > 0.0) ? a_color0 : vec4_splat(1);
	
	vec3 positionOffset = vec3(0,0,0);
	vec4 customParameter1 = u_materialCustomParameters[0];
	vec4 customParameter2 = u_materialCustomParameters[1];
	vec4 instanceParameter1 = u_objectInstanceParameters[0];
	vec4 instanceParameter2 = u_objectInstanceParameters[1];
	vec3 cameraPosition = u_viewportOwnerCameraPosition;
#ifdef VERTEX_CODE_BODY
	#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
	#else
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerVertex, _sampler), _uv, u_removeTextureTiling, u_mipMap)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerVertex, _sampler), _uv, u_mipMap)
	#endif
	{
		VERTEX_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif
	worldPosition.xyz += positionOffset;

	mat3 worldMatrix3 = toMat3(worldMatrix);
	
	////TAA jittering test
	//vec4 clipPosition = mul(u_viewProj, worldPosition);
	//clipPosition.xy += jitteringTest.xy * clipPosition.w;
	//gl_Position = clipPosition;
	gl_Position = mul(u_viewProj, worldPosition);
	
	v_texCoord01.xy = texCoord0;
	v_texCoord01.zw = texCoord1;
	v_texCoord23.xy = texCoord2;
	v_texCoord23.zw = vec2_splat(0);//texCoord3;
	v_worldPosition_depth.xyz = worldPosition.xyz;
	v_worldNormal_materialIndex.xyz = normalize(mul(worldMatrix3, normalLocal));
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
	
	//v_reflectionVector = v_worldPosition_depth.xyz - u_viewportOwnerCameraPosition;
	
	v_color0 = color0;
	
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
	addTranslate(previousWorldMatrix, u_viewportOwnerCameraPositionPreviousFrameChange - previousFramePositionChange);
	vec4 previousPosition = mul(previousWorldMatrix, vec4(positionLocal, 1));
	previousPosition.xyz += positionOffset;
	v_previousPosition = mul(u_viewportOwnerViewProjectionPrevious, vec4(previousPosition.xyz,1));
#endif

	//geometry with voxel data
#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
	v_objectSpacePosition = positionLocal;
	voxelOrVirtualizedDataModeCalculateParametersV(u_renderOperationData, worldMatrix, u_viewportOwnerCameraPosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2 );
#endif

}
