// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the bullet type.
	/// </summary>
	[ResourceFileExtension( "bullettype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Bullet\Bullet Type", 400 )]
	[EditorControl( typeof( BulletTypeEditor ) )]
	[Preview( typeof( BulletTypePreview ) )]
	[PreviewImage( typeof( BulletTypePreviewImage ) )]
#endif
	public class BulletType : Component
	{
		int version;

		internal bool? existsCollisionCached;

		//

		//!!!!
		//DataWasChanged()

		//const string meshDefault = @"Content\Weapons\Default\Bullet 1\Bullet 1.mesh|$Mesh";

		/// <summary>
		/// The mesh of the character.
		/// </summary>
		//[DefaultValueReference( meshDefault )]
		[DefaultValue( null )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<BulletType> MeshChanged;
		ReferenceField<Mesh> _mesh = new Reference<Mesh>( null, null );// meshDefault );

		/// <summary>
		/// The gravity multiplier when the bullet has no physical body.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 2 )]
		public Reference<double> GravityFactorNoPhysicalBodyMode
		{
			get { if( _gravityFactorNoPhysicalBodyMode.BeginGet() ) GravityFactorNoPhysicalBodyMode = _gravityFactorNoPhysicalBodyMode.Get( this ); return _gravityFactorNoPhysicalBodyMode.value; }
			set { if( _gravityFactorNoPhysicalBodyMode.BeginSet( this, ref value ) ) { try { GravityFactorNoPhysicalBodyModeChanged?.Invoke( this ); DataWasChanged(); } finally { _gravityFactorNoPhysicalBodyMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GravityFactorNoPhysicalBodyMode"/> property value changes.</summary>
		public event Action<BulletType> GravityFactorNoPhysicalBodyModeChanged;
		ReferenceField<double> _gravityFactorNoPhysicalBodyMode = 1.0;

		/// <summary>
		/// The mass when the bullet has no physical body. The mass is used for impulse calculation when hit.
		/// </summary>
		[DefaultValue( 0.05 )]
		public Reference<double> MassNoPhysicalBodyMode
		{
			get { if( _massNoPhysicalBodyMode.BeginGet() ) MassNoPhysicalBodyMode = _massNoPhysicalBodyMode.Get( this ); return _massNoPhysicalBodyMode.value; }
			set { if( _massNoPhysicalBodyMode.BeginSet( this, ref value ) ) { try { MassNoPhysicalBodyModeChanged?.Invoke( this ); DataWasChanged(); } finally { _massNoPhysicalBodyMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MassNoPhysicalBodyMode"/> property value changes.</summary>
		public event Action<BulletType> MassNoPhysicalBodyModeChanged;
		ReferenceField<double> _massNoPhysicalBodyMode = 0.05;

		/// <summary>
		/// Whether the bullet must be destroyed on hit.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> DestroyOnHit
		{
			get { if( _destroyOnHit.BeginGet() ) DestroyOnHit = _destroyOnHit.Get( this ); return _destroyOnHit.value; }
			set { if( _destroyOnHit.BeginSet( this, ref value ) ) { try { DestroyOnHitChanged?.Invoke( this ); DataWasChanged(); } finally { _destroyOnHit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DestroyOnHit"/> property value changes.</summary>
		public event Action<BulletType> DestroyOnHitChanged;
		ReferenceField<bool> _destroyOnHit = true;

		/// <summary>
		/// The strength of the damage. Affects to Health parameter.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> HitDamage
		{
			get { if( _hitDamage.BeginGet() ) HitDamage = _hitDamage.Get( this ); return _hitDamage.value; }
			set { if( _hitDamage.BeginSet( this, ref value ) ) { try { HitDamageChanged?.Invoke( this ); DataWasChanged(); } finally { _hitDamage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitDamage"/> property value changes.</summary>
		public event Action<BulletType> HitDamageChanged;
		ReferenceField<double> _hitDamage = 0.0;

		/// <summary>
		/// The sound that is played when hit happen.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons\Default\Bullet 1\Hit.wav" )]
		public Reference<Sound> HitSound
		{
			get { if( _hitSound.BeginGet() ) HitSound = _hitSound.Get( this ); return _hitSound.value; }
			set { if( _hitSound.BeginSet( this, ref value ) ) { try { HitSoundChanged?.Invoke( this ); DataWasChanged(); } finally { _hitSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitSound"/> property value changes.</summary>
		public event Action<BulletType> HitSoundChanged;
		ReferenceField<Sound> _hitSound = new Reference<Sound>( null, @"Content\Weapons\Default\Bullet 1\Hit.wav" );


		//!!!!local transform


		/// <summary>
		/// The particle system that is created when hit happen.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons\Default\Bullet 1\Hit.particle" )]
		public Reference<ParticleSystem> HitParticle
		{
			get { if( _hitParticle.BeginGet() ) HitParticle = _hitParticle.Get( this ); return _hitParticle.value; }
			set { if( _hitParticle.BeginSet( this, ref value ) ) { try { HitParticleChanged?.Invoke( this ); DataWasChanged(); } finally { _hitParticle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitParticle"/> property value changes.</summary>
		public event Action<BulletType> HitParticleChanged;
		ReferenceField<ParticleSystem> _hitParticle = new Reference<ParticleSystem>( null, @"Content\Weapons\Default\Bullet 1\Hit.particle" );

		[DefaultValue( 1 )]
		public Reference<double> HitParticleLifetime
		{
			get { if( _hitParticleLifetime.BeginGet() ) HitParticleLifetime = _hitParticleLifetime.Get( this ); return _hitParticleLifetime.value; }
			set { if( _hitParticleLifetime.BeginSet( this, ref value ) ) { try { HitParticleLifetimeChanged?.Invoke( this ); DataWasChanged(); } finally { _hitParticleLifetime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitParticleLifetime"/> property value changes.</summary>
		public event Action<BulletType> HitParticleLifetimeChanged;
		ReferenceField<double> _hitParticleLifetime = 1;

		[DefaultValue( true )]
		public Reference<bool> HitParticleApplyHitNormal
		{
			get { if( _hitParticleApplyHitNormal.BeginGet() ) HitParticleApplyHitNormal = _hitParticleApplyHitNormal.Get( this ); return _hitParticleApplyHitNormal.value; }
			set { if( _hitParticleApplyHitNormal.BeginSet( this, ref value ) ) { try { HitParticleApplyHitNormalChanged?.Invoke( this ); } finally { _hitParticleApplyHitNormal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitParticleApplyHitNormal"/> property value changes.</summary>
		public event Action<BulletType> HitParticleApplyHitNormalChanged;
		ReferenceField<bool> _hitParticleApplyHitNormal = true;

		[DefaultValue( true )]
		public Reference<bool> HitDecal
		{
			get { if( _hitDecal.BeginGet() ) HitDecal = _hitDecal.Get( this ); return _hitDecal.value; }
			set { if( _hitDecal.BeginSet( this, ref value ) ) { try { HitDecalChanged?.Invoke( this ); } finally { _hitDecal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitDecal"/> property value changes.</summary>
		public event Action<BulletType> HitDecalChanged;
		ReferenceField<bool> _hitDecal = true;

		[DefaultValue( "0.03 0.1 0.1" )]
		public Reference<Vector3> HitDecalScale
		{
			get { if( _hitDecalScale.BeginGet() ) HitDecalScale = _hitDecalScale.Get( this ); return _hitDecalScale.value; }
			set { if( _hitDecalScale.BeginSet( this, ref value ) ) { try { HitDecalScaleChanged?.Invoke( this ); } finally { _hitDecalScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitDecalScale"/> property value changes.</summary>
		public event Action<BulletType> HitDecalScaleChanged;
		ReferenceField<Vector3> _hitDecalScale = new Vector3( 0.03, 0.1, 0.1 );

		[DefaultValue( 10 )]
		public Reference<double> HitDecalLifetime
		{
			get { if( _hitDecalLifetime.BeginGet() ) HitDecalLifetime = _hitDecalLifetime.Get( this ); return _hitDecalLifetime.value; }
			set { if( _hitDecalLifetime.BeginSet( this, ref value ) ) { try { HitDecalLifetimeChanged?.Invoke( this ); } finally { _hitDecalLifetime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitDecalLifetime"/> property value changes.</summary>
		public event Action<BulletType> HitDecalLifetimeChanged;
		ReferenceField<double> _hitDecalLifetime = 10;

		[DefaultValue( 1.0 )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> HitDecalVisibilityDistanceFactor
		{
			get { if( _hitDecalVisibilityDistanceFactor.BeginGet() ) HitDecalVisibilityDistanceFactor = _hitDecalVisibilityDistanceFactor.Get( this ); return _hitDecalVisibilityDistanceFactor.value; }
			set { if( _hitDecalVisibilityDistanceFactor.BeginSet( this, ref value ) ) { try { HitDecalVisibilityDistanceFactorChanged?.Invoke( this ); } finally { _hitDecalVisibilityDistanceFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitDecalVisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<BulletType> HitDecalVisibilityDistanceFactorChanged;
		ReferenceField<double> _hitDecalVisibilityDistanceFactor = 1.0;

		[DefaultValue( null )]
		[DefaultValueReference( @"Content\Weapons\Default\Bullet 1\Hit decal.material" )]
		public Reference<Material> HitDecalMaterial
		{
			get { if( _hitDecalMaterial.BeginGet() ) HitDecalMaterial = _hitDecalMaterial.Get( this ); return _hitDecalMaterial.value; }
			set { if( _hitDecalMaterial.BeginSet( this, ref value ) ) { try { HitDecalMaterialChanged?.Invoke( this ); } finally { _hitDecalMaterial.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitDecalMaterial"/> property value changes.</summary>
		public event Action<BulletType> HitDecalMaterialChanged;
		ReferenceField<Material> _hitDecalMaterial = new Reference<Material>( null, @"Content\Weapons\Default\Bullet 1\Hit decal.material" );

		//!!!!HitImpulse?

		//!!!!VolumeDamage, VolumeImpulse


		////!!!!сделать как итемы, чтобы расширять? как в MeshInSpace. с другой стороны Item сложнее настраивать, чем просто тип выбрать. где еще так
		///// <summary>
		///// The list of objects to create when the bullet get a collision.
		///// </summary>
		//[Serialize]
		//[Cloneable]
		//public ReferenceList<Metadata.TypeInfo> HitObjects
		//{
		//	get { return _hitObjects; }
		//}
		//public delegate void HitObjectsChangedDelegate( BulletType sender );
		//public event HitObjectsChangedDelegate HitObjectsChanged;
		//ReferenceList<Metadata.TypeInfo> _hitObjects;

		[DefaultValue( 0.0 )]
		public Reference<double> HitVolumeRadius
		{
			get { if( _hitVolumeRadius.BeginGet() ) HitVolumeRadius = _hitVolumeRadius.Get( this ); return _hitVolumeRadius.value; }
			set { if( _hitVolumeRadius.BeginSet( this, ref value ) ) { try { HitVolumeRadiusChanged?.Invoke( this ); } finally { _hitVolumeRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitVolumeRadius"/> property value changes.</summary>
		public event Action<BulletType> HitVolumeRadiusChanged;
		ReferenceField<double> _hitVolumeRadius = 0.0;

		[DefaultValue( 0.0 )]
		public Reference<double> HitVolumeImpulse
		{
			get { if( _hitVolumeImpulse.BeginGet() ) HitVolumeImpulse = _hitVolumeImpulse.Get( this ); return _hitVolumeImpulse.value; }
			set { if( _hitVolumeImpulse.BeginSet( this, ref value ) ) { try { HitVolumeImpulseChanged?.Invoke( this ); } finally { _hitVolumeImpulse.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitVolumeImpulse"/> property value changes.</summary>
		public event Action<BulletType> HitVolumeImpulseChanged;
		ReferenceField<double> _hitVolumeImpulse = 0.0;

		[DefaultValue( 0.0 )]
		public Reference<double> HitVolumeDamage
		{
			get { if( _hitVolumeDamage.BeginGet() ) HitVolumeDamage = _hitVolumeDamage.Get( this ); return _hitVolumeDamage.value; }
			set { if( _hitVolumeDamage.BeginSet( this, ref value ) ) { try { HitVolumeDamageChanged?.Invoke( this ); } finally { _hitVolumeDamage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitVolumeDamage"/> property value changes.</summary>
		public event Action<BulletType> HitVolumeDamageChanged;
		ReferenceField<double> _hitVolumeDamage = 0.0;


		/////////////////////////////////////////

		///// <summary>
		///// The list of objects to created when the bullet get a collision.
		///// </summary>
		//[Serialize]
		//[Cloneable]
		//public ReferenceList<Metadata.TypeInfo> LifetimeEndObjects
		//{
		//	get { return _lifetimeEndObjects; }
		//}
		//public delegate void LifetimeEndObjectsChangedDelegate( Bullet sender );
		//public event LifetimeEndObjectsChangedDelegate LifetimeEndObjectsChanged;
		//ReferenceList<Metadata.TypeInfo> _lifetimeEndObjects;

		/////////////////////////////////////////

		//protected virtual void OnHitBefore( Bullet bullet, Bullet.HitData hit ) { }
		public delegate void HitBeforeDelegate( BulletType sender, Bullet bullet, Bullet.HitData hit );
		public event HitBeforeDelegate HitBefore;

		public void PerformHitBefore( Bullet bullet, Bullet.HitData hit )
		{
			//OnHitBefore( bullet, hit );
			//if( hit.Handled )
			//	return;
			if( HitBefore != null )
			{
				HitBefore.Invoke( this, bullet, hit );
				if( hit.Handled )
					return;
			}
		}

		public delegate void HitDelegate( BulletType sender, Bullet bullet, Bullet.HitData hit );
		public event HitDelegate Hit;

		public void PerformHit( Bullet bullet, Bullet.HitData hit )
		{
			//OnHit( bullet, hit );
			//if( hit.Handled )
			//	return;
			if( Hit != null )
			{
				Hit.Invoke( this, bullet, hit );
				if( hit.Handled )
					return;
			}
		}

		/////////////////////////////////////////

		//public BulletType()
		//{
		//	_hitObjects = new ReferenceList<Metadata.TypeInfo>( this, () => HitObjectsChanged?.Invoke( this ) );
		//	//_lifetimeEndObjects = new ReferenceList<Metadata.TypeInfo>( this, () => LifetimeEndObjectsChanged?.Invoke( this ) );
		//}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( HitDecalScale ):
				case nameof( HitDecalLifetime ):
				case nameof( HitDecalVisibilityDistanceFactor ):
				case nameof( HitDecalMaterial ):
					if( !HitDecal )
						skip = true;
					break;

				case nameof( HitParticleLifetime ):
					//case nameof( HitParticleApplyBulletRotation ):
					if( !HitParticle.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( HitVolumeDamage ):
				case nameof( HitVolumeImpulse ):
					if( HitVolumeRadius <= 0 )
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

		//public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		//{
		//	if( Components.Count == 0 )
		//	{
		//		var seat = CreateComponent<BulletSeat>();
		//		seat.Name = "Bullet Seat";
		//		seat.ExitTransform = new Transform( new Vector3( 0, 2, 1 ), Quaternion.Identity, Vector3.One );
		//	}
		//}
	}
}
