// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a convex polygon.
	/// </summary>
	public static class ConvexPolygon
	{
		struct PointItem
		{
			public int sourceIndex;
			public Vector2 point;
		}

		//

		static double GetTriangleSquare( Vector2 a, Vector2 b, Vector2 c )
		{
			return Math.Abs( ( b.X - a.X ) * ( c.Y - a.Y ) - ( c.X - a.X ) * ( b.Y - a.Y ) ) * .5;
		}

		static bool cw( Vector2 a, Vector2 b, Vector2 c )
		{
			return a.X * ( b.Y - c.Y ) + b.X * ( c.Y - a.Y ) + c.X * ( a.Y - b.Y ) < 0;
		}

		static bool ccw( Vector2 a, Vector2 b, Vector2 c )
		{
			return a.X * ( b.Y - c.Y ) + b.X * ( c.Y - a.Y ) + c.X * ( a.Y - b.Y ) > 0;
		}

		/// <summary>
		/// Convex Hull Graham algorithm.
		/// </summary>
		/// <param name="points"></param>
		/// <param name="epsilon"></param>
		/// <returns></returns>
		public static int[] GetFromPoints( IList<Vector2> points, double epsilon )
		{
			List<PointItem> list = new List<PointItem>( points.Count );
			for( int nPoint = 0; nPoint < points.Count; nPoint++ )
			{
				Vector2 point = points[ nPoint ];

				//const double epsilon = .001;

				bool skip = false;
				for( int nPointItem = 0; nPointItem < list.Count; nPointItem++ )
				{
					PointItem listItem = list[ nPointItem ];
					if( listItem.point.Equals( ref point, epsilon ) )
					{
						skip = true;
						break;
					}
				}
				if( skip )
					continue;

				PointItem pointItem = new PointItem();
				pointItem.sourceIndex = nPoint;
				pointItem.point = points[ nPoint ];
				list.Add( pointItem );
			}

			if( list.Count < 3 )
			{
				int[] array = new int[ points.Count ];
				for( int n = 0; n < array.Length; n++ )
					array[ n ] = n;
				return array;
			}

			//!!!!merge sort
			CollectionUtility.SelectionSort( list, delegate ( PointItem item1, PointItem item2 )
			{
				Vector2 i1 = item1.point;
				Vector2 i2 = item2.point;

				if( i1.X < i2.X || i1.X == i2.X && i1.Y < i2.Y )
					return 1;
				else
					return -1;
			} );

			PointItem p1 = list[ 0 ];
			PointItem p2 = list[ list.Count - 1 ];

			List<PointItem> up = new List<PointItem>( points.Count );
			List<PointItem> down = new List<PointItem>( points.Count );
			up.Add( p1 );
			down.Add( p1 );

			for( int i = 1; i < list.Count; ++i )
			{
				if( i == list.Count - 1 || cw( p1.point, list[ i ].point, p2.point ) )
				{
					while( up.Count >= 2 && !cw( up[ up.Count - 2 ].point, up[ up.Count - 1 ].point,
						list[ i ].point ) )
					{
						up.RemoveAt( up.Count - 1 );
					}
					up.Add( list[ i ] );
				}
				if( i == list.Count - 1 || ccw( p1.point, list[ i ].point, p2.point ) )
				{
					while( down.Count >= 2 && !ccw( down[ down.Count - 2 ].point,
						down[ down.Count - 1 ].point, list[ i ].point ) )
					{
						down.RemoveAt( down.Count - 1 );
					}
					down.Add( list[ i ] );
				}
			}

			List<int> result = new List<int>( points.Count );

			for( int i = 0; i < up.Count; i++ )
				result.Add( up[ i ].sourceIndex );
			for( int i = down.Count - 2; i > 0; i-- )
				result.Add( down[ i ].sourceIndex );

			return result.ToArray();
		}

	}
}
