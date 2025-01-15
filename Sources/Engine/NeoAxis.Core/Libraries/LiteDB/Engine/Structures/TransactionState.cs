#if !NO_LITE_DB
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB.Engine
{
    internal enum TransactionState
    {
        Active,
        Committed,
        Aborted,
        Disposed
    }
}
#endif