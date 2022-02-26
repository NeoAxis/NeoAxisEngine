// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
#ifndef __Vector3I_H__
#define __Vector3I_H__

#include "OgrePrerequisites.h"
#include "OgreMath.h"
#include "OgreQuaternion.h"

namespace Ogre
{
    class Vector3I
    {
    public:
		int x, y, z;

    public:
        FORCEINLINE Vector3I()
        {
        }

        FORCEINLINE Vector3I( const int fX, const int fY, const int fZ )
            : x( fX ), y( fY ), z( fZ )
        {
        }

        FORCEINLINE explicit Vector3I( const int afCoordinate[3] )
            : x( afCoordinate[0] ),
              y( afCoordinate[1] ),
              z( afCoordinate[2] )
        {
        }

        FORCEINLINE explicit Vector3I( int* const r )
            : x( r[0] ), y( r[1] ), z( r[2] )
        {
        }

        FORCEINLINE explicit Vector3I( const int scaler )
            : x( scaler )
            , y( scaler )
            , z( scaler )
        {
        }


		/** Exchange the contents of this vector with another. 
		*/
		FORCEINLINE void swap(Vector3I& other)
		{
			std::swap(x, other.x);
			std::swap(y, other.y);
			std::swap(z, other.z);
		}

		FORCEINLINE int operator [] ( const size_t i ) const
        {
            assert( i < 3 );

            return *(&x+i);
        }

		FORCEINLINE int& operator [] ( const size_t i )
        {
            assert( i < 3 );

            return *(&x+i);
        }
		/// Pointer accessor for direct copying
		FORCEINLINE int* ptr()
		{
			return &x;
		}
		/// Pointer accessor for direct copying
		FORCEINLINE const int* ptr() const
		{
			return &x;
		}

        /** Assigns the value of the other vector.
            @param
                rkVector The other vector
        */
        FORCEINLINE Vector3I& operator = ( const Vector3I& rkVector )
        {
            x = rkVector.x;
            y = rkVector.y;
            z = rkVector.z;

            return *this;
        }

        FORCEINLINE Vector3I& operator = ( const int fScaler )
        {
            x = fScaler;
            y = fScaler;
            z = fScaler;

            return *this;
        }

        FORCEINLINE bool operator == ( const Vector3I& rkVector ) const
        {
            return ( x == rkVector.x && y == rkVector.y && z == rkVector.z );
        }

        FORCEINLINE bool operator != ( const Vector3I& rkVector ) const
        {
            return ( x != rkVector.x || y != rkVector.y || z != rkVector.z );
        }

        // arithmetic operations
        FORCEINLINE Vector3I operator + ( const Vector3I& rkVector ) const
        {
            return Vector3I(
                x + rkVector.x,
                y + rkVector.y,
                z + rkVector.z);
        }

        FORCEINLINE Vector3I operator - ( const Vector3I& rkVector ) const
        {
            return Vector3I(
                x - rkVector.x,
                y - rkVector.y,
                z - rkVector.z);
        }

        FORCEINLINE Vector3I operator * ( const int fScalar ) const
        {
            return Vector3I(
                x * fScalar,
                y * fScalar,
                z * fScalar);
        }

        FORCEINLINE Vector3I operator * ( const Vector3I& rhs ) const
        {
            return Vector3I(
                x * rhs.x,
                y * rhs.y,
                z * rhs.z);
        }

        FORCEINLINE Vector3I operator / ( const int scalar ) const
        {
            assert( scalar != 0 );

            return Vector3I(
                x / scalar,
                y / scalar,
                z / scalar);
        }

        FORCEINLINE Vector3I operator / ( const Vector3I& rhs ) const
        {
            return Vector3I(
                x / rhs.x,
                y / rhs.y,
                z / rhs.z);
        }

        FORCEINLINE const Vector3I& operator + () const
        {
            return *this;
        }

        FORCEINLINE Vector3I operator - () const
        {
            return Vector3I(-x, -y, -z);
        }

        // overloaded operators to help Vector3I
        FORCEINLINE friend Vector3I operator * ( const int fScalar, const Vector3I& rkVector )
        {
            return Vector3I(
                fScalar * rkVector.x,
                fScalar * rkVector.y,
                fScalar * rkVector.z);
        }

