// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;

namespace NeoAxis
{
	/// <summary>
	/// A class for streaming data from an array.
	/// </summary>
	public class ArrayDataReader
	{
		byte[] data;
		int currentPosition;
		int endPosition;
		bool overflow;

		//

		public ArrayDataReader()
		{
		}

		public ArrayDataReader( byte[] data )
		{
			this.data = data;
			endPosition = data.Length;
		}

		public ArrayDataReader( byte[] data, int startPosition, int length )
		{
			Init( data, startPosition, length );
		}

		public ArrayDataReader( ArraySegment<byte> data )
		{
			Init( data.Array, data.Offset, data.Count );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Init( byte[] data, int startPosition, int length )
		{
			this.data = data;
			currentPosition = startPosition;
			endPosition = startPosition + length;
			overflow = false;
		}

		public int CurrentPosition
		{
			get { return currentPosition; }
		}

		public int EndPosition
		{
			get { return endPosition; }
		}

		public bool Overflow
		{
			get { return overflow; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Complete()
		{
			return currentPosition == endPosition && !overflow;
		}

		public void ReadSkip( int length )
		{
			currentPosition += length;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe void ReadBuffer( void* destination, int length )
		{
			int newPosition = currentPosition + length;
			if( overflow || newPosition > endPosition )
			{
				overflow = true;
				return;
			}

			fixed( byte* pData = data )
			{
				byte* p = pData + currentPosition;

				if( length == 8 )
					*(ulong*)destination = *(ulong*)p;
				else if( length == 4 )
					*(uint*)destination = *(uint*)p;
				else if( length == 2 )
					*(ushort*)destination = *(ushort*)p;
				else
					Buffer.MemoryCopy( p, destination, length, length );
			}

			//Marshal.Copy( data, currentPosition, (IntPtr)destination, length );

			currentPosition = newPosition;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadBuffer( byte[] destination, int offset, int length )
		{
			int newPosition = currentPosition + length;
			if( overflow || newPosition > endPosition )
			{
				overflow = true;
				return;
			}
			Array.Copy( data, currentPosition, destination, offset, length );
			currentPosition = newPosition;
		}

		//public void ReadBuffer( byte[] destination, int byteOffset, int byteLength )
		//{
		//	int newPosition = bitPosition + byteLength * 8;
		//	if( overflow || newPosition > endBitPosition )
		//	{
		//		overflow = true;
		//		return;
		//	}
		//	BitWriter.ReadBytes( data, byteLength, bitPosition, destination, byteOffset );
		//	bitPosition = newPosition;
		//}

		//public void ReadBuffer( byte[] destination )
		//{
		//	ReadBuffer( destination, 0, destination.Length );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool ReadBoolean()
		{
			return ReadByte() != 0;

			//int newPosition = bitPosition + 1;
			//if( overflow || newPosition > endBitPosition )
			//{
			//	overflow = true;
			//	return false;
			//}
			//byte value = BitWriter.ReadByte( data, 1, bitPosition );
			//bitPosition = newPosition;
			//return ( value > 0 ? true : false );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public byte ReadByte()
		{
			int newPosition = currentPosition + 1;
			if( overflow || newPosition > endPosition )
			{
				overflow = true;
				return 0;
			}
			var value = data[ currentPosition ];
			currentPosition = newPosition;
			return value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public sbyte ReadSByte()
		{
			unsafe
			{
				sbyte result = 0;
				ReadBuffer( &result, 1 );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public short ReadInt16()
		{
			unsafe
			{
				short result = 0;
				ReadBuffer( &result, 2 );
				return result;
			}

			//int newPosition = bitPosition + 16;
			//if( overflow || newPosition > endBitPosition )
			//{
			//	overflow = true;
			//	return 0;
			//}
			//uint value = BitWriter.ReadUInt32( data, 16, bitPosition );
			//bitPosition = newPosition;
			//return (short)value;
		}

		public short ReadShort()
		{
			return ReadInt16();
		}

		//public byte ReadInt16( int numberOfBits )

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ushort ReadUInt16()
		{
			unsafe
			{
				ushort result = 0;
				ReadBuffer( &result, 2 );
				return result;
			}

			//int newPosition = bitPosition + 16;
			//if( overflow || newPosition > endBitPosition )
			//{
			//	overflow = true;
			//	return 0;
			//}
			//uint value = BitWriter.ReadUInt32( data, 16, bitPosition );
			//bitPosition = newPosition;
			//return (ushort)value;
		}

		public ushort ReadUShort()
		{
			return ReadUInt16();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public char ReadChar()
		{
			unsafe
			{
				char result = (char)0;
				ReadBuffer( &result, 2 );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int ReadInt32()
		{
			unsafe
			{
				int result = 0;
				ReadBuffer( &result, 4 );
				return result;
			}

			//int newPosition = bitPosition + 32;
			//if( overflow || newPosition > endBitPosition )
			//{
			//	overflow = true;
			//	return 0;
			//}
			//uint value = BitWriter.ReadUInt32( data, 32, bitPosition );
			//bitPosition = newPosition;
			//return (int)value;
		}

		public int ReadInt()
		{
			return ReadInt32();
		}

		//public int ReadInt32( int numberOfBits )
		//{
		//	int newPosition = bitPosition + numberOfBits;
		//	if( overflow || newPosition > endBitPosition )
		//	{
		//		overflow = true;
		//		return 0;
		//	}

		//	uint value = BitWriter.ReadUInt32( data, numberOfBits, bitPosition );
		//	bitPosition += numberOfBits;

		//	if( numberOfBits == 32 )
		//		return (int)value;

		//	int signBit = 1 << ( numberOfBits - 1 );
		//	if( ( value & signBit ) == 0 )
		//		return (int)value; // positive

		//	// negative
		//	unchecked
		//	{
		//		uint mask = ( (uint)-1 ) >> ( 33 - numberOfBits );
		//		uint tmp = ( value & mask ) + 1;
		//		return -( (int)tmp );
		//	}
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public uint ReadUInt32()
		{
			unsafe
			{
				uint result = 0;
				ReadBuffer( &result, 4 );
				return result;
			}

			//int newPosition = bitPosition + 32;
			//if( overflow || newPosition > endBitPosition )
			//{
			//	overflow = true;
			//	return 0;
			//}
			//uint value = BitWriter.ReadUInt32( data, 32, bitPosition );
			//bitPosition += 32;
			//return value;
		}

		public uint ReadUInt()
		{
			return ReadUInt32();
		}

		//public UInt32 ReadUInt32( int numberOfBits )
		//{
		//	int newPosition = bitPosition + numberOfBits;
		//	if( overflow || newPosition > endBitPosition )
		//	{
		//		overflow = true;
		//		return 0;
		//	}
		//	uint value = BitWriter.ReadUInt32( data, numberOfBits, bitPosition );
		//	bitPosition += numberOfBits;
		//	return value;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public long ReadInt64()
		{
			unsafe
			{
				long result = 0;
				ReadBuffer( &result, 8 );
				return result;
			}

			//int newPosition = bitPosition + 64;
			//if( overflow || newPosition > endBitPosition )
			//{
			//	overflow = true;
			//	return 0;
			//}
			//unchecked
			//{
			//	ulong value = ReadUInt64();
			//	long longValue = (long)value;
			//	return longValue;
			//}
		}

		public long ReadLong()
		{
			return ReadInt64();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ulong ReadUInt64()
		{
			unsafe
			{
				ulong result = 0;
				ReadBuffer( &result, 8 );
				return result;
			}

			//int newPosition = bitPosition + 64;
			//if( overflow || newPosition > endBitPosition )
			//{
			//	overflow = true;
			//	return 0;
			//}
			//ulong low = BitWriter.ReadUInt32( data, 32, bitPosition );
			//bitPosition += 32;
			//ulong high = BitWriter.ReadUInt32( data, 32, bitPosition );
			//bitPosition += 32;
			//ulong value = low + ( high << 32 );
			//return value;
		}

		public ulong ReadULong()
		{
			return ReadUInt64();
		}

		//public ulong ReadUInt64( int numberOfBits )
		//{
		//	int newPosition = bitPosition + numberOfBits;
		//	if( overflow || newPosition > endBitPosition )
		//	{
		//		overflow = true;
		//		return 0;
		//	}

		//	ulong value;
		//	if( numberOfBits <= 32 )
		//	{
		//		value = (ulong)BitWriter.ReadUInt32( data, numberOfBits, bitPosition );
		//	}
		//	else
		//	{
		//		value = BitWriter.ReadUInt32( data, 32, bitPosition );
		//		value |= BitWriter.ReadUInt32( data, numberOfBits - 32, bitPosition ) << 32;
		//	}
		//	bitPosition += numberOfBits;
		//	return value;
		//}

		//public long ReadInt64( int numberOfBits )
		//{
		//	int newPosition = bitPosition + numberOfBits;
		//	if( overflow || newPosition > endBitPosition )
		//	{
		//		overflow = true;
		//		return 0;
		//	}
		//	return (long)ReadUInt64( numberOfBits );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float ReadSingle()
		{
			unsafe
			{
				float result = 0;
				ReadBuffer( &result, 4 );
				return result;
			}

			//int newPosition = bitPosition + 32;
			//if( overflow || newPosition > endBitPosition )
			//{
			//	overflow = true;
			//	return 0;
			//}

			////read directly
			//if( ( bitPosition & 7 ) == 0 )
			//{
			//	//endianness is handled inside BitConverter.ToSingle
			//	float value = BitConverter.ToSingle( data, bitPosition >> 3 );
			//	bitPosition += 32;
			//	return value;
			//}

			//byte[] bytes = new byte[ 4 ];
			//ReadBuffer( bytes );
			//if( overflow )
			//	return 0;
			////endianness is handled inside BitConverter.ToSingle
			//return BitConverter.ToSingle( bytes, 0 );
		}

		public float ReadFloat()
		{
			return ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double ReadDouble()
		{
			unsafe
			{
				double result = 0;
				ReadBuffer( &result, 8 );
				return result;
			}

			//int newPosition = bitPosition + 64;
			//if( overflow || newPosition > endBitPosition )
			//{
			//	overflow = true;
			//	return 0;
			//}

			////read directly
			//if( ( bitPosition & 7 ) == 0 )
			//{
			//	double value = BitConverter.ToDouble( data, bitPosition >> 3 );
			//	bitPosition += 64;
			//	return value;
			//}

			//byte[] bytes = new byte[ 8 ];
			//ReadBuffer( bytes );
			//if( overflow )
			//	return 0;
			////endianness is handled inside BitConverter.ToSingle
			//return BitConverter.ToDouble( bytes, 0 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public decimal ReadDecimal()
		{
			unsafe
			{
				decimal result = 0;
				ReadBuffer( &result, sizeof( decimal ) );
				return result;
			}
		}

		/// <summary>
		/// Reads a UInt32 written using WriteVariableUInt32()
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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
		/// Reads a Int32 written using WriteVariableInt32()
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		public int ReadVariableInt()
		{
			return ReadVariableInt32();
		}

		/// <summary>
		/// Reads a UInt64 written using WriteVariableUInt64()
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		public ulong ReadVariableULong()
		{
			return ReadVariableUInt64();
		}

		/// <summary>
		/// Reads a Int64 written using WriteVariableInt64()
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public long ReadVariableInt64()
		{
			if( overflow )
				return 0;

			long num1 = 0;
			int num2 = 0;
			while( true )
			{
				if( num2 == 0x77 )
				{
					overflow = true;
					return 0;
				}

				byte num3 = ReadByte();
				if( overflow )
					return 0;

				num1 |= ( (long)num3 & 0x7f ) << num2;
				num2 += 7;
				if( ( num3 & 0x80 ) == 0 )
				{
					long sign = ( num1 << 63 ) >> 63;
					return sign ^ ( num1 >> 1 );
				}
			}
		}

		public long ReadVariableLong()
		{
			return ReadVariableInt64();
		}


		///// <summary>
		///// Reads a float written using WriteSignedSingle()
		///// </summary>
		//public float ReadSignedSingle( int numberOfBits )
		//{
		//	if( overflow || bitPosition + numberOfBits > endBitPosition )
		//	{
		//		overflow = true;
		//		return 0;
		//	}

		//	uint encodedVal = ReadUInt32( numberOfBits );
		//	int maxVal = ( 1 << numberOfBits ) - 1;
		//	return ( (float)( encodedVal + 1 ) / (float)( maxVal + 1 ) - 0.5f ) * 2.0f;
		//}

		///// <summary>
		///// Reads a float written using WriteUnitSingle()
		///// </summary>
		//public float ReadUnitSingle( int numberOfBits )
		//{
		//	if( overflow || bitPosition + numberOfBits > endBitPosition )
		//	{
		//		overflow = true;
		//		return 0;
		//	}

		//	uint encodedVal = ReadUInt32( numberOfBits );
		//	int maxVal = ( 1 << numberOfBits ) - 1;
		//	return (float)( encodedVal + 1 ) / (float)( maxVal + 1 );
		//}

		///// <summary>
		///// Reads a float written using WriteRangedSingle() using the same MIN and MAX values
		///// </summary>
		//public float ReadRangedSingle( float min, float max, int numberOfBits )
		//{
		//	if( overflow || bitPosition + numberOfBits > endBitPosition )
		//	{
		//		overflow = true;
		//		return 0;
		//	}
		//	float range = max - min;
		//	int maxVal = ( 1 << numberOfBits ) - 1;
		//	float encodedVal = (float)ReadUInt32( numberOfBits );
		//	float unit = encodedVal / (float)maxVal;
		//	return min + ( unit * range );
		//}

		//static int BitsToHoldUInt( uint value )
		//{
		//	int bits = 1;
		//	while( ( value >>= 1 ) != 0 )
		//		bits++;
		//	return bits;
		//}

		///// <summary>
		///// Reads an integer written using WriteRangedInteger() using the same min/max values
		///// </summary>
		//public int ReadRangedInteger( int min, int max )
		//{
		//	uint range = (uint)( max - min );
		//	int numBits = BitsToHoldUInt( range );
		//	if( overflow || bitPosition + numBits > endBitPosition )
		//	{
		//		overflow = true;
		//		return 0;
		//	}
		//	uint rvalue = ReadUInt32( numBits );
		//	return (int)( min + rvalue );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadVector2F( out Vector2F result )
		{
			result.X = ReadSingle();
			result.Y = ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2F ReadVector2F()
		{
			return new Vector2F( ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadRangeF( out RangeF result )
		{
			result.Minimum = ReadSingle();
			result.Maximum = ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RangeF ReadRangeF()
		{
			return new RangeF( ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadVector3F( out Vector3F result )
		{
			result.X = ReadSingle();
			result.Y = ReadSingle();
			result.Z = ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3F ReadVector3F()
		{
			return new Vector3F( ReadSingle(), ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadVector4F( out Vector4F result )
		{
			result.X = ReadSingle();
			result.Y = ReadSingle();
			result.Z = ReadSingle();
			result.W = ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector4F ReadVector4F()
		{
			return new Vector4F( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadBoundsF( out BoundsF result )
		{
			result.Minimum.X = ReadSingle();
			result.Minimum.Y = ReadSingle();
			result.Minimum.Z = ReadSingle();
			result.Maximum.X = ReadSingle();
			result.Maximum.Y = ReadSingle();
			result.Maximum.Z = ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BoundsF ReadBoundsF()
		{
			return new BoundsF( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadQuaternionF( out QuaternionF result )
		{
			result.X = ReadSingle();
			result.Y = ReadSingle();
			result.Z = ReadSingle();
			result.W = ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public QuaternionF ReadQuaternionF()
		{
			return new QuaternionF( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		//public QuaternionF ReadQuatertionF( int bitsPerElement )
		//{
		//	return new QuaternionF(
		//		ReadRangedSingle( -1, 1, bitsPerElement ),
		//		ReadRangedSingle( -1, 1, bitsPerElement ),
		//		ReadRangedSingle( -1, 1, bitsPerElement ),
		//		ReadRangedSingle( -1, 1, bitsPerElement ) );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadColorValue( out ColorValue result )
		{
			result.Red = ReadSingle();
			result.Green = ReadSingle();
			result.Blue = ReadSingle();
			result.Alpha = ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValue ReadColorValue()
		{
			return new ColorValue( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorByte ReadColorByte()
		{
			return new ColorByte( ReadUInt() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValuePowered ReadColorValuePowered()
		{
			return new ColorValuePowered( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadSphericalDirectionF( out SphericalDirectionF result )
		{
			result.Horizontal = ReadSingle();
			result.Vertical = ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SphericalDirectionF ReadSphericalDirectionF()
		{
			return new SphericalDirectionF( ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadVector2I( out Vector2I result )
		{
			result.X = ReadInt32();
			result.Y = ReadInt32();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2I ReadVector2I()
		{
			return new Vector2I( ReadInt32(), ReadInt32() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadVector3I( out Vector3I result )
		{
			result.X = ReadInt32();
			result.Y = ReadInt32();
			result.Z = ReadInt32();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3I ReadVector3I()
		{
			return new Vector3I( ReadInt32(), ReadInt32(), ReadInt32() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadVector4I( out Vector4I result )
		{
			result.X = ReadInt32();
			result.Y = ReadInt32();
			result.Z = ReadInt32();
			result.W = ReadInt32();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector4I ReadVector4I()
		{
			return new Vector4I( ReadInt32(), ReadInt32(), ReadInt32(), ReadInt32() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadRectangleF( out RectangleF result )
		{
			result.Left = ReadSingle();
			result.Top = ReadSingle();
			result.Right = ReadSingle();
			result.Bottom = ReadSingle();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RectangleF ReadRectangleF()
		{
			return new RectangleF( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadRectangleI( out RectangleI result )
		{
			result.Left = ReadInt32();
			result.Top = ReadInt32();
			result.Right = ReadInt32();
			result.Bottom = ReadInt32();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RectangleI ReadRectangleI()
		{
			return new RectangleI( ReadInt32(), ReadInt32(), ReadInt32(), ReadInt32() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public DegreeF ReadDegreeF()
		{
			return new DegreeF( ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RadianF ReadRadianF()
		{
			return new RadianF( ReadSingle() );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public string ReadString()
		{
			int length = ReadVariableInt32();
			if( length == -1 )
				return null;
			if( length == 0 )
				return string.Empty;

			int newPosition = currentPosition + length;
			if( overflow || newPosition > endPosition )
			{
				overflow = true;
				return string.Empty;
			}
			var result = Encoding.UTF8.GetString( data, currentPosition, length );
			currentPosition = newPosition;

			return result;
		}

		//old
		//[MethodImpl( (MethodImplOptions)512 )]
		//public string ReadString()
		//{
		//	int length = (int)ReadVariableUInt32();
		//	if( length == 0 )
		//		return string.Empty;

		//	int newPosition = currentPosition + length;
		//	if( overflow || newPosition > endPosition )
		//	{
		//		overflow = true;
		//		return string.Empty;
		//	}
		//	var result = Encoding.UTF8.GetString( data, currentPosition, length );
		//	currentPosition = newPosition;

		//	return result;
		//}

		///// <summary>
		///// Pads data with enough bits to reach a full byte. Decreases cpu usage for subsequent byte writes.
		///// </summary>
		//public void SkipPadBits()
		//{
		//	bitPosition = ( ( bitPosition + 7 ) / 8 ) * 8;
		//}

		///// <summary>
		///// Pads data with the specified number of bits.
		///// </summary>
		//public void SkipPadBits( int numberOfBits )
		//{
		//	bitPosition += numberOfBits;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadVector2( out Vector2 result )
		{
			result.X = ReadDouble();
			result.Y = ReadDouble();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2 ReadVector2()
		{
			return new Vector2( ReadDouble(), ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadRange( out Range result )
		{
			result.Minimum = ReadDouble();
			result.Maximum = ReadDouble();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Range ReadRange()
		{
			return new Range( ReadDouble(), ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RangeI ReadRangeI()
		{
			return new RangeI( ReadInt(), ReadInt() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadVector3( out Vector3 result )
		{
			result.X = ReadDouble();
			result.Y = ReadDouble();
			result.Z = ReadDouble();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 ReadVector3()
		{
			return new Vector3( ReadDouble(), ReadDouble(), ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadVector4( out Vector4 result )
		{
			result.X = ReadDouble();
			result.Y = ReadDouble();
			result.Z = ReadDouble();
			result.W = ReadDouble();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector4 ReadVector4()
		{
			return new Vector4( ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadBounds( out Bounds result )
		{
			result.Minimum.X = ReadDouble();
			result.Minimum.Y = ReadDouble();
			result.Minimum.Z = ReadDouble();
			result.Maximum.X = ReadDouble();
			result.Maximum.Y = ReadDouble();
			result.Maximum.Z = ReadDouble();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Bounds ReadBounds()
		{
			return new Bounds( ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadQuaternion( out Quaternion result )
		{
			result.X = ReadDouble();
			result.Y = ReadDouble();
			result.Z = ReadDouble();
			result.W = ReadDouble();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Quaternion ReadQuaternion()
		{
			return new Quaternion( ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadSphericalDirection( out SphericalDirection result )
		{
			result.Horizontal = ReadDouble();
			result.Vertical = ReadDouble();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SphericalDirection ReadSphericalDirection()
		{
			return new SphericalDirection( ReadDouble(), ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ReadRectangle( out Rectangle result )
		{
			result.Left = ReadDouble();
			result.Top = ReadDouble();
			result.Right = ReadDouble();
			result.Bottom = ReadDouble();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Rectangle ReadRectangle()
		{
			return new Rectangle( ReadDouble(), ReadDouble(), ReadDouble(), ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Degree ReadDegree()
		{
			return new Degree( ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Radian ReadRadian()
		{
			return new Radian( ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public AnglesF ReadAnglesF()
		{
			return new AnglesF( ReadSingle(), ReadSingle(), ReadSingle() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Angles ReadAngles()
		{
			return new Angles( ReadDouble(), ReadDouble(), ReadDouble() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public DateTime ReadDateTime()
		{
			return new DateTime( ReadInt64(), DateTimeKind.Utc );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2H ReadVector2H()
		{
			unsafe
			{
				Vector2H result;
				ReadBuffer( &result, sizeof( Vector2H ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3H ReadVector3H()
		{
			unsafe
			{
				Vector3H result;
				ReadBuffer( &result, sizeof( Vector3H ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector4H ReadVector4H()
		{
			unsafe
			{
				Vector4H result;
				ReadBuffer( &result, sizeof( Vector4H ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public QuaternionH ReadQuaternionH()
		{
			unsafe
			{
				QuaternionH result;
				ReadBuffer( &result, sizeof( QuaternionH ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public HalfType ReadHalf()
		{
			unsafe
			{
				HalfType result;
				ReadBuffer( &result, sizeof( HalfType ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix2F ReadMatrix2F()
		{
			unsafe
			{
				Matrix2F result;
				ReadBuffer( &result, sizeof( Matrix2F ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix2 ReadMatrix2()
		{
			unsafe
			{
				Matrix2 result;
				ReadBuffer( &result, sizeof( Matrix2 ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix3F ReadMatrix3F()
		{
			unsafe
			{
				Matrix3F result;
				ReadBuffer( &result, sizeof( Matrix3F ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix3 ReadMatrix3()
		{
			unsafe
			{
				Matrix3 result;
				ReadBuffer( &result, sizeof( Matrix3 ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4F ReadMatrix4F()
		{
			unsafe
			{
				Matrix4F result;
				ReadBuffer( &result, sizeof( Matrix4F ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4 ReadMatrix4()
		{
			unsafe
			{
				Matrix4 result;
				ReadBuffer( &result, sizeof( Matrix4 ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public PlaneF ReadPlaneF()
		{
			unsafe
			{
				PlaneF result;
				ReadBuffer( &result, sizeof( PlaneF ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Plane ReadPlane()
		{
			unsafe
			{
				Plane result;
				ReadBuffer( &result, sizeof( Plane ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform ReadTransform()
		{
			return new Transform( ReadVector3(), ReadQuaternion(), ReadVector3() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SphereF ReadSphereF()
		{
			unsafe
			{
				SphereF result;
				ReadBuffer( &result, sizeof( SphereF ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Sphere ReadSphere()
		{
			unsafe
			{
				Sphere result;
				ReadBuffer( &result, sizeof( Sphere ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public UIMeasureValueDouble ReadUIMeasureValueDouble()
		{
			unsafe
			{
				UIMeasureValueDouble result;
				ReadBuffer( &result, sizeof( UIMeasureValueDouble ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public UIMeasureValueVector2 ReadUIMeasureValueVector2()
		{
			unsafe
			{
				UIMeasureValueVector2 result;
				ReadBuffer( &result, sizeof( UIMeasureValueVector2 ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public UIMeasureValueRectangle ReadUIMeasureValueRectangle()
		{
			unsafe
			{
				UIMeasureValueRectangle result;
				ReadBuffer( &result, sizeof( UIMeasureValueRectangle ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RangeVector3F ReadRangeVector3F()
		{
			unsafe
			{
				RangeVector3F result;
				ReadBuffer( &result, sizeof( RangeVector3F ) );
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RangeColorValue ReadRangeColorValue()
		{
			unsafe
			{
				RangeColorValue result;
				ReadBuffer( &result, sizeof( RangeColorValue ) );
				return result;
			}
		}

		//public ObjectId ReadObjectId()
		//{
		//	ObjectId.TryParse( ReadString(), out var result );
		//	return result;
		//}



		//not work
		//object CreateNullable( Type nullableType, Type innerType, bool hasValue, object value )
		//{
		//	if( !hasValue )
		//	{
		//		// return default(Nullable<innerType>) -> Activator.CreateInstance(nullableType)
		//		return Activator.CreateInstance( nullableType );
		//	}

		//	// ensure value is of the inner type
		//	if( value != null && !innerType.IsInstanceOfType( value ) )
		//	{
		//		value = Convert.ChangeType( value, innerType );
		//	}

		//	// create Nullable<innerType> with the inner value
		//	return Activator.CreateInstance( nullableType, new object[] { value } );
		//}

		///////////////////////////////////////////////

		public class AllocationStatistics
		{
			public long BytesLimit;
			public long ObjectsLimit;

			public long BytesAllocated;
			public long ObjectsAllocated;

			//

			public AllocationStatistics( long maxBytes, long maxObjects )
			{
				BytesLimit = maxBytes;
				ObjectsLimit = maxObjects;
			}

			public bool IsExceeded()
			{
				return BytesAllocated > BytesLimit || ObjectsAllocated > ObjectsLimit;
			}

			public void AddAllocation( long bytes )
			{
				BytesAllocated += bytes;
				ObjectsAllocated++;

				if( IsExceeded() )
					throw new InvalidOperationException( "Allocation limit exceeded." );
			}
		}

		///////////////////////////////////////////////

		long GetTypeSizeForAllocationStatistics( Type type )
		{
			try
			{
				return Marshal.SizeOf( type );
			}
			catch { }
			return 8;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public object Read( Type typeToRead, AllocationStatistics allocationStatistics )
		{
			//!!!!slowly

			//simple types
			var simpleType = SimpleTypes.GetTypeItem( typeToRead );
			if( simpleType != null )
			{
				if( allocationStatistics != null && simpleType.Type.IsValueType )
					allocationStatistics.AddAllocation( GetTypeSizeForAllocationStatistics( simpleType.Type ) );

				var value = simpleType.ReadFunction( this );

				if( allocationStatistics != null )
				{
					var str = value as string;
					if( str != null )
						allocationStatistics.AddAllocation( 8 + sizeof( char ) * str.Length );
				}

				return value;
			}

			//array
			if( typeToRead.IsArray )
			{
				//!!!!slowly
				//arrays with simple types may be optimized. same as MetadataManager. and cache simple type item

				var count = ReadVariableInt();
				if( count >= 0 )
				{
					var elementType = typeToRead.GetElementType();
					var value = (object)Array.CreateInstance( elementType, count );
					var methodSetValue = value.GetType().GetMethod( "SetValue", new Type[] { typeof( object ), typeof( int ) } );

					allocationStatistics?.AddAllocation( GetTypeSizeForAllocationStatistics( elementType ) * count );

					for( int n = 0; n < count; n++ )
					{
						var itemValue = Read( elementType, allocationStatistics );
						methodSetValue.Invoke( value, new object[] { itemValue, n } );

						if( overflow )
							break;
					}

					return value;
				}
				else
					return null;
			}

			//containers
			{
				Type containerType = typeToRead;
				if( typeToRead.IsGenericType )
					containerType = typeToRead.GetGenericTypeDefinition();

				if( containerType == typeof( List<> ) ||
					containerType == typeof( ESet<> ) ||
					containerType == typeof( HashSet<> ) ||
					containerType == typeof( SortedSet<> ) ||
					containerType == typeof( Stack<> ) ||
					containerType == typeof( Queue<> ) )
				{
					var elementType = TypeUtility.GetGenericArgumentInBaseTypes( typeToRead, containerType, 0 );

					var count = ReadVariableInt();
					if( count >= 0 )
					{
						allocationStatistics?.AddAllocation( GetTypeSizeForAllocationStatistics( elementType ) * count );

						var value = typeToRead.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { count } );

						string addName;
						if( containerType == typeof( Stack<> ) )
							addName = "Push";
						else if( containerType == typeof( Queue<> ) )
							addName = "Enqueue";
						else
							addName = "Add";
						var methodAdd = value.GetType().GetMethod( addName, new Type[] { elementType } );

						for( int n = 0; n < count; n++ )
						{
							var elementObject = Read( elementType, allocationStatistics );
							methodAdd.Invoke( value, new object[] { elementObject } );

							if( overflow )
								break;
						}

						return value;
					}
					else
						return null;
				}

				if( containerType == typeof( Dictionary<,> ) ||
					containerType == typeof( EDictionary<,> ) ||
					containerType == typeof( SortedList<,> ) )
				{
					var elementTypes = TypeUtility.GetGenericArgumentsInBaseTypes( typeToRead, containerType );
					var elementTypeKey = elementTypes[ 0 ];
					var elementTypeValue = elementTypes[ 1 ];

					var count = ReadVariableInt();
					if( count >= 0 )
					{
						allocationStatistics?.AddAllocation( ( GetTypeSizeForAllocationStatistics( elementTypeKey ) + GetTypeSizeForAllocationStatistics( elementTypeValue ) ) * count );

						var value = typeToRead.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, new object[] { count } );

						var methodAdd = value.GetType().GetMethod( "Add", new Type[] { elementTypeKey, elementTypeValue } );

						for( int n = 0; n < count; n++ )
						{
							var elementKey = Read( elementTypeKey, allocationStatistics );
							var elementValue = Read( elementTypeValue, allocationStatistics );
							methodAdd.Invoke( value, new object[] { elementKey, elementValue } );

							if( overflow )
								break;
						}

						return value;
					}
					else
						return null;
				}
			}

			//tuples
			if( typeToRead.IsGenericType )
			{
				var genericType = typeToRead.GetGenericTypeDefinition();

				if( genericType == typeof( ValueTuple<> ) ||
					genericType == typeof( ValueTuple<,> ) ||
					genericType == typeof( ValueTuple<,,> ) ||
					genericType == typeof( ValueTuple<,,,> ) ||
					genericType == typeof( ValueTuple<,,,,> ) ||
					genericType == typeof( ValueTuple<,,,,,> ) ||
					genericType == typeof( ValueTuple<,,,,,,> ) ||
					genericType == typeof( ValueTuple<,,,,,,,> ) )
				{
					var fields = typeToRead.GetFields( BindingFlags.Public | BindingFlags.Instance );
					if( fields != null && fields.Length > 0 )
					{
						var value = typeToRead.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );

						foreach( var field in fields )
						{
							var fieldValue = Read( field.FieldType, allocationStatistics );
							field.SetValue( value, fieldValue );
						}

						return value;
					}
				}

				if( genericType == typeof( Tuple<> ) ||
					genericType == typeof( Tuple<,> ) ||
					genericType == typeof( Tuple<,,> ) ||
					genericType == typeof( Tuple<,,,> ) ||
					genericType == typeof( Tuple<,,,,> ) ||
					genericType == typeof( Tuple<,,,,,> ) ||
					genericType == typeof( Tuple<,,,,,,> ) ||
					genericType == typeof( Tuple<,,,,,,,> ) )
				{
					var properties = typeToRead.GetProperties( BindingFlags.Public | BindingFlags.Instance );
					if( properties != null && properties.Length > 0 )
					{
						var constructor = typeToRead.GetConstructors().FirstOrDefault();
						if( constructor == null )
							throw new InvalidOperationException( "No constructor found for the Tuple type." );

						allocationStatistics?.AddAllocation( GetTypeSizeForAllocationStatistics( typeToRead ) );

						var arguments = new object[ properties.Length ];
						for( int n = 0; n < properties.Length; n++ )
							arguments[ n ] = Read( properties[ n ].PropertyType, allocationStatistics );

						var value = Activator.CreateInstance( typeToRead, arguments );

						return value;
					}
				}
			}

			//custom structure
			{
				//!!!!slowly cache properties info per type

				if( ArrayDataWriter.TypeToWriteCustomStructureIsSupported( typeToRead, 0, out var properties ) )
				{

					//!!!!slowly. make cache of properties

					var value = ReadCustomStructure( typeToRead, allocationStatistics );

					return value;


					//if( answer.ResultCustomStructureValues != null )
					//{
					//	try
					//	{
					//		var returnParameter = method.ReturnParameter;

					//		var valueType = typeof( T );
					//		var value = (T)valueType.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );

					//		if( returnParameter.CustomTypeProperties.Length != answer.ResultCustomStructureValues.Length )
					//			return new CallMethodResult<T> { Error = "Invalid answer. returnParameter.CustomTypeProperties.Length != answer.ResultCustomStructureValues.Length." };

					//		for( int n = 0; n < returnParameter.CustomTypeProperties.Length; n++ )
					//		{
					//			var p = returnParameter.CustomTypeProperties[ n ];
					//			var resultCustomStructureValue = answer.ResultCustomStructureValues[ n ];

					//			if( p.FieldType != null )
					//			{
					//				var field = valueType.GetField( p.Name );
					//				if( field == null )
					//					return new CallMethodResult<T> { Error = $"No field with name \"{p.Name}\"." };
					//				field.SetValue( value, resultCustomStructureValue );
					//			}
					//			else if( p.PropertyType != null )
					//			{
					//				var property = valueType.GetProperty( p.Name );
					//				if( property == null )
					//					return new CallMethodResult<T> { Error = $"No property with name \"{p.Name}\"." };
					//				property.SetValue( value, resultCustomStructureValue );
					//			}
					//		}

					//		return new CallMethodResult<T> { Value = value };
					//	}
					//	catch( Exception e )
					//	{
					//		return new CallMethodResult<T> { Error = e.Message };
					//	}
					//}
					//else

				}
			}



			//not work
			////nullable
			//if( typeToRead.IsGenericType && typeToRead.GetGenericTypeDefinition() == typeof( Nullable<> ) )
			//{
			//	//var underlyingType = Nullable.GetUnderlyingType( typeToRead )!;

			//	//var hasValue = ReadBoolean();
			//	//if( !hasValue )
			//	//{
			//	//	// Return default(Nullable<T>)
			//	//	return Activator.CreateInstance( typeToRead ); // HasValue == false
			//	//}

			//	////// Read and ensure exact underlying type
			//	//object value = Read( underlyingType );
			//	////if( value != null && value.GetType() != underlyingType )
			//	////{
			//	////	value = Convert.ChangeType( value, underlyingType, CultureInfo.InvariantCulture );
			//	////}

			//	//// Construct Nullable<T> with a value: new Nullable<T>((T)value)
			//	//return Activator.CreateInstance( typeToRead, value );


			//	object innerValue;

			//	var hasValue = ReadBoolean();
			//	if( !hasValue )
			//	{
			//		if( typeToRead.IsValueType )
			//			innerValue = Activator.CreateInstance( typeToRead );
			//		else
			//			innerValue = null;
			//	}
			//	else
			//	{
			//		var underlyingType = Nullable.GetUnderlyingType( typeToRead );
			//		innerValue = Read( underlyingType );
			//	}

			//	//convert to Nullable<> and return. but unboxing make just value
			//	return Activator.CreateInstance( typeToRead, new object[] { innerValue } );
			//	//return Activator.CreateInstance( typeToRead, innerValue );
			//}


			throw new NotSupportedException();
		}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public T Read<T>()
		//{
		//	return (T)Read( typeof( T ) );
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		object ReadCustomStructure( Type typeToRead, AllocationStatistics allocationStatistics )
		{
			var value = typeToRead.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );

			allocationStatistics?.AddAllocation( GetTypeSizeForAllocationStatistics( typeToRead ) );

			var fields = typeToRead.GetFields( BindingFlags.Public | BindingFlags.Instance );
			foreach( var field in fields )
			{
				//!!!!more checks

				var fieldValue = Read( field.FieldType, allocationStatistics );
				field.SetValue( value, fieldValue );
			}

			var properties = typeToRead.GetProperties( BindingFlags.Public | BindingFlags.Instance );
			foreach( var property in properties )
			{
				//!!!!more checks

				var propertyValue = Read( property.PropertyType, allocationStatistics );
				property.SetValue( value, propertyValue );
			}

			return value;
		}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public T ReadCustomStructure<T>()
		//{
		//	return (T)Read( typeof( T ) );
		//}

		//[MethodImpl( (MethodImplOptions)512 )]
		//public object[] ReadCustomStructureProperties( ArrayDataWriter.TypeToWriteCustomStructureProperty[] properties )
		//{
		//	var values = new object[ properties.Length ];

		//	for( int n = 0; n < properties.Length; n++ )
		//	{
		//		var p = properties[ n ];
		//		if( p.FieldType != null )
		//			values[ n ] = Read( p.FieldType );
		//		else if( p.PropertyType != null )
		//			values[ n ] = Read( p.PropertyType );
		//	}

		//	return values;
		//}
	}
}
