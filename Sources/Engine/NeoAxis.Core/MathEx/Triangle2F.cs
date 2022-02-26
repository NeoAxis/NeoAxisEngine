// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a single precision triangle in three-dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Triangle2F
	{
		/// <summary>
		/// The A corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector2F A;
		/// <summary>
		/// The B corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector2F B;
		/// <summary>
		/// The C corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector2F C;

		/// <summary>
		/// Constructs a triangle with the given A, B and C components.
		/// </summary>
		/// <param name="a">The A component of the triangle.</param>
		/// <param name="b">The B component of the triangle.</param>
		/// <param name="c">The C component of the triangle.</param>
		public Triangle2F( Vector2F a, Vector2F b, Vector2F c )
		{
			this.A = a;
			this.B = b;
			this.C = c;
		}

		/// <summary>
		/// Constructs a triangle with another specified <see cref="Triangle"/> object.
		/// </summary>
		/// <param name="source">The triangle of <see cref="Triangle"/> format.</param>
		public Triangle2F( Triangle2 source )
		{
			this.A = source.A.ToVector2F();
			this.B = source.B.ToVector2F();
			this.C = source.C.ToVector2F();
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the A, B, or C component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the A component, 1 for the B component, and 2 for the C component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
		public unsafe Vector2F this[ int index ]
		{
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector2F* v = &this.A )
					return v[ index ];
			}
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector2F* v = &this.A )
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
		/// Determines whether the specified triangle is equal to the current instance of <see cref="Triangle2F"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The triangle to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified triangle is equal to the current instance of <see cref="Triangle2F"/>; False otherwise.</returns>
		public bool Equals( Triangle2F v, float epsilon )
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
		/// Determines whether the specified triangle is equal to the current instance of <see cref="Triangle2F"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The triangle to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified triangle is equal to the current instance of <see cref="Triangle2F"/>; False otherwise.</returns>
		public bool Equals( ref Triangle2F v, float epsilon )
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
		/// Determines whether the specified object is equal to the current instance of <see cref="Triangle2F"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Triangle2F"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Triangle2F"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Triangle2F && this == (Triangle2F)obj );
		}

		/// <summary>
		/// Determines whether two given triangles are equal.
		/// </summary>
		/// <param name="v1">The first triangle to compare.</param>
		/// <param name="v2">The second triangle to compare.</param>
		/// <returns>True if the triangles are equal; False otherwise.</returns>
		public static bool operator ==( Triangle2F v1, Triangle2F v2 )
		{
			return v1.A == v2.A && v1.B == v2.B && v1.C == v2.C;
		}

		/// <summary>
		/// Determines whether two given triangles are unequal.
		/// </summary>
		/// <param name="v1">The first triangle to compare.</param>
		/// <param name="v2">The second triangle to compare.</param>
		/// <returns>True if the triangles are unequal; False otherwise.</returns>
		public static bool operator !=( Triangle2F v1, Triangle2F v2 )
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
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Triangle2F"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Triangle2F"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0}; {1}; {2}", A.ToString(), B.ToString(), C.ToString() );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Triangle2F"/> to the triangle of <see cref="Triangle"/> format.
		/// </summary>
		/// <returns>The triangle of <see cref="Triangle"/> format.</returns>
		[AutoConvertType]
		public Triangle2 ToTriangle()
		{
			Triangle2 result;
			result.A.X = A.X;
			result.A.Y = A.Y;
			result.B.X = B.X;
			result.B.Y = B.Y;
			result.C.X = C.X;
			result.C.Y = C.Y;
			return result;
		}
	}
}
