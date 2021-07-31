// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
#ifndef __Bounds_H_
#define __Bounds_H_

// Precompiler options
#include "OgrePrerequisites.h"

#include "OgreVector3.h"
#include "OgreMatrix4.h"

namespace Ogre {

	class Bounds
	{
	public:
		Vector3 minimum;
		Vector3 maximum;

		inline Bounds()
		{
		}

		FORCEINLINE Bounds( const Bounds& bounds ) : 
			minimum( bounds.minimum ), maximum( bounds.maximum )
		{
		}

		FORCEINLINE Bounds( const Vector3& min, const Vector3& max ) : minimum( min ), maximum( max )
		{
		}

		FORCEINLINE Bounds( Real minX, Real minY, Real minZ, Real maxX, Real maxY, Real maxZ ) : 
			minimum( Vector3( minX, minY, minZ ) ), maximum( Vector3( maxX, maxY, maxZ ) )
		{
		}

		FORCEINLINE Bounds( const Vector3& point ) : 
			minimum( point ), maximum( point )
		{
		}

		Bounds& operator=(const Bounds& bounds)
		{
			minimum = bounds.minimum;
			maximum = bounds.maximum;
			return *this;
		}

		/** Gets the minimum corner of the box.
		*/
		inline const Vector3& getMinimum(void) const
		{ 
			return minimum; 
		}

		/** Gets a modifiable version of the minimum
		corner of the box.
		*/
		inline Vector3& getMinimum(void)
		{ 
			return minimum; 
		}

		/** Gets the maximum corner of the box.
		*/
		inline const Vector3& getMaximum(void) const
		{ 
			return maximum;
		}

		/** Gets a modifiable version of the maximum
		corner of the box.
		*/
		inline Vector3& getMaximum(void)
		{ 
			return maximum;
		}


		/** Sets the minimum corner of the box.
		*/
		inline void setMinimum( const Vector3& vec )
		{
			minimum = vec;
		}

		inline void setMinimum( Real x, Real y, Real z )
		{
			minimum.x = x;
			minimum.y = y;
			minimum.z = z;
		}

		/** Changes one of the components of the minimum corner of the box
		used to resize only one dimension of the box
		*/
		inline void setMinimumX(Real x)
		{
			minimum.x = x;
		}

		inline void setMinimumY(Real y)
		{
			minimum.y = y;
		}

		inline void setMinimumZ(Real z)
		{
			minimum.z = z;
		}

		/** Sets the maximum corner of the box.
		*/
		inline void setMaximum( const Vector3& vec )
		{
			maximum = vec;
		}

		inline void setMaximum( Real x, Real y, Real z )
		{
			maximum.x = x;
			maximum.y = y;
			maximum.z = z;
		}

		/** Changes one of the components of the maximum corner of the box
		used to resize only one dimension of the box
		*/
		inline void setMaximumX( Real x )
		{
			maximum.x = x;
		}

		inline void setMaximumY( Real y )
		{
			maximum.y = y;
		}

		inline void setMaximumZ( Real z )
		{
			maximum.z = z;
		}

		/** Sets both minimum and maximum extents at once.
		*/
		FORCEINLINE void setExtents( const Vector3& min, const Vector3& max )
		{
			minimum = min;
			maximum = max;
		}

		FORCEINLINE void setExtents( Real minX, Real minY, Real minZ, Real maxX, Real maxY, Real maxZ )
		{
			minimum.x = minX;
			minimum.y = minY;
			minimum.z = minZ;
			maximum.x = maxX;
			maximum.y = maxY;
			maximum.z = maxZ;
		}

		//_OgreExport friend std::ostream& operator<<( std::ostream& o, const Bounds aab )
		//{
		//	o << "Bounds(min=" << aab.minimum << ", max=" << aab.maximum << ")";
		//}

		/** Merges the passed in box into the current box. The result is the
		box which encompasses both.
		*/
		void merge( const Bounds& rhs )
		{
			Vector3 min = minimum;
			Vector3 max = maximum;
			max.makeCeil(rhs.maximum);
			min.makeFloor(rhs.minimum);
			setExtents(min, max);
		}

		/** Extends the box to encompass the specified point (if needed).
		*/
		inline void merge( const Vector3& point )
		{
			maximum.makeCeil(point);
			minimum.makeFloor(point);
		}

		///** Transforms the box according to the matrix supplied.
		//@remarks
		//By calling this method you get the axis-aligned box which
		//surrounds the transformed version of this box. Therefore each
		//corner of the box is transformed by the matrix, then the
		//extents are mapped back onto the axes to produce another
		//AABB. Useful when you have a local AABB for an object which
		//is then transformed.
		//*/
		//inline void transform( const Matrix4& matrix )
		//{
		//	Vector3 oldMin, oldMax, currentCorner;

		//	// Getting the old values so that we can use the existing merge method.
		//	oldMin = minimum;
		//	oldMax = maximum;

