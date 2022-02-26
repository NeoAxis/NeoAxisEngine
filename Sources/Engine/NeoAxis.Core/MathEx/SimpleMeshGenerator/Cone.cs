// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public enum ConeOrigin
		{
			Bottom,
			Center,
		}

		public static void GenerateCone( int axis, ConeOrigin origin, double radius, double height, int segments, bool needSide, bool needBottom, out Vector3[] positions, out int[] indices )
		{
			if( !needSide && !needBottom )
			{
				positions = new Vector3[ 0 ];
				indices = new int[ 0 ];
				return;
			}

			if( axis < 0 || axis > 2 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCone: axis < 0 || axis > 2." );
			//if( radius < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCone: radius < 0." );
			//if( height < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCone: height < 0." );
			if( segments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCone: segments < 3." );

			int topIndex = 0;
			int bottomIndex = 0;

			//positions
			{
				int vertexCount = segments;
				if( needSide )
					vertexCount++;
				if( needBottom )
					vertexCount++;

				positions = new Vector3[ vertexCount ];

				double[] cosTable = new double[ segments ];
				double[] sinTable = new double[ segments ];
				{
					double angleStep = Math.PI * 2 / segments;
					for( int n = 0; n < segments; n++ )
					{
						double angle = angleStep * n;
						cosTable[ n ] = Math.Cos( angle );
						sinTable[ n ] = Math.Sin( angle );
					}
				}

				int currentPosition = 0;

				for( int n = 0; n < segments; n++ )
				{
					positions[ currentPosition ] = new Vector3( 0, cosTable[ n ] * radius, sinTable[ n ] * radius );
					currentPosition++;
				}

				if( needSide )
				{
					topIndex = currentPosition;
					positions[ currentPosition ] = new Vector3( Math.Abs( height ), 0, 0 );
					currentPosition++;
				}

				if( needBottom )
				{
					bottomIndex = currentPosition;
					positions[ currentPosition ] = new Vector3( 0, 0, 0 );
					currentPosition++;
				}

				if( positions.Length != currentPosition )
					Log.Fatal( "SimpleMeshGenerator: GenerateCone: positions.Length != currentPosition." );
			}

			//indices
			{
				int indexCount = 0;
				if( needSide )
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
						indices[ currentIndex++ ] = start;
						if( radius < 0 || height < 0 )
						{
							indices[ currentIndex++ ] = topIndex;
							indices[ currentIndex++ ] = end;
						}
						else
						{
							indices[ currentIndex++ ] = end;
							indices[ currentIndex++ ] = topIndex;
						}
					}
				}
				if( needBottom )
				{
					for( int n = 0; n < segments; n++ )
					{
						int start = n;
						int end = ( n + 1 ) % segments;
						indices[ currentIndex++ ] = end;
						if( radius < 0 || height < 0 )
						{
							indices[ currentIndex++ ] = bottomIndex;
							indices[ currentIndex++ ] = start;
						}
						else
						{
							indices[ currentIndex++ ] = start;
							indices[ currentIndex++ ] = bottomIndex;
						}
					}
				}

				if( indices.Length != currentIndex )
					Log.Fatal( "SimpleMeshGenerator: GenerateCone: indices.Length != currentIndex." );
			}

			if( origin == ConeOrigin.Center )
			{
				Vector3 offset = new Vector3( -Math.Abs( height ) * .5, 0, 0 );
				var newValues = new Vector3[ positions.Length ];
				for( int n = 0; n < positions.Length; n++ )
					newValues[ n ] = positions[ n ] + offset;
				positions = newValues;
			}

			positions = RotateByAxis( axis, positions );
		}

		public static void GenerateCone( int axis, ConeOrigin origin, double radius, double height, int segments, bool needSide, bool needBottom, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			if( !needSide && !needBottom )
			{
				positions = new Vector3[ 0 ];
				indices = new int[ 0 ];
				normals = new Vector3[ 0 ];
				tangents = new Vector4[ 0 ];
				texCoords = new Vector2[ 0 ];
				faces = null;
				return;
			}

			if( axis < 0 || axis > 2 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCone: axis < 0 || axis > 2." );
			//if( radius < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCone: radius < 0." );
			//if( height < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCone: height < 0." );
			if( segments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCone: segments < 3." );

			int bottomStartIndex = 0;

			//positions
			{
				int vertexCount = 0;
				if( needSide )
					vertexCount += ( segments + 1 ) * 2;
				if( needBottom )
					vertexCount += segments + 2;

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
					for( int n = 0; n < segments + 1; n++ )
					{
						var fromVector = new Vector3( 0, cosTable[ n ], sinTable[ n ] );

						var from = fromVector * radius;
						var to = new Vector3( Math.Abs( height ), 0, 0 );

						positions[ currentPosition ] = from;

						//normals
						Vector3 normal;
						if( radius == 0 )
							normal = fromVector;
						else if( height == 0 )
							normal = Vector3.XAxis;
						else
						{
							var v = new Vector3( 0, from.Y, from.Z ).GetNormalize();
							normal = new Vector3( radius / height, v.Y * ( height / radius ), v.Z * ( height / radius ) ).GetNormalize();
						}
						if( radius < 0 || height < 0 )
							normal = -normal;
						normals[ currentPosition ] = normal;

						tangents[ currentPosition ] = new Vector4( ( to - from ).GetNormalize(), -1 );
						texCoords[ currentPosition ] = new Vector2( 0, (double)n / (double)segments * 2 );

						currentPosition++;
					}

					for( int n = 0; n < segments + 1; n++ )
					{
						double angle = ( (double)n + 0.5 ) / (double)segments * Math.PI * 2;
						var fromVector = new Vector3( 0, Math.Cos( angle ), Math.Sin( angle ) );
						//var fromVector = new Vec3( 0, cosTable[ n ], sinTable[ n ] );

						var from = fromVector * radius;
						var to = new Vector3( Math.Abs( height ), 0, 0 );

						positions[ currentPosition ] = to;

						//normals
						Vector3 normal;
						if( radius == 0 )
							normal = fromVector;
						else if( height == 0 )
							normal = Vector3.XAxis;
						else
						{
							var v = new Vector3( 0, from.Y, from.Z ).GetNormalize();
							normal = new Vector3( radius / height, v.Y * ( height / radius ), v.Z * ( height / radius ) ).GetNormalize();
						}
						if( radius < 0 || height < 0 )
							normal = -normal;
						normals[ currentPosition ] = normal;

						tangents[ currentPosition ] = new Vector4( ( to - from ).GetNormalize(), -1 );
						texCoords[ currentPosition ] = new Vector2( 1, ( (double)n + 0.5 ) / (double)segments * 2 );

						currentPosition++;
					}
				}

				if( needBottom )
				{
					bottomStartIndex = currentPosition;
					for( int n = 0; n < segments + 1; n++ )
					{
						positions[ currentPosition ] = new Vector3( 0, cosTable[ n ] * radius, sinTable[ n ] * radius );
						normals[ currentPosition ] = new Vector3( -1, 0, 0 );
						if( radius < 0 || height < 0 )
							normals[ currentPosition ] = -normals[ currentPosition ];
						tangents[ currentPosition ] = new Vector4( 0, 0, -1, -1 );

						if( radius < 0 || height < 0 )
							texCoords[ currentPosition ] = new Vector2( sinTable[ n ], cosTable[ n ] ) * 0.5 + new Vector2( 0.5, 0.5 );
						else
							texCoords[ currentPosition ] = new Vector2( -sinTable[ n ], cosTable[ n ] ) * 0.5 + new Vector2( 0.5, 0.5 );
						currentPosition++;
					}

					positions[ currentPosition ] = new Vector3( 0, 0, 0 );
					normals[ currentPosition ] = new Vector3( -1, 0, 0 );
					if( radius < 0 || height < 0 )
						normals[ currentPosition ] = -normals[ currentPosition ];
					tangents[ currentPosition ] = new Vector4( 0, 0, -1, -1 );
					texCoords[ currentPosition ] = new Vector2( 0.5, 0.5 );
					currentPosition++;
				}

				if( positions.Length != currentPosition )
					Log.Fatal( "SimpleMeshGenerator: GenerateCone: positions.Length != currentPosition." );
			}



			//indices and faces
			{
				int faceCount = 0;
				if( needSide )
					faceCount += segments;
				if( needBottom )
					faceCount += 1;

				//side faces - faces[0..segments) ; bottom - faces[segments]
				faces = new Face[ faceCount ];

				//Vertex indices will be: on bottom [0..segments); top[segments] ; bottom center[segments+1]
				const int bottomVertexStart = 0;
				int topVertex = segments;
				int bottomCenterVertex = segments + 1;

				int indexCount = 0;
				if( needSide )
					indexCount += segments * 3;
				if( needBottom )
					indexCount += segments * 3;

				indices = new int[ indexCount ];

				int currentIndex = 0;

				if( needSide )
				{
					for( int n = 0; n < segments; n++ )
					{
						//int index = n * 2;
						indices[ currentIndex++ ] = n;
						int nextClipped = ( n + 1 ) % segments;
						if( radius < 0 || height < 0 )
						{
							indices[ currentIndex++ ] = n + segments + 1; //top 
							indices[ currentIndex++ ] = n + 1;

							faces[ n ] = new Face
							{
								Triangles = new[]{
									new FaceVertex(bottomVertexStart + n, n),
									new FaceVertex(topVertex, n + segments + 1),
									new FaceVertex(bottomVertexStart + nextClipped, n + 1)}
							};
						}
						else
						{
							indices[ currentIndex++ ] = n + 1;
							indices[ currentIndex++ ] = n + segments + 1;

							faces[ n ] = new Face
							{
								Triangles = new[]{
									new FaceVertex(bottomVertexStart + n, n),
									new FaceVertex(bottomVertexStart + nextClipped, n + 1),
									new FaceVertex(topVertex, n + segments + 1)}
							};

						}

					}
				}
				if( needBottom )
				{
					var faceTriangles = new FaceVertex[ segments * 3 ];

					for( int n = 0; n < segments; n++ )
					{
						int index = bottomStartIndex + n;
						int n3 = n * 3;

						faceTriangles[ n3 ] = new FaceVertex(bottomVertexStart + ( n + 1 ) % segments, index + 1 );
						indices[ currentIndex++ ] = index + 1;

						if( radius < 0 || height < 0 )
						{
							faceTriangles[ n3 + 1 ] = new FaceVertex( bottomCenterVertex, bottomStartIndex + segments + 1 );
							indices[ currentIndex++ ] = bottomStartIndex + segments + 1; //center

							faceTriangles[ n3 + 2 ] = new FaceVertex( bottomVertexStart + n , index );
							indices[ currentIndex++ ] = index;
						}
						else
						{
							faceTriangles[ n3 + 1 ] = new FaceVertex( bottomVertexStart + n, index );
							indices[ currentIndex++ ] = index;

							faceTriangles[ n3 + 2 ] = new FaceVertex( bottomCenterVertex, bottomStartIndex + segments + 1 );
							indices[ currentIndex++ ] = bottomStartIndex + segments + 1;
						}
					}
					faces[ segments ] = new Face( faceTriangles );
				}

				if( indices.Length != currentIndex )
					Log.Fatal( "SimpleMeshGenerator: GenerateCone: indices.Length != currentIndex." );
			}

			if( origin == ConeOrigin.Center )
			{
				Vector3 offset = new Vector3( -Math.Abs( height ) * .5, 0, 0 );
				var newValues = new Vector3[ positions.Length ];
				for( int n = 0; n < positions.Length; n++ )
					newValues[ n ] = positions[ n ] + offset;
				positions = newValues;
			}

			positions = RotateByAxis( axis, positions );
			normals = RotateByAxis( axis, normals );
			tangents = RotateByAxis( axis, tangents );
		}

		public static void GenerateCone( int axis, ConeOrigin origin, double radius, double height, int segments, bool needSide, bool needBottom, out Vector3F[] positions, out int[] indices )
		{
			GenerateCone( axis, origin, radius, height, segments, needSide, needBottom, out Vector3[] positionsD, out indices );
			positions = ToVector3F( positionsD );
		}

		public static void GenerateCone( int axis, ConeOrigin origin, double radius, double height, int segments, bool needSide, bool needBottom, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			Vector3[] positionsD;
			Vector3[] normalsD;
			Vector4[] tangentsD;
			Vector2[] texCoordsD;
			GenerateCone( axis, origin, radius, height, segments, needSide, needBottom, out positionsD, out normalsD, out tangentsD, out texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
