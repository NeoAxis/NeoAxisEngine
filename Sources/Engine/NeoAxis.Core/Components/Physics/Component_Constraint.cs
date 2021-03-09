// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Constraint link between two physical bodies.
	/// </summary>
	[EditorNewObjectSettings( typeof( NewObjectSettingsConstraint ) )]
	public class Component_Constraint : Component_ObjectInSpace, Component_IPhysicalObject
	{
		bool created;
		Component_PhysicalBody creationBodyA;
		Component_PhysicalBody creationBodyB;
		Generic6DofSpring2Constraint constraintRigid;
		Matrix4 constraintAFrame = Matrix4.Identity;
		int softAnchorModeClosestNodeIndex = -1;
		//int closestFaceIdx = -1;

		bool duringCreateDestroy;
		bool setTransformWithoutUpdatingConstraint;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The first physical body used.
		/// </summary>
		[Serialize]
		public Reference<Component_PhysicalBody> BodyA
		{
			get { if( _bodyA.BeginGet() ) BodyA = _bodyA.Get( this ); return _bodyA.value; }
			set
			{
				if( _bodyA.BeginSet( ref value ) )
				{
					try
					{
						BodyAChanged?.Invoke( this );
						RecreateConstraint();
					}
					finally { _bodyA.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BodyA"/> property value changes.</summary>
		public event Action<Component_Constraint> BodyAChanged;
		ReferenceField<Component_PhysicalBody> _bodyA;

		/// <summary>
		/// The second physical body used.
		/// </summary>
		[Serialize]
		public Reference<Component_PhysicalBody> BodyB
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
		public event Action<Component_Constraint> BodyBChanged;
		ReferenceField<Component_PhysicalBody> _bodyB;

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
						RecreateConstraint();
					}
					finally { _collisionsBetweenLinkedBodies.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionsBetweenLinkedBodies"/> property value changes.</summary>
		public event Action<Component_Constraint> CollisionsBetweenLinkedBodiesChanged;
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
		//public event Action<Component_Constraint> BreakableChanged;

		//!!!!после того как сломается, то как это обрабатывать? видимо нужно cериализовать сломанность
		/// <summary>
		/// The value indicates the maximum value of the applied impulse to the body, which may be before it is breaks.
		/// </summary>
		[DefaultValue( 1.0e+005 )]
		//!!!!Infinity
		//[ApplicableRange( 0, float.MaxValue, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential, 100 )]
		//[ApplicableRange( 0, float.MaxValue, ApplicableRangeAttribute.ConvenientDistributionEnum.Exponential, 6 )]//!!!!
		[Range( 0, 1.0e+005, RangeAttribute.ConvenientDistributionEnum.Exponential, 6 )]
		[Serialize]
		public Reference<double> BreakingImpulseThreshold
		{
			get { if( _breakingImpulseThreshold.BeginGet() ) BreakingImpulseThreshold = _breakingImpulseThreshold.Get( this ); return _breakingImpulseThreshold.value; }
			set
			{
				if( _breakingImpulseThreshold.BeginSet( ref value ) )
				{
					try
					{
						BreakingImpulseThresholdChanged?.Invoke( this );
						if( ConstraintRigid != null )
							ConstraintRigid.BreakingImpulseThreshold = value;
					}
					finally { _breakingImpulseThreshold.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BreakingImpulseThreshold"/> property value changes.</summary>
		public event Action<Component_Constraint> BreakingImpulseThresholdChanged;
		ReferenceField<double> _breakingImpulseThreshold = 1.0e+005;//!!!!

		/// <summary>
		/// Overrides solver processes number on every physics frame.
		/// </summary>
		[DefaultValue( -1 )]
		[Range( -1, 100 )]
		[Serialize]
		public Reference<int> OverrideNumberSolverIterations
		{
			get { if( _overrideNumberSolverIterations.BeginGet() ) OverrideNumberSolverIterations = _overrideNumberSolverIterations.Get( this ); return _overrideNumberSolverIterations.value; }
			set
			{
				if( _overrideNumberSolverIterations.BeginSet( ref value ) )
				{
					try
					{
						OverrideNumberSolverIterationsChanged?.Invoke( this );
						if( ConstraintRigid != null )
							ConstraintRigid.OverrideNumSolverIterations = value;
					}
					finally { _overrideNumberSolverIterations.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="OverrideNumberSolverIterations"/> property value changes.</summary>
		public event Action<Component_Constraint> OverrideNumberSolverIterationsChanged;
		ReferenceField<int> _overrideNumberSolverIterations = -1;

		/// <summary>
		/// Whether to activate sleeping bodies when any motor of the constraint is enabled.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> AutoActivateBodies
		{
			get { if( _autoActivateBodies.BeginGet() ) AutoActivateBodies = _autoActivateBodies.Get( this ); return _autoActivateBodies.value; }
			set { if( _autoActivateBodies.BeginSet( ref value ) ) { try { AutoActivateBodiesChanged?.Invoke( this ); } finally { _autoActivateBodies.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AutoActivateBodies"/> property value changes.</summary>
		public event Action<Component_Constraint> AutoActivateBodiesChanged;
		ReferenceField<bool> _autoActivateBodies = false;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public Component_Scene.PhysicsWorldDataClass GetPhysicsWorldData()
		{
			var scene = ParentScene;
			if( scene != null )
				return scene.PhysicsWorldData;
			return null;
		}

		void GetSoftBodyModeBodies( out Component_SoftBody body1, out Component_PhysicalBody body2 )
		{
			//body1 is always soft body
			if( creationBodyA != null && creationBodyA.BulletBody is SoftBody )
			{
				body1 = (Component_SoftBody)creationBodyA;
				body2 = creationBodyB;
			}
			else
			{
				body1 = (Component_SoftBody)creationBodyB;
				body2 = creationBodyA;
			}
		}

		void CreateConstraint()
		{
			if( constraintRigid != null )
				Log.Fatal( "Component_Constraint: CreateConstraint: creationConstraintRigid != null." );
			if( !EnabledInHierarchy )
				Log.Fatal( "Component_Constraint: CreateConstraint: !EnabledInHierarchy." );

			duringCreateDestroy = true;
			try
			{
				var ba = BodyA.Value;
				var bb = BodyB.Value;

				//check bodies are enabled
				if( ba != null && ba.BulletBody == null )
					return;
				if( bb != null && bb.BulletBody == null )
					return;
				//check no bodies
				if( ba == null && bb == null )
					return;

				created = true;
				creationBodyA = ba;
				creationBodyB = bb;

				if( SoftBodyMode )
				{
					GetSoftBodyModeBodies( out var body1, out var body2 );

					softAnchorModeClosestNodeIndex = body1.FindClosestNodeIndex( Transform.Value.Position );
					//closestFaceIdx = softComp.FindClosestFaceIndex( Transform.Value.Position );

					//real creation only in simulation
					if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
					{
						if( SoftNodeAnchorMode )// all linear and angular are locked
						{
							var b1 = (SoftBody)body1.BulletBody;
							if( body2 == null )
								b1.SetMass( softAnchorModeClosestNodeIndex, 0 );
							else
							{
								b1.AppendAnchor( softAnchorModeClosestNodeIndex, (RigidBody)body2.BulletBody, !CollisionsBetweenLinkedBodies.Value );

								//var vector = BulletUtils.Convert( Transform.Value.Position - body1.GetNodePosition( softAnchorModeClosestNodeIndex ) );
								//b1.AppendAnchor( softAnchorModeClosestNodeIndex, (RigidBody)body2.BulletBody, vector, !CollisionsBetweenLinkedBodies.Value );
							}
						}
						else
						{
							//!!!!

							Log.Warning( "Component_Constraint: CreateConstraint: Free, Limited axes are not supported for soft body. No implementation." );

							//var t = Transform.Value;

							//if( AllLinear( PhysicsAxisMode.Free ) )
							//	AppendLinearConstraint( body1, body2, t.Position );

							//if( LinearAxisX.Value == PhysicsAxisMode.Limited )
							//{
							//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( LinearAxisXLimitHigh.Value, 0, 0 ), .5, .5, 1 );
							//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( LinearAxisXLimitLow.Value, 0, 0 ), .5, .5, 1 );
							//}

							//if( LinearAxisY.Value == PhysicsAxisMode.Limited )
							//{
							//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( 0, LinearAxisYLimitHigh.Value, 0 ), .5, .5, 1 );
							//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( 0, LinearAxisYLimitLow.Value, 0 ), .5, .5, 1 );
							//}

							//if( LinearAxisZ.Value == PhysicsAxisMode.Limited )
							//{
							//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( 0, 0, LinearAxisZLimitHigh.Value ), .5, .5, 1 );
							//	AppendLinearConstraint( body1, body2, t.Position + new Vec3( 0, 0, LinearAxisZLimitLow.Value ), .5, .5, 1 );
							//}

							//if( AngularAxisX.Value == PhysicsAxisMode.Limited )
							//	AppendAngularConstraint( body1, body2, Vec3.XAxis, .5, .5, 1 );
							//if( AngularAxisY.Value == PhysicsAxisMode.Limited )
							//	AppendAngularConstraint( body1, body2, Vec3.YAxis, .5, .5, 1 );
							//if( AngularAxisZ.Value == PhysicsAxisMode.Limited )
							//	AppendAngularConstraint( body1, body2, Vec3.ZAxis, .5, .5, 1 );
						}
					}
				}
				else
				{
					//rigid bodies only

					//transform without Scale
					var transform = Transform.Value;
					var t = new Matrix4( transform.Rotation.ToMatrix3(), transform.Position );

					if( creationBodyA != null && creationBodyB != null )
					{
						//!!!!need?
						//bodyB.ActivationState = ActivationState.DisableDeactivation;

						var bodyATransformFull = creationBodyA.Transform.Value;
						var bodyATransform = new Matrix4( bodyATransformFull.Rotation.ToMatrix3(), bodyATransformFull.Position );
						var bodyBTransformFull = creationBodyB.Transform.Value;
						var bodyBTransform = new Matrix4( bodyBTransformFull.Rotation.ToMatrix3(), bodyBTransformFull.Position );
						constraintAFrame = bodyATransform.GetInverse() * t;
						Matrix matA = BulletPhysicsUtility.Convert( constraintAFrame );
						Matrix matB = BulletPhysicsUtility.Convert( bodyBTransform.GetInverse() * t );

						//constraintAFrame = bodyA.Transform.Value.ToMat4().GetInverse() * t;
						//Matrix matA = BulletUtils.Convert( constraintAFrame );
						//Matrix matB = BulletUtils.Convert( bodyB.Transform.Value.ToMat4().GetInverse() * t );

						constraintRigid = new Generic6DofSpring2Constraint( (RigidBody)creationBodyA.BulletBody, (RigidBody)creationBodyB.BulletBody, matA, matB );
					}
					else
					{
						var body = creationBodyA ?? creationBodyB;

						var bodyTransformFull = body.Transform.Value;
						var bodyTransform = new Matrix4( bodyTransformFull.Rotation.ToMatrix3(), bodyTransformFull.Position );
						constraintAFrame = bodyTransform.GetInverse() * t;

						var mat = BulletPhysicsUtility.Convert( constraintAFrame );
						constraintRigid = new Generic6DofSpring2Constraint( (RigidBody)body.BulletBody, mat );
					}

					if( constraintRigid != null )
					{
						UpdateLinearLimits( constraintRigid );
						UpdateLinearMotors( constraintRigid );
						UpdateLinearSprings( constraintRigid );

						UpdateAngularLimits( constraintRigid );
						UpdateAngularMotors( constraintRigid );
						UpdateAngularSprings( constraintRigid );

						constraintRigid.BreakingImpulseThreshold = BreakingImpulseThreshold;
						constraintRigid.OverrideNumSolverIterations = OverrideNumberSolverIterations;
						constraintRigid.Userobject = this;
						ParentScene.PhysicsWorldData.world.AddConstraint( constraintRigid, !CollisionsBetweenLinkedBodies );

						////add actions to automatic destroy before bodies deletion
						//bodyA.AddActionBeforeBodyDestroy( DestroyConstraint );
						//bodyB.AddActionBeforeBodyDestroy( DestroyConstraint );
					}

				}
			}
			finally
			{
				duringCreateDestroy = false;
			}
		}

		public void DestroyConstraint()
		{
			duringCreateDestroy = true;

			var physicsData = GetPhysicsWorldData();
			if( physicsData != null )
			{
				if( constraintRigid != null )
				{
					//!!!!что еще удалять?

					//OnBeforeDestroyConstraint();

					physicsData.world.RemoveConstraint( constraintRigid );

					//!!!!всё равно падает
					////!!!!new
					////remove from rigid bodies
					//var bodyA = constraintRigid.RigidBodyA;
					//var bodyB = constraintRigid.RigidBodyB;
					//bodyA?.RemoveConstraintRef( constraintRigid );
					//bodyB?.RemoveConstraintRef( constraintRigid );

					constraintRigid.Dispose();
				}

				//!!!!remove soft body joints
				//!!!!remove anchors
			}

			created = false;
			creationBodyA = null;
			creationBodyB = null;
			constraintRigid = null;
			softAnchorModeClosestNodeIndex = -1;

			duringCreateDestroy = false;
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

			//if( BodyA.Value is Component_SoftBody softComp )
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
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( created && AutoActivateBodies )
			{
				if( LinearAxisXMotorTargetVelocity != 0 && LinearAxisXMotorMaxForce != 0 ||
					LinearAxisYMotorTargetVelocity != 0 && LinearAxisYMotorMaxForce != 0 ||
					LinearAxisZMotorTargetVelocity != 0 && LinearAxisZMotorMaxForce != 0 ||
					AngularAxisXMotorTargetVelocity != 0 && AngularAxisXMotorMaxForce != 0 ||
					AngularAxisYMotorTargetVelocity != 0 && AngularAxisYMotorMaxForce != 0 ||
					AngularAxisZMotorTargetVelocity != 0 && AngularAxisZMotorMaxForce != 0 )
				{
					var bodyA = creationBodyA as Component_RigidBody;
					if( bodyA != null && bodyA.MotionType.Value == Component_RigidBody.MotionTypeEnum.Dynamic && !bodyA.Active )
						bodyA.Activate();

					var bodyB = creationBodyB as Component_RigidBody;
					if( bodyB != null && bodyB.MotionType.Value == Component_RigidBody.MotionTypeEnum.Dynamic && !bodyB.Active )
						bodyB.Activate();
				}
			}
		}

		public void Render( ViewportRenderingContext context, out int verticesRendered )
		{
			verticesRendered = 0;

			var context2 = context.objectInSpaceRenderingContext;

			if( SoftBodyMode )
			{
				GetSoftBodyModeBodies( out var body1, out var body2 );

				if( SoftNodeAnchorMode )
				{
					var renderer = context.Owner.Simple3DRenderer;

					var pos = body1.GetNodePosition( softAnchorModeClosestNodeIndex );
					var thickness = renderer.GetThicknessByPixelSize( pos, 5 );
					renderer.SetColor( new ColorValue( 0, 1, 0 ) );
					renderer.AddSphere( pos, thickness );
					verticesRendered += 32 * 3 * 8;

					if( body2 != null )
					{
						renderer.SetColor( new ColorValue( 0, 0, 0 ) );
						renderer.AddLine( pos, Transform.Value.Position );
						renderer.AddLine( Transform.Value.Position, body2.Transform.Value.Position );
						verticesRendered += 16;
					}
				}
				else
				{
					//!!!!

					var trans = Transform.Value;

					if( AllLinear( PhysicsAxisMode.Free ) )
					{
						context.Owner.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 0 ) );

						if( body2 == null )
						{
							context.Owner.Simple3DRenderer.AddLine( trans.Position, body1.Transform.Value.Position );
							verticesRendered += 8;
						}
						else
						{
							context.Owner.Simple3DRenderer.AddLine( trans.Position, body1.Transform.Value.Position );
							context.Owner.Simple3DRenderer.AddLine( trans.Position, body2.Transform.Value.Position );
							verticesRendered += 16;
						}
					}

					if( LinearAxisX.Value == PhysicsAxisMode.Limited )
					{
						var p1 = trans.Position + new Vector3( LinearAxisXLimitHigh.Value, 0, 0 );
						var p2 = trans.Position + new Vector3( LinearAxisXLimitLow.Value, 0, 0 );
						DrawLinearLimitedJoint( context2, p1, p2, body1, body2, ref verticesRendered );
					}
					if( LinearAxisY.Value == PhysicsAxisMode.Limited )
					{
						var p1 = trans.Position + new Vector3( 0, LinearAxisYLimitHigh.Value, 0 );
						var p2 = trans.Position + new Vector3( 0, LinearAxisYLimitLow.Value, 0 );
						DrawLinearLimitedJoint( context2, p1, p2, body1, body2, ref verticesRendered );
					}
					if( LinearAxisZ.Value == PhysicsAxisMode.Limited )
					{
						var p1 = trans.Position + new Vector3( 0, 0, LinearAxisZLimitHigh.Value );
						var p2 = trans.Position + new Vector3( 0, 0, LinearAxisZLimitLow.Value );
						DrawLinearLimitedJoint( context2, p1, p2, body1, body2, ref verticesRendered );
					}
				}
			}
			else
			{
				if( ConstraintRigid != null && !ParentScene.DisplayPhysicsInternal )
				{
					UpdateDebugDrawSize( context.Owner );

					var physicsData = GetPhysicsWorldData();
					physicsData.debugDraw.verticesRenderedCounter = 0;
					physicsData.world.DebugDrawConstraint( ConstraintRigid );
					verticesRendered += physicsData.debugDraw.verticesRenderedCounter;
					physicsData.debugDraw.verticesRenderedCounter = 0;
				}
			}

			//if( !Broken )
			//{
			//	Vec3 anchor = Anchor;
			//	Vec3 halfDir = Axis.Direction * .5f;
			//	debugGeometry.AddLine( anchor - halfDir, anchor + halfDir );
			//}
		}

		//public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
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

		//						if( LinearAxisX.Value == PhysicsAxisMode.Limited )
		//						{
		//							var p1 = trans.Position + new Vector3( LinearAxisXLimitHigh.Value, 0, 0 );
		//							var p2 = trans.Position + new Vector3( LinearAxisXLimitLow.Value, 0, 0 );
		//							DrawLinearLimitedJoint( context2, p1, p2, body1, body2 );
		//						}
		//						if( LinearAxisY.Value == PhysicsAxisMode.Limited )
		//						{
		//							var p1 = trans.Position + new Vector3( 0, LinearAxisYLimitHigh.Value, 0 );
		//							var p2 = trans.Position + new Vector3( 0, LinearAxisYLimitLow.Value, 0 );
		//							DrawLinearLimitedJoint( context2, p1, p2, body1, body2 );
		//						}
		//						if( LinearAxisZ.Value == PhysicsAxisMode.Limited )
		//						{
		//							var p1 = trans.Position + new Vector3( 0, 0, LinearAxisZLimitHigh.Value );
		//							var p2 = trans.Position + new Vector3( 0, 0, LinearAxisZLimitLow.Value );
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

		void DrawLinearLimitedJoint( RenderingContext context, Vector3 p1, Vector3 p2, Component_SoftBody body1, Component_PhysicalBody body2, ref int verticesRendered )
		{
			context.viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 0, 0 ) );

			context.viewport.Simple3DRenderer.AddLine( p1, body1.Transform.Value.Position );
			context.viewport.Simple3DRenderer.AddLine( p2, body1.Transform.Value.Position );
			verticesRendered += 16;

			if( body2 != null )
			{
				context.viewport.Simple3DRenderer.AddLine( p1, body2.Transform.Value.Position );
				context.viewport.Simple3DRenderer.AddLine( p2, body2.Transform.Value.Position );
				verticesRendered += 16;
			}
			else
			{
				var thickness = context.viewport.Simple3DRenderer.GetThicknessByPixelSize( p1, 5 );
				context.viewport.Simple3DRenderer.SetColor( new ColorValue( 0, 1, 0 ) );
				context.viewport.Simple3DRenderer.AddSphere( p1, thickness );
				verticesRendered += 32 * 3 * 8;
				context.viewport.Simple3DRenderer.AddSphere( p2, thickness );
				verticesRendered += 32 * 3 * 8;
			}
		}

		public void UpdateDebugDrawSize( Viewport viewport )
		{
			if( ConstraintRigid != null )
			{
				var t = Transform.Value;
				var position = t.Position;
				var scale = Math.Max( t.Scale.X, Math.Max( t.Scale.Y, t.Scale.Z ) );

				//!!!!в конфиг
				//!!!!может указывать в пикселях? или вертикальным размером?
				//!!!!графиком настраивать
				//!!!!может картинку рисовать, может разным цветом
				double maxSize = 80;
				double minSize = 20;
				double maxDistance = 100;

				double distance = ( position - viewport.CameraSettings.Position ).Length();
				if( distance < maxDistance )
				{
					var size = MathEx.Lerp( maxSize, minSize, distance / maxDistance );
					var size2 = viewport.Simple3DRenderer.GetThicknessByPixelSize( position, size );
					ConstraintRigid.DebugDrawSize = size2 * scale;
				}
				else
				{
					//!!!!check: внутри не рисуется если 0?
					ConstraintRigid.DebugDrawSize = 0;
				}

				////!!!!TransformToolConfig
				//var toolSize = viewport.DebugGeometry.GetThicknessByPixelSize( t.Position, TransformToolConfig.size );
				//var size = toolSize / 1.5;

				//Constraint.DebugDrawSize = size;
			}
		}

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

		//!!!!descriptions

		//LinearAxisX
		ReferenceField<PhysicsAxisMode> _linearAxisX = PhysicsAxisMode.Locked;
		/// <summary>
		/// Linear X-axis constraint mode.
		/// </summary>
		[Serialize]
		[DefaultValue( PhysicsAxisMode.Locked )]
		[Category( "Linear Axis X" )]
		public Reference<PhysicsAxisMode> LinearAxisX
		{
			get
			{
				if( _linearAxisX.BeginGet() )
					LinearAxisX = _linearAxisX.Get( this );
				return _linearAxisX.value;
			}
			set
			{
				if( _linearAxisX.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearLimits( ConstraintRigid );
					}
					finally { _linearAxisX.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisX"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXChanged;

		//LinearAxisXLimitLow
		ReferenceField<double> _linearAxisXLimitLow = -1.0;
		[Serialize]
		[DefaultValue( -1.0 )]
		[DisplayName( "Linear Axis X Limit Low" )]//[DisplayName( "Limit Low" )]
		[Category( "Linear Axis X" )]
		public Reference<double> LinearAxisXLimitLow
		{
			get
			{
				if( _linearAxisXLimitLow.BeginGet() )
					LinearAxisXLimitLow = _linearAxisXLimitLow.Get( this );
				return _linearAxisXLimitLow.value;
			}
			set
			{
				if( _linearAxisXLimitLow.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXLimitLowChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearLimits( ConstraintRigid );
					}
					finally { _linearAxisXLimitLow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXLimitLow"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXLimitLowChanged;

		//LinearAxisXLimitHigh
		ReferenceField<double> _linearAxisXLimitHigh = 1.0;
		[Serialize]
		[DefaultValue( 1.0 )]
		[DisplayName( "Linear Axis X Limit High" )]//[DisplayName( "Limit High" )]
		[Category( "Linear Axis X" )]
		public Reference<double> LinearAxisXLimitHigh
		{
			get
			{
				if( _linearAxisXLimitHigh.BeginGet() )
					LinearAxisXLimitHigh = _linearAxisXLimitHigh.Get( this );
				return _linearAxisXLimitHigh.value;
			}
			set
			{
				if( _linearAxisXLimitHigh.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXLimitHighChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearLimits( ConstraintRigid );
					}
					finally { _linearAxisXLimitHigh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXLimitHigh"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXLimitHighChanged;


		//LinearAxisXMotor
		ReferenceField<bool> _linearAxisXMotor = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Linear Axis X Motor" )]//[DisplayName( "Motor" )]
		[Category( "Linear Axis X" )]
		public Reference<bool> LinearAxisXMotor
		{
			get
			{
				if( _linearAxisXMotor.BeginGet() )
					LinearAxisXMotor = _linearAxisXMotor.Get( this );
				return _linearAxisXMotor.value;
			}
			set
			{
				if( _linearAxisXMotor.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXMotorChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisXMotor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXMotor"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXMotorChanged;

		//LinearAxisXMotorTargetVelocity
		ReferenceField<double> _linearAxisXMotorTargetVelocity = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis X Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		[Category( "Linear Axis X" )]
		public Reference<double> LinearAxisXMotorTargetVelocity
		{
			get
			{
				if( _linearAxisXMotorTargetVelocity.BeginGet() )
					LinearAxisXMotorTargetVelocity = _linearAxisXMotorTargetVelocity.Get( this );
				return _linearAxisXMotorTargetVelocity.value;
			}
			set
			{
				if( _linearAxisXMotorTargetVelocity.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXMotorTargetVelocityChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisXMotorTargetVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXMotorTargetVelocity"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXMotorTargetVelocityChanged;

		//LinearAxisXMotorMaxForce
		ReferenceField<double> _linearAxisXMotorMaxForce = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis X Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		[Category( "Linear Axis X" )]
		public Reference<double> LinearAxisXMotorMaxForce
		{
			get
			{
				if( _linearAxisXMotorMaxForce.BeginGet() )
					LinearAxisXMotorMaxForce = _linearAxisXMotorMaxForce.Get( this );
				return _linearAxisXMotorMaxForce.value;
			}
			set
			{
				if( _linearAxisXMotorMaxForce.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXMotorMaxForceChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisXMotorMaxForce.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXMotorMaxForce"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXMotorMaxForceChanged;

		//LinearAxisXMotorRestitution
		ReferenceField<double> _linearAxisXMotorRestitution = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis X Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		[Category( "Linear Axis X" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> LinearAxisXMotorRestitution
		{
			get
			{
				if( _linearAxisXMotorRestitution.BeginGet() )
					LinearAxisXMotorRestitution = _linearAxisXMotorRestitution.Get( this );
				return _linearAxisXMotorRestitution.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _linearAxisXMotorRestitution.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXMotorRestitutionChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisXMotorRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXMotorRestitution"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXMotorRestitutionChanged;

		//LinearAxisXMotorServo
		ReferenceField<bool> _linearAxisXMotorServo = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Linear Axis X Motor Servo" )]//[DisplayName( "Motor Servo" )]
		[Category( "Linear Axis X" )]
		public Reference<bool> LinearAxisXMotorServo
		{
			get
			{
				if( _linearAxisXMotorServo.BeginGet() )
					LinearAxisXMotorServo = _linearAxisXMotorServo.Get( this );
				return _linearAxisXMotorServo.value;
			}
			set
			{
				if( _linearAxisXMotorServo.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXMotorServoChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisXMotorServo.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXMotorServo"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXMotorServoChanged;



		//LinearAxisXMotorServoTarget
		ReferenceField<double> _linearAxisXMotorServoTarget = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis X Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		[Category( "Linear Axis X" )]
		public Reference<double> LinearAxisXMotorServoTarget
		{
			get
			{
				if( _linearAxisXMotorServoTarget.BeginGet() )
					LinearAxisXMotorServoTarget = _linearAxisXMotorServoTarget.Get( this );
				return _linearAxisXMotorServoTarget.value;
			}
			set
			{
				if( _linearAxisXMotorServoTarget.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXMotorServoTargetChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisXMotorServoTarget.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXMotorServoTarget"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXMotorServoTargetChanged;

		//LinearAxisXSpring
		ReferenceField<bool> _linearAxisXSpring = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Linear Axis X Spring" )]//[DisplayName( "Spring" )]
		[Category( "Linear Axis X" )]
		public Reference<bool> LinearAxisXSpring
		{
			get
			{
				if( _linearAxisXSpring.BeginGet() )
					LinearAxisXSpring = _linearAxisXSpring.Get( this );
				return _linearAxisXSpring.value;
			}
			set
			{
				if( _linearAxisXSpring.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXSpringChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearSprings( ConstraintRigid );
					}
					finally { _linearAxisXSpring.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXSpring"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXSpringChanged;

		//LinearAxisXSpringStiffness
		ReferenceField<double> _linearAxisXSpringStiffness = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis X Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		[Category( "Linear Axis X" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> LinearAxisXSpringStiffness
		{
			get
			{
				if( _linearAxisXSpringStiffness.BeginGet() )
					LinearAxisXSpringStiffness = _linearAxisXSpringStiffness.Get( this );
				return _linearAxisXSpringStiffness.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _linearAxisXSpringStiffness.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXSpringStiffnessChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearSprings( ConstraintRigid );
					}
					finally { _linearAxisXSpringStiffness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXSpringStiffness"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXSpringStiffnessChanged;

		//LinearAxisXSpringDamping
		ReferenceField<double> _linearAxisXSpringDamping = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis X Spring Damping" )]//[DisplayName( "Spring Damping" )]
		[Category( "Linear Axis X" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> LinearAxisXSpringDamping
		{
			get
			{
				if( _linearAxisXSpringDamping.BeginGet() )
					LinearAxisXSpringDamping = _linearAxisXSpringDamping.Get( this );
				return _linearAxisXSpringDamping.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _linearAxisXSpringDamping.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisXSpringDampingChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearSprings( ConstraintRigid );
					}
					finally { _linearAxisXSpringDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisXSpringDamping"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisXSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//LinearAxisY
		ReferenceField<PhysicsAxisMode> _linearAxisY = PhysicsAxisMode.Locked;
		/// <summary>
		/// Linear Y-axis constraint mode.
		/// </summary>
		[DefaultValue( PhysicsAxisMode.Locked )]
		[Serialize]
		[Category( "Linear Axis Y" )]
		public Reference<PhysicsAxisMode> LinearAxisY
		{
			get
			{
				if( _linearAxisY.BeginGet() )
					LinearAxisY = _linearAxisY.Get( this );
				return _linearAxisY.value;
			}
			set
			{
				if( _linearAxisY.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearLimits( ConstraintRigid );
					}
					finally { _linearAxisY.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisY"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYChanged;

		//LinearAxisYLimitLow
		ReferenceField<double> _linearAxisYLimitLow = -1.0;
		[DefaultValue( -1.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Y Limit Low" )]//[DisplayName( "Limit Low" )]
		[Category( "Linear Axis Y" )]
		public Reference<double> LinearAxisYLimitLow
		{
			get
			{
				if( _linearAxisYLimitLow.BeginGet() )
					LinearAxisYLimitLow = _linearAxisYLimitLow.Get( this );
				return _linearAxisYLimitLow.value;
			}
			set
			{
				if( _linearAxisYLimitLow.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYLimitLowChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearLimits( ConstraintRigid );
					}
					finally { _linearAxisYLimitLow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYLimitLow"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYLimitLowChanged;

		//LinearAxisYLimitHigh
		ReferenceField<double> _linearAxisYLimitHigh = 1.0;
		[DefaultValue( 1.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Y Limit High" )]//[DisplayName( "Limit High" )]
		[Category( "Linear Axis Y" )]
		public Reference<double> LinearAxisYLimitHigh
		{
			get
			{
				if( _linearAxisYLimitHigh.BeginGet() )
					LinearAxisYLimitHigh = _linearAxisYLimitHigh.Get( this );
				return _linearAxisYLimitHigh.value;
			}
			set
			{
				if( _linearAxisYLimitHigh.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYLimitHighChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearLimits( ConstraintRigid );
					}
					finally { _linearAxisYLimitHigh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYLimitHigh"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYLimitHighChanged;

		//LinearAxisYMotor
		ReferenceField<bool> _linearAxisYMotor = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Linear Axis Y Motor" )]//[DisplayName( "Motor" )]
		[Category( "Linear Axis Y" )]
		public Reference<bool> LinearAxisYMotor
		{
			get
			{
				if( _linearAxisYMotor.BeginGet() )
					LinearAxisYMotor = _linearAxisYMotor.Get( this );
				return _linearAxisYMotor.value;
			}
			set
			{
				if( _linearAxisYMotor.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYMotorChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisYMotor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYMotor"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYMotorChanged;

		//LinearAxisYMotorTargetVelocity
		ReferenceField<double> _linearAxisYMotorTargetVelocity = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Y Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		[Category( "Linear Axis Y" )]
		public Reference<double> LinearAxisYMotorTargetVelocity
		{
			get
			{
				if( _linearAxisYMotorTargetVelocity.BeginGet() )
					LinearAxisYMotorTargetVelocity = _linearAxisYMotorTargetVelocity.Get( this );
				return _linearAxisYMotorTargetVelocity.value;
			}
			set
			{
				if( _linearAxisYMotorTargetVelocity.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYMotorTargetVelocityChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisYMotorTargetVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYMotorTargetVelocity"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYMotorTargetVelocityChanged;

		//LinearAxisYMotorMaxForce
		ReferenceField<double> _linearAxisYMotorMaxForce = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Y Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		[Category( "Linear Axis Y" )]
		public Reference<double> LinearAxisYMotorMaxForce
		{
			get
			{
				if( _linearAxisYMotorMaxForce.BeginGet() )
					LinearAxisYMotorMaxForce = _linearAxisYMotorMaxForce.Get( this );
				return _linearAxisYMotorMaxForce.value;
			}
			set
			{
				if( _linearAxisYMotorMaxForce.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYMotorMaxForceChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisYMotorMaxForce.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYMotorMaxForce"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYMotorMaxForceChanged;

		//LinearAxisYMotorRestitution
		ReferenceField<double> _linearAxisYMotorRestitution = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Y Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		[Category( "Linear Axis Y" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> LinearAxisYMotorRestitution
		{
			get
			{
				if( _linearAxisYMotorRestitution.BeginGet() )
					LinearAxisYMotorRestitution = _linearAxisYMotorRestitution.Get( this );
				return _linearAxisYMotorRestitution.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _linearAxisYMotorRestitution.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYMotorRestitutionChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisYMotorRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYMotorRestitution"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYMotorRestitutionChanged;

		//LinearAxisYMotorServo
		ReferenceField<bool> _linearAxisYMotorServo = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Linear Axis Y Motor Servo" )]//[DisplayName( "Motor Servo" )]
		[Category( "Linear Axis Y" )]
		public Reference<bool> LinearAxisYMotorServo
		{
			get
			{
				if( _linearAxisYMotorServo.BeginGet() )
					LinearAxisYMotorServo = _linearAxisYMotorServo.Get( this );
				return _linearAxisYMotorServo.value;
			}
			set
			{
				if( _linearAxisYMotorServo.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYMotorServoChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisYMotorServo.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYMotorServo"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYMotorServoChanged;

		//LinearAxisYMotorServoTarget
		ReferenceField<double> _linearAxisYMotorServoTarget = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Y Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		[Category( "Linear Axis Y" )]
		public Reference<double> LinearAxisYMotorServoTarget
		{
			get
			{
				if( _linearAxisYMotorServoTarget.BeginGet() )
					LinearAxisYMotorServoTarget = _linearAxisYMotorServoTarget.Get( this );
				return _linearAxisYMotorServoTarget.value;
			}
			set
			{
				if( _linearAxisYMotorServoTarget.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYMotorServoTargetChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisYMotorServoTarget.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYMotorServoTarget"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYMotorServoTargetChanged;

		//LinearAxisYSpring
		ReferenceField<bool> _linearAxisYSpring = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Linear Axis Y Spring" )]//[DisplayName( "Spring" )]
		[Category( "Linear Axis Y" )]
		public Reference<bool> LinearAxisYSpring
		{
			get
			{
				if( _linearAxisYSpring.BeginGet() )
					LinearAxisYSpring = _linearAxisYSpring.Get( this );
				return _linearAxisYSpring.value;
			}
			set
			{
				if( _linearAxisYSpring.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYSpringChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearSprings( ConstraintRigid );
					}
					finally { _linearAxisYSpring.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYSpring"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYSpringChanged;

		//LinearAxisYSpringStiffness
		ReferenceField<double> _linearAxisYSpringStiffness = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Y Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		[Category( "Linear Axis Y" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> LinearAxisYSpringStiffness
		{
			get
			{
				if( _linearAxisYSpringStiffness.BeginGet() )
					LinearAxisYSpringStiffness = _linearAxisYSpringStiffness.Get( this );
				return _linearAxisYSpringStiffness.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _linearAxisYSpringStiffness.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYSpringStiffnessChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearSprings( ConstraintRigid );
					}
					finally { _linearAxisYSpringStiffness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYSpringStiffness"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYSpringStiffnessChanged;

		//LinearAxisYSpringDamping
		ReferenceField<double> _linearAxisYSpringDamping = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Y Spring Damping" )]//[DisplayName( "Spring Damping" )]
		[Category( "Linear Axis Y" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> LinearAxisYSpringDamping
		{
			get
			{
				if( _linearAxisYSpringDamping.BeginGet() )
					LinearAxisYSpringDamping = _linearAxisYSpringDamping.Get( this );
				return _linearAxisYSpringDamping.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _linearAxisYSpringDamping.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisYSpringDampingChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearSprings( ConstraintRigid );
					}
					finally { _linearAxisYSpringDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisYSpringDamping"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisYSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//LinearAxisZ
		ReferenceField<PhysicsAxisMode> _linearAxisZ = PhysicsAxisMode.Locked;
		/// <summary>
		/// Linear Z-axis constraint mode.
		/// </summary>
		[Serialize]
		[DefaultValue( PhysicsAxisMode.Locked )]
		[Category( "Linear Axis Z" )]
		public Reference<PhysicsAxisMode> LinearAxisZ
		{
			get
			{
				if( _linearAxisZ.BeginGet() )
					LinearAxisZ = _linearAxisZ.Get( this );
				return _linearAxisZ.value;
			}
			set
			{
				if( _linearAxisZ.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearLimits( ConstraintRigid );
					}
					finally { _linearAxisZ.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZ"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZChanged;

		//LinearAxisZLimitLow
		ReferenceField<double> _linearAxisZLimitLow = -1.0;
		[DefaultValue( -1.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Z Limit Low" )]//[DisplayName( "Limit Low" )]
		[Category( "Linear Axis Z" )]
		public Reference<double> LinearAxisZLimitLow
		{
			get
			{
				if( _linearAxisZLimitLow.BeginGet() )
					LinearAxisZLimitLow = _linearAxisZLimitLow.Get( this );
				return _linearAxisZLimitLow.value;
			}
			set
			{
				if( _linearAxisZLimitLow.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZLimitLowChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearLimits( ConstraintRigid );
					}
					finally { _linearAxisZLimitLow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZLimitLow"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZLimitLowChanged;

		//LinearAxisZLimitHigh
		ReferenceField<double> _linearAxisZLimitHigh = 1.0;
		[DefaultValue( 1.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Z Limit High" )]//[DisplayName( "Limit High" )]
		[Category( "Linear Axis Z" )]
		public Reference<double> LinearAxisZLimitHigh
		{
			get
			{
				if( _linearAxisZLimitHigh.BeginGet() )
					LinearAxisZLimitHigh = _linearAxisZLimitHigh.Get( this );
				return _linearAxisZLimitHigh.value;
			}
			set
			{
				if( _linearAxisZLimitHigh.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZLimitHighChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearLimits( ConstraintRigid );
					}
					finally { _linearAxisZLimitHigh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZLimitHigh"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZLimitHighChanged;

		//LinearAxisZMotor
		ReferenceField<bool> _linearAxisZMotor = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Linear Axis Z Motor" )]//[DisplayName( "Motor" )]
		[Category( "Linear Axis Z" )]
		public Reference<bool> LinearAxisZMotor
		{
			get
			{
				if( _linearAxisZMotor.BeginGet() )
					LinearAxisZMotor = _linearAxisZMotor.Get( this );
				return _linearAxisZMotor.value;
			}
			set
			{
				if( _linearAxisZMotor.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZMotorChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisZMotor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZMotor"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZMotorChanged;

		//LinearAxisZMotorTargetVelocity
		ReferenceField<double> _linearAxisZMotorTargetVelocity = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Z Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		[Category( "Linear Axis Z" )]
		public Reference<double> LinearAxisZMotorTargetVelocity
		{
			get
			{
				if( _linearAxisZMotorTargetVelocity.BeginGet() )
					LinearAxisZMotorTargetVelocity = _linearAxisZMotorTargetVelocity.Get( this );
				return _linearAxisZMotorTargetVelocity.value;
			}
			set
			{
				if( _linearAxisZMotorTargetVelocity.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZMotorTargetVelocityChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisZMotorTargetVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZMotorTargetVelocity"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZMotorTargetVelocityChanged;

		//LinearAxisZMotorMaxForce
		ReferenceField<double> _linearAxisZMotorMaxForce = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Z Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		[Category( "Linear Axis Z" )]
		public Reference<double> LinearAxisZMotorMaxForce
		{
			get
			{
				if( _linearAxisZMotorMaxForce.BeginGet() )
					LinearAxisZMotorMaxForce = _linearAxisZMotorMaxForce.Get( this );
				return _linearAxisZMotorMaxForce.value;
			}
			set
			{
				if( _linearAxisZMotorMaxForce.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZMotorMaxForceChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisZMotorMaxForce.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZMotorMaxForce"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZMotorMaxForceChanged;

		//LinearAxisZMotorRestitution
		ReferenceField<double> _linearAxisZMotorRestitution = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Z Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		[Category( "Linear Axis Z" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> LinearAxisZMotorRestitution
		{
			get
			{
				if( _linearAxisZMotorRestitution.BeginGet() )
					LinearAxisZMotorRestitution = _linearAxisZMotorRestitution.Get( this );
				return _linearAxisZMotorRestitution.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _linearAxisZMotorRestitution.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZMotorRestitutionChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisZMotorRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZMotorRestitution"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZMotorRestitutionChanged;

		//LinearAxisZMotorServo
		ReferenceField<bool> _linearAxisZMotorServo = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Linear Axis Z Motor Servo" )]//[DisplayName( "Motor Servo" )]
		[Category( "Linear Axis Z" )]
		public Reference<bool> LinearAxisZMotorServo
		{
			get
			{
				if( _linearAxisZMotorServo.BeginGet() )
					LinearAxisZMotorServo = _linearAxisZMotorServo.Get( this );
				return _linearAxisZMotorServo.value;
			}
			set
			{
				if( _linearAxisZMotorServo.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZMotorServoChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisZMotorServo.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZMotorServo"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZMotorServoChanged;

		//LinearAxisZMotorServoTarget
		ReferenceField<double> _linearAxisZMotorServoTarget = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Z Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		[Category( "Linear Axis Z" )]
		public Reference<double> LinearAxisZMotorServoTarget
		{
			get
			{
				if( _linearAxisZMotorServoTarget.BeginGet() )
					LinearAxisZMotorServoTarget = _linearAxisZMotorServoTarget.Get( this );
				return _linearAxisZMotorServoTarget.value;
			}
			set
			{
				if( _linearAxisZMotorServoTarget.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZMotorServoTargetChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearMotors( ConstraintRigid );
					}
					finally { _linearAxisZMotorServoTarget.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZMotorServoTarget"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZMotorServoTargetChanged;

		//LinearAxisZSpring
		ReferenceField<bool> _linearAxisZSpring = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Linear Axis Z Spring" )]//[DisplayName( "Spring" )]
		[Category( "Linear Axis Z" )]
		public Reference<bool> LinearAxisZSpring
		{
			get
			{
				if( _linearAxisZSpring.BeginGet() )
					LinearAxisZSpring = _linearAxisZSpring.Get( this );
				return _linearAxisZSpring.value;
			}
			set
			{
				if( _linearAxisZSpring.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZSpringChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearSprings( ConstraintRigid );
					}
					finally { _linearAxisZSpring.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZSpring"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZSpringChanged;

		//LinearAxisZSpringStiffness
		ReferenceField<double> _linearAxisZSpringStiffness = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Z Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		[Category( "Linear Axis Z" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> LinearAxisZSpringStiffness
		{
			get
			{
				if( _linearAxisZSpringStiffness.BeginGet() )
					LinearAxisZSpringStiffness = _linearAxisZSpringStiffness.Get( this );
				return _linearAxisZSpringStiffness.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _linearAxisZSpringStiffness.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZSpringStiffnessChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearSprings( ConstraintRigid );
					}
					finally { _linearAxisZSpringStiffness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZSpringStiffness"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZSpringStiffnessChanged;

		//LinearAxisZSpringDamping
		ReferenceField<double> _linearAxisZSpringDamping = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Linear Axis Z Spring Damping" )]//[DisplayName( "Spring Damping" )]
		[Category( "Linear Axis Z" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> LinearAxisZSpringDamping
		{
			get
			{
				if( _linearAxisZSpringDamping.BeginGet() )
					LinearAxisZSpringDamping = _linearAxisZSpringDamping.Get( this );
				return _linearAxisZSpringDamping.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _linearAxisZSpringDamping.BeginSet( ref value ) )
				{
					try
					{
						LinearAxisZSpringDampingChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateLinearSprings( ConstraintRigid );
					}
					finally { _linearAxisZSpringDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearAxisZSpringDamping"/> property value changes.</summary>
		public event Action<Component_Constraint> LinearAxisZSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//AngularAxisX
		ReferenceField<PhysicsAxisMode> _angularAxisX = PhysicsAxisMode.Locked;
		/// <summary>
		/// Angular X-axis constraint mode.
		/// </summary>
		[Serialize]
		[DefaultValue( PhysicsAxisMode.Locked )]
		[Category( "Angular Axis X" )]
		public Reference<PhysicsAxisMode> AngularAxisX
		{
			get
			{
				if( _angularAxisX.BeginGet() )
					AngularAxisX = _angularAxisX.Get( this );
				return _angularAxisX.value;
			}
			set
			{
				if( _angularAxisX.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularLimits( ConstraintRigid );
					}
					finally { _angularAxisX.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisX"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXChanged;

		//AngularAxisXLimitLow
		ReferenceField<Degree> _angularAxisXLimitLow = new Degree( -45.0 );
		[Serialize]
		[DefaultValue( "-45" )]
		[Range( -180, 180 )]
		[DisplayName( "Angular Axis X Limit Low" )]//[DisplayName( "Limit Low" )]
		[Category( "Angular Axis X" )]
		public Reference<Degree> AngularAxisXLimitLow
		{
			get
			{
				if( _angularAxisXLimitLow.BeginGet() )
					AngularAxisXLimitLow = _angularAxisXLimitLow.Get( this );
				return _angularAxisXLimitLow.value;
			}
			set
			{
				if( _angularAxisXLimitLow.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXLimitLowChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularLimits( ConstraintRigid );
					}
					finally { _angularAxisXLimitLow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXLimitLow"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXLimitLowChanged;

		//AngularAxisXLimitHigh
		ReferenceField<Degree> _angularAxisXLimitHigh = new Degree( 45.0 );
		[Serialize]
		[DefaultValue( "45" )]
		[Range( -180, 180 )]
		[DisplayName( "Angular Axis X Limit High" )]//[DisplayName( "Limit High" )]
		[Category( "Angular Axis X" )]
		public Reference<Degree> AngularAxisXLimitHigh
		{
			get
			{
				if( _angularAxisXLimitHigh.BeginGet() )
					AngularAxisXLimitHigh = _angularAxisXLimitHigh.Get( this );
				return _angularAxisXLimitHigh.value;
			}
			set
			{
				if( _angularAxisXLimitHigh.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXLimitHighChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularLimits( ConstraintRigid );
					}
					finally { _angularAxisXLimitHigh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXLimitHigh"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXLimitHighChanged;


		//AngularAxisXMotor
		ReferenceField<bool> _angularAxisXMotor;
		[Serialize]
		[DefaultValue( false )]
		[DisplayName( "Angular Axis X Motor" )]//[DisplayName( "Motor" )]
		[Category( "Angular Axis X" )]
		public Reference<bool> AngularAxisXMotor
		{
			get
			{
				if( _angularAxisXMotor.BeginGet() )
					AngularAxisXMotor = _angularAxisXMotor.Get( this );
				return _angularAxisXMotor.value;
			}
			set
			{
				if( _angularAxisXMotor.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXMotorChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisXMotor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXMotor"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXMotorChanged;

		//AngularAxisXMotorTargetVelocity
		ReferenceField<double> _angularAxisXMotorTargetVelocity = 0.0;
		[Serialize]
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Axis X Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		[Category( "Angular Axis X" )]
		public Reference<double> AngularAxisXMotorTargetVelocity
		{
			get
			{
				if( _angularAxisXMotorTargetVelocity.BeginGet() )
					AngularAxisXMotorTargetVelocity = _angularAxisXMotorTargetVelocity.Get( this );
				return _angularAxisXMotorTargetVelocity.value;
			}
			set
			{
				if( _angularAxisXMotorTargetVelocity.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXMotorTargetVelocityChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisXMotorTargetVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXMotorTargetVelocity"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXMotorTargetVelocityChanged;

		//AngularAxisXMotorMaxForce
		ReferenceField<double> _angularAxisXMotorMaxForce = 0.0;
		[Serialize]
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Axis X Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		[Category( "Angular Axis X" )]
		public Reference<double> AngularAxisXMotorMaxForce
		{
			get
			{
				if( _angularAxisXMotorMaxForce.BeginGet() )
					AngularAxisXMotorMaxForce = _angularAxisXMotorMaxForce.Get( this );
				return _angularAxisXMotorMaxForce.value;
			}
			set
			{
				if( _angularAxisXMotorMaxForce.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXMotorMaxForceChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisXMotorMaxForce.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXMotorMaxForce"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXMotorMaxForceChanged;

		//AngularAxisXMotorRestitution
		ReferenceField<double> _angularAxisXMotorRestitution = 0.0;
		[Serialize]
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Axis X Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		[Category( "Angular Axis X" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AngularAxisXMotorRestitution
		{
			get
			{
				if( _angularAxisXMotorRestitution.BeginGet() )
					AngularAxisXMotorRestitution = _angularAxisXMotorRestitution.Get( this );
				return _angularAxisXMotorRestitution.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _angularAxisXMotorRestitution.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXMotorRestitutionChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisXMotorRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXMotorRestitution"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXMotorRestitutionChanged;

		//AngularAxisXServo
		ReferenceField<bool> _angularAxisXServo = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Angular Axis X Motor Servo" )]//[DisplayName( "Motor Servo" )]
		[Category( "Angular Axis X" )]
		public Reference<bool> AngularAxisXServo
		{
			get
			{
				if( _angularAxisXServo.BeginGet() )
					AngularAxisXServo = _angularAxisXServo.Get( this );
				return _angularAxisXServo.value;
			}
			set
			{
				if( _angularAxisXServo.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXServoChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisXServo.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXServo"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXServoChanged;

		//AngularAxisXServoTarget
		ReferenceField<Degree> _angularAxisXServoTarget = new Degree( 0 );
		[DefaultValue( "0" )]
		[Range( -180.0, 180.0 )]
		[Serialize]
		[DisplayName( "Angular Axis X Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		[Category( "Angular Axis X" )]
		public Reference<Degree> AngularAxisXServoTarget
		{
			get
			{
				if( _angularAxisXServoTarget.BeginGet() )
					AngularAxisXServoTarget = _angularAxisXServoTarget.Get( this );
				return _angularAxisXServoTarget.value;
			}
			set
			{
				if( _angularAxisXServoTarget.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXServoTargetChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisXServoTarget.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXServoTarget"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXServoTargetChanged;

		//AngularAxisXSpring
		ReferenceField<bool> _angularAxisXSpring = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Angular Axis X Spring" )]//[DisplayName( "Spring" )]
		[Category( "Angular Axis X" )]
		public Reference<bool> AngularAxisXSpring
		{
			get
			{
				if( _angularAxisXSpring.BeginGet() )
					AngularAxisXSpring = _angularAxisXSpring.Get( this );
				return _angularAxisXSpring.value;
			}
			set
			{
				if( _angularAxisXSpring.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXSpringChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularSprings( ConstraintRigid );
					}
					finally { _angularAxisXSpring.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXSpring"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXSpringChanged;

		//AngularAxisXSpringStiffness
		ReferenceField<double> _angularAxisXSpringStiffness = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Angular Axis X Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		[Category( "Angular Axis X" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AngularAxisXSpringStiffness
		{
			get
			{
				if( _angularAxisXSpringStiffness.BeginGet() )
					AngularAxisXSpringStiffness = _angularAxisXSpringStiffness.Get( this );
				return _angularAxisXSpringStiffness.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _angularAxisXSpringStiffness.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXSpringStiffnessChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularSprings( ConstraintRigid );
					}
					finally { _angularAxisXSpringStiffness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXSpringStiffness"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXSpringStiffnessChanged;

		//AngularAxisXSpringDamping
		ReferenceField<double> _angularAxisXSpringDamping = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Angular Axis X Spring Damping" )]//[DisplayName( "Spring Damping" )]
		[Category( "Angular Axis X" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AngularAxisXSpringDamping
		{
			get
			{
				if( _angularAxisXSpringDamping.BeginGet() )
					AngularAxisXSpringDamping = _angularAxisXSpringDamping.Get( this );
				return _angularAxisXSpringDamping.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _angularAxisXSpringDamping.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisXSpringDampingChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularSprings( ConstraintRigid );
					}
					finally { _angularAxisXSpringDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisXSpringDamping"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisXSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//AngularAxisY
		ReferenceField<PhysicsAxisMode> _angularAxisY = PhysicsAxisMode.Locked;
		/// <summary>
		/// Angular Y-axis constraint mode.
		/// </summary>
		[Serialize]
		[DefaultValue( PhysicsAxisMode.Locked )]
		[Category( "Angular Axis Y" )]
		public Reference<PhysicsAxisMode> AngularAxisY
		{
			get
			{
				if( _angularAxisY.BeginGet() )
					AngularAxisY = _angularAxisY.Get( this );
				return _angularAxisY.value;
			}
			set
			{
				if( _angularAxisY.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularLimits( ConstraintRigid );
					}
					finally { _angularAxisY.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisY"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYChanged;

		//AngularAxisYLimitLow
		ReferenceField<Degree> _angularAxisYLimitLow = new Degree( -45.0 );
		[Serialize]
		[DefaultValue( "-45" )]
		[Range( -90.0, 90.0 )]
		[DisplayName( "Angular Axis Y Limit Low" )]//[DisplayName( "Limit Low" )]
		[Category( "Angular Axis Y" )]
		public Reference<Degree> AngularAxisYLimitLow
		{
			get
			{
				if( _angularAxisYLimitLow.BeginGet() )
					AngularAxisYLimitLow = _angularAxisYLimitLow.Get( this );
				return _angularAxisYLimitLow.value;
			}
			set
			{
				if( _angularAxisYLimitLow.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYLimitLowChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularLimits( ConstraintRigid );
					}
					finally { _angularAxisYLimitLow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYLimitLow"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYLimitLowChanged;

		//AngularAxisYLimitHigh
		ReferenceField<Degree> _angularAxisYLimitHigh = new Degree( 45.0 );
		[Serialize]
		[DefaultValue( "45" )]
		[Range( -90.0, 90.0 )]
		[DisplayName( "Angular Axis Y Limit High" )]//[DisplayName( "Limit High" )]
		[Category( "Angular Axis Y" )]
		public Reference<Degree> AngularAxisYLimitHigh
		{
			get
			{
				if( _angularAxisYLimitHigh.BeginGet() )
					AngularAxisYLimitHigh = _angularAxisYLimitHigh.Get( this );
				return _angularAxisYLimitHigh.value;
			}
			set
			{
				if( _angularAxisYLimitHigh.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYLimitHighChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularLimits( ConstraintRigid );
					}
					finally { _angularAxisYLimitHigh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYLimitHigh"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYLimitHighChanged;

		//AngularAxisYMotor
		ReferenceField<bool> _angularAxisYMotor;
		[Serialize]
		[DefaultValue( false )]
		[DisplayName( "Angular Axis Y Motor" )]//[DisplayName( "Motor" )]
		[Category( "Angular Axis Y" )]
		public Reference<bool> AngularAxisYMotor
		{
			get
			{
				if( _angularAxisYMotor.BeginGet() )
					AngularAxisYMotor = _angularAxisYMotor.Get( this );
				return _angularAxisYMotor.value;
			}
			set
			{
				if( _angularAxisYMotor.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYMotorChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisYMotor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYMotor"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYMotorChanged;

		//AngularAxisYMotorTargetVelocity
		ReferenceField<double> _angularAxisYMotorTargetVelocity = 0.0;
		[Serialize]
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Axis Y Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		[Category( "Angular Axis Y" )]
		public Reference<double> AngularAxisYMotorTargetVelocity
		{
			get
			{
				if( _angularAxisYMotorTargetVelocity.BeginGet() )
					AngularAxisYMotorTargetVelocity = _angularAxisYMotorTargetVelocity.Get( this );
				return _angularAxisYMotorTargetVelocity.value;
			}
			set
			{
				if( _angularAxisYMotorTargetVelocity.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYMotorTargetVelocityChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisYMotorTargetVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYMotorTargetVelocity"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYMotorTargetVelocityChanged;

		//AngularAxisYMotorMaxForce
		ReferenceField<double> _angularAxisYMotorMaxForce = 0.0;
		[Serialize]
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Axis Y Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		[Category( "Angular Axis Y" )]
		public Reference<double> AngularAxisYMotorMaxForce
		{
			get
			{
				if( _angularAxisYMotorMaxForce.BeginGet() )
					AngularAxisYMotorMaxForce = _angularAxisYMotorMaxForce.Get( this );
				return _angularAxisYMotorMaxForce.value;
			}
			set
			{
				if( _angularAxisYMotorMaxForce.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYMotorMaxForceChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisYMotorMaxForce.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYMotorMaxForce"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYMotorMaxForceChanged;

		//AngularAxisYMotorRestitution
		ReferenceField<double> _angularAxisYMotorRestitution = 0.0;
		[Serialize]
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Axis Y Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		[Category( "Angular Axis Y" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AngularAxisYMotorRestitution
		{
			get
			{
				if( _angularAxisYMotorRestitution.BeginGet() )
					AngularAxisYMotorRestitution = _angularAxisYMotorRestitution.Get( this );
				return _angularAxisYMotorRestitution.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _angularAxisYMotorRestitution.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYMotorRestitutionChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisYMotorRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYMotorRestitution"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYMotorRestitutionChanged;

		//AngularAxisYServo
		ReferenceField<bool> _angularAxisYServo = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Angular Axis Y Motor Servo" )]//[DisplayName( "Motor Servo" )]
		[Category( "Angular Axis Y" )]
		public Reference<bool> AngularAxisYServo
		{
			get
			{
				if( _angularAxisYServo.BeginGet() )
					AngularAxisYServo = _angularAxisYServo.Get( this );
				return _angularAxisYServo.value;
			}
			set
			{
				if( _angularAxisYServo.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYServoChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisYServo.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYServo"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYServoChanged;

		//AngularAxisYServoTarget
		ReferenceField<Degree> _angularAxisYServoTarget = new Degree( 0 );
		[DefaultValue( "0" )]
		[Range( -180.0, 180.0 )]
		[Serialize]
		[DisplayName( "Angular Axis Y Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		[Category( "Angular Axis Y" )]
		public Reference<Degree> AngularAxisYServoTarget
		{
			get
			{
				if( _angularAxisYServoTarget.BeginGet() )
					AngularAxisYServoTarget = _angularAxisYServoTarget.Get( this );
				return _angularAxisYServoTarget.value;
			}
			set
			{
				if( _angularAxisYServoTarget.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYServoTargetChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisYServoTarget.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYServoTarget"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYServoTargetChanged;

		//AngularAxisYSpring
		ReferenceField<bool> _angularAxisYSpring = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Angular Axis Y Spring" )]//[DisplayName( "Spring" )]
		[Category( "Angular Axis Y" )]
		public Reference<bool> AngularAxisYSpring
		{
			get
			{
				if( _angularAxisYSpring.BeginGet() )
					AngularAxisYSpring = _angularAxisYSpring.Get( this );
				return _angularAxisYSpring.value;
			}
			set
			{
				if( _angularAxisYSpring.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYSpringChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularSprings( ConstraintRigid );
					}
					finally { _angularAxisYSpring.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYSpring"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYSpringChanged;

		//AngularAxisYSpringStiffness
		ReferenceField<double> _angularAxisYSpringStiffness = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Angular Axis Y Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		[Category( "Angular Axis Y" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AngularAxisYSpringStiffness
		{
			get
			{
				if( _angularAxisYSpringStiffness.BeginGet() )
					AngularAxisYSpringStiffness = _angularAxisYSpringStiffness.Get( this );
				return _angularAxisYSpringStiffness.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _angularAxisYSpringStiffness.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYSpringStiffnessChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularSprings( ConstraintRigid );
					}
					finally { _angularAxisYSpringStiffness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYSpringStiffness"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYSpringStiffnessChanged;

		//AngularAxisYSpringDamping
		ReferenceField<double> _angularAxisYSpringDamping = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Angular Axis Y Spring Damping" )]//[DisplayName( "Spring Damping" )]
		[Category( "Angular Axis Y" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AngularAxisYSpringDamping
		{
			get
			{
				if( _angularAxisYSpringDamping.BeginGet() )
					AngularAxisYSpringDamping = _angularAxisYSpringDamping.Get( this );
				return _angularAxisYSpringDamping.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _angularAxisYSpringDamping.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisYSpringDampingChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularSprings( ConstraintRigid );
					}
					finally { _angularAxisYSpringDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisYSpringDamping"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisYSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//AngularAxisZ
		ReferenceField<PhysicsAxisMode> _angularAxisZ = PhysicsAxisMode.Locked;
		/// <summary>
		/// Angular Z-axis constraint mode.
		/// </summary>
		[Serialize]
		[DefaultValue( PhysicsAxisMode.Locked )]
		[Category( "Angular Axis Z" )]
		public Reference<PhysicsAxisMode> AngularAxisZ
		{
			get
			{
				if( _angularAxisZ.BeginGet() )
					AngularAxisZ = _angularAxisZ.Get( this );
				return _angularAxisZ.value;
			}
			set
			{
				if( _angularAxisZ.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularLimits( ConstraintRigid );
					}
					finally { _angularAxisZ.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZ"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZChanged;

		//AngularAxisZLimitLow
		ReferenceField<Degree> _angularAxisZLimitLow = new Degree( -45 );
		[Serialize]
		[DefaultValue( "-45" )]
		[Range( -180, 180 )]
		[DisplayName( "Angular Axis Z Limit Low" )]//[DisplayName( "Limit Low" )]
		[Category( "Angular Axis Z" )]
		public Reference<Degree> AngularAxisZLimitLow
		{
			get
			{
				if( _angularAxisZLimitLow.BeginGet() )
					AngularAxisZLimitLow = _angularAxisZLimitLow.Get( this );
				return _angularAxisZLimitLow.value;
			}
			set
			{
				if( _angularAxisZLimitLow.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZLimitLowChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularLimits( ConstraintRigid );
					}
					finally { _angularAxisZLimitLow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZLimitLow"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZLimitLowChanged;

		//AngularAxisZLimitHigh
		ReferenceField<Degree> _angularAxisZLimitHigh = new Degree( 45 );
		[Serialize]
		[DefaultValue( "45" )]
		[Range( -180, 180 )]
		[DisplayName( "Angular Axis Z Limit High" )]//[DisplayName( "Limit High" )]
		[Category( "Angular Axis Z" )]
		public Reference<Degree> AngularAxisZLimitHigh
		{
			get
			{
				if( _angularAxisZLimitHigh.BeginGet() )
					AngularAxisZLimitHigh = _angularAxisZLimitHigh.Get( this );
				return _angularAxisZLimitHigh.value;
			}
			set
			{
				if( _angularAxisZLimitHigh.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZLimitHighChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularLimits( ConstraintRigid );
					}
					finally { _angularAxisZLimitHigh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZLimitHigh"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZLimitHighChanged;

		//AngularAxisZMotor
		ReferenceField<bool> _angularAxisZMotor;
		[Serialize]
		[DefaultValue( false )]
		[DisplayName( "Angular Axis Z Motor" )]//[DisplayName( "Motor" )]
		[Category( "Angular Axis Z" )]
		public Reference<bool> AngularAxisZMotor
		{
			get
			{
				if( _angularAxisZMotor.BeginGet() )
					AngularAxisZMotor = _angularAxisZMotor.Get( this );
				return _angularAxisZMotor.value;
			}
			set
			{
				if( _angularAxisZMotor.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZMotorChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisZMotor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZMotor"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZMotorChanged;

		//AngularAxisZMotorTargetVelocity
		ReferenceField<double> _angularAxisZMotorTargetVelocity = 0.0;
		[Serialize]
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Axis Z Motor Target Velocity" )]//[DisplayName( "Motor Target Velocity" )]
		[Category( "Angular Axis Z" )]
		public Reference<double> AngularAxisZMotorTargetVelocity
		{
			get
			{
				if( _angularAxisZMotorTargetVelocity.BeginGet() )
					AngularAxisZMotorTargetVelocity = _angularAxisZMotorTargetVelocity.Get( this );
				return _angularAxisZMotorTargetVelocity.value;
			}
			set
			{
				if( _angularAxisZMotorTargetVelocity.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZMotorTargetVelocityChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisZMotorTargetVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZMotorTargetVelocity"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZMotorTargetVelocityChanged;

		//AngularAxisZMotorMaxForce
		ReferenceField<double> _angularAxisZMotorMaxForce = 0.0;
		[Serialize]
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Axis Z Motor Max Force" )]//[DisplayName( "Motor Max Force" )]
		[Category( "Angular Axis Z" )]
		public Reference<double> AngularAxisZMotorMaxForce
		{
			get
			{
				if( _angularAxisZMotorMaxForce.BeginGet() )
					AngularAxisZMotorMaxForce = _angularAxisZMotorMaxForce.Get( this );
				return _angularAxisZMotorMaxForce.value;
			}
			set
			{
				if( _angularAxisZMotorMaxForce.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZMotorMaxForceChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisZMotorMaxForce.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZMotorMaxForce"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZMotorMaxForceChanged;

		//AngularAxisZMotorRestitution
		ReferenceField<double> _angularAxisZMotorRestitution = 0.0;
		[Serialize]
		[DefaultValue( 0.0 )]
		[DisplayName( "Angular Axis Z Motor Restitution" )]//[DisplayName( "Motor Restitution" )]
		[Category( "Angular Axis Z" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AngularAxisZMotorRestitution
		{
			get
			{
				if( _angularAxisZMotorRestitution.BeginGet() )
					AngularAxisZMotorRestitution = _angularAxisZMotorRestitution.Get( this );
				return _angularAxisZMotorRestitution.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _angularAxisZMotorRestitution.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZMotorRestitutionChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisZMotorRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZMotorRestitution"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZMotorRestitutionChanged;

		//AngularAxisZServo
		ReferenceField<bool> _angularAxisZServo = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Angular Axis Z Motor Servo" )]//[DisplayName( "Motor Servo" )]
		[Category( "Angular Axis Z" )]
		public Reference<bool> AngularAxisZServo
		{
			get
			{
				if( _angularAxisZServo.BeginGet() )
					AngularAxisZServo = _angularAxisZServo.Get( this );
				return _angularAxisZServo.value;
			}
			set
			{
				if( _angularAxisZServo.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZServoChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisZServo.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZServo"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZServoChanged;

		//AngularAxisZServoTarget
		ReferenceField<Degree> _angularAxisZServoTarget = new Degree( 0 );
		[DefaultValue( "0" )]
		[Range( -180.0, 180.0 )]
		[Serialize]
		[DisplayName( "Angular Axis Z Motor Servo Target" )]//[DisplayName( "Motor Servo Target" )]
		[Category( "Angular Axis Z" )]
		public Reference<Degree> AngularAxisZServoTarget
		{
			get
			{
				if( _angularAxisZServoTarget.BeginGet() )
					AngularAxisZServoTarget = _angularAxisZServoTarget.Get( this );
				return _angularAxisZServoTarget.value;
			}
			set
			{
				if( _angularAxisZServoTarget.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZServoTargetChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularMotors( ConstraintRigid );
					}
					finally { _angularAxisZServoTarget.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZServoTarget"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZServoTargetChanged;

		//AngularAxisZSpring
		ReferenceField<bool> _angularAxisZSpring = false;
		[DefaultValue( false )]
		[Serialize]
		[DisplayName( "Angular Axis Z Spring" )]//[DisplayName( "Spring" )]
		[Category( "Angular Axis Z" )]
		public Reference<bool> AngularAxisZSpring
		{
			get
			{
				if( _angularAxisZSpring.BeginGet() )
					AngularAxisZSpring = _angularAxisZSpring.Get( this );
				return _angularAxisZSpring.value;
			}
			set
			{
				if( _angularAxisZSpring.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZSpringChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularSprings( ConstraintRigid );
					}
					finally { _angularAxisZSpring.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZSpring"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZSpringChanged;

		//AngularAxisZSpringStiffness
		ReferenceField<double> _angularAxisZSpringStiffness = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Angular Axis Z Spring Stiffness" )]//[DisplayName( "Spring Stiffness" )]
		[Category( "Angular Axis Z" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AngularAxisZSpringStiffness
		{
			get
			{
				if( _angularAxisZSpringStiffness.BeginGet() )
					AngularAxisZSpringStiffness = _angularAxisZSpringStiffness.Get( this );
				return _angularAxisZSpringStiffness.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _angularAxisZSpringStiffness.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZSpringStiffnessChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularSprings( ConstraintRigid );
					}
					finally { _angularAxisZSpringStiffness.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZSpringStiffness"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZSpringStiffnessChanged;

		//AngularAxisZSpringDamping
		ReferenceField<double> _angularAxisZSpringDamping = 0.0;
		[DefaultValue( 0.0 )]
		[Serialize]
		[DisplayName( "Angular Axis Z Spring Damping" )]//[DisplayName( "Spring Damping" )]
		[Category( "Angular Axis Z" )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> AngularAxisZSpringDamping
		{
			get
			{
				if( _angularAxisZSpringDamping.BeginGet() )
					AngularAxisZSpringDamping = _angularAxisZSpringDamping.Get( this );
				return _angularAxisZSpringDamping.value;
			}
			set
			{
				value = new Reference<double>( Math.Max( 0, value.Value ), value.GetByReference );
				if( _angularAxisZSpringDamping.BeginSet( ref value ) )
				{
					try
					{
						AngularAxisZSpringDampingChanged?.Invoke( this );
						if( ConstraintRigid != null )
							UpdateAngularSprings( ConstraintRigid );
					}
					finally { _angularAxisZSpringDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularAxisZSpringDamping"/> property value changes.</summary>
		public event Action<Component_Constraint> AngularAxisZSpringDampingChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				//var bodyA = BodyA.Value;

				switch( member.Name )
				{
				//case nameof( CollisionsBetweenLinkedBodies ):
				//	skip = BodyB.Value == null;
				//	break;
				case nameof( BreakingImpulseThreshold ):
				case nameof( OverrideNumberSolverIterations ):
					if( SoftBodyMode )
						skip = true;
					break;
				}

				//if( bodyA == null )
				//{
				//	// skip all other except "BodyA"
				//	skip = member.Name != nameof( BodyA );

				//	return;
				//}

				LinearMembersFilter( context, member.Name, ref skip );
				AngularMembersFilter( context, member.Name, ref skip );
			}
		}

		void LinearMembersFilter( Metadata.GetMembersContext context, string memberName, ref bool skip )
		{
			var sbMode = SoftBodyMode;

			switch( memberName )
			{
			//LinearAxisX
			case nameof( LinearAxisXLimitLow ):
			case nameof( LinearAxisXLimitHigh ):
				if( LinearAxisX.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;
			case nameof( LinearAxisXMotor ):
				if( LinearAxisX.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisXMotorServo ):
			case nameof( LinearAxisXMotorTargetVelocity ):
			case nameof( LinearAxisXMotorMaxForce ):
			case nameof( LinearAxisXMotorRestitution ):
				if( LinearAxisX.Value == PhysicsAxisMode.Locked || !LinearAxisXMotor || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisXMotorServoTarget ):
				if( LinearAxisX.Value == PhysicsAxisMode.Locked || !LinearAxisXMotor || !LinearAxisXMotorServo || sbMode )
					skip = true;
				break;

			case nameof( LinearAxisXSpring ):
				if( LinearAxisX.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisXSpringStiffness ):
			case nameof( LinearAxisXSpringDamping ):
				if( LinearAxisX.Value == PhysicsAxisMode.Locked || !LinearAxisXSpring || sbMode )
					skip = true;
				break;

			//LinearAxisY
			case nameof( LinearAxisYLimitLow ):
			case nameof( LinearAxisYLimitHigh ):
				if( LinearAxisY.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;
			case nameof( LinearAxisYMotor ):
				if( LinearAxisY.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisYMotorServo ):
			case nameof( LinearAxisYMotorTargetVelocity ):
			case nameof( LinearAxisYMotorMaxForce ):
			case nameof( LinearAxisYMotorRestitution ):
				if( LinearAxisY.Value == PhysicsAxisMode.Locked || !LinearAxisYMotor || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisYMotorServoTarget ):
				if( LinearAxisY.Value == PhysicsAxisMode.Locked || !LinearAxisYMotor || !LinearAxisYMotorServo || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisYSpring ):
				if( LinearAxisY.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisYSpringStiffness ):
			case nameof( LinearAxisYSpringDamping ):
				if( LinearAxisY.Value == PhysicsAxisMode.Locked || !LinearAxisYSpring || sbMode )
					skip = true;
				break;

			//LinearAxisZ
			case nameof( LinearAxisZLimitLow ):
			case nameof( LinearAxisZLimitHigh ):
				if( LinearAxisZ.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;
			case nameof( LinearAxisZMotor ):
				if( LinearAxisZ.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisZMotorServo ):
			case nameof( LinearAxisZMotorTargetVelocity ):
			case nameof( LinearAxisZMotorMaxForce ):
			case nameof( LinearAxisZMotorRestitution ):
				if( LinearAxisZ.Value == PhysicsAxisMode.Locked || !LinearAxisZMotor || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisZMotorServoTarget ):
				if( LinearAxisZ.Value == PhysicsAxisMode.Locked || !LinearAxisZMotor || !LinearAxisZMotorServo || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisZSpring ):
				if( LinearAxisZ.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( LinearAxisZSpringStiffness ):
			case nameof( LinearAxisZSpringDamping ):
				if( LinearAxisZ.Value == PhysicsAxisMode.Locked || !LinearAxisZSpring || sbMode )
					skip = true;
				break;
			}
		}

		void AngularMembersFilter( Metadata.GetMembersContext context, string memberName, ref bool skip )
		{
			var sbMode = SoftBodyMode;

			switch( memberName )
			{
			//AngularAxisX
			case nameof( AngularAxisXLimitLow ):
			case nameof( AngularAxisXLimitHigh ):
				if( AngularAxisX.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;
			case nameof( AngularAxisXMotor ):
				if( AngularAxisX.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisXServo ):
			case nameof( AngularAxisXMotorTargetVelocity ):
			case nameof( AngularAxisXMotorMaxForce ):
			case nameof( AngularAxisXMotorRestitution ):
				if( AngularAxisX.Value == PhysicsAxisMode.Locked || !AngularAxisXMotor || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisXServoTarget ):
				if( AngularAxisX.Value == PhysicsAxisMode.Locked || !AngularAxisXMotor || !AngularAxisXServo || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisXSpring ):
				if( AngularAxisX.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisXSpringStiffness ):
			case nameof( AngularAxisXSpringDamping ):
				if( AngularAxisX.Value == PhysicsAxisMode.Locked || !AngularAxisXSpring || sbMode )
					skip = true;
				break;

			//AngularAxisY
			case nameof( AngularAxisYLimitLow ):
			case nameof( AngularAxisYLimitHigh ):
				if( AngularAxisY.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;
			case nameof( AngularAxisYMotor ):
				if( AngularAxisY.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisYServo ):
			case nameof( AngularAxisYMotorTargetVelocity ):
			case nameof( AngularAxisYMotorMaxForce ):
			case nameof( AngularAxisYMotorRestitution ):
				if( AngularAxisY.Value == PhysicsAxisMode.Locked || !AngularAxisYMotor || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisYServoTarget ):
				if( AngularAxisY.Value == PhysicsAxisMode.Locked || !AngularAxisYMotor || !AngularAxisYServo || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisYSpring ):
				if( AngularAxisY.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisYSpringStiffness ):
			case nameof( AngularAxisYSpringDamping ):
				if( AngularAxisY.Value == PhysicsAxisMode.Locked || !AngularAxisYSpring || sbMode )
					skip = true;
				break;

			//AngularAxisZ
			case nameof( AngularAxisZLimitLow ):
			case nameof( AngularAxisZLimitHigh ):
				if( AngularAxisZ.Value != PhysicsAxisMode.Limited )
					skip = true;
				break;
			case nameof( AngularAxisZMotor ):
				if( AngularAxisZ.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisZServo ):
			case nameof( AngularAxisZMotorTargetVelocity ):
			case nameof( AngularAxisZMotorMaxForce ):
			case nameof( AngularAxisZMotorRestitution ):
				if( AngularAxisZ.Value == PhysicsAxisMode.Locked || !AngularAxisZMotor || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisZServoTarget ):
				if( AngularAxisZ.Value == PhysicsAxisMode.Locked || !AngularAxisZMotor || !AngularAxisZServo || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisZSpring ):
				if( AngularAxisZ.Value == PhysicsAxisMode.Locked || sbMode )
					skip = true;
				break;
			case nameof( AngularAxisZSpringStiffness ):
			case nameof( AngularAxisZSpringDamping ):
				if( AngularAxisZ.Value == PhysicsAxisMode.Locked || !AngularAxisZSpring || sbMode )
					skip = true;
				break;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// A set of settings for <see cref="Component_Constraint"/> creation in the editor.
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
				var c = (Component_Constraint)context.newObject;

				//!!!!Constraint: создание: если выделены тела, то констрейнт становится чилдом
				////get bodies from selected objects
				//{
				//	//!!!!пока так. надо брать из документа
				//	var selectedObjects = SettingsWindow.Instance._SelectedObjects;
				//	if( selectedObjects.Length == 2 )
				//	{
				//		var bodyA = selectedObjects[ 0 ] as Component_RigidBody;
				//		var bodyB = selectedObjects[ 1 ] as Component_RigidBody;
				//		if( bodyA != null && bodyB != null )
				//		{
				//			c.BodyA = new Reference<Component_RigidBody>( null, ReferenceUtils.CalculateThisReference( c, bodyA ) );
				//			c.BodyB = new Reference<Component_RigidBody>( null, ReferenceUtils.CalculateThisReference( c, bodyB ) );

				//			//!!!!можно луч от курсора учитывать
				//			var pos = ( bodyA.Transform.Value.Position + bodyB.Transform.Value.Position ) * 0.5;
				//			c.Transform = new Transform( pos, Quat.Identity );
				//		}
				//	}
				//}

				switch( InitialConfiguration )
				{
				case InitialConfigurationEnum.Hinge:
					c.AngularAxisZ = PhysicsAxisMode.Free;
					break;

				case InitialConfigurationEnum.Point:
					c.AngularAxisX = PhysicsAxisMode.Free;
					c.AngularAxisY = PhysicsAxisMode.Free;
					c.AngularAxisZ = PhysicsAxisMode.Free;
					break;

				case InitialConfigurationEnum.ConeTwist:
					//!!!!так?
					c.AngularAxisX = PhysicsAxisMode.Free;
					c.AngularAxisY = PhysicsAxisMode.Limited;
					c.AngularAxisZ = PhysicsAxisMode.Limited;
					break;

				case InitialConfigurationEnum.Slider:
					c.LinearAxisX = PhysicsAxisMode.Free;
					break;
				}

				return base.Creation( context );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Browsable( false )]
		public Generic6DofSpring2Constraint ConstraintRigid
		{
			get { return constraintRigid; }
		}

		public void UpdateDataFromPhysicsEngine()
		{
			if( ConstraintRigid != null && ConstraintRigid.RigidBodyA.IsActive )
			{
				ConstraintRigid.RigidBodyA.GetWorldTransform( out var bodyATransform );
				BulletPhysicsUtility.Convert( ref bodyATransform, out var bodyATransform2 );
				Matrix4.Multiply( ref bodyATransform2, ref constraintAFrame, out var transformA );
				//var transformA = BulletPhysicsUtility.Convert( ConstraintRigid.RigidBodyA.WorldTransform ) * constraintAFrame;
				//var transformA = BulletUtils.Convert( constraint.RigidBodyA.CenterOfMassTransform ) * BulletUtils.Convert( constraint.AFrame );

				transformA.Decompose( out Vector3 translation, out Matrix3 rotation, out Vector3 scale );
				var oldT = Transform.Value;
				var newT = new Transform( translation, rotation.ToQuaternion(), oldT.Scale );

				SetTransformWithoutUpdatingConstraint( newT );
			}

			//soft body mode
			if( created )
			{
				if( SoftBodyMode )
				{
					GetSoftBodyModeBodies( out var body1, out var body2 );
					if( body1 != null )
					{
						if( softAnchorModeClosestNodeIndex != -1 )
						{
							var pos = body1.GetNodePosition( softAnchorModeClosestNodeIndex );

							var oldT = Transform.Value;
							var newT = new Transform( pos, Quaternion.Identity, oldT.Scale );
							SetTransformWithoutUpdatingConstraint( newT );
						}
					}
				}
				else
				{
				}
			}
		}

		void UpdateLinearLimits( Generic6DofSpring2Constraint c )
		{
			Vector3 low = Vector3.Zero;
			Vector3 high = Vector3.Zero;

			//LinearAxisX
			if( LinearAxisX.Value == PhysicsAxisMode.Limited )
			{
				low.X = LinearAxisXLimitLow;
				high.X = LinearAxisXLimitHigh;
			}
			else if( LinearAxisX.Value == PhysicsAxisMode.Free )
			{
				low.X = 1;
				high.X = -1;
			}

			//LinearAxisY
			if( LinearAxisY.Value == PhysicsAxisMode.Limited )
			{
				low.Y = LinearAxisYLimitLow;
				high.Y = LinearAxisYLimitHigh;
			}
			else if( LinearAxisY.Value == PhysicsAxisMode.Free )
			{
				low.Y = 1;
				high.Y = -1;
			}

			//LinearAxisZ
			if( LinearAxisZ.Value == PhysicsAxisMode.Limited )
			{
				low.Z = LinearAxisZLimitLow;
				high.Z = LinearAxisZLimitHigh;
			}
			else if( LinearAxisZ.Value == PhysicsAxisMode.Free )
			{
				low.Z = 1;
				high.Z = -1;
			}

			c.LinearLowerLimit = BulletPhysicsUtility.Convert( low );
			c.LinearUpperLimit = BulletPhysicsUtility.Convert( high );
		}

		void UpdateLinearMotors( Generic6DofSpring2Constraint c )
		{
			c.EnableMotor( 0, LinearAxisX != PhysicsAxisMode.Locked && LinearAxisXMotor );
			c.EnableMotor( 1, LinearAxisY != PhysicsAxisMode.Locked && LinearAxisYMotor );
			c.EnableMotor( 2, LinearAxisZ != PhysicsAxisMode.Locked && LinearAxisZMotor );

			c.TranslationalLimitMotor.MaxMotorForce = new BulletSharp.Math.Vector3( LinearAxisXMotorMaxForce, LinearAxisYMotorMaxForce, LinearAxisZMotorMaxForce );
			c.TranslationalLimitMotor.TargetVelocity = new BulletSharp.Math.Vector3( LinearAxisXMotorTargetVelocity, LinearAxisYMotorTargetVelocity, LinearAxisZMotorTargetVelocity );
			c.TranslationalLimitMotor.Bounce = new BulletSharp.Math.Vector3( LinearAxisXMotorRestitution, LinearAxisYMotorRestitution, LinearAxisZMotorRestitution );

			c.SetServo( 0, LinearAxisXMotorServo );
			c.SetServo( 1, LinearAxisYMotorServo );
			c.SetServo( 2, LinearAxisZMotorServo );
			c.SetServoTarget( 0, LinearAxisXMotorServoTarget );
			c.SetServoTarget( 1, LinearAxisYMotorServoTarget );
			c.SetServoTarget( 2, LinearAxisZMotorServoTarget );
		}

		void UpdateLinearSprings( Generic6DofSpring2Constraint c )
		{
			c.EnableSpring( 0, LinearAxisX.Value != PhysicsAxisMode.Locked && LinearAxisXSpring );
			c.EnableSpring( 1, LinearAxisY.Value != PhysicsAxisMode.Locked && LinearAxisYSpring );
			c.EnableSpring( 2, LinearAxisZ.Value != PhysicsAxisMode.Locked && LinearAxisZSpring );

			c.SetStiffness( 0, LinearAxisXSpringStiffness );
			c.SetStiffness( 1, LinearAxisYSpringStiffness );
			c.SetStiffness( 2, LinearAxisZSpringStiffness );

			c.SetDamping( 0, LinearAxisXSpringDamping );
			c.SetDamping( 1, LinearAxisYSpringDamping );
			c.SetDamping( 2, LinearAxisZSpringDamping );
		}

		void UpdateAngularLimits( Generic6DofSpring2Constraint c )
		{
			Vector3 low = Vector3.Zero;
			Vector3 high = Vector3.Zero;

			//AngularAxisX
			if( AngularAxisX.Value == PhysicsAxisMode.Limited )
			{
				low.X = AngularAxisXLimitLow.Value.InRadians();
				high.X = AngularAxisXLimitHigh.Value.InRadians();
			}
			else if( AngularAxisX.Value == PhysicsAxisMode.Free )
			{
				low.X = 1;
				high.X = -1;
			}

			//AngularAxisY
			if( AngularAxisY.Value == PhysicsAxisMode.Limited )
			{
				low.Y = AngularAxisYLimitLow.Value.InRadians();
				high.Y = AngularAxisYLimitHigh.Value.InRadians();
			}
			else if( AngularAxisY.Value == PhysicsAxisMode.Free )
			{
				low.Y = 1;
				high.Y = -1;
			}

			//AngularAxisZ
			if( AngularAxisZ.Value == PhysicsAxisMode.Limited )
			{
				low.Z = AngularAxisZLimitLow.Value.InRadians();
				high.Z = AngularAxisZLimitHigh.Value.InRadians();
			}
			else if( AngularAxisZ.Value == PhysicsAxisMode.Free )
			{
				low.Z = 1;
				high.Z = -1;
			}

			c.AngularLowerLimit = BulletPhysicsUtility.Convert( low );
			c.AngularUpperLimit = BulletPhysicsUtility.Convert( high );

			//!!!!пока нет поддержки
			//c.SetBounce( 0, 1 );
			//c.SetBounce( 1, 1 );
			//c.SetBounce( 2, 1 );
		}

		void UpdateAngularMotors( Generic6DofSpring2Constraint c )
		{
			c.EnableMotor( 3, AngularAxisX != PhysicsAxisMode.Locked && AngularAxisXMotor );
			c.EnableMotor( 4, AngularAxisY != PhysicsAxisMode.Locked && AngularAxisYMotor );
			c.EnableMotor( 5, AngularAxisZ != PhysicsAxisMode.Locked && AngularAxisZMotor );

			c.GetRotationalLimitMotor( 0 ).Bounce = AngularAxisXMotorRestitution;
			c.GetRotationalLimitMotor( 0 ).TargetVelocity = AngularAxisXMotorTargetVelocity;
			c.GetRotationalLimitMotor( 0 ).MaxMotorForce = AngularAxisXMotorMaxForce;

			c.GetRotationalLimitMotor( 1 ).Bounce = AngularAxisYMotorRestitution;
			c.GetRotationalLimitMotor( 1 ).TargetVelocity = AngularAxisYMotorTargetVelocity;
			c.GetRotationalLimitMotor( 1 ).MaxMotorForce = AngularAxisYMotorMaxForce;

			c.GetRotationalLimitMotor( 2 ).Bounce = AngularAxisZMotorRestitution;
			c.GetRotationalLimitMotor( 2 ).TargetVelocity = AngularAxisZMotorTargetVelocity;
			c.GetRotationalLimitMotor( 2 ).MaxMotorForce = AngularAxisZMotorMaxForce;

			c.SetServo( 3, AngularAxisXServo );
			c.SetServo( 4, AngularAxisYServo );
			c.SetServo( 5, AngularAxisZServo );
			c.SetServoTarget( 3, AngularAxisXServoTarget.Value.InRadians() );
			c.SetServoTarget( 4, AngularAxisYServoTarget.Value.InRadians() );
			c.SetServoTarget( 5, AngularAxisZServoTarget.Value.InRadians() );
		}

		void UpdateAngularSprings( Generic6DofSpring2Constraint c )
		{
			c.EnableSpring( 3, AngularAxisX.Value != PhysicsAxisMode.Locked && AngularAxisXSpring );
			c.EnableSpring( 4, AngularAxisY.Value != PhysicsAxisMode.Locked && AngularAxisYSpring );
			c.EnableSpring( 5, AngularAxisZ.Value != PhysicsAxisMode.Locked && AngularAxisZSpring );

			c.SetStiffness( 3, AngularAxisXSpringStiffness );
			c.SetStiffness( 4, AngularAxisYSpringStiffness );
			c.SetStiffness( 5, AngularAxisZSpringStiffness );

			c.SetDamping( 3, AngularAxisXSpringDamping );
			c.SetDamping( 4, AngularAxisYSpringDamping );
			c.SetDamping( 5, AngularAxisZSpringDamping );
		}

		[Browsable( false )]
		bool SoftBodyMode
		{
			get { return creationBodyA?.BulletBody as SoftBody != null || creationBodyB?.BulletBody as SoftBody != null; }
		}

		bool AllLinear( PhysicsAxisMode mode )
		{
			return LinearAxisX.Value == mode && LinearAxisY.Value == mode && LinearAxisZ.Value == mode;
		}

		bool AllAngular( PhysicsAxisMode mode )
		{
			return AngularAxisX.Value == mode && AngularAxisY.Value == mode && AngularAxisZ.Value == mode;
		}

		[Browsable( false )]
		bool SoftNodeAnchorMode
		{
			get { return AllLinear( PhysicsAxisMode.Locked ) && AllAngular( PhysicsAxisMode.Locked ) && SoftBodyMode; }
		}

		////!!!!
		//void AppendLinearConstraint( Component_SoftBody body1, Component_PhysicalBody body2, Vec3 pos, double forceMixing = 1, double errorReduction = 1, double split = 1 )
		//{
		//	using( var specs = new LinearJoint.Specs() )
		//	{
		//		specs.Position = BulletUtils.ToVector3( pos );
		//		specs.ConstraintForceMixing = forceMixing;
		//		specs.ErrorReductionParameter = errorReduction;
		//		specs.Split = split;

		//		var b1 = (SoftBody)body1.BulletBody;
		//		if( body2 == null )
		//			b1.AppendLinearJoint( specs );
		//		else if( body2.BulletBody is SoftBody )
		//			b1.AppendLinearJoint( specs, (SoftBody)body2.BulletBody );
		//		else if( body2.BulletBody is RigidBody )
		//			b1.AppendLinearJoint( specs, new Body( body2.BulletBody ) );
		//	}
		//}

		////!!!!
		//void AppendAngularConstraint( Component_SoftBody body1, Component_PhysicalBody body2, Vec3 axis, double forceMixing = 1, double errorReduction = 1, double split = 1 )
		//{
		//	using( var specs = new AngularJoint.Specs() )
		//	{
		//		specs.Axis = BulletUtils.ToVector3( axis );
		//		specs.ConstraintForceMixing = forceMixing;
		//		specs.ErrorReductionParameter = errorReduction;
		//		specs.Split = split;

		//		var b1 = (SoftBody)body1.BulletBody;
		//		if( body2 == null )
		//			b1.AppendAngularJoint( specs );
		//		else if( body2.BulletBody is SoftBody )
		//			b1.AppendAngularJoint( specs, (SoftBody)body2.BulletBody );
		//		else if( body2.BulletBody is RigidBody )
		//			b1.AppendAngularJoint( specs, new Body( body2.BulletBody ) );
		//	}
		//}
	}
}
