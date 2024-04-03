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
	public class CurveLine : CurveSpline
	{
		[MethodImpl( (MethodImplOptions)512 )]
		public override void CalculateValueByTime( double time, out Vector3 result )
		{
			if( points.Count == 1 )
			{
				result = points[ 0 ].value;
				return;
			}

			double clampedTime = GetClampedTime( time );
			int i = GetIndexForTime( clampedTime );
			if( i == 0 )
			{
				GetValueForIndex( i, out result );
				return;
			}
			if( i >= points.Count )
			{
				GetValueForIndex( points.Count - 1, out result );
				return;
			}

			GetValueForIndex( i - 1, out var from );
			GetValueForIndex( i, out var to );
			double length = points[ i ].time - points[ i - 1 ].time;
			if( length == 0 )
			{
				result = from;
				return;
			}

			double diffCoef = ( time - points[ i - 1 ].time ) / length;
			result = from + diffCoef * ( to - from );
		}

		//[MethodImpl( (MethodImplOptions)512 )]
		//public override Vector3 CalculateValueByTime( double time )
		//{
		//	if( points.Count == 1 )
		//		return points[ 0 ].value;

		//	double clampedTime = GetClampedTime( time );
		//	int i = GetIndexForTime( clampedTime );
		//	if( i == 0 )
		//		return GetValueForIndex( i );
		//	if( i >= points.Count )
		//		return GetValueForIndex( points.Count - 1 );

		//	Vector3 from = GetValueForIndex( i - 1 );
		//	Vector3 to = GetValueForIndex( i );
		//	double length = points[ i ].time - points[ i - 1 ].time;
		//	if( length == 0 )
		//		return from;

		//	double diffCoef = ( time - points[ i - 1 ].time ) / length;
		//	Vector3 v = from + diffCoef * ( to - from );
		//	return v;
		//}
	}
}
