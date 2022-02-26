// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Vec3I> ) )]
	/// <summary>
	/// A structure encapsulating three integer values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Vector3I
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
		/// Returns the vector (0,0,0).
		/// </summary>
		public static readonly Vector3I Zero = new Vector3I( 0, 0, 0 );
		/// <summary>
		/// Returns the vector (1,1,1,1).
		/// </summary>
		public static readonly Vector3I One = new Vector3I( 1, 1, 1 );

		/// <summary>
		/// Constructs a vector with another given vector.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector3I( Vector3I source )
		{
			this.X = source.X;
			this.Y = source.Y;
			this.Z = source.Z;
		}

		/// <summary>
		/// Constructs a vector with a given <see cref="Vector2I"/> and a scalar.
		/// </summary>
		/// <param name="xy">The given <see cref="Vector2I"/>.</param>
		/// <param name="z">The scalar value.</param>
		public Vector3I( Vector2I xy, int z )
		{
			this.X = xy.X;
			this.Y = xy.Y;
			this.Z = z;
		}

		/// <summary>
		/// Constructs a vector with the given individual elements.
		/// </summary>
		/// <param name="x">X component.</param>
		/// <param name="y">Y component.</param>
		/// <param name="z">Z component.</param>
		public Vector3I( int x, int y, int z )
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		/// <summary>
		/// Converts a string representation of a vector into the equivalent <see cref="Vector3I"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the vector (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Vector3I"/> structure.</returns>
		[AutoConvertType]
		public static Vector3I Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 3 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 3 parts separated by spaces in the form (x y z).", text ) );

			try
			{
				return new Vector3I(
					int.Parse( vals[ 0 ] ),
					int.Parse( vals[ 1 ] ),
					int.Parse( vals[ 2 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector3I"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector3I"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1} {2}", X, Y, Z );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Vector3I"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Vector3I"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Vector3I"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Vector3I && this == (Vector3I)obj );
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return ( X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() );
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="v1">The first vector to add.</param>
		/// <param name="v2">The second vector to add.</param>
		/// <returns>The sum of the two vectors.</returns>
		public static Vector3I operator +( Vector3I v1, Vector3I v2 )
		{
			Vector3I result;
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
			result.Z = v1.Z + v2.Z;
			return result;
		}

		/// <summary>
		/// Subtracts two vectors.
		/// </summary>
		/// <param name="v1">The vector to subtract from.</param>
		/// <param name="v2">The vector to be subtracted from another vector.</param>
		/// <returns>The difference of the two vectors.</returns>
		public static Vector3I operator -( Vector3I v1, Vector3I v2 )
		{
			Vector3I result;
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
			result.Z = v1.Z - v2.Z;
			return result;
		}

		/// <summary>
		/// Multiplies two vectors together.
		/// </summary>
		/// <param name="v1">The first vector to multiply.</param>
		/// <param name="v2">The second vector to multiply.</param>
		/// <returns>The product vector.</returns>
		public static Vector3I operator *( Vector3I v1, Vector3I v2 )
		{
			Vector3I result;
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
			result.Z = v1.Z * v2.Z;
			return result;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to scale.</param>
		/// <param name="i">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector3I operator *( Vector3I v, int i )
		{
			Vector3I result;
			result.X = v.X * i;
			result.Y = v.Y * i;
			result.Z = v.Z * i;
			return result;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector3I operator *( int i, Vector3I v )
		{
			Vector3I result;
			result.X = i * v.X;
			result.Y = i * v.Y;
			result.Z = i * v.Z;
			return result;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector3I operator /( Vector3I v1, Vector3I v2 )
		{
			Vector3I result;
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
			result.Z = v1.Z / v2.Z;
			return result;
		}

		/// <summary>
		/// Divides a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to divide.</param>
		/// <param name="i">The scalar value.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector3I operator /( Vector3I v, int i )
		{
			Vector3I result;
			result.X = v.X / i;
			result.Y = v.Y / i;
			result.Z = v.Z / i;
			return result;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <returns>The scaled vector.</returns>
		public static Vector3I operator /( int i, Vector3I v )
		{
			Vector3I result;
			result.X = i / v.X;
			result.Y = i / v.Y;
			result.Z = i / v.Z;
			return result;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <returns>A vector facing in the opposite direction.</returns>
		public static Vector3I operator -( Vector3I v )
		{
			Vector3I result;
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
			return result;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="v1">The first vector to add.</param>
		/// <param name="v2">The second vector to add.</param>
		/// <param name="result">When the method completes, contains the sum of the two vectors.</param>
		public static void Add( ref Vector3I v1, ref Vector3I v2, out Vector3I result )
		{
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
			result.Z = v1.Z + v2.Z;
		}

		/// <summary>
		/// Subtracts two vectors.
		/// </summary>
		/// <param name="v1">The vector from which to subtract.</param>
		/// <param name="v2">The vector which should be subtracted from another.</param>
		/// <param name="result">When the method completes, contains the difference of the two vectors.</param>
		public static void Subtract( ref Vector3I v1, ref  Vector3I v2, out Vector3I result )
		{
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
			result.Z = v1.Z - v2.Z;
		}

		/// <summary>
		/// Multiplies two vectors together.
		/// </summary>
		/// <param name="v1">The first vector to multiply.</param>
		/// <param name="v2">The second vector to multiply.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( ref Vector3I v1, ref Vector3I v2, out Vector3I result )
		{
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
			result.Z = v1.Z * v2.Z;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to scale.</param>
		/// <param name="i">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( ref Vector3I v, int i, out Vector3I result )
		{
			result.X = v.X * i;
			result.Y = v.Y * i;
			result.Z = v.Z * i;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Multiply( int i, ref Vector3I v, out Vector3I result )
		{
			result.X = v.X * i;
			result.Y = v.Y * i;
			result.Z = v.Z * i;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( ref Vector3I v1, ref Vector3I v2, out Vector3I result )
		{
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
			result.Z = v1.Z / v2.Z;
		}

		/// <summary>
		/// Divides a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to divide.</param>
		/// <param name="i">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( ref Vector3I v, int i, out Vector3I result )
		{
			result.X = v.X / i;
			result.Y = v.Y / i;
			result.Z = v.Z / i;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="i">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		public static void Divide( int i, ref Vector3I v, out Vector3I result )
		{
			result.X = i / v.X;
			result.Y = i / v.Y;
			result.Z = i / v.Z;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <param name="result">When the method completes, contains a vector facing in the opposite direction.</param>
		public static void Negate( ref Vector3I v, out Vector3I result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
		}

		//public static Vec3I Add( Vec3I v1, Vec3I v2 )
		//{
		//	Vec3I result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	result.z = v1.z + v2.z;
		//	return result;
		//}

		//public static Vec3I Subtract( Vec3I v1, Vec3I v2 )
		//{
		//	Vec3I result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	result.z = v1.z - v2.z;
		//	return result;
		//}

		//public static Vec3I Multiply( Vec3I v1, Vec3I v2 )
		//{
		//	Vec3I result;
		//	result.x = v1.x * v2.x;
		//	result.y = v1.y * v2.y;
		//	result.z = v1.z * v2.z;
		//	return result;
		//}

		//public static Vec3I Multiply( Vec3I v, int i )
		//{
		//	Vec3I result;
		//	result.x = v.x * i;
		//	result.y = v.y * i;
		//	result.z = v.z * i;
		//	return result;
		//}

		//public static Vec3I Multiply( int i, Vec3I v )
		//{
		//	Vec3I result;
		//	result.x = v.x * i;
		//	result.y = v.y * i;
		//	result.z = v.z * i;
		//	return result;
		//}

		//public static Vec3I Divide( Vec3I v1, Vec3I v2 )
		//{
		//	Vec3I result;
		//	result.x = v1.x / v2.x;
		//	result.y = v1.y / v2.y;
		//	result.z = v1.z / v2.z;
		//	return result;
		//}

		//public static Vec3I Divide( Vec3I v, int i )
		//{
		//	Vec3I result;
		//	result.x = v.x / i;
		//	result.y = v.y / i;
		//	result.z = v.z / i;
		//	return result;
		//}

		//public static Vec3I Divide( int i, Vec3I v )
		//{
		//	Vec3I result;
		//	result.x = i / v.x;
		//	result.y = i / v.y;
		//	result.z = i / v.z;
		//	return result;
		//}

		//public static Vec3I Negate( Vec3I v )
		//{
		//	Vec3I result;
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
		public static bool operator ==( Vector3I v1, Vector3I v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z );
		}

		/// <summary>
		/// Determines whether two given vectors are unequal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are unequal; False otherwise.</returns>
		public static bool operator !=( Vector3I v1, Vector3I v2 )
		{
			return ( v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z );
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the X, Y, or Z component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, and 2 for the Z component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
		public unsafe int this[ int index ]
		{
			get
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( int* v = &this.X )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( int* v = &this.X )
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
		public static int Dot( Vector3I v1, Vector3I v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>When the method completes, contains the dot product of the two vectors.</returns>
		public static void Dot( ref Vector3I v1, ref Vector3I v2, out int result )
		{
			result = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The cross product of the two vectors.</returns>
		public static Vector3I Cross( Vector3I v1, Vector3I v2 )
		{
			return new Vector3I(
				v1.Y * v2.Z - v1.Z * v2.Y,
				v1.Z * v2.X - v1.X * v2.Z,
				v1.X * v2.Y - v1.Y * v2.X );
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="result">When the method completes, contains the cross product of the two vectors.</param>
		public static void Cross( ref Vector3I v1, ref Vector3I v2, out Vector3I result )
		{
			result.X = v1.Y * v2.Z - v1.Z * v2.Y;
			result.Y = v1.Z * v2.X - v1.X * v2.Z;
			result.Z = v1.X * v2.Y - v1.Y * v2.X;
		}

		/// <summary>
		/// Restricts the current instance of <see cref="Vector3I"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector3I"/>.</returns>
		public void Clamp( Vector3I min, Vector3I max )
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
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector3I"/> into an instance of <see cref="Vector2I"/>.
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
		/// Converts the current instance of <see cref="Vector3I"/> into the equivalent <see cref="Vector3F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector3F"/> structure.</returns>
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
		/// Converts the current instance of <see cref="Vector3I"/> into the equivalent <see cref="Vector3"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector3"/> structure.</returns>
		[AutoConvertType]
		public Vector3 ToVector3()
		{
			Vector3 result;
			result.X = X;
			result.Y = Y;
			result.Z = Z;
			return result;
		}

		/// <summary>
		/// Chooses one of two vectors depending on the <paramref name="pick1"/> value.
		/// </summary>
		/// <param name="v1">The first vector to choose.</param>
		/// <param name="v2">The second vector to choose.</param>
		/// <param name="pick1">If this value is true, the method chooses the virst vector, otherwise it chooses the second one.</param>
		/// <returns>The selected vector.</returns>
		public static Vector3I Select( Vector3I v1, Vector3I v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/// <summary>
		/// Determines whether any component of a given vector is unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if any component of the specified vector is unequal to the zero; otherwise, False.</returns>
		public static bool AnyNonZero( Vector3I v )
		{
			return v != Zero;
		}

		/// <summary>
		/// Determines whether all components of a given vector are unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if all components of the specified vector are unequal to the zero; otherwise, False.</returns>
		public static bool AllNonZero( Vector3I v )
		{
			return v.X != 0 && v.Y != 0 && v.Z != 0;
		}

		/// <summary>
		/// Returns a vector containing the largest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the largest components of the specified vectors.</returns>
		public static Vector3I Max( Vector3I v1, Vector3I v2 )
		{
			return new Vector3I( Math.Max( v1.X, v2.X ), Math.Max( v1.Y, v2.Y ), Math.Max( v1.Z, v2.Z ) );
		}

		/// <summary>
		/// Returns a vector containing the smallest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the smallest components of the specified vectors.</returns>
		public static Vector3I Min( Vector3I v1, Vector3I v2 )
		{
			return new Vector3I( Math.Min( v1.X, v2.X ), Math.Min( v1.Y, v2.Y ), Math.Min( v1.Z, v2.Z ) );
		}

		/// <summary>
		/// Returns the value of the smallest component of the current instance of <see cref="Vector3I"/>.
		/// </summary>
		/// <returns>The value of the smallest component of the current instance of <see cref="Vector3I"/>.</returns>
		public int MinComponent()
		{
			return Math.Min( X, Math.Min( Y, Z ) );
		}

		/// <summary>
		/// Returns the value of the largest component of the current instance of <see cref="Vector3I"/>.
		/// </summary>
		/// <returns>The value of the largest component of the current instance of <see cref="Vector3I"/>.</returns>
		public int MaxComponent()
		{
			return Math.Max( X, Math.Max( Y, Z ) );
		}
	}
}
