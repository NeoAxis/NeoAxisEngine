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
	public class WeaponType : Component
	{
		int version;

		//

		//!!!!
		//DataWasChanged()

		const string meshDefault = @"Content\Weapons\Default\scene.gltf|$Mesh";

		/// <summary>
		/// The main mesh of the weapon.
		/// </summary>
		[DefaultValueReference( meshDefault )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
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
			set { if( _idleAnimation.BeginSet( ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
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
			set { if( _mode1Enabled.BeginSet( ref value ) ) { try { Mode1EnabledChanged?.Invoke( this ); } finally { _mode1Enabled.EndSet(); } } }
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
			set { if( _mode1FiringTotalTime.BeginSet( ref value ) ) { try { Mode1FiringTotalTimeChanged?.Invoke( this ); } finally { _mode1FiringTotalTime.EndSet(); } } }
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
			set { if( _mode1FiringFireTime.BeginSet( ref value ) ) { try { Mode1FiringFireTimeChanged?.Invoke( this ); } finally { _mode1FiringFireTime.EndSet(); } } }
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
			set { if( _mode1BulletType.BeginSet( ref value ) ) { try { Mode1BulletTypeChanged?.Invoke( this ); } finally { _mode1BulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1BulletType"/> property value changes.</summary>
		public event Action<WeaponType> Mode1BulletTypeChanged;
		ReferenceField<BulletType> _mode1BulletType = new Reference<BulletType>( null, @"Content\Weapons\Default\Bullet 1\Default Bullet.bullettype" );

		///// <summary>
		///// The type of the bullet.
		///// </summary>
		//[DefaultValueReference( @"Content\Weapons\Default\Bullet 1\Bullet 1.objectinspace" )]
		//[DisplayName( "Mode 1 Bullet Type" )]
		//[Category( "Mode 1" )]
		//public Reference<Metadata.TypeInfo> Mode1BulletType
		//{
		//	get { if( _mode1BulletType.BeginGet() ) Mode1BulletType = _mode1BulletType.Get( this ); return _mode1BulletType.value; }
		//	set { if( _mode1BulletType.BeginSet( ref value ) ) { try { Mode1BulletTypeChanged?.Invoke( this ); } finally { _mode1BulletType.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Mode1BulletType"/> property value changes.</summary>
		//public event Action<WeaponType> Mode1BulletTypeChanged;
		//ReferenceField<Metadata.TypeInfo> _mode1BulletType = new Reference<Metadata.TypeInfo>( null, @"Content\Weapons\Default\Bullet 1\Bullet 1.objectinspace" );

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
				if( _mode1BulletTransform.BeginSet( ref value ) ) { try { Mode1BulletTransformChanged?.Invoke( this ); } finally { _mode1BulletTransform.EndSet(); } }
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
			set { if( _mode1BulletSpeed.BeginSet( ref value ) ) { try { Mode1BulletSpeedChanged?.Invoke( this ); } finally { _mode1BulletSpeed.EndSet(); } } }
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
			set { if( _mode1BulletDispersionAngle.BeginSet( ref value ) ) { try { Mode1BulletDispersionAngleChanged?.Invoke( this ); } finally { _mode1BulletDispersionAngle.EndSet(); } } }
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
			set { if( _mode1BulletCount.BeginSet( ref value ) ) { try { Mode1BulletCountChanged?.Invoke( this ); } finally { _mode1BulletCount.EndSet(); } } }
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
			set { if( _mode1FireAnimation.BeginSet( ref value ) ) { try { Mode1FireAnimationChanged?.Invoke( this ); } finally { _mode1FireAnimation.EndSet(); } } }
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
			set { if( _mode1FireAnimationSpeed.BeginSet( ref value ) ) { try { Mode1FireAnimationSpeedChanged?.Invoke( this ); } finally { _mode1FireAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FireAnimationSpeed"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FireAnimationSpeedChanged;
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
		public event Action<WeaponType> Mode1FiringBeginSoundChanged;
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
		public event Action<WeaponType> Mode1FireSoundChanged;
		ReferenceField<Sound> _mode1FireSound = null;

		[DefaultValueReference( @"Content\Weapons\Default\Fire 1 muzzle flash\Fire 1 muzzle flash.particle" )]
		[DisplayName( "Mode 1 Fire Muzzle Flash Particle" )]
		[Category( "Mode 1" )]
		public Reference<ParticleSystem> Mode1FireMuzzleFlashParticle
		{
			get { if( _mode1FireMuzzleFlashParticle.BeginGet() ) Mode1FireMuzzleFlashParticle = _mode1FireMuzzleFlashParticle.Get( this ); return _mode1FireMuzzleFlashParticle.value; }
			set { if( _mode1FireMuzzleFlashParticle.BeginSet( ref value ) ) { try { Mode1FireMuzzleFlashParticleChanged?.Invoke( this ); } finally { _mode1FireMuzzleFlashParticle.EndSet(); } } }
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
			set { if( _mode1MaxRecoilOffset.BeginSet( ref value ) ) { try { Mode1MaxRecoilOffsetChanged?.Invoke( this ); } finally { _mode1MaxRecoilOffset.EndSet(); } } }
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
			set { if( _mode1MaxRecoilOffsetTime.BeginSet( ref value ) ) { try { Mode1MaxRecoilOffsetTimeChanged?.Invoke( this ); } finally { _mode1MaxRecoilOffsetTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1MaxRecoilOffsetTime"/> property value changes.</summary>
		public event Action<WeaponType> Mode1MaxRecoilOffsetTimeChanged;
		ReferenceField<double> _mode1MaxRecoilOffsetTime = 0.03;

		[DefaultValue( "1 50" )]
		[DisplayName( "Mode 1 Firing Distance" )]
		[Category( "Mode 1" )]
		public Reference<Range> Mode1FiringDistance
		{
			get { if( _mode1FiringDistance.BeginGet() ) Mode1FiringDistance = _mode1FiringDistance.Get( this ); return _mode1FiringDistance.value; }
			set { if( _mode1FiringDistance.BeginSet( ref value ) ) { try { Mode1FiringDistanceChanged?.Invoke( this ); } finally { _mode1FiringDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode1FiringDistance"/> property value changes.</summary>
		public event Action<WeaponType> Mode1FiringDistanceChanged;
		ReferenceField<Range> _mode1FiringDistance = new Range( 1, 50 );

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
			set { if( _mode2Enabled.BeginSet( ref value ) ) { try { Mode2EnabledChanged?.Invoke( this ); } finally { _mode2Enabled.EndSet(); } } }
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
			set { if( _mode2FiringTotalTime.BeginSet( ref value ) ) { try { Mode2FiringTotalTimeChanged?.Invoke( this ); } finally { _mode2FiringTotalTime.EndSet(); } } }
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
			set { if( _mode2FiringFireTime.BeginSet( ref value ) ) { try { Mode2FiringFireTimeChanged?.Invoke( this ); } finally { _mode2FiringFireTime.EndSet(); } } }
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
			set { if( _mode2BulletType.BeginSet( ref value ) ) { try { Mode2BulletTypeChanged?.Invoke( this ); } finally { _mode2BulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2BulletType"/> property value changes.</summary>
		public event Action<WeaponType> Mode2BulletTypeChanged;
		ReferenceField<BulletType> _mode2BulletType;// = new Reference<BulletType>( null, @"Content\Weapons\Default\Bullet 1\Default Bullet.bullettype" );

		///// <summary>
		///// The type of the bullet.
		///// </summary>
		//[DefaultValueReference( @"Content\Weapons\Default\Bullet 2\Bullet 2.objectinspace" )]
		//[DisplayName( "Mode 2 Bullet Type" )]
		//[Category( "Mode 2" )]
		//public Reference<Metadata.TypeInfo> Mode2BulletType
		//{
		//	get { if( _mode2BulletType.BeginGet() ) Mode2BulletType = _mode2BulletType.Get( this ); return _mode2BulletType.value; }
		//	set { if( _mode2BulletType.BeginSet( ref value ) ) { try { Mode2BulletTypeChanged?.Invoke( this ); } finally { _mode2BulletType.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Mode2BulletType"/> property value changes.</summary>
		//public event Action<WeaponType> Mode2BulletTypeChanged;
		//ReferenceField<Metadata.TypeInfo> _mode2BulletType = new Reference<Metadata.TypeInfo>( null, @"Content\Weapons\Default\Bullet 2\Bullet 2.objectinspace" );

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
				if( _mode2BulletTransform.BeginSet( ref value ) ) { try { Mode2BulletTransformChanged?.Invoke( this ); } finally { _mode2BulletTransform.EndSet(); } }
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
			set { if( _mode2BulletSpeed.BeginSet( ref value ) ) { try { Mode2BulletSpeedChanged?.Invoke( this ); } finally { _mode2BulletSpeed.EndSet(); } } }
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
			set { if( _mode2BulletDispersionAngle.BeginSet( ref value ) ) { try { Mode2BulletDispersionAngleChanged?.Invoke( this ); } finally { _mode2BulletDispersionAngle.EndSet(); } } }
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
			set { if( _mode2BulletCount.BeginSet( ref value ) ) { try { Mode2BulletCountChanged?.Invoke( this ); } finally { _mode2BulletCount.EndSet(); } } }
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
			set { if( _mode2FireAnimation.BeginSet( ref value ) ) { try { Mode2FireAnimationChanged?.Invoke( this ); } finally { _mode2FireAnimation.EndSet(); } } }
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
			set { if( _mode2FireAnimationSpeed.BeginSet( ref value ) ) { try { Mode2FireAnimationSpeedChanged?.Invoke( this ); } finally { _mode2FireAnimationSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FireAnimationSpeed"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FireAnimationSpeedChanged;
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
		public event Action<WeaponType> Mode2FiringBeginSoundChanged;
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
		public event Action<WeaponType> Mode2FireSoundChanged;
		ReferenceField<Sound> _mode2FireSound = null;

		[DefaultValue( null )]
		[DisplayName( "Mode 2 Fire Muzzle Flash Particle" )]
		[Category( "Mode 2" )]
		public Reference<ParticleSystem> Mode2FireMuzzleFlashParticle
		{
			get { if( _mode2FireMuzzleFlashParticle.BeginGet() ) Mode2FireMuzzleFlashParticle = _mode2FireMuzzleFlashParticle.Get( this ); return _mode2FireMuzzleFlashParticle.value; }
			set { if( _mode2FireMuzzleFlashParticle.BeginSet( ref value ) ) { try { Mode2FireMuzzleFlashParticleChanged?.Invoke( this ); } finally { _mode2FireMuzzleFlashParticle.EndSet(); } } }
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
			set { if( _mode2MaxRecoilOffset.BeginSet( ref value ) ) { try { Mode2MaxRecoilOffsetChanged?.Invoke( this ); } finally { _mode2MaxRecoilOffset.EndSet(); } } }
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
			set { if( _mode2MaxRecoilOffsetTime.BeginSet( ref value ) ) { try { Mode2MaxRecoilOffsetTimeChanged?.Invoke( this ); } finally { _mode2MaxRecoilOffsetTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2MaxRecoilOffsetTime"/> property value changes.</summary>
		public event Action<WeaponType> Mode2MaxRecoilOffsetTimeChanged;
		ReferenceField<double> _mode2MaxRecoilOffsetTime = 0.05;

		[DefaultValue( "1 50" )]
		[DisplayName( "Mode 2 Firing Distance" )]
		[Category( "Mode 2" )]
		public Reference<Range> Mode2FiringDistance
		{
			get { if( _mode2FiringDistance.BeginGet() ) Mode2FiringDistance = _mode2FiringDistance.Get( this ); return _mode2FiringDistance.value; }
			set { if( _mode2FiringDistance.BeginSet( ref value ) ) { try { Mode2FiringDistanceChanged?.Invoke( this ); } finally { _mode2FiringDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode2FiringDistance"/> property value changes.</summary>
		public event Action<WeaponType> Mode2FiringDistanceChanged;
		ReferenceField<Range> _mode2FiringDistance = new Range( 1, 50 );

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
		//public event Action<WeaponType> Mode2FiringEndSoundChanged;
		//ReferenceField<Sound> _mode2FiringEndSound = null;

		/////////////////////////////////////////

		/// <summary>
		/// The place of holding with the left hand. X - forward, -Z - palm. Set all zeros to disable the left hand.
		/// </summary>
		[DefaultValue( "0 0 0; 0 0 0 0; 0 0 0" )]//[DefaultValue( "0.2 0 0; 0 0 0 1; 1 1 1" )]
		[Category( "Attaching" )]
		public Reference<Transform> LeftHandTransform
		{
			get { if( _leftHandTransform.BeginGet() ) LeftHandTransform = _leftHandTransform.Get( this ); return _leftHandTransform.value; }
			set { if( _leftHandTransform.BeginSet( ref value ) ) { try { LeftHandTransformChanged?.Invoke( this ); } finally { _leftHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftHandTransform"/> property value changes.</summary>
		public event Action<WeaponType> LeftHandTransformChanged;
		ReferenceField<Transform> _leftHandTransform = new Transform( Vector3.Zero, Quaternion.Zero, Vector3.Zero ); //new Transform( new Vector3( 0.2, 0, 0 ), new Quaternion( 0, 0, 0, 1 ), Vector3.One );

		/// <summary>
		/// The place of holding with the right hand. X - forward, -Z - palm. Set all zeros to disable the right hand.
		/// </summary>
		[DefaultValue( "-0.1 0 -0.05; -0.707106781186548 0 0 0.707106781186547; 1 1 1" )]
		[Category( "Attaching" )]
		public Reference<Transform> RightHandTransform
		{
			get { if( _rightHandTransform.BeginGet() ) RightHandTransform = _rightHandTransform.Get( this ); return _rightHandTransform.value; }
			set { if( _rightHandTransform.BeginSet( ref value ) ) { try { RightHandTransformChanged?.Invoke( this ); } finally { _rightHandTransform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightHandTransform"/> property value changes.</summary>
		public event Action<WeaponType> RightHandTransformChanged;
		ReferenceField<Transform> _rightHandTransform = new Transform( new Vector3( -0.1, 0, -0.05 ), new Quaternion( -0.707106781186548, 0, 0, 0.707106781186547 ), Vector3.One );

		/// <summary>
		/// The transform offset of the weapon relative to the owner.
		/// </summary>
		[DefaultValue( "0.2 -0.15 0.1" )]
		[Category( "Attaching" )]
		public Reference<Vector3> PositionOffsetWhenAttached
		{
			get { if( _positionOffsetWhenAttached.BeginGet() ) PositionOffsetWhenAttached = _positionOffsetWhenAttached.Get( this ); return _positionOffsetWhenAttached.value; }
			set { if( _positionOffsetWhenAttached.BeginSet( ref value ) ) { try { PositionOffsetWhenAttachedChanged?.Invoke( this ); } finally { _positionOffsetWhenAttached.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionOffsetWhenAttached"/> property value changes.</summary>
		public event Action<WeaponType> PositionOffsetWhenAttachedChanged;
		ReferenceField<Vector3> _positionOffsetWhenAttached = new Vector3( 0.2, -0.15, 0.1 );

		/// <summary>
		/// The additional vertical degree for the weapon to the owner eyes looking vector.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Attaching" )]
		public Reference<Degree> RotationOffsetWhenAttached
		{
			get { if( _rotationOffsetWhenAttached.BeginGet() ) RotationOffsetWhenAttached = _rotationOffsetWhenAttached.Get( this ); return _rotationOffsetWhenAttached.value; }
			set { if( _rotationOffsetWhenAttached.BeginSet( ref value ) ) { try { RotationOffsetWhenAttachedChanged?.Invoke( this ); } finally { _rotationOffsetWhenAttached.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffsetWhenAttached"/> property value changes.</summary>
		public event Action<WeaponType> RotationOffsetWhenAttachedChanged;
		ReferenceField<Degree> _rotationOffsetWhenAttached = new Degree( 1.0 );

		/// <summary>
		/// The transform offset when the weapon is attached to the first person camera.
		/// </summary>
		[DefaultValue( "0 -0.1 -0.22" )]
		[Category( "Attaching" )]
		public Reference<Vector3> PositionOffsetWhenAttachedFirstPerson
		{
			get { if( _positionOffsetWhenAttachedFirstPerson.BeginGet() ) PositionOffsetWhenAttachedFirstPerson = _positionOffsetWhenAttachedFirstPerson.Get( this ); return _positionOffsetWhenAttachedFirstPerson.value; }
			set { if( _positionOffsetWhenAttachedFirstPerson.BeginSet( ref value ) ) { try { PositionOffsetWhenAttachedFirstPersonChanged?.Invoke( this ); } finally { _positionOffsetWhenAttachedFirstPerson.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PositionOffsetWhenAttachedFirstPerson"/> property value changes.</summary>
		public event Action<WeaponType> PositionOffsetWhenAttachedFirstPersonChanged;
		ReferenceField<Vector3> _positionOffsetWhenAttachedFirstPerson = new Vector3( 0, -0.1, -0.22 );

		/// <summary>
		/// The additional vertical degree when the weapon is attached to the first person camera.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Attaching" )]
		public Reference<Degree> RotationOffsetWhenAttachedFirstPerson
		{
			get { if( _rotationOffsetWhenAttachedFirstPerson.BeginGet() ) RotationOffsetWhenAttachedFirstPerson = _rotationOffsetWhenAttachedFirstPerson.Get( this ); return _rotationOffsetWhenAttachedFirstPerson.value; }
			set { if( _rotationOffsetWhenAttachedFirstPerson.BeginSet( ref value ) ) { try { RotationOffsetWhenAttachedFirstPersonChanged?.Invoke( this ); } finally { _rotationOffsetWhenAttachedFirstPerson.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RotationOffsetWhenAttachedFirstPerson"/> property value changes.</summary>
		public event Action<WeaponType> RotationOffsetWhenAttachedFirstPersonChanged;
		ReferenceField<Degree> _rotationOffsetWhenAttachedFirstPerson = new Degree( 1.0 );

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
				case nameof( Mode1FireMuzzleFlashParticle ):
				case nameof( Mode1MaxRecoilOffset ):
				case nameof( Mode1FiringDistance ):
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
				case nameof( Mode2FireMuzzleFlashParticle ):
				case nameof( Mode2MaxRecoilOffset ):
				case nameof( Mode2FiringDistance ):
					//case nameof( Mode2FiringEndSound ):
					if( !Mode2Enabled )
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

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			//if( Components.Count == 0 )
			//{
			//	var seat = CreateComponent<WeaponSeat>();
			//	seat.Name = "Weapon Seat";
			//	seat.ExitTransform = new Transform( new Vector3( 0, 2, 1 ), Quaternion.Identity, Vector3.One );
			//}
		}
	}
}
