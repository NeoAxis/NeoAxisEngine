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
using System.Reflection;
using System.Runtime.InteropServices;

namespace Internal.Assimp.Unmanaged
{
    /// <summary>
    /// Resolves unmanaged DLLs for <see cref="UnmanagedLibrary"/>. The process is completely configurable, where the user can supply alternative library names (e.g. versioned libs),
    /// an override library name, and probing paths. These can be set for both 32/64 bit, or seperately for 32 or 64 bit. See <see cref="UnmanagedLibraryResolver.ResolveLibraryPath(string)"/>
    /// for the search strategy.
    /// </summary>
    public sealed class UnmanagedLibraryResolver
    {
        private string[] m_32BitLibNames;
        private string[] m_64BitLibNames;

        private string[] m_32BitProbingPaths;
        private string[] m_64BitProbingPaths;

        private string m_override32BitName;
        private string m_override64BitName;

        private Platform m_platform;

        /// <summary>
        /// Gets the platform that the application is running on. 
        /// </summary>
        public Platform Platform => m_platform;

        /// <summary>
        /// Constructs a new instance of the <see cref="UnmanagedLibraryResolver"/> class.
        /// </summary>
        /// <param name="platformHint">Platform we're resolving binaries for.</param>
        internal UnmanagedLibraryResolver(Platform platformHint)
        {
            m_platform = platformHint;
        }

        /// <summary>
        /// Sets the collection of fallback library names (e.g. versioned libs) for 32-bit probing.
        /// </summary>
        /// <param name="fallbackLibNames">Null to clear, or set of fallback library names.</param>
        public void SetFallbackLibraryNames32(params string[] fallbackLibNames)
        {
            m_32BitLibNames = fallbackLibNames;
        }

        /// <summary>
        /// Sets the collection of fallback library names (e.g. versioned libs) for 64-bit probing.
        /// </summary>
        /// <param name="fallbackLibNames">Null to clear, or set of fallback library names.</param>
        public void SetFallbackLibraryNames64(params string[] fallbackLibNames)
        {
            m_64BitLibNames = fallbackLibNames;
        }

        /// <summary>
        /// Sets the collection of fallback library names (e.g. versioned libs) for both 32-bit and 64-bit probing.
        /// </summary>
        /// <param name="fallbackLibNames">Null to clear, or set of fallback library names.</param>
        public void SetFallbackLibraryNames(params string[] fallbackLibNames)
        {
            m_32BitLibNames = fallbackLibNames;
            m_64BitLibNames = fallbackLibNames;
        }

        /// <summary>
        /// Sets the collection of file paths to probe for 32-bit libraries. These paths always are first to be searched, in the order
        /// that they are given.
        /// </summary>
        /// <param name="probingPaths">Null to clear, or set of paths to probe.</param>
        public void SetProbingPaths32(params string[] probingPaths)
        {
            m_32BitProbingPaths = probingPaths;
        }

        /// <summary>
        /// Sets the collection of file paths to probe for 64-bit libraries. These paths always are first to be searched, in the order
        /// that they are given.
        /// </summary>
        /// <param name="probingPaths">Null to clear, or set of paths to probe.</param>
        public void SetProbingPaths64(params string[] probingPaths)
        {
            m_64BitProbingPaths = probingPaths;
        }

        /// <summary>
        /// Sets the collection of file paths to probe for both 32-bit and 64-bit libraries. These paths always are first to be searched, in the order
        /// that they are given.
        /// </summary>
        /// <param name="probingPaths">Null to clear, or set of paths to probe.</param>
        public void SetProbingPaths(params string[] probingPaths)
        {
            m_32BitProbingPaths = probingPaths;
            m_64BitProbingPaths = probingPaths;
        }

        /// <summary>
        /// Sets an override 32-bit library name. By default, the <see cref="UnmanagedLibrary"/> implementations creates a default name for the library, which
        /// is passed into <see cref="ResolveLibraryPath(string)"/> for resolving. If the override is non-null, it will be used instead. This is useful if the library
        /// to be loaded is not conforming to the platform's default prefix/extension scheme (e.g. libXYZ.so on linux where "lib" is the prefix and ".so" the extension).
        /// </summary>
        /// <param name="overrideName">Null to clear, or override library name.</param>
        public void SetOverrideLibraryName32(string overrideName)
        {
            m_override32BitName = overrideName;
        }

