// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NeoAxis.Editor;

namespace NeoAxis.Addon.Builder3D
{
	public class BuilderWorkareaMode : Component_Scene_DocumentWindow.WorkareaModeClass_Scene
	{
		public SelectionMode selectionMode;
		//object[] selectedObjectsToRestore;

		//selected mesh in space
		Component_MeshInSpace meshInSpaceToEdit;
		//mesh data
		internal MeshData meshToEditData;
		readonly List<Vertex> meshVertices = new List<Vertex>();
		List<Edge> meshEdges = new List<Edge>();
		List<Face> meshFaces = new List<Face>();
		internal Component_Mesh.ExtractedStructure meshExtractedStructure;

		bool transformToolModifing;

		bool needUpdateMeshData;

		CalculateVisiblePointsData calculateVisiblePointsDataCache;
		Task calculateVisiblePointsTask;

		CalculateVisibleEdgesData calculateVisibleEdgesDataCache;
		Task calculateVisibleEdgesTask;

		/////////////////////////////////////////

		public class Vertex
		{
			public int Index;

			public Vector3 Position;
			public Quaternion Rotation = Quaternion.Identity;
			public Vector3 Scale = Vector3.One;

			public Rectangle lastLabelScreenRectangle;
		}

		/////////////////////////////////////////

		public class Edge
		{
			public int Index;


			public Vector3 Position;
			public Quaternion Rotation = Quaternion.Identity;
			public Vector3 Scale = Vector3.One;

			//!!!!
			//public Rectangle lastLabelScreenRectangle;
		}

		/////////////////////////////////////////

		public class Face
		{
			public int Index;


			public Vector3 Position;
			public Quaternion Rotation = Quaternion.Identity;
			public Vector3 Scale = Vector3.One;

			//!!!!
			//public Rectangle lastLabelScreenRectangle;
		}

		/////////////////////////////////////////

		class CalculateVisiblePointsData
		{
			public MeshData meshToEditData;

			public Matrix4 meshInSpaceTransformInvert;
			public Component_Mesh.CompiledData meshCompiledData_OnlyToCompare;
			public bool meshInSpaceTwoSided;
			public Vector3F[] meshExtractedVerticesPositions;
			public int[] meshExtractedIndices;
			public MeshRayTestOptimized meshRayTestOptimized;

			public List<Vertex> meshVertices;
			public Vector3 viewportCameraSettingsPosition;

			public ESet<Vertex> visiblePoints;
		}

		/////////////////////////////////////////

		class CalculateVisibleEdgesData
		{
			public MeshData meshToEditData;

			public List<Vertex> meshVertices;
			public List<Edge> meshEdges;
			public Component_Mesh.ExtractedStructure meshExtractedStructure;
			public Vector3 viewportCameraSettingsPosition;

			public ESet<int> visibleEdges;
		}

		/////////////////////////////////////////

		public BuilderWorkareaMode( Component_Scene_DocumentWindow documentWindow )
			: base( documentWindow )
		{
			////save selected objects
			//selectedObjectsToRestore = (object[])DocumentWindow.SelectedObjects.Clone();

			if( GetEditableSelectedObject( DocumentWindow.SelectedObjects, out var meshInSpace ) )//, out var mesh ) )
			{
				meshInSpaceToEdit = meshInSpace;
				//meshToEdit = mesh;
				UpdateMeshData();

				//clear selection
				DocumentWindow.SelectObjects( null );
			}

			DocumentWindow.Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;
			DocumentWindow.SelectedObjectsChanged += DocumentWindow_SelectedObjectsChanged;
		}

		protected override void OnDestroy()
		{
			DocumentWindow.Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;
			DocumentWindow.SelectedObjectsChanged -= DocumentWindow_SelectedObjectsChanged;

			//restore selected objects
			if( !EditorAPI.ClosingApplication )
			{
				if( meshInSpaceToEdit != null && meshInSpaceToEdit.EnabledInHierarchy )
					DocumentWindow.SelectObjects( new object[] { meshInSpaceToEdit } );
				else
				{
					if( DocumentWindow.SelectedObjects.FirstOrDefault( obj => obj is Vertex || obj is Edge || obj is Face ) != null )
						DocumentWindow.SelectObjects( null );
				}

				//var objectsToSelect = new List<object>();
				//if( selectedObjectsToRestore != null )
				//{
				//	foreach( var obj in selectedObjectsToRestore )
				//	{
				//		var objectInSpace = obj as Component_ObjectInSpace;
				//		if( objectInSpace != null && !objectInSpace.Disposed )
				//			objectsToSelect.Add( objectInSpace );
				//	}
				//}
				//DocumentWindow.SelectObjects( objectsToSelect );
			}
		}

		protected override bool OnMouseDown( Viewport viewport, EMouseButtons button )
		{
			return false;
		}

		protected override bool OnMouseUp( Viewport viewport, EMouseButtons button )
		{
			return false;
		}

		protected override void OnTick( Viewport viewport, double delta )
		{
			//CheckDeferredCacheClear();

			var meshToEdit = GetMeshToEdit();
			if( meshToEdit != null && !meshToEdit.EnabledInHierarchy )
				needUpdateMeshData = true;

			if( needUpdateMeshData )
				UpdateMeshData();

			//unselect deleted selected objects
			{
				bool clear = false;

				foreach( var obj in DocumentWindow.SelectedObjects )
				{
					var vertex = obj as Vertex;
					if( vertex != null && !meshVertices.Contains( vertex ) )
					{
						clear = true;
						break;
					}

					var edge = obj as Edge;
					if( edge != null && !meshEdges.Contains( edge ) )
					{
						clear = true;
						break;
					}

					var face = obj as Face;
					if( face != null && !meshFaces.Contains( face ) )
					{
						clear = true;
						break;
					}
				}

				if( clear )
					DocumentWindow.SelectObjects( null );
			}

			CalculateVisiblePointsTaskUpdate( viewport );
			CalculateVisibleEdgesTaskUpdate( viewport );
		}

		static bool MeshRayCast( CalculateVisiblePointsData data, Ray ray, out double scale, out int triangleIndex )
		{
			scale = 0;
			triangleIndex = -1;

			RayF rayF = ray.ToRayF();

			//octree optimized cached
			var result = data.meshRayTestOptimized.RayTest( rayF, MeshRayTestOptimized.Mode.OneClosest, data.meshInSpaceTwoSided );
			if( result.Length > 0 )
			{
				var item = result[ 0 ];
				scale = item.Scale;
				triangleIndex = item.TriangleIndex;
				return true;
			}

			return false;
		}

		static bool MeshInSpaceToEditRayCast( CalculateVisiblePointsData data, Ray ray, out double scale, out int triangleIndex )
		{
			Ray localRay = data.meshInSpaceTransformInvert * ray;

			if( MeshRayCast( data, localRay, out scale, out triangleIndex ) )
				return true;

			scale = 0;
			triangleIndex = -1;
			return false;
		}

