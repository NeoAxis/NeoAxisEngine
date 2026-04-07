#if !NO_LITE_DB
using System;
using static NeoAxis.LiteDB.Constants;

namespace NeoAxis.LiteDB
{
    /// <summary>
    /// Set a name to this property in BsonDocument
    /// </summary>
    public class BsonFieldAttribute : Attribute
    {
        public string Name { get; set; }

        public BsonFieldAttribute(string name)
        {
            this.Name = name;
        }

        public BsonFieldAttribute()
        {
        }
    }
}
#endif