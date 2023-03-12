// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<PlaneF> ) )]
	/// <summary>
	/// Represents a single precision plane in three-dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct PlaneF
	{
		/// <summary>
		/// The A component of the plane.
		/// </summary>
		public float A;
		/// <summary>
		/// The B component of the plane.
		/// </summary>
		public float B;
		/// <summary>
		/// The C component of the plane.
		/// </summary>
		public float C;
		/// <summary>
		/// The D component of the plane.
		/// </summary>
		public float D;

		/// <summary>
		/// Determines from which side of the plane the object can be.
		/// </summary>
		public enum Side
		{
			Negative,
			No,
			Positive,
		}

		/// <summary>
		/// Returns the plane with all of its components set to zero.
		/// </summary>
		public static readonly PlaneF Zero = new PlaneF( 0.0f, 0.0f, 0.0f, 0.0f );

		/// <summary>
		/// Constructs a plane with another specified <see cref="PlaneF"/> object.
		/// </summary>
		/// <param name="source">A specified plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public PlaneF( PlaneF source )
		{
			this.A = source.A;
			this.B = source.B;
			this.C = source.C;
			this.D = source.D;
		}

		/// <summary>
		/// Constructs a plane with the given A, B, C and D components.
		/// </summary>
		/// <param name="a">The A component of the plane.</param>
		/// <param name="b">The B component of the plane.</param>
		/// <param name="c">The C component of the plane.</param>
		/// <param name="d">The D component of the plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public PlaneF( float a, float b, float c, float d )
		{
			this.A = a;
			this.B = b;
			this.C = c;
			this.D = d;
		}

		/// <summary>
		/// Constructs a plane with the given normal and the distance along this normal from the origin.
		/// </summary>
		/// <param name="normal">The normal vector of the plane.</param>
		/// <param name="distance">The distance of the plane along its normal from the origin.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public PlaneF( Vector3F normal, float distance )
		{
			A = normal.X;
			B = normal.Y;
			C = normal.Z;
			D = -distance;
		}

		/// <summary>
		/// Constructs a plane from the given <see cref="Vector4F"/> object.
		/// </summary>
		/// <param name="source">A specified vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public PlaneF( Vector4F source )
		{
			A = source.X;
			B = source.Y;
			C = source.Z;
			D = source.W;
		}

		/// <summary>
		/// Converts a string representation of a plane into the equivalent <see cref="PlaneF"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the plane (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="PlaneF"/> structure.</returns>
		[AutoConvertType]
		public static PlaneF Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (x y z w).", text ) );

			try
			{
				return new PlaneF(
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
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="PlaneF"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="PlaneF"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 8 );
			//return string.Format( "{0} {1} {2} {3}", a, b, c, d );
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="PlaneF"/> with a given precision.
		/// </summary>
		/// <param name="precision">The precision value.</param>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="PlaneF"/>.</returns>
		public string ToString( int precision )
		{
			string format = "";

			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "}";
			return string.Format( format, A, B, C, D );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="PlaneF"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="PlaneF"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="PlaneF"/>; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is PlaneF && this == (PlaneF)obj );
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode() ^ D.GetHashCode() );
		}

		/// <summary>
		/// Adds two planes.
		/// </summary>
		/// <param name="p0">The first plane to add.</param>
		/// <param name="p1">The second plane to add.</param>
		/// <returns>The resulting plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static PlaneF operator +( PlaneF p0, PlaneF p1 )
		{
			PlaneF result;
			result.A = p0.A + p1.A;
			result.B = p0.B + p1.B;
			result.C = p0.C + p1.C;
			result.D = p0.D + p1.D;
			return result;
		}

		/// <summary>
		/// Subtracts two planes.
		/// </summary>
		/// <param name="p0">The plane to subtract from.</param>
		/// <param name="p1">The plane to be subtracted from another plane.</param>
		/// <returns>The resulting plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static PlaneF operator -( PlaneF p0, PlaneF p1 )
		{
			PlaneF result;
			result.A = p0.A - p1.A;
			result.B = p0.B - p1.B;
			result.C = p0.C - p1.C;
			result.D = p0.D - p1.D;
			return result;
		}

		/// <summary>
		/// Reverses the direction of a given plane. 
		/// </summary>
		/// <param name="p">The plane to negate.</param>
		/// <returns>A plane facing in the opposite direction.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static PlaneF operator -( PlaneF p )
		{
			PlaneF result;
			result.A = -p.A;
			result.B = -p.B;
			result.C = -p.C;
			result.D = -p.D;
			return result;
		}

		/// <summary>
		/// Adds two planes.
		/// </summary>
		/// <param name="p0">The first plane to add.</param>
		/// <param name="p1">The second plane to add.</param>
		/// <param name="result">When the method completes, contains the resulting plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref PlaneF p0, ref PlaneF p1, out PlaneF result )
		{
			result.A = p0.A + p1.A;
			result.B = p0.B + p1.B;
			result.C = p0.C + p1.C;
			result.D = p0.D + p1.D;
		}

		/// <summary>
		/// Subtracts two planes.
		/// </summary>
		/// <param name="p0">The plane to subtract from.</param>
		/// <param name="p1">The plane to be subtracted from another plane.</param>
		/// <param name="result">When the method completes, contains the resulting plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref PlaneF p0, ref PlaneF p1, out PlaneF result )
		{
			result.A = p0.A - p1.A;
			result.B = p0.B - p1.B;
			result.C = p0.C - p1.C;
			result.D = p0.D - p1.D;
		}

		/// <summary>
		/// Reverses the direction of a given plane.
		/// </summary>
		/// <param name="p">The plane to negate.</param>
		/// <param name="result">When the method completes, contains the plane facing in the opposite direction.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Negate( ref PlaneF p, out PlaneF result )
		{
			result.A = -p.A;
			result.B = -p.B;
			result.C = -p.C;
			result.D = -p.D;
		}

		//public static PlaneF Add( PlaneF p0, PlaneF p1 )
		//{
		//	PlaneF result;
		//	result.a = p0.a + p1.a;
		//	result.b = p0.b + p1.b;
		//	result.c = p0.c + p1.c;
		//	result.d = p0.d + p1.d;
		//	return result;
		//}

		//public static PlaneF Subtract( PlaneF p0, PlaneF p1 )
		//{
		//	PlaneF result;
		//	result.a = p0.a - p1.a;
		//	result.b = p0.b - p1.b;
		//	result.c = p0.c - p1.c;
		//	result.d = p0.d - p1.d;
		//	return result;
		//}

		//public static PlaneF Negate( PlaneF p )
		//{
		//	PlaneF result;
		//	result.a = -p.a;
		//	result.b = -p.b;
		//	result.c = -p.c;
		//	result.d = -p.d;
		//	return result;
		//}

		/// <summary>
		/// Determines whether two given planes are equal.
		/// </summary>
		/// <param name="p0">The first plane to compare.</param>
		/// <param name="p1">The second plane to compare.</param>
		/// <returns>True if the planes are equal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( PlaneF p0, PlaneF p1 )
		{
			return ( p0.A == p1.A && p0.B == p1.B && p0.C == p1.C && p0.D == p1.D );
		}

		/// <summary>
		/// Determines whether two given planes are unequal.
		/// </summary>
		/// <param name="p0">The first plane to compare.</param>
		/// <param name="p1">The second plane to compare.</param>
		/// <returns>True if the planes are unequal; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( PlaneF p0, PlaneF p1 )
		{
			return ( p0.A != p1.A || p0.B != p1.B || p0.C != p1.C || p0.D != p1.D );
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the A, B, C, or D component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the A component, 1 for the B component, 2 for the C component, and 3 for the D component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
		public unsafe float this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( float* v = &this.A )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( float* v = &this.A )
				{
					v[ index ] = value;
				}
			}
		}

		/// <summary>
		/// Determines whether the specified plane is equal to the current instance of <see cref="PlaneF"/>
		/// with a given precision.
		/// </summary>
		/// <param name="p">The plane to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified plane is equal to the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( PlaneF p, float epsilon )
		{
			if( Math.Abs( A - p.A ) > epsilon )
				return false;
			if( Math.Abs( B - p.B ) > epsilon )
				return false;
			if( Math.Abs( C - p.C ) > epsilon )
				return false;
			if( Math.Abs( D - p.D ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified plane is equal to the current instance of <see cref="PlaneF"/>
		/// with a given precision.
		/// </summary>
		/// <param name="p">The plane to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified plane is equal to the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref PlaneF p, float epsilon )
		{
			if( Math.Abs( A - p.A ) > epsilon )
				return false;
			if( Math.Abs( B - p.B ) > epsilon )
				return false;
			if( Math.Abs( C - p.C ) > epsilon )
				return false;
			if( Math.Abs( D - p.D ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified plane is equal to the current instance of <see cref="PlaneF"/>
		/// with the given normal and distance precisions.
		/// </summary>
		/// <param name="p">The plane to compare.</param>
		/// <param name="normalEpsilon">The precision value for the plane normal.</param>
		/// <param name="distanceEpsilon">The precision value for the distance component of the plane.</param>
		/// <returns>True if the specified plane is equal to the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( PlaneF p, float normalEpsilon, float distanceEpsilon )
		{
			if( Math.Abs( D - p.D ) > distanceEpsilon )
				return false;
			Vector3F n = Normal;
			Vector3F pn = p.Normal;
			if( !n.Equals( ref pn, normalEpsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified plane is equal to the current instance of <see cref="PlaneF"/>
		/// with the given normal and distance precisions.
		/// </summary>
		/// <param name="p">The plane to compare.</param>
		/// <param name="normalEpsilon">The precision value for the plane normal.</param>
		/// <param name="distanceEpsilon">The precision value for the distance component of the plane.</param>
		/// <returns>True if the specified plane is equal to the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref PlaneF p, float normalEpsilon, float distanceEpsilon )
		{
			if( Math.Abs( D - p.D ) > distanceEpsilon )
				return false;
			Vector3F n = Normal;
			Vector3F pn = p.Normal;
			if( !n.Equals( ref pn, normalEpsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// The normal vector of the plane.
		/// </summary>
		public Vector3F Normal
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector3F result;
				result.X = A;
				result.Y = B;
				result.Z = C;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { A = value.X; B = value.Y; C = value.Z; }
		}

		/// <summary>
		/// The distance of the plane along its normal from the origin.
		/// </summary>
		public float Distance
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return -D; }
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { D = -value; }
		}

		/// <summary>
		/// Converts a normal vector of the current instance of <see cref="PlaneF"/> into a unit vector.
		/// </summary>
		/// <returns>The length of the plane normal vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float Normalize()
		{
			Vector3F normal = Normal;
			float length = normal.Normalize();
			Normal = normal;
			return length;
		}

		/// <summary>
		/// Converts a normal vector of the current instance of <see cref="PlaneF"/> into a unit vector and returns the resulting plane.
		/// </summary>
		/// <returns>The plane with normalized normal vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public PlaneF GetNormalize()
		{
			PlaneF result;
			Vector3F normal = Normal;
			normal.Normalize();
			result.A = normal.X;
			result.B = normal.Y;
			result.C = normal.Z;
			result.D = D;
			return result;
		}

		/// <summary>
		/// Calculates the dot product of a specified vector and the normal of the plane plus the distance value of the plane.
		/// </summary>
		/// <param name="v">A specified vector.</param>
		/// <returns>The dot product of a specified vector and the normal of the plane plus the distance value of the plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetDistance( Vector3F v )
		{
			return A * v.X + B * v.Y + C * v.Z + D;
		}

		/// <summary>
		/// Calculates the dot product of a specified vector and the normal of the plane plus the distance value of the plane.
		/// </summary>
		/// <param name="v">A specified vector.</param>
		/// <returns>The dot product of a specified vector and the normal of the plane plus the distance value of the plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetDistance( ref Vector3F v )
		{
			return A * v.X + B * v.Y + C * v.Z + D;
		}

		/// <summary>
		/// Determines from which side of the plane the point is on.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <returns>The resulting side of the plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( Vector3F point )
		{
			float distance = GetDistance( ref point );
			if( distance > 0.0f )
				return Side.Positive;
			if( distance < 0.0f )
				return Side.Negative;
			return Side.No;
		}

		/// <summary>
		/// Determines from which side of the plane the point is on.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <returns>The resulting side of the plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( ref Vector3F point )
		{
			float distance = GetDistance( ref point );
			if( distance > 0.0f )
				return Side.Positive;
			if( distance < 0.0f )
				return Side.Negative;
			return Side.No;
		}

		/// <summary>
		/// Determines from which side of the plane the point is on (with the given precision).
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <param name="epsilon">The given precision.</param>
		/// <returns>The resulting side of the plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( Vector3F point, float epsilon )
		{
			float distance = GetDistance( ref point );
			if( distance > epsilon )
				return Side.Positive;
			if( distance < -epsilon )
				return Side.Negative;
			return Side.No;
		}

		/// <summary>
		/// Determines from which side of the plane the point is on (with the given precision).
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <param name="epsilon">The given precision.</param>
		/// <returns>The resulting side of the plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( ref Vector3F point, float epsilon )
		{
			float distance = GetDistance( ref point );
			if( distance > epsilon )
				return Side.Positive;
			if( distance < -epsilon )
				return Side.Negative;
			return Side.No;
		}

		/// <summary>
		/// Converts the current instance of <see cref="PlaneF"/> into an instance of <see cref="Vector4F"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector4F ToVector4F()
		{
			Vector4F result;
			result.X = A;
			result.Y = B;
			result.Z = C;
			result.W = D;
			return result;
		}

		/// <summary>
		/// Creates an instance of <see cref="PlaneF"/> that contains the three given points. 
		/// </summary>
		/// <param name="point0">The first point defining the plane.</param>
		/// <param name="point1">The second point defining the plane.</param>
		/// <param name="point2">The third point defining the plane.</param>
		/// <returns>The resulting plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static PlaneF FromPoints( Vector3F point0, Vector3F point1, Vector3F point2 )
		{
			PlaneF result;
			FromPoints( ref point0, ref point1, ref point2, out result );
			return result;
			//Vec3 edge1 = point1 - point0;
			//Vec3 edge2 = point2 - point0;
			//Vec3 normal = Vec3.Cross( edge1, edge2 ).GetNormalize();
			//return new Plane( normal, Vec3.Dot( normal, point0 ) );
		}

		/// <summary>
		/// Creates an instance of <see cref="PlaneF"/> that contains the three given points. 
		/// </summary>
		/// <param name="point0">The first point defining the plane.</param>
		/// <param name="point1">The second point defining the plane.</param>
		/// <param name="point2">The third point defining the plane.</param>
		/// <param name="result">When the method completes, contains the resulting plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromPoints( ref Vector3F point0, ref Vector3F point1, ref Vector3F point2,
			out PlaneF result )
		{
			Vector3F edge1;
			Vector3F.Subtract( ref point1, ref point0, out edge1 );
			Vector3F edge2;
			Vector3F.Subtract( ref point2, ref point0, out edge2 );

			Vector3F normal;
			Vector3F.Cross( ref edge1, ref edge2, out normal );
			normal.Normalize();
			result.A = normal.X;
			result.B = normal.Y;
			result.C = normal.Z;
			result.D = -( normal.X * point0.X + normal.Y * point0.Y + normal.Z * point0.Z );
		}

		/// <summary>
		/// Creates an instance of <see cref="PlaneF"/> with point and two direction vectors.
		/// </summary>
		/// <param name="dir1">The first direction vector.</param>
		/// <param name="dir2">The second direction vector.</param>
		/// <param name="p">The point that is the start of direction vectors.</param>
		/// <returns>The resulting plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static PlaneF FromVectors( Vector3F dir1, Vector3F dir2, Vector3F p )
		{
			PlaneF result;
			Vector3F normal;
			Vector3F.Cross( ref dir1, ref dir2, out normal );
			normal.Normalize();
			result.A = normal.X;
			result.B = normal.Y;
			result.C = normal.Z;
			result.D = -( normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z );
			return result;
			//Vec3 normal = Vec3.Cross( dir1, dir2 ).GetNormalize();
			//return new Plane( normal, normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z );
		}

		/// <summary>
		/// Creates an instance of <see cref="PlaneF"/> with point and two direction vectors.
		/// </summary>
		/// <param name="dir1">The first direction vector.</param>
		/// <param name="dir2">The second direction vector.</param>
		/// <param name="p">The point that is the start of direction vectors.</param>
		/// <param name="result">When the method completes, contains the resulting plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromVectors( ref Vector3F dir1, ref Vector3F dir2, ref Vector3F p, out PlaneF result )
		{
			Vector3F normal;
			Vector3F.Cross( ref dir1, ref dir2, out normal );
			normal.Normalize();
			result.A = normal.X;
			result.B = normal.Y;
			result.C = normal.Z;
			result.D = -( normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z );
		}

		/// <summary>
		/// Creates an instance of <see cref="PlaneF"/> with the normal and the point.
		/// </summary>
		/// <param name="point">Any point that lies along the plane.</param>
		/// <param name="normal">The normal vector to the plane.</param>
		/// <returns>The resulting plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static PlaneF FromPointAndNormal( Vector3F point, Vector3F normal )
		{
			float distance = Vector3F.Dot( ref point, ref normal );
			return new PlaneF( normal, distance );
		}

		/// <summary>
		/// Creates an instance of <see cref="PlaneF"/> with the normal and the point.
		/// </summary>
		/// <param name="point">Any point that lies along the plane.</param>
		/// <param name="normal">The normal vector to the plane.</param>
		/// <param name="plane">When the method completes, contains the resulting plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromPointAndNormal( ref Vector3F point, ref Vector3F normal, out PlaneF plane )
		{
			float distance = Vector3F.Dot( ref point, ref normal );
			plane = new PlaneF( normal, distance );
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="PlaneF"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <param name="scale">When the method completes, contains the ray and plane intersection.</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref RayF ray, out float scale )
		{
			float d1 = A * ray.Origin.X + B * ray.Origin.Y + C * ray.Origin.Z + D;
			float d2 = A * ray.Direction.X + B * ray.Direction.Y + C * ray.Direction.Z;
			if( d2 == 0.0f )
			{
				scale = 0;
				return false;
			}
			scale = -( d1 / d2 );
			return true;
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="PlaneF"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <param name="scale">When the method completes, contains the ray and plane intersection.</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( RayF ray, out float scale )
		{
			float d1 = A * ray.Origin.X + B * ray.Origin.Y + C * ray.Origin.Z + D;
			float d2 = A * ray.Direction.X + B * ray.Direction.Y + C * ray.Direction.Z;
			if( d2 == 0.0f )
			{
				scale = 0;
				return false;
			}
			scale = -( d1 / d2 );
			return true;
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="PlaneF"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <param name="intersectionPoint">The resulting point of intersection of the plane and the ray (if they intersected).</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref RayF ray, out Vector3F intersectionPoint )
		{
			float d1 = A * ray.Origin.X + B * ray.Origin.Y + C * ray.Origin.Z + D;
			float d2 = A * ray.Direction.X + B * ray.Direction.Y + C * ray.Direction.Z;
			if( d2 == 0.0f )
			{
				intersectionPoint = Vector3F.Zero;
				return false;
			}
			float scale = -( d1 / d2 );
			intersectionPoint = ray.GetPointOnRay( scale );
			return true;
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="PlaneF"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <param name="intersectionPoint">The resulting point of intersection of the plane and the ray (if they intersected).</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( RayF ray, out Vector3F intersectionPoint )
		{
			float d1 = A * ray.Origin.X + B * ray.Origin.Y + C * ray.Origin.Z + D;
			float d2 = A * ray.Direction.X + B * ray.Direction.Y + C * ray.Direction.Z;
			if( d2 == 0.0f )
			{
				intersectionPoint = Vector3F.Zero;
				return false;
			}
			float scale = -( d1 / d2 );
			intersectionPoint = ray.GetPointOnRay( scale );
			return true;
		}

		/// <summary>
		/// Determines whether the given line intersects the current instance of <see cref="PlaneF"/>.
		/// </summary>
		/// <param name="line">The line to check.</param>
		/// <param name="scale">When the method completes, contains the line and plane intersection.</param>
		/// <returns>True if the given line intersects the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref Line3F line, out float scale )
		{
			scale = 0;
			float d1 = A * line.Start.X + B * line.Start.Y + C * line.Start.Z + D;
			float d2 = A * line.End.X + B * line.End.Y + C * line.End.Z + D;
			if( d1 == d2 )
				return false;
			if( d1 > 0.0f && d2 > 0.0f )
				return false;
			if( d1 < 0.0f && d2 < 0.0f )
				return false;
			float fraction = ( d1 / ( d1 - d2 ) );
			if( fraction < 0.0f || fraction > 1.0f )
				return false;
			scale = fraction;
			return true;
		}

		/// <summary>
		/// Determines whether the given line intersects the current instance of <see cref="PlaneF"/>.
		/// </summary>
		/// <param name="line">The line to check.</param>
		/// <param name="scale">When the method completes, contains the line and plane intersection.</param>
		/// <returns>True if the given line intersects the current instance of <see cref="PlaneF"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Line3F line, out float scale )
		{
			return Intersects( ref line, out scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Line3F line )
		{
			float scale;
			return Intersects( line, out scale );
		}

		/// <summary>
		/// Returns side of the plane that the given box lies on.
		/// The box is defined as centre/half-size pairs for effectively.
		/// </summary>
		/// <param name="boundsCenter">The box center.</param>
		/// <param name="boundsHalfSize">The box extents.</param>
		/// <returns>The resulting side.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( ref Vector3F boundsCenter, ref Vector3F boundsHalfSize )
		{
			// Calculate the distance between box centre and the plane
			float distance = A * boundsCenter.X + B * boundsCenter.Y + C * boundsCenter.Z + D;
			//float distance = GetDistance( ref boundsCenter );

			// Calculate the maximise allows absolute distance for
			// the distance between box centre and plane
			float maxAbsDist =
				Math.Abs( A * boundsHalfSize.X ) +
				Math.Abs( B * boundsHalfSize.Y ) +
				Math.Abs( C * boundsHalfSize.Z );
			//float maxAbsDist = normal.absDotProduct( halfSize );

			if( distance < -maxAbsDist )
				return Side.Negative;
			if( distance > maxAbsDist )
				return Side.Positive;
			return Side.No;
		}

		/// <summary>
		/// Returns side of the plane that the given box lies on.
		/// The box is defined as centre/half-size pairs for effectively.
		/// </summary>
		/// <param name="boundsCenter">The box center.</param>
		/// <param name="boundsHalfSize">The box extents.</param>
		/// <returns>The resulting side.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( Vector3F boundsCenter, Vector3F boundsHalfSize )
		{
			return GetSide( ref boundsCenter, ref boundsHalfSize );
		}

		/// <summary>
		/// Returns side of the plane that the given box lies on.
		/// The box is defined as bounds.
		/// </summary>
		/// <param name="bounds">The given bounds.</param>
		/// <returns>The resulting side.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( ref BoundsF bounds )
		{
			Vector3F boundsCenter;
			bounds.GetCenter( out boundsCenter );

			//Vec3 boundsHalfSize = boundsCenter - bounds.Minimum;
			Vector3F boundsHalfSize;
			Vector3F.Subtract( ref boundsCenter, ref bounds.Minimum, out boundsHalfSize );

			return GetSide( ref boundsCenter, ref boundsHalfSize );
		}

		/// <summary>
		/// Returns side of the plane that the given box lies on.
		/// The box is defined as bounds.
		/// </summary>
		/// <param name="bounds">The given bounds.</param>
		/// <returns>The resulting side.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( BoundsF bounds )
		{
			return GetSide( ref bounds );
		}

		/// <summary>
		/// Converts the current instance of <see cref="PlaneF"/> to the plane of <see cref="Plane"/> format.
		/// </summary>
		/// <returns>The plane of <see cref="Plane"/> format.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Plane ToPlane()
		{
			Plane result;
			result.A = A;
			result.B = B;
			result.C = C;
			result.D = D;
			return result;
		}

#if !DISABLE_IMPLICIT
		/// <summary>
		/// Implicit conversion from <see cref="PlaneF"/> type to <see cref="Plane"/> type for the given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Plane( PlaneF v )
		{
			return new Plane( v );
		}
#endif
	}
}