		static bool CheckPointVisibility( CalculateVisiblePointsData data, Vector3 point )
		{
			var offsets = new Vector3[] {
				Vector3.Zero,
				new Vector3( -0.05, 0,0 ), new Vector3( 0.05, 0,0 ),
				new Vector3( 0, -0.05, 0 ), new Vector3( 0, 0.05, 0 ) ,
				new Vector3( 0, 0, -0.05 ), new Vector3( 0, 0, 0.05 ) };

			foreach( var offset in offsets )
			{
				var direction = ( point + offset ) - data.viewportCameraSettingsPosition;
				var length = direction.Normalize();
				direction *= length - 0.05;

				var ray = new Ray( data.viewportCameraSettingsPosition, direction );
				if( !MeshInSpaceToEditRayCast( data, ray, out _, out _ ) )
					return true;
			}

			return false;
		}

		static void CalculateVisiblePoints( object data2 )
		{
			var data = (CalculateVisiblePointsData)data2;

			data.meshRayTestOptimized = new MeshRayTestOptimized( data.meshExtractedVerticesPositions, data.meshExtractedIndices );

			data.visiblePoints = new ESet<Vertex>( data.meshVertices.Count );
			foreach( var vertex in data.meshVertices )
			{
				if( CheckPointVisibility( data, vertex.Position ) )
					data.visiblePoints.Add( vertex );
			}
		}

		ESet<Vertex> GetVisiblePoints( Viewport viewport )
		{
			if( calculateVisiblePointsDataCache != null && calculateVisiblePointsDataCache.meshToEditData == meshToEditData )
				return calculateVisiblePointsDataCache.visiblePoints;
			return new ESet<Vertex>();
		}

		void DrawVertexLabel( Viewport viewport, Vertex vertex, List<CanvasRenderer.TriangleVertex> triangles )
		{
			var context = viewport.RenderingContext.objectInSpaceRenderingContext;
			//var viewport = context.viewport;
			//var t = Transform.Value;
			//var pos = t.Position;

			if( viewport.CameraSettings.ProjectToScreenCoordinates( vertex.Position, out Vector2 screenPosition ) )
			{
				if( new Rectangle( 0, 0, 1, 1 ).Contains( ref screenPosition ) )
				{
					//!!!!в конфиг
					//!!!!может указывать в пикселях? или вертикальным размером?
					//!!!!графиком настраивать
					//!!!!может картинку рисовать, может разным цветом
					Vector2 maxSize = new Vector2( 20, 20 );
					Vector2 minSize = new Vector2( 5, 5 );
					double maxDistance = 100;

					double distance = ( vertex.Position - viewport.CameraSettings.Position ).Length();
					if( distance < maxDistance )
					{
						Vector2 sizeInPixels = Vector2.Lerp( maxSize, minSize, distance / maxDistance );
						Vector2 screenSize = sizeInPixels / viewport.SizeInPixels.ToVector2();

						ColorValue color;
						double sizeMultiplier;
						if( context.selectedObjects.Contains( vertex ) )
						{
							color = ProjectSettings.Get.SelectedColor;
							sizeMultiplier = 0.5;
						}
						else if( context.canSelectObjects.Contains( vertex ) )
						{
							color = ProjectSettings.Get.CanSelectColor;
							sizeMultiplier = 0.5;
						}
						else
						{
							color = ProjectSettings.Get.ScreenLabelColor;
							sizeMultiplier = 0.4;
						}

						Rectangle rect = new Rectangle( screenPosition - screenSize * .5, screenPosition + screenSize * .5 );
						var visualRect = new Rectangle( screenPosition - screenSize * .5 * sizeMultiplier, screenPosition + screenSize * .5 * sizeMultiplier ).ToRectangleF();

						var v0 = new CanvasRenderer.TriangleVertex( visualRect.LeftTop, color, new Vector2F( 0, 0 ) );
						var v1 = new CanvasRenderer.TriangleVertex( visualRect.RightTop, color, new Vector2F( 1, 0 ) );
						var v2 = new CanvasRenderer.TriangleVertex( visualRect.RightBottom, color, new Vector2F( 1, 1 ) );
						var v3 = new CanvasRenderer.TriangleVertex( visualRect.LeftBottom, color, new Vector2F( 0, 1 ) );
						triangles.Add( v0 );
						triangles.Add( v1 );
						triangles.Add( v2 );
						triangles.Add( v2 );
						triangles.Add( v3 );
						triangles.Add( v0 );
						//if( texture == null )
						//	texture = ResourceManager.LoadResource<Component_Image>( "Base\\UI\\Images\\Circle.png" );
						//viewport.CanvasRenderer.AddQuad( visualRect, new RectangleF( 0, 0, 1, 1 ), texture, color, true );

						vertex.lastLabelScreenRectangle = rect;
					}
				}
			}
		}

		void DrawVertices( Viewport viewport )
		{
			var triangles = new List<CanvasRenderer.TriangleVertex>( 256 );

			var visiblePoints = GetVisiblePoints( viewport );

			for( var i = 0; i < meshVertices.Count; i++ )
			{
				var vertex = meshVertices[ i ];
				vertex.lastLabelScreenRectangle = Rectangle.Cleared;

				if( visiblePoints.Contains( vertex ) )
					DrawVertexLabel( viewport, vertex, triangles );
			}

			if( triangles.Count != 0 )
			{
				var texture = ResourceManager.LoadResource<Component_Image>( "Base\\UI\\Images\\Circle.png" );
				viewport.CanvasRenderer.AddTriangles( triangles, texture, true );
			}
		}

		ESet<int> GetVisibleEdges( Viewport viewport )
		{
			if( calculateVisibleEdgesDataCache != null && calculateVisibleEdgesDataCache.meshToEditData == meshToEditData )
				return calculateVisibleEdgesDataCache.visibleEdges;
			return new ESet<int>();
		}

		static void CalculateVisibleEdges( object data2 )
		{
			var data = (CalculateVisibleEdgesData)data2;

			data.visibleEdges = new ESet<int>( data.meshEdges.Count );

			var meshEdgeByKey = new Dictionary<Vector2I, int>( data.meshEdges.Count );
			{
				foreach( var meshEdge in data.meshEdges )
				{
					if( meshEdge.Index < data.meshExtractedStructure.Structure.Edges.Length )
					{
						var edge = data.meshExtractedStructure.Structure.Edges[ meshEdge.Index ];

						int v1, v2;
						if( edge.Vertex1 > edge.Vertex2 )
						{
							v1 = edge.Vertex2;
							v2 = edge.Vertex1;
						}
						else
						{
							v1 = edge.Vertex1;
							v2 = edge.Vertex2;
						}
						var key = new Vector2I( v1, v2 );

						meshEdgeByKey[ key ] = meshEdge.Index;
					}
				}
			}

			foreach( var face in data.meshExtractedStructure.Structure.Faces )
			{
				for( int nTriangle = 0; nTriangle < face.Triangles.Length / 3; nTriangle++ )
				{
					var vertex0 = face.Triangles[ nTriangle * 3 + 0 ].Vertex;
					var vertex1 = face.Triangles[ nTriangle * 3 + 1 ].Vertex;
					var vertex2 = face.Triangles[ nTriangle * 3 + 2 ].Vertex;

					//!!!!double
					var p0 = data.meshVertices[ vertex0 ].Position.ToVector3F();
					var p1 = data.meshVertices[ vertex1 ].Position.ToVector3F();
					var p2 = data.meshVertices[ vertex2 ].Position.ToVector3F();

					var plane = Plane.FromPoints( p0, p1, p2 );
					if( plane.GetSide( data.viewportCameraSettingsPosition ) == Plane.Side.Positive )
					{

						void AddEdge( int vv1, int vv2 )
						{
							int v1, v2;
							if( vv1 > vv2 )
							{
								v1 = vv2;
								v2 = vv1;
							}
							else
							{
								v1 = vv1;
								v2 = vv2;
							}
							var key = new Vector2I( v1, v2 );

							if( meshEdgeByKey.TryGetValue( key, out var index ) )
								data.visibleEdges.AddWithCheckAlreadyContained( index );
						}

						AddEdge( vertex0, vertex1 );
						AddEdge( vertex1, vertex2 );
						AddEdge( vertex2, vertex0 );
					}
				}
			}
		}

