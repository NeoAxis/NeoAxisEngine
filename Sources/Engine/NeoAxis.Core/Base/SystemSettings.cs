// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Reflection;
using Internal;

namespace NeoAxis
{
	/// <summary>
	/// Class for getting information about the operating system.
	/// </summary>
	public static class SystemSettings
	{
		static Platform platform;
		static List<Vector2I> videoModes;

		static NetRuntimeType netRuntime;

		static Vector2I? videoModeChanged;
		static float? gammaChanged;

		static Dictionary<string, string> commandLineParameters;

		///////////////////////////////////////////

		public enum Platform
		{
			Windows,
			macOS,
			UWP,
			Android,
			iOS,
			
			//!!!!
			Store,

			//!!!!Special,
		}

		///////////////////////////////////////////

		public enum NetRuntimeType
		{
			Net,
			Mono,
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents an item for <see cref="AllDisplays"/> property.
		/// </summary>
		public sealed class DisplayInfo
		{
			string deviceName;
			RectangleI bounds;
			RectangleI workingArea;
			bool primary;

			public DisplayInfo( string deviceName, RectangleI bounds, RectangleI workingArea, bool primary )
			{
				this.deviceName = deviceName;
				this.bounds = bounds;
				this.workingArea = workingArea;
				this.primary = primary;
			}

			public string DeviceName
			{
				get { return deviceName; }
				set { deviceName = value; }
			}

			public RectangleI Bounds
			{
				get { return bounds; }
				set { bounds = value; }
			}

			public RectangleI WorkingArea
			{
				get { return workingArea; }
				set { workingArea = value; }
			}

			public bool Primary
			{
				get { return primary; }
				set { primary = value; }
			}
		}

		///////////////////////////////////////////

		public enum FloatingPointModelEnum
		{
			Fast24Bits,
			Strict53Bits,
			Precise64Bits,
		}

		///////////////////////////////////////////

		const uint OS_WOW6432 = 0x1E;
		[DllImport( "shlwapi.dll" )]
		static extern bool IsOS( uint dwOS );

		///////////////////////////////////////////

		struct MacOSXUtilsNativeWrapper
		{
			[DllImport( "UtilsNativeWrapper", EntryPoint = "UtilsNativeWrapper_GetOSVersion", CallingConvention = CallingConvention.Cdecl )]
			public static extern void GetOSVersion( out int major, out int minor, out int bugFix );

			[return: MarshalAs( UnmanagedType.U1 )]
			[DllImport( "UtilsNativeWrapper", EntryPoint = "UtilsNativeWrapper_IsSystem64Bit", CallingConvention = CallingConvention.Cdecl )]
			public static extern bool IsSystem64Bit();
		}

		///////////////////////////////////////////

		[DllImport( NativeUtility.library, CallingConvention = NativeUtility.convention )]
		static extern int FloatingPointModel_GetValue();

		[DllImport( NativeUtility.library, CallingConvention = NativeUtility.convention )]
		static extern void FloatingPointModel_SetValue( int value );

		///////////////////////////////////////////

		static class UWPHelper
		{
			const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

			[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
			static extern int GetCurrentPackageFullName( ref int packageFullNameLength, StringBuilder packageFullName );

			public static bool IsRunningAsUwp()
			{
				if( IsWindows7OrLower )
				{
					return false;
				}
				else
				{
					int length = 0;
					StringBuilder sb = new StringBuilder( 0 );
					int result = GetCurrentPackageFullName( ref length, sb );

					sb = new StringBuilder( length );
					result = GetCurrentPackageFullName( ref length, sb );

					return result != APPMODEL_ERROR_NO_PACKAGE;
				}
			}

			private static bool IsWindows7OrLower
			{
				get
				{
					int versionMajor = Environment.OSVersion.Version.Major;
					int versionMinor = Environment.OSVersion.Version.Minor;
					double version = versionMajor + (double)versionMinor / 10;
					return version <= 6.1;
				}
			}
		}

		///////////////////////////////////////////

		static SystemSettings()
		{
#if ANDROID
			platform = Platform.Android;
#elif IOS
			platform = Platform.iOS;
#else
			if( Environment.OSVersion.Platform == PlatformID.Unix )
			{
				platform = Platform.macOS;
				//try
				//{
				//   if( AndroidAppNativeWrapper.IsAndroid() )
				//      platform = Platforms.Android;
				//}
				//catch { }
			}
			else if( UWPHelper.IsRunningAsUwp() )
				platform = Platform.UWP;
			else
				platform = Platform.Windows;
#endif

			netRuntime = Type.GetType( "Mono.Runtime", false ) != null ? NetRuntimeType.Mono : NetRuntimeType.Net;
		}

		public static Platform CurrentPlatform
		{
			get { return platform; }
		}

		public static Version OSVersion
		{
			get
			{
				if( CurrentPlatform == Platform.macOS )
				{
					int major, minor, bugFix;
					MacOSXUtilsNativeWrapper.GetOSVersion( out major, out minor, out bugFix );
					return new Version( major, minor, bugFix );
				}
				//else if( Platform == Platforms.Android )
				//{
				//   int major, minor, bugFix;
				//   AndroidAppNativeWrapper.GetOSVersion( out major, out minor, out bugFix );
				//   return new Version( major, minor, bugFix );
				//}
				else
					return Environment.OSVersion.Version;
			}
		}

