// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	public partial class Component_Character2D_DocumentWindow : DocumentWindowWithViewport
	{
		bool firstCameraUpdate = true;

		bool needRecreateInstance;
		Component_Character2D instanceInScene;

		//

		public Component_Character2D_DocumentWindow()
		{
			InitializeComponent();
		}

		public Component_Character2D Character
		{
			get { return (Component_Character2D)ObjectOfWindow; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( Character != null )
			{
				var scene = CreateScene( false );
				scene.Mode = Component_Scene.ModeEnum._2D;
				scene.DisplayPhysicalObjects = true;

				instanceInScene = (Component_Character2D)Character.Clone();
				scene.AddComponent( instanceInScene );

				scene.Enabled = true;

				if( Document != null )
					Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

				if( ObjectOfWindow != null )
					SelectObjects( new object[] { ObjectOfWindow } );
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

			if( Character != null && needRecreateInstance )
			{
				instanceInScene?.Dispose();

				instanceInScene = (Component_Character2D)Character.Clone();
				Scene.AddComponent( instanceInScene );

				needRecreateInstance = false;
			}
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
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
			var bounds = Character.SpaceBounds.CalculatedBoundingBox;
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
