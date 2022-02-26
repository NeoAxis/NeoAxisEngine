// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

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

void billboardRotateWorldMatrix(vec4 renderOperationData[5], inout mat4 worldMatrix, bool shadowCaster, vec3 shadowCasterCameraPosition, out vec4 billboardRotation )
{
	billboardRotation = vec4_splat(0);
	
	BRANCH
	if(renderOperationData[0].z != 0.0)
	{
		//get rotation value and restore matrix
#ifdef GLSL
		float rotationAngle = worldMatrix[0][1];
		worldMatrix[0][1] = 0.0;
#else
		float rotationAngle = worldMatrix[1][0];
		worldMatrix[1][0] = 0.0;
#endif

		//get billboard rotation quaternion and restore matrix
#ifdef GLSL
		billboardRotation = vec4(worldMatrix[0][2], worldMatrix[1][0], worldMatrix[1][2], worldMatrix[2][0]);
		worldMatrix[0][2] = 0.0;
		worldMatrix[1][0] = 0.0;
		worldMatrix[1][2] = 0.0;
		worldMatrix[2][0] = 0.0;
#else
		billboardRotation = vec4(worldMatrix[2][0], worldMatrix[0][1], worldMatrix[2][1], worldMatrix[0][2]);
		worldMatrix[2][0] = 0.0;
		worldMatrix[0][1] = 0.0;
		worldMatrix[2][1] = 0.0;
		worldMatrix[0][2] = 0.0;
#endif
		
		//face to camera
		{
			int mode = int(renderOperationData[0].z);
			
			//!!!!precalculate constant values?
			
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
			
			float billboardDataMode = renderOperationData[1].w;
			BRANCH
			if(billboardDataMode == 0.0)
			{
				//!!!!works only for Billboard, not for mesh billboard
				//apply rotation parameter
				rotationMatrix2 = mul(rotationMatrix2, makeRotationMatrix(-rotationAngle, vec3(0,1,0)));
				
				//rotationMatrix2 = mul(rotationMatrix2, makeRotationMatrix(, vec3(1,0,0)));
			}
			
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
			vec3 worldPosition;
#ifdef GLSL
			worldPosition[0] = worldMatrix[3][0];
			worldPosition[1] = worldMatrix[3][1];
			worldPosition[2] = worldMatrix[3][2];
#else
			worldPosition[0] = worldMatrix[0][3];
			worldPosition[1] = worldMatrix[1][3];
			worldPosition[2] = worldMatrix[2][3];
#endif
			
			vec3 scale = getScaleFromMatrix(toMat3(worldMatrix));
			float scaleFloat = max(max(scale.x, scale.y), scale.z);
			
			vec3 direction = normalize(worldPosition - shadowCasterCameraPosition);
			vec3 offset = direction * renderOperationData[0].w * scaleFloat;
			worldPosition += offset;
			
#ifdef GLSL
			worldMatrix[3][0] = worldPosition[0];
			worldMatrix[3][1] = worldPosition[1];
			worldMatrix[3][2] = worldPosition[2];
#else
			worldMatrix[0][3] = worldPosition[0];
			worldMatrix[1][3] = worldPosition[1];
			worldMatrix[2][3] = worldPosition[2];
#endif
		}
#endif

	}	
}

