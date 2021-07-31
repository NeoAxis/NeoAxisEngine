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

#include "OgreMathD.h"
//#include "asm_math.h"
#include "OgreVector2.h"
#include "OgreVector3D.h"
#include "OgreVector4.h"
#include "OgreRayD.h"
#include "OgreSphereD.h"
#include "OgreAxisAlignedBox.h"
#include "OgrePlaneD.h"
#include "OgreMatrix3D.h"


namespace Ogre
{

    const double MathD::POS_INFINITY = std::numeric_limits<double>::infinity();
    const double MathD::NEG_INFINITY = -std::numeric_limits<double>::infinity();
    const double MathD::PI = double( 4.0 * atan( 1.0 ) );
    const double MathD::TWO_PI = double( 2.0 * PI );
    const double MathD::HALF_PI = double( 0.5 * PI );
	const double MathD::fDeg2Rad = PI / double(180.0);
	const double MathD::fRad2Deg = double(180.0) / PI;
	const double MathD::LOG2 = log(double(2.0));

   // int MathD::mTrigTableSize;
	MathD::AngleDUnit MathD::msAngleDUnit = AU_DEGREE;
   //MathD::AngleDUnit MathD::msAngleDUnit;

    //double  MathD::mTrigTableFactor;
    //double *MathD::mSinTable = NULL;
    //double *MathD::mTanTable = NULL;

    //-----------------------------------------------------------------------
    //MathD::MathD( unsigned int trigTableSize )
    //{
    //    msAngleDUnit = AU_DEGREE;

    //    mTrigTableSize = trigTableSize;
    //    mTrigTableFactor = mTrigTableSize / MathD::TWO_PI;

    //    mSinTable = OGRE_ALLOC_T(double, mTrigTableSize, MEMCATEGORY_GENERAL);
    //    mTanTable = OGRE_ALLOC_T(double, mTrigTableSize, MEMCATEGORY_GENERAL);

    //    buildTrigTables();
    //}

    //-----------------------------------------------------------------------
    //MathD::~MathD()
    //{
    //    OGRE_FREE(mSinTable, MEMCATEGORY_GENERAL);
    //    OGRE_FREE(mTanTable, MEMCATEGORY_GENERAL);
    //}

    //-----------------------------------------------------------------------
    //void MathD::buildTrigTables(void)
    //{
    //    // Build trig lookup tables
    //    // Could get away with building only PI sized Sin table but simpler this 
    //    // way. Who cares, it'll ony use an extra 8k of memory anyway and I like 
    //    // simplicity.
    //    double angle;
    //    for (int i = 0; i < mTrigTableSize; ++i)
    //    {
    //        angle = MathD::TWO_PI * i / mTrigTableSize;
    //        mSinTable[i] = sin(angle);
    //        mTanTable[i] = tan(angle);
    //    }
    //}
	//-----------------------------------------------------------------------	
	//double MathD::SinTable (double fValue)
 //   {
 //       // Convert range to index values, wrap if required
 //       int idx;
 //       if (fValue >= 0)
 //       {
 //           idx = int(fValue * mTrigTableFactor) % mTrigTableSize;
 //       }
 //       else
 //       {
 //           idx = mTrigTableSize - (int(-fValue * mTrigTableFactor) % mTrigTableSize) - 1;
 //       }

