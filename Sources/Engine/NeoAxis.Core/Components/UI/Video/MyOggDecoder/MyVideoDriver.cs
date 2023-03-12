// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using NeoAxis;
using OggDecoder;

namespace MyOggDecoder
{
	class VideoBuffer
	{
		Vector2I size;
		IntPtr buffer;

		bool needUpdateTexture;
		NeoAxis.ImageComponent texture;
		PixelFormat textureFormat;

		//

		public PixelFormat TextureFormat
		{
			get { return textureFormat; }
		}

		public IntPtr GetBufferForWriting( Vector2I size )
		{
			unsafe
			{
				if( this.size != size )
				{
					Clear();

					this.size = size;

					if( size != Vector2I.Zero )
					{
						//create buffer
						buffer = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.SoundAndVideo, size.X * size.Y * 4 );
						byte* pBuffer = (byte*)buffer;
						int z = 0;
						for( int n = 0; n < size.X * size.Y; n++ )
						{
							pBuffer[ z + 0 ] = 0;
							pBuffer[ z + 1 ] = 0;
							pBuffer[ z + 2 ] = 0;
							pBuffer[ z + 3 ] = 255;
							z += 4;
						}
					}
				}

				needUpdateTexture = true;
				return buffer;
			}
		}

		public void Clear()
		{
			DestroyTexture();

			if( buffer != IntPtr.Zero )
			{
				NativeUtility.Free( buffer );
				buffer = IntPtr.Zero;
			}

			size = Vector2I.Zero;
		}

		int Round2( int value )
		{
			int v = 1;
			while( value > v )
				v *= 2;
			return v;
		}

		bool CreateTexture()
		{
			Vector2I textureSize = new Vector2I( Round2( size.X ), Round2( size.Y ) );

			texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
			texture.CreateType = ImageComponent.TypeEnum._2D;
			texture.CreateSize = textureSize;
			texture.CreateMipmaps = false;// 0;
										  //!!!!
			texture.CreateFormat = PixelFormat.A8R8G8B8;
			texture.CreateUsage = ImageComponent.Usages.Dynamic | ImageComponent.Usages.WriteOnly;
			texture.Enabled = true;

			//if( !c.Create( Texture.TypeEnum.Type2D, textureSize, 1, 0, PixelFormat.A8R8G8B8, GpuTexture.Usages.DynamicWriteOnly ) )
			//{
			//	texture.Dispose();
			//	return false;
			//}

			GpuTexture gpuTexture = texture.Result;

			int totalSize = PixelFormatUtility.GetNumElemBytes( gpuTexture.ResultFormat ) * gpuTexture.ResultSize.X * gpuTexture.ResultSize.Y;
			byte[] data = new byte[ totalSize ];
			var d = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( data ) };
			gpuTexture.SetData( d );

			textureFormat = texture.CreateFormat;

			//HardwarePixelBuffer pixelBuffer = texture.GetBuffer();
			//pixelBuffer.Lock( HardwareBuffer.LockOptions.Discard );
			//PixelBox pixelBox = pixelBuffer.GetCurrentLock();
			//textureFormat = pixelBox.Format;
			//NativeUtils.ZeroMemory( pixelBox.Data, pixelBox.SlicePitch * 4 );
			//pixelBuffer.Unlock();

			if( textureFormat != PixelFormat.A8R8G8B8 && textureFormat != PixelFormat.A8B8G8R8 )
				Log.Warning( "UIVideo: CreateTexture: Lock texture format != PixelFormat.A8R8G8B8 and != PixelFormat.A8B8G8R8 ({0}).", textureFormat );

			RenderingSystem.RenderSystemEvent += RenderSystem_RenderSystemEvent;

