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
using System.Linq;
using System.Threading;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.Buffers;
using System.Security;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NeoAxis
{
	/// <summary>
	/// Internal class for implementing the target platform.
	/// </summary>
	public abstract class PlatformSpecificUtility
	{
		static PlatformSpecificUtility instance;

		protected void SetInstance( PlatformSpecificUtility instance )
		{
			PlatformSpecificUtility.instance = instance;
		}

		public static PlatformSpecificUtility Instance
		{
			get
			{
				if( instance == null )
				{
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
						instance = new MacOSPlatformSpecificUtility();
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
						instance = new LinuxPlatformSpecificUtility();
					else
						instance = new WindowsPlatformSpecificUtility();
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
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!move to special dll

	class WindowsPlatformSpecificUtility : PlatformSpecificUtility
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

		//static Assembly win32RegistryAssembly;

		//static Type GetRegistryClass()
		//{
		//	if( win32RegistryAssembly == null )
		//		win32RegistryAssembly = GetAssemblyByName( "Microsoft.Win32.Registry" );
		//	return win32RegistryAssembly.GetType( "Microsoft.Win32.Registry" );
		//}

		///////////////////////////////////////////////

		//[DllImport( "advapi32.dll", CharSet = CharSet.Auto )]
		//public static extern int RegOpenKeyEx( UIntPtr hKey, string subKey, int ulOptions, int samDesired, out UIntPtr hkResult );

		//[DllImport( "advapi32.dll", SetLastError = true )]
		//static extern uint RegQueryValueEx( UIntPtr hKey, string lpValueName, int lpReserved, ref RegistryValueKind lpType, IntPtr lpData, ref int lpcbData );

		public sealed class SafeRegistryHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			internal SafeRegistryHandle()
				: base( ownsHandle: true )
			{
			}

			public SafeRegistryHandle( IntPtr preexistingHandle, bool ownsHandle )
				: base( ownsHandle )
			{
				SetHandle( preexistingHandle );
			}

			protected override bool ReleaseHandle()
			{
				return Interop.Advapi32.RegCloseKey( handle ) == 0;
			}
		}

		public static class Registry
		{
			public static readonly RegistryKey CurrentUser = RegistryKey.OpenBaseKey( RegistryHive.CurrentUser, RegistryView.Default );

			public static readonly RegistryKey LocalMachine = RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, RegistryView.Default );

			public static readonly RegistryKey ClassesRoot = RegistryKey.OpenBaseKey( RegistryHive.ClassesRoot, RegistryView.Default );

			public static readonly RegistryKey Users = RegistryKey.OpenBaseKey( RegistryHive.Users, RegistryView.Default );

			public static readonly RegistryKey PerformanceData = RegistryKey.OpenBaseKey( RegistryHive.PerformanceData, RegistryView.Default );

			public static readonly RegistryKey CurrentConfig = RegistryKey.OpenBaseKey( RegistryHive.CurrentConfig, RegistryView.Default );

			private static RegistryKey GetBaseKeyFromKeyName( string keyName, out string subKeyName )
			{
				if( keyName == null )
				{
					throw new ArgumentNullException( "keyName" );
				}
				int num = keyName.IndexOf( '\\' );
				int num2 = ( ( num != -1 ) ? num : keyName.Length );
				RegistryKey registryKey = null;
				switch( num2 )
				{
				case 10:
					registryKey = Users;
					break;
				case 17:
					registryKey = ( ( char.ToUpperInvariant( keyName[ 6 ] ) == 'L' ) ? ClassesRoot : CurrentUser );
					break;
				case 18:
					registryKey = LocalMachine;
					break;
				case 19:
					registryKey = CurrentConfig;
					break;
				case 21:
					registryKey = PerformanceData;
					break;
				}
				if( registryKey != null && keyName.StartsWith( registryKey.Name, StringComparison.OrdinalIgnoreCase ) )
				{
					subKeyName = ( ( num == -1 || num == keyName.Length ) ? string.Empty : keyName.Substring( num + 1, keyName.Length - num - 1 ) );
					return registryKey;
				}
				throw new ArgumentException( SR.Format( SR.Arg_RegInvalidKeyName, "keyName" ), "keyName" );
			}

			public static object GetValue( string keyName, string valueName, object defaultValue )
			{
				string subKeyName;
				RegistryKey baseKeyFromKeyName = GetBaseKeyFromKeyName( keyName, out subKeyName );
				using( RegistryKey registryKey = baseKeyFromKeyName.OpenSubKey( subKeyName ) )
				{
					return registryKey?.GetValue( valueName, defaultValue );
				}
			}

			public static void SetValue( string keyName, string valueName, object value )
			{
				SetValue( keyName, valueName, value, RegistryValueKind.Unknown );
			}

			public static void SetValue( string keyName, string valueName, object value, RegistryValueKind valueKind )
			{
				string subKeyName;
				RegistryKey baseKeyFromKeyName = GetBaseKeyFromKeyName( keyName, out subKeyName );
				using( RegistryKey registryKey = baseKeyFromKeyName.CreateSubKey( subKeyName ) )
				{
					registryKey.SetValue( valueName, value, valueKind );
				}
			}
		}

		public enum RegistryHive
		{
			ClassesRoot = int.MinValue,
			CurrentUser,
			LocalMachine,
			Users,
			PerformanceData,
			CurrentConfig
		}

		public sealed class RegistryKey : MarshalByRefObject, IDisposable
		{
			[Flags]
			private enum StateFlags
			{
				Dirty = 0x1,
				SystemKey = 0x2,
				WriteAccess = 0x4,
				PerfData = 0x8
			}

			private static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr( int.MinValue );

			private static readonly IntPtr HKEY_CURRENT_USER = new IntPtr( -2147483647 );

			private static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr( -2147483646 );

			private static readonly IntPtr HKEY_USERS = new IntPtr( -2147483645 );

			private static readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr( -2147483644 );

			private static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr( -2147483643 );

			private static readonly string[] s_hkeyNames = new string[ 6 ] { "HKEY_CLASSES_ROOT", "HKEY_CURRENT_USER", "HKEY_LOCAL_MACHINE", "HKEY_USERS", "HKEY_PERFORMANCE_DATA", "HKEY_CURRENT_CONFIG" };

			private volatile SafeRegistryHandle _hkey;

			private volatile string _keyName;

			private volatile bool _remoteKey;

			private volatile StateFlags _state;

			private volatile RegistryKeyPermissionCheck _checkMode;

			private volatile RegistryView _regView;

			public int SubKeyCount
			{
				get
				{
					EnsureNotDisposed();
					return InternalSubKeyCountCore();
				}
			}

			public RegistryView View
			{
				get
				{
					EnsureNotDisposed();
					return _regView;
				}
			}

			public SafeRegistryHandle Handle
			{
				get
				{
					EnsureNotDisposed();
					if( !IsSystemKey() )
					{
						return _hkey;
					}
					return SystemKeyHandle;
				}
			}

			public int ValueCount
			{
				get
				{
					EnsureNotDisposed();
					return InternalValueCountCore();
				}
			}

			public string Name
			{
				get
				{
					EnsureNotDisposed();
					return _keyName;
				}
			}

			private SafeRegistryHandle SystemKeyHandle
			{
				get
				{
					int errorCode = 6;
					IntPtr hKey = (IntPtr)0;
					switch( _keyName )
					{
					case "HKEY_CLASSES_ROOT":
						hKey = HKEY_CLASSES_ROOT;
						break;
					case "HKEY_CURRENT_USER":
						hKey = HKEY_CURRENT_USER;
						break;
					case "HKEY_LOCAL_MACHINE":
						hKey = HKEY_LOCAL_MACHINE;
						break;
					case "HKEY_USERS":
						hKey = HKEY_USERS;
						break;
					case "HKEY_PERFORMANCE_DATA":
						hKey = HKEY_PERFORMANCE_DATA;
						break;
					case "HKEY_CURRENT_CONFIG":
						hKey = HKEY_CURRENT_CONFIG;
						break;
					default:
						Win32Error( errorCode, null );
						break;
					}
					errorCode = Interop.Advapi32.RegOpenKeyEx( hKey, null, 0, GetRegistryKeyAccess( IsWritable() ) | (int)_regView, out var hkResult );
					if( errorCode == 0 && !hkResult.IsInvalid )
					{
						return hkResult;
					}
					Win32Error( errorCode, null );
					throw new IOException( Interop.Kernel32.GetMessage( errorCode ), errorCode );
				}
			}

			private RegistryKey( SafeRegistryHandle hkey, bool writable, RegistryView view )
				: this( hkey, writable, systemkey: false, remoteKey: false, isPerfData: false, view )
			{
			}

			private RegistryKey( SafeRegistryHandle hkey, bool writable, bool systemkey, bool remoteKey, bool isPerfData, RegistryView view )
			{
				ValidateKeyView( view );
				_hkey = hkey;
				_keyName = "";
				_remoteKey = remoteKey;
				_regView = view;
				if( systemkey )
				{
					_state |= StateFlags.SystemKey;
				}
				if( writable )
				{
					_state |= StateFlags.WriteAccess;
				}
				if( isPerfData )
				{
					_state |= StateFlags.PerfData;
				}
			}

			public void Flush()
			{
				FlushCore();
			}

			public void Close()
			{
				Dispose();
			}

			public void Dispose()
			{
				if( _hkey == null )
				{
					return;
				}
				if( !IsSystemKey() )
				{
					try
					{
						_hkey.Dispose();
					}
					catch( IOException )
					{
					}
					finally
					{
						_hkey = null;
					}
				}
				else if( IsPerfDataKey() )
				{
					ClosePerfDataKey();
				}
			}

			public RegistryKey CreateSubKey( string subkey )
			{
				return CreateSubKey( subkey, _checkMode );
			}

			public RegistryKey CreateSubKey( string subkey, bool writable )
			{
				return CreateSubKey( subkey, ( !writable ) ? RegistryKeyPermissionCheck.ReadSubTree : RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None );
			}

			public RegistryKey CreateSubKey( string subkey, bool writable, RegistryOptions options )
			{
				return CreateSubKey( subkey, ( !writable ) ? RegistryKeyPermissionCheck.ReadSubTree : RegistryKeyPermissionCheck.ReadWriteSubTree, options );
			}

			public RegistryKey CreateSubKey( string subkey, RegistryKeyPermissionCheck permissionCheck )
			{
				return CreateSubKey( subkey, permissionCheck, RegistryOptions.None );
			}

			//public RegistryKey CreateSubKey( string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions, RegistrySecurity registrySecurity )
			//{
			//	return CreateSubKey( subkey, permissionCheck, registryOptions );
			//}

			//public RegistryKey CreateSubKey( string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity )
			//{
			//	return CreateSubKey( subkey, permissionCheck, RegistryOptions.None );
			//}

			public RegistryKey CreateSubKey( string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions )
			{
				ValidateKeyOptions( registryOptions );
				ValidateKeyName( subkey );
				ValidateKeyMode( permissionCheck );
				EnsureWriteable();
				subkey = FixupName( subkey );
				if( !_remoteKey )
				{
					RegistryKey registryKey = InternalOpenSubKeyWithoutSecurityChecks( subkey, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree );
					if( registryKey != null )
					{
						registryKey._checkMode = permissionCheck;
						return registryKey;
					}
				}
				return CreateSubKeyInternalCore( subkey, permissionCheck, registryOptions );
			}

			public void DeleteSubKey( string subkey )
			{
				DeleteSubKey( subkey, throwOnMissingSubKey: true );
			}

			public void DeleteSubKey( string subkey, bool throwOnMissingSubKey )
			{
				ValidateKeyName( subkey );
				EnsureWriteable();
				subkey = FixupName( subkey );
				RegistryKey registryKey = InternalOpenSubKeyWithoutSecurityChecks( subkey, writable: false );
				if( registryKey != null )
				{
					using( registryKey )
					{
						if( registryKey.SubKeyCount > 0 )
						{
							throw new InvalidOperationException( SR.InvalidOperation_RegRemoveSubKey );
						}
					}
					DeleteSubKeyCore( subkey, throwOnMissingSubKey );
				}
				else if( throwOnMissingSubKey )
				{
					throw new ArgumentException( SR.Arg_RegSubKeyAbsent );
				}
			}

			public void DeleteSubKeyTree( string subkey )
			{
				DeleteSubKeyTree( subkey, throwOnMissingSubKey: true );
			}

			public void DeleteSubKeyTree( string subkey, bool throwOnMissingSubKey )
			{
				ValidateKeyName( subkey );
				if( subkey.Length == 0 && IsSystemKey() )
				{
					throw new ArgumentException( SR.Arg_RegKeyDelHive );
				}
				EnsureWriteable();
				subkey = FixupName( subkey );
				RegistryKey registryKey = InternalOpenSubKeyWithoutSecurityChecks( subkey, writable: true );
				if( registryKey != null )
				{
					using( registryKey )
					{
						if( registryKey.SubKeyCount > 0 )
						{
							string[] subKeyNames = registryKey.GetSubKeyNames();
							for( int i = 0; i < subKeyNames.Length; i++ )
							{
								registryKey.DeleteSubKeyTreeInternal( subKeyNames[ i ] );
							}
						}
					}
					DeleteSubKeyTreeCore( subkey );
				}
				else if( throwOnMissingSubKey )
				{
					throw new ArgumentException( SR.Arg_RegSubKeyAbsent );
				}
			}

			private void DeleteSubKeyTreeInternal( string subkey )
			{
				RegistryKey registryKey = InternalOpenSubKeyWithoutSecurityChecks( subkey, writable: true );
				if( registryKey != null )
				{
					using( registryKey )
					{
						if( registryKey.SubKeyCount > 0 )
						{
							string[] subKeyNames = registryKey.GetSubKeyNames();
							for( int i = 0; i < subKeyNames.Length; i++ )
							{
								registryKey.DeleteSubKeyTreeInternal( subKeyNames[ i ] );
							}
						}
					}
					DeleteSubKeyTreeCore( subkey );
					return;
				}
				throw new ArgumentException( SR.Arg_RegSubKeyAbsent );
			}

			public void DeleteValue( string name )
			{
				DeleteValue( name, throwOnMissingValue: true );
			}

			public void DeleteValue( string name, bool throwOnMissingValue )
			{
				EnsureWriteable();
				DeleteValueCore( name, throwOnMissingValue );
			}

			public static RegistryKey OpenBaseKey( RegistryHive hKey, RegistryView view )
			{
				ValidateKeyView( view );
				return OpenBaseKeyCore( hKey, view );
			}

			public static RegistryKey OpenRemoteBaseKey( RegistryHive hKey, string machineName )
			{
				return OpenRemoteBaseKey( hKey, machineName, RegistryView.Default );
			}

			public static RegistryKey OpenRemoteBaseKey( RegistryHive hKey, string machineName, RegistryView view )
			{
				if( machineName == null )
				{
					throw new ArgumentNullException( "machineName" );
				}
				ValidateKeyView( view );
				return OpenRemoteBaseKeyCore( hKey, machineName, view );
			}

			public RegistryKey OpenSubKey( string name )
			{
				return OpenSubKey( name, writable: false );
			}

			public RegistryKey OpenSubKey( string name, bool writable )
			{
				ValidateKeyName( name );
				EnsureNotDisposed();
				name = FixupName( name );
				return InternalOpenSubKeyCore( name, writable );
			}

			public RegistryKey OpenSubKey( string name, RegistryKeyPermissionCheck permissionCheck )
			{
				ValidateKeyMode( permissionCheck );
				return OpenSubKey( name, permissionCheck, (RegistryRights)GetRegistryKeyAccess( permissionCheck ) );
			}

			public RegistryKey OpenSubKey( string name, RegistryRights rights )
			{
				return OpenSubKey( name, _checkMode, rights );
			}

			public RegistryKey OpenSubKey( string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights )
			{
				ValidateKeyName( name );
				ValidateKeyMode( permissionCheck );
				ValidateKeyRights( rights );
				EnsureNotDisposed();
				name = FixupName( name );
				return InternalOpenSubKeyCore( name, permissionCheck, (int)rights );
			}

			internal RegistryKey InternalOpenSubKeyWithoutSecurityChecks( string name, bool writable )
			{
				ValidateKeyName( name );
				EnsureNotDisposed();
				return InternalOpenSubKeyWithoutSecurityChecksCore( name, writable );
			}

			//public RegistrySecurity GetAccessControl()
			//{
			//	return GetAccessControl( AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group );
			//}

			//public RegistrySecurity GetAccessControl( AccessControlSections includeSections )
			//{
			//	EnsureNotDisposed();
			//	return new RegistrySecurity( Handle, Name, includeSections );
			//}

			//public void SetAccessControl( RegistrySecurity registrySecurity )
			//{
			//	EnsureWriteable();
			//	if( registrySecurity == null )
			//	{
			//		throw new ArgumentNullException( "registrySecurity" );
			//	}
			//	registrySecurity.Persist( Handle, Name );
			//}

			public static RegistryKey FromHandle( SafeRegistryHandle handle )
			{
				return FromHandle( handle, RegistryView.Default );
			}

			public static RegistryKey FromHandle( SafeRegistryHandle handle, RegistryView view )
			{
				if( handle == null )
				{
					throw new ArgumentNullException( "handle" );
				}
				ValidateKeyView( view );
				return new RegistryKey( handle, writable: true, view );
			}

			public string[] GetSubKeyNames()
			{
				EnsureNotDisposed();
				int subKeyCount = SubKeyCount;
				if( subKeyCount <= 0 )
				{
					return Array.Empty<string>();
				}
				return InternalGetSubKeyNamesCore( subKeyCount );
			}

			public string[] GetValueNames()
			{
				EnsureNotDisposed();
				int valueCount = ValueCount;
				if( valueCount <= 0 )
				{
					return Array.Empty<string>();
				}
				return GetValueNamesCore( valueCount );
			}

			public object GetValue( string name )
			{
				return InternalGetValue( name, null, doNotExpand: false );
			}

			public object GetValue( string name, object defaultValue )
			{
				return InternalGetValue( name, defaultValue, doNotExpand: false );
			}

			public object GetValue( string name, object defaultValue, RegistryValueOptions options )
			{
				if( options < RegistryValueOptions.None || options > RegistryValueOptions.DoNotExpandEnvironmentNames )
				{
					throw new ArgumentException( SR.Format( SR.Arg_EnumIllegalVal, (int)options ), "options" );
				}
				bool doNotExpand = options == RegistryValueOptions.DoNotExpandEnvironmentNames;
				return InternalGetValue( name, defaultValue, doNotExpand );
			}

			private object InternalGetValue( string name, object defaultValue, bool doNotExpand )
			{
				EnsureNotDisposed();
				return InternalGetValueCore( name, defaultValue, doNotExpand );
			}

			public RegistryValueKind GetValueKind( string name )
			{
				EnsureNotDisposed();
				return GetValueKindCore( name );
			}

			public void SetValue( string name, object value )
			{
				SetValue( name, value, RegistryValueKind.Unknown );
			}

			public void SetValue( string name, object value, RegistryValueKind valueKind )
			{
				if( value == null )
				{
					throw new ArgumentNullException( "value" );
				}
				if( name != null && name.Length > 16383 )
				{
					throw new ArgumentException( SR.Arg_RegValStrLenBug, "name" );
				}
				if( !Enum.IsDefined( typeof( RegistryValueKind ), valueKind ) )
				{
					throw new ArgumentException( SR.Arg_RegBadKeyKind, "valueKind" );
				}
				EnsureWriteable();
				if( valueKind == RegistryValueKind.Unknown )
				{
					valueKind = CalculateValueKind( value );
				}
				SetValueCore( name, value, valueKind );
			}

			private RegistryValueKind CalculateValueKind( object value )
			{
				if( value is int )
				{
					return RegistryValueKind.DWord;
				}
				if( value is Array )
				{
					if( value is byte[] )
					{
						return RegistryValueKind.Binary;
					}
					if( value is string[] )
					{
						return RegistryValueKind.MultiString;
					}
					throw new ArgumentException( SR.Format( SR.Arg_RegSetBadArrType, value.GetType().Name ) );
				}
				return RegistryValueKind.String;
			}

			public override string ToString()
			{
				EnsureNotDisposed();
				return _keyName;
			}

			private static string FixupName( string name )
			{
				if( name.IndexOf( '\\' ) == -1 )
				{
					return name;
				}
				StringBuilder stringBuilder = new StringBuilder( name );
				FixupPath( stringBuilder );
				int num = stringBuilder.Length - 1;
				if( num >= 0 && stringBuilder[ num ] == '\\' )
				{
					stringBuilder.Length = num;
				}
				return stringBuilder.ToString();
			}

			private static void FixupPath( StringBuilder path )
			{
				int length = path.Length;
				bool flag = false;
				char c = '\uffff';
				int i;
				for( i = 1; i < length - 1; i++ )
				{
					if( path[ i ] == '\\' )
					{
						i++;
						while( i < length && path[ i ] == '\\' )
						{
							path[ i ] = c;
							i++;
							flag = true;
						}
					}
				}
				if( !flag )
				{
					return;
				}
				i = 0;
				int num = 0;
				while( i < length )
				{
					if( path[ i ] == c )
					{
						i++;
						continue;
					}
					path[ num ] = path[ i ];
					i++;
					num++;
				}
				path.Length += num - i;
			}

			private void EnsureNotDisposed()
			{
				if( _hkey == null )
				{
					throw new ObjectDisposedException( _keyName, SR.ObjectDisposed_RegKeyClosed );
				}
			}

			private void EnsureWriteable()
			{
				EnsureNotDisposed();
				if( !IsWritable() )
				{
					throw new UnauthorizedAccessException( SR.UnauthorizedAccess_RegistryNoWrite );
				}
			}

			private RegistryKeyPermissionCheck GetSubKeyPermissionCheck( bool subkeyWritable )
			{
				if( _checkMode == RegistryKeyPermissionCheck.Default )
				{
					return _checkMode;
				}
				if( subkeyWritable )
				{
					return RegistryKeyPermissionCheck.ReadWriteSubTree;
				}
				return RegistryKeyPermissionCheck.ReadSubTree;
			}

			private static void ValidateKeyName( string name )
			{
				if( name == null )
				{
					throw new ArgumentNullException( "name" );
				}
				int num = name.IndexOf( "\\", StringComparison.OrdinalIgnoreCase );
				int num2 = 0;
				while( num != -1 )
				{
					if( num - num2 > 255 )
					{
						throw new ArgumentException( SR.Arg_RegKeyStrLenBug, "name" );
					}
					num2 = num + 1;
					num = name.IndexOf( "\\", num2, StringComparison.OrdinalIgnoreCase );
				}
				if( name.Length - num2 > 255 )
				{
					throw new ArgumentException( SR.Arg_RegKeyStrLenBug, "name" );
				}
			}

			private static void ValidateKeyMode( RegistryKeyPermissionCheck mode )
			{
				if( mode < RegistryKeyPermissionCheck.Default || mode > RegistryKeyPermissionCheck.ReadWriteSubTree )
				{
					throw new ArgumentException( SR.Argument_InvalidRegistryKeyPermissionCheck, "mode" );
				}
			}

			private static void ValidateKeyOptions( RegistryOptions options )
			{
				if( options < RegistryOptions.None || options > RegistryOptions.Volatile )
				{
					throw new ArgumentException( SR.Argument_InvalidRegistryOptionsCheck, "options" );
				}
			}

			private static void ValidateKeyView( RegistryView view )
			{
				if( view != 0 && view != RegistryView.Registry32 && view != RegistryView.Registry64 )
				{
					throw new ArgumentException( SR.Argument_InvalidRegistryViewCheck, "view" );
				}
			}

			private static void ValidateKeyRights( RegistryRights rights )
			{
				if( ( (uint)rights & 0xFFF0FFC0u ) != 0 )
				{
					throw new SecurityException( SR.Security_RegistryPermission );
				}
			}

			private bool IsDirty()
			{
				return ( _state & StateFlags.Dirty ) != 0;
			}

			private bool IsSystemKey()
			{
				return ( _state & StateFlags.SystemKey ) != 0;
			}

			private bool IsWritable()
			{
				return ( _state & StateFlags.WriteAccess ) != 0;
			}

			private bool IsPerfDataKey()
			{
				return ( _state & StateFlags.PerfData ) != 0;
			}

			private void SetDirty()
			{
				_state |= StateFlags.Dirty;
			}

			private void ClosePerfDataKey()
			{
				Interop.Advapi32.RegCloseKey( HKEY_PERFORMANCE_DATA );
			}

			private void FlushCore()
			{
				if( _hkey != null && IsDirty() )
				{
					Interop.Advapi32.RegFlushKey( _hkey );
				}
			}

			private RegistryKey CreateSubKeyInternalCore( string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions )
			{
				Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs = default( Interop.Kernel32.SECURITY_ATTRIBUTES );
				int lpdwDisposition = 0;
				SafeRegistryHandle hkResult = null;
				int num = Interop.Advapi32.RegCreateKeyEx( _hkey, subkey, 0, null, (int)registryOptions, GetRegistryKeyAccess( permissionCheck != RegistryKeyPermissionCheck.ReadSubTree ) | (int)_regView, ref secAttrs, out hkResult, out lpdwDisposition );
				if( num == 0 && !hkResult.IsInvalid )
				{
					RegistryKey registryKey = new RegistryKey( hkResult, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree, systemkey: false, _remoteKey, isPerfData: false, _regView );
					registryKey._checkMode = permissionCheck;
					if( subkey.Length == 0 )
					{
						registryKey._keyName = _keyName;
					}
					else
					{
						registryKey._keyName = _keyName + "\\" + subkey;
					}
					return registryKey;
				}
				if( num != 0 )
				{
					Win32Error( num, _keyName + "\\" + subkey );
				}
				return null;
			}

			private void DeleteSubKeyCore( string subkey, bool throwOnMissingSubKey )
			{
				int num = Interop.Advapi32.RegDeleteKeyEx( _hkey, subkey, (int)_regView, 0 );
				switch( num )
				{
				case 2:
					if( throwOnMissingSubKey )
					{
						throw new ArgumentException( SR.Arg_RegSubKeyAbsent );
					}
					break;
				default:
					Win32Error( num, null );
					break;
				case 0:
					break;
				}
			}

			private void DeleteSubKeyTreeCore( string subkey )
			{
				int num = Interop.Advapi32.RegDeleteKeyEx( _hkey, subkey, (int)_regView, 0 );
				if( num != 0 )
				{
					Win32Error( num, null );
				}
			}

			private void DeleteValueCore( string name, bool throwOnMissingValue )
			{
				int num = Interop.Advapi32.RegDeleteValue( _hkey, name );
				if( num == 2 || num == 206 )
				{
					if( throwOnMissingValue )
					{
						throw new ArgumentException( SR.Arg_RegSubKeyValueAbsent );
					}
					num = 0;
				}
			}

			private static RegistryKey OpenBaseKeyCore( RegistryHive hKeyHive, RegistryView view )
			{
				IntPtr intPtr = (IntPtr)(int)hKeyHive;
				int num = (int)intPtr & 0xFFFFFFF;
				bool flag = intPtr == HKEY_PERFORMANCE_DATA;
				SafeRegistryHandle hkey = new SafeRegistryHandle( intPtr, flag );
				RegistryKey registryKey = new RegistryKey( hkey, writable: true, systemkey: true, remoteKey: false, flag, view );
				registryKey._checkMode = RegistryKeyPermissionCheck.Default;
				registryKey._keyName = s_hkeyNames[ num ];
				return registryKey;
			}

			private static RegistryKey OpenRemoteBaseKeyCore( RegistryHive hKey, string machineName, RegistryView view )
			{
				int num = (int)( hKey & (RegistryHive)268435455 );
				if( num < 0 || num >= s_hkeyNames.Length || ( (ulong)hKey & 0xFFFFFFF0uL ) != 2147483648u )
				{
					throw new ArgumentException( SR.Arg_RegKeyOutOfRange );
				}
				SafeRegistryHandle result = null;
				int num2 = Interop.Advapi32.RegConnectRegistry( machineName, new SafeRegistryHandle( new IntPtr( (int)hKey ), ownsHandle: false ), out result );
				switch( num2 )
				{
				case 1114:
					throw new ArgumentException( SR.Arg_DllInitFailure );
				default:
					Win32ErrorStatic( num2, null );
					break;
				case 0:
					break;
				}
				if( result.IsInvalid )
				{
					throw new ArgumentException( SR.Format( SR.Arg_RegKeyNoRemoteConnect, machineName ) );
				}
				RegistryKey registryKey = new RegistryKey( result, writable: true, systemkey: false, remoteKey: true, (IntPtr)(int)hKey == HKEY_PERFORMANCE_DATA, view );
				registryKey._checkMode = RegistryKeyPermissionCheck.Default;
				registryKey._keyName = s_hkeyNames[ num ];
				return registryKey;
			}

			private RegistryKey InternalOpenSubKeyCore( string name, RegistryKeyPermissionCheck permissionCheck, int rights )
			{
				SafeRegistryHandle hkResult = null;
				int num = Interop.Advapi32.RegOpenKeyEx( _hkey, name, 0, rights | (int)_regView, out hkResult );
				if( num == 0 && !hkResult.IsInvalid )
				{
					RegistryKey registryKey = new RegistryKey( hkResult, permissionCheck == RegistryKeyPermissionCheck.ReadWriteSubTree, systemkey: false, _remoteKey, isPerfData: false, _regView );
					registryKey._keyName = _keyName + "\\" + name;
					registryKey._checkMode = permissionCheck;
					return registryKey;
				}
				if( num == 5 || num == 1346 )
				{
					throw new SecurityException( SR.Security_RegistryPermission );
				}
				return null;
			}

			private RegistryKey InternalOpenSubKeyCore( string name, bool writable )
			{
				SafeRegistryHandle hkResult = null;
				int num = Interop.Advapi32.RegOpenKeyEx( _hkey, name, 0, GetRegistryKeyAccess( writable ) | (int)_regView, out hkResult );
				if( num == 0 && !hkResult.IsInvalid )
				{
					RegistryKey registryKey = new RegistryKey( hkResult, writable, systemkey: false, _remoteKey, isPerfData: false, _regView );
					registryKey._checkMode = GetSubKeyPermissionCheck( writable );
					registryKey._keyName = _keyName + "\\" + name;
					return registryKey;
				}
				if( num == 5 || num == 1346 )
				{
					throw new SecurityException( SR.Security_RegistryPermission );
				}
				return null;
			}

			internal RegistryKey InternalOpenSubKeyWithoutSecurityChecksCore( string name, bool writable )
			{
				SafeRegistryHandle hkResult = null;
				if( Interop.Advapi32.RegOpenKeyEx( _hkey, name, 0, GetRegistryKeyAccess( writable ) | (int)_regView, out hkResult ) == 0 && !hkResult.IsInvalid )
				{
					RegistryKey registryKey = new RegistryKey( hkResult, writable, systemkey: false, _remoteKey, isPerfData: false, _regView );
					registryKey._keyName = _keyName + "\\" + name;
					return registryKey;
				}
				return null;
			}

			private int InternalSubKeyCountCore()
			{
				int lpcSubKeys = 0;
				int lpcValues = 0;
				int num = Interop.Advapi32.RegQueryInfoKey( _hkey, null, null, IntPtr.Zero, ref lpcSubKeys, null, null, ref lpcValues, null, null, null, null );
				if( num != 0 )
				{
					Win32Error( num, null );
				}
				return lpcSubKeys;
			}

			private string[] InternalGetSubKeyNamesCore( int subkeys )
			{
				List<string> list = new List<string>( subkeys );
				char[] array = ArrayPool<char>.Shared.Rent( 256 );
				try
				{
					int lpcbName = array.Length;
					int num;
					while( ( num = Interop.Advapi32.RegEnumKeyEx( _hkey, list.Count, array, ref lpcbName, null, null, null, null ) ) != 259 )
					{
						if( num == 0 )
						{
							list.Add( new string( array, 0, lpcbName ) );
							lpcbName = array.Length;
						}
						else
						{
							Win32Error( num, null );
						}
					}
				}
				finally
				{
					ArrayPool<char>.Shared.Return( array );
				}
				return list.ToArray();
			}

			private int InternalValueCountCore()
			{
				int lpcValues = 0;
				int lpcSubKeys = 0;
				int num = Interop.Advapi32.RegQueryInfoKey( _hkey, null, null, IntPtr.Zero, ref lpcSubKeys, null, null, ref lpcValues, null, null, null, null );
				if( num != 0 )
				{
					Win32Error( num, null );
				}
				return lpcValues;
			}

			private unsafe string[] GetValueNamesCore( int values )
			{
				List<string> list = new List<string>( values );
				char[] array = ArrayPool<char>.Shared.Rent( 100 );
				try
				{
					int lpcbValueName = array.Length;
					int num;
					while( ( num = Interop.Advapi32.RegEnumValue( _hkey, list.Count, array, ref lpcbValueName, IntPtr.Zero, null, null, null ) ) != 259 )
					{
						switch( num )
						{
						case 0:
							list.Add( new string( array, 0, lpcbValueName ) );
							break;
						case 234:
							if( IsPerfDataKey() )
							{
								try
								{
									fixed( char* value = &array[ 0 ] )
									{
										list.Add( new string( value ) );
									}
								}
								finally
								{
								}
							}
							else
							{
								char[] array2 = array;
								int num2 = array2.Length;
								array = null;
								ArrayPool<char>.Shared.Return( array2 );
								array = ArrayPool<char>.Shared.Rent( checked(num2 * 2) );
							}
							break;
						default:
							Win32Error( num, null );
							break;
						}
						lpcbValueName = array.Length;
					}
				}
				finally
				{
					if( array != null )
					{
						ArrayPool<char>.Shared.Return( array );
					}
				}
				return list.ToArray();
			}

			private object InternalGetValueCore( string name, object defaultValue, bool doNotExpand )
			{
				object obj = defaultValue;
				int lpType = 0;
				int lpcbData = 0;
				int num = Interop.Advapi32.RegQueryValueEx( _hkey, name, (int[])null, ref lpType, (byte[])null, ref lpcbData );
				if( num != 0 )
				{
					if( IsPerfDataKey() )
					{
						int num2 = 65000;
						int lpcbData2 = num2;
						byte[] array = new byte[ num2 ];
						int num3;
						while( 234 == ( num3 = Interop.Advapi32.RegQueryValueEx( _hkey, name, null, ref lpType, array, ref lpcbData2 ) ) )
						{
							if( num2 == int.MaxValue )
							{
								Win32Error( num3, name );
							}
							else
							{
								num2 = ( ( num2 <= 1073741823 ) ? ( num2 * 2 ) : int.MaxValue );
							}
							lpcbData2 = num2;
							array = new byte[ num2 ];
						}
						if( num3 != 0 )
						{
							Win32Error( num3, name );
						}
						return array;
					}
					if( num != 234 )
					{
						return obj;
					}
				}
				if( lpcbData < 0 )
				{
					lpcbData = 0;
				}
				switch( lpType )
				{
				case 0:
				case 3:
				case 5:
					{
						byte[] array4 = new byte[ lpcbData ];
						num = Interop.Advapi32.RegQueryValueEx( _hkey, name, null, ref lpType, array4, ref lpcbData );
						obj = array4;
						break;
					}
				case 11:
					if( lpcbData <= 8 )
					{
						long lpData = 0L;
						num = Interop.Advapi32.RegQueryValueEx( _hkey, name, null, ref lpType, ref lpData, ref lpcbData );
						obj = lpData;
						break;
					}
					goto case 0;
				case 4:
					if( lpcbData <= 4 )
					{
						int lpData2 = 0;
						num = Interop.Advapi32.RegQueryValueEx( _hkey, name, null, ref lpType, ref lpData2, ref lpcbData );
						obj = lpData2;
						break;
					}
					goto case 11;
				case 1:
					{
						if( lpcbData % 2 == 1 )
						{
							try
							{
								lpcbData = checked(lpcbData + 1);
							}
							catch( OverflowException innerException2 )
							{
								throw new IOException( SR.Arg_RegGetOverflowBug, innerException2 );
							}
						}
						char[] array5 = new char[ lpcbData / 2 ];
						num = Interop.Advapi32.RegQueryValueEx( _hkey, name, null, ref lpType, array5, ref lpcbData );
						obj = ( ( array5.Length == 0 || array5[ array5.Length - 1 ] != 0 ) ? new string( array5 ) : new string( array5, 0, array5.Length - 1 ) );
						break;
					}
				case 2:
					{
						if( lpcbData % 2 == 1 )
						{
							try
							{
								lpcbData = checked(lpcbData + 1);
							}
							catch( OverflowException innerException3 )
							{
								throw new IOException( SR.Arg_RegGetOverflowBug, innerException3 );
							}
						}
						char[] array6 = new char[ lpcbData / 2 ];
						num = Interop.Advapi32.RegQueryValueEx( _hkey, name, null, ref lpType, array6, ref lpcbData );
						obj = ( ( array6.Length == 0 || array6[ array6.Length - 1 ] != 0 ) ? new string( array6 ) : new string( array6, 0, array6.Length - 1 ) );
						if( !doNotExpand )
						{
							obj = Environment.ExpandEnvironmentVariables( (string)obj );
						}
						break;
					}
				case 7:
					{
						if( lpcbData % 2 == 1 )
						{
							try
							{
								lpcbData = checked(lpcbData + 1);
							}
							catch( OverflowException innerException )
							{
								throw new IOException( SR.Arg_RegGetOverflowBug, innerException );
							}
						}
						char[] array2 = new char[ lpcbData / 2 ];
						num = Interop.Advapi32.RegQueryValueEx( _hkey, name, null, ref lpType, array2, ref lpcbData );
						if( array2.Length != 0 && array2[ array2.Length - 1 ] != 0 )
						{
							Array.Resize( ref array2, array2.Length + 1 );
						}
						string[] array3 = Array.Empty<string>();
						int num4 = 0;
						int num5 = 0;
						int num6 = array2.Length;
						while( num == 0 && num5 < num6 )
						{
							int i;
							for( i = num5; i < num6 && array2[ i ] != 0; i++ )
							{
							}
							string text = null;
							if( i < num6 )
							{
								if( i - num5 > 0 )
								{
									text = new string( array2, num5, i - num5 );
								}
								else if( i != num6 - 1 )
								{
									text = string.Empty;
								}
							}
							else
							{
								text = new string( array2, num5, num6 - num5 );
							}
							num5 = i + 1;
							if( text != null )
							{
								if( array3.Length == num4 )
								{
									Array.Resize( ref array3, ( num4 > 0 ) ? ( num4 * 2 ) : 4 );
								}
								array3[ num4++ ] = text;
							}
						}
						Array.Resize( ref array3, num4 );
						obj = array3;
						break;
					}
				}
				return obj;
			}

			private RegistryValueKind GetValueKindCore( string name )
			{
				int lpType = 0;
				int lpcbData = 0;
				int num = Interop.Advapi32.RegQueryValueEx( _hkey, name, (int[])null, ref lpType, (byte[])null, ref lpcbData );
				if( num != 0 )
				{
					Win32Error( num, null );
				}
				if( lpType != 0 )
				{
					if( Enum.IsDefined( typeof( RegistryValueKind ), lpType ) )
					{
						return (RegistryValueKind)lpType;
					}
					return RegistryValueKind.Unknown;
				}
				return RegistryValueKind.None;
			}

			private void SetValueCore( string name, object value, RegistryValueKind valueKind )
			{
				int num = 0;
				try
				{
					switch( valueKind )
					{
					case RegistryValueKind.String:
					case RegistryValueKind.ExpandString:
						{
							string text = value.ToString();
							num = Interop.Advapi32.RegSetValueEx( _hkey, name, 0, (int)valueKind, text, checked(text.Length * 2 + 2) );
							break;
						}
					case RegistryValueKind.MultiString:
						{
							string[] array2 = (string[])( (string[])value ).Clone();
							int num2 = 1;
							for( int i = 0; i < array2.Length; i++ )
							{
								if( array2[ i ] == null )
								{
									throw new ArgumentException( SR.Arg_RegSetStrArrNull );
								}
								num2 = checked(num2 + ( array2[ i ].Length + 1 ));
							}
							int cbData = checked(num2 * 2);
							char[] array3 = new char[ num2 ];
							int num3 = 0;
							for( int j = 0; j < array2.Length; j++ )
							{
								int length = array2[ j ].Length;
								array2[ j ].CopyTo( 0, array3, num3, length );
								num3 += length + 1;
							}
							num = Interop.Advapi32.RegSetValueEx( _hkey, name, 0, 7, array3, cbData );
							break;
						}
					case RegistryValueKind.None:
					case RegistryValueKind.Binary:
						{
							byte[] array = (byte[])value;
							num = Interop.Advapi32.RegSetValueEx( _hkey, name, 0, ( valueKind != RegistryValueKind.None ) ? 3 : 0, array, array.Length );
							break;
						}
					case RegistryValueKind.DWord:
						{
							int lpData2 = Convert.ToInt32( value, CultureInfo.InvariantCulture );
							num = Interop.Advapi32.RegSetValueEx( _hkey, name, 0, 4, ref lpData2, 4 );
							break;
						}
					case RegistryValueKind.QWord:
						{
							long lpData = Convert.ToInt64( value, CultureInfo.InvariantCulture );
							num = Interop.Advapi32.RegSetValueEx( _hkey, name, 0, 11, ref lpData, 8 );
							break;
						}
					case RegistryValueKind.Unknown:
					case (RegistryValueKind)5:
					case (RegistryValueKind)6:
					case (RegistryValueKind)8:
					case (RegistryValueKind)9:
					case (RegistryValueKind)10:
						break;
					}
				}
				catch( Exception ex ) when( ex is OverflowException || ex is InvalidOperationException || ex is FormatException || ex is InvalidCastException )
				{
					throw new ArgumentException( SR.Arg_RegSetMismatchedKind );
				}
				if( num == 0 )
				{
					SetDirty();
				}
				else
				{
					Win32Error( num, null );
				}
			}

			private void Win32Error( int errorCode, string str )
			{
				switch( errorCode )
				{
				case 5:
					throw ( str != null ) ? new UnauthorizedAccessException( SR.Format( SR.UnauthorizedAccess_RegistryKeyGeneric_Key, str ) ) : new UnauthorizedAccessException();
				case 6:
					if( !IsPerfDataKey() )
					{
						_hkey.SetHandleAsInvalid();
						_hkey = null;
					}
					break;
				case 2:
					throw new IOException( SR.Arg_RegKeyNotFound, errorCode );
				}
				throw new IOException( Interop.Kernel32.GetMessage( errorCode ), errorCode );
			}

			private static void Win32ErrorStatic( int errorCode, string str )
			{
				if( errorCode == 5 )
				{
					throw ( str != null ) ? new UnauthorizedAccessException( SR.Format( SR.UnauthorizedAccess_RegistryKeyGeneric_Key, str ) ) : new UnauthorizedAccessException();
				}
				throw new IOException( Interop.Kernel32.GetMessage( errorCode ), errorCode );
			}

			private static int GetRegistryKeyAccess( bool isWritable )
			{
				if( !isWritable )
				{
					return 131097;
				}
				return 131103;
			}

			private static int GetRegistryKeyAccess( RegistryKeyPermissionCheck mode )
			{
				int result = 0;
				switch( mode )
				{
				case RegistryKeyPermissionCheck.Default:
				case RegistryKeyPermissionCheck.ReadSubTree:
					result = 131097;
					break;
				case RegistryKeyPermissionCheck.ReadWriteSubTree:
					result = 131103;
					break;
				}
				return result;
			}
		}

		public enum RegistryKeyPermissionCheck
		{
			Default,
			ReadSubTree,
			ReadWriteSubTree
		}

		[Flags]
		public enum RegistryOptions
		{
			None = 0x0,
			Volatile = 0x1
		}

		public enum RegistryValueKind
		{
			String = 1,
			ExpandString = 2,
			Binary = 3,
			DWord = 4,
			MultiString = 7,
			QWord = 11,
			Unknown = 0,
			None = -1
		}

		[Flags]
		public enum RegistryValueOptions
		{
			None = 0x0,
			DoNotExpandEnvironmentNames = 0x1
		}

		public enum RegistryView
		{
			Default = 0,
			Registry64 = 0x100,
			Registry32 = 0x200
		}

		//class System
		//{

		internal static class SR
		{
			//private static ResourceManager s_resourceManager;

			//internal static ResourceManager ResourceManager => s_resourceManager ?? ( s_resourceManager = new ResourceManager( typeof( FxResources.Microsoft.Win32.Registry.SR ) ) );

			internal static string AccessControl_InvalidHandle => GetResourceString( "AccessControl_InvalidHandle" );

			internal static string Arg_RegSubKeyAbsent => GetResourceString( "Arg_RegSubKeyAbsent" );

			internal static string Arg_RegKeyDelHive => GetResourceString( "Arg_RegKeyDelHive" );

			internal static string Arg_RegKeyNoRemoteConnect => GetResourceString( "Arg_RegKeyNoRemoteConnect" );

			internal static string Arg_RegKeyOutOfRange => GetResourceString( "Arg_RegKeyOutOfRange" );

			internal static string Arg_RegKeyNotFound => GetResourceString( "Arg_RegKeyNotFound" );

			internal static string Arg_RegKeyStrLenBug => GetResourceString( "Arg_RegKeyStrLenBug" );

			internal static string Arg_RegValStrLenBug => GetResourceString( "Arg_RegValStrLenBug" );

			internal static string Arg_RegBadKeyKind => GetResourceString( "Arg_RegBadKeyKind" );

			internal static string Arg_RegGetOverflowBug => GetResourceString( "Arg_RegGetOverflowBug" );

			internal static string Arg_RegSetMismatchedKind => GetResourceString( "Arg_RegSetMismatchedKind" );

			internal static string Arg_RegSetBadArrType => GetResourceString( "Arg_RegSetBadArrType" );

			internal static string Arg_RegSetStrArrNull => GetResourceString( "Arg_RegSetStrArrNull" );

			internal static string Arg_RegInvalidKeyName => GetResourceString( "Arg_RegInvalidKeyName" );

			internal static string Arg_DllInitFailure => GetResourceString( "Arg_DllInitFailure" );

			internal static string Arg_EnumIllegalVal => GetResourceString( "Arg_EnumIllegalVal" );

			internal static string Arg_RegSubKeyValueAbsent => GetResourceString( "Arg_RegSubKeyValueAbsent" );

			internal static string Argument_InvalidRegistryOptionsCheck => GetResourceString( "Argument_InvalidRegistryOptionsCheck" );

			internal static string Argument_InvalidRegistryViewCheck => GetResourceString( "Argument_InvalidRegistryViewCheck" );

			internal static string Argument_InvalidRegistryKeyPermissionCheck => GetResourceString( "Argument_InvalidRegistryKeyPermissionCheck" );

			internal static string InvalidOperation_RegRemoveSubKey => GetResourceString( "InvalidOperation_RegRemoveSubKey" );

			internal static string ObjectDisposed_RegKeyClosed => GetResourceString( "ObjectDisposed_RegKeyClosed" );

			internal static string Security_RegistryPermission => GetResourceString( "Security_RegistryPermission" );

			internal static string UnauthorizedAccess_RegistryKeyGeneric_Key => GetResourceString( "UnauthorizedAccess_RegistryKeyGeneric_Key" );

			internal static string UnauthorizedAccess_RegistryNoWrite => GetResourceString( "UnauthorizedAccess_RegistryNoWrite" );

			[MethodImpl( MethodImplOptions.NoInlining )]
			private static bool UsingResourceKeys()
			{
				return false;
			}

			internal static string GetResourceString( string resourceKey, string defaultString = null )
			{
				if( UsingResourceKeys() )
				{
					return defaultString ?? resourceKey;
				}
				string text = null;
				//try
				//{
				//	text = ResourceManager.GetString( resourceKey );
				//}
				//catch( MissingManifestResourceException )
				//{
				//}
				if( defaultString != null && resourceKey.Equals( text ) )
				{
					return defaultString;
				}
				return text;
			}

			internal static string Format( string resourceFormat, object p1 )
			{
				if( UsingResourceKeys() )
				{
					return string.Join( ", ", resourceFormat, p1 );
				}
				return string.Format( resourceFormat, p1 );
			}
		}
		//}

		internal static class Interop
		{
			internal class Advapi32
			{
				[DllImport( "advapi32.dll" )]
				internal static extern int RegCloseKey( IntPtr hKey );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegConnectRegistryW" )]
				internal static extern int RegConnectRegistry( string machineName, SafeRegistryHandle key, out SafeRegistryHandle result );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegCreateKeyExW" )]
				internal static extern int RegCreateKeyEx( SafeRegistryHandle hKey, string lpSubKey, int Reserved, string lpClass, int dwOptions, int samDesired, ref Kernel32.SECURITY_ATTRIBUTES secAttrs, out SafeRegistryHandle hkResult, out int lpdwDisposition );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegDeleteKeyExW" )]
				internal static extern int RegDeleteKeyEx( SafeRegistryHandle hKey, string lpSubKey, int samDesired, int Reserved );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegDeleteValueW" )]
				internal static extern int RegDeleteValue( SafeRegistryHandle hKey, string lpValueName );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegEnumKeyExW" )]
				internal static extern int RegEnumKeyEx( SafeRegistryHandle hKey, int dwIndex, char[] lpName, ref int lpcbName, int[] lpReserved, [Out] char[] lpClass, int[] lpcbClass, long[] lpftLastWriteTime );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegEnumValueW" )]
				internal static extern int RegEnumValue( SafeRegistryHandle hKey, int dwIndex, char[] lpValueName, ref int lpcbValueName, IntPtr lpReserved_MustBeZero, int[] lpType, byte[] lpData, int[] lpcbData );

				[DllImport( "advapi32.dll" )]
				internal static extern int RegFlushKey( SafeRegistryHandle hKey );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegOpenKeyExW" )]
				internal static extern int RegOpenKeyEx( SafeRegistryHandle hKey, string lpSubKey, int ulOptions, int samDesired, out SafeRegistryHandle hkResult );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegOpenKeyExW" )]
				internal static extern int RegOpenKeyEx( IntPtr hKey, string lpSubKey, int ulOptions, int samDesired, out SafeRegistryHandle hkResult );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryInfoKeyW" )]
				internal static extern int RegQueryInfoKey( SafeRegistryHandle hKey, [Out] char[] lpClass, int[] lpcbClass, IntPtr lpReserved_MustBeZero, ref int lpcSubKeys, int[] lpcbMaxSubKeyLen, int[] lpcbMaxClassLen, ref int lpcValues, int[] lpcbMaxValueNameLen, int[] lpcbMaxValueLen, int[] lpcbSecurityDescriptor, int[] lpftLastWriteTime );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW" )]
				internal static extern int RegQueryValueEx( SafeRegistryHandle hKey, string lpValueName, int[] lpReserved, ref int lpType, [Out] byte[] lpData, ref int lpcbData );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW" )]
				internal static extern int RegQueryValueEx( SafeRegistryHandle hKey, string lpValueName, int[] lpReserved, ref int lpType, ref int lpData, ref int lpcbData );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW" )]
				internal static extern int RegQueryValueEx( SafeRegistryHandle hKey, string lpValueName, int[] lpReserved, ref int lpType, ref long lpData, ref int lpcbData );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW" )]
				internal static extern int RegQueryValueEx( SafeRegistryHandle hKey, string lpValueName, int[] lpReserved, ref int lpType, [Out] char[] lpData, ref int lpcbData );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW" )]
				internal static extern int RegSetValueEx( SafeRegistryHandle hKey, string lpValueName, int Reserved, int dwType, byte[] lpData, int cbData );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW" )]
				internal static extern int RegSetValueEx( SafeRegistryHandle hKey, string lpValueName, int Reserved, int dwType, char[] lpData, int cbData );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW" )]
				internal static extern int RegSetValueEx( SafeRegistryHandle hKey, string lpValueName, int Reserved, int dwType, ref int lpData, int cbData );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW" )]
				internal static extern int RegSetValueEx( SafeRegistryHandle hKey, string lpValueName, int Reserved, int dwType, ref long lpData, int cbData );

				[DllImport( "advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW" )]
				internal static extern int RegSetValueEx( SafeRegistryHandle hKey, string lpValueName, int Reserved, int dwType, string lpData, int cbData );
			}

			internal class Kernel32
			{
				internal struct SECURITY_ATTRIBUTES
				{
					internal uint nLength;

					internal IntPtr lpSecurityDescriptor;

					internal BOOL bInheritHandle;
				}

				[DllImport( "kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW", SetLastError = true )]
				private unsafe static extern int FormatMessage( int dwFlags, IntPtr lpSource, uint dwMessageId, int dwLanguageId, void* lpBuffer, int nSize, IntPtr arguments );

				internal static string GetMessage( int errorCode )
				{
					return GetMessage( errorCode, IntPtr.Zero );
				}

				internal unsafe static string GetMessage( int errorCode, IntPtr moduleHandle )
				{
					int flags = 12800;
					if( moduleHandle != IntPtr.Zero )
						flags |= 0x800;

					// First try with a reasonably sized fixed buffer (stackalloc).
					const int stackChars = 256;
					char* stackBuffer = stackalloc char[ stackChars ];
					int chars = FormatMessage( flags, moduleHandle, (uint)errorCode, 0, stackBuffer, stackChars, IntPtr.Zero );
					if( chars > 0 )
						return GetAndTrimString( stackBuffer, chars );

					// If buffer is too small, request system-allocated buffer.
					if( Marshal.GetLastWin32Error() == 122 )
					{
						IntPtr allocated = IntPtr.Zero;
						try
						{
							int chars2 = FormatMessage( flags | 0x100, moduleHandle, (uint)errorCode, 0, &allocated, 0, IntPtr.Zero );
							if( chars2 > 0 && allocated != IntPtr.Zero )
								return GetAndTrimString( (char*)allocated, chars2 );
						}
						finally
						{
							// Note: original code used FreeHGlobal. Keep behavior to avoid changing interop contract here.
							Marshal.FreeHGlobal( allocated );
						}
					}

					return $"Unknown error (0x{errorCode:x})";
				}

				unsafe static string GetAndTrimString( char* buffer, int length )
				{
					int end = length;
					while( end > 0 && buffer[ end - 1 ] <= ' ' )
						end--;

					return new string( buffer, 0, end );
				}

				//internal static string GetMessage( int errorCode )
				//{
				//	return GetMessage( errorCode, IntPtr.Zero );
				//}

				//internal unsafe static string GetMessage( int errorCode, IntPtr moduleHandle )
				//{
				//	int num = 12800;
				//	if( moduleHandle != IntPtr.Zero )
				//		num |= 0x800;

				//	Span<char> span = stackalloc char[ 256 ];
				//	fixed( char* lpBuffer = span )
				//	{
				//		int num2 = FormatMessage( num, moduleHandle, (uint)errorCode, 0, lpBuffer, span.Length, IntPtr.Zero );
				//		if( num2 > 0 )
				//			return GetAndTrimString( span.Slice( 0, num2 ) );
				//	}
				//	if( Marshal.GetLastWin32Error() == 122 )
				//	{
				//		IntPtr intPtr = IntPtr.Zero;
				//		try
				//		{
				//			int num3 = FormatMessage( num | 0x100, moduleHandle, (uint)errorCode, 0, &intPtr, 0, IntPtr.Zero );
				//			if( num3 > 0 )
				//				return GetAndTrimString( new Span<char>( (void*)intPtr, num3 ) );
				//		}
				//		finally
				//		{
				//			Marshal.FreeHGlobal( intPtr );
				//		}
				//	}
				//	return $"Unknown error (0x{errorCode:x})";
				//}

				//static string GetAndTrimString( Span<char> buffer )
				//{
				//	int num = buffer.Length;
				//	while( num > 0 && buffer[ num - 1 ] <= ' ' )
				//		num--;
				//	return buffer.Slice( 0, num ).ToString();
				//}
			}

			internal enum BOOL
			{
				FALSE,
				TRUE
			}
		}

		[Flags]
		public enum RegistryRights
		{
			QueryValues = 0x1,
			SetValue = 0x2,
			CreateSubKey = 0x4,
			EnumerateSubKeys = 0x8,
			Notify = 0x10,
			CreateLink = 0x20,
			ExecuteKey = 0x20019,
			ReadKey = 0x20019,
			WriteKey = 0x20006,
			Delete = 0x10000,
			ReadPermissions = 0x20000,
			ChangePermissions = 0x40000,
			TakeOwnership = 0x80000,
			FullControl = 0xF003F
		}

		public override object GetRegistryValue( string keyName, string valueName, object defaultValue )
		{
			return Registry.GetValue( keyName, valueName, defaultValue );

			//var registry = GetRegistryClass();

			//var method = registry.GetMethod( "GetValue", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof( string ), typeof( string ), typeof( object ) }, null );
			//return method.Invoke( null, new object[] { keyName, valueName, defaultValue } );
		}

		public override void SetRegistryValue( string keyName, string valueName, object value )
		{
			Registry.SetValue( keyName, valueName, value );

			//var registry = GetRegistryClass();

			//var method = registry.GetMethod( "SetValue", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof( string ), typeof( string ), typeof( object ) }, null );
			//method.Invoke( null, new object[] { keyName, valueName, value } );
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!move to special dll

	class LinuxPlatformSpecificUtility : PlatformSpecificUtility
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

		public override object GetRegistryValue( string keyName, string valueName, object defaultValue ) { return defaultValue; }
		public override void SetRegistryValue( string keyName, string valueName, object value ) { }

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
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class MacOSPlatformSpecificUtility : PlatformSpecificUtility
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
	}
}
