using Internal.DotRecast.Detour.Dynamic.Colliders;

namespace Internal.DotRecast.Recast.Toolset.Gizmos
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