// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

struct Vertex
{
	vec3 position;
	vec3 normal;
	vec4 tangent;
	vec4 color;
	vec2 texCoord0;
	vec2 texCoord1;
	vec2 texCoord2;
	ivec4 blendIndices;
	vec4 blendWeights;
	float color3;
};

#define VertexElementType_None 0
#define VertexElementType_Float1 1
#define VertexElementType_Float2 2
#define VertexElementType_Float3 3
#define VertexElementType_Float4 4
#define VertexElementType_Short1 6
#define VertexElementType_Short2 7
#define VertexElementType_Short3 8
#define VertexElementType_Short4 9
#define VertexElementType_UByte4 10
#define VertexElementType_ColorARGB 11
#define VertexElementType_ColorABGR 12
#define VertexElementType_Integer1 13
#define VertexElementType_Integer2 14
#define VertexElementType_Integer3 15
#define VertexElementType_Integer4 16
#define VertexElementType_Half1 17
#define VertexElementType_Half2 18
#define VertexElementType_Half3 19
#define VertexElementType_Half4 20

void tessLoadVertex( inout Vertex vertex, int index, int vertexSizeInFloat )
{
	float vertexData[ 20 * 4 ];
	{
		//!!!!slowly
		
		for( int n = 0; n < vertexSizeInFloat; n++ )
		{
			int index2 = index * vertexSizeInFloat + n;			
			vertexData[ n ] = s_sourceVertexBuffer[ index2 / 4 ][ index2 % 4 ];
		}
	}
	/*
	vec4 vertexData[ 20 ];
	{
		for( int n = 0; n < vertexSizeInVec4; n++ )
			vertexData[ n ] = s_sourceVertexBuffer[ index * vertexSizeInVec4 + n ];
	}
	*/

	//!!!!
	vertex.position = vec3(0,0,0);
	vertex.normal = vec3(0,0,0);
	vertex.tangent = vec4(0,0,0,0);
	vertex.color = vec4(1,1,1,1);
	vertex.texCoord0 = vec2(0,0);
	vertex.texCoord1 = vec2(0,0);
	vertex.texCoord2 = vec2(0,0);
	vertex.blendIndices = ivec4(0,0,0,0);
	vertex.blendWeights = vec4(0,0,0,0);
	vertex.color3 = 0;

	
	//position
	{
		int info = int( u_tessellationSourceVertexLayout[ 0 ].x );
		int offset = info / 256;
		int format = info % 256;
		//uint info = asuint( u_tessellationSourceVertexLayout[ 0 ].x );
		//uint offset = ( info & uint( 0xffff0000 ) ) >> 16;
		//uint format = info & uint( 0x0000ffff );		
		////int offset = int( u_tessellationSourceVertexLayout[ 0 ].x );
		////int format = int( u_tessellationSourceVertexLayout[ 0 ].y );

		switch( format )
		{
		case VertexElementType_Float3:
			vertex.position = vec3( vertexData[ offset + 0 ], vertexData[ offset + 1 ], vertexData[ offset + 2 ] );
			break;
			
		default:
			vertex.position = vec3_splat( 0 );
			break;
		}
	}

	//normal
	{
		int info = int( u_tessellationSourceVertexLayout[ 0 ].y );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float3:
			vertex.normal = vec3( vertexData[ offset + 0 ], vertexData[ offset + 1 ], vertexData[ offset + 2 ] );
			break;
			
		default:
			vertex.normal = vec3_splat( 0 );
			break;
		}
	}

	/*
	//tangent
	{
		int info = int( u_tessellationSourceVertexLayout[ 0 ].z );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float4:
			vertex.tangent = vec4( vertexData[ offset + 0 ], vertexData[ offset + 1 ], vertexData[ offset + 2 ], vertexData[ offset + 3 ] );
			break;

		case VertexElementType_Half4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.tangent.xy = unpackTwoHalfs( vertexData[ offset0 / 4 ][ offset0 % 4 ] );
				vertex.tangent.zw = unpackTwoHalfs( vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;
			
		default:
			vertex.tangent = vec4_splat( 0 );
			break;
		}
	}

	//color
	{
		int info = int( u_tessellationSourceVertexLayout[ 0 ].w );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				int offset3 = offset + 3;
				vertex.color = vec4(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ],
					vertexData[ offset2 / 4 ][ offset2 % 4 ],
					vertexData[ offset3 / 4 ][ offset3 % 4 ] );
			}
			break;
		
		case VertexElementType_Half4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.color.xy = unpackTwoHalfs( vertexData[ offset0 / 4 ][ offset0 % 4 ] );
				vertex.color.zw = unpackTwoHalfs( vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		default:
			vertex.color = vec4_splat( 0 );
			break;
		}
	}

	//texCoord0
	{
		int info = int( u_tessellationSourceVertexLayout[ 1 ].x );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float2:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.texCoord0 = vec2(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		case VertexElementType_Half2:
			vertex.texCoord0 = unpackTwoHalfs( vertexData[ offset / 4 ][ offset % 4 ] );
			break;
			
		default:
			vertex.texCoord0 = vec2_splat( 0 );
			break;
		}
	}

	//texCoord1
	{
		int info = int( u_tessellationSourceVertexLayout[ 1 ].y );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float2:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.texCoord1 = vec2(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		case VertexElementType_Half2:
			vertex.texCoord1 = unpackTwoHalfs( vertexData[ offset / 4 ][ offset % 4 ] );
			break;
			
		default:
			vertex.texCoord1 = vec2_splat( 0 );
			break;
		}
	}

	//texCoord2
	{
		int info = int( u_tessellationSourceVertexLayout[ 1 ].z );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float2:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.texCoord2 = vec2(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		case VertexElementType_Half2:
			vertex.texCoord2 = unpackTwoHalfs( vertexData[ offset / 4 ][ offset % 4 ] );
			break;
			
		default:
			vertex.texCoord2 = vec2_splat( 0 );
			break;
		}
	}

	//blendIndices
	{
		int info = int( u_tessellationSourceVertexLayout[ 1 ].w );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
			//!!!!check
		case VertexElementType_Integer4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				int offset3 = offset + 3;
				vertex.blendIndices = ivec4(
					asuint( vertexData[ offset0 / 4 ][ offset0 % 4 ] ),
					asuint( vertexData[ offset1 / 4 ][ offset1 % 4 ] ),
					asuint( vertexData[ offset2 / 4 ][ offset2 % 4 ] ),
					asuint( vertexData[ offset3 / 4 ][ offset3 % 4 ] ) );
			}
			break;
			
		case VertexElementType_Float4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				int offset3 = offset + 3;
				vertex.blendIndices = ivec4( vec4(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ],
					vertexData[ offset2 / 4 ][ offset2 % 4 ],
					vertexData[ offset3 / 4 ][ offset3 % 4 ] ) );
			}
			break;

		default:
			vertex.blendIndices = ivec4( 0, 0, 0, 0 );
			break;
		}
	}

	//blendWeights
	{
		int info = int( u_tessellationSourceVertexLayout[ 2 ].x );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				int offset3 = offset + 3;
				vertex.blendWeights = vec4(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ],
					vertexData[ offset2 / 4 ][ offset2 % 4 ],
					vertexData[ offset3 / 4 ][ offset3 % 4 ] );
			}
			break;
		
		case VertexElementType_Half4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.blendWeights.xy = unpackTwoHalfs( vertexData[ offset0 / 4 ][ offset0 % 4 ] );
				vertex.blendWeights.zw = unpackTwoHalfs( vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		default:
			vertex.blendWeights = vec4_splat( 0 );
			break;
		}
	}

	//color3
	{
		int info = int( u_tessellationSourceVertexLayout[ 2 ].y );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float1:
			vertex.color3 = vertexData[ offset / 4 ][ offset % 4 ];
			break;
		
		default:
			vertex.color3 = 0;
			break;
		}
	}
	*/
	
	
