// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NeoAxis
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

		public static LogPlatformFunctionality Get()
		{
			if( instance == null )
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.MacOS )
					instance = new LogPlatformFunctionalityMacOSX();
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
					Log.Fatal( "LogPlatformFunctionality: Get: Instance must be already initialized." );
				else
					instance = new LogPlatformFunctionalityWindows();
			}
			return instance;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

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
			while( ShowCursor( 1 ) < 0 ) { }

			IntPtr hwnd = IntPtr.Zero;
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation && EngineApp.CreatedInsideEngineWindow != null )
				hwnd = EngineApp.CreatedInsideEngineWindow.Handle;

			return (EDialogResult)MessageBox( hwnd, text, caption, (int)buttons | MB_ICONEXCLAMATION );
			//MessageBox( hwnd, text, caption, MB_OK | MB_ICONEXCLAMATION );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	class LogPlatformFunctionalityMacOSX : LogPlatformFunctionality
	{
		struct MacAppNativeWrapper
		{
			[DllImport( "NeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_MessageBox",
				CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode )]
			public static extern void MessageBox( string text, string caption );
		}

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons )
		{
			//!!!!buttons, result
			MacAppNativeWrapper.MessageBox( text, caption );
			return EDialogResult.OK;
		}
	}
}
