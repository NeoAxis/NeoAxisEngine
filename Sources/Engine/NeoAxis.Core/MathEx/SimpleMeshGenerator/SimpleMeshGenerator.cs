// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents a set of algorithms for generating simple 3D models.
	/// </summary>
	public static partial class SimpleMeshGenerator
	{
		static Matrix3 axisRotationXToY = new Matrix3( 0, 1, 0, -1, 0, 0, 0, 0, 1 ); 
		//static Matrix3 axisRotationXToY = new Matrix3( 0, 1, 0, 1, 0, 0, 0, 0, -1 ); // ToDo !!!! Раньше было так. Но тогда вверх ногами переворачивается 
		static Matrix3 axisRotationXToZ = new Matrix3( 0, 0, 1, 0, 1, 0, -1, 0, 0 );

		/////////////////////////////////////////

		public struct FaceVertex
		{
			public int Vertex;
			public int RawVertex;

			public FaceVertex( int vertex, int rawVertex )
			{
				Vertex = vertex;
				RawVertex = rawVertex;
			}
		}

		/////////////////////////////////////////

		public struct Face
		{
			public FaceVertex[] Triangles;

			public Face( FaceVertex[] triangles )
			{
				Triangles = triangles;
			}
		}

		/////////////////////////////////////////

		public static Matrix3 GetRotationMatrix( int axis )
		{
			switch( axis )
			{
			case 1: return axisRotationXToY;
			case 2: return axisRotationXToZ;
			default: return Matrix3.Identity;
			}
		}

		public static Vector3[] RotateByAxis( int axis, Vector3[] values )
		{
			if( axis == 1 )
			{
				var newValues = new Vector3[ values.Length ];
				for( int n = 0; n < values.Length; n++ )
					newValues[ n ] = axisRotationXToY * values[ n ];
				return newValues;
			}
			if( axis == 2 )
			{
				var newValues = new Vector3[ values.Length ];
				for( int n = 0; n < values.Length; n++ )
					newValues[ n ] = axisRotationXToZ * values[ n ];
				return newValues;
			}
			return values;
		}

		public static Vector4[] RotateByAxis( int axis, Vector4[] values )
		{
			if( axis == 1 )
			{
				var newValues = new Vector4[ values.Length ];
				for( int n = 0; n < values.Length; n++ )
					newValues[ n ] = new Vector4( axisRotationXToY * values[ n ].ToVector3(), values[ n ].W );
				return newValues;
			}
			if( axis == 2 )
			{
				var newValues = new Vector4[ values.Length ];
				for( int n = 0; n < values.Length; n++ )
					newValues[ n ] = new Vector4( axisRotationXToZ * values[ n ].ToVector3(), values[ n ].W );
				return newValues;
			}
			return values;
		}

		static Vector2F[] ToVector2F( Vector2[] source )
		{
			if( source != null )
				return MathUtility.ToVector2FArray( source );
			return null;
		}

		static Vector3F[] ToVector3F( Vector3[] source )
		{
			if( source != null )
				return MathUtility.ToVector3FArray( source );
			return null;
		}

		static Vector4F[] ToVector4F( Vector4[] source )
		{
			if( source != null )
				return MathUtility.ToVector4FArray( source );
			return null;
		}

		public static Component_Mesh.StructureClass CreateMeshStructure( Face[] faces )
		{
			var result = new Component_Mesh.StructureClass();
			int vertexMaxIndex = 0;

			//faces
			result.Faces = new Component_Mesh.StructureClass.Face[ faces.Length ];
			for( int nFace = 0; nFace < faces.Length; nFace++ )
			{
				var face = faces[ nFace ];

				var triangles2 = new Component_Mesh.StructureClass.FaceVertex[ face.Triangles.Length ];
				for( int n = 0; n < triangles2.Length; n++ )
				{
					var faceVertex = face.Triangles[ n ];
					triangles2[ n ] = new Component_Mesh.StructureClass.FaceVertex( faceVertex.Vertex, 0, faceVertex.RawVertex );

					vertexMaxIndex = Math.Max( vertexMaxIndex, faceVertex.Vertex );
				}

				result.Faces[ nFace ] = new Component_Mesh.StructureClass.Face( triangles2, null, 0 );
			}

			//edges
			var edges = new ESet<Vector2I>( vertexMaxIndex * 3 );
			for( int nFace = 0; nFace < faces.Length; nFace++ )
			{
				var face = faces[ nFace ];

				var edgeCounts = new Dictionary<Vector2I, int>( face.Triangles.Length );

				for( int nTriangle = 0; nTriangle < face.Triangles.Length / 3; nTriangle++ )
				{
					var faceVertex0 = face.Triangles[ nTriangle * 3 + 0 ];
					var faceVertex1 = face.Triangles[ nTriangle * 3 + 1 ];
					var faceVertex2 = face.Triangles[ nTriangle * 3 + 2 ];

					void AddEdge( int vertex1, int vertex2 )
					{
						int v1, v2;
						if( vertex1 > vertex2 )
						{
							v1 = vertex2;
							v2 = vertex1;
						}
						else
						{
							v1 = vertex1;
							v2 = vertex2;
						}
						var key = new Vector2I( v1, v2 );
						edgeCounts.TryGetValue( key, out var count );
						edgeCounts[ key ] = count + 1;
					}

					AddEdge( faceVertex0.Vertex, faceVertex1.Vertex );
					AddEdge( faceVertex1.Vertex, faceVertex2.Vertex );
					AddEdge( faceVertex2.Vertex, faceVertex0.Vertex );
				}

				foreach( var pair in edgeCounts )
				{
					if( pair.Value == 1 )
					{
						var edge = pair.Key;
						edges.AddWithCheckAlreadyContained( new Vector2I( edge.X, edge.Y ) );
					}
				}

				//for( int nTriangle = 0; nTriangle < face.Triangles.Length / 3; nTriangle++ )
				//{
				//	var faceVertex0 = face.Triangles[ nTriangle * 3 + 0 ];
				//	var faceVertex1 = face.Triangles[ nTriangle * 3 + 1 ];
				//	var faceVertex2 = face.Triangles[ nTriangle * 3 + 2 ];

				//	void AddEdge( int vertex1, int vertex2 )
				//	{
				//		int v1, v2;
				//		if( vertex1 > vertex2 )
				//		{
				//			v1 = vertex2;
				//			v2 = vertex1;
				//		}
				//		else
				//		{
				//			v1 = vertex1;
				//			v2 = vertex2;
				//		}
				//		edges.AddWithCheckAlreadyContained( new Vector2I( v1, v2 ) );
				//	}

				//	AddEdge( faceVertex0.Vertex, faceVertex1.Vertex );
				//	AddEdge( faceVertex1.Vertex, faceVertex2.Vertex );
				//	AddEdge( faceVertex2.Vertex, faceVertex0.Vertex );
				//}
			}
			result.Edges = edges.Select( e => new Component_Mesh.StructureClass.Edge( e.X, e.Y ) ).ToArray();

			//vertices
			result.Vertices = new Component_Mesh.StructureClass.Vertex[ vertexMaxIndex + 1 ];

			return result;
		}

		#region CommonPipeBuilder

		//ToDo : Сглаживание и индексация(сейчас все вертексы деиндексированы).

		static Vector2[][] CalculateUV( Point[][] positionsLegs, bool flipU, bool flipV )
		{
			int lastLegIndex = positionsLegs.Length - 1;
			int legPointCount = positionsLegs[ 0 ].Length;

			var texCoords = new Vector2[ positionsLegs.Length ][]; //значение U,V координаты для каждой точки в каждом leg. 

			texCoords[ 0 ] = new Vector2[ legPointCount ];
			for( int leg = 1; leg < positionsLegs.Length; leg++ )
			{
				var positionsA = positionsLegs[ leg - 1 ];
				var positionsB = positionsLegs[ leg ];

				texCoords[ leg ] = new Vector2[ legPointCount ];
				for( int i = 0; i < legPointCount; i++ )
				{
					double len = ( positionsB[ i ].Position - positionsA[ i ].Position ).Length();
					texCoords[ leg ][ i ].X = texCoords[ leg - 1 ][ i ].X + len;
				}
			}

			for( int leg = 0; leg < positionsLegs.Length; leg++ )
			{
				var pos = positionsLegs[ leg ];
				for( int i = 1; i < legPointCount; i++ )
					texCoords[ leg ][ i ].Y = texCoords[ leg ][ i - 1 ].Y + ( pos[ i ].Position - pos[ i - 1 ].Position ).Length();
			}

			for( int leg = 0; leg < positionsLegs.Length; leg++ )
			{
				double totalVLength = texCoords[ leg ][ legPointCount - 1 ].Y;
				for( int i = 0; i < legPointCount; i++ )
					texCoords[ leg ][ i ].Y /= totalVLength;
			}

			for( int i = 0; i < legPointCount; i++ )
			{
				double totalULength = texCoords[ positionsLegs.Length - 1 ][ i ].X;
				for( int leg = 0; leg < positionsLegs.Length; leg++ )
					texCoords[ leg ][ i ].X /= totalULength;
			}

			if( flipU )
				for( int leg = 0; leg < positionsLegs.Length; leg++ )
					for( int i = 0; i < legPointCount; i++ )
						texCoords[ leg ][ i ].X = 1 - texCoords[ leg ][ i ].X;
			if( flipV )
				for( int leg = 0; leg < positionsLegs.Length; leg++ )
					for( int i = 0; i < legPointCount; i++ )
						texCoords[ leg ][ i ].Y = 1 - texCoords[ leg ][ i ].Y;

			return texCoords;
		}

		static Vector3 CalculateNormal( Vector3 p0, Vector3 p1, Vector3 p2 )
		{
			return Vector3.Cross( p1 - p0, p2 - p0 ).GetNormalize();
		}

		static Vector3 GetAxisVector( int axis )
		{
			switch( axis )
			{
			case 0: return new Vector3( 1, 0, 0 );
			case 1: return new Vector3( 0, 1, 0 );
			case 2: return new Vector3( 0, 0, 1 );
			default: throw new Exception();
			}
		}

		struct Point
		{
			public Point( Vector3 position, int vertex )
			{
				Position = position;
				Vertex = vertex;
			}
			public Vector3 Position;
			public int Vertex;

			public override string ToString()
			{
				return Position + " , " + Vertex;
			}
		}

		//ToDo ? Сделать с параметром axis, чтобы не вызывать потом ToAxis?
		//Когда circumference 360 градусов, первая и последняя точки совпадают.
		static Point[] GenerateCircleZ( int pointCount, double radius, Degree circumference, ref int nextVertex, double startAngle = 0 )
		{
			if( 360 < circumference )
				throw new Exception();
			bool loop = circumference == 360;
			var ret = new Point[ pointCount ];

			double angleStep = circumference.InRadians().ToDouble() / ( pointCount - 1 ); //количество точек на 1 больше чем интервалов между точками.
			for( int n = 0; n < pointCount; n++ )
			{
				double angle = startAngle + angleStep * n;
				ret[ n ].Position = new Vector3( Math.Cos( angle ) * radius, Math.Sin( angle ) * radius, 0 );
				ret[ n ].Vertex = nextVertex + n;
			}

			nextVertex += pointCount;
			if( loop )
			{
				ret[ pointCount - 1 ].Vertex = ret[ 0 ].Vertex;
				nextVertex--;
			}
			return ret;
		}

		static Point[] GenerateLine( int pointCount, Vector3 start, Vector3 step, ref int nextVertex )
		{
			var ret = new Point[ pointCount ];
			for( int n = 0; n < pointCount; n++ )
			{
				ret[ n ].Position = start;
				ret[ n ].Vertex = nextVertex++;
				start += step;
			}
			return ret;
		}

		//Фигуру в плоскости Z=0, поворачивает в другую плоскость.
		static void FromZAxisToAxis( Point[] polygon, int normalAxis )
		{
			switch( normalAxis )
			{
			case 0:
				for( int i = 0; i < polygon.Length; i++ )
					polygon[ i ] = new Point( new Vector3( 0, polygon[ i ].Position.X, polygon[ i ].Position.Y ), polygon[ i ].Vertex );
				break;
			case 1:
				for( int i = 0; i < polygon.Length; i++ )
					polygon[ i ] = new Point( new Vector3( -polygon[ i ].Position.X, 0, polygon[ i ].Position.Y ), polygon[ i ].Vertex );
				break;
			case 2:
				for( int i = 0; i < polygon.Length; i++ )
					polygon[ i ] = new Point( new Vector3( polygon[ i ].Position.X, polygon[ i ].Position.Y, 0 ), polygon[ i ].Vertex );
				break;
			}
		}

		//Заменяет координатные оси значение в axis2 становится осью 2. Результаты в axis0,axis1 это новые оси 0,1, такие что новая система координат остается правосторонней.
		static void ReplaceAxes( out int axis0, out int axis1, int axis2 )
		{
			switch( axis2 )
			{
			case 0:
				axis0 = 1;
				axis1 = 2;
				break;
			case 1:
				axis0 = 2;
				axis1 = 0;
				break;
			case 2:
				axis0 = 0;
				axis1 = 1;
				break;
			default: throw new Exception();
			}
		}

		static Point[] TransformPoints( Point[] points, Matrix4 m, int vertexOffset )
		{
			var ret = new Point[ points.Length ];
			for( int i = 0; i < points.Length; i++ )
			{
				ret[ i ].Position = m * points[ i ].Position;
				ret[ i ].Vertex = points[ i ].Vertex + vertexOffset;
			}

			return ret;
		}
		static Point[][] GenerateLegs( Point[] points, Matrix4[] m, int vertexOffsetIncrement, bool loopOfLegs )
		{
			var ret = new Point[ m.Length ][];
			int vertexOffset = 0;
			for( int i = 0; i < m.Length; i++, vertexOffset += vertexOffsetIncrement )
				ret[ i ] = TransformPoints( points, m[ i ], ( loopOfLegs && i == m.Length - 1 ? 0 : vertexOffset ) );
			return ret;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="matrixCount">Количество матриц в return value. Оно на 1 больше чем интервалов.</param>
		/// <param name="circumference"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		static Matrix4[] GetTransformsOfRotationCircle( int matrixCount, int axis, Degree circumference, Vector3 offset )
		{
			Debug.Assert( 1 < matrixCount );

			var offsetM = Matrix4.FromTranslate( offset );

			Matrix4 GetMatrix( double angle )
			{
				switch( axis )
				{
				case 0: return Matrix3.FromRotateByX( angle ).ToMatrix4() * offsetM;
				case 1: return Matrix3.FromRotateByY( angle ).ToMatrix4() * offsetM;
				case 2: return Matrix3.FromRotateByZ( angle ).ToMatrix4() * offsetM;
				}
				throw new Exception();
			}

			var ret = new Matrix4[ matrixCount ];
			double angleStep = circumference.InRadians().ToDouble() / ( matrixCount - 1 ); //количество отрезков на 1 меньше, чем количество концов отрезков.
			for( int i = 0; i < matrixCount; i++ )
			{
				double angle = angleStep * i;

				ret[ i ] = GetMatrix( angle );
			}
			return ret;
		}

		static Matrix4[] GetTransformsOfLinePath( int matrixCount, Vector3 start, Vector3 end )
		{
			Vector3 shift = end - start;
			Debug.Assert( 0 < matrixCount );
			double len = shift.Length();
			if( len < 1e-5 || matrixCount == 1 )
				return new Matrix4[] { Matrix4.Identity };

			var ret = new Matrix4[ matrixCount ];
			Vector3 shiftNormalized = shift.GetNormalize();
			double step = len / ( matrixCount - 1 ); //количество отрезков на 1 меньше, чем количество концов отрезков.
			for( int i = 0; i < matrixCount; i++ )
				ret[ i ] = Matrix4.FromTranslate( start + shiftNormalized * step * i );
			return ret;
		}

		public static void GetEmptyData( out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
		{
			positions = new Vector3[ 0 ];
			normals = new Vector3[ 0 ];
			tangents = new Vector4[ 0 ];
			texCoords = new Vector2[ 0 ];
			indices = new int[ 0 ];
			faces = null;
		}

		class CommonPipeBuilder
		{
			public CommonPipeBuilder( int rawVertexCount, int faceCount, bool smoothNormals = false )
			{
				curPos = 0;
				curTriangle = 0;
				curFace = 0;
				positions = new Vector3[ rawVertexCount ];
				indices = new int[ rawVertexCount ];
				normals = new Vector3[ rawVertexCount ];
				tangents = new Vector4[ rawVertexCount ];
				texCoords = new Vector2[ rawVertexCount ];
				faces = new Face[ faceCount ];
				if( smoothNormals )
					smoothingGroups = new int[ rawVertexCount ];
			}

			public int startIndex = 0;
			public int curPos;
			int curTriangle;
			int curFace;
			public Vector3[] positions;
			public int[] indices;
			public Vector3[] normals;
			public Face[] faces;
			public Vector4[] tangents;
			public Vector2[] texCoords;
			public int[] smoothingGroups;

			FaceVertex[] faceTrianglesArray;

			public void BeginFace( int count )
			{
				faceTrianglesArray = new FaceVertex[ count ];
				curTriangle = 0;
			}

			public void EndFace()
			{
				if( faceTrianglesArray.Length != curTriangle )
					throw new Exception();
				faces[ curFace++ ] = new Face( faceTrianglesArray );

				faceTrianglesArray = null;
				curTriangle = 0;
			}

			public void GetData( out Vector3[] positions, out Vector3[] normals, out Vector4[] tangents, out Vector2[] texCoords, out int[] indices, out Face[] faces )
			{
				if( smoothingGroups != null )
					SmoothNormals();
				if( this.positions.Length != curPos )
					throw new Exception( "Wrong vertex count" );
				positions = this.positions;
				normals = this.normals;
				tangents = this.tangents;
				texCoords = this.texCoords;
				indices = this.indices;

				if( curFace != this.faces.Length )
					throw new Exception( "Wrong face count" );
				faces = this.faces;
			}

			void SmoothNormals()
			{

				int maxVertex = int.MinValue;
				foreach( var f in faces )
				{
					for( int i = 0; i < f.Triangles.Length; i++ )
					{
						if( maxVertex < f.Triangles[ i ].Vertex )
							maxVertex = f.Triangles[ i ].Vertex;
					}
				}

				if( maxVertex == int.MinValue )
					return;

				var s = new (Vector3 normal, int count)[ maxVertex + 1 ];

				var groups = new HashSet<int>();
				for( int i = 0; i < smoothingGroups.Length; i++ )
				{
					if( smoothingGroups[ i ] != 0 )
						groups.Add( smoothingGroups[ i ] );
				}


				foreach( int curGroup in groups )
				{
					for( int i = 0; i < s.Length; i++ )
						s[ i ] = (Vector3.Zero, 0);

					foreach( var f in this.faces )
					{
						for( int i = 0; i < f.Triangles.Length; i++ )
						{
							ref var fv = ref f.Triangles[ i ];
							if( curGroup == smoothingGroups[ fv.RawVertex ] )
							{
								s[ fv.Vertex ].normal += normals[ fv.RawVertex ];
								s[ fv.Vertex ].count++;
							}
						}
					}

					for( int i = 0; i < s.Length; i++ )
						s[ i ].normal = ( s[ i ].normal / s[ i ].count ).GetNormalize();

					foreach( var f in faces )
					{
						for( int i = 0; i < f.Triangles.Length; i++ )
						{
							ref var fv = ref f.Triangles[ i ];
							if( curGroup == smoothingGroups[ fv.RawVertex ] )
								normals[ fv.RawVertex ] = s[ fv.Vertex ].normal;
						}
					}
				}
			}

			public void AddTriangle(
				Point p0, Point p1, Point p2,
				Vector4 t0, Vector4 t1, Vector4 t2,
				Vector2 texCoord0, Vector2 texCoord1, Vector2 texCoord2,
				bool insideOut, int smoothingGroup = 0
				)
			{
				int curPos1 = curPos + 1;
				int curPos2 = curPos + 2;
				if( insideOut )
				{
					var nA = CalculateNormal( p0.Position, p2.Position, p1.Position );

					positions[ curPos ] = p0.Position;
					indices[ curPos ] = startIndex++;
					normals[ curPos ] = nA;
					tangents[ curPos ] = t0;
					texCoords[ curPos ] = texCoord0;

					positions[ curPos1 ] = p2.Position;
					indices[ curPos1 ] = startIndex++;
					normals[ curPos1 ] = nA;
					tangents[ curPos1 ] = t2;
					texCoords[ curPos1 ] = texCoord2;

					positions[ curPos2 ] = p1.Position;
					indices[ curPos2 ] = startIndex++;
					normals[ curPos2 ] = nA;
					tangents[ curPos2 ] = t1;
					texCoords[ curPos2 ] = texCoord1;

					int nextIndex = startIndex - 3;

					faceTrianglesArray[ curTriangle++ ] = new FaceVertex( p0.Vertex, nextIndex++ );
					faceTrianglesArray[ curTriangle++ ] = new FaceVertex( p2.Vertex, nextIndex++ );
					faceTrianglesArray[ curTriangle++ ] = new FaceVertex( p1.Vertex, nextIndex++ );
				}
				else
				{
					var nA = CalculateNormal( p0.Position, p1.Position, p2.Position );

					positions[ curPos ] = p0.Position;
					indices[ curPos ] = startIndex++;
					normals[ curPos ] = nA;
					tangents[ curPos ] = t0;
					texCoords[ curPos ] = texCoord0;

					positions[ curPos1 ] = p1.Position;
					indices[ curPos1 ] = startIndex++;
					normals[ curPos1 ] = nA;
					tangents[ curPos1 ] = t1;
					texCoords[ curPos1 ] = texCoord1;

					positions[ curPos2 ] = p2.Position;
					indices[ curPos2 ] = startIndex++;
					normals[ curPos2 ] = nA;
					tangents[ curPos2 ] = t2;
					texCoords[ curPos2 ] = texCoord2;

					int nextIndex = startIndex - 3;

					faceTrianglesArray[ curTriangle++ ] = new FaceVertex( p0.Vertex, nextIndex++ );
					faceTrianglesArray[ curTriangle++ ] = new FaceVertex( p1.Vertex, nextIndex++ );
					faceTrianglesArray[ curTriangle++ ] = new FaceVertex( p2.Vertex, nextIndex++ );

				}

				if( smoothingGroups != null )
				{
					smoothingGroups[ curPos ] = smoothingGroup;
					smoothingGroups[ curPos1 ] = smoothingGroup;
					smoothingGroups[ curPos2 ] = smoothingGroup;
				}

				curPos += 3;
			}

			public void AddQuad(
				Point p0, Point p1, Point p2, Point p3,
				Vector4 t0, Vector4 t1, Vector4 t2, Vector4 t3,
				Vector2 texCoord0, Vector2 texCoord1, Vector2 texCoord2, Vector2 texCoord3,
				bool insideOut, int smoothingGroup = 0, bool singleFace = false
				)
			{
				if( singleFace )
					BeginFace( 6 );
				AddTriangle( p0, p1, p2, t0, t1, t2, texCoord0, texCoord1, texCoord2, insideOut, smoothingGroup );
				AddTriangle( p2, p3, p0, t2, t3, t0, texCoord2, texCoord3, texCoord0, insideOut, smoothingGroup );
				if( singleFace )
					EndFace();
			}

			//Для зацикленных фигур loopInLeg==true, в переданных параметрах первая точка должна быть продублирована. Если зацикленность между leg, то первую leg надо продублировать в конце.А флажки loopInLeg, bool loopOfLegs только чтобы вертексы сделать общими 
			public void AddLegs( Point[][] positionsLegs, bool insideOut, int smoothingGroup = 0, bool flipU = false, bool flipV = false )
			{
				if( positionsLegs.Length < 2 )
					return;
				int legPointCount = positionsLegs[ 0 ].Length;
				Debug.Assert( 2 <= legPointCount );


				for( int i = 0; i < positionsLegs.Length; i++ )
				{
					if( legPointCount != positionsLegs[ i ].Length )
						throw new Exception(); //ToDo : ...
				}

				//через A,B обозначаются текущий и следующий leg. Номерами обозначаются точки внутри leg

				Vector2[][] tx = CalculateUV( positionsLegs, flipU, flipV );

				for( int leg = 0; leg < positionsLegs.Length - 1; leg++ )
				{
					int legA = leg;
					int legB = leg + 1;
					var positionsA = positionsLegs[ legA ];
					var positionsB = positionsLegs[ legB ];

					for( int i = 0; i < legPointCount - 1; i++ )
					{
						int i0 = i;
						int i1 = i + 1;

						//Пока без индексации(бес сжатия точек). Если (A0,A1) не паралелен (B0,B1), то разные нормали и нельзя сжать. Либо сжимать(индексировать потом), либо здесь проверять на паралельность.

						ref Point a0 = ref positionsA[ i0 ];
						ref Point a1 = ref positionsA[ i1 ];

						ref Point b0 = ref positionsB[ i0 ];
						ref Point b1 = ref positionsB[ i1 ];

						Vector4 t0 = new Vector4( b0.Position - a0.Position, -1 );
						Vector4 t1 = new Vector4( b1.Position - a1.Position, -1 );

						AddQuad(
							a0, a1, b1, b0,
							t0, t1, t1, t0,
							tx[ legA ][ i0 ], tx[ legA ][ i1 ], tx[ legB ][ i1 ], tx[ legB ][ i0 ],
							insideOut, smoothingGroup, true
							);
					}
				}
			}

			public void CalculateTexCoordForFlatPolygon2( Point[] points1, Point[] points2, Vector3 normal, Vector3 uDirection, out Vector2[] texCoord1, out Vector2[] texCoord2, out Vector4 tangent )
			{
				//ToDo !!!!! Uncomment
				Debug.Assert( Math.Abs( Vector3.Dot( normal, uDirection ) ) < 1e-5 ); //ToDo ? Если uDirection не перпендикулярен normal, делать перпендикулярным?

				Vector3 vDirection = -Vector3.Cross( normal, uDirection ).GetNormalize();

				texCoord1 = new Vector2[ points1.Length ];
				texCoord2 = new Vector2[ points2.Length ];

				for( int i = 0; i < points1.Length; i++ )
					texCoord1[ i ] = ToUV( points1[ i ].Position, uDirection, vDirection );
				for( int i = 0; i < points2.Length; i++ )
					texCoord2[ i ] = ToUV( points2[ i ].Position, uDirection, vDirection );

				Vector2 max = new Vector2( double.MinValue, double.MinValue );
				Vector2 min = new Vector2( double.MaxValue, double.MaxValue );
				for( int i = 0; i < texCoord1.Length; i++ )
				{
					max = Vector2.Max( texCoord1[ i ], max );
					min = Vector2.Min( texCoord1[ i ], min );
				}
				for( int i = 0; i < texCoord2.Length; i++ )
				{
					max = Vector2.Max( texCoord2[ i ], max );
					min = Vector2.Min( texCoord2[ i ], min );
				}
				Vector2 range = max - min;

				for( int i = 0; i < texCoord1.Length; i++ )
					texCoord1[ i ] = new Vector2( ( texCoord1[ i ].X - min.X ) / range.X, ( texCoord1[ i ].Y - min.Y ) / range.Y );
				for( int i = 0; i < texCoord2.Length; i++ )
					texCoord2[ i ] = new Vector2( ( texCoord2[ i ].X - min.X ) / range.X, ( texCoord2[ i ].Y - min.Y ) / range.Y );
				tangent = new Vector4( uDirection, -1 );
			}

			public void CalculateTexCoordForFlatPolygon2( Point[][] points, Vector3 normal, Vector3 uDirection, out Vector2[][] texCoord, out Vector4 tangent )
			{
				Debug.Assert( Math.Abs( Vector3.Dot( normal, uDirection ) ) < 1e-5 ); //ToDo ? Если uDirection не перпендикулярен normal, делать перпендикулярным?

				Vector3 vDirection = -Vector3.Cross( normal, uDirection ).GetNormalize();

				texCoord = new Vector2[ points.Length ][];
				for( int i = 0; i < texCoord.Length; i++ )
					texCoord[ i ] = new Vector2[ points[ i ].Length ];

				for( int i = 0; i < points.Length; i++ )
					for( int j = 0; j < points[ i ].Length; j++ )
						texCoord[ i ][ j ] = ToUV( points[ i ][ j ].Position, uDirection, vDirection );

				Vector2 max = new Vector2( double.MinValue, double.MinValue );
				Vector2 min = new Vector2( double.MaxValue, double.MaxValue );

				for( int i = 0; i < texCoord.Length; i++ )
					for( int j = 0; j < texCoord[ i ].Length; j++ )
					{
						max = Vector2.Max( texCoord[ i ][ j ], max );
						min = Vector2.Min( texCoord[ i ][ j ], min );
					}

				Vector2 range = max - min;

				for( int i = 0; i < texCoord.Length; i++ )
				{
					Vector2[] texCoordI = texCoord[ i ];
					for( int j = 0; j < texCoordI.Length; j++ )
						texCoordI[ j ] = new Vector2( ( texCoordI[ j ].X - min.X ) / range.X, ( texCoordI[ j ].Y - min.Y ) / range.Y );
				}
				tangent = new Vector4( uDirection, -1 );
			}

			//When the bottom is paralel to Z=0
			public void CalculateTexCoordForFlatPolygon2Z( Vector3[] outerPoints, Vector3[] innerPoints, bool insideOut, out Vector2[] outerTexCoord, out Vector2[] innerTexCoord, out Vector4 tangent )
			{
				Vector3 max = new Vector3( double.MinValue, double.MinValue, double.MinValue );
				Vector3 min = new Vector3( double.MaxValue, double.MaxValue, double.MaxValue );
				for( int i = 0; i < outerPoints.Length; i++ )
				{
					max = Vector3.Max( outerPoints[ i ], max );
					min = Vector3.Min( outerPoints[ i ], min );
				}
				Vector3 range = max - min;

				Vector2 ToTexCoord( Vector3 pt ) => insideOut
					? new Vector2( ( pt.X - min.X ) / range.X, 1 - ( pt.Y - min.Y ) / range.Y )
					: new Vector2( ( pt.X - min.X ) / range.X, ( pt.Y - min.Y ) / range.Y );

				outerTexCoord = new Vector2[ outerPoints.Length ];
				innerTexCoord = new Vector2[ innerPoints.Length ];

				for( int i = 0; i < outerPoints.Length; i++ )
					outerTexCoord[ i ] = ToTexCoord( outerPoints[ i ] );
				for( int i = 0; i < innerPoints.Length; i++ )
					innerTexCoord[ i ] = ToTexCoord( innerPoints[ i ] );
				tangent = new Vector4( 1, 0, 0, -1 );
			}

			static Vector2 ToUV( Vector3 p, Vector3 uDirection, Vector3 vDirection ) => new Vector2( Vector3.Dot( uDirection, p ), Vector3.Dot( vDirection, p ) );

			public void CalculateTexCoordForFlatPolygon( Point[] points, Vector3 normal, Vector3 uDirection, out Vector2[] texCoord, out Vector4 tangent )
			{
				Debug.Assert( Math.Abs( Vector3.Dot( normal, uDirection ) ) < 1e-5 );

				Vector3 vDirection = -Vector3.Cross( normal, uDirection ).GetNormalize();
				texCoord = new Vector2[ points.Length ];


				for( int i = 0; i < points.Length; i++ )
					texCoord[ i ] = ToUV( points[ i ].Position, uDirection, vDirection );
				Vector2 max = new Vector2( double.MinValue, double.MinValue );
				Vector2 min = new Vector2( double.MaxValue, double.MaxValue );
				for( int i = 0; i < texCoord.Length; i++ )
				{
					max = Vector2.Max( texCoord[ i ], max );
					min = Vector2.Min( texCoord[ i ], min );
				}
				Vector2 range = max - min;

				for( int i = 0; i < texCoord.Length; i++ )
					texCoord[ i ] = new Vector2( ( texCoord[ i ].X - min.X ) / range.X, ( texCoord[ i ].Y - min.Y ) / range.Y );
				tangent = new Vector4( uDirection, -1 );
			}

			//ToDO ? передавать сразу Vector3 uDirection ?
			//Только для случая когда outerPoints.Length == innerPoints.Length. Строит прямоугольники между одноименными парами точек.
			public void AddPolygon2( Point[] outerPoints, Point[] innerPoints, bool insideOut, bool flipU )
			{
				Debug.Assert( outerPoints.Length == innerPoints.Length );

				Vector3 uDirection = ( outerPoints[ 0 ].Position - innerPoints[ 0 ].Position ).GetNormalize();
				if( flipU )
					uDirection = -uDirection;
				Vector3 normal = CalculateNormal( innerPoints[ 0 ].Position, outerPoints[ 0 ].Position, outerPoints[ 1 ].Position );
				if( insideOut )
					normal = -normal;
				CalculateTexCoordForFlatPolygon2( outerPoints, innerPoints, normal, uDirection, out Vector2[] outerTex, out Vector2[] innerTex, out Vector4 tangent );

				//CalculateTexCoordForFlatPolygonWithHoleZ( outerPoints, innerPoints, insideOut, out Vector2[] outerTex, out Vector2[] innerTex, out Vector4 tangent );

				BeginFace( 6 * ( outerPoints.Length - 1 ) );
				for( int i = 0; i < outerPoints.Length - 1; i++ )
				{
					int i0 = i;
					int i1 = i + 1;

					AddQuad(
						innerPoints[ i0 ], outerPoints[ i0 ], outerPoints[ i1 ], innerPoints[ i1 ],
						tangent, tangent, tangent, tangent,
						innerTex[ i0 ], outerTex[ i0 ], outerTex[ i1 ], innerTex[ i1 ],
						insideOut
					);
				}

				EndFace();
			}

			public void CalculateNormalAndU( Point p0, Point p1, Point p2, bool insideOut, bool flipU, out Vector3 normal, out Vector3 uDirection )
			{
				uDirection = ( p0.Position - p2.Position ).GetNormalize();
				if( flipU )
					uDirection = -uDirection;
				normal = CalculateNormal( p0.Position, p1.Position, p2.Position );
				if( insideOut )
					normal = -normal;
			}

			//Only for convex polygons.
			//Триангуляция прямоугольниками. Между двумя половинками полигона строятся четырехугольники. Если число сегментов не четное, то вместо одного четырехугольника один треугольник.
			public void AddPolygonSliced( Point[] points, bool insideOut, bool flipU )
			{
				int pointCount = points.Length;
				if( points[ 0 ].Vertex == points[ points.Length - 1 ].Vertex ) //loop
					pointCount--;


				int pt2 = pointCount == 3 ? 2 : ( pointCount % 2 == 0 ? pointCount - 1 : pointCount - 2 );
				CalculateNormalAndU( points[ 0 ], points[ 1 ], points[ pt2 ], insideOut, flipU, out Vector3 normal, out Vector3 uDirection );
				CalculateTexCoordForFlatPolygon( points, normal, uDirection, out Vector2[] texCoord, out Vector4 tangent );

				bool additionalTriangle = false;
				if( pointCount % 2 == 1 )
				{
					additionalTriangle = true;
					pointCount--;
				}
				int quadCount = pointCount / 2 - 1;

				BeginFace( ( additionalTriangle ? 3 : 0 ) + quadCount * 6 );
				if( additionalTriangle ) //при нечетном один треугольник, остальные четырехугольники.
				{
					AddTriangle(
						points[ 0 ], points[ pointCount - 1 ], points[ pointCount ],
						tangent, tangent, tangent,
						texCoord[ 0 ], texCoord[ pointCount - 1 ], texCoord[ pointCount ],
						insideOut
						);
				}

				for( int i = 0; i < quadCount; i++ )
				{
					int i0 = i;
					int i1 = i + 1;
					int i3 = pointCount - 1 - i;
					int i2 = i3 - 1;

					AddQuad(
						points[ i0 ], points[ i1 ], points[ i2 ], points[ i3 ],
						tangent, tangent, tangent, tangent,
						texCoord[ i0 ], texCoord[ i1 ], texCoord[ i2 ], texCoord[ i3 ],
						insideOut
						);
				}

				EndFace();
			}

			//Only for convex polygons. Triangles have common center. The triangles count == points.Length-1 (segments count)
			public void AddPolygonRadial( Point[] points, Point center, bool insideOut, bool flipU )
			{
				CalculateNormalAndU( points[ 0 ], points[ 1 ], center, insideOut, flipU, out Vector3 normal, out Vector3 uDirection );
				CalculateTexCoordForFlatPolygon2( points, new[] { center }, normal, uDirection, out Vector2[] texCoord, out Vector2[] texCoordCenter, out Vector4 tangent );

				BeginFace( 3 * ( points.Length - 1 ) );
				for( int i = 0; i < points.Length - 1; i++ )
				{
					AddTriangle(
						points[ i ], points[ i + 1 ], center,
						tangent, tangent, tangent,
						texCoord[ i ], texCoord[ i + 1 ], texCoordCenter[ 0 ],
						insideOut
						);
				}
				EndFace();
			}
			
			//Only for convex polygons. 3 segments - single triangle, 4 segments - 2 triangles, =>5 segments - triangles for each segment from a center;
			public void AddPolygonAdaptive( Point[] points, Point center, bool insideOut, bool flipU )
			{
				if( points[ 0 ].Vertex == points[ points.Length - 1 ].Vertex && points.Length - 1 <= 4 )
					AddPolygonSliced( points, insideOut, flipU );
				else
					AddPolygonRadial( points, center, insideOut, flipU );
			}

			public void AddPolygon( Point[] points, int[] triangles, bool insideOut, bool flipU )
			{
				BeginFace(triangles.Length);
				CalculateNormalAndU( points[ triangles[0] ], points[ triangles[1] ], points[ triangles[2] ], insideOut, flipU, out Vector3 normal, out Vector3 uDirection );
				CalculateTexCoordForFlatPolygon( points, normal, uDirection, out Vector2[] texCoord, out Vector4 tangent );
				for( int i = 0; i < triangles.Length; i+=3 )
				{
					AddTriangle( 
						points[triangles[i]], points[ triangles[ i + 1] ], points[ triangles[ i +2] ],
						tangent, tangent, tangent,
						texCoord[ triangles[ i ] ], texCoord[ triangles[ i + 1 ]], texCoord[ triangles[ i + 2] ],
						insideOut
						);
				}
				EndFace();
			}

			public static int GetPolygonAdaptiveRawVertexCount( Point[] points ) =>
				GetPolygonAdaptiveRawVertexCount( points[ 0 ].Vertex == points[ points.Length - 1 ].Vertex, points.Length );

			public static int GetPolygonAdaptiveRawVertexCount( bool loop, int pointCount ) =>
				loop && pointCount - 1 <= 4 ? GetPolygonSlicedRawVertexCount( loop, pointCount ) : ( pointCount - 1 ) * 3;

			public static int GetPolygonSlicedRawVertexCount( bool loop, int pointCount )
			{
				if( loop )
					pointCount--;
				int ret = 0;
				if( pointCount % 2 != 0 )
				{
					pointCount--;
					ret += 3;
				}

				ret += ( pointCount / 2 - 1 ) * 6;
				return ret;
			}

			public static int GetPolygon2RawVertexCount( Point[] points ) => 6 * ( points.Length - 1 );


			public static int GetLegsRawVertexCount( Point[][] legs ) => ( legs[ 0 ].Length - 1 ) * 6 * ( legs.Length - 1 ); //Only when all legs[i] are equal
		}

		#endregion
	}
}
