using System;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class TriangleShape : PolyhedralConvexShape
	{
		private Vector3Array _vertices;

		internal TriangleShape(IntPtr native)
			: base(native)
		{
		}

		public TriangleShape()
			: base(btTriangleShape_new())
		{
		}

		public TriangleShape(BVector3 p0, BVector3 p1, BVector3 p2)
			: base(btTriangleShape_new2(ref p0, ref p1, ref p2))
		{
		}

		public void CalcNormal(out BVector3 normal)
		{
			btTriangleShape_calcNormal(Native, out normal);
		}

		public void GetPlaneEquation(int i, out BVector3 planeNormal, out BVector3 planeSupport)
		{
			btTriangleShape_getPlaneEquation(Native, i, out planeNormal, out planeSupport);
		}

		public IntPtr GetVertexPtr(int index)
		{
			return btTriangleShape_getVertexPtr(Native, index);
		}

		public Vector3Array Vertices
		{
			get
			{
				if (_vertices == null)
				{
					_vertices = new Vector3Array(btTriangleShape_getVertices1(Native), 3);
				}
				return _vertices;
			}
		}
	}
}
