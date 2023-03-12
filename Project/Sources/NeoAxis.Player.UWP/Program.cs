// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.IO;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;

namespace NeoAxis.Player
{
	/// <summary>
	/// Player application.
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// Defines the entry point of the application.
		/// </summary>
		[MTAThread]
		private static void Main()
		{
			//configure fullscreen mode
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

			// init non xaml app
			var appSource = new AppViewSourceUWP( Init, Run, Exit );
			CoreApplication.Run( appSource );

			//TODO: implement xaml swapChainPanel alternative.
		}

		static void Init()
		{
			new PlatformFunctionalityUWP();

			EngineApp.ApplicationType = EngineApp.ApplicationTypeEnum.Simulation;

			//get project's directories
			string projectDirectory = "";
			string userSettingsDirectory = "";
			{
				projectDirectory = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

				var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
				if( Directory.Exists( localFolder ) )
					userSettingsDirectory = Path.Combine( localFolder, "User settings" );
				else
					Log.Fatal( "Program: Init: Local folder not found: " + localFolder );
			}
			//ProjectUtility.GetDefaultProjectPaths( out string dataDirectory, out string userSettingsDirectory );

			if( !VirtualFileSystem.Init( "user:Logs/Player.log", true, projectDirectory, userSettingsDirectory ) )
				return;

			//configure general settings
			EngineApp.InitSettings.ConfigVirtualFileName = "user:Configs/Player.config";
			EngineApp.InitSettings.AllowChangeScreenVideoMode = true;
			EngineApp.InitSettings.AllowJoysticksAndSpecialInputDevices = true;
			EngineApp.InitSettings.UseDirectInputForMouseRelativeMode = false; // not implemented for UWP

			//init engine application
			EngineApp.Init();

			//fullscreen is configured in Main() method
			////configuring window size is not implemented
			////EngineApp.InitSettings.CreateWindowSize = new Vector2I( 320, 240 );
			////EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Maximized;
			////EngineApp.InitSettings.CreateWindowState = EngineApp.WindowStateEnum.Normal;
			////EngineApp.InitSettings.CreateWindowSize = ( EngineApp.GetScreenSize().ToVector2() * .8 ).ToVector2I();

			if( !EngineApp.Create() )
			{
				// TODO: implement fail logic
			}
		}

		static void Run()
		{
			EngineApp.Run();
		}

		static void Exit()
		{
			EngineApp.Shutdown();
			Log.DumpToFile( "Program END\r\n" );
			VirtualFileSystem.Shutdown();
		}
	}
}