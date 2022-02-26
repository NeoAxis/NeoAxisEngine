using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class SphereShape : ConvexInternalShape
	{
		public SphereShape(double radius)
			: base(btSphereShape_new(radius))
		{
		}

		public void SetUnscaledRadius(double radius)
		{
			btSphereShape_setUnscaledRadius(Native, radius);
		}

		public double Radius => btSphereShape_getRadius(Native);
	}
}
