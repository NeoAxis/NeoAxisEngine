// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace NeoAxis
{
	/// <summary>
	/// Internal class for implementing the target platform.
	/// </summary>
	public abstract class PlatformSpecificUtility
	{
		static PlatformSpecificUtility instance;

		protected void SetInstance( PlatformSpecificUtility instance )
		{
			PlatformSpecificUtility.instance = instance;
		}

		public abstract string GetExecutableDirectoryPath();
		public abstract IntPtr LoadLibrary( string path );

		public static PlatformSpecificUtility Instance
		{
			get
			{
				if( instance == null )
				{
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.MacOS )
						instance = new MacOSXPlatformSpecificUtility();
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else
						instance = new WindowsPlatformSpecificUtility();
				}
				return instance;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	class WindowsPlatformSpecificUtility : PlatformSpecificUtility
	{
		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern int GetModuleFileName( IntPtr hModule, StringBuilder buffer, int length );

		[DllImport( "kernel32.dll", EntryPoint = "LoadLibrary", CharSet = CharSet.Unicode )]
		static extern IntPtr Win32LoadLibrary( string lpLibFileName );

		public override string GetExecutableDirectoryPath()
		{
			try
			{
				string fileName = Process.GetCurrentProcess().MainModule.FileName;
				return Path.GetDirectoryName( fileName );
			}
			catch
			{
				//old implementation
				//really need this code?
				var module = Assembly.GetExecutingAssembly().GetModules()[ 0 ];
				IntPtr hModule = Marshal.GetHINSTANCE( module );
				if( hModule == new IntPtr( -1 ) )
					hModule = IntPtr.Zero;
				StringBuilder buffer = new StringBuilder( 260 );
				int length = GetModuleFileName( hModule, buffer, buffer.Capacity );
				return Path.GetDirectoryName( Path.GetFullPath( buffer.ToString() ) );
			}
		}

		public override IntPtr LoadLibrary( string path )
		{
			return Win32LoadLibrary( path );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	class MacOSXPlatformSpecificUtility : PlatformSpecificUtility
	{
		[DllImport( "NeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_LoadLibrary",
			CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode )]
		public static extern IntPtr MacLoadLibrary( string name );

		public override string GetExecutableDirectoryPath()
		{
			//old: GetCallingAssembly
			string codeBaseURI = Assembly.GetExecutingAssembly().CodeBase;
			return Path.GetDirectoryName( codeBaseURI.Replace( "file://", "" ) );
		}

		public override IntPtr LoadLibrary( string path )
		{
			return MacLoadLibrary( path );
		}
	}
}
