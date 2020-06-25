// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Auxiliary class for Win32 API functionality.
	/// </summary>
	static class Win32Utility
	{
		[StructLayout( LayoutKind.Sequential )]
		struct SHELLEXECUTEINFO
		{
			public uint cbSize;
			public uint fMask;
			public IntPtr hwnd;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpVerb;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpFile;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpParameters;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpDirectory;
			public int nShow;
			public IntPtr hInstApp;
			public IntPtr lpIDList;
			[MarshalAs( UnmanagedType.LPWStr )]
			public string lpClass;
			public IntPtr hkeyClass;
			public uint dwHotKey;
			public IntPtr hIcon;
			public IntPtr hProcess;
		}

		const int SEE_MASK_INVOKEIDLIST = 0x0000000C;
		const int SW_SHOWNORMAL = 1;

		[DllImport( "shell32.dll", CharSet = CharSet.Unicode )]
		static extern bool ShellExecuteEx( ref SHELLEXECUTEINFO lpExecInfo );

		//

		public static void ShellExecuteEx( string verb, string realFileName )
		{
			try
			{
				SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
				info.cbSize = (uint)Marshal.SizeOf( typeof( SHELLEXECUTEINFO ) );
				info.fMask = SEE_MASK_INVOKEIDLIST;
				info.lpVerb = verb;
				info.lpFile = realFileName;
				info.nShow = SW_SHOWNORMAL;

				ShellExecuteEx( ref info );
			}
			catch( Exception )
			{
			}
		}
	}
}
