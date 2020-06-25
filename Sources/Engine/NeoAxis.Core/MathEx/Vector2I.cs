// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Vec2I> ) )]
	/// <summary>
	/// A structure encapsulating two integer values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Vector2I
	{
		/// <summary>
		/// The X component of the vector.
		/// </summary>
		public int X;
		/// <summary>
		/// The Y component of the vector.
		/// </summary>
		public int Y;

		/// <summary>
		/// Returns the vector (0,0).
		/// </summary>
		public static readonly Vector2I Zero = new Vector2I( 0, 0 );
		/// <summary>
		/// Returns the vector (1,1).
		/// </summary>
		public static readonly Vector2I One = new Vector2I( 1, 1 );

		/// <summary>
		/// Constructs a vector with another given vector.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector2I( Vector2I source )
		{
			X = source.X;
			Y = source.Y;
		}

		/// <summary>
		/// Constructs a vector with the given individual elements.
		/// </summary>
		/// <param name="x">X component.</param>
		/// <param name="y">Y component.</param>
		public Vector2I( int x, int y )
		{
			this.X = x;
			this.Y = y;
		}

		/// <summary>
		/// Converts a string representation of a vector into the equivalent <see cref="Vector2I"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the vector (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Vector2I"/> structure.</returns>
		[AutoConvertType]
		public static Vector2I Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (x y).", text ) );

			try
			{
				return new Vector2I(
					int.Parse( vals[ 0 ] ),
						  int.Parse( vals[ 1 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector2I"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector2I"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1}", X, Y );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Vector2I"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Vector2I"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Vector2I"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Vector2I && this == (Vector2I)obj );
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
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
		public static Vector2I operator +( Vector2I v1, Vector2I v2 )
		{
			Vector2I result;
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
		public static Vector2I operator -( Vector2I v1, Vector2I v2 )
		{
			Vector2I result;
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
		public static Vector2I operator *( Vector2I v1, Vector2I v2 )
		{
			Vector2I result;
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
			return result;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to scale.</param>
		/// <param name="i">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector2I operator *( Vector2I v, int i )
		{
			Vector2I result;
			result.X = v.X * i;
			result.Y = v.Y * i;
			return result;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector2I operator *( int i, Vector2I v )
		{
			Vector2I result;
			result.X = i * v.X;
			result.Y = i * v.Y;
			return result;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector2I operator /( Vector2I v1, Vector2I v2 )
		{
			Vector2I result;
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
			return result;
		}

		/// <summary>
		/// Divides a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to divide.</param>
		/// <param name="i">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector2I operator /( Vector2I v, int i )
		{
			Vector2I result;
			result.X = v.X / i;
			result.Y = v.Y / i;
			return result;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector2I operator /( int i, Vector2I v )
		{
			Vector2I result;
			result.X = i / v.X;
			result.Y = i / v.Y;
			return result;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <returns>A vector facing in the opposite direction.</returns>
		public static Vector2I operator -( Vector2I v )
		{
			Vector2I result;
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
		public static void Add( ref Vector2I v1, ref Vector2I v2, out Vector2I result )
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
		public static void Subtract( ref Vector2I v1, ref Vector2I v2, out Vector2I result )
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
		public static void Multiply( ref Vector2I v1, ref Vector2I v2, out Vector2I result )
		{
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to scale.</param>
		/// <param name="i">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( ref Vector2I v, int i, out Vector2I result )
		{
			result.X = v.X * i;
			result.Y = v.Y * i;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( int i, ref Vector2I v, out Vector2I result )
		{
			result.X = v.X * i;
			result.Y = v.Y * i;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( ref Vector2I v1, ref Vector2I v2, out Vector2I result )
		{
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
		}

		/// <summary>
		/// Divides a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to divide.</param>
		/// <param name="i">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( ref Vector2I v, int i, out Vector2I result )
		{
			result.X = v.X / i;
			result.Y = v.Y / i;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="v1">The scalar value.</param>
		/// <param name="v2">The vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( int v1, ref Vector2I v2, out Vector2I result )
		{
			result.X = v1 / v2.X;
			result.Y = v1 / v2.Y;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <param name="result">When the method completes, contains a vector facing in the opposite direction.</param>
		public static void Negate( ref Vector2I v, out Vector2I result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
		}

		//public static Vec2I Add( Vec2I v1, Vec2I v2 )
		//{
		//	Vec2I result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	return result;
		//}

		//public static Vec2I Subtract( Vec2I v1, Vec2I v2 )
		//{
		//	Vec2I result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	return result;
		//}

		//public static Vec2I Multiply( Vec2I v1, Vec2I v2 )
		//{
		//	Vec2I result;
		//	result.x = v1.x * v2.x;
		//	result.y = v1.y * v2.y;
		//	return result;
		//}

		//public static Vec2I Multiply( Vec2I v, int i )
		//{
		//	Vec2I result;
		//	result.x = v.x * i;
		//	result.y = v.y * i;
		//	return result;
		//}

		//public static Vec2I Multiply( int i, Vec2I v )
		//{
		//	Vec2I result;
		//	result.x = v.x * i;
		//	result.y = v.y * i;
		//	return result;
		//}

		//public static Vec2I Divide( Vec2I v1, Vec2I v2 )
		//{
		//	Vec2I result;
		//	result.x = v1.x / v2.x;
		//	result.y = v1.y / v2.y;
		//	return result;
		//}

		//public static Vec2I Divide( Vec2I v, int i )
		//{
		//	Vec2I result;
		//	result.x = v.x / i;
		//	result.y = v.y / i;
		//	return result;
		//}

		//public static Vec2I Divide( int i, Vec2I v )
		//{
		//	Vec2I result;
		//	result.x = i / v.x;
		//	result.y = i / v.y;
		//	return result;
		//}

		//public static Vec2I Negate( Vec2I v )
		//{
		//	Vec2I result;
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
		public static bool operator ==( Vector2I v1, Vector2I v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y );
		}

		/// <summary>
		/// Determines whether two given vectors are unequal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are unequal; False otherwise.</returns>
		public static bool operator !=( Vector2I v1, Vector2I v2 )
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
		public unsafe int this[ int index ]
		{
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( int* v = &this.X )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( int* v = &this.X )
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
		public static int Dot( Vector2I v1, Vector2I v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>When the method completes, contains the dot product of the two vectors.</returns>
		public static void Dot( ref Vector2I v1, ref Vector2I v2, out int result )
		{
			result = v1.X * v2.X + v1.Y * v2.Y;
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The cross product of the two vectors.</returns>
		public static Vector2I Cross( Vector2I v1, Vector2I v2 )
		{
			return new Vector2I(
				 v1.Y * v2.X - v1.X * v2.Y,
				 v1.X * v2.Y - v1.Y * v2.X );
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="result">When the method completes, contains the cross product of the two vectors.</param>
		public static void Cross( ref Vector2I v1, ref Vector2I v2, out Vector2I result )
		{
			result.X = v1.Y * v2.X - v1.X * v2.Y;
			result.Y = v1.X * v2.Y - v1.Y * v2.X;
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector2I"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector2I"/>; False otherwise.</returns>
		public bool Equals( Vector2I v, int epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Restricts the current instance of <see cref="Vector2I"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector2I"/>.</returns>
		public void Clamp( Vector2I min, Vector2I max )
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
		/// Converts the current instance of <see cref="Vector2I"/> into the equivalent <see cref="Vector2F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector2F"/> structure.</returns>
		[AutoConvertType]
		public Vector2F ToVector2F()
		{
			Vector2F result;
			result.X = X;
			result.Y = Y;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector2I"/> into the equivalent <see cref="Vector2"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector2"/> structure.</returns>
		[AutoConvertType]
		public Vector2 ToVector2()
		{
			Vector2 result;
			result.X = X;
			result.Y = Y;
			return result;
		}

		/// <summary>
		/// Chooses one of two vectors depending on the <paramref name="pick1"/> value.
		/// </summary>
		/// <param name="v1">The first vector to choose.</param>
		/// <param name="v2">The second vector to choose.</param>
		/// <param name="pick1">If this value is true, the method chooses the virst vector, otherwise it chooses the second one.</param>
		/// <returns>The selected vector.</returns>
		public static Vector2I Select( Vector2I v1, Vector2I v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/// <summary>
		/// Determines whether any component of a given vector is unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if any component of the specified vector is unequal to the zero; otherwise, False.</returns>
		public static bool AnyNonZero( Vector2I v )
		{
			return v != Zero;
		}

		/// <summary>
		/// Determines whether all components of a given vector are unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if all components of the specified vector are unequal to the zero; otherwise, False.</returns>
		public static bool AllNonZero( Vector2I v )
		{
			return v.X != 0 && v.Y != 0;
		}

		/// <summary>
		/// Returns a vector containing the largest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the largest components of the specified vectors.</returns>
		public static Vector2I Max( Vector2I v1, Vector2I v2 )
		{
			return new Vector2I( Math.Max( v1.X, v2.X ), Math.Max( v1.Y, v2.Y ) );
		}

		/// <summary>
		/// Returns a vector containing the smallest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the smallest components of the specified vectors.</returns>
		public static Vector2I Min( Vector2I v1, Vector2I v2 )
		{
			return new Vector2I( Math.Min( v1.X, v2.X ), Math.Min( v1.Y, v2.Y ) );
		}

		/// <summary>
		/// Returns the value of the smallest component of the current instance of <see cref="Vector2I"/>.
		/// </summary>
		/// <returns>The value of the smallest component of the current instance of <see cref="Vector2I"/>.</returns>
		public int MinComponent()
		{
			return Math.Min( X, Y );
		}

		/// <summary>
		/// Returns the value of the largest component of the current instance of <see cref="Vector2I"/>.
		/// </summary>
		/// <returns>The value of the largest component of the current instance of <see cref="Vector2I"/>.</returns>
		public int MaxComponent()
		{
			return Math.Max( X, Y );
		}
	}
}
