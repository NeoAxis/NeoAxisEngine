// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	sealed class MacOSXVirtualFileStream : VirtualFileStream
	{
		IntPtr handle;
		int position;

		///////////////////////////////////////////

		[DllImport( "NeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Open",
			CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode )]
		public static extern IntPtr VirtualFileStream_Open( string realPath );

		[DllImport( "NeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Close", CallingConvention = CallingConvention.Cdecl )]
		public static extern void VirtualFileStream_Close( IntPtr handle );

		[DllImport( "NeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Length", CallingConvention = CallingConvention.Cdecl )]
		public static extern int VirtualFileStream_Length( IntPtr handle );

		[DllImport( "NeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Read", CallingConvention = CallingConvention.Cdecl )]
		public static extern int VirtualFileStream_Read( IntPtr handle, IntPtr buffer, int count );

		[DllImport( "NeoAxisCoreNative", EntryPoint = "MacAppNativeWrapper_VirtualFileStream_Seek", CallingConvention = CallingConvention.Cdecl )]
		public static extern int VirtualFileStream_Seek( IntPtr handle, int offset, SeekOrigin origin );

		///////////////////////////////////////////

		public MacOSXVirtualFileStream( string realPath )
		{
			handle = VirtualFileStream_Open( realPath );
			if( handle == IntPtr.Zero )
			{
				if( !File.Exists( realPath ) )
					throw new FileNotFoundException( "File not found.", realPath );
				else
					throw new IOException( string.Format( "Opening of a file failed \"{0}\".", realPath ) );
			}
		}

		//public override void Close()
		//{
		//	if( handle != IntPtr.Zero )
		//	{
		//		VirtualFileStream_Close( handle );
		//		handle = IntPtr.Zero;
		//	}
		//	base.Close();
		//}

		protected override void Dispose( bool disposing )
		{
			if( handle != IntPtr.Zero )
			{
				VirtualFileStream_Close( handle );
				handle = IntPtr.Zero;
			}
			base.Dispose( disposing );
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush() { }

		public override long Length
		{
			get
			{
				if( handle == IntPtr.Zero )
					throw new ObjectDisposedException( null );
				return VirtualFileStream_Length( handle );
			}
		}

		public override long Position
		{
			get
			{
				if( handle == IntPtr.Zero )
					throw new ObjectDisposedException( null );
				return position;
			}
			set
			{
				Seek( value, SeekOrigin.Begin );
			}
		}

		public override long Seek( long offset, SeekOrigin origin )
		{
			if( handle == IntPtr.Zero )
				throw new ObjectDisposedException( null );

			int result = VirtualFileStream_Seek( handle, (int)offset, origin );
			if( result != 0 )
				throw new IOException( "Seeking file length failed." );

			switch( origin )
			{
			case SeekOrigin.Begin:
				position = (int)offset;
				break;
			case SeekOrigin.Current:
				position += (int)offset;
				break;
			case SeekOrigin.End:
				position = (int)Length + (int)offset;
				break;
			}

			return position;
		}

		public override void SetLength( long value )
		{
			throw new NotSupportedException( "The method is not supported." );
		}

		public override void Write( byte[] buffer, int offset, int count )
		{
			throw new NotSupportedException( "The method is not supported." );
		}

		public override int Read( byte[] buffer, int offset, int count )
		{
			if( handle == IntPtr.Zero )
				throw new ObjectDisposedException( null );
			if( buffer.Length - offset < count )
				throw new ArgumentException( "Invalid offset length." );
			if( offset < 0 )
				throw new ArgumentOutOfRangeException( "offset" );

			int ret;
			unsafe
			{
				fixed( byte* pBuffer = buffer )
				{
					ret = ReadUnmanaged( (IntPtr)( pBuffer + offset ), count );
				}
			}
			return ret;
		}

		public override int ReadUnmanaged( IntPtr buffer, int count )
		{
			if( handle == IntPtr.Zero )
				throw new ObjectDisposedException( null );
			if( count < 0 )
				throw new ArgumentOutOfRangeException( "count" );

			if( count == 0 )
				return 0;

			int readed = VirtualFileStream_Read( handle, buffer, count );
			if( readed < 0 )
				throw new IOException( "Reading file failed." );

			position += readed;
			return readed;
		}

		public override int ReadByte()
		{
			if( handle == IntPtr.Zero )
				throw new ObjectDisposedException( null );

			byte b;
			int readed;

			unsafe
			{
				readed = VirtualFileStream_Read( handle, (IntPtr)( &b ), 1 );
			}
			if( readed < 0 )
				throw new IOException( "Reading file failed." );

			if( readed == 0 )
				return -1;

			position++;
			return b;
		}

	}
}
