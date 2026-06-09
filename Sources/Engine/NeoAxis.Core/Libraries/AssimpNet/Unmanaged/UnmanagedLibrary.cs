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
using System.IO;
using System.Runtime.InteropServices;

namespace Internal.Assimp.Unmanaged
{
    /// <summary>
    /// Represents management and access to an unmanaged library. An unmanaged library can be loaded and unloaded dynamically. The library then searches for a list
    /// of exported functions to create managed delegates for, allowing callers to access the library. Each OS platform has its own implementation to determine how to load
    /// unmanaged libraries.
    /// </summary>
    public abstract class UnmanagedLibrary
    {
        private static object s_defaultLoadSync = new object();

        private UnmanagedLibraryImplementation m_impl;
        private UnmanagedLibraryResolver m_resolver;
        private string m_libraryPath = string.Empty;
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
        public bool IsLibraryLoaded => m_impl.IsLibraryLoaded;

        /// <summary>
        /// Gets the default name of the unmanaged library DLL. This is dependent based on the platform extension and name prefix. Additional
        /// names can be set in the <see cref="UnmanagedLibraryResolver"/> (e.g. to load versioned DLLs)
        /// </summary>
        public string DefaultLibraryName => m_impl.DefaultLibraryName;

        /// <summary>
        /// Gets the path to the unmanaged library DLL that is currently loaded.
        /// </summary>
        public string LibraryPath => m_libraryPath;

        /// <summary>
        /// Gets the resolver used to find the unmanaged library DLL when loading.
        /// </summary>
        public UnmanagedLibraryResolver Resolver => m_resolver;

        /// <summary>
        /// Gets or sets whether an <see cref="AssimpException"/> is thrown if the unmanaged DLL fails to load for whatever reason. By
        /// default this is true.
        /// </summary>
        public bool ThrowOnLoadFailure
        {
            get => m_impl.ThrowOnLoadFailure;
            set => m_impl.ThrowOnLoadFailure = value;
        }

        /// <summary>
        /// Queries if the OS is 64-bit, if false then it is 32-bit.
        /// </summary>
        public static bool Is64Bit => IntPtr.Size == 8;

        /// <summary>
        /// Constructs a new <see cref="UnmanagedLibrary"/>.
        /// </summary>
        /// <param name="defaultName">Default name (NOT path) of the unmanaged library.</param>
        /// <param name="unmanagedFunctionDelegateTypes">Delegate types to instantiate and load.</param>
        protected UnmanagedLibrary(string defaultName, Type[] unmanagedFunctionDelegateTypes)
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

            //UWP targets in Unity 2021.3.0 with IL2CPP (HoloLens 2, ...) are picked up as being "Microsoft Windows NT
            //xx.y.zzzzz.w", but _not_ as RuntimePlatform Windows above. Should be fixed in 2021.3.2, see:
            //https://issuetracker.unity3d.com/issues/windows-fails-to-identify-as-windows-when-building-the-project-using-il2cpp-scripting-backend
            //https://issuetracker.unity3d.com/issues/uwp-os-version-detection-is-wrong-on-some-windows-versions
            if(RuntimeInformation.OSDescription.Contains("Microsoft Windows"))
                return Platform.Windows;

            throw new InvalidOperationException("Cannot determine OS-specific implementation.");
        }

        /// <summary>
        /// Loads the unmanaged library using the <see cref="UnmanagedLibraryResolver"/>.
        /// </summary>
        /// <returns>True if the library was found and successfully loaded.</returns>
        public bool LoadLibrary()
        {
            string libPath = m_resolver.ResolveLibraryPath(DefaultLibraryName);
            return LoadLibrary(libPath);
        }

        /// <summary>
        /// Loads the unmanaged library using the supplied 32 and 64 bit paths, the one chosen is based on the OS bitness.
        /// </summary>
        /// <param name="lib32Path">Path to the 32-bit DLL</param>
        /// <param name="lib64Path">Path to the 64-bit DLL</param>
        /// <returns>True if the library was found and successfully loaded.</returns>
        public bool LoadLibrary(string lib32Path, string lib64Path)
        {
            return LoadLibrary((Is64Bit) ? lib64Path : lib32Path);
        }

