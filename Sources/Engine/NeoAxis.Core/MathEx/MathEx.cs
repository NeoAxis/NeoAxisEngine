// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;

namespace NeoAxis
{
	/// <summary>
	/// Expanded set of basic mathematical operations.
	/// </summary>
	public static class MathEx
	{
		public const float Epsilon = 1e-6f;
		public const float PI = 3.14159265358979323846f;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static float DegreeToRadian( float v )
		{
			return v * ( PI / 180.0f );
		}

		public static float RadianToDegree( float v )
		{
			return v * ( 180.0f / PI );
		}

		public static float InvSqrt( float v )
		{
			return 1.0f / (float)Math.Sqrt( v );
		}

		public static float Sqrt( float v )
		{
			return (float)Math.Sqrt( v );
		}

		public static float Sin( float v )
		{
			return (float)Math.Sin( v );
		}

		public static float Sinh( float v )
		{
			return (float)Math.Sinh( v );
		}

		public static float Cos( float v )
		{
			return (float)Math.Cos( v );
		}

		public static float Cosh( float v )
		{
			return (float)Math.Cosh( v );
		}

		public static float Tan( float v )
		{
			return (float)Math.Tan( v );
		}

		public static float Tanh( float v )
		{
			return (float)Math.Tanh( v );
		}

		public static float Asin( float v )
		{
			return (float)Math.Asin( v );
		}

		public static float Acos( float v )
		{
			return (float)Math.Acos( v );
		}

		public static float Atan( float v )
		{
			return (float)Math.Atan( v );
		}

		public static float Atan2( float y, float x )
		{
			return (float)Math.Atan2( y, x );
		}

		public static float Pow( float x, float y )
		{
			return (float)Math.Pow( x, y );
		}

		public static float Exp( float v )
		{
			return (float)Math.Exp( v );
		}

		public static void Swap( ref float v1, ref float v2 )
		{
			float c = v1;
			v1 = v2;
			v2 = c;
		}

		public static float Floor( float v )
		{
			return (float)Math.Floor( v );
		}

		public static float Log( float v )
		{
			return (float)Math.Log( v );
		}

		public static float Log( float v, float newBase )
		{
			return (float)Math.Log( v, newBase );
		}

		public static float Log10( float v )
		{
			return (float)Math.Log10( v );
		}

		public static float Round( float v )
		{
			return (float)Math.Round( v );
		}

		public static float RadianNormalize360( float angle )
		{
			if( ( angle >= ( PI * 2.0f ) ) || ( angle < 0.0f ) )
				angle -= Floor( angle / ( PI * 2.0f ) ) * ( PI * 2.0f );
			return angle;
		}

		public static float RadianNormalize180( float angle )
		{
			angle = RadianNormalize360( angle );
			if( angle > PI )
				angle -= ( PI * 2.0f );
			return angle;
		}

		//public static float RadiansDelta( float angle1, float angle2 )
		//{
		//	return RadiansNormalize180( angle1 - angle2 );
		//}

		public static double DegreeToRadian( double v )
		{
			return v * ( Math.PI / 180.0 );
		}

		public static double RadianToDegree( double v )
		{
			return v * ( 180.0 / Math.PI );
		}

		public static void Swap( ref double v1, ref double v2 )
		{
			double c = v1;
			v1 = v2;
			v2 = c;
		}

		public static double RadianNormalize360( double angle )
		{
			if( ( angle >= ( Math.PI * 2.0 ) ) || ( angle < 0.0 ) )
				angle -= Math.Floor( angle / ( Math.PI * 2.0 ) ) * ( Math.PI * 2.0 );
			return angle;
		}

		public static double RadianNormalize180( double angle )
		{
			angle = RadianNormalize360( angle );
			if( angle > Math.PI )
				angle -= ( Math.PI * 2.0 );
			return angle;
		}

		//public static double RadiansDelta( double angle1, double angle2 )
		//{
		//	return RadiansNormalize180( angle1 - angle2 );
		//}

