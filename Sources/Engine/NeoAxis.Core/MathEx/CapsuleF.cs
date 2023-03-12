// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<CapsuleF> ) )]
	/// <summary>
	/// Represents a single precision capsule shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct CapsuleF
	{
		[Serialize]
		internal Vector3F Point1;
		[Serialize]
		internal Vector3F Point2;
		[Serialize]
		internal float Radius;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public CapsuleF( CapsuleF source )
		{
			Point1 = source.Point1;
			Point2 = source.Point2;
			Radius = source.Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public CapsuleF( Vector3F point1, Vector3F point2, float radius )
		{
			this.Point1 = point1;
			this.Point2 = point2;
			this.Radius = radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is CapsuleF && this == (CapsuleF)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Point1.GetHashCode() ^ Point2.GetHashCode() ^ Radius.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( CapsuleF v1, CapsuleF v2 )
		{
			return ( v1.Point1 == v2.Point1 && v1.Point2 == v2.Point2 && v1.Radius == v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( CapsuleF v1, CapsuleF v2 )
		{
			return ( v1.Point1 != v2.Point1 || v1.Point2 != v2.Point2 || v1.Radius != v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( CapsuleF v, float epsilon )
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
		public BoundsF ToBounds()
		{
			BoundsF result = new BoundsF( Point1 );
			result.Add( Point2 );
			result.Expand( Radius );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToBounds( out BoundsF result )
		{
			result = new BoundsF( Point1 );
			result.Add( Point2 );
			result.Expand( Radius );
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
		public Capsule ToCapsule()
		{
			Capsule result;
			result.Point1 = Point1.ToVector3();
			result.Point2 = Point2.ToVector3();
			result.Radius = Radius;
			return result;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public bool Contains( Vector3F point )
		{
			if( Point1 != Point2 )
			{
				float radiusSquared = Radius * Radius;

				if( ( Point1 - point ).LengthSquared() <= radiusSquared )
					return true;
				if( ( Point2 - point ).LengthSquared() <= radiusSquared )
					return true;

				Vector3F projectPoint;
				MathAlgorithms.ProjectPointToLine( ref Point1, ref Point2, ref point, out projectPoint );

				BoundsF pointsBounds = new BoundsF( Point1 );
				pointsBounds.Add( Point2 );

				if( pointsBounds.Contains( projectPoint ) )
				{
					if( ( projectPoint - point ).LengthSquared() <= radiusSquared )
						return true;
				}

				return false;
			}
			else
			{
				return new SphereF( Point1, Radius ).Contains( point );
			}
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString


#if !DISABLE_IMPLICIT
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Capsule( CapsuleF v )
		{
			return new Capsule( v );
		}
#endif
	}
}
