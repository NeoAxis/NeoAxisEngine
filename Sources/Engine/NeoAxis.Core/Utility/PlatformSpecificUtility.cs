// Copyright 2006–2026 Ivan Efimov. All rights reserved.

//The MIT License (MIT)
//Copyright( c ).NET Foundation and Contributors

using System;
using System.Threading.Tasks;

namespace NeoAxis
{
	/// <summary>
	/// Platform specific functionality.
	/// </summary>
	public abstract class PlatformSpecificUtility
	{
		static PlatformSpecificUtility instance;

		//

		protected PlatformSpecificUtility()
		{
			if( instance != null )
				Log.Fatal( "PlatformSpecificUtility: Instance already initialized." );
			instance = this;
		}

		public static PlatformSpecificUtility Instance
		{
			get
			{
				if( instance == null )
				{
					Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );

					//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					//	Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					//else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
					//	Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					//else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
					//	Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					//else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
					//	Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					//else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
					//	instance = new MacOSPlatformSpecificUtility();
					//else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
					//	instance = new LinuxPlatformSpecificUtility();
					//else
					//	instance = new WindowsPlatformSpecificUtility();
				}
				return instance;
			}
		}

		///////////////////////////////////////////////

		public abstract string GetExecutableDirectoryPath();
		//public abstract IntPtr LoadLibrary( string path );

		public abstract Task<string> GetClipboardTextAsync();
		public abstract void SetClipboardText( string text );

		public virtual object GetRegistryValue( string keyName, string valueName, object defaultValue ) { return null; }
		public virtual void SetRegistryValue( string keyName, string valueName, object value ) { }

		///////////////////////////////////////////////

		public abstract EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons = EMessageBoxButtons.OK );

		///////////////////////////////////////////////

		//!!!!make better API

		public delegate void TrayClickHandler( IntPtr obj );
		public delegate void TrayItemOnClickedHandler( IntPtr sender, uint id );

		public virtual int TrayMenuCreate( IntPtr hIcon, string tip, TrayClickHandler onClick, TrayClickHandler onDoubleClick, out IntPtr pInstance ) { pInstance = IntPtr.Zero; return 0; }
		public virtual int TrayMenuRelease( IntPtr pInstance ) { return 0; }
		public virtual int TrayMenuShow( IntPtr pInstance ) { return 0; }
		public virtual int TrayMenuClose( IntPtr pInstance ) { return 0; }
		public virtual int TrayMenuAdd( IntPtr pInstance, IntPtr pTrayMenuItem ) { return 0; }
		public virtual int TrayMenuRemove( IntPtr pInstance, IntPtr pTrayMenuItem ) { return 0; }
		public virtual int TrayMenuItemCreate( TrayItemOnClickedHandler onClicked, out IntPtr pInstance ) { pInstance = IntPtr.Zero; return 0; }
		public virtual int TrayMenuItemRelease( ref IntPtr pInstance ) { return 0; }
		public virtual int TrayMenuItemContent( IntPtr instance, string value ) { return 0; }
		public virtual int TrayMenuItemIsChecked( IntPtr instance, bool value ) { return 0; }
	}
}
