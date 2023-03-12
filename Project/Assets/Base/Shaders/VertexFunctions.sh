// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#ifdef GLOBAL_SKELETAL_ANIMATION

mat4 getBoneTransform(sampler2D bones, int index)
{
	vec4 item0 = texelFetch(bones, ivec2(0, index), 0);
	vec4 item1 = texelFetch(bones, ivec2(1, index), 0);
	vec4 item2 = texelFetch(bones, ivec2(2, index), 0);
	vec4 item3 = texelFetch(bones, ivec2(3, index), 0);
	return mtxFromCols(item0, item1, item2, item3);
}

void getAnimationData(vec4 renderOperationData, sampler2D bones, uvec4 indices, vec4 weight, inout vec3 position, inout vec3 normal, inout vec4 tangent)
{
	BRANCH
	if(renderOperationData.y > 0.0)
	{
		mat4 transform = 
			getBoneTransform(bones, int(indices.x)) * weight.x +
			getBoneTransform(bones, int(indices.y)) * weight.y +
			getBoneTransform(bones, int(indices.z)) * weight.z +
			getBoneTransform(bones, int(indices.w)) * weight.w;
		mat3 transform3 = toMat3(transform);
		position = (mul(transform, vec4(position, 1.0))).xyz;
		normal = normalize(mul(transform3, normal));
		tangent.xyz = normalize(mul(transform3, tangent.xyz));
	}
}

#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec3 getScaleFromMatrix(mat3 m)
{
#ifdef GLSL
	float sx = length(vec3(m[0][0], m[1][0], m[2][0]));
	float sy = length(vec3(m[0][1], m[1][1], m[2][1]));
	float sz = length(vec3(m[0][2], m[1][2], m[2][2]));
#else
	float sx = length(vec3(m[0][0], m[0][1], m[0][2]));
	float sy = length(vec3(m[1][0], m[1][1], m[1][2]));
	float sz = length(vec3(m[2][0], m[2][1], m[2][2]));
#endif
	return vec3(sx, sy, sz);
}

vec4 mat3ToQuat(mat3 m)
{
#ifdef GLSL
	m = transpose(m);
#endif

	float result[4];

	float s;
	float t;

	float trace = m[0].x + m[1].y + m[2].z;
	
	if( trace > 0.0 )
	{
		t = trace + 1.0;
		s = 1.0 / sqrt( t ) * 0.5;

		result[0] = ( m[2].y - m[1].z ) * s;
		result[1] = ( m[0].z - m[2].x ) * s;
		result[2] = ( m[1].x - m[0].y ) * s;
		result[3] = s * t;
	}
	else
	{
		int i = 0;
		if( m[1].y > m[0].x )
			i = 1;
		if( m[2].z > m[ i ][ i ] )
			i = 2;

		int j = i + 1;
		if( j > 2 )
			j = 0;
		int k = j + 1;
		if( k > 2 )
			k = 0;

		t = ( m[ i ][ i ] - ( m[ j ][ j ] + m[ k ][ k ] ) ) + 1.0;
		s = 1.0 / sqrt( t ) * 0.5;

		result[0] = 0.0;
		result[1] = 0.0;
		result[2] = 0.0;
		result[3] = 1.0;

		result[ i ] = s * t;
		result[ 3 ] = ( m[ k ][ j ] - m[ j ][ k ] ) * s;
		result[ j ] = ( m[ j ][ i ] + m[ i ][ j ] ) * s;
		result[ k ] = ( m[ k ][ i ] + m[ i ][ k ] ) * s;		
	}

	return vec4(result[0], result[1], result[2], result[3]);
}

