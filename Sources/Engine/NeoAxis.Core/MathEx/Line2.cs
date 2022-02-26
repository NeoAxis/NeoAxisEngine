// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Line2> ) )]
	/// <summary>
	/// Represents a double precision line in two-dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Line2
	{
		[Serialize]
		public Vector2 Start;
		[Serialize]
		public Vector2 End;

		public Line2( Line2 source )
		{
			Start = source.Start;
			End = source.End;
		}

		public Line2( Vector2 start, Vector2 end )
		{
			this.Start = start;
			this.End = end;
		}

		public override bool Equals( object obj )
		{
			return ( obj is Line2 && this == (Line2)obj );
		}

		public override int GetHashCode()
		{
			return ( Start.GetHashCode() ^ End.GetHashCode() );
		}

		public static bool operator ==( Line2 v1, Line2 v2 )
		{
			return ( v1.Start == v2.Start && v1.End == v2.End );
		}

		public static bool operator !=( Line2 v1, Line2 v2 )
		{
			return ( v1.Start != v2.Start || v1.End != v2.End );
		}

		public bool Equals( Line2 v, double epsilon )
		{
			if( !Start.Equals( v.Start, epsilon ) )
				return false;
			if( !End.Equals( v.End, epsilon ) )
				return false;
			return true;
		}

		[AutoConvertType]
		public Line2F ToLine2F()
		{
			Line2F result;
			result.Start = Start.ToVector2F();
			result.End = End.ToVector2F();
			return result;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

	}
}