		void DrawEdges( Viewport viewport )
		{
			var context = viewport.RenderingContext.objectInSpaceRenderingContext;
			var renderer = viewport.Simple3DRenderer;

			var lines = new List<Line3F>( meshVertices.Count * 4 );
			var linesCanSelect = new List<Line3F>( meshVertices.Count * 4 );
			var linesSelected = new List<Line3F>( meshVertices.Count * 4 );

			foreach( var index in GetVisibleEdges( viewport ) )
			{
				var edge = meshExtractedStructure.Structure.Edges[ index ];
				var meshEdge = meshEdges[ index ];

				//!!!!double
				var v1 = meshVertices[ edge.Vertex1 ].Position.ToVector3F();
				var v2 = meshVertices[ edge.Vertex2 ].Position.ToVector3F();
				var line = new Line3F( v1, v2 );

				lines.Add( line );

				if( context.selectedObjects.Contains( meshEdge ) )
					linesSelected.Add( line );
				else if( context.canSelectObjects.Contains( meshEdge ) )
					linesCanSelect.Add( line );
			}

			if( lines.Count != 0 )
			{
				renderer.SetColor( new ColorValue( 0, 0, 1 ) );
				foreach( var line in lines )
					renderer.AddLineThin( line );
			}

			if( linesCanSelect.Count != 0 )
			{
				var color = ProjectSettings.Get.CanSelectColor.Value;
				//color.Alpha *= 0.5f;
				renderer.SetColor( color );

				foreach( var line in linesCanSelect )
					renderer.AddLine( line );
			}

			if( linesSelected.Count != 0 )
			{
				var color = ProjectSettings.Get.SelectedColor.Value;
				//color.Alpha *= 0.5f;
				renderer.SetColor( color );

				foreach( var line in linesSelected )
					renderer.AddLine( line );
			}
		}

		void DrawFaces( Viewport viewport )
		{
			var context = viewport.RenderingContext.objectInSpaceRenderingContext;
			var renderer = viewport.Simple3DRenderer;

			var lines = new List<Line3F>( meshVertices.Count * 4 );
			//var triangles = new List<Vector3F>( meshVertices.Count * 4 );
			var trianglesCanSelect = new List<Vector3F>( meshVertices.Count * 4 );
			var trianglesSelected = new List<Vector3F>( meshVertices.Count * 4 );

			foreach( var meshFace in meshFaces )
			{
				if( meshFace.Index < meshExtractedStructure.Structure.Faces.Length )
				{
					var face = meshExtractedStructure.Structure.Faces[ meshFace.Index ];

					var edgeCounts = new Dictionary<Vector2I, int>( face.Triangles.Length );

					for( int nTriangle = 0; nTriangle < face.Triangles.Length / 3; nTriangle++ )
					{
						var vertex0 = face.Triangles[ nTriangle * 3 + 0 ].Vertex;
						var vertex1 = face.Triangles[ nTriangle * 3 + 1 ].Vertex;
						var vertex2 = face.Triangles[ nTriangle * 3 + 2 ].Vertex;

						//!!!!double
						var p0 = meshVertices[ vertex0 ].Position.ToVector3F();
						var p1 = meshVertices[ vertex1 ].Position.ToVector3F();
						var p2 = meshVertices[ vertex2 ].Position.ToVector3F();

						var plane = Plane.FromPoints( p0, p1, p2 );
						if( plane.GetSide( viewport.CameraSettings.Position ) == Plane.Side.Positive )
						{

							void AddEdge( int vv1, int vv2 )
							{
								int v1, v2;
								if( vv1 > vv2 )
								{
									v1 = vv2;
									v2 = vv1;
								}
								else
								{
									v1 = vv1;
									v2 = vv2;
								}
								var key = new Vector2I( v1, v2 );
								edgeCounts.TryGetValue( key, out var count );
								edgeCounts[ key ] = count + 1;
							}

							AddEdge( vertex0, vertex1 );
							AddEdge( vertex1, vertex2 );
							AddEdge( vertex2, vertex0 );
							//triangles.Add( p0 );
							//triangles.Add( p1 );
							//triangles.Add( p2 );

							if( context.selectedObjects.Contains( meshFace ) )
							{
								trianglesSelected.Add( p0 );
								trianglesSelected.Add( p1 );
								trianglesSelected.Add( p2 );
							}
							else if( context.canSelectObjects.Contains( meshFace ) )
							{
								trianglesCanSelect.Add( p0 );
								trianglesCanSelect.Add( p1 );
								trianglesCanSelect.Add( p2 );
							}
						}
					}

					foreach( var pair in edgeCounts )
					{
						if( pair.Value == 1 )
						{
							var edge = pair.Key;

							//!!!!double
							var vertex0 = meshVertices[ edge.X ].Position.ToVector3F();
							var vertex1 = meshVertices[ edge.Y ].Position.ToVector3F();
							lines.Add( new Line3F( vertex0, vertex1 ) );
						}
					}
				}
			}

			if( lines.Count != 0 )
			{
				renderer.SetColor( new ColorValue( 0, 0, 1 ) );
				foreach( var line in lines )
					renderer.AddLineThin( line );
			}
			//if( triangles.Count != 0 )
			//{
			//	renderer.SetColor( new ColorValue( 0, 0, 1 ) );
			//	renderer.AddTriangles( triangles, true, true );
			//}

			if( trianglesCanSelect.Count != 0 )
			{
				var color = ProjectSettings.Get.CanSelectColor.Value;
				color.Alpha *= 0.5f;
				renderer.SetColor( color );

				renderer.AddTriangles( trianglesCanSelect, false, true );
			}

			if( trianglesSelected.Count != 0 )
			{
				var color = ProjectSettings.Get.SelectedColor.Value;
				color.Alpha *= 0.5f;
				renderer.SetColor( color );

				renderer.AddTriangles( trianglesSelected, false, true );
			}
		}