        /// <summary>
        /// Loads the unmanaged library using the supplied path.
        /// </summary>
        /// <param name="libPath">Path to the unmanaged DLL.</param>
        /// <returns>True if the library was found and successfully loaded.</returns>
        public bool LoadLibrary(string libPath)
        {
            if(IsLibraryLoaded)
            {
                //Ignore repeated calls...but do assert
                System.Diagnostics.Debug.Assert(false, "Library already loaded");
                return true;
            }

            //Automatically append extension if necessary
            if(!string.IsNullOrEmpty(libPath) && !Path.HasExtension(libPath))
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
                m_libraryPath = string.Empty;
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
        public T GetFunction<T>(string funcName) where T : class
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

        private void CreateRuntimeImplementation(string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
        {
            Platform platform = GetPlatform();
            m_resolver = new UnmanagedLibraryResolver(platform);
            m_impl = CreateRuntimeImplementationForPlatform(platform, defaultLibName, unmanagedFunctionDelegateTypes);
        }

        private UnmanagedLibraryImplementation CreateRuntimeImplementationForPlatform(Platform platform, string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
        {
            switch(platform)
            {
                case Platform.Windows:
                    return CreateRuntimeImplementationForWindowsPlatform(defaultLibName, unmanagedFunctionDelegateTypes);
                case Platform.Linux:
                    return CreateRuntimeImplementationForLinuxPlatform(defaultLibName, unmanagedFunctionDelegateTypes);
                case Platform.Mac:
                    return CreateRuntimeImplementationForMacPlatform(defaultLibName, unmanagedFunctionDelegateTypes);
            }

            throw new PlatformNotSupportedException();
        }

        private UnmanagedLibraryImplementation CreateRuntimeImplementationForWindowsPlatform(string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
        {
            try
            {
                // If we can't load regular DLLs this will error
                NativeMethods.WinNativeLoadLibrary("non-existent-dll-that-is-never-used.dll");

                return new UnmanagedWin32LibraryImplementation(defaultLibName, unmanagedFunctionDelegateTypes);
            }
            catch (Exception e) when (e is DllNotFoundException || e is EntryPointNotFoundException)
            {
                // Continue with fallback.
            }

            try
            {
                //If we're running in an UWP context, we need to use LoadPackagedLibrary. On non-UWP contexts, this
                //will fail with APPMODEL_ERROR_NO_PACKAGE, so fall back to LoadLibrary.
                NativeMethods.WinUwpLoadLibrary("non-existent-dll-that-is-never-used.dll");

                return new UnmanagedUwpLibraryImplementation(defaultLibName, unmanagedFunctionDelegateTypes);
            }
            catch (Exception e) when (e is DllNotFoundException || e is EntryPointNotFoundException)
            {
                // Continue with fallback.
            }

            throw new PlatformNotSupportedException(
                "Windows system detected, but neither Win32 LoadLibrary nor UWP LoadPackagedLibrary could be " +
                "called, which are necessary to load the Assimp DLL. Your version of Windows is likely not supported."
            );
        }

        private UnmanagedLibraryImplementation CreateRuntimeImplementationForMacPlatform(string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
        {
            return new UnmanagedMacLibraryImplementation(defaultLibName, unmanagedFunctionDelegateTypes);
        }

        private UnmanagedLibraryImplementation CreateRuntimeImplementationForLinuxPlatform(string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
        {
            try 
            {
                NativeMethods.libc6_dlerror();

                return new UnmanagedLinuxLibc6LibraryImplementation(defaultLibName, unmanagedFunctionDelegateTypes);
            }
            catch (Exception e) when (e is DllNotFoundException || e is EntryPointNotFoundException) 
            {
                // Continue with fallback.
            }

            try
            {
                //Recent versions of glibc include the dl* symbols in the main library, and the C library is pretty much
                //always present in applications. Older versions glibc don't include these symbols in libc.so, and
                //require loading libdl separately instead.
                NativeMethods.libdl_dlerror();

                return new UnmanagedLinuxLibdlLibraryImplementation(defaultLibName, unmanagedFunctionDelegateTypes);
            }
            catch (Exception e) when (e is DllNotFoundException || e is EntryPointNotFoundException)
            {
                // Continue with fallback.
            }

            throw new PlatformNotSupportedException(
                "Linux system detected, but neither libc.so.6 nor libdl.so contains symbol " +
                "'dlopen' necessary to load Assimp DLL. Check that either of these so files are " +
                "present on your (Linux) system and correctly expose this symbol."
            );
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true, EntryPoint = "LoadLibrary")]
            public static extern IntPtr WinNativeLoadLibrary(string fileName);

            [DllImport("api-ms-win-core-libraryloader-l2-1-0.dll", SetLastError = true, EntryPoint = "LoadPackagedLibrary")]
            public static extern IntPtr WinUwpLoadLibrary([MarshalAs(UnmanagedType.LPWStr)] string libraryName, int reserved = 0);

            [DllImport("libdl.so", EntryPoint = "dlerror")]
            public static extern IntPtr libdl_dlerror();

            [DllImport("libc.so.6", EntryPoint = "dlerror")]
            public static extern IntPtr libc6_dlerror();
        }
    }
}
