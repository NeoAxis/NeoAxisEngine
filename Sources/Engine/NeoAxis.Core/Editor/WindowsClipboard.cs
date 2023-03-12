#if !DEPLOY
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace NeoAxis.Editor
{
	// https://github.com/svn2github/QTTabBar/blob/master/QTTabBar/Interop/ShellMethods.cs

	static class WindowsClipboard
	{
		static class PI
		{
			internal const uint CF_HDROP = 15;

			[DllImport( "User32.dll", SetLastError = true )]
			[return: MarshalAs( UnmanagedType.Bool )]
			internal static extern bool IsClipboardFormatAvailable( uint format );

			[DllImport( "User32.dll", SetLastError = true )]
			internal static extern IntPtr GetClipboardData( uint uFormat );

			[DllImport( "kernel32.dll" )]
			internal static extern IntPtr GlobalAlloc( uint uFlags, IntPtr dwBytes );

			[DllImport( "kernel32.dll", SetLastError = true )]
			internal static extern IntPtr GlobalLock( IntPtr hMem );

			[DllImport( "kernel32.dll", SetLastError = true )]
			[return: MarshalAs( UnmanagedType.Bool )]
			internal static extern bool GlobalUnlock( IntPtr hMem );

			[DllImport( "user32.dll", SetLastError = true )]
			[return: MarshalAs( UnmanagedType.Bool )]
			internal static extern bool OpenClipboard( IntPtr hWndNewOwner );

			[DllImport( "user32.dll", SetLastError = true )]
			[return: MarshalAs( UnmanagedType.Bool )]
			internal static extern bool CloseClipboard();

			[DllImport( "user32.dll", SetLastError = true )]
			internal static extern IntPtr SetClipboardData( uint uFormat, IntPtr data );

			[DllImport( "user32.dll" )]
			internal static extern bool EmptyClipboard();

			[DllImport( "Kernel32.dll", SetLastError = true )]
			internal static extern int GlobalSize( IntPtr hMem );

			[DllImport( "shell32.dll", CharSet = CharSet.Unicode )]
			internal static extern uint DragQueryFile( IntPtr hDrop, uint iFile, StringBuilder lpszFile, int cch );

			[DllImport( "user32.dll", CharSet = CharSet.Unicode )]
			internal static extern uint RegisterClipboardFormat( string lpszFormat );

			[DllImport( "user32.dll" )]
			internal static extern uint EnumClipboardFormats( uint format );

			[StructLayout( LayoutKind.Sequential, Pack = 1 )]
			public struct DROPFILES
			{
				public int pFiles;
				public Point pt;
				public bool fNC;
				public bool fWide;
			}
		}


		public static bool IsCutPrefferdDropEffect( IntPtr hwnd = default )
		{
			uint uFormat = PI.RegisterClipboardFormat( "Preferred DropEffect" );
			bool flag = false;
			if( PI.OpenClipboard( hwnd ) )
			{
				try
				{
					IntPtr clipboardData = PI.GetClipboardData( uFormat );
					if( !( clipboardData != IntPtr.Zero ) )
					{
						return flag;
					}
					IntPtr source = PI.GlobalLock( clipboardData );
					try
					{
						if( source != IntPtr.Zero )
						{
							byte[] destination = new byte[ 4 ];
							Marshal.Copy( source, destination, 0, 4 );
							flag = ( destination[ 0 ] & 2 ) != 0;
						}
					}
					finally
					{
						PI.GlobalUnlock( clipboardData );
					}
				}
				finally
				{
					PI.CloseClipboard();
				}
			}
			return flag;
		}

		public static bool ContainsFileDropPaths( IntPtr hwnd = default )
		{
			if( ( hwnd != IntPtr.Zero ) && PI.OpenClipboard( hwnd ) )
			{
				try
				{
					for( uint i = PI.EnumClipboardFormats( 0 ); i != 0; i = PI.EnumClipboardFormats( i ) )
					{
						if( i == PI.CF_HDROP )
						{
							return true;
						}
					}
				}
				finally
				{
					PI.CloseClipboard();
				}
			}
			return false;
		}

		public static List<string> GetFileDropPaths( IntPtr hwnd = default )
		{
			List<string> list = new List<string>();
			if( PI.OpenClipboard( hwnd ) )
			{
				try
				{
					IntPtr clipboardData = PI.GetClipboardData( PI.CF_HDROP );
					if( !( clipboardData != IntPtr.Zero ) )
					{
						return list;
					}
					IntPtr hDrop = PI.GlobalLock( clipboardData );
					if( !( hDrop != IntPtr.Zero ) )
					{
						return list;
					}
					try
					{
						uint num = PI.DragQueryFile( hDrop, uint.MaxValue, null, 0 );
						if( num > 0 )
						{
							for( uint i = 0; i < num; i++ )
							{
								const int MAX_PATH = 260;
								StringBuilder lpszFile = new StringBuilder( MAX_PATH );
								PI.DragQueryFile( hDrop, i, lpszFile, lpszFile.Capacity );
								if( lpszFile.Length > 0 )
								{
									list.Add( lpszFile.ToString() );
								}
							}
						}
						return list;
					}
					finally
					{
						PI.GlobalUnlock( clipboardData );
					}
				}
				finally
				{
					PI.CloseClipboard();
				}
			}
			return list;
		}

		//!!!! we have heap corruption with this method despite the fact that everything looks right.
		public static bool SetFileDropPaths( List<string> lstPaths, bool fCut, IntPtr hwnd = default )
		{
			string str = MakeFILEOPPATHS( lstPaths );
			if( /*( hwnd != IntPtr.Zero ) &&*/ ( str.Length > 1 ) )
			{
				if( !PI.OpenClipboard( hwnd ) )
				{
					return false;
				}
				PI.EmptyClipboard();
				try
				{
					PI.DROPFILES structure = new PI.DROPFILES();
					structure.pFiles = Marshal.SizeOf( structure );
					structure.fWide = true;
					int size = Marshal.SizeOf( structure ) + ( str.Length * Marshal.SystemMaxDBCSCharSize );
					IntPtr hMem = PI.GlobalAlloc( 0x42, (IntPtr)size );
					if( hMem != IntPtr.Zero )
					{
						IntPtr ptr = PI.GlobalLock( hMem );
						Marshal.StructureToPtr( structure, ptr, false );
						Marshal.Copy( str.ToCharArray(), 0, PtrPlus( ptr, Marshal.SizeOf( structure ) ), str.Length );
						PI.GlobalUnlock( hMem );
					}
					IntPtr ptr3 = PI.GlobalAlloc( 0x42, (IntPtr)4 );
					if( ptr3 != IntPtr.Zero )
					{
						IntPtr destination = PI.GlobalLock( ptr3 );
						byte[] source = new byte[ 4 ];
						source[ 0 ] = fCut ? ( (byte)2 ) : ( (byte)5 );
						Marshal.Copy( source, 0, destination, 4 );
						PI.GlobalUnlock( ptr3 );
					}
					if( ( hMem != IntPtr.Zero ) && ( ptr3 != IntPtr.Zero ) )
					{
						uint uFormat = PI.RegisterClipboardFormat( "Preferred DropEffect" );
						PI.SetClipboardData( PI.CF_HDROP, hMem );
						PI.SetClipboardData( uFormat, ptr3 );
						return true;
					}
				}
				finally
				{
					PI.CloseClipboard();
				}
			}
			return false;
		}

		public static string StringJoin<T>( this IEnumerable<T> list, string separator )
		{
			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach( T t in list )
			{
				if( first ) first = false;
				else sb.Append( separator );
				sb.Append( t.ToString() );
			}
			return sb.ToString();
		}

		static string MakeFILEOPPATHS( List<string> paths )
		{
			return paths.StringJoin( "\0" ) + "\0\0";
		}

		static IntPtr PtrPlus( IntPtr p, int cOffset )
		{
			if( IntPtr.Size == 4 )
				return (IntPtr)( ( (int)p ) + cOffset );
			return (IntPtr)( ( (long)p ) + cOffset );
		}

	}
}
#endif