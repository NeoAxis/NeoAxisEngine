// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Ray2> ) )]
	/// <summary>
	/// Represents a double precision two-dimensional line based on a point in space and a direction.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Ray2
	{
		[Serialize]
		public Vector2 Origin;
		[Serialize]
		public Vector2 Direction;

		public Ray2( Ray2 source )
		{
			Origin = source.Origin;
			Direction = source.Direction;
		}

		public Ray2( Vector2 origin, Vector2 direction )
		{
			this.Origin = origin;
			this.Direction = direction;
		}

		public Ray2( Ray2F source )
		{
			Origin = source.Origin.ToVector2();
			Direction = source.Direction.ToVector2();
		}

		public override bool Equals( object obj )
		{
			return ( obj is Ray2 && this == (Ray2)obj );
		}

		public override int GetHashCode()
		{
			return ( Origin.GetHashCode() ^ Direction.GetHashCode() );
		}

		public static bool operator ==( Ray2 v1, Ray2 v2 )
		{
			return ( v1.Origin == v2.Origin && v1.Direction == v2.Direction );
		}

		public static bool operator !=( Ray2 v1, Ray2 v2 )
		{
			return ( v1.Origin != v2.Origin || v1.Direction != v2.Direction );
		}

		public bool Equals( Ray2 v, double epsilon )
		{
			if( !Origin.Equals( v.Origin, epsilon ) )
				return false;
			if( !Direction.Equals( v.Direction, epsilon ) )
				return false;
			return true;
		}

		public Vector2 GetPointOnRay( double t )
		{
			Vector2 result;
			result.X = Origin.X + Direction.X * t;
			result.Y = Origin.Y + Direction.Y * t;
			return result;
		}

		public void GetPointOnRay( double t, out Vector2 result )
		{
			result.X = Origin.X + Direction.X * t;
			result.Y = Origin.Y + Direction.Y * t;
		}

		[AutoConvertType]
		public Ray2F ToRay2F()
		{
			Ray2F result;
			result.Origin = Origin.ToVector2F();
			result.Direction = Direction.ToVector2F();
			return result;
		}

		public Vector2 GetEndPoint()
		{
			return Origin + Direction;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString
	}
}
