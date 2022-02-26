// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// Defines a memory mapped file stream for virtual file system.
	/// </summary>
	public class MemoryVirtualFileStream : VirtualFileStream
	{
		byte[] data;
		int position;
		bool closed;

		//

		public MemoryVirtualFileStream( byte[] buffer )
		{
			if( buffer == null )
				throw new ArgumentNullException( "buffer" );

			this.data = buffer;
		}

		//public override void Close()
		//{
		//	closed = true;
		//	base.Close();
		//}

		protected override void Dispose( bool disposing )
		{
			closed = true;
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
			get { return data.Length; }
		}

		public override long Position
		{
			get
			{
				if( closed )
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
			if( closed )
				throw new ObjectDisposedException( null );

			switch( origin )
			{
			case SeekOrigin.Begin:
				if( offset < 0 )
					throw new IOException( "Seek before begin." );
				position = (int)offset;
				break;

			case SeekOrigin.Current:
				if( position + offset < 0 )
					throw new IOException( "Seek before begin." );
				position += (int)offset;
				break;

			case SeekOrigin.End:
				if( data.Length + offset < 0 )
					throw new IOException( "Seek before begin." );
				position = data.Length + (int)offset;
				break;

			default:
				throw new ArgumentException( "Invalid seek origin." );
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
			if( closed )
				throw new ObjectDisposedException( null );
			if( offset < 0 )
				throw new ArgumentOutOfRangeException( "offset" );
			if( count < 0 )
				throw new ArgumentOutOfRangeException( "count" );
			if( buffer.Length - offset < count )
				throw new ArgumentException( "Invalid offset length." );

			int needRead = data.Length - position;
			if( needRead > count )
				needRead = count;
			if( needRead <= 0 )
				return 0;

			if( needRead <= 8 )
			{
				int num2 = needRead;
				while( --num2 >= 0 )
					buffer[ offset + num2 ] = data[ position + num2 ];
			}
			else
			{
				Buffer.BlockCopy( data, position, buffer, offset, needRead );
			}
			position += needRead;
			return needRead;
		}

		public override int ReadUnmanaged( IntPtr buffer, int count )
		{
			if( closed )
				throw new ObjectDisposedException( null );
			if( count < 0 )
				throw new ArgumentOutOfRangeException( "count" );

			int needRead = data.Length - position;
			if( needRead > count )
				needRead = count;
			if( needRead <= 0 )
				return 0;

			Marshal.Copy( data, position, buffer, needRead );
			position += needRead;
			return needRead;
		}

		public override int ReadByte()
		{
			if( closed )
				throw new ObjectDisposedException( null );
			if( position >= data.Length )
				return -1;
			return data[ position++ ];
		}
	}
}
