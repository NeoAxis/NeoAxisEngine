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

using System.Runtime.InteropServices;

namespace Internal.Assimp
{
    /// <summary>
    /// Binds an anim mesh (referenced by an index) to a specific point in time.
    /// </summary>
    /// <param name="Time">The time of this key.</param>
    /// <param name="Value">Index of the anim mesh that corresponds to this keyframe.</param>
    [StructLayout(LayoutKind.Sequential)]
    public record struct MeshKey(double Time, int Value)
    {
        /// <summary>
        /// Tests inequality between two keys.
        /// </summary>
        /// <param name="a">The first key</param>
        /// <param name="b">The second key</param>
        /// <returns>True if the first key's time is less than the second key's.</returns>
        public static bool operator <(MeshKey a, MeshKey b) => a.Time < b.Time;

        /// <summary>
        /// Tests inequality between two keys.
        /// </summary>
        /// <param name="a">The first key</param>
        /// <param name="b">The second key</param>
        /// <returns>True if the first key's time is greater than the second key's.</returns>
        public static bool operator >(MeshKey a, MeshKey b) => a.Time > b.Time;

        /// <summary>
        /// Tests equality between this key and another.
        /// </summary>
        /// <param name="key">Other key to test</param>
        /// <returns>True if their indices are equal</returns>
        public bool Equals(MeshKey key) => Value == key.Value;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => Value.GetHashCode();
    }
}