		//	// reset
		//	setNull();

		//	// We sequentially compute the corners in the following order :
		//	// 0, 6, 5, 1, 2, 4 ,7 , 3
		//	// This sequence allows us to only change one member at a time to get at all corners.

		//	// For each one, we transform it using the matrix
		//	// Which gives the resulting point and merge the resulting point.

		//	// First corner 
		//	// min min min
		//	currentCorner = oldMin;
		//	merge( matrix * currentCorner );

		//	// min,min,max
		//	currentCorner.z = oldMax.z;
		//	merge( matrix * currentCorner );

		//	// min max max
		//	currentCorner.y = oldMax.y;
		//	merge( matrix * currentCorner );

		//	// min max min
		//	currentCorner.z = oldMin.z;
		//	merge( matrix * currentCorner );

		//	// max max min
		//	currentCorner.x = oldMax.x;
		//	merge( matrix * currentCorner );

		//	// max max max
		//	currentCorner.z = oldMax.z;
		//	merge( matrix * currentCorner );

		//	// max min max
		//	currentCorner.y = oldMin.y;
		//	merge( matrix * currentCorner );

		//	// max min min
		//	currentCorner.z = oldMin.z;
		//	merge( matrix * currentCorner ); 
		//}

		///** Transforms the box according to the affine matrix supplied.
		//@remarks
		//By calling this method you get the axis-aligned box which
		//surrounds the transformed version of this box. Therefore each
		//corner of the box is transformed by the matrix, then the
		//extents are mapped back onto the axes to produce another
		//AABB. Useful when you have a local AABB for an object which
		//is then transformed.
		//@note
		//The matrix must be an affine matrix. @see Matrix4::isAffine.
		//*/
		//void transformAffine(const Matrix4& m)
		//{
		//	assert(m.isAffine());

		//	// Do nothing if current null or infinite
		//	if ( mExtent != EXTENT_FINITE )
		//		return;

		//	Vector3 centre = getCenter();
		//	Vector3 halfSize = getHalfSize();

		//	Vector3 newCentre = m.transformAffine(centre);
		//	Vector3 newHalfSize(
		//		Math::Abs(m[0][0]) * halfSize.x + Math::Abs(m[0][1]) * halfSize.y + Math::Abs(m[0][2]) * halfSize.z, 
		//		Math::Abs(m[1][0]) * halfSize.x + Math::Abs(m[1][1]) * halfSize.y + Math::Abs(m[1][2]) * halfSize.z,
		//		Math::Abs(m[2][0]) * halfSize.x + Math::Abs(m[2][1]) * halfSize.y + Math::Abs(m[2][2]) * halfSize.z);

		//	setExtents(newCentre - newHalfSize, newCentre + newHalfSize);
		//}

		FORCEINLINE void setZero()
		{
			setExtents( 0, 0, 0, 0, 0, 0 );
		}

		FORCEINLINE bool isZero(void) const
		{
			return *this == BOUNDS_ZERO;
		}

		bool isCleared(void) const
		{
			return minimum.x > maximum.x;
		}

		FORCEINLINE void setCleared()
		{
			setExtents( FLT_MAX, FLT_MAX, FLT_MAX, FLT_MIN, FLT_MIN, FLT_MIN );
		}

		/** Returns whether or not this box intersects another. */
		inline bool intersects(const Bounds& b2) const
		{
			// Use up to 6 separating planes
			if (maximum.x < b2.minimum.x)
				return false;
			if (maximum.y < b2.minimum.y)
				return false;
			if (maximum.z < b2.minimum.z)
				return false;

			if (minimum.x > b2.maximum.x)
				return false;
			if (minimum.y > b2.maximum.y)
				return false;
			if (minimum.z > b2.maximum.z)
				return false;

			// otherwise, must be intersecting
			return true;
		}

		///// Calculate the area of intersection of this box and another
		//inline Bounds intersection(const Bounds& b2) const
		//{
		//	Vector3 intMin = minimum;
  //          Vector3 intMax = maximum;

  //          intMin.makeCeil(b2.getMinimum());
  //          intMax.makeFloor(b2.getMaximum());

  //          // Check intersection isn't null
  //          if (intMin.x < intMax.x &&
  //              intMin.y < intMax.y &&
  //              intMin.z < intMax.z)
  //          {
  //              return Bounds(intMin, intMax);
  //          }

  //          return Bounds();
		//}

		/// Calculate the volume of this box
		Real volume(void) const
		{
			Vector3 diff = maximum - minimum;
			return diff.x * diff.y * diff.z;
		}

		///** Scales the AABB by the vector given. */
		//inline void scale(const Vector3& s)
		//{
		//	// Do nothing if current null or infinite
		//	if (mExtent != EXTENT_FINITE)
		//		return;

		//	// NB assumes centered on origin
		//	Vector3 min = minimum * s;
		//	Vector3 max = maximum * s;
		//	setExtents(min, max);
		//}

