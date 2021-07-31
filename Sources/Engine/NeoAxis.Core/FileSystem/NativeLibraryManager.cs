// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Helper class for loading native DLLs.
	/// </summary>
	public static class NativeLibraryManager
	{
		static object lockObject = new object();
		static Dictionary<string, int> loadedNativeLibraries = new Dictionary<string, int>();

		///////////////////////////////////////////

		[DllImport( "kernel32.dll", EntryPoint = "SetDllDirectory", CharSet = CharSet.Unicode )]
		static extern bool SetDllDirectory( string lpPathName );

		//[DllImport( "kernel32.dll", EntryPoint = "GetDllDirectory", CharSet = CharSet.Unicode )]
		//static extern int _GetDllDirectory( int size, StringBuilder buffer );

		//static string GetDllDirectory()
		//{
		//   StringBuilder b = new StringBuilder( 4096 );
		//   int res = _GetDllDirectory( 4096, b );
		//   return b.ToString();
		//}

		public static void PreLoadLibrary( string baseName, string overrideSetCurrentDirectory = "" )
		{
			lock( lockObject )
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				{
					if( Path.GetExtension( baseName ) != ".dll" )
						baseName += ".dll";
				}
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.MacOS )
				{
					//remove ".dll"
					if( Path.GetExtension( baseName ) != ".dll" )
						baseName = Path.ChangeExtension( baseName, null );

					string checkPath = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, baseName + ".bundle" );
					if( Directory.Exists( checkPath ) )
						baseName += ".bundle";
					else
						baseName += ".dylib";
				}
				else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
				{

					//no preloading on Android
					return;


					//if( Path.GetExtension( baseName ) != ".so" )
					//	baseName += ".so";

					//string prefix = "lib";
					//if( baseName.Length > 3 && baseName.Substring( 0, 3 ) == "lib" )
					//	prefix = "";
					//baseName = prefix + baseName + ".so";
				}
				else
				{
					Log.Fatal( "NativeLibraryManager: PreLoadLibrary: no code." );
				}

				if( loadedNativeLibraries.ContainsKey( baseName ) )
					return;
				loadedNativeLibraries.Add( baseName, 0 );

				string saveCurrentDirectory = Directory.GetCurrentDirectory();

				//Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, additionalPath )
				//!!!!new
				if( !string.IsNullOrEmpty( overrideSetCurrentDirectory ) )
					Directory.SetCurrentDirectory( overrideSetCurrentDirectory );
				else
					Directory.SetCurrentDirectory( VirtualFileSystem.Directories.PlatformSpecific );

				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
					SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				{
					try
					{
						SetDllDirectory( VirtualFileSystem.Directories.PlatformSpecific );
					}
					catch { }
				}

				string fullPath = Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, baseName );

				if( PlatformSpecificUtility.Instance.LoadLibrary( fullPath ) == IntPtr.Zero )
					Log.Fatal( "NativeLibraryManager: PreLoadLibrary: Loading native library failed ({0}).", fullPath );

				Directory.SetCurrentDirectory( saveCurrentDirectory );
			}
		}
	}
}
