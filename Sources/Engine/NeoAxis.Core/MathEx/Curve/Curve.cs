// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Base class of curves.
	/// </summary>
	public class Curve
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
			internal double time;
			internal Vector3 value;
			internal object additionalData;

			//

			public Point( double time, Vector3 value, object additionalData = null )
			{
				this.time = time;
				this.value = value;
				this.additionalData = additionalData;
			}

			public double Time
			{
				get { return time; }
			}

			public Vector3 Value
			{
				get { return value; }
			}

			public object AdditionalData
			{
				get { return additionalData; }
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

		public int AddPoint( double time, Vector3 value, object additionalData = null )
		{
			return AddPoint( new Point( time, value, additionalData ) );
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

		public virtual Vector3 CalculateValueByTime( double time )
		{
			int i = GetIndexForTime( time );
			if( i >= points.Count )
				return points[ points.Count - 1 ].value;
			else
				return points[ i ].value;
		}

		public virtual Vector3 GetCurrentFirstDerivative( double time )
		{
			return Vector3.Zero;
		}

		protected int GetIndexForTime( double time )
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

		protected double GetSpeed( double time )
		{
			int i;
			double speed;
			Vector3 value;

			value = GetCurrentFirstDerivative( time );
			for( speed = 0.0f, i = 0; i < 3; i++ )
				speed += value[ i ] * value[ i ];
			return Math.Sqrt( speed );
		}
	}
}
