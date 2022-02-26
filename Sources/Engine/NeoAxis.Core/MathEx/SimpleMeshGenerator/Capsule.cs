// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GenerateCapsule( int axis, double radius, double height, int hSegments, int vSegments, out Vector3[] positions, out int[] indices )
		{
			if( axis < 0 || axis > 2 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: axis < 0 || axis > 2." );
			//if( radius < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: radius < 0." );
			//if( height < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: height < 0." );
			if( hSegments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: hSegments < 3." );
			if( vSegments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: vSegments < 3." );
			if( vSegments % 2 != 1 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: vSegments % 2 != 1." );

			//positions
			{
				int vertexCount = hSegments * ( vSegments - 1 ) + 2;

				positions = new Vector3[ vertexCount ];

				double[] cosTable = new double[ hSegments ];
				double[] sinTable = new double[ hSegments ];
				{
					double angleStep = Math.PI * 2 / hSegments;
					for( int n = 0; n < hSegments; n++ )
					{
						double angle = angleStep * n;
						cosTable[ n ] = Math.Cos( angle );
						sinTable[ n ] = Math.Sin( angle );
					}
				}

				int levelCount = vSegments + 1;

				int currentPosition = 0;
				for( int v = 0; v < levelCount; v++ )
				{
					if( v == 0 )
					{
						positions[ currentPosition ] = new Vector3( radius + height * .5, 0, 0 );
						//positions[ currentPosition ] = new Vec3( 0, 0, radius + height * .5 );
						currentPosition++;
					}
					else if( v == vSegments )
					{
						positions[ currentPosition ] = new Vector3( -radius - height * .5, 0, 0 );
						//positions[ currentPosition ] = new Vec3( 0, 0, -radius - height * .5 );
						currentPosition++;
					}
					else
					{
						bool top = v <= vSegments / 2;

						double c;
						if( top )
							c = ( (double)v / (double)( vSegments - 1 ) );
						else
							c = ( (double)( v - 1 ) / (double)( vSegments - 1 ) );

						double angle = -( c * 2 - 1 ) * Math.PI / 2;
						double hRadius = Math.Cos( angle ) * radius;
						double h = Math.Sin( angle ) * radius + ( top ? height * .5 : -height * .5 );

						for( int n = 0; n < hSegments; n++ )
						{
							positions[ currentPosition ] = new Vector3( h, cosTable[ n ] * hRadius, sinTable[ n ] * hRadius );
							//positions[ currentPosition ] = new Vec3( cosTable[ n ] * hRadius, sinTable[ n ] * hRadius, h );
							currentPosition++;
						}
					}
				}

				if( positions.Length != currentPosition )
					Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: positions.Length != currentPosition." );
			}

			//indices
			{
				int indexCount = hSegments * ( vSegments - 2 ) * 2 * 3 + hSegments * 3 + hSegments * 3;
				indices = new int[ indexCount ];
				int levelCount = vSegments + 1;

				int currentIndex = 0;
				for( int v = 0; v < levelCount - 1; v++ )
				{
					int index;
					int nextIndex;
					if( v != 0 )
					{
						index = 1 + ( v - 1 ) * hSegments;
						nextIndex = index + hSegments;
					}
					else
					{
						index = 0;
						nextIndex = 1;
					}

					for( int n = 0; n < hSegments; n++ )
					{
						int start = n;
						int end = ( n + 1 ) % hSegments;

						if( v == 0 )
						{
							indices[ currentIndex++ ] = index;
							indices[ currentIndex++ ] = nextIndex + start;
							indices[ currentIndex++ ] = nextIndex + end;
						}
						else if( v == vSegments - 1 )
						{
							indices[ currentIndex++ ] = index + end;
							indices[ currentIndex++ ] = index + start;
							indices[ currentIndex++ ] = nextIndex;
						}
						else
						{
							indices[ currentIndex++ ] = index + end;
							indices[ currentIndex++ ] = index + start;
							indices[ currentIndex++ ] = nextIndex + end;

							indices[ currentIndex++ ] = nextIndex + start;
							indices[ currentIndex++ ] = nextIndex + end;
							indices[ currentIndex++ ] = index + start;
						}
					}
				}

				if( indices.Length != currentIndex )
					Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: indices.Length != currentIndex." );
			}

			positions = RotateByAxis( axis, positions );
		}

		public static void GenerateCapsule( int axis, double radius, double height, int hSegments, int vSegments, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			if( axis < 0 || axis > 2 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: axis < 0 || axis > 2." );
			//if( radius < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: radius < 0." );
			//if( height < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: height < 0." );
			if( hSegments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: hSegments < 3." );
			if( vSegments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: vSegments < 3." );
			if( vSegments % 2 != 1 )
				Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: vSegments % 2 != 1." );

			double[] cosTable = new double[ hSegments ];
			double[] sinTable = new double[ hSegments ];
			{
				double angleStep = Math.PI * 2 / hSegments;
				for( int n = 0; n < hSegments; n++ )
				{
					double angle = angleStep * n;
					cosTable[ n ] = Math.Cos( angle );// *radius;
					sinTable[ n ] = Math.Sin( angle );// *radius;
				}
			}

			//positions
			{
				int vertexCount = ( hSegments + 1 ) * ( vSegments + 1 );
				positions = new Vector3[ vertexCount ];
				normals = new Vector3[ vertexCount ];
				tangents = new Vector4[ vertexCount ];
				texCoords = new Vector2[ vertexCount ];

				int currentPosition = 0;

				for( int v = 0; v < vSegments + 1; v++ )
				{
					for( int n = 0; n < hSegments + 1; n++ )
					{
						bool up = v < ( vSegments + 1 ) / 2;

						double vCoef;
						if( up )
							vCoef = ( (double)v / (double)( vSegments - 1 ) );
						else
							vCoef = (double)( v - 1 ) / (double)( vSegments - 1 );

						SphericalDirection dir = new SphericalDirection(
							(double)n / (double)hSegments * Math.PI * 2,
							( vCoef - 0.5 ) * Math.PI );

						var p = dir.GetVector() * radius;
						var posNoOffset = new Vector3( p.Z, p.Y, -p.X );
						var posResult = posNoOffset;
						if( up )
							posResult.X -= height / 2;
						else
							posResult.X += height / 2;
						positions[ currentPosition ] = posResult;

						//normals
						normals[ currentPosition ] = posNoOffset.GetNormalize();
						if( radius < 0 )
							normals[ currentPosition ] = -normals[ currentPosition ];

						//tangents
						{
							Matrix3 rotationMatrix = Matrix3.FromRotateByX( -Math.PI / 2 );

							var dir2 = new SphericalDirection( (double)n / (double)hSegments * Math.PI * 2, 0 ).GetVector();
							dir2 = new Vector3( 0, dir2.Y, -dir2.X );
							tangents[ currentPosition ] = new Vector4( ( rotationMatrix * dir2 ).GetNormalize(), -1 );
						}

						//texCoords
						texCoords[ currentPosition ] = new Vector2(
							dir.Horizontal / ( MathEx.PI * 2 ) * 2,
							-dir.Vertical / MathEx.PI + 0.5 );
						if( !up )
							texCoords[ currentPosition ] = texCoords[ currentPosition ] - new Vector2( 0, 1 );

						if( radius < 0 || height < 0 )
							texCoords[ currentPosition ] = -texCoords[ currentPosition ];

						currentPosition++;
					}
				}

				if( positions.Length != currentPosition )
					Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: positions.Length != currentPosition." );
			}

			//indices
			{
				int indexCount = vSegments * hSegments * 6;
				indices = new int[ indexCount ];

				int currentIndex = 0;
				for( int v = 0; v < vSegments; v++ )
				{
					for( int n = 0; n < hSegments; n++ )
					{
						int index = v * ( hSegments + 1 ) + n;

						indices[ currentIndex++ ] = index;
						indices[ currentIndex++ ] = index + 1;
						indices[ currentIndex++ ] = index + hSegments + 1;

						indices[ currentIndex++ ] = index + hSegments + 1;
						indices[ currentIndex++ ] = index + 1;
						indices[ currentIndex++ ] = index + hSegments + 1 + 1;
					}
				}

				if( indices.Length != currentIndex )
					Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: indices.Length != currentIndex." );
			}

			positions = RotateByAxis( axis, positions );
			normals = RotateByAxis( axis, normals );
			tangents = RotateByAxis( axis, tangents );

			//??? Если нужно каждый фейс сделать прямоугольником то задать: rawVerticesInFace = rawVerticesInFaceForMiddle = 6
			faces = BuildFacesForGeoSphere( hSegments, vSegments, 3, 6, indices );
			
			////positions
			//{
			//	int vertexCount = hSegments * ( vSegments - 1 ) + 2;

			//	positions = new Vec3[ vertexCount ];
			//	normals = new Vec3[ vertexCount ];
			//	tangents = new Vec4[ vertexCount ];
			//	texCoords = new Vec2[ vertexCount ];

			//	double[] cosTable = new double[ hSegments ];
			//	double[] sinTable = new double[ hSegments ];
			//	{
			//		double angleStep = Math.PI * 2 / hSegments;
			//		for( int n = 0; n < hSegments; n++ )
			//		{
			//			double angle = angleStep * n;
			//			cosTable[ n ] = Math.Cos( angle );
			//			sinTable[ n ] = Math.Sin( angle );
			//		}
			//	}

			//	int levelCount = vSegments + 1;

			//	int currentPosition = 0;
			//	for( int v = 0; v < levelCount; v++ )
			//	{
			//		if( v == 0 )
			//		{
			//			positions[ currentPosition ] = new Vec3( radius + height * .5, 0, 0 );
			//			normals[ currentPosition ] = new Vec3( 1, 0, 0 );
			//			currentPosition++;
			//		}
			//		else if( v == vSegments )
			//		{
			//			positions[ currentPosition ] = new Vec3( -radius - height * .5, 0, 0 );
			//			normals[ currentPosition ] = new Vec3( -1, 0, 0 );
			//			currentPosition++;
			//		}
			//		else
			//		{
			//			bool top = v <= vSegments / 2;

			//			double c;
			//			if( top )
			//				c = ( (double)v / (double)( vSegments - 1 ) );
			//			else
			//				c = ( (double)( v - 1 ) / (double)( vSegments - 1 ) );

			//			double angle = -( c * 2 - 1 ) * Math.PI / 2;
			//			double hRadius = Math.Cos( angle ) * radius;
			//			double h = Math.Sin( angle ) * radius + ( top ? height * .5 : -height * .5 );

			//			for( int n = 0; n < hSegments; n++ )
			//			{
			//				positions[ currentPosition ] = new Vec3( h, cosTable[ n ] * hRadius, sinTable[ n ] * hRadius );
			//				normals[ currentPosition ] = new Vec3(
			//					Math.Sin( angle ) * radius, cosTable[ n ] * hRadius, sinTable[ n ] * hRadius ).GetNormalize();
			//				if( radius < 0 )
			//					normals[ currentPosition ] = -normals[ currentPosition ];

			//				currentPosition++;
			//			}
			//		}
			//	}

			//	if( positions.Length != currentPosition )
			//		Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: positions.Length != currentPosition." );
			//}

			////indices
			//{
			//	int indexCount = hSegments * ( vSegments - 2 ) * 2 * 3 + hSegments * 3 + hSegments * 3;
			//	indices = new int[ indexCount ];
			//	int levelCount = vSegments + 1;

			//	int currentIndex = 0;
			//	for( int v = 0; v < levelCount - 1; v++ )
			//	{
			//		int index;
			//		int nextIndex;

			//		if( v != 0 )
			//		{
			//			index = 1 + ( v - 1 ) * hSegments;
			//			nextIndex = index + hSegments;
			//		}
			//		else
			//		{
			//			index = 0;
			//			nextIndex = 1;
			//		}

			//		for( int n = 0; n < hSegments; n++ )
			//		{
			//			int start = n;
			//			int end = ( n + 1 ) % hSegments;

			//			if( v == 0 )
			//			{
			//				indices[ currentIndex++ ] = index;
			//				indices[ currentIndex++ ] = nextIndex + start;
			//				indices[ currentIndex++ ] = nextIndex + end;
			//			}
			//			else if( v == vSegments - 1 )
			//			{
			//				indices[ currentIndex++ ] = index + end;
			//				indices[ currentIndex++ ] = index + start;
			//				indices[ currentIndex++ ] = nextIndex;
			//			}
			//			else
			//			{
			//				indices[ currentIndex++ ] = index + end;
			//				indices[ currentIndex++ ] = index + start;
			//				indices[ currentIndex++ ] = nextIndex + end;

			//				indices[ currentIndex++ ] = nextIndex + start;
			//				indices[ currentIndex++ ] = nextIndex + end;
			//				indices[ currentIndex++ ] = index + start;
			//			}
			//		}
			//	}

			//	if( indices.Length != currentIndex )
			//		Log.Fatal( "SimpleMeshGenerator: GenerateCapsule: indices.Length != currentIndex." );
			//}

			//positions = RotateByAxis( axis, positions );
			//normals = RotateByAxis( axis, normals );
			//tangents = RotateByAxis( axis, tangents );
		}

		public static void GenerateCapsule( int axis, double radius, double height, int hSegments, int vSegments, out Vector3F[] positions, out int[] indices )
		{
			Vector3[] positionsD;
			GenerateCapsule( axis, radius, height, hSegments, vSegments, out positionsD, out indices );
			positions = ToVector3F( positionsD );
		}

		public static void GenerateCapsule( int axis, double radius, double height, int hSegments, int vSegments, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			Vector3[] positionsD;
			Vector3[] normalsD;
			Vector4[] tangentsD;
			Vector2[] texCoordsD;
			GenerateCapsule( axis, radius, height, hSegments, vSegments, out positionsD, out normalsD, out tangentsD, out texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
