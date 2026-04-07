#if !NO_LITE_DB
using System;
using static NeoAxis.LiteDB.Constants;

namespace NeoAxis.LiteDB
{
    /// <summary>
    /// Indicate which constructor method will be used in this entity
    /// </summary>
    public class BsonCtorAttribute : Attribute
    {
    }
}
#endif