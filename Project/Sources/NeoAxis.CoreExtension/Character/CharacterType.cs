// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	public enum HandEnum
	{
		Left,
		Right,
	}

	/// <summary>
	/// A definition of the character type.
	/// </summary>
	[ResourceFileExtension( "charactertype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Base\3D\Character Type", -8998 )]
	[EditorControl( typeof( CharacterTypeEditor ) )]
	[Preview( typeof( CharacterTypePreview ) )]
	[PreviewImage( typeof( CharacterTypePreviewImage ) )]
#endif
	public class CharacterType : Component
	{
		int version;

		//

		//!!!!
		//динамически получаемым по идее не нужно
		//DataWasChanged()


		/////////////////////////////////////////
		//Basic

		const string meshDefault = @"Content\Characters\Default\Human.fbx|$Mesh";

		/// <summary>
		/// The mesh of the character.
		/// </summary>
		[DefaultValueReference( meshDefault )]
		[Category( "Basic" )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<CharacterType> MeshChanged;
		ReferenceField<Mesh> _mesh = new Reference<Mesh>( null, meshDefault );

		/// <summary>
		/// The height of the character.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 1.8 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set { if( _height.BeginSet( ref value ) ) { try { HeightChanged?.Invoke( this ); DataWasChanged(); } finally { _height.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<CharacterType> HeightChanged;
		ReferenceField<double> _height = 1.8;

		/// <summary>
		/// The radius of the collision capsule.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 0.35 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set { if( _radius.BeginSet( ref value ) ) { try { RadiusChanged?.Invoke( this ); DataWasChanged(); } finally { _radius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<CharacterType> RadiusChanged;
		ReferenceField<double> _radius = 0.35;

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
		public event Action<CharacterType> MassChanged;
		ReferenceField<double> _mass = 70;

		//!!!!name climb?
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
		public event Action<CharacterType> WalkUpHeightChanged;
		ReferenceField<double> _walkUpHeight = 0.6;

		///// <summary>
		///// The distance from the character position to the ground.
		///// </summary>
		//[Category( "Basic" )]
		//[DefaultValue( 1.15 )]
		//public Reference<double> PositionToGroundHeight
		//{
		//	get { if( _positionToGroundHeight.BeginGet() ) PositionToGroundHeight = _positionToGroundHeight.Get( this ); return _positionToGroundHeight.value; }
		//	set { if( _positionToGroundHeight.BeginSet( ref value ) ) { try { PositionToGroundHeightChanged?.Invoke( this ); } finally { _positionToGroundHeight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PositionToGroundHeight"/> property value changes.</summary>
		//public event Action<CharacterType> PositionToGroundHeightChanged;
		//ReferenceField<double> _positionToGroundHeight = 1.15;

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
		public event Action<CharacterType> MaxSlopeAngleChanged;
		ReferenceField<Degree> _maxSlopeAngle = new Degree( 50 );

		/// <summary>
		/// The position of the eyes relative to the position of the character.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( "0.23 0 1.73" )]
		public Reference<Vector3> EyePosition
		{
			get { if( _eyePosition.BeginGet() ) EyePosition = _eyePosition.Get( this ); return _eyePosition.value; }
			set { if( _eyePosition.BeginSet( ref value ) ) { try { EyePositionChanged?.Invoke( this ); } finally { _eyePosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EyePosition"/> property value changes.</summary>
		public event Action<CharacterType> EyePositionChanged;
		ReferenceField<Vector3> _eyePosition = new Vector3( 0.23, 0, 1.73 );

		/// <summary>
		/// Whether to consider the speed of the body on which the character is standing.
		/// </summary>
		[Category( "Physics" )]
		[DefaultValue( false )]
		public Reference<bool> ApplyGroundVelocity
		{
			get { if( _applyGroundVelocity.BeginGet() ) ApplyGroundVelocity = _applyGroundVelocity.Get( this ); return _applyGroundVelocity.value; }
			set { if( _applyGroundVelocity.BeginSet( ref value ) ) { try { ApplyGroundVelocityChanged?.Invoke( this ); } finally { _applyGroundVelocity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ApplyGroundVelocity"/> property value changes.</summary>
		public event Action<CharacterType> ApplyGroundVelocityChanged;
		ReferenceField<bool> _applyGroundVelocity = false;

		[Category( "Physics" )]
		[DefaultValue( 1.0 )]
		public Reference<double> MinSpeedToSleepBody
		{
			get { if( _minSpeedToSleepBody.BeginGet() ) MinSpeedToSleepBody = _minSpeedToSleepBody.Get( this ); return _minSpeedToSleepBody.value; }
			set { if( _minSpeedToSleepBody.BeginSet( ref value ) ) { try { MinSpeedToSleepBodyChanged?.Invoke( this ); } finally { _minSpeedToSleepBody.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MinSpeedToSleepBody"/> property value changes.</summary>
		public event Action<CharacterType> MinSpeedToSleepBodyChanged;
		ReferenceField<double> _minSpeedToSleepBody = 1.0;

		//!!!!default
		/// <summary>
		/// The value of linear dumping when a character is standing on the ground.
		/// </summary>
		[Category( "Physics" )]
		[DefaultValue( 5 )]
		public Reference<double> LinearDampingOnGroundIdle
		{
			get { if( _linearDampingOnGroundIdle.BeginGet() ) LinearDampingOnGroundIdle = _linearDampingOnGroundIdle.Get( this ); return _linearDampingOnGroundIdle.value; }
			set { if( _linearDampingOnGroundIdle.BeginSet( ref value ) ) { try { LinearDampingOnGroundIdleChanged?.Invoke( this ); } finally { _linearDampingOnGroundIdle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearDampingOnGroundIdle"/> property value changes.</summary>
		public event Action<CharacterType> LinearDampingOnGroundIdleChanged;
		ReferenceField<double> _linearDampingOnGroundIdle = 5;

		//!!!!default
		/// <summary>
		/// The value of linear dumping when a character is standing on the ground.
		/// </summary>
		[Category( "Physics" )]
		[DefaultValue( 3 )]//0.99 )]
		public Reference<double> LinearDampingOnGroundMove
		{
			get { if( _linearDampingOnGround.BeginGet() ) LinearDampingOnGroundMove = _linearDampingOnGround.Get( this ); return _linearDampingOnGround.value; }
			set { if( _linearDampingOnGround.BeginSet( ref value ) ) { try { LinearDampingOnGroundChanged?.Invoke( this ); } finally { _linearDampingOnGround.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearDampingOnGroundMove"/> property value changes.</summary>
		public event Action<CharacterType> LinearDampingOnGroundChanged;
		ReferenceField<double> _linearDampingOnGround = 3;//0.99;

		//!!!!
		/// <summary>
		/// The value of linear dumping when a character is flying.
		/// </summary>
		[Category( "Physics" )]
		[DefaultValue( 0.15 )]
		public Reference<double> LinearDampingFly
		{
			get { if( _linearDampingFly.BeginGet() ) LinearDampingFly = _linearDampingFly.Get( this ); return _linearDampingFly.value; }
			set { if( _linearDampingFly.BeginSet( ref value ) ) { try { LinearDampingFlyChanged?.Invoke( this ); } finally { _linearDampingFly.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearDampingFly"/> property value changes.</summary>
		public event Action<CharacterType> LinearDampingFlyChanged;
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
		public event Action<CharacterType> WalkForwardMaxSpeedChanged;
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
		public event Action<CharacterType> WalkBackwardMaxSpeedChanged;
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
		public event Action<CharacterType> WalkSideMaxSpeedChanged;
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
		public event Action<CharacterType> WalkForceChanged;
		ReferenceField<double> _walkForce = 100000;

		/////////////////////////////////////////
		//Run

		/// <summary>
		/// Can the character run.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( true )]
		public Reference<bool> Run
		{
			get { if( _run.BeginGet() ) Run = _run.Get( this ); return _run.value; }
			set { if( _run.BeginSet( ref value ) ) { try { RunChanged?.Invoke( this ); } finally { _run.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Run"/> property value changes.</summary>
		public event Action<CharacterType> RunChanged;
		ReferenceField<bool> _run = true;

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
		public event Action<CharacterType> RunForwardMaxSpeedChanged;
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
		public event Action<CharacterType> RunBackwardMaxSpeedChanged;
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
		public event Action<CharacterType> RunSideMaxSpeedChanged;
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
		public event Action<CharacterType> RunForceChanged;
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
		public event Action<CharacterType> TurningSpeedChanged;
		ReferenceField<Degree> _turningSpeed = new Degree( 90.0 );

		/////////////////////////////////////////
		//Fly control

		/// <summary>
		/// Can a character control himself in flight.
		/// </summary>
		[Category( "Fly Control" )]
		[DefaultValue( true )]
		public Reference<bool> FlyControl
		{
			get { if( _flyControl.BeginGet() ) FlyControl = _flyControl.Get( this ); return _flyControl.value; }
			set { if( _flyControl.BeginSet( ref value ) ) { try { FlyControlChanged?.Invoke( this ); } finally { _flyControl.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyControl"/> property value changes.</summary>
		public event Action<CharacterType> FlyControlChanged;
		ReferenceField<bool> _flyControl = true;

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
		public event Action<CharacterType> FlyControlMaxSpeedChanged;
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
		public event Action<CharacterType> FlyControlForceChanged;
		ReferenceField<double> _flyControlForce = 10000;

		/////////////////////////////////////////
		//Jump

		/// <summary>
		/// Can the character jump.
		/// </summary>
		[Category( "Jump" )]
		[DefaultValue( true )]
		public Reference<bool> Jump
		{
			get { if( _jump.BeginGet() ) Jump = _jump.Get( this ); return _jump.value; }
			set { if( _jump.BeginSet( ref value ) ) { try { JumpChanged?.Invoke( this ); } finally { _jump.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Jump"/> property value changes.</summary>
		public event Action<CharacterType> JumpChanged;
		ReferenceField<bool> _jump = true;

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
		public event Action<CharacterType> JumpSpeedChanged;
		ReferenceField<double> _jumpSpeed = 4;

		[Category( "Jump" )]
		[DefaultValue( null )]
		public Reference<Sound> JumpSound
		{
			get { if( _jumpSound.BeginGet() ) JumpSound = _jumpSound.Get( this ); return _jumpSound.value; }
			set { if( _jumpSound.BeginSet( ref value ) ) { try { JumpSoundChanged?.Invoke( this ); } finally { _jumpSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpSound"/> property value changes.</summary>
		public event Action<CharacterType> JumpSoundChanged;
		ReferenceField<Sound> _jumpSound = null;

		/////////////////////////////////////////
		//Crouching

		//!!!!crouching is disabled
		[Browsable( false )]
		[Category( "Crouching" )]
		[DefaultValue( false )]
		public Reference<bool> Crouching
		{
			get { if( _crouching.BeginGet() ) Crouching = _crouching.Get( this ); return _crouching.value; }
			set { if( _crouching.BeginSet( ref value ) ) { try { CrouchingChanged?.Invoke( this ); } finally { _crouching.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Crouching"/> property value changes.</summary>
		public event Action<CharacterType> CrouchingChanged;
		ReferenceField<bool> _crouching = false;

		[Category( "Crouching" )]
		[DefaultValue( 1.0 )]
		public Reference<double> CrouchingHeight
		{
			get { if( _crouchingHeight.BeginGet() ) CrouchingHeight = _crouchingHeight.Get( this ); return _crouchingHeight.value; }
			set { if( _crouchingHeight.BeginSet( ref value ) ) { try { CrouchingHeightChanged?.Invoke( this ); } finally { _crouchingHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingHeight"/> property value changes.</summary>
		public event Action<CharacterType> CrouchingHeightChanged;
		ReferenceField<double> _crouchingHeight = 1.0;

		[Category( "Crouching" )]
		[DefaultValue( 0.1 )]
		public Reference<double> CrouchingWalkUpHeight
		{
			get { if( _crouchingWalkUpHeight.BeginGet() ) CrouchingWalkUpHeight = _crouchingWalkUpHeight.Get( this ); return _crouchingWalkUpHeight.value; }
			set { if( _crouchingWalkUpHeight.BeginSet( ref value ) ) { try { CrouchingWalkUpHeightChanged?.Invoke( this ); } finally { _crouchingWalkUpHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingWalkUpHeight"/> property value changes.</summary>
		public event Action<CharacterType> CrouchingWalkUpHeightChanged;
		ReferenceField<double> _crouchingWalkUpHeight = 0.1;

		//[Category( "Crouching" )]
		//[DefaultValue( 0.55 )]
		//public Reference<double> CrouchingPositionToGroundHeight
		//{
		//	get { if( _crouchingPositionToGroundHeight.BeginGet() ) CrouchingPositionToGroundHeight = _crouchingPositionToGroundHeight.Get( this ); return _crouchingPositionToGroundHeight.value; }
		//	set { if( _crouchingPositionToGroundHeight.BeginSet( ref value ) ) { try { CrouchingPositionToGroundHeightChanged?.Invoke( this ); } finally { _crouchingPositionToGroundHeight.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CrouchingPositionToGroundHeight"/> property value changes.</summary>
		//public event Action<CharacterType> CrouchingPositionToGroundHeightChanged;
		//ReferenceField<double> _crouchingPositionToGroundHeight = 0.55;

		[Category( "Crouching" )]
		[DefaultValue( 1.0 )]
		public Reference<double> CrouchingMaxSpeed
		{
			get { if( _crouchingMaxSpeed.BeginGet() ) CrouchingMaxSpeed = _crouchingMaxSpeed.Get( this ); return _crouchingMaxSpeed.value; }
			set { if( _crouchingMaxSpeed.BeginSet( ref value ) ) { try { CrouchingMaxSpeedChanged?.Invoke( this ); } finally { _crouchingMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingMaxSpeed"/> property value changes.</summary>
		public event Action<CharacterType> CrouchingMaxSpeedChanged;
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
		public event Action<CharacterType> CrouchingForceChanged;
		ReferenceField<double> _crouchingForce = 100000;

		/////////////////////////////////////////
		//Animate

		/// <summary>
		/// Whether to enable default animation method of the character.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( true )]
		public Reference<bool> Animate
		{
			get { if( _animate.BeginGet() ) Animate = _animate.Get( this ); return _animate.value; }
			set { if( _animate.BeginSet( ref value ) ) { try { AnimateChanged?.Invoke( this ); DataWasChanged(); } finally { _animate.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Animate"/> property value changes.</summary>
		public event Action<CharacterType> AnimateChanged;
		ReferenceField<bool> _animate = true;

		const string idleAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Idle";

		/// <summary>
		/// Animation of character at rest.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValueReference( idleAnimationDefault )]
		public Reference<Animation> IdleAnimation
		{
			get { if( _idleAnimation.BeginGet() ) IdleAnimation = _idleAnimation.Get( this ); return _idleAnimation.value; }
			set { if( _idleAnimation.BeginSet( ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IdleAnimation"/> property value changes.</summary>
		public event Action<CharacterType> IdleAnimationChanged;
		ReferenceField<Animation> _idleAnimation = new Reference<Animation>( null, idleAnimationDefault );

		const string walkAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Walk";

		/// <summary>
		/// Character walking animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValueReference( walkAnimationDefault )]
		public Reference<Animation> WalkAnimation
		{
			get { if( _walkAnimation.BeginGet() ) WalkAnimation = _walkAnimation.Get( this ); return _walkAnimation.value; }
			set { if( _walkAnimation.BeginSet( ref value ) ) { try { WalkAnimationChanged?.Invoke( this ); } finally { _walkAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkAnimation"/> property value changes.</summary>
		public event Action<CharacterType> WalkAnimationChanged;
		ReferenceField<Animation> _walkAnimation = new Reference<Animation>( null, walkAnimationDefault );

		/// <summary>
		/// The multiplier for playing the walking animation depending on the speed of the character.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( 0.55 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> WalkAnimationSpeed
		{
			get { if( _walkAnimationSpeed.BeginGet() ) WalkAnimationSpeed = _walkAnimationSpeed.Get( this ); return _walkAnimationSpeed.value; }
			set { if( _walkAnimationSpeed.BeginSet( ref value ) ) { try { WalkAnimationSpeedChanged?.Invoke( this ); } finally { _walkAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkAnimationSpeed"/> property value changes.</summary>
		public event Action<CharacterType> WalkAnimationSpeedChanged;
		ReferenceField<double> _walkAnimationSpeed = 0.55;

		const string runAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Run";

		/// <summary>
		/// Character running animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValueReference( runAnimationDefault )]
		public Reference<Animation> RunAnimation
		{
			get { if( _runAnimation.BeginGet() ) RunAnimation = _runAnimation.Get( this ); return _runAnimation.value; }
			set { if( _runAnimation.BeginSet( ref value ) ) { try { RunAnimationChanged?.Invoke( this ); } finally { _runAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunAnimation"/> property value changes.</summary>
		public event Action<CharacterType> RunAnimationChanged;
		ReferenceField<Animation> _runAnimation = new Reference<Animation>( null, runAnimationDefault );

		/// <summary>
		/// The multiplier for playing the running animation depending on the speed of the character.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( 0.2 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> RunAnimationSpeed
		{
			get { if( _runAnimationSpeed.BeginGet() ) RunAnimationSpeed = _runAnimationSpeed.Get( this ); return _runAnimationSpeed.value; }
			set { if( _runAnimationSpeed.BeginSet( ref value ) ) { try { RunAnimationSpeedChanged?.Invoke( this ); } finally { _runAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunAnimationSpeed"/> property value changes.</summary>
		public event Action<CharacterType> RunAnimationSpeedChanged;
		ReferenceField<double> _runAnimationSpeed = 0.2;

		const string flyAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Fly";

		/// <summary>
		/// Character flying animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValueReference( flyAnimationDefault )]
		public Reference<Animation> FlyAnimation
		{
			get { if( _flyAnimation.BeginGet() ) FlyAnimation = _flyAnimation.Get( this ); return _flyAnimation.value; }
			set { if( _flyAnimation.BeginSet( ref value ) ) { try { FlyAnimationChanged?.Invoke( this ); } finally { _flyAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyAnimation"/> property value changes.</summary>
		public event Action<CharacterType> FlyAnimationChanged;
		ReferenceField<Animation> _flyAnimation = new Reference<Animation>( null, flyAnimationDefault );

		const string jumpAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Jump";

		/// <summary>
		/// Character jump animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValueReference( jumpAnimationDefault )]
		public Reference<Animation> JumpAnimation
		{
			get { if( _jumpAnimation.BeginGet() ) JumpAnimation = _jumpAnimation.Get( this ); return _jumpAnimation.value; }
			set { if( _jumpAnimation.BeginSet( ref value ) ) { try { JumpAnimationChanged?.Invoke( this ); } finally { _jumpAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpAnimation"/> property value changes.</summary>
		public event Action<CharacterType> JumpAnimationChanged;
		ReferenceField<Animation> _jumpAnimation = new Reference<Animation>( null, jumpAnimationDefault );

		const string leftTurnAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Left Turn";

		/// <summary>
		/// Character left turn animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValueReference( leftTurnAnimationDefault )]
		public Reference<Animation> LeftTurnAnimation
		{
			get { if( _leftTurnAnimation.BeginGet() ) LeftTurnAnimation = _leftTurnAnimation.Get( this ); return _leftTurnAnimation.value; }
			set { if( _leftTurnAnimation.BeginSet( ref value ) ) { try { LeftTurnAnimationChanged?.Invoke( this ); } finally { _leftTurnAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftTurnAnimation"/> property value changes.</summary>
		public event Action<CharacterType> LeftTurnAnimationChanged;
		ReferenceField<Animation> _leftTurnAnimation = new Reference<Animation>( null, leftTurnAnimationDefault );

		const string rightTurnAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Right Turn";

		/// <summary>
		/// Character left turn animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValueReference( rightTurnAnimationDefault )]
		public Reference<Animation> RightTurnAnimation
		{
			get { if( _rightTurnAnimation.BeginGet() ) RightTurnAnimation = _rightTurnAnimation.Get( this ); return _rightTurnAnimation.value; }
			set { if( _rightTurnAnimation.BeginSet( ref value ) ) { try { RightTurnAnimationChanged?.Invoke( this ); } finally { _rightTurnAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightTurnAnimation"/> property value changes.</summary>
		public event Action<CharacterType> RightTurnAnimationChanged;
		ReferenceField<Animation> _rightTurnAnimation = new Reference<Animation>( null, rightTurnAnimationDefault );

		/////////////////////////////////////////

		//[Category( "Skeleton State" )]
		//[DefaultValue( "spine" )] //!!!!? chest
		//public Reference<string> TorsoBone
		//{
		//	get { if( _torsoBone.BeginGet() ) TorsoBone = _torsoBone.Get( this ); return _torsoBone.value; }
		//	set { if( _torsoBone.BeginSet( ref value ) ) { try { TorsoBoneChanged?.Invoke( this ); } finally { _torsoBone.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TorsoBone"/> property value changes.</summary>
		//public event Action<CharacterType> TorsoBoneChanged;
		//ReferenceField<string> _torsoBone = "spine";

		/// <summary>
		/// The name of the left hand bone.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:LeftHand" )]
		public Reference<string> LeftHandBone
		{
			get { if( _leftHandBone.BeginGet() ) LeftHandBone = _leftHandBone.Get( this ); return _leftHandBone.value; }
			set { if( _leftHandBone.BeginSet( ref value ) ) { try { LeftHandBoneChanged?.Invoke( this ); } finally { _leftHandBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandBone"/> property value changes.</summary>
		public event Action<CharacterType> LeftHandBoneChanged;
		ReferenceField<string> _leftHandBone = "mixamorig:LeftHand";

		/// <summary>
		/// The name of the right hand bone.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:RightHand" )]
		public Reference<string> RightHandBone
		{
			get { if( _rightHandBone.BeginGet() ) RightHandBone = _rightHandBone.Get( this ); return _rightHandBone.value; }
			set { if( _rightHandBone.BeginSet( ref value ) ) { try { RightHandBoneChanged?.Invoke( this ); } finally { _rightHandBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandBone"/> property value changes.</summary>
		public event Action<CharacterType> RightHandBoneChanged;
		ReferenceField<string> _rightHandBone = "mixamorig:RightHand";

		/// <summary>
		/// The name of the head body.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:Head" )]
		public Reference<string> HeadBone
		{
			get { if( _headBone.BeginGet() ) HeadBone = _headBone.Get( this ); return _headBone.value; }
			set { if( _headBone.BeginSet( ref value ) ) { try { HeadBoneChanged?.Invoke( this ); } finally { _headBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadBone"/> property value changes.</summary>
		public event Action<CharacterType> HeadBoneChanged;
		ReferenceField<string> _headBone = "mixamorig:Head";

		//[Category( "Skeleton State" )]
		//[DefaultValue( 0.0 )]
		//[Range( -0.25, 1 )]
		//public Reference<double> LeftHandThumbFingerFlexion
		//{
		//	get { if( _leftHandThumbFingerFlexion.BeginGet() ) LeftHandThumbFingerFlexion = _leftHandThumbFingerFlexion.Get( this ); return _leftHandThumbFingerFlexion.value; }
		//	set { if( _leftHandThumbFingerFlexion.BeginSet( ref value ) ) { try { LeftHandThumbFingerFlexionChanged?.Invoke( this ); } finally { _leftHandThumbFingerFlexion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeftHandThumbFingerFlexion"/> property value changes.</summary>
		//public event Action<CharacterType> LeftHandThumbFingerFlexionChanged;
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
		//public event Action<CharacterType> LeftHandIndexFingerFlexionChanged;
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
		//public event Action<CharacterType> LeftHandMiddleFingerFlexionChanged;
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
		//public event Action<CharacterType> LeftHandRingFingerFlexionChanged;
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
		//public event Action<CharacterType> LeftHandPinkyFingerFlexionChanged;
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
		//public event Action<CharacterType> RightHandThumbFingerFlexionChanged;
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
		//public event Action<CharacterType> RightHandIndexFingerFlexionChanged;
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
		//public event Action<CharacterType> RightHandMiddleFingerFlexionChanged;
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
		//public event Action<CharacterType> RightHandRingFingerFlexionChanged;
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
		//public event Action<CharacterType> RightHandPinkyFingerFlexionChanged;
		//ReferenceField<double> _rightHandPinkyFingerFlexion = 0.0;

		//!!!!
		//[Category( "Skeleton State" )]
		//[DefaultValue( null )]
		//public Reference<ObjectInSpace> EyesLookAt
		//{
		//	get { if( _eyesLookAt.BeginGet() ) EyesLookAt = _eyesLookAt.Get( this ); return _eyesLookAt.value; }
		//	set { if( _eyesLookAt.BeginSet( ref value ) ) { try { EyesLookAtChanged?.Invoke( this ); } finally { _eyesLookAt.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="EyesLookAt"/> property value changes.</summary>
		//public event Action<CharacterType> EyesLookAtChanged;
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


		///////////////////////////////////////////////

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
					if( !Run )
						skip = true;
					break;

				case nameof( FlyControlMaxSpeed ):
				case nameof( FlyControlForce ):
					if( !FlyControl )
						skip = true;
					break;

				case nameof( JumpSpeed ):
				case nameof( JumpSound ):
					if( !Jump )
						skip = true;
					break;

				case nameof( CrouchingHeight ):
				case nameof( CrouchingWalkUpHeight ):
				//case nameof( CrouchingPositionToGroundHeight ):
				case nameof( CrouchingMaxSpeed ):
				case nameof( CrouchingForce ):
					if( !Crouching )
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

		[Browsable( false )]
		public int Version
		{
			get { return version; }
		}

		public void DataWasChanged()
		{
			unchecked
			{
				version++;
			}
		}

		///////////////////////////////////////////////

		public void GetBodyFormInfo( bool crouching, out double outHeight, out double outWalkUpHeight )//, out double outPositionToGroundHeight )
		{
			if( crouching )
			{
				outHeight = CrouchingHeight;
				outWalkUpHeight = CrouchingWalkUpHeight;
				//outPositionToGroundHeight = CrouchingPositionToGroundHeight;
			}
			else
			{
				outHeight = Height;
				outWalkUpHeight = WalkUpHeight;
				//outPositionToGroundHeight = PositionToGroundHeight;
			}
		}
	}
}
