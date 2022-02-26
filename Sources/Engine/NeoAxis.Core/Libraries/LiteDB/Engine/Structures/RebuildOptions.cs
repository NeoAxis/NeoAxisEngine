#if !NO_LITE_DB
using System;
using System.Collections.Generic;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB.Engine
{
    /// <summary>
    /// </summary>
    public class RebuildOptions
    {
        /// <summary>
        /// Rebuild database with a new password
        /// </summary>
        public string Password { get; set; } = null;

        /// <summary>
        /// Define a new collation when rebuild
        /// </summary>
        public Collation Collation { get; set; } = null;
    }
}
#endif