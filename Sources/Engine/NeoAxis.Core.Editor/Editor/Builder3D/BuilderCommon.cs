// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public enum BuilderSelectionMode
	{
		Object, // This is the normal scene editor mode, enabled by default.
		Vertex, // In this mode you can select the vertices, move them, perform any editing actions.
		Edge, // In this mode you can select the edges, perform any editing actions.
		Face // In this mode you can select the faces, perform any editing actions.
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class BuilderSelection
	{
		public BuilderSelectionMode SelectionMode;
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

		public int VertexCount => SelectionMode == BuilderSelectionMode.Vertex && vertices != null ? vertices.Length : 0;
		public int EdgeCount => SelectionMode == BuilderSelectionMode.Edge && edges != null ? edges.Length : 0;
		public int FaceCount => SelectionMode == BuilderSelectionMode.Face && faces != null ? faces.Length : 0;

		public void ClearSelection()
		{
			Changed = 0 < VertexCount || 0 < EdgeCount || 0 < FaceCount;
			Vertices = null;
			Edges = null;
			Faces = null;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public struct BuilderActionContext
	{
		public readonly DocumentWindow DocumentWindow;
		public readonly object[] ObjectsInFocus;
		public readonly BuilderWorkareaMode BuilderWorkareaMode;
		public readonly BuilderSelection Selection;

		//

		public BuilderActionContext( EditorActionClickContext context )
		{
			DocumentWindow = (DocumentWindow)context.ObjectsInFocus.DocumentWindow;
			ObjectsInFocus = context.ObjectsInFocus.Objects;
			BuilderWorkareaMode = BuilderWorkareaMode.GetWorkareaMode( DocumentWindow );
			Selection = BuilderWorkareaMode.GetSelection( DocumentWindow );
		}

		public BuilderActionContext( EditorActionGetStateContext context )
		{
			DocumentWindow = (DocumentWindow)context.ObjectsInFocus.DocumentWindow;
			ObjectsInFocus = context.ObjectsInFocus.Objects;
			BuilderWorkareaMode = BuilderWorkareaMode.GetWorkareaMode( DocumentWindow );
			Selection = BuilderWorkareaMode.GetSelection( DocumentWindow );
		}

		public BuilderActionContext( DocumentWindow documentWindow )
		{
			DocumentWindow = documentWindow;
			ObjectsInFocus = null;
			BuilderWorkareaMode = BuilderWorkareaMode.GetWorkareaMode( documentWindow );
			Selection = BuilderWorkareaMode.GetSelection( DocumentWindow );
		}

		public BuilderActionContext( DocumentWindow documentWindow, object[] objectsInFocus )
		{
			DocumentWindow = documentWindow;
			ObjectsInFocus = objectsInFocus;
			BuilderWorkareaMode = BuilderWorkareaMode.GetWorkareaMode( documentWindow );
			Selection = BuilderWorkareaMode.GetSelection( DocumentWindow );
		}

		public (MeshInSpace meshInSpace, Mesh mesh) GetSelectedMesh()
		{
			return BuilderWorkareaMode.GetSelectedMesh( DocumentWindow, ObjectsInFocus );
		}

		//works only for Object selection mode
		public MeshInSpace[] GetSelectedMeshInSpaces()
		{
			var selectedObjects = DocumentWindow?.SelectedObjects;
			if( selectedObjects != null )
			{
				List<MeshInSpace> result = null;
				foreach( var obj in selectedObjects )
				{
					if( obj is MeshInSpace meshInSpace )
					{
						if( result == null )
							result = new List<MeshInSpace>();
						result.Add( meshInSpace );
					}
				}
				if( result != null )
					return result.ToArray();
			}

			return Array.Empty<MeshInSpace>();
		}

		public MeshGeometry[] GetSelectedMeshGeometries()
		{
			var selectedObjects = DocumentWindow?.SelectedObjects;
			if( selectedObjects != null )
			{
				List<MeshGeometry> result = null;
				foreach( var obj in selectedObjects )
				{
					if( obj is MeshGeometry meshGeometry )
					{
						if( result == null )
							result = new List<MeshGeometry>();
						result.Add( meshGeometry );
					}
				}
				if( result != null )
					return result.ToArray();
			}

			return Array.Empty<MeshGeometry>();
		}

		public BuilderSelectionMode SelectionMode
		{
			get { return BuilderWorkareaMode?.selectionMode ?? BuilderSelectionMode.Object; }
		}

		public void SelectMeshesInSpace( params MeshInSpace[] meshesInSpace )
		{
			DocumentWindow.SelectObjects( meshesInSpace );
		}

		public void ActionEnd()
		{
			if( !Selection.Changed )
				return;

			switch( SelectionMode )
			{
			case BuilderSelectionMode.Vertex:
				BuilderWorkareaMode.SelectVertices( Selection.Vertices ?? new int[ 0 ] );
				break;
			case BuilderSelectionMode.Edge:
				BuilderWorkareaMode.SelectEdges( Selection.Edges ?? new int[ 0 ] );
				break;
			case BuilderSelectionMode.Face:
				BuilderWorkareaMode.SelectFaces( Selection.Faces ?? new int[ 0 ] );
				break;
			}
			Selection.Changed = false;
		}
	}
}
#endif