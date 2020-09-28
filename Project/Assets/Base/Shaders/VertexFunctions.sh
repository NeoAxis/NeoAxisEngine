// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

mat3 makeRotationMatrix(float angle, vec3 axis)
{
	float c, s;
	sincos(angle, s, c);

	float t = 1.0 - c;
	float x = axis.x;
	float y = axis.y;
	float z = axis.z;

	return mtxFromRows(
		t * x * x + c, t * x * y - s * z, t * x * z + s * y,
		t * x * y + s * z, t * y * y + c, t * y * z - s * x,
		t * x * z - s * y, t * y * z + s * x, t * z * z + c
	);
}

vec3 getScaleFromMatrix(mat3 m)
{
	float sx = length(vec3(m[0][0], m[0][1], m[0][2]));
	float sy = length(vec3(m[1][0], m[1][1], m[1][2]));
	float sz = length(vec3(m[2][0], m[2][1], m[2][2]));
	return vec3(sx, sy, sz);
}

void billboardRotateWorldMatrix(vec4 renderOperationData, inout mat4 worldMatrix, bool shadowCaster, vec3 shadowCasterCameraPosition)
{
	BRANCH
	if(renderOperationData.z != 0.0)
	{
		//get rotation value and restore matrix
		float rotation = worldMatrix[1][0];
		worldMatrix[1][0] = 0.0;
		
		//face to camera
		{
			int mode = int(renderOperationData.z);
			
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

			//apply rotation parameter
			rotationMatrix2 = mul(rotationMatrix2, makeRotationMatrix(-rotation, vec3(0,1,0)));
			
			mat3 rotationMatrix = mul(transpose(toMat3(u_view)), rotationMatrix2);
			
			mat3 m = mul(rotationMatrix, toMat3(worldMatrix));
			for(int y=0;y<3;y++)
				for(int x=0;x<3;x++)
					worldMatrix[x][y] = m[x][y];
		}
			
		//add offset to shadow caster
		BRANCH
		if(shadowCaster)
		{
			vec3 worldPosition;
			for(int n=0;n<3;n++)
				worldPosition[n] = worldMatrix[n][3];
			
			vec3 scale = getScaleFromMatrix(toMat3(worldMatrix));
			float scaleFloat = max(max(scale.x, scale.y), scale.z);
			
			vec3 direction = normalize(worldPosition - shadowCasterCameraPosition);
			vec3 offset = direction * renderOperationData.w * scaleFloat;
			worldPosition += offset;
			
			for(int n2=0;n2<3;n2++)
				worldMatrix[n2][3] = worldPosition[n2];
		}
	}	
}