        /// <summary>
        /// Sets an override 64-bit library name. By default, the <see cref="UnmanagedLibrary"/> implementations creates a default name for the library, which
        /// is passed into <see cref="ResolveLibraryPath(string)"/> for resolving. If the override is non-null, it will be used instead. This is useful if the library
        /// to be loaded is not conforming to the platform's default prefix/extension scheme (e.g. libXYZ.so on linux where "lib" is the prefix and ".so" the extension).
        /// </summary>
        /// <param name="overrideName">Null to clear, or override library name.</param>
        public void SetOverrideLibraryName64(string overrideName)
        {
            m_override64BitName = overrideName;
        }

        /// <summary>
        /// Sets an override 32-bit and 64-bit library name. By default, the <see cref="UnmanagedLibrary"/> implementations creates a default name for the library, which
        /// is passed into <see cref="ResolveLibraryPath(string)"/> for resolving. If the override is non-null, it will be used instead. This is useful if the library
        /// to be loaded is not conforming to the platform's default prefix/extension scheme (e.g. libXYZ.so on linux where "lib" is the prefix and ".so" the extension).
        /// </summary>
        /// <param name="overrideName">Null to clear, or override library name.</param>
        public void SetOverrideLibraryName(string overrideName)
        {
            m_override32BitName = overrideName;
            m_override64BitName = overrideName;
        }

        /// <summary>
        /// Given a library name, this function attempts to resolve the file path from which it can be loaded. Each step of the search strategy uses the fallback
        /// library names if the given name was not found in the current step. If the search is unsuccessfully, the library name is returned which means the OS will try
        /// and do its own search strategy when attempting to load the library (this is dependent on the OS). The search strategy is the following, in order of execution:
        /// <para>
        /// <list type="number">
        /// <item><description>Search user-specified probing paths.</description></item>
        /// <item><description>Search {AppBaseDirectory}/runtimes/{RID}/native/.</description></item>
        /// <item><description>Search {AppBaseDirectory}/.</description></item>
        /// <item><description>Search nuget package path, e.g. {UserProfile}/.nuget/packages/{PackageId}/{PackageVersion}/runtimes/{RID}/native/.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// The search strategy gives priority to user-specified probing paths, then local paths to the application, then finally examining the global nuget cache. The RID
        /// is the Runtime Identifier based on the platform/architecture, see also <a href="https://docs.microsoft.com/en-us/dotnet/core/rid-catalog">Microsoft's RID Catalog</a>.
        /// </para>
        /// </summary>
        /// <param name="libName">Name of the library to attempt to resolve.</param>
        /// <returns>Full file path to the library, or the file name if not found (e.g. "libXYZ.so").</returns>
        public string ResolveLibraryPath(string libName)
        {
            //Determine bitness to control which names + paths we use
            bool is64Bit = UnmanagedLibrary.Is64Bit;

            //If any override name, replace incoming name with that
            string overrideName = (is64Bit) ? m_override64BitName : m_override32BitName;
            if(!string.IsNullOrEmpty(overrideName))
                libName = overrideName;

            //If incoming lib name does not exist, abort
            if(string.IsNullOrEmpty(libName))
                return libName;

            //Pick fallbacks and proving paths
            string[] fallbackNames = (is64Bit) ? m_64BitLibNames : m_32BitLibNames;
            string[] probingPaths = (is64Bit) ? m_64BitProbingPaths : m_32BitProbingPaths;
            string rid = GetRuntimeIdentifier();

            return ResolveLibraryPathInternal(libName, rid, fallbackNames, probingPaths);
        }

