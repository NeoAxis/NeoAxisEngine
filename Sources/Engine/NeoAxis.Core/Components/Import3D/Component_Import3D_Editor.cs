// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public class Component_Import3D_Editor : CanvasBasedEditor
	{
		bool needResetCamera = true;

		bool needRecreateDisplayObject;
		Component_Import3D displayObject;
		Dictionary<Component_Mesh, Transform> displayObjectTransformBySourceMesh;

		//

		public Component_Import3D Import
		{
			get { return (Component_Import3D)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			var scene = CreateScene( false );
			CreateDisplayObject();
			scene.Enabled = true;

			if( ObjectOfEditor != null )
				SelectObjects( new object[] { ObjectOfEditor } );

			if( Document != null )
				Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;
		}

		protected override void OnDestroy()
		{
			if( Document != null )
				Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

			base.OnDestroy();
		}

		public void NeedRecreateDisplayObject( bool resetCamera )
		{
			needRecreateDisplayObject = true;
			if( resetCamera )
				needResetCamera = true;

			unchecked
			{
				if( Import != null )
					Import.VersionForPreviewDisplay++;
			}
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needRecreateDisplayObject = true;

			unchecked
			{
				if( Import != null )
					Import.VersionForPreviewDisplay++;
			}
		}

		protected override void OnViewportCreated()
		{
			base.OnViewportCreated();

			//!!!!?
			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		void CreateDisplayObject()
		{
			if( Import != null )
				displayObject = Import.CreateForPreviewDisplay( Scene, out _, out displayObjectTransformBySourceMesh );
		}

		void DestroyDisplayObject()
		{
			if( displayObject != null )
			{
				displayObject.RemoveFromParent( false );
				displayObject.Dispose();
				displayObject = null;
				displayObjectTransformBySourceMesh = null;
			}
		}

		protected override void OnViewportUpdateBegin()
		{
			base.OnViewportUpdateBegin();

			if( Scene != null && needRecreateDisplayObject )
			{
				needRecreateDisplayObject = false;
				DestroyDisplayObject();
				CreateDisplayObject();
			}
		}

		protected override void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.OnSceneViewportUpdateGetCameraSettings( ref processed );

			if( needResetCamera && Scene.CameraEditor.Value != null )
			{
				InitCamera();
				Viewport.CameraSettings = new Viewport.CameraSettingsClass( Viewport, Scene.CameraEditor );

				needResetCamera = false;
			}
		}

		void InitCamera()
		{
			var camera = Scene.CameraEditor.Value;

			var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();

			var cameraLookTo = bounds.GetCenter();

			//!!!!

			double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			double distance = maxGararite * 2;// 2.3;
			if( distance < 2 )
				distance = 2;

			double cameraZoomFactor = 1;
			SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );

			var cameraPosition = cameraLookTo - cameraDirection.GetVector() * distance * cameraZoomFactor;
			var center = cameraLookTo;// GetSceneCenter();

			Vector3 from = cameraPosition;//center + cameraDirection.GetVector() * cameraDistance;
			Vector3 to = center;
			Degree fov = 65;// 75;

			//!!!!
			//camera.AspectRatio = (double)ViewportControl.Viewport.SizeInPixels.X / (double)ViewportControl.Viewport.SizeInPixels.Y;
			camera.FieldOfView = fov;
			camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );//.1;
			camera.FarClipPlane = Math.Max( 1000, distance * 2 );

			camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
			camera.FixedUp = Vector3.ZAxis;
		}

		Bounds GetTotalBoundsOfObjectInSpace()
		{
			Bounds total = NeoAxis.Bounds.Cleared;

			if( displayObject != null )
			{
				//if( createdObject.SpaceBounds.BoundingBox.HasValue )
				//	total.Add( createdObject.SpaceBounds.BoundingBox.Value );
				foreach( var obj in displayObject.GetComponents<Component_ObjectInSpace>( false, true ) )
				{
					if( obj.SpaceBounds.BoundingBox.HasValue )
						total.Add( obj.SpaceBounds.BoundingBox.Value );
				}
			}

			return total;
		}

		protected override void OnGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.OnGetTextInfoLeftTopCorner( lines );

			if( Import != null )
			{
				{
					int count = Import.GetComponents<Component_Material>( false, true ).Length;
					lines.Add( "Materials: " + count.ToString() );
				}

				{
					int count = Import.GetComponents<Component_Mesh>( false, true ).Where( m => m.FindParent<Component_Mesh>() == null ).Count();
					lines.Add( "Meshes: " + count.ToString() );
				}

				int sceneObjectsCount = Import.GetComponents<Component_ObjectInSpace>( false, true ).Length;
				if( sceneObjectsCount != 0 )
					lines.Add( "Scene objects: " + sceneObjectsCount.ToString() );

				var totalBounds = GetTotalBoundsOfObjectInSpace();
				if( !totalBounds.IsCleared() )
				{
					string text = "";
					if( Import.GetComponent( "Mesh" ) != null )
						text = "Mesh size";//text = "Mesh bounding box size";
					else if( sceneObjectsCount != 0 )
						text = "Total size of scene objects";//text = "Total bounding box size of scene objects";
					if( !string.IsNullOrEmpty( text ) )
						lines.Add( string.Format( "{0}: {1}", text, totalBounds.GetSize().ToString( 3 ) ) );
				}
			}
		}

		T[] ExtractChannel<T>( byte[] vertices, int vertexSize, int vertexCount, int startOffsetInBytes ) where T : unmanaged
		{
			T[] result = new T[ vertexCount ];
			unsafe
			{
				fixed ( byte* pVertices = vertices )
				{
					byte* src = pVertices + startOffsetInBytes;
					for( int n = 0; n < vertexCount; n++ )
					{
						result[ n ] = *(T*)src;
						src += vertexSize;
					}
				}
			}
			return result;
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			//highlight selected mesh geometries, materials
			{
				var geometriesToHighlight = new ESet<Component_MeshGeometry>();
				foreach( var geometry in Import.GetComponents<Component_MeshGeometry>( checkChildren: true ) )
				{
					//skip when selected is LOD
					if( geometry.ParentMesh != null && geometry.ParentMesh.FindParent<Component_Mesh>() == null )
					{
						//mesh selected. skip when available only one mesh
						if( SelectedObjectsSet.Contains( geometry.ParentMesh ) && Import.GetComponent( "Mesh" ) == null )
							geometriesToHighlight.AddWithCheckAlreadyContained( geometry );

						if( SelectedObjectsSet.Contains( geometry ) )
							geometriesToHighlight.AddWithCheckAlreadyContained( geometry );

						var material = geometry.Material.Value;
						if( material != null && SelectedObjectsSet.Contains( material ) )
							geometriesToHighlight.AddWithCheckAlreadyContained( geometry );
					}
				}

				foreach( var geometry in geometriesToHighlight )
				{
					var mesh = geometry.ParentMesh;
					var structure = geometry.VertexStructure.Value;
					var vertices = geometry.Vertices.Value;
					var indices = geometry.Indices.Value;

					if( structure != null )
					{
						structure.GetInfo( out var vertexSize, out var holes );
						var vertexCount = vertices.Length / vertexSize;

						try
						{
							if( structure.GetElementBySemantic( VertexElementSemantic.Position, out var element ) && element.Type == VertexElementType.Float3 )
							{
								if( vertices != null && indices != null )
								{
									//!!!!кешировать?

									var positions = ExtractChannel<Vector3F>( vertices, vertexSize, vertexCount, element.Offset );

									Transform transform;
									if( !displayObjectTransformBySourceMesh.TryGetValue( mesh, out transform ) )
										transform = Transform.Identity;
									var transformMatrix = transform.ToMatrix4();

									Viewport.Simple3DRenderer.SetColor( ProjectSettings.Get.SelectedColor );
									Viewport.Simple3DRenderer.AddTriangles( positions, indices, ref transformMatrix, true, true );
								}
							}
						}
						catch( Exception e )
						{
							Log.Warning( e.Message );
						}
					}
				}
			}

			////total bounds
			//var totalBounds = GetTotalBoundsOfObjectInSpace();
			//if( !totalBounds.IsCleared() )
			//{
			//	viewport.DebugGeometry.SetColor( new ColorValue( 1, 1, 0, 0.5 ) );
			//	viewport.DebugGeometry.AddBounds( totalBounds );
			//}

			//highlight selected objects in space
			{
				var selected = new ESet<Component_ObjectInSpace>();
				foreach( var obj in SelectedObjects )
				{
					var objectInSpace = obj as Component_ObjectInSpace;
					if( objectInSpace != null )
						selected.Add( objectInSpace );
				}

				foreach( var obj in selected )
				{
					var bounds = obj.SpaceBounds.CalculatedBoundingBox;

					Viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0, 0.5 ) );
					Viewport.Simple3DRenderer.AddBounds( bounds );

					//if( b.BoundingBox.HasValue )
					//	viewport.DebugGeometry.AddBounds( b.BoundingBox.Value );
					//if( b.BoundingSphere.HasValue )
					//	viewport.DebugGeometry.AddSphere( b.BoundingSphere.Value );
				}
			}
		}
	}
}
