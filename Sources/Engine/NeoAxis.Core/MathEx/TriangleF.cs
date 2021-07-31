// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a single precision triangle in three-dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct TriangleF
	{
		/// <summary>
		/// The A corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector3F A;
		/// <summary>
		/// The B corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector3F B;
		/// <summary>
		/// The C corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector3F C;

		/// <summary>
		/// Constructs a triangle with the given A, B and C components.
		/// </summary>
		/// <param name="a">The A component of the triangle.</param>
		/// <param name="b">The B component of the triangle.</param>
		/// <param name="c">The C component of the triangle.</param>
		public TriangleF( Vector3F a, Vector3F b, Vector3F c )
		{
			this.A = a;
			this.B = b;
			this.C = c;
		}

		/// <summary>
		/// Constructs a triangle with another specified <see cref="Triangle"/> object.
		/// </summary>
		/// <param name="source">The triangle of <see cref="Triangle"/> format.</param>
		public TriangleF( Triangle source )
		{
			this.A = source.A.ToVector3F();
			this.B = source.B.ToVector3F();
			this.C = source.C.ToVector3F();
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the A, B, or C component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the A component, 1 for the B component, and 2 for the C component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
		public unsafe Vector3F this[ int index ]
		{
			get
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3F* v = &this.A )
					return v[ index ];
			}
			set
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3F* v = &this.A )
					v[ index ] = value;
			}
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified triangle is equal to the current instance of <see cref="TriangleF"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The triangle to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified triangle is equal to the current instance of <see cref="TriangleF"/>; False otherwise.</returns>
		public bool Equals( TriangleF v, float epsilon )
		{
			if( !A.Equals( ref v.A, epsilon ) )
				return false;
			if( !B.Equals( ref v.B, epsilon ) )
				return false;
			if( !C.Equals( ref v.C, epsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified triangle is equal to the current instance of <see cref="TriangleF"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The triangle to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified triangle is equal to the current instance of <see cref="TriangleF"/>; False otherwise.</returns>
		public bool Equals( ref TriangleF v, float epsilon )
		{
			if( !A.Equals( ref v.A, epsilon ) )
				return false;
			if( !B.Equals( ref v.B, epsilon ) )
				return false;
			if( !C.Equals( ref v.C, epsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="TriangleF"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="TriangleF"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="TriangleF"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is TriangleF && this == (TriangleF)obj );
		}

		/// <summary>
		/// Determines whether two given triangles are equal.
		/// </summary>
		/// <param name="v1">The first triangle to compare.</param>
		/// <param name="v2">The second triangle to compare.</param>
		/// <returns>True if the triangles are equal; False otherwise.</returns>
		public static bool operator ==( TriangleF v1, TriangleF v2 )
		{
			return v1.A == v2.A && v1.B == v2.B && v1.C == v2.C;
		}

		/// <summary>
		/// Determines whether two given triangles are unequal.
		/// </summary>
		/// <param name="v1">The first triangle to compare.</param>
		/// <param name="v2">The second triangle to compare.</param>
		/// <returns>True if the triangles are unequal; False otherwise.</returns>
		public static bool operator !=( TriangleF v1, TriangleF v2 )
		{
			return v1.A != v2.A || v1.B != v2.B || v1.C != v2.C;
		}

		//[AutoConvertType]
		//public static Triangle Parse( string text )
		//{
		//	if( string.IsNullOrEmpty( text ) )
		//		throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

		//	string[] vals = text.Split( new char[] { ' ' },
		//		StringSplitOptions.RemoveEmptyEntries );

		//	if( vals.Length != 9 )
		//		throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 9 parts separated by spaces.", text ) );

		//	try
		//	{
		//		return new Triangle(
		//			float.Parse( vals[ 0 ] ),
		//			float.Parse( vals[ 1 ] ),
		//			float.Parse( vals[ 2 ] ),
		//			float.Parse( vals[ 3 ] ),
		//			float.Parse( vals[ 4 ] ),
		//			float.Parse( vals[ 5 ] ),
		//			float.Parse( vals[ 6 ] ),
		//			float.Parse( vals[ 7 ] ),
		//			float.Parse( vals[ 8 ] ) );
		//	}
		//	catch( Exception )
		//	{
		//		throw new FormatException( "The parts of the vectors must be decimal numbers." );
		//	}
		//}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="TriangleF"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="TriangleF"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0}; {1}; {2}", A.ToString(), B.ToString(), C.ToString() );
		}

		/// <summary>
		/// Converts the current instance of <see cref="TriangleF"/> to the triangle of <see cref="Triangle"/> format.
		/// </summary>
		/// <returns>The triangle of <see cref="Triangle"/> format.</returns>
		[AutoConvertType]
		public Triangle ToTriangle()
		{
			Triangle result;
			result.A.X = A.X;
			result.A.Y = A.Y;
			result.A.Z = A.Z;
			result.B.X = B.X;
			result.B.Y = B.Y;
			result.B.Z = B.Z;
			result.C.X = C.X;
			result.C.Y = C.Y;
			result.C.Z = C.Z;
			return result;
		}
	}
}
