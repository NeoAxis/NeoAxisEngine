// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Base class for making 2D weapons.
	/// </summary>
	[ResourceFileExtension( "weapon2dtype" )]
	[NewObjectDefaultName( "Weapon 2D Type" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Weapon 2D\Weapon 2D Type", 23100 )]
	[EditorControl( typeof( Weapon2DTypeEditor ) )]
	[Preview( typeof( Weapon2DTypePreview ) )]
	[PreviewImage( typeof( Weapon2DTypePreviewImage ) )]
#endif
	public class Weapon2DType : Component, ItemTypeInterface
	{
		int version;

		//!!!!
		//DataWasChanged()

		/// <summary>
		/// Total time of one firing cycle.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Weapon 2D" )]
		public Reference<double> FiringTotalTime
		{
			get { if( _firingTotalTime.BeginGet() ) FiringTotalTime = _firingTotalTime.Get( this ); return _firingTotalTime.value; }
			set { if( _firingTotalTime.BeginSet( this, ref value ) ) { try { FiringTotalTimeChanged?.Invoke( this ); } finally { _firingTotalTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FiringTotalTime"/> property value changes.</summary>
		public event Action<Weapon2DType> FiringTotalTimeChanged;
		ReferenceField<double> _firingTotalTime = 1.0;

		/// <summary>
		/// The time of the bullet creation during firing cycle.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Weapon 2D" )]
		public Reference<double> FiringFireTime
		{
			get { if( _firingFireTime.BeginGet() ) FiringFireTime = _firingFireTime.Get( this ); return _firingFireTime.value; }
			set { if( _firingFireTime.BeginSet( this, ref value ) ) { try { FiringFireTimeChanged?.Invoke( this ); } finally { _firingFireTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FiringFireTime"/> property value changes.</summary>
		public event Action<Weapon2DType> FiringFireTimeChanged;
		ReferenceField<double> _firingFireTime = 0.0;

		/// <summary>
		/// The type of the bullet.
		/// </summary>
		[DefaultValueReference( @"Content\Weapons 2D\Default\Bullet\Bullet.objectinspace" )]
		[Category( "Weapon 2D" )]
		public Reference<Metadata.TypeInfo> BulletType
		{
			get { if( _bulletType.BeginGet() ) BulletType = _bulletType.Get( this ); return _bulletType.value; }
			set { if( _bulletType.BeginSet( this, ref value ) ) { try { BulletTypeChanged?.Invoke( this ); } finally { _bulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BulletType"/> property value changes.</summary>
		public event Action<Weapon2DType> BulletTypeChanged;
		ReferenceField<Metadata.TypeInfo> _bulletType = new Reference<Metadata.TypeInfo>( null, @"Content\Weapons 2D\Default\Bullet\Bullet.objectinspace" );

		/// <summary>
		/// The initial position, rotation and scale of the bullet.
		/// </summary>
		[DefaultValue( "0.65 0.05 0; 0 0 0 1; 1 1 1" )]
		[Category( "Weapon 2D" )]
		public Reference<Transform> BulletTransform
		{
			get { if( _bulletTransform.BeginGet() ) BulletTransform = _bulletTransform.Get( this ); return _bulletTransform.value; }
			set
			{
				//fix invalid value
				if( value.Value == null )
					value = new Reference<Transform>( NeoAxis.Transform.Identity, value.GetByReference );
				if( _bulletTransform.BeginSet( this, ref value ) ) { try { BulletTransformChanged?.Invoke( this ); } finally { _bulletTransform.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="BulletTransform"/> property value changes.</summary>
		public event Action<Weapon2DType> BulletTransformChanged;
		ReferenceField<Transform> _bulletTransform = new Transform( new Vector3( 0.65, 0.05, 0 ), Quaternion.Identity, Vector3.One );

		/// <summary>
		/// The initial speed of the bullet.
		/// </summary>
		[DefaultValue( 15.0 )]
		[Category( "Weapon 2D" )]
		public Reference<double> BulletSpeed
		{
			get { if( _bulletSpeed.BeginGet() ) BulletSpeed = _bulletSpeed.Get( this ); return _bulletSpeed.value; }
			set { if( _bulletSpeed.BeginSet( this, ref value ) ) { try { BulletSpeedChanged?.Invoke( this ); } finally { _bulletSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BulletSpeed"/> property value changes.</summary>
		public event Action<Weapon2DType> BulletSpeedChanged;
		ReferenceField<double> _bulletSpeed = 15.0;

		/// <summary>
		/// The sound that is played when the firing begins.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Weapon 2D" )]
		public Reference<Sound> FiringBeginSound
		{
			get { if( _firingBeginSound.BeginGet() ) FiringBeginSound = _firingBeginSound.Get( this ); return _firingBeginSound.value; }
			set { if( _firingBeginSound.BeginSet( this, ref value ) ) { try { FiringBeginSoundChanged?.Invoke( this ); } finally { _firingBeginSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FiringBeginSound"/> property value changes.</summary>
		public event Action<Weapon2DType> FiringBeginSoundChanged;
		ReferenceField<Sound> _firingBeginSound = null;

		/// <summary>
		///The sound that is played when a shot occurs.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Weapon 2D" )]
		public Reference<Sound> FireSound
		{
			get { if( _fireSound.BeginGet() ) FireSound = _fireSound.Get( this ); return _fireSound.value; }
			set { if( _fireSound.BeginSet( this, ref value ) ) { try { FireSoundChanged?.Invoke( this ); } finally { _fireSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FireSound"/> property value changes.</summary>
		public event Action<Weapon2DType> FireSoundChanged;
		ReferenceField<Sound> _fireSound = null;

		///// <summary>
		/////The sound that is played when the firing ends.
		///// </summary>
		//[DefaultValue( null )]
		//[Category( "Weapon 2D" )]
		//public Reference<Sound> FiringEndSound
		//{
		//	get { if( _firingEndSound.BeginGet() ) FiringEndSound = _firingEndSound.Get( this ); return _firingEndSound.value; }
		//	set { if( _firingEndSound.BeginSet( this, ref value ) ) { try { FiringEndSoundChanged?.Invoke( this ); } finally { _firingEndSound.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="FiringEndSound"/> property value changes.</summary>
		//public event Action<Weapon2DType> FiringEndSoundChanged;
		//ReferenceField<Sound> _firingEndSound = null;

		/////////////////////////////////////////
		//Animate

		///// <summary>
		///// Whether to enable default animation method of the weapon.
		///// </summary>
		//[Category( "Animation" )]
		//[DefaultValue( false )]
		//public Reference<bool> Animate
		//{
		//	get { if( _animate.BeginGet() ) Animate = _animate.Get( this ); return _animate.value; }
		//	set { if( _animate.BeginSet( this, ref value ) ) { try { AnimateChanged?.Invoke( this ); DataWasChanged(); } finally { _animate.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Animate"/> property value changes.</summary>
		//public event Action<Weapon2DType> AnimateChanged;
		//ReferenceField<bool> _animate = false;

		/// <summary>
		/// Animation of weapon at rest.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Animation> IdleAnimation
		{
			get { if( _idleAnimation.BeginGet() ) IdleAnimation = _idleAnimation.Get( this ); return _idleAnimation.value; }
			set { if( _idleAnimation.BeginSet( this, ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IdleAnimation"/> property value changes.</summary>
		public event Action<Weapon2DType> IdleAnimationChanged;
		ReferenceField<Animation> _idleAnimation = null;

		/// <summary>
		/// Fire animation of the weapon.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Animation> FireAnimation
		{
			get { if( _fireAnimation.BeginGet() ) FireAnimation = _fireAnimation.Get( this ); return _fireAnimation.value; }
			set { if( _fireAnimation.BeginSet( this, ref value ) ) { try { FireAnimationChanged?.Invoke( this ); } finally { _fireAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FireAnimation"/> property value changes.</summary>
		public event Action<Weapon2DType> FireAnimationChanged;
		ReferenceField<Animation> _fireAnimation = null;

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
		public event Action<Weapon2DType> InventoryImageChanged;
		ReferenceField<ImageComponent> _inventoryImage = null;

		/////////////////////////////////////////

		//protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		//{
		//	base.OnMetadataGetMembersFilter( context, member, ref skip );

		//	if( member is Metadata.Property )
		//	{
		//		switch( member.Name )
		//		{
		//		case nameof( IdleAnimation ):
		//		case nameof( FireAnimation ):
		//			if( !Animate )
		//				skip = true;
		//			break;
		//		}
		//	}
		//}


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
	}
}
