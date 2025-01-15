#if !NO_LITE_DB
using System;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB
{
    /// <summary>
    /// Indicate which constructor method will be used in this entity
    /// </summary>
    public class BsonCtorAttribute : Attribute
    {
    }
}
#endif