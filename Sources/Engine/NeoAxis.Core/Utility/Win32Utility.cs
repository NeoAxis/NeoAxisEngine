// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if !DEPLOY
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
#endif

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for Win32 API functionality.
	/// </summary>
	public static class Win32Utility
	{
#if !DEPLOY
		[StructLayout( LayoutKind.Sequential )]
		internal/*obfuscator*/ struct SHELLEXECUTEINFO
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
		internal/*obfuscator*/ static extern bool ShellExecuteEx( ref SHELLEXECUTEINFO lpExecInfo );
#endif

		//

		public static void ShellExecuteEx( string verb, string realFileName )
		{
#if !DEPLOY
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
#endif
		}

#if !DEPLOY
		public static Bitmap ResizeImage( Image sourceImage, int destWidth, int destHeight )
		{
			var toReturn = new Bitmap( destWidth, destHeight );

			using( var graphics = Graphics.FromImage( toReturn ) )
			using( var attributes = new ImageAttributes() )
			{
				toReturn.SetResolution( sourceImage.HorizontalResolution, sourceImage.VerticalResolution );

				attributes.SetWrapMode( WrapMode.TileFlipXY );

				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.PixelOffsetMode = PixelOffsetMode.Half;
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.DrawImage( sourceImage, System.Drawing.Rectangle.FromLTRB( 0, 0, destWidth, destHeight ), 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, attributes );
			}

			return toReturn;
		}
#endif
	}
}