		protected override void OnUpdateBeforeOutput( Viewport viewport )
		{
			if( meshToEditData != null )
			{
				//!!!!может грани и фейсы перед этим рисовать
				if( transformToolModifing )
					DrawMeshTransformToolModifing( viewport );

				//vertex mode
				if( selectionMode == SelectionMode.Vertex )
					DrawVertices( viewport );

				//edge mode
				if( selectionMode == SelectionMode.Edge && meshExtractedStructure != null )
					DrawEdges( viewport );

				//face mode
				if( selectionMode == SelectionMode.Face && meshExtractedStructure != null )
					DrawFaces( viewport );
			}
		}

		protected override void OnGetTextInfoCenterBottomCorner( List<string> lines )
		{
			base.OnGetTextInfoCenterBottomCorner( lines );

			var meshToEdit = GetMeshToEdit();
			if( meshToEdit != null )
				if( !CommonFunctions.CheckValidMesh( meshToEdit, out var error ) )
					lines.Add( error );
		}

		public override bool AllowSelectObjects
		{
			get { return true; }
		}

		public override bool AllowCreateObjectsByDrop
		{
			get { return true; }
		}

		public override bool AllowCreateObjectsByClick
		{
			get { return false; }
		}

		public override bool AllowCreateObjectsByBrush
		{
			get { return false; }
		}

		protected override bool OnGetObjectsToSelectByRectangle( Rectangle rectangle, ref List<object> objectsToSelect )
		{
			var viewport = DocumentWindow.Viewport;

			//vertices
			if( selectionMode == SelectionMode.Vertex )
			{
				foreach( var vertex in meshVertices )
				{
					if( !vertex.lastLabelScreenRectangle.IsCleared() )
					{
						if( viewport.CameraSettings.ProjectToScreenCoordinates( vertex.Position, out var screenPosition ) )
						{
							if( rectangle.Contains( screenPosition ) )
								objectsToSelect.Add( vertex );
						}
					}
				}
			}

			//edges
			if( selectionMode == SelectionMode.Edge && meshExtractedStructure != null )
			{
				foreach( var index in GetVisibleEdges( viewport ) )
				{
					var edge = meshExtractedStructure.Structure.Edges[ index ];
					var meshEdge = meshEdges[ index ];

					var v1 = meshVertices[ edge.Vertex1 ].Position;
					var v2 = meshVertices[ edge.Vertex2 ].Position;
					var center = ( v1 + v2 ) / 2;

					if( viewport.CameraSettings.ProjectToScreenCoordinates( center, out var screenPosition ) )
					{
						if( rectangle.Contains( screenPosition ) )
							objectsToSelect.Add( meshEdge );
					}
				}
			}

			//faces
			if( selectionMode == SelectionMode.Face && meshExtractedStructure != null )
			{
				foreach( var meshFace in meshFaces )
				{
					if( meshFace.Index < meshExtractedStructure.Structure.Faces.Length )
					{
						var face = meshExtractedStructure.Structure.Faces[ meshFace.Index ];

						for( int nTriangle = 0; nTriangle < face.Triangles.Length / 3; nTriangle++ )
						{
							var vertex0 = face.Triangles[ nTriangle * 3 + 0 ].Vertex;
							var vertex1 = face.Triangles[ nTriangle * 3 + 1 ].Vertex;
							var vertex2 = face.Triangles[ nTriangle * 3 + 2 ].Vertex;

							var p0 = meshVertices[ vertex0 ].Position;
							var p1 = meshVertices[ vertex1 ].Position;
							var p2 = meshVertices[ vertex2 ].Position;

							var plane = Plane.FromPoints( p0, p1, p2 );
							if( plane.GetSide( viewport.CameraSettings.Position ) == Plane.Side.Positive )
							{
								var center = ( p0 + p1 + p2 ) / 3;

								if( viewport.CameraSettings.ProjectToScreenCoordinates( center, out var screenPosition ) )
								{
									if( rectangle.Contains( screenPosition ) )
									{
										objectsToSelect.Add( meshFace );
										break;
									}
								}
							}
						}
					}
				}
			}

			return true;
		}

		static bool IsMouseNearLine( Viewport viewport, Vector3 start, Vector3 end )//, out Vector2 projectedScreenPointInPixels, out Radian projectedScreenAngle )
		{
			double selectNearPixels = 6 * EditorAPI.DPIScale;

			var projectedScreenPointInPixels = Vector2.Zero;
			//projectedScreenAngle = 0;

			Vector2 viewportSize = viewport.SizeInPixels.ToVector2();
			Vector2 mouseInPixels = viewport.MousePosition * viewportSize;

			Vector2 screenStart;
			if( !viewport.CameraSettings.ProjectToScreenCoordinates( start, out screenStart ) )
				return false;
			Vector2 screenEnd;
			if( !viewport.CameraSettings.ProjectToScreenCoordinates( end, out screenEnd ) )
				return false;

			Vector2 screenStartInPixels = screenStart * viewportSize;
			Vector2 screenEndInPixels = screenEnd * viewportSize;

			Rectangle rect = new Rectangle( screenStartInPixels );
			rect.Add( screenEndInPixels );
			rect.Expand( selectNearPixels );

			if( !rect.Contains( mouseInPixels ) )
				return false;

			projectedScreenPointInPixels = MathAlgorithms.ProjectPointToLine( screenStartInPixels, screenEndInPixels, mouseInPixels );

			double distance = ( mouseInPixels - projectedScreenPointInPixels ).Length();
			if( distance > selectNearPixels )
				return false;

			//Vector2 screenDiff = screenEndInPixels - screenStartInPixels;
			//projectedScreenAngle = Math.Atan2( screenDiff.Y, screenDiff.X );

			return true;
		}

