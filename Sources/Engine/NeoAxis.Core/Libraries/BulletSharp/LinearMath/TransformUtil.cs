using System;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	public static class TransformUtil
	{
		public static void CalculateDiffAxisAngle(ref BMatrix transform0, ref BMatrix transform1,
			out BVector3 axis, out double angle)
		{
			btTransformUtil_calculateDiffAxisAngle(ref transform0, ref transform1,
				out axis, out angle);
		}

		public static void CalculateDiffAxisAngleQuaternion(ref BQuaternion orn0, ref BQuaternion orn1a,
			out BVector3 axis, out double angle)
		{
			btTransformUtil_calculateDiffAxisAngleQuaternion(ref orn0, ref orn1a,
				out axis, out angle);
		}

		public static void CalculateVelocity(ref BMatrix transform0, ref BMatrix transform1,
			double timeStep, out BVector3 linVel, out BVector3 angVel)
		{
			btTransformUtil_calculateVelocity(ref transform0, ref transform1, timeStep,
				out linVel, out angVel);
		}

		public static void CalculateVelocityQuaternion(ref BVector3 pos0, ref BVector3 pos1,
			ref BQuaternion orn0, ref BQuaternion orn1, double timeStep, out BVector3 linVel, out BVector3 angVel)
		{
			btTransformUtil_calculateVelocityQuaternion(ref pos0, ref pos1, ref orn0,
				ref orn1, timeStep, out linVel, out angVel);
		}

		public static void IntegrateTransform(ref BMatrix curTrans, ref BVector3 linvel, ref BVector3 angvel,
			double timeStep, out BMatrix predictedTransform)
		{
			btTransformUtil_integrateTransform(ref curTrans, ref linvel, ref angvel,
				timeStep, out predictedTransform);
		}
	}

	public class ConvexSeparatingDistanceUtil : IDisposable
	{
		internal IntPtr _native;

		public ConvexSeparatingDistanceUtil(double boundingRadiusA, double boundingRadiusB)
		{
			_native = btConvexSeparatingDistanceUtil_new(boundingRadiusA, boundingRadiusB);
		}

		public void InitSeparatingDistance(ref BVector3 separatingVector, double separatingDistance,
			ref BMatrix transA, ref BMatrix transB)
		{
			btConvexSeparatingDistanceUtil_initSeparatingDistance(_native, ref separatingVector,
				separatingDistance, ref transA, ref transB);
		}

		public void UpdateSeparatingDistance(ref BMatrix transA, ref BMatrix transB)
		{
			btConvexSeparatingDistanceUtil_updateSeparatingDistance(_native, ref transA,
				ref transB);
		}

		public double ConservativeSeparatingDistance => btConvexSeparatingDistanceUtil_getConservativeSeparatingDistance(_native);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_native != IntPtr.Zero)
			{
				btConvexSeparatingDistanceUtil_delete(_native);
				_native = IntPtr.Zero;
			}
		}

		~ConvexSeparatingDistanceUtil()
		{
			Dispose(false);
		}
	}
}
