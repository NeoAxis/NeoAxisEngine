// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Basic 2D character class.
	/// </summary>
	[AddToResourcesWindow( @"Base\2D\Character 2D", -7899 )]
	[NewObjectDefaultName( "Character 2D" )]
#if !DEPLOY
	[Editor.SettingsCell( typeof( Editor.Character2DSettingsCell ), true )]
#endif
	public class Character2D : Sprite, InteractiveObjectInterface
	{
		Character2DType typeCached = new Character2DType();
		RigidBody2D mainBody;

		//on ground and flying states
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double mainBodyGroundDistanceNoScale = 1000;//from center of the body/object
		RigidBody2D groundBody;
		Radian groundBodySlopeAngle;

		double forceIsOnGroundRemainingTime;
		double disableGravityRemainingTime;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double onGroundTime;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double elapsedTimeSinceLastGroundContact;
		//Vector3 lastSimulationStepPosition;

		//smooth?
		Vector2? lastTransformPosition;
		Vector2 lastLinearVelocity;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double jumpInactiveTime;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double jumpDisableRemainingTime;

		Vector2 lookToDirection;

		//moveVector
		int moveVectorTimer;//is disabled when equal 0
		double moveVector;
		bool moveVectorRun;
		double lastTickForceVector;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		//Vector2 linearVelocityForSerialization;

		Vector2 groundRelativeVelocity;
		Vector2[] groundRelativeVelocitySmoothArray;
		Vector2 groundRelativeVelocitySmooth;

		////damageFastChangeSpeed
		//Vector2 damageFastChangeSpeedLastVelocity = new Vector2( double.NaN, double.NaN );

		double allowToSleepTime;

		//crouching
		//const float crouchingVisualSwitchTime = .3f;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		bool crouching;
		//float crouchingSwitchRemainingTime;
		////[FieldSerialize( FieldSerializeSerializationTypes.World )]
		//float crouchingVisualFactor;

		Animation eventAnimation;
		Action eventAnimationUpdateMethod;

		//optimization
		SpriteAnimationController animationControllerCached;

		///////////////////////////////////////////////

		const string characterTypeDefault = @"Content\Characters 2D\Default\Default.character2dtype";

		[DefaultValueReference( characterTypeDefault )]
		public Reference<Character2DType> CharacterType
		{
			get { if( _characterType.BeginGet() ) CharacterType = _characterType.Get( this ); return _characterType.value; }
			set
			{
				if( _characterType.BeginSet( this, ref value ) )
				{
					try
					{
						CharacterTypeChanged?.Invoke( this );

						//update cached type
						typeCached = _characterType.value;
						if( typeCached == null )
							typeCached = new Character2DType();

						//update mesh
						if( EnabledInHierarchyAndIsInstance )
							UpdateMesh();
					}
					finally { _characterType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CharacterType"/> property value changes.</summary>
		public event Action<Character2D> CharacterTypeChanged;
		ReferenceField<Character2DType> _characterType = new Reference<Character2DType>( null, characterTypeDefault );

		[Browsable( false )]
		public Character2DType TypeCached
		{
			get { return typeCached; }
		}

		void UpdateMesh()
		{
			//reset animation
			var controller = GetAnimationController();
			if( controller != null )
			{
				controller.PlayAnimation = null;
				controller.Speed = 1;
			}

			TickAnimate( 0 );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			animationControllerCached = null;
			CharacterType.Touch();

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				//optimize networking
				{
					var controller = GetAnimationController();
					if( controller != null )
					{
						controller.NetworkDisablePropertySynchronization( "PlayAnimation" );
						controller.NetworkDisablePropertySynchronization( "Speed" );
						controller.NetworkDisablePropertySynchronization( "AutoRewind" );
						controller.NetworkDisablePropertySynchronization( "FreezeOnEnd" );
					}
				}

				UpdateMesh();

				var tr = GetTransform();
				SetLookToDirection( tr.Rotation.GetForward().ToVector2() );

				UpdateCollisionBody();

				//if( mainBody != null )
				//	mainBody.LinearVelocity = linearVelocityForSerialization;

				if( ParentScene != null )
					ParentScene.PhysicsSimulationStepAfter += ParentScene_PhysicsSimulationStepAfter;

				TickAnimate( 0 );
			}
			else
			{
				if( ParentScene != null )
					ParentScene.PhysicsSimulationStepAfter -= ParentScene_PhysicsSimulationStepAfter;

				mainBody = null;
			}
		}

		private void ParentScene_PhysicsSimulationStepAfter( Scene obj )
		{
			if( ParentScene == null )
				return;

			if( mainBody != null && mainBody.Active )
				UpdateRotation();
		}

		public Transform GetTransform()
		{
			if( mainBody != null )
				return mainBody.TransformV;
			else
				return TransformV;
		}

		public void SetTransform( Transform value )
		{
			var body = mainBody ?? GetCollisionBody();
			if( body != null && !body.Transform.ReferenceSpecified )
				body.Transform = value;

			if( !Transform.ReferenceSpecified )
				Transform = value;

			//if( mainBody != null )
			//	mainBody.Transform = value;
			//else
			//	Transform = value;
		}

		public RigidBody2D GetCollisionBody()
		{
			return GetComponent<RigidBody2D>( "Collision Body" );
		}

		public delegate void UpdateCollisionBodyEventDelegate( Character2D sender );
		public event UpdateCollisionBodyEventDelegate UpdateCollisionBodyEvent;

		public void UpdateCollisionBody()
		{
			if( NetworkIsClient )
				return;

			TypeCached.GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var fromPositionToFloorDistance );

			var body = GetCollisionBody();
			if( body == null )
			{
				body = CreateComponent<RigidBody2D>( enabled: false );
				body.Name = "Collision Body";
				body.Transform = TransformV.UpdateScale( new Vector3( TypeCached.InitialScale, TypeCached.InitialScale, TypeCached.InitialScale ) );
			}
			//else
			//	body.Enabled = false;
			mainBody = body;

			body.MotionType = RigidBody2D.MotionTypeEnum.Dynamic;
			body.CanBeSelected = false;

			//body.LinearSleepingThreshold = 0;
			//body.AngularSleepingThreshold = 0;

			body.AngularDamping = 10;
			body.Mass = TypeCached.Mass;
			body.FixedRotation = true;

			var length = height - TypeCached.Radius * 2;
			if( length < 0.01 )
				length = 0.01;

			var capsuleShape = body.GetComponent<CollisionShape2D_Capsule>();
			if( capsuleShape == null )
			{
				body.Enabled = false;
				capsuleShape = body.CreateComponent<CollisionShape2D_Capsule>();
				capsuleShape.Name = ComponentUtility.GetNewObjectUniqueName( capsuleShape );
			}
			capsuleShape.Height = length;
			capsuleShape.Radius = TypeCached.Radius;
			double offset = fromPositionToFloorDistance - height / 2;
			capsuleShape.TransformRelativeToParent = new Transform( new Vector3( 0, 0, -offset ), Quaternion.Identity );

			capsuleShape.Friction = 0;
			capsuleShape.Restitution = 0;
			////shape.SpecialLiquidDensity = 2000;
			////shape.ContactGroup = (int)ContactGroup.Dynamic;

			UpdateCollisionBodyEvent?.Invoke( this );

			body.Enabled = true;

			//set reference. lock Transform of the character to Transform of the body
			Transform = ReferenceUtility.MakeThisReference( this, body, "Transform" );
		}

		//void DestroyCollisionBody()
		//{
		//	if( NetworkIsClient )
		//		return;

		//	//ReleaseCollisionEvents();

		//	//reset Transform reference
		//	if( Transform.ReferenceSpecified )
		//		Transform = Transform.Value;

		//	var body = GetCollisionBody();
		//	if( body != null )
		//		body.Dispose();
		//	mainBody = null;
		//}

		public double GetScaleFactor()
		{
			//!!!!cache
			var result = GetTransform().Scale.MaxComponent();
			if( result == 0 )
				result = 0.0001;
			return result;
		}

		///////////////////////////////////////////

		public void SetMoveVector( double direction, bool run )
		{
			if( NetworkIsServer )
			{
				//one second
				moveVectorTimer = (int)( 1.0 / Time.SimulationDelta );
			}
			else
				moveVectorTimer = 2;

			moveVector = direction;
			moveVectorRun = run;
		}

		[Browsable( false )]
		public RigidBody2D MainBody
		{
			get { return mainBody; }
		}

		[Browsable( false )]
		public Vector2 LookToDirection
		{
			get { return lookToDirection; }
		}

		public void SetLookToDirection( Vector2 value )
		{
			lookToDirection = value;

			UpdateRotation();
		}

		public void UpdateRotation()
		{
			var rot = new Angles( 0, lookToDirection.X >= 0 ? 0 : -180, 0 ).ToQuaternion();
			var tr = GetTransform();

			if( !tr.Rotation.Equals( rot, 0.0001 ) )
			{
				//bool keepDisableControlPhysicsModelPushedToWorldFlag = DisableControlPhysicsModelPushedToWorldFlag;
				//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
				//   DisableControlPhysicsModelPushedToWorldFlag = true;

				SetTransform( tr.UpdateRotation( rot ) );

				//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
				//   DisableControlPhysicsModelPushedToWorldFlag = false;
			}
		}

		public bool IsOnGround()
		{
			if( jumpInactiveTime != 0 )
				return false;
			if( forceIsOnGroundRemainingTime > 0 )
				return true;

			double distanceFromPositionToFloor = crouching ? TypeCached.CrouchingPositionToGroundHeight : TypeCached.PositionToGroundHeight;
			const double maxThreshold = 0.2;
			return mainBodyGroundDistanceNoScale - maxThreshold < distanceFromPositionToFloor && groundBody != null;
		}

		public double GetElapsedTimeSinceLastGroundContact()
		{
			return elapsedTimeSinceLastGroundContact;
		}

		//protected override void OnSave( TextBlock block )
		//{
		//	if( mainBody != null )
		//		linearVelocityForSerialization = mainBody.LinearVelocity;

		//	base.OnSave( block );
		//}

		//protected override void OnSuspendPhysicsDuringMapLoading( bool suspend )
		//{
		//	base.OnSuspendPhysicsDuringMapLoading( suspend );

		//	//After loading a map, the physics simulate 5 seconds, that bodies have fallen asleep.
		//	//During this time we will disable physics for this entity.
		//		foreach( Body body in PhysicsModel.Bodies )
		//		{
		//			body.Static = suspend;
		//			if( !suspend )
		//				mainBody.Sleeping = false;
		//		}
		//}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//clear groundBody when disposed
			if( groundBody != null && groundBody.Disposed )
			{
				groundBody = null;
				groundBodySlopeAngle = 0;
			}

			if( mainBody != null )
			{
				TickMovement();
				TickPhysicsForce();
			}

			UpdateRotation();
			if( TypeCached.JumpSupport )
				TickJump( false );

			if( IsOnGround() )
				onGroundTime += Time.SimulationDelta;
			else
				onGroundTime = 0;
			if( !IsOnGround() )
				elapsedTimeSinceLastGroundContact += Time.SimulationDelta;
			else
				elapsedTimeSinceLastGroundContact = 0;
			CalculateGroundRelativeVelocity();

			if( moveVectorTimer != 0 )
				moveVectorTimer--;

			//if( CrouchingSupport )
			//	TickCrouching();

			//if( DamageFastChangeSpeedFactor != 0 )
			//	DamageFastChangeSpeedTick();

			var trPosition = GetTransform().Position;
			if( lastTransformPosition.HasValue )
				lastLinearVelocity = ( trPosition.ToVector2() - lastTransformPosition.Value ) / Time.SimulationDelta;
			else
				lastLinearVelocity = Vector2.Zero;
			lastTransformPosition = trPosition.ToVector2();

			//lastSimulationStepPosition = GetTransform().Position;

			if( forceIsOnGroundRemainingTime > 0 )
			{
				forceIsOnGroundRemainingTime -= Time.SimulationDelta;
				if( forceIsOnGroundRemainingTime < 0 )
					forceIsOnGroundRemainingTime = 0;
			}

			if( disableGravityRemainingTime > 0 )
			{
				disableGravityRemainingTime -= Time.SimulationDelta;
				if( disableGravityRemainingTime < 0 )
					disableGravityRemainingTime = 0;
			}
		}

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			if( mainBody != null && mainBody.Parent == null )
				mainBody = null;
			if( mainBody == null )
				mainBody = GetCollisionBody();

			//clear groundBody when disposed
			if( groundBody != null && groundBody.Disposed )
			{
				groundBody = null;
				groundBodySlopeAngle = 0;
			}

			if( mainBody != null )
				CalculateMainBodyGroundDistanceAndGroundBody();

			//UpdateRotation();
			if( TypeCached.JumpSupport )
				TickJumpClient( false );

			if( IsOnGround() )
				onGroundTime += Time.SimulationDelta;
			else
				onGroundTime = 0;
			if( !IsOnGround() )
				elapsedTimeSinceLastGroundContact += Time.SimulationDelta;
			else
				elapsedTimeSinceLastGroundContact = 0;
			CalculateGroundRelativeVelocity();

			////if( CrouchingSupport )
			////	TickCrouching();

			////if( DamageFastChangeSpeedFactor != 0 )
			////	DamageFastChangeSpeedTick();

			var trPosition = GetTransform().Position;
			if( lastTransformPosition.HasValue )
				lastLinearVelocity = ( trPosition.ToVector2() - lastTransformPosition.Value ) / Time.SimulationDelta;
			else
				lastLinearVelocity = Vector2.Zero;
			lastTransformPosition = trPosition.ToVector2();

			////lastSimulationStepPosition = GetTransform().Position;

			if( forceIsOnGroundRemainingTime > 0 )
			{
				forceIsOnGroundRemainingTime -= Time.SimulationDelta;
				if( forceIsOnGroundRemainingTime < 0 )
					forceIsOnGroundRemainingTime = 0;
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			TickAnimate( delta );
			UpdateEnabledItemTransform();

			//touch Transform to update character AABB
			if( EnabledInHierarchyAndIsInstance && VisibleInHierarchy )
			{
				Transform.Touch();
				//var t = Transform;
			}
		}

		public bool IsNeedRun()
		{
			//use specified force move vector
			if( moveVectorTimer != 0 )
				return moveVectorRun;

			return false;
		}

		double GetMoveVector()
		{
			//use specified move vector
			if( moveVectorTimer != 0 )
				return moveVector;

			return 0;
		}

		void TickPhysicsForce()
		{
			var forceVec = GetMoveVector();
			if( forceVec != 0 )
			{
				float speedMultiplier = 1;
				//if( FastMoveInfluence != null )
				//	speedCoefficient = FastMoveInfluence.Type.Coefficient;

				double maxSpeed = 0;
				double force = 0;

				if( IsOnGround() )
				{
					//calcualate maxSpeed and force on ground.

					Vector2 localVec = ( new Vector3( forceVec, 0, 0 ) * GetTransform().Rotation.GetInverse() ).ToVector2();

					var absSum = Math.Abs( localVec.X ) + Math.Abs( localVec.Y );
					if( absSum > 1 )
						localVec /= absSum;

					maxSpeed = 0;
					force = 0;

					if( !Crouching )
					{
						bool running = IsNeedRun();

						if( Math.Abs( localVec.X ) >= .001f )
						{
							//forward and backward
							double speedX = running ? TypeCached.RunForwardMaxSpeed : TypeCached.WalkForwardMaxSpeed;
							//if( localVec.X > 0 )
							//	speedX = running ? RunForwardMaxSpeed : WalkForwardMaxSpeed;
							//else
							//	speedX = running ? RunBackwardMaxSpeed : WalkBackwardMaxSpeed;
							maxSpeed += speedX * Math.Abs( localVec.X );
							force += ( running ? TypeCached.RunForce : TypeCached.WalkForce ) * Math.Abs( localVec.X );
						}
					}
					else
					{
						maxSpeed = TypeCached.CrouchingMaxSpeed;
						force = TypeCached.CrouchingForce;
					}
				}
				else
				{
					//calcualate maxSpeed and force when flying.
					if( TypeCached.FlyControlSupport )
					{
						maxSpeed = TypeCached.FlyControlMaxSpeed;
						force = TypeCached.FlyControlForce;
					}
				}

				if( force > 0 )
				{
					//speedCoefficient
					maxSpeed *= speedMultiplier;
					force *= speedMultiplier;

					var scaleFactor = GetScaleFactor();
					maxSpeed *= scaleFactor;
					force *= scaleFactor;

					if( Math.Abs( GetLinearVelocity().X ) < maxSpeed )
						mainBody.ApplyForce( new Vector2( forceVec, 0 ) * force * Time.SimulationDelta, Vector2.Zero );
				}
			}

			lastTickForceVector = forceVec;
		}

		void UpdateMainBodyDamping()
		{
			if( IsOnGround() && jumpInactiveTime == 0 )
			{
				if( allowToSleepTime > Time.SimulationDelta * 2.5f && GetMoveVector() == 0 )
					mainBody.LinearDamping = TypeCached.LinearDampingOnGroundIdle;
				else
					mainBody.LinearDamping = TypeCached.LinearDampingOnGroundMove;
			}
			else
				mainBody.LinearDamping = TypeCached.LinearDampingFly;
		}

		void TickMovement()
		{
			//wake up when ground is moving
			if( !mainBody.Active && groundBody != null && groundBody.Active && ( groundBody.LinearVelocity.Value.LengthSquared() > .3f || Math.Abs( groundBody.AngularVelocity.Value ) > .3f ) )
			{
				if( mainBody.Physics2DBody != null )
					mainBody.Physics2DBody.Awake = true;
			}

			CalculateMainBodyGroundDistanceAndGroundBody();// out var addForceOnBigSlope );

			if( mainBody.Active || !IsOnGround() )
			{
				UpdateMainBodyDamping();

				if( IsOnGround() )
				{
					//move the object when it underground
					if( lastTickForceVector != 0 && forceIsOnGroundRemainingTime == 0 )
					{
						var scaleFactor = GetScaleFactor();

						var maxSpeed = TypeCached.WalkForwardMaxSpeed;//Math.Min( WalkSideMaxSpeed, Math.Min( WalkForwardMaxSpeed, WalkBackwardMaxSpeed ) );
						Vector2 newPositionOffsetNoScale = new Vector2( ( lastTickForceVector > 0 ? 1 : -1 ) * maxSpeed * .15f, 0 );

						ClimbObstacleTest( newPositionOffsetNoScale, out var upHeightNoScale );

						//move object
						TypeCached.GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var fromPositionToFloorDistance );
						if( upHeightNoScale > TypeCached.Radius * 0.5 && upHeightNoScale <= walkUpHeight && jumpInactiveTime == 0 && walkUpHeight != 0 )//upHeight > .01f 
						{
							//bool keepDisableControlPhysicsModelPushedToWorldFlag = DisableControlPhysicsModelPushedToWorldFlag;
							//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
							//   DisableControlPhysicsModelPushedToWorldFlag = true;

							////additional offset
							//upHeight += Radius * 0.5;

							var tr = GetTransform();
							var newPosition = tr.Position + new Vector3( 0, upHeightNoScale * scaleFactor, 0 );

							if( IsFreePositionToMove( newPosition.ToVector2() ) )
							{
								mainBody.Transform = tr.UpdatePosition( newPosition );

								forceIsOnGroundRemainingTime = 0.2;
								disableGravityRemainingTime = 0.2;

								if( NetworkIsServer )
								{
									var writer = BeginNetworkMessageToEveryone( "TickMovement" );
									writer.Write( (float)forceIsOnGroundRemainingTime );
									EndNetworkMessage();
								}
							}

							//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
							//   DisableControlPhysicsModelPushedToWorldFlag = false;
						}
					}
				}

				////add force to body on big slope
				//if( addForceOnBigSlope != Vector3.Zero )
				//{
				//	mainBody.ApplyForce( addForceOnBigSlope * Time.SimulationDelta, Vector3.Zero );
				//	//mainBody.AddForce( ForceType.GlobalAtLocalPos, TickDelta, addForceOnBigSlope, Vector3.Zero );
				//}

				//on dynamic ground velocity
				if( IsOnGround() && groundBody != null && TypeCached.ApplyGroundVelocity )
				{
					if( groundBody.MotionType.Value != RigidBody2D.MotionTypeEnum.Static && groundBody.Active )
					{
						Vector2 groundVel = groundBody.LinearVelocity;

						Vector2 vel = mainBody.LinearVelocity;

						if( groundVel.X > 0 && vel.X >= 0 && vel.X < groundVel.X )
							vel.X = groundVel.X;
						else if( groundVel.X < 0 && vel.X <= 0 && vel.X > groundVel.X )
							vel.X = groundVel.X;

						if( groundVel.Y > 0 && vel.Y >= 0 && vel.Y < groundVel.Y )
							vel.Y = groundVel.Y;
						else if( groundVel.Y < 0 && vel.Y <= 0 && vel.Y > groundVel.Y )
							vel.Y = groundVel.Y;

						//simple anti-damping
						vel += groundVel * .25f;

						mainBody.LinearVelocity = vel;
					}
				}

				//sleep if on ground and zero velocity

				bool needSleep = false;
				if( IsOnGround() && groundBody != null )
				{
					bool groundStopped = !groundBody.Active ||
						( groundBody.LinearVelocity.Value.LengthSquared() < .3f && Math.Abs( groundBody.AngularVelocity.Value ) < .3f );
					if( groundStopped && Math.Abs( GetLinearVelocity().X ) < TypeCached.MinSpeedToSleepBody && GetMoveVector() == 0 )
						needSleep = true;
				}

				if( needSleep )
					allowToSleepTime += Time.SimulationDelta;
				else
					allowToSleepTime = 0;

				if( mainBody.Physics2DBody != null )
					mainBody.Physics2DBody.Awake = allowToSleepTime < Time.SimulationDelta * 2.5f;
			}

			mainBody.EnableGravity = disableGravityRemainingTime == 0;
		}

		bool VolumeCheckGetFirstNotFreePlace( ref Capsule2 sourceVolumeCapsule, Vector2 destVector, bool firstIteration, float step, out List<RigidBody2D> collisionBodies, out float collisionDistance, out bool collisionOnFirstCheck )
		{
			collisionBodies = new List<RigidBody2D>();
			collisionDistance = 0;
			collisionOnFirstCheck = false;

			bool firstCheck = true;

			var direction = destVector.GetNormalize();
			float totalDistance = (float)destVector.Length();
			int stepCount = (int)( (float)totalDistance / step ) + 2;
			Vector2 previousOffset = Vector2.Zero;

			for( int nStep = 0; nStep < stepCount; nStep++ )
			{
				float distance = (float)nStep * step;
				if( distance > totalDistance )
					distance = totalDistance;
				var offset = direction * distance;

				var scene = ParentScene;
				if( scene != null )
				{
					var capsule = sourceVolumeCapsule;
					CapsuleAddOffset( ref capsule, ref offset );

					//!!!!works bad, maybe need more exact checking
					////check by capsule
					//{
					//	var contactTestItem = new Physics2DContactTestItem( capsule, 32, Physics2DCategories.All, Physics2DCategories.All, 0, Physics2DContactTestItem.ModeEnum.All );
					//	ParentScene.Physics2DContactTest( contactTestItem );

					//	foreach( var item in contactTestItem.Result )
					//	{
					//		if( item.Body == mainBody )
					//			continue;
					//		var body = item.Body as RigidBody2D;
					//		if( body != null && !collisionBodies.Contains( body ) )
					//			collisionBodies.Add( body );
					//	}
					//}

					//!!!!works bad too
					//check by box at bottom
					{
						var rectangle = new Rectangle( capsule.GetCenter() );
						rectangle.Add( capsule.Point1 - new Vector2( 0, TypeCached.Radius ) );
						rectangle.Expand( new Vector2( TypeCached.Radius, 0 ) );

						if( rectangle.Size.Y > 0 )
						{
							var contactTestItem = new Physics2DContactTestItem( rectangle, Physics2DCategories.All, Physics2DCategories.All, 0, Physics2DContactTestItem.ModeEnum.All );
							ParentScene.Physics2DContactTest( contactTestItem );

							foreach( var item in contactTestItem.Result )
							{
								if( item.Body == mainBody )
									continue;
								var body = item.Body as RigidBody2D;
								if( body != null && !collisionBodies.Contains( body ) )
									collisionBodies.Add( body );
							}
						}

						//var cylinder = new Cylinder( capsule.GetCenter(), capsule.Point1 - new Vector3( 0, 0, Radius ), Radius );
						//if( cylinder.Point1 != cylinder.Point2 )
						//{
						//	var contactTestItem = new PhysicsContactTestItem( 1, -1, PhysicsContactTestItem.ModeEnum.All, cylinder );
						//	ParentScene.PhysicsContactTest( contactTestItem );

						//	foreach( var item in contactTestItem.Result )
						//	{
						//		if( item.Body == mainBody )
						//			continue;
						//		var body = item.Body as RigidBody;
						//		if( body != null && !collisionBodies.Contains( body ) )
						//			collisionBodies.Add( body );
						//	}
						//}
					}
				}

				if( collisionBodies.Count != 0 )
				{
					//second iteration
					if( nStep != 0 && firstIteration )
					{
						float step2 = step / 10;
						var sourceVolumeCapsules2 = sourceVolumeCapsule;
						CapsuleAddOffset( ref sourceVolumeCapsule, ref previousOffset );
						var destVector2 = offset - previousOffset;

						if( VolumeCheckGetFirstNotFreePlace( ref sourceVolumeCapsules2, destVector2, false, step2, out var collisionBodies2, out var collisionDistance2, out var collisionOnFirstCheck2 ) )
						{
							collisionBodies = collisionBodies2;
							collisionDistance = ( previousOffset != Vector2.Zero ? (float)previousOffset.Length() : 0 ) + collisionDistance2;
							collisionOnFirstCheck = false;
							return true;
						}
					}

					collisionDistance = distance;
					collisionOnFirstCheck = firstCheck;
					return true;
				}

				firstCheck = false;
				previousOffset = offset;
			}

			return false;
		}

		//List<Ray2> debugRays = new List<Ray2>();

		void CalculateMainBodyGroundDistanceAndGroundBody()// out Vector2 addForceOnBigSlope )
		{
			//addForceOnBigSlope = Vector2.Zero;

			//debugRays.Clear();

			TypeCached.GetBodyFormInfo( crouching, out var height, out _, out var fromPositionToFloorDistance );

			GetVolumeCapsule( out var capsule );
			//make radius smaller
			capsule.Radius *= .99f;

			mainBodyGroundDistanceNoScale = 1000;
			groundBody = null;
			groundBodySlopeAngle = 0;

			var scene = ParentScene;
			if( scene != null )
			{
				var scaleFactor = GetScaleFactor();

				//1. get collision bodies
				List<RigidBody2D> collisionBodies;
				float collisionOffset;
				{
					Vector2 destVector = new Vector2( 0, -height * scaleFactor );
					//Vector2 destVector = new Vector2( 0, -height * 1.5f * scaleFactor );
					var step = (float)( TypeCached.Radius.Value / 2 * scaleFactor );
					VolumeCheckGetFirstNotFreePlace( ref capsule, destVector, true, step, out collisionBodies, out collisionOffset, out _ );
				}

				//2. check slope angle
				if( collisionBodies.Count != 0 )
				{
					var rayCenter = capsule.Point1 - new Vector2( 0, collisionOffset );

					RigidBody2D foundBodyWithGoodAngle = null;
					double foundBodyWithGoodAngleSlopAngle = 0;
					//Vector2 bigSlopeVector = Vector2.Zero;

					const int stepCount = 8;

					for( int nStep = 0; nStep < stepCount; nStep++ )
					{
						var factor = (float)nStep / (float)( stepCount - 1 );
						var angle = MathEx.Lerp( -Math.PI / 2, Math.PI / 2, factor ) * 0.8;
						var vector = new Vector2( Math.Sin( angle ), -Math.Cos( angle ) );
						var ray = new Ray2( rayCenter, vector * TypeCached.Radius * 1.4f * scaleFactor );// * 1.1f );

						//debugRays.Add( ray );

						var item = new Physics2DRayTestItem( ray, Physics2DCategories.All, Physics2DCategories.All, 0, Physics2DRayTestItem.ModeEnum.All );

						scene.Physics2DRayTest( item );

						if( item.Result.Length != 0 )
						{
							foreach( var result in item.Result )
							{
								var rigidBody = result.Body as RigidBody2D;
								if( rigidBody != null && result.Body != mainBody )
								{
									//check slope angle
									var slope = MathAlgorithms.GetVectorsAngle( result.Normal, Vector2.YAxis );
									if( slope < TypeCached.MaxSlopeAngle.Value.InRadians() )
									{
										if( foundBodyWithGoodAngle == null || slope < foundBodyWithGoodAngleSlopAngle )
										{
											foundBodyWithGoodAngle = rigidBody;
											foundBodyWithGoodAngleSlopAngle = slope;
										}
									}
									//else
									//{
									//	Vector2 vector = new Vector3( result.Normal.X, result.Normal.Y, 0 );
									//	if( vector != Vector2.Zero )
									//		bigSlopeVector += vector;
									//}
								}
							}
						}
					}


					//foundBodyWithGoodAngle = collisionBodies[ 0 ];
					//foundBodyWithGoodAngleSlopAngle = 0;


					if( foundBodyWithGoodAngle != null )
					{
						groundBody = foundBodyWithGoodAngle;
						groundBodySlopeAngle = foundBodyWithGoodAngleSlopAngle;
						mainBodyGroundDistanceNoScale = fromPositionToFloorDistance + collisionOffset / scaleFactor;
					}
					//else
					//{
					//	if( bigSlopeVector != Vector2.Zero )
					//	{
					//		//add force
					//		bigSlopeVector.Normalize();
					//		bigSlopeVector *= mainBody.Mass * 2;
					//		addForceOnBigSlope = bigSlopeVector;
					//	}
					//}
				}
			}

			//Log.Info( "On ground: " + IsOnGround().ToString() + " angle: " + groundBodySlopeAngle.InDegrees().ToString() );
		}

		Capsule2 GetWorldCapsule( CollisionShape2D_Capsule shape )
		{
			var capsule = new Capsule2();

			var bodyTransform = mainBody.TransformV;
			var pos = ( bodyTransform.Position + shape.TransformRelativeToParent.Value.Position ).ToVector2();
			var rot = bodyTransform.Rotation;
			var scl = bodyTransform.Scale.MaxComponent();

			var diff = ( rot * new Vector3( 0, shape.Height * 0.5, 0 ) ).ToVector2();
			if( diff.Y > 0 )
			{
				capsule.Point1 = pos - diff * scl;
				capsule.Point2 = pos + diff * scl;
			}
			else
			{
				capsule.Point1 = pos + diff * scl;
				capsule.Point2 = pos - diff * scl;
			}
			capsule.Radius = shape.Radius * scl;

			return capsule;
		}

		void GetVolumeCapsule( out Capsule2 volumeCapsule )
		{
			var capsuleShape = mainBody.GetComponent<CollisionShape2D_Capsule>();
			if( capsuleShape != null )
				volumeCapsule = GetWorldCapsule( capsuleShape );
			else
				volumeCapsule = new Capsule2( mainBody.TransformV.Position.ToVector2(), mainBody.TransformV.Position.ToVector2(), 0.1 );
		}

		void CapsuleAddOffset( ref Capsule2 capsule, ref Vector2 offset )
		{
			capsule.Point1 += offset;
			capsule.Point2 += offset;
		}

		void ClimbObstacleTest( Vector2 newPositionOffsetNoScale, out double upHeightNoScale )
		{
			TypeCached.GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var fromPositionToFloorDistance );

			var scaleFactor = GetScaleFactor();

			GetVolumeCapsule( out var capsule );
			{
				Vector2 offset = ( newPositionOffsetNoScale + new Vector2( 0, walkUpHeight ) ) * scaleFactor;
				CapsuleAddOffset( ref capsule, ref offset );
			}

			Vector2 destVector = new Vector2( 0, -walkUpHeight );
			var step = (float)TypeCached.Radius / 2;
			List<RigidBody2D> collisionBodies;
			float collisionDistance;
			bool collisionOnFirstCheck;
			bool foundCollision = VolumeCheckGetFirstNotFreePlace( ref capsule, destVector, true, step, out collisionBodies,
				out collisionDistance, out collisionOnFirstCheck );
			if( foundCollision )
			{
				if( collisionOnFirstCheck )
					upHeightNoScale = float.MaxValue;
				else
					upHeightNoScale = ( walkUpHeight - collisionDistance ) / scaleFactor;
			}
			else
				upHeightNoScale = 0;
		}

		protected virtual void OnJump()
		{
		}

		void TickJump( bool ignoreTicks )
		{
			if( !ignoreTicks )
			{
				if( jumpDisableRemainingTime != 0 )
				{
					jumpDisableRemainingTime -= Time.SimulationDelta;
					if( jumpDisableRemainingTime < 0 )
						jumpDisableRemainingTime = 0;
				}

				if( jumpInactiveTime != 0 )
				{
					jumpInactiveTime -= Time.SimulationDelta;
					if( jumpInactiveTime < 0 )
						jumpInactiveTime = 0;
				}
			}

			if( IsOnGround() && onGroundTime > Time.SimulationDelta && jumpInactiveTime == 0 && jumpDisableRemainingTime != 0 )
			{
				Vector2 vel = mainBody.LinearVelocity;
				vel.Y = TypeCached.JumpSpeed * GetScaleFactor();
				mainBody.LinearVelocity = vel;

				//bool keepDisableControlPhysicsModelPushedToWorldFlag = DisableControlPhysicsModelPushedToWorldFlag;
				//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
				//   DisableControlPhysicsModelPushedToWorldFlag = true;

				var tr = GetTransform();
				mainBody.Transform = tr.UpdatePosition( tr.Position + new Vector3( 0, 0.05, 0 ) );

				//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
				//   DisableControlPhysicsModelPushedToWorldFlag = false;

				jumpInactiveTime = 0.2;
				jumpDisableRemainingTime = 0;

				UpdateMainBodyDamping();

				SoundPlay( TypeCached.JumpSound );

				if( TypeCached.JumpAnimation.Value != null )
				{
					EventAnimationBegin( TypeCached.JumpAnimation, delegate ()
					{
						if( IsOnGround() )
							EventAnimationBegin( null );
					} );
				}

				if( NetworkIsServer && ( TypeCached.JumpSound.ReferenceOrValueSpecified || TypeCached.JumpAnimation.ReferenceOrValueSpecified ) )
				{
					BeginNetworkMessageToEveryone( "Jump" );
					EndNetworkMessage();
				}

				OnJump();
			}
		}

		void TickJumpClient( bool ignoreTicks )
		{
			if( !ignoreTicks )
			{
				if( jumpDisableRemainingTime != 0 )
				{
					jumpDisableRemainingTime -= Time.SimulationDelta;
					if( jumpDisableRemainingTime < 0 )
						jumpDisableRemainingTime = 0;
				}

				if( jumpInactiveTime != 0 )
				{
					jumpInactiveTime -= Time.SimulationDelta;
					if( jumpInactiveTime < 0 )
						jumpInactiveTime = 0;
				}
			}
		}

		public void Jump()
		{
			if( !TypeCached.JumpSupport )
				return;
			if( Crouching )
				return;

			jumpDisableRemainingTime = 0.4;
			TickJump( true );
		}

		public void JumpClient()
		{
			if( !TypeCached.JumpSupport )
				return;
			if( Crouching )
				return;

			BeginNetworkMessageToServer( "Jump" );
			EndNetworkMessage();
		}

		[Browsable( false )]
		public double LastTickForceVector
		{
			get { return lastTickForceVector; }
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			if( mainBody != null )
			{
				GetBox( out var box );
				box.ToBounds( out var bounds );
				newBounds = SpaceBounds.Merge( newBounds, new SpaceBounds( ref bounds ) );
			}
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			context.thisObjectWasChecked = true;
			if( SpaceBounds.BoundingBox.Intersects( context.ray, out var scale ) )
				context.thisObjectResultRayScale = scale;
		}

		void CalculateGroundRelativeVelocity()
		{
			if( mainBody != null )
			{
				groundRelativeVelocity = GetLinearVelocity();
				if( groundBody != null && Math.Abs( groundBody.AngularVelocity.Value ) < .3f )
					groundRelativeVelocity -= groundBody.LinearVelocity;
			}
			else
				groundRelativeVelocity = Vector2.Zero;

			//groundRelativeVelocityToSmooth
			if( groundRelativeVelocitySmoothArray == null )
			{
				var seconds = .2f;
				var count = ( seconds / Time.SimulationDelta ) + .999f;
				groundRelativeVelocitySmoothArray = new Vector2[ (int)count ];
			}
			for( int n = 0; n < groundRelativeVelocitySmoothArray.Length - 1; n++ )
				groundRelativeVelocitySmoothArray[ n ] = groundRelativeVelocitySmoothArray[ n + 1 ];
			groundRelativeVelocitySmoothArray[ groundRelativeVelocitySmoothArray.Length - 1 ] = groundRelativeVelocity;
			groundRelativeVelocitySmooth = Vector2.Zero;
			for( int n = 0; n < groundRelativeVelocitySmoothArray.Length; n++ )
				groundRelativeVelocitySmooth += groundRelativeVelocitySmoothArray[ n ];
			groundRelativeVelocitySmooth /= (float)groundRelativeVelocitySmoothArray.Length;
		}

		[Browsable( false )]
		public Vector2 GroundRelativeVelocity
		{
			get { return groundRelativeVelocity; }
		}

		[Browsable( false )]
		public Vector2 GroundRelativeVelocitySmooth
		{
			get { return groundRelativeVelocitySmooth; }
		}

		public Vector2 GetLinearVelocity()
		{
			return lastLinearVelocity;
			//if( EngineApp.IsSimulation )
			//	return ( GetTransform().Position - lastSimulationStepPosition ) / Time.SimulationDelta;
			//return Vector3.Zero;
		}

		//public void DamageFastChangeSpeedResetLastVelocity()
		//{
		//	damageFastChangeSpeedLastVelocity = new Vector3( float.NaN, float.NaN, float.NaN );
		//}

		//void DamageFastChangeSpeedTick()
		//{
		//	if( MainBody == null )
		//		return;
		//	Vector3 velocity = MainBody.LinearVelocity;

		//	if( float.IsNaN( damageFastChangeSpeedLastVelocity.X ) )
		//		damageFastChangeSpeedLastVelocity = velocity;

		//	Vector3 diff = velocity - damageFastChangeSpeedLastVelocity;
		//	if( diff.Z > 0 )
		//	{
		//		float v = diff.Z - Type.DamageFastChangeSpeedMinimalSpeed;
		//		if( v > 0 )
		//		{
		//			float damage = v * Type.DamageFastChangeSpeedFactor;
		//			if( damage > 0 )
		//				DoDamage( null, Position, null, damage, true );
		//		}
		//	}

		//	damageFastChangeSpeedLastVelocity = velocity;
		//}

		[Browsable( false )]
		public bool Crouching
		{
			get { return crouching; }
		}

		//void ReCreateMainBody()
		//{
		//	Vector3 oldLinearVelocity = Vector3.Zero;
		//	float oldLinearDamping = 0;
		//	if( mainBody != null )
		//	{
		//		oldLinearVelocity = mainBody.LinearVelocity;
		//		oldLinearDamping = mainBody.LinearDamping;
		//	}

		//	CreateMainBody();

		//	if( mainBody != null )
		//	{
		//		mainBody.LinearVelocity = oldLinearVelocity;
		//		mainBody.LinearDamping = oldLinearDamping;
		//	}
		//}

		//void TickCrouching()
		//{
		//	if( crouchingSwitchRemainingTime > 0 )
		//	{
		//		crouchingSwitchRemainingTime -= TickDelta;
		//		if( crouchingSwitchRemainingTime < 0 )
		//			crouchingSwitchRemainingTime = 0;
		//	}

		//	if( Intellect != null && crouchingSwitchRemainingTime == 0 )
		//	{
		//		bool needCrouching = Intellect.IsControlKeyPressed( GameControlKeys.Crouching );

		//		if( crouching != needCrouching )
		//		{
		//			Vector3 newPosition;
		//			{
		//				float diff = Type.HeightFromPositionToGround - Type.CrouchingHeightFromPositionToGround;
		//				if( needCrouching )
		//					newPosition = Position + new Vector3( 0, 0, -diff );
		//				else
		//					newPosition = Position + new Vector3( 0, 0, diff );
		//			}

		//			bool freePlace = true;
		//			{
		//				Capsule capsule;
		//				{
		//					float radius = Type.Radius - .01f;

		//					float length;
		//					if( needCrouching )
		//						length = Type.CrouchingHeight - radius * 2 - Type.CrouchingWalkUpHeight;
		//					else
		//						length = Type.Height - radius * 2 - Type.WalkUpHeight;

		//					capsule = new Capsule(
		//						newPosition + new Vector3( 0, 0, -length / 2 ),
		//						newPosition + new Vector3( 0, 0, length / 2 ), radius );
		//				}

		//				Body[] bodies = PhysicsWorld.Instance.VolumeCast( capsule, (int)ContactGroup.CastOnlyContact );
		//				foreach( Body body in bodies )
		//				{
		//					if( body == mainBody )
		//						continue;

		//					freePlace = false;
		//					break;
		//				}
		//			}

		//			if( freePlace )
		//			{
		//				crouching = needCrouching;
		//				crouchingSwitchRemainingTime = .3f;

		//				ReCreateMainBody();

		//				Position = newPosition;
		//				OldPosition = Position;

		//				Vector3 addForceOnBigSlope;
		//				CalculateMainBodyGroundDistanceAndGroundBody( out addForceOnBigSlope );
		//			}
		//		}
		//	}
		//}

		void GetBox( out Box box )
		{
			var tr = TransformV;
			GetVolumeCapsule( out var capsule );
			var extents = new Vector3( capsule.Radius, ( capsule.Point2 - capsule.Point1 ).Length() * 0.5 + capsule.Radius, tr.Scale.Z );
			box = new Box( new Vector3( capsule.GetCenter(), tr.Position.Z ), extents, tr.Rotation.ToMatrix3() );
		}

		//void DebugDraw( Viewport viewport )
		//{
		//	var renderer = viewport.Simple3DRenderer;
		//	GetBox( out var box );
		//	var points = box.ToPoints();

		//	renderer.AddArrow( points[ 0 ], points[ 1 ], 0, 0, true, 0 );
		//	renderer.AddLine( points[ 1 ], points[ 2 ], -1 );
		//	renderer.AddArrow( points[ 3 ], points[ 2 ], 0, 0, true, 0 );
		//	renderer.AddLine( points[ 3 ], points[ 0 ], -1 );

		//	renderer.AddArrow( points[ 4 ], points[ 5 ], 0, 0, true, 0 );
		//	renderer.AddLine( points[ 5 ], points[ 6 ], -1 );
		//	renderer.AddArrow( points[ 7 ], points[ 6 ], 0, 0, true, 0 );
		//	renderer.AddLine( points[ 7 ], points[ 4 ], -1 );

		//	renderer.AddLine( points[ 0 ], points[ 4 ], -1 );
		//	renderer.AddLine( points[ 1 ], points[ 5 ], -1 );
		//	renderer.AddLine( points[ 2 ], points[ 6 ], -1 );
		//	renderer.AddLine( points[ 3 ], points[ 7 ], -1 );
		//}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;
				var scene = context.Owner.AttachedScene;

				if( scene != null && context.SceneDisplayDevelopmentDataInThisApplication && scene.DisplayPhysicalObjects )
				{
					TypeCached.GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var fromPositionToFloorDistance );

					var renderer = context.Owner.Simple3DRenderer;
					var tr = GetTransform();
					var scaleFactor = tr.Scale.MaxComponent();

					renderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );

					//unit center
					renderer.AddSphere( new Sphere( tr.Position, .05f ), 16 );

					//ground height
					renderer.AddSphere( new Sphere( tr.Position - new Vector3( 0, fromPositionToFloorDistance * scaleFactor, 0 ), .05f ), 16 );

					//stand up height
					{
						Vector3 pos = tr.Position - new Vector3( 0, ( fromPositionToFloorDistance - walkUpHeight ) * scaleFactor, 0 );
						renderer.AddLine( pos + new Vector3( .2f, 0, 0 ), pos - new Vector3( .2f, 0, 0 ) );
						renderer.AddLine( pos + new Vector3( 0, .2f, 0 ), pos - new Vector3( 0, .2f, 0 ) );
					}

					//eye position
					renderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );
					renderer.AddSphere( new Sphere( TransformV * new Vector3( TypeCached.EyePosition.Value, 0 ), .05f ), 16 );
				}

				var showLabels = /*show &&*/ mainBody == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;

				//#if !DEPLOY
				//				//draw selection
				//				if( mainBody != null )
				//				{
				//					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
				//					{
				//						ColorValue color;
				//						if( context2.selectedObjects.Contains( this ) )
				//							color = ProjectSettings.Get.Colors.SelectedColor;
				//						else //if( context2.canSelectObjects.Contains( this ) )
				//							color = ProjectSettings.Get.Colors.CanSelectColor;
				//						//else
				//						//	color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;

				//						var viewport = context.Owner;

				//						viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
				//						DebugDraw( viewport );
				//					}
				//				}
				//#endif


				//foreach( var ray in debugRays )
				//{
				//	var renderer = context.Owner.Simple3DRenderer;
				//	renderer.SetColor( new ColorValue( 1, 0, 0 ) );
				//	renderer.AddArrow( new Vector3( ray.Origin, 2 ), new Vector3( ray.GetEndPoint(), 2 ), 0, 0, true, 0 );
				//}
			}

			//protected override void OnRenderFrame()
			//
			//if( ( crouching && crouchingVisualFactor < 1 ) || ( !crouching && crouchingVisualFactor > 0 ) )
			//{
			//	float delta = RendererWorld.Instance.FrameRenderTimeStep / crouchingVisualSwitchTime;
			//	if( crouching )
			//	{
			//		crouchingVisualFactor += delta;
			//		if( crouchingVisualFactor > 1 )
			//			crouchingVisualFactor = 1;
			//	}
			//	else
			//	{
			//		crouchingVisualFactor -= delta;
			//		if( crouchingVisualFactor < 0 )
			//			crouchingVisualFactor = 0;
			//	}
			//}
		}

		public SpriteAnimationController GetAnimationController()
		{
			if( animationControllerCached == null )
			{
				animationControllerCached = GetComponent<SpriteAnimationController>();
				if( animationControllerCached == null && !NetworkIsClient )
				{
					animationControllerCached = CreateComponent<SpriteAnimationController>();
					animationControllerCached.Name = "Animation Controller";
				}
			}
			return animationControllerCached;
		}

		void TickAnimate( float delta )
		{
			//if( TypeCached.Animate )
			//{
			var controller = GetAnimationController();
			if( controller != null )
			{
				//update event animation
				if( eventAnimation != null )
					eventAnimationUpdateMethod?.Invoke();
				if( eventAnimation != null )
				{
					var current = controller.PlayAnimation.Value;
					if( current == eventAnimation && controller.CurrentAnimationTime == eventAnimation.Length )
						EventAnimationEnd();
				}

				//update controller

				Animation animation = null;
				bool autoRewind = true;
				double speed = 1;

				//event animation
				if( eventAnimation != null )
				{
					animation = eventAnimation;
					autoRewind = false;
				}

				//current state animation
				if( animation == null && EngineApp.IsSimulation )
				{
					if( IsOnGround() )
					{
						var linearSpeedNoScale = Math.Abs( GetLinearVelocity().X ) / GetScaleFactor();

						//RunAnimation
						if( TypeCached.RunSupport )
						{
							var running = Math.Abs( linearSpeedNoScale ) > TypeCached.RunForwardMaxSpeed * 0.8;
							if( running )
							{
								animation = TypeCached.RunAnimation;
								autoRewind = true;
								if( animation != null )
									speed = TypeCached.RunAnimationSpeed * linearSpeedNoScale;
							}
						}

						//WalkAnimation
						if( animation == null )
						{
							var walking = Math.Abs( linearSpeedNoScale ) > TypeCached.WalkForwardMaxSpeed * 0.2;
							if( walking )
							{
								animation = TypeCached.WalkAnimation;
								autoRewind = true;
								if( animation != null )
									speed = TypeCached.WalkAnimationSpeed * linearSpeedNoScale;
							}
						}
					}
					else
					{
						animation = TypeCached.FlyAnimation;
						autoRewind = true;
					}
				}

				if( animation == null )
				{
					animation = TypeCached.IdleAnimation;
					autoRewind = true;
				}

				//update controller
				controller.PlayAnimation = animation;
				controller.AutoRewind = autoRewind;
				controller.Speed = speed;
			}
			//}
		}

		//public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		//{
		//if( Components.Count == 0 )
		//{
		//	var controller = CreateComponent<SpriteAnimationController>();
		//	controller.Name = "Animation Controller";


		//Height = 1.2;
		//PositionToGroundHeight = 0.75;
		//EyePosition = new Vector2( 0.05, 0.2 );

		//RunSupport = true;
		//FlyControlSupport = true;
		//JumpSupport = true;

		//Animate = true;
		//IdleAnimation = ReferenceUtility.MakeReference( @"Content\2D\Penguin\Animation Idle.component" );
		//WalkAnimation = ReferenceUtility.MakeReference( @"Content\2D\Penguin\Animation Walk.component" );
		////RunAnimation = ReferenceUtility.MakeReference( @"Content\2D\Penguin\Animation Run.component" );
		//FlyAnimation = ReferenceUtility.MakeReference( @"Content\2D\Penguin\Animation Fly.component" );
		//JumpAnimation = ReferenceUtility.MakeReference( @"Content\2D\Penguin\Animation Jump.component" );
		//}
		//}

		public bool IsFreePositionToMove( Vector2 position )
		{
			GetVolumeCapsule( out var capsule );
			//make radius smaller
			capsule.Radius *= .99f;

			var offset = position - capsule.GetCenter();
			var checkCapsule = capsule;
			CapsuleAddOffset( ref checkCapsule, ref offset );

			var scene = ParentScene;
			if( scene != null )
			{
				var contactTestItem = new Physics2DContactTestItem( checkCapsule, 32, Physics2DCategories.All, Physics2DCategories.All, 0, Physics2DContactTestItem.ModeEnum.All );
				ParentScene.Physics2DContactTest( contactTestItem );

				foreach( var item in contactTestItem.Result )
				{
					if( item.Body == mainBody )
						continue;
					var body = item.Body as RigidBody2D;
					if( body != null )
					{
						return false;
						//collisionBodies.Add( body );
					}
				}
			}

			return true;
		}

		[Browsable( false )]
		public Animation EventAnimation
		{
			get { return eventAnimation; }
		}

		[Browsable( false )]
		public Action EventAnimationUpdateMethod
		{
			get { return eventAnimationUpdateMethod; }
		}

		public void EventAnimationBegin( Animation animation, Action updateMethod = null )
		{
			eventAnimation = animation;
			eventAnimationUpdateMethod = updateMethod;
		}

		public void EventAnimationEnd()
		{
			eventAnimation = null;
			eventAnimationUpdateMethod = null;
		}

		[Browsable( false )]
		public RigidBody2D GroundBody
		{
			get { return groundBody; }
		}

		[Browsable( false )]
		public double GroundBodySlopeAngle
		{
			get { return groundBodySlopeAngle; }
		}

		/////////////////////////////////////////

		public ItemInterface[] GetAllItems()
		{
			return GetComponents<ItemInterface>();
		}

		public ItemInterface GetItemByType( ItemTypeInterface type )
		{
			if( type != null )
			{
				foreach( var c in GetComponents<ItemInterface>() )
				{
					var item = c as Item2D;
					if( item != null )
					{
						if( item.ItemType.Value == type )
							return c;
					}

					var weapon = c as Weapon2D;
					if( weapon != null )
					{
						if( weapon.WeaponType.Value == type )
							return c;
					}
				}
			}
			return null;
		}

		public ItemInterface GetItemByResourceName( string resourceName )
		{
			foreach( var c in GetComponents<ItemInterface>() )
			{
				var item = c as Item2D;
				if( item != null )
				{
					var itemType = item.ItemType.Value;
					if( itemType != null )
					{
						var resource = ComponentUtility.GetResourceInstanceByComponent( itemType );
						if( resource != null && resource.Owner.Name == resourceName )
							return c;
					}
				}

				var weapon = c as Weapon2D;
				if( weapon != null )
				{
					var itemType = weapon.WeaponType.Value;
					if( itemType != null )
					{
						var resource = ComponentUtility.GetResourceInstanceByComponent( itemType );
						if( resource != null && resource.Owner.Name == resourceName )
							return c;
					}
				}

			}
			return null;
		}

		public ItemInterface GetActiveItem()
		{
			foreach( var item in GetAllItems() )
			{
				if( item.Enabled )
					return item;
			}
			return null;
		}

		public Weapon2D GetActiveWeapon()
		{
			return GetActiveItem() as Weapon2D;
		}

		public bool ItemCanTake( GameMode gameMode, ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			//check already taken
			if( item2.Parent == this )
				return false;

			//disable taking from another owner
			if( item2.Parent as ObjectInSpace != null )
				return false;

			//InventoryCharacterCanHaveSeveralWeapons, InventoryCharacterCanHaveSeveralWeaponsOfSameType
			var weapon = item2 as Weapon2D;
			if( weapon != null )
			{
				if( !gameMode.InventoryCharacterCanHaveSeveralWeapons && GetComponent<Weapon2D>() != null )
					return false;
				if( !gameMode.InventoryCharacterCanHaveSeveralWeaponsOfSameType && GetItemByType( weapon.WeaponType.Value ) != null )
					return false;
			}

			//!!!!need check by distance and do other checks. can be done in GameMode.ItemTakeEvent

			var allowAction = true;
			gameMode.PerformItemCanTakeEvent( this, item, ref allowAction );
			if( !allowAction )
				return false;

			return true;
		}

		/// <summary>
		/// Takes the item. The item will moved to the character and will disabled.
		/// </summary>
		/// <param name="item"></param>
		public bool ItemTake( GameMode gameMode, ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			if( gameMode != null )
			{
				if( !ItemCanTake( gameMode, item ) )
					return false;
			}

			//disable
			item2.Enabled = false;

			//detach
			ObjectInSpaceUtility.Detach( item2 );
			item2.RemoveFromParent( false );

			//Item: combine into one item
			var basicItem = item as Item2D;
			if( basicItem != null )
			{
				if( basicItem.ItemType.Value.CanCombineIntoOneItem )
				{
					var existsItem = GetItemByType( basicItem.ItemType.Value );
					if( existsItem != null )
					{
						existsItem.ItemCount += basicItem.ItemCount;
						return true;
					}
				}
			}

			var originalScale = item2.TransformV.Scale;

			//attach
			AddComponent( item2 );
			item2.Transform = Transform.Value;
			var transformOffset = ObjectInSpaceUtility.Attach( this, item2, TransformOffset.ModeEnum.Elements );
			if( transformOffset != null )
				transformOffset.ScaleOffset = originalScale / GetScaleFactor();

			return true;
		}

		/// <summary>
		/// Drops the item. The item will moved to the scene and will enabled.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="newTransform"></param>
		public bool ItemDrop( GameMode gameMode, ItemInterface item, /*bool calculateTransform, Transform setTransform, */double amount )
		{
			var item2 = (ObjectInSpace)item;
			var amount2 = amount;

			//check can drop
			if( item2.Parent != this )
				return false;

			var allowAction = true;
			gameMode.PerformItemCanDropEvent( this, item, ref allowAction, ref amount2 );
			if( !allowAction )
				return false;

			//Item2D: combined into one item
			var itemSplit = false;
			{
				var basicItem = item2 as Item2D;
				if( basicItem != null )
				{
					if( basicItem.ItemCount - amount2 > 0 )
					{
						basicItem.ItemCount -= amount2;
						itemSplit = true;
					}
				}
			}

			if( !itemSplit )
			{
				//disable
				item2.Enabled = false;

				//detach
				ObjectInSpaceUtility.Detach( item2 );
				item2.RemoveFromParent( false );
			}
			else
			{
				item2 = (ObjectInSpace)item2.Clone();
				( (Item2D)item2 ).ItemCount = amount2;
			}

			//add to the scene
			ParentScene.AddComponent( item2 );
			//if( calculateTransform )
			{
				//it is simple implementation
				item2.Transform = new Transform( TransformV.Position, TransformV.Rotation, item2.TransformV.Scale );
			}
			//else if( setTransform != null )
			//	item2.Transform = setTransform;

			//enable
			item2.Enabled = true;

			return true;
		}

		public void ItemDropClient( ItemInterface item, int amount )
		{
			var component = item as Component;
			if( component != null )
			{
				var writer = BeginNetworkMessageToServer( "ItemDrop" );
				if( writer != null )
				{
					writer.WriteVariableUInt64( (ulong)component.NetworkID );
					writer.WriteVariableInt32( amount );
					EndNetworkMessage();
				}
			}
		}

		/// <summary>
		/// Activates the item. The item will enabled.
		/// </summary>
		/// <param name="item"></param>
		public bool ItemActivate( GameMode gameMode, ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			//Item2D
			var basicItem = item as Item2D;
			if( basicItem != null && !basicItem.ItemType.Value.CanActivate )
				return false;

			if( gameMode != null )
			{
				var allowAction = true;
				gameMode.PerformItemCanActivateEvent( this, item, ref allowAction );
				if( !allowAction )
					return false;
			}

			//deactivate other before activate new
			{
				foreach( var item3 in GetAllItems() )
					ItemDeactivate( gameMode, item3 );

				//if can't deactivate other, then can't activate new
				if( GetActiveItem() != null )
					return false;
			}

			item2.Enabled = true;

			return true;
		}

		/// <summary>
		/// Deactivates the item. The item will disabled.
		/// </summary>
		/// <param name="item"></param>
		public bool ItemDeactivate( GameMode gameMode, ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			var allowAction = true;
			gameMode.PerformItemCanDeactivateEvent( this, item, ref allowAction );
			if( !allowAction )
				return false;

			item2.Enabled = false;

			return true;
		}

		public void ItemTakeAndActivateClient( ItemInterface item, bool activate = true )
		{
			var item2 = (ObjectInSpace)item;

			var writer = BeginNetworkMessageToServer( "ItemTakeAndActivate" );
			if( writer != null )
			{
				writer.WriteVariableUInt64( (ulong)item2.NetworkID );
				writer.Write( activate );
				EndNetworkMessage();
			}
		}

		public void ItemActivateClient( ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			var writer = BeginNetworkMessageToServer( "ItemActivate" );
			if( writer != null )
			{
				writer.WriteVariableUInt64( (ulong)item2.NetworkID );
				EndNetworkMessage();
			}
		}

		public void ItemDeactivateClient( ItemInterface item )
		{
			var item2 = (ObjectInSpace)item;

			var writer = BeginNetworkMessageToServer( "ItemDeactivate" );
			if( writer != null )
			{
				writer.WriteVariableUInt64( (ulong)item2.NetworkID );
				EndNetworkMessage();
			}
		}

		/// <summary>
		/// Returns first item of the character.
		/// </summary>
		/// <returns></returns>
		public ItemInterface ItemGetFirst()
		{
			foreach( var c in Components )
				if( c is ItemInterface item )
					return item;
			return null;
		}

		/// <summary>
		/// Returns first activated item of the character.
		/// </summary>
		/// <returns></returns>
		public ItemInterface ItemGetEnabledFirst()
		{
			foreach( var c in Components )
				if( c.Enabled && c is ItemInterface item )
					return item;
			return null;
		}

		protected virtual void UpdateEnabledItemTransform()
		{
			var item = ItemGetEnabledFirst();
			if( item != null )
			{
				var obj = item as ObjectInSpace;
				if( obj != null )
				{
					var offset = obj.GetComponent<TransformOffset>();
					if( offset != null )
					{
						//!!!!add customization support

						double sideOffset = 0.5;

						offset.PositionOffset = new Vector3( 0, 0, sideOffset );

						//offset.RotationOffset = Quaternion.FromRotateByY( LookToDirection.Vertical );
					}
				}
			}
		}

		public delegate void SoundPlayBeforeDelegate( Character2D sender, ref Sound sound, ref bool handled );
		public event SoundPlayBeforeDelegate SoundPlayBefore;

		public virtual void SoundPlay( Sound sound )
		{
			var handled = false;
			SoundPlayBefore?.Invoke( this, ref sound, ref handled );
			if( handled )
				return;

			ParentScene?.SoundPlay2D( sound );
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "Jump" )
			{
				SoundPlay( TypeCached.JumpSound );

				if( TypeCached.JumpAnimation.Value != null )
				{
					EventAnimationBegin( TypeCached.JumpAnimation, delegate ()
					{
						if( IsOnGround() )
							EventAnimationBegin( null );
					} );
				}
			}
			else if( message == "TickMovement" )
			{
				var forceIsOnGroundRemainingTime2 = reader.ReadSingle();
				if( !reader.Complete() )
					return false;

				forceIsOnGroundRemainingTime = forceIsOnGroundRemainingTime2;
			}

			return true;
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			//security check the object is controlled by the player
			var networkLogic = NetworkLogicUtility.GetNetworkLogic( this );
			if( networkLogic != null && networkLogic.ServerGetObjectControlledByUser( client.User, true ) == this )
			{
				if( message == "Jump" )
					Jump();
				else if( message == "ItemTakeAndActivate" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					var activate = reader.ReadBoolean();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null )
					{
						var item2 = item as ItemInterface;
						if( item2 != null )
						{
							var gameMode = (GameMode)ParentScene?.GetGameMode();
							if( gameMode != null )
							{
								if( ItemTake( gameMode, item2 ) )
								{
									if( activate )
										ItemActivate( gameMode, item2 );
								}
							}
						}
					}
				}
				else if( message == "ItemDrop" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					var amount = reader.ReadVariableInt32();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null )
					{
						var gameMode = (GameMode)ParentScene?.GetGameMode();
						if( gameMode != null )
						{
							var item2 = item as ItemInterface;
							if( item2 != null )
								ItemDrop( gameMode, item2, amount );
						}
					}
				}
				else if( message == "ItemActivate" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null && item.Parent == this )
					{
						var gameMode = (GameMode)ParentScene?.GetGameMode();
						if( gameMode != null )
						{
							var item2 = item as ItemInterface;
							if( item2 != null )
								ItemActivate( gameMode, item2 );
						}
					}
				}
				else if( message == "ItemDeactivate" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null && item.Parent == this )
					{
						var gameMode = (GameMode)ParentScene?.GetGameMode();
						if( gameMode != null )
						{
							var item2 = item as ItemInterface;
							if( item2 != null )
								ItemDeactivate( gameMode, item2 );
						}
					}
				}
			}

			return true;
		}

		public delegate void InteractionGetInfoEventDelegate( Character2D sender, GameMode gameMode, ref InteractiveObjectObjectInfo info );
		public event InteractionGetInfoEventDelegate InteractionGetInfoEvent;

		public virtual void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			GetComponent<AI>()?.InteractionGetInfo( gameMode, initiator, ref info );
			InteractionGetInfoEvent?.Invoke( this, gameMode, ref info );
		}

		public bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message )
		{
			var ai = GetComponent<AI>();
			if( ai != null && ai.InteractionInputMessage( gameMode, initiator, message ) )
				return true;
			return false;
		}

		public void InteractionEnter( ObjectInteractionContext context )
		{
			GetComponent<AI>()?.InteractionEnter( context );
		}

		public void InteractionExit( ObjectInteractionContext context )
		{
			GetComponent<AI>()?.InteractionExit( context );
		}

		public void InteractionUpdate( ObjectInteractionContext context )
		{
			GetComponent<AI>()?.InteractionUpdate( context );
		}

		protected override void OnComponentRemoved( Component component )
		{
			base.OnComponentRemoved( component );

			if( animationControllerCached == component )
				animationControllerCached = null;
		}
	}
}
