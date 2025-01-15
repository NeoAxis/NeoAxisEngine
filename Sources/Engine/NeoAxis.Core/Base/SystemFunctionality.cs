// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !WEB
using NeoAxis.Editor;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A class to work with the operating system.
	/// </summary>
	public static class SystemFunctionality
	{
		public static class TrayIcon
		{
			//!!!!make better API

			const string library = "NeoAxisCoreNative";

			public delegate void ClickHandler( IntPtr obj );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuCreate( IntPtr hIcon, [MarshalAs( UnmanagedType.LPWStr )] string tip, ClickHandler onClick, ClickHandler onDoubleClick, out IntPtr pInstance );

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


			public delegate void OnClicked( IntPtr sender, uint id );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuItemCreate( OnClicked onClicked, out IntPtr pInstance );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuItemRelease( ref IntPtr pInstance );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuItemContent( IntPtr instance, [MarshalAs( UnmanagedType.LPWStr )] string value );

			[DllImport( library, ExactSpelling = true, CharSet = CharSet.Unicode )]
			public static extern int TrayMenuItemIsChecked( IntPtr instance, bool value );
		}

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
}
#endif