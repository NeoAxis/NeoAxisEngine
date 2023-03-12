// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NeoAxis;

namespace SampleWidgetWinForms
{
	public partial class AdditionalForm : Form
	{
		public AdditionalForm()
		{
			InitializeComponent();

			base.Font = new System.Drawing.Font( new FontFamily( "Microsoft Sans Serif" ), 8f );

			widgetControl1.ViewportCreated += WidgetControl1_ViewportCreated;
		}

		private void buttonClose_Click( object sender, EventArgs e )
		{
			Close();
		}

		private void WidgetControl1_ViewportCreated( NeoAxis.Editor.EngineViewportControl sender )
		{
			Scene.First.ViewportUpdateGetCameraSettings += Scene_ViewportUpdateGetCameraSettings;

			//bind scene to viewport
			widgetControl1.Viewport.AttachedScene = Scene.First;
		}

		protected virtual void Scene_ViewportUpdateGetCameraSettings( Scene scene, Viewport viewport, ref bool processed )
		{
			if( widgetControl1.Viewport == viewport )
			{
				var defaultCamera = scene.CameraDefault.Value;
				if( defaultCamera == null )
					defaultCamera = scene.Mode.Value == Scene.ModeEnum._3D ? scene.CameraEditor : scene.CameraEditor2D;

				var position = new Vector3( 27, 4, -9 );
				var lookTo = new Vector3( 25, -0.6, -10 );
				var up = Vector3.ZAxis;

				//var obj = scene.GetComponent<ObjectInSpace>( "Mesh in Space 68" );
				//if( obj != null )
				//{
				//	var tr = obj.TransformV;
				//	position = tr.Position + tr.Rotation * new Vector3( 0.2, 0, 0.1 );
				//	lookTo = position + tr.Rotation * new Vector3( 1, 0, 0 );
				//	up = tr.Rotation * new Vector3( 0, 0, 1 );
				//}

				var camera = (Camera)defaultCamera.Clone();
				//camera = new Camera();
				camera.Transform = new Transform( position, Quaternion.LookAt( ( lookTo - position ).GetNormalize(), up ) );
				camera.FixedUp = up;
				viewport.CameraSettings = new Viewport.CameraSettingsClass( viewport, camera );

				processed = true;
			}
		}
	}
}
