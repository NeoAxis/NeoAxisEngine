// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// A class for streaming data to an array.
	/// </summary>
	public class ArrayDataWriter
	{
		byte[] data;// = new byte[ 32 ];
		int length;
		//int bitLength;

		//

		public ArrayDataWriter( int initialCapacity = 32 )
		{
			data = new byte[ initialCapacity ];
		}

		public void Reset()
		{
			length = 0;
			//bitLength = 0;
		}

		public byte[] Data
		{
			get { return data; }
		}

		public int Length
		{
			get { return length; }
		}

		//public int BitLength
		//{
		//	get { return bitLength; }
		//}

		//public int GetByteLength()
		//{
		//	return ( bitLength >> 3 ) + ( ( bitLength & 7 ) > 0 ? 1 : 0 );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void ExpandBuffer( int demandLength )
		{
			if( data.Length < demandLength )
			{
				int newLength = data.Length;
				while( newLength < demandLength )
					newLength *= 2;
				Array.Resize( ref data, newLength );
			}
		}

		//void ExpandBuffer( int demandBitLength )
		//{
		//	int demandByteLength = ( demandBitLength >> 3 ) + ( ( demandBitLength & 7 ) > 0 ? 1 : 0 );

		//	if( data.Length < demandByteLength )
		//	{
		//		int newLength = data.Length;
		//		while( newLength < demandByteLength )
		//			newLength *= 2;
		//		Array.Resize<byte>( ref data, newLength );
		//	}
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe void Write( void* source, int length )
		{
			int newLength = this.length + length;
			ExpandBuffer( newLength );

			fixed( byte* pData = data )
			{
				byte* p = pData + this.length;

				if( length == 8 )
					*(ulong*)p = *(ulong*)source;
				else if( length == 4 )
					*(uint*)p = *(uint*)source;
				else
					Buffer.MemoryCopy( source, p, length, length );
			}

			//Marshal.Copy( (IntPtr)source, data, byteLength, dataByteLength );

			this.length = newLength;
		}

		//public void Write( byte[] source, int byteOffset, int byteLength )
		//{
		//	int newLength = bitLength + byteLength * 8;
		//	ExpandBuffer( newLength );
		//	BitWriter.WriteBytes( source, byteOffset, byteLength, data, bitLength );
		//	bitLength = newLength;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( byte[] source )
		{
			int newLength = length + source.Length;
			ExpandBuffer( newLength );
			Array.Copy( source, 0, data, length, source.Length );
			length = newLength;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( byte[] source, int offset, int length )
		{
			int newLength = this.length + length;
			ExpandBuffer( newLength );
			Array.Copy( source, offset, data, this.length, length );
			this.length = newLength;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write<T>( T[] source ) where T : unmanaged
		{
			unsafe
			{
				fixed( T* pSource = source )
					Write( pSource, source.Length * sizeof( T ) );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( bool source )
		{
			Write( source ? (byte)1 : (byte)0 );

			//byte v = source ? (byte)1 : (byte)0;
			//unsafe
			//{
			//	Write( &v, 1 );
			//}

			//int newLength = bitLength + 1;
			//ExpandBuffer( newLength );
			//BitWriter.WriteByte( ( source ? (byte)1 : (byte)0 ), 1, data, bitLength );
			//bitLength = newLength;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( byte source )
		{
			int newLength = length + 1;
			ExpandBuffer( newLength );
			data[ length ] = source;
			length = newLength;

			//unsafe
			//{
			//	Write( &source, 1 );
			//}

			//int newLength = bitLength + 8;
			//ExpandBuffer( newLength );
			//BitWriter.WriteByte( source, 8, data, bitLength );
			//bitLength = newLength;
		}

		//public void Write( byte source, int numberOfBits )
		//{
		//	int newLength = bitLength + numberOfBits;
		//	ExpandBuffer( newLength );
		//	BitWriter.WriteByte( source, numberOfBits, data, bitLength );
		//	bitLength = newLength;
		//}

		//public void Write( sbyte source )
		//{
		//   unchecked
		//   {
		//      Write( (byte)source );
		//   }
		//}

		//public void Write( sbyte source, int numberOfBits )

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ushort source )
		{
			unsafe
			{
				Write( &source, 2 );
			}
			//int newLength = bitLength + 16;
			//ExpandBuffer( newLength );
			//BitWriter.WriteUInt32( (uint)source, 16, data, bitLength );
			//bitLength = newLength;
		}

		//public void Write( ushort source, int numberOfBits )
		//{
		//	int newLength = bitLength + numberOfBits;
		//	ExpandBuffer( newLength );
		//	BitWriter.WriteUInt32( (uint)source, numberOfBits, data, bitLength );
		//	bitLength = newLength;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( short source )
		{
			unsafe
			{
				Write( &source, 2 );
			}
			//unchecked
			//{
			//	Write( (ushort)source );
			//}
		}

		//public void Write( short source, int numberOfBits )

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( int source )
		{
			unsafe
			{
				Write( &source, 4 );
			}

			//int newLength = bitLength + 32;
			//ExpandBuffer( newLength );

			//if( bitLength % 8 == 0 )
			//{
			//	unsafe
			//	{
			//		fixed( byte* numRef = &data[ bitLength / 8 ] )
			//		{
			//			*( (int*)numRef ) = source;
			//		}
			//	}
			//}
			//else
			//	BitWriter.WriteUInt32( (uint)source, 32, data, bitLength );

			//bitLength = newLength;
		}

		//public void Write( int source, int numberOfBits )
		//{
		//	int newLength = bitLength + numberOfBits;
		//	ExpandBuffer( newLength );

		//	if( numberOfBits != 32 )
		//	{
		//		//make first bit sign
		//		int signBit = 1 << ( numberOfBits - 1 );
		//		if( source < 0 )
		//			source = ( -source - 1 ) | signBit;
		//		else
		//			source &= ( ~signBit );
		//	}

		//	BitWriter.WriteUInt32( (uint)source, numberOfBits, data, bitLength );

		//	bitLength = newLength;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( uint source )
		{
			unsafe
			{
				Write( &source, 4 );
			}

			//int newLength = bitLength + 32;
			//ExpandBuffer( newLength );

			//if( bitLength % 8 == 0 )
			//{
			//	unsafe
			//	{
			//		fixed( byte* numRef = &data[ bitLength / 8 ] )
			//		{
			//			*( (uint*)numRef ) = source;
			//		}
			//	}
			//}
			//else
			//{
			//	BitWriter.WriteUInt32( source, 32, data, bitLength );
			//}

			//bitLength = newLength;
		}

		//public void Write( uint source, int numberOfBits )
		//{
		//	int newLength = bitLength + numberOfBits;
		//	ExpandBuffer( newLength );
		//	BitWriter.WriteUInt32( source, numberOfBits, data, bitLength );
		//	bitLength = newLength;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( long source )
		{
			unsafe
			{
				Write( &source, 8 );
			}

			//int newLength = bitLength + 64;
			//ExpandBuffer( newLength );
			////!!!!good convertion?
			//ulong usource = (ulong)source;
			//BitWriter.WriteUInt64( usource, 64, data, bitLength );
			//bitLength = newLength;
		}

		//public void Write( long source, int numberOfBits )
		//{
		//	int newLength = bitLength + numberOfBits;
		//	ExpandBuffer( newLength );
		//	ulong usource = (ulong)source;
		//	BitWriter.WriteUInt64( usource, numberOfBits, data, bitLength );
		//	bitLength = newLength;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ulong source )
		{
			unsafe
			{
				Write( &source, 8 );
			}

			//int newLength = bitLength + 64;
			//ExpandBuffer( newLength );
			//BitWriter.WriteUInt64( source, 64, data, bitLength );
			//bitLength = newLength;
		}

		//public void Write( ulong source, int numberOfBits )
		//{
		//	int newLength = bitLength + numberOfBits;
		//	ExpandBuffer( newLength );
		//	BitWriter.WriteUInt64( source, numberOfBits, data, bitLength );
		//	bitLength = newLength;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( float source )
		{
			unsafe
			{
				Write( &source, 4 );
			}

			//unsafe
			//{
			//	uint val = *( (uint*)&source );
			//	Write( val );
			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( double source )
		{
			unsafe
			{
				Write( &source, 8 );
			}

			//unsafe
			//{
			//	ulong val = *( (ulong*)&source );
			//	Write( val );
			//}
		}

		//!!!!их тоже можно через поинтер все элементы сразу

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Vector2F source )
		{
			Write( source.X );
			Write( source.Y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector2F source )
		{
			Write( source.X );
			Write( source.Y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref RangeF source )
		{
			Write( source.Minimum );
			Write( source.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( RangeF source )
		{
			Write( source.Minimum );
			Write( source.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Vector3F source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector3F source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Vector4F source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector4F source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref BoundsF source )
		{
			Write( source.Minimum.X );
			Write( source.Minimum.Y );
			Write( source.Minimum.Z );
			Write( source.Maximum.X );
			Write( source.Maximum.Y );
			Write( source.Maximum.Z );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( BoundsF source )
		{
			Write( source.Minimum.X );
			Write( source.Minimum.Y );
			Write( source.Minimum.Z );
			Write( source.Maximum.X );
			Write( source.Maximum.Y );
			Write( source.Maximum.Z );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref QuaternionF source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( QuaternionF source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		//public void Write( QuaternionF source, int bitsPerElement )
		//{
		//	float x = source.X;
		//	float y = source.Y;
		//	float z = source.Z;
		//	float w = source.W;
		//	MathEx.Clamp( ref x, -1, 1 );
		//	MathEx.Clamp( ref y, -1, 1 );
		//	MathEx.Clamp( ref z, -1, 1 );
		//	MathEx.Clamp( ref w, -1, 1 );
		//	WriteRangedSingle( x, -1, 1, bitsPerElement );
		//	WriteRangedSingle( y, -1, 1, bitsPerElement );
		//	WriteRangedSingle( z, -1, 1, bitsPerElement );
		//	WriteRangedSingle( w, -1, 1, bitsPerElement );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref ColorValue source )
		{
			Write( source.Red );
			Write( source.Green );
			Write( source.Blue );
			Write( source.Alpha );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ColorValue source )
		{
			Write( source.Red );
			Write( source.Green );
			Write( source.Blue );
			Write( source.Alpha );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref SphericalDirectionF source )
		{
			Write( source.Horizontal );
			Write( source.Vertical );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( SphericalDirectionF source )
		{
			Write( source.Horizontal );
			Write( source.Vertical );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Vector2I source )
		{
			Write( source.X );
			Write( source.Y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector2I source )
		{
			Write( source.X );
			Write( source.Y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Vector3I source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector3I source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Vector4I source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector4I source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref RectangleF source )
		{
			Write( source.Left );
			Write( source.Top );
			Write( source.Right );
			Write( source.Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( RectangleF source )
		{
			Write( source.Left );
			Write( source.Top );
			Write( source.Right );
			Write( source.Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref RectangleI source )
		{
			Write( source.Left );
			Write( source.Top );
			Write( source.Right );
			Write( source.Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( RectangleI source )
		{
			Write( source.Left );
			Write( source.Top );
			Write( source.Right );
			Write( source.Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( DegreeF source )
		{
			Write( (float)source );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( RadianF source )
		{
			Write( (float)source );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void Write( string source )
		{
			if( !string.IsNullOrEmpty( source ) )
			{
				if( source.Length < 255 )
				{
					unsafe
					{
						fixed( char* chars = source )
						{
							var bytes = stackalloc byte[ 1024 ];
							var bytesLength = Encoding.UTF8.GetBytes( chars, source.Length, bytes, 1024 );
							WriteVariableUInt32( (uint)bytesLength );
							Write( bytes, bytesLength );
						}
					}
				}
				else
				{
					var bytes = Encoding.UTF8.GetBytes( source );
					WriteVariableUInt32( (uint)bytes.Length );
					Write( bytes );
				}
			}
			else
				WriteVariableUInt32( 0 );
		}

		/// <summary>
		/// Write Base128 encoded variable sized unsigned integer
		/// </summary>
		/// <returns>number of bytes written</returns>
		public int WriteVariableUInt32( uint source )
		{
			int retval = 1;
			uint num1 = source;
			while( num1 >= 0x80 )
			{
				Write( (byte)( num1 | 0x80 ) );
				num1 = num1 >> 7;
				retval++;
			}
			Write( (byte)num1 );
			return retval;
		}

		/// <summary>
		/// Write Base128 encoded variable sized signed integer
		/// </summary>
		/// <returns>number of bytes written</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int WriteVariableInt32( int source )
		{
			int retval = 1;
			uint num1 = (uint)( ( source << 1 ) ^ ( source >> 31 ) );
			while( num1 >= 0x80 )
			{
				Write( (byte)( num1 | 0x80 ) );
				num1 = num1 >> 7;
				retval++;
			}
			Write( (byte)num1 );
			return retval;
		}

		/// <summary>
		/// Write ulong encoded variable sized unsigned integer.
		/// </summary>
		/// <returns>number of bytes written</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int WriteVariableUInt64( ulong source )
		{
			int retval = 1;
			ulong num1 = source;
			while( num1 >= 0x80 )
			{
				Write( (byte)( num1 | 0x80 ) );
				num1 = num1 >> 7;
				retval++;
			}
			Write( (byte)num1 );
			return retval;
		}

		///// <summary>
		///// Compress (lossy) a float in the range -1..1 using numberOfBits bits
		///// </summary>
		//public void WriteSignedSingle( float source, int numberOfBits )
		//{
		//	if( source < -1 )
		//		Log.Fatal( "SendDataWriter: WriteRangedInteger: source < -1." );
		//	if( source > 1 )
		//		Log.Fatal( "SendDataWriter: WriteRangedInteger: source > 1." );

		//	float unit = ( source + 1.0f ) * 0.5f;
		//	int maxVal = ( 1 << numberOfBits ) - 1;
		//	uint writeVal = (uint)( unit * (float)maxVal );
		//	Write( writeVal, numberOfBits );
		//}

		///// <summary>
		///// Compress (lossy) a float in the range 0..1 using numberOfBits bits
		///// </summary>
		//public void WriteUnitSingle( float source, int numberOfBits )
		//{
		//	if( source < 0 )
		//		Log.Fatal( "SendDataWriter: WriteRangedInteger: source < 0." );
		//	if( source > 1 )
		//		Log.Fatal( "SendDataWriter: WriteRangedInteger: source > 1." );

		//	int maxValue = ( 1 << numberOfBits ) - 1;
		//	uint writeVal = (uint)( source * (float)maxValue );
		//	Write( writeVal, numberOfBits );
		//}

		///// <summary>
		///// Compress a float within a specified range using a certain number of bits
		///// </summary>
		//public void WriteRangedSingle( float source, float min, float max, int numberOfBits )
		//{
		//	if( source < min )
		//		Log.Fatal( "SendDataWriter: WriteRangedInteger: source < min." );
		//	if( source > max )
		//		Log.Fatal( "SendDataWriter: WriteRangedInteger: source > max." );

		//	float range = max - min;
		//	float unit = ( ( source - min ) / range );
		//	int maxVal = ( 1 << numberOfBits ) - 1;
		//	Write( (uint)( (float)maxVal * unit ), numberOfBits );
		//}

		//static int BitsToHoldUInt( uint value )
		//{
		//	int bits = 1;
		//	while( ( value >>= 1 ) != 0 )
		//		bits++;
		//	return bits;
		//}

		///// <summary>
		///// Writes an integer with the least amount of bits need for the specified range
		///// </summary>
		///// <returns>number of bits written</returns>
		//public int WriteRangedInteger( int source, int min, int max )
		//{
		//	if( source < min )
		//		Log.Fatal( "SendDataWriter: WriteRangedInteger: source < min." );
		//	if( source > max )
		//		Log.Fatal( "SendDataWriter: WriteRangedInteger: source > max." );

		//	uint range = (uint)( max - min );
		//	int numBits = BitsToHoldUInt( range );

		//	uint rvalue = (uint)( source - min );
		//	Write( rvalue, numBits );

		//	return numBits;
		//}

		///// <summary>
		///// Pads data with enough bits to reach a full byte. Decreases cpu usage for subsequent byte writes.
		///// </summary>
		//public void WritePadBits()
		//{
		//	bitLength += ( ( bitLength + 7 ) / 8 ) * 8;
		//	ExpandBuffer( bitLength );
		//}

		///// <summary>
		///// Pads data with the specified number of bits.
		///// </summary>
		//public void WritePadBits( int numberOfBits )
		//{
		//	bitLength += numberOfBits;
		//	ExpandBuffer( bitLength );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Vector2 source )
		{
			Write( source.X );
			Write( source.Y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector2 source )
		{
			Write( source.X );
			Write( source.Y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Range source )
		{
			Write( source.Minimum );
			Write( source.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Range source )
		{
			Write( source.Minimum );
			Write( source.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Vector3 source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector3 source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Vector4 source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector4 source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Bounds source )
		{
			Write( source.Minimum.X );
			Write( source.Minimum.Y );
			Write( source.Minimum.Z );
			Write( source.Maximum.X );
			Write( source.Maximum.Y );
			Write( source.Maximum.Z );
		}
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Bounds source )
		{
			Write( source.Minimum.X );
			Write( source.Minimum.Y );
			Write( source.Minimum.Z );
			Write( source.Maximum.X );
			Write( source.Maximum.Y );
			Write( source.Maximum.Z );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Quaternion source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Quaternion source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref SphericalDirection source )
		{
			Write( source.Horizontal );
			Write( source.Vertical );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( SphericalDirection source )
		{
			Write( source.Horizontal );
			Write( source.Vertical );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( ref Rectangle source )
		{
			Write( source.Left );
			Write( source.Top );
			Write( source.Right );
			Write( source.Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Rectangle source )
		{
			Write( source.Left );
			Write( source.Top );
			Write( source.Right );
			Write( source.Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Degree source )
		{
			Write( (double)source );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Radian source )
		{
			Write( (double)source );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( DateTime source )
		{
			Write( source.Ticks );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public byte[] ToArray()
		{
			var result = new byte[ Length ];
			Array.Copy( data, 0, result, 0, Length );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( HalfType source )
		{
			unsafe
			{
				Write( &source, sizeof( HalfType ) );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector2H source )
		{
			unsafe
			{
				Write( &source, sizeof( Vector2H ) );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector3H source )
		{
			unsafe
			{
				Write( &source, sizeof( Vector3H ) );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write( Vector4H source )
		{
			unsafe
			{
				Write( &source, sizeof( Vector4H ) );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Write<T>( ref T source ) where T : unmanaged
		{
			unsafe
			{
				fixed( T* pSource = &source )
					Write( pSource, sizeof( T ) );
			}
		}

		//!!!!more types
	}
}
