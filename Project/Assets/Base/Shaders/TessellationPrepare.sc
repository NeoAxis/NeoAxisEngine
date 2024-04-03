// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#include "Common/bgfx_compute.sh"
#include "Common.sh"

uniform vec4 u_tessellationParameters;
uniform vec4 u_tessellationSourceVertexLayout[ 3 ];
uniform vec4 u_tessellationDestVertexLayout[ 3 ];
////source and dest are same
//#define u_tessellationDestVertexLayout u_tessellationSourceVertexLayout

BUFFER_RO(s_sourceVertexBuffer, vec4, 0);
BUFFER_RO(s_sourceIndexBuffer, uint, 1);
BUFFER_WO(s_destVertexBuffer, vec4, 2);
BUFFER_WO(s_destIndexBuffer, uint, 3);

//#ifdef GLOBAL_TESSELLATION
#include "LoadWriteVertex.sh"
//#endif

float getVectorsAngle( vec3 vector1, vec3 vector2 )
{
	float _cos = dot( vector1, vector2 ) / ( length( vector1 ) * length( vector2 ) );
	_cos = clamp( _cos, -1.0, 1.0 );
	return acos( _cos );
}

NUM_THREADS(512, 1, 1)
void main()
{
	int sourceTriangleCount = int( u_tessellationParameters.x );
	int triangleIndex = int( gl_GlobalInvocationID.x );
	if( triangleIndex >= sourceTriangleCount )
		return;

	int sourceVertexSizeInFloat = int( u_tessellationParameters.y );
	int destVertexSizeInFloat = int( u_tessellationParameters.z );
	//int sourceVertexSizeInVec4 = int( u_tessellationParameters.y );
	//int destVertexSizeInVec4 = int( u_tessellationParameters.z );
	
	int index0 = s_sourceIndexBuffer[ triangleIndex * 3 + 0 ];
	int index1 = s_sourceIndexBuffer[ triangleIndex * 3 + 1 ];
	int index2 = s_sourceIndexBuffer[ triangleIndex * 3 + 2 ];

	//get source vertices
	Vertex sourceVertices[ 3 ];
	tessLoadVertex( sourceVertices[ 0 ], index0, sourceVertexSizeInFloat );//, u_tessellationSourceVertexLayout );
	tessLoadVertex( sourceVertices[ 1 ], index1, sourceVertexSizeInFloat );//, u_tessellationSourceVertexLayout );
	tessLoadVertex( sourceVertices[ 2 ], index2, sourceVertexSizeInFloat );//, u_tessellationSourceVertexLayout );

	
	//!!!!
	
	const int destVertexCount = 4;
	const int destIndexCount = 9;
	
	Vertex destVertices[ destVertexCount ];	
	destVertices[ 0 ] = sourceVertices[ 0 ];
	destVertices[ 1 ] = sourceVertices[ 1 ];
	destVertices[ 2 ] = sourceVertices[ 2 ];

	
	{	
		Vertex center;
		center.position = ( destVertices[ 0 ].position + destVertices[ 1 ].position + destVertices[ 2 ].position ) / 3;

		center.normal = normalize( ( destVertices[ 0 ].normal + destVertices[ 1 ].normal + destVertices[ 2 ].normal ) / 3 );

		center.tangent.xyz = normalize( ( destVertices[ 0 ].tangent.xyz + destVertices[ 1 ].tangent.xyz + destVertices[ 2 ].tangent.xyz ) / 3 );
		center.tangent.w = destVertices[ 0 ].tangent.w;

		center.color = ( destVertices[ 0 ].color + destVertices[ 1 ].color + destVertices[ 2 ].color ) / 3;
		center.texCoord0 = ( destVertices[ 0 ].texCoord0 + destVertices[ 1 ].texCoord0 + destVertices[ 2 ].texCoord0 ) / 3;
		center.texCoord1 = ( destVertices[ 0 ].texCoord1 + destVertices[ 1 ].texCoord1 + destVertices[ 2 ].texCoord1 ) / 3;
		center.texCoord2 = ( destVertices[ 0 ].texCoord2 + destVertices[ 1 ].texCoord2 + destVertices[ 2 ].texCoord2 ) / 3;

		//!!!!
		center.blendIndices = destVertices[ 0 ].blendIndices;
		center.blendWeights = destVertices[ 0 ].blendWeights;

		center.color3 = destVertices[ 0 ].color3;

		float len = 
			length( destVertices[ 1 ].position - destVertices[ 0 ].position ) + 
			length( destVertices[ 2 ].position - destVertices[ 1 ].position ) + 
			length( destVertices[ 0 ].position - destVertices[ 2 ].position );

		float angleSum = 
			getVectorsAngle( destVertices[ 0 ].normal, destVertices[ 1 ].normal ) + 
			getVectorsAngle( destVertices[ 1 ].normal, destVertices[ 2 ].normal ) + 
			getVectorsAngle( destVertices[ 2 ].normal, destVertices[ 0 ].normal );
			
		//float positionOffset = len * 0.025;
		float positionOffset = angleSum * len * 0.025;

		center.position += center.normal * positionOffset;
		
	
		destVertices[ 3 ] = center;
	}
	
	int destIndices[ destIndexCount ];

	destIndices[ 0 ] = 0;
	destIndices[ 1 ] = 1;
	destIndices[ 2 ] = 3;

	destIndices[ 3 ] = 1;
	destIndices[ 4 ] = 2;
	destIndices[ 5 ] = 3;

	destIndices[ 6 ] = 2;
	destIndices[ 7 ] = 0;
	destIndices[ 8 ] = 3;

	
/*	
	//!!!!
	int steps = 2;
	
	for( int step = 0; step < steps; step++ )
	{
	}
*/
	
	//!!!!
	//if (capsLock)
	//{
	//	destVertices[ 0 ].position += vec3( 0.1, 0, 0 );
	//}

	for( int n = 0; n < destVertexCount; n++ )
		tessWriteVertex( destVertices[ n ], triangleIndex * destVertexCount + n, destVertexSizeInFloat );

	for( int n = 0; n < destIndexCount; n++ )
		s_destIndexBuffer[ triangleIndex * destIndexCount + n ] = triangleIndex * destVertexCount + destIndices[ n ];

	
	
	
	/*
	
	Vertex destVertices[ 3 ];
	
	destVertices[ 0 ] = sourceVertices[ 0 ];
	destVertices[ 1 ] = sourceVertices[ 1 ];
	destVertices[ 2 ] = sourceVertices[ 2 ];
	
	//!!!!
	//if (capsLock)
	{
		destVertices[ 0 ].position += vec3( 0.1, 0, 0 );
	}
	
	tessWriteVertex( destVertices[ 0 ], triangleIndex * 3 + 0, destVertexSizeInVec4 );
	tessWriteVertex( destVertices[ 1 ], triangleIndex * 3 + 1, destVertexSizeInVec4 );
	tessWriteVertex( destVertices[ 2 ], triangleIndex * 3 + 2, destVertexSizeInVec4 );
	
	s_destIndexBuffer[ triangleIndex * 3 + 0 ] = triangleIndex * 3 + 0;
	s_destIndexBuffer[ triangleIndex * 3 + 1 ] = triangleIndex * 3 + 1;
	s_destIndexBuffer[ triangleIndex * 3 + 2 ] = triangleIndex * 3 + 2;	
	
	*/
}
