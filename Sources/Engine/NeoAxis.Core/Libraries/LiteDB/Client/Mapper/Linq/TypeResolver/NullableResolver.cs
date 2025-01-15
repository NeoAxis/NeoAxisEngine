#if !NO_LITE_DB
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB
{
    internal class NullableResolver : ITypeResolver
    {
        public string ResolveMethod(MethodInfo method)
        {
            return null;
        }

        public string ResolveMember(MemberInfo member)
        {
            switch (member.Name)
            {
                case "HasValue": return "(IS_NULL(#) = false)";
                case "Value": return "#";
            }

            return null;
        }

        public string ResolveCtor(ConstructorInfo ctor) => null;
    }
}

#endif