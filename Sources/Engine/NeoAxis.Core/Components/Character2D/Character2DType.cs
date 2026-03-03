// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the character 2D type.
	/// </summary>
	[ResourceFileExtension( "character2dtype" )]
	[NewObjectDefaultName( "Character 2D Type" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Base\2D\Character 2D Type", -7900 )]
	[EditorControl( typeof( Character2DTypeEditor ), true )]
	[Preview( typeof( Character2DTypePreview ) )]
	[PreviewImage( typeof( Character2DTypePreviewImage ) )]
#endif
	public class Character2DType : Component
	{
		int version;

		/////////////////////////////////////////
		//Basic

		//!!!!Mesh if no need to animate

		/// <summary>
		/// The scale of the character at creation.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 1.0 )]
		public Reference<double> InitialScale
		{
			get { if( _initialScale.BeginGet() ) InitialScale = _initialScale.Get( this ); return _initialScale.value; }
			set { if( _initialScale.BeginSet( this, ref value ) ) { try { InitialScaleChanged?.Invoke( this ); } finally { _initialScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InitialScale"/> property value changes.</summary>
		public event Action<Character2DType> InitialScaleChanged;
		ReferenceField<double> _initialScale = 1.0;

		/// <summary>
		/// The height of the character.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 1.8 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set { if( _height.BeginSet( this, ref value ) ) { try { HeightChanged?.Invoke( this ); DataWasChanged(); } finally { _height.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<Character2DType> HeightChanged;
		ReferenceField<double> _height = 1.8;

		/// <summary>
		/// The radius of the collision capsule.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 0.3 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set { if( _radius.BeginSet( this, ref value ) ) { try { RadiusChanged?.Invoke( this ); DataWasChanged(); } finally { _radius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<Character2DType> RadiusChanged;
		ReferenceField<double> _radius = 0.3;

		/// <summary>
		/// The height to which the character can rise. Set 0 to disable functionality of walking up.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 0.6 )]
		public Reference<double> WalkUpHeight
		{
			get { if( _walkUpHeight.BeginGet() ) WalkUpHeight = _walkUpHeight.Get( this ); return _walkUpHeight.value; }
			set { if( _walkUpHeight.BeginSet( this, ref value ) ) { try { WalkUpHeightChanged?.Invoke( this ); } finally { _walkUpHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkUpHeight"/> property value changes.</summary>
		public event Action<Character2DType> WalkUpHeightChanged;
		ReferenceField<double> _walkUpHeight = 0.6;

		/// <summary>
		/// The distance from the character position to the ground.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 1.1 )]
		public Reference<double> PositionToGroundHeight
		{
			get { if( _positionToGroundHeight.BeginGet() ) PositionToGroundHeight = _positionToGroundHeight.Get( this ); return _positionToGroundHeight.value; }
			set { if( _positionToGroundHeight.BeginSet( this, ref value ) ) { try { PositionToGroundHeightChanged?.Invoke( this ); } finally { _positionToGroundHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionToGroundHeight"/> property value changes.</summary>
		public event Action<Character2DType> PositionToGroundHeightChanged;
		ReferenceField<double> _positionToGroundHeight = 1.1;

		/// <summary>
		/// The mass of the character.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 70 )]
		public Reference<double> Mass
		{
			get { if( _mass.BeginGet() ) Mass = _mass.Get( this ); return _mass.value; }
			set { if( _mass.BeginSet( this, ref value ) ) { try { MassChanged?.Invoke( this ); DataWasChanged(); } finally { _mass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mass"/> property value changes.</summary>
		public event Action<Character2DType> MassChanged;
		ReferenceField<double> _mass = 70;

		/// <summary>
		/// The maximum angle of the surface on which the character can stand.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 50 )]
		public Reference<Degree> MaxSlopeAngle
		{
			get { if( _maxSlopeAngle.BeginGet() ) MaxSlopeAngle = _maxSlopeAngle.Get( this ); return _maxSlopeAngle.value; }
			set { if( _maxSlopeAngle.BeginSet( this, ref value ) ) { try { MaxSlopeAngleChanged?.Invoke( this ); } finally { _maxSlopeAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxSlopeAngle"/> property value changes.</summary>
		public event Action<Character2DType> MaxSlopeAngleChanged;
		ReferenceField<Degree> _maxSlopeAngle = new Degree( 50 );

		/// <summary>
		/// The position of the eyes relative to the position of the character.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( "0.14 0.6" )]
		public Reference<Vector2> EyePosition
		{
			get { if( _eyePosition.BeginGet() ) EyePosition = _eyePosition.Get( this ); return _eyePosition.value; }
			set { if( _eyePosition.BeginSet( this, ref value ) ) { try { EyePositionChanged?.Invoke( this ); } finally { _eyePosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EyePosition"/> property value changes.</summary>
		public event Action<Character2DType> EyePositionChanged;
		ReferenceField<Vector2> _eyePosition = new Vector2( 0.14, 0.6 );

		/// <summary>
		/// Whether to consider the speed of the body on which the character is standing.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( false )]
		public Reference<bool> ApplyGroundVelocity
		{
			get { if( _applyGroundVelocity.BeginGet() ) ApplyGroundVelocity = _applyGroundVelocity.Get( this ); return _applyGroundVelocity.value; }
			set { if( _applyGroundVelocity.BeginSet( this, ref value ) ) { try { ApplyGroundVelocityChanged?.Invoke( this ); } finally { _applyGroundVelocity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ApplyGroundVelocity"/> property value changes.</summary>
		public event Action<Character2DType> ApplyGroundVelocityChanged;
		ReferenceField<bool> _applyGroundVelocity = false;

		[Category( "Advanced" )]
		[DefaultValue( 0.5 )]
		public Reference<double> MinSpeedToSleepBody
		{
			get { if( _minSpeedToSleepBody.BeginGet() ) MinSpeedToSleepBody = _minSpeedToSleepBody.Get( this ); return _minSpeedToSleepBody.value; }
			set { if( _minSpeedToSleepBody.BeginSet( this, ref value ) ) { try { MinSpeedToSleepBodyChanged?.Invoke( this ); } finally { _minSpeedToSleepBody.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MinSpeedToSleepBody"/> property value changes.</summary>
		public event Action<Character2DType> MinSpeedToSleepBodyChanged;
		ReferenceField<double> _minSpeedToSleepBody = 0.5;

		/// <summary>
		/// The value of linear dumping when a character is standing on the ground.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( 5 )]
		public Reference<double> LinearDampingOnGroundIdle
		{
			get { if( _linearDampingOnGroundIdle.BeginGet() ) LinearDampingOnGroundIdle = _linearDampingOnGroundIdle.Get( this ); return _linearDampingOnGroundIdle.value; }
			set { if( _linearDampingOnGroundIdle.BeginSet( this, ref value ) ) { try { LinearDampingOnGroundIdleChanged?.Invoke( this ); } finally { _linearDampingOnGroundIdle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearDampingOnGroundIdle"/> property value changes.</summary>
		public event Action<Character2DType> LinearDampingOnGroundIdleChanged;
		ReferenceField<double> _linearDampingOnGroundIdle = 5;

		/// <summary>
		/// The value of linear dumping when a character is standing on the ground.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( 3.0 )]
		public Reference<double> LinearDampingOnGroundMove
		{
			get { if( _linearDampingOnGround.BeginGet() ) LinearDampingOnGroundMove = _linearDampingOnGround.Get( this ); return _linearDampingOnGround.value; }
			set { if( _linearDampingOnGround.BeginSet( this, ref value ) ) { try { LinearDampingOnGroundChanged?.Invoke( this ); } finally { _linearDampingOnGround.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearDampingOnGroundMove"/> property value changes.</summary>
		public event Action<Character2DType> LinearDampingOnGroundChanged;
		ReferenceField<double> _linearDampingOnGround = 3.0;

		/// <summary>
		/// The value of linear dumping when a character is flying.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( 0.15 )]
		public Reference<double> LinearDampingFly
		{
			get { if( _linearDampingFly.BeginGet() ) LinearDampingFly = _linearDampingFly.Get( this ); return _linearDampingFly.value; }
			set { if( _linearDampingFly.BeginSet( this, ref value ) ) { try { LinearDampingFlyChanged?.Invoke( this ); } finally { _linearDampingFly.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearDampingFly"/> property value changes.</summary>
		public event Action<Character2DType> LinearDampingFlyChanged;
		ReferenceField<double> _linearDampingFly = 0.15;

		/////////////////////////////////////////
		//Walk

		/// <summary>
		/// Maximum speed when walking forward.
		/// </summary>
		[Category( "Walk" )]
		[DefaultValue( 2.0 )]
		public Reference<double> WalkForwardMaxSpeed
		{
			get { if( _walkForwardMaxSpeed.BeginGet() ) WalkForwardMaxSpeed = _walkForwardMaxSpeed.Get( this ); return _walkForwardMaxSpeed.value; }
			set { if( _walkForwardMaxSpeed.BeginSet( this, ref value ) ) { try { WalkForwardMaxSpeedChanged?.Invoke( this ); } finally { _walkForwardMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkForwardMaxSpeed"/> property value changes.</summary>
		public event Action<Character2DType> WalkForwardMaxSpeedChanged;
		ReferenceField<double> _walkForwardMaxSpeed = 2.0;

		///// <summary>
		///// Maximum speed when walking backward.
		///// </summary>
		//[Category( "Walk" )]
		//[DefaultValue( 1.5 )]
		//public Reference<double> WalkBackwardMaxSpeed
		//{
		//	get { if( _walkBackwardMaxSpeed.BeginGet() ) WalkBackwardMaxSpeed = _walkBackwardMaxSpeed.Get( this ); return _walkBackwardMaxSpeed.value; }
		//	set { if( _walkBackwardMaxSpeed.BeginSet( this, ref value ) ) { try { WalkBackwardMaxSpeedChanged?.Invoke( this ); } finally { _walkBackwardMaxSpeed.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="WalkBackwardMaxSpeed"/> property value changes.</summary>
		//public event Action<Character2DType> WalkBackwardMaxSpeedChanged;
		//ReferenceField<double> _walkBackwardMaxSpeed = 1.5;

		/// <summary>
		/// Physical force applied to the body for walking.
		/// </summary>
		[Category( "Walk" )]
		[DefaultValue( 30000 )]
		public Reference<double> WalkForce
		{
			get { if( _walkForce.BeginGet() ) WalkForce = _walkForce.Get( this ); return _walkForce.value; }
			set { if( _walkForce.BeginSet( this, ref value ) ) { try { WalkForceChanged?.Invoke( this ); } finally { _walkForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkForce"/> property value changes.</summary>
		public event Action<Character2DType> WalkForceChanged;
		ReferenceField<double> _walkForce = 30000;

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
			set { if( _runSupport.BeginSet( this, ref value ) ) { try { RunSupportChanged?.Invoke( this ); } finally { _runSupport.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunSupport"/> property value changes.</summary>
		public event Action<Character2DType> RunSupportChanged;
		ReferenceField<bool> _runSupport = false;

		/// <summary>
		/// Maximum speed when running forward.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( 5 )]
		public Reference<double> RunForwardMaxSpeed
		{
			get { if( _runForwardMaxSpeed.BeginGet() ) RunForwardMaxSpeed = _runForwardMaxSpeed.Get( this ); return _runForwardMaxSpeed.value; }
			set { if( _runForwardMaxSpeed.BeginSet( this, ref value ) ) { try { RunForwardMaxSpeedChanged?.Invoke( this ); } finally { _runForwardMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunForwardMaxSpeed"/> property value changes.</summary>
		public event Action<Character2DType> RunForwardMaxSpeedChanged;
		ReferenceField<double> _runForwardMaxSpeed = 5;

		///// <summary>
		///// Maximum speed when running backward.
		///// </summary>
		//[Category( "Run" )]
		//[DefaultValue( 5 )]
		//public Reference<double> RunBackwardMaxSpeed
		//{
		//	get { if( _runBackwardMaxSpeed.BeginGet() ) RunBackwardMaxSpeed = _runBackwardMaxSpeed.Get( this ); return _runBackwardMaxSpeed.value; }
		//	set { if( _runBackwardMaxSpeed.BeginSet( this, ref value ) ) { try { RunBackwardMaxSpeedChanged?.Invoke( this ); } finally { _runBackwardMaxSpeed.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RunBackwardMaxSpeed"/> property value changes.</summary>
		//public event Action<Character2D> RunBackwardMaxSpeedChanged;
		//ReferenceField<double> _runBackwardMaxSpeed = 5;

		/// <summary>
		/// Physical force applied to the body for running.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( 50000 )]
		public Reference<double> RunForce
		{
			get { if( _runForce.BeginGet() ) RunForce = _runForce.Get( this ); return _runForce.value; }
			set { if( _runForce.BeginSet( this, ref value ) ) { try { RunForceChanged?.Invoke( this ); } finally { _runForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunForce"/> property value changes.</summary>
		public event Action<Character2DType> RunForceChanged;
		ReferenceField<double> _runForce = 50000;

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
			set { if( _flyControlSupport.BeginSet( this, ref value ) ) { try { FlyControlSupportChanged?.Invoke( this ); } finally { _flyControlSupport.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyControlSupport"/> property value changes.</summary>
		public event Action<Character2DType> FlyControlSupportChanged;
		ReferenceField<bool> _flyControlSupport = false;

		/// <summary>
		/// Maximum speed of character control in flight.
		/// </summary>
		[Category( "Fly Control" )]
		[DefaultValue( 10 )]
		public Reference<double> FlyControlMaxSpeed
		{
			get { if( _flyControlMaxSpeed.BeginGet() ) FlyControlMaxSpeed = _flyControlMaxSpeed.Get( this ); return _flyControlMaxSpeed.value; }
			set { if( _flyControlMaxSpeed.BeginSet( this, ref value ) ) { try { FlyControlMaxSpeedChanged?.Invoke( this ); } finally { _flyControlMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyControlMaxSpeed"/> property value changes.</summary>
		public event Action<Character2DType> FlyControlMaxSpeedChanged;
		ReferenceField<double> _flyControlMaxSpeed = 10;

		/// <summary>
		/// Physical force applied to the body for flying.
		/// </summary>
		[Category( "Fly Control" )]
		[DefaultValue( 3000 )]
		public Reference<double> FlyControlForce
		{
			get { if( _flyControlForce.BeginGet() ) FlyControlForce = _flyControlForce.Get( this ); return _flyControlForce.value; }
			set { if( _flyControlForce.BeginSet( this, ref value ) ) { try { FlyControlForceChanged?.Invoke( this ); } finally { _flyControlForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyControlForce"/> property value changes.</summary>
		public event Action<Character2DType> FlyControlForceChanged;
		ReferenceField<double> _flyControlForce = 3000;

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
			set { if( _jumpSupport.BeginSet( this, ref value ) ) { try { JumpSupportChanged?.Invoke( this ); } finally { _jumpSupport.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpSupport"/> property value changes.</summary>
		public event Action<Character2DType> JumpSupportChanged;
		ReferenceField<bool> _jumpSupport = false;

		/// <summary>
		/// The vertical speed of a jump.
		/// </summary>
		[Category( "Jump" )]
		[DefaultValue( 8 )]
		public Reference<double> JumpSpeed
		{
			get { if( _jumpSpeed.BeginGet() ) JumpSpeed = _jumpSpeed.Get( this ); return _jumpSpeed.value; }
			set { if( _jumpSpeed.BeginSet( this, ref value ) ) { try { JumpSpeedChanged?.Invoke( this ); } finally { _jumpSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpSpeed"/> property value changes.</summary>
		public event Action<Character2DType> JumpSpeedChanged;
		ReferenceField<double> _jumpSpeed = 8;

		[Category( "Jump" )]
		[DefaultValue( null )]
		public Reference<Sound> JumpSound
		{
			get { if( _jumpSound.BeginGet() ) JumpSound = _jumpSound.Get( this ); return _jumpSound.value; }
			set { if( _jumpSound.BeginSet( this, ref value ) ) { try { JumpSoundChanged?.Invoke( this ); } finally { _jumpSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpSound"/> property value changes.</summary>
		public event Action<Character2DType> JumpSoundChanged;
		ReferenceField<Sound> _jumpSound = null;

		/////////////////////////////////////////
		//Crouching

		//!!!!is disabled
		[Browsable( false )]
		[Category( "Crouching" )]
		[DefaultValue( false )]
		public Reference<bool> CrouchingSupport
		{
			get { if( _crouchingSupport.BeginGet() ) CrouchingSupport = _crouchingSupport.Get( this ); return _crouchingSupport.value; }
			set { if( _crouchingSupport.BeginSet( this, ref value ) ) { try { CrouchingSupportChanged?.Invoke( this ); } finally { _crouchingSupport.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingSupport"/> property value changes.</summary>
		public event Action<Character2DType> CrouchingSupportChanged;
		ReferenceField<bool> _crouchingSupport = false;

		[Category( "Crouching" )]
		[DefaultValue( 1.0 )]
		public Reference<double> CrouchingHeight
		{
			get { if( _crouchingHeight.BeginGet() ) CrouchingHeight = _crouchingHeight.Get( this ); return _crouchingHeight.value; }
			set { if( _crouchingHeight.BeginSet( this, ref value ) ) { try { CrouchingHeightChanged?.Invoke( this ); } finally { _crouchingHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingHeight"/> property value changes.</summary>
		public event Action<Character2DType> CrouchingHeightChanged;
		ReferenceField<double> _crouchingHeight = 1.0;

		[Category( "Crouching" )]
		[DefaultValue( 0.1 )]
		public Reference<double> CrouchingWalkUpHeight
		{
			get { if( _crouchingWalkUpHeight.BeginGet() ) CrouchingWalkUpHeight = _crouchingWalkUpHeight.Get( this ); return _crouchingWalkUpHeight.value; }
			set { if( _crouchingWalkUpHeight.BeginSet( this, ref value ) ) { try { CrouchingWalkUpHeightChanged?.Invoke( this ); } finally { _crouchingWalkUpHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingWalkUpHeight"/> property value changes.</summary>
		public event Action<Character2DType> CrouchingWalkUpHeightChanged;
		ReferenceField<double> _crouchingWalkUpHeight = 0.1;

		[Category( "Crouching" )]
		[DefaultValue( 0.55 )]
		public Reference<double> CrouchingPositionToGroundHeight
		{
			get { if( _crouchingPositionToGroundHeight.BeginGet() ) CrouchingPositionToGroundHeight = _crouchingPositionToGroundHeight.Get( this ); return _crouchingPositionToGroundHeight.value; }
			set { if( _crouchingPositionToGroundHeight.BeginSet( this, ref value ) ) { try { CrouchingPositionToGroundHeightChanged?.Invoke( this ); } finally { _crouchingPositionToGroundHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingPositionToGroundHeight"/> property value changes.</summary>
		public event Action<Character2DType> CrouchingPositionToGroundHeightChanged;
		ReferenceField<double> _crouchingPositionToGroundHeight = 0.55;

		[Category( "Crouching" )]
		[DefaultValue( 1.0 )]
		public Reference<double> CrouchingMaxSpeed
		{
			get { if( _crouchingMaxSpeed.BeginGet() ) CrouchingMaxSpeed = _crouchingMaxSpeed.Get( this ); return _crouchingMaxSpeed.value; }
			set { if( _crouchingMaxSpeed.BeginSet( this, ref value ) ) { try { CrouchingMaxSpeedChanged?.Invoke( this ); } finally { _crouchingMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingMaxSpeed"/> property value changes.</summary>
		public event Action<Character2DType> CrouchingMaxSpeedChanged;
		ReferenceField<double> _crouchingMaxSpeed = 1.0;

		//!!!!
		[Category( "Crouching" )]
		[DefaultValue( 10000 )]
		public Reference<double> CrouchingForce
		{
			get { if( _crouchingForce.BeginGet() ) CrouchingForce = _crouchingForce.Get( this ); return _crouchingForce.value; }
			set { if( _crouchingForce.BeginSet( this, ref value ) ) { try { CrouchingForceChanged?.Invoke( this ); } finally { _crouchingForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingForce"/> property value changes.</summary>
		public event Action<Character2DType> CrouchingForceChanged;
		ReferenceField<double> _crouchingForce = 10000;

		/////////////////////////////////////////
		//Animate

		///// <summary>
		///// Whether to enable default animation method of the character.
		///// </summary>
		//[Category( "Animation" )]
		//[DefaultValue( false )]
		//public Reference<bool> Animate
		//{
		//	get { if( _animate.BeginGet() ) Animate = _animate.Get( this ); return _animate.value; }
		//	set { if( _animate.BeginSet( this, ref value ) ) { try { AnimateChanged?.Invoke( this ); } finally { _animate.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Animate"/> property value changes.</summary>
		//public event Action<Character2DType> AnimateChanged;
		//ReferenceField<bool> _animate = false;

		/// <summary>
		/// Animation of character at rest.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Animation> IdleAnimation
		{
			get { if( _idleAnimation.BeginGet() ) IdleAnimation = _idleAnimation.Get( this ); return _idleAnimation.value; }
			set { if( _idleAnimation.BeginSet( this, ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IdleAnimation"/> property value changes.</summary>
		public event Action<Character2DType> IdleAnimationChanged;
		ReferenceField<Animation> _idleAnimation = null;

		/// <summary>
		/// Character walking animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Animation> WalkAnimation
		{
			get { if( _walkAnimation.BeginGet() ) WalkAnimation = _walkAnimation.Get( this ); return _walkAnimation.value; }
			set { if( _walkAnimation.BeginSet( this, ref value ) ) { try { WalkAnimationChanged?.Invoke( this ); } finally { _walkAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkAnimation"/> property value changes.</summary>
		public event Action<Character2DType> WalkAnimationChanged;
		ReferenceField<Animation> _walkAnimation = null;

		/// <summary>
		/// The multiplier for playing the walking animation depending on the speed of the character.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> WalkAnimationSpeed
		{
			get { if( _walkAnimationSpeed.BeginGet() ) WalkAnimationSpeed = _walkAnimationSpeed.Get( this ); return _walkAnimationSpeed.value; }
			set { if( _walkAnimationSpeed.BeginSet( this, ref value ) ) { try { WalkAnimationSpeedChanged?.Invoke( this ); } finally { _walkAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkAnimationSpeed"/> property value changes.</summary>
		public event Action<Character2DType> WalkAnimationSpeedChanged;
		ReferenceField<double> _walkAnimationSpeed = 1.0;

		/// <summary>
		/// Character running animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Animation> RunAnimation
		{
			get { if( _runAnimation.BeginGet() ) RunAnimation = _runAnimation.Get( this ); return _runAnimation.value; }
			set { if( _runAnimation.BeginSet( this, ref value ) ) { try { RunAnimationChanged?.Invoke( this ); } finally { _runAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunAnimation"/> property value changes.</summary>
		public event Action<Character2DType> RunAnimationChanged;
		ReferenceField<Animation> _runAnimation = null;

		/// <summary>
		/// The multiplier for playing the running animation depending on the speed of the character.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> RunAnimationSpeed
		{
			get { if( _runAnimationSpeed.BeginGet() ) RunAnimationSpeed = _runAnimationSpeed.Get( this ); return _runAnimationSpeed.value; }
			set { if( _runAnimationSpeed.BeginSet( this, ref value ) ) { try { RunAnimationSpeedChanged?.Invoke( this ); } finally { _runAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunAnimationSpeed"/> property value changes.</summary>
		public event Action<Character2DType> RunAnimationSpeedChanged;
		ReferenceField<double> _runAnimationSpeed = 1.0;

		/// <summary>
		/// Character flying animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Animation> FlyAnimation
		{
			get { if( _flyAnimation.BeginGet() ) FlyAnimation = _flyAnimation.Get( this ); return _flyAnimation.value; }
			set { if( _flyAnimation.BeginSet( this, ref value ) ) { try { FlyAnimationChanged?.Invoke( this ); } finally { _flyAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyAnimation"/> property value changes.</summary>
		public event Action<Character2DType> FlyAnimationChanged;
		ReferenceField<Animation> _flyAnimation = null;

		/// <summary>
		/// Character jumping animation.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Animation> JumpAnimation
		{
			get { if( _jumpAnimation.BeginGet() ) JumpAnimation = _jumpAnimation.Get( this ); return _jumpAnimation.value; }
			set { if( _jumpAnimation.BeginSet( this, ref value ) ) { try { JumpAnimationChanged?.Invoke( this ); } finally { _jumpAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpAnimation"/> property value changes.</summary>
		public event Action<Character2DType> JumpAnimationChanged;
		ReferenceField<Animation> _jumpAnimation = null;

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
				//case nameof( RunBackwardMaxSpeed ):
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

				//case nameof( IdleAnimation ):
				//case nameof( WalkAnimation ):
				//case nameof( WalkAnimationSpeed ):
				//case nameof( RunAnimation ):
				//case nameof( RunAnimationSpeed ):
				//case nameof( FlyAnimation ):
				//	if( !Animate )
				//		skip = true;
				//	break;
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

		public void GetBodyFormInfo( bool crouching, out double outHeight, out double outWalkUpHeight, out double outPositionToGroundHeight )
		{
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
	}
}
