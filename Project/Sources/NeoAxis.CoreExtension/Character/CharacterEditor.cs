// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class CharacterEditor : ObjectInSpaceEditor//CanvasBasedEditor
	{
		//bool firstCameraUpdate = true;

		//bool needRecreateInstance;
		//Character instanceInScene;

		//

		public Character Character
		{
			get { return (Character)ObjectOfEditor; }
		}

		//protected override void OnCreate()
		//{
		//	base.OnCreate();

		//	if( Character != null )
		//	{
		//		var scene = CreateScene( false );
		//		scene.DisplayPhysicalObjects = true;

		//		instanceInScene = (Character)Character.Clone();
		//		scene.AddComponent( instanceInScene );

		//		scene.Enabled = true;

		//		if( Document != null )
		//			Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

		//		if( ObjectOfEditor != null )
		//			SelectObjects( new object[] { ObjectOfEditor } );
		//	}
		//}

		//protected override void OnDestroy()
		//{
		//	if( Document != null )
		//		Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

		//	base.OnDestroy();
		//}

		//protected override void OnViewportCreated()
		//{
		//	base.OnViewportCreated();

		//	ViewportControl.Viewport.AllowRenderScreenLabels = false;
		//}

		//protected override void OnViewportUpdateBeforeOutput()
		//{
		//	base.OnViewportUpdateBeforeOutput();

		//	if( Character != null && needRecreateInstance )
		//	{
		//		instanceInScene?.Dispose();

		//		instanceInScene = (Character)Character.Clone();
		//		Scene.AddComponent( instanceInScene );

		//		needRecreateInstance = false;
		//	}
		//}

		//protected override void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		//{
		//	base.OnSceneViewportUpdateGetCameraSettings( ref processed );

		//	if( firstCameraUpdate && Scene.CameraEditor.Value != null )
		//	{
		//		InitCamera();
		//		Viewport.CameraSettings = new Viewport.CameraSettingsClass( Viewport, Scene.CameraEditor );
		//	}

		//	firstCameraUpdate = false;
		//}

		//void InitCamera()
		//{
		//	var camera = Scene.CameraEditor.Value;
		//	var bounds = Character.SpaceBounds.CalculatedBoundingBox;
		//	var cameraLookTo = bounds.GetCenter();

		//	double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
		//	double distance = maxGararite * 2;
		//	if( distance < 2 )
		//		distance = 2;

		//	double cameraZoomFactor = 1;
		//	SphericalDirection cameraDirection = new SphericalDirection( -3.83, -.47 );

		//	var cameraPosition = cameraLookTo - cameraDirection.GetVector() * distance * cameraZoomFactor;
		//	var center = cameraLookTo;

		//	Vector3 from = cameraPosition;
		//	Vector3 to = center;
		//	Degree fov = 65;

		//	camera.FieldOfView = fov;
		//	camera.NearClipPlane = Math.Max( distance / 10000, 0.01 );
		//	camera.FarClipPlane = Math.Max( 1000, distance * 2 );

		//	camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
		//	camera.FixedUp = Vector3.ZAxis;
		//}

		//private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		//{
		//	needRecreateInstance = true;
		//}
	}
}
#endif