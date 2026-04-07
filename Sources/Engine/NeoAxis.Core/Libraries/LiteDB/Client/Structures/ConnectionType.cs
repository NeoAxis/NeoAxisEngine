#if !NO_LITE_DB
using NeoAxis.LiteDB.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using static NeoAxis.LiteDB.Constants;

namespace NeoAxis.LiteDB
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