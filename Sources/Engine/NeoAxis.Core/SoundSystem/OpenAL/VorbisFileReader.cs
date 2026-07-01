// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
		VorbisFile.ov_callbacksWeb callbacksWeb = new VorbisFile.ov_callbacksWeb();

		//

		public unsafe VorbisFileReader( VirtualFileStream stream, bool needCloseStream )
		{
			this.stream = stream;
			this.needCloseStream = needCloseStream;

			streamGCHandle = GCHandle.Alloc( stream );

			if(SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
			{
				callbacksWeb.read_func = (delegate* unmanaged[Cdecl]< IntPtr, uint, uint, IntPtr, uint >)&Vorbis_read_funcWeb;
				callbacksWeb.seek_func = (delegate* unmanaged[Cdecl]< IntPtr, long, int, int >)&Vorbis_seek_funcWeb;
				callbacksWeb.tell_func = (delegate* unmanaged[Cdecl]< IntPtr, int >)&Vorbis_tell_funcWeb;
			}
			else
			{
			callbacks.read_func = Vorbis_read_func;
			callbacks.seek_func = Vorbis_seek_func;
			callbacks.tell_func = Vorbis_tell_func;
			}

			////callbacks.close_func = Vorbis_close_func;
		}

		public bool OpenVorbisFile( VorbisFile.File vorbisFile )
		{
			IntPtr datasource = GCHandle.ToIntPtr( streamGCHandle );

			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
			{
				if( vorbisFile.open_callbacksWeb( datasource, IntPtr.Zero, 0, callbacksWeb ) != 0 )
					return false;
			}
			else
			{
			if( vorbisFile.open_callbacks( datasource, IntPtr.Zero, 0, callbacks ) != 0 )
				return false;
			}

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

		static uint Vorbis_read_func( IntPtr ptr, uint size, uint nmemb, IntPtr datasource )
		{
			GCHandle gcHandle = GCHandle.FromIntPtr( datasource );
			VirtualFileStream stream = (VirtualFileStream)gcHandle.Target;

			return (uint)stream.ReadUnmanaged( ptr, (int)size * (int)nmemb ) / size;
		}

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

		static int Vorbis_tell_func( IntPtr datasource )
		{
			GCHandle gcHandle = GCHandle.FromIntPtr( datasource );
			VirtualFileStream stream = (VirtualFileStream)gcHandle.Target;

			return (int)stream.Position;
		}

		[UnmanagedCallersOnly( CallConvs = new[] { typeof( CallConvCdecl ) } )]
		static uint Vorbis_read_funcWeb( IntPtr ptr, uint size, uint nmemb, IntPtr datasource )
		{
			return Vorbis_read_func( ptr, size, nmemb, datasource );
		}

		[UnmanagedCallersOnly( CallConvs = new[] { typeof( CallConvCdecl ) } )]
		static int Vorbis_seek_funcWeb( IntPtr datasource, long offset, int whence )
		{
			return Vorbis_seek_func( datasource, offset, whence );
		}

		[UnmanagedCallersOnly( CallConvs = new[] { typeof( CallConvCdecl ) } )]
		static int Vorbis_tell_funcWeb( IntPtr datasource )
		{
			return Vorbis_tell_func( datasource );
		}
	}
}
