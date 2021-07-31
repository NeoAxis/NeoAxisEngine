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
#ifndef __MathD_H__
#define __MathD_H__

#include "OgrePrerequisites.h"
//#include "OgreVector3D.h"
//#include "OgreAxisAlignedBoxD.h"

namespace Ogre
{
	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup MathD
	*  @{
	*/
    /** Wrapper class which indicates a given angle value is in RadianDs.
    @remarks
        RadianD values are interchangeable with DegreeD values, and conversions
        will be done automatically between them.
    */
	class RadianD
	{
		double mRad;

	public:
		explicit RadianD ( double r=0 ) : mRad(r) {}
		RadianD ( const DegreeD& d );
		RadianD& operator = ( const double& f ) { mRad = f; return *this; }
		RadianD& operator = ( const RadianD& r ) { mRad = r.mRad; return *this; }
		RadianD& operator = ( const DegreeD& d );

		double valueDegreeDs() const; // see bottom of this file
		double valueRadianDs() const { return mRad; }
		double valueAngleDUnits() const;

        const RadianD& operator + () const { return *this; }
		RadianD operator + ( const RadianD& r ) const { return RadianD ( mRad + r.mRad ); }
		RadianD operator + ( const DegreeD& d ) const;
		RadianD& operator += ( const RadianD& r ) { mRad += r.mRad; return *this; }
		RadianD& operator += ( const DegreeD& d );
		RadianD operator - () const { return RadianD(-mRad); }
		RadianD operator - ( const RadianD& r ) const { return RadianD ( mRad - r.mRad ); }
		RadianD operator - ( const DegreeD& d ) const;
		RadianD& operator -= ( const RadianD& r ) { mRad -= r.mRad; return *this; }
		RadianD& operator -= ( const DegreeD& d );
		RadianD operator * ( double f ) const { return RadianD ( mRad * f ); }
        RadianD operator * ( const RadianD& f ) const { return RadianD ( mRad * f.mRad ); }
		RadianD& operator *= ( double f ) { mRad *= f; return *this; }
		RadianD operator / ( double f ) const { return RadianD ( mRad / f ); }
		RadianD& operator /= ( double f ) { mRad /= f; return *this; }

		bool operator <  ( const RadianD& r ) const { return mRad <  r.mRad; }
		bool operator <= ( const RadianD& r ) const { return mRad <= r.mRad; }
		bool operator == ( const RadianD& r ) const { return mRad == r.mRad; }
		bool operator != ( const RadianD& r ) const { return mRad != r.mRad; }
		bool operator >= ( const RadianD& r ) const { return mRad >= r.mRad; }
		bool operator >  ( const RadianD& r ) const { return mRad >  r.mRad; }

		FORCEINLINE _OgreExport friend std::ostream& operator <<
			( std::ostream& o, const RadianD& v )
		{
			o << "RadianD(" << v.valueRadianDs() << ")";
			return o;
		}
	};

    /** Wrapper class which indicates a given angle value is in DegreeDs.
    @remarks
        DegreeD values are interchangeable with RadianD values, and conversions
        will be done automatically between them.
    */
	class DegreeD
	{
		double mDeg; // if you get an error here - make sure to define/typedef 'double' first

	public:
		explicit DegreeD ( double d=0 ) : mDeg(d) {}
		DegreeD ( const RadianD& r ) : mDeg(r.valueDegreeDs()) {}
		DegreeD& operator = ( const double& f ) { mDeg = f; return *this; }
		DegreeD& operator = ( const DegreeD& d ) { mDeg = d.mDeg; return *this; }
		DegreeD& operator = ( const RadianD& r ) { mDeg = r.valueDegreeDs(); return *this; }

		double valueDegreeDs() const { return mDeg; }
		double valueRadianDs() const; // see bottom of this file
		double valueAngleDUnits() const;

		const DegreeD& operator + () const { return *this; }
		DegreeD operator + ( const DegreeD& d ) const { return DegreeD ( mDeg + d.mDeg ); }
		DegreeD operator + ( const RadianD& r ) const { return DegreeD ( mDeg + r.valueDegreeDs() ); }
		DegreeD& operator += ( const DegreeD& d ) { mDeg += d.mDeg; return *this; }
		DegreeD& operator += ( const RadianD& r ) { mDeg += r.valueDegreeDs(); return *this; }
		DegreeD operator - () const { return DegreeD(-mDeg); }
		DegreeD operator - ( const DegreeD& d ) const { return DegreeD ( mDeg - d.mDeg ); }
		DegreeD operator - ( const RadianD& r ) const { return DegreeD ( mDeg - r.valueDegreeDs() ); }
		DegreeD& operator -= ( const DegreeD& d ) { mDeg -= d.mDeg; return *this; }
		DegreeD& operator -= ( const RadianD& r ) { mDeg -= r.valueDegreeDs(); return *this; }
		DegreeD operator * ( double f ) const { return DegreeD ( mDeg * f ); }
        DegreeD operator * ( const DegreeD& f ) const { return DegreeD ( mDeg * f.mDeg ); }
		DegreeD& operator *= ( double f ) { mDeg *= f; return *this; }
		DegreeD operator / ( double f ) const { return DegreeD ( mDeg / f ); }
		DegreeD& operator /= ( double f ) { mDeg /= f; return *this; }

		bool operator <  ( const DegreeD& d ) const { return mDeg <  d.mDeg; }
		bool operator <= ( const DegreeD& d ) const { return mDeg <= d.mDeg; }
		bool operator == ( const DegreeD& d ) const { return mDeg == d.mDeg; }
		bool operator != ( const DegreeD& d ) const { return mDeg != d.mDeg; }
		bool operator >= ( const DegreeD& d ) const { return mDeg >= d.mDeg; }
		bool operator >  ( const DegreeD& d ) const { return mDeg >  d.mDeg; }

		FORCEINLINE _OgreExport friend std::ostream& operator <<
			( std::ostream& o, const DegreeD& v )
		{
			o << "DegreeD(" << v.valueDegreeDs() << ")";
			return o;
		}
	};

    /** Wrapper class which identifies a value as the currently default angle 
        type, as defined by MathD::setAngleDUnit.
    @remarks
        AngleD values will be automatically converted between radians and degrees,
        as appropriate.
    */
	class AngleD
	{
		double mAngleD;
	public:
		explicit AngleD ( double angle ) : mAngleD(angle) {}
		operator RadianD() const;
		operator DegreeD() const;
	};

	//// these functions could not be defined within the class definition of class
	//// RadianD because they required class DegreeD to be defined
	//FORCEINLINE RadianD::RadianD ( const DegreeD& d ) : mRad(d.valueRadianDs()) {
	//}
	//FORCEINLINE RadianD& RadianD::operator = ( const DegreeD& d ) {
	//	mRad = d.valueRadianDs(); return *this;
	//}
	//FORCEINLINE RadianD RadianD::operator + ( const DegreeD& d ) const {
	//	return RadianD ( mRad + d.valueRadianDs() );
	//}
	//FORCEINLINE RadianD& RadianD::operator += ( const DegreeD& d ) {
	//	mRad += d.valueRadianDs();
	//	return *this;
	//}
	//FORCEINLINE RadianD RadianD::operator - ( const DegreeD& d ) const {
	//	return RadianD ( mRad - d.valueRadianDs() );
	//}
	//FORCEINLINE RadianD& RadianD::operator -= ( const DegreeD& d ) {
	//	mRad -= d.valueRadianDs();
	//	return *this;
	//}

    /** Class to provide access to common mathematical functions.
        @remarks
            Most of the maths functions are aliased versions of the C runtime
            library functions. They are aliased here to provide future
            optimisation opportunities, either from faster RTLs or custom
            math approximations.
        @note
            <br>This is based on MgcMathD.h from
            <a href="http://www.geometrictools.com/">Wild Magic</a>.
    */
    class _OgreExport MathD 
    {
   public:
       /** The angular units used by the API. This functionality is now deprecated in favor
	       of discreet angular unit types ( see DegreeD and RadianD above ). The only place
		   this functionality is actually still used is when parsing files. Search for
		   usage of the AngleD class for those instances
       */
       enum AngleDUnit
       {
           AU_DEGREE,
           AU_RADIAN
       };

    protected:
       // angle units used by the api
       static AngleDUnit msAngleDUnit;

        /// Size of the trig tables as determined by constructor.
        //static int mTrigTableSize;

        /// RadianD -> index factor value ( mTrigTableSize / 2 * PI )
        //static double mTrigTableFactor;
        //static double* mSinTable;
        //static double* mTanTable;

        ///** Private function to build trig tables.
        //*/
        //void buildTrigTables();

		//static double SinTable (double fValue);
		//static double TanTable (double fValue);
    public:
        ///** Default constructor.
        //    @param
        //        trigTableSize Optional parameter to set the size of the
        //        tables used to implement Sin, Cos, Tan
        //*/
        //MathD(unsigned int trigTableSize = 4096);

        ///** Default destructor.
        //*/
        //~MathD();

		static FORCEINLINE int IAbs (int iValue) { return ( iValue >= 0 ? iValue : -iValue ); }
		static FORCEINLINE int ICeil (float fValue) { return int(ceil(fValue)); }
		static FORCEINLINE int IFloor (float fValue) { return int(floor(fValue)); }
        static int ISign (int iValue);

		static FORCEINLINE double Abs (double fValue) { return double(fabs(fValue)); }
		static FORCEINLINE DegreeD Abs (const DegreeD& dValue) { return DegreeD(fabs(dValue.valueDegreeDs())); }
		static FORCEINLINE RadianD Abs (const RadianD& rValue) { return RadianD(fabs(rValue.valueRadianDs())); }
		static RadianD ACos (double fValue);
		static RadianD ASin (double fValue);
		static FORCEINLINE RadianD ATan (double fValue) { return RadianD(atan(fValue)); }
		static FORCEINLINE RadianD ATan2 (double fY, double fX) { return RadianD(atan2(fY,fX)); }
		static FORCEINLINE double Ceil (double fValue) { return double(ceil(fValue)); }
		static FORCEINLINE bool isNaN(double f)
		{
			// std::isnan() is C99, not supported by all compilers
			// However NaN always fails this next test, no other number does.
			return f != f;
		}

        /** Cosine function.
            @param
                fValue AngleD in radians
            @param
                useTables If true, uses lookup tables rather than
                calculation - faster but less accurate.
        */
        static FORCEINLINE double Cos (const RadianD& fValue) { return double(cos(fValue.valueRadianDs())); }
   //     static FORCEINLINE double Cos (const RadianD& fValue, bool useTables = false) {
			//return (!useTables) ? double(cos(fValue.valueRadianDs())) : SinTable(fValue.valueRadianDs() + HALF_PI);
		//}
        /** Cosine function.
            @param
                fValue AngleD in radians
            @param
                useTables If true, uses lookup tables rather than
                calculation - faster but less accurate.
        */
		static FORCEINLINE double Cos (double fValue) { return double(cos(fValue)); }
		//static FORCEINLINE double Cos (double fValue, bool useTables = false) {
			//return (!useTables) ? double(cos(fValue)) : SinTable(fValue + HALF_PI);
		//}

		static FORCEINLINE double Exp (double fValue) { return double(exp(fValue)); }

		static FORCEINLINE double Floor (double fValue) { return double(floor(fValue)); }

		static FORCEINLINE double Log (double fValue) { return double(log(fValue)); }

		/// Stored value of log(2) for frequent use
		static const double LOG2;

		static FORCEINLINE double Log2 (double fValue) { return double(log(fValue)/LOG2); }

		static FORCEINLINE double LogN (double base, double fValue) { return double(log(fValue)/log(base)); }

		static FORCEINLINE double Pow (double fBase, double fExponent) { return double(pow(fBase,fExponent)); }

        static double Sign (double fValue);
		static FORCEINLINE RadianD Sign ( const RadianD& rValue )
		{
			return RadianD(Sign(rValue.valueRadianDs()));
		}
		static FORCEINLINE DegreeD Sign ( const DegreeD& dValue )
		{
			return DegreeD(Sign(dValue.valueDegreeDs()));
		}

        /** Sine function.
            @param
                fValue AngleD in radians
            @param
                useTables If true, uses lookup tables rather than
                calculation - faster but less accurate.
        */
        static FORCEINLINE double Sin (const RadianD& fValue) { return double(sin(fValue.valueRadianDs())); }
  //      static FORCEINLINE double Sin (const RadianD& fValue, bool useTables = false) {
		//	return (!useTables) ? double(sin(fValue.valueRadianDs())) : SinTable(fValue.valueRadianDs());
		//}
        /** Sine function.
            @param
                fValue AngleD in radians
            @param
                useTables If true, uses lookup tables rather than
                calculation - faster but less accurate.
        */
		  static FORCEINLINE double Sin (double fValue) { return double(sin(fValue)); }
   //     static FORCEINLINE double Sin (double fValue, bool useTables = false) {
			//return (!useTables) ? double(sin(fValue)) : SinTable(fValue);
		//}

		static FORCEINLINE double Sqr (double fValue) { return fValue*fValue; }

		static FORCEINLINE double Sqrt (double fValue) { return double(sqrt(fValue)); }

        static FORCEINLINE RadianD Sqrt (const RadianD& fValue) { return RadianD(sqrt(fValue.valueRadianDs())); }

        static FORCEINLINE DegreeD Sqrt (const DegreeD& fValue) { return DegreeD(sqrt(fValue.valueDegreeDs())); }

        /** Inverse square root i.e. 1 / Sqrt(x), good for vector
            normalisation.
        */
		static double InvSqrt(double fValue);

        static double UnitRandom ();  // in [0,1]

        static double RangeRandom (double fLow, double fHigh);  // in [fLow,fHigh]

        static double SymmetricRandom ();  // in [-1,1]

        /** Tangent function.
            @param
                fValue AngleD in radians
            @param
                useTables If true, uses lookup tables rather than
                calculation - faster but less accurate.
        */
		static FORCEINLINE double Tan (const RadianD& fValue) { return double(tan(fValue.valueRadianDs())); }
		//static FORCEINLINE double Tan (const RadianD& fValue, bool useTables = false) {
		//	return (!useTables) ? double(tan(fValue.valueRadianDs())) : TanTable(fValue.valueRadianDs());
		//}
        /** Tangent function.
            @param
                fValue AngleD in radians
            @param
                useTables If true, uses lookup tables rather than
                calculation - faster but less accurate.
        */
		//static FORCEINLINE double Tan (double fValue, bool useTables = false) {
		//	return (!useTables) ? double(tan(fValue)) : TanTable(fValue);
		//}

		static FORCEINLINE double DegreeDsToRadianDs(double degrees) { return degrees * fDeg2Rad; }
        static FORCEINLINE double RadianDsToDegreeDs(double radians) { return radians * fRad2Deg; }

       ///** These functions used to set the assumed angle units (radians or degrees) 
       //     expected when using the AngleD type.
       //@par
       //     You can set this directly after creating a new Root, and also before/after resource creation,
       //     depending on whether you want the change to affect resource files.
       //*/
       //static void setAngleDUnit(AngleDUnit unit);
       ///** Get the unit being used for angles. */
       //static AngleDUnit getAngleDUnit(void);

       /** Convert from the current AngleDUnit to radians. */
       static double AngleDUnitsToRadianDs(double units);
       /** Convert from radians to the current AngleDUnit . */
       static double RadianDsToAngleDUnits(double radians);
       /** Convert from the current AngleDUnit to degrees. */
       static double AngleDUnitsToDegreeDs(double units);
       /** Convert from degrees to the current AngleDUnit. */
       static double DegreeDsToAngleDUnits(double degrees);

       /** Checks whether a given point is inside a triangle, in a
            2-dimensional (Cartesian) space.
            @remarks
                The vertices of the triangle must be given in either
                trigonometrical (anticlockwise) or inverse trigonometrical
                (clockwise) order.
            @param
                p The point.
            @param
                a The triangle's first vertex.
            @param
                b The triangle's second vertex.
            @param
                c The triangle's third vertex.
            @returns
                If the point resides in the triangle, <b>true</b> is
                returned.
            @par
                If the point is outside the triangle, <b>false</b> is
                returned.
        */
        static bool pointInTri2D(const Vector2& p, const Vector2& a, 
			const Vector2& b, const Vector2& c);

       /** Checks whether a given 3D point is inside a triangle.
       @remarks
            The vertices of the triangle must be given in either
            trigonometrical (anticlockwise) or inverse trigonometrical
            (clockwise) order, and the point must be guaranteed to be in the
			same plane as the triangle
        @param
            p The point.
        @param
            a The triangle's first vertex.
        @param
            b The triangle's second vertex.
        @param
            c The triangle's third vertex.
		@param 
			normal The triangle plane's normal (passed in rather than calculated
				on demand since the callermay already have it)
        @returns
            If the point resides in the triangle, <b>true</b> is
            returned.
        @par
            If the point is outside the triangle, <b>false</b> is
            returned.
        */
        static bool pointInTri3D(const Vector3D& p, const Vector3D& a, 
			const Vector3D& b, const Vector3D& c, const Vector3D& normal);
        /** RayD / plane intersection, returns boolean result and distance. */
        static std::pair<bool, double> intersects(const RayD& ray, const PlaneD& plane);

        /** RayD / sphere intersection, returns boolean result and distance. */
        static std::pair<bool, double> intersects(const RayD& ray, const SphereD& sphere, 
            bool discardInside = true);
        
        /** RayD / box intersection, returns boolean result and distance. */
        static std::pair<bool, double> intersects(const RayD& ray, const AxisAlignedBoxD& box);

        /** RayD / box intersection, returns boolean result and two intersection distance.
        @param
            ray The ray.
        @param
            box The box.
        @param
            d1 A real pointer to retrieve the near intersection distance
                from the ray origin, maybe <b>null</b> which means don't care
                about the near intersection distance.
        @param
            d2 A real pointer to retrieve the far intersection distance
                from the ray origin, maybe <b>null</b> which means don't care
                about the far intersection distance.
        @returns
            If the ray is intersects the box, <b>true</b> is returned, and
            the near intersection distance is return by <i>d1</i>, the
            far intersection distance is return by <i>d2</i>. Guarantee
            <b>0</b> <= <i>d1</i> <= <i>d2</i>.
        @par
            If the ray isn't intersects the box, <b>false</b> is returned, and
            <i>d1</i> and <i>d2</i> is unmodified.
        */
        static bool intersects(const RayD& ray, const AxisAlignedBoxD& box,
            double* d1, double* d2);

        /** RayD / triangle intersection, returns boolean result and distance.
        @param
            ray The ray.
        @param
            a The triangle's first vertex.
        @param
            b The triangle's second vertex.
        @param
            c The triangle's third vertex.
		@param 
			normal The triangle plane's normal (passed in rather than calculated
				on demand since the callermay already have it), doesn't need
                normalised since we don't care.
        @param
            positiveSide Intersect with "positive side" of the triangle
        @param
            negativeSide Intersect with "negative side" of the triangle
        @returns
            If the ray is intersects the triangle, a pair of <b>true</b> and the
            distance between intersection point and ray origin returned.
        @par
            If the ray isn't intersects the triangle, a pair of <b>false</b> and
            <b>0</b> returned.
        */
        static std::pair<bool, double> intersects(const RayD& ray, const Vector3D& a,
            const Vector3D& b, const Vector3D& c, const Vector3D& normal,
            bool positiveSide = true, bool negativeSide = true);

        /** RayD / triangle intersection, returns boolean result and distance.
        @param
            ray The ray.
        @param
            a The triangle's first vertex.
        @param
            b The triangle's second vertex.
        @param
            c The triangle's third vertex.
        @param
            positiveSide Intersect with "positive side" of the triangle
        @param
            negativeSide Intersect with "negative side" of the triangle
        @returns
            If the ray is intersects the triangle, a pair of <b>true</b> and the
            distance between intersection point and ray origin returned.
        @par
            If the ray isn't intersects the triangle, a pair of <b>false</b> and
            <b>0</b> returned.
        */
        static std::pair<bool, double> intersects(const RayD& ray, const Vector3D& a,
            const Vector3D& b, const Vector3D& c,
            bool positiveSide = true, bool negativeSide = true);

        /** SphereD / box intersection test. */
        static bool intersects(const SphereD& sphere, const AxisAlignedBoxD& box);

        /** PlaneD / box intersection test. */
        static bool intersects(const PlaneD& plane, const AxisAlignedBoxD& box);

        /** RayD / convex plane list intersection test. 
        @param ray The ray to test with
        @param plaeList List of planes which form a convex volume
        @param normalIsOutside Does the normal point outside the volume
        */
        static std::pair<bool, double> intersects(
            const RayD& ray, const vector<PlaneD>& planeList, 
            bool normalIsOutside);
        /** RayD / convex plane list intersection test. 
        @param ray The ray to test with
        @param plaeList List of planes which form a convex volume
        @param normalIsOutside Does the normal point outside the volume
        */
        static std::pair<bool, double> intersects(
            const RayD& ray, const list<PlaneD>& planeList, 
            bool normalIsOutside);

        /** SphereD / plane intersection test. 
        @remarks NB just do a plane.getDistance(sphere.getCenter()) for more detail!
        */
        static bool intersects(const SphereD& sphere, const PlaneD& plane);

        /** Compare 2 reals, using tolerance for inaccuracies.
        */
        static bool doubleEqual(double a, double b,
            double tolerance = std::numeric_limits<double>::epsilon());

        /** Calculates the tangent space vector for a given set of positions / texture coords. */
        static Vector3D calculateTangentSpaceVector(
            const Vector3D& position1, const Vector3D& position2, const Vector3D& position3,
            double u1, double v1, double u2, double v2, double u3, double v3);

        /** Build a reflection matrix for the passed in plane. */
        static Matrix4 buildReflectionMatrix(const PlaneD& p);
        /** Calculate a face normal, including the w component which is the offset from the origin. */
        static Vector4 calculateFaceNormal(const Vector3D& v1, const Vector3D& v2, const Vector3D& v3);
        /** Calculate a face normal, no w-information. */
        static Vector3D calculateBasicFaceNormal(const Vector3D& v1, const Vector3D& v2, const Vector3D& v3);
        /** Calculate a face normal without normalize, including the w component which is the offset from the origin. */
        static Vector4 calculateFaceNormalWithoutNormalize(const Vector3D& v1, const Vector3D& v2, const Vector3D& v3);
        /** Calculate a face normal without normalize, no w-information. */
        static Vector3D calculateBasicFaceNormalWithoutNormalize(const Vector3D& v1, const Vector3D& v2, const Vector3D& v3);

		/** Generates a value based on the Gaussian (normal) distribution function
			with the given offset and scale parameters.
		*/
		static double gaussianDistribution(double x, double offset = 0.0f, double scale = 1.0f);

		/** Clamp a value within an inclusive range. */
		template <typename T>
		static T Clamp(T val, T minval, T maxval)
		{
			assert (minval < maxval && "Invalid clamp range");
			return std::max(std::min(val, maxval), minval);
		}

		//static Matrix4 makeViewMatrix(const Vector3D& position, const Quaternion& orientation, 
		//	const Matrix4* reflectMatrix = 0);

		/** Get a bounding radius value from a bounding box. */
		static double boundingRadiusFromAABB(const AxisAlignedBoxD& aabb);



        static const double POS_INFINITY;
        static const double NEG_INFINITY;
        static const double PI;
        static const double TWO_PI;
        static const double HALF_PI;
		static const double fDeg2Rad;
		static const double fRad2Deg;

    };

	// these functions must be defined down here, because they rely on the
	// angle unit conversion functions in class MathD:

	FORCEINLINE double RadianD::valueDegreeDs() const
	{
		return MathD::RadianDsToDegreeDs ( mRad );
	}

	FORCEINLINE double RadianD::valueAngleDUnits() const
	{
		return MathD::RadianDsToAngleDUnits ( mRad );
	}

	FORCEINLINE double DegreeD::valueRadianDs() const
	{
		return MathD::DegreeDsToRadianDs ( mDeg );
	}

	FORCEINLINE double DegreeD::valueAngleDUnits() const
	{
		return MathD::DegreeDsToAngleDUnits ( mDeg );
	}

	FORCEINLINE AngleD::operator RadianD() const
	{
		return RadianD(MathD::AngleDUnitsToRadianDs(mAngleD));
	}

	FORCEINLINE AngleD::operator DegreeD() const
	{
		return DegreeD(MathD::AngleDUnitsToDegreeDs(mAngleD));
	}

	FORCEINLINE RadianD operator * ( double a, const RadianD& b )
	{
		return RadianD ( a * b.valueRadianDs() );
	}

	FORCEINLINE RadianD operator / ( double a, const RadianD& b )
	{
		return RadianD ( a / b.valueRadianDs() );
	}

	FORCEINLINE DegreeD operator * ( double a, const DegreeD& b )
	{
		return DegreeD ( a * b.valueDegreeDs() );
	}

	FORCEINLINE DegreeD operator / ( double a, const DegreeD& b )
	{
		return DegreeD ( a / b.valueDegreeDs() );
	}
	/** @} */
	/** @} */

}
#endif
