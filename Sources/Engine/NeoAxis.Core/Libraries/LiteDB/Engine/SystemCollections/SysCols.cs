#if !NO_LITE_DB
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB.Engine
{
    public partial class LiteEngine
    {
        private IEnumerable<BsonDocument> SysCols()
        {
            foreach (var col in _header.GetCollections())
            {
                yield return new BsonDocument
                {
                    ["name"] = col.Key,
                    ["type"] = "user"
                };
            }

            foreach (var item in _systemCollections)
            {
                yield return new BsonDocument
                {
                    ["name"] = item.Key,
                    ["type"] = "system"
                };
            }

        }
    }
}
#endif