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
    internal sealed class UnmanagedWin32LibraryImplementation : UnmanagedLibraryImplementation
    {
        public override string DllExtension => ".dll";

        public UnmanagedWin32LibraryImplementation(string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
            : base(defaultLibName, unmanagedFunctionDelegateTypes)
        {
        }

        protected override IntPtr NativeLoadLibrary(string path)
        {
            IntPtr libraryHandle = WinNativeLoadLibrary(path);

            if(libraryHandle == IntPtr.Zero && ThrowOnLoadFailure)
            {
                Exception innerException = null;

                //Keep the try-catch in case we're running on Mono. We're providing our own implementation of "Marshal.GetHRForLastWin32Error" which is NOT implemented
                //in mono, but let's just be cautious.
                try
                {
                    int hr = GetHRForLastWin32Error();
                    innerException = Marshal.GetExceptionForHR(hr);
                }
                catch(Exception) { }

                if(innerException != null)
                    throw new AssimpException(
                        $"Error loading unmanaged library from path: {path}\n\n{innerException.Message}", innerException);
                else
                    throw new AssimpException($"Error loading unmanaged library from path: {path}");
            }

            return libraryHandle;
        }

        protected override IntPtr NativeGetProcAddress(IntPtr handle, string functionName)
        {
            return GetProcAddress(handle, functionName);
        }

        protected override void NativeFreeLibrary(IntPtr handle)
        {
            FreeLibrary(handle);
        }

        private int GetHRForLastWin32Error()
        {
            //Mono, for some reason, throws in Marshal.GetHRForLastWin32Error(), but it should implement GetLastWin32Error, which is recommended than
            //p/invoking it ourselves when SetLastError is set in DllImport
            int dwLastError = Marshal.GetLastWin32Error();

            if((dwLastError & 0x80000000) == 0x80000000)
                return dwLastError;
            else
                return (dwLastError & 0x0000FFFF) | unchecked((int) 0x80070000);
        }

        #region Native Methods

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true, EntryPoint = "LoadLibrary")]
        private static extern IntPtr WinNativeLoadLibrary(string fileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        #endregion
    }
}
