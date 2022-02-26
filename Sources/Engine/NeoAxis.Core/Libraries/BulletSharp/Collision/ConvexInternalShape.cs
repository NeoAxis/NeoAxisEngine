using System;
using System.Runtime.InteropServices;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public abstract class ConvexInternalShape : ConvexShape
	{
		internal ConvexInternalShape(IntPtr native)
			: base(native)
		{
		}

		public void SetSafeMargin(double minDimension, double defaultMarginMultiplier = 0.1f)
		{
			btConvexInternalShape_setSafeMargin(Native, minDimension, defaultMarginMultiplier);
		}

		public void SetSafeMarginRef(ref BVector3 halfExtents, double defaultMarginMultiplier = 0.1f)
		{
			btConvexInternalShape_setSafeMargin2(Native, ref halfExtents, defaultMarginMultiplier);
		}

		public void SetSafeMargin(BVector3 halfExtents, double defaultMarginMultiplier = 0.1f)
		{
			btConvexInternalShape_setSafeMargin2(Native, ref halfExtents, defaultMarginMultiplier);
		}

		public BVector3 ImplicitShapeDimensions
		{
			get
			{
				BVector3 value;
				btConvexInternalShape_getImplicitShapeDimensions(Native, out value);
				return value;
			}
			set { btConvexInternalShape_setImplicitShapeDimensions(Native, ref value); }
		}

		public BVector3 LocalScalingNV
		{
			get
			{
				BVector3 value;
				btConvexInternalShape_getLocalScalingNV(Native, out value);
				return value;
			}
		}

		public double MarginNV => btConvexInternalShape_getMarginNV(Native);
	}

	public abstract class ConvexInternalAabbCachingShape : ConvexInternalShape
	{
		internal ConvexInternalAabbCachingShape(IntPtr native)
			: base(native)
		{
		}

		public void RecalcLocalAabb()
		{
			btConvexInternalAabbCachingShape_recalcLocalAabb(Native);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct ConvexInternalShapeData
	{
		public CollisionShapeData CollisionShapeData;
		public Vector3FloatData LocalScaling;
		public Vector3FloatData ImplicitShapeDimensions;
		public float CollisionMargin;
		public int Padding;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(ConvexInternalShapeData), fieldName).ToInt32(); }
	}
}
