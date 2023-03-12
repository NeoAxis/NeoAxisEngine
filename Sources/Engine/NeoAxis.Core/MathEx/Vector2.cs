// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Vec2> ) )]
	/// <summary>
	/// A structure encapsulating two double precision floating point values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Vector2
	{
		/// <summary>
		/// The X component of the vector.
		/// </summary>
		[ShaderGenerationFunction( "{this}.x" )]
		public double X;
		/// <summary>
		/// The Y component of the vector.
		/// </summary>
		[ShaderGenerationFunction( "{this}.y" )]
		public double Y;

		/// <summary>
		/// Returns the vector (0,0).
		/// </summary>
		public static readonly Vector2 Zero = new Vector2( 0.0, 0.0 );
		/// <summary>
		/// Returns the vector (1,1).
		/// </summary>
		public static readonly Vector2 One = new Vector2( 1.0, 1.0 );
		/// <summary>
		/// Returns the vector (1,0,0).
		/// </summary>
		public static readonly Vector2 XAxis = new Vector2( 1.0, 0.0 );
		/// <summary>
		/// Returns the vector (0,1,0).
		/// </summary>
		public static readonly Vector2 YAxis = new Vector2( 0.0, 1.0 );

		/// <summary>
		/// Constructs a vector with another given vector.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector2( Vector2 source )
		{
			this.X = source.X;
			this.Y = source.Y;
		}

		/// <summary>
		/// Constructs a vector with the given individual elements.
		/// </summary>
		/// <param name="x">X component.</param>
		/// <param name="y">Y component.</param>
		[ShaderGenerationFunction( "vec2({x}, {y})" )]
		public Vector2( double x, double y )
		{
			this.X = x;
			this.Y = y;
		}

		/// <summary>
		/// Constructs a vector with another given vector of <see cref="Vector2F"/> format.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector2( Vector2F source )
		{
			X = source.X;
			Y = source.Y;
		}

		/// <summary>
		/// Constructs a vector with another given vector of <see cref="Vector2I"/> format.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector2( Vector2I source )
		{
			X = source.X;
			Y = source.Y;
		}

		/// <summary>
		/// Converts a string representation of a vector into the equivalent <see cref="Vector2"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the vector (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Vector2"/> structure.</returns>
		[AutoConvertType]
		public static Vector2 Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (x y).", text ) );

			try
			{
				return new Vector2(
					double.Parse( vals[ 0 ] ),
					double.Parse( vals[ 1 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector2"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector2"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 17 );
			//return string.Format( "{0} {1}", x, y );
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector2"/> with a given precision.
		/// </summary>
		/// <param name="precision">The precision value.</param>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector2"/>.</returns>
		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "}";
			return string.Format( format, X, Y );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Vector2"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Vector2"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Vector2"/>; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Vector2 && this == (Vector2)obj );
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( X.GetHashCode() ^ Y.GetHashCode() );
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="v1">The first vector to add.</param>
		/// <param name="v2">The second vector to add.</param>
		/// <returns>The sum of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} + {v2}" )]
		public static Vector2 operator +( Vector2 v1, Vector2 v2 )
		{
			Vector2 result;
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
			return result;
		}

		/// <summary>
		/// Subtracts two vectors.
		/// </summary>
		/// <param name="v1">The vector to subtract from.</param>
		/// <param name="v2">The vector to be subtracted from another vector.</param>
		/// <returns>The difference of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} - {v2}" )]
		public static Vector2 operator -( Vector2 v1, Vector2 v2 )
		{
			Vector2 result;
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
			return result;
		}

		/// <summary>
		/// Multiplies two vectors together.
		/// </summary>
		/// <param name="v1">The first vector to multiply.</param>
		/// <param name="v2">The second vector to multiply.</param>
		/// <returns>The product vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} * {v2}" )]
		public static Vector2 operator *( Vector2 v1, Vector2 v2 )
		{
			Vector2 result;
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
			return result;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to scale.</param>
		/// <param name="s">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v} * {s}" )]
		public static Vector2 operator *( Vector2 v, double s )
		{
			Vector2 result;
			result.X = v.X * s;
			result.Y = v.Y * s;
			return result;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <returns>The scaled vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{s} * {v}" )]
		public static Vector2 operator *( double s, Vector2 v )
		{
			Vector2 result;
			result.X = s * v.X;
			result.Y = s * v.Y;
			return result;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The scaled vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} / {v2}" )]
		public static Vector2 operator /( Vector2 v1, Vector2 v2 )
		{
			Vector2 result;
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
			return result;
		}

		/// <summary>
		/// Divides a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to divide.</param>
		/// <param name="s">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v} / {s}" )]
		public static Vector2 operator /( Vector2 v, double s )
		{
			Vector2 result;
			double invS = 1.0 / s;
			result.X = v.X * invS;
			result.Y = v.Y * invS;
			return result;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <returns>The scaled vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{s} / {v}" )]
		public static Vector2 operator /( double s, Vector2 v )
		{
			Vector2 result;
			result.X = s / v.X;
			result.Y = s / v.Y;
			return result;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <returns>A vector facing in the opposite direction.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "-{v}" )]
		public static Vector2 operator -( Vector2 v )
		{
			Vector2 result;
			result.X = -v.X;
			result.Y = -v.Y;
			return result;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="v1">The first vector to add.</param>
		/// <param name="v2">The second vector to add.</param>
		/// <param name="result">When the method completes, contains the sum of the two vectors.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref Vector2 v1, ref Vector2 v2, out Vector2 result )
		{
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
		}

		/// <summary>
		/// Subtracts two vectors.
		/// </summary>
		/// <param name="v1">The vector from which to subtract.</param>
		/// <param name="v2">The vector which should be subtracted from another.</param>
		/// <param name="result">When the method completes, contains the difference of the two vectors.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref Vector2 v1, ref Vector2 v2, out Vector2 result )
		{
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
		}

		/// <summary>
		/// Multiplies two vectors together.
		/// </summary>
		/// <param name="v1">The first vector to multiply.</param>
		/// <param name="v2">The second vector to multiply.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Vector2 v1, ref Vector2 v2, out Vector2 result )
		{
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to scale.</param>
		/// <param name="s">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Vector2 v, double s, out Vector2 result )
		{
			result.X = v.X * s;
			result.Y = v.Y * s;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( double s, ref Vector2 v, out Vector2 result )
		{
			result.X = v.X * s;
			result.Y = v.Y * s;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( ref Vector2 v1, ref Vector2 v2, out Vector2 result )
		{
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
		}

		/// <summary>
		/// Divides a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to divide.</param>
		/// <param name="s">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( ref Vector2 v, double s, out Vector2 result )
		{
			double invS = 1.0 / s;
			result.X = v.X * invS;
			result.Y = v.Y * invS;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( double s, ref Vector2 v, out Vector2 result )
		{
			result.X = s / v.X;
			result.Y = s / v.Y;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <param name="result">When the method completes, contains a vector facing in the opposite direction.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Negate( ref Vector2 v, out Vector2 result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
		}

		//public static Vec2 Add( Vec2 v1, Vec2 v2 )
		//{
		//	Vec2 result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	return result;
		//}

		//public static Vec2 Subtract( Vec2 v1, Vec2 v2 )
		//{
		//	Vec2 result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	return result;
		//}

		//public static Vec2 Multiply( Vec2 v1, Vec2 v2 )
		//{
		//	Vec2 result;
		//	result.x = v1.x * v2.x;
		//	result.y = v1.y * v2.y;
		//	return result;
		//}

		//public static Vec2 Multiply( Vec2 v, double s )
		//{
		//	Vec2 result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	return result;
		//}

		//public static Vec2 Multiply( double s, Vec2 v )
		//{
		//	Vec2 result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	return result;
		//}

		//public static Vec2 Divide( Vec2 v1, Vec2 v2 )
		//{
		//	Vec2 result;
		//	result.x = v1.x / v2.x;
		//	result.y = v1.y / v2.y;
		//	return result;
		//}

		//public static Vec2 Divide( Vec2 v, double s )
		//{
		//	Vec2 result;
		//	double invS = 1.0 / s;
		//	result.x = v.x * invS;
		//	result.y = v.y * invS;
		//	return result;
		//}

		//public static Vec2 Divide( double s, Vec2 v )
		//{
		//	Vec2 result;
		//	result.x = s / v.x;
		//	result.y = s / v.y;
		//	return result;
		//}

		//public static Vec2 Negate( Vec2 v )
		//{
		//	Vec2 result;
		//	result.x = -v.x;
		//	result.y = -v.y;
		//	return result;
		//}


		/// <summary>
		/// Determines whether two given vectors are equal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are equal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Vector2 v1, Vector2 v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y );
		}

		/// <summary>
		/// Determines whether two given vectors are unequal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are unequal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Vector2 v1, Vector2 v2 )
		{
			return ( v1.X != v2.X || v1.Y != v2.Y );
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the X or Y component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the X component and 1 for the Y component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 1].</exception>
		public unsafe double this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.X )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.X )
				{
					v[ index ] = value;
				}
			}
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "dot({v1}, {v2})" )]
		public static double Dot( Vector2 v1, Vector2 v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static double Dot( ref Vector2 v1, ref Vector2 v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>When the method completes, contains the dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal static void Dot( ref Vector2 v1, ref Vector2 v2, out double result )
		{
			result = v1.X * v2.X + v1.Y * v2.Y;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double Dot( Vector2 v )
		{
			return X * v.X + Y * v.Y;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double Dot( ref Vector2 v )
		{
			return X * v.X + Y * v.Y;
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The cross product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "cross({v1}, {v2})" )]
		public static Vector2 Cross( Vector2 v1, Vector2 v2 )
		{
			return new Vector2(
				v1.Y * v2.X - v1.X * v2.Y,
				v1.X * v2.Y - v1.Y * v2.X );
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="result">When the method completes, contains the cross product of the two vectors.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Cross( ref Vector2 v1, ref Vector2 v2, out Vector2 result )
		{
			result.X = v1.Y * v2.X - v1.X * v2.Y;
			result.Y = v1.X * v2.Y - v1.Y * v2.X;
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The cross product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2 Cross( Vector2 v )
		{
			return new Vector2(
				Y * v.X - X * v.Y,
				X * v.Y - Y * v.X );
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <param name="result">When the method completes, contains the cross product of the two vectors.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Cross( ref Vector2 v, out Vector2 result )
		{
			result.X = Y * v.X - X * v.Y;
			result.Y = X * v.Y - Y * v.X;
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector2"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector2"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Vector2 v, double epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector2"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector2"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref Vector2 v, double epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Calculates the length of the current instance of <see cref="Vector2"/>.
		/// </summary>
		/// <returns>The length of the current instance of <see cref="Vector2"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "length({this})" )]
		public double Length()
		{
			return Math.Sqrt( X * X + Y * Y );
		}

		/// <summary>
		/// Calculates the squared length of the current instance of <see cref="Vector2"/>.
		/// </summary>
		/// <returns>The squared length of the current instance of <see cref="Vector2"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double LengthSquared()
		{
			return X * X + Y * Y;
		}

		/// <summary>
		/// Restricts the current instance of <see cref="Vector2"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector2"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Clamp( Vector2 min, Vector2 max )
		{
			if( X < min.X )
				X = min.X;
			else if( X > max.X )
				X = max.X;

			if( Y < min.Y )
				Y = min.Y;
			else if( Y > max.Y )
				Y = max.Y;
		}

		/// <summary>
		/// Restricts the current instance of <see cref="Vector2"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector2"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "clamp({this}, {min}, {max})" )]
		public Vector2 GetClamp( Vector2 min, Vector2 max )
		{
			var r = this;
			r.Clamp( min, max );
			return r;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector2"/> into a unit vector.
		/// </summary>
		/// <returns>Returns the length of the current instance of <see cref="Vector2"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double Normalize()
		{
			double sqrLength = X * X + Y * Y;
			double invLength = 1.0 / Math.Sqrt( sqrLength );
			X *= invLength;
			Y *= invLength;
			return invLength * sqrLength;
		}

		/// <summary>
		/// Converts a vector into a unit vector. 
		/// </summary>
		/// <param name="v">The vector to normalize.</param>
		/// <returns>The normalized vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2 Normalize( Vector2 v )
		{
			double invLength = 1.0 / Math.Sqrt( v.X * v.X + v.Y * v.Y );
			return new Vector2( v.X * invLength, v.Y * invLength );
		}

		/// <summary>
		/// Converts a vector into a unit vector.
		/// </summary>
		/// <param name="v">The vector to normalize.</param>
		/// <param name="result">When the method completes, contains the normalized vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Normalize( ref Vector2 v, out Vector2 result )
		{
			double invLength = 1.0 / Math.Sqrt( v.X * v.X + v.Y * v.Y );
			result.X = v.X * invLength;
			result.Y = v.Y * invLength;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector2"/> into a unit vector and returns the normalized vector.
		/// </summary>
		/// <returns>The normalized instance of <see cref="Vector2"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "normalize({this})" )]
		public Vector2 GetNormalize()
		{
			double invLength = 1.0 / Math.Sqrt( X * X + Y * Y );
			return new Vector2( X * invLength, Y * invLength );
		}

		/// <summary>
		/// Performs a linear interpolation between two vectors based on the given weighting.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="v2"/>.</param>
		/// <returns>The interpolated vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "lerp({v1}, {v2}, {amount})" )]
		public static Vector2 Lerp( Vector2 v1, Vector2 v2, double amount )
		{
			Vector2 result;
			result.X = v1.X + ( ( v2.X - v1.X ) * amount );
			result.Y = v1.Y + ( ( v2.Y - v1.Y ) * amount );
			return result;
		}

		/// <summary>
		/// Performs a linear interpolation between two vectors based on the given weighting.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="v2"/>.</param>
		/// <param name="result">When the method completes, contains the linear interpolation of the two vectors.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Lerp( ref Vector2 v1, ref Vector2 v2, double amount, out Vector2 result )
		{
			result.X = v1.X + ( ( v2.X - v1.X ) * amount );
			result.Y = v1.Y + ( ( v2.Y - v1.Y ) * amount );
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector2"/> between 0 and 1.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Saturate()
		{
			MathEx.Saturate( ref X );
			MathEx.Saturate( ref Y );
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector2"/> between 0 and 1 and returns the saturated value.
		/// </summary>
		/// <returns>The saturated value.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "saturate({this})" )]
		public Vector2 GetSaturate()
		{
			var result = this;
			result.Saturate();
			return result;
		}

		/// <summary>
		/// Calculates the distance between two vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The distance between two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "distance({v1}, {v2})" )]
		public static double Distance( Vector2 v1, Vector2 v2 )
		{
			Subtract( ref v1, ref v2, out Vector2 result );
			return result.Length();
		}

		/// <summary>
		/// Calculates the distance between two vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The distance between two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static double Distance( ref Vector2 v1, ref Vector2 v2 )
		{
			Subtract( ref v1, ref v2, out Vector2 result );
			return result.Length();
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector2"/> into the equivalent <see cref="Vector2F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector2F"/> structure.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector2F ToVector2F()
		{
			Vector2F result;
			result.X = (float)X;
			result.Y = (float)Y;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector2"/> into the equivalent <see cref="Vector2I"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector2I"/> structure.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector2I ToVector2I()
		{
			Vector2I result;
			result.X = (int)X;
			result.Y = (int)Y;
			return result;
		}

		/// <summary>
		/// Chooses one of two vectors depending on the <paramref name="pick1"/> value.
		/// </summary>
		/// <param name="v1">The first vector to choose.</param>
		/// <param name="v2">The second vector to choose.</param>
		/// <param name="pick1">If this value is true, the method chooses the virst vector, otherwise it chooses the second one.</param>
		/// <returns>The selected vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{pick1} ? {v1} : {v2}" )]
		public static Vector2 Select( Vector2 v1, Vector2 v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/// <summary>
		/// Rounds the current instance of <see cref="Vector2"/> towards zero for each component in a vector.
		/// </summary>
		/// <param name="v"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Truncate()
		{
			X = (long)X;
			Y = (long)Y;
		}

		/// <summary>
		/// Rounds a given vector towards zero for each component in it and returns the truncated vector.
		/// </summary>
		/// <param name="v">The vector to truncate.</param>
		/// <returns>The truncated vector</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "trunc({v})" )]
		public Vector2 GetTruncate( Vector2 v )
		{
			return new Vector2( (long)v.X, (long)v.Y );
		}

		/// <summary>
		/// Determines whether any component of a given vector is unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if any component of the specified vector is unequal to the zero; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "any({v})" )]
		public static bool AnyNonZero( Vector2 v )
		{
			return v != Zero;
		}

		/// <summary>
		/// Determines whether all components of a given vector are unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if all components of the specified vector are unequal to the zero; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "all({v})" )]
		public static bool AllNonZero( Vector2 v )
		{
			return v.X != 0 && v.Y != 0;
		}

		/// <summary>
		/// Returns a vector whose elements are the absolute values of each of the specified vector's components.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The absolute value vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "abs({v})" )]
		public static Vector2 Abs( Vector2 v )
		{
			return new Vector2( Math.Abs( v.X ), Math.Abs( v.Y ) );
		}

		/// <summary>
		/// Calculates the arc-cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose cosines are equal to the
		/// corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "acos({v})" )]
		public static Vector2 Acos( Vector2 v )
		{
			return new Vector2( Math.Acos( v.X ), Math.Acos( v.Y ) );
		}

		/// <summary>
		/// Calculates the arc-sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose sines are equal to the
		/// corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "asin({v})" )]
		public static Vector2 Asin( Vector2 v )
		{
			return new Vector2( Math.Asin( v.X ), Math.Asin( v.Y ) );
		}

		/// <summary>
		/// Calculates the arc-tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are equal to the
		/// corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "atan({v})" )]
		public static Vector2 Atan( Vector2 v )
		{
			return new Vector2( Math.Atan( v.X ), Math.Atan( v.Y ) );
		}

		/// <summary>
		/// Returns the vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.
		/// </summary>
		/// <param name="y">The first vector.</param>
		/// <param name="x">The second vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "atan2({y}, {x})" )]
		public static Vector2 Atan2( Vector2 y, Vector2 x )
		{
			return new Vector2( Math.Atan2( y.X, x.X ), Math.Atan2( y.Y, x.Y ) );
		}

		/// <summary>
		/// Calculates the cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the cosines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "cos({v})" )]
		public static Vector2 Cos( Vector2 v )
		{
			return new Vector2( Math.Cos( v.X ), Math.Cos( v.Y ) );
		}

		/// <summary>
		/// Calculates the hyperbolic cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic cosines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "cosh({v})" )]
		public static Vector2 Cosh( Vector2 v )
		{
			return new Vector2( Math.Cosh( v.X ), Math.Cosh( v.Y ) );
		}

		/// <summary>
		/// Returns the vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "exp({v})" )]
		public static Vector2 Exp( Vector2 v )
		{
			return new Vector2( Math.Exp( v.X ), Math.Exp( v.Y ) );
		}

		/// <summary>
		/// Calculates the natural logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the natural logarithms of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "log({v})" )]
		public static Vector2 Log( Vector2 v )
		{
			return new Vector2( Math.Log( v.X ), Math.Log( v.Y ) );
		}

		/// <summary>
		/// Calculates the base 10 logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the base 10 logarithms of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "log10({v})" )]
		public static Vector2 Log10( Vector2 v )
		{
			return new Vector2( Math.Log10( v.X ), Math.Log10( v.Y ) );
		}

		/// <summary>
		/// Returns a vector containing the largest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the largest components of the specified vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "max({v1}, {v2})" )]
		public static Vector2 Max( Vector2 v1, Vector2 v2 )
		{
			return new Vector2( Math.Max( v1.X, v2.X ), Math.Max( v1.Y, v2.Y ) );
		}

		/// <summary>
		/// Returns a vector containing the smallest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the smallest components of the specified vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "min({v1}, {v2})" )]
		public static Vector2 Min( Vector2 v1, Vector2 v2 )
		{
			return new Vector2( Math.Min( v1.X, v2.X ), Math.Min( v1.Y, v2.Y ) );
		}

		/// <summary>
		/// Returns the vector which contains the components of the first specified vector raised to power of the numbers which are equal to the corresponding components of the second specified vector.
		/// </summary>
		/// <param name="x">The first vector.</param>
		/// <param name="y">The second vector.</param>
		/// <returns>The vector which contains the components of the first specified vector raised to power of
		/// the numbers which are equal to the corresponding components of the second specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "pow({x}, {y})" )]
		public static Vector2 Pow( Vector2 x, Vector2 y )
		{
			return new Vector2( Math.Pow( x.X, y.X ), Math.Pow( x.Y, y.Y ) );
		}

		/// <summary>
		/// Calculates the sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the sines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "sin({v})" )]
		public static Vector2 Sin( Vector2 v )
		{
			return new Vector2( Math.Sin( v.X ), Math.Sin( v.Y ) );
		}

		/// <summary>
		/// Calculates the hyperbolic sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic sines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "sinh({v})" )]
		public static Vector2 Sinh( Vector2 v )
		{
			return new Vector2( Math.Sinh( v.X ), Math.Sinh( v.Y ) );
		}

		/// <summary>
		/// Calculates the square root of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the square root of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "sqrt({v})" )]
		public static Vector2 Sqrt( Vector2 v )
		{
			return new Vector2( Math.Sqrt( v.X ), Math.Sqrt( v.Y ) );
		}

		/// <summary>
		/// Calculates the tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the tangents of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "tan({v})" )]
		public static Vector2 Tan( Vector2 v )
		{
			return new Vector2( Math.Tan( v.X ), Math.Tan( v.Y ) );
		}

		/// <summary>
		/// Calculates the hyperbolic tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic tangents of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "tanh({v})" )]
		public static Vector2 Tanh( Vector2 v )
		{
			return new Vector2( Math.Tanh( v.X ), Math.Tanh( v.Y ) );
		}

		/// <summary>
		/// Returns the value of the smallest component of the current instance of <see cref="Vector2"/>.
		/// </summary>
		/// <returns>The value of the smallest component of the current instance of <see cref="Vector2"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double MinComponent()
		{
			return Math.Min( X, Y );
		}

		/// <summary>
		/// Returns the value of the largest component of the current instance of <see cref="Vector2"/>.
		/// </summary>
		/// <returns>The value of the largest component of the current instance of <see cref="Vector2"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double MaxComponent()
		{
			return Math.Max( X, Y );
		}
	}
}
