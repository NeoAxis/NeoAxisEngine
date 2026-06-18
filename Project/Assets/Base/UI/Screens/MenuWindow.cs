// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	public class MenuWindow : UIWindow
	{
		public static bool EnableButtonScenes { get; set; } = true;

		static MenuWindow instance;

		//

		UIButton GetButtonClose() { return GetComponent<UIButton>( "Button Close" ); }
		UIButton GetButtonScenes() { return GetComponent<UIButton>( "Button Scenes" ); }
		UIButton GetButtonOptions() { return GetComponent<UIButton>( "Button Options" ); }
		UIButton GetButtonMainMenu() { return GetComponent<UIButton>( "Button Main Menu" ); }
		UIButton GetButtonExit() { return GetComponent<UIButton>( "Button Exit" ); }

		//

		public static MenuWindow Instance
		{
			get { return instance; }
		}

		protected override void OnEnabledInSimulation()
		{
			instance = this;

			var networkClientByCommandLine = false;
			if( SystemSettings.CommandLineParameters.TryGetValue( "-client", out var isClient ) && isClient == "1" )
				networkClientByCommandLine = true;

			if( GetButtonClose() != null )
				GetButtonClose().Click += ButtonClose_Click;

			if( GetButtonScenes() != null )
			{
				if( EnableButtonScenes )
				{
					GetButtonScenes().Click += ButtonScenes_Click;
					GetButtonScenes().ReadOnly = networkClientByCommandLine;
				}
				else
					GetButtonScenes().Enabled = false;
			}

			if( GetButtonOptions() != null )
				GetButtonOptions().Click += ButtonOptions_Click;

			if( GetButtonMainMenu() != null )
			{
				GetButtonMainMenu().Click += ButtonMainMenu_Click;
				GetButtonMainMenu().ReadOnly = networkClientByCommandLine;
			}

			if( GetButtonExit() != null )
				GetButtonExit().Click += ButtonExit_Click;

			//fix size of the window
			if( !EnableButtonScenes )
			{
				var size = Size.Value;
				size.Y -= 60;
				Size = size;
			}
		}

		protected override void OnDisabledInSimulation()
		{
			base.OnDisabledInSimulation();

			instance = null;
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
			SimulationApp.ChangeUIScreen( @"Base\UI\Screens\MainMenuScreen.ui", true );
		}

		void ButtonExit_Click( UIButton sender )
		{
			EngineApp.NeedExit = true;
		}
	}
}