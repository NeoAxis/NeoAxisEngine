/*
* Copyright (c) 2007-2010 SlimDX Group
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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Globalization;

namespace Internal.BulletSharp.Math
{
    /// <summary>
    /// Represents a four dimensional mathematical quaternion.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    //[TypeConverter(typeof(SlimMath.Design.QuaternionConverter))]
    public struct BQuaternion : IEquatable<BQuaternion>, IFormattable
    {
        /// <summary>
        /// The size of the <see cref="SlimMath.Quaternion"/> type, in bytes.
        /// </summary>
        public const int SizeInBytes = 4 * sizeof(double);

        /// <summary>
        /// A <see cref="SlimMath.Quaternion"/> with all of its components set to zero.
        /// </summary>
        public static readonly BQuaternion Zero = new BQuaternion();

        /// <summary>
        /// A <see cref="SlimMath.Quaternion"/> with all of its components set to one.
        /// </summary>
        public static readonly BQuaternion One = new BQuaternion(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// The identity <see cref="SlimMath.Quaternion"/> (0, 0, 0, 1).
        /// </summary>
        public static readonly BQuaternion Identity = new BQuaternion(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// The X component of the quaternion.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y component of the quaternion.
        /// </summary>
        public double Y;

        /// <summary>
        /// The Z component of the quaternion.
        /// </summary>
        public double Z;

        /// <summary>
        /// The W component of the quaternion.
        /// </summary>
        public double W;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlimMath.Quaternion"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public BQuaternion(double value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlimMath.Quaternion"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the components.</param>
        public BQuaternion(BVector4 value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = value.W;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlimMath.Quaternion"/> struct.
        /// </summary>
        /// <param name="axis">A vector containing the values with which to initialize the X, Y, and Z components.</param>
        /// <param name="angle">Initial value for the angle of the quaternion.</param>
        public BQuaternion(BVector3 axis, double angle)
        {
            double angle2 = angle * 0.5f;
            double s = (double)System.Math.Sin(angle2) / axis.Length;
            X = axis.X * s;
            Y = axis.Y * s;
            Z = axis.Z * s;
            W = (double)System.Math.Cos(angle2);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlimMath.Quaternion"/> struct.
        /// </summary>
        /// <param name="x">Initial value for the X component of the quaternion.</param>
        /// <param name="y">Initial value for the Y component of the quaternion.</param>
        /// <param name="z">Initial value for the Z component of the quaternion.</param>
        /// <param name="w">Initial value for the W component of the quaternion.</param>
        public BQuaternion(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlimMath.Quaternion"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y, Z, and W components of the quaternion. This must be an array with four elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
        public BQuaternion(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException("values", "There must be four and only four input values for Quaternion.");

            X = values[0];
            Y = values[1];
            Z = values[2];
            W = values[3];
        }

        /// <summary>
        /// Gets a value indicating whether this instance is equivalent to the identity quaternion.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is an identity quaternion; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdentity
        {
            get { return this.Equals(Identity); }
        }

        /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public bool IsNormalized
        {
            get { return System.Math.Abs((X * X) + (Y * Y) + (Z * Z) + (W * W) - 1f) < Utilities.ZeroTolerance; }
        }

        /// <summary>
        /// Gets the angle of the quaternion.
        /// </summary>
        /// <value>The quaternion's angle.</value>
        public double Angle
        {
            get
            {
                double length = (X * X) + (Y * Y) + (Z * Z);
                if (length < Utilities.ZeroTolerance)
                    return 0.0f;

                return (double)(2.0 * System.Math.Acos(W));
            }
        }

        /// <summary>
        /// Gets the axis components of the quaternion.
        /// </summary>
        /// <value>The axis components of the quaternion.</value>
        public BVector3 Axis
        {
            get
            {
                double inv = 1.0f / (W * W);
                if (inv < 10.0f * Utilities.ZeroTolerance)
                    return BVector3.UnitX;

                return new BVector3(X * inv, Y * inv, Z * inv);
            }
        }

        /// <summary>
        /// Calculates the length of the quaternion.
        /// </summary>
        /// <remarks>
        /// <see cref="SlimMath.Quaternion.LengthSquared"/> may be preferred when only the relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public double Length
        {
            get { return (double)System.Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W)); }
        }

        /// <summary>
        /// Calculates the squared length of the quaternion.
        /// </summary>
        /// <remarks>
        /// This property may be preferred to <see cref="SlimMath.Quaternion.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public double LengthSquared
        {
            get { return (X * X) + (Y * Y) + (Z * Z) + (W * W); }
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Quaternion run from 0 to 3, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Quaternion run from 0 to 3, inclusive.");
                }
            }
        }

        /// <summary>
        /// Conjugates the quaternion.
        /// </summary>
        public void Conjugate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }

        /// <summary>
        /// Reverses the direction of a given quaternion.
        /// </summary>
        public void Negate()
        {
            this.X = -X;
            this.Y = -Y;
            this.Z = -Z;
            this.W = -W;
        }

        /// <summary>
        /// Return the inverse of this quaternion.
        /// </summary>
        public BQuaternion Inverse
        {
            get { return new BQuaternion(-X, -Y, -Z, W); }
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        public void Invert()
        {
            double lengthSq = LengthSquared;
            if (lengthSq > Utilities.ZeroTolerance)
            {
                lengthSq = 1.0f / lengthSq;

                X = -X * lengthSq;
                Y = -Y * lengthSq;
                Z = -Z * lengthSq;
                W = W * lengthSq;
            }
        }

        /// <summary>
        /// Converts the quaternion into a unit quaternion.
        /// </summary>
        public void Normalize()
        {
            double length = Length;
            if (length > Utilities.ZeroTolerance)
            {
                double inverse = 1.0f / length;
                X *= inverse;
                Y *= inverse;
                Z *= inverse;
                W *= inverse;
            }
        }

        /// <summary>
        /// Exponentiates a quaternion.
        /// </summary>
        public void Exponential()
        {
            Exponential(ref this, out this);
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified quaternion.
        /// </summary>
        public void Logarithm()
        {
            Logarithm(ref this, out this);
        }

        /// <summary>
        /// Creates an array containing the elements of the quaternion.
        /// </summary>
        /// <returns>A four-element array containing the components of the quaternion.</returns>
        public double[] ToArray()
        {
            return new double[] { X, Y, Z, W };
        }

        /// <summary>
        /// Adds two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to add.</param>
        /// <param name="right">The second quaternion to add.</param>
        /// <param name="result">When the method completes, contains the sum of the two quaternions.</param>
        public static void Add(ref BQuaternion left, ref BQuaternion right, out BQuaternion result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            result.W = left.W + right.W;
        }

        /// <summary>
        /// Adds two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to add.</param>
        /// <param name="right">The second quaternion to add.</param>
        /// <returns>The sum of the two quaternions.</returns>
        public static BQuaternion Add(BQuaternion left, BQuaternion right)
        {
            BQuaternion result;
            Add(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Subtracts two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to subtract.</param>
        /// <param name="right">The second quaternion to subtract.</param>
        /// <param name="result">When the method completes, contains the difference of the two quaternions.</param>
        public static void Subtract(ref BQuaternion left, ref BQuaternion right, out BQuaternion result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = left.W - right.W;
        }

        /// <summary>
        /// Subtracts two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to subtract.</param>
        /// <param name="right">The second quaternion to subtract.</param>
        /// <returns>The difference of the two quaternions.</returns>
        public static BQuaternion Subtract(BQuaternion left, BQuaternion right)
        {
            BQuaternion result;
            Subtract(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scalar">The amount by which to scale the quaternion.</param>
        /// <param name="result">When the method completes, contains the scaled quaternion.</param>
        public static void Multiply(ref BQuaternion value, double scalar, out BQuaternion result)
        {
            result.X = value.X * scalar;
            result.Y = value.Y * scalar;
            result.Z = value.Z * scalar;
            result.W = value.W * scalar;
        }

        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scalar">The amount by which to scale the quaternion.</param>
        /// <returns>The scaled quaternion.</returns>
        public static BQuaternion Multiply(BQuaternion value, double scalar)
        {
            BQuaternion result;
            Multiply(ref value, scalar, out result);
            return result;
        }

        /// <summary>
        /// Modulates a quaternion by another.
        /// </summary>
        /// <param name="left">The first quaternion to modulate.</param>
        /// <param name="right">The second quaternion to modulate.</param>
        /// <param name="result">When the moethod completes, contains the modulated quaternion.</param>
        public static void Multiply(ref BQuaternion left, ref BQuaternion right, out BQuaternion result)
        {
            double lx = left.X;
            double ly = left.Y;
            double lz = left.Z;
            double lw = left.W;
            double rx = right.X;
            double ry = right.Y;
            double rz = right.Z;
            double rw = right.W;

            result.X = (rx * lw + lx * rw + ry * lz) - (rz * ly);
            result.Y = (ry * lw + ly * rw + rz * lx) - (rx * lz);
            result.Z = (rz * lw + lz * rw + rx * ly) - (ry * lx);
            result.W = (rw * lw) - (rx * lx + ry * ly + rz * lz);
        }

        /// <summary>
        /// Modulates a quaternion by another.
        /// </summary>
        /// <param name="left">The first quaternion to modulate.</param>
        /// <param name="right">The second quaternion to modulate.</param>
        /// <returns>The modulated quaternion.</returns>
        public static BQuaternion Multiply(BQuaternion left, BQuaternion right)
        {
            BQuaternion result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scalar">The amount by which to scale the vector.</param>
        /// <param name="result">When the method completes, contains the scaled vector.</param>
        public static void Divide(ref BQuaternion value, double scalar, out BQuaternion result)
        {
            result = new BQuaternion(value.X / scalar, value.Y / scalar, value.Z / scalar, value.W / scalar);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scalar">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        public static BQuaternion Divide(BQuaternion value, double scalar)
        {
            return new BQuaternion(value.X / scalar, value.Y / scalar, value.Z / scalar, value.W / scalar);
        }

        /// <summary>
        /// Reverses the direction of a given quaternion.
        /// </summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <param name="result">When the method completes, contains a quaternion facing in the opposite direction.</param>
        public static void Negate(ref BQuaternion value, out BQuaternion result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
        }

        /// <summary>
        /// Reverses the direction of a given quaternion.
        /// </summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>A quaternion facing in the opposite direction.</returns>
        public static BQuaternion Negate(BQuaternion value)
        {
            BQuaternion result;
            Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <param name="result">When the method completes, contains a new <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</param>
        public static void Barycentric(ref BQuaternion value1, ref BQuaternion value2, ref BQuaternion value3, double amount1, double amount2, out BQuaternion result)
        {
            BQuaternion start, end;
            Slerp(ref value1, ref value2, amount1 + amount2, out start);
            Slerp(ref value1, ref value3, amount1 + amount2, out end);
            Slerp(ref start, ref end, amount2 / (amount1 + amount2), out result);
        }

        /// <summary>
        /// Returns a <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <returns>A new <see cref="SlimMath.Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</returns>
        public static BQuaternion Barycentric(BQuaternion value1, BQuaternion value2, BQuaternion value3, double amount1, double amount2)
        {
            BQuaternion result;
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out result);
            return result;
        }

        /// <summary>
        /// Conjugates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to conjugate.</param>
        /// <param name="result">When the method completes, contains the conjugated quaternion.</param>
        public static void Conjugate(ref BQuaternion value, out BQuaternion result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = value.W;
        }

        /// <summary>
        /// Conjugates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to conjugate.</param>
        /// <returns>The conjugated quaternion.</returns>
        public static BQuaternion Conjugate(BQuaternion value)
        {
            BQuaternion result;
            Conjugate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Calculates the dot product of two quaternions.
        /// </summary>
        /// <param name="left">First source quaternion.</param>
        /// <param name="right">Second source quaternion.</param>
        /// <param name="result">When the method completes, contains the dot product of the two quaternions.</param>
        public static void Dot(ref BQuaternion left, ref BQuaternion right, out double result)
        {
            result = (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
        }

        /// <summary>
        /// Calculates the dot product of two quaternions.
        /// </summary>
        /// <param name="left">First source quaternion.</param>
        /// <param name="right">Second source quaternion.</param>
        /// <returns>The dot product of the two quaternions.</returns>
        public static double Dot(BQuaternion left, BQuaternion right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
        }

        /// <summary>
        /// Exponentiates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to exponentiate.</param>
        /// <param name="result">When the method completes, contains the exponentiated quaternion.</param>
        public static void Exponential(ref BQuaternion value, out BQuaternion result)
        {
            double angle = (double)System.Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
            double sin = (double)System.Math.Sin(angle);

            if (System.Math.Abs(sin) >= Utilities.ZeroTolerance)
            {
                double coeff = sin / angle;
                result.X = coeff * value.X;
                result.Y = coeff * value.Y;
                result.Z = coeff * value.Z;
            }
            else
            {
                result = value;
            }

            result.W = (double)System.Math.Cos(angle);
        }

        /// <summary>
        /// Exponentiates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to exponentiate.</param>
        /// <returns>The exponentiated quaternion.</returns>
        public static BQuaternion Exponential(BQuaternion value)
        {
            BQuaternion result;
            Exponential(ref value, out result);
            return result;
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        /// <param name="value">The quaternion to conjugate and renormalize.</param>
        /// <param name="result">When the method completes, contains the conjugated and renormalized quaternion.</param>
        public static void Invert(ref BQuaternion value, out BQuaternion result)
        {
            result = value;
            result.Invert();
        }

        /// <summary>
        /// Conjugates and renormalizes the quaternion.
        /// </summary>
        /// <param name="value">The quaternion to conjugate and renormalize.</param>
        /// <returns>The conjugated and renormalized quaternion.</returns>
        public static BQuaternion Invert(BQuaternion value)
        {
            BQuaternion result;
            Invert(ref value, out result);
            return result;
        }

        /// <summary>
        /// Performs a linear interpolation between two quaternions.
        /// </summary>
        /// <param name="start">Start quaternion.</param>
        /// <param name="end">End quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the linear interpolation of the two quaternions.</param>
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// <code>start + (end - start) * amount</code>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static void Lerp(ref BQuaternion start, ref BQuaternion end, double amount, out BQuaternion result)
        {
            double inverse = 1.0f - amount;

            if (Dot(start, end) >= 0.0f)
            {
                result.X = (inverse * start.X) + (amount * end.X);
                result.Y = (inverse * start.Y) + (amount * end.Y);
                result.Z = (inverse * start.Z) + (amount * end.Z);
                result.W = (inverse * start.W) + (amount * end.W);
            }
            else
            {
                result.X = (inverse * start.X) - (amount * end.X);
                result.Y = (inverse * start.Y) - (amount * end.Y);
                result.Z = (inverse * start.Z) - (amount * end.Z);
                result.W = (inverse * start.W) - (amount * end.W);
            }

            result.Normalize();
        }

        /// <summary>
        /// Performs a linear interpolation between two quaternion.
        /// </summary>
        /// <param name="start">Start quaternion.</param>
        /// <param name="end">End quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The linear interpolation of the two quaternions.</returns>
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// <code>start + (end - start) * amount</code>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static BQuaternion Lerp(BQuaternion start, BQuaternion end, double amount)
        {
            BQuaternion result;
            Lerp(ref start, ref end, amount, out result);
            return result;
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified quaternion.
        /// </summary>
        /// <param name="value">The quaternion whose logarithm will be calculated.</param>
        /// <param name="result">When the method completes, contains the natural logarithm of the quaternion.</param>
        public static void Logarithm(ref BQuaternion value, out BQuaternion result)
        {
            if (System.Math.Abs(value.W) < 1.0f)
            {
                double angle = (double)System.Math.Acos(value.W);
                double sin = (double)System.Math.Sin(angle);

                if (System.Math.Abs(sin) >= Utilities.ZeroTolerance)
                {
                    double coeff = angle / sin;
                    result.X = value.X * coeff;
                    result.Y = value.Y * coeff;
                    result.Z = value.Z * coeff;
                }
                else
                {
                    result = value;
                }
            }
            else
            {
                result = value;
            }

            result.W = 0.0f;
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified quaternion.
        /// </summary>
        /// <param name="value">The quaternion whose logarithm will be calculated.</param>
        /// <returns>The natural logarithm of the quaternion.</returns>
        public static BQuaternion Logarithm(BQuaternion value)
        {
            BQuaternion result;
            Logarithm(ref value, out result);
            return result;
        }

        /// <summary>
        /// Converts the quaternion into a unit quaternion.
        /// </summary>
        /// <param name="value">The quaternion to normalize.</param>
        /// <param name="result">When the method completes, contains the normalized quaternion.</param>
        public static void Normalize(ref BQuaternion value, out BQuaternion result)
        {
            BQuaternion temp = value;
            result = temp;
            result.Normalize();
        }

        /// <summary>
        /// Converts the quaternion into a unit quaternion.
        /// </summary>
        /// <param name="value">The quaternion to normalize.</param>
        /// <returns>The normalized quaternion.</returns>
        public static BQuaternion Normalize(BQuaternion value)
        {
            value.Normalize();
            return value;
        }

        public BVector3 Rotate(BVector3 v)
        {
            BQuaternion rotation = this;
            BQuaternion q = rotation * v;
            rotation.Invert();
            q *= rotation;
            return new BVector3(q.X, q.Y, q.Z);
        }

        /// <summary>
        /// Creates a quaternion given a rotation and an axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationAxis(ref BVector3 axis, double angle, out BQuaternion result)
        {
            BVector3 normalized;
            BVector3.Normalize(ref axis, out normalized);

            double half = angle * 0.5f;
            double sin = (double)System.Math.Sin(half);
            double cos = (double)System.Math.Cos(half);

            result.X = normalized.X * sin;
            result.Y = normalized.Y * sin;
            result.Z = normalized.Z * sin;
            result.W = cos;
        }

        /// <summary>
        /// Creates a quaternion given a rotation and an axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation.</param>
        /// <returns>The newly created quaternion.</returns>
        public static BQuaternion RotationAxis(BVector3 axis, double angle)
        {
            BQuaternion result;
            RotationAxis(ref axis, angle, out result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationMatrix(ref BMatrix matrix, out BQuaternion result)
        {
            double sqrt;
            double half;
            double scale = matrix.M11 + matrix.M22 + matrix.M33;

            if (scale > 0.0f)
            {
                sqrt = (double)System.Math.Sqrt(scale + 1.0f);
                result.W = sqrt * 0.5f;
                sqrt = 0.5f / sqrt;

                result.X = (matrix.M23 - matrix.M32) * sqrt;
                result.Y = (matrix.M31 - matrix.M13) * sqrt;
                result.Z = (matrix.M12 - matrix.M21) * sqrt;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                sqrt = (double)System.Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = 0.5f * sqrt;
                result.Y = (matrix.M12 + matrix.M21) * half;
                result.Z = (matrix.M13 + matrix.M31) * half;
                result.W = (matrix.M23 - matrix.M32) * half;
            }
            else if (matrix.M22 > matrix.M33)
            {
                sqrt = (double)System.Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                half = 0.5f / sqrt;

                result.X = (matrix.M21 + matrix.M12) * half;
                result.Y = 0.5f * sqrt;
                result.Z = (matrix.M32 + matrix.M23) * half;
                result.W = (matrix.M31 - matrix.M13) * half;
            }
            else
            {
                sqrt = (double)System.Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                half = 0.5f / sqrt;

                result.X = (matrix.M31 + matrix.M13) * half;
                result.Y = (matrix.M32 + matrix.M23) * half;
                result.Z = 0.5f * sqrt;
                result.W = (matrix.M12 - matrix.M21) * half;
            }
        }

        /// <summary>
        /// Creates a quaternion given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>The newly created quaternion.</returns>
        public static BQuaternion RotationMatrix(BMatrix matrix)
        {
            BQuaternion result;
            RotationMatrix(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion given a yaw, pitch, and roll value.
        /// </summary>
        /// <param name="yaw">The yaw of rotation.</param>
        /// <param name="pitch">The pitch of rotation.</param>
        /// <param name="roll">The roll of rotation.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationYawPitchRoll(double yaw, double pitch, double roll, out BQuaternion result)
        {
            double halfRoll = roll * 0.5f;
            double halfPitch = pitch * 0.5f;
            double halfYaw = yaw * 0.5f;

            double sinRoll = (double)System.Math.Sin(halfRoll);
            double cosRoll = (double)System.Math.Cos(halfRoll);
            double sinPitch = (double)System.Math.Sin(halfPitch);
            double cosPitch = (double)System.Math.Cos(halfPitch);
            double sinYaw = (double)System.Math.Sin(halfYaw);
            double cosYaw = (double)System.Math.Cos(halfYaw);

            result.X = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
            result.Y = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
            result.Z = (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll);
            result.W = (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll);
        }

        /// <summary>
        /// Creates a quaternion given a yaw, pitch, and roll value.
        /// </summary>
        /// <param name="yaw">The yaw of rotation.</param>
        /// <param name="pitch">The pitch of rotation.</param>
        /// <param name="roll">The roll of rotation.</param>
        /// <returns>The newly created quaternion.</returns>
        public static BQuaternion RotationYawPitchRoll(double yaw, double pitch, double roll)
        {
            BQuaternion result;
            RotationYawPitchRoll(yaw, pitch, roll, out result);
            return result;
        }

        /// <summary>
        /// Interpolates between two quaternions, using spherical linear interpolation.
        /// </summary>
        /// <param name="start">Start quaternion.</param>
        /// <param name="end">End quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the spherical linear interpolation of the two quaternions.</param>
        public static void Slerp(ref BQuaternion start, ref BQuaternion end, double amount, out BQuaternion result)
        {
            double opposite;
            double inverse;
            double dot = Dot(start, end);

            if (System.Math.Abs(dot) > 1.0f - Utilities.ZeroTolerance)
            {
                inverse = 1.0f - amount;
                opposite = amount * System.Math.Sign(dot);
            }
            else
            {
                double acos = (double)System.Math.Acos(System.Math.Abs(dot));
                double invSin = (double)(1.0f / System.Math.Sin(acos));

                inverse = (double)System.Math.Sin((1.0f - amount) * acos) * invSin;
                opposite = (double)System.Math.Sin(amount * acos) * invSin * System.Math.Sign(dot);
            }

            result.X = (inverse * start.X) + (opposite * end.X);
            result.Y = (inverse * start.Y) + (opposite * end.Y);
            result.Z = (inverse * start.Z) + (opposite * end.Z);
            result.W = (inverse * start.W) + (opposite * end.W);
        }

        /// <summary>
        /// Interpolates between two quaternions, using spherical linear interpolation.
        /// </summary>
        /// <param name="start">Start quaternion.</param>
        /// <param name="end">End quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The spherical linear interpolation of the two quaternions.</returns>
        public static BQuaternion Slerp(BQuaternion start, BQuaternion end, double amount)
        {
            BQuaternion result;
            Slerp(ref start, ref end, amount, out result);
            return result;
        }

        /// <summary>
        /// Interpolates between quaternions, using spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Thrid source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
        /// <param name="result">When the method completes, contains the spherical quadrangle interpolation of the quaternions.</param>
        public static void Squad(ref BQuaternion value1, ref BQuaternion value2, ref BQuaternion value3, ref BQuaternion value4, double amount, out BQuaternion result)
        {
            BQuaternion start, end;
            Slerp(ref value1, ref value4, amount, out start);
            Slerp(ref value2, ref value3, amount, out end);
            Slerp(ref start, ref end, 2.0f * amount * (1.0f - amount), out result);
        }

        /// <summary>
        /// Interpolates between quaternions, using spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Thrid source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
        /// <returns>The spherical quadrangle interpolation of the quaternions.</returns>
        public static BQuaternion Squad(BQuaternion value1, BQuaternion value2, BQuaternion value3, BQuaternion value4, double amount)
        {
            BQuaternion result;
            Squad(ref value1, ref value2, ref value3, ref value4, amount, out result);
            return result;
        }

        /// <summary>
        /// Sets up control points for spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Third source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <param name="result1">When the method completes, contains the first control point for spherical quadrangle interpolation.</param>
        /// <param name="result2">When the method completes, contains the second control point for spherical quadrangle interpolation.</param>
        /// <param name="result3">When the method completes, contains the third control point for spherical quadrangle interpolation.</param>
        public static void SquadSetup(ref BQuaternion value1, ref BQuaternion value2, ref BQuaternion value3, ref BQuaternion value4, out BQuaternion result1, out BQuaternion result2, out BQuaternion result3)
        {
            BQuaternion q0 = (value1 + value2).LengthSquared < (value1 - value2).LengthSquared ? -value1 : value1;
            BQuaternion q2 = (value2 + value3).LengthSquared < (value2 - value3).LengthSquared ? -value3 : value3;
            BQuaternion q3 = (value3 + value4).LengthSquared < (value3 - value4).LengthSquared ? -value4 : value4;
            BQuaternion q1 = value2;

            BQuaternion q1Exp, q2Exp;
            Exponential(ref q1, out q1Exp);
            Exponential(ref q2, out q2Exp);

            result1 = q1 * Exponential(-0.25f * (Logarithm(q1Exp * q2) + Logarithm(q1Exp * q0)));
            result2 = q2 * Exponential(-0.25f * (Logarithm(q2Exp * q3) + Logarithm(q2Exp * q1)));
            result3 = q2;
        }

        /// <summary>
        /// Sets up control points for spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Third source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <returns>An array of three quaternions that represent control points for spherical quadrangle interpolation.</returns>
        public static BQuaternion[] SquadSetup(BQuaternion value1, BQuaternion value2, BQuaternion value3, BQuaternion value4)
        {
            BQuaternion[] results = new BQuaternion[3];
            SquadSetup(ref value1, ref value2, ref value3, ref value4, out results[0], out results[1], out results[2]);

            return results;
        }

        /// <summary>
        /// Adds two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to add.</param>
        /// <param name="right">The second quaternion to add.</param>
        /// <returns>The sum of the two quaternions.</returns>
        public static BQuaternion operator +(BQuaternion left, BQuaternion right)
        {
            BQuaternion result;
            Add(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Subtracts two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to subtract.</param>
        /// <param name="right">The second quaternion to subtract.</param>
        /// <returns>The difference of the two quaternions.</returns>
        public static BQuaternion operator -(BQuaternion left, BQuaternion right)
        {
            BQuaternion result;
            Subtract(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Reverses the direction of a given quaternion.
        /// </summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>A quaternion facing in the opposite direction.</returns>
        public static BQuaternion operator -(BQuaternion value)
        {
            BQuaternion result;
            Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scalar">The amount by which to scale the quaternion.</param>
        /// <returns>The scaled quaternion.</returns>
        public static BQuaternion operator *(double scalar, BQuaternion value)
        {
            BQuaternion result;
            Multiply(ref value, scalar, out result);
            return result;
        }

        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scalar">The amount by which to scale the quaternion.</param>
        /// <returns>The scaled quaternion.</returns>
        public static BQuaternion operator *(BQuaternion value, double scalar)
        {
            BQuaternion result;
            Multiply(ref value, scalar, out result);
            return result;
        }

        /// <summary>
        /// Multiplies a quaternion by another.
        /// </summary>
        /// <param name="left">The first quaternion to multiply.</param>
        /// <param name="right">The second quaternion to multiply.</param>
        /// <returns>The multiplied quaternion.</returns>
        public static BQuaternion operator *(BQuaternion left, BQuaternion right)
        {
            BQuaternion result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies a quaternion by a vector.
        /// </summary>
        /// <param name="q">The quaternion to multiply.</param>
        /// <param name="w">The vector to multiply.</param>
        /// <returns>The multiplied quaternion.</returns>
        public static BQuaternion operator *(BQuaternion q, BVector3 w)
        {
            return new BQuaternion(
                q.W * w.X + q.Y * w.Z - q.Z * w.Y,
                q.W * w.Y + q.Z * w.X - q.X * w.Z,
                q.W * w.Z + q.X * w.Y - q.Y * w.X,
                -q.X * w.X - q.Y * w.Y - q.Z * w.Z);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scalar">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        public static BQuaternion operator /(BQuaternion value, double scalar)
        {
            return new BQuaternion(value.X / scalar, value.Y / scalar, value.Z / scalar, value.W / scalar);
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(BQuaternion left, BQuaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(BQuaternion left, BQuaternion right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, CultureInfo.CurrentCulture),
                Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture), W.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider), Z.ToString(format, formatProvider), W.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="SlimMath.Quaternion"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SlimMath.Quaternion"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="SlimMath.Quaternion"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(BQuaternion other)
        {
            return (this.X == other.X) && (this.Y == other.Y) && (this.Z == other.Z) && (this.W == other.W);
        }

        /// <summary>
        /// Determines whether the specified <see cref="SlimMath.Quaternion"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SlimMath.Quaternion"/> to compare with this instance.</param>
        /// <param name="epsilon">The amount of error allowed.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="SlimMath.Quaternion"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(BQuaternion other, double epsilon)
        {
            return ((double)System.Math.Abs(other.X - X) < epsilon &&
                (double)System.Math.Abs(other.Y - Y) < epsilon &&
                (double)System.Math.Abs(other.Z - Z) < epsilon &&
                (double)System.Math.Abs(other.W - W) < epsilon);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return Equals((BQuaternion)obj);
        }

#if SlimDX1xInterop
        /// <summary>
        /// Performs an implicit conversion from <see cref="SlimMath.Quaternion"/> to <see cref="SlimDX.Quaternion"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SlimDX.Quaternion(Quaternion value)
        {
            return new SlimDX.Quaternion(value.X, value.Y, value.Z, value.W);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SlimDX.Quaternion"/> to <see cref="SlimMath.Quaternion"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Quaternion(SlimDX.Quaternion value)
        {
            return new Quaternion(value.X, value.Y, value.Z, value.W);
        }
#endif

#if WPFInterop
        /// <summary>
        /// Performs an implicit conversion from <see cref="SlimMath.Quaternion"/> to <see cref="System.Windows.Media.Media3D.Quaternion"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator System.Windows.Media.Media3D.Quaternion(Quaternion value)
        {
            return new System.Windows.Media.Media3D.Quaternion(value.X, value.Y, value.Z, value.W);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Windows.Media.Media3D.Quaternion"/> to <see cref="SlimMath.Quaternion"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Quaternion(System.Windows.Media.Media3D.Quaternion value)
        {
            return new Quaternion((double)value.X, (double)value.Y, (double)value.Z, (double)value.W);
        }
#endif

#if XnaInterop
        /// <summary>
        /// Performs an implicit conversion from <see cref="SlimMath.Quaternion"/> to <see cref="Microsoft.Xna.Framework.Quaternion"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Microsoft.Xna.Framework.Quaternion(Quaternion value)
        {
            return new Microsoft.Xna.Framework.Quaternion(value.X, value.Y, value.Z, value.W);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Microsoft.Xna.Framework.Quaternion"/> to <see cref="SlimMath.Quaternion"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Quaternion(Microsoft.Xna.Framework.Quaternion value)
        {
            return new Quaternion(value.X, value.Y, value.Z, value.W);
        }
#endif
    }
}
