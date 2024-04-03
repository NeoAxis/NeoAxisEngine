// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Internal;
using NeoAxis;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace NeoAxis
{
	class PlatformSpecificUtilityUWP : PlatformSpecificUtility
	{
		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		static extern int GetModuleFileName( IntPtr hModule, StringBuilder buffer, int length );

		[DllImport( "kernel32.dll", EntryPoint = "LoadPackagedLibrary", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true )]
		public static extern IntPtr LoadPackagedLibrary( string lpwLibFileName, uint Reserved );

		public PlatformSpecificUtilityUWP()
		{
			SetInstance( this );
		}

		public override string GetExecutableDirectoryPath()
		{
			var installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
			return installedLocation.Path;
			//// alternative:
			//string fileName = Process.GetCurrentProcess().MainModule.FileName;
			//return Path.GetDirectoryName( fileName );
		}

		public override IntPtr LoadLibrary( string path )
		{
			path = VirtualFileSystem.MakePathRelative( path );
			IntPtr result = LoadPackagedLibrary( path, 0 );
			if( result == IntPtr.Zero )
				Debug.Fail( "library loading error" + "\r\nError: " + DebugUtil.GetLastErrorStr() );
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
					object obj = null;
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
					string text = null;
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
					string text = null;
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

		public override string GetClipboardText()
		{
			try
			{
				return Clipboard.GetText();
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
			}
			catch { }
		}
	}
}