        private string ResolveLibraryPathInternal(string libName, string rid, string[] fallbackNames, string[] probingPaths)
        {
            //Check user-specified probing paths
            if(probingPaths != null)
            {
                foreach(string probingPath in probingPaths)
                {
                    string potentialPath = TryGetExistingFile(probingPath, libName, fallbackNames);
                    if(!string.IsNullOrEmpty(potentialPath))
                        return potentialPath;
                }
            }

            //Check runtimes folder based on RID
            string runtimeFolder = Path.Combine(AppContext.BaseDirectory, Path.Combine("runtimes", Path.Combine(rid, "native")));
            if(Directory.Exists(runtimeFolder))
            {
                string potentialPath = TryGetExistingFile(runtimeFolder, libName, fallbackNames);
                if(!string.IsNullOrEmpty(potentialPath))
                    return potentialPath;
            }

            //Check base directory
            string pathInAppFolder = TryGetExistingFile(AppContext.BaseDirectory, libName, fallbackNames);
            if(!string.IsNullOrEmpty(pathInAppFolder))
                return pathInAppFolder;

            //Check nuget path
            string nugetRidFolder = GetPackageRuntimeFolder(GetNugetPackagePath(), rid);
            string pathInNugetCache = TryGetExistingFile(nugetRidFolder, libName, fallbackNames);
            if(!string.IsNullOrEmpty(pathInNugetCache))
                return pathInNugetCache;

            //Resolve failed, just return the lib name as we received it and hope the OS can resolve it
            return libName;
        }

        private string GetPackageRuntimeFolder(string packagePath, string rid)
        {
            if(string.IsNullOrEmpty(packagePath))
                return null;

            return Path.Combine(packagePath, Path.Combine("runtimes", Path.Combine(rid, "native")));
        }

        private string GetNugetPackagePath()
        {
            //Resolve packageId based on assembly informational version
            Assembly assembly = Assembly.GetExecutingAssembly();
            string packageId = assembly.GetName().Name.ToLowerInvariant();
            string packageVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            if(string.IsNullOrEmpty(packageId) || string.IsNullOrEmpty(packageVersion))
                return null;

            string nugetPackage = Path.Combine(packageId, packageVersion);

            //Check if a non-default path is set as an environment variable
            string packageDir = Environment.GetEnvironmentVariable("NUGET_PACKAGES");

            //Make sure its valid and exists, otherwise we'll try the default location
            if(!string.IsNullOrEmpty(packageDir) && Directory.Exists(packageDir))
                return Path.Combine(packageDir, nugetPackage);

            //Build a path to the default cache in the user's folder
            if(m_platform == Platform.Windows)
            {
                packageDir = Environment.GetEnvironmentVariable("USERPROFILE");
            }
            else
            {
                packageDir = Environment.GetEnvironmentVariable("HOME");
            }

            if(!string.IsNullOrEmpty(packageDir))
                return Path.Combine(packageDir, Path.Combine(Path.Combine(".nuget", "packages"), nugetPackage));

            return null;
        }

        private string TryGetExistingFile(string basePath, string libName, string[] fallbackNames)
        {
            //Do not attempt if the base directory does not exist
            if(string.IsNullOrEmpty(basePath) || !Directory.Exists(basePath))
                return null;

            //Check the initial lib name
            string combinedPath = Path.Combine(basePath, libName);
            if(File.Exists(combinedPath))
                return combinedPath;

            //If not found, fallback to any other names
            if(fallbackNames != null)
            {
                foreach(string fallbackName in fallbackNames)
                {
                    combinedPath = Path.Combine(basePath, fallbackName);
                    if(File.Exists(combinedPath))
                        return combinedPath;
                }
            }

            return null;
        }

        private string GetRuntimeIdentifier()
        {
            return $"{GetRIDOS()}-{GetRIDArch()}";
        }

        private string GetRIDOS()
        {
            switch(m_platform)
            {
                case Platform.Windows:
                    return "win";
                case Platform.Mac:
                    return "osx";
                case Platform.Linux:
                    return "linux";
            }

            return "unknown";
        }

        private string GetRIDArch()
        {
            switch(RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.Arm:
                    return "arm";
                case Architecture.Arm64:
                    return "arm64";
                case Architecture.X86:
                    return "x86";
                case Architecture.X64:
                default:
                    return "x64";
            }
        }
    }
}
