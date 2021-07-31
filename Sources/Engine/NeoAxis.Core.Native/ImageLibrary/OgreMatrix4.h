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
#ifndef __Matrix4__
#define __Matrix4__

// Precompiler options
#include "OgrePrerequisites.h"

#include "OgreVector3.h"
#include "OgreMatrix3.h"
#include "OgreVector4.h"
#include "OgrePlane.h"
namespace Ogre
{

	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup Math
	*  @{
	*/
    /** Class encapsulating a standard 4x4 homogeneous matrix.
        @remarks
            OGRE uses column vectors when applying matrix multiplications,
            This means a vector is represented as a single column, 4-row
            matrix. This has the effect that the transformations implemented
            by the matrices happens right-to-left e.g. if vector V is to be
            transformed by M1 then M2 then M3, the calculation would be
            M3 * M2 * M1 * V. The order that matrices are concatenated is
            vital since matrix multiplication is not commutative, i.e. you
            can get a different result if you concatenate in the wrong order.
        @par
            The use of column vectors and right-to-left ordering is the
            standard in most mathematical texts, and is the same as used in
            OpenGL. It is, however, the opposite of Direct3D, which has
            inexplicably chosen to differ from the accepted standard and uses
            row vectors and left-to-right matrix multiplication.
        @par
            OGRE deals with the differences between D3D and OpenGL etc.
            internally when operating through different render systems. OGRE
            users only need to conform to standard maths conventions, i.e.
            right-to-left matrix multiplication, (OGRE transposes matrices it
            passes to D3D to compensate).
        @par
            The generic form M * V which shows the layout of the matrix 
            entries is shown below:
            <pre>
                [ m[0][0]  m[0][1]  m[0][2]  m[0][3] ]   {x}
                | m[1][0]  m[1][1]  m[1][2]  m[1][3] | * {y}
                | m[2][0]  m[2][1]  m[2][2]  m[2][3] |   {z}
                [ m[3][0]  m[3][1]  m[3][2]  m[3][3] ]   {1}
            </pre>
    */
    class _OgreExport Matrix4
    {
    protected:
        /// The matrix entries, indexed by [row][col].
        union {
            Real m[4][4];
            Real _m[16];
        };
    public:
        /** Default constructor.
            @note
                It does <b>NOT</b> initialize the matrix for efficiency.
        */
        FORCEINLINE Matrix4()
        {
        }

        FORCEINLINE Matrix4(
            Real m00, Real m01, Real m02, Real m03,
            Real m10, Real m11, Real m12, Real m13,
            Real m20, Real m21, Real m22, Real m23,
            Real m30, Real m31, Real m32, Real m33 )
        {
            m[0][0] = m00;
            m[0][1] = m01;
            m[0][2] = m02;
            m[0][3] = m03;
            m[1][0] = m10;
            m[1][1] = m11;
            m[1][2] = m12;
            m[1][3] = m13;
            m[2][0] = m20;
            m[2][1] = m21;
            m[2][2] = m22;
            m[2][3] = m23;
            m[3][0] = m30;
            m[3][1] = m31;
            m[3][2] = m32;
            m[3][3] = m33;
        }

        /** Creates a standard 4x4 transformation matrix with a zero translation part from a rotation/scaling 3x3 matrix.
         */

        FORCEINLINE Matrix4(const Matrix3& m3x3)
        {
          operator=(IDENTITY);
          operator=(m3x3);
        }

        /** Creates a standard 4x4 transformation matrix with a zero translation part from a rotation/scaling Quaternion.
         */
        
        FORCEINLINE Matrix4(const Quaternion& rot)
        {
          Matrix3 m3x3;
          rot.ToRotationMatrix(m3x3);
          operator=(IDENTITY);
          operator=(m3x3);
        }
        

		/** Exchange the contents of this matrix with another. 
		*/
		FORCEINLINE void swap(Matrix4& other)
		{
			std::swap(m[0][0], other.m[0][0]);
			std::swap(m[0][1], other.m[0][1]);
			std::swap(m[0][2], other.m[0][2]);
			std::swap(m[0][3], other.m[0][3]);
			std::swap(m[1][0], other.m[1][0]);
			std::swap(m[1][1], other.m[1][1]);
			std::swap(m[1][2], other.m[1][2]);
			std::swap(m[1][3], other.m[1][3]);
			std::swap(m[2][0], other.m[2][0]);
			std::swap(m[2][1], other.m[2][1]);
			std::swap(m[2][2], other.m[2][2]);
			std::swap(m[2][3], other.m[2][3]);
			std::swap(m[3][0], other.m[3][0]);
			std::swap(m[3][1], other.m[3][1]);
			std::swap(m[3][2], other.m[3][2]);
			std::swap(m[3][3], other.m[3][3]);
		}

        FORCEINLINE Real* operator [] ( size_t iRow )
        {
            assert( iRow < 4 );
            return m[iRow];
        }

        FORCEINLINE const Real *operator [] ( size_t iRow ) const
        {
            assert( iRow < 4 );
            return m[iRow];
        }

        FORCEINLINE Matrix4 concatenate(const Matrix4 &m2) const
        {
            Matrix4 r;
            r.m[0][0] = m[0][0] * m2.m[0][0] + m[0][1] * m2.m[1][0] + m[0][2] * m2.m[2][0] + m[0][3] * m2.m[3][0];
            r.m[0][1] = m[0][0] * m2.m[0][1] + m[0][1] * m2.m[1][1] + m[0][2] * m2.m[2][1] + m[0][3] * m2.m[3][1];
            r.m[0][2] = m[0][0] * m2.m[0][2] + m[0][1] * m2.m[1][2] + m[0][2] * m2.m[2][2] + m[0][3] * m2.m[3][2];
            r.m[0][3] = m[0][0] * m2.m[0][3] + m[0][1] * m2.m[1][3] + m[0][2] * m2.m[2][3] + m[0][3] * m2.m[3][3];

            r.m[1][0] = m[1][0] * m2.m[0][0] + m[1][1] * m2.m[1][0] + m[1][2] * m2.m[2][0] + m[1][3] * m2.m[3][0];
            r.m[1][1] = m[1][0] * m2.m[0][1] + m[1][1] * m2.m[1][1] + m[1][2] * m2.m[2][1] + m[1][3] * m2.m[3][1];
            r.m[1][2] = m[1][0] * m2.m[0][2] + m[1][1] * m2.m[1][2] + m[1][2] * m2.m[2][2] + m[1][3] * m2.m[3][2];
            r.m[1][3] = m[1][0] * m2.m[0][3] + m[1][1] * m2.m[1][3] + m[1][2] * m2.m[2][3] + m[1][3] * m2.m[3][3];

            r.m[2][0] = m[2][0] * m2.m[0][0] + m[2][1] * m2.m[1][0] + m[2][2] * m2.m[2][0] + m[2][3] * m2.m[3][0];
            r.m[2][1] = m[2][0] * m2.m[0][1] + m[2][1] * m2.m[1][1] + m[2][2] * m2.m[2][1] + m[2][3] * m2.m[3][1];
            r.m[2][2] = m[2][0] * m2.m[0][2] + m[2][1] * m2.m[1][2] + m[2][2] * m2.m[2][2] + m[2][3] * m2.m[3][2];
            r.m[2][3] = m[2][0] * m2.m[0][3] + m[2][1] * m2.m[1][3] + m[2][2] * m2.m[2][3] + m[2][3] * m2.m[3][3];

            r.m[3][0] = m[3][0] * m2.m[0][0] + m[3][1] * m2.m[1][0] + m[3][2] * m2.m[2][0] + m[3][3] * m2.m[3][0];
            r.m[3][1] = m[3][0] * m2.m[0][1] + m[3][1] * m2.m[1][1] + m[3][2] * m2.m[2][1] + m[3][3] * m2.m[3][1];
            r.m[3][2] = m[3][0] * m2.m[0][2] + m[3][1] * m2.m[1][2] + m[3][2] * m2.m[2][2] + m[3][3] * m2.m[3][2];
            r.m[3][3] = m[3][0] * m2.m[0][3] + m[3][1] * m2.m[1][3] + m[3][2] * m2.m[2][3] + m[3][3] * m2.m[3][3];

            return r;
        }

        /** Matrix concatenation using '*'.
        */
        FORCEINLINE Matrix4 operator * ( const Matrix4 &m2 ) const
        {
            return concatenate( m2 );
        }

        FORCEINLINE static void concatenate(const Matrix4& m1, const Matrix4& m2, Matrix4& result)
		{
            result.m[0][0] = m1.m[0][0] * m2.m[0][0] + m1.m[0][1] * m2.m[1][0] + m1.m[0][2] * m2.m[2][0] + m1.m[0][3] * m2.m[3][0];
            result.m[0][1] = m1.m[0][0] * m2.m[0][1] + m1.m[0][1] * m2.m[1][1] + m1.m[0][2] * m2.m[2][1] + m1.m[0][3] * m2.m[3][1];
            result.m[0][2] = m1.m[0][0] * m2.m[0][2] + m1.m[0][1] * m2.m[1][2] + m1.m[0][2] * m2.m[2][2] + m1.m[0][3] * m2.m[3][2];
            result.m[0][3] = m1.m[0][0] * m2.m[0][3] + m1.m[0][1] * m2.m[1][3] + m1.m[0][2] * m2.m[2][3] + m1.m[0][3] * m2.m[3][3];

            result.m[1][0] = m1.m[1][0] * m2.m[0][0] + m1.m[1][1] * m2.m[1][0] + m1.m[1][2] * m2.m[2][0] + m1.m[1][3] * m2.m[3][0];
            result.m[1][1] = m1.m[1][0] * m2.m[0][1] + m1.m[1][1] * m2.m[1][1] + m1.m[1][2] * m2.m[2][1] + m1.m[1][3] * m2.m[3][1];
            result.m[1][2] = m1.m[1][0] * m2.m[0][2] + m1.m[1][1] * m2.m[1][2] + m1.m[1][2] * m2.m[2][2] + m1.m[1][3] * m2.m[3][2];
            result.m[1][3] = m1.m[1][0] * m2.m[0][3] + m1.m[1][1] * m2.m[1][3] + m1.m[1][2] * m2.m[2][3] + m1.m[1][3] * m2.m[3][3];

            result.m[2][0] = m1.m[2][0] * m2.m[0][0] + m1.m[2][1] * m2.m[1][0] + m1.m[2][2] * m2.m[2][0] + m1.m[2][3] * m2.m[3][0];
            result.m[2][1] = m1.m[2][0] * m2.m[0][1] + m1.m[2][1] * m2.m[1][1] + m1.m[2][2] * m2.m[2][1] + m1.m[2][3] * m2.m[3][1];
            result.m[2][2] = m1.m[2][0] * m2.m[0][2] + m1.m[2][1] * m2.m[1][2] + m1.m[2][2] * m2.m[2][2] + m1.m[2][3] * m2.m[3][2];
            result.m[2][3] = m1.m[2][0] * m2.m[0][3] + m1.m[2][1] * m2.m[1][3] + m1.m[2][2] * m2.m[2][3] + m1.m[2][3] * m2.m[3][3];

            result.m[3][0] = m1.m[3][0] * m2.m[0][0] + m1.m[3][1] * m2.m[1][0] + m1.m[3][2] * m2.m[2][0] + m1.m[3][3] * m2.m[3][0];
            result.m[3][1] = m1.m[3][0] * m2.m[0][1] + m1.m[3][1] * m2.m[1][1] + m1.m[3][2] * m2.m[2][1] + m1.m[3][3] * m2.m[3][1];
            result.m[3][2] = m1.m[3][0] * m2.m[0][2] + m1.m[3][1] * m2.m[1][2] + m1.m[3][2] * m2.m[2][2] + m1.m[3][3] * m2.m[3][2];
            result.m[3][3] = m1.m[3][0] * m2.m[0][3] + m1.m[3][1] * m2.m[1][3] + m1.m[3][2] * m2.m[2][3] + m1.m[3][3] * m2.m[3][3];
		}

        /** Vector transformation using '*'.
            @remarks
                Transforms the given 3-D vector by the matrix, projecting the 
                result back into <i>w</i> = 1.
            @note
                This means that the initial <i>w</i> is considered to be 1.0,
                and then all the tree elements of the resulting 3-D vector are
                divided by the resulting <i>w</i>.
        */
        FORCEINLINE Vector3 operator * ( const Vector3 &v ) const
        {
            Vector3 r;

            Real fInvW = 1.0f / ( m[3][0] * v.x + m[3][1] * v.y + m[3][2] * v.z + m[3][3] );

            r.x = ( m[0][0] * v.x + m[0][1] * v.y + m[0][2] * v.z + m[0][3] ) * fInvW;
            r.y = ( m[1][0] * v.x + m[1][1] * v.y + m[1][2] * v.z + m[1][3] ) * fInvW;
            r.z = ( m[2][0] * v.x + m[2][1] * v.y + m[2][2] * v.z + m[2][3] ) * fInvW;

            return r;
        }
        FORCEINLINE Vector4 operator * (const Vector4& v) const
        {
            return Vector4(
                m[0][0] * v.x + m[0][1] * v.y + m[0][2] * v.z + m[0][3] * v.w, 
                m[1][0] * v.x + m[1][1] * v.y + m[1][2] * v.z + m[1][3] * v.w,
                m[2][0] * v.x + m[2][1] * v.y + m[2][2] * v.z + m[2][3] * v.w,
                m[3][0] * v.x + m[3][1] * v.y + m[3][2] * v.z + m[3][3] * v.w
                );
        }

   //     FORCEINLINE Plane operator * (const Plane& p) const
   //     {
   //         Plane ret;
			//slowly
			//Matrix4 invTrans = inverse().transpose();
			//Vector4 v4( p.normal.x, p.normal.y, p.normal.z, p.d );
			//v4 = invTrans * v4;
			//ret.normal.x = v4.x; 
			//ret.normal.y = v4.y; 
			//ret.normal.z = v4.z;
			//ret.d = v4.w / ret.normal.normalise();

   //         return ret;
   //     }


        /** Matrix addition.
        */
        FORCEINLINE Matrix4 operator + ( const Matrix4 &m2 ) const
        {
            Matrix4 r;

            r.m[0][0] = m[0][0] + m2.m[0][0];
            r.m[0][1] = m[0][1] + m2.m[0][1];
            r.m[0][2] = m[0][2] + m2.m[0][2];
            r.m[0][3] = m[0][3] + m2.m[0][3];

            r.m[1][0] = m[1][0] + m2.m[1][0];
            r.m[1][1] = m[1][1] + m2.m[1][1];
            r.m[1][2] = m[1][2] + m2.m[1][2];
            r.m[1][3] = m[1][3] + m2.m[1][3];

            r.m[2][0] = m[2][0] + m2.m[2][0];
            r.m[2][1] = m[2][1] + m2.m[2][1];
            r.m[2][2] = m[2][2] + m2.m[2][2];
            r.m[2][3] = m[2][3] + m2.m[2][3];

            r.m[3][0] = m[3][0] + m2.m[3][0];
            r.m[3][1] = m[3][1] + m2.m[3][1];
            r.m[3][2] = m[3][2] + m2.m[3][2];
            r.m[3][3] = m[3][3] + m2.m[3][3];

            return r;
        }

        /** Matrix subtraction.
        */
        FORCEINLINE Matrix4 operator - ( const Matrix4 &m2 ) const
        {
            Matrix4 r;
            r.m[0][0] = m[0][0] - m2.m[0][0];
            r.m[0][1] = m[0][1] - m2.m[0][1];
            r.m[0][2] = m[0][2] - m2.m[0][2];
            r.m[0][3] = m[0][3] - m2.m[0][3];

            r.m[1][0] = m[1][0] - m2.m[1][0];
            r.m[1][1] = m[1][1] - m2.m[1][1];
            r.m[1][2] = m[1][2] - m2.m[1][2];
            r.m[1][3] = m[1][3] - m2.m[1][3];

            r.m[2][0] = m[2][0] - m2.m[2][0];
            r.m[2][1] = m[2][1] - m2.m[2][1];
            r.m[2][2] = m[2][2] - m2.m[2][2];
            r.m[2][3] = m[2][3] - m2.m[2][3];

            r.m[3][0] = m[3][0] - m2.m[3][0];
            r.m[3][1] = m[3][1] - m2.m[3][1];
            r.m[3][2] = m[3][2] - m2.m[3][2];
            r.m[3][3] = m[3][3] - m2.m[3][3];

            return r;
        }

        /** Tests 2 matrices for equality.
        */
        FORCEINLINE bool operator == ( const Matrix4& m2 ) const
        {
            if( 
                m[0][0] != m2.m[0][0] || m[0][1] != m2.m[0][1] || m[0][2] != m2.m[0][2] || m[0][3] != m2.m[0][3] ||
                m[1][0] != m2.m[1][0] || m[1][1] != m2.m[1][1] || m[1][2] != m2.m[1][2] || m[1][3] != m2.m[1][3] ||
                m[2][0] != m2.m[2][0] || m[2][1] != m2.m[2][1] || m[2][2] != m2.m[2][2] || m[2][3] != m2.m[2][3] ||
                m[3][0] != m2.m[3][0] || m[3][1] != m2.m[3][1] || m[3][2] != m2.m[3][2] || m[3][3] != m2.m[3][3] )
                return false;
            return true;
        }

        /** Tests 2 matrices for inequality.
        */
        FORCEINLINE bool operator != ( const Matrix4& m2 ) const
        {
            if( 
                m[0][0] != m2.m[0][0] || m[0][1] != m2.m[0][1] || m[0][2] != m2.m[0][2] || m[0][3] != m2.m[0][3] ||
                m[1][0] != m2.m[1][0] || m[1][1] != m2.m[1][1] || m[1][2] != m2.m[1][2] || m[1][3] != m2.m[1][3] ||
                m[2][0] != m2.m[2][0] || m[2][1] != m2.m[2][1] || m[2][2] != m2.m[2][2] || m[2][3] != m2.m[2][3] ||
                m[3][0] != m2.m[3][0] || m[3][1] != m2.m[3][1] || m[3][2] != m2.m[3][2] || m[3][3] != m2.m[3][3] )
                return true;
            return false;
        }

        /** Assignment from 3x3 matrix.
        */
        FORCEINLINE void operator = ( const Matrix3& mat3 )
        {
            m[0][0] = mat3.m[0][0]; m[0][1] = mat3.m[0][1]; m[0][2] = mat3.m[0][2];
            m[1][0] = mat3.m[1][0]; m[1][1] = mat3.m[1][1]; m[1][2] = mat3.m[1][2];
            m[2][0] = mat3.m[2][0]; m[2][1] = mat3.m[2][1]; m[2][2] = mat3.m[2][2];
        }

        FORCEINLINE Matrix4 transpose(void) const
        {
            return Matrix4(m[0][0], m[1][0], m[2][0], m[3][0],
                           m[0][1], m[1][1], m[2][1], m[3][1],
                           m[0][2], m[1][2], m[2][2], m[3][2],
                           m[0][3], m[1][3], m[2][3], m[3][3]);
        }

        FORCEINLINE void transpose(Matrix4& result) const
		{
			result.m[0][0] = m[0][0];			
			result.m[0][1] = m[1][0];
			result.m[0][2] = m[2][0];
			result.m[0][3] = m[3][0];
			result.m[1][0] = m[0][1];
			result.m[1][1] = m[1][1];
			result.m[1][2] = m[2][1];
			result.m[1][3] = m[3][1];
			result.m[2][0] = m[0][2];
			result.m[2][1] = m[1][2];
			result.m[2][2] = m[2][2];
			result.m[2][3] = m[3][2];
			result.m[3][0] = m[0][3];
			result.m[3][1] = m[1][3];
			result.m[3][2] = m[2][3];
			result.m[3][3] = m[3][3];
		}

        FORCEINLINE void transpose(float* result) const
		{
			result[0] = m[0][0];			
			result[1] = m[1][0];
			result[2] = m[2][0];
			result[3] = m[3][0];
			result[4] = m[0][1];
			result[5] = m[1][1];
			result[6] = m[2][1];
			result[7] = m[3][1];
			result[8] = m[0][2];
			result[9] = m[1][2];
			result[10] = m[2][2];
			result[11] = m[3][2];
			result[12] = m[0][3];
			result[13] = m[1][3];
			result[14] = m[2][3];
			result[15] = m[3][3];
		}


        /*
        -----------------------------------------------------------------------
        Translation Transformation
        -----------------------------------------------------------------------
        */
        /** Sets the translation transformation part of the matrix.
        */
        FORCEINLINE void setTrans( const Vector3& v )
        {
            m[0][3] = v.x;
            m[1][3] = v.y;
            m[2][3] = v.z;
        }

        /** Extracts the translation transformation part of the matrix.
         */
        FORCEINLINE Vector3 getTrans() const
        {
          return Vector3(m[0][3], m[1][3], m[2][3]);
        }
        

        /** Builds a translation matrix
        */
        FORCEINLINE void makeTrans( const Vector3& v )
        {
            m[0][0] = 1.0; m[0][1] = 0.0; m[0][2] = 0.0; m[0][3] = v.x;
            m[1][0] = 0.0; m[1][1] = 1.0; m[1][2] = 0.0; m[1][3] = v.y;
            m[2][0] = 0.0; m[2][1] = 0.0; m[2][2] = 1.0; m[2][3] = v.z;
            m[3][0] = 0.0; m[3][1] = 0.0; m[3][2] = 0.0; m[3][3] = 1.0;
        }

        FORCEINLINE void makeTrans( Real tx, Real ty, Real tz )
        {
            m[0][0] = 1.0; m[0][1] = 0.0; m[0][2] = 0.0; m[0][3] = tx;
            m[1][0] = 0.0; m[1][1] = 1.0; m[1][2] = 0.0; m[1][3] = ty;
            m[2][0] = 0.0; m[2][1] = 0.0; m[2][2] = 1.0; m[2][3] = tz;
            m[3][0] = 0.0; m[3][1] = 0.0; m[3][2] = 0.0; m[3][3] = 1.0;
        }

        /** Gets a translation matrix.
        */
        FORCEINLINE static Matrix4 getTrans( const Vector3& v )
        {
            Matrix4 r;

            r.m[0][0] = 1.0; r.m[0][1] = 0.0; r.m[0][2] = 0.0; r.m[0][3] = v.x;
            r.m[1][0] = 0.0; r.m[1][1] = 1.0; r.m[1][2] = 0.0; r.m[1][3] = v.y;
            r.m[2][0] = 0.0; r.m[2][1] = 0.0; r.m[2][2] = 1.0; r.m[2][3] = v.z;
            r.m[3][0] = 0.0; r.m[3][1] = 0.0; r.m[3][2] = 0.0; r.m[3][3] = 1.0;

            return r;
        }

        /** Gets a translation matrix - variation for not using a vector.
        */
        FORCEINLINE static Matrix4 getTrans( Real t_x, Real t_y, Real t_z )
        {
            Matrix4 r;

            r.m[0][0] = 1.0; r.m[0][1] = 0.0; r.m[0][2] = 0.0; r.m[0][3] = t_x;
            r.m[1][0] = 0.0; r.m[1][1] = 1.0; r.m[1][2] = 0.0; r.m[1][3] = t_y;
            r.m[2][0] = 0.0; r.m[2][1] = 0.0; r.m[2][2] = 1.0; r.m[2][3] = t_z;
            r.m[3][0] = 0.0; r.m[3][1] = 0.0; r.m[3][2] = 0.0; r.m[3][3] = 1.0;

            return r;
        }

        /*
        -----------------------------------------------------------------------
        Scale Transformation
        -----------------------------------------------------------------------
        */
        /** Sets the scale part of the matrix.
        */
        FORCEINLINE void setScale( const Vector3& v )
        {
            m[0][0] = v.x;
            m[1][1] = v.y;
            m[2][2] = v.z;
        }

        /** Gets a scale matrix.
        */
        FORCEINLINE static Matrix4 getScale( const Vector3& v )
        {
            Matrix4 r;
            r.m[0][0] = v.x; r.m[0][1] = 0.0; r.m[0][2] = 0.0; r.m[0][3] = 0.0;
            r.m[1][0] = 0.0; r.m[1][1] = v.y; r.m[1][2] = 0.0; r.m[1][3] = 0.0;
            r.m[2][0] = 0.0; r.m[2][1] = 0.0; r.m[2][2] = v.z; r.m[2][3] = 0.0;
            r.m[3][0] = 0.0; r.m[3][1] = 0.0; r.m[3][2] = 0.0; r.m[3][3] = 1.0;

            return r;
        }

        /** Gets a scale matrix - variation for not using a vector.
        */
        FORCEINLINE static Matrix4 getScale( Real s_x, Real s_y, Real s_z )
        {
            Matrix4 r;
            r.m[0][0] = s_x; r.m[0][1] = 0.0; r.m[0][2] = 0.0; r.m[0][3] = 0.0;
            r.m[1][0] = 0.0; r.m[1][1] = s_y; r.m[1][2] = 0.0; r.m[1][3] = 0.0;
            r.m[2][0] = 0.0; r.m[2][1] = 0.0; r.m[2][2] = s_z; r.m[2][3] = 0.0;
            r.m[3][0] = 0.0; r.m[3][1] = 0.0; r.m[3][2] = 0.0; r.m[3][3] = 1.0;

            return r;
        }

        /** Extracts the rotation / scaling part of the Matrix as a 3x3 matrix. 
        @param m3x3 Destination Matrix3
        */
        FORCEINLINE void extract3x3Matrix(Matrix3& m3x3) const
        {
            m3x3.m[0][0] = m[0][0];
            m3x3.m[0][1] = m[0][1];
            m3x3.m[0][2] = m[0][2];
            m3x3.m[1][0] = m[1][0];
            m3x3.m[1][1] = m[1][1];
            m3x3.m[1][2] = m[1][2];
            m3x3.m[2][0] = m[2][0];
            m3x3.m[2][1] = m[2][1];
            m3x3.m[2][2] = m[2][2];

        }

		/** Determines if this matrix involves a scaling. */
		FORCEINLINE bool hasScale() const
		{
			// check magnitude of column vectors (==local axes)
			Real t = m[0][0] * m[0][0] + m[1][0] * m[1][0] + m[2][0] * m[2][0];
			if (!Math::RealEqual(t, 1.0, (Real)1e-04))
				return true;
			t = m[0][1] * m[0][1] + m[1][1] * m[1][1] + m[2][1] * m[2][1];
			if (!Math::RealEqual(t, 1.0, (Real)1e-04))
				return true;
			t = m[0][2] * m[0][2] + m[1][2] * m[1][2] + m[2][2] * m[2][2];
			if (!Math::RealEqual(t, 1.0, (Real)1e-04))
				return true;

			return false;
		}

		/** Determines if this matrix involves a negative scaling. */
		FORCEINLINE bool hasNegativeScale() const
		{
			return determinant() < 0;
		}

        /** Extracts the rotation / scaling part as a quaternion from the Matrix.
         */
        FORCEINLINE Quaternion extractQuaternion() const
        {
          Matrix3 m3x3;
          extract3x3Matrix(m3x3);
          return Quaternion(m3x3);
        }

        static const Matrix4 ZERO;
        static const Matrix4 IDENTITY;
        /** Useful little matrix which takes 2D clipspace {-1, 1} to {0,1}
            and inverts the Y. */
        static const Matrix4 CLIPSPACE2DTOIMAGESPACE;

        FORCEINLINE Matrix4 operator*(Real scalar) const
        {
            return Matrix4(
                scalar*m[0][0], scalar*m[0][1], scalar*m[0][2], scalar*m[0][3],
                scalar*m[1][0], scalar*m[1][1], scalar*m[1][2], scalar*m[1][3],
                scalar*m[2][0], scalar*m[2][1], scalar*m[2][2], scalar*m[2][3],
                scalar*m[3][0], scalar*m[3][1], scalar*m[3][2], scalar*m[3][3]);
        }

        /** Function for writing to a stream.
        */
        FORCEINLINE _OgreExport friend std::ostream& operator <<
            ( std::ostream& o, const Matrix4& mat )
        {
            o << "Matrix4(";
			for (size_t i = 0; i < 4; ++i)
            {
                o << " row" << (unsigned)i << "{";
                for(size_t j = 0; j < 4; ++j)
                {
                    o << mat[i][j] << " ";
                }
                o << "}";
            }
            o << ")";
            return o;
        }
		
		Matrix4 adjoint() const;
		Real determinant() const;

		void inverseSelf();
		void inverse(Matrix4& result) const;

        /** Building a Matrix4 from orientation / scale / position.
        @remarks
            Transform is performed in the order scale, rotate, translation, i.e. translation is independent
            of orientation axes, scale does not affect size of translation, rotation and scaling are always
            centered on the origin.
        */
        void makeTransform(const Vector3& position, const Vector3& scale, const Quaternion& orientation);

        /** Building an inverse Matrix4 from orientation / scale / position.
        @remarks
            As makeTransform except it build the inverse given the same data as makeTransform, so
            performing -translation, -rotate, 1/scale in that order.
        */
        void makeInverseTransform(const Vector3& position, const Vector3& scale, const Quaternion& orientation);

        /** Decompose a Matrix4 to orientation / scale / position.
        */
        void decomposition(Vector3& position, Vector3& scale, Quaternion& orientation) const;

        /** Check whether or not the matrix is affine matrix.
            @remarks
                An affine matrix is a 4x4 matrix with row 3 equal to (0, 0, 0, 1),
                e.g. no projective coefficients.
        */
        FORCEINLINE bool isAffine(void) const
        {
            return m[3][0] == 0 && m[3][1] == 0 && m[3][2] == 0 && m[3][3] == 1;
        }

        /** Returns the inverse of the affine matrix.
            @note
                The matrix must be an affine matrix. @see Matrix4::isAffine.
        */
        //Matrix4 inverseAffine(void) const;
        void inverseAffine(Matrix4& result) const;

        /** Concatenate two affine matrices.
            @note
                The matrices must be affine matrix. @see Matrix4::isAffine.
        */
        //FORCEINLINE Matrix4 concatenateAffine(const Matrix4 &m2) const
        //{
        //    assert(isAffine() && m2.isAffine());

        //    return Matrix4(
        //        m[0][0] * m2.m[0][0] + m[0][1] * m2.m[1][0] + m[0][2] * m2.m[2][0],
        //        m[0][0] * m2.m[0][1] + m[0][1] * m2.m[1][1] + m[0][2] * m2.m[2][1],
        //        m[0][0] * m2.m[0][2] + m[0][1] * m2.m[1][2] + m[0][2] * m2.m[2][2],
        //        m[0][0] * m2.m[0][3] + m[0][1] * m2.m[1][3] + m[0][2] * m2.m[2][3] + m[0][3],

        //        m[1][0] * m2.m[0][0] + m[1][1] * m2.m[1][0] + m[1][2] * m2.m[2][0],
        //        m[1][0] * m2.m[0][1] + m[1][1] * m2.m[1][1] + m[1][2] * m2.m[2][1],
        //        m[1][0] * m2.m[0][2] + m[1][1] * m2.m[1][2] + m[1][2] * m2.m[2][2],
        //        m[1][0] * m2.m[0][3] + m[1][1] * m2.m[1][3] + m[1][2] * m2.m[2][3] + m[1][3],

        //        m[2][0] * m2.m[0][0] + m[2][1] * m2.m[1][0] + m[2][2] * m2.m[2][0],
        //        m[2][0] * m2.m[0][1] + m[2][1] * m2.m[1][1] + m[2][2] * m2.m[2][1],
        //        m[2][0] * m2.m[0][2] + m[2][1] * m2.m[1][2] + m[2][2] * m2.m[2][2],
        //        m[2][0] * m2.m[0][3] + m[2][1] * m2.m[1][3] + m[2][2] * m2.m[2][3] + m[2][3],

        //        0, 0, 0, 1);
        //}

        FORCEINLINE static void concatenateAffine(const Matrix4& m1, const Matrix4& m2, Matrix4& result)
		{
            result.m[0][0] = m1.m[0][0] * m2.m[0][0] + m1.m[0][1] * m2.m[1][0] + m1.m[0][2] * m2.m[2][0];
            result.m[0][1] = m1.m[0][0] * m2.m[0][1] + m1.m[0][1] * m2.m[1][1] + m1.m[0][2] * m2.m[2][1];
            result.m[0][2] = m1.m[0][0] * m2.m[0][2] + m1.m[0][1] * m2.m[1][2] + m1.m[0][2] * m2.m[2][2];
            result.m[0][3] = m1.m[0][0] * m2.m[0][3] + m1.m[0][1] * m2.m[1][3] + m1.m[0][2] * m2.m[2][3] + m1.m[0][3];

            result.m[1][0] = m1.m[1][0] * m2.m[0][0] + m1.m[1][1] * m2.m[1][0] + m1.m[1][2] * m2.m[2][0];
            result.m[1][1] = m1.m[1][0] * m2.m[0][1] + m1.m[1][1] * m2.m[1][1] + m1.m[1][2] * m2.m[2][1];
            result.m[1][2] = m1.m[1][0] * m2.m[0][2] + m1.m[1][1] * m2.m[1][2] + m1.m[1][2] * m2.m[2][2];
            result.m[1][3] = m1.m[1][0] * m2.m[0][3] + m1.m[1][1] * m2.m[1][3] + m1.m[1][2] * m2.m[2][3] + m1.m[1][3];

            result.m[2][0] = m1.m[2][0] * m2.m[0][0] + m1.m[2][1] * m2.m[1][0] + m1.m[2][2] * m2.m[2][0];
            result.m[2][1] = m1.m[2][0] * m2.m[0][1] + m1.m[2][1] * m2.m[1][1] + m1.m[2][2] * m2.m[2][1];
            result.m[2][2] = m1.m[2][0] * m2.m[0][2] + m1.m[2][1] * m2.m[1][2] + m1.m[2][2] * m2.m[2][2];
            result.m[2][3] = m1.m[2][0] * m2.m[0][3] + m1.m[2][1] * m2.m[1][3] + m1.m[2][2] * m2.m[2][3] + m1.m[2][3];

            result.m[3][0] = 0;
            result.m[3][1] = 0;
            result.m[3][2] = 0;
            result.m[3][3] = 1;
		}

        /** 3-D Vector transformation specially for an affine matrix.
            @remarks
                Transforms the given 3-D vector by the matrix, projecting the 
                result back into <i>w</i> = 1.
            @note
                The matrix must be an affine matrix. @see Matrix4::isAffine.
        */
        FORCEINLINE Vector3 transformAffine(const Vector3& v) const
        {
            assert(isAffine());

            return Vector3(
                    m[0][0] * v.x + m[0][1] * v.y + m[0][2] * v.z + m[0][3], 
                    m[1][0] * v.x + m[1][1] * v.y + m[1][2] * v.z + m[1][3],
                    m[2][0] * v.x + m[2][1] * v.y + m[2][2] * v.z + m[2][3]);
        }

        FORCEINLINE void transformAffine(const Vector3& v, Vector3& result) const
        {
            assert(isAffine());

			result.x = m[0][0] * v.x + m[0][1] * v.y + m[0][2] * v.z + m[0][3];
			result.y = m[1][0] * v.x + m[1][1] * v.y + m[1][2] * v.z + m[1][3];
			result.z = m[2][0] * v.x + m[2][1] * v.y + m[2][2] * v.z + m[2][3];
        }

        /** 4-D Vector transformation specially for an affine matrix.
            @note
                The matrix must be an affine matrix. @see Matrix4::isAffine.
        */
        FORCEINLINE Vector4 transformAffine(const Vector4& v) const
        {
            assert(isAffine());

            return Vector4(
                m[0][0] * v.x + m[0][1] * v.y + m[0][2] * v.z + m[0][3] * v.w, 
                m[1][0] * v.x + m[1][1] * v.y + m[1][2] * v.z + m[1][3] * v.w,
                m[2][0] * v.x + m[2][1] * v.y + m[2][2] * v.z + m[2][3] * v.w,
                v.w);
        }

        FORCEINLINE void transformAffine(const Vector4& v, Vector4& result) const
        {
            assert(isAffine());

            result.x = m[0][0] * v.x + m[0][1] * v.y + m[0][2] * v.z + m[0][3] * v.w;
			result.y = m[1][0] * v.x + m[1][1] * v.y + m[1][2] * v.z + m[1][3] * v.w;
            result.z = m[2][0] * v.x + m[2][1] * v.y + m[2][2] * v.z + m[2][3] * v.w;
            result.w = v.w;
        }

        FORCEINLINE void transformAffine(const Vector3& v, Vector4& result) const
        {
            assert(isAffine());

            result.x = m[0][0] * v.x + m[0][1] * v.y + m[0][2] * v.z + m[0][3];
			result.y = m[1][0] * v.x + m[1][1] * v.y + m[1][2] * v.z + m[1][3];
            result.z = m[2][0] * v.x + m[2][1] * v.y + m[2][2] * v.z + m[2][3];
            result.w = 1;
        }


    };

    /* Removed from Vector4 and made a non-member here because otherwise
       OgreMatrix4.h and OgreVector4.h have to try to include and inline each 
       other, which frankly doesn't work ;)
   */
    FORCEINLINE Vector4 operator * (const Vector4& v, const Matrix4& mat)
    {
        return Vector4(
            v.x*mat[0][0] + v.y*mat[1][0] + v.z*mat[2][0] + v.w*mat[3][0],
            v.x*mat[0][1] + v.y*mat[1][1] + v.z*mat[2][1] + v.w*mat[3][1],
            v.x*mat[0][2] + v.y*mat[1][2] + v.z*mat[2][2] + v.w*mat[3][2],
            v.x*mat[0][3] + v.y*mat[1][3] + v.z*mat[2][3] + v.w*mat[3][3]
            );
    }
	/** @} */
	/** @} */

}
#endif
