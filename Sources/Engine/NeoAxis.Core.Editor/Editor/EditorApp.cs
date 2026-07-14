// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents main application class of the editor.
	/// </summary>
	public static class EditorApp
	{
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
			//#if !DEPLOY
			//			if( EditorCommandLineTools.Process() )
			//				return;
			//#endif

			Application.SetHighDpiMode( HighDpiMode.SystemAware );

			Platforms.Initialize();
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

			EngineApp.Init();

			//command line parameters
			{
				//rendererBackend
				{
					if( SystemSettings.CommandLineParameters.TryGetValue( "-rendererBackend", out var str ) )
					{
						try
						{
							EngineApp.InitSettings.RendererBackend = (Internal.SharpBgfx.RendererBackend)Enum.Parse( typeof( Internal.SharpBgfx.RendererBackend ), str );

							//!!!!temp consider Vulkan as limited device
							if( EngineApp.InitSettings.RendererBackend == Internal.SharpBgfx.RendererBackend.Vulkan )
								SystemSettings._UpdateDeviceProperties();
						}
						catch { }
					}
				}

				//soundSystem
				{
					if( SystemSettings.CommandLineParameters.TryGetValue( "-soundSystem", out var str ) )
						EngineApp.InitSettings.SoundSystem = str;
				}
			}

			Application.SetCompatibleTextRenderingDefault( false );

			var form = new EditorForm();
			form.Show();
			while( form.Created )
			{
				EditorAPI2.ApplicationDoEvents( false );

				if( EditorForm.Instance == null || EngineApp.Instance == null || EngineApp.Closing )
					break;

				EditorForm.Instance.RenderViewports( out bool existActiveViewports );
				if( !existActiveViewports && EngineApp.Instance != null )
					EngineApp.MessageLoopWaitMessage();
			}

			EngineApp.Shutdown();
			Log.DumpToFile( "Program END\r\n" );
			VirtualFileSystem.Shutdown();

			EditorAPI2.GetRestartApplication( out var needRestart, out _ );
			if( needRestart )
			{
				string fullPath = Process.GetCurrentProcess().MainModule.FileName;
				Process.Start( new ProcessStartInfo( fullPath ) { UseShellExecute = true } );
				Thread.Sleep( 1000 );
			}

			//prevent internal exception and freeze on exit
			try
			{
				Process.GetCurrentProcess().Kill();
			}
			catch { }
		}
	}
}
