// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#include "Common/bgfx_compute.sh"
//#include "Common.sh"

uniform vec4 giClearGridParameters;

UIMAGE3D_WR(s_grid, rg32ui, 0);

NUM_THREADS(8, 8, 8)
void main()
{
	ivec3 gridSize = ivec3( giClearGridParameters.xyz );
	if( gl_GlobalInvocationID.x >= gridSize.x || gl_GlobalInvocationID.y >= gridSize.y || gl_GlobalInvocationID.z >= gridSize.z )
		return;
	
	ivec3 gridIndex = ivec3( gl_GlobalInvocationID );
	imageStore( s_grid, gridIndex, uvec4_splat(0) );
}


	/*
	int gridSize = int( giClearGridParameters.x );
	if( gl_GlobalInvocationID.x >= gridSize || gl_GlobalInvocationID.y >= gridSize || gl_GlobalInvocationID.z >= gridSize )
		return;
	
	ivec3 gridIndex = ivec3( gl_GlobalInvocationID );
	
	imageStore( s_grid, gridIndex, uvec4_splat(0) );
	*/



	/*
	if(gridIndex.x > 30 && gridIndex.x < 35 && gridIndex.y > 30 && gridIndex.y < 35 && gridIndex.z > 30 && gridIndex.z < 35)
	{
		uvec4 result = uvec4_splat( 0 );
		
		vec3 emissive = vec3(1,0,0);
		result.w = convertVec4ToRGBA8( encodeRGBE8( emissive ) * 255.0 );
		
		imageStore( s_grid, gridIndex, result );
		return;
	}
	*/	
