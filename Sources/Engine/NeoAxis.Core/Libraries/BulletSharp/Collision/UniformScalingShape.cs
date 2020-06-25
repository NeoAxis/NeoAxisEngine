using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp
{
	public class UniformScalingShape : ConvexShape
	{
		public UniformScalingShape(ConvexShape convexChildShape, double uniformScalingFactor)
			: base(btUniformScalingShape_new(convexChildShape.Native, uniformScalingFactor))
		{
			ChildShape = convexChildShape;
		}

		public ConvexShape ChildShape { get; }

		public double UniformScalingFactor
		{
			get { return btUniformScalingShape_getUniformScalingFactor(Native); }
		}
	}
}
