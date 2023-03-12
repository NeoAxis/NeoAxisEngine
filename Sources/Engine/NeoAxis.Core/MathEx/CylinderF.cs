// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<CylinderF> ) )]
	/// <summary>
	/// Represents a single precision cylinder shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct CylinderF
	{
		[Serialize]
		public Vector3F Point1;
		[Serialize]
		public Vector3F Point2;
		[Serialize]
		public float Radius;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public CylinderF( CylinderF source )
		{
			Point1 = source.Point1;
			Point2 = source.Point2;
			Radius = source.Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public CylinderF( Vector3F point1, Vector3F point2, float radius )
		{
			this.Point1 = point1;
			this.Point2 = point2;
			this.Radius = radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is CylinderF && this == (CylinderF)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Point1.GetHashCode() ^ Point2.GetHashCode() ^ Radius.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( CylinderF v1, CylinderF v2 )
		{
			return ( v1.Point1 == v2.Point1 && v1.Point2 == v2.Point2 && v1.Radius == v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( CylinderF v1, CylinderF v2 )
		{
			return ( v1.Point1 != v2.Point1 || v1.Point2 != v2.Point2 || v1.Radius != v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( CylinderF v, float epsilon )
		{
			if( !Point1.Equals( ref v.Point1, epsilon ) )
				return false;
			if( !Point2.Equals( ref v.Point2, epsilon ) )
				return false;
			if( Math.Abs( Radius - v.Radius ) > epsilon )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToBounds( out BoundsF result )
		{
			Vector3F a = Point2 - Point1;
			var dotA = Vector3F.Dot( a, a );
			if( dotA == 0 )
				dotA = MathEx.Epsilon;

			Vector3F e = new Vector3F(
				Radius * MathEx.Sqrt( 1.0f - a.X * a.X / dotA ),
				Radius * MathEx.Sqrt( 1.0f - a.Y * a.Y / dotA ),
				Radius * MathEx.Sqrt( 1.0f - a.Z * a.Z / dotA ) );

			result = new BoundsF( Vector3F.Min( Point1 - e, Point2 - e ), Vector3F.Max( Point1 + e, Point2 + e ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BoundsF ToBounds()
		{
			ToBounds( out var result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3F GetCenter()
		{
			Vector3F result;
			Vector3F.Add( ref Point1, ref Point2, out result );
			result.X *= .5f;
			result.Y *= .5f;
			result.Z *= .5f;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetCenter( out Vector3F result )
		{
			Vector3F.Add( ref Point1, ref Point2, out result );
			result.X *= .5f;
			result.Y *= .5f;
			result.Z *= .5f;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetLength()
		{
			Vector3F result;
			Vector3F.Subtract( ref Point2, ref Point1, out result );
			return result.Length();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3F GetDirection()
		{
			Vector3F result;
			Vector3F.Subtract( ref Point2, ref Point1, out result );
			result.Normalize();
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetDirection( out Vector3F result )
		{
			Vector3F.Subtract( ref Point2, ref Point1, out result );
			result.Normalize();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Cylinder ToCylinder()
		{
			Cylinder result;
			result.Point1 = Point1.ToVector3();
			result.Point2 = Point2.ToVector3();
			result.Radius = Radius;
			return result;
		}

		//public bool IsContainsPoint( Vec3F point )
		//{
		//	if( point1 != point2 )
		//	{
		//		float radiusSquared = radius * radius;

		//		if( ( point1 - point ).LengthSqr() <= radiusSquared )
		//			return true;
		//		if( ( point2 - point ).LengthSqr() <= radiusSquared )
		//			return true;

		//		Vec3F projectPoint;
		//		MathAlgorithms.ProjectPointToLine( ref point1, ref point2, ref point, out projectPoint );

		//		BoundsF pointsBounds = new BoundsF( point1 );
		//		pointsBounds.Add( point2 );

		//		if( pointsBounds.IsContainsPoint( projectPoint ) )
		//		{
		//			if( ( projectPoint - point ).LengthSqr() <= radiusSquared )
		//				return true;
		//		}

		//		return false;
		//	}
		//	else
		//	{
		//		return new SphereF( point1, radius ).ContainsPoint( point );
		//	}
		//}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString


#if !DISABLE_IMPLICIT
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Cylinder( CylinderF v )
		{
			return new Cylinder( v );
		}
#endif
	}
}
