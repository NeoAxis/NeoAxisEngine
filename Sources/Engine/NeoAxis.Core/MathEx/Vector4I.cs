// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Vec4I> ) )]
	/// <summary>
	/// A structure encapsulating four integer values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Vector4I
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
		/// The Z component of the vector.
		/// </summary>
		public int Z;
		/// <summary>
		/// The W component of the vector.
		/// </summary>
		public int W;

		/// <summary>
		/// Returns the vector (0,0,0,0).
		/// </summary>
		public static readonly Vector4I Zero = new Vector4I( 0, 0, 0, 0 );
		/// <summary>
		/// Returns the vector (1,1,1,1).
		/// </summary>
		public static readonly Vector4I One = new Vector4I( 1, 1, 1, 1 );

		/// <summary>
		/// Constructs a vector with another given vector.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector4I( Vector4I source )
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
		public Vector4I( int x, int y, int z, int w )
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		/// <summary>
		/// Constructs a <see cref="Vector4I"/> from the given <see cref="Vector3I"/> and a W component.
		/// </summary>
		/// <param name="v">The vector to use as the X, Y and Z components.</param>
		/// <param name="w">The W component.</param>
		public Vector4I( Vector3I v, int w )
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
			this.W = w;
		}

		/// <summary>
		/// Converts a string representation of a vector into the equivalent <see cref="Vector4I"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the vector (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Vector4I"/> structure.</returns>
		[AutoConvertType]
		public static Vector4I Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' }, 
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (x y z w).", text ) );

			try
			{
				return new Vector4I(
					int.Parse( vals[ 0 ] ),
					int.Parse( vals[ 1 ] ),
					int.Parse( vals[ 2 ] ),
					int.Parse( vals[ 3 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector4I"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector4I"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1} {2} {3}", X, Y, Z, W );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Vector4I"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Vector4I"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Vector4I"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Vector4I && this == (Vector4I)obj );
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
		public static Vector4I operator +( Vector4I v1, Vector4I v2 )
		{
			Vector4I result;
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
		public static Vector4I operator -( Vector4I v1, Vector4I v2 )
		{
			Vector4I result;
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
		public static Vector4I operator *( Vector4I v1, Vector4I v2 )
		{
			Vector4I result;
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
		/// <param name="i">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4I operator *( Vector4I v, int i )
		{
			Vector4I result;
			result.X = v.X * i;
			result.Y = v.Y * i;
			result.Z = v.Z * i;
			result.W = v.W * i;
			return result;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4I operator *( int i, Vector4I v )
		{
			Vector4I result;
			result.X = i * v.X;
			result.Y = i * v.Y;
			result.Z = i * v.Z;
			result.W = i * v.W;
			return result;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4I operator /( Vector4I v1, Vector4I v2 )
		{
			Vector4I result;
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
		/// <param name="i">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4I operator /( Vector4I v, int i )
		{
			Vector4I result;
			result.X = v.X / i;
			result.Y = v.Y / i;
			result.Z = v.Z / i;
			result.W = v.W / i;
			return result;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector4I operator /( int i, Vector4I v )
		{
			Vector4I result;
			result.X = i / v.X;
			result.Y = i / v.Y;
			result.Z = i / v.Z;
			result.W = i / v.W;
			return result;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <returns>A vector facing in the opposite direction.</returns>
		public static Vector4I operator -( Vector4I v )
		{
			Vector4I result;
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
		public static void Add( ref Vector4I v1, ref Vector4I v2, out Vector4I result )
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
		public static void Subtract( ref Vector4I v1, ref Vector4I v2, out Vector4I result )
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
		public static void Multiply( ref Vector4I v1, ref Vector4I v2, out Vector4I result )
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
		/// <param name="i">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( ref Vector4I v, int i, out Vector4I result )
		{
			result.X = v.X * i;
			result.Y = v.Y * i;
			result.Z = v.Z * i;
			result.W = v.W * i;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( int i, ref Vector4I v, out Vector4I result )
		{
			result.X = v.X * i;
			result.Y = v.Y * i;
			result.Z = v.Z * i;
			result.W = v.W * i;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( ref Vector4I v1, ref Vector4I v2, out Vector4I result )
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
		/// <param name="i">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( ref Vector4I v, int i, out Vector4I result )
		{
			result.X = v.X / i;
			result.Y = v.Y / i;
			result.Z = v.Z / i;
			result.W = v.W / i;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( int i, ref Vector4I v, out Vector4I result )
		{
			result.X = i / v.X;
			result.Y = i / v.Y;
			result.Z = i / v.Z;
			result.W = i / v.W;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <param name="result">When the method completes, contains a vector facing in the opposite direction.</param>
		public static void Negate( ref Vector4I v, out Vector4I result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
			result.W = -v.W;
		}

		//public static Vec4I Add( Vec4I v1, Vec4I v2 )
		//{
		//	Vec4I result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	result.z = v1.z + v2.z;
		//	result.w = v1.w + v2.w;
		//	return result;
		//}

		//public static Vec4I Subtract( Vec4I v1, Vec4I v2 )
		//{
		//	Vec4I result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	result.z = v1.z - v2.z;
		//	result.w = v1.w - v2.w;
		//	return result;
		//}

		//public static Vec4I Multiply( Vec4I v1, Vec4I v2 )
		//{
		//	Vec4I result;
		//	result.x = v1.x * v2.x;
		//	result.y = v1.y * v2.y;
		//	result.z = v1.z * v2.z;
		//	result.w = v1.w * v2.w;
		//	return result;
		//}

		//public static Vec4I Multiply( Vec4I v, int i )
		//{
		//	Vec4I result;
		//	result.x = v.x * i;
		//	result.y = v.y * i;
		//	result.z = v.z * i;
		//	result.w = v.w * i;
		//	return result;
		//}

		//public static Vec4I Multiply( int i, Vec4I v )
		//{
		//	Vec4I result;
		//	result.x = v.x * i;
		//	result.y = v.y * i;
		//	result.z = v.z * i;
		//	result.w = v.w * i;
		//	return result;
		//}

		//public static Vec4I Divide( Vec4I v1, Vec4I v2 )
		//{
		//	Vec4I result;
		//	result.x = v1.x / v2.x;
		//	result.y = v1.y / v2.y;
		//	result.z = v1.z / v2.z;
		//	result.w = v1.w / v2.w;
		//	return result;
		//}

		//public static Vec4I Divide( Vec4I v1, int v2 )
		//{
		//	Vec4I result;
		//	result.x = v1.x / v2;
		//	result.y = v1.y / v2;
		//	result.z = v1.z / v2;
		//	result.w = v1.w / v2;
		//	return result;
		//}

		//public static Vec4I Divide( int i, Vec4I v )
		//{
		//	Vec4I result;
		//	result.x = i / v.x;
		//	result.y = i / v.y;
		//	result.z = i / v.z;
		//	result.w = i / v.w;
		//	return result;
		//}

		//public static Vec4I Negate( Vec4I v )
		//{
		//	Vec4I result;
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
		public static bool operator ==( Vector4I v1, Vector4I v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W );
		}

		/// <summary>
		/// Determines whether two given vectors are unequal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are unequal; False otherwise.</returns>
		public static bool operator !=( Vector4I v1, Vector4I v2 )
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
		public unsafe int this[ int index ]
		{
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( int* v = &this.X )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( int* v = &this.X )
				{
					v[ index ] = value;
				}
			}
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector4I"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector4I"/>; False otherwise.</returns>
		public bool Equals( Vector4I v, int epsilon )
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

		void Clamp( Vector4I min, Vector4I max )
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
		/// Converts the current instance of <see cref="Vector4I"/> into an instance of <see cref="Vector2I"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
		[AutoConvertType]
		public Vector2I ToVector2I()
		{
			Vector2I result;
			result.X = X;
			result.Y = Y;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4I"/> into an instance of <see cref="Vector3I"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
		[AutoConvertType]
		public Vector3I ToVector3I()
		{
			Vector3I result;
			result.X = X;
			result.Y = Y;
			result.Z = Z;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4I"/> into the equivalent <see cref="Vector4F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector4F"/> structure.</returns>
		[AutoConvertType]
		public Vector4F ToVector4F()
		{
			Vector4F result;
			result.X = X;
			result.Y = Y;
			result.Z = Z;
			result.W = W;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4I"/> into the equivalent <see cref="Vector4"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector4"/> structure.</returns>
		[AutoConvertType]
		public Vector4 ToVec4()
		{
			Vector4 result;
			result.X = X;
			result.Y = Y;
			result.Z = Z;
			result.W = W;
			return result;
		}

		/// <summary>
		/// Chooses one of two vectors depending on the <paramref name="pick1"/> value.
		/// </summary>
		/// <param name="v1">The first vector to choose.</param>
		/// <param name="v2">The second vector to choose.</param>
		/// <param name="pick1">If this value is true, the method chooses the virst vector, otherwise it chooses the second one.</param>
		/// <returns>The selected vector.</returns>
		public static Vector4I Select( Vector4I v1, Vector4I v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/// <summary>
		/// Determines whether any component of a given vector is unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if any component of the specified vector is unequal to the zero; otherwise, False.</returns>
		public static bool AnyNonZero( Vector4I v )
		{
			return v != Zero;
		}

		/// <summary>
		/// Determines whether all components of a given vector are unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if all components of the specified vector are unequal to the zero; otherwise, False.</returns>
		public static bool AllNonZero( Vector4I v )
		{
			return v.X != 0 && v.Y != 0 && v.Z != 0 && v.W != 0;
		}

		/// <summary>
		/// Returns a vector containing the largest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the largest components of the specified vectors.</returns>
		public static Vector4I Max( Vector4I v1, Vector4I v2 )
		{
			return new Vector4I( Math.Max( v1.X, v2.X ), Math.Max( v1.Y, v2.Y ), Math.Max( v1.Z, v2.Z ), Math.Max( v1.W, v2.W ) );
		}

		/// <summary>
		/// Returns a vector containing the smallest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the smallest components of the specified vectors.</returns>
		public static Vector4I Min( Vector4I v1, Vector4I v2 )
		{
			return new Vector4I( Math.Min( v1.X, v2.X ), Math.Min( v1.Y, v2.Y ), Math.Min( v1.Z, v2.Z ), Math.Min( v1.W, v2.W ) );
		}

		/// <summary>
		/// Returns the value of the smallest component of the current instance of <see cref="Vector4I"/>.
		/// </summary>
		/// <returns>The value of the smallest component of the current instance of <see cref="Vector4I"/>.</returns>
		public int MinComponent()
		{
			return Math.Min( Math.Min( X, Y ), Math.Min( Z, W ) );
		}

		/// <summary>
		/// Returns the value of the largest component of the current instance of <see cref="Vector4"/>.
		/// </summary>
		/// <returns>The value of the largest component of the current instance of <see cref="Vector4"/>.</returns>
		public int MaxComponent()
		{
			return Math.Max( Math.Max( X, Y ), Math.Max( Z, W ) );
		}
	}
}
