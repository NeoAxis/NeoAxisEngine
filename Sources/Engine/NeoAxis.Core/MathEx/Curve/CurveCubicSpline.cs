// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a cubic curve.
	/// </summary>
	public class CurveCubicSpline : CurveSpline
	{
		public unsafe override Vector3 CalculateValueByTime( double time )
		{
			if( points.Count == 1 )
				return points[ 0 ].value;

			double* bvals = stackalloc double[ 4 ];

			double clampedTime = GetClampedTime( time );
			int i = GetIndexForTime( clampedTime );
			Basis( i - 1, clampedTime, bvals );
			Vector3 v = Vector3.Zero;
			for( int j = 0; j < 4; j++ )
			{
				int k = i + j - 2;
				v += bvals[ j ] * GetValueForIndex( k );
			}
			return v;
		}

		unsafe void Basis( int index, double t, double* bvals )
		{
			double s = ( t - GetTimeForIndex( index ) ) / ( GetTimeForIndex( index + 1 ) - GetTimeForIndex( index ) );
			bvals[ 0 ] = ( ( -s + 2.0f ) * s - 1.0f ) * s * .5f;
			bvals[ 1 ] = ( ( ( 3.0f * s - 5.0f ) * s ) * s + 2.0f ) * .5f;
			bvals[ 2 ] = ( ( -3.0f * s + 4.0f ) * s + 1.0f ) * s * .5f;
			bvals[ 3 ] = ( ( s - 1.0f ) * s * s ) * .5f;
		}
	}
}
