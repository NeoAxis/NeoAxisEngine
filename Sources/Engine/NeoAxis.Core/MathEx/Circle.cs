// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a double precision circle shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Circle
	{
		public Vector2 Center;
		public double Radius;

		public static readonly Circle Zero = new Circle( Vector2.Zero, 0.0f );
		public static readonly Circle Cleared = new Circle( Vector2.Zero, -1.0f );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Circle( Circle source )
		{
			Center = source.Center;
			Radius = source.Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Circle( Vector2 center, double radius )
		{
			this.Center = center;
			this.Radius = radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Circle( CircleF source )
		{
			Center = source.Center.ToVector2();
			Radius = source.Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Circle && this == (Circle)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Center.GetHashCode() ^ Radius.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Circle v1, Circle v2 )
		{
			return ( v1.Center == v2.Center && v1.Radius == v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Circle v1, Circle v2 )
		{
			return ( v1.Center != v2.Center || v1.Radius != v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Circle v, double epsilon )
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
		public Rectangle ToBounds()
		{
			Rectangle result;
			result.Left = Center.X - Radius;
			result.Top = Center.Y - Radius;
			result.Right = Center.X + Radius;
			result.Bottom = Center.Y + Radius;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToBounds( out Rectangle result )
		{
			result.Left = Center.X - Radius;
			result.Top = Center.Y - Radius;
			result.Right = Center.X + Radius;
			result.Bottom = Center.Y + Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( Vector2 p )
		{
			double x = p.X - Center.X;
			double y = p.Y - Center.Y;
			double lengthSqr = x * x + y * y;
			if( lengthSqr > Radius * Radius )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref Circle s )
		{
			double x = s.Center.X - Center.X;
			double y = s.Center.Y - Center.Y;
			double lengthSqr = x * x + y * y;
			double r = s.Radius + Radius;
			if( lengthSqr > r * r )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Circle s )
		{
			return Intersects( ref s );
		}

		//public bool Intersects( Cone cone )
		//{
		//	return cone.Intersects( this );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( ref Circle s )
		{
			return ( Center - s.Center ).Length() + s.Radius <= Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( Circle s )
		{
			return Contains( ref s );
		}

		//public bool Contains( ref Box box )
		//{
		//	Vector2[] points = null;
		//	box.ToPoints( ref points );
		//	foreach( Vector2 point in points )
		//	{
		//		if( !Contains( point ) )
		//			return false;
		//	}
		//	return true;
		//}

		//public bool Contains( Box box )
		//{
		//	return Contains( ref box );
		//}

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

		//static void ClosestPtPointTriangle( ref Vector2 p, ref Vector2 a, ref Vector2 b, ref Vector2 c, out Vector2 result )
		//{
		//	Vector2 ab;
		//	Vector2.Subtract( ref b, ref a, out ab );
		//	//Vec3D ab = b - a;

		//	Vector2 ac;
		//	Vector2.Subtract( ref c, ref a, out ac );
		//	//Vec3D ac = c - a;

		//	Vector2 ap;
		//	Vector2.Subtract( ref p, ref a, out ap );
		//	//Vec3D ap = p - a;

		//	double d1;
		//	Vector2.Dot( ref ab, ref ap, out d1 );
		//	double d2;
		//	Vector2.Dot( ref ac, ref ap, out d2 );
		//	if( d1 <= 0.0 && d2 <= 0.0 )
		//	{
		//		result = a;
		//		return;
		//	}

		//	//check if P in vertex region outside B
		//	Vector2 bp;
		//	Vector2.Subtract( ref p, ref b, out bp );
		//	//Vec3D bp = p - b;
		//	double d3;
		//	Vector2.Dot( ref ab, ref bp, out d3 );
		//	double d4;
		//	Vector2.Dot( ref ac, ref bp, out d4 );
		//	if( d3 >= 0.0 && d4 <= d3 )
		//	{
		//		result = b;
		//		return;
		//	}

		//	double vc = d1 * d4 - d3 * d2;
		//	if( vc <= 0.0 && d1 >= 0.0 && d3 <= 0.0 )
		//	{
		//		double v = d1 / ( d1 - d3 );
		//		result = a + v * ab;
		//		return;
		//	}

		//	Vector2 cp;
		//	Vector2.Subtract( ref p, ref c, out cp );
		//	//Vec3D cp = p - c;
		//	double d5; Vector2.Dot( ref ab, ref cp, out d5 );
		//	double d6; Vector2.Dot( ref ac, ref cp, out d6 );
		//	if( d6 >= 0.0 && d5 <= d6 )
		//	{
		//		result = c;
		//		return;
		//	}

		//	double vb = d5 * d2 - d1 * d6;
		//	if( vb <= 0.0 && d2 >= 0.0 && d6 <= 0.0 )
		//	{
		//		double w = d2 / ( d2 - d6 );
		//		result = a + w * ac;
		//		return;
		//	}

		//	double va = d3 * d6 - d5 * d4;
		//	if( va <= 0.0 && ( d4 - d3 ) >= 0.0 && ( d5 - d6 ) >= 0.0 )
		//	{
		//		double w = ( d4 - d3 ) / ( ( d4 - d3 ) + ( d5 - d6 ) );
		//		result = b + w * ( c - b );
		//		return;
		//	}

		//	{
		//		double denom = 1.0 / ( va + vb + vc );
		//		double v = vb * denom;
		//		double w = vc * denom;
		//		result = a + ab * v + ac * w;
		//		return;
		//	}
		//}

		//public bool Intersects( ref Triangle2 triangle )
		//{
		//	Vector2 point;
		//	ClosestPtPointTriangle( ref Origin, ref triangle.A, ref triangle.B, ref triangle.C, out point );
		//	Vector2 v;
		//	Vector2.Subtract( ref point, ref Origin, out v );
		//	return v.LengthSquared() <= Radius * Radius;
		//}

		//public bool Intersects( Triangle triangle )
		//{
		//	return Intersects( ref triangle );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref Rectangle bounds )
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

			return distanceSqr <= Radius * Radius;
		}

		public bool Intersects( Rectangle bounds )
		{
			return Intersects( ref bounds );
		}

		//public bool Intersects( ref Box box )
		//{
		//	Matrix3 transposedAxis;
		//	box.Axis.GetTranspose( out transposedAxis );

		//	//Vec3D diff = origin - box.center;
		//	Vector2 diff;
		//	Vector2.Subtract( ref Origin, ref box.Center, out diff );

		//	//Vec3D localCirclePosition = inverseBoxAxis * diff;
		//	Vector2 localCirclePosition;
		//	Matrix3.Multiply( ref transposedAxis, ref diff, out localCirclePosition );

		//	double distanceSquared = 0;

		//	for( int n = 0; n < 3; n++ )
		//	{
		//		if( localCirclePosition[ n ] > box.Extents[ n ] )
		//		{
		//			double z = localCirclePosition[ n ] - box.Extents[ n ];
		//			distanceSquared += z * z;
		//		}
		//		else if( localCirclePosition[ n ] < -box.Extents[ n ] )
		//		{
		//			double z = -box.Extents[ n ] - localCirclePosition[ n ];
		//			distanceSquared += z * z;
		//		}
		//	}

		//	return distanceSquared <= Radius * Radius;
		//}

		//public bool Intersects( Box box )
		//{
		//	return Intersects( ref box );
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Line2 line )
		{
			Vector2 s = line.Start - Center;
			Vector2 e = line.End - Center;
			Vector2 r = e - s;
			double a = Vector2.Dot( -s, r );
			if( a <= 0 )
			{
				return ( Vector2.Dot( s, s ) < Radius * Radius );
			}
			else if( a >= Vector2.Dot( r, r ) )
			{
				return ( Vector2.Dot( e, e ) < Radius * Radius );
			}
			else
			{
				r = s + ( a / ( Vector2.Dot( r, r ) ) ) * r;
				return ( Vector2.Dot( r, r ) < Radius * Radius );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Ray2 ray, out double scale1, out double scale2 )
		{
			Vector2 p = ray.Origin - Center;
			double a = Vector2.Dot( ray.Direction, ray.Direction );
			double b = Vector2.Dot( ray.Direction, p );
			double c = Vector2.Dot( p, p ) - Radius * Radius;
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
		public bool Intersects( Ray2 ray )
		{
			Vector2 p = ray.Origin - Center;
			double a = Vector2.Dot( ray.Direction, ray.Direction );
			double b = Vector2.Dot( ray.Direction, p );
			double c = Vector2.Dot( p, p ) - Radius * Radius;
			double d = b * b - c * a;
			if( d < 0 )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public CircleF ToCircleF()
		{
			CircleF result;
			result.Center = Center.ToVector2F();
			result.Radius = (float)Radius;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetPointDistance( Vector2 point )
		{
			var diff = point - Center;
			if( diff == Vector2.Zero )
				return 0;
			var distance = diff.Length() - Radius;
			if( distance < 0 )
				return 0;
			return distance;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

		//public static Circle Merge( Circle a, Circle b )
		//{
		//	if( a == b )
		//		return a;

		//	var c1 = a.Origin;
		//	var c2 = b.Origin;
		//	var r1 = a.Radius;
		//	var r2 = b.Radius;

		//	var r = ( c1 - c2 ).Length();
		//	if( r + r1 < r2 )
		//		return b;
		//	if( r + r2 < r1 )
		//		return a;

		//	var newR = ( r1 + r2 + r ) * 0.5;
		//	var newC = c1 + ( c2 - c1 ) * ( newR - r1 ) / r;
		//	return new Circle( newC, newR );
		//}
	}
}
