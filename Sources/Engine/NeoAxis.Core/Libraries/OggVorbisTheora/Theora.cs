// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Reflection;

namespace NeoAxis.OggVorbisTheora
{
	/*public */static class theora
	{
		const string library = "NeoAxisCoreNative";
		const CallingConvention convention = CallingConvention.Cdecl;

		///////////////////////////////////////////

		static bool nativeLibraryLoaded;

		static void LoadNativeLibrary()
		{
			if( !nativeLibraryLoaded )
			{
				NativeUtility.PreloadLibrary( library );
				nativeLibraryLoaded = true;
			}
		}

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr theora_yuv_buffer_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int theora_yuv_buffer_destroy( IntPtr b );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int theora_yuv_buffer_get_data( IntPtr b,
			out int y_width, out int y_height, out int y_stride,
			out int uv_width, out int uv_height, out int uv_stride,
			out IntPtr y, out IntPtr u, out IntPtr v
			);

		///////////////////////////////////////////

		public class YUVBuffer : IDisposable
		{
			internal IntPtr native;

			public YUVBuffer()
			{
				LoadNativeLibrary();

				native = theora_yuv_buffer_create();
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					theora_yuv_buffer_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public int get_data( out int y_width, out int y_height, out int y_stride, out int uv_width,
				out int uv_height, out int uv_stride, out IntPtr y, out IntPtr u, out IntPtr v )
			{
				return theora_yuv_buffer_get_data( native, out y_width, out y_height, out y_stride,
					out uv_width, out uv_height, out uv_stride, out y, out u, out v );
			}
		}

		/////////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr theora_info_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int theora_info_destroy( IntPtr c );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern uint theora_info_get_width( IntPtr c );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern uint theora_info_get_height( IntPtr c );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern uint theora_info_get_fps_numerator( IntPtr c );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern uint theora_info_get_fps_denominator( IntPtr c );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void theora_info_init( IntPtr c );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void theora_info_clear( IntPtr c );

		///////////////////////////////////////////
		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int theora_decode_header( IntPtr/*theora_info* */ci,
			IntPtr/*theora_comment* */cc, IntPtr/*ogg_packet* */op );

		///////////////////////////////////////////

		public class Info : IDisposable
		{
			internal IntPtr native;

			public Info()
			{
				LoadNativeLibrary();

				native = theora_info_create();
				theora_info_init( native );
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					theora_info_clear( native );
					theora_info_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public int decode_header( Comment comment, ogg.Packet packet )
			{
				return theora_decode_header( native, comment.native, packet.native );
			}

			public uint width
			{
				get
				{
					return theora_info_get_width( native );
				}
			}

			public uint height
			{
				get
				{
					return theora_info_get_height( native );
				}
			}

			public uint fps_numerator
			{
				get
				{
					return theora_info_get_fps_numerator( native );
				}
			}

			public uint fps_denominator
			{
				get
				{
					return theora_info_get_fps_denominator( native );
				}
			}

		}

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr theora_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int theora_destroy( IntPtr th );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern long theora_get_granulepos( IntPtr th );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int theora_decode_init( IntPtr/*theora_state* */th, IntPtr/*theora_info* */c );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int theora_decode_packetin( IntPtr/*theora_state* */th, IntPtr/*ogg_packet* */op );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern double theora_granule_time( IntPtr/*theora_state* */th, long granulepos );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int theora_decode_YUVout( IntPtr/*theora_state* */th, IntPtr/*yuv_buffer* */yuv );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void theora_clear( IntPtr/*theora_state* */th );

		///////////////////////////////////////////

		public class State : IDisposable
		{
			IntPtr native;

			public State()
			{
				LoadNativeLibrary();

				native = theora_create();
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					theora_clear( native );
					theora_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}

			public int decode_init( Info info ) { return theora_decode_init( native, info.native ); }
			public int decode_packetin( ogg.Packet packet ) { return theora_decode_packetin( native, packet.native ); }
			public double granule_time( long granulepos ) { return theora_granule_time( native, granulepos ); }
			public int decode_YUVout( YUVBuffer yuv ) { return theora_decode_YUVout( native, yuv.native ); }

			public long granulepos
			{
				get 
				{
					return theora_get_granulepos( native );
				}
			}
		}

		///////////////////////////////////////////

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern IntPtr theora_comment_create();

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern int theora_comment_destroy( IntPtr tc );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void theora_comment_init( IntPtr tc );

		[DllImport( library, CallingConvention = convention ), SuppressUnmanagedCodeSecurity]
		static extern void theora_comment_clear( IntPtr tc );

		///////////////////////////////////////////

		public class Comment : IDisposable
		{
			internal IntPtr native;

			public Comment()
			{
				LoadNativeLibrary();

				native = theora_comment_create();
				theora_comment_init( native );
			}

			public void Dispose()
			{
				if( native != IntPtr.Zero )
				{
					theora_comment_clear( native );
					theora_comment_destroy( native );
					native = IntPtr.Zero;
				}
				GC.SuppressFinalize( this );
			}
		}

		///////////////////////////////////////////

	}
}
