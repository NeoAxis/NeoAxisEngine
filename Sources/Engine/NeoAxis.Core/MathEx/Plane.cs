// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Plane> ) )]
	/// <summary>
	/// Represents a double precision plane in three-dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Plane
	{
		/// <summary>
		/// The A component of the plane.
		/// </summary>
		public double A;
		/// <summary>
		/// The B component of the plane.
		/// </summary>
		public double B;
		/// <summary>
		/// The C component of the plane.
		/// </summary>
		public double C;
		/// <summary>
		/// The D component of the plane.
		/// </summary>
		public double D;

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
		public static readonly Plane Zero = new Plane( 0.0, 0.0, 0.0, 0.0 );

		/// <summary>
		/// Constructs a plane with another specified <see cref="Plane"/> object.
		/// </summary>
		/// <param name="source">A specified plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Plane( Plane source )
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
		public Plane( double a, double b, double c, double d )
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
		public Plane( Vector3 normal, double distance )
		{
			A = normal.X;
			B = normal.Y;
			C = normal.Z;
			D = -distance;
		}

		/// <summary>
		/// Constructs a plane with the specified <see cref="PlaneF"/> object.
		/// </summary>
		/// <param name="source">A specified plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Plane( PlaneF source )
		{
			A = source.A;
			B = source.B;
			C = source.C;
			D = source.D;
		}

		/// <summary>
		/// Constructs a plane from the given <see cref="Vector4"/> object.
		/// </summary>
		/// <param name="source">A specified vector.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Plane( Vector4 source )
		{
			A = source.X;
			B = source.Y;
			C = source.Z;
			D = source.W;
		}

		/// <summary>
		/// Converts a string representation of a plane into the equivalent <see cref="Plane"/> structure.
		/// </summary>
		/// <param name="text">The string representation of the plane (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Plane"/> structure.</returns>
		[AutoConvertType]
		public static Plane Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (x y z w).", text ) );

			try
			{
				return new Plane(
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
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Plane"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Plane"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 17 );
			//return string.Format( "{0} {1} {2} {3}", a, b, c, d );
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Plane"/> with a given precision.
		/// </summary>
		/// <param name="precision">The precision value.</param>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Plane"/>.</returns>
		public string ToString( int precision )
		{
			string format = "";

			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "}";
			return string.Format( format, A, B, C, D );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Plane"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Plane"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Plane"/>; otherwise, False.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Plane && this == (Plane)obj );
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
		public static Plane operator +( Plane p0, Plane p1 )
		{
			Plane result;
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
		public static Plane operator -( Plane p0, Plane p1 )
		{
			Plane result;
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
		public static Plane operator -( Plane p )
		{
			Plane result;
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
		public static void Add( ref Plane p0, ref Plane p1, out Plane result )
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
		public static void Subtract( ref Plane p0, ref Plane p1, out Plane result )
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
		public static void Negate( ref Plane p, out Plane result )
		{
			result.A = -p.A;
			result.B = -p.B;
			result.C = -p.C;
			result.D = -p.D;
		}

		//public static Plane Add( Plane p0, Plane p1 )
		//{
		//	Plane result;
		//	result.a = p0.a + p1.a;
		//	result.b = p0.b + p1.b;
		//	result.c = p0.c + p1.c;
		//	result.d = p0.d + p1.d;
		//	return result;
		//}

		//public static Plane Subtract( Plane p0, Plane p1 )
		//{
		//	Plane result;
		//	result.a = p0.a - p1.a;
		//	result.b = p0.b - p1.b;
		//	result.c = p0.c - p1.c;
		//	result.d = p0.d - p1.d;
		//	return result;
		//}

		//public static Plane Negate( Plane p )
		//{
		//	Plane result;
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
		public static bool operator ==( Plane p0, Plane p1 )
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
		public static bool operator !=( Plane p0, Plane p1 )
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
		public unsafe double this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.A )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.A )
				{
					v[ index ] = value;
				}
			}
		}

		/// <summary>
		/// Determines whether the specified plane is equal to the current instance of <see cref="Plane"/>
		/// with a given precision.
		/// </summary>
		/// <param name="p">The plane to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified plane is equal to the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Plane p, double epsilon )
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
		/// Determines whether the specified plane is equal to the current instance of <see cref="Plane"/>
		/// with a given precision.
		/// </summary>
		/// <param name="p">The plane to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified plane is equal to the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref Plane p, double epsilon )
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
		/// Determines whether the specified plane is equal to the current instance of <see cref="Plane"/>
		/// with the given normal and distance precisions.
		/// </summary>
		/// <param name="p">The plane to compare.</param>
		/// <param name="normalEpsilon">The precision value for the plane normal.</param>
		/// <param name="distanceEpsilon">The precision value for the distance component of the plane.</param>
		/// <returns>True if the specified plane is equal to the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Plane p, double normalEpsilon, double distanceEpsilon )
		{
			if( Math.Abs( D - p.D ) > distanceEpsilon )
				return false;
			Vector3 n = Normal;
			Vector3 pn = p.Normal;
			if( !n.Equals( ref pn, normalEpsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the specified plane is equal to the current instance of <see cref="Plane"/>
		/// with the given normal and distance precisions.
		/// </summary>
		/// <param name="p">The plane to compare.</param>
		/// <param name="normalEpsilon">The precision value for the plane normal.</param>
		/// <param name="distanceEpsilon">The precision value for the distance component of the plane.</param>
		/// <returns>True if the specified plane is equal to the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref Plane p, double normalEpsilon, double distanceEpsilon )
		{
			if( Math.Abs( D - p.D ) > distanceEpsilon )
				return false;
			Vector3 n = Normal;
			Vector3 pn = p.Normal;
			if( !n.Equals( ref pn, normalEpsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// The normal vector of the plane.
		/// </summary>
		public Vector3 Normal
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector3 result;
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
		public double Distance
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return -D; }
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { D = -value; }
		}

		/// <summary>
		/// Converts a normal vector of the current instance of <see cref="Plane"/> into a unit vector.
		/// </summary>
		/// <returns>The length of the plane normal vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double Normalize()
		{
			Vector3 normal = Normal;
			double length = normal.Normalize();
			Normal = normal;
			return length;
		}

		/// <summary>
		/// Converts a normal vector of the current instance of <see cref="Plane"/> into a unit vector and returns the resulting plane.
		/// </summary>
		/// <returns>The plane with normalized normal vector.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Plane GetNormalize()
		{
			Plane result;
			Vector3 normal = Normal;
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
		public double GetDistance( Vector3 v )
		{
			return A * v.X + B * v.Y + C * v.Z + D;
		}

		/// <summary>
		/// Calculates the dot product of a specified vector and the normal of the plane plus the distance value of the plane.
		/// </summary>
		/// <param name="v">A specified vector.</param>
		/// <returns>The dot product of a specified vector and the normal of the plane plus the distance value of the plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetDistance( ref Vector3 v )
		{
			return A * v.X + B * v.Y + C * v.Z + D;
		}

		/// <summary>
		/// Determines from which side of the plane the point is on.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <returns>The resulting side of the plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( Vector3 point )
		{
			double distance = GetDistance( ref point );
			if( distance > 0.0 )
				return Side.Positive;
			if( distance < 0.0 )
				return Side.Negative;
			return Side.No;
		}

		/// <summary>
		/// Determines from which side of the plane the point is on.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <returns>The resulting side of the plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( ref Vector3 point )
		{
			double distance = GetDistance( ref point );
			if( distance > 0.0 )
				return Side.Positive;
			if( distance < 0.0 )
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
		public Side GetSide( Vector3 point, double epsilon )
		{
			double distance = GetDistance( ref point );
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
		public Side GetSide( ref Vector3 point, double epsilon )
		{
			double distance = GetDistance( ref point );
			if( distance > epsilon )
				return Side.Positive;
			if( distance < -epsilon )
				return Side.Negative;
			return Side.No;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Plane"/> into an instance of <see cref="Vector4"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector4 ToVector4()
		{
			Vector4 result;
			result.X = A;
			result.Y = B;
			result.Z = C;
			result.W = D;
			return result;
		}

		/// <summary>
		/// Creates an instance of <see cref="Plane"/> that contains the three given points. 
		/// </summary>
		/// <param name="point0">The first point defining the plane.</param>
		/// <param name="point1">The second point defining the plane.</param>
		/// <param name="point2">The third point defining the plane.</param>
		/// <returns>The resulting plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Plane FromPoints( Vector3 point0, Vector3 point1, Vector3 point2 )
		{
			Plane result;
			FromPoints( ref point0, ref point1, ref point2, out result );
			return result;
			//Vec3d edge1 = point1 - point0;
			//Vec3d edge2 = point2 - point0;
			//Vec3d normal = Vec3d.Cross( edge1, edge2 ).GetNormalize();
			//return new PlaneD( normal, Vec3d.Dot( normal, point0 ) );
		}

		/// <summary>
		/// Creates an instance of <see cref="Plane"/> that contains the three given points. 
		/// </summary>
		/// <param name="point0">The first point defining the plane.</param>
		/// <param name="point1">The second point defining the plane.</param>
		/// <param name="point2">The third point defining the plane.</param>
		/// <param name="result">When the method completes, contains the resulting plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromPoints( ref Vector3 point0, ref Vector3 point1, ref Vector3 point2, out Plane result )
		{
			Vector3 edge1;
			Vector3.Subtract( ref point1, ref point0, out edge1 );
			Vector3 edge2;
			Vector3.Subtract( ref point2, ref point0, out edge2 );

			Vector3 normal;
			Vector3.Cross( ref edge1, ref edge2, out normal );
			normal.Normalize();
			result.A = normal.X;
			result.B = normal.Y;
			result.C = normal.Z;
			result.D = -( normal.X * point0.X + normal.Y * point0.Y + normal.Z * point0.Z );
		}

		/// <summary>
		/// Creates an instance of <see cref="Plane"/> with point and two direction vectors.
		/// </summary>
		/// <param name="dir1">The first direction vector.</param>
		/// <param name="dir2">The second direction vector.</param>
		/// <param name="p">The point that is the start of direction vectors.</param>
		/// <returns>The resulting plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Plane FromVectors( Vector3 dir1, Vector3 dir2, Vector3 p )
		{
			Plane result;
			Vector3 normal;
			Vector3.Cross( ref dir1, ref dir2, out normal );
			normal.Normalize();
			result.A = normal.X;
			result.B = normal.Y;
			result.C = normal.Z;
			result.D = -( normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z );
			return result;
			//Vec3d normal = Vec3d.Cross( dir1, dir2 ).GetNormalize();
			//return new PlaneD( normal, normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z );
		}

		/// <summary>
		/// Creates an instance of <see cref="Plane"/> with point and two direction vectors.
		/// </summary>
		/// <param name="dir1">The first direction vector.</param>
		/// <param name="dir2">The second direction vector.</param>
		/// <param name="p">The point that is the start of direction vectors.</param>
		/// <param name="result">When the method completes, contains the resulting plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromVectors( ref Vector3 dir1, ref Vector3 dir2, ref Vector3 p, out Plane result )
		{
			Vector3 normal;
			Vector3.Cross( ref dir1, ref dir2, out normal );
			normal.Normalize();
			result.A = normal.X;
			result.B = normal.Y;
			result.C = normal.Z;
			result.D = -( normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z );
		}

		/// <summary>
		/// Creates an instance of <see cref="Plane"/> with the normal and the point.
		/// </summary>
		/// <param name="point">Any point that lies along the plane.</param>
		/// <param name="normal">The normal vector to the plane.</param>
		/// <returns>The resulting plane.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Plane FromPointAndNormal( Vector3 point, Vector3 normal )
		{
			double distance = Vector3.Dot( ref point, ref normal );
			return new Plane( normal, distance );
		}

		/// <summary>
		/// Creates an instance of <see cref="Plane"/> with the normal and the point.
		/// </summary>
		/// <param name="point">Any point that lies along the plane.</param>
		/// <param name="normal">The normal vector to the plane.</param>
		/// <param name="plane">When the method completes, contains the resulting plane.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromPointAndNormal( ref Vector3 point, ref Vector3 normal, out Plane plane )
		{
			double distance = Vector3.Dot( ref point, ref normal );
			plane = new Plane( normal, distance );
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="Plane"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <param name="scale">When the method completes, contains the ray and plane intersection.</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref Ray ray, out double scale )
		{
			double d1 = A * ray.Origin.X + B * ray.Origin.Y + C * ray.Origin.Z + D;
			double d2 = A * ray.Direction.X + B * ray.Direction.Y + C * ray.Direction.Z;
			if( d2 == 0.0 )
			{
				scale = 0;
				return false;
			}
			scale = -( d1 / d2 );
			return true;
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="Plane"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <param name="scale">When the method completes, contains the ray and plane intersection.</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Ray ray, out double scale )
		{
			double d1 = A * ray.Origin.X + B * ray.Origin.Y + C * ray.Origin.Z + D;
			double d2 = A * ray.Direction.X + B * ray.Direction.Y + C * ray.Direction.Z;
			if( d2 == 0.0 )
			{
				scale = 0;
				return false;
			}
			scale = -( d1 / d2 );
			return true;
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="Plane"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <param name="intersectionPoint">The resulting point of intersection of the plane and the ray (if they intersected).</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref Ray ray, out Vector3 intersectionPoint )
		{
			double d1 = A * ray.Origin.X + B * ray.Origin.Y + C * ray.Origin.Z + D;
			double d2 = A * ray.Direction.X + B * ray.Direction.Y + C * ray.Direction.Z;
			if( d2 == 0.0 )
			{
				intersectionPoint = Vector3.Zero;
				return false;
			}
			double scale = -( d1 / d2 );
			intersectionPoint = ray.GetPointOnRay( scale );
			return true;
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="Plane"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <param name="intersectionPoint">The resulting point of intersection of the plane and the ray (if they intersected).</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Ray ray, out Vector3 intersectionPoint )
		{
			double d1 = A * ray.Origin.X + B * ray.Origin.Y + C * ray.Origin.Z + D;
			double d2 = A * ray.Direction.X + B * ray.Direction.Y + C * ray.Direction.Z;
			if( d2 == 0.0 )
			{
				intersectionPoint = Vector3.Zero;
				return false;
			}
			double scale = -( d1 / d2 );
			intersectionPoint = ray.GetPointOnRay( scale );
			return true;
		}

		/// <summary>
		/// Determines whether the given line intersects the current instance of <see cref="Plane"/>.
		/// </summary>
		/// <param name="line">The line to check.</param>
		/// <param name="scale">When the method completes, contains the line and plane intersection.</param>
		/// <returns>True if the given line intersects the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref Line3 line, out double scale )
		{
			scale = 0;
			double d1 = A * line.Start.X + B * line.Start.Y + C * line.Start.Z + D;
			double d2 = A * line.End.X + B * line.End.Y + C * line.End.Z + D;
			if( d1 == d2 )
				return false;
			if( d1 > 0.0 && d2 > 0.0 )
				return false;
			if( d1 < 0.0 && d2 < 0.0 )
				return false;
			double fraction = ( d1 / ( d1 - d2 ) );
			if( fraction < 0.0 || fraction > 1.0 )
				return false;
			scale = fraction;
			return true;
		}

		/// <summary>
		/// Determines whether the given line intersects the current instance of <see cref="Plane"/>.
		/// </summary>
		/// <param name="line">The line to check.</param>
		/// <param name="scale">When the method completes, contains the line and plane intersection.</param>
		/// <returns>True if the given line intersects the current instance of <see cref="Plane"/>; False otherwise.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Line3 line, out double scale )
		{
			return Intersects( ref line, out scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Line3 line, Vector3 end )
		{
			double scale;
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
		public Side GetSide( ref Vector3 boundsCenter, ref Vector3 boundsHalfSize )
		{
			// Calculate the distance between box centre and the plane
			double distance = A * boundsCenter.X + B * boundsCenter.Y + C * boundsCenter.Z + D;
			//double distance = GetDistance( ref boundsCenter );

			// Calculate the maximise allows absolute distance for
			// the distance between box centre and plane
			double maxAbsDist =
				Math.Abs( A * boundsHalfSize.X ) +
				Math.Abs( B * boundsHalfSize.Y ) +
				Math.Abs( C * boundsHalfSize.Z );
			//double maxAbsDist = normal.absDotProduct( halfSize );

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
		public Side GetSide( Vector3 boundsCenter, Vector3 boundsHalfSize )
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
		public Side GetSide( ref Bounds bounds )
		{
			Vector3 boundsCenter;
			bounds.GetCenter( out boundsCenter );

			//Vec3d boundsHalfSize = boundsCenter - bounds.Minimum;
			Vector3 boundsHalfSize;
			Vector3.Subtract( ref boundsCenter, ref bounds.Minimum, out boundsHalfSize );

			return GetSide( ref boundsCenter, ref boundsHalfSize );
		}

		/// <summary>
		/// Returns side of the plane that the given box lies on.
		/// The box is defined as bounds.
		/// </summary>
		/// <param name="bounds">The given bounds.</param>
		/// <returns>The resulting side.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Side GetSide( Bounds bounds )
		{
			return GetSide( ref bounds );
		}

		/// <summary>
		/// Converts the current instance of <see cref="Plane"/> to the plane of <see cref="PlaneF"/> format.
		/// </summary>
		/// <returns>The plane of <see cref="PlaneF"/> format.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public PlaneF ToPlaneF()
		{
			PlaneF result;
			result.A = (float)A;
			result.B = (float)B;
			result.C = (float)C;
			result.D = (float)D;
			return result;
		}
	}
}