		protected override bool OnGetMouseOverObjectToSelectByClick( Component_Scene_DocumentWindow.GetMouseOverObjectToSelectByClickContext context )
		{
			var viewport = DocumentWindow.Viewport;
			var mouse = viewport.MousePosition;

			double minDistance = double.MaxValue;

			//select another object in the scene
			DocumentWindow.GetMouseOverObjectInSpaceToSelectByClick( context );
			if( context.ResultObject != null )
			{
				if( meshInSpaceToEdit == context.ResultObject )
					context.ResultObject = null;
				else
				{
					//по идее не нужно показывать метки каких нельзя выделить

					//check object is editable by the Builder
					if( GetEditableSelectedObject( new object[] { context.ResultObject }, out var meshInSpace ) )//, out var mesh ) )
					{
						if( context.ResultPosition.HasValue )
							minDistance = ( context.ResultPosition.Value - viewport.CameraSettings.Position ).Length();
						else
							minDistance = 0;
					}
					else
						context.ResultObject = null;
				}
			}

			//vertex
			if( selectionMode == SelectionMode.Vertex )
			{
				foreach( var vertex in meshVertices )
				{
					if( viewport.CameraSettings.ProjectToScreenCoordinates( vertex.Position, out var screenPosition ) )
					{
						if( !vertex.lastLabelScreenRectangle.IsCleared() && vertex.lastLabelScreenRectangle.Contains( mouse ) )
						{
							var distance = ( vertex.Position - viewport.CameraSettings.Position ).Length();
							if( distance < minDistance )
							{
								minDistance = distance;
								context.ResultObject = vertex;
								context.ResultPosition = vertex.Position;
							}
						}
					}
				}
			}

			//edge
			if( selectionMode == SelectionMode.Edge && meshExtractedStructure != null )
			{
				foreach( var index in GetVisibleEdges( viewport ) )
				{
					var edge = meshExtractedStructure.Structure.Edges[ index ];
					var meshEdge = meshEdges[ index ];

					var v1 = meshVertices[ edge.Vertex1 ].Position;
					var v2 = meshVertices[ edge.Vertex2 ].Position;
					var center = ( v1 + v2 ) / 2;

					if( IsMouseNearLine( viewport, v1, v2 ) )
					{
						var distance = ( center - viewport.CameraSettings.Position ).Length();
						if( distance < minDistance )
						{
							minDistance = distance;
							context.ResultObject = meshEdge;
						}
					}
				}
			}

			//face
			if( selectionMode == SelectionMode.Face && meshExtractedStructure != null )
			{
				var ray = viewport.CameraSettings.GetRayByScreenCoordinates( mouse );

				foreach( var meshFace in meshFaces )
				{
					if( meshFace.Index < meshExtractedStructure.Structure.Faces.Length )
					{
						var face = meshExtractedStructure.Structure.Faces[ meshFace.Index ];

						for( int nTriangle = 0; nTriangle < face.Triangles.Length / 3; nTriangle++ )
						{
							var vertex0 = face.Triangles[ nTriangle * 3 + 0 ].Vertex;
							var vertex1 = face.Triangles[ nTriangle * 3 + 1 ].Vertex;
							var vertex2 = face.Triangles[ nTriangle * 3 + 2 ].Vertex;

							var p0 = meshVertices[ vertex0 ].Position;
							var p1 = meshVertices[ vertex1 ].Position;
							var p2 = meshVertices[ vertex2 ].Position;

							var bounds = new Bounds( p0 );
							bounds.Add( ref p1 );
							bounds.Add( ref p2 );

							if( bounds.Intersects( ref ray ) && MathAlgorithms.IntersectTriangleRay( ref p0, ref p1, ref p2, ref ray ) )
							{
								var center = ( p0 + p1 + p2 ) / 3;

								var distance = ( center - viewport.CameraSettings.Position ).Length();
								if( distance < minDistance )
								{
									minDistance = distance;
									context.ResultObject = meshFace;
								}
							}
						}
					}
				}
			}

			return true;
		}
		
		protected override bool OnTransformToolCreateObject( object forObject, ref TransformToolObject transformToolObject )
		{
			Vector3 GetCenter() => transformToolOperationCenter;

			if( forObject is Vertex vertex )
				transformToolObject = new TransformToolObject_Vertex( vertex, GetCenter );
			else if( forObject is Edge edge )
				transformToolObject = new TransformToolObject_Edge( edge, meshToEditData, meshVertices, GetCenter );
			else if( forObject is Face face )
				transformToolObject = new TransformToolObject_Face( face, meshToEditData, meshVertices, GetCenter );
			
			return true;
		}

		Vector3 transformToolOperationCenter;
		protected override bool OnTransformToolModifyBegin()
		{
			transformToolModifing = true;

			//Calculate transformToolOperationCenter
			var objects = DocumentWindow.TransformTool.Objects;
			Vector3 center = Vector3.Zero;
			foreach( var t in objects )
				center += t.Position;
			transformToolOperationCenter = center / objects.Count;

			return true;
		}

		protected override bool OnTransformToolModifyCommit()
		{

			int[] vertices = null;


			switch( selectionMode )
			{
			//vertices transform
			case SelectionMode.Vertex: vertices = GetSelectedVertices( DocumentWindow ); break;

			case SelectionMode.Edge:
				{
					var verticesH = new HashSet<int>();
					foreach( int nEdge in GetSelectedEdges( DocumentWindow ) )
					{
						verticesH.Add( meshToEditData.Edges[ nEdge ].Vertex1 );
						verticesH.Add( meshToEditData.Edges[ nEdge ].Vertex2 );
					}
					vertices = verticesH.ToArray();
					break;
				}

			case SelectionMode.Face:
				{
					var verticesH = new HashSet<int>();
					foreach( int nFace in GetSelectedFaces( DocumentWindow ) )
						foreach( var t in meshToEditData.Faces[ nFace ].Triangles )
							verticesH.Add( t.Vertex );
					vertices = verticesH.ToArray();
					break;
				}
			}

			if( vertices != null )
			{
				var transform = meshInSpaceToEdit.TransformV.ToMatrix4().GetInverse();

				var newPositions = new Vector3F[ vertices.Length ];
				for( int n = 0; n < vertices.Length; n++ )
				{
					var vertex = vertices[ n ];
					var meshVertex = meshVertices[ vertex ];

					var localPosition = transform * meshVertex.Position;

					newPositions[ n ] = localPosition.ToVector3F();
				}

				OneMeshActions.MoveVertices( new ActionContext( DocumentWindow ), vertices, newPositions );
			}


			//!!!!

			transformToolModifing = false;

			return true;
		}

		protected override bool OnTransformToolModifyCancel()
		{
			transformToolModifing = false;

			return true;
		}

		protected override bool OnTransformToolCloneAndSelectObjects()
		{
			//!!!!

			return true;
		}

		protected override void OnEditorActionGetState( EditorAction.GetStateContext context )
		{
			switch( context.Action.Name )
			{
			case "Delete":
				OneMeshActions.DeleteFacesGetState( context, new ActionContext( context ) );

				//!!!!
				//context.Enabled = true;
				//if( CanDeleteObjects( out List<Component> resultObjectsToDelete ) )
				//	context.Enabled = true;
				break;
			case "Duplicate":
				OneMeshActions.CloneFacesGetState( context, new ActionContext( context ) );
				break;
			}

		}

		protected override void OnEditorActionClick( EditorAction.ClickContext context )
		{
			switch( context.Action.Name )
			{
			case "Delete":
				OneMeshActions.DeleteFaces( new ActionContext( context ) );
				SelectFaces( new int[] { } );
				break;
			case "Duplicate":
				OneMeshActions.CloneFaces( new ActionContext( context ) );
				break;
			}
		}

		public Component_Mesh GetMeshToEdit()
		{
			if( meshInSpaceToEdit != null )
				return meshInSpaceToEdit.Mesh;
			return null;
		}

		public static (Component_MeshInSpace meshInSpace, Component_Mesh mesh) GetSelectedMesh( DocumentWindow documentWindow, object[] objectsInFocus )
		{
			var workareaMode = GetWorkareaMode( documentWindow );

			if( workareaMode != null )
				return (workareaMode.meshInSpaceToEdit, workareaMode.GetMeshToEdit());
			else
			{
				if( objectsInFocus != null && objectsInFocus.Length > 0 )
				{
					var meshInSpace = objectsInFocus[ 0 ] as Component_MeshInSpace;
					if( meshInSpace != null )
						return (meshInSpace, meshInSpace.Mesh);
				}
				else
				{
					var s = documentWindow?.SelectedObjects;
					if( s != null && 0 < s.Length && s[ 0 ] is Component_MeshInSpace meshInSpace )
						return (meshInSpace, meshInSpace.Mesh);
				}
				return (null, null);
			}
		}

