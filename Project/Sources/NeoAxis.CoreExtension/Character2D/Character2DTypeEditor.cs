// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Character2DTypeEditor : CanvasBasedEditor
	{
		bool firstCameraUpdate = true;

		bool needRecreateInstance;
		Character2D objectInSpace;

		//

		public Character2DType Character2DType
		{
			get { return (Character2DType)ObjectOfEditor; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Character2D>( enabled: false );
			objectInSpace.NewObjectSetDefaultConfiguration();
			objectInSpace.CharacterType = Character2DType;
			objectInSpace.Enabled = true;
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( Character2DType != null )
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Mode = Scene.ModeEnum._2D;
				scene.DisplayDevelopmentDataInEditor = true;
				scene.DisplayPhysicalObjects = true;
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

			//var camera = Scene.CameraEditor2D.Value;
			//var bounds = objectInSpace.SpaceBounds.BoundingBox;
			//var cameraLookTo = bounds.GetCenter();

			//double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			//double distance = maxGararite * 1.5;// 2;
			//if( distance < 2 )
			//	distance = 2;

			//double cameraZoomFactor = 1;
			//SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );

			//var cameraPosition = cameraLookTo - cameraDirection.GetVector() * distance * cameraZoomFactor;
			//var center = cameraLookTo;

			//Vector3 from = cameraPosition;
			//Vector3 to = center;
			//Degree fov = 65;

			//camera.FieldOfView = fov;
			//camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );
			//camera.FarClipPlane = Math.Max( 1000, distance * 2 );

			//camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
			//camera.FixedUp = Vector3.ZAxis;
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needRecreateInstance = true;
		}
	}
}
#endif