			return true;
		}

		public unsafe void UpdateTexture( bool clearWholeTexture )
		{
			if( size == Vector2I.Zero )
				return;
			if( RenderingSystem.IsDeviceLost() )
				return;

			if( texture != null && texture.Disposed )
				texture = null;

			//create texture
			if( texture == null )
			{
				if( !CreateTexture() )
					return;
			}

			if( textureFormat != PixelFormat.A8R8G8B8 && textureFormat != PixelFormat.A8B8G8R8 )
				return;

			GpuTexture gpuTexture = texture.Result;

			int totalSize = PixelFormatUtility.GetNumElemBytes( gpuTexture.ResultFormat ) * gpuTexture.ResultSize.X * gpuTexture.ResultSize.Y;

			//!!!!можно каждый раз не создавать массив
			byte[] data = new byte[ totalSize ];

			//HardwarePixelBuffer pixelBuffer = texture.GetBuffer();
			//pixelBuffer.Lock( HardwareBuffer.LockOptions.Discard );
			//PixelBox pixelBox = pixelBuffer.GetCurrentLock();

			//if( clearWholeTexture )
			//	NativeUtility.ZeroMemory( data, pixelBox.SlicePitch * 4 );

			byte* pointer = (byte*)buffer;
			for( int y = 0; y < size.Y; y++ )
			{
				//!!!!всегда так?
				int rowPitch = PixelFormatUtility.GetNumElemBytes( gpuTexture.ResultFormat ) * gpuTexture.ResultSize.X;

				Marshal.Copy( (IntPtr)pointer, data, y * rowPitch, size.X * 4 );
				//NativeUtility.CopyMemory( pointer, source, length );
				//pixelBox.WriteDataUnmanaged( y * pixelBox.RowPitch * 4, pointer, size.X * 4 );

				pointer += size.X * 4;
			}

			//pixelBuffer.Unlock();

			var d = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( data ) };
			gpuTexture.SetData( d );

			needUpdateTexture = false;
		}

		public NeoAxis.ImageComponent GetUpdatedTexture()
		{
			if( buffer == IntPtr.Zero )
				return null;

			if( texture != null && texture.Disposed )
				texture = null;

			if( needUpdateTexture )
				UpdateTexture( false );
			return texture;
		}

		void DestroyTexture()
		{
			if( texture != null )
			{
				RenderingSystem.RenderSystemEvent -= RenderSystem_RenderSystemEvent;

				texture.Dispose();
				texture = null;
			}
		}

		void RenderSystem_RenderSystemEvent( RenderSystemEvent name )
		{
			if( name == RenderSystemEvent.DeviceRestored )
				UpdateTexture( true );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	struct OgreWrapper
	{
		public const string library = "NeoAxisCoreNative";
		public const CallingConvention convention = CallingConvention.Cdecl;
	}

	struct YUVToRGBConverter
	{
		[DllImport( OgreWrapper.library, EntryPoint = "YUVToRGBConverter_Convert", CallingConvention = OgreWrapper.convention )]
		public static extern void Convert( int yWidth, int yHeight, int yStride,
			int uvWidth, int uvHeight, int uvStride, IntPtr ySrc, IntPtr uSrc, IntPtr vSrc,
			int destBufferSizeX, IntPtr destBuffer, [MarshalAs( UnmanagedType.U1 )] bool isABGR );
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////

	static class _RendererAddition
	{
		public static void YUVToRGBConverter_Convert( int yWidth, int yHeight, int yStride,
			int uvWidth, int uvHeight, int uvStride, IntPtr ySrc, IntPtr uSrc, IntPtr vSrc,
				int destBufferSizeX, IntPtr destBuffer, bool isABGR )
		{
			YUVToRGBConverter.Convert( yWidth, yHeight, yStride, uvWidth, uvHeight, uvStride, ySrc,
				uSrc, vSrc, destBufferSizeX, destBuffer, isABGR );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	class MyVideoDriver : VideoDriver
	{
		VideoBuffer videoBuffer;

		//

		public MyVideoDriver( VideoBuffer videoBuffer )
		{
			this.videoBuffer = videoBuffer;
		}

		unsafe protected override sealed void OnBlit()
		{
			IntPtr buffer = videoBuffer.GetBufferForWriting( GetSize() );

			if( buffer != IntPtr.Zero )
			{
				int y_width;
				int y_height;
				int y_stride;
				int uv_width;
				int uv_height;
				int uv_stride;
				IntPtr y;
				IntPtr u;
				IntPtr v;

				YUVBuffer.get_data( out y_width, out y_height, out y_stride, out uv_width, out uv_height,
					out uv_stride, out y, out u, out v );

				//Convert 4:2:0 YUV YCrCb to an RGB Bitmap
				_RendererAddition.YUVToRGBConverter_Convert( y_width, y_height, y_stride, uv_width, uv_height,
					uv_stride, y, u, v, GetSize().X, buffer, videoBuffer.TextureFormat == PixelFormat.A8B8G8R8 );
			}
		}
	}
}
