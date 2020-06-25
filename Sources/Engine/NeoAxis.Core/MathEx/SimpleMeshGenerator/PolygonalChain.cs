// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NeoAxis
{
	public static partial class SimpleMeshGenerator
	{
		public static void GeneratePolygonalChain( Vector3[] points, double radius, out Vector3[] positions, out int[] indices )
		{
			if( points.Length < 2 )
			{
				positions = new Vector3[ 0 ];
				indices = new int[ 0 ];
				return;
			}

			int lineCount = points.Length - 1;
			int triangleCount = lineCount * 8;
			positions = new Vector3[ points.Length * 4 ];
			indices = new int[ triangleCount * 3 ];

			for( int nPoint = 0; nPoint < points.Length; nPoint++ )
			{
				int positionOffset = nPoint * 4;
				Vector3 point = points[ nPoint ];

				//!!!!new code
				//!!!!!!what be when direction == Zero?
				Quaternion rotation = Quaternion.Identity;
				{
					Vector3 direction;
					if( nPoint == 0 )
						direction = points[ 1 ] - points[ 0 ];
					else
						direction = points[ nPoint ] - points[ nPoint - 1 ];

					if( direction != Vector3.Zero )
						rotation = Quaternion.FromDirectionZAxisUp( direction.GetNormalize() );
				}

				for( int nAngleStep = 0; nAngleStep < 4; nAngleStep++ )
				{
					double angle = ( (double)nAngleStep / 4.0f ) * MathEx.PI * 2.0f;
					Vector3 pos = point + rotation * ( new Vector3( 0, Math.Sin( angle ), Math.Cos( angle ) ) * radius );
					positions[ positionOffset + nAngleStep ] = pos;
				}
			}

			for( int nLine = 0; nLine < lineCount; nLine++ )
			{
				int indexOffset = nLine * 24;

				indices[ indexOffset + 0 ] = nLine * 4 + 0;
				indices[ indexOffset + 1 ] = nLine * 4 + 4;
				indices[ indexOffset + 2 ] = nLine * 4 + 5;
				indices[ indexOffset + 3 ] = nLine * 4 + 5;
				indices[ indexOffset + 4 ] = nLine * 4 + 1;
				indices[ indexOffset + 5 ] = nLine * 4 + 0;

				indices[ indexOffset + 6 ] = nLine * 4 + 1;
				indices[ indexOffset + 7 ] = nLine * 4 + 5;
				indices[ indexOffset + 8 ] = nLine * 4 + 6;
				indices[ indexOffset + 9 ] = nLine * 4 + 6;
				indices[ indexOffset + 10 ] = nLine * 4 + 2;
				indices[ indexOffset + 11 ] = nLine * 4 + 1;

				indices[ indexOffset + 12 ] = nLine * 4 + 2;
				indices[ indexOffset + 13 ] = nLine * 4 + 6;
				indices[ indexOffset + 14 ] = nLine * 4 + 7;
				indices[ indexOffset + 15 ] = nLine * 4 + 7;
				indices[ indexOffset + 16 ] = nLine * 4 + 3;
				indices[ indexOffset + 17 ] = nLine * 4 + 2;

				indices[ indexOffset + 18 ] = nLine * 4 + 3;
				indices[ indexOffset + 19 ] = nLine * 4 + 7;
				indices[ indexOffset + 20 ] = nLine * 4 + 4;
				indices[ indexOffset + 21 ] = nLine * 4 + 4;
				indices[ indexOffset + 22 ] = nLine * 4 + 0;
				indices[ indexOffset + 23 ] = nLine * 4 + 3;
			}
		}

		public static void GeneratePolygonalChain( Vector3F[] points, double radius, out Vector3F[] positions, out int[] indices )
		{
			Vector3[] pointsD = new Vector3[ points.Length ];
			for( int n = 0; n < points.Length; n++ )
				pointsD[ n ] = points[ n ].ToVector3();
			Vector3[] positionsD;
			GeneratePolygonalChain( pointsD, radius, out positionsD, out indices );
			positions = ToVector3F( positionsD );
		}
	}
}
