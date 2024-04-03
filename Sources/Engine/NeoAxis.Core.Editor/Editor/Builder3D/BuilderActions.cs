// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public static partial class BuilderActions
	{
		public class MeshInSpaceItem
		{
			public MeshInSpace meshInSpace;
			public List<UndoSystem.Action> undoMeshChanges = new List<UndoSystem.Action>();
			public bool undoMeshChangesLocked;//means no sense to add because a new mesh was created
		}

		public delegate void ActionExecuteDelegate( MeshInSpaceItem[] meshInSpaces, List<UndoSystem.Action> undoMeshInSpacesCreateDelete );

		public static void ExecuteAction( BuilderActionContext actionContext, ActionExecuteDelegate execute, bool copyExternalMeshOnlyOfFirstMeshInSpace )
		{
			var meshInSpaces = actionContext.GetSelectedMeshInSpaces();
			if( meshInSpaces.Length == 0 )
				return;

			//!!!! Enabled = false для Parent у MeshInSpace? Тогда Select вынести из Execute.

			var meshInSpaceItems = new MeshInSpaceItem[ meshInSpaces.Length ];
			for( int n = 0; n < meshInSpaceItems.Length; n++ )
			{
				var meshInSpace = meshInSpaces[ n ];

				var item = new MeshInSpaceItem();
				meshInSpaceItems[ n ] = item;

				item.meshInSpace = meshInSpace;

				//copy external meshes
				if( n == 0 || !copyExternalMeshOnlyOfFirstMeshInSpace )
				{
					var mesh = meshInSpace.Mesh.Value;
					if( mesh != null )
					{
						BuilderCommonFunctions.CopyExternalMesh( actionContext.DocumentWindow.Document2, meshInSpace, ref mesh, item.undoMeshChanges, ref item.undoMeshChangesLocked );
					}
				}
			}

			var undoMeshInSpacesCreateDelete = new List<UndoSystem.Action>();

			//execute
			execute( meshInSpaceItems, undoMeshInSpacesCreateDelete );

			//undo
			var undoActions = new UndoMultiAction();
			foreach( var item in meshInSpaceItems )
				undoActions.AddActions( item.undoMeshChanges );
			undoActions.AddActions( undoMeshInSpacesCreateDelete );

			undoActions.Actions.Reverse();

			actionContext.DocumentWindow?.Document2?.CommitUndoAction( undoActions );
		}

		//static bool CheckValidMeshWithMessageBox( Mesh mesh )
		//{
		//	if( !BuilderCommonFunctions.CheckValidMesh( mesh, out string error ) )
		//	{
		//		EditorMessageBox.ShowWarning( error );
		//		return false;
		//	}
		//	return true;
		//}

		public static void MergeObjectsGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode != BuilderSelectionMode.Object )
				return;

			//merge mesh geometries
			var geometries = actionContext.GetSelectedMeshGeometries();
			if( actionContext.ObjectsInFocus.All( obj => obj is MeshGeometry ) && geometries.Length > 1 )
			{
				var equalParent = true;
				for( int n = 1; n < geometries.Length; n++ )
				{
					if( geometries[ 0 ].Parent != geometries[ n ].Parent )
						equalParent = false;
				}
				if( equalParent )
					context.Enabled = true;
			}

			//mesh mesh in spaces
			if( actionContext.ObjectsInFocus.All( obj => obj is MeshInSpace ) && actionContext.ObjectsInFocus.Length > 1 )
				context.Enabled = true;
		}

		public static void MergeObjects( BuilderActionContext actionContext )
		{
			//merge mesh geometries
			var geometries = actionContext.GetSelectedMeshGeometries();
			if( actionContext.ObjectsInFocus.All( obj => obj is MeshGeometry ) && geometries.Length > 1 )
			{
				var mesh = geometries[ 0 ].Parent as Mesh;
				if( mesh != null )
				{
					var meshInSpace = mesh.Parent as MeshInSpace;
					if( meshInSpace != null )
					{
						bool wasEnabled = meshInSpace.Enabled;
						meshInSpace.Enabled = false;
						try
						{
							var document = actionContext.DocumentWindow.Document2;

							var item = new MeshInSpaceItem();
							item.meshInSpace = meshInSpace;
							BuilderCommonFunctions.CopyExternalMesh( document, meshInSpace, ref mesh, item.undoMeshChanges, ref item.undoMeshChangesLocked );

							MergeGeometries( mesh, geometries, document, item.undoMeshChanges, ref item.undoMeshChangesLocked );

							//undo
							var undoActions = new UndoMultiAction();
							undoActions.AddActions( item.undoMeshChanges );

							undoActions.Actions.Reverse();

							document.CommitUndoAction( undoActions );
						}
						finally
						{
							meshInSpace.Enabled = wasEnabled;
						}
					}
				}
			}

			//mesh mesh in spaces
			if( actionContext.ObjectsInFocus.All( obj => obj is MeshInSpace ) && actionContext.ObjectsInFocus.Length > 1 )
			{
				void Execute( MeshInSpaceItem[] meshInSpaceItems, List<UndoSystem.Action> undoMeshInSpacesCreateDelete )
				{
					MergeMeshInSpaces( meshInSpaceItems, actionContext.DocumentWindow?.Document2, undoMeshInSpacesCreateDelete );
					//var newMeshInSpace = MergeMeshInSpaces( meshInSpaces, actionContext.DocumentWindow?.Document, undoMeshInSpacesCreateDelete );
					//actionContext.SelectMeshesInSpace( newMeshInSpace );
				}

				ExecuteAction( actionContext, Execute, true );
			}
		}

		static void MergeGeometries( Mesh mesh, MeshGeometry[] geometries, DocumentInstance document, List<UndoSystem.Action> undoMeshChanges, ref bool undoMeshChangesLocked )
		{
			var extractedData = mesh.ExtractData();

			var newIndices = new List<int>();
			var newVertices = new List<byte>();
			var newVertexStructure = extractedData.MeshGeometries[ 0 ].VertexStructure;
			var newVertexFormat = new BuilderMeshData.MeshGeometryFormat( newVertexStructure );

			for( int geomIndex = 0; geomIndex < extractedData.MeshGeometries.Length; geomIndex++ )
			{
				var g = extractedData.MeshGeometries[ geomIndex ];


				if( g.Vertices == null || g.Indices == null )
					continue;
				int indexOffset = newVertices.Count / newVertexFormat.vertexSize;

				for( int i = 0; i < g.Indices.Length; i++ )
					newIndices.Add( g.Indices[ i ] + indexOffset );

				if( !BuilderCommonFunctions.IsSameVertexStructure( newVertexStructure, g.VertexStructure ) )
					g.Vertices = BuilderMeshData.ConvertToFormat( new BuilderMeshData.MeshGeometryFormat( g.VertexStructure ), g.Vertices, newVertexFormat );

				newVertices.AddRange( g.Vertices );

				if( extractedData.Structure != null )
				{
					foreach( var face in extractedData.Structure.Faces )
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
			}


			var firstGeometry = geometries[ 0 ];
			var insertIndex = firstGeometry.Parent.Components.IndexOf( firstGeometry );

			var newGeometry = mesh.CreateComponent<MeshGeometry>( insertIndex );
			newGeometry.VertexStructure = newVertexStructure;
			newGeometry.Vertices = newVertices.ToArray();
			newGeometry.Indices = newIndices.ToArray();
			newGeometry.Material = firstGeometry.Material;

			//add created geometry to undo
			if( !undoMeshChangesLocked )
				undoMeshChanges.Add( new UndoActionComponentCreateDelete( document, new Component[] { newGeometry }, true ) );

			//delete old mesh geometries
			if( !undoMeshChangesLocked )
				undoMeshChanges.Add( new UndoActionComponentCreateDelete( document, geometries, false ) );
			else
			{
				foreach( var geometry in geometries )
					geometry.Dispose();
			}

			//undo mesh structure and rebuild
			if( !undoMeshChangesLocked )
			{
				var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Mesh.Structure ) );
				undoMeshChanges.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure ) ) );
			}
			mesh.BuildStructure();

			newGeometry.Name = BuilderCommonFunctions.GetUniqueFriendlyName( newGeometry );
		}

		static void/*MeshInSpace*/ MergeMeshInSpaces( MeshInSpaceItem[] meshInSpaceItems, DocumentInstance document, List<UndoSystem.Action> undoMeshInSpacesCreateDelete )
		{
			var meshInSpaces = meshInSpaceItems.Select( item => item.meshInSpace ).ToArray();

			if( meshInSpaces.Length < 2 )
				return;// null;

			var firstMeshInSpaceItem = meshInSpaceItems[ 0 ];
			var firstMeshInSpace = meshInSpaces[ 0 ];

			var newTransform = firstMeshInSpace.Transform.Value;
			var newMatrixInverse = newTransform.ToMatrix4().GetInverse();
			var newRotationInverse = newTransform.Rotation.GetInverse();

			Mesh.StructureClass newStructure = null;
			var newGeometries = new List<MeshGeometry>();
			for( int i = 0; i < meshInSpaces.Length; i++ )
			{
				var m = meshInSpaces[ i ].Mesh.Value;
				if( m == null )
					continue;
				var oldTransform = meshInSpaces[ i ].Transform.Value;
				var transformMatrix = newMatrixInverse * oldTransform.ToMatrix4();
				var rotation = newRotationInverse * oldTransform.Rotation;

				var geometries = m.GetComponents<MeshGeometry>();
				newStructure = Mesh.StructureClass.Concat( newStructure, m.ExtractData().Structure, newGeometries.Count );
				foreach( var geometry in geometries )
				{
					if( geometry is MeshGeometry_Procedural meshGeometryProcedural )
					{
						VertexElement[] vertexStructure = null;
						byte[] vertices = null;
						int[] indices = null;
						Material material = null;
						byte[] voxelData = null;
						byte[] clusterData = null;
						Mesh.StructureClass structure = null;
						meshGeometryProcedural.GetProceduralGeneratedData( ref vertexStructure, ref vertices, ref indices, ref material, ref voxelData, ref clusterData, ref structure );

						var newMeshGeometry = new MeshGeometry();
						newMeshGeometry.Name = meshGeometryProcedural.Name;
						newMeshGeometry.VertexStructure = vertexStructure;
						newMeshGeometry.Vertices = vertices;
						newMeshGeometry.Indices = indices;

						//!!!!clusters, voxels?

						newMeshGeometry.Material = meshGeometryProcedural.Material;
						//newMeshGeometry.Material = material;

						TransformVertices( newMeshGeometry.Vertices.Value, new BuilderMeshData.MeshGeometryFormat( newMeshGeometry.VertexStructure ), transformMatrix, rotation );
						newGeometries.Add( newMeshGeometry );
					}
					else
					{
						var newMeshGeometry = (MeshGeometry)geometry.Clone();

						if( newMeshGeometry.Vertices.Value != null )
						{
							var vertices = (byte[])newMeshGeometry.Vertices.Value.Clone();
							TransformVertices( vertices, new BuilderMeshData.MeshGeometryFormat( newMeshGeometry.VertexStructure ), transformMatrix, rotation );
							newMeshGeometry.Vertices = vertices;
						}
						newGeometries.Add( newMeshGeometry );
					}
				}
			}

			//changes 

			//var parent = firstMeshInSpace.Parent;
			//var insertIndex = firstMeshInSpace.Parent.Components.IndexOf( firstMeshInSpace );

			//var newMeshInSpace = parent.CreateComponent<MeshInSpace>( insertIndex, enabled: false );
			bool wasEnabled = firstMeshInSpace.Enabled;
			try
			{
				firstMeshInSpace.Enabled = false;

				//newMeshInSpace.Name = BuilderCommonFunctions.GetUniqueFriendlyName( newMeshInSpace );
				//newMeshInSpace.Transform = newTransform;

				////!!!!может не так делать? Clone? копировать всё в первый?
				//newMeshInSpace.ReplaceMaterial = firstMeshInSpace.ReplaceMaterial;
				//for( int n = 0; n < firstMeshInSpace.ReplaceMaterialSelectively.Count; n++ )
				//	newMeshInSpace.ReplaceMaterialSelectively.Add( firstMeshInSpace.ReplaceMaterialSelectively[ n ] );
				//newMeshInSpace.Color = firstMeshInSpace.Color;
				//newMeshInSpace.VisibilityDistance = firstMeshInSpace.VisibilityDistance;
				//newMeshInSpace.CastShadows = firstMeshInSpace.CastShadows;
				//newMeshInSpace.ReceiveDecals = firstMeshInSpace.ReceiveDecals;
				//newMeshInSpace.MotionBlurFactor = firstMeshInSpace.MotionBlurFactor;
				//if( firstMeshInSpace.SpecialEffects.ReferenceSpecified )
				//	newMeshInSpace.SpecialEffects = firstMeshInSpace.SpecialEffects;
				//else if( firstMeshInSpace.SpecialEffects.Value != null )
				//	newMeshInSpace.SpecialEffects = new List<ObjectSpecialRenderingEffect>( firstMeshInSpace.SpecialEffects.Value );

				//firstMeshInSpaceItem.undoMeshChanges

				var mesh = firstMeshInSpace.GetComponent<Mesh>();
				if( mesh != null )
				{
					//var newMesh = firstMeshInSpace.CreateComponent<Mesh>();
					//newMesh.Name = BuilderCommonFunctions.GetUniqueFriendlyName( newMesh );

					//firstMeshInSpace.Mesh = ReferenceUtility.MakeThisReference( firstMeshInSpace, newMesh );
					//newMeshInSpace.Mesh = ReferenceUtility.MakeThisReference( newMeshInSpace, newMesh );

					if( !firstMeshInSpaceItem.undoMeshChangesLocked )
						firstMeshInSpaceItem.undoMeshChanges.Add( new UndoActionComponentCreateDelete( document, mesh.GetComponents(), false ) );
					else
					{
						foreach( var c in mesh.GetComponents() )
							c.Dispose();
					}

					foreach( var geometry in newGeometries )
					{
						mesh.AddComponent( geometry );
						BuilderCommonFunctions.EnsureNameIsUnique( geometry );
					}

					if( !firstMeshInSpaceItem.undoMeshChangesLocked )
						firstMeshInSpaceItem.undoMeshChanges.Add( new UndoActionComponentCreateDelete( document, newGeometries.ToArray(), true ) );

					//undo mesh structure and rebuild
					if( !firstMeshInSpaceItem.undoMeshChangesLocked )
					{
						var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Mesh.Structure ) );
						firstMeshInSpaceItem.undoMeshChanges.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, mesh.Structure ) ) );
					}
					mesh.BuildStructure();
				}
			}
			finally
			{
				firstMeshInSpace.Enabled = wasEnabled;
				//newMeshInSpace.Enabled = wasEnabled;
			}

			var meshInSpacesWithoutFirst = new List<MeshInSpace>( meshInSpaces );
			meshInSpacesWithoutFirst.RemoveAt( 0 );

			undoMeshInSpacesCreateDelete.Add( new UndoActionComponentCreateDelete( document, meshInSpacesWithoutFirst.ToArray(), false ) );
			//undoMeshInSpacesCreateDelete.Add( new UndoActionComponentCreateDelete( document, meshInSpaces, false ) );
			//undoMeshInSpacesCreateDelete.Add( new UndoActionComponentCreateDelete( document, new[] { newMeshInSpace }, true ) );

			//return newMeshInSpace;
		}

		internal /*obfuscator*/ static unsafe void TransformVertices( byte[] vertices, BuilderMeshData.MeshGeometryFormat format, Matrix4 matrix, Quaternion rotation )
		{
			fixed( byte* ptr = vertices )
			{
				int count = vertices.Length / format.vertexSize;
				for( int i = 0; i < count; i++ )
				{
					byte* vertexStart = ptr + i * format.vertexSize;
					Vector3F* posPtr = (Vector3F*)( vertexStart + format.positionOffset );
					*posPtr = ( *posPtr * matrix ).ToVector3F();

					if( format.normalOffset != -1 )
					{
						if( format.normalFormat == VertexElementType.Float3 )
						{
							Vector3F* normalPtr = (Vector3F*)( vertexStart + format.normalOffset );
							*normalPtr = ( *normalPtr * rotation ).ToVector3F();
						}
						else if( format.normalFormat == VertexElementType.Half3 )
						{
							Vector3H* normalPtr = (Vector3H*)( vertexStart + format.normalOffset );
							*normalPtr = ( *normalPtr * rotation ).ToVector3H();
						}
					}

					if( format.tangentOffset != -1 )
					{
						if( format.tangentFormat == VertexElementType.Float4 )
						{
							Vector4F* tangentPtr = (Vector4F*)( vertexStart + format.tangentOffset );
							*tangentPtr = new Vector4F( ( ( *tangentPtr ).ToVector3F() * rotation ).ToVector3F(), ( *tangentPtr ).W );
						}
						else if( format.tangentFormat == VertexElementType.Half4 )
						{
							Vector4H* tangentPtr = (Vector4H*)( vertexStart + format.tangentOffset );
							var tangent2 = ( *tangentPtr ).ToVector4F();
							*tangentPtr = new Vector4F( ( tangent2.ToVector3F() * rotation ).ToVector3F(), tangent2.W ).ToVector4H();
						}
					}
				}
			}
		}

		//////////////////////////////////////////////////////////

		public static void BoolActionGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object )
			{
				var meshInSpaces = actionContext.GetSelectedMeshInSpaces();
				if( meshInSpaces.Length == 2 && meshInSpaces[ 0 ].Mesh.Value != null && meshInSpaces[ 1 ].Mesh.Value != null )
					context.Enabled = true;
			}
		}

		public enum BoolActionEnum
		{
			Union,
			Intersect,
			Subtract
		}

		static bool CheckValid( Vector3F[] vertices, int[] indices )
		{
			foreach( var i in indices )
			{
				if( i < 0 || i >= vertices.Length )
					return false;
			}
			return true;
		}

		public static void BoolActionExecute( MeshInSpaceItem[] meshInSpaceItems, List<UndoSystem.Action> undoMeshInSpacesCreateDelete, DocumentInstance document, BoolActionEnum boolAction )
		{
			var meshInSpaceItem1 = meshInSpaceItems[ 0 ];
			var meshInSpace1 = meshInSpaceItem1.meshInSpace;
			var meshInSpace2 = meshInSpaceItems[ 1 ].meshInSpace;

			//the first operand of the union operation must be a single geometry, otherwise duplicate parts will be made
			if( boolAction == BoolActionEnum.Union && meshInSpace1.Mesh.Value.GetComponents<MeshGeometry>().Length > 1 )
			{
				var geometries2 = meshInSpace1.Mesh.Value.GetComponents<MeshGeometry>();
				MergeGeometries( meshInSpace1.Mesh, geometries2, document, meshInSpaceItem1.undoMeshChanges, ref meshInSpaceItem1.undoMeshChangesLocked );
			}

			BuilderCommonFunctions.ConvertProceduralMeshGeometries( document, meshInSpace1.Mesh, meshInSpaceItem1.undoMeshChanges, ref meshInSpaceItem1.undoMeshChangesLocked );

			List<(Vector3F[] positions, int[] indices)> data1List = GetData( meshInSpace1 );

			(Vector3F[] positions, int[] indices) data2 = MergeData( GetData( meshInSpace2 ) );

			//convert the second mesh in space, to the transform of first mesh in space
			var matrix = meshInSpace1.Transform.Value.ToMatrix4().GetInverse() * meshInSpace2.Transform.Value.ToMatrix4();
			var vertices2 = new Internal.Net3dBool.Vector3[ data2.positions.Length ];
			for( int i = 0; i < data2.positions.Length; i++ )
				vertices2[ i ] = ToNet3DBoolVector3( ( matrix * data2.positions[ i ] ).ToVector3F() );
			var operand2 = new Internal.Net3dBool.Solid( vertices2, data2.indices );

			var geometries = meshInSpace1.Mesh.Value.GetComponents<MeshGeometry>();
			var resultGeometries = new List<(Vector3F[] positions, int[] indices, BuilderMeshData.MeshGeometryFormat format)>();
			var geometriesToDelete = new List<MeshGeometry>();

			for( int geometryIndex = 0; geometryIndex < data1List.Count; geometryIndex++ )
			{
				var data1 = data1List[ geometryIndex ];
				var vertices1 = data1.positions.Select( ToNet3DBoolVector3 ).ToArray();

				var modeller = new Internal.Net3dBool.BooleanModeller( new Internal.Net3dBool.Solid( vertices1, data1.indices ), operand2 );

				Internal.Net3dBool.Solid result = null;
				switch( boolAction )
				{
				case BoolActionEnum.Union: result = modeller.GetUnion(); break;
				case BoolActionEnum.Intersect: result = modeller.GetIntersection(); break;
				case BoolActionEnum.Subtract: result = modeller.GetDifference(); break;
				}

				var newVertices = result.getVertices().Select( ToVector3F ).ToArray();
				if( newVertices.Length != 0 )
				{
					resultGeometries.Add( (newVertices, result.getIndices(), new BuilderMeshData.MeshGeometryFormat( geometries[ geometryIndex ].VertexStructure )) );
				}
				else
					geometriesToDelete.Add( geometries[ geometryIndex ] );
			}

			foreach( var g in resultGeometries )
			{
				if( !CheckValid( g.positions, g.indices ) )
				{
					//!!!!
					throw new Exception();
				}
			}

			//delete empty mesh geometry
			if( geometriesToDelete.Count != 0 )
			{
				if( !meshInSpaceItem1.undoMeshChangesLocked )
					meshInSpaceItem1.undoMeshChanges.Add( new UndoActionComponentCreateDelete( document, geometriesToDelete.ToArray(), false ) );
				else
				{
					foreach( var geometry in geometriesToDelete )
						geometry.Dispose();
				}
			}

			//!!!!
			var meshData = BuilderMeshData.BuildFromRaw( resultGeometries );

			//??? No selection?
			meshData?.Save( meshInSpace1.Mesh.Value, null, meshInSpaceItem1.undoMeshChanges, ref meshInSpaceItem1.undoMeshChangesLocked );
			meshInSpace1.Mesh.Value?.BuildStructure();
		}

		//Результат будет помещен в первый MeshInSpace. Если операция Union то в первом MeshInSpace объединятся все geometry.
		//Для операций Intersect, Subtract geometry из первого MeshInSpace обрабатываются раздельно.
		public static void BoolAction( BuilderActionContext actionContext, BoolActionEnum boolAction )
		{
			if( actionContext.SelectionMode != BuilderSelectionMode.Object )
				return;

			void Execute( MeshInSpaceItem[] meshInSpaceItems, List<UndoSystem.Action> undoMeshInSpacesCreateDelete )
			{
				if( meshInSpaceItems.Length != 2 )
					return;

				BoolActionExecute( meshInSpaceItems, undoMeshInSpacesCreateDelete, actionContext.DocumentWindow?.Document2, boolAction );
			}

			ExecuteAction( actionContext, Execute, true );
		}

		internal/*obfuscator*/ static Vector3F ToVector3F( Internal.Net3dBool.Vector3 v ) => new Vector3F( (float)v.x, (float)v.y, (float)v.z );

		internal/*obfuscator*/ static Internal.Net3dBool.Vector3 ToNet3DBoolVector3( Vector3F v ) => new Internal.Net3dBool.Vector3( v.X, v.Y, v.Z );

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

		//The data are extracted from Mesh.ExtractedData.
		//Хотя можно взять из MeshGeometry.Vertices,Indices, но там проиндексировано вместе с другими элементами(normals,...).
		static List<(Vector3F[] positions, int[] indices)> GetData( MeshInSpace meshInSpace )
		{
			var ret = new List<(Vector3F[] positions, int[] indices)>();

			var structure = meshInSpace.Mesh.Value.ExtractData();
			if( structure != null )
			{
				//extract old positions
				var oldPositions = new Vector3F[ structure.MeshGeometries.Length ][];
				for( int geomIndex = 0; geomIndex < structure.MeshGeometries.Length; geomIndex++ )
				{
					var vs = new BuilderMeshData.MeshGeometryFormat( structure.MeshGeometries[ geomIndex ].VertexStructure );
					oldPositions[ geomIndex ] = new Vector3F[ structure.MeshGeometries[ geomIndex ].Vertices.Length / vs.vertexSize ];

					unsafe
					{
						fixed( byte* ptr = structure.MeshGeometries[ geomIndex ].Vertices )
						{
							for( int i = 0; i < oldPositions[ geomIndex ].Length; i++ )
								oldPositions[ geomIndex ][ i ] = *(Vector3F*)( ptr + i * vs.vertexSize + vs.positionOffset );
						}
					}
				}
				//------------

				var geomTriangles = new List<Mesh.StructureClass.FaceVertex>[ structure.MeshGeometries.Length ];
				for( int i = 0; i < structure.MeshGeometries.Length; i++ )
					geomTriangles[ i ] = new List<Mesh.StructureClass.FaceVertex>();
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
			}

			return ret;
		}

		/*
		// Недостаток - извлекает из geometry, там проиндексировано не только по Position, но и по другим (normal,...)
		static List<(Vector3F[] positions, int[] indices)> GetData2( MeshInSpace ms )
		{
			var geoms = ms.Mesh.Value.GetComponents<MeshGeometry>();
			var ret = new List<(Vector3F[] vertices, int[] indices)>();
			foreach( var g in geoms )
			{
				int[] indices = null;
				VertexElement[] vertexStructure = null;
				byte[] verticesData = null;
				if( g is MeshGeometry_Procedural proc )
				{
					Material material = null;
					Mesh.StructureClass structure = null;
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

		/////////////////////////////////////////

		public static void SetColorGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object && actionContext.GetSelectedMesh().mesh != null || actionContext.Selection.VertexCount != 0 || actionContext.Selection.EdgeCount != 0 || actionContext.Selection.FaceCount != 0 )
			{
				context.Enabled = true;
			}
		}

		public static void SetColor( BuilderActionContext actionContext, ColorValue color )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object )
			{
				var document = actionContext.DocumentWindow.Document2;
				var undo = new List<UndoSystem.Action>();
				var undoLocked = false;

				foreach( var meshInSpace in actionContext.GetSelectedMeshInSpaces() )
				{
					//set Color property
					{
						var property = (Metadata.Property)meshInSpace.MetadataGetMemberBySignature( "property:" + nameof( MeshInSpace.Color ) );
						undo.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshInSpace, property, meshInSpace.Color ) ) );

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
								foreach( var geom in mesh.GetComponents<MeshGeometry>() )
								{
									if( !geom.Material.ReferenceSpecified && geom.Material.Value == null )
										BuilderOneMeshActions.SetMaterialForGeometry( geom, referenceToMaterial, undo, ref undoLocked );
								}
							}
						}
					}
				}

				var undoMultiAction = new UndoMultiAction();
				undoMultiAction.AddActions( undo );
				document.CommitUndoAction( undoMultiAction );
			}
			else
				BuilderOneMeshActions.SetVertexColor( actionContext, color );
		}

		public static ColorValue? GetInitialColor( BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object )
			{
				var msAr = actionContext.GetSelectedMeshInSpaces();
				if( msAr.Length != 0 )
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
				return BuilderOneMeshActions.GetInitialColor( actionContext );
		}

		/////////////////////////////////////////

		public static void MirrorObjectsGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object && actionContext.GetSelectedMesh().mesh != null )
				context.Enabled = true;
		}

		static void Mirror( Mesh mesh, int axis, List<UndoSystem.Action> undoMeshChanges, bool undoMeshChangesLocked )
		{
			var oldStructure = mesh.Structure;

			//build structure
			if( mesh.Structure == null )
				mesh.BuildStructure();

			var extractedData = mesh.ExtractData();

			for( int geometryIndex = 0; geometryIndex < extractedData.MeshGeometries.Length; geometryIndex++ )
			{
				var geometry = extractedData.MeshGeometries[ geometryIndex ];

				geometry.Vertices = (byte[])geometry.Vertices.Clone();
				geometry.Indices = (int[])geometry.Indices.Clone();

				var format = new BuilderMeshData.MeshGeometryFormat( geometry.VertexStructure );
				int vertexCount = geometry.Vertices.Length / format.vertexSize;

				unsafe
				{
					fixed( byte* ptr = geometry.Vertices )
					{
						for( int i = 0; i < vertexCount; i++ )
						{
							byte* vertexPtr = ptr + i * format.vertexSize;

							var pos = *(Vector3F*)( vertexPtr + format.positionOffset );

							Vector3F normal = Vector3F.Zero;
							if( format.normalOffset != -1 )
							{
								if( format.normalFormat == VertexElementType.Float3 )
									normal = *(Vector3F*)( vertexPtr + format.normalOffset );
								else if( format.normalFormat == VertexElementType.Half3 )
									normal = *(Vector3H*)( vertexPtr + format.normalOffset );
							}

							Vector4F tangent = Vector4F.Zero;
							if( format.tangentOffset != -1 )
							{
								if( format.tangentFormat == VertexElementType.Float4 )
									tangent = *(Vector4F*)( vertexPtr + format.tangentOffset );
								else if( format.tangentFormat == VertexElementType.Half4 )
									tangent = *(Vector4H*)( vertexPtr + format.tangentOffset );
							}

							switch( axis )
							{
							case 0:
								pos.X = -pos.X;
								normal.X = -normal.X;
								tangent.X = -tangent.X;
								break;

							case 1:
								pos.Y = -pos.Y;
								normal.Y = -normal.Y;
								tangent.Y = -tangent.Y;
								break;

							case 2:
								pos.Z = -pos.Z;
								normal.Z = -normal.Z;
								tangent.Z = -tangent.Z;
								break;

							}

							*(Vector3F*)( vertexPtr + format.positionOffset ) = pos;

							if( format.normalOffset != -1 )
							{
								if( format.normalFormat == VertexElementType.Float3 )
									*(Vector3F*)( vertexPtr + format.normalOffset ) = normal;
								else if( format.normalFormat == VertexElementType.Half3 )
									*(Vector3H*)( vertexPtr + format.normalOffset ) = normal;
							}

							if( format.tangentOffset != -1 )
							{
								if( format.tangentFormat == VertexElementType.Float4 )
									*(Vector4F*)( vertexPtr + format.tangentOffset ) = tangent;
								else if( format.tangentFormat == VertexElementType.Half4 )
									*(Vector4H*)( vertexPtr + format.tangentOffset ) = tangent;
							}



							//ref Vector3F posRef = ref *(Vector3F*)( vertexPtr + format.positionOffset );
							//ref Vector3F normalRef = ref *(Vector3F*)( vertexPtr + format.normalOffset );
							//ref Vector4F tangentRef = ref *(Vector4F*)( vertexPtr + format.tangentOffset );

							//switch( axis )
							//{
							//case 0:
							//	posRef.X = -posRef.X;
							//	if( format.normalOffset != -1 )
							//		normalRef.X = -normalRef.X;
							//	if( format.tangentOffset != -1 )
							//		tangentRef.X = -tangentRef.X;
							//	break;

							//case 1:
							//	posRef.Y = -posRef.Y;
							//	if( format.normalOffset != -1 )
							//		normalRef.Y = -normalRef.Y;
							//	if( format.tangentOffset != -1 )
							//		tangentRef.Y = -tangentRef.Y;
							//	break;

							//case 2:
							//	posRef.Z = -posRef.Z;
							//	if( format.normalOffset != -1 )
							//		normalRef.Z = -normalRef.Z;
							//	if( format.tangentOffset != -1 )
							//		tangentRef.Z = -tangentRef.Z;
							//	break;
							//}



						}
					}
				}

				//flip triangles
				for( int i = 0; i < geometry.Indices.Length; i += 3 )
				{
					int temp = geometry.Indices[ i + 1 ];
					geometry.Indices[ i + 1 ] = geometry.Indices[ i + 2 ];
					geometry.Indices[ i + 2 ] = temp;
				}

				//flip faces
				if( extractedData.Structure != null )
				{
					foreach( var face in extractedData.Structure.Faces )
					{
						for( int i = 0; i < face.Triangles.Length; i += 3 )
						{
							var temp = face.Triangles[ i + 1 ];
							face.Triangles[ i + 1 ] = face.Triangles[ i + 2 ];
							face.Triangles[ i + 2 ] = temp;
						}
					}
				}
			}

			//////////////////////////////////////////////

			//update mesh geometries
			var meshGeometries = mesh.GetComponents<MeshGeometry>();
			for( int i = 0; i < meshGeometries.Length; i++ )
			{
				var meshGeometry = meshGeometries[ i ];

				if( !undoMeshChangesLocked )
				{
					var property = (Metadata.Property)meshGeometry.MetadataGetMemberBySignature( "property:" + nameof( MeshGeometry.Vertices ) );
					undoMeshChanges.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshGeometry, property, meshGeometry.Vertices ) ) );

					var propertyIndices = (Metadata.Property)meshGeometry.MetadataGetMemberBySignature( "property:" + nameof( MeshGeometry.Indices ) );
					undoMeshChanges.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( meshGeometry, propertyIndices, meshGeometry.Indices ) ) );
				}
				meshGeometry.Vertices = extractedData.MeshGeometries[ i ].Vertices;
				meshGeometry.Indices = extractedData.MeshGeometries[ i ].Indices;
			}

			//update structure
			if( !undoMeshChangesLocked )
			{
				var property = (Metadata.Property)mesh.MetadataGetMemberBySignature( "property:" + nameof( Mesh.Structure ) );
				undoMeshChanges.Add( new UndoActionPropertiesChange( new UndoActionPropertiesChange.Item( mesh, property, oldStructure ) ) );
			}
			mesh.Structure = extractedData.Structure;
		}

		public static void MirrorObjects( BuilderActionContext actionContext, int axis )
		{
			void Execute( MeshInSpaceItem[] meshInSpaceItems, List<UndoSystem.Action> undoMeshInSpacesCreateDelete )
			{
				foreach( var meshInSpaceItem in meshInSpaceItems )
				{
					var meshInSpace = meshInSpaceItem.meshInSpace;
					var mesh = meshInSpace.Mesh.Value;
					if( mesh != null )
					{
						BuilderCommonFunctions.ConvertProceduralMeshGeometries( actionContext.DocumentWindow.Document2, mesh, meshInSpaceItem.undoMeshChanges, ref meshInSpaceItem.undoMeshChangesLocked );

						Mirror( mesh, axis, meshInSpaceItem.undoMeshChanges, meshInSpaceItem.undoMeshChangesLocked );
					}
				}
			}

			ExecuteAction( actionContext, Execute, false );
		}

		/////////////////////////////////////////

		public static void AddPaintLayerGetState( EditorActionGetStateContext context, BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object && actionContext.GetSelectedMeshInSpaces().Length > 0 )
				context.Enabled = true;
		}

		public static void AddPaintLayer( BuilderActionContext actionContext )
		{
			if( actionContext.SelectionMode == BuilderSelectionMode.Object )
			{
				var meshesInSpace = actionContext.GetSelectedMeshInSpaces();
				if( meshesInSpace.Length != 0 )
				{
					var newObjects = new List<Component>();

					foreach( var meshInSpace in meshesInSpace )
					{
						var modifier = meshInSpace.CreateComponent<PaintLayer>( enabled: false );
						modifier.Name = BuilderCommonFunctions.GetUniqueFriendlyName( modifier );
						modifier.Enabled = true;

						newObjects.Add( modifier );
					}

					actionContext.DocumentWindow.Focus();

					//undo
					var document = actionContext.DocumentWindow.Document2;
					var action = new UndoActionComponentCreateDelete( document, newObjects, true );
					document.CommitUndoAction( action );
					actionContext.DocumentWindow.SelectObjects( newObjects.ToArray() );
				}
			}
		}
	}
}
#endif