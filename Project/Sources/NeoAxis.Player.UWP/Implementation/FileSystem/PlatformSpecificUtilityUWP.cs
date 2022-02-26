// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Internal;

namespace NeoAxis
{
	class PlatformSpecificUtilityUWP : PlatformSpecificUtility
	{
		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern int GetModuleFileName( IntPtr hModule, StringBuilder buffer, int length );

		[DllImport( "kernel32.dll", EntryPoint = "LoadPackagedLibrary", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true )]
		public static extern IntPtr LoadPackagedLibrary( string lpwLibFileName, uint Reserved );

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

		public override IntPtr LoadLibrary( string path )
		{
			path = VirtualFileSystem.MakePathRelative( path );
			IntPtr result = LoadPackagedLibrary( path, 0 );
			if( result == IntPtr.Zero )
				Debug.Fail( "library loading error" + "\r\nError: " + DebugUtil.GetLastErrorStr() );
			return result;
		}
	}
}
