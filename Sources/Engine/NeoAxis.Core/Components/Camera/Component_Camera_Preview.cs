// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	public partial class Component_Camera_Preview : Component_ObjectInSpace_Preview
	{
		bool displayPreview;

		//

		public Component_Camera_Preview()
		{
		}

		public Component_Camera Camera
		{
			get { return ObjectOfPreview as Component_Camera; }
		}

		protected override bool EnableViewportControl
		{
			get { return Camera != null && Camera.ParentScene != null; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( Camera != null && Camera.ParentScene != null )
				displayPreview = true;
		}

		protected override void OnViewportCreated()
		{
			base.OnViewportCreated();

			if( displayPreview )
			{
				Viewport.AttachedScene = Camera.ParentScene;
				ViewportControl.OverrideCameraSettings = new Viewport.CameraSettingsClass( Viewport, Camera );
			}
		}

		protected override void OnViewportUpdateBegin()
		{
			base.OnViewportUpdateBegin();

			if( displayPreview )
				ViewportControl.OverrideCameraSettings = new Viewport.CameraSettingsClass( Viewport, Camera );
		}
	}
}
