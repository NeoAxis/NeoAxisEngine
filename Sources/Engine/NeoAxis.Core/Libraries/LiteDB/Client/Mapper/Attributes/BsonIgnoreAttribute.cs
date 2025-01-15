#if !NO_LITE_DB
using System;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB
{
    /// <summary>
    /// Indicate that property will not be persist in Bson serialization
    /// </summary>
    public class BsonIgnoreAttribute : Attribute
    {
    }
}
#endif