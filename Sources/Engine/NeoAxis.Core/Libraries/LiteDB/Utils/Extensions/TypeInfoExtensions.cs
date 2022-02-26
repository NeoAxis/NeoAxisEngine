#if !NO_LITE_DB
using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB
{
    internal static class TypeInfoExtensions
    {
        public static bool IsAnonymousType(this Type type)
        {
            bool isAnonymousType = 
                type.FullName.Contains("AnonymousType") &&
                type.GetTypeInfo().GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();

            return isAnonymousType;
        }

        public static bool IsEnumerable(this Type type)
        {
            return 
                type != typeof(String) &&
                typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}

#endif