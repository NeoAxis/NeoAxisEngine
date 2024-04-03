// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#include "Common/bgfx_compute.sh"
#include "Common.sh"
#include "UniformsFragment.sh"

IMAGE3D_WO(s_lightGrid, rgba32f, 0);
SAMPLER2D(s_lightsTexture, 1);

bool rectIntersects( vec4 v1, vec4 v2 )
{
	return v1[ 2 ] > v2[ 0 ] && v1[ 3 ] > v2[ 1 ] && v1[ 0 ] < v2[ 2 ] && v1[ 1 ] < v2[ 3 ];
}

NUM_THREADS(32, 32, 1)
void main()
{
	int gridSize = int( d_lightGridSize );
	uvec2 gridIndex = gl_GlobalInvocationID.xy;
	
	if( gridIndex.x >= gridSize || gridIndex.y >= gridSize )
		return;
	
	float result[ 31 ];
	UNROLL
	for( int n=0;n<31;n++)
		result[n] = 0.0;
	
	int count = 0;

	vec2 cellStart = vec2_splat( d_lightGridStart ) + vec2( gridIndex ) * d_lightGridCellSize;
	vec4 cellRectangle = vec4( cellStart.x, cellStart.y, cellStart.x + d_lightGridCellSize, cellStart.y + d_lightGridCellSize );
	
	int lightCount = d_lightCount;
	LOOP
	for( int nLight = 1; nLight < lightCount; nLight++ )
	{
		vec4 lightData0 = texelFetch( s_lightsTexture, uvec2( 0, nLight ), 0 );		
		vec3 lightPosition = lightData0.xyz;
		float lightBoundingRadius = lightData0.w;
		
		vec4 lightRectangle = vec4( 
			lightPosition.x - lightBoundingRadius, lightPosition.y - lightBoundingRadius,
			lightPosition.x + lightBoundingRadius, lightPosition.y + lightBoundingRadius );

		bool intersects = rectIntersects( lightRectangle, cellRectangle );

		if( intersects )
		{
			if( count == 31 )
			{
				//too much lights. set special overloaded state
				count = -100;
				break;
			}
			
			result[ count ] = nLight;
			count++;
		}		
	}

	imageStore( s_lightGrid, ivec3( gridIndex, 0 ), vec4( count, result[ 0 ], result[ 1 ], result[ 2 ] ) );
	BRANCH
	if( count >= 4 )
	{
		imageStore( s_lightGrid, ivec3( gridIndex, 1 ), vec4( result[ 3 ], result[ 4 ], result[ 5 ], result[ 6 ] ) );
		if( count >= 8 )
		{
			imageStore( s_lightGrid, ivec3( gridIndex, 2 ), vec4( result[ 7 ], result[ 8 ], result[ 9 ], result[ 10 ] ) );
			if( count >= 12 )
			{
				imageStore( s_lightGrid, ivec3( gridIndex, 3 ), vec4( result[ 11 ], result[ 12 ], result[ 13 ], result[ 14 ] ) );
				BRANCH
				if( count >= 16 )
				{
					imageStore( s_lightGrid, ivec3( gridIndex, 4 ), vec4( result[ 15 ], result[ 16 ], result[ 17 ], result[ 18 ] ) );
					if( count >= 20 )
					{
						imageStore( s_lightGrid, ivec3( gridIndex, 5 ), vec4( result[ 19 ], result[ 20 ], result[ 21 ], result[ 22 ] ) );
						if( count >= 24 )
						{
							imageStore( s_lightGrid, ivec3( gridIndex, 6 ), vec4( result[ 23 ], result[ 24 ], result[ 25 ], result[ 26 ] ) );
							if( count >= 28 )
								imageStore( s_lightGrid, ivec3( gridIndex, 7 ), vec4( result[ 27 ], result[ 28 ], result[ 29 ], result[ 30 ] ) );
						}
					}
				}
			}
		}
	}
}