#ifdef GLOBAL_BILLBOARD_DATA
void billboardDataModeCalculateParameters(float billboardDataMode, vec3 worldObjectPositionBeforeChanges, vec4 billboardRotation, out vec4 billboardDataIndexes, out vec3 billboardDataFactors, out vec4 billboardDataAngles)
{
	billboardDataIndexes = vec4(0.5,0,0,0);
	billboardDataFactors = vec3_splat(0);
	billboardDataAngles = vec4(0,0,0,0);
	
	BRANCH
	if(billboardDataMode >= 2.0)
	{
		//5 images, 26 images
		
		vec4 billboardRotationInverse = quatInverse(billboardRotation);
		
		vec3 direction = worldObjectPositionBeforeChanges - u_viewportOwnerCameraPosition;
		direction = normalize(mulQuat(billboardRotationInverse, direction));

			
#ifdef GLSL
		const vec3 cameraPositions[26] = vec3[26] ( vec3(-1, -1, -1), vec3(0, -1, -1), vec3(1, -1, -1), vec3(-1, 0, -1), vec3(0, 0, -1), vec3(1, 0, -1), vec3(-1, 1, -1), vec3(0, 1, -1), vec3(1, 1, -1), vec3(-1, -1, 0), vec3(0, -1, 0), vec3(1, -1, 0), vec3(-1, 0, 0), vec3(1, 0, 0), vec3(-1, 1, 0), vec3(0, 1, 0), vec3(1, 1, 0), vec3(-1, -1, 1), vec3(0, -1, 1), vec3(1, -1, 1), vec3(-1, 0, 1), vec3(0, 0, 1), vec3(1, 0, 1), vec3(-1, 1, 1), vec3(0, 1, 1), vec3(1, 1, 1) );
#else
		const vec3 cameraPositions[26] = { vec3(-1, -1, -1), vec3(0, -1, -1), vec3(1, -1, -1), vec3(-1, 0, -1), vec3(0, 0, -1), vec3(1, 0, -1), vec3(-1, 1, -1), vec3(0, 1, -1), vec3(1, 1, -1), vec3(-1, -1, 0), vec3(0, -1, 0), vec3(1, -1, 0), vec3(-1, 0, 0), vec3(1, 0, 0), vec3(-1, 1, 0), vec3(0, 1, 0), vec3(1, 1, 0), vec3(-1, -1, 1), vec3(0, -1, 1), vec3(1, -1, 1), vec3(-1, 0, 1), vec3(0, 0, 1), vec3(1, 0, 1), vec3(-1, 1, 1), vec3(0, 1, 1), vec3(1, 1, 1) };
#endif

		float angles[26];
		{
			LOOP
			for(int n=0;n<26;n++)
			{
				vec3 cameraDirection = -normalize(cameraPositions[n]);
				
				float _cos = dot( cameraDirection, direction );
				_cos = clamp(_cos, -1.0, 1.0);
				float angle = acos(_cos);
				
				angles[n] = angle;
			}
		}

		
		ivec4 minIndexes = ivec4(0,0,0,0);
		vec4 minAngles = vec4_splat(0);
		
		{
			UNROLL
			for(int c=0;c<4;c++)
			{
				int minIndex = 0;
				float minAngle = 10000.0;
				
				LOOP
				for(int n=0;n<26;n++)
				{
					float angle = angles[n];
					if(angle < minAngle)
					{
						minAngle = angle;
						minIndex = n;
					}
				}
				
				minIndexes[c] = minIndex;
				minAngles[c] = minAngle;
				
				//disable item in the array
				angles[minIndex] = 10000.0;
			}
		}

		//calculate factors
		vec4 minFactors;
		{
			minFactors = saturate((PI / 6.0 - minAngles) / (PI / 6.0)); //saturate((PI / 4.0 - minAngles[c]) / (PI / 4.0));
			//UNROLL
			//for(int c=0;c<4;c++)
			//	minFactors[c] = saturate((PI / 6.0 - minAngles[c]) / (PI / 6.0)); //saturate((PI / 4.0 - minAngles[c]) / (PI / 4.0));
		}
		
		//normalize factors
		{
			float q = minFactors.x + minFactors.y + minFactors.z + minFactors.w;
			minFactors /= q;
		}
		
		ivec4 minIndexesOriginal = minIndexes;
		
		//redirect 26 images to 5 images
		BRANCH
		if(billboardDataMode == 2.0)
		{
			#ifdef GLSL
				const int redirects[26] = int[26] ( 1, 1, 1, 1, 0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 4, 3, 3, 3, 3 );
			#else
				const int redirects[26] = { 1, 1, 1, 1, 0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 4, 3, 3, 3, 3 };
			#endif

			minIndexes[0] = redirects[minIndexes[0]];
			minIndexes[1] = redirects[minIndexes[1]];
			minIndexes[2] = redirects[minIndexes[2]];
			minIndexes[3] = redirects[minIndexes[3]];
			//UNROLL
			//for(int c=0;c<4;c++)
			//{
			//	int index = minIndexes[c];
			//	minIndexes[c] = redirects[index];
			//}
		}
		
		billboardDataIndexes = vec4(minIndexes) + vec4_splat(0.5);
		billboardDataFactors = minFactors.xyz;

		
		vec3 objectSpaceCameraUp = normalize(mulQuat(billboardRotationInverse, u_viewportOwnerCameraUp));
		
		UNROLL
		for(int c=0;c<4;c++)
		{
			int minIndex = minIndexesOriginal[c];
			vec3 cameraPosition = normalize(cameraPositions[minIndex]);
			vec3 cameraDirection = -cameraPosition;
			
			vec3 cameraUp = vec3(0,0,1);
			if( cameraPosition.z > 0.99 )
				cameraUp = vec3(1,0,0);
			if( cameraPosition.z < -0.99 )
				cameraUp = vec3(-1,0,0);
			
			vec3 directionSpaceCameraUp = normalize(mul(transpose(lookAt(cameraDirection, cameraUp)), objectSpaceCameraUp));

			billboardDataAngles[c] = PI/2.0 - atan2(directionSpaceCameraUp.z, directionSpaceCameraUp.y);
		}
		
	}
}
#endif
