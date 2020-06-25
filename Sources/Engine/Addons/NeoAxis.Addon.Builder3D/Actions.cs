// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using NeoAxis.Editor;

namespace NeoAxis.Addon.Builder3D
{
	public static partial class Actions
	{
		//public delegate string ActionCanExecuteDelegate( );

		public delegate void ActionExecuteDelegate( Component_MeshInSpace[] selectedMeshInSpaces, UndoMultiAction undoForMeshes, UndoMultiAction undoForOtherActions );

		//??? Enabled = false для Parent у MeshInSpace? Тогда Select вынести из Execute.
		public static void ExecuteAction( ActionContext actionContext, ActionExecuteDelegate execute )//, ActionCanExecuteDelegate canExecute )
		{
			var ms = actionContext.GetSelectedMeshInSpaceArray();
			if( ms.Length == 0 )
				return;

			var undo = new UndoMultiAction();

			bool needUndo = false;
			bool changed = false;
			foreach( var m in ms )
			{
				var mesh = m.Mesh.Value;
				if( mesh != null )
				{
					//??? Если несколько Mesh переобразовалось и несколоько не изменились, сейчас обрабатывается так - undo не нужен только если все преобразовались.
					CommonFunctions.CopyExternalMesh( actionContext.DocumentWindow.Document, m, ref mesh, undo, out var needUndoForNextActions ); //make copy of the mesh if needed
					if( needUndoForNextActions )
						needUndo = true;
					else
						changed = true;
				}
			}
			foreach( var m in ms )
				if( !CheckValidMeshWithMessageBox( m.Mesh ) )
				{
					if( changed )
						actionContext.DocumentWindow?.Document?.CommitUndoAction( undo );
					return;
				}

			execute( ms, needUndo ? undo : null, undo );

			undo.Actions.Reverse();
			actionContext.DocumentWindow?.Document?.CommitUndoAction( undo );
		}

		static bool CheckValidMeshWithMessageBox( Component_Mesh mesh )
		{
			if( !CommonFunctions.CheckValidMesh( mesh, out string error ) )
			{
				EditorMessageBox.ShowWarning( error );
				return false;
			}
			return true;
		}

		public static void MergeObjectsGetState( EditorAction.GetStateContext context, ActionContext actionContext )
		{
			if( actionContext.SelectionMode != SelectionMode.Object )
				return;
			var ms = actionContext.GetSelectedMeshInSpaceArray();
			if( ms.Length == 0 )
				return;
			if( ms.Length == 1 )
			{
				if( 1 < ms[ 0 ].Mesh.Value?.GetComponents<Component_MeshGeometry>()?.Length )
					context.Enabled = true;
			}
			else
				context.Enabled = true;
		}

		//Combines the selected objects into a single one. If the single Mesh In Space object is selected than merges all child geometries into a single one.
		//If several mesh objects are selected than places all child geometries of these objects into the single mesh object.
		public static void MergeObjects( ActionContext actionContext )
		{
			void Execute( Component_MeshInSpace[] selectedMeshInSpaces, UndoMultiAction undoForMeshes, UndoMultiAction undoForOtherActions )
			{
				if( selectedMeshInSpaces.Length == 1 )
				{
					var mesh = selectedMeshInSpaces[ 0 ].Mesh.Value;
					if( mesh == null )
						return;

					MergeGeometries( mesh, actionContext.DocumentWindow?.Document, undoForMeshes );
				}
				else
				{
					var newMeshInSpace = MergeMeshInSpaces( selectedMeshInSpaces, actionContext.DocumentWindow?.Document, undoForOtherActions );
					actionContext.SelectMeshesInSpace( newMeshInSpace );
				}
			}

			ExecuteAction( actionContext, Execute );
		}

