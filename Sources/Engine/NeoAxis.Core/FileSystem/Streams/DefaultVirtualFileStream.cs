// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	class DefaultVirtualFileStream : VirtualFileStream
	{
		FileStream stream;
		byte[] tempReadUnmanagedBuffer;

		///////////////////////////////////////////

		public DefaultVirtualFileStream( string realPath )
		{
			stream = new FileStream( realPath, FileMode.Open, FileAccess.Read );
		}

		//public override void Close()
		//{
		//	if( stream != null )
		//		stream.Close();
		//	base.Close();
		//}

		protected override void Dispose( bool disposing )
		{
			if( stream != null )
			{
				stream.Dispose();
				stream = null;
			}
			base.Dispose( disposing );
		}

		public override bool CanRead
		{
			get
			{
				if( stream == null )
					throw new ObjectDisposedException( null );
				return stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				if( stream == null )
					throw new ObjectDisposedException( null );
				return stream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				if( stream == null )
					throw new ObjectDisposedException( null );
				return stream.CanWrite;
			}
		}

		public override void Flush()
		{
			if( stream == null )
				throw new ObjectDisposedException( null );
			stream.Flush();
		}

		public override long Length
		{
			get
			{
				if( stream == null )
					throw new ObjectDisposedException( null );
				return stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				if( stream == null )
					throw new ObjectDisposedException( null );
				return stream.Position;
			}
			set
			{
				if( stream == null )
					throw new ObjectDisposedException( null );
				stream.Position = value;
			}
		}

		public override long Seek( long offset, SeekOrigin origin )
		{
			if( stream == null )
				throw new ObjectDisposedException( null );
			return stream.Seek( offset, origin );
		}

		public override void SetLength( long value )
		{
			if( stream == null )
				throw new ObjectDisposedException( null );
			stream.SetLength( value );
		}

		public override void Write( byte[] buffer, int offset, int count )
		{
			if( stream == null )
				throw new ObjectDisposedException( null );
			stream.Write( buffer, offset, count );
		}

		public override int Read( byte[] buffer, int offset, int count )
		{
			if( stream == null )
				throw new ObjectDisposedException( null );
			return stream.Read( buffer, offset, count );
		}

		public override int ReadUnmanaged( IntPtr buffer, int count )
		{
			if( stream == null )
				throw new ObjectDisposedException( null );

			if( tempReadUnmanagedBuffer == null || tempReadUnmanagedBuffer.Length < count )
				tempReadUnmanagedBuffer = new byte[ count ];

			int readed = stream.Read( tempReadUnmanagedBuffer, 0, count );
			if( readed > 0 )
				Marshal.Copy( tempReadUnmanagedBuffer, 0, buffer, readed );
			return readed;
		}

		public override int ReadByte()
		{
			if( stream == null )
				throw new ObjectDisposedException( null );
			return stream.ReadByte();
		}

	}
}