mat3 lookAt( vec3 direction, vec3 up )
{
	vec3 x = normalize( direction );

	vec3 _cross = cross( up, x );
	//if( cross.Equals( Vector3.Zero, MathEx.Epsilon ) )
	//	return Identity;

	vec3 y = normalize( _cross );

	vec3 z = -normalize(cross( y, x ));
	return transpose(mat3( x, y, z ));
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void billboardRotateWorldMatrix(vec4 renderOperationData[7], inout mat4 worldMatrix, bool shadowCaster, vec3 shadowCasterCameraPosition, out vec4 billboardRotation )
{
	billboardRotation = vec4_splat(0);
	
	BRANCH
	if(renderOperationData[0].z != 0.0)
	{
		//get rotation value and restore matrix
		float rotationAngle = getValue(worldMatrix, 1, 0);
		setValue(worldMatrix, 1, 0, 0.0);

		//get billboard rotation quaternion and restore matrix
		billboardRotation = vec4(getValue(worldMatrix, 2, 0), getValue(worldMatrix, 0, 1), getValue(worldMatrix, 2, 1), getValue(worldMatrix, 0, 2));
		setValue(worldMatrix, 2, 0, 0.0);
		setValue(worldMatrix, 0, 1, 0.0);
		setValue(worldMatrix, 2, 1, 0.0);
		setValue(worldMatrix, 0, 2, 0.0);
		
		//face to camera
		{
			int mode = int(renderOperationData[0].z);
			
			mat3 rotationMatrix2;
			switch(mode)
			{
			case 1:
				rotationMatrix2 = makeRotationMatrix(-PI/2.0, vec3(1,0,0));
				break;
			case 2:
				rotationMatrix2 = mul(makeRotationMatrix(PI, vec3(0,0,1)), makeRotationMatrix(PI/2.0, vec3(1,0,0)));
				break;
			case 3:
				rotationMatrix2 = mul(makeRotationMatrix(-PI/2.0, vec3(0,0,1)), makeRotationMatrix(-PI/2.0, vec3(0,1,0)));
				break;
			case 4:
			default:
				rotationMatrix2 = mul(makeRotationMatrix(PI/2.0, vec3(0,0,1)), makeRotationMatrix(PI/2.0, vec3(0,1,0)));
				break;				
			}

			//!!!!
			//float voxelDataMode = renderOperationData[1].w;
			//BRANCH
			//if(voxelDataMode == 0.0)
			//{
				////!!!!works only for Billboard, not for mesh billboard
				
			//apply rotation parameter
			rotationMatrix2 = mul(rotationMatrix2, makeRotationMatrix(-rotationAngle, vec3(0,1,0)));
				
				//rotationMatrix2 = mul(rotationMatrix2, makeRotationMatrix(, vec3(1,0,0)));
			//}
			
			mat3 rotationMatrix = mul(transpose(toMat3(u_view)), rotationMatrix2);
			
			mat3 m = mul(rotationMatrix, toMat3(worldMatrix));
			worldMatrix[0][0] = m[0][0];
			worldMatrix[0][1] = m[0][1];
			worldMatrix[0][2] = m[0][2];
			worldMatrix[1][0] = m[1][0];
			worldMatrix[1][1] = m[1][1];
			worldMatrix[1][2] = m[1][2];
			worldMatrix[2][0] = m[2][0];
			worldMatrix[2][1] = m[2][1];
			worldMatrix[2][2] = m[2][2];
		}
			
#ifdef SHADOW_CASTER
		//add offset to shadow caster
		BRANCH
		if(shadowCaster && renderOperationData[0].w != 0.0)
		{
			vec3 worldPosition = getTranslate(worldMatrix);
			
			vec3 scale = getScaleFromMatrix(toMat3(worldMatrix));
			float scaleFloat = max(max(scale.x, scale.y), scale.z);
			
			vec3 direction = normalize(worldPosition - shadowCasterCameraPosition);
			vec3 offset = direction * renderOperationData[0].w * scaleFloat;
			worldPosition += offset;

			setTranslate(worldMatrix, worldPosition);
		}
#endif

	}	
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef GLOBAL_VOXEL_LOD

void voxelOrVirtualizedDataModeCalculateParametersV(vec4 renderOperationData[7], mat4 worldMatrix, vec3 cameraPosition, inout vec3 cameraPositionObjectSpace, inout vec4 worldMatrix0, inout vec4 worldMatrix1, inout vec4 worldMatrix2 )//, inout vec4 worldMatrixRotation, inout vec3 worldMatrixScale)
{
	float voxelDataMode = renderOperationData[1].w;
	//float virtualizedDataMode = renderOperationData[3].w;
	
	BRANCH
	if( voxelDataMode != 0.0 )//|| virtualizedDataMode != 0.0)
	{
		mat4 worldMatrixInv = matInverse(worldMatrix);
		cameraPositionObjectSpace = mul(worldMatrixInv, vec4(cameraPosition, 1.0)).xyz;
		
		worldMatrix0 = worldMatrix[ 0 ];
		worldMatrix1 = worldMatrix[ 1 ];
		worldMatrix2 = worldMatrix[ 2 ];
		
		//mat3 worldMatrix3 = toMat3(worldMatrix);
		//worldMatrixRotation = mat3ToQuat(worldMatrix3);
		//worldMatrixScale = getScaleFromMatrix(worldMatrix3);
		
		//mat3 m = toMat3(worldMatrix);
		//mat3 mm = transpose(m);
		//voxelCameraPositionObjectSpace = mul(mm, cameraPosition - getTranslate(worldMatrix));		
		//voxelCameraPositionObjectSpace = mul(worldMatrixBeforeChangesInv, vec4(cameraPosition, 1.0)).xyz;
	}
}

#endif
