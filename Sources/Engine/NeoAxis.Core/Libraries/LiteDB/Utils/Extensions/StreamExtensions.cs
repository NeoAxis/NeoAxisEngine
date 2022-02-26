#if !NO_LITE_DB
using System;
using System.IO;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// If Stream are FileStream, flush content direct to disk (avoid OS cache)
        /// </summary>
        public static void FlushToDisk(this Stream stream)
        {
            if (stream is FileStream fstream)
            {
                fstream.Flush(true);
            }
            else
            {
                stream.Flush();
            }
        }
    }
}
#endif