#ifndef __AxisAlignedBoxI_H_
#define __AxisAlignedBoxI_H_

// Precompiler options
#include "OgrePrerequisites.h"

#include "Vector3I.h"

namespace Ogre {
	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup Math
	*  @{
	*/

	/** A 3D box aligned with the x/y/z axes.
	@remarks
	This class represents a simple box which is aligned with the
	axes. Internally it only stores 2 points as the extremeties of
	the box, one which is the minima of all 3 axes, and the other
	which is the maxima of all 3 axes. This class is typically used
	for an axis-aligned bounding box (AABB) for collision and
	visibility determination.
	*/
	class AxisAlignedBoxI
	{
	public:
		enum Extent
		{
			EXTENT_NULL,
			EXTENT_FINITE,
			EXTENT_INFINITE
		};
	protected:

		Vector3I mMinimum;
		Vector3I mMaximum;
		Extent mExtent;
		mutable Vector3I* mpCorners;

	public:
		/*
		1-----2
		/|    /|
		/ |   / |
		5-----4  |
		|  0--|--3
		| /   | /
		|/    |/
		6-----7
		*/
		typedef enum {
			FAR_LEFT_BOTTOM = 0,
			FAR_LEFT_TOP = 1,
			FAR_RIGHT_TOP = 2,
			FAR_RIGHT_BOTTOM = 3,
			NEAR_RIGHT_BOTTOM = 7,
			NEAR_LEFT_BOTTOM = 6,
			NEAR_LEFT_TOP = 5,
			NEAR_RIGHT_TOP = 4
		} CornerEnum;
		inline AxisAlignedBoxI() : mMinimum(Vector3I::ZERO), mMaximum(Vector3I::UNIT_SCALE), mpCorners(0)
		{
			// Default to a null box 
			setMinimum( 0, 0, 0 );
			setMaximum( 0, 0, 0 );
			mExtent = EXTENT_NULL;
		}
		inline AxisAlignedBoxI(Extent e) : mMinimum(Vector3I::ZERO), mMaximum(Vector3I::UNIT_SCALE), mpCorners(0)
		{
			setMinimum( 0, 0, 0 );
			setMaximum( 0, 0, 0 );
			mExtent = e;
		}

		inline AxisAlignedBoxI(const AxisAlignedBoxI & rkBox) : mMinimum(Vector3I::ZERO), mMaximum(Vector3I::UNIT_SCALE), mpCorners(0)
		{
			if (rkBox.isNull())
				setNull();
			else if (rkBox.isInfinite())
				setInfinite();
			else
				setExtents( rkBox.mMinimum, rkBox.mMaximum );
		}

		inline AxisAlignedBoxI( const Vector3I& min, const Vector3I& max ) : mMinimum(Vector3I::ZERO), mMaximum(Vector3I::UNIT_SCALE), mpCorners(0)
		{
			setExtents( min, max );
		}

		inline AxisAlignedBoxI(
			int mx, int my, int mz,
			int Mx, int My, int Mz ) : mMinimum(Vector3I::ZERO), mMaximum(Vector3I::UNIT_SCALE), mpCorners(0)
		{
			setExtents( mx, my, mz, Mx, My, Mz );
		}

		AxisAlignedBoxI& operator=(const AxisAlignedBoxI& rhs)
		{
			// Specifically override to avoid copying mpCorners
			if (rhs.isNull())
				setNull();
			else if (rhs.isInfinite())
				setInfinite();
			else
				setExtents(rhs.mMinimum, rhs.mMaximum);

			return *this;
		}

		~AxisAlignedBoxI()
		{
			if (mpCorners)
				OGRE_FREE(mpCorners, MEMCATEGORY_SCENE_CONTROL);
		}


		/** Gets the minimum corner of the box.
		*/
		inline const Vector3I& getMinimum(void) const
		{ 
			return mMinimum; 
		}

		/** Gets a modifiable version of the minimum
		corner of the box.
		*/
		inline Vector3I& getMinimum(void)
		{ 
			return mMinimum; 
		}

		/** Gets the maximum corner of the box.
		*/
		inline const Vector3I& getMaximum(void) const
		{ 
			return mMaximum;
		}

		/** Gets a modifiable version of the maximum
		corner of the box.
		*/
		inline Vector3I& getMaximum(void)
		{ 
			return mMaximum;
		}


		/** Sets the minimum corner of the box.
		*/
		inline void setMinimum( const Vector3I& vec )
		{
			mExtent = EXTENT_FINITE;
			mMinimum = vec;
		}

		inline void setMinimum( int x, int y, int z )
		{
			mExtent = EXTENT_FINITE;
			mMinimum.x = x;
			mMinimum.y = y;
			mMinimum.z = z;
		}

		/** Changes one of the components of the minimum corner of the box
		used to resize only one dimension of the box
		*/
		inline void setMinimumX(int x)
		{
			mMinimum.x = x;
		}

		inline void setMinimumY(int y)
		{
			mMinimum.y = y;
		}

		inline void setMinimumZ(int z)
		{
			mMinimum.z = z;
		}

		/** Sets the maximum corner of the box.
		*/
		inline void setMaximum( const Vector3I& vec )
		{
			mExtent = EXTENT_FINITE;
			mMaximum = vec;
		}

		inline void setMaximum( int x, int y, int z )
		{
			mExtent = EXTENT_FINITE;
			mMaximum.x = x;
			mMaximum.y = y;
			mMaximum.z = z;
		}

		/** Changes one of the components of the maximum corner of the box
		used to resize only one dimension of the box
		*/
		inline void setMaximumX( int x )
		{
			mMaximum.x = x;
		}

		inline void setMaximumY( int y )
		{
			mMaximum.y = y;
		}

		inline void setMaximumZ( int z )
		{
			mMaximum.z = z;
		}

		/** Sets both minimum and maximum extents at once.
		*/
		inline void setExtents( const Vector3I& min, const Vector3I& max )
		{
            assert( (min.x <= max.x && min.y <= max.y && min.z <= max.z) &&
                "The minimum corner of the box must be less than or equal to maximum corner" );

			mExtent = EXTENT_FINITE;
			mMinimum = min;
			mMaximum = max;
		}

		inline void setExtents(
			int mx, int my, int mz,
			int Mx, int My, int Mz )
		{
            assert( (mx <= Mx && my <= My && mz <= Mz) &&
                "The minimum corner of the box must be less than or equal to maximum corner" );

			mExtent = EXTENT_FINITE;

			mMinimum.x = mx;
			mMinimum.y = my;
			mMinimum.z = mz;

			mMaximum.x = Mx;
			mMaximum.y = My;
			mMaximum.z = Mz;

		}

		/** Returns a pointer to an array of 8 corner points, useful for
		collision vs. non-aligned objects.
		@remarks
		If the order of these corners is important, they are as
		follows: The 4 points of the minimum Z face (note that
		because Ogre uses right-handed coordinates, the minimum Z is
		at the 'back' of the box) starting with the minimum point of
		all, then anticlockwise around this face (if you are looking
		onto the face from outside the box). Then the 4 points of the
		maximum Z face, starting with maximum point of all, then
		anticlockwise around this face (looking onto the face from
		outside the box). Like this:
		<pre>
		1-----2
		/|    /|
		/ |   / |
		5-----4  |
		|  0--|--3
		| /   | /
		|/    |/
		6-----7
		</pre>
		@remarks as this implementation uses a static member, make sure to use your own copy !
		*/
		inline const Vector3I* getAllCorners(void) const
		{
			assert( (mExtent == EXTENT_FINITE) && "Can't get corners of a null or infinite AAB" );

			// The order of these items is, using right-handed co-ordinates:
			// Minimum Z face, starting with Min(all), then anticlockwise
			//   around face (looking onto the face)
			// Maximum Z face, starting with Max(all), then anticlockwise
			//   around face (looking onto the face)
			// Only for optimization/compatibility.
			if (!mpCorners)
				mpCorners = OGRE_ALLOC_T(Vector3I, 8, MEMCATEGORY_SCENE_CONTROL);

			mpCorners[0] = mMinimum;
			mpCorners[1].x = mMinimum.x; mpCorners[1].y = mMaximum.y; mpCorners[1].z = mMinimum.z;
			mpCorners[2].x = mMaximum.x; mpCorners[2].y = mMaximum.y; mpCorners[2].z = mMinimum.z;
			mpCorners[3].x = mMaximum.x; mpCorners[3].y = mMinimum.y; mpCorners[3].z = mMinimum.z;            

			mpCorners[4] = mMaximum;
			mpCorners[5].x = mMinimum.x; mpCorners[5].y = mMaximum.y; mpCorners[5].z = mMaximum.z;
			mpCorners[6].x = mMinimum.x; mpCorners[6].y = mMinimum.y; mpCorners[6].z = mMaximum.z;
			mpCorners[7].x = mMaximum.x; mpCorners[7].y = mMinimum.y; mpCorners[7].z = mMaximum.z;

			return mpCorners;
		}

		/** gets the position of one of the corners
		*/
		Vector3I getCorner(CornerEnum cornerToGet) const
		{
			switch(cornerToGet)
			{
			case FAR_LEFT_BOTTOM:
				return mMinimum;
			case FAR_LEFT_TOP:
				return Vector3I(mMinimum.x, mMaximum.y, mMinimum.z);
			case FAR_RIGHT_TOP:
				return Vector3I(mMaximum.x, mMaximum.y, mMinimum.z);
			case FAR_RIGHT_BOTTOM:
				return Vector3I(mMaximum.x, mMinimum.y, mMinimum.z);
			case NEAR_RIGHT_BOTTOM:
				return Vector3I(mMaximum.x, mMinimum.y, mMaximum.z);
			case NEAR_LEFT_BOTTOM:
				return Vector3I(mMinimum.x, mMinimum.y, mMaximum.z);
			case NEAR_LEFT_TOP:
				return Vector3I(mMinimum.x, mMaximum.y, mMaximum.z);
			case NEAR_RIGHT_TOP:
				return mMaximum;
			default:
				return Vector3I();
			}
		}

		_OgreExport friend std::ostream& operator<<( std::ostream& o, const AxisAlignedBoxI aab )
		{
			switch (aab.mExtent)
			{
			case EXTENT_NULL:
				o << "AxisAlignedBoxI(null)";
				return o;

			case EXTENT_FINITE:
				o << "AxisAlignedBoxI(min=" << aab.mMinimum << ", max=" << aab.mMaximum << ")";
				return o;

			case EXTENT_INFINITE:
				o << "AxisAlignedBoxI(infinite)";
				return o;

			default: // shut up compiler
				assert( false && "Never reached" );
				return o;
			}
		}

		/** Merges the passed in box into the current box. The result is the
		box which encompasses both.
		*/
		void merge( const AxisAlignedBoxI& rhs )
		{
			// Do nothing if rhs null, or this is infinite
			if ((rhs.mExtent == EXTENT_NULL) || (mExtent == EXTENT_INFINITE))
			{
				return;
			}
			// Otherwise if rhs is infinite, make this infinite, too
			else if (rhs.mExtent == EXTENT_INFINITE)
			{
				mExtent = EXTENT_INFINITE;
			}
			// Otherwise if current null, just take rhs
			else if (mExtent == EXTENT_NULL)
			{
				setExtents(rhs.mMinimum, rhs.mMaximum);
			}
			// Otherwise merge
			else
			{
				Vector3I min = mMinimum;
				Vector3I max = mMaximum;
				max.makeCeil(rhs.mMaximum);
				min.makeFloor(rhs.mMinimum);

				setExtents(min, max);
			}

		}

		/** Extends the box to encompass the specified point (if needed).
		*/
		inline void merge( const Vector3I& point )
		{
			switch (mExtent)
			{
			case EXTENT_NULL: // if null, use this point
				setExtents(point, point);
				return;

			case EXTENT_FINITE:
				mMaximum.makeCeil(point);
				mMinimum.makeFloor(point);
				return;

			case EXTENT_INFINITE: // if infinite, makes no difference
				return;
			}

			assert( false && "Never reached" );
		}

		/** Sets the box to a 'null' value i.e. not a box.
		*/
		inline void setNull()
		{
			mExtent = EXTENT_NULL;
		}

		/** Returns true if the box is null i.e. empty.
		*/
		inline bool isNull(void) const
		{
			return (mExtent == EXTENT_NULL);
		}

		/** Returns true if the box is finite.
		*/
		bool isFinite(void) const
		{
			return (mExtent == EXTENT_FINITE);
		}

		/** Sets the box to 'infinite'
		*/
		inline void setInfinite()
		{
			mExtent = EXTENT_INFINITE;
		}

		/** Returns true if the box is infinite.
		*/
		bool isInfinite(void) const
		{
			return (mExtent == EXTENT_INFINITE);
		}

		/** Returns whether or not this box intersects another. */
		inline bool intersects(const AxisAlignedBoxI& b2) const
		{
			// Early-fail for nulls
			if (this->isNull() || b2.isNull())
				return false;

			// Early-success for infinites
			if (this->isInfinite() || b2.isInfinite())
				return true;

			// Use up to 6 separating planes
			if (mMaximum.x < b2.mMinimum.x)
				return false;
			if (mMaximum.y < b2.mMinimum.y)
				return false;
			if (mMaximum.z < b2.mMinimum.z)
				return false;

			if (mMinimum.x > b2.mMaximum.x)
				return false;
			if (mMinimum.y > b2.mMaximum.y)
				return false;
			if (mMinimum.z > b2.mMaximum.z)
				return false;

			// otherwise, must be intersecting
			return true;

		}

		/// Calculate the area of intersection of this box and another
		inline AxisAlignedBoxI intersection(const AxisAlignedBoxI& b2) const
		{
            if (this->isNull() || b2.isNull())
			{
				return AxisAlignedBoxI();
			}
			else if (this->isInfinite())
			{
				return b2;
			}
			else if (b2.isInfinite())
			{
				return *this;
			}

			Vector3I intMin = mMinimum;
            Vector3I intMax = mMaximum;

            intMin.makeCeil(b2.getMinimum());
            intMax.makeFloor(b2.getMaximum());

            // Check intersection isn't null
            if (intMin.x < intMax.x &&
                intMin.y < intMax.y &&
                intMin.z < intMax.z)
            {
                return AxisAlignedBoxI(intMin, intMax);
            }

            return AxisAlignedBoxI();
		}

		/// Calculate the volume of this box
		int volume(void) const
		{
			switch (mExtent)
			{
			case EXTENT_NULL:
				return 0.0f;

			case EXTENT_FINITE:
				{
					Vector3I diff = mMaximum - mMinimum;
					return diff.x * diff.y * diff.z;
				}

			case EXTENT_INFINITE:
				return Math::POS_INFINITY;

			default: // shut up compiler
				assert( false && "Never reached" );
				return 0.0f;
			}
		}

		/// Gets the centre of the box
		Vector3I getCenter(void) const
		{
			assert( (mExtent == EXTENT_FINITE) && "Can't get center of a null or infinite AAB" );

			return Vector3I(
				(mMaximum.x + mMinimum.x) / 2,
				(mMaximum.y + mMinimum.y) / 2,
				(mMaximum.z + mMinimum.z) / 2);
		}
		/// Gets the size of the box
		Vector3I getSize(void) const
		{
			switch (mExtent)
			{
			case EXTENT_NULL:
				return Vector3I::ZERO;

			case EXTENT_FINITE:
				return mMaximum - mMinimum;

			case EXTENT_INFINITE:
				return Vector3I(
					Math::POS_INFINITY,
					Math::POS_INFINITY,
					Math::POS_INFINITY);

			default: // shut up compiler
				assert( false && "Never reached" );
				return Vector3I::ZERO;
			}
		}

        /** Tests whether the given point contained by this box.
        */
        bool contains(const Vector3I& v) const
        {
            if (isNull())
                return false;
            if (isInfinite())
                return true;

            return mMinimum.x <= v.x && v.x <= mMaximum.x &&
                   mMinimum.y <= v.y && v.y <= mMaximum.y &&
                   mMinimum.z <= v.z && v.z <= mMaximum.z;
        }

        /** Tests whether another box contained by this box.
        */
        bool contains(const AxisAlignedBoxI& other) const
        {
            if (other.isNull() || this->isInfinite())
                return true;

            if (this->isNull() || other.isInfinite())
                return false;

            return this->mMinimum.x <= other.mMinimum.x &&
                   this->mMinimum.y <= other.mMinimum.y &&
                   this->mMinimum.z <= other.mMinimum.z &&
                   other.mMaximum.x <= this->mMaximum.x &&
                   other.mMaximum.y <= this->mMaximum.y &&
                   other.mMaximum.z <= this->mMaximum.z;
        }

        /** Tests 2 boxes for equality.
        */
        bool operator== (const AxisAlignedBoxI& rhs) const
        {
            if (this->mExtent != rhs.mExtent)
                return false;

            if (!this->isFinite())
                return true;

            return this->mMinimum == rhs.mMinimum &&
                   this->mMaximum == rhs.mMaximum;
        }

        /** Tests 2 boxes for inequality.
        */
        bool operator!= (const AxisAlignedBoxI& rhs) const
        {
            return !(*this == rhs);
        }

		// special values
		static const AxisAlignedBoxI BOX_NULL;
		static const AxisAlignedBoxI BOX_INFINITE;


	};

	/** @} */
	/** @} */
} // namespace Ogre

#endif
