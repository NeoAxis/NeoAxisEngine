// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Vec4F> ) )]
	/// <summary>
	/// A structure encapsulating four single precision floating point values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Vector4F
	{
		/// <summary>
		/// The X component of the vector.
		/// </summary>
		public float X;
		/// <summary>
		/// The Y component of the vector.
		/// </summary>
		public float Y;
		/// <summary>
		/// The Z component of the vector.
		/// </summary>
		public float Z;
		/// <summary>
		/// The W component of the vector.
		/// </summary>
		public float W;

		/// <summary>
		/// Returns the vector (0,0,0,0).
		/// </summary>
		public static readonly Vector4F Zero = new Vector4F( 0.0f, 0.0f, 0.0f, 0.0f );
		/// <summary>
		/// Returns the vector (1,1,1,1).
		/// </summary>
		public static readonly Vector4F One = new Vector4F( 1.0f, 1.0f, 1.0f, 1.0f );

		/// <summary>
		/// Constructs a vector with another given vector.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector4F( Vector4F source )
		{
			this.X = source.X;
			this.Y = source.Y;
			this.Z = source.Z;
			this.W = source.W;
		}

		/// <summary>
		/// Constructs a vector with the given individual elements.
		/// </summary>
		/// <param name="w">W component.</param>
		/// <param name="x">X component.</param>
		/// <param name="y">Y component.</param>
		/// <param name="z">Z component.</param>
		public Vector4F( float x, float y, float z, float w )
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		/// <summary>
		/// Constructs a <see cref="Vector4F"/> from the given <see cref="Vector3F"/> and a W component.
		/// </summary>
		/// <param name="v">The vector to use as the X, Y and Z components.</param>
		/// <param name="w">The W component.</param>
		public Vector4F( Vector3F v, float w )
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
			this.W = w;
		}

		/// <summary>
		/// Converts a string representation of a vector into the equivalent <see cref="Vector4F"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the vector (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Vector4F"/> structure.</returns>
		[AutoConvertType]
		public static Vector4F Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (x y z w).", text ) );

			try
			{
				return new Vector4F(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ),
					float.Parse( vals[ 2 ] ),
					float.Parse( vals[ 3 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector4F"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector4F"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			//!!!!slowly? везде так
			return ToString( 8 );
			//return string.Format( "{0} {1} {2} {3}", x, y, z, w );
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector4F"/> with a given precision.
		/// </summary>
		/// <param name="precision">The precision value.</param>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector4F"/>.</returns>
		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "}";
			return string.Format( format, X, Y, Z, W );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Vector4F"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Vector4F"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Vector4F"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Vector4F && this == (Vector4F)obj );
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return ( X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode() );
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="v1">The first vector to add.</param>
		/// <param name="v2">The second vector to add.</param>
		/// <returns>The sum of the two vectors.</returns>
		public static Vector4F operator +( Vector4F v1, Vector4F v2 )
		{
			Vector4F result;
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
			result.Z = v1.Z + v2.Z;
			result.W = v1.W + v2.W;
			return result;
		}

		/// <summary>
		/// Subtracts two vectors.
		/// </summary>
		/// <param name="v1">The vector to subtract from.</param>
		/// <param name="v2">The vector to be subtracted from another vector.</param>
		/// <returns>The difference of the two vectors.</returns>
		public static Vector4F operator -( Vector4F v1, Vector4F v2 )
		{
			Vector4F result;
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
			result.Z = v1.Z - v2.Z;
			result.W = v1.W - v2.W;
			return result;
		}

		/// <summary>
		/// Multiplies two vectors together.
		/// </summary>
		/// <param name="v1">The first vector to multiply.</param>
		/// <param name="v2">The second vector to multiply.</param>
		/// <returns>The product vector.</returns>
		public static Vector4F operator *( Vector4F v1, Vector4F v2 )
		{
			Vector4F result;
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
			result.Z = v1.Z * v2.Z;
			result.W = v1.W * v2.W;
			return result;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to scale.</param>
		/// <param name="s">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4F operator *( Vector4F v, float s )
		{
			Vector4F result;
			result.X = v.X * s;
			result.Y = v.Y * s;
			result.Z = v.Z * s;
			result.W = v.W * s;
			return result;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4F operator *( float s, Vector4F v )
		{
			Vector4F result;
			result.X = s * v.X;
			result.Y = s * v.Y;
			result.Z = s * v.Z;
			result.W = s * v.W;
			return result;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4F operator /( Vector4F v1, Vector4F v2 )
		{
			Vector4F result;
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
			result.Z = v1.Z / v2.Z;
			result.W = v1.W / v2.W;
			return result;
		}

		/// <summary>
		/// Divides a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to divide.</param>
		/// <param name="s">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4F operator /( Vector4F v, float s )
		{
			Vector4F result;
			float invS = 1.0f / s;
			result.X = v.X * invS;
			result.Y = v.Y * invS;
			result.Z = v.Z * invS;
			result.W = v.W * invS;
			return result;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4F operator /( float s, Vector4F v )
		{
			Vector4F result;
			result.X = s / v.X;
			result.Y = s / v.Y;
			result.Z = s / v.Z;
			result.W = s / v.W;
			return result;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <returns>A vector facing in the opposite direction.</returns>
		public static Vector4F operator -( Vector4F v )
		{
			Vector4F result;
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
			result.W = -v.W;
			return result;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="v1">The first vector to add.</param>
		/// <param name="v2">The second vector to add.</param>
		/// <param name="result">When the method completes, contains the sum of the two vectors.</param>
		public static void Add( ref Vector4F v1, ref Vector4F v2, out Vector4F result )
		{
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
			result.Z = v1.Z + v2.Z;
			result.W = v1.W + v2.W;
		}

		/// <summary>
		/// Subtracts two vectors.
		/// </summary>
		/// <param name="v1">The vector from which to subtract.</param>
		/// <param name="v2">The vector which should be subtracted from another.</param>
		/// <param name="result">When the method completes, contains the difference of the two vectors.</param>
		public static void Subtract( ref Vector4F v1, ref Vector4F v2, out Vector4F result )
		{
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
			result.Z = v1.Z - v2.Z;
			result.W = v1.W - v2.W;
		}

		/// <summary>
		/// Multiplies two vectors together.
		/// </summary>
		/// <param name="v1">The first vector to multiply.</param>
		/// <param name="v2">The second vector to multiply.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( ref Vector4F v1, ref Vector4F v2, out Vector4F result )
		{
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
			result.Z = v1.Z * v2.Z;
			result.W = v1.W * v2.W;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to scale.</param>
		/// <param name="s">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( ref Vector4F v, float s, out Vector4F result )
		{
			result.X = v.X * s;
			result.Y = v.Y * s;
			result.Z = v.Z * s;
			result.W = v.W * s;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( float s, ref Vector4F v, out Vector4F result )
		{
			result.X = v.X * s;
			result.Y = v.Y * s;
			result.Z = v.Z * s;
			result.W = v.W * s;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( ref Vector4F v1, ref Vector4F v2, out Vector4F result )
		{
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
			result.Z = v1.Z / v2.Z;
			result.W = v1.W / v2.W;
		}

		/// <summary>
		/// Divides a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to divide.</param>
		/// <param name="s">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( ref Vector4F v, float s, out Vector4F result )
		{
			float invS = 1.0f / s;
			result.X = v.X * invS;
			result.Y = v.Y * invS;
			result.Z = v.Z * invS;
			result.W = v.W * invS;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( float s, ref Vector4F v, out Vector4F result )
		{
			result.X = s / v.X;
			result.Y = s / v.Y;
			result.Z = s / v.Z;
			result.W = s / v.W;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <param name="result">When the method completes, contains a vector facing in the opposite direction.</param>
		public static void Negate( ref Vector4F v, out Vector4F result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
			result.W = -v.W;
		}

		//public static Vec4F Add( Vec4F v1, Vec4F v2 )
		//{
		//	Vec4F result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	result.z = v1.z + v2.z;
		//	result.w = v1.w + v2.w;
		//	return result;
		//}

		//public static Vec4F Subtract( Vec4F v1, Vec4F v2 )
		//{
		//	Vec4F result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	result.z = v1.z - v2.z;
		//	result.w = v1.w - v2.w;
		//	return result;
		//}

		//public static Vec4F Multiply( Vec4F v1, Vec4F v2 )
		//{
		//	Vec4F result;
		//	result.x = v1.x * v2.x;
		//	result.y = v1.y * v2.y;
		//	result.z = v1.z * v2.z;
		//	result.w = v1.w * v2.w;
		//	return result;
		//}

		//public static Vec4F Multiply( Vec4F v, float s )
		//{
		//	Vec4F result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	result.z = v.z * s;
		//	result.w = v.w * s;
		//	return result;
		//}

		//public static Vec4F Multiply( float s, Vec4F v )
		//{
		//	Vec4F result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	result.z = v.z * s;
		//	result.w = v.w * s;
		//	return result;
		//}

		//public static Vec4F Divide( Vec4F v1, Vec4F v2 )
		//{
		//	Vec4F result;
		//	result.x = v1.x / v2.x;
		//	result.y = v1.y / v2.y;
		//	result.z = v1.z / v2.z;
		//	result.w = v1.w / v2.w;
		//	return result;
		//}

		//public static Vec4F Divide( Vec4F v, float s )
		//{
		//	Vec4F result;
		//	float invS = 1.0f / s;
		//	result.x = v.x * invS;
		//	result.y = v.y * invS;
		//	result.z = v.z * invS;
		//	result.w = v.w * invS;
		//	return result;
		//}

		//public static Vec4F Divide( float s, Vec4F v )
		//{
		//	Vec4F result;
		//	result.x = s / v.x;
		//	result.y = s / v.y;
		//	result.z = s / v.z;
		//	result.w = s / v.w;
		//	return result;
		//}

		//public static Vec4F Negate( Vec4F v )
		//{
		//	Vec4F result;
		//	result.x = -v.x;
		//	result.y = -v.y;
		//	result.z = -v.z;
		//	result.w = -v.w;
		//	return result;
		//}


		/// <summary>
		/// Determines whether two given vectors are equal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are equal; False otherwise.</returns>
		public static bool operator ==( Vector4F v1, Vector4F v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W );
		}

		/// <summary>
		/// Determines whether two given vectors are unequal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are unequal; False otherwise.</returns>
		public static bool operator !=( Vector4F v1, Vector4F v2 )
		{
			return ( v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z || v1.W != v2.W );
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
		public unsafe float this[ int index ]
		{
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( float* v = &this.X )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( float* v = &this.X )
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
		public static float Dot( Vector4F v1, Vector4F v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		public static float Dot( ref Vector4F v1, ref Vector4F v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		public float Dot( Vector4F v )
		{
			return X * v.X + Y * v.Y + Z * v.Z + W * v.W;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		public float Dot( ref Vector4F v )
		{
			return X * v.X + Y * v.Y + Z * v.Z + W * v.W;
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector4F"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector4F"/>; False otherwise.</returns>
		public bool Equals( Vector4F v, float epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			if( Math.Abs( Z - v.Z ) > epsilon )
				return false;
			if( Math.Abs( W - v.W ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector4F"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector4F"/>; False otherwise.</returns>
		public bool Equals( ref Vector4F v, float epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			if( Math.Abs( Z - v.Z ) > epsilon )
				return false;
			if( Math.Abs( W - v.W ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Calculates the length of the current instance of <see cref="Vector4F"/>.
		/// </summary>
		/// <returns>The length of the current instance of <see cref="Vector4F"/>.</returns>
		public float Length()
		{
			return MathEx.Sqrt( X * X + Y * Y + Z * Z + W * W );
		}

		/// <summary>
		/// Calculates the squared length of the current instance of <see cref="Vector4F"/>.
		/// </summary>
		/// <returns>The squared length of the current instance of <see cref="Vector4F"/>.</returns>
		public float LengthSquared()
		{
			return X * X + Y * Y + Z * Z + W * W;
		}

		void Clamp( Vector4F min, Vector4F max )
		{
			if( X < min.X )
				X = min.X;
			else if( X > max.X )
				X = max.X;

			if( Y < min.Y )
				Y = min.Y;
			else if( Y > max.Y )
				Y = max.Y;

			if( Z < min.Z )
				Z = min.Z;
			else if( Z > max.Z )
				Z = max.Z;

			if( W < min.W )
				W = min.W;
			else if( W > max.W )
				W = max.W;
		}

		/// <summary>
		/// Restricts the current instance of <see cref="Vector4F"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector4F"/>.</returns>
		public Vector4F GetClamp( Vector4F min, Vector4F max )
		{
			var r = this;
			r.Clamp( min, max );
			return r;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4F"/> into a unit vector.
		/// </summary>
		/// <returns>Returns the length of the current instance of <see cref="Vector4F"/>.</returns>
		public float Normalize()
		{
			float sqrLength = X * X + Y * Y + Z * Z + W * W;
			float invLength = MathEx.InvSqrt( sqrLength );
			X *= invLength;
			Y *= invLength;
			Z *= invLength;
			W *= invLength;
			return invLength * sqrLength;
		}

		/// <summary>
		/// Converts a vector into a unit vector. 
		/// </summary>
		/// <param name="v">The vector to normalize.</param>
		/// <returns>The normalized vector.</returns>
		public static Vector4F Normalize( Vector4F v )
		{
			float invLength = MathEx.InvSqrt( v.X * v.X + v.Y * v.Y + v.Z * v.Z + v.W * v.W );
			return new Vector4F( v.X * invLength, v.Y * invLength, v.Z * invLength, v.W * invLength );
		}

		/// <summary>
		/// Converts a vector into a unit vector.
		/// </summary>
		/// <param name="v">The vector to normalize.</param>
		/// <param name="result">When the method completes, contains the normalized vector.</param>
		public static void Normalize( ref Vector4F v, out Vector4F result )
		{
			float invLength = MathEx.InvSqrt( v.X * v.X + v.Y * v.Y + v.Z * v.Z + v.W * v.W );
			result.X = v.X * invLength;
			result.Y = v.Y * invLength;
			result.Z = v.Z * invLength;
			result.W = v.W * invLength;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4F"/> into a unit vector and returns the normalized vector.
		/// </summary>
		/// <returns>The normalized instance of <see cref="Vector4F"/>.</returns>
		public Vector4F GetNormalize()
		{
			float invLength = MathEx.InvSqrt( X * X + Y * Y + Z * Z + W * W );
			return new Vector4F( X * invLength, Y * invLength, Z * invLength, W * invLength );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4F"/> into an instance of <see cref="Vector2F"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
		[AutoConvertType]
		public Vector2F ToVector2F()
		{
			Vector2F result;
			result.X = X;
			result.Y = Y;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4F"/> into an instance of <see cref="Vector3F"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
		[AutoConvertType]
		public Vector3F ToVector3F()
		{
			Vector3F result;
			result.X = X;
			result.Y = Y;
			result.Z = Z;
			return result;
		}

		/// <summary>
		/// Performs a linear interpolation between two vectors based on the given weighting.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="v2"/>.</param>
		/// <returns>The interpolated vector.</returns>
		public static Vector4F Lerp( Vector4F v1, Vector4F v2, float amount )
		{
			Vector4F result;
			result.X = v1.X + ( ( v2.X - v1.X ) * amount );
			result.Y = v1.Y + ( ( v2.Y - v1.Y ) * amount );
			result.Z = v1.Z + ( ( v2.Z - v1.Z ) * amount );
			result.W = v1.W + ( ( v2.W - v1.W ) * amount );
			return result;
		}

		/// <summary>
		/// Performs a linear interpolation between two vectors based on the given weighting.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="v2"/>.</param>
		/// <param name="result">When the method completes, contains the linear interpolation of the two vectors.</param>
		public static void Lerp( ref Vector4F v1, ref Vector4F v2, float amount, out Vector4F result )
		{
			result.X = v1.X + ( ( v2.X - v1.X ) * amount );
			result.Y = v1.Y + ( ( v2.Y - v1.Y ) * amount );
			result.Z = v1.Z + ( ( v2.Z - v1.Z ) * amount );
			result.W = v1.W + ( ( v2.W - v1.W ) * amount );
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector4F"/> between 0 and 1.
		/// </summary>
		public void Saturate()
		{
			MathEx.Saturate( ref X );
			MathEx.Saturate( ref Y );
			MathEx.Saturate( ref Z );
			MathEx.Saturate( ref W );
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector4F"/> between 0 and 1 and returns the saturated value.
		/// </summary>
		/// <returns>The saturated value.</returns>
		public Vector4F GetSaturate()
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
		public static float Distance( Vector4F v1, Vector4F v2 )
		{
			Subtract( ref v1, ref v2, out Vector4F result );
			return result.Length();
		}

		/// <summary>
		/// Calculates the distance between two vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The distance between two vectors.</returns>
		public static float Distance( ref Vector4F v1, ref Vector4F v2 )
		{
			Subtract( ref v1, ref v2, out Vector4F result );
			return result.Length();
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4F"/> into the equivalent <see cref="Vector4"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector4"/> structure.</returns>
		[AutoConvertType]
		public Vector4 ToVector4()
		{
			Vector4 result;
			result.X = X;
			result.Y = Y;
			result.Z = Z;
			result.W = W;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4F"/> into the equivalent <see cref="Vector4I"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector4I"/> structure.</returns>
		[AutoConvertType]
		public Vector4I ToVector4I()
		{
			Vector4I result;
			result.X = (int)X;
			result.Y = (int)Y;
			result.Z = (int)Z;
			result.W = (int)W;
			return result;
		}

		/// <summary>
		/// Converts each component of the current instance of <see cref="Vector4F"/> into the
		/// component of the <see cref="ColorValue"/> structure: X to red component,
		/// Y to green component, Z to blue component, W to alpha component.
		/// </summary>
		/// <returns>The equivalent <see cref="ColorValue"/> structure.</returns>
		public ColorValue ToColorValue()
		{
			ColorValue result;
			result.Red = X;
			result.Green = Y;
			result.Blue = Z;
			result.Alpha = W;
			return result;
		}

#if !DISABLE_IMPLICIT
		/// <summary>
		/// Implicit conversion from <see cref="Vector4F"/> type to <see cref="Vector4"/> type for given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		public static implicit operator Vector4( Vector4F v )
		{
			return new Vector4( v );
		}
#endif

		/// <summary>
		/// Chooses one of two vectors depending on the <paramref name="pick1"/> value.
		/// </summary>
		/// <param name="v1">The first vector to choose.</param>
		/// <param name="v2">The second vector to choose.</param>
		/// <param name="pick1">If this value is true, the method chooses the virst vector, otherwise it chooses the second one.</param>
		/// <returns>The selected vector.</returns>
		public static Vector4F Select( Vector4F v1, Vector4F v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/// <summary>
		/// Rounds the current instance of <see cref="Vector4F"/> towards zero for each component in a vector.
		/// </summary>
		/// <param name="v"></param>
		public void Truncate( Vector4F v )
		{
			X = (int)X;
			Y = (int)Y;
			Z = (int)Z;
			W = (int)W;
		}

		/// <summary>
		/// Rounds a given vector towards zero for each component in it and returns the truncated vector.
		/// </summary>
		/// <param name="v">The vector to truncate.</param>
		/// <returns>The truncated vector</returns>
		public Vector4F GetTruncate( Vector4F v )
		{
			return new Vector4F( (int)v.X, (int)v.Y, (int)v.Z, (int)v.W );
		}

		/// <summary>
		/// Determines whether any component of a given vector is unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if any component of the specified vector is unequal to the zero; otherwise, False.</returns>
		public static bool AnyNonZero( Vector4F v )
		{
			return v != Zero;
		}

		/// <summary>
		/// Determines whether all components of a given vector are unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if all components of the specified vector are unequal to the zero; otherwise, False.</returns>
		public static bool AllNonZero( Vector4F v )
		{
			return v.X != 0 && v.Y != 0 && v.Z != 0 && v.W != 0;
		}

		/// <summary>
		/// Returns a vector whose elements are the absolute values of each of the specified vector's components.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The absolute value vector.</returns>
		public static Vector4F Abs( Vector4F v )
		{
			return new Vector4F( Math.Abs( v.X ), Math.Abs( v.Y ), Math.Abs( v.Z ), Math.Abs( v.W ) );
		}

		/// <summary>
		/// Calculates the arc-cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose cosines are equal to the
		/// corresponding components in the specified vector.</returns>
		public static Vector4F Acos( Vector4F v )
		{
			return new Vector4F( MathEx.Acos( v.X ), MathEx.Acos( v.Y ), MathEx.Acos( v.Z ), MathEx.Acos( v.W ) );
		}

		/// <summary>
		/// Calculates the arc-sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose sines are equal to the
		/// corresponding components in the specified vector.</returns>
		public static Vector4F Asin( Vector4F v )
		{
			return new Vector4F( MathEx.Asin( v.X ), MathEx.Asin( v.Y ), MathEx.Asin( v.Z ), MathEx.Asin( v.W ) );
		}

		/// <summary>
		/// Calculates the arc-tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are equal to the
		/// corresponding components in the specified vector.</returns>
		public static Vector4F Atan( Vector4F v )
		{
			return new Vector4F( MathEx.Atan( v.X ), MathEx.Atan( v.Y ), MathEx.Atan( v.Z ), MathEx.Atan( v.W ) );
		}

		/// <summary>
		/// Returns the vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.
		/// </summary>
		/// <param name="y">The first vector.</param>
		/// <param name="x">The second vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.</returns>
		public static Vector4F Atan2( Vector4F y, Vector4F x )
		{
			return new Vector4F( MathEx.Atan2( y.X, x.X ), MathEx.Atan2( y.Y, x.Y ), MathEx.Atan2( y.Z, x.Z ), MathEx.Atan2( y.W, x.W ) );
		}

		/// <summary>
		/// Calculates the cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the cosines of the corresponding components in the specified vector.</returns>
		public static Vector4F Cos( Vector4F v )
		{
			return new Vector4F( MathEx.Cos( v.X ), MathEx.Cos( v.Y ), MathEx.Cos( v.Z ), MathEx.Cos( v.W ) );
		}

		/// <summary>
		/// Calculates the hyperbolic cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic cosines of the corresponding components in the specified vector.</returns>
		public static Vector4F Cosh( Vector4F v )
		{
			return new Vector4F( MathEx.Cosh( v.X ), MathEx.Cosh( v.Y ), MathEx.Cosh( v.Z ), MathEx.Cosh( v.W ) );
		}

		/// <summary>
		/// Returns the vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.</returns>
		public static Vector4F Exp( Vector4F v )
		{
			return new Vector4F( MathEx.Exp( v.X ), MathEx.Exp( v.Y ), MathEx.Exp( v.Z ), MathEx.Exp( v.W ) );
		}

		/// <summary>
		/// Calculates the natural logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the natural logarithms of the corresponding components in the specified vector.</returns>
		public static Vector4F Log( Vector4F v )
		{
			return new Vector4F( MathEx.Log( v.X ), MathEx.Log( v.Y ), MathEx.Log( v.Z ), MathEx.Log( v.W ) );
		}

		/// <summary>
		/// Calculates the base 10 logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the base 10 logarithms of the corresponding components in the specified vector.</returns>
		public static Vector4F Log10( Vector4F v )
		{
			return new Vector4F( MathEx.Log10( v.X ), MathEx.Log10( v.Y ), MathEx.Log10( v.Z ), MathEx.Log10( v.W ) );
		}

		/// <summary>
		/// Returns a vector containing the largest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the largest components of the specified vectors.</returns>
		public static Vector4F Max( Vector4F v1, Vector4F v2 )
		{
			return new Vector4F( Math.Max( v1.X, v2.X ), Math.Max( v1.Y, v2.Y ), Math.Max( v1.Z, v2.Z ), Math.Max( v1.W, v2.W ) );
		}

		/// <summary>
		/// Returns a vector containing the smallest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the smallest components of the specified vectors.</returns>
		public static Vector4F Min( Vector4F v1, Vector4F v2 )
		{
			return new Vector4F( Math.Min( v1.X, v2.X ), Math.Min( v1.Y, v2.Y ), Math.Min( v1.Z, v2.Z ), Math.Min( v1.W, v2.W ) );
		}

		/// <summary>
		/// Returns the vector which contains the components of the first specified vector raised to power of the numbers which are equal to the corresponding components of the second specified vector.
		/// </summary>
		/// <param name="x">The first vector.</param>
		/// <param name="y">The second vector.</param>
		/// <returns>The vector which contains the components of the first specified vector raised to power of
		/// the numbers which are equal to the corresponding components of the second specified vector.</returns>
		public static Vector4F Pow( Vector4F x, Vector4F y )
		{
			return new Vector4F( MathEx.Pow( x.X, y.X ), MathEx.Pow( x.Y, y.Y ), MathEx.Pow( x.Z, y.Z ), MathEx.Pow( x.W, y.W ) );
		}

		/// <summary>
		/// Calculates the sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the sines of the corresponding components in the specified vector.</returns>
		public static Vector4F Sin( Vector4F v )
		{
			return new Vector4F( MathEx.Sin( v.X ), MathEx.Sin( v.Y ), MathEx.Sin( v.Z ), MathEx.Sin( v.W ) );
		}

		/// <summary>
		/// Calculates the hyperbolic sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic sines of the corresponding components in the specified vector.</returns>
		public static Vector4F Sinh( Vector4F v )
		{
			return new Vector4F( MathEx.Sinh( v.X ), MathEx.Sinh( v.Y ), MathEx.Sinh( v.Z ), MathEx.Sinh( v.W ) );
		}

		/// <summary>
		/// Calculates the square root of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the square root of the corresponding components in the specified vector.</returns>
		public static Vector4F Sqrt( Vector4F v )
		{
			return new Vector4F( MathEx.Sqrt( v.X ), MathEx.Sqrt( v.Y ), MathEx.Sqrt( v.Z ), MathEx.Sqrt( v.W ) );
		}

		/// <summary>
		/// Calculates the tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the tangents of the corresponding components in the specified vector.</returns>
		public static Vector4F Tan( Vector4F v )
		{
			return new Vector4F( MathEx.Tan( v.X ), MathEx.Tan( v.Y ), MathEx.Tan( v.Z ), MathEx.Tan( v.W ) );
		}

		/// <summary>
		/// Calculates the hyperbolic tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic tangents of the corresponding components in the specified vector.</returns>
		public static Vector4F Tanh( Vector4F v )
		{
			return new Vector4F( MathEx.Tanh( v.X ), MathEx.Tanh( v.Y ), MathEx.Tanh( v.Z ), MathEx.Tanh( v.W ) );
		}

		/// <summary>
		/// Returns the value of the smallest component of the current instance of <see cref="Vector4F"/>.
		/// </summary>
		/// <returns>The value of the smallest component of the current instance of <see cref="Vector4F"/>.</returns>
		public float MinComponent()
		{
			return Math.Min( Math.Min( X, Y ), Math.Min( Z, W ) );
		}

		/// <summary>
		/// Returns the value of the largest component of the current instance of <see cref="Vector4F"/>.
		/// </summary>
		/// <returns>The value of the largest component of the current instance of <see cref="Vector4F"/>.</returns>
		public float MaxComponent()
		{
			return Math.Max( Math.Max( X, Y ), Math.Max( Z, W ) );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4F"/> into the equivalent <see cref="Plane"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Plane"/> structure.</returns>
		public PlaneF ToPlane()
		{
			return new PlaneF( this );
		}

		public bool Equals( ref Vector4F v )
		{
			return X == v.X && Y == v.Y && Z == v.Z && W == v.W;
		}

		public static bool Equals( ref Vector4F v1, ref Vector4F v2 )
		{
			return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W;
		}
	}
}
