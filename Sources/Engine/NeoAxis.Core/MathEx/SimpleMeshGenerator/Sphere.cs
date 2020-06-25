// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		// 20 faces, 60 indexes, 12 vertices
		private static int[] icosahedronIndexes = new int[] {
			0,11,5,
			0,5,1,
			0,1,7,
			0,7,10,
			0,10,11,
			1,5,9,
			5,11,4,
			11,10,2,
			10,7,6,
			7,1,8,
			3,9,4,
			3,4,2,
			3,2,6,
			3,6,8,
			3,8,9,
			4,9,5,
			2,4,11,
			6,2,10,
			8,6,7,
			9,8,1 };


		public static void GenerateSphere( double radius, int hSegments, int vSegments, bool insideOut, out Vector3[] positions, out int[] indices )
		{
			var radius2 = radius;
			if( insideOut )
				radius2 = -radius2;

			//if( radius < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateSphere: radius < 0." );
			if( hSegments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateSphere: hSegments < 3." );
			if( vSegments < 2 )
				Log.Fatal( "SimpleMeshGenerator: GenerateSphere: vSegments < 2." );

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

				int currentPosition = 0;

				for( int v = 0; v < vSegments + 1; v++ )
				{
					if( v == 0 )
					{
						positions[ currentPosition++ ] = new Vector3( 0, 0, radius2 );
					}
					else if( v == vSegments )
					{
						positions[ currentPosition++ ] = new Vector3( 0, 0, -radius2 );
					}
					else
					{
						double c = ( (double)v / (double)vSegments );
						double angle = -( c * 2 - 1 ) * Math.PI / 2;

						double hRadius = Math.Cos( angle ) * radius2;
						double h = Math.Sin( angle ) * radius2;

						for( int n = 0; n < hSegments; n++ )
							positions[ currentPosition++ ] = new Vector3( cosTable[ n ] * hRadius, sinTable[ n ] * hRadius, h );
					}
				}

				if( positions.Length != currentPosition )
					Log.Fatal( "SimpleMeshGenerator: GenerateSphere: positions.Length != currentPosition." );
			}

			//indices
			{
				int indexCount = hSegments * ( vSegments - 2 ) * 2 * 3 + hSegments * 3 + hSegments * 3;
				indices = new int[ indexCount ];

				int currentIndex = 0;
				for( int v = 0; v < vSegments; v++ )
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
					Log.Fatal( "SimpleMeshGenerator: GenerateSphere: indices.Length != currentIndex." );
			}
		}

		public static void GenerateSphere( double radius, int hSegments, int vSegments, bool insideOut,
			out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			var radius2 = radius;
			if( insideOut )
				radius2 = -radius2;

			//if( radius < 0 )
			//	Log.Fatal( "SimpleMeshGenerator: GenerateSphere: radius < 0." );
			if( hSegments < 3 )
				Log.Fatal( "SimpleMeshGenerator: GenerateSphere: hSegments < 3." );
			if( vSegments < 2 )
				Log.Fatal( "SimpleMeshGenerator: GenerateSphere: vSegments < 2." );

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
						SphericalDirection dir = new SphericalDirection(
							(double)n / (double)hSegments * Math.PI * 2,
							( (double)v / (double)vSegments - 0.5 ) * Math.PI );
						positions[ currentPosition ] = dir.GetVector() * radius2;

						texCoords[ currentPosition ] = new Vector2(
							dir.Horizontal / ( MathEx.PI * 2 ) * 2,
							-dir.Vertical / MathEx.PI + 0.5 );

						if( radius2 < 0 )
							texCoords[ currentPosition ] = new Vector2( 1.0f - texCoords[ currentPosition ].X, 1.0f - texCoords[ currentPosition ].Y );

						currentPosition++;
					}
				}

				if( positions.Length != currentPosition )
					Log.Fatal( "SimpleMeshGenerator: GenerateSphere: positions.Length != currentPosition." );
			}

			//normals
			{
				for( int n = 0; n < positions.Length; n++ )
				{
					normals[ n ] = positions[ n ].GetNormalize();
					if( radius2 < 0 )
						normals[ n ] = -normals[ n ];
				}
			}

			//tangents
			{
				Matrix3 rotationMatrix = new Matrix3( 0, -1, 0, 1, 0, 0, 0, 0, 1 );

				for( int n = 0; n < positions.Length; n++ )
				{
					var p = positions[ n ];
					var rot = Quaternion.FromDirectionZAxisUp( p.GetNormalize() ).ToMatrix3() * rotationMatrix;
					if( radius2 < 0 )
						tangents[ n ] = new Vector4( rot * Vector3.XAxis, -1 );
					else
						tangents[ n ] = new Vector4( rot * -Vector3.XAxis, -1 );
				}
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
					Log.Fatal( "SimpleMeshGenerator: GenerateSphere: indices.Length != currentIndex." );
			}

			//??? Если нужно каждый фейс сделать прямоугольником то задать: rawVerticesInFace = rawVerticesInFaceForMiddle = 6
			faces = BuildFacesForGeoSphere( hSegments, vSegments, 3, 3, indices );
		}

		//Is used in Sphere, Capsule
		public static Face[] BuildFacesForGeoSphere( int hSegments, int vSegments, int rawVerticesInFace, int rawVerticesInFaceForMiddle, int[] indices )
		{
			Debug.Assert( rawVerticesInFace == 3 && ( rawVerticesInFaceForMiddle == 3 || rawVerticesInFaceForMiddle == 6 ) );

			int faceCount = hSegments * ( vSegments - 2 ) * 6 / rawVerticesInFace +
			                hSegments * 6 / rawVerticesInFaceForMiddle;
			var faces = new Face[ faceCount];

			int middleSegmentStart = ( vSegments / 2 ) * hSegments * 6;
			int middleSegmentEnd = middleSegmentStart + hSegments * 6;
			int curTriangle = 0;
			int curFace = 0;

			FaceVertex[] faceTriangles = null;
			for( int i = 0; i < indices.Length; i++ )
			{
				int index = indices[ i ];
				int vIndex = index / ( hSegments + 1 );
				int hIndex = index % ( hSegments + 1 );
				hIndex %= hSegments;

				//Each vSegment has hSegments of vertices, exept the first and last vSegment, they have 1 vertex.
				//Verext count == hSegments * (vSegments-1) + 2; 
				int vertexIndex;
				if( vIndex == 0 )
					vertexIndex = hSegments * ( vSegments - 1 );
				else if( vIndex == vSegments )
					vertexIndex = hSegments * ( vSegments - 1 ) + 1;
				else
					vertexIndex = ( vIndex - 1 ) * hSegments + hIndex;
				if( faceTriangles == null )
				{
					faceTriangles = new FaceVertex[ middleSegmentStart <= i && i < middleSegmentEnd ? rawVerticesInFaceForMiddle : rawVerticesInFace ];
					curTriangle = 0;
				}

				faceTriangles[ curTriangle++ ] = new FaceVertex( vertexIndex, index );
				if( curTriangle == faceTriangles.Length )
				{
					bool isFirstDegenerate = IsDegenerate( faceTriangles[ 0 ].Vertex, faceTriangles[ 1 ].Vertex, faceTriangles[ 2 ].Vertex );
					Debug.Assert( ! ( faceTriangles.Length == 6 && (
							   isFirstDegenerate || IsDegenerate( faceTriangles[ 3 ].Vertex, faceTriangles[ 4 ].Vertex, faceTriangles[ 5 ].Vertex ) ) 
						   ));

					//??? Сейчас отбрасываются вырожденные. Можно ли сделать чтобы вырожденные не перебирались совсем?
					if( !isFirstDegenerate )
						faces[ curFace++ ] = new Face( faceTriangles );

					faceTriangles = null;
				}
			}

			Debug.Assert(faces.Length == curFace );
			return faces.ToArray();

			bool IsDegenerate( int i0, int i1, int i2 ) => i0 == i1 || i0 == i2 || i1 == i2;
		}

		//public static void GenerateSphere( double radius, int hSegments, int vSegments, out Vec3[] positions, out int[] indices )
		//{
		//	//if( radius < 0 )
		//	//	Log.Fatal( "SimpleMeshGenerator: GenerateSphere: radius < 0." );
		//	if( hSegments < 3 )
		//		Log.Fatal( "SimpleMeshGenerator: GenerateSphere: hSegments < 3." );
		//	if( vSegments < 2 )
		//		Log.Fatal( "SimpleMeshGenerator: GenerateSphere: vSegments < 2." );
		//	if( vSegments % 2 != 0 )
		//		Log.Fatal( "SimpleMeshGenerator: GenerateSphere: vSegments % 2 != 0." );

		//	//positions
		//	{
		//		int vertexCount;
		//		//if( !half )
		//		vertexCount = hSegments * ( vSegments - 1 ) + 2;
		//		//else
		//		//	vertexCount = hSegments * vSegments / 2 + 1;

		//		positions = new Vec3[ vertexCount ];

		//		double[] cosTable = new double[ hSegments ];
		//		double[] sinTable = new double[ hSegments ];
		//		{
		//			double angleStep = Math.PI * 2 / hSegments;
		//			for( int n = 0; n < hSegments; n++ )
		//			{
		//				double angle = angleStep * n;
		//				cosTable[ n ] = Math.Cos( angle );
		//				sinTable[ n ] = Math.Sin( angle );
		//			}
		//		}

		//		int currentPosition = 0;

		//		int levelCount;
		//		//if( !half )
		//		levelCount = vSegments + 1;
		//		//else
		//		//	levelCount = vSegments / 2 + 1;

		//		for( int v = 0; v < levelCount; v++ )
		//		{
		//			if( v == 0 )
		//			{
		//				positions[ currentPosition++ ] = new Vec3( 0, 0, radius );
		//			}
		//			else if( v == vSegments )
		//			{
		//				positions[ currentPosition++ ] = new Vec3( 0, 0, -radius );
		//			}
		//			else
		//			{
		//				double c = ( (double)v / (double)vSegments );
		//				double angle = -( c * 2 - 1 ) * Math.PI / 2;

		//				double hRadius = Math.Cos( angle ) * radius;
		//				double h = Math.Sin( angle ) * radius;

		//				for( int n = 0; n < hSegments; n++ )
		//				{
		//					positions[ currentPosition++ ] = new Vec3( cosTable[ n ] * hRadius, sinTable[ n ] * hRadius, h );
		//				}
		//			}
		//		}

		//		if( positions.Length != currentPosition )
		//			Log.Fatal( "SimpleMeshGenerator: GenerateSphere: positions.Length != currentPosition." );
		//	}

		//	//indices
		//	{
		//		int indexCount;
		//		//if( !half )
		//		indexCount = hSegments * ( vSegments - 2 ) * 2 * 3 + hSegments * 3 + hSegments * 3;
		//		//else
		//		//	indexCount = hSegments * ( vSegments / 2 - 1 ) * 2 * 3 + hSegments * 3;

		//		indices = new int[ indexCount ];

		//		int levelCount;
		//		//if( !half )
		//		levelCount = vSegments + 1;
		//		//else
		//		//	levelCount = vSegments / 2 + 1;

		//		int currentIndex = 0;

		//		for( int v = 0; v < levelCount - 1; v++ )
		//		{
		//			int index;
		//			int nextIndex;

		//			if( v != 0 )
		//			{
		//				index = 1 + ( v - 1 ) * hSegments;
		//				nextIndex = index + hSegments;
		//			}
		//			else
		//			{
		//				index = 0;
		//				nextIndex = 1;
		//			}

		//			for( int n = 0; n < hSegments; n++ )
		//			{
		//				int start = n;
		//				int end = ( n + 1 ) % hSegments;

		//				if( v == 0 )
		//				{

		//					indices[ currentIndex++ ] = index;
		//					indices[ currentIndex++ ] = nextIndex + start;
		//					indices[ currentIndex++ ] = nextIndex + end;
		//				}
		//				else if( v == vSegments - 1 )
		//				{
		//					indices[ currentIndex++ ] = index + end;
		//					indices[ currentIndex++ ] = index + start;
		//					indices[ currentIndex++ ] = nextIndex;
		//				}
		//				else
		//				{
		//					indices[ currentIndex++ ] = index + end;
		//					indices[ currentIndex++ ] = index + start;
		//					indices[ currentIndex++ ] = nextIndex + end;

		//					indices[ currentIndex++ ] = nextIndex + start;
		//					indices[ currentIndex++ ] = nextIndex + end;
		//					indices[ currentIndex++ ] = index + start;
		//				}
		//			}
		//		}

		//		if( indices.Length != currentIndex )
		//			Log.Fatal( "SimpleMeshGenerator: GenerateSphere: indices.Length != currentIndex." );
		//	}
		//}

		//public static void GenerateSphere( double radius, int hSegments, int vSegments, bool half,
		//	out Vec3[] positions, out Vec3[] normals, out Vec4[] tangents, out Vec2[] texCoords, out int[] indices )
		//{
		//	//if( radius < 0 )
		//	//	Log.Fatal( "SimpleMeshGenerator: GenerateSphere: radius < 0." );
		//	if( hSegments < 3 )
		//		Log.Fatal( "SimpleMeshGenerator: GenerateSphere: hSegments < 3." );
		//	if( vSegments < 2 )
		//		Log.Fatal( "SimpleMeshGenerator: GenerateSphere: vSegments < 2." );
		//	if( vSegments % 2 != 0 )
		//		Log.Fatal( "SimpleMeshGenerator: GenerateSphere: vSegments % 2 != 0." );

		//	//positions
		//	{
		//		int vertexCount;
		//		if( !half )
		//			vertexCount = hSegments * ( vSegments - 1 ) + 2;
		//		else
		//			vertexCount = hSegments * vSegments / 2 + 1;

		//		positions = new Vec3[ vertexCount ];

		//		double[] cosTable = new double[ hSegments ];
		//		double[] sinTable = new double[ hSegments ];
		//		{
		//			double angleStep = Math.PI * 2 / hSegments;
		//			for( int n = 0; n < hSegments; n++ )
		//			{
		//				double angle = angleStep * n;
		//				cosTable[ n ] = Math.Cos( angle );
		//				sinTable[ n ] = Math.Sin( angle );
		//			}
		//		}

		//		int currentPosition = 0;

		//		int levelCount;
		//		if( !half )
		//			levelCount = vSegments + 1;
		//		else
		//			levelCount = vSegments / 2 + 1;

		//		for( int v = 0; v < levelCount; v++ )
		//		{
		//			if( v == 0 )
		//			{
		//				positions[ currentPosition++ ] = new Vec3( 0, 0, radius );
		//			}
		//			else if( v == vSegments )
		//			{
		//				positions[ currentPosition++ ] = new Vec3( 0, 0, -radius );
		//			}
		//			else
		//			{
		//				double c = ( (double)v / (double)vSegments );
		//				double angle = -( c * 2 - 1 ) * Math.PI / 2;

		//				double hRadius = Math.Cos( angle ) * radius;
		//				double h = Math.Sin( angle ) * radius;

		//				for( int n = 0; n < hSegments; n++ )
		//				{
		//					positions[ currentPosition++ ] = new Vec3( cosTable[ n ] * hRadius, sinTable[ n ] * hRadius, h );
		//				}
		//			}
		//		}

		//		if( positions.Length != currentPosition )
		//			Log.Fatal( "SimpleMeshGenerator: GenerateSphere: positions.Length != currentPosition." );
		//	}

		//	//normals
		//	{
		//		normals = new Vec3[ positions.Length ];
		//		for( int n = 0; n < positions.Length; n++ )
		//		{
		//			normals[ n ] = positions[ n ].GetNormalize();
		//			if( radius < 0 )
		//				normals[ n ] = -normals[ n ];
		//		}
		//	}

		//	//tangents
		//	{
		//		Mat3 rotationMatrix = new Mat3( 0, -1, 0, 1, 0, 0, 0, 0, 1 );

		//		tangents = new Vec4[ positions.Length ];
		//		for( int n = 0; n < positions.Length; n++ )
		//		{
		//			var p = positions[ n ];
		//			var rot = Quat.FromDirectionZAxisUp( p.GetNormalize() ).ToMat3() * rotationMatrix;
		//			tangents[ n ] = new Vec4( rot * Vec3.XAxis, 1 );
		//		}
		//	}

		//	//texCoords
		//	{
		//		texCoords = new Vec2[ positions.Length ];
		//		//for( int n = 0; n < positions.Length; n++ )
		//		//{
		//		//	Vec3 v = positions[ n ].GetNormalize();
		//		//	texCoords[ n ] = new Vec2( Math.Asin( v.X ) / MathEx.PI, Math.Asin( v.Y ) / MathEx.PI );
		//		//}

		//		for( int n = 0; n < positions.Length; n++ )
		//		{
		//			Vec3 v = positions[ n ].GetNormalize();

		//			var dir = SphereDir.FromVector( v );
		//			texCoords[ n ] = new Vec2(
		//				dir.Horizontal / ( MathEx.PI * 2 ) * 2,
		//				dir.Vertical / ( MathEx.PI ) + 0.5f );
		//		}

		//		//pVertex->texCoord0 = new Vec2F(
		//		//	(float)Math.Atan2( v.y, v.x ) / ( 2 * MathEx.PI ) + 0.5f,
		//		//	v.z * 0.5f + 0.5f );


		//		//pVertex->texCoord0 = pVertex->normal.ToVec2() / 2 + new Vec2F( 0.5f, 0.5f );
		//		//pVertex->texCoord0 = pVertex->position.ToVec2() * 2;
		//	}

		//	//indices
		//	{
		//		int indexCount;

		//		if( !half )
		//			indexCount = hSegments * ( vSegments - 2 ) * 2 * 3 + hSegments * 3 + hSegments * 3;
		//		else
		//			indexCount = hSegments * ( vSegments / 2 - 1 ) * 2 * 3 + hSegments * 3;

		//		indices = new int[ indexCount ];

		//		int levelCount;
		//		if( !half )
		//			levelCount = vSegments + 1;
		//		else
		//			levelCount = vSegments / 2 + 1;

		//		int currentIndex = 0;

		//		for( int v = 0; v < levelCount - 1; v++ )
		//		{
		//			int index;
		//			int nextIndex;

		//			if( v != 0 )
		//			{
		//				index = 1 + ( v - 1 ) * hSegments;
		//				nextIndex = index + hSegments;
		//			}
		//			else
		//			{
		//				index = 0;
		//				nextIndex = 1;
		//			}

		//			for( int n = 0; n < hSegments; n++ )
		//			{
		//				int start = n;
		//				int end = ( n + 1 ) % hSegments;

		//				if( v == 0 )
		//				{

		//					indices[ currentIndex++ ] = index;
		//					indices[ currentIndex++ ] = nextIndex + start;
		//					indices[ currentIndex++ ] = nextIndex + end;
		//				}
		//				else if( v == vSegments - 1 )
		//				{
		//					indices[ currentIndex++ ] = index + end;
		//					indices[ currentIndex++ ] = index + start;
		//					indices[ currentIndex++ ] = nextIndex;
		//				}
		//				else
		//				{
		//					indices[ currentIndex++ ] = index + end;
		//					indices[ currentIndex++ ] = index + start;
		//					indices[ currentIndex++ ] = nextIndex + end;

		//					indices[ currentIndex++ ] = nextIndex + start;
		//					indices[ currentIndex++ ] = nextIndex + end;
		//					indices[ currentIndex++ ] = index + start;
		//				}
		//			}
		//		}

		//		if( indices.Length != currentIndex )
		//			Log.Fatal( "SimpleMeshGenerator: GenerateSphere: indices.Length != currentIndex." );
		//	}
		//}

		//public static void GenerateIcoSphere( double radius, int subdivisions, bool insideOut, out Vec3[] positions, out int[] indices )
		//{
		//	var faceCount = 20;// 20 faces, icosahedronIndexes.Length / 3
		//	var vCount = 12;// 12 vertices
		//	var divCount = subdivisions;

		//	/*   /\
		//	    /  \
		//	   /____\
		//	  /\    /\
		//	 /  \  /  \
		//	/____\/____\  */

		//	while( divCount-- > 0 )
		//	{
		//		// 3 new vetices for each face
		//		vCount += faceCount * 3;
		//		// subdivide each face by 4 new ones
		//		faceCount *= 4;
		//	}

		//	var pList = new List<Vec3>( vCount );
		//	var nList = new List<Vec3>( vCount );
		//	var indexList = new List<int>( icosahedronIndexes );

		//	var t = ( 1.0 + Math.Sqrt( 5.0 ) ) / 2.0;

		//	//icosahedron
		//	nList.Add( new Vec3( -1, t, 0 ).GetNormalize() );
		//	nList.Add( new Vec3( 1, t, 0 ).GetNormalize() );
		//	nList.Add( new Vec3( -1, -t, 0 ).GetNormalize() );
		//	nList.Add( new Vec3( 1, -t, 0 ).GetNormalize() );

		//	nList.Add( new Vec3( 0, -1, t ).GetNormalize() );
		//	nList.Add( new Vec3( 0, 1, t ).GetNormalize() );
		//	nList.Add( new Vec3( 0, -1, -t ).GetNormalize() );
		//	nList.Add( new Vec3( 0, 1, -t ).GetNormalize() );

		//	nList.Add( new Vec3( t, 0, -1 ).GetNormalize() );
		//	nList.Add( new Vec3( t, 0, 1 ).GetNormalize() );
		//	nList.Add( new Vec3( -t, 0, -1 ).GetNormalize() );
		//	nList.Add( new Vec3( -t, 0, 1 ).GetNormalize() );

		//	foreach( var n in nList )
		//		pList.Add( n * radius );

		//	for( int iter = 0; iter < subdivisions; iter++ )
		//	{
		//		var newindices = new List<int>( indexList.Count / 3 * 12 );

		//		for( int i = 0; i < indexList.Count / 3; i++ )
		//		{
		//			var i1 = indexList[ i * 3 ];
		//			var i2 = indexList[ i * 3 + 1 ];
		//			int i3 = indexList[ i * 3 + 2 ];

		//			//Get Vertices and find mid point
		//			var v1 = pList[ i1 ];
		//			var v2 = pList[ i2 ];
		//			var v3 = pList[ i3 ];

		//			var e1 = ( v1 + v2 ) * 0.5f;
		//			var e2 = ( v2 + v3 ) * 0.5f;
		//			var e3 = ( v3 + v1 ) * 0.5f;

		//			var ie1 = pList.Count;
		//			var ie2 = pList.Count + 1;
		//			var ie3 = pList.Count + 2;

		//			//Push 3 new vertices
		//			e1.Normalize();
		//			nList.Add( e1 );
		//			pList.Add( e1 * radius );

		//			e2.Normalize();
		//			nList.Add( e2 );
		//			pList.Add( e2 * radius );

		//			e3.Normalize();
		//			nList.Add( e3 );
		//			pList.Add( e3 * radius );

		//			//Push 4 triangles
		//			newindices.Add( ie1 );
		//			newindices.Add( ie2 );
		//			newindices.Add( ie3 );

		//			newindices.Add( i1 );
		//			newindices.Add( ie1 );
		//			newindices.Add( ie3 );

		//			newindices.Add( i2 );
		//			newindices.Add( ie2 );
		//			newindices.Add( ie1 );

		//			newindices.Add( i3 );
		//			newindices.Add( ie3 );
		//			newindices.Add( ie2 );
		//		}

		//		indexList = newindices;
		//	}

		//	//tangents and texcoords
		//	{
		//		var rotationMatrix = new Mat3( 0, -1, 0, 1, 0, 0, 0, 0, 1 );
		//		//var pi2 = 1 / Math.PI * 2;
		//		//var pi1 = 1 / Math.PI;

		//		for( int n = 0; n < pList.Count; n++ )
		//		{
		//			var p = pList[ n ];
		//			var rot = Quat.FromDirectionZAxisUp( p.GetNormalize() ).ToMat3() * rotationMatrix;
		//			var tan = new Vec4( rot * Vec3.XAxis, 1 );
		//		}
		//	}

		//	positions = pList.ToArray();
		//	indices = indexList.ToArray();

		//	if( insideOut )
		//	{
		//		var idx = 0;
		//		while( idx < indices.Length )
		//		{
		//			var temp = indices[ idx + 1 ];
		//			indices[ idx + 1 ] = indices[ idx + 2 ];
		//			indices[ idx + 2 ] = temp;
		//			idx += 3;
		//		}
		//	}
		//}

		public static void GenerateIcoSphere( double radius, int subdivisions, bool insideOut, out Vector3[] positions, out int[] indices )
		{
			var faceCount = 20;// 20 faces, icosahedronIndexes.Length / 3
			var vCount = 12;// 12 vertices
			var divCount = subdivisions;

			/*   /\
			    /  \
			   /____\
			  /\    /\
			 /  \  /  \
			/____\/____\  */

			while( divCount-- > 0 )
			{
				// 3 new vetices for each face
				vCount += faceCount * 3;
				// subdivide each face by 4 new ones
				faceCount *= 4;
			}

			var pList = new List<Vector3>( vCount );
			var nList = new List<Vector3>( vCount );
			var indexList = new List<int>( icosahedronIndexes );

			var t = ( 1.0 + Math.Sqrt( 5.0 ) ) / 2.0;

			//icosahedron
			nList.Add( new Vector3( -1, t, 0 ).GetNormalize() );
			nList.Add( new Vector3( 1, t, 0 ).GetNormalize() );
			nList.Add( new Vector3( -1, -t, 0 ).GetNormalize() );
			nList.Add( new Vector3( 1, -t, 0 ).GetNormalize() );

			nList.Add( new Vector3( 0, -1, t ).GetNormalize() );
			nList.Add( new Vector3( 0, 1, t ).GetNormalize() );
			nList.Add( new Vector3( 0, -1, -t ).GetNormalize() );
			nList.Add( new Vector3( 0, 1, -t ).GetNormalize() );

			nList.Add( new Vector3( t, 0, -1 ).GetNormalize() );
			nList.Add( new Vector3( t, 0, 1 ).GetNormalize() );
			nList.Add( new Vector3( -t, 0, -1 ).GetNormalize() );
			nList.Add( new Vector3( -t, 0, 1 ).GetNormalize() );

			foreach( var n in nList )
				pList.Add( n * radius );

			//!!!!GC
			var cache = new Dictionary<long, int>();

			// Refine faces
			for( int iter = 0; iter < subdivisions; iter++ )
			{
				var newindices = new List<int>( indexList.Count / 3 * 12 );

				for( int i = 0; i < indexList.Count / 3; i++ )
				{
					var i1 = indexList[ i * 3 ];
					var i2 = indexList[ i * 3 + 1 ];
					int i3 = indexList[ i * 3 + 2 ];

					// get mid points
					int a = CreateMiddlePoint( pList, i1, i2, cache, radius, nList );
					int b = CreateMiddlePoint( pList, i2, i3, cache, radius, nList );
					int c = CreateMiddlePoint( pList, i3, i1, cache, radius, nList );

					//Push 4 triangles
					newindices.Add( i1 );
					newindices.Add( a );
					newindices.Add( c );

					newindices.Add( i2 );
					newindices.Add( b );
					newindices.Add( a );

					newindices.Add( i3 );
					newindices.Add( c );
					newindices.Add( b );

					newindices.Add( a );
					newindices.Add( b );
					newindices.Add( c );
				}

				indexList = newindices;
			}

			positions = pList.ToArray();
			indices = indexList.ToArray();

			if( insideOut )
			{
				var idx = 0;
				while( idx < indices.Length )
				{
					var temp = indices[ idx + 1 ];
					indices[ idx + 1 ] = indices[ idx + 2 ];
					indices[ idx + 2 ] = temp;
					idx += 3;
				}
			}
		}

		public static void GenerateIcoSphere( double radius, int subdivisions, bool insideOut, out Vector3F[] positions, out int[] indices )
		{
			Vector3[] positionsD;
			GenerateIcoSphere( radius, subdivisions, insideOut, out positionsD, out indices );
			positions = ToVector3F( positionsD );
		}

		public static void GenerateIcoSphere( double radius, int subdivisions, bool insideOut,
			out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			var faceCount = 20;// 20 faces, icosahedronIndexes.Length / 3
			var vCount = 12;// 12 vertices
			var divCount = subdivisions;

			/*   /\
			    /  \
			   /____\
			  /\    /\
			 /  \  /  \
			/____\/____\  */

			while( divCount-- > 0 )
			{
				// 3 new vetices for each face
				vCount += faceCount * 3;
				// subdivide each face by 4 new ones
				faceCount *= 4;
			}

			List<int> vertexIndexList; //viList.count == pList.Count (the vertex index of element in pList)
			var pList = new List<Vector3>( vCount );
			var nList = new List<Vector3>( vCount );
			var tList = new List<Vector4>( vCount );
			var txList = new List<Vector2>( vCount );
			var indexList = new List<int>( icosahedronIndexes );

			var t = ( 1.0 + Math.Sqrt( 5.0 ) ) / 2.0;

			//icosahedron
			nList.Add( new Vector3( -1, t, 0 ).GetNormalize() );
			nList.Add( new Vector3( 1, t, 0 ).GetNormalize() );
			nList.Add( new Vector3( -1, -t, 0 ).GetNormalize() );
			nList.Add( new Vector3( 1, -t, 0 ).GetNormalize() );

			nList.Add( new Vector3( 0, -1, t ).GetNormalize() );
			nList.Add( new Vector3( 0, 1, t ).GetNormalize() );
			nList.Add( new Vector3( 0, -1, -t ).GetNormalize() );
			nList.Add( new Vector3( 0, 1, -t ).GetNormalize() );

			nList.Add( new Vector3( t, 0, -1 ).GetNormalize() );
			nList.Add( new Vector3( t, 0, 1 ).GetNormalize() );
			nList.Add( new Vector3( -t, 0, -1 ).GetNormalize() );
			nList.Add( new Vector3( -t, 0, 1 ).GetNormalize() );

			foreach( var n in nList )
				pList.Add( n * radius );

			//!!!!GC
			var cache = new Dictionary<long, int>();

			// Refine faces
			for( int iter = 0; iter < subdivisions; iter++ )
			{
				var newindices = new List<int>( indexList.Count / 3 * 12 );

				for( int i = 0; i < indexList.Count / 3; i++ )
				{
					var i1 = indexList[ i * 3 ];
					var i2 = indexList[ i * 3 + 1 ];
					int i3 = indexList[ i * 3 + 2 ];

					// get mid points
					int a = CreateMiddlePoint( pList, i1, i2, cache, radius, nList );
					int b = CreateMiddlePoint( pList, i2, i3, cache, radius, nList );
					int c = CreateMiddlePoint( pList, i3, i1, cache, radius, nList );

					//Push 4 triangles
					newindices.Add( i1 );
					newindices.Add( a );
					newindices.Add( c );

					newindices.Add( i2 );
					newindices.Add( b );
					newindices.Add( a );

					newindices.Add( i3 );
					newindices.Add( c );
					newindices.Add( b );

					newindices.Add( a );
					newindices.Add( b );
					newindices.Add( c );
				}

				indexList = newindices;
			}

			//tangents and texcoords
			{
				var rotationMatrix = new Matrix3( 0, -1, 0, 1, 0, 0, 0, 0, 1 );

				for( int n = 0; n < nList.Count; n++ )
				{
					var p = nList[ n ];
					var rot = Quaternion.FromDirectionZAxisUp( p ).ToMatrix3() * rotationMatrix;
					Vector4 tan;
					if( insideOut )
						tan = new Vector4( rot * Vector3.XAxis, -1 );
					else
						tan = new Vector4( rot * -Vector3.XAxis, -1 );

					tList.Add( tan );

					var dir = SphericalDirection.FromVector( p );

					var tx = new Vector2(
						dir.Horizontal / ( MathEx.PI * 2 ) * 2,// [-1..1]
						-dir.Vertical / MathEx.PI + 0.5 );// [0..1]

					//var tx = new Vec2(
					//	( 0.5 - Math.Atan2( p.X, p.Z ) /  Math.PI * 2 ),
					//	( 0.5 - Math.Asin( p.Y ) / Math.PI ));

					txList.Add( tx );
				}
			}

			bool upUsed = false;
			bool downUsed = false;

			//all the vertices in pList now have the unique positions. No more unique positions.
			vertexIndexList = new List<int>(pList.Count);
			for( int i = 0; i < pList.Count; i++ )
				vertexIndexList.Add( i );


			// Correct poles.
			for( int i = 0; i < indexList.Count; i += 3 )
			{
				var vidx1 = indexList[ i ];
				var vidx2 = indexList[ i + 1 ];
				var vidx3 = indexList[ i + 2 ];

				var n1 = nList[ vidx1 ];
				var n2 = nList[ vidx2 ];
				var n3 = nList[ vidx3 ];

				if( Math.Abs( n1.Z ) == 1 )
				{
					var dirUp = SphericalDirection.FromVector( n1 );
					var mid = ( pList[ vidx2 ] + pList[ vidx3 ] ) * .5;
					var dir = SphericalDirection.FromVector( Vector3.Normalize( mid ) );
					dir.Vertical = dirUp.Vertical;

					var txt = new Vector2(
						dir.Horizontal / ( MathEx.PI * 2 ) * 2,// [-1..1]
						-dir.Vertical / MathEx.PI + 0.5 );// [0..1]

					var update = false;

					if( n1.Z == 1 && !upUsed )//up
					{
						update = true;
						upUsed = true;
					}
					if( n1.Z == -1 && !downUsed )//down
					{
						update = true;
						downUsed = true;
					}

					if( update )
					{
						txList[ vidx1 ] = txt;
					}
					else
					{
						// vertex1 is new pole, duplicate it and update the face.
						//newVertexIndex = pList.Count;
						indexList[ i ] = pList.Count;

						pList.Add( pList[ vidx1 ] );
						vertexIndexList.Add( vertexIndexList[vidx1] );
						nList.Add( nList[ vidx1 ] );
						tList.Add( tList[ vidx1 ] );
						txList.Add( txt );
					}
				}
				else if( Math.Abs( n2.Z ) == 1 )
				{
					var dirUp = SphericalDirection.FromVector( n2 );
					var mid = ( pList[ vidx1 ] + pList[ vidx3 ] ) * .5;
					var dir = SphericalDirection.FromVector( Vector3.Normalize( mid ) );
					dir.Vertical = dirUp.Vertical;

					var txt = new Vector2(
						dir.Horizontal / ( MathEx.PI * 2 ) * 2,// [-1..1]
						-dir.Vertical / MathEx.PI + 0.5 );// [0..1]

					var update = false;

					if( n1.Z == 1 && !upUsed )//up
					{
						update = true;
						upUsed = true;
					}
					if( n1.Z == -1 && !downUsed )//down
					{
						update = true;
						downUsed = true;
					}

					if( update )
					{
						txList[ vidx2 ] = txt;
					}
					else
					{
						// vertex2 is new pole, duplicate it and update the face.
						//newVertexIndex = pList.Count;
						indexList[ i + 1 ] = pList.Count;

						pList.Add( pList[ vidx2 ] );
						vertexIndexList.Add( vertexIndexList[vidx2] );
						nList.Add( nList[ vidx2 ] );
						tList.Add( tList[ vidx2 ] );
						txList.Add( txt );
					}
				}
				else if( Math.Abs( n3.Z ) == 1 )
				{
					var dirUp = SphericalDirection.FromVector( n3 );
					var mid = ( pList[ vidx1 ] + pList[ vidx2 ] ) * .5;
					var dir = SphericalDirection.FromVector( Vector3.Normalize( mid ) );
					dir.Vertical = dirUp.Vertical;

					var txt = new Vector2(
						dir.Horizontal / ( MathEx.PI * 2 ) * 2,// [-1..1]
						-dir.Vertical / MathEx.PI + 0.5 );// [0..1]

					var update = false;

					if( n1.Z == 1 && !upUsed )//up
					{
						update = true;
						upUsed = true;
					}
					if( n1.Z == -1 && !downUsed )//down
					{
						update = true;
						downUsed = true;
					}

					if( update )
					{
						txList[ vidx3 ] = txt;
					}
					else
					{
						// vertex3 is new pole, duplicate it and update the face.
						//newVertexIndex = pList.Count;
						indexList[ i + 2 ] = pList.Count;

						pList.Add( pList[ vidx3 ] );
						vertexIndexList.Add( vertexIndexList[vidx3] );
						nList.Add( nList[ vidx3 ] );
						tList.Add( tList[ vidx3 ] );
						txList.Add( txt );
					}
				}
			}

			//!!!!GC
			var correctionMap = new Dictionary<int, int>();

			//correct seam
			for( int i = indexList.Count - 3; i >= 0; i -= 3 )
			{
				var v0 = new Vector3( txList[ indexList[ i + 0 ] ], 0 );
				var v1 = new Vector3( txList[ indexList[ i + 1 ] ], 0 );
				var v2 = new Vector3( txList[ indexList[ i + 2 ] ], 0 );

				var cross = Vector3.Cross( v0 - v1, v2 - v1 );

				if( cross.Z <= 0 )//test if the face crosses a texture boundary.
				{
					for( var j = i; j < i + 3; j++ )
					{
						var index = indexList[ j ];
						var texCoord = txList[ index ];
						var shift = 0.0;

						if( texCoord.X >= .8 )
							shift = -2;

						//if( texCoord.X <= -.8 )
						//	shift = 2;

						if( shift == 0 )
							continue;

						if( correctionMap.ContainsKey( index ) )
						{
							indexList[ j ] = correctionMap[ index ];
						}
						else
						{
							texCoord.X += shift;

							txList.Add( texCoord );

							pList.Add( pList[ index ] );
							vertexIndexList.Add( vertexIndexList[index] ); 
							nList.Add( nList[ index ] );
							tList.Add( tList[ index ] );

							var correctedVertexIndex = txList.Count - 1;

							correctionMap.Add( index, correctedVertexIndex );

							indexList[ j ] = correctedVertexIndex;
						}
					}
				}
			}

			positions = pList.ToArray();
			normals = nList.ToArray();
			tangents = tList.ToArray();
			texCoords = txList.ToArray();
			indices = indexList.ToArray();

			if( insideOut )
			{
				var idx = 0;
				while( idx < indices.Length )
				{
					var temp = indices[ idx + 1 ];
					indices[ idx + 1 ] = indices[ idx + 2 ];
					indices[ idx + 2 ] = temp;
					idx += 3;
				}

				idx = 0;
				while( idx < normals.Length )
				{
					Vector3.Negate( ref normals[ idx ], out normals[ idx ] );
					idx++;
				}

				for( int n = 0; n < texCoords.Length; n++ )
					texCoords[ n ] = new Vector2( 1.0f - texCoords[ n ].X, texCoords[ n ].Y );
			}


			const int rawVerticesInFace = 3;
			faces = new Face[ indices.Length / rawVerticesInFace ];
			for( int i = 0; i < indices.Length; i += rawVerticesInFace )
			{
				faces[ i / 3 ] = new Face( new FaceVertex[]{
					new FaceVertex(vertexIndexList[ indices[ i ] ], indices[ i ]),
					new FaceVertex(vertexIndexList[ indices[ i + 1 ] ], indices[ i + 1 ]),
					new FaceVertex(vertexIndexList[ indices[ i + 2 ] ], indices[ i + 2 ]),
				} );
			}

			//faces = new Face[ indices.Length / rawVerticesInFace ];
			//int curTriangle = 0;
			//int curFace = 0;
			//FaceVertex[] faceTriangles = null;
			//for( int i = 0; i < indices.Length; i++ )
			//{
			//	if( faceTriangles == null )
			//	{
			//		faceTriangles = new FaceVertex[ rawVerticesInFace ];
			//		curTriangle = 0;
			//	}
			//	faceTriangles[ curTriangle++ ] = new FaceVertex( vertexIndexList[ indices[ i ] ], indices[ i ] );

			//	if( curTriangle == rawVerticesInFace )
			//	{
			//		faces[ curFace++ ] = new Face( faceTriangles );
			//		faceTriangles = null;
			//	}
			//}
		}

		// Get index of point in the middle of p1 and p2.
		private static int CreateMiddlePoint( List<Vector3> vList, int v1, int v2, Dictionary<long, int> cache, double r, List<Vector3> nList = null )
		{
			// Check if the middle point is already cached. If yes, return it.
			var firstIsSmaller = v1 < v2;
			long smallerIndex = firstIsSmaller ? v1 : v2;
			long greaterIndex = firstIsSmaller ? v2 : v1;
			var key = ( smallerIndex << 32 ) + greaterIndex;

			if( cache.TryGetValue( key, out int index ) )
				return index;

			index = vList.Count;

			// Middle point is not cached.
			var vertex1 = vList[ v1 ];
			var vertex2 = vList[ v2 ];

			var vec = ( vertex1 + vertex2 ) * .5;

			vec.Normalize();

			nList?.Add( vec );
			vList?.Add( vec * r );

			// Cache new middle point and return it.
			cache.Add( key, index );

			return index;
		}

		public static void GenerateIcoSphere( double radius, int subdivisions, bool insideOut,
			out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			Vector3[] positionsD;
			Vector3[] normalsD;
			Vector4[] tangentsD;
			Vector2[] texCoordsD;
			GenerateIcoSphere( radius, subdivisions, insideOut, out positionsD, out normalsD, out tangentsD, out texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}

		public static void GenerateSphere( double radius, int hSegments, int vSegments, bool insideOut, out Vector3F[] positions, out int[] indices )
		{
			GenerateSphere( radius, hSegments, vSegments, insideOut, out Vector3[] positionsD, out indices );
			positions = ToVector3F( positionsD );
		}

		public static void GenerateSphere( double radius, int hSegments, int vSegments, bool insideOut,
			out Vector3F[] positions, out Vector3F[] normals, out Vector4F[] tangents, out Vector2F[] texCoords, out int[] indices, out Face[] faces )
		{
			Vector3[] positionsD;
			Vector3[] normalsD;
			Vector4[] tangentsD;
			Vector2[] texCoordsD;
			GenerateSphere( radius, hSegments, vSegments, insideOut, out positionsD, out normalsD, out tangentsD, out texCoordsD, out indices, out faces );
			positions = ToVector3F( positionsD );
			normals = ToVector3F( normalsD );
			tangents = ToVector4F( tangentsD );
			texCoords = ToVector2F( texCoordsD );
		}
	}
}
