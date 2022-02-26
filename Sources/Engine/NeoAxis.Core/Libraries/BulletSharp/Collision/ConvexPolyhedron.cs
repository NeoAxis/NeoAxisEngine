using Internal.BulletSharp.Math;
using System;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class Face : IDisposable
	{
		internal IntPtr Native;

		internal Face(IntPtr native)
		{
			Native = native;
		}

		public Face()
		{
			Native = btFace_new();
		}
		/*
		public AlignedIntArray Indices
		{
			get { return new AlignedIntArray(btFace_getIndices(Native)); }
		}

		public ScalarArray Plane
		{
			get { return new ScalarArray(btFace_getPlane(Native)); }
		}
		*/
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Native != IntPtr.Zero)
			{
				btFace_delete(Native);
				Native = IntPtr.Zero;
			}
		}

		~Face()
		{
			Dispose(false);
		}
	}

	public class ConvexPolyhedron : IDisposable
	{
		internal IntPtr Native;

		//AlignedFaceArray _faces;
		AlignedVector3Array _uniqueEdges;
		AlignedVector3Array _vertices;

		internal ConvexPolyhedron(IntPtr native)
		{
			Native = native;
		}

		public ConvexPolyhedron()
		{
			Native = btConvexPolyhedron_new();
		}

		public void Initialize()
		{
			btConvexPolyhedron_initialize(Native);
		}

		public void ProjectRef(ref BMatrix trans, ref BVector3 dir, out double minProj, out double maxProj,
			out BVector3 witnesPtMin, out BVector3 witnesPtMax)
		{
			btConvexPolyhedron_project(Native, ref trans, ref dir, out minProj,
				out maxProj, out witnesPtMin, out witnesPtMax);
		}

		public void Project(BMatrix trans, BVector3 dir, out double minProj, out double maxProj,
			out BVector3 witnesPtMin, out BVector3 witnesPtMax)
		{
			btConvexPolyhedron_project(Native, ref trans, ref dir, out minProj,
				out maxProj, out witnesPtMin, out witnesPtMax);
		}

		public bool TestContainment()
		{
			return btConvexPolyhedron_testContainment(Native);
		}

		public BVector3 Extents
		{
			get
			{
				BVector3 value;
				btConvexPolyhedron_getExtents(Native, out value);
				return value;
			}
			set => btConvexPolyhedron_setExtents(Native, ref value);
		}
		/*
		public AlignedFaceArray Faces
		{
			get { return btConvexPolyhedron_getFaces(Native); }
		}
		*/
		public BVector3 LocalCenter
		{
			get
			{
				BVector3 value;
				btConvexPolyhedron_getLocalCenter(Native, out value);
				return value;
			}
			set => btConvexPolyhedron_setLocalCenter(Native, ref value);
		}

		public BVector3 C
		{
			get
			{
				BVector3 value;
				btConvexPolyhedron_getMC(Native, out value);
				return value;
			}
			set => btConvexPolyhedron_setMC(Native, ref value);
		}

		public BVector3 E
		{
			get
			{
				BVector3 value;
				btConvexPolyhedron_getME(Native, out value);
				return value;
			}
			set => btConvexPolyhedron_setME(Native, ref value);
		}

		public double Radius
		{
			get => btConvexPolyhedron_getRadius(Native);
			set => btConvexPolyhedron_setRadius(Native, value);
		}

		public AlignedVector3Array UniqueEdges
		{
			get
			{
				if (_uniqueEdges == null)
				{
					_uniqueEdges = new AlignedVector3Array(btConvexPolyhedron_getUniqueEdges(Native));
				}
				return _uniqueEdges;
			}
		}

		public AlignedVector3Array Vertices
		{
			get
			{
				if (_vertices == null)
				{
					_vertices = new AlignedVector3Array(btConvexPolyhedron_getVertices(Native));
				}
				return _vertices;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Native != IntPtr.Zero)
			{
				btConvexPolyhedron_delete(Native);
				Native = IntPtr.Zero;
			}
		}

		~ConvexPolyhedron()
		{
			Dispose(false);
		}
	}
}
