// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	public partial class Component_Surface_DocumentWindow : DocumentWindowWithViewport
	{
		double lastUpdateTime;
		bool needCameraUpdate = true;

		//

		public Component_Surface_DocumentWindow()
		{
			InitializeComponent();
		}

		public Component_Surface Surface
		{
			get { return (Component_Surface)ObjectOfWindow; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			//create scene
			if( Surface != null )
			{
				var scene = CreateScene( false );
				CreateObjects( scene );
				scene.Enabled = true;
			}

			if( ObjectOfWindow != null )
				SelectObjects( new object[] { ObjectOfWindow } );
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			//!!!!?
			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			//update scene
			if( Time.Current > lastUpdateTime + 0.1 )
			{
				if( Surface != null && Scene != null )
					CreateObjects( Scene );
			}
		}

		protected override void Scene_ViewportUpdateGetCameraSettings( Component_Scene scene, Viewport viewport, ref bool processed )
		{
			base.Scene_ViewportUpdateGetCameraSettings( scene, viewport, ref processed );

			if( needCameraUpdate && scene.CameraEditor.Value != null )
			{
				InitCamera( 0.5 );
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, scene.CameraEditor );

				needCameraUpdate = false;
			}
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

			camera.Transform = new Transform( from, Quaternion.LookAt( ( to - from ).GetNormalize(), Vector3.ZAxis ) );
			camera.FixedUp = Vector3.ZAxis;
		}

		void CreateObjects( Component_Scene scene )
		{
			Component_SurfaceUtility.CreatePreviewObjects( scene, Surface );
			lastUpdateTime = Time.Current;
		}
	}
}
