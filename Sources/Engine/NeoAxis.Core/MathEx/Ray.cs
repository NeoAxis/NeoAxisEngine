// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Ray> ) )]
	/// <summary>
	/// Represents a double precision three-dimensional line based on a point in space and a direction.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Ray
	{
		[Serialize]
		public Vector3 Origin;
		[Serialize]
		public Vector3 Direction;

		public Ray( Ray source )
		{
			Origin = source.Origin;
			Direction = source.Direction;
		}

		public Ray( Vector3 origin, Vector3 direction )
		{
			this.Origin = origin;
			this.Direction = direction;
		}

		public Ray( RayF source )
		{
			Origin = source.Origin.ToVector3();
			Direction = source.Direction.ToVector3();
		}

		public override bool Equals( object obj )
		{
			return ( obj is Ray && this == (Ray)obj );
		}

		public override int GetHashCode()
		{
			return ( Origin.GetHashCode() ^ Direction.GetHashCode() );
		}

		public static bool operator ==( Ray v1, Ray v2 )
		{
			return ( v1.Origin == v2.Origin && v1.Direction == v2.Direction );
		}

		public static bool operator !=( Ray v1, Ray v2 )
		{
			return ( v1.Origin != v2.Origin || v1.Direction != v2.Direction );
		}

		public bool Equals( Ray v, double epsilon )
		{
			if( !Origin.Equals( v.Origin, epsilon ) )
				return false;
			if( !Direction.Equals( v.Direction, epsilon ) )
				return false;
			return true;
		}

		public Vector3 GetPointOnRay( double t )
		{
			Vector3 result;
			result.X = Origin.X + Direction.X * t;
			result.Y = Origin.Y + Direction.Y * t;
			result.Z = Origin.Z + Direction.Z * t;
			return result;
		}

		public void GetPointOnRay( double t, out Vector3 result )
		{
			result.X = Origin.X + Direction.X * t;
			result.Y = Origin.Y + Direction.Y * t;
			result.Z = Origin.Z + Direction.Z * t;
		}

		[AutoConvertType]
		public RayF ToRayF()
		{
			RayF result;
			result.Origin = Origin.ToVector3F();
			result.Direction = Direction.ToVector3F();
			return result;
		}

		[AutoConvertType]
		public Ray2 ToRay2()
		{
			Ray2 result;
			result.Origin = Origin.ToVector2();
			result.Direction = Direction.ToVector2();
			return result;
		}

		public Vector3 GetEndPoint()
		{
			return Origin + Direction;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString
	}
}
