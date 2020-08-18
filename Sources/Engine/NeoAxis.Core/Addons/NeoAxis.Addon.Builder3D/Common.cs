// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.Text;
using ComponentFactory.Krypton.Toolkit;
using NeoAxis.Editor;

namespace NeoAxis.Addon.Builder3D
{
	public enum SelectionMode
	{
		Object, //This is the normal scene editor mode, enabled by default.
		Vertex, //In this mode you can select the vertices, move them, perform any editing actions.
		Edge, // In this mode you can select the edges, perform any editing actions.
		Face // In this mode you can select the faces, perform any editing actions.
	}

	public class Selection
	{
		public SelectionMode SelectionMode;
		public bool Changed;
		int[] vertices, edges, faces;
		static readonly int[] empty = new int[ 0 ];

		public int[] Vertices
		{
			get => vertices ?? empty;
			set
			{
				vertices = value;
				Changed = true;
			}
		}
		public int[] Edges
		{
			get => edges ?? empty;
			set
			{
				edges = value;
				Changed = true;
			}
		}
		public int[] Faces
		{
			get => faces ?? empty;
			set
			{
				faces = value;
				Changed = true;
			}
		}

		public int VertexCount => SelectionMode == SelectionMode.Vertex && vertices != null ? vertices.Length : 0;
		public int EdgeCount => SelectionMode == SelectionMode.Edge && edges != null ? edges.Length : 0;
		public int FaceCount => SelectionMode == SelectionMode.Face && faces != null ? faces.Length : 0;

		public void ClearSelection()
		{
			Changed = 0 < VertexCount || 0 < EdgeCount || 0 < FaceCount;
			Vertices = null;
			Edges = null;
			Faces = null;
		}
	}

	public struct ActionContext
	{
		public ActionContext( EditorAction.ClickContext context )
		{
			DocumentWindow = context.ObjectsInFocus.DocumentWindow;
			ObjectsInFocus = context.ObjectsInFocus.Objects;
			BuilderWorkareaMode = BuilderWorkareaMode.GetWorkareaMode( DocumentWindow );
			Selection = BuilderWorkareaMode.GetSelection( DocumentWindow );
		}
		public ActionContext( EditorAction.GetStateContext context )
		{
			DocumentWindow = context.ObjectsInFocus.DocumentWindow;
			ObjectsInFocus = context.ObjectsInFocus.Objects;
			BuilderWorkareaMode = BuilderWorkareaMode.GetWorkareaMode( DocumentWindow );
			Selection = BuilderWorkareaMode.GetSelection( DocumentWindow );
		}
		public ActionContext( DocumentWindow documentWindow )
		{
			DocumentWindow = documentWindow;
			ObjectsInFocus = null;
			BuilderWorkareaMode = BuilderWorkareaMode.GetWorkareaMode( documentWindow );
			Selection = BuilderWorkareaMode.GetSelection( DocumentWindow );
		}

		public ActionContext( DocumentWindow documentWindow, object[] objectsInFocus )
		{
			DocumentWindow = documentWindow;
			ObjectsInFocus = objectsInFocus;
			BuilderWorkareaMode = BuilderWorkareaMode.GetWorkareaMode( documentWindow );
			Selection = BuilderWorkareaMode.GetSelection( DocumentWindow );
		}
		public readonly DocumentWindow DocumentWindow;
		public readonly object[] ObjectsInFocus;
		public readonly BuilderWorkareaMode BuilderWorkareaMode;
		public readonly Selection Selection;

		public (Component_MeshInSpace meshInSpace, Component_Mesh mesh) GetSelectedMesh()
		{
			return BuilderWorkareaMode.GetSelectedMesh( DocumentWindow, ObjectsInFocus );
		}

		//Не возвращает ничего если режим выделения не Object
		//onlyWhenAllHaveStructure==true - не возвращает ничего если есть без structure.
		//??? Или лучше возвращать только те где есть структура, но тогда пользователю может быть не понятно почему часть отбрасывается. Либо возвращать все, выдается ошибка "Structure do not exist".  Пользователю было бы понятнее если объекты без Structure выделять другим цветом
		public Component_MeshInSpace[] GetSelectedMeshInSpaceArray( bool onlyWhenAllHaveStructure = false )
		{
			var result = new List<Component_MeshInSpace>();

			//var selectedObjects1 = EditorAPI.SelectedDocumentWindow?.SelectedObjects;
			var selectedObjects = DocumentWindow?.SelectedObjects;
			if( selectedObjects == null )
				return new Component_MeshInSpace[ 0 ];

			foreach( var obj in selectedObjects )
			{
				if( obj is Component_MeshInSpace meshInSpace )
				{
					if( onlyWhenAllHaveStructure && !HasStructure( meshInSpace ) )
						return new Component_MeshInSpace[ 0 ];
					result.Add( meshInSpace );
				}
			}

			return result.ToArray();
		}

		static bool HasStructure( Component_MeshInSpace ms ) => ms.Mesh.Value?.Structure != null;

		public SelectionMode SelectionMode => BuilderWorkareaMode?.selectionMode ?? SelectionMode.Object;

		public void SelectMeshesInSpace( params Component_MeshInSpace[] meshesInSpace ) => DocumentWindow.SelectObjects( meshesInSpace );

		public void ActionEnd()
		{
			if( !Selection.Changed )
				return;

			switch( SelectionMode )
			{
			case SelectionMode.Vertex:
				BuilderWorkareaMode.SelectVertices( Selection.Vertices ?? new int[ 0 ] );
				break;
			case SelectionMode.Edge:
				BuilderWorkareaMode.SelectEdges( Selection.Edges ?? new int[ 0 ] );
				break;
			case SelectionMode.Face:
				BuilderWorkareaMode.SelectFaces( Selection.Faces ?? new int[ 0 ] );
				break;
			}
			Selection.Changed = false;
		}
	}
}
#endif