// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections.Generic;
using System.IO;
using Internal;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	static class PhysicsNative
	{
		internal const string library = "NeoAxisCoreNative";
		internal const CallingConvention convention = CallingConvention.Cdecl;

		///////////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		public struct DebugDrawLineItem
		{
			public Vector3 From;
			public Vector3 To;
			public ColorByte Color;
			//public int IsArrow;
		}

		///////////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		public struct RayTestResult
		{
			public uint bodyId;
			public int shapeIndex;
			public Vector3 position;
			public Vector3F normal;
			public float distanceScale;
			//!!!!
			public int triangleIndexSource;
			public int triangleIndexProcessed;
		}

		///////////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		public struct VolumeTestResult
		{
			public uint bodyId;
		}

		///////////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		public struct ContactsItem
		{
			public Vector3 worldPositionOn1;
			public Vector3 worldPositionOn2;
			public uint body2Id;
			//public IntPtr body2;
		}

		///////////////////////////////////////////////

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JCreateSystem( int maxBodies, int maxBodyPairs, int maxContactConstraints );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JDestroySystem( IntPtr system );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_SetPhysicsSettings( IntPtr system/*, [MarshalAs( UnmanagedType.U1 )] bool useDafault*/,
			int maxInFlightBodyPairs,
			int stepListenersBatchSize,
			int stepListenerBatchesPerJob,
			float baumgarte,
			float speculativeContactDistance,
			float penetrationSlop,
			float linearCastThreshold,
			float linearCastMaxPenetration,
			float manifoldToleranceSq,
			float maxPenetrationDistance,
			float bodyPairCacheMaxDeltaPositionSq,
			float bodyPairCacheCosMaxDeltaRotationDiv2,
			float contactNormalCosMaxDeltaRotation,
			float contactPointPreserveLambdaMaxDistSq,
			int numVelocitySteps,
			int numPositionSteps,
			float minVelocityForRestitution,
			float timeBeforeSleep,
			float pointVelocitySleepThreshold,
			[MarshalAs( UnmanagedType.U1 )] bool constraintWarmStart,
			[MarshalAs( UnmanagedType.U1 )] bool useBodyPairContactCache,
			[MarshalAs( UnmanagedType.U1 )] bool useManifoldReduction,
			[MarshalAs( UnmanagedType.U1 )] bool allowSleeping,
			[MarshalAs( UnmanagedType.U1 )] bool checkActiveEdges );

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JPhysicsSystem_SetGravity( IntPtr system, ref Vector3F value );

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JPhysicsSystem_OptimizeBroadPhase( IntPtr system );

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JPhysicsSystem_Update( IntPtr system, float deltaTime, int collisionSteps, int integrationSubSteps );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_GetActiveBodies( IntPtr system, out int count, out uint* bodies );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_GetActiveBodiesFree( IntPtr system, uint* bodies );

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JPhysicsSystem_DebugDraw( IntPtr system, out int lineCount, out DebugDrawLineItem* lines );

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JPhysicsSystem_DebugDrawFree( IntPtr system, DebugDrawLineItem* lines );

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JCreateBody( IntPtr system, IntPtr shape, int motionType, float linearDamping, float angularDamping, ref Vector3 position, ref QuaternionF rotation, [MarshalAs( UnmanagedType.U1 )] bool activate, float mass, [MarshalAs( UnmanagedType.U1 )] bool centerOfMassManual, ref Vector3F centerOfMassPosition, ref Vector3F inertiaTensorFactor, ref uint bodyId, int motionQuality );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JDestroyBody( IntPtr system, IntPtr body );

		//!!!!optimization. where else
		//[DllImport( library, CallingConvention = convention )]
		//public static extern void JDestroyBodies( IntPtr system, IntPtr list, int count );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_GetData( IntPtr body, out Vector3 position, out QuaternionF rotation, out Vector3F linearVelocity, out Vector3F angularVelocity, [MarshalAs( UnmanagedType.U1 )] out bool active );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_GetAABB( IntPtr body, out Vector3 boundsMin, out Vector3 boundsMax );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_GetShapeCenterOfMass( IntPtr body, out Vector3F centerOfMass );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_Activate( IntPtr body );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_Deactivate( IntPtr body );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_SetLinearVelocity( IntPtr body, ref Vector3F value );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_SetAngularVelocity( IntPtr body, ref Vector3F value );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_SetFriction( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_SetRestitution( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_SetLinearDamping( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_SetAngularDamping( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_SetGravityFactor( IntPtr body, float value );

		//[DllImport( library, CallingConvention = convention )]
		//public static extern void JBody_SetShape( IntPtr body, );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_SetTransform( IntPtr body, ref Vector3 position, ref QuaternionF rotation, [MarshalAs( UnmanagedType.U1 )] bool activate );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_ApplyForce( IntPtr body, ref Vector3F force, ref Vector3F relativePosition );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_SetMotionQuality( IntPtr body, int motionQuality );

		[DllImport( library, CallingConvention = convention )]
		public static unsafe extern void JBody_GetContacts( IntPtr body, out int count, out ContactsItem* buffer );

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JCreateShape( [MarshalAs( UnmanagedType.U1 )] bool mutableCompoundShape );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JDestroyShape( IntPtr shape );

		[DllImport( library, CallingConvention = convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool JShape_IsValid( IntPtr shape );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JShape_AddSphere( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, float radius );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JShape_AddBox( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, ref Vector3F dimensions, float convexRadius );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JShape_AddCapsule( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, float halfHeightOfCylinder, float radius );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JShape_AddCylinder( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, float halfHeightOfCylinder, float radius, float convexRadius );

		//[DllImport( library, CallingConvention = convention )]
		//public static extern void JShape_AddCone( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, float halfHeightOfCylinder, float radius );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JShape_AddMesh( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, ref Vector3F localScaling, Vector3F* vertices, int vertexCount, int* indices, int indexCount );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JShape_AddConvexHull( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, ref Vector3F localScaling, Vector3F* points, int pointCount, float convexRadius );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_RayTest( IntPtr system, ref Ray ray, int mode, int flags, out int resultCount, out RayTestResult* results, IntPtr testShape );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_RayTestFree( IntPtr system, RayTestResult* results );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_VolumeTestSphere( IntPtr system, int mode, out int resultCount, out VolumeTestResult* results, ref Vector3 direction, ref Vector3 position, float radius );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_VolumeTestBox( IntPtr system, int mode, out int resultCount, out VolumeTestResult* results, ref Vector3 direction, ref Vector3 center, ref QuaternionF axis, ref Vector3F extents );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_VolumeTestCapsule( IntPtr system, int mode, out int resultCount, out VolumeTestResult* results, ref Vector3 direction, ref Vector3 position, ref QuaternionF rotation, float height, float radius );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_VolumeTestCylinder( IntPtr system, int mode, out int resultCount, out VolumeTestResult* results, ref Vector3 direction, ref Vector3 position, ref QuaternionF rotation, float height, float radius );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_VolumeTestFree( IntPtr system, VolumeTestResult* results );

		//, [MarshalAs( UnmanagedType.U1 )] bool transformInWorldSpace, Vector3D& positionA, Vector3& axisXA, Vector3& axisYA, Vector3D& positionB, Vector3& axisXB, Vector3& axisYB

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JCreateConstraintSixDOF( IntPtr system, IntPtr bodyA, IntPtr bodyB, [MarshalAs( UnmanagedType.U1 )] bool transformInWorldSpace, ref Vector3 positionA, ref Vector3F axisXA, ref Vector3F axisYA, ref Vector3 positionB, ref Vector3F axisXB, ref Vector3F axisYB, PhysicsAxisMode linearAxisX, ref RangeF linearLimitX, PhysicsAxisMode linearAxisY, ref RangeF linearLimitY, PhysicsAxisMode linearAxisZ, ref RangeF linearLimitZ, PhysicsAxisMode angularAxisX, ref RangeF angularLimitX, PhysicsAxisMode angularAxisY, ref RangeF angularLimitY, PhysicsAxisMode angularAxisZ, ref RangeF angularLimitZ, float linearXFriction, float linearYFriction, float linearZFriction, float angularXFriction, float angularYFriction, float angularZFriction );

		//[DllImport( library, CallingConvention = convention )]
		//public static extern IntPtr JCreateConstraintSixDOF( IntPtr system, IntPtr bodyA, IntPtr bodyB, [MarshalAs( UnmanagedType.U1 )] bool transformInWorldSpace, ref Vector3 positionA, ref Vector3F axisXA, ref Vector3F axisYA, ref Vector3 positionB, ref Vector3F axisXB, ref Vector3F axisYB, ref RangeF linearLimitX, ref RangeF linearLimitY, ref RangeF linearLimitZ, ref RangeF angularLimitX, ref RangeF angularLimitY, ref RangeF angularLimitZ );

		//[DllImport( library, CallingConvention = convention )]
		//public static extern IntPtr JCreateConstraintSixDOF( IntPtr system, IntPtr bodyA, IntPtr bodyB, ref Vector3 position, ref Vector3F axisX, ref Vector3F axisY, ref RangeF linearLimitX, ref RangeF linearLimitY, ref RangeF linearLimitZ, ref RangeF angularLimitX, ref RangeF angularLimitY, ref RangeF angularLimitZ );

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JCreateConstraintFixed( IntPtr system, IntPtr bodyA, IntPtr bodyB, [MarshalAs( UnmanagedType.U1 )] bool transformInWorldSpace, ref Vector3 positionA, ref Vector3F axisXA, ref Vector3F axisYA, ref Vector3 positionB, ref Vector3F axisXB, ref Vector3F axisYB );

		//[DllImport( library, CallingConvention = convention )]
		//public static extern IntPtr JCreateConstraintFixed( IntPtr system, IntPtr bodyA, IntPtr bodyB, ref Vector3 position, ref Vector3F axisX, ref Vector3F axisY );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern IntPtr JCreateConstraintVehicle( IntPtr system, IntPtr body, int wheelCount, Scene.PhysicsWorldClass.VehicleWheelSettings* wheelsSettings, float frontWheelAntiRollBarStiffness, float rearWheelAntiRollBarStiffness, float maxPitchRollAngle, float engineMaxTorque, float engineMinRPM, float engineMaxRPM, [MarshalAs( UnmanagedType.U1 )] bool transmissionAuto, int transmissionGearRatiosCount, double* transmissionGearRatios, int transmissionReverseGearRatiosCount, double* transmissionReverseGearRatios, float transmissionSwitchTime, float transmissionClutchReleaseTime, float transmissionSwitchLatency, float transmissionShiftUpRPM, float transmissionShiftDownRPM, float transmissionClutchStrength, [MarshalAs( UnmanagedType.U1 )] bool frontWheelDrive, [MarshalAs( UnmanagedType.U1 )] bool rearWheelDrive, float frontWheelDifferentialRatio, float frontWheelDifferentialLeftRightSplit, float frontWheelDifferentialLimitedSlipRatio, float frontWheelDifferentialEngineTorqueRatio, float rearWheelDifferentialRatio, float rearWheelDifferentialLeftRightSplit, float rearWheelDifferentialLimitedSlipRatio, float rearWheelDifferentialEngineTorqueRatio, float maxSlopeAngleInRadians );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JDestroyConstraint( IntPtr system, IntPtr constraint );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JConstraint_Draw( IntPtr constraint, float drawSize, out int lineCount, out DebugDrawLineItem* lines );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JConstraint_DrawFree( IntPtr constraint, DebugDrawLineItem* lines );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JConstraint_SetCollisionsBetweenLinkedBodies( IntPtr constraint, [MarshalAs( UnmanagedType.U1 )] bool value );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JConstraint_SetNumVelocityStepsOverride( IntPtr constraint, int value );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JConstraint_SetNumPositionStepsOverride( IntPtr constraint, int value );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JConstraint_SetSimulate( IntPtr constraint, [MarshalAs( UnmanagedType.U1 )] bool value );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JSixDOFConstraint_SetLimit( IntPtr constraint, int axis, float min, float max );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JSixDOFConstraint_SetFriction( IntPtr constraint, int axis, float value );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JSixDOFConstraint_SetMotor( IntPtr constraint, int axis, int mode, float frequency, float damping, float limitMin, float limitMax, float target );

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JConstraint_SixDOF_SetMotor( IntPtr constraint, int axis, [MarshalAs( UnmanagedType.U1 )] bool enable, float targetVelocity, float maxForce, [MarshalAs( UnmanagedType.U1 )] bool servo, float servoTarget );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JVehicleConstraint_SetDriverInput( IntPtr constraint, float forward, /*float left, */float right, float brake, float handBrake, [MarshalAs( UnmanagedType.U1 )] bool activateBody );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JVehicleConstraint_SetStepListenerAddedMustBeAdded( IntPtr constraint, [MarshalAs( UnmanagedType.U1 )] bool value );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JVehicleConstraint_GetData( IntPtr constraint, Scene.PhysicsWorldClass.VehicleWheelData* wheelsData, [MarshalAs( UnmanagedType.U1 )] out bool active );

		//[DllImport( library, CallingConvention = convention )]
		//public static extern void JVehicleConstraint_GetData2( IntPtr constraint, out Vector3 wheel0, out Vector3 wheel1, out Vector3 wheel2, out Vector3 wheel3 );

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JVehicleConstraint_Update( IntPtr constraint );
	}
}
