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
#ifndef __RayD_H_
#define __RayD_H_

// Precompiler options
#include "OgrePrerequisites.h"

#include "OgreVector3D.h"
#include "OgrePlaneBoundedVolume.h"
#include "OgreMathD.h"

namespace Ogre {

	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup Math
	*  @{
	*/
    /** Representation of a ray in space, i.e. a line with an origin and direction. */
    class _OgreExport RayD
    {
    protected:
        Vector3D mOrigin;
        Vector3D mDirection;
    public:
        RayD():mOrigin(Vector3D::ZERO), mDirection(Vector3D::UNIT_Z) {}
        RayD(const Vector3D& origin, const Vector3D& direction)
            :mOrigin(origin), mDirection(direction) {}

        /** Sets the origin of the ray. */
        void setOrigin(const Vector3D& origin) {mOrigin = origin;} 
        /** Gets the origin of the ray. */
        const Vector3D& getOrigin(void) const {return mOrigin;} 

        /** Sets the direction of the ray. */
        void setDirection(const Vector3D& dir) {mDirection = dir;} 
        /** Gets the direction of the ray. */
        const Vector3D& getDirection(void) const {return mDirection;} 

		/** Gets the position of a point t units along the ray. */
		Vector3D getPoint(double t) const { 
			return Vector3D(mOrigin + (mDirection * t));
		}
		
		/** Gets the position of a point t units along the ray. */
		Vector3D operator*(double t) const { 
			return getPoint(t);
		}

		/** Tests whether this ray intersects the given plane. 
		@returns A pair structure where the first element indicates whether
			an intersection occurs, and if true, the second element will
			indicate the distance along the ray at which it intersects. 
			This can be converted to a point in space by calling getPoint().
		*/
		std::pair<bool, double> intersects(const PlaneD& p) const
		{
			return MathD::intersects(*this, p);
		}
        ///** Tests whether this ray intersects the given plane bounded volume. 
        //@returns A pair structure where the first element indicates whether
        //an intersection occurs, and if true, the second element will
        //indicate the distance along the ray at which it intersects. 
        //This can be converted to a point in space by calling getPoint().
        //*/
        //std::pair<bool, double> intersects(const PlaneDBoundedVolume& p) const
        //{
        //    return MathD::intersects(*this, p.planes, p.outside == PlaneD::POSITIVE_SIDE);
        //}
		/** Tests whether this ray intersects the given sphere. 
		@returns A pair structure where the first element indicates whether
			an intersection occurs, and if true, the second element will
			indicate the distance along the ray at which it intersects. 
			This can be converted to a point in space by calling getPoint().
		*/
		std::pair<bool, double> intersects(const SphereD& s) const
		{
			return MathD::intersects(*this, s);
		}
		/** Tests whether this ray intersects the given box. 
		@returns A pair structure where the first element indicates whether
			an intersection occurs, and if true, the second element will
			indicate the distance along the ray at which it intersects. 
			This can be converted to a point in space by calling getPoint().
		*/
		std::pair<bool, double> intersects(const AxisAlignedBoxD& box) const
		{
			return MathD::intersects(*this, box);
		}

    };
	/** @} */
	/** @} */

}
#endif
