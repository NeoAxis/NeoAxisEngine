// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Basic character class.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character", -8999 )]
	[ResourceFileExtension( "character" )]
	[EditorDocumentWindow( typeof( Component_Character_Editor ), true )]
	public class Component_Character : Component_ObjectInSpace
	{
		Component_RigidBody mainBody;

		//on ground and flying states
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double mainBodyGroundDistanceNoScale = 1000;//from center of the body/object
		Component_RigidBody groundBody;
		double forceIsOnGroundRemainingTime;
		double disableGravityRemainingTime;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double onGroundTime;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double elapsedTimeSinceLastGroundContact;
		//Vector3 lastSimulationStepPosition;

		//!!!!smooth?
		Vector3? lastTransformPosition;
		Vector3 lastLinearVelocity;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double jumpInactiveTime;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		double jumpDisableRemainingTime;

		SphericalDirection currentTurnToDirection;
		SphericalDirection requiredTurnToDirection;
		//Radian horizontalDirectionForUpdateRotation;

		//moveVector
		int moveVectorTimer;//is disabled when equal 0
		Vector2 moveVector;
		bool moveVectorRun;
		Vector2 lastTickForceVector;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		//Vector3 linearVelocityForSerialization;

		Vector3 groundRelativeVelocity;
		Vector3[] groundRelativeVelocitySmoothArray;
		Vector3 groundRelativeVelocitySmooth;

		//damageFastChangeSpeed
		Vector3 damageFastChangeSpeedLastVelocity = new Vector3( float.NaN, float.NaN, float.NaN );

		double allowToSleepTime;

		//crouching
		const float crouchingVisualSwitchTime = .3f;
		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		bool crouching;
		//float crouchingSwitchRemainingTime;
		////[FieldSerialize( FieldSerializeSerializationTypes.World )]
		//float crouchingVisualFactor;

		//wiggle camera when walking
		float wiggleWhenWalkingSpeedFactor;

		//smooth camera
		double smoothCameraOffsetZ;
		Vector3? initialTransformOffsetPositionInSimulation;

		//play one animation
		Component_Animation playOneAnimation = null;
		double playOneAnimationSpeed = 1;
		double playOneAnimationRemainingTime;

		/////////////////////////////////////////
		//Basic

		/// <summary>
		/// The height of the character.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 1.8 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set
			{
				if( _height.BeginSet( ref value ) )
				{
					try
					{
						HeightChanged?.Invoke( this );
						UpdateCollisionBody();
					}
					finally { _height.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<Component_Character> HeightChanged;
		ReferenceField<double> _height = 1.8;

		/// <summary>
		/// The radius of the collision capsule.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 0.3 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set
			{
				if( _radius.BeginSet( ref value ) )
				{
					try
					{
						RadiusChanged?.Invoke( this );
						UpdateCollisionBody();
					}
					finally { _radius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<Component_Character> RadiusChanged;
		ReferenceField<double> _radius = 0.3;

		/// <summary>
		/// The height to which the character can rise. Set 0 to disable functionality of walking up.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 0.6 )]
		public Reference<double> WalkUpHeight
		{
			get { if( _walkUpHeight.BeginGet() ) WalkUpHeight = _walkUpHeight.Get( this ); return _walkUpHeight.value; }
			set { if( _walkUpHeight.BeginSet( ref value ) ) { try { WalkUpHeightChanged?.Invoke( this ); } finally { _walkUpHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkUpHeight"/> property value changes.</summary>
		public event Action<Component_Character> WalkUpHeightChanged;
		ReferenceField<double> _walkUpHeight = 0.6;

		/// <summary>
		/// The distance from the character position to the ground.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 1.15 )]
		public Reference<double> PositionToGroundHeight
		{
			get { if( _positionToGroundHeight.BeginGet() ) PositionToGroundHeight = _positionToGroundHeight.Get( this ); return _positionToGroundHeight.value; }
			set { if( _positionToGroundHeight.BeginSet( ref value ) ) { try { PositionToGroundHeightChanged?.Invoke( this ); } finally { _positionToGroundHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionToGroundHeight"/> property value changes.</summary>
		public event Action<Component_Character> PositionToGroundHeightChanged;
		ReferenceField<double> _positionToGroundHeight = 1.15;

		/// <summary>
		/// The mass of the character.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 70 )]
		public Reference<double> Mass
		{
			get { if( _mass.BeginGet() ) Mass = _mass.Get( this ); return _mass.value; }
			set { if( _mass.BeginSet( ref value ) ) { try { MassChanged?.Invoke( this ); } finally { _mass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mass"/> property value changes.</summary>
		public event Action<Component_Character> MassChanged;
		ReferenceField<double> _mass = 70;

		/// <summary>
		/// The maximum angle of the surface on which the character can stand.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 50 )]
		public Reference<Degree> MaxSlopeAngle
		{
			get { if( _maxSlopeAngle.BeginGet() ) MaxSlopeAngle = _maxSlopeAngle.Get( this ); return _maxSlopeAngle.value; }
			set { if( _maxSlopeAngle.BeginSet( ref value ) ) { try { MaxSlopeAngleChanged?.Invoke( this ); } finally { _maxSlopeAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxSlopeAngle"/> property value changes.</summary>
		public event Action<Component_Character> MaxSlopeAngleChanged;
		ReferenceField<Degree> _maxSlopeAngle = new Degree( 50 );

		/// <summary>
		/// The position of the eyes relative to the position of the character.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( "0.23 0 0.58" )]
		public Reference<Vector3> EyePosition
		{
			get { if( _eyePosition.BeginGet() ) EyePosition = _eyePosition.Get( this ); return _eyePosition.value; }
			set { if( _eyePosition.BeginSet( ref value ) ) { try { EyePositionChanged?.Invoke( this ); } finally { _eyePosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EyePosition"/> property value changes.</summary>
		public event Action<Component_Character> EyePositionChanged;
		ReferenceField<Vector3> _eyePosition = new Vector3( 0.23, 0, 0.58 );

		/// <summary>
		/// Whether to consider the speed of the body on which the character is standing.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( false )]
		public Reference<bool> ApplyGroundVelocity
		{
			get { if( _applyGroundVelocity.BeginGet() ) ApplyGroundVelocity = _applyGroundVelocity.Get( this ); return _applyGroundVelocity.value; }
			set { if( _applyGroundVelocity.BeginSet( ref value ) ) { try { ApplyGroundVelocityChanged?.Invoke( this ); } finally { _applyGroundVelocity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ApplyGroundVelocity"/> property value changes.</summary>
		public event Action<Component_Character> ApplyGroundVelocityChanged;
		ReferenceField<bool> _applyGroundVelocity = false;

		[Category( "Advanced" )]
		[DefaultValue( 1.0/*0.6*/ )]
		public Reference<double> MinSpeedToSleepBody
		{
			get { if( _minSpeedToSleepBody.BeginGet() ) MinSpeedToSleepBody = _minSpeedToSleepBody.Get( this ); return _minSpeedToSleepBody.value; }
			set { if( _minSpeedToSleepBody.BeginSet( ref value ) ) { try { MinSpeedToSleepBodyChanged?.Invoke( this ); } finally { _minSpeedToSleepBody.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MinSpeedToSleepBody"/> property value changes.</summary>
		public event Action<Component_Character> MinSpeedToSleepBodyChanged;
		ReferenceField<double> _minSpeedToSleepBody = 1.0;//0.6;

		/// <summary>
		/// The value of linear dumping when a character is standing on the ground.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( 5 )]
		public Reference<double> LinearDampingOnGroundIdle
		{
			get { if( _linearDampingOnGroundIdle.BeginGet() ) LinearDampingOnGroundIdle = _linearDampingOnGroundIdle.Get( this ); return _linearDampingOnGroundIdle.value; }
			set { if( _linearDampingOnGroundIdle.BeginSet( ref value ) ) { try { LinearDampingOnGroundIdleChanged?.Invoke( this ); } finally { _linearDampingOnGroundIdle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearDampingOnGroundIdle"/> property value changes.</summary>
		public event Action<Component_Character> LinearDampingOnGroundIdleChanged;
		ReferenceField<double> _linearDampingOnGroundIdle = 5;

		/// <summary>
		/// The value of linear dumping when a character is standing on the ground.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( 0.99 )]
		public Reference<double> LinearDampingOnGroundMove
		{
			get { if( _linearDampingOnGround.BeginGet() ) LinearDampingOnGroundMove = _linearDampingOnGround.Get( this ); return _linearDampingOnGround.value; }
			set { if( _linearDampingOnGround.BeginSet( ref value ) ) { try { LinearDampingOnGroundChanged?.Invoke( this ); } finally { _linearDampingOnGround.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearDampingOnGroundMove"/> property value changes.</summary>
		public event Action<Component_Character> LinearDampingOnGroundChanged;
		ReferenceField<double> _linearDampingOnGround = 0.99;

		/// <summary>
		/// The value of linear dumping when a character is flying.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( 0.15 )]
		public Reference<double> LinearDampingFly
		{
			get { if( _linearDampingFly.BeginGet() ) LinearDampingFly = _linearDampingFly.Get( this ); return _linearDampingFly.value; }
			set { if( _linearDampingFly.BeginSet( ref value ) ) { try { LinearDampingFlyChanged?.Invoke( this ); } finally { _linearDampingFly.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearDampingFly"/> property value changes.</summary>
		public event Action<Component_Character> LinearDampingFlyChanged;
		ReferenceField<double> _linearDampingFly = 0.15;

		/////////////////////////////////////////
		//Walk

		/// <summary>
		/// Maximum speed when walking forward.
		/// </summary>
		[Category( "Walk" )]
		[DefaultValue( 1.5 )]
		public Reference<double> WalkForwardMaxSpeed
		{
			get { if( _walkForwardMaxSpeed.BeginGet() ) WalkForwardMaxSpeed = _walkForwardMaxSpeed.Get( this ); return _walkForwardMaxSpeed.value; }
			set { if( _walkForwardMaxSpeed.BeginSet( ref value ) ) { try { WalkForwardMaxSpeedChanged?.Invoke( this ); } finally { _walkForwardMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkForwardMaxSpeed"/> property value changes.</summary>
		public event Action<Component_Character> WalkForwardMaxSpeedChanged;
		ReferenceField<double> _walkForwardMaxSpeed = 1.5;

		/// <summary>
		/// Maximum speed when walking backward.
		/// </summary>
		[Category( "Walk" )]
		[DefaultValue( 1.5 )]
		public Reference<double> WalkBackwardMaxSpeed
		{
			get { if( _walkBackwardMaxSpeed.BeginGet() ) WalkBackwardMaxSpeed = _walkBackwardMaxSpeed.Get( this ); return _walkBackwardMaxSpeed.value; }
			set { if( _walkBackwardMaxSpeed.BeginSet( ref value ) ) { try { WalkBackwardMaxSpeedChanged?.Invoke( this ); } finally { _walkBackwardMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkBackwardMaxSpeed"/> property value changes.</summary>
		public event Action<Component_Character> WalkBackwardMaxSpeedChanged;
		ReferenceField<double> _walkBackwardMaxSpeed = 1.5;

		/// <summary>
		/// Maximum speed when walking to a side.
		/// </summary>
		[Category( "Walk" )]
		[DefaultValue( 1.5 )]
		public Reference<double> WalkSideMaxSpeed
		{
			get { if( _walkSideMaxSpeed.BeginGet() ) WalkSideMaxSpeed = _walkSideMaxSpeed.Get( this ); return _walkSideMaxSpeed.value; }
			set { if( _walkSideMaxSpeed.BeginSet( ref value ) ) { try { WalkSideMaxSpeedChanged?.Invoke( this ); } finally { _walkSideMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkSideMaxSpeed"/> property value changes.</summary>
		public event Action<Component_Character> WalkSideMaxSpeedChanged;
		ReferenceField<double> _walkSideMaxSpeed = 1.5;

		/// <summary>
		/// Physical force applied to the body for walking.
		/// </summary>
		[Category( "Walk" )]
		[DefaultValue( 100000 )]
		public Reference<double> WalkForce
		{
			get { if( _walkForce.BeginGet() ) WalkForce = _walkForce.Get( this ); return _walkForce.value; }
			set { if( _walkForce.BeginSet( ref value ) ) { try { WalkForceChanged?.Invoke( this ); } finally { _walkForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkForce"/> property value changes.</summary>
		public event Action<Component_Character> WalkForceChanged;
		ReferenceField<double> _walkForce = 100000;

		/////////////////////////////////////////
		//Run

		/// <summary>
		/// Can the character run.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( false )]
		public Reference<bool> RunSupport
		{
			get { if( _runSupport.BeginGet() ) RunSupport = _runSupport.Get( this ); return _runSupport.value; }
			set { if( _runSupport.BeginSet( ref value ) ) { try { RunSupportChanged?.Invoke( this ); } finally { _runSupport.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunSupport"/> property value changes.</summary>
		public event Action<Component_Character> RunSupportChanged;
		ReferenceField<bool> _runSupport = false;

		/// <summary>
		/// Maximum speed when running forward.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( 5 )]
		public Reference<double> RunForwardMaxSpeed
		{
			get { if( _runForwardMaxSpeed.BeginGet() ) RunForwardMaxSpeed = _runForwardMaxSpeed.Get( this ); return _runForwardMaxSpeed.value; }
			set { if( _runForwardMaxSpeed.BeginSet( ref value ) ) { try { RunForwardMaxSpeedChanged?.Invoke( this ); } finally { _runForwardMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunForwardMaxSpeed"/> property value changes.</summary>
		public event Action<Component_Character> RunForwardMaxSpeedChanged;
		ReferenceField<double> _runForwardMaxSpeed = 5;

		/// <summary>
		/// Maximum speed when running backward.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( 5 )]
		public Reference<double> RunBackwardMaxSpeed
		{
			get { if( _runBackwardMaxSpeed.BeginGet() ) RunBackwardMaxSpeed = _runBackwardMaxSpeed.Get( this ); return _runBackwardMaxSpeed.value; }
			set { if( _runBackwardMaxSpeed.BeginSet( ref value ) ) { try { RunBackwardMaxSpeedChanged?.Invoke( this ); } finally { _runBackwardMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunBackwardMaxSpeed"/> property value changes.</summary>
		public event Action<Component_Character> RunBackwardMaxSpeedChanged;
		ReferenceField<double> _runBackwardMaxSpeed = 5;

		/// <summary>
		/// Maximum speed when running to a side.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( 5 )]
		public Reference<double> RunSideMaxSpeed
		{
			get { if( _runSideMaxSpeed.BeginGet() ) RunSideMaxSpeed = _runSideMaxSpeed.Get( this ); return _runSideMaxSpeed.value; }
			set { if( _runSideMaxSpeed.BeginSet( ref value ) ) { try { RunSideMaxSpeedChanged?.Invoke( this ); } finally { _runSideMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunSideMaxSpeed"/> property value changes.</summary>
		public event Action<Component_Character> RunSideMaxSpeedChanged;
		ReferenceField<double> _runSideMaxSpeed = 5;

		/// <summary>
		/// Physical force applied to the body for running.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( 150000 )]
		public Reference<double> RunForce
		{
			get { if( _runForce.BeginGet() ) RunForce = _runForce.Get( this ); return _runForce.value; }
			set { if( _runForce.BeginSet( ref value ) ) { try { RunForceChanged?.Invoke( this ); } finally { _runForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunForce"/> property value changes.</summary>
		public event Action<Component_Character> RunForceChanged;
		ReferenceField<double> _runForce = 150000;

		/////////////////////////////////////////
		//Turning

		/// <summary>
		/// The speed of rotation of the character around its axis.
		/// </summary>
		[Category( "Turning" )]
		[DefaultValue( 90.0 )]
		public Reference<Degree> TurningSpeed
		{
			get { if( _turningSpeed.BeginGet() ) TurningSpeed = _turningSpeed.Get( this ); return _turningSpeed.value; }
			set { if( _turningSpeed.BeginSet( ref value ) ) { try { TurningSpeedChanged?.Invoke( this ); } finally { _turningSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TurningSpeed"/> property value changes.</summary>
		public event Action<Component_Character> TurningSpeedChanged;
		ReferenceField<Degree> _turningSpeed = new Degree( 90.0 );

		/////////////////////////////////////////
		//Fly control

		/// <summary>
		/// Can a character control himself in flight.
		/// </summary>
		[Category( "Fly Control" )]
		[DefaultValue( false )]
		public Reference<bool> FlyControlSupport
		{
			get { if( _flyControlSupport.BeginGet() ) FlyControlSupport = _flyControlSupport.Get( this ); return _flyControlSupport.value; }
			set { if( _flyControlSupport.BeginSet( ref value ) ) { try { FlyControlSupportChanged?.Invoke( this ); } finally { _flyControlSupport.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyControlSupport"/> property value changes.</summary>
		public event Action<Component_Character> FlyControlSupportChanged;
		ReferenceField<bool> _flyControlSupport = false;

		/// <summary>
		/// Maximum speed of character control in flight.
		/// </summary>
		[Category( "Fly Control" )]
		[DefaultValue( 10 )]
		public Reference<double> FlyControlMaxSpeed
		{
			get { if( _flyControlMaxSpeed.BeginGet() ) FlyControlMaxSpeed = _flyControlMaxSpeed.Get( this ); return _flyControlMaxSpeed.value; }
			set { if( _flyControlMaxSpeed.BeginSet( ref value ) ) { try { FlyControlMaxSpeedChanged?.Invoke( this ); } finally { _flyControlMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyControlMaxSpeed"/> property value changes.</summary>
		public event Action<Component_Character> FlyControlMaxSpeedChanged;
		ReferenceField<double> _flyControlMaxSpeed = 10;

		/// <summary>
		/// Physical force applied to the body for flying.
		/// </summary>
		[Category( "Fly Control" )]
		[DefaultValue( 10000 )]
		public Reference<double> FlyControlForce
		{
			get { if( _flyControlForce.BeginGet() ) FlyControlForce = _flyControlForce.Get( this ); return _flyControlForce.value; }
			set { if( _flyControlForce.BeginSet( ref value ) ) { try { FlyControlForceChanged?.Invoke( this ); } finally { _flyControlForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyControlForce"/> property value changes.</summary>
		public event Action<Component_Character> FlyControlForceChanged;
		ReferenceField<double> _flyControlForce = 10000;

		/////////////////////////////////////////
		//Jump

		/// <summary>
		/// Can the character jump.
		/// </summary>
		[Category( "Jump" )]
		[DefaultValue( false )]
		public Reference<bool> JumpSupport
		{
			get { if( _jumpSupport.BeginGet() ) JumpSupport = _jumpSupport.Get( this ); return _jumpSupport.value; }
			set { if( _jumpSupport.BeginSet( ref value ) ) { try { JumpSupportChanged?.Invoke( this ); } finally { _jumpSupport.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpSupport"/> property value changes.</summary>
		public event Action<Component_Character> JumpSupportChanged;
		ReferenceField<bool> _jumpSupport = false;

		/// <summary>
		/// The vertical speed of a jump.
		/// </summary>
		[Category( "Jump" )]
		[DefaultValue( 4 )]
		public Reference<double> JumpSpeed
		{
			get { if( _jumpSpeed.BeginGet() ) JumpSpeed = _jumpSpeed.Get( this ); return _jumpSpeed.value; }
			set { if( _jumpSpeed.BeginSet( ref value ) ) { try { JumpSpeedChanged?.Invoke( this ); } finally { _jumpSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpSpeed"/> property value changes.</summary>
		public event Action<Component_Character> JumpSpeedChanged;
		ReferenceField<double> _jumpSpeed = 4;

		[Category( "Jump" )]
		[DefaultValue( null )]
		public Reference<Component_Sound> JumpSound
		{
			get { if( _jumpSound.BeginGet() ) JumpSound = _jumpSound.Get( this ); return _jumpSound.value; }
			set { if( _jumpSound.BeginSet( ref value ) ) { try { JumpSoundChanged?.Invoke( this ); } finally { _jumpSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpSound"/> property value changes.</summary>
		public event Action<Component_Character> JumpSoundChanged;
		ReferenceField<Component_Sound> _jumpSound = null;

		/////////////////////////////////////////
		//Crouching

		//!!!!crouching is disabled
		[Browsable( false )]
		[Category( "Crouching" )]
		[DefaultValue( false )]
		public Reference<bool> CrouchingSupport
		{
			get { if( _crouchingSupport.BeginGet() ) CrouchingSupport = _crouchingSupport.Get( this ); return _crouchingSupport.value; }
			set { if( _crouchingSupport.BeginSet( ref value ) ) { try { CrouchingSupportChanged?.Invoke( this ); } finally { _crouchingSupport.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingSupport"/> property value changes.</summary>
		public event Action<Component_Character> CrouchingSupportChanged;
		ReferenceField<bool> _crouchingSupport = false;

		[Category( "Crouching" )]
		[DefaultValue( 1.0 )]
		public Reference<double> CrouchingHeight
		{
			get { if( _crouchingHeight.BeginGet() ) CrouchingHeight = _crouchingHeight.Get( this ); return _crouchingHeight.value; }
			set { if( _crouchingHeight.BeginSet( ref value ) ) { try { CrouchingHeightChanged?.Invoke( this ); } finally { _crouchingHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingHeight"/> property value changes.</summary>
		public event Action<Component_Character> CrouchingHeightChanged;
		ReferenceField<double> _crouchingHeight = 1.0;

		[Category( "Crouching" )]
		[DefaultValue( 0.1 )]
		public Reference<double> CrouchingWalkUpHeight
		{
			get { if( _crouchingWalkUpHeight.BeginGet() ) CrouchingWalkUpHeight = _crouchingWalkUpHeight.Get( this ); return _crouchingWalkUpHeight.value; }
			set { if( _crouchingWalkUpHeight.BeginSet( ref value ) ) { try { CrouchingWalkUpHeightChanged?.Invoke( this ); } finally { _crouchingWalkUpHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingWalkUpHeight"/> property value changes.</summary>
		public event Action<Component_Character> CrouchingWalkUpHeightChanged;
		ReferenceField<double> _crouchingWalkUpHeight = 0.1;

		[Category( "Crouching" )]
		[DefaultValue( 0.55 )]
		public Reference<double> CrouchingPositionToGroundHeight
		{
			get { if( _crouchingPositionToGroundHeight.BeginGet() ) CrouchingPositionToGroundHeight = _crouchingPositionToGroundHeight.Get( this ); return _crouchingPositionToGroundHeight.value; }
			set { if( _crouchingPositionToGroundHeight.BeginSet( ref value ) ) { try { CrouchingPositionToGroundHeightChanged?.Invoke( this ); } finally { _crouchingPositionToGroundHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingPositionToGroundHeight"/> property value changes.</summary>
		public event Action<Component_Character> CrouchingPositionToGroundHeightChanged;
		ReferenceField<double> _crouchingPositionToGroundHeight = 0.55;

		[Category( "Crouching" )]
		[DefaultValue( 1.0 )]
		public Reference<double> CrouchingMaxSpeed
		{
			get { if( _crouchingMaxSpeed.BeginGet() ) CrouchingMaxSpeed = _crouchingMaxSpeed.Get( this ); return _crouchingMaxSpeed.value; }
			set { if( _crouchingMaxSpeed.BeginSet( ref value ) ) { try { CrouchingMaxSpeedChanged?.Invoke( this ); } finally { _crouchingMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingMaxSpeed"/> property value changes.</summary>
		public event Action<Component_Character> CrouchingMaxSpeedChanged;
		ReferenceField<double> _crouchingMaxSpeed = 1.0;

		//!!!!
		[Category( "Crouching" )]
		[DefaultValue( 100000 )]
		public Reference<double> CrouchingForce
		{
			get { if( _crouchingForce.BeginGet() ) CrouchingForce = _crouchingForce.Get( this ); return _crouchingForce.value; }
			set { if( _crouchingForce.BeginSet( ref value ) ) { try { CrouchingForceChanged?.Invoke( this ); } finally { _crouchingForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingForce"/> property value changes.</summary>
		public event Action<Component_Character> CrouchingForceChanged;
		ReferenceField<double> _crouchingForce = 100000;

		/////////////////////////////////////////
		//Animate

		/// <summary>
		/// Whether to enable default animation method of the character.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( false )]
		public Reference<bool> Animate
		{
			get { if( _animate.BeginGet() ) Animate = _animate.Get( this ); return _animate.value; }
			set
			{
				if( _animate.BeginSet( ref value ) )
				{
					try
					{
						AnimateChanged?.Invoke( this );
						OnAnimateChanged();
					}
					finally { _animate.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Animate"/> property value changes.</summary>
		public event Action<Component_Character> AnimateChanged;
		ReferenceField<bool> _animate = false;

		/// <summary>
		/// Animation of character at rest.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Component_Animation> IdleAnimation
		{
			get { if( _idleAnimation.BeginGet() ) IdleAnimation = _idleAnimation.Get( this ); return _idleAnimation.value; }
			set { if( _idleAnimation.BeginSet( ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IdleAnimation"/> property value changes.</summary>
		public event Action<Component_Character> IdleAnimationChanged;
		ReferenceField<Component_Animation> _idleAnimation = null;

		/// <summary>
		/// Character walking animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Component_Animation> WalkAnimation
		{
			get { if( _walkAnimation.BeginGet() ) WalkAnimation = _walkAnimation.Get( this ); return _walkAnimation.value; }
			set { if( _walkAnimation.BeginSet( ref value ) ) { try { WalkAnimationChanged?.Invoke( this ); } finally { _walkAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkAnimation"/> property value changes.</summary>
		public event Action<Component_Character> WalkAnimationChanged;
		ReferenceField<Component_Animation> _walkAnimation = null;

		/// <summary>
		/// The multiplier for playing the walking animation depending on the speed of the character.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> WalkAnimationSpeed
		{
			get { if( _walkAnimationSpeed.BeginGet() ) WalkAnimationSpeed = _walkAnimationSpeed.Get( this ); return _walkAnimationSpeed.value; }
			set { if( _walkAnimationSpeed.BeginSet( ref value ) ) { try { WalkAnimationSpeedChanged?.Invoke( this ); } finally { _walkAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkAnimationSpeed"/> property value changes.</summary>
		public event Action<Component_Character> WalkAnimationSpeedChanged;
		ReferenceField<double> _walkAnimationSpeed = 1.0;

		/// <summary>
		/// Character running animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Component_Animation> RunAnimation
		{
			get { if( _runAnimation.BeginGet() ) RunAnimation = _runAnimation.Get( this ); return _runAnimation.value; }
			set { if( _runAnimation.BeginSet( ref value ) ) { try { RunAnimationChanged?.Invoke( this ); } finally { _runAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunAnimation"/> property value changes.</summary>
		public event Action<Component_Character> RunAnimationChanged;
		ReferenceField<Component_Animation> _runAnimation = null;

		/// <summary>
		/// The multiplier for playing the running animation depending on the speed of the character.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> RunAnimationSpeed
		{
			get { if( _runAnimationSpeed.BeginGet() ) RunAnimationSpeed = _runAnimationSpeed.Get( this ); return _runAnimationSpeed.value; }
			set { if( _runAnimationSpeed.BeginSet( ref value ) ) { try { RunAnimationSpeedChanged?.Invoke( this ); } finally { _runAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunAnimationSpeed"/> property value changes.</summary>
		public event Action<Component_Character> RunAnimationSpeedChanged;
		ReferenceField<double> _runAnimationSpeed = 1.0;

		/// <summary>
		/// Character flying animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Component_Animation> FlyAnimation
		{
			get { if( _flyAnimation.BeginGet() ) FlyAnimation = _flyAnimation.Get( this ); return _flyAnimation.value; }
			set { if( _flyAnimation.BeginSet( ref value ) ) { try { FlyAnimationChanged?.Invoke( this ); } finally { _flyAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyAnimation"/> property value changes.</summary>
		public event Action<Component_Character> FlyAnimationChanged;
		ReferenceField<Component_Animation> _flyAnimation = null;

		/// <summary>
		/// Character jump animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Component_Animation> JumpAnimation
		{
			get { if( _jumpAnimation.BeginGet() ) JumpAnimation = _jumpAnimation.Get( this ); return _jumpAnimation.value; }
			set { if( _jumpAnimation.BeginSet( ref value ) ) { try { JumpAnimationChanged?.Invoke( this ); } finally { _jumpAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpAnimation"/> property value changes.</summary>
		public event Action<Component_Character> JumpAnimationChanged;
		ReferenceField<Component_Animation> _jumpAnimation = null;

		/// <summary>
		/// Character left turn animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Component_Animation> LeftTurnAnimation
		{
			get { if( _leftTurnAnimation.BeginGet() ) LeftTurnAnimation = _leftTurnAnimation.Get( this ); return _leftTurnAnimation.value; }
			set { if( _leftTurnAnimation.BeginSet( ref value ) ) { try { LeftTurnAnimationChanged?.Invoke( this ); } finally { _leftTurnAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftTurnAnimation"/> property value changes.</summary>
		public event Action<Component_Character> LeftTurnAnimationChanged;
		ReferenceField<Component_Animation> _leftTurnAnimation = null;

		/// <summary>
		/// Character left turn animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Component_Animation> RightTurnAnimation
		{
			get { if( _rightTurnAnimation.BeginGet() ) RightTurnAnimation = _rightTurnAnimation.Get( this ); return _rightTurnAnimation.value; }
			set { if( _rightTurnAnimation.BeginSet( ref value ) ) { try { RightTurnAnimationChanged?.Invoke( this ); } finally { _rightTurnAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightTurnAnimation"/> property value changes.</summary>
		public event Action<Component_Character> RightTurnAnimationChanged;
		ReferenceField<Component_Animation> _rightTurnAnimation = null;

		/////////////////////////////////////////

		//[Category( "Skeleton State" )]
		//[DefaultValue( "NaN NaN NaN" )]
		//public Reference<Vector3> TorsoLookAt
		//{
		//	get { if( _torsoLookAt.BeginGet() ) TorsoLookAt = _torsoLookAt.Get( this ); return _torsoLookAt.value; }
		//	set { if( _torsoLookAt.BeginSet( ref value ) ) { try { TorsoLookAtChanged?.Invoke( this ); } finally { _torsoLookAt.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TorsoLookAt"/> property value changes.</summary>
		//public event Action<Component_Character> TorsoLookAtChanged;
		//ReferenceField<Vector3> _torsoLookAt = new Vector3( double.NaN, double.NaN, double.NaN );

		//[Category( "Skeleton State" )]
		//[DefaultValue( "spine" )] //!!!!? chest
		//public Reference<string> TorsoBone
		//{
		//	get { if( _torsoBone.BeginGet() ) TorsoBone = _torsoBone.Get( this ); return _torsoBone.value; }
		//	set { if( _torsoBone.BeginSet( ref value ) ) { try { TorsoBoneChanged?.Invoke( this ); } finally { _torsoBone.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TorsoBone"/> property value changes.</summary>
		//public event Action<Component_Character> TorsoBoneChanged;
		//ReferenceField<string> _torsoBone = "spine";

		/// <summary>
		/// The name of the left hand bone.
		/// </summary>
		[Category( "Skeleton State" )]
		[DefaultValue( "mixamorig:LeftHand" )]
		public Reference<string> LeftHandBone
		{
			get { if( _leftHandBone.BeginGet() ) LeftHandBone = _leftHandBone.Get( this ); return _leftHandBone.value; }
			set { if( _leftHandBone.BeginSet( ref value ) ) { try { LeftHandBoneChanged?.Invoke( this ); } finally { _leftHandBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandBone"/> property value changes.</summary>
		public event Action<Component_Character> LeftHandBoneChanged;
		ReferenceField<string> _leftHandBone = "mixamorig:LeftHand";

		/// <summary>
		/// Left hand control ratio.
		/// </summary>
		[Category( "Skeleton State" )]
		[Range( 0, 1 )]
		[DefaultValue( 0.0 )]
		public Reference<double> LeftHandFactor
		{
			get { if( _leftHandFactor.BeginGet() ) LeftHandFactor = _leftHandFactor.Get( this ); return _leftHandFactor.value; }
			set { if( _leftHandFactor.BeginSet( ref value ) ) { try { LeftHandFactorChanged?.Invoke( this ); } finally { _leftHandFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandFactor"/> property value changes.</summary>
		public event Action<Component_Character> LeftHandFactorChanged;
		ReferenceField<double> _leftHandFactor = 0.0;

		/// <summary>
		/// Left hand target transform in the world coordinates. X - forward, -Z - palm.
		/// </summary>
		[Category( "Skeleton State" )]
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> LeftHandTransform
		{
			get { if( _leftHandTransform.BeginGet() ) LeftHandTransform = _leftHandTransform.Get( this ); return _leftHandTransform.value; }
			set { if( _leftHandTransform.BeginSet( ref value ) ) { try { LeftHandTransformChanged?.Invoke( this ); } finally { _leftHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandTransform"/> property value changes.</summary>
		public event Action<Component_Character> LeftHandTransformChanged;
		ReferenceField<Transform> _leftHandTransform = NeoAxis.Transform.Identity;

		///// <summary>
		///// Left hand target position in the world coordinates.
		///// </summary>
		//[Category( "Skeleton State" )]
		//[DefaultValue( "0 0 0" )]
		//public Reference<Vector3> LeftHandPosition
		//{
		//	get { if( _leftHandPosition.BeginGet() ) LeftHandPosition = _leftHandPosition.Get( this ); return _leftHandPosition.value; }
		//	set { if( _leftHandPosition.BeginSet( ref value ) ) { try { LeftHandPositionChanged?.Invoke( this ); } finally { _leftHandPosition.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeftHandPosition"/> property value changes.</summary>
		//public event Action<Component_Character> LeftHandPositionChanged;
		//ReferenceField<Vector3> _leftHandPosition = new Vector3( 0, 0, 0 );

		///// <summary>
		///// Left hand rotation in the world coordinates. X - forward, -Z - palm.
		///// </summary>
		//[Category( "Skeleton State" )]
		//[DefaultValue( "0 0 0 1" )]
		//public Reference<Quaternion> LeftHandRotation
		//{
		//	get { if( _leftHandRotation.BeginGet() ) LeftHandRotation = _leftHandRotation.Get( this ); return _leftHandRotation.value; }
		//	set { if( _leftHandRotation.BeginSet( ref value ) ) { try { LeftHandRotationChanged?.Invoke( this ); } finally { _leftHandRotation.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeftHandRotation"/> property value changes.</summary>
		//public event Action<Component_Character> LeftHandRotationChanged;
		//ReferenceField<Quaternion> _leftHandRotation = new Quaternion( 0, 0, 0, 1 );

		/// <summary>
		/// The name of the right hand bone.
		/// </summary>
		[Category( "Skeleton State" )]
		[DefaultValue( "mixamorig:RightHand" )]
		public Reference<string> RightHandBone
		{
			get { if( _rightHandBone.BeginGet() ) RightHandBone = _rightHandBone.Get( this ); return _rightHandBone.value; }
			set { if( _rightHandBone.BeginSet( ref value ) ) { try { RightHandBoneChanged?.Invoke( this ); } finally { _rightHandBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandBone"/> property value changes.</summary>
		public event Action<Component_Character> RightHandBoneChanged;
		ReferenceField<string> _rightHandBone = "mixamorig:RightHand";

		/// <summary>
		/// Right hand control ratio.
		/// </summary>
		[Category( "Skeleton State" )]
		[Range( 0, 1 )]
		[DefaultValue( 0.0 )]
		public Reference<double> RightHandFactor
		{
			get { if( _rightHandFactor.BeginGet() ) RightHandFactor = _rightHandFactor.Get( this ); return _rightHandFactor.value; }
			set { if( _rightHandFactor.BeginSet( ref value ) ) { try { RightHandFactorChanged?.Invoke( this ); } finally { _rightHandFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandFactor"/> property value changes.</summary>
		public event Action<Component_Character> RightHandFactorChanged;
		ReferenceField<double> _rightHandFactor = 0.0;

		/// <summary>
		/// Right hand target transform in the world coordinates. X - forward, -Z - palm.
		/// </summary>
		[Category( "Skeleton State" )]
		[DefaultValue( NeoAxis.Transform.IdentityAsString )]
		public Reference<Transform> RightHandTransform
		{
			get { if( _rightHandTransform.BeginGet() ) RightHandTransform = _rightHandTransform.Get( this ); return _rightHandTransform.value; }
			set { if( _rightHandTransform.BeginSet( ref value ) ) { try { RightHandTransformChanged?.Invoke( this ); } finally { _rightHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandTransform"/> property value changes.</summary>
		public event Action<Component_Character> RightHandTransformChanged;
		ReferenceField<Transform> _rightHandTransform = NeoAxis.Transform.Identity;

		///// <summary>
		///// Right hand target position in the world coordinates.
		///// </summary>
		//[Category( "Skeleton State" )]
		//[DefaultValue( "0 0 0" )]
		//public Reference<Vector3> RightHandPosition
		//{
		//	get { if( _rightHandPosition.BeginGet() ) RightHandPosition = _rightHandPosition.Get( this ); return _rightHandPosition.value; }
		//	set { if( _rightHandPosition.BeginSet( ref value ) ) { try { RightHandPositionChanged?.Invoke( this ); } finally { _rightHandPosition.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RightHandPosition"/> property value changes.</summary>
		//public event Action<Component_Character> RightHandPositionChanged;
		//ReferenceField<Vector3> _rightHandPosition = new Vector3( 0, 0, 0 );

		///// <summary>
		///// Right hand rotation in the world coordinates. X - forward, -Z - palm.
		///// </summary>
		//[Category( "Skeleton State" )]
		//[DefaultValue( "0 0 0 1" )]
		//public Reference<Quaternion> RightHandRotation
		//{
		//	get { if( _rightHandRotation.BeginGet() ) RightHandRotation = _rightHandRotation.Get( this ); return _rightHandRotation.value; }
		//	set { if( _rightHandRotation.BeginSet( ref value ) ) { try { RightHandRotationChanged?.Invoke( this ); } finally { _rightHandRotation.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RightHandRotation"/> property value changes.</summary>
		//public event Action<Component_Character> RightHandRotationChanged;
		//ReferenceField<Quaternion> _rightHandRotation = new Quaternion( 0, 0, 0, 1 );

		/// <summary>
		/// The name of the head body.
		/// </summary>
		[Category( "Skeleton State" )]
		[DefaultValue( "mixamorig:Head" )]
		public Reference<string> HeadBone
		{
			get { if( _headBone.BeginGet() ) HeadBone = _headBone.Get( this ); return _headBone.value; }
			set { if( _headBone.BeginSet( ref value ) ) { try { HeadBoneChanged?.Invoke( this ); } finally { _headBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadBone"/> property value changes.</summary>
		public event Action<Component_Character> HeadBoneChanged;
		ReferenceField<string> _headBone = "mixamorig:Head";

		/// <summary>
		/// Head control ratio.
		/// </summary>
		[Category( "Skeleton State" )]
		[Range( 0, 1 )]
		[DefaultValue( 0.0 )]
		public Reference<double> HeadFactor
		{
			get { if( _headFactor.BeginGet() ) HeadFactor = _headFactor.Get( this ); return _headFactor.value; }
			set { if( _headFactor.BeginSet( ref value ) ) { try { HeadFactorChanged?.Invoke( this ); } finally { _headFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadFactor"/> property value changes.</summary>
		public event Action<Component_Character> HeadFactorChanged;
		ReferenceField<double> _headFactor = 0.0;

		/// <summary>
		/// Target position of the head.
		/// </summary>
		[Category( "Skeleton State" )]
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> HeadLookAt
		{
			get { if( _headLookAt.BeginGet() ) HeadLookAt = _headLookAt.Get( this ); return _headLookAt.value; }
			set { if( _headLookAt.BeginSet( ref value ) ) { try { HeadLookAtChanged?.Invoke( this ); } finally { _headLookAt.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadLookAt"/> property value changes.</summary>
		public event Action<Component_Character> HeadLookAtChanged;
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
		//public event Action<Component_Character> LeftHandThumbFingerFlexionChanged;
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
		//public event Action<Component_Character> LeftHandIndexFingerFlexionChanged;
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
		//public event Action<Component_Character> LeftHandMiddleFingerFlexionChanged;
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
		//public event Action<Component_Character> LeftHandRingFingerFlexionChanged;
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
		//public event Action<Component_Character> LeftHandPinkyFingerFlexionChanged;
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
		//public event Action<Component_Character> RightHandThumbFingerFlexionChanged;
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
		//public event Action<Component_Character> RightHandIndexFingerFlexionChanged;
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
		//public event Action<Component_Character> RightHandMiddleFingerFlexionChanged;
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
		//public event Action<Component_Character> RightHandRingFingerFlexionChanged;
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
		//public event Action<Component_Character> RightHandPinkyFingerFlexionChanged;
		//ReferenceField<double> _rightHandPinkyFingerFlexion = 0.0;

		//!!!!
		//[Category( "Skeleton State" )]
		//[DefaultValue( null )]
		//public Reference<Component_ObjectInSpace> EyesLookAt
		//{
		//	get { if( _eyesLookAt.BeginGet() ) EyesLookAt = _eyesLookAt.Get( this ); return _eyesLookAt.value; }
		//	set { if( _eyesLookAt.BeginSet( ref value ) ) { try { EyesLookAtChanged?.Invoke( this ); } finally { _eyesLookAt.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="EyesLookAt"/> property value changes.</summary>
		//public event Action<Component_Character> EyesLookAtChanged;
		//ReferenceField<Component_ObjectInSpace> _eyesLookAt = null;

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

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( RunForwardMaxSpeed ):
				case nameof( RunBackwardMaxSpeed ):
				case nameof( RunSideMaxSpeed ):
				case nameof( RunForce ):
					if( !RunSupport )
						skip = true;
					break;

				case nameof( FlyControlMaxSpeed ):
				case nameof( FlyControlForce ):
					if( !FlyControlSupport )
						skip = true;
					break;

				case nameof( JumpSpeed ):
				case nameof( JumpSound ):
					if( !JumpSupport )
						skip = true;
					break;

				case nameof( CrouchingHeight ):
				case nameof( CrouchingWalkUpHeight ):
				case nameof( CrouchingPositionToGroundHeight ):
				case nameof( CrouchingMaxSpeed ):
				case nameof( CrouchingForce ):
					if( !CrouchingSupport )
						skip = true;
					break;

				case nameof( IdleAnimation ):
				case nameof( WalkAnimation ):
				case nameof( WalkAnimationSpeed ):
				case nameof( RunAnimation ):
				case nameof( RunAnimationSpeed ):
				case nameof( FlyAnimation ):
				case nameof( JumpAnimation ):
				case nameof( LeftTurnAnimation ):
				case nameof( RightTurnAnimation ):
					if( !Animate )
						skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				var tr = GetTransform();
				SetTurnToDirection( SphericalDirection.FromVector( tr.Rotation.GetForward() ), true );

				//if( FindCollisionBody() == null )
				UpdateCollisionBody();

				//if( mainBody != null )
				//	mainBody.LinearVelocity = linearVelocityForSerialization;

				if( ParentScene != null )
					ParentScene.PhysicsSimulationStepAfter += ParentScene_PhysicsSimulationStepAfter;

				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
					TickAnimate( 0.001f );
			}
			else
			{
				if( ParentScene != null )
					ParentScene.PhysicsSimulationStepAfter -= ParentScene_PhysicsSimulationStepAfter;

				mainBody = null;
			}
		}

		private void ParentScene_PhysicsSimulationStepAfter( Component_Scene obj )
		{
			if( ParentScene == null )
				return;

			if( mainBody != null && mainBody.Active )
				UpdateRotation();// false );
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
			if( mainBody != null )
				mainBody.Transform = value;
			else
				Transform = value;
		}

		public Component_RigidBody GetCollisionBody()
		{
			return GetComponent<Component_RigidBody>( "Collision Body" );
		}

		public delegate void UpdateCollisionBodyEventDelegate( Component_Character sender );
		public event UpdateCollisionBodyEventDelegate UpdateCollisionBodyEvent;

		public void UpdateCollisionBody()
		{
			//DestroyCollisionBody();

			GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var fromPositionToFloorDistance );

			var body = GetCollisionBody();
			if( body == null )
			{
				body = CreateComponent<Component_RigidBody>( enabled: false );
				body.Name = "Collision Body";
			}
			//else
			//	body.Enabled = false;
			mainBody = body;

			body.MotionType = Component_RigidBody.MotionTypeEnum.Dynamic;
			body.CanBeSelected = false;
			body.LinearSleepingThreshold = 0;
			body.AngularSleepingThreshold = 0;
			body.AngularDamping = 10;
			body.Mass = Mass;
			//body.PhysX_SolverPositionIterations = body.PhysX_SolverPositionIterations * 2;
			//body.PhysX_SolverVelocityIterations = body.PhysX_SolverVelocityIterations * 2;

			body.MaterialFriction = 0;
			body.MaterialSpinningFriction = 0;
			body.MaterialRollingFriction = 0;
			body.MaterialRestitution = 0;
			//shape.Hardness = 0;
			//shape.SpecialLiquidDensity = 2000;
			//shape.ContactGroup = (int)ContactGroup.Dynamic;

			var length = height - Radius * 2;
			if( length < 0.01 )
				length = 0.01;

			var capsuleShape = body.GetComponent<Component_CollisionShape_Capsule>();
			if( capsuleShape == null )
			{
				body.Enabled = false;
				capsuleShape = body.CreateComponent<Component_CollisionShape_Capsule>();
				capsuleShape.Name = ComponentUtility.GetNewObjectUniqueName( capsuleShape );
			}
			capsuleShape.Height = length;
			capsuleShape.Radius = Radius;
			double offset = fromPositionToFloorDistance - height / 2;
			capsuleShape.TransformRelativeToParent = new Transform( new Vector3( 0, 0, -offset ), Quaternion.Identity );

			UpdateCollisionBodyEvent?.Invoke( this );

			body.Enabled = true;

			//DisableControlPhysicsModelPushedToWorldFlag = true;

			//HookColisionEvents();

			//set reference. lock Transform of the character to Transform of the body
			Transform = ReferenceUtility.MakeThisReference( this, body, "Transform" );
		}

		void DestroyCollisionBody()
		{
			//ReleaseCollisionEvents();

			//reset Transform reference
			if( Transform.ReferenceSpecified )
				Transform = Transform.Value;

			var body = GetCollisionBody();
			if( body != null )
				body.Dispose();
			mainBody = null;
		}

		public double GetScaleFactor()
		{
			//!!!!cache
			var result = GetTransform().Scale.MaxComponent();
			if( result == 0 )
				result = 0.0001;
			return result;
		}

		public void GetBodyFormInfo( bool crouching, out double outHeight, out double outWalkUpHeight, out double outPositionToGroundHeight )
		{
			//!!!!add event GetBodyFormInfoEvent

			if( crouching )
			{
				outHeight = CrouchingHeight;
				outWalkUpHeight = CrouchingWalkUpHeight;
				outPositionToGroundHeight = CrouchingPositionToGroundHeight;
			}
			else
			{
				outHeight = Height;
				outWalkUpHeight = WalkUpHeight;
				outPositionToGroundHeight = PositionToGroundHeight;
			}
		}

		///////////////////////////////////////////

		public void SetMoveVector( Vector2 direction, bool run )
		{
			moveVectorTimer = 2;
			moveVector = direction;
			moveVectorRun = run;
		}

		[Browsable( false )]
		public Component_RigidBody MainBody
		{
			get { return mainBody; }
		}

		[Browsable( false )]
		public SphericalDirection CurrentTurnToDirection
		{
			get { return currentTurnToDirection; }
		}

		[Browsable( false )]
		public SphericalDirection RequiredTurnToDirection
		{
			get { return requiredTurnToDirection; }
		}

		public void SetTurnToDirection( SphericalDirection value, bool turnInstantly )
		{
			requiredTurnToDirection = value;
			if( turnInstantly )
				currentTurnToDirection = requiredTurnToDirection;

			//var diff = turnToDirection.GetVector();
			//horizontalDirectionForUpdateRotation = Math.Atan2( diff.Y, diff.X );

			if( turnInstantly )
				UpdateRotation();// true );
		}

		public void SetTurnToDirection( Vector3 value, bool turnInstantly )
		{
			SetTurnToDirection( SphericalDirection.FromVector( value ), turnInstantly );
		}

		public void UpdateRotation()// bool allowUpdateOldRotation )
		{
			var diff = CurrentTurnToDirection.GetVector();
			var halfAngle = Math.Atan2( diff.Y, diff.X ) * 0.5;
			//var halfAngle = horizontalDirectionForUpdateRotation * 0.5;
			Quaternion rot = new Quaternion( new Vector3( 0, 0, Math.Sin( halfAngle ) ), Math.Cos( halfAngle ) );

			const float epsilon = .0001f;

			var tr = GetTransform();

			//update Rotation
			if( !tr.Rotation.Equals( rot, epsilon ) )
			{
				//bool keepDisableControlPhysicsModelPushedToWorldFlag = DisableControlPhysicsModelPushedToWorldFlag;
				//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
				//   DisableControlPhysicsModelPushedToWorldFlag = true;

				SetTransform( tr.UpdateRotation( rot ) );
				//Rotation = rot;

				//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
				//   DisableControlPhysicsModelPushedToWorldFlag = false;
			}

			////update OldRotation
			//if( allowUpdateOldRotation )
			//{
			//	//disable updating OldRotation property for PlatformerDemo
			//	bool updateOldRotation = true;
			//	if( Intellect != null && PlayerIntellect.Instance == Intellect )
			//	{
			//		if( GameMap.Instance != null && GameMap.Instance.GameType == GameMap.GameTypes.PlatformerDemo )
			//			updateOldRotation = false;
			//	}
			//	if( updateOldRotation )
			//		OldRotation = rot;
			//}
		}

		public bool IsOnGround()
		{
			if( jumpInactiveTime != 0 )
				return false;
			if( forceIsOnGroundRemainingTime > 0 )
				return true;

			double distanceFromPositionToFloor = crouching ? CrouchingPositionToGroundHeight : PositionToGroundHeight;
			const double maxThreshold = 0.2;
			return mainBodyGroundDistanceNoScale - maxThreshold < distanceFromPositionToFloor && groundBody != null;
		}

		public bool IsOnGroundWithLatency()
		{
			return elapsedTimeSinceLastGroundContact < 0.25 || IsOnGround();
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
				groundBody = null;

			TickMovement();
			TickPhysicsForce();
			TickCurrentTurnToDirection();

			UpdateRotation();// true );
			if( JumpSupport )
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

			var trPosition = GetTransform().Position;
			if( lastTransformPosition.HasValue )
				lastLinearVelocity = ( trPosition - lastTransformPosition.Value ) / Time.SimulationDelta;
			else
				lastLinearVelocity = Vector3.Zero;
			lastTransformPosition = trPosition;

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

			UpdateTransformOffsetInSimulation();

			//Log.Info( GetLinearVelocity().ToVector2().ToString() + " " + GetLinearVelocity().Length().ToString() );
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			TickAnimate( delta );
			UpdateEnabledItemTransform();

			//touch Transform to update character AABB
			if( EnabledInHierarchy && VisibleInHierarchy )
			{
				var t = Transform;
			}
		}

		public bool IsNeedRun()
		{
			//use specified force move vector
			if( moveVectorTimer != 0 )
				return moveVectorRun;

			return false;
		}

		Vector2 GetMoveVector()
		{
			//use specified move vector
			if( moveVectorTimer != 0 )
				return moveVector;

			return Vector2.Zero;
		}

		void TickPhysicsForce()
		{
			Vector2 forceVec = GetMoveVector();
			if( forceVec != Vector2.Zero )
			{
				float speedMultiplier = 1;
				//if( FastMoveInfluence != null )
				//	speedCoefficient = FastMoveInfluence.Type.Coefficient;

				double maxSpeed = 0;
				double force = 0;

				if( IsOnGround() )
				{
					//calcualate maxSpeed and force on ground.

					Vector2 localVec = ( new Vector3( forceVec.X, forceVec.Y, 0 ) * GetTransform().Rotation.GetInverse() ).ToVector2();

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
								speedX = running ? RunForwardMaxSpeed : WalkForwardMaxSpeed;
							else
								speedX = running ? RunBackwardMaxSpeed : WalkBackwardMaxSpeed;
							maxSpeed += speedX * Math.Abs( localVec.X );
							force += ( running ? RunForce : WalkForce ) * Math.Abs( localVec.X );
						}

						if( Math.Abs( localVec.Y ) >= .001f )
						{
							//left and right
							maxSpeed += ( running ? RunSideMaxSpeed : WalkSideMaxSpeed ) * Math.Abs( localVec.Y );
							force += ( running ? RunForce : WalkForce ) * Math.Abs( localVec.Y );
						}
					}
					else
					{
						maxSpeed = CrouchingMaxSpeed;
						force = CrouchingForce;
					}
				}
				else
				{
					//calcualate maxSpeed and force when flying.
					if( FlyControlSupport )
					{
						maxSpeed = FlyControlMaxSpeed;
						force = FlyControlForce;
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
						mainBody.ApplyForce( new Vector3( forceVec.X, forceVec.Y, 0 ) * force * Time.SimulationDelta, Vector3.Zero );
				}
			}

			lastTickForceVector = forceVec;
		}

		void UpdateMainBodyDamping()
		{
			if( IsOnGround() && jumpInactiveTime == 0 )
			{
				if( allowToSleepTime > Time.SimulationDelta * 2.5f && moveVector == Vector2.Zero )
					mainBody.LinearDamping = LinearDampingOnGroundIdle;
				else
					mainBody.LinearDamping = LinearDampingOnGroundMove;
			}
			else
				mainBody.LinearDamping = LinearDampingFly;
		}

		void TickMovement()
		{
			//wake up when ground is moving
			if( !mainBody.Active && groundBody != null && groundBody.Active && ( groundBody.LinearVelocity.Value.LengthSquared() > .3f || groundBody.AngularVelocity.Value.LengthSquared() > .3f ) )
			{
				mainBody.Activate();
			}

			CalculateMainBodyGroundDistanceAndGroundBody();//out var addForceOnBigSlope );

			if( mainBody.Active || !IsOnGround() )
			{
				UpdateMainBodyDamping();

				if( IsOnGround() )
				{
					//reset angular velocity
					mainBody.AngularVelocity = Vector3.Zero;

					//move the object when it underground
					if( lastTickForceVector != Vector2.Zero && forceIsOnGroundRemainingTime == 0 )
					{
						var scaleFactor = GetScaleFactor();

						var maxSpeed = Math.Min( WalkSideMaxSpeed, Math.Min( WalkForwardMaxSpeed, WalkBackwardMaxSpeed ) );
						Vector3 newPositionOffsetNoScale = new Vector3( lastTickForceVector.GetNormalize() * maxSpeed * .15f, 0 );

						ClimbObstacleTest( newPositionOffsetNoScale, out var upHeightNoScale );

						//move object
						GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var fromPositionToFloorDistance );
						if( upHeightNoScale > Radius * 0.5 && upHeightNoScale <= walkUpHeight && jumpInactiveTime == 0 && walkUpHeight != 0 )//upHeight > .01f 
						{
							//bool keepDisableControlPhysicsModelPushedToWorldFlag = DisableControlPhysicsModelPushedToWorldFlag;
							//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
							//   DisableControlPhysicsModelPushedToWorldFlag = true;

							//additional offset
							upHeightNoScale += Radius * 0.5;

							var tr = GetTransform();
							var newPosition = tr.Position + new Vector3( 0, 0, upHeightNoScale * scaleFactor );

							if( IsFreePositionToMove( newPosition ) )
							{
								mainBody.Transform = tr.UpdatePosition( newPosition );

								forceIsOnGroundRemainingTime = 0.5;//0.2;
								disableGravityRemainingTime = 0.5;
								smoothCameraOffsetZ = Math.Min( tr.Position.Z - newPosition.Z, smoothCameraOffsetZ );
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
				if( IsOnGround() && groundBody != null && ApplyGroundVelocity )
				{
					if( groundBody.MotionType.Value != Component_RigidBody.MotionTypeEnum.Static && groundBody.Active )
					{
						Vector3 groundVel = groundBody.LinearVelocity;

						Vector3 vel = mainBody.LinearVelocity;

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

						mainBody.LinearVelocity = vel;

						//simple anti-damping
						mainBody.LinearVelocity += groundVel * .25f;
					}
				}

				//sleep if on ground and zero velocity

				bool needSleep = false;
				if( IsOnGround() && groundBody != null )
				{
					bool groundStopped = !groundBody.Active ||
						( groundBody.LinearVelocity.Value.LengthSquared() < .3f && groundBody.AngularVelocity.Value.LengthSquared() < .3f );
					if( groundStopped && GetLinearVelocity().ToVector2().Length() < MinSpeedToSleepBody && moveVector == Vector2.Zero )
						needSleep = true;
				}

				////strange fix for PhysX. The character can frezee in fly with zero linear velocity.
				//if( PhysicsWorld.Instance.IsPhysX() )
				//{
				//	if( !needSleep && mainBody.LinearVelocity == Vector3.Zero && lastTickPosition == Position )
				//	{
				//		mainBody.Sleeping = true;
				//		needSleep = false;
				//	}
				//}

				if( needSleep )
					allowToSleepTime += Time.SimulationDelta;
				else
					allowToSleepTime = 0;

				if( allowToSleepTime > Time.SimulationDelta * 2.5f )
					mainBody.WantsDeactivation();
				else
					mainBody.Activate();
				//mainBody.Sleeping = allowToSleepTime > Time.SimulationDelta * 2.5f;
			}

			mainBody.EnableGravity = disableGravityRemainingTime == 0;
		}

		bool VolumeCheckGetFirstNotFreePlace( Capsule[] sourceVolumeCapsules, Vector3 destVector, bool firstIteration, float step,
			out List<Component_RigidBody> collisionBodies, out float collisionDistance, out bool collisionOnFirstCheck )
		{
			collisionBodies = new List<Component_RigidBody>();
			collisionDistance = 0;
			collisionOnFirstCheck = false;

			bool firstCheck = true;

			Vector3 direction = destVector.GetNormalize();
			float totalDistance = (float)destVector.Length();
			int stepCount = (int)( (float)totalDistance / step ) + 2;
			Vector3 previousOffset = Vector3.Zero;

			for( int nStep = 0; nStep < stepCount; nStep++ )
			{
				float distance = (float)nStep * step;
				if( distance > totalDistance )
					distance = totalDistance;
				Vector3 offset = direction * distance;

				foreach( Capsule sourceVolumeCapsule in sourceVolumeCapsules )
				{
					var scene = ParentScene;
					if( scene != null )
					{
						Capsule capsule = CapsuleAddOffset( sourceVolumeCapsule, offset );

						//check by capsule
						{
							var contactTestItem = new PhysicsContactTestItem( 1, -1, PhysicsContactTestItem.ModeEnum.All, capsule );
							ParentScene.PhysicsContactTest( contactTestItem );

							foreach( var item in contactTestItem.Result )
							{
								if( item.Body == mainBody )
									continue;
								var body = item.Body as Component_RigidBody;
								if( body != null && !collisionBodies.Contains( body ) )
									collisionBodies.Add( body );
							}
						}

						//check by cylinder at bottom
						{
							var cylinder = new Cylinder( capsule.GetCenter(), capsule.Point1 - new Vector3( 0, 0, Radius ), Radius );
							if( cylinder.Point1 != cylinder.Point2 )
							{
								var contactTestItem = new PhysicsContactTestItem( 1, -1, PhysicsContactTestItem.ModeEnum.All, cylinder );
								ParentScene.PhysicsContactTest( contactTestItem );

								foreach( var item in contactTestItem.Result )
								{
									if( item.Body == mainBody )
										continue;
									var body = item.Body as Component_RigidBody;
									if( body != null && !collisionBodies.Contains( body ) )
										collisionBodies.Add( body );
								}
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
						Capsule[] sourceVolumeCapsules2 = new Capsule[ sourceVolumeCapsules.Length ];
						for( int n = 0; n < sourceVolumeCapsules2.Length; n++ )
							sourceVolumeCapsules2[ n ] = CapsuleAddOffset( sourceVolumeCapsules[ n ], previousOffset );
						Vector3 destVector2 = offset - previousOffset;

						if( VolumeCheckGetFirstNotFreePlace( sourceVolumeCapsules2, destVector2, false, step2, out var collisionBodies2,
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

		//List<Ray> debugRays = new List<Ray>();

		void CalculateMainBodyGroundDistanceAndGroundBody()//out Vector3 addForceOnBigSlope )
		{
			//addForceOnBigSlope = Vector3.Zero;

			//debugRays.Clear();

			GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var fromPositionToFloorDistance );

			Capsule[] volumeCapsules = GetVolumeCapsules();
			//make radius smaller
			for( int n = 0; n < volumeCapsules.Length; n++ )
			{
				Capsule capsule = volumeCapsules[ n ];
				capsule.Radius *= .99f;
				volumeCapsules[ n ] = capsule;
			}

			mainBodyGroundDistanceNoScale = 1000;
			groundBody = null;

			var scene = ParentScene;
			if( scene != null )
			{
				var scaleFactor = GetScaleFactor();

				//1. get collision bodies
				List<Component_RigidBody> collisionBodies;
				float collisionOffset = 0;
				{
					Vector3 destVector = new Vector3( 0, 0, -height * scaleFactor );
					//Vector3 destVector = new Vector3( 0, 0, -height * 1.5f * scaleFactor );
					var step = (float)( Radius.Value / 2 * scaleFactor );
					VolumeCheckGetFirstNotFreePlace( volumeCapsules, destVector, true, step, out collisionBodies, out collisionOffset, out _ );
				}

				//2. check slope angle
				if( collisionBodies.Count != 0 )
				{
					Capsule capsule = volumeCapsules[ volumeCapsules.Length - 1 ];
					Vector3 rayCenter = capsule.Point1 - new Vector3( 0, 0, collisionOffset );

					Component_RigidBody foundBodyWithGoodAngle = null;
					Radian foundBodyWithGoodAngleSlopeAngle = 0;
					//Vector3 bigSlopeVector = Vector3.Zero;

					const int horizontalStepCount = 16;
					const int verticalStepCount = 8;

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

							SphericalDirection sphereDir = new SphericalDirection( horizontalAngle, -verticalAngle );
							Ray ray = new Ray( rayCenter, sphereDir.GetVector() * Radius * 1.3f * scaleFactor );//Type.Radius * 1.1f );

							var item = new PhysicsRayTestItem( ray, 1, -1, PhysicsRayTestItem.ModeEnum.All );
							scene.PhysicsRayTest( item );

							//debugRays.Add( ray );

							if( item.Result.Length != 0 )
							{
								foreach( var result in item.Result )
								{
									var rigidBody = result.Body as Component_RigidBody;
									if( rigidBody != null && result.Body != mainBody )
									{
										//check slope angle
										var slope = MathAlgorithms.GetVectorsAngle( result.Normal, Vector3.ZAxis );
										if( slope < MaxSlopeAngle.Value.InRadians() )
										{
											if( foundBodyWithGoodAngle == null || slope < foundBodyWithGoodAngleSlopeAngle )
											{
												foundBodyWithGoodAngle = rigidBody;
												foundBodyWithGoodAngleSlopeAngle = slope;

											}
											//foundBodyWithGoodAngle = rigidBody;
											//break;
										}
										//else
										//{
										//	Vector3 vector = new Vector3( result.Normal.X, result.Normal.Y, 0 );
										//	if( vector != Vector3.Zero )
										//		bigSlopeVector += vector;
										//}
									}
								}

								if( foundBodyWithGoodAngle != null )
									break;
							}
						}
						if( foundBodyWithGoodAngle != null )
							break;
					}

					if( foundBodyWithGoodAngle != null )
					{
						groundBody = foundBodyWithGoodAngle;
						mainBodyGroundDistanceNoScale = fromPositionToFloorDistance + collisionOffset / scaleFactor;
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

		Capsule GetWorldCapsule( Component_CollisionShape_Capsule shape )
		{
			var scaleFactor = GetScaleFactor();

			Capsule capsule = new Capsule();

			var bodyTransform = mainBody.TransformV;
			var pos = bodyTransform.Position + shape.TransformRelativeToParent.Value.Position;
			var rot = bodyTransform.Rotation;

			var diff = rot * new Vector3( 0, 0, shape.Height * 0.5 * scaleFactor );
			capsule.Point1 = pos - diff;
			capsule.Point2 = pos + diff;
			capsule.Radius = shape.Radius * scaleFactor;

			return capsule;
		}

		//need array?
		Capsule[] GetVolumeCapsules()
		{
			var volumeCapsules = new Capsule[ 1 ];

			var capsuleShape = mainBody.GetComponent<Component_CollisionShape_Capsule>();
			if( capsuleShape != null )
				volumeCapsules[ 0 ] = GetWorldCapsule( capsuleShape );
			else
				volumeCapsules[ 0 ] = new Capsule( mainBody.TransformV.Position, mainBody.TransformV.Position, 0.1 );
			return volumeCapsules;

			//Matrix4 t = bodyTransform.ToMatrix4();
			//var local = capsuleShape.TransformRelativeToParent.Value;
			//if( !local.IsIdentity )
			//	t *= local.ToMatrix4();
			//int axis = Axis.Value;

			//Capsule[] volumeCapsules = new Capsule[ mainBody.Shapes.Length ];
			//for( int n = 0; n < volumeCapsules.Length; n++ )
			//	volumeCapsules[ n ] = GetWorldCapsule( (CapsuleShape)mainBody.Shapes[ n ] );
			//return volumeCapsules;
		}

		Capsule CapsuleAddOffset( Capsule capsule, Vector3 offset )
		{
			return new Capsule( capsule.Point1 + offset, capsule.Point2 + offset, capsule.Radius );
		}

		void ClimbObstacleTest( Vector3 newPositionOffsetNoScale, out double upHeightNoScale )
		{
			GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var fromPositionToFloorDistance );

			var scaleFactor = GetScaleFactor();

			Capsule[] volumeCapsules = GetVolumeCapsules();
			{
				Vector3 offset = ( newPositionOffsetNoScale + new Vector3( 0, 0, walkUpHeight ) ) * scaleFactor;
				for( int n = 0; n < volumeCapsules.Length; n++ )
					volumeCapsules[ n ] = CapsuleAddOffset( volumeCapsules[ n ], offset );
			}

			Vector3 destVector = new Vector3( 0, 0, -walkUpHeight );
			var step = (float)Radius / 2;
			List<Component_RigidBody> collisionBodies;
			float collisionDistance;
			bool collisionOnFirstCheck;
			bool foundCollision = VolumeCheckGetFirstNotFreePlace( volumeCapsules, destVector, true, step, out collisionBodies,
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
				Vector3 vel = mainBody.LinearVelocity;
				vel.Z = JumpSpeed * GetScaleFactor();
				mainBody.LinearVelocity = vel;

				//bool keepDisableControlPhysicsModelPushedToWorldFlag = DisableControlPhysicsModelPushedToWorldFlag;
				//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
				//   DisableControlPhysicsModelPushedToWorldFlag = true;

				var tr = GetTransform();
				mainBody.Transform = tr.UpdatePosition( tr.Position + new Vector3( 0, 0, 0.05 ) );
				//Position += new Vector3( 0, 0, .05f );

				//if( !keepDisableControlPhysicsModelPushedToWorldFlag )
				//   DisableControlPhysicsModelPushedToWorldFlag = false;

				jumpInactiveTime = 0.2;
				jumpDisableRemainingTime = 0;

				UpdateMainBodyDamping();

				mainBody.Activate();

				SoundPlay( JumpSound );

				StartPlayOneAnimation( JumpAnimation );

				OnJump();
			}
		}

		public void TryJump()
		{
			if( !JumpSupport )
				return;
			if( Crouching )
				return;

			jumpDisableRemainingTime = 0.4;
			TickJump( true );
		}

		[Browsable( false )]
		public Vector2 LastTickForceVector
		{
			get { return lastTickForceVector; }
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			//	Vector3 oldPos = Position;

			//	base.OnSetTransform( ref pos, ref rot, ref scl );

			//	if( ( oldPos - Position ).Length() > .3f )
			//	{
			//		if( PhysicsModel != null )
			//		{
			//			foreach( Body body in PhysicsModel.Bodies )
			//				body.Sleeping = false;
			//		}
			//	}
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			if( mainBody != null )
				newBounds = SpaceBounds.Merge( newBounds, new SpaceBounds( GetBox().ToBounds() ) );
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			context.thisObjectWasChecked = true;
			if( SpaceBounds.CalculatedBoundingBox.Intersects( context.ray, out var scale ) )
				context.thisObjectResultRayScale = scale;
		}

		void CalculateGroundRelativeVelocity()
		{
			if( mainBody != null )
			{
				groundRelativeVelocity = GetLinearVelocity();
				if( groundBody != null && groundBody.AngularVelocity.Value.LengthSquared() < .3f )
					groundRelativeVelocity -= groundBody.LinearVelocity;
			}
			else
				groundRelativeVelocity = Vector3.Zero;

			//groundRelativeVelocityToSmooth
			if( groundRelativeVelocitySmoothArray == null )
			{
				var seconds = .2f;
				var count = ( seconds / Time.SimulationDelta ) + .999f;
				groundRelativeVelocitySmoothArray = new Vector3[ (int)count ];
			}
			for( int n = 0; n < groundRelativeVelocitySmoothArray.Length - 1; n++ )
				groundRelativeVelocitySmoothArray[ n ] = groundRelativeVelocitySmoothArray[ n + 1 ];
			groundRelativeVelocitySmoothArray[ groundRelativeVelocitySmoothArray.Length - 1 ] = groundRelativeVelocity;
			groundRelativeVelocitySmooth = Vector3.Zero;
			for( int n = 0; n < groundRelativeVelocitySmoothArray.Length; n++ )
				groundRelativeVelocitySmooth += groundRelativeVelocitySmoothArray[ n ];
			groundRelativeVelocitySmooth /= (float)groundRelativeVelocitySmoothArray.Length;
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

		public Vector3 GetLinearVelocity()
		{
			return lastLinearVelocity;
			//if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
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

		Box GetBox()
		{
			var capsule = GetVolumeCapsules()[ 0 ];
			var scaleFactor = GetScaleFactor();
			var extents = new Vector3( capsule.Radius, capsule.Radius, ( capsule.Point2 - capsule.Point1 ).Length() * 0.5 + capsule.Radius );
			return new Box( capsule.GetCenter(), extents, TransformV.Rotation.ToMatrix3() );
		}

		void DebugDraw( Viewport viewport )
		{
			var renderer = viewport.Simple3DRenderer;
			var points = GetBox().ToPoints();

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

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;
				var scene = context.Owner.AttachedScene;

				if( scene != null && scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects )
				{
					GetBodyFormInfo( crouching, out var height, out var walkUpHeight, out var fromPositionToFloorDistance );

					var renderer = context.Owner.Simple3DRenderer;
					var tr = GetTransform();
					var scaleFactor = tr.Scale.MaxComponent();

					renderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );

					//unit center
					renderer.AddSphere( new Sphere( tr.Position, .05f ), 16 );

					//ground height
					renderer.AddSphere( new Sphere( tr.Position - new Vector3( 0, 0, fromPositionToFloorDistance * scaleFactor ), .05f ), 16 );

					//stand up height
					{
						Vector3 pos = tr.Position - new Vector3( 0, 0, ( fromPositionToFloorDistance - walkUpHeight ) * scaleFactor );
						renderer.AddLine( pos + new Vector3( .2f, 0, 0 ), pos - new Vector3( .2f, 0, 0 ) );
						renderer.AddLine( pos + new Vector3( 0, .2f, 0 ), pos - new Vector3( 0, .2f, 0 ) );
					}

					//eye position
					renderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );
					renderer.AddSphere( new Sphere( TransformV * EyePosition.Value, .05f ), 16 );
				}

				var showLabels = /*show &&*/ mainBody == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;

				//draw selection
				if( mainBody != null )
				{
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
					{
						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.SelectedColor;
						else //if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.CanSelectColor;
						//else
						//	color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;

						var viewport = context.Owner;

						viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
						DebugDraw( viewport );
					}
				}

				//foreach( var ray in debugRays )
				//{
				//	var renderer = context.Owner.Simple3DRenderer;
				//	renderer.SetColor( new ColorValue( 1, 0, 0 ) );
				//	renderer.AddArrow( ray.Origin, ray.GetEndPoint(), 0, 0, true, 0 );
				//}

				//!!!!debug
				if( _tempDebug.Count != 0 )
				{
					var meshInSpace = GetComponent<Component_MeshInSpace>( onlyEnabledInHierarchy: true );
					if( meshInSpace != null )
					{
						foreach( var p in _tempDebug )
						{
							var viewport = context.Owner;
							viewport.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );

							var pp = meshInSpace.TransformV * p;

							viewport.Simple3DRenderer.AddSphere( new Sphere( pp, 0.01 ) );
						}
					}
				}
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
		}

		void TickWiggleWhenWalkingSpeedFactor()
		{
			float destinationFactor;
			if( IsOnGround() )
			{
				destinationFactor = (float)GroundRelativeVelocitySmooth.Length() * .3f;
				if( destinationFactor < .5f )
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
				var speed = Height.Value * 0.5;

				smoothCameraOffsetZ += Time.SimulationDelta * speed;

				if( smoothCameraOffsetZ > 0 )
					smoothCameraOffsetZ = 0;
			}
		}

		Vector3 GetSmoothCameraOffset()
		{
			return new Vector3( 0, 0, smoothCameraOffsetZ );
		}

		public void GetFirstPersonCameraPosition( out Vector3 position, out Vector3 forward, out Vector3 up )
		{
			var tr = TransformV;

			position = tr * EyePosition.Value + GetSmoothCameraOffset();
			//forward = Vector3.XAxis;
			//up = Vector3.ZAxis;

			//!!!!coefficient in properties

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

			//Character: wiggle camera when walking
			//if( EntitySystemWorld.Instance.Simulation && !EntitySystemWorld.Instance.SystemPauseOfSimulation )
			{

				//!!!!slowly?

				//change position
				{
					var angle = Time.Current * 10;
					var radius = wiggleWhenWalkingSpeedFactor * .04f;
					Vector3 localPosition = new Vector3( 0,
						Math.Cos( angle ) * radius,
						Math.Abs( Math.Sin( angle ) * radius ) );
					position += localPosition * tr.ToMatrix4( false, true, true );//GetInterpolatedRotation();
				}

				//change up vector
				{
					var angle = Time.Current * 20;
					var radius = wiggleWhenWalkingSpeedFactor * .003f;
					Vector3 localUp = new Vector3(
						Math.Cos( angle ) * radius,
						Math.Sin( angle ) * radius, 1 );
					localUp.Normalize();
					up = localUp * tr.Rotation;//GetInterpolatedRotation();
				}
			}

			//calculate forward
			forward = CurrentTurnToDirection.GetVector();
		}

		public Vector3 GetSmoothPosition()
		{
			return TransformV.Position + GetSmoothCameraOffset();
		}

		void UpdateTransformOffsetInSimulation()
		{
			var meshInSpace = GetComponent<Component_MeshInSpace>( onlyEnabledInHierarchy: true );
			if( meshInSpace != null )
			{
				var transformOffset = meshInSpace.GetComponent<Component_TransformOffset>( onlyEnabledInHierarchy: true );
				if( transformOffset != null )
				{
					if( initialTransformOffsetPositionInSimulation == null )
						initialTransformOffsetPositionInSimulation = transformOffset.PositionOffset;

					transformOffset.PositionOffset = initialTransformOffsetPositionInSimulation.Value + GetSmoothCameraOffset();
				}
			}
		}

		public Component_MeshInSpaceAnimationController GetAnimationController()
		{
			var meshInSpace = GetComponent<Component_MeshInSpace>( onlyEnabledInHierarchy: true );
			if( meshInSpace != null )
				return meshInSpace.GetComponent<Component_MeshInSpaceAnimationController>( onlyEnabledInHierarchy: true );
			return null;
		}

		void OnAnimateChanged()
		{
			//reset
			if( !Animate )
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

		//!!!!temp
		List<Vector3> _tempDebug = new List<Vector3>();

		protected virtual void AdditionalBoneTransformsUpdate( Component_MeshInSpaceAnimationController controller, Component_MeshInSpaceAnimationController.AnimationStateClass animationState, Component_Skeleton skeleton, Component_SkeletonAnimationTrack.CalculateBoneTransformsItem[] result, ref bool updateTwice, int updateIteration )
		{
			//!!!!
			_tempDebug.Clear();

			var meshInSpace = GetComponent<Component_MeshInSpace>( onlyEnabledInHierarchy: true );
			if( meshInSpace == null )
				return;
			if( controller.Bones == null )
				return;

			var inverseTransformCalculated = false;
			Matrix4 inverseTransform = Matrix4.Zero;

			//hands
			for( int nSide = 0; nSide < 2; nSide++ )
			{
				var left = nSide == 0;

				var factor = left ? LeftHandFactor.Value : RightHandFactor.Value;
				if( factor > 0 )
				{
					//update skeleton twice because during calculation the data of bone transforms taken from previous update
					updateTwice = true;

					if( updateIteration == 1 )
					{
						var worldHandTransform = left ? LeftHandTransform.Value : RightHandTransform.Value;
						//var worldHandPosition = left ? LeftHandPosition.Value : RightHandPosition.Value;

						//inverseTransform
						if( !inverseTransformCalculated )
						{
							meshInSpace.TransformV.ToMatrix4().GetInverse( out inverseTransform );
							inverseTransformCalculated = true;
						}

						var objectHandPosition = inverseTransform * worldHandTransform.Position;
						var objectHandRotation = Quaternion.LookAt( inverseTransform * worldHandTransform.Rotation.GetForward(), inverseTransform * worldHandTransform.Rotation.GetUp() );

						//!!!!
						//_tempDebug.Add( localHandPosition );


						var handBoneIndex = controller.GetBoneIndex( left ? LeftHandBone : RightHandBone );
						if( handBoneIndex >= 0 && handBoneIndex < result.Length )
						{
							var handBoneComponent = controller.Bones[ handBoneIndex ];

							var foreArmBoneComponent = handBoneComponent.Parent as Component_SkeletonBone;
							if( foreArmBoneComponent != null )
							{
								var foreArmBoneIndex = controller.GetBoneIndex( foreArmBoneComponent.Name );
								if( foreArmBoneIndex >= 0 && foreArmBoneIndex < result.Length )
								{
									var armBoneComponent = foreArmBoneComponent.Parent as Component_SkeletonBone;
									if( armBoneComponent != null )
									{
										var armBoneIndex = controller.GetBoneIndex( armBoneComponent.Name );
										if( armBoneIndex >= 0 && armBoneIndex < result.Length )
										{
											var shoulderBoneComponent = armBoneComponent.Parent as Component_SkeletonBone;
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
							meshInSpace.TransformV.ToMatrix4().GetInverse( out inverseTransform );
							inverseTransformCalculated = true;
						}

						var localLookAt = inverseTransform * worldLookAt;

						//!!!!
						//_tempDebug.Add( localHandPosition );


						var headBoneIndex = controller.GetBoneIndex( HeadBone );
						if( headBoneIndex >= 0 && headBoneIndex < result.Length )
						{
							var headBoneComponent = controller.Bones[ headBoneIndex ];

							var neckBoneComponent = headBoneComponent.Parent as Component_SkeletonBone;
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

			if( Animate )
			{
				var controller = GetAnimationController();
				if( controller != null )
				{
					Component_Animation animation = null;
					double speed = 1;
					bool autoRewind = true;

					if( IsOnGroundWithLatency() )
					{
						var localVelocity = GetTransform().Rotation.GetInverse() * GetLinearVelocity();
						var linearSpeedNoScale = ( localVelocity.X + Math.Abs( localVelocity.Y ) * 0.5 ) / GetScaleFactor();

						//RunAnimation
						if( RunSupport )
						{
							var running = Math.Abs( linearSpeedNoScale ) > RunForwardMaxSpeed * 0.8;
							if( running )
							{
								animation = RunAnimation;
								if( animation != null )
									speed = RunAnimationSpeed * linearSpeedNoScale;
							}
						}

						//WalkAnimation
						if( animation == null )
						{
							var walking = Math.Abs( linearSpeedNoScale ) > WalkForwardMaxSpeed * 0.2;
							if( walking )
							{
								animation = WalkAnimation;
								if( animation != null )
									speed = WalkAnimationSpeed * linearSpeedNoScale;
							}
						}

						//Left Turn, Right Turn
						if( animation == null )
						{
							//!!!!vertical
							if( currentTurnToDirection.Horizontal != requiredTurnToDirection.Horizontal && IsOnGround() )
							//if( currentTurnToDirection != requiredTurnToDirection && IsOnGround() )
							{
								bool leftTurn;
								{
									var angle = requiredTurnToDirection.Horizontal - currentTurnToDirection.Horizontal;
									var d = Math.Sin( angle );
									leftTurn = d > 0;
								}

								animation = leftTurn ? LeftTurnAnimation : RightTurnAnimation;
							}
						}

					}
					else
						animation = FlyAnimation;

					if( animation == null )
						animation = IdleAnimation;

					//play one animation
					if( playOneAnimation != null )
					{
						animation = playOneAnimation;
						speed = playOneAnimationSpeed;
						autoRewind = false;
					}

					var state = new Component_MeshInSpaceAnimationController.AnimationStateClass();
					state.Animations.Add( new Component_MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = animation, Speed = speed, AutoRewind = autoRewind } );

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
				var body = CreateComponent<Component_RigidBody>();
				body.Name = "Collision Body";
				body.CanBeSelected = false;
				body.MotionType = Component_RigidBody.MotionTypeEnum.Dynamic;

				var meshInSpace = CreateComponent<Component_MeshInSpace>();
				meshInSpace.Name = "Mesh In Space";
				meshInSpace.CanBeSelected = false;
				meshInSpace.Mesh = new Reference<Component_Mesh>( null, "Base\\Models\\Human.fbx|$Mesh" );
				meshInSpace.Transform = new Reference<Transform>( NeoAxis.Transform.Identity, "this:$Transform Offset\\Result" );

				var controller = meshInSpace.CreateComponent<Component_MeshInSpaceAnimationController>();
				controller.Name = "Mesh In Space Animation Controller";

				var transformOffset = meshInSpace.CreateComponent<Component_TransformOffset>();
				transformOffset.Name = "Transform Offset";
				transformOffset.PositionOffset = new Vector3( 0, 0, -1.15 );
				transformOffset.Source = new Reference<Transform>( NeoAxis.Transform.Identity, "this:..\\..\\$Collision Body\\Transform" );

				var inputProcessing = CreateComponent<Component_CharacterInputProcessing>();
				inputProcessing.Name = "Character Input Processing";

				var characterAI = CreateComponent<Component_CharacterAI>();
				characterAI.Name = "Character AI";

				RunSupport = true;
				FlyControlSupport = true;
				JumpSupport = true;
				Animate = true;
				IdleAnimation = new Reference<Component_Animation>( null, "Base\\Models\\Human.fbx|$Mesh\\$Animations\\$Idle" );
				WalkAnimation = new Reference<Component_Animation>( null, "Base\\Models\\Human.fbx|$Mesh\\$Animations\\$Walk" );
				RunAnimation = new Reference<Component_Animation>( null, "Base\\Models\\Human.fbx|$Mesh\\$Animations\\$Run" );
				FlyAnimation = new Reference<Component_Animation>( null, "Base\\Models\\Human.fbx|$Mesh\\$Animations\\$Fly" );
				JumpAnimation = new Reference<Component_Animation>( null, "Base\\Models\\Human.fbx|$Mesh\\$Animations\\$Jump" );
				LeftTurnAnimation = new Reference<Component_Animation>( null, "Base\\Models\\Human.fbx|$Mesh\\$Animations\\$Left Turn" );
				RightTurnAnimation = new Reference<Component_Animation>( null, "Base\\Models\\Human.fbx|$Mesh\\$Animations\\$Right Turn" );
				WalkAnimationSpeed = 0.55;
				RunAnimationSpeed = 0.2;
			}
		}

		public bool IsFreePositionToMove( Vector3 position )
		{
			//var bodyTransform = mainBody.TransformV;
			//var pos = bodyTransform.Position + shape.TransformRelativeToParent.Value.Position;
			//var rot = bodyTransform.Rotation;

			Capsule[] volumeCapsules = GetVolumeCapsules();
			//make radius smaller
			for( int n = 0; n < volumeCapsules.Length; n++ )
			{
				Capsule capsule = volumeCapsules[ n ];
				capsule.Radius *= .99f;
				volumeCapsules[ n ] = capsule;
			}

			foreach( Capsule sourceVolumeCapsule in volumeCapsules )
			{
				var offset = position - sourceVolumeCapsule.GetCenter();
				Capsule checkCapsule = CapsuleAddOffset( sourceVolumeCapsule, offset );

				var scene = ParentScene;
				if( scene != null )
				{
					var contactTestItem = new PhysicsContactTestItem( 1, -1, PhysicsContactTestItem.ModeEnum.All, checkCapsule );
					ParentScene.PhysicsContactTest( contactTestItem );

					foreach( var item in contactTestItem.Result )
					{
						if( item.Body == mainBody )
							continue;
						var body = item.Body as Component_RigidBody;
						if( body != null )
						{
							return false;
							//collisionBodies.Add( body );
						}
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
		public void ItemTake( IComponent_Item item )
		{
			var item2 = (Component_ObjectInSpace)item;

			//check already taken
			if( item2.Parent == this )
				return;

			//disable
			item2.Enabled = false;

			//detach
			Component_ObjectInSpace_Utility.Detach( item2 );
			item2.RemoveFromParent( false );

			var originalScale = item2.TransformV.Scale;

			//attach
			AddComponent( item2 );
			item2.Transform = Transform.Value;
			var transformOffset = Component_ObjectInSpace_Utility.Attach( this, item2 );
			if( transformOffset != null )
				transformOffset.ScaleOffset = originalScale / GetScaleFactor();
		}

		/// <summary>
		/// Drops the item. The item will moved to the scene and will enabled.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="newTransform"></param>
		public void ItemDrop( IComponent_Item item, Transform newTransform = null )
		{
			var item2 = (Component_ObjectInSpace)item;

			//check can drop
			if( item2.Parent != this )
				return;

			//disable
			item2.Enabled = false;

			//detach
			Component_ObjectInSpace_Utility.Detach( item2 );
			item2.RemoveFromParent( false );

			//add to the scene
			ParentScene.AddComponent( item2 );
			if( newTransform != null )
				item2.Transform = newTransform;

			//enable
			item2.Enabled = true;
		}

		/// <summary>
		/// Activates the item. The item will enabled.
		/// </summary>
		/// <param name="item"></param>
		public void ItemActivate( IComponent_Item item )
		{
			var item2 = (Component_ObjectInSpace)item;
			item2.Enabled = true;
		}

		/// <summary>
		/// Deactivates the item. The item will disabled.
		/// </summary>
		/// <param name="item"></param>
		public void ItemDeactivate( IComponent_Item item )
		{
			var item2 = (Component_ObjectInSpace)item;
			item2.Enabled = false;
		}

		/// <summary>
		/// Returns first activated item of the character.
		/// </summary>
		/// <returns></returns>
		public IComponent_Item ItemGetEnabledFirst()
		{
			foreach( var c in Components )
				if( c.Enabled && c is IComponent_Item item )
					return item;
			return null;
		}

		protected virtual void UpdateEnabledItemTransform()
		{
			var item = ItemGetEnabledFirst();
			if( item != null )
			{
				var obj = item as Component_ObjectInSpace;
				if( obj != null )
				{
					var offset = obj.GetComponent<Component_TransformOffset>();
					if( offset != null )
					{
						//!!!!add customization support

						offset.PositionOffset = new Vector3( 0, 0, 0.3 );

						offset.RotationOffset = Quaternion.FromRotateByY( CurrentTurnToDirection.Vertical );
					}
				}
			}
		}

		public void SoundPlay( Component_Sound sound )
		{
			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		public void StartPlayOneAnimation( Component_Animation animation, double speed = 1.0 )
		{
			playOneAnimation = animation;
			playOneAnimationSpeed = speed;

			if( playOneAnimation != null )
			{
				playOneAnimationRemainingTime = playOneAnimation.Length * playOneAnimationSpeed;

				var controller = GetAnimationController();
				if( controller != null && playOneAnimationRemainingTime > controller.InterpolationTime.Value )
					playOneAnimationRemainingTime -= controller.InterpolationTime.Value;
			}
			else
				playOneAnimationRemainingTime = 0;
		}

		[Browsable( false )]
		public Component_Animation PlayOneAnimation
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
			//!!!!vertical
			if( currentTurnToDirection.Horizontal != requiredTurnToDirection.Horizontal && IsOnGround() )
			//if( currentTurnToDirection != requiredTurnToDirection && IsOnGround() )
			{
				var step = (double)TurningSpeed.Value.InRadians() * Time.SimulationDelta;

				bool leftTurn;
				{
					var angle = requiredTurnToDirection.Horizontal - currentTurnToDirection.Horizontal;
					var d = Math.Sin( angle );
					leftTurn = d > 0;
				}

				currentTurnToDirection.Horizontal += leftTurn ? step : -step;

				bool newLeftTurn;
				{
					var angle = requiredTurnToDirection.Horizontal - currentTurnToDirection.Horizontal;
					var d = Math.Sin( angle );
					newLeftTurn = d > 0;
				}

				if( newLeftTurn != leftTurn )
					currentTurnToDirection.Horizontal = requiredTurnToDirection.Horizontal;
			}
		}
	}
}
