// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.IO;
using DirectInput;

namespace NeoAxis
{
	// remove this

	partial class PlatformFunctionalityUWP : PlatformFunctionality
	{
		[DllImport( "user32.dll" )]
		static extern short GetKeyState( int nVirtKey );

		const int VK_LBUTTON = 0x01;
		const int VK_RBUTTON = 0x02;
		const int VK_MBUTTON = 0x04;
		const int VK_XBUTTON1 = 0x05;
		const int VK_XBUTTON2 = 0x06;


		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern IntPtr SetCapture( IntPtr hCursor );

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern bool ReleaseCapture();

		[DllImport( "user32.dll" )]
		static extern int ShowCursor( int show );

		[DllImport( "user32.dll" )]
		static extern bool ClipCursor( IntPtr lpRect );

	}
}
