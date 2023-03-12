// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a curve with flat lines.
	/// </summary>
	public class CurveLine1 : CurveSpline1
	{
		[MethodImpl( (MethodImplOptions)512 )]
		public override double CalculateValueByTime( double time )
		{
			if( points.Count == 1 )
				return points[ 0 ].value;

			var clampedTime = GetClampedTime( time );
			int i = GetIndexForTime( clampedTime );
			if( i == 0 )
				return GetValueForIndex( i );
			if( i >= points.Count )
				return GetValueForIndex( points.Count - 1 );

			var from = GetValueForIndex( i - 1 );
			var to = GetValueForIndex( i );
			var length = points[ i ].time - points[ i - 1 ].time;
			if( length == 0 )
				return from;

			var diffCoef = ( time - points[ i - 1 ].time ) / length;
			var v = from + diffCoef * ( to - from );
			return v;
		}
	}
}
