using NeoAxis.DotRecast.Recast;

namespace NeoAxis.DotRecast.Detour.Extras.Jumplink
{
    public interface IDtGroundSampler
    {
        void Sample(DtJumpLinkBuilderConfig acfg, RcBuilderResult result, DtEdgeSampler es);
    }
}