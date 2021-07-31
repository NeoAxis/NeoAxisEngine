// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GenerateCylinder( int axis, double radius, double height, int segments, bool needTop, bool needSide, bool needBottom, out Vector3[] positions, out int[] indices )
		{
			if( axis < 0 || axis > 2 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: axis < 0 || axis > 2." );
			//if( radius < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: radius < 0." );
			//if( height < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: height < 0." );
			if( segments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: segments < 3." );

			int topSideIndex = 0;
			int bottomSideIndex = 0;
			int topIndex = 0;
			int bottomIndex = 0;

			//positions
			{
				int vertexCount = 0;

				if( needSide )
				{
					vertexCount += segments * 2;
					if( needTop )
						vertexCount++;
					if( needBottom )
						vertexCount++;
				}
				else
				{
					if( needTop )
						vertexCount += segments + 1;
					if( needBottom )
						vertexCount += segments + 1;
				}

				positions = new Vector3[ vertexCount ];

				double[] cosTable = new double[ segments ];
				double[] sinTable = new double[ segments ];
				{
					double angleStep = Math.PI * 2 / segments;
					for( int n = 0; n < segments; n++ )
					{
						double angle = angleStep * n;
						cosTable[ n ] = Math.Cos( angle );// *radius;
						sinTable[ n ] = Math.Sin( angle );// *radius;
					}
				}

				int currentPosition = 0;

				if( needSide || needTop )
				{
					topSideIndex = currentPosition;
					for( int n = 0; n < segments; n++ )
					{
						positions[ currentPosition ] = new Vector3( height * .5, cosTable[ n ] * radius, sinTable[ n ] * radius );
						//positions[ currentPosition ] = new Vec3( cosTable[ n ] * radius, sinTable[ n ] * radius, height * .5 );
						currentPosition++;
					}

					if( needTop )
					{
						topIndex = currentPosition;
						positions[ currentPosition ] = new Vector3( height * .5, 0, 0 );
						//positions[ currentPosition ] = new Vec3( 0, 0, height * .5 );
						currentPosition++;
					}
				}

				if( needSide || needBottom )
				{
					bottomSideIndex = currentPosition;
					for( int n = 0; n < segments; n++ )
					{
						positions[ currentPosition ] = new Vector3( -height * .5, cosTable[ n ] * radius, sinTable[ n ] * radius );
						//positions[ currentPosition ] = new Vec3( cosTable[ n ] * radius, sinTable[ n ] * radius, -height * .5 );
						currentPosition++;
					}

					if( needBottom )
					{
						bottomIndex = currentPosition;
						positions[ currentPosition ] = new Vector3( -height * .5, 0, 0 );
						//positions[ currentPosition ] = new Vec3( 0, 0, -height * .5 );
						currentPosition++;
					}
				}

				if( positions.Length != currentPosition )
					Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: positions.Length != currentPosition." );
			}

			//indices
			{
				int indexCount = 0;

				if( needSide )
					indexCount += segments * 2 * 3;
				if( needTop )
					indexCount += segments * 3;
				if( needBottom )
					indexCount += segments * 3;

				indices = new int[ indexCount ];

				int currentIndex = 0;

				if( needSide )
				{
					for( int n = 0; n < segments; n++ )
					{
						int start = n;
						int end = ( n + 1 ) % segments;

						indices[ currentIndex++ ] = topSideIndex + end;
						indices[ currentIndex++ ] = topSideIndex + start;
						indices[ currentIndex++ ] = bottomSideIndex + start;

						indices[ currentIndex++ ] = bottomSideIndex + start;
						indices[ currentIndex++ ] = bottomSideIndex + end;
						indices[ currentIndex++ ] = topSideIndex + end;
					}
				}
				if( needTop )
				{
					for( int n = 0; n < segments; n++ )
					{
						int start = n;
						int end = ( n + 1 ) % segments;

						indices[ currentIndex++ ] = topSideIndex + start;
						indices[ currentIndex++ ] = topSideIndex + end;
						indices[ currentIndex++ ] = topIndex;
					}
				}
				if( needBottom )
				{
					for( int n = 0; n < segments; n++ )
					{
						int start = n;
						int end = ( n + 1 ) % segments;

						indices[ currentIndex++ ] = bottomSideIndex + end;
						indices[ currentIndex++ ] = bottomSideIndex + start;
						indices[ currentIndex++ ] = bottomIndex;
					}
				}
			}

			positions = RotateByAxis( axis, positions );
		}

		public static void GenerateCylinder( int axis, double radius, double height, int segments, bool needTop, bool needSide, bool needBottom, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			if( axis < 0 || axis > 2 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: axis < 0 || axis > 2." );
			//if( radius < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: radius < 0." );
			//if( height < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: height < 0." );
			if( segments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: segments < 3." );

			int topIndex = 0;
			int topStartIndex = 0;

			int bottomIndex = 0;
			int bottomStartIndex = 0;

			int sideTopIndex = 0;
			int sideBottomIndex = 0;

			//positions
			{
				int vertexCount = 0;

				if( needSide )
					vertexCount += ( segments + 1 ) * 2;
				if( needTop )
					vertexCount += segments + 1;
				if( needBottom )
					vertexCount += segments + 1;

				positions = new Vector3[ vertexCount ];
				normals = new Vector3[ vertexCount ];
				tangents = new Vector4[ vertexCount ];
				texCoords = new Vector2[ vertexCount ];

				double[] cosTable = new double[ segments + 1 ];
				double[] sinTable = new double[ segments + 1 ];
				{
					double angleStep = Math.PI * 2 / segments;
					for( int n = 0; n < segments + 1; n++ )
					{
						double angle = angleStep * n;
						cosTable[ n ] = Math.Cos( angle );
						sinTable[ n ] = Math.Sin( angle );
					}
				}

				int currentPosition = 0;

				if( needSide )
				{
					sideTopIndex = currentPosition;
					for( int n = 0; n < segments + 1; n++ )
					{
						positions[ currentPosition ] = new Vector3( height * .5, cosTable[ n ] * radius, sinTable[ n ] * radius );
						normals[ currentPosition ] = new Vector3( 0, cosTable[ n ], sinTable[ n ] );
						if( radius < 0 || height < 0 )
							tangents[ currentPosition ] = new Vector4( -1, 0, 0, -1 );
						else
							tangents[ currentPosition ] = new Vector4( 1, 0, 0, -1 );
						texCoords[ currentPosition ] = new Vector2( 1, (double)n / (double)segments * 2 );
						currentPosition++;
					}

					sideBottomIndex = currentPosition;
					for( int n = 0; n < segments + 1; n++ )
					{
						positions[ currentPosition ] = new Vector3( -height * .5, cosTable[ n ] * radius, sinTable[ n ] * radius );
						normals[ currentPosition ] = new Vector3( 0, cosTable[ n ], sinTable[ n ] );
						if( radius < 0 || height < 0 )
							tangents[ currentPosition ] = new Vector4( -1, 0, 0, -1 );
						else
							tangents[ currentPosition ] = new Vector4( 1, 0, 0, -1 );
						texCoords[ currentPosition ] = new Vector2( 0, (double)n / (double)segments * 2 );
						currentPosition++;
					}
				}

				if( needTop )
				{
					topStartIndex = currentPosition;
					for( int n = 0; n < segments; n++ )
					{
						positions[ currentPosition ] = new Vector3( height * .5f, cosTable[ n ] * radius, sinTable[ n ] * radius );
						normals[ currentPosition ] = new Vector3( 1, 0, 0 );
						tangents[ currentPosition ] = new Vector4( 0, 0, -1, -1 );
						if( radius < 0 || height < 0 )
							texCoords[ currentPosition ] = new Vector2( sinTable[ n ], cosTable[ n ] ) * 0.5 + new Vector2( 0.5, 0.5 );
						else
							texCoords[ currentPosition ] = new Vector2( -sinTable[ n ], -cosTable[ n ] ) * 0.5 + new Vector2( 0.5, 0.5 );
						currentPosition++;
					}

					topIndex = currentPosition;
					positions[ currentPosition ] = new Vector3( height * .5, 0, 0 );
					normals[ currentPosition ] = new Vector3( 1, 0, 0 );
					tangents[ currentPosition ] = new Vector4( 0, 0, -1, -1 );
					texCoords[ currentPosition ] = new Vector2( 0.5, 0.5 );
					currentPosition++;
				}

				if( needBottom )
				{
					bottomStartIndex = currentPosition;
					for( int n = 0; n < segments; n++ )
					{
						positions[ currentPosition ] = new Vector3( -height * .5, cosTable[ n ] * radius, sinTable[ n ] * radius );
						normals[ currentPosition ] = new Vector3( -1, 0, 0 );
						tangents[ currentPosition ] = new Vector4( 0, 0, -1, -1 );
						if( radius < 0 || height < 0 )
							texCoords[ currentPosition ] = new Vector2( sinTable[ n ], -cosTable[ n ] ) * 0.5 + new Vector2( 0.5, 0.5 );
						else
							texCoords[ currentPosition ] = new Vector2( -sinTable[ n ], cosTable[ n ] ) * 0.5 + new Vector2( 0.5, 0.5 );
						currentPosition++;
					}

					bottomIndex = currentPosition;
					positions[ currentPosition ] = new Vector3( -height * .5, 0, 0 );
					normals[ currentPosition ] = new Vector3( -1, 0, 0 );
					tangents[ currentPosition ] = new Vector4( 0, 0, -1, -1 );
					texCoords[ currentPosition ] = new Vector2( 0.5, 0.5 );
					currentPosition++;
				}

				if( positions.Length != currentPosition )
					Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: positions.Length != currentPosition." );
			}

			//indices and faces
			{
				int faceCount = 0;
				if( needSide )
					faceCount += segments;
				if( needTop )
					faceCount += 1;
				if( needBottom )
					faceCount += 1;

				//side faces - faces[0..segments) ; top - faces[segments]; bottom - faces[segments + 1]
				faces = new Face[ faceCount ];

				//Vertex indices will be: on top [0..segments), top center [segments], on bottom [segments+1..segments*2+1), bottom center[segments*2+1]
				const int topVertexStart = 0;
				int topCenterVertex = segments;
				int bottomVertexStart = segments + 1;
				int bottomCenterVertex = segments * 2 + 1;


				int indexCount = 0;

				if( needSide )
					indexCount += segments * 2 * 3;
				if( needTop )
					indexCount += segments * 3;
				if( needBottom )
					indexCount += segments * 3;

				indices = new int[ indexCount ];

				int currentIndex = 0;

				if( needSide )
				{
					for( int n = 0; n < segments; n++ )
					{
						int start = n;
						int end = ( n + 1 );// % segments;
						
						indices[ currentIndex++ ] = sideTopIndex + end;
						indices[ currentIndex++ ] = sideTopIndex + start;
						indices[ currentIndex++ ] = sideBottomIndex + start;

						indices[ currentIndex++ ] = sideBottomIndex + start;
						indices[ currentIndex++ ] = sideBottomIndex + end;
						indices[ currentIndex++ ] = sideTopIndex + end;

						int endClipped = end % segments;
						faces[ n ] = new Face
						{
							Triangles = new[]{
								new FaceVertex(topVertexStart + endClipped, sideTopIndex + end),
								new FaceVertex(topVertexStart + start, sideTopIndex + start),
								new FaceVertex(bottomVertexStart + start, sideBottomIndex + start),
								new FaceVertex(bottomVertexStart + start, sideBottomIndex + start),
								new FaceVertex(bottomVertexStart + endClipped, sideBottomIndex + end),
								new FaceVertex(topVertexStart + endClipped, sideTopIndex + end) }
						};
					}
				}
				if( needTop )
				{
					var faceTriangles = new FaceVertex[ segments * 3 ];
					for( int n = 0; n < segments; n++ )
					{
						int start = n;
						int end = ( n + 1 ) % segments;
						int n3 = n * 3;

						faceTriangles[ n3 ] = new FaceVertex( topVertexStart + start, topStartIndex + start );
						indices[ currentIndex++ ] = topStartIndex + start;

						faceTriangles[ n3 + 1 ] = new FaceVertex( topVertexStart + end, topStartIndex + end );
						indices[ currentIndex++ ] = topStartIndex + end;

						faceTriangles[ n3 + 2 ] = new FaceVertex( topCenterVertex, topIndex );
						indices[ currentIndex++ ] = topIndex;
					}
					faces[ segments ] = new Face( faceTriangles );
				}
				if( needBottom )
				{
					var faceTriangles = new FaceVertex[ segments * 3 ];
					for( int n = 0; n < segments; n++ )
					{
						int start = n;
						int end = ( n + 1 ) % segments;
						int n3 = n * 3;

						faceTriangles[ n3 ] = new FaceVertex( bottomVertexStart + end, bottomStartIndex + end );
						indices[ currentIndex++ ] = bottomStartIndex + end;

						faceTriangles[ n3 + 1 ] = new FaceVertex( bottomVertexStart + start, bottomStartIndex + start );
						indices[ currentIndex++ ] = bottomStartIndex + start;

						faceTriangles[ n3 + 2 ] = new FaceVertex( bottomCenterVertex, bottomIndex );
						indices[ currentIndex++ ] = bottomIndex;
					}
					faces[ segments + 1 ] = new Face( faceTriangles );
				}

				if( indices.Length != currentIndex )
					Log.Fatal( "SimpleMeshGenerator: GenerateCylinder: indices.Length != currentIndex." );
			}

			positions = RotateByAxis( axis, positions );
			normals = RotateByAxis( axis, normals );
			tangents = RotateByAxis( axis, tangents );
		}

		public static void GenerateCylinder( int axis, double radius, double height, int segments, bool needTop, bool needSide, bool needBottom, out Vector3F[] positions, out int[] indices )
		{
			GenerateCylinder( axis, radius, height, segments, needTop, needSide, needBottom, out Vector3[] positionsD, out indices );
			positions = ToVector3F( positionsD );
		}

		public static void GenerateCylinder( int axis, double radius, double height, int segments, bool needTop, bool needSide, bool needBottom, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			Vector3[] positionsD;
			Vector3[] normalsD;
			Vector4[] tangentsD;
			Vector2[] texCoordsD;
			GenerateCylinder( axis, radius, height, segments, needTop, needSide, needBottom, out positionsD, out normalsD, out tangentsD, out texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
