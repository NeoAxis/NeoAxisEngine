// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#include "Common/bgfx_compute.sh"

uniform vec4 giCopyLightingGridParameters[3];

IMAGE3D_WR(s_lightingGridCopy, rgba16f, 0);
SAMPLER3D(s_lightingGridTexture, 1);
//!!!!is not work
//IMAGE3D_RO(s_lightingGrid, rgba16f, 1);

NUM_THREADS(8, 8, 8)
void main()
{
	float gridResolution = giCopyLightingGridParameters[ 0 ].x;
	float currentLevel = giCopyLightingGridParameters[ 0 ].y;
	vec3 giGridIndexesMin = giCopyLightingGridParameters[ 1 ].xyz;
	vec3 giGridIndexesMax = giCopyLightingGridParameters[ 2 ].xyz;

	ivec3 giGridIndexOffset = ivec3( giGridIndexesMin.xyz );
	ivec3 giGridIndexSize = ivec3( giGridIndexesMax.xyz ) - giGridIndexOffset + ivec3(1,1,1);
	
	if( gl_GlobalInvocationID.x >= giGridIndexSize.x || gl_GlobalInvocationID.y >= giGridIndexSize.y || gl_GlobalInvocationID.z >= giGridIndexSize.z )
		return;
	
	ivec3 giGridIndex = giGridIndexOffset + ivec3( gl_GlobalInvocationID );
	
	ivec3 giGridIndexWithOffset = giGridIndex;
	giGridIndexWithOffset.x += int( gridResolution * currentLevel );	
	vec4 value = texelFetch( s_lightingGridTexture, giGridIndexWithOffset, 0 );
	//vec4 value = imageLoad( s_lightingGrid, giGridIndexWithOffset );

	//!!!!
	//value = saturate( value );
	//value.xyz += vec3(0,0,0.5);
	//value.w = 1;
	//vec4 value = vec4(0,0,0.5,1);
	
	imageStore( s_lightingGridCopy, giGridIndex, value );
}
