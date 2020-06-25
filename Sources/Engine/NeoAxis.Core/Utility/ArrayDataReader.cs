// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// A class for streaming data from an array.
	/// </summary>
	public class ArrayDataReader
	{
		byte[] data;
		int bitPosition;
		int startBitPosition;
		int endBitPosition;
		bool overflow;

		//

		public ArrayDataReader()
		{
		}

		public ArrayDataReader( byte[] data, int bitOffset, int bitLength )
		{
			Init( data, bitOffset, bitLength );
		}

		public ArrayDataReader( byte[] data )
		{
			Init( data, 0, data.Length * 8 );
		}

		public void Init( byte[] data, int bitOffset, int bitLength )
		{
			this.data = data;
			bitPosition = bitOffset;
			startBitPosition = bitPosition;
			endBitPosition = bitPosition + bitLength;
			overflow = false;
		}

		public int BitPosition
		{
			get { return bitPosition; }
		}

		public int StartBitPosition
		{
			get { return startBitPosition; }
		}

		public int EndBitPosition
		{
			get { return endBitPosition; }
		}

		public bool Overflow
		{
			get { return overflow; }
		}

		public bool Complete()
		{
			return bitPosition == endBitPosition && !overflow;
		}

		public void ReadBuffer( byte[] destination, int byteOffset, int byteLength )
		{
			int newPosition = bitPosition + byteLength * 8;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return;
			}
			BitWriter.ReadBytes( data, byteLength, bitPosition, destination, byteOffset );
			bitPosition = newPosition;
		}

		public void ReadBuffer( byte[] destination )
		{
			ReadBuffer( destination, 0, destination.Length );
		}

		public bool ReadBoolean()
		{
			int newPosition = bitPosition + 1;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return false;
			}
			byte value = BitWriter.ReadByte( data, 1, bitPosition );
			bitPosition = newPosition;
			return ( value > 0 ? true : false );
		}

		public byte ReadByte()
		{
			int newPosition = bitPosition + 8;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			byte value = BitWriter.ReadByte( data, 8, bitPosition );
			bitPosition = newPosition;
			return value;
		}

		public byte ReadByte( int numberOfBits )
		{
			int newPosition = bitPosition + numberOfBits;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			byte value = BitWriter.ReadByte( data, numberOfBits, bitPosition );
			bitPosition = newPosition;
			return value;
		}

		//public sbyte ReadSByte()
		//{
		//   unchecked
		//   {
		//      return (sbyte)ReadByte();
		//   }
		//}

		//public byte ReadSByte( int numberOfBits )

		public short ReadInt16()
		{
			int newPosition = bitPosition + 16;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			uint value = BitWriter.ReadUInt32( data, 16, bitPosition );
			bitPosition = newPosition;
			return (short)value;
		}

		//public byte ReadInt16( int numberOfBits )

		public ushort ReadUInt16()
		{
			int newPosition = bitPosition + 16;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			uint value = BitWriter.ReadUInt32( data, 16, bitPosition );
			bitPosition = newPosition;
			return (ushort)value;
		}

		//public byte ReadUInt16( int numberOfBits )

		public int ReadInt32()
		{
			int newPosition = bitPosition + 32;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			uint value = BitWriter.ReadUInt32( data, 32, bitPosition );
			bitPosition = newPosition;
			return (int)value;
		}

		public int ReadInt32( int numberOfBits )
		{
			int newPosition = bitPosition + numberOfBits;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}

			uint value = BitWriter.ReadUInt32( data, numberOfBits, bitPosition );
			bitPosition += numberOfBits;

			if( numberOfBits == 32 )
				return (int)value;

			int signBit = 1 << ( numberOfBits - 1 );
			if( ( value & signBit ) == 0 )
				return (int)value; // positive

			// negative
			unchecked
			{
				uint mask = ( (uint)-1 ) >> ( 33 - numberOfBits );
				uint tmp = ( value & mask ) + 1;
				return -( (int)tmp );
			}
		}

		public uint ReadUInt32()
		{
			int newPosition = bitPosition + 32;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			uint value = BitWriter.ReadUInt32( data, 32, bitPosition );
			bitPosition += 32;
			return value;
		}

		public UInt32 ReadUInt32( int numberOfBits )
		{
			int newPosition = bitPosition + numberOfBits;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			uint value = BitWriter.ReadUInt32( data, numberOfBits, bitPosition );
			bitPosition += numberOfBits;
			return value;
		}

		public long ReadInt64()
		{
			int newPosition = bitPosition + 64;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			unchecked
			{
				ulong value = ReadUInt64();
				long longValue = (long)value;
				return longValue;
			}
		}

		public ulong ReadUInt64()
		{
			int newPosition = bitPosition + 64;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			ulong low = BitWriter.ReadUInt32( data, 32, bitPosition );
			bitPosition += 32;
			ulong high = BitWriter.ReadUInt32( data, 32, bitPosition );
			bitPosition += 32;
			ulong value = low + ( high << 32 );
			return value;
		}

		public ulong ReadUInt64( int numberOfBits )
		{
			int newPosition = bitPosition + numberOfBits;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}

			ulong value;
			if( numberOfBits <= 32 )
			{
				value = (ulong)BitWriter.ReadUInt32( data, numberOfBits, bitPosition );
			}
			else
			{
				value = BitWriter.ReadUInt32( data, 32, bitPosition );
				value |= BitWriter.ReadUInt32( data, numberOfBits - 32, bitPosition ) << 32;
			}
			bitPosition += numberOfBits;
			return value;
		}

		public long ReadInt64( int numberOfBits )
		{
			int newPosition = bitPosition + numberOfBits;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			return (long)ReadUInt64( numberOfBits );
		}

		public float ReadSingle()
		{
			int newPosition = bitPosition + 32;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}

			//read directly
			if( ( bitPosition & 7 ) == 0 )
			{
				//endianness is handled inside BitConverter.ToSingle
				float value = BitConverter.ToSingle( data, bitPosition >> 3 );
				bitPosition += 32;
				return value;
			}

			byte[] bytes = new byte[ 4 ];
			ReadBuffer( bytes );
			if( overflow )
				return 0;
			//endianness is handled inside BitConverter.ToSingle
			return BitConverter.ToSingle( bytes, 0 );
		}

		public double ReadDouble()
		{
			int newPosition = bitPosition + 64;
			if( overflow || newPosition > endBitPosition )
			{
				overflow = true;
				return 0;
			}

			//read directly
			if( ( bitPosition & 7 ) == 0 )
			{
				double value = BitConverter.ToDouble( data, bitPosition >> 3 );
				bitPosition += 64;
				return value;
			}

			byte[] bytes = new byte[ 8 ];
			ReadBuffer( bytes );
			if( overflow )
				return 0;
			//endianness is handled inside BitConverter.ToSingle
			return BitConverter.ToDouble( bytes, 0 );
		}

		/// <summary>
		/// Reads a UInt32 written using WriteUnsignedVarInt()
		/// </summary>
		public uint ReadVariableUInt32()
		{
			if( overflow )
				return 0;

			int num1 = 0;
			int num2 = 0;
			while( true )
			{
				if( num2 == 0x23 )
				{
					overflow = true;
					return 0;
					//throw new FormatException( "Bad 7-bit encoded integer" );
				}

				byte num3 = ReadByte();
				if( overflow )
					return 0;

				num1 |= ( num3 & 0x7f ) << ( num2 & 0x1f );
				num2 += 7;
				if( ( num3 & 0x80 ) == 0 )
					return (uint)num1;
			}
		}

		/// <summary>
		/// Reads a Int32 written using WriteSignedVarInt()
		/// </summary>
		public int ReadVariableInt32()
		{
			if( overflow )
				return 0;

			int num1 = 0;
			int num2 = 0;
			while( true )
			{
				if( num2 == 0x23 )
				{
					overflow = true;
					return 0;
					//throw new FormatException( "Bad 7-bit encoded integer" );
				}

				byte num3 = ReadByte();
				if( overflow )
					return 0;

				num1 |= ( num3 & 0x7f ) << ( num2 & 0x1f );
				num2 += 7;
				if( ( num3 & 0x80 ) == 0 )
				{
					int sign = ( num1 << 31 ) >> 31;
					return sign ^ ( num1 >> 1 );
				}
			}
		}

		/// <summary>
		/// Reads a UInt32 written using WriteUnsignedVarInt()
		/// </summary>
		public ulong ReadVariableUInt64()
		{
			if( overflow )
				return 0;

			ulong num1 = 0;
			int num2 = 0;
			while( true )
			{
				if( num2 == 0x77 )
				{
					overflow = true;
					return 0;
					//throw new FormatException( "Bad 7-bit encoded integer" );
				}

				byte num3 = ReadByte();
				if( overflow )
					return 0;

				num1 |= ( (ulong)num3 & 0x7f ) << num2;
				num2 += 7;
				if( ( num3 & 0x80 ) == 0 )
					return num1;
			}
		}

		/// <summary>
		/// Reads a float written using WriteSignedSingle()
		/// </summary>
		public float ReadSignedSingle( int numberOfBits )
		{
			if( overflow || bitPosition + numberOfBits > endBitPosition )
			{
				overflow = true;
				return 0;
			}

			uint encodedVal = ReadUInt32( numberOfBits );
			int maxVal = ( 1 << numberOfBits ) - 1;
			return ( (float)( encodedVal + 1 ) / (float)( maxVal + 1 ) - 0.5f ) * 2.0f;
		}

		/// <summary>
		/// Reads a float written using WriteUnitSingle()
		/// </summary>
		public float ReadUnitSingle( int numberOfBits )
		{
			if( overflow || bitPosition + numberOfBits > endBitPosition )
			{
				overflow = true;
				return 0;
			}

			uint encodedVal = ReadUInt32( numberOfBits );
			int maxVal = ( 1 << numberOfBits ) - 1;
			return (float)( encodedVal + 1 ) / (float)( maxVal + 1 );
		}

		/// <summary>
		/// Reads a float written using WriteRangedSingle() using the same MIN and MAX values
		/// </summary>
		public float ReadRangedSingle( float min, float max, int numberOfBits )
		{
			if( overflow || bitPosition + numberOfBits > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			float range = max - min;
			int maxVal = ( 1 << numberOfBits ) - 1;
			float encodedVal = (float)ReadUInt32( numberOfBits );
			float unit = encodedVal / (float)maxVal;
			return min + ( unit * range );
		}

		static int BitsToHoldUInt( uint value )
		{
			int bits = 1;
			while( ( value >>= 1 ) != 0 )
				bits++;
			return bits;
		}

		/// <summary>
		/// Reads an integer written using WriteRangedInteger() using the same min/max values
		/// </summary>
		public int ReadRangedInteger( int min, int max )
		{
			uint range = (uint)( max - min );
			int numBits = BitsToHoldUInt( range );
			if( overflow || bitPosition + numBits > endBitPosition )
			{
				overflow = true;
				return 0;
			}
			uint rvalue = ReadUInt32( numBits );
			return (int)( min + rvalue );
		}

		public Vector2F ReadVec2F()
		{
			return new Vector2F( ReadSingle(), ReadSingle() );
		}

		public RangeF ReadRangeF()
		{
			return new RangeF( ReadSingle(), ReadSingle() );
		}

		public Vector3F ReadVec3F()
		{
			return new Vector3F( ReadSingle(), ReadSingle(), ReadSingle() );
		}

		public Vector4F ReadVec4F()
		{
			return new Vector4F( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		public BoundsF ReadBoundsF()
		{
			return new BoundsF( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		public QuaternionF ReadQuatF()
		{
			return new QuaternionF( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		public QuaternionF ReadQuatF( int bitsPerElement )
		{
			return new QuaternionF(
				ReadRangedSingle( -1, 1, bitsPerElement ),
				ReadRangedSingle( -1, 1, bitsPerElement ),
				ReadRangedSingle( -1, 1, bitsPerElement ),
				ReadRangedSingle( -1, 1, bitsPerElement ) );
		}

		public ColorValue ReadColorValue()
		{
			return new ColorValue( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		public SphericalDirectionF ReadSphericalDirectionF()
		{
			return new SphericalDirectionF( ReadSingle(), ReadSingle() );
		}

		public Vector2I ReadVec2I()
		{
			return new Vector2I( ReadInt32(), ReadInt32() );
		}

		public Vector3I ReadVec3I()
		{
			return new Vector3I( ReadInt32(), ReadInt32(), ReadInt32() );
		}

		public Vector4I ReadVec4I()
		{
			return new Vector4I( ReadInt32(), ReadInt32(), ReadInt32(), ReadInt32() );
		}

		public RectangleF ReadRectF()
		{
			return new RectangleF( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		public RectangleI ReadRectI()
		{
			return new RectangleI( ReadInt32(), ReadInt32(), ReadInt32(), ReadInt32() );
		}

		public DegreeF ReadDegreeF()
		{
			return new DegreeF( ReadSingle() );
		}

		public RadianF ReadRadianF()
		{
			return new RadianF( ReadSingle() );
		}

		public string ReadString()
		{
			int byteLength = (int)ReadVariableUInt32();

			if( byteLength == 0 )
				return "";

			if( overflow || bitPosition + byteLength * 8 > endBitPosition )
			{
				overflow = true;
				return "";
			}

			if( ( bitPosition & 7 ) == 0 )
			{
				//read directly
				string result = System.Text.Encoding.UTF8.GetString( data, bitPosition >> 3, byteLength );
				bitPosition += ( byteLength * 8 );
				return result;
			}

			byte[] bytes = new byte[ byteLength ];
			ReadBuffer( bytes );
			if( overflow )
				return "";
			return System.Text.Encoding.UTF8.GetString( bytes, 0, bytes.Length );
		}

		/// <summary>
		/// Pads data with enough bits to reach a full byte. Decreases cpu usage for subsequent byte writes.
		/// </summary>
		public void SkipPadBits()
		{
			bitPosition = ( ( bitPosition + 7 ) / 8 ) * 8;
		}

		/// <summary>
		/// Pads data with the specified number of bits.
		/// </summary>
		public void SkipPadBits( int numberOfBits )
		{
			bitPosition += numberOfBits;
		}

		public Vector2 ReadVec2()
		{
			return new Vector2( ReadDouble(), ReadDouble() );
		}

		public Range ReadRange()
		{
			return new Range( ReadDouble(), ReadDouble() );
		}

		public Vector3 ReadVec3()
		{
			return new Vector3( ReadDouble(), ReadDouble(), ReadDouble() );
		}

		public Vector4 ReadVec4()
		{
			return new Vector4( ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble() );
		}

		public Bounds ReadBounds()
		{
			return new Bounds( ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble() );
		}

		public Quaternion ReadQuat()
		{
			return new Quaternion( ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble() );
		}

		public SphericalDirection ReadSphericalDirection()
		{
			return new SphericalDirection( ReadDouble(), ReadDouble() );
		}

		public Rectangle ReadRect()
		{
			return new Rectangle( ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble() );
		}

		public Degree ReadDegree()
		{
			return new Degree( ReadDouble() );
		}

		public Radian ReadRadian()
		{
			return new Radian( ReadDouble() );
		}

		//!!!!more types
	}
}
