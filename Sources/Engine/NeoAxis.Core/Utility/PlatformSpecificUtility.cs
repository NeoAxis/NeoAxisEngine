// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

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
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.Buffers;
using System.Security;
using System.Runtime.CompilerServices;

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
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
					{
						//#if MACOS
						Log.Fatal( "PlatformSpecificUtility: Instance: impl." );
						//instance = new MacOSXPlatformSpecificUtility();
						//#endif
					}
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
						Log.Fatal( "PlatformSpecificUtility: Get: Instance must be already initialized." );
					else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
					{
#if !ANDROID && !IOS && !WEB && !UWP
						instance = new LinuxPlatformSpecificUtility();
#endif
					}
					else
					{
#if !ANDROID && !IOS && !WEB && !UWP
						instance = new WindowsPlatformSpecificUtility();
#endif
					}
				}
				return instance;
			}
		}

		///////////////////////////////////////////////

		public abstract string GetExecutableDirectoryPath();
		public abstract IntPtr LoadLibrary( string path );

		public abstract string GetClipboardText();
		public abstract void SetClipboardText( string text );

		public virtual object GetRegistryValue( string keyName, string valueName, object defaultValue ) { return null; }
		public virtual void SetRegistryValue( string keyName, string valueName, object value ) { }
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if !ANDROID && !IOS && !WEB && !UWP
	class WindowsPlatformSpecificUtility : PlatformSpecificUtility
	{
		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern int GetModuleFileName( IntPtr hModule, StringBuilder buffer, int length );

		[DllImport( "kernel32.dll", EntryPoint = "LoadLibrary", CharSet = CharSet.Unicode )]
		static extern IntPtr Win32LoadLibrary( string lpLibFileName );

		[DllImport( "kernel32.dll" )]
		static extern uint GetLastError();

		public override string GetExecutableDirectoryPath()
		{
			var result = "";

			try
			{
				string fileName = Process.GetCurrentProcess().MainModule.FileName;
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
				var remove = VirtualPathUtility.NormalizePath( @"\NeoAxis.Internal\Platforms\Windows\dotnet5" );
				//var remove = VirtualPathUtility.NormalizePath( @"\NeoAxis.Internal\Platforms\Windows\dotnet" );

				var index = result.IndexOf( remove );
				if( index != -1 )
					result = result.Remove( index, remove.Length );
			}

			return result;
		}

		public override IntPtr LoadLibrary( string path )
		{
			var result = Win32LoadLibrary( path );

			//if( result == IntPtr.Zero )
			//{
			//	var error = GetLastError().ToString();
			//Log.Info( "Last error: " + error );
			//}

			return result;
		}

		///////////////////////////////////////////////

		internal static class UnsafeNativeMethods
		{
			[DllImport( "ole32.dll", ExactSpelling = true, SetLastError = true )]
			public static extern int OleInitialize( int val = 0 );

			[DllImport( "ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
			public static extern int OleGetClipboard( ref System.Runtime.InteropServices.ComTypes.IDataObject data );

			[DllImport( "ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
			public static extern int OleSetClipboard( System.Runtime.InteropServices.ComTypes.IDataObject pDataObj );

			[DllImport( "ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
			public static extern int OleFlushClipboard();

			[DllImport( "kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
			public static extern IntPtr GlobalAlloc( int uFlags, int dwBytes );

			[DllImport( "kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
			public static extern IntPtr GlobalReAlloc( HandleRef handle, int bytes, int flags );

			[DllImport( "kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
			public static extern IntPtr GlobalLock( HandleRef handle );

			[DllImport( "kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
			public static extern bool GlobalUnlock( HandleRef handle );

			[DllImport( "kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
			public static extern IntPtr GlobalFree( HandleRef handle );

			[DllImport( "kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
			public static extern int GlobalSize( HandleRef handle );

			[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "RtlMoveMemory", ExactSpelling = true )]
			public static extern void CopyMemoryW( IntPtr pdst, string psrc, int cb );

			[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "RtlMoveMemory", ExactSpelling = true )]
			public static extern void CopyMemoryW( IntPtr pdst, char[] psrc, int cb );

			[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
			public static extern int WideCharToMultiByte( int codePage, int flags, [MarshalAs( UnmanagedType.LPWStr )] string wideStr, int chars, [In][Out] byte[] pOutBytes, int bufferBytes, IntPtr defaultChar, IntPtr pDefaultUsed );

			[DllImport( "kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "RtlMoveMemory", ExactSpelling = true, SetLastError = true )]
			public static extern void CopyMemory( HandleRef destData, HandleRef srcData, int size );

			[DllImport( "kernel32.dll", EntryPoint = "RtlMoveMemory", ExactSpelling = true )]
			public static extern void CopyMemory( IntPtr pdst, byte[] psrc, int cb );

		}

		internal static class SafeNativeMethods
		{
			[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
			public static extern int RegisterClipboardFormat( string format );

			[DllImport( "user32.dll", CharSet = CharSet.Auto )]
			public static extern int GetClipboardFormatName( int format, StringBuilder lpString, int cchMax );
		}

		public enum TextDataFormat
		{
			Text,
			UnicodeText,
			Rtf,
			Html,
			CommaSeparatedValue
		}

		[ComVisible( true )]
		public interface IDataObject
		{
			object GetData( string format, bool autoConvert );

			object GetData( string format );

			object GetData( Type format );

			void SetData( string format, bool autoConvert, object data );

			void SetData( string format, object data );

			void SetData( Type format, object data );

			void SetData( object data );

			bool GetDataPresent( string format, bool autoConvert );

			bool GetDataPresent( string format );

			bool GetDataPresent( Type format );

			string[] GetFormats( bool autoConvert );

			string[] GetFormats();
		}

		internal class BackCompatibleStringComparer : IEqualityComparer
		{
			internal static IEqualityComparer Default = new BackCompatibleStringComparer();

			internal BackCompatibleStringComparer()
			{
			}

			public unsafe static int GetHashCode( string obj )
			{
				fixed( char* ptr = obj )
				{
					int num = 5381;
					char* ptr2 = ptr;
					int num2;
					while( ( num2 = *ptr2 ) != 0 )
					{
						num = ( ( num << 5 ) + num ) ^ num2;
						ptr2++;
					}
					return num;
				}
			}

			bool IEqualityComparer.Equals( object a, object b )
			{
				return object.Equals( a, b );
			}

			public virtual int GetHashCode( object o )
			{
				string text = o as string;
				if( text == null )
				{
					return o.GetHashCode();
				}
				return GetHashCode( text );
			}
		}

		public static class DataFormats
		{
			public class Format
			{
				public string Name { get; }

				public int Id { get; }

				public Format( string name, int id )
				{
					Name = name;
					Id = id;
				}
			}

			public static readonly string Text = "Text";

			public static readonly string UnicodeText = "UnicodeText";

			public static readonly string Dib = "DeviceIndependentBitmap";

			public static readonly string Bitmap = "Bitmap";

			public static readonly string EnhancedMetafile = "EnhancedMetafile";

			public static readonly string MetafilePict = "MetaFilePict";

			public static readonly string SymbolicLink = "SymbolicLink";

			public static readonly string Dif = "DataInterchangeFormat";

			public static readonly string Tiff = "TaggedImageFileFormat";

			public static readonly string OemText = "OEMText";

			public static readonly string Palette = "Palette";

			public static readonly string PenData = "PenData";

			public static readonly string Riff = "RiffAudio";

			public static readonly string WaveAudio = "WaveAudio";

			public static readonly string FileDrop = "FileDrop";

			public static readonly string Locale = "Locale";

			public static readonly string Html = "HTML Format";

			public static readonly string Rtf = "Rich Text Format";

			public static readonly string CommaSeparatedValue = "Csv";

			public static readonly string StringFormat = typeof( string ).FullName;

			internal static string WindowsFormsVersion => "WindowsForms10";
			public static readonly string Serializable = /*Application.*/WindowsFormsVersion + "PersistentObject";

			private static Format[] s_formatList;

			private static int s_formatCount = 0;

			private static readonly object s_internalSyncObject = new object();

			public static Format GetFormat( string format )
			{
				lock( s_internalSyncObject )
				{
					EnsurePredefined();
					for( int i = 0; i < s_formatCount; i++ )
					{
						if( s_formatList[ i ].Name.Equals( format ) )
						{
							return s_formatList[ i ];
						}
					}
					for( int j = 0; j < s_formatCount; j++ )
					{
						if( string.Equals( s_formatList[ j ].Name, format, StringComparison.OrdinalIgnoreCase ) )
						{
							return s_formatList[ j ];
						}
					}
					int num = SafeNativeMethods.RegisterClipboardFormat( format );
					if( num == 0 )
					{
						throw new Exception();// Win32Exception( Marshal.GetLastWin32Error(), SR.RegisterCFFailed );
					}
					EnsureFormatSpace( 1 );
					s_formatList[ s_formatCount ] = new Format( format, num );
					return s_formatList[ s_formatCount++ ];
				}
			}

			public static Format GetFormat( int id )
			{
				ushort num = (ushort)( (uint)id & 0xFFFFu );
				lock( s_internalSyncObject )
				{
					EnsurePredefined();
					for( int i = 0; i < s_formatCount; i++ )
					{
						if( s_formatList[ i ].Id == num )
						{
							return s_formatList[ i ];
						}
					}
					StringBuilder stringBuilder = new StringBuilder( 256 );
					if( SafeNativeMethods.GetClipboardFormatName( num, stringBuilder, stringBuilder.Capacity ) == 0 )
					{
						stringBuilder.Length = 0;
						stringBuilder.Append( "Format" ).Append( num );
					}
					EnsureFormatSpace( 1 );
					s_formatList[ s_formatCount ] = new Format( stringBuilder.ToString(), num );
					return s_formatList[ s_formatCount++ ];
				}
			}

			private static void EnsureFormatSpace( int size )
			{
				if( s_formatList == null || s_formatList.Length <= s_formatCount + size )
				{
					Format[] array = new Format[ s_formatCount + 20 ];
					for( int i = 0; i < s_formatCount; i++ )
					{
						array[ i ] = s_formatList[ i ];
					}
					s_formatList = array;
				}
			}

			private static void EnsurePredefined()
			{
				if( s_formatCount == 0 )
				{
					s_formatList = new Format[ 16 ]
					{
					new Format(UnicodeText, 13),
					new Format(Text, 1),
					new Format(Bitmap, 2),
					new Format(MetafilePict, 3),
					new Format(EnhancedMetafile, 14),
					new Format(Dif, 5),
					new Format(Tiff, 6),
					new Format(OemText, 7),
					new Format(Dib, 8),
					new Format(Palette, 9),
					new Format(PenData, 10),
					new Format(Riff, 11),
					new Format(WaveAudio, 12),
					new Format(SymbolicLink, 4),
					new Format(FileDrop, 15),
					new Format(Locale, 16)
					};
					s_formatCount = s_formatList.Length;
				}
			}
		}

		internal static class ClientUtils
		{
			public static bool IsEnumValid( Enum enumValue, int value, int minValue, int maxValue )
			{
				if( value >= minValue )
				{
					return value <= maxValue;
				}
				return false;
			}
		}

		internal static class NativeMethods
		{
			public static bool Failed( int hr )
			{
				return hr < 0;
			}
		}

		[ClassInterface( ClassInterfaceType.None )]
		public class DataObject : IDataObject, System.Runtime.InteropServices.ComTypes.IDataObject
		{
			private class FormatEnumerator : IEnumFORMATETC
			{
				internal IDataObject parent;

				internal ArrayList formats = new ArrayList();

				internal int current;

				public FormatEnumerator( IDataObject parent )
					: this( parent, parent.GetFormats() )
				{
				}

				public FormatEnumerator( IDataObject parent, FORMATETC[] formats )
				{
					this.formats.Clear();
					this.parent = parent;
					current = 0;
					if( formats != null )
					{
						DataObject dataObject = parent as DataObject;
						if( dataObject != null && dataObject.RestrictedFormats && !Clipboard.IsFormatValid( formats ) )
						{
							throw new Exception();// SecurityException( SR.ClipboardSecurityException );
						}
						for( int i = 0; i < formats.Length; i++ )
						{
							FORMATETC fORMATETC = formats[ i ];
							FORMATETC fORMATETC2 = new FORMATETC
							{
								cfFormat = fORMATETC.cfFormat,
								dwAspect = fORMATETC.dwAspect,
								ptd = fORMATETC.ptd,
								lindex = fORMATETC.lindex,
								tymed = fORMATETC.tymed
							};
							this.formats.Add( fORMATETC2 );
						}
					}
				}

				public FormatEnumerator( IDataObject parent, string[] formats )
				{
					this.parent = parent;
					this.formats.Clear();
					if( formats == null )
					{
						return;
					}
					DataObject dataObject = parent as DataObject;
					if( dataObject != null && dataObject.RestrictedFormats && !Clipboard.IsFormatValid( formats ) )
					{
						throw new Exception();// SecurityException( SR.ClipboardSecurityException );
					}
					foreach( string text in formats )
					{
						FORMATETC fORMATETC = new FORMATETC
						{
							cfFormat = (short)(ushort)DataFormats.GetFormat( text ).Id,
							dwAspect = DVASPECT.DVASPECT_CONTENT,
							ptd = IntPtr.Zero,
							lindex = -1
						};
						if( text.Equals( DataFormats.Bitmap ) )
						{
							fORMATETC.tymed = TYMED.TYMED_GDI;
						}
						else if( text.Equals( DataFormats.EnhancedMetafile ) )
						{
							fORMATETC.tymed = TYMED.TYMED_ENHMF;
						}
						else if( text.Equals( DataFormats.Text ) || text.Equals( DataFormats.UnicodeText ) || text.Equals( DataFormats.StringFormat ) || text.Equals( DataFormats.Rtf ) || text.Equals( DataFormats.CommaSeparatedValue ) || text.Equals( DataFormats.FileDrop ) || text.Equals( CF_DEPRECATED_FILENAME ) || text.Equals( CF_DEPRECATED_FILENAMEW ) )
						{
							fORMATETC.tymed = TYMED.TYMED_HGLOBAL;
						}
						else
						{
							fORMATETC.tymed = TYMED.TYMED_HGLOBAL;
						}
						if( fORMATETC.tymed != 0 )
						{
							this.formats.Add( fORMATETC );
						}
					}
				}

				public int Next( int celt, FORMATETC[] rgelt, int[] pceltFetched )
				{
					if( current < formats.Count && celt > 0 )
					{
						FORMATETC fORMATETC = (FORMATETC)formats[ current ];
						rgelt[ 0 ].cfFormat = fORMATETC.cfFormat;
						rgelt[ 0 ].tymed = fORMATETC.tymed;
						rgelt[ 0 ].dwAspect = DVASPECT.DVASPECT_CONTENT;
						rgelt[ 0 ].ptd = IntPtr.Zero;
						rgelt[ 0 ].lindex = -1;
						if( pceltFetched != null )
						{
							pceltFetched[ 0 ] = 1;
						}
						current++;
						return 0;
					}
					if( pceltFetched != null )
					{
						pceltFetched[ 0 ] = 0;
					}
					return 1;
				}

				public int Skip( int celt )
				{
					if( current + celt >= formats.Count )
					{
						return 1;
					}
					current += celt;
					return 0;
				}

				public int Reset()
				{
					current = 0;
					return 0;
				}

				public void Clone( out IEnumFORMATETC ppenum )
				{
					FORMATETC[] array = new FORMATETC[ formats.Count ];
					formats.CopyTo( array, 0 );
					ppenum = new FormatEnumerator( parent, array );
				}
			}

			internal static class Interop
			{
				internal enum HRESULT
				{
					S_OK = 0,
					S_FALSE = 1,
					E_NOTIMPL = -2147467263,
					E_NOINTERFACE = -2147467262,
					E_POINTER = -2147467261,
					E_FAIL = -2147467259,
					DRAGDROP_E_NOTREGISTERED = -2147221248,
					DRAGDROP_E_ALREADYREGISTERED = -2147221247,
					STG_E_INVALIDFUNCTION = -2147287039,
					STG_E_FILENOTFOUND = -2147287038,
					STG_E_ACCESSDENIED = -2147287035,
					STG_E_INVALIDPARAMETER = -2147286953,
					STG_E_INVALIDFLAG = -2147286785,
					E_ACCESSDENIED = -2147024891,
					E_INVALIDARG = -2147024809
				}

				internal static class Ole32
				{
					[Flags]
					public enum STGC : uint
					{
						STGC_DEFAULT = 0x0u,
						STGC_OVERWRITE = 0x1u,
						STGC_ONLYIFCURRENT = 0x2u,
						STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 0x4u,
						STGC_CONSOLIDATE = 0x8u
					}

					public enum STATFLAG : uint
					{
						STATFLAG_DEFAULT,
						STATFLAG_NONAME
					}

					[ComImport]
					[InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
					[Guid( "0000000C-0000-0000-C000-000000000046" )]
					public interface IStream
					{
						unsafe void Read( byte* pv, uint cb, uint* pcbRead );

						unsafe void Write( byte* pv, uint cb, uint* pcbWritten );

						unsafe void Seek( long dlibMove, SeekOrigin dwOrigin, ulong* plibNewPosition );

						void SetSize( ulong libNewSize );

						unsafe void CopyTo( IStream pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten );

						void Commit( STGC grfCommitFlags );

						void Revert();

						[PreserveSig]
						HRESULT LockRegion( ulong libOffset, ulong cb, uint dwLockType );

						[PreserveSig]
						HRESULT UnlockRegion( ulong libOffset, ulong cb, uint dwLockType );

						void Stat( out STATSTG pstatstg, STATFLAG grfStatFlag );

						IStream Clone();
					}
				}

			}


			private class OleConverter : IDataObject
			{
				internal System.Runtime.InteropServices.ComTypes.IDataObject innerData;

				public System.Runtime.InteropServices.ComTypes.IDataObject OleDataObject => innerData;

				public OleConverter( System.Runtime.InteropServices.ComTypes.IDataObject data )
				{
					innerData = data;
				}

				private unsafe object GetDataFromOleIStream( string format )
				{
					FORMATETC formatetc = default( FORMATETC );
					STGMEDIUM medium = default( STGMEDIUM );
					formatetc.cfFormat = (short)(ushort)DataFormats.GetFormat( format ).Id;
					formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
					formatetc.lindex = -1;
					formatetc.tymed = TYMED.TYMED_ISTREAM;
					medium.tymed = TYMED.TYMED_ISTREAM;
					if( QueryGetDataUnsafe( ref formatetc ) != 0 )
					{
						return null;
					}
					try
					{
						innerData.GetData( ref formatetc, out medium );
					}
					catch
					{
						return null;
					}
					if( medium.unionmember != IntPtr.Zero )
					{
						Interop.Ole32.IStream obj2 = (Interop.Ole32.IStream)Marshal.GetObjectForIUnknown( medium.unionmember );
						Marshal.Release( medium.unionmember );
						obj2.Stat( out var pstatstg, Interop.Ole32.STATFLAG.STATFLAG_DEFAULT );
						IntPtr intPtr = UnsafeNativeMethods.GlobalAlloc( 8258, (int)pstatstg.cbSize );
						IntPtr intPtr2 = UnsafeNativeMethods.GlobalLock( new HandleRef( innerData, intPtr ) );
						obj2.Read( (byte*)(void*)intPtr2, (uint)pstatstg.cbSize, null );
						UnsafeNativeMethods.GlobalUnlock( new HandleRef( innerData, intPtr ) );
						return GetDataFromHGLOBAL( format, intPtr );
					}
					return null;
				}

				private object GetDataFromHGLOBAL( string format, IntPtr hglobal )
				{
					object result = null;
					if( hglobal != IntPtr.Zero )
					{
						result = ( ( format.Equals( DataFormats.Text ) || format.Equals( DataFormats.Rtf ) || format.Equals( DataFormats.OemText ) ) ? ReadStringFromHandle( hglobal, unicode: false ) : ( format.Equals( DataFormats.Html ) ? ReadHtmlFromHandle( hglobal ) : ( format.Equals( DataFormats.UnicodeText ) ? ReadStringFromHandle( hglobal, unicode: true ) : ( format.Equals( DataFormats.FileDrop ) ? ReadFileListFromHandle( hglobal ) : ( format.Equals( CF_DEPRECATED_FILENAME ) ? new string[ 1 ] { ReadStringFromHandle( hglobal, unicode: false ) } : ( ( !format.Equals( CF_DEPRECATED_FILENAMEW ) ) ? ReadObjectFromHandle( hglobal, RestrictDeserializationToSafeTypes( format ) ) : new string[ 1 ] { ReadStringFromHandle( hglobal, unicode: true ) } ) ) ) ) ) );
						UnsafeNativeMethods.GlobalFree( new HandleRef( null, hglobal ) );
					}
					return result;
				}

				private object GetDataFromOleHGLOBAL( string format, out bool done )
				{
					done = false;
					FORMATETC formatetc = default( FORMATETC );
					STGMEDIUM medium = default( STGMEDIUM );
					formatetc.cfFormat = (short)(ushort)DataFormats.GetFormat( format ).Id;
					formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
					formatetc.lindex = -1;
					formatetc.tymed = TYMED.TYMED_HGLOBAL;
					medium.tymed = TYMED.TYMED_HGLOBAL;
					object result = null;
					if( QueryGetDataUnsafe( ref formatetc ) == 0 )
					{
						try
						{
							innerData.GetData( ref formatetc, out medium );
							if( !( medium.unionmember != IntPtr.Zero ) )
							{
								return result;
							}
							result = GetDataFromHGLOBAL( format, medium.unionmember );
							return result;
						}
						catch( RestrictedTypeDeserializationException )
						{
							done = true;
							return result;
						}
						catch
						{
							return result;
						}
					}
					return result;
				}

				private object GetDataFromOleOther( string format )
				{
					FORMATETC formatetc = default( FORMATETC );
					STGMEDIUM medium = default( STGMEDIUM );
					TYMED tYMED = TYMED.TYMED_NULL;
					if( format.Equals( DataFormats.Bitmap ) )
					{
						tYMED = TYMED.TYMED_GDI;
					}
					else if( format.Equals( DataFormats.EnhancedMetafile ) )
					{
						tYMED = TYMED.TYMED_ENHMF;
					}
					if( tYMED == TYMED.TYMED_NULL )
					{
						return null;
					}
					formatetc.cfFormat = (short)(ushort)DataFormats.GetFormat( format ).Id;
					formatetc.dwAspect = DVASPECT.DVASPECT_CONTENT;
					formatetc.lindex = -1;
					formatetc.tymed = tYMED;
					medium.tymed = tYMED;
					object result = null;
					if( QueryGetDataUnsafe( ref formatetc ) == 0 )
					{
						try
						{
							innerData.GetData( ref formatetc, out medium );
						}
						catch
						{
						}
					}
					//if( medium.unionmember != IntPtr.Zero && format.Equals( DataFormats.Bitmap ) )
					//{
					//	Image image = Image.FromHbitmap( medium.unionmember );
					//	if( image != null )
					//	{
					//		Image image2 = image;
					//		image = (Image)image.Clone();
					//		Interop.Gdi32.DeleteObject( medium.unionmember );
					//		image2.Dispose();
					//	}
					//	result = image;
					//}
					return result;
				}

				private object GetDataFromBoundOleDataObject( string format, out bool done )
				{
					object obj = null;
					done = false;
					try
					{
						obj = GetDataFromOleOther( format );
						if( obj == null )
						{
							obj = GetDataFromOleHGLOBAL( format, out done );
						}
						if( obj == null )
						{
							if( !done )
							{
								obj = GetDataFromOleIStream( format );
								return obj;
							}
							return obj;
						}
						return obj;
					}
					catch( Exception )
					{
						return obj;
					}
				}

				private Stream ReadByteStreamFromHandle( IntPtr handle, out bool isSerializedObject )
				{
					IntPtr intPtr = UnsafeNativeMethods.GlobalLock( new HandleRef( null, handle ) );
					if( intPtr == IntPtr.Zero )
					{
						throw new Exception();// ExternalException( SR.ExternalException, -2147024882 );
					}
					try
					{
						int num = UnsafeNativeMethods.GlobalSize( new HandleRef( null, handle ) );
						byte[] array = new byte[ num ];
						Marshal.Copy( intPtr, array, 0, num );
						int num2 = 0;
						if( num > serializedObjectID.Length )
						{
							isSerializedObject = true;
							for( int i = 0; i < serializedObjectID.Length; i++ )
							{
								if( serializedObjectID[ i ] != array[ i ] )
								{
									isSerializedObject = false;
									break;
								}
							}
							if( isSerializedObject )
							{
								num2 = serializedObjectID.Length;
							}
						}
						else
						{
							isSerializedObject = false;
						}
						return new MemoryStream( array, num2, array.Length - num2 );
					}
					finally
					{
						UnsafeNativeMethods.GlobalUnlock( new HandleRef( null, handle ) );
					}
				}

				private object ReadObjectFromHandle( IntPtr handle, bool restrictDeserialization )
				{
					//object obj = null;
					bool isSerializedObject;
					Stream stream = ReadByteStreamFromHandle( handle, out isSerializedObject );
					if( isSerializedObject )
					{
						return ReadObjectFromHandleDeserializer( stream, restrictDeserialization );
					}
					return stream;
				}

				private static object ReadObjectFromHandleDeserializer( Stream stream, bool restrictDeserialization )
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					//if( restrictDeserialization )
					//{
					//	binaryFormatter.Binder = new BitmapBinder();
					//}
					//binaryFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
					return binaryFormatter.Deserialize( stream );
				}

				private string[] ReadFileListFromHandle( IntPtr hdrop )
				{
					string[] array = null;
					StringBuilder stringBuilder = new StringBuilder( 260 );
					//int num = UnsafeNativeMethods.DragQueryFile( new HandleRef( null, hdrop ), -1, null, 0 );
					//if( num > 0 )
					//{
					//	array = new string[ num ];
					//	for( int i = 0; i < num; i++ )
					//	{
					//		int num2 = UnsafeNativeMethods.DragQueryFileLongPath( new HandleRef( null, hdrop ), i, stringBuilder );
					//		if( num2 != 0 )
					//		{
					//			string text = stringBuilder.ToString( 0, num2 );
					//			Path.GetFullPath( text );
					//			array[ i ] = text;
					//		}
					//	}
					//}
					return array;
				}

				private unsafe string ReadStringFromHandle( IntPtr handle, bool unicode )
				{
					//string text = null;
					IntPtr intPtr = UnsafeNativeMethods.GlobalLock( new HandleRef( null, handle ) );
					try
					{
						if( unicode )
						{
							return new string( (char*)(void*)intPtr );
						}
						return new string( (sbyte*)(void*)intPtr );
					}
					finally
					{
						UnsafeNativeMethods.GlobalUnlock( new HandleRef( null, handle ) );
					}
				}

				private unsafe string ReadHtmlFromHandle( IntPtr handle )
				{
					//string text = null;
					IntPtr intPtr = UnsafeNativeMethods.GlobalLock( new HandleRef( null, handle ) );
					try
					{
						int num = UnsafeNativeMethods.GlobalSize( new HandleRef( null, handle ) );
						return Encoding.UTF8.GetString( (byte*)(void*)intPtr, num - 1 );
					}
					finally
					{
						UnsafeNativeMethods.GlobalUnlock( new HandleRef( null, handle ) );
					}
				}

				public virtual object GetData( string format, bool autoConvert )
				{
					bool done;
					object dataFromBoundOleDataObject = GetDataFromBoundOleDataObject( format, out done );
					object obj = dataFromBoundOleDataObject;
					if( !done && autoConvert && ( dataFromBoundOleDataObject == null || dataFromBoundOleDataObject is MemoryStream ) )
					{
						string[] mappedFormats = GetMappedFormats( format );
						if( mappedFormats != null )
						{
							int num = 0;
							while( !done && num < mappedFormats.Length )
							{
								if( !format.Equals( mappedFormats[ num ] ) )
								{
									dataFromBoundOleDataObject = GetDataFromBoundOleDataObject( mappedFormats[ num ], out done );
									if( !done && dataFromBoundOleDataObject != null && !( dataFromBoundOleDataObject is MemoryStream ) )
									{
										obj = null;
										break;
									}
								}
								num++;
							}
						}
					}
					if( obj != null )
					{
						return obj;
					}
					return dataFromBoundOleDataObject;
				}

				public virtual object GetData( string format )
				{
					return GetData( format, autoConvert: true );
				}

				public virtual object GetData( Type format )
				{
					return GetData( format.FullName );
				}

				public virtual void SetData( string format, bool autoConvert, object data )
				{
				}

				public virtual void SetData( string format, object data )
				{
					SetData( format, autoConvert: true, data );
				}

				public virtual void SetData( Type format, object data )
				{
					SetData( format.FullName, data );
				}

				public virtual void SetData( object data )
				{
					if( data is ISerializable )
					{
						SetData( DataFormats.Serializable, data );
					}
					else
					{
						SetData( data.GetType(), data );
					}
				}

				private int QueryGetDataUnsafe( ref FORMATETC formatetc )
				{
					return innerData.QueryGetData( ref formatetc );
				}

				private int QueryGetDataInner( ref FORMATETC formatetc )
				{
					return innerData.QueryGetData( ref formatetc );
				}

				public virtual bool GetDataPresent( Type format )
				{
					return GetDataPresent( format.FullName );
				}

				private bool GetDataPresentInner( string format )
				{
					FORMATETC fORMATETC = default( FORMATETC );
					fORMATETC.cfFormat = (short)(ushort)DataFormats.GetFormat( format ).Id;
					fORMATETC.dwAspect = DVASPECT.DVASPECT_CONTENT;
					fORMATETC.lindex = -1;
					FORMATETC formatetc = fORMATETC;
					for( int i = 0; i < ALLOWED_TYMEDS.Length; i++ )
					{
						formatetc.tymed |= ALLOWED_TYMEDS[ i ];
					}
					return QueryGetDataUnsafe( ref formatetc ) == 0;
				}

				public virtual bool GetDataPresent( string format, bool autoConvert )
				{
					bool dataPresentInner = GetDataPresentInner( format );
					if( !dataPresentInner && autoConvert )
					{
						string[] mappedFormats = GetMappedFormats( format );
						if( mappedFormats != null )
						{
							for( int i = 0; i < mappedFormats.Length; i++ )
							{
								if( !format.Equals( mappedFormats[ i ] ) )
								{
									dataPresentInner = GetDataPresentInner( mappedFormats[ i ] );
									if( dataPresentInner )
									{
										break;
									}
								}
							}
						}
					}
					return dataPresentInner;
				}

				public virtual bool GetDataPresent( string format )
				{
					return GetDataPresent( format, autoConvert: true );
				}

				public virtual string[] GetFormats( bool autoConvert )
				{
					IEnumFORMATETC enumFORMATETC = null;
					ArrayList arrayList = new ArrayList();
					try
					{
						enumFORMATETC = innerData.EnumFormatEtc( DATADIR.DATADIR_GET );
					}
					catch
					{
					}
					if( enumFORMATETC != null )
					{
						enumFORMATETC.Reset();
						FORMATETC[] array = new FORMATETC[ 1 ];
						int[] array2 = new int[ 1 ] { 1 };
						while( array2[ 0 ] > 0 )
						{
							array2[ 0 ] = 0;
							try
							{
								enumFORMATETC.Next( 1, array, array2 );
							}
							catch
							{
							}
							if( array2[ 0 ] <= 0 )
							{
								continue;
							}
							string name = DataFormats.GetFormat( array[ 0 ].cfFormat ).Name;
							if( autoConvert )
							{
								string[] mappedFormats = GetMappedFormats( name );
								for( int i = 0; i < mappedFormats.Length; i++ )
								{
									arrayList.Add( mappedFormats[ i ] );
								}
							}
							else
							{
								arrayList.Add( name );
							}
						}
					}
					string[] array3 = new string[ arrayList.Count ];
					arrayList.CopyTo( array3, 0 );
					return GetDistinctStrings( array3 );
				}

				public virtual string[] GetFormats()
				{
					return GetFormats( autoConvert: true );
				}
			}

			private class DataStore : IDataObject
			{
				private class DataStoreEntry
				{
					public object data;

					public bool autoConvert;

					public DataStoreEntry( object data, bool autoConvert )
					{
						this.data = data;
						this.autoConvert = autoConvert;
					}
				}

				private readonly Hashtable data = new Hashtable( BackCompatibleStringComparer.Default );

				public virtual object GetData( string format, bool autoConvert )
				{
					if( string.IsNullOrWhiteSpace( format ) )
					{
						return null;
					}
					DataStoreEntry dataStoreEntry = (DataStoreEntry)data[ format ];
					object obj = null;
					if( dataStoreEntry != null )
					{
						obj = dataStoreEntry.data;
					}
					object obj2 = obj;
					if( autoConvert && ( dataStoreEntry == null || dataStoreEntry.autoConvert ) && ( obj == null || obj is MemoryStream ) )
					{
						string[] mappedFormats = GetMappedFormats( format );
						if( mappedFormats != null )
						{
							for( int i = 0; i < mappedFormats.Length; i++ )
							{
								if( !format.Equals( mappedFormats[ i ] ) )
								{
									DataStoreEntry dataStoreEntry2 = (DataStoreEntry)data[ mappedFormats[ i ] ];
									if( dataStoreEntry2 != null )
									{
										obj = dataStoreEntry2.data;
									}
									if( obj != null && !( obj is MemoryStream ) )
									{
										obj2 = null;
										break;
									}
								}
							}
						}
					}
					if( obj2 != null )
					{
						return obj2;
					}
					return obj;
				}

				public virtual object GetData( string format )
				{
					return GetData( format, autoConvert: true );
				}

				public virtual object GetData( Type format )
				{
					return GetData( format.FullName );
				}

				public virtual void SetData( string format, bool autoConvert, object data )
				{
					if( string.IsNullOrWhiteSpace( format ) )
					{
						if( format == null )
						{
							throw new ArgumentNullException( "format" );
						}
						throw new Exception();// ArgumentException( SR.DataObjectWhitespaceEmptyFormatNotAllowed, "format" );
					}
					//if( data is Bitmap && format.Equals( DataFormats.Dib ) )
					//{
					//	if( !autoConvert )
					//	{
					//		throw new NotSupportedException( SR.DataObjectDibNotSupported );
					//	}
					//	format = DataFormats.Bitmap;
					//}
					this.data[ format ] = new DataStoreEntry( data, autoConvert );
				}

				public virtual void SetData( string format, object data )
				{
					SetData( format, autoConvert: true, data );
				}

				public virtual void SetData( Type format, object data )
				{
					if( format == null )
					{
						throw new ArgumentNullException( "format" );
					}
					SetData( format.FullName, data );
				}

				public virtual void SetData( object data )
				{
					if( data == null )
					{
						throw new ArgumentNullException( "data" );
					}
					if( data is ISerializable && !this.data.ContainsKey( DataFormats.Serializable ) )
					{
						SetData( DataFormats.Serializable, data );
					}
					SetData( data.GetType(), data );
				}

				public virtual bool GetDataPresent( Type format )
				{
					return GetDataPresent( format.FullName );
				}

				public virtual bool GetDataPresent( string format, bool autoConvert )
				{
					if( string.IsNullOrWhiteSpace( format ) )
					{
						return false;
					}
					if( !autoConvert )
					{
						return data.ContainsKey( format );
					}
					string[] formats = GetFormats( autoConvert );
					for( int i = 0; i < formats.Length; i++ )
					{
						if( format.Equals( formats[ i ] ) )
						{
							return true;
						}
					}
					return false;
				}

				public virtual bool GetDataPresent( string format )
				{
					return GetDataPresent( format, autoConvert: true );
				}

				public virtual string[] GetFormats( bool autoConvert )
				{
					string[] array = new string[ data.Keys.Count ];
					data.Keys.CopyTo( array, 0 );
					if( autoConvert )
					{
						ArrayList arrayList = new ArrayList();
						for( int i = 0; i < array.Length; i++ )
						{
							if( ( (DataStoreEntry)data[ array[ i ] ] ).autoConvert )
							{
								string[] mappedFormats = GetMappedFormats( array[ i ] );
								for( int j = 0; j < mappedFormats.Length; j++ )
								{
									arrayList.Add( mappedFormats[ j ] );
								}
							}
							else
							{
								arrayList.Add( array[ i ] );
							}
						}
						string[] array2 = new string[ arrayList.Count ];
						arrayList.CopyTo( array2, 0 );
						array = GetDistinctStrings( array2 );
					}
					return array;
				}

				public virtual string[] GetFormats()
				{
					return GetFormats( autoConvert: true );
				}
			}

			//private class BitmapBinder : SerializationBinder
			//{
			//	private static readonly string s_allowedTypeName = "System.Drawing.Bitmap";

			//	private static readonly string s_allowedAssemblyName = "System.Drawing";

			//	private static readonly byte[] s_allowedToken = new byte[ 8 ] { 176, 63, 95, 127, 17, 213, 10, 58 };

			//	public override Type BindToType( string assemblyName, string typeName )
			//	{
			//		if( string.CompareOrdinal( typeName, s_allowedTypeName ) == 0 )
			//		{
			//			AssemblyName assemblyName2 = null;
			//			try
			//			{
			//				assemblyName2 = new AssemblyName( assemblyName );
			//			}
			//			catch
			//			{
			//			}
			//			if( assemblyName2 != null && string.CompareOrdinal( assemblyName2.Name, s_allowedAssemblyName ) == 0 )
			//			{
			//				byte[] publicKeyToken = assemblyName2.GetPublicKeyToken();
			//				if( publicKeyToken != null && s_allowedToken != null && publicKeyToken.Length == s_allowedToken.Length )
			//				{
			//					bool flag = false;
			//					for( int i = 0; i < s_allowedToken.Length; i++ )
			//					{
			//						if( s_allowedToken[ i ] != publicKeyToken[ i ] )
			//						{
			//							flag = true;
			//							break;
			//						}
			//					}
			//					if( !flag )
			//					{
			//						return null;
			//					}
			//				}
			//			}
			//		}
			//		throw new RestrictedTypeDeserializationException( SR.UnexpectedClipboardType );
			//	}

			//	public override void BindToName( Type serializedType, out string assemblyName, out string typeName )
			//	{
			//		assemblyName = null;
			//		typeName = null;
			//		if( serializedType != null && !serializedType.Equals( typeof( string ) ) && !serializedType.Equals( typeof( Bitmap ) ) )
			//		{
			//			throw new SerializationException( string.Format( SR.UnexpectedTypeForClipboardFormat, serializedType.FullName ) );
			//		}
			//	}
			//}

			private class RestrictedTypeDeserializationException : Exception
			{
				public RestrictedTypeDeserializationException( string message )
					: base( message )
				{
				}
			}

			private static readonly string CF_DEPRECATED_FILENAME = "FileName";

			private static readonly string CF_DEPRECATED_FILENAMEW = "FileNameW";

			private const int DV_E_FORMATETC = -2147221404;

			private const int DV_E_LINDEX = -2147221400;

			private const int DV_E_TYMED = -2147221399;

			private const int DV_E_DVASPECT = -2147221397;

			private const int OLE_E_NOTRUNNING = -2147221499;

			private const int OLE_E_ADVISENOTSUPPORTED = -2147221501;

			private const int DATA_S_SAMEFORMATETC = 262448;

			private static readonly TYMED[] ALLOWED_TYMEDS = new TYMED[ 5 ]
			{
			TYMED.TYMED_HGLOBAL,
			TYMED.TYMED_ISTREAM,
			TYMED.TYMED_ENHMF,
			TYMED.TYMED_MFPICT,
			TYMED.TYMED_GDI
			};

			private readonly IDataObject innerData;

			private static readonly byte[] serializedObjectID = new Guid( "FD9EA796-3B13-4370-A679-56106BB288FB" ).ToByteArray();

			internal bool RestrictedFormats { get; set; }

			internal DataObject( IDataObject data )
			{
				innerData = data;
			}

			internal DataObject( System.Runtime.InteropServices.ComTypes.IDataObject data )
			{
				if( data is DataObject )
				{
					innerData = data as IDataObject;
				}
				else
				{
					innerData = new OleConverter( data );
				}
			}

			public DataObject()
			{
				innerData = new DataStore();
			}

			public DataObject( object data )
			{
				if( data is IDataObject && !Marshal.IsComObject( data ) )
				{
					innerData = (IDataObject)data;
					return;
				}
				if( data is System.Runtime.InteropServices.ComTypes.IDataObject )
				{
					innerData = new OleConverter( (System.Runtime.InteropServices.ComTypes.IDataObject)data );
					return;
				}
				innerData = new DataStore();
				SetData( data );
			}

			public DataObject( string format, object data )
				: this()
			{
				SetData( format, data );
			}

			//private IntPtr GetCompatibleBitmap( Bitmap bm )
			//{
			//	using ScreenDC screenDC = ScreenDC.Create();
			//	IntPtr hbitmap = bm.GetHbitmap();
			//	IntPtr intPtr = Interop.Gdi32.CreateCompatibleDC( screenDC );
			//	IntPtr h = Interop.Gdi32.SelectObject( intPtr, hbitmap );
			//	IntPtr intPtr2 = Interop.Gdi32.CreateCompatibleDC( screenDC );
			//	IntPtr intPtr3 = SafeNativeMethods.CreateCompatibleBitmap( new HandleRef( null, screenDC ), bm.Size.Width, bm.Size.Height );
			//	IntPtr h2 = Interop.Gdi32.SelectObject( intPtr2, intPtr3 );
			//	SafeNativeMethods.BitBlt( new HandleRef( null, intPtr2 ), 0, 0, bm.Size.Width, bm.Size.Height, new HandleRef( null, intPtr ), 0, 0, 13369376 );
			//	Interop.Gdi32.SelectObject( intPtr, h );
			//	Interop.Gdi32.SelectObject( intPtr2, h2 );
			//	Interop.Gdi32.DeleteDC( intPtr );
			//	Interop.Gdi32.DeleteDC( intPtr2 );
			//	return intPtr3;
			//}

			public virtual object GetData( string format, bool autoConvert )
			{
				return innerData.GetData( format, autoConvert );
			}

			public virtual object GetData( string format )
			{
				return GetData( format, autoConvert: true );
			}

			public virtual object GetData( Type format )
			{
				if( format == null )
				{
					return null;
				}
				return GetData( format.FullName );
			}

			public virtual bool GetDataPresent( Type format )
			{
				if( format == null )
				{
					return false;
				}
				return GetDataPresent( format.FullName );
			}

			public virtual bool GetDataPresent( string format, bool autoConvert )
			{
				return innerData.GetDataPresent( format, autoConvert );
			}

			public virtual bool GetDataPresent( string format )
			{
				return GetDataPresent( format, autoConvert: true );
			}

			public virtual string[] GetFormats( bool autoConvert )
			{
				return innerData.GetFormats( autoConvert );
			}

			public virtual string[] GetFormats()
			{
				return GetFormats( autoConvert: true );
			}

			public virtual bool ContainsAudio()
			{
				return GetDataPresent( DataFormats.WaveAudio, autoConvert: false );
			}

			public virtual bool ContainsFileDropList()
			{
				return GetDataPresent( DataFormats.FileDrop, autoConvert: true );
			}

			public virtual bool ContainsImage()
			{
				return GetDataPresent( DataFormats.Bitmap, autoConvert: true );
			}

			public virtual bool ContainsText()
			{
				return ContainsText( TextDataFormat.UnicodeText );
			}

			public virtual bool ContainsText( TextDataFormat format )
			{
				if( !ClientUtils.IsEnumValid( format, (int)format, 0, 4 ) )
				{
					throw new Exception();// InvalidEnumArgumentException( "format", (int)format, typeof( TextDataFormat ) );
				}
				return GetDataPresent( ConvertToDataFormats( format ), autoConvert: false );
			}

			//public virtual Stream GetAudioStream()
			//{
			//	return GetData( DataFormats.WaveAudio, autoConvert: false ) as Stream;
			//}

			//public virtual StringCollection GetFileDropList()
			//{
			//	StringCollection stringCollection = new StringCollection();
			//	string[] array = GetData( DataFormats.FileDrop, autoConvert: true ) as string[];
			//	if( array != null )
			//	{
			//		stringCollection.AddRange( array );
			//	}
			//	return stringCollection;
			//}

			//public virtual Image GetImage()
			//{
			//	return GetData( DataFormats.Bitmap, autoConvert: true ) as Image;
			//}

			public virtual string GetText()
			{
				return GetText( TextDataFormat.UnicodeText );
			}

			public virtual string GetText( TextDataFormat format )
			{
				if( !ClientUtils.IsEnumValid( format, (int)format, 0, 4 ) )
				{
					throw new InvalidEnumArgumentException( "format", (int)format, typeof( TextDataFormat ) );
				}
				string text = GetData( ConvertToDataFormats( format ), autoConvert: false ) as string;
				if( text != null )
				{
					return text;
				}
				return string.Empty;
			}

			//public virtual void SetAudio( byte[] audioBytes )
			//{
			//	if( audioBytes == null )
			//	{
			//		throw new ArgumentNullException( "audioBytes" );
			//	}
			//	SetAudio( new MemoryStream( audioBytes ) );
			//}

			//public virtual void SetAudio( Stream audioStream )
			//{
			//	if( audioStream == null )
			//	{
			//		throw new ArgumentNullException( "audioStream" );
			//	}
			//	SetData( DataFormats.WaveAudio, autoConvert: false, audioStream );
			//}

			//public virtual void SetFileDropList( StringCollection filePaths )
			//{
			//	if( filePaths == null )
			//	{
			//		throw new ArgumentNullException( "filePaths" );
			//	}
			//	string[] array = new string[ filePaths.Count ];
			//	filePaths.CopyTo( array, 0 );
			//	SetData( DataFormats.FileDrop, autoConvert: true, array );
			//}

			//public virtual void SetImage( Image image )
			//{
			//	if( image == null )
			//	{
			//		throw new ArgumentNullException( "image" );
			//	}
			//	SetData( DataFormats.Bitmap, autoConvert: true, image );
			//}

			public virtual void SetText( string textData )
			{
				SetText( textData, TextDataFormat.UnicodeText );
			}

			public virtual void SetText( string textData, TextDataFormat format )
			{
				if( string.IsNullOrEmpty( textData ) )
				{
					throw new ArgumentNullException( "textData" );
				}
				if( !ClientUtils.IsEnumValid( format, (int)format, 0, 4 ) )
				{
					throw new InvalidEnumArgumentException( "format", (int)format, typeof( TextDataFormat ) );
				}
				SetData( ConvertToDataFormats( format ), autoConvert: false, textData );
			}

			private static string ConvertToDataFormats( TextDataFormat format )
			{
				switch( format )
				{
				case TextDataFormat.UnicodeText:
					return DataFormats.UnicodeText;
				case TextDataFormat.Rtf:
					return DataFormats.Rtf;
				case TextDataFormat.Html:
					return DataFormats.Html;
				case TextDataFormat.CommaSeparatedValue:
					return DataFormats.CommaSeparatedValue;
				default:
					return DataFormats.UnicodeText;
				}
			}

			private static string[] GetDistinctStrings( string[] formats )
			{
				ArrayList arrayList = new ArrayList();
				foreach( string text in formats )
				{
					if( !arrayList.Contains( text ) )
					{
						arrayList.Add( text );
					}
				}
				string[] array = new string[ arrayList.Count ];
				arrayList.CopyTo( array, 0 );
				return array;
			}

			private static string[] GetMappedFormats( string format )
			{
				if( format == null )
				{
					return null;
				}
				if( !format.Equals( DataFormats.Text ) && !format.Equals( DataFormats.UnicodeText ) && !format.Equals( DataFormats.StringFormat ) )
				{
					//if( !format.Equals( DataFormats.FileDrop ) && !format.Equals( CF_DEPRECATED_FILENAME ) && !format.Equals( CF_DEPRECATED_FILENAMEW ) )
					//{
					//	if( !format.Equals( DataFormats.Bitmap ) && !format.Equals( typeof( Bitmap ).FullName ) )
					//	{
					//		return new string[ 1 ] { format };
					//	}
					//	return new string[ 2 ]
					//	{
					//		typeof(Bitmap).FullName,
					//		DataFormats.Bitmap
					//	};
					//}
					return new string[ 3 ]
					{
						DataFormats.FileDrop,
						CF_DEPRECATED_FILENAMEW,
						CF_DEPRECATED_FILENAME
					};
				}
				return new string[ 3 ]
				{
					DataFormats.StringFormat,
					DataFormats.UnicodeText,
					DataFormats.Text
				};
			}

			private bool GetTymedUseable( TYMED tymed )
			{
				for( int i = 0; i < ALLOWED_TYMEDS.Length; i++ )
				{
					if( ( tymed & ALLOWED_TYMEDS[ i ] ) != 0 )
					{
						return true;
					}
				}
				return false;
			}

			private void GetDataIntoOleStructs( ref FORMATETC formatetc, ref STGMEDIUM medium )
			{
				if( GetTymedUseable( formatetc.tymed ) && GetTymedUseable( medium.tymed ) )
				{
					string name = DataFormats.GetFormat( formatetc.cfFormat ).Name;
					if( GetDataPresent( name ) )
					{
						object data = GetData( name );
						if( ( formatetc.tymed & TYMED.TYMED_HGLOBAL ) != 0 )
						{
							int num = SaveDataToHandle( data, name, ref medium );
							if( NativeMethods.Failed( num ) )
							{
								Marshal.ThrowExceptionForHR( num );
							}
						}
						else if( ( formatetc.tymed & TYMED.TYMED_GDI ) != 0 )
						{
							//if( name.Equals( DataFormats.Bitmap ) )
							//{
							//	Bitmap bitmap = data as Bitmap;
							//	if( bitmap != null && bitmap != null )
							//	{
							//		medium.unionmember = GetCompatibleBitmap( bitmap );
							//	}
							//}
						}
						else
						{
							Marshal.ThrowExceptionForHR( -2147221399 );
						}
					}
					else
					{
						Marshal.ThrowExceptionForHR( -2147221404 );
					}
				}
				else
				{
					Marshal.ThrowExceptionForHR( -2147221399 );
				}
			}

			int System.Runtime.InteropServices.ComTypes.IDataObject.DAdvise( ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection )
			{
				if( innerData is OleConverter )
				{
					return ( (OleConverter)innerData ).OleDataObject.DAdvise( ref pFormatetc, advf, pAdvSink, out pdwConnection );
				}
				pdwConnection = 0;
				return -2147467263;
			}

			void System.Runtime.InteropServices.ComTypes.IDataObject.DUnadvise( int dwConnection )
			{
				if( innerData is OleConverter )
				{
					( (OleConverter)innerData ).OleDataObject.DUnadvise( dwConnection );
				}
				else
				{
					Marshal.ThrowExceptionForHR( -2147467263 );
				}
			}

			int System.Runtime.InteropServices.ComTypes.IDataObject.EnumDAdvise( out IEnumSTATDATA enumAdvise )
			{
				if( innerData is OleConverter )
				{
					return ( (OleConverter)innerData ).OleDataObject.EnumDAdvise( out enumAdvise );
				}
				enumAdvise = null;
				return -2147221501;
			}

			IEnumFORMATETC System.Runtime.InteropServices.ComTypes.IDataObject.EnumFormatEtc( DATADIR dwDirection )
			{
				if( innerData is OleConverter )
				{
					return ( (OleConverter)innerData ).OleDataObject.EnumFormatEtc( dwDirection );
				}
				if( dwDirection == DATADIR.DATADIR_GET )
				{
					return new FormatEnumerator( this );
				}
				throw new Exception();// ExternalException( SR.ExternalException, -2147467263 );
			}

			int System.Runtime.InteropServices.ComTypes.IDataObject.GetCanonicalFormatEtc( ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut )
			{
				if( innerData is OleConverter )
				{
					return ( (OleConverter)innerData ).OleDataObject.GetCanonicalFormatEtc( ref pformatetcIn, out pformatetcOut );
				}
				pformatetcOut = default( FORMATETC );
				return 262448;
			}

			void System.Runtime.InteropServices.ComTypes.IDataObject.GetData( ref FORMATETC formatetc, out STGMEDIUM medium )
			{
				if( innerData is OleConverter )
				{
					( (OleConverter)innerData ).OleDataObject.GetData( ref formatetc, out medium );
					return;
				}
				medium = default( STGMEDIUM );
				if( GetTymedUseable( formatetc.tymed ) )
				{
					if( ( formatetc.tymed & TYMED.TYMED_HGLOBAL ) != 0 )
					{
						medium.tymed = TYMED.TYMED_HGLOBAL;
						medium.unionmember = UnsafeNativeMethods.GlobalAlloc( 8258, 1 );
						if( medium.unionmember == IntPtr.Zero )
						{
							throw new OutOfMemoryException();
						}
						try
						{
							( (System.Runtime.InteropServices.ComTypes.IDataObject)this ).GetDataHere( ref formatetc, ref medium );
						}
						catch
						{
							UnsafeNativeMethods.GlobalFree( new HandleRef( medium, medium.unionmember ) );
							medium.unionmember = IntPtr.Zero;
							throw;
						}
					}
					else
					{
						medium.tymed = formatetc.tymed;
						( (System.Runtime.InteropServices.ComTypes.IDataObject)this ).GetDataHere( ref formatetc, ref medium );
					}
				}
				else
				{
					Marshal.ThrowExceptionForHR( -2147221399 );
				}
			}

			void System.Runtime.InteropServices.ComTypes.IDataObject.GetDataHere( ref FORMATETC formatetc, ref STGMEDIUM medium )
			{
				if( innerData is OleConverter )
				{
					( (OleConverter)innerData ).OleDataObject.GetDataHere( ref formatetc, ref medium );
				}
				else
				{
					GetDataIntoOleStructs( ref formatetc, ref medium );
				}
			}

			int System.Runtime.InteropServices.ComTypes.IDataObject.QueryGetData( ref FORMATETC formatetc )
			{
				if( innerData is OleConverter )
				{
					return ( (OleConverter)innerData ).OleDataObject.QueryGetData( ref formatetc );
				}
				if( formatetc.dwAspect == DVASPECT.DVASPECT_CONTENT )
				{
					if( GetTymedUseable( formatetc.tymed ) )
					{
						if( formatetc.cfFormat == 0 )
						{
							return 1;
						}
						if( !GetDataPresent( DataFormats.GetFormat( formatetc.cfFormat ).Name ) )
						{
							return -2147221404;
						}
						return 0;
					}
					return -2147221399;
				}
				return -2147221397;
			}

			void System.Runtime.InteropServices.ComTypes.IDataObject.SetData( ref FORMATETC pFormatetcIn, ref STGMEDIUM pmedium, bool fRelease )
			{
				if( innerData is OleConverter )
				{
					( (OleConverter)innerData ).OleDataObject.SetData( ref pFormatetcIn, ref pmedium, fRelease );
					return;
				}
				throw new NotImplementedException();
			}

			private static bool RestrictDeserializationToSafeTypes( string format )
			{
				if( !format.Equals( DataFormats.StringFormat ) && /*!format.Equals( typeof( Bitmap ).FullName ) &&*/ !format.Equals( DataFormats.CommaSeparatedValue ) && !format.Equals( DataFormats.Dib ) && !format.Equals( DataFormats.Dif ) && !format.Equals( DataFormats.Locale ) && !format.Equals( DataFormats.PenData ) && !format.Equals( DataFormats.Riff ) && !format.Equals( DataFormats.SymbolicLink ) && !format.Equals( DataFormats.Tiff ) && !format.Equals( DataFormats.WaveAudio ) && !format.Equals( DataFormats.Bitmap ) && !format.Equals( DataFormats.EnhancedMetafile ) && !format.Equals( DataFormats.Palette ) )
				{
					return format.Equals( DataFormats.MetafilePict );
				}
				return true;
			}

			private int SaveDataToHandle( object data, string format, ref STGMEDIUM medium )
			{
				int result = -2147467259;
				/*if( data is Stream )
				{
					result = SaveStreamToHandle( ref medium.unionmember, (Stream)data );
				}
				else*/
				if( format.Equals( DataFormats.Text ) || format.Equals( DataFormats.Rtf ) || format.Equals( DataFormats.OemText ) )
				{
					result = SaveStringToHandle( medium.unionmember, data.ToString(), unicode: false );
				}
				//else if( format.Equals( DataFormats.Html ) )
				//{
				//	result = SaveHtmlToHandle( medium.unionmember, data.ToString() );
				//}
				else if( format.Equals( DataFormats.UnicodeText ) )
				{
					result = SaveStringToHandle( medium.unionmember, data.ToString(), unicode: true );
				}
				//else if( format.Equals( DataFormats.FileDrop ) )
				//{
				//	result = SaveFileListToHandle( medium.unionmember, (string[])data );
				//}
				else if( format.Equals( CF_DEPRECATED_FILENAME ) )
				{
					string[] array = (string[])data;
					result = SaveStringToHandle( medium.unionmember, array[ 0 ], unicode: false );
				}
				else if( format.Equals( CF_DEPRECATED_FILENAMEW ) )
				{
					string[] array2 = (string[])data;
					result = SaveStringToHandle( medium.unionmember, array2[ 0 ], unicode: true );
				}
				//else if( format.Equals( DataFormats.Dib ) && data is Image )
				//{
				//	result = -2147221399;
				//}
				//else if( format.Equals( DataFormats.Serializable ) || data is ISerializable || ( data != null && data.GetType().IsSerializable ) )
				//{
				//	result = SaveObjectToHandle( ref medium.unionmember, data, RestrictDeserializationToSafeTypes( format ) );
				//}
				return result;
			}

			//private int SaveObjectToHandle( ref IntPtr handle, object data, bool restrictSerialization )
			//{
			//	Stream stream = new MemoryStream();
			//	new BinaryWriter( stream ).Write( serializedObjectID );
			//	SaveObjectToHandleSerializer( stream, data, restrictSerialization );
			//	return SaveStreamToHandle( ref handle, stream );
			//}

			//private static void SaveObjectToHandleSerializer( Stream stream, object data, bool restrictSerialization )
			//{
			//	BinaryFormatter binaryFormatter = new BinaryFormatter();
			//	if( restrictSerialization )
			//	{
			//		binaryFormatter.Binder = new BitmapBinder();
			//	}
			//	binaryFormatter.Serialize( stream, data );
			//}

			//private unsafe int SaveStreamToHandle( ref IntPtr handle, Stream stream )
			//{
			//	if( handle != IntPtr.Zero )
			//	{
			//		UnsafeNativeMethods.GlobalFree( new HandleRef( null, handle ) );
			//	}
			//	int num = (int)stream.Length;
			//	handle = UnsafeNativeMethods.GlobalAlloc( 8194, num );
			//	if( handle == IntPtr.Zero )
			//	{
			//		return -2147024882;
			//	}
			//	IntPtr intPtr = UnsafeNativeMethods.GlobalLock( new HandleRef( null, handle ) );
			//	if( intPtr == IntPtr.Zero )
			//	{
			//		return -2147024882;
			//	}
			//	try
			//	{
			//		Span<byte> buffer = new Span<byte>( intPtr.ToPointer(), num );
			//		stream.Position = 0L;
			//		stream.Read( buffer );
			//	}
			//	finally
			//	{
			//		UnsafeNativeMethods.GlobalUnlock( new HandleRef( null, handle ) );
			//	}
			//	return 0;
			//}

			//private int SaveFileListToHandle( IntPtr handle, string[] files )
			//{
			//	if( files == null )
			//	{
			//		return 0;
			//	}
			//	if( files.Length < 1 )
			//	{
			//		return 0;
			//	}
			//	if( handle == IntPtr.Zero )
			//	{
			//		return -2147024809;
			//	}
			//	IntPtr zero = IntPtr.Zero;
			//	int num = 20;
			//	int num2 = num;
			//	for( int i = 0; i < files.Length; i++ )
			//	{
			//		num2 += ( files[ i ].Length + 1 ) * 2;
			//	}
			//	num2 += 2;
			//	IntPtr intPtr = UnsafeNativeMethods.GlobalReAlloc( new HandleRef( null, handle ), num2, 8194 );
			//	if( intPtr == IntPtr.Zero )
			//	{
			//		return -2147024882;
			//	}
			//	IntPtr intPtr2 = UnsafeNativeMethods.GlobalLock( new HandleRef( null, intPtr ) );
			//	if( intPtr2 == IntPtr.Zero )
			//	{
			//		return -2147024882;
			//	}
			//	zero = intPtr2;
			//	int[] array = new int[ 5 ] { num, 0, 0, 0, 0 };
			//	array[ 4 ] = -1;
			//	Marshal.Copy( array, 0, zero, array.Length );
			//	zero = (IntPtr)( (long)zero + num );
			//	for( int j = 0; j < files.Length; j++ )
			//	{
			//		UnsafeNativeMethods.CopyMemoryW( zero, files[ j ], files[ j ].Length * 2 );
			//		zero = (IntPtr)( (long)zero + files[ j ].Length * 2 );
			//		Marshal.Copy( new byte[ 2 ], 0, zero, 2 );
			//		zero = (IntPtr)( (long)zero + 2 );
			//	}
			//	Marshal.Copy( new char[ 1 ], 0, zero, 1 );
			//	zero = (IntPtr)( (long)zero + 2 );
			//	UnsafeNativeMethods.GlobalUnlock( new HandleRef( null, intPtr ) );
			//	return 0;
			//}

			private int SaveStringToHandle( IntPtr handle, string str, bool unicode )
			{
				if( handle == IntPtr.Zero )
				{
					return -2147024809;
				}
				IntPtr zero = IntPtr.Zero;
				if( unicode )
				{
					int bytes = str.Length * 2 + 2;
					zero = UnsafeNativeMethods.GlobalReAlloc( new HandleRef( null, handle ), bytes, 8258 );
					if( zero == IntPtr.Zero )
					{
						return -2147024882;
					}
					IntPtr intPtr = UnsafeNativeMethods.GlobalLock( new HandleRef( null, zero ) );
					if( intPtr == IntPtr.Zero )
					{
						return -2147024882;
					}
					char[] array = str.ToCharArray( 0, str.Length );
					UnsafeNativeMethods.CopyMemoryW( intPtr, array, array.Length * 2 );
				}
				else
				{
					int num = UnsafeNativeMethods.WideCharToMultiByte( 0, 0, str, str.Length, null, 0, IntPtr.Zero, IntPtr.Zero );
					byte[] array2 = new byte[ num ];
					UnsafeNativeMethods.WideCharToMultiByte( 0, 0, str, str.Length, array2, array2.Length, IntPtr.Zero, IntPtr.Zero );
					zero = UnsafeNativeMethods.GlobalReAlloc( new HandleRef( null, handle ), num + 1, 8258 );
					if( zero == IntPtr.Zero )
					{
						return -2147024882;
					}
					IntPtr intPtr2 = UnsafeNativeMethods.GlobalLock( new HandleRef( null, zero ) );
					if( intPtr2 == IntPtr.Zero )
					{
						return -2147024882;
					}
					UnsafeNativeMethods.CopyMemory( intPtr2, array2, num );
					Marshal.Copy( new byte[ 1 ], 0, (IntPtr)( (long)intPtr2 + num ), 1 );
				}
				if( zero != IntPtr.Zero )
				{
					UnsafeNativeMethods.GlobalUnlock( new HandleRef( null, zero ) );
				}
				return 0;
			}

			//private int SaveHtmlToHandle( IntPtr handle, string str )
			//{
			//	if( handle == IntPtr.Zero )
			//	{
			//		return -2147024809;
			//	}
			//	IntPtr zero = IntPtr.Zero;
			//	byte[] bytes = new UTF8Encoding().GetBytes( str );
			//	zero = UnsafeNativeMethods.GlobalReAlloc( new HandleRef( null, handle ), bytes.Length + 1, 8258 );
			//	if( zero == IntPtr.Zero )
			//	{
			//		return -2147024882;
			//	}
			//	IntPtr intPtr = UnsafeNativeMethods.GlobalLock( new HandleRef( null, zero ) );
			//	if( intPtr == IntPtr.Zero )
			//	{
			//		return -2147024882;
			//	}
			//	try
			//	{
			//		UnsafeNativeMethods.CopyMemory( intPtr, bytes, bytes.Length );
			//		Marshal.Copy( new byte[ 1 ], 0, (IntPtr)( (long)intPtr + bytes.Length ), 1 );
			//	}
			//	finally
			//	{
			//		UnsafeNativeMethods.GlobalUnlock( new HandleRef( null, zero ) );
			//	}
			//	return 0;
			//}

			public virtual void SetData( string format, bool autoConvert, object data )
			{
				innerData.SetData( format, autoConvert, data );
			}

			public virtual void SetData( string format, object data )
			{
				innerData.SetData( format, data );
			}

			public virtual void SetData( Type format, object data )
			{
				innerData.SetData( format, data );
			}

			public virtual void SetData( object data )
			{
				innerData.SetData( data );
			}
		}

		public sealed class Clipboard
		{
			private Clipboard()
			{
			}

			private static bool IsFormatValid( DataObject data )
			{
				return IsFormatValid( data.GetFormats() );
			}

			internal static bool IsFormatValid( string[] formats )
			{
				if( formats != null && formats.Length <= 4 )
				{
					for( int i = 0; i < formats.Length; i++ )
					{
						switch( formats[ i ] )
						{
						case "Text":
						case "UnicodeText":
						case "System.String":
						case "Csv":
							continue;
						}
						return false;
					}
					return true;
				}
				return false;
			}

			internal static bool IsFormatValid( FORMATETC[] formats )
			{
				if( formats != null && formats.Length <= 4 )
				{
					for( int i = 0; i < formats.Length; i++ )
					{
						short cfFormat = formats[ i ].cfFormat;
						if( cfFormat != 1 && cfFormat != 13 && cfFormat != DataFormats.GetFormat( "System.String" ).Id && cfFormat != DataFormats.GetFormat( "Csv" ).Id )
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}

			public static void SetDataObject( object data )
			{
				SetDataObject( data, copy: false );
			}

			public static void SetDataObject( object data, bool copy )
			{
				SetDataObject( data, copy, 10, 100 );
			}

			static bool oleInitializeCalled;

			public static void SetDataObject( object data, bool copy, int retryTimes, int retryDelay )
			{
				if( !oleInitializeCalled )
				{
					oleInitializeCalled = true;
					UnsafeNativeMethods.OleInitialize();
				}

				//if( Application.OleRequired() != 0 )
				//{
				//	throw new Exception();// ThreadStateException( SR.ThreadMustBeSTA );
				//}
				if( data == null )
				{
					throw new ArgumentNullException( "data" );
				}
				//if( retryTimes < 0 )
				//{
				//	throw new ArgumentOutOfRangeException( "retryTimes", retryTimes, string.Format( SR.InvalidLowBoundArgumentEx, "retryTimes", retryTimes, 0 ) );
				//}
				//if( retryDelay < 0 )
				//{
				//	throw new ArgumentOutOfRangeException( "retryDelay", retryDelay, string.Format( SR.InvalidLowBoundArgumentEx, "retryDelay", retryDelay, 0 ) );
				//}
				DataObject dataObject = null;
				if( !( data is System.Runtime.InteropServices.ComTypes.IDataObject ) )
				{
					dataObject = new DataObject( data );
				}
				if( dataObject != null )
				{
					dataObject.RestrictedFormats = false;
				}
				int num = retryTimes;
				int num2;
				do
				{
					num2 = ( ( !( data is System.Runtime.InteropServices.ComTypes.IDataObject ) ) ? UnsafeNativeMethods.OleSetClipboard( dataObject ) : UnsafeNativeMethods.OleSetClipboard( (System.Runtime.InteropServices.ComTypes.IDataObject)data ) );
					if( num2 != 0 )
					{
						if( num == 0 )
						{
							ThrowIfFailed( num2 );
						}
						num--;
						Thread.Sleep( retryDelay );
					}
				}
				while( num2 != 0 );
				if( !copy )
				{
					return;
				}
				num = retryTimes;
				do
				{
					num2 = UnsafeNativeMethods.OleFlushClipboard();
					if( num2 != 0 )
					{
						if( num == 0 )
						{
							ThrowIfFailed( num2 );
						}
						num--;
						Thread.Sleep( retryDelay );
					}
				}
				while( num2 != 0 );
			}

			public static IDataObject GetDataObject()
			{
				if( !oleInitializeCalled )
				{
					oleInitializeCalled = true;
					UnsafeNativeMethods.OleInitialize();
				}

				//if( Application.OleRequired() != 0 )
				//{
				//	//if( Application.MessageLoop )
				//	//{
				//	//	throw new ThreadStateException( SR.ThreadMustBeSTA );
				//	//}
				//	return null;
				//}
				return GetDataObject( 10, 100 );
			}

			private static IDataObject GetDataObject( int retryTimes, int retryDelay )
			{
				System.Runtime.InteropServices.ComTypes.IDataObject data = null;
				int num = retryTimes;
				int num2;
				do
				{
					num2 = UnsafeNativeMethods.OleGetClipboard( ref data );
					if( num2 != 0 )
					{
						if( num == 0 )
						{
							ThrowIfFailed( num2 );
						}
						num--;
						Thread.Sleep( retryDelay );
					}
				}
				while( num2 != 0 );
				if( data != null )
				{
					if( data is IDataObject && !Marshal.IsComObject( data ) )
					{
						return (IDataObject)data;
					}
					return new DataObject( data );
				}
				return null;
			}

			public static void Clear()
			{
				SetDataObject( new DataObject() );
			}

			//public static bool ContainsAudio()
			//{
			//	return GetDataObject()?.GetDataPresent( DataFormats.WaveAudio, autoConvert: false ) ?? false;
			//}

			public static bool ContainsData( string format )
			{
				return GetDataObject()?.GetDataPresent( format, autoConvert: false ) ?? false;
			}

			//public static bool ContainsFileDropList()
			//{
			//	return GetDataObject()?.GetDataPresent( DataFormats.FileDrop, autoConvert: true ) ?? false;
			//}

			//public static bool ContainsImage()
			//{
			//	return GetDataObject()?.GetDataPresent( DataFormats.Bitmap, autoConvert: true ) ?? false;
			//}

			public static bool ContainsText()
			{
				return ContainsText( TextDataFormat.UnicodeText );
			}

			public static bool ContainsText( TextDataFormat format )
			{
				if( !ClientUtils.IsEnumValid( format, (int)format, 0, 4 ) )
				{
					throw new InvalidEnumArgumentException( "format", (int)format, typeof( TextDataFormat ) );
				}
				return GetDataObject()?.GetDataPresent( ConvertToDataFormats( format ), autoConvert: false ) ?? false;
			}

			//public static Stream GetAudioStream()
			//{
			//	IDataObject dataObject = GetDataObject();
			//	if( dataObject != null )
			//	{
			//		return dataObject.GetData( DataFormats.WaveAudio, autoConvert: false ) as Stream;
			//	}
			//	return null;
			//}

			public static object GetData( string format )
			{
				return GetDataObject()?.GetData( format );
			}

			//public static StringCollection GetFileDropList()
			//{
			//	IDataObject dataObject = GetDataObject();
			//	StringCollection stringCollection = new StringCollection();
			//	if( dataObject != null )
			//	{
			//		string[] array = dataObject.GetData( DataFormats.FileDrop, autoConvert: true ) as string[];
			//		if( array != null )
			//		{
			//			stringCollection.AddRange( array );
			//		}
			//	}
			//	return stringCollection;
			//}

			//public static Image GetImage()
			//{
			//	IDataObject dataObject = GetDataObject();
			//	if( dataObject != null )
			//	{
			//		return dataObject.GetData( DataFormats.Bitmap, autoConvert: true ) as Image;
			//	}
			//	return null;
			//}

			public static string GetText()
			{
				return GetText( TextDataFormat.UnicodeText );
			}

			public static string GetText( TextDataFormat format )
			{
				if( !ClientUtils.IsEnumValid( format, (int)format, 0, 4 ) )
				{
					throw new InvalidEnumArgumentException( "format", (int)format, typeof( TextDataFormat ) );
				}
				IDataObject dataObject = GetDataObject();
				if( dataObject != null )
				{
					string text = dataObject.GetData( ConvertToDataFormats( format ), autoConvert: false ) as string;
					if( text != null )
					{
						return text;
					}
				}
				return string.Empty;
			}

			//public static void SetAudio( byte[] audioBytes )
			//{
			//	if( audioBytes == null )
			//	{
			//		throw new ArgumentNullException( "audioBytes" );
			//	}
			//	SetAudio( new MemoryStream( audioBytes ) );
			//}

			//public static void SetAudio( Stream audioStream )
			//{
			//	if( audioStream == null )
			//	{
			//		throw new ArgumentNullException( "audioStream" );
			//	}
			//	DataObject dataObject = new DataObject();
			//	( (IDataObject)dataObject ).SetData( DataFormats.WaveAudio, autoConvert: false, (object)audioStream );
			//	SetDataObject( dataObject, copy: true );
			//}

			public static void SetData( string format, object data )
			{
				DataObject dataObject = new DataObject();
				( (IDataObject)dataObject ).SetData( format, data );
				SetDataObject( dataObject, copy: true );
			}

			//public static void SetFileDropList( StringCollection filePaths )
			//{
			//	if( filePaths == null )
			//	{
			//		throw new ArgumentNullException( "filePaths" );
			//	}
			//	if( filePaths.Count == 0 )
			//	{
			//		throw new ArgumentException( SR.CollectionEmptyException );
			//	}
			//	StringEnumerator enumerator = filePaths.GetEnumerator();
			//	try
			//	{
			//		while( enumerator.MoveNext() )
			//		{
			//			string current = enumerator.Current;
			//			try
			//			{
			//				Path.GetFullPath( current );
			//			}
			//			catch( Exception ex )
			//			{
			//				if( ClientUtils.IsSecurityOrCriticalException( ex ) )
			//				{
			//					throw;
			//				}
			//				throw new ArgumentException( string.Format( SR.Clipboard_InvalidPath, current, "filePaths" ), ex );
			//			}
			//		}
			//	}
			//	finally
			//	{
			//		( enumerator as IDisposable )?.Dispose();
			//	}
			//	if( filePaths.Count > 0 )
			//	{
			//		DataObject dataObject = new DataObject();
			//		string[] array = new string[ filePaths.Count ];
			//		filePaths.CopyTo( array, 0 );
			//		( (IDataObject)dataObject ).SetData( DataFormats.FileDrop, autoConvert: true, (object)array );
			//		SetDataObject( dataObject, copy: true );
			//	}
			//}

			//public static void SetImage( Image image )
			//{
			//	if( image == null )
			//	{
			//		throw new ArgumentNullException( "image" );
			//	}
			//	DataObject dataObject = new DataObject();
			//	( (IDataObject)dataObject ).SetData( DataFormats.Bitmap, autoConvert: true, (object)image );
			//	SetDataObject( dataObject, copy: true );
			//}

			public static void SetText( string text )
			{
				SetText( text, TextDataFormat.UnicodeText );
			}

			public static void SetText( string text, TextDataFormat format )
			{
				if( string.IsNullOrEmpty( text ) )
				{
					throw new ArgumentNullException( "text" );
				}
				if( !ClientUtils.IsEnumValid( format, (int)format, 0, 4 ) )
				{
					throw new InvalidEnumArgumentException( "format", (int)format, typeof( TextDataFormat ) );
				}
				DataObject dataObject = new DataObject();
				( (IDataObject)dataObject ).SetData( ConvertToDataFormats( format ), autoConvert: false, (object)text );
				SetDataObject( dataObject, copy: true );
			}

			private static string ConvertToDataFormats( TextDataFormat format )
			{
				switch( format )
				{
				case TextDataFormat.Text:
					return DataFormats.Text;
				case TextDataFormat.UnicodeText:
					return DataFormats.UnicodeText;
				case TextDataFormat.Rtf:
					return DataFormats.Rtf;
				case TextDataFormat.Html:
					return DataFormats.Html;
				case TextDataFormat.CommaSeparatedValue:
					return DataFormats.CommaSeparatedValue;
				default:
					return DataFormats.UnicodeText;
				}
			}

			private static void ThrowIfFailed( int hr )
			{
				if( hr != 0 )
				{
					throw new ExternalException();// SR.ClipboardOperationFailed, hr );
				}
			}
		}

		///////////////////////////////////////////////

		//static Assembly GetAssemblyByName( string name )
		//{
		//	return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault( assembly => assembly.GetName().Name == name );
		//}

		//static Assembly winFormsAssembly;

		//static Type GetClipboardClass()
		//{
		//	if( winFormsAssembly == null )
		//		winFormsAssembly = GetAssemblyByName( "System.Windows.Forms" );
		//	return winFormsAssembly.GetType( "System.Windows.Forms.Clipboard" );
		//}

		public override string GetClipboardText()
		{
			try
			{
				return Clipboard.GetText();

				//var clipboard = GetClipboardClass();
				//var method = clipboard.GetMethod( "GetText", BindingFlags.Public | BindingFlags.Static, null, new Type[ 0 ], null );
				//return method.Invoke( null, new object[ 0 ] ) as string;
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
				Clipboard.SetText( text );

				//var clipboard = GetClipboardClass();
				//var method = clipboard.GetMethod( "SetText", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof( string ) }, null );
				//method.Invoke( null, new object[] { text } );
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
					int num = 12800;
					if( moduleHandle != IntPtr.Zero )
					{
						num |= 0x800;
					}
					Span<char> span = stackalloc char[ 256 ];
					fixed( char* lpBuffer = span )
					{
						int num2 = FormatMessage( num, moduleHandle, (uint)errorCode, 0, lpBuffer, span.Length, IntPtr.Zero );
						if( num2 > 0 )
						{
							return GetAndTrimString( span.Slice( 0, num2 ) );
						}
					}
					if( Marshal.GetLastWin32Error() == 122 )
					{
						IntPtr intPtr = default( IntPtr );
						try
						{
							int num3 = FormatMessage( num | 0x100, moduleHandle, (uint)errorCode, 0, &intPtr, 0, IntPtr.Zero );
							if( num3 > 0 )
							{
								return GetAndTrimString( new Span<char>( (void*)intPtr, num3 ) );
							}
						}
						finally
						{
							Marshal.FreeHGlobal( intPtr );
						}
					}
					return $"Unknown error (0x{errorCode:x})";
				}

				private static string GetAndTrimString( Span<char> buffer )
				{
					int num = buffer.Length;
					while( num > 0 && buffer[ num - 1 ] <= ' ' )
					{
						num--;
					}
					return buffer.Slice( 0, num ).ToString();
				}
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
#endif

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if !ANDROID && !IOS && !WEB && !UWP
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

		public override IntPtr LoadLibrary( string path )
		{
			var result = IntPtr.Zero;

			//Console.WriteLine( "LoadLibrary: " + path );

			//if( !NativeLibrary.TryLoad( path, out result ) )
			//{
			//	//try with "lib" prefix
			//	var newPath = Path.Combine( Path.GetDirectoryName( path ), "lib" + Path.GetFileName( path ) );

			//	Console.WriteLine( "second: " + newPath );

			//	NativeLibrary.TryLoad( newPath, out result );
			//}

			//Console.WriteLine( "LoadLibrary Result: " + result.ToString() );

			return result;
		}

		///////////////////////////////////////////////

		public override object GetRegistryValue( string keyName, string valueName, object defaultValue ) { return defaultValue; }
		public override void SetRegistryValue( string keyName, string valueName, object value ) { }

		public override string GetClipboardText()
		{
			//!!!!impl
			return "";
		}

		public override void SetClipboardText( string text )
		{
			//!!!!impl
		}
	}
#endif


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//#if MACOS
	//class MacOSXPlatformSpecificUtility : PlatformSpecificUtility
	//{
	//	[DllImport( "NeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_LoadLibrary",
	//		CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode )]
	//	public static extern IntPtr MacLoadLibrary( string name );

	//	public override string GetExecutableDirectoryPath()
	//	{
	//		//old: GetCallingAssembly
	//		string codeBaseURI = Assembly.GetExecutingAssembly().CodeBase;
	//		return Path.GetDirectoryName( codeBaseURI.Replace( "file://", "" ) );
	//	}

	//	public override IntPtr LoadLibrary( string path )
	//	{
	//		return MacLoadLibrary( path );
	//	}
	//}
	//#endif
}
