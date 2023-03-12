// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Ray2F> ) )]
	/// <summary>
	/// Represents a single precision two-dimensional line based on a point in space and a direction.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Ray2F
	{
		[Serialize]
		public Vector2F Origin;
		[Serialize]
		public Vector2F Direction;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Ray2F( Ray2F source )
		{
			Origin = source.Origin;
			Direction = source.Direction;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Ray2F( Vector2F origin, Vector2F direction )
		{
			this.Origin = origin;
			this.Direction = direction;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Ray2F && this == (Ray2F)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Origin.GetHashCode() ^ Direction.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Ray2F v1, Ray2F v2 )
		{
			return ( v1.Origin == v2.Origin && v1.Direction == v2.Direction );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Ray2F v1, Ray2F v2 )
		{
			return ( v1.Origin != v2.Origin || v1.Direction != v2.Direction );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Ray2F v, float epsilon )
		{
			if( !Origin.Equals( v.Origin, epsilon ) )
				return false;
			if( !Direction.Equals( v.Direction, epsilon ) )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2F GetPointOnRay( float t )
		{
			Vector2F result;
			result.X = Origin.X + Direction.X * t;
			result.Y = Origin.Y + Direction.Y * t;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetPointOnRay( float t, out Vector2F result )
		{
			result.X = Origin.X + Direction.X * t;
			result.Y = Origin.Y + Direction.Y * t;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Ray2 ToRay()
		{
			Ray2 result;
			result.Origin = Origin.ToVector2();
			result.Direction = Direction.ToVector2();
			return result;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

#if !DISABLE_IMPLICIT
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Ray( Ray2F v )
		{
			return new Ray( v );
		}
#endif

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2F GetEndPoint()
		{
			return Origin + Direction;
		}

	}
}
