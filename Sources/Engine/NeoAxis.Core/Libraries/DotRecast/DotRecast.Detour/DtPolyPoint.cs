using Internal.DotRecast.Core.Numerics;

namespace Internal.DotRecast.Detour
{
    public readonly struct DtPolyPoint
    {
        public readonly long refs;
        public readonly RcVec3f pt;

        public DtPolyPoint(long polyRefs, RcVec3f polyPt)
        {
            refs = polyRefs;
            pt = polyPt;
        }
    }
}