		//Component_MeshInSpace contains only one Component_Mesh.
		static void MergeGeometries( Component_Mesh mesh, DocumentInstance document, UndoMultiAction undo )
		{
			Component_MeshGeometry[] geometries = mesh.GetComponents<Component_MeshGeometry>();
			if( geometries == null || geometries.Length < 2 )
				return;

			Reference<Component_Material> material = geometries[ 0 ].Material;
			//for( int i = 1; i < geometries.Length; i++ )
			//{
			//	if( !( material.Value == null && geometries[ i ].Material.Value == null || material.Equals( geometries[ i ].Material ) ) )
			//	{
			//		//??? Если разные Material какой вариант лучше: 1) Брать из первого geometry с вопросом в MessageBox; 2) Disable в меню для Action. 3) Соединять те, которые с одинаковым материалом.
			//		if( EditorMessageBox.ShowQuestion( "Mesh geometries have different materials. Merge them using a material from the first geometry?", MessageBoxButtons.OKCancel ) == DialogResult.Cancel )
			//		{
			//			return;
			//		}
			//	}
			//}

			var extracted = mesh.ExtractStructure();

			var newIndices = new List<int>();
			var newVertices = new List<byte>();
			var newVertexStructure = extracted.MeshGeometries[ 0 ].VertexStructure;
			var newVertexFormat = new MeshData.MeshGeometryFormat( newVertexStructure );

			for( int geomIndex = 0; geomIndex < extracted.MeshGeometries.Length; geomIndex++ )
			{
				var g = extracted.MeshGeometries[ geomIndex ];


				if( g.Vertices == null || g.Indices == null )
					continue;
				int indexOffset = newVertices.Count / newVertexFormat.vertexSize;

				for( int i = 0; i < g.Indices.Length; i++ )
					newIndices.Add( g.Indices[ i ] + indexOffset );

				if( !CommonFunctions.IsSameVertexStructure( newVertexStructure, g.VertexStructure ) )
					g.Vertices = MeshData.ConvertToFormat( new MeshData.MeshGeometryFormat( g.VertexStructure ), g.Vertices, newVertexFormat );

				newVertices.AddRange( g.Vertices );

				foreach( var face in extracted.Structure.Faces )
				{
					for( int i = 0; i < face.Triangles.Length; i++ )
					{
						if( face.Triangles[ i ].RawGeometry == geomIndex )
						{
							face.Triangles[ i ].RawGeometry = 0;
							face.Triangles[ i ].RawVertex += indexOffset;
						}
					}
				}
			}

			// changes in the mesh

			if( undo != null )
			{
				//add structure update to undo
				var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Component_Mesh.Structure ) );
				undo.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure?.Clone() ) ) );
			}

			bool meshWasEnabled = mesh.Enabled;
			mesh.Enabled = false;
			try
			{
				var newGeometry = mesh.CreateComponent<Component_MeshGeometry>();
				newGeometry.Material = material;

				newGeometry.Vertices = newVertices.ToArray();
				newGeometry.Indices = newIndices.ToArray();
				newGeometry.VertexStructure = newVertexStructure;

				//add created geometry to undo
				undo?.AddAction( new UndoActionComponentCreateDelete( document, new Component[] { newGeometry }, create: true ) );

				mesh.Structure = extracted.Structure;

				//delete old mesh geometry
				undo?.AddAction( new UndoActionComponentCreateDelete( document, geometries, create: false ) );

				newGeometry.Name = CommonFunctions.GetUniqueFriendlyName( newGeometry );
			}
			finally
			{
				mesh.Enabled = meshWasEnabled;
			}
		}

		//??? Для объединенного MeshInSpace transform выбирать как у первого MeshInSpace или лучше считать средний transform?
		//??? MeshInSpace.ReplaceMeterial,ReplaceMeterialSelectively ? отбрасывать или выбирать первый. Как с geometry?
		static Component_MeshInSpace MergeMeshInSpaces( Component_MeshInSpace[] meshInSpaces, DocumentInstance document, UndoMultiAction undo )
		{
			if( meshInSpaces.Length < 2 )
				return null;

			var newTransform = meshInSpaces[ 0 ].Transform.Value;
			var newMatrixInverse = newTransform.ToMatrix4().GetInverse();
			var newRotationInverse = newTransform.Rotation.GetInverse();

			Component_Mesh.StructureClass newStructure = null;
			var newGeometries = new List<Component_MeshGeometry>();
			for( int i = 0; i < meshInSpaces.Length; i++ )
			{
				var m = meshInSpaces[ i ].Mesh.Value;
				if( m == null )
					continue;
				var oldTransform = meshInSpaces[ i ].Transform.Value;
				var transformMatrix = newMatrixInverse * oldTransform.ToMatrix4();
				var rotation = newRotationInverse * oldTransform.Rotation;

				var geometries = m.GetComponents<Component_MeshGeometry>();
				newStructure = Component_Mesh.StructureClass.Concat( newStructure, m.ExtractStructure().Structure, newGeometries.Count );
				foreach( var g in geometries )
				{
					if( g is Component_MeshGeometry_Procedural meshGeometryProcedural )
					{
						VertexElement[] vertexStructure = null;
						byte[] vertices = null;
						int[] indices = null;
						Component_Material material = null;
						Component_Mesh.StructureClass structure = null;
						meshGeometryProcedural.GetProceduralGeneratedData( ref vertexStructure, ref vertices, ref indices, ref material, ref structure );

						var newMeshGeometry = new Component_MeshGeometry();
						newMeshGeometry.Name = meshGeometryProcedural.Name;
						newMeshGeometry.VertexStructure = vertexStructure;
						newMeshGeometry.Vertices = vertices;
						newMeshGeometry.Indices = indices;

						newMeshGeometry.Material = meshGeometryProcedural.Material;
						//newMeshGeometry.Material = material;

						TransformVertices( newMeshGeometry.Vertices.Value, new MeshData.MeshGeometryFormat( newMeshGeometry.VertexStructure ), transformMatrix, rotation );
						newGeometries.Add( newMeshGeometry );
					}
					else
					{
						//??? Проверять CloneSupport?
						var newMeshGeometry = (Component_MeshGeometry)g.Clone();
						if( newMeshGeometry.Vertices.Value != null )
						{
							newMeshGeometry.Vertices = (byte[])newMeshGeometry.Vertices.Value.Clone();
							TransformVertices( newMeshGeometry.Vertices.Value, new MeshData.MeshGeometryFormat( newMeshGeometry.VertexStructure ), transformMatrix, rotation );
						}
						newGeometries.Add( newMeshGeometry );
					}
				}
			}

			//changes 

			var parent = meshInSpaces[ 0 ].Parent;
			undo.AddAction( new UndoActionComponentCreateDelete( document, meshInSpaces, create: false ) );
			Component_MeshInSpace newMeshInSpace = parent.CreateComponent<Component_MeshInSpace>();
			bool wasEnabled = newMeshInSpace.Enabled;
			try
			{

				newMeshInSpace.Enabled = false;
				newMeshInSpace.Name = CommonFunctions.GetUniqueFriendlyName( newMeshInSpace );
				newMeshInSpace.Transform = newTransform;

				var newMesh = newMeshInSpace.CreateComponent<Component_Mesh>();
				newMesh.Name = CommonFunctions.GetUniqueFriendlyName( newMesh );
				newMesh.Structure = newStructure;
				newMeshInSpace.Mesh = ReferenceUtility.MakeReference<Component_Mesh>( null, ReferenceUtility.CalculateRootReference( newMesh ) );

				foreach( var g in newGeometries )
				{
					newMesh.AddComponent( g );
					CommonFunctions.EnsureNameIsUnique( g );
				}
			}
			finally
			{
				newMeshInSpace.Enabled = wasEnabled;
			}

			undo.AddAction( new UndoActionComponentCreateDelete( document, new[] { newMeshInSpace }, create: true ) );
			return newMeshInSpace;
		}

		static unsafe void TransformVertices( byte[] vertices, MeshData.MeshGeometryFormat format, Matrix4 matrix, Quaternion rotation )
		{
			fixed ( byte* ptr = vertices )
			{
				int count = vertices.Length / format.vertexSize;
				for( int i = 0; i < count; i++ )
				{
					byte* vertexStart = ptr + i * format.vertexSize;
					Vector3F* posPtr = (Vector3F*)( vertexStart + format.positionOffset );
					*posPtr = ( *posPtr * matrix ).ToVector3F();

					Vector3F* normalPtr = (Vector3F*)( vertexStart + format.normalOffset );
					*normalPtr = ( *normalPtr * rotation ).ToVector3F();

					Vector4F* tangentPtr = (Vector4F*)( vertexStart + format.tangentOffset );
					*tangentPtr = new Vector4F( ( ( *tangentPtr ).ToVector3F() * rotation ).ToVector3F(), ( *tangentPtr ).W );
				}
			}
		}

		//////////////////////////////////////////////////////////

		public static void BoolActionGetState( EditorAction.GetStateContext context, ActionContext actionContext )
		{
			if( actionContext.SelectionMode == SelectionMode.Object && actionContext.GetSelectedMeshInSpaceArray().Length == 2 )
				context.Enabled = true;
		}

		public enum BoolActionEnum { Union, Intersect, Subtract }

		//Для выполнения операции выделить 2 MeshInSpace. Результат будет помещен в первый MeshInSpace. Если операция Union то в первом MeshInSpace объединятся все geometry.
		//Для операций Intersect,Subtract, geometry из первого MeshInSpace обрабатываются раздельно.
		//На основе библиотеки Net3dBool. Эта библиотека после действия возвращает массив Vector3[] positions, int[] indices. Данные о Structure теряются.
		//Потом автоматически строится простая Structure, расчитываются нормали.

		//BoolActionEnum.Union - Implements constructive solid geometry union operation for two selected Mesh In Space objects:
		//BoolActionEnum.Subtruct - Implements constructive solid geometry difference operation for two selected Mesh In Space objects.
		//                          The result object will be the first selected mesh minus the second selected mesh.
		//BoolActionEnum.Intersect - Implements constructive solid geometry intersect operation for two selected Mesh In Space objects.
		public static void BoolAction( ActionContext actionContext, BoolActionEnum boolAction )
		{
			if( actionContext.SelectionMode != SelectionMode.Object )
				return;

			void Exec( Component_MeshInSpace[] selectedMeshInSpaces, UndoMultiAction undoForMeshes, UndoMultiAction undoForOtherActions )
			{
				if( selectedMeshInSpaces.Length != 2 )
					return;

				BoolActionExecute( selectedMeshInSpaces[ 0 ], selectedMeshInSpaces[ 1 ], undoForMeshes, actionContext.DocumentWindow?.Document, boolAction );
			}
			ExecuteAction( actionContext, Exec );
		}

		public static void BoolActionExecute( Component_MeshInSpace firstMeshInSpace, Component_MeshInSpace secondMeshInSpace, UndoMultiAction undo, DocumentInstance document, BoolActionEnum boolAction )
		{
			//the first operand of the union operation must be a single geometry, otherwise duplicate parts will be made.
			if( boolAction == BoolActionEnum.Union && 1 < firstMeshInSpace.Mesh.Value.GetComponents<Component_MeshGeometry>().Length )
			{
				MergeGeometries( firstMeshInSpace.Mesh, document, undo );
			}
			bool needUndoForNextActions = true;
			CommonFunctions.ConvertProceduralMeshGeometries( document, firstMeshInSpace.Mesh, undo, ref needUndoForNextActions );

			List<(Vector3F[] positions, int[] indices)> data1List = GetData( firstMeshInSpace );
			(Vector3F[] positions, int[] indices) data2 = MergeData( GetData( secondMeshInSpace ) );

			//convert the second mesh in space, to the transform of first mesh in space
			var matrix = firstMeshInSpace.Transform.Value.ToMatrix4().GetInverse() * secondMeshInSpace.Transform.Value.ToMatrix4();
			Net3dBool.Vector3[] vertices2 = new Net3dBool.Vector3[ data2.positions.Length ];
			for( int i = 0; i < data2.positions.Length; i++ )
				vertices2[ i ] = ToNet3DBoolVector3( ( matrix * data2.positions[ i ] ).ToVector3F() );
			var operand2 = new Net3dBool.Solid( vertices2, data2.indices );

			var geometries = firstMeshInSpace.Mesh.Value.GetComponents<Component_MeshGeometry>();
			var resultGeometries = new List<(Vector3F[] positions, int[] indices, MeshData.MeshGeometryFormat format)>();
			var geometriesToDelete = new List<Component_MeshGeometry>();

			for( int geomIndex = 0; geomIndex < data1List.Count; geomIndex++ )
			{
				var data1 = data1List[ geomIndex ];
				Net3dBool.Vector3[] vertices1 = data1.positions.Select( ToNet3DBoolVector3 ).ToArray();

				var modeller = new Net3dBool.BooleanModeller( new Net3dBool.Solid( vertices1, data1.indices ), operand2 ); //Большую часть времени на вычисления занимает эта сторка

				Net3dBool.Solid result = null;
				switch( boolAction )
				{
				case BoolActionEnum.Union: result = modeller.GetUnion(); break;
				case BoolActionEnum.Intersect: result = modeller.GetIntersection(); break;
				case BoolActionEnum.Subtract: result = modeller.GetDifference(); break;
				default: return;
				}

				var newVertices = result.getVertices().Select( ToVector3F ).ToArray();
				if( 0 < newVertices.Length )
					resultGeometries.Add( (newVertices, result.getIndices(), new MeshData.MeshGeometryFormat( geometries[ geomIndex ].VertexStructure )) );
				else
					geometriesToDelete.Add( geometries[ geomIndex ] );
			}

			foreach( var g in resultGeometries )
				if( !CheckValid( g.positions, g.indices ) )
					throw new Exception();

			//delete empty mesh geometry //
			if( 0 < geometriesToDelete.Count )
				undo?.AddAction( new UndoActionComponentCreateDelete( document, geometriesToDelete.ToArray(), create: false ) );

			var meshData = MeshData.BuildFromRaw( resultGeometries );
			meshData?.Save( firstMeshInSpace.Mesh.Value, needUndoForNextActions ? undo : null, null ); //??? No selection?
			firstMeshInSpace.Mesh.Value?.RebuildStructure();
		}

		static bool CheckValid( Vector3F[] vertices, int[] indices )
		{
			foreach( var i in indices )
			{
				if( i < 0 && vertices.Length <= i )
					return false;
			}
			return true;
		}

		static Vector3F ToVector3F( Net3dBool.Vector3 v ) => new Vector3F( (float)v.x, (float)v.y, (float)v.z );

		static Net3dBool.Vector3 ToNet3DBoolVector3( Vector3F v ) => new Net3dBool.Vector3( v.X, v.Y, v.Z );

		//static byte[] ToVertices( Net3dBool.Vector3[] vertices, MeshData.MeshGeometryFormat vs )
		//{
		//	byte[] ret = new byte[ vertices.Length * vs.vertexSize ];
		//	unsafe
		//	{
		//		fixed ( byte* ptr = ret )
		//		{
		//			for( int i = 0; i < vertices.Length; i++ )
		//			{
		//				var v = vertices[ i ];

		//				var pos = (Vector3F*)( ptr + i * vs.vertexSize + vs.positionOffset );
		//				( *pos ) = new Vector3F( (float)v.x, (float)v.y, (float)v.z );
		//			}
		//		}
		//	}

		//	return ret;
		//}


		//The data are extracted from Component_Mesh.ExtractedStructure.
		//Хотя можно взять из MeshGeometry.Vertices,Indices, но там проиндексировано вместе с другими элементами(normals,...).
		static List<(Vector3F[] positions, int[] indices)> GetData( Component_MeshInSpace ms )
		{
			Component_Mesh.ExtractedStructure structure = ms.Mesh.Value.ExtractStructure();

			var ret = new List<(Vector3F[] positions, int[] indices)>();

			//extract old positions
			var oldPositions = new Vector3F[ structure.MeshGeometries.Length ][];
			for( int geomIndex = 0; geomIndex < structure.MeshGeometries.Length; geomIndex++ )
			{
				var vs = new MeshData.MeshGeometryFormat( structure.MeshGeometries[ geomIndex ].VertexStructure );
				oldPositions[ geomIndex ] = new Vector3F[ structure.MeshGeometries[ geomIndex ].Vertices.Length / vs.vertexSize ];

				unsafe
				{
					fixed ( byte* ptr = structure.MeshGeometries[ geomIndex ].Vertices )
					{
						for( int i = 0; i < oldPositions[ geomIndex ].Length; i++ )
							oldPositions[ geomIndex ][ i ] = *(Vector3F*)( ptr + i * vs.vertexSize + vs.positionOffset );
					}
				}
			}
			//------------

			var geomTriangles = new List<Component_Mesh.StructureClass.FaceVertex>[ structure.MeshGeometries.Length ];
			for( int i = 0; i < structure.MeshGeometries.Length; i++ )
				geomTriangles[ i ] = new List<Component_Mesh.StructureClass.FaceVertex>();
			for( int faceIndex = 0; faceIndex < structure.Structure.Faces.Length; faceIndex++ )
			{
				var face = structure.Structure.Faces[ faceIndex ];
				for( int i = 0; i < face.Triangles.Length; i++ )
					geomTriangles[ face.Triangles[ i ].RawGeometry ].Add( face.Triangles[ i ] );
			}

			for( int i = 0; i < geomTriangles.Length; i++ )
			{
				var g = geomTriangles[ i ];
				int[] oldVertexToNewMapping = new int[ structure.Structure.Vertices.Length ];
				for( int j = 0; j < oldVertexToNewMapping.Length; j++ )
					oldVertexToNewMapping[ j ] = -1;
				int nextIndex = 0;
				var newPositions = new List<Vector3F>();
				var newIndices = new int[ g.Count ];

				for( int j = 0; j < g.Count; j++ )
				{
					var tv = g[ j ];
					int newIndex = oldVertexToNewMapping[ tv.Vertex ];
					if( newIndex == -1 )
					{
						newIndex = nextIndex++;
						oldVertexToNewMapping[ tv.Vertex ] = newIndex;
						newPositions.Add( oldPositions[ tv.RawGeometry ][ tv.RawVertex ] );
					}
					newIndices[ j ] = newIndex;
				}
				ret.Add( (newPositions.ToArray(), newIndices) );
			}

			return ret;
		}

		/*
		// Недостаток - извлекает из geometry, там проиндексировано не только по Position, но и по другим (normal,...)
		static List<(Vector3F[] positions, int[] indices)> GetData2( Component_MeshInSpace ms )
		{
			var geoms = ms.Mesh.Value.GetComponents<Component_MeshGeometry>();
			var ret = new List<(Vector3F[] vertices, int[] indices)>();
			foreach( var g in geoms )
			{
				int[] indices = null;
				VertexElement[] vertexStructure = null;
				byte[] verticesData = null;
				if( g is Component_MeshGeometry_Procedural proc )
				{
					Component_Material material = null;
					Component_Mesh.StructureClass structure = null;
					proc.GetProceduralGeneratedData( ref vertexStructure, ref verticesData, ref indices, ref material, ref structure );
				}
				else
				{
					vertexStructure = g.VertexStructure;
					verticesData = g.Vertices.Value;
					indices = g.Indices.Value;
				}

				var vs = new MeshData.MeshGeometryFormat( vertexStructure );
				var positions = new Vector3F[ verticesData.Length / vs.vertexSize ];
				unsafe
				{
					fixed ( byte* ptr = verticesData )
					{
						for( int i = 0; i < positions.Length; i++ )
							positions[ i ] = *(Vector3F*)( ptr + i * vs.vertexSize + vs.positionOffset );
					}
				}
				ret.Add( (positions, indices) );
			}

			return ret;
		}
		*/

		static (Vector3F[] positions, int[] indices) MergeData( List<(Vector3F[] positions, int[] indices)> data )
		{
			var positions = new List<Vector3F>();
			var indices = new List<int>();

			foreach( var d in data )
			{
				int indexOffset = positions.Count;
				positions.AddRange( d.positions );
				int start = indices.Count;
				indices.AddRange( d.indices );
				for( int i = start; i < indices.Count; i++ )
					indices[ i ] += indexOffset;
			}
			return (positions.ToArray(), indices.ToArray());
		}

		//////////////////////////////////////////////////////////


		////!!!!
		//public static void MergeObjects_Temp()
		//{
		//	//var meshInSpaces = GetSelectedMeshInSpaces();

		//	////!!!!больше двух
		//	//if( meshInSpaces.Length != 2 )
		//	//	return;

		//	////!!!!

		//	//var mesh0 = meshInSpaces[ 0 ].Mesh.Value;
		//	//var mesh1 = meshInSpaces[ 1 ].Mesh.Value;

		//	////disable mesh
		//	//bool mesh0WasEnabled = mesh0.Enabled;
		//	//mesh0.Enabled = false;
		//	//bool mesh1WasEnabled = mesh1.Enabled;
		//	//mesh1.Enabled = false;
		//	//try
		//	//{
		//	//	if( !ConvertMeshGeometryType( mesh0 ) )
		//	//		return;
		//	//	//!!!!
		//	//	if( !ConvertMeshGeometryType( mesh1 ) )
		//	//		return;

		//	//	//!!!!что еще проверять
		//	//	if( mesh0.Structure == null )
		//	//	{
		//	//		EditorMessageBox.ShowWarning( "Mesh structure is not exists." );
		//	//		return;
		//	//	}
		//	//	if( mesh1.Structure == null )
		//	//	{
		//	//		EditorMessageBox.ShowWarning( "Mesh structure is not exists." );
		//	//		return;
		//	//	}

		//	//	//!!!!несколько MeshGeometry в каждом меше
		//	//	//!!!!разные структуры вершины
		//	//	//!!!!StructureClass? хотя не меняется
		//	//	//!!!!что еще

		//	//	var meshGeometry0 = mesh0.GetComponent<Component_MeshGeometry>();
		//	//	var meshGeometry1 = mesh1.GetComponent<Component_MeshGeometry>();

		//	//	//!!!!

		//	//	meshGeometry0.VertexStructure.Value.GetInfo( out var vertexSize, out var holes );
		//	//	int vertexCount0 = meshGeometry0.Vertices.Value.Length / vertexSize;

		//	//	//!!!!смещать положение
		//	//	meshGeometry0.Vertices = CollectionUtility.Merge( meshGeometry0.Vertices.Value, meshGeometry1.Vertices.Value );

		//	//	var list = new List<int>();
		//	//	list.AddRange( meshGeometry0.Indices.Value );
		//	//	foreach( var index in meshGeometry1.Indices.Value )
		//	//		list.Add( index + vertexCount0 );
		//	//	meshGeometry0.Indices = list.ToArray();

		//	//	meshInSpaces[ 1 ].Dispose();
		//	//}
		//	//finally
		//	//{
		//	//	//enable mesh
		//	//	mesh0.Enabled = mesh0WasEnabled;
		//	//	if( !meshInSpaces[ 1 ].Disposed )
		//	//		mesh1.Enabled = mesh1WasEnabled;
		//	//}
		//}

		/////////////////////////////////////////

		public static void SetColorGetState( EditorAction.GetStateContext context, ActionContext actionContext )
		{
			if( actionContext.SelectionMode == SelectionMode.Object && actionContext.GetSelectedMesh().mesh != null ||
				0 < actionContext.Selection.VertexCount ||
				0 < actionContext.Selection.EdgeCount ||
				0 < actionContext.Selection.FaceCount
				)
			{
				context.Enabled = true;
			}
		}

		public static void SetColor( ActionContext actionContext, ColorValue color )
		{
			if( actionContext.SelectionMode == SelectionMode.Object )
			{
				var document = actionContext.DocumentWindow.Document;
				var undo = new UndoMultiAction();

				foreach( var meshInSpace in actionContext.GetSelectedMeshInSpaceArray( false ) )
				{
					//set Color property
					{
						var property = (Metadata.Property)meshInSpace.MetadataGetMemberBySignature( "property:" + nameof( Component_MeshInSpace.Color ) );
						undo.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshInSpace, property, meshInSpace.Color ) ) );

						meshInSpace.Color = color;
					}

					//set white material for mesh geometries if material is null
					{
						var referenceToMaterial = new ReferenceNoValue( @"Base\Materials\White.material" );

						var mesh = meshInSpace.Mesh.Value;
						if( mesh != null )
						{
							if( mesh.Parent == meshInSpace )
							{
								foreach( var geom in mesh.GetComponents<Component_MeshGeometry>() )
								{
									if( !geom.Material.ReferenceSpecified && geom.Material.Value == null )
										OneMeshActions.SetMaterialForGeometry( geom, referenceToMaterial, undo );
								}
							}
						}
					}
				}
				document.CommitUndoAction( undo );
			}
			else
				OneMeshActions.SetVertexColor( actionContext, color );
		}

		public static ColorValue? GetInitialColor( ActionContext actionContext )
		{
			if( actionContext.SelectionMode == SelectionMode.Object )
			{
				var msAr = actionContext.GetSelectedMeshInSpaceArray( false );
				if( 0 < msAr.Length )
				{
					ColorValue color = msAr[ 0 ].Color;
					for( int i = 1; i < msAr.Length; i++ )
						if( color != msAr[ i ].Color )
							return null;
					return color;
				}
				else
					return null;
			}
			else
				return OneMeshActions.GetInitialColor( actionContext );
		}

		/////////////////////////////////////////

		public static void MirrorObjectsGetState( EditorAction.GetStateContext context, ActionContext actionContext )
		{
			if( actionContext.SelectionMode == SelectionMode.Object && actionContext.GetSelectedMesh().mesh != null )
			{
				context.Enabled = true;
			}
		}

		public static void MirrorObjects( ActionContext actionContext, int axis )
		{
			void Execute( Component_MeshInSpace[] selectedMeshInSpaces, UndoMultiAction undoForMeshes, UndoMultiAction undoForOtherActions )
			{
				foreach( var ms in selectedMeshInSpaces )
				{
					bool needUndoForNextActions = undoForMeshes != null;
					var mesh = ms.Mesh.Value;
					CommonFunctions.ConvertProceduralMeshGeometries( actionContext.DocumentWindow.Document, mesh, undoForMeshes, ref needUndoForNextActions );

					Mirror( mesh, axis, needUndoForNextActions ? undoForMeshes : null );
				}
			}
			ExecuteAction( actionContext, Execute );
		}

		static void Mirror( Component_Mesh mesh, int axis, UndoMultiAction undoForMeshes )
		{
			Component_Mesh.ExtractedStructure structure = mesh.ExtractStructure();

			for( int geomIndex = 0; geomIndex < structure.MeshGeometries.Length; geomIndex++ )
			{
				var geom = structure.MeshGeometries[ geomIndex ];
				geom.Vertices = (byte[])geom.Vertices.Clone();
				geom.Indices = (int[])geom.Indices.Clone();
				var vs = new MeshData.MeshGeometryFormat( geom.VertexStructure );
				int vertexCount = geom.Vertices.Length / vs.vertexSize;

				unsafe
				{
					fixed ( byte* ptr = geom.Vertices )
					{
						for( int i = 0; i < vertexCount; i++ )
						{
							byte* vertexPtr = ptr + i * vs.vertexSize;

							ref Vector3F posRef = ref *(Vector3F*)( vertexPtr + vs.positionOffset );
							ref Vector3F normalRef = ref *(Vector3F*)( vertexPtr + vs.normalOffset );
							ref Vector4F tangentRef = ref *(Vector4F*)( vertexPtr + vs.tangentOffset );
							switch( axis )
							{
							case 0:
								posRef.X = -posRef.X;
								if( 0 <= vs.normalOffset )
									normalRef.X = -normalRef.X;
								if( 0 <= vs.tangentOffset )
									tangentRef.X = -tangentRef.X;

								break;
							case 1:
								posRef.Y = -posRef.Y;
								if( 0 <= vs.normalOffset )
									normalRef.Y = -normalRef.Y;
								if( 0 <= vs.tangentOffset )
									tangentRef.Y = -tangentRef.Y;
								break;

							case 2:
								posRef.Z = -posRef.Z;
								if( 0 <= vs.normalOffset )
									normalRef.Z = -normalRef.Z;
								if( 0 <= vs.tangentOffset )
									tangentRef.Z = -tangentRef.Z;
								break;
							default: throw new Exception();
							}
						}
					}
				}

				//flip triangles
				for( int i = 0; i < geom.Indices.Length; i += 3 )
				{
					int temp = geom.Indices[ i + 1 ];
					geom.Indices[ i + 1 ] = geom.Indices[ i + 2 ];
					geom.Indices[ i + 2 ] = temp;
				}

				foreach( var face in structure.Structure.Faces )
				{
					for( int i = 0; i < face.Triangles.Length; i += 3 )
					{
						var temp = face.Triangles[ i + 1 ];
						face.Triangles[ i + 1 ] = face.Triangles[ i + 2 ];
						face.Triangles[ i + 2 ] = temp;
					}
				}
			}

			//-------------------------------------

			var meshGeometries = mesh.GetComponents<Component_MeshGeometry>();

			for( int i = 0; i < meshGeometries.Length; i++ )
			{
				var meshGeometry = meshGeometries[ i ];

				if( meshGeometry is Component_MeshGeometry_Procedural )
					throw new Exception();

				if( undoForMeshes != null )
				{
					var property = (Metadata.Property)meshGeometry.MetadataGetMemberBySignature( "property:" + nameof( Component_MeshGeometry.Vertices ) );
					undoForMeshes.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshGeometry, property, meshGeometry.Vertices ) ) );

					var propertyIndices = (Metadata.Property)meshGeometry.MetadataGetMemberBySignature( "property:" + nameof( Component_MeshGeometry.Indices ) );
					undoForMeshes.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshGeometry, propertyIndices, meshGeometry.Indices ) ) );
				}
				meshGeometry.Vertices = structure.MeshGeometries[ i ].Vertices;
				meshGeometry.Indices = structure.MeshGeometries[ i ].Indices;
			}

			if( undoForMeshes != null )
			{
				var propertyStructure = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Component_Mesh.Structure ) );
				undoForMeshes.AddAction( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, propertyStructure, mesh.Structure ) ) );
			}
			mesh.Structure = structure.Structure;
		}

		/////////////////////////////////////////

		public static void AddPaintLayerGetState( EditorAction.GetStateContext context, ActionContext actionContext )
		{
			if( actionContext.SelectionMode == SelectionMode.Object && actionContext.GetSelectedMeshInSpaceArray().Length > 0 )
				context.Enabled = true;
		}

		public static void AddPaintLayer( ActionContext actionContext )
		{
			if( actionContext.SelectionMode == SelectionMode.Object )
			{
				var meshesInSpace = actionContext.GetSelectedMeshInSpaceArray();
				if( meshesInSpace.Length != 0 )
				{
					var newObjects = new List<Component>();

					foreach( var meshInSpace in meshesInSpace )
					{
						var modifier = meshInSpace.CreateComponent<Component_PaintLayer>( enabled: false );
						modifier.Name = CommonFunctions.GetUniqueFriendlyName( modifier );
						modifier.Enabled = true;

						newObjects.Add( modifier );
					}

					actionContext.DocumentWindow.Focus();

					//undo
					var document = actionContext.DocumentWindow.Document;
					var action = new UndoActionComponentCreateDelete( document, newObjects, true );
					document.CommitUndoAction( action );
					actionContext.DocumentWindow.SelectObjects( newObjects.ToArray() );
				}
			}
		}
	}
}
