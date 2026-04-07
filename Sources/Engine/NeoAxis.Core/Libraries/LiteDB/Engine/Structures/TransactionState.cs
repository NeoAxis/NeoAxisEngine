#if !NO_LITE_DB
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using static NeoAxis.LiteDB.Constants;

namespace NeoAxis.LiteDB.Engine
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