/*	
	//position
	{
		int info = int( u_tessellationSourceVertexLayout[ 0 ].x );
		int offset = info / 256;
		int format = info % 256;
		//uint info = asuint( u_tessellationSourceVertexLayout[ 0 ].x );
		//uint offset = ( info & uint( 0xffff0000 ) ) >> 16;
		//uint format = info & uint( 0x0000ffff );		
		////int offset = int( u_tessellationSourceVertexLayout[ 0 ].x );
		////int format = int( u_tessellationSourceVertexLayout[ 0 ].y );

		switch( format )
		{
		case VertexElementType_Float3:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				vertex.position = vec3(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ],
					vertexData[ offset2 / 4 ][ offset2 % 4 ] );
			}
			break;
			
		default:
			vertex.position = vec3_splat( 0 );
			break;
		}
	}

	//normal
	{
		int info = int( u_tessellationSourceVertexLayout[ 0 ].y );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float3:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				vertex.normal = vec3(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ],
					vertexData[ offset2 / 4 ][ offset2 % 4 ] );
			}
			break;
			
		default:
			vertex.normal = vec3_splat( 0 );
			break;
		}
	}

	//tangent
	{
		int info = int( u_tessellationSourceVertexLayout[ 0 ].z );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				int offset3 = offset + 3;
				vertex.tangent = vec4(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ],
					vertexData[ offset2 / 4 ][ offset2 % 4 ],
					vertexData[ offset3 / 4 ][ offset3 % 4 ] );
			}
			break;

		case VertexElementType_Half4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.tangent.xy = unpackTwoHalfs( vertexData[ offset0 / 4 ][ offset0 % 4 ] );
				vertex.tangent.zw = unpackTwoHalfs( vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;
			
		default:
			vertex.tangent = vec4_splat( 0 );
			break;
		}
	}

	//color
	{
		int info = int( u_tessellationSourceVertexLayout[ 0 ].w );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				int offset3 = offset + 3;
				vertex.color = vec4(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ],
					vertexData[ offset2 / 4 ][ offset2 % 4 ],
					vertexData[ offset3 / 4 ][ offset3 % 4 ] );
			}
			break;
		
		case VertexElementType_Half4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.color.xy = unpackTwoHalfs( vertexData[ offset0 / 4 ][ offset0 % 4 ] );
				vertex.color.zw = unpackTwoHalfs( vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		default:
			vertex.color = vec4_splat( 0 );
			break;
		}
	}

	//texCoord0
	{
		int info = int( u_tessellationSourceVertexLayout[ 1 ].x );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float2:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.texCoord0 = vec2(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		case VertexElementType_Half2:
			vertex.texCoord0 = unpackTwoHalfs( vertexData[ offset / 4 ][ offset % 4 ] );
			break;
			
		default:
			vertex.texCoord0 = vec2_splat( 0 );
			break;
		}
	}

	//texCoord1
	{
		int info = int( u_tessellationSourceVertexLayout[ 1 ].y );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float2:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.texCoord1 = vec2(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		case VertexElementType_Half2:
			vertex.texCoord1 = unpackTwoHalfs( vertexData[ offset / 4 ][ offset % 4 ] );
			break;
			
		default:
			vertex.texCoord1 = vec2_splat( 0 );
			break;
		}
	}

	//texCoord2
	{
		int info = int( u_tessellationSourceVertexLayout[ 1 ].z );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float2:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.texCoord2 = vec2(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		case VertexElementType_Half2:
			vertex.texCoord2 = unpackTwoHalfs( vertexData[ offset / 4 ][ offset % 4 ] );
			break;
			
		default:
			vertex.texCoord2 = vec2_splat( 0 );
			break;
		}
	}

	//blendIndices
	{
		int info = int( u_tessellationSourceVertexLayout[ 1 ].w );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
			//!!!!check
		case VertexElementType_Integer4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				int offset3 = offset + 3;
				vertex.blendIndices = ivec4(
					asuint( vertexData[ offset0 / 4 ][ offset0 % 4 ] ),
					asuint( vertexData[ offset1 / 4 ][ offset1 % 4 ] ),
					asuint( vertexData[ offset2 / 4 ][ offset2 % 4 ] ),
					asuint( vertexData[ offset3 / 4 ][ offset3 % 4 ] ) );
			}
			break;
			
		case VertexElementType_Float4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				int offset3 = offset + 3;
				vertex.blendIndices = ivec4( vec4(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ],
					vertexData[ offset2 / 4 ][ offset2 % 4 ],
					vertexData[ offset3 / 4 ][ offset3 % 4 ] ) );
			}
			break;

		default:
			vertex.blendIndices = ivec4( 0, 0, 0, 0 );
			break;
		}
	}

	//blendWeights
	{
		int info = int( u_tessellationSourceVertexLayout[ 2 ].x );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				int offset2 = offset + 2;
				int offset3 = offset + 3;
				vertex.blendWeights = vec4(
					vertexData[ offset0 / 4 ][ offset0 % 4 ],
					vertexData[ offset1 / 4 ][ offset1 % 4 ],
					vertexData[ offset2 / 4 ][ offset2 % 4 ],
					vertexData[ offset3 / 4 ][ offset3 % 4 ] );
			}
			break;
		
		case VertexElementType_Half4:
			{
				int offset0 = offset + 0;
				int offset1 = offset + 1;
				vertex.blendWeights.xy = unpackTwoHalfs( vertexData[ offset0 / 4 ][ offset0 % 4 ] );
				vertex.blendWeights.zw = unpackTwoHalfs( vertexData[ offset1 / 4 ][ offset1 % 4 ] );
			}
			break;

		default:
			vertex.blendWeights = vec4_splat( 0 );
			break;
		}
	}

	//color3
	{
		int info = int( u_tessellationSourceVertexLayout[ 2 ].y );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float1:
			vertex.color3 = vertexData[ offset / 4 ][ offset % 4 ];
			break;
		
		default:
			vertex.color3 = 0;
			break;
		}
	}
*/
}

