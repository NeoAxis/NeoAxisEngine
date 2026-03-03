// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class SeatTypeEditor : CanvasBasedEditor
	{
		bool firstCameraUpdate = true;

		bool needRecreateInstance;
		Seat objectInSpace;

		//need shadows
		//MeshInSpace ground;

		//

		public SeatType SeatType
		{
			get { return (SeatType)ObjectOfEditor; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Seat>( enabled: false );
			objectInSpace.SeatType = SeatType;
			objectInSpace.Enabled = true;

			objectInSpace.DebugVisualization = true;
		}

		//void CreateGround()
		//{
		//	ground?.Dispose();

		//	ground = Scene.CreateComponent<MeshInSpace>( enabled: false );

		//	var mesh = ground.CreateComponent<Mesh>();
		//	mesh.Name = "Mesh";
		//	var geometry = mesh.CreateComponent<MeshGeometry_Cylinder>();

		//	double radius;
		//	if( SeatType.Mesh.Value != null )
		//		radius = SeatType.Mesh.Value.Result.SpaceBounds.BoundingSphere.Radius * 1.5;
		//	else
		//		radius = 3;

		//	geometry.Radius = radius;
		//	geometry.Height = geometry.Radius / 40;
		//	geometry.Segments = 128;
		//	geometry.Material = new ReferenceNoValue( @"Base\Materials\Dark Gray.material" );

		//	ground.Mesh = ReferenceUtility.MakeThisReference( ground, mesh );
		//	ground.Transform = new Transform( new Vector3( 0, 0, -geometry.Height / 2 ), Quaternion.Identity );

		//	ground.Enabled = true;
		//}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( SeatType != null )
			{
				var scene = CreateScene( false );
				CreateObject();
				//CreateGround();
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
				//CreateGround();
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

			firstCameraUpdate = false;
		}

		void InitCamera()
		{
			var camera = Scene.CameraEditor.Value;
			var bounds = objectInSpace.SpaceBounds.BoundingBox;
			var cameraLookTo = bounds.GetCenter();

			double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			double distance = maxGararite * 1.5;// 2;
			if( distance < 2 )
				distance = 2;

			double cameraZoomFactor = 0.8;
			//double cameraZoomFactor = 1;
			SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );

			var cameraPosition = cameraLookTo - cameraDirection.GetVector() * distance * cameraZoomFactor;
			var center = cameraLookTo;

			Vector3 from = cameraPosition;
			Vector3 to = center;
			Degree fov = 65;

			camera.FieldOfView = fov;
			camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );
			camera.FarClipPlane = Math.Max( 1000, distance * 2 );

			camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
			camera.FixedUp = Vector3.ZAxis;
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needRecreateInstance = true;
		}
	}
}
#endif