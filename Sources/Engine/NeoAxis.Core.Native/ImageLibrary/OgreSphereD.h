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
#ifndef __SphereD_H_
#define __SphereD_H_

// Precompiler options
#include "OgrePrerequisites.h"

#include "OgreVector3D.h"
#include "OgreAxisAlignedBoxD.h"
#include "OgrePlaneD.h"
#include "OgreMathD.h"

namespace Ogre {


	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup Math
	*  @{
	*/
    /** A sphere primitive, mostly used for bounds checking. 
    @remarks
        A sphere in math texts is normally represented by the function
        x^2 + y^2 + z^2 = r^2 (for sphere's centered on the origin). Ogre stores spheres
        simply as a center point and a radius.
    */
    class _OgreExport SphereD
    {
    protected:
        double mRadius;
        Vector3D mCenter;
    public:
        /** Standard constructor - creates a unit sphere around the origin.*/
        SphereD() : mRadius(1.0), mCenter(Vector3D::ZERO) {}
        /** Constructor allowing arbitrary spheres. 
            @param center The center point of the sphere.
            @param radius The radius of the sphere.
        */
        SphereD(const Vector3D& center, double radius)
            : mRadius(radius), mCenter(center) {}

        /** Returns the radius of the sphere. */
        double getRadius(void) const { return mRadius; }

        /** Sets the radius of the sphere. */
        void setRadius(double radius) { mRadius = radius; }

        /** Returns the center point of the sphere. */
        const Vector3D& getCenter(void) const { return mCenter; }

        /** Sets the center point of the sphere. */
        void setCenter(const Vector3D& center) { mCenter = center; }

		/** Returns whether or not this sphere intersects another sphere. */
		bool intersects(const SphereD& s) const
		{
            return (s.mCenter - mCenter).squaredLength() <=
                Math::Sqr(s.mRadius + mRadius);
		}
		/** Returns whether or not this sphere intersects a box. */
		bool intersects(const AxisAlignedBoxD& box) const
		{
			return MathD::intersects(*this, box);
		}
		/** Returns whether or not this sphere intersects a plane. */
		bool intersects(const PlaneD& plane) const
		{
			return MathD::intersects(*this, plane);
		}
		/** Returns whether or not this sphere intersects a point. */
		bool intersects(const Vector3D& v) const
		{
            return ((v - mCenter).squaredLength() <= Math::Sqr(mRadius));
		}
        

    };
	/** @} */
	/** @} */

}

#endif

