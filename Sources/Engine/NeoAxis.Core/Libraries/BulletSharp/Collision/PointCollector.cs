using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class PointCollector : DiscreteCollisionDetectorInterface.Result
	{
		public PointCollector()
			: base(btPointCollector_new())
		{
		}

		public double Distance
		{
			get => btPointCollector_getDistance(Native);
			set => btPointCollector_setDistance(Native, value);
		}

		public bool HasResult
		{
			get => btPointCollector_getHasResult(Native);
			set => btPointCollector_setHasResult(Native, value);
		}

		public BVector3 NormalOnBInWorld
		{
			get
			{
				BVector3 value;
				btPointCollector_getNormalOnBInWorld(Native, out value);
				return value;
			}
			set => btPointCollector_setNormalOnBInWorld(Native, ref value);
		}

		public BVector3 PointInWorld
		{
			get
			{
				BVector3 value;
				btPointCollector_getPointInWorld(Native, out value);
				return value;
			}
			set => btPointCollector_setPointInWorld(Native, ref value);
		}
	}
}
