// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A basic class for characters.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character", -8999 )]
	public class Character : MeshInSpace, IProcessDamage, InteractiveObject
	{
		CharacterType typeCached = new CharacterType();

		//on ground and flying states
		float mainBodyGroundDistanceNoScale = 1000;//from Transform (bottom) of the body/object
		Scene.PhysicsWorldClass.Body groundBody;
		float forceIsOnGroundRemainingTime;
		float disableGravityRemainingTime;
		bool isOnGroundHasValue;
		bool isOnGroundValue;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		float onGroundTime;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		float elapsedTimeSinceLastGroundContact;
		//Vector3 lastSimulationStepPosition;

		//moveVector
		int moveVectorTimer;//is disabled when equal 0
		Vector2F moveVector;
		bool moveVectorRun;
		Vector2F lastTickForceVector;

		//jumping state
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		float jumpInactiveTime;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		float jumpDisableRemainingTime;

		//!!!!smooth?
		Vector3? lastTransformPosition;
		Vector3F lastLinearVelocity;

		//right now it includes look to. can separate
		SphericalDirectionF currentTurnToDirection;
		SphericalDirectionF requiredTurnToDirection;
		////Radian horizontalDirectionForUpdateRotation;
		//SphericalDirection currentLookToDirection;
		//SphericalDirection requiredLookToDirection;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		//Vector3 linearVelocityForSerialization;

		Vector3F groundRelativeVelocity;
		Vector3F[] groundRelativeVelocitySmoothArray;
		Vector3F groundRelativeVelocitySmooth;

		float allowToSleepTime;

		//crouching
		//const float crouchingVisualSwitchTime = .3f;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		bool crouching;
		//float crouchingSwitchRemainingTime;
		////[FieldSerialize( FieldSerializeSerializationTypes.World )]
		//float crouchingVisualFactor;

		//wiggle camera when walking
		float wiggleWhenWalkingSpeedFactor;

		//smooth camera
		double smoothCameraOffsetZ;
		//Vector3? initialTransformOffsetPositionInSimulation;

		//play one animation
		Animation playOneAnimation;
		double playOneAnimationSpeed = 1;
		double playOneAnimationRemainingTime;

		//weapon
		double disableUpdateAttachedWeaponRemainingTime;

		//optimization
		MeshInSpaceAnimationController animationControllerCached;
		//CharacterInputProcessing characterInputProcessingCached;
		//!!!!for what animations add the cache

		////damageFastChangeSpeed
		//Vector3 damageFastChangeSpeedLastVelocity = new Vector3( float.NaN, float.NaN, float.NaN );

		/////////////////////////////////////////

		const string characterTypeDefault = @"Content\Characters\Default\Default.charactertype";

		[DefaultValueReference( characterTypeDefault )]
		//[Category( "General" )]
		public Reference<CharacterType> CharacterType
		{
			get { if( _characterType.BeginGet() ) CharacterType = _characterType.Get( this ); return _characterType.value; }
			set
			{
				if( _characterType.BeginSet( ref value ) )
				{
					try
					{
						CharacterTypeChanged?.Invoke( this );

						//update cached type and mesh
						typeCached = _characterType.value;
						if( typeCached == null )
							typeCached = new CharacterType();
						UpdateMesh();
					}
					finally { _characterType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CharacterType"/> property value changes.</summary>
		public event Action<Character> CharacterTypeChanged;
		ReferenceField<CharacterType> _characterType = new Reference<CharacterType>( null, characterTypeDefault );

		//[Category( "Skeleton State" )]
		//[DefaultValue( "NaN NaN NaN" )]
		//public Reference<Vector3> TorsoLookAt
		//{
		//	get { if( _torsoLookAt.BeginGet() ) TorsoLookAt = _torsoLookAt.Get( this ); return _torsoLookAt.value; }
		//	set { if( _torsoLookAt.BeginSet( ref value ) ) { try { TorsoLookAtChanged?.Invoke( this ); } finally { _torsoLookAt.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TorsoLookAt"/> property value changes.</summary>
		//public event Action<Character> TorsoLookAtChanged;
		//ReferenceField<Vector3> _torsoLookAt = new Vector3( double.NaN, double.NaN, double.NaN );

		/// <summary>
		/// Left hand control ratio.
		/// </summary>
		[Category( "State" )]
		[Range( 0, 1 )]
		[DefaultValue( 0.0 )]
		public Reference<double> LeftHandFactor
		{
			get { if( _leftHandFactor.BeginGet() ) LeftHandFactor = _leftHandFactor.Get( this ); return _leftHandFactor.value; }
			set { if( _leftHandFactor.BeginSet( ref value ) ) { try { LeftHandFactorChanged?.Invoke( this ); } finally { _leftHandFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandFactor"/> property value changes.</summary>
		public event Action<Character> LeftHandFactorChanged;
		ReferenceField<double> _leftHandFactor = 0.0;

		/// <summary>
		/// Left hand target transform in the world coordinates. X - forward, -Z - palm.
		/// </summary>
		[Category( "State" )]
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> LeftHandTransform
		{
			get { if( _leftHandTransform.BeginGet() ) LeftHandTransform = _leftHandTransform.Get( this ); return _leftHandTransform.value; }
			set { if( _leftHandTransform.BeginSet( ref value ) ) { try { LeftHandTransformChanged?.Invoke( this ); } finally { _leftHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandTransform"/> property value changes.</summary>
		public event Action<Character> LeftHandTransformChanged;
		ReferenceField<Transform> _leftHandTransform = NeoAxis.Transform.Identity;

		/// <summary>
		/// Right hand control ratio.
		/// </summary>
		[Category( "State" )]
		[Range( 0, 1 )]
		[DefaultValue( 0.0 )]
		public Reference<double> RightHandFactor
		{
			get { if( _rightHandFactor.BeginGet() ) RightHandFactor = _rightHandFactor.Get( this ); return _rightHandFactor.value; }
			set { if( _rightHandFactor.BeginSet( ref value ) ) { try { RightHandFactorChanged?.Invoke( this ); } finally { _rightHandFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandFactor"/> property value changes.</summary>
		public event Action<Character> RightHandFactorChanged;
		ReferenceField<double> _rightHandFactor = 0.0;

		/// <summary>
		/// Right hand target transform in the world coordinates. X - forward, -Z - palm.
		/// </summary>
		[Category( "State" )]
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> RightHandTransform
		{
			get { if( _rightHandTransform.BeginGet() ) RightHandTransform = _rightHandTransform.Get( this ); return _rightHandTransform.value; }
			set { if( _rightHandTransform.BeginSet( ref value ) ) { try { RightHandTransformChanged?.Invoke( this ); } finally { _rightHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandTransform"/> property value changes.</summary>
		public event Action<Character> RightHandTransformChanged;
		ReferenceField<Transform> _rightHandTransform = NeoAxis.Transform.Identity;

		/// <summary>
		/// Head control ratio.
		/// </summary>
		[Category( "State" )]
		[Range( 0, 1 )]
		[DefaultValue( 0.0 )]
		public Reference<double> HeadFactor
		{
			get { if( _headFactor.BeginGet() ) HeadFactor = _headFactor.Get( this ); return _headFactor.value; }
			set { if( _headFactor.BeginSet( ref value ) ) { try { HeadFactorChanged?.Invoke( this ); } finally { _headFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadFactor"/> property value changes.</summary>
		public event Action<Character> HeadFactorChanged;
		ReferenceField<double> _headFactor = 0.0;

		/// <summary>
		/// Target position of the head.
		/// </summary>
		[Category( "State" )]
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> HeadLookAt
		{
			get { if( _headLookAt.BeginGet() ) HeadLookAt = _headLookAt.Get( this ); return _headLookAt.value; }
			set { if( _headLookAt.BeginSet( ref value ) ) { try { HeadLookAtChanged?.Invoke( this ); } finally { _headLookAt.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadLookAt"/> property value changes.</summary>
		public event Action<Character> HeadLookAtChanged;
		ReferenceField<Vector3> _headLookAt = new Vector3( 0, 0, 0 );

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> LeftHandThumbFingerFlexion
		//{
		//	get { if( _leftHandThumbFingerFlexion.BeginGet() ) LeftHandThumbFingerFlexion = _leftHandThumbFingerFlexion.Get( this ); return _leftHandThumbFingerFlexion.value; }
		//	set { if( _leftHandThumbFingerFlexion.BeginSet( ref value ) ) { try { LeftHandThumbFingerFlexionChanged?.Invoke( this ); } finally { _leftHandThumbFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeftHandThumbFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> LeftHandThumbFingerFlexionChanged;
		//ReferenceField<double> _leftHandThumbFingerFlexion = 0.0;

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> LeftHandIndexFingerFlexion
		//{
		//	get { if( _leftHandIndexFingerFlexion.BeginGet() ) LeftHandIndexFingerFlexion = _leftHandIndexFingerFlexion.Get( this ); return _leftHandIndexFingerFlexion.value; }
		//	set { if( _leftHandIndexFingerFlexion.BeginSet( ref value ) ) { try { LeftHandIndexFingerFlexionChanged?.Invoke( this ); } finally { _leftHandIndexFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeftHandIndexFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> LeftHandIndexFingerFlexionChanged;
		//ReferenceField<double> _leftHandIndexFingerFlexion = 0.0;

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> LeftHandMiddleFingerFlexion
		//{
		//	get { if( _leftHandMiddleFingerFlexion.BeginGet() ) LeftHandMiddleFingerFlexion = _leftHandMiddleFingerFlexion.Get( this ); return _leftHandMiddleFingerFlexion.value; }
		//	set { if( _leftHandMiddleFingerFlexion.BeginSet( ref value ) ) { try { LeftHandMiddleFingerFlexionChanged?.Invoke( this ); } finally { _leftHandMiddleFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeftHandMiddleFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> LeftHandMiddleFingerFlexionChanged;
		//ReferenceField<double> _leftHandMiddleFingerFlexion = 0.0;

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> LeftHandRingFingerFlexion
		//{
		//	get { if( _leftHandRingFingerFlexion.BeginGet() ) LeftHandRingFingerFlexion = _leftHandRingFingerFlexion.Get( this ); return _leftHandRingFingerFlexion.value; }
		//	set { if( _leftHandRingFingerFlexion.BeginSet( ref value ) ) { try { LeftHandRingFingerFlexionChanged?.Invoke( this ); } finally { _leftHandRingFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeftHandRingFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> LeftHandRingFingerFlexionChanged;
		//ReferenceField<double> _leftHandRingFingerFlexion = 0.0;

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> LeftHandPinkyFingerFlexion
		//{
		//	get { if( _leftHandPinkyFingerFlexion.BeginGet() ) LeftHandPinkyFingerFlexion = _leftHandPinkyFingerFlexion.Get( this ); return _leftHandPinkyFingerFlexion.value; }
		//	set { if( _leftHandPinkyFingerFlexion.BeginSet( ref value ) ) { try { LeftHandPinkyFingerFlexionChanged?.Invoke( this ); } finally { _leftHandPinkyFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeftHandPinkyFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> LeftHandPinkyFingerFlexionChanged;
		//ReferenceField<double> _leftHandPinkyFingerFlexion = 0.0;


		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> RightHandThumbFingerFlexion
		//{
		//	get { if( _rightHandThumbFingerFlexion.BeginGet() ) RightHandThumbFingerFlexion = _rightHandThumbFingerFlexion.Get( this ); return _rightHandThumbFingerFlexion.value; }
		//	set { if( _rightHandThumbFingerFlexion.BeginSet( ref value ) ) { try { RightHandThumbFingerFlexionChanged?.Invoke( this ); } finally { _rightHandThumbFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RightHandThumbFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> RightHandThumbFingerFlexionChanged;
		//ReferenceField<double> _rightHandThumbFingerFlexion = 0.0;

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> RightHandIndexFingerFlexion
		//{
		//	get { if( _rightHandIndexFingerFlexion.BeginGet() ) RightHandIndexFingerFlexion = _rightHandIndexFingerFlexion.Get( this ); return _rightHandIndexFingerFlexion.value; }
		//	set { if( _rightHandIndexFingerFlexion.BeginSet( ref value ) ) { try { RightHandIndexFingerFlexionChanged?.Invoke( this ); } finally { _rightHandIndexFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RightHandIndexFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> RightHandIndexFingerFlexionChanged;
		//ReferenceField<double> _rightHandIndexFingerFlexion = 0.0;

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> RightHandMiddleFingerFlexion
		//{
		//	get { if( _rightHandMiddleFingerFlexion.BeginGet() ) RightHandMiddleFingerFlexion = _rightHandMiddleFingerFlexion.Get( this ); return _rightHandMiddleFingerFlexion.value; }
		//	set { if( _rightHandMiddleFingerFlexion.BeginSet( ref value ) ) { try { RightHandMiddleFingerFlexionChanged?.Invoke( this ); } finally { _rightHandMiddleFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RightHandMiddleFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> RightHandMiddleFingerFlexionChanged;
		//ReferenceField<double> _rightHandMiddleFingerFlexion = 0.0;

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> RightHandRingFingerFlexion
		//{
		//	get { if( _rightHandRingFingerFlexion.BeginGet() ) RightHandRingFingerFlexion = _rightHandRingFingerFlexion.Get( this ); return _rightHandRingFingerFlexion.value; }
		//	set { if( _rightHandRingFingerFlexion.BeginSet( ref value ) ) { try { RightHandRingFingerFlexionChanged?.Invoke( this ); } finally { _rightHandRingFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RightHandRingFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> RightHandRingFingerFlexionChanged;
		//ReferenceField<double> _rightHandRingFingerFlexion = 0.0;

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> RightHandPinkyFingerFlexion
		//{
		//	get { if( _rightHandPinkyFingerFlexion.BeginGet() ) RightHandPinkyFingerFlexion = _rightHandPinkyFingerFlexion.Get( this ); return _rightHandPinkyFingerFlexion.value; }
		//	set { if( _rightHandPinkyFingerFlexion.BeginSet( ref value ) ) { try { RightHandPinkyFingerFlexionChanged?.Invoke( this ); } finally { _rightHandPinkyFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RightHandPinkyFingerFlexion"/> property value changes.</summary>
		//public event Action<Character> RightHandPinkyFingerFlexionChanged;
		//ReferenceField<double> _rightHandPinkyFingerFlexion = 0.0;

		//!!!!foots

		//!!!!
		//EyesFactor
		//EyesControlFactor

		//!!!!
		//[Category( "Skeleton State" )]
		//[DefaultValue( null )]
		//public Reference<ObjectInSpace> EyesLookAt
		//{
		//	get { if( _eyesLookAt.BeginGet() ) EyesLookAt = _eyesLookAt.Get( this ); return _eyesLookAt.value; }
		//	set { if( _eyesLookAt.BeginSet( ref value ) ) { try { EyesLookAtChanged?.Invoke( this ); } finally { _eyesLookAt.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="EyesLookAt"/> property value changes.</summary>
		//public event Action<Character> EyesLookAtChanged;
		//ReferenceField<ObjectInSpace> _eyesLookAt = null;

		/////////////////////////////////////////
		//Crawl

		////damageFastChangeSpeed

		//const float damageFastChangeSpeedMinimalSpeedDefault = 10;
		//[FieldSerialize]
		//float damageFastChangeSpeedMinimalSpeed = damageFastChangeSpeedMinimalSpeedDefault;

		//const float damageFastChangeSpeedFactorDefault = 40;
		//[FieldSerialize]
		//float damageFastChangeSpeedFactor = damageFastChangeSpeedFactorDefault;

		/////////////////////////////////////////

		/// <summary>
		/// The health of the character.
		/// </summary>
		[Category( "Game Framework" )]
		[DefaultValue( 0.0 )]
		public Reference<double> Health
		{
			get { if( _health.BeginGet() ) Health = _health.Get( this ); return _health.value; }
			set { if( _health.BeginSet( ref value ) ) { try { HealthChanged?.Invoke( this ); } finally { _health.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Health"/> property value changes.</summary>
		public event Action<Character> HealthChanged;
		ReferenceField<double> _health = 0.0;

		/// <summary>
		/// The team index of the object.
		/// </summary>
		[Category( "Game Framework" )]
		[DefaultValue( 0 )]
		public Reference<int> Team
		{
			get { if( _team.BeginGet() ) Team = _team.Get( this ); return _team.value; }
			set { if( _team.BeginSet( ref value ) ) { try { TeamChanged?.Invoke( this ); } finally { _team.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Team"/> property value changes.</summary>
		public event Action<Character> TeamChanged;
		ReferenceField<int> _team = 0;

		/////////////////////////////////////////

		public Character()
		{
			Collision = true;
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//these properties are under control by the class
				case nameof( Mesh ):
				case nameof( Collision ):
					skip = true;
					break;
				}
			}
		}

		void UpdateMesh()
		{
			Mesh = typeCached.Mesh;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			animationControllerCached = null;
			//characterInputProcessingCached = null;

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				//update cached type and mesh
				typeCached = CharacterType.Value;
				if( typeCached == null )
					typeCached = new CharacterType();
				UpdateMesh();

				//update requiredTurnToDirection, currentTurnToDirection
				requiredTurnToDirection = SphericalDirectionF.FromVector( TransformV.Rotation.GetForward().ToVector3F() );
				currentTurnToDirection = requiredTurnToDirection;

				//if( mainBody != null )
				//	mainBody.LinearVelocity = linearVelocityForSerialization;

				if( ParentScene != null )
					ParentScene.PhysicsSimulationStepAfter += ParentScene_PhysicsSimulationStepAfter;

				//if( EngineApp.IsSimulation )
				TickAnimate( 0.001f );
				SpaceBoundsUpdate();
			}
			else
			{
				if( ParentScene != null )
					ParentScene.PhysicsSimulationStepAfter -= ParentScene_PhysicsSimulationStepAfter;
			}
		}

		private void ParentScene_PhysicsSimulationStepAfter( Scene obj )
		{
			if( ParentScene == null )
				return;

			if( PhysicalBody != null && PhysicalBody.Active )
				UpdateRotation();
		}

		//CharacterInputProcessing GetCharacterInputProcessing()
		//{
		//	if( characterInputProcessingCached == null )
		//		characterInputProcessingCached = GetComponent<CharacterInputProcessing>();
		//	return characterInputProcessingCached;
		//}

		public void SetTransformAndTurnToDirectionInstantly( Transform value )
		{
			Transform = value;
			TurnToDirection( value.Rotation.GetForward().ToVector3F(), true );
		}

		protected override RigidBody OnGetCollisionShapeData( Mesh.CompiledData meshResult )
		{
			//it must support physics shape caching

			var body = new RigidBody();
			body.MotionType = PhysicsMotionType.Dynamic;
			body.Mass = typeCached.Mass;

			//!!!!center of mass

			typeCached.GetBodyFormInfo( crouching, out var height, out _ );
			var length = height - typeCached.Radius * 2;
			if( length < 0.01 )
				length = 0.01;

			var capsuleShape = body.CreateComponent<CollisionShape_Capsule>();
			capsuleShape.Height = length;
			capsuleShape.Radius = typeCached.Radius;
			capsuleShape.LocalTransform = new Transform( new Vector3( 0, 0, height * 0.5 ), Quaternion.Identity );

			return body;
		}

		protected override void OnCollisionBodyCreated()
		{
			base.OnCollisionBodyCreated();

			if( PhysicalBody != null )
			{
				//!!!!было
				//body.LinearSleepingThreshold = 0;
				//body.AngularSleepingThreshold = 0;

				//!!!!было 10
				//!!!!можно ли 10?
				PhysicalBody.AngularDamping = 1;
				//body.AngularDamping = 10;

				PhysicalBody.Friction = 0;
				PhysicalBody.Restitution = 0;
			}
		}

		//public void UpdateCollisionBody()
		//{
		//	if( NetworkIsClient )
		//		return;

		//	////DestroyCollisionBody();

		//	////body.PhysX_SolverPositionIterations = body.PhysX_SolverPositionIterations * 2;
		//	////body.PhysX_SolverVelocityIterations = body.PhysX_SolverVelocityIterations * 2;

		//	//body.MaterialFriction = 0;
		//	////!!!!
		//	////body.MaterialSpinningFriction = 0;
		//	////body.MaterialRollingFriction = 0;
		//	//body.MaterialRestitution = 0;
		//	////shape.Hardness = 0;
		//	////shape.SpecialLiquidDensity = 2000;
		//	////shape.ContactGroup = (int)ContactGroup.Dynamic;

		//}

		public double GetScaleFactor()
		{
			var result = TransformV.Scale.MaxComponent();
			if( result == 0 )
				result = 0.0001;
			return result;
		}

		///////////////////////////////////////////

		public void SetMoveVector( Vector2F direction, bool run )
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
		public SphericalDirectionF CurrentTurnToDirection
		{
			get { return currentTurnToDirection; }
		}

		[Browsable( false )]
		public SphericalDirectionF RequiredTurnToDirection
		{
			get { return requiredTurnToDirection; }
		}

		public void TurnToDirection( SphericalDirectionF value, bool turnInstantly )
		{
			requiredTurnToDirection = value;
			if( turnInstantly )
				currentTurnToDirection = requiredTurnToDirection;

			if( turnInstantly )
				UpdateRotation();
		}

		public void TurnToDirection( Vector3F value, bool turnInstantly )
		{
			TurnToDirection( SphericalDirectionF.FromVector( value ), turnInstantly );
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			//update requiredTurnToDirection, currentTurnToDirection
			if( EngineApp.IsEditor )
			{
				requiredTurnToDirection = SphericalDirectionF.FromVector( TransformV.Rotation.GetForward().ToVector3F() );
				currentTurnToDirection = requiredTurnToDirection;
			}

			isOnGroundHasValue = false;
		}

		void UpdateRotation()
		{
			var diff = CurrentTurnToDirection.GetVector();
			var halfAngle = Math.Atan2( diff.Y, diff.X ) * 0.5;
			//var halfAngle = horizontalDirectionForUpdateRotation * 0.5;
			Quaternion rot = new Quaternion( new Vector3( 0, 0, Math.Sin( halfAngle ) ), Math.Cos( halfAngle ) );

			const float epsilon = .0001f;

			var tr = TransformV;

			//update Rotation
			if( !tr.Rotation.Equals( rot, epsilon ) )
				Transform = tr.UpdateRotation( rot );
		}

		bool CalculateIsOnGround()
		{
			if( jumpInactiveTime != 0 )
				return false;
			if( forceIsOnGroundRemainingTime > 0 )
				return true;

			const float maxThreshold = 0.1f;
			//const float maxThreshold = 0.2f;
			return mainBodyGroundDistanceNoScale < maxThreshold && groundBody != null;

			//double distanceFromPositionToFloor = 0.0;//crouching ? typeCached.CrouchingPositionToGroundHeight : typeCached.PositionToGroundHeight;
			//const double maxThreshold = 0.2;
			//return mainBodyGroundDistanceNoScale - maxThreshold < distanceFromPositionToFloor && groundBody != null;
		}

		public bool IsOnGround()
		{
			if( !isOnGroundHasValue )
			{
				isOnGroundHasValue = true;
				isOnGroundValue = CalculateIsOnGround();
			}
			return isOnGroundValue;
		}

		public bool IsOnGroundWithLatency()
		{
			return elapsedTimeSinceLastGroundContact < 0.25f || IsOnGround();
		}

		public float GetElapsedTimeSinceLastGroundContact()
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
				groundBody = null;

			isOnGroundHasValue = false;

			//ScreenMessages.Add( "IsOnGround: " + IsOnGround().ToString() );
			//ScreenMessages.Add( "required " + requiredTurnToDirection.Horizontal.ToString() );
			//ScreenMessages.Add( "current " + currentTurnToDirection.Horizontal.ToString() );
			//ScreenMessages.Add( "GetSmoothCameraOffset " + GetSmoothCameraOffset().ToString() );
			//ScreenMessages.Add( "damping " + PhysicalBody.LinearDamping.ToString() );
			//ScreenMessages.Add( "moveVector " + GetMoveVector().ToString() );

			//!!!!всё тут ниже

			//!!!!что-то можно реже вызывать


			if( PhysicalBody != null )
			{
				TickMovement();
				TickPhysicsForce();
			}
			TickCurrentTurnToDirection();
			//UpdateRotation();// true );

			if( typeCached.Jump )
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

			TickWiggleWhenWalkingSpeedFactor();
			TickSmoothCameraOffset();

			if( moveVectorTimer != 0 )
				moveVectorTimer--;

			//if( CrouchingSupport )
			//	TickCrouching();

			//if( DamageFastChangeSpeedFactor != 0 )
			//	DamageFastChangeSpeedTick();

			var trPosition = TransformV.Position;
			if( lastTransformPosition.HasValue )
				lastLinearVelocity = ( ( trPosition - lastTransformPosition.Value ) / Time.SimulationDelta ).ToVector3F();
			else
				lastLinearVelocity = Vector3F.Zero;
			lastTransformPosition = trPosition;

			//lastSimulationStepPosition = GetTransform().Position;

			if( forceIsOnGroundRemainingTime > 0 )
			{
				forceIsOnGroundRemainingTime -= Time.SimulationDelta;
				if( forceIsOnGroundRemainingTime < 0 )
				{
					forceIsOnGroundRemainingTime = 0;
					isOnGroundHasValue = false;
				}
			}

			if( disableGravityRemainingTime > 0 )
			{
				disableGravityRemainingTime -= Time.SimulationDelta;
				if( disableGravityRemainingTime < 0 )
					disableGravityRemainingTime = 0;
			}

			//UpdateTransformOffsetInSimulation();
		}

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			//clear groundBody when disposed
			if( groundBody != null && groundBody.Disposed )
				groundBody = null;

			isOnGroundHasValue = false;


			//!!!!update


			if( PhysicalBody != null )
				CalculateMainBodyGroundDistanceAndGroundBody();

			//UpdateRotation();// true );
			if( typeCached.Jump )
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

			TickWiggleWhenWalkingSpeedFactor();
			TickSmoothCameraOffset();

			////if( CrouchingSupport )
			////	TickCrouching();

			////if( DamageFastChangeSpeedFactor != 0 )
			////	DamageFastChangeSpeedTick();

			var trPosition = TransformV.Position;
			if( lastTransformPosition.HasValue )
				lastLinearVelocity = ( ( trPosition - lastTransformPosition.Value ) / Time.SimulationDelta ).ToVector3F();
			else
				lastLinearVelocity = Vector3F.Zero;
			lastTransformPosition = trPosition;

			////lastSimulationStepPosition = GetTransform().Position;

			if( forceIsOnGroundRemainingTime > 0 )
			{
				forceIsOnGroundRemainingTime -= Time.SimulationDelta;
				if( forceIsOnGroundRemainingTime < 0 )
				{
					forceIsOnGroundRemainingTime = 0;
					isOnGroundHasValue = false;
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.IsEditor )
				isOnGroundHasValue = false;

			//!!!!slowly

			TickAnimate( delta );
			UpdateEnabledItemTransform( delta );
		}

		public bool IsNeedRun()
		{
			//use specified force move vector
			if( moveVectorTimer != 0 )
				return moveVectorRun;

			return false;
		}

		Vector2F GetMoveVector()
		{
			//use specified move vector
			if( moveVectorTimer != 0 )
				return moveVector;

			return Vector2F.Zero;
		}

		void TickPhysicsForce()
		{
			//!!!!slowly?

			var forceVec = GetMoveVector();
			if( forceVec != Vector2.Zero )
			{
				float speedMultiplier = 1;
				//if( FastMoveInfluence != null )
				//	speedCoefficient = FastMoveInfluence.Type.Coefficient;

				double maxSpeed = 0;
				double force = 0;

				if( IsOnGround() )
				{
					//calcualate maxSpeed and force on ground

					var localVec = ( new Vector3F( forceVec.X, forceVec.Y, 0 ) * TransformV.Rotation.GetInverse().ToQuaternionF() ).ToVector2();

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
							double speedX;
							if( localVec.X > 0 )
								speedX = running ? typeCached.RunForwardMaxSpeed : typeCached.WalkForwardMaxSpeed;
							else
								speedX = running ? typeCached.RunBackwardMaxSpeed : typeCached.WalkBackwardMaxSpeed;
							maxSpeed += speedX * Math.Abs( localVec.X );
							force += ( running ? typeCached.RunForce : typeCached.WalkForce ) * Math.Abs( localVec.X );
						}

						if( Math.Abs( localVec.Y ) >= .001f )
						{
							//left and right
							maxSpeed += ( running ? typeCached.RunSideMaxSpeed : typeCached.WalkSideMaxSpeed ) * Math.Abs( localVec.Y );
							force += ( running ? typeCached.RunForce : typeCached.WalkForce ) * Math.Abs( localVec.Y );
						}
					}
					else
					{
						maxSpeed = typeCached.CrouchingMaxSpeed;
						force = typeCached.CrouchingForce;
					}
				}
				else
				{
					//calcualate maxSpeed and force when flying.
					if( typeCached.FlyControl )
					{
						maxSpeed = typeCached.FlyControlMaxSpeed;
						force = typeCached.FlyControlForce;
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

					if( GetLinearVelocity().ToVector2().Length() < maxSpeed )
						PhysicalBody.ApplyForce( new Vector3F( forceVec.X, forceVec.Y, 0 ) * (float)force * Time.SimulationDelta, Vector3F.Zero );
				}
			}

			lastTickForceVector = forceVec;
		}

		void UpdateMainBodyDamping()
		{
			//!!!!
			//реже вызывать

			if( IsOnGround() && jumpInactiveTime == 0 )
			{
				//!!!!new commented
				if( /*allowToSleepTime > Time.SimulationDelta * 2.5f &&*/ GetMoveVector() == Vector2F.Zero )
					PhysicalBody.LinearDamping = (float)typeCached.LinearDampingOnGroundIdle;
				else
					PhysicalBody.LinearDamping = (float)typeCached.LinearDampingOnGroundMove;
			}
			else
				PhysicalBody.LinearDamping = (float)typeCached.LinearDampingFly;
		}

		void TickMovement()
		{
			//!!!!need?
			////wake up when ground is moving
			//if( !PhysicalBody.Active && groundBody != null && groundBody.Active && ( groundBody.LinearVelocity.LengthSquared() > .3f || groundBody.AngularVelocity.LengthSquared() > .3f ) )
			//{
			//	PhysicalBody.Active = true;
			//}

			CalculateMainBodyGroundDistanceAndGroundBody();//out var addForceOnBigSlope );

			if( PhysicalBody.Active || !IsOnGround() )
			{
				UpdateMainBodyDamping();

				if( IsOnGround() )
				{
					//reset angular velocity
					PhysicalBody.AngularVelocity = Vector3F.Zero;

					//move the object when it underground
					if( lastTickForceVector != Vector2.Zero && forceIsOnGroundRemainingTime == 0 )
					{
						var scaleFactor = GetScaleFactor();

						var maxSpeed = Math.Min( typeCached.WalkSideMaxSpeed, Math.Min( typeCached.WalkForwardMaxSpeed, typeCached.WalkBackwardMaxSpeed ) );
						Vector3 newPositionOffsetNoScale = new Vector3( lastTickForceVector.GetNormalize() * (float)maxSpeed * .15f, 0 );

						ClimbObstacleTest( ref newPositionOffsetNoScale, out var upHeightNoScale );

						//move object
						typeCached.GetBodyFormInfo( crouching, out var height, out var walkUpHeight );
						//!!!! * 0.5?
						if( upHeightNoScale > typeCached.Radius * 0.5 && upHeightNoScale <= walkUpHeight && jumpInactiveTime == 0 && walkUpHeight != 0 )
						{
							////additional offset
							//upHeightNoScale += typeCached.Radius * 0.5;

							var tr = TransformV;
							var newPosition = tr.Position + new Vector3( 0, 0, upHeightNoScale * scaleFactor );

							if( IsFreePositionToMove( newPosition ) )
							{
								Transform = tr.UpdatePosition( newPosition );
								//SetTransform( tr.UpdatePosition( newPosition ), false );

								////!!!!temp
								//ScreenMessages.Add( "climb" );

								forceIsOnGroundRemainingTime = 0.5f;//0.2f;
								disableGravityRemainingTime = 0.5f;
								isOnGroundHasValue = false;
								smoothCameraOffsetZ = Math.Min( tr.Position.Z - newPosition.Z, smoothCameraOffsetZ );

								if( NetworkIsServer )
								{
									var writer = BeginNetworkMessageToEveryone( "TickMovement" );
									writer.Write( (float)forceIsOnGroundRemainingTime );
									writer.Write( (float)smoothCameraOffsetZ );
									EndNetworkMessage();
								}
							}
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
				if( IsOnGround() && groundBody != null && typeCached.ApplyGroundVelocity )
				{
					if( groundBody.MotionType != PhysicsMotionType.Static && groundBody.Active )
					{
						Vector3F groundVel = groundBody.LinearVelocity;

						Vector3F vel = PhysicalBody.LinearVelocity;

						if( groundVel.X > 0 && vel.X >= 0 && vel.X < groundVel.X )
							vel.X = groundVel.X;
						else if( groundVel.X < 0 && vel.X <= 0 && vel.X > groundVel.X )
							vel.X = groundVel.X;

						if( groundVel.Y > 0 && vel.Y >= 0 && vel.Y < groundVel.Y )
							vel.Y = groundVel.Y;
						else if( groundVel.Y < 0 && vel.Y <= 0 && vel.Y > groundVel.Y )
							vel.Y = groundVel.Y;

						if( groundVel.Z > 0 && vel.Z >= 0 && vel.Z < groundVel.Z )
							vel.Z = groundVel.Z;
						else if( groundVel.Z < 0 && vel.Z <= 0 && vel.Z > groundVel.Z )
							vel.Z = groundVel.Z;

						//simple anti-damping
						vel += groundVel * .25f;

						PhysicalBody.LinearVelocity = vel;
					}
				}

				//sleep if on ground and zero velocity

				bool needSleep = false;
				if( IsOnGround() && groundBody != null )
				{
					bool groundStopped = !groundBody.Active || ( groundBody.LinearVelocity.LengthSquared() < .3f && groundBody.AngularVelocity.LengthSquared() < .3f );
					if( groundStopped && GetLinearVelocity().ToVector2().Length() < typeCached.MinSpeedToSleepBody && GetMoveVector() == Vector2F.Zero )
						needSleep = true;
				}

				if( needSleep )
					allowToSleepTime += Time.SimulationDelta;
				else
					allowToSleepTime = 0;

				PhysicalBody.Active = allowToSleepTime < Time.SimulationDelta * 2.5f;

				//if( allowToSleepTime > Time.SimulationDelta * 2.5f )
				//{
				//	PhysicalBody.Active = false;
				//	//mainBody.WantsDeactivation();
				//}
				//else
				//{
				//	PhysicalBody.Active = true;
				//	//mainBody.Activate();
				//}
			}

			PhysicalBody.GravityFactor = disableGravityRemainingTime == 0 ? 1 : 0;
			//mainBody.EnableGravity = disableGravityRemainingTime == 0;
		}

		bool VolumeCheckGetFirstNotFreePlace( ref Capsule sourceVolumeCapsule, ref Vector3 destVector, bool firstIteration, float step, out List<Scene.PhysicsWorldClass.Body> collisionBodies, out float collisionDistance, out bool collisionOnFirstCheck )
		{

			//!!!!GC. где еще

			collisionBodies = new List<Scene.PhysicsWorldClass.Body>();
			collisionDistance = 0;
			collisionOnFirstCheck = false;

			//!!!!sweep юзать

			bool firstCheck = true;

			Vector3 direction = destVector.GetNormalize();
			float totalDistance = (float)destVector.Length();
			int stepCount = (int)( (float)totalDistance / step ) + 2;
			Vector3 previousOffset = Vector3.Zero;


			//!!!!sweep test


			for( int nStep = 0; nStep < stepCount; nStep++ )
			{
				float distance = (float)nStep * step;
				if( distance > totalDistance )
					distance = totalDistance;
				Vector3 offset = direction * distance;

				var scene = ParentScene;
				if( scene != null )
				{
					var capsule = sourceVolumeCapsule;
					CapsuleAddOffset( ref capsule, ref offset );

					//check by capsule
					{
						var contactTestItem = new PhysicsVolumeTestItem( capsule, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.All/*OneForEach*/ );
						ParentScene.PhysicsVolumeTest( contactTestItem );

						foreach( var item in contactTestItem.Result )
						{
							if( item.Body == PhysicalBody )
								continue;
							if( !collisionBodies.Contains( item.Body ) )
								collisionBodies.Add( item.Body );
						}
					}

					//check by cylinder at bottom
					{
						var cylinder = new Cylinder( capsule.GetCenter(), capsule.Point1 - new Vector3( 0, 0, typeCached.Radius ), typeCached.Radius );
						if( cylinder.Point1 != cylinder.Point2 )
						{
							var contactTestItem = new PhysicsVolumeTestItem( cylinder, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.All/*OneForEach*/ );
							ParentScene.PhysicsVolumeTest( contactTestItem );

							foreach( var resultItem in contactTestItem.Result )
							{
								if( resultItem.Body == PhysicalBody )
									continue;
								if( !collisionBodies.Contains( resultItem.Body ) )
									collisionBodies.Add( resultItem.Body );
							}
						}
					}
				}

				if( collisionBodies.Count != 0 )
				{
					//second iteration
					if( nStep != 0 && firstIteration )
					{
						float step2 = step / 10;
						var capsule = sourceVolumeCapsule;
						CapsuleAddOffset( ref capsule, ref previousOffset );
						var destVector2 = offset - previousOffset;

						if( VolumeCheckGetFirstNotFreePlace( ref capsule, ref destVector2, false, step2, out var collisionBodies2,
							out var collisionDistance2, out var collisionOnFirstCheck2 ) )
						{
							collisionBodies = collisionBodies2;
							collisionDistance = ( previousOffset != Vector3.Zero ? (float)previousOffset.Length() : 0 ) + collisionDistance2;
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

		//!!!!
		List<Ray> debugRays;// = new List<Ray>();

		void CalculateMainBodyGroundDistanceAndGroundBody()//out Vector3 addForceOnBigSlope )
		{
			//addForceOnBigSlope = Vector3.Zero;

			debugRays?.Clear();

			if( PhysicalBody.Active || !IsOnGround() )
			{
				typeCached.GetBodyFormInfo( crouching, out var height, out var walkUpHeight );//, out var fromPositionToFloorDistance );

				GetVolumeCapsule( out var capsule );
				//make radius smaller
				capsule.Radius *= .99f;

				mainBodyGroundDistanceNoScale = 1000;
				groundBody = null;

				var scene = ParentScene;
				if( scene != null )
				{
					var scaleFactor = GetScaleFactor();

					//1. get collision bodies
					List<Scene.PhysicsWorldClass.Body> collisionBodies;
					float collisionOffset;
					{
						Vector3 destVector = new Vector3( 0, 0, -height * scaleFactor );
						//Vector3 destVector = new Vector3( 0, 0, -height * 1.5f * scaleFactor );
						var step = (float)( typeCached.Radius.Value / 2 * scaleFactor );
						VolumeCheckGetFirstNotFreePlace( ref capsule, ref destVector, true, step, out collisionBodies, out collisionOffset, out _ );
					}

					//2. check slope angle
					if( collisionBodies.Count != 0 )
					{
						Vector3 rayCenter = capsule.Point1 - new Vector3( 0, 0, collisionOffset );

						Scene.PhysicsWorldClass.Body foundBodyWithGoodAngle = null;
						Radian foundBodyWithGoodAngleSlopeAngle = 0;
						//Vector3 bigSlopeVector = Vector3.Zero;

						//!!!!call parallel?

						const int horizontalStepCount = 8;
						const int verticalStepCount = 4;

						for( int verticalStep = 0; verticalStep < verticalStepCount; verticalStep++ )
						{
							//.8f - to disable checking by horizontal rays
							float verticalAngle = MathEx.PI / 2 - ( (float)verticalStep / (float)verticalStepCount ) * MathEx.PI / 2 * .8f;

							for( int horizontalStep = 0; horizontalStep < horizontalStepCount; horizontalStep++ )
							{
								//skip same rays on direct vertical ray
								if( verticalStep == 0 && horizontalStep != 0 )
									continue;

								float horizontalAngle = ( (float)horizontalStep / (float)horizontalStepCount ) * MathEx.PI * 2;

								var sphereDir = new SphericalDirection( horizontalAngle, -verticalAngle );
								Ray ray = new Ray( rayCenter, sphereDir.GetVector() * typeCached.Radius * 1.3f * scaleFactor );//Type.Radius * 1.1f );

								var item = new PhysicsRayTestItem( ray, PhysicsRayTestItem.ModeEnum.All, PhysicsRayTestItem.FlagsEnum.CalculateNormal );
								scene.PhysicsRayTest( item );

								debugRays?.Add( ray );

								for( int n = 0; n < item.Result.Length; n++ )//foreach( var resultItem in item.Result )
								{
									ref var resultItem = ref item.Result[ n ];

									if( resultItem.Body != PhysicalBody )
									{
										//check slope angle
										var slope = MathAlgorithms.GetVectorsAngle( resultItem.Normal, Vector3.ZAxis );
										if( slope < typeCached.MaxSlopeAngle.Value.InRadians() )
										{
											if( foundBodyWithGoodAngle == null || slope < foundBodyWithGoodAngleSlopeAngle )
											{
												foundBodyWithGoodAngle = resultItem.Body;
												foundBodyWithGoodAngleSlopeAngle = slope;
											}
										}
									}
									//else
									//{
									//	Vector3 vector = new Vector3( result.Normal.X, result.Normal.Y, 0 );
									//	if( vector != Vector3.Zero )
									//		bigSlopeVector += vector;
									//}
								}

								if( foundBodyWithGoodAngle != null )
									break;
							}
							if( foundBodyWithGoodAngle != null )
								break;
						}

						if( foundBodyWithGoodAngle != null )
						{
							groundBody = foundBodyWithGoodAngle;
							mainBodyGroundDistanceNoScale = /*fromPositionToFloorDistance + */collisionOffset / (float)scaleFactor;
						}
						//else
						//{
						//	if( bigSlopeVector != Vector3.Zero )
						//	{
						//		//add force
						//		bigSlopeVector.Normalize();
						//		bigSlopeVector *= mainBody.Mass * 2;
						//		addForceOnBigSlope = bigSlopeVector;
						//	}
						//}
					}
				}
			}

			isOnGroundHasValue = false;
		}

		void GetVolumeCapsule( out Capsule capsule )
		{
			var tr = TransformV;
			var height = typeCached.Height.Value;
			var radius = typeCached.Radius.Value;
			capsule = new Capsule( tr.Position + new Vector3( 0, 0, radius ), tr.Position + new Vector3( 0, 0, height - radius ), radius );
		}

		void CapsuleAddOffset( ref Capsule capsule, ref Vector3 offset )
		{
			capsule.Point1 += offset;
			capsule.Point2 += offset;
		}

		void ClimbObstacleTest( ref Vector3 newPositionOffsetNoScale, out double upHeightNoScale )
		{
			typeCached.GetBodyFormInfo( crouching, out var height, out var walkUpHeight );

			var scaleFactor = GetScaleFactor();

			GetVolumeCapsule( out var volumeCapsule );
			var offset = ( newPositionOffsetNoScale + new Vector3( 0, 0, walkUpHeight ) ) * scaleFactor;
			CapsuleAddOffset( ref volumeCapsule, ref offset );

			var destVector = new Vector3( 0, 0, -walkUpHeight );
			var step = (float)typeCached.Radius / 2;
			List<Scene.PhysicsWorldClass.Body> collisionBodies;
			float collisionDistance;
			bool collisionOnFirstCheck;
			bool foundCollision = VolumeCheckGetFirstNotFreePlace( ref volumeCapsule, ref destVector, true, step, out collisionBodies, out collisionDistance, out collisionOnFirstCheck );
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
					{
						jumpInactiveTime = 0;
						isOnGroundHasValue = false;
					}
				}
			}

			if( IsOnGround() && onGroundTime > Time.SimulationDelta && jumpInactiveTime == 0 && jumpDisableRemainingTime != 0 && PhysicalBody != null )
			{
				var vel = PhysicalBody.LinearVelocity;
				vel.Z = (float)( typeCached.JumpSpeed * GetScaleFactor() );
				PhysicalBody.LinearVelocity = vel;

				var tr = TransformV;
				Transform = tr.UpdatePosition( tr.Position + new Vector3( 0, 0, 0.05 ) );
				//SetTransform( tr.UpdatePosition( tr.Position + new Vector3( 0, 0, 0.05 ) ), false );

				jumpInactiveTime = 0.2f;
				jumpDisableRemainingTime = 0;
				isOnGroundHasValue = false;

				UpdateMainBodyDamping();

				//PhysicalBody.Active = true;

				SoundPlay( typeCached.JumpSound );
				StartPlayOneAnimation( typeCached.JumpAnimation );

				if( NetworkIsServer && ( typeCached.JumpSound.ReferenceOrValueSpecified || typeCached.JumpAnimation.ReferenceOrValueSpecified ) )
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
					{
						jumpInactiveTime = 0;
						isOnGroundHasValue = false;
					}
				}
			}
		}

		public void TryJump()
		{
			if( !typeCached.Jump )
				return;
			if( Crouching )
				return;

			jumpDisableRemainingTime = 0.4f;
			TickJump( true );
		}

		[Browsable( false )]
		public Vector2 LastTickForceVector
		{
			get { return lastTickForceVector; }
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			//base.OnSpaceBoundsUpdate( ref newBounds );

			//!!!!bounds prediction

			var meshResult = Mesh.Value?.Result;
			if( meshResult != null )
			{
				//make bounds from bounds sphere (to expand bounds for animation)
				var meshBounds = meshResult.SpaceBounds;
				meshBounds.BoundingSphere.ToBounds( out var b );
				var b2 = TransformV * b;
				newBounds = new SpaceBounds( ref b2 );
			}
			else
				base.OnSpaceBoundsUpdate( ref newBounds );

			//if( PhysicalBody != null )
			//{
			//	GetBox( out var box );
			//	box.ToBounds( out var bounds );
			//	newBounds = SpaceBounds.Merge( newBounds, new SpaceBounds( ref bounds ) );
			//}
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			context.thisObjectWasChecked = true;
			GetBox( out var box );
			if( box.Intersects( context.ray, out var scale1, out var scale2 ) )
				context.thisObjectResultRayScale = Math.Min( scale1, scale2 );

			//if( SpaceBounds.BoundingBox.Intersects( context.ray, out var scale ) )
			//	context.thisObjectResultRayScale = scale;
		}

		void CalculateGroundRelativeVelocity()
		{
			if( PhysicalBody != null && PhysicalBody.Active )
			{
				groundRelativeVelocity = GetLinearVelocity();
				if( groundBody != null && groundBody.AngularVelocity.LengthSquared() < .3f )
					groundRelativeVelocity -= groundBody.LinearVelocity;

				//groundRelativeVelocityToSmooth
				if( groundRelativeVelocitySmoothArray == null )
				{
					var seconds = .2f;
					var count = ( seconds / Time.SimulationDelta ) + .999f;
					groundRelativeVelocitySmoothArray = new Vector3F[ (int)count ];
				}
				for( int n = 0; n < groundRelativeVelocitySmoothArray.Length - 1; n++ )
					groundRelativeVelocitySmoothArray[ n ] = groundRelativeVelocitySmoothArray[ n + 1 ];
				groundRelativeVelocitySmoothArray[ groundRelativeVelocitySmoothArray.Length - 1 ] = groundRelativeVelocity;
				groundRelativeVelocitySmooth = Vector3F.Zero;
				for( int n = 0; n < groundRelativeVelocitySmoothArray.Length; n++ )
					groundRelativeVelocitySmooth += groundRelativeVelocitySmoothArray[ n ];
				groundRelativeVelocitySmooth /= (float)groundRelativeVelocitySmoothArray.Length;
			}
			else
			{
				//!!!!slowly?

				groundRelativeVelocity = Vector3F.Zero;
				groundRelativeVelocitySmoothArray = null;
				groundRelativeVelocitySmooth = Vector3F.Zero;
			}
		}

		[Browsable( false )]
		public Vector3 GroundRelativeVelocity
		{
			get { return groundRelativeVelocity; }
		}

		[Browsable( false )]
		public Vector3 GroundRelativeVelocitySmooth
		{
			get { return groundRelativeVelocitySmooth; }
		}

		public Vector3F GetLinearVelocity()
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

		public void GetBox( out Box box )
		{
			GetVolumeCapsule( out var capsule );
			var extents = new Vector3( capsule.Radius, capsule.Radius, ( capsule.Point2 - capsule.Point1 ).Length() * 0.5 + capsule.Radius );
			box = new Box( capsule.GetCenter(), extents, TransformV.Rotation.ToMatrix3() );
		}

		void DebugDraw( Viewport viewport )
		{
			var renderer = viewport.Simple3DRenderer;
			GetBox( out var box );
			var points = box.ToPoints();

			renderer.AddArrow( points[ 0 ], points[ 1 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 1 ], points[ 2 ], -1 );
			renderer.AddArrow( points[ 3 ], points[ 2 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 3 ], points[ 0 ], -1 );

			renderer.AddArrow( points[ 4 ], points[ 5 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 5 ], points[ 6 ], -1 );
			renderer.AddArrow( points[ 7 ], points[ 6 ], 0, 0, true, 0 );
			renderer.AddLine( points[ 7 ], points[ 4 ], -1 );

			renderer.AddLine( points[ 0 ], points[ 4 ], -1 );
			renderer.AddLine( points[ 1 ], points[ 5 ], -1 );
			renderer.AddLine( points[ 2 ], points[ 6 ], -1 );
			renderer.AddLine( points[ 3 ], points[ 7 ], -1 );
		}

		void UpdateTransformVisualOverride()
		{
			//!!!!slowly

			var offset = GetSmoothCameraOffset();
			if( offset != Vector3.Zero )
			{
				var tr = TransformV;
				TransformVisualOverride = new Transform( tr.Position + GetSmoothCameraOffset(), tr.Rotation, tr.Scale );
			}
			else
				TransformVisualOverride = null;
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			EditorAllowRenderSelection = false;

			UpdateTransformVisualOverride();

			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			//!!!!

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;
				var scene = context.Owner.AttachedScene;

				if( scene != null && context.SceneDisplayDevelopmentDataInThisApplication && scene.DisplayPhysicalObjects )
				{
					typeCached.GetBodyFormInfo( crouching, out var height, out var walkUpHeight );//, out var fromPositionToFloorDistance );

					var renderer = context.Owner.Simple3DRenderer;
					var tr = TransformV;
					var scaleFactor = tr.Scale.MaxComponent();

					renderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );

					//object position
					renderer.AddSphere( new Sphere( tr.Position, .05f ), 16 );

					//stand up height
					{
						Vector3 pos = tr.Position - new Vector3( 0, 0, ( /*fromPositionToFloorDistance*/ -walkUpHeight ) * scaleFactor );
						renderer.AddLine( pos + new Vector3( .2f, 0, 0 ), pos - new Vector3( .2f, 0, 0 ) );
						renderer.AddLine( pos + new Vector3( 0, .2f, 0 ), pos - new Vector3( 0, .2f, 0 ) );
					}

					//eye position
					renderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );
					var eyePosition = TransformV * typeCached.EyePosition.Value;
					renderer.AddArrow( eyePosition, eyePosition + new Vector3( typeCached.Height.Value * 0.05, 0, 0 ) );
					//renderer.AddSphere( new Sphere( TransformV * typeCached.EyePosition.Value, .05f ), 16 );
				}

				var showLabels = /*show &&*/ PhysicalBody == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;

#if !DEPLOY
				//draw selection
				if( EngineApp.IsEditor && PhysicalBody != null )
				{
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
					{
						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else //if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.CanSelectColor;
						//else
						//	color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;

						var viewport = context.Owner;

						viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
						DebugDraw( viewport );
					}
				}
#endif

				if( debugRays != null && debugRays.Count != 0 )
				{
					foreach( var ray in debugRays )
					{
						var renderer = context.Owner.Simple3DRenderer;
						renderer.SetColor( new ColorValue( 1, 0, 0 ) );
						renderer.AddArrow( ray.Origin, ray.GetEndPoint(), 0, 0, true, 0 );
					}
				}

				//if( _tempDebug.Count != 0 )
				//{
				//	var meshInSpace = GetComponent<MeshInSpace>( onlyEnabledInHierarchy: true );
				//	if( meshInSpace != null )
				//	{
				//		foreach( var p in _tempDebug )
				//		{
				//			var viewport = context.Owner;
				//			viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );

				//			var pp = meshInSpace.TransformV * p;

				//			viewport.Simple3DRenderer.AddSphere( new Sphere( pp, 0.01 ) );
				//		}
				//	}
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

			//{
			//	var renderer = context.viewport.Simple3DRenderer;
			//	renderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );
			//	foreach( var c in GetVolumeCapsules() )
			//	{
			//		renderer.AddCapsule( c );
			//	}
			//}



			////!!!!temp
			//if( PhysicalBody != null )
			//{
			//	var position = TransformV.Position + new Vector3( 2, 0, 0 );

			//	GetVolumeCapsule( out var capsule );
			//	//make radius smaller
			//	capsule.Radius *= .99f;

			//	var offset = position - TransformV.Position;//capsule.GetCenter();
			//	var checkCapsule = capsule;
			//	CapsuleAddOffset( ref checkCapsule, ref offset );


			//	var viewport = context.Owner;
			//	var renderer = viewport.Simple3DRenderer;
			//	GetBox( out var box );
			//	var points = box.ToPoints();

			//	renderer.SetColor( new ColorValue( 1, 0, 0 ) );
			//	renderer.AddCapsule( checkCapsule );


			//	//IsFreePositionToMove(TransformV.Position + new Vector3(2,0,0))
			//}
		}

		void TickWiggleWhenWalkingSpeedFactor()
		{
			float destinationFactor;
			if( IsOnGround() )
			{
				destinationFactor = (float)GroundRelativeVelocitySmooth.Length() * .3f;
				if( destinationFactor < 0.1f ) //0.5f
					destinationFactor = 0;
				if( destinationFactor > 1 )
					destinationFactor = 1;
			}
			else
				destinationFactor = 0;

			float step = Time.SimulationDelta/* RendererWorld.Instance.FrameRenderTimeStep*/ * 5;
			if( wiggleWhenWalkingSpeedFactor < destinationFactor )
			{
				wiggleWhenWalkingSpeedFactor += step;
				if( wiggleWhenWalkingSpeedFactor > destinationFactor )
					wiggleWhenWalkingSpeedFactor = destinationFactor;
			}
			else
			{
				wiggleWhenWalkingSpeedFactor -= step;
				if( wiggleWhenWalkingSpeedFactor < destinationFactor )
					wiggleWhenWalkingSpeedFactor = destinationFactor;
			}
		}

		void TickSmoothCameraOffset()
		{
			if( smoothCameraOffsetZ < 0 )
			{
				var speed = typeCached.Height.Value * 0.75;

				smoothCameraOffsetZ += Time.SimulationDelta * speed;

				if( smoothCameraOffsetZ > 0 )
					smoothCameraOffsetZ = 0;
			}
		}

		Vector3 GetSmoothCameraOffset()
		{
			return new Vector3( 0, 0, smoothCameraOffsetZ );
		}

		public bool GetEyesPosition( out Vector3 position )
		{
			var controller = GetAnimationController();
			if( controller != null && controller.Bones != null )
			{
				var bones = new List<SkeletonBone>();
				foreach( var bone in controller.Bones )
				{
					if( bone.Name.Contains( "eye", StringComparison.OrdinalIgnoreCase ) )
						bones.Add( bone );
				}

				if( bones.Count != 0 )
				{
					position = Vector3.Zero;

					UpdateTransformVisualOverride();
					var tr = TransformVisualOverride ?? TransformV;

					foreach( var bone in bones )
					{
						var boneIndex = controller.GetBoneIndex( bone );
						Matrix4F globalMatrix = Matrix4F.Zero;
						controller.GetBoneGlobalTransform( boneIndex, ref globalMatrix );

						var m = tr.ToMatrix4() * globalMatrix;

						position += m.GetTranslation();
					}

					position /= bones.Count;

					return true;
				}
			}

			position = Vector3.Zero;
			return false;
		}

		public virtual void GetFirstPersonCameraPosition( bool useEyesPositionOfModel, out Vector3 position, out Vector3 forward, out Vector3 up )
		{
			position = Vector3.Zero;

			UpdateTransformVisualOverride();
			var tr = TransformVisualOverride ?? TransformV;

			var positionCalculated = false;

			//get eyes position from skeleton
			if( useEyesPositionOfModel )
			{
				if( GetEyesPosition( out position ) )
					positionCalculated = true;
			}

			//calculate position
			if( !positionCalculated )
			{
				position = tr * typeCached.EyePosition.Value + GetSmoothCameraOffset();

				//if( CrouchingSupport )
				//{
				//	if( ( crouching && crouchingVisualFactor != 1 ) || ( !crouching && crouchingVisualFactor != 0 ) )
				//	{
				//		float diff = Type.HeightFromPositionToGround - Type.CrouchingHeightFromPositionToGround;
				//		if( !crouching )
				//			position -= new Vector3( 0, 0, diff * crouchingVisualFactor );
				//		else
				//			position += new Vector3( 0, 0, diff * ( 1.0f - crouchingVisualFactor ) );
				//	}
				//}

				//wiggle camera when walking
				var angle = Time.Current * 10;
				var radius = wiggleWhenWalkingSpeedFactor * .04f;
				Vector3 localPosition = new Vector3( 0, Math.Cos( angle ) * radius, Math.Abs( Math.Sin( angle ) * radius ) );
				position += localPosition * tr.ToMatrix4( false, true, true );//GetInterpolatedRotation();
			}

			//calculate up vector. wiggle camera when walking
			{
				var angle = Time.Current * 20;
				var radius = wiggleWhenWalkingSpeedFactor * .003f;
				Vector3 localUp = new Vector3( Math.Cos( angle ) * radius, Math.Sin( angle ) * radius, 1 );
				localUp.Normalize();
				up = localUp * tr.Rotation;//GetInterpolatedRotation();
			}

			//calculate forward vector
			forward = CurrentTurnToDirection.GetVector();
		}

		//public Vector3 GetSmoothPosition()
		//{
		//	return TransformV.Position + GetSmoothCameraOffset();
		//}

		public Transform GetCenteredTransform()
		{
			var tr = TransformV;
			var offset = new Vector3( 0, 0, typeCached.Height * 0.5 );
			return new Transform( tr.Position + tr.Rotation * offset, tr.Rotation, tr.Scale );
		}

		//public Vector3 GetCenteredPosition()
		//{
		//	return TransformV.Position + new Vector3( 0, 0, typeCached.Height * 0.5 );
		//}

		public Vector3 GetCenteredSmoothPosition()
		{
			return TransformV.Position + new Vector3( 0, 0, typeCached.Height * 0.5 ) + GetSmoothCameraOffset();
		}

		//void UpdateTransformOffsetInSimulation()
		//{
		//var meshInSpace = GetComponent<MeshInSpace>( onlyEnabledInHierarchy: true );
		//if( meshInSpace != null )
		//{
		//	var transformOffset = meshInSpace.GetComponent<TransformOffset>( onlyEnabledInHierarchy: true );
		//	if( transformOffset != null )
		//	{
		//		if( initialTransformOffsetPositionInSimulation == null )
		//			initialTransformOffsetPositionInSimulation = transformOffset.PositionOffset;

		//		transformOffset.PositionOffset = initialTransformOffsetPositionInSimulation.Value + GetSmoothCameraOffset();
		//	}
		//}
		//}

		public MeshInSpaceAnimationController GetAnimationController()
		{
			if( animationControllerCached == null )
				animationControllerCached = GetComponent<MeshInSpaceAnimationController>( onlyEnabledInHierarchy: true );
			return animationControllerCached;
		}

		//!!!!
		void OnAnimateChanged()
		{
			//reset
			if( !typeCached.Animate )
			{
				var controller = GetAnimationController();
				if( controller != null )
				{
					controller.PlayAnimation = null;
					controller.Speed = 1;
					controller.SetAnimationState( null, false );
				}
			}
		}

		void GetHandTransform( HandEnum hand, out double factor, out Transform transform )
		{
			//override for weapon
			var item = ItemGetEnabledFirst();
			if( item != null )
			{
				var obj = item as ObjectInSpace;
				if( obj != null )
				{
					var weapon = item as Weapon;
					if( weapon != null )
					{
						var tr = hand == HandEnum.Left ? weapon.WeaponTypeCached.LeftHandTransform.Value : weapon.WeaponTypeCached.RightHandTransform.Value;
						if( tr != NeoAxis.Transform.Zero )
						{
							factor = 1;
							transform = weapon.GetHandWorldTransform( hand );
							return;
						}
					}
				}
			}

			factor = hand == HandEnum.Left ? LeftHandFactor.Value : RightHandFactor.Value;
			transform = hand == HandEnum.Left ? LeftHandTransform.Value : RightHandTransform.Value;
		}

		////!!!!temp
		//List<Vector3> _tempDebug = new List<Vector3>();

		protected virtual void AdditionalBoneTransformsUpdate( MeshInSpaceAnimationController controller, MeshInSpaceAnimationController.AnimationStateClass animationState, Skeleton skeleton, SkeletonAnimationTrack.CalculateBoneTransformsItem[] result, ref bool updateTwice, int updateIteration )
		{
			//!!!!too hardcoded

			////!!!!
			//_tempDebug.Clear();

			//var meshInSpace = GetComponent<MeshInSpace>( onlyEnabledInHierarchy: true );
			//if( meshInSpace == null )
			//	return;
			if( controller.Bones == null )
				return;

			var inverseTransformCalculated = false;
			Matrix4 inverseTransform = Matrix4.Zero;

			//hands
			for( var hand = HandEnum.Left; hand <= HandEnum.Right; hand++ )
			{
				GetHandTransform( hand, out var factor, out var worldHandTransform );
				//var factor = hand == HandEnum.Left ? LeftHandFactor.Value : RightHandFactor.Value;
				if( factor > 0 )
				{
					//update skeleton twice because during calculation the data of bone transforms taken from previous update
					updateTwice = true;

					if( updateIteration == 1 )
					{
						//var worldHandTransform = hand == HandEnum.Left ? LeftHandTransform.Value : RightHandTransform.Value;

						//inverseTransform
						if( !inverseTransformCalculated )
						{
							/*meshInSpace.*/
							TransformV.ToMatrix4().GetInverse( out inverseTransform );
							inverseTransformCalculated = true;
						}

						var objectHandPosition = inverseTransform * worldHandTransform.Position;
						var objectHandRotation = Quaternion.LookAt( inverseTransform * worldHandTransform.Rotation.GetForward(), inverseTransform * worldHandTransform.Rotation.GetUp() );

						//!!!!
						//_tempDebug.Add( localHandPosition );


						var handBoneIndex = controller.GetBoneIndex( hand == HandEnum.Left ? typeCached.LeftHandBone : typeCached.RightHandBone );
						if( handBoneIndex >= 0 && handBoneIndex < result.Length )
						{
							var handBoneComponent = controller.Bones[ handBoneIndex ];

							var foreArmBoneComponent = handBoneComponent.Parent as SkeletonBone;
							if( foreArmBoneComponent != null )
							{
								var foreArmBoneIndex = controller.GetBoneIndex( foreArmBoneComponent.Name );
								if( foreArmBoneIndex >= 0 && foreArmBoneIndex < result.Length )
								{
									var armBoneComponent = foreArmBoneComponent.Parent as SkeletonBone;
									if( armBoneComponent != null )
									{
										var armBoneIndex = controller.GetBoneIndex( armBoneComponent.Name );
										if( armBoneIndex >= 0 && armBoneIndex < result.Length )
										{
											var shoulderBoneComponent = armBoneComponent.Parent as SkeletonBone;
											if( shoulderBoneComponent != null )
											{
												var shoulderBoneIndex = controller.GetBoneIndex( shoulderBoneComponent.Name );
												if( shoulderBoneIndex >= 0 && shoulderBoneIndex < result.Length )
												{
													ref var handBone = ref result[ handBoneIndex ];
													ref var foreArmBone = ref result[ foreArmBoneIndex ];
													ref var armBone = ref result[ armBoneIndex ];
													ref var shoulderBone = ref result[ shoulderBoneIndex ];

													Matrix4F handBoneMatrix = Matrix4F.Identity;
													Matrix4F foreArmBoneMatrix = Matrix4F.Identity;
													Matrix4F armBoneMatrix = Matrix4F.Identity;
													Matrix4F shoulderBoneMatrix = Matrix4F.Identity;

													if( controller.GetBoneGlobalTransform( handBoneIndex, ref handBoneMatrix ) &&
														controller.GetBoneGlobalTransform( foreArmBoneIndex, ref foreArmBoneMatrix ) &&
														controller.GetBoneGlobalTransform( armBoneIndex, ref armBoneMatrix ) &&
														controller.GetBoneGlobalTransform( shoulderBoneIndex, ref shoulderBoneMatrix ) )
													{
														var handBonePosition = handBoneMatrix.GetTranslation();
														var foreArmBonePosition = foreArmBoneMatrix.GetTranslation();
														var armBonePosition = armBoneMatrix.GetTranslation();
														//var shoulderBonePosition = shoulderBoneMatrix.GetTranslation();

														Vector3 requiredForeArmBonePosition;
														{
															var bone1Length = ( foreArmBonePosition - armBonePosition ).Length();
															var bone2Length = ( handBonePosition - foreArmBonePosition ).Length();
															var totalLength = bone1Length + bone2Length;
															var requiredLength = ( objectHandPosition - armBonePosition ).Length();

															if( requiredLength >= totalLength )
															{
																//flat
																requiredForeArmBonePosition = ( armBonePosition + objectHandPosition ) * 0.5;
															}
															else
															{
																//bend

																var a = requiredLength;
																var b = bone1Length;
																var c = bone2Length;
																var p = 0.5 * ( a + b + c );
																var h = ( 2.0 * Math.Sqrt( p * ( p - a ) * ( p - b ) * ( p - c ) ) ) / a;

																//h = b * sin(yAngle)
																var yAngle = Math.Asin( h / b );

																var dir = ( objectHandPosition - armBonePosition ).GetNormalize();
																var sphericalDir = SphericalDirection.FromVector( dir );
																sphericalDir.Vertical -= yAngle;

																requiredForeArmBonePosition = armBonePosition + sphericalDir.GetVector().GetNormalize() * bone1Length;
															}

															//_tempDebug.Add( requiredForeArmBonePosition );
														}

														//_tempDebug.Add( foreArmBonePosition );

														//_tempDebug.Add( armBonePosition );
														//_tempDebug.Add( requiredForeArmBonePosition );
														//_tempDebug.Add( localHandPosition );

														QuaternionF armRotationOffset;

														//calculate arm bone
														{
															var dir = ( requiredForeArmBonePosition - armBonePosition ).GetNormalize();

															armBoneMatrix.Decompose( out _, out QuaternionF objectArmRotation, out _ );

															shoulderBoneMatrix.Decompose( out _, out QuaternionF objectShoulderRotation, out _ );
															var objectShoulderRotationInv = objectShoulderRotation.GetInverse();

															//!!!!take into account the starting angles. now hands are always down, idle

															var sphericalDir = SphericalDirection.FromVector( dir );

															armRotationOffset =
																QuaternionF.FromRotateByZ( (float)-sphericalDir.Horizontal ) *
																QuaternionF.FromRotateByY( (float)sphericalDir.Vertical + MathEx.PI / 2 );

															var rot = armRotationOffset * objectArmRotation;

															var newRotation = objectShoulderRotationInv * rot;
															armBone.Rotation = QuaternionF.Slerp( armBone.Rotation, newRotation, (float)factor );
														}

														QuaternionF foreArmRotationOffset;

														//calculate fore arm bone
														{
															var dir = ( objectHandPosition - requiredForeArmBonePosition ).GetNormalize();

															foreArmBoneMatrix.Decompose( out _, out QuaternionF objectForeArmRotation, out _ );

															armBoneMatrix.Decompose( out _, out QuaternionF objectArmRotation, out _ );
															var objectArmRotationInv = objectArmRotation.GetInverse();

															//!!!!take into account the starting angles. now hands are always down, idle

															var sphericalDir = SphericalDirection.FromVector( dir );

															foreArmRotationOffset = armRotationOffset.GetInverse() *
																QuaternionF.FromRotateByZ( (float)-sphericalDir.Horizontal ) *
																QuaternionF.FromRotateByY( (float)sphericalDir.Vertical + MathEx.PI / 2 );

															var rot = foreArmRotationOffset * objectForeArmRotation;

															//var rot = armRotationOffset.GetInverse() *
															//	QuaternionF.FromRotateByZ( (float)-sphericalDir.Horizontal ) *
															//	QuaternionF.FromRotateByY( (float)sphericalDir.Vertical + MathEx.PI / 2 ) *
															//	objectForeArmRotation;

															var newRotation = objectArmRotationInv * rot;
															foreArmBone.Rotation = QuaternionF.Slerp( foreArmBone.Rotation, newRotation, (float)factor );
														}

														////calculate hand bone
														//{
														//	handBoneMatrix.Decompose( out _, out QuaternionF objectHandRotationM, out _ );

														//	foreArmBoneMatrix.Decompose( out _, out QuaternionF objectForeArmRotation, out _ );
														//	var objectForeArmRotationInv = objectForeArmRotation.GetInverse();

														//	var rot = foreArmRotationOffset.GetInverse() *
														//		//QuaternionF.FromRotateByY( (float)MathEx.PI / 2 ) *
														//		QuaternionF.FromRotateByZ( (float)EngineApp.EngineTime ) *
														//		objectHandRotationM;

														//	//var rot = foreArmRotationOffset * objectHandRotationM;

														//	var newRotation = objectForeArmRotationInv * rot;
														//	handBone.Rotation = QuaternionF.Slerp( handBone.Rotation, newRotation, (float)factor );

														//	//handBone.Rotation = QuaternionF.FromRotateByX( (float)MathEx.PI / 2 );

														//	//handBone.Rotation = QuaternionF.FromRotateByX( (float)EngineApp.EngineTime );
														//}




														////calculate arm bone
														//{
														//	var dir = ( requiredForeArmBonePosition - armBonePosition ).GetNormalize();

														//	shoulderBoneMatrix.Decompose( out _, out QuaternionF shoulderRot, out _ );
														//	var dirLocal = shoulderRot.GetInverse() * dir.ToVector3F();
														//	if( !left )
														//		dirLocal = -dirLocal;

														//	var upLocal = shoulderRot.GetInverse() * ( new Vector3F( 0, left ? -1 : 1, 1 ).GetNormalize() );

														//	var newRotation = QuaternionF.LookAt( dirLocal, upLocal );
														//	armBone.Rotation = QuaternionF.Slerp( armBone.Rotation, newRotation, (float)factor );
														//}

														////calculate fore arm bone
														//{
														//	var dir = ( localHandPosition - foreArmBonePosition ).GetNormalize();

														//	armBoneMatrix.Decompose( out _, out QuaternionF armRot, out _ );
														//	var dirLocal = armRot.GetInverse() * dir.ToVector3F();
														//	if( !left )
														//		dirLocal = -dirLocal;

														//	var upLocal = armRot.GetInverse() * ( new Vector3F( 0, left ? -1 : 1, 1 ).GetNormalize() );

														//	var newRotation = QuaternionF.LookAt( dirLocal, upLocal );
														//	foreArmBone.Rotation = QuaternionF.Slerp( foreArmBone.Rotation, newRotation, (float)factor );
														//}

													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}

			//head
			{
				var factor = HeadFactor.Value;
				if( factor > 0 )
				{
					//update skeleton twice because during calculation the data of bone transforms taken from previous update
					updateTwice = true;

					if( updateIteration == 1 )
					{
						var worldLookAt = HeadLookAt.Value;

						//inverseTransform
						if( !inverseTransformCalculated )
						{
							/*meshInSpace.*/
							TransformV.ToMatrix4().GetInverse( out inverseTransform );
							inverseTransformCalculated = true;
						}

						var localLookAt = inverseTransform * worldLookAt;

						//!!!!
						//_tempDebug.Add( localHandPosition );


						var headBoneIndex = controller.GetBoneIndex( typeCached.HeadBone );
						if( headBoneIndex >= 0 && headBoneIndex < result.Length )
						{
							var headBoneComponent = controller.Bones[ headBoneIndex ];

							var neckBoneComponent = headBoneComponent.Parent as SkeletonBone;
							if( neckBoneComponent != null )
							{
								var neckBoneIndex = controller.GetBoneIndex( neckBoneComponent.Name );
								if( neckBoneIndex >= 0 && neckBoneIndex < result.Length )
								{
									ref var headBone = ref result[ headBoneIndex ];
									ref var neckBone = ref result[ neckBoneIndex ];

									Matrix4F headBoneMatrix = Matrix4F.Identity;
									Matrix4F neckBoneMatrix = Matrix4F.Identity;

									if( controller.GetBoneGlobalTransform( headBoneIndex, ref headBoneMatrix ) &&
										controller.GetBoneGlobalTransform( neckBoneIndex, ref neckBoneMatrix ) )
									{
										var headBonePosition = headBoneMatrix.GetTranslation();
										//var neckBonePosition = neckBoneMatrix.GetTranslation();


										//!!!!simple implementation

										var dir = ( localLookAt - headBonePosition ).GetNormalize();
										var sphericalDir = SphericalDirectionF.FromVector( dir.ToVector3F() );

										var newRotation = QuaternionF.FromRotateByY( -sphericalDir.Horizontal ) * QuaternionF.FromRotateByX( sphericalDir.Vertical );


										//var rot = QuaternionF.LookAt( dir.ToVector3F(), new Vector3F( 0, 0, 1 ) );

										//var newRotation = rot * QuaternionF.FromRotateByY( MathEx.PI / 2 );// Quaternion.FromRotateByX( EngineApp.EngineTime ).ToQuaternionF();

										//headBoneMatrix.Decompose( out _, out QuaternionF headRot, out _ );
										//neckBoneMatrix.Decompose( out _, out QuaternionF neckRot, out _ );
										//var dirLocal = neckRot.GetInverse() * dir.ToVector3F();

										//neckRot *= QuaternionF.FromRotateByY( MathEx.PI / 2 ) * QuaternionF.FromRotateByX( MathEx.PI / 2 );
										//headRot *= QuaternionF.FromRotateByY( MathEx.PI / 2 ) * QuaternionF.FromRotateByX( MathEx.PI / 2 );

										//_tempDebug.Add( headBonePosition + neckRot.GetForward() );
										//_tempDebug.Add( headBonePosition + neckRot.GetUp() * 0.5f );

										//var newRotation = headBone.Rotation * neckRot.GetInverse() * rot;

										//var upLocal = neckRot.GetInverse() * ( new Vector3F( 0, 1, 0 ).GetNormalize() );

										//var newRotation = Quaternion.FromRotateByX( EngineApp.EngineTime ).ToQuaternionF();

										//var newRotation = neckRot.GetInverse() * QuaternionF.FromRotateByX( -MathEx.PI / 2 ) * Quaternion.FromRotateByY( -sphericalDir.Horizontal ).ToQuaternionF();


										headBone.Rotation = QuaternionF.Slerp( headBone.Rotation, newRotation, (float)factor );
									}
								}
							}
						}
					}
				}
			}

		}

		void TickAnimate( float delta )
		{
			//play one animation
			if( playOneAnimation != null )
			{
				playOneAnimationRemainingTime -= delta;
				if( playOneAnimationRemainingTime <= 0 )
					StartPlayOneAnimation( null );
			}

			if( typeCached.Animate )
			{
				var controller = GetAnimationController();
				if( controller != null )
				{
					Animation animation = null;
					double speed = 1;
					bool autoRewind = true;

					if( PhysicalBody != null )
					{
						if( IsOnGroundWithLatency() )
						{
							var localVelocity = TransformV.Rotation.GetInverse() * GetLinearVelocity();
							var linearSpeedNoScale = ( localVelocity.X + Math.Abs( localVelocity.Y ) * 0.5 ) / GetScaleFactor();

							//RunAnimation
							if( typeCached.Run )
							{
								var running = Math.Abs( linearSpeedNoScale ) > typeCached.RunForwardMaxSpeed * 0.6;
								if( running )
								{
									animation = typeCached.RunAnimation;
									if( animation != null )
										speed = typeCached.RunAnimationSpeed * linearSpeedNoScale;
								}
							}

							//WalkAnimation
							if( animation == null )
							{
								var walking = Math.Abs( linearSpeedNoScale ) > typeCached.WalkForwardMaxSpeed * 0.2;
								if( walking )
								{
									animation = typeCached.WalkAnimation;
									if( animation != null )
										speed = typeCached.WalkAnimationSpeed * linearSpeedNoScale;
								}
							}

							//Left Turn, Right Turn
							if( animation == null )
							{
								if( currentTurnToDirection.Horizontal != requiredTurnToDirection.Horizontal && IsOnGround() )
								{
									var angle = requiredTurnToDirection.Horizontal - currentTurnToDirection.Horizontal;
									var leftTurn = Math.Sin( angle ) > 0;

									animation = leftTurn ? typeCached.LeftTurnAnimation : typeCached.RightTurnAnimation;
								}
							}
						}
						else
							animation = typeCached.FlyAnimation;
					}

					if( animation == null )
						animation = typeCached.IdleAnimation;

					//play one animation
					if( playOneAnimation != null )
					{
						animation = playOneAnimation;
						speed = playOneAnimationSpeed;
						autoRewind = false;
					}

					//!!!!GC

					var state = new MeshInSpaceAnimationController.AnimationStateClass();
					state.Animations.Add( new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = animation, Speed = speed, AutoRewind = autoRewind } );

					state.AdditionalBoneTransformsUpdate = AdditionalBoneTransformsUpdate;


					//var headLookAt = HeadLookAt.Value;
					//if( headLookAt != null )
					//{
					//	state.HeadLookAt = headLookAt.TransformV.Position;
					//	state.HeadBone = HeadBone;
					//}

					//var leftHandTransform = LeftHandTransform.Value;
					//if( leftHandTransform != null )
					//{
					//	var tr = leftHandTransform.TransformV;
					//	state.LeftHandTransform = true;
					//	state.LeftHandPosition = tr.Position;
					//	state.LeftHandRotation = tr.Rotation;
					//	state.LeftHandBone = LeftHandBone;
					//}

					//var rightHandTransform = RightHandTransform.Value;
					//if( rightHandTransform != null )
					//{
					//	var tr = rightHandTransform.TransformV;
					//	state.RightHandTransform = true;
					//	state.RightHandPosition = tr.Position;
					//	state.RightHandRotation = tr.Rotation;
					//	state.RightHandBone = RightHandBone;
					//}

					//state.LeftHandThumbFingerFlexion = LeftHandThumbFingerFlexion;
					//state.LeftHandIndexFingerFlexion = LeftHandIndexFingerFlexion;
					//state.LeftHandMiddleFingerFlexion = LeftHandMiddleFingerFlexion;
					//state.LeftHandRingFingerFlexion = LeftHandRingFingerFlexion;
					//state.LeftHandPinkyFingerFlexion = LeftHandPinkyFingerFlexion;

					//state.RightHandThumbFingerFlexion = RightHandThumbFingerFlexion;
					//state.RightHandIndexFingerFlexion = RightHandIndexFingerFlexion;
					//state.RightHandMiddleFingerFlexion = RightHandMiddleFingerFlexion;
					//state.RightHandRingFingerFlexion = RightHandRingFingerFlexion;
					//state.RightHandPinkyFingerFlexion = RightHandPinkyFingerFlexion;

					//update controller

					controller.SetAnimationState( state, true );
					//controller.PlayAnimation = animation;
					//controller.Speed = speed;
				}
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			if( Components.Count == 0 )
			{
				var controller = CreateComponent<MeshInSpaceAnimationController>();
				controller.Name = "Animation Controller";

				//var inputProcessing = CreateComponent<CharacterInputProcessing>();
				//inputProcessing.Name = "Character Input Processing";

				//var characterAI = CreateComponent<CharacterAI>();
				//characterAI.Name = "Character AI";
				//characterAI.NetworkMode = NetworkModeEnum.False;
			}
		}

		public bool IsFreePositionToMove( Vector3 position )
		{
			if( PhysicalBody != null )
			{
				GetVolumeCapsule( out var capsule );
				//make radius smaller
				capsule.Radius *= .99f;

				var offset = position - TransformV.Position;//capsule.GetCenter();
				var checkCapsule = capsule;
				CapsuleAddOffset( ref checkCapsule, ref offset );

				var scene = ParentScene;
				if( scene != null )
				{
					var contactTestItem = new PhysicsVolumeTestItem( checkCapsule, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.All/*OneForEach*/ );
					ParentScene.PhysicsVolumeTest( contactTestItem );

					foreach( var item in contactTestItem.Result )
					{
						if( item.Body == PhysicalBody )
							continue;

						return false;
					}
				}
			}

			return true;
		}

		/////////////////////////////////////////

		/// <summary>
		/// Takes the item. The item will moved to the character and will disabled.
		/// </summary>
		/// <param name="item"></param>
		public void ItemTake( IGameFrameworkItem item )
		{
			var item2 = (ObjectInSpace)item;

			//check already taken
			if( item2.Parent == this )
				return;

			//disable
			item2.Enabled = false;

			//detach
			ObjectInSpaceUtility.Detach( item2 );
			item2.RemoveFromParent( false );

			var originalScale = item2.TransformV.Scale;

			//attach
			AddComponent( item2 );
			//!!!!
			item2.Transform = GetCenteredTransform();//Transform.Value;
			var transformOffset = ObjectInSpaceUtility.Attach( this, item2, TransformOffset.ModeEnum.Elements );
			if( transformOffset != null )
				transformOffset.ScaleOffset = originalScale / GetScaleFactor();
		}

		/// <summary>
		/// Drops the item. The item will moved to the scene and will enabled.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="newTransform"></param>
		public void ItemDrop( IGameFrameworkItem item, bool calculateTransform = false, Transform setTransform = null )
		{
			var item2 = (ObjectInSpace)item;

			//check can drop
			if( item2.Parent != this )
				return;

			//disable
			item2.Enabled = false;

			//detach
			ObjectInSpaceUtility.Detach( item2 );
			item2.RemoveFromParent( false );

			//add to the scene
			ParentScene.AddComponent( item2 );
			if( calculateTransform )
			{
				//it is simple implementation
				var scaleFactor = GetScaleFactor();

				item2.Transform = new Transform( TransformV.Position + new Vector3( 0, 0, 0.2 * scaleFactor /*- typeCached.PositionToGroundHeight * scaleFactor*/ ), TransformV.Rotation, item2.TransformV.Scale );
			}
			else if( setTransform != null )
				item2.Transform = setTransform;

			//enable
			item2.Enabled = true;
		}

		/// <summary>
		/// Activates the item. The item will enabled.
		/// </summary>
		/// <param name="item"></param>
		public void ItemActivate( IGameFrameworkItem item )
		{
			var item2 = (ObjectInSpace)item;
			item2.Enabled = true;
		}

		/// <summary>
		/// Deactivates the item. The item will disabled.
		/// </summary>
		/// <param name="item"></param>
		public void ItemDeactivate( IGameFrameworkItem item )
		{
			var item2 = (ObjectInSpace)item;
			item2.Enabled = false;
		}

		/// <summary>
		/// Returns first activated item of the character.
		/// </summary>
		/// <returns></returns>
		public IGameFrameworkItem ItemGetEnabledFirst()
		{
			foreach( var c in Components )
				if( c.Enabled && c is IGameFrameworkItem item )
					return item;
			return null;
		}

		void RotateWeaponIfCollided( Weapon weapon, TransformOffset offset, Degree rotationOffsetVertical )
		{
			var mesh = weapon.Mesh.Value;
			if( mesh == null || mesh.Result == null )
				return;

			////mesh length + offset
			var meshLength = mesh.Result.SpaceBounds.BoundingBox.Maximum.X;// * 1.5;// + 0.1;
			if( meshLength <= 0 )
				return;

			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			//!!!!check volume
			var weaponTransform = weapon.TransformV;
			var weaponDirection = weaponTransform.Rotation.GetForward();
			var ray = new Ray( weaponTransform.Position - weaponDirection * meshLength * 0.5, weaponDirection * meshLength * 1.5 );
			//var ray = new Ray( weaponTransform.Position, weaponTransform.Rotation.GetForward() * meshLength );

			//!!!!volume cast sweep

			var item = new PhysicsRayTestItem( ray, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
			scene.PhysicsRayTest( item );


			Degree? newAngle = null;

			for( int n = 0; n < item.Result.Length; n++ )//foreach( var resultItem in item.Result )
			{
				ref var resultItem = ref item.Result[ n ];

				if( PhysicalBody != null && resultItem.Body == PhysicalBody )
					continue;
				//what else to skip

				var length = ray.Direction.Length() * resultItem.DistanceScale;

				var factor = length / ( meshLength * 1.5 );
				if( factor < 1 )
				{
					var angle = 90 * ( 1.0 - factor );

					//!!!!

					newAngle = rotationOffsetVertical - angle;
					if( newAngle.Value < -90 )
						newAngle = -90;

					//if( rotationOffsetVertical > 0 )
					//{
					//}
					//else
					//{
					//}

					break;
				}
			}

			if( newAngle != null )
				offset.RotationOffset = Quaternion.FromRotateByY( newAngle.Value.InRadians() );
		}

		protected virtual void UpdateEnabledItemTransform( float delta )
		{
			//disable update for first person camera
			if( disableUpdateAttachedWeaponRemainingTime > 0 )
			{
				disableUpdateAttachedWeaponRemainingTime -= delta;
				if( disableUpdateAttachedWeaponRemainingTime < 0 )
					disableUpdateAttachedWeaponRemainingTime = 0;
			}
			else
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
							var weapon = item as Weapon;
							if( weapon != null )
							{
								var rotationOffet = CurrentTurnToDirection.Vertical + weapon.WeaponTypeCached.RotationOffsetWhenAttached.Value.InRadians();

								//!!!!
								var centeredOffset = new Vector3( 0, 0, typeCached.Height * 0.5 );

								offset.PositionOffset = centeredOffset + weapon.WeaponTypeCached.PositionOffsetWhenAttached;
								offset.RotationOffset = Quaternion.FromRotateByY( rotationOffet );

								RotateWeaponIfCollided( weapon, offset, rotationOffet.InDegrees() );

								//reset motion blur settings
								weapon.MotionBlurFactor = 1;
							}
						}
					}
				}
			}
		}

		public void UpdateDataForFirstPersonCamera( GameMode gameMode, Viewport viewport )
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
						var weapon = item as Weapon;
						if( weapon != null )
						{
							var positionOffset = weapon.WeaponTypeCached.PositionOffsetWhenAttachedFirstPerson.Value;
							var rotationOffset = weapon.WeaponTypeCached.RotationOffsetWhenAttachedFirstPerson.Value;

							//idle, walking
							{
								var maxOffset = 0.005;

								//!!!!impl when moving
								//if( IsOnGround() )
								//{
								//	var maxOffset2 = 0.01;
								//	var factor = MathEx.Saturate( GetLinearVelocity().Length() * 0.5 );
								//	maxOffset = MathEx.Lerp( maxOffset, maxOffset2, factor );
								//}

								positionOffset.Y += Math.Sin( Time.Current * 0.25 ) * maxOffset;
								positionOffset.Z += Math.Sin( Time.Current * 0.43 ) * maxOffset;
							}

							//add recoil offset
							{
								var maxRecoil = 0.0;

								for( int mode = 1; mode <= 2; mode++ )
								{
									if( weapon.IsFiring( mode ) )
									{
										var currentTime = weapon.GetFiringCurrentTime( mode );

										var firingTotalTime = mode == 1 ? weapon.WeaponTypeCached.Mode1FiringTotalTime : weapon.WeaponTypeCached.Mode2FiringTotalTime;
										var recoilOffset = mode == 1 ? weapon.WeaponTypeCached.Mode1MaxRecoilOffset : weapon.WeaponTypeCached.Mode2MaxRecoilOffset;
										var maxRecoilOffsetTime = mode == 1 ? weapon.WeaponTypeCached.Mode1MaxRecoilOffsetTime : weapon.WeaponTypeCached.Mode2MaxRecoilOffsetTime;

										var endTime = Math.Min( firingTotalTime, maxRecoilOffsetTime * 10 );

										var curve = new CurveCubicSpline1();//new CurveLine1();
										curve.AddPoint( 0, 0 );
										curve.AddPoint( maxRecoilOffsetTime, recoilOffset );
										curve.AddPoint( endTime, 0 );

										var recoil = curve.CalculateValueByTime( currentTime );
										if( recoil > maxRecoil )
											maxRecoil = recoil;
									}
								}

								positionOffset.X -= maxRecoil;
							}

							var cameraSettings = viewport.CameraSettings;
							var worldTransform = new Transform( cameraSettings.Position + cameraSettings.Rotation * positionOffset, cameraSettings.Rotation * new Angles( 0, rotationOffset, 0 ).ToQuaternion() );

							var localToCameraTransform = TransformV.ToMatrix4().GetInverse() * worldTransform.ToMatrix4();
							localToCameraTransform.Decompose( out var position, out Quaternion rotation, out var scale );

							offset.PositionOffset = position;
							offset.RotationOffset = rotation;

							RotateWeaponIfCollided( weapon, offset, new Radian( CurrentTurnToDirection.Vertical ).InDegrees() + rotationOffset );

							//update bounds
							weapon.SpaceBoundsUpdate();

							//disable motion blur
							weapon.MotionBlurFactor = 0;

							//!!!!how to do it better?
							//inform the character about first person update to disable default behaviour which doing in UpdateEnabledItemTransform()
							disableUpdateAttachedWeaponRemainingTime = 0.25;
						}
					}
				}
			}
		}

		public void SoundPlay( Sound sound )
		{
			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		public void StartPlayOneAnimation( Animation animation, double speed = 1.0 )
		{
			playOneAnimation = animation;
			playOneAnimationSpeed = speed;
			if( playOneAnimation != null && playOneAnimationSpeed == 0 )
				playOneAnimationSpeed = 0.000001;

			if( playOneAnimation != null )
			{
				playOneAnimationRemainingTime = playOneAnimation.Length / playOneAnimationSpeed;

				var controller = GetAnimationController();
				if( controller != null && playOneAnimationRemainingTime > controller.InterpolationTime.Value )
					playOneAnimationRemainingTime -= controller.InterpolationTime.Value;
			}
			else
				playOneAnimationRemainingTime = 0;
		}

		[Browsable( false )]
		public Animation PlayOneAnimation
		{
			get { return playOneAnimation; }
		}

		[Browsable( false )]
		public double PlayOneAnimationSpeed
		{
			get { return playOneAnimationSpeed; }
		}

		[Browsable( false )]
		public double PlayOneAnimationRemainingTime
		{
			get { return playOneAnimationRemainingTime; }
		}

		void TickCurrentTurnToDirection()
		{
			if( currentTurnToDirection.Horizontal != requiredTurnToDirection.Horizontal && IsOnGround() )
			{
				var angle = requiredTurnToDirection.Horizontal - currentTurnToDirection.Horizontal;
				var leftTurn = Math.Sin( angle ) > 0;

				var step = (float)(double)typeCached.TurningSpeed.Value.InRadians() * Time.SimulationDelta;
				currentTurnToDirection.Horizontal += leftTurn ? step : -step;

				var newAngle = requiredTurnToDirection.Horizontal - currentTurnToDirection.Horizontal;
				var newLeftTurn = Math.Sin( newAngle ) > 0;

				if( newLeftTurn != leftTurn )
					currentTurnToDirection.Horizontal = requiredTurnToDirection.Horizontal;

				//!!!!new
				UpdateRotation();
			}
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "Jump" )
			{
				SoundPlay( typeCached.JumpSound );
				StartPlayOneAnimation( typeCached.JumpAnimation );
			}
			else if( message == "TickMovement" )
			{
				var forceIsOnGroundRemainingTime2 = reader.ReadSingle();
				var smoothCameraOffsetZ2 = reader.ReadSingle();
				if( !reader.Complete() )
					return false;

				forceIsOnGroundRemainingTime = forceIsOnGroundRemainingTime2;
				smoothCameraOffsetZ = smoothCameraOffsetZ2;
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
				if( message == "ItemTakeAndActivate" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null )
					{
						//!!!!check it can be taken

						var item2 = item as IGameFrameworkItem;
						if( item2 != null )
						{
							ItemTake( item2 );
							ItemActivate( item2 );
						}
					}
				}
				else if( message == "ItemDrop" )
				{
					var itemNetworkID = (long)reader.ReadVariableUInt64();
					if( !reader.Complete() )
						return false;

					var item = ParentRoot.HierarchyController.GetComponentByNetworkID( itemNetworkID );
					if( item != null )
					{
						//!!!!check it can be dropped

						var item2 = item as IGameFrameworkItem;
						if( item2 != null )
							ItemDrop( item2, true );
					}
				}

			}

			return true;
		}

		public delegate void ProcessDamageBeforeDelegate( Character sender, long whoFired, ref double damage, ref object anyData, ref bool handled );
		public event ProcessDamageBeforeDelegate ProcessDamageBefore;
		public static event ProcessDamageBeforeDelegate ProcessDamageBeforeAll;

		public delegate void ProcessDamageAfterDelegate( Character sender, long whoFired, double damage, object anyData, double oldHealth );
		public event ProcessDamageAfterDelegate ProcessDamageAfter;
		public static event ProcessDamageAfterDelegate ProcessDamageAfterAll;

		public void ProcessDamage( long whoFired, double damage, object anyData )
		{
			var oldHealth = Health.Value;

			var damage2 = damage;
			var anyData2 = anyData;
			var handled = false;
			ProcessDamageBefore?.Invoke( this, whoFired, ref damage2, ref anyData2, ref handled );
			ProcessDamageBeforeAll?.Invoke( this, whoFired, ref damage2, ref anyData2, ref handled );

			if( !handled )
			{
				var health = Health.Value;
				if( health > 0 )
				{
					Health = health - damage;

					if( Health.Value <= 0 )
						RemoveFromParent( true );
				}
			}

			ProcessDamageAfter?.Invoke( this, whoFired, damage2, anyData2, oldHealth );
			ProcessDamageAfterAll?.Invoke( this, whoFired, damage2, anyData2, oldHealth );
		}

		public virtual void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			GetComponent<AI>()?.ObjectInteractionGetInfo( gameMode, ref info );
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var ai = GetComponent<AI>();
			if( ai != null && ai.ObjectInteractionInputMessage( gameMode, message ) )
				return true;

			return false;
		}

		public virtual void ObjectInteractionEnter( ObjectInteractionContext context )
		{
			GetComponent<AI>()?.ObjectInteractionEnter( context );
		}

		public virtual void ObjectInteractionExit( ObjectInteractionContext context )
		{
			GetComponent<AI>()?.ObjectInteractionExit( context );
		}

		public virtual void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
			GetComponent<AI>()?.ObjectInteractionUpdate( context );
		}

		protected override void OnComponentRemoved( Component component )
		{
			base.OnComponentRemoved( component );

			if( animationControllerCached == component )
				animationControllerCached = null;
			//if( characterInputProcessingCached == component )
			//	characterInputProcessingCached = null;
		}
	}
}
