// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Ray( Ray source )
		{
			Origin = source.Origin;
			Direction = source.Direction;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Ray( Vector3 origin, Vector3 direction )
		{
			this.Origin = origin;
			this.Direction = direction;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Ray( RayF source )
		{
			Origin = source.Origin.ToVector3();
			Direction = source.Direction.ToVector3();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Ray && this == (Ray)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Origin.GetHashCode() ^ Direction.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Ray v1, Ray v2 )
		{
			return ( v1.Origin == v2.Origin && v1.Direction == v2.Direction );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Ray v1, Ray v2 )
		{
			return ( v1.Origin != v2.Origin || v1.Direction != v2.Direction );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Ray v, double epsilon )
		{
			if( !Origin.Equals( v.Origin, epsilon ) )
				return false;
			if( !Direction.Equals( v.Direction, epsilon ) )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 GetPointOnRay( double t )
		{
			Vector3 result;
			result.X = Origin.X + Direction.X * t;
			result.Y = Origin.Y + Direction.Y * t;
			result.Z = Origin.Z + Direction.Z * t;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetPointOnRay( double t, out Vector3 result )
		{
			result.X = Origin.X + Direction.X * t;
			result.Y = Origin.Y + Direction.Y * t;
			result.Z = Origin.Z + Direction.Z * t;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public RayF ToRayF()
		{
			RayF result;
			result.Origin = Origin.ToVector3F();
			result.Direction = Direction.ToVector3F();
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Ray2 ToRay2()
		{
			Ray2 result;
			result.Origin = Origin.ToVector2();
			result.Direction = Direction.ToVector2();
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 GetEndPoint()
		{
			return Origin + Direction;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString
	}
}
