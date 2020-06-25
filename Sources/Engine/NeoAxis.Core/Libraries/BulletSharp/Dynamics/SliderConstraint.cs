using System;
using System.Runtime.InteropServices;
using BulletSharp.Math;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp
{
	[Flags]
	public enum SliderFlags
	{
		None = 0,
		CfmDirLinear = 1,
		ErpDirLinear = 2,
		CfmDirAngular = 4,
		ErpDirAngular = 8,
		CfmOrthoLinear = 16,
		ErpOrthoLinear = 32,
		CfmOrthoAngular = 64,
		ErpOrthoAngular = 128,
		CfmLimitLinear = 512,
		ErpLimitLinear = 1024,
		CfmLimitAngular = 2048,
		ErpLimitAngular = 4096
	}

	public class SliderConstraint : TypedConstraint
	{
		public SliderConstraint(RigidBody rigidBodyA, RigidBody rigidBodyB, Matrix frameInA,
			Matrix frameInB, bool useLinearReferenceFrameA)
			: base(btSliderConstraint_new(rigidBodyA.Native, rigidBodyB.Native,
				ref frameInA, ref frameInB, useLinearReferenceFrameA))
		{
			_rigidBodyA = rigidBodyA;
			_rigidBodyB = rigidBodyB;
		}

		public SliderConstraint(RigidBody rigidBodyB, Matrix frameInB, bool useLinearReferenceFrameA)
			: base(btSliderConstraint_new2(rigidBodyB.Native, ref frameInB, useLinearReferenceFrameA))
		{
			_rigidBodyA = GetFixedBody();
			_rigidBodyB = rigidBodyB;
		}

		public void CalculateTransformsRef(ref Matrix transA, ref Matrix transB)
		{
			btSliderConstraint_calculateTransforms(Native, ref transA, ref transB);
		}

		public void CalculateTransforms(Matrix transA, Matrix transB)
		{
			btSliderConstraint_calculateTransforms(Native, ref transA, ref transB);
		}

		public void GetInfo1NonVirtual(ConstraintInfo1 info)
		{
			btSliderConstraint_getInfo1NonVirtual(Native, info._native);
		}

		public void GetInfo2NonVirtual(ConstraintInfo2 info, Matrix transA, Matrix transB,
			Vector3 linVelA, Vector3 linVelB, double rbAinvMass, double rbBinvMass)
		{
			btSliderConstraint_getInfo2NonVirtual(Native, info._native, ref transA,
				ref transB, ref linVelA, ref linVelB, rbAinvMass, rbBinvMass);
		}

		public void SetFramesRef(ref Matrix frameA, ref Matrix frameB)
		{
			btSliderConstraint_setFrames(Native, ref frameA, ref frameB);
		}

		public void SetFrames(Matrix frameA, Matrix frameB)
		{
			btSliderConstraint_setFrames(Native, ref frameA, ref frameB);
		}

		public void TestAngularLimits()
		{
			btSliderConstraint_testAngLimits(Native);
		}

		public void TestLinearLimits()
		{
			btSliderConstraint_testLinLimits(Native);
		}

		public Vector3 AncorInA
		{
			get
			{
				Vector3 value;
				btSliderConstraint_getAncorInA(Native, out value);
				return value;
			}
		}

		public Vector3 AncorInB
		{
			get
			{
				Vector3 value;
				btSliderConstraint_getAncorInB(Native, out value);
				return value;
			}
		}

		public double AngularDepth => btSliderConstraint_getAngDepth(Native);

		public double AngularPosition => btSliderConstraint_getAngularPos(Native);

		public Matrix CalculatedTransformA
		{
			get
			{
				Matrix value;
				btSliderConstraint_getCalculatedTransformA(Native, out value);
				return value;
			}
		}

		public Matrix CalculatedTransformB
		{
			get
			{
				Matrix value;
				btSliderConstraint_getCalculatedTransformB(Native, out value);
				return value;
			}
		}

		public double DampingDirAngular
		{
			get => btSliderConstraint_getDampingDirAng(Native);
			set => btSliderConstraint_setDampingDirAng(Native, value);
		}

		public double DampingDirLinear
		{
			get => btSliderConstraint_getDampingDirLin(Native);
			set => btSliderConstraint_setDampingDirLin(Native, value);
		}

		public double DampingLimAngular
		{
			get => btSliderConstraint_getDampingLimAng(Native);
			set => btSliderConstraint_setDampingLimAng(Native, value);
		}

		public double DampingLimLinear
		{
			get => btSliderConstraint_getDampingLimLin(Native);
			set => btSliderConstraint_setDampingLimLin(Native, value);
		}

		public double DampingOrthoAngular
		{
			get => btSliderConstraint_getDampingOrthoAng(Native);
			set => btSliderConstraint_setDampingOrthoAng(Native, value);
		}

		public double DampingOrthoLinear
		{
			get => btSliderConstraint_getDampingOrthoLin(Native);
			set => btSliderConstraint_setDampingOrthoLin(Native, value);
		}

		public SliderFlags Flags => btSliderConstraint_getFlags(Native);
		public Matrix FrameOffsetA
		{
			get
			{
				Matrix value;
				btSliderConstraint_getFrameOffsetA(Native, out value);
				return value;
			}
		}

		public Matrix FrameOffsetB
		{
			get
			{
				Matrix value;
				btSliderConstraint_getFrameOffsetB(Native, out value);
				return value;
			}
		}

		public double LinearDepth => btSliderConstraint_getLinDepth(Native);

		public double LinearPosition => btSliderConstraint_getLinearPos(Native);

		public double LowerAngularLimit
		{
			get => btSliderConstraint_getLowerAngLimit(Native);
			set => btSliderConstraint_setLowerAngLimit(Native, value);
		}

		public double LowerLinearLimit
		{
			get => btSliderConstraint_getLowerLinLimit(Native);
			set => btSliderConstraint_setLowerLinLimit(Native, value);
		}

		public double MaxAngMotorForce
		{
			get => btSliderConstraint_getMaxAngMotorForce(Native);
			set => btSliderConstraint_setMaxAngMotorForce(Native, value);
		}

		public double MaxLinearMotorForce
		{
			get => btSliderConstraint_getMaxLinMotorForce(Native);
			set => btSliderConstraint_setMaxLinMotorForce(Native, value);
		}

		public bool PoweredAngularMotor
		{
			get => btSliderConstraint_getPoweredAngMotor(Native);
			set => btSliderConstraint_setPoweredAngMotor(Native, value);
		}

		public bool PoweredLinearMotor
		{
			get => btSliderConstraint_getPoweredLinMotor(Native);
			set => btSliderConstraint_setPoweredLinMotor(Native, value);
		}

		public double RestitutionDirAngular
		{
			get => btSliderConstraint_getRestitutionDirAng(Native);
			set => btSliderConstraint_setRestitutionDirAng(Native, value);
		}

		public double RestitutionDirLinear
		{
			get => btSliderConstraint_getRestitutionDirLin(Native);
			set => btSliderConstraint_setRestitutionDirLin(Native, value);
		}

		public double RestitutionLimAngular
		{
			get => btSliderConstraint_getRestitutionLimAng(Native);
			set => btSliderConstraint_setRestitutionLimAng(Native, value);
		}

		public double RestitutionLimLinear
		{
			get => btSliderConstraint_getRestitutionLimLin(Native);
			set => btSliderConstraint_setRestitutionLimLin(Native, value);
		}

		public double RestitutionOrthoAngular
		{
			get => btSliderConstraint_getRestitutionOrthoAng(Native);
			set => btSliderConstraint_setRestitutionOrthoAng(Native, value);
		}

		public double RestitutionOrthoLinear
		{
			get => btSliderConstraint_getRestitutionOrthoLin(Native);
			set => btSliderConstraint_setRestitutionOrthoLin(Native, value);
		}

		public double SoftnessDirAngular
		{
			get => btSliderConstraint_getSoftnessDirAng(Native);
			set => btSliderConstraint_setSoftnessDirAng(Native, value);
		}

		public double SoftnessDirLinear
		{
			get => btSliderConstraint_getSoftnessDirLin(Native);
			set => btSliderConstraint_setSoftnessDirLin(Native, value);
		}

		public double SoftnessLimAngular
		{
			get => btSliderConstraint_getSoftnessLimAng(Native);
			set => btSliderConstraint_setSoftnessLimAng(Native, value);
		}

		public double SoftnessLimLinear
		{
			get => btSliderConstraint_getSoftnessLimLin(Native);
			set => btSliderConstraint_setSoftnessLimLin(Native, value);
		}

		public double SoftnessOrthoAngular
		{
			get => btSliderConstraint_getSoftnessOrthoAng(Native);
			set => btSliderConstraint_setSoftnessOrthoAng(Native, value);
		}

		public double SoftnessOrthoLinear
		{
			get => btSliderConstraint_getSoftnessOrthoLin(Native);
			set => btSliderConstraint_setSoftnessOrthoLin(Native, value);
		}

		public bool SolveAngularLimit => btSliderConstraint_getSolveAngLimit(Native);

		public bool SolveLinearLimit => btSliderConstraint_getSolveLinLimit(Native);

		public double TargetAngularMotorVelocity
		{
			get => btSliderConstraint_getTargetAngMotorVelocity(Native);
			set => btSliderConstraint_setTargetAngMotorVelocity(Native, value);
		}

		public double TargetLinearMotorVelocity
		{
			get => btSliderConstraint_getTargetLinMotorVelocity(Native);
			set => btSliderConstraint_setTargetLinMotorVelocity(Native, value);
		}

		public double UpperAngularLimit
		{
			get => btSliderConstraint_getUpperAngLimit(Native);
			set => btSliderConstraint_setUpperAngLimit(Native, value);
		}

		public double UpperLinearLimit
		{
			get => btSliderConstraint_getUpperLinLimit(Native);
			set => btSliderConstraint_setUpperLinLimit(Native, value);
		}

		public bool UseFrameOffset
		{
			get => btSliderConstraint_getUseFrameOffset(Native);
			set => btSliderConstraint_setUseFrameOffset(Native, value);
		}

		public bool UseLinearReferenceFrameA => btSliderConstraint_getUseLinearReferenceFrameA(Native);
	}

    [StructLayout(LayoutKind.Sequential)]
	internal struct SliderConstraintFloatData
	{
		public TypedConstraintFloatData TypedConstraintData;
		public TransformFloatData RigidBodyAFrame;
		public TransformFloatData RigidBodyBFrame;
		public float LinearUpperLimit;
		public float LinearLowerLimit;
		public float AngularUpperLimit;
		public float AngularLowerLimit;
		public int UseLinearReferenceFrameA;
        public int UseOffsetForConstraintFrame;

        public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(SliderConstraintFloatData), fieldName).ToInt32(); }
    }

	[StructLayout(LayoutKind.Sequential)]
	internal struct SliderConstraintdoubleData
	{
		public TypedConstraintDoubleData TypedConstraintData;
		public TransformDoubleData RigidBodyAFrame;
		public TransformDoubleData RigidBodyBFrame;
		public double LinearUpperLimit;
		public double LinearLowerLimit;
		public double AngularUpperLimit;
		public double AngularLowerLimit;
		public int UseLinearReferenceFrameA;
		public int UseOffsetForConstraintFrame;

		public static int Offset(string fieldName) { return Marshal.OffsetOf(typeof(SliderConstraintdoubleData), fieldName).ToInt32(); }
	}
}
