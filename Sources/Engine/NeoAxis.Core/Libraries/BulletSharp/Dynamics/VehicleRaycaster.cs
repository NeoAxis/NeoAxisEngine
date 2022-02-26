using Internal.BulletSharp.Math;

namespace Internal.BulletSharp
{
    public class VehicleRaycasterResult
    {
        public double DistFraction { get; set; }
        public BVector3 HitNormalInWorld { get; set; }
        public BVector3 HitPointInWorld { get; set; }
    }
    
    public interface IVehicleRaycaster
	{
        object CastRay(ref BVector3 from, ref BVector3 to, VehicleRaycasterResult result);
	}
}
