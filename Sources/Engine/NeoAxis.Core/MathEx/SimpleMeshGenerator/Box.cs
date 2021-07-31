// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GenerateBox( Vector3 size, out Vector3[] positions, out int[] indices )
		{
			//if( size.X < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateBox: size.X < 0." );
			//if( size.Y < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateBox: size.Y < 0." );
			//if( size.Z < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateBox: size.Z < 0." );

			Vector3 half = size * .5;

			positions = new Vector3[ 8 ];
			positions[ 0 ] = new Vector3( half.X, -half.Y, -half.Z );
			positions[ 1 ] = new Vector3( half.X, -half.Y, half.Z );
			positions[ 2 ] = new Vector3( half.X, half.Y, -half.Z );
			positions[ 3 ] = new Vector3( half.X, half.Y, half.Z );
			positions[ 4 ] = new Vector3( -half.X, -half.Y, -half.Z );
			positions[ 5 ] = new Vector3( -half.X, -half.Y, half.Z );
			positions[ 6 ] = new Vector3( -half.X, half.Y, -half.Z );
			positions[ 7 ] = new Vector3( -half.X, half.Y, half.Z );

			indices = new int[] {
				0, 3, 1,
				0, 2, 3,
				3, 6, 7,
				3, 2, 6,
				1, 7, 5,
				1, 3, 7,
				4, 7, 6,
				4, 5, 7,
				1, 4, 0,
				5, 4, 1,
				4, 2, 0,
				4, 6, 2 };
		}

		public static void GenerateBox( Vector3 size, bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			//if( size.X < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateBox: size.X < 0." );
			//if( size.Y < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateBox: size.Y < 0." );
			//if( size.Z < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateBox: size.Z < 0." );

			Vector3 half = size * .5;
			if( insideOut )
				half = -half;

			positions = new Vector3[ 24 ];
			positions[ 0 ] = new Vector3( -half.X, half.Y, -half.Z );
			positions[ 1 ] = new Vector3( -half.X, half.Y, half.Z );
			positions[ 2 ] = new Vector3( half.X, half.Y, half.Z );
			positions[ 3 ] = new Vector3( half.X, half.Y, -half.Z );
			positions[ 4 ] = new Vector3( -half.X, -half.Y, -half.Z );
			positions[ 5 ] = new Vector3( half.X, -half.Y, -half.Z );
			positions[ 6 ] = new Vector3( half.X, -half.Y, half.Z );
			positions[ 7 ] = new Vector3( -half.X, -half.Y, half.Z );
			positions[ 8 ] = new Vector3( -half.X, half.Y, -half.Z );
			positions[ 9 ] = new Vector3( half.X, half.Y, -half.Z );
			positions[ 10 ] = new Vector3( half.X, -half.Y, -half.Z );
			positions[ 11 ] = new Vector3( -half.X, -half.Y, -half.Z );
			positions[ 12 ] = new Vector3( half.X, half.Y, -half.Z );
			positions[ 13 ] = new Vector3( half.X, half.Y, half.Z );
			positions[ 14 ] = new Vector3( half.X, -half.Y, half.Z );
			positions[ 15 ] = new Vector3( half.X, -half.Y, -half.Z );
			positions[ 16 ] = new Vector3( half.X, half.Y, half.Z );
			positions[ 17 ] = new Vector3( -half.X, half.Y, half.Z );
			positions[ 18 ] = new Vector3( -half.X, -half.Y, half.Z );
			positions[ 19 ] = new Vector3( half.X, -half.Y, half.Z );
			positions[ 20 ] = new Vector3( -half.X, half.Y, half.Z );
			positions[ 21 ] = new Vector3( -half.X, half.Y, -half.Z );
			positions[ 22 ] = new Vector3( -half.X, -half.Y, -half.Z );
			positions[ 23 ] = new Vector3( -half.X, -half.Y, half.Z );

			normals = new Vector3[ 24 ];
			normals[ 0 ] = new Vector3( 0, 1, 0 );
			normals[ 1 ] = new Vector3( 0, 1, 0 );
			normals[ 2 ] = new Vector3( 0, 1, 0 );
			normals[ 3 ] = new Vector3( 0, 1, 0 );
			normals[ 4 ] = new Vector3( 0, -1, 0 );
			normals[ 5 ] = new Vector3( 0, -1, 0 );
			normals[ 6 ] = new Vector3( 0, -1, 0 );
			normals[ 7 ] = new Vector3( 0, -1, 0 );
			normals[ 8 ] = new Vector3( 0, 0, -1 );
			normals[ 9 ] = new Vector3( 0, 0, -1 );
			normals[ 10 ] = new Vector3( 0, 0, -1 );
			normals[ 11 ] = new Vector3( 0, 0, -1 );
			normals[ 12 ] = new Vector3( 1, 0, 0 );
			normals[ 13 ] = new Vector3( 1, 0, 0 );
			normals[ 14 ] = new Vector3( 1, 0, 0 );
			normals[ 15 ] = new Vector3( 1, 0, 0 );
			normals[ 16 ] = new Vector3( 0, 0, 1 );
			normals[ 17 ] = new Vector3( 0, 0, 1 );
			normals[ 18 ] = new Vector3( 0, 0, 1 );
			normals[ 19 ] = new Vector3( 0, 0, 1 );
			normals[ 20 ] = new Vector3( -1, 0, 0 );
			normals[ 21 ] = new Vector3( -1, 0, 0 );
			normals[ 22 ] = new Vector3( -1, 0, 0 );
			normals[ 23 ] = new Vector3( -1, 0, 0 );

			tangents = new Vector4[ 24 ];
			tangents[ 0 ] = new Vector4( -1, 0, 0, -1 );
			tangents[ 1 ] = new Vector4( -1, 0, 0, -1 );
			tangents[ 2 ] = new Vector4( -1, 0, 0, -1 );
			tangents[ 3 ] = new Vector4( -1, 0, 0, -1 );
			tangents[ 4 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 5 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 6 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 7 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 8 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 9 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 10 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 11 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 12 ] = new Vector4( 0, 1, 0, -1 );
			tangents[ 13 ] = new Vector4( 0, 1, 0, -1 );
			tangents[ 14 ] = new Vector4( 0, 1, 0, -1 );
			tangents[ 15 ] = new Vector4( 0, 1, 0, -1 );
			tangents[ 16 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 17 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 18 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 19 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 20 ] = new Vector4( 0, -1, 0, -1 );
			tangents[ 21 ] = new Vector4( 0, -1, 0, -1 );
			tangents[ 22 ] = new Vector4( 0, -1, 0, -1 );
			tangents[ 23 ] = new Vector4( 0, -1, 0, -1 );

			texCoords = new Vector2[ 24 ];
			texCoords[ 0 ] = new Vector2( 1, 1 );
			texCoords[ 1 ] = new Vector2( 1, 0 );
			texCoords[ 2 ] = new Vector2( 0, 0 );
			texCoords[ 3 ] = new Vector2( 0, 1 );
			texCoords[ 4 ] = new Vector2( 0, 1 );
			texCoords[ 5 ] = new Vector2( 1, 1 );
			texCoords[ 6 ] = new Vector2( 1, 0 );
			texCoords[ 7 ] = new Vector2( 0, 0 );
			texCoords[ 8 ] = new Vector2( 0, 1 );
			texCoords[ 9 ] = new Vector2( 1, 1 );
			texCoords[ 10 ] = new Vector2( 1, 0 );
			texCoords[ 11 ] = new Vector2( 0, 0 );
			texCoords[ 12 ] = new Vector2( 1, 1 );
			texCoords[ 13 ] = new Vector2( 1, 0 );
			texCoords[ 14 ] = new Vector2( 0, 0 );
			texCoords[ 15 ] = new Vector2( 0, 1 );
			texCoords[ 16 ] = new Vector2( 1, 0 );
			texCoords[ 17 ] = new Vector2( 0, 0 );
			texCoords[ 18 ] = new Vector2( 0, 1 );
			texCoords[ 19 ] = new Vector2( 1, 1 );
			texCoords[ 20 ] = new Vector2( 0, 0 );
			texCoords[ 21 ] = new Vector2( 0, 1 );
			texCoords[ 22 ] = new Vector2( 1, 1 );
			texCoords[ 23 ] = new Vector2( 1, 0 );

			if( insideOut )
			{
				for( int n = 0; n < texCoords.Length; n++ )
					texCoords[ n ] = new Vector2( 1.0f - texCoords[ n ].X, 1.0f - texCoords[ n ].Y );
			}

			indices = new int[] {
				0, 1, 2, 2, 3, 0, //+y face 
				4, 5, 6, 6, 7, 4, // -y face
				8, 9, 10, 10, 11, 8, // -z face
				12, 13, 14, 14, 15, 12, //+x face
				16, 17, 18, 18, 19, 16, //+z face
				20, 21, 22, 22, 23, 20 // -x face
			};

			//!!!!right?

			faces = new Face[]
			{
				//-x face
				new Face{ Triangles = new []{
					new FaceVertex(1, 20),
					new FaceVertex(0, 21),
					new FaceVertex(4, 22),
					new FaceVertex(4, 22),
					new FaceVertex(7, 23),
					new FaceVertex(1, 20) } },

				//+x face
				new Face{ Triangles = new []{
					new FaceVertex(3, 12),
					new FaceVertex(2, 13),
					new FaceVertex(6, 14),
					new FaceVertex(6, 14),
					new FaceVertex(5, 15),
					new FaceVertex(3, 12) } },

				//-y face
				new Face{ Triangles = new []{
					new FaceVertex(4, 4),
					new FaceVertex(5, 5),
					new FaceVertex(6, 6),
					new FaceVertex(6, 6),
					new FaceVertex(7, 7),
					new FaceVertex(4, 4) } },

				//+y face
				new Face{ Triangles = new []{
					new FaceVertex(0, 0),
					new FaceVertex(1, 1),
					new FaceVertex(2, 2),
					new FaceVertex(2, 2),
					new FaceVertex(3, 3),
					new FaceVertex(0, 0) } },

				//-z face
				new Face{ Triangles = new []{
					new FaceVertex(0, 8),
					new FaceVertex(3, 9),
					new FaceVertex(5, 10),
					new FaceVertex(5, 10),
					new FaceVertex(4, 11),
					new FaceVertex(0, 8) } },

				//+z face
				new Face{ Triangles = new []{
					new FaceVertex(2, 16),
					new FaceVertex(1, 17),
					new FaceVertex(7, 18),
					new FaceVertex(7, 18),
					new FaceVertex(6, 19),
					new FaceVertex(2, 16) } },
			};

			//structure.Vertices = new[]
			//{
			//	CreateVertex(0, 8,  21), //-x,+y,-z
			//	CreateVertex(1, 17, 20), //-x,+y,+z
			//	CreateVertex(2, 13, 16), //+x,+y,+z
			//	CreateVertex(3, 9,  12), //+x,+y,-z

			//	CreateVertex(4, 11, 22), //-x,-y,-z
			//	CreateVertex(5, 10, 15), //+x,-y,-z
			//	CreateVertex(6, 14, 19), //+x,-y,+z
			//	CreateVertex(7, 18, 23), //-x,-y,+z
			//};

			//structure.Edges = new Edge[]
			//{
			//	new Edge(0,1), new Edge(1,2), new Edge(2,3), new Edge(3,0), //from +y face 
			//	new Edge(4,5), new Edge(5,6), new Edge(6,7), new Edge(7,4), //from -y face 
			//	new Edge(3,5), new Edge(4,0), new Edge(2,6), new Edge(1,7),
			//};

			//structure.Faces = new Face[]
			//{
			//	new Face{ Triangles = new []{0, 1, 2, 2, 3, 0}},  //+y face 
			//	new Face{ Triangles = new []{4, 5, 6, 6, 7, 4,}}, // -y face
			//	new Face{ Triangles = new []{0, 3, 5, 5, 4, 0,}},  // -z face
			//	new Face{ Triangles = new []{3, 2, 6, 6, 5, 3,}}, //+x face
			//	new Face{ Triangles = new []{2, 1, 7, 7, 6, 2,}}, //+z face
			//	new Face{ Triangles = new []{1, 0, 4, 4, 7, 1}}, // -x face
			//};
		}

		public static void GenerateBox( Vector3 size, out Vector3F[] positions, out int[] indices )
		{
			Vector3[] positionsD;
			GenerateBox( size, out positionsD, out indices );
			positions = ToVector3F( positionsD );
		}

		public static void GenerateBox( Vector3 size, bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			//!!!!slowly? везде так

			GenerateBox( size, insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
