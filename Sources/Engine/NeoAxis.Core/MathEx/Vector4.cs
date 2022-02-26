// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Vec4> ) )]
	/// <summary>
	/// A structure encapsulating four double precision floating point values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Vector4
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
		/// The W component of the vector.
		/// </summary>
		[ShaderGenerationFunction( "{this}.w" )]
		public double W;

		/// <summary>
		/// Returns the vector (0,0,0,0).
		/// </summary>
		public static readonly Vector4 Zero = new Vector4( 0.0, 0.0, 0.0, 0.0 );
		/// <summary>
		/// Returns the vector (1,1,1,1).
		/// </summary>
		public static readonly Vector4 One = new Vector4( 1.0, 1.0, 1.0, 1.0 );

		/// <summary>
		/// Constructs a vector with another given vector.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector4( Vector4 source )
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
		[ShaderGenerationFunction( "vec4({x}, {y}, {z}, {w})" )]
		public Vector4( double x, double y, double z, double w )
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		/// <summary>
		/// Constructs a <see cref="Vector4"/> from the given <see cref="Vector3"/> and a W component.
		/// </summary>
		/// <param name="v">The vector to use as the X, Y and Z components.</param>
		/// <param name="w">The W component.</param>
		[ShaderGenerationFunction( "vec4({v}, {w})" )]
		public Vector4( Vector3 v, double w )
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
			this.W = w;
		}

		/// <summary>
		/// Constructs a vector with another given vector of <see cref="Vector4F"/> format.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector4( Vector4F source )
		{
			X = source.X;
			Y = source.Y;
			Z = source.Z;
			W = source.W;
		}

		/// <summary>
		/// Constructs a vector with another given vector of <see cref="Vector4I"/> format.
		/// </summary>
		/// <param name="source">The source vector.</param>
		public Vector4( Vector4I source )
		{
			X = source.X;
			Y = source.Y;
			Z = source.Z;
			W = source.W;
		}

		/// <summary>
		/// Converts a string representation of a vector into the equivalent <see cref="Vector4"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the vector (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Vector4"/> structure.</returns>
		[AutoConvertType]
		public static Vector4 Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (x y z w).", text ) );

			try
			{
				return new Vector4(
					double.Parse( vals[ 0 ] ),
					double.Parse( vals[ 1 ] ),
					double.Parse( vals[ 2 ] ),
					double.Parse( vals[ 3 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector4"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector4"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 17 );
			//return string.Format( "{0} {1} {2} {3}", x, y, z, w );
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Vector4"/> with a given precision.
		/// </summary>
		/// <param name="precision">The precision value.</param>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Vector4"/>.</returns>
		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "}";
			return string.Format( format, X, Y, Z, W );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Vector4"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Vector4"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Vector4"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Vector4 && this == (Vector4)obj );
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
		[ShaderGenerationFunction( "{v1} + {v2}" )]
		public static Vector4 operator +( Vector4 v1, Vector4 v2 )
		{
			Vector4 result;
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
		[ShaderGenerationFunction( "{v1} - {v2}" )]
		public static Vector4 operator -( Vector4 v1, Vector4 v2 )
		{
			Vector4 result;
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
		[ShaderGenerationFunction( "{v1} * {v2}" )]
		public static Vector4 operator *( Vector4 v1, Vector4 v2 )
		{
			Vector4 result;
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
		[ShaderGenerationFunction( "{v} * {s}" )]
		public static Vector4 operator *( Vector4 v, double s )
		{
			Vector4 result;
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
		[ShaderGenerationFunction( "{s} * {v}" )]
		public static Vector4 operator *( double s, Vector4 v )
		{
			Vector4 result;
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
		[ShaderGenerationFunction( "{v1} / {v2}" )]
		public static Vector4 operator /( Vector4 v1, Vector4 v2 )
		{
			Vector4 result;
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
		[ShaderGenerationFunction( "{v} / {s}" )]
		public static Vector4 operator /( Vector4 v, double s )
		{
			Vector4 result;
			double invS = 1.0 / s;
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
		[ShaderGenerationFunction( "{s} / {v}" )]
		public static Vector4 operator /( double s, Vector4 v )
		{
			Vector4 result;
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
		[ShaderGenerationFunction( "-{v}" )]
		public static Vector4 operator -( Vector4 v )
		{
			Vector4 result;
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
		public static void Add( ref Vector4 v1, ref Vector4 v2, out Vector4 result )
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
		public static void Subtract( ref Vector4 v1, ref Vector4 v2, out Vector4 result )
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
		public static void Multiply( ref Vector4 v1, ref Vector4 v2, out Vector4 result )
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
		public static void Multiply( ref Vector4 v, double s, out Vector4 result )
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
		public static void Multiply( double s, ref Vector4 v, out Vector4 result )
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
		public static void Divide( ref Vector4 v1, ref Vector4 v2, out Vector4 result )
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
		public static void Divide( ref Vector4 v, double s, out Vector4 result )
		{
			double invS = 1.0 / s;
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
		public static void Divide( double s, ref Vector4 v, out Vector4 result )
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
		public static void Negate( ref Vector4 v, out Vector4 result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
			result.W = -v.W;
		}

		//public static Vec4 Add( Vec4 v1, Vec4 v2 )
		//{
		//	Vec4 result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	result.z = v1.z + v2.z;
		//	result.w = v1.w + v2.w;
		//	return result;
		//}

		//public static Vec4 Subtract( Vec4 v1, Vec4 v2 )
		//{
		//	Vec4 result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	result.z = v1.z - v2.z;
		//	result.w = v1.w - v2.w;
		//	return result;
		//}

		//public static Vec4 Multiply( Vec4 v1, Vec4 v2 )
		//{
		//	Vec4 result;
		//	result.x = v1.x * v2.x;
		//	result.y = v1.y * v2.y;
		//	result.z = v1.z * v2.z;
		//	result.w = v1.w * v2.w;
		//	return result;
		//}

		//public static Vec4 Multiply( Vec4 v, double s )
		//{
		//	Vec4 result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	result.z = v.z * s;
		//	result.w = v.w * s;
		//	return result;
		//}

		//public static Vec4 Multiply( double s, Vec4 v )
		//{
		//	Vec4 result;
		//	result.x = v.x * s;
		//	result.y = v.y * s;
		//	result.z = v.z * s;
		//	result.w = v.w * s;
		//	return result;
		//}

		//public static Vec4 Divide( Vec4 v1, Vec4 v2 )
		//{
		//	Vec4 result;
		//	result.x = v1.x / v2.x;
		//	result.y = v1.y / v2.y;
		//	result.z = v1.z / v2.z;
		//	result.w = v1.w / v2.w;
		//	return result;
		//}

		//public static Vec4 Divide( Vec4 v, double s )
		//{
		//	Vec4 result;
		//	double invS = 1.0 / s;
		//	result.x = v.x * invS;
		//	result.y = v.y * invS;
		//	result.z = v.z * invS;
		//	result.w = v.w * invS;
		//	return result;
		//}

		//public static Vec4 Divide( double s, Vec4 v )
		//{
		//	Vec4 result;
		//	result.x = s / v.x;
		//	result.y = s / v.y;
		//	result.z = s / v.z;
		//	result.w = s / v.w;
		//	return result;
		//}

		//public static Vec4 Negate( Vec4 v )
		//{
		//	Vec4 result;
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
		public static bool operator ==( Vector4 v1, Vector4 v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W );
		}

		/// <summary>
		/// Determines whether two given vectors are unequal.
		/// </summary>
		/// <param name="v1">The first vector to compare.</param>
		/// <param name="v2">The second vector to compare.</param>
		/// <returns>True if the vectors are unequal; False otherwise.</returns>
		public static bool operator !=( Vector4 v1, Vector4 v2 )
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
		public unsafe double this[ int index ]
		{
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( double* v = &this.X )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( double* v = &this.X )
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
		[ShaderGenerationFunction( "dot({v1}, {v2})" )]
		public static double Dot( Vector4 v1, Vector4 v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		public static double Dot( ref Vector4 v1, ref Vector4 v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>When the method completes, contains the dot product of the two vectors.</returns>
		internal static void Dot( ref Vector4 v1, ref Vector4 v2, out double result )
		{
			result = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		public double Dot( Vector4 v )
		{
			return X * v.X + Y * v.Y + Z * v.Z + W * v.W;
		}

		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="v">The second vector.</param>
		/// <returns>The dot product of the two vectors.</returns>
		public double Dot( ref Vector4 v )
		{
			return X * v.X + Y * v.Y + Z * v.Z + W * v.W;
		}

		/// <summary>
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector4"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector4"/>; False otherwise.</returns>
		public bool Equals( Vector4 v, double epsilon )
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
		/// Determines whether the specified vector is equal to the current instance of <see cref="Vector4"/> with a given precision.
		/// </summary>
		/// <param name="v">The vector to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified vector is equal to the current instance of <see cref="Vector4"/>; False otherwise.</returns>
		public bool Equals( ref Vector4 v, double epsilon )
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
		/// Calculates the length of the current instance of <see cref="Vector4"/>.
		/// </summary>
		/// <returns>The length of the current instance of <see cref="Vector4"/>.</returns>
		[ShaderGenerationFunction( "length({this})" )]
		public double Length()
		{
			return Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
		}

		/// <summary>
		/// Calculates the squared length of the current instance of <see cref="Vector4"/>.
		/// </summary>
		/// <returns>The squared length of the current instance of <see cref="Vector4"/>.</returns>
		public double LengthSquared()
		{
			return X * X + Y * Y + Z * Z + W * W;
		}

		void Clamp( Vector4 min, Vector4 max )
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
		/// Restricts the current instance of <see cref="Vector4"/> to be within a specified range and returns the clamped value.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped instance of <see cref="Vector4"/>.</returns>
		[ShaderGenerationFunction( "clamp({this}, {min}, {max})" )]
		public Vector4 GetClamp( Vector4 min, Vector4 max )
		{
			var r = this;
			r.Clamp( min, max );
			return r;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4"/> into a unit vector.
		/// </summary>
		/// <returns>Returns the length of the current instance of <see cref="Vector4"/>.</returns>
		public double Normalize()
		{
			double sqrLength = X * X + Y * Y + Z * Z + W * W;
			double invLength = 1.0 / Math.Sqrt( sqrLength );
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
		public static Vector4 Normalize( Vector4 v )
		{
			double invLength = 1.0 / Math.Sqrt( v.X * v.X + v.Y * v.Y + v.Z * v.Z + v.W * v.W );
			return new Vector4( v.X * invLength, v.Y * invLength, v.Z * invLength, v.W * invLength );
		}

		/// <summary>
		/// Converts a vector into a unit vector.
		/// </summary>
		/// <param name="v">The vector to normalize.</param>
		/// <param name="result">When the method completes, contains the normalized vector.</param>
		public static void Normalize( ref Vector4 v, out Vector4 result )
		{
			double invLength = 1.0 / Math.Sqrt( v.X * v.X + v.Y * v.Y + v.Z * v.Z + v.W * v.W );
			result.X = v.X * invLength;
			result.Y = v.Y * invLength;
			result.Z = v.Z * invLength;
			result.W = v.W * invLength;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4"/> into a unit vector and returns the normalized vector.
		/// </summary>
		/// <returns>The normalized instance of <see cref="Vector4"/>.</returns>
		[ShaderGenerationFunction( "normalize({this})" )]
		public Vector4 GetNormalize()
		{
			double invLength = 1.0 / Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
			return new Vector4( X * invLength, Y * invLength, Z * invLength, W * invLength );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4"/> into an instance of <see cref="Vector2"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
		[AutoConvertType]
		public Vector2 ToVector2()
		{
			Vector2 result;
			result.X = X;
			result.Y = Y;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4"/> into an instance of <see cref="Vector3"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
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
		/// Performs a linear interpolation between two vectors based on the given weighting.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="v2"/>.</param>
		/// <returns>The interpolated vector.</returns>
		[ShaderGenerationFunction( "lerp({v1}, {v2}, {amount})" )]
		public static Vector4 Lerp( Vector4 v1, Vector4 v2, double amount )
		{
			Vector4 result;
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
		public static void Lerp( ref Vector4 v1, ref Vector4 v2, double amount, out Vector4 result )
		{
			result.X = v1.X + ( ( v2.X - v1.X ) * amount );
			result.Y = v1.Y + ( ( v2.Y - v1.Y ) * amount );
			result.Z = v1.Z + ( ( v2.Z - v1.Z ) * amount );
			result.W = v1.W + ( ( v2.W - v1.W ) * amount );
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector4"/> between 0 and 1.
		/// </summary>
		public void Saturate()
		{
			MathEx.Saturate( ref X );
			MathEx.Saturate( ref Y );
			MathEx.Saturate( ref Z );
			MathEx.Saturate( ref W );
		}

		/// <summary>
		/// Clamps the components of the current instance of <see cref="Vector4"/> between 0 and 1 and returns the saturated value.
		/// </summary>
		/// <returns>The saturated value.</returns>
		[ShaderGenerationFunction( "saturate({this})" )]
		public Vector4 GetSaturate()
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
		[ShaderGenerationFunction( "distance({v1}, {v2})" )]
		public static double Distance( Vector4 v1, Vector4 v2 )
		{
			Subtract( ref v1, ref v2, out Vector4 result );
			return result.Length();
		}

		/// <summary>
		/// Calculates the distance between two vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>The distance between two vectors.</returns>
		public static double Distance( ref Vector4 v1, ref Vector4 v2 )
		{
			Subtract( ref v1, ref v2, out Vector4 result );
			return result.Length();
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4"/> into the equivalent <see cref="Vector4F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector4F"/> structure.</returns>
		[AutoConvertType]
		public Vector4F ToVector4F()
		{
			Vector4F result;
			result.X = (float)X;
			result.Y = (float)Y;
			result.Z = (float)Z;
			result.W = (float)W;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4"/> into the equivalent <see cref="Vector4I"/> structure.
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
		/// Converts each component of the current instance of <see cref="Vector4"/> into the
		/// component of the <see cref="ColorValue"/> structure: X to red component,
		/// Y to green component, Z to blue component, W to alpha component.
		/// </summary>
		/// <returns>The equivalent <see cref="ColorValue"/> structure.</returns>
		public ColorValue ToColorValue()
		{
			ColorValue result;
			result.Red = (float)X;
			result.Green = (float)Y;
			result.Blue = (float)Z;
			result.Alpha = (float)W;
			return result;
		}

		/// <summary>
		/// Chooses one of two vectors depending on the <paramref name="pick1"/> value.
		/// </summary>
		/// <param name="v1">The first vector to choose.</param>
		/// <param name="v2">The second vector to choose.</param>
		/// <param name="pick1">If this value is true, the method chooses the virst vector, otherwise it chooses the second one.</param>
		/// <returns>The selected vector.</returns>
		[ShaderGenerationFunction( "{pick1} ? {v1} : {v2}" )]
		public static Vector4 Select( Vector4 v1, Vector4 v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		/// <summary>
		/// Rounds the current instance of <see cref="Vector4"/> towards zero for each component in a vector.
		/// </summary>
		/// <param name="v"></param>
		public void Truncate( Vector4 v )
		{
			X = (long)X;
			Y = (long)Y;
			Z = (long)Z;
			W = (long)W;
		}

		/// <summary>
		/// Rounds a given vector towards zero for each component in it and returns the truncated vector.
		/// </summary>
		/// <param name="v">The vector to truncate.</param>
		/// <returns>The truncated vector</returns>
		[ShaderGenerationFunction( "trunc({v})" )]
		public Vector4 GetTruncate( Vector4 v )
		{
			return new Vector4( (long)v.X, (long)v.Y, (long)v.Z, (long)v.W );
		}

		/// <summary>
		/// Determines whether any component of a given vector is unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if any component of the specified vector is unequal to the zero; otherwise, False.</returns>
		[ShaderGenerationFunction( "any({v})" )]
		public static bool AnyNonZero( Vector4 v )
		{
			return v != Zero;
		}

		/// <summary>
		/// Determines whether all components of a given vector are unequal to the zero.
		/// </summary>
		/// <param name="v">The vector to compare with the zero vector.</param>
		/// <returns>True if all components of the specified vector are unequal to the zero; otherwise, False.</returns>
		[ShaderGenerationFunction( "all({v})" )]
		public static bool AllNonZero( Vector4 v )
		{
			return v.X != 0 && v.Y != 0 && v.Z != 0 && v.W != 0;
		}

		/// <summary>
		/// Returns a vector whose elements are the absolute values of each of the specified vector's components.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The absolute value vector.</returns>
		[ShaderGenerationFunction( "abs({v})" )]
		public static Vector4 Abs( Vector4 v )
		{
			return new Vector4( Math.Abs( v.X ), Math.Abs( v.Y ), Math.Abs( v.Z ), Math.Abs( v.W ) );
		}

		/// <summary>
		/// Calculates the arc-cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose cosines are equal to the
		/// corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "acos({v})" )]
		public static Vector4 Acos( Vector4 v )
		{
			return new Vector4( Math.Acos( v.X ), Math.Acos( v.Y ), Math.Acos( v.Z ), Math.Acos( v.W ) );
		}

		/// <summary>
		/// Calculates the arc-sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose sines are equal to the
		/// corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "asin({v})" )]
		public static Vector4 Asin( Vector4 v )
		{
			return new Vector4( Math.Asin( v.X ), Math.Asin( v.Y ), Math.Asin( v.Z ), Math.Asin( v.W ) );
		}

		/// <summary>
		/// Calculates the arc-tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are equal to the
		/// corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "atan({v})" )]
		public static Vector4 Atan( Vector4 v )
		{
			return new Vector4( Math.Atan( v.X ), Math.Atan( v.Y ), Math.Atan( v.Z ), Math.Atan( v.W ) );
		}

		/// <summary>
		/// Returns the vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.
		/// </summary>
		/// <param name="y">The first vector.</param>
		/// <param name="x">The second vector.</param>
		/// <returns>The vector which contains the angles in radians whose tangents are the quotient of the corresponding components in the first specified vector <paramref name="y"/> and the second specified vector <paramref name="x"/>.</returns>
		[ShaderGenerationFunction( "atan2({y}, {x})" )]
		public static Vector4 Atan2( Vector4 y, Vector4 x )
		{
			return new Vector4( Math.Atan2( y.X, x.X ), Math.Atan2( y.Y, x.Y ), Math.Atan2( y.Z, x.Z ), Math.Atan2( y.W, x.W ) );
		}

		/// <summary>
		/// Calculates the cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the cosines of the corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "cos({v})" )]
		public static Vector4 Cos( Vector4 v )
		{
			return new Vector4( Math.Cos( v.X ), Math.Cos( v.Y ), Math.Cos( v.Z ), Math.Cos( v.W ) );
		}

		/// <summary>
		/// Calculates the hyperbolic cosine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic cosines of the corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "cosh({v})" )]
		public static Vector4 Cosh( Vector4 v )
		{
			return new Vector4( Math.Cosh( v.X ), Math.Cosh( v.Y ), Math.Cosh( v.Z ), Math.Cosh( v.W ) );
		}

		/// <summary>
		/// Returns the vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains e raised to the power of n, where n is the corresponding component in the specified vector.</returns>
		[ShaderGenerationFunction( "exp({v})" )]
		public static Vector4 Exp( Vector4 v )
		{
			return new Vector4( Math.Exp( v.X ), Math.Exp( v.Y ), Math.Exp( v.Z ), Math.Exp( v.W ) );
		}

		/// <summary>
		/// Calculates the natural logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the natural logarithms of the corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "log({v})" )]
		public static Vector4 Log( Vector4 v )
		{
			return new Vector4( Math.Log( v.X ), Math.Log( v.Y ), Math.Log( v.Z ), Math.Log( v.W ) );
		}

		/// <summary>
		/// Calculates the base 10 logarithm of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the base 10 logarithms of the corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "log10({v})" )]
		public static Vector4 Log10( Vector4 v )
		{
			return new Vector4( Math.Log10( v.X ), Math.Log10( v.Y ), Math.Log10( v.Z ), Math.Log10( v.W ) );
		}

		/// <summary>
		/// Returns a vector containing the largest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the largest components of the specified vectors.</returns>
		[ShaderGenerationFunction( "max({v1}, {v2})" )]
		public static Vector4 Max( Vector4 v1, Vector4 v2 )
		{
			return new Vector4( Math.Max( v1.X, v2.X ), Math.Max( v1.Y, v2.Y ), Math.Max( v1.Z, v2.Z ), Math.Max( v1.W, v2.W ) );
		}

		/// <summary>
		/// Returns a vector containing the smallest components of the specified vectors. 
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		/// <returns>A vector containing the smallest components of the specified vectors.</returns>
		[ShaderGenerationFunction( "min({v1}, {v2})" )]
		public static Vector4 Min( Vector4 v1, Vector4 v2 )
		{
			return new Vector4( Math.Min( v1.X, v2.X ), Math.Min( v1.Y, v2.Y ), Math.Min( v1.Z, v2.Z ), Math.Min( v1.W, v2.W ) );
		}

		/// <summary>
		/// Returns the vector which contains the components of the first specified vector raised to power of the numbers which are equal to the corresponding components of the second specified vector.
		/// </summary>
		/// <param name="x">The first vector.</param>
		/// <param name="y">The second vector.</param>
		/// <returns>The vector which contains the components of the first specified vector raised to power of
		/// the numbers which are equal to the corresponding components of the second specified vector.</returns>
		[ShaderGenerationFunction( "pow({x}, {y})" )]
		public static Vector4 Pow( Vector4 x, Vector4 y )
		{
			return new Vector4( Math.Pow( x.X, y.X ), Math.Pow( x.Y, y.Y ), Math.Pow( x.Z, y.Z ), Math.Pow( x.W, y.W ) );
		}

		/// <summary>
		/// Calculates the sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the sines of the corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "sin({v})" )]
		public static Vector4 Sin( Vector4 v )
		{
			return new Vector4( Math.Sin( v.X ), Math.Sin( v.Y ), Math.Sin( v.Z ), Math.Sin( v.W ) );
		}

		/// <summary>
		/// Calculates the hyperbolic sine of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic sines of the corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "sinh({v})" )]
		public static Vector4 Sinh( Vector4 v )
		{
			return new Vector4( Math.Sinh( v.X ), Math.Sinh( v.Y ), Math.Sinh( v.Z ), Math.Sinh( v.W ) );
		}

		/// <summary>
		/// Calculates the square root of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the square root of the corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "sqrt({v})" )]
		public static Vector4 Sqrt( Vector4 v )
		{
			return new Vector4( Math.Sqrt( v.X ), Math.Sqrt( v.Y ), Math.Sqrt( v.Z ), Math.Sqrt( v.W ) );
		}

		/// <summary>
		/// Calculates the tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the tangents of the corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "tan({v})" )]
		public static Vector4 Tan( Vector4 v )
		{
			return new Vector4( Math.Tan( v.X ), Math.Tan( v.Y ), Math.Tan( v.Z ), Math.Tan( v.W ) );
		}

		/// <summary>
		/// Calculates the hyperbolic tangent of each component of the specified vector.
		/// </summary>
		/// <param name="v">The specified vector.</param>
		/// <returns>The vector which contains the hyperbolic tangents of the corresponding components in the specified vector.</returns>
		[ShaderGenerationFunction( "tanh({v})" )]
		public static Vector4 Tanh( Vector4 v )
		{
			return new Vector4( Math.Tanh( v.X ), Math.Tanh( v.Y ), Math.Tanh( v.Z ), Math.Tanh( v.W ) );
		}

		/// <summary>
		/// Returns the value of the smallest component of the current instance of <see cref="Vector4"/>.
		/// </summary>
		/// <returns>The value of the smallest component of the current instance of <see cref="Vector4"/>.</returns>
		public double MinComponent()
		{
			return Math.Min( Math.Min( X, Y ), Math.Min( Z, W ) );
		}

		/// <summary>
		/// Returns the value of the largest component of the current instance of <see cref="Vector4"/>.
		/// </summary>
		/// <returns>The value of the largest component of the current instance of <see cref="Vector4"/>.</returns>
		public double MaxComponent()
		{
			return Math.Max( Math.Max( X, Y ), Math.Max( Z, W ) );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector4"/> into the equivalent <see cref="Plane"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Plane"/> structure.</returns>
		public Plane ToPlane()
		{
			return new Plane( this );
		}
	}
}
