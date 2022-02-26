// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a single precision circle shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct CircleF
	{
		public Vector2F Center;
		public float Radius;

		public static readonly CircleF Zero = new CircleF( Vector2F.Zero, 0.0f );
		public static readonly CircleF Cleared = new CircleF( Vector2F.Zero, -1.0f );

		public CircleF( CircleF source )
		{
			Center = source.Center;
			Radius = source.Radius;
		}

		public CircleF( Vector2F center, float radius )
		{
			this.Center = center;
			this.Radius = radius;
		}

		public override bool Equals( object obj )
		{
			return ( obj is CircleF && this == (CircleF)obj );
		}

		public override int GetHashCode()
		{
			return ( Center.GetHashCode() ^ Radius.GetHashCode() );
		}

		public static bool operator ==( CircleF v1, CircleF v2 )
		{
			return ( v1.Center == v2.Center && v1.Radius == v2.Radius );
		}

		public static bool operator !=( CircleF v1, CircleF v2 )
		{
			return ( v1.Center != v2.Center || v1.Radius != v2.Radius );
		}

		public bool Equals( CircleF v, float epsilon )
		{
			if( !Center.Equals( ref v.Center, epsilon ) )
				return false;
			if( Math.Abs( Radius - v.Radius ) > epsilon )
				return false;
			return true;
		}

		//!!!!без Is. везде так
		public bool IsCleared()
		{
			return Radius < 0.0f;
		}

		public RectangleF ToBounds()
		{
			RectangleF result;
			result.Left = Center.X - Radius;
			result.Top = Center.Y - Radius;
			result.Right = Center.X + Radius;
			result.Bottom = Center.Y + Radius;
			return result;
		}

		public void ToBounds( out RectangleF result )
		{
			result.Left = Center.X - Radius;
			result.Top = Center.Y - Radius;
			result.Right = Center.X + Radius;
			result.Bottom = Center.Y + Radius;
		}

		public bool Contains( Vector2F p )
		{
			float x = p.X - Center.X;
			float y = p.Y - Center.Y;
			float lengthSqr = x * x + y * y;
			if( lengthSqr > Radius * Radius )
				return false;
			return true;
		}

		public bool Intersects( ref CircleF s )
		{
			float x = s.Center.X - Center.X;
			float y = s.Center.Y - Center.Y;
			float lengthSqr = x * x + y * y;
			float r = s.Radius + Radius;
			if( lengthSqr > r * r )
				return false;
			return true;
		}

		public bool Intersects( CircleF s )
		{
			return Intersects( ref s );
		}

		//public bool Intersects( ConeF cone )
		//{
		//	return cone.Intersects( this );
		//}

		public bool Contains( ref CircleF s )
		{
			return ( Center - s.Center ).Length() + s.Radius <= Radius;
		}

		public bool Contains( CircleF s )
		{
			return Contains( ref s );
		}

		//public bool Contains( ref BoxF box )
		//{
		//	Vector2F[] points = null;
		//	box.ToPoints( ref points );
		//	foreach( Vector2F point in points )
		//	{
		//		if( !Contains( point ) )
		//			return false;
		//	}
		//	return true;
		//}

		//public bool Contains( BoxF box )
		//{
		//	return Contains( ref box );
		//}

		//!!!!было. сложно.
		//public bool Add( Vec3F p )
		//{
		//	if( radius < 0.0f )
		//	{
		//		origin = p;
		//		radius = 0.0f;
		//		return true;
		//	}
		//	else
		//	{
		//		float x = p.x - origin.x;
		//		float y = p.y - origin.y;
		//		float z = p.z - origin.z;
		//		float lengthSqr = x * x + y * y + z * z;
		//		float r = lengthSqr;
		//		//float r = ( p - origin ).LengthSqr();

		//		if( r > radius * radius )
		//		{
		//			r = MathEx.Sqrt( r );

		//			float coef = .5f * ( 1.0f - radius / r );
		//			origin.x += x * coef;
		//			origin.y += y * coef;
		//			origin.z += z * coef;
		//			//origin += ( p - origin ) * 0.5f * ( 1.0f - radius / r );

		//			radius += .5f * ( r - radius );
		//			return true;
		//		}
		//		return false;
		//	}
		//}

		//static void ClosestPtPointTriangle( ref Vector2F p, ref Vector2F a, ref Vector2F b, ref Vector2F c, out Vector2F result )
		//{
		//	Vector2F ab;
		//	Vector2F.Subtract( ref b, ref a, out ab );
		//	//Vec3 ab = b - a;

		//	Vector2F ac;
		//	Vector2F.Subtract( ref c, ref a, out ac );
		//	//Vec3 ac = c - a;

		//	Vector2F ap;
		//	Vector2F.Subtract( ref p, ref a, out ap );
		//	//Vec3 ap = p - a;

		//	float d1;
		//	Vector2F.Dot( ref ab, ref ap, out d1 );
		//	float d2;
		//	Vector2F.Dot( ref ac, ref ap, out d2 );
		//	if( d1 <= 0.0f && d2 <= 0.0f )
		//	{
		//		result = a;
		//		return;
		//	}

		//	//check if P in vertex region outside B
		//	Vector2F bp;
		//	Vector2F.Subtract( ref p, ref b, out bp );
		//	//Vec3 bp = p - b;
		//	float d3;
		//	Vector2F.Dot( ref ab, ref bp, out d3 );
		//	float d4;
		//	Vector2F.Dot( ref ac, ref bp, out d4 );
		//	if( d3 >= 0.0f && d4 <= d3 )
		//	{
		//		result = b;
		//		return;
		//	}

		//	float vc = d1 * d4 - d3 * d2;
		//	if( vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f )
		//	{
		//		float v = d1 / ( d1 - d3 );
		//		result = a + v * ab;
		//		return;
		//	}

		//	Vector2F cp;
		//	Vector2F.Subtract( ref p, ref c, out cp );
		//	//Vec3 cp = p - c;
		//	float d5; Vector2F.Dot( ref ab, ref cp, out d5 );
		//	float d6; Vector2F.Dot( ref ac, ref cp, out d6 );
		//	if( d6 >= 0.0f && d5 <= d6 )
		//	{
		//		result = c;
		//		return;
		//	}

		//	float vb = d5 * d2 - d1 * d6;
		//	if( vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f )
		//	{
		//		float w = d2 / ( d2 - d6 );
		//		result = a + w * ac;
		//		return;
		//	}

		//	float va = d3 * d6 - d5 * d4;
		//	if( va <= 0.0f && ( d4 - d3 ) >= 0.0f && ( d5 - d6 ) >= 0.0f )
		//	{
		//		float w = ( d4 - d3 ) / ( ( d4 - d3 ) + ( d5 - d6 ) );
		//		result = b + w * ( c - b );
		//		return;
		//	}

		//	{
		//		float denom = 1.0f / ( va + vb + vc );
		//		float v = vb * denom;
		//		float w = vc * denom;
		//		result = a + ab * v + ac * w;
		//		return;
		//	}
		//}

		//public bool Intersects( ref TriangleF triangle )
		//{
		//	Vector2F point;
		//	ClosestPtPointTriangle( ref Origin, ref triangle.A, ref triangle.B, ref triangle.C, out point );
		//	Vector2F v;
		//	Vector2F.Subtract( ref point, ref Origin, out v );
		//	return v.LengthSquared() <= Radius * Radius;
		//}

		//public bool Intersects( TriangleF triangle )
		//{
		//	return Intersects( ref triangle );
		//}

		public bool Intersects( ref RectangleF bounds )
		{
			float distanceSqr = 0;

			if( Center.X < bounds.Minimum.X )
			{
				float s = Center.X - bounds.Minimum.X;
				distanceSqr += s * s;
			}
			else if( Center.X > bounds.Maximum.X )
			{
				float s = Center.X - bounds.Maximum.X;
				distanceSqr += s * s;
			}

			if( Center.Y < bounds.Minimum.Y )
			{
				float s = Center.Y - bounds.Minimum.Y;
				distanceSqr += s * s;
			}
			else if( Center.Y > bounds.Maximum.Y )
			{
				float s = Center.Y - bounds.Maximum.Y;
				distanceSqr += s * s;
			}

			return distanceSqr <= Radius * Radius;
		}

		public bool Intersects( RectangleF bounds )
		{
			return Intersects( ref bounds );
		}

		//public bool Intersects( ref BoxF box )
		//{
		//	Matrix3F transposedAxis;
		//	box.Axis.GetTranspose( out transposedAxis );

		//	//Vec3 diff = origin - box.center;
		//	Vector2F diff;
		//	Vector2F.Subtract( ref Origin, ref box.Center, out diff );

		//	//Vec3 localSpherePosition = inverseBoxAxis * diff;
		//	Vector2F localSpherePosition;
		//	Matrix3F.Multiply( ref transposedAxis, ref diff, out localSpherePosition );

		//	float distanceSquared = 0;

		//	for( int n = 0; n < 3; n++ )
		//	{
		//		if( localSpherePosition[ n ] > box.Extents[ n ] )
		//		{
		//			float z = localSpherePosition[ n ] - box.Extents[ n ];
		//			distanceSquared += z * z;
		//		}
		//		else if( localSpherePosition[ n ] < -box.Extents[ n ] )
		//		{
		//			float z = -box.Extents[ n ] - localSpherePosition[ n ];
		//			distanceSquared += z * z;
		//		}
		//	}

		//	return distanceSquared <= Radius * Radius;
		//}

		//public bool Intersects( BoxF box )
		//{
		//	return Intersects( ref box );
		//}

		public bool Intersects( Line2F line )
		{
			Vector2F s = line.Start - Center;
			Vector2F e = line.End - Center;
			Vector2F r = e - s;
			float a = Vector2F.Dot( -s, r );
			if( a <= 0 )
			{
				return ( Vector2F.Dot( s, s ) < Radius * Radius );
			}
			else if( a >= Vector2F.Dot( r, r ) )
			{
				return ( Vector2F.Dot( e, e ) < Radius * Radius );
			}
			else
			{
				r = s + ( a / ( Vector2F.Dot( r, r ) ) ) * r;
				return ( Vector2F.Dot( r, r ) < Radius * Radius );
			}
		}

		public bool Intersects( Ray2F ray, out float scale1, out float scale2 )
		{
			Vector2F p = ray.Origin - Center;
			double a = Vector2F.Dot( ray.Direction, ray.Direction );
			double b = Vector2F.Dot( ray.Direction, p );
			double c = Vector2F.Dot( p, p ) - Radius * Radius;
			double d = b * b - c * a;
			if( d < 0 )
			{
				scale1 = 0;
				scale2 = 0;
				return false;
			}
			double sqrt = Math.Sqrt( d );
			a = 1.0f / a;
			scale1 = (float)( ( -b + sqrt ) * a );
			scale2 = (float)( ( -b - sqrt ) * a );
			return true;
		}

		public bool Intersects( Ray2F ray )
		{
			Vector2F p = ray.Origin - Center;
			double a = Vector2F.Dot( ray.Direction, ray.Direction );
			double b = Vector2F.Dot( ray.Direction, p );
			double c = Vector2F.Dot( p, p ) - Radius * Radius;
			double d = b * b - c * a;
			if( d < 0 )
				return false;
			return true;
		}

		[AutoConvertType]
		public Circle ToCircle()
		{
			Circle result;
			result.Center = Center.ToVector2();
			result.Radius = Radius;
			return result;
		}

		//!!!!!
		//[AutoConvertType]
		//!!!!Parse, ToString

#if !DISABLE_IMPLICIT
		public static implicit operator Circle( CircleF v )
		{
			return new Circle( v );
		}
#endif

		public double GetPointDistance( Vector2F point )
		{
			var diff = point - Center;
			if( diff == Vector2F.Zero )
				return 0;
			var distance = diff.Length() - Radius;
			if( distance < 0 )
				return 0;
			return distance;
		}

		//public static CircleF Merge( CircleF a, CircleF b )
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

		//	var newR = ( r1 + r2 + r ) * 0.5f;
		//	var newC = c1 + ( c2 - c1 ) * ( newR - r1 ) / r;
		//	return new CircleF( newC, newR );
		//}
	}
}
