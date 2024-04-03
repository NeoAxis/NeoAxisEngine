// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Base class of curves.
	/// </summary>
	public class Curve1
	{
		internal List<Point> points;// = new List<Point>();

		//bad for multithreading
		//public bool AllowCachingIndexForTime { get; set; } = true;
		//int cachedIndex = -1;

		/////////////////////////////////////////

		/// <summary>
		/// Represents a point of <see cref="Curve"/>.
		/// </summary>
		public struct Point
		{
			internal double time;
			internal double value;

			//

			public Point( double time, double value )
			{
				this.time = time;
				this.value = value;
			}

			public double Time
			{
				get { return time; }
			}

			public double Value
			{
				get { return value; }
			}

			public override string ToString()
			{
				return string.Format( "Time: {0}; Value {1}", time, value );
			}
		}

		/////////////////////////////////////////

		public Curve1( int initialCapacity = 4 )
		{
			points = new List<Point>( initialCapacity );
		}

		/// <summary>
		/// The list of points sorted by time.
		/// </summary>
		public List<Point> Points
		{
			get { return points; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int AddPoint( Point point )
		{
			int index = GetIndexForTime( point.time );
			points.Insert( index, point );
			return index;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int AddPoint( double time, double value )
		{
			return AddPoint( new Point( time, value ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void RemovePoint( int index )
		{
			points.RemoveAt( index );
			//cachedIndex = -1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Clear()
		{
			points.Clear();
			//cachedIndex = -1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public virtual double CalculateValueByTime( double time )
		{
			int i = GetIndexForTime( time );
			if( i >= points.Count )
				return points[ points.Count - 1 ].value;
			else
				return points[ i ].value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public virtual double GetCurrentFirstDerivative( double time )
		{
			return 0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected int GetIndexForTime( double time )
		{
			int len, mid, offset, res;

			////optimization
			//if( AllowCachingIndexForTime )
			//{
			//	var index = cachedIndex;//.Value;
			//	if( index >= 0 && index <= points.Count )
			//	{
			//		if( index == 0 )
			//		{
			//			if( time <= points[ index ].time )
			//				return index;
			//		}
			//		else if( index == points.Count )
			//		{
			//			if( time > points[ index - 1 ].time )
			//				return index;
			//		}
			//		else if( time > points[ index - 1 ].time && time <= points[ index ].time )
			//		{
			//			return index;
			//		}
			//		else if( time > points[ index ].time && ( index + 1 == points.Count || time <= points[ index + 1 ].time ) )
			//		{
			//			cachedIndex/*.Value*/ = index + 1;
			//			return index + 1;
			//		}
			//	}
			//}

			//default calculation
			{
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
				var index = offset + res;
				//cachedIndex = index;
				return index;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected double GetSpeed( double time )
		{
			//int i;
			//double speed;
			double value;

			value = GetCurrentFirstDerivative( time );
			return value;
			//for( speed = 0.0f, i = 0; i < 3; i++ )
			//	speed += value[ i ] * value[ i ];
			//return Math.Sqrt( speed );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int GetPointIndexByTime( double time )
		{
			for( int n = 0; n < points.Count; n++ )
				if( points[ n ].time == time )
					return n;
			return -1;
		}
	}
}
