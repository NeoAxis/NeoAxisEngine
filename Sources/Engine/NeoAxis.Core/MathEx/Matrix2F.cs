// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A structure encapsulating a single precision 2x2 matrix.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Matrix2F
	{
		/// <summary>
		/// The first row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector2F Item0;
		/// <summary>
		/// The second row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector2F Item1;

		/// <summary>
		/// The matrix with all of its components set to zero.
		/// </summary>
		public static readonly Matrix2F Zero = new Matrix2F( 0, 0, 0, 0 );
		/// <summary>
		/// The identity matrix.
		/// </summary>
		public static readonly Matrix2F Identity = new Matrix2F( 1, 0, 0, 1 );

		/// <summary>
		/// Constructs a matrix with the given individual elements.
		/// </summary>
		/// <param name="xx">Value at row 1 column 1 of the matrix.</param>
		/// <param name="xy">Value at row 1 column 2 of the matrix.</param>
		/// <param name="yx">Value at row 2 column 1 of the matrix.</param>
		/// <param name="yy">Value at row 2 column 2 of the matrix.</param>
		public Matrix2F( float xx, float xy, float yx, float yy )
		{
			Item0 = new Vector2F( xx, xy );
			Item1 = new Vector2F( yx, yy );
		}

		/// <summary>
		/// Constructs a matrix with the specified <see cref="Vector2F"/> elements,
		/// which are the corresponding rows of the matrix.
		/// </summary>
		/// <param name="x">The vector which is the first row.</param>
		/// <param name="y">The vector which is the second row.</param>
		public Matrix2F( Vector2F x, Vector2F y )
		{
			Item0 = x;
			Item1 = y;
		}

		/// <summary>
		/// Constructs a matrix with another specified <see cref="Matrix2F"/> object.
		/// </summary>
		/// <param name="m">A specified matrix.</param>
		public Matrix2F( Matrix2F m )
		{
			Item0 = m.Item0;
			Item1 = m.Item1;
		}

		/// <summary>
		/// Gets or sets the row of the current instance of <see cref="Matrix2F"/> at the specified index.
		/// </summary>
		/// <value>The value of the corresponding row of <see cref="Vector2F"/> format, depending on the index.</value>
		/// <param name="index">The index of the row to access.</param>
		/// <returns>The value of the row of <see cref="Vector2F"/> format at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 1].</exception>
		public unsafe Vector2F this[ int index ]
		{
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector2F* v = &this.Item0 )
					return v[ index ];
			}
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector2F* v = &this.Item0 )
					v[ index ] = value;
			}
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return Item0.GetHashCode() ^ Item1.GetHashCode();
		}

		/// <summary>
		/// Adds two matricies.
		/// </summary>
		/// <param name="v1">The first matrix to add.</param>
		/// <param name="v2">The second matrix to add.</param>
		/// <returns>The sum of the two matricies.</returns>
		public static Matrix2F operator +( Matrix2F v1, Matrix2F v2 )
		{
			Matrix2F result;
			result.Item0.X = v1.Item0.X + v2.Item0.X;
			result.Item0.Y = v1.Item0.Y + v2.Item0.Y;
			result.Item1.X = v1.Item1.X + v2.Item1.X;
			result.Item1.Y = v1.Item1.Y + v2.Item1.Y;
			return result;
		}

		/// <summary>
		/// Subtracts two matricies.
		/// </summary>
		/// <param name="v1">The matrix to subtract from.</param>
		/// <param name="v2">The matrix to be subtracted from another matrix.</param>
		/// <returns>The difference between the two matricies.</returns>
		public static Matrix2F operator -( Matrix2F v1, Matrix2F v2 )
		{
			Matrix2F result;
			result.Item0.X = v1.Item0.X - v2.Item0.X;
			result.Item0.Y = v1.Item0.Y - v2.Item0.Y;
			result.Item1.X = v1.Item1.X - v2.Item1.X;
			result.Item1.Y = v1.Item1.Y - v2.Item1.Y;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="s">The value by which to multiply.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix2F operator *( Matrix2F m, float s )
		{
			Matrix2F result;
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="s">The value by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <returns>The scaled matrix.</returns>
		public static Matrix2F operator *( float s, Matrix2F m )
		{
			Matrix2F result;
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector2F"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector2F operator *( Matrix2F m, Vector2F v )
		{
			Vector2F result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector2F"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector2F operator *( Vector2F v, Matrix2F m )
		{
			Vector2F result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y;
			return result;
		}

		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="v1">The first matrix to multiply.</param>
		/// <param name="v2">The second matrix to multiply.</param>
		/// <returns>The product of the two matricies.</returns>
		public static Matrix2F operator *( Matrix2F v1, Matrix2F v2 )
		{
			Matrix2F result;
			result.Item0.X = v1.Item0.X * v2.Item0.X + v1.Item1.X * v2.Item0.Y;
			result.Item0.Y = v1.Item0.Y * v2.Item0.X + v1.Item1.Y * v2.Item0.Y;
			result.Item1.X = v1.Item0.X * v2.Item1.X + v1.Item1.X * v2.Item1.Y;
			result.Item1.Y = v1.Item0.Y * v2.Item1.X + v1.Item1.Y * v2.Item1.Y;
			return result;
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="v">The matrix to negate.</param>
		/// <returns>The negated matrix.</returns>
		public static Matrix2F operator -( Matrix2F v )
		{
			Matrix2F result;
			result.Item0.X = -v.Item0.X;
			result.Item0.Y = -v.Item0.Y;
			result.Item1.X = -v.Item1.X;
			result.Item1.Y = -v.Item1.Y;
			return result;
		}

		/// <summary>
		/// Adds two matricies.
		/// </summary>
		/// <param name="v1">The first matrix to add.</param>
		/// <param name="v2">The second matrix to add.</param>
		/// <param name="result">When the method completes, contains the sum of the two matricies.</param>
		public static void Add( ref Matrix2F v1, ref Matrix2F v2, out Matrix2F result )
		{
			result.Item0.X = v1.Item0.X + v2.Item0.X;
			result.Item0.Y = v1.Item0.Y + v2.Item0.Y;
			result.Item1.X = v1.Item1.X + v2.Item1.X;
			result.Item1.Y = v1.Item1.Y + v2.Item1.Y;
		}

		/// <summary>
		/// Subtracts two matricies.
		/// </summary>
		/// <param name="v1">The matrix to subtract from.</param>
		/// <param name="v2">The matrix to be subtracted from another matrix.</param>
		/// <param name="result">When the method completes, contains the difference of the two matricies.</param>
		public static void Subtract( ref Matrix2F v1, ref Matrix2F v2, out Matrix2F result )
		{
			result.Item0.X = v1.Item0.X - v2.Item0.X;
			result.Item0.Y = v1.Item0.Y - v2.Item0.Y;
			result.Item1.X = v1.Item1.X - v2.Item1.X;
			result.Item1.Y = v1.Item1.Y - v2.Item1.Y;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="s">The value by which to multiply.</param>
		/// <param name="result">When the method completes, contains the scaled matrix.</param>
		public static void Multiply( ref Matrix2F m, float s, out Matrix2F result )
		{
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="s">The value by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the scaled matrix.</param>
		public static void Multiply( float s, ref Matrix2F m, out Matrix2F result )
		{
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector2F"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		public static void Multiply( ref Vector2F v, ref Matrix2F m, out Vector2F result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector2F"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		public static void Multiply( ref Matrix2F m, ref Vector2F v, out Vector2F result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y;
		}

		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="v1">The first matrix to multiply.</param>
		/// <param name="v2">The second matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the product of the two matricies.</param>
		public static void Multiply( ref Matrix2F v1, ref Matrix2F v2, out Matrix2F result )
		{
			result.Item0.X = v1.Item0.X * v2.Item0.X + v1.Item1.X * v2.Item0.Y;
			result.Item0.Y = v1.Item0.Y * v2.Item0.X + v1.Item1.Y * v2.Item0.Y;
			result.Item1.X = v1.Item0.X * v2.Item1.X + v1.Item1.X * v2.Item1.Y;
			result.Item1.Y = v1.Item0.Y * v2.Item1.X + v1.Item1.Y * v2.Item1.Y;
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="m">The matrix to negate.</param>
		/// <param name="result">When the method completes, contains the negated matrix.</param>
		public static void Negate( ref Matrix2F m, out Matrix2F result )
		{
			result.Item0.X = -m.Item0.X;
			result.Item0.Y = -m.Item0.Y;
			result.Item1.X = -m.Item1.X;
			result.Item1.Y = -m.Item1.Y;
		}

		//public static Mat2F Add( Mat2F v1, Mat2F v2 )
		//{
		//	Mat2F result;
		//	Add( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat2F Subtract( Mat2F v1, Mat2F v2 )
		//{
		//	Mat2F result;
		//	Subtract( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat2F Multiply( Mat2F m, float s )
		//{
		//	Mat2F result;
		//	Multiply( ref m, s, out result );
		//	return result;
		//}

		//public static Mat2F Multiply( float s, Mat2F m )
		//{
		//	Mat2F result;
		//	Multiply( ref m, s, out result );
		//	return result;
		//}

		//public static Vec2F Multiply( Mat2F m, Vec2F v )
		//{
		//	Vec2F result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Vec2F Multiply( Vec2F v, Mat2F m )
		//{
		//	Vec2F result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Mat2F Multiply( Mat2F v1, Mat2F v2 )
		//{
		//	Mat2F result;
		//	Multiply( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat2F Negate( Mat2F m )
		//{
		//	Mat2F result;
		//	Negate( ref m, out result );
		//	return result;
		//}

		/// <summary>
		/// Determines whether the specified matrix is equal to the current instance of <see cref="Matrix2F"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The matrix to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified matrix is equal to the current instance of <see cref="Matrix2F"/>; False otherwise.</returns>
		public bool Equals( Matrix2F v, float epsilon )
		{
			if( !Item0.Equals( ref v.Item0, epsilon ) )
				return false;
			if( !Item1.Equals( ref v.Item1, epsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified matrix is equal to the current instance of <see cref="Matrix2F"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The matrix to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified matrix is equal to the current instance of <see cref="Matrix2F"/>; False otherwise.</returns>
		public bool Equals( ref Matrix2F v, float epsilon )
		{
			if( !Item0.Equals( ref v.Item0, epsilon ) )
				return false;
			if( !Item1.Equals( ref v.Item1, epsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Gets the trace of the matrix, the sum of the values along the diagonal.
		/// </summary>
		/// <returns>The trace of the matrix.</returns>
		public float GetTrace()
		{
			return Item0.X + Item1.Y;
		}

		/// <summary>
		/// Transposes the matrix.
		/// </summary>
		public void Transpose()
		{
			float v;
			v = Item0.Y; Item0.Y = Item1.X; Item1.X = v;
		}

		/// <summary>
		/// Returns the transpose of the current instance of <see cref="Matrix2F"/>.
		/// </summary>
		/// <returns>The transpose of the current instance of <see cref="Matrix2F"/>.</returns>
		public Matrix2F GetTranspose()
		{
			Matrix2F result;
			result.Item0.X = Item0.X;
			result.Item0.Y = Item1.X;
			result.Item1.X = Item0.Y;
			result.Item1.Y = Item1.Y;
			return result;
		}

		/// <summary>
		/// Calculates the transpose of the current instance of <see cref="Matrix2F"/>.
		/// </summary>
		/// <param name="result">When the method completes, contains the transpose of the current instance of <see cref="Matrix2F"/>.</param>
		public void GetTranspose( out Matrix2F result )
		{
			result.Item0.X = Item0.X;
			result.Item0.Y = Item1.X;
			result.Item1.X = Item0.Y;
			result.Item1.Y = Item1.Y;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix2F"/> and determines whether the matrix is invertible.
		/// Determines whether the current instance of <see cref="Matrix2F"/> is invertible and, if so, inverts this matrix.
		/// </summary>
		/// <returns>True if the current instance of <see cref="Matrix2F"/> is invertible; False otherwise.</returns>
		public bool Inverse()
		{
			double det = Item0.X * Item1.Y - Item0.Y * Item1.X;
			if( Math.Abs( det ) < 1e-14 )
				return false;
			double invDet = 1.0f / det;
			double a = Item0.X;
			Item0.X = (float)( Item1.Y * invDet );
			Item0.Y = (float)( -Item0.Y * invDet );
			Item1.X = (float)( -Item1.X * invDet );
			Item1.Y = (float)( a * invDet );
			return true;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix2F"/> if it is invertible and returns the result.
		/// </summary>
		/// <returns>If the current instance of <see cref="Matrix2F"/> is invertible, returns the inverted matrix;
		/// otherwise returns the original matrix.</returns>
		public Matrix2F GetInverse()
		{
			Matrix2F result = this;
			result.Inverse();
			return result;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix2F"/> if it is invertible.
		/// </summary>
		/// <param name="result">When the method completes, contains the inverted matrix if the current instance
		/// of <see cref="Matrix2F"/> is invertible; otherwise, contains the original matrix.</param>
		public void GetInverse( out Matrix2F result )
		{
			result = this;
			result.Inverse();
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix2F"/> into the equivalent <see cref="Matrix3F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Matrix3F"/> structure.</returns>
		[AutoConvertType]
		public Matrix3F ToMatrix3F()
		{
			Matrix3F result;
			result.Item0.X = Item0.X;
			result.Item0.Y = Item0.Y;
			result.Item0.Z = 0;
			result.Item1.X = Item1.X;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = 1;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix2F"/> into the equivalent <see cref="Matrix3F"/> structure.
		/// </summary>
		/// <param name="result">When the method completes, contains the equivalent <see cref="Matrix3F"/> structure.</param>
		public void ToMatrix3F( out Matrix3F result )
		{
			result.Item0.X = Item0.X;
			result.Item0.Y = Item0.Y;
			result.Item0.Z = 0;
			result.Item1.X = Item1.X;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = 1;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Matrix2F"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Matrix2F"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Matrix2F"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Matrix2F && this == (Matrix2F)obj );
		}

		/// <summary>
		/// Determines whether two given matricies are equal.
		/// </summary>
		/// <param name="v1">The first matrix to compare.</param>
		/// <param name="v2">The second matrix to compare.</param>
		/// <returns>True if the matricies are equal; False otherwise.</returns>
		public static bool operator ==( Matrix2F v1, Matrix2F v2 )
		{
			return v1.Item0 == v2.Item0 && v1.Item1 == v2.Item1;
		}

		/// <summary>
		/// Determines whether two given matricies are unequal.
		/// </summary>
		/// <param name="v1">The first matrix to compare.</param>
		/// <param name="v2">The second matrix to compare.</param>
		/// <returns>True if the matricies are unequal; False otherwise.</returns>
		public static bool operator !=( Matrix2F v1, Matrix2F v2 )
		{
			return v1.Item0 != v2.Item0 || v1.Item1 != v2.Item1;
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the matrix component, depending on the index.</value>
		/// <param name="row">The row of the matrix to access.</param>
		/// <param name="column">The column of the matrix to access.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>
		/// is out of the range [0, 1].</exception>
		public unsafe float this[ int row, int column ]
		{
			get
			{
				if( row < 0 || row > 1 )
					throw new ArgumentOutOfRangeException( "row" );
				if( column < 0 || column > 1 )
					throw new ArgumentOutOfRangeException( "column" );
				fixed ( float* v = &this.Item0.X )
					return v[ row * 2 + column ];
			}
			set
			{
				if( row < 0 || row > 1 )
					throw new ArgumentOutOfRangeException( "row" );
				if( column < 0 || column > 1 )
					throw new ArgumentOutOfRangeException( "column" );
				fixed ( float* v = &this.Item0.X )
					v[ row * 2 + column ] = value;
			}
		}

		/// <summary>
		/// Converts a string representation of a matrix into the equivalent <see cref="Matrix2F"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the matrix (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Matrix2F"/> structure.</returns>
		[AutoConvertType]
		public static Matrix2F Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces.", text ) );

			try
			{
				return new Matrix2F(
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
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Matrix2F"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Matrix2F"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1}", Item0.ToString(), Item1.ToString() );
		}

		/// <summary>
		/// Creates a scale matrix with the specified <see cref="Vector2F"/> object.
		/// </summary>
		/// <param name="scale">The specified vector.</param>
		/// <returns>The scale matrix.</returns>
		public static Matrix2F FromScale( Vector2F scale )
		{
			return new Matrix2F(
				scale.X, 0,
				0, scale.Y );
		}

		/// <summary>
		/// Creates a scale matrix with the specified <see cref="Vector2F"/> object.
		/// </summary>
		/// <param name="scale">The specified vector.</param>
		/// <param name="result">When the method completes, contains the scale matrix.</param>
		public static void FromScale( ref Vector2F scale, out Matrix2F result )
		{
			result.Item0.X = scale.X;
			result.Item0.Y = 0;
			result.Item1.X = 0;
			result.Item1.Y = scale.Y;
		}

		/// <summary>
		/// Creates a rotation matrix.
		/// </summary>
		/// <param name="angle">Angle in radians to rotate counter-clockwise.</param>
		/// <returns>The resulting rotation matrix.</returns>
		public static Matrix2F FromRotate( RadianF angle )
		{
			float sin = MathEx.Sin( angle );
			float cos = MathEx.Cos( angle );
			Matrix2F result;
			result.Item0.X = cos;
			result.Item0.Y = -sin;
			result.Item1.X = sin;
			result.Item1.Y = cos;
			return result;
		}

		/// <summary>
		/// Creates a rotation matrix.
		/// </summary>
		/// <param name="angle">Angle in radians to rotate counter-clockwise.</param>
		/// <param name="result">When the method completes, contains the resulting rotation matrix.</param>
		public static void FromRotate( RadianF angle, out Matrix2F result )
		{
			float sin = MathEx.Sin( angle );
			float cos = MathEx.Cos( angle );
			result.Item0.X = cos;
			result.Item0.Y = -sin;
			result.Item1.X = sin;
			result.Item1.Y = cos;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix2F"/> into the equivalent <see cref="Matrix2"/> structure.
		/// </summary>
		/// <param name="result">When the method completes, contains the equivalent <see cref="Matrix2"/> structure.</param>
		public void ToMatrix2( out Matrix2 result )
		{
			result.Item0.X = Item0.X;
			result.Item0.Y = Item0.Y;
			result.Item1.X = Item1.X;
			result.Item1.Y = Item1.Y;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix2F"/> into the equivalent <see cref="Matrix2"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Matrix2"/> structure.</returns>
		[AutoConvertType]
		public Matrix2 ToMatrix2()
		{
			ToMatrix2( out var result );
			return result;
		}

#if !DISABLE_IMPLICIT
		/// <summary>
		/// Implicit conversion from <see cref="Matrix2F"/> type to <see cref="Matrix2"/> type for given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		public static implicit operator Matrix2( Matrix2F v )
		{
			return new Matrix2( v );
		}
#endif
	}
}
