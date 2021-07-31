// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// A class for streaming data to an array.
	/// </summary>
	public class ArrayDataWriter
	{
		byte[] data = new byte[ 4 ];
		int bitLength;

		//

		public ArrayDataWriter()
		{
		}

		public void Reset()
		{
			bitLength = 0;
		}

		public byte[] Data
		{
			get { return data; }
		}

		public int BitLength
		{
			get { return bitLength; }
		}

		public int GetByteLength()
		{
			return ( bitLength >> 3 ) + ( ( bitLength & 7 ) > 0 ? 1 : 0 );
		}

		void ExpandBuffer( int demandBitLength )
		{
			int demandByteLength = ( demandBitLength >> 3 ) + ( ( demandBitLength & 7 ) > 0 ? 1 : 0 );

			if( data.Length < demandByteLength )
			{
				int newLength = data.Length;
				while( newLength < demandByteLength )
					newLength *= 2;
				Array.Resize<byte>( ref data, newLength );
			}
		}

		public void Write( byte[] source, int byteOffset, int byteLength )
		{
			int newLength = bitLength + byteLength * 8;
			ExpandBuffer( newLength );
			BitWriter.WriteBytes( source, byteOffset, byteLength, data, bitLength );
			bitLength = newLength;
		}

		public void Write( byte[] source )
		{
			Write( source, 0, source.Length );
		}

		public void Write( bool source )
		{
			int newLength = bitLength + 1;
			ExpandBuffer( newLength );
			BitWriter.WriteByte( ( source ? (byte)1 : (byte)0 ), 1, data, bitLength );
			bitLength = newLength;
		}

		public void Write( byte source )
		{
			int newLength = bitLength + 8;
			ExpandBuffer( newLength );
			BitWriter.WriteByte( source, 8, data, bitLength );
			bitLength = newLength;
		}

		public void Write( byte source, int numberOfBits )
		{
			int newLength = bitLength + numberOfBits;
			ExpandBuffer( newLength );
			BitWriter.WriteByte( source, numberOfBits, data, bitLength );
			bitLength = newLength;
		}

		//public void Write( sbyte source )
		//{
		//   unchecked
		//   {
		//      Write( (byte)source );
		//   }
		//}

		//public void Write( sbyte source, int numberOfBits )

		public void Write( ushort source )
		{
			int newLength = bitLength + 16;
			ExpandBuffer( newLength );
			BitWriter.WriteUInt32( (uint)source, 16, data, bitLength );
			bitLength = newLength;
		}

		public void Write( ushort source, int numberOfBits )
		{
			int newLength = bitLength + numberOfBits;
			ExpandBuffer( newLength );
			BitWriter.WriteUInt32( (uint)source, numberOfBits, data, bitLength );
			bitLength = newLength;
		}

		public void Write( short source )
		{
			unchecked
			{
				Write( (ushort)source );
			}
		}

		//public void Write( short source, int numberOfBits )

		public void Write( int source )
		{
			int newLength = bitLength + 32;
			ExpandBuffer( newLength );

			if( bitLength % 8 == 0 )
			{
				unsafe
				{
					fixed ( byte* numRef = &data[ bitLength / 8 ] )
					{
						*( (int*)numRef ) = source;
					}
				}
			}
			else
				BitWriter.WriteUInt32( (uint)source, 32, data, bitLength );

			bitLength = newLength;
		}

		public void Write( int source, int numberOfBits )
		{
			int newLength = bitLength + numberOfBits;
			ExpandBuffer( newLength );

			if( numberOfBits != 32 )
			{
				//make first bit sign
				int signBit = 1 << ( numberOfBits - 1 );
				if( source < 0 )
					source = ( -source - 1 ) | signBit;
				else
					source &= ( ~signBit );
			}

			BitWriter.WriteUInt32( (uint)source, numberOfBits, data, bitLength );

			bitLength = newLength;
		}

		public void Write( uint source )
		{
			int newLength = bitLength + 32;
			ExpandBuffer( newLength );

			if( bitLength % 8 == 0 )
			{
				unsafe
				{
					fixed ( byte* numRef = &data[ bitLength / 8 ] )
					{
						*( (uint*)numRef ) = source;
					}
				}
			}
			else
			{
				BitWriter.WriteUInt32( source, 32, data, bitLength );
			}

			bitLength = newLength;
		}

		public void Write( uint source, int numberOfBits )
		{
			int newLength = bitLength + numberOfBits;
			ExpandBuffer( newLength );
			BitWriter.WriteUInt32( source, numberOfBits, data, bitLength );
			bitLength = newLength;
		}

		public void Write( long source )
		{
			int newLength = bitLength + 64;
			ExpandBuffer( newLength );
			ulong usource = (ulong)source;
			BitWriter.WriteUInt64( usource, 64, data, bitLength );
			bitLength = newLength;
		}

		public void Write( long source, int numberOfBits )
		{
			int newLength = bitLength + numberOfBits;
			ExpandBuffer( newLength );
			ulong usource = (ulong)source;
			BitWriter.WriteUInt64( usource, numberOfBits, data, bitLength );
			bitLength = newLength;
		}

		public void Write( ulong source )
		{
			int newLength = bitLength + 64;
			ExpandBuffer( newLength );
			BitWriter.WriteUInt64( source, 64, data, bitLength );
			bitLength = newLength;
		}

		public void Write( ulong source, int numberOfBits )
		{
			int newLength = bitLength + numberOfBits;
			ExpandBuffer( newLength );
			BitWriter.WriteUInt64( source, numberOfBits, data, bitLength );
			bitLength = newLength;
		}

		public void Write( float source )
		{
			unsafe
			{
				uint val = *( (uint*)&source );
				Write( val );
			}
		}

		public void Write( double source )
		{
			unsafe
			{
				ulong val = *( (ulong*)&source );
				Write( val );
			}
		}

		public void Write( Vector2F source )
		{
			Write( source.X );
			Write( source.Y );
		}

		public void Write( RangeF source )
		{
			Write( source.Minimum );
			Write( source.Maximum );
		}

		public void Write( Vector3F source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
		}

		public void Write( Vector4F source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		public void Write( BoundsF source )
		{
			Write( source.Minimum.X );
			Write( source.Minimum.Y );
			Write( source.Minimum.Z );
			Write( source.Maximum.X );
			Write( source.Maximum.Y );
			Write( source.Maximum.Z );
		}

		public void Write( QuaternionF source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		public void Write( QuaternionF source, int bitsPerElement )
		{
			float x = source.X;
			float y = source.Y;
			float z = source.Z;
			float w = source.W;
			MathEx.Clamp( ref x, -1, 1 );
			MathEx.Clamp( ref y, -1, 1 );
			MathEx.Clamp( ref z, -1, 1 );
			MathEx.Clamp( ref w, -1, 1 );
			WriteRangedSingle( x, -1, 1, bitsPerElement );
			WriteRangedSingle( y, -1, 1, bitsPerElement );
			WriteRangedSingle( z, -1, 1, bitsPerElement );
			WriteRangedSingle( w, -1, 1, bitsPerElement );
		}

		public void Write( ColorValue source )
		{
			Write( source.Red );
			Write( source.Green );
			Write( source.Blue );
			Write( source.Alpha );
		}

		public void Write( SphericalDirectionF source )
		{
			Write( source.Horizontal );
			Write( source.Vertical );
		}

		public void Write( Vector2I source )
		{
			Write( source.X );
			Write( source.Y );
		}

		public void Write( Vector3I source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
		}

		public void Write( Vector4I source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		public void Write( RectangleF source )
		{
			Write( source.Minimum.X );
			Write( source.Minimum.Y );
			Write( source.Maximum.X );
			Write( source.Maximum.Y );
		}

		public void Write( RectangleI source )
		{
			Write( source.Minimum.X );
			Write( source.Minimum.Y );
			Write( source.Maximum.X );
			Write( source.Maximum.Y );
		}

		public void Write( DegreeF source )
		{
			Write( (float)source );
		}

		public void Write( RadianF source )
		{
			Write( (float)source );
		}

		public void Write( string source )
		{
			if( string.IsNullOrEmpty( source ) )
			{
				WriteVariableUInt32( 0 );
				return;
			}

			byte[] bytes = Encoding.UTF8.GetBytes( source );
			WriteVariableUInt32( (uint)bytes.Length );
			Write( bytes );
		}

		/// <summary>
		/// Write Base128 encoded variable sized unsigned integer
		/// </summary>
		/// <returns>number of bytes written</returns>
		public int WriteVariableUInt32( uint source )
		{
			int retval = 1;
			uint num1 = (uint)source;
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
		/// Write Base128 encoded variable sized unsigned integer
		/// </summary>
		/// <returns>number of bytes written</returns>
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

		/// <summary>
		/// Compress (lossy) a float in the range -1..1 using numberOfBits bits
		/// </summary>
		public void WriteSignedSingle( float source, int numberOfBits )
		{
			if( source < -1 )
				Log.Fatal( "SendDataWriter: WriteRangedInteger: source < -1." );
			if( source > 1 )
				Log.Fatal( "SendDataWriter: WriteRangedInteger: source > 1." );

			float unit = ( source + 1.0f ) * 0.5f;
			int maxVal = ( 1 << numberOfBits ) - 1;
			uint writeVal = (uint)( unit * (float)maxVal );
			Write( writeVal, numberOfBits );
		}

		/// <summary>
		/// Compress (lossy) a float in the range 0..1 using numberOfBits bits
		/// </summary>
		public void WriteUnitSingle( float source, int numberOfBits )
		{
			if( source < 0 )
				Log.Fatal( "SendDataWriter: WriteRangedInteger: source < 0." );
			if( source > 1 )
				Log.Fatal( "SendDataWriter: WriteRangedInteger: source > 1." );

			int maxValue = ( 1 << numberOfBits ) - 1;
			uint writeVal = (uint)( source * (float)maxValue );
			Write( writeVal, numberOfBits );
		}

		/// <summary>
		/// Compress a float within a specified range using a certain number of bits
		/// </summary>
		public void WriteRangedSingle( float source, float min, float max, int numberOfBits )
		{
			if( source < min )
				Log.Fatal( "SendDataWriter: WriteRangedInteger: source < min." );
			if( source > max )
				Log.Fatal( "SendDataWriter: WriteRangedInteger: source > max." );

			float range = max - min;
			float unit = ( ( source - min ) / range );
			int maxVal = ( 1 << numberOfBits ) - 1;
			Write( (uint)( (float)maxVal * unit ), numberOfBits );
		}

		static int BitsToHoldUInt( uint value )
		{
			int bits = 1;
			while( ( value >>= 1 ) != 0 )
				bits++;
			return bits;
		}

		/// <summary>
		/// Writes an integer with the least amount of bits need for the specified range
		/// </summary>
		/// <returns>number of bits written</returns>
		public int WriteRangedInteger( int source, int min, int max )
		{
			if( source < min )
				Log.Fatal( "SendDataWriter: WriteRangedInteger: source < min." );
			if( source > max )
				Log.Fatal( "SendDataWriter: WriteRangedInteger: source > max." );

			uint range = (uint)( max - min );
			int numBits = BitsToHoldUInt( range );

			uint rvalue = (uint)( source - min );
			Write( rvalue, numBits );

			return numBits;
		}

		/// <summary>
		/// Pads data with enough bits to reach a full byte. Decreases cpu usage for subsequent byte writes.
		/// </summary>
		public void WritePadBits()
		{
			bitLength += ( ( bitLength + 7 ) / 8 ) * 8;
			ExpandBuffer( bitLength );
		}

		/// <summary>
		/// Pads data with the specified number of bits.
		/// </summary>
		public void WritePadBits( int numberOfBits )
		{
			bitLength += numberOfBits;
			ExpandBuffer( bitLength );
		}

		public void Write( Vector2 source )
		{
			Write( source.X );
			Write( source.Y );
		}

		public void Write( Range source )
		{
			Write( source.Minimum );
			Write( source.Maximum );
		}

		public void Write( Vector3 source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
		}

		public void Write( Vector4 source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		public void Write( Bounds source )
		{
			Write( source.Minimum.X );
			Write( source.Minimum.Y );
			Write( source.Minimum.Z );
			Write( source.Maximum.X );
			Write( source.Maximum.Y );
			Write( source.Maximum.Z );
		}

		public void Write( Quaternion source )
		{
			Write( source.X );
			Write( source.Y );
			Write( source.Z );
			Write( source.W );
		}

		public void Write( SphericalDirection source )
		{
			Write( source.Horizontal );
			Write( source.Vertical );
		}

		public void Write( Rectangle source )
		{
			Write( source.Minimum.X );
			Write( source.Minimum.Y );
			Write( source.Maximum.X );
			Write( source.Maximum.Y );
		}

		public void Write( Degree source )
		{
			Write( (double)source );
		}

		public void Write( Radian source )
		{
			Write( (double)source );
		}

		//!!!!more types
	}
}
