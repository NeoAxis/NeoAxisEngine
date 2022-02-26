// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A basis class for making weapons.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Weapon", -6000 )]
	[NewObjectDefaultName( "Weapon" )]
	public class Weapon : MeshInSpace, IGameFrameworkItem, InteractiveObject
	{
		bool[] firing = new bool[ 3 ];
		double[] firingCurrentTime = new double[ 3 ];

		//play one animation
		Animation playOneAnimation = null;
		double playOneAnimationSpeed = 1;
		double playOneAnimationRemainingTime;

		/////////////////////////////////////////

		/// <summary>
		/// An animation of the weapon at rest.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Animation> IdleAnimation
		{
			get { if( _idleAnimation.BeginGet() ) IdleAnimation = _idleAnimation.Get( this ); return _idleAnimation.value; }
			set { if( _idleAnimation.BeginSet( ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IdleAnimation"/> property value changes.</summary>
		public event Action<Weapon> IdleAnimationChanged;
		ReferenceField<Animation> _idleAnimation = null;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to enabled a mode 1.
		/// </summary>
		[DefaultValue( true )]
		[DisplayName( "Mode 1 Enabled" )]
		[Category( "Mode 1" )]
		public Reference<bool> Mode1Enabled
		{
			get { if( _mode1Enabled.BeginGet() ) Mode1Enabled = _mode1Enabled.Get( this ); return _mode1Enabled.value; }
			set { if( _mode1Enabled.BeginSet( ref value ) ) { try { Mode1EnabledChanged?.Invoke( this ); } finally { _mode1Enabled.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1Enabled"/> property value changes.</summary>
		public event Action<Weapon> Mode1EnabledChanged;
		ReferenceField<bool> _mode1Enabled = true;

		/// <summary>
		/// Total time of one firing cycle.
		/// </summary>
		[DefaultValue( 0.05 )]
		[DisplayName( "Mode 1 Firing Total Time" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1FiringTotalTime
		{
			get { if( _mode1FiringTotalTime.BeginGet() ) Mode1FiringTotalTime = _mode1FiringTotalTime.Get( this ); return _mode1FiringTotalTime.value; }
			set { if( _mode1FiringTotalTime.BeginSet( ref value ) ) { try { Mode1FiringTotalTimeChanged?.Invoke( this ); } finally { _mode1FiringTotalTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FiringTotalTime"/> property value changes.</summary>
		public event Action<Weapon> Mode1FiringTotalTimeChanged;
		ReferenceField<double> _mode1FiringTotalTime = 0.05;

		/// <summary>
		/// The time of the bullet creation during firing cycle.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Mode 1 Firing Fire Time" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1FiringFireTime
		{
			get { if( _mode1FiringFireTime.BeginGet() ) Mode1FiringFireTime = _mode1FiringFireTime.Get( this ); return _mode1FiringFireTime.value; }
			set { if( _mode1FiringFireTime.BeginSet( ref value ) ) { try { Mode1FiringFireTimeChanged?.Invoke( this ); } finally { _mode1FiringFireTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FiringFireTime"/> property value changes.</summary>
		public event Action<Weapon> Mode1FiringFireTimeChanged;
		ReferenceField<double> _mode1FiringFireTime = 0.0;

		/// <summary>
		/// The type of the bullet.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons\Default\Bullet 1\Bullet 1.objectInSpace" )]
		[DisplayName( "Mode 1 Bullet Type" )]
		[Category( "Mode 1" )]
		public Reference<Metadata.TypeInfo> Mode1BulletType
		{
			get { if( _mode1BulletType.BeginGet() ) Mode1BulletType = _mode1BulletType.Get( this ); return _mode1BulletType.value; }
			set { if( _mode1BulletType.BeginSet( ref value ) ) { try { Mode1BulletTypeChanged?.Invoke( this ); } finally { _mode1BulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1BulletType"/> property value changes.</summary>
		public event Action<Weapon> Mode1BulletTypeChanged;
		ReferenceField<Metadata.TypeInfo> _mode1BulletType = new Reference<Metadata.TypeInfo>( null, @"Content\Weapons\Default\Bullet 1\Bullet 1.objectInSpace" );

		/// <summary>
		/// The initial position, rotation and scale of the bullet.
		/// </summary>
		[DefaultValue( "0.5 0 0; 0 0 0 1; 1 1 1" )]
		[DisplayName( "Mode 1 Bullet Transform" )]
		[Category( "Mode 1" )]
		public Reference<Transform> Mode1BulletTransform
		{
			get { if( _mode1BulletTransform.BeginGet() ) Mode1BulletTransform = _mode1BulletTransform.Get( this ); return _mode1BulletTransform.value; }
			set
			{
				//fix invalid value
				if( value.Value == null )
					value = new Reference<Transform>( NeoAxis.Transform.Identity, value.GetByReference );
				if( _mode1BulletTransform.BeginSet( ref value ) ) { try { Mode1BulletTransformChanged?.Invoke( this ); } finally { _mode1BulletTransform.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="Mode1BulletTransform"/> property value changes.</summary>
		public event Action<Weapon> Mode1BulletTransformChanged;
		ReferenceField<Transform> _mode1BulletTransform = new Transform( new Vector3( 0.5, 0, 0 ), Quaternion.Identity, Vector3.One );

		/// <summary>
		/// The initial speed of the bullet.
		/// </summary>
		[DefaultValue( 40.0 )]
		[DisplayName( "Mode 1 Bullet Speed" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1BulletSpeed
		{
			get { if( _mode1BulletSpeed.BeginGet() ) Mode1BulletSpeed = _mode1BulletSpeed.Get( this ); return _mode1BulletSpeed.value; }
			set { if( _mode1BulletSpeed.BeginSet( ref value ) ) { try { Mode1BulletSpeedChanged?.Invoke( this ); } finally { _mode1BulletSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1BulletSpeed"/> property value changes.</summary>
		public event Action<Weapon> Mode1BulletSpeedChanged;
		ReferenceField<double> _mode1BulletSpeed = 40.0;

		/// <summary>
		/// The maximal dispersion angle of the bullet when fired.
		/// </summary>
		[DefaultValue( 2 )]
		[DisplayName( "Mode 1 Bullet Dispersion Angle" )]
		[Category( "Mode 1" )]
		public Reference<Degree> Mode1BulletDispersionAngle
		{
			get { if( _mode1BulletDispersionAngle.BeginGet() ) Mode1BulletDispersionAngle = _mode1BulletDispersionAngle.Get( this ); return _mode1BulletDispersionAngle.value; }
			set { if( _mode1BulletDispersionAngle.BeginSet( ref value ) ) { try { Mode1BulletDispersionAngleChanged?.Invoke( this ); } finally { _mode1BulletDispersionAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1BulletDispersionAngle"/> property value changes.</summary>
		public event Action<Weapon> Mode1BulletDispersionAngleChanged;
		ReferenceField<Degree> _mode1BulletDispersionAngle = new Degree( 2 );

		/// <summary>
		/// The number of bullets when fired.
		/// </summary>
		[DefaultValue( 1 )]
		[DisplayName( "Mode 1 Bullet Count" )]
		[Category( "Mode 1" )]
		public Reference<int> Mode1BulletCount
		{
			get { if( _mode1BulletCount.BeginGet() ) Mode1BulletCount = _mode1BulletCount.Get( this ); return _mode1BulletCount.value; }
			set { if( _mode1BulletCount.BeginSet( ref value ) ) { try { Mode1BulletCountChanged?.Invoke( this ); } finally { _mode1BulletCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1BulletCount"/> property value changes.</summary>
		public event Action<Weapon> Mode1BulletCountChanged;
		ReferenceField<int> _mode1BulletCount = 1;

		/// <summary>
		/// An animation of the weapon at fire.
		/// </summary>
		[DefaultValue( null )]
		[DisplayName( "Mode 1 Fire Animation" )]
		[Category( "Mode 1" )]
		public Reference<Animation> Mode1FireAnimation
		{
			get { if( _mode1FireAnimation.BeginGet() ) Mode1FireAnimation = _mode1FireAnimation.Get( this ); return _mode1FireAnimation.value; }
			set { if( _mode1FireAnimation.BeginSet( ref value ) ) { try { Mode1FireAnimationChanged?.Invoke( this ); } finally { _mode1FireAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FireAnimation"/> property value changes.</summary>
		public event Action<Weapon> Mode1FireAnimationChanged;
		ReferenceField<Animation> _mode1FireAnimation = null;

		/// <summary>
		/// The multiplier for playing the fire animation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DisplayName( "Mode 1 Fire Animation Speed" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1FireAnimationSpeed
		{
			get { if( _mode1FireAnimationSpeed.BeginGet() ) Mode1FireAnimationSpeed = _mode1FireAnimationSpeed.Get( this ); return _mode1FireAnimationSpeed.value; }
			set { if( _mode1FireAnimationSpeed.BeginSet( ref value ) ) { try { Mode1FireAnimationSpeedChanged?.Invoke( this ); } finally { _mode1FireAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FireAnimationSpeed"/> property value changes.</summary>
		public event Action<Weapon> Mode1FireAnimationSpeedChanged;
		ReferenceField<double> _mode1FireAnimationSpeed = 1.0;

		/// <summary>
		/// The sound that is played when the firing begins.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons\Default\Fire 1.wav" )]
		[DisplayName( "Mode 1 Firing Begin Sound" )]
		[Category( "Mode 1" )]
		public Reference<Sound> Mode1FiringBeginSound
		{
			get { if( _mode1FiringBeginSound.BeginGet() ) Mode1FiringBeginSound = _mode1FiringBeginSound.Get( this ); return _mode1FiringBeginSound.value; }
			set { if( _mode1FiringBeginSound.BeginSet( ref value ) ) { try { Mode1FiringBeginSoundChanged?.Invoke( this ); } finally { _mode1FiringBeginSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FiringBeginSound"/> property value changes.</summary>
		public event Action<Weapon> Mode1FiringBeginSoundChanged;
		ReferenceField<Sound> _mode1FiringBeginSound = new Reference<Sound>( null, @"Content\Weapons\Default\Fire 1.wav" );

		/// <summary>
		///The sound that is played when a shot occurs.
		/// </summary>
		[DefaultValue( null )]
		[DisplayName( "Mode 1 Fire Sound" )]
		[Category( "Mode 1" )]
		public Reference<Sound> Mode1FireSound
		{
			get { if( _mode1FireSound.BeginGet() ) Mode1FireSound = _mode1FireSound.Get( this ); return _mode1FireSound.value; }
			set { if( _mode1FireSound.BeginSet( ref value ) ) { try { Mode1FireSoundChanged?.Invoke( this ); } finally { _mode1FireSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FireSound"/> property value changes.</summary>
		public event Action<Weapon> Mode1FireSoundChanged;
		ReferenceField<Sound> _mode1FireSound = null;

		///// <summary>
		/////The sound that is played when the firing ends.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Mode 1 Firing End Sound" )]
		//[Category( "Mode 1" )]
		//public Reference<Sound> Mode1FiringEndSound
		//{
		//	get { if( _mode1FiringEndSound.BeginGet() ) Mode1FiringEndSound = _mode1FiringEndSound.Get( this ); return _mode1FiringEndSound.value; }
		//	set { if( _mode1FiringEndSound.BeginSet( ref value ) ) { try { Mode1FiringEndSoundChanged?.Invoke( this ); } finally { _mode1FiringEndSound.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Mode1FiringEndSound"/> property value changes.</summary>
		//public event Action<Weapon> Mode1FiringEndSoundChanged;
		//ReferenceField<Sound> _mode1FiringEndSound = null;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to enabled a mode 2.
		/// </summary>
		[DefaultValue( true )]
		[DisplayName( "Mode 2 Enabled" )]
		[Category( "Mode 2" )]
		public Reference<bool> Mode2Enabled
		{
			get { if( _mode2Enabled.BeginGet() ) Mode2Enabled = _mode2Enabled.Get( this ); return _mode2Enabled.value; }
			set { if( _mode2Enabled.BeginSet( ref value ) ) { try { Mode2EnabledChanged?.Invoke( this ); } finally { _mode2Enabled.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2Enabled"/> property value changes.</summary>
		public event Action<Weapon> Mode2EnabledChanged;
		ReferenceField<bool> _mode2Enabled = true;

		/// <summary>
		/// Total time of one firing cycle.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "Mode 2 Firing Total Time" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2FiringTotalTime
		{
			get { if( _mode2FiringTotalTime.BeginGet() ) Mode2FiringTotalTime = _mode2FiringTotalTime.Get( this ); return _mode2FiringTotalTime.value; }
			set { if( _mode2FiringTotalTime.BeginSet( ref value ) ) { try { Mode2FiringTotalTimeChanged?.Invoke( this ); } finally { _mode2FiringTotalTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FiringTotalTime"/> property value changes.</summary>
		public event Action<Weapon> Mode2FiringTotalTimeChanged;
		ReferenceField<double> _mode2FiringTotalTime = 1.0;

		/// <summary>
		/// The time of the bullet creation during firing cycle.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Mode 2 Firing Fire Time" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2FiringFireTime
		{
			get { if( _mode2FiringFireTime.BeginGet() ) Mode2FiringFireTime = _mode2FiringFireTime.Get( this ); return _mode2FiringFireTime.value; }
			set { if( _mode2FiringFireTime.BeginSet( ref value ) ) { try { Mode2FiringFireTimeChanged?.Invoke( this ); } finally { _mode2FiringFireTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FiringFireTime"/> property value changes.</summary>
		public event Action<Weapon> Mode2FiringFireTimeChanged;
		ReferenceField<double> _mode2FiringFireTime = 0.0;

		/// <summary>
		/// The type of the bullet.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons\Default\Bullet 2\Bullet 2.objectInSpace" )]
		[DisplayName( "Mode 2 Bullet Type" )]
		[Category( "Mode 2" )]
		public Reference<Metadata.TypeInfo> Mode2BulletType
		{
			get { if( _mode2BulletType.BeginGet() ) Mode2BulletType = _mode2BulletType.Get( this ); return _mode2BulletType.value; }
			set { if( _mode2BulletType.BeginSet( ref value ) ) { try { Mode2BulletTypeChanged?.Invoke( this ); } finally { _mode2BulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2BulletType"/> property value changes.</summary>
		public event Action<Weapon> Mode2BulletTypeChanged;
		ReferenceField<Metadata.TypeInfo> _mode2BulletType = new Reference<Metadata.TypeInfo>( null, @"Content\Weapons\Default\Bullet 2\Bullet 2.objectInSpace" );

		/// <summary>
		/// The initial position, rotation and scale of the bullet.
		/// </summary>
		[DefaultValue( "0.5 0 0; 0 0 0 1; 1 1 1" )]//[DefaultValue( "0.5 0 0; 0 -0.0871557427476582 0 0.996194698091746; 1 1 1" )]
		[DisplayName( "Mode 2 Bullet Transform" )]
		[Category( "Mode 2" )]
		public Reference<Transform> Mode2BulletTransform
		{
			get { if( _mode2BulletTransform.BeginGet() ) Mode2BulletTransform = _mode2BulletTransform.Get( this ); return _mode2BulletTransform.value; }
			set
			{
				//fix invalid value
				if( value.Value == null )
					value = new Reference<Transform>( NeoAxis.Transform.Identity, value.GetByReference );
				if( _mode2BulletTransform.BeginSet( ref value ) ) { try { Mode2BulletTransformChanged?.Invoke( this ); } finally { _mode2BulletTransform.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="Mode2BulletTransform"/> property value changes.</summary>
		public event Action<Weapon> Mode2BulletTransformChanged;
		ReferenceField<Transform> _mode2BulletTransform = new Transform( new Vector3( 0.5, 0, 0 ), Quaternion.Identity, Vector3.One );//new Transform( new Vector3( 0.5, 0, 0 ), new Quaternion( 0, -0.0871557427476582, 0, 0.996194698091746 ), Vector3.One );

		/// <summary>
		/// The initial speed of the bullet.
		/// </summary>
		[DefaultValue( 20.0 )]
		[DisplayName( "Mode 2 Bullet Speed" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2BulletSpeed
		{
			get { if( _mode2BulletSpeed.BeginGet() ) Mode2BulletSpeed = _mode2BulletSpeed.Get( this ); return _mode2BulletSpeed.value; }
			set { if( _mode2BulletSpeed.BeginSet( ref value ) ) { try { Mode2BulletSpeedChanged?.Invoke( this ); } finally { _mode2BulletSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2BulletSpeed"/> property value changes.</summary>
		public event Action<Weapon> Mode2BulletSpeedChanged;
		ReferenceField<double> _mode2BulletSpeed = 20.0;

		/// <summary>
		/// The maximal dispersion angle of the bullet when fired.
		/// </summary>
		[DefaultValue( 0 )]
		[DisplayName( "Mode 2 Bullet Dispersion Angle" )]
		[Category( "Mode 2" )]
		public Reference<Degree> Mode2BulletDispersionAngle
		{
			get { if( _mode2BulletDispersionAngle.BeginGet() ) Mode2BulletDispersionAngle = _mode2BulletDispersionAngle.Get( this ); return _mode2BulletDispersionAngle.value; }
			set { if( _mode2BulletDispersionAngle.BeginSet( ref value ) ) { try { Mode2BulletDispersionAngleChanged?.Invoke( this ); } finally { _mode2BulletDispersionAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2BulletDispersionAngle"/> property value changes.</summary>
		public event Action<Weapon> Mode2BulletDispersionAngleChanged;
		ReferenceField<Degree> _mode2BulletDispersionAngle = new Degree( 0 );

		/// <summary>
		/// The number of bullets when fired.
		/// </summary>
		[DefaultValue( 1 )]
		[DisplayName( "Mode 2 Bullet Count" )]
		[Category( "Mode 2" )]
		public Reference<int> Mode2BulletCount
		{
			get { if( _mode2BulletCount.BeginGet() ) Mode2BulletCount = _mode2BulletCount.Get( this ); return _mode2BulletCount.value; }
			set { if( _mode2BulletCount.BeginSet( ref value ) ) { try { Mode2BulletCountChanged?.Invoke( this ); } finally { _mode2BulletCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2BulletCount"/> property value changes.</summary>
		public event Action<Weapon> Mode2BulletCountChanged;
		ReferenceField<int> _mode2BulletCount = 1;

		/// <summary>
		/// An animation of the weapon at fire.
		/// </summary>
		[DefaultValue( null )]
		[DisplayName( "Mode 2 Fire Animation" )]
		[Category( "Mode 2" )]
		public Reference<Animation> Mode2FireAnimation
		{
			get { if( _mode2FireAnimation.BeginGet() ) Mode2FireAnimation = _mode2FireAnimation.Get( this ); return _mode2FireAnimation.value; }
			set { if( _mode2FireAnimation.BeginSet( ref value ) ) { try { Mode2FireAnimationChanged?.Invoke( this ); } finally { _mode2FireAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FireAnimation"/> property value changes.</summary>
		public event Action<Weapon> Mode2FireAnimationChanged;
		ReferenceField<Animation> _mode2FireAnimation = null;

		/// <summary>
		/// The multiplier for playing the fire animation.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DisplayName( "Mode 2 Fire Animation Speed" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2FireAnimationSpeed
		{
			get { if( _mode2FireAnimationSpeed.BeginGet() ) Mode2FireAnimationSpeed = _mode2FireAnimationSpeed.Get( this ); return _mode2FireAnimationSpeed.value; }
			set { if( _mode2FireAnimationSpeed.BeginSet( ref value ) ) { try { Mode2FireAnimationSpeedChanged?.Invoke( this ); } finally { _mode2FireAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FireAnimationSpeed"/> property value changes.</summary>
		public event Action<Weapon> Mode2FireAnimationSpeedChanged;
		ReferenceField<double> _mode2FireAnimationSpeed = 1.0;

		/// <summary>
		/// The sound that is played when the firing begins.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons\Default\Fire 2.wav" )]
		[DisplayName( "Mode 2 Firing Begin Sound" )]
		[Category( "Mode 2" )]
		public Reference<Sound> Mode2FiringBeginSound
		{
			get { if( _mode2FiringBeginSound.BeginGet() ) Mode2FiringBeginSound = _mode2FiringBeginSound.Get( this ); return _mode2FiringBeginSound.value; }
			set { if( _mode2FiringBeginSound.BeginSet( ref value ) ) { try { Mode2FiringBeginSoundChanged?.Invoke( this ); } finally { _mode2FiringBeginSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FiringBeginSound"/> property value changes.</summary>
		public event Action<Weapon> Mode2FiringBeginSoundChanged;
		ReferenceField<Sound> _mode2FiringBeginSound = new Reference<Sound>( null, @"Content\Weapons\Default\Fire 2.wav" );

		/// <summary>
		///The sound that is played when a shot occurs.
		/// </summary>
		[DefaultValue( null )]
		[DisplayName( "Mode 2 Fire Sound" )]
		[Category( "Mode 2" )]
		public Reference<Sound> Mode2FireSound
		{
			get { if( _mode2FireSound.BeginGet() ) Mode2FireSound = _mode2FireSound.Get( this ); return _mode2FireSound.value; }
			set { if( _mode2FireSound.BeginSet( ref value ) ) { try { Mode2FireSoundChanged?.Invoke( this ); } finally { _mode2FireSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FireSound"/> property value changes.</summary>
		public event Action<Weapon> Mode2FireSoundChanged;
		ReferenceField<Sound> _mode2FireSound = null;

		///// <summary>
		/////The sound that is played when the firing ends.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Mode 2 Firing End Sound" )]
		//[Category( "Mode 2" )]
		//public Reference<Sound> Mode2FiringEndSound
		//{
		//	get { if( _mode2FiringEndSound.BeginGet() ) Mode2FiringEndSound = _mode2FiringEndSound.Get( this ); return _mode2FiringEndSound.value; }
		//	set { if( _mode2FiringEndSound.BeginSet( ref value ) ) { try { Mode2FiringEndSoundChanged?.Invoke( this ); } finally { _mode2FiringEndSound.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Mode2FiringEndSound"/> property value changes.</summary>
		//public event Action<Weapon> Mode2FiringEndSoundChanged;
		//ReferenceField<Sound> _mode2FiringEndSound = null;

		/////////////////////////////////////////

		/// <summary>
		/// The place of holding with the left hand. X - forward, -Z - palm. Set all zeros to disable the left hand.
		/// </summary>
		[DefaultValue( "0 0 0; 0 0 0 0; 0 0 0" )]//[DefaultValue( "0.2 0 0; 0 0 0 1; 1 1 1" )]
		public Reference<Transform> LeftHandTransform
		{
			get { if( _leftHandTransform.BeginGet() ) LeftHandTransform = _leftHandTransform.Get( this ); return _leftHandTransform.value; }
			set { if( _leftHandTransform.BeginSet( ref value ) ) { try { LeftHandTransformChanged?.Invoke( this ); } finally { _leftHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandTransform"/> property value changes.</summary>
		public event Action<Weapon> LeftHandTransformChanged;
		ReferenceField<Transform> _leftHandTransform = new Transform( Vector3.Zero, Quaternion.Zero, Vector3.Zero ); //new Transform( new Vector3( 0.2, 0, 0 ), new Quaternion( 0, 0, 0, 1 ), Vector3.One );

		/// <summary>
		/// The place of holding with the right hand. X - forward, -Z - palm. Set all zeros to disable the right hand.
		/// </summary>
		[ DefaultValue( "-0.2 0 -0.1; -0.707106781186548 0 0 0.707106781186547; 1 1 1" )]
		public Reference<Transform> RightHandTransform
		{
			get { if( _rightHandTransform.BeginGet() ) RightHandTransform = _rightHandTransform.Get( this ); return _rightHandTransform.value; }
			set { if( _rightHandTransform.BeginSet( ref value ) ) { try { RightHandTransformChanged?.Invoke( this ); } finally { _rightHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandTransform"/> property value changes.</summary>
		public event Action<Weapon> RightHandTransformChanged;
		ReferenceField<Transform> _rightHandTransform = new Transform( new Vector3( -0.2, 0, -0.1 ), new Quaternion( -0.707106781186548, 0, 0, 0.707106781186547 ), Vector3.One );

		/// <summary>
		/// The transform offset of the weapon relative to the owner.
		/// </summary>
		[DefaultValue( "0.2 -0.15 0.1" )]
		public Reference<Vector3> PositionOffsetWhenAttached
		{
			get { if( _positionOffsetWhenAttached.BeginGet() ) PositionOffsetWhenAttached = _positionOffsetWhenAttached.Get( this ); return _positionOffsetWhenAttached.value; }
			set { if( _positionOffsetWhenAttached.BeginSet( ref value ) ) { try { PositionOffsetWhenAttachedChanged?.Invoke( this ); } finally { _positionOffsetWhenAttached.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionOffsetWhenAttached"/> property value changes.</summary>
		public event Action<Weapon> PositionOffsetWhenAttachedChanged;
		ReferenceField<Vector3> _positionOffsetWhenAttached = new Vector3( 0.2, -0.15, 0.1 );

		/// <summary>
		/// The additional vertical degree for the weapon to the owner eyes looking vector.
		/// </summary>
		[DefaultValue( 5.0 )]
		public Reference<Degree> RotationOffsetWhenAttached
		{
			get { if( _rotationOffsetWhenAttached.BeginGet() ) RotationOffsetWhenAttached = _rotationOffsetWhenAttached.Get( this ); return _rotationOffsetWhenAttached.value; }
			set { if( _rotationOffsetWhenAttached.BeginSet( ref value ) ) { try { RotationOffsetWhenAttachedChanged?.Invoke( this ); } finally { _rotationOffsetWhenAttached.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffsetWhenAttached"/> property value changes.</summary>
		public event Action<Weapon> RotationOffsetWhenAttachedChanged;
		ReferenceField<Degree> _rotationOffsetWhenAttached = new Degree( 5.0 );

		/// <summary>
		/// The transform offset when the weapon is attached to the first person camera.
		/// </summary>
		[DefaultValue( "0 -0.1 -0.2" )]
		public Reference<Vector3> PositionOffsetWhenAttachedFirstPerson
		{
			get { if( _positionOffsetWhenAttachedFirstPerson.BeginGet() ) PositionOffsetWhenAttachedFirstPerson = _positionOffsetWhenAttachedFirstPerson.Get( this ); return _positionOffsetWhenAttachedFirstPerson.value; }
			set { if( _positionOffsetWhenAttachedFirstPerson.BeginSet( ref value ) ) { try { PositionOffsetWhenAttachedFirstPersonChanged?.Invoke( this ); } finally { _positionOffsetWhenAttachedFirstPerson.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionOffsetWhenAttachedFirstPerson"/> property value changes.</summary>
		public event Action<Weapon> PositionOffsetWhenAttachedFirstPersonChanged;
		ReferenceField<Vector3> _positionOffsetWhenAttachedFirstPerson = new Vector3( 0, -0.1, -0.2 );

		/// <summary>
		/// The additional vertical degree when the weapon is attached to the first person camera.
		/// </summary>
		[DefaultValue( 5.0 )]
		public Reference<Degree> RotationOffsetWhenAttachedFirstPerson
		{
			get { if( _rotationOffsetWhenAttachedFirstPerson.BeginGet() ) RotationOffsetWhenAttachedFirstPerson = _rotationOffsetWhenAttachedFirstPerson.Get( this ); return _rotationOffsetWhenAttachedFirstPerson.value; }
			set { if( _rotationOffsetWhenAttachedFirstPerson.BeginSet( ref value ) ) { try { RotationOffsetWhenAttachedFirstPersonChanged?.Invoke( this ); } finally { _rotationOffsetWhenAttachedFirstPerson.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffsetWhenAttachedFirstPerson"/> property value changes.</summary>
		public event Action<Weapon> RotationOffsetWhenAttachedFirstPersonChanged;
		ReferenceField<Degree> _rotationOffsetWhenAttachedFirstPerson = new Degree( 5.0 );

		/// <summary>
		/// Whether to display initial position and direction of a bullet and places of hands.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> AlwaysDisplayAdditionalInfo
		{
			get { if( _alwaysDisplayAdditionalInfo.BeginGet() ) AlwaysDisplayAdditionalInfo = _alwaysDisplayAdditionalInfo.Get( this ); return _alwaysDisplayAdditionalInfo.value; }
			set { if( _alwaysDisplayAdditionalInfo.BeginSet( ref value ) ) { try { AlwaysDisplayAdditionalInfoChanged?.Invoke( this ); } finally { _alwaysDisplayAdditionalInfo.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AlwaysDisplayAdditionalInfo"/> property value changes.</summary>
		public event Action<Weapon> AlwaysDisplayAdditionalInfoChanged;
		ReferenceField<bool> _alwaysDisplayAdditionalInfo = false;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Mode1FiringTotalTime ):
				case nameof( Mode1FiringFireTime ):
				case nameof( Mode1BulletType ):
				case nameof( Mode1BulletTransform ):
				case nameof( Mode1BulletSpeed ):
				case nameof( Mode1BulletDispersionAngle ):
				case nameof( Mode1BulletCount ):
				case nameof( Mode1FireAnimation ):
				case nameof( Mode1FireAnimationSpeed ):
				case nameof( Mode1FiringBeginSound ):
				case nameof( Mode1FireSound ):
					//case nameof( Mode1FiringEndSound ):
					if( !Mode1Enabled )
						skip = true;
					break;

				case nameof( Mode2FiringTotalTime ):
				case nameof( Mode2FiringFireTime ):
				case nameof( Mode2BulletType ):
				case nameof( Mode2BulletTransform ):
				case nameof( Mode2BulletSpeed ):
				case nameof( Mode2BulletDispersionAngle ):
				case nameof( Mode2BulletCount ):
				case nameof( Mode2FireAnimation ):
				case nameof( Mode2FireAnimationSpeed ):
				case nameof( Mode2FiringBeginSound ):
				case nameof( Mode2FireSound ):
					//case nameof( Mode2FiringEndSound ):
					if( !Mode2Enabled )
						skip = true;
					break;
				}
			}
		}

		//

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
					TickAnimate( 0.001f );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			TickAnimate( delta );
		}

		public bool IsModeEnabled( int mode )
		{
			return mode == 1 ? Mode1Enabled.Value : Mode2Enabled.Value;
		}

		public bool IsFiring( int mode )
		{
			return firing[ mode - 1 ];
		}

		public double GetFiringCurrentTime( int mode )
		{
			return firingCurrentTime[ mode - 1 ];
		}

		//

		protected virtual void OnCanFire( int mode, ref bool canFire )
		{
			if( ( mode == 1 ? Mode1BulletType.Value : Mode2BulletType.Value ) == null )
				canFire = false;
		}

		public delegate void CanFireDelegate( Weapon sender, int mode, ref bool canFire );
		public event CanFireDelegate CanFire;

		public bool PerformCanFire( int mode )
		{
			if( !IsModeEnabled( mode ) )
				return false;

			var canFire = true;
			OnCanFire( mode, ref canFire );
			CanFire?.Invoke( this, mode, ref canFire );
			return canFire;
		}

		//

		protected virtual void OnFire( int mode )
		{
			var bulletType = mode == 1 ? Mode1BulletType.Value : Mode2BulletType.Value;
			if( bulletType != null )
			{
				var bulletCount = mode == 1 ? Mode1BulletCount.Value : Mode2BulletCount.Value;
				var bulletSpeed = mode == 1 ? Mode1BulletSpeed.Value : Mode2BulletSpeed.Value;
				var dispersionAngle = mode == 1 ? Mode1BulletDispersionAngle.Value : Mode2BulletDispersionAngle.Value;

				FastRandom random = null;

				for( int nCount = 0; nCount < bulletCount; nCount++ )
				{
					var component = Parent.CreateComponent( bulletType, enabled: false );

					var objectInSpace = component as ObjectInSpace;
					if( objectInSpace != null )
					{
						//creation parameters
						var transform = GetBulletWorldInitialTransform( mode );

						//dispersion angle
						if( dispersionAngle != new Degree( 0 ) )
						{
							if( random == null )
								random = new FastRandom();

							Matrix3F.FromRotateByX( random.NextFloat() * MathEx.PI * 2, out var matrix2 );
							Matrix3F.FromRotateByZ( random.NextFloat() * (float)dispersionAngle.InRadians(), out var matrix3 );
							Matrix3F.Multiply( ref matrix2, ref matrix3, out var matrix );
							matrix.ToQuaternion( out var rot );

							transform = transform.UpdateRotation( transform.Rotation * rot );
						}

						var velocity = transform.Rotation.GetForward() * bulletSpeed;

						//configure transform and velocity
						var collisionBody = objectInSpace.GetComponent<RigidBody>();
						if( collisionBody != null )
						{
							collisionBody.Transform = transform;
							collisionBody.LinearVelocity = velocity;
						}
						else
							objectInSpace.Transform = transform;

						//configure Bullet component
						var bullet = objectInSpace as Bullet;
						if( bullet != null )
						{
							//make a reference path from the root component (scene)
							bullet.OriginalCreator = ReferenceUtility.MakeRootReference( this );
							bullet.SetLinearVelocity( velocity );
						}
					}

					component.Enabled = true;
				}
			}
		}

		public delegate void FireDelegate( Weapon sender, int mode );
		public event FireDelegate Fire;

		public bool PerformFire( int mode )
		{
			if( !PerformCanFire( mode ) )
				return false;

			SoundPlay( mode == 1 ? Mode1FireSound : Mode2FireSound );
			OnFire( mode );
			Fire?.Invoke( this, mode );

			return true;
		}

		//

		public delegate void FiringBeginEndDelegate( Weapon sender, int mode );
		public event FiringBeginEndDelegate FiringBeginEvent;
		public event FiringBeginEndDelegate FiringEndEvent;

		public bool FiringBegin( int mode )
		{
			if( IsFiring( mode ) )
				return false;
			if( !PerformCanFire( mode ) )
				return false;

			firing[ mode - 1 ] = true;
			firingCurrentTime[ mode - 1 ] = 0;
			StartPlayOneAnimation( mode == 1 ? Mode1FireAnimation : Mode2FireAnimation, mode == 1 ? Mode1FireAnimationSpeed : Mode2FireAnimationSpeed );
			SoundPlay( mode == 1 ? Mode1FiringBeginSound : Mode2FiringBeginSound );
			FiringBeginEvent?.Invoke( this, mode );

			return true;
		}

		public void FiringEnd( int mode )
		{
			if( !IsFiring( mode ) )
				return;

			firing[ mode - 1 ] = false;
			firingCurrentTime[ mode - 1 ] = 0;
			//SoundPlay( mode == 1 ? Mode1FiringEndSound : Mode2FiringEndSound );
			FiringEndEvent?.Invoke( this, mode );
		}

		//

		void Simulate( float delta )
		{
			for( int mode = 1; mode <= 2; mode++ )
			{
				if( IsFiring( mode ) )
				{
					var firingFireTime = mode == 1 ? Mode1FiringFireTime : Mode2FiringFireTime;
					var firingTotalTime = mode == 1 ? Mode1FiringTotalTime : Mode2FiringTotalTime;

					var before = GetFiringCurrentTime( mode ) < firingFireTime || ( GetFiringCurrentTime( mode ) == 0 && firingFireTime == 0 );
					firingCurrentTime[ mode - 1 ] += delta;
					var after = GetFiringCurrentTime( mode ) < firingFireTime;

					if( before != after )
						PerformFire( mode );

					if( GetFiringCurrentTime( mode ) >= firingTotalTime )
						FiringEnd( mode );
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			Simulate( Time.SimulationDelta );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			if( Components.Count == 0 && !Mesh.ReferenceSpecified )
			{
				//Animation controller
				{
					var controller = CreateComponent<MeshInSpaceAnimationController>();
					controller.Name = "Mesh In Space Animation Controller";
					controller.InterpolationTime = 0.05;
				}

				//Barrel mesh
				Mesh = ReferenceUtility.MakeReference( @"Content\Weapons\Default\Barrel.mesh" );

				//Stock mesh
				{
					var meshInSpace = CreateComponent<MeshInSpace>();
					meshInSpace.Name = "Stock";
					meshInSpace.CanBeSelected = false;
					meshInSpace.Mesh = ReferenceUtility.MakeReference( @"Content\Weapons\Default\Stock.mesh" );

					var transformOffset = meshInSpace.CreateComponent<TransformOffset>();
					transformOffset.Name = "Attach Transform Offset";
					transformOffset.PositionOffset = new Vector3( -0.22, 0, -0.1 );
					transformOffset.Source = ReferenceUtility.MakeThisReference( transformOffset, this, "Transform" );

					meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, transformOffset, "Result" );
				}
			}
		}

		public void SoundPlay( Sound sound )
		{
			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		protected override bool OnSpaceBoundsUpdateIncludeChildren()
		{
			return true;
		}

		public void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			//take by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character;
			if( character != null && character.ItemGetEnabledFirst() == null )
			{
				info = new InteractiveObjectObjectInfo();
				info.AllowInteract = true;//CanTake;
				info.SelectionTextInfo.Add( Name );
				info.SelectionTextInfo.Add( "Click mouse button to take. Press T to drop." );
			}
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					//take by a character
					var character = gameMode.ObjectControlledByPlayer.Value as Character;
					if( character != null && character.ItemGetEnabledFirst() == null )
					{
						character.ItemTake( this );
						character.ItemActivate( this );
						return true;
					}
				}
			}

			return false;
		}

		public void ObjectInteractionEnter( ObjectInteractionContext context )
		{
		}

		public void ObjectInteractionExit( ObjectInteractionContext context )
		{
		}

		public void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
		}

		public delegate void GetBulletWorldInitialTransformEventDelegate( Weapon sender, ref Transform transform );
		public event GetBulletWorldInitialTransformEventDelegate GetBulletWorldInitialTransformEvent;

		public virtual Transform GetBulletWorldInitialTransform( int mode )
		{
			var transform = mode == 1 ? Mode1BulletTransform.Value : Mode2BulletTransform.Value;

			var result = Transform.Value * transform;
			GetBulletWorldInitialTransformEvent?.Invoke( this, ref result );
			return result;
		}

		Transform GetHandTransform( HandEnum hand )
		{
			return hand == HandEnum.Left ? LeftHandTransform.Value : RightHandTransform.Value;
		}

		public delegate void GetHandWorldTransformEventDelegate( Weapon sender, HandEnum hand, ref Transform transform );
		public event GetHandWorldTransformEventDelegate GetHandWorldTransformEvent;

		public virtual Transform GetHandWorldTransform( HandEnum hand )
		{
			var transform = GetHandTransform( hand );

			var result = Transform.Value * transform;
			GetHandWorldTransformEvent?.Invoke( this, hand, ref result );
			return result;
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				bool show = ( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this ) || AlwaysDisplayAdditionalInfo;
				if( show )
				{
					var renderer = context.Owner.Simple3DRenderer;
					if( renderer != null )
					{
						for( int weaponMode = 1; weaponMode <= 2; weaponMode++ )
						{
							if( IsModeEnabled( weaponMode ) )
							{
								var tr = GetBulletWorldInitialTransform( weaponMode );
								var ray = new Ray( tr.Position, tr.Rotation.GetForward() * 0.1 );

								renderer.SetColor( new ColorValue( 1, 0, 0 ) );
								renderer.AddArrow( ray.Origin, ray.GetEndPoint() );
							}
						}

						for( var hand = HandEnum.Left; hand <= HandEnum.Right; hand++ )
						{
							var tr = GetHandWorldTransform( hand );
							if( tr != NeoAxis.Transform.Zero )
							{
								{
									var ray = new Ray( tr.Position, tr.Rotation.GetForward() * 0.1 );
									renderer.SetColor( new ColorValue( 1, 0, 0 ) );
									renderer.AddArrow( ray.Origin, ray.GetEndPoint() );
								}

								{
									var ray = new Ray( tr.Position, tr.Rotation.GetUp() * -0.1 );
									renderer.SetColor( new ColorValue( 0, 0, 1 ) );
									renderer.AddArrow( ray.Origin, ray.GetEndPoint() );
								}
							}
						}
					}
				}
			}
		}

		public MeshInSpaceAnimationController GetAnimationController()
		{
			return GetComponent<MeshInSpaceAnimationController>( onlyEnabledInHierarchy: true );
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

			var controller = GetAnimationController();
			if( controller != null )
			{
				var character = Parent as Character;
				if( character != null )
				{
					Animation animation = null;
					double speed = 1;
					bool autoRewind = true;

					if( animation == null )
						animation = IdleAnimation;

					//play one animation
					if( playOneAnimation != null )
					{
						animation = playOneAnimation;
						speed = playOneAnimationSpeed;
						autoRewind = false;
					}

					var state = new MeshInSpaceAnimationController.AnimationStateClass();
					state.Animations.Add( new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = animation, Speed = speed, AutoRewind = autoRewind } );

					//update controller
					controller.SetAnimationState( state, true );
				}
				else
					controller.SetAnimationState( null, false );
			}
		}

		public void StartPlayOneAnimation( Animation animation, double speed = 1.0, bool resetSameAnimation = false )
		{
			if( !resetSameAnimation && animation != null && playOneAnimation == animation && playOneAnimationSpeed == speed )
				return;

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

	}

}
