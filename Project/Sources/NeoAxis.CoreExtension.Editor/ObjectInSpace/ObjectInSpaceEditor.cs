#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class ObjectInSpaceEditor : CanvasBasedEditorWithObjectTransformTools//CanvasBasedEditor
	{
		bool firstCameraUpdate = true;

		bool needRecreateInstance;
		ObjectInSpace instanceInScene;

		//

		public ObjectInSpace ObjectInSpace
		{
			get { return (ObjectInSpace)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( ObjectInSpace != null )
			{
				var scene = CreateScene( false );
				scene.DisplayPhysicalObjects = true;

				instanceInScene = (ObjectInSpace)ObjectInSpace.Clone();
				scene.AddComponent( instanceInScene );

				scene.Enabled = true;

				if( Document != null )
					Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

				if( ObjectOfEditor != null )
					SelectObjects( new object[] { ObjectOfEditor } );
			}
		}

		protected override void OnDestroy()
		{
			if( Document != null )
				Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

			base.OnDestroy();
		}

		protected override void OnViewportCreated()
		{
			base.OnViewportCreated();

			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void OnTick( float delta )
		{
			base.OnTick( delta );

			if( ObjectInSpace != null && needRecreateInstance )
			{
				instanceInScene?.Dispose();

				instanceInScene = (ObjectInSpace)ObjectInSpace.Clone();
				Scene.AddComponent( instanceInScene );

				needRecreateInstance = false;
			}
		}

		protected override void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.OnSceneViewportUpdateGetCameraSettings( ref processed );

			if( firstCameraUpdate && Scene.CameraEditor.Value != null )
			{
				InitCamera();
				Viewport.CameraSettings = new Viewport.CameraSettingsClass( Viewport, Scene.CameraEditor );
			}

			if( ObjectInSpace != null && Scene.CameraEditor.Value != null )
				ObjectInSpace.EditorCameraTransform = Scene.CameraEditor.Value.Transform;

			firstCameraUpdate = false;
		}

		void InitCamera()
		{
			var camera = Scene.CameraEditor.Value;
			var bounds = ObjectInSpace.SpaceBounds.BoundingBox;
			var cameraLookTo = bounds.GetCenter();

			double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			double distance = maxGararite * 2;
			if( distance < 2 )
				distance = 2;

			double cameraZoomFactor = 1;
			SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );

			var cameraPosition = cameraLookTo - cameraDirection.GetVector() * distance * cameraZoomFactor;
			var center = cameraLookTo;

			Vector3 from = cameraPosition;
			Vector3 to = center;
			Degree fov = 65;

			camera.FieldOfView = fov;
			camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );
			camera.FarClipPlane = Math.Max( 1000, distance * 2 );

			if( ObjectInSpace != null && ObjectInSpace.EditorCameraTransform != null )
				camera.Transform = ObjectInSpace.EditorCameraTransform;
			else
				camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );

			camera.FixedUp = Vector3.ZAxis;
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needRecreateInstance = true;
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			if( ObjectInSpace != null )
			{
				//centering borders
				if( ProjectSettings.Get.General.ShowCenteringBorders )
				{
					double v = 0.8;

					var offsetX = v * 0.5 * Viewport.CanvasRenderer.AspectRatioInv;
					var offsetY = v * 0.5;

					var r = new Rectangle( new Vector2( 0.5, 0.5 ) );
					r.Expand( new Vector2( offsetX, offsetY ) );
					Viewport.CanvasRenderer.AddRectangle( r, new ColorValue( 1, 1, 1, 0.05 ) );
				}
			}
		}
	}
}

#endif