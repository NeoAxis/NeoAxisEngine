// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Internal;

namespace NeoAxis.Player.MacOS
{
	internal class Program
	{
		//public static bool needRestartApplication;

		static void Main( string[] args )
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

			//!!!!
			Console.WriteLine( "TEst2" );
			Thread.Sleep( 2000 );
			LogPlatformFunctionality.Instance.ShowMessageBox( "Test", "Fatal: Exception" );
			Thread.Sleep( 2000 );


			//set application type
			EngineApp.ApplicationType = EngineApp.ApplicationTypeEnum.Simulation;

			var isServer = SystemSettings.CommandLineParameters.TryGetValue( "-server", out var projectServer ) && projectServer == "1";
			//!!!!need console mode?
			//var isConsole = SystemSettings.CommandLineParameters.TryGetValue( "-console", out var console ) && console == "1";

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

			//!!!!impl
			EngineApp.InitSettings.AllowJoysticksAndSpecialInputDevices = false;

			//Change Floating Point Model for FPU math calculations. Default is Strict53Bits.
			//SystemSettings.FloatingPointModel = SystemSettings.FloatingPointModelEnum.Strict53Bits;

			//init engine application
			EngineApp.Init();

			EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Maximized;

			//create and run application loop.
			if( EngineApp.Create() )
			{
				//!!!!icon
				//configure app icon
				var iconFilePath = Path.Combine( VirtualFileSystem.Directories.EngineInternal, "App.ico" );
				if( File.Exists( iconFilePath ) )
					EngineApp.CreatedInsideEngineWindow.IconFilePath = iconFilePath;
				//EngineApp.CreatedInsideEngineWindow.Icon = NeoAxis.Player.Properties.Resources.Logo;

				//!!!!need console mode?

				EngineApp.Run();

				//from linux:

				//if( isServer )
				//	Console.WriteLine( "Server mode" );

				////console
				////if( isConsole )
				//{
				//	Console.WriteLine( "Console mode. Press ESC to stop" );
				//	do
				//	{
				//		while( !Console.KeyAvailable )
				//		{
				//			EngineApp.DoTick();
				//			if( EngineApp.Closing || EngineApp.NeedExit )
				//				break;
				//			Thread.Sleep( 1 );
				//		}
				//	} while( Console.ReadKey( true ).Key != ConsoleKey.Escape );

				//	//while( true )
				//	//{
				//	//	EngineApp.DoTick();
				//	//	if( EngineApp.Closing || EngineApp.NeedExit )
				//	//		break;

				//	//	Thread.Sleep( 1 );
				//	//}
				//}
				////else
				////{
				////	//EngineApp.Run();
				////}
			}

			EngineApp.Shutdown();

			Log.DumpToFile( "Program END\r\n" );

			VirtualFileSystem.Shutdown();

			//!!!!check
			////restart app
			//if( needRestartApplication )
			//	Process.Start( new ProcessStartInfo( Assembly.GetExecutingAssembly().Location, "" ) { UseShellExecute = true } );
		}
	}
}
