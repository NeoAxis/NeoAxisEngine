#if !NO_LITE_DB
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB
{
    internal enum DocumentScope
    {
        Source,
        Root,
        Current
    }
}

#endif