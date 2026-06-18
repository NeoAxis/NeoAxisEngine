// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class CameraPreview : ObjectInSpacePreview
	{
		bool displayPreview;

		//

		public CameraPreview()
		{
		}

		public Camera Camera
		{
			get { return ObjectOfPreview as Camera; }
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