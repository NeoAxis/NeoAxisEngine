using System.IO;
using Internal.DotRecast.Core;

namespace Internal.DotRecast.Detour.Io
{
    public class DtNavMeshParamWriter
    {
        public void Write(BinaryWriter stream, in DtNavMeshParams option, RcByteOrder order)
        {
            RcIO.Write(stream, option.orig.X, order);
            RcIO.Write(stream, option.orig.Y, order);
            RcIO.Write(stream, option.orig.Z, order);
            RcIO.Write(stream, option.tileWidth, order);
            RcIO.Write(stream, option.tileHeight, order);
            RcIO.Write(stream, option.maxTiles, order);
            RcIO.Write(stream, option.maxPolys, order);
        }
    }
}