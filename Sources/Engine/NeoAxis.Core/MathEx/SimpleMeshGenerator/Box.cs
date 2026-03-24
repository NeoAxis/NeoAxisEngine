// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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

		public static void GenerateBox( Bounds bounds, out Vector3[] positions, out int[] indices )
		{
			positions = new Vector3[ 8 ];
			positions[ 0 ] = new Vector3( bounds.Maximum.X, bounds.Minimum.Y, bounds.Minimum.Z );
			positions[ 1 ] = new Vector3( bounds.Maximum.X, bounds.Minimum.Y, bounds.Maximum.Z );
			positions[ 2 ] = new Vector3( bounds.Maximum.X, bounds.Maximum.Y, bounds.Minimum.Z );
			positions[ 3 ] = new Vector3( bounds.Maximum.X, bounds.Maximum.Y, bounds.Maximum.Z );
			positions[ 4 ] = new Vector3( bounds.Minimum.X, bounds.Minimum.Y, bounds.Minimum.Z );
			positions[ 5 ] = new Vector3( bounds.Minimum.X, bounds.Minimum.Y, bounds.Maximum.Z );
			positions[ 6 ] = new Vector3( bounds.Minimum.X, bounds.Maximum.Y, bounds.Minimum.Z );
			positions[ 7 ] = new Vector3( bounds.Minimum.X, bounds.Maximum.Y, bounds.Maximum.Z );

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

		public static void GenerateBox( BoundsF bounds, out Vector3F[] positions, out int[] indices )
		{
			positions = new Vector3F[ 8 ];
			positions[ 0 ] = new Vector3F( bounds.Maximum.X, bounds.Minimum.Y, bounds.Minimum.Z );
			positions[ 1 ] = new Vector3F( bounds.Maximum.X, bounds.Minimum.Y, bounds.Maximum.Z );
			positions[ 2 ] = new Vector3F( bounds.Maximum.X, bounds.Maximum.Y, bounds.Minimum.Z );
			positions[ 3 ] = new Vector3F( bounds.Maximum.X, bounds.Maximum.Y, bounds.Maximum.Z );
			positions[ 4 ] = new Vector3F( bounds.Minimum.X, bounds.Minimum.Y, bounds.Minimum.Z );
			positions[ 5 ] = new Vector3F( bounds.Minimum.X, bounds.Minimum.Y, bounds.Maximum.Z );
			positions[ 6 ] = new Vector3F( bounds.Minimum.X, bounds.Maximum.Y, bounds.Minimum.Z );
			positions[ 7 ] = new Vector3F( bounds.Minimum.X, bounds.Maximum.Y, bounds.Maximum.Z );

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

		//roundingRadiuses are always 0 or more.
		//roundingSegments are always 2 or more.
		//when roundingRadiuses are zero, need make sharp corners, not smooth ones. so need special case for zero radiuses.
		public static void GenerateRoundingBox( Vector3 size, Vector3 roundingRadiuses, int roundingSegments, bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			var half = size * 0.5;

			// Clamp radiuses.
			var r = new Vector3(
				Math.Max( 0.0, Math.Min( roundingRadiuses.X, Math.Abs( half.X ) ) ),
				Math.Max( 0.0, Math.Min( roundingRadiuses.Y, Math.Abs( half.Y ) ) ),
				Math.Max( 0.0, Math.Min( roundingRadiuses.Z, Math.Abs( half.Z ) ) ) );

			var countZeroRadiuses = 0;
			if( r.X <= 1e-6 )
				countZeroRadiuses++;
			if( r.Y <= 1e-6 )
				countZeroRadiuses++;
			if( r.Z <= 1e-6 )
				countZeroRadiuses++;

			// If no rounding requested -> use regular box generator.
			if( countZeroRadiuses >= 2 )
			{
				GenerateBox( size, insideOut, out positions, out normals, out tangents, out texCoords, out indices, out faces );
				return;
			}

			//if( countZeroRadiuses == 1 )
			//{
			//	GenerateRoundingBoxWhenOneRoundingSegmentIsZero( size, roundingRadiuses, roundingSegments, insideOut, out positions, out normals, out tangents, out texCoords, out indices, out faces );
			//	return;
			//}

			// Avoid division by zero
			var epsilon = 1e-10;
			if( r.X < epsilon ) r.X = epsilon;
			if( r.Y < epsilon ) r.Y = epsilon;
			if( r.Z < epsilon ) r.Z = epsilon;

			var posList = new List<Vector3>( 4096 );
			var nrmList = new List<Vector3>( 4096 );
			var tanList = new List<Vector4>( 4096 );
			var uvList = new List<Vector2>( 4096 );
			var idxList = new List<int>( 8192 );

			static int SideIndexFromNormal( Vector3 n )
			{
				var ax = Math.Abs( n.X );
				var ay = Math.Abs( n.Y );
				var az = Math.Abs( n.Z );
				if( ax >= ay && ax >= az ) return n.X >= 0 ? 1 : 0;
				if( ay >= ax && ay >= az ) return n.Y >= 0 ? 3 : 2;
				return n.Z >= 0 ? 5 : 4;
			}

			static Vector2 PlanarUV( Vector3 p, int side, Vector3 halfLocal )
			{
				switch( side )
				{
				case 0:
				case 1:
					return new Vector2(
						( p.Z + halfLocal.Z ) / ( halfLocal.Z * 2.0 ),
						1.0 - ( p.Y + halfLocal.Y ) / ( halfLocal.Y * 2.0 ) );
				case 2:
				case 3:
					return new Vector2(
						( p.X + halfLocal.X ) / ( halfLocal.X * 2.0 ),
						( p.Z + halfLocal.Z ) / ( halfLocal.Z * 2.0 ) );
				case 4:
				case 5:
					return new Vector2(
						( p.X + halfLocal.X ) / ( halfLocal.X * 2.0 ),
						1.0 - ( p.Y + halfLocal.Y ) / ( halfLocal.Y * 2.0 ) );
				default:
					return Vector2.Zero;
				}
			}

			static Vector3 SafeNormalize( Vector3 v )
			{
				var len = v.Length();
				return len > 1e-12 ? v / len : new Vector3( 0, 1, 0 );
			}

			int AddVertex( Vector3 p, Vector3 n, Vector3 t3, Vector2 uv )
			{
				var vi = posList.Count;
				posList.Add( p );
				nrmList.Add( n );
				tanList.Add( new Vector4( t3, -1 ) );
				uvList.Add( uv );
				return vi;
			}

			void AddTri( int a, int b, int c )
			{
				idxList.Add( a );
				idxList.Add( b );
				idxList.Add( c );
			}

			void AddQuadAutoWinding( int v00, int v10, int v11, int v01 )
			{
				var p00 = posList[ v00 ];
				var p10 = posList[ v10 ];
				var p01 = posList[ v01 ];

				var nDesired = SafeNormalize( nrmList[ v00 ] + nrmList[ v10 ] + nrmList[ v11 ] + nrmList[ v01 ] );
				var nGeom = SafeNormalize( Vector3.Cross( p10 - p00, p01 - p00 ) );

				if( Vector3.Dot( nGeom, nDesired ) >= 0 )
				{
					AddTri( v00, v10, v11 );
					AddTri( v11, v01, v00 );
				}
				else
				{
					AddTri( v00, v01, v11 );
					AddTri( v11, v10, v00 );
				}
			}

			void BuildGrid( int segU, int segV, Func<int, int, (Vector3 p, Vector3 n, Vector3 t, Vector2 uv)> eval )
			{
				var vtx = new int[ segU + 1, segV + 1 ];
				for( int v = 0; v <= segV; v++ )
				{
					for( int u = 0; u <= segU; u++ )
					{
						var d = eval( u, v );
						vtx[ u, v ] = AddVertex( d.p, d.n, d.t, d.uv );
					}
				}

				for( int v = 0; v < segV; v++ )
				{
					for( int u = 0; u < segU; u++ )
					{
						AddQuadAutoWinding( vtx[ u, v ], vtx[ u + 1, v ], vtx[ u + 1, v + 1 ], vtx[ u, v + 1 ] );
					}
				}
			}

			// 1) Inner planar faces
			void BuildInnerFace( int side )
			{
				switch( side )
				{
				case 3: // +y
					{
						var y = half.Y;
						var minX = -half.X + r.X; var maxX = half.X - r.X;
						var minZ = -half.Z + r.Z; var maxZ = half.Z - r.Z;
						if( maxX <= minX || maxZ <= minZ ) return;
						BuildGrid( 1, 1, ( ix, iz ) =>
						{
							var p = new Vector3( ix == 0 ? minX : maxX, y, iz == 0 ? minZ : maxZ );
							return (p, new Vector3( 0, 1, 0 ), new Vector3( -1, 0, 0 ), PlanarUV( p, side, half ));
						} );
					}
					break;
				case 2: // -y
					{
						var y = -half.Y;
						var minX = -half.X + r.X; var maxX = half.X - r.X;
						var minZ = -half.Z + r.Z; var maxZ = half.Z - r.Z;
						if( maxX <= minX || maxZ <= minZ ) return;
						BuildGrid( 1, 1, ( ix, iz ) =>
						{
							var p = new Vector3( ix == 0 ? minX : maxX, y, iz == 0 ? minZ : maxZ );
							return (p, new Vector3( 0, -1, 0 ), new Vector3( 1, 0, 0 ), PlanarUV( p, side, half ));
						} );
					}
					break;
				case 1: // +x
					{
						var x = half.X;
						var minY = -half.Y + r.Y; var maxY = half.Y - r.Y;
						var minZ = -half.Z + r.Z; var maxZ = half.Z - r.Z;
						if( maxY <= minY || maxZ <= minZ ) return;
						BuildGrid( 1, 1, ( iy, iz ) =>
						{
							var p = new Vector3( x, iy == 0 ? minY : maxY, iz == 0 ? minZ : maxZ );
							return (p, new Vector3( 1, 0, 0 ), new Vector3( 0, 1, 0 ), PlanarUV( p, side, half ));
						} );
					}
					break;
				case 0: // -x
					{
						var x = -half.X;
						var minY = -half.Y + r.Y; var maxY = half.Y - r.Y;
						var minZ = -half.Z + r.Z; var maxZ = half.Z - r.Z;
						if( maxY <= minY || maxZ <= minZ ) return;
						BuildGrid( 1, 1, ( iy, iz ) =>
						{
							var p = new Vector3( x, iy == 0 ? minY : maxY, iz == 0 ? minZ : maxZ );
							return (p, new Vector3( -1, 0, 0 ), new Vector3( 0, -1, 0 ), PlanarUV( p, side, half ));
						} );
					}
					break;
				case 5: // +z
					{
						var z = half.Z;
						var minX = -half.X + r.X; var maxX = half.X - r.X;
						var minY = -half.Y + r.Y; var maxY = half.Y - r.Y;
						if( maxX <= minX || maxY <= minY ) return;
						BuildGrid( 1, 1, ( ix, iy ) =>
						{
							var p = new Vector3( ix == 0 ? minX : maxX, iy == 0 ? minY : maxY, z );
							return (p, new Vector3( 0, 0, 1 ), new Vector3( 1, 0, 0 ), PlanarUV( p, side, half ));
						} );
					}
					break;
				case 4: // -z
					{
						var z = -half.Z;
						var minX = -half.X + r.X; var maxX = half.X - r.X;
						var minY = -half.Y + r.Y; var maxY = half.Y - r.Y;
						if( maxX <= minX || maxY <= minY ) return;
						BuildGrid( 1, 1, ( ix, iy ) =>
						{
							var p = new Vector3( ix == 0 ? minX : maxX, iy == 0 ? minY : maxY, z );
							return (p, new Vector3( 0, 0, -1 ), new Vector3( 1, 0, 0 ), PlanarUV( p, side, half ));
						} );
					}
					break;
				}
			}

			for( int side = 0; side < 6; side++ )
				BuildInnerFace( side );

			// 2) Edge patches
			void BuildEdgeAlongX( int sy, int sz )
			{
				var y0 = sy * ( half.Y - r.Y );
				var z0 = sz * ( half.Z - r.Z );
				var minX = -half.X + r.X; var maxX = half.X - r.X;

				BuildGrid( roundingSegments, roundingSegments, ( iu, iv ) =>
				{
					var tx = (double)iu / roundingSegments;
					var a = (double)iv / roundingSegments * ( Math.PI * 0.5 );
					var x = minX + ( maxX - minX ) * tx;
					var y = y0 + sy * r.Y * Math.Cos( a );
					var z = z0 + sz * r.Z * Math.Sin( a );

					var ny = (float)( sy * Math.Cos( a ) / r.Y );
					var nz = (float)( sz * Math.Sin( a ) / r.Z );
					var n = SafeNormalize( new Vector3( 0, ny, nz ) );
					return (new Vector3( x, y, z ), n, new Vector3( 1, 0, 0 ), PlanarUV( new Vector3( x, y, z ), SideIndexFromNormal( n ), half ));
				} );
			}

			void BuildEdgeAlongY( int sx, int sz )
			{
				var x0 = sx * ( half.X - r.X );
				var z0 = sz * ( half.Z - r.Z );
				var minY = -half.Y + r.Y; var maxY = half.Y - r.Y;

				BuildGrid( roundingSegments, roundingSegments, ( iu, iv ) =>
				{
					var ty = (double)iu / roundingSegments;
					var a = (double)iv / roundingSegments * ( Math.PI * 0.5 );
					var y = minY + ( maxY - minY ) * ty;
					var x = x0 + sx * r.X * Math.Cos( a );
					var z = z0 + sz * r.Z * Math.Sin( a );

					var nx = (float)( sx * Math.Cos( a ) / r.X );
					var nz = (float)( sz * Math.Sin( a ) / r.Z );
					var n = SafeNormalize( new Vector3( nx, 0, nz ) );
					return (new Vector3( x, y, z ), n, new Vector3( 0, 1, 0 ), PlanarUV( new Vector3( x, y, z ), SideIndexFromNormal( n ), half ));
				} );
			}

			void BuildEdgeAlongZ( int sx, int sy )
			{
				var x0 = sx * ( half.X - r.X );
				var y0 = sy * ( half.Y - r.Y );
				var minZ = -half.Z + r.Z; var maxZ = half.Z - r.Z;

				BuildGrid( roundingSegments, roundingSegments, ( iu, iv ) =>
				{
					var tz = (double)iu / roundingSegments;
					var a = (double)iv / roundingSegments * ( Math.PI * 0.5 );
					var z = minZ + ( maxZ - minZ ) * tz;
					var x = x0 + sx * r.X * Math.Cos( a );
					var y = y0 + sy * r.Y * Math.Sin( a );

					var nx = (float)( sx * Math.Cos( a ) / r.X );
					var ny = (float)( sy * Math.Sin( a ) / r.Y );
					var n = SafeNormalize( new Vector3( nx, ny, 0 ) );
					return (new Vector3( x, y, z ), n, new Vector3( 0, 0, 1 ), PlanarUV( new Vector3( x, y, z ), SideIndexFromNormal( n ), half ));
				} );
			}

			for( int sy = -1; sy <= 1; sy += 2 ) for( int sz = -1; sz <= 1; sz += 2 ) BuildEdgeAlongX( sy, sz );
			for( int sx = -1; sx <= 1; sx += 2 ) for( int sz = -1; sz <= 1; sz += 2 ) BuildEdgeAlongY( sx, sz );
			for( int sx = -1; sx <= 1; sx += 2 ) for( int sy = -1; sy <= 1; sy += 2 ) BuildEdgeAlongZ( sx, sy );

			// 3) Corner patches
			Vector3 CornerCenter( int sx, int sy, int sz ) => new Vector3( sx * ( half.X - r.X ), sy * ( half.Y - r.Y ), sz * ( half.Z - r.Z ) );

			for( int sx = -1; sx <= 1; sx += 2 )
			{
				for( int sy = -1; sy <= 1; sy += 2 )
				{
					for( int sz = -1; sz <= 1; sz += 2 )
					{
						BuildGrid( roundingSegments, roundingSegments, ( iu, iv ) =>
						{
							var u01 = (double)iu / roundingSegments;
							var v01 = (double)iv / roundingSegments;
							var a = u01 * ( Math.PI * 0.5 );
							var b = v01 * ( Math.PI * 0.5 );
							var dx = Math.Cos( a ) * Math.Cos( b );
							var dy = Math.Sin( a ) * Math.Cos( b );
							var dz = Math.Sin( b );

							var local = new Vector3( sx * r.X * dx, sy * r.Y * dy, sz * r.Z * dz );
							var n = SafeNormalize( new Vector3( sx * dx / r.X, sy * dy / r.Y, sz * dz / r.Z ) );

							var t = SafeNormalize( new Vector3( sx * -Math.Sin( a ) * Math.Cos( b ) * r.X, sy * Math.Cos( a ) * Math.Cos( b ) * r.Y, 0 ) );
							if( Math.Abs( Vector3.Dot( t, n ) ) > 0.99 )
								t = SafeNormalize( Vector3.Cross( Math.Abs( n.Y ) < 0.9 ? new Vector3( 0, 1, 0 ) : new Vector3( 1, 0, 0 ), n ) );

							var p = CornerCenter( sx, sy, sz ) + local;
							return (p, n, t, PlanarUV( p, SideIndexFromNormal( n ), half ));
						} );
					}
				}
			}

			if( insideOut )
			{
				for( int i = 0; i < nrmList.Count; i++ ) nrmList[ i ] = -nrmList[ i ];
				for( int i = 0; i < idxList.Count; i += 3 ) (idxList[ i + 1 ], idxList[ i + 2 ]) = (idxList[ i + 2 ], idxList[ i + 1 ]);
				for( int i = 0; i < uvList.Count; i++ ) uvList[ i ] = new Vector2( 1.0f - uvList[ i ].X, 1.0f - uvList[ i ].Y );
			}

			positions = posList.ToArray();
			normals = nrmList.ToArray();
			tangents = tanList.ToArray();
			texCoords = uvList.ToArray();
			indices = idxList.ToArray();

			var rebuilt = new List<FaceVertex>[ 6 ];
			for( int i = 0; i < 6; i++ ) rebuilt[ i ] = new List<FaceVertex>();

			for( int i = 0; i < indices.Length; i += 3 )
			{
				var ia = indices[ i ]; var ib = indices[ i + 1 ]; var ic = indices[ i + 2 ];
				var nAvg = SafeNormalize( normals[ ia ] + normals[ ib ] + normals[ ic ] );
				var side = SideIndexFromNormal( nAvg );
				rebuilt[ side ].Add( new FaceVertex( ia, ia ) );
				rebuilt[ side ].Add( new FaceVertex( ib, ib ) );
				rebuilt[ side ].Add( new FaceVertex( ic, ic ) );
			}

			faces = new Face[ 6 ];
			for( int i = 0; i < 6; i++ ) faces[ i ] = new Face { Triangles = rebuilt[ i ].ToArray() };
		}


		//public static void GenerateRoundingBox( Vector3 size, Vector3 roundingRadiuses, Vector3I roundingSegments, bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		//{
		//	//roundingSegments must be equal or works wrong

		//	//roundingRadiuses are always 0 or more.
		//	//roundingSegments are always 2 or more.

		//	// Plan (pseudocode):
		//	// 1) Clamp radiuses to [0, halfSize] per axis.
		//	// 2) If all radiuses are zero -> fallback to GenerateBox(size, insideOut, ...).
		//	// 3) Build surface patches (inner faces, edges, corners) emitting vertices and quads.
		//	// 4) For every quad, enforce consistent outward winding by comparing geometric normal to averaged per-vertex normal.
		//	//    - This fixes sporadic inverted triangles caused by parameterization direction differences on some patches.
		//	// 5) If insideOut: invert normals, flip triangle winding, mirror texcoords.
		//	// 6) Build faces[] by dominant triangle normal.

		//	var half = size * 0.5;

		//	// Clamp radiuses.
		//	var r = new Vector3(
		//		Math.Max( 0.0, Math.Min( roundingRadiuses.X, Math.Abs( half.X ) ) ),
		//		Math.Max( 0.0, Math.Min( roundingRadiuses.Y, Math.Abs( half.Y ) ) ),
		//		Math.Max( 0.0, Math.Min( roundingRadiuses.Z, Math.Abs( half.Z ) ) ) );

		//	// If no rounding requested -> use regular box generator.
		//	if( r.X <= 0 && r.Y <= 0 && r.Z <= 0 )
		//	{
		//		GenerateBox( size, insideOut, out positions, out normals, out tangents, out texCoords, out indices, out faces );
		//		return;
		//	}

		//	//!!!!bug without it
		//	var epsilon = 0.0000000001;
		//	if( r.X < epsilon )
		//		r.X = epsilon;
		//	if( r.Y < epsilon )
		//		r.Y = epsilon;
		//	if( r.Z < epsilon )
		//		r.Z = epsilon;

		//	// Edge arc segments derived from involved axes.
		//	int EdgeArcSegYZ() => Math.Max( roundingSegments.Y, roundingSegments.Z );
		//	int EdgeArcSegXZ() => Math.Max( roundingSegments.X, roundingSegments.Z );
		//	int EdgeArcSegXY() => Math.Max( roundingSegments.X, roundingSegments.Y );

		//	var posList = new List<Vector3>( 4096 );
		//	var nrmList = new List<Vector3>( 4096 );
		//	var tanList = new List<Vector4>( 4096 );
		//	var uvList = new List<Vector2>( 4096 );
		//	var idxList = new List<int>( 8192 );

		//	static int SideIndexFromNormal( Vector3 n )
		//	{
		//		var ax = Math.Abs( n.X );
		//		var ay = Math.Abs( n.Y );
		//		var az = Math.Abs( n.Z );

		//		if( ax >= ay && ax >= az )
		//			return n.X >= 0 ? 1 : 0;
		//		if( ay >= ax && ay >= az )
		//			return n.Y >= 0 ? 3 : 2;
		//		return n.Z >= 0 ? 5 : 4;
		//	}

		//	static Vector2 PlanarUV( Vector3 p, int side, Vector3 halfLocal )
		//	{
		//		switch( side )
		//		{
		//		case 0:
		//		case 1:
		//			return new Vector2(
		//				( p.Z + halfLocal.Z ) / ( halfLocal.Z * 2.0 ),
		//				1.0 - ( p.Y + halfLocal.Y ) / ( halfLocal.Y * 2.0 ) );
		//		case 2:
		//		case 3:
		//			return new Vector2(
		//				( p.X + halfLocal.X ) / ( halfLocal.X * 2.0 ),
		//				( p.Z + halfLocal.Z ) / ( halfLocal.Z * 2.0 ) );
		//		case 4:
		//		case 5:
		//			return new Vector2(
		//				( p.X + halfLocal.X ) / ( halfLocal.X * 2.0 ),
		//				1.0 - ( p.Y + halfLocal.Y ) / ( halfLocal.Y * 2.0 ) );
		//		default:
		//			return Vector2.Zero;
		//		}
		//	}

		//	static Vector3 SafeNormalize( Vector3 v )
		//	{
		//		var len = v.Length();
		//		if( len > 1e-12 )
		//			return v / len;
		//		return new Vector3( 0, 1, 0 );
		//	}

		//	int AddVertex( Vector3 p, Vector3 n, Vector3 t3, Vector2 uv )
		//	{
		//		var vi = posList.Count;
		//		posList.Add( p );
		//		nrmList.Add( n );
		//		tanList.Add( new Vector4( t3, -1 ) );
		//		uvList.Add( uv );
		//		return vi;
		//	}

		//	void AddTri( int a, int b, int c )
		//	{
		//		idxList.Add( a );
		//		idxList.Add( b );
		//		idxList.Add( c );
		//	}

		//	// Enforce consistent outward winding for every quad based on geometry vs desired normal.
		//	void AddQuadAutoWinding( int v00, int v10, int v11, int v01 )
		//	{
		//		var p00 = posList[ v00 ];
		//		var p10 = posList[ v10 ];
		//		var p01 = posList[ v01 ];

		//		var nDesired = SafeNormalize( nrmList[ v00 ] + nrmList[ v10 ] + nrmList[ v11 ] + nrmList[ v01 ] );

		//		// Geometric normal for triangle (00,10,01) with assumed winding (00->10->01).
		//		var nGeom = SafeNormalize( Vector3.Cross( p10 - p00, p01 - p00 ) );

		//		// If geometric normal points opposite to desired, swap winding.
		//		var flip = Vector3.Dot( nGeom, nDesired ) < 0;

		//		if( !flip )
		//		{
		//			AddTri( v00, v10, v11 );
		//			AddTri( v11, v01, v00 );
		//		}
		//		else
		//		{
		//			AddTri( v00, v01, v11 );
		//			AddTri( v11, v10, v00 );
		//		}
		//	}

		//	void BuildGrid( int segU, int segV, Func<int, int, (Vector3 p, Vector3 n, Vector3 t, Vector2 uv)> eval )
		//	{
		//		var vtx = new int[ segU + 1, segV + 1 ];
		//		for( int v = 0; v <= segV; v++ )
		//		{
		//			for( int u = 0; u <= segU; u++ )
		//			{
		//				var d = eval( u, v );
		//				vtx[ u, v ] = AddVertex( d.p, d.n, d.t, d.uv );
		//			}
		//		}

		//		for( int v = 0; v < segV; v++ )
		//		{
		//			for( int u = 0; u < segU; u++ )
		//			{
		//				var v00 = vtx[ u, v ];
		//				var v10 = vtx[ u + 1, v ];
		//				var v11 = vtx[ u + 1, v + 1 ];
		//				var v01 = vtx[ u, v + 1 ];
		//				AddQuadAutoWinding( v00, v10, v11, v01 );
		//			}
		//		}
		//	}

		//	// 1) Inner planar faces (shrunk rectangles).
		//	void BuildInnerFace( int side )
		//	{
		//		//if( half.X == 0 || half.Y == 0 || half.Z == 0 )
		//		//	return;

		//		switch( side )
		//		{
		//		case 3: // +y
		//			{
		//				var y = half.Y;
		//				var minX = -half.X + r.X;
		//				var maxX = half.X - r.X;
		//				var minZ = -half.Z + r.Z;
		//				var maxZ = half.Z - r.Z;
		//				if( maxX <= minX || maxZ <= minZ )
		//					return;

		//				BuildGrid( 1, 1, ( ix, iz ) =>
		//				{
		//					var x = ix == 0 ? minX : maxX;
		//					var z = iz == 0 ? minZ : maxZ;
		//					var p = new Vector3( x, y, z );
		//					var n = new Vector3( 0, 1, 0 );
		//					var t = new Vector3( -1, 0, 0 );
		//					var uv = PlanarUV( p, side, half );
		//					return (p, n, t, uv);
		//				} );
		//			}
		//			break;

		//		case 2: // -y
		//			{
		//				var y = -half.Y;
		//				var minX = -half.X + r.X;
		//				var maxX = half.X - r.X;
		//				var minZ = -half.Z + r.Z;
		//				var maxZ = half.Z - r.Z;
		//				if( maxX <= minX || maxZ <= minZ )
		//					return;

		//				BuildGrid( 1, 1, ( ix, iz ) =>
		//				{
		//					var x = ix == 0 ? minX : maxX;
		//					var z = iz == 0 ? minZ : maxZ;
		//					var p = new Vector3( x, y, z );
		//					var n = new Vector3( 0, -1, 0 );
		//					var t = new Vector3( 1, 0, 0 );
		//					var uv = PlanarUV( p, side, half );
		//					return (p, n, t, uv);
		//				} );
		//			}
		//			break;

		//		case 1: // +x
		//			{
		//				var x = half.X;
		//				var minY = -half.Y + r.Y;
		//				var maxY = half.Y - r.Y;
		//				var minZ = -half.Z + r.Z;
		//				var maxZ = half.Z - r.Z;
		//				if( maxY <= minY || maxZ <= minZ )
		//					return;

		//				BuildGrid( 1, 1, ( iy, iz ) =>
		//				{
		//					var y = iy == 0 ? minY : maxY;
		//					var z = iz == 0 ? minZ : maxZ;
		//					var p = new Vector3( x, y, z );
		//					var n = new Vector3( 1, 0, 0 );
		//					var t = new Vector3( 0, 1, 0 );
		//					var uv = PlanarUV( p, side, half );
		//					return (p, n, t, uv);
		//				} );
		//			}
		//			break;

		//		case 0: // -x
		//			{
		//				var x = -half.X;
		//				var minY = -half.Y + r.Y;
		//				var maxY = half.Y - r.Y;
		//				var minZ = -half.Z + r.Z;
		//				var maxZ = half.Z - r.Z;
		//				if( maxY <= minY || maxZ <= minZ )
		//					return;

		//				BuildGrid( 1, 1, ( iy, iz ) =>
		//				{
		//					var y = iy == 0 ? minY : maxY;
		//					var z = iz == 0 ? minZ : maxZ;
		//					var p = new Vector3( x, y, z );
		//					var n = new Vector3( -1, 0, 0 );
		//					var t = new Vector3( 0, -1, 0 );
		//					var uv = PlanarUV( p, side, half );
		//					return (p, n, t, uv);
		//				} );
		//			}
		//			break;

		//		case 5: // +z
		//			{
		//				var z = half.Z;
		//				var minX = -half.X + r.X;
		//				var maxX = half.X - r.X;
		//				var minY = -half.Y + r.Y;
		//				var maxY = half.Y - r.Y;
		//				if( maxX <= minX || maxY <= minY )
		//					return;

		//				BuildGrid( 1, 1, ( ix, iy ) =>
		//				{
		//					var x = ix == 0 ? minX : maxX;
		//					var y = iy == 0 ? minY : maxY;
		//					var p = new Vector3( x, y, z );
		//					var n = new Vector3( 0, 0, 1 );
		//					var t = new Vector3( 1, 0, 0 );
		//					var uv = PlanarUV( p, side, half );
		//					return (p, n, t, uv);
		//				} );
		//			}
		//			break;

		//		case 4: // -z
		//			{
		//				var z = -half.Z;
		//				var minX = -half.X + r.X;
		//				var maxX = half.X - r.X;
		//				var minY = -half.Y + r.Y;
		//				var maxY = half.Y - r.Y;
		//				if( maxX <= minX || maxY <= minY )
		//					return;

		//				BuildGrid( 1, 1, ( ix, iy ) =>
		//				{
		//					var x = ix == 0 ? minX : maxX;
		//					var y = iy == 0 ? minY : maxY;
		//					var p = new Vector3( x, y, z );
		//					var n = new Vector3( 0, 0, -1 );
		//					var t = new Vector3( 1, 0, 0 );
		//					var uv = PlanarUV( p, side, half );
		//					return (p, n, t, uv);
		//				} );
		//			}
		//			break;
		//		}
		//	}

		//	for( int side = 0; side < 6; side++ )
		//		BuildInnerFace( side );

		//	// 2) Edge patches
		//	void BuildEdgeAlongX( int sy, int sz )
		//	{
		//		//if( r.Y <= 0 || r.Z <= 0 )
		//		//	return;

		//		var arcSeg = EdgeArcSegYZ();

		//		var y0 = sy * ( half.Y - r.Y );
		//		var z0 = sz * ( half.Z - r.Z );

		//		var minX = -half.X + r.X;
		//		var maxX = half.X - r.X;
		//		//var minX = -half.X + ( r.X > 0 ? r.X : 0 );
		//		//var maxX = half.X - ( r.X > 0 ? r.X : 0 );
		//		//if( r.X <= 0 )
		//		//{
		//		//	minX = -half.X;
		//		//	maxX = half.X;
		//		//}
		//		//if( maxX <= minX )
		//		//	return;

		//		BuildGrid( roundingSegments.X, arcSeg, ( iu, iv ) =>
		//		{
		//			var tx = (double)iu / roundingSegments.X;
		//			var a = (double)iv / arcSeg * ( Math.PI * 0.5 );

		//			var x = minX + ( maxX - minX ) * tx;
		//			var y = y0 + sy * r.Y * Math.Cos( a );
		//			var z = z0 + sz * r.Z * Math.Sin( a );
		//			var p = new Vector3( x, y, z );

		//			var ny = (float)( sy * Math.Cos( a ) / Math.Max( r.Y, 1e-9 ) );
		//			var nz = (float)( sz * Math.Sin( a ) / Math.Max( r.Z, 1e-9 ) );
		//			var n = SafeNormalize( new Vector3( 0, ny, nz ) );

		//			var t = new Vector3( 1, 0, 0 );

		//			var side = SideIndexFromNormal( n );
		//			var uv = PlanarUV( p, side, half );
		//			return (p, n, t, uv);
		//		} );
		//	}

		//	void BuildEdgeAlongY( int sx, int sz )
		//	{
		//		//if( r.X <= 0 || r.Z <= 0 )
		//		//	return;

		//		var arcSeg = EdgeArcSegXZ();

		//		var x0 = sx * ( half.X - r.X );
		//		var z0 = sz * ( half.Z - r.Z );

		//		var minY = -half.Y + r.Y;
		//		var maxY = half.Y - r.Y;
		//		//var minY = -half.Y + ( r.Y > 0 ? r.Y : 0 );
		//		//var maxY = half.Y - ( r.Y > 0 ? r.Y : 0 );
		//		//if( r.Y <= 0 )
		//		//{
		//		//	minY = -half.Y;
		//		//	maxY = half.Y;
		//		//}
		//		//if( maxY <= minY )
		//		//	return;

		//		BuildGrid( roundingSegments.Y, arcSeg, ( iu, iv ) =>
		//		{
		//			var ty = (double)iu / roundingSegments.Y;
		//			var a = (double)iv / arcSeg * ( Math.PI * 0.5 );

		//			var y = minY + ( maxY - minY ) * ty;
		//			var x = x0 + sx * r.X * Math.Cos( a );
		//			var z = z0 + sz * r.Z * Math.Sin( a );
		//			var p = new Vector3( x, y, z );

		//			var nx = (float)( sx * Math.Cos( a ) / Math.Max( r.X, 1e-9 ) );
		//			var nz = (float)( sz * Math.Sin( a ) / Math.Max( r.Z, 1e-9 ) );
		//			var n = SafeNormalize( new Vector3( nx, 0, nz ) );

		//			var t = new Vector3( 0, 1, 0 );

		//			var side = SideIndexFromNormal( n );
		//			var uv = PlanarUV( p, side, half );
		//			return (p, n, t, uv);
		//		} );
		//	}

		//	void BuildEdgeAlongZ( int sx, int sy )
		//	{
		//		//if( r.X <= 0 || r.Y <= 0 )
		//		//	return;

		//		var arcSeg = EdgeArcSegXY();

		//		var x0 = sx * ( half.X - r.X );
		//		var y0 = sy * ( half.Y - r.Y );

		//		var minZ = -half.Z + r.Z;
		//		var maxZ = half.Z - r.Z;
		//		//var minZ = -half.Z + ( r.Z > 0 ? r.Z : 0 );
		//		//var maxZ = half.Z - ( r.Z > 0 ? r.Z : 0 );
		//		//if( r.Z <= 0 )
		//		//{
		//		//	minZ = -half.Z;
		//		//	maxZ = half.Z;
		//		//}
		//		//if( maxZ <= minZ )
		//		//	return;

		//		BuildGrid( roundingSegments.Z, arcSeg, ( iu, iv ) =>
		//		{
		//			var tz = (double)iu / roundingSegments.Z;
		//			var a = (double)iv / arcSeg * ( Math.PI * 0.5 );

		//			var z = minZ + ( maxZ - minZ ) * tz;
		//			var x = x0 + sx * r.X * Math.Cos( a );
		//			var y = y0 + sy * r.Y * Math.Sin( a );
		//			var p = new Vector3( x, y, z );

		//			var nx = (float)( sx * Math.Cos( a ) / Math.Max( r.X, 1e-9 ) );
		//			var ny = (float)( sy * Math.Sin( a ) / Math.Max( r.Y, 1e-9 ) );
		//			var n = SafeNormalize( new Vector3( nx, ny, 0 ) );

		//			var t = new Vector3( 0, 0, 1 );

		//			var side = SideIndexFromNormal( n );
		//			var uv = PlanarUV( p, side, half );
		//			return (p, n, t, uv);
		//		} );
		//	}

		//	for( int sy = -1; sy <= 1; sy += 2 )
		//		for( int sz = -1; sz <= 1; sz += 2 )
		//			BuildEdgeAlongX( sy, sz );

		//	for( int sx = -1; sx <= 1; sx += 2 )
		//		for( int sz = -1; sz <= 1; sz += 2 )
		//			BuildEdgeAlongY( sx, sz );

		//	for( int sx = -1; sx <= 1; sx += 2 )
		//		for( int sy = -1; sy <= 1; sy += 2 )
		//			BuildEdgeAlongZ( sx, sy );

		//	// 3) Corner patches (8 corners).
		//	Vector3 CornerCenter( int sx, int sy, int sz )
		//	{
		//		return new Vector3(
		//			sx * ( half.X - r.X ),
		//			sy * ( half.Y - r.Y ),
		//			sz * ( half.Z - r.Z ) );
		//	}

		//	(Vector3 p, Vector3 n, Vector3 t) EvalCorner( int sx, int sy, int sz, double u01, double v01 )
		//	{
		//		var a = u01 * ( Math.PI * 0.5 );
		//		var b = v01 * ( Math.PI * 0.5 );

		//		var dx = Math.Cos( a ) * Math.Cos( b );
		//		var dy = Math.Sin( a ) * Math.Cos( b );
		//		var dz = Math.Sin( b );

		//		var rx = r.X;
		//		var ry = r.Y;
		//		var rz = r.Z;

		//		var local = new Vector3(
		//			sx * rx * dx,
		//			sy * ry * dy,
		//			sz * rz * dz );

		//		var center = CornerCenter( sx, sy, sz );
		//		var p = center + local;

		//		Vector3 grad = Vector3.Zero;
		//		if( rx > 0 ) grad.X = sx * ( dx / rx );
		//		if( ry > 0 ) grad.Y = sy * ( dy / ry );
		//		if( rz > 0 ) grad.Z = sz * ( dz / rz );

		//		var n = SafeNormalize( grad );

		//		var tDir = new Vector3(
		//			sx * (float)( -Math.Sin( a ) * Math.Cos( b ) * rx ),
		//			sy * (float)( Math.Cos( a ) * Math.Cos( b ) * ry ),
		//			0 );
		//		var t = SafeNormalize( tDir );
		//		if( Math.Abs( Vector3.Dot( t, n ) ) > 0.99 )
		//		{
		//			var axis = Math.Abs( n.Y ) < 0.9 ? new Vector3( 0, 1, 0 ) : new Vector3( 1, 0, 0 );
		//			t = SafeNormalize( Vector3.Cross( axis, n ) );
		//		}

		//		return (p, n, t);
		//	}

		//	//if( r.X > 0 && r.Y > 0 && r.Z > 0 )
		//	{
		//		for( int sx = -1; sx <= 1; sx += 2 )
		//		{
		//			for( int sy = -1; sy <= 1; sy += 2 )
		//			{
		//				for( int sz = -1; sz <= 1; sz += 2 )
		//				{
		//					var segU = Math.Max( roundingSegments.X, roundingSegments.Y );
		//					var segV = roundingSegments.Z;

		//					BuildGrid( segU, segV, ( iu, iv ) =>
		//					{
		//						var u01 = (double)iu / segU;
		//						var v01 = (double)iv / segV;

		//						var (p, n, t) = EvalCorner( sx, sy, sz, u01, v01 );
		//						var side = SideIndexFromNormal( n );
		//						var uv = PlanarUV( p, side, half );
		//						return (p, n, t, uv);
		//					} );
		//				}
		//			}
		//		}
		//	}

		//	if( insideOut )
		//	{
		//		for( int i = 0; i < nrmList.Count; i++ )
		//			nrmList[ i ] = -nrmList[ i ];

		//		for( int i = 0; i < idxList.Count; i += 3 )
		//			(idxList[ i + 1 ], idxList[ i + 2 ]) = (idxList[ i + 2 ], idxList[ i + 1 ]);

		//		for( int i = 0; i < uvList.Count; i++ )
		//			uvList[ i ] = new Vector2( 1.0f - uvList[ i ].X, 1.0f - uvList[ i ].Y );
		//	}

		//	positions = posList.ToArray();
		//	normals = nrmList.ToArray();
		//	tangents = tanList.ToArray();
		//	texCoords = uvList.ToArray();
		//	indices = idxList.ToArray();

		//	// Build faces from indices by dominant normal of triangle
		//	var rebuilt = new List<FaceVertex>[ 6 ];
		//	for( int i = 0; i < 6; i++ )
		//		rebuilt[ i ] = new List<FaceVertex>( indices.Length / 6 );

		//	for( int i = 0; i < indices.Length; i += 3 )
		//	{
		//		var ia = indices[ i ];
		//		var ib = indices[ i + 1 ];
		//		var ic = indices[ i + 2 ];

		//		var nAvg = SafeNormalize( normals[ ia ] + normals[ ib ] + normals[ ic ] );
		//		var side = SideIndexFromNormal( nAvg );

		//		rebuilt[ side ].Add( new FaceVertex( ia, ia ) );
		//		rebuilt[ side ].Add( new FaceVertex( ib, ib ) );
		//		rebuilt[ side ].Add( new FaceVertex( ic, ic ) );
		//	}

		//	faces = new Face[]
		//	{
		//		new Face{ Triangles = rebuilt[ 0 ].ToArray() }, //-x
		//		new Face{ Triangles = rebuilt[ 1 ].ToArray() }, //+x
		//		new Face{ Triangles = rebuilt[ 2 ].ToArray() }, //-y
		//		new Face{ Triangles = rebuilt[ 3 ].ToArray() }, //+y
		//		new Face{ Triangles = rebuilt[ 4 ].ToArray() }, //-z
		//		new Face{ Triangles = rebuilt[ 5 ].ToArray() }, //+z
		//	};
		//}

		public static void GenerateRoundingBox( Vector3 size, Vector3 roundingRadiuses, int roundingSegments, bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GenerateRoundingBox( size, roundingRadiuses, roundingSegments, insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}

		//public static void GenerateRoundingBox( Vector3 size, Vector3 roundingRadiuses, Vector3I roundingSegments, bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		//{
		//	GenerateRoundingBox( size, roundingRadiuses, roundingSegments, insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
		//	positions = ToVector3F( positionsD );
		//	normals = ToVector3F( normalsD );
		//	tangents = ToVector4F( tangentsD );
		//	texCoords = ToVector2F( texCoordsD );
		//}
	}
}
