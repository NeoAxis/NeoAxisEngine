using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class BoxShape : PolyhedralConvexShape
	{
		public BoxShape(BVector3 boxHalfExtents)
			: base(btBoxShape_new(ref boxHalfExtents))
		{
		}

		public BoxShape(double boxHalfExtent)
			: base(btBoxShape_new2(boxHalfExtent))
		{
		}

		public BoxShape(double boxHalfExtentX, double boxHalfExtentY, double boxHalfExtentZ)
			: base(btBoxShape_new3(boxHalfExtentX, boxHalfExtentY, boxHalfExtentZ))
		{
		}

		public void GetPlaneEquation(out BVector4 plane, int i)
		{
			btBoxShape_getPlaneEquation(Native, out plane, i);
		}

		public BVector3 HalfExtentsWithMargin
		{
			get
			{
				BVector3 value;
				btBoxShape_getHalfExtentsWithMargin(Native, out value);
				return value;
			}
		}

		public BVector3 HalfExtentsWithoutMargin
		{
			get
			{
				BVector3 value;
				btBoxShape_getHalfExtentsWithoutMargin(Native, out value);
				return value;
			}
		}
	}
}
