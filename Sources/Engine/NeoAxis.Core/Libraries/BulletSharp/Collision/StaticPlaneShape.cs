using System.Runtime.InteropServices;
using BulletSharp.Math;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp
{
	public class StaticPlaneShape : ConcaveShape
	{
		public StaticPlaneShape(Vector3 planeNormal, double planeConstant)
			: base(btStaticPlaneShape_new(ref planeNormal, planeConstant))
		{
		}

		public double PlaneConstant => btStaticPlaneShape_getPlaneConstant(Native);

		public Vector3 PlaneNormal
		{
			get
			{
				Vector3 value;
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
