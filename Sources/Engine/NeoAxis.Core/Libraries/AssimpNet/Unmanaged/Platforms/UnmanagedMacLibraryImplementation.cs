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
using System.Runtime.InteropServices;

namespace Internal.Assimp.Unmanaged
{
    internal sealed class UnmanagedMacLibraryImplementation : UnmanagedLibraryImplementation
    {
        public override string DllExtension => ".dylib";

        public override string DllPrefix => "lib";

        public UnmanagedMacLibraryImplementation(string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
            : base(defaultLibName, unmanagedFunctionDelegateTypes)
        {
        }

        protected override IntPtr NativeLoadLibrary(string path)
        {
            IntPtr libraryHandle = dlopen(path, RTLD_NOW);

            if(libraryHandle == IntPtr.Zero && ThrowOnLoadFailure)
            {
                IntPtr errPtr = dlerror();
                string msg = Marshal.PtrToStringAnsi(errPtr);
                if(!string.IsNullOrEmpty(msg))
                    throw new AssimpException($"Error loading unmanaged library from path: {path}\n\n{msg}");
                else
                    throw new AssimpException($"Error loading unmanaged library from path: {path}");
            }

            return libraryHandle;
        }

        protected override IntPtr NativeGetProcAddress(IntPtr handle, string functionName)
        {
            return dlsym(handle, functionName);
        }

        protected override void NativeFreeLibrary(IntPtr handle)
        {
            dlclose(handle);
        }

        #region Native Methods

        [DllImport("libSystem.B.dylib")]
        private static extern IntPtr dlopen(string fileName, int flags);

        [DllImport("libSystem.B.dylib")]
        private static extern IntPtr dlsym(IntPtr handle, string functionName);

        [DllImport("libSystem.B.dylib")]
        private static extern int dlclose(IntPtr handle);

        [DllImport("libSystem.B.dylib")]
        private static extern IntPtr dlerror();

        private const int RTLD_NOW = 2;

        #endregion
    }
}
