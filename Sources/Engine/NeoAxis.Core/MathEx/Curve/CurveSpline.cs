// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Base class of spline curves.
	/// </summary>
	public class CurveSpline : Curve
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected void GetValueForIndex( int index, out Vector3 result )
		{
			int n = points.Count - 1;

			if( index < 0 )
				result = points[ 0 ].value + index * ( points[ 1 ].value - points[ 0 ].value );
			else if( index > n )
				result = points[ n ].value + ( index - n ) * ( points[ n ].value - points[ n - 1 ].value );
			else
				result = points[ index ].value;
		}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//protected Vector3 GetValueForIndex( int index )
		//{
		//	int n = points.Count - 1;

		//	if( index < 0 )
		//		return points[ 0 ].value + index * ( points[ 1 ].value - points[ 0 ].value );
		//	else if( index > n )
		//		return points[ n ].value + ( index - n ) * ( points[ n ].value - points[ n - 1 ].value );
		//	return points[ index ].value;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected double GetTimeForIndex( int index )
		{
			int n = points.Count - 1;

			if( index < 0 )
				return points[ 0 ].time + index * ( points[ 1 ].time - points[ 0 ].time );
			else if( index > n )
				return points[ n ].time + ( index - n ) * ( points[ n ].time - points[ n - 1 ].time );
			return points[ index ].time;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected double GetClampedTime( double t )
		{
			return t;
		}
	}
}
