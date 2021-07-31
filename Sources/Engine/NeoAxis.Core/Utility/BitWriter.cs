/* Copyright (c) 2008 Michael Lidgren

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom
the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoAxis
{
	static class BitWriter
	{
		/// <summary>
		/// Read 1-8 bits from a buffer into a byte
		/// </summary>
		public static byte ReadByte( byte[] fromBuffer, int numberOfBits, int readBitOffset )
		{
			int bytePtr = readBitOffset >> 3;
			int startReadAtIndex = readBitOffset - ( bytePtr * 8 ); // (readBitOffset % 8);

			if( startReadAtIndex == 0 && numberOfBits == 8 )
				return fromBuffer[ bytePtr ];

			// mask away unused bits lower than (right of) relevant bits in first byte
			byte returnValue = (byte)( fromBuffer[ bytePtr ] >> startReadAtIndex );

			int numberOfBitsInSecondByte = numberOfBits - ( 8 - startReadAtIndex );

			if( numberOfBitsInSecondByte < 1 )
			{
				// we don't need to read from the second byte, but we DO need
				// to mask away unused bits higher than (left of) relevant bits
				return (byte)( returnValue & ( 255 >> ( 8 - numberOfBits ) ) );
			}

			byte second = fromBuffer[ bytePtr + 1 ];

			// mask away unused bits higher than (left of) relevant bits in second byte
			second &= (byte)( 255 >> ( 8 - numberOfBitsInSecondByte ) );

			return (byte)( returnValue | (byte)( second << ( numberOfBits -
				numberOfBitsInSecondByte ) ) );
		}

		/// <summary>
		/// Read several bytes from a buffer
		/// </summary>
		public static void ReadBytes( byte[] fromBuffer, int numberOfBytes, int readBitOffset,
			byte[] destination, int destinationByteOffset )
		{
			int readPtr = readBitOffset >> 3;
			int startReadAtIndex = readBitOffset - ( readPtr * 8 ); // (readBitOffset % 8);

			if( startReadAtIndex == 0 )
			{
				for( int i = 0; i < numberOfBytes; i++ )
					destination[ destinationByteOffset++ ] = fromBuffer[ readPtr++ ];
				return;
			}

			int secondPartLen = 8 - startReadAtIndex;
			int secondMask = 255 >> secondPartLen;

			for( int i = 0; i < numberOfBytes; i++ )
			{
				// mask away unused bits lower than (right of) relevant bits in byte
				int b = fromBuffer[ readPtr ] >> startReadAtIndex;

				readPtr++;

				// mask away unused bits higher than (left of) relevant bits in second byte
				int second = fromBuffer[ readPtr ] & secondMask;

				destination[ destinationByteOffset++ ] = (byte)( b | ( second << secondPartLen ) );
			}
		}

		/// <summary>
		/// Write a byte consisting of 1-8 bits to a buffer; assumes buffer is previously allocated
		/// </summary>
		public static void WriteByte( byte source, int numberOfBits, byte[] destination,
			int destBitOffset )
		{
			// mask out unwanted bits in the source
			uint isrc = (uint)source & ( ( ~(uint)0 ) >> ( 8 - numberOfBits ) );

			int bytePtr = destBitOffset >> 3;

			int localBitLen = ( destBitOffset % 8 );
			if( localBitLen == 0 )
			{
				destination[ bytePtr ] = (byte)isrc;
				return;
			}

			destination[ bytePtr ] = (byte)(
				( destination[ bytePtr ] & ( 255 >> ( 8 - localBitLen ) ) ) |
				(byte)( ( isrc << localBitLen ) )
			);

			// need write into next byte?
			if( localBitLen + numberOfBits > 8 )
			{
				destination[ bytePtr + 1 ] = (byte)(
					( destination[ bytePtr + 1 ] & ( 255 << localBitLen ) ) |
					( (byte)( isrc >> ( 8 - localBitLen ) ) )
				);
			}
		}

		/// <summary>
		/// Write several whole bytes
		/// </summary>
		public static void WriteBytes( byte[] source, int sourceByteOffset, int numberOfBytes,
			byte[] destination, int destBitOffset )
		{
			int dstBytePtr = destBitOffset >> 3;
			int firstPartLen = ( destBitOffset % 8 );

			if( firstPartLen == 0 )
			{
				// optimized; TODO: write 32 bit chunks if possible
				for( int i = 0; i < numberOfBytes; i++ )
					destination[ dstBytePtr++ ] = source[ sourceByteOffset + i ];
				return;
			}

			int lastPartLen = 8 - firstPartLen;

			for( int i = 0; i < numberOfBytes; i++ )
			{
				byte src = source[ sourceByteOffset + i ];

				// write last part of this byte
				destination[ dstBytePtr ] &= (byte)( 255 >> lastPartLen ); // clear before writing
				destination[ dstBytePtr ] |= (byte)( src << firstPartLen ); // write first half

				dstBytePtr++;

				// write first part of next byte
				destination[ dstBytePtr ] &= (byte)( 255 << firstPartLen ); // clear before writing
				destination[ dstBytePtr ] |= (byte)( src >> lastPartLen ); // write second half
			}

			return;
		}

		//[CLSCompliant( false )]
		public static uint ReadUInt32( byte[] fromBuffer, int numberOfBits, int readBitOffset )
		{
			unsafe
			{
				if( numberOfBits == 32 && ( ( readBitOffset % 8 ) == 0 ) )
				{
					fixed( byte* ptr = &( fromBuffer[ readBitOffset / 8 ] ) )
					{
						return *( ( (uint*)ptr ) );
					}
				}
			}

			uint returnValue;
			if( numberOfBits <= 8 )
			{
				returnValue = ReadByte( fromBuffer, numberOfBits, readBitOffset );
				return returnValue;
			}
			returnValue = ReadByte( fromBuffer, 8, readBitOffset );
			numberOfBits -= 8;
			readBitOffset += 8;

			if( numberOfBits <= 8 )
			{
				returnValue |= (uint)( ReadByte( fromBuffer, numberOfBits, readBitOffset ) << 8 );
				return returnValue;
			}
			returnValue |= (uint)( ReadByte( fromBuffer, 8, readBitOffset ) << 8 );
			numberOfBits -= 8;
			readBitOffset += 8;

			if( numberOfBits <= 8 )
			{
				uint r = ReadByte( fromBuffer, numberOfBits, readBitOffset );
				r <<= 16;
				returnValue |= r;
				return returnValue;
			}
			returnValue |= (uint)( ReadByte( fromBuffer, 8, readBitOffset ) << 16 );
			numberOfBits -= 8;
			readBitOffset += 8;

			returnValue |= (uint)( ReadByte( fromBuffer, numberOfBits, readBitOffset ) << 24 );

			//#if BIGENDIAN
			//         // reorder bytes
			//         return
			//            ((a & 0xff000000) >> 24) |
			//            ((a & 0x00ff0000) >> 8) |
			//            ((a & 0x0000ff00) << 8) |
			//            ((a & 0x000000ff) << 24);
			//#endif

			return returnValue;
		}

		//[CLSCompliant( false )]
		public static ulong ReadUInt64( byte[] fromBuffer, int numberOfBits, int readBitOffset )
		{
			throw new NotImplementedException();

			//#if BIGENDIAN
			//         // reorder bytes
			//         return ((a & 0xff00000000000000L) >> 56) |
			//            ((a & 0x00ff000000000000L) >> 40) |
			//            ((a & 0x0000ff0000000000L) >> 24) |
			//            ((a & 0x000000ff00000000L) >> 8) |
			//            ((a & 0x00000000ff000000L) << 8) |
			//            ((a & 0x0000000000ff0000L) << 24) |
			//            ((a & 0x000000000000ff00L) << 40) |
			//            ((a & 0x00000000000000ffL) << 56);
			//#endif
		}

		//[CLSCompliant( false )]
		public static int WriteUInt32( uint source, int numberOfBits, byte[] destination,
			int destinationBitOffset )
		{
			//#if BIGENDIAN
			//         // reorder bytes
			//         source = ((source & 0xff000000) >> 24) |
			//            ((source & 0x00ff0000) >> 8) |
			//            ((source & 0x0000ff00) << 8) |
			//            ((source & 0x000000ff) << 24);
			//#endif

			int returnValue = destinationBitOffset + numberOfBits;
			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)source, numberOfBits, destination, destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)source, 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)( source >> 8 ), numberOfBits, destination,
					destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)( source >> 8 ), 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)( source >> 16 ), numberOfBits, destination,
					destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)( source >> 16 ), 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			WriteByte( (byte)( source >> 24 ), numberOfBits, destination,
				destinationBitOffset );
			return returnValue;
		}

		//[CLSCompliant( false )]
		public static int WriteUInt64( ulong source, int numberOfBits, byte[] destination,
			int destinationBitOffset )
		{
			//#if BIGENDIAN
			//         source = ((source & 0xff00000000000000L) >> 56) |
			//            ((source & 0x00ff000000000000L) >> 40) |
			//            ((source & 0x0000ff0000000000L) >> 24) |
			//            ((source & 0x000000ff00000000L) >> 8) |
			//            ((source & 0x00000000ff000000L) << 8) |
			//            ((source & 0x0000000000ff0000L) << 24) |
			//            ((source & 0x000000000000ff00L) << 40) |
			//            ((source & 0x00000000000000ffL) << 56);
			//#endif

			int returnValue = destinationBitOffset + numberOfBits;
			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)source, numberOfBits, destination, destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)source, 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)( source >> 8 ), numberOfBits, destination, destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)( source >> 8 ), 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)( source >> 16 ), numberOfBits, destination, destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)( source >> 16 ), 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)( source >> 24 ), numberOfBits, destination, destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)( source >> 24 ), 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)( source >> 32 ), numberOfBits, destination, destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)( source >> 32 ), 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)( source >> 40 ), numberOfBits, destination, destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)( source >> 40 ), 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)( source >> 48 ), numberOfBits, destination, destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)( source >> 48 ), 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			if( numberOfBits <= 8 )
			{
				WriteByte( (byte)( source >> 56 ), numberOfBits, destination,
					destinationBitOffset );
				return returnValue;
			}
			WriteByte( (byte)( source >> 56 ), 8, destination, destinationBitOffset );
			destinationBitOffset += 8;
			numberOfBits -= 8;

			return returnValue;
		}

		//
		// Variable size
		//

		/// <summary>
		/// Write Base128 encoded variable sized unsigned integer
		/// </summary>
		/// <returns>number of bytes written</returns>
		//[CLSCompliant( false )]
		public static int WriteVariableUInt32( byte[] destination, int offset, uint value )
		{
			int retval = 0;
			uint num1 = (uint)value;
			while( num1 >= 0x80 )
			{
				destination[ offset + retval ] = (byte)( num1 | 0x80 );
				num1 = num1 >> 7;
				retval++;
			}
			destination[ offset + retval ] = (byte)num1;
			return retval + 1;
		}

		/// <summary>
		/// Reads a UInt32 written using WriteUnsignedVarInt(); will increment offset!
		/// </summary>
		//[CLSCompliant( false )]
		public static uint ReadVariableUInt32( byte[] buffer, ref int offset )
		{
			int num1 = 0;
			int num2 = 0;
			while( true )
			{
				if( num2 == 0x23 )
					throw new FormatException( "Bad 7-bit encoded integer" );

				byte num3 = buffer[ offset++ ];
				num1 |= ( num3 & 0x7f ) << ( num2 & 0x1f );
				num2 += 7;
				if( ( num3 & 0x80 ) == 0 )
					return (uint)num1;
			}
		}
	}
}
