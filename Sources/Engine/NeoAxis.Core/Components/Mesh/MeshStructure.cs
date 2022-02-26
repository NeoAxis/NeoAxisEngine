// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoAxis
{
	partial class Mesh
	{
		/// <summary>
		/// Specifies the structure of the mesh. Includes information about edges, faces.
		/// </summary>
		public class StructureClass : ICloneable
		{
			public Vertex[] Vertices;
			public Edge[] Edges;
			public Face[] Faces;

			////////////

			/// <summary>
			/// Specifies the vertex of the mesh <see cref="StructureClass"/>.
			/// </summary>
			public struct Vertex
			{
				public List<(string, string)> Properties;

				//

				public Vertex( List<(string, string)> properties = null )
				{
					Properties = properties;
				}
			}

			////////////

			/// <summary>
			/// Specifies the edge of the mesh <see cref="StructureClass"/>.
			/// </summary>
			public struct Edge
			{
				public int Vertex1;
				public int Vertex2;
				public List<(string, string)> Properties;

				//

				public Edge( int vertex1, int vertex2, List<(string, string)> properties = null )
				{
					Vertex1 = vertex1;
					Vertex2 = vertex2;
					Properties = properties;
				}
			}

			////////////

			/// <summary>
			/// Specifies the vertex of the <see cref="Face"/> of the mesh structure.
			/// </summary>
			public struct FaceVertex
			{
				public int Vertex;
				public int RawGeometry;
				public int RawVertex;

				//

				public FaceVertex( int vertex, int rawGeometry, int rawVertex )
				{
					Vertex = vertex;
					RawGeometry = rawGeometry;
					RawVertex = rawVertex;
				}
			}

			////////////

			/// <summary>
			/// Specifies the face of the mesh <see cref="StructureClass"/>.
			/// </summary>
			public struct Face
			{
				public FaceVertex[] Triangles;
				public List<(string, string)> Properties;
				//smoothing group can be considered as extension
				public int SmoothingGroup;

				//

				public Face( FaceVertex[] triangles, List<(string, string)> properties, int smoothingGroup )
				{
					Triangles = triangles;
					Properties = properties;
					SmoothingGroup = smoothingGroup;
				}
			}

			////////////

			public object Clone()
			{
				var result = new StructureClass();

				result.Vertices = new Vertex[ Vertices.Length ];
				for( int i = 0; i < result.Vertices.Length; i++ )
				{
					ref var v = ref Vertices[ i ];
					ref var newV = ref result.Vertices[ i ];

					if( v.Properties != null )
						newV.Properties = v.Properties.ToList();
				}

				result.Edges = new Edge[ Edges.Length ];
				for( int i = 0; i < result.Edges.Length; i++ )
				{
					ref var e = ref Edges[ i ];
					ref var newE = ref result.Edges[ i ];

					newE.Vertex1 = e.Vertex1;
					newE.Vertex2 = e.Vertex2;
					if( e.Properties != null )
						newE.Properties = e.Properties.ToList();
				}

				result.Faces = new Face[ Faces.Length ];
				for( int i = 0; i < result.Faces.Length; i++ )
				{
					ref var f = ref Faces[ i ];
					ref var newF = ref result.Faces[ i ];

					if( f.Triangles != null )
						newF.Triangles = (FaceVertex[])f.Triangles.Clone();
					if( f.Properties != null )
						newF.Properties = f.Properties.ToList();
					newF.SmoothingGroup = f.SmoothingGroup;
				}

				return result;
			}

			public bool Load( Metadata.LoadContext context, TextBlock block, out string error )
			{
				int maxVertexIndex = 0;

				var edgesString = block.GetAttribute( "Edges" );
				if( edgesString != null )
				{
					var values = edgesString.Split( new char[] { ' ' } );

					Edges = new Edge[ values.Length / 2 ];
					for( int n = 0; n < Edges.Length; n++ )
					{
						ref var edge = ref Edges[ n ];
						edge.Vertex1 = int.Parse( values[ n * 2 + 0 ] );
						edge.Vertex2 = int.Parse( values[ n * 2 + 1 ] );

						maxVertexIndex = Math.Max( edge.Vertex1, maxVertexIndex );
						maxVertexIndex = Math.Max( edge.Vertex2, maxVertexIndex );
					}
				}

				var facesString = block.GetAttribute( "Faces" );
				if( facesString != null )
				{
					var values = facesString.Split( new char[] { ' ' } );

					int current = 0;
					int Get()
					{
						return int.Parse( values[ current++ ] );
					}

					Faces = new Face[ Get() ];
					for( int nFace = 0; nFace < Faces.Length; nFace++ )
					{
						ref var face = ref Faces[ nFace ];

						face.Triangles = new FaceVertex[ Get() ];
						for( int n = 0; n < face.Triangles.Length; n++ )
						{
							ref var faceVertex = ref face.Triangles[ n ];

							faceVertex.Vertex = Get();
							faceVertex.RawGeometry = Get();
							faceVertex.RawVertex = Get();

							maxVertexIndex = Math.Max( faceVertex.Vertex, maxVertexIndex );
						}

						face.SmoothingGroup = Get();
					}
				}

				Vertices = new Vertex[ maxVertexIndex + 1 ];

				foreach( var b in block.Children )
				{
					if( b.Name == "VertexProperties" && Vertices != null )
					{
						int index = int.Parse( b.Data );
						if( index < Vertices.Length )
						{
							ref var vertex = ref Vertices[ index ];
							if( vertex.Properties == null )
								vertex.Properties = new List<(string, string)>();
							vertex.Properties.Add( (b.GetAttribute( "Item1" ), b.GetAttribute( "Item2" )) );
						}
					}

					if( b.Name == "EdgeProperties" && Edges != null )
					{
						int index = int.Parse( b.Data );
						if( index < Edges.Length )
						{
							ref var edge = ref Edges[ index ];
							if( edge.Properties == null )
								edge.Properties = new List<(string, string)>();
							edge.Properties.Add( (b.GetAttribute( "Item1" ), b.GetAttribute( "Item2" )) );
						}
					}

					if( b.Name == "FaceProperties" && Faces != null )
					{
						int index = int.Parse( b.Data );
						if( index < Faces.Length )
						{
							ref var face = ref Faces[ index ];
							if( face.Properties == null )
								face.Properties = new List<(string, string)>();
							face.Properties.Add( (b.GetAttribute( "Item1" ), b.GetAttribute( "Item2" )) );
						}
					}
				}

				error = "";
				return true;
			}

			public bool Save( Metadata.SaveContext context, TextBlock block, out string error )
			{
				if( Edges != null )
				{
					var str = new StringBuilder( Edges.Length * 2 * 5 );
					foreach( var edge in Edges )
					{
						if( str.Length != 0 ) str.Append( ' ' );
						str.Append( edge.Vertex1 );

						str.Append( ' ' );
						str.Append( edge.Vertex2 );
					}
					block.SetAttribute( "Edges", str.ToString() );
				}

				if( Faces != null )
				{
					int count = 1;
					foreach( var face in Faces )
						count += 2 + face.Triangles.Length * 3;

					var str = new StringBuilder( count * 5 );
					str.Append( Faces.Length );
					foreach( var face in Faces )
					{
						str.Append( ' ' ); str.Append( face.Triangles.Length );
						foreach( var faceVertex in face.Triangles )
						{
							str.Append( ' ' ); str.Append( faceVertex.Vertex );
							str.Append( ' ' ); str.Append( faceVertex.RawGeometry );
							str.Append( ' ' ); str.Append( faceVertex.RawVertex );
						}
						str.Append( ' ' ); str.Append( face.SmoothingGroup );
					}

					block.SetAttribute( "Faces", str.ToString() );
				}

				if( Vertices != null )
				{
					for( int n = 0; n < Vertices.Length; n++ )
					{
						ref var vertex = ref Vertices[ n ];

						if( vertex.Properties != null )
						{
							foreach( var item in vertex.Properties )
							{
								var b = block.AddChild( "VertexProperties", n.ToString() );
								b.SetAttribute( "Item1", item.Item1 );
								b.SetAttribute( "Item2", item.Item2 );
							}
						}
					}
				}

				if( Edges != null )
				{
					for( int n = 0; n < Edges.Length; n++ )
					{
						ref var edge = ref Edges[ n ];

						if( edge.Properties != null )
						{
							foreach( var item in edge.Properties )
							{
								var b = block.AddChild( "EdgeProperties", n.ToString() );
								b.SetAttribute( "Item1", item.Item1 );
								b.SetAttribute( "Item2", item.Item2 );
							}
						}
					}
				}

				if( Faces != null )
				{
					for( int n = 0; n < Faces.Length; n++ )
					{
						ref var face = ref Faces[ n ];

						if( face.Properties != null )
						{
							foreach( var item in face.Properties )
							{
								var b = block.AddChild( "FaceProperties", n.ToString() );
								b.SetAttribute( "Item1", item.Item1 );
								b.SetAttribute( "Item2", item.Item2 );
							}
						}
					}
				}

				error = "";
				return true;
			}

			//structure, structureToAppend can be null. Pure function( return value does not have references on the data in structure, structureToAppend )
			public static StructureClass Concat( StructureClass structure, StructureClass structureToAppend, int structureToAppendGeometryOffset )
			{
				if( structureToAppend == null )
					return (StructureClass)structure?.Clone();

				structureToAppend = (StructureClass)structureToAppend.Clone();

				if( structure == null )
				{
					for( int i = 0; i < structureToAppend.Faces.Length; i++ )
					{
						var triangles = structureToAppend.Faces[ i ].Triangles;
						for( int j = 0; j < triangles.Length; j++ )
							triangles[ j ].RawGeometry += structureToAppendGeometryOffset;
					}
					return structureToAppend;
				}

				int vertexIndexOffset = structure.Vertices.Length;
				for( var i = 0; i < structureToAppend.Edges.Length; i++ )
				{
					ref var edge = ref structureToAppend.Edges[ i ];
					edge.Vertex1 += vertexIndexOffset;
					edge.Vertex2 += vertexIndexOffset;
				}

				int smoothingGroupOffset = 0;
				foreach( var face in structure.Faces )
					if( smoothingGroupOffset < face.SmoothingGroup )
						smoothingGroupOffset = face.SmoothingGroup;
				smoothingGroupOffset++;

				for( int i = 0; i < structureToAppend.Faces.Length; i++ )
				{
					//??? Учесть соглашения, об особых группах(значениях) сглаживания. Пока только 0 не меняется, если будут другие - их тоже не корректировать.

					//correct the smoothing group, so that it will not intersect with an existing group.
					if( structureToAppend.Faces[ i ].SmoothingGroup != 0 )
						structureToAppend.Faces[ i ].SmoothingGroup += smoothingGroupOffset;
					var triangles = structureToAppend.Faces[ i ].Triangles;
					for( int j = 0; j < triangles.Length; j++ )
					{
						triangles[ j ].Vertex += vertexIndexOffset;
						triangles[ j ].RawGeometry += structureToAppendGeometryOffset;
					}
				}

				return new StructureClass
				{
					Vertices = Concat( structure.Vertices, structureToAppend.Vertices ),
					Edges = Concat( structure.Edges, structureToAppend.Edges ),
					Faces = Concat( structure.Faces, structureToAppend.Faces ),
				};

				//----------------------
				T[] Concat<T>( T[] x, T[] y )
				{
					int oldLen = x.Length;
					Array.Resize( ref x, x.Length + y.Length );
					Array.Copy( y, 0, x, oldLen, y.Length );
					return x;
				}
			}

			public override string ToString()
			{
				var vertices = Vertices != null ? Vertices.Length.ToString() : "null";
				var edges = Edges != null ? Edges.Length.ToString() : "null";
				var faces = Faces != null ? Faces.Length.ToString() : "null";
				return $"Vertices {vertices}; Edges {edges}; Faces {faces}";
			}
		}

		/////////////////////////////////////////

		class GeometryEntry
		{
			public byte[] vertices;
			public int[] indices;
			public VertexElement positionElement;
			public int vertexSize;
			public int vertexCount;

			public static GeometryEntry ToGeometryEntry( MeshGeometry g )
			{
				if( g is MeshGeometry_Procedural )
					return null;

				g.VertexStructure.Value.GetInfo( out int vSize, out bool _ );
				return new GeometryEntry
				{
					vertices = g.Vertices.Value,
					indices = g.Indices.Value,
					positionElement = g.VertexStructure.Value.First( elem => elem.Semantic == VertexElementSemantic.Position ),
					vertexSize = vSize,
					vertexCount = g.Vertices.Value.Length / vSize
				};
			}
		}

		/// <summary>
		/// Builds the structure of the mesh. The structure includes edges and faces.
		/// </summary>
		/// <param name="mergeTrianglesIntoFaces"> If true. Merges adjacent triangles if their normals ar equal</param>
		public static StructureClass BuildStructure( Mesh mesh, bool mergeTrianglesIntoFaces = true, float normalComparisonEpsilon = 1e-4f )
		{
			//geometries[x] == null for MeshGeometry_Procedural
			//Procedural geometries are skipped because their structures are appended in ExtractStructure()
			GeometryEntry[] geometries = mesh.GetComponents<MeshGeometry>().Select( GeometryEntry.ToGeometryEntry ).ToArray();

			if( geometries.All( g => g == null ) )
				return null;

			Vector3F GetPosition( (int geomIndex, int vertexIndex) vertexRef )
			{
				var g = geometries[ vertexRef.geomIndex ];

				unsafe
				{
					fixed( byte* ptr = g.vertices )
						return *(Vector3F*)( ptr + g.vertexSize * vertexRef.vertexIndex + g.positionElement.Offset );
				}
			}

			//-----------------------------------------
			// Indexing of vertices by only positions. For all the geometries as if it is a single geometry (vertices from the different geometries can be merged).

			var vertexFinder = new SpatialSort<(int geomIndex, int rawVertexIndex)>( GetPosition );
			for( int geomIndex = 0; geomIndex < geometries.Length; geomIndex++ )
			{
				var geom = geometries[ geomIndex ];
				if( geom == null )
					continue;
				for( int i = 0; i < geom.vertexCount; i++ )
					vertexFinder.Append( (geomIndex, i) );
			}
			vertexFinder.FinishAppendAndSort();

			int[][] geomRawVertexSharedVertex = new int[ geometries.Length ][]; //geomRawVertexSharedVertex[geometryIndex][rawVertexIndex] == SharedVertexIndex
			for( int geomIndex = 0; geomIndex < geomRawVertexSharedVertex.Length; geomIndex++ )
			{
				var geom = geometries[ geomIndex ];
				if( geom == null )
					continue;
				var vertices = new int[ geom.vertexCount ];
				for( int j = 0; j < vertices.Length; j++ )
					vertices[ j ] = -1;
				geomRawVertexSharedVertex[ geomIndex ] = vertices;
			}

			bool CompareVertices( (int, int) v1, (int, int) v2, float epsilon )
			{
				var p1 = GetPosition( v1 );
				var p2 = GetPosition( v2 );
				return p1.Equals( p2, epsilon );
			}

			int nextSharedVertexIndex = 0;
			for( int geomIndex = 0; geomIndex < geometries.Length; geomIndex++ )
			{
				var geom = geometries[ geomIndex ];
				if( geom == null )
					continue;

				for( int vertexIndex = 0; vertexIndex < geomRawVertexSharedVertex[ geomIndex ].Length; vertexIndex++ )
				{
					if( -1 != geomRawVertexSharedVertex[ geomIndex ][ vertexIndex ] )
						continue; //already processed

					var close = vertexFinder.FindAllClose( (geomIndex, vertexIndex), CompareVertices );
					int assignedIndex = -1;
					//find any already assigned index in the group 
					for( int j = 0; j < close.Count; j++ )
					{
						assignedIndex = geomRawVertexSharedVertex[ close[ j ].geomIndex ][ close[ j ].rawVertexIndex ];
						if( assignedIndex != -1 )
							break;
					}

					if( assignedIndex == -1 ) //all unassigned
						assignedIndex = nextSharedVertexIndex++;

					//now fill all anassigned verex indices in the close group
					geomRawVertexSharedVertex[ geomIndex ][ vertexIndex ] = assignedIndex;
					for( int j = 0; j < close.Count; j++ )
					{
						ref int curIndexRef = ref geomRawVertexSharedVertex[ close[ j ].geomIndex ][ close[ j ].rawVertexIndex ];
						if( curIndexRef == -1 )
							curIndexRef = assignedIndex;
					}
				}
			}

			//Now the vertices are indexed by only positions. Indices are stored in geomRawVertexSharedVertex
			//--------------------------------------------------------

			var faceVertices = new List<StructureClass.FaceVertex>();

			for( int geomIndex = 0; geomIndex < geometries.Length; geomIndex++ )
			{
				var geom = geometries[ geomIndex ];
				if( geom == null )
					continue;

				for( int i = 0; i < geom.indices.Length; i++ )
				{
					var fv = new StructureClass.FaceVertex( geomRawVertexSharedVertex[ geomIndex ][ geom.indices[ i ] ], geomIndex, geom.indices[ i ] );
					faceVertices.Add( fv );
				}
			}

			var structure = new StructureClass();

			structure.Vertices = new StructureClass.Vertex[ nextSharedVertexIndex ];
			for( int i = 0; i < structure.Vertices.Length; i++ )
				structure.Vertices[ i ] = new StructureClass.Vertex();

			if( mergeTrianglesIntoFaces )
				structure.Faces = MergeTrianglesIntoFaces( faceVertices, normalComparisonEpsilon, GetPosition );
			else
			{
				structure.Faces = new StructureClass.Face[ faceVertices.Count / 3 ];
				for( int i = 0; i < structure.Faces.Length; i++ )
				{
					int start = i * 3;
					structure.Faces[ i ] = new StructureClass.Face( new StructureClass.FaceVertex[] { faceVertices[ start ], faceVertices[ start + 1 ], faceVertices[ start + 2 ] }, null, 0 );
				}
			}

			structure.Edges = BuildEdges( structure.Faces, nextSharedVertexIndex );

			return structure;
		}

		/// <summary>
		/// Builds the structure of the mesh. The structure includes edges and faces.
		/// </summary>
		/// <param name="mergeTrianglesIntoFaces"> If true. Merges adjacent triangles if their normals ar equal</param>
		public void BuildStructure( bool mergeTrianglesIntoFaces = true, float normalComparisonEpsilon = 1e-4f )
		{
			Structure = BuildStructure( this, mergeTrianglesIntoFaces, normalComparisonEpsilon );
		}

		static StructureClass.Face[] MergeTrianglesIntoFaces( List<StructureClass.FaceVertex> faceVertices, float normalComparisonEpsilon, Func<(int geometryIndex, int vertexIndex), Vector3F> positionGetter )
		{
			(int lowVertex, int highVertex) OrderVertices( int vertex1, int vertex2 ) => vertex1 < vertex2 ? (vertex1, vertex2) : (vertex2, vertex1);

			int triangleCount = faceVertices.Count / 3;
			var triangleInfo = new (Vector3F normal, int faсeIndex)[ triangleCount ];
			int nextFaceIndex = 0;
			for( int i = 0; i < triangleCount; i++ )
			{
				var fv0 = faceVertices[ i * 3 ];
				var fv1 = faceVertices[ i * 3 + 1 ];
				var fv2 = faceVertices[ i * 3 + 2 ];

				var p0 = positionGetter( (fv0.RawGeometry, fv0.RawVertex) );
				var p1 = positionGetter( (fv1.RawGeometry, fv1.RawVertex) );
				var p2 = positionGetter( (fv2.RawGeometry, fv2.RawVertex) );
				triangleInfo[ i ].normal = Vector3F.Cross( p1 - p0, p2 - p0 ).GetNormalize();
				triangleInfo[ i ].faсeIndex = -1;
			}

			//fill the edges dictionary
			//key - edge, value - triangle index ( faceVertices[value*3]==triangle start index)
			var edges = new Dictionary<(int vertexLow, int vertexHigh), List<int>>();
			void AddEdgeTriangle( int triangleIndex, (int vertexLow, int vertexHigh) edge )
			{
				if( !edges.TryGetValue( edge, out var list ) )
				{
					list = new List<int>();
					edges[ edge ] = list;
				}
				list.Add( triangleIndex );
			}

			for( int i = 0; i < triangleCount; i++ )
			{

				int triangleStart = i * 3;
				AddEdgeTriangle( i, OrderVertices( faceVertices[ triangleStart ].Vertex, faceVertices[ triangleStart + 1 ].Vertex ) );
				AddEdgeTriangle( i, OrderVertices( faceVertices[ triangleStart + 1 ].Vertex, faceVertices[ triangleStart + 2 ].Vertex ) );
				AddEdgeTriangle( i, OrderVertices( faceVertices[ triangleStart + 2 ].Vertex, faceVertices[ triangleStart ].Vertex ) );
			}

			for( int i = 0; i < triangleCount; i++ )
			{
				if( triangleInfo[ i ].faсeIndex != -1 )
					continue; //already assigned

				MarkAllSameNormalNeighbours( i, nextFaceIndex++, triangleInfo[ i ].normal );
			}

			void MarkAllSameNormalNeighbours( int triangleIndex, int faceIndexMark, Vector3F normal )
			{
				var triangleIndicesStack = new Stack<int>();
				triangleIndicesStack.Push( triangleIndex );

				while( triangleIndicesStack.Count != 0 )
				{
					int ti = triangleIndicesStack.Pop();
					if( triangleInfo[ ti ].faсeIndex != -1 ) //???
						continue;

					triangleInfo[ ti ].faсeIndex = faceIndexMark;
					//ToDo ??? Если нормали постепенно очень мало меняются, сходство определяется не по соседнему, а по первому. Т.е. у цилиндра боковые фейсы не объединятся в одну. Хотя можно сделать и так.
					//normal = triangleInfo[ triangleIndex ].normal;

					int triangleStart = ti * 3;
					var lst0 = edges[ OrderVertices( faceVertices[ triangleStart ].Vertex, faceVertices[ triangleStart + 1 ].Vertex ) ];
					var lst1 = edges[ OrderVertices( faceVertices[ triangleStart + 1 ].Vertex, faceVertices[ triangleStart + 2 ].Vertex ) ];
					var lst2 = edges[ OrderVertices( faceVertices[ triangleStart + 2 ].Vertex, faceVertices[ triangleStart ].Vertex ) ];

					foreach( var t in lst0 )
						if( triangleInfo[ t ].faсeIndex == -1 && normal.Equals( triangleInfo[ t ].normal, normalComparisonEpsilon ) )
							triangleIndicesStack.Push( t );
					foreach( var t in lst1 )
						if( triangleInfo[ t ].faсeIndex == -1 && normal.Equals( triangleInfo[ t ].normal, normalComparisonEpsilon ) )
							triangleIndicesStack.Push( t );
					foreach( var t in lst2 )
						if( triangleInfo[ t ].faсeIndex == -1 && normal.Equals( triangleInfo[ t ].normal, normalComparisonEpsilon ) )
							triangleIndicesStack.Push( t );
				}
			}

			////Вариант с рекурсией, но не желательно т.к. глубина может быть большая.
			//void MarkAllSameNormalNeighboursRec( int triangleIndex, int faceIndexMark, Vector3F normal )
			//{
			//	if( triangleInfo[ triangleIndex ].faсeIndex != -1 ) //???
			//		return;

			//	triangleInfo[ triangleIndex ].faсeIndex = faceIndexMark;

			//	int triangleStart = triangleIndex * 3;
			//	var lst0 = edges[ OrderVertices( faceVertices[ triangleStart ].Vertex, faceVertices[ triangleStart + 1 ].Vertex ) ];
			//	var lst1 = edges[ OrderVertices( faceVertices[ triangleStart + 1 ].Vertex, faceVertices[ triangleStart + 2 ].Vertex ) ];
			//	var lst2 = edges[ OrderVertices( faceVertices[ triangleStart + 2 ].Vertex, faceVertices[ triangleStart ].Vertex ) ];

			//	foreach( var t in lst0 )
			//		if( triangleInfo[ t ].faсeIndex == -1 && normal.Equals( triangleInfo[ t ].normal, normalComparisonEpsilon ) )
			//			MarkAllSameNormalNeighboursRec( t, faceIndexMark, normal );
			//	foreach( var t in lst1 )
			//		if( triangleInfo[ t ].faсeIndex == -1 && normal.Equals( triangleInfo[ t ].normal, normalComparisonEpsilon ) )
			//			MarkAllSameNormalNeighboursRec( t, faceIndexMark, normal );
			//	foreach( var t in lst2 )
			//		if( triangleInfo[ t ].faсeIndex == -1 && normal.Equals( triangleInfo[ t ].normal, normalComparisonEpsilon ) )
			//			MarkAllSameNormalNeighboursRec( t, faceIndexMark, normal );
			//}

			//Group triangles in faces by the marks in triangleInfo[ * ].faсeIndex
			var facesLst = new List<StructureClass.FaceVertex>[ nextFaceIndex ];
			for( int i = 0; i < nextFaceIndex; i++ )
				facesLst[ i ] = new List<StructureClass.FaceVertex>();
			for( int i = 0; i < triangleCount; i++ )
			{
				int triangleStart = i * 3;
				facesLst[ triangleInfo[ i ].faсeIndex ].Add( faceVertices[ triangleStart ] );
				facesLst[ triangleInfo[ i ].faсeIndex ].Add( faceVertices[ triangleStart + 1 ] );
				facesLst[ triangleInfo[ i ].faсeIndex ].Add( faceVertices[ triangleStart + 2 ] );
			}
			var ret = new StructureClass.Face[ nextFaceIndex ];
			for( int i = 0; i < ret.Length; i++ )
				ret[ i ] = new StructureClass.Face( facesLst[ i ].ToArray(), null, 0 );

			System.Diagnostics.Debug.Assert( faceVertices.Count == ret.SelectMany( _ => _.Triangles ).Count() );

			return ret;
		}

		//Перенесено, почти без изменений из SimpleMeshGenerator.cs
		static StructureClass.Edge[] BuildEdges( StructureClass.Face[] faces, int vertexMaxIndex )
		{
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

			}

			return edges.Select( e => new StructureClass.Edge( e.X, e.Y ) ).ToArray();
		}

		//-----------------------------------------------------------------------------
		/// <summary>
		/// A little helper class to quickly find all vertices in the epsilon environment of a given
		/// position. Construct an instance with an array of positions. The class stores the given positions
		/// by their indices and sorts them by their distance to an arbitrary chosen plane.
		/// You can then query the instance for all vertices close to a given position in an average O(log n)
		/// time, with O(n) worst case complexity when all vertices lay on the plane. The plane is chosen
		/// so that it avoids common planes in usual data sets. */
		/// </summary>
		class SpatialSort<TVertexRef> where TVertexRef : struct
		{
			//Normal of the sorting plane, normalized. The center is always at (0, 0, 0) 
			readonly Vector3F planeNormal;
			Func<TVertexRef, Vector3F> positionGetter;

			readonly float baseEpsilon = 1e-4f;
			float epsilon;
			Vector3F maxVec = new Vector3F( -1e10f, -1e10f, -1e10f );
			Vector3F minVec = new Vector3F( 1e10f, 1e10f, 1e10f );

			struct Entry
			{
				public readonly TVertexRef VertexRef; // The vertex referred by this entry
				public readonly Vector3F Position; // Position
				public readonly float Distance; // Distance of this vertex to the sorting plane


				public Entry( TVertexRef vertexRef, Vector3F position, float distance )
				{
					VertexRef = vertexRef;
					Position = position;
					Distance = distance;
				}

				//public static Entry Default { get; } = new Entry( 999999999, new Vec3F(), 99999); //Default constructor in Assimp assigned such values
			}

			class EntryComparer : IComparer<Entry>
			{
				public int Compare( Entry x, Entry y )
				{
					if( x.Distance < y.Distance )
						return -1;
					if( x.Distance > y.Distance )
						return 1;
					return 0;
				}
			}

			// all positions, sorted by distance to the sorting plane
			readonly List<Entry> positions = new List<Entry>();

			//baseEpsilon will be refined by the positions of the added vertices
			public SpatialSort( Func<TVertexRef, Vector3F> positionGetter, float baseEpsilon = 1e-4f )
			{
				this.positionGetter = positionGetter;
				this.baseEpsilon = baseEpsilon;
				epsilon = this.baseEpsilon;
				// define the reference plane. We choose some arbitrary vector away from all basic axises
				// in the hope that no model spreads all its vertices along this plane.
				planeNormal = new Vector3F( 0.8523f, 0.34321f, 0.5736f );
			}

			public void Append( IEnumerable<TVertexRef> vertices )
			{
				// store references to all given positions along with their distance to the reference plane
				foreach( var vertex in vertices )
				{
					Vector3F position = positionGetter( vertex );
					float distance = Vector3F.Dot( position, planeNormal );
					positions.Add( new Entry( vertex, position, distance ) );

					minVec = Vector3F.Min( minVec, position );
					maxVec = Vector3F.Max( maxVec, position );
				}
			}
			public void Append( TVertexRef vertex )
			{
				Vector3F position = positionGetter( vertex );
				float distance = Vector3F.Dot( position, planeNormal );
				positions.Add( new Entry( vertex, position, distance ) );

				minVec = Vector3F.Min( minVec, position );
				maxVec = Vector3F.Max( maxVec, position );
			}

			public void FinishAppendAndSort()
			{
				// now sort the array ascending by distance.
				positions.Sort( new EntryComparer() );
				epsilon = ( maxVec - minVec ).Length() * baseEpsilon;
			}

			/// <summary>
			/// Finds all positions close to the given position
			/// </summary>
			/// <param name="position">The position to look for vertices.</param>
			/// <param name="radius">Maximal distance from the position a vertex may have to be counted in.</param>
			/// <returns></returns>
			public List<TVertexRef> FindPositions( Vector3F position, float radius )
			{
				float dist = Vector3F.Dot( position, planeNormal );
				float minDist = dist - radius;
				float maxDist = dist + radius;

				var results = new List<TVertexRef>();
				// quick check for positions outside the range
				if( positions.Count == 0 )
					return results;
				if( maxDist < positions[ 0 ].Distance )
					return results;
				if( minDist > positions[ positions.Count - 1 ].Distance )
					return results;

				//int index = BinarySerach( positions, minDist ); //Warning : В Assimp был такой BinarySearch

				// do a binary search for the minimal distance to start the iteration there
				int index = positions.BinarySearch( new Entry( default, new Vector3F(), minDist ), new EntryComparer() );
				if( index < 0 )
					index = ~index;
				else
					index++;

				// Now start iterating from there until the first position lays outside of the distance range.
				// Add all positions inside the distance range within the given radius to the result aray
				float squaredRadius = radius * radius;
				for( int i = index; i < positions.Count && positions[ i ].Distance < maxDist; i++ )
				{
					if( ( positions[ i ].Position - position ).LengthSquared() < squaredRadius )
						results.Add( positions[ i ].VertexRef );
				}
				return results;
			}

			//vertexEqualityComparer compares vertices(not references)
			//vertex is included in return

			//??? Если надо будет возвращать включая искомый вертекс, то сделать как опцию добавить bool флажок.
			/// <summary>
			/// 
			/// </summary>
			/// <param name="vertex"></param>
			/// <param name="vertexEqualityComparerWithEpsilon"></param>
			/// <returns>All vertices close to <paramref name="vertex"/>, not including itself</returns>
			public List<TVertexRef> FindAllClose( TVertexRef vertex, Func<TVertexRef, TVertexRef, float, bool> vertexEqualityComparerWithEpsilon )
			{
				List<TVertexRef> closeVertices = FindPositions( positionGetter( vertex ), epsilon );
				System.Diagnostics.Debug.Assert( closeVertices.Count != 0 ); //at least must find itself
				List<TVertexRef> closeAndEqualVertices = new List<TVertexRef>();

				//select only equal vertices in the group and exclude i
				for( int j = 0; j < closeVertices.Count; j++ )
				{
					if( !closeVertices[ j ].Equals( vertex ) && vertexEqualityComparerWithEpsilon( vertex, closeVertices[ j ], epsilon ) )
						closeAndEqualVertices.Add( closeVertices[ j ] );
				}
				return closeAndEqualVertices;
			}
		}

	}
}
