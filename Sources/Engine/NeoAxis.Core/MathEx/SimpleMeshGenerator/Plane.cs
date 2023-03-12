// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GenerateSegmentedPlane( int axis, Vector2 size, Vector2I segments, Vector2 uvTilesPerUnit, Vector2 uvTilesInTotal, Vector2 uvOffset, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			int resX = Math.Max( 2, segments.X + 1 ); // 2 minimum
			int resY = Math.Max( 2, segments.Y + 1 );

			positions = new Vector3[ resX * resY ];
			texCoords = new Vector2[ positions.Length ];

			Vector2 vt = uvTilesPerUnit * size;

			for( int y = 0; y < resY; y++ )
			{
				// [ -height / 2, height / 2 ]
				double yPos = ( (double)y / ( resY - 1 ) - .5 ) * size.Y;
				double ty = yPos + size.Y / 2;// [0, height]
				ty /= size.Y;// [0, 1]

				for( int x = 0; x < resX; x++ )
				{
					// [ -width / 2, width / 2 ]
					double xPos = ( (double)x / ( resX - 1 ) - .5 ) * size.X;

					double tx = xPos + size.X / 2;// [0, width]
					tx /= size.X;// [0, 1]

					int index = x + y * resX;

					if( axis == 0 )
						positions[ index ] = new Vector3( 0, xPos, yPos );
					else if( axis == 1 )
						positions[ index ] = new Vector3( -xPos, 0, yPos );
					else
						positions[ index ] = new Vector3( xPos, yPos, 0 );
					//texCoords[ index ] = new Vector2( tx, 1.0f - ty );

					if( uvTilesInTotal != Vector2.Zero )
						texCoords[ index ] = new Vector2( tx * uvTilesInTotal.X, 1.0f - ty * uvTilesInTotal.Y );
					else if( uvTilesPerUnit != Vector2.Zero )
						texCoords[ index ] = new Vector2( tx * vt.X, 1.0f - ty * vt.Y );
					else
						texCoords[ index ] = new Vector2( tx, 1.0f - ty );
				}
			}

			if( uvOffset != Vector2.Zero )
			{
				for( int n = 0; n < texCoords.Length; n++ )
					texCoords[ n ] += uvOffset;
			}

			normals = new Vector3[ positions.Length ];
			for( int n = 0; n < normals.Length; n++ )
			{
				if( axis == 0 )
					normals[ n ] = Vector3F.XAxis;
				else if( axis == 1 )
					normals[ n ] = Vector3F.YAxis;
				else
					normals[ n ] = Vector3F.ZAxis;
			}

			tangents = new Vector4[ positions.Length ];
			for( int n = 0; n < tangents.Length; n++ )
			{
				if( axis == 0 )
					tangents[ n ] = new Vector4F( 0, 1, 0, -1 );
				else if( axis == 1 )
					tangents[ n ] = new Vector4F( -1, 0, 0, -1 );
				else
					tangents[ n ] = new Vector4F( 1, 0, 0, -1 );
			}

			int nbFaces = ( resX - 1 ) * ( resY - 1 );
			indices = new int[ nbFaces * 6 ];
			int tIdx = 0;
			for( int fy = 0; fy < ( resY - 1 ); fy++ )
			{
				for( int fx = 0; fx < ( resX - 1 ); fx++ )
				{
					indices[ tIdx++ ] = fy * resX + fx;
					indices[ tIdx++ ] = fy * resX + 1 + fx;
					indices[ tIdx++ ] = fy * resX + resX + fx;

					indices[ tIdx++ ] = fy * resX + resX + fx;
					indices[ tIdx++ ] = fy * resX + 1 + fx;
					indices[ tIdx++ ] = fy * resX + resX + 1 + fx;
				}
			}

			var faceTriangles = new FaceVertex[ indices.Length ];
			for( int i = 0; i < indices.Length; i++ )
				faceTriangles[ i ] = new FaceVertex( indices[ i ], indices[ i ] );
			faces = new Face[] { new Face( faceTriangles ) };
		}

		public static void GeneratePlane( Vector2 size, Vector2 uvTilesPerUnit, Vector2 uvTilesInTotal, Vector2 uvOffset, out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			var half = size * 0.5;
			positions = new Vector3[ 4 ];
			positions[ 0 ] = new Vector3( -half.X, -half.Y, 0 );
			positions[ 1 ] = new Vector3( half.X, -half.Y, 0 );
			positions[ 2 ] = new Vector3( half.X, half.Y, 0 );
			positions[ 3 ] = new Vector3( -half.X, half.Y, 0 );

			normals = new Vector3[ 4 ];
			normals[ 0 ] = Vector3.ZAxis;
			normals[ 1 ] = Vector3.ZAxis;
			normals[ 2 ] = Vector3.ZAxis;
			normals[ 3 ] = Vector3.ZAxis;

			tangents = new Vector4[ 4 ];
			tangents[ 0 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 1 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 2 ] = new Vector4( 1, 0, 0, -1 );
			tangents[ 3 ] = new Vector4( 1, 0, 0, -1 );
			//tangents[ 0 ] = new Vector4( 1, 0, 0, 1 );
			//tangents[ 1 ] = new Vector4( 1, 0, 0, 1 );
			//tangents[ 2 ] = new Vector4( 1, 0, 0, 1 );
			//tangents[ 3 ] = new Vector4( 1, 0, 0, 1 );

			texCoords = new Vector2[ 4 ];
			if( uvTilesInTotal != Vector2.Zero )
			{
				texCoords[ 0 ] = new Vector2( 0, uvTilesInTotal.Y );
				texCoords[ 1 ] = new Vector2( uvTilesInTotal.X, uvTilesInTotal.Y );
				texCoords[ 2 ] = new Vector2( uvTilesInTotal.X, 0 );
				texCoords[ 3 ] = new Vector2( 0, 0 );

				//var half = uvTilesInTotal * 0.5;
				//texCoords[ 0 ] = new Vec2( -half.X, -half.Y );
				//texCoords[ 1 ] = new Vec2( half.X, -half.Y );
				//texCoords[ 2 ] = new Vec2( half.X, half.Y );
				//texCoords[ 3 ] = new Vec2( -half.X, half.Y );
			}
			else if( uvTilesPerUnit != Vector2.Zero )
			{
				Vector2 v = uvTilesPerUnit * size;
				texCoords[ 0 ] = new Vector2( 0, v.Y );
				texCoords[ 1 ] = new Vector2( v.X, v.Y );
				texCoords[ 2 ] = new Vector2( v.X, 0 );
				texCoords[ 3 ] = new Vector2( 0, 0 );

				//Vec2 half = uvTilesPerUnit * size * 0.5;
				//texCoords[ 0 ] = new Vec2( -half.X, -half.Y );
				//texCoords[ 1 ] = new Vec2( half.X, -half.Y );
				//texCoords[ 2 ] = new Vec2( half.X, half.Y );
				//texCoords[ 3 ] = new Vec2( -half.X, half.Y );
			}

			if( uvOffset != Vector2.Zero )
			{
				for( int n = 0; n < texCoords.Length; n++ )
					texCoords[ n ] += uvOffset;
			}

			indices = new int[] { 0, 1, 2, 2, 3, 0 };

			var faceTriangles = new FaceVertex[ indices.Length ];
			for( int i = 0; i < indices.Length; i++ )
				faceTriangles[ i ] = new FaceVertex( indices[ i ], indices[ i ] );
			faces = new Face[] { new Face( faceTriangles ) };
		}

		public static void GeneratePlane( Vector2 size, Vector2 uvTilesPerUnit, Vector2 uvTilesInTotal, Vector2 uvOffset, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GeneratePlane( size, uvTilesPerUnit, uvTilesInTotal, uvOffset, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}

		public static void GenerateSegmentedPlane( int axis, Vector2F size, Vector2I segments, Vector2F uvTilesPerUnit, Vector2F uvTilesInTotal, Vector2F uvOffset, out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			GenerateSegmentedPlane( axis, size, segments, uvTilesPerUnit, uvTilesInTotal, uvOffset, out Vector3[] positionsD, out Vector3[] normalsD, out Vector4[] tangentsD, out Vector2[] texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
