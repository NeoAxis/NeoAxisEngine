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
using System.Runtime.InteropServices;

namespace Assimp.Unmanaged
{
    /// <summary>
    /// Enumerates supported platforms.
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Windows platform.
        /// </summary>
        Windows,

        /// <summary>
        /// Linux platform.
        /// </summary>
        Linux,

        /// <summary>
        /// Mac platform.
        /// </summary>
        Mac
    }

    /// <summary>
    /// An attribute that represents the name of an unmanaged function to import.
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate)]
    public class UnmanagedFunctionNameAttribute : Attribute
    {
        private String m_unmanagedFunctionName;

        /// <summary>
        /// Name of the unmanaged function.
        /// </summary>
        public String UnmanagedFunctionName
        {
            get
            {
                return m_unmanagedFunctionName;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="UnmanagedFunctionName"/>.
        /// </summary>
        /// <param name="unmanagedFunctionName">Name of the function.</param>
        public UnmanagedFunctionNameAttribute(String unmanagedFunctionName)
        {
            m_unmanagedFunctionName = unmanagedFunctionName;
        }
    }

    /// <summary>
    /// Represents management and access to an unmanaged library. An unmanaged library can be loaded and unloaded dynamically. The library then searches for a list
    /// of exported functions to create managed delegates for, allowing callers to access the library. Each OS platform has its own implementation to determine how to load
    /// unmanaged libraries.
    /// </summary>
    public abstract class UnmanagedLibrary
    {
        private static Object s_defaultLoadSync = new Object();

        private UnmanagedLibraryImplementation m_impl;
        private UnmanagedLibraryResolver m_resolver;
        private String m_libraryPath = String.Empty;
        private volatile bool m_checkNeedsLoading = true;       

        /// <summary>
        /// Occurs when the unmanaged library is loaded.
        /// </summary>
        public event EventHandler LibraryLoaded;

        /// <summary>
        /// Occurs when the unmanaged library is freed.
        /// </summary>
        public event EventHandler LibraryFreed;

        /// <summary>
        /// Queries if the unmanaged library has been loaded or not.
        /// </summary>
        public bool IsLibraryLoaded
        {
            get
            {
                return m_impl.IsLibraryLoaded;
            }
        }

        /// <summary>
        /// Gets the default name of the unmanaged library DLL. This is dependent based on the platform extension and name prefix. Additional
        /// names can be set in the <see cref="UnmanagedLibraryResolver"/> (e.g. to load versioned DLLs)
        /// </summary>
        public String DefaultLibraryName
        {
            get
            {
                return m_impl.DefaultLibraryName;
            }
        }

        /// <summary>
        /// Gets the path to the unmanaged library DLL that is currently loaded.
        /// </summary>
        public String LibraryPath
        {
            get
            {
                return m_libraryPath;
            }
        }

        /// <summary>
        /// Gets the resolver used to find the unmanaged library DLL when loading.
        /// </summary>
        public UnmanagedLibraryResolver Resolver
        {
            get
            {
                return m_resolver;
            }
        }

        /// <summary>
        /// Gets or sets whether an <see cref="AssimpException"/> is thrown if the unmanaged DLL fails to load for whatever reason. By
        /// default this is true.
        /// </summary>
        public bool ThrowOnLoadFailure
        {
            get
            {
                return m_impl.ThrowOnLoadFailure;
            }
            set
            {
                m_impl.ThrowOnLoadFailure = value;
            }
        }

        /// <summary>
        /// Queries if the OS is 64-bit, if false then it is 32-bit.
        /// </summary>
        public static bool Is64Bit
        {
            get
            {
                return IntPtr.Size == 8;
            }
        }

        /// <summary>
        /// Constructs a new <see cref="UnmanagedLibrary"/>.
        /// </summary>
        /// <param name="defaultName">Default name (NOT path) of the unmanaged library.</param>
        /// <param name="unmanagedFunctionDelegateTypes">Delegate types to instantiate and load.</param>
        protected UnmanagedLibrary(String defaultName, Type[] unmanagedFunctionDelegateTypes)
        {
            CreateRuntimeImplementation(defaultName, unmanagedFunctionDelegateTypes);
        }

        /// <summary>
        /// Gets an enum representing the current OS that is application is executing on.
        /// </summary>
        /// <returns>Platform enumeration.</returns>
        public static Platform GetPlatform()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Platform.Windows;

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return Platform.Linux;

            if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return Platform.Mac;

            throw new InvalidOperationException("Cannot determine OS-specific implementation.");
        }

        /// <summary>
        /// Loads the unmanaged library using the <see cref="UnmanagedLibraryResolver"/>.
        /// </summary>
        /// <returns>True if the library was found and successfully loaded.</returns>
        public bool LoadLibrary()
        {
            String libPath = m_resolver.ResolveLibraryPath(DefaultLibraryName);
            return LoadLibrary(libPath);
        }

        /// <summary>
        /// Loads the unmanaged library using the supplied 32 and 64 bit paths, the one chosen is based on the OS bitness.
        /// </summary>
        /// <param name="lib32Path">Path to the 32-bit DLL</param>
        /// <param name="lib64Path">Path to the 64-bit DLL</param>
        /// <returns>True if the library was found and successfully loaded.</returns>
        public bool LoadLibrary(String lib32Path, String lib64Path)
        {
            return LoadLibrary((Is64Bit) ? lib64Path : lib32Path);
        }

        /// <summary>
        /// Loads the unmanaged library using the supplied path.
        /// </summary>
        /// <param name="libPath">Path to the unmanaged DLL.</param>
        /// <returns>True if the library was found and successfully loaded.</returns>
        public bool LoadLibrary(String libPath)
        {
            if(IsLibraryLoaded)
            {
                //Ignore repeated calls...but do assert
                System.Diagnostics.Debug.Assert(false, "Library already loaded");
                return true;
            }

            //Automatically append extension if necessary
            if(!String.IsNullOrEmpty(libPath) && !Path.HasExtension(libPath))
                libPath = Path.ChangeExtension(libPath, m_impl.DllExtension);

            if(m_impl.LoadLibrary(libPath))
            {
                m_libraryPath = libPath;

                OnLibraryLoaded();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Frees the unmanaged library that is currently loaded.
        /// </summary>
        /// <returns>True if the library was sucessfully freed.</returns>
        public bool FreeLibrary()
        {
            if(IsLibraryLoaded)
            {
                OnLibraryFreed();

                m_impl.FreeLibrary();
                m_libraryPath = String.Empty;
                m_checkNeedsLoading = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a delegate based on the unmanaged function name.
        /// </summary>
        /// <typeparam name="T">Type of delegate.</typeparam>
        /// <param name="funcName">Name of unmanaged function that is exported by the library.</param>
        /// <returns>The delegate, or null if not found.</returns>
        public T GetFunction<T>(String funcName) where T : class
        {
            return m_impl.GetFunction<T>(funcName);
        }

        /// <summary>
        /// If library is not explicitly loaded by user, call this when trying to call an unmanaged function to load the unmanaged library
        /// from the default path. This function is thread safe.
        /// </summary>
        protected void LoadIfNotLoaded()
        {
            //Check the loading flag so we don't have to lock every time we want to talk to the native library...
            if(!m_checkNeedsLoading)
                return;

            lock(s_defaultLoadSync)
            {
                if(!IsLibraryLoaded)
                    LoadLibrary();

                m_checkNeedsLoading = false;
            }
        }

        /// <summary>
        /// Called when the library is loaded.
        /// </summary>
        protected virtual void OnLibraryLoaded()
        {
            EventHandler evt = LibraryLoaded;

            if(evt != null)
                evt(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the library is freed.
        /// </summary>
        protected virtual void OnLibraryFreed()
        {
            EventHandler evt = LibraryFreed;

            if(evt != null)
                evt(this, EventArgs.Empty);
        }

        private void CreateRuntimeImplementation(String defaultLibName, Type[] unmanagedFunctionDelegateTypes)
        {
            Platform platform = GetPlatform();
            m_resolver = new UnmanagedLibraryResolver(platform);

            switch(platform)
            {
                case Platform.Windows:
                    m_impl = new UnmanagedWindowsLibraryImplementation(defaultLibName, unmanagedFunctionDelegateTypes);
                    break;
                case Platform.Linux:
                    m_impl = new UnmanagedLinuxLibraryImplementation(defaultLibName, unmanagedFunctionDelegateTypes);
                    break;
                case Platform.Mac:
                    m_impl = new UnmanagedMacLibraryImplementation(defaultLibName, unmanagedFunctionDelegateTypes);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        #region Base Implementation

        internal abstract class UnmanagedLibraryImplementation : IDisposable
        {
            private String m_defaultLibName;
            private Type[] m_unmanagedFunctionDelegateTypes;
            private Dictionary<String, Delegate> m_nameToUnmanagedFunction;
            private IntPtr m_libraryHandle;
            private bool m_isDisposed;
            private bool m_throwOnLoadFailure;

            public bool IsLibraryLoaded
            {
                get
                {
                    return m_libraryHandle != IntPtr.Zero;
                }
            }

            public bool IsDisposed
            {
                get
                {
                    return m_isDisposed;
                }
            }

            public String DefaultLibraryName
            {
                get
                {
                    return m_defaultLibName;
                }
            }

            public bool ThrowOnLoadFailure
            {
                get
                {
                    return m_throwOnLoadFailure;
                }
                set
                {
                    m_throwOnLoadFailure = value;
                }
            }

            public abstract String DllExtension { get; }

            public virtual String DllPrefix { get { return String.Empty; } }

            public UnmanagedLibraryImplementation(String defaultLibName, Type[] unmanagedFunctionDelegateTypes)
            {
                m_defaultLibName = DllPrefix + Path.ChangeExtension(defaultLibName, DllExtension);

                m_unmanagedFunctionDelegateTypes = unmanagedFunctionDelegateTypes;

                m_nameToUnmanagedFunction = new Dictionary<String, Delegate>();
                m_isDisposed = false;
                m_libraryHandle = IntPtr.Zero;

                m_throwOnLoadFailure = true;
            }

            ~UnmanagedLibraryImplementation()
            {
                Dispose(false);
            }

            public T GetFunction<T>(String functionName) where T : class
            {
                if(String.IsNullOrEmpty(functionName))
                    return null;

                Delegate function;
                if(!m_nameToUnmanagedFunction.TryGetValue(functionName, out function))
                    return null;

                Object obj = (Object) function;

                return (T) obj;
            }

            public bool LoadLibrary(String path)
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
                    String funcName = GetUnmanagedName(funcType);
                    if(String.IsNullOrEmpty(funcName))
                    {
                        System.Diagnostics.Debug.Assert(false, String.Format("No UnmanagedFunctionNameAttribute on {0} type.", funcType.AssemblyQualifiedName));
                        continue;
                    }

                    IntPtr procAddr = NativeGetProcAddress(m_libraryHandle, funcName);
                    if(procAddr == IntPtr.Zero)
                    {
                        System.Diagnostics.Debug.Assert(false, String.Format("No unmanaged function found for {0} type.", funcType.AssemblyQualifiedName));
                        continue;
                    }

                    Delegate function;
                    if(!m_nameToUnmanagedFunction.TryGetValue(funcName, out function))
                    {
                        function = PlatformHelper.GetDelegateForFunctionPointer(procAddr, funcType);
                        m_nameToUnmanagedFunction.Add(funcName, function);
                    }
                }
            }

            private String GetUnmanagedName(Type funcType)
            {
                object[] attributes = PlatformHelper.GetCustomAttributes(funcType, typeof(UnmanagedFunctionNameAttribute), false);
                foreach(object attr in attributes)
                {
                    if(attr is UnmanagedFunctionNameAttribute)
                        return (attr as UnmanagedFunctionNameAttribute).UnmanagedFunctionName;
                }

                return null;
            }

            protected abstract IntPtr NativeLoadLibrary(String path);
            protected abstract void NativeFreeLibrary(IntPtr handle);
            protected abstract IntPtr NativeGetProcAddress(IntPtr handle, String functionName);

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

        #endregion

        #region Windows Implementation

        internal sealed class UnmanagedWindowsLibraryImplementation : UnmanagedLibraryImplementation
        {
            public override String DllExtension
            {
                get
                {
                    return ".dll";
                }
            }

            public UnmanagedWindowsLibraryImplementation(String defaultLibName, Type[] unmanagedFunctionDelegateTypes)
                : base(defaultLibName, unmanagedFunctionDelegateTypes)
            {
            }

            protected override IntPtr NativeLoadLibrary(String path)
            {
                IntPtr libraryHandle = WinLoadLibrary(path);

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
                        throw new AssimpException(String.Format("Error loading unmanaged library from path: {0}\n\n{1}", path, innerException.Message), innerException);
                    else
                        throw new AssimpException(String.Format("Error loading unmanaged library from path: {0}", path));
                }

                return libraryHandle;
            }

            protected override IntPtr NativeGetProcAddress(IntPtr handle, String functionName)
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
            private static extern IntPtr WinLoadLibrary(String fileName);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32.dll")]
            private static extern IntPtr GetProcAddress(IntPtr hModule, String procName);

            #endregion
        }

        #endregion

        #region Linux Implementation

        internal sealed class UnmanagedLinuxLibraryImplementation : UnmanagedLibraryImplementation
        {
            public override String DllExtension
            {
                get
                {
                    return ".so";
                }
            }

            public override String DllPrefix
            {
                get
                {
                    return "lib";
                }
            }

            public UnmanagedLinuxLibraryImplementation(String defaultLibName, Type[] unmanagedFunctionDelegateTypes)
                : base(defaultLibName, unmanagedFunctionDelegateTypes)
            {
            }

            protected override IntPtr NativeLoadLibrary(String path)
            {
                IntPtr libraryHandle = dlopen(path, RTLD_NOW);

                if(libraryHandle == IntPtr.Zero &&  ThrowOnLoadFailure)
                {
                    IntPtr errPtr = dlerror();
                    String msg = Marshal.PtrToStringAnsi(errPtr);
                    if(!String.IsNullOrEmpty(msg))
                        throw new AssimpException(String.Format("Error loading unmanaged library from path: {0}\n\n{1}", path, msg));
                    else
                        throw new AssimpException(String.Format("Error loading unmanaged library from path: {0}", path));
                }

                return libraryHandle;
            }

            protected override IntPtr NativeGetProcAddress(IntPtr handle, String functionName)
            {
                return dlsym(handle, functionName);
            }

            protected override void NativeFreeLibrary(IntPtr handle)
            {
                dlclose(handle);
            }

            #region Native Methods

            [DllImport("libdl.so")]
            private static extern IntPtr dlopen(String fileName, int flags);

            [DllImport("libdl.so")]
            private static extern IntPtr dlsym(IntPtr handle, String functionName);

            [DllImport("libdl.so")]
            private static extern int dlclose(IntPtr handle);

            [DllImport("libdl.so")]
            private static extern IntPtr dlerror();

            private const int RTLD_NOW = 2;

            #endregion
        }

        #endregion

        #region Mac Implementation

        internal sealed class UnmanagedMacLibraryImplementation : UnmanagedLibraryImplementation
        {
            public override String DllExtension
            {
                get
                {
                    return ".dylib";
                }
            }

            public override String DllPrefix
            {
                get
                {
                    return "lib";
                }
            }

            public UnmanagedMacLibraryImplementation(String defaultLibName, Type[] unmanagedFunctionDelegateTypes)
                : base(defaultLibName, unmanagedFunctionDelegateTypes)
            {
            }

            protected override IntPtr NativeLoadLibrary(String path)
            {
                IntPtr libraryHandle = dlopen(path, RTLD_NOW);

                if(libraryHandle == IntPtr.Zero && ThrowOnLoadFailure)
                {
                    IntPtr errPtr = dlerror();
                    String msg = Marshal.PtrToStringAnsi(errPtr);
                    if(!String.IsNullOrEmpty(msg))
                        throw new AssimpException(String.Format("Error loading unmanaged library from path: {0}\n\n{1}", path, msg));
                    else
                        throw new AssimpException(String.Format("Error loading unmanaged library from path: {0}", path));
                }

                return libraryHandle;
            }

            protected override IntPtr NativeGetProcAddress(IntPtr handle, String functionName)
            {
                return dlsym(handle, functionName);
            }

            protected override void NativeFreeLibrary(IntPtr handle)
            {
                dlclose(handle);
            }

            #region Native Methods

            [DllImport("libSystem.B.dylib")]
            private static extern IntPtr dlopen(String fileName, int flags);

            [DllImport("libSystem.B.dylib")]
            private static extern IntPtr dlsym(IntPtr handle, String functionName);

            [DllImport("libSystem.B.dylib")]
            private static extern int dlclose(IntPtr handle);

            [DllImport("libSystem.B.dylib")]
            private static extern IntPtr dlerror();

            private const int RTLD_NOW = 2;

            #endregion
        }

        #endregion
    }
}
