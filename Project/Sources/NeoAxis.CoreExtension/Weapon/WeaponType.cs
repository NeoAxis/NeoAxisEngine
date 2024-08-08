// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the weapon type.
	/// </summary>
	[ResourceFileExtension( "weapontype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Weapon\Weapon Type", 22100 )]
	[EditorControl( typeof( WeaponTypeEditor ) )]
	[Preview( typeof( WeaponTypePreview ) )]
	[PreviewImage( typeof( WeaponTypePreviewImage ) )]
#endif
	public class WeaponType : Component, ItemTypeInterface
	{
		int version;

		//

		//!!!!use Version
		//DataWasChanged()

		public enum WayToUseEnum
		{
			Rifle,
			OneHandedMelee,
		}

		/// <summary>
		/// How to use the weapon.
		/// </summary>
		[DefaultValue( WayToUseEnum.Rifle )]
		public Reference<WayToUseEnum> WayToUse
		{
			get { if( _wayToUse.BeginGet() ) WayToUse = _wayToUse.Get( this ); return _wayToUse.value; }
			set { if( _wayToUse.BeginSet( this, ref value ) ) { try { WayToUseChanged?.Invoke( this ); } finally { _wayToUse.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WayToUse"/> property value changes.</summary>
		public event Action<WeaponType> WayToUseChanged;
		ReferenceField<WayToUseEnum> _wayToUse = WayToUseEnum.Rifle;

		const string meshDefault = @"Content\Weapons\Default\scene.gltf|$Mesh";

		/// <summary>
		/// The mesh of the weapon.
		/// </summary>
		[DefaultValueReference( meshDefault )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<WeaponType> MeshChanged;
		ReferenceField<Mesh> _mesh = new Reference<Mesh>( null, meshDefault );

		/////////////////////////////////////////

		/// <summary>
		/// An animation of the weapon at rest.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Animation> IdleAnimation
		{
			get { if( _idleAnimation.BeginGet() ) IdleAnimation = _idleAnimation.Get( this ); return _idleAnimation.value; }
			set { if( _idleAnimation.BeginSet( this, ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IdleAnimation"/> property value changes.</summary>
		public event Action<WeaponType> IdleAnimationChanged;
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
			set { if( _mode1Enabled.BeginSet( this, ref value ) ) { try { Mode1EnabledChanged?.Invoke( this ); } finally { _mode1Enabled.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1Enabled"/> property value changes.</summary>
		public event Action<WeaponType> Mode1EnabledChanged;
		ReferenceField<bool> _mode1Enabled = true;

		/// <summary>
		/// Total time of one firing cycle.
		/// </summary>
		[DefaultValue( 0.1 )]
		[DisplayName( "Mode 1 Firing Total Time" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1FiringTotalTime
		{
			get { if( _mode1FiringTotalTime.BeginGet() ) Mode1FiringTotalTime = _mode1FiringTotalTime.Get( this ); return _mode1FiringTotalTime.value; }
			set { if( _mode1FiringTotalTime.BeginSet( this, ref value ) ) { try { Mode1FiringTotalTimeChanged?.Invoke( this ); } finally { _mode1FiringTotalTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FiringTotalTime"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FiringTotalTimeChanged;
		ReferenceField<double> _mode1FiringTotalTime = 0.1;

		/// <summary>
		/// The time of the bullet creation during firing cycle.
		/// </summary>
		[DefaultValue( 0.0 )]
		[DisplayName( "Mode 1 Firing Fire Time" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1FiringFireTime
		{
			get { if( _mode1FiringFireTime.BeginGet() ) Mode1FiringFireTime = _mode1FiringFireTime.Get( this ); return _mode1FiringFireTime.value; }
			set { if( _mode1FiringFireTime.BeginSet( this, ref value ) ) { try { Mode1FiringFireTimeChanged?.Invoke( this ); } finally { _mode1FiringFireTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FiringFireTime"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FiringFireTimeChanged;
		ReferenceField<double> _mode1FiringFireTime = 0.0;

		/// <summary>
		/// The type of the bullet.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons\Default\Bullet 1\Default Bullet.bullettype" )]
		[DisplayName( "Mode 1 Bullet Type" )]
		[Category( "Mode 1" )]
		public Reference<BulletType> Mode1BulletType
		{
			get { if( _mode1BulletType.BeginGet() ) Mode1BulletType = _mode1BulletType.Get( this ); return _mode1BulletType.value; }
			set { if( _mode1BulletType.BeginSet( this, ref value ) ) { try { Mode1BulletTypeChanged?.Invoke( this ); } finally { _mode1BulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1BulletType"/> property value changes.</summary>
		public event Action<WeaponType> Mode1BulletTypeChanged;
		ReferenceField<BulletType> _mode1BulletType = new Reference<BulletType>( null, @"Content\Weapons\Default\Bullet 1\Default Bullet.bullettype" );

		/// <summary>
		/// The initial position, rotation and scale of the bullet.
		/// </summary>
		[DefaultValue( "0.59 0 0.073; 0 0 0 1; 1 1 1" )]
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
				if( _mode1BulletTransform.BeginSet( this, ref value ) ) { try { Mode1BulletTransformChanged?.Invoke( this ); } finally { _mode1BulletTransform.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="Mode1BulletTransform"/> property value changes.</summary>
		public event Action<WeaponType> Mode1BulletTransformChanged;
		ReferenceField<Transform> _mode1BulletTransform = new Transform( new Vector3( 0.59, 0, 0.073 ), Quaternion.Identity, Vector3.One );

		/// <summary>
		/// The initial speed of the bullet.
		/// </summary>
		[DefaultValue( 930 )]
		[DisplayName( "Mode 1 Bullet Speed" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1BulletSpeed
		{
			get { if( _mode1BulletSpeed.BeginGet() ) Mode1BulletSpeed = _mode1BulletSpeed.Get( this ); return _mode1BulletSpeed.value; }
			set { if( _mode1BulletSpeed.BeginSet( this, ref value ) ) { try { Mode1BulletSpeedChanged?.Invoke( this ); } finally { _mode1BulletSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1BulletSpeed"/> property value changes.</summary>
		public event Action<WeaponType> Mode1BulletSpeedChanged;
		ReferenceField<double> _mode1BulletSpeed = 930;

		/// <summary>
		/// The maximal dispersion angle of the bullet when fired.
		/// </summary>
		[DefaultValue( 1 )]
		[DisplayName( "Mode 1 Bullet Dispersion Angle" )]
		[Category( "Mode 1" )]
		public Reference<Degree> Mode1BulletDispersionAngle
		{
			get { if( _mode1BulletDispersionAngle.BeginGet() ) Mode1BulletDispersionAngle = _mode1BulletDispersionAngle.Get( this ); return _mode1BulletDispersionAngle.value; }
			set { if( _mode1BulletDispersionAngle.BeginSet( this, ref value ) ) { try { Mode1BulletDispersionAngleChanged?.Invoke( this ); } finally { _mode1BulletDispersionAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1BulletDispersionAngle"/> property value changes.</summary>
		public event Action<WeaponType> Mode1BulletDispersionAngleChanged;
		ReferenceField<Degree> _mode1BulletDispersionAngle = new Degree( 1 );

		/// <summary>
		/// The number of bullets when fired.
		/// </summary>
		[DefaultValue( 1 )]
		[DisplayName( "Mode 1 Bullet Count" )]
		[Category( "Mode 1" )]
		public Reference<int> Mode1BulletCount
		{
			get { if( _mode1BulletCount.BeginGet() ) Mode1BulletCount = _mode1BulletCount.Get( this ); return _mode1BulletCount.value; }
			set { if( _mode1BulletCount.BeginSet( this, ref value ) ) { try { Mode1BulletCountChanged?.Invoke( this ); } finally { _mode1BulletCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1BulletCount"/> property value changes.</summary>
		public event Action<WeaponType> Mode1BulletCountChanged;
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
			set { if( _mode1FireAnimation.BeginSet( this, ref value ) ) { try { Mode1FireAnimationChanged?.Invoke( this ); } finally { _mode1FireAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FireAnimation"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FireAnimationChanged;
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
			set { if( _mode1FireAnimationSpeed.BeginSet( this, ref value ) ) { try { Mode1FireAnimationSpeedChanged?.Invoke( this ); } finally { _mode1FireAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FireAnimationSpeed"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FireAnimationSpeedChanged;
		ReferenceField<double> _mode1FireAnimationSpeed = 1.0;

		/// <summary>
		/// The sound that is played when the firing begins.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons\Default\Sounds\Fire 1.ogg" )]
		[DisplayName( "Mode 1 Firing Begin Sound" )]
		[Category( "Mode 1" )]
		public Reference<Sound> Mode1FiringBeginSound
		{
			get { if( _mode1FiringBeginSound.BeginGet() ) Mode1FiringBeginSound = _mode1FiringBeginSound.Get( this ); return _mode1FiringBeginSound.value; }
			set { if( _mode1FiringBeginSound.BeginSet( this, ref value ) ) { try { Mode1FiringBeginSoundChanged?.Invoke( this ); } finally { _mode1FiringBeginSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FiringBeginSound"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FiringBeginSoundChanged;
		ReferenceField<Sound> _mode1FiringBeginSound = new Reference<Sound>( null, @"Content\Weapons\Default\Sounds\Fire 1.ogg" );

		/// <summary>
		///The sound that is played when a shot occurs.
		/// </summary>
		[DefaultValue( null )]
		[DisplayName( "Mode 1 Fire Sound" )]
		[Category( "Mode 1" )]
		public Reference<Sound> Mode1FireSound
		{
			get { if( _mode1FireSound.BeginGet() ) Mode1FireSound = _mode1FireSound.Get( this ); return _mode1FireSound.value; }
			set { if( _mode1FireSound.BeginSet( this, ref value ) ) { try { Mode1FireSoundChanged?.Invoke( this ); } finally { _mode1FireSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FireSound"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FireSoundChanged;
		ReferenceField<Sound> _mode1FireSound = null;

		[DefaultValueReference( @"Content\Weapons\Default\Fire 1 muzzle flash\Fire 1 muzzle flash.particle" )]
		[DisplayName( "Mode 1 Fire Muzzle Flash Particle" )]
		[Category( "Mode 1" )]
		public Reference<ParticleSystem> Mode1FireMuzzleFlashParticle
		{
			get { if( _mode1FireMuzzleFlashParticle.BeginGet() ) Mode1FireMuzzleFlashParticle = _mode1FireMuzzleFlashParticle.Get( this ); return _mode1FireMuzzleFlashParticle.value; }
			set { if( _mode1FireMuzzleFlashParticle.BeginSet( this, ref value ) ) { try { Mode1FireMuzzleFlashParticleChanged?.Invoke( this ); } finally { _mode1FireMuzzleFlashParticle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FireMuzzleFlashParticle"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FireMuzzleFlashParticleChanged;
		ReferenceField<ParticleSystem> _mode1FireMuzzleFlashParticle = new Reference<ParticleSystem>( null, @"Content\Weapons\Default\Fire 1 muzzle flash\Fire 1 muzzle flash.particle" );

		[DefaultValue( 0.05 )]
		[DisplayName( "Mode 1 Max Recoil Offset" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1MaxRecoilOffset
		{
			get { if( _mode1MaxRecoilOffset.BeginGet() ) Mode1MaxRecoilOffset = _mode1MaxRecoilOffset.Get( this ); return _mode1MaxRecoilOffset.value; }
			set { if( _mode1MaxRecoilOffset.BeginSet( this, ref value ) ) { try { Mode1MaxRecoilOffsetChanged?.Invoke( this ); } finally { _mode1MaxRecoilOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1MaxRecoilOffset"/> property value changes.</summary>
		public event Action<WeaponType> Mode1MaxRecoilOffsetChanged;
		ReferenceField<double> _mode1MaxRecoilOffset = 0.05;

		[DefaultValue( 0.03 )]
		[DisplayName( "Mode 1 Max Recoil Offset Time" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1MaxRecoilOffsetTime
		{
			get { if( _mode1MaxRecoilOffsetTime.BeginGet() ) Mode1MaxRecoilOffsetTime = _mode1MaxRecoilOffsetTime.Get( this ); return _mode1MaxRecoilOffsetTime.value; }
			set { if( _mode1MaxRecoilOffsetTime.BeginSet( this, ref value ) ) { try { Mode1MaxRecoilOffsetTimeChanged?.Invoke( this ); } finally { _mode1MaxRecoilOffsetTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1MaxRecoilOffsetTime"/> property value changes.</summary>
		public event Action<WeaponType> Mode1MaxRecoilOffsetTimeChanged;
		ReferenceField<double> _mode1MaxRecoilOffsetTime = 0.03;

		[DefaultValue( 0.0 )]
		[DisplayName( "Mode 1 Recoil Force" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1RecoilForce
		{
			get { if( _mode1RecoilForce.BeginGet() ) Mode1RecoilForce = _mode1RecoilForce.Get( this ); return _mode1RecoilForce.value; }
			set { if( _mode1RecoilForce.BeginSet( this, ref value ) ) { try { Mode1RecoilForceChanged?.Invoke( this ); } finally { _mode1RecoilForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1RecoilForce"/> property value changes.</summary>
		public event Action<WeaponType> Mode1RecoilForceChanged;
		ReferenceField<double> _mode1RecoilForce = 0.0;

		[DefaultValue( "1 50" )]
		[DisplayName( "Mode 1 Firing Distance" )]
		[Category( "Mode 1" )]
		public Reference<Range> Mode1FiringDistance
		{
			get { if( _mode1FiringDistance.BeginGet() ) Mode1FiringDistance = _mode1FiringDistance.Get( this ); return _mode1FiringDistance.value; }
			set { if( _mode1FiringDistance.BeginSet( this, ref value ) ) { try { Mode1FiringDistanceChanged?.Invoke( this ); } finally { _mode1FiringDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FiringDistance"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FiringDistanceChanged;
		ReferenceField<Range> _mode1FiringDistance = new Range( 1, 50 );

		public enum MeleeCollisionDetectionMethodEnum
		{
			Mesh,
			Volume
		}

		[DisplayName( "Mode 1 Melee Collision Detection Method" )]
		[Category( "Mode 1" )]
		[DefaultValue( MeleeCollisionDetectionMethodEnum.Mesh )]
		public Reference<MeleeCollisionDetectionMethodEnum> Mode1MeleeCollisionDetectionMethod
		{
			get { if( _mode1MeleeCollisionDetectionMethod.BeginGet() ) Mode1MeleeCollisionDetectionMethod = _mode1MeleeCollisionDetectionMethod.Get( this ); return _mode1MeleeCollisionDetectionMethod.value; }
			set { if( _mode1MeleeCollisionDetectionMethod.BeginSet( this, ref value ) ) { try { Mode1MeleeCollisionDetectionMethodChanged?.Invoke( this ); } finally { _mode1MeleeCollisionDetectionMethod.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1MeleeCollisionDetectionMethod"/> property value changes.</summary>
		public event Action<WeaponType> Mode1MeleeCollisionDetectionMethodChanged;
		ReferenceField<MeleeCollisionDetectionMethodEnum> _mode1MeleeCollisionDetectionMethod = MeleeCollisionDetectionMethodEnum.Mesh;

		//[DefaultValue( )]
		[DisplayName( "Mode 1 Melee Collision Volume" )]
		[Category( "Mode 1" )]
		public Reference<Sphere> Mode1MeleeCollisionVolume
		{
			get { if( _mode1MeleeCollisionVolume.BeginGet() ) Mode1MeleeCollisionVolume = _mode1MeleeCollisionVolume.Get( this ); return _mode1MeleeCollisionVolume.value; }
			set { if( _mode1MeleeCollisionVolume.BeginSet( this, ref value ) ) { try { Mode1MeleeCollisionVolumeChanged?.Invoke( this ); } finally { _mode1MeleeCollisionVolume.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1MeleeCollisionVolume"/> property value changes.</summary>
		public event Action<WeaponType> Mode1MeleeCollisionVolumeChanged;
		ReferenceField<Sphere> _mode1MeleeCollisionVolume;

		[DefaultValue( 0.0 )]
		[DisplayName( "Mode 1 Melee Damage" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1MeleeDamage
		{
			get { if( _mode1MeleeDamage.BeginGet() ) Mode1MeleeDamage = _mode1MeleeDamage.Get( this ); return _mode1MeleeDamage.value; }
			set { if( _mode1MeleeDamage.BeginSet( this, ref value ) ) { try { Mode1MeleeDamageChanged?.Invoke( this ); } finally { _mode1MeleeDamage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1MeleeDamage"/> property value changes.</summary>
		public event Action<WeaponType> Mode1MeleeDamageChanged;
		ReferenceField<double> _mode1MeleeDamage = 0.0;

		[DefaultValue( 0.0 )]
		[DisplayName( "Mode 1 Melee Impulse" )]
		[Category( "Mode 1" )]
		public Reference<double> Mode1MeleeImpulse
		{
			get { if( _mode1MeleeImpulse.BeginGet() ) Mode1MeleeImpulse = _mode1MeleeImpulse.Get( this ); return _mode1MeleeImpulse.value; }
			set { if( _mode1MeleeImpulse.BeginSet( this, ref value ) ) { try { Mode1MeleeImpulseChanged?.Invoke( this ); } finally { _mode1MeleeImpulse.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1MeleeImpulse"/> property value changes.</summary>
		public event Action<WeaponType> Mode1MeleeImpulseChanged;
		ReferenceField<double> _mode1MeleeImpulse = 0.0;

		[DefaultValue( null )]
		[DisplayName( "Mode 1 Melee Collision Sound" )]
		[Category( "Mode 1" )]
		public Reference<Sound> Mode1MeleeCollisionSound
		{
			get { if( _mode1MeleeCollisionSound.BeginGet() ) Mode1MeleeCollisionSound = _mode1MeleeCollisionSound.Get( this ); return _mode1MeleeCollisionSound.value; }
			set { if( _mode1MeleeCollisionSound.BeginSet( this, ref value ) ) { try { Mode1MeleeCollisionSoundChanged?.Invoke( this ); } finally { _mode1MeleeCollisionSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1MeleeCollisionSound"/> property value changes.</summary>
		public event Action<WeaponType> Mode1MeleeCollisionSoundChanged;
		ReferenceField<Sound> _mode1MeleeCollisionSound = null;

		[DefaultValue( false )]
		[DisplayName( "Mode 1 Stop Movement When Firing" )]
		[Category( "Mode 1" )]
		public Reference<bool> Mode1StopMovementWhenFiring
		{
			get { if( _mode1StopMovementWhenFiring.BeginGet() ) Mode1StopMovementWhenFiring = _mode1StopMovementWhenFiring.Get( this ); return _mode1StopMovementWhenFiring.value; }
			set { if( _mode1StopMovementWhenFiring.BeginSet( this, ref value ) ) { try { Mode1StopMovementWhenFiringChanged?.Invoke( this ); } finally { _mode1StopMovementWhenFiring.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1StopMovementWhenFiring"/> property value changes.</summary>
		public event Action<WeaponType> Mode1StopMovementWhenFiringChanged;
		ReferenceField<bool> _mode1StopMovementWhenFiring = false;

		///// <summary>
		/////The sound that is played when the firing ends.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Mode 1 Firing End Sound" )]
		//[Category( "Mode 1" )]
		//public Reference<Sound> Mode1FiringEndSound
		//{
		//	get { if( _mode1FiringEndSound.BeginGet() ) Mode1FiringEndSound = _mode1FiringEndSound.Get( this ); return _mode1FiringEndSound.value; }
		//	set { if( _mode1FiringEndSound.BeginSet( this, ref value ) ) { try { Mode1FiringEndSoundChanged?.Invoke( this ); } finally { _mode1FiringEndSound.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Mode1FiringEndSound"/> property value changes.</summary>
		//public event Action<WeaponType> Mode1FiringEndSoundChanged;
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
			set { if( _mode2Enabled.BeginSet( this, ref value ) ) { try { Mode2EnabledChanged?.Invoke( this ); } finally { _mode2Enabled.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2Enabled"/> property value changes.</summary>
		public event Action<WeaponType> Mode2EnabledChanged;
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
			set { if( _mode2FiringTotalTime.BeginSet( this, ref value ) ) { try { Mode2FiringTotalTimeChanged?.Invoke( this ); } finally { _mode2FiringTotalTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FiringTotalTime"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FiringTotalTimeChanged;
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
			set { if( _mode2FiringFireTime.BeginSet( this, ref value ) ) { try { Mode2FiringFireTimeChanged?.Invoke( this ); } finally { _mode2FiringFireTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FiringFireTime"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FiringFireTimeChanged;
		ReferenceField<double> _mode2FiringFireTime = 0.0;

		/// <summary>
		/// The type of the bullet.
		/// </summary>
		//!!!![DefaultValueReference( @"Content\Weapons\Default\Bullet 1\Default Bullet.bullettype" )]
		[DefaultValue( null )]
		[DisplayName( "Mode 2 Bullet Type" )]
		[Category( "Mode 2" )]
		public Reference<BulletType> Mode2BulletType
		{
			get { if( _mode2BulletType.BeginGet() ) Mode2BulletType = _mode2BulletType.Get( this ); return _mode2BulletType.value; }
			set { if( _mode2BulletType.BeginSet( this, ref value ) ) { try { Mode2BulletTypeChanged?.Invoke( this ); } finally { _mode2BulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2BulletType"/> property value changes.</summary>
		public event Action<WeaponType> Mode2BulletTypeChanged;
		ReferenceField<BulletType> _mode2BulletType;// = new Reference<BulletType>( null, @"Content\Weapons\Default\Bullet 1\Default Bullet.bullettype" );

		/// <summary>
		/// The initial position, rotation and scale of the bullet.
		/// </summary>
		[DefaultValue( "0.43 0 -0.02; 0 0 0 1; 1 1 1" )]//[DefaultValue( "0.5 0 0; 0 -0.0871557427476582 0 0.996194698091746; 1 1 1" )]
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
				if( _mode2BulletTransform.BeginSet( this, ref value ) ) { try { Mode2BulletTransformChanged?.Invoke( this ); } finally { _mode2BulletTransform.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="Mode2BulletTransform"/> property value changes.</summary>
		public event Action<WeaponType> Mode2BulletTransformChanged;
		ReferenceField<Transform> _mode2BulletTransform = new Transform( new Vector3( 0.43, 0, -0.02 ), Quaternion.Identity, Vector3.One );//new Transform( new Vector3( 0.5, 0, 0 ), new Quaternion( 0, -0.0871557427476582, 0, 0.996194698091746 ), Vector3.One );

		/// <summary>
		/// The initial speed of the bullet.
		/// </summary>
		[DefaultValue( 20.0 )]
		[DisplayName( "Mode 2 Bullet Speed" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2BulletSpeed
		{
			get { if( _mode2BulletSpeed.BeginGet() ) Mode2BulletSpeed = _mode2BulletSpeed.Get( this ); return _mode2BulletSpeed.value; }
			set { if( _mode2BulletSpeed.BeginSet( this, ref value ) ) { try { Mode2BulletSpeedChanged?.Invoke( this ); } finally { _mode2BulletSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2BulletSpeed"/> property value changes.</summary>
		public event Action<WeaponType> Mode2BulletSpeedChanged;
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
			set { if( _mode2BulletDispersionAngle.BeginSet( this, ref value ) ) { try { Mode2BulletDispersionAngleChanged?.Invoke( this ); } finally { _mode2BulletDispersionAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2BulletDispersionAngle"/> property value changes.</summary>
		public event Action<WeaponType> Mode2BulletDispersionAngleChanged;
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
			set { if( _mode2BulletCount.BeginSet( this, ref value ) ) { try { Mode2BulletCountChanged?.Invoke( this ); } finally { _mode2BulletCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2BulletCount"/> property value changes.</summary>
		public event Action<WeaponType> Mode2BulletCountChanged;
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
			set { if( _mode2FireAnimation.BeginSet( this, ref value ) ) { try { Mode2FireAnimationChanged?.Invoke( this ); } finally { _mode2FireAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FireAnimation"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FireAnimationChanged;
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
			set { if( _mode2FireAnimationSpeed.BeginSet( this, ref value ) ) { try { Mode2FireAnimationSpeedChanged?.Invoke( this ); } finally { _mode2FireAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FireAnimationSpeed"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FireAnimationSpeedChanged;
		ReferenceField<double> _mode2FireAnimationSpeed = 1.0;

		/// <summary>
		/// The sound that is played when the firing begins.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons\Default\Sounds\Fire 2.ogg" )]
		[DisplayName( "Mode 2 Firing Begin Sound" )]
		[Category( "Mode 2" )]
		public Reference<Sound> Mode2FiringBeginSound
		{
			get { if( _mode2FiringBeginSound.BeginGet() ) Mode2FiringBeginSound = _mode2FiringBeginSound.Get( this ); return _mode2FiringBeginSound.value; }
			set { if( _mode2FiringBeginSound.BeginSet( this, ref value ) ) { try { Mode2FiringBeginSoundChanged?.Invoke( this ); } finally { _mode2FiringBeginSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FiringBeginSound"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FiringBeginSoundChanged;
		ReferenceField<Sound> _mode2FiringBeginSound = new Reference<Sound>( null, @"Content\Weapons\Default\Sounds\Fire 2.ogg" );

		/// <summary>
		///The sound that is played when a shot occurs.
		/// </summary>
		[DefaultValue( null )]
		[DisplayName( "Mode 2 Fire Sound" )]
		[Category( "Mode 2" )]
		public Reference<Sound> Mode2FireSound
		{
			get { if( _mode2FireSound.BeginGet() ) Mode2FireSound = _mode2FireSound.Get( this ); return _mode2FireSound.value; }
			set { if( _mode2FireSound.BeginSet( this, ref value ) ) { try { Mode2FireSoundChanged?.Invoke( this ); } finally { _mode2FireSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FireSound"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FireSoundChanged;
		ReferenceField<Sound> _mode2FireSound = null;

		[DefaultValue( null )]
		[DisplayName( "Mode 2 Fire Muzzle Flash Particle" )]
		[Category( "Mode 2" )]
		public Reference<ParticleSystem> Mode2FireMuzzleFlashParticle
		{
			get { if( _mode2FireMuzzleFlashParticle.BeginGet() ) Mode2FireMuzzleFlashParticle = _mode2FireMuzzleFlashParticle.Get( this ); return _mode2FireMuzzleFlashParticle.value; }
			set { if( _mode2FireMuzzleFlashParticle.BeginSet( this, ref value ) ) { try { Mode2FireMuzzleFlashParticleChanged?.Invoke( this ); } finally { _mode2FireMuzzleFlashParticle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FireMuzzleFlashParticle"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FireMuzzleFlashParticleChanged;
		ReferenceField<ParticleSystem> _mode2FireMuzzleFlashParticle = null;

		[DefaultValue( 0.15 )]
		[DisplayName( "Mode 2 Max Recoil Offset" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2MaxRecoilOffset
		{
			get { if( _mode2MaxRecoilOffset.BeginGet() ) Mode2MaxRecoilOffset = _mode2MaxRecoilOffset.Get( this ); return _mode2MaxRecoilOffset.value; }
			set { if( _mode2MaxRecoilOffset.BeginSet( this, ref value ) ) { try { Mode2MaxRecoilOffsetChanged?.Invoke( this ); } finally { _mode2MaxRecoilOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2MaxRecoilOffset"/> property value changes.</summary>
		public event Action<WeaponType> Mode2MaxRecoilOffsetChanged;
		ReferenceField<double> _mode2MaxRecoilOffset = 0.15;

		[DefaultValue( 0.05 )]
		[DisplayName( "Mode 2 Max Recoil Offset Time" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2MaxRecoilOffsetTime
		{
			get { if( _mode2MaxRecoilOffsetTime.BeginGet() ) Mode2MaxRecoilOffsetTime = _mode2MaxRecoilOffsetTime.Get( this ); return _mode2MaxRecoilOffsetTime.value; }
			set { if( _mode2MaxRecoilOffsetTime.BeginSet( this, ref value ) ) { try { Mode2MaxRecoilOffsetTimeChanged?.Invoke( this ); } finally { _mode2MaxRecoilOffsetTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2MaxRecoilOffsetTime"/> property value changes.</summary>
		public event Action<WeaponType> Mode2MaxRecoilOffsetTimeChanged;
		ReferenceField<double> _mode2MaxRecoilOffsetTime = 0.05;

		[DefaultValue( 0.0 )]
		[DisplayName( "Mode 2 Recoil Force" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2RecoilForce
		{
			get { if( _mode2RecoilForce.BeginGet() ) Mode2RecoilForce = _mode2RecoilForce.Get( this ); return _mode2RecoilForce.value; }
			set { if( _mode2RecoilForce.BeginSet( this, ref value ) ) { try { Mode2RecoilForceChanged?.Invoke( this ); } finally { _mode2RecoilForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2RecoilForce"/> property value changes.</summary>
		public event Action<WeaponType> Mode2RecoilForceChanged;
		ReferenceField<double> _mode2RecoilForce = 0.0;

		[DefaultValue( "1 50" )]
		[DisplayName( "Mode 2 Firing Distance" )]
		[Category( "Mode 2" )]
		public Reference<Range> Mode2FiringDistance
		{
			get { if( _mode2FiringDistance.BeginGet() ) Mode2FiringDistance = _mode2FiringDistance.Get( this ); return _mode2FiringDistance.value; }
			set { if( _mode2FiringDistance.BeginSet( this, ref value ) ) { try { Mode2FiringDistanceChanged?.Invoke( this ); } finally { _mode2FiringDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FiringDistance"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FiringDistanceChanged;
		ReferenceField<Range> _mode2FiringDistance = new Range( 1, 50 );

		[DisplayName( "Mode 2 Melee Collision Detection Method" )]
		[Category( "Mode 2" )]
		[DefaultValue( MeleeCollisionDetectionMethodEnum.Mesh )]
		public Reference<MeleeCollisionDetectionMethodEnum> Mode2MeleeCollisionDetectionMethod
		{
			get { if( _mode2MeleeCollisionDetectionMethod.BeginGet() ) Mode2MeleeCollisionDetectionMethod = _mode2MeleeCollisionDetectionMethod.Get( this ); return _mode2MeleeCollisionDetectionMethod.value; }
			set { if( _mode2MeleeCollisionDetectionMethod.BeginSet( this, ref value ) ) { try { Mode2MeleeCollisionDetectionMethodChanged?.Invoke( this ); } finally { _mode2MeleeCollisionDetectionMethod.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2MeleeCollisionDetectionMethod"/> property value changes.</summary>
		public event Action<WeaponType> Mode2MeleeCollisionDetectionMethodChanged;
		ReferenceField<MeleeCollisionDetectionMethodEnum> _mode2MeleeCollisionDetectionMethod = MeleeCollisionDetectionMethodEnum.Mesh;

		//[DefaultValue( )]
		[DisplayName( "Mode 2 Melee Collision Volume" )]
		[Category( "Mode 2" )]
		public Reference<Sphere> Mode2MeleeCollisionVolume
		{
			get { if( _mode2MeleeCollisionVolume.BeginGet() ) Mode2MeleeCollisionVolume = _mode2MeleeCollisionVolume.Get( this ); return _mode2MeleeCollisionVolume.value; }
			set { if( _mode2MeleeCollisionVolume.BeginSet( this, ref value ) ) { try { Mode2MeleeCollisionVolumeChanged?.Invoke( this ); } finally { _mode2MeleeCollisionVolume.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2MeleeCollisionVolume"/> property value changes.</summary>
		public event Action<WeaponType> Mode2MeleeCollisionVolumeChanged;
		ReferenceField<Sphere> _mode2MeleeCollisionVolume;

		[DefaultValue( 0.0 )]
		[DisplayName( "Mode 2 Melee Damage" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2MeleeDamage
		{
			get { if( _mode2MeleeDamage.BeginGet() ) Mode2MeleeDamage = _mode2MeleeDamage.Get( this ); return _mode2MeleeDamage.value; }
			set { if( _mode2MeleeDamage.BeginSet( this, ref value ) ) { try { Mode2MeleeDamageChanged?.Invoke( this ); } finally { _mode2MeleeDamage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2MeleeDamage"/> property value changes.</summary>
		public event Action<WeaponType> Mode2MeleeDamageChanged;
		ReferenceField<double> _mode2MeleeDamage = 0.0;

		[DefaultValue( 0.0 )]
		[DisplayName( "Mode 2 Melee Impulse" )]
		[Category( "Mode 2" )]
		public Reference<double> Mode2MeleeImpulse
		{
			get { if( _mode2MeleeImpulse.BeginGet() ) Mode2MeleeImpulse = _mode2MeleeImpulse.Get( this ); return _mode2MeleeImpulse.value; }
			set { if( _mode2MeleeImpulse.BeginSet( this, ref value ) ) { try { Mode2MeleeImpulseChanged?.Invoke( this ); } finally { _mode2MeleeImpulse.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2MeleeImpulse"/> property value changes.</summary>
		public event Action<WeaponType> Mode2MeleeImpulseChanged;
		ReferenceField<double> _mode2MeleeImpulse = 0.0;

		[DefaultValue( null )]
		[DisplayName( "Mode 2 Melee Collision Sound" )]
		[Category( "Mode 2" )]
		public Reference<Sound> Mode2MeleeCollisionSound
		{
			get { if( _mode2MeleeCollisionSound.BeginGet() ) Mode2MeleeCollisionSound = _mode2MeleeCollisionSound.Get( this ); return _mode2MeleeCollisionSound.value; }
			set { if( _mode2MeleeCollisionSound.BeginSet( this, ref value ) ) { try { Mode2MeleeCollisionSoundChanged?.Invoke( this ); } finally { _mode2MeleeCollisionSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2MeleeCollisionSound"/> property value changes.</summary>
		public event Action<WeaponType> Mode2MeleeCollisionSoundChanged;
		ReferenceField<Sound> _mode2MeleeCollisionSound = null;

		[DefaultValue( false )]
		[DisplayName( "Mode 2 Stop Movement When Firing" )]
		[Category( "Mode 2" )]
		public Reference<bool> Mode2StopMovementWhenFiring
		{
			get { if( _mode2StopMovementWhenFiring.BeginGet() ) Mode2StopMovementWhenFiring = _mode2StopMovementWhenFiring.Get( this ); return _mode2StopMovementWhenFiring.value; }
			set { if( _mode2StopMovementWhenFiring.BeginSet( this, ref value ) ) { try { Mode2StopMovementWhenFiringChanged?.Invoke( this ); } finally { _mode2StopMovementWhenFiring.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2StopMovementWhenFiring"/> property value changes.</summary>
		public event Action<WeaponType> Mode2StopMovementWhenFiringChanged;
		ReferenceField<bool> _mode2StopMovementWhenFiring = false;

		///// <summary>
		/////The sound that is played when the firing ends.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Mode 2 Firing End Sound" )]
		//[Category( "Mode 2" )]
		//public Reference<Sound> Mode2FiringEndSound
		//{
		//	get { if( _mode2FiringEndSound.BeginGet() ) Mode2FiringEndSound = _mode2FiringEndSound.Get( this ); return _mode2FiringEndSound.value; }
		//	set { if( _mode2FiringEndSound.BeginSet( this, ref value ) ) { try { Mode2FiringEndSoundChanged?.Invoke( this ); } finally { _mode2FiringEndSound.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Mode2FiringEndSound"/> property value changes.</summary>
		//public event Action<WeaponType> Mode2FiringEndSoundChanged;
		//ReferenceField<Sound> _mode2FiringEndSound = null;

		/////////////////////////////////////////

		[Category( "Use" )]
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> PistolGripBottom
		{
			get { if( _pistolGripBottom.BeginGet() ) PistolGripBottom = _pistolGripBottom.Get( this ); return _pistolGripBottom.value; }
			set { if( _pistolGripBottom.BeginSet( this, ref value ) ) { try { PistolGripBottomChanged?.Invoke( this ); } finally { _pistolGripBottom.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PistolGripBottom"/> property value changes.</summary>
		public event Action<WeaponType> PistolGripBottomChanged;
		ReferenceField<Vector3> _pistolGripBottom = new Vector3( 0, 0, 0 );

		[Category( "Use" )]
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> PistolGripTop
		{
			get { if( _pistolGripTop.BeginGet() ) PistolGripTop = _pistolGripTop.Get( this ); return _pistolGripTop.value; }
			set { if( _pistolGripTop.BeginSet( this, ref value ) ) { try { PistolGripTopChanged?.Invoke( this ); } finally { _pistolGripTop.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PistolGripTop"/> property value changes.</summary>
		public event Action<WeaponType> PistolGripTopChanged;
		ReferenceField<Vector3> _pistolGripTop = new Vector3( 0, 0, 0 );

		//[Category( "Use" )]
		//[DefaultValue( "0 0 0" )]
		//public Reference<Vector3> Trigger
		//{
		//	get { if( _trigger.BeginGet() ) Trigger = _trigger.Get( this ); return _trigger.value; }
		//	set { if( _trigger.BeginSet( this, ref value ) ) { try { TriggerChanged?.Invoke( this ); } finally { _trigger.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Trigger"/> property value changes.</summary>
		//public event Action<WeaponType> TriggerChanged;
		//ReferenceField<Vector3> _trigger = new Vector3( 0, 0, 0 );

		[Category( "Use" )]
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> HandguardNear
		{
			get { if( _handguardNear.BeginGet() ) HandguardNear = _handguardNear.Get( this ); return _handguardNear.value; }
			set { if( _handguardNear.BeginSet( this, ref value ) ) { try { HandguardNearChanged?.Invoke( this ); } finally { _handguardNear.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HandguardNear"/> property value changes.</summary>
		public event Action<WeaponType> HandguardNearChanged;
		ReferenceField<Vector3> _handguardNear = new Vector3( 0, 0, 0 );

		[Category( "Use" )]
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> HandguardFar
		{
			get { if( _handguardFar.BeginGet() ) HandguardFar = _handguardFar.Get( this ); return _handguardFar.value; }
			set { if( _handguardFar.BeginSet( this, ref value ) ) { try { HandguardFarChanged?.Invoke( this ); } finally { _handguardFar.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HandguardFar"/> property value changes.</summary>
		public event Action<WeaponType> HandguardFarChanged;
		ReferenceField<Vector3> _handguardFar = new Vector3( 0, 0, 0 );

		/// <summary>
		/// The image of the weapon type to preview in the inventory.
		/// </summary>
		[Category( "Inventory" )]
		[DefaultValue( null )]
		public Reference<ImageComponent> InventoryImage
		{
			get { if( _inventoryImage.BeginGet() ) InventoryImage = _inventoryImage.Get( this ); return _inventoryImage.value; }
			set { if( _inventoryImage.BeginSet( this, ref value ) ) { try { InventoryImageChanged?.Invoke( this ); } finally { _inventoryImage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InventoryImage"/> property value changes.</summary>
		public event Action<WeaponType> InventoryImageChanged;
		ReferenceField<ImageComponent> _inventoryImage = null;

		[Serialize]
		[Browsable( false )]
		public Transform EditorCameraTransform;

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
				case nameof( Mode1FireAnimation ):
				case nameof( Mode1FiringBeginSound ):
				case nameof( Mode1FireSound ):
				case nameof( Mode1FireMuzzleFlashParticle ):
				case nameof( Mode1MaxRecoilOffset ):
				case nameof( Mode1MaxRecoilOffsetTime ):
				case nameof( Mode1RecoilForce ):
				case nameof( Mode1FiringDistance ):
				case nameof( Mode1StopMovementWhenFiring ):
					//case nameof( Mode1FiringEndSound ):
					if( !Mode1Enabled )
						skip = true;
					break;

				case nameof( Mode1FireAnimationSpeed ):
					if( !Mode1Enabled || !Mode1FireAnimation.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( Mode1BulletTransform ):
				case nameof( Mode1BulletSpeed ):
				case nameof( Mode1BulletDispersionAngle ):
				case nameof( Mode1BulletCount ):
					if( !Mode1Enabled || !Mode1BulletType.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( Mode1MeleeCollisionDetectionMethod ):
				case nameof( Mode1MeleeDamage ):
				case nameof( Mode1MeleeImpulse ):
				case nameof( Mode1MeleeCollisionSound ):
					if( !Mode1Enabled || WayToUse.Value != WayToUseEnum.OneHandedMelee )
						skip = true;
					break;

				case nameof( Mode1MeleeCollisionVolume ):
					if( !Mode1Enabled || WayToUse.Value != WayToUseEnum.OneHandedMelee || Mode1MeleeCollisionDetectionMethod.Value != MeleeCollisionDetectionMethodEnum.Volume )
						skip = true;
					break;

				case nameof( Mode2FiringTotalTime ):
				case nameof( Mode2FiringFireTime ):
				case nameof( Mode2BulletType ):
				case nameof( Mode2FireAnimation ):
				case nameof( Mode2FiringBeginSound ):
				case nameof( Mode2FireSound ):
				case nameof( Mode2FireMuzzleFlashParticle ):
				case nameof( Mode2MaxRecoilOffset ):
				case nameof( Mode2MaxRecoilOffsetTime ):
				case nameof( Mode2RecoilForce ):
				case nameof( Mode2FiringDistance ):
				case nameof( Mode2StopMovementWhenFiring ):
					//case nameof( Mode2FiringEndSound ):
					if( !Mode2Enabled )
						skip = true;
					break;

				case nameof( Mode2FireAnimationSpeed ):
					if( !Mode2Enabled || !Mode2FireAnimation.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( Mode2BulletTransform ):
				case nameof( Mode2BulletSpeed ):
				case nameof( Mode2BulletDispersionAngle ):
				case nameof( Mode2BulletCount ):
					if( !Mode2Enabled || !Mode2BulletType.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( Mode2MeleeCollisionDetectionMethod ):
				case nameof( Mode2MeleeDamage ):
				case nameof( Mode2MeleeImpulse ):
				case nameof( Mode2MeleeCollisionSound ):
					if( !Mode2Enabled || WayToUse.Value != WayToUseEnum.OneHandedMelee )
						skip = true;
					break;

				case nameof( Mode2MeleeCollisionVolume ):
					if( !Mode2Enabled || WayToUse.Value != WayToUseEnum.OneHandedMelee || Mode2MeleeCollisionDetectionMethod.Value != MeleeCollisionDetectionMethodEnum.Volume )
						skip = true;
					break;

				case nameof( PistolGripBottom ):
				case nameof( PistolGripTop ):
				case nameof( HandguardNear ):
				case nameof( HandguardFar ):
					if( WayToUse.Value != WayToUseEnum.Rifle )
						skip = true;
					break;
				}
			}
		}

		//!!!!not used
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

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
		}

		public Vector3 GetHandguardCenter()
		{
			return ( HandguardNear.Value + HandguardFar.Value ) * 0.5;
		}

		public Vector3 GetPistolGripCenter()
		{
			return ( PistolGripBottom.Value + PistolGripTop.Value ) * 0.5;
		}

		public Vector3 GetArmsCenter()
		{
			var p1 = ( PistolGripBottom.Value + PistolGripTop.Value ) * 0.5;
			var p2 = ( HandguardNear.Value + HandguardFar.Value ) * 0.5;
			return ( p1 + p2 ) * 0.5;
		}
	}
}
