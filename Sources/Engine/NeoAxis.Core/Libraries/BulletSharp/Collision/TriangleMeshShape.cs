using System;
using System.Runtime.InteropServices;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class TriangleMeshShape : ConcaveShape
	{
		protected StridingMeshInterface _meshInterface;

		internal TriangleMeshShape(IntPtr native)
			: base(native)
		{
		}

		public void LocalGetSupportingVertex(ref BVector3 vec, out BVector3 value)
		{
			btTriangleMeshShape_localGetSupportingVertex(Native, ref vec, out value);
		}

		public BVector3 LocalGetSupportingVertex(BVector3 vec)
		{
			BVector3 value;
			btTriangleMeshShape_localGetSupportingVertex(Native, ref vec, out value);
			return value;
		}

		public void LocalGetSupportingVertexWithoutMargin(ref BVector3 vec, out BVector3 value)
		{
			btTriangleMeshShape_localGetSupportingVertexWithoutMargin(Native, ref vec,
				out value);
		}

		public BVector3 LocalGetSupportingVertexWithoutMargin(BVector3 vec)
		{
			BVector3 value;
			btTriangleMeshShape_localGetSupportingVertexWithoutMargin(Native, ref vec,
				out value);
			return value;
		}

		public void RecalcLocalAabb()
		{
			btTriangleMeshShape_recalcLocalAabb(Native);
		}

		public BVector3 LocalAabbMax
		{
			get
			{
				BVector3 value;
				btTriangleMeshShape_getLocalAabbMax(Native, out value);
				return value;
			}
		}

		public BVector3 LocalAabbMin
		{
			get
			{
				BVector3 value;
				btTriangleMeshShape_getLocalAabbMin(Native, out value);
				return value;
			}
		}

		public StridingMeshInterface MeshInterface
		{
			get
			{
				if (_meshInterface == null)
				{
					_meshInterface = new StridingMeshInterface(btTriangleMeshShape_getMeshInterface(Native));
				}
				return _meshInterface;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct TriangleMeshShapeData
	{
		public CollisionShapeData CollisionShapeData;
		public StridingMeshInterfaceData MeshInterface;
		public IntPtr QuantizeddoubleBvh;
		public IntPtr QuantizedDoubleBvh;
		public IntPtr TriangleInfoMap;
		public double CollisionMargin;
		public int Pad;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(TriangleMeshShapeData), fieldName).ToInt32(); }
	}
}
