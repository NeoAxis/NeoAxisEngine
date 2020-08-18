// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis.Addon.Builder3D
{
	class MeshData
	{
		public List<Vertex> Vertices;
		public List<Edge> Edges;
		public List<Face> Faces;
		//!!!!
		MeshGeometryFormat[] meshGeometriesFormat;

		//public List<Component_MeshGeometry> SourceGeometries;

		//public List<Geometry> Geometries = new List<Geometry>();
		//public Component_Mesh.StructureClass Structure = new Component_Mesh.StructureClass();

		/////////////////////////////////////////

		//Todo ? MeshGeometryFormat вместо отдельных offset?

		public class RawVertex
		{
			readonly byte[] data;
			readonly int positionOffset = -1;
			readonly int normalOffset = -1;
			readonly int tangentOffset = -1;
			readonly int colorOffset = -1;
			readonly int texCoordOffset = -1;

			private RawVertex() { }

			//public RawVertex( byte[] data, int positionOffset, int normalOffset, int tangentOffset, int colorOffset, int texCoordOffset )
			//{
			//	this.data = data;
			//	this.positionOffset = positionOffset;
			//	this.normalOffset = normalOffset;
			//	this.tangentOffset = tangentOffset;
			//	this.colorOffset = colorOffset;
			//	this.texCoordOffset = texCoordOffset;
			//}

			public RawVertex( byte[] data, MeshGeometryFormat format )
			{
				this.data = data;
				positionOffset = format.positionOffset;
				normalOffset = format.normalOffset;
				tangentOffset = format.tangentOffset;
				colorOffset = format.colorOffset;
				texCoordOffset = format.texCoordOffset;
			}

			public byte[] Data
			{
				get { return data; }
			}

			public unsafe Vector3F Position
			{
				get
				{
					if( positionOffset != -1 )
						fixed ( byte* pData = data )
							return *(Vector3F*)( pData + positionOffset );
					return Vector3F.Zero;
				}
				set
				{
					if( positionOffset != -1 )
						fixed ( byte* pData = data )
							*(Vector3F*)( pData + positionOffset ) = value;
				}
			}

			public unsafe Vector3F Normal
			{
				get
				{
					if( normalOffset != -1 )
						fixed ( byte* pData = data )
							return *(Vector3F*)( pData + normalOffset );
					return Vector3F.Zero;
				}
				set
				{
					if( normalOffset != -1 )
						fixed ( byte* pData = data )
							*(Vector3F*)( pData + normalOffset ) = value;
				}
			}

			public unsafe Vector4F Tangent
			{
				get
				{
					if( tangentOffset != -1 )
						fixed ( byte* pData = data )
							return *(Vector4F*)( pData + tangentOffset );
					return Vector4F.Zero;
				}
				set
				{
					if( tangentOffset != -1 )
						fixed ( byte* pData = data )
							*(Vector4F*)( pData + tangentOffset ) = value;
				}
			}

			public unsafe Vector4F Color
			{
				get
				{
					if( colorOffset != -1 )
						fixed ( byte* pData = data )
							return *(Vector4F*)( pData + colorOffset );
					return Vector4F.Zero;
				}
				set
				{
					if( colorOffset != -1 )
						fixed ( byte* pData = data )
							*(Vector4F*)( pData + colorOffset ) = value;
				}
			}

			public unsafe Vector2F TexCoord
			{
				get
				{
					if( texCoordOffset != -1 )
						fixed ( byte* pData = data )
							return *(Vector2F*)( pData + texCoordOffset );
					return Vector2F.Zero;
				}
				set
				{
					if( texCoordOffset != -1 )
						fixed ( byte* pData = data )
							*(Vector2F*)( pData + texCoordOffset ) = value;
				}
			}

		}

		/////////////////////////////////////////

		public class Vertex
		{
			public List<(string, string)> Properties;

			//maybe good for optimization, but memory
			//List<Face> ReferencedFaces;

			//

			public Vertex( List<(string, string)> properties = null )
			{
				Properties = properties;
			}
		}

		/////////////////////////////////////////

		public class Edge
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

		/////////////////////////////////////////

		public class FaceVertex
		{
			public int Vertex;
			public int RawGeometry;
			public RawVertex RawVertex;

			//

			public FaceVertex( int vertex, int rawGeometry, RawVertex rawVertex )
			{
				Vertex = vertex;
				RawGeometry = rawGeometry;
				RawVertex = rawVertex;
			}
		}

		/////////////////////////////////////////

		public class Face
		{
			public List<FaceVertex> Triangles;
			public List<(string, string)> Properties;
			//smoothing group can be considered as extension
			public int SmoothingGroup;

			//

			public Face( List<FaceVertex> triangles, List<(string, string)> properties, int smoothingGroup )
			{
				Triangles = triangles;
				Properties = properties;
				SmoothingGroup = smoothingGroup;
			}
		}

		/////////////////////////////////////////

		//public class Geometry
		//{
		//	//!!!!
		//	public Component_MeshGeometry SourceMeshGeometry;

		//	public List<RawVertex> Vertices;
		//	//!!!!нужны ли индексы. может разбирать на вершины, потом собирать
		//	public List<int> Indices;
		//	//public List<RawTriangle> Triangles;
		//}

		/////////////////////////////////////////

		internal class MeshGeometryFormat
		{
			public int vertexSize;
			public int positionOffset = -1;
			public int normalOffset = -1;
			public int tangentOffset = -1;
			public int colorOffset = -1;
			public int texCoordOffset = -1;

			//

			public MeshGeometryFormat( VertexElement[] vertexStructure )
			{
				vertexStructure.GetInfo( out vertexSize, out _ );

				foreach( var element in vertexStructure )
				{
					switch( element.Semantic )
					{
					case VertexElementSemantic.Position:
						if( element.Type == VertexElementType.Float3 )
							positionOffset = element.Offset;
						break;
					case VertexElementSemantic.Normal:
						if( element.Type == VertexElementType.Float3 )
							normalOffset = element.Offset;
						break;
					case VertexElementSemantic.Tangent:
						if( element.Type == VertexElementType.Float4 )
							tangentOffset = element.Offset;
						break;
					case VertexElementSemantic.Color0:
						if( element.Type == VertexElementType.Float4 )
							colorOffset = element.Offset;
						break;
					case VertexElementSemantic.TextureCoordinate0:
						if( element.Type == VertexElementType.Float2 )
							texCoordOffset = element.Offset;
						break;

					}
				}
			}
		}

		/////////////////////////////////////////

		static readonly Vector4F DefaultVertexColor = new Vector4F( 1, 1, 1, 1 );


		static MeshGeometryFormat[] GetMeshGeometriesFormat( Component_Mesh.ExtractedStructure.MeshGeometryItem[] geometries )
		{
			var result = new List<MeshGeometryFormat>();
			foreach( var geometry in geometries )
				result.Add( new MeshGeometryFormat( geometry.VertexStructure ) );
			return result.ToArray();
		}

		static MeshGeometryFormat[] GetMeshGeometriesFormat( Component_MeshGeometry[] geometries )
		{
			var result = new List<MeshGeometryFormat>();
			foreach( var geometry in geometries )
				result.Add( new MeshGeometryFormat( geometry.VertexStructure ) );
			return result.ToArray();
		}

		public static List<(string, string)> CloneProperties( List<(string, string)> source )
		{
			if( source != null )
				return source.ToList();
			return null;
		}

		public FaceVertex CreateFaceVertex( int vertex, int meshGeometry )
		{
			var format = meshGeometriesFormat[ meshGeometry ];
			var rawVertex = new RawVertex( new byte[ format.vertexSize ], format );
			rawVertex.Color = DefaultVertexColor;
			return new FaceVertex(
				vertex,
				meshGeometry,
				rawVertex );

		}

		public FaceVertex CloneFaceVertex( FaceVertex vertex )
		{
			var format = meshGeometriesFormat[ vertex.RawGeometry ];
			return new FaceVertex(
				vertex.Vertex,
				vertex.RawGeometry,
				new RawVertex( (byte[])vertex.RawVertex.Data.Clone(), format ) );
		}

		public static RawVertex ConvertRawVertex( RawVertex rv, MeshGeometryFormat newFormat )
		{
			var newRv = new RawVertex( new byte[ newFormat.vertexSize ], newFormat )
			{
				Position = rv.Position, Normal = rv.Normal, Tangent = rv.Tangent, Color = rv.Color, TexCoord = rv.TexCoord
			};
			//ToDo : Texcoord
			return newRv;
		}

		public static byte[] ConvertToFormat( MeshGeometryFormat originalFormat, byte[] originalData, MeshGeometryFormat newFormat )
		{
			int originalCount = originalData.Length / originalFormat.vertexSize;
			byte[] newData = new byte[ newFormat.vertexSize * originalCount ];

			byte[] buf = new byte[ originalFormat.vertexSize ];
			for( int i = 0; i < originalCount; i++ )
			{
				Array.Copy( originalData, i * originalFormat.vertexSize, buf, 0, originalFormat.vertexSize );
				var old = new RawVertex( buf, originalFormat );
				var newVertex = ConvertRawVertex( old, newFormat );
				Array.Copy( newVertex.Data, 0, newData, i * newFormat.vertexSize, newFormat.vertexSize );
			}
			return newData;
		}

		public void Load( Component_Mesh.ExtractedStructure extractedStructure )
		{
			var structure = extractedStructure.Structure;

			//get mesh geometry and it's data
			//var meshGeometries = mesh.GetComponents<Component_MeshGeometry>();
			meshGeometriesFormat = GetMeshGeometriesFormat( extractedStructure.MeshGeometries );

			//load data

			Vertices = new List<Vertex>( structure.Vertices.Length );
			foreach( var vertex in structure.Vertices )
				Vertices.Add( new Vertex( CloneProperties( vertex.Properties ) ) );

			Edges = new List<Edge>( structure.Edges.Length );
			foreach( var edge in structure.Edges )
				Edges.Add( new Edge( edge.Vertex1, edge.Vertex2, CloneProperties( edge.Properties ) ) );

			Faces = new List<Face>( structure.Faces.Length );
			foreach( var face in structure.Faces )
			{
				var triangles = new List<FaceVertex>( face.Triangles.Length );
				foreach( var faceVertex in face.Triangles )
				{
					var geometryData = meshGeometriesFormat[ faceVertex.RawGeometry ];
					var geometry = extractedStructure.MeshGeometries[ faceVertex.RawGeometry ];

					var data = new byte[ geometryData.vertexSize ];
					Array.Copy( geometry.Vertices, faceVertex.RawVertex * geometryData.vertexSize, data, 0, geometryData.vertexSize );
					var rawVertex = new RawVertex( data, geometryData );
					triangles.Add( new FaceVertex( faceVertex.Vertex, faceVertex.RawGeometry, rawVertex ) );
				}

				Faces.Add( new Face( triangles, CloneProperties( face.Properties ), face.SmoothingGroup ) );
			}
		}

		//selection - will be corrected, if Save removes some objects so that indices change.
		public void Save( Component_Mesh mesh, UndoMultiAction undoForGeometries, Selection selection )
		{
			//get mesh geometry and it's data
			var meshGeometries = mesh.GetComponents<Component_MeshGeometry>();
			//var meshGeometriesFormat = GetMeshGeometriesFormat( meshGeometries );

			//!!!!может удалять неиспользуемые данные в save structure
			RemoveUnusedData( selection );

			var structure = new Component_Mesh.StructureClass();

			////!!!!temp. пока одна mesh geometry
			//List<byte> geometryVertices = new List<byte>();
			//int geometryVerticesCounter = 0;
			//List<int> geometryIndices = new List<int>();

			var geomData = new (List<byte> geometryVertices, List<int> geometryIndices, int geometryVerticesCounter)[ meshGeometries.Length ];
			for( int i = 0; i < meshGeometries.Length; i++ )
			{
				geomData[ i ].geometryVertices = new List<byte>();
				geomData[ i ].geometryIndices = new List<int>();
			}

			{
				var vertices = new List<Component_Mesh.StructureClass.Vertex>( Vertices.Count );
				foreach( var vertex in Vertices )
					vertices.Add( new Component_Mesh.StructureClass.Vertex( vertex.Properties ) );
				structure.Vertices = vertices.ToArray();

				var edges = new List<Component_Mesh.StructureClass.Edge>( Edges.Count );
				foreach( var edge in Edges )
					edges.Add( new Component_Mesh.StructureClass.Edge( edge.Vertex1, edge.Vertex2, edge.Properties ) );
				structure.Edges = edges.ToArray();

				var faces = new List<Component_Mesh.StructureClass.Face>( Faces.Count );
				foreach( var face in Faces )
				{
					var triangles = new List<Component_Mesh.StructureClass.FaceVertex>( face.Triangles.Count );
					foreach( var triangle in face.Triangles )
					{
						//!!!!объединять

						ref var gRef = ref geomData[ triangle.RawGeometry ];

						var rawVertex = gRef.geometryVerticesCounter;
						gRef.geometryVertices.AddRange( triangle.RawVertex.Data );
						gRef.geometryIndices.Add( gRef.geometryVerticesCounter );

						gRef.geometryVerticesCounter++;

						triangles.Add( new Component_Mesh.StructureClass.FaceVertex( triangle.Vertex, triangle.RawGeometry, rawVertex ) );
					}

					faces.Add( new Component_Mesh.StructureClass.Face( triangles.ToArray(), face.Properties, face.SmoothingGroup ) );
				}
				structure.Faces = faces.ToArray();
			}

			//add to undo
			if( undoForGeometries != null )
			{
				//structure
				var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Component_Mesh.Structure ) );
				undoForGeometries.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure ) ) );


				foreach( var meshGeometry in meshGeometries )
				{
					//vertices
					property = (Metadata.Property)meshGeometry.MetadataGetMemberBySignature( "property:" + nameof( Component_MeshGeometry.Vertices ) );
					undoForGeometries.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshGeometry, property, meshGeometry.Vertices ) ) );

					//indices
					property = (Metadata.Property)meshGeometry.MetadataGetMemberBySignature( "property:" + nameof( Component_MeshGeometry.Indices ) );
					undoForGeometries.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshGeometry, property, meshGeometry.Indices ) ) );
				}
			}

			//save structure
			mesh.Structure = structure;

			//save mesh geometries
			for( int i = 0; i < meshGeometries.Length; i++ )
			{
				ref var gRef = ref geomData[ i ];
				meshGeometries[ i ].Vertices = gRef.geometryVertices.ToArray();
				meshGeometries[ i ].Indices = gRef.geometryIndices.ToArray();
			}
		}

		public void RemoveUnusedData( Selection selection )
		{
			//!!!!raw вершины склеивать у одинаковых вершин структуры?
			//!!!!что еще?

			//Delete the degenerate triangles. And fill usedVertices array.

			var usedVertices = new bool[ Vertices.Count ];
			var usedEdges = new Dictionary<(int lowVertex, int highVertex), int>(); //the vertices of edge may exist but no triangles referencing the edge. 
			void AddUsedEdge( int vertex1, int vertex2 ) => usedEdges[ CommonFunctions.OrderVertices( vertex1, vertex2 ) ] = -1;

			for( int i = 0; i < Faces.Count; i++ )
			{
				var face = Faces[ i ];
				if( face == null )
					continue;

				int triangleCount = face.Triangles.Count / 3;
				for( int j = 0; j < triangleCount; j++ )
				{
					var triangles = face.Triangles;
					int start = j * 3;
					var v0 = triangles[ start ];
					var v1 = triangles[ start + 1 ];
					var v2 = triangles[ start + 2 ];

					if( v0 == null || v1 == null || v2 == null ||
						v0.Vertex == v1.Vertex || v0.Vertex == v2.Vertex || v1.Vertex == v2.Vertex
						)
					{
						triangles[ start ] = null;
						triangles[ start + 1 ] = null;
						triangles[ start + 2 ] = null;
					}

					if( face.Triangles[ start ] != null )
					{
						usedVertices[ triangles[ start ].Vertex ] = true;
						usedVertices[ triangles[ start + 1 ].Vertex ] = true;
						usedVertices[ triangles[ start + 2 ].Vertex ] = true;

						AddUsedEdge( triangles[ start ].Vertex, triangles[ start + 1 ].Vertex );
						AddUsedEdge( triangles[ start + 1 ].Vertex, triangles[ start + 2 ].Vertex );
						AddUsedEdge( triangles[ start ].Vertex, triangles[ start + 2 ].Vertex );
					}
				}
			}

			//delete the edges that are: referencing deleted vertices, degenerate, duplicate.
			for( int i = 0; i < Edges.Count; i++ )
			{
				var e = Edges[ i ];
				if( e != null )
				{
					if( e.Vertex1 == e.Vertex2 || !usedVertices[ e.Vertex1 ] || !usedVertices[ e.Vertex2 ] )
						Edges[ i ] = null;
					var ei = CommonFunctions.OrderVertices( e.Vertex1, e.Vertex2 );
					if( usedEdges.TryGetValue( ei, out int edgeIndex ) && edgeIndex == -1 )
						usedEdges[ ei ] = i; //to exclude the duplicate edges 
					else
					{
						//ToDo : Когда дублирующиеся Edge удаляются, не надо ли объединять Property?
						Edges[ i ] = null;
					}
				}
			}

			//-------------------------------------------------

			// Defragmentation of Vertices,Edges,Faces arrays.
			//ToDo ? Можно сначала проверки - нужна ли дефрагментация для каждого массива.

			var newVertices = new List<Vertex>();
			int[] vertexOldToNewMapping = new int[ Vertices.Count ]; //oldToNewMapping[oldVertexIndex]==newVertexIndex
			for( int i = 0; i < Vertices.Count; i++ )
			{
				if( !usedVertices[ i ] )
					vertexOldToNewMapping[ i ] = -1;
				else
				{
					vertexOldToNewMapping[ i ] = newVertices.Count;
					newVertices.Add( Vertices[ i ] );
				}
			}
			Vertices = newVertices;

			// defragmentation and a vertex index correction of edges
			var newEdges = new List<Edge>();
			int[] edgeOldToNewMapping = null;
			bool isEdgeCorrection = selection != null && 0 < selection.EdgeCount;
			if( isEdgeCorrection )
			{
				edgeOldToNewMapping = new int[ Edges.Count ];
				for( int i = 0; i < edgeOldToNewMapping.Length; i++ )
					edgeOldToNewMapping[ i ] = -1;
			}

			for( int i = 0; i < Edges.Count; i++ )
			{
				var e = Edges[ i ];
				if( e != null )
				{
					e.Vertex1 = vertexOldToNewMapping[ e.Vertex1 ];
					e.Vertex2 = vertexOldToNewMapping[ e.Vertex2 ];
					newEdges.Add( e );
					if( isEdgeCorrection )
						edgeOldToNewMapping[ i ] = newEdges.Count - 1;
				}
			}
			Edges = newEdges;

			//defragmentation and a vertex index correction of faces
			var newFaces = new List<Face>();

			int[] faceOldToNewMapping = null;
			bool isFaceCorrection = selection != null && 0 < selection.FaceCount;
			if( isFaceCorrection )
			{
				faceOldToNewMapping = new int[ Faces.Count ];
				for( int i = 0; i < faceOldToNewMapping.Length; i++ )
					faceOldToNewMapping[ i ] = -1;
			}
			for( int i = 0; i < Faces.Count; i++ )
			{
				var face = Faces[ i ];
				if( face == null || face.Triangles == null || face.Triangles.Count == 0 ) //face was deleted
					continue;

				//defragmentation of the triangles array
				var newTriangles = new List<FaceVertex>();
				for( int j = 0; j < face.Triangles.Count; j++ )
				{
					var v = face.Triangles[ j ];
					if( v != null )
					{
						v.Vertex = vertexOldToNewMapping[ v.Vertex ];
						newTriangles.Add( v );
					}
				}
				if( newTriangles.Count % 3 != 0 )
					throw new Exception(); //ToDo : ... Log
				if( newTriangles.Count != 0 )
				{
					face.Triangles = newTriangles;
					newFaces.Add( face );
					if( isFaceCorrection )
						faceOldToNewMapping[ i ] = newFaces.Count - 1;
				}
			}
			Faces = newFaces;

			bool isVertexCorrection = selection != null && 0 < selection.VertexCount;
			if( isVertexCorrection )
				for( int i = 0; i < selection.Vertices.Length; i++ )
				{
					int newIndex = vertexOldToNewMapping[ selection.Vertices[ i ] ];
					if( selection.Vertices[ i ] != newIndex )
					{
						selection.Vertices[ i ] = newIndex;
						selection.Changed = true;
					}
				}

			if( isEdgeCorrection )
				for( int i = 0; i < selection.Edges.Length; i++ )
				{
					int newIndex = edgeOldToNewMapping[ selection.Edges[ i ] ];
					if( selection.Edges[ i ] != newIndex )
					{
						selection.Edges[ i ] = newIndex;
						selection.Changed = true;
					}
				}

			if( isFaceCorrection )
				for( int i = 0; i < selection.Faces.Length; i++ )
				{
					int newIndex = faceOldToNewMapping[ selection.Faces[ i ] ];
					if( selection.Faces[ i ] != newIndex )
					{
						selection.Faces[ i ] = newIndex;
						selection.Changed = true;
					}
				}

			//??? Пересчет нормалей.			
		}


		//public Geometry GetGeometry( Component_MeshGeometry meshGeometry )
		//{
		//	foreach( var geometry in Geometries )
		//	{
		//		if( geometry.SourceMeshGeometry == meshGeometry )
		//			return geometry;
		//	}
		//	return null;
		//}


		//Для треугольников у которых перемещаются все 3 вертекса не надо пересчитывать normal, tangent. 
		//change position of the vertices. all affected raw vertices are updated
		public void MoveVertices( int[] vertices, Vector3F[] newPositions )
		{
			var allMoved = new HashSet<int>( vertices );
			for( int n = 0; n < vertices.Length; n++ )
				MoveVertex( vertices[ n ], newPositions[ n ], allMoved );

		}

		public void MoveVertex( int vertex, Vector3F newPosition, HashSet<int> allMoved = null )
		{
			//??? Если обновлять позже(а здесь только помечать):
			//(плюсы) Если обновлять позже - то при нескольких изменениях в одном треугольнике расчет будет только один раз.
			//(минусы) Для пересчета Tangents(с алгоритмом вращения от старой нормали) нужно запоминать старую нормаль до изменения. Не подходит сглаженная нормаль - нужен перпендикуляр.
			//        Проблема - если изменятся 2 вертекса в одном треугольнике, то надо запоминать нормаль до первого изменения.
			//		  Тогда в вызывающем коде - нужна доп. функция - запоминание исходных нормалей до первого изменения. Либо какой-то флажок, что нормаль уже запомнилась, чтобы не затереть.

			foreach( var face in Faces )
			{
				var triangles = face.Triangles;
				for( var i = 0; i < triangles.Count; i += 3 )
				{
					var v0 = triangles[ i ];
					var v1 = triangles[ i + 1 ];
					var v2 = triangles[ i + 2 ];

					if( v0.Vertex == vertex || v1.Vertex == vertex || v2.Vertex == vertex )
					{
						var oldNormal = CommonFunctions.CalculateNormal( v0.RawVertex.Position, v1.RawVertex.Position, v2.RawVertex.Position );

						if( v0.Vertex == vertex )
							v0.RawVertex.Position = newPosition;
						if( v1.Vertex == vertex )
							v1.RawVertex.Position = newPosition;
						if( v2.Vertex == vertex )
							v2.RawVertex.Position = newPosition;

						//Для треугольников у которых перемещаются все 3 вертекса не надо пересчитывать normal, tangent. 
						if( !( allMoved != null && allMoved.Contains( v0.Vertex ) && allMoved.Contains( v1.Vertex ) && allMoved.Contains( v2.Vertex ) ) )
							UpdateNormalsAndTangentsAfterPositionChange( v0, v1, v2, oldNormal );
					}
				}
			}
		}

		//ToDo ???? Вызывается из Split, при множественном split - при пересчете normal,tangent проверить корректно ли будет, не надо ли обходить граф начиная с тех что граничат с неизменяемыми?
		//    .... Если Split для всех точек треугольника, для него не будет adjacent треугольников, и tangents не смогут исправиться методом - по соседним. Проверить как здесь работает.
		//    .... При Split, возможно, вобще не надо пересчитывать normal,tangent если вертексы смещаются всегда в плоскости треугольника.

		//triangleStart and vertexIndex must be in the same triangle.
		public void MoveVertex( List<MeshData.FaceVertex> fVertices, int triangleStart, int vertexToMoveIndex, Vector3F newPosition )
		{
			Vector3F oldNormal = CommonFunctions.CalculateNormal(
				fVertices[ triangleStart ].RawVertex.Position,
				fVertices[ triangleStart + 1 ].RawVertex.Position,
				fVertices[ triangleStart + 2 ].RawVertex.Position );
			fVertices[ vertexToMoveIndex ].RawVertex.Position = newPosition;
			UpdateNormalsAndTangentsAfterPositionChange( fVertices[ triangleStart ], fVertices[ triangleStart + 1 ], fVertices[ triangleStart + 2 ], oldNormal );
		}


		//ToDo !!!!! (есть проблемы) Коррекция Tangent после перемещения работает не идеально.
		//  CalculateTriangleTangentsAndNormalByOldNormalRotation : Она вычисляет поворот от старой нормали к новой, и делает такой же поворот для Tangent. Он оказывается всегда в плоскости треугольника.
		//     Если по очереди перемещать вертексы треугольника, так что плоскость треугольника поворачивается очень сильно. Если в конце концов вернуть на то же место,
		//     то tangent повернется в плоскости треугольника (от начального положения), иногда очень сильно.
		//     !! Можно немного снизить дефект, если для  множественных перемещений точек реализовать пересчет не по одной, а сразу всех нужных в одном треугольнике
		//
		//  Возможно, нужны другие алгоритмы : 
		//		- сохранять угол между tangent и edge (только одним из 3) в плоскости  - но это тоже не всегда то что нужно (если в плоскости переместить вертекс, tangent повернется).
		//	    - Вычислять tangent заново по соседним не изменившимся треугольникам, через CalculateTriangleTangentsAndNormalByAdjacent как в BridgeEdges.
		//		  Но если изменилась очень большая область. То это равносильно полностью автоматическому расчету - его пока нет, и для него алгоритм не однозначный.
		//          Todo !!! Если использовать CalculateTriangleTangentsAndNormalByAdjacent, то видимо, доделать: Т.к. использовался в BridgeEdges для другого сценария.
		//					Доделать:
		//					- обходить граф треугольников, начиная с тех которые граничат с не меняющимися, и так постепенно все пройти.
		//					- Убирать из поиска текущий треугольник. Работает с учетом того что текущий треугольник еще не добавлен. Если он добавлен, то результат может немного ухудшиться. 
		//				    - И если нет соседних, обрабатывать как отдельный случай.
		//					- Соптимизировать поиск треугольников по вертексу - по словарю, т.к. может выполняться для множественных вертексов.
		//
		public void UpdateNormalsAndTangentsAfterPositionChange( MeshData.FaceVertex v0, MeshData.FaceVertex v1, MeshData.FaceVertex v2, Vector3F? oldNormal )
		{
			//CommonFunctions.CalculateNormal( v0, v1, v2 );

			//CommonFunctions.CalculateTriangleTangentsAndNormalByOldNormalRotation( v0, v1, v2, oldNormal.Value ); //Второй вариант.

			//ToDo : (требуется доработка) Этот вариант работает, когда у перемещаемого вертекса соседний не перемещается (когда выделено мало вертексов) . 
			// Хотя этот алгоритм тоже проверяет вращение нормали (вращение вокруг общего edge), но только для точек в общих edge, это корректно, т.к. положение точек при вращении не меняется.
			//
			CommonFunctions.CalculateTriangleTangentsAndNormalByAdjacent( this, v0, v1, v2 );
		}

		//public void MoveVertex( int vertex, Vector3F newPosition )
		//{
		//	//!!!!нужно ли сразу обновлять нормали, тангенты
		//	//!!!!!или позже. тогда может нужно пометить что потом обновить
		//	foreach( var face in Faces )
		//	{
		//		foreach( var faceVertex in face.Triangles )
		//		{
		//			if( faceVertex.Vertex == vertex )
		//				faceVertex.RawVertex.Position = newPosition;
		//		}
		//	}
		//}


		public Vector3F GetVertexPosition( int vertex )
		{
			foreach( var face in Faces )
			{
				foreach( var faceVertex in face.Triangles )
				{
					if( faceVertex.Vertex == vertex )
						return faceVertex.RawVertex.Position;
				}
			}
			return Vector3F.Zero;
		}

		public Dictionary<int, Vector3> GetVertexPositions()
		{
			int maxVertex = 0;
			foreach( var face in Faces )
				foreach( var faceVertex in face.Triangles )
					maxVertex = Math.Max( maxVertex, faceVertex.Vertex );

			var result = new Dictionary<int, Vector3>( maxVertex + 1 );

			foreach( var face in Faces )
				foreach( var faceVertex in face.Triangles )
					result[ faceVertex.Vertex ] = faceVertex.RawVertex.Position;

			return result;
		}

		public void ReplaceVertexIndexForEdgesAndFaces( int oldVertex, int newVertex )
		{
			foreach( var edge in Edges )
			{
				if( edge.Vertex1 == oldVertex )
					edge.Vertex1 = newVertex;
				if( edge.Vertex2 == oldVertex )
					edge.Vertex2 = newVertex;
			}

			foreach( var face in Faces )
			{
				for( int n = 0; n < face.Triangles.Count; n++ )
				{
					if( face.Triangles[ n ].Vertex == oldVertex )
						face.Triangles[ n ].Vertex = newVertex;
				}
			}
		}

		public void AddEdge( int vertex1, int vertex2 )
		{
			//if( FindEdge( vertex1, vertex2 ) == null )
			Edges.Add( new MeshData.Edge( vertex1, vertex2 ) );
		}

		//public Edge FindEdge( int vertex1, int vertex2 )
		//{
		//	foreach( var e in Edges )
		//	{
		//		if( e == null )
		//			continue;
		//		if( e.Vertex1 == vertex1 && e.Vertex2 == vertex2 )
		//			return e;
		//		if( e.Vertex1 == vertex2 && e.Vertex2 == vertex1 )
		//			return e;
		//	}
		//	return null;
		//}

		//public void RemoveEdgeWithVertex( int vertexIndex )
		//{
		//	for( int i = 0; i < Edges.Count; i++ )
		//	{
		//		var e = Edges[ i ];
		//		if( e != null && ( e.Vertex1 == vertexIndex || e.Vertex2 == vertexIndex ) )
		//			Edges[ i ] = null;
		//	}
		//}

		public static MeshData BuildFromRaw( List<(Vector3F[] positions, int[] indices, MeshGeometryFormat format)> geometries )
		{
			if( geometries == null || geometries.Count == 0 )
				return null;
			var ret = new MeshData();
			ret.meshGeometriesFormat = geometries.Select( _ => _.format ).ToArray();

			ret.Vertices = new List<Vertex>();
			ret.Faces = new List<Face>();
			var edges = new HashSet<(int lowVertex, int highVertex)>();

			for( int geomIndex = 0; geomIndex < geometries.Count; geomIndex++ )
			{
				var g = geometries[ geomIndex ];

				int vertexOffset = ret.Vertices.Count;
				for( int i = 0; i < g.positions.Length; i++ )
					ret.Vertices.Add( new Vertex() );

				int triangleCount = g.indices.Length / 3;
				if( g.indices.Length % 3 != 0 )
					throw new Exception();

				for( int i = 0; i < triangleCount; i++ )
				{
					int startIndex = i * 3;

					var triangles = new List<FaceVertex>();
					Vector4F tempTangent = new Vector4F( 1, 0, 0, -1 ); //ToDo : Tangent пока задается произвольный.

					var v0 = new FaceVertex( vertexOffset + g.indices[ startIndex ], geomIndex, new RawVertex( new byte[ g.format.vertexSize ], g.format ) )
					{
						RawVertex = { Position = g.positions[ g.indices[ startIndex ] ], Color = DefaultVertexColor, Tangent = tempTangent }
					};
					triangles.Add( v0 );

					var v1 = new FaceVertex( vertexOffset + g.indices[ startIndex + 1 ], geomIndex, new RawVertex( new byte[ g.format.vertexSize ], g.format ) )
					{
						RawVertex = { Position = g.positions[ g.indices[ startIndex + 1 ] ], Color = DefaultVertexColor, Tangent = tempTangent }
					};
					triangles.Add( v1 );

					var v2 = new FaceVertex( vertexOffset + g.indices[ startIndex + 2 ], geomIndex, new RawVertex( new byte[ g.format.vertexSize ], g.format ) )
					{
						RawVertex = { Position = g.positions[ g.indices[ startIndex + 2 ] ], Color = DefaultVertexColor, Tangent = tempTangent }
					};
					triangles.Add( v2 );

					CommonFunctions.CalculateNormal( v0, v1, v2 );

					var f = new Face( triangles, null, 0 );
					ret.Faces.Add( f );

					edges.Add( CommonFunctions.OrderVertices( v0.Vertex, v1.Vertex ) );
					edges.Add( CommonFunctions.OrderVertices( v1.Vertex, v2.Vertex ) );
					edges.Add( CommonFunctions.OrderVertices( v2.Vertex, v0.Vertex ) );
				}
			}

			ret.Edges = edges.Select( _ => new Edge( _.lowVertex, _.highVertex ) ).ToList();

			return ret;
		}
	}
}
#endif