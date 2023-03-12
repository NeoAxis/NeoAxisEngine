// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a double precision triangle in two-dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Triangle2
	{
		/// <summary>
		/// The A corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector2 A;
		/// <summary>
		/// The B corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector2 B;
		/// <summary>
		/// The C corner point of the triangle.
		/// </summary>
		[Browsable( false )]
		public Vector2 C;

		/// <summary>
		/// Constructs a triangle with the given A, B and C components.
		/// </summary>
		/// <param name="a">The A component of the triangle.</param>
		/// <param name="b">The B component of the triangle.</param>
		/// <param name="c">The C component of the triangle.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Triangle2( Vector2 a, Vector2 b, Vector2 c )
		{
			this.A = a;
			this.B = b;
			this.C = c;
		}

		/// <summary>
		/// Constructs a triangle with another specified <see cref="Triangle2F"/> object.
		/// </summary>
		/// <param name="source">The triangle of <see cref="Triangle2F"/> format.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Triangle2( Triangle2F source )
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
		public unsafe Vector2 this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector2* v = &this.A )
					return v[ index ];
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector2* v = &this.A )
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
			return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified triangle is equal to the current instance of <see cref="Triangle2"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The triangle to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified triangle is equal to the current instance of <see cref="Triangle2"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Triangle2 v, double epsilon )
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
		/// Determines whether the specified triangle is equal to the current instance of <see cref="Triangle2"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The triangle to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified triangle is equal to the current instance of <see cref="Triangle2"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref Triangle2 v, double epsilon )
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
		/// Determines whether the specified object is equal to the current instance of <see cref="Triangle2"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Triangle2"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Triangle2"/>; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Triangle2 && this == (Triangle2)obj );
		}

		/// <summary>
		/// Determines whether two given triangles are equal.
		/// </summary>
		/// <param name="v1">The first triangle to compare.</param>
		/// <param name="v2">The second triangle to compare.</param>
		/// <returns>True if the triangles are equal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Triangle2 v1, Triangle2 v2 )
		{
			return v1.A == v2.A && v1.B == v2.B && v1.C == v2.C;
		}

		/// <summary>
		/// Determines whether two given triangles are unequal.
		/// </summary>
		/// <param name="v1">The first triangle to compare.</param>
		/// <param name="v2">The second triangle to compare.</param>
		/// <returns>True if the triangles are unequal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Triangle2 v1, Triangle2 v2 )
		{
			return v1.A != v2.A || v1.B != v2.B || v1.C != v2.C;
		}

		//[AutoConvertType]
		//public static Triangle2 Parse( string text )
		//{
		//	if( string.IsNullOrEmpty( text ) )
		//		throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

		//	string[] vals = text.Split( new char[] { ' ' },
		//		StringSplitOptions.RemoveEmptyEntries );

		//	if( vals.Length != 9 )
		//		throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 9 parts separated by spaces.", text ) );

		//	try
		//	{
		//		return new Triangle2(
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
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Triangle2"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Triangle2"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0}; {1}; {2}", A.ToString(), B.ToString(), C.ToString() );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Triangle2"/> to the triangle of <see cref="Triangle2F"/> format.
		/// </summary>
		/// <returns>The triangle of <see cref="Triangle2F"/> format.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Triangle2F ToTriangle2F()
		{
			Triangle2F result;
			result.A.X = (float)A.X;
			result.A.Y = (float)A.Y;
			result.B.X = (float)B.X;
			result.B.Y = (float)B.Y;
			result.C.X = (float)C.X;
			result.C.Y = (float)C.Y;
			return result;
		}
	}
}