 //       return mSinTable[idx];
 //   }
	//-----------------------------------------------------------------------
	//double MathD::TanTable (double fValue)
 //   {
 //       // Convert range to index values, wrap if required
	//	int idx = int(fValue *= mTrigTableFactor) % mTrigTableSize;
	//	return mTanTable[idx];
 //   }
    //-----------------------------------------------------------------------
    int MathD::ISign (int iValue)
    {
        return ( iValue > 0 ? +1 : ( iValue < 0 ? -1 : 0 ) );
    }
    //-----------------------------------------------------------------------
    RadianD MathD::ACos (double fValue)
    {
        if ( -1.0 < fValue )
        {
            if ( fValue < 1.0 )
                return RadianD(acos(fValue));
            else
                return RadianD(0.0);
        }
        else
        {
            return RadianD(PI);
        }
    }
    //-----------------------------------------------------------------------
    RadianD MathD::ASin (double fValue)
    {
        if ( -1.0 < fValue )
        {
            if ( fValue < 1.0 )
                return RadianD(asin(fValue));
            else
                return RadianD(HALF_PI);
        }
        else
        {
            return RadianD(-HALF_PI);
        }
    }
    //-----------------------------------------------------------------------
    double MathD::Sign (double fValue)
    {
        if ( fValue > 0.0 )
            return 1.0;

        if ( fValue < 0.0 )
            return -1.0;

        return 0.0;
    }
	//-----------------------------------------------------------------------
	double MathD::InvSqrt(double fValue)
	{
		return 1. / sqrt(fValue);
		//return double(asm_rsq(fValue));
	}
    //-----------------------------------------------------------------------
    double MathD::UnitRandom ()
    {
		Fatal("impl 1");
		return 0;
        //return asm_rand() / asm_rand_max();
    }
    
    //-----------------------------------------------------------------------
    double MathD::RangeRandom (double fLow, double fHigh)
    {
        return (fHigh-fLow)*UnitRandom() + fLow;
    }

    //-----------------------------------------------------------------------
    double MathD::SymmetricRandom ()
    {
		return 2.0f * UnitRandom() - 1.0f;
    }

   ////-----------------------------------------------------------------------
   // void MathD::setAngleDUnit(MathD::AngleDUnit unit)
   //{
   //    msAngleDUnit = unit;
   //}
   ////-----------------------------------------------------------------------
   //MathD::AngleDUnit MathD::getAngleDUnit(void)
   //{
   //    return msAngleDUnit;
   //}
    //-----------------------------------------------------------------------
    double MathD::AngleDUnitsToRadianDs(double angleunits)
    {
       if (msAngleDUnit == AU_DEGREE)
           return angleunits * fDeg2Rad;
       else
           return angleunits;
    }

    //-----------------------------------------------------------------------
    double MathD::RadianDsToAngleDUnits(double radians)
    {
       if (msAngleDUnit == AU_DEGREE)
           return radians * fRad2Deg;
       else
           return radians;
    }

    //-----------------------------------------------------------------------
    double MathD::AngleDUnitsToDegreeDs(double angleunits)
    {
       if (msAngleDUnit == AU_RADIAN)
           return angleunits * fRad2Deg;
       else
           return angleunits;
    }

    //-----------------------------------------------------------------------
    double MathD::DegreeDsToAngleDUnits(double degrees)
    {
       if (msAngleDUnit == AU_RADIAN)
           return degrees * fDeg2Rad;
       else
           return degrees;
    }

