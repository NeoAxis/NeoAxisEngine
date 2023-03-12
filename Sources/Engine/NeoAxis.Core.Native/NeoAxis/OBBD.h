// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
/*
-----------------------------------------------------------------------------
This source file is part of OGRE
	(Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org/

Copyright (c) 2000-2009 Torus Knot Software Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
#ifndef __OBBD_H_
#define __OBBD_H_

#include "OgrePrerequisites.h"
#include "OgreVector3D.h"
#include "OgreMatrix3D.h"
#include "BoundsD.h"

namespace Ogre
{
	class OBBD
	{
	public:
		Vector3D center;
		Vector3D extents;
		Matrix3D axis;

		FORCEINLINE OBBD() {}

		FORCEINLINE explicit OBBD( const Vector3D& center, const Vector3D& extents, const Matrix3D& axis )
		{
			this->center = center;
			this->extents = extents;
			this->axis = axis;
		}

		FORCEINLINE explicit OBBD( const Vector3D& point )
		{
			this->center = point;
			this->extents = Vector3D::ZERO;
			this->axis = Matrix3D::IDENTITY;
		}

		FORCEINLINE explicit OBBD( const BoundsD& bounds )
		{
			this->center = bounds.getCenter();
			this->extents = bounds.getMaximum() - this->center;
			this->axis = Matrix3D::IDENTITY;
		}

		FORCEINLINE explicit OBBD( const BoundsD& bounds, const Vector3D& origin, const Matrix3D& axis )
		{
			this->center = bounds.getCenter();
			this->extents = bounds.getMaximum() - this->center;
			this->center = origin + this->center * axis;
			this->axis = axis;
		}

		//bool compare( const OBBD& a ) const
		//{
		//	return center == a.center && extents == a.extents && axis == a.axis;
		//}

		//bool compare( const OBBD& a, const float epsilon ) const
		//{
		//	return ( center.compare( a.center, epsilon ) && extents.compare( a.extents, epsilon ) && axis.compare( a.axis, epsilon ) );
		//}

		FORCEINLINE bool operator == ( const OBBD& a ) const
		{
			return center == a.center && extents == a.extents && axis == a.axis;
		}

		FORCEINLINE bool operator != ( const OBBD& a ) const
		{
			return center != a.center || extents != a.extents || axis != a.axis;
		}

		FORCEINLINE const Vector3D& getCenter() const { return center; }
		FORCEINLINE void setCenter( const Vector3D& center ) { this->center = center; }

		FORCEINLINE const Vector3D& getExtents() const { return extents; }
		FORCEINLINE void setExtents( const Vector3D& extents ) { this->extents = extents; }

		FORCEINLINE const Matrix3D& getAxis() const { return axis; }
		FORCEINLINE void setAxis( const Matrix3D& axis ) { this->axis = axis; }

		FORCEINLINE double GetVolume() const
		{
			return ( extents * 2.0f ).squaredLength();
		}

		FORCEINLINE bool IsCleared() const
		{
			return extents.x < 0.0f;
		}

		//FORCEINLINE void setCleared()
		//{
		//	setExtents( FLT_MAX, FLT_MAX, FLT_MAX, FLT_MIN, FLT_MIN, FLT_MIN );
		//}

		//bool			AddPoint( const idVec3 &v );					// add the point, returns true if the box expanded
		//bool			AddBox( const idBox &a );						// add the box, returns true if the box expanded

		//public void expand( double d )
		//{
		//	extents.X += d;
		//	extents.Y += d;
		//	extents.Z += d;
		//}
		//idBox			Expand( const double d ) const;					// return box expanded in all directions with the given value

		//idBox &			ExpandSelf( const double d );					// expand box in all directions with the given value
		//idBox			Translate( const idVec3 &translation ) const;	// return translated box
		//idBox &			TranslateSelf( const idVec3 &translation );		// translate this box
		//idBox			Rotate( const idMat3 &rotation ) const;			// return rotated box
		//idBox &			RotateSelf( const idMat3 &rotation );			// rotate this box

		//float			PlaneDistance( const idPlane &plane ) const;
		//int				PlaneSide( const idPlane &plane, const float epsilon = ON_EPSILON ) const;

		//bool			ContainsPoint( const idVec3 &p ) const;			// includes touching
		//bool			IntersectsBox( const idBox &a ) const;			// includes touching
		//bool			LineIntersection( const idVec3 &start, const idVec3 &end ) const;
		//				// intersection points are (start + dir * scale1) and (start + dir * scale2)
		//bool			RayIntersection( const idVec3 &start, const idVec3 &dir, float &scale1, float &scale2 ) const;

		//				// tight box for a collection of points
		//void			FromPoints( const idVec3 *points, const int numPoints );
		//				// most tight box for a translation
		//void			FromPointTranslation( const idVec3 &point, const idVec3 &translation );
		//void			FromBoxTranslation( const idBox &box, const idVec3 &translation );
		//				// most tight box for a rotation
		//void			FromPointRotation( const idVec3 &point, const idRotation &rotation );
		//void			FromBoxRotation( const idBox &box, const idRotation &rotation );

		//void			ToPoints( idVec3 points[8] ) const;
		//idSphere		ToSphere( void ) const;

		//				// calculates the projection of this box onto the given axis
		//void			AxisProjection( const idVec3 &dir, float &min, float &max ) const;
		//void			AxisProjection( const idMat3 &ax, idBoundsD &bounds ) const;

		//				// calculates the silhouette of the box
		//int				GetProjectionSilhouetteVerts( const idVec3 &projectionOrigin, idVec3 silVerts[6] ) const;
		//int				GetParallelProjectionSilhouetteVerts( const idVec3 &projectionDir, idVec3 silVerts[6] ) const;


		//public static Box operator +( Box b, Vec3 v )
		//{
		//	Box result;
		//	Vec3.Add( ref b.center, ref v, out result.center );
		//	result.extents = b.extents;
		//	result.axis = b.axis;
		//	return result;
		//	//return new Box( b.center + v, b.extents, b.axis );
		//}

		//public static Box operator *( Box b, Mat3 m )
		//{
		//	Box result;
		//	Mat3.Multiply( ref b.center, ref m, out result.center );
		//	result.extents = b.extents;
		//	Mat3.Multiply( ref b.axis, ref m, out result.axis );
		//	return result;
		//	//return new Box( b.center * m, b.extents, b.axis * m );
		//}

		//public static Box operator *( Box b, Mat4 m )
		//{
		//	Box result;
		//	Mat3 m3;
		//	m.ToMat3( out m3 );
		//	Multiply( ref b, ref m3, out result );
		//	result.center.x += m.mat3.x;
		//	result.center.y += m.mat3.y;
		//	result.center.z += m.mat3.z;
		//	return result;
		//	//return ( b * m.ToMat3() ) + m.mat3.ToVec3();
		//}

		//public static void Add( ref Box b, ref Vec3 v, out Box result )
		//{
		//	Vec3.Add( ref b.center, ref v, out result.center );
		//	result.extents = b.extents;
		//	result.axis = b.axis;
		//}

		//public static void Multiply( ref Box b, ref Mat3 m, out Box result )
		//{
		//	Mat3.Multiply( ref b.center, ref m, out result.center );
		//	result.extents = b.extents;
		//	Mat3.Multiply( ref b.axis, ref m, out result.axis );
		//}

		//public static void Multiply( ref Box b, ref Mat4 m, out Box result )
		//{
		//	Mat3 m3;
		//	m.ToMat3( out m3 );
		//	Multiply( ref b, ref m3, out result );
		//	result.center.x += m.mat3.x;
		//	result.center.y += m.mat3.y;
		//	result.center.z += m.mat3.z;
		//}

		//public static Box Add( ref Box b, ref Vec3 v )
		//{
		//	Box result;
		//	Vec3.Add( ref b.center, ref v, out result.center );
		//	result.extents = b.extents;
		//	result.axis = b.axis;
		//	return result;
		//}

		//public static Box Multiply( ref Box b, ref Mat3 m )
		//{
		//	Box result;
		//	Multiply( ref b, ref m, out result );
		//	return result;
		//}

		//public static Box Multiply( ref Box b, ref Mat4 m )
		//{
		//	Box result;
		//	Multiply( ref b, ref m, out result );
		//	return result;
		//}

		//public void ToPoints( ref Vec3[] points )
		//{
		//	if( points == null || points.Length < 8 )
		//		points = new Vec3[ 8 ];

		//	Vec3 axMat0;
		//	Vec3.Multiply( extents.x, ref axis.mat0, out axMat0 );
		//	//Vec3 axMat0 = extents.x * axis.mat0;

		//	Vec3 axMat1;
		//	Vec3.Multiply( extents.y, ref  axis.mat1, out axMat1 );
		//	//Vec3 axMat1 = extents.y * axis.mat1;

		//	Vec3 axMat2;
		//	Vec3.Multiply( extents.z, ref axis.mat2, out axMat2 );
		//	//Vec3 axMat2 = extents.z * axis.mat2;

		//	Vec3 temp0;
		//	Vec3.Subtract( ref center, ref axMat0, out temp0 );
		//	//Vec3 temp0 = center - axMat0;

		//	Vec3 temp1;
		//	Vec3.Add( ref center, ref axMat0, out temp1 );
		//	//Vec3 temp1 = center + axMat0;

		//	Vec3 temp2;
		//	Vec3.Subtract( ref axMat1, ref axMat2, out temp2 );
		//	//Vec3 temp2 = axMat1 - axMat2;

		//	Vec3 temp3;
		//	Vec3.Add( ref axMat1, ref axMat2, out temp3 );
		//	//Vec3 temp3 = axMat1 + axMat2;

		//	points[ 0 ] = temp0 - temp3;
		//	points[ 1 ] = temp1 - temp3;
		//	points[ 2 ] = temp1 + temp2;
		//	points[ 3 ] = temp0 + temp2;
		//	points[ 4 ] = temp0 - temp2;
		//	points[ 5 ] = temp1 - temp2;
		//	points[ 6 ] = temp1 + temp3;
		//	points[ 7 ] = temp0 + temp3;
		//}

		//unsafe internal void ToPoints( Vec3* points )
		//{
		//	Vec3 axMat0;
		//	Vec3.Multiply( extents.x, ref axis.mat0, out axMat0 );
		//	//Vec3 axMat0 = extents.x * axis.mat0;

		//	Vec3 axMat1;
		//	Vec3.Multiply( extents.y, ref  axis.mat1, out axMat1 );
		//	//Vec3 axMat1 = extents.y * axis.mat1;

		//	Vec3 axMat2;
		//	Vec3.Multiply( extents.z, ref axis.mat2, out axMat2 );
		//	//Vec3 axMat2 = extents.z * axis.mat2;

		//	Vec3 temp0;
		//	Vec3.Subtract( ref center, ref axMat0, out temp0 );
		//	//Vec3 temp0 = center - axMat0;

		//	Vec3 temp1;
		//	Vec3.Add( ref center, ref axMat0, out temp1 );
		//	//Vec3 temp1 = center + axMat0;

		//	Vec3 temp2;
		//	Vec3.Subtract( ref axMat1, ref axMat2, out temp2 );
		//	//Vec3 temp2 = axMat1 - axMat2;

		//	Vec3 temp3;
		//	Vec3.Add( ref axMat1, ref axMat2, out temp3 );
		//	//Vec3 temp3 = axMat1 + axMat2;

		//	points[ 0 ] = temp0 - temp3;
		//	points[ 1 ] = temp1 - temp3;
		//	points[ 2 ] = temp1 + temp2;
		//	points[ 3 ] = temp0 + temp2;
		//	points[ 4 ] = temp0 - temp2;
		//	points[ 5 ] = temp1 - temp2;
		//	points[ 6 ] = temp1 + temp3;
		//	points[ 7 ] = temp0 + temp3;
		//}

		FORCEINLINE bool contains( const Vector3D& point )
		{
			Vector3D localPoint = point - center;

			if( abs( localPoint.dotProduct( Vector3D( axis[ 0 ][ 0 ], axis[ 0 ][ 1 ], axis[ 0 ][ 2 ] ) ) ) > extents.x ||
				abs( localPoint.dotProduct( Vector3D( axis[ 1 ][ 0 ], axis[ 1 ][ 1 ], axis[ 1 ][ 2 ] ) ) ) > extents.y ||
				abs( localPoint.dotProduct( Vector3D( axis[ 2 ][ 0 ], axis[ 2 ][ 1 ], axis[ 2 ][ 2 ] ) ) ) > extents.z )
			{
				return false;
			}
			return true;
		}

		bool contains( const BoundsD& bounds )
		{
			//TO DO: slowly

			if( !contains( Vector3D( bounds.getMinimum().x, bounds.getMinimum().y, bounds.getMinimum().z ) ) )
				return false;
			if( !contains( Vector3D( bounds.getMinimum().x, bounds.getMinimum().y, bounds.getMaximum().z ) ) )
				return false;
			if( !contains( Vector3D( bounds.getMinimum().x, bounds.getMaximum().y, bounds.getMinimum().z ) ) )
				return false;
			if( !contains( Vector3D( bounds.getMinimum().x, bounds.getMaximum().y, bounds.getMaximum().z ) ) )
				return false;
			if( !contains( Vector3D( bounds.getMaximum().x, bounds.getMinimum().y, bounds.getMinimum().z ) ) )
				return false;
			if( !contains( Vector3D( bounds.getMaximum().x, bounds.getMinimum().y, bounds.getMaximum().z ) ) )
				return false;
			if( !contains( Vector3D( bounds.getMaximum().x, bounds.getMaximum().y, bounds.getMinimum().z ) ) )
				return false;
			if( !contains( Vector3D( bounds.getMaximum().x, bounds.getMaximum().y, bounds.getMaximum().z ) ) )
				return false;
			return true;
		}

		//static bool BoxPlaneClip( float denom, float numer, ref float scale0, ref float scale1 )
		//{
		//	if( denom > 0.0f )
		//	{
		//		if( numer > denom * scale1 )
		//			return false;
		//		if( numer > denom * scale0 )
		//			scale0 = numer / denom;
		//		return true;
		//	}
		//	else if( denom < 0.0f )
		//	{
		//		if( numer > denom * scale0 )
		//			return false;
		//		if( numer > denom * scale1 )
		//			scale1 = numer / denom;
		//		return true;
		//	}
		//	else
		//		return ( numer <= 0.0f );
		//}

		//public bool RayIntersection( Ray ray, out float scale1, out float scale2 )
		//{
		//	Mat3 transposedAxis;
		//	axis.GetTranspose( out transposedAxis );

		//	Vec3 localStart;
		//	Vec3 diff;
		//	Vec3.Subtract( ref ray.origin, ref center, out diff );
		//	Mat3.Multiply( ref diff, ref transposedAxis, out localStart );
		//	//Vec3 localStart = ( ray.Origin - center ) * transposedAxis;

		//	Vec3 localDir;
		//	Mat3.Multiply( ref ray.direction, ref transposedAxis, out localDir );
		//	//Vec3 localDir = ray.Direction * transposedAxis;

		//	float s1 = -MathFunctions.Infinity;
		//	float s2 = MathFunctions.Infinity;
		//	bool ret =
		//		BoxPlaneClip( localDir.x, -localStart.x - extents.x, ref s1, ref s2 ) &&
		//		BoxPlaneClip( -localDir.x, localStart.x - extents.x, ref s1, ref s2 ) &&
		//		BoxPlaneClip( localDir.y, -localStart.y - extents.y, ref s1, ref s2 ) &&
		//		BoxPlaneClip( -localDir.y, localStart.y - extents.y, ref s1, ref s2 ) &&
		//		BoxPlaneClip( localDir.z, -localStart.z - extents.z, ref s1, ref s2 ) &&
		//		BoxPlaneClip( -localDir.z, localStart.z - extents.z, ref s1, ref s2 );

		//	scale1 = s1;
		//	scale2 = s2;

		//	return ret;
		//}

		//public bool RayIntersection( Ray ray )
		//{
		//	float scale1;
		//	float scale2;
		//	return RayIntersection( ray, out scale1, out scale2 );
		//}

		//////not used
		////static float[] ld = new float[ 3 ];
		////public bool LineIntersection( Vec3 start, Vec3 end )
		////{
		////   Vec3 lineDir = 0.5f * ( end - start );
		////   Vec3 lineCenter = start + lineDir;
		////   Vec3 dir = lineCenter - center;

		////   ld[ 0 ] = Math.Abs( Vec3.Dot( lineDir, axis[ 0 ] ) );
		////   if( Math.Abs( Vec3.Dot( dir, axis[ 0 ] ) ) > extents[ 0 ] + ld[ 0 ] )
		////      return false;

		////   ld[ 1 ] = Math.Abs( Vec3.Dot( lineDir, axis[ 1 ] ) );
		////   if( Math.Abs( Vec3.Dot( dir, axis[ 1 ] ) ) > extents[ 1 ] + ld[ 1 ] )
		////      return false;

		////   ld[ 2 ] = Math.Abs( Vec3.Dot( lineDir, axis[ 2 ] ) );
		////   if( Math.Abs( Vec3.Dot( dir, axis[ 2 ] ) ) > extents[ 2 ] + ld[ 2 ] )
		////      return false;

		////   Vec3 cross = lineDir.Cross( dir );

		////   if( Math.Abs( Vec3.Dot( cross, axis[ 0 ] ) ) > extents[ 1 ] * ld[ 2 ] + extents[ 2 ] * ld[ 1 ] )
		////      return false;

		////   if( Math.Abs( Vec3.Dot( cross, axis[ 1 ] ) ) > extents[ 0 ] * ld[ 2 ] + extents[ 2 ] * ld[ 0 ] )
		////      return false;

		////   if( Math.Abs( Vec3.Dot( cross, axis[ 2 ] ) ) > extents[ 0 ] * ld[ 1 ] + extents[ 1 ] * ld[ 0 ] )
		////      return false;

		////   return true;
		////}

		//public bool IsIntersectsBox( ref Box box )
		//{
		//	float c00, c01, c02, c10, c11, c12, c20, c21, c22;
		//	//float c[3][3];		// matrix c = axis.Transpose() * a.axis
		//	float ac00, ac01, ac02, ac10, ac11, ac12, ac20, ac21, ac22;
		//	//float ac[3][3];		// absolute values of c

		//	Vec3 axisDir = Vec3.Zero;	// axis[i] * dir
		//	float d, e0, e1;	// distance between centers and projected extents

		//	Vec3 dir;
		//	Vec3.Subtract( ref box.center, ref center, out dir );
		//	//Vec3 dir = b.center - center;

		//	// axis C0 + t * A0
		//	c00 = Vec3.Dot( ref axis.mat0, ref box.axis.mat0 );
		//	c01 = Vec3.Dot( ref axis.mat0, ref box.axis.mat1 );
		//	c02 = Vec3.Dot( ref axis.mat0, ref box.axis.mat2 );
		//	axisDir.X = Vec3.Dot( ref axis.mat0, ref dir );
		//	ac00 = Math.Abs( c00 );
		//	ac01 = Math.Abs( c01 );
		//	ac02 = Math.Abs( c02 );

		//	d = Math.Abs( axisDir.X );
		//	e0 = extents.X;
		//	e1 = box.extents.X * ac00 + box.extents.Y * ac01 + box.extents.Z * ac02;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A1
		//	c10 = Vec3.Dot( ref axis.mat1, ref box.axis.mat0 );
		//	c11 = Vec3.Dot( ref  axis.mat1, ref box.axis.mat1 );
		//	c12 = Vec3.Dot( ref axis.mat1, ref box.axis.mat2 );
		//	axisDir.Y = Vec3.Dot( ref axis.mat1, ref  dir );
		//	ac10 = Math.Abs( c10 );
		//	ac11 = Math.Abs( c11 );
		//	ac12 = Math.Abs( c12 );

		//	d = Math.Abs( axisDir.Y );
		//	e0 = extents.Y;
		//	e1 = box.extents.X * ac10 + box.extents.Y * ac11 + box.extents.Z * ac12;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A2
		//	c20 = Vec3.Dot( ref axis.mat2, ref box.axis.mat0 );
		//	c21 = Vec3.Dot( ref axis.mat2, ref box.axis.mat1 );
		//	c22 = Vec3.Dot( ref axis.mat2, ref box.axis.mat2 );
		//	axisDir.Z = Vec3.Dot( ref  axis.mat2, ref dir );
		//	ac20 = Math.Abs( c20 );
		//	ac21 = Math.Abs( c21 );
		//	ac22 = Math.Abs( c22 );

		//	d = Math.Abs( axisDir.Z );
		//	e0 = extents.Z;
		//	e1 = box.extents.X * ac20 + box.extents.Y * ac21 + box.extents.Z * ac22;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * B0
		//	d = Math.Abs( Vec3.Dot( ref  box.axis.mat0, ref  dir ) );
		//	e0 = extents.X * ac00 + extents.Y * ac10 + extents.Z * ac20;
		//	e1 = box.extents.X;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * B1
		//	d = Math.Abs( Vec3.Dot( ref  box.axis.mat1, ref dir ) );
		//	e0 = extents.X * ac01 + extents.Y * ac11 + extents.Z * ac21;
		//	e1 = box.extents.Y;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * B2
		//	d = Math.Abs( Vec3.Dot( ref box.axis.mat2, ref dir ) );
		//	e0 = extents.X * ac02 + extents.Y * ac12 + extents.Z * ac22;
		//	e1 = box.extents.Z;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A0xB0
		//	d = Math.Abs( axisDir.Z * c10 - axisDir.Y * c20 );
		//	e0 = extents.Y * ac20 + extents.Z * ac10;
		//	e1 = box.extents.Y * ac02 + box.extents.Z * ac01;
		//	if( d > e0 + e1 )
		//	{
		//		return false;
		//	}

		//	// axis C0 + t * A0xB1
		//	d = Math.Abs( axisDir.Z * c11 - axisDir.Y * c21 );
		//	e0 = extents.Y * ac21 + extents.Z * ac11;
		//	e1 = box.extents.X * ac02 + box.extents.Z * ac00;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A0xB2
		//	d = Math.Abs( axisDir.Z * c12 - axisDir.Y * c22 );
		//	e0 = extents.Y * ac22 + extents.Z * ac12;
		//	e1 = box.extents.X * ac01 + box.extents.Y * ac00;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A1xB0
		//	d = Math.Abs( axisDir.X * c20 - axisDir.Z * c00 );
		//	e0 = extents.X * ac20 + extents.Z * ac00;
		//	e1 = box.extents.Y * ac12 + box.extents.Z * ac11;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A1xB1
		//	d = Math.Abs( axisDir.X * c21 - axisDir.Z * c01 );
		//	e0 = extents.X * ac21 + extents.Z * ac01;
		//	e1 = box.extents.X * ac12 + box.extents.Z * ac10;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A1xB2
		//	d = Math.Abs( axisDir.X * c22 - axisDir.Z * c02 );
		//	e0 = extents.X * ac22 + extents.Z * ac02;
		//	e1 = box.extents.X * ac11 + box.extents.Y * ac10;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A2xB0
		//	d = Math.Abs( axisDir.Y * c00 - axisDir.X * c10 );
		//	e0 = extents.X * ac10 + extents.Y * ac00;
		//	e1 = box.extents.Y * ac22 + box.extents.Z * ac21;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A2xB1
		//	d = Math.Abs( axisDir.Y * c01 - axisDir.X * c11 );
		//	e0 = extents.X * ac11 + extents.Y * ac01;
		//	e1 = box.extents.X * ac22 + box.extents.Z * ac20;
		//	if( d > e0 + e1 )
		//		return false;

		//	// axis C0 + t * A2xB2
		//	d = Math.Abs( axisDir.Y * c02 - axisDir.X * c12 );
		//	e0 = extents.X * ac12 + extents.Y * ac02;
		//	e1 = box.extents.X * ac21 + box.extents.Y * ac20;
		//	if( d > e0 + e1 )
		//		return false;
		//	return true;
		//}

		//public bool IsIntersectsBox( Box box )
		//{
		//	return IsIntersectsBox( ref box );
		//}

		bool intersects( const BoundsD& bounds )
		{
			double c00, c01, c02, c10, c11, c12, c20, c21, c22;
			//double c[3][3]; // matrix c = axis.Transpose() * a.axis
			double ac00, ac01, ac02, ac10, ac11, ac12, ac20, ac21, ac22;
			//double ac[3][3]; // absolute values of c
			double d, e0, e1;	// distance between centers and projected extents
			Vector3D boundsCenter = bounds.getCenter();
			// vector between centers
			Vector3D dir = boundsCenter - center;
			Vector3D axisDir = Vector3D::ZERO;// axis[ i ] * dir
			Vector3D boundsExtents = bounds.getMaximum() - boundsCenter;

			// axis C0 + t * A0
			c00 = axis[ 0 ][ 0 ];//Vec3.Dot( axis[ 0 ], Vec3.XAxis );
			c01 = axis[ 0 ][ 1 ];//Vec3.Dot( axis[ 0 ], Vec3.YAxis );
			c02 = axis[ 0 ][ 2 ];//Vec3.Dot( axis[ 0 ], Vec3.ZAxis );
			axisDir.x = Vector3D( c00, c01, c02 ).dotProduct( dir );
			ac00 = abs( c00 );
			ac01 = abs( c01 );
			ac02 = abs( c02 );

			d = abs( axisDir.x );
			e0 = extents.x;
			e1 = boundsExtents.x * ac00 + boundsExtents.y * ac01 + boundsExtents.z * ac02;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1
			c10 = axis[ 1 ][ 0 ];// Vec3.Dot( axis[ 1 ], Vec3.XAxis );
			c11 = axis[ 1 ][ 1 ];//Vec3.Dot( axis[ 1 ], Vec3.YAxis );
			c12 = axis[ 1 ][ 2 ];//Vec3.Dot( axis[ 1 ], Vec3.ZAxis );
			axisDir.y = Vector3D( c10, c11, c12 ).dotProduct( dir );
			ac10 = abs( c10 );
			ac11 = abs( c11 );
			ac12 = abs( c12 );

			d = abs( axisDir.y );
			e0 = extents.y;
			e1 = boundsExtents.x * ac10 + boundsExtents.y * ac11 + boundsExtents.z * ac12;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2
			c20 = axis[ 2 ][ 0 ];// Vec3.Dot( axis[ 2 ], Vec3.XAxis );
			c21 = axis[ 2 ][ 1 ];// Vec3.Dot( axis[ 2 ], Vec3.YAxis );
			c22 = axis[ 2 ][ 2 ];// Vec3.Dot( axis[ 2 ], Vec3.ZAxis );
			axisDir.z = Vector3D( c20, c21, c22 ).dotProduct( dir );
			ac20 = abs( c20 );
			ac21 = abs( c21 );
			ac22 = abs( c22 );

			d = abs( axisDir.z );
			e0 = extents.z;
			e1 = boundsExtents.x * ac20 + boundsExtents.y * ac21 + boundsExtents.z * ac22;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * B0
			d = abs( dir.x );//Vec3.Dot( Vec3.XAxis, dir ) );
			e0 = extents.x * ac00 + extents.y * ac10 + extents.z * ac20;
			e1 = boundsExtents.x;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * B1
			d = abs( dir.y );//Vec3.Dot( Vec3.YAxis, dir ) );
			e0 = extents.x * ac01 + extents.y * ac11 + extents.z * ac21;
			e1 = boundsExtents.y;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * B2
			d = abs( dir.z );//Vec3.Dot( Vec3.ZAxis, dir ) );
			e0 = extents.x * ac02 + extents.y * ac12 + extents.z * ac22;
			e1 = boundsExtents.z;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A0xB0
			d = abs( axisDir.z * c10 - axisDir.y * c20 );
			e0 = extents.y * ac20 + extents.z * ac10;
			e1 = boundsExtents.y * ac02 + boundsExtents.z * ac01;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A0xB1
			d = abs( axisDir.z * c11 - axisDir.y * c21 );
			e0 = extents.y * ac21 + extents.z * ac11;
			e1 = boundsExtents.x * ac02 + boundsExtents.z * ac00;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A0xB2
			d = abs( axisDir.z * c12 - axisDir.y * c22 );
			e0 = extents.y * ac22 + extents.z * ac12;
			e1 = boundsExtents.x * ac01 + boundsExtents.y * ac00;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1xB0
			d = abs( axisDir.x * c20 - axisDir.z * c00 );
			e0 = extents.x * ac20 + extents.z * ac00;
			e1 = boundsExtents.y * ac12 + boundsExtents.z * ac11;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1xB1
			d = abs( axisDir.x * c21 - axisDir.z * c01 );
			e0 = extents.x * ac21 + extents.z * ac01;
			e1 = boundsExtents.x * ac12 + boundsExtents.z * ac10;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1xB2
			d = abs( axisDir.x * c22 - axisDir.z * c02 );
			e0 = extents.x * ac22 + extents.z * ac02;
			e1 = boundsExtents.x * ac11 + boundsExtents.y * ac10;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2xB0
			d = abs( axisDir.y * c00 - axisDir.x * c10 );
			e0 = extents.x * ac10 + extents.y * ac00;
			e1 = boundsExtents.y * ac22 + boundsExtents.z * ac21;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2xB1
			d = abs( axisDir.y * c01 - axisDir.x * c11 );
			e0 = extents.x * ac11 + extents.y * ac01;
			e1 = boundsExtents.x * ac22 + boundsExtents.z * ac20;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2xB2
			d = abs( axisDir.y * c02 - axisDir.x * c12 );
			e0 = extents.x * ac12 + extents.y * ac02;
			e1 = boundsExtents.x * ac21 + boundsExtents.y * ac20;
			if( d > e0 + e1 )
				return false;

			return true;
		}

		//public bool IsIntersectsBoundsD( BoundsD bounds )
		//{
		//	return IsIntersectsBoundsD( ref bounds );
		//}

		//public Plane.Side GetPlaneSide( ref Plane plane )
		//{
		//	Vec3 localNormal;
		//	Mat3 transposedAxis;
		//	axis.GetTranspose( out transposedAxis );
		//	Vec3 normal = plane.Normal;
		//	Mat3.Multiply( ref normal, ref transposedAxis, out localNormal );
		//	//Vec3 localNormal = plane.Normal * axis.GetTranspose();

		//	float d1 = plane.GetDistance( ref center );
		//	float d2 = Math.Abs( extents.X * localNormal.X ) +
		//		Math.Abs( extents.Y * localNormal.Y ) +
		//		Math.Abs( extents.Z * localNormal.Z );

		//	if( d1 - d2 > 0 )
		//		return Plane.Side.Positive;
		//	if( d1 + d2 < 0 )
		//		return Plane.Side.Negative;
		//	return Plane.Side.No;
		//}

		//public Plane.Side GetPlaneSide( Plane plane )
		//{
		//	return GetPlaneSide( ref plane );
		//}

		//public float GetPlaneDistance( ref Plane plane )
		//{
		//	Vec3 localNormal;
		//	Mat3 transposedAxis;
		//	axis.GetTranspose( out transposedAxis );
		//	Vec3 normal = plane.Normal;
		//	Mat3.Multiply( ref normal, ref transposedAxis, out localNormal );
		//	//Vec3 localNormal = plane.Normal * axis.GetTranspose();

		//	float d1 = plane.GetDistance( ref center );
		//	float d2 = Math.Abs( extents.X * localNormal.X ) +
		//		Math.Abs( extents.Y * localNormal.Y ) +
		//		Math.Abs( extents.Z * localNormal.Z );

		//	if( d1 - d2 > 0 )
		//		return d1 - d2;
		//	if( d1 + d2 < 0 )
		//		return d1 + d2;
		//	return 0;
		//}

		//public float GetPlaneDistance( Plane plane )
		//{
		//	return GetPlaneDistance( ref plane );
		//}

		//public float GetPointDistanceSqr( Vec3 point )
		//{
		//	Vec3 localPoint;
		//	Vec3.Subtract( ref point, ref center, out localPoint );
		//	//Vec3 localPoint = point - center;

		//	float sqr = 0;

		//	float x = Math.Abs( Vec3.Dot( ref localPoint, ref axis.mat0 ) ) - extents.x;
		//	if( x > 0 )
		//		sqr += x * x;
		//	float y = Math.Abs( Vec3.Dot( ref localPoint, ref axis.mat1 ) ) - extents.y;
		//	if( y > 0 )
		//		sqr += y * y;
		//	float z = Math.Abs( Vec3.Dot( ref localPoint, ref axis.mat2 ) ) - extents.z;
		//	if( z > 0 )
		//		sqr += z * z;

		//	return sqr;
		//}

		//public float GetPointDistance( Vec3 point )
		//{
		//	float sqr = GetPointDistanceSqr( point );
		//	if( sqr == 0 )
		//		return 0;
		//	return MathFunctions.Sqrt( sqr );
		//}

		//public BoxD ToBoxD()
		//{
		//	BoxD result;
		//	result.center = center.ToVec3D();
		//	result.extents = extents.ToVec3D();
		//	result.axis = axis.ToMat3D();
		//	return result;
		//}

		//inline BoundsD()
		//{
		//}

		//FORCEINLINE BoundsD( const BoundsD& bounds ) : 
		//	minimum( bounds.minimum ), maximum( bounds.maximum )
		//{
		//}

		//FORCEINLINE BoundsD( const Vector3D& min, const Vector3D& max ) : minimum( min ), maximum( max )
		//{
		//}

		//FORCEINLINE BoundsD( double minX, double minY, double minZ, double maxX, double maxY, double maxZ ) : 
		//	minimum( Vector3D( minX, minY, minZ ) ), maximum( Vector3D( maxX, maxY, maxZ ) )
		//{
		//}

		//BoundsD& operator=(const BoundsD& bounds)
		//{
		//	minimum = bounds.minimum;
		//	maximum = bounds.maximum;
		//	return *this;
		//}

		///** Gets the minimum corner of the box.
		//*/
		//inline const Vector3D& getMinimum(void) const
		//{ 
		//	return minimum; 
		//}

		///** Gets a modifiable version of the minimum
		//corner of the box.
		//*/
		//inline Vector3D& getMinimum(void)
		//{ 
		//	return minimum; 
		//}

		///** Gets the maximum corner of the box.
		//*/
		//inline const Vector3D& getMaximum(void) const
		//{ 
		//	return maximum;
		//}

		///** Gets a modifiable version of the maximum
		//corner of the box.
		//*/
		//inline Vector3D& getMaximum(void)
		//{ 
		//	return maximum;
		//}


		///** Sets the minimum corner of the box.
		//*/
		//inline void setMinimum( const Vector3D& vec )
		//{
		//	minimum = vec;
		//}

		//inline void setMinimum( double x, double y, double z )
		//{
		//	minimum.x = x;
		//	minimum.y = y;
		//	minimum.z = z;
		//}

		///** Changes one of the components of the minimum corner of the box
		//used to resize only one dimension of the box
		//*/
		//inline void setMinimumX(double x)
		//{
		//	minimum.x = x;
		//}

		//inline void setMinimumY(double y)
		//{
		//	minimum.y = y;
		//}

		//inline void setMinimumZ(double z)
		//{
		//	minimum.z = z;
		//}

		///** Sets the maximum corner of the box.
		//*/
		//inline void setMaximum( const Vector3D& vec )
		//{
		//	maximum = vec;
		//}

		//inline void setMaximum( double x, double y, double z )
		//{
		//	maximum.x = x;
		//	maximum.y = y;
		//	maximum.z = z;
		//}

		///** Changes one of the components of the maximum corner of the box
		//used to resize only one dimension of the box
		//*/
		//inline void setMaximumX( double x )
		//{
		//	maximum.x = x;
		//}

		//inline void setMaximumY( double y )
		//{
		//	maximum.y = y;
		//}

		//inline void setMaximumZ( double z )
		//{
		//	maximum.z = z;
		//}

		///** Sets both minimum and maximum extents at once.
		//*/
		//FORCEINLINE void setExtents( const Vector3D& min, const Vector3D& max )
		//{
		//	minimum = min;
		//	maximum = max;
		//}

		//FORCEINLINE void setExtents( double minX, double minY, double minZ, double maxX, double maxY, double maxZ )
		//{
		//	minimum.x = minX;
		//	minimum.y = minY;
		//	minimum.z = minZ;
		//	maximum.x = maxX;
		//	maximum.y = maxY;
		//	maximum.z = maxZ;
		//}

		//_OgreExport friend std::ostream& operator<<( std::ostream& o, const BoundsD aab )
		//{
		//	o << "BoundsD(min=" << aab.minimum << ", max=" << aab.maximum << ")";
		//}

		///** Merges the passed in box into the current box. The result is the
		//box which encompasses both.
		//*/
		//void merge( const BoundsD& rhs )
		//{
		//	Vector3D min = minimum;
		//	Vector3D max = maximum;
		//	max.makeCeil(rhs.maximum);
		//	min.makeFloor(rhs.minimum);
		//	setExtents(min, max);
		//}

		///** Extends the box to encompass the specified point (if needed).
		//*/
		//inline void merge( const Vector3D& point )
		//{
		//	maximum.makeCeil(point);
		//	minimum.makeFloor(point);
		//}

		/////** Transforms the box according to the matrix supplied.
		////@remarks
		////By calling this method you get the axis-aligned box which
		////surrounds the transformed version of this box. Therefore each
		////corner of the box is transformed by the matrix, then the
		////extents are mapped back onto the axes to produce another
		////AABB. Useful when you have a local AABB for an object which
		////is then transformed.
		////*/
		////inline void transform( const Matrix4& matrix )
		////{
		////	Vector3D oldMin, oldMax, currentCorner;

		////	// Getting the old values so that we can use the existing merge method.
		////	oldMin = minimum;
		////	oldMax = maximum;

		////	// reset
		////	setNull();

		////	// We sequentially compute the corners in the following order :
		////	// 0, 6, 5, 1, 2, 4 ,7 , 3
		////	// This sequence allows us to only change one member at a time to get at all corners.

		////	// For each one, we transform it using the matrix
		////	// Which gives the resulting point and merge the resulting point.

		////	// First corner 
		////	// min min min
		////	currentCorner = oldMin;
		////	merge( matrix * currentCorner );

		////	// min,min,max
		////	currentCorner.z = oldMax.z;
		////	merge( matrix * currentCorner );

		////	// min max max
		////	currentCorner.y = oldMax.y;
		////	merge( matrix * currentCorner );

		////	// min max min
		////	currentCorner.z = oldMin.z;
		////	merge( matrix * currentCorner );

		////	// max max min
		////	currentCorner.x = oldMax.x;
		////	merge( matrix * currentCorner );

		////	// max max max
		////	currentCorner.z = oldMax.z;
		////	merge( matrix * currentCorner );

		////	// max min max
		////	currentCorner.y = oldMin.y;
		////	merge( matrix * currentCorner );

		////	// max min min
		////	currentCorner.z = oldMin.z;
		////	merge( matrix * currentCorner ); 
		////}

		/////** Transforms the box according to the affine matrix supplied.
		////@remarks
		////By calling this method you get the axis-aligned box which
		////surrounds the transformed version of this box. Therefore each
		////corner of the box is transformed by the matrix, then the
		////extents are mapped back onto the axes to produce another
		////AABB. Useful when you have a local AABB for an object which
		////is then transformed.
		////@note
		////The matrix must be an affine matrix. @see Matrix4::isAffine.
		////*/
		////void transformAffine(const Matrix4& m)
		////{
		////	assert(m.isAffine());

		////	// Do nothing if current null or infinite
		////	if ( mExtent != EXTENT_FINITE )
		////		return;

		////	Vector3D centre = getCenter();
		////	Vector3D halfSize = getHalfSize();

		////	Vector3D newCentre = m.transformAffine(centre);
		////	Vector3D newHalfSize(
		////		Math::Abs(m[0][0]) * halfSize.x + Math::Abs(m[0][1]) * halfSize.y + Math::Abs(m[0][2]) * halfSize.z, 
		////		Math::Abs(m[1][0]) * halfSize.x + Math::Abs(m[1][1]) * halfSize.y + Math::Abs(m[1][2]) * halfSize.z,
		////		Math::Abs(m[2][0]) * halfSize.x + Math::Abs(m[2][1]) * halfSize.y + Math::Abs(m[2][2]) * halfSize.z);

		////	setExtents(newCentre - newHalfSize, newCentre + newHalfSize);
		////}

		//FORCEINLINE void setZero()
		//{
		//	setExtents( 0, 0, 0, 0, 0, 0 );
		//}

		//FORCEINLINE bool isZero(void) const
		//{
		//	return *this == BOUNDS_ZERO;
		//}

		//bool isCleared(void) const
		//{
		//	return minimum.x > maximum.x;
		//}

		//FORCEINLINE void setCleared()
		//{
		//	setExtents( FLT_MAX, FLT_MAX, FLT_MAX, FLT_MIN, FLT_MIN, FLT_MIN );
		//}

		///** Returns whether or not this box intersects another. */
		//inline bool intersects(const BoundsD& b2) const
		//{
		//	// Use up to 6 separating planes
		//	if (maximum.x < b2.minimum.x)
		//		return false;
		//	if (maximum.y < b2.minimum.y)
		//		return false;
		//	if (maximum.z < b2.minimum.z)
		//		return false;

		//	if (minimum.x > b2.maximum.x)
		//		return false;
		//	if (minimum.y > b2.maximum.y)
		//		return false;
		//	if (minimum.z > b2.maximum.z)
		//		return false;

		//	// otherwise, must be intersecting
		//	return true;
		//}

		/////// Calculate the area of intersection of this box and another
		////inline BoundsD intersection(const BoundsD& b2) const
		////{
		////	Vector3D intMin = minimum;
  ////          Vector3D intMax = maximum;

  ////          intMin.makeCeil(b2.getMinimum());
  ////          intMax.makeFloor(b2.getMaximum());

  ////          // Check intersection isn't null
  ////          if (intMin.x < intMax.x &&
  ////              intMin.y < intMax.y &&
  ////              intMin.z < intMax.z)
  ////          {
  ////              return BoundsD(intMin, intMax);
  ////          }

  ////          return BoundsD();
		////}

		///// Calculate the volume of this box
		//double volume(void) const
		//{
		//	Vector3D diff = maximum - minimum;
		//	return diff.x * diff.y * diff.z;
		//}

		/////** Scales the AABB by the vector given. */
		////inline void scale(const Vector3D& s)
		////{
		////	// Do nothing if current null or infinite
		////	if (mExtent != EXTENT_FINITE)
		////		return;

		////	// NB assumes centered on origin
		////	Vector3D min = minimum * s;
		////	Vector3D max = maximum * s;
		////	setExtents(min, max);
		////}

		/////** Tests whether this box intersects a sphere. */
		////bool intersects(const Sphere& s) const
		////{
		////	return Math::intersects(s, *this); 
		////}
		/////** Tests whether this box intersects a plane. */
		////bool intersects(const Plane& p) const
		////{
		////	return Math::intersects(p, *this);
		////}
		///** Tests whether the vector point is within this box. */
		//bool intersects(const Vector3D& v) const
		//{
		//	return(v.x >= minimum.x  &&  v.x <= maximum.x  && 
		//		v.y >= minimum.y  &&  v.y <= maximum.y  && 
		//		v.z >= minimum.z  &&  v.z <= maximum.z);
		//}
		///// Gets the centre of the box
		//Vector3D getCenter(void) const
		//{
		//	return Vector3D(
		//		(maximum.x + minimum.x) * 0.5f,
		//		(maximum.y + minimum.y) * 0.5f,
		//		(maximum.z + minimum.z) * 0.5f);
		//}
		///// Gets the size of the box
		//Vector3D getSize(void) const
		//{
		//	return maximum - minimum;
		//}
		///// Gets the half-size of the box
		//Vector3D getHalfSize(void) const
		//{
		//	return (maximum - minimum) * 0.5;
		//}

  //      /** Tests whether the given point contained by this box.
  //      */
  //      bool contains(const Vector3D& v) const
  //      {
  //          return minimum.x <= v.x && v.x <= maximum.x &&
  //                 minimum.y <= v.y && v.y <= maximum.y &&
  //                 minimum.z <= v.z && v.z <= maximum.z;
  //      }

  //      /** Tests whether another box contained by this box.
  //      */
  //      bool contains(const BoundsD& other) const
  //      {
  //          return this->minimum.x <= other.minimum.x &&
  //                 this->minimum.y <= other.minimum.y &&
  //                 this->minimum.z <= other.minimum.z &&
  //                 other.maximum.x <= this->maximum.x &&
  //                 other.maximum.y <= this->maximum.y &&
  //                 other.maximum.z <= this->maximum.z;
  //      }

  //      /** Tests 2 boxes for equality.
  //      */
  //      bool operator== (const BoundsD& rhs) const
  //      {
  //          return this->minimum == rhs.minimum && this->maximum == rhs.maximum;
  //      }

  //      /** Tests 2 boxes for inequality.
  //      */
  //      bool operator!= (const BoundsD& rhs) const
  //      {
  //          return !(*this == rhs);
  //      }

		//void add( const Vector3D& v )
		//{
		//	if( v.x < minimum.x )
		//		minimum.x = v.x;
		//	if( v.x > maximum.x )
		//		maximum.x = v.x;
		//	if( v.y < minimum.y )
		//		minimum.y = v.y;
		//	if( v.y > maximum.y )
		//		maximum.y = v.y;
		//	if( v.z < minimum.z )
		//		minimum.z = v.z;
		//	if( v.z > maximum.z )
		//		maximum.z = v.z;
		//}

		//void add( const BoundsD& v )
		//{
		//	if( v.minimum.x < minimum.x )
		//		minimum.x = v.minimum.x;
		//	if( v.minimum.y < minimum.y )
		//		minimum.y = v.minimum.y;
		//	if( v.minimum.z < minimum.z )
		//		minimum.z = v.minimum.z;
		//	if( v.maximum.x > maximum.x )
		//		maximum.x = v.maximum.x;
		//	if( v.maximum.y > maximum.y )
		//		maximum.y = v.maximum.y;
		//	if( v.maximum.z > maximum.z )
		//		maximum.z = v.maximum.z;
		//}

		//void expand( float d )
		//{
		//	minimum.x -= d;
		//	minimum.y -= d;
		//	minimum.z -= d;
		//	maximum.x += d;
		//	maximum.y += d;
		//	maximum.z += d;
		//}

		//void expand( const Vector3D& d )
		//{
		//	minimum.x -= d.x;
		//	minimum.y -= d.y;
		//	minimum.z -= d.z;
		//	maximum.x += d.x;
		//	maximum.y += d.y;
		//	maximum.z += d.z;
		//}

		//const Vector3D& operator[]( const int index ) const
		//{
		//	return *( &minimum + index );
		//}

		//Vector3D& operator[]( const int index )
		//{
		//	return *( &minimum + index );
		//}

		//void toPoints( Vector3D points[ 8 ] ) const
		//{
		//	for( int n = 0; n < 8; n++ )
		//	{
		//		points[ n ] = Vector3D(
		//			( *this )[ ( n ^ ( n >> 1 ) ) & 1 ].x, 
		//			( *this )[ ( n >> 1 ) & 1 ].y, 
		//			( *this )[ ( n >> 2 ) & 1 ].z );
		//		//points[ n ][ 0 ] = this[ ( n ^ ( n >> 1 ) ) & 1 ][ 0 ];
		//		//points[ n ][ 1 ] = this[ ( n >> 1 ) & 1 ][ 1 ];
		//		//points[ n ][ 2 ] = this[ ( n >> 2 ) & 1 ][ 2 ];
		//	}
		//}

		FORCEINLINE BoundsD toBoundsD() const
		{
			Vector3D halfSize = Vector3D(
				abs( axis[ 0 ][ 0 ] * extents.x ) + abs( axis[ 1 ][ 0 ] * extents.y ) + abs( axis[ 2 ][ 0 ] * extents.z ),
				abs( axis[ 0 ][ 1 ] * extents.x ) + abs( axis[ 1 ][ 1 ] * extents.y ) + abs( axis[ 2 ][ 1 ] * extents.z ),
				abs( axis[ 0 ][ 2 ] * extents.x ) + abs( axis[ 1 ][ 2 ] * extents.y ) + abs( axis[ 2 ][ 2 ] * extents.z ) );
			return BoundsD( center - halfSize, center + halfSize );
		}

		// special values
		static const OBBD OBBD_ZERO;
		static const OBBD OBBD_CLEARED;
	};

	/** @} */
	/** @} */
} // namespace Ogre

#endif
