#if !NO_LITE_DB
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB.Engine
{
    public partial class LiteEngine
    {
        private IEnumerable<BsonDocument> SysSequences()
        {
            var values = _sequences.ToArray();

            foreach(var value in values)
            {
                yield return new BsonDocument
                {
                    ["collection"] = value.Key,
                    ["value"] = value.Value
                };
            }
        }
    }
}
#endif