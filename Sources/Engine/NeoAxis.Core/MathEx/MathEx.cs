// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.CompilerServices;

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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float DegreeToRadian( float v )
		{
			return v * ( PI / 180.0f );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float RadianToDegree( float v )
		{
			return v * ( 180.0f / PI );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float InvSqrt( float v )
		{
			return 1.0f / (float)Math.Sqrt( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Sqrt( float v )
		{
			return (float)Math.Sqrt( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Sin( float v )
		{
			return (float)Math.Sin( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Sinh( float v )
		{
			return (float)Math.Sinh( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Cos( float v )
		{
			return (float)Math.Cos( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Cosh( float v )
		{
			return (float)Math.Cosh( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Tan( float v )
		{
			return (float)Math.Tan( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Tanh( float v )
		{
			return (float)Math.Tanh( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Asin( float v )
		{
			return (float)Math.Asin( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Acos( float v )
		{
			return (float)Math.Acos( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Atan( float v )
		{
			return (float)Math.Atan( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Atan2( float y, float x )
		{
			return (float)Math.Atan2( y, x );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Pow( float x, float y )
		{
			return (float)Math.Pow( x, y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Exp( float v )
		{
			return (float)Math.Exp( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Swap( ref float v1, ref float v2 )
		{
			float c = v1;
			v1 = v2;
			v2 = c;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Floor( float v )
		{
			return (float)Math.Floor( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Log( float v )
		{
			return (float)Math.Log( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Log( float v, float newBase )
		{
			return (float)Math.Log( v, newBase );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Log10( float v )
		{
			return (float)Math.Log10( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Round( float v )
		{
			return (float)Math.Round( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static int RoundToInteger( double value )
		{
			return (int)Math.Round( value, 0 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float RadianNormalize360( float angle )
		{
			if( ( angle >= ( PI * 2.0f ) ) || ( angle < 0.0f ) )
				angle -= Floor( angle / ( PI * 2.0f ) ) * ( PI * 2.0f );
			return angle;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static double DegreeToRadian( double v )
		{
			return v * ( Math.PI / 180.0 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static double RadianToDegree( double v )
		{
			return v * ( 180.0 / Math.PI );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Swap( ref double v1, ref double v2 )
		{
			double c = v1;
			v1 = v2;
			v2 = c;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static double RadianNormalize360( double angle )
		{
			if( ( angle >= ( Math.PI * 2.0 ) ) || ( angle < 0.0 ) )
				angle -= Math.Floor( angle / ( Math.PI * 2.0 ) ) * ( Math.PI * 2.0 );
			return angle;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Clamp( ref byte value, byte min, byte max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Clamp( ref int value, int min, int max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Clamp( ref float value, float min, float max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Clamp( ref double value, double min, double max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Clamp( ref double value, Range range )
		{
			if( value < range.Minimum )
				value = range.Minimum;
			if( value > range.Maximum )
				value = range.Maximum;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Clamp( ref RadianF value, RadianF min, RadianF max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Clamp( ref DegreeF value, DegreeF min, DegreeF max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Clamp( ref Radian value, Radian min, Radian max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Clamp( ref Degree value, Degree min, Degree max )
		{
			if( value < min )
				value = min;
			if( value > max )
				value = max;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static byte Clamp( byte value, byte min, byte max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static int Clamp( int value, int min, int max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Clamp( float value, float min, float max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "clamp({value}, {min}, {max})" )]
		public static double Clamp( double value, double min, double max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF Clamp( RadianF value, RadianF min, RadianF max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF Clamp( DegreeF value, DegreeF min, DegreeF max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian Clamp( Radian value, Radian min, Radian max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Degree Clamp( Degree value, Degree min, Degree max )
		{
			if( value < min )
				return min;
			if( value > max )
				return max;
			return value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Saturate( ref HalfType v )
		{
			if( v < HalfType.Zero )
				v = HalfType.Zero;
			if( v > HalfType.One )
				v = HalfType.One;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Saturate( ref float v )
		{
			if( v < 0 )
				v = 0;
			if( v > 1 )
				v = 1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Saturate( ref double v )
		{
			if( v < 0 )
				v = 0;
			if( v > 1 )
				v = 1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Saturate( ref decimal v )
		{
			if( v < 0 )
				v = 0;
			if( v > 1 )
				v = 1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Saturate( float v )
		{
			if( v < 0 )
				return 0;
			if( v > 1 )
				return 1;
			return v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "saturate({v})" )]
		public static double Saturate( double v )
		{
			if( v < 0 )
				return 0;
			if( v > 1 )
				return 1;
			return v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static decimal Saturate( decimal v )
		{
			if( v < 0 )
				return 0;
			if( v > 1 )
				return 1;
			return v;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} + {v2}" )]
		public static double Add( double v1, double v2 )
		{
			return v1 + v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} / {v2}" )]
		public static double Divide( double v1, double v2 )
		{
			return v1 / v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} % {v2}" )]
		public static double Modulus( double v1, double v2 )
		{
			return v1 % v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "-{v1}" )]
		public static double Negate( double v )
		{
			return -v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} == {v2}" )]
		public static bool Equals( double v1, double v2 )
		{
			return v1 == v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} != {v2}" )]
		public static bool NotEquals( double v1, double v2 )
		{
			return v1 != v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} > {v2}" )]
		public static bool GreaterThan( double v1, double v2 )
		{
			return v1 > v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} < {v2}" )]
		public static bool LessThan( double v1, double v2 )
		{
			return v1 < v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} >= {v2}" )]
		public static bool GreaterThanOrEqual( double v1, double v2 )
		{
			return v1 >= v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} <= {v2}" )]
		public static bool LessThanOrEqual( double v1, double v2 )
		{
			return v1 <= v2;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} + {v2}" )]
		public static int Add( int v1, int v2 )
		{
			return v1 + v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} - {v2}" )]
		public static int Subtract( int v1, int v2 )
		{
			return v1 - v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} * {v2}" )]
		public static int Multiply( int v1, int v2 )
		{
			return v1 * v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} / {v2}" )]
		public static int Divide( int v1, int v2 )
		{
			return v1 / v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} % {v2}" )]
		public static int Modulus( int v1, int v2 )
		{
			return v1 % v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "-{v1}" )]
		public static int Negate( int v )
		{
			return -v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} == {v2}" )]
		public static bool Equals( int v1, int v2 )
		{
			return v1 == v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} != {v2}" )]
		public static bool NotEquals( int v1, int v2 )
		{
			return v1 != v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} > {v2}" )]
		public static bool GreaterThan( int v1, int v2 )
		{
			return v1 > v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} < {v2}" )]
		public static bool LessThan( int v1, int v2 )
		{
			return v1 < v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} >= {v2}" )]
		public static bool GreaterThanOrEqual( int v1, int v2 )
		{
			return v1 >= v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} <= {v2}" )]
		public static bool LessThanOrEqual( int v1, int v2 )
		{
			return v1 <= v2;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} & {v2}" )]
		public static bool And( bool v1, bool v2 )
		{
			return v1 & v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} | {v2}" )]
		public static bool Or( bool v1, bool v2 )
		{
			return v1 | v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} ^ {v2}" )]
		public static bool Xor( bool v1, bool v2 )
		{
			return v1 ^ v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "~{v1}" )]
		public static bool Not( bool v )
		{
			return !v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} == {v2}" )]
		public static bool Equals( bool v1, bool v2 )
		{
			return v1 == v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} != {v2}" )]
		public static bool NotEquals( bool v1, bool v2 )
		{
			return v1 != v2;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{pick1} ? {v1} : {v2}" )]
		public static double Select( double v1, double v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{pick1} ? {v1} : {v2}" )]
		public static int Select( int v1, int v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Select( float v1, float v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "lerp({v1}, {v2}, {amount})" )]
		public static double Lerp( double v1, double v2, double amount )
		{
			return v1 + ( v2 - v1 ) * amount;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static float Lerp( float v1, float v2, float amount )
		{
			return v1 + ( v2 - v1 ) * amount;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "abs({v})" )]
		public static double Abs( double v )
		{
			return Math.Abs( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "acos({v})" )]
		public static double Acos( double v )
		{
			return Math.Acos( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "asin({v})" )]
		public static double Asin( double v )
		{
			return Math.Asin( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "atan({v})" )]
		public static double Atan( double v )
		{
			return Math.Atan( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "atan2({y}, {x})" )]
		public static double Atan2( double y, double x )
		{
			return Math.Atan2( y, x );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "cos({v})" )]
		public static double Cos( double v )
		{
			return Math.Cos( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "cosh({v})" )]
		public static double Cosh( double v )
		{
			return Math.Cosh( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "exp({v})" )]
		public static double Exp( double v )
		{
			return Math.Exp( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "log({v})" )]
		public static double Log( double v )
		{
			return Math.Log( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "log10({v})" )]
		public static double Log10( double v )
		{
			return Math.Log10( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "max({v1}, {v2})" )]
		public static double Max( double v1, double v2 )
		{
			return Math.Max( v1, v2 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "min({v1}, {v2})" )]
		public static double Min( double v1, double v2 )
		{
			return Math.Min( v1, v2 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "pow({x}, {y})" )]
		public static double Pow( double x, double y )
		{
			return Math.Pow( x, y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "sin({v})" )]
		public static double Sin( double v )
		{
			return Math.Sin( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "sinh({v})" )]
		public static double Sinh( double v )
		{
			return Math.Sinh( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "sqrt({v})" )]
		public static double Sqrt( double v )
		{
			return Math.Sqrt( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "tan({v})" )]
		public static double Tan( double v )
		{
			return Math.Tan( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void TransformMultiply( ref Vector3 position, ref QuaternionF rotation, ref Vector3F scale, ref Vector3 point, out Vector3 result )
		{
			var pointF = point.ToVector3F();
			Vector3F.Multiply( ref scale, ref pointF, out var v1 );
			QuaternionF.Multiply( ref rotation, ref v1, out var v2 );
			result.X = v2.X + position.X;
			result.Y = v2.Y + position.Y;
			result.Z = v2.Z + position.Z;

			//result = rotation * ( point * scale ) + position;
		}
	}
}
