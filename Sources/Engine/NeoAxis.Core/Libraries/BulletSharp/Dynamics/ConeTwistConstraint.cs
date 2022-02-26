using System;
using System.Runtime.InteropServices;
using Internal.BulletSharp.Math;
using static Internal.BulletSharp.UnsafeNativeMethods;

namespace Internal.BulletSharp
{
	[Flags]
	public enum ConeTwistFlags
	{
		None = 0,
		LinearCfm = 1,
		LinearErp = 2,
		AngularCfm = 4
	}

	public class ConeTwistConstraint : TypedConstraint
	{
		public ConeTwistConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, BMatrix rigidBodyAFrame,
			BMatrix rigidBodyBFrame)
			: base(btConeTwistConstraint_new(rigidBodyA.Native, rigidBodyB.Native,
				ref rigidBodyAFrame, ref rigidBodyBFrame))
		{
			_rigidBodyA = rigidBodyA;
			_rigidBodyB = rigidBodyB;
		}

		public ConeTwistConstraint(RigidBody rigidBodyA, BMatrix rigidBodyAFrame)
			: base(btConeTwistConstraint_new2(rigidBodyA.Native, ref rigidBodyAFrame))
		{
			_rigidBodyA = rigidBodyA;
			_rigidBodyB = GetFixedBody();
		}

		public void CalcAngleInfo()
		{
			btConeTwistConstraint_calcAngleInfo(Native);
		}

		public void CalcAngleInfo2Ref(ref BMatrix transA, ref BMatrix transB, ref BMatrix invInertiaWorldA,
			BMatrix invInertiaWorldB)
		{
			btConeTwistConstraint_calcAngleInfo2(Native, ref transA, ref transB,
				ref invInertiaWorldA, ref invInertiaWorldB);
		}

		public void CalcAngleInfo2(BMatrix transA, BMatrix transB, BMatrix invInertiaWorldA,
			BMatrix invInertiaWorldB)
		{
			btConeTwistConstraint_calcAngleInfo2(Native, ref transA, ref transB,
				ref invInertiaWorldA, ref invInertiaWorldB);
		}

		public void EnableMotor(bool b)
		{
			btConeTwistConstraint_enableMotor(Native, b);
		}

		public void GetInfo2NonVirtualRef(ConstraintInfo2 info, ref BMatrix transA, ref BMatrix transB,
			BMatrix invInertiaWorldA, BMatrix invInertiaWorldB)
		{
			btConeTwistConstraint_getInfo2NonVirtual(Native, info._native, ref transA,
				ref transB, ref invInertiaWorldA, ref invInertiaWorldB);
		}

		public void GetInfo2NonVirtual(ConstraintInfo2 info, BMatrix transA, BMatrix transB,
			BMatrix invInertiaWorldA, BMatrix invInertiaWorldB)
		{
			btConeTwistConstraint_getInfo2NonVirtual(Native, info._native, ref transA,
				ref transB, ref invInertiaWorldA, ref invInertiaWorldB);
		}

		public double GetLimit(int limitIndex)
		{
			return btConeTwistConstraint_getLimit(Native, limitIndex);
		}

		public BVector3 GetPointForAngle(double fAngleInRadians, double fLength)
		{
			BVector3 value;
			btConeTwistConstraint_GetPointForAngle(Native, fAngleInRadians, fLength,
				out value);
			return value;
		}

		public void SetFramesRef(ref BMatrix frameA, ref BMatrix frameB)
		{
			btConeTwistConstraint_setFrames(Native, ref frameA, ref frameB);
		}

		public void SetFrames(BMatrix frameA, BMatrix frameB)
		{
			btConeTwistConstraint_setFrames(Native, ref frameA, ref frameB);
		}

		public void SetLimit(int limitIndex, double limitValue)
		{
			btConeTwistConstraint_setLimit(Native, limitIndex, limitValue);
		}

		public void SetLimit(double swingSpan1, double swingSpan2, double twistSpan,
			double softness = 1.0f, double biasFactor = 0.3f, double relaxationFactor = 1.0f)
		{
			btConeTwistConstraint_setLimit2(Native, swingSpan1, swingSpan2, twistSpan,
				softness, biasFactor, relaxationFactor);
		}

		public void SetMaxMotorImpulseNormalized(double maxMotorImpulse)
		{
			btConeTwistConstraint_setMaxMotorImpulseNormalized(Native, maxMotorImpulse);
		}

