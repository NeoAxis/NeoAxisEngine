#if !NO_LITE_DB
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static NeoAxis.LiteDB.Constants;

namespace NeoAxis.LiteDB
{
    internal class ICollectionResolver : EnumerableResolver
    {
        public override string ResolveMethod(MethodInfo method)
        {
            // special Contains method
            switch(method.Name)
            {
                case "Contains": return "# ANY = @0";
            };

            return base.ResolveMethod(method);
        }
    }
}
#endif