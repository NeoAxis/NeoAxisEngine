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

using System.Numerics;
using System.Runtime.InteropServices;

namespace Internal.Assimp
{
    /// <summary>
    /// Defines how an UV channel is transformed.
    /// </summary>
    /// <param name="Translation">Translation on the U and V axes. Default is 0|0.</param>
    /// <param name="Scaling">Scaling on the U and V axes. Default is 1|1.</param>
    /// <param name="Rotation">Rotation in counter-clockwise direction, specified in
    /// radians. The rotation center is 0.5f|0.5f and the
    /// default value is zero.</param>
    [StructLayout(LayoutKind.Sequential)]
    public record struct UVTransform(Vector2 Translation, Vector2 Scaling, float Rotation);
}
