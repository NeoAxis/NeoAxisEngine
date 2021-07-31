// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Reflection;

namespace NeoAxis.OggVorbisTheora
{
	public static class ogg
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
		static extern IntPtr ogg_page_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_page_destroy( IntPtr og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void ogg_page_checksum_set( IntPtr og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_page_version( IntPtr og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_page_continued( IntPtr og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_page_bos( IntPtr og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_page_eos( IntPtr og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern long ogg_page_granulepos( IntPtr og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_page_serialno( IntPtr og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_page_pageno( IntPtr og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_page_packets( IntPtr og );

		///////////////////////////////////////////

		public class Page : IDisposable
		{
			internal IntPtr native;

			public Page()
			{
				LoadNativeLibrary();

				native = ogg_page_create();
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					ogg_page_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public void checksum_set() { ogg_page_checksum_set( native ); }
			public int version() { return ogg_page_version( native ); }
			public int continued() { return ogg_page_continued( native ); }
			public int bos() { return ogg_page_bos( native ); }
			public int eos() { return ogg_page_eos( native ); }
			public long granulepos() { return ogg_page_granulepos( native ); }
			public int serialno() { return ogg_page_serialno( native ); }
			public int pageno() { return ogg_page_pageno( native ); }
			public int packets() { return ogg_page_packets( native ); }
		}

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr ogg_stream_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_stream_init( IntPtr os, int serialno );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_stream_clear( IntPtr os );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_stream_reset( IntPtr os );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_stream_reset_serialno( IntPtr os, int serialno );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_stream_destroy( IntPtr os );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_stream_eos( IntPtr os );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_stream_pagein( IntPtr/*ogg_stream_state* */os, IntPtr/*ogg_page* */og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_stream_packetout( IntPtr/*ogg_stream_state* */os, IntPtr/*ogg_packet* */op );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_stream_packetpeek( IntPtr/*ogg_stream_state* */os, IntPtr/*ogg_packet* */op );

		///////////////////////////////////////////

		public class StreamState : IDisposable
		{
			IntPtr native;

			public StreamState( int serialno )
			{
				LoadNativeLibrary();

				native = ogg_stream_create();
				ogg_stream_init( native, serialno );
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					ogg_stream_clear( native );
					ogg_stream_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public int eos() { return ogg_stream_eos( native ); }

			public int pagein( Page page ) { return ogg_stream_pagein( native, page.native ); }
			public int packetout( Packet packet ) { return ogg_stream_packetout( native, packet.native ); }
			public int packetpeek( Packet packet ) { return ogg_stream_packetpeek( native, packet.native ); }
		}

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr ogg_packet_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void ogg_packet_destroy( IntPtr op );

		//[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		//static extern int ogg_packet_clear( IntPtr op );

		///////////////////////////////////////////

		public class Packet : IDisposable
		{
			internal IntPtr native;

			public Packet()
			{
				LoadNativeLibrary();

				native = ogg_packet_create();
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					ogg_packet_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}
		}

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr ogg_sync_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_sync_init( IntPtr oy );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_sync_clear( IntPtr oy );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_sync_reset( IntPtr oy );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_sync_destroy( IntPtr oy );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr/*char* */ ogg_sync_buffer( IntPtr oy, int size );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_sync_wrote( IntPtr oy, int bytes );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_sync_pageseek( IntPtr oy, IntPtr /*ogg_page**/ og );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int ogg_sync_pageout( IntPtr oy, IntPtr /*ogg_page**/ og );

		///////////////////////////////////////////

		public class SyncState : IDisposable
		{
			IntPtr native;

			public SyncState()
			{
				LoadNativeLibrary();

				native = ogg_sync_create();
				ogg_sync_init( native );
				ogg_sync_reset( native );
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					ogg_sync_clear( native );
					ogg_sync_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public IntPtr buffer( int size ) { return ogg_sync_buffer( native, size ); }
			public int wrote( int bytes ) { return ogg_sync_wrote( native, bytes ); }
			public int pageseek( Page og ) { return ogg_sync_pageseek( native, og.native ); }
			public int pageout( Page og ) { return ogg_sync_pageout( native, og.native ); }
		}

		///////////////////////////////////////////

	}
}
