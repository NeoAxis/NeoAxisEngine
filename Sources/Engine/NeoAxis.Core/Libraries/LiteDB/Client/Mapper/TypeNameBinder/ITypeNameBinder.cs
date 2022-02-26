#if !NO_LITE_DB
using System;

namespace Internal.LiteDB
{
    public interface ITypeNameBinder
    {
        string GetName(Type type);
        Type GetType(string name);
    }
}
#endif