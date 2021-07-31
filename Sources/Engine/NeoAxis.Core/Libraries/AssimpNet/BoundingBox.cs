/*
* Copyright (c) 2012-2019 AssimpNet - Nicholas Woodfield
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
using System.Globalization;
using System.Runtime.InteropServices;

namespace Assimp
{
    /// <summary>
    /// Represents an axis-aligned bounding box
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        /// <summary>
        /// Minimum point of the bounding box.
        /// </summary>
        public Vector3D Min;
        
        /// <summary>
        /// Maximum point of the bounding box.
        /// </summary>
        public Vector3D Max;

        /// <summary>
        /// Constructs a new BoundingBox.
        /// </summary>
        /// <param name="min">Minimum point.</param>
        /// <param name="max">Maximum point.</param>
        public BoundingBox(Vector3D min, Vector3D max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Tests equality between two Bounding Boxes.
        /// </summary>
        /// <param name="a">First BoundingBox</param>
        /// <param name="b">Second BoundingBox</param>
        /// <returns>True if the Bounding Boxes are equal, false otherwise</returns>
        public static bool operator ==(BoundingBox a, BoundingBox b)
        {
            return (a.Min == b.Min) && (a.Max == b.Max);
        }

        /// <summary>
        /// Tests inequality between two Bounding Boxes.
        /// </summary>
        /// <param name="a">First BoundingBox</param>
        /// <param name="b">Second BoundingBox</param>
        /// <returns>True if the Bounding Boxes are not equal, false otherwise</returns>
        public static bool operator !=(BoundingBox a, BoundingBox b)
        {
            return (a.Min != b.Min) || (a.Max != b.Max);
        }

        /// <summary>
        /// Tests equality between this BoundingBox and another BoundingBox.
        /// </summary>
        /// <param name="other">BoundingBox to test against</param>
        /// <returns>True if components are equal</returns>
        public bool Equals(BoundingBox other)
        {
            return (Min == other.Min) && (Max == other.Max);
        }

        /// <summary>
        /// Tests equality between this vector and another object.
        /// </summary>
        /// <param name="obj">Object to test against</param>
        /// <returns>True if the object is a vector and the components are equal</returns>
        public override bool Equals(object obj)
        {
            if(obj is BoundingBox)
                return Equals((BoundingBox) obj);

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (Min.GetHashCode() + Max.GetHashCode());
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            CultureInfo info = CultureInfo.CurrentCulture;
            return String.Format(info, "{{Min:{0} Max:{1}}",
                new Object[] { Min.ToString(), Max.ToString() });
        }
    }
}
