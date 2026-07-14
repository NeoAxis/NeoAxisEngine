// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using NeoAxis.Editor;

namespace NeoAxis
{
	class PlatformSpecificUtilityUWP : PlatformSpecificUtility
	{
		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern int GetModuleFileName( IntPtr hModule, StringBuilder buffer, int length );

		[DllImport( "kernel32.dll", EntryPoint = "LoadPackagedLibrary", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true )]
		public static extern IntPtr LoadPackagedLibrary( string lpwLibFileName, uint Reserved );

		//

		public override string GetExecutableDirectoryPath()
		{
			var installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
			return installedLocation.Path;
			//// alternative:
			//string fileName = Process.GetCurrentProcess().MainModule.FileName;
			//return Path.GetDirectoryName( fileName );
		}

		//public override IntPtr LoadLibrary( string path )
		//{
		//	path = VirtualFileSystem.MakePathRelative( path );
		//	IntPtr result = LoadPackagedLibrary( path, 0 );
		//	if( result == IntPtr.Zero )
		//		Debug.Fail( "library loading error" + "\r\nError: " + DebugUtil.GetLastErrorStr() );
		//	return result;
		//}

		///////////////////////////////////////////////

		public override async Task<string> GetClipboardTextAsync()
		{
			try
			{
				var content = Clipboard.GetContent();
				if( content.Contains( StandardDataFormats.Text ) )
					return await content.GetTextAsync();
			}
			catch { }
			return "";
		}

		public override void SetClipboardText( string text )
		{
			try
			{
				var dataPackage = new DataPackage();
				dataPackage.SetText( text );
				Clipboard.SetContent( dataPackage );
			}
			catch { }
		}

		///////////////////////////////////////////////

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
		}
	}
}