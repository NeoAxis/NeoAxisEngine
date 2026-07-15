// Copyright 2006–2026 Ivan Efimov. All rights reserved.

//The MIT License (MIT)
//Copyright( c ).NET Foundation and Contributors

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using NeoAxis;
using NeoAxis.Editor;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.Buffers;
using System.Security;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Internal
{
	class PlatformSpecificUtilityWindows : PlatformSpecificUtility
	{
		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern int GetModuleFileName( IntPtr hModule, StringBuilder buffer, int length );

		//[DllImport( "kernel32.dll", EntryPoint = "LoadLibrary", CharSet = CharSet.Unicode )]
		//static extern IntPtr Win32LoadLibrary( string lpLibFileName );

		[DllImport( "kernel32.dll" )]
		static extern uint GetLastError();

		public override string GetExecutableDirectoryPath()
		{
			var result = "";

			try
			{
				var fileName = Process.GetCurrentProcess().MainModule.FileName;
				result = Path.GetDirectoryName( fileName );
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
				result = Path.GetDirectoryName( Path.GetFullPath( buffer.ToString() ) );
			}

			result = VirtualPathUtility.NormalizePath( result );

			//when run by means built-in dotnet.exe from NeoAxis.Internal
			{
				var remove = VirtualPathUtility.NormalizePath( @"\NeoAxis.Internal\Platforms\Windows\dotnet" );
				var index = result.IndexOf( remove );
				if( index != -1 )
					result = result.Remove( index, remove.Length );
			}

			return result;
		}

		//public override IntPtr LoadLibrary( string path )
		//{
		//	return Win32LoadLibrary( path );
		//}

		///////////////////////////////////////////////

		//code from TextCopy library (MIT)

		static class WindowsClipboard
		{
			public static async Task SetTextAsync( string text, CancellationToken cancellation )
			{
				await TryOpenClipboardAsync( cancellation );

				InnerSet( text );
			}

			public static void SetText( string text )
			{
				TryOpenClipboard();

				InnerSet( text );
			}

			static void InnerSet( string text )
			{
				EmptyClipboard();
				IntPtr hGlobal = default;
				try
				{
					var bytes = ( text.Length + 1 ) * 2;
					hGlobal = Marshal.AllocHGlobal( bytes );

					if( hGlobal == default )
					{
						ThrowWin32();
					}

					var target = GlobalLock( hGlobal );

					if( target == default )
					{
						ThrowWin32();
					}

					try
					{
						Marshal.Copy( text.ToCharArray(), 0, target, text.Length );
					}
					finally
					{
						GlobalUnlock( target );
					}

					if( SetClipboardData( cfUnicodeText, hGlobal ) == default )
					{
						ThrowWin32();
					}

					hGlobal = default;
				}
				finally
				{
					if( hGlobal != default )
					{
						Marshal.FreeHGlobal( hGlobal );
					}

					CloseClipboard();
				}
			}

			static async Task TryOpenClipboardAsync( CancellationToken cancellation )
			{
				var num = 10;
				while( true )
				{
					if( OpenClipboard( default ) )
					{
						break;
					}

					if( --num == 0 )
					{
						ThrowWin32();
					}

					await Task.Delay( 100, cancellation );
				}
			}

			static void TryOpenClipboard()
			{
				var num = 10;
				while( true )
				{
					if( OpenClipboard( default ) )
					{
						break;
					}

					if( --num == 0 )
					{
						ThrowWin32();
					}

					Thread.Sleep( 100 );
				}
			}

			public static async Task<string?> GetTextAsync( CancellationToken cancellation )
			{
				if( !IsClipboardFormatAvailable( cfUnicodeText ) )
				{
					return null;
				}
				await TryOpenClipboardAsync( cancellation );

				return InnerGet();
			}

			public static string? GetText()
			{
				if( !IsClipboardFormatAvailable( cfUnicodeText ) )
				{
					return null;
				}
				TryOpenClipboard();

				return InnerGet();
			}

			static string? InnerGet()
			{
				IntPtr handle = default;

				IntPtr pointer = default;
				try
				{
					handle = GetClipboardData( cfUnicodeText );
					if( handle == default )
					{
						return null;
					}

					pointer = GlobalLock( handle );
					if( pointer == default )
					{
						return null;
					}

					var size = GlobalSize( handle );
					var buff = new byte[ size ];

					Marshal.Copy( pointer, buff, 0, size );

					return Encoding.Unicode.GetString( buff ).TrimEnd( '\0' );
				}
				finally
				{
					if( pointer != default )
					{
						GlobalUnlock( handle );
					}

					CloseClipboard();
				}
			}

			const uint cfUnicodeText = 13;

			static void ThrowWin32()
			{
				throw new Win32Exception( Marshal.GetLastWin32Error() );
			}

			[DllImport( "User32.dll", SetLastError = true )]
			[return: MarshalAs( UnmanagedType.Bool )]
			static extern bool IsClipboardFormatAvailable( uint format );

			[DllImport( "User32.dll", SetLastError = true )]
			static extern IntPtr GetClipboardData( uint uFormat );

			[DllImport( "kernel32.dll", SetLastError = true )]
			static extern IntPtr GlobalLock( IntPtr hMem );

			[DllImport( "kernel32.dll", SetLastError = true )]
			[return: MarshalAs( UnmanagedType.Bool )]
			static extern bool GlobalUnlock( IntPtr hMem );

			[DllImport( "user32.dll", SetLastError = true )]
			[return: MarshalAs( UnmanagedType.Bool )]
			static extern bool OpenClipboard( IntPtr hWndNewOwner );

			[DllImport( "user32.dll", SetLastError = true )]
			[return: MarshalAs( UnmanagedType.Bool )]
			static extern bool CloseClipboard();

			[DllImport( "user32.dll", SetLastError = true )]
			static extern IntPtr SetClipboardData( uint uFormat, IntPtr data );

			[DllImport( "user32.dll" )]
			static extern bool EmptyClipboard();

			[DllImport( "Kernel32.dll", SetLastError = true )]
			static extern int GlobalSize( IntPtr hMem );
		}


		public async override Task<string> GetClipboardTextAsync()
		{
			try
			{
				var cts = new CancellationTokenSource( 1000 );
				return await WindowsClipboard.GetTextAsync( cts.Token );
			}
			catch
			{
				return "";
			}
		}

		public override void SetClipboardText( string text )
		{
			try
			{
				Task.Run( async delegate ()
				{
					try
					{
						var cts = new CancellationTokenSource( 1000 );
						await WindowsClipboard.SetTextAsync( text, cts.Token );
					}
					catch { }
				} );
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


			//if( EngineApp.IsEditor )
			//{
			//	if( buttons != EMessageBoxButtons.OK )
			//		return EditorMessageBox.ShowQuestion( text, buttons, caption );
			//	else
			//	{
			//		EditorMessageBox.ShowWarning( text, caption );
			//		return EDialogResult.OK;
			//	}
			//}
			//else
			//{
			//	while( ShowCursor( 1 ) < 0 ) { }

			//	IntPtr hwnd = IntPtr.Zero;
			//	if( EngineApp.IsSimulation && EngineApp.CreatedInsideEngineWindow != null )
			//		hwnd = EngineApp.CreatedInsideEngineWindow.Handle;
			//	if( EngineApp.IsEditor && Process.GetCurrentProcess().MainWindowHandle != IntPtr.Zero )
			//		hwnd = Process.GetCurrentProcess().MainWindowHandle;

			//	return (EDialogResult)MessageBox( hwnd, text, caption, (int)buttons | MB_ICONEXCLAMATION );
			//	//MessageBox( hwnd, text, caption, MB_OK | MB_ICONEXCLAMATION );
			//}
		}

		///////////////////////////////////////////////

		static class TrayIcon
		{
			//!!!!make better API

			const string library = "libNeoAxisCoreNative";

			//public delegate void ClickHandler( IntPtr obj );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuCreate( IntPtr hIcon, [MarshalAs( UnmanagedType.LPWStr )] string tip, TrayClickHandler onClick, TrayClickHandler onDoubleClick, out IntPtr pInstance );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuRelease( IntPtr pInstance );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuShow( IntPtr pInstance );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuClose( IntPtr pInstance );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuAdd( IntPtr pInstance, IntPtr pTrayMenuItem );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuRemove( IntPtr pInstance, IntPtr pTrayMenuItem );


			//public delegate void OnClicked( IntPtr sender, uint id );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuItemCreate( TrayItemOnClickedHandler onClicked, out IntPtr pInstance );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuItemRelease( ref IntPtr pInstance );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuItemContent( IntPtr instance, [MarshalAs( UnmanagedType.LPWStr )] string value );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuItemIsChecked( IntPtr instance, bool value );
		}

		//////////////////////

		public override int TrayMenuCreate( IntPtr hIcon, string tip, TrayClickHandler onClick, TrayClickHandler onDoubleClick, out IntPtr pInstance )
		{
			return TrayIcon.TrayMenuCreate( hIcon, tip, onClick, onDoubleClick, out pInstance );
		}
		public override int TrayMenuRelease( IntPtr pInstance ) { return TrayIcon.TrayMenuRelease( pInstance ); }
		public override int TrayMenuShow( IntPtr pInstance ) { return TrayIcon.TrayMenuShow( pInstance ); }
		public override int TrayMenuClose( IntPtr pInstance ) { return TrayIcon.TrayMenuClose( pInstance ); }
		public override int TrayMenuAdd( IntPtr pInstance, IntPtr pTrayMenuItem ) { return TrayIcon.TrayMenuAdd( pInstance, pTrayMenuItem ); }
		public override int TrayMenuRemove( IntPtr pInstance, IntPtr pTrayMenuItem ) { return TrayIcon.TrayMenuRemove( pInstance, pTrayMenuItem ); }
		public override int TrayMenuItemCreate( TrayItemOnClickedHandler onClicked, out IntPtr pInstance ) { return TrayIcon.TrayMenuItemCreate( onClicked, out pInstance ); }
		public override int TrayMenuItemRelease( ref IntPtr pInstance ) { return TrayIcon.TrayMenuItemRelease( ref pInstance ); }
		public override int TrayMenuItemContent( IntPtr instance, string value ) { return TrayIcon.TrayMenuItemContent( instance, value ); }
		public override int TrayMenuItemIsChecked( IntPtr instance, bool value ) { return TrayIcon.TrayMenuItemIsChecked( instance, value ); }

		//////////////////////

		//public static void PInvokeDemo()
		//{
		//	var icon = Properties.Resources.EditorLogo;
		//	//var icon = new Icon( typeof( SimpleTrayIconApi ), "SimpleTrayIcon.Demo.tray-icon-1.ico" );

		//	var hIcon = icon.Handle;
		//	DoubleClickHandler onDoubleClick = _ => Console.WriteLine( "Double click!" );
		//	TrayIcon.TrayMenuCreate( hIcon, "tip", onDoubleClick, out var hMenu );

		//	TrayIcon.TrayMenuItemCreate( ( s, e ) =>
		//	{
		//		EditorMessageBox.ShowInfo( "Clicked1" );
		//	}, out var hItem1 );

		//	TrayIcon.TrayMenuItemCreate( ( s, e ) =>
		//	{
		//		EditorMessageBox.ShowInfo( "Clicked2" );
		//	}, out var hItem2 );

		//	var item3Checked = false;
		//	string item3Content = "a";
		//	TrayIcon.TrayMenuItemCreate( ( s, e ) =>
		//	{
		//		TrayIcon.TrayMenuItemIsChecked( s, item3Checked = !item3Checked );
		//		item3Content += (char)( item3Content.Last() + 1 );
		//		TrayIcon.TrayMenuItemContent( s, item3Content );
		//		Console.WriteLine( "Clicked3" );
		//	}, out var hItem3 );

		//	TrayIcon.TrayMenuItemContent( hItem1, "item1" );
		//	TrayIcon.TrayMenuItemContent( hItem2, "item2" );
		//	TrayIcon.TrayMenuItemContent( hItem3, item3Content );

		//	TrayIcon.TrayMenuAdd( hMenu, hItem1 );
		//	TrayIcon.TrayMenuAdd( hMenu, hItem2 );
		//	TrayIcon.TrayMenuAdd( hMenu, hItem3 );

		//	TrayIcon.TrayMenuShow( hMenu );

		//	//NativeMethods.RunLoop();
		//}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class PlatformSpecificUtilityLinux : PlatformSpecificUtility
	{
		public override string GetExecutableDirectoryPath()
		{
			var result = "";

			try
			{
				string fileName = Process.GetCurrentProcess().MainModule.FileName;
				result = Path.GetDirectoryName( fileName );
			}
			catch { }

			result = VirtualPathUtility.NormalizePath( result );

			////when run by means built-in dotnet.exe from NeoAxis.Internal
			//{
			//	var remove = VirtualPathUtility.NormalizePath( @"\NeoAxis.Internal\Platforms\Windows\dotnet" );

			//	var index = result.IndexOf( remove );
			//	if( index != -1 )
			//		result = result.Remove( index, remove.Length );
			//}

			return result;
		}

		//public override IntPtr LoadLibrary( string path )
		//{
		//	var result = IntPtr.Zero;

		//	//Console.WriteLine( "LoadLibrary: " + path );

		//	//if( !NativeLibrary.TryLoad( path, out result ) )
		//	//{
		//	//	//try with "lib" prefix
		//	//	var newPath = Path.Combine( Path.GetDirectoryName( path ), "lib" + Path.GetFileName( path ) );

		//	//	Console.WriteLine( "second: " + newPath );

		//	//	NativeLibrary.TryLoad( newPath, out result );
		//	//}

		//	//Console.WriteLine( "LoadLibrary Result: " + result.ToString() );

		//	return result;
		//}

		///////////////////////////////////////////////

		public async override Task<string> GetClipboardTextAsync()
		{
			//check TextCopy library

			//!!!!impl
			return "";
		}

		public override void SetClipboardText( string text )
		{
			//!!!!impl
		}

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons )
		{
			//!!!!buttons, result

			Console.WriteLine( "MESSAGE:\n" + caption + ":" + text );

			//Android.Util.Log.WriteLine( Android.Util.LogPriority.Debug, "MyApp", "MESSAGE:\r\n" + caption + ":" + text );

			//while( ShowCursor( 1 ) < 0 ) { }

			//IntPtr hwnd = IntPtr.Zero;
			//if( EngineApp.IsSimulation && EngineApp.CreatedInsideEngineWindow != null )
			//	hwnd = EngineApp.CreatedInsideEngineWindow.Handle;

			//return (EDialogResult)MessageBox( hwnd, text, caption, (int)buttons | MB_ICONEXCLAMATION );

			return EDialogResult.OK;
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class PlatformSpecificUtilityMacOS : PlatformSpecificUtility
	{

		////!!!!test
		//[DllImport( "libNeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_Test", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode )]
		//public static extern int TestPInvoke();

		//[DllImport( "libNeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_LoadLibrary", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode )]
		//public static extern IntPtr MacLoadLibrary( string name );

		public async override Task<string> GetClipboardTextAsync()
		{
			//!!!!impl

			return "";
		}

		public override string GetExecutableDirectoryPath()
		{

			//!!!!?
			//AppContext.BaseDirectory

			var fileName = Process.GetCurrentProcess().MainModule.FileName;
			return Path.GetDirectoryName( fileName );

			////old: GetCallingAssembly
			//string codeBaseURI = Assembly.GetExecutingAssembly().CodeBase;
			//return Path.GetDirectoryName( codeBaseURI.Replace( "file://", "" ) );
		}

		//public override IntPtr LoadLibrary( string path )
		//{
		//	//!!!!test
		//	Console.WriteLine( "LoadLibrary: " + path );
		//	var result = TestPInvoke();
		//	Console.WriteLine( "TestPInvoke: " + result.ToString() );

		//	return MacLoadLibrary( path );
		//}

		public override void SetClipboardText( string text )
		{
			//!!!!impl
		}

		struct MacAppNativeWrapper
		{
			[DllImport( "libNeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_MessageBox", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode )]
			public static extern void MessageBox( string text, string caption );
		}

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons )
		{
			Console.WriteLine( "MESSAGE:\n" + caption + ":" + text );

			//!!!!buttons, result
			MacAppNativeWrapper.MessageBox( text, caption );
			return EDialogResult.OK;
		}
	}
}
