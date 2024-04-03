// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents a curve with rounded corners.
	/// </summary>
	public class CurveRoundedLine : CurveSpline
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetCurvatureRadius( int index )
		{
			var data = Points[ index ].additionalData;
			if( data != null && data is double )
				return (double)data;
			return 0;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		static bool GetCirclePoint( Vector3 p1, Vector3 p2, Vector3 p3, Vector3 pointOnLine, double radius, out Vector3 result )
		{
			Vector3 p = ( p1 + p3 ) * .5f;

			Vector3 projected1 = MathAlgorithms.ProjectPointToLine( p1, p2, p );
			Vector3 projected2 = MathAlgorithms.ProjectPointToLine( p2, p3, p );

			//Camera camera = RendererWorld.DefaultCamera;

			//camera.DebugGeometry.Color = new ColorValue( 1, 1, 0 );
			//camera.DebugGeometry.AddSphere( new Sphere( p, .1f ) );

			//camera.DebugGeometry.Color = new ColorValue( 1, 0, 0 );
			//camera.DebugGeometry.AddSphere( new Sphere( projected1, .1f ) );
			//camera.DebugGeometry.AddSphere( new Sphere( projected2, .1f ) );

			Vector3 dir1 = ( p - projected1 ).GetNormalize();
			Vector3 dir2 = ( p - projected2 ).GetNormalize();
			Line3 line1 = new Line3( p1 + dir1 * radius, p2 + dir1 * radius );

			//camera.DebugGeometry.AddLine( line1.Start, line1.End );

			Plane plane2 = Plane.FromPointAndNormal( projected2 + dir2 * radius, dir2 );

			Vector3 circleCenter;
			if( plane2.Intersects( new Ray( line1.Start, line1.End - line1.Start ), out circleCenter ) )
			{
				//camera.DebugGeometry.Color = new ColorValue( 1, 1, 0 );
				//camera.DebugGeometry.AddSphere( new Sphere( circleCenter, .1f ) );

				Vector3 dir = ( circleCenter - p2 ).GetNormalize();
				Sphere sphere = new Sphere( circleCenter, radius );

				double scale1;
				double scale2;
				Ray ray = new Ray( pointOnLine, dir * radius * 100 );
				if( sphere.Intersects( ray, out scale1, out scale2 ) )
				{
					double scale = Math.Min( scale1, scale2 );
					Vector3 pt = ray.GetPointOnRay( scale );

					Vector3 sphereProjected1 = MathAlgorithms.ProjectPointToLine( p1, p2, circleCenter );
					Vector3 sphereProjected2 = MathAlgorithms.ProjectPointToLine( p2, p3, circleCenter );

					if( ( sphereProjected1 - p2 ).LengthSquared() < ( p2 - p1 ).LengthSquared() &&
						( sphereProjected2 - p2 ).LengthSquared() < ( p3 - p2 ).LengthSquared() )
					{
						if( ( pt - p2 ).LengthSquared() < ( sphereProjected1 - p2 ).LengthSquared() )
						{
							result = pt;
							return true;
						}
					}
				}
			}

			result = Vector3.Zero;
			return false;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public override void CalculateValueByTime( double time, out Vector3 result )
		{
			if( points.Count == 1 )
			{
				result = points[ 0 ].value;
				return;
			}

			double clampedTime = GetClampedTime( time );
			int i = GetIndexForTime( clampedTime );
			if( i == 0 )
			{
				GetValueForIndex( i, out result );
				return;
			}
			if( i >= points.Count )
			{
				GetValueForIndex( points.Count - 1, out result );
				return;
			}

			GetValueForIndex( i - 1, out var from );
			GetValueForIndex( i, out var to );
			double length = points[ i ].time - points[ i - 1 ].time;
			if( length == 0 )
			{
				result = from;
				return;
			}

			double diffCoef = ( time - points[ i - 1 ].time ) / length;
			Vector3 pointOnLine = from + diffCoef * ( to - from );

			if( i > 1 )
			{
				double radius = GetCurvatureRadius( i - 1 );

				GetValueForIndex( i - 2, out var p1 );
				Vector3 p2 = from;
				Vector3 p3 = to;

				//!!!!MathEx.PI / 32?
				if( MathAlgorithms.GetVectorsAngle( p2 - p1, p3 - p2 ) > MathEx.PI / 32 )
				{
					Vector3 result2;
					if( GetCirclePoint( p1, p2, p3, pointOnLine, radius, out result2 ) )
					{
						result = result2;
						return;
					}
				}
			}
			if( i < points.Count - 1 )
			{
				double radius = GetCurvatureRadius( i );

				Vector3 p1 = from;
				Vector3 p2 = to;
				GetValueForIndex( i + 1, out var p3 );

				//!!!!MathEx.PI / 32?
				if( MathAlgorithms.GetVectorsAngle( p2 - p1, p3 - p2 ) > MathEx.PI / 32 )
				{
					Vector3 result2;
					if( GetCirclePoint( p1, p2, p3, pointOnLine, radius, out result2 ) )
					{
						result = result2;
						return;
					}
				}
			}

			result = pointOnLine;
		}

		//[MethodImpl( (MethodImplOptions)512 )]
		//public override Vector3 CalculateValueByTime( double time )
		//{
		//	if( points.Count == 1 )
		//		return points[ 0 ].value;

		//	double clampedTime = GetClampedTime( time );
		//	int i = GetIndexForTime( clampedTime );
		//	if( i == 0 )
		//		return GetValueForIndex( i );
		//	if( i >= points.Count )
		//		return GetValueForIndex( points.Count - 1 );

		//	Vector3 from = GetValueForIndex( i - 1 );
		//	Vector3 to = GetValueForIndex( i );
		//	double length = points[ i ].time - points[ i - 1 ].time;
		//	if( length == 0 )
		//		return from;

		//	double diffCoef = ( time - points[ i - 1 ].time ) / length;
		//	Vector3 pointOnLine = from + diffCoef * ( to - from );

		//	if( i > 1 )
		//	{
		//		double radius = GetCurvatureRadius( i - 1 );

		//		Vector3 p1 = GetValueForIndex( i - 2 );
		//		Vector3 p2 = from;
		//		Vector3 p3 = to;

		//		//!!!!MathEx.PI / 32?
		//		if( MathAlgorithms.GetVectorsAngle( p2 - p1, p3 - p2 ) > MathEx.PI / 32 )
		//		{
		//			Vector3 result;
		//			if( GetCirclePoint( p1, p2, p3, pointOnLine, radius, out result ) )
		//				return result;
		//		}
		//	}
		//	if( i < points.Count - 1 )
		//	{
		//		double radius = GetCurvatureRadius( i );

		//		Vector3 p1 = from;
		//		Vector3 p2 = to;
		//		Vector3 p3 = GetValueForIndex( i + 1 );

		//		//!!!!MathEx.PI / 32?
		//		if( MathAlgorithms.GetVectorsAngle( p2 - p1, p3 - p2 ) > MathEx.PI / 32 )
		//		{
		//			Vector3 result;
		//			if( GetCirclePoint( p1, p2, p3, pointOnLine, radius, out result ) )
		//				return result;
		//		}
		//	}

		//	return pointOnLine;
		//}
	}
}
