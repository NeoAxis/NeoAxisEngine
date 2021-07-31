using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp
{
	public class ConeShape : ConvexInternalShape
	{
		internal ConeShape(IntPtr native)
			: base(native)
		{
		}

		public ConeShape(double radius, double height)
			: base(btConeShape_new(radius, height))
		{
		}

		public int ConeUpIndex
		{
			get => btConeShape_getConeUpIndex(Native);
			set => btConeShape_setConeUpIndex(Native, value);
		}

		public double Height
		{
			get => btConeShape_getHeight(Native);
			set => btConeShape_setHeight(Native, value);
		}

		public double Radius
		{
			get => btConeShape_getRadius(Native);
			set => btConeShape_setRadius(Native, value);
		}
	}

	public class ConeShapeX : ConeShape
	{
		public ConeShapeX(double radius, double height)
			: base(btConeShapeX_new(radius, height))
		{
		}
	}

	public class ConeShapeZ : ConeShape
	{
		public ConeShapeZ(double radius, double height)
			: base(btConeShapeZ_new(radius, height))
		{
		}
	}
}
