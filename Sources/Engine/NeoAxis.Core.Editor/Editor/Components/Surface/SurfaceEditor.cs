#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class SurfaceEditor : CanvasBasedEditor
	{
		double lastUpdateTime;
		bool needCameraUpdate = true;

		//

		public Surface Surface
		{
			get { return (Surface)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			//create scene
			if( Surface != null )
			{
				var scene = CreateScene( false );
				CreateObjects( scene );
				scene.Enabled = true;
			}

			if( ObjectOfEditor != null )
				SelectObjects( new object[] { ObjectOfEditor } );
		}

		protected override void OnViewportCreated()
		{
			base.OnViewportCreated();

			//!!!!?
			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void OnTick( float delta )
		{
			base.OnTick( delta );

			//update scene
			if( Time.Current > lastUpdateTime + 1.0 )// 0.1 )
			{
				if( Surface != null && Scene != null )
					CreateObjects( Scene );
			}
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			////update scene
			//if( Time.Current > lastUpdateTime + 1.0 )// 0.1 )
			//{
			//	if( Surface != null && Scene != null )
			//		CreateObjects( Scene );
			//}

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

		protected override void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.OnSceneViewportUpdateGetCameraSettings( ref processed );

			if( needCameraUpdate && Scene.CameraEditor.Value != null )
			{
				InitCamera( 0.5 );
				Viewport.CameraSettings = new Viewport.CameraSettingsClass( Viewport, Scene.CameraEditor );

				needCameraUpdate = false;
			}

			if( Surface != null && Scene.CameraEditor.Value != null )
				Surface.EditorCameraTransform = Scene.CameraEditor.Value.Transform;
		}

		void InitCamera( double distanceScale = 1 )
		{
			var camera = Scene.CameraEditor.Value;
			var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();
			var cameraLookTo = bounds.GetCenter();

			double maxGararite = Math.Max( Math.Max( bounds.GetSize().X, bounds.GetSize().Y ), bounds.GetSize().Z );
			double distance = maxGararite * 2 * distanceScale;// 2.3;
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

			if( Surface != null && Surface.EditorCameraTransform != null )
				camera.Transform = Surface.EditorCameraTransform;
			else
				camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );

			camera.FixedUp = Vector3.ZAxis;
		}

		void CreateObjects( Scene scene )
		{
			SurfaceEditorUtility.CreatePreviewObjects( scene, Surface );
			lastUpdateTime = Time.Current;
		}
	}
}

#endif