		///** Tests whether this box intersects a sphere. */
		//bool intersects(const Sphere& s) const
		//{
		//	return Math::intersects(s, *this); 
		//}
		///** Tests whether this box intersects a plane. */
		//bool intersects(const Plane& p) const
		//{
		//	return Math::intersects(p, *this);
		//}
		/** Tests whether the vector point is within this box. */
		bool intersects(const Vector3& v) const
		{
			return(v.x >= minimum.x  &&  v.x <= maximum.x  && 
				v.y >= minimum.y  &&  v.y <= maximum.y  && 
				v.z >= minimum.z  &&  v.z <= maximum.z);
		}
		/// Gets the centre of the box
		Vector3 getCenter(void) const
		{
			return Vector3(
				(maximum.x + minimum.x) * 0.5f,
				(maximum.y + minimum.y) * 0.5f,
				(maximum.z + minimum.z) * 0.5f);
		}
		/// Gets the size of the box
		Vector3 getSize(void) const
		{
			return maximum - minimum;
		}
		/// Gets the half-size of the box
		Vector3 getHalfSize(void) const
		{
			return (maximum - minimum) * 0.5;
		}

        /** Tests whether the given point contained by this box.
        */
        bool contains(const Vector3& v) const
        {
            return minimum.x <= v.x && v.x <= maximum.x &&
                   minimum.y <= v.y && v.y <= maximum.y &&
                   minimum.z <= v.z && v.z <= maximum.z;
        }

        /** Tests whether another box contained by this box.
        */
        bool contains(const Bounds& other) const
        {
            return this->minimum.x <= other.minimum.x &&
                   this->minimum.y <= other.minimum.y &&
                   this->minimum.z <= other.minimum.z &&
                   other.maximum.x <= this->maximum.x &&
                   other.maximum.y <= this->maximum.y &&
                   other.maximum.z <= this->maximum.z;
        }

        /** Tests 2 boxes for equality.
        */
        bool operator== (const Bounds& rhs) const
        {
            return this->minimum == rhs.minimum && this->maximum == rhs.maximum;
        }

        /** Tests 2 boxes for inequality.
        */
        bool operator!= (const Bounds& rhs) const
        {
            return !(*this == rhs);
        }

		void add( const Vector3& v )
		{
			if( v.x < minimum.x )
				minimum.x = v.x;
			if( v.x > maximum.x )
				maximum.x = v.x;
			if( v.y < minimum.y )
				minimum.y = v.y;
			if( v.y > maximum.y )
				maximum.y = v.y;
			if( v.z < minimum.z )
				minimum.z = v.z;
			if( v.z > maximum.z )
				maximum.z = v.z;
		}

		void add( const Bounds& v )
		{
			if( v.minimum.x < minimum.x )
				minimum.x = v.minimum.x;
			if( v.minimum.y < minimum.y )
				minimum.y = v.minimum.y;
			if( v.minimum.z < minimum.z )
				minimum.z = v.minimum.z;
			if( v.maximum.x > maximum.x )
				maximum.x = v.maximum.x;
			if( v.maximum.y > maximum.y )
				maximum.y = v.maximum.y;
			if( v.maximum.z > maximum.z )
				maximum.z = v.maximum.z;
		}

		void expand( float d )
		{
			minimum.x -= d;
			minimum.y -= d;
			minimum.z -= d;
			maximum.x += d;
			maximum.y += d;
			maximum.z += d;
		}

		void expand( const Vector3& d )
		{
			minimum.x -= d.x;
			minimum.y -= d.y;
			minimum.z -= d.z;
			maximum.x += d.x;
			maximum.y += d.y;
			maximum.z += d.z;
		}

		const Vector3& operator[]( const int index ) const
		{
			//if( index != 0 )
			//	return maximum;
			//else
			//	return minimum;
			return *( &minimum + index );
		}

		Vector3& operator[]( const int index )
		{
			//if( index != 0 )
			//	return maximum;
			//else
			//	return minimum;
			return *( &minimum + index );
		}

		void toPoints( Vector3 points[ 8 ] ) const
		{
			for( int n = 0; n < 8; n++ )
			{
				points[ n ] = Vector3(
					( *this )[ ( n ^ ( n >> 1 ) ) & 1 ].x, 
					( *this )[ ( n >> 1 ) & 1 ].y, 
					( *this )[ ( n >> 2 ) & 1 ].z );
				//points[ n ][ 0 ] = this[ ( n ^ ( n >> 1 ) ) & 1 ][ 0 ];
				//points[ n ][ 1 ] = this[ ( n >> 1 ) & 1 ][ 1 ];
				//points[ n ][ 2 ] = this[ ( n >> 2 ) & 1 ][ 2 ];
			}
		}

		// special values
		static const Bounds BOUNDS_ZERO;
		static const Bounds BOUNDS_CLEARED;
	};

	/** @} */
	/** @} */
} // namespace Ogre

#endif
