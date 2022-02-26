using System;
using System.Runtime.InteropServices;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class CylinderShape : ConvexInternalShape
	{
		internal CylinderShape(IntPtr native)
			: base(native)
		{
		}

		public CylinderShape(BVector3 halfExtents)
			: base(btCylinderShape_new(ref halfExtents))
		{
		}

		public CylinderShape(double halfExtentX, double halfExtentY, double halfExtentZ)
			: base(btCylinderShape_new2(halfExtentX, halfExtentY, halfExtentZ))
		{
		}

		public BVector3 HalfExtentsWithMargin
		{
			get
			{
				BVector3 value;
				btCylinderShape_getHalfExtentsWithMargin(Native, out value);
				return value;
			}
		}

		public BVector3 HalfExtentsWithoutMargin
		{
			get
			{
				BVector3 value;
				btCylinderShape_getHalfExtentsWithoutMargin(Native, out value);
				return value;
			}
		}

		public double Radius => btCylinderShape_getRadius(Native);

		public int UpAxis => btCylinderShape_getUpAxis(Native);
	}

	public class CylinderShapeX : CylinderShape
	{
		public CylinderShapeX(BVector3 halfExtents)
			: base(btCylinderShapeX_new(ref halfExtents))
		{
		}

		public CylinderShapeX(double halfExtentX, double halfExtentY, double halfExtentZ)
			: base(btCylinderShapeX_new2(halfExtentX, halfExtentY, halfExtentZ))
		{
		}
	}

	public class CylinderShapeZ : CylinderShape
	{
		public CylinderShapeZ(BVector3 halfExtents)
			: base(btCylinderShapeZ_new(ref halfExtents))
		{
		}

		public CylinderShapeZ(double halfExtentX, double halfExtentY, double halfExtentZ)
			: base(btCylinderShapeZ_new2(halfExtentX, halfExtentY, halfExtentZ))
		{
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct CylinderShapeData
	{
		public ConvexInternalShapeData ConvexInternalShapeData;
		public int UpAxis;
		public int Padding;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(CylinderShapeData), fieldName).ToInt32(); }
	}
}