		public static Selection GetSelection( DocumentWindow documentWindow )
		{
			var ret = new Selection();
			ret.SelectionMode = SelectionMode.Object;

			var workareaMode = GetWorkareaMode( documentWindow );
			if( workareaMode != null )
			{
				ret.SelectionMode = workareaMode.selectionMode;
				var result = new List<int>();
				var sel = documentWindow.SelectedObjects;
				switch( workareaMode.selectionMode )
				{
				case SelectionMode.Vertex:
					{
						foreach( var obj in sel )
							if( obj is Vertex vertex )
								result.Add( vertex.Index );
						ret.Vertices = result.ToArray();
						break;
					}
				case SelectionMode.Edge:
					{
						foreach( var obj in sel )
							if( obj is Edge edge )
								result.Add( edge.Index );
						ret.Edges = result.ToArray();
						break;
					}
				case SelectionMode.Face:
					{
						foreach( var obj in sel )
							if( obj is Face face )
								result.Add( face.Index );
						ret.Faces = result.ToArray();
						break;
					}
				}
			}

			ret.Changed = false;
			return ret;
		}

		//ToDo ??? Удалить  GetSelectedVertices,GetSelectedEdges,GetSelectedFaces
		public static int[] GetSelectedVertices( DocumentWindow documentWindow )
		{
			var workareaMode = GetWorkareaMode( documentWindow );
			return workareaMode == null ? new int[ 0 ] : workareaMode.GetSelectedVertices();
		}

		public int[] GetSelectedVertices()
		{
			var result = new List<int>();
			if( selectionMode == SelectionMode.Vertex )
			{
				foreach( var obj in DocumentWindow.SelectedObjects )
					if( obj is Vertex vertex )
						result.Add( vertex.Index );
			}
			return result.ToArray();
		}

		public static int[] GetSelectedEdges( DocumentWindow documentWindow )
		{
			var workareaMode = GetWorkareaMode( documentWindow );
			return workareaMode == null ? new int[ 0 ] : workareaMode.GetSelectedEdges();
		}

		public int[] GetSelectedEdges()
		{
			var result = new List<int>();
			if( selectionMode == SelectionMode.Edge )
			{
				foreach( var obj in DocumentWindow.SelectedObjects )
					if( obj is Edge edge )
						result.Add( edge.Index );
			}
			return result.ToArray();
		}

		public static int[] GetSelectedFaces( DocumentWindow documentWindow )
		{
			var workareaMode = GetWorkareaMode( documentWindow );
			return workareaMode == null ? new int[ 0 ] : workareaMode.GetSelectedFaces();
		}

		public int[] GetSelectedFaces()
		{
			var result = new List<int>();

			if( selectionMode == SelectionMode.Face )
			{
				foreach( var obj in DocumentWindow.SelectedObjects )
					if( obj is Face face )
						result.Add( face.Index );
			}
			return result.ToArray();
		}

		void DrawMeshTransformToolModifing( Viewport viewport )
		{
			var renderer = viewport.Simple3DRenderer;

			renderer.SetColor( new ColorValue( 0, 0, 1 ) );

			foreach( var face in meshToEditData.Faces )
			{
				for( int nTriangle = 0; nTriangle < face.Triangles.Count / 3; nTriangle++ )
				{
					var vertex0 = face.Triangles[ nTriangle * 3 + 0 ].Vertex;
					var vertex1 = face.Triangles[ nTriangle * 3 + 1 ].Vertex;
					var vertex2 = face.Triangles[ nTriangle * 3 + 2 ].Vertex;

					var p0 = meshVertices[ vertex0 ].Position;
					var p1 = meshVertices[ vertex1 ].Position;
					var p2 = meshVertices[ vertex2 ].Position;

					renderer.AddLineThin( p0, p1 );
					renderer.AddLineThin( p1, p2 );
					renderer.AddLineThin( p2, p0 );
				}
			}
		}

		void ClearMeshData()
		{
			meshToEditData = null;
			meshVertices.Clear();
			meshEdges.Clear();
			meshFaces.Clear();
			meshExtractedStructure = null;
		}

		public void UpdateMeshData()
		{
			needUpdateMeshData = false;

			var meshToEdit = GetMeshToEdit();

			if( meshToEdit == null || !meshToEdit.EnabledInHierarchy )
			{
				ClearMeshData();
				return;
			}

			//!!!!show error
			if( !CommonFunctions.CheckValidMesh( meshToEdit, out var error ) )
			{
				ClearMeshData();
				return;
			}

			////!!!!не только вершины
			////get last selected objects
			//var lastSelectedVertices = new List<int>();
			//foreach( var obj in DocumentWindow.SelectedObjects )
			//{
			//	var vertex = obj as Vertex;
			//	if( vertex != null )
			//		lastSelectedVertices.Add( vertex.Index );
			//}

			meshToEditData = new MeshData();
			meshExtractedStructure = meshToEdit.ExtractStructure();
			meshToEditData.Load( meshExtractedStructure );
			//meshToEditData.Load( meshToEdit );

			//create vertices
			if( meshVertices.Count != meshToEditData.Vertices.Count )
			{
				meshVertices.Clear();
				foreach( var vertex in meshToEditData.Vertices )
					meshVertices.Add( new Vertex() );
			}

			var vertexPositions = meshToEditData.GetVertexPositions();

			//update vertices
			var transform = meshInSpaceToEdit.TransformV;
			for( int nVertex = 0; nVertex < meshToEditData.Vertices.Count; nVertex++ )
			{
				var vertex = meshVertices[ nVertex ];
				vertex.Index = nVertex;

				vertexPositions.TryGetValue( nVertex, out var vertexPosition );
				vertex.Position = transform * vertexPosition;
				//vertex.Position = transform * meshToEditData.GetVertexPosition( nVertex );

				vertex.Rotation = Quaternion.Identity;
				vertex.Scale = new Vector3( 1, 1, 1 );
			}

			//!!!!когда нужно edges?
			if( selectionMode == SelectionMode.Edge )//|| selectionMode == SelectionMode.Face )
			{
				//create edges
				if( meshEdges.Count != meshToEditData.Edges.Count )
				{
					meshEdges.Clear();
					foreach( var edge in meshToEditData.Edges )
						meshEdges.Add( new Edge() );
				}

				//update edges
				for( int nEdge = 0; nEdge < meshToEditData.Edges.Count; nEdge++ )
				{
					var edge = meshEdges[ nEdge ];
					edge.Index = nEdge;

					edge.Rotation = Quaternion.Identity;
					edge.Scale = new Vector3( 1, 1, 1 );

					edge.Position = ( meshVertices[ meshToEditData.Edges[ nEdge ].Vertex1 ].Position + meshVertices[ meshToEditData.Edges[ nEdge ].Vertex2 ].Position ) / 2;
				}
			}

			if( selectionMode == SelectionMode.Face )
			{
				//create faces
				if( meshFaces.Count != meshToEditData.Faces.Count )
				{
					meshFaces.Clear();
					foreach( var face in meshToEditData.Faces )
						meshFaces.Add( new Face() );
				}

				//update faces
				for( int nFace = 0; nFace < meshToEditData.Faces.Count; nFace++ )
				{
					var face = meshFaces[ nFace ];
					face.Index = nFace;

					face.Rotation = Quaternion.Identity;
					face.Scale = new Vector3( 1, 1, 1 );

					Vector3 sum = Vector3.Zero;
					int count = 0;
					foreach( var t in meshToEditData.Faces[ nFace ].Triangles )
					{
						sum += meshVertices[ t.Vertex ].Position;
						count++;
					}
					face.Position = sum / count;
				}
			}
		}
		

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needUpdateMeshData = true;
		}

