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
#include "OgreMatrix4.h"
#include "OgreVector3.h"
#include "OgreMatrix3.h"
#include "OgreOptimisedUtil.h"

//#include "OgrePlatformInformation.h"
//#if __OGRE_HAVE_SSE
//#include <xmmintrin.h>
//#endif


namespace Ogre
{

    const Matrix4 Matrix4::ZERO(
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0 );

    const Matrix4 Matrix4::IDENTITY(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1 );

    const Matrix4 Matrix4::CLIPSPACE2DTOIMAGESPACE(
        0.5,    0,  0, 0.5, 
          0, -0.5,  0, 0.5, 
          0,    0,  1,   0,
          0,    0,  0,   1);

    //-----------------------------------------------------------------------
    inline static Real
        MINOR(const Matrix4& m, const size_t r0, const size_t r1, const size_t r2, 
								const size_t c0, const size_t c1, const size_t c2)
    {
        return m[r0][c0] * (m[r1][c1] * m[r2][c2] - m[r2][c1] * m[r1][c2]) -
            m[r0][c1] * (m[r1][c0] * m[r2][c2] - m[r2][c0] * m[r1][c2]) +
            m[r0][c2] * (m[r1][c0] * m[r2][c1] - m[r2][c0] * m[r1][c1]);
    }
    //-----------------------------------------------------------------------
    Matrix4 Matrix4::adjoint() const
    {
        return Matrix4( MINOR(*this, 1, 2, 3, 1, 2, 3),
            -MINOR(*this, 0, 2, 3, 1, 2, 3),
            MINOR(*this, 0, 1, 3, 1, 2, 3),
            -MINOR(*this, 0, 1, 2, 1, 2, 3),

            -MINOR(*this, 1, 2, 3, 0, 2, 3),
            MINOR(*this, 0, 2, 3, 0, 2, 3),
            -MINOR(*this, 0, 1, 3, 0, 2, 3),
            MINOR(*this, 0, 1, 2, 0, 2, 3),

            MINOR(*this, 1, 2, 3, 0, 1, 3),
            -MINOR(*this, 0, 2, 3, 0, 1, 3),
            MINOR(*this, 0, 1, 3, 0, 1, 3),
            -MINOR(*this, 0, 1, 2, 0, 1, 3),

            -MINOR(*this, 1, 2, 3, 0, 1, 2),
            MINOR(*this, 0, 2, 3, 0, 1, 2),
            -MINOR(*this, 0, 1, 3, 0, 1, 2),
            MINOR(*this, 0, 1, 2, 0, 1, 2));
    }
    //-----------------------------------------------------------------------
    Real Matrix4::determinant() const
    {
        return m[0][0] * MINOR(*this, 1, 2, 3, 1, 2, 3) -
            m[0][1] * MINOR(*this, 1, 2, 3, 0, 2, 3) +
            m[0][2] * MINOR(*this, 1, 2, 3, 0, 1, 3) -
            m[0][3] * MINOR(*this, 1, 2, 3, 0, 1, 2);
    }
    //-----------------------------------------------------------------------
	void Matrix4::inverseSelf()
	{
		OptimisedUtil::getImplementation()->matrixInverse(*this, *this);
	}
    //-----------------------------------------------------------------------
    void Matrix4::inverse(Matrix4& result) const
    {
		OptimisedUtil::getImplementation()->matrixInverse(*this, result);
    }
    //-----------------------------------------------------------------------
    //Matrix4 Matrix4::inverseAffine(void) const
    //{
    //    assert(isAffine());

    //    Real m10 = m[1][0], m11 = m[1][1], m12 = m[1][2];
    //    Real m20 = m[2][0], m21 = m[2][1], m22 = m[2][2];

    //    Real t00 = m22 * m11 - m21 * m12;
    //    Real t10 = m20 * m12 - m22 * m10;
    //    Real t20 = m21 * m10 - m20 * m11;

    //    Real m00 = m[0][0], m01 = m[0][1], m02 = m[0][2];

    //    Real invDet = 1 / (m00 * t00 + m01 * t10 + m02 * t20);

    //    t00 *= invDet; t10 *= invDet; t20 *= invDet;

    //    m00 *= invDet; m01 *= invDet; m02 *= invDet;

    //    Real r00 = t00;
    //    Real r01 = m02 * m21 - m01 * m22;
    //    Real r02 = m01 * m12 - m02 * m11;

    //    Real r10 = t10;
    //    Real r11 = m00 * m22 - m02 * m20;
    //    Real r12 = m02 * m10 - m00 * m12;

    //    Real r20 = t20;
    //    Real r21 = m01 * m20 - m00 * m21;
    //    Real r22 = m00 * m11 - m01 * m10;

    //    Real m03 = m[0][3], m13 = m[1][3], m23 = m[2][3];

    //    Real r03 = - (r00 * m03 + r01 * m13 + r02 * m23);
    //    Real r13 = - (r10 * m03 + r11 * m13 + r12 * m23);
    //    Real r23 = - (r20 * m03 + r21 * m13 + r22 * m23);

