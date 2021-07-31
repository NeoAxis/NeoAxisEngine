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
#include "OgreStableHeaders.h"
#include "OgrePlaneD.h"
#include "OgreMatrix3D.h"
#include "OgreAxisAlignedBoxD.h" 

namespace Ogre {
	//-----------------------------------------------------------------------
	PlaneD::PlaneD ()
	{
		normal = Vector3D::ZERO;
		d = 0.0;
	}
	//-----------------------------------------------------------------------
	PlaneD::PlaneD (const PlaneD& rhs)
	{
		normal = rhs.normal;
		d = rhs.d;
	}
	//-----------------------------------------------------------------------
	PlaneD::PlaneD (const Vector3D& rkNormal, double fConstant)
	{
		normal = rkNormal;
		d = -fConstant;
	}
	//---------------------------------------------------------------------
	PlaneD::PlaneD (double a, double b, double c, double _d)
		: normal(a, b, c), d(_d)
	{
	}
	//-----------------------------------------------------------------------
	PlaneD::PlaneD (const Vector3D& rkNormal, const Vector3D& rkPoint)
	{
		redefine(rkNormal, rkPoint);
	}
	//-----------------------------------------------------------------------
	PlaneD::PlaneD (const Vector3D& rkPoint0, const Vector3D& rkPoint1,
		const Vector3D& rkPoint2)
	{
		redefine(rkPoint0, rkPoint1, rkPoint2);
	}
	//-----------------------------------------------------------------------
	double PlaneD::getDistance (const Vector3D& rkPoint) const
	{
		return normal.dotProduct(rkPoint) + d;
	}
	//-----------------------------------------------------------------------
	PlaneD::Side PlaneD::getSide (const Vector3D& rkPoint) const
	{
		double fDistance = getDistance(rkPoint);

		if ( fDistance < 0.0 )
			return PlaneD::NEGATIVE_SIDE;

		if ( fDistance > 0.0 )
			return PlaneD::POSITIVE_SIDE;

		return PlaneD::NO_SIDE;
	}


	////-----------------------------------------------------------------------
	PlaneD::Side PlaneD::getSide (const AxisAlignedBoxD& box) const
	{
		if (box.isNull()) 
			return NO_SIDE;
		if (box.isInfinite())
			return BOTH_SIDE;

        return getSide(box.getCenter(), box.getHalfSize());
	}
    //-----------------------------------------------------------------------
    PlaneD::Side PlaneD::getSide (const Vector3D& centre, const Vector3D& halfSize) const
    {
        // Calculate the distance between box centre and the plane
        double dist = getDistance(centre);

        // Calculate the maximise allows absolute distance for
        // the distance between box centre and plane
        double maxAbsDist = normal.absDotProduct(halfSize);

        if (dist < -maxAbsDist)
            return PlaneD::NEGATIVE_SIDE;

        if (dist > +maxAbsDist)
            return PlaneD::POSITIVE_SIDE;

        return PlaneD::BOTH_SIDE;
    }
	//-----------------------------------------------------------------------
	void PlaneD::redefine(const Vector3D& rkPoint0, const Vector3D& rkPoint1,
		const Vector3D& rkPoint2)
	{
		Vector3D kEdge1 = rkPoint1 - rkPoint0;
		Vector3D kEdge2 = rkPoint2 - rkPoint0;
		normal = kEdge1.crossProduct(kEdge2);
		normal.normalise();
		d = -normal.dotProduct(rkPoint0);
	}
	//-----------------------------------------------------------------------
	void PlaneD::redefine(const Vector3D& rkNormal, const Vector3D& rkPoint)
	{
		normal = rkNormal;
		d = -rkNormal.dotProduct(rkPoint);
	}
	//-----------------------------------------------------------------------
	Vector3D PlaneD::projectVector(const Vector3D& p) const
	{
		// We know plane normal is unit length, so use simple method
		Matrix3D xform;
		xform[0][0] = 1.0f - normal.x * normal.x;
		xform[0][1] = -normal.x * normal.y;
		xform[0][2] = -normal.x * normal.z;
		xform[1][0] = -normal.y * normal.x;
		xform[1][1] = 1.0f - normal.y * normal.y;
		xform[1][2] = -normal.y * normal.z;
		xform[2][0] = -normal.z * normal.x;
		xform[2][1] = -normal.z * normal.y;
		xform[2][2] = 1.0f - normal.z * normal.z;
		return xform * p;

	}
	//-----------------------------------------------------------------------
    double PlaneD::normalise(void)
    {
        double fLength = normal.length();

        // Will also work for zero-sized vectors, but will change nothing
        if (fLength > 1e-08f)
        {
            double fInvLength = 1.0f / fLength;
            normal *= fInvLength;
            d *= fInvLength;
        }

        return fLength;
    }
	//-----------------------------------------------------------------------
	std::ostream& operator<< (std::ostream& o, const PlaneD& p)
	{
		o << "PlaneD(normal=" << p.normal << ", d=" << p.d << ")";
		return o;
	}
} // namespace Ogre
