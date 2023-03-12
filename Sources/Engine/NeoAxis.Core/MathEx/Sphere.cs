// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a double precision sphere shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Sphere
	{
		public Vector3 Center;
		public double Radius;

		public static readonly Sphere Zero = new Sphere( Vector3.Zero, 0.0f );
		public static readonly Sphere Cleared = new Sphere( Vector3.Zero, -1.0f );

		public Sphere( Sphere source )
		{
			Center = source.Center;
			Radius = source.Radius;
		}

		public Sphere( Vector3 center, double radius )
		{
			this.Center = center;
			this.Radius = radius;
		}

		public Sphere( SphereF source )
		{
			Center = source.Center.ToVector3();
			Radius = source.Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Sphere && this == (Sphere)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Center.GetHashCode() ^ Radius.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Sphere v1, Sphere v2 )
		{
			return ( v1.Center == v2.Center && v1.Radius == v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Sphere v1, Sphere v2 )
		{
			return ( v1.Center != v2.Center || v1.Radius != v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Sphere v, double epsilon )
		{
			if( !Center.Equals( ref v.Center, epsilon ) )
				return false;
			if( Math.Abs( Radius - v.Radius ) > epsilon )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IsCleared()
		{
			return Radius < 0.0f;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Bounds ToBounds()
		{
			Bounds result;
			result.Minimum.X = Center.X - Radius;
			result.Minimum.Y = Center.Y - Radius;
			result.Minimum.Z = Center.Z - Radius;
			result.Maximum.X = Center.X + Radius;
			result.Maximum.Y = Center.Y + Radius;
			result.Maximum.Z = Center.Z + Radius;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToBounds( out Bounds result )
		{
			result.Minimum.X = Center.X - Radius;
			result.Minimum.Y = Center.Y - Radius;
			result.Minimum.Z = Center.Z - Radius;
			result.Maximum.X = Center.X + Radius;
			result.Maximum.Y = Center.Y + Radius;
			result.Maximum.Z = Center.Z + Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( Vector3 p )
		{
			double x = p.X - Center.X;
			double y = p.Y - Center.Y;
			double z = p.Z - Center.Z;
			double lengthSqr = x * x + y * y + z * z;
			if( lengthSqr > Radius * Radius )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref Sphere s )
		{
			double x = s.Center.X - Center.X;
			double y = s.Center.Y - Center.Y;
			double z = s.Center.Z - Center.Z;
			double lengthSqr = x * x + y * y + z * z;
			double r = s.Radius + Radius;
			if( lengthSqr > r * r )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Sphere s )
		{
			return Intersects( ref s );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Cone cone )
		{
			return cone.Intersects( this );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( ref Sphere s )
		{
			return ( Center - s.Center ).Length() + s.Radius <= Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( Sphere s )
		{
			return Contains( ref s );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( ref Box box )
		{
			Vector3[] points = null;
			box.ToPoints( ref points );
			foreach( Vector3 point in points )
			{
				if( !Contains( point ) )
					return false;
			}
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( Box box )
		{
			return Contains( ref box );
		}

		//!!!!было. сложно.
		//public bool Add( Vec3 p )
		//{
		//	if( radius < 0.0 )
		//	{
		//		origin = p;
		//		radius = 0.0;
		//		return true;
		//	}
		//	else
		//	{
		//		double x = p.x - origin.x;
		//		double y = p.y - origin.y;
		//		double z = p.z - origin.z;
		//		double lengthSqr = x * x + y * y + z * z;
		//		double r = lengthSqr;
		//		//double r = ( p - origin ).LengthSqr();

		//		if( r > radius * radius )
		//		{
		//			r = Math.Sqrt( r );

		//			double coef = .5 * ( 1.0 - radius / r );
		//			origin.x += x * coef;
		//			origin.y += y * coef;
		//			origin.z += z * coef;
		//			//origin += ( p - origin ) * 0.5f * ( 1.0f - radius / r );

		//			radius += .5 * ( r - radius );
		//			return true;
		//		}
		//		return false;
		//	}
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		static void ClosestPtPointTriangle( ref Vector3 p, ref Vector3 a, ref Vector3 b, ref Vector3 c, out Vector3 result )
		{
			Vector3 ab;
			Vector3.Subtract( ref b, ref a, out ab );
			//Vec3D ab = b - a;

			Vector3 ac;
			Vector3.Subtract( ref c, ref a, out ac );
			//Vec3D ac = c - a;

			Vector3 ap;
			Vector3.Subtract( ref p, ref a, out ap );
			//Vec3D ap = p - a;

			double d1;
			Vector3.Dot( ref ab, ref ap, out d1 );
			double d2;
			Vector3.Dot( ref ac, ref ap, out d2 );
			if( d1 <= 0.0 && d2 <= 0.0 )
			{
				result = a;
				return;
			}

			//check if P in vertex region outside B
			Vector3 bp;
			Vector3.Subtract( ref p, ref b, out bp );
			//Vec3D bp = p - b;
			double d3;
			Vector3.Dot( ref ab, ref bp, out d3 );
			double d4;
			Vector3.Dot( ref ac, ref bp, out d4 );
			if( d3 >= 0.0 && d4 <= d3 )
			{
				result = b;
				return;
			}

			double vc = d1 * d4 - d3 * d2;
			if( vc <= 0.0 && d1 >= 0.0 && d3 <= 0.0 )
			{
				double v = d1 / ( d1 - d3 );
				result = a + v * ab;
				return;
			}

			Vector3 cp;
			Vector3.Subtract( ref p, ref c, out cp );
			//Vec3D cp = p - c;
			double d5; Vector3.Dot( ref ab, ref cp, out d5 );
			double d6; Vector3.Dot( ref ac, ref cp, out d6 );
			if( d6 >= 0.0 && d5 <= d6 )
			{
				result = c;
				return;
			}

			double vb = d5 * d2 - d1 * d6;
			if( vb <= 0.0 && d2 >= 0.0 && d6 <= 0.0 )
			{
				double w = d2 / ( d2 - d6 );
				result = a + w * ac;
				return;
			}

			double va = d3 * d6 - d5 * d4;
			if( va <= 0.0 && ( d4 - d3 ) >= 0.0 && ( d5 - d6 ) >= 0.0 )
			{
				double w = ( d4 - d3 ) / ( ( d4 - d3 ) + ( d5 - d6 ) );
				result = b + w * ( c - b );
				return;
			}

			{
				double denom = 1.0 / ( va + vb + vc );
				double v = vb * denom;
				double w = vc * denom;
				result = a + ab * v + ac * w;
				return;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref Triangle triangle )
		{
			Vector3 point;
			ClosestPtPointTriangle( ref Center, ref triangle.A, ref triangle.B, ref triangle.C, out point );
			Vector3 v;
			Vector3.Subtract( ref point, ref Center, out v );
			return v.LengthSquared() <= Radius * Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Triangle triangle )
		{
			return Intersects( ref triangle );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public bool Intersects( ref Bounds bounds )
		{
			double distanceSqr = 0;

			if( Center.X < bounds.Minimum.X )
			{
				double s = Center.X - bounds.Minimum.X;
				distanceSqr += s * s;
			}
			else if( Center.X > bounds.Maximum.X )
			{
				double s = Center.X - bounds.Maximum.X;
				distanceSqr += s * s;
			}

			if( Center.Y < bounds.Minimum.Y )
			{
				double s = Center.Y - bounds.Minimum.Y;
				distanceSqr += s * s;
			}
			else if( Center.Y > bounds.Maximum.Y )
			{
				double s = Center.Y - bounds.Maximum.Y;
				distanceSqr += s * s;
			}

			if( Center.Z < bounds.Minimum.Z )
			{
				double s = Center.Z - bounds.Minimum.Z;
				distanceSqr += s * s;
			}
			else if( Center.Z > bounds.Maximum.Z )
			{
				double s = Center.Z - bounds.Maximum.Z;
				distanceSqr += s * s;
			}

			return distanceSqr <= Radius * Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Bounds bounds )
		{
			return Intersects( ref bounds );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public bool Intersects( ref Box box )
		{
			Matrix3 transposedAxis;
			box.Axis.GetTranspose( out transposedAxis );

			//Vec3D diff = origin - box.center;
			Vector3 diff;
			Vector3.Subtract( ref Center, ref box.Center, out diff );

			//Vec3D localSpherePosition = inverseBoxAxis * diff;
			Vector3 localSpherePosition;
			Matrix3.Multiply( ref transposedAxis, ref diff, out localSpherePosition );

			double distanceSquared = 0;

			for( int n = 0; n < 3; n++ )
			{
				if( localSpherePosition[ n ] > box.Extents[ n ] )
				{
					double z = localSpherePosition[ n ] - box.Extents[ n ];
					distanceSquared += z * z;
				}
				else if( localSpherePosition[ n ] < -box.Extents[ n ] )
				{
					double z = -box.Extents[ n ] - localSpherePosition[ n ];
					distanceSquared += z * z;
				}
			}

			return distanceSquared <= Radius * Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Box box )
		{
			return Intersects( ref box );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Line3 line )
		{
			Vector3 s = line.Start - Center;
			Vector3 e = line.End - Center;
			Vector3 r = e - s;
			double a = Vector3.Dot( -s, r );
			if( a <= 0 )
			{
				return ( Vector3.Dot( s, s ) < Radius * Radius );
			}
			else if( a >= Vector3.Dot( r, r ) )
			{
				return ( Vector3.Dot( e, e ) < Radius * Radius );
			}
			else
			{
				r = s + ( a / ( Vector3.Dot( r, r ) ) ) * r;
				return ( Vector3.Dot( r, r ) < Radius * Radius );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Ray ray, out double scale1, out double scale2 )
		{
			Vector3 p = ray.Origin - Center;
			double a = Vector3.Dot( ray.Direction, ray.Direction );
			double b = Vector3.Dot( ray.Direction, p );
			double c = Vector3.Dot( p, p ) - Radius * Radius;
			double d = b * b - c * a;
			if( d < 0 )
			{
				scale1 = 0;
				scale2 = 0;
				return false;
			}
			double sqrt = Math.Sqrt( d );
			a = 1.0 / a;
			scale1 = (double)( ( -b + sqrt ) * a );
			scale2 = (double)( ( -b - sqrt ) * a );
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Ray ray )
		{
			Vector3 p = ray.Origin - Center;
			double a = Vector3.Dot( ray.Direction, ray.Direction );
			double b = Vector3.Dot( ray.Direction, p );
			double c = Vector3.Dot( p, p ) - Radius * Radius;
			double d = b * b - c * a;
			if( d < 0 )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SphereF ToSphereF()
		{
			SphereF result;
			result.Center = Center.ToVector3F();
			result.Radius = (float)Radius;
			return result;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Sphere Merge( Sphere a, Sphere b )
		{
			if( a == b )
				return a;

			var c1 = a.Center;
			var c2 = b.Center;
			var r1 = a.Radius;
			var r2 = b.Radius;

			var r = ( c1 - c2 ).Length();
			if( r + r1 < r2 )
				return b;
			if( r + r2 < r1 )
				return a;

			var newR = ( r1 + r2 + r ) * 0.5;
			var newC = c1 + ( c2 - c1 ) * ( newR - r1 ) / r;
			return new Sphere( newC, newR );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Merge( ref Sphere a, ref Sphere b, out Sphere result )
		{
			if( a == b )
			{
				result = a;
				return;
			}

			ref var c1 = ref a.Center;
			ref var c2 = ref b.Center;
			var r1 = a.Radius;
			var r2 = b.Radius;

			var r = ( c1 - c2 ).Length();
			if( r + r1 < r2 )
			{
				result = b;
				return;
			}
			if( r + r2 < r1 )
			{
				result = a;
				return;
			}

			var newR = ( r1 + r2 + r ) * 0.5;
			var newC = c1 + ( c2 - c1 ) * ( newR - r1 ) / r;
			result = new Sphere( newC, newR );
		}
	}
}
