// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis.Editor
{
	public partial class SpriteEditor : DocumentWindowWithViewport
	{
		bool firstCameraUpdate = true;

		bool needRecreateInstance;
		Sprite instanceInScene;

		//

		public SpriteEditor()
		{
			InitializeComponent();
		}

		public Sprite Sprite
		{
			get { return (Sprite)ObjectOfWindow; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( Sprite != null && Sprite.ParentScene == null )//show only when not in a scene
			{
				var scene = CreateScene( false );
				scene.Mode = Scene.ModeEnum._2D;

				instanceInScene = (Sprite)Sprite.Clone();
				scene.AddComponent( instanceInScene );

				scene.Enabled = true;

				if( Document != null )
					Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

				if( ObjectOfWindow != null )
					SelectObjects( new object[] { ObjectOfWindow } );
			}
			else
			{
				ViewportControl.AllowCreateRenderWindow = false;
				ViewportControl.Visible = false;
			}
		}

		protected override void OnDestroy()
		{
			if( Document != null )
				Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

			base.OnDestroy();
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			if( Sprite != null && Sprite.ParentScene == null && needRecreateInstance )
			{
				instanceInScene?.Dispose();

				instanceInScene = (Sprite)Sprite.Clone();
				Scene.AddComponent( instanceInScene );

				needRecreateInstance = false;
			}
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Scene scene, Viewport viewport, ref bool processed )
		{
			base.Scene_ViewportUpdateGetCameraSettings( scene, viewport, ref processed );

			if( firstCameraUpdate && scene.CameraEditor2D.Value != null )
			{
				InitCamera();
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, scene.CameraEditor2D );
			}

			firstCameraUpdate = false;
		}

		void InitCamera()
		{
			var camera = Scene.CameraEditor2D.Value;
			var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();
			var cameraLookTo = bounds.GetCenter();

			var from = cameraLookTo + new Vector3( 0, 0, Scene.CameraEditor2DPositionZ );
			Vector3 to = cameraLookTo;

			//camera.AspectRatio = (double)ViewportControl.Viewport.SizeInPixels.X / (double)ViewportControl.Viewport.SizeInPixels.Y;
			camera.NearClipPlane = 0.01;
			camera.FarClipPlane = 1000;
			camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.YAxis ) );
			camera.Projection = ProjectionType.Orthographic;
			camera.FixedUp = Vector3.YAxis;
			//!!!!need consider size by X
			camera.Height = bounds.GetSize().Y * 2;// 1.4;
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needRecreateInstance = true;
		}
	}
}