		public static IList<Vector2I> VideoModes
		{
			get
			{
				if( videoModes == null )
					GenerateVideoModes();
				return videoModes.AsReadOnly();
			}
		}

		public static bool VideoModeExists( Vector2I videoMode )
		{
			foreach( Vector2I mode in VideoModes )
			{
				if( videoMode == mode )
					return true;
			}
			return false;
		}

		static void GenerateVideoModes()
		{
			//generate list
			videoModes = PlatformFunctionality.Instance.GetVideoModes();

			//safe list
			if( videoModes.Count == 0 )
			{
				videoModes.Add( new Vector2I( 640, 480 ) );
				videoModes.Add( new Vector2I( 800, 600 ) );
				videoModes.Add( new Vector2I( 1024, 768 ) );
				videoModes.Add( new Vector2I( 1152, 864 ) );
				videoModes.Add( new Vector2I( 1280, 1024 ) );
				videoModes.Add( new Vector2I( 1600, 1200 ) );
			}

			//sort
			CollectionUtility.SelectionSort( videoModes, delegate ( Vector2I mode1, Vector2I mode2 )
			{
				if( mode1.X < mode2.X )
					return -1;
				if( mode1.X > mode2.X )
					return 1;
				if( mode1.Y < mode2.Y )
					return -1;
				if( mode1.Y > mode2.Y )
					return 1;
				return 0;
			} );
		}

		/// <summary>
		/// Returns a list of available displays in the system.
		/// </summary>
		public static IList<DisplayInfo> AllDisplays
		{
			get { return PlatformFunctionality.Instance.GetAllDisplays(); }
		}

		public static RectangleI AllDisplaysBounds
		{
			get
			{
				var displays = AllDisplays;
				var result = displays[ 0 ].Bounds;
				for( int n = 1; n < displays.Count; n++ )
					result.Add( displays[ n ].Bounds );
				return result;
			}
		}

		public static bool ChangeVideoMode( Vector2I mode )
		{
			if( !PlatformFunctionality.Instance.ChangeVideoMode( mode ) )
				return false;
			videoModeChanged = mode;
			return true;
		}

		public static void RestoreVideoMode()
		{
			if( videoModeChanged != null )
			{
				PlatformFunctionality.Instance.RestoreVideoMode();
				videoModeChanged = null;
			}
		}

		public static void SetGamma( float value )
		{
			PlatformFunctionality.Instance.SetGamma( value );
			gammaChanged = value;
		}

		public static void ResetGamma()
		{
			if( gammaChanged != null )
			{
				PlatformFunctionality.Instance.SetGamma( 1 );
				gammaChanged = null;
			}
		}

		public static NetRuntimeType NetRuntime
		{
			get { return netRuntime; }
		}

		public static string GetNetRuntimeDisplayName()
		{
			//if( NetRuntime == NetRuntimeType.Mono )
			//{
			//	Type monoRuntimeType = Type.GetType( "Mono.Runtime", false );
			//	MethodInfo getDisplayNameMethod = monoRuntimeType.GetMethod( "GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding );
			//	if( getDisplayNameMethod != null )
			//	{
			//		string displayName = (string)getDisplayNameMethod.Invoke( null, new object[ 0 ] );
			//		return string.Format( "{0} {1}, {2}", NetRuntime.ToString(), displayName, Environment.Version );
			//	}
			//}

			return string.Format( "{0} {1}", NetRuntime.ToString(), Environment.Version );
		}

		public static FloatingPointModelEnum FloatingPointModel
		{
			get
			{
				try
				{
					NativeUtility.LoadUtilsNativeWrapperLibrary();
					return (FloatingPointModelEnum)FloatingPointModel_GetValue();
				}
				catch
				{
					return FloatingPointModelEnum.Strict53Bits;
				}
			}
			set
			{
				try
				{
					NativeUtility.LoadUtilsNativeWrapperLibrary();
					FloatingPointModel_SetValue( (int)value );
				}
				catch { }
			}
		}

		static Dictionary<string, string> ParseCommandLineParameters()
		{
			var result = new Dictionary<string, string>();

			if( CurrentPlatform == Platform.Windows || CurrentPlatform == Platform.macOS )
			{
				try
				{
					string[] args = Environment.GetCommandLineArgs();
					for( int n = 1; n < args.Length; n++ )
					{
						string command = args[ n ];
						n++;
						if( n < args.Length )
							result[ command ] = args[ n ];
					}
				}
				catch { }
			}

			return result;
		}

		public static Dictionary<string, string> CommandLineParameters
		{
			get
			{
				if( commandLineParameters == null )
					commandLineParameters = ParseCommandLineParameters();
				return commandLineParameters;
			}
		}

		/// <summary>
		/// Mobile device. Android, iOS.
		/// </summary>
		public static bool LimitedDevice
		{
			get { return CurrentPlatform == Platform.Android || CurrentPlatform == Platform.iOS; }
		}

		/// <summary>
		/// Android, iOS.
		/// </summary>
		public static bool MobileDevice
		{
			get { return CurrentPlatform == Platform.Android || CurrentPlatform == Platform.iOS; }
		}

	}
}
