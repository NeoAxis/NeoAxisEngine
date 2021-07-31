// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "YUVToRGBConverter.h"

using namespace Ogre;

YUVToRGBConverter* YUVToRGBConverter::instance = NULL;

void YUVToRGBConverter::Init()
{
	instance = new YUVToRGBConverter();
	instance->InitInternal();
}

void YUVToRGBConverter::Shutdown()
{
	if(instance)
	{
		instance->ShutdownInternal();

		delete instance;
		instance = NULL;
	}
}

void YUVToRGBConverter::InitInternal()
{
	//calculate lookup table
	{
		//used to bring the table into the high side (scale up) so we
		//can maintain high precision and not use floats (FIXED POINT)
		int scale = 1 << 13;
		for( int i = 0; i < 256; i++ )
		{
			int temp = i - 128;

			//Calc Y component
			YTable[ i ] = (int)( ( 1.164 * scale + 0.5 ) * ( i - 16 ) );

			//Calc R component
			RVTable[ i ] = (int)( ( 1.596 * scale + 0.5 ) * temp );

			//Calc G u & v components
			GUTable[ i ] = (int)( ( 0.391 * scale + 0.5 ) * temp );
			GVTable[ i ] = (int)( ( 0.813 * scale + 0.5 ) * temp );

			//Calc B component
			BUTable[ i ] = (int)( ( 2.018 * scale + 0.5 ) * temp );
		}
	}
}

void YUVToRGBConverter::ShutdownInternal()
{
}

FORCEINLINE uint8 ClipToRGBColor( int x )
{
	return (uint8)( x > 255 ? 255 : ( x < 0 ? 0 : x ));
}

