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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Internal.Assimp.Unmanaged
{
    internal abstract class UnmanagedLibraryImplementation : IDisposable
    {
        private string m_defaultLibName;
        private Type[] m_unmanagedFunctionDelegateTypes;
        private Dictionary<string, Delegate> m_nameToUnmanagedFunction;
        private IntPtr m_libraryHandle;
        private bool m_isDisposed;
        private bool m_throwOnLoadFailure;

        public bool IsLibraryLoaded => m_libraryHandle != IntPtr.Zero;

        public bool IsDisposed => m_isDisposed;

        public string DefaultLibraryName => m_defaultLibName;

        public bool ThrowOnLoadFailure
        {
            get => m_throwOnLoadFailure;
            set => m_throwOnLoadFailure = value;
        }

        public abstract string DllExtension { get; }

        public virtual string DllPrefix => string.Empty;

        public UnmanagedLibraryImplementation(string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
        {
            m_defaultLibName = DllPrefix + Path.ChangeExtension(defaultLibName, DllExtension);

            m_unmanagedFunctionDelegateTypes = unmanagedFunctionDelegateTypes;

            m_nameToUnmanagedFunction = new Dictionary<string, Delegate>();
            m_isDisposed = false;
            m_libraryHandle = IntPtr.Zero;

            m_throwOnLoadFailure = true;
        }

        ~UnmanagedLibraryImplementation()
        {
            Dispose(false);
        }

        public T GetFunction<T>(string functionName) where T : class
        {
            if(string.IsNullOrEmpty(functionName))
                return null;

            Delegate function;
            if(!m_nameToUnmanagedFunction.TryGetValue(functionName, out function))
                return null;

            object obj = (object) function;

            return (T) obj;
        }

        public bool LoadLibrary(string path)
        {
            FreeLibrary(true);

            m_libraryHandle = NativeLoadLibrary(path);

            if(m_libraryHandle != IntPtr.Zero)
                LoadFunctions();

            return m_libraryHandle != IntPtr.Zero;
        }

        public bool FreeLibrary()
        {
            return FreeLibrary(true);
        }

        private bool FreeLibrary(bool clearFunctions)
        {
            if(m_libraryHandle != IntPtr.Zero)
            {
                NativeFreeLibrary(m_libraryHandle);
                m_libraryHandle = IntPtr.Zero;

                if(clearFunctions)
                    m_nameToUnmanagedFunction.Clear();

                return true;
            }

            return false;
        }

        private void LoadFunctions()
        {
            foreach(Type funcType in m_unmanagedFunctionDelegateTypes)
            {
                string funcName = GetUnmanagedName(funcType);
                if(string.IsNullOrEmpty(funcName))
                {
                    System.Diagnostics.Debug.Assert(false,
                        $"No UnmanagedFunctionNameAttribute on {funcType.AssemblyQualifiedName} type.");
                    continue;
                }

                IntPtr procAddr = NativeGetProcAddress(m_libraryHandle, funcName);
                if(procAddr == IntPtr.Zero)
                {
                    System.Diagnostics.Debug.Assert(false,
                        $"No unmanaged function found for {funcType.AssemblyQualifiedName} type.");
                    continue;
                }

                Delegate function;
                if(!m_nameToUnmanagedFunction.TryGetValue(funcName, out function))
                {
                    function = Marshal.GetDelegateForFunctionPointer(procAddr, funcType);
                    m_nameToUnmanagedFunction.Add(funcName, function);
                }
            }
        }

        private string GetUnmanagedName(Type funcType)
        {
            return funcType.GetCustomAttribute<UnmanagedFunctionNameAttribute>(false)?.UnmanagedFunctionName;
        }

        protected abstract IntPtr NativeLoadLibrary(string path);
        protected abstract void NativeFreeLibrary(IntPtr handle);
        protected abstract IntPtr NativeGetProcAddress(IntPtr handle, string functionName);

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if(!m_isDisposed)
            {
                FreeLibrary(isDisposing);

                m_isDisposed = true;
            }
        }
    }
}
