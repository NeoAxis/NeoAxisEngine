// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Cylinder> ) )]
	/// <summary>
	/// Represents a double precision cylinder shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Cylinder
	{
		[Serialize]
		public Vector3 Point1;
		[Serialize]
		public Vector3 Point2;
		[Serialize]
		public double Radius;

		public Cylinder( Cylinder source )
		{
			Point1 = source.Point1;
			Point2 = source.Point2;
			Radius = source.Radius;
		}

		public Cylinder( Vector3 point1, Vector3 point2, double radius )
		{
			this.Point1 = point1;
			this.Point2 = point2;
			this.Radius = radius;
		}

		public override bool Equals( object obj )
		{
			return ( obj is Cylinder && this == (Cylinder)obj );
		}

		public override int GetHashCode()
		{
			return ( Point1.GetHashCode() ^ Point2.GetHashCode() ^ Radius.GetHashCode() );
		}

		public static bool operator ==( Cylinder v1, Cylinder v2 )
		{
			return ( v1.Point1 == v2.Point1 && v1.Point2 == v2.Point2 && v1.Radius == v2.Radius );
		}

		public static bool operator !=( Cylinder v1, Cylinder v2 )
		{
			return ( v1.Point1 != v2.Point1 || v1.Point2 != v2.Point2 || v1.Radius != v2.Radius );
		}

		public bool Equals( Cylinder v, double epsilon )
		{
			if( !Point1.Equals( ref v.Point1, epsilon ) )
				return false;
			if( !Point2.Equals( ref v.Point2, epsilon ) )
				return false;
			if( Math.Abs( Radius - v.Radius ) > epsilon )
				return false;
			return true;
		}

		public void ToBounds( out Bounds result )
		{
			Vector3 a = Point2 - Point1;
			var dotA = Vector3.Dot( a, a );
			if( dotA == 0 )
				dotA = MathEx.Epsilon;

			Vector3 e = new Vector3(
				Radius * Math.Sqrt( 1.0 - a.X * a.X / dotA ),
				Radius * Math.Sqrt( 1.0 - a.Y * a.Y / dotA ),
				Radius * Math.Sqrt( 1.0 - a.Z * a.Z / dotA ) );

			result = new Bounds( Vector3.Min( Point1 - e, Point2 - e ), Vector3.Max( Point1 + e, Point2 + e ) );
		}

		public Bounds ToBounds()
		{
			ToBounds( out var result );
			return result;
		}

		public Vector3 GetCenter()
		{
			Vector3 result;
			Vector3.Add( ref Point1, ref Point2, out result );
			result.X *= .5;
			result.Y *= .5;
			result.Z *= .5;
			return result;
		}

		public void GetCenter( out Vector3 result )
		{
			Vector3.Add( ref Point1, ref Point2, out result );
			result.X *= .5;
			result.Y *= .5;
			result.Z *= .5;
		}

		public double GetLength()
		{
			Vector3 result;
			Vector3.Subtract( ref Point2, ref Point1, out result );
			return result.Length();
		}

		public Vector3 GetDirection()
		{
			Vector3 result;
			Vector3.Subtract( ref Point2, ref Point1, out result );
			result.Normalize();
			return result;
		}

		public void GetDirection( out Vector3 result )
		{
			Vector3.Subtract( ref Point2, ref Point1, out result );
			result.Normalize();
		}

		//public bool Contains( Vector3 point )
		//{
		//	if( Point1 != Point2 )
		//	{
		//		xx xx;

		//		double radiusSquared = Radius * Radius;

		//		if( ( Point1 - point ).LengthSquared() <= radiusSquared )
		//			return true;
		//		if( ( Point2 - point ).LengthSquared() <= radiusSquared )
		//			return true;

		//		Vector3 projectPoint;
		//		MathAlgorithms.ProjectPointToLine( ref Point1, ref Point2, ref point, out projectPoint );

		//		Bounds pointsBounds = new Bounds( Point1 );
		//		pointsBounds.Add( Point2 );

		//		if( pointsBounds.Contains( projectPoint ) )
		//		{
		//			if( ( projectPoint - point ).LengthSquared() <= radiusSquared )
		//				return true;
		//		}

		//	}
		//	return false;
		//}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

	}
}
