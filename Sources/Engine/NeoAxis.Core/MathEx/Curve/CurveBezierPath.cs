// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a path of Bezier curves.
	/// </summary>
	public class CurveBezierPath : Curve
	{
		public int AddPoint( double time, Vector3 value, Vector3 handle1, Vector3 handle2 )
		{
			return AddPoint( new Point( time, value, (handle1, handle2) ) );
		}

		public unsafe override Vector3 CalculateValueByTime( double time )
		{
			//!!!!slowly
			//!!!!искать делением пополам. или метод встроенный

			for( int nInterval = 0; nInterval < Points.Count - 1; nInterval++ )
			{
				var p1 = Points[ nInterval ];
				var p2 = Points[ nInterval + 1 ];

				if( time >= p1.Time && time < p2.Time )
				{
					if( p1.Time == p2.Time )
						return p1.Value;

					var p1Handles = ((Vector3, Vector3))p1.AdditionalData;
					var p2Handles = ((Vector3, Vector3))p2.AdditionalData;

					//!!!!GC
					var bezier = new CurveBezier();
					bezier.AddPoint( 0, p1.Value );
					bezier.AddPoint( 1.0 / 3.0, p1Handles.Item2 );
					bezier.AddPoint( 2.0 / 3.0, p2Handles.Item1 );
					bezier.AddPoint( 1, p2.Value );

					var t = ( time - p1.Time ) / ( p2.Time - p1.Time );

					return bezier.CalculateValueByTime( t );
				}
			}

			if( Points.Count != 0 )
				return Points[ Points.Count - 1 ].Value;
			else
				return Vector3.Zero;
		}

		//public unsafe override Vector3 GetCurrentFirstDerivative( double time )
		//{
		//}
	}
}
