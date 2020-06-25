// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	public enum ProjectionType
	{
		Perspective,
		Orthographic,
	}

	/// <summary>
	/// Defines a frustum and helps determine whether objects intersect with it.
	/// </summary>
	public class Frustum
	{
		//!!!!надо ли такой параметр?
		bool readOnlyFrustumProperties;

		ProjectionType projection;

		Vector3 origin;
		Quaternion rotation;
		double nearDistance;
		double farDistance;
		double halfWidth;// half the width at the far plane
		double halfHeight;// half the height at the far plane

		//precalculated values
		Matrix3 _axis;//inverted Y axis.
		Matrix3 _axisTransposed;
		double invFarDistance;
		Vector3[] points;
		Plane[] planes;

		//

		public Frustum( bool readOnlyFrustumProperties, ProjectionType projection, Vector3 origin, Quaternion rotation, double near, double far, double halfWidth, double halfHeight )
		{
			this.readOnlyFrustumProperties = readOnlyFrustumProperties;

			this.projection = projection;
			this.origin = origin;
			this.rotation = rotation;
			this.nearDistance = near;
			this.farDistance = far;
			this.halfWidth = halfWidth;
			this.halfHeight = halfHeight;

			Matrix3 mat = rotation.ToMatrix3();
			_axis = new Matrix3( mat.Item0, -mat.Item1, mat.Item2 );
			_axis.GetTranspose( out _axisTransposed );
			invFarDistance = 1.0f / far;
			points = null;
			planes = null;
		}

		public bool ReadOnlyFrustumProperties
		{
			get { return readOnlyFrustumProperties; }
		}

		public ProjectionType Projection
		{
			get { return projection; }
			set
			{
				if( readOnlyFrustumProperties )
					Log.Fatal( "Frustum: set Projection: ReadOnlyFrustumProperties == true." );

				if( projection == value )
					return;
				projection = value;

				points = null;
				planes = null;
			}
		}

		public Vector3 Origin
		{
			get { return origin; }
			set
			{
				if( readOnlyFrustumProperties )
					Log.Fatal( "Frustum: set Origin: ReadOnlyFrustumProperties == true." );

				if( origin == value )
					return;
				origin = value;

				points = null;
				planes = null;
			}
		}

		///// <summary>
		///// Bug here. Inverted Y axis. Better to use <see cref="Rotation"/> property.
		///// </summary>
		//public Mat3 _Axis
		//{
		//   get { return _axis; }
		//}

		public Quaternion Rotation
		{
			get { return rotation; }
			set
			{
				if( readOnlyFrustumProperties )
					Log.Fatal( "Frustum: set Rotation: ReadOnlyFrustumProperties == true." );

				if( rotation == value )
					return;
				rotation = value;

				Matrix3 mat = rotation.ToMatrix3();
				_axis = new Matrix3( mat.Item0, -mat.Item1, mat.Item2 );
				_axis.GetTranspose( out _axisTransposed );

				points = null;
				planes = null;
			}
		}

		public double NearDistance
		{
			get { return nearDistance; }
			set
			{
				if( readOnlyFrustumProperties )
					Log.Fatal( "Frustum: set NearDistance: ReadOnlyFrustumProperties == true." );

				if( nearDistance == value )
					return;
				if( value < 0 )
					Log.Fatal( "Frustum: set NearDistance: value < 0." );
				nearDistance = value;

				points = null;
				planes = null;
			}
		}

		public double FarDistance
		{
			get { return farDistance; }
			set
			{
				if( readOnlyFrustumProperties )
					Log.Fatal( "Frustum: set FarDistance: ReadOnlyFrustumProperties == true." );

				if( farDistance == value )
					return;
				if( value < 0 )
					Log.Fatal( "Frustum: set FarDistance: value < 0." );
				farDistance = value;
				invFarDistance = 1.0f / farDistance;

				points = null;
				planes = null;
			}
		}

		public double HalfWidth
		{
			get { return halfWidth; }
			set
			{
				if( readOnlyFrustumProperties )
					Log.Fatal( "Frustum: set HalfWidth: ReadOnlyFrustumProperties == true." );

				if( halfWidth == value )
					return;
				if( value <= 0 )
					Log.Fatal( "Frustum: set HalfWidth: value <= 0." );
				halfWidth = value;

				points = null;
				planes = null;
			}
		}

		public double HalfHeight
		{
			get { return halfHeight; }
			set
			{
				if( readOnlyFrustumProperties )
					Log.Fatal( "Frustum: set HalfHeight: ReadOnlyFrustumProperties == true." );

				if( halfHeight == value )
					return;
				if( value <= 0 )
					Log.Fatal( "Frustum: set HalfHeight: value <= 0." );
				halfHeight = value;

				points = null;
				planes = null;
			}
		}

		public void MoveFarDistance( double far )
		{
			if( readOnlyFrustumProperties )
				Log.Fatal( "Frustum: MoveFarDistance: ReadOnlyFrustumProperties == true." );

			//if( far <= nearDistance )
			//   Log.Fatal( "Frustum: MoveFarDistance: far <= nearDistance." );

			double scale = far / farDistance;
			farDistance = far;
			invFarDistance = 1.0f / farDistance;

			if( projection == ProjectionType.Perspective )
			{
				halfWidth *= scale;
				halfHeight *= scale;
			}

			points = null;
			planes = null;
		}

		//public bool IsValid()
		//{
		//   return farDistance > nearDistance;
		//}

		public override bool Equals( object obj )
		{
			return ( obj is Frustum && this == (Frustum)obj );
		}

		public override int GetHashCode()
		{
			return ( projection.GetHashCode() ^ origin.GetHashCode() ^ rotation.GetHashCode() ^
				nearDistance.GetHashCode() ^ farDistance.GetHashCode() ^ halfWidth.GetHashCode() ^
				halfHeight.GetHashCode() );
		}

		public static bool operator ==( Frustum v1, Frustum v2 )
		{
			bool v1Null = ReferenceEquals( v1, null );
			bool v2Null = ReferenceEquals( v2, null );
			if( v1Null || v2Null )
				return v1Null && v2Null;

			return ( v1.projection == v2.projection && v1.origin == v2.origin &&
				v1.rotation == v2.rotation && v1.nearDistance == v2.nearDistance &&
				v1.farDistance == v2.farDistance && v1.halfWidth == v2.halfWidth &&
				v1.halfHeight == v2.halfHeight );
		}

		public static bool operator !=( Frustum v1, Frustum v2 )
		{
			bool v1Null = ReferenceEquals( v1, null );
			bool v2Null = ReferenceEquals( v2, null );
			if( v1Null || v2Null )
				return !( v1Null && v2Null );

			return ( v1.projection != v2.projection && v1.origin != v2.origin ||
				v1.rotation != v2.rotation || v1.nearDistance != v2.nearDistance ||
				v1.farDistance != v2.farDistance || v1.halfWidth != v2.halfWidth ||
				v1.halfHeight != v2.halfHeight );
		}

		void GetOrthoBox( out Box box )
		{
			double centerDistance = ( farDistance + nearDistance ) * .5f;
			box.Center = origin + _axis * new Vector3( centerDistance, 0, 0 );
			box.Extents = new Vector3( farDistance - centerDistance, halfWidth, halfHeight );
			box.Axis = _axis;
		}

		public bool Intersects( ref Vector3 point )
		{
			if( projection == ProjectionType.Perspective )
			{
				Vector3 diff;
				Vector3.Subtract( ref point, ref origin, out diff );
				//Mat3 transposedAxis;
				//_axis.GetTranspose( out transposedAxis );
				Vector3 p;
				Matrix3.Multiply( ref diff, ref _axisTransposed, out p );
				//Vec3 p = ( point - origin ) * axis.GetTranspose();

				if( p.X < nearDistance || p.X > farDistance )
					return false;
				double scale = p.X * invFarDistance;
				if( Math.Abs( p.Y ) > halfWidth * scale )
					return false;
				if( Math.Abs( p.Z ) > halfHeight * scale )
					return false;
				return true;
			}
			else
			{
				Box box;
				GetOrthoBox( out box );
				return box.Contains( ref point );
			}
		}

		public bool Intersects( Vector3 point )
		{
			return Intersects( ref point );
		}

		public bool Intersects( Bounds bounds )
		{
			//!!!!slowly

			var points = bounds.ToPoints();

			foreach( var plane in Planes )
			{
				bool allClipped = true;
				foreach( var p in points )
				{
					if( plane.GetSide( p ) == Plane.Side.Negative )
					{
						allClipped = false;
						break;
					}
				}
				if( allClipped )
					return false;
			}

			return true;
		}

		public bool Intersects( Box box )
		{
			//!!!!slowly

			var points = box.ToPoints();

			foreach( var plane in Planes )
			{
				bool allClipped = true;
				foreach( var p in points )
				{
					if( plane.GetSide( p ) == Plane.Side.Negative )
					{
						allClipped = false;
						break;
					}
				}
				if( allClipped )
					return false;
			}

			return true;
		}

		////!!!!temp
		//bool OBBToFrustumIntersect2( Frustum frustum, Box box )
		////bool OBBToFrustumIntersect2( Frustum frustum, Vector3[] points )
		//{
		//	//!!!!slowly

		//	var points = box.ToPoints();

		//	foreach( var plane in frustum.Planes )
		//	{
		//		bool allClipped = true;
		//		foreach( var p in points )
		//		{
		//			if( plane.GetSide( p ) == Plane.Side.Negative )
		//			{
		//				allClipped = false;
		//				break;
		//			}
		//		}
		//		if( allClipped )
		//			return false;
		//	}

		//	return true;
		//}

		//!!!!temp
		//bool OBBToFrustumIntersect( Frustum frustum, Box box )
		//{
		//	// Do a SAT test along each frustum axis.
		//	// A frustum consists of six planes. Note that as near and far planes are parallel, we could optimize 
		//	// one iteration away.
		//	var planes = frustum.Planes;
		//	for( int i = 0; i < planes.Length; i++ )
		//	{
		//		var planeNormal = planes[ i ].Normal;
		//		//var planeDistance = planes[ i ].Distance;
		//		var planeDistance = -planes[ i ].Distance;

		//		// Find the negative and positive far points of the OBB to the current frustum plane.
		//		var x = Vector3.Dot( box.Axis[ 0 ], planeNormal ) >= 0.0f ? box.Extents[ 0 ] : -box.Extents[ 0 ];
		//		var y = Vector3.Dot( box.Axis[ 1 ], planeNormal ) >= 0.0f ? box.Extents[ 1 ] : -box.Extents[ 1 ];
		//		var z = Vector3.Dot( box.Axis[ 2 ], planeNormal ) >= 0.0f ? box.Extents[ 2 ] : -box.Extents[ 2 ];

		//		// There are eight half-diagonal vectors on an OBB. (A half-diagonal is a vector from OBB center to one of its
		//		// corner vertices). Compute the half-diagonal vector that points most in the same direction than the plane normal.
		//		var diag = x * box.Axis[ 0 ] + y * box.Axis[ 1 ] + z * box.Axis[ 2 ];
		//		// nPoint is now the corner point of the OBB that lies "the most" inside the frustum, 
		//		// its projection on the plane normal extends the most to the negative direction of that normal.
		//		var nPoint = box.Center - diag;
		//		//		const float3 pPoint = obb.center + diag;

		//		if( Vector3.Dot( nPoint, planeNormal ) + planeDistance >= 0.0f )
		//			return false; // OBB outside frustum.

		//		// If we would like to check whether the OBB is fully inside the frustum, need to compute
		//		// Dot(pPoint, frustum.planes.normal) + frustum.planes.d. If it's < 0 for all planes, OBB is totally
		//		// inside the frustum and doesn't intersect any of the frustum planes.
		//	}

		//	return true; // OBB inside the frustum or part of the OBB intersects the frustum.
		//}

		//!!!!temp
		//public static bool GetOrientedBoxIntersection( Frustum frustum, Box box )
		//{
		//	//ref Bounds box, ref Vector3 right, ref Vector3 up, ref Vector3 forward

		//	var center = box.Center;
		//	var extents = box.Extents;

		//	//!!!!
		//	var right = box.Axis * Vector3.XAxis;
		//	var up = box.Axis * Vector3.YAxis;
		//	var forward = box.Axis * Vector3.ZAxis;
		//	//var forward = box.Axis * Vector3.XAxis;
		//	//var up = box.Axis * Vector3.ZAxis;
		//	//var right = box.Axis * Vector3.YAxis;

		//	var planes = frustum.Planes;
		//	for( int i = 0; i < planes.Length; i++ )
		//	{
		//		var planeNormal = planes[ i ].Normal;
		//		//!!!!
		//		var planeDistance = planes[ i ].Distance;
		//		//var planeDistance = -planes[ i ].Distance;

		//		var r =
		//			extents.X * Math.Abs( Vector3.Dot( planeNormal, right ) ) +
		//			extents.Y * Math.Abs( Vector3.Dot( planeNormal, up ) ) +
		//			extents.Z * Math.Abs( Vector3.Dot( planeNormal, forward ) );

		//		var s = planeNormal.X * center.X + planeNormal.Y * center.Y + planeNormal.Z * center.Z;

		//		if( s + r < -planeDistance )
		//			return false;
		//	}

		//	return true;
		//}



		///// <summary>
		///// Fast culling. Not cull everything outside the frustum.
		///// </summary>
		///// <param name="bounds"></param>
		///// <returns></returns>
		//public bool IntersectsFast( ref Bounds bounds )
		//{
		//	if( projection == ProjectionType.Perspective )
		//	{
		//		Vector3 center;
		//		bounds.GetCenter( out center );
		//		//Vec3 center = bounds.GetCenter();

		//		Vector3 extents;
		//		Vector3.Subtract( ref bounds.Maximum, ref center, out extents );
		//		//Vec3 extents = bounds.maximum - center;

		//		//Mat3 transposedAxis;
		//		//_axis.GetTranspose( out transposedAxis );
		//		//Mat3 transposedAxis = axis.GetTranspose();

		//		Vector3 localOrigin;
		//		Vector3 diff;
		//		Vector3.Subtract( ref center, ref origin, out diff );
		//		Matrix3.Multiply( ref diff, ref _axisTransposed, out localOrigin );
		//		//Vec3 localOrigin = ( center - origin ) * transposedAxis;

		//		return !CullLocalBox( localOrigin, extents, ref _axisTransposed );
		//	}
		//	else
		//	{
		//		Box box;
		//		GetOrthoBox( out box );
		//		return box.Intersects( ref bounds );
		//	}
		//}

		///// <summary>
		///// Fast culling. Not cull everything outside the frustum.
		///// </summary>
		///// <param name="bounds"></param>
		///// <returns></returns>
		//public bool IntersectsFast( Bounds bounds )
		//{
		//	return IntersectsFast( ref bounds );
		//}

		//!!!!Bugged. 
		///// <summary>
		///// Fast culling. Not cull everything outside the frustum.
		///// </summary>
		///// <param name="box"></param>
		///// <returns></returns>
		//public bool IntersectsFast( ref Box box )
		//{
		//	if( projection == ProjectionType.Perspective )
		//	{
		//		//Mat3 transposedAxis;
		//		//_axis.GetTranspose( out transposedAxis );
		//		//Mat3 transposedAxis = axis.GetTranspose();

		//		Vector3 localOrigin;
		//		Vector3 diff;
		//		Vector3.Subtract( ref box.Center, ref origin, out diff );
		//		Matrix3.Multiply( ref diff, ref _axisTransposed, out localOrigin );
		//		//Vec3 localOrigin = ( box.center - origin ) * transposedAxis;

		//		Matrix3 localAxis;
		//		Matrix3.Multiply( ref box.Axis, ref _axisTransposed, out localAxis );
		//		//Mat3 localAxis = box.axis * transposedAxis;

		//		return !CullLocalBox( localOrigin, box.Extents, ref localAxis );
		//	}
		//	else
		//	{
		//		Box frustumBox;
		//		GetOrthoBox( out frustumBox );
		//		return frustumBox.Intersects( ref box );
		//	}
		//}

		///// <summary>
		///// Fast culling. Not cull everything outside the frustum.
		///// </summary>
		///// <param name="box"></param>
		///// <returns></returns>
		//public bool IntersectsFast( Box box )
		//{
		//	return IntersectsFast( ref box );
		//}

		//bool CullLocalBox( Vector3 localOrigin, Vector3 extents, ref Matrix3 localAxis )
		//{
		//	double d1, d2;
		//	Vector3 testOrigin;
		//	Matrix3 testAxis;

		//	// near plane
		//	d1 = nearDistance - localOrigin.X;
		//	d2 = Math.Abs( extents.X * localAxis.Item0.X ) +
		//		Math.Abs( extents.Y * localAxis.Item1.X ) +
		//		Math.Abs( extents.Z * localAxis.Item2.X );
		//	if( d1 - d2 > 0.0f )
		//		return true;

		//	// far plane
		//	d1 = localOrigin.X - farDistance;
		//	if( d1 - d2 > 0.0f )
		//		return true;

		//	testOrigin = localOrigin;
		//	testAxis = localAxis;

		//	if( testOrigin.Y < 0.0f )
		//	{
		//		testOrigin.Y = -testOrigin.Y;
		//		testAxis[ 0, 1 ] = -testAxis.Item0.Y;
		//		testAxis[ 1, 1 ] = -testAxis.Item1.Y;
		//		testAxis[ 2, 1 ] = -testAxis.Item2.Y;
		//	}

		//	// test left/right planes
		//	d1 = farDistance * testOrigin.Y - halfWidth * testOrigin.X;
		//	d2 = Math.Abs( extents.X * ( farDistance * testAxis.Item0.Y - halfWidth * testAxis.Item0.X ) ) +
		//		Math.Abs( extents.Y * ( farDistance * testAxis.Item1.Y - halfWidth * testAxis.Item1.X ) ) +
		//		Math.Abs( extents.Z * ( farDistance * testAxis.Item2.Y - halfWidth * testAxis.Item2.X ) );
		//	if( d1 - d2 > 0.0f )
		//		return true;

		//	if( testOrigin.Z < 0.0f )
		//	{
		//		testOrigin.Z = -testOrigin.Z;
		//		testAxis[ 0, 2 ] = -testAxis.Item0.Z;
		//		testAxis[ 1, 2 ] = -testAxis.Item1.Z;
		//		testAxis[ 2, 2 ] = -testAxis.Item2.Z;
		//	}

		//	// test up/down planes
		//	d1 = farDistance * testOrigin.Z - halfHeight * testOrigin.X;
		//	d2 = Math.Abs( extents.X * ( farDistance * testAxis.Item0.Z - halfHeight * testAxis.Item0.X ) ) +
		//		Math.Abs( extents.Y * ( farDistance * testAxis.Item1.Z - halfHeight * testAxis.Item1.X ) ) +
		//		Math.Abs( extents.Z * ( farDistance * testAxis.Item2.Z - halfHeight * testAxis.Item2.X ) );
		//	if( d1 - d2 > 0.0f )
		//		return true;

		//	return false;
		//}

		///// <summary>
		///// Exact intersection test.
		///// </summary>
		///// <param name="bounds"></param>
		///// <returns></returns>
		//public bool Intersects( ref Bounds bounds )
		//{
		//	if( projection == ProjectionType.Perspective )
		//	{
		//		unsafe
		//		{
		//			Vector3* indexPoints = stackalloc Vector3[ 8 ];
		//			Vector3* cornerVecs = stackalloc Vector3[ 4 ];

		//			Vector3 center;
		//			bounds.GetCenter( out center );
		//			//Vec3 center = bounds.GetCenter();

		//			Vector3 extents;
		//			Vector3.Subtract( ref bounds.Maximum, ref center, out extents );
		//			//Vec3 extents = bounds.Maximum - center;

		//			//Mat3 transposedAxis;
		//			//_axis.GetTranspose( out transposedAxis );
		//			//Mat3 transposedAxis = axis.GetTranspose();

		//			Vector3 localOrigin;
		//			Vector3 diff;
		//			Vector3.Subtract( ref center, ref origin, out diff );
		//			Matrix3.Multiply( ref diff, ref _axisTransposed, out localOrigin );
		//			//Vec3 localOrigin = ( center - origin ) * transposedAxis;

		//			if( CullLocalBox( localOrigin, extents, ref _axisTransposed ) )
		//				return false;

		//			ToIndexPointsAndCornerVecs( indexPoints, cornerVecs );

		//			if( BoundsCullLocalFrustum( ref bounds, this, indexPoints, cornerVecs ) )
		//				return false;

		//			Vector3 p;
		//			p = indexPoints[ 2 ]; indexPoints[ 2 ] = indexPoints[ 3 ]; indexPoints[ 3 ] = p;
		//			p = indexPoints[ 6 ]; indexPoints[ 6 ] = indexPoints[ 7 ]; indexPoints[ 7 ] = p;

		//			if( LocalFrustumIntersectsBounds( indexPoints, ref bounds ) )
		//				return true;

		//			Box box;
		//			box.Center = localOrigin;
		//			box.Extents = extents;
		//			box.Axis = _axisTransposed;
		//			box.ToPoints( indexPoints );
		//			//new Box( localOrigin, extents, transposedAxis ).ToPoints( indexPoints );

		//			if( LocalFrustumIntersectsFrustum( indexPoints, true ) )
		//				return true;
		//		}

		//		return false;
		//	}
		//	else
		//	{
		//		Box box;
		//		GetOrthoBox( out box );
		//		return box.Intersects( ref bounds );
		//	}
		//}

		///// <summary>
		///// Exact intersection test.
		///// </summary>
		///// <param name="bounds"></param>
		///// <returns></returns>
		//public bool Intersects( Bounds bounds )
		//{
		//	return Intersects( ref bounds );
		//}

		unsafe void ToIndexPointsAndCornerVecs( Vector3* indexPoints, Vector3* cornerVecs )
		{
			Vector3 scaled0, scaled1, scaled2;

			scaled0.X = origin.X + _axis.Item0.X * nearDistance;
			scaled0.Y = origin.Y + _axis.Item0.Y * nearDistance;
			scaled0.Z = origin.Z + _axis.Item0.Z * nearDistance;
			//scaled0 = origin + axis.mat0 * nearDistance;
			Vector3.Multiply( ref _axis.Item1, halfWidth * nearDistance * invFarDistance, out scaled1 );
			//scaled1 = axis.mat1 * ( halfWidth * nearDistance * invFarDistance );
			Vector3.Multiply( ref _axis.Item2, halfHeight * nearDistance * invFarDistance, out scaled2 );
			//scaled2 = axis.mat2 * ( halfHeight * nearDistance * invFarDistance );
			//Mat3 scaled = new Mat3(
			//   origin + axis.mat0 * nearDistance,
			//   axis.mat1 * ( halfWidth * nearDistance * invFarDistance ),
			//   axis.mat2 * ( halfHeight * nearDistance * invFarDistance ) );

			indexPoints[ 0 ] = new Vector3(
				scaled0.X - scaled1.X - scaled2.X,
				scaled0.Y - scaled1.Y - scaled2.Y,
				scaled0.Z - scaled1.Z - scaled2.Z );
			//indexPoints[ 0 ] = scaled0 - scaled1 - scaled2;

			indexPoints[ 1 ] = new Vector3(
				scaled0.X - scaled1.X + scaled2.X,
				scaled0.Y - scaled1.Y + scaled2.Y,
				scaled0.Z - scaled1.Z + scaled2.Z );
			//indexPoints[ 1 ] = scaled0 - scaled1 + scaled2;

			indexPoints[ 2 ] = new Vector3(
				scaled0.X + scaled1.X - scaled2.X,
				scaled0.Y + scaled1.Y - scaled2.Y,
				scaled0.Z + scaled1.Z - scaled2.Z );
			//indexPoints[ 2 ] = scaled0 + scaled1 - scaled2;

			indexPoints[ 3 ] = new Vector3(
				scaled0.X + scaled1.X + scaled2.X,
				scaled0.Y + scaled1.Y + scaled2.Y,
				scaled0.Z + scaled1.Z + scaled2.Z );
			//indexPoints[ 3 ] = scaled0 + scaled1 + scaled2;

			//indexPoints[ 0 ] = scaled0 - scaled1;
			//indexPoints[ 2 ] = scaled0 + scaled1;
			//indexPoints[ 1 ] = indexPoints[ 0 ] + scaled2;
			//indexPoints[ 3 ] = indexPoints[ 2 ] + scaled2;
			//indexPoints[ 0 ] -= scaled2;
			//indexPoints[ 2 ] -= scaled2;

			Vector3.Multiply( ref _axis.Item0, farDistance, out scaled0 );
			//scaled0 = axis.mat0 * farDistance;
			Vector3.Multiply( ref _axis.Item1, halfWidth, out scaled1 );
			//scaled1 = axis.mat1 * halfWidth;
			Vector3.Multiply( ref _axis.Item2, halfHeight, out scaled2 );
			//scaled2 = axis.mat2 * halfHeight;

			cornerVecs[ 0 ] = new Vector3(
				scaled0.X - scaled1.X - scaled2.X,
				scaled0.Y - scaled1.Y - scaled2.Y,
				scaled0.Z - scaled1.Z - scaled2.Z );
			//cornerVecs[ 0 ] = scaled0 - scaled1 - scaled2;

			cornerVecs[ 2 ] = new Vector3(
				scaled0.X + scaled1.X - scaled2.X,
				scaled0.Y + scaled1.Y - scaled2.Y,
				scaled0.Z + scaled1.Z - scaled2.Z );
			//cornerVecs[ 2 ] = scaled0 + scaled1 - scaled2;

			cornerVecs[ 1 ] = new Vector3(
				scaled0.X - scaled1.X + scaled2.X,
				scaled0.Y - scaled1.Y + scaled2.Y,
				scaled0.Z - scaled1.Z + scaled2.Z );
			//cornerVecs[ 1 ] = scaled0 - scaled1 + scaled2;

			cornerVecs[ 3 ] = new Vector3(
				scaled0.X + scaled1.X + scaled2.X,
				scaled0.Y + scaled1.Y + scaled2.Y,
				scaled0.Z + scaled1.Z + scaled2.Z );
			//cornerVecs[ 3 ] = scaled0 + scaled1 + scaled2;

			//cornerVecs[ 0 ] = scaled0 - scaled1;
			//cornerVecs[ 2 ] = scaled0 + scaled1;
			//cornerVecs[ 1 ] = cornerVecs[ 0 ] + scaled2;
			//cornerVecs[ 3 ] = cornerVecs[ 2 ] + scaled2;
			//cornerVecs[ 0 ] -= scaled2;
			//cornerVecs[ 2 ] -= scaled2;

			Vector3.Add( ref cornerVecs[ 0 ], ref origin, out indexPoints[ 4 ] );
			//indexPoints[ 4 ] = cornerVecs[ 0 ] + origin;
			Vector3.Add( ref cornerVecs[ 1 ], ref origin, out indexPoints[ 5 ] );
			//indexPoints[ 5 ] = cornerVecs[ 1 ] + origin;
			Vector3.Add( ref cornerVecs[ 2 ], ref origin, out indexPoints[ 6 ] );
			//indexPoints[ 6 ] = cornerVecs[ 2 ] + origin;
			Vector3.Add( ref cornerVecs[ 3 ], ref origin, out indexPoints[ 7 ] );
			//indexPoints[ 7 ] = cornerVecs[ 3 ] + origin;
		}

		static int FLOATSIGNBITSET( double v )
		{
			return v >= 0 ? 0 : 1;
		}

		static int FLOATSIGNBITNOTSET( double v )
		{
			return v < 0 ? 0 : 1;
		}

		static bool FLOATSIGNBITSET_BOOL( double v )
		{
			return v < 0;
		}

		static bool FLOATNOTZERO( double v )
		{
			return v != 0;
		}

		static unsafe bool BoundsCullLocalFrustum( ref Bounds bounds, Frustum localFrustum, Vector3* indexPoints, Vector3* cornerVecs )
		{
			int index;
			double dx, dy, dz;

			dy = -localFrustum._axis.Item1.X;
			dz = -localFrustum._axis.Item2.X;
			index = ( FLOATSIGNBITSET( dy ) << 1 ) | FLOATSIGNBITSET( dz );
			dx = -cornerVecs[ index ].X;
			index |= ( FLOATSIGNBITSET( dx ) << 2 );

			if( indexPoints[ index ].X < bounds.Minimum.X )
				return true;

			dy = localFrustum._axis.Item1.X;
			dz = localFrustum._axis.Item2.X;
			index = ( FLOATSIGNBITSET( dy ) << 1 ) | FLOATSIGNBITSET( dz );
			dx = cornerVecs[ index ].X;
			index |= ( FLOATSIGNBITSET( dx ) << 2 );

			if( indexPoints[ index ].X > bounds.Maximum.X )
				return true;

			dy = -localFrustum._axis.Item1.Y;
			dz = -localFrustum._axis.Item2.Y;
			index = ( FLOATSIGNBITSET( dy ) << 1 ) | FLOATSIGNBITSET( dz );
			dx = -cornerVecs[ index ].Y;
			index |= ( FLOATSIGNBITSET( dx ) << 2 );

			if( indexPoints[ index ].Y < bounds.Minimum.Y )
				return true;

			dy = localFrustum._axis.Item1.Y;
			dz = localFrustum._axis.Item2.Y;
			index = ( FLOATSIGNBITSET( dy ) << 1 ) | FLOATSIGNBITSET( dz );
			dx = cornerVecs[ index ].Y;
			index |= ( FLOATSIGNBITSET( dx ) << 2 );

			if( indexPoints[ index ].Y > bounds.Maximum.Y )
				return true;

			dy = -localFrustum._axis.Item1.Z;
			dz = -localFrustum._axis.Item2.Z;
			index = ( FLOATSIGNBITSET( dy ) << 1 ) | FLOATSIGNBITSET( dz );
			dx = -cornerVecs[ index ].Z;
			index |= ( FLOATSIGNBITSET( dx ) << 2 );

			if( indexPoints[ index ].Z < bounds.Minimum.Z )
				return true;

			dy = localFrustum._axis.Item1.Z;
			dz = localFrustum._axis.Item2.Z;
			index = ( FLOATSIGNBITSET( dy ) << 1 ) | FLOATSIGNBITSET( dz );
			dx = cornerVecs[ index ].Z;
			index |= ( FLOATSIGNBITSET( dx ) << 2 );

			if( indexPoints[ index ].Z > bounds.Maximum.Z )
				return true;

			return false;
		}

		unsafe bool LocalFrustumIntersectsBounds( Vector3* points, ref Bounds bounds )
		{
			for( int i = 0; i < 4; i++ )
				if( bounds.LineIntersection( ref points[ i ], ref points[ 4 + i ] ) )
					return true;
			if( nearDistance > 0.0f )
			{
				for( int i = 0; i < 4; i++ )
					if( bounds.LineIntersection( ref points[ i ], ref points[ ( i + 1 ) & 3 ] ) )
						return true;
			}
			for( int i = 0; i < 4; i++ )
				if( bounds.LineIntersection( ref points[ 4 + i ], ref points[ 4 + ( ( i + 1 ) & 3 ) ] ) )
					return true;
			return false;
		}

		unsafe bool LocalFrustumIntersectsFrustum( Vector3* points, bool testFirstSide )
		{
			for( int i = 0; i < 4; i++ )
				if( LocalLineIntersection( ref points[ i ], ref points[ 4 + i ] ) )
					return true;
			if( testFirstSide )
			{
				for( int i = 0; i < 4; i++ )
					if( LocalLineIntersection( ref points[ i ], ref points[ ( i + 1 ) & 3 ] ) )
						return true;
			}
			for( int i = 0; i < 4; i++ )
				if( LocalLineIntersection( ref points[ 4 + i ], ref points[ 4 + ( ( i + 1 ) & 3 ) ] ) )
					return true;

			return false;
		}

		bool LocalLineIntersection( ref Vector3 start, ref Vector3 end )
		{
			double d1, d2, fstart, fend, lstart, lend, f, x;
			int startInside = 1;

			double leftScale = halfWidth * invFarDistance;
			double upScale = halfHeight * invFarDistance;

			Vector3 dir;
			Vector3.Subtract( ref end, ref start, out dir );
			//Vec3 dir = end - start;

			// test near plane
			if( nearDistance > 0.0f )
			{
				d1 = nearDistance - start.X;
				startInside &= FLOATSIGNBITSET( d1 );
				if( FLOATNOTZERO( d1 ) )
				{
					d2 = nearDistance - end.X;
					if( FLOATSIGNBITSET_BOOL( d1 ) ^ FLOATSIGNBITSET_BOOL( d2 ) )
					{
						f = d1 / ( d1 - d2 );
						if( Math.Abs( start.Y + f * dir.Y ) <= nearDistance * leftScale )
							if( Math.Abs( start.Z + f * dir.Z ) <= nearDistance * upScale )
								return true;
					}
				}
			}

			// test far plane
			d1 = start.X - farDistance;
			startInside &= FLOATSIGNBITSET( d1 );
			if( FLOATNOTZERO( d1 ) )
			{
				d2 = end.X - farDistance;
				if( FLOATSIGNBITSET_BOOL( d1 ) ^ FLOATSIGNBITSET_BOOL( d2 ) )
				{
					f = d1 / ( d1 - d2 );
					if( Math.Abs( start.Y + f * dir.Y ) <= farDistance * leftScale )
						if( Math.Abs( start.Z + f * dir.Z ) <= farDistance * upScale )
							return true;
				}
			}

			fstart = farDistance * start.Y;
			fend = farDistance * end.Y;
			lstart = halfWidth * start.X;
			lend = halfWidth * end.X;

			// test left plane
			d1 = fstart - lstart;
			startInside &= FLOATSIGNBITSET( d1 );
			if( FLOATNOTZERO( d1 ) )
			{
				d2 = fend - lend;
				if( FLOATSIGNBITSET_BOOL( d1 ) ^ FLOATSIGNBITSET_BOOL( d2 ) )
				{
					f = d1 / ( d1 - d2 );
					x = start.X + f * dir.X;
					if( x >= nearDistance && x <= farDistance )
						if( Math.Abs( start.Z + f * dir.Z ) <= x * upScale )
							return true;
				}
			}

			// test right plane
			d1 = -fstart - lstart;
			startInside &= FLOATSIGNBITSET( d1 );
			if( FLOATNOTZERO( d1 ) )
			{
				d2 = -fend - lend;
				if( FLOATSIGNBITSET_BOOL( d1 ) ^ FLOATSIGNBITSET_BOOL( d2 ) )
				{
					f = d1 / ( d1 - d2 );
					x = start.X + f * dir.X;
					if( x >= nearDistance && x <= farDistance )
						if( Math.Abs( start.Z + f * dir.Z ) <= x * upScale )
							return true;
				}
			}

			fstart = farDistance * start.Z;
			fend = farDistance * end.Z;
			lstart = halfHeight * start.X;
			lend = halfHeight * end.X;

			// test up plane
			d1 = fstart - lstart;
			startInside &= FLOATSIGNBITSET( d1 );
			if( FLOATNOTZERO( d1 ) )
			{
				d2 = fend - lend;
				if( FLOATSIGNBITSET_BOOL( d1 ) ^ FLOATSIGNBITSET_BOOL( d2 ) )
				{
					f = d1 / ( d1 - d2 );
					x = start.X + f * dir.X;
					if( x >= nearDistance && x <= farDistance )
						if( Math.Abs( start.Y + f * dir.Y ) <= x * leftScale )
							return true;
				}
			}

			// test down plane
			d1 = -fstart - lstart;
			startInside &= FLOATSIGNBITSET( d1 );
			if( FLOATNOTZERO( d1 ) )
			{
				d2 = -fend - lend;
				if( FLOATSIGNBITSET_BOOL( d1 ) ^ FLOATSIGNBITSET_BOOL( d2 ) )
				{
					f = d1 / ( d1 - d2 );
					x = start.X + f * dir.X;
					if( x >= nearDistance && x <= farDistance )
						if( Math.Abs( start.Y + f * dir.Y ) <= x * leftScale )
							return true;
				}
			}

			return ( startInside != 0 );
		}

		public bool Intersects( ref Sphere sphere )
		{
			if( projection == ProjectionType.Perspective )
			{
				foreach( Plane plane in Planes )
				{
					if( plane.GetDistance( ref sphere.Origin ) > sphere.Radius )
						return false;
				}
				return true;
			}
			else
			{
				Box box;
				GetOrthoBox( out box );
				return sphere.Intersects( ref box );
			}
		}

		public bool Intersects( Sphere sphere )
		{
			return Intersects( ref sphere );
		}

		/// <summary>
		/// Gets 8 corner points.
		/// </summary>
		public Vector3[] Points
		{
			get
			{
				if( points == null )
				{
					var points2 = new Vector3[ 8 ];

					if( projection == ProjectionType.Perspective )
					{
						Vector3 scaled0, scaled1, scaled2;
						scaled0 = origin + ( _axis.Item0 * nearDistance );
						scaled1 = _axis.Item1 * ( halfWidth * nearDistance * invFarDistance );
						scaled2 = _axis.Item2 * ( halfHeight * nearDistance * invFarDistance );

						//Mat3 scaled = new Mat3(
						//   origin + axis.mat0 * nearDistance,
						//   axis.mat1 * ( halfWidth * nearDistance * invFarDistance ),
						//   axis.mat2 * ( halfHeight * nearDistance * invFarDistance ) );

						points2[ 0 ] = new Vector3(
							scaled0.X + scaled1.X + scaled2.X,
							scaled0.Y + scaled1.Y + scaled2.Y,
							scaled0.Z + scaled1.Z + scaled2.Z );
						//points[ 0 ] = scaled0 + scaled1 + scaled2;

						points2[ 1 ] = new Vector3(
							scaled0.X - scaled1.X + scaled2.X,
							scaled0.Y - scaled1.Y + scaled2.Y,
							scaled0.Z - scaled1.Z + scaled2.Z );
						//points[ 1 ] = scaled0 - scaled1 + scaled2;

						points2[ 2 ] = new Vector3(
							scaled0.X - scaled1.X - scaled2.X,
							scaled0.Y - scaled1.Y - scaled2.Y,
							scaled0.Z - scaled1.Z - scaled2.Z );
						//points[ 2 ] = scaled0 - scaled1 - scaled2;

						points2[ 3 ] = new Vector3(
							scaled0.X + scaled1.X - scaled2.X,
							scaled0.Y + scaled1.Y - scaled2.Y,
							scaled0.Z + scaled1.Z - scaled2.Z );
						//points[ 3 ] = scaled0 + scaled1 - scaled2;

						//points[ 0 ] = scaled0 + scaled1;
						//points[ 1 ] = scaled0 - scaled1;
						//points[ 2 ] = points[ 1 ] - scaled2;
						//points[ 3 ] = points[ 0 ] - scaled2;
						//points[ 0 ] += scaled2;
						//points[ 1 ] += scaled2;

						scaled0 = origin + _axis.Item0 * farDistance;
						scaled1 = _axis.Item1 * halfWidth;
						scaled2 = _axis.Item2 * halfHeight;

						points2[ 4 ] = new Vector3(
							scaled0.X + scaled1.X + scaled2.X,
							scaled0.Y + scaled1.Y + scaled2.Y,
							scaled0.Z + scaled1.Z + scaled2.Z );
						//points[ 0 ] = scaled0 + scaled1 + scaled2;

						points2[ 5 ] = new Vector3(
							scaled0.X - scaled1.X + scaled2.X,
							scaled0.Y - scaled1.Y + scaled2.Y,
							scaled0.Z - scaled1.Z + scaled2.Z );
						//points[ 1 ] = scaled0 - scaled1 + scaled2;

						points2[ 6 ] = new Vector3(
							scaled0.X - scaled1.X - scaled2.X,
							scaled0.Y - scaled1.Y - scaled2.Y,
							scaled0.Z - scaled1.Z - scaled2.Z );
						//points[ 2 ] = scaled0 - scaled1 - scaled2;

						points2[ 7 ] = new Vector3(
							scaled0.X + scaled1.X - scaled2.X,
							scaled0.Y + scaled1.Y - scaled2.Y,
							scaled0.Z + scaled1.Z - scaled2.Z );
						//points[ 3 ] = scaled0 + scaled1 - scaled2;

						//points[ 4 ] = scaled0 + scaled1;
						//points[ 5 ] = scaled0 - scaled1;
						//points[ 6 ] = points[ 5 ] - scaled2;
						//points[ 7 ] = points[ 4 ] - scaled2;
						//points[ 4 ] += scaled2;
						//points[ 5 ] += scaled2;
					}
					else
					{
						//slowly

						Vector3 nearPos = origin + _axis * new Vector3( nearDistance, 0, 0 );
						points2[ 0 ] = nearPos + _axis * new Vector3( 0, halfWidth, halfHeight );
						points2[ 1 ] = nearPos + _axis * new Vector3( 0, -halfWidth, halfHeight );
						points2[ 2 ] = nearPos + _axis * new Vector3( 0, -halfWidth, -halfHeight );
						points2[ 3 ] = nearPos + _axis * new Vector3( 0, halfWidth, -halfHeight );

						Vector3 farPos = origin + _axis * new Vector3( farDistance, 0, 0 );
						points2[ 4 ] = farPos + _axis * new Vector3( 0, halfWidth, halfHeight );
						points2[ 5 ] = farPos + _axis * new Vector3( 0, -halfWidth, halfHeight );
						points2[ 6 ] = farPos + _axis * new Vector3( 0, -halfWidth, -halfHeight );
						points2[ 7 ] = farPos + _axis * new Vector3( 0, halfWidth, -halfHeight );
					}

					points = points2;
				}

				return points;
			}
		}

		/// <summary>
		/// Gets 6 clip planes. Order: Near, far, side planes.
		/// </summary>
		public Plane[] Planes
		{
			get
			{
				if( planes == null )
				{
					var planes2 = new Plane[ 6 ];

					Vector3[] pts = Points;

					//near clip plane
					planes2[ 0 ] = Plane.FromPoints( pts[ 1 ], pts[ 2 ], pts[ 0 ] );

					//far clip plane
					planes2[ 1 ] = Plane.FromPoints( pts[ 5 ], pts[ 4 ], pts[ 6 ] );

					//side planes
					planes2[ 2 ] = Plane.FromPoints( pts[ 1 ], pts[ 5 ], pts[ 2 ] );
					planes2[ 3 ] = Plane.FromPoints( pts[ 4 ], pts[ 0 ], pts[ 7 ] );
					planes2[ 4 ] = Plane.FromPoints( pts[ 1 ], pts[ 0 ], pts[ 5 ] );
					planes2[ 5 ] = Plane.FromPoints( pts[ 6 ], pts[ 7 ], pts[ 2 ] );

					planes = planes2;
				}

				return planes;
			}
		}

		//!!!!check
		/// <summary>Gets the direction.</summary>
		public Vector3 Direction
		{
			get { return rotation.GetForward(); }
		}

		//!!!!check
		/// <summary>Gets the up vector.</summary>
		public Vector3 Up
		{
			get { return rotation.GetUp(); }
		}

		public Frustum Clone( bool readOnlyFrustumProperties )
		{
			return new Frustum( readOnlyFrustumProperties, projection, origin, rotation, nearDistance, farDistance, halfWidth, halfHeight );
		}

		///// <summary>
		///// Projects world position to screen coordinates.
		///// </summary>
		///// <param name="position">The world position.</param>
		///// <param name="screenPosition">The result screen coordinates.</param>
		///// <returns>
		///// <b>true</b> if screen position successfully received; otherwise, <b>false</b>.
		///// </returns>
		//public bool ProjectToScreenCoordinates( Vec3 position, out Vec2 screenPosition )
		//{
		//	screenPosition = new Vec2( -1, -1 );

		//	xx xx;

		//	// Transform light position into camera space

		//	// Don't use getViewMatrix here, incase overrided by camera and return a cull frustum view matrix
		//	Vector3 eyeSpacePos =  mViewMatrix.transformAffine( point );

		//	if( eyeSpacePos.z >= 0 )
		//		return false;
		//	// early-exit
		//	if( eyeSpacePos.squaredLength() <= 0 )
		//		return false;

		//	updateFrustum();
		//	Vector3 screenSpacePos = mProjMatrix * eyeSpacePos;

		//	*left = screenSpacePos.x;
		//	*top = screenSpacePos.y;
		//	return true;


		//	//!!!!!
		//	//Log.Fatal( "Frustum: ProjectToScreenCoordinates: impl." );
		//	//screenPosition = Vec2.Zero;
		//	//return false;
		//}

		///// <summary>
		///// Generates world ray from screen coordinates.
		///// </summary>
		///// <param name="screenPosition">The screen coordinates.</param>
		///// <returns>The ray.</returns>
		//public Ray GetRayByScreenCoordinates( Vec2 screenPosition )
		//{
		//	xx xx;

		//	//!!!!!
		//	//Log.Fatal( "Frustum: GetRayFromScreenCoordinates: impl." );
		//	return new Ray( new Vec3( 0, 0, 0 ), new Vec3( 1000, 1000, 1000 ) );
		//}
	}
}
