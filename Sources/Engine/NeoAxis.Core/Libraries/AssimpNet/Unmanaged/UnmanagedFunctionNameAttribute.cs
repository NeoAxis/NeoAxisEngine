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

namespace Internal.Assimp.Unmanaged
{
    /// <summary>
    /// An attribute that represents the name of an unmanaged function to import.
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate)]
    public class UnmanagedFunctionNameAttribute : Attribute
    {
        private string m_unmanagedFunctionName;

        /// <summary>
        /// Name of the unmanaged function.
        /// </summary>
        public string UnmanagedFunctionName => m_unmanagedFunctionName;

        /// <summary>
        /// Constructs a new <see cref="UnmanagedFunctionName"/>.
        /// </summary>
        /// <param name="unmanagedFunctionName">Name of the function.</param>
        public UnmanagedFunctionNameAttribute(string unmanagedFunctionName)
        {
            m_unmanagedFunctionName = unmanagedFunctionName;
        }
    }
}
