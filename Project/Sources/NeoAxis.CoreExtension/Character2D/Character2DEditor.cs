// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Character2DEditor : CanvasBasedEditor
	{
		bool firstCameraUpdate = true;

		bool needRecreateInstance;
		Character2D instanceInScene;

		//

		public Character2D Character
		{
			get { return (Character2D)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( Character != null )
			{
				var scene = CreateScene( false );
				scene.Mode = Scene.ModeEnum._2D;
				scene.DisplayPhysicalObjects = true;

				instanceInScene = (Character2D)Character.Clone();
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

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			if( Character != null && needRecreateInstance )
			{
				instanceInScene?.Dispose();

				instanceInScene = (Character2D)Character.Clone();
				Scene.AddComponent( instanceInScene );

				needRecreateInstance = false;
			}
		}

		protected override void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.OnSceneViewportUpdateGetCameraSettings( ref processed );

			if( firstCameraUpdate && Scene.CameraEditor2D.Value != null )
			{
				InitCamera();
				Viewport.CameraSettings = new Viewport.CameraSettingsClass( Viewport, Scene.CameraEditor2D );
			}

			firstCameraUpdate = false;
		}

		void InitCamera()
		{
			var camera = Scene.CameraEditor2D.Value;
			var bounds = Character.SpaceBounds.BoundingBox;
			var cameraLookTo = bounds.GetCenter();

			camera.Projection = ProjectionType.Orthographic;
			camera.Height = bounds.GetSize().MaxComponent() * 3;
			camera.FarClipPlane = 100;
			camera.Transform = new Transform( cameraLookTo + new Vector3( 0, 0, 10 ), Quaternion.LookAt( -Vector3.ZAxis, Vector3.YAxis ), Vector3.YAxis );
			camera.FixedUp = Vector3.YAxis;
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needRecreateInstance = true;
		}
	}
}
#endif