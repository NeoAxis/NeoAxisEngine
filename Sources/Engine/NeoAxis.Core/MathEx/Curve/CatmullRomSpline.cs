// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a Catmull-Rom spline curve.
	/// </summary>
	/// <remarks>
	/// Uses the generalized (parameterized) Catmull-Rom formulation. The curve is converted to a cubic Hermite segment between points p1 and p2, where tangents are computed from the neighbor points using non-uniform knot spacing controlled by <see cref="Alpha"/>.
	/// </remarks>
	public class CatmullRomSpline : CurveSpline
	{
		const double Epsilon = 1e-12;

		/// <summary>
		/// Tangent tension. 0 gives a standard Catmull-Rom curve, 1 gives zero tangents (sharp corners at the points), negative values give looser curves. Typical range is [0, 1].
		/// </summary>
		public double Tension { get; set; }

		/// <summary>
		/// Tangent length multiplier. 1 is the default; values below 1 flatten the curve towards straight lines, values above 1 increase the curvature (overshoot).
		/// </summary>
		public double Stiffness { get; set; } = 1.0;

		/// <summary>
		/// Knot parameterization. 0 = uniform, 0.5 = centripetal (default, avoids cusps and self-intersections), 1 = chordal.
		/// </summary>
		public double Alpha { get; set; } = 0.5;

		//

		[MethodImpl( (MethodImplOptions)512 )]
		public override void CalculateValueByTime( double time, out Vector3 result )
		{
			if( Points.Count == 1 )
			{
				result = Points[ 0 ].Value;
				return;
			}

			var clampedTime = GetClampedTime( time );
			int i = GetIndexForTime( clampedTime );

			GetValueForIndex( i - 2, out Vector3 p0 );
			GetValueForIndex( i - 1, out Vector3 p1 );
			GetValueForIndex( i, out Vector3 p2 );
			GetValueForIndex( i + 1, out Vector3 p3 );

			var t0 = GetTimeForIndex( i - 1 );
			var t1 = GetTimeForIndex( i );

			var timeInterval = t1 - t0;

			if( Math.Abs( timeInterval ) < Epsilon )
			{
				result = p1;
				return;
			}

			var s = ( clampedTime - t0 ) / timeInterval;
			s = Math.Max( 0.0, Math.Min( 1.0, s ) );

			if( ( p2 - p1 ).LengthSquared() < Epsilon * Epsilon )
			{
				result = p1;
				return;
			}

			var d01 = KnotInterval( p0, p1 );
			var d12 = KnotInterval( p1, p2 );
			var d23 = KnotInterval( p2, p3 );

			var tangentScale = ( 1.0 - Tension ) * Stiffness;

			var m1 = ComputeTangent( p0, p1, p2, d01, d12 ) * ( d12 * tangentScale );
			var m2 = ComputeTangent( p1, p2, p3, d12, d23 ) * ( d12 * tangentScale );

			var s2 = s * s;
			var s3 = s2 * s;

			var h00 = 2.0 * s3 - 3.0 * s2 + 1.0;
			var h10 = s3 - 2.0 * s2 + s;
			var h01 = -2.0 * s3 + 3.0 * s2;
			var h11 = s3 - s2;

			result = h00 * p1 + h10 * m1 + h01 * p2 + h11 * m2;
		}

		/// <summary>
		/// Computes the knot interval between two points: |b - a|^Alpha. Falls back to 1 for coincident points to avoid division by zero.
		/// </summary>
		double KnotInterval( Vector3 a, Vector3 b )
		{
			var alpha = Math.Max( 0.0, Math.Min( 1.0, Alpha ) );
			if( alpha == 0.0 )
				return 1.0;
			var distance = ( b - a ).Length();
			if( distance < Epsilon )
				return Epsilon;
			if( alpha == 0.5 )
				return Math.Sqrt( distance );
			if( alpha == 1.0 )
				return distance;
			return Math.Pow( distance, alpha );
		}

		/// <summary>
		/// Computes the tangent (derivative with respect to the knot parameter) at the middle point
		/// <paramref name="b"/> using its neighbors <paramref name="a"/> and <paramref name="c"/>.
		/// <paramref name="dAB"/> and <paramref name="dBC"/> are the knot intervals of both sides.
		/// </summary>
		static Vector3 ComputeTangent( Vector3 a, Vector3 b, Vector3 c, double dAB, double dBC )
		{
			if( dAB < Epsilon || dBC < Epsilon )
				return Vector3.Zero;
			var denominator = dAB + dBC;
			if( denominator < Epsilon )
				return Vector3.Zero;
			return ( b - a ) / dAB - ( c - a ) / denominator + ( c - b ) / dBC;
		}
	}
}
