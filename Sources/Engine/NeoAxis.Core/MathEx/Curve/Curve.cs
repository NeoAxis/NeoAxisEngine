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
	public class Curve
	{
		internal List<Point> points = new List<Point>();

		//public bool AllowCachingIndexForTime { get; set; } = true;
		//int cachedIndex = -1;

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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int AddPoint( Point point )
		{
			int index = GetIndexForTime( point.time );
			points.Insert( index, point );
			return index;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int AddPoint( double time, Vector3 value, object additionalData = null )
		{
			return AddPoint( new Point( time, value, additionalData ) );
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
		public virtual void CalculateValueByTime( double time, out Vector3 result )
		{
			int i = GetIndexForTime( time );
			if( i >= points.Count )
				result = points[ points.Count - 1 ].value;
			else
				result = points[ i ].value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 CalculateValueByTime( double time )
		{
			CalculateValueByTime( time, out var result );
			return result;
		}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public virtual Vector3 CalculateValueByTime( double time )
		//{
		//	int i = GetIndexForTime( time );
		//	if( i >= points.Count )
		//		return points[ points.Count - 1 ].value;
		//	else
		//		return points[ i ].value;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public virtual Vector3 GetCurrentFirstDerivative( double time )
		{
			return Vector3.Zero;
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
