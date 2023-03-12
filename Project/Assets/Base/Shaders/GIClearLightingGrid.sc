// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#include "Common/bgfx_compute.sh"
//#include "Common.sh"

uniform vec4 giClearGridParameters;

IMAGE3D_WR(s_grid, rgba16f, 0);

NUM_THREADS(8, 8, 8)
void main()
{
	ivec3 gridSize = ivec3( giClearGridParameters.xyz );
	if( gl_GlobalInvocationID.x >= gridSize.x || gl_GlobalInvocationID.y >= gridSize.y || gl_GlobalInvocationID.z >= gridSize.z )
		return;
	
	ivec3 gridIndex = ivec3( gl_GlobalInvocationID );	
	imageStore( s_grid, gridIndex, vec4_splat(0) );
}



	//if( gridIndex.x == 5 )
	//	imageStore( s_grid, gridIndex, vec4(1,0,0,1) );
	
	//!!!!
	//imageStore( s_grid, gridIndex, vec4_splat(1) );
	

/*

	int gridSize = int( giClearGridParameters.x );	
	if( gl_GlobalInvocationID.x >= gridSize || gl_GlobalInvocationID.y >= gridSize || gl_GlobalInvocationID.z >= gridSize )
		return;
	
	ivec3 gridIndex = ivec3( gl_GlobalInvocationID );	
	
	imageStore( s_grid, gridIndex, vec4_splat(0) );

*/