		public static void Clamp( ref byte value, byte min, byte max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		public static void Clamp( ref int value, int min, int max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		public static void Clamp( ref float value, float min, float max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		public static void Clamp( ref double value, double min, double max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		public static void Clamp( ref RadianF value, RadianF min, RadianF max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		public static void Clamp( ref DegreeF value, DegreeF min, DegreeF max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		public static void Clamp( ref Radian value, Radian min, Radian max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		public static void Clamp( ref Degree value, Degree min, Degree max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		public static byte Clamp( byte value, byte min, byte max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		public static int Clamp( int value, int min, int max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		public static float Clamp( float value, float min, float max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		[ShaderGenerationFunction( "clamp({value}, {min}, {max})" )]
		public static double Clamp( double value, double min, double max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		public static RadianF Clamp( RadianF value, RadianF min, RadianF max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		public static DegreeF Clamp( DegreeF value, DegreeF min, DegreeF max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		public static Radian Clamp( Radian value, Radian min, Radian max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		public static Degree Clamp( Degree value, Degree min, Degree max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		public static void Saturate( ref float v )
		{
			if( v < 0 )
				v = 0;
			if( v > 1 )
				v = 1;
		}

		public static void Saturate( ref double v )
		{
			if( v < 0 )
				v = 0;
			if( v > 1 )
				v = 1;
		}

		public static void Saturate( ref decimal v )
		{
			if( v < 0 )
				v = 0;
			if( v > 1 )
				v = 1;
		}

		public static float Saturate( float v )
		{
			if( v < 0 )
				return 0;
			if( v > 1 )
				return 1;
			return v;
		}

		[ShaderGenerationFunction( "saturate({v})" )]
		public static double Saturate( double v )
		{
			if( v < 0 )
				return 0;
			if( v > 1 )
				return 1;
			return v;
		}

		public static decimal Saturate( decimal v )
		{
			if( v < 0 )
				return 0;
			if( v > 1 )
				return 1;
			return v;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[ShaderGenerationFunction( "{v1} + {v2}" )]
		public static double Add( double v1, double v2 )
		{
			return v1 + v2;
		}

		[ShaderGenerationFunction( "{v1} - {v2}" )]
		public static double Subtract( double v1, double v2 )
		{
			return v1 - v2;
		}

		[ShaderGenerationFunction( "{v1} * {v2}" )]
		public static double Multiply( double v1, double v2 )
		{
			return v1 * v2;
		}

		[ShaderGenerationFunction( "{v1} / {v2}" )]
		public static double Divide( double v1, double v2 )
		{
			return v1 / v2;
		}

		[ShaderGenerationFunction( "{v1} % {v2}" )]
		public static double Modulus( double v1, double v2 )
		{
			return v1 % v2;
		}

		[ShaderGenerationFunction( "-{v1}" )]
		public static double Negate( double v )
		{
			return -v;
		}

		[ShaderGenerationFunction( "{v1} == {v2}" )]
		public static bool Equals( double v1, double v2 )
		{
			return v1 == v2;
		}

		[ShaderGenerationFunction( "{v1} != {v2}" )]
		public static bool NotEquals( double v1, double v2 )
		{
			return v1 != v2;
		}

		[ShaderGenerationFunction( "{v1} > {v2}" )]
		public static bool GreaterThan( double v1, double v2 )
		{
			return v1 > v2;
		}

		[ShaderGenerationFunction( "{v1} < {v2}" )]
		public static bool LessThan( double v1, double v2 )
		{
			return v1 < v2;
		}

		[ShaderGenerationFunction( "{v1} >= {v2}" )]
		public static bool GreaterThanOrEqual( double v1, double v2 )
		{
			return v1 >= v2;
		}

		[ShaderGenerationFunction( "{v1} <= {v2}" )]
		public static bool LessThanOrEqual( double v1, double v2 )
		{
			return v1 <= v2;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[ShaderGenerationFunction( "{v1} + {v2}" )]
		public static int Add( int v1, int v2 )
		{
			return v1 + v2;
		}

		[ShaderGenerationFunction( "{v1} - {v2}" )]
		public static int Subtract( int v1, int v2 )
		{
			return v1 - v2;
		}

		[ShaderGenerationFunction( "{v1} * {v2}" )]
		public static int Multiply( int v1, int v2 )
		{
			return v1 * v2;
		}

		[ShaderGenerationFunction( "{v1} / {v2}" )]
		public static int Divide( int v1, int v2 )
		{
			return v1 / v2;
		}

		[ShaderGenerationFunction( "{v1} % {v2}" )]
		public static int Modulus( int v1, int v2 )
		{
			return v1 % v2;
		}

		[ShaderGenerationFunction( "-{v1}" )]
		public static int Negate( int v )
		{
			return -v;
		}

		[ShaderGenerationFunction( "{v1} == {v2}" )]
		public static bool Equals( int v1, int v2 )
		{
			return v1 == v2;
		}

		[ShaderGenerationFunction( "{v1} != {v2}" )]
		public static bool NotEquals( int v1, int v2 )
		{
			return v1 != v2;
		}

		[ShaderGenerationFunction( "{v1} > {v2}" )]
		public static bool GreaterThan( int v1, int v2 )
		{
			return v1 > v2;
		}

		[ShaderGenerationFunction( "{v1} < {v2}" )]
		public static bool LessThan( int v1, int v2 )
		{
			return v1 < v2;
		}

		[ShaderGenerationFunction( "{v1} >= {v2}" )]
		public static bool GreaterThanOrEqual( int v1, int v2 )
		{
			return v1 >= v2;
		}

		[ShaderGenerationFunction( "{v1} <= {v2}" )]
		public static bool LessThanOrEqual( int v1, int v2 )
		{
			return v1 <= v2;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[ShaderGenerationFunction( "{v1} & {v2}" )]
		public static bool And( bool v1, bool v2 )
		{
			return v1 & v2;
		}

		[ShaderGenerationFunction( "{v1} | {v2}" )]
		public static bool Or( bool v1, bool v2 )
		{
			return v1 | v2;
		}

		[ShaderGenerationFunction( "{v1} ^ {v2}" )]
		public static bool Xor( bool v1, bool v2 )
		{
			return v1 ^ v2;
		}

		[ShaderGenerationFunction( "~{v1}" )]
		public static bool Not( bool v )
		{
			return !v;
		}

		[ShaderGenerationFunction( "{v1} == {v2}" )]
		public static bool Equals( bool v1, bool v2 )
		{
			return v1 == v2;
		}

		[ShaderGenerationFunction( "{v1} != {v2}" )]
		public static bool NotEquals( bool v1, bool v2 )
		{
			return v1 != v2;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!new

		[ShaderGenerationFunction( "{pick1} ? {v1} : {v2}" )]
		public static double Select( double v1, double v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		[ShaderGenerationFunction( "{pick1} ? {v1} : {v2}" )]
		public static int Select( int v1, int v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		public static float Select( float v1, float v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[ShaderGenerationFunction( "lerp({v1}, {v2}, {amount})" )]
		public static double Lerp( double v1, double v2, double amount )
		{
			return v1 + ( v2 - v1 ) * amount;
		}

		public static float Lerp( float v1, float v2, float amount )
		{
			return v1 + ( v2 - v1 ) * amount;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[ShaderGenerationFunction( "abs({v})" )]
		public static double Abs( double v )
		{
			return Math.Abs( v );
		}

		[ShaderGenerationFunction( "acos({v})" )]
		public static double Acos( double v )
		{
			return Math.Acos( v );
		}

		[ShaderGenerationFunction( "asin({v})" )]
		public static double Asin( double v )
		{
			return Math.Asin( v );
		}

		[ShaderGenerationFunction( "atan({v})" )]
		public static double Atan( double v )
		{
			return Math.Atan( v );
		}

		[ShaderGenerationFunction( "atan2({y}, {x})" )]
		public static double Atan2( double y, double x )
		{
			return Math.Atan2( y, x );
		}

		[ShaderGenerationFunction( "cos({v})" )]
		public static double Cos( double v )
		{
			return Math.Cos( v );
		}

		[ShaderGenerationFunction( "cosh({v})" )]
		public static double Cosh( double v )
		{
			return Math.Cosh( v );
		}

		[ShaderGenerationFunction( "exp({v})" )]
		public static double Exp( double v )
		{
			return Math.Exp( v );
		}

		[ShaderGenerationFunction( "log({v})" )]
		public static double Log( double v )
		{
			return Math.Log( v );
		}

		[ShaderGenerationFunction( "log10({v})" )]
		public static double Log10( double v )
		{
			return Math.Log10( v );
		}

		[ShaderGenerationFunction( "max({v1}, {v2})" )]
		public static double Max( double v1, double v2 )
		{
			return Math.Max( v1, v2 );
		}

		[ShaderGenerationFunction( "min({v1}, {v2})" )]
		public static double Min( double v1, double v2 )
		{
			return Math.Min( v1, v2 );
		}

		[ShaderGenerationFunction( "pow({x}, {y})" )]
		public static double Pow( double x, double y )
		{
			return Math.Pow( x, y );
		}

		[ShaderGenerationFunction( "sin({v})" )]
		public static double Sin( double v )
		{
			return Math.Sin( v );
		}

		[ShaderGenerationFunction( "sinh({v})" )]
		public static double Sinh( double v )
		{
			return Math.Sinh( v );
		}

		[ShaderGenerationFunction( "sqrt({v})" )]
		public static double Sqrt( double v )
		{
			return Math.Sqrt( v );
		}

		[ShaderGenerationFunction( "tan({v})" )]
		public static double Tan( double v )
		{
			return Math.Tan( v );
		}

		[ShaderGenerationFunction( "tanh({v})" )]
		public static double Tanh( double v )
		{
			return Math.Tanh( v );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Calculates the next highest power of two.
		/// </summary>
		/// <remarks>
		/// This is a minimal method meant to be fast. There is a known edge case where an input of 0 will output 0
		/// instead of the mathematically correct value of 1. It will not be fixed.
		/// </remarks>
		/// <param name="v">A value.</param>
		/// <returns>The next power of two after the value.</returns>
		public static int NextPowerOfTwo( int v )
		{
			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;

			return v;
		}

		/// <summary>
		/// Calculates the next highest power of two.
		/// </summary>
		/// <remarks>
		/// This is a minimal method meant to be fast. There is a known edge case where an input of 0 will output 0
		/// instead of the mathematically correct value of 1. It will not be fixed.
		/// </remarks>
		/// <param name="v">A value.</param>
		/// <returns>The next power of two after the value.</returns>
		public static uint NextPowerOfTwo( uint v )
		{
			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;

			return v;
		}
	}
}
