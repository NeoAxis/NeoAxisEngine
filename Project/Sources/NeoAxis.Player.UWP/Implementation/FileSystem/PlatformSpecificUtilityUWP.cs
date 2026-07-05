// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;

namespace NeoAxis
{
	class PlatformSpecificUtilityUWP : PlatformSpecificUtility
	{
		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern int GetModuleFileName( IntPtr hModule, StringBuilder buffer, int length );

		[DllImport( "kernel32.dll", EntryPoint = "LoadPackagedLibrary", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true )]
		public static extern IntPtr LoadPackagedLibrary( string lpwLibFileName, uint Reserved );

		//

		public PlatformSpecificUtilityUWP()
		{
			SetInstance( this );
		}

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
	}
}