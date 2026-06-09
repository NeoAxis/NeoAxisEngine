using Internal.DotRecast.Core.Numerics;

namespace Internal.DotRecast.Detour.Extras.Jumplink
{
    public interface IDtTrajectory
    {
        RcVec3f Apply(RcVec3f start, RcVec3f end, float u);
    }
}