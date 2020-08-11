// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	public partial class Component_Camera_PreviewControl : Component_ObjectInSpace_PreviewControl
	{
		bool displayPreview;

		//

		public Component_Camera_PreviewControl()
		{
			InitializeComponent();
		}

		public Component_Camera Camera
		{
			get { return ObjectOfPreview as Component_Camera; }
		}

		protected override bool EnableViewportControl
		{
			get { return Camera != null && Camera.ParentScene != null; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( Camera != null && Camera.ParentScene != null )
				displayPreview = true;
		}

		protected override void ViewportControl_ViewportCreated( EngineViewportControl sender )
		{
			base.ViewportControl_ViewportCreated( sender );

			if( displayPreview )
			{
				Viewport.AttachedScene = Camera.ParentScene;
				ViewportControl.OverrideCameraSettings = new Viewport.CameraSettingsClass( Viewport, Camera );
			}
		}

		protected override void Viewport_UpdateBegin( Viewport viewport )
		{
			base.Viewport_UpdateBegin( viewport );

			if( displayPreview )
				ViewportControl.OverrideCameraSettings = new Viewport.CameraSettingsClass( Viewport, Camera );
		}
	}
}
