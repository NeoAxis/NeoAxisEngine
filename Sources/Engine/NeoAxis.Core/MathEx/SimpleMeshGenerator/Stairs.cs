// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

using System;
using System.Threading;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GenerateStairs( int axis, double width, double height, double depth, int steps, Degree curvature, double radius, bool sides, bool insideOut, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			if( width <= 0 || height <= 0 || depth <= 0 || steps < 1 || radius <= 0 )
			{
				GetEmptyData( out positions, out normals, out tangents, out texCoords, out indices, out faces );
				return;
			}

			if( curvature != 0 && radius < width )
				width = radius;

			int nextVertex = 0;
			bool back = sides;

			Vector3
				offsetA = new Vector3( 0, -width / 2, 0 ),
				offsetB = new Vector3( 0, width / 2, 0 );


			//ToDo ??? когда нет изгиба, располагает середину лестницы в начале координат. Но при изгибе, начало лестницы прыгает в начало координат. Если нужно, чтобы начало лестницы было всегда в начале координат то закомментировать.
			//if( curvature == 0 )
			//{
			//	offsetA.Y -= depth / 2;
			//	offsetB.Y -= depth / 2;
			//}

			GenerateStairsPoints( depth, offsetA, curvature, radius, height, steps, ref nextVertex, out Point[] pointsA0, out Point[] pointsA1, out Point[] pointsA2 );
			GenerateStairsPoints( depth, offsetB, curvature, radius - width, height, steps, ref nextVertex, out Point[] pointsB0, out Point[] pointsB1, out Point[] pointsB2 );

			if( curvature < 0 )
				insideOut = !insideOut;

			int rawVertexCount =
				6 * 2 * steps + // stair horizontal + vertical
				( sides ? steps * ( 6 * 2 + 3 * 2 ) - 3 * 2 : 0 ) + //first step side has no triangle
				( back ? 6 : 0 );
			int faceCount = ( 2 + ( sides ? 2 : 0 ) ) * steps + ( back ? 1 : 0 );
			var builder = new CommonPipeBuilder( rawVertexCount, faceCount );
			for( int i = 0; i < steps; i++ )
			{
				Point[][] pp = { pointsA1, pointsA2, pointsB1, pointsB2 };
				builder.CalculateTexCoordForFlatPolygon2( pp, new Vector3( 0, 0, 1 ), new Vector3( 1, 0, 0 ), out Vector2[][] texCoordHoriz, out Vector4 tangentHoriz );

				//ToDo ??? Здесь сомнительно для вертикальных частей ступенек, т.к. плоскости поворачиваются (когда есть rotation, все вертикальные части ступенек в разных плоскостях(может быть перпендикулярна, направлению). Может для каждой части координаты (0,1) ?
				builder.CalculateTexCoordForFlatPolygon2( pp, new Vector3( 0, -1, 0 ), new Vector3( 1, 0, 0 ), out Vector2[][] texCoordVert, out Vector4 tangentVert );

				Vector4 tangentVertical = new Vector4( pointsA1[ i ].Position - pointsB1[ i ].Position, -1 );

				//vertical
				builder.AddQuad(
					pointsA1[ i ], pointsA2[ i ], pointsB2[ i ], pointsB1[ i ],
					tangentVertical, tangentVertical, tangentVertical, tangentVertical,
					texCoordVert[ 0 ][ i ], texCoordVert[ 1 ][ i ], texCoordVert[ 3 ][ i ], texCoordVert[ 2 ][ i ],
					insideOut, 0, true
					);

				//horizontal
				builder.AddQuad(
					pointsA2[ i ], pointsA1[ i + 1 ], pointsB1[ i + 1 ], pointsB2[ i ],
					tangentHoriz, tangentHoriz, tangentHoriz, tangentHoriz,
					texCoordHoriz[ 1 ][ i ], texCoordHoriz[ 0 ][ i + 1 ], texCoordHoriz[ 2 ][ i + 1 ], texCoordHoriz[ 3 ][ i ],
					insideOut, 0, true
				);
			}

			if( sides )
			{
				AddStairSide( builder, pointsA0, pointsA1, pointsA2, insideOut, 1.0 / steps, height / steps );
				AddStairSide( builder, pointsB0, pointsB1, pointsB2, !insideOut, 1.0 / steps, height / steps );
			}
			if( back )
			{
				////bottom
				//for( int i = 0; i < steps; i++ )
				//{
				//	builder.AddQuad(
				//		pointsA0[ i ], pointsB0[ i ], pointsB0[ i + 1 ], pointsA0[ i + 1 ],
				//		t, t, t, t,
				//		tex, tex, tex, tex,
				//		insideOut
				//		);
				//	builder.FaceEnd();
				//}

				double u0 = 0;
				double u1 = 1;
				double v0 = 1;
				double v1 = 0;
				if( insideOut )
				{
					u0 = 1;
					u1 = 0;
				}

				Vector4 t = new Vector4( pointsB0[ pointsB0.Length - 1 ].Position - pointsA0[ pointsA0.Length - 1 ].Position, -1 );
				builder.AddQuad(
					pointsA0[ pointsA0.Length - 1 ], pointsB0[ pointsB0.Length - 1 ], pointsB1[ pointsB1.Length - 1 ], pointsA1[ pointsA1.Length - 1 ],
					t, t, t, t,
					new Vector2( u0, v0 ), new Vector2( u1, v0 ), new Vector2( u1, v1 ), new Vector2( u0, v1 ),
					insideOut, 0, true
				);
			}

			builder.GetData( out positions, out normals, out tangents, out texCoords, out indices, out faces );

			//special for 2D
			if( axis == 1 )
			{
				Matrix3 axisRotation = new Matrix3( 0, 1, 0, -1, 0, 0, 0, 0, 1 );
				axisRotation *= Matrix3.FromRotateByX( Math.PI / 2 );

				{
					var values = positions;
					var newValues = new Vector3[ values.Length ];
					for( int n = 0; n < values.Length; n++ )
						newValues[ n ] = axisRotation * values[ n ];
					positions = newValues;
				}

				{
					var values = normals;
					var newValues = new Vector3[ values.Length ];
					for( int n = 0; n < values.Length; n++ )
						newValues[ n ] = axisRotation * values[ n ];
					normals = newValues;
				}

				{
					var values = tangents;
					var newValues = new Vector4[ values.Length ];
					for( int n = 0; n < values.Length; n++ )
						newValues[ n ] = new Vector4( axisRotation * values[ n ].ToVector3(), values[ n ].W );
					tangents = newValues;
				}
			}
			else
			{
				positions = RotateByAxis( axis, positions );
				normals = RotateByAxis( axis, normals );
				tangents = RotateByAxis( axis, tangents );
			}
		}

		static void AddStairSide( CommonPipeBuilder builder, Point[] points0, Point[] points1, Point[] points2, bool insideOut, double uStep, double vStep )
		{
			int steps = points0.Length - 1;
			double u0 = 0;
			double u1 = uStep;
			double v0 = 1;
			double v1 = 1 - vStep;
			double v2;
			if( insideOut )
			{
				u0 = 1 - u0;
				u1 = 1 - u1;
			}

			Vector4 t = new Vector4( points0[ 1 ].Position - points0[ 0 ].Position, -1 );

			builder.AddQuad(
				points0[ 0 ], points0[ 1 ], points1[ 1 ], points2[ 0 ],
				t, t, t, t,
				new Vector2( u0, v0 ), new Vector2( u1, v0 ), new Vector2( u1, v1 ), new Vector2( u0, v1 ),
				insideOut, 0, true );

			for( int i = 1; i < steps; i++ )
			{
				builder.BeginFace( 3 + 6 );
				u0 = uStep * i;
				u1 = uStep * ( i + 1 );
				v0 = 1;
				v1 = 1 - vStep * i;
				v2 = 1 - vStep * ( i + 1 );
				if( insideOut )
				{
					u0 = 1 - u0;
					u1 = 1 - u1;
				}

				t = new Vector4( points0[ i + 1 ].Position - points0[ i ].Position, -1 );
				builder.AddTriangle(
					points1[ i ], points1[ i + 1 ], points2[ i ],
					t, t, t,
					new Vector2( u0, v1 ), new Vector2( u1, v2 ), new Vector2( u0, v2 ),
					insideOut
					);
				builder.AddQuad(
					points0[ i ], points0[ i + 1 ], points1[ i + 1 ], points1[ i ],
					t, t, t, t,
					new Vector2( u0, v0 ), new Vector2( u1, v0 ), new Vector2( u1, v2 ), new Vector2( u0, v1 ),
					insideOut );
				builder.EndFace();
			}
		}

		static void GenerateStairsPoints( double depth, Vector3 offset, Degree curvature, double radius, double height, int steps, ref int nextVertex, out Point[] points0, out Point[] points1, out Point[] points2 )
		{
			int pointCount = steps + 1;
			double stepHeight = height / steps;
			bool reverse = false;
			if( curvature < 0 )
			{
				reverse = true;
				curvature = -curvature;
			}

			int tempVertex = 0;
			Point[] pts;
			if( curvature == 0 )
				pts = GenerateLine( pointCount, offset, new Vector3( depth / (double)steps, 0, 0 ), ref tempVertex );
			else
			{
				pts = GenerateCircleZ( pointCount, radius, curvature, ref tempVertex, -Math.PI / 2 );
				for( int i = 0; i < pts.Length; i++ )
				{
					pts[ i ].Position.Y += radius;
					if( reverse )
					{
						pts[ i ].Position.Y = -pts[ i ].Position.Y;
						pts[ i ].Position -= offset;
					}
					else
						pts[ i ].Position += offset;
				}
			}


			points0 = new Point[ pointCount ];
			points1 = new Point[ pointCount ];
			points2 = new Point[ pointCount ];


			points0[ 0 ].Position = new Vector3( pts[ 0 ].Position.X, pts[ 0 ].Position.Y, 0 );
			points2[ 0 ].Position = new Vector3( pts[ 0 ].Position.X, pts[ 0 ].Position.Y, stepHeight );
			points0[ 0 ].Vertex = nextVertex++;
			points2[ 0 ].Vertex = nextVertex++;
			points1[ 0 ] = points0[ 0 ];
			for( int i = 1; i < pointCount - 1; i++ )
			{
				points0[ i ].Position = new Vector3( pts[ i ].Position.X, pts[ i ].Position.Y, 0 );
				points1[ i ].Position = new Vector3( pts[ i ].Position.X, pts[ i ].Position.Y, stepHeight * i );
				points2[ i ].Position = new Vector3( pts[ i ].Position.X, pts[ i ].Position.Y, stepHeight * ( i + 1 ) );

				points0[ i ].Vertex = nextVertex++;
				points1[ i ].Vertex = nextVertex++;
				points2[ i ].Vertex = nextVertex++;
			}
			points0[ pointCount - 1 ].Position = new Vector3( pts[ pointCount - 1 ].Position.X, pts[ pointCount - 1 ].Position.Y, 0 );
			points1[ pointCount - 1 ].Position = new Vector3( pts[ pointCount - 1 ].Position.X, pts[ pointCount - 1 ].Position.Y, stepHeight * ( pointCount - 1 ) );
			points0[ pointCount - 1 ].Vertex = nextVertex++;
			points1[ pointCount - 1 ].Vertex = nextVertex++;
			points2[ pointCount - 1 ] = points1[ pointCount - 1 ];

		}

		public static void GenerateStairs( int axis, double width, double height, double depth, int steps, Degree curvature, double radius, bool sides, bool insideOut, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GenerateStairs( axis, width, height, depth, steps, curvature, radius, sides, insideOut, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
