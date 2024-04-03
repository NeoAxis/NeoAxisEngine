// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	//!!!!всё тут

	//ToDo ? CanExecute в Actions, ... дополнительные проверки?

	public static partial class BuilderOneMeshActions
	{
		public delegate string OneMeshActionCanExecuteDelegate( Mesh mesh );

		//undoForGeomAndStructure, undoForExternalComponent - разделены. Т.к. для ProceduralGeometry, не нужен undo в Save, но нужен undo для отмены CreateComponent<MeshGeometry>
		//
		//undoForGeomAndStructure - only for geometries of the mesh and Mesh.Structure, if all the geometries was procedural then undoForGeomAndStructure==null because they are saved already
		//undoForExternalComponent - always not null.
		public delegate void OneMeshActionExecuteDelegate( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked );
		//public delegate void OneMeshActionExecuteDelegate( Mesh mesh, UndoMultiAction undoForGeomAndStructure, UndoMultiAction undoForExternalComponent );

		//?? Внутри execute нельзя выделять объекты через WorkareaMode.Select... (т.к. mesh Disabled) 
		static void ExecuteOneMeshAction( BuilderActionContext actionContext, OneMeshActionCanExecuteDelegate canExecute, OneMeshActionExecuteDelegate execute, bool allowEdit = true )
		{
			var (meshInSpace, mesh) = actionContext.GetSelectedMesh();
			if( mesh != null )
			{
				//no mesh geometries
				if( mesh.GetComponents<MeshGeometry>().Length == 0 )
					return;

				//??? Если нужно чтобы проверка на наличие Structure делалась раньше, чем создается дочерний Mesh тогда здесь должно быть, внизу закомментировать.
				//var reason = canExecute( mesh );
				//if( !string.IsNullOrEmpty( reason ) )
				//{
				//	EditorMessageBox.ShowWarning( reason );
				//	return;
				//}

				//disable the object
				bool meshInSpaceWasEnabled = meshInSpace.Enabled;

				try
				{
					//disable mesh
					if( allowEdit )
						meshInSpace.Enabled = false;

					var document = actionContext.DocumentWindow.Document2;
					var undo = new List<UndoSystem.Action>();
					var undoLocked = false;

					//make copy of the mesh if needed
					if( allowEdit )
						BuilderCommonFunctions.CopyExternalMesh( document, meshInSpace, ref mesh, undo, ref undoLocked );

					//convert mesh geometries
					if( allowEdit )
						BuilderCommonFunctions.ConvertProceduralMeshGeometries( document, mesh, undo, ref undoLocked );

					//process action
					//Внутри Execute выделять нельзя
					execute( mesh, undo, ref undoLocked );
					//execute( mesh, needUndoForNextActions ? undo : null, undo ); //Внутри Execute выделять нельзя.

					//undo
					if( undo.Count != 0 )
					{
						var undoMultiAction = new UndoMultiAction();
						undoMultiAction.AddActions( undo );
						document.CommitUndoAction( undoMultiAction );
					}
				}
				finally
				{
					//enable mesh
					if( allowEdit )
						meshInSpace.Enabled = meshInSpaceWasEnabled;

					actionContext.ActionEnd();
				}
			}
		}


		static string CanExecute( Mesh mesh )
		{
			//if( !BuilderCommonFunctions.CheckValidMesh( mesh, out var error ) )
			//	return error;
			return "";
		}

		/////////////////////////////////////////

		public static void MergeVerticesGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.Selection.VertexCount != 0 )
				context.Enabled = true;
		}

		//Merges selected vertices into the single vertex. This new single vertex can be placed at the center of the original vertices or at the first vertex position.
		public static void MergeVertices( BuilderActionContext actionContext, bool moveToFirst )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				//load mesh data
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );

				int vi = MergeVerticesExecute( meshData, actionContext.Selection.Vertices, moveToFirst );
				actionContext.Selection.Vertices = new[] { vi };

				//copy back to the mesh
				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );

				//var old = meshGeometry.Material;
				//meshGeometry.Material = ResourceUtility.MaterialInvalid;
				//var property = (Metadata.Property)meshGeometry.MetadataGetMemberBySignature( "property:" + nameof( MeshGeometry.Material ) );
				//undoForGeomAndStructure.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshGeometry, property, old ) ) );

			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		static int MergeVerticesExecute( BuilderMeshData meshData, int[] verticesToMerge, bool moveToFirst )
		{
			int mergedVertex = verticesToMerge[ 0 ];
			//get destination position
			Vector3F toPosition;
			{
				if( moveToFirst )
					toPosition = meshData.GetVertexPosition( mergedVertex );
				else
				{
					toPosition = Vector3F.Zero;
					foreach( var vertexToMerge in verticesToMerge )
						toPosition += meshData.GetVertexPosition( vertexToMerge );
					toPosition /= verticesToMerge.Length;
				}
			}

			//change position of the vertices. all affected raw vertices are updated
			foreach( var vertexToMerge in verticesToMerge )
				meshData.MoveVertex( vertexToMerge, toPosition );

			//replace vertex indexes for edges, faces. raw data is saved, only vertex indexes are updated
			for( int n = 1; n < verticesToMerge.Length; n++ )
				meshData.ReplaceVertexIndexForEdgesAndFaces( verticesToMerge[ n ], mergedVertex );

			return mergedVertex;
		}

		/////////////////////////////////////////

		public static void DeleteFacesGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.Selection.FaceCount != 0 )
				context.Enabled = true;
		}

		public static void DeleteFaces( BuilderActionContext actionContext )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var selFaces = actionContext.Selection.Faces;
				if( selFaces.Length == 0 )
					return;
				string s = selFaces.Length > 1 ? "s" : "";
				if( EditorMessageBox.ShowQuestion( $"Delete selected face{s}?", EMessageBoxButtons.YesNo ) == EDialogResult.No )
					return;
				DeleteFacesExecute( mesh, selFaces, actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		static void DeleteFacesExecute( Mesh mesh, int[] facesToDelete, BuilderSelection selection, List<UndoSystem.Action> undo, ref bool undoLocked )
		{
			if( facesToDelete == null || facesToDelete.Length == 0 )
				return;

			var meshData = new BuilderMeshData();
			var extractedData = mesh.ExtractData();
			meshData.Load( extractedData );

			//??? Так удалять , deffered ?
			for( int i = 0; i < facesToDelete.Length; i++ )
				meshData.Faces[ facesToDelete[ i ] ] = null;

			meshData.Save( mesh, selection, undo, ref undoLocked );
		}

		/////////////////////////////////////////

		public static void MoveVertices( BuilderActionContext actionContext, int[] vertices, Vector3F[] newPositions )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				//load mesh data
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );

				meshData.MoveVertices( vertices, newPositions );

				//copy back to the mesh
				meshData.Save( mesh, selection: actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		/////////////////////////////////////////
		#region SplitVertices

		const float SplitVertexOffset = 15f;

		public static void SplitVerticesGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.Selection.VertexCount != 0 )
				context.Enabled = true;
		}

		//Makes the vertices that are shared by many triangles independent. A new independent vertex is created for each triangle.
		public static void SplitVertices( BuilderActionContext actionContext, bool splitOnlyOne, bool shiftSplitVertices )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );
				var selVertices = actionContext.Selection.Vertices;
				List<int>[] newVertices;
				if( HasMultipleFacesVorVertex( meshData, selVertices ) )
					newVertices = SplitVerticesExecutePreserveFace( meshData, selVertices, splitOnlyOne, shiftSplitVertices );
				else
					newVertices = SplitVerticesExecute( meshData, selVertices, splitOnlyOne, true );

				actionContext.Selection.ClearSelection();
				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
			}

			//? Можно было бы оставлять выделение если не было разделения, но при нескольких vertex, не совсем логично что часть выделится часть нет.
			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		static bool HasMultipleFacesVorVertex( BuilderMeshData meshData, int[] vertices )
		{
			foreach( var vertex in vertices )
			{
				int foundFace = -1;
				for( int i = 0; i < meshData.Faces.Count; i++ )
				{
					var f = meshData.Faces[ i ];
					for( int j = 0; j < f.Triangles.Count; j++ )
					{
						if( f.Triangles[ j ].Vertex == vertex )
						{
							if( foundFace == -1 )
								foundFace = i;
							else
							{
								if( foundFace != i )
									return true;
							}
						}
					}
				}
			}
			return false;
		}

		//returns new vertices for each split vertex.
		static List<int>[] SplitVerticesExecute( BuilderMeshData meshData, int[] verticesToSplit, bool splitOnlyOne, bool shiftSplitVertices )
		{
			var ret = new List<int>[ verticesToSplit.Length ];

			if( verticesToSplit.Length == 0 )
				return new List<int>[ 0 ];

			for( int i = 0; i < verticesToSplit.Length; i++ )
			{
				var fVertices = BuilderCommonFunctions.GetTrianglesByVertex( meshData, verticesToSplit[ i ] );
				if( 3 < fVertices.Count )
				{
					ret[ i ] = new List<int>();
					//meshData.RemoveEdgeWithVertex( verticesToSplit[ i ] );

					int lastIndex = splitOnlyOne ? 2 * 3 : fVertices.Count;
					var edges = new Dictionary<(int lowVertex, int highVertex), int>();

					for( int j = 0; j < lastIndex; j += 3 )
					{
						var fv = fVertices[ j ];

						var newVertex = new BuilderMeshData.Vertex( BuilderMeshData.CloneProperties( meshData.Vertices[ fv.Vertex ].Properties ) );
						meshData.Vertices.Add( newVertex );
						fv.Vertex = meshData.Vertices.Count - 1;
						ret[ i ].Add( fv.Vertex );

						AddEdge( edges, fVertices[ j ].Vertex, fVertices[ j + 1 ].Vertex );
						AddEdge( edges, fVertices[ j ].Vertex, fVertices[ j + 2 ].Vertex );

						if( shiftSplitVertices )
						{
							var offset = fVertices[ j + 1 ].RawVertex.Position - fVertices[ j ].RawVertex.Position;
							offset += fVertices[ j + 2 ].RawVertex.Position - fVertices[ j ].RawVertex.Position;
							offset /= 2 * SplitVertexOffset;
							//the vertices are reordered, the first FaceVertex in a triangle is bound to the vertex that was split
							meshData.MoveVertex( fVertices, j, j, fVertices[ j ].RawVertex.Position + offset );
						}
					}
					foreach( var e in edges )
						if( e.Value == 1 )
							meshData.AddEdge( e.Key.lowVertex, e.Key.highVertex );
				}
			}

			return ret;
		}


		//returns new vertices for each split vertex.
		static List<int>[] SplitVerticesExecutePreserveFace( BuilderMeshData meshData, int[] verticesToSplit, bool splitOnlyOne, bool shiftSplitVertices )
		{
			var ret = new List<int>[ verticesToSplit.Length ];

			if( verticesToSplit.Length == 0 )
				return new List<int>[ 0 ];

			for( int i = 0; i < verticesToSplit.Length; i++ )
			{
				ret[ i ] = new List<int>();
				//meshData.RemoveEdgeWithVertex( verticesToSplit[ i ] );

				var fVerticesByFace = GetTrianglesByVertexGroupedByFace( meshData, verticesToSplit[ i ] );
				if( 1 < fVerticesByFace.Count )
				{
					int lastIndex = splitOnlyOne ? 2 : fVerticesByFace.Count;

					for( int j = 0; j < lastIndex; j++ )
					{
						var fvForFace = fVerticesByFace[ j ];

						var newVertex = new BuilderMeshData.Vertex( BuilderMeshData.CloneProperties( meshData.Vertices[ fvForFace[ 0 ].Vertex ].Properties ) ); //ToDo ??? Какие задавать Property?
						meshData.Vertices.Add( newVertex );
						int newVertexIndex = meshData.Vertices.Count - 1;
						ret[ i ].Add( newVertexIndex );

						var edges = new Dictionary<(int lowVertex, int highVertex), int>();
						Vector3F offset = Vector3F.Zero;
						for( int k = 0; k < fvForFace.Count; k += 3 )
						{
							fvForFace[ k ].Vertex = newVertexIndex;
							AddEdge( edges, fvForFace[ k ].Vertex, fvForFace[ k + 1 ].Vertex );
							AddEdge( edges, fvForFace[ k ].Vertex, fvForFace[ k + 2 ].Vertex );
							if( shiftSplitVertices )
							{
								offset += ( fvForFace[ k + 1 ].RawVertex.Position - fvForFace[ k ].RawVertex.Position );
								offset += ( fvForFace[ k + 2 ].RawVertex.Position - fvForFace[ k ].RawVertex.Position );
							}
						}

						if( shiftSplitVertices )
						{
							offset /= 2 * ( fvForFace.Count / 3 ) * SplitVertexOffset;
							for( int k = 0; k < fvForFace.Count; k += 3 ) //the vertices are reordered, the first FaceVertex in a triangle is bound to the vertex that was split
								meshData.MoveVertex( fvForFace, k, k, fvForFace[ k ].RawVertex.Position + offset );
						}

						foreach( var e in edges )
							if( e.Value == 1 )
								meshData.AddEdge( e.Key.lowVertex, e.Key.highVertex );
					}
				}
			}

			return ret;
		}

		//static List<(int faceIndex, int faceVertexIndex)> GetFaceVerticesIndicesByVertex( MeshData meshData, int vertexIndex )
		//{
		//	var ret = new List<(int faceIndex, int faceVertexIndex)>();
		//	for( int i = 0; i < meshData.Faces.Count; i++ )
		//	{
		//		var f = meshData.Faces[ i ];
		//		for( int j = 0; j < f.Triangles.Count; j++ )
		//		{
		//			if( f.Triangles[ j ].Vertex == vertexIndex )
		//				ret.Add( (i, j) );
		//		}
		//	}
		//	return ret;
		//}


		//static List<List<MeshData.FaceVertex>> GetFaceVerticesByVertexGroupedByFace( MeshData meshData, int vertexIndex )
		//{
		//	var ret = new List<List<MeshData.FaceVertex>>();
		//	for( int i = 0; i < meshData.Faces.Count; i++ )
		//	{
		//		var f = meshData.Faces[ i ];
		//		List<MeshData.FaceVertex> foundInFace = null;
		//		for( int j = 0; j < f.Triangles.Count; j++ )
		//		{
		//			if( f.Triangles[ j ].Vertex == vertexIndex )
		//			{
		//				if( foundInFace == null )
		//					foundInFace = new List<MeshData.FaceVertex>();
		//				foundInFace.Add( f.Triangles[ j ] );
		//			}
		//		}
		//		if( foundInFace != null )
		//			ret.Add( foundInFace );
		//	}
		//	return ret;
		//}


		//в треугольниках точки переупорядочены, так что на первом месте идет точка для vertexIndex, но не нарушен порядок следования точек против часовой стрелки.
		static List<List<BuilderMeshData.FaceVertex>> GetTrianglesByVertexGroupedByFace( BuilderMeshData meshData, int vertexIndex )
		{
			var ret = new List<List<BuilderMeshData.FaceVertex>>();
			for( int i = 0; i < meshData.Faces.Count; i++ )
			{
				var f = meshData.Faces[ i ];
				List<BuilderMeshData.FaceVertex> foundInFace = null;
				for( int j = 0; j < f.Triangles.Count; j++ )
				{
					if( f.Triangles[ j ].Vertex == vertexIndex )
					{
						if( foundInFace == null )
							foundInFace = new List<BuilderMeshData.FaceVertex>();
						foundInFace.Add( f.Triangles[ j ] );

						int start = j - j % 3;
						if( j == start )
						{
							foundInFace.Add( f.Triangles[ start + 1 ] );
							foundInFace.Add( f.Triangles[ start + 2 ] );
						}
						else if( j == start + 1 )
						{
							foundInFace.Add( f.Triangles[ start + 2 ] );
							foundInFace.Add( f.Triangles[ start ] );
						}
						else if( j == start + 2 )
						{
							foundInFace.Add( f.Triangles[ start ] );
							foundInFace.Add( f.Triangles[ start + 1 ] );
						}
					}
				}
				if( foundInFace != null )
					ret.Add( foundInFace );
			}
			return ret;
		}

		#endregion

		/////////////////////////////////////////

		public static void FlipNormalsGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object && actionContext.GetSelectedMeshInSpaces().Length == 1 ||
				 actionContext.Selection.FaceCount != 0 )
				context.Enabled = true;
		}

		//Inverts the normals of selected faces.
		public static void FlipNormals( BuilderActionContext actionContext )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var extractedData = mesh.ExtractData();
				var meshData = new BuilderMeshData();
				meshData.Load( extractedData );

				if( actionContext.SelectionMode == BuilderSelectionMode.Object )
					for( int i = 0; i < meshData.Faces.Count; i++ )
						FlipNormals( meshData, i );//, undoForGeomAndStructure );
				else if( actionContext.SelectionMode == BuilderSelectionMode.Face )
					foreach( var fi in actionContext.Selection.Faces )
						FlipNormals( meshData, fi );//, undoForGeomAndStructure );

				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		static void FlipNormals( BuilderMeshData meshData, int faceIndex )//, UndoMultiAction undoForGeomAndStructure )
		{
			var face = meshData.Faces[ faceIndex ];
			for( int i = 0; i < face.Triangles.Count; i += 3 )
			{
				var temp = face.Triangles[ i ];
				face.Triangles[ i ] = face.Triangles[ i + 1 ];
				face.Triangles[ i + 1 ] = temp;

				face.Triangles[ i ].RawVertex.Normal = -face.Triangles[ i ].RawVertex.Normal;
				face.Triangles[ i + 1 ].RawVertex.Normal = -face.Triangles[ i + 1 ].RawVertex.Normal;
				face.Triangles[ i + 2 ].RawVertex.Normal = -face.Triangles[ i + 2 ].RawVertex.Normal;
			}
		}


		/////////////////////////////////////////

		#region BridgeEdges  

		public static void BridgeEdgesGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			var selEdges = actionContext.Selection.Edges;
			if( selEdges.Length == 2 )
			{
				var meshData = actionContext.BuilderWorkareaMode.meshToEditData; //ToDo ???? Правильно брать отсюда?
				var tA = FindTriangles( meshData, meshData.Edges[ selEdges[ 0 ] ].Vertex1, meshData.Edges[ selEdges[ 0 ] ].Vertex2 );
				var tB = FindTriangles( meshData, meshData.Edges[ selEdges[ 1 ] ].Vertex1, meshData.Edges[ selEdges[ 1 ] ].Vertex2 );
				if( tA.Count == 1 && tB.Count == 1 )
					context.Enabled = true;
			}
		}

		//Connects two selected edges with a new face. If the edges have a common vertex then a single triangle is added. If the edges do not have a common vertex then two triangles are added.
		public static void BridgeEdges( BuilderActionContext actionContext )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var selEdges = actionContext.Selection.Edges;
				if( selEdges.Length == 2 )
				{
					var meshData = new BuilderMeshData();
					var extractedData = mesh.ExtractData();
					meshData.Load( extractedData );

					BridgeEdgesExecute( meshData, selEdges[ 0 ], selEdges[ 1 ] );
					meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
				}
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		//Соединяет 2 Edge треугольниками. Если 2 edge имеют общую точку, то один треугольник. Если нет общей точки, то 2 треугольника.
		//Для созданных треугольников, надо определить в какую сторону направлена нормаль - один из двух вариантов(нуружу или внутрь).
		//Для определения направления нормали анализируются соседние треугольники.
		//Учитывается порядок следования точек. Используется соглашение - в треугольнике точки следуют против часовой стрелки, если смотреть навстречу нормали (перпендикулярной к плоскости)
		//(нормаль записанная в вертексе, не должна отклоняться от этого направления больше чем на 90 градусов).
		//Если нормаль направлена в противоположную сторону от этого соглашения, назовем - flipped. Сейчас это не учитывается, созданные треугольники не будут flipped. - Если это надо можно будет сделать.

		static void BridgeEdgesExecute( BuilderMeshData meshData, int edge1, int edge2 )
		{
			int vertexA1 = meshData.Edges[ edge1 ].Vertex1;
			int vertexA2 = meshData.Edges[ edge1 ].Vertex2;
			int vertexB1 = meshData.Edges[ edge2 ].Vertex1;
			int vertexB2 = meshData.Edges[ edge2 ].Vertex2;

			bool areAdjacent = false;
			{
				int vertexCommon = -1;
				int vertexA = -1;
				int vertexB = -1;

				if( vertexA1 == vertexB1 )
				{
					vertexCommon = vertexA1;
					vertexA = vertexA2;
					vertexB = vertexB2;
				}
				else if( vertexA1 == vertexB2 )
				{
					vertexCommon = vertexA1;
					vertexA = vertexA2;
					vertexB = vertexB1;
				}
				else if( vertexA2 == vertexB1 )
				{
					vertexCommon = vertexA2;
					vertexA = vertexA1;
					vertexB = vertexB2;
				}
				else if( vertexA2 == vertexB2 )
				{
					vertexCommon = vertexA2;
					vertexA = vertexA1;
					vertexB = vertexB1;
				}

				if( vertexCommon != -1 ) // adjacent
				{
					areAdjacent = true;

					BridgeAdjacentEdges( meshData, vertexA, vertexCommon, vertexB );
				}
			}

			if( !areAdjacent )
			{
				BridgeIndependentEdges( meshData, vertexA1, vertexA2, vertexB1, vertexB2 );
			}
		}

		static void BridgeIndependentEdges( BuilderMeshData meshData, int vertexA1, int vertexA2, int vertexB1, int vertexB2 )
		{
			var tAs = FindTriangles( meshData, vertexA1, vertexA2 );
			var tBs = FindTriangles( meshData, vertexB1, vertexB2 );
			if( tAs.Count != 1 || tBs.Count != 1 )
			{
				//Bridge Edges requires that the edges must be bound to a single triangle.
				//ToDo : Message, Log
				return;
			}

			var tA = tAs[ 0 ];
			var tB = tBs[ 0 ];

			int meshGeom = tA.fv0.RawGeometry; //ToDo ? Если в EdgeA из разных geometry?

			if( tA.reversed )
				Exchange( ref vertexA1, ref vertexA2 );
			if( tB.reversed )
				Exchange( ref vertexB1, ref vertexB2 );

			var pA1 = meshData.GetVertexPosition( vertexA1 );
			var pA2 = meshData.GetVertexPosition( vertexA2 );
			var pB1 = meshData.GetVertexPosition( vertexB1 );
			var pB2 = meshData.GetVertexPosition( vertexB2 );

			//ToDo ? TexCoord
			{
				//second triangle on the same edge has reverse order (A2,A1,..)
				var fv0 = meshData.CreateFaceVertex( vertexA2, meshGeom );
				fv0.RawVertex.Position = pA2;
				//fv0.RawVertex.

				var fv1 = meshData.CreateFaceVertex( vertexA1, meshGeom );
				fv1.RawVertex.Position = pA1;

				var fv2 = meshData.CreateFaceVertex( vertexB1, meshGeom );
				fv2.RawVertex.Position = pB1;

				BuilderCommonFunctions.CalculateTriangleTangentsAndNormalByAdjacent( meshData, fv0, fv1, fv2 );
				meshData.Faces.Add( new BuilderMeshData.Face(
					new List<BuilderMeshData.FaceVertex>() { fv0, fv1, fv2 },
					null,
					0 //ToDo ? SmoothingGroup
				) );
				meshData.AddEdge( vertexA1, vertexB1 );
				meshData.AddEdge( vertexA2, vertexB1 );
			}
			{
				//second triangle on the same edge has reverse order (B2,B1,..)
				var fv0 = meshData.CreateFaceVertex( vertexB2, meshGeom );
				fv0.RawVertex.Position = pB2;

				var fv1 = meshData.CreateFaceVertex( vertexB1, meshGeom );
				fv1.RawVertex.Position = pB1;

				var fv2 = meshData.CreateFaceVertex( vertexA1, meshGeom );
				fv2.RawVertex.Position = pA1;

				BuilderCommonFunctions.CalculateTriangleTangentsAndNormalByAdjacent( meshData, fv0, fv1, fv2 );
				meshData.Faces.Add( new BuilderMeshData.Face(
					new List<BuilderMeshData.FaceVertex>() { fv0, fv1, fv2 },
					null,
					0 //ToDo ? SmoothingGroup
				) );
				meshData.AddEdge( vertexA1, vertexB2 );
			}

		}

		static void BridgeAdjacentEdges( BuilderMeshData meshData, int vertexA, int vertexCommon, int vertexB )
		{
			//build triangle (A,B,Common) or reverse(B,A,Common)

			var tAs = FindTriangles( meshData, vertexA, vertexCommon );
			var tBs = FindTriangles( meshData, vertexCommon, vertexB );
			if( tAs.Count != 1 || tBs.Count != 1 )
			{
				//Bridge Edges requires that the edges must be bound to a single triangle.
				//ToDo : Message, Log
				return;
			}

			var tA = tAs[ 0 ];
			var tB = tBs[ 0 ];

			if( tA.reversed != tB.reversed )
			{
				//Can not conform normals for Bridge Edges.
				//ToDo : Log, or message.
				return;
			}
			if( tA.reversed )
			{
				Exchange( ref tA, ref tB );
				Exchange( ref vertexA, ref vertexB );
				//tA.reversed = false; tB.reversed = false;
			}

			int meshGeom = tA.fv0.RawGeometry; //ToDo ? Если в EdgeA из разных geometry?

			var fv0 = meshData.CreateFaceVertex( vertexA, meshGeom );
			fv0.RawVertex.Position = meshData.GetVertexPosition( vertexA );

			var fv1 = meshData.CreateFaceVertex( vertexB, meshGeom );
			fv1.RawVertex.Position = meshData.GetVertexPosition( vertexB );

			var fv2 = meshData.CreateFaceVertex( vertexCommon, meshGeom );
			fv2.RawVertex.Position = meshData.GetVertexPosition( vertexCommon );

			BuilderCommonFunctions.CalculateTriangleTangentsAndNormalByAdjacent( meshData, fv0, fv1, fv2 );

			meshData.Faces.Add( new BuilderMeshData.Face(
				new List<BuilderMeshData.FaceVertex>() { fv0, fv1, fv2 },
				null,
				0 //ToDo ? SmoothingGroup
			) );
			meshData.AddEdge( vertexA, vertexB );
		}

		static void Exchange<T>( ref T val1, ref T val2 )
		{
			T temp = val1;
			val1 = val2;
			val2 = temp;
		}

		//return value - треугольники переупорядочены, так что vertex1,vertex2 идут на первом месте. Но проядок следования вершин сохранен (против часовой стрелки).
		//Если reversed==false, то треугольник (vertex1,vertex2,...) ; Если reversed==true, то треугольник (vertex2,vertex1,...)
		internal/*obfuscator*/ static List<(BuilderMeshData.FaceVertex fv0, BuilderMeshData.FaceVertex fv1, BuilderMeshData.FaceVertex fv2, bool reversed)> FindTriangles( BuilderMeshData data, int vertex1, int vertex2 )
		{
			var ret = new List<(BuilderMeshData.FaceVertex fv1, BuilderMeshData.FaceVertex fv2, BuilderMeshData.FaceVertex fv3, bool reversed)>();
			for( int i = 0; i < data.Faces.Count; i++ )
			{
				var t = data.Faces[ i ].Triangles;
				for( int j = 0; j < t.Count; j += 3 )
				{
					var v0 = t[ j ];
					var v1 = t[ j + 1 ];
					var v2 = t[ j + 2 ];

					if( v0.Vertex == vertex1 && v1.Vertex == vertex2 )
						ret.Add( (v0, v1, v2, false) );
					else if( v1.Vertex == vertex1 && v2.Vertex == vertex2 )
						ret.Add( (v1, v2, v0, false) );
					else if( v2.Vertex == vertex1 && v0.Vertex == vertex2 )
						ret.Add( (v2, v0, v1, false) );

					if( v0.Vertex == vertex2 && v1.Vertex == vertex1 )
						ret.Add( (v0, v1, v2, true) );
					else if( v1.Vertex == vertex2 && v2.Vertex == vertex1 )
						ret.Add( (v1, v2, v0, true) );
					else if( v2.Vertex == vertex2 && v0.Vertex == vertex1 )
						ret.Add( (v2, v0, v1, true) );
				}
			}
			return ret;
		}

		#endregion

		/////////////////////////////////////////

		#region ConformNormals  

		public static void ConformNormalsGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( /*actionContext.GetSelectedMesh() != null && */
				( actionContext.SelectionMode == BuilderSelectionMode.Object && actionContext.GetSelectedMeshInSpaces().Length == 1 ||
				  0 < actionContext.Selection.FaceCount ) )
				context.Enabled = true;
		}

		//??? действует на весь объект, а не на выделенные Faces. Или сделать чтобы действовало только на выделенные?

		//Makes all normals in the mesh conformed to each other. There are two possible directions for conformed normals, which one to use is determined by the majority of triangles.
		public static void ConformNormals( BuilderActionContext actionContext )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );

				ConformNormals( meshData.Faces );
				RecalculateNegativeNormals( meshData );

				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		//---------------------

		//ConformNormals :
		// Анализирует направления нормалей, если для меньшинство треугольников нормали не соответствуют большинству, то исправляется для меньшинства.
		//
		// Что значит соответствие нормалей: Например, для книги лицевые стороны передней и задней обложки соответствуют (conformed). Потому что, если книгу развернуть,
		// нормали обложек совпадут. Хотя когда книга сложена, нормали противоположны, но по прежнему conformed. Т.к. поворот в edge(месте соединения) не меняет соответствие(conformed).
		//
		// Если в одном Edge сходятся 3 треугольника. Их не возможно всех привести в Conformed состояние. Но таких фигур не должно быть. (ToDo ? можно было бы выдавать warning, если для одного edge 3 треугольника )
		//
		// Если объект содержит несколько не связаных частей, то они обрабатываются независимо. 
		//
		//ToDo ? Протестировать еще на несвязных фигурах (состоящих из нескольких отдельных частей)
		internal/*obfuscator*/ static void ConformNormals( List<BuilderMeshData.Face> faces )
		{
			//На одном edge не должно быть больше 2 треугольников, иначе их нельзя сделать conformant.

			//Для определения ориентации треугольника, анализируется порядок следования вертексов в треугольнике(против часовой).
			//RawVertex.Normal, пока не учитывается. При инвертировании нормали меняется порядок следования точек, а RawVertex.Normal заменяется на отрицательный.

			// vertex indices are ordered in a key (always vertexLow < vertexHigh) .
			//(ToDo ? optimize) Чтобы для каждого Edge не создавался List, вместо List можно структуру с 2 элементами + List если редко будут дополнительные
			var edges = new Dictionary<(int vertexLow, int vertexHigh), List<(int faceIndex, int triangleIndex, bool reversed)>>();

			void AddTriangle( int faceIndex, int triangleIndex, int vertex1, int vertex2 ) //vertex1, vertex2 are ordered
			{
				bool reverse = false;
				if( vertex2 < vertex1 )
				{
					Exchange( ref vertex1, ref vertex2 );
					reverse = true;
				}

				if( !edges.TryGetValue( (vertex1, vertex2), out var list ) )
				{
					list = new List<(int faceIndex, int triangleIndex, bool reversed)>();
					edges[ (vertex1, vertex2) ] = list;
				}
				list.Add( (faceIndex, triangleIndex, reverse) );
			}

			//--------------

			for( int i = 0; i < faces.Count; i++ )
			{
				var f = faces[ i ];

				for( int j = 0; j < f.Triangles.Count; j += 3 )
				{
					int tIndex = j / 3;
					AddTriangle( i, tIndex, f.Triangles[ j ].Vertex, f.Triangles[ j + 1 ].Vertex );
					AddTriangle( i, tIndex, f.Triangles[ j + 1 ].Vertex, f.Triangles[ j + 2 ].Vertex );
					AddTriangle( i, tIndex, f.Triangles[ j + 2 ].Vertex, f.Triangles[ j ].Vertex );
				}
			}

			//Временные метки на треугольниках, для обхода графа связей треугольников, triangleMarks[faceIndex][triangleIndex] ==  ConformantMark
			ConformantMark[][] triangleMarks = new ConformantMark[ faces.Count ][];
			for( int i = 0; i < faces.Count; i++ )
				triangleMarks[ i ] = new ConformantMark[ faces[ i ].Triangles.Count / 3 ];

			//------------------

			var stack = new Stack<(int faceIndex, int triangleIndex)>(); //в стеке лежат треугольники, для которых уже вычислено Conformant, но надо вычислить для его соседей.

			// Алгоритм обходит граф связей треугольников(связи через edge). Начиная с произвольного треугольника. Помечая пройденые треугольники.
			// Если несколько компонент связности, затем ищет любой не пройденый, и снова проходит по связям. Пока не останется не пройденных.
			while( true )
			{
				//find the first not processed triangle
				(int faceIndex, int triangleIndex)? notProcessed = null;
				for( int i = 0; i < triangleMarks.Length; i++ )
				{
					for( int j = 0; j < triangleMarks[ i ].Length; j++ )
					{
						if( triangleMarks[ i ][ j ] == ConformantMark.NotProcessed )
						{
							notProcessed = (i, j);
							break;
						}
					}
					if( notProcessed != null )
						break;
				}

				if( notProcessed == null )
					break;
				triangleMarks[ notProcessed.Value.faceIndex ][ notProcessed.Value.triangleIndex ] = ConformantMark.Conformant; //Первый треугольник, для начала помечаем как Conformant. Хотя если таких будет меньшинство, то они будут flipped
				stack.Push( (notProcessed.Value.faceIndex, notProcessed.Value.triangleIndex) ); //в стеке лежат треугольинки уже помеченные Conformant или NonConformant, но надо проверить их соседей.
				ConformNormals( faces, edges, triangleMarks, stack );

				// Если Mesh был не связный. То надо выбрать не обработанный треугольник и продолжить для него.
			}
		}

		enum ConformantMark { NotProcessed = 0, Conformant, NonConformant, Done }

		static void ConformNormals(
			List<BuilderMeshData.Face> faces,
			Dictionary<(int vertexLow, int vertexHigh), List<(int faceIndex, int triangleIndex, bool reversed)>> edges,
			ConformantMark[][] triangleMarks,
			Stack<(int faceIndex, int triangleIndex)> stack )
		{

			while( stack.Count != 0 )
			{
				var ti = stack.Pop();
				ConformantMark mark = triangleMarks[ ti.faceIndex ][ ti.triangleIndex ];
				bool isConformant;
				if( mark == ConformantMark.Conformant )
					isConformant = true;
				else if( mark == ConformantMark.NonConformant )
					isConformant = false;
				else
					continue;

				var triangles = faces[ ti.faceIndex ].Triangles;
				int start = ti.triangleIndex * 3;

				ProcEdge( triangles[ start ].Vertex, triangles[ start + 1 ].Vertex, isConformant );
				ProcEdge( triangles[ start + 1 ].Vertex, triangles[ start + 2 ].Vertex, isConformant );
				ProcEdge( triangles[ start + 2 ].Vertex, triangles[ start ].Vertex, isConformant );
			}

			int conformantCount = 0;
			int nonConformantCount = 0;
			foreach( var f in triangleMarks )
			{
				foreach( var t in f )
				{
					if( t == ConformantMark.Conformant )
						conformantCount++;
					else if( t == ConformantMark.NonConformant )
						nonConformantCount++;
				}
			}

			bool reverseNonConformant = nonConformantCount < conformantCount;
			for( int i = 0; i < faces.Count; i++ )
			{
				var f = faces[ i ];
				int tCount = f.Triangles.Count / 3;
				for( int j = 0; j < tCount; j++ )
				{
					if( reverseNonConformant && triangleMarks[ i ][ j ] == ConformantMark.NonConformant ||
						!reverseNonConformant && triangleMarks[ i ][ j ] == ConformantMark.Conformant )
					{
						int start = j * 3;
						var temp = f.Triangles[ start ];
						f.Triangles[ start ] = f.Triangles[ start + 1 ];
						f.Triangles[ start + 1 ] = temp;

						f.Triangles[ start ].RawVertex.Normal = -f.Triangles[ start ].RawVertex.Normal;
						f.Triangles[ start + 1 ].RawVertex.Normal = -f.Triangles[ start + 1 ].RawVertex.Normal;
						f.Triangles[ start + 2 ].RawVertex.Normal = -f.Triangles[ start + 2 ].RawVertex.Normal;
					}
				}
			}

			foreach( var f in triangleMarks )
			{
				for( int i = 0; i < f.Length; i++ )
				{
					if( f[ i ] == ConformantMark.Conformant || f[ i ] == ConformantMark.NonConformant )
						f[ i ] = ConformantMark.Done;
				}
			}

			// ------------------------------------------------

			void ProcEdge( int v1, int v2, bool isConformant )
			{
				bool reversed = false;
				if( v2 < v1 )
				{
					Exchange( ref v1, ref v2 );
					reversed = true;
				}

				var list = edges[ (v1, v2) ];
				foreach( var item in list )
				{
					if( triangleMarks[ item.faceIndex ][ item.triangleIndex ] == ConformantMark.NotProcessed ) //еще не вычисленый треугольник.
					{
						bool different = item.reversed != reversed; //для соседних треугольников порядок следования вертексов должен отличаться.
						bool isCurConformant = different ? isConformant : !isConformant;
						triangleMarks[ item.faceIndex ][ item.triangleIndex ] = isCurConformant ? ConformantMark.Conformant : ConformantMark.NonConformant;
						stack.Push( (item.faceIndex, item.triangleIndex) );
					}
				}
			}
		}

		//Если теоретическая нормаль(перпендикуляр к треугольнику) отличается больше чем на 90 градусов от custom нормали прописанной в RawVertex.Normal, то прописывается перпендикулярная нормаль.
		static void RecalculateNegativeNormals( BuilderMeshData meshData )
		{
			foreach( var face in meshData.Faces )
			{
				for( int i = 0; i < face.Triangles.Count; i += 3 )
				{
					var v0 = face.Triangles[ i ].RawVertex;
					var v1 = face.Triangles[ i + 1 ].RawVertex;
					var v2 = face.Triangles[ i + 2 ].RawVertex;

					var realNormal = BuilderCommonFunctions.CalculateNormal( v0.Position, v1.Position, v2.Position );
					if( Vector3F.Dot( realNormal, v0.Normal ) < 0 || Vector3F.Dot( realNormal, v1.Normal ) < 0 || Vector3F.Dot( realNormal, v2.Normal ) < 0 )
					{
						v0.Normal = realNormal;
						v1.Normal = realNormal;
						v2.Normal = realNormal;
					}
				}
			}
		}

		#endregion

		/////////////////////////////////////////

		public static void FlatNormalsGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object && actionContext.GetSelectedMeshInSpaces().Length == 1 || actionContext.Selection.FaceCount != 0 )
				context.Enabled = true;
		}

		//Recalculates the normals of selected faces so that they do not have smoothing (perpendicular to the planes of the triangles).
		public static void FlatNormals( BuilderActionContext actionContext )
		{

			Mesh selMesh = null;
			int[] selFaces = null;
			if( actionContext.SelectionMode == BuilderSelectionMode.Object )
				selMesh = actionContext.GetSelectedMesh().mesh;
			else
				selFaces = actionContext.Selection.Faces;


			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );

				if( selMesh != null )
					foreach( var face in meshData.Faces )
						CalculateNormals( face.Triangles );
				else if( selFaces != null )
					foreach( var fi in selFaces )
						CalculateNormals( meshData.Faces[ fi ].Triangles );

				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		internal/*obfuscator*/ static void CalculateNormals( List<BuilderMeshData.FaceVertex> triangles )
		{
			for( int i = 0; i < triangles.Count; i += 3 )
				BuilderCommonFunctions.CalculateNormal( triangles[ i ], triangles[ i + 1 ], triangles[ i + 2 ] );
		}

		/////////////////////////////////////////

		public static void SmoothNormalsGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object && actionContext.GetSelectedMeshInSpaces().Length == 1 || 0 < actionContext.Selection.FaceCount )
				context.Enabled = true;
		}

		//Recalculates the normals of selected faces to smooth them.
		public static void SmoothNormals( BuilderActionContext actionContext )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );

				int[] selFaces = actionContext.SelectionMode == BuilderSelectionMode.Object ?
					Enumerable.Range( 0, meshData.Faces.Count ).ToArray() :
					actionContext.Selection.Faces;

				Vector3F[] vertexAvgNormals = new Vector3F[ meshData.Vertices.Count ];

				//??? Надо пресчитывать normals при сглаживании, или сглаживать существующие?
				foreach( var fi in selFaces )
					CalculateNormals( meshData.Faces[ fi ].Triangles );

				// Надо усреднять не только по выделенным face, но и по граничным с выделенными. Сейчас vertexSumNormals заполняется для всех.
				foreach( var face in meshData.Faces )
					foreach( var t in face.Triangles )
						vertexAvgNormals[ t.Vertex ] += t.RawVertex.Normal;

				for( int i = 0; i < vertexAvgNormals.Length; i++ )
					vertexAvgNormals[ i ] = vertexAvgNormals[ i ].GetNormalize();

				foreach( var fi in selFaces )
					foreach( var t in meshData.Faces[ fi ].Triangles )
						t.RawVertex.Normal = vertexAvgNormals[ t.Vertex ];

				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		/////////////////////////////////////////

		#region MergeFaces

		public static void MergeFacesGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.Selection.FaceCount > 1 )
				context.Enabled = true;
		}

		//??? Сейчас объединяет даже треугольники из разных Geometry. Решить потом, как правильно.

		//Combines selected faces into a single one. If the single Mesh In Space is selected then merges all the geometries into a single geometry.
		//If several meshes are selected then replaces them by a single mesh and places all geometries into it.
		public static void MergeFaces( BuilderActionContext actionContext )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var selFaces = actionContext.Selection.Faces;
				if( selFaces.Length < 2 )
					return;
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );

				int fi = MergeFaces( meshData, selFaces );
				actionContext.Selection.Faces = new[] { fi };

				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		static int MergeFaces( BuilderMeshData meshData, int[] facesToMerge )
		{
			int commonFaceIndex = facesToMerge[ 0 ];
			var commonFace = meshData.Faces[ commonFaceIndex ];
			for( int i = 1; i < facesToMerge.Length; i++ )
			{
				commonFace.Triangles.AddRange( meshData.Faces[ facesToMerge[ i ] ].Triangles );
				//ToDo : commonFace.Properties = ...;
				//ToDo : commonFace.SmoothingGroup = ...;
				meshData.Faces[ facesToMerge[ i ] ] = null;
			}

			//remove the inner edges
			var edges = GetEdgesCountDictionary( commonFace.Triangles );

			for( int i = 0; i < meshData.Edges.Count; i++ )
			{
				var e = meshData.Edges[ i ];
				if( edges.TryGetValue( e.Vertex1 < e.Vertex2 ? (e.Vertex1, e.Vertex2) : (e.Vertex2, e.Vertex1), out var foundEdgeCount ) && 1 < foundEdgeCount )
					meshData.Edges[ i ] = null;
			}

			return commonFaceIndex;
		}

		internal/*obfuscator*/ static Dictionary<(int lowVertex, int highVertex), int> GetEdgesCountDictionary( List<BuilderMeshData.FaceVertex> fVertices )
		{
			var edges = new Dictionary<(int lowVertex, int highVertex), int>();
			for( int i = 0; i < fVertices.Count; i += 3 )
			{
				AddEdge( edges, fVertices[ i ].Vertex, fVertices[ i + 1 ].Vertex );
				AddEdge( edges, fVertices[ i + 1 ].Vertex, fVertices[ i + 2 ].Vertex );
				AddEdge( edges, fVertices[ i + 2 ].Vertex, fVertices[ i ].Vertex );
			}
			return edges;
		}

		internal/*obfuscator*/ static void AddEdge( Dictionary<(int lowVertex, int highVertex), int> edgesOrderedCount, int vertex1, int vertex2 )
		{
			var e = BuilderCommonFunctions.OrderVertices( vertex1, vertex2 );
			if( edgesOrderedCount.TryGetValue( e, out var count ) )
				count++;
			else
				count = 1;
			edgesOrderedCount[ e ] = count;
		}

		#endregion

		/////////////////////////////////////////

		public static void TriangulateFacesGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.Selection.FaceCount != 0 )
				context.Enabled = true;
		}

		//Splits selected faces to the separate triangles. Each triangle in a face will become a face.
		public static void TriangulateFaces( BuilderActionContext actionContext )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var selFaces = actionContext.Selection.Faces;
				if( selFaces.Length == 0 )
					return;
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );
				actionContext.Selection.Faces = TriangulateFaces( meshData, selFaces ).ToArray();
				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		static List<int> TriangulateFaces( BuilderMeshData meshData, int[] faces )
		{
			var newFaces = new List<int>();
			foreach( var fi in faces )
			{
				var face = meshData.Faces[ fi ];
				meshData.Faces[ fi ] = null;

				for( int i = 0; i < face.Triangles.Count; i += 3 )
				{
					var triangles = new List<BuilderMeshData.FaceVertex>( 3 );
					triangles.Add( face.Triangles[ i ] );
					triangles.Add( face.Triangles[ i + 1 ] );
					triangles.Add( face.Triangles[ i + 2 ] );

					meshData.Edges.Add( new BuilderMeshData.Edge( triangles[ 0 ].Vertex, triangles[ 1 ].Vertex ) );
					meshData.Edges.Add( new BuilderMeshData.Edge( triangles[ 1 ].Vertex, triangles[ 2 ].Vertex ) );
					meshData.Edges.Add( new BuilderMeshData.Edge( triangles[ 2 ].Vertex, triangles[ 0 ].Vertex ) );

					meshData.Faces.Add( new BuilderMeshData.Face(
						triangles,
						null, //ToDo : Properties
						face.SmoothingGroup //ToDo ??? 
						) );
					newFaces.Add( meshData.Faces.Count - 1 );
				}
			}

			return newFaces;
		}

		/////////////////////////////////////////

		#region DetachFaces

		public static void DetachFacesGetState( EditorActionGetStateContext context, BuilderActionContext actionContext, bool toMeshInSpace )
		{
			if( actionContext.Selection.FaceCount != 0 )
				context.Enabled = true;
		}

		static (MeshInSpace newMeshInSpace, int[] newFaces) DetachFaces( bool toMeshInSpace, bool splitVertices, BuilderWorkareaMode workareaMode, Mesh mesh, int[] faces, BuilderSelection selection, List<UndoSystem.Action> undo, ref bool undoLocked )
		{
			var meshData = new BuilderMeshData();
			var extractedData = mesh.ExtractData();
			meshData.Load( extractedData );

			(MeshInSpace newMeshInSpace, int[] newFaces) ret = (null, null);

			Mesh newMesh = null;
			MeshGeometry newGeometry;
			BuilderMeshData newMeshData = null;
			int newGeometryIndex;
			if( toMeshInSpace )
			{
				var parent = mesh.Parent?.Parent;
				if( parent == null )
				{
					return ret; //ToDo :??? Log Error?
				}

				var newMeshInSpace = parent.CreateComponent<MeshInSpace>();
				newMeshInSpace.Name = BuilderCommonFunctions.GetUniqueFriendlyName( newMeshInSpace );
				newMeshInSpace.Transform = ( (MeshInSpace)mesh.Parent ).Transform;

				newMesh = newMeshInSpace.CreateComponent<Mesh>();
				newMesh.Name = BuilderCommonFunctions.GetUniqueFriendlyName( newMesh );
				newMeshInSpace.Mesh = ReferenceUtility.MakeReference<Mesh>( null, ReferenceUtility.CalculateRootReference( newMesh ) );

				newGeometry = newMesh.CreateComponent<MeshGeometry>();
				newGeometry.Name = BuilderCommonFunctions.GetUniqueFriendlyName( newGeometry );

				if( !undoLocked )
					undo.Add( new UndoActionComponentCreateDelete( workareaMode.DocumentWindow.Document, new[] { newMeshInSpace }, true ) );
				newMeshData = new BuilderMeshData();

				ret.newMeshInSpace = newMeshInSpace;
				ret.newFaces = Enumerable.Range( 0, faces.Length ).ToArray();
				newGeometryIndex = 0;
			}
			else
			{
				newGeometry = mesh.CreateComponent<MeshGeometry>();
				newGeometry.Name = BuilderCommonFunctions.GetUniqueFriendlyName( newGeometry );
				if( !undoLocked )
					undo.Add( new UndoActionComponentCreateDelete( workareaMode.DocumentWindow.Document, new[] { newGeometry }, true ) );
				newGeometryIndex = extractedData.MeshGeometries.Length;
			}

			if(/* toMeshInSpace ||*/ splitVertices )
				DetachFaces( meshData, faces );

			//!!!!может не нужно, может достаточно новые faces выделить
			//add offset
			if( !toMeshInSpace && splitVertices )
			{
				//!!!!
				var offset = new Vector3F( 0, 0, 0.2f );

				foreach( var fi in faces )
					foreach( var t in meshData.Faces[ fi ].Triangles )
						t.RawVertex.Position += offset;
			}

			var geometriesInSelection = GetGeometriesOfFaces( meshData, faces );

			var newGeometryItem = extractedData.MeshGeometries[ geometriesInSelection[ 0 ] ];
			var newVertexStructure = (VertexElement[])newGeometryItem.VertexStructure.Clone();

			newGeometry.VertexStructure = newVertexStructure;
			newGeometry.Material = newGeometryItem.Material;

			if( !BuilderCommonFunctions.IsSameVertexStructure( extractedData, geometriesInSelection ) )
			{
				var newFormat = new BuilderMeshData.MeshGeometryFormat( newVertexStructure );
				foreach( var fi in faces )
					foreach( var t in meshData.Faces[ fi ].Triangles )
						t.RawVertex = BuilderMeshData.ConvertRawVertex( t.RawVertex, newFormat );
			}

			foreach( var fi in faces )
				foreach( var t in meshData.Faces[ fi ].Triangles )
					t.RawGeometry = newGeometryIndex;

			if( toMeshInSpace )
			{
				selection.Faces = null; //clear the selection in original MeshInSpace
				newMeshData.Faces = new List<BuilderMeshData.Face>();
				foreach( var fi in faces )
				{
					newMeshData.Faces.Add( meshData.Faces[ fi ] );
					meshData.Faces[ fi ] = null;
				}
				newMeshData.Vertices = new List<BuilderMeshData.Vertex>( meshData.Vertices );
				newMeshData.Edges = new List<BuilderMeshData.Edge>( meshData.Edges ); //?? Видимо, клонировать каждый Edge не надо, т.к. после разделения нет общих вертексов, значит и общих Edge
																					  //newMesh.VisibilityDistance = mesh.VisibilityDistance; // Remove?
				var newSelection = new BuilderSelection() { SelectionMode = BuilderSelectionMode.Face, Faces = ret.newFaces };
				newMeshData.Save( newMesh, newSelection, undo, ref undoLocked );
				ret.newFaces = newSelection.Faces;
			}

			if( toMeshInSpace )
				meshData.Save( mesh, selection, undo, ref undoLocked );
			else
				meshData.Save( mesh, selection, undo, ref undoLocked );

			//!!!!impl
			if( !toMeshInSpace && splitVertices )
				ret.newFaces = new int[ 0 ];

			return ret;
		}

		//Removes selected faces from a geometry and places them in a new geometry. There are options:
		//  - Detach Faces to Mesh Geometry - The new geometry may share the same vertices with an old geometry.
		//	- Detach Faces to Mesh Geometry (Split Vertices) - The new geometry will not share the same vertices.
		//	- Detach Faces to Mesh In Space - The new geometry will be placed in a new Mesh In Space.
		public static void DetachFaces( BuilderActionContext actionContext, bool toMeshInSpace, bool splitVertices )
		{
			(MeshInSpace newMeshInSpace, int[] faces) newSelection = (null, null);

			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var selFaces = actionContext.Selection.Faces;
				if( selFaces.Length == 0 )
					return;

				newSelection = DetachFaces( toMeshInSpace, splitVertices, actionContext.BuilderWorkareaMode, mesh, selFaces, actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );

			if( newSelection.newMeshInSpace != null )
			{
				//??? Так правильно выделять faces в другом MeshInSpace?
				actionContext.SelectMeshesInSpace( newSelection.newMeshInSpace );
				actionContext.BuilderWorkareaMode.SelectFaces( newSelection.faces );
			}
			if( !toMeshInSpace && splitVertices )
				actionContext.BuilderWorkareaMode.SelectFaces( newSelection.faces );
		}

		static List<int> GetGeometriesOfFaces( BuilderMeshData meshData, int[] faces )
		{
			var geometries = new List<int>();
			foreach( var fi in faces )
				foreach( var t in meshData.Faces[ fi ].Triangles )
				{
					int geomIndex = t.RawGeometry;
					if( !geometries.Contains( geomIndex ) )
						geometries.Add( geomIndex );
				}
			return geometries;
		}

		static void DetachFaces( BuilderMeshData meshData, int[] faces )
		{
			var verticesInSelection = new Dictionary<int, int>();
			foreach( var fi in faces )
			{
				var face = meshData.Faces[ fi ];

				for( int i = 0; i < face.Triangles.Count; i++ )
				{
					int v = face.Triangles[ i ].Vertex;
					verticesInSelection[ v ] = v;
				}
			}

			var sharedVertices = new List<int>();
			for( int i = 0; i < meshData.Faces.Count; i++ )
			{
				if( faces.Contains( i ) )
					continue;
				var face = meshData.Faces[ i ];

				for( int j = 0; j < face.Triangles.Count; j++ )
				{
					if( verticesInSelection.ContainsKey( face.Triangles[ j ].Vertex ) )
						sharedVertices.Add( face.Triangles[ j ].Vertex );
				}
			}

			//sharedVertices must be split
			foreach( var sv in sharedVertices )
			{
				meshData.Vertices.Add( new BuilderMeshData.Vertex() );
				verticesInSelection[ sv ] = meshData.Vertices.Count - 1;
			}
			foreach( var fi in faces )
			{
				var face = meshData.Faces[ fi ];

				for( int i = 0; i < face.Triangles.Count; i += 3 )
				{
					var v0 = face.Triangles[ i ];
					var v1 = face.Triangles[ i + 1 ];
					var v2 = face.Triangles[ i + 2 ];

					var nw0 = verticesInSelection[ v0.Vertex ];
					var nw1 = verticesInSelection[ v1.Vertex ];
					var nw2 = verticesInSelection[ v2.Vertex ];

					if( v0.Vertex != nw0 || v1.Vertex != nw1 )
						meshData.AddEdge( nw0, nw1 );
					if( v1.Vertex != nw1 || v2.Vertex != nw2 )
						meshData.AddEdge( nw1, nw2 );
					if( v2.Vertex != nw2 || v0.Vertex != nw0 )
						meshData.AddEdge( nw2, nw0 );

					v0.Vertex = nw0; //set links on the new vertices
					v1.Vertex = nw1;
					v2.Vertex = nw2;
				}
			}
		}

		#endregion

		/////////////////////////////////////////

		public static void CloneFacesGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.Selection.FaceCount != 0 )
				context.Enabled = true;
		}

		public static void CloneFaces( BuilderActionContext actionContext )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );

				var selFaces = actionContext.Selection.Faces;
				if( selFaces.Length == 0 )
					return;
				string s = selFaces.Length > 1 ? "s" : "";
				if( EditorMessageBox.ShowQuestion( $"Clone selected face{s}?", EMessageBoxButtons.YesNo ) == EDialogResult.No )
					return;
				actionContext.Selection.Faces = CloneFacesExecute( meshData, selFaces );

				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
			}
			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		static int[] CloneFacesExecute( BuilderMeshData meshData, int[] facesToClone )
		{
			int[] newFacesIndices = Enumerable.Range( meshData.Faces.Count, facesToClone.Length ).ToArray();
			foreach( var fi in facesToClone )
			{
				var oldFace = meshData.Faces[ fi ];
				var newTriangles = new List<BuilderMeshData.FaceVertex>( oldFace.Triangles.Count );
				foreach( var t in oldFace.Triangles )
					newTriangles.Add( meshData.CloneFaceVertex( t ) );

				var newFace = new BuilderMeshData.Face( newTriangles, BuilderMeshData.CloneProperties( oldFace.Properties ), oldFace.SmoothingGroup ); //ToDo ??? Properties, SmoothingGroup
				meshData.Faces.Add( newFace );
			}

			DetachFaces( meshData, newFacesIndices );

			return newFacesIndices;
		}

		/////////////////////////////////////////

		public static void SetMaterialGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object && actionContext.GetSelectedMeshInSpaces().Length != 0 ||
				actionContext.GetSelectedMesh().mesh != null && actionContext.Selection.FaceCount != 0 )
			{
				var referenceToMaterial = GetSelectedMaterialToSet();
				if( !string.IsNullOrEmpty( referenceToMaterial.GetByReference ) )
					context.Enabled = true;
			}
		}

		//Sets a material for the geometries of the selected faces. The material to use has to be selected in the Resources Window.
		public static void SetMaterial( BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object && actionContext.GetSelectedMeshInSpaces().Length != 0 )
			{
				//Object mode

				var referenceToMaterial = GetSelectedMaterialToSet();
				//if( !referenceToMaterial.ReferenceSpecified )
				//{
				//	//use SetReferenceWindow

				//	var meshGeometriesToUpdate = new List<MeshGeometry>();
				//	var meshInSpacesToUpdate = new List<MeshInSpace>();

				//	foreach( var meshInSpace in actionContext.GetSelectedMeshInSpaces() )
				//	{
				//		var mesh = meshInSpace.Mesh.Value;
				//		if( mesh != null )
				//		{
				//			if( mesh.Parent == meshInSpace )
				//			{
				//				if( meshInSpace.ReplaceMaterial.ReferenceSpecified )
				//					meshInSpacesToUpdate.Add( meshInSpace );
				//				else
				//				{
				//					foreach( var geom in mesh.GetComponents<MeshGeometry>() )
				//						meshGeometriesToUpdate.Add( geom );
				//				}
				//			}
				//			else
				//				meshInSpacesToUpdate.Add( meshInSpace );
				//		}
				//	}

				//	if( meshInSpacesToUpdate.Count != 0 )
				//	{
				//		var type = MetadataManager.GetTypeOfNetType( typeof( MeshInSpace ) );
				//		var property = type.MetadataGetMemberBySignature( "property:ReplaceMaterial" ) as Metadata.Property;
				//		if( property != null )
				//			EditorAPI.OpenSetReferenceWindow( actionContext.DocumentWindow, meshInSpacesToUpdate.ToArray(), meshInSpacesToUpdate.ToArray(), property, null );
				//	}
				//	else if( meshGeometriesToUpdate.Count != 0 )
				//	{
				//		var type = MetadataManager.GetTypeOfNetType( typeof( MeshGeometry ) );
				//		var property = type.MetadataGetMemberBySignature( "property:Material" ) as Metadata.Property;
				//		if( property != null )
				//			EditorAPI.OpenSetReferenceWindow( actionContext.DocumentWindow, meshGeometriesToUpdate.ToArray(), meshGeometriesToUpdate.ToArray(), property, null );
				//	}
				//}
				//else
				{
					//set material which is selected in Resources Window

					var undo = new List<UndoSystem.Action>();
					var undoLocked = false;

					foreach( var meshInSpace in actionContext.GetSelectedMeshInSpaces() )
					{
						var mesh = meshInSpace.Mesh.Value;
						if( mesh != null )
						{
							if( mesh.Parent == meshInSpace )
							{
								SetReplaceMaterial( meshInSpace, new Reference<Material>(), undo, ref undoLocked );
								foreach( var geom in mesh.GetComponents<MeshGeometry>() )
									SetMaterialForGeometry( geom, referenceToMaterial, undo, ref undoLocked );
							}
							else
								SetReplaceMaterial( meshInSpace, referenceToMaterial, undo, ref undoLocked );
						}
					}

					//undo
					var undoMultiAction = new UndoMultiAction();
					undoMultiAction.AddActions( undo );
					actionContext.DocumentWindow?.Document2?.CommitUndoAction( undoMultiAction );
				}
			}
			else if( actionContext.GetSelectedMesh().mesh != null && actionContext.Selection.FaceCount != 0 )
			{
				//Face mode

				var referenceToMaterial = GetSelectedMaterialToSet();

				void ExecuteFacesMode( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
				{
					var selFaces = actionContext.Selection.Faces;
					if( selFaces.Length == 0 )
						return;

					//!!!!как тут с undo будет при Detach и других действиях одновременно

					//DetachFaces( false, false, actionContext.BuilderWorkareaMode, mesh, selFaces, actionContext.Selection, undo, ref undoLocked );

					var meshData = new BuilderMeshData();
					var extractedData = mesh.ExtractData();
					meshData.Load( extractedData );

					//!!!!мержить с геометрией с новым материалом? если несколько таких?

					var meshGeometries = mesh.GetComponents<MeshGeometry>();

					//obfuscator
					var selectedGeometries = new List<MeshGeometry>();
					{
						foreach( var g in GetGeometriesOfFaces( meshData, actionContext.Selection.Faces ) )
							selectedGeometries.Add( meshGeometries[ g ] );
					}
					//var selectedGeometries = GetGeometriesOfFaces( meshData, actionContext.Selection.Faces ).Select( _ => meshGeometries[ _ ] );


					//if( !referenceToMaterial.ReferenceSpecified )
					//{
					//	//use SetReferenceWindow

					//	var type = MetadataManager.GetTypeOfNetType( typeof( MeshGeometry ) );
					//	var property = type.MetadataGetMemberBySignature( "property:Material" ) as Metadata.Property;
					//	if( property != null )
					//	{
					//		EditorAPI.OpenSetReferenceWindow( actionContext.DocumentWindow, selectedGeometries.ToArray(), selectedGeometries.ToArray(), property, null );
					//	}
					//}
					//else
					{
						//set material which is selected in Resources Window

						if( mesh.Parent is MeshInSpace meshInSpace )
							SetReplaceMaterial( meshInSpace, new Reference<Material>(), undo, ref undoLocked );
						foreach( var geometry in selectedGeometries )
							SetMaterialForGeometry( geometry, referenceToMaterial, undo, ref undoLocked );
					}

					//!!!!полезен инструмент с созданием/удалением геометрий

					//!!!!имена дурацкие стают
					//!!!!слетает выделение
					//!!!!пустые геометрии

					//!!!!undo
					//mesh.MergeGeometriesWithEqualVertexStructureAndMaterial();

					//!!!!!сбилдить структуру?
					//mesh.BuildStructure();

					////copy back to the mesh
					//meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );

				}

				ExecuteOneMeshAction( actionContext, CanExecute, ExecuteFacesMode );
			}


			//if( actionContext.SelectionMode == SelectionMode.Object && 0 < actionContext.GetSelectedMeshInSpaceArray().Length )
			//{
			//	var undo = new UndoMultiAction();

			//	foreach( var meshInSpace in actionContext.GetSelectedMeshInSpaceArray() )
			//	{
			//		var mesh = meshInSpace.Mesh.Value;
			//		if( mesh != null )
			//		{
			//			if( mesh.Parent == meshInSpace )
			//			{
			//				SetReplaceMaterial( meshInSpace, new Reference<Material>(), undo );
			//				foreach( var geom in mesh.GetComponents<MeshGeometry>() )
			//					SetMaterialForGeometry( geom, referenceToMaterial, undo );
			//			}
			//			else
			//				SetReplaceMaterial( meshInSpace, referenceToMaterial, undo );
			//		}
			//	}

			//	actionContext.DocumentWindow?.Document?.CommitUndoAction( undo );
			//}
			//else if( actionContext.GetSelectedMesh().mesh != null && actionContext.GetSelectedFaces().Length > 0 )
			//{
			//	void ExecuteFacesMode( Mesh mesh, UndoMultiAction undoForGeomAndStructure, UndoMultiAction undoForExternalComponent )
			//	{
			//		var meshData = new MeshData();
			//		var extractedStructure = mesh.ExtractStructure();
			//		meshData.Load( extractedStructure );

			//		//IEnumerable<MeshGeometry> selectedGeometries = null;

			//		//if( actionContext.SelectionMode == SelectionMode.Object )
			//		//	selectedGeometries = mesh.GetComponents<MeshGeometry>();
			//		//else
			//		//{
			//		var selectedFaces = actionContext.GetSelectedFaces();
			//		//if( selectedFaces.Length == 0 )
			//		//	return;
			//		var meshGeometries = mesh.GetComponents<MeshGeometry>();
			//		var selectedGeometries = GetGeometriesOfFaces( meshData, selectedFaces ).Select( _ => meshGeometries[ _ ] );
			//		//}

			//		// undoForExternalComponent is used because the undo is needed even for "procedural", because it changes the parent 
			//		if( mesh.Parent is MeshInSpace meshInSpace )
			//			SetReplaceMaterial( meshInSpace, new Reference<Material>(), undoForExternalComponent );
			//		foreach( var geom in selectedGeometries )
			//			SetMaterialForGeometry( geom, referenceToMaterial, undoForGeomAndStructure );
			//	}

			//	ExecuteOneMeshAction( actionContext, CanExecute, ExecuteFacesMode );
			//}
		}

		public static void SetMaterialForGeometry( MeshGeometry geom, Reference<Material> material, List<UndoSystem.Action> undo, ref bool undoLocked )
		{
			if( !undoLocked )
			{
				var property = (Metadata.Property)geom.MetadataGetMemberBySignature( "property:" + nameof( MeshGeometry.Material ) );
				undo.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( geom, property, geom.Material ) ) );
			}
			geom.Material = material;
		}

		static void SetReplaceMaterial( MeshInSpace meshInSpace, Reference<Material> value, List<UndoSystem.Action> undo, ref bool undoLocked )
		{
			if( !meshInSpace.ReplaceMaterial.Equals( value ) )
			{
				if( !undoLocked )
				{
					var property = (Metadata.Property)meshInSpace.MetadataGetMemberBySignature( "property:" + nameof( MeshInSpace.ReplaceMaterial ) );
					undo.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshInSpace, property, meshInSpace.ReplaceMaterial ) ) );
				}

				meshInSpace.ReplaceMaterial = value;
			}

			//var refList = meshInSpace.ReplaceMaterialSelectively;  //ToDo : ReplaceMaterialSelectively. //в refList как соответствия с geometry? По одинаковым индексам?
		}

		static ReferenceNoValue GetSelectedMaterialToSet()
		{
			(var objectType, var referenceToObject, var anyData, var objectName) = EditorAPI2.GetSelectedObjectToCreate();
			if( objectType != null )
			{
				var componentType = objectType as Metadata.ComponentTypeInfo;
				if( componentType != null && componentType.BasedOnObject != null )
				{
					//!!!!пока только ресурсные ссылки поддерживаются

					//Material
					var material = componentType.BasedOnObject as Material;
					if( material != null )
						return ReferenceUtility.MakeResourceReference( material );

					//Import3D
					if( componentType.BasedOnObject is Import3D )
					{
						material = componentType.BasedOnObject.GetComponent( "Material" ) as Material;
						if( material != null )
							return ReferenceUtility.MakeResourceReference( material );
					}
				}
			}

			return new ReferenceNoValue();
		}

		/////////////////////////////////////////

		public static void GrowSelectionGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.Selection.FaceCount != 0 || actionContext.Selection.EdgeCount != 0 || actionContext.Selection.VertexCount != 0 )
				context.Enabled = true;
		}

		//Adds adjacent elements to the currently selected elements.
		public static void GrowSelection( BuilderActionContext actionContext )
		{
			//int[] newSelection = null;

			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var meshData = new BuilderMeshData();
				var extractedData = mesh.ExtractData();
				meshData.Load( extractedData );

				switch( actionContext.SelectionMode )
				{
				case BuilderSelectionMode.Face:
					{
						//Selects faces that have a common edge with the faces in selectedFaces
						var selectedFaces = actionContext.Selection.Faces;
						var allEdges = new HashSet<(int lowVertex, int highVertex)>();
						foreach( var e in meshData.Edges )
							allEdges.Add( BuilderCommonFunctions.OrderVertices( e.Vertex1, e.Vertex2 ) );

						var selectedEdges = new HashSet<(int lowVertex, int highVertex)>();
						for( int i = 0; i < selectedFaces.Length; i++ )
							selectedEdges.UnionWith( IntersectEdgesByFace( allEdges, meshData.Faces[ selectedFaces[ i ] ].Triangles ) );

						var newSelectedFaces = new List<int>();
						for( int i = 0; i < meshData.Faces.Count; i++ )
							if( IntersectEdgesByFace( selectedEdges, meshData.Faces[ i ].Triangles ).Count != 0 )
								newSelectedFaces.Add( i );
						actionContext.Selection.Faces = newSelectedFaces.ToArray();
					}
					break;

				case BuilderSelectionMode.Edge:
					{
						var selectedEdges = actionContext.Selection.Edges;
						if( selectedEdges.Length == 0 )
							return;

						var selectedVertices = new HashSet<int>();
						foreach( var ei in selectedEdges )
						{
							var e = meshData.Edges[ ei ];
							selectedVertices.Add( e.Vertex1 );
							selectedVertices.Add( e.Vertex2 );
						}

						var newSelectedEdges = new List<int>();
						for( int i = 0; i < meshData.Edges.Count; i++ )
						{
							var e = meshData.Edges[ i ];
							if( selectedVertices.Contains( e.Vertex1 ) || selectedVertices.Contains( e.Vertex2 ) )
								newSelectedEdges.Add( i );
						}
						actionContext.Selection.Edges = newSelectedEdges.ToArray();
					}
					break;

				case BuilderSelectionMode.Vertex:
					{
						var selectedVertices = new HashSet<int>( actionContext.Selection.Vertices );
						var newSelVertices = new HashSet<int>();
						foreach( var face in meshData.Faces )
						{
							for( int i = 0; i < face.Triangles.Count; i += 3 )
							{
								if( selectedVertices.Contains( face.Triangles[ i ].Vertex ) ||
									selectedVertices.Contains( face.Triangles[ i + 1 ].Vertex ) ||
									selectedVertices.Contains( face.Triangles[ i + 2 ].Vertex ) )
								{
									newSelVertices.Add( face.Triangles[ i ].Vertex );
									newSelVertices.Add( face.Triangles[ i + 1 ].Vertex );
									newSelVertices.Add( face.Triangles[ i + 2 ].Vertex );
								}
							}
						}

						actionContext.Selection.Vertices = newSelVertices.ToArray();
					}
					break;
				}
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute, false );
		}

		internal/*obfuscator*/ static HashSet<(int lowVertex, int highVertex)> IntersectEdgesByFace( HashSet<(int lowVertex, int highVertex)> edges, List<BuilderMeshData.FaceVertex> triangles )
		{
			var ret = new HashSet<(int lowVertex, int highVertex)>();
			for( int i = 0; i < triangles.Count; i += 3 )
			{
				var e0 = BuilderCommonFunctions.OrderVertices( triangles[ i ].Vertex, triangles[ i + 1 ].Vertex );
				var e1 = BuilderCommonFunctions.OrderVertices( triangles[ i + 1 ].Vertex, triangles[ i + 2 ].Vertex );
				var e2 = BuilderCommonFunctions.OrderVertices( triangles[ i + 2 ].Vertex, triangles[ i ].Vertex );
				if( edges.Contains( e0 ) )
					ret.Add( e0 );
				if( edges.Contains( e1 ) )
					ret.Add( e1 );
				if( edges.Contains( e2 ) )
					ret.Add( e2 );
			}

			return ret;
		}

		/////////////////////////////////////////

		public static void SelectByMaterialGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.Selection.FaceCount != 0 )
				context.Enabled = true;
		}

		//Selects all the faces that have the same material as currently selected faces.
		public static void SelectByMaterial( BuilderActionContext actionContext )
		{
			//Reference.Value == null materials are considered equal
			bool Contains( List<Reference<Material>> materialList, Reference<Material> material )
			{
				foreach( var m in materialList )
					if( m.Value == null && material.Value == null || material.Equals( m ) )
						return true;
				return false;
			}

			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				var geometries = mesh.GetComponents<MeshGeometry>();

				var meshData = new BuilderMeshData();
				var extractedStructure = mesh.ExtractData();
				meshData.Load( extractedStructure );

				var selectedFaces = actionContext.Selection.Faces;
				if( selectedFaces.Length == 0 )
					return;

				var selectedGeometries = new HashSet<int>(); //??? new List
				foreach( var fi in selectedFaces )
				{
					var triangles = meshData.Faces[ fi ].Triangles;
					foreach( var t in triangles )
						selectedGeometries.Add( t.RawGeometry );
				}

				var selectedMaterials = new List<Reference<Material>>();
				foreach( var gi in selectedGeometries )
				{
					var m = geometries[ gi ].Material;
					if( !Contains( selectedMaterials, m ) )
						selectedMaterials.Add( m );
				}

				var newSelectedGeometries = new HashSet<int>();
				for( int i = 0; i < geometries.Length; i++ )
				{
					if( Contains( selectedMaterials, geometries[ i ].Material ) )
						newSelectedGeometries.Add( i );
				}

				var newSelectedFaces = new List<int>();
				for( int i = 0; i < meshData.Faces.Count; i++ )
				{
					var triangles = meshData.Faces[ i ].Triangles;
					foreach( var t in triangles )
					{
						if( newSelectedGeometries.Contains( t.RawGeometry ) )
						{
							newSelectedFaces.Add( i );
							break;
						}
					}
				}

				actionContext.Selection.Faces = newSelectedFaces.ToArray();
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute, false );
		}

		/////////////////////////////////////////

		public static void SetVertexColor( BuilderActionContext actionContext, ColorValue color )
		{
			void Execute( Mesh mesh, List<UndoSystem.Action> undo, ref bool undoLocked )
			{
				//load mesh data
				var meshData = new BuilderMeshData();
				var extractedStructure = mesh.ExtractData();
				meshData.Load( extractedStructure );

				var color4F = color.ToVector4F();
				if( actionContext.Selection.SelectionMode == BuilderSelectionMode.Face )
				{
					foreach( var faceIndex in actionContext.Selection.Faces )
						foreach( var t in meshData.Faces[ faceIndex ].Triangles )
							t.RawVertex.Color = color4F;
				}
				else
				{
					//in Vertex, Edge modes the colors for all raw vertices bound to a selected vertex are changed
					var vertices = GetSelectedVerticesForAllModes( actionContext, meshData );
					foreach( var face in meshData.Faces )
						foreach( var t in face.Triangles )
							if( vertices.Contains( t.Vertex ) )
								t.RawVertex.Color = color4F;
				}

				//copy back to the mesh
				meshData.Save( mesh, actionContext.Selection, undo, ref undoLocked );
			}

			ExecuteOneMeshAction( actionContext, CanExecute, Execute );
		}

		public static ColorValue? GetInitialColor( BuilderActionContext actionContext )
		{
			var (_, mesh) = actionContext.GetSelectedMesh();
			if( mesh == null )
				return null;

			var meshData = new BuilderMeshData();
			var extractedStructure = mesh.ExtractData();
			meshData.Load( extractedStructure );

			if( actionContext.SelectionMode == BuilderSelectionMode.Face )
			{
				ColorValue? color = null;
				foreach( var faceIndex in actionContext.Selection.Faces )
				{
					foreach( var t in meshData.Faces[ faceIndex ].Triangles )
					{
						if( color == null )
							color = new ColorValue( t.RawVertex.Color );
						else if( color != new ColorValue( t.RawVertex.Color ) )
							return null;
					}
				}
				return color;
			}
			else
			{
				ColorValue? color = null;

				var vertices = GetSelectedVerticesForAllModes( actionContext, meshData );
				if( vertices.Count != 0 )
				{
					foreach( var face in meshData.Faces )
					{
						foreach( var t in face.Triangles )
						{
							if( vertices.Contains( t.Vertex ) )
							{
								if( color == null )
									color = new ColorValue( t.RawVertex.Color );
								else if( color != new ColorValue( t.RawVertex.Color ) )
									return null;
							}
						}
					}
				}

				return color;
			}
		}

		static ESet<int> GetSelectedVerticesForAllModes( BuilderActionContext actionContext, BuilderMeshData mesh )
		{
			var vertices = new ESet<int>();
			switch( actionContext.SelectionMode )
			{
			case BuilderSelectionMode.Vertex:
				foreach( var v in actionContext.Selection.Vertices )
					vertices.AddWithCheckAlreadyContained( v );
				break;

			case BuilderSelectionMode.Edge:
				foreach( var e in actionContext.Selection.Edges )
				{
					vertices.AddWithCheckAlreadyContained( mesh.Edges[ e ].Vertex1 );
					vertices.AddWithCheckAlreadyContained( mesh.Edges[ e ].Vertex2 );
				}
				break;

			case BuilderSelectionMode.Face:
				foreach( var f in actionContext.Selection.Faces )
					foreach( var t in mesh.Faces[ f ].Triangles )
						vertices.AddWithCheckAlreadyContained( t.Vertex );
				break;
			}
			return vertices;
		}

	}
}
#endif