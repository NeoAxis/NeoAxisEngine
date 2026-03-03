// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoAxis.Editor
{
	public class VehicleTypeEditor : CanvasBasedEditorWithObjectTransformTools
	{
		bool firstCameraUpdate = true;

		bool needRecreateInstance;
		Vehicle objectInSpace;
		int createdVersionOfType;

		//need shadows
		//MeshInSpace ground;

		//

		public VehicleType VehicleType
		{
			get { return (VehicleType)ObjectOfEditor; }
		}

		void CreateObject()
		{
			objectInSpace?.Dispose();

			objectInSpace = Scene.CreateComponent<Vehicle>( enabled: false );
			objectInSpace.VehicleType = VehicleType;
			//objectInSpace.Headlights = 1;
			objectInSpace.Enabled = true;

			objectInSpace.DebugVisualization = true;

			createdVersionOfType = VehicleType.Version;
		}

		//void CreateGround()
		//{
		//	ground?.Dispose();

		//	ground = Scene.CreateComponent<MeshInSpace>( enabled: false );

		//	var mesh = ground.CreateComponent<Mesh>();
		//	mesh.Name = "Mesh";
		//	var geometry = mesh.CreateComponent<MeshGeometry_Cylinder>();

		//	double radius;
		//	if( VehicleType.Mesh.Value != null )
		//		radius = VehicleType.Mesh.Value.Result.SpaceBounds.BoundingSphere.Radius * 1.5;
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

			if( VehicleType != null )
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

			if( createdVersionOfType != VehicleType.Version )
				needRecreateInstance = true;

			if( needRecreateInstance )
			{
				CreateObject();
				//CreateGround();
				needRecreateInstance = false;
			}

			if( Scene != null )
				Scene.DisplayPhysicalObjects = VehicleType.EditorDisplayPhysics;

			//visualize selected objects
			var renderer = Viewport.Simple3DRenderer;
			foreach( var obj in SelectedObjects )
			{
				var light = obj as Light;
				if( light != null )
				{
					var color = ProjectSettings.Get.Colors.SceneShowLightColor.Value;
					//color.Alpha *= 0.25f;
					renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
					light.DebugDraw( Viewport );
				}
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

			if( VehicleType != null && Scene.CameraEditor.Value != null )
				VehicleType.EditorCameraTransform = Scene.CameraEditor.Value.Transform;

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

			if( VehicleType != null && VehicleType.EditorCameraTransform != null )
				camera.Transform = VehicleType.EditorCameraTransform;
			else
				camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );

			camera.FixedUp = Vector3.ZAxis;
		}

		private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		{
			needRecreateInstance = true;
		}

		protected override void AddObjectScreenLabel( ViewportRenderingContext context, Component obj, Vector3 objectPosition )
		{
			//hide screen labels depending of the settings
			var wheel = obj as VehicleTypeWheel;
			if( wheel != null && !VehicleType.EditorDisplayWheels )
				return;
			var light = obj as Light;
			if( light != null && !VehicleType.EditorDisplayLights )
				return;
			var seatItem = obj as SeatItem;
			if( seatItem != null && !VehicleType.EditorDisplaySeats )
				return;

			base.AddObjectScreenLabel( context, obj, objectPosition );
		}
	}
}
