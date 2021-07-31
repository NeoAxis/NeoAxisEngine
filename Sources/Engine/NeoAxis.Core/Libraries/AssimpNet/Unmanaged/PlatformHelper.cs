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

namespace Assimp.Unmanaged
{
#if !NETSTANDARD1_3
    internal enum OSPlatform
    {
        Windows = 0,
        Linux = 1,
        OSX = 2
    }

    internal static class RuntimeInformation
    {
        private static OSPlatform s_platform;

        static RuntimeInformation()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    if (Directory.Exists("/Applications") && Directory.Exists("/System") && Directory.Exists("/Users") && Directory.Exists("/Volumes"))
                        s_platform = OSPlatform.OSX;
                    else
                        s_platform = OSPlatform.Linux;
                    break;
                case PlatformID.MacOSX:
                    s_platform = OSPlatform.OSX;
                    break;
                default:
                    s_platform = OSPlatform.Windows;
                    break;
            }
        }

        public static bool IsOSPlatform(OSPlatform osPlat)
        {
            return s_platform == osPlat;
        }
    }
#endif

    //Helper class for making it easier to access certain reflection methods on types between .Net framework and .Net standard (pre-netstandard 2.0)
    internal class PlatformHelper
    {
        public static String GetInformationalVersion()
        {
#if NETSTANDARD1_3
            AssemblyInformationalVersionAttribute attr = typeof(PlatformHelper).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return (attr != null) ? attr.InformationalVersion : null;
#else
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            if(attributes == null || attributes.Length == 0)
                return null;

            AssemblyInformationalVersionAttribute attr = attributes[0] as AssemblyInformationalVersionAttribute;
            return (attr != null) ? attr.InformationalVersion : null;
#endif
        }

        public static String GetAssemblyName()
        {
#if NETSTANDARD1_3
            return typeof(PlatformHelper).GetTypeInfo().Assembly.GetName().Name;
#else
            return Assembly.GetExecutingAssembly().GetName().Name;
#endif
        }

        public static String GetAppBaseDirectory()
        {
#if NETSTANDARD1_3
            return AppContext.BaseDirectory;
#else
            return AppDomain.CurrentDomain.BaseDirectory;
#endif
        }

        public static bool IsAssignable(Type baseType, Type subType)
        {
            if(baseType == null || subType == null)
                return false;

#if NETSTANDARD1_3
            return baseType.GetTypeInfo().IsAssignableFrom(subType.GetTypeInfo());
#else
            return baseType.IsAssignableFrom(subType);
#endif
        }

        public static Type[] GetNestedTypes(Type type)
        {
            if(type == null)
                return new Type[0];

#if NETSTANDARD1_3
            IEnumerable<TypeInfo> typeInfos = type.GetTypeInfo().DeclaredNestedTypes;
            List<Type> types = new List<Type>();
            foreach(TypeInfo typeInfo in typeInfos)
                types.Add(typeInfo.AsType());

            return types.ToArray();
#else
            return type.GetNestedTypes();
#endif
        }

        public static Object[] GetCustomAttributes(Type type, Type attributeType, bool inherit)
        {
            if(type == null || attributeType == null)
                return new Object[0];

#if NETSTANDARD1_3
            IEnumerable<Attribute> attributes = type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
            List<Object> list = new List<Object>();
            list.AddRange(attributes);

            return list.ToArray();
#else
            return type.GetCustomAttributes(attributeType, inherit);
#endif
        }

        //These methods are marked obsolete in netstandard and net451+
#if NETSTANDARD1_3
#pragma warning disable CS0618
#endif

        public static Delegate GetDelegateForFunctionPointer(IntPtr procAddress, Type delegateType)
        {
            if(procAddress == IntPtr.Zero || delegateType == null)
                return null;

            return Marshal.GetDelegateForFunctionPointer(procAddress, delegateType);
        }

        public static IntPtr GetFunctionPointerForDelegate(Delegate func)
        {
            if(func == null)
                return IntPtr.Zero;
            
            return Marshal.GetFunctionPointerForDelegate(func);
        }

#if NETSTANDARD1_3
#pragma warning restore CS0618
#endif
    }
}