        FORCEINLINE friend Vector3I operator / ( const int fScalar, const Vector3I& rkVector )
        {
            return Vector3I(
                fScalar / rkVector.x,
                fScalar / rkVector.y,
                fScalar / rkVector.z);
        }

        FORCEINLINE friend Vector3I operator + (const Vector3I& lhs, const int rhs)
        {
            return Vector3I(
                lhs.x + rhs,
                lhs.y + rhs,
                lhs.z + rhs);
        }

        FORCEINLINE friend Vector3I operator + (const int lhs, const Vector3I& rhs)
        {
            return Vector3I(
                lhs + rhs.x,
                lhs + rhs.y,
                lhs + rhs.z);
        }

        FORCEINLINE friend Vector3I operator - (const Vector3I& lhs, const int rhs)
        {
            return Vector3I(
                lhs.x - rhs,
                lhs.y - rhs,
                lhs.z - rhs);
        }

        FORCEINLINE friend Vector3I operator - (const int lhs, const Vector3I& rhs)
        {
            return Vector3I(
                lhs - rhs.x,
                lhs - rhs.y,
                lhs - rhs.z);
        }

        // arithmetic updates
        FORCEINLINE Vector3I& operator += ( const Vector3I& rkVector )
        {
            x += rkVector.x;
            y += rkVector.y;
            z += rkVector.z;

            return *this;
        }

        FORCEINLINE Vector3I& operator += ( const int fScalar )
        {
            x += fScalar;
            y += fScalar;
            z += fScalar;
            return *this;
        }

        FORCEINLINE Vector3I& operator -= ( const Vector3I& rkVector )
        {
            x -= rkVector.x;
            y -= rkVector.y;
            z -= rkVector.z;

            return *this;
        }

        FORCEINLINE Vector3I& operator -= ( const int fScalar )
        {
            x -= fScalar;
            y -= fScalar;
            z -= fScalar;
            return *this;
        }

        FORCEINLINE Vector3I& operator *= ( const int fScalar )
        {
            x *= fScalar;
            y *= fScalar;
            z *= fScalar;
            return *this;
        }

        FORCEINLINE Vector3I& operator *= ( const Vector3I& rkVector )
        {
            x *= rkVector.x;
            y *= rkVector.y;
            z *= rkVector.z;

            return *this;
        }

        FORCEINLINE Vector3I& operator /= ( const int fScalar )
        {
            assert( fScalar != 0 );

			x /= fScalar;
            y /= fScalar;
            z /= fScalar;

            return *this;
        }

        FORCEINLINE Vector3I& operator /= ( const Vector3I& rkVector )
        {
            x /= rkVector.x;
            y /= rkVector.y;
            z /= rkVector.z;

            return *this;
        }

        /** Returns true if the vector's scalar components are all greater
            that the ones of the vector it is compared against.
        */
        FORCEINLINE bool operator < ( const Vector3I& rhs ) const
        {
            if( x < rhs.x && y < rhs.y && z < rhs.z )
                return true;
            return false;
        }

        /** Returns true if the vector's scalar components are all smaller
            that the ones of the vector it is compared against.
        */
        FORCEINLINE bool operator > ( const Vector3I& rhs ) const
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
        FORCEINLINE void makeFloor( const Vector3I& cmp )
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
        FORCEINLINE void makeCeil( const Vector3I& cmp )
        {
            if( cmp.x > x ) x = cmp.x;
            if( cmp.y > y ) y = cmp.y;
            if( cmp.z > z ) z = cmp.z;
        }

		FORCEINLINE Vector3 toVector3() const
		{
			return Vector3( x, y, z );
		}

		Vector3D toVector3D() const;

        // special points
        static const Vector3I ZERO;
        static const Vector3I UNIT_X;
        static const Vector3I UNIT_Y;
        static const Vector3I UNIT_Z;
        static const Vector3I NEGATIVE_UNIT_X;
        static const Vector3I NEGATIVE_UNIT_Y;
        static const Vector3I NEGATIVE_UNIT_Z;
        static const Vector3I UNIT_SCALE;

        /** Function for writing to a stream.
        */
        FORCEINLINE _OgreExport friend std::ostream& operator <<
            ( std::ostream& o, const Vector3I& v )
        {
            o << "Vector3I(" << v.x << ", " << v.y << ", " << v.z << ")";
            return o;
        }
    };
	/** @} */
	/** @} */

}
#endif
