// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace NeoAxis.Player
{
	/// <summary>
	/// Defines an input point in the application.
	/// </summary>
	public static class Program
	{
		public static bool needRestartApplication;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			if( Debugger.IsAttached )
			{
				Main2();
			}
			else
			{
				try
				{
					Main2();
				}
				catch( Exception e )
				{
					Log.FatalAsException( e.ToString() );
				}
			}
		}

		static void Main2()
		{
			//set application type
			EngineApp.ApplicationType = EngineApp.ApplicationTypeEnum.Simulation;

			//initialize file system of the engine
			ProjectUtility.GetDefaultProjectPaths( out string projectDirectory, out string userSettingsDirectory );
			if( !VirtualFileSystem.Init( "user:Logs/Player.log", true, projectDirectory, userSettingsDirectory ) )
				return;

			//configure general settings
			EngineApp.InitSettings.ConfigVirtualFileName = "user:Configs/Player.config";

			EngineApp.InitSettings.AllowChangeScreenVideoMode = true;

			//these parameters are enabled by default
			//EngineApp.EnginePauseWhenApplicationIsNotActive = false;
			//EngineApp.InitSettings.UseDirectInputForMouseRelativeMode = false;
			//EngineApp.InitSettings.AllowJoysticksAndSpecialInputDevices = false;

			//Change Floating Point Model for FPU math calculations. Default is Strict53Bits.
			//SystemSettings.FloatingPointModel = SystemSettings.FloatingPointModelEnum.Strict53Bits;

			//init engine application
			EngineApp.Init();

			EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Maximized;
			//EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Normal;
			//var screenSize = EngineApp.GetScreenSize().ToVec2();
			//EngineApp.InitSettings.CreateWindowSize = new Vec2( screenSize.X * 0.85, screenSize.Y * 0.9 ).ToVec2I();

			//create and run application loop.
			if( EngineApp.Create() )
			{
				EngineApp.CreatedInsideEngineWindow.Icon = NeoAxis.Player.Properties.Resources.Logo;

				EngineApp.Run();
			}

			EngineApp.Shutdown();

			Log.DumpToFile( "Program END\r\n" );

			VirtualFileSystem.Shutdown();

			if( needRestartApplication )
				Process.Start( Assembly.GetExecutingAssembly().Location, "" );
		}
	}
}
