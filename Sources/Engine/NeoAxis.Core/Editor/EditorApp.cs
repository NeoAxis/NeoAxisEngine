// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents main application class of the editor.
	/// </summary>
	public static class EditorApp
	{
		[DllImport( "user32.dll" )]
		static extern bool SetProcessDPIAware();

		//[STAThread]
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
			if( Environment.OSVersion.Version.Major >= 6 )
			{
				try
				{
					//TODO: we can use "app.manifest" and dpiAware prop for .net < 4.7
					// eg https://medium.com/@EverydayBits/windows-forms-high-dpi-f8bbd70b4dc
					//
					//or use DpiAwareness in ApplicationConfigurationSection in  "app.config" for .net >= 4.7
					// eg https://docs.microsoft.com/ru-ru/dotnet/framework/winforms/high-dpi-support-in-windows-forms
					//
					// and we can remove API call SetProcessDPIAware after.

					SetProcessDPIAware();
				}
				catch { }
			}

			EngineApp.ApplicationType = EngineApp.ApplicationTypeEnum.Editor;

			Log.Handlers.WarningHandler += delegate ( string text, ref bool handled, ref bool dumpToLogFile )
			{
				SplashForm.Instance?.Hide();
			};
			Log.Handlers.ErrorHandler += delegate ( string text, ref bool handled, ref bool dumpToLogFile )
			{
				SplashForm.Instance?.Hide();
			};
			Log.Handlers.FatalHandler += delegate ( string text, string createdLogFilePath, ref bool handled )
			{
				SplashForm.Instance?.Hide();
			};

			//get project's directories
			ProjectUtility.GetDefaultProjectPaths( out string projectDirectory, out string userSettingsDirectory );

			if( !VirtualFileSystem.Init( "user:Logs/Editor.log", true, projectDirectory, userSettingsDirectory ) )
				return;

			//configure general settings
			EngineApp.InitSettings.ConfigVirtualFileName = "user:Configs/Editor.config";

			//!!!!не было
			//!!!!теперь true по дефолту
			//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
			//	EngineApp.InitSettings.UseDirectInputForMouseRelativeMode = true;

			//!!!!не включать по дефолту. но нужно раскомментить
			//EngineApp.InitSettings.AllowJoysticksAndSpecialInputDevices= true;
			//!!!!!
			//EngineApp.InitSettings.AllowWriteEngineConfigFile = true;
			//EngineApp.InitSettings.AllowChangeScreenVideoMode = true;
			//Change Floating Point Model for FPU math calculations. Default is Strict53Bits.
			//FloatingPointModel.Model = FloatingPointModel.Models.Strict53Bits;

			EngineApp.Init();
			//EngineApp.Init( new EngineApp() );// EngineApp.ApplicationTypes.Editor ) );
			//EngineApp.Init( new SimulationApp() );

			//!!!!всем в сборке зарегать прост?
			//enable support field and properties serialization for GameEngineApp class.
			//EngineApp.Instance.Config.RegisterClassParameters( typeof( SimulationApp ) );

			//update window
			//!!!!
			//EngineApp.Instance.WindowTitle = "Game";
			//!!!!!чуть ниже стало
			//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
			//	EngineApp.Instance.CreatedInsideEngineWindow.Icon = NeoAxis.Game.Properties.Resources.Logo;

			//EngineApp.Instance.SuspendWorkingWhenApplicationIsNotActive = false;

			//!!!!!
			//create and run application loop.
			//if( EngineApp.Instance.Create() )
			//{
			//	//!!!!!
			//	//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
			//	//	EngineApp.Instance.CreatedInsideEngineWindow.Icon = NeoAxis.Game.Properties.Resources.Logo;

			//	//EngineApp.Instance.Run();
			//}
			//else
			//{
			//	//!!!!!
			//}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			EditorForm form = new EditorForm();
			form.Show();
			while( form.Created )
			{
				//!!!!так?

				Application.DoEvents();

				if( EditorForm.Instance == null || EngineApp.Instance == null || EngineApp.Closing )
					break;

				EditorForm.Instance.RenderViewports( out bool existActiveViewports );
				if( !existActiveViewports && EngineApp.Instance != null )
					EngineApp.MessageLoopWaitMessage();

				//bool allowRender = MainForm.Instance.Visible &&
				//	MainForm.Instance.WindowState != FormWindowState.Minimized &&
				//	MainForm.Instance.IsAllowRenderScene();

				//if( allowRender )
				//	form.RenderScene();
				//else
				//	EngineApp.Instance.MessageLoopWaitMessage();
			}
			//Application.Run( new EditorForm() );

			EngineApp.Shutdown();
			Log.DumpToFile( "Program END\r\n" );
			VirtualFileSystem.Shutdown();

			EditorAPI.GetRestartApplication( out var needRestart, out _ );
			if( needRestart )
				Application.Restart();
			else
			{
				//!!!!
				//bug fix for ".NET-BroadcastEventWindow" error
				Application.Exit();
			}

			//	//Mono check
			//	if( RuntimeFramework.Runtime == RuntimeFramework.RuntimeType.Mono )
			//	{
			//		string text = "The Map Editor does not work correctly on the Mono Runtime.\n\nContinue?";
			//		if( MessageBox.Show( text, "Map Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.No )
			//			return;
			//	}

			//	if( !VirtualFileSystem.Init( "user:Logs/MapEditor.log", true, null, null, null, null ) )
			//		return;

			//	Log.Handlers.InfoHandler += delegate( string text, ref bool dumpToLogFile )
			//	{
			//		if( MapEditorEngineApp.Instance != null )
			//			MapEditorEngineApp.Instance.AddScreenMessage( text );
			//	};

			//	Log.Handlers.WarningHandler += delegate( string text, ref bool handled, ref bool dumpToLogFile )
			//	{
			//		handled = true;

			//		if( SplashForm.Instance != null )
			//			SplashForm.Instance.Hide();
			//		if( EngineApp.Instance != null )
			//			EngineApp.Instance.ShowSystemCursor = true;

			//		string caption = ToolsLocalization.Translate( "Various", "Warning" );
			//		MessageBox.Show( text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning );
			//	};

			//	Log.Handlers.ErrorHandler += delegate( string text, ref bool handled, ref bool dumpToLogFile )
			//	{
			//		handled = true;

			//		if( SplashForm.Instance != null )
			//			SplashForm.Instance.Hide();
			//		if( EngineApp.Instance != null )
			//			EngineApp.Instance.ShowSystemCursor = true;

			//		string caption = ToolsLocalization.Translate( "Various", "Error" );
			//		MessageBox.Show( text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning );
			//	};

			//	Log.Handlers.FatalHandler += delegate( string text, string createdLogFilePath, ref bool handled )
			//	{
			//		if( SplashForm.Instance != null )
			//			SplashForm.Instance.Hide();
			//		if( EngineApp.Instance != null )
			//			EngineApp.Instance.ShowSystemCursor = true;
			//	};

			//	Application.EnableVisualStyles();
			//	Application.SetCompatibleTextRenderingDefault( false );

			//	EngineApp.ConfigName = "user:Configs/MapEditor.config";

			//	EngineApp.Init( new MapEditorEngineApp() );

			//	//Do message loop

			//	MainForm form = new MainForm();
			//	form.Show();
			//	while( form.Created )
			//	{
			//		Application.DoEvents();

			//		if( MainForm.Instance == null )
			//			break;

			//		bool allowRender = MainForm.Instance.Visible &&
			//			MainForm.Instance.WindowState != FormWindowState.Minimized &&
			//			MainForm.Instance.IsAllowRenderScene();

			//		if( allowRender )
			//			form.RenderScene();
			//		else
			//			EngineApp.Instance.MessageLoopWaitMessage();
			//	}
			//}

		}
	}
}
