// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Item2DTypeEditor : CanvasBasedEditor
	{
		bool firstCameraUpdate = true;

		bool needRecreateInstance;
		Item2D objectInSpace;

		//

		public Item2DType ItemType
		{
			get { return (Item2DType)ObjectOfEditor; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Item2D>( enabled: false );
			objectInSpace.ItemType = ItemType;
			objectInSpace.Enabled = true;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( ItemType != null )
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Mode = Scene.ModeEnum._2D;
				Scene.DisplayDevelopmentDataInEditor = true;
				Scene.DisplayPhysicalObjects = true;
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

			if( needRecreateInstance )
			{
				CreateObject();
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
			var bounds = objectInSpace.SpaceBounds.BoundingBox;
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