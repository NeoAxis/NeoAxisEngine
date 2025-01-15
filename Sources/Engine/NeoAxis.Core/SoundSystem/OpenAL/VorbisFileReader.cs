// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using NeoAxis;
using NeoAxis.OggVorbisTheora;
using System.Runtime.CompilerServices;

namespace OpenALSoundSystem
{
	class VorbisFileReader : IDisposable
	{
		VirtualFileStream stream;
		bool needCloseStream;

		GCHandle streamGCHandle;

		VorbisFile.ov_callbacks callbacks = new VorbisFile.ov_callbacks();

		//

		public unsafe VorbisFileReader( VirtualFileStream stream, bool needCloseStream )
		{
			this.stream = stream;
			this.needCloseStream = needCloseStream;

			streamGCHandle = GCHandle.Alloc( stream );

#if WEB
			callbacks.read_func = (delegate* unmanaged[Cdecl]< IntPtr, uint, uint, IntPtr, uint >)&Vorbis_read_func;
			callbacks.seek_func = (delegate* unmanaged[Cdecl]< IntPtr, long, int, int >)&Vorbis_seek_func;
			callbacks.tell_func = (delegate* unmanaged[Cdecl]< IntPtr, int >)&Vorbis_tell_func;
#else
			callbacks.read_func = Vorbis_read_func;
			callbacks.seek_func = Vorbis_seek_func;
			callbacks.tell_func = Vorbis_tell_func;
#endif

			////callbacks.close_func = Vorbis_close_func;
		}

		public bool OpenVorbisFile( VorbisFile.File vorbisFile )
		{
			IntPtr datasource = GCHandle.ToIntPtr( streamGCHandle );

			if( vorbisFile.open_callbacks( datasource, IntPtr.Zero, 0, callbacks ) != 0 )
				return false;

			return true;
		}

		public void Dispose()
		{
			streamGCHandle.Free();

			if( needCloseStream && stream != null )
			{
				stream.Dispose();
				needCloseStream = false;
			}
			stream = null;
		}

		public void RewindStreamToBegin()
		{
			stream.Seek( 0, System.IO.SeekOrigin.Begin );
		}

#if WEB
		[UnmanagedCallersOnly( CallConvs = new[] { typeof( CallConvCdecl ) } )]
#endif
		static uint Vorbis_read_func( IntPtr ptr, uint size, uint nmemb, IntPtr datasource )
		{
			GCHandle gcHandle = GCHandle.FromIntPtr( datasource );
			VirtualFileStream stream = (VirtualFileStream)gcHandle.Target;

			return (uint)stream.ReadUnmanaged( ptr, (int)size * (int)nmemb ) / size;
		}

#if WEB
		[UnmanagedCallersOnly( CallConvs = new[] { typeof( CallConvCdecl ) } )]
#endif
		static int Vorbis_seek_func( IntPtr datasource, long offset, int whence )
		{
			GCHandle gcHandle = GCHandle.FromIntPtr( datasource );
			VirtualFileStream stream = (VirtualFileStream)gcHandle.Target;

			const int c_SEEK_CUR = 1;
			const int c_SEEK_END = 2;
			const int c_SEEK_SET = 0;

			SeekOrigin origin = SeekOrigin.Begin;
			switch( whence )
			{
			case c_SEEK_CUR: origin = SeekOrigin.Current; break;
			case c_SEEK_END: origin = SeekOrigin.End; break;
			case c_SEEK_SET: origin = SeekOrigin.Begin; break;
			}

			try
			{
				stream.Seek( offset, origin );
			}
			catch
			{
				return 1;
			}
			return 0;
		}

		//static int Vorbis_close_func( IntPtr datasource )
		//{
		//   return 0;
		//}

#if WEB
		[UnmanagedCallersOnly( CallConvs = new[] { typeof( CallConvCdecl ) } )]
#endif
		static int Vorbis_tell_func( IntPtr datasource )
		{
			GCHandle gcHandle = GCHandle.FromIntPtr( datasource );
			VirtualFileStream stream = (VirtualFileStream)gcHandle.Target;

			return (int)stream.Position;
		}

	}
}
