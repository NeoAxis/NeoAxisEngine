// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Bezier curve.
	/// </summary>
	public class CurveBezier : Curve
	{
		public unsafe override Vector3 CalculateValueByTime( double time )
		{
			double* bvals = stackalloc double[ points.Count ];

			Basis( points.Count, time, bvals, 0 );
			Vector3 v = bvals[ 0 ] * points[ 0 ].value;
			for( int i = 1; i < points.Count; i++ )
				v += bvals[ i ] * points[ i ].value;
			return v;
		}

		public unsafe override Vector3 GetCurrentFirstDerivative( double time )
		{
			int i;
			double d;
			Vector3 v;

			double* bvals = stackalloc double[ points.Count ];

			BasisFirstDerivative( points.Count, time, bvals, 0 );
			v = bvals[ 0 ] * points[ 0 ].value;
			for( i = 1; i < points.Count; i++ )
				v += bvals[ i ] * points[ i ].value;
			d = ( points[ points.Count - 1 ].time - points[ 0 ].time );

			return ( (double)( points.Count - 1 ) / d ) * v;
		}

		unsafe void Basis( int order, double t, double* bvals, int bvalsOffset )
		{
			int i, j, d;
			double c1, c2, s, o, ps, po;

			bvals[ 0 + bvalsOffset ] = 1.0f;
			d = order - 1;
			if( d <= 0 )
				return;

			double* c = stackalloc double[ d + 1 ];

			s = (double)( t - points[ 0 ].time ) / ( points[ points.Count - 1 ].time - points[ 0 ].time );
			o = 1.0f - s;
			ps = s;
			po = o;

			for( i = 1; i < d; i++ )
				c[ i ] = 1.0f;

			for( i = 1; i < d; i++ )
			{
				c[ i - 1 ] = 0.0f;
				c1 = c[ i ];
				c[ i ] = 1.0f;
				for( j = i + 1; j <= d; j++ )
				{
					c2 = c[ j ];
					c[ j ] = c1 + c[ j - 1 ];
					c1 = c2;
				}
				bvals[ i + bvalsOffset ] = c[ d ] * ps;
				ps *= s;
			}

			for( i = d - 1; i >= 0; i-- )
			{
				bvals[ i + bvalsOffset ] *= po;
				po *= o;
			}
			bvals[ d + bvalsOffset ] = ps;
		}

		unsafe void BasisFirstDerivative( int order, double t, double* bvals, int bvalsOffset )
		{
			Basis( order - 1, t, bvals, 1 + bvalsOffset );
			bvals[ 0 + bvalsOffset ] = 0.0f;
			for( int i = 0; i < order - 1; i++ )
				bvals[ i + bvalsOffset ] -= bvals[ i + 1 + bvalsOffset ];
		}
	}
}
