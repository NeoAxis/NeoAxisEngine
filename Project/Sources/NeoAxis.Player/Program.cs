// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace NeoAxis.Player
{
	/// <summary>
	/// Defines a common code of the player application.
	/// </summary>
	public static class PlayerApp
	{
		public static bool NeedRestartApplication;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main()
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

			var isServer = SystemSettings.CommandLineParameters.TryGetValue( "-server", out var projectServer ) && projectServer == "1";

			//initialize file system of the engine
			var logFileName = "user:Logs/Player.log";
			if( isServer )
				logFileName = "user:Logs/Server.log";

			ProjectUtility.GetDefaultProjectPaths( out string projectDirectory, out string userSettingsDirectory );
			if( !VirtualFileSystem.Init( logFileName, true, projectDirectory, userSettingsDirectory ) )
				return;

			//configure general settings
			EngineApp.InitSettings.ConfigVirtualFileName = "user:Configs/Player.config";
			if( isServer )
				EngineApp.InitSettings.ConfigVirtualFileName = "user:Configs/Server.config";

			EngineApp.InitSettings.AllowChangeScreenVideoMode = true;

			//these parameters are enabled by default
			//EngineApp.EnginePauseWhenApplicationIsNotActive = false;
			//EngineApp.InitSettings.UseDirectInputForMouseRelativeMode = false;
			//EngineApp.InitSettings.AllowJoysticksAndSpecialInputDevices = false;

			//Change Floating Point Model for FPU math calculations. Default is Strict53Bits.
			//SystemSettings.FloatingPointModel = SystemSettings.FloatingPointModelEnum.Strict53Bits;

			//init engine application
			EngineApp.Init();

			//EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Maximized;
			////EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Normal;
			////var screenSize = EngineApp.GetScreenSize().ToVector2();
			////EngineApp.InitSettings.CreateWindowSize = new Vec2( screenSize.X * 0.85, screenSize.Y * 0.9 ).ToVec2I();

			//create and run application loop.
			if( EngineApp.Create() )
			{
				//configure app icon
				var iconFilePath = Path.Combine( VirtualFileSystem.Directories.EngineInternal, "App.ico" );
				if( File.Exists( iconFilePath ) )
					EngineApp.CreatedInsideEngineWindow.IconFilePath = iconFilePath;

				//run application
				EngineApp.Run();
			}

			//shutdown engine application
			EngineApp.Shutdown();
			Log.DumpToFile( "Program END\r\n" );
			VirtualFileSystem.Shutdown();

			//restart application if needed
			if( NeedRestartApplication )
				Process.Start( new ProcessStartInfo( Assembly.GetExecutingAssembly().Location, "" ) { UseShellExecute = true } );
		}
	}


	///// <summary>
	///// Defines an input point in the application.
	///// </summary>
	//static class Program
	//{
	//	/// <summary>
	//	/// The main entry point for the application.
	//	/// </summary>
	//	[STAThread]
	//	static void Main()
	//	{
	//		PlayerApp.Main();
	//	}
	//}
}