		public void SetMotorTargetInConstraintSpace(BQuaternion q)
		{
			btConeTwistConstraint_setMotorTargetInConstraintSpace(Native, ref q);
		}

		public void UpdateRhs(double timeStep)
		{
			btConeTwistConstraint_updateRHS(Native, timeStep);
		}

		public BMatrix AFrame
		{
			get
			{
				BMatrix value;
				btConeTwistConstraint_getAFrame(Native, out value);
				return value;
			}
		}

		public bool AngularOnly
		{
			get => btConeTwistConstraint_getAngularOnly(Native);
			set => btConeTwistConstraint_setAngularOnly(Native, value);
		}

		public BMatrix BFrame
		{
			get
			{
				BMatrix value;
				btConeTwistConstraint_getBFrame(Native, out value);
				return value;
			}
		}

		public double BiasFactor => btConeTwistConstraint_getBiasFactor(Native);

		public double Damping
		{
			get => btConeTwistConstraint_getDamping(Native);
			set => btConeTwistConstraint_setDamping(Native, value);
		}

		public double FixThresh
		{
			get => btConeTwistConstraint_getFixThresh(Native);
			set => btConeTwistConstraint_setFixThresh(Native, value);
		}

		public ConeTwistFlags Flags => btConeTwistConstraint_getFlags(Native);

		public BMatrix FrameOffsetA
		{
			get
			{
				BMatrix value;
				btConeTwistConstraint_getFrameOffsetA(Native, out value);
				return value;
			}
		}

		public BMatrix FrameOffsetB
		{
			get
			{
				BMatrix value;
				btConeTwistConstraint_getFrameOffsetB(Native, out value);
				return value;
			}
		}

		public bool IsMaxMotorImpulseNormalized => btConeTwistConstraint_isMaxMotorImpulseNormalized(Native);

		public bool IsMotorEnabled => btConeTwistConstraint_isMotorEnabled(Native);

		public bool IsPastSwingLimit => btConeTwistConstraint_isPastSwingLimit(Native);

		public double LimitSoftness => btConeTwistConstraint_getLimitSoftness(Native);

		public double MaxMotorImpulse
		{
			get => btConeTwistConstraint_getMaxMotorImpulse(Native);
			set => btConeTwistConstraint_setMaxMotorImpulse(Native, value);
		}

		public BQuaternion MotorTarget
		{
			get
			{
				BQuaternion value;
				btConeTwistConstraint_getMotorTarget(Native, out value);
				return value;
			}
			set => btConeTwistConstraint_setMotorTarget(Native, ref value);
		}

		public double RelaxationFactor => btConeTwistConstraint_getRelaxationFactor(Native);
		public int SolveSwingLimit => btConeTwistConstraint_getSolveSwingLimit(Native);
		public int SolveTwistLimit => btConeTwistConstraint_getSolveTwistLimit(Native);
		public double SwingSpan1 => btConeTwistConstraint_getSwingSpan1(Native);
		public double SwingSpan2 => btConeTwistConstraint_getSwingSpan2(Native);
		public double TwistAngle => btConeTwistConstraint_getTwistAngle(Native);
		public double TwistLimitSign => btConeTwistConstraint_getTwistLimitSign(Native);
		public double TwistSpan => btConeTwistConstraint_getTwistSpan(Native);
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct ConeTwistConstraintFloatData
	{
		public TypedConstraintFloatData TypedConstraintData;
		public TransformFloatData RigidBodyAFrame;
		public TransformFloatData RigidBodyBFrame;
		public float SwingSpan1;
		public float SwingSpan2;
		public float TwistSpan;
		public float LimitSoftness;
		public float BiasFactor;
		public float RelaxationFactor;
		public float Damping;
		public int Pad;

        public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(ConeTwistConstraintFloatData), fieldName).ToInt32(); }
    }

	[StructLayout(LayoutKind.Sequential)]
	internal struct ConeTwistConstraintDoubleData
	{
		public TypedConstraintDoubleData TypedConstraintData;
		public TransformDoubleData RigidBodyAFrame;
		public TransformDoubleData RigidBodyBFrame;
		public double SwingSpan1;
		public double SwingSpan2;
		public double TwistSpan;
		public double LimitSoftness;
		public double BiasFactor;
		public double RelaxationFactor;
		public double Damping;
		public int Pad;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(ConeTwistConstraintDoubleData), fieldName).ToInt32(); }
	}
}
