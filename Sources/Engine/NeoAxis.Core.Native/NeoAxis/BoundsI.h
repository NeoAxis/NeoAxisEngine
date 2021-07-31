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
#ifndef __BoundsI_H_
#define __BoundsI_H_

// Precompiler options
#include "OgrePrerequisites.h"

#include "Vector3I.h"

namespace Ogre {

	class BoundsI
	{
	public:
		Vector3I minimum;
		Vector3I maximum;

		FORCEINLINE BoundsI()
		{
		}

		FORCEINLINE BoundsI( const BoundsI& bounds ) : 
			minimum( bounds.minimum ), maximum( bounds.maximum )
		{
		}

		FORCEINLINE BoundsI( const Vector3I& min, const Vector3I& max ) : minimum( min ), maximum( max )
		{
		}

		FORCEINLINE BoundsI( int minX, int minY, int minZ, int maxX, int maxY, int maxZ ) : 
			minimum( Vector3I( minX, minY, minZ ) ), maximum( Vector3I( maxX, maxY, maxZ ) )
		{
		}

		BoundsI& operator = ( const BoundsI& bounds )
		{
			minimum = bounds.minimum;
			maximum = bounds.maximum;
			return *this;
		}

		/** Gets the minimum corner of the box.
		*/
		FORCEINLINE const Vector3I& getMinimum(void) const
		{ 
			return minimum; 
		}

		/** Gets a modifiable version of the minimum
		corner of the box.
		*/
		FORCEINLINE Vector3I& getMinimum(void)
		{ 
			return minimum; 
		}

		/** Gets the maximum corner of the box.
		*/
		FORCEINLINE const Vector3I& getMaximum(void) const
		{ 
			return maximum;
		}

		/** Gets a modifiable version of the maximum
		corner of the box.
		*/
		FORCEINLINE Vector3I& getMaximum(void)
		{ 
			return maximum;
		}


		/** Sets the minimum corner of the box.
		*/
		FORCEINLINE void setMinimum( const Vector3I& vec )
		{
			minimum = vec;
		}

		FORCEINLINE void setMinimum( int x, int y, int z )
		{
			minimum.x = x;
			minimum.y = y;
			minimum.z = z;
		}

		/** Changes one of the components of the minimum corner of the box
		used to resize only one dimension of the box
		*/
		FORCEINLINE void setMinimumX(int x)
		{
			minimum.x = x;
		}

		FORCEINLINE void setMinimumY(int y)
		{
			minimum.y = y;
		}

		FORCEINLINE void setMinimumZ(int z)
		{
			minimum.z = z;
		}

		/** Sets the maximum corner of the box.
		*/
		FORCEINLINE void setMaximum( const Vector3I& vec )
		{
			maximum = vec;
		}

		FORCEINLINE void setMaximum( int x, int y, int z )
		{
			maximum.x = x;
			maximum.y = y;
			maximum.z = z;
		}

		/** Changes one of the components of the maximum corner of the box
		used to resize only one dimension of the box
		*/
		FORCEINLINE void setMaximumX( int x )
		{
			maximum.x = x;
		}

		FORCEINLINE void setMaximumY( int y )
		{
			maximum.y = y;
		}

		FORCEINLINE void setMaximumZ( int z )
		{
			maximum.z = z;
		}

		/** Sets both minimum and maximum extents at once.
		*/
		FORCEINLINE void setExtents( const Vector3I& min, const Vector3I& max )
		{
			minimum = min;
			maximum = max;
		}

		FORCEINLINE void setExtents( int minX, int minY, int minZ, int maxX, int maxY, int maxZ )
		{
			minimum.x = minX;
			minimum.y = minY;
			minimum.z = minZ;
			maximum.x = maxX;
			maximum.y = maxY;
			maximum.z = maxZ;
		}

		//_OgreExport friend std::ostream& operator<<( std::ostream& o, const BoundsI aab )
		//{
		//	o << "BoundsI(min=" << aab.minimum << ", max=" << aab.maximum << ")";
		//}

		/** Merges the passed in box into the current box. The result is the
		box which encompasses both.
		*/
		void merge( const BoundsI& rhs )
		{
			Vector3I min = minimum;
			Vector3I max = maximum;
			max.makeCeil( rhs.maximum );
			min.makeFloor( rhs.minimum );
			setExtents( min, max );
		}

		/** Extends the box to encompass the specified point (if needed).
		*/
		FORCEINLINE void merge( const Vector3I& point )
		{
			maximum.makeCeil( point );
			minimum.makeFloor( point);
		}

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
			setExtents( INT_MAX, INT_MAX, INT_MAX, INT_MIN, INT_MIN, INT_MIN );
		}

		/** Returns whether or not this box intersects another. */
		FORCEINLINE bool intersects(const BoundsI& b2) const
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
		//FORCEINLINE BoundsI intersection(const BoundsI& b2) const
		//{
		//	Vector3I intMin = mMinimum;
  //          Vector3I intMax = mMaximum;

  //          intMin.makeCeil(b2.getMinimum());
  //          intMax.makeFloor(b2.getMaximum());

  //          // Check intersection isn't null
  //          if (intMin.x < intMax.x &&
  //              intMin.y < intMax.y &&
  //              intMin.z < intMax.z)
  //          {
  //              return BoundsI(intMin, intMax);
  //          }

  //          return BoundsI();
		//}

		///// Calculate the volume of this box
		//int volume(void) const
		//{
		//	switch (mExtent)
		//	{
		//	case EXTENT_NULL:
		//		return 0.0f;

		//	case EXTENT_FINITE:
		//		{
		//			Vector3I diff = mMaximum - mMinimum;
		//			return diff.x * diff.y * diff.z;
		//		}

		//	case EXTENT_INFINITE:
		//		return Math::POS_INFINITY;

		//	default: // shut up compiler
		//		assert( false && "Never reached" );
		//		return 0.0f;
		//	}
		//}

		///// Gets the centre of the box
		//Vector3I getCenter(void) const
		//{
		//	assert( (mExtent == EXTENT_FINITE) && "Can't get center of a null or infinite AAB" );

		//	return Vector3I(
		//		(mMaximum.x + mMinimum.x) / 2,
		//		(mMaximum.y + mMinimum.y) / 2,
		//		(mMaximum.z + mMinimum.z) / 2);
		//}

		/// Gets the size of the box
		FORCEINLINE Vector3I getSize(void) const
		{
			return maximum - minimum;
		}

        /** Tests whether the given point contained by this box.
        */
        FORCEINLINE bool contains(const Vector3I& v) const
        {
            return minimum.x <= v.x && v.x <= maximum.x &&
                   minimum.y <= v.y && v.y <= maximum.y &&
                   minimum.z <= v.z && v.z <= maximum.z;
        }

        /** Tests whether another box contained by this box.
        */
        FORCEINLINE bool contains(const BoundsI& other) const
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
        bool operator== (const BoundsI& rhs) const
        {
            return minimum == rhs.minimum && maximum == rhs.maximum;
        }

        /** Tests 2 boxes for inequality.
        */
        bool operator!= (const BoundsI& rhs) const
        {
            return !(*this == rhs);
        }

		// special values
		static const BoundsI BOUNDS_ZERO;
		static const BoundsI BOUNDS_CLEARED;
	};

	/** @} */
	/** @} */
} // namespace Ogre

#endif
