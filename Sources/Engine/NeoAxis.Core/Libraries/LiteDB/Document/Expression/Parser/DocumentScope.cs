#if !NO_LITE_DB
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static NeoAxis.LiteDB.Constants;

namespace NeoAxis.LiteDB
{
    internal enum DocumentScope
    {
        Source,
        Root,
        Current
    }
}

#endif