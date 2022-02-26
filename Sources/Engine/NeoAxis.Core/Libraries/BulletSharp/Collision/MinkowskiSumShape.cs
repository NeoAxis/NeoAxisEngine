using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class MinkowskiSumShape : ConvexInternalShape
	{
		public MinkowskiSumShape(ConvexShape shapeA, ConvexShape shapeB)
			: base(btMinkowskiSumShape_new(shapeA.Native, shapeB.Native))
		{
			ShapeA = shapeA;
			ShapeB = shapeB;
		}

		public ConvexShape ShapeA { get; }

		public ConvexShape ShapeB { get; }

		public BMatrix TransformA
		{
			get
			{
				BMatrix value;
				btMinkowskiSumShape_getTransformA(Native, out value);
				return value;
			}
			set => btMinkowskiSumShape_setTransformA(Native, ref value);
		}

		public BMatrix TransformB
		{
			get
			{
				BMatrix value;
				btMinkowskiSumShape_GetTransformB(Native, out value);
				return value;
			}
			set => btMinkowskiSumShape_setTransformB(Native, ref value);
		}
	}
}
