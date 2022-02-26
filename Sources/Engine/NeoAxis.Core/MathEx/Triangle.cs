// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a double precision triangle in three-dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Triangle
	{
		/// <summary>
		/// The A corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector3 A;
		/// <summary>
		/// The B corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector3 B;
		/// <summary>
		/// The C corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector3 C;

		/// <summary>
		/// Constructs a triangle with the given A, B and C components.
		/// </summary>
		/// <param name="a">The A component of the triangle.</param>
		/// <param name="b">The B component of the triangle.</param>
		/// <param name="c">The C component of the triangle.</param>
		public Triangle( Vector3 a, Vector3 b, Vector3 c )
		{
			this.A = a;
			this.B = b;
			this.C = c;
		}

		/// <summary>
		/// Constructs a triangle with another specified <see cref="TriangleF"/> object.
		/// </summary>
		/// <param name="source">The triangle of <see cref="TriangleF"/> format.</param>
		public Triangle( TriangleF source )
		{
			this.A = source.A;
			this.B = source.B;
			this.C = source.C;
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the A, B, or C component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the A component, 1 for the B component, and 2 for the C component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
		public unsafe Vector3 this[ int index ]
		{
			get
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3* v = &this.A )
					return v[ index ];
			}
			set
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3* v = &this.A )
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
		/// Determines whether the specified triangle is equal to the current instance of <see cref="Triangle"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The triangle to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified triangle is equal to the current instance of <see cref="Triangle"/>; False otherwise.</returns>
		public bool Equals( Triangle v, double epsilon )
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
		/// Determines whether the specified triangle is equal to the current instance of <see cref="Triangle"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The triangle to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified triangle is equal to the current instance of <see cref="Triangle"/>; False otherwise.</returns>
		public bool Equals( ref Triangle v, double epsilon )
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
		/// Determines whether the specified object is equal to the current instance of <see cref="Triangle"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Triangle"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Triangle"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Triangle && this == (Triangle)obj );
		}

		/// <summary>
		/// Determines whether two given triangles are equal.
		/// </summary>
		/// <param name="v1">The first triangle to compare.</param>
		/// <param name="v2">The second triangle to compare.</param>
		/// <returns>True if the triangles are equal; False otherwise.</returns>
		public static bool operator ==( Triangle v1, Triangle v2 )
		{
			return v1.A == v2.A && v1.B == v2.B && v1.C == v2.C;
		}

		/// <summary>
		/// Determines whether two given triangles are unequal.
		/// </summary>
		/// <param name="v1">The first triangle to compare.</param>
		/// <param name="v2">The second triangle to compare.</param>
		/// <returns>True if the triangles are unequal; False otherwise.</returns>
		public static bool operator !=( Triangle v1, Triangle v2 )
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
		//			double.Parse( vals[ 0 ] ),
		//			double.Parse( vals[ 1 ] ),
		//			double.Parse( vals[ 2 ] ),
		//			double.Parse( vals[ 3 ] ),
		//			double.Parse( vals[ 4 ] ),
		//			double.Parse( vals[ 5 ] ),
		//			double.Parse( vals[ 6 ] ),
		//			double.Parse( vals[ 7 ] ),
		//			double.Parse( vals[ 8 ] ) );
		//	}
		//	catch( Exception )
		//	{
		//		throw new FormatException( "The parts of the vectors must be decimal numbers." );
		//	}
		//}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Triangle"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Triangle"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0}; {1}; {2}", A.ToString(), B.ToString(), C.ToString() );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Triangle"/> to the triangle of <see cref="TriangleF"/> format.
		/// </summary>
		/// <returns>The triangle of <see cref="TriangleF"/> format.</returns>
		[AutoConvertType]
		public TriangleF ToTriangleF()
		{
			TriangleF result;
			result.A.X = (float)A.X;
			result.A.Y = (float)A.Y;
			result.A.Z = (float)A.Z;
			result.B.X = (float)B.X;
			result.B.Y = (float)B.Y;
			result.B.Z = (float)B.Z;
			result.C.X = (float)C.X;
			result.C.Y = (float)C.Y;
			result.C.Z = (float)C.Z;
			return result;
		}
	}
}
