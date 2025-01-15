// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using NeoAxis;
using NeoAxis.Editor;
using System.Diagnostics;

namespace Internal//NeoAxis
{
	/// <summary>
	/// Internal class for implementing the target platform.
	/// </summary>
	public abstract class LogPlatformFunctionality
	{
		static LogPlatformFunctionality instance;

		//

		protected void SetInstance( LogPlatformFunctionality instance )
		{
			LogPlatformFunctionality.instance = instance;
		}

		public abstract EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons = EMessageBoxButtons.OK );

		public static LogPlatformFunctionality Instance
		{
			get
			{
				if( instance == null )
				{
#if !WEB
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
						instance = new LogPlatformFunctionalityMacOS();
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
						Log.Fatal( "LogPlatformFunctionality: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
						Log.Fatal( "LogPlatformFunctionality: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
						Log.Fatal( "LogPlatformFunctionality: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
						instance = new LogPlatformFunctionalityLinux();
					else
					{
						//#if WINDOWS || UWP
						instance = new LogPlatformFunctionalityWindows();
						//#endif
					}
#else
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
						Log.Fatal( "LogPlatformFunctionality: Get: Instance must be already initialized." );
#endif
				}
				return instance;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

#if !WEB
	//#if WINDOWS || UWP
	class LogPlatformFunctionalityWindows : LogPlatformFunctionality
	{
		[DllImport( "user32.dll" )]
		static extern int ShowCursor( int show );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
		static extern int MessageBox( IntPtr hWnd, string text, string caption, int type );
		const int MB_OK = 0x00000000;
		const int MB_ICONEXCLAMATION = 0x00000030;

		///////////////////////////////////////////

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons )
		{
			{
				var counter = 0;
				while( ShowCursor( 1 ) < 0 && counter < 100 ) { counter++; }
			}

			if( EngineApp.IsEditor )
			{
				if( buttons != EMessageBoxButtons.OK )
					return EditorMessageBox.ShowQuestion( text, buttons, caption );
				else
				{
					EditorMessageBox.ShowWarning( text, caption );
					return EDialogResult.OK;
				}
			}
			else
			{
				IntPtr hwnd = IntPtr.Zero;
				if( EngineApp.IsSimulation && EngineApp.CreatedInsideEngineWindow != null )
					hwnd = EngineApp.CreatedInsideEngineWindow.Handle;
				if( EngineApp.IsEditor && Process.GetCurrentProcess().MainWindowHandle != IntPtr.Zero )
					hwnd = Process.GetCurrentProcess().MainWindowHandle;

				return (EDialogResult)MessageBox( hwnd, text, caption, (int)buttons | MB_ICONEXCLAMATION );
			}


			//if( EngineApp.IsEditor )
			//{
			//	if( buttons != EMessageBoxButtons.OK )
			//		return EditorMessageBox.ShowQuestion( text, buttons, caption );
			//	else
			//	{
			//		EditorMessageBox.ShowWarning( text, caption );
			//		return EDialogResult.OK;
			//	}
			//}
			//else
			//{
			//	while( ShowCursor( 1 ) < 0 ) { }

			//	IntPtr hwnd = IntPtr.Zero;
			//	if( EngineApp.IsSimulation && EngineApp.CreatedInsideEngineWindow != null )
			//		hwnd = EngineApp.CreatedInsideEngineWindow.Handle;
			//	if( EngineApp.IsEditor && Process.GetCurrentProcess().MainWindowHandle != IntPtr.Zero )
			//		hwnd = Process.GetCurrentProcess().MainWindowHandle;

			//	return (EDialogResult)MessageBox( hwnd, text, caption, (int)buttons | MB_ICONEXCLAMATION );
			//	//MessageBox( hwnd, text, caption, MB_OK | MB_ICONEXCLAMATION );
			//}
		}
	}
	//#endif
#endif

	////////////////////////////////////////////////////////////////////////////////////////////////

	class LogPlatformFunctionalityLinux : LogPlatformFunctionality
	{
		///////////////////////////////////////////

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons )
		{
			//!!!!buttons, result

			Console.WriteLine( "MESSAGE:\n" + caption + ":" + text );

			//Android.Util.Log.WriteLine( Android.Util.LogPriority.Debug, "MyApp", "MESSAGE:\r\n" + caption + ":" + text );

			//while( ShowCursor( 1 ) < 0 ) { }

			//IntPtr hwnd = IntPtr.Zero;
			//if( EngineApp.IsSimulation && EngineApp.CreatedInsideEngineWindow != null )
			//	hwnd = EngineApp.CreatedInsideEngineWindow.Handle;

			//return (EDialogResult)MessageBox( hwnd, text, caption, (int)buttons | MB_ICONEXCLAMATION );

			return EDialogResult.OK;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

#if !WEB
	class LogPlatformFunctionalityMacOS : LogPlatformFunctionality
	{
		struct MacAppNativeWrapper
		{
			[DllImport( "NeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_MessageBox", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode )]
			public static extern void MessageBox( string text, string caption );
		}

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons )
		{
			Console.WriteLine( "MESSAGE:\n" + caption + ":" + text );

			//!!!!buttons, result
			MacAppNativeWrapper.MessageBox( text, caption );
			return EDialogResult.OK;
		}
	}
#endif
}
