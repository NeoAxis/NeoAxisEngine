// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using NeoAxis.Editor;

namespace NeoAxis.Widget
{
	/// <summary>
	/// Class for initialization an engine widget for Windows Forms and WPF.
	/// </summary>
	public static class WidgetApp
	{
		[DllImport( "user32.dll" )]
		static extern bool SetProcessDPIAware();

		//[STAThread]
		public static void WinFormsMain( Type appMainFormType, bool callSetProcessDPIAware, string logFileName, string configFileName )
		{
			if( Debugger.IsAttached )
			{
				Main2( appMainFormType, callSetProcessDPIAware, logFileName, configFileName );
			}
			else
			{
				try
				{
					Main2( appMainFormType, callSetProcessDPIAware, logFileName, configFileName );
				}
				catch( Exception e )
				{
					Log.FatalAsException( e.ToString() );
				}
			}
		}

		static void Main2( Type appMainFormType, bool callSetProcessDPIAware, string logFileName, string configFileName )
		{
			if( callSetProcessDPIAware && Environment.OSVersion.Version.Major >= 6 )
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

			EngineApp.ApplicationType = EngineApp.ApplicationTypeEnum.Simulation;

			//get project's directories
			ProjectUtility.GetDefaultProjectPaths( out string projectDirectory, out string userSettingsDirectory );

			if( !VirtualFileSystem.Init( logFileName, true, projectDirectory, userSettingsDirectory ) )
				return;

			//configure general settings
			EngineApp.InitSettings.ConfigVirtualFileName = configFileName;

			EngineApp.Init();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			var form = (Form)appMainFormType.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );
			form.Show();
			while( form.Created )
			{
				Application.DoEvents();

				if( /*WidgetAppForm.Instance == null ||*/ EngineApp.Instance == null || EngineApp.Closing )
					break;

				RenderViewports( form, out bool existActiveViewports );
				if( !existActiveViewports && EngineApp.Instance != null )
					EngineApp.MessageLoopWaitMessage();
			}
			//Application.Run( form );

			EngineApp.Shutdown();
			Log.DumpToFile( "Program END\r\n" );
			VirtualFileSystem.Shutdown();

			//!!!!
			//bug fix for ".NET-BroadcastEventWindow" error
			Application.Exit();
		}

		static void RenderViewports( Form form, out bool existActiveViewports )
		{
			existActiveViewports = false;

			//!!!!каким-то не нужно часто обновляться

			if( form.Visible && form.WindowState != FormWindowState.Minimized )
			{
				//get available controls
				List<EngineViewportControl> toRender = new List<EngineViewportControl>();
				List<EngineViewportControl> unvisible = new List<EngineViewportControl>();

				foreach( var control in EngineViewportControl.AllInstances )
				{
					if( control.IsAllowRender() )
					{
						if( control.AutomaticUpdateFPS != 0 )
							toRender.Add( control );
					}
					else
						unvisible.Add( control );
				}

				bool callFrame = false;

				//destroy render targets for unvisible controls
				foreach( var control in unvisible )
				{
					var context = control.Viewport?.RenderingContext;
					if( context != null )
					{
						if( context.DynamicTexturesAreExists() )
						{
							context.MultiRenderTarget_DestroyAll();
							context.DynamicTexture_DestroyAll();

							callFrame = true;
						}
					}
				}

				if( callFrame )
				{
					RenderingSystem.CallBgfxFrame();
					RenderingSystem.CallBgfxFrame();
				}

				//render
				if( toRender.Count != 0 )
				{
					existActiveViewports = true;

					foreach( var control in toRender )
						control.TryRender();
				}
			}
		}

		/////////////////////////////////////////

		public static void WPFInit( string logFileName, string configFileName )
		{
			EngineApp.ApplicationType = EngineApp.ApplicationTypeEnum.Simulation;

			//get project's directories
			ProjectUtility.GetDefaultProjectPaths( out string projectDirectory, out string userSettingsDirectory );

			if( !VirtualFileSystem.Init( logFileName, true, projectDirectory, userSettingsDirectory ) )
				return;

			//configure general settings
			EngineApp.InitSettings.ConfigVirtualFileName = configFileName;

			EngineApp.Init();
		}

		public static void WPFShutdown()
		{
			try
			{
				foreach( var control in WidgetControlWPF.AllInstances.ToArray() )
				{
					if( control.InnerControl != null && !control.InnerControl.Destroyed )
					{
						control.InnerControl.AllowCreateRenderWindow = false;
						control.InnerControl.DestroyRenderTarget();
					}
				}
			}
			catch { }

			EngineApp.Shutdown();
			Log.DumpToFile( "Program END\r\n" );
			VirtualFileSystem.Shutdown();

			////bug fix for ".NET-BroadcastEventWindow" error
			//Application.Exit();
		}
	}
}
