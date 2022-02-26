// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Base class of curves.
	/// </summary>
	public class Curve1F
	{
		internal List<Point> points = new List<Point>();

		ThreadLocal<int> currentIndex = new ThreadLocal<int>( () => -1 );
		//int currentIndex = -1;

		/////////////////////////////////////////

		/// <summary>
		/// Represents a point of <see cref="Curve"/>.
		/// </summary>
		public struct Point
		{
			internal float time;
			internal float value;

			//

			public Point( float time, float value )
			{
				this.time = time;
				this.value = value;
			}

			public float Time
			{
				get { return time; }
			}

			public float Value
			{
				get { return value; }
			}

			public override string ToString()
			{
				return string.Format( "Time: {0}; Value {1}", time, value );
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// The list of points sorted by time.
		/// </summary>
		public List<Point> Points
		{
			get { return points; }
		}

		public int AddPoint( Point point )
		{
			int index = GetIndexForTime( point.time );
			points.Insert( index, point );
			return index;
		}

		public int AddPoint( float time, float value )
		{
			return AddPoint( new Point( time, value ) );
		}

		public void RemovePoint( int index )
		{
			points.RemoveAt( index );
			if( points.Count == 0 )
				currentIndex.Value = -1;
		}

		public void Clear()
		{
			points.Clear();
			currentIndex.Value = -1;
		}

		public virtual float CalculateValueByTime( float time )
		{
			int i = GetIndexForTime( time );
			if( i >= points.Count )
				return points[ points.Count - 1 ].value;
			else
				return points[ i ].value;
		}

		public virtual float GetCurrentFirstDerivative( float time )
		{
			return 0;
		}

		protected int GetIndexForTime( float time )
		{
			int len, mid, offset, res;

			//optimization
			var index = currentIndex.Value;
			if( index >= 0 && index <= points.Count )
			{
				if( index == 0 )
				{
					if( time <= points[ index ].time )
						return index;
				}
				else if( index == points.Count )
				{
					if( time > points[ index - 1 ].time )
						return index;
				}
				else if( time > points[ index - 1 ].time && time <= points[ index ].time )
				{
					return index;
				}
				else if( time > points[ index ].time && ( index + 1 == points.Count || time <= points[ index + 1 ].time ) )
				{
					currentIndex.Value = index + 1;
					return index + 1;
				}
			}

			//default calculation
			len = points.Count;
			mid = len;
			offset = 0;
			res = 0;
			while( mid > 0 )
			{
				mid = len >> 1;
				if( time == points[ offset + mid ].time )
				{
					return offset + mid;
				}
				else if( time > points[ offset + mid ].time )
				{
					offset += mid;
					len -= mid;
					res = 1;
				}
				else
				{
					len -= mid;
					res = 0;
				}
			}
			index = offset + res;
			currentIndex.Value = index;
			return index;
		}

		protected float GetSpeed( float time )
		{
			//int i;
			//float speed;
			float value;

			value = GetCurrentFirstDerivative( time );
			return value;
			//for( speed = 0.0f, i = 0; i < 3; i++ )
			//	speed += value[ i ] * value[ i ];
			//return Math.Sqrt( speed );
		}

		public int GetPointIndexByTime( float time )
		{
			for( int n = 0; n < points.Count; n++ )
				if( points[ n ].time == time )
					return n;
			return -1;
		}
	}
}