		public static BuilderWorkareaMode GetWorkareaMode( DocumentWindow documentWindow )
		{
			var sceneDocumentWindow = documentWindow as Component_Scene_DocumentWindow;
			if( sceneDocumentWindow != null )
				return sceneDocumentWindow.WorkareaMode as BuilderWorkareaMode;
			return null;
		}

		public void ChangeSelectionMode( SelectionMode value )
		{
			if( selectionMode != value )
			{
				var oldSelectionMode = selectionMode;

				if( meshToEditData == null )
				{
					selectionMode = value;
					DocumentWindow.SelectObjects( null );
					UpdateMeshData();
					return;
				}

				HashSet<int> oldSelectedVertices = null;

				switch( oldSelectionMode )
				{
				case SelectionMode.Face:
					{
						int[] selectedFaces = GetSelectedFaces( DocumentWindow );
						if( selectedFaces.Length != 0 )
						{
							var vs = selectedFaces //ToDo ??? Соптимизировать?
								.SelectMany( _ => meshToEditData.Faces[ _ ].Triangles )
								.Select( _ => _.Vertex ).ToArray();
							oldSelectedVertices = new HashSet<int>();
							foreach( var v in vs )
								oldSelectedVertices.Add( v );
						}
					}
					break;

				case SelectionMode.Vertex:
					{
						var vs = GetSelectedVertices( DocumentWindow );
						if( vs.Length != 0 )
						{
							oldSelectedVertices = new HashSet<int>();
							foreach( var v in vs )
								oldSelectedVertices.Add( v );
						}
					}
					break;

				case SelectionMode.Edge:
					{
						var es = GetSelectedEdges( DocumentWindow );
						if( es.Length != 0 )
						{
							oldSelectedVertices = new HashSet<int>();
							foreach( var e in es )
							{
								oldSelectedVertices.Add( meshToEditData.Edges[ e ].Vertex1 );
								oldSelectedVertices.Add( meshToEditData.Edges[ e ].Vertex2 );
							}
						}
					}
					break;
				}


				//----------------------------

				selectionMode = value; //Задавать до Select, иначе select не сработает.

				if( oldSelectedVertices == null || oldSelectedVertices.Count == 0 )
				{
					DocumentWindow.SelectObjects( null );
					UpdateMeshData(); //Todo ??? Нужно ли здесь.
					return;
				}

				switch( selectionMode )
				{
				case SelectionMode.Vertex:
					SelectVertices( oldSelectedVertices.ToArray() );
					break;

				case SelectionMode.Edge:
					var edges = new List<int>();
					for( int i = 0; i < meshToEditData.Edges.Count; i++ )
						if( oldSelectedVertices.Contains( meshToEditData.Edges[ i ].Vertex1 ) && oldSelectedVertices.Contains( meshToEditData.Edges[ i ].Vertex2 ) )
							edges.Add( i );
					SelectEdges( edges.ToArray() );
					break;

				case SelectionMode.Face:
					var faces = new List<int>();
					for( int i = 0; i < meshToEditData.Faces.Count; i++ )
					{
						var f = meshToEditData.Faces[ i ];
						bool all = true;
						for( int j = 0; j < f.Triangles.Count; j++ )
						{
							if( !oldSelectedVertices.Contains( f.Triangles[ j ].Vertex ) )
							{
								all = false;
								break;
							}
						}

						if( all )
							faces.Add( i );
					}
					SelectFaces( faces.ToArray() );
					break;

				case SelectionMode.Object:
					DocumentWindow.SelectObjects( null );
					break;
				}

				UpdateMeshData(); //Todo ??? Нужно ли здесь.
			}
		}

		public void SelectVertices( int[] vertices )
		{
			UpdateMeshData();

			var verticesSet = new ESet<int>();
			foreach( var v in vertices )
				if( 0 <= v )  //После удаления объектов, может указывать на -1, проще здесь отсекать.
					verticesSet.Add( v );

			var objectsToSelect = new List<Vertex>();
			foreach( var vertex in meshVertices )
			{
				if( verticesSet.Contains( vertex.Index ) )
					objectsToSelect.Add( vertex );
			}

			DocumentWindow.SelectObjects( objectsToSelect.ToArray() );
		}

		public void SelectEdges( int[] edges )
		{
			UpdateMeshData();

			var edgesSet = new ESet<int>();
			foreach( var e in edges )
				if( 0 <= e )
					edgesSet.Add( e );

			var objectsToSelect = new List<Edge>();
			foreach( var edge in meshEdges )
			{
				if( edgesSet.Contains( edge.Index ) )
					objectsToSelect.Add( edge );
			}

			DocumentWindow.SelectObjects( objectsToSelect.ToArray() );
		}

		public void SelectFaces( int[] faces )
		{
			UpdateMeshData();

			var facesSet = new ESet<int>();
			foreach( var f in faces )
				if( 0 <= f )
					facesSet.Add( f );

			var objectsToSelect = new List<Face>();
			foreach( var face in meshFaces )
			{
				if( facesSet.Contains( face.Index ) )
					objectsToSelect.Add( face );
			}

			DocumentWindow.SelectObjects( objectsToSelect.ToArray() );
		}

		static bool GetEditableSelectedObject( object[] objectsToCheck, out Component_MeshInSpace meshInSpace )//, out Component_Mesh mesh )
		{
			//!!!!что еще проверять
			//!!!!!как когда несколько объектов выделено
			if( objectsToCheck.Length != 0 )
			{
				var meshInSpace2 = objectsToCheck[ 0 ] as Component_MeshInSpace;
				if( meshInSpace2 != null )
				{
					//var mesh2 = meshInSpace2.GetComponent<Component_Mesh>(); //ToDo ???  Было так. Но так не редактируются объекты у которых нет дочернего Mesh, но есть meshInSpace2.Mesh.Value
					var mesh2 = meshInSpace2.Mesh.Value; //ToDo ??? Так правильно? а не через meshInSpace2.GetComponent<Component_Mesh>(); 

					if( mesh2 != null )
					{
						meshInSpace = meshInSpace2;
						//mesh = mesh2;
						return true;
					}
				}
			}

			meshInSpace = null;
			//mesh = null;
			return false;
		}

