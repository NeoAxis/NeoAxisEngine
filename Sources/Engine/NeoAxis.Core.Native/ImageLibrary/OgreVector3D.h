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
#ifndef __Vector3D_H__
#define __Vector3D_H__

#include "OgrePrerequisites.h"
#include "OgreMath.h"
#include "OgreQuaternion.h"

namespace Ogre
{

	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup Math
	*  @{
	*/
    /** Standard 3-dimensional vector.
        @remarks
            A direction in 3D space represented as distances along the 3
            orthogonal axes (x, y, z). Note that positions, directions and
            scaling factors can be represented by a vector, depending on how
            you interpret the values.
    */
    class _OgreExport Vector3D
    {
    public:
		double x, y, z;

    public:
        FORCEINLINE Vector3D()
        {
        }

        FORCEINLINE Vector3D( const double fX, const double fY, const double fZ )
            : x( fX ), y( fY ), z( fZ )
        {
        }

        FORCEINLINE explicit Vector3D( const double afCoordinate[3] )
            : x( afCoordinate[0] ),
              y( afCoordinate[1] ),
              z( afCoordinate[2] )
        {
        }

        FORCEINLINE explicit Vector3D( const int afCoordinate[3] )
        {
            x = (double)afCoordinate[0];
            y = (double)afCoordinate[1];
            z = (double)afCoordinate[2];
        }

        FORCEINLINE explicit Vector3D( double* const r )
            : x( r[0] ), y( r[1] ), z( r[2] )
        {
        }

        FORCEINLINE explicit Vector3D( const double scaler )
            : x( scaler )
            , y( scaler )
            , z( scaler )
        {
        }


		/** Exchange the contents of this vector with another. 
		*/
		FORCEINLINE void swap(Vector3D& other)
		{
			std::swap(x, other.x);
			std::swap(y, other.y);
			std::swap(z, other.z);
		}

		FORCEINLINE double operator [] ( const size_t i ) const
        {
            assert( i < 3 );

            return *(&x+i);
        }

		FORCEINLINE double& operator [] ( const size_t i )
        {
            assert( i < 3 );

            return *(&x+i);
        }
		/// Pointer accessor for direct copying
		FORCEINLINE double* ptr()
		{
			return &x;
		}
		/// Pointer accessor for direct copying
		FORCEINLINE const double* ptr() const
		{
			return &x;
		}

        /** Assigns the value of the other vector.
            @param
                rkVector The other vector
        */
        FORCEINLINE Vector3D& operator = ( const Vector3D& rkVector )
        {
            x = rkVector.x;
            y = rkVector.y;
            z = rkVector.z;

            return *this;
        }

        FORCEINLINE Vector3D& operator = ( const double fScaler )
        {
            x = fScaler;
            y = fScaler;
            z = fScaler;

            return *this;
        }

        FORCEINLINE bool operator == ( const Vector3D& rkVector ) const
        {
            return ( x == rkVector.x && y == rkVector.y && z == rkVector.z );
        }

        FORCEINLINE bool operator != ( const Vector3D& rkVector ) const
        {
            return ( x != rkVector.x || y != rkVector.y || z != rkVector.z );
        }

        // arithmetic operations
        FORCEINLINE Vector3D operator + ( const Vector3D& rkVector ) const
        {
            return Vector3D(
                x + rkVector.x,
                y + rkVector.y,
                z + rkVector.z);
        }

        FORCEINLINE Vector3D operator - ( const Vector3D& rkVector ) const
        {
            return Vector3D(
                x - rkVector.x,
                y - rkVector.y,
                z - rkVector.z);
        }

        FORCEINLINE Vector3D operator * ( const double fScalar ) const
        {
            return Vector3D(
                x * fScalar,
                y * fScalar,
                z * fScalar);
        }

        FORCEINLINE Vector3D operator * ( const Vector3D& rhs) const
        {
            return Vector3D(
                x * rhs.x,
                y * rhs.y,
                z * rhs.z);
        }

        FORCEINLINE Vector3D operator / ( const double fScalar ) const
        {
            assert( fScalar != 0.0 );

            double fInv = 1.0f / fScalar;

            return Vector3D(
                x * fInv,
                y * fInv,
                z * fInv);
        }

        FORCEINLINE Vector3D operator / ( const Vector3D& rhs) const
        {
            return Vector3D(
                x / rhs.x,
                y / rhs.y,
                z / rhs.z);
        }

        FORCEINLINE const Vector3D& operator + () const
        {
            return *this;
        }

        FORCEINLINE Vector3D operator - () const
        {
            return Vector3D(-x, -y, -z);
        }

        // overloaded operators to help Vector3D
        FORCEINLINE friend Vector3D operator * ( const double fScalar, const Vector3D& rkVector )
        {
            return Vector3D(
                fScalar * rkVector.x,
                fScalar * rkVector.y,
                fScalar * rkVector.z);
        }

        FORCEINLINE friend Vector3D operator / ( const double fScalar, const Vector3D& rkVector )
        {
            return Vector3D(
                fScalar / rkVector.x,
                fScalar / rkVector.y,
                fScalar / rkVector.z);
        }

        FORCEINLINE friend Vector3D operator + (const Vector3D& lhs, const double rhs)
        {
            return Vector3D(
                lhs.x + rhs,
                lhs.y + rhs,
                lhs.z + rhs);
        }

        FORCEINLINE friend Vector3D operator + (const double lhs, const Vector3D& rhs)
        {
            return Vector3D(
                lhs + rhs.x,
                lhs + rhs.y,
                lhs + rhs.z);
        }

        FORCEINLINE friend Vector3D operator - (const Vector3D& lhs, const double rhs)
        {
            return Vector3D(
                lhs.x - rhs,
                lhs.y - rhs,
                lhs.z - rhs);
        }

        FORCEINLINE friend Vector3D operator - (const double lhs, const Vector3D& rhs)
        {
            return Vector3D(
                lhs - rhs.x,
                lhs - rhs.y,
                lhs - rhs.z);
        }

        // arithmetic updates
        FORCEINLINE Vector3D& operator += ( const Vector3D& rkVector )
        {
            x += rkVector.x;
            y += rkVector.y;
            z += rkVector.z;

            return *this;
        }

        FORCEINLINE Vector3D& operator += ( const double fScalar )
        {
            x += fScalar;
            y += fScalar;
            z += fScalar;
            return *this;
        }

        FORCEINLINE Vector3D& operator -= ( const Vector3D& rkVector )
        {
            x -= rkVector.x;
            y -= rkVector.y;
            z -= rkVector.z;

            return *this;
        }

        FORCEINLINE Vector3D& operator -= ( const double fScalar )
        {
            x -= fScalar;
            y -= fScalar;
            z -= fScalar;
            return *this;
        }

        FORCEINLINE Vector3D& operator *= ( const double fScalar )
        {
            x *= fScalar;
            y *= fScalar;
            z *= fScalar;
            return *this;
        }

        FORCEINLINE Vector3D& operator *= ( const Vector3D& rkVector )
        {
            x *= rkVector.x;
            y *= rkVector.y;
            z *= rkVector.z;

            return *this;
        }

        FORCEINLINE Vector3D& operator /= ( const double fScalar )
        {
            assert( fScalar != 0.0 );

            double fInv = 1.0f / fScalar;

            x *= fInv;
            y *= fInv;
            z *= fInv;

            return *this;
        }

        FORCEINLINE Vector3D& operator /= ( const Vector3D& rkVector )
        {
            x /= rkVector.x;
            y /= rkVector.y;
            z /= rkVector.z;

            return *this;
        }


        /** Returns the length (magnitude) of the vector.
            @warning
                This operation requires a square root and is expensive in
                terms of CPU operations. If you don't need to know the exact
                length (e.g. for just comparing lengths) use squaredLength()
                instead.
        */
        FORCEINLINE double length () const
        {
            return sqrt( x * x + y * y + z * z );
        }

        /** Returns the square of the length(magnitude) of the vector.
            @remarks
                This  method is for efficiency - calculating the actual
                length of a vector requires a square root, which is expensive
                in terms of the operations required. This method returns the
                square of the length of the vector, i.e. the same as the
                length but before the square root is taken. Use this if you
                want to find the longest / shortest vector without incurring
                the square root.
        */
        FORCEINLINE double squaredLength () const
        {
            return x * x + y * y + z * z;
        }

        /** Returns the distance to another vector.
            @warning
                This operation requires a square root and is expensive in
                terms of CPU operations. If you don't need to know the exact
                distance (e.g. for just comparing distances) use squaredDistance()
                instead.
        */
        FORCEINLINE double distance(const Vector3D& rhs) const
        {
            return (*this - rhs).length();
        }

        /** Returns the square of the distance to another vector.
            @remarks
                This method is for efficiency - calculating the actual
                distance to another vector requires a square root, which is
                expensive in terms of the operations required. This method
                returns the square of the distance to another vector, i.e.
                the same as the distance but before the square root is taken.
                Use this if you want to find the longest / shortest distance
                without incurring the square root.
        */
        FORCEINLINE double squaredDistance(const Vector3D& rhs) const
        {
            return (*this - rhs).squaredLength();
        }

        /** Calculates the dot (scalar) product of this vector with another.
            @remarks
                The dot product can be used to calculate the angle between 2
                vectors. If both are unit vectors, the dot product is the
                cosine of the angle; otherwise the dot product must be
                divided by the product of the lengths of both vectors to get
                the cosine of the angle. This result can further be used to
                calculate the distance of a point from a plane.
            @param
                vec Vector with which to calculate the dot product (together
                with this one).
            @returns
                A float representing the dot product value.
        */
        FORCEINLINE double dotProduct(const Vector3D& vec) const
        {
            return x * vec.x + y * vec.y + z * vec.z;
        }

        /** Calculates the absolute dot (scalar) product of this vector with another.
            @remarks
                This function work similar dotProduct, except it use absolute value
                of each component of the vector to computing.
            @param
                vec Vector with which to calculate the absolute dot product (together
                with this one).
            @returns
                A double representing the absolute dot product value.
        */
        FORCEINLINE double absDotProduct(const Vector3D& vec) const
        {
            return abs(x * vec.x) + abs(y * vec.y) + abs(z * vec.z);
        }

        /** Normalises the vector.
            @remarks
                This method normalises the vector such that it's
                length / magnitude is 1. The result is called a unit vector.
            @note
                This function will not crash for zero-sized vectors, but there
                will be no changes made to their components.
            @returns The previous length of the vector.
        */
        FORCEINLINE double normalise()
        {
            double fLength = sqrt( x * x + y * y + z * z );

            // Will also work for zero-sized vectors, but will change nothing
            if ( fLength > 1e-08 )
            {
                double fInvLength = 1.0f / fLength;
                x *= fInvLength;
                y *= fInvLength;
                z *= fInvLength;
            }

            return fLength;
        }

        /** Calculates the cross-product of 2 vectors, i.e. the vector that
            lies perpendicular to them both.
            @remarks
                The cross-product is normally used to calculate the normal
                vector of a plane, by calculating the cross-product of 2
                non-equivalent vectors which lie on the plane (e.g. 2 edges
                of a triangle).
            @param
                vec Vector which, together with this one, will be used to
                calculate the cross-product.
            @returns
                A vector which is the result of the cross-product. This
                vector will <b>NOT</b> be normalised, to maximise efficiency
                - call Vector3D::normalise on the result if you wish this to
                be done. As for which side the resultant vector will be on, the
                returned vector will be on the side from which the arc from 'this'
                to rkVector is anticlockwise, e.g. UNIT_Y.crossProduct(UNIT_Z)
                = UNIT_X, whilst UNIT_Z.crossProduct(UNIT_Y) = -UNIT_X.
				This is because OGRE uses a right-handed coordinate system.
            @par
                For a clearer explanation, look a the left and the bottom edges
                of your monitor's screen. Assume that the first vector is the
                left edge and the second vector is the bottom edge, both of
                them starting from the lower-left corner of the screen. The
                resulting vector is going to be perpendicular to both of them
                and will go <i>inside</i> the screen, towards the cathode tube
                (assuming you're using a CRT monitor, of course).
        */
        FORCEINLINE Vector3D crossProduct( const Vector3D& rkVector ) const
        {
            return Vector3D(
                y * rkVector.z - z * rkVector.y,
                z * rkVector.x - x * rkVector.z,
                x * rkVector.y - y * rkVector.x);
        }

        /** Returns a vector at a point half way between this and the passed
            in vector.
        */
        FORCEINLINE Vector3D midPoint( const Vector3D& vec ) const
        {
            return Vector3D(
                ( x + vec.x ) * 0.5f,
                ( y + vec.y ) * 0.5f,
                ( z + vec.z ) * 0.5f );
        }

        /** Returns true if the vector's scalar components are all greater
            that the ones of the vector it is compared against.
        */
        FORCEINLINE bool operator < ( const Vector3D& rhs ) const
        {
            if( x < rhs.x && y < rhs.y && z < rhs.z )
                return true;
            return false;
        }

        /** Returns true if the vector's scalar components are all smaller
            that the ones of the vector it is compared against.
        */
        FORCEINLINE bool operator > ( const Vector3D& rhs ) const
        {
            if( x > rhs.x && y > rhs.y && z > rhs.z )
                return true;
            return false;
        }

        /** Sets this vector's components to the minimum of its own and the
            ones of the passed in vector.
            @remarks
                'Minimum' in this case means the combination of the lowest
                value of x, y and z from both vectors. Lowest is taken just
                numerically, not magnitude, so -1 < 0.
        */
        FORCEINLINE void makeFloor( const Vector3D& cmp )
        {
            if( cmp.x < x ) x = cmp.x;
            if( cmp.y < y ) y = cmp.y;
            if( cmp.z < z ) z = cmp.z;
        }

        /** Sets this vector's components to the maximum of its own and the
            ones of the passed in vector.
            @remarks
                'Maximum' in this case means the combination of the highest
                value of x, y and z from both vectors. Highest is taken just
                numerically, not magnitude, so 1 > -3.
        */
        FORCEINLINE void makeCeil( const Vector3D& cmp )
        {
            if( cmp.x > x ) x = cmp.x;
            if( cmp.y > y ) y = cmp.y;
            if( cmp.z > z ) z = cmp.z;
        }

        /** Generates a vector perpendicular to this vector (eg an 'up' vector).
            @remarks
                This method will return a vector which is perpendicular to this
                vector. There are an infinite number of possibilities but this
                method will guarantee to generate one of them. If you need more
                control you should use the Quaternion class.
        */
        FORCEINLINE Vector3D perpendicular(void) const
        {
            static const double fSquareZero = (double)(1e-06 * 1e-06);

            Vector3D perp = this->crossProduct( Vector3D::UNIT_X );

            // Check length
            if( perp.squaredLength() < fSquareZero )
            {
                /* This vector is the Y axis multiplied by a scalar, so we have
                   to use another axis.
                */
                perp = this->crossProduct( Vector3D::UNIT_Y );
            }
			perp.normalise();

            return perp;
        }
        ///** Generates a new random vector which deviates from this vector by a
        //    given angle in a random direction.
        //    @remarks
        //        This method assumes that the random number generator has already
        //        been seeded appropriately.
        //    @param
        //        angle The angle at which to deviate
        //    @param
        //        up Any vector perpendicular to this one (which could generated
        //        by cross-product of this vector and any other non-colinear
        //        vector). If you choose not to provide this the function will
        //        derive one on it's own, however if you provide one yourself the
        //        function will be faster (this allows you to reuse up vectors if
        //        you call this method more than once)
        //    @returns
        //        A random vector which deviates from this vector by angle. This
        //        vector will not be normalised, normalise it if you wish
        //        afterwards.
        //*/
        //FORCEINLINE Vector3D randomDeviant(
        //    const Radian& angle,
        //    const Vector3D& up = Vector3D::ZERO ) const
        //{
        //    Vector3D newUp;

        //    if (up == Vector3D::ZERO)
        //    {
        //        // Generate an up vector
        //        newUp = this->perpendicular();
        //    }
        //    else
        //    {
        //        newUp = up;
        //    }

        //    // Rotate up vector by random amount around this
        //    Quaternion q;
        //    q.FromAngleAxis( Radian(Math::UnitRandom() * Math::TWO_PI), *this );
        //    newUp = q * newUp;

        //    // Finally rotate this by given angle around randomised up
        //    q.FromAngleAxis( angle, newUp );
        //    return q * (*this);
        //}

		/** Gets the angle between 2 vectors.
		@remarks
			Vectors do not have to be unit-length but must represent directions.
		*/
		FORCEINLINE Radian angleBetween(const Vector3D& dest)
        {
			double lenProduct = length() * dest.length();

			// Divide by zero check
			if(lenProduct < 1e-6f)
				lenProduct = 1e-6f;

			double f = dotProduct(dest) / lenProduct;

			f = Math::Clamp(f, (double)-1.0, (double)1.0);
			return Math::ACos(f);

        }
   //     /** Gets the shortest arc quaternion to rotate this vector to the destination
   //         vector.
   //     @remarks
   //         If you call this with a dest vector that is close to the inverse
   //         of this vector, we will rotate 180 degrees around the 'fallbackAxis'
			//(if specified, or a generated axis if not) since in this case
			//ANY axis of rotation is valid.
   //     */
   //     Quaternion getRotationTo(const Vector3D& dest,
			//const Vector3D& fallbackAxis = Vector3D::ZERO) const
   //     {
   //         // Based on Stan Melax's article in Game Programming Gems
   //         Quaternion q;
   //         // Copy, since cannot modify local
   //         Vector3D v0 = *this;
   //         Vector3D v1 = dest;
   //         v0.normalise();
   //         v1.normalise();

   //         double d = v0.dotProduct(v1);
   //         // If dot == 1, vectors are the same
   //         if (d >= 1.0f)
   //         {
   //             return Quaternion::IDENTITY;
   //         }
			//if (d < (1e-6f - 1.0f))
			//{
			//	if (fallbackAxis != Vector3D::ZERO)
			//	{
			//		// rotate 180 degrees about the fallback axis
			//		q.FromAngleAxis(Radian(Math::PI), fallbackAxis);
			//	}
			//	else
			//	{
			//		// Generate an axis
			//		Vector3D axis = Vector3D::UNIT_X.crossProduct(*this);
			//		if (axis.isZeroLength()) // pick another if colinear
			//			axis = Vector3D::UNIT_Y.crossProduct(*this);
			//		axis.normalise();
			//		q.FromAngleAxis(Radian(Math::PI), axis);
			//	}
			//}
			//else
			//{
   //             double s = Math::Sqrt( (1+d)*2 );
	  //          double invs = 1 / s;

			//	Vector3D c = v0.crossProduct(v1);

   // 	        q.x = c.x * invs;
   //     	    q.y = c.y * invs;
   //         	q.z = c.z * invs;
   //         	q.w = s * 0.5f;
			//	q.normalise();
			//}
   //         return q;
   //     }

        /** Returns true if this vector is zero length. */
        FORCEINLINE bool isZeroLength(void) const
        {
            double sqlen = (x * x) + (y * y) + (z * z);
            return (sqlen < (1e-06 * 1e-06));

        }

        /** As normalise, except that this vector is unaffected and the
            normalised vector is returned as a copy. */
        FORCEINLINE Vector3D normalisedCopy(void) const
        {
            Vector3D ret = *this;
            ret.normalise();
            return ret;
        }

        /** Calculates a reflection vector to the plane with the given normal .
        @remarks NB assumes 'this' is pointing AWAY FROM the plane, invert if it is not.
        */
        FORCEINLINE Vector3D reflect(const Vector3D& normal) const
        {
            return Vector3D( *this - ( 2 * this->dotProduct(normal) * normal ) );
        }

		///** Returns whether this vector is within a positional tolerance
		//	of another vector.
		//@param rhs The vector to compare with
		//@param tolerance The amount that each element of the vector may vary by
		//	and still be considered equal
		//*/
		//FORCEINLINE bool positionEquals(const Vector3D& rhs, double tolerance = 1e-03) const
		//{
		//	return Math::RealEqual(x, rhs.x, tolerance) &&
		//		Math::RealEqual(y, rhs.y, tolerance) &&
		//		Math::RealEqual(z, rhs.z, tolerance);

		//}

		/** Returns whether this vector is within a positional tolerance
			of another vector, also take scale of the vectors into account.
		@param rhs The vector to compare with
		@param tolerance The amount (related to the scale of vectors) that distance
            of the vector may vary by and still be considered close
		*/
		FORCEINLINE bool positionCloses(const Vector3D& rhs, double tolerance = 1e-03f) const
		{
			return squaredDistance(rhs) <=
                (squaredLength() + rhs.squaredLength()) * tolerance;
		}

		///** Returns whether this vector is within a directional tolerance
		//	of another vector.
		//@param rhs The vector to compare with
		//@param tolerance The maximum angle by which the vectors may vary and
		//	still be considered equal
		//@note Both vectors should be normalised.
		//*/
		//FORCEINLINE bool directionEquals(const Vector3D& rhs,
		//	const Radian& tolerance) const
		//{
		//	double dot = dotProduct(rhs);
		//	Radian angle = Math::ACos(dot);

		//	return Math::Abs(angle.valueRadians()) <= tolerance.valueRadians();

		//}

		/// Check whether this vector contains valid values
		FORCEINLINE bool isNaN() const
		{
			//return Math::isNaN(x) || Math::isNaN(y) || Math::isNaN(z);
			return x!=x || y!=y || z!=z;
		}

        // special points
        static const Vector3D ZERO;
        static const Vector3D UNIT_X;
        static const Vector3D UNIT_Y;
        static const Vector3D UNIT_Z;
        static const Vector3D NEGATIVE_UNIT_X;
        static const Vector3D NEGATIVE_UNIT_Y;
        static const Vector3D NEGATIVE_UNIT_Z;
        static const Vector3D UNIT_SCALE;

        /** Function for writing to a stream.
        */
        FORCEINLINE _OgreExport friend std::ostream& operator <<
            ( std::ostream& o, const Vector3D& v )
        {
            o << "Vector3D(" << v.x << ", " << v.y << ", " << v.z << ")";
            return o;
        }
    };
	/** @} */
	/** @} */

}
#endif
