// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Capsule2F> ) )]
	/// <summary>
	/// Represents a single precision 2D capsule shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Capsule2F
	{
		[Serialize]
		public Vector2F Point1;
		[Serialize]
		public Vector2F Point2;
		[Serialize]
		public float Radius;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Capsule2F( Capsule2F source )
		{
			Point1 = source.Point1;
			Point2 = source.Point2;
			Radius = source.Radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Capsule2F( Vector2F point1, Vector2F point2, float radius )
		{
			this.Point1 = point1;
			this.Point2 = point2;
			this.Radius = radius;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Capsule2F && this == (Capsule2F)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Point1.GetHashCode() ^ Point2.GetHashCode() ^ Radius.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Capsule2F v1, Capsule2F v2 )
		{
			return ( v1.Point1 == v2.Point1 && v1.Point2 == v2.Point2 && v1.Radius == v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Capsule2F v1, Capsule2F v2 )
		{
			return ( v1.Point1 != v2.Point1 || v1.Point2 != v2.Point2 || v1.Radius != v2.Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Capsule2F v, float epsilon )
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
		public Rectangle ToBounds()
		{
			Rectangle result = new Rectangle( Point1 );
			result.Add( Point2 );
			result.Expand( Radius );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToBounds( out Rectangle result )
		{
			result = new Rectangle( Point1 );
			result.Add( Point2 );
			result.Expand( Radius );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2F GetCenter()
		{
			Vector2F result;
			Vector2F.Add( ref Point1, ref Point2, out result );
			result.X *= .5f;
			result.Y *= .5f;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetCenter( out Vector2F result )
		{
			Vector2F.Add( ref Point1, ref Point2, out result );
			result.X *= .5f;
			result.Y *= .5f;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float GetLength()
		{
			Vector2F result;
			Vector2F.Subtract( ref Point2, ref Point1, out result );
			return result.Length();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2F GetDirection()
		{
			Vector2F result;
			Vector2F.Subtract( ref Point2, ref Point1, out result );
			result.Normalize();
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetDirection( out Vector2F result )
		{
			Vector2F.Subtract( ref Point2, ref Point1, out result );
			result.Normalize();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static bool CircleContains( ref Vector2F center, float radius, ref Vector2F p )
		{
			float x = p.X - center.X;
			float y = p.Y - center.Y;
			float lengthSqr = x * x + y * y;
			if( lengthSqr > radius * radius )
				return false;
			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public bool Contains( Vector2F point )
		{
			if( Point1 != Point2 )
			{
				float radiusSquared = Radius * Radius;

				if( ( Point1 - point ).LengthSquared() <= radiusSquared )
					return true;
				if( ( Point2 - point ).LengthSquared() <= radiusSquared )
					return true;

				Vector2F projectPoint;
				MathAlgorithms.ProjectPointToLine( ref Point1, ref Point2, ref point, out projectPoint );

				Rectangle pointsBounds = new Rectangle( Point1 );
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
				return CircleContains( ref Point1, Radius, ref point );
				//return new Sphere( Point1, Radius ).Contains( point );
			}
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

	}
}
