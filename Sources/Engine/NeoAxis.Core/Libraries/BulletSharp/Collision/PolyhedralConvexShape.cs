using System;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public abstract class PolyhedralConvexShape : ConvexInternalShape
	{
		private ConvexPolyhedron _convexPolyhedron;

		internal PolyhedralConvexShape(IntPtr native)
			: base(native)
		{
		}

		public void GetEdge(int i, out BVector3 pa, out BVector3 pb)
		{
			btPolyhedralConvexShape_getEdge(Native, i, out pa, out pb);
		}

		public void GetPlane(out BVector3 planeNormal, out BVector3 planeSupport, int i)
		{
			btPolyhedralConvexShape_getPlane(Native, out planeNormal, out planeSupport,
				i);
		}

		public void GetVertex(int i, out BVector3 vtx)
		{
			btPolyhedralConvexShape_getVertex(Native, i, out vtx);
		}

		public bool InitializePolyhedralFeatures(int shiftVerticesByMargin = 0)
		{
			return btPolyhedralConvexShape_initializePolyhedralFeatures(Native,
				shiftVerticesByMargin);
		}

		public bool IsInsideRef(ref BVector3 pt, double tolerance)
		{
			return btPolyhedralConvexShape_isInside(Native, ref pt, tolerance);
		}

		public bool IsInside(BVector3 pt, double tolerance)
		{
			return btPolyhedralConvexShape_isInside(Native, ref pt, tolerance);
		}

		public ConvexPolyhedron ConvexPolyhedron
		{
			get
			{
				if (_convexPolyhedron == null)
				{
					IntPtr ptr = btPolyhedralConvexShape_getConvexPolyhedron(Native);
					if (ptr == IntPtr.Zero)
					{
						return null;
					}
					_convexPolyhedron = new ConvexPolyhedron();
				}
				return _convexPolyhedron;
			}
		}

		public int NumEdges => btPolyhedralConvexShape_getNumEdges(Native);

		public int NumPlanes => btPolyhedralConvexShape_getNumPlanes(Native);

		public int NumVertices => btPolyhedralConvexShape_getNumVertices(Native);
	}

	public abstract class PolyhedralConvexAabbCachingShape : PolyhedralConvexShape
	{
		internal PolyhedralConvexAabbCachingShape(IntPtr native)
			: base(native)
		{
		}

		public void GetNonvirtualAabbRef(ref BMatrix trans, out BVector3 aabbMin, out BVector3 aabbMax,
			double margin)
		{
			btPolyhedralConvexAabbCachingShape_getNonvirtualAabb(Native, ref trans,
				out aabbMin, out aabbMax, margin);
		}

		public void GetNonvirtualAabb(BMatrix trans, out BVector3 aabbMin, out BVector3 aabbMax,
			double margin)
		{
			btPolyhedralConvexAabbCachingShape_getNonvirtualAabb(Native, ref trans,
				out aabbMin, out aabbMax, margin);
		}

		public void RecalcLocalAabb()
		{
			btPolyhedralConvexAabbCachingShape_recalcLocalAabb(Native);
		}
	}
}
