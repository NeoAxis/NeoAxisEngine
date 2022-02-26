using System;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class TriangleMesh : TriangleIndexVertexArray
	{
		internal TriangleMesh(IntPtr native)
			: base(native)
		{
		}

		public TriangleMesh(bool use32BitIndices = true, bool use4ComponentVertices = true)
			: base(btTriangleMesh_new(use32BitIndices, use4ComponentVertices))
		{
		}

		public void AddIndex(int index)
		{
			btTriangleMesh_addIndex(Native, index);
		}

	   public void AddTriangleRef(ref BVector3 vertex0, ref BVector3 vertex1, ref BVector3 vertex2,
		   bool removeDuplicateVertices = false)
	   {
		   btTriangleMesh_addTriangle(Native, ref vertex0, ref vertex1, ref vertex2,
			   removeDuplicateVertices);
	   }

		public void AddTriangle(BVector3 vertex0, BVector3 vertex1, BVector3 vertex2,
			bool removeDuplicateVertices = false)
		{
			btTriangleMesh_addTriangle(Native, ref vertex0, ref vertex1, ref vertex2,
				removeDuplicateVertices);
		}

		public void AddTriangleIndices(int index1, int index2, int index3)
		{
			btTriangleMesh_addTriangleIndices(Native, index1, index2, index3);
		}

		public int FindOrAddVertexRef(BVector3 vertex, bool removeDuplicateVertices)
		{
			return btTriangleMesh_findOrAddVertex(Native, ref vertex, removeDuplicateVertices);
		}

		public int FindOrAddVertex(BVector3 vertex, bool removeDuplicateVertices)
		{
			return btTriangleMesh_findOrAddVertex(Native, ref vertex, removeDuplicateVertices);
		}

		public int NumTriangles => btTriangleMesh_getNumTriangles(Native);

		public bool Use32BitIndices => btTriangleMesh_getUse32bitIndices(Native);

		public bool Use4ComponentVertices => btTriangleMesh_getUse4componentVertices(Native);

		public double WeldingThreshold
		{
			get => btTriangleMesh_getWeldingThreshold(Native);
			set => btTriangleMesh_setWeldingThreshold(Native, value);
		}
	}
}