		private void DocumentWindow_SelectedObjectsChanged( DocumentWindow sender, object[] oldSelectedObjects )
		{
			////!!!!не только вершины
			//if( oldSelectedObjects.FirstOrDefault( obj => obj is Vertex ) == null )
			//{
			//	//save selected objects
			//	selectedObjectsToRestore = (object[])oldSelectedObjects.Clone();
			//}

			if( GetEditableSelectedObject( DocumentWindow.SelectedObjects, out var meshInSpace ) )//, out var mesh ) )
			{
				meshInSpaceToEdit = meshInSpace;
				//meshToEdit = mesh;
				UpdateMeshData();

				//clear selection
				DocumentWindow.SelectObjects( null );
			}
		}

		public void SelectAllGetState( EditorAction.GetStateContext context )
		{
			switch( selectionMode )
			{
			case SelectionMode.Vertex:
				context.Enabled = !meshVertices.All( obj => DocumentWindow.IsObjectSelected( obj ) );
				break;
			case SelectionMode.Edge:
				context.Enabled = !meshEdges.All( obj => DocumentWindow.IsObjectSelected( obj ) );
				break;
			case SelectionMode.Face:
				context.Enabled = !meshFaces.All( obj => DocumentWindow.IsObjectSelected( obj ) );
				break;
			}
		}

		//Selects all elements.
		public void SelectAll()
		{
			switch( selectionMode )
			{
			case SelectionMode.Vertex:
				DocumentWindow.SelectObjects( meshVertices.ToArray() );
				break;
			case SelectionMode.Edge:
				DocumentWindow.SelectObjects( meshEdges.ToArray() );
				break;
			case SelectionMode.Face:
				DocumentWindow.SelectObjects( meshFaces.ToArray() );
				break;
			}
		}

		public void InvertSelectionGetState( EditorAction.GetStateContext context )
		{
			switch( selectionMode )
			{
			case SelectionMode.Vertex:
				if( meshVertices.Count != 0 )
					context.Enabled = true;
				break;
			case SelectionMode.Edge:
				if( meshEdges.Count != 0 )
					context.Enabled = true;
				break;
			case SelectionMode.Face:
				if( meshFaces.Count != 0 )
					context.Enabled = true;
				break;
			}
		}

		//Selects all the elements that are not currently selected and removes selection from the currently selected elements.
		public void InvertSelection()
		{
			switch( selectionMode )
			{
			case SelectionMode.Vertex:
				DocumentWindow.SelectObjects( meshVertices.Where( obj => !DocumentWindow.IsObjectSelected( obj ) ).ToArray() );
				break;
			case SelectionMode.Edge:
				DocumentWindow.SelectObjects( meshEdges.Where( obj => !DocumentWindow.IsObjectSelected( obj ) ).ToArray() );
				break;
			case SelectionMode.Face:
				DocumentWindow.SelectObjects( meshFaces.Where( obj => !DocumentWindow.IsObjectSelected( obj ) ).ToArray() );
				break;
			}
		}

		void CalculateVisiblePointsTaskUpdate( Viewport viewport )
		{
			if( selectionMode == SelectionMode.Vertex )
			{
				//check cancelled
				if( calculateVisiblePointsTask != null && calculateVisiblePointsTask.IsCanceled )
				{
					var data = (CalculateVisiblePointsData)calculateVisiblePointsTask.AsyncState;
					data.meshRayTestOptimized?.Dispose();
					calculateVisiblePointsTask = null;
				}

				//process result
				if( calculateVisiblePointsTask != null && calculateVisiblePointsTask.IsCompleted )
				{
					var data = (CalculateVisiblePointsData)calculateVisiblePointsTask.AsyncState;
					data.meshRayTestOptimized?.Dispose();
					calculateVisiblePointsTask = null;

					calculateVisiblePointsDataCache = data;
				}

				//create new task
				if( calculateVisiblePointsTask == null )
				{
					if( meshInSpaceToEdit != null )
					{
						var mesh = meshInSpaceToEdit.MeshOutput;
						if( mesh != null && mesh.Result != null && mesh.Result.ExtractedVerticesPositions != null )
						{
							var data = new CalculateVisiblePointsData();
							data.meshToEditData = meshToEditData;

							data.meshInSpaceTransformInvert = meshInSpaceToEdit.Transform.Value.ToMatrix4().GetInverse();
							data.meshInSpaceTwoSided = Component_MeshInSpace.IsTwoSided( mesh, meshInSpaceToEdit.ReplaceMaterial );
							data.meshCompiledData_OnlyToCompare = mesh.Result;
							data.meshExtractedVerticesPositions = mesh.Result.ExtractedVerticesPositions;
							data.meshExtractedIndices = mesh.Result.ExtractedIndices;

							data.meshVertices = new List<Vertex>( meshVertices );
							data.viewportCameraSettingsPosition = viewport.CameraSettings.Position;

							var alreadyCalculated = calculateVisiblePointsDataCache != null &&
								calculateVisiblePointsDataCache.meshInSpaceTransformInvert == data.meshInSpaceTransformInvert &&
								calculateVisiblePointsDataCache.meshInSpaceTwoSided == data.meshInSpaceTwoSided &&
								calculateVisiblePointsDataCache.meshCompiledData_OnlyToCompare == data.meshCompiledData_OnlyToCompare &&
								calculateVisiblePointsDataCache.meshToEditData == data.meshToEditData &&
								calculateVisiblePointsDataCache.viewportCameraSettingsPosition == data.viewportCameraSettingsPosition;

							if( !alreadyCalculated )
							{
								calculateVisiblePointsTask = new Task( CalculateVisiblePoints, data );
								calculateVisiblePointsTask.Start();
							}
						}
					}
				}
			}
		}

		void CalculateVisibleEdgesTaskUpdate( Viewport viewport )
		{
			if( selectionMode == SelectionMode.Edge && meshExtractedStructure != null )
			{
				//check cancelled
				if( calculateVisibleEdgesTask != null && calculateVisibleEdgesTask.IsCanceled )
					calculateVisibleEdgesTask = null;

				//process result
				if( calculateVisibleEdgesTask != null && calculateVisibleEdgesTask.IsCompleted )
				{
					calculateVisibleEdgesDataCache = (CalculateVisibleEdgesData)calculateVisibleEdgesTask.AsyncState;
					calculateVisibleEdgesTask = null;
				}

				//create new task
				if( calculateVisibleEdgesTask == null )
				{
					var data = new CalculateVisibleEdgesData();
					data.meshToEditData = meshToEditData;
					data.meshVertices = new List<Vertex>( meshVertices );
					data.meshEdges = new List<Edge>( meshEdges );
					data.meshExtractedStructure = meshExtractedStructure;
					data.viewportCameraSettingsPosition = viewport.CameraSettings.Position;

					var alreadyCalculated = calculateVisibleEdgesDataCache != null &&
						calculateVisibleEdgesDataCache.meshToEditData == data.meshToEditData &&
						calculateVisibleEdgesDataCache.viewportCameraSettingsPosition == data.viewportCameraSettingsPosition;

					if( !alreadyCalculated )
					{
						calculateVisibleEdgesTask = new Task( CalculateVisibleEdges, data );
						calculateVisibleEdgesTask.Start();
					}
				}
			}
		}
	}
}
#endif