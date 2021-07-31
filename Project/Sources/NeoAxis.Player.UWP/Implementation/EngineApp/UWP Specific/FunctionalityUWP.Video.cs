// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;

namespace NeoAxis
{
	partial class PlatformFunctionalityUWP : PlatformFunctionality
	{
		static List<SystemSettings.DisplayInfo> tempScreenList = new List<SystemSettings.DisplayInfo>();

		///////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
		public struct RAMP
		{
			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
			public ushort[] red;
			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
			public ushort[] green;
			[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
			public ushort[] blue;
		}

		[DllImport( "gdi32.dll" )]
		static extern int GetDeviceGammaRamp( IntPtr hdc, out RAMP ramp );
		[DllImport( "gdi32.dll" )]
		static extern int SetDeviceGammaRamp( IntPtr hdc, ref RAMP ramp );

		///////////////////////////////////////////

		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
		static extern IntPtr GetDC( IntPtr hWnd );
		[DllImport( "user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true )]
		static extern int ReleaseDC( IntPtr hWnd, IntPtr hDC );

		[DllImport( "user32.dll" )]
		static extern IntPtr GetDesktopWindow();

		///////////////////////////////////////////

		const int MONITORINFOF_PRIMARY = 0x00000001;

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		struct MONITORINFOEX
		{
			public int cbSize;
			public RectangleI rcMonitor;
			public RectangleI rcWork;
			public int dwFlags;
			public unsafe fixed sbyte szDeviceName[ 32 ];
		}

		delegate bool MonitorEnumDelegate( IntPtr hMonitor, IntPtr hdcMonitor, ref RectangleI lprcMonitor, IntPtr dwData );

		[DllImport( "user32.dll" )]
		static extern bool EnumDisplayMonitors( IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData );

		[DllImport( "user32.dll" )]
		static extern bool GetMonitorInfo( IntPtr hMonitor, ref MONITORINFOEX lpmi );

		///////////////////////////////////////////

		//!!!! rename to GetScreenResolutions ?
		public override List<Vector2I> GetVideoModes()
		{
			// not implemented !

			// IDXGIAdapter1.EnumOutputs, IDXGIOutput::GetDisplayModeList to enumerate resolutions.
			// see SDL.

			// current resolution
			return new List<Vector2I>() { GetScreenSize() };
		}

		public override bool ChangeVideoMode( Vector2I mode )
		{
			if( mode == GetScreenSize() )
				return true;

			// we can't change video mode in UWP
			return false;
		}

		public override void RestoreVideoMode()
		{
			// we can't change video mode in UWP
		}

		public override void SetGamma( float value )
		{
			//copy from WindowsPlatformFunctionality

			RAMP gamma;
			IntPtr hWnd = GetDesktopWindow();
			IntPtr hdc = GetDC( hWnd );
			if( GetDeviceGammaRamp( hdc, out gamma ) != 0 )
			{
				for( int n = 0; n < 256; n++ )
				{
					ushort g = (ushort)( 255 * (ushort)( 255.0f * MathEx.Pow( (float)n / 255.0f, 1.0f / value ) ) );
					gamma.red[ n ] = g;
					gamma.green[ n ] = g;
					gamma.blue[ n ] = g;
				}
				if( SetDeviceGammaRamp( hdc, ref gamma ) == 0 )
				{
					//Error
					//ReleaseDC(hWnd, hdc);
					//return;
				}
			}

			ReleaseDC( hWnd, hdc );


			//// it seems it works, but it's cross-platform?

			//var wp = new WindowsPlatformFunctionality();
			//wp.SetGamma( value );
		}

		public override void ProcessChangingVideoMode()
		{
			// we can't change video mode in UWP

			// нам нужно включать фулскрин именно тут ?

			if( CreatedWindow_IsWindowActive() )
			{
				Debug.Assert( EngineApp.FullscreenEnabled == !applicationView.IsFullScreenMode );

				if( EngineApp.FullscreenEnabled )
				{
					bool result = applicationView.TryEnterFullScreenMode();
				}
				else
				{
					applicationView.ExitFullScreenMode();
				}
			}
		}

		//copy from WindowsPlatformFunctionality
		unsafe static bool MonitorEnumProc( IntPtr hMonitor, IntPtr hdcMonitor, ref RectangleI lprcMonitor, IntPtr dwData )
		{
			MONITORINFOEX info = new MONITORINFOEX();
			info.cbSize = sizeof( MONITORINFOEX );
			if( GetMonitorInfo( hMonitor, ref info ) )
			{
				SystemSettings.DisplayInfo displayInfo = new SystemSettings.DisplayInfo( new string( info.szDeviceName ), info.rcMonitor,
					info.rcWork, ( info.dwFlags & MONITORINFOF_PRIMARY ) != 0 );
				tempScreenList.Add( displayInfo );
			}

			return true;
		}

		public override IList<SystemSettings.DisplayInfo> GetAllDisplays()
		{
			//copy from WindowsPlatformFunctionality

			List<SystemSettings.DisplayInfo> result = new List<SystemSettings.DisplayInfo>();

			lock( tempScreenList )
			{
				IntPtr hdc = GetDC( IntPtr.Zero );
				if( hdc != IntPtr.Zero )
				{
					tempScreenList.Clear();

					EnumDisplayMonitors( hdc, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero );
					ReleaseDC( IntPtr.Zero, hdc );

					result.AddRange( tempScreenList );
					tempScreenList.Clear();
				}
			}

			if( result.Count == 0 )
			{
				RectangleI area = new RectangleI( Vector2I.Zero, GetScreenSize() );
				SystemSettings.DisplayInfo info = new SystemSettings.DisplayInfo( "Primary", area, area, true );
				result.Add( info );
			}

			return result;

			//// it seems it works, but it's cross-platform?

			//// alternative implementation can be used. see SDL:
			//// IDXGIAdapter1.EnumOutputs, IDXGIOutput::GetDisplayModeList.

			//var wp = new WindowsPlatformFunctionality();
			//return wp.GetAllDisplays();
		}
	}
}
