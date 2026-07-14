$input a_position, a_normal, a_tangent, a_texcoord0, a_texcoord1, a_texcoord2, a_color0, a_color3, a_indices, a_weight, i_data0, i_data1, i_data2, i_data3, i_data4
$output v_texCoord01, v_worldPosition_depth, v_worldNormal_materialIndex, v_tangent, v_fogFactor, v_color0, v_eyeTangentSpace, v_normalTangentSpace, v_position, v_previousPosition, v_texCoord23, v_colorParameter, v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor, v_objectSpacePosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2

// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#define FORWARD 1
#include "Common.sh"
#include "VertexFunctions.sh"

#ifdef GLOBAL_SKELETAL_ANIMATION
	SAMPLER2D(s_bones_0, 0);
#endif

SAMPLER2D(s_drawBufferTexture_5, 5);
#define s_drawBufferTexture s_drawBufferTexture_5
#include "DrawBuffer.sh"

#ifndef LIMITED_DEVICE
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
	MEDIUMP vec3 normalLocal = a_normal;
	MEDIUMP vec4 tangentLocal = a_tangent;
#ifdef GLOBAL_SKELETAL_ANIMATION
	getAnimationData(d_renderOperationData0, s_bones_0, a_indices, a_weight, positionLocal, normalLocal, tangentLocal);
#endif

	mat4 worldMatrix;
	vec3 previousFramePositionChange;
	uint cullingByCameraDirectionData = uint(0);
	BRANCH
	if(d_renderOperationData0.y < 0.0)
	{
		//instancing
		worldMatrix = mtxFromRows(i_data0, i_data1, i_data2, vec4(0,0,0,1));
		addTranslate(worldMatrix, d_renderOperationData7.xyz);
		previousFramePositionChange = i_data3.xyz;
		
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.xy = i_data4.xy;
		uint data2 = asuint(i_data4.z);
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.z = float((data2 & uint(0x000000ff)) >> 0) / 255.0;
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.w = float((data2 & uint(0x0000ff00)) >> 8) / 255.0;
		uint colorExp = ( data2 & uint( 0x00ff0000 ) ) >> 16;
		//v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor = i_data4;
		
		v_colorParameter = decodePackedInstanceColor( i_data3.w, colorExp );
		
		if(v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y < 0.0)
			v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor.y = d_renderOperationData1.y;
		
		cullingByCameraDirectionData = asuint( i_data4.w );
	}
	else
	{
		worldMatrix = u_model[0];
		vec4 renderOperationData1 = d_renderOperationData1;
		vec4 renderOperationData2 = d_renderOperationData2;
		previousFramePositionChange = renderOperationData2.xyz;
		v_colorParameter = d_renderOperationData4;
		v_lodValue_visibilityDistance_receiveDecals_motionBlurFactor = vec4(renderOperationData2.w, renderOperationData1.y, renderOperationData1.x, renderOperationData1.z);
		cullingByCameraDirectionData = asuint( d_renderOperationData3.w );
	}
	
	MEDIUMP vec4 billboardRotation;
	billboardRotateWorldMatrix(d_renderOperationData0, worldMatrix, false, vec3_splat(0), billboardRotation);
	vec4 worldPosition = mul(worldMatrix, vec4(positionLocal, 1.0));

	vec2 texCoord0 = a_texcoord0;
	vec2 texCoord1 = a_texcoord1;
	vec2 texCoord2 = a_texcoord2;
	//vec2 texCoord3 = a_texcoord3;
	vec2 unwrappedUV = getUnwrappedUV(texCoord0, texCoord1, texCoord2/*, texCoord3*/, d_renderOperationData3.x);
	MEDIUMP vec4 color0 = (d_renderOperationData3.y > 0.0) ? a_color0 : vec4_splat(1);
	vec3 positionOffset = vec3(0,0,0);
	vec4 customParameter1 = d_materialCustomParameters0;
	vec4 customParameter2 = d_materialCustomParameters1;
	vec4 instanceParameter1 = d_objectInstanceParameters0;
	vec4 instanceParameter2 = d_objectInstanceParameters1;
	vec3 cameraPosition = u_viewportOwnerCameraPosition;
	
#ifdef VERTEX_CODE_BODY
	#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerVertex, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerVertex, _sampler), 0 ).x ), 0.5 ) * 0.1)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerVertex, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerVertex, _sampler), 0 ).x ), 0.5 ) * 0.1)
	#else
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DRemoveTiling(makeSampler(s_linearSamplerVertex, _sampler), _uv, u_removeTextureTiling, u_mipBias)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerVertex, _sampler), _uv, u_mipBias)
	#endif
	{
		VERTEX_CODE_BODY
	}
	#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
	#undef CODE_BODY_TEXTURE2D
#endif
	worldPosition.xyz += positionOffset;

	MEDIUMP mat3 worldMatrix3 = toMat3(worldMatrix);
	
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

#ifndef LIMITED_DEVICE
	BRANCH
	if( cullingByCameraDirectionData != 0 )
	{
		vec4 data;
		data.x = float((cullingByCameraDirectionData & uint(0x000000ff)) >> 0);
		data.y = float((cullingByCameraDirectionData & uint(0x0000ff00)) >> 8);
		data.z = float((cullingByCameraDirectionData & uint(0x00ff0000)) >> 16);
		data.w = float((cullingByCameraDirectionData & uint(0xff000000)) >> 24);
		data = data / 255.0;
		
		MEDIUMP vec3 cullingNormal = normalize( expand( data.xyz ) );
		MEDIUMP vec3 dir = normalize( u_viewportOwnerCameraPosition - worldPosition.xyz );
		MEDIUMP float _cos = dot( dir, cullingNormal );
		MEDIUMP float _acos = acos( clamp( _cos, -1.0, 1.0 ) );
		if( _acos > PI / 2.0 + data.w * PI / 2.0 )
			gl_Position.x = 0.0 / 0.0;
	}
#endif
	
	//fog
#ifdef GLOBAL_FOG
	#if defined(BLEND_MODE_TRANSPARENT) || defined(BLEND_MODE_ADD)
		v_fogFactor = getFogFactor(u_viewportOwnerCameraPosition, v_worldPosition_depth.xyz, /*1.0,*/ false);
	#endif
#endif

	v_color0 = color0;

	//displacement
#ifdef DISPLACEMENT
	{
		vec3 eyeWorldSpace = worldPosition.xyz - u_viewportOwnerCameraPosition;

		MEDIUMP vec3 normalizedNormal = normalize(normalLocal);
		MEDIUMP vec3 normalizedTangent = normalize(tangentLocal.xyz);
		MEDIUMP vec3 binormal = normalize(cross( normalizedNormal, normalizedTangent ) * tangentLocal.w);
	
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
	addTranslate(previousWorldMatrix, u_viewportOwnerCameraPositionPreviousFrameChange - previousFramePositionChange);
	vec4 previousPosition = mul(previousWorldMatrix, vec4(positionLocal, 1));
	previousPosition.xyz += positionOffset;
	v_previousPosition = mul(u_viewportOwnerViewProjectionPrevious, vec4(previousPosition.xyz,1));
#endif

	//geometry with voxel data
#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )
	v_objectSpacePosition = positionLocal;
	voxelOrVirtualizedDataModeCalculateParametersV(d_renderOperationData1, worldMatrix, u_viewportOwnerCameraPosition, v_cameraPositionObjectSpace, v_worldMatrix0, v_worldMatrix1, v_worldMatrix2);
#endif
}