    //-----------------------------------------------------------------------
	bool MathD::pointInTri2D(const Vector2& p, const Vector2& a, 
		const Vector2& b, const Vector2& c)
    {
		// Winding must be consistent from all edges for point to be inside
		Vector2 v1, v2;
		double dot[3];
		bool zeroDot[3];

		v1 = b - a;
		v2 = p - a;

		// Note we don't care about normalisation here since sign is all we need
		// It means we don't have to worry about magnitude of cross products either
		dot[0] = v1.crossProduct(v2);
		zeroDot[0] = MathD::doubleEqual(dot[0], 0.0f, 1e-3);


		v1 = c - b;
		v2 = p - b;

		dot[1] = v1.crossProduct(v2);
		zeroDot[1] = MathD::doubleEqual(dot[1], 0.0f, 1e-3);

		// Compare signs (ignore colinear / coincident points)
		if(!zeroDot[0] && !zeroDot[1] 
		&& MathD::Sign(dot[0]) != MathD::Sign(dot[1]))
		{
			return false;
		}

		v1 = a - c;
		v2 = p - c;

		dot[2] = v1.crossProduct(v2);
		zeroDot[2] = MathD::doubleEqual(dot[2], 0.0f, 1e-3);
		// Compare signs (ignore colinear / coincident points)
		if((!zeroDot[0] && !zeroDot[2] 
			&& MathD::Sign(dot[0]) != MathD::Sign(dot[2])) ||
			(!zeroDot[1] && !zeroDot[2] 
			&& MathD::Sign(dot[1]) != MathD::Sign(dot[2])))
		{
			return false;
		}


		return true;
    }
	//-----------------------------------------------------------------------
	bool MathD::pointInTri3D(const Vector3D& p, const Vector3D& a, 
		const Vector3D& b, const Vector3D& c, const Vector3D& normal)
	{
        // Winding must be consistent from all edges for point to be inside
		Vector3D v1, v2;
		double dot[3];
		bool zeroDot[3];

        v1 = b - a;
        v2 = p - a;

		// Note we don't care about normalisation here since sign is all we need
		// It means we don't have to worry about magnitude of cross products either
        dot[0] = v1.crossProduct(v2).dotProduct(normal);
		zeroDot[0] = MathD::doubleEqual(dot[0], 0.0f, 1e-3);


        v1 = c - b;
        v2 = p - b;

		dot[1] = v1.crossProduct(v2).dotProduct(normal);
		zeroDot[1] = MathD::doubleEqual(dot[1], 0.0f, 1e-3);

		// Compare signs (ignore colinear / coincident points)
		if(!zeroDot[0] && !zeroDot[1] 
			&& MathD::Sign(dot[0]) != MathD::Sign(dot[1]))
		{
            return false;
		}

        v1 = a - c;
        v2 = p - c;

		dot[2] = v1.crossProduct(v2).dotProduct(normal);
		zeroDot[2] = MathD::doubleEqual(dot[2], 0.0f, 1e-3);
		// Compare signs (ignore colinear / coincident points)
		if((!zeroDot[0] && !zeroDot[2] 
			&& MathD::Sign(dot[0]) != MathD::Sign(dot[2])) ||
			(!zeroDot[1] && !zeroDot[2] 
			&& MathD::Sign(dot[1]) != MathD::Sign(dot[2])))
		{
			return false;
		}


        return true;
	}
    //-----------------------------------------------------------------------
    bool MathD::doubleEqual( double a, double b, double tolerance )
    {
        if (fabs(b-a) <= tolerance)
            return true;
        else
            return false;
    }

