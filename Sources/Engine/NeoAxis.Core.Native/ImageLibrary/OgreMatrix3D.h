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
#ifndef __Matrix3D_H__
#define __Matrix3D_H__

#include "OgrePrerequisites.h"

#include "OgreVector3D.h"

// NB All code adapted from Wild Magic 0.2 Matrix math (free source code)
// http://www.geometrictools.com/

// NOTE.  The (x,y,z) coordinate system is assumed to be right-handed.
// Coordinate axis rotation matrices are of the form
//   RX =    1       0       0
//           0     cos(t) -sin(t)
//           0     sin(t)  cos(t)
// where t > 0 indicates a counterclockwise rotation in the yz-plane
//   RY =  cos(t)    0     sin(t)
//           0       1       0
//        -sin(t)    0     cos(t)
// where t > 0 indicates a counterclockwise rotation in the zx-plane
//   RZ =  cos(t) -sin(t)    0
//         sin(t)  cos(t)    0
//           0       0       1
// where t > 0 indicates a counterclockwise rotation in the xy-plane.

namespace Ogre
{
	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup Math
	*  @{
	*/
    /** A 3x3 matrix which can represent rotations around axes.
        @note
            <b>All the code is adapted from the Wild Magic 0.2 Matrix
            library (http://www.geometrictools.com/).</b>
        @par
            The coordinate system is assumed to be <b>right-handed</b>.
    */
    class _OgreExport Matrix3D
    {
    public:
        /** Default constructor.
            @note
                It does <b>NOT</b> initialize the matrix for efficiency.
        */
		FORCEINLINE Matrix3D () {}
        FORCEINLINE explicit Matrix3D (const double arr[3][3])
		{
			memcpy(m,arr,9*sizeof(double));
		}
        FORCEINLINE Matrix3D (const Matrix3D& rkMatrix)
		{
			memcpy(m,rkMatrix.m,9*sizeof(double));
		}
        Matrix3D (double fEntry00, double fEntry01, double fEntry02,
                    double fEntry10, double fEntry11, double fEntry12,
                    double fEntry20, double fEntry21, double fEntry22)
		{
			m[0][0] = fEntry00;
			m[0][1] = fEntry01;
			m[0][2] = fEntry02;
			m[1][0] = fEntry10;
			m[1][1] = fEntry11;
			m[1][2] = fEntry12;
			m[2][0] = fEntry20;
			m[2][1] = fEntry21;
			m[2][2] = fEntry22;
		}

		/** Exchange the contents of this matrix with another. 
		*/
		FORCEINLINE void swap(Matrix3D& other)
		{
			std::swap(m[0][0], other.m[0][0]);
			std::swap(m[0][1], other.m[0][1]);
			std::swap(m[0][2], other.m[0][2]);
			std::swap(m[1][0], other.m[1][0]);
			std::swap(m[1][1], other.m[1][1]);
			std::swap(m[1][2], other.m[1][2]);
			std::swap(m[2][0], other.m[2][0]);
			std::swap(m[2][1], other.m[2][1]);
			std::swap(m[2][2], other.m[2][2]);
		}

        // member access, allows use of construct mat[r][c]
        FORCEINLINE double* operator[] (size_t iRow) const
		{
			return (double*)m[iRow];
		}
        /*FORCEINLINE operator double* ()
		{
			return (double*)m[0];
		}*/
        Vector3D GetColumn (size_t iCol) const;
        void SetColumn(size_t iCol, const Vector3D& vec);
        void FromAxes(const Vector3D& xAxis, const Vector3D& yAxis, const Vector3D& zAxis);

        // assignment and comparison
        FORCEINLINE Matrix3D& operator= (const Matrix3D& rkMatrix)
		{
			memcpy(m,rkMatrix.m,9*sizeof(double));
			return *this;
		}
        bool operator== (const Matrix3D& rkMatrix) const;
        FORCEINLINE bool operator!= (const Matrix3D& rkMatrix) const
		{
			return !operator==(rkMatrix);
		}

        // arithmetic operations
        Matrix3D operator+ (const Matrix3D& rkMatrix) const;
        Matrix3D operator- (const Matrix3D& rkMatrix) const;
        Matrix3D operator* (const Matrix3D& rkMatrix) const;
        Matrix3D operator- () const;

        // matrix * vector [3x3 * 3x1 = 3x1]
        Vector3D operator* (const Vector3D& rkVector) const;

        // vector * matrix [1x3 * 3x3 = 1x3]
        _OgreExport friend Vector3D operator* (const Vector3D& rkVector,
            const Matrix3D& rkMatrix);

        // matrix * scalar
        Matrix3D operator* (double fScalar) const;

        // scalar * matrix
        _OgreExport friend Matrix3D operator* (double fScalar, const Matrix3D& rkMatrix);

        // utilities
        Matrix3D Transpose () const;

		FORCEINLINE void transposeSelf()
		{
			double v;
			v = m[2][1]; m[2][1] = m[1][2]; m[1][2] = v;
			v = m[0][1]; m[0][1] = m[1][0]; m[1][0] = v;
			v = m[0][2]; m[0][2] = m[2][0]; m[2][0] = v;
		}

        bool Inverse (Matrix3D& rkInverse, double fTolerance = 1e-06) const;
        Matrix3D Inverse (double fTolerance = 1e-06) const;
        double Determinant () const;

        // singular value decomposition
        void SingularValueDecomposition (Matrix3D& rkL, Vector3D& rkS,
            Matrix3D& rkR) const;
        void SingularValueComposition (const Matrix3D& rkL,
            const Vector3D& rkS, const Matrix3D& rkR);

        // Gram-Schmidt orthonormalization (applied to columns of rotation matrix)
        void Orthonormalize ();

        // orthogonal Q, diagonal D, upper triangular U stored as (u01,u02,u12)
        void QDUDecomposition (Matrix3D& rkQ, Vector3D& rkD,
            Vector3D& rkU) const;

        double SpectralNorm () const;

        // matrix must be orthonormal
        void ToAxisAngle (Vector3D& rkAxis, Radian& rfAngle) const;
		FORCEINLINE void ToAxisAngle (Vector3D& rkAxis, Degree& rfAngle) const {
			Radian r;
			ToAxisAngle ( rkAxis, r );
			rfAngle = r;
		}
        void FromAxisAngle (const Vector3D& rkAxis, const Radian& fRadians);

        // The matrix must be orthonormal.  The decomposition is yaw*pitch*roll
        // where yaw is rotation about the Up vector, pitch is rotation about the
        // Right axis, and roll is rotation about the Direction axis.
        bool ToEulerAnglesXYZ (Radian& rfYAngle, Radian& rfPAngle,
            Radian& rfRAngle) const;
        bool ToEulerAnglesXZY (Radian& rfYAngle, Radian& rfPAngle,
            Radian& rfRAngle) const;
        bool ToEulerAnglesYXZ (Radian& rfYAngle, Radian& rfPAngle,
            Radian& rfRAngle) const;
        bool ToEulerAnglesYZX (Radian& rfYAngle, Radian& rfPAngle,
            Radian& rfRAngle) const;
        bool ToEulerAnglesZXY (Radian& rfYAngle, Radian& rfPAngle,
            Radian& rfRAngle) const;
        bool ToEulerAnglesZYX (Radian& rfYAngle, Radian& rfPAngle,
            Radian& rfRAngle) const;
        void FromEulerAnglesXYZ (const Radian& fYAngle, const Radian& fPAngle, const Radian& fRAngle);
        void FromEulerAnglesXZY (const Radian& fYAngle, const Radian& fPAngle, const Radian& fRAngle);
        void FromEulerAnglesYXZ (const Radian& fYAngle, const Radian& fPAngle, const Radian& fRAngle);
        void FromEulerAnglesYZX (const Radian& fYAngle, const Radian& fPAngle, const Radian& fRAngle);
        void FromEulerAnglesZXY (const Radian& fYAngle, const Radian& fPAngle, const Radian& fRAngle);
        void FromEulerAnglesZYX (const Radian& fYAngle, const Radian& fPAngle, const Radian& fRAngle);
        // eigensolver, matrix must be symmetric
        void EigenSolveSymmetric (double afEigenvalue[3],
            Vector3D akEigenvector[3]) const;

        static void TensorProduct (const Vector3D& rkU, const Vector3D& rkV,
            Matrix3D& rkProduct);

		///** Determines if this matrix involves a scaling. */
		//FORCEINLINE bool hasScale() const
		//{
		//	// check magnitude of column vectors (==local axes)
		//	double t = m[0][0] * m[0][0] + m[1][0] * m[1][0] + m[2][0] * m[2][0];
		//	if (!Math::RealEqual(t, 1.0, (double)1e-04))
		//		return true;
		//	t = m[0][1] * m[0][1] + m[1][1] * m[1][1] + m[2][1] * m[2][1];
		//	if (!Math::RealEqual(t, 1.0, (double)1e-04))
		//		return true;
		//	t = m[0][2] * m[0][2] + m[1][2] * m[1][2] + m[2][2] * m[2][2];
		//	if (!Math::RealEqual(t, 1.0, (double)1e-04))
		//		return true;

		//	return false;
		//}


        static const double EPSILON;
        static const Matrix3D ZERO;
        static const Matrix3D IDENTITY;

    protected:
        // support for eigensolver
        void Tridiagonal (double afDiag[3], double afSubDiag[3]);
        bool QLAlgorithm (double afDiag[3], double afSubDiag[3]);

        // support for singular value decomposition
        static const double ms_fSvdEpsilon;
        static const unsigned int ms_iSvdMaxIterations;
        static void Bidiagonalize (Matrix3D& kA, Matrix3D& kL,
            Matrix3D& kR);
        static void GolubKahanStep (Matrix3D& kA, Matrix3D& kL,
            Matrix3D& kR);

        // support for spectral norm
        static double MaxCubicRoot (double afCoeff[3]);

        double m[3][3];

        // for faster access
        friend class Matrix4;
    };

	/** @} */
	/** @} */
}
#endif
