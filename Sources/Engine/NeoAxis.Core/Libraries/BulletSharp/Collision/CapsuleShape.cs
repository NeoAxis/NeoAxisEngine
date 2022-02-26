using System;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class CapsuleShape : ConvexInternalShape
	{
		internal CapsuleShape(IntPtr native)
			: base(native)
		{
		}

		public CapsuleShape(double radius, double height)
			: base(btCapsuleShape_new(radius, height))
		{
		}

		public double HalfHeight => btCapsuleShape_getHalfHeight(Native);

		public double Radius => btCapsuleShape_getRadius(Native);

		public int UpAxis => btCapsuleShape_getUpAxis(Native);
	}

	public class CapsuleShapeX : CapsuleShape
	{
		public CapsuleShapeX(double radius, double height)
			: base(btCapsuleShapeX_new(radius, height))
		{
		}
	}

	public class CapsuleShapeZ : CapsuleShape
	{
		public CapsuleShapeZ(double radius, double height)
			: base(btCapsuleShapeZ_new(radius, height))
		{
		}
	}
}
