// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

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

		//bool meshGeneratorNeedUpdate;

		//

		//!!!!
		//динамически получаемым по идее не нужно
		//DataWasChanged()


		/////////////////////////////////////////
		//Basic

		const string meshDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Bryce.fbx|$Mesh";
		//const string meshDefault = @"Content\Characters\Default\Human.fbx|$Mesh";

		/// <summary>
		/// The mesh of the character.
		/// </summary>
		[DefaultValueReference( meshDefault )]
		[Category( "Basic" )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
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
			set { if( _height.BeginSet( this, ref value ) ) { try { HeightChanged?.Invoke( this ); DataWasChanged(); /*MeshGeneratorNeedUpdate();*/ } finally { _height.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<CharacterType> HeightChanged;
		ReferenceField<double> _height = 1.8;

		/// <summary>
		/// The radius of the collision capsule.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 0.4 )]//0.35 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set { if( _radius.BeginSet( this, ref value ) ) { try { RadiusChanged?.Invoke( this ); DataWasChanged(); } finally { _radius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<CharacterType> RadiusChanged;
		ReferenceField<double> _radius = 0.4;//0.35;

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
			set { if( _walkUpHeight.BeginSet( this, ref value ) ) { try { WalkUpHeightChanged?.Invoke( this ); } finally { _walkUpHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkUpHeight"/> property value changes.</summary>
		public event Action<CharacterType> WalkUpHeightChanged;
		ReferenceField<double> _walkUpHeight = 0.6;

		/// <summary>
		/// The height to which the character can go down without start flying. Set 0 to disable functionality of walking down.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 0.6 )]
		public Reference<double> WalkDownHeight
		{
			get { if( _walkDownHeight.BeginGet() ) WalkDownHeight = _walkDownHeight.Get( this ); return _walkDownHeight.value; }
			set { if( _walkDownHeight.BeginSet( this, ref value ) ) { try { WalkDownHeightChanged?.Invoke( this ); } finally { _walkDownHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkDownHeight"/> property value changes.</summary>
		public event Action<CharacterType> WalkDownHeightChanged;
		ReferenceField<double> _walkDownHeight = 0.6;

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
		public event Action<CharacterType> MaxSlopeAngleChanged;
		ReferenceField<Degree> _maxSlopeAngle = new Degree( 50 );

		/// <summary>
		/// The position of the eyes relative to the position of the character.
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( "0.23 0 1.67" )]//"0.23 0 1.73" )]
		public Reference<Vector3> EyePosition
		{
			get { if( _eyePosition.BeginGet() ) EyePosition = _eyePosition.Get( this ); return _eyePosition.value; }
			set { if( _eyePosition.BeginSet( this, ref value ) ) { try { EyePositionChanged?.Invoke( this ); } finally { _eyePosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EyePosition"/> property value changes.</summary>
		public event Action<CharacterType> EyePositionChanged;
		ReferenceField<Vector3> _eyePosition = new Vector3( 0.23, 0, 1.67 );// new Vector3( 0.23, 0, 1.73 );

		///// <summary>
		///// Whether to enable default animation method of the character.
		///// </summary>
		//[Category( "Basic" )]
		//[DefaultValue( true )]
		//public Reference<bool> Animate
		//{
		//	get { if( _animate.BeginGet() ) Animate = _animate.Get( this ); return _animate.value; }
		//	set { if( _animate.BeginSet( this, ref value ) ) { try { AnimateChanged?.Invoke( this ); DataWasChanged(); } finally { _animate.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Animate"/> property value changes.</summary>
		//public event Action<CharacterType> AnimateChanged;
		//ReferenceField<bool> _animate = true;

		[Category( "Basic" )]
		[DefaultValue( true )]
		public Reference<bool> AllowManageInventory
		{
			get { if( _allowManageInventory.BeginGet() ) AllowManageInventory = _allowManageInventory.Get( this ); return _allowManageInventory.value; }
			set { if( _allowManageInventory.BeginSet( this, ref value ) ) { try { AllowManageInventoryChanged?.Invoke( this ); } finally { _allowManageInventory.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowManageInventory"/> property value changes.</summary>
		public event Action<CharacterType> AllowManageInventoryChanged;
		ReferenceField<bool> _allowManageInventory = true;

		[Category( "Basic" )]
		[DefaultValue( false )]
		public Reference<bool> RotateMeshDependingGround
		{
			get { if( _rotateMeshDependingGround.BeginGet() ) RotateMeshDependingGround = _rotateMeshDependingGround.Get( this ); return _rotateMeshDependingGround.value; }
			set { if( _rotateMeshDependingGround.BeginSet( this, ref value ) ) { try { RotateMeshDependingGroundChanged?.Invoke( this ); } finally { _rotateMeshDependingGround.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotateMeshDependingGround"/> property value changes.</summary>
		public event Action<CharacterType> RotateMeshDependingGroundChanged;
		ReferenceField<bool> _rotateMeshDependingGround = false;

		///// <summary>
		///// Whether to consider the speed of the body on which the character is standing.
		///// </summary>
		//[Category( "Physics" )]
		//[DefaultValue( false )]
		//public Reference<bool> ApplyGroundVelocity
		//{
		//	get { if( _applyGroundVelocity.BeginGet() ) ApplyGroundVelocity = _applyGroundVelocity.Get( this ); return _applyGroundVelocity.value; }
		//	set { if( _applyGroundVelocity.BeginSet( this, ref value ) ) { try { ApplyGroundVelocityChanged?.Invoke( this ); } finally { _applyGroundVelocity.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ApplyGroundVelocity"/> property value changes.</summary>
		//public event Action<CharacterType> ApplyGroundVelocityChanged;
		//ReferenceField<bool> _applyGroundVelocity = false;

		//[Category( "Physics" )]
		//[DefaultValue( 1.0 )]
		//public Reference<double> MinSpeedToSleepBody
		//{
		//	get { if( _minSpeedToSleepBody.BeginGet() ) MinSpeedToSleepBody = _minSpeedToSleepBody.Get( this ); return _minSpeedToSleepBody.value; }
		//	set { if( _minSpeedToSleepBody.BeginSet( this, ref value ) ) { try { MinSpeedToSleepBodyChanged?.Invoke( this ); } finally { _minSpeedToSleepBody.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MinSpeedToSleepBody"/> property value changes.</summary>
		//public event Action<CharacterType> MinSpeedToSleepBodyChanged;
		//ReferenceField<double> _minSpeedToSleepBody = 1.0;

		/// <summary>
		/// Maximum force with which the character can push other bodies (N).
		/// </summary>
		[Category( "Basic" )]
		[DefaultValue( 30.0 )]
		public Reference<double> MaxStrength
		{
			get { if( _maxStrength.BeginGet() ) MaxStrength = _maxStrength.Get( this ); return _maxStrength.value; }
			set { if( _maxStrength.BeginSet( this, ref value ) ) { try { MaxStrengthChanged?.Invoke( this ); } finally { _maxStrength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxStrength"/> property value changes.</summary>
		public event Action<CharacterType> MaxStrengthChanged;
		ReferenceField<double> _maxStrength = 30.0;

		////!!!!default
		///// <summary>
		///// The value of linear dumping when a character is standing on the ground.
		///// </summary>
		//[Category( "Physics" )]
		//[DefaultValue( 5 )]
		//public Reference<double> LinearDampingOnGroundIdle
		//{
		//	get { if( _linearDampingOnGroundIdle.BeginGet() ) LinearDampingOnGroundIdle = _linearDampingOnGroundIdle.Get( this ); return _linearDampingOnGroundIdle.value; }
		//	set { if( _linearDampingOnGroundIdle.BeginSet( this, ref value ) ) { try { LinearDampingOnGroundIdleChanged?.Invoke( this ); } finally { _linearDampingOnGroundIdle.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LinearDampingOnGroundIdle"/> property value changes.</summary>
		//public event Action<CharacterType> LinearDampingOnGroundIdleChanged;
		//ReferenceField<double> _linearDampingOnGroundIdle = 5;

		////!!!!default
		///// <summary>
		///// The value of linear dumping when a character is standing on the ground.
		///// </summary>
		//[Category( "Physics" )]
		//[DefaultValue( 3 )]//0.99 )]
		//public Reference<double> LinearDampingOnGroundMove
		//{
		//	get { if( _linearDampingOnGround.BeginGet() ) LinearDampingOnGroundMove = _linearDampingOnGround.Get( this ); return _linearDampingOnGround.value; }
		//	set { if( _linearDampingOnGround.BeginSet( this, ref value ) ) { try { LinearDampingOnGroundChanged?.Invoke( this ); } finally { _linearDampingOnGround.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LinearDampingOnGroundMove"/> property value changes.</summary>
		//public event Action<CharacterType> LinearDampingOnGroundChanged;
		//ReferenceField<double> _linearDampingOnGround = 3;//0.99;

		////!!!!
		///// <summary>
		///// The value of linear dumping when a character is flying.
		///// </summary>
		//[Category( "Physics" )]
		//[DefaultValue( 0.15 )]
		//public Reference<double> LinearDampingFly
		//{
		//	get { if( _linearDampingFly.BeginGet() ) LinearDampingFly = _linearDampingFly.Get( this ); return _linearDampingFly.value; }
		//	set { if( _linearDampingFly.BeginSet( this, ref value ) ) { try { LinearDampingFlyChanged?.Invoke( this ); } finally { _linearDampingFly.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LinearDampingFly"/> property value changes.</summary>
		//public event Action<CharacterType> LinearDampingFlyChanged;
		//ReferenceField<double> _linearDampingFly = 0.15;

		/////////////////////////////////////////
		//Idle

		const string idleAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Idle.animation";
		//const string idleAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Idle";

		/// <summary>
		/// Animation of character at rest.
		/// </summary>
		[Category( "Idle" )]
		[DefaultValueReference( idleAnimationDefault )]
		public Reference<Animation> IdleAnimation
		{
			get { if( _idleAnimation.BeginGet() ) IdleAnimation = _idleAnimation.Get( this ); return _idleAnimation.value; }
			set { if( _idleAnimation.BeginSet( this, ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IdleAnimation"/> property value changes.</summary>
		public event Action<CharacterType> IdleAnimationChanged;
		ReferenceField<Animation> _idleAnimation = new Reference<Animation>( null, idleAnimationDefault );

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
			set { if( _walkForwardMaxSpeed.BeginSet( this, ref value ) ) { try { WalkForwardMaxSpeedChanged?.Invoke( this ); } finally { _walkForwardMaxSpeed.EndSet(); } } }
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
			set { if( _walkBackwardMaxSpeed.BeginSet( this, ref value ) ) { try { WalkBackwardMaxSpeedChanged?.Invoke( this ); } finally { _walkBackwardMaxSpeed.EndSet(); } } }
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
			set { if( _walkSideMaxSpeed.BeginSet( this, ref value ) ) { try { WalkSideMaxSpeedChanged?.Invoke( this ); } finally { _walkSideMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkSideMaxSpeed"/> property value changes.</summary>
		public event Action<CharacterType> WalkSideMaxSpeedChanged;
		ReferenceField<double> _walkSideMaxSpeed = 1.5;

		///// <summary>
		///// Physical force applied to the body for walking.
		///// </summary>
		//[Category( "Walk" )]
		//[DefaultValue( 100000 )]
		//public Reference<double> WalkForce
		//{
		//	get { if( _walkForce.BeginGet() ) WalkForce = _walkForce.Get( this ); return _walkForce.value; }
		//	set { if( _walkForce.BeginSet( this, ref value ) ) { try { WalkForceChanged?.Invoke( this ); } finally { _walkForce.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="WalkForce"/> property value changes.</summary>
		//public event Action<CharacterType> WalkForceChanged;
		//ReferenceField<double> _walkForce = 100000;

		const string walkAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Walk.animation";
		//const string walkAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Walk";
		/// <summary>
		/// Character walking animation.
		/// </summary>
		[Category( "Walk" )]
		[DefaultValueReference( walkAnimationDefault )]
		public Reference<Animation> WalkAnimation
		{
			get { if( _walkAnimation.BeginGet() ) WalkAnimation = _walkAnimation.Get( this ); return _walkAnimation.value; }
			set { if( _walkAnimation.BeginSet( this, ref value ) ) { try { WalkAnimationChanged?.Invoke( this ); } finally { _walkAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkAnimation"/> property value changes.</summary>
		public event Action<CharacterType> WalkAnimationChanged;
		ReferenceField<Animation> _walkAnimation = new Reference<Animation>( null, walkAnimationDefault );

		/// <summary>
		/// The multiplier for playing the walking animation depending on the speed of the character.
		/// </summary>
		[Category( "Walk" )]
		[DefaultValue( 0.7 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> WalkAnimationSpeed
		{
			get { if( _walkAnimationSpeed.BeginGet() ) WalkAnimationSpeed = _walkAnimationSpeed.Get( this ); return _walkAnimationSpeed.value; }
			set { if( _walkAnimationSpeed.BeginSet( this, ref value ) ) { try { WalkAnimationSpeedChanged?.Invoke( this ); } finally { _walkAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WalkAnimationSpeed"/> property value changes.</summary>
		public event Action<CharacterType> WalkAnimationSpeedChanged;
		ReferenceField<double> _walkAnimationSpeed = 0.7;

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
			set { if( _run.BeginSet( this, ref value ) ) { try { RunChanged?.Invoke( this ); } finally { _run.EndSet(); } } }
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
			set { if( _runForwardMaxSpeed.BeginSet( this, ref value ) ) { try { RunForwardMaxSpeedChanged?.Invoke( this ); } finally { _runForwardMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunForwardMaxSpeed"/> property value changes.</summary>
		public event Action<CharacterType> RunForwardMaxSpeedChanged;
		ReferenceField<double> _runForwardMaxSpeed = 5;

		/// <summary>
		/// Maximum speed when running backward.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( 1.5 )]
		public Reference<double> RunBackwardMaxSpeed
		{
			get { if( _runBackwardMaxSpeed.BeginGet() ) RunBackwardMaxSpeed = _runBackwardMaxSpeed.Get( this ); return _runBackwardMaxSpeed.value; }
			set { if( _runBackwardMaxSpeed.BeginSet( this, ref value ) ) { try { RunBackwardMaxSpeedChanged?.Invoke( this ); } finally { _runBackwardMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunBackwardMaxSpeed"/> property value changes.</summary>
		public event Action<CharacterType> RunBackwardMaxSpeedChanged;
		ReferenceField<double> _runBackwardMaxSpeed = 1.5;

		/// <summary>
		/// Maximum speed when running to a side.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( 1.5 )]
		public Reference<double> RunSideMaxSpeed
		{
			get { if( _runSideMaxSpeed.BeginGet() ) RunSideMaxSpeed = _runSideMaxSpeed.Get( this ); return _runSideMaxSpeed.value; }
			set { if( _runSideMaxSpeed.BeginSet( this, ref value ) ) { try { RunSideMaxSpeedChanged?.Invoke( this ); } finally { _runSideMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunSideMaxSpeed"/> property value changes.</summary>
		public event Action<CharacterType> RunSideMaxSpeedChanged;
		ReferenceField<double> _runSideMaxSpeed = 1.5;

		///// <summary>
		///// Physical force applied to the body for running.
		///// </summary>
		//[Category( "Run" )]
		//[DefaultValue( 150000 )]
		//public Reference<double> RunForce
		//{
		//	get { if( _runForce.BeginGet() ) RunForce = _runForce.Get( this ); return _runForce.value; }
		//	set { if( _runForce.BeginSet( this, ref value ) ) { try { RunForceChanged?.Invoke( this ); } finally { _runForce.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RunForce"/> property value changes.</summary>
		//public event Action<CharacterType> RunForceChanged;
		//ReferenceField<double> _runForce = 150000;

		const string runAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Run.animation";
		//const string runAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Run";

		/// <summary>
		/// Character running animation.
		/// </summary>
		[Category( "Run" )]
		[DefaultValueReference( runAnimationDefault )]
		public Reference<Animation> RunAnimation
		{
			get { if( _runAnimation.BeginGet() ) RunAnimation = _runAnimation.Get( this ); return _runAnimation.value; }
			set { if( _runAnimation.BeginSet( this, ref value ) ) { try { RunAnimationChanged?.Invoke( this ); } finally { _runAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunAnimation"/> property value changes.</summary>
		public event Action<CharacterType> RunAnimationChanged;
		ReferenceField<Animation> _runAnimation = new Reference<Animation>( null, runAnimationDefault );

		/// <summary>
		/// The multiplier for playing the running animation depending on the speed of the character.
		/// </summary>
		[Category( "Run" )]
		[DefaultValue( 0.2 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> RunAnimationSpeed
		{
			get { if( _runAnimationSpeed.BeginGet() ) RunAnimationSpeed = _runAnimationSpeed.Get( this ); return _runAnimationSpeed.value; }
			set { if( _runAnimationSpeed.BeginSet( this, ref value ) ) { try { RunAnimationSpeedChanged?.Invoke( this ); } finally { _runAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RunAnimationSpeed"/> property value changes.</summary>
		public event Action<CharacterType> RunAnimationSpeedChanged;
		ReferenceField<double> _runAnimationSpeed = 0.2;

		const string flyAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Fly.animation";
		//const string flyAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Fly";

		/////////////////////////////////////////
		//Turning

		/// <summary>
		/// The speed of rotation of the character around its axis.
		/// </summary>
		[Category( "Turn" )]
		[DefaultValue( 180.0 )]// 90.0 )]
		public Reference<Degree> TurningSpeed
		{
			get { if( _turningSpeed.BeginGet() ) TurningSpeed = _turningSpeed.Get( this ); return _turningSpeed.value; }
			set { if( _turningSpeed.BeginSet( this, ref value ) ) { try { TurningSpeedChanged?.Invoke( this ); } finally { _turningSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TurningSpeed"/> property value changes.</summary>
		public event Action<CharacterType> TurningSpeedChanged;
		ReferenceField<Degree> _turningSpeed = new Degree( 180.0 );// 90.0 );

		/// <summary>
		/// The speed of rotation of the character looking to position.
		/// </summary>
		[Category( "Turn" )]
		[DefaultValue( 180.0 )]//360.0 )]
		public Reference<Degree> TurningSpeedOfLooking
		{
			get { if( _turningSpeedOfLooking.BeginGet() ) TurningSpeedOfLooking = _turningSpeedOfLooking.Get( this ); return _turningSpeedOfLooking.value; }
			set { if( _turningSpeedOfLooking.BeginSet( this, ref value ) ) { try { TurningSpeedOfLookingChanged?.Invoke( this ); } finally { _turningSpeedOfLooking.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TurningSpeedOfLooking"/> property value changes.</summary>
		public event Action<CharacterType> TurningSpeedOfLookingChanged;
		ReferenceField<Degree> _turningSpeedOfLooking = new Degree( 180.0 );//360.0 );

		const string leftTurnAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Left Turn.animation";
		//const string leftTurnAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Left Turn";

		/// <summary>
		/// Character left turn animation.
		/// </summary>
		[Category( "Turn" )]
		[DefaultValueReference( leftTurnAnimationDefault )]
		public Reference<Animation> LeftTurnAnimation
		{
			get { if( _leftTurnAnimation.BeginGet() ) LeftTurnAnimation = _leftTurnAnimation.Get( this ); return _leftTurnAnimation.value; }
			set { if( _leftTurnAnimation.BeginSet( this, ref value ) ) { try { LeftTurnAnimationChanged?.Invoke( this ); } finally { _leftTurnAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftTurnAnimation"/> property value changes.</summary>
		public event Action<CharacterType> LeftTurnAnimationChanged;
		ReferenceField<Animation> _leftTurnAnimation = new Reference<Animation>( null, leftTurnAnimationDefault );

		const string rightTurnAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Right Turn.animation";
		//const string rightTurnAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Right Turn";

		/// <summary>
		/// Character left turn animation.
		/// </summary>
		[Category( "Turn" )]
		[DefaultValueReference( rightTurnAnimationDefault )]
		public Reference<Animation> RightTurnAnimation
		{
			get { if( _rightTurnAnimation.BeginGet() ) RightTurnAnimation = _rightTurnAnimation.Get( this ); return _rightTurnAnimation.value; }
			set { if( _rightTurnAnimation.BeginSet( this, ref value ) ) { try { RightTurnAnimationChanged?.Invoke( this ); } finally { _rightTurnAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightTurnAnimation"/> property value changes.</summary>
		public event Action<CharacterType> RightTurnAnimationChanged;
		ReferenceField<Animation> _rightTurnAnimation = new Reference<Animation>( null, rightTurnAnimationDefault );

		/// <summary>
		/// The speed multiplier of turn animations.
		/// </summary>
		[Category( "Turn" )]
		[DefaultValue( 1.0 )]
		[Range( 0.25, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> TurnAnimationSpeed
		{
			get { if( _turnAnimationSpeed.BeginGet() ) TurnAnimationSpeed = _turnAnimationSpeed.Get( this ); return _turnAnimationSpeed.value; }
			set { if( _turnAnimationSpeed.BeginSet( this, ref value ) ) { try { TurnAnimationSpeedChanged?.Invoke( this ); } finally { _turnAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TurnAnimationSpeed"/> property value changes.</summary>
		public event Action<CharacterType> TurnAnimationSpeedChanged;
		ReferenceField<double> _turnAnimationSpeed = 1.0;

		/////////////////////////////////////////
		//Fly

		/// <summary>
		/// Character flying animation.
		/// </summary>
		[Category( "Fly" )]
		[DefaultValueReference( flyAnimationDefault )]
		public Reference<Animation> FlyAnimation
		{
			get { if( _flyAnimation.BeginGet() ) FlyAnimation = _flyAnimation.Get( this ); return _flyAnimation.value; }
			set { if( _flyAnimation.BeginSet( this, ref value ) ) { try { FlyAnimationChanged?.Invoke( this ); } finally { _flyAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyAnimation"/> property value changes.</summary>
		public event Action<CharacterType> FlyAnimationChanged;
		ReferenceField<Animation> _flyAnimation = new Reference<Animation>( null, flyAnimationDefault );

		/// <summary>
		/// Can a character control himself in flight.
		/// </summary>
		[Category( "Fly" )]
		[DefaultValue( true )]
		public Reference<bool> FlyControl
		{
			get { if( _flyControl.BeginGet() ) FlyControl = _flyControl.Get( this ); return _flyControl.value; }
			set { if( _flyControl.BeginSet( this, ref value ) ) { try { FlyControlChanged?.Invoke( this ); } finally { _flyControl.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyControl"/> property value changes.</summary>
		public event Action<CharacterType> FlyControlChanged;
		ReferenceField<bool> _flyControl = true;

		/// <summary>
		/// Maximum speed of character control in flight.
		/// </summary>
		[Category( "Fly" )]
		[DefaultValue( 1 )]//10 )]
		public Reference<double> FlyControlMaxSpeed
		{
			get { if( _flyControlMaxSpeed.BeginGet() ) FlyControlMaxSpeed = _flyControlMaxSpeed.Get( this ); return _flyControlMaxSpeed.value; }
			set { if( _flyControlMaxSpeed.BeginSet( this, ref value ) ) { try { FlyControlMaxSpeedChanged?.Invoke( this ); } finally { _flyControlMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyControlMaxSpeed"/> property value changes.</summary>
		public event Action<CharacterType> FlyControlMaxSpeedChanged;
		ReferenceField<double> _flyControlMaxSpeed = 1;//10;

		///// <summary>
		///// Physical force applied to the body for flying.
		///// </summary>
		//[Category( "Fly" )]
		//[DefaultValue( 10000 )]
		//public Reference<double> FlyControlForce
		//{
		//	get { if( _flyControlForce.BeginGet() ) FlyControlForce = _flyControlForce.Get( this ); return _flyControlForce.value; }
		//	set { if( _flyControlForce.BeginSet( this, ref value ) ) { try { FlyControlForceChanged?.Invoke( this ); } finally { _flyControlForce.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="FlyControlForce"/> property value changes.</summary>
		//public event Action<CharacterType> FlyControlForceChanged;
		//ReferenceField<double> _flyControlForce = 10000;

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
			set { if( _jump.BeginSet( this, ref value ) ) { try { JumpChanged?.Invoke( this ); } finally { _jump.EndSet(); } } }
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
			set { if( _jumpSpeed.BeginSet( this, ref value ) ) { try { JumpSpeedChanged?.Invoke( this ); } finally { _jumpSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpSpeed"/> property value changes.</summary>
		public event Action<CharacterType> JumpSpeedChanged;
		ReferenceField<double> _jumpSpeed = 4;

		const string jumpAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Jump.animation";
		//const string jumpAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Jump";

		/// <summary>
		/// Character jump animation.
		/// </summary>
		[Category( "Jump" )]
		[DefaultValueReference( jumpAnimationDefault )]
		public Reference<Animation> JumpAnimation
		{
			get { if( _jumpAnimation.BeginGet() ) JumpAnimation = _jumpAnimation.Get( this ); return _jumpAnimation.value; }
			set { if( _jumpAnimation.BeginSet( this, ref value ) ) { try { JumpAnimationChanged?.Invoke( this ); } finally { _jumpAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpAnimation"/> property value changes.</summary>
		public event Action<CharacterType> JumpAnimationChanged;
		ReferenceField<Animation> _jumpAnimation = new Reference<Animation>( null, jumpAnimationDefault );

		/////////////////////////////////////////
		//Die

		const string dieAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Die.animation";
		//const string dieAnimationDefault = @"Content\Characters\Default\Human.fbx|$Mesh\$Animations\$Die";

		/// <summary>
		/// The animation of dying.
		/// </summary>
		[Category( "Die" )]
		[DefaultValueReference( dieAnimationDefault )]
		public Reference<Animation> DieAnimation
		{
			get { if( _dieAnimation.BeginGet() ) DieAnimation = _dieAnimation.Get( this ); return _dieAnimation.value; }
			set { if( _dieAnimation.BeginSet( this, ref value ) ) { try { DieAnimationChanged?.Invoke( this ); } finally { _dieAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DieAnimation"/> property value changes.</summary>
		public event Action<CharacterType> DieAnimationChanged;
		ReferenceField<Animation> _dieAnimation = new Reference<Animation>( null, dieAnimationDefault );

		/////////////////////////////////////////
		//Sit

		/// <summary>
		/// The distance from the bottom to the butt when the character is sitting.
		/// </summary>
		[Category( "Sit" )]
		[DefaultValue( 0.4 )]
		public Reference<double> SitButtHeight
		{
			get { if( _sitButtHeight.BeginGet() ) SitButtHeight = _sitButtHeight.Get( this ); return _sitButtHeight.value; }
			set { if( _sitButtHeight.BeginSet( this, ref value ) ) { try { SitButtHeightChanged?.Invoke( this ); } finally { _sitButtHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SitButtHeight"/> property value changes.</summary>
		public event Action<CharacterType> SitButtHeightChanged;
		ReferenceField<double> _sitButtHeight = 0.4;

		const string sitAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Sit.animation";

		/// <summary>
		/// The animation of sitting.
		/// </summary>
		[Category( "Sit" )]
		[DefaultValueReference( sitAnimationDefault )]
		public Reference<Animation> SitAnimation
		{
			get { if( _sitAnimation.BeginGet() ) SitAnimation = _sitAnimation.Get( this ); return _sitAnimation.value; }
			set { if( _sitAnimation.BeginSet( this, ref value ) ) { try { SitAnimationChanged?.Invoke( this ); } finally { _sitAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SitAnimation"/> property value changes.</summary>
		public event Action<CharacterType> SitAnimationChanged;
		ReferenceField<Animation> _sitAnimation = new Reference<Animation>( null, sitAnimationDefault );

		/// <summary>
		/// Whether to enable sitting test in the editor to calibrate SitButtHeight.
		/// </summary>
		[Category( "Sit" )]
		[DefaultValue( false )]
		public Reference<bool> SitTest
		{
			get { if( _sitTest.BeginGet() ) SitTest = _sitTest.Get( this ); return _sitTest.value; }
			set { if( _sitTest.BeginSet( this, ref value ) ) { try { SitTestChanged?.Invoke( this ); } finally { _sitTest.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SitTest"/> property value changes.</summary>
		public event Action<CharacterType> SitTestChanged;
		ReferenceField<bool> _sitTest = false;

		/////////////////////////////////////////
		//Crouching

		//!!!!crouching is disabled
		[Browsable( false )]
		[Category( "Crouching" )]
		[DefaultValue( false )]
		public Reference<bool> Crouching
		{
			get { if( _crouching.BeginGet() ) Crouching = _crouching.Get( this ); return _crouching.value; }
			set { if( _crouching.BeginSet( this, ref value ) ) { try { CrouchingChanged?.Invoke( this ); } finally { _crouching.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Crouching"/> property value changes.</summary>
		public event Action<CharacterType> CrouchingChanged;
		ReferenceField<bool> _crouching = false;

		[Category( "Crouching" )]
		[DefaultValue( 1.0 )]
		public Reference<double> CrouchingHeight
		{
			get { if( _crouchingHeight.BeginGet() ) CrouchingHeight = _crouchingHeight.Get( this ); return _crouchingHeight.value; }
			set { if( _crouchingHeight.BeginSet( this, ref value ) ) { try { CrouchingHeightChanged?.Invoke( this ); } finally { _crouchingHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingHeight"/> property value changes.</summary>
		public event Action<CharacterType> CrouchingHeightChanged;
		ReferenceField<double> _crouchingHeight = 1.0;

		[Category( "Crouching" )]
		[DefaultValue( 0.1 )]
		public Reference<double> CrouchingWalkUpHeight
		{
			get { if( _crouchingWalkUpHeight.BeginGet() ) CrouchingWalkUpHeight = _crouchingWalkUpHeight.Get( this ); return _crouchingWalkUpHeight.value; }
			set { if( _crouchingWalkUpHeight.BeginSet( this, ref value ) ) { try { CrouchingWalkUpHeightChanged?.Invoke( this ); } finally { _crouchingWalkUpHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingWalkUpHeight"/> property value changes.</summary>
		public event Action<CharacterType> CrouchingWalkUpHeightChanged;
		ReferenceField<double> _crouchingWalkUpHeight = 0.1;

		[Category( "Crouching" )]
		[DefaultValue( 0.1 )]
		public Reference<double> CrouchingWalkDownHeight
		{
			get { if( _crouchingWalkDownHeight.BeginGet() ) CrouchingWalkDownHeight = _crouchingWalkDownHeight.Get( this ); return _crouchingWalkDownHeight.value; }
			set { if( _crouchingWalkDownHeight.BeginSet( this, ref value ) ) { try { CrouchingWalkDownHeightChanged?.Invoke( this ); } finally { _crouchingWalkDownHeight.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingWalkDownHeight"/> property value changes.</summary>
		public event Action<CharacterType> CrouchingWalkDownHeightChanged;
		ReferenceField<double> _crouchingWalkDownHeight = 0.1;

		[Category( "Crouching" )]
		[DefaultValue( 1.0 )]
		public Reference<double> CrouchingMaxSpeed
		{
			get { if( _crouchingMaxSpeed.BeginGet() ) CrouchingMaxSpeed = _crouchingMaxSpeed.Get( this ); return _crouchingMaxSpeed.value; }
			set { if( _crouchingMaxSpeed.BeginSet( this, ref value ) ) { try { CrouchingMaxSpeedChanged?.Invoke( this ); } finally { _crouchingMaxSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CrouchingMaxSpeed"/> property value changes.</summary>
		public event Action<CharacterType> CrouchingMaxSpeedChanged;
		ReferenceField<double> _crouchingMaxSpeed = 1.0;

		//[Category( "Crouching" )]
		//[DefaultValue( 100000 )]
		//public Reference<double> CrouchingForce
		//{
		//	get { if( _crouchingForce.BeginGet() ) CrouchingForce = _crouchingForce.Get( this ); return _crouchingForce.value; }
		//	set { if( _crouchingForce.BeginSet( this, ref value ) ) { try { CrouchingForceChanged?.Invoke( this ); } finally { _crouchingForce.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CrouchingForce"/> property value changes.</summary>
		//public event Action<CharacterType> CrouchingForceChanged;
		//ReferenceField<double> _crouchingForce = 100000;

		/////////////////////////////////////////

		//[Category( "Skeleton State" )]
		//[DefaultValue( "spine" )] //!!!!? chest
		//public Reference<string> TorsoBone
		//{
		//	get { if( _torsoBone.BeginGet() ) TorsoBone = _torsoBone.Get( this ); return _torsoBone.value; }
		//	set { if( _torsoBone.BeginSet( this, ref value ) ) { try { TorsoBoneChanged?.Invoke( this ); } finally { _torsoBone.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TorsoBone"/> property value changes.</summary>
		//public event Action<CharacterType> TorsoBoneChanged;
		//ReferenceField<string> _torsoBone = "spine";

		//!!!!or mixamorig:Spine
		/// <summary>
		/// The name of the root bone of upper part of the model.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:Spine1" )]
		public Reference<string> UpperPartBone
		{
			get { if( _upperPartBone.BeginGet() ) UpperPartBone = _upperPartBone.Get( this ); return _upperPartBone.value; }
			set { if( _upperPartBone.BeginSet( this, ref value ) ) { try { UpperPartBoneChanged?.Invoke( this ); } finally { _upperPartBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UpperPartBone"/> property value changes.</summary>
		public event Action<CharacterType> UpperPartBoneChanged;
		ReferenceField<string> _upperPartBone = "mixamorig:Spine1";

		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:Spine" )]
		[DisplayName( "Spine Bone 1" )]
		public Reference<string> SpineBone1
		{
			get { if( _spineBone1.BeginGet() ) SpineBone1 = _spineBone1.Get( this ); return _spineBone1.value; }
			set { if( _spineBone1.BeginSet( this, ref value ) ) { try { SpineBone1Changed?.Invoke( this ); } finally { _spineBone1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpineBone1"/> property value changes.</summary>
		public event Action<CharacterType> SpineBone1Changed;
		ReferenceField<string> _spineBone1 = "mixamorig:Spine";

		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:Spine1" )]
		[DisplayName( "Spine Bone 2" )]
		public Reference<string> SpineBone2
		{
			get { if( _spineBone2.BeginGet() ) SpineBone2 = _spineBone2.Get( this ); return _spineBone2.value; }
			set { if( _spineBone2.BeginSet( this, ref value ) ) { try { SpineBone2Changed?.Invoke( this ); } finally { _spineBone2.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpineBone2"/> property value changes.</summary>
		public event Action<CharacterType> SpineBone2Changed;
		ReferenceField<string> _spineBone2 = "mixamorig:Spine1";

		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:Spine2" )]
		[DisplayName( "Spine Bone 3" )]
		public Reference<string> SpineBone3
		{
			get { if( _spineBone3.BeginGet() ) SpineBone3 = _spineBone3.Get( this ); return _spineBone3.value; }
			set { if( _spineBone3.BeginSet( this, ref value ) ) { try { SpineBone3Changed?.Invoke( this ); } finally { _spineBone3.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpineBone3"/> property value changes.</summary>
		public event Action<CharacterType> SpineBone3Changed;
		ReferenceField<string> _spineBone3 = "mixamorig:Spine2";

		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:LeftShoulder" )]
		public Reference<string> LeftShoulderBone
		{
			get { if( _leftShoulderBone.BeginGet() ) LeftShoulderBone = _leftShoulderBone.Get( this ); return _leftShoulderBone.value; }
			set { if( _leftShoulderBone.BeginSet( this, ref value ) ) { try { LeftShoulderBoneChanged?.Invoke( this ); } finally { _leftShoulderBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftShoulderBone"/> property value changes.</summary>
		public event Action<CharacterType> LeftShoulderBoneChanged;
		ReferenceField<string> _leftShoulderBone = "mixamorig:LeftShoulder";

		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:RightShoulder" )]
		public Reference<string> RightShoulderBone
		{
			get { if( _rightShoulderBone.BeginGet() ) RightShoulderBone = _rightShoulderBone.Get( this ); return _rightShoulderBone.value; }
			set { if( _rightShoulderBone.BeginSet( this, ref value ) ) { try { RightShoulderBoneChanged?.Invoke( this ); } finally { _rightShoulderBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightShoulderBone"/> property value changes.</summary>
		public event Action<CharacterType> RightShoulderBoneChanged;
		ReferenceField<string> _rightShoulderBone = "mixamorig:RightShoulder";


		/// <summary>
		/// The name of the left arm bone.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:LeftArm" )]
		public Reference<string> LeftArmBone
		{
			get { if( _leftArmBone.BeginGet() ) LeftArmBone = _leftArmBone.Get( this ); return _leftArmBone.value; }
			set { if( _leftArmBone.BeginSet( this, ref value ) ) { try { LeftArmBoneChanged?.Invoke( this ); } finally { _leftArmBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftArmBone"/> property value changes.</summary>
		public event Action<CharacterType> LeftArmBoneChanged;
		ReferenceField<string> _leftArmBone = "mixamorig:LeftArm";

		/// <summary>
		/// The name of the right arm bone.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:RightArm" )]
		public Reference<string> RightArmBone
		{
			get { if( _rightArmBone.BeginGet() ) RightArmBone = _rightArmBone.Get( this ); return _rightArmBone.value; }
			set { if( _rightArmBone.BeginSet( this, ref value ) ) { try { RightArmBoneChanged?.Invoke( this ); } finally { _rightArmBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightArmBone"/> property value changes.</summary>
		public event Action<CharacterType> RightArmBoneChanged;
		ReferenceField<string> _rightArmBone = "mixamorig:RightArm";


		/// <summary>
		/// The name of the left hand bone.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:LeftHand" )]
		public Reference<string> LeftHandBone
		{
			get { if( _leftHandBone.BeginGet() ) LeftHandBone = _leftHandBone.Get( this ); return _leftHandBone.value; }
			set { if( _leftHandBone.BeginSet( this, ref value ) ) { try { LeftHandBoneChanged?.Invoke( this ); } finally { _leftHandBone.EndSet(); } } }
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
			set { if( _rightHandBone.BeginSet( this, ref value ) ) { try { RightHandBoneChanged?.Invoke( this ); } finally { _rightHandBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandBone"/> property value changes.</summary>
		public event Action<CharacterType> RightHandBoneChanged;
		ReferenceField<string> _rightHandBone = "mixamorig:RightHand";


		/// <summary>
		/// The format of naming for finger bones.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:{0}Hand{1}{2}" )]
		public Reference<string> FingerBoneFormat
		{
			get { if( _fingerBoneFormat.BeginGet() ) FingerBoneFormat = _fingerBoneFormat.Get( this ); return _fingerBoneFormat.value; }
			set { if( _fingerBoneFormat.BeginSet( this, ref value ) ) { try { FingerBoneFormatChanged?.Invoke( this ); } finally { _fingerBoneFormat.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FingerBoneFormat"/> property value changes.</summary>
		public event Action<CharacterType> FingerBoneFormatChanged;
		ReferenceField<string> _fingerBoneFormat = "mixamorig:{0}Hand{1}{2}";

		/// <summary>
		/// The name of the head bone.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:Head" )]
		public Reference<string> HeadBone
		{
			get { if( _headBone.BeginGet() ) HeadBone = _headBone.Get( this ); return _headBone.value; }
			set { if( _headBone.BeginSet( this, ref value ) ) { try { HeadBoneChanged?.Invoke( this ); } finally { _headBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadBone"/> property value changes.</summary>
		public event Action<CharacterType> HeadBoneChanged;
		ReferenceField<string> _headBone = "mixamorig:Head";

		/// <summary>
		/// The name of the head top end bone.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:HeadTop_End" )]
		public Reference<string> HeadTopBone
		{
			get { if( _headTopBone.BeginGet() ) HeadTopBone = _headTopBone.Get( this ); return _headTopBone.value; }
			set { if( _headTopBone.BeginSet( this, ref value ) ) { try { HeadTopBoneChanged?.Invoke( this ); } finally { _headTopBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadTopBone"/> property value changes.</summary>
		public event Action<CharacterType> HeadTopBoneChanged;
		ReferenceField<string> _headTopBone = "mixamorig:HeadTop_End";

		///// <summary>
		///// The name of the left eye bone.
		///// </summary>
		//[Category( "Skeleton Control" )]
		//[DefaultValue( "mixamorig:LeftEye" )]
		//public Reference<string> LeftEyeBone
		//{
		//	get { if( _leftEyeBone.BeginGet() ) LeftEyeBone = _leftEyeBone.Get( this ); return _leftEyeBone.value; }
		//	set { if( _leftEyeBone.BeginSet( this, ref value ) ) { try { LeftEyeBoneChanged?.Invoke( this ); } finally { _leftEyeBone.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LeftEyeBone"/> property value changes.</summary>
		//public event Action<CharacterType> LeftEyeBoneChanged;
		//ReferenceField<string> _leftEyeBone = "mixamorig:LeftEye";

		///// <summary>
		///// The name of the right eye bone.
		///// </summary>
		//[Category( "Skeleton Control" )]
		//[DefaultValue( "mixamorig:RightEye" )]
		//public Reference<string> RightEyeBone
		//{
		//	get { if( _rightEyeBone.BeginGet() ) RightEyeBone = _rightEyeBone.Get( this ); return _rightEyeBone.value; }
		//	set { if( _rightEyeBone.BeginSet( this, ref value ) ) { try { RightEyeBoneChanged?.Invoke( this ); } finally { _rightEyeBone.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RightEyeBone"/> property value changes.</summary>
		//public event Action<CharacterType> RightEyeBoneChanged;
		//ReferenceField<string> _rightEyeBone = "mixamorig:RightEye";

		///////////////////////////////////////////////

		/// <summary>
		/// The name of the left leg bone.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:LeftLeg" )]
		public Reference<string> LeftLegBone
		{
			get { if( _leftLegBone.BeginGet() ) LeftLegBone = _leftLegBone.Get( this ); return _leftLegBone.value; }
			set { if( _leftLegBone.BeginSet( this, ref value ) ) { try { LeftLegBoneChanged?.Invoke( this ); } finally { _leftLegBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftLegBone"/> property value changes.</summary>
		public event Action<CharacterType> LeftLegBoneChanged;
		ReferenceField<string> _leftLegBone = "mixamorig:LeftLeg";

		/// <summary>
		/// The name of the right leg bone.
		/// </summary>
		[Category( "Skeleton Control" )]
		[DefaultValue( "mixamorig:RightLeg" )]
		public Reference<string> RightLegBone
		{
			get { if( _rightLegBone.BeginGet() ) RightLegBone = _rightLegBone.Get( this ); return _rightLegBone.value; }
			set { if( _rightLegBone.BeginSet( this, ref value ) ) { try { RightLegBoneChanged?.Invoke( this ); } finally { _rightLegBone.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightLegBone"/> property value changes.</summary>
		public event Action<CharacterType> RightLegBoneChanged;
		ReferenceField<string> _rightLegBone = "mixamorig:RightLeg";

		///////////////////////////////////////////////

		const string footstepSoundDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Sounds\Footstep.ogg";
		/// <summary>
		/// The sound of footstep when walking or running.
		/// </summary>
		[Category( "Sound" )]
		[DefaultValueReference( footstepSoundDefault )]
		public Reference<Sound> FootstepSound
		{
			get { if( _footstepSound.BeginGet() ) FootstepSound = _footstepSound.Get( this ); return _footstepSound.value; }
			set { if( _footstepSound.BeginSet( this, ref value ) ) { try { FootstepSoundChanged?.Invoke( this ); } finally { _footstepSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FootstepSound"/> property value changes.</summary>
		public event Action<CharacterType> FootstepSoundChanged;
		ReferenceField<Sound> _footstepSound = new Reference<Sound>( null, footstepSoundDefault );

		const string jumpSoundDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Sounds\Jump.ogg";
		[Category( "Sound" )]
		[DefaultValueReference( jumpSoundDefault )]
		public Reference<Sound> JumpSound
		{
			get { if( _jumpSound.BeginGet() ) JumpSound = _jumpSound.Get( this ); return _jumpSound.value; }
			set { if( _jumpSound.BeginSet( this, ref value ) ) { try { JumpSoundChanged?.Invoke( this ); } finally { _jumpSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="JumpSound"/> property value changes.</summary>
		public event Action<CharacterType> JumpSoundChanged;
		ReferenceField<Sound> _jumpSound = new Reference<Sound>( null, jumpSoundDefault );

		const string flyEndSoundDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Sounds\Fly end.ogg";
		[Category( "Sound" )]
		[DefaultValueReference( flyEndSoundDefault )]
		public Reference<Sound> FlyEndSound
		{
			get { if( _flyEndSound.BeginGet() ) FlyEndSound = _flyEndSound.Get( this ); return _flyEndSound.value; }
			set { if( _flyEndSound.BeginSet( this, ref value ) ) { try { FlyEndSoundChanged?.Invoke( this ); } finally { _flyEndSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlyEndSound"/> property value changes.</summary>
		public event Action<CharacterType> FlyEndSoundChanged;
		ReferenceField<Sound> _flyEndSound = new Reference<Sound>( null, flyEndSoundDefault );

		/////////////////////////////////////////

		[Category( "Rifle" )]
		[DefaultValue( "0.46 -0.14 1.39" )]
		public Reference<Vector3> RifleAimingArmsCenter
		{
			get { if( _rifleAimingArmsCenter.BeginGet() ) RifleAimingArmsCenter = _rifleAimingArmsCenter.Get( this ); return _rifleAimingArmsCenter.value; }
			set { if( _rifleAimingArmsCenter.BeginSet( this, ref value ) ) { try { RifleAimingArmsCenterChanged?.Invoke( this ); } finally { _rifleAimingArmsCenter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RifleAimingArmsCenter"/> property value changes.</summary>
		public event Action<CharacterType> RifleAimingArmsCenterChanged;
		ReferenceField<Vector3> _rifleAimingArmsCenter = new Vector3( 0.46, -0.14, 1.39 );

		[Category( "Rifle" )]
		[DefaultValue( "0.46 -0.3 1.25" )]
		public Reference<Vector3> RifleAimingArmsCenterFirstPerson
		{
			get { if( _rifleAimingArmsCenterFirstPerson.BeginGet() ) RifleAimingArmsCenterFirstPerson = _rifleAimingArmsCenterFirstPerson.Get( this ); return _rifleAimingArmsCenterFirstPerson.value; }
			set { if( _rifleAimingArmsCenterFirstPerson.BeginSet( this, ref value ) ) { try { RifleAimingArmsCenterFirstPersonChanged?.Invoke( this ); } finally { _rifleAimingArmsCenterFirstPerson.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RifleAimingArmsCenterFirstPerson"/> property value changes.</summary>
		public event Action<CharacterType> RifleAimingArmsCenterFirstPersonChanged;
		ReferenceField<Vector3> _rifleAimingArmsCenterFirstPerson = new Vector3( 0.46, -0.3, 1.25 );

		const string rifleAimingIdleAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Rifle Aiming Idle.animation";

		/// <summary>
		/// Animation of character at rest with the rifle.
		/// </summary>
		[Category( "Rifle" )]
		[DefaultValueReference( rifleAimingIdleAnimationDefault )]
		public Reference<Animation> RifleAimingIdleAnimation
		{
			get { if( _rifleAimingIdleAnimation.BeginGet() ) RifleAimingIdleAnimation = _rifleAimingIdleAnimation.Get( this ); return _rifleAimingIdleAnimation.value; }
			set { if( _rifleAimingIdleAnimation.BeginSet( this, ref value ) ) { try { RifleAimingIdleAnimationChanged?.Invoke( this ); } finally { _rifleAimingIdleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RifleAimingIdleAnimation"/> property value changes.</summary>
		public event Action<CharacterType> RifleAimingIdleAnimationChanged;
		ReferenceField<Animation> _rifleAimingIdleAnimation = new Reference<Animation>( null, rifleAimingIdleAnimationDefault );

		const string rifleAimingIdleMinus45AnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Rifle Aiming Idle Minus 45.animation";

		[Category( "Rifle" )]
		[DefaultValueReference( rifleAimingIdleMinus45AnimationDefault )]
		[DisplayName( "Rifle Aiming Idle Minus 45 Animation" )]
		public Reference<Animation> RifleAimingIdleMinus45Animation
		{
			get { if( _rifleAimingIdleMinus45Animation.BeginGet() ) RifleAimingIdleMinus45Animation = _rifleAimingIdleMinus45Animation.Get( this ); return _rifleAimingIdleMinus45Animation.value; }
			set { if( _rifleAimingIdleMinus45Animation.BeginSet( this, ref value ) ) { try { RifleAimingIdleMinus45AnimationChanged?.Invoke( this ); } finally { _rifleAimingIdleMinus45Animation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RifleAimingIdleMinus45Animation"/> property value changes.</summary>
		public event Action<CharacterType> RifleAimingIdleMinus45AnimationChanged;
		ReferenceField<Animation> _rifleAimingIdleMinus45Animation = new Reference<Animation>( null, rifleAimingIdleMinus45AnimationDefault );

		const string rifleAimingIdlePlus45AnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Rifle Aiming Idle Plus 45.animation";

		[Category( "Rifle" )]
		[DefaultValueReference( rifleAimingIdlePlus45AnimationDefault )]
		[DisplayName( "Rifle Aiming Idle Plus 45 Animation" )]
		public Reference<Animation> RifleAimingIdlePlus45Animation
		{
			get { if( _rifleAimingIdlePlus45Animation.BeginGet() ) RifleAimingIdlePlus45Animation = _rifleAimingIdlePlus45Animation.Get( this ); return _rifleAimingIdlePlus45Animation.value; }
			set { if( _rifleAimingIdlePlus45Animation.BeginSet( this, ref value ) ) { try { RifleAimingIdlePlus45AnimationChanged?.Invoke( this ); } finally { _rifleAimingIdlePlus45Animation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RifleAimingIdlePlus45Animation"/> property value changes.</summary>
		public event Action<CharacterType> RifleAimingIdlePlus45AnimationChanged;
		ReferenceField<Animation> _rifleAimingIdlePlus45Animation = new Reference<Animation>( null, rifleAimingIdlePlus45AnimationDefault );

		const string rifleAimingWalkingAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Rifle Aiming Walking.animation";

		/// <summary>
		/// Character walking animation with the aiming rifle.
		/// </summary>
		[Category( "Rifle" )]
		[DefaultValueReference( rifleAimingWalkingAnimationDefault )]
		public Reference<Animation> RifleAimingWalkAnimation
		{
			get { if( _rifleAimingWalkAnimation.BeginGet() ) RifleAimingWalkAnimation = _rifleAimingWalkAnimation.Get( this ); return _rifleAimingWalkAnimation.value; }
			set { if( _rifleAimingWalkAnimation.BeginSet( this, ref value ) ) { try { RifleAimingWalkAnimationChanged?.Invoke( this ); } finally { _rifleAimingWalkAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RifleAimingWalkAnimation"/> property value changes.</summary>
		public event Action<CharacterType> RifleAimingWalkAnimationChanged;
		ReferenceField<Animation> _rifleAimingWalkAnimation = new Reference<Animation>( null, rifleAimingWalkingAnimationDefault );

		/// <summary>
		/// The multiplier for playing the walking animation depending on the speed of the character.
		/// </summary>
		[Category( "Rifle" )]
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> RifleAimingWalkAnimationSpeed
		{
			get { if( _rifleAimingWalkAnimationSpeed.BeginGet() ) RifleAimingWalkAnimationSpeed = _rifleAimingWalkAnimationSpeed.Get( this ); return _rifleAimingWalkAnimationSpeed.value; }
			set { if( _rifleAimingWalkAnimationSpeed.BeginSet( this, ref value ) ) { try { RifleAimingWalkAnimationSpeedChanged?.Invoke( this ); } finally { _rifleAimingWalkAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RifleAimingWalkAnimationSpeed"/> property value changes.</summary>
		public event Action<CharacterType> RifleAimingWalkAnimationSpeedChanged;
		ReferenceField<double> _rifleAimingWalkAnimationSpeed = 1.0;

		///////////////////////////////////////////////

		[Category( "Item" )]
		[DefaultValue( "0.25 -0.25 1.15" )]
		public Reference<Vector3> ItemHoldingPosition
		{
			get { if( _itemHoldingPosition.BeginGet() ) ItemHoldingPosition = _itemHoldingPosition.Get( this ); return _itemHoldingPosition.value; }
			set { if( _itemHoldingPosition.BeginSet( this, ref value ) ) { try { ItemHoldingPositionChanged?.Invoke( this ); } finally { _itemHoldingPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ItemHoldingPosition"/> property value changes.</summary>
		public event Action<CharacterType> ItemHoldingPositionChanged;
		ReferenceField<Vector3> _itemHoldingPosition = new Vector3( 0.25, -0.25, 1.15 );

		/// <summary>	
		/// Animation of character at rest with the rifle.
		/// </summary>
		[Category( "Item" )]
		[DefaultValue( null )]
		public Reference<Animation> ItemHoldingIdleAnimation
		{
			get { if( _itemHoldingIdleAnimation.BeginGet() ) ItemHoldingIdleAnimation = _itemHoldingIdleAnimation.Get( this ); return _itemHoldingIdleAnimation.value; }
			set { if( _itemHoldingIdleAnimation.BeginSet( this, ref value ) ) { try { ItemHoldingIdleAnimationChanged?.Invoke( this ); } finally { _itemHoldingIdleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ItemHoldingIdleAnimation"/> property value changes.</summary>
		public event Action<CharacterType> ItemHoldingIdleAnimationChanged;
		ReferenceField<Animation> _itemHoldingIdleAnimation;

		///////////////////////////////////////////////

		const string oneHandedMeleeWeaponIdleAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Sword And Shield Idle.animation";

		/// <summary>
		/// Animation of character at rest with one-handed melee weapon.
		/// </summary>
		[Category( "One-Handed Melee Weapon" )]
		[DefaultValueReference( oneHandedMeleeWeaponIdleAnimationDefault )]
		public Reference<Animation> OneHandedMeleeWeaponIdleAnimation
		{
			get { if( _oneHandedMeleeWeaponIdleAnimation.BeginGet() ) OneHandedMeleeWeaponIdleAnimation = _oneHandedMeleeWeaponIdleAnimation.Get( this ); return _oneHandedMeleeWeaponIdleAnimation.value; }
			set { if( _oneHandedMeleeWeaponIdleAnimation.BeginSet( this, ref value ) ) { try { OneHandedMeleeWeaponIdleAnimationChanged?.Invoke( this ); } finally { _oneHandedMeleeWeaponIdleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OneHandedMeleeWeaponIdleAnimation"/> property value changes.</summary>
		public event Action<CharacterType> OneHandedMeleeWeaponIdleAnimationChanged;
		ReferenceField<Animation> _oneHandedMeleeWeaponIdleAnimation = new Reference<Animation>( null, oneHandedMeleeWeaponIdleAnimationDefault );

		const string oneHandedMeleeWeaponWalkAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Sword And Shield Walk.animation";

		/// <summary>
		/// Walk animation with one-handed melee weapon.
		/// </summary>
		[Category( "One-Handed Melee Weapon" )]
		[DefaultValueReference( oneHandedMeleeWeaponWalkAnimationDefault )]
		public Reference<Animation> OneHandedMeleeWeaponWalkAnimation
		{
			get { if( _oneHandedMeleeWeaponWalkAnimation.BeginGet() ) OneHandedMeleeWeaponWalkAnimation = _oneHandedMeleeWeaponWalkAnimation.Get( this ); return _oneHandedMeleeWeaponWalkAnimation.value; }
			set { if( _oneHandedMeleeWeaponWalkAnimation.BeginSet( this, ref value ) ) { try { OneHandedMeleeWeaponWalkAnimationChanged?.Invoke( this ); } finally { _oneHandedMeleeWeaponWalkAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OneHandedMeleeWeaponWalkAnimation"/> property value changes.</summary>
		public event Action<CharacterType> OneHandedMeleeWeaponWalkAnimationChanged;
		ReferenceField<Animation> _oneHandedMeleeWeaponWalkAnimation = new Reference<Animation>( null, oneHandedMeleeWeaponWalkAnimationDefault );

		/// <summary>
		/// The multiplier for playing the walking with one-handed melee weapon animation depending on the speed of the character.
		/// </summary>
		[Category( "One-Handed Melee Weapon" )]
		[DefaultValue( 0.75 )]//1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> OneHandedMeleeWeaponWalkAnimationSpeed
		{
			get { if( _oneHandedMeleeWeaponWalkAnimationSpeed.BeginGet() ) OneHandedMeleeWeaponWalkAnimationSpeed = _oneHandedMeleeWeaponWalkAnimationSpeed.Get( this ); return _oneHandedMeleeWeaponWalkAnimationSpeed.value; }
			set { if( _oneHandedMeleeWeaponWalkAnimationSpeed.BeginSet( this, ref value ) ) { try { OneHandedMeleeWeaponWalkAnimationSpeedChanged?.Invoke( this ); } finally { _oneHandedMeleeWeaponWalkAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OneHandedMeleeWeaponWalkAnimationSpeed"/> property value changes.</summary>
		public event Action<CharacterType> OneHandedMeleeWeaponWalkAnimationSpeedChanged;
		ReferenceField<double> _oneHandedMeleeWeaponWalkAnimationSpeed = 0.75;//1.0;

		const string oneHandedMeleeWeaponAttackAnimationDefault = @"Content\Characters\Authors\NeoAxis\Bryce\Animations\Sword And Shield Slash.animation";

		/// <summary>
		/// Attack animation by one-handed melee weapon.
		/// </summary>
		[Category( "One-Handed Melee Weapon" )]
		[DefaultValueReference( oneHandedMeleeWeaponAttackAnimationDefault )]
		public Reference<Animation> OneHandedMeleeWeaponAttackAnimation
		{
			get { if( _oneHandedMeleeWeaponAttackAnimation.BeginGet() ) OneHandedMeleeWeaponAttackAnimation = _oneHandedMeleeWeaponAttackAnimation.Get( this ); return _oneHandedMeleeWeaponAttackAnimation.value; }
			set { if( _oneHandedMeleeWeaponAttackAnimation.BeginSet( this, ref value ) ) { try { OneHandedMeleeWeaponAttackAnimationChanged?.Invoke( this ); } finally { _oneHandedMeleeWeaponAttackAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OneHandedMeleeWeaponAttackAnimation"/> property value changes.</summary>
		public event Action<CharacterType> OneHandedMeleeWeaponAttackAnimationChanged;
		ReferenceField<Animation> _oneHandedMeleeWeaponAttackAnimation = new Reference<Animation>( null, oneHandedMeleeWeaponAttackAnimationDefault );

		/// <summary>
		/// The multiplier for playing the attacking with one-handed melee weapon animation depending on the speed of the character.
		/// </summary>
		[Category( "One-Handed Melee Weapon" )]
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> OneHandedMeleeWeaponAttackAnimationSpeed
		{
			get { if( _oneHandedMeleeWeaponAttackAnimationSpeed.BeginGet() ) OneHandedMeleeWeaponAttackAnimationSpeed = _oneHandedMeleeWeaponAttackAnimationSpeed.Get( this ); return _oneHandedMeleeWeaponAttackAnimationSpeed.value; }
			set { if( _oneHandedMeleeWeaponAttackAnimationSpeed.BeginSet( this, ref value ) ) { try { OneHandedMeleeWeaponAttackAnimationSpeedChanged?.Invoke( this ); } finally { _oneHandedMeleeWeaponAttackAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OneHandedMeleeWeaponAttackAnimationSpeed"/> property value changes.</summary>
		public event Action<CharacterType> OneHandedMeleeWeaponAttackAnimationSpeedChanged;
		ReferenceField<double> _oneHandedMeleeWeaponAttackAnimationSpeed = 1.0;

		///////////////////////////////////////////////
		//Crawl

		////damageFastChangeSpeed

		//const float damageFastChangeSpeedMinimalSpeedDefault = 10;
		//[FieldSerialize]
		//float damageFastChangeSpeedMinimalSpeed = damageFastChangeSpeedMinimalSpeedDefault;

		//const float damageFastChangeSpeedFactorDefault = 40;
		//[FieldSerialize]
		//float damageFastChangeSpeedFactor = damageFastChangeSpeedFactorDefault;

		///////////////////////////////////////////////

		//[Category( "Mesh Generator" )]
		//[DefaultValue( false )]
		//public Reference<bool> GenerateMesh
		//{
		//	get { if( _generateMesh.BeginGet() ) GenerateMesh = _generateMesh.Get( this ); return _generateMesh.value; }
		//	set { if( _generateMesh.BeginSet( this, ref value ) ) { try { GenerateMeshChanged?.Invoke( this ); MeshGeneratorNeedUpdate(); } finally { _generateMesh.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="GenerateMesh"/> property value changes.</summary>
		//public event Action<CharacterType> GenerateMeshChanged;
		//ReferenceField<bool> _generateMesh = false;

		////!!!!
		///// <summary>
		///// The color multiplier.
		///// </summary>
		//[Category( "Mesh Generator" )]
		//[DefaultValue( "0.1 0.7 0.1" )]
		//public Reference<ColorValue> GenerateMeshColor
		//{
		//	get { if( _generateMeshColor.BeginGet() ) GenerateMeshColor = _generateMeshColor.Get( this ); return _generateMeshColor.value; }
		//	set { if( _generateMeshColor.BeginSet( this, ref value ) ) { try { GenerateMeshChanged?.Invoke( this ); MeshGeneratorNeedUpdate(); } finally { _generateMeshColor.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		//public event Action<CharacterMaker> GenerateMeshColorChanged;
		//ReferenceField<ColorValue> _generateMeshColor = new ColorValue( 0.1, 0.7, 0.1 );

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
					//case nameof( RunForce ):
					if( !Run )
						skip = true;
					break;

				case nameof( FlyControlMaxSpeed ):
					//case nameof( FlyControlForce ):
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
				case nameof( CrouchingWalkDownHeight ):
				case nameof( CrouchingMaxSpeed ):
					//case nameof( CrouchingForce ):
					if( !Crouching )
						skip = true;
					break;

					//case nameof( GenerateMeshColor ):
					//	if( !GenerateMesh )
					//		skip = true;
					//	break;

					//case nameof( IdleAnimation ):
					//case nameof( WalkAnimation ):
					//case nameof( WalkAnimationSpeed ):
					//case nameof( RunAnimation ):
					//case nameof( RunAnimationSpeed ):
					//case nameof( FlyAnimation ):
					//case nameof( JumpAnimation ):
					//case nameof( LeftTurnAnimation ):
					//case nameof( RightTurnAnimation ):
					//case nameof( DieAnimation ):
					//case nameof( SitAnimation ):
					//case nameof( RifleAimingIdleAnimation ):
					//case nameof( RifleAimingIdleMinus45Animation ):
					//case nameof( RifleAimingIdlePlus45Animation ):
					//case nameof( RifleAimingWalkAnimation ):
					//case nameof( RifleAimingWalkAnimationSpeed ):
					//case nameof( ItemHoldingIdleAnimation ):
					//case nameof( OneHandedMeleeWeaponIdleAnimation ):
					//case nameof( OneHandedMeleeWeaponWalkAnimation ):
					//case nameof( OneHandedMeleeWeaponWalkAnimationSpeed ):
					//case nameof( OneHandedMeleeWeaponAttackAnimation ):
					//case nameof( OneHandedMeleeWeaponAttackAnimationSpeed ):
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

		public void GetBodyFormInfo( bool crouching, out double height, out double walkUpHeight, out double walkDownHeight )
		{
			if( crouching )
			{
				height = CrouchingHeight;
				walkUpHeight = CrouchingWalkUpHeight;
				walkDownHeight = CrouchingWalkDownHeight;
			}
			else
			{
				height = Height;
				walkUpHeight = WalkUpHeight;
				walkDownHeight = WalkDownHeight;
			}
		}

		public delegate void FootstepSoundBeforeDelegate( CharacterType sender, Character character, MeshInSpaceAnimationController.AnimationStateClass animationState, MeshInSpaceAnimationController.AnimationStateClass.AnimationItem animationItem, AnimationTrigger trigger, ref bool handled );
		public event FootstepSoundBeforeDelegate FootstepSoundBefore;

		public void PerformFootstepSoundBefore( Character character, MeshInSpaceAnimationController.AnimationStateClass animationState, MeshInSpaceAnimationController.AnimationStateClass.AnimationItem animationItem, AnimationTrigger trigger, ref bool handled )
		{
			FootstepSoundBefore?.Invoke( this, character, animationState, animationItem, trigger, ref handled );
		}

		public delegate void FlyEndSoundBeforeDelegate( CharacterType sender, Character character, ref bool handled );
		public event FlyEndSoundBeforeDelegate FlyEndSoundBefore;

		public void PerformFlyEndSoundBefore( Character character, ref bool handled )
		{
			FlyEndSoundBefore?.Invoke( this, character, ref handled );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public string GetFingerBoneName( string handName, string fingerName, int boneNumber )
		{
			try
			{
				return string.Format( FingerBoneFormat.Value, handName, fingerName, boneNumber );
			}
			catch
			{
				return "";
			}
		}

		//protected override void OnUpdate( float delta )
		//{
		//	base.OnUpdate( delta );

		//	MeshGeneratorUpdateIfNeed();
		//}

		////!!!!when no sense to call this method?
		//public void MeshGeneratorNeedUpdate()
		//{
		//	meshGeneratorNeedUpdate = true;
		//}

		//public bool MeshGeneratorUpdateIfNeed()
		//{
		//	////!!!!
		//	//return false;

		//	if( meshGeneratorNeedUpdate )
		//	{
		//		DoGenerateMesh();
		//		return true;
		//	}
		//	return false;
		//}

		//void MeshGeneratorDeleteComponent()
		//{
		//	var generatedMesh = GetComponent<Mesh>( "Generated Mesh" );
		//	if( generatedMesh != null )
		//	{
		//		generatedMesh.Dispose();

		//		//restore Mesh property
		//		Mesh = new Reference<Mesh>( null, meshDefault );
		//	}
		//}

		//void DoGenerateMesh()
		//{
		//	MeshGeneratorDeleteComponent();
		//	meshGeneratorNeedUpdate = false;

		//	if( !GenerateMesh )
		//		return;


		//	var sourceSkeleton = ResourceManager.LoadResource<Skeleton>( @"Base\Components\CharacterMaker\Skeleton.component" );
		//	var sourceHeight = 1.947;

		//	//create a mesh
		//	var mesh = CreateComponent<Mesh>();
		//	mesh.Name = "Generated Mesh";

		//	//create a mesh geometry
		//	var geometry = mesh.CreateComponent<MeshGeometry>();
		//	{
		//		//!!!!
		//		var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out int vertexSize );
		//		//var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

		//		var boxes = new Bounds[ 8 ];
		//		boxes[ 0 ] = new Bounds( -0.06, -0.13, 0.4, 0.06, 0.13, 0.8 );
		//		boxes[ 1 ] = new Bounds( -0.08, -0.08, 0.8, 0.08, 0.08, 1.0 );
		//		boxes[ 2 ] = new Bounds( -0.04, -0.04 - 0.05, 0.0, 0.04, 0.04 - 0.05, 0.4 );
		//		boxes[ 3 ] = new Bounds( -0.04, -0.04 + 0.05, 0.0, 0.04, 0.04 + 0.05, 0.4 );
		//		boxes[ 4 ] = new Bounds( -0.04, -0.04 - 0.05, 0.0, 0.10, 0.04 - 0.05, 0.05 );
		//		boxes[ 5 ] = new Bounds( -0.04, -0.04 + 0.05, 0.0, 0.10, 0.04 + 0.05, 0.05 );
		//		boxes[ 6 ] = new Bounds( -0.04, -0.04 - 0.17, 0.40, 0.04, 0.04 - 0.17, 0.75 );
		//		boxes[ 7 ] = new Bounds( -0.04, -0.04 + 0.17, 0.40, 0.04, 0.04 + 0.17, 0.75 );

		//		//scale depending Height
		//		for( int n = 0; n < boxes.Length; n++ )
		//		{
		//			ref var b = ref boxes[ n ];
		//			b.Minimum *= Height.Value;
		//			b.Maximum *= Height.Value;
		//		}

		//		var totalVertices = 24 * boxes.Length;
		//		var totalIndices = 36 * boxes.Length;

		//		var vertices = new byte[ vertexSize * totalVertices ];
		//		var indices = new int[ totalIndices ];

		//		unsafe
		//		{
		//			fixed( byte* pVertices = vertices )
		//			{
		//				StandardVertex* pVertices2 = (StandardVertex*)pVertices;
		//				//StandardVertex.StaticOneTexCoord* pVertices2 = (StandardVertex.StaticOneTexCoord*)pVertices;
		//				var currentVertex = 0;
		//				var currentIndex = 0;

		//				for( int nBox = 0; nBox < boxes.Length; nBox++ )
		//				{
		//					var b = boxes[ nBox ];

		//					SimpleMeshGenerator.GenerateBox( b.GetSize(), false, out var positions, out Vector3F[] normals, out var tangents, out var texCoords, out var indices2, out _ );

		//					var center = b.GetCenter().ToVector3F();

		//					var indexOffset = currentVertex;

		//					for( int n = 0; n < positions.Length; n++ )
		//					{
		//						var pVertex = pVertices2 + currentVertex;

		//						pVertex->Position = positions[ n ] + center;
		//						pVertex->Normal = normals[ n ];
		//						pVertex->Tangent = tangents[ n ];
		//						pVertex->Color = new ColorValue( 1, 1, 1, 1 );
		//						pVertex->TexCoord0 = texCoords[ n ];

		//						pVertex->TexCoord1 = texCoords[ n ];
		//						pVertex->TexCoord2 = texCoords[ n ];
		//						pVertex->TexCoord3 = texCoords[ n ];

		//						//!!!!
		//						pVertex->BlendIndices = new Vector4I( 3, 0, 0, 0 );
		//						pVertex->BlendWeights = new Vector4F( 1, 0, 0, 0 );

		//						currentVertex++;
		//					}

		//					for( int n = 0; n < indices2.Length; n++ )
		//						indices[ currentIndex++ ] = indices2[ n ] + indexOffset;
		//				}
		//			}
		//		}


		//		//var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

		//		//var size = new Vector3( Height / 6, Height / 3, Height );
		//		//SimpleMeshGenerator.GenerateBox( size, false, out var positions, out Vector3F[] normals, out var tangents, out var texCoords, out var indices, out _ );

		//		//var vertices = new byte[ vertexSize * positions.Length ];
		//		//unsafe
		//		//{
		//		//	fixed( byte* pVertices = vertices )
		//		//	{
		//		//		StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

		//		//		for( int n = 0; n < positions.Length; n++ )
		//		//		{
		//		//			pVertex->Position = positions[ n ] + new Vector3F( 0, 0, (float)Height.Value / 2 );
		//		//			pVertex->Normal = normals[ n ];
		//		//			pVertex->Tangent = tangents[ n ];
		//		//			pVertex->Color = new ColorValue( 1, 1, 1, 1 );
		//		//			pVertex->TexCoord0 = texCoords[ n ];

		//		//			pVertex++;
		//		//		}
		//		//	}
		//		//}

		//		geometry.Name = "Mesh Geometry";
		//		geometry.VertexStructure = vertexStructure;
		//		geometry.Vertices = vertices;
		//		geometry.Indices = indices;
		//	}

		//	//create a material
		//	var material = geometry.CreateComponent<Material>();
		//	{
		//		//!!!!make shader graph?
		//		//material.NewObjectSetDefaultConfiguration( false );

		//		material.Name = "Material";
		//		material.BaseColor = GenerateMeshColor;

		//		geometry.Material = ReferenceUtility.MakeThisReference( geometry, material );
		//	}

		//	//create a skeleton
		//	var skeleton = (Skeleton)sourceSkeleton.Clone();
		//	{
		//		skeleton.Name = "Skeleton";

		//		//apply scale
		//		var scale = Height / sourceHeight;
		//		foreach( var bone in skeleton.GetComponents<SkeletonBone>( checkChildren: true ) )
		//		{
		//			var tr = bone.Transform.Value;
		//			var tr2 = tr.UpdatePosition( tr.Position * scale );
		//			bone.Transform = tr2;
		//		}

		//		mesh.AddComponent( skeleton );

		//		mesh.Skeleton = ReferenceUtility.MakeThisReference( mesh, skeleton );
		//	}

		//	//set Mesh property
		//	Mesh = ReferenceUtility.MakeThisReference( this, mesh );
		//}
	}
}
