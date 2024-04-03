// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// A structure encapsulating a single precision 4x4 matrix.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Matrix4F
	{
		/// <summary>
		/// The matrix with all of its components set to zero.
		/// </summary>
		public static readonly Matrix4F Zero = new Matrix4F( 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
		/// <summary>
		/// The identity matrix.
		/// </summary>
		public static readonly Matrix4F Identity = new Matrix4F( 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 );

		//
		/// <summary>
		/// The first row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector4F Item0;
		/// <summary>
		/// The second row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector4F Item1;
		/// <summary>
		/// The third row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector4F Item2;
		/// <summary>
		/// The fourth row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector4F Item3;

		//

		/// <summary>
		/// Constructs a matrix with the given individual elements.
		/// </summary>
		/// <param name="xx">Value at row 1 column 1 of the matrix.</param>
		/// <param name="xy">Value at row 1 column 2 of the matrix.</param>
		/// <param name="xz">Value at row 1 column 3 of the matrix.</param>
		/// <param name="xw">Value at row 1 column 4 of the matrix.</param>
		/// <param name="yx">Value at row 2 column 1 of the matrix.</param>
		/// <param name="yy">Value at row 2 column 2 of the matrix.</param>
		/// <param name="yz">Value at row 2 column 3 of the matrix.</param>
		/// <param name="yw">Value at row 2 column 4 of the matrix.</param>
		/// <param name="zx">Value at row 3 column 1 of the matrix.</param>
		/// <param name="zy">Value at row 3 column 2 of the matrix.</param>
		/// <param name="zz">Value at row 3 column 3 of the matrix.</param>
		/// <param name="zw">Value at row 3 column 4 of the matrix.</param>
		/// <param name="wx">Value at row 4 column 1 of the matrix.</param>
		/// <param name="wy">Value at row 4 column 2 of the matrix.</param>
		/// <param name="wz">Value at row 4 column 3 of the matrix.</param>
		/// <param name="ww">Value at row 4 column 4 of the matrix.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4F(
			float xx, float xy, float xz, float xw,
			float yx, float yy, float yz, float yw,
			float zx, float zy, float zz, float zw,
			float wx, float wy, float wz, float ww )
		{
			Item0 = new Vector4F( xx, xy, xz, xw );
			Item1 = new Vector4F( yx, yy, yz, yw );
			Item2 = new Vector4F( zx, zy, zz, zw );
			Item3 = new Vector4F( wx, wy, wz, ww );
		}

		/// <summary>
		/// Constructs a matrix with the specified <see cref="Vector4F"/> elements,
		/// which are the corresponding rows of the matrix.
		/// </summary>
		/// <param name="x">The vector which is the first row.</param>
		/// <param name="y">The vector which is the second row.</param>
		/// <param name="z">The vector which is the third row.</param>
		/// <param name="w">The vector which is the fourth row.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4F( Vector4F x, Vector4F y, Vector4F z, Vector4F w )
		{
			Item0 = x;
			Item1 = y;
			Item2 = z;
			Item3 = w;
		}

		/// <summary>
		/// Constructs a matrix with another specified <see cref="Matrix4F"/> object.
		/// </summary>
		/// <param name="source">A specified matrix.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4F( Matrix4F source )
		{
			Item0 = source.Item0;
			Item1 = source.Item1;
			Item2 = source.Item2;
			Item3 = source.Item3;
		}

		/// <summary>
		/// Constructs a matrix with the given rotation and translation components.
		/// </summary>
		/// <param name="rotation">The rotation <see cref="Matrix3F"/>.</param>
		/// <param name="translation">The translation <see cref="Vector3F"/>.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4F( Matrix3F rotation, Vector3F translation )
		{
			Item0.X = rotation.Item0.X;
			Item0.Y = rotation.Item0.Y;
			Item0.Z = rotation.Item0.Z;
			Item0.W = 0.0f;
			Item1.X = rotation.Item1.X;
			Item1.Y = rotation.Item1.Y;
			Item1.Z = rotation.Item1.Z;
			Item1.W = 0.0f;
			Item2.X = rotation.Item2.X;
			Item2.Y = rotation.Item2.Y;
			Item2.Z = rotation.Item2.Z;
			Item2.W = 0.0f;
			Item3.X = translation.X;
			Item3.Y = translation.Y;
			Item3.Z = translation.Z;
			Item3.W = 1.0f;
		}

		/// <summary>
		/// Constructs a matrix with the given rotation and translation components.
		/// </summary>
		/// <param name="rotation">The rotation <see cref="Matrix3F"/>.</param>
		/// <param name="translation">The translation <see cref="Vector3F"/>.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4F( ref Matrix3F rotation, ref Vector3F translation )
		{
			Item0.X = rotation.Item0.X;
			Item0.Y = rotation.Item0.Y;
			Item0.Z = rotation.Item0.Z;
			Item0.W = 0.0f;
			Item1.X = rotation.Item1.X;
			Item1.Y = rotation.Item1.Y;
			Item1.Z = rotation.Item1.Z;
			Item1.W = 0.0f;
			Item2.X = rotation.Item2.X;
			Item2.Y = rotation.Item2.Y;
			Item2.Z = rotation.Item2.Z;
			Item2.W = 0.0f;
			Item3.X = translation.X;
			Item3.Y = translation.Y;
			Item3.Z = translation.Z;
			Item3.W = 1.0f;
		}

		/// <summary>
		/// Constructs a matrix with the given rotation and translation components and returns the resulting <see cref="Matrix4F"/>.
		/// </summary>
		/// <param name="rotation">The rotation <see cref="Matrix3F"/>.</param>
		/// <param name="translation">The translation <see cref="Vector3F"/>.</param>
		/// <param name="result">When the method completes, contains the resulting <see cref="Matrix4F"/>.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Construct( ref Matrix3F rotation, ref Vector3F translation, out Matrix4F result )
		{
			result.Item0.X = rotation.Item0.X;
			result.Item0.Y = rotation.Item0.Y;
			result.Item0.Z = rotation.Item0.Z;
			result.Item0.W = 0.0f;
			result.Item1.X = rotation.Item1.X;
			result.Item1.Y = rotation.Item1.Y;
			result.Item1.Z = rotation.Item1.Z;
			result.Item1.W = 0.0f;
			result.Item2.X = rotation.Item2.X;
			result.Item2.Y = rotation.Item2.Y;
			result.Item2.Z = rotation.Item2.Z;
			result.Item2.W = 0.0f;
			result.Item3.X = translation.X;
			result.Item3.Y = translation.Y;
			result.Item3.Z = translation.Z;
			result.Item3.W = 1.0f;
		}

		/// <summary>
		/// Constructs a matrix with specified one-dimensional <see cref="float"/> array with sixteen elements.
		/// </summary>
		/// <param name="array">One-dimensional <see cref="float"/> array with sixteen elements.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4F( float[] array )
		{
			Item0 = new Vector4F( array[ 0 ], array[ 1 ], array[ 2 ], array[ 3 ] );
			Item1 = new Vector4F( array[ 4 ], array[ 5 ], array[ 6 ], array[ 7 ] );
			Item2 = new Vector4F( array[ 8 ], array[ 9 ], array[ 10 ], array[ 11 ] );
			Item3 = new Vector4F( array[ 12 ], array[ 13 ], array[ 14 ], array[ 15 ] );
		}

		/// <summary>
		/// Gets or sets the row of the current instance of <see cref="Matrix4F"/> at the specified index.
		/// </summary>
		/// <value>The value of the corresponding row of <see cref="Vector4F"/> format, depending on the index.</value>
		/// <param name="index">The index of the row to access.</param>
		/// <returns>The value of the row of <see cref="Vector4F"/> format at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
		public unsafe Vector4F this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( Vector4F* v = &this.Item0 )
					return v[ index ];
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( Vector4F* v = &this.Item0 )
					v[ index ] = value;
			}
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return Item0.GetHashCode() ^ Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode();
		}

		/// <summary>
		/// Adds two matricies.
		/// </summary>
		/// <param name="v1">The first matrix to add.</param>
		/// <param name="v2">The second matrix to add.</param>
		/// <returns>The sum of the two matricies.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Matrix4F operator +( Matrix4F v1, Matrix4F v2 )
		{
			Matrix4F result;
			result.Item0.X = v1.Item0.X + v2.Item0.X;
			result.Item0.Y = v1.Item0.Y + v2.Item0.Y;
			result.Item0.Z = v1.Item0.Z + v2.Item0.Z;
			result.Item0.W = v1.Item0.W + v2.Item0.W;
			result.Item1.X = v1.Item1.X + v2.Item1.X;
			result.Item1.Y = v1.Item1.Y + v2.Item1.Y;
			result.Item1.Z = v1.Item1.Z + v2.Item1.Z;
			result.Item1.W = v1.Item1.W + v2.Item1.W;
			result.Item2.X = v1.Item2.X + v2.Item2.X;
			result.Item2.Y = v1.Item2.Y + v2.Item2.Y;
			result.Item2.Z = v1.Item2.Z + v2.Item2.Z;
			result.Item2.W = v1.Item2.W + v2.Item2.W;
			result.Item3.X = v1.Item3.X + v2.Item3.X;
			result.Item3.Y = v1.Item3.Y + v2.Item3.Y;
			result.Item3.Z = v1.Item3.Z + v2.Item3.Z;
			result.Item3.W = v1.Item3.W + v2.Item3.W;
			return result;
		}

		/// <summary>
		/// Subtracts two matricies.
		/// </summary>
		/// <param name="v1">The matrix to subtract from.</param>
		/// <param name="v2">The matrix to be subtracted from another matrix.</param>
		/// <returns>The difference between the two matricies.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Matrix4F operator -( Matrix4F v1, Matrix4F v2 )
		{
			Matrix4F result;
			result.Item0.X = v1.Item0.X - v2.Item0.X;
			result.Item0.Y = v1.Item0.Y - v2.Item0.Y;
			result.Item0.Z = v1.Item0.Z - v2.Item0.Z;
			result.Item0.W = v1.Item0.W - v2.Item0.W;
			result.Item1.X = v1.Item1.X - v2.Item1.X;
			result.Item1.Y = v1.Item1.Y - v2.Item1.Y;
			result.Item1.Z = v1.Item1.Z - v2.Item1.Z;
			result.Item1.W = v1.Item1.W - v2.Item1.W;
			result.Item2.X = v1.Item2.X - v2.Item2.X;
			result.Item2.Y = v1.Item2.Y - v2.Item2.Y;
			result.Item2.Z = v1.Item2.Z - v2.Item2.Z;
			result.Item2.W = v1.Item2.W - v2.Item2.W;
			result.Item3.X = v1.Item3.X - v2.Item3.X;
			result.Item3.Y = v1.Item3.Y - v2.Item3.Y;
			result.Item3.Z = v1.Item3.Z - v2.Item3.Z;
			result.Item3.W = v1.Item3.W - v2.Item3.W;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="s">The value by which to multiply.</param>
		/// <returns>The scaled matrix.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Matrix4F operator *( Matrix4F m, float s )
		{
			Matrix4F result;
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item0.Z = m.Item0.Z * s;
			result.Item0.W = m.Item0.W * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			result.Item1.Z = m.Item1.Z * s;
			result.Item1.W = m.Item1.W * s;
			result.Item2.X = m.Item2.X * s;
			result.Item2.Y = m.Item2.Y * s;
			result.Item2.Z = m.Item2.Z * s;
			result.Item2.W = m.Item2.W * s;
			result.Item3.X = m.Item3.X * s;
			result.Item3.Y = m.Item3.Y * s;
			result.Item3.Z = m.Item3.Z * s;
			result.Item3.W = m.Item3.W * s;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="s">The value by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <returns>The scaled matrix.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Matrix4F operator *( float s, Matrix4F m )
		{
			Matrix4F result;
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item0.Z = m.Item0.Z * s;
			result.Item0.W = m.Item0.W * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			result.Item1.Z = m.Item1.Z * s;
			result.Item1.W = m.Item1.W * s;
			result.Item2.X = m.Item2.X * s;
			result.Item2.Y = m.Item2.Y * s;
			result.Item2.Z = m.Item2.Z * s;
			result.Item2.W = m.Item2.W * s;
			result.Item3.X = m.Item3.X * s;
			result.Item3.Y = m.Item3.Y * s;
			result.Item3.Z = m.Item3.Z * s;
			result.Item3.W = m.Item3.W * s;
			return result;
		}

		/// <summary>
		/// Translates the ray coordinates to the space defined by a matrix.
		/// </summary>
		/// <param name="m">The given matrix.</param>
		/// <param name="r">The given ray.</param>
		/// <returns>The translated ray.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RayF operator *( Matrix4F m, RayF r )
		{
			RayF result;
			Matrix4F.Multiply( ref r.Origin, ref m, out result.Origin );
			Vector3F sourceTo;
			Vector3F.Add( ref r.Origin, ref r.Direction, out sourceTo );
			Vector3F to;
			Matrix4F.Multiply( ref sourceTo, ref m, out to );
			Vector3F.Subtract( ref to, ref result.Origin, out result.Direction );
			return result;
			//Vec3 origin = r.Origin * m;
			//Vec3 to = ( r.Origin + r.Direction ) * m;
			//return new Ray( origin, to - origin );
		}

		/// <summary>
		/// Translates the ray coordinates to the space defined by a matrix.
		/// </summary>
		/// <param name="r">The given ray.</param>
		/// <param name="m">The given matrix.</param>
		/// <returns>The translated ray.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RayF operator *( RayF r, Matrix4F m )
		{
			RayF result;
			Matrix4F.Multiply( ref r.Origin, ref m, out result.Origin );
			Vector3F sourceTo;
			Vector3F.Add( ref r.Origin, ref r.Direction, out sourceTo );
			Vector3F to;
			Matrix4F.Multiply( ref sourceTo, ref m, out to );
			Vector3F.Subtract( ref to, ref result.Origin, out result.Direction );
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3F"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector3F operator *( Matrix4F m, Vector3F v )
		{
			Vector3F result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3F"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector3F operator *( Vector3F v, Matrix4F m )
		{
			Vector3F result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector4F"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector4F operator *( Matrix4F m, Vector4F v )
		{
			Vector4F result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X * v.W;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y * v.W;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z * v.W;
			result.W = m.Item0.W * v.X + m.Item1.W * v.Y + m.Item2.W * v.Z + m.Item3.W * v.W;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector4F"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector4F operator *( Vector4F v, Matrix4F m )
		{
			Vector4F result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X * v.W;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y * v.W;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z * v.W;
			result.W = m.Item0.W * v.X + m.Item1.W * v.Y + m.Item2.W * v.Z + m.Item3.W * v.W;
			return result;
		}

		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="v1">The first matrix to multiply.</param>
		/// <param name="v2">The second matrix to multiply.</param>
		/// <returns>The product of the two matricies.</returns>
		[MethodImpl( (MethodImplOptions)512 )]
		public static Matrix4F operator *( Matrix4F v1, Matrix4F v2 )
		{
			Matrix4F result;
			result.Item0.X = v1.Item0.X * v2.Item0.X + v1.Item1.X * v2.Item0.Y + v1.Item2.X * v2.Item0.Z + v1.Item3.X * v2.Item0.W;
			result.Item0.Y = v1.Item0.Y * v2.Item0.X + v1.Item1.Y * v2.Item0.Y + v1.Item2.Y * v2.Item0.Z + v1.Item3.Y * v2.Item0.W;
			result.Item0.Z = v1.Item0.Z * v2.Item0.X + v1.Item1.Z * v2.Item0.Y + v1.Item2.Z * v2.Item0.Z + v1.Item3.Z * v2.Item0.W;
			result.Item0.W = v1.Item0.W * v2.Item0.X + v1.Item1.W * v2.Item0.Y + v1.Item2.W * v2.Item0.Z + v1.Item3.W * v2.Item0.W;
			result.Item1.X = v1.Item0.X * v2.Item1.X + v1.Item1.X * v2.Item1.Y + v1.Item2.X * v2.Item1.Z + v1.Item3.X * v2.Item1.W;
			result.Item1.Y = v1.Item0.Y * v2.Item1.X + v1.Item1.Y * v2.Item1.Y + v1.Item2.Y * v2.Item1.Z + v1.Item3.Y * v2.Item1.W;
			result.Item1.Z = v1.Item0.Z * v2.Item1.X + v1.Item1.Z * v2.Item1.Y + v1.Item2.Z * v2.Item1.Z + v1.Item3.Z * v2.Item1.W;
			result.Item1.W = v1.Item0.W * v2.Item1.X + v1.Item1.W * v2.Item1.Y + v1.Item2.W * v2.Item1.Z + v1.Item3.W * v2.Item1.W;
			result.Item2.X = v1.Item0.X * v2.Item2.X + v1.Item1.X * v2.Item2.Y + v1.Item2.X * v2.Item2.Z + v1.Item3.X * v2.Item2.W;
			result.Item2.Y = v1.Item0.Y * v2.Item2.X + v1.Item1.Y * v2.Item2.Y + v1.Item2.Y * v2.Item2.Z + v1.Item3.Y * v2.Item2.W;
			result.Item2.Z = v1.Item0.Z * v2.Item2.X + v1.Item1.Z * v2.Item2.Y + v1.Item2.Z * v2.Item2.Z + v1.Item3.Z * v2.Item2.W;
			result.Item2.W = v1.Item0.W * v2.Item2.X + v1.Item1.W * v2.Item2.Y + v1.Item2.W * v2.Item2.Z + v1.Item3.W * v2.Item2.W;
			result.Item3.X = v1.Item0.X * v2.Item3.X + v1.Item1.X * v2.Item3.Y + v1.Item2.X * v2.Item3.Z + v1.Item3.X * v2.Item3.W;
			result.Item3.Y = v1.Item0.Y * v2.Item3.X + v1.Item1.Y * v2.Item3.Y + v1.Item2.Y * v2.Item3.Z + v1.Item3.Y * v2.Item3.W;
			result.Item3.Z = v1.Item0.Z * v2.Item3.X + v1.Item1.Z * v2.Item3.Y + v1.Item2.Z * v2.Item3.Z + v1.Item3.Z * v2.Item3.W;
			result.Item3.W = v1.Item0.W * v2.Item3.X + v1.Item1.W * v2.Item3.Y + v1.Item2.W * v2.Item3.Z + v1.Item3.W * v2.Item3.W;
			return result;
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="v">The matrix to negate.</param>
		/// <returns>The negated matrix.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Matrix4F operator -( Matrix4F v )
		{
			Matrix4F result;
			result.Item0.X = -v.Item0.X;
			result.Item0.Y = -v.Item0.Y;
			result.Item0.Z = -v.Item0.Z;
			result.Item0.W = -v.Item0.W;
			result.Item1.X = -v.Item1.X;
			result.Item1.Y = -v.Item1.Y;
			result.Item1.Z = -v.Item1.Z;
			result.Item1.W = -v.Item1.W;
			result.Item2.X = -v.Item2.X;
			result.Item2.Y = -v.Item2.Y;
			result.Item2.Z = -v.Item2.Z;
			result.Item2.W = -v.Item2.W;
			result.Item3.X = -v.Item3.X;
			result.Item3.Y = -v.Item3.Y;
			result.Item3.Z = -v.Item3.Z;
			result.Item3.W = -v.Item3.W;
			return result;
		}

		/// <summary>
		/// Adds two matricies.
		/// </summary>
		/// <param name="v1">The first matrix to add.</param>
		/// <param name="v2">The second matrix to add.</param>
		/// <param name="result">When the method completes, contains the sum of the two matricies.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref Matrix4F v1, ref Matrix4F v2, out Matrix4F result )
		{
			result.Item0.X = v1.Item0.X + v2.Item0.X;
			result.Item0.Y = v1.Item0.Y + v2.Item0.Y;
			result.Item0.Z = v1.Item0.Z + v2.Item0.Z;
			result.Item0.W = v1.Item0.W + v2.Item0.W;
			result.Item1.X = v1.Item1.X + v2.Item1.X;
			result.Item1.Y = v1.Item1.Y + v2.Item1.Y;
			result.Item1.Z = v1.Item1.Z + v2.Item1.Z;
			result.Item1.W = v1.Item1.W + v2.Item1.W;
			result.Item2.X = v1.Item2.X + v2.Item2.X;
			result.Item2.Y = v1.Item2.Y + v2.Item2.Y;
			result.Item2.Z = v1.Item2.Z + v2.Item2.Z;
			result.Item2.W = v1.Item2.W + v2.Item2.W;
			result.Item3.X = v1.Item3.X + v2.Item3.X;
			result.Item3.Y = v1.Item3.Y + v2.Item3.Y;
			result.Item3.Z = v1.Item3.Z + v2.Item3.Z;
			result.Item3.W = v1.Item3.W + v2.Item3.W;
		}

		/// <summary>
		/// Subtracts two matricies.
		/// </summary>
		/// <param name="v1">The matrix to subtract from.</param>
		/// <param name="v2">The matrix to be subtracted from another matrix.</param>
		/// <param name="result">When the method completes, contains the difference of the two matricies.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref Matrix4F v1, ref Matrix4F v2, out Matrix4F result )
		{
			result.Item0.X = v1.Item0.X - v2.Item0.X;
			result.Item0.Y = v1.Item0.Y - v2.Item0.Y;
			result.Item0.Z = v1.Item0.Z - v2.Item0.Z;
			result.Item0.W = v1.Item0.W - v2.Item0.W;
			result.Item1.X = v1.Item1.X - v2.Item1.X;
			result.Item1.Y = v1.Item1.Y - v2.Item1.Y;
			result.Item1.Z = v1.Item1.Z - v2.Item1.Z;
			result.Item1.W = v1.Item1.W - v2.Item1.W;
			result.Item2.X = v1.Item2.X - v2.Item2.X;
			result.Item2.Y = v1.Item2.Y - v2.Item2.Y;
			result.Item2.Z = v1.Item2.Z - v2.Item2.Z;
			result.Item2.W = v1.Item2.W - v2.Item2.W;
			result.Item3.X = v1.Item3.X - v2.Item3.X;
			result.Item3.Y = v1.Item3.Y - v2.Item3.Y;
			result.Item3.Z = v1.Item3.Z - v2.Item3.Z;
			result.Item3.W = v1.Item3.W - v2.Item3.W;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="s">The value by which to multiply.</param>
		/// <param name="result">When the method completes, contains the scaled matrix.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Matrix4F m, float s, out Matrix4F result )
		{
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item0.Z = m.Item0.Z * s;
			result.Item0.W = m.Item0.W * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			result.Item1.Z = m.Item1.Z * s;
			result.Item1.W = m.Item1.W * s;
			result.Item2.X = m.Item2.X * s;
			result.Item2.Y = m.Item2.Y * s;
			result.Item2.Z = m.Item2.Z * s;
			result.Item2.W = m.Item2.W * s;
			result.Item3.X = m.Item3.X * s;
			result.Item3.Y = m.Item3.Y * s;
			result.Item3.Z = m.Item3.Z * s;
			result.Item3.W = m.Item3.W * s;
		}

		/// <summary>
		/// Multiplies a matrix by a given value.
		/// </summary>
		/// <param name="s">The value by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the scaled matrix.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( float s, ref Matrix4F m, out Matrix4F result )
		{
			result.Item0.X = m.Item0.X * s;
			result.Item0.Y = m.Item0.Y * s;
			result.Item0.Z = m.Item0.Z * s;
			result.Item0.W = m.Item0.W * s;
			result.Item1.X = m.Item1.X * s;
			result.Item1.Y = m.Item1.Y * s;
			result.Item1.Z = m.Item1.Z * s;
			result.Item1.W = m.Item1.W * s;
			result.Item2.X = m.Item2.X * s;
			result.Item2.Y = m.Item2.Y * s;
			result.Item2.Z = m.Item2.Z * s;
			result.Item2.W = m.Item2.W * s;
			result.Item3.X = m.Item3.X * s;
			result.Item3.Y = m.Item3.Y * s;
			result.Item3.Z = m.Item3.Z * s;
			result.Item3.W = m.Item3.W * s;
		}

		/// <summary>
		/// Translates the ray coordinates to the space defined by a matrix.
		/// </summary>
		/// <param name="m">The given matrix.</param>
		/// <param name="r">The given ray.</param>
		/// <param name="result">When the method completes, contains the translated ray.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Matrix4F m, ref RayF r, out RayF result )
		{
			Matrix4F.Multiply( ref r.Origin, ref m, out result.Origin );
			Vector3F sourceTo;
			Vector3F.Add( ref r.Origin, ref r.Direction, out sourceTo );
			Vector3F to;
			Matrix4F.Multiply( ref sourceTo, ref m, out to );
			Vector3F.Subtract( ref to, ref result.Origin, out result.Direction );
		}

		/// <summary>
		/// Translates the ray coordinates to the space defined by a matrix.
		/// </summary>
		/// <param name="r">The given ray.</param>
		/// <param name="m">The given matrix.</param>
		/// <param name="result">When the method completes, contains the translated ray.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref RayF r, ref Matrix4F m, out RayF result )
		{
			Matrix4F.Multiply( ref r.Origin, ref m, out result.Origin );
			Vector3F sourceTo;
			Vector3F.Add( ref r.Origin, ref r.Direction, out sourceTo );
			Vector3F to;
			Matrix4F.Multiply( ref sourceTo, ref m, out to );
			Vector3F.Subtract( ref to, ref result.Origin, out result.Direction );
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3F"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Matrix4F m, ref Vector3F v, out Vector3F result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3F"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Vector3F v, ref Matrix4F m, out Vector3F result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector4F"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Matrix4F m, ref Vector4F v, out Vector4F result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X * v.W;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y * v.W;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z * v.W;
			result.W = m.Item0.W * v.X + m.Item1.W * v.Y + m.Item2.W * v.Z + m.Item3.W * v.W;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector4F"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Vector4F v, ref Matrix4F m, out Vector4F result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X * v.W;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y * v.W;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z * v.W;
			result.W = m.Item0.W * v.X + m.Item1.W * v.Y + m.Item2.W * v.Z + m.Item3.W * v.W;
		}

		/// <summary>
		/// Multiplies two matrices.
		/// </summary>
		/// <param name="v1">The first matrix to multiply.</param>
		/// <param name="v2">The second matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the product of the two matricies.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Matrix4F v1, ref Matrix4F v2, out Matrix4F result )
		{
			result.Item0.X = v1.Item0.X * v2.Item0.X + v1.Item1.X * v2.Item0.Y + v1.Item2.X * v2.Item0.Z + v1.Item3.X * v2.Item0.W;
			result.Item0.Y = v1.Item0.Y * v2.Item0.X + v1.Item1.Y * v2.Item0.Y + v1.Item2.Y * v2.Item0.Z + v1.Item3.Y * v2.Item0.W;
			result.Item0.Z = v1.Item0.Z * v2.Item0.X + v1.Item1.Z * v2.Item0.Y + v1.Item2.Z * v2.Item0.Z + v1.Item3.Z * v2.Item0.W;
			result.Item0.W = v1.Item0.W * v2.Item0.X + v1.Item1.W * v2.Item0.Y + v1.Item2.W * v2.Item0.Z + v1.Item3.W * v2.Item0.W;
			result.Item1.X = v1.Item0.X * v2.Item1.X + v1.Item1.X * v2.Item1.Y + v1.Item2.X * v2.Item1.Z + v1.Item3.X * v2.Item1.W;
			result.Item1.Y = v1.Item0.Y * v2.Item1.X + v1.Item1.Y * v2.Item1.Y + v1.Item2.Y * v2.Item1.Z + v1.Item3.Y * v2.Item1.W;
			result.Item1.Z = v1.Item0.Z * v2.Item1.X + v1.Item1.Z * v2.Item1.Y + v1.Item2.Z * v2.Item1.Z + v1.Item3.Z * v2.Item1.W;
			result.Item1.W = v1.Item0.W * v2.Item1.X + v1.Item1.W * v2.Item1.Y + v1.Item2.W * v2.Item1.Z + v1.Item3.W * v2.Item1.W;
			result.Item2.X = v1.Item0.X * v2.Item2.X + v1.Item1.X * v2.Item2.Y + v1.Item2.X * v2.Item2.Z + v1.Item3.X * v2.Item2.W;
			result.Item2.Y = v1.Item0.Y * v2.Item2.X + v1.Item1.Y * v2.Item2.Y + v1.Item2.Y * v2.Item2.Z + v1.Item3.Y * v2.Item2.W;
			result.Item2.Z = v1.Item0.Z * v2.Item2.X + v1.Item1.Z * v2.Item2.Y + v1.Item2.Z * v2.Item2.Z + v1.Item3.Z * v2.Item2.W;
			result.Item2.W = v1.Item0.W * v2.Item2.X + v1.Item1.W * v2.Item2.Y + v1.Item2.W * v2.Item2.Z + v1.Item3.W * v2.Item2.W;
			result.Item3.X = v1.Item0.X * v2.Item3.X + v1.Item1.X * v2.Item3.Y + v1.Item2.X * v2.Item3.Z + v1.Item3.X * v2.Item3.W;
			result.Item3.Y = v1.Item0.Y * v2.Item3.X + v1.Item1.Y * v2.Item3.Y + v1.Item2.Y * v2.Item3.Z + v1.Item3.Y * v2.Item3.W;
			result.Item3.Z = v1.Item0.Z * v2.Item3.X + v1.Item1.Z * v2.Item3.Y + v1.Item2.Z * v2.Item3.Z + v1.Item3.Z * v2.Item3.W;
			result.Item3.W = v1.Item0.W * v2.Item3.X + v1.Item1.W * v2.Item3.Y + v1.Item2.W * v2.Item3.Z + v1.Item3.W * v2.Item3.W;
		}

		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="m">The matrix to negate.</param>
		/// <param name="result">When the method completes, contains the negated matrix.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Negate( ref Matrix4F m, out Matrix4F result )
		{
			result.Item0.X = -m.Item0.X;
			result.Item0.Y = -m.Item0.Y;
			result.Item0.Z = -m.Item0.Z;
			result.Item0.W = -m.Item0.W;
			result.Item1.X = -m.Item1.X;
			result.Item1.Y = -m.Item1.Y;
			result.Item1.Z = -m.Item1.Z;
			result.Item1.W = -m.Item1.W;
			result.Item2.X = -m.Item2.X;
			result.Item2.Y = -m.Item2.Y;
			result.Item2.Z = -m.Item2.Z;
			result.Item2.W = -m.Item2.W;
			result.Item3.X = -m.Item3.X;
			result.Item3.Y = -m.Item3.Y;
			result.Item3.Z = -m.Item3.Z;
			result.Item3.W = -m.Item3.W;
		}

		//public static Mat4F Add( Mat4F v1, Mat4F v2 )
		//{
		//	Mat4F result;
		//	Add( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat4F Subtract( Mat4F v1, Mat4F v2 )
		//{
		//	Mat4F result;
		//	Subtract( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat4F Multiply( Mat4F m, float s )
		//{
		//	Mat4F result;
		//	Multiply( ref m, s, out result );
		//	return result;
		//}

		//public static Mat4F Multiply( float s, Mat4F m )
		//{
		//	Mat4F result;
		//	Multiply( ref m, s, out result );
		//	return result;
		//}

		//public static RayF Multiply( Mat4F m, RayF r )
		//{
		//	RayF result;
		//	Multiply( ref m, ref r, out result );
		//	return result;
		//}

		//public static RayF Multiply( RayF r, Mat4F m )
		//{
		//	RayF result;
		//	Multiply( ref m, ref r, out result );
		//	return result;
		//}

		//public static Vec3F Multiply( Mat4F m, Vec3F v )
		//{
		//	Vec3F result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Vec3F Multiply( Vec3F v, Mat4F m )
		//{
		//	Vec3F result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Vec4F Multiply( Mat4F m, Vec4F v )
		//{
		//	Vec4F result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Vec4F Multiply( Vec4F v, Mat4F m )
		//{
		//	Vec4F result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Mat4F Multiply( Mat4F v1, Mat4F v2 )
		//{
		//	Mat4F result;
		//	Multiply( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat4F Negate( Mat4F m )
		//{
		//	Mat4F result;
		//	Negate( ref m, out result );
		//	return result;
		//}

		/// <summary>
		/// Gets the trace of the matrix, the sum of the values along the diagonal.
		/// </summary>
		/// <returns>The trace of the matrix.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetTrace()
		{
			return Item0.X + Item1.Y + Item2.Z + Item3.W;
		}

		/// <summary>
		/// Transposes the matrix.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Transpose()
		{
			float v;
			v = Item0.Y; Item0.Y = Item1.X; Item1.X = v;
			v = Item0.Z; Item0.Z = Item2.X; Item2.X = v;
			v = Item0.W; Item0.W = Item3.X; Item3.X = v;
			v = Item1.Z; Item1.Z = Item2.Y; Item2.Y = v;
			v = Item1.W; Item1.W = Item3.Y; Item3.Y = v;
			v = Item2.W; Item2.W = Item3.Z; Item3.Z = v;
		}

		/// <summary>
		/// Returns the transpose of the current instance of <see cref="Matrix4F"/>.
		/// </summary>
		/// <returns>The transpose of the current instance of <see cref="Matrix4F"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4F GetTranspose()
		{
			Matrix4F result = this;
			result.Transpose();
			return result;
		}

		/// <summary>
		/// Calculates the transpose of the current instance of <see cref="Matrix4F"/>.
		/// </summary>
		/// <param name="result">When the method completes, contains the transpose of the current instance of <see cref="Matrix4F"/>.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetTranspose( out Matrix4F result )
		{
			//!!!!slowly. where else

			result = this;
			result.Transpose();
		}

		/// <summary>
		/// Calculates the transpose of the current instance of <see cref="Matrix4F"/>.
		/// </summary>
		/// <param name="result">When the method completes, contains the transpose of the current instance
		/// of <see cref="Matrix4F"/> in <see cref="Matrix3x4F"/> format.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetTranspose( out Matrix3x4F result )
		{
			result.Item0.X = Item0.X;
			result.Item0.Y = Item1.X;
			result.Item0.Z = Item2.X;
			result.Item0.W = Item3.X;

			result.Item1.X = Item0.Y;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = Item2.Y;
			result.Item1.W = Item3.Y;

			result.Item2.X = Item0.Z;
			result.Item2.Y = Item1.Z;
			result.Item2.Z = Item2.Z;
			result.Item2.W = Item3.Z;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix4F"/> and determines whether the matrix is invertible.
		/// Determines whether the current instance of <see cref="Matrix4F"/> is invertible and, if so, inverts this matrix.
		/// </summary>
		/// <returns>True if the current instance of <see cref="Matrix4F"/> is invertible; False otherwise.</returns>
		[MethodImpl( (MethodImplOptions)512 )]
		public bool Inverse()
		{
			double det, invDet;

			float det2_01_01 = Item0.X * Item1.Y - Item0.Y * Item1.X;
			float det2_01_02 = Item0.X * Item1.Z - Item0.Z * Item1.X;
			float det2_01_03 = Item0.X * Item1.W - Item0.W * Item1.X;
			float det2_01_12 = Item0.Y * Item1.Z - Item0.Z * Item1.Y;
			float det2_01_13 = Item0.Y * Item1.W - Item0.W * Item1.Y;
			float det2_01_23 = Item0.Z * Item1.W - Item0.W * Item1.Z;

			float det3_201_012 = Item2.X * det2_01_12 - Item2.Y * det2_01_02 + Item2.Z * det2_01_01;
			float det3_201_013 = Item2.X * det2_01_13 - Item2.Y * det2_01_03 + Item2.W * det2_01_01;
			float det3_201_023 = Item2.X * det2_01_23 - Item2.Z * det2_01_03 + Item2.W * det2_01_02;
			float det3_201_123 = Item2.Y * det2_01_23 - Item2.Z * det2_01_13 + Item2.W * det2_01_12;

			det = ( -det3_201_123 * Item3.X + det3_201_023 * Item3.Y - det3_201_013 * Item3.Z + det3_201_012 * Item3.W );

			if( Math.Abs( det ) < 1e-14 )
				return false;

			invDet = 1.0f / det;

			float det2_03_01 = Item0.X * Item3.Y - Item0.Y * Item3.X;
			float det2_03_02 = Item0.X * Item3.Z - Item0.Z * Item3.X;
			float det2_03_03 = Item0.X * Item3.W - Item0.W * Item3.X;
			float det2_03_12 = Item0.Y * Item3.Z - Item0.Z * Item3.Y;
			float det2_03_13 = Item0.Y * Item3.W - Item0.W * Item3.Y;
			float det2_03_23 = Item0.Z * Item3.W - Item0.W * Item3.Z;

			float det2_13_01 = Item1.X * Item3.Y - Item1.Y * Item3.X;
			float det2_13_02 = Item1.X * Item3.Z - Item1.Z * Item3.X;
			float det2_13_03 = Item1.X * Item3.W - Item1.W * Item3.X;
			float det2_13_12 = Item1.Y * Item3.Z - Item1.Z * Item3.Y;
			float det2_13_13 = Item1.Y * Item3.W - Item1.W * Item3.Y;
			float det2_13_23 = Item1.Z * Item3.W - Item1.W * Item3.Z;

			float det3_203_012 = Item2.X * det2_03_12 - Item2.Y * det2_03_02 + Item2.Z * det2_03_01;
			float det3_203_013 = Item2.X * det2_03_13 - Item2.Y * det2_03_03 + Item2.W * det2_03_01;
			float det3_203_023 = Item2.X * det2_03_23 - Item2.Z * det2_03_03 + Item2.W * det2_03_02;
			float det3_203_123 = Item2.Y * det2_03_23 - Item2.Z * det2_03_13 + Item2.W * det2_03_12;

			float det3_213_012 = Item2.X * det2_13_12 - Item2.Y * det2_13_02 + Item2.Z * det2_13_01;
			float det3_213_013 = Item2.X * det2_13_13 - Item2.Y * det2_13_03 + Item2.W * det2_13_01;
			float det3_213_023 = Item2.X * det2_13_23 - Item2.Z * det2_13_03 + Item2.W * det2_13_02;
			float det3_213_123 = Item2.Y * det2_13_23 - Item2.Z * det2_13_13 + Item2.W * det2_13_12;

			float det3_301_012 = Item3.X * det2_01_12 - Item3.Y * det2_01_02 + Item3.Z * det2_01_01;
			float det3_301_013 = Item3.X * det2_01_13 - Item3.Y * det2_01_03 + Item3.W * det2_01_01;
			float det3_301_023 = Item3.X * det2_01_23 - Item3.Z * det2_01_03 + Item3.W * det2_01_02;
			float det3_301_123 = Item3.Y * det2_01_23 - Item3.Z * det2_01_13 + Item3.W * det2_01_12;

			Item0.X = (float)( (double)-det3_213_123 * invDet );
			Item1.X = (float)( (double)+det3_213_023 * invDet );
			Item2.X = (float)( (double)-det3_213_013 * invDet );
			Item3.X = (float)( (double)+det3_213_012 * invDet );

			Item0.Y = (float)( (double)+det3_203_123 * invDet );
			Item1.Y = (float)( (double)-det3_203_023 * invDet );
			Item2.Y = (float)( (double)+det3_203_013 * invDet );
			Item3.Y = (float)( (double)-det3_203_012 * invDet );

			Item0.Z = (float)( (double)+det3_301_123 * invDet );
			Item1.Z = (float)( (double)-det3_301_023 * invDet );
			Item2.Z = (float)( (double)+det3_301_013 * invDet );
			Item3.Z = (float)( (double)-det3_301_012 * invDet );

			Item0.W = (float)( (double)-det3_201_123 * invDet );
			Item1.W = (float)( (double)+det3_201_023 * invDet );
			Item2.W = (float)( (double)-det3_201_013 * invDet );
			Item3.W = (float)( (double)+det3_201_012 * invDet );

			return true;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix4F"/> if it is invertible and returns the result.
		/// </summary>
		/// <returns>If the current instance of <see cref="Matrix4F"/> is invertible, returns the inverted matrix;
		/// otherwise returns the original matrix.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4F GetInverse()
		{
			Matrix4F result = this;
			result.Inverse();
			return result;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix4F"/> if it is invertible.
		/// </summary>
		/// <param name="result">When the method completes, contains the inverted matrix if the current instance
		/// of <see cref="Matrix4F"/> is invertible; otherwise, contains the original matrix.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetInverse( out Matrix4F result )
		{
			result = this;
			result.Inverse();
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Matrix4F"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Matrix4F"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Matrix4F"/>; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Matrix4F && this == (Matrix4F)obj );
		}

		/// <summary>
		/// Determines whether two given matricies are equal.
		/// </summary>
		/// <param name="v1">The first matrix to compare.</param>
		/// <param name="v2">The second matrix to compare.</param>
		/// <returns>True if the matricies are equal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Matrix4F v1, Matrix4F v2 )
		{
			return v1.Item0 == v2.Item0 && v1.Item1 == v2.Item1 && v1.Item2 == v2.Item2 && v1.Item3 == v2.Item3;
		}

		/// <summary>
		/// Determines whether two given matricies are unequal.
		/// </summary>
		/// <param name="v1">The first matrix to compare.</param>
		/// <param name="v2">The second matrix to compare.</param>
		/// <returns>True if the matricies are unequal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Matrix4F v1, Matrix4F v2 )
		{
			return v1.Item0 != v2.Item0 || v1.Item1 != v2.Item1 || v1.Item2 != v2.Item2 || v1.Item3 != v2.Item3;
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the matrix component, depending on the index.</value>
		/// <param name="row">The row of the matrix to access.</param>
		/// <param name="column">The column of the matrix to access.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>
		/// is out of the range [0, 3].</exception>
		public unsafe float this[ int row, int column ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( row < 0 || row > 3 )
					throw new ArgumentOutOfRangeException( "row" );
				if( column < 0 || column > 3 )
					throw new ArgumentOutOfRangeException( "column" );
				fixed( float* v = &this.Item0.X )
					return v[ row * 4 + column ];
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( row < 0 || row > 3 )
					throw new ArgumentOutOfRangeException( "row" );
				if( column < 0 || column > 3 )
					throw new ArgumentOutOfRangeException( "column" );
				fixed( float* v = &this.Item0.X )
					v[ row * 4 + column ] = value;
			}
		}

		/// <summary>
		/// Determines whether the specified matrix is equal to the current instance of <see cref="Matrix4F"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The matrix to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified matrix is equal to the current instance of <see cref="Matrix4F"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Matrix4F v, float epsilon )
		{
			if( !Item0.Equals( ref v.Item0, epsilon ) )
				return false;
			if( !Item1.Equals( ref v.Item1, epsilon ) )
				return false;
			if( !Item2.Equals( ref v.Item2, epsilon ) )
				return false;
			if( !Item3.Equals( ref v.Item3, epsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Creates a look-at matrix.
		/// </summary>
		/// <param name="eye">Eye (camera) position in world space.</param>
		/// <param name="dir">Target position in world space.</param>
		/// <param name="up">Up vector in world space.</param>
		/// <returns>The instance of <see cref="Matrix4F"/> that transforms world space to camera space.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Matrix4F LookAt( Vector3F eye, Vector3F dir, Vector3F up )
		{
			Matrix4F result;
			LookAt( ref eye, ref dir, ref up, out result );
			return result;
		}

		/// <summary>
		/// Creates a look-at matrix.
		/// </summary>
		/// <param name="eye">Eye (camera) position in world space.</param>
		/// <param name="dir">Target position in world space.</param>
		/// <param name="up">Up vector in world space.</param>
		/// <param name="result">When the method completes, contains the instance of <see cref="Matrix4F"/> that transforms world space to camera space.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void LookAt( ref Vector3F eye, ref Vector3F dir, ref Vector3F up, out Matrix4F result )
		{
			Vector3F z;
			Vector3F.Subtract( ref eye, ref dir, out z );
			//Vec3 z = eye - dir;
			z.Normalize();

			Vector3F x;
			Vector3F.Cross( ref up, ref z, out x );
			//Vec3 x = Vec3.Cross( up, z );

			x.Normalize();
			Vector3F y;
			Vector3F.Cross( ref z, ref x, out y );
			//Vec3 y = Vec3.Cross( z, x );
			y.Normalize();

			result.Item0.X = x.X; result.Item1.X = x.Y; result.Item2.X = x.Z; result.Item3.X = 0;
			result.Item0.Y = y.X; result.Item1.Y = y.Y; result.Item2.Y = y.Z; result.Item3.Y = 0;
			result.Item0.Z = z.X; result.Item1.Z = z.Y; result.Item2.Z = z.Z; result.Item3.Z = 0;
			result.Item0.W = 0.0f; result.Item1.W = 0.0f; result.Item2.W = 0.0f; result.Item3.W = 1.0f;

			Matrix4F m1 = Matrix4F.Identity;
			m1[ 3, 0 ] = -eye.X;
			m1[ 3, 1 ] = -eye.Y;
			m1[ 3, 2 ] = -eye.Z;
			result *= m1;
		}

		/// <summary>
		/// Creates a perspective projection matrix. 
		/// </summary>
		/// <param name="fov">Angle of the field of view in the y direction.</param>
		/// <param name="aspect">Aspect ratio of the view (width / height).</param>
		/// <param name="znear">Distance to the near clip plane.</param>
		/// <param name="zfar">Distance to the far clip plane.</param>
		/// <returns>A projection matrix that transforms camera space to raster space.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Matrix4F Perspective( float fov, float aspect, float znear, float zfar )
		{
			Matrix4F result;
			Perspective( fov, aspect, znear, zfar, out result );
			return result;
		}

		/// <summary>
		/// Creates a perspective projection matrix.
		/// </summary>
		/// <param name="fov">Angle of the field of view in the y direction.</param>
		/// <param name="aspect">Aspect ratio of the view (width / height).</param>
		/// <param name="znear">Distance to the near clip plane.</param>
		/// <param name="zfar">Distance to the far clip plane.</param>
		/// <param name="result">When the method completes, contains a projection matrix that transforms camera space to raster space.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Perspective( float fov, float aspect, float znear, float zfar,
			out Matrix4F result )
		{
			float y = MathEx.Tan( fov * (float)Math.PI / 360.0f );
			float x = y * aspect;

			result.Item0.X = 1.0f / x;
			result.Item1.X = 0.0f;
			result.Item2.X = 0.0f;
			result.Item3.X = 0.0f;
			result.Item0.Y = 0.0f;
			result.Item1.Y = 1.0f / y;
			result.Item2.Y = 0.0f;
			result.Item3.Y = 0.0f;
			result.Item0.Z = 0.0f;
			result.Item1.Z = 0.0f;
			result.Item2.Z = -( zfar + znear ) / ( zfar - znear );
			result.Item3.Z = -( 2.0f * zfar * znear ) / ( zfar - znear );
			result.Item0.W = 0.0f;
			result.Item1.W = 0.0f;
			result.Item2.W = -1.0f;
			result.Item3.W = 0.0f;
		}

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="translation">The amount to translate in each axis.</param>
		/// <returns>The translation matrix.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Matrix4F FromTranslate( Vector3F translation )
		{
			Matrix4F result;
			result.Item0.X = 1;
			result.Item0.Y = 0;
			result.Item0.Z = 0;
			result.Item0.W = 0;
			result.Item1.X = 0;
			result.Item1.Y = 1;
			result.Item1.Z = 0;
			result.Item1.W = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = 1;
			result.Item2.W = 0;
			result.Item3.X = translation.X;
			result.Item3.Y = translation.Y;
			result.Item3.Z = translation.Z;
			result.Item3.W = 1;
			return result;
		}

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="translation">The amount to translate in each axis.</param>
		/// <param name="result">When the method completes, contains the translation matrix.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromTranslate( ref Vector3F translation, out Matrix4F result )
		{
			result.Item0.X = 1;
			result.Item0.Y = 0;
			result.Item0.Z = 0;
			result.Item0.W = 0;
			result.Item1.X = 0;
			result.Item1.Y = 1;
			result.Item1.Z = 0;
			result.Item1.W = 0;
			result.Item2.X = 0;
			result.Item2.Y = 0;
			result.Item2.Z = 1;
			result.Item2.W = 0;
			result.Item3.X = translation.X;
			result.Item3.Y = translation.Y;
			result.Item3.Z = translation.Z;
			result.Item3.W = 1;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix4F"/> into the equivalent <see cref="Matrix3F"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Matrix3F"/> structure.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Matrix3F ToMatrix3()
		{
			Matrix3F result;
			result.Item0.X = Item0.X;
			result.Item0.Y = Item0.Y;
			result.Item0.Z = Item0.Z;
			result.Item1.X = Item1.X;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = Item1.Z;
			result.Item2.X = Item2.X;
			result.Item2.Y = Item2.Y;
			result.Item2.Z = Item2.Z;
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix4F"/> into the equivalent <see cref="Matrix3F"/> structure.
		/// </summary>
		/// <param name="result">When the method completes, contains the equivalent <see cref="Matrix3F"/> structure.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToMatrix3( out Matrix3F result )
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
		/// Converts a string representation of a matrix into the equivalent <see cref="Matrix4F"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the matrix (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Matrix4F"/> structure.</returns>
		[AutoConvertType]
		public static Matrix4F Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 16 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 16 parts separated by spaces.", text ) );

			try
			{
				return new Matrix4F(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ),
					float.Parse( vals[ 2 ] ),
					float.Parse( vals[ 3 ] ),
					float.Parse( vals[ 4 ] ),
					float.Parse( vals[ 5 ] ),
					float.Parse( vals[ 6 ] ),
					float.Parse( vals[ 7 ] ),
					float.Parse( vals[ 8 ] ),
					float.Parse( vals[ 9 ] ),
					float.Parse( vals[ 10 ] ),
					float.Parse( vals[ 11 ] ),
					float.Parse( vals[ 12 ] ),
					float.Parse( vals[ 13 ] ),
					float.Parse( vals[ 14 ] ),
					float.Parse( vals[ 15 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Matrix4F"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Matrix4F"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1} {2} {3}", Item0.ToString(), Item1.ToString(), Item2.ToString(), Item3.ToString() );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix4F"/> to the matrix of <see cref="Matrix4"/> format.
		/// </summary>
		/// <param name="result">When the method completes, contains the matrix of <see cref="Matrix4"/> format.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToMatrix4( out Matrix4 result )
		{
			result.Item0.X = Item0.X;
			result.Item0.Y = Item0.Y;
			result.Item0.Z = Item0.Z;
			result.Item0.W = Item0.W;
			result.Item1.X = Item1.X;
			result.Item1.Y = Item1.Y;
			result.Item1.Z = Item1.Z;
			result.Item1.W = Item1.W;
			result.Item2.X = Item2.X;
			result.Item2.Y = Item2.Y;
			result.Item2.Z = Item2.Z;
			result.Item2.W = Item2.W;
			result.Item3.X = Item3.X;
			result.Item3.Y = Item3.Y;
			result.Item3.Z = Item3.Z;
			result.Item3.W = Item3.W;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix4F"/> to the matrix of <see cref="Matrix4H"/> format.
		/// </summary>
		/// <param name="result">When the method completes, contains the matrix of <see cref="Matrix4H"/> format.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToMatrix4H( out Matrix4H result )
		{
			result.Item0.X = new HalfType( Item0.X );
			result.Item0.Y = new HalfType( Item0.Y );
			result.Item0.Z = new HalfType( Item0.Z );
			result.Item0.W = new HalfType( Item0.W );
			result.Item1.X = new HalfType( Item1.X );
			result.Item1.Y = new HalfType( Item1.Y );
			result.Item1.Z = new HalfType( Item1.Z );
			result.Item1.W = new HalfType( Item1.W );
			result.Item2.X = new HalfType( Item2.X );
			result.Item2.Y = new HalfType( Item2.Y );
			result.Item2.Z = new HalfType( Item2.Z );
			result.Item2.W = new HalfType( Item2.W );
			result.Item3.X = new HalfType( Item3.X );
			result.Item3.Y = new HalfType( Item3.Y );
			result.Item3.Z = new HalfType( Item3.Z );
			result.Item3.W = new HalfType( Item3.W );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix4F"/> to the matrix of <see cref="Matrix4"/> format.
		/// </summary>
		/// <returns>The matrix of <see cref="Matrix4"/> format.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Matrix4 ToMatrix4()
		{
			ToMatrix4( out var result );
			return result;
		}

#if !DISABLE_IMPLICIT
		/// <summary>
		/// Implicit conversion from <see cref="Matrix4F"/> type to <see cref="Matrix4"/> type for given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Matrix4( Matrix4F v )
		{
			return new Matrix4( v );
		}
#endif
		/// <summary>
		/// Returns the translation of the current instance of <see cref="Matrix4F"/>.
		/// </summary>
		/// <returns>The translation of the current instance of <see cref="Matrix4F"/>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3F GetTranslation()
		{
			return Item3.ToVector3F();
		}

		/// <summary>
		/// Returns the translation of the current instance of <see cref="Matrix4F"/>.
		/// </summary>
		/// <param name="result">When the method completes, contains the translation of the current instance of <see cref="Matrix4F"/>.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetTranslation( out Vector3F result )
		{
			result = Item3.ToVector3F();
		}

		/// <summary>
		/// Sets the translation of the current instance of <see cref="Matrix4F"/>.
		/// </summary>
		/// <param name="value">The translation to set.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SetTranslation( Vector3F value )
		{
			Item3 = new Vector4F( value, Item3.W );
		}

		/// <summary>
		/// Sets the translation of the current instance of <see cref="Matrix4F"/>.
		/// </summary>
		/// <param name="value">The translation to set.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SetTranslation( ref Vector3F value )
		{
			Item3 = new Vector4F( value, Item3.W );
		}

		/// <summary>
		/// Decomposes a matrix into a scale, rotation, and translation.
		/// </summary>
		/// <param name="translation">When the method completes, contains the translation component of the decomposed matrix.</param>
		/// <param name="rotation">When the method completes, contains the rtoation component of the decomposed matrix.</param>
		/// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
		/// <remarks>
		/// This method is designed to decompose an SRT transformation matrix only.
		/// </remarks>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Decompose( out Vector3F translation, out Matrix3F rotation, out Vector3F scale )
		{
			translation = GetTranslation();

			ToMatrix3( out Matrix3F rotationMat3 );
			return rotationMat3.Decompose( out rotation, out scale );
		}

		/// <summary>
		/// Decomposes a matrix into a scale, rotation, and translation.
		/// </summary>
		/// <param name="translation">When the method completes, contains the translation component of the decomposed matrix.</param>
		/// <param name="rotation">When the method completes, contains the rtoation component of the decomposed matrix.</param>
		/// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
		/// <remarks>
		/// This method is designed to decompose an SRT transformation matrix only.
		/// </remarks>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Decompose( out Vector3F translation, out QuaternionF rotation, out Vector3F scale )
		{
			if( !Decompose( out translation, out Matrix3F rotationMat3, out scale ) )
			{
				rotation = QuaternionF.Identity;
				return false;
			}
			rotationMat3.ToQuaternion( out rotation );
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void DecomposeScale( out Vector3F result )
		{
			ToMatrix3( out Matrix3F rotationMat3 );
			rotationMat3.DecomposeScale( out result );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3F DecomposeScale()
		{
			DecomposeScale( out var result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float DecomposeScaleMaxComponent()
		{
			var scaleSquared = new Vector3F(
				Item0.X * Item0.X + Item0.Y * Item0.Y + Item0.Z * Item0.Z,
				Item1.X * Item1.X + Item1.Y * Item1.Y + Item1.Z * Item1.Z,
				Item2.X * Item2.X + Item2.Y * Item2.Y + Item2.Z * Item2.Z );

			return MathEx.Sqrt( scaleSquared.MaxComponent() );

			//ToMatrix3( out Matrix3F rotationMat3 );
			//return rotationMat3.DecomposeScaleMaxComponent();
		}
	}
}

