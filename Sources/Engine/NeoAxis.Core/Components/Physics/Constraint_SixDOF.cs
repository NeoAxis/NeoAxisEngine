// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// 6-degrees of freedom constraint link between two physical bodies.
	/// </summary>
#if !DEPLOY
	[NewObjectSettings( typeof( NewObjectSettingsConstraint ) )]
#endif
	public class Constraint_SixDOF : ObjectInSpace, IPhysicalObject
	{
		bool created;
		ObjectInSpace creationBodyA;
		ObjectInSpace creationBodyB;
		Scene.PhysicsWorldClass.TwoBodyConstraint physicalConstraint;
		//int softAnchorModeClosestNodeIndex = -1;
		////int closestFaceIdx = -1;

		bool duringCreateDestroy;
		bool setTransformWithoutUpdatingConstraint;

		int needUpdateMotors;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The first physical body used.
		/// </summary>
		[Serialize]
		public Reference<ObjectInSpace> BodyA
		{
			get { if( _bodyA.BeginGet() ) BodyA = _bodyA.Get( this ); return _bodyA.value; }
			set
			{
				if( _bodyA.BeginSet( ref value ) )
				{
					try
					{
						BodyAChanged?.Invoke( this );
						//!!!!maybe use NeedRecreateConstraint
						RecreateConstraint();
					}
					finally { _bodyA.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BodyA"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> BodyAChanged;
		ReferenceField<ObjectInSpace> _bodyA;

		/// <summary>
		/// The second physical body used.
		/// </summary>
		[Serialize]
		public Reference<ObjectInSpace> BodyB
		{
			get { if( _bodyB.BeginGet() ) BodyB = _bodyB.Get( this ); return _bodyB.value; }
			set
			{
				if( _bodyB.BeginSet( ref value ) )
				{
					try
					{
						BodyBChanged?.Invoke( this );
						RecreateConstraint();
					}
					finally { _bodyB.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BodyB"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> BodyBChanged;
		ReferenceField<ObjectInSpace> _bodyB;

		/// <summary>
		/// Whether the collision detection is enabled between the linked physical bodies.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		public Reference<bool> CollisionsBetweenLinkedBodies
		{
			get { if( _collisionsBetweenLinkedBodies.BeginGet() ) CollisionsBetweenLinkedBodies = _collisionsBetweenLinkedBodies.Get( this ); return _collisionsBetweenLinkedBodies.value; }
			set
			{
				if( _collisionsBetweenLinkedBodies.BeginSet( ref value ) )
				{
					try
					{
						CollisionsBetweenLinkedBodiesChanged?.Invoke( this );
						physicalConstraint?.SetCollisionsBetweenLinkedBodies( CollisionsBetweenLinkedBodies );
					}
					finally { _collisionsBetweenLinkedBodies.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionsBetweenLinkedBodies"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> CollisionsBetweenLinkedBodiesChanged;
		ReferenceField<bool> _collisionsBetweenLinkedBodies = true;

		////Breakable
		//ReferenceField<bool> _breakable;
		//[DefaultValue( false )]
		//[Serialize]
		//public Reference<bool> Breakable
		//{
		//	get
		//	{
		//		if( _breakable.BeginGet() )
		//			Breakable = _breakable.Get( this );
		//		return _breakable.value;
		//	}
		//	set
		//	{
		//		if( _breakable.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				BreakableChanged?.Invoke( this );

		//				xx;
		//				RecreateConstraint();
		//			}
		//			finally { _breakable.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Constraint> BreakableChanged;


		//!!!!impl

		////!!!!после того как сломается, то как это обрабатывать? видимо нужно cериализовать сломанность
		///// <summary>
		///// The value indicates the maximum value of the applied impulse to the body, which may be before it is breaks.
		///// </summary>
		//[DefaultValue( 1.0e+005 )]
		////!!!!Infinity
		////[ApplicableRange( 0, float.MaxValue, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential, 100 )]
		////[ApplicableRange( 0, float.MaxValue, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential, 6 )]//!!!!
		//[Range( 0, 1.0e+005, RangeAttribute.ConvenientDistributionEnum.Exponential, 6 )]
		//[Serialize]
		//public Reference<double> BreakingImpulseThreshold
		//{
		//	get { if( _breakingImpulseThreshold.BeginGet() ) BreakingImpulseThreshold = _breakingImpulseThreshold.Get( this ); return _breakingImpulseThreshold.value; }
		//	set
		//	{
		//		if( _breakingImpulseThreshold.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				BreakingImpulseThresholdChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	InternalConstraintRigid.BreakingImpulseThreshold = value;
		//			}
		//			finally { _breakingImpulseThreshold.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="BreakingImpulseThreshold"/> property value changes.</summary>
		//public event Action<Constraint> BreakingImpulseThresholdChanged;
		//ReferenceField<double> _breakingImpulseThreshold = 1.0e+005;//!!!!


		/// <summary>
		/// Override for the number of solver velocity iterations to run, the total amount of iterations is the max of Scene.PhysicsNumVelocitySteps and this for all constraints in the island.
		/// </summary>
		[DefaultValue( 0 )]
		public Reference<int> NumVelocityStepsOverride
		{
			get { if( _numVelocityStepsOverride.BeginGet() ) NumVelocityStepsOverride = _numVelocityStepsOverride.Get( this ); return _numVelocityStepsOverride.value; }
			set
			{
				if( _numVelocityStepsOverride.BeginSet( ref value ) )
				{
					try
					{
						NumVelocityStepsOverrideChanged?.Invoke( this );
						physicalConstraint?.SetNumVelocityStepsOverride( NumVelocityStepsOverride );
					}
					finally { _numVelocityStepsOverride.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="NumVelocityStepsOverride"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> NumVelocityStepsOverrideChanged;
		ReferenceField<int> _numVelocityStepsOverride = 0;

		/// <summary>
		/// Override for the number of position velocity iterations to run, the total amount of iterations is the max of Scene.PhysicsNumPositionSteps and this for all constraints in the island.
		/// </summary>
		[DefaultValue( 0 )]
		public Reference<int> NumPositionStepsOverride
		{
			get { if( _numPositionStepsOverride.BeginGet() ) NumPositionStepsOverride = _numPositionStepsOverride.Get( this ); return _numPositionStepsOverride.value; }
			set
			{
				if( _numPositionStepsOverride.BeginSet( ref value ) )
				{
					try
					{
						NumPositionStepsOverrideChanged?.Invoke( this );
						physicalConstraint?.SetNumPositionStepsOverride( NumPositionStepsOverride );
					}
					finally { _numPositionStepsOverride.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="NumPositionStepsOverride"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> NumPositionStepsOverrideChanged;
		ReferenceField<int> _numPositionStepsOverride = 0;

		/// <summary>
		/// Whether to enable the constraint in the simulation. The property can be used to implement a breakable constraint.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> Simulate
		{
			get { if( _simulate.BeginGet() ) Simulate = _simulate.Get( this ); return _simulate.value; }
			set
			{
				if( _simulate.BeginSet( ref value ) )
				{
					try
					{
						SimulateChanged?.Invoke( this );
						physicalConstraint?.SetSimulate( Simulate );
					}
					finally { _simulate.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Simulate"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> SimulateChanged;
		ReferenceField<bool> _simulate = true;

		///// <summary>
		///// Overrides solver processes number on every physics frame.
		///// </summary>
		//[DefaultValue( -1 )]
		//[Range( -1, 100 )]
		//[Serialize]
		//public Reference<int> OverrideNumberSolverIterations
		//{
		//	get { if( _overrideNumberSolverIterations.BeginGet() ) OverrideNumberSolverIterations = _overrideNumberSolverIterations.Get( this ); return _overrideNumberSolverIterations.value; }
		//	set
		//	{
		//		if( _overrideNumberSolverIterations.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				OverrideNumberSolverIterationsChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	InternalConstraintRigid.OverrideNumSolverIterations = value;
		//			}
		//			finally { _overrideNumberSolverIterations.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="OverrideNumberSolverIterations"/> property value changes.</summary>
		//public event Action<Constraint> OverrideNumberSolverIterationsChanged;
		//ReferenceField<int> _overrideNumberSolverIterations = -1;

		//!!!!need?

		///// <summary>
		///// Whether to activate sleeping bodies when any motor of the constraint is enabled.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> AutoActivateBodies
		//{
		//	get { if( _autoActivateBodies.BeginGet() ) AutoActivateBodies = _autoActivateBodies.Get( this ); return _autoActivateBodies.value; }
		//	set { if( _autoActivateBodies.BeginSet( ref value ) ) { try { AutoActivateBodiesChanged?.Invoke( this ); } finally { _autoActivateBodies.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AutoActivateBodies"/> property value changes.</summary>
		//public event Action<Constraint> AutoActivateBodiesChanged;
		//ReferenceField<bool> _autoActivateBodies = false;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public Scene.PhysicsWorldClass GetPhysicsWorldData()
		{
			var scene = ParentScene;
			if( scene != null )
				return scene.PhysicsWorld;
			return null;
		}

		//void GetSoftBodyModeBodies( out SoftBody body1, out PhysicalBody body2 )
		//{
		//	//body1 is always soft body
		//	if( creationBodyA != null && creationBodyA.BulletBody is Internal.BulletSharp.SoftBody.SoftBody )
		//	{
		//		body1 = (SoftBody)creationBodyA;
		//		body2 = creationBodyB;
		//	}
		//	else
		//	{
		//		body1 = (SoftBody)creationBodyB;
		//		body2 = creationBodyA;
		//	}
		//}

		static Scene.PhysicsWorldClass.Body GetBody( ObjectInSpace obj )
		{
			var rigidBody = obj as RigidBody;
			if( rigidBody != null )
				return rigidBody.PhysicalBody;

			var meshInSpace = obj as MeshInSpace;
			if( meshInSpace != null )
			{
				if( meshInSpace.Collision )
					return meshInSpace.PhysicalBody;
				else
				{
					var rigidBody2 = meshInSpace.GetComponent<RigidBody>( "Collision Body" );
					if( rigidBody2 != null )
						return rigidBody2.PhysicalBody;
				}
			}

			return null;
		}

		void CreateConstraint()
		{
			if( physicalConstraint != null )
				Log.Fatal( "Constraint: CreateConstraint: physicalConstraint != null." );
			if( !EnabledInHierarchy )
				Log.Fatal( "Constraint: CreateConstraint: !EnabledInHierarchy." );

			duringCreateDestroy = true;

			var physicsWorldData = GetPhysicsWorldData();
			if( physicsWorldData != null )
			{
				var ba = BodyA.Value;
				var bb = BodyB.Value;

				var physicalBodyA = ba != null ? GetBody( ba ) : null;
				var physicalBodyB = bb != null ? GetBody( bb ) : null;

				//check bodies are enabled
				if( ba != null && physicalBodyA == null )
					return;
				if( bb != null && physicalBodyB == null )
					return;
				//check no bodies
				if( ba == null && bb == null )
					return;

				created = true;
				creationBodyA = ba;
				creationBodyB = bb;

				//if( SoftBodyMode )
				//{
				//}
				//else
				//{
				//rigid bodies only

				//transform without Scale
				var transform = Transform.Value;
				//var t = new Matrix4( transform.Rotation.ToMatrix3(), transform.Position );

				if( creationBodyA != null && creationBodyB != null )
				{
					var position = transform.Position;
					var rotation = transform.Rotation.ToQuaternionF();
					var visualScale = transform.Scale.ToVector3F();

					if( LinearXAxis.Value == PhysicsAxisMode.Locked && LinearYAxis.Value == PhysicsAxisMode.Locked && LinearZAxis.Value == PhysicsAxisMode.Locked && AngularXAxis.Value == PhysicsAxisMode.Locked && AngularYAxis.Value == PhysicsAxisMode.Locked && AngularZAxis.Value == PhysicsAxisMode.Locked )
					{
						physicalConstraint = physicsWorldData.CreateConstraintFixed( this, physicalBodyA, physicalBodyB, true, ref position, ref rotation, ref position, ref rotation, ref visualScale );
					}
					else
					{

						//var bodyATransformFull = creationBodyA.Transform.Value;
						//var bodyATransform = new Matrix4( bodyATransformFull.Rotation.ToMatrix3(), bodyATransformFull.Position );
						//var bodyBTransformFull = creationBodyB.Transform.Value;
						//var bodyBTransform = new Matrix4( bodyBTransformFull.Rotation.ToMatrix3(), bodyBTransformFull.Position );
						//constraintAFrame = bodyATransform.GetInverse() * t;
						//BMatrix matA = BulletPhysicsUtility.Convert( constraintAFrame );
						//BMatrix matB = BulletPhysicsUtility.Convert( bodyBTransform.GetInverse() * t );

						////constraintAFrame = bodyA.Transform.Value.ToMat4().GetInverse() * t;
						////Matrix matA = BulletUtils.Convert( constraintAFrame );
						////Matrix matB = BulletUtils.Convert( bodyB.Transform.Value.ToMat4().GetInverse() * t );

						//constraintRigid = new Generic6DofSpring2Constraint( (Internal.BulletSharp.RigidBody)creationBodyA.BulletBody, (Internal.BulletSharp.RigidBody)creationBodyB.BulletBody, matA, matB );

						var linearLimitX = RangeF.Zero;
						var linearLimitY = RangeF.Zero;
						var linearLimitZ = RangeF.Zero;
						var angularLimitX = RangeF.Zero;
						var angularLimitY = RangeF.Zero;
						var angularLimitZ = RangeF.Zero;

						//LinearXAxis
						if( LinearXAxis.Value == PhysicsAxisMode.Limited )
						{
							linearLimitX = LinearXAxisLimit.Value.ToRangeF();
							if( linearLimitX.Minimum > linearLimitX.Maximum )
								linearLimitX.Maximum = linearLimitX.Minimum;
							if( linearLimitX.Minimum > 0 )
								linearLimitX.Minimum = 0;
							if( linearLimitX.Maximum < 0 )
								linearLimitX.Maximum = 0;
						}

						//LinearYAxis
						if( LinearYAxis.Value == PhysicsAxisMode.Limited )
						{
							linearLimitY = LinearYAxisLimit.Value.ToRangeF();
							if( linearLimitY.Minimum > linearLimitY.Maximum )
								linearLimitY.Maximum = linearLimitY.Minimum;
							if( linearLimitY.Minimum > 0 )
								linearLimitY.Minimum = 0;
							if( linearLimitY.Maximum < 0 )
								linearLimitY.Maximum = 0;
						}

						//LinearZAxis
						if( LinearZAxis.Value == PhysicsAxisMode.Limited )
						{
							linearLimitZ = LinearZAxisLimit.Value.ToRangeF();
							if( linearLimitZ.Minimum > linearLimitZ.Maximum )
								linearLimitZ.Maximum = linearLimitZ.Minimum;
							if( linearLimitZ.Minimum > 0 )
								linearLimitZ.Minimum = 0;
							if( linearLimitZ.Maximum < 0 )
								linearLimitZ.Maximum = 0;
						}

						//AngularXAxis
						if( AngularXAxis.Value == PhysicsAxisMode.Limited )
						{
							angularLimitX = AngularXAxisLimit.Value.ToRangeF();
							if( angularLimitX.Minimum > angularLimitX.Maximum )
								angularLimitX.Maximum = angularLimitX.Minimum;
							if( angularLimitX.Minimum > 0 )
								angularLimitX.Minimum = 0;
							if( angularLimitX.Maximum < 0 )
								angularLimitX.Maximum = 0;
						}

						//AngularYAxis
						if( AngularYAxis.Value == PhysicsAxisMode.Limited )
						{
							angularLimitY = AngularYAxisLimit.Value.ToRangeF();
							if( angularLimitY.Minimum > angularLimitY.Maximum )
								angularLimitY.Maximum = angularLimitY.Minimum;
							if( angularLimitY.Minimum > 0 )
								angularLimitY.Minimum = 0;
							if( angularLimitY.Maximum < 0 )
								angularLimitY.Maximum = 0;
						}

						//AngularZAxis
						if( AngularZAxis.Value == PhysicsAxisMode.Limited )
						{
							angularLimitZ = AngularZAxisLimit.Value.ToRangeF();
							if( angularLimitZ.Minimum > angularLimitZ.Maximum )
								angularLimitZ.Maximum = angularLimitZ.Minimum;
							if( angularLimitZ.Minimum > 0 )
								angularLimitZ.Minimum = 0;
							if( angularLimitZ.Maximum < 0 )
								angularLimitZ.Maximum = 0;
						}

						var linearXFriction = (float)( LinearXAxis.Value != PhysicsAxisMode.Locked ? LinearXAxisFriction : 0 );
						var linearYFriction = (float)( LinearYAxis.Value != PhysicsAxisMode.Locked ? LinearYAxisFriction : 0 );
						var linearZFriction = (float)( LinearZAxis.Value != PhysicsAxisMode.Locked ? LinearZAxisFriction : 0 );
						var angularXFriction = (float)( AngularXAxis.Value != PhysicsAxisMode.Locked ? AngularXAxisFriction : 0 );
						var angularYFriction = (float)( AngularYAxis.Value != PhysicsAxisMode.Locked ? AngularYAxisFriction : 0 );
						var angularZFriction = (float)( AngularZAxis.Value != PhysicsAxisMode.Locked ? AngularZAxisFriction : 0 );

						physicalConstraint = physicsWorldData.CreateConstraintSixDOF( this, physicalBodyA, physicalBodyB, true, ref position, ref rotation, ref position, ref rotation, ref visualScale, LinearXAxis, ref linearLimitX, LinearYAxis, ref linearLimitY, LinearZAxis, ref linearLimitZ, AngularXAxis, ref angularLimitX, AngularYAxis, ref angularLimitY, AngularZAxis, ref angularLimitZ, linearXFriction, linearYFriction, linearZFriction, angularXFriction, angularYFriction, angularZFriction );


						////bodyB.ActivationState = ActivationState.DisableDeactivation;

						//var bodyATransformFull = creationBodyA.Transform.Value;
						//var bodyATransform = new Matrix4( bodyATransformFull.Rotation.ToMatrix3(), bodyATransformFull.Position );
						//var bodyBTransformFull = creationBodyB.Transform.Value;
						//var bodyBTransform = new Matrix4( bodyBTransformFull.Rotation.ToMatrix3(), bodyBTransformFull.Position );
						//constraintAFrame = bodyATransform.GetInverse() * t;
						//BMatrix matA = BulletPhysicsUtility.Convert( constraintAFrame );
						//BMatrix matB = BulletPhysicsUtility.Convert( bodyBTransform.GetInverse() * t );

						////constraintAFrame = bodyA.Transform.Value.ToMat4().GetInverse() * t;
						////Matrix matA = BulletUtils.Convert( constraintAFrame );
						////Matrix matB = BulletUtils.Convert( bodyB.Transform.Value.ToMat4().GetInverse() * t );

						//constraintRigid = new Generic6DofSpring2Constraint( (Internal.BulletSharp.RigidBody)creationBodyA.BulletBody, (Internal.BulletSharp.RigidBody)creationBodyB.BulletBody, matA, matB );
					}
				}
				else
				{
					//!!!!need?

					//var body = creationBodyA ?? creationBodyB;

					//var bodyTransformFull = body.Transform.Value;
					//var bodyTransform = new Matrix4( bodyTransformFull.Rotation.ToMatrix3(), bodyTransformFull.Position );
					//constraintAFrame = bodyTransform.GetInverse() * t;

					//var mat = BulletPhysicsUtility.Convert( constraintAFrame );
					//constraintRigid = new Generic6DofSpring2Constraint( (Internal.BulletSharp.RigidBody)body.BulletBody, mat );
				}

				if( physicalConstraint != null )
				{
					physicalConstraint.DisposedEvent += PhysicalConstraint_DisposedEvent;

					if( !CollisionsBetweenLinkedBodies )
						physicalConstraint.SetCollisionsBetweenLinkedBodies( false );
					if( NumVelocityStepsOverride != 0 )
						physicalConstraint.SetNumVelocityStepsOverride( NumVelocityStepsOverride );
					if( NumPositionStepsOverride != 0 )
						physicalConstraint.SetNumPositionStepsOverride( NumPositionStepsOverride );
					if( !Simulate )
						physicalConstraint.SetSimulate( false );
				}

				var sixDOF = physicalConstraint as Scene.PhysicsWorldClass.SixDOFConstraint;
				if( sixDOF != null )
				{
					UpdateMotor( sixDOF, Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX );
					UpdateMotor( sixDOF, Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY );
					UpdateMotor( sixDOF, Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ );
					UpdateMotor( sixDOF, Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
					UpdateMotor( sixDOF, Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
					UpdateMotor( sixDOF, Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );


					//constraintRigid.BreakingImpulseThreshold = BreakingImpulseThreshold;

				}
			}

			duringCreateDestroy = false;



			//if( constraintRigid != null )
			//	Log.Fatal( "Constraint: CreateConstraint: creationConstraintRigid != null." );
			//if( !EnabledInHierarchy )
			//	Log.Fatal( "Constraint: CreateConstraint: !EnabledInHierarchy." );

			//duringCreateDestroy = true;
			//try
			//{
			//	var ba = BodyA.Value;
			//	var bb = BodyB.Value;

			//	//check bodies are enabled
			//	if( ba != null && ba.BulletBody == null )
			//		return;
			//	if( bb != null && bb.BulletBody == null )
			//		return;
			//	//check no bodies
			//	if( ba == null && bb == null )
			//		return;

			//	created = true;
			//	creationBodyA = ba;
			//	creationBodyB = bb;

			//	if( SoftBodyMode )
			//	{
			//		GetSoftBodyModeBodies( out var body1, out var body2 );

			//		softAnchorModeClosestNodeIndex = body1.FindClosestNodeIndex( Transform.Value.Position );
			//		//closestFaceIdx = softComp.FindClosestFaceIndex( Transform.Value.Position );

			//		//real creation only in simulation
			//		if( EngineApp.IsSimulation )
			//		{
			//			if( SoftNodeAnchorMode )// all linear and angular are locked
			//			{
			//				var b1 = (Internal.BulletSharp.SoftBody.SoftBody)body1.BulletBody;
			//				if( body2 == null )
			//					b1.SetMass( softAnchorModeClosestNodeIndex, 0 );
			//				else
			//				{
			//					b1.AppendAnchor( softAnchorModeClosestNodeIndex, (Internal.BulletSharp.RigidBody)body2.BulletBody, !CollisionsBetweenLinkedBodies.Value );

			//					//var vector = BulletUtils.Convert( Transform.Value.Position - body1.GetNodePosition( softAnchorModeClosestNodeIndex ) );
			//					//b1.AppendAnchor( softAnchorModeClosestNodeIndex, (RigidBody)body2.BulletBody, vector, !CollisionsBetweenLinkedBodies.Value );
			//				}
			//			}
			//			else
			//			{
			//				//!!!!

			//				Log.Warning( "Constraint: CreateConstraint: Free, Limited axes are not supported for soft body. No implementation." );

			//				//var t = Transform.Value;

			//				//if( AllLinear( PhysicsAxisMode.Free ) )
			//				//	AppendLinearConstraint( body1, body2, t.Position );

			//				//if( LinearXAxis.Value == PhysicsAxisMode.Limited )
			//				//{
			//				//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( LinearXAxisLimitHigh.Value, 0, 0 ), .5, .5, 1 );
			//				//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( LinearXAxisLimitLow.Value, 0, 0 ), .5, .5, 1 );
			//				//}

			//				//if( LinearYAxis.Value == PhysicsAxisMode.Limited )
			//				//{
			//				//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( 0, LinearYAxisLimitHigh.Value, 0 ), .5, .5, 1 );
			//				//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( 0, LinearYAxisLimitLow.Value, 0 ), .5, .5, 1 );
			//				//}

			//				//if( LinearZAxis.Value == PhysicsAxisMode.Limited )
			//				//{
			//				//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( 0, 0, LinearZAxisLimitHigh.Value ), .5, .5, 1 );
			//				//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( 0, 0, LinearZAxisLimitLow.Value ), .5, .5, 1 );
			//				//}

			//				//if( AngularXAxis.Value == PhysicsAxisMode.Limited )
			//				//	AppendAngularConstraint( body1, body2, Vec3.XAxis, .5, .5, 1 );
			//				//if( AngularYAxis.Value == PhysicsAxisMode.Limited )
			//				//	AppendAngularConstraint( body1, body2, Vec3.YAxis, .5, .5, 1 );
			//				//if( AngularZAxis.Value == PhysicsAxisMode.Limited )
			//				//	AppendAngularConstraint( body1, body2, Vec3.ZAxis, .5, .5, 1 );
			//			}
			//		}
			//	}
			//	else
			//	{
			//		//rigid bodies only

			//		//transform without Scale
			//		var transform = Transform.Value;
			//		var t = new Matrix4( transform.Rotation.ToMatrix3(), transform.Position );

			//		if( creationBodyA != null && creationBodyB != null )
			//		{
			//			//!!!!need?
			//			//bodyB.ActivationState = ActivationState.DisableDeactivation;

			//			var bodyATransformFull = creationBodyA.Transform.Value;
			//			var bodyATransform = new Matrix4( bodyATransformFull.Rotation.ToMatrix3(), bodyATransformFull.Position );
			//			var bodyBTransformFull = creationBodyB.Transform.Value;
			//			var bodyBTransform = new Matrix4( bodyBTransformFull.Rotation.ToMatrix3(), bodyBTransformFull.Position );
			//			constraintAFrame = bodyATransform.GetInverse() * t;
			//			BMatrix matA = BulletPhysicsUtility.Convert( constraintAFrame );
			//			BMatrix matB = BulletPhysicsUtility.Convert( bodyBTransform.GetInverse() * t );

			//			//constraintAFrame = bodyA.Transform.Value.ToMat4().GetInverse() * t;
			//			//Matrix matA = BulletUtils.Convert( constraintAFrame );
			//			//Matrix matB = BulletUtils.Convert( bodyB.Transform.Value.ToMat4().GetInverse() * t );

			//			constraintRigid = new Generic6DofSpring2Constraint( (Internal.BulletSharp.RigidBody)creationBodyA.BulletBody, (Internal.BulletSharp.RigidBody)creationBodyB.BulletBody, matA, matB );
			//		}
			//		else
			//		{
			//			var body = creationBodyA ?? creationBodyB;

			//			var bodyTransformFull = body.Transform.Value;
			//			var bodyTransform = new Matrix4( bodyTransformFull.Rotation.ToMatrix3(), bodyTransformFull.Position );
			//			constraintAFrame = bodyTransform.GetInverse() * t;

			//			var mat = BulletPhysicsUtility.Convert( constraintAFrame );
			//			constraintRigid = new Generic6DofSpring2Constraint( (Internal.BulletSharp.RigidBody)body.BulletBody, mat );
			//		}

			//		if( constraintRigid != null )
			//		{
			//			UpdateLinearLimits( constraintRigid );
			//			UpdateLinearMotors( constraintRigid );
			//			UpdateLinearSprings( constraintRigid );

			//			UpdateAngularLimits( constraintRigid );
			//			UpdateAngularMotors( constraintRigid );
			//			UpdateAngularSprings( constraintRigid );

			//			constraintRigid.BreakingImpulseThreshold = BreakingImpulseThreshold;
			//			constraintRigid.OverrideNumSolverIterations = OverrideNumberSolverIterations;
			//			constraintRigid.Userobject = this;
			//			ParentScene.PhysicsWorldData.world.AddConstraint( constraintRigid, !CollisionsBetweenLinkedBodies );

			//			////add actions to automatic destroy before bodies deletion
			//			//bodyA.AddActionBeforeBodyDestroy( DestroyConstraint );
			//			//bodyB.AddActionBeforeBodyDestroy( DestroyConstraint );
			//		}

			//	}
			//}
			//finally
			//{
			//	duringCreateDestroy = false;
			//}
		}

		private void PhysicalConstraint_DisposedEvent( Scene.PhysicsWorldClass.Constraint obj )
		{
			DestroyConstraint();
		}

		public void DestroyConstraint()
		{
			duringCreateDestroy = true;

			//!!!!что еще удалять?

			if( physicalConstraint != null )
			{
				physicalConstraint.DisposedEvent -= PhysicalConstraint_DisposedEvent;
				physicalConstraint.Dispose();
			}

			created = false;
			creationBodyA = null;
			creationBodyB = null;
			physicalConstraint = null;
			//!!!!softAnchorModeClosestNodeIndex = -1;

			duringCreateDestroy = false;




			//duringCreateDestroy = true;

			//var physicsData = GetPhysicsWorldData();
			//if( physicsData != null )
			//{
			//	if( constraintRigid != null )
			//	{
			//		//!!!!что еще удалять?

			//		//OnBeforeDestroyConstraint();

			//		physicsData.world.RemoveConstraint( constraintRigid );

			//		//!!!!всё равно падает
			//		////!!!!new
			//		////remove from rigid bodies
			//		//var bodyA = constraintRigid.RigidBodyA;
			//		//var bodyB = constraintRigid.RigidBodyB;
			//		//bodyA?.RemoveConstraintRef( constraintRigid );
			//		//bodyB?.RemoveConstraintRef( constraintRigid );

			//		constraintRigid.Dispose();
			//	}

			//	//!!!!remove soft body joints
			//	//!!!!remove anchors
			//}

			//created = false;
			//creationBodyA = null;
			//creationBodyB = null;
			//constraintRigid = null;
			//softAnchorModeClosestNodeIndex = -1;

			//duringCreateDestroy = false;
		}

		public void RecreateConstraint()
		{
			if( EnabledInHierarchy && !duringCreateDestroy )
			{
				DestroyConstraint();
				CreateConstraint();
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( GetPhysicsWorldData() != null )
			{
				if( EnabledInHierarchy )
				{
					if( !created )
						CreateConstraint();
				}
				else
					DestroyConstraint();
			}
		}

		protected override void OnTransformChanged()
		{
			if( created && !setTransformWithoutUpdatingConstraint )
			//if( ConstraintRigid != null && !setTransformWithoutUpdatingConstraint )
			{
				//!!!!обязательно ли пересоздавать?

				//recreate
				RecreateConstraint();
			}

			base.OnTransformChanged();

			//closestNodeIdx = -1;
			////closestFaceIdx = -1;

			//if( BodyA.Value is SoftBody softComp )
			//{
			//	closestNodeIdx = softComp.FindClosestNodeIndex( Transform.Value.Position );
			//	//closestFaceIdx = softComp.FindClosestFaceIndex( Transform.Value.Position );
			//}
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			return base.OnEnabledSelectionByCursor();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			//create when must be created. can happen after undo of deletion physical body.
			if( EnabledInHierarchy && !created && GetPhysicsWorldData() != null )
				CreateConstraint();

			if( needUpdateMotors != 0 )
			{
				var sixDOF = physicalConstraint as Scene.PhysicsWorldClass.SixDOFConstraint;
				if( sixDOF != null )
				{
					for( int n = 0; n < 6; n++ )
					{
						if( ( needUpdateMotors & ( 1 << n ) ) != 0 )
							UpdateMotor( sixDOF, (Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum)n );
					}
				}
				needUpdateMotors = 0;
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//!!!!было в bullet
			//if( created && AutoActivateBodies )
			//{
			//	if( LinearXAxisMotorTargetVelocity != 0 && LinearXAxisMotorMaxForce != 0 ||
			//		LinearYAxisMotorTargetVelocity != 0 && LinearYAxisMotorMaxForce != 0 ||
			//		LinearZAxisMotorTargetVelocity != 0 && LinearZAxisMotorMaxForce != 0 ||
			//		AngularXAxisMotorTargetVelocity != 0 && AngularXAxisMotorMaxForce != 0 ||
			//		AngularYAxisMotorTargetVelocity != 0 && AngularYAxisMotorMaxForce != 0 ||
			//		AngularZAxisMotorTargetVelocity != 0 && AngularZAxisMotorMaxForce != 0 )
			//	{
			//		var bodyA = creationBodyA as RigidBody;
			//		if( bodyA != null && bodyA.MotionType.Value == PhysicsMotionType.Dynamic && !bodyA.Active )
			//			bodyA.Activate();

			//		var bodyB = creationBodyB as RigidBody;
			//		if( bodyB != null && bodyB.MotionType.Value == PhysicsMotionType.Dynamic && !bodyB.Active )
			//			bodyB.Activate();
			//	}
			//}
		}

		//public void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered )
		//{
		//	verticesRendered = 0;

		//	var context2 = context.ObjectInSpaceRenderingContext;

		//	//!!!!
		//	//if( SoftBodyMode )
		//	//{
		//	//	GetSoftBodyModeBodies( out var body1, out var body2 );

		//	//	if( SoftNodeAnchorMode )
		//	//	{
		//	//		var renderer = context.Owner.Simple3DRenderer;

		//	//		var pos = body1.GetNodePosition( softAnchorModeClosestNodeIndex );
		//	//		var thickness = renderer.GetThicknessByPixelSize( pos, 5 );
		//	//		renderer.SetColor( new ColorValue( 0, 1, 0 ) );
		//	//		renderer.AddSphere( pos, thickness );
		//	//		verticesRendered += 32 * 3 * 8;

		//	//		if( body2 != null )
		//	//		{
		//	//			renderer.SetColor( new ColorValue( 0, 0, 0 ) );
		//	//			renderer.AddLine( pos, Transform.Value.Position );
		//	//			renderer.AddLine( Transform.Value.Position, body2.Transform.Value.Position );
		//	//			verticesRendered += 16;
		//	//		}
		//	//	}
		//	//	else
		//	//	{
		//	//		//!!!!

		//	//		var trans = Transform.Value;

		//	//		if( AllLinear( PhysicsAxisMode.Free ) )
		//	//		{
		//	//			context.Owner.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 0 ) );

		//	//			if( body2 == null )
		//	//			{
		//	//				context.Owner.Simple3DRenderer.AddLine( trans.Position, ( (ObjectInSpace)body1 ).Transform.Value.Position );
		//	//				verticesRendered += 8;
		//	//			}
		//	//			else
		//	//			{
		//	//				context.Owner.Simple3DRenderer.AddLine( trans.Position, ( (ObjectInSpace)body1 ).Transform.Value.Position );
		//	//				context.Owner.Simple3DRenderer.AddLine( trans.Position, ( (ObjectInSpace)body2 ).Transform.Value.Position );
		//	//				verticesRendered += 16;
		//	//			}
		//	//		}

		//	//		if( LinearXAxis.Value == PhysicsAxisMode.Limited )
		//	//		{
		//	//			var p1 = trans.Position + new Vector3( LinearXAxisLimitHigh.Value, 0, 0 );
		//	//			var p2 = trans.Position + new Vector3( LinearXAxisLimitLow.Value, 0, 0 );
		//	//			DrawLinearLimitedJoint( context2, p1, p2, body1, body2, ref verticesRendered );
		//	//		}
		//	//		if( LinearYAxis.Value == PhysicsAxisMode.Limited )
		//	//		{
		//	//			var p1 = trans.Position + new Vector3( 0, LinearYAxisLimitHigh.Value, 0 );
		//	//			var p2 = trans.Position + new Vector3( 0, LinearYAxisLimitLow.Value, 0 );
		//	//			DrawLinearLimitedJoint( context2, p1, p2, body1, body2, ref verticesRendered );
		//	//		}
		//	//		if( LinearZAxis.Value == PhysicsAxisMode.Limited )
		//	//		{
		//	//			var p1 = trans.Position + new Vector3( 0, 0, LinearZAxisLimitHigh.Value );
		//	//			var p2 = trans.Position + new Vector3( 0, 0, LinearZAxisLimitLow.Value );
		//	//			DrawLinearLimitedJoint( context2, p1, p2, body1, body2, ref verticesRendered );
		//	//		}
		//	//	}
		//	//}
		//	//else
		//	{
		//		if( physicalConstraint != null )//&& !ParentScene.DisplayPhysicsInternal )
		//		{
		//			UpdateDebugDrawSize( context.Owner );

		//			//!!!!

		//			//var physicsData = GetPhysicsWorldData();
		//			//physicsData.debugDraw.verticesRenderedCounter = 0;
		//			//physicsData.world.DebugDrawConstraint( InternalConstraintRigid );
		//			//verticesRendered += physicsData.debugDraw.verticesRenderedCounter;
		//			//physicsData.debugDraw.verticesRenderedCounter = 0;
		//		}
		//	}

		//	//if( !Broken )
		//	//{
		//	//	Vec3 anchor = Anchor;
		//	//	Vec3 halfDir = Axis.Direction * .5f;
		//	//	debugGeometry.AddLine( anchor - halfDir, anchor + halfDir );
		//	//}

		//}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				bool show = context.SceneDisplayDevelopmentDataInThisApplication && ParentScene.DisplayPhysicalObjects;
#if !DEPLOY
				if( !show )
					show = context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
#endif

				if( show && physicalConstraint != null )
				{
					var verticesRendered = 0;
					physicalConstraint.RenderPhysicalObject( context, ref verticesRendered );
				}

				var showLabels = show;// /*show &&*/ physicalConstraint == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;

				//var showLabels = /*show &&*/ physicalConstraint == null;
				//if( !showLabels )
				//	context2.disableShowingLabelForThisObject = true;
			}
		}

		//protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		//{
		//	base.OnGetRenderSceneData( context, mode );

		//	if( mode == GetRenderSceneDataMode.InsideFrustum )
		//	{
		//		var context2 = context.objectInSpaceRenderingContext;

		//		bool show = ( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplayPhysicalObjects ) ||
		//			context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.dragDropCreateObject == this;
		//		if( show && context.Owner.Simple3DRenderer != null )
		//		{
		//			if( context2.displayPhysicalObjectsCounter < context2.displayPhysicalObjectsMax )
		//			{
		//				context2.displayPhysicalObjectsCounter++;

		//				if( SoftBodyMode )
		//				{
		//					GetSoftBodyModeBodies( out var body1, out var body2 );

		//					if( SoftNodeAnchorMode )
		//					{
		//						var renderer = context.Owner.Simple3DRenderer;

		//						var pos = body1.GetNodePosition( softAnchorModeClosestNodeIndex );
		//						var thickness = renderer.GetThicknessByPixelSize( pos, 5 );
		//						renderer.SetColor( new ColorValue( 0, 1, 0 ) );
		//						renderer.AddSphere( pos, thickness );

		//						if( body2 != null )
		//						{
		//							renderer.SetColor( new ColorValue( 0, 0, 0 ) );
		//							renderer.AddLine( pos, Transform.Value.Position );
		//							renderer.AddLine( Transform.Value.Position, body2.Transform.Value.Position );
		//						}
		//					}
		//					else
		//					{
		//						//!!!!

		//						var trans = Transform.Value;

		//						if( AllLinear( PhysicsAxisMode.Free ) )
		//						{
		//							context.Owner.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 0 ) );

		//							if( body2 == null )
		//								context.Owner.Simple3DRenderer.AddLine( trans.Position, body1.Transform.Value.Position );
		//							else
		//							{
		//								context.Owner.Simple3DRenderer.AddLine( trans.Position, body1.Transform.Value.Position );
		//								context.Owner.Simple3DRenderer.AddLine( trans.Position, body2.Transform.Value.Position );
		//							}
		//						}

		//						if( LinearXAxis.Value == PhysicsAxisMode.Limited )
		//						{
		//							var p1 = trans.Position + new Vector3( LinearXAxisLimitHigh.Value, 0, 0 );
		//							var p2 = trans.Position + new Vector3( LinearXAxisLimitLow.Value, 0, 0 );
		//							DrawLinearLimitedJoint( context2, p1, p2, body1, body2 );
		//						}
		//						if( LinearYAxis.Value == PhysicsAxisMode.Limited )
		//						{
		//							var p1 = trans.Position + new Vector3( 0, LinearYAxisLimitHigh.Value, 0 );
		//							var p2 = trans.Position + new Vector3( 0, LinearYAxisLimitLow.Value, 0 );
		//							DrawLinearLimitedJoint( context2, p1, p2, body1, body2 );
		//						}
		//						if( LinearZAxis.Value == PhysicsAxisMode.Limited )
		//						{
		//							var p1 = trans.Position + new Vector3( 0, 0, LinearZAxisLimitHigh.Value );
		//							var p2 = trans.Position + new Vector3( 0, 0, LinearZAxisLimitLow.Value );
		//							DrawLinearLimitedJoint( context2, p1, p2, body1, body2 );
		//						}
		//					}
		//				}
		//				else
		//				{
		//					if( ConstraintRigid != null && !ParentScene.DisplayPhysicsInternal )
		//					{
		//						UpdateDebugDrawSize( context.Owner );
		//						GetPhysicsWorldData().world.DebugDrawConstraint( ConstraintRigid );
		//					}
		//				}

		//				//if( !Broken )
		//				//{
		//				//	Vec3 anchor = Anchor;
		//				//	Vec3 halfDir = Axis.Direction * .5f;
		//				//	debugGeometry.AddLine( anchor - halfDir, anchor + halfDir );
		//				//}
		//			}
		//		}
		//	}
		//}

		//void DrawLinearLimitedJoint( RenderingContext context, Vector3 p1, Vector3 p2, SoftBody body1, ObjectInSpace/*PhysicalBody*/ body2, ref int verticesRendered )
		//{
		//	context.viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 0 ) );

		//	context.viewport.Simple3DRenderer.AddLine( p1, ( (ObjectInSpace)body1 ).Transform.Value.Position );
		//	context.viewport.Simple3DRenderer.AddLine( p2, ( (ObjectInSpace)body1 ).Transform.Value.Position );
		//	verticesRendered += 16;

		//	if( body2 != null )
		//	{
		//		context.viewport.Simple3DRenderer.AddLine( p1, body2.Transform.Value.Position );
		//		context.viewport.Simple3DRenderer.AddLine( p2, body2.Transform.Value.Position );
		//		verticesRendered += 16;
		//	}
		//	else
		//	{
		//		var thickness = context.viewport.Simple3DRenderer.GetThicknessByPixelSize( p1, 5 );
		//		context.viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0 ) );
		//		context.viewport.Simple3DRenderer.AddSphere( p1, thickness );
		//		verticesRendered += 32 * 3 * 8;
		//		context.viewport.Simple3DRenderer.AddSphere( p2, thickness );
		//		verticesRendered += 32 * 3 * 8;
		//	}
		//}
		//}

		public void SetTransformWithoutUpdatingConstraint( Transform transform )
		{
			try
			{
				setTransformWithoutUpdatingConstraint = true;
				Transform = transform;
			}
			finally
			{
				setTransformWithoutUpdatingConstraint = false;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Linear X-axis constraint mode.
		/// </summary>
		[DefaultValue( PhysicsAxisMode.Locked )]
		[DisplayName( "Linear X Axis" )]
		[Category( "Linear X Axis" )]
		public Reference<PhysicsAxisMode> LinearXAxis
		{
			get { if( _linearXAxis.BeginGet() ) LinearXAxis = _linearXAxis.Get( this ); return _linearXAxis.value; }
			set
			{
				if( _linearXAxis.BeginSet( ref value ) )
				{
					try
					{
						LinearXAxisChanged?.Invoke( this );

						//!!!!impl dynamic update
					}
					finally { _linearXAxis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearXAxis"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearXAxisChanged;
		ReferenceField<PhysicsAxisMode> _linearXAxis = PhysicsAxisMode.Locked;

		/// <summary>
		/// The limits of the linear axis X.
		/// </summary>
		[DefaultValue( "-1 1" )]
		[DisplayName( "Linear X Axis Limit" )]
		[Category( "Linear X Axis" )]
		public Reference<Range> LinearXAxisLimit
		{
			get { if( _linearXAxisLimit.BeginGet() ) LinearXAxisLimit = _linearXAxisLimit.Get( this ); return _linearXAxisLimit.value; }
			set { if( _linearXAxisLimit.BeginSet( ref value ) ) { try { LinearXAxisLimitChanged?.Invoke( this ); UpdateLimit( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearXAxisLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearXAxisLimitChanged;
		ReferenceField<Range> _linearXAxisLimit = new Range( -1, 1 );

		/// <summary>
		/// The friction of linear axis X.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Linear X Axis Friction" )]
		[Category( "Linear X Axis" )]
		public Reference<double> LinearXAxisFriction
		{
			get { if( _linearXAxisFriction.BeginGet() ) LinearXAxisFriction = _linearXAxisFriction.Get( this ); return _linearXAxisFriction.value; }
			set { if( _linearXAxisFriction.BeginSet( ref value ) ) { try { LinearXAxisFrictionChanged?.Invoke( this ); UpdateFriction( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearXAxisFriction"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearXAxisFrictionChanged;
		ReferenceField<double> _linearXAxisFriction = 0.0;

		/// <summary>
		/// The motor mode of linear axis X.
		/// </summary>
		[DefaultValue( Scene.PhysicsWorldClass.MotorModeEnum.None )]
		[DisplayName( "Linear X Axis Motor" )]
		[Category( "Linear X Axis" )]
		public Reference<Scene.PhysicsWorldClass.MotorModeEnum> LinearXAxisMotor
		{
			get { if( _linearXAxisMotor.BeginGet() ) LinearXAxisMotor = _linearXAxisMotor.Get( this ); return _linearXAxisMotor.value; }
			set { if( _linearXAxisMotor.BeginSet( ref value ) ) { try { LinearXAxisMotorChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisMotor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearXAxisMotor"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearXAxisMotorChanged;
		ReferenceField<Scene.PhysicsWorldClass.MotorModeEnum> _linearXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.None;

		/// <summary>
		/// Oscillation frequency when solving position target (Hz). Should be in the range (0, 0.5 * simulation frequency]. When simulating at 60 Hz, 20 is a good value for a strong motor. Only used for position motors.
		/// </summary>
		[DefaultValue( 2.0 )]
		[DisplayName( "Linear X Axis Motor Frequency" )]
		[Category( "Linear X Axis" )]
		public Reference<double> LinearXAxisMotorFrequency
		{
			get { if( _linearXAxisMotorFrequency.BeginGet() ) LinearXAxisMotorFrequency = _linearXAxisMotorFrequency.Get( this ); return _linearXAxisMotorFrequency.value; }
			set { if( _linearXAxisMotorFrequency.BeginSet( ref value ) ) { try { LinearXAxisMotorFrequencyChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisMotorFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearXAxisMotorFrequency"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearXAxisMotorFrequencyChanged;
		ReferenceField<double> _linearXAxisMotorFrequency = 2.0;

		/// <summary>
		/// Damping when solving position target (0 = minimal damping, 1 = critical damping). Only used for position motors.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "Linear X Axis Motor Damping" )]
		[Category( "Linear X Axis" )]
		public Reference<double> LinearXAxisMotorDamping
		{
			get { if( _linearXAxisMotorDamping.BeginGet() ) LinearXAxisMotorDamping = _linearXAxisMotorDamping.Get( this ); return _linearXAxisMotorDamping.value; }
			set { if( _linearXAxisMotorDamping.BeginSet( ref value ) ) { try { LinearXAxisMotorDampingChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisMotorDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearXAxisMotorDamping"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearXAxisMotorDampingChanged;
		ReferenceField<double> _linearXAxisMotorDamping = 1.0;

		/// <summary>
		///Force range to apply in case of a linear constraint (N). Usually this is -mMaxForceLimit unless you want a motor that can e.g. push but not pull. Not used when motor is an angular motor.
		/// </summary>
		[DefaultValue( "-1000000 1000000" )]
		[DisplayName( "Linear X Axis Motor Limit" )]
		[Category( "Linear X Axis" )]
		public Reference<Range> LinearXAxisMotorLimit
		{
			get { if( _linearXAxisMotorLimit.BeginGet() ) LinearXAxisMotorLimit = _linearXAxisMotorLimit.Get( this ); return _linearXAxisMotorLimit.value; }
			set { if( _linearXAxisMotorLimit.BeginSet( ref value ) ) { try { LinearXAxisMotorLimitChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisMotorLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearXAxisMotorLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearXAxisMotorLimitChanged;
		ReferenceField<Range> _linearXAxisMotorLimit = new Range( -1000000, 1000000 );

		/// <summary>
		/// The target value of the linear axis X motor. It is a velocity or a position depending the mode.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Linear X Axis Motor Target" )]
		[Category( "Linear X Axis" )]
		public Reference<double> LinearXAxisMotorTarget
		{
			get { if( _linearXAxisMotorTarget.BeginGet() ) LinearXAxisMotorTarget = _linearXAxisMotorTarget.Get( this ); return _linearXAxisMotorTarget.value; }
			set { if( _linearXAxisMotorTarget.BeginSet( ref value ) ) { try { LinearXAxisMotorTargetChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisMotorTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearXAxisMotorTarget"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearXAxisMotorTargetChanged;
		ReferenceField<double> _linearXAxisMotorTarget = 0.0;



		///// <summary>
		///// Torque range to apply in case of a angular constraint (N m). Usually this is -mMaxTorqueLimit unless you want a motor that can e.g. push but not pull. Not used when motor is a position motor.
		///// </summary>
		//[DefaultValue( "-1000000 1000000" )]
		//[DisplayName( "Linear X Axis Motor Torque Limit" )]
		//[Category( "Linear X Axis" )]
		//public Reference<Range> LinearXAxisMotorTorqueLimit
		//{
		//	get { if( _linearXAxisMotorTorqueLimit.BeginGet() ) LinearXAxisMotorTorqueLimit = _linearXAxisMotorTorqueLimit.Get( this ); return _linearXAxisMotorTorqueLimit.value; }
		//	set { if( _linearXAxisMotorTorqueLimit.BeginSet( ref value ) ) { try { LinearXAxisMotorTorqueLimitChanged?.Invoke( this ); UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisMotorTorqueLimit.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisMotorTorqueLimit"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisMotorTorqueLimitChanged;
		//ReferenceField<Range> _linearXAxisMotorTorqueLimit = new Range( -1000000, 1000000 );


		////LinearXAxisMotor
		//ReferenceField<bool> _linearXAxisMotor = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Linear X Axis Motor" )]//[DisplayName( "Motor" )]
		//[Category( "Linear X Axis" )]
		//public Reference<bool> LinearXAxisMotor
		//{
		//	get
		//	{
		//		if( _linearXAxisMotor.BeginGet() )
		//			LinearXAxisMotor = _linearXAxisMotor.Get( this );
		//		return _linearXAxisMotor.value;
		//	}
		//	set
		//	{
		//		if( _linearXAxisMotor.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearXAxisMotorChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearXAxisMotor.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisMotor"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisMotorChanged;

		///// <summary>
		///// The target velocity of the linear axis X motor.
		///// </summary>
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Linear X Axis Motor Target Velocity" )]
		//[Category( "Linear X Axis" )]
		//public Reference<double> LinearXAxisMotorTargetVelocity
		//{
		//	get { if( _linearXAxisMotorTargetVelocity.BeginGet() ) LinearXAxisMotorTargetVelocity = _linearXAxisMotorTargetVelocity.Get( this ); return _linearXAxisMotorTargetVelocity.value; }
		//	set { if( _linearXAxisMotorTargetVelocity.BeginSet( ref value ) ) { try { LinearXAxisMotorTargetVelocityChanged?.Invoke( this ); UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisMotorTargetVelocity.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisMotorTargetVelocity"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisMotorTargetVelocityChanged;
		//ReferenceField<double> _linearXAxisMotorTargetVelocity = 0.0;

		///// <summary>
		///// The target position of the linear axis X motor.
		///// </summary>
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Linear X Axis Motor Target Position" )]
		//[Category( "Linear X Axis" )]
		//public Reference<double> LinearXAxisMotorTargetPosition
		//{
		//	get { if( _linearXAxisMotorTargetPosition.BeginGet() ) LinearXAxisMotorTargetPosition = _linearXAxisMotorTargetPosition.Get( this ); return _linearXAxisMotorTargetPosition.value; }
		//	set { if( _linearXAxisMotorTargetPosition.BeginSet( ref value ) ) { try { LinearXAxisMotorTargetPositionChanged?.Invoke( this ); UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearXAxisMotorTargetPosition.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisMotorTargetPosition"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisMotorTargetPositionChanged;
		//ReferenceField<double> _linearXAxisMotorTargetPosition = 0.0;


		////LinearXAxisMotorMaxForce
		//ReferenceField<double> _linearXAxisMotorMaxForce = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear X Axis Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		//[Category( "Linear X Axis" )]
		//public Reference<double> LinearXAxisMotorMaxForce
		//{
		//	get
		//	{
		//		if( _linearXAxisMotorMaxForce.BeginGet() )
		//			LinearXAxisMotorMaxForce = _linearXAxisMotorMaxForce.Get( this );
		//		return _linearXAxisMotorMaxForce.value;
		//	}
		//	set
		//	{
		//		if( _linearXAxisMotorMaxForce.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearXAxisMotorMaxForceChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearXAxisMotorMaxForce.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisMotorMaxForce"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisMotorMaxForceChanged;

		////LinearXAxisMotorRestitution
		//ReferenceField<double> _linearXAxisMotorRestitution = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear X Axis Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		//[Category( "Linear X Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> LinearXAxisMotorRestitution
		//{
		//	get
		//	{
		//		if( _linearXAxisMotorRestitution.BeginGet() )
		//			LinearXAxisMotorRestitution = _linearXAxisMotorRestitution.Get( this );
		//		return _linearXAxisMotorRestitution.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _linearXAxisMotorRestitution.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearXAxisMotorRestitutionChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearXAxisMotorRestitution.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisMotorRestitution"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisMotorRestitutionChanged;

		////LinearXAxisMotorServo
		//ReferenceField<bool> _linearXAxisMotorServo = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Linear X Axis Motor Servo" )]//[DisplayName( "Motor Servo" )]
		//[Category( "Linear X Axis" )]
		//public Reference<bool> LinearXAxisMotorServo
		//{
		//	get
		//	{
		//		if( _linearXAxisMotorServo.BeginGet() )
		//			LinearXAxisMotorServo = _linearXAxisMotorServo.Get( this );
		//		return _linearXAxisMotorServo.value;
		//	}
		//	set
		//	{
		//		if( _linearXAxisMotorServo.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearXAxisMotorServoChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearXAxisMotorServo.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisMotorServo"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisMotorServoChanged;


		////LinearXAxisMotorServoTarget
		//ReferenceField<double> _linearXAxisMotorServoTarget = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear X Axis Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		//[Category( "Linear X Axis" )]
		//public Reference<double> LinearXAxisMotorServoTarget
		//{
		//	get
		//	{
		//		if( _linearXAxisMotorServoTarget.BeginGet() )
		//			LinearXAxisMotorServoTarget = _linearXAxisMotorServoTarget.Get( this );
		//		return _linearXAxisMotorServoTarget.value;
		//	}
		//	set
		//	{
		//		if( _linearXAxisMotorServoTarget.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearXAxisMotorServoTargetChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearXAxisMotorServoTarget.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisMotorServoTarget"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisMotorServoTargetChanged;

		////LinearXAxisSpring
		//ReferenceField<bool> _linearXAxisSpring = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Linear X Axis Spring" )]//[DisplayName( "Spring" )]
		//[Category( "Linear X Axis" )]
		//public Reference<bool> LinearXAxisSpring
		//{
		//	get
		//	{
		//		if( _linearXAxisSpring.BeginGet() )
		//			LinearXAxisSpring = _linearXAxisSpring.Get( this );
		//		return _linearXAxisSpring.value;
		//	}
		//	set
		//	{
		//		if( _linearXAxisSpring.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearXAxisSpringChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearSprings( InternalConstraintRigid );
		//			}
		//			finally { _linearXAxisSpring.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisSpring"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisSpringChanged;

		////LinearXAxisSpringStiffness
		//ReferenceField<double> _linearXAxisSpringStiffness = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear X Axis Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		//[Category( "Linear X Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> LinearXAxisSpringStiffness
		//{
		//	get
		//	{
		//		if( _linearXAxisSpringStiffness.BeginGet() )
		//			LinearXAxisSpringStiffness = _linearXAxisSpringStiffness.Get( this );
		//		return _linearXAxisSpringStiffness.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _linearXAxisSpringStiffness.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearXAxisSpringStiffnessChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearSprings( InternalConstraintRigid );
		//			}
		//			finally { _linearXAxisSpringStiffness.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisSpringStiffness"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisSpringStiffnessChanged;

		////LinearXAxisSpringDamping
		//ReferenceField<double> _linearXAxisSpringDamping = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear X Axis Spring Damping" )]//[DisplayName( "Spring Damping" )]
		//[Category( "Linear X Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> LinearXAxisSpringDamping
		//{
		//	get
		//	{
		//		if( _linearXAxisSpringDamping.BeginGet() )
		//			LinearXAxisSpringDamping = _linearXAxisSpringDamping.Get( this );
		//		return _linearXAxisSpringDamping.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _linearXAxisSpringDamping.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearXAxisSpringDampingChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearSprings( InternalConstraintRigid );
		//			}
		//			finally { _linearXAxisSpringDamping.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearXAxisSpringDamping"/> property value changes.</summary>
		//public event Action<Constraint> LinearXAxisSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Linear Y-axis constraint mode.
		/// </summary>
		[DefaultValue( PhysicsAxisMode.Locked )]
		[DisplayName( "Linear Y Axis" )]
		[Category( "Linear Y Axis" )]
		public Reference<PhysicsAxisMode> LinearYAxis
		{
			get { if( _linearYAxis.BeginGet() ) LinearYAxis = _linearYAxis.Get( this ); return _linearYAxis.value; }
			set
			{
				if( _linearYAxis.BeginSet( ref value ) )
				{
					try
					{
						LinearYAxisChanged?.Invoke( this );

						//!!!!impl dynamic update
					}
					finally { _linearYAxis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearYAxis"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearYAxisChanged;
		ReferenceField<PhysicsAxisMode> _linearYAxis = PhysicsAxisMode.Locked;

		/// <summary>
		/// The limits of the linear axis Y.
		/// </summary>
		[DefaultValue( "-1 1" )]
		[DisplayName( "Linear Y Axis Limit" )]
		[Category( "Linear Y Axis" )]
		public Reference<Range> LinearYAxisLimit
		{
			get { if( _linearYAxisLimit.BeginGet() ) LinearYAxisLimit = _linearYAxisLimit.Get( this ); return _linearYAxisLimit.value; }
			set { if( _linearYAxisLimit.BeginSet( ref value ) ) { try { LinearYAxisLimitChanged?.Invoke( this ); UpdateLimit( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY ); } finally { _linearYAxisLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearYAxisLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearYAxisLimitChanged;
		ReferenceField<Range> _linearYAxisLimit = new Range( -1, 1 );

		/// <summary>
		/// The friction of linear axis Y.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Linear Y Axis Friction" )]
		[Category( "Linear Y Axis" )]
		public Reference<double> LinearYAxisFriction
		{
			get { if( _linearYAxisFriction.BeginGet() ) LinearYAxisFriction = _linearYAxisFriction.Get( this ); return _linearYAxisFriction.value; }
			set { if( _linearYAxisFriction.BeginSet( ref value ) ) { try { LinearYAxisFrictionChanged?.Invoke( this ); UpdateFriction( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY ); } finally { _linearYAxisFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearYAxisFriction"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearYAxisFrictionChanged;
		ReferenceField<double> _linearYAxisFriction = 0.0;

		/// <summary>
		/// The motor mode of linear axis Y.
		/// </summary>
		[DefaultValue( Scene.PhysicsWorldClass.MotorModeEnum.None )]
		[DisplayName( "Linear Y Axis Motor" )]
		[Category( "Linear Y Axis" )]
		public Reference<Scene.PhysicsWorldClass.MotorModeEnum> LinearYAxisMotor
		{
			get { if( _linearYAxisMotor.BeginGet() ) LinearYAxisMotor = _linearYAxisMotor.Get( this ); return _linearYAxisMotor.value; }
			set { if( _linearYAxisMotor.BeginSet( ref value ) ) { try { LinearYAxisMotorChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY ); } finally { _linearYAxisMotor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearYAxisMotor"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearYAxisMotorChanged;
		ReferenceField<Scene.PhysicsWorldClass.MotorModeEnum> _linearYAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.None;

		/// <summary>
		/// Oscillation frequency when solving position target (Hz). Should be in the range (0, 0.5 * simulation frequency]. When simulating at 60 Hz, 20 is a good value for a strong motor. Only used for position motors.
		/// </summary>
		[DefaultValue( 2.0 )]
		[DisplayName( "Linear Y Axis Motor Frequency" )]
		[Category( "Linear Y Axis" )]
		public Reference<double> LinearYAxisMotorFrequency
		{
			get { if( _linearYAxisMotorFrequency.BeginGet() ) LinearYAxisMotorFrequency = _linearYAxisMotorFrequency.Get( this ); return _linearYAxisMotorFrequency.value; }
			set { if( _linearYAxisMotorFrequency.BeginSet( ref value ) ) { try { LinearYAxisMotorFrequencyChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearYAxisMotorFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearYAxisMotorFrequency"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearYAxisMotorFrequencyChanged;
		ReferenceField<double> _linearYAxisMotorFrequency = 2.0;

		/// <summary>
		/// Damping when solving position target (0 = minimal damping, 1 = critical damping). Only used for position motors.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "Linear Y Axis Motor Damping" )]
		[Category( "Linear Y Axis" )]
		public Reference<double> LinearYAxisMotorDamping
		{
			get { if( _linearYAxisMotorDamping.BeginGet() ) LinearYAxisMotorDamping = _linearYAxisMotorDamping.Get( this ); return _linearYAxisMotorDamping.value; }
			set { if( _linearYAxisMotorDamping.BeginSet( ref value ) ) { try { LinearYAxisMotorDampingChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY ); } finally { _linearYAxisMotorDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearYAxisMotorDamping"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearYAxisMotorDampingChanged;
		ReferenceField<double> _linearYAxisMotorDamping = 1.0;

		/// <summary>
		///Force range to apply in case of a linear constraint (N). Usually this is -mMaxForceLimit unless you want a motor that can e.g. push but not pull. Not used when motor is an angular motor.
		/// </summary>
		[DefaultValue( "-1000000 1000000" )]
		[DisplayName( "Linear Y Axis Motor Limit" )]
		[Category( "Linear Y Axis" )]
		public Reference<Range> LinearYAxisMotorLimit
		{
			get { if( _linearYAxisMotorLimit.BeginGet() ) LinearYAxisMotorLimit = _linearYAxisMotorLimit.Get( this ); return _linearYAxisMotorLimit.value; }
			set { if( _linearYAxisMotorLimit.BeginSet( ref value ) ) { try { LinearYAxisMotorLimitChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY ); } finally { _linearYAxisMotorLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearYAxisMotorLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearYAxisMotorLimitChanged;
		ReferenceField<Range> _linearYAxisMotorLimit = new Range( -1000000, 1000000 );

		/// <summary>
		/// The target value of the linear axis Y motor. It is a velocity or a position depending the mode.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Linear Y Axis Motor Target" )]
		[Category( "Linear Y Axis" )]
		public Reference<double> LinearYAxisMotorTarget
		{
			get { if( _linearYAxisMotorTarget.BeginGet() ) LinearYAxisMotorTarget = _linearYAxisMotorTarget.Get( this ); return _linearYAxisMotorTarget.value; }
			set { if( _linearYAxisMotorTarget.BeginSet( ref value ) ) { try { LinearYAxisMotorTargetChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY ); } finally { _linearYAxisMotorTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearYAxisMotorTarget"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearYAxisMotorTargetChanged;
		ReferenceField<double> _linearYAxisMotorTarget = 0.0;



		////LinearYAxis
		//ReferenceField<PhysicsAxisMode> _linearYAxis = PhysicsAxisMode.Locked;
		///// <summary>
		///// Linear Y-axis constraint mode.
		///// </summary>
		//[DefaultValue( PhysicsAxisMode.Locked )]
		//[Serialize]
		//[Category( "Linear Y Axis" )]
		//public Reference<PhysicsAxisMode> LinearYAxis
		//{
		//	get
		//	{
		//		if( _linearYAxis.BeginGet() )
		//			LinearYAxis = _linearYAxis.Get( this );
		//		return _linearYAxis.value;
		//	}
		//	set
		//	{
		//		if( _linearYAxis.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearLimits( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxis.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxis"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisChanged;

		////LinearYAxisLimitLow
		//ReferenceField<double> _linearYAxisLimitLow = -1.0;
		//[DefaultValue( -1.0 )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Limit Low" )]//[DisplayName( "Limit Low" )]
		//[Category( "Linear Y Axis" )]
		//public Reference<double> LinearYAxisLimitLow
		//{
		//	get
		//	{
		//		if( _linearYAxisLimitLow.BeginGet() )
		//			LinearYAxisLimitLow = _linearYAxisLimitLow.Get( this );
		//		return _linearYAxisLimitLow.value;
		//	}
		//	set
		//	{
		//		if( _linearYAxisLimitLow.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisLimitLowChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearLimits( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisLimitLow.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisLimitLow"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisLimitLowChanged;

		////LinearYAxisLimitHigh
		//ReferenceField<double> _linearYAxisLimitHigh = 1.0;
		//[DefaultValue( 1.0 )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Limit High" )]//[DisplayName( "Limit High" )]
		//[Category( "Linear Y Axis" )]
		//public Reference<double> LinearYAxisLimitHigh
		//{
		//	get
		//	{
		//		if( _linearYAxisLimitHigh.BeginGet() )
		//			LinearYAxisLimitHigh = _linearYAxisLimitHigh.Get( this );
		//		return _linearYAxisLimitHigh.value;
		//	}
		//	set
		//	{
		//		if( _linearYAxisLimitHigh.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisLimitHighChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearLimits( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisLimitHigh.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisLimitHigh"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisLimitHighChanged;

		////LinearYAxisMotor
		//ReferenceField<bool> _linearYAxisMotor = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Motor" )]//[DisplayName( "Motor" )]
		//[Category( "Linear Y Axis" )]
		//public Reference<bool> LinearYAxisMotor
		//{
		//	get
		//	{
		//		if( _linearYAxisMotor.BeginGet() )
		//			LinearYAxisMotor = _linearYAxisMotor.Get( this );
		//		return _linearYAxisMotor.value;
		//	}
		//	set
		//	{
		//		if( _linearYAxisMotor.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisMotorChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisMotor.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisMotor"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisMotorChanged;

		////LinearYAxisMotorTargetVelocity
		//ReferenceField<double> _linearYAxisMotorTargetVelocity = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		//[Category( "Linear Y Axis" )]
		//public Reference<double> LinearYAxisMotorTargetVelocity
		//{
		//	get
		//	{
		//		if( _linearYAxisMotorTargetVelocity.BeginGet() )
		//			LinearYAxisMotorTargetVelocity = _linearYAxisMotorTargetVelocity.Get( this );
		//		return _linearYAxisMotorTargetVelocity.value;
		//	}
		//	set
		//	{
		//		if( _linearYAxisMotorTargetVelocity.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisMotorTargetVelocityChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisMotorTargetVelocity.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisMotorTargetVelocity"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisMotorTargetVelocityChanged;

		////LinearYAxisMotorMaxForce
		//ReferenceField<double> _linearYAxisMotorMaxForce = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		//[Category( "Linear Y Axis" )]
		//public Reference<double> LinearYAxisMotorMaxForce
		//{
		//	get
		//	{
		//		if( _linearYAxisMotorMaxForce.BeginGet() )
		//			LinearYAxisMotorMaxForce = _linearYAxisMotorMaxForce.Get( this );
		//		return _linearYAxisMotorMaxForce.value;
		//	}
		//	set
		//	{
		//		if( _linearYAxisMotorMaxForce.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisMotorMaxForceChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisMotorMaxForce.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisMotorMaxForce"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisMotorMaxForceChanged;

		////LinearYAxisMotorRestitution
		//ReferenceField<double> _linearYAxisMotorRestitution = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		//[Category( "Linear Y Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> LinearYAxisMotorRestitution
		//{
		//	get
		//	{
		//		if( _linearYAxisMotorRestitution.BeginGet() )
		//			LinearYAxisMotorRestitution = _linearYAxisMotorRestitution.Get( this );
		//		return _linearYAxisMotorRestitution.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _linearYAxisMotorRestitution.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisMotorRestitutionChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisMotorRestitution.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisMotorRestitution"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisMotorRestitutionChanged;

		////LinearYAxisMotorServo
		//ReferenceField<bool> _linearYAxisMotorServo = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Motor Servo" )]//[DisplayName( "Motor Servo" )]
		//[Category( "Linear Y Axis" )]
		//public Reference<bool> LinearYAxisMotorServo
		//{
		//	get
		//	{
		//		if( _linearYAxisMotorServo.BeginGet() )
		//			LinearYAxisMotorServo = _linearYAxisMotorServo.Get( this );
		//		return _linearYAxisMotorServo.value;
		//	}
		//	set
		//	{
		//		if( _linearYAxisMotorServo.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisMotorServoChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisMotorServo.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisMotorServo"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisMotorServoChanged;

		////LinearYAxisMotorServoTarget
		//ReferenceField<double> _linearYAxisMotorServoTarget = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		//[Category( "Linear Y Axis" )]
		//public Reference<double> LinearYAxisMotorServoTarget
		//{
		//	get
		//	{
		//		if( _linearYAxisMotorServoTarget.BeginGet() )
		//			LinearYAxisMotorServoTarget = _linearYAxisMotorServoTarget.Get( this );
		//		return _linearYAxisMotorServoTarget.value;
		//	}
		//	set
		//	{
		//		if( _linearYAxisMotorServoTarget.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisMotorServoTargetChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisMotorServoTarget.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisMotorServoTarget"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisMotorServoTargetChanged;

		////LinearYAxisSpring
		//ReferenceField<bool> _linearYAxisSpring = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Spring" )]//[DisplayName( "Spring" )]
		//[Category( "Linear Y Axis" )]
		//public Reference<bool> LinearYAxisSpring
		//{
		//	get
		//	{
		//		if( _linearYAxisSpring.BeginGet() )
		//			LinearYAxisSpring = _linearYAxisSpring.Get( this );
		//		return _linearYAxisSpring.value;
		//	}
		//	set
		//	{
		//		if( _linearYAxisSpring.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisSpringChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearSprings( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisSpring.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisSpring"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisSpringChanged;

		////LinearYAxisSpringStiffness
		//ReferenceField<double> _linearYAxisSpringStiffness = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		//[Category( "Linear Y Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> LinearYAxisSpringStiffness
		//{
		//	get
		//	{
		//		if( _linearYAxisSpringStiffness.BeginGet() )
		//			LinearYAxisSpringStiffness = _linearYAxisSpringStiffness.Get( this );
		//		return _linearYAxisSpringStiffness.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _linearYAxisSpringStiffness.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisSpringStiffnessChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearSprings( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisSpringStiffness.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisSpringStiffness"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisSpringStiffnessChanged;

		////LinearYAxisSpringDamping
		//ReferenceField<double> _linearYAxisSpringDamping = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Y Axis Spring Damping" )]//[DisplayName( "Spring Damping" )]
		//[Category( "Linear Y Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> LinearYAxisSpringDamping
		//{
		//	get
		//	{
		//		if( _linearYAxisSpringDamping.BeginGet() )
		//			LinearYAxisSpringDamping = _linearYAxisSpringDamping.Get( this );
		//		return _linearYAxisSpringDamping.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _linearYAxisSpringDamping.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearYAxisSpringDampingChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearSprings( InternalConstraintRigid );
		//			}
		//			finally { _linearYAxisSpringDamping.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearYAxisSpringDamping"/> property value changes.</summary>
		//public event Action<Constraint> LinearYAxisSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Linear Z-axis constraint mode.
		/// </summary>
		[DefaultValue( PhysicsAxisMode.Locked )]
		[DisplayName( "Linear Z Axis" )]
		[Category( "Linear Z Axis" )]
		public Reference<PhysicsAxisMode> LinearZAxis
		{
			get { if( _linearZAxis.BeginGet() ) LinearZAxis = _linearZAxis.Get( this ); return _linearZAxis.value; }
			set
			{
				if( _linearZAxis.BeginSet( ref value ) )
				{
					try
					{
						LinearZAxisChanged?.Invoke( this );

						//!!!!impl dynamic update
					}
					finally { _linearZAxis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearZAxis"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearZAxisChanged;
		ReferenceField<PhysicsAxisMode> _linearZAxis = PhysicsAxisMode.Locked;

		/// <summary>
		/// The limits of the linear axis Z.
		/// </summary>
		[DefaultValue( "-1 1" )]
		[DisplayName( "Linear Z Axis Limit" )]
		[Category( "Linear Z Axis" )]
		public Reference<Range> LinearZAxisLimit
		{
			get { if( _linearZAxisLimit.BeginGet() ) LinearZAxisLimit = _linearZAxisLimit.Get( this ); return _linearZAxisLimit.value; }
			set { if( _linearZAxisLimit.BeginSet( ref value ) ) { try { LinearZAxisLimitChanged?.Invoke( this ); UpdateLimit( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ ); } finally { _linearZAxisLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearZAxisLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearZAxisLimitChanged;
		ReferenceField<Range> _linearZAxisLimit = new Range( -1, 1 );

		/// <summary>
		/// The friction of linear axis Z.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Linear Z Axis Friction" )]
		[Category( "Linear Z Axis" )]
		public Reference<double> LinearZAxisFriction
		{
			get { if( _linearZAxisFriction.BeginGet() ) LinearZAxisFriction = _linearZAxisFriction.Get( this ); return _linearZAxisFriction.value; }
			set { if( _linearZAxisFriction.BeginSet( ref value ) ) { try { LinearZAxisFrictionChanged?.Invoke( this ); UpdateFriction( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ ); } finally { _linearZAxisFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearZAxisFriction"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearZAxisFrictionChanged;
		ReferenceField<double> _linearZAxisFriction = 0.0;

		/// <summary>
		/// The motor mode of linear axis Z.
		/// </summary>
		[DefaultValue( Scene.PhysicsWorldClass.MotorModeEnum.None )]
		[DisplayName( "Linear Z Axis Motor" )]
		[Category( "Linear Z Axis" )]
		public Reference<Scene.PhysicsWorldClass.MotorModeEnum> LinearZAxisMotor
		{
			get { if( _linearZAxisMotor.BeginGet() ) LinearZAxisMotor = _linearZAxisMotor.Get( this ); return _linearZAxisMotor.value; }
			set { if( _linearZAxisMotor.BeginSet( ref value ) ) { try { LinearZAxisMotorChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX ); } finally { _linearZAxisMotor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearZAxisMotor"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearZAxisMotorChanged;
		ReferenceField<Scene.PhysicsWorldClass.MotorModeEnum> _linearZAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.None;

		/// <summary>
		/// Oscillation frequency when solving position target (Hz). Should be in the range (0, 0.5 * simulation frequency]. When simulating at 60 Hz, 20 is a good value for a strong motor. Only used for position motors.
		/// </summary>
		[DefaultValue( 2.0 )]
		[DisplayName( "Linear Z Axis Motor Frequency" )]
		[Category( "Linear Z Axis" )]
		public Reference<double> LinearZAxisMotorFrequency
		{
			get { if( _linearZAxisMotorFrequency.BeginGet() ) LinearZAxisMotorFrequency = _linearZAxisMotorFrequency.Get( this ); return _linearZAxisMotorFrequency.value; }
			set { if( _linearZAxisMotorFrequency.BeginSet( ref value ) ) { try { LinearZAxisMotorFrequencyChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ ); } finally { _linearZAxisMotorFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearZAxisMotorFrequency"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearZAxisMotorFrequencyChanged;
		ReferenceField<double> _linearZAxisMotorFrequency = 2.0;

		/// <summary>
		/// Damping when solving position target (0 = minimal damping, 1 = critical damping). Only used for position motors.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "Linear Z Axis Motor Damping" )]
		[Category( "Linear Z Axis" )]
		public Reference<double> LinearZAxisMotorDamping
		{
			get { if( _linearZAxisMotorDamping.BeginGet() ) LinearZAxisMotorDamping = _linearZAxisMotorDamping.Get( this ); return _linearZAxisMotorDamping.value; }
			set { if( _linearZAxisMotorDamping.BeginSet( ref value ) ) { try { LinearZAxisMotorDampingChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ ); } finally { _linearZAxisMotorDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearZAxisMotorDamping"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearZAxisMotorDampingChanged;
		ReferenceField<double> _linearZAxisMotorDamping = 1.0;

		/// <summary>
		///Force range to apply in case of a linear constraint (N). Usually this is -mMaxForceLimit unless you want a motor that can e.g. push but not pull. Not used when motor is an angular motor.
		/// </summary>
		[DefaultValue( "-1000000 1000000" )]
		[DisplayName( "Linear Z Axis Motor Limit" )]
		[Category( "Linear Z Axis" )]
		public Reference<Range> LinearZAxisMotorLimit
		{
			get { if( _linearZAxisMotorLimit.BeginGet() ) LinearZAxisMotorLimit = _linearZAxisMotorLimit.Get( this ); return _linearZAxisMotorLimit.value; }
			set { if( _linearZAxisMotorLimit.BeginSet( ref value ) ) { try { LinearZAxisMotorLimitChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ ); } finally { _linearZAxisMotorLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearZAxisMotorLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearZAxisMotorLimitChanged;
		ReferenceField<Range> _linearZAxisMotorLimit = new Range( -1000000, 1000000 );

		/// <summary>
		/// The target value of the linear axis Z motor. It is a velocity or a position depending the mode.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Linear Z Axis Motor Target" )]
		[Category( "Linear Z Axis" )]
		public Reference<double> LinearZAxisMotorTarget
		{
			get { if( _linearZAxisMotorTarget.BeginGet() ) LinearZAxisMotorTarget = _linearZAxisMotorTarget.Get( this ); return _linearZAxisMotorTarget.value; }
			set { if( _linearZAxisMotorTarget.BeginSet( ref value ) ) { try { LinearZAxisMotorTargetChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ ); } finally { _linearZAxisMotorTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearZAxisMotorTarget"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> LinearZAxisMotorTargetChanged;
		ReferenceField<double> _linearZAxisMotorTarget = 0.0;


		////LinearZAxis
		//ReferenceField<PhysicsAxisMode> _linearZAxis = PhysicsAxisMode.Locked;
		///// <summary>
		///// Linear Z-axis constraint mode.
		///// </summary>
		//[Serialize]
		//[DefaultValue( PhysicsAxisMode.Locked )]
		//[Category( "Linear Z Axis" )]
		//public Reference<PhysicsAxisMode> LinearZAxis
		//{
		//	get
		//	{
		//		if( _linearZAxis.BeginGet() )
		//			LinearZAxis = _linearZAxis.Get( this );
		//		return _linearZAxis.value;
		//	}
		//	set
		//	{
		//		if( _linearZAxis.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearLimits( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxis.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxis"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisChanged;

		////LinearZAxisLimitLow
		//ReferenceField<double> _linearZAxisLimitLow = -1.0;
		//[DefaultValue( -1.0 )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Limit Low" )]//[DisplayName( "Limit Low" )]
		//[Category( "Linear Z Axis" )]
		//public Reference<double> LinearZAxisLimitLow
		//{
		//	get
		//	{
		//		if( _linearZAxisLimitLow.BeginGet() )
		//			LinearZAxisLimitLow = _linearZAxisLimitLow.Get( this );
		//		return _linearZAxisLimitLow.value;
		//	}
		//	set
		//	{
		//		if( _linearZAxisLimitLow.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisLimitLowChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearLimits( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisLimitLow.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisLimitLow"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisLimitLowChanged;

		////LinearZAxisLimitHigh
		//ReferenceField<double> _linearZAxisLimitHigh = 1.0;
		//[DefaultValue( 1.0 )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Limit High" )]//[DisplayName( "Limit High" )]
		//[Category( "Linear Z Axis" )]
		//public Reference<double> LinearZAxisLimitHigh
		//{
		//	get
		//	{
		//		if( _linearZAxisLimitHigh.BeginGet() )
		//			LinearZAxisLimitHigh = _linearZAxisLimitHigh.Get( this );
		//		return _linearZAxisLimitHigh.value;
		//	}
		//	set
		//	{
		//		if( _linearZAxisLimitHigh.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisLimitHighChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearLimits( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisLimitHigh.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisLimitHigh"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisLimitHighChanged;

		////LinearZAxisMotor
		//ReferenceField<bool> _linearZAxisMotor = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Motor" )]//[DisplayName( "Motor" )]
		//[Category( "Linear Z Axis" )]
		//public Reference<bool> LinearZAxisMotor
		//{
		//	get
		//	{
		//		if( _linearZAxisMotor.BeginGet() )
		//			LinearZAxisMotor = _linearZAxisMotor.Get( this );
		//		return _linearZAxisMotor.value;
		//	}
		//	set
		//	{
		//		if( _linearZAxisMotor.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisMotorChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisMotor.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisMotor"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisMotorChanged;

		////LinearZAxisMotorTargetVelocity
		//ReferenceField<double> _linearZAxisMotorTargetVelocity = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		//[Category( "Linear Z Axis" )]
		//public Reference<double> LinearZAxisMotorTargetVelocity
		//{
		//	get
		//	{
		//		if( _linearZAxisMotorTargetVelocity.BeginGet() )
		//			LinearZAxisMotorTargetVelocity = _linearZAxisMotorTargetVelocity.Get( this );
		//		return _linearZAxisMotorTargetVelocity.value;
		//	}
		//	set
		//	{
		//		if( _linearZAxisMotorTargetVelocity.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisMotorTargetVelocityChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisMotorTargetVelocity.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisMotorTargetVelocity"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisMotorTargetVelocityChanged;

		////LinearZAxisMotorMaxForce
		//ReferenceField<double> _linearZAxisMotorMaxForce = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		//[Category( "Linear Z Axis" )]
		//public Reference<double> LinearZAxisMotorMaxForce
		//{
		//	get
		//	{
		//		if( _linearZAxisMotorMaxForce.BeginGet() )
		//			LinearZAxisMotorMaxForce = _linearZAxisMotorMaxForce.Get( this );
		//		return _linearZAxisMotorMaxForce.value;
		//	}
		//	set
		//	{
		//		if( _linearZAxisMotorMaxForce.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisMotorMaxForceChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisMotorMaxForce.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisMotorMaxForce"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisMotorMaxForceChanged;

		////LinearZAxisMotorRestitution
		//ReferenceField<double> _linearZAxisMotorRestitution = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		//[Category( "Linear Z Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> LinearZAxisMotorRestitution
		//{
		//	get
		//	{
		//		if( _linearZAxisMotorRestitution.BeginGet() )
		//			LinearZAxisMotorRestitution = _linearZAxisMotorRestitution.Get( this );
		//		return _linearZAxisMotorRestitution.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _linearZAxisMotorRestitution.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisMotorRestitutionChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisMotorRestitution.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisMotorRestitution"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisMotorRestitutionChanged;

		////LinearZAxisMotorServo
		//ReferenceField<bool> _linearZAxisMotorServo = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Motor Servo" )]//[DisplayName( "Motor Servo" )]
		//[Category( "Linear Z Axis" )]
		//public Reference<bool> LinearZAxisMotorServo
		//{
		//	get
		//	{
		//		if( _linearZAxisMotorServo.BeginGet() )
		//			LinearZAxisMotorServo = _linearZAxisMotorServo.Get( this );
		//		return _linearZAxisMotorServo.value;
		//	}
		//	set
		//	{
		//		if( _linearZAxisMotorServo.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisMotorServoChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisMotorServo.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisMotorServo"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisMotorServoChanged;

		////LinearZAxisMotorServoTarget
		//ReferenceField<double> _linearZAxisMotorServoTarget = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		//[Category( "Linear Z Axis" )]
		//public Reference<double> LinearZAxisMotorServoTarget
		//{
		//	get
		//	{
		//		if( _linearZAxisMotorServoTarget.BeginGet() )
		//			LinearZAxisMotorServoTarget = _linearZAxisMotorServoTarget.Get( this );
		//		return _linearZAxisMotorServoTarget.value;
		//	}
		//	set
		//	{
		//		if( _linearZAxisMotorServoTarget.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisMotorServoTargetChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearMotors( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisMotorServoTarget.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisMotorServoTarget"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisMotorServoTargetChanged;

		////LinearZAxisSpring
		//ReferenceField<bool> _linearZAxisSpring = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Spring" )]//[DisplayName( "Spring" )]
		//[Category( "Linear Z Axis" )]
		//public Reference<bool> LinearZAxisSpring
		//{
		//	get
		//	{
		//		if( _linearZAxisSpring.BeginGet() )
		//			LinearZAxisSpring = _linearZAxisSpring.Get( this );
		//		return _linearZAxisSpring.value;
		//	}
		//	set
		//	{
		//		if( _linearZAxisSpring.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisSpringChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearSprings( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisSpring.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisSpring"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisSpringChanged;

		////LinearZAxisSpringStiffness
		//ReferenceField<double> _linearZAxisSpringStiffness = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		//[Category( "Linear Z Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> LinearZAxisSpringStiffness
		//{
		//	get
		//	{
		//		if( _linearZAxisSpringStiffness.BeginGet() )
		//			LinearZAxisSpringStiffness = _linearZAxisSpringStiffness.Get( this );
		//		return _linearZAxisSpringStiffness.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _linearZAxisSpringStiffness.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisSpringStiffnessChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearSprings( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisSpringStiffness.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisSpringStiffness"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisSpringStiffnessChanged;

		////LinearZAxisSpringDamping
		//ReferenceField<double> _linearZAxisSpringDamping = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Linear Z Axis Spring Damping" )]//[DisplayName( "Spring Damping" )]
		//[Category( "Linear Z Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> LinearZAxisSpringDamping
		//{
		//	get
		//	{
		//		if( _linearZAxisSpringDamping.BeginGet() )
		//			LinearZAxisSpringDamping = _linearZAxisSpringDamping.Get( this );
		//		return _linearZAxisSpringDamping.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _linearZAxisSpringDamping.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearZAxisSpringDampingChanged?.Invoke( this );
		//				//!!!!
		//				//if( InternalConstraintRigid != null )
		//				//	UpdateLinearSprings( InternalConstraintRigid );
		//			}
		//			finally { _linearZAxisSpringDamping.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearZAxisSpringDamping"/> property value changes.</summary>
		//public event Action<Constraint> LinearZAxisSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Angular X-axis constraint mode.
		/// </summary>
		[DefaultValue( PhysicsAxisMode.Locked )]
		[DisplayName( "Angular X Axis" )]
		[Category( "Angular X Axis" )]
		public Reference<PhysicsAxisMode> AngularXAxis
		{
			get { if( _angularXAxis.BeginGet() ) AngularXAxis = _angularXAxis.Get( this ); return _angularXAxis.value; }
			set
			{
				if( _angularXAxis.BeginSet( ref value ) )
				{
					try
					{
						AngularXAxisChanged?.Invoke( this );

						//!!!!impl dynamic update
					}
					finally { _angularXAxis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularXAxis"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularXAxisChanged;
		ReferenceField<PhysicsAxisMode> _angularXAxis = PhysicsAxisMode.Locked;

		/// <summary>
		/// The limits of the angular axis X.
		/// </summary>
		[DefaultValue( "-45 45" )]
		[DisplayName( "Angular X Axis Limit" )]
		[Category( "Angular X Axis" )]
		public Reference<Range> AngularXAxisLimit
		{
			get { if( _angularXAxisLimit.BeginGet() ) AngularXAxisLimit = _angularXAxisLimit.Get( this ); return _angularXAxisLimit.value; }
			set { if( _angularXAxisLimit.BeginSet( ref value ) ) { try { AngularXAxisLimitChanged?.Invoke( this ); UpdateLimit( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX ); } finally { _angularXAxisLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularXAxisLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularXAxisLimitChanged;
		ReferenceField<Range> _angularXAxisLimit = new Range( -45, 45 );

		/// <summary>
		/// The friction of angular axis X.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular X Axis Friction" )]
		[Category( "Angular X Axis" )]
		public Reference<double> AngularXAxisFriction
		{
			get { if( _angularXAxisFriction.BeginGet() ) AngularXAxisFriction = _angularXAxisFriction.Get( this ); return _angularXAxisFriction.value; }
			set { if( _angularXAxisFriction.BeginSet( ref value ) ) { try { AngularXAxisFrictionChanged?.Invoke( this ); UpdateFriction( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX ); } finally { _angularXAxisFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularXAxisFriction"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularXAxisFrictionChanged;
		ReferenceField<double> _angularXAxisFriction = 0.0;

		/// <summary>
		/// The motor mode of angular axis X.
		/// </summary>
		[DefaultValue( Scene.PhysicsWorldClass.MotorModeEnum.None )]
		[DisplayName( "Angular X Axis Motor" )]
		[Category( "Angular X Axis" )]
		public Reference<Scene.PhysicsWorldClass.MotorModeEnum> AngularXAxisMotor
		{
			get { if( _angularXAxisMotor.BeginGet() ) AngularXAxisMotor = _angularXAxisMotor.Get( this ); return _angularXAxisMotor.value; }
			set { if( _angularXAxisMotor.BeginSet( ref value ) ) { try { AngularXAxisMotorChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX ); } finally { _angularXAxisMotor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularXAxisMotor"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularXAxisMotorChanged;
		ReferenceField<Scene.PhysicsWorldClass.MotorModeEnum> _angularXAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.None;

		/// <summary>
		/// Oscillation frequency when solving position target (Hz). Should be in the range (0, 0.5 * simulation frequency]. When simulating at 60 Hz, 20 is a good value for a strong motor. Only used for position motors.
		/// </summary>
		[DefaultValue( 2.0 )]
		[DisplayName( "Angular X Axis Motor Frequency" )]
		[Category( "Angular X Axis" )]
		public Reference<double> AngularXAxisMotorFrequency
		{
			get { if( _angularXAxisMotorFrequency.BeginGet() ) AngularXAxisMotorFrequency = _angularXAxisMotorFrequency.Get( this ); return _angularXAxisMotorFrequency.value; }
			set { if( _angularXAxisMotorFrequency.BeginSet( ref value ) ) { try { AngularXAxisMotorFrequencyChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX ); } finally { _angularXAxisMotorFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularXAxisMotorFrequency"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularXAxisMotorFrequencyChanged;
		ReferenceField<double> _angularXAxisMotorFrequency = 2.0;

		/// <summary>
		/// Damping when solving position target (0 = minimal damping, 1 = critical damping). Only used for position motors.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "Angular X Axis Motor Damping" )]
		[Category( "Angular X Axis" )]
		public Reference<double> AngularXAxisMotorDamping
		{
			get { if( _angularXAxisMotorDamping.BeginGet() ) AngularXAxisMotorDamping = _angularXAxisMotorDamping.Get( this ); return _angularXAxisMotorDamping.value; }
			set { if( _angularXAxisMotorDamping.BeginSet( ref value ) ) { try { AngularXAxisMotorDampingChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX ); } finally { _angularXAxisMotorDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularXAxisMotorDamping"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularXAxisMotorDampingChanged;
		ReferenceField<double> _angularXAxisMotorDamping = 1.0;

		/// <summary>
		///Force range to apply in case of a angular constraint (N). Usually this is -mMaxForceLimit unless you want a motor that can e.g. push but not pull. Not used when motor is an angular motor.
		/// </summary>
		[DefaultValue( "-1000000 1000000" )]
		[DisplayName( "Angular X Axis Motor Limit" )]
		[Category( "Angular X Axis" )]
		public Reference<Range> AngularXAxisMotorLimit
		{
			get { if( _angularXAxisMotorLimit.BeginGet() ) AngularXAxisMotorLimit = _angularXAxisMotorLimit.Get( this ); return _angularXAxisMotorLimit.value; }
			set { if( _angularXAxisMotorLimit.BeginSet( ref value ) ) { try { AngularXAxisMotorLimitChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX ); } finally { _angularXAxisMotorLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularXAxisMotorLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularXAxisMotorLimitChanged;
		ReferenceField<Range> _angularXAxisMotorLimit = new Range( -1000000, 1000000 );

		/// <summary>
		/// The target value of the angular axis X motor. It is a velocity or a position depending the mode.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular X Axis Motor Target" )]
		[Category( "Angular X Axis" )]
		public Reference<double> AngularXAxisMotorTarget
		{
			get { if( _angularXAxisMotorTarget.BeginGet() ) AngularXAxisMotorTarget = _angularXAxisMotorTarget.Get( this ); return _angularXAxisMotorTarget.value; }
			set { if( _angularXAxisMotorTarget.BeginSet( ref value ) ) { try { AngularXAxisMotorTargetChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX ); } finally { _angularXAxisMotorTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularXAxisMotorTarget"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularXAxisMotorTargetChanged;
		ReferenceField<double> _angularXAxisMotorTarget = 0.0;


		////AngularXAxisMotor
		//ReferenceField<bool> _angularXAxisMotor;
		//[Serialize]
		//[DefaultValue( false )]
		//[DisplayName( "Angular X Axis Motor" )]//[DisplayName( "Motor" )]
		//[Category( "Angular X Axis" )]
		//public Reference<bool> AngularXAxisMotor
		//{
		//	get
		//	{
		//		if( _angularXAxisMotor.BeginGet() )
		//			AngularXAxisMotor = _angularXAxisMotor.Get( this );
		//		return _angularXAxisMotor.value;
		//	}
		//	set
		//	{
		//		if( _angularXAxisMotor.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularXAxisMotorChanged?.Invoke( this );
		//				zzzzz;
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
		//			}
		//			finally { _angularXAxisMotor.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularXAxisMotor"/> property value changes.</summary>
		//public event Action<Constraint> AngularXAxisMotorChanged;

		////AngularXAxisMotorTargetVelocity
		//ReferenceField<double> _angularXAxisMotorTargetVelocity = 0.0;
		//[Serialize]
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Angular X Axis Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		//[Category( "Angular X Axis" )]
		//public Reference<double> AngularXAxisMotorTargetVelocity
		//{
		//	get
		//	{
		//		if( _angularXAxisMotorTargetVelocity.BeginGet() )
		//			AngularXAxisMotorTargetVelocity = _angularXAxisMotorTargetVelocity.Get( this );
		//		return _angularXAxisMotorTargetVelocity.value;
		//	}
		//	set
		//	{
		//		if( _angularXAxisMotorTargetVelocity.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularXAxisMotorTargetVelocityChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
		//			}
		//			finally { _angularXAxisMotorTargetVelocity.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularXAxisMotorTargetVelocity"/> property value changes.</summary>
		//public event Action<Constraint> AngularXAxisMotorTargetVelocityChanged;

		////AngularXAxisMotorMaxForce
		//ReferenceField<double> _angularXAxisMotorMaxForce = 0.0;
		//[Serialize]
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Angular X Axis Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		//[Category( "Angular X Axis" )]
		//public Reference<double> AngularXAxisMotorMaxForce
		//{
		//	get
		//	{
		//		if( _angularXAxisMotorMaxForce.BeginGet() )
		//			AngularXAxisMotorMaxForce = _angularXAxisMotorMaxForce.Get( this );
		//		return _angularXAxisMotorMaxForce.value;
		//	}
		//	set
		//	{
		//		if( _angularXAxisMotorMaxForce.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularXAxisMotorMaxForceChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
		//			}
		//			finally { _angularXAxisMotorMaxForce.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularXAxisMotorMaxForce"/> property value changes.</summary>
		//public event Action<Constraint> AngularXAxisMotorMaxForceChanged;

		////!!!!

		////AngularXAxisMotorRestitution
		//ReferenceField<double> _angularXAxisMotorRestitution = 0.0;
		//[Serialize]
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Angular X Axis Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		//[Category( "Angular X Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> AngularXAxisMotorRestitution
		//{
		//	get
		//	{
		//		if( _angularXAxisMotorRestitution.BeginGet() )
		//			AngularXAxisMotorRestitution = _angularXAxisMotorRestitution.Get( this );
		//		return _angularXAxisMotorRestitution.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _angularXAxisMotorRestitution.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularXAxisMotorRestitutionChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
		//			}
		//			finally { _angularXAxisMotorRestitution.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularXAxisMotorRestitution"/> property value changes.</summary>
		//public event Action<Constraint> AngularXAxisMotorRestitutionChanged;

		////AngularXAxisServo
		//ReferenceField<bool> _angularXAxisServo = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Angular X Axis Motor Servo" )]//[DisplayName( "Motor Servo" )]
		//[Category( "Angular X Axis" )]
		//public Reference<bool> AngularXAxisServo
		//{
		//	get
		//	{
		//		if( _angularXAxisServo.BeginGet() )
		//			AngularXAxisServo = _angularXAxisServo.Get( this );
		//		return _angularXAxisServo.value;
		//	}
		//	set
		//	{
		//		if( _angularXAxisServo.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularXAxisServoChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
		//			}
		//			finally { _angularXAxisServo.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularXAxisServo"/> property value changes.</summary>
		//public event Action<Constraint> AngularXAxisServoChanged;

		////AngularXAxisServoTarget
		//ReferenceField<Degree> _angularXAxisServoTarget = new Degree( 0 );
		//[DefaultValue( "0" )]
		//[Range( -180.0, 180.0 )]
		//[Serialize]
		//[DisplayName( "Angular X Axis Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		//[Category( "Angular X Axis" )]
		//public Reference<Degree> AngularXAxisServoTarget
		//{
		//	get
		//	{
		//		if( _angularXAxisServoTarget.BeginGet() )
		//			AngularXAxisServoTarget = _angularXAxisServoTarget.Get( this );
		//		return _angularXAxisServoTarget.value;
		//	}
		//	set
		//	{
		//		if( _angularXAxisServoTarget.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularXAxisServoTargetChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
		//			}
		//			finally { _angularXAxisServoTarget.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularXAxisServoTarget"/> property value changes.</summary>
		//public event Action<Constraint> AngularXAxisServoTargetChanged;

		////AngularXAxisSpring
		//ReferenceField<bool> _angularXAxisSpring = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Angular X Axis Spring" )]//[DisplayName( "Spring" )]
		//[Category( "Angular X Axis" )]
		//public Reference<bool> AngularXAxisSpring
		//{
		//	get
		//	{
		//		if( _angularXAxisSpring.BeginGet() )
		//			AngularXAxisSpring = _angularXAxisSpring.Get( this );
		//		return _angularXAxisSpring.value;
		//	}
		//	set
		//	{
		//		if( _angularXAxisSpring.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularXAxisSpringChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
		//			}
		//			finally { _angularXAxisSpring.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularXAxisSpring"/> property value changes.</summary>
		//public event Action<Constraint> AngularXAxisSpringChanged;

		////AngularXAxisSpringStiffness
		//ReferenceField<double> _angularXAxisSpringStiffness = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Angular X Axis Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		//[Category( "Angular X Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> AngularXAxisSpringStiffness
		//{
		//	get
		//	{
		//		if( _angularXAxisSpringStiffness.BeginGet() )
		//			AngularXAxisSpringStiffness = _angularXAxisSpringStiffness.Get( this );
		//		return _angularXAxisSpringStiffness.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _angularXAxisSpringStiffness.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularXAxisSpringStiffnessChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
		//			}
		//			finally { _angularXAxisSpringStiffness.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularXAxisSpringStiffness"/> property value changes.</summary>
		//public event Action<Constraint> AngularXAxisSpringStiffnessChanged;

		////AngularXAxisSpringDamping
		//ReferenceField<double> _angularXAxisSpringDamping = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Angular X Axis Spring Damping" )]//[DisplayName( "Spring Damping" )]
		//[Category( "Angular X Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> AngularXAxisSpringDamping
		//{
		//	get
		//	{
		//		if( _angularXAxisSpringDamping.BeginGet() )
		//			AngularXAxisSpringDamping = _angularXAxisSpringDamping.Get( this );
		//		return _angularXAxisSpringDamping.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _angularXAxisSpringDamping.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularXAxisSpringDampingChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX );
		//			}
		//			finally { _angularXAxisSpringDamping.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularXAxisSpringDamping"/> property value changes.</summary>
		//public event Action<Constraint> AngularXAxisSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Angular Y-axis constraint mode.
		/// </summary>
		[DefaultValue( PhysicsAxisMode.Locked )]
		[DisplayName( "Angular Y Axis" )]
		[Category( "Angular Y Axis" )]
		public Reference<PhysicsAxisMode> AngularYAxis
		{
			get { if( _angularYAxis.BeginGet() ) AngularYAxis = _angularYAxis.Get( this ); return _angularYAxis.value; }
			set
			{
				if( _angularYAxis.BeginSet( ref value ) )
				{
					try
					{
						AngularYAxisChanged?.Invoke( this );

						//!!!!impl dynamic update
					}
					finally { _angularYAxis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularYAxis"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularYAxisChanged;
		ReferenceField<PhysicsAxisMode> _angularYAxis = PhysicsAxisMode.Locked;

		/// <summary>
		/// The limits of the angular axis Y.
		/// </summary>
		[DefaultValue( "-45 45" )]
		[DisplayName( "Angular Y Axis Limit" )]
		[Category( "Angular Y Axis" )]
		public Reference<Range> AngularYAxisLimit
		{
			get { if( _angularYAxisLimit.BeginGet() ) AngularYAxisLimit = _angularYAxisLimit.Get( this ); return _angularYAxisLimit.value; }
			set { if( _angularYAxisLimit.BeginSet( ref value ) ) { try { AngularYAxisLimitChanged?.Invoke( this ); UpdateLimit( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY ); } finally { _angularYAxisLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularYAxisLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularYAxisLimitChanged;
		ReferenceField<Range> _angularYAxisLimit = new Range( -45, 45 );

		/// <summary>
		/// The friction of angular axis Y.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Y Axis Friction" )]
		[Category( "Angular Y Axis" )]
		public Reference<double> AngularYAxisFriction
		{
			get { if( _angularYAxisFriction.BeginGet() ) AngularYAxisFriction = _angularYAxisFriction.Get( this ); return _angularYAxisFriction.value; }
			set { if( _angularYAxisFriction.BeginSet( ref value ) ) { try { AngularYAxisFrictionChanged?.Invoke( this ); UpdateFriction( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY ); } finally { _angularYAxisFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularYAxisFriction"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularYAxisFrictionChanged;
		ReferenceField<double> _angularYAxisFriction = 0.0;

		/// <summary>
		/// The motor mode of angular axis Y.
		/// </summary>
		[DefaultValue( Scene.PhysicsWorldClass.MotorModeEnum.None )]
		[DisplayName( "Angular Y Axis Motor" )]
		[Category( "Angular Y Axis" )]
		public Reference<Scene.PhysicsWorldClass.MotorModeEnum> AngularYAxisMotor
		{
			get { if( _angularYAxisMotor.BeginGet() ) AngularYAxisMotor = _angularYAxisMotor.Get( this ); return _angularYAxisMotor.value; }
			set { if( _angularYAxisMotor.BeginSet( ref value ) ) { try { AngularYAxisMotorChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY ); } finally { _angularYAxisMotor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularYAxisMotor"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularYAxisMotorChanged;
		ReferenceField<Scene.PhysicsWorldClass.MotorModeEnum> _angularYAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.None;

		/// <summary>
		/// Oscillation frequency when solving position target (Hz). Should be in the range (0, 0.5 * simulation frequency]. When simulating at 60 Hz, 20 is a good value for a strong motor. Only used for position motors.
		/// </summary>
		[DefaultValue( 2.0 )]
		[DisplayName( "Angular Y Axis Motor Frequency" )]
		[Category( "Angular Y Axis" )]
		public Reference<double> AngularYAxisMotorFrequency
		{
			get { if( _angularYAxisMotorFrequency.BeginGet() ) AngularYAxisMotorFrequency = _angularYAxisMotorFrequency.Get( this ); return _angularYAxisMotorFrequency.value; }
			set { if( _angularYAxisMotorFrequency.BeginSet( ref value ) ) { try { AngularYAxisMotorFrequencyChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY ); } finally { _angularYAxisMotorFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularYAxisMotorFrequency"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularYAxisMotorFrequencyChanged;
		ReferenceField<double> _angularYAxisMotorFrequency = 2.0;

		/// <summary>
		/// Damping when solving position target (0 = minimal damping, 1 = critical damping). Only used for position motors.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "Angular Y Axis Motor Damping" )]
		[Category( "Angular Y Axis" )]
		public Reference<double> AngularYAxisMotorDamping
		{
			get { if( _angularYAxisMotorDamping.BeginGet() ) AngularYAxisMotorDamping = _angularYAxisMotorDamping.Get( this ); return _angularYAxisMotorDamping.value; }
			set { if( _angularYAxisMotorDamping.BeginSet( ref value ) ) { try { AngularYAxisMotorDampingChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY ); } finally { _angularYAxisMotorDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularYAxisMotorDamping"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularYAxisMotorDampingChanged;
		ReferenceField<double> _angularYAxisMotorDamping = 1.0;

		/// <summary>
		///Force range to apply in case of a angular constraint (N). Usually this is -mMaxForceLimit unless you want a motor that can e.g. push but not pull. Not used when motor is an angular motor.
		/// </summary>
		[DefaultValue( "-1000000 1000000" )]
		[DisplayName( "Angular Y Axis Motor Limit" )]
		[Category( "Angular Y Axis" )]
		public Reference<Range> AngularYAxisMotorLimit
		{
			get { if( _angularYAxisMotorLimit.BeginGet() ) AngularYAxisMotorLimit = _angularYAxisMotorLimit.Get( this ); return _angularYAxisMotorLimit.value; }
			set { if( _angularYAxisMotorLimit.BeginSet( ref value ) ) { try { AngularYAxisMotorLimitChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY ); } finally { _angularYAxisMotorLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularYAxisMotorLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularYAxisMotorLimitChanged;
		ReferenceField<Range> _angularYAxisMotorLimit = new Range( -1000000, 1000000 );

		/// <summary>
		/// The target value of the angular axis Y motor. It is a velocity or a position depending the mode.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Y Axis Motor Target" )]
		[Category( "Angular Y Axis" )]
		public Reference<double> AngularYAxisMotorTarget
		{
			get { if( _angularYAxisMotorTarget.BeginGet() ) AngularYAxisMotorTarget = _angularYAxisMotorTarget.Get( this ); return _angularYAxisMotorTarget.value; }
			set { if( _angularYAxisMotorTarget.BeginSet( ref value ) ) { try { AngularYAxisMotorTargetChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY ); } finally { _angularYAxisMotorTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularYAxisMotorTarget"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularYAxisMotorTargetChanged;
		ReferenceField<double> _angularYAxisMotorTarget = 0.0;


		////AngularYAxis
		//ReferenceField<PhysicsAxisMode> _angularYAxis = PhysicsAxisMode.Locked;
		///// <summary>
		///// Angular Y-axis constraint mode.
		///// </summary>
		//[Serialize]
		//[DefaultValue( PhysicsAxisMode.Locked )]
		//[Category( "Angular Y Axis" )]
		//public Reference<PhysicsAxisMode> AngularYAxis
		//{
		//	get
		//	{
		//		if( _angularYAxis.BeginGet() )
		//			AngularYAxis = _angularYAxis.Get( this );
		//		return _angularYAxis.value;
		//	}
		//	set
		//	{
		//		if( _angularYAxis.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisChanged?.Invoke( this );
		//				//if( physicalConstraint != null )
		//				//	UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxis.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxis"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisChanged;

		////AngularYAxisLimitLow
		//ReferenceField<Degree> _angularYAxisLimitLow = new Degree( -45.0 );
		//[Serialize]
		//[DefaultValue( "-45" )]
		//[Range( -90.0, 90.0 )]
		//[DisplayName( "Angular Y Axis Limit Low" )]//[DisplayName( "Limit Low" )]
		//[Category( "Angular Y Axis" )]
		//public Reference<Degree> AngularYAxisLimitLow
		//{
		//	get
		//	{
		//		if( _angularYAxisLimitLow.BeginGet() )
		//			AngularYAxisLimitLow = _angularYAxisLimitLow.Get( this );
		//		return _angularYAxisLimitLow.value;
		//	}
		//	set
		//	{
		//		if( _angularYAxisLimitLow.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisLimitLowChanged?.Invoke( this );
		//				//if( physicalConstraint != null )
		//				//	UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisLimitLow.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisLimitLow"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisLimitLowChanged;

		////AngularYAxisLimitHigh
		//ReferenceField<Degree> _angularYAxisLimitHigh = new Degree( 45.0 );
		//[Serialize]
		//[DefaultValue( "45" )]
		//[Range( -90.0, 90.0 )]
		//[DisplayName( "Angular Y Axis Limit High" )]//[DisplayName( "Limit High" )]
		//[Category( "Angular Y Axis" )]
		//public Reference<Degree> AngularYAxisLimitHigh
		//{
		//	get
		//	{
		//		if( _angularYAxisLimitHigh.BeginGet() )
		//			AngularYAxisLimitHigh = _angularYAxisLimitHigh.Get( this );
		//		return _angularYAxisLimitHigh.value;
		//	}
		//	set
		//	{
		//		if( _angularYAxisLimitHigh.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisLimitHighChanged?.Invoke( this );
		//				//if( physicalConstraint != null )
		//				//	UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisLimitHigh.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisLimitHigh"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisLimitHighChanged;

		////AngularYAxisMotor
		//ReferenceField<bool> _angularYAxisMotor;
		//[Serialize]
		//[DefaultValue( false )]
		//[DisplayName( "Angular Y Axis Motor" )]//[DisplayName( "Motor" )]
		//[Category( "Angular Y Axis" )]
		//public Reference<bool> AngularYAxisMotor
		//{
		//	get
		//	{
		//		if( _angularYAxisMotor.BeginGet() )
		//			AngularYAxisMotor = _angularYAxisMotor.Get( this );
		//		return _angularYAxisMotor.value;
		//	}
		//	set
		//	{
		//		if( _angularYAxisMotor.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisMotorChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisMotor.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisMotor"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisMotorChanged;

		////AngularYAxisMotorTargetVelocity
		//ReferenceField<double> _angularYAxisMotorTargetVelocity = 0.0;
		//[Serialize]
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Angular Y Axis Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		//[Category( "Angular Y Axis" )]
		//public Reference<double> AngularYAxisMotorTargetVelocity
		//{
		//	get
		//	{
		//		if( _angularYAxisMotorTargetVelocity.BeginGet() )
		//			AngularYAxisMotorTargetVelocity = _angularYAxisMotorTargetVelocity.Get( this );
		//		return _angularYAxisMotorTargetVelocity.value;
		//	}
		//	set
		//	{
		//		if( _angularYAxisMotorTargetVelocity.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisMotorTargetVelocityChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisMotorTargetVelocity.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisMotorTargetVelocity"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisMotorTargetVelocityChanged;

		////AngularYAxisMotorMaxForce
		//ReferenceField<double> _angularYAxisMotorMaxForce = 0.0;
		//[Serialize]
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Angular Y Axis Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		//[Category( "Angular Y Axis" )]
		//public Reference<double> AngularYAxisMotorMaxForce
		//{
		//	get
		//	{
		//		if( _angularYAxisMotorMaxForce.BeginGet() )
		//			AngularYAxisMotorMaxForce = _angularYAxisMotorMaxForce.Get( this );
		//		return _angularYAxisMotorMaxForce.value;
		//	}
		//	set
		//	{
		//		if( _angularYAxisMotorMaxForce.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisMotorMaxForceChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisMotorMaxForce.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisMotorMaxForce"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisMotorMaxForceChanged;

		////AngularYAxisMotorRestitution
		//ReferenceField<double> _angularYAxisMotorRestitution = 0.0;
		//[Serialize]
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Angular Y Axis Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		//[Category( "Angular Y Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> AngularYAxisMotorRestitution
		//{
		//	get
		//	{
		//		if( _angularYAxisMotorRestitution.BeginGet() )
		//			AngularYAxisMotorRestitution = _angularYAxisMotorRestitution.Get( this );
		//		return _angularYAxisMotorRestitution.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _angularYAxisMotorRestitution.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisMotorRestitutionChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisMotorRestitution.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisMotorRestitution"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisMotorRestitutionChanged;

		////AngularYAxisServo
		//ReferenceField<bool> _angularYAxisServo = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Angular Y Axis Motor Servo" )]//[DisplayName( "Motor Servo" )]
		//[Category( "Angular Y Axis" )]
		//public Reference<bool> AngularYAxisServo
		//{
		//	get
		//	{
		//		if( _angularYAxisServo.BeginGet() )
		//			AngularYAxisServo = _angularYAxisServo.Get( this );
		//		return _angularYAxisServo.value;
		//	}
		//	set
		//	{
		//		if( _angularYAxisServo.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisServoChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisServo.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisServo"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisServoChanged;

		////AngularYAxisServoTarget
		//ReferenceField<Degree> _angularYAxisServoTarget = new Degree( 0 );
		//[DefaultValue( "0" )]
		//[Range( -180.0, 180.0 )]
		//[Serialize]
		//[DisplayName( "Angular Y Axis Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		//[Category( "Angular Y Axis" )]
		//public Reference<Degree> AngularYAxisServoTarget
		//{
		//	get
		//	{
		//		if( _angularYAxisServoTarget.BeginGet() )
		//			AngularYAxisServoTarget = _angularYAxisServoTarget.Get( this );
		//		return _angularYAxisServoTarget.value;
		//	}
		//	set
		//	{
		//		if( _angularYAxisServoTarget.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisServoTargetChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisServoTarget.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisServoTarget"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisServoTargetChanged;

		////AngularYAxisSpring
		//ReferenceField<bool> _angularYAxisSpring = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Angular Y Axis Spring" )]//[DisplayName( "Spring" )]
		//[Category( "Angular Y Axis" )]
		//public Reference<bool> AngularYAxisSpring
		//{
		//	get
		//	{
		//		if( _angularYAxisSpring.BeginGet() )
		//			AngularYAxisSpring = _angularYAxisSpring.Get( this );
		//		return _angularYAxisSpring.value;
		//	}
		//	set
		//	{
		//		if( _angularYAxisSpring.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisSpringChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisSpring.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisSpring"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisSpringChanged;

		////AngularYAxisSpringStiffness
		//ReferenceField<double> _angularYAxisSpringStiffness = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Angular Y Axis Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		//[Category( "Angular Y Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> AngularYAxisSpringStiffness
		//{
		//	get
		//	{
		//		if( _angularYAxisSpringStiffness.BeginGet() )
		//			AngularYAxisSpringStiffness = _angularYAxisSpringStiffness.Get( this );
		//		return _angularYAxisSpringStiffness.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _angularYAxisSpringStiffness.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisSpringStiffnessChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisSpringStiffness.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisSpringStiffness"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisSpringStiffnessChanged;

		////AngularYAxisSpringDamping
		//ReferenceField<double> _angularYAxisSpringDamping = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Angular Y Axis Spring Damping" )]//[DisplayName( "Spring Damping" )]
		//[Category( "Angular Y Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> AngularYAxisSpringDamping
		//{
		//	get
		//	{
		//		if( _angularYAxisSpringDamping.BeginGet() )
		//			AngularYAxisSpringDamping = _angularYAxisSpringDamping.Get( this );
		//		return _angularYAxisSpringDamping.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _angularYAxisSpringDamping.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularYAxisSpringDampingChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY );
		//			}
		//			finally { _angularYAxisSpringDamping.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularYAxisSpringDamping"/> property value changes.</summary>
		//public event Action<Constraint> AngularYAxisSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Angular Z-axis constraint mode.
		/// </summary>
		[DefaultValue( PhysicsAxisMode.Locked )]
		[DisplayName( "Angular Z Axis" )]
		[Category( "Angular Z Axis" )]
		public Reference<PhysicsAxisMode> AngularZAxis
		{
			get { if( _angularZAxis.BeginGet() ) AngularZAxis = _angularZAxis.Get( this ); return _angularZAxis.value; }
			set
			{
				if( _angularZAxis.BeginSet( ref value ) )
				{
					try
					{
						AngularZAxisChanged?.Invoke( this );

						//!!!!impl dynamic update
					}
					finally { _angularZAxis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularZAxis"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularZAxisChanged;
		ReferenceField<PhysicsAxisMode> _angularZAxis = PhysicsAxisMode.Locked;

		/// <summary>
		/// The limits of the angular axis Z.
		/// </summary>
		[DefaultValue( "-45 45" )]
		[DisplayName( "Angular Z Axis Limit" )]
		[Category( "Angular Z Axis" )]
		public Reference<Range> AngularZAxisLimit
		{
			get { if( _angularZAxisLimit.BeginGet() ) AngularZAxisLimit = _angularZAxisLimit.Get( this ); return _angularZAxisLimit.value; }
			set { if( _angularZAxisLimit.BeginSet( ref value ) ) { try { AngularZAxisLimitChanged?.Invoke( this ); UpdateLimit( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ ); } finally { _angularZAxisLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularZAxisLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularZAxisLimitChanged;
		ReferenceField<Range> _angularZAxisLimit = new Range( -45, 45 );

		/// <summary>
		/// The friction of angular axis Z.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Z Axis Friction" )]
		[Category( "Angular Z Axis" )]
		public Reference<double> AngularZAxisFriction
		{
			get { if( _angularZAxisFriction.BeginGet() ) AngularZAxisFriction = _angularZAxisFriction.Get( this ); return _angularZAxisFriction.value; }
			set { if( _angularZAxisFriction.BeginSet( ref value ) ) { try { AngularZAxisFrictionChanged?.Invoke( this ); UpdateFriction( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ ); } finally { _angularZAxisFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularZAxisFriction"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularZAxisFrictionChanged;
		ReferenceField<double> _angularZAxisFriction = 0.0;

		/// <summary>
		/// The motor mode of angular axis Z.
		/// </summary>
		[DefaultValue( Scene.PhysicsWorldClass.MotorModeEnum.None )]
		[DisplayName( "Angular Z Axis Motor" )]
		[Category( "Angular Z Axis" )]
		public Reference<Scene.PhysicsWorldClass.MotorModeEnum> AngularZAxisMotor
		{
			get { if( _angularZAxisMotor.BeginGet() ) AngularZAxisMotor = _angularZAxisMotor.Get( this ); return _angularZAxisMotor.value; }
			set { if( _angularZAxisMotor.BeginSet( ref value ) ) { try { AngularZAxisMotorChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ ); } finally { _angularZAxisMotor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularZAxisMotor"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularZAxisMotorChanged;
		ReferenceField<Scene.PhysicsWorldClass.MotorModeEnum> _angularZAxisMotor = Scene.PhysicsWorldClass.MotorModeEnum.None;

		/// <summary>
		/// Oscillation frequency when solving position target (Hz). Should be in the range (0, 0.5 * simulation frequency]. When simulating at 60 Hz, 20 is a good value for a strong motor. Only used for position motors.
		/// </summary>
		[DefaultValue( 2.0 )]
		[DisplayName( "Angular Z Axis Motor Frequency" )]
		[Category( "Angular Z Axis" )]
		public Reference<double> AngularZAxisMotorFrequency
		{
			get { if( _angularZAxisMotorFrequency.BeginGet() ) AngularZAxisMotorFrequency = _angularZAxisMotorFrequency.Get( this ); return _angularZAxisMotorFrequency.value; }
			set { if( _angularZAxisMotorFrequency.BeginSet( ref value ) ) { try { AngularZAxisMotorFrequencyChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ ); } finally { _angularZAxisMotorFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularZAxisMotorFrequency"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularZAxisMotorFrequencyChanged;
		ReferenceField<double> _angularZAxisMotorFrequency = 2.0;

		/// <summary>
		/// Damping when solving position target (0 = minimal damping, 1 = critical damping). Only used for position motors.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "Angular Z Axis Motor Damping" )]
		[Category( "Angular Z Axis" )]
		public Reference<double> AngularZAxisMotorDamping
		{
			get { if( _angularZAxisMotorDamping.BeginGet() ) AngularZAxisMotorDamping = _angularZAxisMotorDamping.Get( this ); return _angularZAxisMotorDamping.value; }
			set { if( _angularZAxisMotorDamping.BeginSet( ref value ) ) { try { AngularZAxisMotorDampingChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ ); } finally { _angularZAxisMotorDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularZAxisMotorDamping"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularZAxisMotorDampingChanged;
		ReferenceField<double> _angularZAxisMotorDamping = 1.0;

		/// <summary>
		///Force range to apply in case of a angular constraint (N). Usually this is -mMaxForceLimit unless you want a motor that can e.g. push but not pull. Not used when motor is an angular motor.
		/// </summary>
		[DefaultValue( "-1000000 1000000" )]
		[DisplayName( "Angular Z Axis Motor Limit" )]
		[Category( "Angular Z Axis" )]
		public Reference<Range> AngularZAxisMotorLimit
		{
			get { if( _angularZAxisMotorLimit.BeginGet() ) AngularZAxisMotorLimit = _angularZAxisMotorLimit.Get( this ); return _angularZAxisMotorLimit.value; }
			set { if( _angularZAxisMotorLimit.BeginSet( ref value ) ) { try { AngularZAxisMotorLimitChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ ); } finally { _angularZAxisMotorLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularZAxisMotorLimit"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularZAxisMotorLimitChanged;
		ReferenceField<Range> _angularZAxisMotorLimit = new Range( -1000000, 1000000 );

		/// <summary>
		/// The target value of the angular axis Z motor. It is a velocity or a position depending the mode.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Z Axis Motor Target" )]
		[Category( "Angular Z Axis" )]
		public Reference<double> AngularZAxisMotorTarget
		{
			get { if( _angularZAxisMotorTarget.BeginGet() ) AngularZAxisMotorTarget = _angularZAxisMotorTarget.Get( this ); return _angularZAxisMotorTarget.value; }
			set { if( _angularZAxisMotorTarget.BeginSet( ref value ) ) { try { AngularZAxisMotorTargetChanged?.Invoke( this ); NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ ); } finally { _angularZAxisMotorTarget.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularZAxisMotorTarget"/> property value changes.</summary>
		public event Action<Constraint_SixDOF> AngularZAxisMotorTargetChanged;
		ReferenceField<double> _angularZAxisMotorTarget = 0.0;


		////AngularZAxis
		//ReferenceField<PhysicsAxisMode> _angularZAxis = PhysicsAxisMode.Locked;
		///// <summary>
		///// Angular Z-axis constraint mode.
		///// </summary>
		//[Serialize]
		//[DefaultValue( PhysicsAxisMode.Locked )]
		//[Category( "Angular Z Axis" )]
		//public Reference<PhysicsAxisMode> AngularZAxis
		//{
		//	get
		//	{
		//		if( _angularZAxis.BeginGet() )
		//			AngularZAxis = _angularZAxis.Get( this );
		//		return _angularZAxis.value;
		//	}
		//	set
		//	{
		//		if( _angularZAxis.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisChanged?.Invoke( this );
		//				//!!!!?
		//				//if( physicalConstraint != null )
		//				//	UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxis.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxis"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisChanged;

		////AngularZAxisLimitLow
		//ReferenceField<Degree> _angularZAxisLimitLow = new Degree( -45 );
		//[Serialize]
		//[DefaultValue( "-45" )]
		//[Range( -180, 180 )]
		//[DisplayName( "Angular Z Axis Limit Low" )]//[DisplayName( "Limit Low" )]
		//[Category( "Angular Z Axis" )]
		//public Reference<Degree> AngularZAxisLimitLow
		//{
		//	get
		//	{
		//		if( _angularZAxisLimitLow.BeginGet() )
		//			AngularZAxisLimitLow = _angularZAxisLimitLow.Get( this );
		//		return _angularZAxisLimitLow.value;
		//	}
		//	set
		//	{
		//		if( _angularZAxisLimitLow.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisLimitLowChanged?.Invoke( this );
		//				//if( physicalConstraint != null )
		//				//	UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisLimitLow.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisLimitLow"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisLimitLowChanged;

		////AngularZAxisLimitHigh
		//ReferenceField<Degree> _angularZAxisLimitHigh = new Degree( 45 );
		//[Serialize]
		//[DefaultValue( "45" )]
		//[Range( -180, 180 )]
		//[DisplayName( "Angular Z Axis Limit High" )]//[DisplayName( "Limit High" )]
		//[Category( "Angular Z Axis" )]
		//public Reference<Degree> AngularZAxisLimitHigh
		//{
		//	get
		//	{
		//		if( _angularZAxisLimitHigh.BeginGet() )
		//			AngularZAxisLimitHigh = _angularZAxisLimitHigh.Get( this );
		//		return _angularZAxisLimitHigh.value;
		//	}
		//	set
		//	{
		//		if( _angularZAxisLimitHigh.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisLimitHighChanged?.Invoke( this );
		//				//if( physicalConstraint != null )
		//				//	UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisLimitHigh.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisLimitHigh"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisLimitHighChanged;

		////AngularZAxisMotor
		//ReferenceField<bool> _angularZAxisMotor;
		//[Serialize]
		//[DefaultValue( false )]
		//[DisplayName( "Angular Z Axis Motor" )]//[DisplayName( "Motor" )]
		//[Category( "Angular Z Axis" )]
		//public Reference<bool> AngularZAxisMotor
		//{
		//	get
		//	{
		//		if( _angularZAxisMotor.BeginGet() )
		//			AngularZAxisMotor = _angularZAxisMotor.Get( this );
		//		return _angularZAxisMotor.value;
		//	}
		//	set
		//	{
		//		if( _angularZAxisMotor.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisMotorChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisMotor.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisMotor"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisMotorChanged;

		////AngularZAxisMotorTargetVelocity
		//ReferenceField<double> _angularZAxisMotorTargetVelocity = 0.0;
		//[Serialize]
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Angular Z Axis Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		//[Category( "Angular Z Axis" )]
		//public Reference<double> AngularZAxisMotorTargetVelocity
		//{
		//	get
		//	{
		//		if( _angularZAxisMotorTargetVelocity.BeginGet() )
		//			AngularZAxisMotorTargetVelocity = _angularZAxisMotorTargetVelocity.Get( this );
		//		return _angularZAxisMotorTargetVelocity.value;
		//	}
		//	set
		//	{
		//		if( _angularZAxisMotorTargetVelocity.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisMotorTargetVelocityChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisMotorTargetVelocity.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisMotorTargetVelocity"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisMotorTargetVelocityChanged;

		////AngularZAxisMotorMaxForce
		//ReferenceField<double> _angularZAxisMotorMaxForce = 0.0;
		//[Serialize]
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Angular Z Axis Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		//[Category( "Angular Z Axis" )]
		//public Reference<double> AngularZAxisMotorMaxForce
		//{
		//	get
		//	{
		//		if( _angularZAxisMotorMaxForce.BeginGet() )
		//			AngularZAxisMotorMaxForce = _angularZAxisMotorMaxForce.Get( this );
		//		return _angularZAxisMotorMaxForce.value;
		//	}
		//	set
		//	{
		//		if( _angularZAxisMotorMaxForce.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisMotorMaxForceChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisMotorMaxForce.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisMotorMaxForce"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisMotorMaxForceChanged;

		////AngularZAxisMotorRestitution
		//ReferenceField<double> _angularZAxisMotorRestitution = 0.0;
		//[Serialize]
		//[DefaultValue( 0.0 )]
		//[DisplayName( "Angular Z Axis Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		//[Category( "Angular Z Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> AngularZAxisMotorRestitution
		//{
		//	get
		//	{
		//		if( _angularZAxisMotorRestitution.BeginGet() )
		//			AngularZAxisMotorRestitution = _angularZAxisMotorRestitution.Get( this );
		//		return _angularZAxisMotorRestitution.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _angularZAxisMotorRestitution.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisMotorRestitutionChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisMotorRestitution.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisMotorRestitution"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisMotorRestitutionChanged;

		////AngularZAxisServo
		//ReferenceField<bool> _angularZAxisServo = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Angular Z Axis Motor Servo" )]//[DisplayName( "Motor Servo" )]
		//[Category( "Angular Z Axis" )]
		//public Reference<bool> AngularZAxisServo
		//{
		//	get
		//	{
		//		if( _angularZAxisServo.BeginGet() )
		//			AngularZAxisServo = _angularZAxisServo.Get( this );
		//		return _angularZAxisServo.value;
		//	}
		//	set
		//	{
		//		if( _angularZAxisServo.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisServoChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisServo.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisServo"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisServoChanged;

		////AngularZAxisServoTarget
		//ReferenceField<Degree> _angularZAxisServoTarget = new Degree( 0 );
		//[DefaultValue( "0" )]
		//[Range( -180.0, 180.0 )]
		//[Serialize]
		//[DisplayName( "Angular Z Axis Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		//[Category( "Angular Z Axis" )]
		//public Reference<Degree> AngularZAxisServoTarget
		//{
		//	get
		//	{
		//		if( _angularZAxisServoTarget.BeginGet() )
		//			AngularZAxisServoTarget = _angularZAxisServoTarget.Get( this );
		//		return _angularZAxisServoTarget.value;
		//	}
		//	set
		//	{
		//		if( _angularZAxisServoTarget.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisServoTargetChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisServoTarget.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisServoTarget"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisServoTargetChanged;

		////AngularZAxisSpring
		//ReferenceField<bool> _angularZAxisSpring = false;
		//[DefaultValue( false )]
		//[Serialize]
		//[DisplayName( "Angular Z Axis Spring" )]//[DisplayName( "Spring" )]
		//[Category( "Angular Z Axis" )]
		//public Reference<bool> AngularZAxisSpring
		//{
		//	get
		//	{
		//		if( _angularZAxisSpring.BeginGet() )
		//			AngularZAxisSpring = _angularZAxisSpring.Get( this );
		//		return _angularZAxisSpring.value;
		//	}
		//	set
		//	{
		//		if( _angularZAxisSpring.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisSpringChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisSpring.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisSpring"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisSpringChanged;

		////AngularZAxisSpringStiffness
		//ReferenceField<double> _angularZAxisSpringStiffness = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Angular Z Axis Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		//[Category( "Angular Z Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> AngularZAxisSpringStiffness
		//{
		//	get
		//	{
		//		if( _angularZAxisSpringStiffness.BeginGet() )
		//			AngularZAxisSpringStiffness = _angularZAxisSpringStiffness.Get( this );
		//		return _angularZAxisSpringStiffness.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _angularZAxisSpringStiffness.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisSpringStiffnessChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisSpringStiffness.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisSpringStiffness"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisSpringStiffnessChanged;

		////AngularZAxisSpringDamping
		//ReferenceField<double> _angularZAxisSpringDamping = 0.0;
		//[DefaultValue( 0.0 )]
		//[Serialize]
		//[DisplayName( "Angular Z Axis Spring Damping" )]//[DisplayName( "Spring Damping" )]
		//[Category( "Angular Z Axis" )]
		//[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> AngularZAxisSpringDamping
		//{
		//	get
		//	{
		//		if( _angularZAxisSpringDamping.BeginGet() )
		//			AngularZAxisSpringDamping = _angularZAxisSpringDamping.Get( this );
		//		return _angularZAxisSpringDamping.value;
		//	}
		//	set
		//	{
		//		value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
		//		if( _angularZAxisSpringDamping.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularZAxisSpringDampingChanged?.Invoke( this );
		//				if( physicalConstraint != null )
		//					UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ );
		//			}
		//			finally { _angularZAxisSpringDamping.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularZAxisSpringDamping"/> property value changes.</summary>
		//public event Action<Constraint> AngularZAxisSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{

				//!!!!

				//switch( member.Name )
				//{
				////case nameof( CollisionsBetweenLinkedBodies ):
				////	skip = BodyB.Value == null;
				////	break;
				//case nameof( BreakingImpulseThreshold ):
				//case nameof( OverrideNumberSolverIterations ):
				//	if( SoftBodyMode )
				//		skip = true;
				//	break;
				//}

				LinearMembersFilter( context, member.Name, ref skip );
				AngularMembersFilter( context, member.Name, ref skip );
			}
		}

		void LinearMembersFilter( Metadata.GetMembersContext context, string memberName, ref bool skip )
		{
			var sbMode = false;
			//var sbMode = SoftBodyMode;

			switch( memberName )
			{
			//LinearXAxis
			case nameof( LinearXAxisLimit ):
				if( LinearXAxis.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;
			case nameof( LinearXAxisFriction ):
			case nameof( LinearXAxisMotor ):
				if( LinearXAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( LinearXAxisMotorFrequency ):
			case nameof( LinearXAxisMotorDamping ):
			case nameof( LinearXAxisMotorLimit ):
			case nameof( LinearXAxisMotorTarget ):
				if( LinearXAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				if( LinearXAxisMotor.Value == Scene.PhysicsWorldClass.MotorModeEnum.None )
					skip = true;
				break;

			//case nameof( LinearXAxisMotorServo ):
			//case nameof( LinearXAxisMotorTargetVelocity ):
			//case nameof( LinearXAxisMotorMaxForce ):
			//case nameof( LinearXAxisMotorRestitution ):
			//	if( LinearXAxis.Value == PhysicsAxisMode.Locked || !LinearXAxisMotor || sbMode )
			//		skip = true;
			//	break;
			//case nameof( LinearXAxisMotorServoTarget ):
			//	if( LinearXAxis.Value == PhysicsAxisMode.Locked || !LinearXAxisMotor || !LinearXAxisMotorServo || sbMode )
			//		skip = true;
			//	break;

			//case nameof( LinearXAxisSpring ):
			//	if( LinearXAxis.Value == PhysicsAxisMode.Locked || sbMode )
			//		skip = true;
			//	break;
			//case nameof( LinearXAxisSpringStiffness ):
			//case nameof( LinearXAxisSpringDamping ):
			//	if( LinearXAxis.Value == PhysicsAxisMode.Locked || !LinearXAxisSpring || sbMode )
			//		skip = true;
			//	break;

			//LinearYAxis
			case nameof( LinearYAxisLimit ):
				if( LinearYAxis.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;
			case nameof( LinearYAxisFriction ):
			case nameof( LinearYAxisMotor ):
				if( LinearYAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( LinearYAxisMotorFrequency ):
			case nameof( LinearYAxisMotorDamping ):
			case nameof( LinearYAxisMotorLimit ):
			case nameof( LinearYAxisMotorTarget ):
				if( LinearYAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				if( LinearYAxisMotor.Value == Scene.PhysicsWorldClass.MotorModeEnum.None )
					skip = true;
				break;

			//LinearZAxis
			case nameof( LinearZAxisLimit ):
				if( LinearZAxis.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;
			case nameof( LinearZAxisFriction ):
			case nameof( LinearZAxisMotor ):
				if( LinearZAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( LinearZAxisMotorFrequency ):
			case nameof( LinearZAxisMotorDamping ):
			case nameof( LinearZAxisMotorLimit ):
			case nameof( LinearZAxisMotorTarget ):
				if( LinearZAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				if( LinearZAxisMotor.Value == Scene.PhysicsWorldClass.MotorModeEnum.None )
					skip = true;
				break;
			}
		}

		void AngularMembersFilter( Metadata.GetMembersContext context, string memberName, ref bool skip )
		{
			var sbMode = false;
			//var sbMode = SoftBodyMode;

			switch( memberName )
			{
			//AngularXAxis
			case nameof( AngularXAxisLimit ):
				if( AngularXAxis.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;

			case nameof( AngularXAxisFriction ):
			case nameof( AngularXAxisMotor ):
				if( AngularXAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;

			case nameof( AngularXAxisMotorFrequency ):
			case nameof( AngularXAxisMotorDamping ):
				if( AngularXAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				if( AngularXAxisMotor.Value != Scene.PhysicsWorldClass.MotorModeEnum.Position )
					skip = true;
				break;

			case nameof( AngularXAxisMotorLimit ):
			case nameof( AngularXAxisMotorTarget ):
				if( AngularXAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				if( AngularXAxisMotor.Value == Scene.PhysicsWorldClass.MotorModeEnum.None )
					skip = true;
				break;

			//case nameof( AngularXAxisLimitLow ):
			//case nameof( AngularXAxisLimitHigh ):
			//	if( AngularXAxis.Value != PhysicsAxisMode.Limited )
			//		skip = true;
			//	break;
			//case nameof( AngularXAxisMotor ):
			//	if( AngularXAxis.Value == PhysicsAxisMode.Locked || sbMode )
			//		skip = true;
			//	break;
			//case nameof( AngularXAxisServo ):
			//case nameof( AngularXAxisMotorTargetVelocity ):
			//case nameof( AngularXAxisMotorMaxForce ):
			//case nameof( AngularXAxisMotorRestitution ):
			//	if( AngularXAxis.Value == PhysicsAxisMode.Locked || !AngularXAxisMotor || sbMode )
			//		skip = true;
			//	break;
			//case nameof( AngularXAxisServoTarget ):
			//	if( AngularXAxis.Value == PhysicsAxisMode.Locked || !AngularXAxisMotor || !AngularXAxisServo || sbMode )
			//		skip = true;
			//	break;
			//case nameof( AngularXAxisSpring ):
			//	if( AngularXAxis.Value == PhysicsAxisMode.Locked || sbMode )
			//		skip = true;
			//	break;
			//case nameof( AngularXAxisSpringStiffness ):
			//case nameof( AngularXAxisSpringDamping ):
			//	if( AngularXAxis.Value == PhysicsAxisMode.Locked || !AngularXAxisSpring || sbMode )
			//		skip = true;
			//	break;

			//AngularYAxis
			case nameof( AngularYAxisLimit ):
				if( AngularYAxis.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;

			case nameof( AngularYAxisFriction ):
			case nameof( AngularYAxisMotor ):
				if( AngularYAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;

			case nameof( AngularYAxisMotorFrequency ):
			case nameof( AngularYAxisMotorDamping ):
				if( AngularYAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				if( AngularYAxisMotor.Value != Scene.PhysicsWorldClass.MotorModeEnum.Position )
					skip = true;
				break;

			case nameof( AngularYAxisMotorLimit ):
			case nameof( AngularYAxisMotorTarget ):
				if( AngularYAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				if( AngularYAxisMotor.Value == Scene.PhysicsWorldClass.MotorModeEnum.None )
					skip = true;
				break;

			//case nameof( AngularYAxisLimitLow ):
			//case nameof( AngularYAxisLimitHigh ):
			//	if( AngularYAxis.Value != PhysicsAxisMode.Limited )
			//		skip = true;
			//	break;
			//case nameof( AngularYAxisMotor ):
			//	if( AngularYAxis.Value == PhysicsAxisMode.Locked || sbMode )
			//		skip = true;
			//	break;
			//case nameof( AngularYAxisServo ):
			//case nameof( AngularYAxisMotorTargetVelocity ):
			//case nameof( AngularYAxisMotorMaxForce ):
			//case nameof( AngularYAxisMotorRestitution ):
			//	if( AngularYAxis.Value == PhysicsAxisMode.Locked || !AngularYAxisMotor || sbMode )
			//		skip = true;
			//	break;
			//case nameof( AngularYAxisServoTarget ):
			//	if( AngularYAxis.Value == PhysicsAxisMode.Locked || !AngularYAxisMotor || !AngularYAxisServo || sbMode )
			//		skip = true;
			//	break;
			//case nameof( AngularYAxisSpring ):
			//	if( AngularYAxis.Value == PhysicsAxisMode.Locked || sbMode )
			//		skip = true;
			//	break;
			//case nameof( AngularYAxisSpringStiffness ):
			//case nameof( AngularYAxisSpringDamping ):
			//	if( AngularYAxis.Value == PhysicsAxisMode.Locked || !AngularYAxisSpring || sbMode )
			//		skip = true;
			//	break;

			//AngularZAxis
			case nameof( AngularZAxisLimit ):
				if( AngularZAxis.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;

			case nameof( AngularZAxisFriction ):
			case nameof( AngularZAxisMotor ):
				if( AngularZAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;

			case nameof( AngularZAxisMotorFrequency ):
			case nameof( AngularZAxisMotorDamping ):
				if( AngularZAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				if( AngularZAxisMotor.Value != Scene.PhysicsWorldClass.MotorModeEnum.Position )
					skip = true;
				break;

			case nameof( AngularZAxisMotorLimit ):
			case nameof( AngularZAxisMotorTarget ):
				if( AngularZAxis.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				if( AngularZAxisMotor.Value == Scene.PhysicsWorldClass.MotorModeEnum.None )
					skip = true;
				break;

				//case nameof( AngularZAxisLimitLow ):
				//case nameof( AngularZAxisLimitHigh ):
				//	if( AngularZAxis.Value != PhysicsAxisMode.Limited )
				//		skip = true;
				//	break;
				//case nameof( AngularZAxisMotor ):
				//	if( AngularZAxis.Value == PhysicsAxisMode.Locked || sbMode )
				//		skip = true;
				//	break;
				//case nameof( AngularZAxisServo ):
				//case nameof( AngularZAxisMotorTargetVelocity ):
				//case nameof( AngularZAxisMotorMaxForce ):
				//case nameof( AngularZAxisMotorRestitution ):
				//	if( AngularZAxis.Value == PhysicsAxisMode.Locked || !AngularZAxisMotor || sbMode )
				//		skip = true;
				//	break;
				//case nameof( AngularZAxisServoTarget ):
				//	if( AngularZAxis.Value == PhysicsAxisMode.Locked || !AngularZAxisMotor || !AngularZAxisServo || sbMode )
				//		skip = true;
				//	break;
				//case nameof( AngularZAxisSpring ):
				//	if( AngularZAxis.Value == PhysicsAxisMode.Locked || sbMode )
				//		skip = true;
				//	break;
				//case nameof( AngularZAxisSpringStiffness ):
				//case nameof( AngularZAxisSpringDamping ):
				//	if( AngularZAxis.Value == PhysicsAxisMode.Locked || !AngularZAxisSpring || sbMode )
				//		skip = true;
				//	break;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if !DEPLOY
		/// <summary>
		/// A set of settings for <see cref="Constraint_SixDOF"/> creation in the editor.
		/// </summary>
		public class NewObjectSettingsConstraint : NewObjectSettings
		{
			/// <summary>
			/// Enumerates the types of constraint link between two physical bodies.
			/// </summary>
			public enum InitialConfigurationEnum
			{
				Fixed,
				Hinge,
				Point,
				ConeTwist,
				Slider,
			}

			InitialConfigurationEnum initialConfiguration = InitialConfigurationEnum.Fixed;

			[DefaultValue( InitialConfigurationEnum.Fixed )]
			[Category( "Options" )]
			public InitialConfigurationEnum InitialConfiguration
			{
				get { return initialConfiguration; }
				set { initialConfiguration = value; }
			}

			public override bool Creation( NewObjectCell.ObjectCreationContext context )
			{
				var c = (Constraint_SixDOF)context.newObject;

				//!!!!Constraint: создание: если выделены тела, то констрейнт становится чилдом
				////get bodies from selected objects
				//{
				//	//!!!!пока так. надо брать из документа
				//	var selectedObjects = SettingsWindow.Instance._SelectedObjects;
				//	if( selectedObjects.Length == 2 )
				//	{
				//		var bodyA = selectedObjects[ 0 ] as RigidBody;
				//		var bodyB = selectedObjects[ 1 ] as RigidBody;
				//		if( bodyA != null && bodyB != null )
				//		{
				//			c.BodyA = new Reference<RigidBody>( null, ReferenceUtils.CalculateThisReference( c, bodyA ) );
				//			c.BodyB = new Reference<RigidBody>( null, ReferenceUtils.CalculateThisReference( c, bodyB ) );

				//			//!!!!можно луч от курсора учитывать
				//			var pos = ( bodyA.Transform.Value.Position + bodyB.Transform.Value.Position ) * 0.5;
				//			c.Transform = new Transform( pos, Quat.Identity );
				//		}
				//	}
				//}

				switch( InitialConfiguration )
				{
				case InitialConfigurationEnum.Hinge:
					c.AngularZAxis = PhysicsAxisMode.Free;
					break;

				case InitialConfigurationEnum.Point:
					c.AngularXAxis = PhysicsAxisMode.Free;
					c.AngularYAxis = PhysicsAxisMode.Free;
					c.AngularZAxis = PhysicsAxisMode.Free;
					break;

				case InitialConfigurationEnum.ConeTwist:
					//!!!!так?
					c.AngularXAxis = PhysicsAxisMode.Free;
					c.AngularYAxis = PhysicsAxisMode.Limited;
					c.AngularZAxis = PhysicsAxisMode.Limited;
					break;

				case InitialConfigurationEnum.Slider:
					c.LinearXAxis = PhysicsAxisMode.Free;
					break;
				}

				return base.Creation( context );
			}
		}
#endif

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Browsable( false )]
		public Scene.PhysicsWorldClass.Constraint PhysicalConstraint
		{
			get { return physicalConstraint; }
		}

		internal void UpdateDataFromPhysicalConstraint()
		{
			//!!!!slowly?

			if( physicalConstraint != null && physicalConstraint.BodyA != null )
			{
				var bodyA = physicalConstraint.BodyA;
				var bodyATransform = new Matrix4( bodyA.Rotation.ToMatrix3(), bodyA.Position );

				Matrix4.Multiply( ref bodyATransform, ref physicalConstraint.constraintAFrame, out var transformA );
				//var transformA = BulletPhysicsUtility.Convert( ConstraintRigid.RigidBodyA.WorldTransform ) * constraintAFrame;
				//var transformA = BulletUtils.Convert( constraint.RigidBodyA.CenterOfMassTransform ) * BulletUtils.Convert( constraint.AFrame );

				transformA.Decompose( out Vector3 translation, out Matrix3 rotation, out Vector3 scale );
				var oldT = Transform.Value;
				var newT = new Transform( translation, rotation.ToQuaternion(), oldT.Scale );

				SetTransformWithoutUpdatingConstraint( newT );


				//var oldTr = TransformV;
				//if( physicalConstraint.Position != oldTr.Position )
				//{
				//	var tr = new Transform( physicalConstraint.Position, oldTr.Rotation, oldTr.Scale );
				//	SetTransformWithoutUpdatingConstraint( tr );
				//}
			}
		}

		//public void UpdateDataFromPhysicsEngine()
		//{
		//	if( InternalConstraintRigid != null && InternalConstraintRigid.RigidBodyA.IsActive )
		//	{
		//		InternalConstraintRigid.RigidBodyA.GetWorldTransform( out var bodyATransform );
		//		BulletPhysicsUtility.Convert( ref bodyATransform, out var bodyATransform2 );
		//		Matrix4.Multiply( ref bodyATransform2, ref constraintAFrame, out var transformA );
		//		//var transformA = BulletPhysicsUtility.Convert( ConstraintRigid.RigidBodyA.WorldTransform ) * constraintAFrame;
		//		//var transformA = BulletUtils.Convert( constraint.RigidBodyA.CenterOfMassTransform ) * BulletUtils.Convert( constraint.AFrame );

		//		transformA.Decompose( out Vector3 translation, out Matrix3 rotation, out Vector3 scale );
		//		var oldT = Transform.Value;
		//		var newT = new Transform( translation, rotation.ToQuaternion(), oldT.Scale );

		//		SetTransformWithoutUpdatingConstraint( newT );
		//	}

		//	//soft body mode
		//	if( created )
		//	{
		//		if( SoftBodyMode )
		//		{
		//			GetSoftBodyModeBodies( out var body1, out var body2 );
		//			if( body1 != null )
		//			{
		//				if( softAnchorModeClosestNodeIndex != -1 )
		//				{
		//					var pos = body1.GetNodePosition( softAnchorModeClosestNodeIndex );

		//					var oldT = Transform.Value;
		//					var newT = new Transform( pos, Quaternion.Identity, oldT.Scale );
		//					SetTransformWithoutUpdatingConstraint( newT );
		//				}
		//			}
		//		}
		//		else
		//		{
		//		}
		//	}
		//}

		//void UpdateLinearLimits( Scene.PhysicsWorldClass.SixDOFConstraint c )
		//{
		//	Vector3 low = Vector3.Zero;
		//	Vector3 high = Vector3.Zero;

		//	//LinearXAxis
		//	if( LinearXAxis.Value == PhysicsAxisMode.Limited )
		//	{
		//		low.X = LinearXAxisLimitLow;
		//		high.X = LinearXAxisLimitHigh;
		//	}
		//	else if( LinearXAxis.Value == PhysicsAxisMode.Free )
		//	{
		//		low.X = 1;
		//		high.X = -1;
		//	}

		//	//LinearYAxis
		//	if( LinearYAxis.Value == PhysicsAxisMode.Limited )
		//	{
		//		low.Y = LinearYAxisLimitLow;
		//		high.Y = LinearYAxisLimitHigh;
		//	}
		//	else if( LinearYAxis.Value == PhysicsAxisMode.Free )
		//	{
		//		low.Y = 1;
		//		high.Y = -1;
		//	}

		//	//LinearZAxis
		//	if( LinearZAxis.Value == PhysicsAxisMode.Limited )
		//	{
		//		low.Z = LinearZAxisLimitLow;
		//		high.Z = LinearZAxisLimitHigh;
		//	}
		//	else if( LinearZAxis.Value == PhysicsAxisMode.Free )
		//	{
		//		low.Z = 1;
		//		high.Z = -1;
		//	}

		//	c.LinearLowerLimit = BulletPhysicsUtility.Convert( low );
		//	c.LinearUpperLimit = BulletPhysicsUtility.Convert( high );
		//}

		//void UpdateLinearLimits( Generic6DofSpring2Constraint c )
		//{
		//	Vector3 low = Vector3.Zero;
		//	Vector3 high = Vector3.Zero;

		//	//LinearXAxis
		//	if( LinearXAxis.Value == PhysicsAxisMode.Limited )
		//	{
		//		low.X = LinearXAxisLimitLow;
		//		high.X = LinearXAxisLimitHigh;
		//	}
		//	else if( LinearXAxis.Value == PhysicsAxisMode.Free )
		//	{
		//		low.X = 1;
		//		high.X = -1;
		//	}

		//	//LinearYAxis
		//	if( LinearYAxis.Value == PhysicsAxisMode.Limited )
		//	{
		//		low.Y = LinearYAxisLimitLow;
		//		high.Y = LinearYAxisLimitHigh;
		//	}
		//	else if( LinearYAxis.Value == PhysicsAxisMode.Free )
		//	{
		//		low.Y = 1;
		//		high.Y = -1;
		//	}

		//	//LinearZAxis
		//	if( LinearZAxis.Value == PhysicsAxisMode.Limited )
		//	{
		//		low.Z = LinearZAxisLimitLow;
		//		high.Z = LinearZAxisLimitHigh;
		//	}
		//	else if( LinearZAxis.Value == PhysicsAxisMode.Free )
		//	{
		//		low.Z = 1;
		//		high.Z = -1;
		//	}

		//	c.LinearLowerLimit = BulletPhysicsUtility.Convert( low );
		//	c.LinearUpperLimit = BulletPhysicsUtility.Convert( high );
		//}

		//void UpdateLinearMotors( Generic6DofSpring2Constraint c )
		//{
		//	c.EnableMotor( 0, LinearXAxis != PhysicsAxisMode.Locked && LinearXAxisMotor );
		//	c.EnableMotor( 1, LinearYAxis != PhysicsAxisMode.Locked && LinearYAxisMotor );
		//	c.EnableMotor( 2, LinearZAxis != PhysicsAxisMode.Locked && LinearZAxisMotor );

		//	c.TranslationalLimitMotor.MaxMotorForce = new Internal.BulletSharp.Math.BVector3( LinearXAxisMotorMaxForce, LinearYAxisMotorMaxForce, LinearZAxisMotorMaxForce );
		//	c.TranslationalLimitMotor.TargetVelocity = new Internal.BulletSharp.Math.BVector3( LinearXAxisMotorTargetVelocity, LinearYAxisMotorTargetVelocity, LinearZAxisMotorTargetVelocity );
		//	c.TranslationalLimitMotor.Bounce = new Internal.BulletSharp.Math.BVector3( LinearXAxisMotorRestitution, LinearYAxisMotorRestitution, LinearZAxisMotorRestitution );

		//	c.SetServo( 0, LinearXAxisMotorServo );
		//	c.SetServo( 1, LinearYAxisMotorServo );
		//	c.SetServo( 2, LinearZAxisMotorServo );
		//	c.SetServoTarget( 0, LinearXAxisMotorServoTarget );
		//	c.SetServoTarget( 1, LinearYAxisMotorServoTarget );
		//	c.SetServoTarget( 2, LinearZAxisMotorServoTarget );
		//}

		//void UpdateLinearSprings( Generic6DofSpring2Constraint c )
		//{
		//	c.EnableSpring( 0, LinearXAxis.Value != PhysicsAxisMode.Locked && LinearXAxisSpring );
		//	c.EnableSpring( 1, LinearYAxis.Value != PhysicsAxisMode.Locked && LinearYAxisSpring );
		//	c.EnableSpring( 2, LinearZAxis.Value != PhysicsAxisMode.Locked && LinearZAxisSpring );

		//	c.SetStiffness( 0, LinearXAxisSpringStiffness );
		//	c.SetStiffness( 1, LinearYAxisSpringStiffness );
		//	c.SetStiffness( 2, LinearZAxisSpringStiffness );

		//	c.SetDamping( 0, LinearXAxisSpringDamping );
		//	c.SetDamping( 1, LinearYAxisSpringDamping );
		//	c.SetDamping( 2, LinearZAxisSpringDamping );
		//}

		//void UpdateAngularLimits( Generic6DofSpring2Constraint c )
		//{
		//	Vector3 low = Vector3.Zero;
		//	Vector3 high = Vector3.Zero;

		//	//AngularXAxis
		//	if( AngularXAxis.Value == PhysicsAxisMode.Limited )
		//	{
		//		low.X = AngularXAxisLimitLow.Value.InRadians();
		//		high.X = AngularXAxisLimitHigh.Value.InRadians();
		//	}
		//	else if( AngularXAxis.Value == PhysicsAxisMode.Free )
		//	{
		//		low.X = 1;
		//		high.X = -1;
		//	}

		//	//AngularYAxis
		//	if( AngularYAxis.Value == PhysicsAxisMode.Limited )
		//	{
		//		low.Y = AngularYAxisLimitLow.Value.InRadians();
		//		high.Y = AngularYAxisLimitHigh.Value.InRadians();
		//	}
		//	else if( AngularYAxis.Value == PhysicsAxisMode.Free )
		//	{
		//		low.Y = 1;
		//		high.Y = -1;
		//	}

		//	//AngularZAxis
		//	if( AngularZAxis.Value == PhysicsAxisMode.Limited )
		//	{
		//		low.Z = AngularZAxisLimitLow.Value.InRadians();
		//		high.Z = AngularZAxisLimitHigh.Value.InRadians();
		//	}
		//	else if( AngularZAxis.Value == PhysicsAxisMode.Free )
		//	{
		//		low.Z = 1;
		//		high.Z = -1;
		//	}

		//	c.AngularLowerLimit = BulletPhysicsUtility.Convert( low );
		//	c.AngularUpperLimit = BulletPhysicsUtility.Convert( high );

		//	//!!!!пока нет поддержки
		//	//c.SetBounce( 0, 1 );
		//	//c.SetBounce( 1, 1 );
		//	//c.SetBounce( 2, 1 );
		//}

		void UpdateLimit( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum axis )
		{
			var sixDOF = physicalConstraint as Scene.PhysicsWorldClass.SixDOFConstraint;
			if( sixDOF != null )
			{
				switch( axis )
				{
				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX:
					sixDOF.SetLimit( axis, LinearXAxisLimit.Value.ToRangeF() );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY:
					sixDOF.SetLimit( axis, LinearYAxisLimit.Value.ToRangeF() );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ:
					sixDOF.SetLimit( axis, LinearZAxisLimit.Value.ToRangeF() );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX:
					sixDOF.SetLimit( axis, AngularXAxisLimit.Value.ToRangeF() );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY:
					sixDOF.SetLimit( axis, AngularYAxisLimit.Value.ToRangeF() );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ:
					sixDOF.SetLimit( axis, AngularZAxisLimit.Value.ToRangeF() );
					break;
				}
			}
		}

		void UpdateFriction( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum axis )
		{
			var sixDOF = physicalConstraint as Scene.PhysicsWorldClass.SixDOFConstraint;
			if( sixDOF != null )
			{
				switch( axis )
				{
				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX:
					sixDOF.SetFriction( axis, (float)( LinearXAxis.Value != PhysicsAxisMode.Locked ? LinearXAxisFriction : 0 ) );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY:
					sixDOF.SetFriction( axis, (float)( LinearYAxis.Value != PhysicsAxisMode.Locked ? LinearYAxisFriction : 0 ) );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ:
					sixDOF.SetFriction( axis, (float)( LinearZAxis.Value != PhysicsAxisMode.Locked ? LinearZAxisFriction : 0 ) );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX:
					sixDOF.SetFriction( axis, (float)( AngularXAxis.Value != PhysicsAxisMode.Locked ? AngularXAxisFriction : 0 ) );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY:
					sixDOF.SetFriction( axis, (float)( AngularYAxis.Value != PhysicsAxisMode.Locked ? AngularYAxisFriction : 0 ) );
					break;

				case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ:
					sixDOF.SetFriction( axis, (float)( AngularZAxis.Value != PhysicsAxisMode.Locked ? AngularZAxisFriction : 0 ) );
					break;
				}
			}
		}

		void UpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint sixDOF, Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum axis )
		{
			switch( axis )
			{
			case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationX:
				sixDOF.SetMotor( axis, (int)LinearXAxisMotor.Value, (float)LinearXAxisMotorFrequency, (float)LinearXAxisMotorDamping, LinearXAxisMotorLimit.Value.ToRangeF(), (float)LinearXAxisMotorTarget );
				break;

			case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationY:
				sixDOF.SetMotor( axis, (int)LinearYAxisMotor.Value, (float)LinearYAxisMotorFrequency, (float)LinearYAxisMotorDamping, LinearYAxisMotorLimit.Value.ToRangeF(), (float)LinearYAxisMotorTarget );
				break;

			case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.TranslationZ:
				sixDOF.SetMotor( axis, (int)LinearZAxisMotor.Value, (float)LinearZAxisMotorFrequency, (float)LinearZAxisMotorDamping, LinearZAxisMotorLimit.Value.ToRangeF(), (float)LinearZAxisMotorTarget );
				break;

			case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationX:
				sixDOF.SetMotor( axis, (int)AngularXAxisMotor.Value, (float)AngularXAxisMotorFrequency, (float)AngularXAxisMotorDamping, AngularXAxisMotorLimit.Value.ToRangeF(), (float)AngularXAxisMotorTarget );
				break;

			case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationY:
				sixDOF.SetMotor( axis, (int)AngularYAxisMotor.Value, (float)AngularYAxisMotorFrequency, (float)AngularYAxisMotorDamping, AngularYAxisMotorLimit.Value.ToRangeF(), (float)AngularYAxisMotorTarget );
				break;

			case Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum.RotationZ:
				sixDOF.SetMotor( axis, (int)AngularZAxisMotor.Value, (float)AngularZAxisMotorFrequency, (float)AngularZAxisMotorDamping, AngularZAxisMotorLimit.Value.ToRangeF(), (float)AngularZAxisMotorTarget );
				break;
			}
		}

		void NeedUpdateMotor( Scene.PhysicsWorldClass.SixDOFConstraint.AxisEnum axis )
		{
			needUpdateMotors |= 1 << (int)axis;
		}

		//void UpdateAngularMotors( Generic6DofSpring2Constraint c )
		//{
		//	c.EnableMotor( 3, AngularXAxis != PhysicsAxisMode.Locked && AngularXAxisMotor );
		//	c.EnableMotor( 4, AngularYAxis != PhysicsAxisMode.Locked && AngularYAxisMotor );
		//	c.EnableMotor( 5, AngularZAxis != PhysicsAxisMode.Locked && AngularZAxisMotor );

		//	c.GetRotationalLimitMotor( 0 ).Bounce = AngularXAxisMotorRestitution;
		//	c.GetRotationalLimitMotor( 0 ).TargetVelocity = AngularXAxisMotorTargetVelocity;
		//	c.GetRotationalLimitMotor( 0 ).MaxMotorForce = AngularXAxisMotorMaxForce;

		//	c.GetRotationalLimitMotor( 1 ).Bounce = AngularYAxisMotorRestitution;
		//	c.GetRotationalLimitMotor( 1 ).TargetVelocity = AngularYAxisMotorTargetVelocity;
		//	c.GetRotationalLimitMotor( 1 ).MaxMotorForce = AngularYAxisMotorMaxForce;

		//	c.GetRotationalLimitMotor( 2 ).Bounce = AngularZAxisMotorRestitution;
		//	c.GetRotationalLimitMotor( 2 ).TargetVelocity = AngularZAxisMotorTargetVelocity;
		//	c.GetRotationalLimitMotor( 2 ).MaxMotorForce = AngularZAxisMotorMaxForce;

		//	c.SetServo( 3, AngularXAxisServo );
		//	c.SetServo( 4, AngularYAxisServo );
		//	c.SetServo( 5, AngularZAxisServo );
		//	c.SetServoTarget( 3, AngularXAxisServoTarget.Value.InRadians() );
		//	c.SetServoTarget( 4, AngularYAxisServoTarget.Value.InRadians() );
		//	c.SetServoTarget( 5, AngularZAxisServoTarget.Value.InRadians() );
		//}

		//void UpdateAngularSprings( Generic6DofSpring2Constraint c )
		//{
		//	c.EnableSpring( 3, AngularXAxis.Value != PhysicsAxisMode.Locked && AngularXAxisSpring );
		//	c.EnableSpring( 4, AngularYAxis.Value != PhysicsAxisMode.Locked && AngularYAxisSpring );
		//	c.EnableSpring( 5, AngularZAxis.Value != PhysicsAxisMode.Locked && AngularZAxisSpring );

		//	c.SetStiffness( 3, AngularXAxisSpringStiffness );
		//	c.SetStiffness( 4, AngularYAxisSpringStiffness );
		//	c.SetStiffness( 5, AngularZAxisSpringStiffness );

		//	c.SetDamping( 3, AngularXAxisSpringDamping );
		//	c.SetDamping( 4, AngularYAxisSpringDamping );
		//	c.SetDamping( 5, AngularZAxisSpringDamping );
		//}

		//[Browsable( false )]
		//bool SoftBodyMode
		//{
		//	get { return creationBodyA?.BulletBody as Internal.BulletSharp.SoftBody.SoftBody != null || creationBodyB?.BulletBody as Internal.BulletSharp.SoftBody.SoftBody != null; }
		//}

		//bool AllLinear( PhysicsAxisMode mode )
		//{
		//	return LinearXAxis.Value == mode && LinearYAxis.Value == mode && LinearZAxis.Value == mode;
		//}

		//bool AllAngular( PhysicsAxisMode mode )
		//{
		//	return AngularXAxis.Value == mode && AngularYAxis.Value == mode && AngularZAxis.Value == mode;
		//}

		//[Browsable( false )]
		//bool SoftNodeAnchorMode
		//{
		//	get { return AllLinear( PhysicsAxisMode.Locked ) && AllAngular( PhysicsAxisMode.Locked ) && SoftBodyMode; }
		//}



		//////!!!!
		////void AppendLinearConstraint( SoftBody body1, PhysicalBody body2, Vec3 pos, double forceMixing = 1, double errorReduction = 1, double split = 1 )
		////{
		////	using( var specs = new LinearJoint.Specs() )
		////	{
		////		specs.Position = BulletUtils.ToVector3( pos );
		////		specs.ConstraintForceMixing = forceMixing;
		////		specs.ErrorReductionParameter = errorReduction;
		////		specs.Split = split;

		////		var b1 = (SoftBody)body1.BulletBody;
		////		if( body2 == null )
		////			b1.AppendLinearJoint( specs );
		////		else if( body2.BulletBody is SoftBody )
		////			b1.AppendLinearJoint( specs, (SoftBody)body2.BulletBody );
		////		else if( body2.BulletBody is RigidBody )
		////			b1.AppendLinearJoint( specs, new Body( body2.BulletBody ) );
		////	}
		////}

		//////!!!!
		////void AppendAngularConstraint( SoftBody body1, PhysicalBody body2, Vec3 axis, double forceMixing = 1, double errorReduction = 1, double split = 1 )
		////{
		////	using( var specs = new AngularJoint.Specs() )
		////	{
		////		specs.Axis = BulletUtils.ToVector3( axis );
		////		specs.ConstraintForceMixing = forceMixing;
		////		specs.ErrorReductionParameter = errorReduction;
		////		specs.Split = split;

		////		var b1 = (SoftBody)body1.BulletBody;
		////		if( body2 == null )
		////			b1.AppendAngularJoint( specs );
		////		else if( body2.BulletBody is SoftBody )
		////			b1.AppendAngularJoint( specs, (SoftBody)body2.BulletBody );
		////		else if( body2.BulletBody is RigidBody )
		////			b1.AppendAngularJoint( specs, new Body( body2.BulletBody ) );
		////	}
		////}

	}
}
