/*
* Copyright (c) 2012-2020 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Internal.Assimp
{
    /// <summary>
    /// Represents a 3x3 matrix. Assimp docs say their matrices are always row-major,
    /// and it looks like they're only describing the memory layout. Matrices are treated
    /// as column vectors however (X base in the first column, Y base the second, and Z base the third)
    /// </summary>
    /// <param name="A1">Value at row 1, column 1 of the matrix.</param>
    /// <param name="A2">Value at row 1, column 2 of the matrix.</param>
    /// <param name="A3">Value at row 1, column 3 of the matrix.</param>
    /// <param name="B1">Value at row 2, column 1 of the matrix.</param>
    /// <param name="B2">Value at row 2, column 2 of the matrix.</param>
    /// <param name="B3">Value at row 2, column 3 of the matrix.</param>
    /// <param name="C1">Value at row 3, column 1 of the matrix.</param>
    /// <param name="C2">Value at row 3, column 2 of the matrix.</param>
    /// <param name="C3">Value at row 3, column 3 of the matrix.</param>
    [StructLayout(LayoutKind.Sequential)]
    public record struct Matrix3x3(float A1, float A2, float A3, float B1, float B2, float B3, float C1, float C2, float C3)
    {
        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        public static Matrix3x3 Identity { get; } = new(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets if this matrix is an identity matrix.
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                float epsilon = 10e-3f;

                return (A2 <= epsilon && A2 >= -epsilon &&
                A3 <= epsilon && A3 >= -epsilon &&
                B1 <= epsilon && B1 >= -epsilon &&
                B3 <= epsilon && B3 >= -epsilon &&
                C1 <= epsilon && C1 >= -epsilon &&
                C2 <= epsilon && C2 >= -epsilon &&
                A1 <= 1.0f + epsilon && A1 >= 1.0f - epsilon &&
                B2 <= 1.0f + epsilon && B2 >= 1.0f - epsilon &&
                C3 <= 1.0f + epsilon && C3 >= 1.0f - epsilon);
            }
        }

        /// <summary>
        /// Gets or sets the value at the specific one-based row, column
        /// index. E.g. i = 1, j = 2 gets the value in row 1, column 2 (MA2). Indices
        /// out of range return a value of zero.
        /// 
        /// </summary>
        /// <param name="i">One-based Row index</param>
        /// <param name="j">One-based Column index</param>
        /// <returns>Matrix value</returns>
        public float this[int i, int j]
        {
            get
            {
                switch(i)
                {
                    case 1:
                        switch(j)
                        {
                            case 1:
                                return A1;
                            case 2:
                                return A2;
                            case 3:
                                return A3;
                            default:
                                return 0;
                        }
                    case 2:
                        switch(j)
                        {
                            case 1:
                                return B1;
                            case 2:
                                return B2;
                            case 3:
                                return B3;
                            default:
                                return 0;
                        }
                    case 3:
                        switch(j)
                        {
                            case 1:
                                return C1;
                            case 2:
                                return C2;
                            case 3:
                                return C3;
                            default:
                                return 0;
                        }
                    default:
                        return 0;
                }
            }
            set
            {
                switch(i)
                {
                    case 1:
                        switch(j)
                        {
                            case 1:
                                A1 = value;
                                break;
                            case 2:
                                A2 = value;
                                break;
                            case 3:
                                A3 = value;
                                break;
                        }
                        break;
                    case 2:
                        switch(j)
                        {
                            case 1:
                                B1 = value;
                                break;
                            case 2:
                                B2 = value;
                                break;
                            case 3:
                                B3 = value;
                                break;
                        }
                        break;
                    case 3:
                        switch(j)
                        {
                            case 1:
                                C1 = value;
                                break;
                            case 2:
                                C2 = value;
                                break;
                            case 3:
                                C3 = value;
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Constructs a new Matrix3x3.
        /// </summary>
        /// <param name="rotMatrix">A 4x4 matrix to construct from, only taking the rotation/scaling part.</param>
        public Matrix3x3(Matrix4x4 rotMatrix) : this(
            rotMatrix.M11, rotMatrix.M12, rotMatrix.M13,
            rotMatrix.M21, rotMatrix.M22, rotMatrix.M23,
            rotMatrix.M31, rotMatrix.M32, rotMatrix.M33)
        { }

        /// <summary>
        /// Transposes this matrix (rows become columns, vice versa).
        /// </summary>
        public void Transpose()
        {
            Matrix3x3 m = new Matrix3x3(this);

            A2 = m.B1;
            A3 = m.C1;

            B1 = m.A2;
            B3 = m.C2;

            C1 = m.A3;
            C2 = m.B3;
        }

        /// <summary>
        /// Inverts the matrix. If the matrix is *not* invertible all elements are set to <see cref="float.NaN"/>.
        /// </summary>
        public void Inverse()
        {
            float det = Determinant();
            if(det == 0.0f)
            {
                // Matrix is not invertible. Setting all elements to NaN is not really
                // correct in a mathematical sense, but it is easy to debug for the
                // programmer.
                A1 = float.NaN;
                A2 = float.NaN;
                A3 = float.NaN;

                B1 = float.NaN;
                B2 = float.NaN;
                B3 = float.NaN;

                C1 = float.NaN;
                C2 = float.NaN;
                C3 = float.NaN;
            }

            float invDet = 1.0f / det;

            float a1 = invDet * (B2 * C3 - B3 * C2);
            float a2 = -invDet * (A2 * C3 - A3 * C2);
            float a3 = invDet * (A2 * B3 - A3 * B2);

            float b1 = -invDet * (B1 * C3 - B3 * C1);
            float b2 = invDet * (A1 * C3 - A3 * C1);
            float b3 = -invDet * (A1 * B3 - A3 * B1);

            float c1 = invDet * (B1 * C2 - B2 * C1);
            float c2 = -invDet * (A1 * C2 - A2 * C1);
            float c3 = invDet * (A1 * B2 - A2 * B1);

            A1 = a1;
            A2 = a2;
            A3 = a3;

            B1 = b1;
            B2 = b2;
            B3 = b3;

            C1 = c1;
            C2 = c2;
            C3 = c3;
        }

        /// <summary>
        /// Compute the determinant of this matrix.
        /// </summary>
        /// <returns>The determinant</returns>
        public float Determinant() => A1 * B2 * C3 - A1 * B3 * C2 + A2 * B3 * C1 - A2 * B1 * C3 + A3 * B1 * C2 - A3 * B2 * C1;

        /// <summary>
        /// Creates a rotation matrix from a set of euler angles.
        /// </summary>
        /// <param name="x">Rotation angle about the x-axis, in radians.</param>
        /// <param name="y">Rotation angle about the y-axis, in radians.</param>
        /// <param name="z">Rotation angle about the z-axis, in radians.</param>
        /// <returns>The rotation matrix</returns>
        public static Matrix3x3 FromEulerAnglesXYZ(float x, float y, float z)
        {
            float cr = (float) Math.Cos(x);
            float sr = (float) Math.Sin(x);
            float cp = (float) Math.Cos(y);
            float sp = (float) Math.Sin(y);
            float cy = (float) Math.Cos(z);
            float sy = (float) Math.Sin(z);

            float srsp = sr * sp;
            float crsp = cr * sp;

            Matrix3x3 m = default;
            m.A1 = cp * cy;
            m.A2 = cp * sy;
            m.A3 = -sp;

            m.B1 = srsp * cy - cr * sy;
            m.B2 = srsp * sy + cr * cy;
            m.B3 = sr * cp;

            m.C1 = crsp * cy + sr * sy;
            m.C2 = crsp * sy - sr * cy;
            m.C3 = cr * cp;

            return m;
        }

        /// <summary>
        /// Creates a rotation matrix from a set of euler angles.
        /// </summary>
        /// <param name="angles">Vector containing the rotation angles about the x, y, z axes, in radians.</param>
        /// <returns>The rotation matrix</returns>
        public static Matrix3x3 FromEulerAnglesXYZ(Vector3 angles) => FromEulerAnglesXYZ(angles.X, angles.Y, angles.Z);

        /// <summary>
        /// Creates a rotation matrix for a rotation about the x-axis.
        /// </summary>
        /// <param name="radians">Rotation angle in radians.</param>
        /// <returns>The rotation matrix</returns>
        public static Matrix3x3 FromRotationX(float radians)
        {
            /*
                 |  1  0       0      |
             M = |  0  cos(A) -sin(A) |
                 |  0  sin(A)  cos(A) |	
            */
            Matrix3x3 m = Identity;
            m.B2 = m.C3 = (float) Math.Cos(radians);
            m.C2 = (float) Math.Sin(radians);
            m.B3 = -m.C2;
            return m;
        }

        /// <summary>
        /// Creates a rotation matrix for a rotation about the y-axis.
        /// </summary>
        /// <param name="radians">Rotation angle in radians.</param>
        /// <returns>The rotation matrix</returns>
        public static Matrix3x3 FromRotationY(float radians)
        {
            /*
                 |  cos(A)  0   sin(A) |
             M = |  0       1   0      |
                 | -sin(A)  0   cos(A) |
            */
            Matrix3x3 m = Identity;
            m.A1 = m.C3 = (float) Math.Cos(radians);
            m.A3 = (float) Math.Sin(radians);
            m.C1 = -m.A3;
            return m;
        }

        /// <summary>
        /// Creates a rotation matrix for a rotation about the z-axis.
        /// </summary>
        /// <param name="radians">Rotation angle in radians.</param>
        /// <returns>The rotation matrix</returns>
        public static Matrix3x3 FromRotationZ(float radians)
        {
            /*
                 |  cos(A)  -sin(A)   0 |
             M = |  sin(A)   cos(A)   0 |
                 |  0        0        1 |
             */
            Matrix3x3 m = Identity;
            m.A1 = m.B2 = (float) Math.Cos(radians);
            m.B1 = (float) Math.Sin(radians);
            m.A2 = -m.B1;
            return m;
        }

        /// <summary>
        /// Creates a rotation matrix for a rotation about an arbitrary axis.
        /// </summary>
        /// <param name="radians">Rotation angle, in radians</param>
        /// <param name="axis">Rotation axis, which should be a normalized vector.</param>
        /// <returns>The rotation matrix</returns>
        public static Matrix3x3 FromAngleAxis(float radians, Vector3 axis)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;

            float sin = (float) System.Math.Sin((double) radians);
            float cos = (float) System.Math.Cos((double) radians);

            float xx = x * x;
            float yy = y * y;
            float zz = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;

            Matrix3x3 m = default;
            m.A1 = xx + (cos * (1.0f - xx));
            m.B1 = (xy - (cos * xy)) + (sin * z);
            m.C1 = (xz - (cos * xz)) - (sin * y);

            m.A2 = (xy - (cos * xy)) - (sin * z);
            m.B2 = yy + (cos * (1.0f - yy));
            m.C2 = (yz - (cos * yz)) + (sin * x);

            m.A3 = (xz - (cos * xz)) + (sin * y);
            m.B3 = (yz - (cos * yz)) - (sin * x);
            m.C3 = zz + (cos * (1.0f - zz));

            return m;
        }

        /// <summary>
        /// Creates a scaling matrix.
        /// </summary>
        /// <param name="scaling">Scaling vector</param>
        /// <returns>The scaling vector</returns>
        public static Matrix3x3 FromScaling(Vector3 scaling)
        {
            Matrix3x3 m = Identity;
            m.A1 = scaling.X;
            m.B2 = scaling.Y;
            m.C3 = scaling.Z;
            return m;
        }

        /// <summary>
        /// Creates a rotation matrix that rotates a vector called "from" into another
        /// vector called "to". Based on an algorithm by Tomas Moller and John Hudges:
        /// <para>
        /// "Efficiently Building a Matrix to Rotate One Vector to Another"         
        /// Journal of Graphics Tools, 4(4):1-4, 1999
        /// </para>
        /// </summary>
        /// <param name="from">Starting vector</param>
        /// <param name="to">Ending vector</param>
        /// <returns>Rotation matrix to rotate from the start to end.</returns>
        public static Matrix3x3 FromToMatrix(Vector3 from, Vector3 to)
        {
            float e = Vector3.Dot(from, to);
            float f = (e < 0) ? -e : e;

            Matrix3x3 m = Identity;

            //"from" and "to" vectors almost parallel
            if(f > 1.0f - 0.00001f)
            {
                Vector3 u, v; //Temp variables
                Vector3 x; //Vector almost orthogonal to "from"

                x.X = (from.X > 0.0f) ? from.X : -from.X;
                x.Y = (from.Y > 0.0f) ? from.Y : -from.Y;
                x.Z = (from.Z > 0.0f) ? from.Z : -from.Z;

                if(x.X < x.Y)
                {
                    if(x.X < x.Z)
                    {
                        x.X = 1.0f;
                        x.Y = 0.0f;
                        x.Z = 0.0f;
                    }
                    else
                    {
                        x.X = 0.0f;
                        x.Y = 0.0f;
                        x.Z = 1.0f;
                    }
                }
                else
                {
                    if(x.Y < x.Z)
                    {
                        x.X = 0.0f;
                        x.Y = 1.0f;
                        x.Z = 0.0f;
                    }
                    else
                    {
                        x.X = 0.0f;
                        x.Y = 0.0f;
                        x.Z = 1.0f;
                    }
                }

                u.X = x.X - from.X;
                u.Y = x.Y - from.Y;
                u.Z = x.Z - from.Z;

                v.X = x.X - to.X;
                v.Y = x.Y - to.Y;
                v.Z = x.Z - to.Z;

                float c1 = 2.0f / Vector3.Dot(u, u);
                float c2 = 2.0f / Vector3.Dot(v, v);
                float c3 = c1 * c2 * Vector3.Dot(u, v);

                for(int i = 1; i < 4; i++)
                {
                    for(int j = 1; j < 4; j++)
                    {
                        m[i, j] = -c1 * Index(u, i) * Index(u, j) - c2 * Index(v, i) * Index(v, j) + c3 * Index(v, i) * Index(u, j);

                        continue;

                        static float Index(Vector3 v, int i)
                        {
                            return i switch
                            {
                                1 => v.X,
                                2 => v.Y,
                                3 => v.Z,
                                _ => 0,
                            };
                        }
                    }
                    m[i, i] += 1.0f;
                }

            }
            else
            {
                //Most common case, unless "from" = "to" or "from" =- "to"
                Vector3 v = Vector3.Cross(from, to);

                //Hand optimized version (9 mults less) by Gottfried Chen
                float h = 1.0f / (1.0f + e);
                float hvx = h * v.X;
                float hvz = h * v.Z;
                float hvxy = hvx * v.Y;
                float hvxz = hvx * v.Z;
                float hvyz = hvz * v.Y;

                m.A1 = e + hvx * v.X;
                m.A2 = hvxy - v.Z;
                m.A3 = hvxz + v.Y;

                m.B1 = hvxy + v.Z;
                m.B2 = e + h * v.Y * v.Y;
                m.B3 = hvyz - v.X;

                m.C1 = hvxz - v.Y;
                m.C2 = hvyz + v.X;
                m.C3 = e + hvz * v.Z;
            }

            return m;
        }

        /// <summary>
        /// Performs matrix-vector multiplication identical to the way Assimp does it.
        /// </summary>
        /// <param name="v">Vector to transform</param>
        /// <returns>Transformed vector</returns>
        public Vector3 Transform(Vector3 v) =>
            new(
                A1 * v.X + A2 * v.Y + A3 * v.Z,
                B1 * v.X + B2 * v.Y + B3 * v.Z,
                C1 * v.X + C2 * v.Y + C3 * v.Z
            );

        /// <summary>
        /// Performs matrix multiplication.Multiplication order is B x A. That way, SRT concatenations
        /// are left to right.
        /// </summary>
        /// <param name="a">First matrix</param>
        /// <param name="b">Second matrix</param>
        /// <returns>Multiplied matrix</returns>
        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
        {
            return new Matrix3x3(a.A1 * b.A1 + a.B1 * b.A2 + a.C1 * b.A3,
                                 a.A2 * b.A1 + a.B2 * b.A2 + a.C2 * b.A3,
                                 a.A3 * b.A1 + a.B3 * b.A2 + a.C3 * b.A3,
                                 a.A1 * b.B1 + a.B1 * b.B2 + a.C1 * b.B3,
                                 a.A2 * b.B1 + a.B2 * b.B2 + a.C2 * b.B3,
                                 a.A3 * b.B1 + a.B3 * b.B2 + a.C3 * b.B3,
                                 a.A1 * b.C1 + a.B1 * b.C2 + a.C1 * b.C3,
                                 a.A2 * b.C1 + a.B2 * b.C2 + a.C2 * b.C3,
                                 a.A3 * b.C1 + a.B3 * b.C2 + a.C3 * b.C3);
        }

        /// <summary>
        /// Implicit conversion from a 4x4 matrix to a 3x3 matrix.
        /// </summary>
        /// <param name="mat">4x4 matrix</param>
        /// <returns>3x3 matrix</returns>
        public static implicit operator Matrix3x3(Matrix4x4 mat)
        {
            Matrix3x3 m = default;
            m.A1 = mat.M11;
            m.A2 = mat.M12;
            m.A3 = mat.M13;

            m.B1 = mat.M21;
            m.B2 = mat.M22;
            m.B3 = mat.M23;

            m.C1 = mat.M31;
            m.C2 = mat.M32;
            m.C3 = mat.M33;
            return m;
        }

        
        /// <summary>
        /// Implicit conversion from a 3x3 matrix to a 4x4 matrix.
        /// </summary>
        /// <param name="mat">3x3 matrix</param>
        /// <returns>4x4 matrix</returns>
        public static implicit operator Matrix4x4(Matrix3x3 mat)
        {
            Matrix4x4 m;
            m.M11 = mat.A1;
            m.M12 = mat.A2;
            m.M13 = mat.A3;
            m.M14 = 0;

            m.M21 = mat.B1;
            m.M22 = mat.B2;
            m.M23 = mat.B3;
            m.M24 = 0;

            m.M31 = mat.C1;
            m.M32 = mat.C2;
            m.M33 = mat.C3;
            m.M34 = 0;

            m.M41 = 0;
            m.M42 = 0;
            m.M43 = 0;
            m.M44 = 1;

            return m;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString() => $"{{[A1:{A1} A2:{A2} A3:{A3}] [B1:{B1} B2:{B2} B3:{B3}] [C1:{C1} C2:{C2} C3:{C3}]}}";
    }
}
