// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Reflection;

namespace NeoAxis.OggVorbisTheora
{
	public static class vorbis
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

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr vorbis_info_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_info_destroy( IntPtr vi );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		internal static extern int vorbis_info_get_channels( IntPtr vi );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		internal static extern int vorbis_info_get_rate( IntPtr vi );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void vorbis_info_init( IntPtr vi );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void vorbis_info_clear( IntPtr vi );

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr vorbis_dsp_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_dsp_destroy( IntPtr v );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern long vorbis_dsp_get_granulepos( IntPtr v );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void vorbis_dsp_clear( IntPtr/*vorbis_dsp_state* */ v );

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_synthesis_headerin( IntPtr/*vorbis_info* */vi,
			IntPtr/*vorbis_comment* */vc, IntPtr/*ogg_packet* */op );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_synthesis_init( IntPtr/*vorbis_dsp_state* */
			v, IntPtr/*vorbis_info* */vi );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_synthesis_read( IntPtr/*vorbis_dsp_state* */v, int samples );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_synthesis_blockin( IntPtr/*vorbis_dsp_state* */v, IntPtr/*vorbis_block* */vb );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_synthesis( IntPtr/*vorbis_block* */vb, IntPtr/*ogg_packet* */op );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		unsafe static extern int vorbis_synthesis_pcmout( IntPtr/*vorbis_dsp_state* */v,
			[Out] out float** /*float*** */ pcm );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern double vorbis_granule_time( IntPtr/*vorbis_dsp_state* */v, long granulepos );

		///////////////////////////////////////////

		public class Info : IDisposable
		{
			internal IntPtr native;

			public Info()
			{
				LoadNativeLibrary();

				native = vorbis_info_create();
				vorbis_info_init( native );
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					vorbis_info_clear( native );
					vorbis_info_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public int synthesis_headerin( Comment comment, ogg.Packet packet )
			{
				return vorbis_synthesis_headerin( native, comment.native, packet.native );
			}

			public int channels
			{
				get
				{
					return vorbis_info_get_channels( native );
				}
			}

			public int rate
			{
				get
				{
					return vorbis_info_get_rate( native );
				}
			}
		}

		///////////////////////////////////////////

		public class DspState : IDisposable
		{
			internal IntPtr native;

			public DspState( Info info )
			{
				LoadNativeLibrary();

				native = vorbis_dsp_create();
				vorbis_synthesis_init( native, info.native );
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					vorbis_dsp_clear( native );
					vorbis_dsp_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public int synthesis_read( int samples ) { return vorbis_synthesis_read( native, samples ); }
			public int synthesis_blockin( Block block ) { return vorbis_synthesis_blockin( native, block.native ); }
			unsafe public int synthesis_pcmout( out float** pcm ) { return vorbis_synthesis_pcmout( native, out pcm ); }

			public double granule_time( long granulepos )
			{
				return vorbis_granule_time( native, granulepos );
			}

			public long granulepos
			{
				get
				{
					return vorbis_dsp_get_granulepos( native );
				}
			}
		}

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr vorbis_block_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_block_destroy( IntPtr vb );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_block_init( IntPtr/*vorbis_dsp_state**/ v, IntPtr/*vorbis_block**/ vb );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_block_clear( IntPtr/*vorbis_block* */vb );

		///////////////////////////////////////////

		public class Block : IDisposable
		{
			internal IntPtr native;

			public Block( DspState dspState )
			{
				LoadNativeLibrary();

				native = vorbis_block_create();
				vorbis_block_init( dspState.native, native );
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					vorbis_block_clear( native );
					vorbis_block_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public int synthesis( ogg.Packet packet ) { return vorbis_synthesis( native, packet.native ); }
		}

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr vorbis_comment_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int vorbis_comment_destroy( IntPtr vi );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void vorbis_comment_init( IntPtr vi );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void vorbis_comment_clear( IntPtr vi );

		///////////////////////////////////////////

		public class Comment : IDisposable
		{
			internal IntPtr native;

			public Comment()
			{
				LoadNativeLibrary();

				native = vorbis_comment_create();
				vorbis_comment_init( native );
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					vorbis_comment_clear( native );
					vorbis_comment_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}
		}

		///////////////////////////////////////////

	}
}
