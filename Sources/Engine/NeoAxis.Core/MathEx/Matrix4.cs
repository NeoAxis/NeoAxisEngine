// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// A structure encapsulating a double precision 4x4 matrix.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Matrix4
	{
		/// <summary>
		/// The matrix with all of its components set to zero.
		/// </summary>
		public static readonly Matrix4 Zero = new Matrix4( 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
		/// <summary>
		/// The identity matrix.
		/// </summary>
		public static readonly Matrix4 Identity = new Matrix4( 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 );

		//

		/// <summary>
		/// The first row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector4 Item0;
		/// <summary>
		/// The second row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector4 Item1;
		/// <summary>
		/// The third row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector4 Item2;
		/// <summary>
		/// The fourth row of the matrix.
		/// </summary>
		[Browsable( false )]
		public Vector4 Item3;

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
		public Matrix4(
			double xx, double xy, double xz, double xw,
			double yx, double yy, double yz, double yw,
			double zx, double zy, double zz, double zw,
			double wx, double wy, double wz, double ww )
		{
			Item0 = new Vector4( xx, xy, xz, xw );
			Item1 = new Vector4( yx, yy, yz, yw );
			Item2 = new Vector4( zx, zy, zz, zw );
			Item3 = new Vector4( wx, wy, wz, ww );
		}

		/// <summary>
		/// Constructs a matrix with the specified <see cref="Vector4"/> elements,
		/// which are the corresponding rows of the matrix.
		/// </summary>
		/// <param name="x">The vector which is the first row.</param>
		/// <param name="y">The vector which is the second row.</param>
		/// <param name="z">The vector which is the third row.</param>
		/// <param name="w">The vector which is the fourth row.</param>
		public Matrix4( Vector4 x, Vector4 y, Vector4 z, Vector4 w )
		{
			Item0 = x;
			Item1 = y;
			Item2 = z;
			Item3 = w;
		}

		/// <summary>
		/// Constructs a matrix with another specified <see cref="Matrix4"/> object.
		/// </summary>
		/// <param name="source">A specified matrix.</param>
		public Matrix4( Matrix4 source )
		{
			Item0 = source.Item0;
			Item1 = source.Item1;
			Item2 = source.Item2;
			Item3 = source.Item3;
		}

		/// <summary>
		/// Constructs a matrix with the given rotation and translation components.
		/// </summary>
		/// <param name="rotation">The rotation <see cref="Matrix3"/>.</param>
		/// <param name="translation">The translation <see cref="Vector3"/>.</param>
		public Matrix4( Matrix3 rotation, Vector3 translation )
		{
			Item0.X = rotation.Item0.X;
			Item0.Y = rotation.Item0.Y;
			Item0.Z = rotation.Item0.Z;
			Item0.W = 0.0;
			Item1.X = rotation.Item1.X;
			Item1.Y = rotation.Item1.Y;
			Item1.Z = rotation.Item1.Z;
			Item1.W = 0.0;
			Item2.X = rotation.Item2.X;
			Item2.Y = rotation.Item2.Y;
			Item2.Z = rotation.Item2.Z;
			Item2.W = 0.0;
			Item3.X = translation.X;
			Item3.Y = translation.Y;
			Item3.Z = translation.Z;
			Item3.W = 1.0;
		}

		/// <summary>
		/// Constructs a matrix with the given rotation and translation components.
		/// </summary>
		/// <param name="rotation">The rotation <see cref="Matrix3"/>.</param>
		/// <param name="translation">The translation <see cref="Vector3"/>.</param>
		public Matrix4( ref Matrix3 rotation, ref Vector3 translation )
		{
			Item0.X = rotation.Item0.X;
			Item0.Y = rotation.Item0.Y;
			Item0.Z = rotation.Item0.Z;
			Item0.W = 0.0;
			Item1.X = rotation.Item1.X;
			Item1.Y = rotation.Item1.Y;
			Item1.Z = rotation.Item1.Z;
			Item1.W = 0.0;
			Item2.X = rotation.Item2.X;
			Item2.Y = rotation.Item2.Y;
			Item2.Z = rotation.Item2.Z;
			Item2.W = 0.0;
			Item3.X = translation.X;
			Item3.Y = translation.Y;
			Item3.Z = translation.Z;
			Item3.W = 1.0;
		}

		//!!!!!по сути не надо. само должно
		//!!!!Quat добавить?
		/// <summary>
		/// Constructs a matrix with the given rotation and translation components.
		/// </summary>
		/// <param name="rotation">The rotation <see cref="Matrix3F"/>.</param>
		/// <param name="translation">The translation <see cref="Vector3"/>.</param>
		public Matrix4( Matrix3F rotation, Vector3 translation )
		{
			Item0.X = rotation.Item0.X;
			Item0.Y = rotation.Item0.Y;
			Item0.Z = rotation.Item0.Z;
			Item0.W = 0.0;
			Item1.X = rotation.Item1.X;
			Item1.Y = rotation.Item1.Y;
			Item1.Z = rotation.Item1.Z;
			Item1.W = 0.0;
			Item2.X = rotation.Item2.X;
			Item2.Y = rotation.Item2.Y;
			Item2.Z = rotation.Item2.Z;
			Item2.W = 0.0;
			Item3.X = translation.X;
			Item3.Y = translation.Y;
			Item3.Z = translation.Z;
			Item3.W = 1.0;
		}

		/// <summary>
		/// Constructs a matrix with the given rotation and translation components.
		/// </summary>
		/// <param name="rotation">The rotation <see cref="Matrix3F"/>.</param>
		/// <param name="translation">The translation <see cref="Vector3"/>.</param>
		public Matrix4( ref Matrix3F rotation, ref Vector3 translation )
		{
			Item0.X = rotation.Item0.X;
			Item0.Y = rotation.Item0.Y;
			Item0.Z = rotation.Item0.Z;
			Item0.W = 0.0;
			Item1.X = rotation.Item1.X;
			Item1.Y = rotation.Item1.Y;
			Item1.Z = rotation.Item1.Z;
			Item1.W = 0.0;
			Item2.X = rotation.Item2.X;
			Item2.Y = rotation.Item2.Y;
			Item2.Z = rotation.Item2.Z;
			Item2.W = 0.0;
			Item3.X = translation.X;
			Item3.Y = translation.Y;
			Item3.Z = translation.Z;
			Item3.W = 1.0;
		}

		/// <summary>
		/// Constructs a matrix with the given rotation and translation components and returns the resulting <see cref="Matrix4"/>.
		/// </summary>
		/// <param name="rotation">The rotation <see cref="Matrix3"/>.</param>
		/// <param name="translation">The translation <see cref="Vector3"/>.</param>
		/// <param name="result">When the method completes, contains the resulting <see cref="Matrix4"/>.</param>
		public static void Construct( ref Matrix3 rotation, ref Vector3 translation, out Matrix4 result )
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
		/// Constructs a matrix with specified one-dimensional <see cref="double"/> array with sixteen elements.
		/// </summary>
		/// <param name="array">One-dimensional <see cref="double"/> array with sixteen elements.</param>
		public Matrix4( double[] array )
		{
			Item0 = new Vector4( array[ 0 ], array[ 1 ], array[ 2 ], array[ 3 ] );
			Item1 = new Vector4( array[ 4 ], array[ 5 ], array[ 6 ], array[ 7 ] );
			Item2 = new Vector4( array[ 8 ], array[ 9 ], array[ 10 ], array[ 11 ] );
			Item3 = new Vector4( array[ 12 ], array[ 13 ], array[ 14 ], array[ 15 ] );
		}

		/// <summary>
		/// Constructs a matrix with another specified matrix of <see cref="Matrix4F"/> format.
		/// </summary>
		/// <param name="source">A specified matrix.</param>
		public Matrix4( Matrix4F source )
		{
			Item0 = source.Item0.ToVector4();
			Item1 = source.Item1.ToVector4();
			Item2 = source.Item2.ToVector4();
			Item3 = source.Item3.ToVector4();
		}

		/// <summary>
		/// Gets or sets the row of the current instance of <see cref="Matrix4"/> at the specified index.
		/// </summary>
		/// <value>The value of the corresponding row of <see cref="Vector4"/> format, depending on the index.</value>
		/// <param name="index">The index of the row to access.</param>
		/// <returns>The value of the row of <see cref="Vector4"/> format at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
		public unsafe Vector4 this[ int index ]
		{
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector4* v = &this.Item0 )
					return v[ index ];
			}
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector4* v = &this.Item0 )
					v[ index ] = value;
			}
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
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
		public static Matrix4 operator +( Matrix4 v1, Matrix4 v2 )
		{
			Matrix4 result;
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
		public static Matrix4 operator -( Matrix4 v1, Matrix4 v2 )
		{
			Matrix4 result;
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
		public static Matrix4 operator *( Matrix4 m, double s )
		{
			Matrix4 result;
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
		public static Matrix4 operator *( double s, Matrix4 m )
		{
			Matrix4 result;
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
		public static Ray operator *( Matrix4 m, Ray r )
		{
			Ray result;
			Matrix4.Multiply( ref r.Origin, ref m, out result.Origin );
			Vector3 sourceTo;
			Vector3.Add( ref r.Origin, ref r.Direction, out sourceTo );
			Vector3 to;
			Matrix4.Multiply( ref sourceTo, ref m, out to );
			Vector3.Subtract( ref to, ref result.Origin, out result.Direction );
			return result;
			//Vec3D origin = r.Origin * m;
			//Vec3D to = ( r.Origin + r.Direction ) * m;
			//return new Ray( origin, to - origin );
		}

		/// <summary>
		/// Translates the ray coordinates to the space defined by a matrix.
		/// </summary>
		/// <param name="r">The given ray.</param>
		/// <param name="m">The given matrix.</param>
		/// <returns>The translated ray.</returns>
		public static Ray operator *( Ray r, Matrix4 m )
		{
			Ray result;
			Matrix4.Multiply( ref r.Origin, ref m, out result.Origin );
			Vector3 sourceTo;
			Vector3.Add( ref r.Origin, ref r.Direction, out sourceTo );
			Vector3 to;
			Matrix4.Multiply( ref sourceTo, ref m, out to );
			Vector3.Subtract( ref to, ref result.Origin, out result.Direction );
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector3 operator *( Matrix4 m, Vector3 v )
		{
			Vector3 result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector3 operator *( Vector3 v, Matrix4 m )
		{
			Vector3 result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector4"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector4 operator *( Matrix4 m, Vector4 v )
		{
			Vector4 result;
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X * v.W;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y * v.W;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z * v.W;
			result.W = m.Item0.W * v.X + m.Item1.W * v.Y + m.Item2.W * v.Z + m.Item3.W * v.W;
			return result;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector4"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <returns>The result of the multiplication.</returns>
		public static Vector4 operator *( Vector4 v, Matrix4 m )
		{
			Vector4 result;
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
		public static Matrix4 operator *( Matrix4 v1, Matrix4 v2 )
		{
			Matrix4 result;
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
		public static Matrix4 operator -( Matrix4 v )
		{
			Matrix4 result;
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
		public static void Add( ref Matrix4 v1, ref Matrix4 v2, out Matrix4 result )
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
		public static void Subtract( ref Matrix4 v1, ref Matrix4 v2, out Matrix4 result )
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
		public static void Multiply( ref Matrix4 m, double s, out Matrix4 result )
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
		public static void Multiply( double s, ref Matrix4 m, out Matrix4 result )
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
		public static void Multiply( ref Matrix4 m, ref Ray r, out Ray result )
		{
			Matrix4.Multiply( ref r.Origin, ref m, out result.Origin );
			Vector3 sourceTo;
			Vector3.Add( ref r.Origin, ref r.Direction, out sourceTo );
			Vector3 to;
			Matrix4.Multiply( ref sourceTo, ref m, out to );
			Vector3.Subtract( ref to, ref result.Origin, out result.Direction );
		}

		/// <summary>
		/// Translates the ray coordinates to the space defined by a matrix.
		/// </summary>
		/// <param name="r">The given ray.</param>
		/// <param name="m">The given matrix.</param>
		/// <param name="result">When the method completes, contains the translated ray.</param>
		public static void Multiply( ref Ray r, ref Matrix4 m, out Ray result )
		{
			Matrix4.Multiply( ref r.Origin, ref m, out result.Origin );
			Vector3 sourceTo;
			Vector3.Add( ref r.Origin, ref r.Direction, out sourceTo );
			Vector3 to;
			Matrix4.Multiply( ref sourceTo, ref m, out to );
			Vector3.Subtract( ref to, ref result.Origin, out result.Direction );
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		public static void Multiply( ref Matrix4 m, ref Vector3 v, out Vector3 result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector3"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		public static void Multiply( ref Vector3 v, ref Matrix4 m, out Vector3 result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector4"/> structure.
		/// </summary>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		public static void Multiply( ref Matrix4 m, ref Vector4 v, out Vector4 result )
		{
			result.X = m.Item0.X * v.X + m.Item1.X * v.Y + m.Item2.X * v.Z + m.Item3.X * v.W;
			result.Y = m.Item0.Y * v.X + m.Item1.Y * v.Y + m.Item2.Y * v.Z + m.Item3.Y * v.W;
			result.Z = m.Item0.Z * v.X + m.Item1.Z * v.Y + m.Item2.Z * v.Z + m.Item3.Z * v.W;
			result.W = m.Item0.W * v.X + m.Item1.W * v.Y + m.Item2.W * v.Z + m.Item3.W * v.W;
		}

		/// <summary>
		/// Multiplies a matrix by a given <see cref="Vector4"/> structure.
		/// </summary>
		/// <param name="v">The vector by which to multiply.</param>
		/// <param name="m">The matrix to multiply.</param>
		/// <param name="result">When the method completes, contains the result of the multiplication.</param>
		public static void Multiply( ref Vector4 v, ref Matrix4 m, out Vector4 result )
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
		public static void Multiply( ref Matrix4 v1, ref Matrix4 v2, out Matrix4 result )
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
		public static void Negate( ref Matrix4 m, out Matrix4 result )
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

		//public static Mat4 Add( Mat4 v1, Mat4 v2 )
		//{
		//	Mat4 result;
		//	Add( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat4 Subtract( Mat4 v1, Mat4 v2 )
		//{
		//	Mat4 result;
		//	Subtract( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat4 Multiply( Mat4 m, double s )
		//{
		//	Mat4 result;
		//	Multiply( ref m, s, out result );
		//	return result;
		//}

		//public static Mat4 Multiply( double s, Mat4 m )
		//{
		//	Mat4 result;
		//	Multiply( ref m, s, out result );
		//	return result;
		//}

		//public static Ray Multiply( Mat4 m, Ray r )
		//{
		//	Ray result;
		//	Multiply( ref m, ref r, out result );
		//	return result;
		//}

		//public static Ray Multiply( Ray r, Mat4 m )
		//{
		//	Ray result;
		//	Multiply( ref m, ref r, out result );
		//	return result;
		//}

		//public static Vec3 Multiply( Mat4 m, Vec3 v )
		//{
		//	Vec3 result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Vec3 Multiply( Vec3 v, Mat4 m )
		//{
		//	Vec3 result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Vec4 Multiply( Mat4 m, Vec4 v )
		//{
		//	Vec4 result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Vec4 Multiply( Vec4 v, Mat4 m )
		//{
		//	Vec4 result;
		//	Multiply( ref m, ref v, out result );
		//	return result;
		//}

		//public static Mat4 Multiply( Mat4 v1, Mat4 v2 )
		//{
		//	Mat4 result;
		//	Multiply( ref v1, ref v2, out result );
		//	return result;
		//}

		//public static Mat4 Negate( Mat4 m )
		//{
		//	Mat4 result;
		//	Negate( ref m, out result );
		//	return result;
		//}

		/// <summary>
		/// Gets the trace of the matrix, the sum of the values along the diagonal.
		/// </summary>
		/// <returns>The trace of the matrix.</returns>
		public double GetTrace()
		{
			return Item0.X + Item1.Y + Item2.Z + Item3.W;
		}

		/// <summary>
		/// Transposes the matrix.
		/// </summary>
		public void Transpose()
		{
			double v;
			v = Item0.Y; Item0.Y = Item1.X; Item1.X = v;
			v = Item0.Z; Item0.Z = Item2.X; Item2.X = v;
			v = Item0.W; Item0.W = Item3.X; Item3.X = v;
			v = Item1.Z; Item1.Z = Item2.Y; Item2.Y = v;
			v = Item1.W; Item1.W = Item3.Y; Item3.Y = v;
			v = Item2.W; Item2.W = Item3.Z; Item3.Z = v;
		}

		/// <summary>
		/// Returns the transpose of the current instance of <see cref="Matrix4"/>.
		/// </summary>
		/// <returns>The transpose of the current instance of <see cref="Matrix4"/>.</returns>
		public Matrix4 GetTranspose()
		{
			Matrix4 result = this;
			result.Transpose();
			return result;
		}

		/// <summary>
		/// Calculates the transpose of the current instance of <see cref="Matrix4"/>.
		/// </summary>
		/// <param name="result">When the method completes, contains the transpose of the current instance of <see cref="Matrix4"/>.</param>
		public void GetTranspose( out Matrix4 result )
		{
			result = this;
			result.Transpose();
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix4"/> and determines whether the matrix is invertible.
		/// Determines whether the current instance of <see cref="Matrix4"/> is invertible and, if so, inverts this matrix.
		/// </summary>
		/// <returns>True if the current instance of <see cref="Matrix4"/> is invertible; False otherwise.</returns>
		public bool Inverse()
		{
			double det, invDet;

			double det2_01_01 = Item0.X * Item1.Y - Item0.Y * Item1.X;
			double det2_01_02 = Item0.X * Item1.Z - Item0.Z * Item1.X;
			double det2_01_03 = Item0.X * Item1.W - Item0.W * Item1.X;
			double det2_01_12 = Item0.Y * Item1.Z - Item0.Z * Item1.Y;
			double det2_01_13 = Item0.Y * Item1.W - Item0.W * Item1.Y;
			double det2_01_23 = Item0.Z * Item1.W - Item0.W * Item1.Z;

			double det3_201_012 = Item2.X * det2_01_12 - Item2.Y * det2_01_02 + Item2.Z * det2_01_01;
			double det3_201_013 = Item2.X * det2_01_13 - Item2.Y * det2_01_03 + Item2.W * det2_01_01;
			double det3_201_023 = Item2.X * det2_01_23 - Item2.Z * det2_01_03 + Item2.W * det2_01_02;
			double det3_201_123 = Item2.Y * det2_01_23 - Item2.Z * det2_01_13 + Item2.W * det2_01_12;

			det = ( -det3_201_123 * Item3.X + det3_201_023 * Item3.Y - det3_201_013 * Item3.Z + det3_201_012 * Item3.W );

			if( Math.Abs( det ) < 1e-14 )
				return false;

			invDet = 1.0 / det;

			double det2_03_01 = Item0.X * Item3.Y - Item0.Y * Item3.X;
			double det2_03_02 = Item0.X * Item3.Z - Item0.Z * Item3.X;
			double det2_03_03 = Item0.X * Item3.W - Item0.W * Item3.X;
			double det2_03_12 = Item0.Y * Item3.Z - Item0.Z * Item3.Y;
			double det2_03_13 = Item0.Y * Item3.W - Item0.W * Item3.Y;
			double det2_03_23 = Item0.Z * Item3.W - Item0.W * Item3.Z;

			double det2_13_01 = Item1.X * Item3.Y - Item1.Y * Item3.X;
			double det2_13_02 = Item1.X * Item3.Z - Item1.Z * Item3.X;
			double det2_13_03 = Item1.X * Item3.W - Item1.W * Item3.X;
			double det2_13_12 = Item1.Y * Item3.Z - Item1.Z * Item3.Y;
			double det2_13_13 = Item1.Y * Item3.W - Item1.W * Item3.Y;
			double det2_13_23 = Item1.Z * Item3.W - Item1.W * Item3.Z;

			double det3_203_012 = Item2.X * det2_03_12 - Item2.Y * det2_03_02 + Item2.Z * det2_03_01;
			double det3_203_013 = Item2.X * det2_03_13 - Item2.Y * det2_03_03 + Item2.W * det2_03_01;
			double det3_203_023 = Item2.X * det2_03_23 - Item2.Z * det2_03_03 + Item2.W * det2_03_02;
			double det3_203_123 = Item2.Y * det2_03_23 - Item2.Z * det2_03_13 + Item2.W * det2_03_12;

			double det3_213_012 = Item2.X * det2_13_12 - Item2.Y * det2_13_02 + Item2.Z * det2_13_01;
			double det3_213_013 = Item2.X * det2_13_13 - Item2.Y * det2_13_03 + Item2.W * det2_13_01;
			double det3_213_023 = Item2.X * det2_13_23 - Item2.Z * det2_13_03 + Item2.W * det2_13_02;
			double det3_213_123 = Item2.Y * det2_13_23 - Item2.Z * det2_13_13 + Item2.W * det2_13_12;

			double det3_301_012 = Item3.X * det2_01_12 - Item3.Y * det2_01_02 + Item3.Z * det2_01_01;
			double det3_301_013 = Item3.X * det2_01_13 - Item3.Y * det2_01_03 + Item3.W * det2_01_01;
			double det3_301_023 = Item3.X * det2_01_23 - Item3.Z * det2_01_03 + Item3.W * det2_01_02;
			double det3_301_123 = Item3.Y * det2_01_23 - Item3.Z * det2_01_13 + Item3.W * det2_01_12;

			Item0.X = -det3_213_123 * invDet;
			Item1.X = +det3_213_023 * invDet;
			Item2.X = -det3_213_013 * invDet;
			Item3.X = +det3_213_012 * invDet;

			Item0.Y = +det3_203_123 * invDet;
			Item1.Y = -det3_203_023 * invDet;
			Item2.Y = +det3_203_013 * invDet;
			Item3.Y = -det3_203_012 * invDet;

			Item0.Z = +det3_301_123 * invDet;
			Item1.Z = -det3_301_023 * invDet;
			Item2.Z = +det3_301_013 * invDet;
			Item3.Z = -det3_301_012 * invDet;

			Item0.W = -det3_201_123 * invDet;
			Item1.W = +det3_201_023 * invDet;
			Item2.W = -det3_201_013 * invDet;
			Item3.W = +det3_201_012 * invDet;

			return true;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix4"/> if it is invertible and returns the result.
		/// </summary>
		/// <returns>If the current instance of <see cref="Matrix4"/> is invertible, returns the inverted matrix;
		/// otherwise returns the original matrix.</returns>
		public Matrix4 GetInverse()
		{
			Matrix4 result = this;
			result.Inverse();
			return result;
		}

		/// <summary>
		/// Inverts the current instance of <see cref="Matrix4"/> if it is invertible.
		/// </summary>
		/// <param name="result">When the method completes, contains the inverted matrix if the current instance
		/// of <see cref="Matrix4"/> is invertible; otherwise, contains the original matrix.</param>
		public void GetInverse( out Matrix4 result )
		{
			result = this;
			result.Inverse();
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Matrix4"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Matrix4"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Matrix4"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Matrix4 && this == (Matrix4)obj );
		}

		/// <summary>
		/// Determines whether two given matricies are equal.
		/// </summary>
		/// <param name="v1">The first matrix to compare.</param>
		/// <param name="v2">The second matrix to compare.</param>
		/// <returns>True if the matricies are equal; False otherwise.</returns>
		public static bool operator ==( Matrix4 v1, Matrix4 v2 )
		{
			return v1.Item0 == v2.Item0 && v1.Item1 == v2.Item1 && v1.Item2 == v2.Item2 && v1.Item3 == v2.Item3;
		}

		/// <summary>
		/// Determines whether two given matricies are unequal.
		/// </summary>
		/// <param name="v1">The first matrix to compare.</param>
		/// <param name="v2">The second matrix to compare.</param>
		/// <returns>True if the matricies are unequal; False otherwise.</returns>
		public static bool operator !=( Matrix4 v1, Matrix4 v2 )
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
		public unsafe double this[ int row, int column ]
		{
			get
			{
				if( row < 0 || row > 3 )
					throw new ArgumentOutOfRangeException( "row" );
				if( column < 0 || column > 3 )
					throw new ArgumentOutOfRangeException( "column" );
				fixed ( double* v = &this.Item0.X )
					return v[ row * 4 + column ];
			}
			set
			{
				if( row < 0 || row > 3 )
					throw new ArgumentOutOfRangeException( "row" );
				if( column < 0 || column > 3 )
					throw new ArgumentOutOfRangeException( "column" );
				fixed ( double* v = &this.Item0.X )
					v[ row * 4 + column ] = value;
			}
		}

		/// <summary>
		/// Determines whether the specified matrix is equal to the current instance of <see cref="Matrix4"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The matrix to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified matrix is equal to the current instance of <see cref="Matrix4"/>; False otherwise.</returns>
		public bool Equals( Matrix4 v, double epsilon )
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

		//!!!!works right?
		//!!!!not used
		/// <summary>
		/// Creates a look-at matrix.
		/// </summary>
		/// <param name="eye">Eye (camera) position in world space.</param>
		/// <param name="dir">Target position in world space.</param>
		/// <param name="up">Up vector in world space.</param>
		/// <returns>The instance of <see cref="Matrix4"/> that transforms world space to camera space.</returns>
		public static Matrix4 LookAt( Vector3 eye, Vector3 dir, Vector3 up )
		{
			Matrix4 result;
			LookAt( ref eye, ref dir, ref up, out result );
			return result;
		}

		//!!!!works right?
		//!!!!!not used
		/// <summary>
		/// Creates a look-at matrix.
		/// </summary>
		/// <param name="eye">Eye (camera) position in world space.</param>
		/// <param name="dir">Target position in world space.</param>
		/// <param name="up">Up vector in world space.</param>
		/// <param name="result">When the method completes, contains the instance of <see cref="Matrix4"/> that transforms world space to camera space.</param>
		public static void LookAt( ref Vector3 eye, ref Vector3 dir, ref Vector3 up, out Matrix4 result )
		{
			Vector3 z;
			Vector3.Subtract( ref eye, ref dir, out z );
			//Vec3D z = eye - dir;
			z.Normalize();

			Vector3 x;
			Vector3.Cross( ref up, ref z, out x );
			//Vec3D x = Vec3D.Cross( up, z );

			x.Normalize();
			Vector3 y;
			Vector3.Cross( ref z, ref x, out y );
			//Vec3D y = Vec3D.Cross( z, x );
			y.Normalize();

			result.Item0.X = x.X; result.Item1.X = x.Y; result.Item2.X = x.Z; result.Item3.X = 0;
			result.Item0.Y = y.X; result.Item1.Y = y.Y; result.Item2.Y = y.Z; result.Item3.Y = 0;
			result.Item0.Z = z.X; result.Item1.Z = z.Y; result.Item2.Z = z.Z; result.Item3.Z = 0;
			result.Item0.W = 0.0; result.Item1.W = 0.0; result.Item2.W = 0.0; result.Item3.W = 1.0;

			Matrix4 m1 = Matrix4.Identity;
			m1[ 3, 0 ] = -eye.X;
			m1[ 3, 1 ] = -eye.Y;
			m1[ 3, 2 ] = -eye.Z;
			result *= m1;
		}

		//!!!!!not used
		/// <summary>
		/// Creates a perspective projection matrix. 
		/// </summary>
		/// <param name="fov">Angle of the field of view in the y direction.</param>
		/// <param name="aspect">Aspect ratio of the view (width / height).</param>
		/// <param name="znear">Distance to the near clip plane.</param>
		/// <param name="zfar">Distance to the far clip plane.</param>
		/// <returns>A projection matrix that transforms camera space to raster space.</returns>
		public static Matrix4 Perspective( double fov, double aspect, double znear, double zfar )
		{
			Matrix4 result;
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
		//!!!!!not used
		public static void Perspective( double fov, double aspect, double znear, double zfar,
			out Matrix4 result )
		{
			double y = Math.Tan( fov * Math.PI / 360.0 );
			double x = y * aspect;

			result.Item0.X = 1.0 / x;
			result.Item1.X = 0.0;
			result.Item2.X = 0.0;
			result.Item3.X = 0.0;
			result.Item0.Y = 0.0;
			result.Item1.Y = 1.0 / y;
			result.Item2.Y = 0.0;
			result.Item3.Y = 0.0;
			result.Item0.Z = 0.0;
			result.Item1.Z = 0.0;
			result.Item2.Z = -( zfar + znear ) / ( zfar - znear );
			result.Item3.Z = -( 2.0 * zfar * znear ) / ( zfar - znear );
			result.Item0.W = 0.0;
			result.Item1.W = 0.0;
			result.Item2.W = -1.0;
			result.Item3.W = 0.0;
		}

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="translation">The amount to translate in each axis.</param>
		/// <returns>The translation matrix.</returns>
		public static Matrix4 FromTranslate( Vector3 translation )
		{
			Matrix4 result;
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
		public static void FromTranslate( ref Vector3 translation, out Matrix4 result )
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
		/// Converts the current instance of <see cref="Matrix4"/> into the equivalent <see cref="Matrix3"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Matrix3"/> structure.</returns>
		[AutoConvertType]
		public Matrix3 ToMatrix3()
		{
			Matrix3 result;
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
		/// Converts the current instance of <see cref="Matrix4"/> into the equivalent <see cref="Matrix3"/> structure.
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
		/// Converts a string representation of a matrix into the equivalent <see cref="Matrix4"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the matrix (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Matrix4"/> structure.</returns>
		[AutoConvertType]
		public static Matrix4 Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 16 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 16 parts separated by spaces.", text ) );

			try
			{
				return new Matrix4(
					double.Parse( vals[ 0 ] ),
					double.Parse( vals[ 1 ] ),
					double.Parse( vals[ 2 ] ),
					double.Parse( vals[ 3 ] ),
					double.Parse( vals[ 4 ] ),
					double.Parse( vals[ 5 ] ),
					double.Parse( vals[ 6 ] ),
					double.Parse( vals[ 7 ] ),
					double.Parse( vals[ 8 ] ),
					double.Parse( vals[ 9 ] ),
					double.Parse( vals[ 10 ] ),
					double.Parse( vals[ 11 ] ),
					double.Parse( vals[ 12 ] ),
					double.Parse( vals[ 13 ] ),
					double.Parse( vals[ 14 ] ),
					double.Parse( vals[ 15 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Matrix4"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Matrix4"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1} {2} {3}", Item0.ToString(), Item1.ToString(), Item2.ToString(), Item3.ToString() );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix4"/> to the matrix of <see cref="Matrix4F"/> format.
		/// </summary>
		/// <param name="result">When the method completes, contains the matrix of <see cref="Matrix4F"/> format.</param>
		public void ToMatrix4F( out Matrix4F result )
		{
			result.Item0.X = (float)Item0.X;
			result.Item0.Y = (float)Item0.Y;
			result.Item0.Z = (float)Item0.Z;
			result.Item0.W = (float)Item0.W;
			result.Item1.X = (float)Item1.X;
			result.Item1.Y = (float)Item1.Y;
			result.Item1.Z = (float)Item1.Z;
			result.Item1.W = (float)Item1.W;
			result.Item2.X = (float)Item2.X;
			result.Item2.Y = (float)Item2.Y;
			result.Item2.Z = (float)Item2.Z;
			result.Item2.W = (float)Item2.W;
			result.Item3.X = (float)Item3.X;
			result.Item3.Y = (float)Item3.Y;
			result.Item3.Z = (float)Item3.Z;
			result.Item3.W = (float)Item3.W;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Matrix4"/> to the matrix of <see cref="Matrix4F"/> format.
		/// </summary>
		/// <returns>The matrix of <see cref="Matrix4F"/> format.</returns>
		[AutoConvertType]
		public Matrix4F ToMatrix4F()
		{
			ToMatrix4F( out var result );
			return result;
		}

		/// <summary>
		/// Returns the translation of the current instance of <see cref="Matrix4"/>.
		/// </summary>
		/// <returns>The translation of the current instance of <see cref="Matrix4"/>.</returns>
		public Vector3 GetTranslation()
		{
			return Item3.ToVector3();
		}

		/// <summary>
		/// Returns the translation of the current instance of <see cref="Matrix4"/>.
		/// </summary>
		/// <param name="result">When the method completes, contains the translation of the current instance of <see cref="Matrix4"/>.</param>
		public void GetTranslation( out Vector3 result )
		{
			result = Item3.ToVector3();
		}

		/// <summary>
		/// Sets the translation of the current instance of <see cref="Matrix4"/>.
		/// </summary>
		/// <param name="value">The translation to set.</param>
		public void SetTranslation( Vector3 value )
		{
			Item3 = new Vector4( value, Item3.W );
		}

		/// <summary>
		/// Sets the translation of the current instance of <see cref="Matrix4"/>.
		/// </summary>
		/// <param name="value">The translation to set.</param>
		public void SetTranslation( ref Vector3 value )
		{
			Item3 = new Vector4( value, Item3.W );
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
		public bool Decompose( out Vector3 translation, out Matrix3 rotation, out Vector3 scale )
		{
			translation = GetTranslation();

			ToMatrix3( out Matrix3 rotationMat3 );
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
		public bool Decompose( out Vector3 translation, out Quaternion rotation, out Vector3 scale )
		{
			if( !Decompose( out translation, out Matrix3 rotationMat3, out scale ) )
			{
				rotation = Quaternion.Identity;
				return false;
			}
			rotationMat3.ToQuaternion( out rotation );
			return true;
		}
	}
}
