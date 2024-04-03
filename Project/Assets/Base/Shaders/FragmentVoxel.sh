// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#if defined( GLOBAL_VOXEL_LOD ) && defined( VOXEL )

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
	uint index = uint( abs( value ) );
	
	uint z = index / 65536;
	uint index2 = index % 65536;
	uint y = index2 / 256;
	uint x = index2 % 256;
	
	return uvec3( x, y, z );
}

void voxelDataModeCalculateParametersF( float voxelDataMode, sampler2D voxelData, vec4 fragCoord, vec3 voxelObjectSpacePosition, vec3 voxelCameraPositionObjectSpace, vec4 worldMatrix0, vec4 worldMatrix1, vec4 worldMatrix2, vec4 renderOperationData[8], vec3 fromCameraDirection, inout vec3 inputWorldNormal, inout vec4 tangent, inout vec2 texCoord0, inout vec2 texCoord1, inout vec2 texCoord2, inout vec4 color0, inout float voxelLengthInside, inout int materialIndex, vec4 colorParameter )
{
	BRANCH
	if( voxelDataMode != 0.0 )
	{
		//get voxel data info
		vec3 gridSizeF = renderOperationData[ 5 ].xyz;
		float fillHolesDistance = abs( renderOperationData[ 5 ].w );
		bool fullFormat = renderOperationData[ 5 ].w < 0.0;
		//vec2 formatAndFillHolesDistance = unpackHalf2x16(asuint(renderOperationData[5].w));
		//bool fullFormat = formatAndFillHolesDistance.x > 0.5;
		//float fillHolesDistance = formatAndFillHolesDistance.y;
		vec3 boundsMin = renderOperationData[ 6 ].xyz;
		float cellSize = abs( renderOperationData[ 6 ].w );
		bool bakedOpacity = renderOperationData[ 6 ].w < 0.0;
		//float cellSize = renderOperationData[ 6 ].w;
		
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
		/*
		//add some blur/antialiasing
#ifndef SHADOW_CASTER
		float rx = random(fragCoord.xy);
		float ry = random(fragCoord.yz);
		float rz = random(fragCoord.zx);
		currentPosition += (vec3(rx, ry, rz) - vec3_splat(0.5)) / gridSizeF / 1.5;
#endif
		*/
		currentPosition = clamp( currentPosition, vec3_splat( 0 ), vec3_splat( 0.9999 ) ); //to have valid grid index
		currentPosition *= gridSizeF;
		
		/*
		if(capsLock)
		{
			currentPosition /= 4.0;
			currentPosition = round( currentPosition );
			currentPosition *= 4.0;
		}
		*/
		
		ivec3 currentIndex = ivec3( currentPosition );
		ivec3 startIndex = currentIndex;
	
		int voxelTextureSize = textureSize( voxelData, 0 ).x;

		const int maxSteps = GLOBAL_VOXEL_LOD_MAX_STEPS; //12;
		
		mat3 worldMatrix3;
		
		bool transparentFound = false;
		int shadingModel = 0;
		
	#if defined( BLEND_MODE_MASKED ) || defined( BLEND_MODE_TRANSPARENT )
		int transparencySteps = bakedOpacity ? 1 : 4;
		LOOP
		for( int nTransparentIteration = 0; nTransparentIteration < transparencySteps; nTransparentIteration++ )
	#endif
		{
			bool found = false;
			float foundVoxelValue = 0.0;
			
			float totalNearestIndexDistance = 10000.0;
			ivec3 totalNearestIndex = ivec3( 0, 0, 0 );
					
			//!!!!skip already checked voxel
			//!!!!!! if( any( cellIndex != lastCheckedIndex ) )
					
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

					if( any( currentPosition < vec3_splat( 0 ) ) || any( currentIndex >= gridSize ) )
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
				currentPosition = vec3( currentIndex );
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

			mat4 worldMatrix = mtxFromRows( worldMatrix0, worldMatrix1, worldMatrix2, vec4(0,0,0,1) );
			/*mat3 */worldMatrix3 = toMat3( worldMatrix );
			
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
			
			#if defined( BLEND_MODE_MASKED ) || defined( BLEND_MODE_TRANSPARENT )
			{
				MEDIUMP float opacity = 1.0;
				
#ifndef SHADOW_CASTER_DEFAULT

				//get material data
				int frameMaterialIndex = uint( renderOperationData[ 0 ].x );
			#ifdef MULTI_MATERIAL_COMBINED_PASS
				uint localGroupMaterialIndex = uint( materialIndex ) - uint( u_multiMaterialCombinedInfo.x );
				//BRANCH
				//if( localGroupMaterialIndex < 0 || localGroupMaterialIndex >= uint( u_multiMaterialCombinedInfo.y ) )
				//	discard;
				frameMaterialIndex = int( u_multiMaterialCombinedMaterials[ localGroupMaterialIndex / 4 ][ localGroupMaterialIndex % 4 ] );
			#endif
				//vec4 materialStandardFragment[ MATERIAL_STANDARD_FRAGMENT_SIZE ];
				//getMaterialData( s_materials, frameMaterialIndex, materialStandardFragment );
				
				shadingModel = u_materialShadingModel;

				BRANCH
				if( !bakedOpacity )
				{
					opacity = u_materialOpacity;
					
					vec2 texCoord0 = vec2_splat(0);//dummy
					vec2 unwrappedUVBeforeDisplacement = vec2_splat(0);//dummy
					vec4 customParameter1 = u_materialCustomParameters[0];
					vec4 customParameter2 = u_materialCustomParameters[1];
					vec4 instanceParameter1 = u_objectInstanceParameters[0];
					vec4 instanceParameter2 = u_objectInstanceParameters[1];

					//float rayTracingReflection = 0.0;
					//int shadingModel = 0;
					bool receiveDecals = false;
					bool useVertexColor = false;
					bool opacityDithering = false;
					float opacityMaskThreshold = 0.0;
					vec3 baseColor = vec3_splat( 0 );

					#define VOXEL_LOD_GET_OPACITY 1
					
					#ifdef SHADOW_CASTER
					
						#ifdef FRAGMENT_CODE_BODY
							#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
							#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
							{
								FRAGMENT_CODE_BODY
							}
							#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
							#undef CODE_BODY_TEXTURE2D
						#endif

					#else

						#ifdef OPACITY_CODE_BODY
							#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
							#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DLod(makeSampler(s_linearSamplerFragment, _sampler), _uv, pow( float( textureSize( makeSampler(s_linearSamplerFragment, _sampler), 0 ).x ), 0.5 ) * 0.1)
							{
								OPACITY_CODE_BODY
							}
							#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
							#undef CODE_BODY_TEXTURE2D
						#endif
					
					#endif
					
					#undef VOXEL_LOD_GET_OPACITY
				}
			
				if( u_materialUseVertexColor != 0.0 )
					opacity *= color0.w;					
			
#else
				opacity *= color0.w;
#endif //!SHADOW_CASTER_DEFAULT
				
				opacity *= colorParameter.w;
				
				if( opacity >= 1.0 || getDitherBoolean( fragCoord, opacity ) )
				{
					transparentFound = true;
					break;
				}

				//continue ray cast
				currentPosition += localRayDirection * 1.43 * float( nTransparentIteration + 1 ); //good? float( nTransparentIteration + 1 )
				currentIndex = ivec3( currentPosition );
				if( any( currentPosition < vec3_splat( 0 ) ) || any( currentIndex >= gridSize ) )
					discard;
			}
			#endif //defined( BLEND_MODE_MASKED ) || defined( BLEND_MODE_TRANSPARENT )
			
		}
		
	#if defined( BLEND_MODE_MASKED ) || defined( BLEND_MODE_TRANSPARENT )
		if( !transparentFound )
			discard;
	#endif
		

		//calculate length from mesh bounding box geometry ray intersection to voxel position
		
		//!!!!startIndex? bolee tochnoe znachenie moshno?

		
		vec3 localVector;
#ifdef SHADOW_CASTER
		float factor;	
		if( shadingModel == 2 ) //Foliage
			factor = 1.0;
		else
			factor = 2.0;		
		localVector = ( vec3( currentIndex - startIndex ) ) * cellSize * factor;		
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
