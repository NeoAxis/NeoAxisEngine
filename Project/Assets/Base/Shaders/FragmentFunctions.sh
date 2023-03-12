// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

//#if defined( GLOBAL_VOXEL_LOD )
//	#inc_lude "Vox_elCommon.sh"
//#endif

#ifdef GLSL
void clip(float v)
{
	if(v < 0.0)
		discard;
}
#endif

void smoothLOD(vec4 fragCoord, float lodValue)
{
	BRANCH
	if(lodValue != 0.0)
	{
		uint x = uint(fragCoord.x);
		uint y = uint(fragCoord.y);
		uint xx = x % uint(4);
		uint yy = y % uint(4);
		uint v = xx * uint(4) + yy;
#ifdef GLSL
		const float thresholds[16] = float[16](0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625);
#else
		const float thresholds[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
#endif
		float threshold = thresholds[v];
		if(lodValue > 0.0)
			clip(threshold - lodValue);
		else
			clip(-threshold - lodValue);
	}
}

float dither(vec4 fragCoord, float value)
{
	BRANCH
	if(value > 0.02 && value < 0.98)
	{
		uint x = uint(fragCoord.x);
		uint y = uint(fragCoord.y);
		uint xx = x % uint(4);
		uint yy = y % uint(4);
		uint v = xx * uint(4) + yy;
#ifdef GLSL
		const float array[16] = float[16](0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625);
#else
		const float array[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
#endif
		float vv = array[v];
		value -= vv * 1.0 - 0.5;
		//value -= vv * 0.5 - 0.25;
		//value -= vv * 0.25 - 0.125;
		value = saturate(value);
	}	
	return value;
}

bool getDitherBoolean(vec4 fragCoord, float value)
{
	uint x = uint(fragCoord.x);
	uint y = uint(fragCoord.y);
	uint xx = x % uint(4);
	uint yy = y % uint(4);
	uint v = xx * uint(4) + yy;
#ifdef GLSL
	const float thresholds[16] = float[16](0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625);
#else
	const float thresholds[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
#endif
	float threshold = thresholds[v];
	return threshold - value < 0.0;
}

int ditherArray4(vec4 fragCoord, vec3 factors)
{
	uint x = uint(fragCoord.x);
	uint y = uint(fragCoord.y);
	uint xx = x % uint(4);
	uint yy = y % uint(4);
	uint v = xx * uint(4) + yy;
#ifdef GLSL
	const float thresholds[16] = float[16](0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625);
#else
	const float thresholds[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
#endif
	float threshold = thresholds[v];
	
	if(threshold < factors[0])
		return 0;
	threshold -= factors[0];
	if(threshold < factors[1])
		return 1;
	threshold -= factors[1];
	if(threshold < factors[2])
		return 2;
	return 3;
}

vec2 vogelDiskSample(int sampleIndex, int sampleCount, float angle)
{
	const float goldenAngle = 2.399963f;
	float r = sqrt(float(sampleIndex) + 0.5f) / sqrt(float(sampleCount));
	float theta = float(sampleIndex) * goldenAngle + angle;
	float sine, cosine;
	sincos(theta, sine, cosine);
	return vec2(cosine, sine) * r;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//lightAttenuation: x = near; y = far; z = power; w = far - near.
float getLightAttenuation(vec4 lightAttenuation, float distance)
{
	return saturate(pow(1.0 - min( (distance - lightAttenuation.x) / lightAttenuation.w, 1.0), lightAttenuation.z));
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

float toClipSpaceDepth(float depthTextureValue)
{
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	return depthTextureValue;
#else
	return depthTextureValue * 2.0 - 1.0;
#endif
}

float getDepthFromClipSpaceDepth(float depthTextureValue)
{
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	return depthTextureValue;
#else
	return (depthTextureValue + 1.0) * 0.5;
#endif
}

float getDepthValue(float depthTextureValue, float near, float far)
{
	float clipDepth = toClipSpaceDepth(depthTextureValue);
	
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	return near * far / (far - clipDepth * (far - near));
#else
	return near * far / (far + near - clipDepth * (far - near));
	//return 2.0 * near * far / (far + near - clipDepth * (far - near));
#endif
}

float getRawDepthValue(float depth, float near, float far)
{
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	float clipDepth = (far - (near * far) / depth) / (far - near);
#else
	float clipDepth = (far + near - (near * far) / depth) / (far - near);
#endif
	
	float rawDepth = getDepthFromClipSpaceDepth(clipDepth);
	return rawDepth;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec3 reconstructWorldPosition(mat4 invViewProj, vec2 texCoord, float rawDepth)
{
	vec3 clip = vec3(texCoord * 2.0 - 1.0, toClipSpaceDepth(rawDepth));
	#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
		clip.y = -clip.y;
	#endif
	vec4 wpos = mul(invViewProj/*u_invViewProj*/, vec4(clip, 1.0));
	return wpos.xyz / wpos.w;
}

vec2 projectToScreenCoordinates(mat4 viewProj, vec3 worldPosition)
{
	// get homogeneous clip space coordinates
	vec4 clipSpace = mul(viewProj, vec4( worldPosition, 1.0 ));
	
	// apply perspective divide to get normalized device coordinates
	vec3 ndc = clipSpace.xyz / clipSpace.w;
	
	// do viewport transform
	vec2 screenSpace = (ndc.xy * 0.5 + vec2_splat( 0.5 ));
	screenSpace.y = 1.0 - screenSpace.y;
	
	return screenSpace;
	
	
	/*
	vec3 eyeSpacePos = mul(viewMatrix, vec4(worldPosition, 1.0)).xyz;
	
	//!!!!
	//if( backsideReturnFalse )

	mat4 m = projMatrix;
	vec3 v = eyeSpacePos;	
	
	//!!!!	
	float invW = 1.0 / ( m[3][0] * v.x + m[3][1] * v.y + m[3][2] * v.z + m[3][3] );
	//float invW = 1.0 / ( m[0][3] * v.x + m[1][3] * v.y + m[2][3] * v.z + m[3][3] );
	//float invW = 1.0 / ( m.Item0.W * v.X + m.Item1.W * v.Y + m.Item2.W * v.Z + m.Item3.W );
	vec2 v2 = mul(m, vec4(v, 1) ) * invW;

	vec2 screenPosition = vec2( ( v2.x + 1.0 ) * 0.5, 1.0 - ( v2.y + 1.0 ) * 0.5 );	

	return screenPosition;
	*/	
	
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*
float encodeBitsToByte(bool v1, bool v2)
{
	float v = v1 ? 0.75 : 0.25;
	if(!v2)
		v = -v;
	return v * 0.5 + 0.5;
}

void decodeBitsFromByte(float source, out bool v1, out bool v2)
{
	float v = source * 2.0 - 1.0;
	v1 = abs(v) > 0.5;
	v2 = v > 0.0;
}
*/

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*
vec4 encodeGBuffer4(vec4 tangent, bool shadingModelSimple, bool receiveDecals)
{
	vec4 result;
	
	result.xyz = tangent.xyz * 0.5 + 0.5;
	if(shadingModelSimple)
		result.xyz = vec3_splat(0);
	
	result.w = encodeBitsToByte(tangent.w > 0.0, receiveDecals);
	
	return result;
}

void decodeGBuffer4(vec4 source, out vec4 tangent, out bool shadingModelSimple, out bool receiveDecals)
{
	bool tangentW;
	decodeBitsFromByte(source.w, tangentW, receiveDecals);
	
	tangent = vec4(normalize(source.xyz * 2.0 - 1.0), tangentW ? 1.0 : -1.0);

	shadingModelSimple = !any2(source.xyz);
}
*/

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct EnvironmentTextureData
{
	vec4 rotation;//mat3 rotation;
	vec4 multiplierAndAffect;
};

vec3 irradianceSphericalHarmonics(vec4 irradiance[9], const vec3 n)
{
    return max(
          irradiance[0]
        + irradiance[1] * (n.y)
        + irradiance[2] * (n.z)
        + irradiance[3] * (n.x)
#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_QUALITY
		+ irradiance[4] * (n.y * n.x)
        + irradiance[5] * (n.y * n.z)
        + irradiance[6] * (3.0 * n.z * n.z - 1.0)
        + irradiance[7] * (n.z * n.x)
        + irradiance[8] * (n.x * n.x - n.y * n.y)
#endif
        , vec4_splat(0.0)).xyz;
}

vec3 getEnvironmentValue(vec4 environmentIrradiance[9], EnvironmentTextureData data, vec3 texCoord)
{
	vec3 v = irradianceSphericalHarmonics(environmentIrradiance, flipCubemapCoords2(mulQuat(data.rotation, texCoord))).rgb * data.multiplierAndAffect.xyz;
	return lerp(vec3_splat(1), v, data.multiplierAndAffect.w);
}

/*
vec3 getEnvironmentValue(samplerCube tex, EnvironmentTextureData data, vec3 texCoord)
{
	vec3 v = textureCube(tex, flipCubemapCoords2(mul(data.rotation, texCoord))).rgb * data.multiplierAndAffect.xyz;
	return lerp(vec3_splat(1), v, data.multiplierAndAffect.w);
}
*/

vec3 getEnvironmentValueLod(samplerCube tex, EnvironmentTextureData data, vec3 texCoord, float lod)
{
	vec3 v = textureCubeLod(tex, flipCubemapCoords2(mulQuat(data.rotation, texCoord)), lod).rgb * data.multiplierAndAffect.xyz;
	return lerp(vec3_splat(1), v, data.multiplierAndAffect.w);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool cutVolumes( vec3 worldPosition )
{
#if GLOBAL_CUT_VOLUME_MAX_AMOUNT > 0

	BRANCH
	if(u_viewportCutVolumeSettings.x > 0.0)
	{
		int count = int(u_viewportCutVolumeSettings.x);
		//BRANCH
		LOOP
		for(int n = 0; n < count; n++)
		{
			mat4 m = u_viewportCutVolumeData[n];
			float shape = m[3][3];
			m[3][3] = 1.0;

			vec3 p = abs(mul(m, vec4(worldPosition, 1.0)).xyz);
			bool invert = shape < 0.0;
			float shapeAbs = abs(shape);
			
			bool clip = false;
			
			if(shapeAbs == 1.0)
			{
				//Box
				clip = p.x < 0.5 && p.y < 0.5 && p.z < 0.5;
			}
			else if(shapeAbs == 2.0)
			{
				//Sphere
				clip = length(p) < 0.5;
			}
			else// if(shapeAbs == 3.0)
			{
				//Cylinder
				clip = p.x < 0.5 && length(p.yz) < 0.5;
			}
/*			else
			{
				//Plane
				vec4 plane = m[0];
				clip = dot(plane, vec4(worldPosition, 1.0));
			}*/
			
			if(invert)
				clip = !clip;
			
			if(clip)
				return true;
		}
	}

#endif

	return false;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef GLSL
void rayAABBIntersect(vec3 ray_origin, vec3 ray_dir, vec3 minpos, vec3 maxpos, out bool intersects, out float intersectScale)
{
	vec3 inverse_dir = 1.0 / ray_dir;
	vec3 tbot = inverse_dir * (minpos - ray_origin);
	vec3 ttop = inverse_dir * (maxpos - ray_origin);
	vec3 tmin = min(ttop, tbot);
	vec3 tmax = max(ttop, tbot);
	vec2 traverse = max(tmin.xx, tmin.yz);
	float traverselow = max(traverse.x, traverse.y);
	traverse = min(tmax.xx, tmax.yz);
	float traversehi = min(traverse.x, traverse.y);

	intersects = traversehi > max(traverselow, 0.0);
	intersectScale = traverselow;
	//return vec3(float(traversehi > max(traverselow, 0.0)), traversehi, traverselow);
}
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef GLOBAL_VOXEL_LOD

float getVoxelValue( sampler2D voxelData, uint voxelTextureSize, uint index )
{
	//!!!!
	uint y = index / voxelTextureSize;
	uint x = index % voxelTextureSize;
	return texelFetch( voxelData, uvec2( x, y ), 0 ).x;
}

uint getVoxelBufferIndexOfVoxel( uvec3 gridSize, uvec3 voxelIndex )
{
	return ( voxelIndex.z * gridSize.y + voxelIndex.y ) * gridSize.x + voxelIndex.x;
}

uvec3 getVoxelNearestIndexFromValue256( float value )
{
	uint index = uint(abs(value));
	
	uint z = index / 65536;
	uint index2 = index % 65536;
	uint y = index2 / 256;
	uint x = index2 % 256;
	
	return uvec3(x, y, z);
}

void voxelDataModeCalculateParametersF( float voxelDataMode, sampler2D voxelData, vec4 fragCoord, vec3 voxelObjectSpacePosition, vec3 voxelCameraPositionObjectSpace, vec4 worldMatrix0, vec4 worldMatrix1, vec4 worldMatrix2, vec4 renderOperationData[ 7 ], vec3 fromCameraDirection, inout vec3 inputWorldNormal, inout vec4 tangent, inout vec2 texCoord0, inout vec2 texCoord1, inout vec2 texCoord2, inout vec4 color0, inout float voxelLengthInside, inout int materialIndex )
{
	BRANCH
	if( voxelDataMode != 0.0 )
	{
		//get voxel data info
		vec3 gridSizeF = renderOperationData[ 5 ].xyz;
		float fillHolesDistanceAndFormat = renderOperationData[ 5 ].w;
		float fillHolesDistance = abs( fillHolesDistanceAndFormat );
		bool fullFormat = fillHolesDistanceAndFormat < 0.0;
		//vec2 formatAndFillHolesDistance = unpackHalf2x16(asuint(renderOperationData[5].w));
		//bool fullFormat = formatAndFillHolesDistance.x > 0.5;
		//float fillHolesDistance = formatAndFillHolesDistance.y;
		vec3 boundsMin = renderOperationData[ 6 ].xyz;
		float cellSize = renderOperationData[ 6 ].w;
		ivec3 gridSize = ivec3( gridSizeF );
		vec3 boundsMax = boundsMin + gridSizeF * cellSize;
		
		vec3 localRayOrigin = voxelCameraPositionObjectSpace;
		vec3 localRayDirection = normalize( voxelObjectSpacePosition - voxelCameraPositionObjectSpace );
		
		bool intersects;
		float intersectScale;
		rayAABBIntersect( localRayOrigin, localRayDirection, boundsMin, boundsMax, intersects, intersectScale );
		//rayAABBIntersect(localRayOrigin, localRayDirection, vec3_splat(0), gridSizeF, intersects, intersectScale);
		
		//BRANCH
		//if(!intersects)
		//	discard;

		vec3 currentPosition = localRayOrigin + localRayDirection * intersectScale;
		
		//convert to grid space
		currentPosition -= boundsMin;
		currentPosition /= (boundsMax - boundsMin);	
		/*//!!!!
		//add some blur/antialiasing
#ifndef SHADOW_CASTER
		float rx = random(fragCoord.xy);
		float ry = random(fragCoord.yz);
		float rz = random(fragCoord.zx);
		//!!!! / 1.5?
		currentPosition += (vec3(rx, ry, rz) - vec3_splat(0.5)) / gridSizeF / 1.5;
#endif*/		
		currentPosition = clamp( currentPosition, vec3_splat( 0 ), vec3_splat( 0.9999 ) ); //to have valid grid index
		currentPosition *= gridSizeF;

		ivec3 currentIndex = ivec3( currentPosition );
		ivec3 startIndex = currentIndex;
	
		int voxelTextureSize = textureSize( voxelData, 0 ).x;

		const int maxSteps = GLOBAL_VOXEL_LOD_MAX_STEPS; //12;

		bool found = false;
		float foundVoxelValue = 0.0;
		
		float totalNearestIndexDistance = 10000.0;
		ivec3 totalNearestIndex = ivec3( 0,0,0 );
				
		LOOP
		for( int nIteration = 0; nIteration < maxSteps; nIteration++ )
		{
			//make HLSL compiler happy
			if( nIteration >= maxSteps )
				break;

			float voxelValue = getVoxelValue( voxelData, voxelTextureSize, getVoxelBufferIndexOfVoxel( gridSize, currentIndex ) );
			//uint indexOfVoxel = getVoxelBufferIndexOfVoxel( gridSize, currentIndex );
			//float voxelValue = getVoxelValue( voxelData, voxelTextureSize, indexOfVoxel / 4 )[ indexOfVoxel % 4 ];
			
			BRANCH
			if( voxelValue > 0.0 )
			{
				found = true;
				foundVoxelValue = voxelValue;
				break;
			}
			else
			{
				ivec3 nearestIndex = getVoxelNearestIndexFromValue256( voxelValue );

				//!!!!use center of voxel?
				//!!!!calculate distances between voxel centers? currentIndex?
				float distance = length( vec3( nearestIndex - currentIndex ) );
				//float distance = length(vec3(nearestIndex + vec3(0.5, 0.5, 0.5)) - currentPosition);
				
				//this is not exact distance to ray but ok
				float distanceToRay = distance;
				if( distanceToRay < totalNearestIndexDistance )
				{
					totalNearestIndexDistance = distanceToRay;
					totalNearestIndex = nearestIndex;
				}
				
				//the way to add more precision
				//distance -= 0.5;
				
				currentPosition += localRayDirection * distance;
				currentIndex = ivec3( currentPosition );
				
				if( currentPosition.x < 0.0 || currentIndex.x >= gridSize.x || 
					currentPosition.y < 0.0 || currentIndex.y >= gridSize.y || 
					currentPosition.z < 0.0 || currentIndex.z >= gridSize.z )
					discard;
			}
		}

		//try to use nearest found voxel
		BRANCH
		if( !found && totalNearestIndexDistance < fillHolesDistance ) // < 1.1)
		{
			found = true;
			foundVoxelValue = getVoxelValue( voxelData, voxelTextureSize, getVoxelBufferIndexOfVoxel( gridSize, totalNearestIndex ) );
			//uint indexOfVoxel = getVoxelBufferIndexOfVoxel( gridSize, totalNearestIndex );
			//foundVoxelValue = getVoxelValue( voxelData, voxelTextureSize, indexOfVoxel / 4 )[ indexOfVoxel % 4 ];
			currentIndex = totalNearestIndex;
		}

		BRANCH
		if( !found )
			discard;

		
		int dataStartIndex = int( foundVoxelValue );
		
		vec2 data0 = unpackHalf2x16( asuint( getVoxelValue( voxelData, voxelTextureSize, dataStartIndex + 0 ) ) );
		vec2 data1 = unpackHalf2x16( asuint( getVoxelValue( voxelData, voxelTextureSize, dataStartIndex + 1 ) ) );
		vec2 data2 = unpackHalf2x16( asuint( getVoxelValue( voxelData, voxelTextureSize, dataStartIndex + 2 ) ) );
		vec2 data3 = unpackHalf2x16( asuint( getVoxelValue( voxelData, voxelTextureSize, dataStartIndex + 3 ) ) );

		//vec4 data0123 = asuint( getVoxelValue( voxelData, voxelTextureSize, ( dataStartIndex + 0 ) / 4 ) );
		//vec2 data0 = unpackHalf2x16( data0123.x );
		//vec2 data1 = unpackHalf2x16( data0123.y );
		//vec2 data2 = unpackHalf2x16( data0123.z );
		//vec2 data3 = unpackHalf2x16( data0123.w );
		
		materialIndex = int( data0.x );

		//useful place for integer
		//int(data1.x)
		
		vec2 normalSpherical;
		vec2 data0c = ( ( data0 % 1.0 ) - 0.5 ) * PI;
		normalSpherical.x = data0c.x * 2.0;
		normalSpherical.y = data0c.y;
		//normalSpherical.x = ((data0.x % 1.0) - 0.5) * PI * 2.0;
		//normalSpherical.y = ((data0.y % 1.0) - 0.5) * PI;

		//!!!!translate?
		mat4 worldMatrix = mtxFromRows( worldMatrix0, worldMatrix1, worldMatrix2, vec4(0,0,0,1) );
		mat3 worldMatrix3 = toMat3( worldMatrix );
		
		vec3 normalObjectSpace = sphericalDirectionGetVector( normalSpherical );
		inputWorldNormal = normalize( mul( worldMatrix3, normalObjectSpace ) );
		//inputWorldNormal = mulQuat( worldMatrixRotation, normalObjectSpace );
		
		texCoord0 = data1;

		tangent.xy = data2;
		tangent.zw = data3;
		tangent.xyz = mul( worldMatrix3, tangent.xyz );
		//tangent.xyz = mulQuat( worldMatrixRotation, tangent.xyz );
		
		BRANCH
		if( fullFormat )
		{
			vec2 data4 = unpackHalf2x16( asuint( getVoxelValue( voxelData, voxelTextureSize, dataStartIndex + 4 ) ) );
			vec2 data5 = unpackHalf2x16( asuint( getVoxelValue( voxelData, voxelTextureSize, dataStartIndex + 5 ) ) );
			vec2 data6 = unpackHalf2x16( asuint( getVoxelValue( voxelData, voxelTextureSize, dataStartIndex + 6 ) ) );
			vec2 data7 = unpackHalf2x16( asuint( getVoxelValue( voxelData, voxelTextureSize, dataStartIndex + 7 ) ) );

			//vec4 data4567 = asuint( getVoxelValue( voxelData, voxelTextureSize, ( dataStartIndex + 4 ) / 4 ) );
			//vec2 data4 = unpackHalf2x16( data4567.x );
			//vec2 data5 = unpackHalf2x16( data4567.y );
			//vec2 data6 = unpackHalf2x16( data4567.z );
			//vec2 data7 = unpackHalf2x16( data4567.w );
			
			texCoord1 = data4;
			texCoord2 = data5;
			color0.xy = data6;
			color0.zw = data7;
		}

		//calculate length from mesh bounding box geometry ray intersection to voxel position
		
		//!!!!startIndex? bolee tochnoe znachenie moshno?

		vec3 localVector;
#ifdef SHADOW_CASTER
		//!!!!2.0
		localVector = ( vec3( currentIndex - startIndex ) ) * cellSize * 2.0;
#else
		localVector = ( vec3( currentIndex - startIndex ) ) * cellSize;
#endif	
		vec3 worldVector = mul( worldMatrix3, localVector );
		voxelLengthInside = length( worldVector );

		
		//invert back side normal
		{
			//!!!!esli odnostoronniy to po idee nushno dalshe iskat drugoy voksel
			
			//!!!!slowly. use dot?

			float _cos = dot( -fromCameraDirection, inputWorldNormal );
			float _acos = acos( clamp( _cos, -1.0, 1.0 ) );
			if( _acos > PI / 2.0 )
				inputWorldNormal = -inputWorldNormal;
		}
	}	
}


#endif

/* ideas about mipmapping
	
ddx, ddy
	
	
#ifdef GLSL
		float size = float(textureSize(billboardData, 0).x);
#else
		float size = float(textureArraySize(billboardData, 0).x);
#endif
		
		float lod;
		{
			#ifdef GLSL
				vec2 texCoordTexels = texCoord0 * size;
				vec2 ddx2 = dFdx(texCoordTexels);
				vec2 ddy2 = dFdy(texCoordTexels);
				lod = max(0.5 * log2(max(dot(ddx2, ddx2), dot(ddy2, ddy2))), 0.0);
			#else
				lod = billboardData.m_texture.CalculateLevelOfDetailUnclamped(billboardData.m_sampler, texCoord0.x);
			#endif
			
			//if(lod < 1.0)
			//	discard;			
			//if(lod > 1.0 && lod < 2.0)
			//	discard;
		}
	
		size /= pow(2.0, trunc(lod));
		vec2 texCoord = trunc(texCoord0 * size) / size;
		
		vec4 value = texture2DArray(billboardData, vec3(texCoord, intIndex));
		
		//opacity
		BRANCH
		if(value.x > 50.0)
			discard;
	
*/



/*
#ifdef GLOBAL_VIRTUALIZED_GEOMETRY

vec4 getVirtualizedData( sampler2D virtualizedData, uint virtualizedTextureSize, uint index )
{
	//!!!!use data buffer
	uint y = index / virtualizedTextureSize;
	uint x = index % virtualizedTextureSize;
	return texelFetch( virtualizedData, uvec2( x, y ), 0 );
}

void virtualizedDataModeCalculateParametersF(float virtualizedDataMode, sampler2D virtualizedData, vec4 fragCoord, vec3 objectSpacePosition, vec3 cameraPositionObjectSpace, vec4 worldMatrixRotation, vec3 worldMatrixScale, vec4 renderOperationData[7], uint primitiveID, inout vec3 inputWorldNormal, inout vec4 tangent, inout vec2 texCoord0, inout vec2 texCoord1, inout vec2 texCoord2, inout vec4 color0, inout float depthOffset, inout int materialIndex)
{
	BRANCH
	if( virtualizedDataMode != 0.0 )
	{
		int virtualizedTextureSize = textureSize( virtualizedData, 0 ).x;
		vec3 localRayOrigin = cameraPositionObjectSpace;
		vec3 localRayDirection = normalize( objectSpacePosition - cameraPositionObjectSpace );

		

		inputWorldNormal = mulQuat( worldMatrixRotation, inputWorldNormal );
		tangent.xyz = mulQuat( worldMatrixRotation, tangent.xyz );
		
	}
}

#endif
*/



		/*
		vec3 vFrom = vec3( startIndex ) * cellSize;
		vec3 vTo = vec3( currentIndex ) * cellSize;

//		float objectSpaceLength = length( vec3( currentIndex - startIndex ) ) * cellSize;
#ifdef SHADOW_CASTER
		vec3 direction = vTo - vFrom;
		float length = length( direction );
		direction = normalize( direction );

		//!!!!constant
		length += cellSize * 5.0;
		//objectSpaceLength += cellSize * 5.0;
		
		vTo = vFrom + direction * length;
#endif
	
		vec3 vFrom2 = mul( worldMatrix3, vFrom );
		vec3 vTo2 = mul( worldMatrix3, vTo );
		
		voxelLengthInside = length ( vTo2 - vFrom2 );
		*/
		
		/*
		float objectSpaceLength = length( vec3( currentIndex - startIndex ) ) * cellSize;
#ifdef SHADOW_CASTER
		//!!!!constant
		objectSpaceLength += cellSize * 5.0;
#endif
		voxelLengthInside = objectSpaceLength * worldMatrixScale.x;
		*/