    //-----------------------------------------------------------------------
    std::pair<bool, double> MathD::intersects(const RayD& ray, const PlaneD& plane)
    {

        double denom = plane.normal.dotProduct(ray.getDirection());
        if (MathD::Abs(denom) < std::numeric_limits<double>::epsilon())
        {
            // Parallel
            return std::pair<bool, double>(false, 0);
        }
        else
        {
            double nom = plane.normal.dotProduct(ray.getOrigin()) + plane.d;
            double t = -(nom/denom);
            return std::pair<bool, double>(t >= 0, t);
        }
        
    }
    //-----------------------------------------------------------------------
    std::pair<bool, double> MathD::intersects(const RayD& ray, 
        const vector<PlaneD>& planes, bool normalIsOutside)
    {
		list<PlaneD> planesList;
		for (vector<PlaneD>::const_iterator i = planes.begin(); i != planes.end(); ++i)
		{
			planesList.push_back(*i);
		}
		return intersects(ray, planesList, normalIsOutside);
    }
    //-----------------------------------------------------------------------
    std::pair<bool, double> MathD::intersects(const RayD& ray, 
        const list<PlaneD>& planes, bool normalIsOutside)
    {
		list<PlaneD>::const_iterator planeit, planeitend;
        planeitend = planes.end();
        bool allInside = true;
        std::pair<bool, double> ret;
		std::pair<bool, double> end;
        ret.first = false;
        ret.second = 0.0f;
		end.first = false;
		end.second = 0;


        // derive side
        // NB we don't pass directly since that would require PlaneD::Side in 
        // interface, which results in recursive includes since MathD is so fundamental
        PlaneD::Side outside = normalIsOutside ? PlaneD::POSITIVE_SIDE : PlaneD::NEGATIVE_SIDE;

        for (planeit = planes.begin(); planeit != planeitend; ++planeit)
        {
            const PlaneD& plane = *planeit;
            // is origin outside?
            if (plane.getSide(ray.getOrigin()) == outside)
            {
                allInside = false;
                // Test single plane
                std::pair<bool, double> planeRes = 
                    ray.intersects(plane);
                if (planeRes.first)
                {
                    // Ok, we intersected
                    ret.first = true;
                    // Use the most distant result since convex volume
                    ret.second = std::max(ret.second, planeRes.second);
                }
				else
        {
					ret.first =false;
            ret.second = 0.0f;
					return ret;
        }
    }
			else
    {
                std::pair<bool, double> planeRes = 
                    ray.intersects(plane);
                if (planeRes.first)
                {
					if( !end.first )
					{
						end.first = true;
						end.second = planeRes.second;
					}
					else
					{
						end.second = std::min( planeRes.second, end.second );
					}

                }

            }
        }

        if (allInside)
        {
            // Intersecting at 0 distance since inside the volume!
            ret.first = true;
            ret.second = 0.0f;
			return ret;
        }

		if( end.first )
		{
			if( end.second < ret.second )
			{
				ret.first = false;
				return ret;
			}
		}
        return ret;
    }
    //-----------------------------------------------------------------------
    std::pair<bool, double> MathD::intersects(const RayD& ray, const SphereD& sphere, 
        bool discardInside)
    {
        const Vector3D& raydir = ray.getDirection();
        // Adjust ray origin relative to sphere center
        const Vector3D& rayorig = ray.getOrigin() - sphere.getCenter();
        double radius = sphere.getRadius();

        // Check origin inside first
        if (rayorig.squaredLength() <= radius*radius && discardInside)
        {
            return std::pair<bool, double>(true, 0);
        }

        // Mmm, quadratics
        // Build coeffs which can be used with std quadratic solver
        // ie t = (-b +/- sqrt(b*b + 4ac)) / 2a
        double a = raydir.dotProduct(raydir);
        double b = 2 * rayorig.dotProduct(raydir);
        double c = rayorig.dotProduct(rayorig) - radius*radius;

        // Calc determinant
        double d = (b*b) - (4 * a * c);
        if (d < 0)
        {
            // No intersection
            return std::pair<bool, double>(false, 0);
        }
        else
        {
            // BTW, if d=0 there is one intersection, if d > 0 there are 2
            // But we only want the closest one, so that's ok, just use the 
            // '-' version of the solver
            double t = ( -b - MathD::Sqrt(d) ) / (2 * a);
            if (t < 0)
                t = ( -b + MathD::Sqrt(d) ) / (2 * a);
            return std::pair<bool, double>(true, t);
        }


    }
    //-----------------------------------------------------------------------
	std::pair<bool, double> MathD::intersects(const RayD& ray, const AxisAlignedBoxD& box)
	{
		if (box.isNull()) return std::pair<bool, double>(false, 0);
		if (box.isInfinite()) return std::pair<bool, double>(true, 0);

		double lowt = 0.0f;
		double t;
		bool hit = false;
		Vector3D hitpoint;
		const Vector3D& min = box.getMinimum();
		const Vector3D& max = box.getMaximum();
		const Vector3D& rayorig = ray.getOrigin();
		const Vector3D& raydir = ray.getDirection();

		// Check origin inside first
		if ( rayorig > min && rayorig < max )
		{
			return std::pair<bool, double>(true, 0);
		}

		// Check each face in turn, only check closest 3
		// Min x
		if (rayorig.x <= min.x && raydir.x > 0)
		{
			t = (min.x - rayorig.x) / raydir.x;
			if (t >= 0)
			{
				// Substitute t back into ray and check bounds and dist
				hitpoint = rayorig + raydir * t;
				if (hitpoint.y >= min.y && hitpoint.y <= max.y &&
					hitpoint.z >= min.z && hitpoint.z <= max.z &&
					(!hit || t < lowt))
				{
					hit = true;
					lowt = t;
				}
			}
		}
		// Max x
		if (rayorig.x >= max.x && raydir.x < 0)
		{
			t = (max.x - rayorig.x) / raydir.x;
			if (t >= 0)
			{
				// Substitute t back into ray and check bounds and dist
				hitpoint = rayorig + raydir * t;
				if (hitpoint.y >= min.y && hitpoint.y <= max.y &&
					hitpoint.z >= min.z && hitpoint.z <= max.z &&
					(!hit || t < lowt))
				{
					hit = true;
					lowt = t;
				}
			}
		}
		// Min y
		if (rayorig.y <= min.y && raydir.y > 0)
		{
			t = (min.y - rayorig.y) / raydir.y;
			if (t >= 0)
			{
				// Substitute t back into ray and check bounds and dist
				hitpoint = rayorig + raydir * t;
				if (hitpoint.x >= min.x && hitpoint.x <= max.x &&
					hitpoint.z >= min.z && hitpoint.z <= max.z &&
					(!hit || t < lowt))
				{
					hit = true;
					lowt = t;
				}
			}
		}
		// Max y
		if (rayorig.y >= max.y && raydir.y < 0)
		{
			t = (max.y - rayorig.y) / raydir.y;
			if (t >= 0)
			{
				// Substitute t back into ray and check bounds and dist
				hitpoint = rayorig + raydir * t;
				if (hitpoint.x >= min.x && hitpoint.x <= max.x &&
					hitpoint.z >= min.z && hitpoint.z <= max.z &&
					(!hit || t < lowt))
				{
					hit = true;
					lowt = t;
				}
			}
		}
		// Min z
		if (rayorig.z <= min.z && raydir.z > 0)
		{
			t = (min.z - rayorig.z) / raydir.z;
			if (t >= 0)
			{
				// Substitute t back into ray and check bounds and dist
				hitpoint = rayorig + raydir * t;
				if (hitpoint.x >= min.x && hitpoint.x <= max.x &&
					hitpoint.y >= min.y && hitpoint.y <= max.y &&
					(!hit || t < lowt))
				{
					hit = true;
					lowt = t;
				}
			}
		}
		// Max z
		if (rayorig.z >= max.z && raydir.z < 0)
		{
			t = (max.z - rayorig.z) / raydir.z;
			if (t >= 0)
			{
				// Substitute t back into ray and check bounds and dist
				hitpoint = rayorig + raydir * t;
				if (hitpoint.x >= min.x && hitpoint.x <= max.x &&
					hitpoint.y >= min.y && hitpoint.y <= max.y &&
					(!hit || t < lowt))
				{
					hit = true;
					lowt = t;
				}
			}
		}

		return std::pair<bool, double>(hit, lowt);

	} 
    //-----------------------------------------------------------------------
    bool MathD::intersects(const RayD& ray, const AxisAlignedBoxD& box,
        double* d1, double* d2)
    {
        if (box.isNull())
            return false;

        if (box.isInfinite())
        {
            if (d1) *d1 = 0;
            if (d2) *d2 = MathD::POS_INFINITY;
            return true;
        }

        const Vector3D& min = box.getMinimum();
        const Vector3D& max = box.getMaximum();
        const Vector3D& rayorig = ray.getOrigin();
        const Vector3D& raydir = ray.getDirection();

        Vector3D absDir;
        absDir[0] = MathD::Abs(raydir[0]);
        absDir[1] = MathD::Abs(raydir[1]);
        absDir[2] = MathD::Abs(raydir[2]);

        // Sort the axis, ensure check minimise floating error axis first
        int imax = 0, imid = 1, imin = 2;
        if (absDir[0] < absDir[2])
        {
            imax = 2;
            imin = 0;
        }
        if (absDir[1] < absDir[imin])
        {
            imid = imin;
            imin = 1;
        }
        else if (absDir[1] > absDir[imax])
        {
            imid = imax;
            imax = 1;
        }

        double start = 0, end = MathD::POS_INFINITY;

#define _CALC_AXIS(i)                                       \
    do {                                                    \
        double denom = 1 / raydir[i];                         \
        double newstart = (min[i] - rayorig[i]) * denom;      \
        double newend = (max[i] - rayorig[i]) * denom;        \
        if (newstart > newend) std::swap(newstart, newend); \
        if (newstart > end || newend < start) return false; \
        if (newstart > start) start = newstart;             \
        if (newend < end) end = newend;                     \
    } while(0)

        // Check each axis in turn

        _CALC_AXIS(imax);

        if (absDir[imid] < std::numeric_limits<double>::epsilon())
        {
            // Parallel with middle and minimise axis, check bounds only
            if (rayorig[imid] < min[imid] || rayorig[imid] > max[imid] ||
                rayorig[imin] < min[imin] || rayorig[imin] > max[imin])
                return false;
        }
        else
        {
            _CALC_AXIS(imid);

            if (absDir[imin] < std::numeric_limits<double>::epsilon())
            {
                // Parallel with minimise axis, check bounds only
                if (rayorig[imin] < min[imin] || rayorig[imin] > max[imin])
                    return false;
            }
            else
            {
                _CALC_AXIS(imin);
            }
        }
#undef _CALC_AXIS

        if (d1) *d1 = start;
        if (d2) *d2 = end;

        return true;
    }
    //-----------------------------------------------------------------------
    std::pair<bool, double> MathD::intersects(const RayD& ray, const Vector3D& a,
        const Vector3D& b, const Vector3D& c, const Vector3D& normal,
        bool positiveSide, bool negativeSide)
    {
        //
        // Calculate intersection with plane.
        //
        double t;
        {
            double denom = normal.dotProduct(ray.getDirection());

            // Check intersect side
            if (denom > + std::numeric_limits<double>::epsilon())
            {
                if (!negativeSide)
                    return std::pair<bool, double>(false, 0);
            }
            else if (denom < - std::numeric_limits<double>::epsilon())
            {
                if (!positiveSide)
                    return std::pair<bool, double>(false, 0);
            }
            else
            {
                // Parallel or triangle area is close to zero when
                // the plane normal not normalised.
                return std::pair<bool, double>(false, 0);
            }

            t = normal.dotProduct(a - ray.getOrigin()) / denom;

            if (t < 0)
            {
                // Intersection is behind origin
                return std::pair<bool, double>(false, 0);
            }
        }

        //
        // Calculate the largest area projection plane in X, Y or Z.
        //
        size_t i0, i1;
        {
            double n0 = MathD::Abs(normal[0]);
            double n1 = MathD::Abs(normal[1]);
            double n2 = MathD::Abs(normal[2]);

            i0 = 1; i1 = 2;
            if (n1 > n2)
            {
                if (n1 > n0) i0 = 0;
            }
            else
            {
                if (n2 > n0) i1 = 0;
            }
        }

        //
        // Check the intersection point is inside the triangle.
        //
        {
            double u1 = b[i0] - a[i0];
            double v1 = b[i1] - a[i1];
            double u2 = c[i0] - a[i0];
            double v2 = c[i1] - a[i1];
            double u0 = t * ray.getDirection()[i0] + ray.getOrigin()[i0] - a[i0];
            double v0 = t * ray.getDirection()[i1] + ray.getOrigin()[i1] - a[i1];

            double alpha = u0 * v2 - u2 * v0;
            double beta  = u1 * v0 - u0 * v1;
            double area  = u1 * v2 - u2 * v1;

            // epsilon to avoid float precision error
            const double EPSILON = 1e-6f;

            double tolerance = - EPSILON * area;

            if (area > 0)
            {
                if (alpha < tolerance || beta < tolerance || alpha+beta > area-tolerance)
                    return std::pair<bool, double>(false, 0);
            }
            else
            {
                if (alpha > tolerance || beta > tolerance || alpha+beta < area-tolerance)
                    return std::pair<bool, double>(false, 0);
            }
        }

        return std::pair<bool, double>(true, t);
    }
    //-----------------------------------------------------------------------
    std::pair<bool, double> MathD::intersects(const RayD& ray, const Vector3D& a,
        const Vector3D& b, const Vector3D& c,
        bool positiveSide, bool negativeSide)
    {
        Vector3D normal = calculateBasicFaceNormalWithoutNormalize(a, b, c);
        return intersects(ray, a, b, c, normal, positiveSide, negativeSide);
    }
    //-----------------------------------------------------------------------
    bool MathD::intersects(const SphereD& sphere, const AxisAlignedBoxD& box)
    {
        if (box.isNull()) return false;
        if (box.isInfinite()) return true;

        // Use splitting planes
        const Vector3D& center = sphere.getCenter();
        double radius = sphere.getRadius();
        const Vector3D& min = box.getMinimum();
        const Vector3D& max = box.getMaximum();

		// Arvo's algorithm
		double s, d = 0;
		for (int i = 0; i < 3; ++i)
		{
			if (center.ptr()[i] < min.ptr()[i])
			{
				s = center.ptr()[i] - min.ptr()[i];
				d += s * s; 
			}
			else if(center.ptr()[i] > max.ptr()[i])
			{
				s = center.ptr()[i] - max.ptr()[i];
				d += s * s; 
			}
		}
		return d <= radius * radius;

    }
    //-----------------------------------------------------------------------
    bool MathD::intersects(const PlaneD& plane, const AxisAlignedBoxD& box)
    {
        return (plane.getSide(box) == PlaneD::BOTH_SIDE);
    }
    //-----------------------------------------------------------------------
    bool MathD::intersects(const SphereD& sphere, const PlaneD& plane)
    {
        return (
            MathD::Abs(plane.getDistance(sphere.getCenter()))
            <= sphere.getRadius() );
    }
    //-----------------------------------------------------------------------
    Vector3D MathD::calculateTangentSpaceVector(
        const Vector3D& position1, const Vector3D& position2, const Vector3D& position3,
        double u1, double v1, double u2, double v2, double u3, double v3)
    {
	    //side0 is the vector along one side of the triangle of vertices passed in, 
	    //and side1 is the vector along another side. Taking the cross product of these returns the normal.
	    Vector3D side0 = position1 - position2;
	    Vector3D side1 = position3 - position1;
	    //Calculate face normal
	    Vector3D normal = side1.crossProduct(side0);
	    normal.normalise();
	    //Now we use a formula to calculate the tangent. 
	    double deltaV0 = v1 - v2;
	    double deltaV1 = v3 - v1;
	    Vector3D tangent = deltaV1 * side0 - deltaV0 * side1;
	    tangent.normalise();
	    //Calculate binormal
	    double deltaU0 = u1 - u2;
	    double deltaU1 = u3 - u1;
	    Vector3D binormal = deltaU1 * side0 - deltaU0 * side1;
	    binormal.normalise();
	    //Now, we take the cross product of the tangents to get a vector which 
	    //should point in the same direction as our normal calculated above. 
	    //If it points in the opposite direction (the dot product between the normals is less than zero), 
	    //then we need to reverse the s and t tangents. 
	    //This is because the triangle has been mirrored when going from tangent space to object space.
	    //reverse tangents if necessary
	    Vector3D tangentCross = tangent.crossProduct(binormal);
	    if (tangentCross.dotProduct(normal) < 0.0f)
	    {
		    tangent = -tangent;
		    binormal = -binormal;
	    }

        return tangent;

    }
    //-----------------------------------------------------------------------
    Matrix4 MathD::buildReflectionMatrix(const PlaneD& p)
    {
        return Matrix4(
            -2 * p.normal.x * p.normal.x + 1,   -2 * p.normal.x * p.normal.y,       -2 * p.normal.x * p.normal.z,       -2 * p.normal.x * p.d, 
            -2 * p.normal.y * p.normal.x,       -2 * p.normal.y * p.normal.y + 1,   -2 * p.normal.y * p.normal.z,       -2 * p.normal.y * p.d, 
            -2 * p.normal.z * p.normal.x,       -2 * p.normal.z * p.normal.y,       -2 * p.normal.z * p.normal.z + 1,   -2 * p.normal.z * p.d, 
            0,                                  0,                                  0,                                  1);
    }
    //-----------------------------------------------------------------------
    Vector4 MathD::calculateFaceNormal(const Vector3D& v1, const Vector3D& v2, const Vector3D& v3)
    {
        Vector3D normal = calculateBasicFaceNormal(v1, v2, v3);
        // Now set up the w (distance of tri from origin
        return Vector4(normal.x, normal.y, normal.z, -(normal.dotProduct(v1)));
    }
    //-----------------------------------------------------------------------
    Vector3D MathD::calculateBasicFaceNormal(const Vector3D& v1, const Vector3D& v2, const Vector3D& v3)
    {
        Vector3D normal = (v2 - v1).crossProduct(v3 - v1);
        normal.normalise();
        return normal;
    }
    //-----------------------------------------------------------------------
    Vector4 MathD::calculateFaceNormalWithoutNormalize(const Vector3D& v1, const Vector3D& v2, const Vector3D& v3)
    {
        Vector3D normal = calculateBasicFaceNormalWithoutNormalize(v1, v2, v3);
        // Now set up the w (distance of tri from origin)
        return Vector4(normal.x, normal.y, normal.z, -(normal.dotProduct(v1)));
    }
    //-----------------------------------------------------------------------
    Vector3D MathD::calculateBasicFaceNormalWithoutNormalize(const Vector3D& v1, const Vector3D& v2, const Vector3D& v3)
    {
        Vector3D normal = (v2 - v1).crossProduct(v3 - v1);
        return normal;
    }
	//-----------------------------------------------------------------------
	double MathD::gaussianDistribution(double x, double offset, double scale)
	{
		double nom = MathD::Exp(
			-MathD::Sqr(x - offset) / (2 * MathD::Sqr(scale)));
		double denom = scale * MathD::Sqrt(2 * MathD::PI);

		return nom / denom;

	}
	////---------------------------------------------------------------------
	//Matrix4 MathD::makeViewMatrix(const Vector3D& position, const Quaternion& orientation, 
	//	const Matrix4* reflectMatrix)
	//{
	//	Matrix4 viewMatrix;

