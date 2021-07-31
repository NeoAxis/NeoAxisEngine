// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A structure encapsulating a single precision 3x3 matrix.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Matrix3F
	{
		/// <summary>
		/// The first row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector3F Item0;
		/// <summary>
		/// The second row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector3F Item1;
		/// <summary>
		/// The third row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector3F Item2;

		/// <summary>
		/// The matrix with all of its components set to zero.
		/// </summary>
		public static readonly Matrix3F Zero = new Matrix3F( 0, 0, 0, 0, 0, 0, 0, 0, 0 );
		/// <summary>
		/// The identity matrix.
		/// </summary>
		public static readonly Matrix3F Identity = new Matrix3F( 1, 0, 0, 0, 1, 0, 0, 0, 1 );

		/// <summary>
		/// Constructs a matrix with the given individual elements.
		/// </summary>
		/// <param name="xx">Value at row 1 column 1 of the matrix.</param>
		/// <param name="xy">Value at row 1 column 2 of the matrix.</param>
		/// <param name="xz">Value at row 1 column 3 of the matrix.</param>
		/// <param name="yx">Value at row 2 column 1 of the matrix.</param>
		/// <param name="yy">Value at row 2 column 2 of the matrix.</param>
		/// <param name="yz">Value at row 2 column 3 of the matrix.</param>
		/// <param name="zx">Value at row 3 column 1 of the matrix.</param>
		/// <param name="zy">Value at row 3 column 2 of the matrix.</param>
		/// <param name="zz">Value at row 3 column 3 of the matrix.</param>
		public Matrix3F( float xx, float xy, float xz, float yx, float yy, float yz, float zx, float zy, float zz )
		{
			Item0 = new Vector3F( xx, xy, xz );
			Item1 = new Vector3F( yx, yy, yz );
			Item2 = new Vector3F( zx, zy, zz );
		}

		/// <summary>
		/// Constructs a matrix with the specified <see cref="Vector3F"/> elements,
		/// which are the corresponding rows of the matrix.
		/// </summary>
		/// <param name="x">The vector which is the first row.</param>
		/// <param name="y">The vector which is the second row.</param>
		/// <param name="z">The vector which is the third row.</param>
		public Matrix3F( Vector3F x, Vector3F y, Vector3F z )
		{
			Item0 = x;
			Item1 = y;
			Item2 = z;
		}

		/// <summary>
		/// Constructs a matrix with another specified <see cref="Matrix3F"/> object.
		/// </summary>
		/// <param name="source">A specified matrix.</param>
		public Matrix3F( Matrix3F source )
		{
			Item0 = source.Item0;
			Item1 = source.Item1;
			Item2 = source.Item2;
		}

		/// <summary>
		/// Gets or sets the row of the current instance of <see cref="Matrix3F"/> at the specified index.
		/// </summary>
		/// <value>The value of the corresponding row of <see cref="Vector3F"/> format, depending on the index.</value>
		/// <param name="index">The index of the row to access.</param>
		/// <returns>The value of the row of <see cref="Vector3F"/> format at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
		public unsafe Vector3F this[ int index ]
		{
			get
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3F* v = &this.Item0 )
					return v[ index ];
			}
			set
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3F* v = &this.Item0 )
					v[ index ] = value;
			}
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return Item0.GetHashCode() ^ Item1.GetHashCode() ^ Item2.GetHashCode();
		}

		/// <summary>
		/// Adds two matricies.
		/// </summary>
		/// <param name="v1">The first matrix to add.</param>
		/// <param name="v2">The second matrix to add.</param>
		/// <returns>The sum of the two matricies.</returns>
		public static Matrix3F operator +( Matrix3F v1, Matrix3F v2 )
		{
			Matrix3F result;
			result.Item0.X = v1.Item0.X + v2.Item0.X;
			result.Item0.Y = v1.Item0.Y + v2.Item0.Y;
			result.Item0.Z = v1.Item0.Z + v2.Item0.Z;
			result.Item1.X = v1.Item1.X + v2.Item1.X;
			result.Item1.Y = v1.Item1.Y + v2.Item1.Y;
			result.Item1.Z = v1.Item1.Z + v2.Item1.Z;
			result.Item2.X = v1.Item2.X + v2.Item2.X;
			result.Item2.Y = v1.Item2.Y + v2.Item2.Y;
			result.Item2.Z = v1.Item2.Z + v2.Item2.Z;
			return result;
		}

		/// <summary>
		/// Subtracts two matricies.
		/// </summary>
		/// <param name="v1">The matrix to subtract from.</param>
		/// <param name="v2">The matrix to be subtracted from another matrix.</param>
		/// <returns>The difference between the two matricies.</returns>
		public static Matrix3F operator -( Matrix3F v1, Matrix3F v2 )
		{
			Matrix3F result;
			result.Item0.X = v1.Item0.X - v2.Item0.X;
			result.Item0.Y = v1.Item0.Y - v2.Item0.Y;
			result.Item0.Z = v1.Item0.Z - v2.Item0.Z;
			result.Item1.X = v1.Item1.X - v2.Item1.X;
			result.Item1.Y = v1.Item1.Y - v2.Item1.Y;
			result.Item1.Z = v1.Item1.Z - v2.Item1.Z;
			result.Item2.X = v1.Item2.X - v2.Item2.X;
			result.Item2.Y = v1.Item2.Y - v2.Item2.Y;
			result.Item2.Z = v1.Item2.Z - v2.Item2.Z;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="s">The value by which to multiply.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix3F operator *( Matrix3F m, float s )
		{
			Matrix3F result;
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item0.Z = m.Item0.Z * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			result.Item1.Z = m.Item1.Z * s;
			result.Item2.X = m.Item2.X * s;
			result.Item2.Y = m.Item2.Y * s;
			result.Item2.Z = m.Item2.Z * s;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="s">The value by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix3F operator *( float s, Matrix3F m )
		{
			Matrix3F result;
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item0.Z = m.Item0.Z * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			result.Item1.Z = m.Item1.Z * s;
			result.Item2.X = m.Item2.X * s;
			result.Item2.Y = m.Item2.Y * s;
			result.Item2.Z = m.Item2.Z * s;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3F"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector3F operator *( Matrix3F m, Vector3F v )
		{
			Vector3F result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3F"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector3F operator *( Vector3F v, Matrix3F m )
		{
			Vector3F result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z;
			return result;
		}

		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="v1">The first matrix to multiply.</param>
		/// <param name="v2">The second matrix to multiply.</param>
		/// <returns>The product of the two matricies.</returns>
		public static Matrix3F operator *( Matrix3F v1, Matrix3F v2 )
		{
			Matrix3F result;
			result.Item0.X = v1.Item0.X * v2.Item0.X + v1.Item1.X * v2.Item0.Y + v1.Item2.X * v2.Item0.Z;
			result.Item0.Y = v1.Item0.Y * v2.Item0.X + v1.Item1.Y * v2.Item0.Y + v1.Item2.Y * v2.Item0.Z;
			result.Item0.Z = v1.Item0.Z * v2.Item0.X + v1.Item1.Z * v2.Item0.Y + v1.Item2.Z * v2.Item0.Z;
			result.Item1.X = v1.Item0.X * v2.Item1.X + v1.Item1.X * v2.Item1.Y + v1.Item2.X * v2.Item1.Z;
			result.Item1.Y = v1.Item0.Y * v2.Item1.X + v1.Item1.Y * v2.Item1.Y + v1.Item2.Y * v2.Item1.Z;
			result.Item1.Z = v1.Item0.Z * v2.Item1.X + v1.Item1.Z * v2.Item1.Y + v1.Item2.Z * v2.Item1.Z;
			result.Item2.X = v1.Item0.X * v2.Item2.X + v1.Item1.X * v2.Item2.Y + v1.Item2.X * v2.Item2.Z;
			result.Item2.Y = v1.Item0.Y * v2.Item2.X + v1.Item1.Y * v2.Item2.Y + v1.Item2.Y * v2.Item2.Z;
			result.Item2.Z = v1.Item0.Z * v2.Item2.X + v1.Item1.Z * v2.Item2.Y + v1.Item2.Z * v2.Item2.Z;
			return result;
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="v">The matrix to negate.</param>
		/// <returns>The negated matrix.</returns>
		public static Matrix3F operator -( Matrix3F v )
		{
			Matrix3F result;
			result.Item0.X = -v.Item0.X;
			result.Item0.Y = -v.Item0.Y;
			result.Item0.Z = -v.Item0.Z;
			result.Item1.X = -v.Item1.X;
			result.Item1.Y = -v.Item1.Y;
			result.Item1.Z = -v.Item1.Z;
			result.Item2.X = -v.Item2.X;
			result.Item2.Y = -v.Item2.Y;
			result.Item2.Z = -v.Item2.Z;
			return result;
		}

		/// <summary>
		/// Adds two matricies.
		/// </summary>
		/// <param name="v1">The first matrix to add.</param>
		/// <param name="v2">The second matrix to add.</param>
		/// <param name="result">When the method completes, contains the sum of the two matricies.</param>
		public static void Add( ref Matrix3F v1, ref Matrix3F v2, out Matrix3F result )
		{
			result.Item0.X = v1.Item0.X + v2.Item0.X;
			result.Item0.Y = v1.Item0.Y + v2.Item0.Y;
			result.Item0.Z = v1.Item0.Z + v2.Item0.Z;
			result.Item1.X = v1.Item1.X + v2.Item1.X;
			result.Item1.Y = v1.Item1.Y + v2.Item1.Y;
			result.Item1.Z = v1.Item1.Z + v2.Item1.Z;
			result.Item2.X = v1.Item2.X + v2.Item2.X;
			result.Item2.Y = v1.Item2.Y + v2.Item2.Y;
			result.Item2.Z = v1.Item2.Z + v2.Item2.Z;
		}

		/// <summary>
		/// Subtracts two matricies.
		/// </summary>
		/// <param name="v1">The matrix to subtract from.</param>
		/// <param name="v2">The matrix to be subtracted from another matrix.</param>
		/// <param name="result">When the method completes, contains the difference of the two matricies.</param>
		public static void Subtract( ref Matrix3F v1, ref Matrix3F v2, out Matrix3F result )
		{
			result.Item0.X = v1.Item0.X - v2.Item0.X;
			result.Item0.Y = v1.Item0.Y - v2.Item0.Y;
			result.Item0.Z = v1.Item0.Z - v2.Item0.Z;
			result.Item1.X = v1.Item1.X - v2.Item1.X;
			result.Item1.Y = v1.Item1.Y - v2.Item1.Y;
			result.Item1.Z = v1.Item1.Z - v2.Item1.Z;
			result.Item2.X = v1.Item2.X - v2.Item2.X;
			result.Item2.Y = v1.Item2.Y - v2.Item2.Y;
			result.Item2.Z = v1.Item2.Z - v2.Item2.Z;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="s">The value by which to multiply.</param>
		/// <param name="result">When the method completes, contains the scaled matrix.</param>
		public static void Multiply( ref Matrix3F m, float s, out Matrix3F result )
		{
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item0.Z = m.Item0.Z * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			result.Item1.Z = m.Item1.Z * s;
			result.Item2.X = m.Item2.X * s;
			result.Item2.Y = m.Item2.Y * s;
			result.Item2.Z = m.Item2.Z * s;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="s">The value by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the scaled matrix.</param>
		public static void Multiply( float s, ref Matrix3F m, out Matrix3F result )
		{
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item0.Z = m.Item0.Z * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			result.Item1.Z = m.Item1.Z * s;
			result.Item2.X = m.Item2.X * s;
			result.Item2.Y = m.Item2.Y * s;
			result.Item2.Z = m.Item2.Z * s;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		public static void Multiply( ref Vector3F v, ref Matrix3F m, out Vector3F result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		public static void Multiply( ref Matrix3F m, ref Vector3F v, out Vector3F result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z;
		}

		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="v1">The first matrix to multiply.</param>
		/// <param name="v2">The second matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the product of the two matricies.</param>
		public static void Multiply( ref Matrix3F v1, ref Matrix3F v2, out Matrix3F result )
		{
			result.Item0.X = v1.Item0.X * v2.Item0.X + v1.Item1.X * v2.Item0.Y + v1.Item2.X * v2.Item0.Z;
			result.Item0.Y = v1.Item0.Y * v2.Item0.X + v1.Item1.Y * v2.Item0.Y + v1.Item2.Y * v2.Item0.Z;
			result.Item0.Z = v1.Item0.Z * v2.Item0.X + v1.Item1.Z * v2.Item0.Y + v1.Item2.Z * v2.Item0.Z;
			result.Item1.X = v1.Item0.X * v2.Item1.X + v1.Item1.X * v2.Item1.Y + v1.Item2.X * v2.Item1.Z;
			result.Item1.Y = v1.Item0.Y * v2.Item1.X + v1.Item1.Y * v2.Item1.Y + v1.Item2.Y * v2.Item1.Z;
			result.Item1.Z = v1.Item0.Z * v2.Item1.X + v1.Item1.Z * v2.Item1.Y + v1.Item2.Z * v2.Item1.Z;
			result.Item2.X = v1.Item0.X * v2.Item2.X + v1.Item1.X * v2.Item2.Y + v1.Item2.X * v2.Item2.Z;
			result.Item2.Y = v1.Item0.Y * v2.Item2.X + v1.Item1.Y * v2.Item2.Y + v1.Item2.Y * v2.Item2.Z;
			result.Item2.Z = v1.Item0.Z * v2.Item2.X + v1.Item1.Z * v2.Item2.Y + v1.Item2.Z * v2.Item2.Z;
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="m">The matrix to negate.</param>
		/// <param name="result">When the method completes, contains the negated matrix.</param>
		public static void Negate( ref Matrix3F m, out Matrix3F result )
		{
			result.Item0.X = -m.Item0.X;
			result.Item0.Y = -m.Item0.Y;
			result.Item0.Z = -m.Item0.Z;
			result.Item1.X = -m.Item1.X;
			result.Item1.Y = -m.Item1.Y;
			result.Item1.Z = -m.Item1.Z;
			result.Item2.X = -m.Item2.X;
			result.Item2.Y = -m.Item2.Y;
			result.Item2.Z = -m.Item2.Z;
		}

		//public static Mat3F Add( Mat3F v1, Mat3F v2 )
		//{
		//	Mat3F result;
		//	Add( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat3F Subtract( Mat3F v1, Mat3F v2 )
		//{
		//	Mat3F result;
		//	Subtract( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat3F Multiply( Mat3F m, float s )
		//{
		//	Mat3F result;
		//	Multiply( ref m, s, out result );
		//	return result;
		//}

		//public static Mat3F Multiply( float s, Mat3F m )
		//{
		//	Mat3F result;
		//	Multiply( ref m, s, out result );
		//	return result;
		//}

		//public static Vec3F Multiply( Mat3F m, Vec3F v )
		//{
		//	Vec3F result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Vec3F Multiply( Vec3F v, Mat3F m )
		//{
		//	Vec3F result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Mat3F Multiply( Mat3F v1, Mat3F v2 )
		//{
		//	Mat3F result;
		//	Multiply( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat3F Negate( Mat3F m )
		//{
		//	Mat3F result;
		//	Negate( ref m, out result );
		//	return result;
		//}

		/// <summary>
		/// Determines whether the specified matrix is equal to the current instance of <see cref="Matrix3F"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The matrix to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified matrix is equal to the current instance of <see cref="Matrix3F"/>; False otherwise.</returns>
		public bool Equals( Matrix3F v, float epsilon )
		{
			if( !Item0.Equals( ref v.Item0, epsilon ) )
				return false;
			if( !Item1.Equals( ref v.Item1, epsilon ) )
				return false;
			if( !Item2.Equals( ref v.Item2, epsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified matrix is equal to the current instance of <see cref="Matrix3F"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The matrix to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified matrix is equal to the current instance of <see cref="Matrix3F"/>; False otherwise.</returns>
		public bool Equals( ref Matrix3F v, float epsilon )
		{
			if( !Item0.Equals( ref v.Item0, epsilon ) )
				return false;
			if( !Item1.Equals( ref v.Item1, epsilon ) )
				return false;
			if( !Item2.Equals( ref v.Item2, epsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Gets the trace of the matrix, the sum of the values along the diagonal.
		/// </summary>
		/// <returns>The trace of the matrix.</returns>
		public float GetTrace()
		{
			return Item0.X + Item1.Y + Item2.Z;
		}

		//public float GetDeterminant()
		//{
		//   float det2_12_01 = mat1.x * mat2.y - mat1.y * mat2.x;
		//   float det2_12_02 = mat1.x * mat2.z - mat1.z * mat2.x;
		//   float det2_12_12 = mat1.y * mat2.z - mat1.z * mat2.y;

		//   return mat0.x * det2_12_12 - mat0.y * det2_12_02 + mat0.z * det2_12_01;
		//}

		/// <summary>
		/// Transposes the matrix.
		/// </summary>
		public void Transpose()
		{
			float v;
			v = Item0.Y; Item0.Y = Item1.X; Item1.X = v;
			v = Item0.Z; Item0.Z = Item2.X; Item2.X = v;
			v = Item1.Z; Item1.Z = Item2.Y; Item2.Y = v;
		}

		/// <summary>
		/// Returns the transpose of the current instance of <see cref="Matrix3F"/>.
		/// </summary>
		/// <returns>The transpose of the current instance of <see cref="Matrix3F"/>.</returns>
		public Matrix3F GetTranspose()
		{
			Matrix3F result;
			result.Item0.X = Item0.X;
			result.Item0.Y = Item1.X;
			result.Item0.Z = Item2.X;
			result.Item1.X = Item0.Y;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = Item2.Y;
			result.Item2.X = Item0.Z;
			result.Item2.Y = Item1.Z;
			result.Item2.Z = Item2.Z;
			return result;
		}

		/// <summary>
		/// Calculates the transpose of the current instance of <see cref="Matrix3F"/>.
		/// </summary>
		/// <param name="result">When the method completes, contains the transpose of the current instance of <see cref="Matrix3F"/>.</param>
		public void GetTranspose( out Matrix3F result )
		{
			result.Item0.X = Item0.X;
			result.Item0.Y = Item1.X;
			result.Item0.Z = Item2.X;
			result.Item1.X = Item0.Y;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = Item2.Y;
			result.Item2.X = Item0.Z;
			result.Item2.Y = Item1.Z;
			result.Item2.Z = Item2.Z;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix3F"/> and determines whether the matrix is invertible.
		/// Determines whether the current instance of <see cref="Matrix3F"/> is invertible and, if so, inverts this matrix.
		/// </summary>
		/// <returns>True if the current instance of <see cref="Matrix3F"/> is invertible; False otherwise.</returns>
		public bool Inverse()
		{
			Matrix3F inverse;
			double det, invDet;

			inverse.Item0.X = Item1.Y * Item2.Z - Item1.Z * Item2.Y;
			inverse.Item1.X = Item1.Z * Item2.X - Item1.X * Item2.Z;
			inverse.Item2.X = Item1.X * Item2.Y - Item1.Y * Item2.X;

			det = Item0.X * inverse.Item0.X + Item0.Y * inverse.Item1.X + Item0.Z * inverse.Item2.X;

			if( Math.Abs( det ) < 1e-14 )
				return false;

			invDet = 1.0f / det;

			inverse.Item0.Y = Item0.Z * Item2.Y - Item0.Y * Item2.Z;
			inverse.Item0.Z = Item0.Y * Item1.Z - Item0.Z * Item1.Y;
			inverse.Item1.Y = Item0.X * Item2.Z - Item0.Z * Item2.X;
			inverse.Item1.Z = Item0.Z * Item1.X - Item0.X * Item1.Z;
			inverse.Item2.Y = Item0.Y * Item2.X - Item0.X * Item2.Y;
			inverse.Item2.Z = Item0.X * Item1.Y - Item0.Y * Item1.X;

			Item0.X = (float)( (double)inverse.Item0.X * invDet );
			Item0.Y = (float)( (double)inverse.Item0.Y * invDet );
			Item0.Z = (float)( (double)inverse.Item0.Z * invDet );

			Item1.X = (float)( (double)inverse.Item1.X * invDet );
			Item1.Y = (float)( (double)inverse.Item1.Y * invDet );
			Item1.Z = (float)( (double)inverse.Item1.Z * invDet );

			Item2.X = (float)( (double)inverse.Item2.X * invDet );
			Item2.Y = (float)( (double)inverse.Item2.Y * invDet );
			Item2.Z = (float)( (double)inverse.Item2.Z * invDet );

			return true;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix3F"/> if it is invertible and returns the result.
		/// </summary>
		/// <returns>If the current instance of <see cref="Matrix3F"/> is invertible, returns the inverted matrix;
		/// otherwise returns the original matrix.</returns>
		public Matrix3F GetInverse()
		{
			Matrix3F result = this;
			result.Inverse();
			return result;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix3F"/> if it is invertible.
		/// </summary>
		/// <param name="result">When the method completes, contains the inverted matrix if the current instance
		/// of <see cref="Matrix3F"/> is invertible; otherwise, contains the original matrix.</param>
		public void GetInverse( out Matrix3F result )
		{
			result = this;
			result.Inverse();
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix3F"/> to Euler angles.
		/// </summary>
		/// <returns>The result of convertion.</returns>
		public AnglesF ToAngles()
		{
			AnglesF result;

			float sp = Item2.X;
			// cap off our sin value so that we don't get any NANs
			if( sp > 1.0f )
				sp = 1.0f;
			else if( sp < -1.0f )
				sp = -1.0f;

			double theta = -Math.Asin( sp );
			double cp = Math.Cos( theta );

			if( cp > 8192.0f * 1.192092896e-07f )
			{
				result.Roll = MathEx.RadianToDegree( (float)Math.Atan2( Item2.Y, Item2.Z ) );
				result.Pitch = MathEx.RadianToDegree( (float)theta );
				result.Yaw = MathEx.RadianToDegree( (float)Math.Atan2( Item1.X, Item0.X ) );
			}
			else
			{
				result.Roll = 0;
				result.Pitch = MathEx.RadianToDegree( (float)theta );
				result.Yaw = MathEx.RadianToDegree( (float)-Math.Atan2( Item0.Y, Item1.Y ) );
			}

			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix3F"/> to Euler angles.
		/// </summary>
		/// <param name="result">When the method completes, contains the result of convertion.</param>
		public void ToAngles( out AnglesF result )
		{
			float sp = Item2.X;
			// cap off our sin value so that we don't get any NANs
			if( sp > 1.0f )
				sp = 1.0f;
			else if( sp < -1.0f )
				sp = -1.0f;

			double theta = -Math.Asin( sp );
			double cp = Math.Cos( theta );

			if( cp > 8192.0f * 1.192092896e-07f )
			{
				result.Roll = MathEx.RadianToDegree( (float)Math.Atan2( Item2.Y, Item2.Z ) );
				result.Pitch = MathEx.RadianToDegree( (float)theta );
				result.Yaw = MathEx.RadianToDegree( (float)Math.Atan2( Item1.X, Item0.X ) );
			}
			else
			{
				result.Roll = 0;
				result.Pitch = MathEx.RadianToDegree( (float)theta );
				result.Yaw = MathEx.RadianToDegree( (float)-Math.Atan2( Item0.Y, Item1.Y ) );
			}
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix3F"/> into the equivalent <see cref="QuaternionF"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="QuaternionF"/> structure.</returns>
		[AutoConvertType]
		public QuaternionF ToQuaternion()
		{
			QuaternionF result;

			float s;
			float t;

			float trace = Item0.X + Item1.Y + Item2.Z;

			if( trace > 0.0f )
			{
				t = trace + 1.0f;
				s = MathEx.InvSqrt( t ) * 0.5f;

				result.X = ( Item1.Z - Item2.Y ) * s;
				result.Y = ( Item2.X - Item0.Z ) * s;
				result.Z = ( Item0.Y - Item1.X ) * s;
				result.W = s * t;
			}
			else
			{
				int i = 0;
				if( Item1.Y > Item0.X )
					i = 1;

				if( Item2.Z > this[ i, i ] )
					i = 2;

				int j = i + 1;
				if( j > 2 )
					j = 0;

				int k = j + 1;
				if( k > 2 )
					k = 0;

				t = ( this[ i, i ] - ( this[ j, j ] + this[ k, k ] ) ) + 1.0f;
				s = MathEx.InvSqrt( t ) * 0.5f;

				result = QuaternionF.Identity;
				result[ i ] = s * t;
				result[ 3 ] = ( this[ j, k ] - this[ k, j ] ) * s;
				result[ j ] = ( this[ i, j ] + this[ j, i ] ) * s;
				result[ k ] = ( this[ i, k ] + this[ k, i ] ) * s;
			}

			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix3F"/> into the equivalent <see cref="QuaternionF"/> structure.
		/// </summary>
		/// <param name="result">When the method completes, contains the equivalent <see cref="QuaternionF"/> structure.</param>
		public void ToQuaternion( out QuaternionF result )
		{
			float s;
			float t;

			float trace = Item0.X + Item1.Y + Item2.Z;

			if( trace > 0.0f )
			{
				t = trace + 1.0f;
				s = MathEx.InvSqrt( t ) * 0.5f;

				result.X = ( Item1.Z - Item2.Y ) * s;
				result.Y = ( Item2.X - Item0.Z ) * s;
				result.Z = ( Item0.Y - Item1.X ) * s;
				result.W = s * t;
			}
			else
			{
				int i = 0;
				if( Item1.Y > Item0.X )
					i = 1;

				if( Item2.Z > this[ i, i ] )
					i = 2;

				int j = i + 1;
				if( j > 2 )
					j = 0;

				int k = j + 1;
				if( k > 2 )
					k = 0;

				t = ( this[ i, i ] - ( this[ j, j ] + this[ k, k ] ) ) + 1.0f;
				s = MathEx.InvSqrt( t ) * 0.5f;

				result = QuaternionF.Identity;
				result[ i ] = s * t;
				result[ 3 ] = ( this[ j, k ] - this[ k, j ] ) * s;
				result[ j ] = ( this[ i, j ] + this[ j, i ] ) * s;
				result[ k ] = ( this[ i, k ] + this[ k, i ] ) * s;
			}
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix3F"/> into the equivalent <see cref="Matrix4F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Matrix4F"/> structure.</returns>
		[AutoConvertType]
		public Matrix4F ToMatrix4()
		{
			Matrix4F result;
			result.Item0.X = Item0.X;
			result.Item0.Y = Item0.Y;
			result.Item0.Z = Item0.Z;
			result.Item0.W = 0;
			result.Item1.X = Item1.X;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = Item1.Z;
			result.Item1.W = 0;
			result.Item2.X = Item2.X;
			result.Item2.Y = Item2.Y;
			result.Item2.Z = Item2.Z;
			result.Item2.W = 0;
			result.Item3.X = 0;
			result.Item3.Y = 0;
			result.Item3.Z = 0;
			result.Item3.W = 1;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix3F"/> into the equivalent <see cref="Matrix4F"/> structure.
		/// </summary>
		/// <param name="result">When the method completes, contains the equivalent <see cref="Matrix4F"/> structure.</param>
		public void ToMatrix4( out Matrix4F result )
		{
			result.Item0.X = Item0.X;
			result.Item0.Y = Item0.Y;
			result.Item0.Z = Item0.Z;
			result.Item0.W = 0;
			result.Item1.X = Item1.X;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = Item1.Z;
			result.Item1.W = 0;
			result.Item2.X = Item2.X;
			result.Item2.Y = Item2.Y;
			result.Item2.Z = Item2.Z;
			result.Item2.W = 0;
			result.Item3.X = 0;
			result.Item3.Y = 0;
			result.Item3.Z = 0;
			result.Item3.W = 1;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Matrix3F"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Matrix3F"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Matrix3F"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Matrix3F && this == (Matrix3F)obj );
		}

		/// <summary>
		/// Determines whether two given matricies are equal.
		/// </summary>
		/// <param name="v1">The first matrix to compare.</param>
		/// <param name="v2">The second matrix to compare.</param>
		/// <returns>True if the matricies are equal; False otherwise.</returns>
		public static bool operator ==( Matrix3F v1, Matrix3F v2 )
		{
			return v1.Item0 == v2.Item0 && v1.Item1 == v2.Item1 && v1.Item2 == v2.Item2;
		}

		/// <summary>
		/// Determines whether two given matricies are unequal.
		/// </summary>
		/// <param name="v1">The first matrix to compare.</param>
		/// <param name="v2">The second matrix to compare.</param>
		/// <returns>True if the matricies are unequal; False otherwise.</returns>
		public static bool operator !=( Matrix3F v1, Matrix3F v2 )
		{
			return v1.Item0 != v2.Item0 || v1.Item1 != v2.Item1 || v1.Item2 != v2.Item2;
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the matrix component, depending on the index.</value>
		/// <param name="row">The row of the matrix to access.</param>
		/// <param name="column">The column of the matrix to access.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>
		/// is out of the range [0, 2].</exception>
		public unsafe float this[ int row, int column ]
		{
			get
			{
				if( row < 0 || row > 2 )
					throw new ArgumentOutOfRangeException( "row" );
				if( column < 0 || column > 2 )
					throw new ArgumentOutOfRangeException( "column" );
				fixed ( float* v = &this.Item0.X )
					return v[ row * 3 + column ];
			}
			set
			{
				if( row < 0 || row > 2 )
					throw new ArgumentOutOfRangeException( "row" );
				if( column < 0 || column > 2 )
					throw new ArgumentOutOfRangeException( "column" );
				fixed ( float* v = &this.Item0.X )
					v[ row * 3 + column ] = value;
			}
		}

		/// <summary>
		/// Converts a string representation of a matrix into the equivalent <see cref="Matrix3F"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the matrix (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Matrix3F"/> structure.</returns>
		[AutoConvertType]
		public static Matrix3F Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 9 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 9 parts separated by spaces.", text ) );

			try
			{
				return new Matrix3F(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ),
					float.Parse( vals[ 2 ] ),
					float.Parse( vals[ 3 ] ),
					float.Parse( vals[ 4 ] ),
					float.Parse( vals[ 5 ] ),
					float.Parse( vals[ 6 ] ),
					float.Parse( vals[ 7 ] ),
					float.Parse( vals[ 8 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Matrix3F"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Matrix3F"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1} {2}", Item0.ToString(), Item1.ToString(), Item2.ToString() );
		}

		/// <summary>
		/// Creates a scale matrix with the specified <see cref="Vector3F"/> object.
		/// </summary>
		/// <param name="scale">The specified vector.</param>
		/// <returns>The scale matrix.</returns>
		public static Matrix3F FromScale( Vector3F scale )
		{
			Matrix3F result;
			result.Item0.X = scale.X;
			result.Item0.Y = 0;
			result.Item0.Z = 0;
			result.Item1.X = 0;
			result.Item1.Y = scale.Y;
			result.Item1.Z = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = scale.Z;
			return result;
		}

		/// <summary>
		/// Creates a scale matrix with the specified <see cref="Vector3F"/> object.
		/// </summary>
		/// <param name="scale">The specified vector.</param>
		/// <param name="result">When the method completes, contains the scale matrix.</param>
		public static void FromScale( ref Vector3F scale, out Matrix3F result )
		{
			result.Item0.X = scale.X;
			result.Item0.Y = 0;
			result.Item0.Z = 0;
			result.Item1.X = 0;
			result.Item1.Y = scale.Y;
			result.Item1.Z = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = scale.Z;
		}

		/// <summary>
		/// Creates a scale matrix with the specified <see cref="float"/> value.
		/// </summary>
		/// <param name="scale">The specified <see cref="float"/> value.</param>
		/// <returns>The scale matrix.</returns>
		public static Matrix3F FromScale( float scale )
		{
			Matrix3F result;
			result.Item0.X = scale;
			result.Item0.Y = 0;
			result.Item0.Z = 0;
			result.Item1.X = 0;
			result.Item1.Y = scale;
			result.Item1.Z = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = scale;
			return result;
		}

		/// <summary>
		/// Creates a scale matrix with the specified <see cref="float"/> value.
		/// </summary>
		/// <param name="scale">The specified <see cref="float"/> value.</param>
		/// <param name="result">When the method completes, contains the scale matrix.</param>
		public static void FromScale( float scale, out Matrix3F result )
		{
			result.Item0.X = scale;
			result.Item0.Y = 0;
			result.Item0.Z = 0;
			result.Item1.X = 0;
			result.Item1.Y = scale;
			result.Item1.Z = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = scale;
		}

		/// <summary>
		/// Creates a matrix that rotates around the x-axis.
		/// </summary>
		/// <param name="angle">The angle in radians.</param>
		/// <returns>The resulting rotation matrix.</returns>
		public static Matrix3F FromRotateByX( RadianF angle )
		{
			Matrix3F result;
			float sin = MathEx.Sin( angle );
			float cos = MathEx.Cos( angle );
			result.Item0.X = 1;
			result.Item0.Y = 0;
			result.Item0.Z = 0;
			result.Item1.X = 0;
			result.Item1.Y = cos;
			result.Item1.Z = -sin;
			result.Item2.X = 0;
			result.Item2.Y = sin;
			result.Item2.Z = cos;
			return result;
		}

		/// <summary>
		/// Creates a matrix that rotates around the x-axis.
		/// </summary>
		/// <param name="angle">The angle in radians.</param>
		/// <param name="result">When the method completes, contains the resulting rotation matrix.</param>
		public static void FromRotateByX( RadianF angle, out Matrix3F result )
		{
			float sin = MathEx.Sin( angle );
			float cos = MathEx.Cos( angle );
			result.Item0.X = 1;
			result.Item0.Y = 0;
			result.Item0.Z = 0;
			result.Item1.X = 0;
			result.Item1.Y = cos;
			result.Item1.Z = -sin;
			result.Item2.X = 0;
			result.Item2.Y = sin;
			result.Item2.Z = cos;
		}

		/// <summary>
		/// Creates a matrix that rotates around the y-axis.
		/// </summary>
		/// <param name="angle">The angle in radians.</param>
		/// <returns>The resulting rotation matrix.</returns>
		public static Matrix3F FromRotateByY( RadianF angle )
		{
			Matrix3F result;
			float sin = MathEx.Sin( angle );
			float cos = MathEx.Cos( angle );
			result.Item0.X = cos;
			result.Item0.Y = 0;
			result.Item0.Z = sin;
			result.Item1.X = 0;
			result.Item1.Y = 1;
			result.Item1.Z = 0;
			result.Item2.X = -sin;
			result.Item2.Y = 0;
			result.Item2.Z = cos;
			return result;
		}

		/// <summary>
		/// Creates a matrix that rotates around the y-axis.
		/// </summary>
		/// <param name="angle">The angle in radians.</param>
		/// <param name="result">When the method completes, contains the resulting rotation matrix.</param>
		public static void FromRotateByY( RadianF angle, out Matrix3F result )
		{
			float sin = MathEx.Sin( angle );
			float cos = MathEx.Cos( angle );
			result.Item0.X = cos;
			result.Item0.Y = 0;
			result.Item0.Z = sin;
			result.Item1.X = 0;
			result.Item1.Y = 1;
			result.Item1.Z = 0;
			result.Item2.X = -sin;
			result.Item2.Y = 0;
			result.Item2.Z = cos;
		}

		/// <summary>
		/// Creates a matrix that rotates around the z-axis.
		/// </summary>
		/// <param name="angle">The angle in radians.</param>
		/// <returns>The resulting rotation matrix.</returns>
		public static Matrix3F FromRotateByZ( RadianF angle )
		{
			Matrix3F result;
			float sin = MathEx.Sin( angle );
			float cos = MathEx.Cos( angle );
			result.Item0.X = cos;
			result.Item0.Y = -sin;
			result.Item0.Z = 0;
			result.Item1.X = sin;
			result.Item1.Y = cos;
			result.Item1.Z = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = 1;
			return result;
		}

		/// <summary>
		/// Creates a matrix that rotates around the z-axis.
		/// </summary>
		/// <param name="angle">The angle in radians.</param>
		/// <param name="result">When the method completes, contains the resulting rotation matrix.</param>
		public static void FromRotateByZ( RadianF angle, out Matrix3F result )
		{
			float sin = MathEx.Sin( angle );
			float cos = MathEx.Cos( angle );
			result.Item0.X = cos;
			result.Item0.Y = -sin;
			result.Item0.Z = 0;
			result.Item1.X = sin;
			result.Item1.Y = cos;
			result.Item1.Z = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = 1;
		}

		/// <summary>
		/// Decomposites the matrix by Gram-Schmidt orthogonalization algorithm (the QR algorithm).
		/// </summary>
		/// <param name="kQ">Orthogonal.</param>
		/// <param name="kD">Diagonal.</param>
		/// <param name="kU">Upper triangular.</param>
		public void QDUDecomposition( out Matrix3F kQ, out Vector3F kD, out Vector3F kU )
		{
			// Factor M = QR = QDU where Q is orthogonal, D is diagonal,
			// and U is upper triangular with ones on its diagonal.  Algorithm uses
			// Gram-Schmidt orthogonalization (the QR algorithm).
			//
			// If M = [ m0 | m1 | m2 ] and Q = [ q0 | q1 | q2 ], then
			//
			//   q0 = m0/|m0|
			//   q1 = (m1-(q0*m1)q0)/|m1-(q0*m1)q0|
			//   q2 = (m2-(q0*m2)q0-(q1*m2)q1)/|m2-(q0*m2)q0-(q1*m2)q1|
			//
			// where |V| indicates length of vector V and A*B indicates dot
			// product of vectors A and B.  The matrix R has entries
			//
			//   r00 = q0*m0  r01 = q0*m1  r02 = q0*m2
			//   r10 = 0      r11 = q1*m1  r12 = q1*m2
			//   r20 = 0      r21 = 0      r22 = q2*m2
			//
			// so D = diag(r00,r11,r22) and U has entries u01 = r01/r00,
			// u02 = r02/r00, and u12 = r12/r11.

			// Q = rotation
			// D = scaling
			// U = shear

			// D stores the three diagonal entries r00, r11, r22
			// U stores the entries U[0] = u01, U[1] = u02, U[2] = u12

			// build orthogonal matrix Q
			kQ = Matrix3F.Zero;

			float fInvLength = MathEx.InvSqrt( this[ 0, 0 ] * this[ 0, 0 ] + this[ 0, 1 ] * this[ 0, 1 ] +
				 this[ 0, 2 ] * this[ 0, 2 ] );
			kQ[ 0, 0 ] = this[ 0, 0 ] * fInvLength;
			kQ[ 0, 1 ] = this[ 0, 1 ] * fInvLength;
			kQ[ 0, 2 ] = this[ 0, 2 ] * fInvLength;

			float fDot = kQ[ 0, 0 ] * this[ 1, 0 ] + kQ[ 0, 1 ] * this[ 1, 1 ] + kQ[ 0, 2 ] * this[ 1, 2 ];
			kQ[ 1, 0 ] = this[ 1, 0 ] - fDot * kQ[ 0, 0 ];
			kQ[ 1, 1 ] = this[ 1, 1 ] - fDot * kQ[ 0, 1 ];
			kQ[ 1, 2 ] = this[ 1, 2 ] - fDot * kQ[ 0, 2 ];
			fInvLength = MathEx.InvSqrt(
				kQ[ 1, 0 ] * kQ[ 1, 0 ] + kQ[ 1, 1 ] * kQ[ 1, 1 ] + kQ[ 1, 2 ] * kQ[ 1, 2 ] );
			kQ[ 1, 0 ] *= fInvLength;
			kQ[ 1, 1 ] *= fInvLength;
			kQ[ 1, 2 ] *= fInvLength;

			fDot = kQ[ 0, 0 ] * this[ 2, 0 ] + kQ[ 0, 1 ] * this[ 2, 1 ] + kQ[ 0, 2 ] * this[ 2, 2 ];
			kQ[ 2, 0 ] = this[ 2, 0 ] - fDot * kQ[ 0, 0 ];
			kQ[ 2, 1 ] = this[ 2, 1 ] - fDot * kQ[ 0, 1 ];
			kQ[ 2, 2 ] = this[ 2, 2 ] - fDot * kQ[ 0, 2 ];
			fDot = kQ[ 1, 0 ] * this[ 2, 0 ] + kQ[ 1, 1 ] * this[ 2, 1 ] + kQ[ 1, 2 ] * this[ 2, 2 ];
			kQ[ 2, 0 ] -= fDot * kQ[ 1, 0 ];
			kQ[ 2, 1 ] -= fDot * kQ[ 1, 1 ];
			kQ[ 2, 2 ] -= fDot * kQ[ 1, 2 ];
			fInvLength = MathEx.InvSqrt( kQ[ 2, 0 ] * kQ[ 2, 0 ] + kQ[ 2, 1 ] * kQ[ 2, 1 ] +
				kQ[ 2, 2 ] * kQ[ 2, 2 ] );
			kQ[ 2, 0 ] *= fInvLength;
			kQ[ 2, 1 ] *= fInvLength;
			kQ[ 2, 2 ] *= fInvLength;

			// guarantee that orthogonal matrix has determinant 1 (no reflections)
			float fDet =
				kQ[ 0, 0 ] * kQ[ 1, 1 ] * kQ[ 2, 2 ] + kQ[ 1, 0 ] * kQ[ 2, 1 ] * kQ[ 0, 2 ] +
				kQ[ 2, 0 ] * kQ[ 0, 1 ] * kQ[ 1, 2 ] - kQ[ 2, 0 ] * kQ[ 1, 1 ] * kQ[ 0, 2 ] -
				kQ[ 1, 0 ] * kQ[ 0, 1 ] * kQ[ 2, 2 ] - kQ[ 0, 0 ] * kQ[ 2, 1 ] * kQ[ 1, 2 ];

			if( fDet < 0.0 )
			{
				for( int iRow = 0; iRow < 3; iRow++ )
					for( int iCol = 0; iCol < 3; iCol++ )
						kQ[ iRow, iCol ] = -kQ[ iRow, iCol ];
			}

			// build "right" matrix R
			Matrix3F kR = Matrix3F.Zero;
			kR[ 0, 0 ] = kQ[ 0, 0 ] * this[ 0, 0 ] + kQ[ 0, 1 ] * this[ 0, 1 ] + kQ[ 0, 2 ] * this[ 0, 2 ];
			kR[ 1, 0 ] = kQ[ 0, 0 ] * this[ 1, 0 ] + kQ[ 0, 1 ] * this[ 1, 1 ] + kQ[ 0, 2 ] * this[ 1, 2 ];
			kR[ 1, 1 ] = kQ[ 1, 0 ] * this[ 1, 0 ] + kQ[ 1, 1 ] * this[ 1, 1 ] + kQ[ 1, 2 ] * this[ 1, 2 ];
			kR[ 2, 0 ] = kQ[ 0, 0 ] * this[ 2, 0 ] + kQ[ 0, 1 ] * this[ 2, 1 ] + kQ[ 0, 2 ] * this[ 2, 2 ];
			kR[ 2, 1 ] = kQ[ 1, 0 ] * this[ 2, 0 ] + kQ[ 1, 1 ] * this[ 2, 1 ] + kQ[ 1, 2 ] * this[ 2, 2 ];
			kR[ 2, 2 ] = kQ[ 2, 0 ] * this[ 2, 0 ] + kQ[ 2, 1 ] * this[ 2, 1 ] + kQ[ 2, 2 ] * this[ 2, 2 ];

			// the scaling component
			kD = new Vector3F( kR[ 0, 0 ], kR[ 1, 1 ], kR[ 2, 2 ] );

			// the shear component
			float fInvD0 = 1.0f / kD[ 0 ];
			kU = new Vector3F( kR[ 1, 0 ] * fInvD0, kR[ 2, 0 ] * fInvD0, kR[ 2, 1 ] / kD[ 1 ] );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix3F"/> into the equivalent <see cref="Matrix3"/> structure.
		/// </summary>
		/// <param name="result">When the method completes, contains the equivalent <see cref="Matrix3"/> structure.</param>
		public void ToMatrix3( out Matrix3 result )
		{
			result.Item0.X = Item0.X;
			result.Item0.Y = Item0.Y;
			result.Item0.Z = Item0.Z;
			result.Item1.X = Item1.X;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = Item1.Z;
			result.Item2.X = Item2.X;
			result.Item2.Y = Item2.Y;
			result.Item2.Z = Item2.Z;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix3F"/> into the equivalent <see cref="Matrix3"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Matrix3"/> structure.</returns>
		[AutoConvertType]
		public Matrix3 ToMatrix3()
		{
			ToMatrix3( out var result );
			return result;
		}

#if !DISABLE_IMPLICIT
		/// <summary>
		/// Implicit conversion from <see cref="Matrix3F"/> type to <see cref="Matrix3"/> type for given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		public static implicit operator Matrix3( Matrix3F v )
		{
			return new Matrix3( v );
		}
#endif

		/// <summary>
		/// Decomposes a matrix into a rotation and scale.
		/// </summary>
		/// <param name="rotation">When the method completes, contains the rtoation component of the decomposed matrix.</param>
		/// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
		/// <remarks>
		/// This method is designed to decompose an SRT transformation matrix only.
		/// </remarks>
		public bool Decompose( out Matrix3F rotation, out Vector3F scale )
		{
			//Source: Unknown
			//References: http://www.gamedev.net/community/forums/topic.asp?topic_id=441695

			//Scaling is the length of the rows.
			scale = new Vector3F(
				(float)Math.Sqrt( ( Item0.X * Item0.X ) + ( Item0.Y * Item0.Y ) + ( Item0.Z * Item0.Z ) ),
				(float)Math.Sqrt( ( Item1.X * Item1.X ) + ( Item1.Y * Item1.Y ) + ( Item1.Z * Item1.Z ) ),
				(float)Math.Sqrt( ( Item2.X * Item2.X ) + ( Item2.Y * Item2.Y ) + ( Item2.Z * Item2.Z ) ) );

			//!!!!1e-6f?
			const double zeroTolerance = 1e-6f;
			//If any of the scaling factors are zero, than the rotation matrix can not exist.
			if( Math.Abs( scale.X ) < zeroTolerance ||
				Math.Abs( scale.Y ) < zeroTolerance ||
				Math.Abs( scale.Z ) < zeroTolerance )
			{
				rotation = Identity;
				return false;
			}

			//The rotation is the left over matrix after dividing out the scaling.
			rotation = new Matrix3F( Item0 / scale.X, Item1 / scale.Y, Item2 / scale.Z );
			return true;
		}

		/// <summary>
		/// Decomposes a matrix into a rotation and scale.
		/// </summary>
		/// <param name="rotation">When the method completes, contains the rtoation component of the decomposed matrix.</param>
		/// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
		/// <remarks>
		/// This method is designed to decompose an SRT transformation matrix only.
		/// </remarks>
		public bool Decompose( out QuaternionF rotation, out Vector3F scale )
		{
			if( !Decompose( out Matrix3F rotationMat3, out scale ) )
			{
				rotation = QuaternionF.Identity;
				return false;
			}
			rotationMat3.ToQuaternion( out rotation );
			return true;
		}

		/// <summary>
		/// Creates a look-at matrix.
		/// </summary>
		/// <param name="direction">Target position in world space.</param>
		/// <param name="up">Up vector in world space.</param>
		/// <returns>The instance of <see cref="Matrix3F"/> that transforms world space to camera space.</returns>
		public static Matrix3F LookAt( Vector3F direction, Vector3F up )
		{

			//!!!!not checked
			if( direction.Equals( Vector3F.Zero, MathEx.Epsilon ) )
				return Identity;

			Vector3F x = direction.GetNormalize();

			var cross = Vector3F.Cross( up, x );
			if( cross.Equals( Vector3F.Zero, MathEx.Epsilon ) )
				return Identity;
			var y = cross.GetNormalize();
			//Vec3 y = Vec3.Cross( up, x ).GetNormalize();

			Vector3F z = -Vector3F.Cross( y, x ).GetNormalize();
			return new Matrix3F( x, y, z );
		}
	}
}
