// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Collections.Generic;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Bounds> ) )]
	/// <summary>
	/// Represents a double precision axis-aligned bounding box in three dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Bounds
	{
		public Vector3 Minimum;
		public Vector3 Maximum;

		public static readonly Bounds Zero = new Bounds( Vector3.Zero, Vector3.Zero );
		public static readonly Bounds Cleared = new Bounds(
			new Vector3( double.MaxValue, double.MaxValue, double.MaxValue ),
			new Vector3( double.MinValue, double.MinValue, double.MinValue ) );

		public Bounds( Bounds source )
		{
			Minimum = source.Minimum;
			Maximum = source.Maximum;
		}

		public Bounds( Vector3 minimum, Vector3 maximum )
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		public Bounds( double minimumX, double minimumY, double minimumZ,
			double maximumX, double maximumY, double maximumZ )
		{
			this.Minimum = new Vector3( minimumX, minimumY, minimumZ );
			this.Maximum = new Vector3( maximumX, maximumY, maximumZ );
		}

		public Bounds( Vector3 v )
		{
			this.Minimum = v;
			this.Maximum = v;
		}

		public Bounds( BoundsF source )
		{
			Minimum = source.Minimum.ToVector3();
			Maximum = source.Maximum.ToVector3();
		}

		public Bounds( BoundsI source )
		{
			Minimum = source.Minimum.ToVector3();
			Maximum = source.Maximum.ToVector3();
		}

		public override bool Equals( object obj )
		{
			return ( obj is Bounds && this == (Bounds)obj );
		}

		public override int GetHashCode()
		{
			return ( Minimum.GetHashCode() ^ Maximum.GetHashCode() );
		}

		public static bool operator ==( Bounds v1, Bounds v2 )
		{
			return ( v1.Minimum == v2.Minimum && v1.Maximum == v2.Maximum );
		}

		public static bool operator !=( Bounds v1, Bounds v2 )
		{
			return ( v1.Minimum != v2.Minimum || v1.Maximum != v2.Maximum );
		}

		public unsafe Vector3 this[ int index ]
		{
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( Vector3* v = &this.Minimum )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( Vector3* v = &this.Minimum )
				{
					v[ index ] = value;
				}
			}
		}

		public bool Equals( Bounds v, double epsilon )
		{
			if( !Minimum.Equals( ref v.Minimum, epsilon ) )
				return false;
			if( !Maximum.Equals( ref v.Maximum, epsilon ) )
				return false;
			return true;
		}

		public bool IsCleared()
		{
			return Minimum.X > Maximum.X;
		}

		public Vector3 GetCenter()
		{
			Vector3 result;
			result.X = ( Minimum.X + Maximum.X ) * .5;
			result.Y = ( Minimum.Y + Maximum.Y ) * .5;
			result.Z = ( Minimum.Z + Maximum.Z ) * .5;
			return result;
		}

		public void GetCenter( out Vector3 result )
		{
			result.X = ( Minimum.X + Maximum.X ) * .5;
			result.Y = ( Minimum.Y + Maximum.Y ) * .5;
			result.Z = ( Minimum.Z + Maximum.Z ) * .5;
		}

		public double GetRadius()
		{
			double total = 0.0;
			for( int i = 0; i < 3; i++ )
			{
				double b0 = Math.Abs( Minimum[ i ] );
				double b1 = Math.Abs( Maximum[ i ] );
				if( b0 > b1 )
					total += b0 * b0;
				else
					total += b1 * b1;
			}
			return Math.Sqrt( total );
		}

		public double GetRadius( Vector3 center )
		{
			double total = 0.0;
			for( int i = 0; i < 3; i++ )
			{
				double b0 = Math.Abs( center[ i ] - Minimum[ i ] );
				double b1 = Math.Abs( Maximum[ i ] - center[ i ] );
				if( b0 > b1 )
					total += b0 * b0;
				else
					total += b1 * b1;
			}
			return Math.Sqrt( total );
		}

		public Sphere GetBoundingSphere()
		{
			//!!!!slowly

			var c = GetCenter();
			return new Sphere( c, GetRadius( c ) );
		}

		public double GetVolume()
		{
			Vector3 s;
			Vector3.Subtract( ref Maximum, ref Minimum, out s );
			return s.X * s.Y * s.Z;
		}

		public void Add( Vector3 v )
		{
			if( v.X < Minimum.X )
				Minimum.X = v.X;
			if( v.X > Maximum.X )
				Maximum.X = v.X;
			if( v.Y < Minimum.Y )
				Minimum.Y = v.Y;
			if( v.Y > Maximum.Y )
				Maximum.Y = v.Y;
			if( v.Z < Minimum.Z )
				Minimum.Z = v.Z;
			if( v.Z > Maximum.Z )
				Maximum.Z = v.Z;
		}

		public void Add( ref Vector3 v )
		{
			if( v.X < Minimum.X )
				Minimum.X = v.X;
			if( v.X > Maximum.X )
				Maximum.X = v.X;
			if( v.Y < Minimum.Y )
				Minimum.Y = v.Y;
			if( v.Y > Maximum.Y )
				Maximum.Y = v.Y;
			if( v.Z < Minimum.Z )
				Minimum.Z = v.Z;
			if( v.Z > Maximum.Z )
				Maximum.Z = v.Z;
		}

		public void Add( Bounds v )
		{
			if( v.Minimum.X < Minimum.X )
				Minimum.X = v.Minimum.X;
			if( v.Minimum.Y < Minimum.Y )
				Minimum.Y = v.Minimum.Y;
			if( v.Minimum.Z < Minimum.Z )
				Minimum.Z = v.Minimum.Z;
			if( v.Maximum.X > Maximum.X )
				Maximum.X = v.Maximum.X;
			if( v.Maximum.Y > Maximum.Y )
				Maximum.Y = v.Maximum.Y;
			if( v.Maximum.Z > Maximum.Z )
				Maximum.Z = v.Maximum.Z;
		}

		public void Add( ref Bounds v )
		{
			if( v.Minimum.X < Minimum.X )
				Minimum.X = v.Minimum.X;
			if( v.Minimum.Y < Minimum.Y )
				Minimum.Y = v.Minimum.Y;
			if( v.Minimum.Z < Minimum.Z )
				Minimum.Z = v.Minimum.Z;
			if( v.Maximum.X > Maximum.X )
				Maximum.X = v.Maximum.X;
			if( v.Maximum.Y > Maximum.Y )
				Maximum.Y = v.Maximum.Y;
			if( v.Maximum.Z > Maximum.Z )
				Maximum.Z = v.Maximum.Z;
		}

		public void Add( ICollection<Vector3> collection )
		{
			foreach( var v in collection )
				Add( v );
		}

		public void Add( Vector3[] collection )
		{
			for( int i = 0; i < collection.Length; i++ )
				Add( ref collection[ i ] );
		}

		public Bounds Intersection( Bounds v )
		{
			Bounds result;
			result.Minimum.X = ( v.Minimum.X > Minimum.X ) ? v.Minimum.X : Minimum.X;
			result.Minimum.Y = ( v.Minimum.Y > Minimum.Y ) ? v.Minimum.Y : Minimum.Y;
			result.Minimum.Z = ( v.Minimum.Z > Minimum.Z ) ? v.Minimum.Z : Minimum.Z;
			result.Maximum.X = ( v.Maximum.X < Maximum.X ) ? v.Maximum.X : Maximum.X;
			result.Maximum.Y = ( v.Maximum.Y < Maximum.Y ) ? v.Maximum.Y : Maximum.Y;
			result.Maximum.Z = ( v.Maximum.Z < Maximum.Z ) ? v.Maximum.Z : Maximum.Z;
			return result;
		}

		public void Intersection( ref Bounds v, out Bounds result )
		{
			result.Minimum.X = ( v.Minimum.X > Minimum.X ) ? v.Minimum.X : Minimum.X;
			result.Minimum.Y = ( v.Minimum.Y > Minimum.Y ) ? v.Minimum.Y : Minimum.Y;
			result.Minimum.Z = ( v.Minimum.Z > Minimum.Z ) ? v.Minimum.Z : Minimum.Z;
			result.Maximum.X = ( v.Maximum.X < Maximum.X ) ? v.Maximum.X : Maximum.X;
			result.Maximum.Y = ( v.Maximum.Y < Maximum.Y ) ? v.Maximum.Y : Maximum.Y;
			result.Maximum.Z = ( v.Maximum.Z < Maximum.Z ) ? v.Maximum.Z : Maximum.Z;
		}

		public void Expand( double d )
		{
			Minimum.X -= d;
			Minimum.Y -= d;
			Minimum.Z -= d;
			Maximum.X += d;
			Maximum.Y += d;
			Maximum.Z += d;
		}

		public void Expand( Vector3 d )
		{
			Minimum.X -= d.X;
			Minimum.Y -= d.Y;
			Minimum.Z -= d.Z;
			Maximum.X += d.X;
			Maximum.Y += d.Y;
			Maximum.Z += d.Z;
		}

		public bool Contains( Vector3 p )
		{
			if( p.X < Minimum.X || p.Y < Minimum.Y || p.Z < Minimum.Z || p.X > Maximum.X || p.Y > Maximum.Y || p.Z > Maximum.Z )
				return false;
			return true;
		}

		public bool Contains( ref Vector3 p )
		{
			if( p.X < Minimum.X || p.Y < Minimum.Y || p.Z < Minimum.Z || p.X > Maximum.X || p.Y > Maximum.Y || p.Z > Maximum.Z )
				return false;
			return true;
		}

		public bool Contains( Bounds v )
		{
			if( v.Minimum.X < Minimum.X || v.Minimum.Y < Minimum.Y || v.Minimum.Z < Minimum.Z
				|| v.Maximum.X > Maximum.X || v.Maximum.Y > Maximum.Y || v.Maximum.Z > Maximum.Z )
				return false;
			return true;
		}

		public bool Contains( ref Bounds v )
		{
			if( v.Minimum.X < Minimum.X || v.Minimum.Y < Minimum.Y || v.Minimum.Z < Minimum.Z
				|| v.Maximum.X > Maximum.X || v.Maximum.Y > Maximum.Y || v.Maximum.Z > Maximum.Z )
				return false;
			return true;
		}

		public bool Intersects( Bounds v )
		{
			if( v.Maximum.X < Minimum.X || v.Maximum.Y < Minimum.Y || v.Maximum.Z < Minimum.Z
				|| v.Minimum.X > Maximum.X || v.Minimum.Y > Maximum.Y || v.Minimum.Z > Maximum.Z )
				return false;
			return true;
		}

		public bool Intersects( ref Bounds v )
		{
			if( v.Maximum.X < Minimum.X || v.Maximum.Y < Minimum.Y || v.Maximum.Z < Minimum.Z
				|| v.Minimum.X > Maximum.X || v.Minimum.Y > Maximum.Y || v.Minimum.Z > Maximum.Z )
				return false;
			return true;
		}

		public Vector3 GetSize()
		{
			Vector3 result;
			Vector3.Subtract( ref Maximum, ref Minimum, out result );
			return result;
		}

		public void GetSize( out Vector3 result )
		{
			Vector3.Subtract( ref Maximum, ref Minimum, out result );
		}

		public Vector3[] ToPoints()
		{
			Vector3[] r = null;
			ToPoints( ref r );
			return r;
		}

		public void ToPoints( ref Vector3[] points )
		{
			if( points == null || points.Length < 8 )
				points = new Vector3[ 8 ];
			for( int i = 0; i < 8; i++ )
			{
				//slowly
				points[ i ].X = this[ ( i ^ ( i >> 1 ) ) & 1 ].X;
				points[ i ].Y = this[ ( i >> 1 ) & 1 ].Y;
				points[ i ].Z = this[ ( i >> 2 ) & 1 ].Z;
			}
		}

		[AutoConvertType]
		public static Bounds Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 6 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 6 parts separated by spaces in the form (x y z x y z).", text ) );

			try
			{
				return new Bounds(
					double.Parse( vals[ 0 ] ),
					double.Parse( vals[ 1 ] ),
					double.Parse( vals[ 2 ] ),
					double.Parse( vals[ 3 ] ),
					double.Parse( vals[ 4 ] ),
					double.Parse( vals[ 5 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1}", Minimum, Maximum );
			//return string.Format( "{0} {1} {2} {3} {4} {5}",
			//	Minimum.X, Minimum.Y, Minimum.Z,
			//	Maximum.X, Maximum.Y, Maximum.Z );
		}

		public bool Intersects( ref Ray ray, out double scale )
		{
			int i, ax0, ax1, ax2, side, inside;
			double f;
			Vector3 hit = Vector3.Zero;

			scale = 0.0;

			ax0 = -1;
			inside = 0;
			for( i = 0; i < 3; i++ )
			{
				if( ray.Origin[ i ] < Minimum[ i ] )
					side = 0;
				else if( ray.Origin[ i ] > Maximum[ i ] )
					side = 1;
				else
				{
					inside++;
					continue;
				}
				if( ray.Direction[ i ] == 0.0 )
					continue;
				f = ( ray.Origin[ i ] - this[ side ][ i ] );
				if( ax0 < 0 || Math.Abs( f ) > Math.Abs( scale * ray.Direction[ i ] ) )
				{
					scale = -( f / ray.Direction[ i ] );
					ax0 = i;
				}
			}

			if( scale < 0 || scale > 1 )
				return false;

			if( ax0 < 0 )
			{
				scale = 0.0;
				return ( inside == 3 );
			}

			ax1 = ( ax0 + 1 ) % 3;
			ax2 = ( ax0 + 2 ) % 3;
			hit[ ax1 ] = ray.Origin[ ax1 ] + scale * ray.Direction[ ax1 ];
			hit[ ax2 ] = ray.Origin[ ax2 ] + scale * ray.Direction[ ax2 ];

			return ( hit[ ax1 ] >= Minimum[ ax1 ] && hit[ ax1 ] <= Maximum[ ax1 ] &&
						hit[ ax2 ] >= Minimum[ ax2 ] && hit[ ax2 ] <= Maximum[ ax2 ] );
		}

		public bool Intersects( Ray ray, out double scale )
		{
			return Intersects( ref ray, out scale );
		}

		public bool Intersects( ref Ray ray )
		{
			double s;
			return Intersects( ref ray, out s );
		}

		public bool Intersects( Ray ray )
		{
			double s;
			return Intersects( ref ray, out s );
		}

		//public static BoundsD FromTransformedBounds( BoundsD bounds, Vec3D origin, Mat3 axis )
		//{
		//   Vec3D center = ( bounds.minimum + bounds.maximum ) * 0.5f;
		//   Vec3D extents = bounds.maximum - center;

		//   Vec3D rotatedExtents = new Vec3D(
		//      Math.Abs( extents.x * axis.mat0.x ) +
		//      Math.Abs( extents.y * axis.mat1.x ) +
		//      Math.Abs( extents.z * axis.mat2.x ),

		//      Math.Abs( extents.x * axis.mat0.y ) +
		//      Math.Abs( extents.y * axis.mat1.y ) +
		//      Math.Abs( extents.z * axis.mat2.y ),

		//      Math.Abs( extents.x * axis.mat0.z ) +
		//      Math.Abs( extents.y * axis.mat1.z ) +
		//      Math.Abs( extents.z * axis.mat2.z ) );

		//   //for( int i = 0; i < 3; i++ )
		//   //{
		//   //   rotatedExtents[ i ] =
		//   //      Math.Abs( extents.x * axis.mat0[ i ] ) +
		//   //      Math.Abs( extents.y * axis.mat1[ i ] ) +
		//   //      Math.Abs( extents.z * axis.mat2[ i ] );
		//   //}

		//   center = origin + center * axis;

		//   return new Bounds( center - rotatedExtents, center + rotatedExtents );
		//}

		public static Bounds operator +( Bounds b, Vector3 v )
		{
			Bounds result;
			Vector3.Add( ref b.Minimum, ref v, out result.Minimum );
			Vector3.Add( ref b.Maximum, ref v, out result.Maximum );
			return result;
		}

		public static Bounds operator +( Vector3 v, Bounds b )
		{
			Bounds result;
			Vector3.Add( ref v, ref b.Minimum, out result.Minimum );
			Vector3.Add( ref v, ref b.Maximum, out result.Maximum );
			return result;
		}

		public static Bounds operator -( Bounds b, Vector3 v )
		{
			Bounds result;
			Vector3.Subtract( ref b.Minimum, ref v, out result.Minimum );
			Vector3.Subtract( ref b.Maximum, ref v, out result.Maximum );
			return result;
		}

		public static Bounds operator -( Vector3 v, Bounds b )
		{
			Bounds result;
			Vector3.Subtract( ref v, ref b.Minimum, out result.Minimum );
			Vector3.Subtract( ref v, ref b.Maximum, out result.Maximum );
			return result;
		}

		public static void Add( ref Bounds b, ref Vector3 v, out Bounds result )
		{
			Vector3.Add( ref b.Minimum, ref v, out result.Minimum );
			Vector3.Add( ref b.Maximum, ref v, out result.Maximum );
		}

		public static void Add( ref Vector3 v, ref Bounds b, out Bounds result )
		{
			Vector3.Add( ref v, ref b.Minimum, out result.Minimum );
			Vector3.Add( ref v, ref b.Maximum, out result.Maximum );
		}

		public static void Subtract( ref Bounds b, ref Vector3 v, out Bounds result )
		{
			Vector3.Subtract( ref b.Minimum, ref v, out result.Minimum );
			Vector3.Subtract( ref b.Maximum, ref v, out result.Maximum );
		}

		public static void Subtract( ref Vector3 v, ref Bounds b, out Bounds result )
		{
			Vector3.Subtract( ref v, ref b.Minimum, out result.Minimum );
			Vector3.Subtract( ref v, ref b.Maximum, out result.Maximum );
		}

		public static Bounds Add( ref Bounds b, ref Vector3 v )
		{
			Bounds result;
			Vector3.Add( ref b.Minimum, ref v, out result.Minimum );
			Vector3.Add( ref b.Maximum, ref v, out result.Maximum );
			return result;
		}

		public static Bounds Add( ref Vector3 v, ref Bounds b )
		{
			Bounds result;
			Vector3.Add( ref v, ref b.Minimum, out result.Minimum );
			Vector3.Add( ref v, ref b.Maximum, out result.Maximum );
			return result;
		}

		public static Bounds Subtract( ref Bounds b, ref Vector3 v )
		{
			Bounds result;
			Vector3.Subtract( ref b.Minimum, ref v, out result.Minimum );
			Vector3.Subtract( ref b.Maximum, ref v, out result.Maximum );
			return result;
		}

		public static Bounds Subtract( ref Vector3 v, ref Bounds b )
		{
			Bounds result;
			Vector3.Subtract( ref v, ref b.Minimum, out result.Minimum );
			Vector3.Subtract( ref v, ref b.Maximum, out result.Maximum );
			return result;
		}

		public Plane.Side GetPlaneSide( ref Plane plane )
		{
			Vector3 center;
			GetCenter( out center );

			double d1 = plane.GetDistance( ref center );
			double d2 = Math.Abs( ( Maximum.X - center.X ) * plane.A ) +
				Math.Abs( ( Maximum.Y - center.Y ) * plane.B ) +
				Math.Abs( ( Maximum.Z - center.Z ) * plane.C );

			if( d1 - d2 > 0 )
				return Plane.Side.Positive;
			if( d1 + d2 < 0 )
				return Plane.Side.Negative;
			return Plane.Side.No;
		}

		public Plane.Side GetPlaneSide( Plane plane )
		{
			return GetPlaneSide( ref plane );
		}

		public double GetPlaneDistance( ref Plane plane )
		{
			Vector3 center;
			GetCenter( out center );

			double d1 = plane.GetDistance( ref center );
			double d2 = Math.Abs( ( Maximum.X - center.X ) * plane.Normal.X ) +
				Math.Abs( ( Maximum.Y - center.Y ) * plane.Normal.Y ) +
				Math.Abs( ( Maximum.Z - center.Z ) * plane.Normal.Z );

			if( d1 - d2 > 0.0 )
				return d1 - d2;
			if( d1 + d2 < 0.0 )
				return d1 + d2;
			return 0.0;
		}

		public double GetPlaneDistance( Plane plane )
		{
			return GetPlaneDistance( ref plane );
		}

		internal bool LineIntersection( ref Vector3 start, ref Vector3 end )
		{
			//unsafe
			{
				Vector3 center;
				GetCenter( out center );

				Vector3 extents;
				Vector3.Subtract( ref Maximum, ref center, out extents );
				//Vec3D extents = Maximum - center;

				Vector3 lineDir;
				lineDir.X = 0.5 * ( end.X - start.X );
				lineDir.Y = 0.5 * ( end.Y - start.Y );
				lineDir.Z = 0.5 * ( end.Z - start.Z );
				//Vec3D lineDir = 0.5f * ( end - start );

				Vector3 lineCenter;
				Vector3.Add( ref start, ref lineDir, out lineCenter );
				//Vec3D lineCenter = start + lineDir;

				Vector3 dir;
				Vector3.Subtract( ref lineCenter, ref center, out dir );
				//Vec3D dir = lineCenter - center;

				double ld0, ld1, ld2;

				ld0 = Math.Abs( lineDir.X );
				if( Math.Abs( dir.X ) > extents.X + ld0 )
					return false;
				ld1 = Math.Abs( lineDir.Y );
				if( Math.Abs( dir.Y ) > extents.Y + ld1 )
					return false;
				ld2 = Math.Abs( lineDir.Z );
				if( Math.Abs( dir.Z ) > extents.Z + ld2 )
					return false;

				Vector3 cross;
				Vector3.Cross( ref lineDir, ref dir, out cross );

				if( Math.Abs( cross.X ) > extents.Y * ld2 + extents.Z * ld1 )
					return false;
				if( Math.Abs( cross.Y ) > extents.X * ld2 + extents.Z * ld0 )
					return false;
				if( Math.Abs( cross.Z ) > extents.X * ld1 + extents.Y * ld0 )
					return false;
				return true;
			}
		}

		public Rectangle ToRectangle()
		{
			Rectangle result;
			result.Left = Minimum.X;
			result.Top = Minimum.Y;
			result.Right = Maximum.X;
			result.Bottom = Maximum.Y;
			return result;
		}

		public void ToRectangle( out Rectangle result )
		{
			result.Left = Minimum.X;
			result.Top = Minimum.Y;
			result.Right = Maximum.X;
			result.Bottom = Maximum.Y;
		}

		//public void GetClosestPoint( ref Vector3 point, out Vector3 result )
		//{
		//	result = point;
		//	MathEx.Clamp( ref result.X, Minimum.X, Maximum.X );
		//	MathEx.Clamp( ref result.Y, Minimum.Y, Maximum.Y );
		//	MathEx.Clamp( ref result.Z, Minimum.Z, Maximum.Z );
		//}

		//public Vector3 GetClosestPoint( Vector3 point )
		//{
		//	GetClosestPoint( ref point, out var result );
		//	return result;
		//}

		public double GetPointDistanceSquared( Vector3 point )
		{
			double x;
			if( point.X < Minimum.X )
				x = Minimum.X - point.X;
			else if( point.X > Maximum.X )
				x = point.X - Maximum.X;
			else
				x = 0;

			double y;
			if( point.Y < Minimum.Y )
				y = Minimum.Y - point.Y;
			else if( point.Y > Maximum.Y )
				y = point.Y - Maximum.Y;
			else
				y = 0;

			double z;
			if( point.Z < Minimum.Z )
				z = Minimum.Z - point.Z;
			else if( point.Z > Maximum.Z )
				z = point.Z - Maximum.Z;
			else
				z = 0;

			double sqr = x * x + y * y + z * z;
			return sqr;
		}

		public double GetPointDistance( Vector3 point )
		{
			double sqr = GetPointDistanceSquared( point );
			if( sqr == 0 )
				return 0;
			return Math.Sqrt( sqr );
		}

		[AutoConvertType]
		public BoundsF ToBoundsF()
		{
			BoundsF result;
			result.Minimum.X = (float)Minimum.X;
			result.Minimum.Y = (float)Minimum.Y;
			result.Minimum.Z = (float)Minimum.Z;
			result.Maximum.X = (float)Maximum.X;
			result.Maximum.Y = (float)Maximum.Y;
			result.Maximum.Z = (float)Maximum.Z;
			return result;
		}

		[AutoConvertType]
		public BoundsI ToBoundsI()
		{
			BoundsI result;
			result.Minimum.X = (int)Minimum.X;
			result.Minimum.Y = (int)Minimum.Y;
			result.Minimum.Z = (int)Minimum.Z;
			result.Maximum.X = (int)Maximum.X;
			result.Maximum.Y = (int)Maximum.Y;
			result.Maximum.Z = (int)Maximum.Z;
			return result;
		}

		public static Bounds Merge( Bounds a, Bounds b )
		{
			var v = a;
			v.Add( b );
			return v;
		}
	}
}
