// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a single precision sphere shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct SphereF
	{
		public Vector3F Origin;
		public float Radius;

		public static readonly SphereF Zero = new SphereF( Vector3F.Zero, 0.0f );
		public static readonly SphereF Cleared = new SphereF( Vector3F.Zero, -1.0f );

		public SphereF( SphereF source )
		{
			Origin = source.Origin;
			Radius = source.Radius;
		}

		public SphereF( Vector3F origin, float radius )
		{
			this.Origin = origin;
			this.Radius = radius;
		}

		public override bool Equals( object obj )
		{
			return ( obj is SphereF && this == (SphereF)obj );
		}

		public override int GetHashCode()
		{
			return ( Origin.GetHashCode() ^ Radius.GetHashCode() );
		}

		public static bool operator ==( SphereF v1, SphereF v2 )
		{
			return ( v1.Origin == v2.Origin && v1.Radius == v2.Radius );
		}

		public static bool operator !=( SphereF v1, SphereF v2 )
		{
			return ( v1.Origin != v2.Origin || v1.Radius != v2.Radius );
		}

		public bool Equals( SphereF v, float epsilon )
		{
			if( !Origin.Equals( ref v.Origin, epsilon ) )
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

		public BoundsF ToBounds()
		{
			BoundsF result;
			result.Minimum.X = Origin.X - Radius;
			result.Minimum.Y = Origin.Y - Radius;
			result.Minimum.Z = Origin.Z - Radius;
			result.Maximum.X = Origin.X + Radius;
			result.Maximum.Y = Origin.Y + Radius;
			result.Maximum.Z = Origin.Z + Radius;
			return result;
		}

		public void ToBounds( out BoundsF result )
		{
			result.Minimum.X = Origin.X - Radius;
			result.Minimum.Y = Origin.Y - Radius;
			result.Minimum.Z = Origin.Z - Radius;
			result.Maximum.X = Origin.X + Radius;
			result.Maximum.Y = Origin.Y + Radius;
			result.Maximum.Z = Origin.Z + Radius;
		}

		public bool Contains( Vector3F p )
		{
			float x = p.X - Origin.X;
			float y = p.Y - Origin.Y;
			float z = p.Z - Origin.Z;
			float lengthSqr = x * x + y * y + z * z;
			if( lengthSqr > Radius * Radius )
				return false;
			return true;
		}

		public bool Intersects( ref SphereF s )
		{
			float x = s.Origin.X - Origin.X;
			float y = s.Origin.Y - Origin.Y;
			float z = s.Origin.Z - Origin.Z;
			float lengthSqr = x * x + y * y + z * z;
			float r = s.Radius + Radius;
			if( lengthSqr > r * r )
				return false;
			return true;
		}

		public bool Intersects( SphereF s )
		{
			return Intersects( ref s );
		}

		public bool Intersects( ConeF cone )
		{
			return cone.Intersects( this );
		}

		public bool Contains( ref SphereF s )
		{
			return ( Origin - s.Origin ).Length() + s.Radius <= Radius;
		}

		public bool Contains( SphereF s )
		{
			return Contains( ref s );
		}

		public bool Contains( ref BoxF box )
		{
			Vector3F[] points = null;
			box.ToPoints( ref points );
			foreach( Vector3F point in points )
			{
				if( !Contains( point ) )
					return false;
			}
			return true;
		}

		public bool Contains( BoxF box )
		{
			return Contains( ref box );
		}

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

		static void ClosestPtPointTriangle( ref Vector3F p, ref Vector3F a, ref Vector3F b, ref Vector3F c, out Vector3F result )
		{
			Vector3F ab;
			Vector3F.Subtract( ref b, ref a, out ab );
			//Vec3 ab = b - a;

			Vector3F ac;
			Vector3F.Subtract( ref c, ref a, out ac );
			//Vec3 ac = c - a;

			Vector3F ap;
			Vector3F.Subtract( ref p, ref a, out ap );
			//Vec3 ap = p - a;

			float d1;
			Vector3F.Dot( ref ab, ref ap, out d1 );
			float d2;
			Vector3F.Dot( ref ac, ref ap, out d2 );
			if( d1 <= 0.0f && d2 <= 0.0f )
			{
				result = a;
				return;
			}

			//check if P in vertex region outside B
			Vector3F bp;
			Vector3F.Subtract( ref p, ref b, out bp );
			//Vec3 bp = p - b;
			float d3;
			Vector3F.Dot( ref ab, ref bp, out d3 );
			float d4;
			Vector3F.Dot( ref ac, ref bp, out d4 );
			if( d3 >= 0.0f && d4 <= d3 )
			{
				result = b;
				return;
			}

			float vc = d1 * d4 - d3 * d2;
			if( vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f )
			{
				float v = d1 / ( d1 - d3 );
				result = a + v * ab;
				return;
			}

			Vector3F cp;
			Vector3F.Subtract( ref p, ref c, out cp );
			//Vec3 cp = p - c;
			float d5; Vector3F.Dot( ref ab, ref cp, out d5 );
			float d6; Vector3F.Dot( ref ac, ref cp, out d6 );
			if( d6 >= 0.0f && d5 <= d6 )
			{
				result = c;
				return;
			}

			float vb = d5 * d2 - d1 * d6;
			if( vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f )
			{
				float w = d2 / ( d2 - d6 );
				result = a + w * ac;
				return;
			}

			float va = d3 * d6 - d5 * d4;
			if( va <= 0.0f && ( d4 - d3 ) >= 0.0f && ( d5 - d6 ) >= 0.0f )
			{
				float w = ( d4 - d3 ) / ( ( d4 - d3 ) + ( d5 - d6 ) );
				result = b + w * ( c - b );
				return;
			}

			{
				float denom = 1.0f / ( va + vb + vc );
				float v = vb * denom;
				float w = vc * denom;
				result = a + ab * v + ac * w;
				return;
			}
		}

		public bool Intersects( ref TriangleF triangle )
		{
			Vector3F point;
			ClosestPtPointTriangle( ref Origin, ref triangle.A, ref triangle.B, ref triangle.C, out point );
			Vector3F v;
			Vector3F.Subtract( ref point, ref Origin, out v );
			return v.LengthSquared() <= Radius * Radius;
		}

		public bool Intersects( TriangleF triangle )
		{
			return Intersects( ref triangle );
		}

		public bool Intersects( ref BoundsF bounds )
		{
			float distanceSqr = 0;

			if( Origin.X < bounds.Minimum.X )
			{
				float s = Origin.X - bounds.Minimum.X;
				distanceSqr += s * s;
			}
			else if( Origin.X > bounds.Maximum.X )
			{
				float s = Origin.X - bounds.Maximum.X;
				distanceSqr += s * s;
			}

			if( Origin.Y < bounds.Minimum.Y )
			{
				float s = Origin.Y - bounds.Minimum.Y;
				distanceSqr += s * s;
			}
			else if( Origin.Y > bounds.Maximum.Y )
			{
				float s = Origin.Y - bounds.Maximum.Y;
				distanceSqr += s * s;
			}

			if( Origin.Z < bounds.Minimum.Z )
			{
				float s = Origin.Z - bounds.Minimum.Z;
				distanceSqr += s * s;
			}
			else if( Origin.Z > bounds.Maximum.Z )
			{
				float s = Origin.Z - bounds.Maximum.Z;
				distanceSqr += s * s;
			}

			return distanceSqr <= Radius * Radius;
		}

		public bool Intersects( BoundsF bounds )
		{
			return Intersects( ref bounds );
		}

		public bool Intersects( ref BoxF box )
		{
			Matrix3F transposedAxis;
			box.Axis.GetTranspose( out transposedAxis );

			//Vec3 diff = origin - box.center;
			Vector3F diff;
			Vector3F.Subtract( ref Origin, ref box.Center, out diff );

			//Vec3 localSpherePosition = inverseBoxAxis * diff;
			Vector3F localSpherePosition;
			Matrix3F.Multiply( ref transposedAxis, ref diff, out localSpherePosition );

			float distanceSquared = 0;

			for( int n = 0; n < 3; n++ )
			{
				if( localSpherePosition[ n ] > box.Extents[ n ] )
				{
					float z = localSpherePosition[ n ] - box.Extents[ n ];
					distanceSquared += z * z;
				}
				else if( localSpherePosition[ n ] < -box.Extents[ n ] )
				{
					float z = -box.Extents[ n ] - localSpherePosition[ n ];
					distanceSquared += z * z;
				}
			}

			return distanceSquared <= Radius * Radius;
		}

		public bool Intersects( BoxF box )
		{
			return Intersects( ref box );
		}

		public bool Intersects( Line3F line )
		{
			Vector3F s = line.Start - Origin;
			Vector3F e = line.End - Origin;
			Vector3F r = e - s;
			float a = Vector3F.Dot( -s, r );
			if( a <= 0 )
			{
				return ( Vector3F.Dot( s, s ) < Radius * Radius );
			}
			else if( a >= Vector3F.Dot( r, r ) )
			{
				return ( Vector3F.Dot( e, e ) < Radius * Radius );
			}
			else
			{
				r = s + ( a / ( Vector3F.Dot( r, r ) ) ) * r;
				return ( Vector3F.Dot( r, r ) < Radius * Radius );
			}
		}

		public bool Intersects( RayF ray, out float scale1, out float scale2 )
		{
			Vector3F p = ray.Origin - Origin;
			double a = Vector3F.Dot( ray.Direction, ray.Direction );
			double b = Vector3F.Dot( ray.Direction, p );
			double c = Vector3F.Dot( p, p ) - Radius * Radius;
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

		public bool Intersects( RayF ray )
		{
			Vector3F p = ray.Origin - Origin;
			double a = Vector3F.Dot( ray.Direction, ray.Direction );
			double b = Vector3F.Dot( ray.Direction, p );
			double c = Vector3F.Dot( p, p ) - Radius * Radius;
			double d = b * b - c * a;
			if( d < 0 )
				return false;
			return true;
		}

		[AutoConvertType]
		public Sphere ToSphere()
		{
			Sphere result;
			result.Origin = Origin.ToVector3();
			result.Radius = Radius;
			return result;
		}

		//!!!!!
		//[AutoConvertType]
		//!!!!Parse, ToString

#if !DISABLE_IMPLICIT
		public static implicit operator Sphere( SphereF v )
		{
			return new Sphere( v );
		}
#endif

		public static SphereF Merge( SphereF a, SphereF b )
		{
			if( a == b )
				return a;

			var c1 = a.Origin;
			var c2 = b.Origin;
			var r1 = a.Radius;
			var r2 = b.Radius;

			var r = ( c1 - c2 ).Length();
			if( r + r1 < r2 )
				return b;
			if( r + r2 < r1 )
				return a;

			var newR = ( r1 + r2 + r ) * 0.5f;
			var newC = c1 + ( c2 - c1 ) * ( newR - r1 ) / r;
			return new SphereF( newC, newR );
		}
	}
}