void tessWriteVertex( inout Vertex vertex, int index, int vertexSizeInFloat )
{
	float vertexData[ 20 * 4 ];
	
	//!!!!need?
	{
		//!!!!slowly?
		UNROLL
		for( int n = 0; n < 20 * 4; n++ )
			vertexData[ n ] = 0;
	}
	
	//position	
	{
		int info = int( u_tessellationDestVertexLayout[ 0 ].x );
		int offset = info / 256;
		int format = info % 256;
		//uint info = asuint( u_tessellationDestVertexLayout[ 0 ].x );
		//uint offset = ( info & uint( 0xffff0000 ) ) >> 16;
		//uint format = info & uint( 0x0000ffff );
		//int offset = int( u_tessellationDestVertexLayout[ 0 ].x );
		//int format = int( u_tessellationDestVertexLayout[ 0 ].y );

		switch( format )
		{
		case VertexElementType_Float3:
			vertexData[ offset + 0 ] = vertex.position.x;
			vertexData[ offset + 1 ] = vertex.position.y;
			vertexData[ offset + 2 ] = vertex.position.z;
			break;
		}
	}

	//normal
	{
		int info = int( u_tessellationDestVertexLayout[ 0 ].y );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float3:
			vertexData[ offset + 0 ] = vertex.normal.x;
			vertexData[ offset + 1 ] = vertex.normal.y;
			vertexData[ offset + 2 ] = vertex.normal.z;
			break;
		}
	}

	//tangent
	{
		int info = int( u_tessellationDestVertexLayout[ 0 ].z );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float4:
			vertexData[ offset + 0 ] = vertex.tangent.x;
			vertexData[ offset + 1 ] = vertex.tangent.y;
			vertexData[ offset + 2 ] = vertex.tangent.z;
			vertexData[ offset + 3 ] = vertex.tangent.w;
			break;

		case VertexElementType_Half4:
			vertexData[ offset + 0 ] = packTwoHalfs( vertex.tangent.xy );
			vertexData[ offset + 1 ] = packTwoHalfs( vertex.tangent.zw );
			break;
		}
	}

	//color
	{
		int info = int( u_tessellationDestVertexLayout[ 0 ].w );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float4:
			vertexData[ offset + 0 ] = vertex.color.x;
			vertexData[ offset + 1 ] = vertex.color.y;
			vertexData[ offset + 2 ] = vertex.color.z;
			vertexData[ offset + 3 ] = vertex.color.w;
			break;

		case VertexElementType_Half4:
			vertexData[ offset + 0 ] = packTwoHalfs( vertex.color.xy );
			vertexData[ offset + 1 ] = packTwoHalfs( vertex.color.zw );
			break;
			
			//!!!!BYTE4
			
		}
	}

	//texCoord0
	{
		int info = int( u_tessellationDestVertexLayout[ 1 ].x );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float2:
			vertexData[ offset + 0 ] = vertex.texCoord0.x;
			vertexData[ offset + 1 ] = vertex.texCoord0.y;
			break;

		case VertexElementType_Half2:
			vertexData[ offset ] = packTwoHalfs( vertex.texCoord0 );
			break;
		}
	}

	//texCoord1
	{
		int info = int( u_tessellationDestVertexLayout[ 1 ].y );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float2:
			vertexData[ offset + 0 ] = vertex.texCoord1.x;
			vertexData[ offset + 1 ] = vertex.texCoord1.y;
			break;

		case VertexElementType_Half2:
			vertexData[ offset ] = packTwoHalfs( vertex.texCoord1 );
			break;
		}
	}

	//texCoord2
	{
		int info = int( u_tessellationDestVertexLayout[ 1 ].z );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float2:
			vertexData[ offset + 0 ] = vertex.texCoord2.x;
			vertexData[ offset + 1 ] = vertex.texCoord2.y;
			break;

		case VertexElementType_Half2:
			vertexData[ offset ] = packTwoHalfs( vertex.texCoord2 );
			break;
		}
	}

	//blendIndices
	{
		int info = int( u_tessellationDestVertexLayout[ 1 ].w );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
			//!!!!check
		case VertexElementType_Integer4:
			vertexData[ offset + 0 ] = asfloat( vertex.blendIndices.x );
			vertexData[ offset + 1 ] = asfloat( vertex.blendIndices.y );
			vertexData[ offset + 2 ] = asfloat( vertex.blendIndices.z );
			vertexData[ offset + 3 ] = asfloat( vertex.blendIndices.w );
			break;
			
		case VertexElementType_Float4:
			vertexData[ offset + 0 ] = vertex.blendIndices.x;
			vertexData[ offset + 1 ] = vertex.blendIndices.y;
			vertexData[ offset + 2 ] = vertex.blendIndices.z;
			vertexData[ offset + 3 ] = vertex.blendIndices.w;
			break;
		}
	}
	
	//blendWeights
	{
		int info = int( u_tessellationDestVertexLayout[ 2 ].x );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float4:
			vertexData[ offset + 0 ] = vertex.blendWeights.x;
			vertexData[ offset + 1 ] = vertex.blendWeights.y;
			vertexData[ offset + 2 ] = vertex.blendWeights.z;
			vertexData[ offset + 3 ] = vertex.blendWeights.w;
			break;

		case VertexElementType_Half4:
			vertexData[ offset + 0 ] = packTwoHalfs( vertex.blendWeights.xy );
			vertexData[ offset + 1 ] = packTwoHalfs( vertex.blendWeights.zw );
			break;
		}
	}

	//color3
	{
		int info = int( u_tessellationDestVertexLayout[ 2 ].y );
		int offset = info / 256;
		int format = info % 256;

		switch( format )
		{
		case VertexElementType_Float1:
			vertexData[ offset ] = vertex.color3;
			break;
		}
	}
	
	//write to dest buffer
	{
		int vertexSizeInVec4 = vertexSizeInFloat / 4;
		for( int n = 0; n < vertexSizeInVec4; n++ )
			s_destVertexBuffer[ index * vertexSizeInVec4 + n ] = vec4( vertexData[ n * 4 + 0 ], vertexData[ n * 4 + 1 ], vertexData[ n * 4 + 2 ], vertexData[ n * 4 + 3 ] );
		
		/*
		for( int n = 0; n < vertexSizeInFloat; n++ )
			s_destVertexBuffer[ index * vertexSizeInFloat + n ] = vertexData[ n ];
		*/
		
		/*
		for( int n = 0; n < vertexSizeInVec4; n++ )
			s_destVertexBuffer[ index * vertexSizeInVec4 + n ] = vec4( vertexData[ n * 4 + 0 ], vertexData[ n * 4 + 1 ], vertexData[ n * 4 + 2 ], vertexData[ n * 4 + 3 ] );
		*/
	}
}
