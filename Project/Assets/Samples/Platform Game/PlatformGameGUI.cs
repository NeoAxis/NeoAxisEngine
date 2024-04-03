using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	/// <summary>
	/// The GUI screen for Platform Game. It is based on BasicSceneScreen to enable basic functionality like inventory widget.
	/// </summary>
	public class PlatformGameGUI : Project.BasicSceneScreen// NeoAxis.UIControl
	{
		protected override void OnTouchControlsUpdate( float delta )
		{
			//override default behavior to show touch control with None camera
			TouchControlsEnable( SystemSettings.MobileDevice && !GameMode.FreeCamera );

			//base.OnTouchControlsUpdate( delta );
		}

		//protected override void OnEnabledInSimulation()
		//{
		//	base.OnEnabledInSimulation();
		//}

		//protected override void OnUpdate( float delta )
		//{
		//	base.OnUpdate( delta );
		//}

		//protected override bool OnTouch( TouchData e )
		//{
		//	return base.OnTouch( e );
		//}

		//protected override void OnSimulationStep()
		//{
		//	base.OnSimulationStep();
		//}
	}
}