// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Line2F> ) )]
	/// <summary>
	/// Represents a single precision line in two-dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Line2F
	{
		[Serialize]
		public Vector2F Start;
		[Serialize]
		public Vector2F End;

		public Line2F( Line2F source )
		{
			Start = source.Start;
			End = source.End;
		}

		public Line2F( Vector2F start, Vector2F end )
		{
			this.Start = start;
			this.End = end;
		}

		public override bool Equals( object obj )
		{
			return ( obj is Line2F && this == (Line2F)obj );
		}

		public override int GetHashCode()
		{
			return ( Start.GetHashCode() ^ End.GetHashCode() );
		}

		public static bool operator ==( Line2F v1, Line2F v2 )
		{
			return ( v1.Start == v2.Start && v1.End == v2.End );
		}

		public static bool operator !=( Line2F v1, Line2F v2 )
		{
			return ( v1.Start != v2.Start || v1.End != v2.End );
		}

		public bool Equals( Line2F v, float epsilon )
		{
			if( !Start.Equals( v.Start, epsilon ) )
				return false;
			if( !End.Equals( v.End, epsilon ) )
				return false;
			return true;
		}

		[AutoConvertType]
		public Line2 ToLine2()
		{
			Line2 result;
			result.Start = Start.ToVector2();
			result.End = End.ToVector2();
			return result;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

#if !DISABLE_IMPLICIT
		public static implicit operator Line2( Line2F v )
		{
			return new Line2( v );
		}
#endif
	}
}
