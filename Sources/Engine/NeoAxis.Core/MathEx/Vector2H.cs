// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Vec3F> ) )]
	/// <summary>
	/// A structure encapsulating three half precision floating point values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	[HCExpandable]
	public struct Vector2H
	{
		/// <summary>
		/// The X component of the vector.
		/// </summary>
		public HalfType X;
		/// <summary>
		/// The Y component of the vector.
		/// </summary>
		public HalfType Y;

		/// <summary>
		/// Returns the vector (0,0,0).
		/// </summary>
		public static readonly Vector2H Zero = new Vector2H( 0.0f, 0.0f );
		/// <summary>
		/// Returns the vector (1,1,1).
		/// </summary>
		public static readonly Vector2H One = new Vector2H( 1.0f, 1.0f );
		/// <summary>
		/// Returns the vector (1,0,0).
		/// </summary>
		public static readonly Vector2H XAxis = new Vector2H( 1.0f, 0.0f );
		/// <summary>
		/// Returns the vector (0,1,0).
		/// </summary>
		public static readonly Vector2H YAxis = new Vector2H( 0.0f, 1.0f );
		/// <summary>
		/// Returns the vector (0,0,1).
		/// </summary>
		public static readonly Vector2H ZAxis = new Vector2H( 0.0f, 0.0f );

		/// <summary>
		/// Constructs a vector with another given vector.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector2H( Vector2H source )
		{
			this.X = source.X;
			this.Y = source.Y;
		}

		///// <summary>
		///// Constructs a vector with a given <see cref="Vector2F"/> and a scalar.
		///// </summary>
		///// <param name="xy">The given <see cref="Vector2F"/>.</param>
		///// <param name="z">The scalar value.</param>
		//public Vector2H( Vector2F xy, float z )
		//{
		//	this.X = xy.X;
		//	this.Y = xy.Y;
		//	this.Z = z;
		//}

		/// <summary>
		/// Constructs a vector with the given individual elements.
		/// </summary>
		/// <param name="x">X component.</param>
		/// <param name="y">Y component.</param>
		/// <param name="z">Z component.</param>
		public Vector2H( HalfType x, HalfType y )
		{
			this.X = x;
			this.Y = y;
		}

		/// <summary>
		/// Constructs a vector with the given individual elements.
		/// </summary>
		/// <param name="x">X component.</param>
		/// <param name="y">Y component.</param>
		/// <param name="z">Z component.</param>
		public Vector2H( float x, float y )
		{
			this.X = new HalfType( x );
			this.Y = new HalfType( y );
		}

		/// <summary>
		/// Converts a string representation of a vector into the equivalent <see cref="Vector2H"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the vector (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Vector2H"/> structure.</returns>
		[AutoConvertType]
		public static Vector2H Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (x y).", text ) );

			try
			{
				return new Vector2H(
					HalfType.Parse( vals[ 0 ] ),
					HalfType.Parse( vals[ 1 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector2H"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector2H"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 8 );
			//return string.Format( "{0} {1} {2}", x, y, z );
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector2H"/> with a given precision.
		/// </summary>
		/// <param name="precision">The precision value.</param>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector2H"/>.</returns>
		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "}";
			return string.Format( format, X, Y );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Vector2H"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Vector2H"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Vector2H"/>; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Vector2H && this == (Vector2H)obj );
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
		public static Vector2H operator +( Vector2H v1, Vector2H v2 )
		{
			Vector2H result;
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
		public static Vector2H operator -( Vector2H v1, Vector2H v2 )
		{
			Vector2H result;
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
		public static Vector2H operator *( Vector2H v1, Vector2H v2 )
		{
			Vector2H result;
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
		public static Vector2H operator *( Vector2H v, HalfType s )
		{
			Vector2H result;
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
		public static Vector2H operator *( HalfType s, Vector2H v )
		{
			Vector2H result;
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
		public static Vector2H operator /( Vector2H v1, Vector2H v2 )
		{
			Vector2H result;
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
		public static Vector2H operator /( Vector2H v, HalfType s )
		{
			Vector2H result;
			float invS = 1.0f / s;
			result.X = new HalfType( v.X * invS );
			result.Y = new HalfType( v.Y * invS );
			return result;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <returns>The scaled vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H operator /( HalfType s, Vector2H v )
		{
			Vector2H result;
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
		public static Vector2H operator -( Vector2H v )
		{
			Vector2H result;
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
		public static void Add( ref Vector2H v1, ref Vector2H v2, out Vector2H result )
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
		public static void Subtract( ref Vector2H v1, ref Vector2H v2, out Vector2H result )
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
		public static void Multiply( ref Vector2H v1, ref Vector2H v2, out Vector2H result )
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
		public static void Multiply( ref Vector2H v, HalfType s, out Vector2H result )
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
		public static void Multiply( HalfType s, ref Vector2H v, out Vector2H result )
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
		public static void Divide( ref Vector2H v1, ref Vector2H v2, out Vector2H result )
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
		public static void Divide( ref Vector2H v, HalfType s, out Vector2H result )
		{
			float invS = 1.0f / s;
			result.X = new HalfType( v.X * invS );
			result.Y = new HalfType( v.Y * invS );
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( HalfType s, ref Vector2H v, out Vector2H result )
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
		public static void Negate( ref Vector2H v, out Vector2H result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
		}

		//public static Vec3F Add( Vec3F v1, Vec3F v2 )
		//{
		//	Vec3F result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	return result;
		//}

		//public static Vec3F Subtract( Vec3F v1, Vec3F v2 )
		//{
		//	Vec3F result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	return result;
		//}

		//public static Vec3F Multiply( Vec3F v1, Vec3F v2 )
		//{
		//	Vec3F result;
		//	result.x = v1.x * v2.x;
		//	result.y = v1.y * v2.y;
		//	return result;
		//}

		//public static Vec3F Multiply( Vec3F v, float s )
		//{
		//	Vec3F result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	result.z = v.z * s;
		//	return result;
		//}

		//public static Vec3F Multiply( float s, Vec3F v )
		//{
		//	Vec3F result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	result.z = v.z * s;
		//	return result;
		//}

		//public static Vec3F Divide( Vec3F v1, Vec3F v2 )
		//{
		//	Vec3F result;
		//	result.x = v1.x / v2.x;
		//	result.y = v1.y / v2.y;
		//	result.z = v1.z / v2.z;
		//	return result;
		//}

		//public static Vec3F Divide( Vec3F v, float s )
		//{
		//	Vec3F result;
		//	float invS = 1.0f / s;
		//	result.x = v.x * invS;
		//	result.y = v.y * invS;
		//	result.z = v.z * invS;
		//	return result;
		//}

		//public static Vec3F Divide( float s, Vec3F v )
		//{
		//	Vec3F result;
		//	result.x = s / v.x;
		//	result.y = s / v.y;
		//	result.z = s / v.z;
		//	return result;
		//}

		//public static Vec3F Negate( Vec3F v )
		//{
		//	Vec3F result;
		//	result.x = -v.x;
		//	result.y = -v.y;
		//	result.z = -v.z;
		//	return result;
		//}


		/// <summary>
		/// Determines whether two given vectors are equal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are equal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Vector2H v1, Vector2H v2 )
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
		public static bool operator !=( Vector2H v1, Vector2H v2 )
		{
			return ( v1.X != v2.X || v1.Y != v2.Y );
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the X, Y, or Z component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, and 2 for the Z component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
		public unsafe HalfType this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( HalfType* v = &this.X )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( HalfType* v = &this.X )
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
		public static HalfType Dot( Vector2H v1, Vector2H v2 )
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
		public static HalfType Dot( ref Vector2H v1, ref Vector2H v2 )
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
		internal static void Dot( ref Vector2H v1, ref Vector2H v2, out HalfType result )
		{
			result = v1.X * v2.X + v1.Y * v2.Y;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public HalfType Dot( Vector2H v )
		{
			return X * v.X + Y * v.Y;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public HalfType Dot( ref Vector2H v )
		{
			return X * v.X + Y * v.Y;
		}

		///// <summary>
		///// Calculates the cross product of two vectors.
		///// </summary>
		///// <param name="v1">The first vector.</param>
		///// <param name="v2">The second vector.</param>
		///// <returns>The cross product of the two vectors.</returns>
		//public static Vector2H Cross( Vector2H v1, Vector2H v2 )
		//{
		//	return new Vector2H(
		//		v1.Y * v2.Z - v1.Z * v2.Y,
		//		v1.Z * v2.X - v1.X * v2.Z,
		//		v1.X * v2.Y - v1.Y * v2.X );
		//}

		///// <summary>
		///// Calculates the cross product of two vectors.
		///// </summary>
		///// <param name="v1">The first vector.</param>
		///// <param name="v2">The second vector.</param>
		///// <param name="result">When the method completes, contains the cross product of the two vectors.</param>
		//public static void Cross( ref Vector2H v1, ref Vector2H v2, out Vector2H result )
		//{
		//	result.X = v1.Y * v2.Z - v1.Z * v2.Y;
		//	result.Y = v1.Z * v2.X - v1.X * v2.Z;
		//	result.Z = v1.X * v2.Y - v1.Y * v2.X;
		//}

		///// <summary>
		///// Calculates the cross product of two vectors.
		///// </summary>
		///// <param name="v">The second vector.</param>
		///// <returns>The cross product of the two vectors.</returns>
		//public Vector2H Cross( Vector2H v )
		//{
		//	return new Vector2H(
		//		Y * v.Z - Z * v.Y,
		//		Z * v.X - X * v.Z,
		//		X * v.Y - Y * v.X );
		//}

		///// <summary>
		///// Calculates the cross product of two vectors.
		///// </summary>
		///// <param name="v">The second vector.</param>
		///// <param name="result">When the method completes, contains the cross product of the two vectors.</param>
		//public void Cross( ref Vector2H v, out Vector2H result )
		//{
		//	result.X = Y * v.Z - Z * v.Y;
		//	result.Y = Z * v.X - X * v.Z;
		//	result.Z = X * v.Y - Y * v.X;
		//}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector2H"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector2H"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Vector2H v, HalfType epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector2H"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector2H"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref Vector2H v, HalfType epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Calculates the length of the current instance of <see cref="Vector2H"/>.
		/// </summary>
		/// <returns>The length of the current instance of <see cref="Vector2H"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public HalfType Length()
		{
			return new HalfType( MathEx.Sqrt( X * X + Y * Y ) );
		}

		/// <summary>
		/// Calculates the squared length of the current instance of <see cref="Vector2H"/>.
		/// </summary>
		/// <returns>The squared length of the current instance of <see cref="Vector2H"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public HalfType LengthSquared()
		{
			return X * X + Y * Y;
		}

		/// <summary>
		/// Restricts the current instance of <see cref="Vector2H"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector2H"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Clamp( Vector2H min, Vector2H max )
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
		/// Restricts the current instance of <see cref="Vector2H"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector2H"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2H GetClamp( Vector2H min, Vector2H max )
		{
			var r = this;
			r.Clamp( min, max );
			return r;
		}

		///// <summary>
		///// Converts the current instance of <see cref="Vector2H"/> into a unit vector.
		///// </summary>
		///// <returns>Returns the length of the current instance of <see cref="Vector2H"/>.</returns>
		//public HalfType Normalize()
		//{
		//	HalfType sqrLength = X * X + Y * Y;
		//	HalfType invLength = new HalfType( MathEx.InvSqrt( sqrLength ) );
		//	X *= invLength;
		//	Y *= invLength;
		//	return invLength * sqrLength;
		//}

		///// <summary>
		///// Converts a vector into a unit vector. 
		///// </summary>
		///// <param name="v">The vector to normalize.</param>
		///// <returns>The normalized vector.</returns>
		//public static Vector2H Normalize( Vector2H v )
		//{
		//	float invLength = MathEx.InvSqrt( v.X * v.X + v.Y * v.Y + v.Z * v.Z );
		//	return new Vector2H( v.X * invLength, v.Y * invLength, v.Z * invLength );
		//}

		///// <summary>
		///// Converts a vector into a unit vector.
		///// </summary>
		///// <param name="v">The vector to normalize.</param>
		///// <param name="result">When the method completes, contains the normalized vector.</param>
		//public static void Normalize( ref Vector2H v, out Vector2H result )
		//{
		//	HalfType invLength = new HalfType( MathEx.InvSqrt( v.X * v.X + v.Y * v.Y + v.Z * v.Z ) );
		//	result.X = v.X * invLength;
		//	result.Y = v.Y * invLength;
		//	result.Z = v.Z * invLength;
		//}

		///// <summary>
		///// Converts the current instance of <see cref="Vector2H"/> into a unit vector and returns the normalized vector.
		///// </summary>
		///// <returns>The normalized instance of <see cref="Vector2H"/>.</returns>
		//public Vector2H GetNormalize()
		//{
		//	float invLength = MathEx.InvSqrt( X * X + Y * Y + Z * Z );
		//	return new Vector2H( X * invLength, Y * invLength, Z * invLength );
		//}

		/// <summary>
		/// Performs a linear interpolation between two vectors based on the given weighting.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="v2"/>.</param>
		/// <returns>The interpolated vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Lerp( Vector2H v1, Vector2H v2, HalfType amount )
		{
			Vector2H result;
			result.X = v1.X + ( v2.X - v1.X ) * amount;
			result.Y = v1.Y + ( v2.Y - v1.Y ) * amount;
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
		public static void Lerp( ref Vector2H v1, ref Vector2H v2, HalfType amount, out Vector2H result )
		{
			result.X = v1.X + ( v2.X - v1.X ) * amount;
			result.Y = v1.Y + ( v2.Y - v1.Y ) * amount;
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector2H"/> between 0 and 1.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Saturate()
		{
			MathEx.Saturate( ref X );
			MathEx.Saturate( ref Y );
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector2H"/> between 0 and 1 and returns the saturated value.
		/// </summary>
		/// <returns>The saturated value.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2H GetSaturate()
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
		public static HalfType Distance( Vector2H v1, Vector2H v2 )
		{
			Subtract( ref v1, ref v2, out Vector2H result );
			return result.Length();
		}

		/// <summary>
		/// Calculates the distance between two vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The distance between two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static HalfType Distance( ref Vector2H v1, ref Vector2H v2 )
		{
			Subtract( ref v1, ref v2, out Vector2H result );
			return result.Length();
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector2H"/> into the equivalent <see cref="Vector2"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector2"/> structure.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector2 ToVector2()
		{
			Vector2 result;
			result.X = X;
			result.Y = Y;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector2H"/> into the equivalent <see cref="Vector2F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector2F"/> structure.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector2F ToVector2F()
		{
			Vector2F result;
			result.X = X;
			result.Y = Y;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector2H"/> into the equivalent <see cref="Vector2I"/> structure.
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
		/// Converts the current instance of <see cref="Vector2F"/> into the equivalent <see cref="Vector2H"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector2H"/> structure.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector2H ToVector2H()
		{
			Vector2H result;
			result.X = new HalfType( X );
			result.Y = new HalfType( Y );
			return result;
		}

#if !DISABLE_IMPLICIT
		/// <summary>
		/// Implicit conversion from <see cref="Vector2H"/> type to <see cref="Vector2"/> type for given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Vector2( Vector2H v )
		{
			return v.ToVector2();
		}

		/// <summary>
		/// Implicit conversion from <see cref="Vector2H"/> type to <see cref="Vector2F"/> type for given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Vector2F( Vector2H v )
		{
			return v.ToVector2F();
		}

		/// <summary>
		/// Implicit conversion from <see cref="Vector2F"/> type to <see cref="Vector2H"/> type for given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Vector2H( Vector2F v )
		{
			return v.ToVector2H();
		}
#endif

		/// <summary>
		/// Chooses one of two vectors depending on the <paramref name="pick1"/> value.
		/// </summary>
		/// <param name="v1">The first vector to choose.</param>
		/// <param name="v2">The second vector to choose.</param>
		/// <param name="pick1">If this value is true, the method chooses the virst vector, otherwise it chooses the second one.</param>
		/// <returns>The selected vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Select( Vector2H v1, Vector2H v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/// <summary>
		/// Rounds the current instance of <see cref="Vector2H"/> towards zero for each component in a vector.
		/// </summary>
		/// <param name="v"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Truncate()
		{
			X = (int)X;
			Y = (int)Y;
		}

		/// <summary>
		/// Rounds a given vector towards zero for each component in it and returns the truncated vector.
		/// </summary>
		/// <param name="v">The vector to truncate.</param>
		/// <returns>The truncated vector</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2H GetTruncate( Vector2H v )
		{
			return new Vector2H( (int)v.X, (int)v.Y );
		}

		/// <summary>
		/// Determines whether any component of a given vector is unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if any component of the specified vector is unequal to the zero; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool AnyNonZero( Vector2H v )
		{
			return v != Zero;
		}

		/// <summary>
		/// Determines whether all components of a given vector are unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if all components of the specified vector are unequal to the zero; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool AllNonZero( Vector2H v )
		{
			return v.X != 0 && v.Y != 0;
		}

		/// <summary>
		/// Returns a vector whose elements are the absolute values of each of the specified vector's components.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The absolute value vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Abs( Vector2H v )
		{
			return new Vector2H( Math.Abs( v.X ), Math.Abs( v.Y ) );
		}

		/// <summary>
		/// Calculates the arc-cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose cosines are equal to the
		/// corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Acos( Vector2H v )
		{
			return new Vector2H( MathEx.Acos( v.X ), MathEx.Acos( v.Y ) );
		}

		/// <summary>
		/// Calculates the arc-sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose sines are equal to the
		/// corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Asin( Vector2H v )
		{
			return new Vector2H( MathEx.Asin( v.X ), MathEx.Asin( v.Y ) );
		}

		/// <summary>
		/// Calculates the arc-tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are equal to the
		/// corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Atan( Vector2H v )
		{
			return new Vector2H( MathEx.Atan( v.X ), MathEx.Atan( v.Y ) );
		}

		/// <summary>
		/// Returns the vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.
		/// </summary>
		/// <param name="y">The first vector.</param>
		/// <param name="x">The second vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Atan2( Vector2H y, Vector2H x )
		{
			return new Vector2H( MathEx.Atan2( y.X, x.X ), MathEx.Atan2( y.Y, x.Y ) );
		}

		/// <summary>
		/// Calculates the cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the cosines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Cos( Vector2H v )
		{
			return new Vector2H( MathEx.Cos( v.X ), MathEx.Cos( v.Y ) );
		}

		/// <summary>
		/// Calculates the hyperbolic cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic cosines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Cosh( Vector2H v )
		{
			return new Vector2H( MathEx.Cosh( v.X ), MathEx.Cosh( v.Y ) );
		}

		/// <summary>
		/// Returns the vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Exp( Vector2H v )
		{
			return new Vector2H( MathEx.Exp( v.X ), MathEx.Exp( v.Y ) );
		}

		/// <summary>
		/// Calculates the natural logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the natural logarithms of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Log( Vector2H v )
		{
			return new Vector2H( MathEx.Log( v.X ), MathEx.Log( v.Y ) );
		}

		/// <summary>
		/// Calculates the base 10 logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the base 10 logarithms of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Log10( Vector2H v )
		{
			return new Vector2H( MathEx.Log10( v.X ), MathEx.Log10( v.Y ) );
		}

		/// <summary>
		/// Returns a vector containing the largest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the largest components of the specified vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Max( Vector2H v1, Vector2H v2 )
		{
			return new Vector2H( Math.Max( v1.X, v2.X ), Math.Max( v1.Y, v2.Y ) );
		}

		/// <summary>
		/// Returns a vector containing the smallest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the smallest components of the specified vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Min( Vector2H v1, Vector2H v2 )
		{
			return new Vector2H( Math.Min( v1.X, v2.X ), Math.Min( v1.Y, v2.Y ) );
		}

		/// <summary>
		/// Returns the vector which contains the components of the first specified vector raised to power of the numbers which are equal to the corresponding components of the second specified vector.
		/// </summary>
		/// <param name="x">The first vector.</param>
		/// <param name="y">The second vector.</param>
		/// <returns>The vector which contains the components of the first specified vector raised to power of
		/// the numbers which are equal to the corresponding components of the second specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Pow( Vector2H x, Vector2H y )
		{
			return new Vector2H( MathEx.Pow( x.X, y.X ), MathEx.Pow( x.Y, y.Y ) );
		}

		/// <summary>
		/// Calculates the sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the sines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Sin( Vector2H v )
		{
			return new Vector2H( MathEx.Sin( v.X ), MathEx.Sin( v.Y ) );
		}

		/// <summary>
		/// Calculates the hyperbolic sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic sines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Sinh( Vector2H v )
		{
			return new Vector2H( MathEx.Sinh( v.X ), MathEx.Sinh( v.Y ) );
		}

		/// <summary>
		/// Calculates the square root of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the square root of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Sqrt( Vector2H v )
		{
			return new Vector2H( MathEx.Sqrt( v.X ), MathEx.Sqrt( v.Y ) );
		}

		/// <summary>
		/// Calculates the tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the tangents of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Tan( Vector2H v )
		{
			return new Vector2H( MathEx.Tan( v.X ), MathEx.Tan( v.Y ) );
		}

		/// <summary>
		/// Calculates the hyperbolic tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic tangents of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector2H Tanh( Vector2H v )
		{
			return new Vector2H( MathEx.Tanh( v.X ), MathEx.Tanh( v.Y ) );
		}

		/// <summary>
		/// Returns the value of the smallest component of the current instance of <see cref="Vector2H"/>.
		/// </summary>
		/// <returns>The value of the smallest component of the current instance of <see cref="Vector2H"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public HalfType MinComponent()
		{
			return new HalfType( Math.Min( X, Y ) );
		}

		/// <summary>
		/// Returns the value of the largest component of the current instance of <see cref="Vector2H"/>.
		/// </summary>
		/// <returns>The value of the largest component of the current instance of <see cref="Vector2H"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public HalfType MaxComponent()
		{
			return new HalfType( Math.Max( X, Y ) );
		}
	}
}
