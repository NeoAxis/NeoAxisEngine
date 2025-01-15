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
			public float fraction;
			public int/*bool*/ backFaceHit;
		}

		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		public struct PhysicsSettings2
		{
			/// Size of body pairs array, corresponds to the maximum amount of potential body pairs that can be in flight at any time.
			/// Setting this to a low value will use less memory but slow down simulation as threads may run out of narrow phase work.
			public int mMaxInFlightBodyPairs;

			/// How many PhysicsStepListeners to notify in 1 batch
			public int mStepListenersBatchSize;

			/// How many step listener batches are needed before spawning another job (set to INT_MAX if no parallelism is desired)
			public int mStepListenerBatchesPerJob;

			/// Baumgarte stabilization factor (how much of the position error to 'fix' in 1 update) (unit: dimensionless, 0 = nothing, 1 = 100%)
			public float mBaumgarte;

			/// Radius around objects inside which speculative contact points will be detected. Note that if this is too big 
			/// you will get ghost collisions as speculative contacts are based on the closest points during the collision detection 
			/// step which may not be the actual closest points by the time the two objects hit (unit: meters)
			public float mSpeculativeContactDistance;

			/// How much bodies are allowed to sink into eachother (unit: meters)
			public float mPenetrationSlop;

			/// Fraction of its inner radius a body must move per step to enable casting for the LinearCast motion quality
			public float mLinearCastThreshold;

			/// Fraction of its inner radius a body may penetrate another body for the LinearCast motion quality
			public float mLinearCastMaxPenetration;

			/// Max squared distance to use to determine if two points are on the same plane for determining the contact manifold between two shape faces (unit: meter^2)
			public float mManifoldToleranceSq;

			/// Maximum distance to correct in a single iteration when solving position constraints (unit: meters)
			public float mMaxPenetrationDistance;

			/// Maximum relative delta position for body pairs to be able to reuse collision results from last frame (units: meter^2)
			public float mBodyPairCacheMaxDeltaPositionSq; ///< 1 mm


			/// Maximum relative delta orientation for body pairs to be able to reuse collision results from last frame, stored as cos(max angle / 2)
			public float mBodyPairCacheCosMaxDeltaRotationDiv2; ///< cos(2 degrees / 2)


			/// Maximum angle between normals that allows manifolds between different sub shapes of the same body pair to be combined
			public float mContactNormalCosMaxDeltaRotation; ///< cos(5 degree)


			/// Maximum allowed distance between old and new contact point to preserve contact forces for warm start (units: meter^2)
			public float mContactPointPreserveLambdaMaxDistSq; ///< 1 cm


			/// Number of solver velocity iterations to run
			/// Note that this needs to be >= 2 in order for friction to work (friction is applied using the non-penetration impulse from the previous iteration)
			public int mNumVelocitySteps;

			/// Number of solver position iterations to run
			public int mNumPositionSteps;

			/// Minimal velocity needed before a collision can be elastic (unit: m)
			public float mMinVelocityForRestitution;

			/// Time before object is allowed to go to sleep (unit: seconds)
			public float mTimeBeforeSleep;

			/// Velocity of points on bounding box of object below which an object can be considered sleeping (unit: m/s)
			public float mPointVelocitySleepThreshold;

			/// By default the simulation is deterministic, it is possible to turn this off by setting this setting to false. This will make the simulation run faster but it will no longer be deterministic.
			public bool mDeterministicSimulation;

			///@name These variables are mainly for debugging purposes, they allow turning on/off certain subsystems. You probably want to leave them alone.
			///@{

			/// Whether or not to use warm starting for constraints (initially applying previous frames impulses)
			public bool mConstraintWarmStart;

			/// Whether or not to use the body pair cache, which removes the need for narrow phase collision detection when orientation between two bodies didn't change
			public bool mUseBodyPairContactCache;

			/// Whether or not to reduce manifolds with similar contact normals into one contact manifold
			public bool mUseManifoldReduction;

			/// If we split up large islands into smaller parallel batches of work (to improve performance)
			public bool mUseLargeIslandSplitter;

			/// If objects can go to sleep or not
			public bool mAllowSleeping;

			/// When false, we prevent collision against non-active (shared) edges. Mainly for debugging the algorithm.
			public bool mCheckActiveEdges;

			///@}
		}

		///////////////////////////////////////////////

		//[StructLayout( LayoutKind.Sequential )]
		//public struct ContactsItem
		//{
		//	public Vector3 worldPositionOn1;
		//	public Vector3 worldPositionOn2;
		//	public uint body2Id;
		//	//public IntPtr body2;
		//}

		///////////////////////////////////////////////

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JCreateSystem( int maxBodies, int maxBodyPairs, int maxContactConstraints );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JDestroySystem( IntPtr system );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JDestroy();

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JPhysicsSystem_SetPhysicsSettings( IntPtr system/*, [MarshalAs( UnmanagedType.U1 )] bool useDafault*/,
		//	int maxInFlightBodyPairs,
		//	int stepListenersBatchSize,
		//	int stepListenerBatchesPerJob,
		//	float baumgarte,
		//	float speculativeContactDistance,
		//	float penetrationSlop,
		//	float linearCastThreshold,
		//	float linearCastMaxPenetration,
		//	float manifoldToleranceSq,
		//	float maxPenetrationDistance,
		//	float bodyPairCacheMaxDeltaPositionSq,
		//	float bodyPairCacheCosMaxDeltaRotationDiv2,
		//	float contactNormalCosMaxDeltaRotation,
		//	float contactPointPreserveLambdaMaxDistSq,
		//	int numVelocitySteps,
		//	int numPositionSteps,
		//	float minVelocityForRestitution,
		//	float timeBeforeSleep,
		//	float pointVelocitySleepThreshold,
		//	[MarshalAs( UnmanagedType.U1 )] bool constraintWarmStart,
		//	[MarshalAs( UnmanagedType.U1 )] bool useBodyPairContactCache,
		//	[MarshalAs( UnmanagedType.U1 )] bool useManifoldReduction,
		//	[MarshalAs( UnmanagedType.U1 )] bool allowSleeping,
		//	[MarshalAs( UnmanagedType.U1 )] bool checkActiveEdges );

		public static void Init_SetPhysicsSettings( ref PhysicsSettings2 settings,
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
			bool deterministicSimulation,
			bool constraintWarmStart,
			bool useBodyPairContactCache,
			bool useManifoldReduction,
			bool allowSleeping,
			bool useLargeIslandSplitter,
			bool checkActiveEdges )
		{
			settings.mMaxInFlightBodyPairs = maxInFlightBodyPairs;
			settings.mStepListenersBatchSize = stepListenersBatchSize;
			settings.mStepListenerBatchesPerJob = stepListenerBatchesPerJob;
			settings.mBaumgarte = baumgarte;
			settings.mSpeculativeContactDistance = speculativeContactDistance;
			settings.mPenetrationSlop = penetrationSlop;
			settings.mLinearCastThreshold = linearCastThreshold;
			settings.mLinearCastMaxPenetration = linearCastMaxPenetration;
			settings.mManifoldToleranceSq = manifoldToleranceSq;
			settings.mMaxPenetrationDistance = maxPenetrationDistance;
			settings.mBodyPairCacheMaxDeltaPositionSq = bodyPairCacheMaxDeltaPositionSq;
			settings.mBodyPairCacheCosMaxDeltaRotationDiv2 = bodyPairCacheCosMaxDeltaRotationDiv2;
			settings.mContactNormalCosMaxDeltaRotation = contactNormalCosMaxDeltaRotation;
			settings.mContactPointPreserveLambdaMaxDistSq = contactPointPreserveLambdaMaxDistSq;
			settings.mNumVelocitySteps = numVelocitySteps;
			settings.mNumPositionSteps = numPositionSteps;
			settings.mMinVelocityForRestitution = minVelocityForRestitution;
			settings.mTimeBeforeSleep = timeBeforeSleep;
			settings.mPointVelocitySleepThreshold = pointVelocitySleepThreshold;
			settings.mDeterministicSimulation = deterministicSimulation;
			settings.mConstraintWarmStart = constraintWarmStart;
			settings.mUseBodyPairContactCache = useBodyPairContactCache;
			settings.mUseManifoldReduction = useManifoldReduction;
			settings.mAllowSleeping = allowSleeping;
			settings.mUseLargeIslandSplitter = useLargeIslandSplitter;
			settings.mCheckActiveEdges = checkActiveEdges;
		}

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_SetPhysicsSettingsStructure( IntPtr system, PhysicsSettings2* settings, int structSizeToCheck );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JPhysicsSystem_SetGravity( IntPtr system, ref Vector3F value );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JPhysicsSystem_OptimizeBroadPhase( IntPtr system );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JPhysicsSystem_Update( IntPtr system, float deltaTime, int collisionSteps/*, int integrationSubSteps*/, [MarshalAs( UnmanagedType.U1 )] bool debug );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_GetActiveBodies( IntPtr system, out int count, out uint* bodies );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JPhysicsSystem_GetActiveBodiesFree( IntPtr system, uint* bodies );

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JPhysicsSystem_DebugDraw( IntPtr system, out int lineCount, out DebugDrawLineItem* lines );

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JPhysicsSystem_DebugDrawFree( IntPtr system, DebugDrawLineItem* lines );

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JCreateBody( IntPtr system, IntPtr shape, int motionType, float linearDamping, float angularDamping, ref Vector3 position, ref QuaternionF rotation, [MarshalAs( UnmanagedType.U1 )] bool activate, float mass, ref Vector3F centerOfMassOffset, /*[MarshalAs( UnmanagedType.U1 )] bool centerOfMassManual, ref Vector3F centerOfMassPosition, ref Vector3F inertiaTensorFactor, */ref uint bodyId, int motionQuality, [MarshalAs( UnmanagedType.U1 )] bool characterMode );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JDestroyBody( IntPtr system, IntPtr body );

		//!!!!optimization. where else
		//[DllImport( library, CallingConvention = convention )]
		//public static extern void JDestroyBodies( IntPtr system, IntPtr list, int count );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_GetData( IntPtr body, out Vector3 position, out QuaternionF rotation, out Vector3F linearVelocity, out Vector3F angularVelocity, [MarshalAs( UnmanagedType.U1 )] out bool active );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_GetAABB( IntPtr body, out Vector3 boundsMin, out Vector3 boundsMax );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_GetShapeCenterOfMass( IntPtr body, out Vector3F centerOfMass );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_Activate( IntPtr body );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JBody_Deactivate( IntPtr body );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_SetLinearVelocity( IntPtr body, ref Vector3F value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_SetAngularVelocity( IntPtr body, ref Vector3F value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_SetFriction( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_SetRestitution( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_SetLinearDamping( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_SetAngularDamping( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
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
		[SuppressGCTransition]
		public static unsafe extern void JBody_GetContacts( IntPtr body, out int count, out Scene.PhysicsWorldClass.ContactItem* buffer );// ContactsItem* buffer );

		[return: MarshalAs( UnmanagedType.U1 )]
		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern bool JBody_ContactsExist( IntPtr body );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern void JBody_SetInverseInertia( IntPtr body, ref Vector3F diagonal, ref QuaternionF rotation );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern void JBody_GetWorldSpaceSurfaceNormal( IntPtr body, uint subShapeID, ref Vector3 position, out Vector3F normal );

		//[DllImport( library, CallingConvention = convention )]
		//[SuppressGCTransition]
		//public static unsafe extern void JBody_SetCharacterMode( IntPtr body, [MarshalAs( UnmanagedType.U1 )] bool value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern void JBody_SetCharacterModeMaxStrength( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern void JBody_SetCharacterModePredictiveContactDistance( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern void JBody_SetCharacterModeWalkUpHeight( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern void JBody_SetCharacterModeWalkDownHeight( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern void JBody_SetCharacterModeMaxSlopeAngle( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern void JBody_SetCharacterModeSetSupportingVolume( IntPtr body, float value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static unsafe extern void JBody_SetCharacterModeSetDesiredVelocity( IntPtr body, ref Vector2F value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JBody_GetCharacterModeData( IntPtr body, out Scene.PhysicsWorldClass.Body.CharacterDataGroundState groundState, out uint groundBodyID, out uint groundBodySubShapeID, out Vector3 groundPosition, out Vector3F groundNormal, out Vector3F groundVelocity, out float walkUpDownLastChange );

		//////////////////////

		[DllImport( library, CallingConvention = convention )]
		public static extern IntPtr JCreateShape( [MarshalAs( UnmanagedType.U1 )] bool mutableCompoundShape );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JDestroyShape( IntPtr shape );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool JShape_IsValid( IntPtr shape );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JShape_AddSphere( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, float radius );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JShape_AddBox( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, ref Vector3F dimensions, float convexRadius );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public static extern void JShape_AddCapsule( IntPtr shape, ref Vector3F position, ref QuaternionF rotation, float halfHeightOfCylinder, float radius );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
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
		public unsafe static extern IntPtr JCreateConstraintVehicle( IntPtr system, IntPtr body, int wheelCount, Scene.PhysicsWorldClass.VehicleWheelSettings* wheelsSettings, float frontWheelAntiRollBarStiffness, float rearWheelAntiRollBarStiffness, float maxPitchRollAngle, float engineMaxTorque, float engineMinRPM, float engineMaxRPM, [MarshalAs( UnmanagedType.U1 )] bool transmissionAuto, int transmissionGearRatiosCount, double* transmissionGearRatios, int transmissionReverseGearRatiosCount, double* transmissionReverseGearRatios, float transmissionSwitchTime, float transmissionClutchReleaseTime, float transmissionSwitchLatency, float transmissionShiftUpRPM, float transmissionShiftDownRPM, float transmissionClutchStrength, /*[MarshalAs( UnmanagedType.U1 )] bool frontWheelDrive, [MarshalAs( UnmanagedType.U1 )] bool rearWheelDrive, float frontWheelDifferentialRatio, float frontWheelDifferentialLeftRightSplit, float frontWheelDifferentialLimitedSlipRatio, float frontWheelDifferentialEngineTorqueRatio, float rearWheelDifferentialRatio, float rearWheelDifferentialLeftRightSplit, float rearWheelDifferentialLimitedSlipRatio, float rearWheelDifferentialEngineTorqueRatio, */float maxSlopeAngleInRadians, [MarshalAs( UnmanagedType.U1 )] bool tracks, int antiRollbarsCount, float* antiRollbars, float differentialLimitedSlipRatio, int engineNormalizedTorqueCount, float* engineNormalizedTorque, float engineInertia, float engineAngularDamping, int differentialsCount, float* differentials, int trackDrivenWheel, float trackInertia, float trackAngularDamping, float trackMaxBrakeTorque, float trackDifferentialRatio );

		[DllImport( library, CallingConvention = convention )]
		public static extern void JDestroyConstraint( IntPtr system, IntPtr constraint );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JConstraint_Draw( IntPtr constraint, float drawSize, out int lineCount, out DebugDrawLineItem* lines );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JConstraint_DrawFree( IntPtr constraint, DebugDrawLineItem* lines );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public unsafe static extern void JConstraint_SetCollisionsBetweenLinkedBodies( IntPtr constraint, [MarshalAs( UnmanagedType.U1 )] bool value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public unsafe static extern void JConstraint_SetNumVelocityStepsOverride( IntPtr constraint, int value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public unsafe static extern void JConstraint_SetNumPositionStepsOverride( IntPtr constraint, int value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public unsafe static extern void JConstraint_SetSimulate( IntPtr constraint, [MarshalAs( UnmanagedType.U1 )] bool value );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public unsafe static extern void JSixDOFConstraint_SetLimit( IntPtr constraint, int axis, float min, float max );

		[DllImport( library, CallingConvention = convention )]
		[SuppressGCTransition]
		public unsafe static extern void JSixDOFConstraint_SetFriction( IntPtr constraint, int axis, float value );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JSixDOFConstraint_SetMotor( IntPtr constraint, int axis, int mode, float frequency, float damping, float limitMin, float limitMax, float target );

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JConstraint_SixDOF_SetMotor( IntPtr constraint, int axis, [MarshalAs( UnmanagedType.U1 )] bool enable, float targetVelocity, float maxForce, [MarshalAs( UnmanagedType.U1 )] bool servo, float servoTarget );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JVehicleConstraint_SetDriverInput( IntPtr constraint, float forward, float leftTracksOnly, float right, float brake, float handBrake, [MarshalAs( UnmanagedType.U1 )] bool activateBody );

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JVehicleConstraint_SetStepListenerMustBeAdded( IntPtr constraint, [MarshalAs( UnmanagedType.U1 )] bool value );

		[DllImport( library, CallingConvention = convention )]
		public unsafe static extern void JVehicleConstraint_GetData( IntPtr constraint, Scene.PhysicsWorldClass.VehicleWheelData* wheelsData, [MarshalAs( UnmanagedType.U1 )] out bool active, out int currentGear, [MarshalAs( UnmanagedType.U1 )] out bool isSwitchingGear, out float currentRPM );

		//[DllImport( library, CallingConvention = convention )]
		//public static extern void JVehicleConstraint_GetData2( IntPtr constraint, out Vector3 wheel0, out Vector3 wheel1, out Vector3 wheel2, out Vector3 wheel3 );

		//[DllImport( library, CallingConvention = convention )]
		//public unsafe static extern void JVehicleConstraint_Update( IntPtr constraint );
	}
}