	//	// View matrix is:
	//	//
	//	//  [ Lx  Uy  Dz  Tx  ]
	//	//  [ Lx  Uy  Dz  Ty  ]
	//	//  [ Lx  Uy  Dz  Tz  ]
	//	//  [ 0   0   0   1   ]
	//	//
	//	// Where T = -(Transposed(Rot) * Pos)

	//	// This is most efficiently done using 3x3 Matrices
	//	Matrix3D rot;
	//	orientation.ToRotationMatrix(rot);

	//	// Make the translation relative to new axes
	//	Matrix3D rotT = rot.Transpose();
	//	Vector3D trans = -rotT * position;

	//	// Make final matrix
	//	viewMatrix = Matrix4::IDENTITY;
	//	viewMatrix = rotT; // fills upper 3x3
	//	viewMatrix[0][3] = trans.x;
	//	viewMatrix[1][3] = trans.y;
	//	viewMatrix[2][3] = trans.z;

	//	// Deal with reflections
	//	if (reflectMatrix)
	//	{
	//		viewMatrix = viewMatrix * (*reflectMatrix);
	//	}

	//	return viewMatrix;

	//}
	//---------------------------------------------------------------------
	double MathD::boundingRadiusFromAABB(const AxisAlignedBoxD& aabb)
	{
		Vector3D max = aabb.getMaximum();
		Vector3D min = aabb.getMinimum();

		Vector3D magnitude = max;
		magnitude.makeCeil(-max);
		magnitude.makeCeil(min);
		magnitude.makeCeil(-min);

		return magnitude.length();
	}

}