void YUVToRGBConverter::Convert(int yWidth, int yHeight, int yStride, int uvWidth, int uvHeight, 
	int uvStride, uint8* ySrc, uint8* uSrc, uint8* vSrc, int destBufferSizeX, uint8* destBuffer,
	bool isABGR) const
{
	const int bytesPerPixel = 4;
	int y_width = yWidth;
	int y_height = yHeight;
	int y_stride = yStride;
	int uv_width = uvWidth;
	int uv_height = uvHeight;
	int uv_stride = uvStride;
	uint8* bitmap = destBuffer;

	//

	uint8* ySrc2 = ( ySrc + y_stride );

	int dstBitmap = 0;
	int dstBitmapOffset = bytesPerPixel * destBufferSizeX;

	//Calculate buffer offsets
	int dstOff = destBufferSizeX/*lockRowPitch*/ * bytesPerPixel;
	int yOff = ( y_stride * 2 ) - y_width;

	//Check if upside down, if so, reverse buffers and offsets
	if( y_height < 0 )
	{
		y_height = -y_height;
		ySrc += ( y_height - 1 ) * y_stride;

		uSrc += ( ( y_height / 2 ) - 1 ) * uv_stride;
		vSrc += ( ( y_height / 2 ) - 1 ) * uv_stride;

		ySrc2 = ySrc - y_stride;
		yOff = -y_width - ( y_stride * 2 );

		uv_stride = -uv_stride;
	}

	//Cut width and height in half (uv field is only half y field)
	y_height = y_height >> 1;
	y_width = y_width >> 1;

	//Convientient temp vars
	int r, g, b, u, v, bU, gUV, rV, rgbY;

	if(isABGR)
	{
		//Loop does four blocks per iteration (2 rows, 2 pixels at a time)
		for( int y = y_height; y > 0; --y )
		{
			for( int x = 0; x < y_width; ++x )
			{
				//Get uv pointers for row
				u = uSrc[ x ];
				v = vSrc[ x ];

				//get corresponding lookup values
				rgbY = YTable[ *ySrc ];
				rV = RVTable[ v ];
				gUV = GUTable[ u ] + GVTable[ v ];
				bU = BUTable[ u ];
				ySrc++;

				//scale down - brings are values back into the 8 bits of a byte
				b = ( rgbY + rV ) >> 13;
				g = ( rgbY - gUV ) >> 13;
				r = ( rgbY + bU ) >> 13;

				//Clip to RGB values (255 0)
				bitmap[ dstBitmap + 2 ] = ClipToRGBColor( r );
				bitmap[ dstBitmap + 1 ] = ClipToRGBColor( g );
				bitmap[ dstBitmap + 0 ] = ClipToRGBColor( b );

				//And repeat for other pixels (note, y is unique for each
				//pixel, while uv are not)
				rgbY = YTable[ *ySrc ];
				b = ( rgbY + rV ) >> 13;
				g = ( rgbY - gUV ) >> 13;
				r = ( rgbY + bU ) >> 13;
				bitmap[ dstBitmap + bytesPerPixel + 2 ] = ClipToRGBColor( r );
				bitmap[ dstBitmap + bytesPerPixel + 1 ] = ClipToRGBColor( g );
				bitmap[ dstBitmap + bytesPerPixel + 0 ] = ClipToRGBColor( b );
				ySrc++;

				rgbY = YTable[ *ySrc2 ];
				b = ( rgbY + rV ) >> 13;
				g = ( rgbY - gUV ) >> 13;
				r = ( rgbY + bU ) >> 13;
				bitmap[ dstBitmapOffset + 2 ] = ClipToRGBColor( r );
				bitmap[ dstBitmapOffset + 1 ] = ClipToRGBColor( g );
				bitmap[ dstBitmapOffset + 0 ] = ClipToRGBColor( b );
				ySrc2++;

				rgbY = YTable[ *ySrc2 ];
				b = ( rgbY + rV ) >> 13;
				g = ( rgbY - gUV ) >> 13;
				r = ( rgbY + bU ) >> 13;
				bitmap[ dstBitmapOffset + bytesPerPixel + 2 ] = ClipToRGBColor( r );
				bitmap[ dstBitmapOffset + bytesPerPixel + 1 ] = ClipToRGBColor( g );
				bitmap[ dstBitmapOffset + bytesPerPixel + 0 ] = ClipToRGBColor( b );
				ySrc2++;

				//Advance inner loop offsets
				dstBitmap += bytesPerPixel << 1;
				dstBitmapOffset += bytesPerPixel << 1;
			} // end for x

			//Advance destination pointers by offsets
			dstBitmap += dstOff;
			dstBitmapOffset += dstOff;
			ySrc += yOff;
			ySrc2 += yOff;
			uSrc += uv_stride;
			vSrc += uv_stride;
		} //end for y
	}
	else
	{
		//Loop does four blocks per iteration (2 rows, 2 pixels at a time)
		for( int y = y_height; y > 0; --y )
		{
			for( int x = 0; x < y_width; ++x )
			{
				//Get uv pointers for row
				u = uSrc[ x ];
				v = vSrc[ x ];

				//get corresponding lookup values
				rgbY = YTable[ *ySrc ];
				rV = RVTable[ v ];
				gUV = GUTable[ u ] + GVTable[ v ];
				bU = BUTable[ u ];
				ySrc++;

				//scale down - brings are values back into the 8 bits of a byte
				r = ( rgbY + rV ) >> 13;
				g = ( rgbY - gUV ) >> 13;
				b = ( rgbY + bU ) >> 13;

				//Clip to RGB values (255 0)
				bitmap[ dstBitmap + 2 ] = ClipToRGBColor( r );
				bitmap[ dstBitmap + 1 ] = ClipToRGBColor( g );
				bitmap[ dstBitmap + 0 ] = ClipToRGBColor( b );

				//And repeat for other pixels (note, y is unique for each
				//pixel, while uv are not)
				rgbY = YTable[ *ySrc ];
				r = ( rgbY + rV ) >> 13;
				g = ( rgbY - gUV ) >> 13;
				b = ( rgbY + bU ) >> 13;
				bitmap[ dstBitmap + bytesPerPixel + 2 ] = ClipToRGBColor( r );
				bitmap[ dstBitmap + bytesPerPixel + 1 ] = ClipToRGBColor( g );
				bitmap[ dstBitmap + bytesPerPixel + 0 ] = ClipToRGBColor( b );
				ySrc++;

				rgbY = YTable[ *ySrc2 ];
				r = ( rgbY + rV ) >> 13;
				g = ( rgbY - gUV ) >> 13;
				b = ( rgbY + bU ) >> 13;
				bitmap[ dstBitmapOffset + 2 ] = ClipToRGBColor( r );
				bitmap[ dstBitmapOffset + 1 ] = ClipToRGBColor( g );
				bitmap[ dstBitmapOffset + 0 ] = ClipToRGBColor( b );
				ySrc2++;

				rgbY = YTable[ *ySrc2 ];
				r = ( rgbY + rV ) >> 13;
				g = ( rgbY - gUV ) >> 13;
				b = ( rgbY + bU ) >> 13;
				bitmap[ dstBitmapOffset + bytesPerPixel + 2 ] = ClipToRGBColor( r );
				bitmap[ dstBitmapOffset + bytesPerPixel + 1 ] = ClipToRGBColor( g );
				bitmap[ dstBitmapOffset + bytesPerPixel + 0 ] = ClipToRGBColor( b );
				ySrc2++;

				//Advance inner loop offsets
				dstBitmap += bytesPerPixel << 1;
				dstBitmapOffset += bytesPerPixel << 1;
			} // end for x

			//Advance destination pointers by offsets
			dstBitmap += dstOff;
			dstBitmapOffset += dstOff;
			ySrc += yOff;
			ySrc2 += yOff;
			uSrc += uv_stride;
			vSrc += uv_stride;
		} //end for y
	}
}
