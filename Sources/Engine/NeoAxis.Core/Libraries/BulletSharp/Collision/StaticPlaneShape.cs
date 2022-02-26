using System.Runtime.InteropServices;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class StaticPlaneShape : ConcaveShape
	{
		public StaticPlaneShape(BVector3 planeNormal, double planeConstant)
			: base(btStaticPlaneShape_new(ref planeNormal, planeConstant))
		{
		}

		public double PlaneConstant => btStaticPlaneShape_getPlaneConstant(Native);

		public BVector3 PlaneNormal
		{
			get
			{
				BVector3 value;
				btStaticPlaneShape_getPlaneNormal(Native, out value);
				return value;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct StaticPlaneShapeData
	{
		public CollisionShapeData CollisionShapeData;
		public Vector3FloatData LocalScaling;
		public Vector3FloatData PlaneNormal;
		public float PlaneConstant;
		public int Padding;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(StaticPlaneShapeData), fieldName).ToInt32(); }
	}
}