    //    return Matrix4(
    //        r00, r01, r02, r03,
    //        r10, r11, r12, r13,
    //        r20, r21, r22, r23,
    //          0,   0,   0,   1);
    //}
    //-----------------------------------------------------------------------
    void Matrix4::inverseAffine(Matrix4& result) const
    {
		//!!!!!!need SSE

        assert(isAffine());

        Real m10 = m[1][0], m11 = m[1][1], m12 = m[1][2];
        Real m20 = m[2][0], m21 = m[2][1], m22 = m[2][2];

        Real t00 = m22 * m11 - m21 * m12;
        Real t10 = m20 * m12 - m22 * m10;
        Real t20 = m21 * m10 - m20 * m11;

        Real m00 = m[0][0], m01 = m[0][1], m02 = m[0][2];

        Real invDet = 1 / (m00 * t00 + m01 * t10 + m02 * t20);

        t00 *= invDet; t10 *= invDet; t20 *= invDet;

        m00 *= invDet; m01 *= invDet; m02 *= invDet;

        Real r00 = t00;
        Real r01 = m02 * m21 - m01 * m22;
        Real r02 = m01 * m12 - m02 * m11;

        Real r10 = t10;
        Real r11 = m00 * m22 - m02 * m20;
        Real r12 = m02 * m10 - m00 * m12;

        Real r20 = t20;
        Real r21 = m01 * m20 - m00 * m21;
        Real r22 = m00 * m11 - m01 * m10;

        Real m03 = m[0][3], m13 = m[1][3], m23 = m[2][3];

        Real r03 = - (r00 * m03 + r01 * m13 + r02 * m23);
        Real r13 = - (r10 * m03 + r11 * m13 + r12 * m23);
        Real r23 = - (r20 * m03 + r21 * m13 + r22 * m23);

		result.m[0][0] = r00;
		result.m[0][1] = r01;
		result.m[0][2] = r02;
		result.m[0][3] = r03;
		result.m[1][0] = r10;
		result.m[1][1] = r11;
		result.m[1][2] = r12;
		result.m[1][3] = r13;
		result.m[2][0] = r20;
		result.m[2][1] = r21;
		result.m[2][2] = r22;
		result.m[2][3] = r23;
		result.m[3][0] = 0;
		result.m[3][1] = 0;
		result.m[3][2] = 0;
		result.m[3][3] = 1;
        //result = Matrix4(
        //    r00, r01, r02, r03,
        //    r10, r11, r12, r13,
        //    r20, r21, r22, r23,
        //      0,   0,   0,   1);
    }
    //-----------------------------------------------------------------------
    void Matrix4::makeTransform(const Vector3& position, const Vector3& scale, const Quaternion& orientation)
    {
        // Ordering:
        //    1. Scale
        //    2. Rotate
        //    3. Translate

        Matrix3 rot3x3;
        orientation.ToRotationMatrix(rot3x3);

        // Set up final matrix with scale, rotation and translation
        m[0][0] = scale.x * rot3x3[0][0]; m[0][1] = scale.y * rot3x3[0][1]; m[0][2] = scale.z * rot3x3[0][2]; m[0][3] = position.x;
        m[1][0] = scale.x * rot3x3[1][0]; m[1][1] = scale.y * rot3x3[1][1]; m[1][2] = scale.z * rot3x3[1][2]; m[1][3] = position.y;
        m[2][0] = scale.x * rot3x3[2][0]; m[2][1] = scale.y * rot3x3[2][1]; m[2][2] = scale.z * rot3x3[2][2]; m[2][3] = position.z;

        // No projection term
        m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;
    }
    //-----------------------------------------------------------------------
    void Matrix4::makeInverseTransform(const Vector3& position, const Vector3& scale, const Quaternion& orientation)
    {
        // Invert the parameters
        Vector3 invTranslate = -position;
        Vector3 invScale(1 / scale.x, 1 / scale.y, 1 / scale.z);
        Quaternion invRot = orientation.Inverse();

        // Because we're inverting, order is translation, rotation, scale
        // So make translation relative to scale & rotation
        invTranslate = invRot * invTranslate; // rotate
        invTranslate *= invScale; // scale

        // Next, make a 3x3 rotation matrix
        Matrix3 rot3x3;
        invRot.ToRotationMatrix(rot3x3);

        // Set up final matrix with scale, rotation and translation
        m[0][0] = invScale.x * rot3x3[0][0]; m[0][1] = invScale.x * rot3x3[0][1]; m[0][2] = invScale.x * rot3x3[0][2]; m[0][3] = invTranslate.x;
        m[1][0] = invScale.y * rot3x3[1][0]; m[1][1] = invScale.y * rot3x3[1][1]; m[1][2] = invScale.y * rot3x3[1][2]; m[1][3] = invTranslate.y;
        m[2][0] = invScale.z * rot3x3[2][0]; m[2][1] = invScale.z * rot3x3[2][1]; m[2][2] = invScale.z * rot3x3[2][2]; m[2][3] = invTranslate.z;		

        // No projection term
        m[3][0] = 0; m[3][1] = 0; m[3][2] = 0; m[3][3] = 1;
    }
    //-----------------------------------------------------------------------
	void Matrix4::decomposition(Vector3& position, Vector3& scale, Quaternion& orientation) const
	{
		assert(isAffine());

		Matrix3 m3x3;
		extract3x3Matrix(m3x3);

		Matrix3 matQ;
		Vector3 vecU;
		m3x3.QDUDecomposition( matQ, scale, vecU ); 

		orientation = Quaternion( matQ );
		position = Vector3( m[0][3], m[1][3], m[2][3] );
	}

}
