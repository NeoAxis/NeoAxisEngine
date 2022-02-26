// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	public class MenuWindow : UIWindow
	{
		protected override void OnEnabledInSimulation()
		{
			if( Components[ "Button Close" ] != null )
				( (UIButton)Components[ "Button Close" ] ).Click += ButtonClose_Click;

			if( Components[ "Button Scenes" ] != null )
				( (UIButton)Components[ "Button Scenes" ] ).Click += ButtonScenes_Click;

			if( Components[ "Button Options" ] != null )
				( (UIButton)Components[ "Button Options" ] ).Click += ButtonOptions_Click;

			if( Components[ "Button Main Menu" ] != null )
				( (UIButton)Components[ "Button Main Menu" ] ).Click += ButtonMainMenu_Click;

			if( Components[ "Button Exit" ] != null )
				( (UIButton)Components[ "Button Exit" ] ).Click += ButtonExit_Click;
		}

		void ButtonClose_Click( UIButton sender )
		{
			Dispose();
		}

		void ButtonScenes_Click( UIButton sender )
		{
			var scenesWindow = ResourceManager.LoadSeparateInstance<UIWindow>( @"Base\UI\Screens\ScenesWindow.ui", false, true );
			if( scenesWindow != null )
			{
				Parent.AddComponent( scenesWindow );

				Dispose();
			}
		}

		void ButtonOptions_Click( UIButton sender )
		{
			var optionsWindow = ResourceManager.LoadSeparateInstance<UIWindow>( @"Base\UI\Screens\OptionsWindow.ui", false, true );
			if( optionsWindow != null )
			{
				Parent.AddComponent( optionsWindow );

				Dispose();
			}
		}

		void ButtonMainMenu_Click( UIButton sender )
		{
			SimulationApp.ChangeUIScreen( @"Base\UI\Screens\MainMenuScreen.ui" );
		}

		void ButtonExit_Click( UIButton sender )
		{
			EngineApp.NeedExit = true;
		}
	}
}