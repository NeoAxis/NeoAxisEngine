// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
// TODO: Add original author copyright.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.
	/// </summary>
	public class FastRandom
	{
		const double DOUBLE_UNIT_INT = 1.0 / ( (double)int.MaxValue + 1.0 );
		const double DOUBLE_UNIT_UINT = 1.0 / ( (double)uint.MaxValue + 1.0 );
		const float FLOAT_UNIT_INT = 1.0f / ( (float)int.MaxValue + 1.0f );

		const uint initY = 842502087, initZ = 3579807591, initW = 273326509;
		uint x, y, z, w;

		uint bitBuffer;
		uint bitMask = 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="FastRandom"/> class, 
		/// using a time-dependent default seed value.
		/// </summary>
		public FastRandom()
		{
			unchecked
			{
				Initialize( Math.Abs( Environment.TickCount * 347865 ) );
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FastRandom"/> class, 
		/// using the specified seed value.
		/// </summary>
		/// <param name="seed">
		/// A number used to calculate a starting value for the pseudo-random number
		/// sequence. If a negative number is specified, the absolute value of the number
		/// is used.
		/// </param>
		public FastRandom( int seed, bool makeBetterSeed = false )
		{
			Random betterSeed = null;
			if( makeBetterSeed )
			{
				betterSeed = new Random( seed );
				seed = betterSeed.Next();
			}

			Initialize( seed );

			if( makeBetterSeed )
			{
				Next( betterSeed.Next() );
				Next( betterSeed.Next() );
			}
		}

		void Initialize( int seed )
		{
			x = (uint)seed;
			y = initY;
			z = initZ;
			w = initW;
		}

		/// <summary>
		/// Returns a nonnegative random number.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer greater than or equal to zero and less than System.Int32.MaxValue.
		/// </returns>
		public int NextInteger()
		{
			uint t = ( x ^ ( x << 11 ) );
			x = y; y = z; z = w;
			w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) );

			// Handle the special case where the value int.MaxValue is generated. This is outside of 
			// the range of permitted values, so we therefore call Next() to try again.
			uint rtn = w & 0x7FFFFFFF;
			if( rtn == 0x7FFFFFFF )
				return NextInteger();
			return (int)rtn;
		}

		/// <summary>
		/// Returns a nonnegative random number less or equal than the specified maximum.
		/// </summary>
		/// <param name="maxValue">
		/// The inclusive upper bound of the random number to be generated. 
		/// maxValue must be greater than or equal to zero.
		/// </param>
		/// <returns>
		/// A 32-bit signed integer greater than or equal to zero, and less or equal than maxValue. The range of return values includes zero and maxValue.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException" >maxValue is less than zero.</exception>
		public int Next( int maxValue )
		{
			if( maxValue < 0 )
				throw new ArgumentOutOfRangeException( "maxValue", maxValue, "maxValue is less than zero." );

			maxValue++;

			uint t = ( x ^ ( x << 11 ) );
			x = y; y = z; z = w;

			// The explicit int cast before the first multiplication gives better performance.
			// See comments in NextDouble.
			return (int)( ( DOUBLE_UNIT_INT * (int)( 0x7FFFFFFF & ( w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) ) ) ) ) * maxValue );
		}

		/// <summary>
		/// Returns a random number within a specified range.
		/// </summary>
		/// <param name="minValue">The inclusive lower bound of the random number returned.</param>
		/// <param name="maxValue">
		/// The inclusive upper bound of the random number returned. maxValue must be
		/// greater than or equal to minValue.
		/// </param>
		/// <returns>
		/// A 32-bit signed integer greater than or equal to minValue and less or equal than maxValue;
		/// that is, the range of return values includes minValue and maxValue.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException" >minValue is greater than maxValue.</exception>
		public int Next( int minValue, int maxValue )
		{
			if( minValue > maxValue )
				throw new ArgumentOutOfRangeException( "maxValue", maxValue, "minValue is greater than maxValue." );

			maxValue++;

			uint t = ( x ^ ( x << 11 ) );
			x = y; y = z; z = w;

			// The explicit int cast before the first multiplication gives better performance.
			// See comments in NextDouble.
			int range = maxValue - minValue;
			if( range < 0 )
			{   // If range is <0 then an overflow has occured and must resort to using long integer arithmetic instead (slower).
				// We also must use all 32 bits of precision, instead of the normal 31, which again is slower.	
				return minValue + (int)( ( DOUBLE_UNIT_UINT * (double)( w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) ) ) ) * (double)( (long)maxValue - (long)minValue ) );
			}

			// 31 bits of precision will suffice if range<=int.MaxValue. This allows us to cast to an int and gain
			// a little more performance.
			return minValue + (int)( ( DOUBLE_UNIT_INT * (double)(int)( 0x7FFFFFFF & ( w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) ) ) ) ) * (double)range );
		}

		/// <summary>
		/// Returns a random number between 0.0 and 1.0.
		/// </summary>
		/// <returns>A floating point number greater than or equal to 0.0, and less than 1.0.</returns>
		public double NextDouble()
		{
			uint t = ( x ^ ( x << 11 ) );
			x = y; y = z; z = w;

			// Here we can gain a 2x speed improvement by generating a value that can be cast to 
			// an int instead of the more easily available uint. If we then explicitly cast to an 
			// int the compiler will then cast the int to a double to perform the multiplication, 
			// this final cast is a lot faster than casting from a uint to a double. The extra cast
			// to an int is very fast (the allocated bits remain the same) and so the overall effect 
			// of the extra cast is a significant performance improvement.
			//
			// Also note that the loss of one bit of precision is equivalent to what occurs within 
			// System.Random.
			return ( DOUBLE_UNIT_INT * (int)( 0x7FFFFFFF & ( w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) ) ) ) );
		}

		/// <summary>
		/// Returns a random number between 0.0 and 1.0.
		/// </summary>
		/// <returns>A floating point number greater than or equal to 0.0, and less than 1.0.</returns>
		public float NextFloat()
		{
			uint t = ( x ^ ( x << 11 ) );
			x = y; y = z; z = w;

			// Here we can gain a 2x speed improvement by generating a value that can be cast to 
			// an int instead of the more easily available uint. If we then explicitly cast to an 
			// int the compiler will then cast the int to a double to perform the multiplication, 
			// this final cast is a lot faster than casting from a uint to a double. The extra cast
			// to an int is very fast (the allocated bits remain the same) and so the overall effect 
			// of the extra cast is a significant performance improvement.
			//
			// Also note that the loss of one bit of precision is equivalent to what occurs within 
			// System.Random.
			return ( FLOAT_UNIT_INT * (int)( 0x7FFFFFFF & ( w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) ) ) ) );
		}

		/// <summary>
		/// Fills the elements of a specified array of bytes with random numbers.
		/// </summary>
		/// <param name="buffer">An array of bytes to contain random numbers.</param>
		public void NextBytes( byte[] buffer )
		{
			// Fill up the bulk of the buffer in chunks of 4 bytes at a time.
			uint x = this.x, y = this.y, z = this.z, w = this.w;
			int i = 0;
			uint t;
			for( int bound = buffer.Length - 3; i < bound; )
			{
				// Generate 4 bytes. 
				// Increased performance is achieved by generating 4 random bytes per loop.
				// Also note that no mask needs to be applied to zero out the higher order bytes before
				// casting because the cast ignores thos bytes. Thanks to Stefan Troschütz for pointing this out.
				t = ( x ^ ( x << 11 ) );
				x = y; y = z; z = w;
				w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) );

				buffer[ i++ ] = (byte)w;
				buffer[ i++ ] = (byte)( w >> 8 );
				buffer[ i++ ] = (byte)( w >> 16 );
				buffer[ i++ ] = (byte)( w >> 24 );
			}

			// Fill up any remaining bytes in the buffer.
			if( i < buffer.Length )
			{
				// Generate 4 bytes.
				t = ( x ^ ( x << 11 ) );
				x = y; y = z; z = w;
				w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) );

				buffer[ i++ ] = (byte)w;
				if( i < buffer.Length )
				{
					buffer[ i++ ] = (byte)( w >> 8 );
					if( i < buffer.Length )
					{
						buffer[ i++ ] = (byte)( w >> 16 );
						if( i < buffer.Length )
						{
							buffer[ i ] = (byte)( w >> 24 );
						}
					}
				}
			}
			this.x = x; this.y = y; this.z = z; this.w = w;
		}

		///// <summary>
		///// A version of NextBytes that uses a pointer to set 4 bytes of the byte buffer in one operation
		///// thus providing a nice speedup. The loop is also partially unrolled to allow out-of-order-execution,
		///// this results in about a x2 speedup on an AMD Athlon. Thus performance may vary wildly on different CPUs
		///// depending on the number of execution units available.
		///// 
		///// Another significant speedup is obtained by setting the 4 bytes by indexing pDWord (e.g. pDWord[i++]=w)
		///// instead of adjusting it dereferencing it (e.g. *pDWord++=w).
		///// 
		///// Note that this routine requires the unsafe compilation flag to be specified and so is commented out by default.
		///// </summary>
		///// <param name="buffer"></param>
		//public unsafe void NextBytesUnsafe( byte[] buffer )
		//{
		//   if( buffer.Length % 8 != 0 )
		//      throw new ArgumentException( "Buffer length must be divisible by 8", "buffer" );

		//   uint x = this.x, y = this.y, z = this.z, w = this.w;

		//   fixed( byte* pByte0 = buffer )
		//   {
		//      uint* pDWord = (uint*)pByte0;
		//      for( int i = 0, len = buffer.Length >> 2; i < len; i += 2 )
		//      {
		//         uint t = ( x ^ ( x << 11 ) );
		//         x = y; y = z; z = w;
		//         pDWord[ i ] = w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) );

		//         t = ( x ^ ( x << 11 ) );
		//         x = y; y = z; z = w;
		//         pDWord[ i + 1 ] = w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) );
		//      }
		//   }

		//   this.x = x; this.y = y; this.z = z; this.w = w;
		//}

		///// <summary>
		///// Generates a uint. Values returned are over the full range of a uint, 
		///// uint.MinValue to uint.MaxValue, inclusive.
		///// 
		///// This is the fastest method for generating a single random number because the underlying
		///// random number generator algorithm generates 32 random bits that can be cast directly to 
		///// a uint.
		///// </summary>
		///// <returns></returns>
		//public uint NextUInt()
		//{
		//   uint t = ( x ^ ( x << 11 ) );
		//   x = y; y = z; z = w;
		//   return ( w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) ) );
		//}

		///// <summary>
		///// Returns a random int over the range 0 to int.MaxValue, inclusive. 
		///// This method differs from Next() only in that the range is 0 to int.MaxValue
		///// and not 0 to int.MaxValue-1.
		///// 
		///// The slight difference in range means this method is slightly faster than Next()
		///// but is not functionally equivalent to System.Random.Next().
		///// </summary>
		///// <returns></returns>
		//public int NextInt()
		//{
		//   uint t = ( x ^ ( x << 11 ) );
		//   x = y; y = z; z = w;
		//   return (int)( 0x7FFFFFFF & ( w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) ) ) );
		//}

		/// <summary>
		/// Returns a single random bit.
		/// </summary>
		/// <returns></returns>
		public bool NextBoolean()
		{
			if( bitMask == 1 )
			{
				// Generate 32 more bits.
				uint t = ( x ^ ( x << 11 ) );
				x = y; y = z; z = w;
				bitBuffer = w = ( w ^ ( w >> 19 ) ) ^ ( t ^ ( t >> 8 ) );

				// Reset the bitMask that tells us which bit to read next.
				bitMask = 0x80000000;
				return ( bitBuffer & bitMask ) == 0;
			}

			return ( bitBuffer & ( bitMask >>= 1 ) ) == 0;
		}

		///// <summary>
		///// Returns a random number between -0.5 and 0.5.
		///// </summary>
		///// <returns>A floating point number greater than or equal to -0.5, and less than 0.5.</returns>
		//public double NextDoubleCenter()
		//{
		//	return NextDouble() - .5;
		//}

		///// <summary>
		///// Returns a random number between -0.5 and 0.5.
		///// </summary>
		///// <returns>A floating point number greater than or equal to -0.5, and less than 0.5.</returns>
		//public float NextFloatCenter()
		//{
		//	return NextFloat() - .5f;
		//}

		/// <summary>
		/// Returns a random number between min and max values.
		/// </summary>
		/// <returns>A floating point number greater than to minValue, and less than to maxValue.</returns>
		public double Next( double minValue, double maxValue )
		{
			return minValue + NextDouble() * ( maxValue - minValue );
		}

		/// <summary>
		/// Returns a random number between zero and max value.
		/// </summary>
		/// <returns>A floating point number greater than to zero, and less than to maxValue.</returns>
		public double Next( double maxValue )
		{
			return NextDouble() * maxValue;
		}

		/// <summary>
		/// Returns a random number between min and max values.
		/// </summary>
		/// <returns>A floating point number greater than to minValue, and less than to maxValue.</returns>
		public float Next( float minValue, float maxValue )
		{
			return minValue + NextFloat() * ( maxValue - minValue );
		}

		/// <summary>
		/// Returns a random number between zero and max value.
		/// </summary>
		/// <returns>A floating point number greater than to zero, and less than to maxValue.</returns>
		public float Next( float maxValue )
		{
			return NextFloat() * maxValue;
		}

		/////////////////////////////////////////

		static FastRandom _static = new FastRandom();

		public static double Generate( double minValue, double maxValue )
		{
			lock( _static )
				return _static.Next( minValue, maxValue );
		}

		//public static double Generate( double maxValue )
		//{
		//	lock( _static )
		//		return _static.Next( maxValue );
		//}

		public static float Generate( float minValue, float maxValue )
		{
			lock( _static )
				return _static.Next( minValue, maxValue );
		}

		//public static float Generate( float maxValue )
		//{
		//	lock( _static )
		//		return _static.Next( maxValue );
		//}

		public static int Generate( int minValue, int maxValue )
		{
			lock( _static )
				return _static.Next( minValue, maxValue );
		}

		//public static int Generate( int maxValue )
		//{
		//	lock( _static )
		//		return _static.Next( maxValue );
		//}

		public static double GenerateDouble()
		{
			lock( _static )
				return _static.NextDouble();
		}

		public static float GenerateFloat()
		{
			lock( _static )
				return _static.NextFloat();
		}

		public static int GenerateInteger()
		{
			lock( _static )
				return _static.NextInteger();
		}

		public static bool GenerateBoolean()
		{
			lock( _static )
				return _static.NextBoolean();
		}

		public static void Generate( byte[] buffer )
		{
			lock( _static )
				_static.NextBytes( buffer );
		}

		public static Vector4 Generate( Vector4 minValue, Vector4 maxValue )
		{
			lock( _static )
				return new Vector4(
					_static.Next( minValue.X, maxValue.X ),
					_static.Next( minValue.Y, maxValue.Y ),
					_static.Next( minValue.Z, maxValue.Z ),
					_static.Next( minValue.W, maxValue.W ) );
		}

		public static Vector3 Generate( Vector3 minValue, Vector3 maxValue )
		{
			lock( _static )
				return new Vector3(
					_static.Next( minValue.X, maxValue.X ),
					_static.Next( minValue.Y, maxValue.Y ),
					_static.Next( minValue.Z, maxValue.Z ) );
		}

		public static Vector2 Generate( Vector2 minValue, Vector2 maxValue )
		{
			lock( _static )
				return new Vector2(
					_static.Next( minValue.X, maxValue.X ),
					_static.Next( minValue.Y, maxValue.Y ) );
		}

		public static Vector4F Generate( Vector4F minValue, Vector4F maxValue )
		{
			lock( _static )
				return new Vector4F(
					_static.Next( minValue.X, maxValue.X ),
					_static.Next( minValue.Y, maxValue.Y ),
					_static.Next( minValue.Z, maxValue.Z ),
					_static.Next( minValue.W, maxValue.W ) );
		}

		public static Vector3F Generate( Vector3F minValue, Vector3F maxValue )
		{
			lock( _static )
				return new Vector3F(
					_static.Next( minValue.X, maxValue.X ),
					_static.Next( minValue.Y, maxValue.Y ),
					_static.Next( minValue.Z, maxValue.Z ) );
		}

		public static Vector2F Generate( Vector2F minValue, Vector2F maxValue )
		{
			lock( _static )
				return new Vector2F(
					_static.Next( minValue.X, maxValue.X ),
					_static.Next( minValue.Y, maxValue.Y ) );
		}

		public static Vector4I Generate( Vector4I minValue, Vector4I maxValue )
		{
			lock( _static )
				return new Vector4I(
					_static.Next( minValue.X, maxValue.X ),
					_static.Next( minValue.Y, maxValue.Y ),
					_static.Next( minValue.Z, maxValue.Z ),
					_static.Next( minValue.W, maxValue.W ) );
		}

		public static Vector3I Generate( Vector3I minValue, Vector3I maxValue )
		{
			lock( _static )
				return new Vector3I(
					_static.Next( minValue.X, maxValue.X ),
					_static.Next( minValue.Y, maxValue.Y ),
					_static.Next( minValue.Z, maxValue.Z ) );
		}

		public static Vector2I Generate( Vector2I minValue, Vector2I maxValue )
		{
			lock( _static )
				return new Vector2I(
					_static.Next( minValue.X, maxValue.X ),
					_static.Next( minValue.Y, maxValue.Y ) );
		}

		public static ColorValue Generate( ColorValue minValue, ColorValue maxValue )
		{
			lock( _static )
				return new ColorValue(
					_static.Next( minValue.Red, maxValue.Red ),
					_static.Next( minValue.Green, maxValue.Green ),
					_static.Next( minValue.Blue, maxValue.Blue ),
					_static.Next( minValue.Alpha, maxValue.Alpha ) );
		}

		public static Angles Generate( Angles minValue, Angles maxValue )
		{
			lock( _static )
				return new Angles(
					_static.Next( minValue.Roll, maxValue.Roll ),
					_static.Next( minValue.Pitch, maxValue.Pitch ),
					_static.Next( minValue.Yaw, maxValue.Yaw ) );
		}

		public static AnglesF Generate( AnglesF minValue, AnglesF maxValue )
		{
			lock( _static )
				return new AnglesF(
					_static.Next( minValue.Roll, maxValue.Roll ),
					_static.Next( minValue.Pitch, maxValue.Pitch ),
					_static.Next( minValue.Yaw, maxValue.Yaw ) );
		}

		public static Radian Generate( Radian minValue, Radian maxValue )
		{
			lock( _static )
				return _static.Next( minValue, maxValue );
		}

		public static RadianF Generate( RadianF minValue, RadianF maxValue )
		{
			lock( _static )
				return _static.Next( minValue, maxValue );
		}

		public static Degree Generate( Degree minValue, Degree maxValue )
		{
			lock( _static )
				return _static.Next( minValue, maxValue );
		}

		public static DegreeF Generate( DegreeF minValue, DegreeF maxValue )
		{
			lock( _static )
				return _static.Next( minValue, maxValue );
		}
	}
}
