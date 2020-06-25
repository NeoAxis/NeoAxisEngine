// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;

namespace NeoAxis.OggVorbisTheora
{
	public static class VorbisFile
	{
		const string library = "NeoAxisCoreNative";
		const CallingConvention convention = CallingConvention.Cdecl;

		///////////////////////////////////////////

		static bool nativeLibraryLoaded;

		static void LoadNativeLibrary()
		{
			if( !nativeLibraryLoaded )
			{
				NativeLibraryManager.PreLoadLibrary( library );
				nativeLibraryLoaded = true;
			}
		}

		///////////////////////////////////////////

		const int NOTOPEN = 0;
		const int PARTOPEN = 1;
		const int OPENED = 2;
		const int STREAMSET = 3;
		const int INITSET = 4;

		///////////////////////////////////////////

		[UnmanagedFunctionPointer( convention )]
		public delegate uint ov_callbacks_read_func( IntPtr ptr, uint size, uint nmemb,
			IntPtr datasource );

		[UnmanagedFunctionPointer( convention )]
		public delegate int ov_callbacks_seek_func( IntPtr datasource, long offset, int whence );

		[UnmanagedFunctionPointer( convention )]
		public delegate int ov_callbacks_close_func( IntPtr datasource );

		[UnmanagedFunctionPointer( convention )]
		public delegate int ov_callbacks_tell_func( IntPtr datasource );

		///////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		public struct ov_callbacks
		{
			public ov_callbacks_read_func read_func;
			public ov_callbacks_seek_func seek_func;
			public ov_callbacks_close_func close_func;
			public ov_callbacks_tell_func tell_func;
		}

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr ov_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ov_destroy( IntPtr vf );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ov_clear( IntPtr vf );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ov_open_callbacks( IntPtr datasource, IntPtr vf,
			IntPtr initial, int ibytes, ov_callbacks callbacks );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern long ov_pcm_total( IntPtr vf, int i );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern double ov_time_total( IntPtr vf, int i );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ov_read( IntPtr vf, IntPtr buffer, int length,
			int bigendianp, int word, int sgned, IntPtr bitstream );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr/*vorbis_info**/ ov_info( IntPtr vf, int link );

		///////////////////////////////////////////

		public class File : IDisposable
		{
			IntPtr native;

			//

			public File()
			{
				LoadNativeLibrary();

				native = ov_create();
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					ov_clear( native );
					ov_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public int open_callbacks( IntPtr datasource, IntPtr initial, int ibytes, ov_callbacks callbacks )
			{
				return ov_open_callbacks( datasource, native, initial, ibytes, callbacks );
			}

			public long pcm_total( int i )
			{
				return ov_pcm_total( native, i );
			}

			public double time_total( int i )
			{
				return ov_time_total( native, i );
			}

			public int read( IntPtr buffer, int length, int bigendianp, int word, int sgned, IntPtr bitstream )
			{
				return ov_read( native, buffer, length, bigendianp, word, sgned, bitstream );
			}

			unsafe public void get_info( int link, out int channels, out int rate )
			{
				IntPtr info = ov_info( native, link );
				channels = vorbis.vorbis_info_get_channels( info );
				rate = vorbis.vorbis_info_get_rate( info );
			}
		}

		///////////////////////////////////////////
	}
}
