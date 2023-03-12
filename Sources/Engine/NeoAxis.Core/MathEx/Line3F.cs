// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Line3F> ) )]
	/// <summary>
	/// Represents a single precision line in three-dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Line3F
	{
		[Serialize]
		public Vector3F Start;
		[Serialize]
		public Vector3F End;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Line3F( Line3F source )
		{
			Start = source.Start;
			End = source.End;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Line3F( Vector3F start, Vector3F end )
		{
			this.Start = start;
			this.End = end;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Line3F && this == (Line3F)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Start.GetHashCode() ^ End.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Line3F v1, Line3F v2 )
		{
			return ( v1.Start == v2.Start && v1.End == v2.End );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Line3F v1, Line3F v2 )
		{
			return ( v1.Start != v2.Start || v1.End != v2.End );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Line3F v, float epsilon )
		{
			if( !Start.Equals( v.Start, epsilon ) )
				return false;
			if( !End.Equals( v.End, epsilon ) )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Line3 ToLine()
		{
			Line3 result;
			result.Start = Start.ToVector3();
			result.End = End.ToVector3();
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Line2F ToLine2F()
		{
			return new Line2F( Start.ToVector2(), End.ToVector2() );
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString


#if !DISABLE_IMPLICIT
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Line3( Line3F v )
		{
			return new Line3( v.Start, v.End );
		}
#endif
	}
}
