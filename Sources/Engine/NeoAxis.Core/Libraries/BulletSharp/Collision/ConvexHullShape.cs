using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public class ConvexHullShape : PolyhedralConvexAabbCachingShape
	{
		private Vector3Array _points;
		private Vector3Array _unscaledPoints;

		public ConvexHullShape()
			: base(btConvexHullShape_new())
		{
		}

		public ConvexHullShape(double[] points)
			: this(points, points.Length / 3, 3 * sizeof(double))
		{
		}

		public ConvexHullShape(double[] points, int numPoints, int stride = 3 * sizeof(double))
			: base(btConvexHullShape_new4(points, numPoints, stride))
		{
		}

		public ConvexHullShape(IEnumerable<BVector3> points, int numPoints)
			: base(btConvexHullShape_new())
		{
			int i = 0;
			foreach (BVector3 v in points)
			{
				BVector3 viter = v;
				AddPointRef(ref viter, false);
				i++;
				if (i == numPoints)
				{
					break;
				}
			}
			RecalcLocalAabb();
		}

		public ConvexHullShape(IEnumerable<BVector3> points)
			: base(btConvexHullShape_new())
		{
			foreach (BVector3 v in points)
			{
				BVector3 viter = v;
				AddPointRef(ref viter, false);
			}
			RecalcLocalAabb();
		}

		public void AddPointRef(ref BVector3 point, bool recalculateLocalAabb = true)
		{
			btConvexHullShape_addPoint(Native, ref point, recalculateLocalAabb);
		}

		public void AddPoint(BVector3 point, bool recalculateLocalAabb = true)
		{
			btConvexHullShape_addPoint(Native, ref point, recalculateLocalAabb);
		}

		public void GetScaledPoint(int i, out BVector3 value)
		{
			btConvexHullShape_getScaledPoint(Native, i, out value);
		}

		public BVector3 GetScaledPoint(int i)
		{
			BVector3 value;
			btConvexHullShape_getScaledPoint(Native, i, out value);
			return value;
		}

		public void OptimizeConvexHull()
		{
			btConvexHullShape_optimizeConvexHull(Native);
		}

		public int NumPoints => btConvexHullShape_getNumPoints(Native);

		public Vector3Array Points
		{
			get
			{
				if (_points == null || _points.Count != NumPoints)
				{
					_points = new Vector3Array(btConvexHullShape_getPoints(Native), NumPoints);
				}
				return _points;
			}
		}

		public Vector3Array UnscaledPoints
		{
			get
			{
				if (_unscaledPoints == null || _unscaledPoints.Count != NumPoints)
				{
					_unscaledPoints = new Vector3Array(btConvexHullShape_getUnscaledPoints(Native), NumPoints);
				}
				return _unscaledPoints;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct ConvexHullShapeData
	{
		public ConvexInternalShapeData ConvexInternalShapeData;
		public IntPtr UnscaledPointsFloatPtr;
		public IntPtr UnscaledPointsDoublePtr;
		public int NumUnscaledPoints;
		public int Padding;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(ConvexHullShapeData), fieldName).ToInt32(); }
	}
}
