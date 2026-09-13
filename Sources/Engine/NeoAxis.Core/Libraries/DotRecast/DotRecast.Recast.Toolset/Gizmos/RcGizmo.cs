using NeoAxis.DotRecast.Detour.Dynamic.Colliders;

namespace NeoAxis.DotRecast.Recast.Toolset.Gizmos
{
    public class RcGizmo
    {
        public readonly IRcGizmoMeshFilter Gizmo;
        public readonly IDtCollider Collider;

        public RcGizmo(IDtCollider collider, IRcGizmoMeshFilter gizmo)
        {
            Collider = collider;
            Gizmo = gizmo;
        }
    }
}