// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Vec3> ) )]
	/// <summary>
	/// A structure encapsulating three double precision floating point values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Vector3
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
		/// The Z component of the vector.
		/// </summary>
		[ShaderGenerationFunction( "{this}.z" )]
		public double Z;

		/// <summary>
		/// Returns the vector (0,0,0).
		/// </summary>
		public static readonly Vector3 Zero = new Vector3( 0.0, 0.0, 0.0 );
		public const string ZeroAsString = "0 0 0";
		/// <summary>
		/// Returns the vector (1,1,1).
		/// </summary>
		public static readonly Vector3 One = new Vector3( 1.0, 1.0, 1.0 );
		public const string OneAsString = "1 1 1";
		/// <summary>
		/// Returns the vector (1,0,0).
		/// </summary>
		public static readonly Vector3 XAxis = new Vector3( 1.0, 0.0, 0.0 );
		/// <summary>
		/// Returns the vector (0,1,0).
		/// </summary>
		public static readonly Vector3 YAxis = new Vector3( 0.0, 1.0, 0.0 );
		/// <summary>
		/// Returns the vector (0,0,1).
		/// </summary>
		public static readonly Vector3 ZAxis = new Vector3( 0.0, 0.0, 1.0 );

		/// <summary>
		/// Constructs a vector with another given vector.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector3( Vector3 source )
		{
			this.X = source.X;
			this.Y = source.Y;
			this.Z = source.Z;
		}

		/// <summary>
		/// Constructs a vector with a given <see cref="Vector2"/> and a scalar.
		/// </summary>
		/// <param name="xy">The given <see cref="Vector2"/>.</param>
		/// <param name="z">The scalar value.</param>
		[ShaderGenerationFunction( "vec3({xy}, {z})" )]
		public Vector3( Vector2 xy, double z )
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
		[ShaderGenerationFunction( "vec3({x}, {y}, {z})" )]
		public Vector3( double x, double y, double z )
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		/// <summary>
		/// Constructs a vector with another given vector of <see cref="Vector3F"/> format.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector3( Vector3F source )
		{
			X = source.X;
			Y = source.Y;
			Z = source.Z;
		}

		/// <summary>
		/// Constructs a vector with another given vector of <see cref="Vector3I"/> format.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector3( Vector3I source )
		{
			X = source.X;
			Y = source.Y;
			Z = source.Z;
		}

		/// <summary>
		/// Converts a string representation of a vector into the equivalent <see cref="Vector3"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the vector (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Vector3"/> structure.</returns>
		[AutoConvertType]
		public static Vector3 Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 3 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 3 parts separated by spaces in the form (x y z).", text ) );

			try
			{
				return new Vector3(
					double.Parse( vals[ 0 ] ),
					double.Parse( vals[ 1 ] ),
					double.Parse( vals[ 2 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector3"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector3"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 17 );
			//return string.Format( "{0} {1} {2}", x, y, z );
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector3"/> with a given precision.
		/// </summary>
		/// <param name="precision">The precision value.</param>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector3"/>.</returns>
		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "}";
			return string.Format( format, X, Y, Z );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Vector3"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Vector3"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Vector3"/>; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Vector3 && this == (Vector3)obj );
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} + {v2}" )]
		public static Vector3 operator +( Vector3 v1, Vector3 v2 )
		{
			Vector3 result;
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} - {v2}" )]
		public static Vector3 operator -( Vector3 v1, Vector3 v2 )
		{
			Vector3 result;
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} * {v2}" )]
		public static Vector3 operator *( Vector3 v1, Vector3 v2 )
		{
			Vector3 result;
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
			result.Z = v1.Z * v2.Z;
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
		public static Vector3 operator *( Vector3 v, double s )
		{
			Vector3 result;
			result.X = v.X * s;
			result.Y = v.Y * s;
			result.Z = v.Z * s;
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
		public static Vector3 operator *( double s, Vector3 v )
		{
			Vector3 result;
			result.X = s * v.X;
			result.Y = s * v.Y;
			result.Z = s * v.Z;
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
		public static Vector3 operator /( Vector3 v1, Vector3 v2 )
		{
			Vector3 result;
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
			result.Z = v1.Z / v2.Z;
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
		public static Vector3 operator /( Vector3 v, double s )
		{
			Vector3 result;
			double invS = 1.0 / s;
			result.X = v.X * invS;
			result.Y = v.Y * invS;
			result.Z = v.Z * invS;
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
		public static Vector3 operator /( double s, Vector3 v )
		{
			Vector3 result;
			result.X = s / v.X;
			result.Y = s / v.Y;
			result.Z = s / v.Z;
			return result;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <returns>A vector facing in the opposite direction.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "-{v}" )]
		public static Vector3 operator -( Vector3 v )
		{
			Vector3 result;
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref Vector3 v1, ref Vector3 v2, out Vector3 result )
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref Vector3 v1, ref Vector3 v2, out Vector3 result )
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Vector3 v1, ref Vector3 v2, out Vector3 result )
		{
			result.X = v1.X * v2.X;
			result.Y = v1.Y * v2.Y;
			result.Z = v1.Z * v2.Z;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to scale.</param>
		/// <param name="s">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Vector3 v, double s, out Vector3 result )
		{
			result.X = v.X * s;
			result.Y = v.Y * s;
			result.Z = v.Z * s;
		}

		/// <summary>
		/// Multiplies a vector by a given scalar.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector to scale.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( double s, ref Vector3 v, out Vector3 result )
		{
			result.X = v.X * s;
			result.Y = v.Y * s;
			result.Z = v.Z * s;
		}

		/// <summary>
		/// Divides the first vector by the second vector.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( ref Vector3 v1, ref Vector3 v2, out Vector3 result )
		{
			result.X = v1.X / v2.X;
			result.Y = v1.Y / v2.Y;
			result.Z = v1.Z / v2.Z;
		}

		/// <summary>
		/// Divides a vector by a given scalar.
		/// </summary>
		/// <param name="v">The vector to divide.</param>
		/// <param name="s">The scalar value.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( ref Vector3 v, double s, out Vector3 result )
		{
			double invS = 1.0 / s;
			result.X = v.X * invS;
			result.Y = v.Y * invS;
			result.Z = v.Z * invS;
		}

		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="s">The scalar value.</param>
		/// <param name="v">The vector.</param>
		/// <param name="result">When the method completes, contains the scaled vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( double s, ref Vector3 v, out Vector3 result )
		{
			result.X = s / v.X;
			result.Y = s / v.Y;
			result.Z = s / v.Z;
		}

		/// <summary>
		/// Reverses the direction of a given vector. 
		/// </summary>
		/// <param name="v">The vector to negate.</param>
		/// <param name="result">When the method completes, contains a vector facing in the opposite direction.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Negate( ref Vector3 v, out Vector3 result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
		}

		//public static Vec3 Add( Vec3 v1, Vec3 v2 )
		//{
		//	Vec3 result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	result.z = v1.z + v2.z;
		//	return result;
		//}

		//public static Vec3 Subtract( Vec3 v1, Vec3 v2 )
		//{
		//	Vec3 result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	result.z = v1.z - v2.z;
		//	return result;
		//}

		//public static Vec3 Multiply( Vec3 v1, Vec3 v2 )
		//{
		//	Vec3 result;
		//	result.x = v1.x * v2.x;
		//	result.y = v1.y * v2.y;
		//	result.z = v1.z * v2.z;
		//	return result;
		//}

		//public static Vec3 Multiply( Vec3 v, double s )
		//{
		//	Vec3 result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	result.z = v.z * s;
		//	return result;
		//}

		//public static Vec3 Multiply( double s, Vec3 v )
		//{
		//	Vec3 result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	result.z = v.z * s;
		//	return result;
		//}

		//public static Vec3 Divide( Vec3 v1, Vec3 v2 )
		//{
		//	Vec3 result;
		//	result.x = v1.x / v2.x;
		//	result.y = v1.y / v2.y;
		//	result.z = v1.z / v2.z;
		//	return result;
		//}

		//public static Vec3 Divide( Vec3 v, double s )
		//{
		//	Vec3 result;
		//	double invS = 1.0 / s;
		//	result.x = v.x * invS;
		//	result.y = v.y * invS;
		//	result.z = v.z * invS;
		//	return result;
		//}

		//public static Vec3 Divide( double s, Vec3 v )
		//{
		//	Vec3 result;
		//	result.x = s / v.x;
		//	result.y = s / v.y;
		//	result.z = s / v.z;
		//	return result;
		//}

		//public static Vec3 Negate( Vec3 v )
		//{
		//	Vec3 result;
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
		public static bool operator ==( Vector3 v1, Vector3 v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z );
		}

		/// <summary>
		/// Determines whether two given vectors are unequal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are unequal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Vector3 v1, Vector3 v2 )
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
		public unsafe double this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.X )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 2 )
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
		public static double Dot( Vector3 v1, Vector3 v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal static double Dot( ref Vector3 v1, ref Vector3 v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>When the method completes, contains the dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Dot( ref Vector3 v1, ref Vector3 v2, out double result )
		{
			result = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double Dot( Vector3 v )
		{
			return X * v.X + Y * v.Y + Z * v.Z;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double Dot( ref Vector3 v )
		{
			return X * v.X + Y * v.Y + Z * v.Z;
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The cross product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "cross({v1}, {v2})" )]
		public static Vector3 Cross( Vector3 v1, Vector3 v2 )
		{
			return new Vector3(
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
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Cross( ref Vector3 v1, ref Vector3 v2, out Vector3 result )
		{
			result.X = v1.Y * v2.Z - v1.Z * v2.Y;
			result.Y = v1.Z * v2.X - v1.X * v2.Z;
			result.Z = v1.X * v2.Y - v1.Y * v2.X;
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The cross product of the two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 Cross( Vector3 v )
		{
			return new Vector3(
				Y * v.Z - Z * v.Y,
				Z * v.X - X * v.Z,
				X * v.Y - Y * v.X );
		}

		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <param name="result">When the method completes, contains the cross product of the two vectors.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Cross( ref Vector3 v, out Vector3 result )
		{
			result.X = Y * v.Z - Z * v.Y;
			result.Y = Z * v.X - X * v.Z;
			result.Z = X * v.Y - Y * v.X;
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector3"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector3"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Vector3 v, double epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			if( Math.Abs( Z - v.Z ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector3"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector3"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref Vector3 v, double epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			if( Math.Abs( Z - v.Z ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Calculates the length of the current instance of <see cref="Vector3"/>.
		/// </summary>
		/// <returns>The length of the current instance of <see cref="Vector3"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "length({this})" )]
		public double Length()
		{
			return Math.Sqrt( X * X + Y * Y + Z * Z );
		}

		/// <summary>
		/// Calculates the squared length of the current instance of <see cref="Vector3"/>.
		/// </summary>
		/// <returns>The squared length of the current instance of <see cref="Vector3"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double LengthSquared()
		{
			return X * X + Y * Y + Z * Z;
		}

		/// <summary>
		/// Restricts the current instance of <see cref="Vector3"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector3"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Clamp( Vector3 min, Vector3 max )
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
		/// Restricts the current instance of <see cref="Vector3"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector3"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "clamp({this}, {min}, {max})" )]
		public Vector3 GetClamp( Vector3 min, Vector3 max )
		{
			var r = this;
			r.Clamp( min, max );
			return r;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector3"/> into a unit vector.
		/// </summary>
		/// <returns>Returns the length of the current instance of <see cref="Vector3"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double Normalize()
		{
			double sqrLength = X * X + Y * Y + Z * Z;
			double invLength = 1.0 / Math.Sqrt( sqrLength );
			X *= invLength;
			Y *= invLength;
			Z *= invLength;
			return invLength * sqrLength;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Negate()
		{
			X = -X;
			Y = -Y;
			Z = -Z;
		}

		/// <summary>
		/// Converts a vector into a unit vector. 
		/// </summary>
		/// <param name="v">The vector to normalize.</param>
		/// <returns>The normalized vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector3 Normalize( Vector3 v )
		{
			double invLength = 1.0 / Math.Sqrt( v.X * v.X + v.Y * v.Y + v.Z * v.Z );
			return new Vector3( v.X * invLength, v.Y * invLength, v.Z * invLength );
		}

		/// <summary>
		/// Converts a vector into a unit vector.
		/// </summary>
		/// <param name="v">The vector to normalize.</param>
		/// <param name="result">When the method completes, contains the normalized vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Normalize( ref Vector3 v, out Vector3 result )
		{
			double invLength = 1.0 / Math.Sqrt( v.X * v.X + v.Y * v.Y + v.Z * v.Z );
			result.X = v.X * invLength;
			result.Y = v.Y * invLength;
			result.Z = v.Z * invLength;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector3"/> into a unit vector and returns the normalized vector.
		/// </summary>
		/// <returns>The normalized instance of <see cref="Vector3"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "normalize({this})" )]
		public Vector3 GetNormalize()
		{
			double invLength = 1.0 / Math.Sqrt( X * X + Y * Y + Z * Z );
			return new Vector3( X * invLength, Y * invLength, Z * invLength );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector3"/> into an instance of <see cref="Vector2"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
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
		/// Converts the current instance of <see cref="Vector3"/> into an instance of <see cref="Vector2"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
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
		/// Performs a linear interpolation between two vectors based on the given weighting.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="v2"/>.</param>
		/// <returns>The interpolated vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "lerp({v1}, {v2}, {amount})" )]
		public static Vector3 Lerp( Vector3 v1, Vector3 v2, double amount )
		{
			Vector3 result;
			result.X = v1.X + ( v2.X - v1.X ) * amount;
			result.Y = v1.Y + ( v2.Y - v1.Y ) * amount;
			result.Z = v1.Z + ( v2.Z - v1.Z ) * amount;
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
		public static void Lerp( ref Vector3 v1, ref Vector3 v2, double amount, out Vector3 result )
		{
			result.X = v1.X + ( v2.X - v1.X ) * amount;
			result.Y = v1.Y + ( v2.Y - v1.Y ) * amount;
			result.Z = v1.Z + ( v2.Z - v1.Z ) * amount;
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector3"/> between 0 and 1.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Saturate()
		{
			MathEx.Saturate( ref X );
			MathEx.Saturate( ref Y );
			MathEx.Saturate( ref Z );
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector3"/> between 0 and 1 and returns the saturated value.
		/// </summary>
		/// <returns>The saturated value.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "saturate({this})" )]
		public Vector3 GetSaturate()
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
		public static double Distance( Vector3 v1, Vector3 v2 )
		{
			Subtract( ref v1, ref v2, out Vector3 result );
			return result.Length();
		}

		/// <summary>
		/// Calculates the distance between two vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The distance between two vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static double Distance( ref Vector3 v1, ref Vector3 v2 )
		{
			Subtract( ref v1, ref v2, out Vector3 result );
			return result.Length();
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector3"/> into the equivalent <see cref="Vector3F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector3F"/> structure.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector3F ToVector3F()
		{
			Vector3F result;
			result.X = (float)X;
			result.Y = (float)Y;
			result.Z = (float)Z;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector3"/> into the equivalent <see cref="Vector3F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector3F"/> structure.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector3H ToVector3H()
		{
			Vector3H result;
			result.X = new HalfType( X );
			result.Y = new HalfType( Y );
			result.Z = new HalfType( Z );
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector3"/> into the equivalent <see cref="Vector3I"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector3I"/> structure.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector3I ToVector3I()
		{
			Vector3I result;
			result.X = (int)X;
			result.Y = (int)Y;
			result.Z = (int)Z;
			return result;
		}

		//public ColorValue ToColorValue()
		//{
		//	return new ColorValue( X, Y, Z );
		//}

		/// <summary>
		/// Chooses one of two vectors depending on the <paramref name="pick1"/> value.
		/// </summary>
		/// <param name="v1">The first vector to choose.</param>
		/// <param name="v2">The second vector to choose.</param>
		/// <param name="pick1">If this value is true, the method chooses the virst vector, otherwise it chooses the second one.</param>
		/// <returns>The selected vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{pick1} ? {v1} : {v2}" )]
		public static Vector3 Select( Vector3 v1, Vector3 v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/// <summary>
		/// Rounds the current instance of <see cref="Vector3"/> towards zero for each component in a vector.
		/// </summary>
		/// <param name="v"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Truncate()
		{
			X = (long)X;
			Y = (long)Y;
			Z = (long)Z;
		}

		/// <summary>
		/// Rounds a given vector towards zero for each component in it and returns the truncated vector.
		/// </summary>
		/// <param name="v">The vector to truncate.</param>
		/// <returns>The truncated vector</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "trunc({v})" )]
		public Vector3 GetTruncate( Vector3 v )
		{
			return new Vector3( (long)v.X, (long)v.Y, (long)v.Z );
		}

		/// <summary>
		/// Determines whether any component of a given vector is unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if any component of the specified vector is unequal to the zero; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "any({v})" )]
		public static bool AnyNonZero( Vector3 v )
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
		public static bool AllNonZero( Vector3 v )
		{
			return v.X != 0 && v.Y != 0 && v.Z != 0;
		}

		/// <summary>
		/// Returns a vector whose elements are the absolute values of each of the specified vector's components.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The absolute value vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "abs({v})" )]
		public static Vector3 Abs( Vector3 v )
		{
			return new Vector3( Math.Abs( v.X ), Math.Abs( v.Y ), Math.Abs( v.Z ) );
		}

		/// <summary>
		/// Calculates the arc-cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose cosines are equal to the
		/// corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "acos({v})" )]
		public static Vector3 Acos( Vector3 v )
		{
			return new Vector3( Math.Acos( v.X ), Math.Acos( v.Y ), Math.Acos( v.Z ) );
		}

		/// <summary>
		/// Calculates the arc-sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose sines are equal to the
		/// corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "asin({v})" )]
		public static Vector3 Asin( Vector3 v )
		{
			return new Vector3( Math.Asin( v.X ), Math.Asin( v.Y ), Math.Asin( v.Z ) );
		}

		/// <summary>
		/// Calculates the arc-tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are equal to the
		/// corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "atan({v})" )]
		public static Vector3 Atan( Vector3 v )
		{
			return new Vector3( Math.Atan( v.X ), Math.Atan( v.Y ), Math.Atan( v.Z ) );
		}

		/// <summary>
		/// Returns the vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.
		/// </summary>
		/// <param name="y">The first vector.</param>
		/// <param name="x">The second vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "atan2({y}, {x})" )]
		public static Vector3 Atan2( Vector3 y, Vector3 x )
		{
			return new Vector3( Math.Atan2( y.X, x.X ), Math.Atan2( y.Y, x.Y ), Math.Atan2( y.Z, x.Z ) );
		}

		/// <summary>
		/// Calculates the cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the cosines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "cos({v})" )]
		public static Vector3 Cos( Vector3 v )
		{
			return new Vector3( Math.Cos( v.X ), Math.Cos( v.Y ), Math.Cos( v.Z ) );
		}

		/// <summary>
		/// Calculates the hyperbolic cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic cosines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "cosh({v})" )]
		public static Vector3 Cosh( Vector3 v )
		{
			return new Vector3( Math.Cosh( v.X ), Math.Cosh( v.Y ), Math.Cosh( v.Z ) );
		}

		/// <summary>
		/// Returns the vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "exp({v})" )]
		public static Vector3 Exp( Vector3 v )
		{
			return new Vector3( Math.Exp( v.X ), Math.Exp( v.Y ), Math.Exp( v.Z ) );
		}

		/// <summary>
		/// Calculates the natural logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the natural logarithms of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "log({v})" )]
		public static Vector3 Log( Vector3 v )
		{
			return new Vector3( Math.Log( v.X ), Math.Log( v.Y ), Math.Log( v.Z ) );
		}

		/// <summary>
		/// Calculates the base 10 logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the base 10 logarithms of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "log10({v})" )]
		public static Vector3 Log10( Vector3 v )
		{
			return new Vector3( Math.Log10( v.X ), Math.Log10( v.Y ), Math.Log10( v.Z ) );
		}

		/// <summary>
		/// Returns a vector containing the largest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the largest components of the specified vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "max({v1}, {v2})" )]
		public static Vector3 Max( Vector3 v1, Vector3 v2 )
		{
			return new Vector3( Math.Max( v1.X, v2.X ), Math.Max( v1.Y, v2.Y ), Math.Max( v1.Z, v2.Z ) );
		}

		/// <summary>
		/// Returns a vector containing the smallest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the smallest components of the specified vectors.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "min({v1}, {v2})" )]
		public static Vector3 Min( Vector3 v1, Vector3 v2 )
		{
			return new Vector3( Math.Min( v1.X, v2.X ), Math.Min( v1.Y, v2.Y ), Math.Min( v1.Z, v2.Z ) );
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
		public static Vector3 Pow( Vector3 x, Vector3 y )
		{
			return new Vector3( Math.Pow( x.X, y.X ), Math.Pow( x.Y, y.Y ), Math.Pow( x.Z, y.Z ) );
		}

		/// <summary>
		/// Calculates the sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the sines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "sin({v})" )]
		public static Vector3 Sin( Vector3 v )
		{
			return new Vector3( Math.Sin( v.X ), Math.Sin( v.Y ), Math.Sin( v.Z ) );
		}

		/// <summary>
		/// Calculates the hyperbolic sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic sines of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "sinh({v})" )]
		public static Vector3 Sinh( Vector3 v )
		{
			return new Vector3( Math.Sinh( v.X ), Math.Sinh( v.Y ), Math.Sinh( v.Z ) );
		}

		/// <summary>
		/// Calculates the square root of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the square root of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "sqrt({v})" )]
		public static Vector3 Sqrt( Vector3 v )
		{
			return new Vector3( Math.Sqrt( v.X ), Math.Sqrt( v.Y ), Math.Sqrt( v.Z ) );
		}

		/// <summary>
		/// Calculates the tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the tangents of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "tan({v})" )]
		public static Vector3 Tan( Vector3 v )
		{
			return new Vector3( Math.Tan( v.X ), Math.Tan( v.Y ), Math.Tan( v.Z ) );
		}

		/// <summary>
		/// Calculates the hyperbolic tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic tangents of the corresponding components in the specified vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "tanh({v})" )]
		public static Vector3 Tanh( Vector3 v )
		{
			return new Vector3( Math.Tanh( v.X ), Math.Tanh( v.Y ), Math.Tanh( v.Z ) );
		}

		/// <summary>
		/// Returns the value of the smallest component of the current instance of <see cref="Vector3"/>.
		/// </summary>
		/// <returns>The value of the smallest component of the current instance of <see cref="Vector3"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double MinComponent()
		{
			return Math.Min( X, Math.Min( Y, Z ) );
		}

		/// <summary>
		/// Returns the value of the largest component of the current instance of <see cref="Vector3"/>.
		/// </summary>
		/// <returns>The value of the largest component of the current instance of <see cref="Vector3"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double MaxComponent()
		{
			return Math.Max( X, Math.Max( Y, Z ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref Vector3 v )
		{
			return X == v.X && Y == v.Y && Z == v.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool Equals( ref Vector3 v1, ref Vector3 v2 )
		{
			return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
		}
	}
}
