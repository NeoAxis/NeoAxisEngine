using System;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class ConvexShape : CollisionShape
	{
		internal ConvexShape(IntPtr native, bool preventDelete = false)
			: base(native, preventDelete)
		{
		}
		/*
		public void BatchedUnitVectorGetSupportingVertexWithoutMargin(Vector3 vectors,
			Vector3 supportVerticesOut, int numVectors)
		{
			btConvexShape_batchedUnitVectorGetSupportingVertexWithoutMargin(Native,
				vectors.Native, supportVerticesOut.Native, numVectors);
		}
		*/

		public void GetAabbNonVirtual(BMatrix t, out BVector3 aabbMin, out BVector3 aabbMax)
		{
			btConvexShape_getAabbNonVirtual(Native, ref t, out aabbMin, out aabbMax);
		}

		public void GetAabbSlow(BMatrix t, out BVector3 aabbMin, out BVector3 aabbMax)
		{
			btConvexShape_getAabbSlow(Native, ref t, out aabbMin, out aabbMax);
		}

		public void GetPreferredPenetrationDirection(int index, out BVector3 penetrationVector)
		{
			btConvexShape_getPreferredPenetrationDirection(Native, index, out penetrationVector);
		}

		public BVector3 LocalGetSupportingVertex(BVector3 vec)
		{
			BVector3 value;
			btConvexShape_localGetSupportingVertex(Native, ref vec, out value);
			return value;
		}

		public BVector3 LocalGetSupportingVertexWithoutMargin(BVector3 vec)
		{
			BVector3 value;
			btConvexShape_localGetSupportingVertexWithoutMargin(Native, ref vec,
				out value);
			return value;
		}

		public BVector3 LocalGetSupportVertexNonVirtual(BVector3 vec)
		{
			BVector3 value;
			btConvexShape_localGetSupportVertexNonVirtual(Native, ref vec, out value);
			return value;
		}

		public BVector3 LocalGetSupportVertexWithoutMarginNonVirtual(BVector3 vec)
		{
			BVector3 value;
			btConvexShape_localGetSupportVertexWithoutMarginNonVirtual(Native, ref vec,
				out value);
			return value;
		}

		public void ProjectRef(ref BMatrix trans, ref BVector3 dir, out double minProj, out double maxProj,
			out BVector3 witnesPtMin, out BVector3 witnesPtMax)
		{
			btConvexShape_project(Native, ref trans, ref dir, out minProj, out maxProj,
				out witnesPtMin, out witnesPtMax);
		}

		public void Project(BMatrix trans, BVector3 dir, out double minProj, out double maxProj,
			out BVector3 witnesPtMin, out BVector3 witnesPtMax)
		{
			btConvexShape_project(Native, ref trans, ref dir, out minProj, out maxProj,
				out witnesPtMin, out witnesPtMax);
		}

		public double MarginNonVirtual => btConvexShape_getMarginNonVirtual(Native);

		public int NumPreferredPenetrationDirections => btConvexShape_getNumPreferredPenetrationDirections(Native);
	}
}
