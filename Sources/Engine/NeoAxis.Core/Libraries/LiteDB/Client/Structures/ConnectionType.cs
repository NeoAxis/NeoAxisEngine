#if !NO_LITE_DB
using Internal.LiteDB.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB
{
    public enum ConnectionType
    {
        Direct,
        Shared
        // MimePipes
        // Tcp
        // Rest
    }
}
#endif