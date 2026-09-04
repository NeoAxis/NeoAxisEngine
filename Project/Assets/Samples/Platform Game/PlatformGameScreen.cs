// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	/// <summary>
	/// The GUI scene screen for Platform Game. It is based on BasicSceneScreen to enable basic functionality like inventory widget.
	/// </summary>
	public class PlatformGameScreen : BasicSceneScreen
	{
		protected override void OnTouchControlsUpdate( float delta )
		{
			//override default behavior to show touch control with None camera
			TouchControlsEnable( SystemSettings.Mobile && !GameMode.FreeCamera );

			//base.OnTouchControlsUpdate( delta );
		}
	}
}