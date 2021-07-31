// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Capsule2> ) )]
	/// <summary>
	/// Represents a double precision 2D capsule shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Capsule2
	{
		[Serialize]
		public Vector2 Point1;
		[Serialize]
		public Vector2 Point2;
		[Serialize]
		public double Radius;

		public Capsule2( Capsule2 source )
		{
			Point1 = source.Point1;
			Point2 = source.Point2;
			Radius = source.Radius;
		}

		public Capsule2( Vector2 point1, Vector2 point2, double radius )
		{
			this.Point1 = point1;
			this.Point2 = point2;
			this.Radius = radius;
		}

		public override bool Equals( object obj )
		{
			return ( obj is Capsule2 && this == (Capsule2)obj );
		}

		public override int GetHashCode()
		{
			return ( Point1.GetHashCode() ^ Point2.GetHashCode() ^ Radius.GetHashCode() );
		}

		public static bool operator ==( Capsule2 v1, Capsule2 v2 )
		{
			return ( v1.Point1 == v2.Point1 && v1.Point2 == v2.Point2 && v1.Radius == v2.Radius );
		}

		public static bool operator !=( Capsule2 v1, Capsule2 v2 )
		{
			return ( v1.Point1 != v2.Point1 || v1.Point2 != v2.Point2 || v1.Radius != v2.Radius );
		}

		public bool Equals( Capsule2 v, double epsilon )
		{
			if( !Point1.Equals( ref v.Point1, epsilon ) )
				return false;
			if( !Point2.Equals( ref v.Point2, epsilon ) )
				return false;
			if( Math.Abs( Radius - v.Radius ) > epsilon )
				return false;
			return true;
		}

		public Rectangle ToBounds()
		{
			Rectangle result = new Rectangle( Point1 );
			result.Add( Point2 );
			result.Expand( Radius );
			return result;
		}

		public void ToBounds( out Rectangle result )
		{
			result = new Rectangle( Point1 );
			result.Add( Point2 );
			result.Expand( Radius );
		}

		public Vector2 GetCenter()
		{
			Vector2 result;
			Vector2.Add( ref Point1, ref Point2, out result );
			result.X *= .5;
			result.Y *= .5;
			return result;
		}

		public void GetCenter( out Vector2 result )
		{
			Vector2.Add( ref Point1, ref Point2, out result );
			result.X *= .5;
			result.Y *= .5;
		}

		public double GetLength()
		{
			Vector2 result;
			Vector2.Subtract( ref Point2, ref Point1, out result );
			return result.Length();
		}

		public Vector2 GetDirection()
		{
			Vector2 result;
			Vector2.Subtract( ref Point2, ref Point1, out result );
			result.Normalize();
			return result;
		}

		public void GetDirection( out Vector2 result )
		{
			Vector2.Subtract( ref Point2, ref Point1, out result );
			result.Normalize();
		}

		static bool CircleContains( ref Vector2 center, double radius, ref Vector2 p )
		{
			double x = p.X - center.X;
			double y = p.Y - center.Y;
			double lengthSqr = x * x + y * y;
			if( lengthSqr > radius * radius )
				return false;
			return true;
		}

		public bool Contains( Vector2 point )
		{
			if( Point1 != Point2 )
			{
				double radiusSquared = Radius * Radius;

				if( ( Point1 - point ).LengthSquared() <= radiusSquared )
					return true;
				if( ( Point2 - point ).LengthSquared() <= radiusSquared )
					return true;

				Vector2 projectPoint;
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
