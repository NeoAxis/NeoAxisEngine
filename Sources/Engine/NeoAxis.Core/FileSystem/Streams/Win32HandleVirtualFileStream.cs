// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace NeoAxis
{
	sealed class Win32HandleVirtualFileStream : VirtualFileStream
	{
		IntPtr handle;
		int position;

		///////////////////////////////////////////

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr CreateFile( string lpFileName, int dwDesiredAccess,
			FileShare dwShareMode, IntPtr securityAttrs, FileMode dwCreationDisposition,
			int dwFlagsAndAttributes, IntPtr hTemplateFile );

		[DllImport( "kernel32.dll", SetLastError = true ), SuppressUnmanagedCodeSecurity]
		static extern bool CloseHandle( IntPtr handle );

		[DllImport( "kernel32.dll", SetLastError = true ), SuppressUnmanagedCodeSecurity]
		static extern int ReadFile( IntPtr handle, IntPtr buffer, int numBytesToRead,
			out int numBytesRead, IntPtr overlapped );

		[DllImport( "kernel32.dll", SetLastError = true ), SuppressUnmanagedCodeSecurity]
		static extern int GetFileSize( IntPtr handle, out int highSize );

		[DllImport( "kernel32.dll", EntryPoint = "SetFilePointer", SetLastError = true ), SuppressUnmanagedCodeSecurity]
		static extern int SetFilePointerWin32( IntPtr handle, int lo, ref int hi, int origin );

		static long SetFilePointer( IntPtr handle, long offset, SeekOrigin origin, out int hr )
		{
			hr = 0;
			int lo = (int)offset;
			int hi = (int)( offset >> 0x20 );
			lo = SetFilePointerWin32( handle, lo, ref hi, (int)origin );
			if( ( lo == -1 ) && ( ( hr = Marshal.GetLastWin32Error() ) != 0 ) )
				return -1L;
			return (long)( ( ( (uint)hi ) << 0x20 ) | ( (uint)lo ) );
		}

		const int GENERIC_READ = -2147483648;
		const int ERROR_FILE_NOT_FOUND = 2;
		const int ERROR_PATH_NOT_FOUND = 3;

		///////////////////////////////////////////

		public Win32HandleVirtualFileStream( string realPath )
		{
			handle = CreateFile( realPath, GENERIC_READ, FileShare.Read, IntPtr.Zero,
				FileMode.Open, 0, IntPtr.Zero );

			if( handle == (IntPtr)( -1 ) )
			{
				handle = IntPtr.Zero;

				int errorCode = Marshal.GetLastWin32Error();

				if( errorCode == ERROR_FILE_NOT_FOUND || errorCode == ERROR_PATH_NOT_FOUND )
					throw new FileNotFoundException( "File not found.", realPath );
				else
					throw new IOException( string.Format( "Opening of a file failed \"{0}\".", realPath ) );
			}
		}

		//public override void Close()
		//{
		//	if( handle != IntPtr.Zero )
		//	{
		//		CloseHandle( handle );
		//		handle = IntPtr.Zero;
		//	}
		//	base.Close();
		//}

		protected override void Dispose( bool disposing )
		{
			if( handle != IntPtr.Zero )
			{
				CloseHandle( handle );
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

				int highSize = 0;
				int fileSize = 0;
				fileSize = GetFileSize( handle, out highSize );
				if( fileSize == -1 )
				{
					int errorCode = Marshal.GetLastWin32Error();
					if( errorCode != 0 )
						throw new IOException( "Getting file length failed." );
				}

				//!!!!так?
				long length = ( highSize << 0x20 ) | fileSize;
				return length;
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

			int errorCode;
			position = (int)SetFilePointer( handle, offset, origin, out errorCode );
			if( position == -1 )
				throw new IOException( "Seeking file length failed." );

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
				fixed ( byte* pBuffer = buffer )
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

			int readed;
			bool error = ReadFile( handle, buffer, count, out readed, IntPtr.Zero ) == 0;
			if( error )
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
			bool error;
			unsafe
			{
				error = ReadFile( handle, (IntPtr)( &b ), 1, out readed, IntPtr.Zero ) == 0;
			}
			if( error )
				throw new IOException( "Reading file failed." );

			if( readed == 0 )
				return -1;

			position++;
			return b;
		}
	}
}
