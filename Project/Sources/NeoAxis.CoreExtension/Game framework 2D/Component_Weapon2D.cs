// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Base class for making weapons.
	/// </summary>
	[AddToResourcesWindow( @"Base\2D\Weapon 2D", -6000 )]
	[NewObjectDefaultName( "Weapon 2D" )]
	public class Component_Weapon2D : Component_Sprite, IComponent_Item2D, IComponent_InteractiveObject
	{
		bool firing;
		double firingCurrentTime;

		Component_Animation eventAnimation;
		Action eventAnimationUpdateMethod;

		/////////////////////////////////////////

		/// <summary>
		/// Total time of one firing cycle.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Weapon 2D" )]
		public Reference<double> FiringTotalTime
		{
			get { if( _firingTotalTime.BeginGet() ) FiringTotalTime = _firingTotalTime.Get( this ); return _firingTotalTime.value; }
			set { if( _firingTotalTime.BeginSet( ref value ) ) { try { FiringTotalTimeChanged?.Invoke( this ); } finally { _firingTotalTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FiringTotalTime"/> property value changes.</summary>
		public event Action<Component_Weapon2D> FiringTotalTimeChanged;
		ReferenceField<double> _firingTotalTime = 1.0;

		/// <summary>
		/// The time of the bullet creation during firing cycle.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Weapon 2D" )]
		public Reference<double> FiringFireTime
		{
			get { if( _firingFireTime.BeginGet() ) FiringFireTime = _firingFireTime.Get( this ); return _firingFireTime.value; }
			set { if( _firingFireTime.BeginSet( ref value ) ) { try { FiringFireTimeChanged?.Invoke( this ); } finally { _firingFireTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FiringFireTime"/> property value changes.</summary>
		public event Action<Component_Weapon2D> FiringFireTimeChanged;
		ReferenceField<double> _firingFireTime = 0.0;

		/// <summary>
		/// The type of the bullet.
		/// </summary>
		[DefaultValueReference( @"Samples\Starter Content\Scene objects\Default Bullet 2D\Default bullet 2D type.scene|$Bullet" )]
		[Category( "Weapon 2D" )]
		public Reference<Metadata.TypeInfo> BulletType
		{
			get { if( _bulletType.BeginGet() ) BulletType = _bulletType.Get( this ); return _bulletType.value; }
			set { if( _bulletType.BeginSet( ref value ) ) { try { BulletTypeChanged?.Invoke( this ); } finally { _bulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BulletType"/> property value changes.</summary>
		public event Action<Component_Weapon2D> BulletTypeChanged;
		ReferenceField<Metadata.TypeInfo> _bulletType = new Reference<Metadata.TypeInfo>( null, @"Samples\Starter Content\Scene objects\Default Bullet 2D\Default bullet 2D type.scene|$Bullet" );

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
				if( _bulletTransform.BeginSet( ref value ) ) { try { BulletTransformChanged?.Invoke( this ); } finally { _bulletTransform.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="BulletTransform"/> property value changes.</summary>
		public event Action<Component_Weapon2D> BulletTransformChanged;
		ReferenceField<Transform> _bulletTransform = new Transform( new Vector3( 0.65, 0.05, 0 ), Quaternion.Identity, Vector3.One );

		/// <summary>
		/// The initial speed of the bullet.
		/// </summary>
		[DefaultValue( 15.0 )]
		[Category( "Weapon 2D" )]
		public Reference<double> BulletSpeed
		{
			get { if( _bulletSpeed.BeginGet() ) BulletSpeed = _bulletSpeed.Get( this ); return _bulletSpeed.value; }
			set { if( _bulletSpeed.BeginSet( ref value ) ) { try { BulletSpeedChanged?.Invoke( this ); } finally { _bulletSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BulletSpeed"/> property value changes.</summary>
		public event Action<Component_Weapon2D> BulletSpeedChanged;
		ReferenceField<double> _bulletSpeed = 15.0;

		/// <summary>
		/// The sound that is played when the firing begins.
		/// </summary>
		[DefaultValueReference( @"Samples\Starter Content\Sounds\Fire.ogg" )]
		[Category( "Weapon 2D" )]
		public Reference<Component_Sound> SoundFiringBegin
		{
			get { if( _soundFiringBegin.BeginGet() ) SoundFiringBegin = _soundFiringBegin.Get( this ); return _soundFiringBegin.value; }
			set { if( _soundFiringBegin.BeginSet( ref value ) ) { try { SoundFiringBeginChanged?.Invoke( this ); } finally { _soundFiringBegin.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundFiringBegin"/> property value changes.</summary>
		public event Action<Component_Weapon2D> SoundFiringBeginChanged;
		ReferenceField<Component_Sound> _soundFiringBegin = new Reference<Component_Sound>( null, @"Samples\Starter Content\Sounds\Fire.ogg" );

		/// <summary>
		///The sound that is played when a shot occurs.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Weapon 2D" )]
		public Reference<Component_Sound> SoundFire
		{
			get { if( _soundFire.BeginGet() ) SoundFire = _soundFire.Get( this ); return _soundFire.value; }
			set { if( _soundFire.BeginSet( ref value ) ) { try { SoundFireChanged?.Invoke( this ); } finally { _soundFire.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundFire"/> property value changes.</summary>
		public event Action<Component_Weapon2D> SoundFireChanged;
		ReferenceField<Component_Sound> _soundFire = null;

		/// <summary>
		///The sound that is played when the firing ends.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Weapon 2D" )]
		public Reference<Component_Sound> SoundFiringEnd
		{
			get { if( _soundFiringEnd.BeginGet() ) SoundFiringEnd = _soundFiringEnd.Get( this ); return _soundFiringEnd.value; }
			set { if( _soundFiringEnd.BeginSet( ref value ) ) { try { SoundFiringEndChanged?.Invoke( this ); } finally { _soundFiringEnd.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundFiringEnd"/> property value changes.</summary>
		public event Action<Component_Weapon2D> SoundFiringEndChanged;
		ReferenceField<Component_Sound> _soundFiringEnd = null;

		/// <summary>
		/// Whether to display initial position and direction of a bullet.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Weapon 2D" )]
		public Reference<bool> AlwaysDisplayBulletInitialPosition
		{
			get { if( _alwaysDisplayBulletInitialPosition.BeginGet() ) AlwaysDisplayBulletInitialPosition = _alwaysDisplayBulletInitialPosition.Get( this ); return _alwaysDisplayBulletInitialPosition.value; }
			set { if( _alwaysDisplayBulletInitialPosition.BeginSet( ref value ) ) { try { AlwaysDisplayBulletInitialPositionChanged?.Invoke( this ); } finally { _alwaysDisplayBulletInitialPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AlwaysDisplayBulletInitialPosition"/> property value changes.</summary>
		public event Action<Component_Weapon2D> AlwaysDisplayBulletInitialPositionChanged;
		ReferenceField<bool> _alwaysDisplayBulletInitialPosition = false;

		/////////////////////////////////////////
		//Animate

		/// <summary>
		/// Whether to enable default animation method of the weapon.
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
		public event Action<Component_Weapon2D> AnimateChanged;
		ReferenceField<bool> _animate = false;

		/// <summary>
		/// Animation of weapon at rest.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Component_Animation> IdleAnimation
		{
			get { if( _idleAnimation.BeginGet() ) IdleAnimation = _idleAnimation.Get( this ); return _idleAnimation.value; }
			set { if( _idleAnimation.BeginSet( ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IdleAnimation"/> property value changes.</summary>
		public event Action<Component_Weapon2D> IdleAnimationChanged;
		ReferenceField<Component_Animation> _idleAnimation = null;

		/// <summary>
		/// Fire animation of the weapon.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Component_Animation> FireAnimation
		{
			get { if( _fireAnimation.BeginGet() ) FireAnimation = _fireAnimation.Get( this ); return _fireAnimation.value; }
			set { if( _fireAnimation.BeginSet( ref value ) ) { try { FireAnimationChanged?.Invoke( this ); } finally { _fireAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FireAnimation"/> property value changes.</summary>
		public event Action<Component_Weapon2D> FireAnimationChanged;
		ReferenceField<Component_Animation> _fireAnimation = null;

		/////////////////////////////////////////

		[Browsable( false )]
		public bool Firing
		{
			get { return firing; }
		}

		[Browsable( false )]
		public double FiringCurrentTime
		{
			get { return firingCurrentTime; }
		}

		//

		protected virtual void OnCanFire( ref bool canFire )
		{
			if( BulletType.Value == null )
				canFire = false;
		}

		public delegate void CanFireDelegate( Component_Weapon2D sender, ref bool canFire );
		public event CanFireDelegate CanFire;

		public bool PerformCanFire()
		{
			var canFire = true;
			OnCanFire( ref canFire );
			CanFire?.Invoke( this, ref canFire );
			return canFire;
		}

		//

		protected virtual void OnFire()
		{
			var bulletType = BulletType.Value;
			if( bulletType != null )
			{
				var bullet = Parent.CreateComponent( bulletType, enabled: false, setUniqueName: true );

				var bullet2 = bullet as Component_ObjectInSpace;
				if( bullet2 != null )
				{
					var initialTransform = GetBulletWorldInitialTransform();

					var collisionBody = bullet2.GetComponent( "Collision Body" ) as Component_RigidBody2D;
					if( collisionBody != null )
					{
						collisionBody.Transform = initialTransform;
						collisionBody.LinearVelocity = ( initialTransform.Rotation.GetForward() * BulletSpeed ).ToVector2();
					}
					else
						bullet2.Transform = initialTransform;

					//interface IComponent_Bullet
					//{
					//	void BulletCreate( Component creator )
					//}

				}

				bullet.Enabled = true;
			}
		}

		public delegate void FireDelegate( Component_Weapon2D sender );
		public event FireDelegate Fire;

		public bool PerformFire()
		{
			if( !PerformCanFire() )
				return false;

			SoundPlay( SoundFire );
			OnFire();
			Fire?.Invoke( this );

			return true;
		}

		//

		public event Action<Component_Weapon2D> FiringBeginEvent;
		public event Action<Component_Weapon2D> FiringEndEvent;

		public bool FiringBegin()
		{
			if( Firing )
				return false;
			if( !PerformCanFire() )
				return false;

			firing = true;
			firingCurrentTime = 0;

			if( FireAnimation.Value != null )
			{
				EventAnimationBegin( FireAnimation, delegate ()
				{
					EventAnimationBegin( null );
				} );
			}

			SoundPlay( SoundFiringBegin );

			FiringBeginEvent?.Invoke( this );
			return true;
		}

		public void FiringEnd()
		{
			if( !Firing )
				return;

			firing = false;
			firingCurrentTime = 0;

			SoundPlay( SoundFiringEnd );

			FiringEndEvent?.Invoke( this );
		}

		void Simulate( float delta )
		{
			if( Firing )
			{
				var before = firingCurrentTime < FiringFireTime || ( firingCurrentTime == 0 && FiringFireTime == 0 );
				firingCurrentTime += delta;
				var after = firingCurrentTime < FiringFireTime;

				if( before != after )
					PerformFire();

				if( firingCurrentTime >= FiringTotalTime )
					FiringEnd();
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			Simulate( Time.SimulationDelta );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			if( Components.Count == 0 )
			{
				var controller = CreateComponent<Component_SpriteAnimationController>();
				controller.Name = "Sprite Animation Controller";

				Animate = true;
				IdleAnimation = ReferenceUtility.MakeReference( @"Base\Sprites\Weapon\Animation Idle.component" );
				//FireAnimation = ReferenceUtility.MakeReference( @"Base\Sprites\Penguin\Animation Fire.component" );
			}
		}

		public void SoundPlay( Component_Sound sound )
		{
			ParentScene?.SoundPlay2D( sound );
		}

		protected override bool OnSpaceBoundsUpdateIncludeChildren()
		{
			return true;
		}

		public void ObjectInteractionGetInfo( UIControl playScreen, Component_GameMode gameMode, ref IComponent_InteractiveObject_ObjectInfo info )
		{
			//take by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Component_Character2D;
			if( character != null && character.ItemGetEnabledFirst() == null )
			{
				info = new IComponent_InteractiveObject_ObjectInfo();
				info.AllowInteract = true;//CanTake;
				info.SelectionTextInfo.Add( Name );
				info.SelectionTextInfo.Add( "Click mouse button to take. Press T to drop." );
			}
		}

		public virtual bool ObjectInteractionInputMessage( UIControl playScreen, Component_GameMode gameMode, InputMessage message )
		{
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					//take by a character
					var character = gameMode.ObjectControlledByPlayer.Value as Component_Character2D;
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

		public void ObjectInteractionEnter( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public void ObjectInteractionExit( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public void ObjectInteractionUpdate( Component_GameMode.ObjectInteractionContextClass context )
		{
		}

		public delegate void GetBulletWorldInitialTransformEventDelegate( Component_Weapon2D sender, ref Transform transform );
		public event GetBulletWorldInitialTransformEventDelegate GetBulletWorldInitialTransformEvent;

		public virtual Transform GetBulletWorldInitialTransform()
		{
			var result = Transform.Value * BulletTransform.Value;
			GetBulletWorldInitialTransformEvent?.Invoke( this, ref result );
			return result;
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				bool show = ( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this ) || AlwaysDisplayBulletInitialPosition;
				if( show )
				{
					var renderer = context.Owner.Simple3DRenderer;

					var tr = GetBulletWorldInitialTransform();
					var ray = new Ray( tr.Position, tr.Rotation.GetForward() * 0.1 );

					renderer.SetColor( new ColorValue( 1, 0, 0 ) );
					renderer.AddArrow( ray.Origin, ray.GetEndPoint() );
				}
			}
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( IdleAnimation ):
				case nameof( FireAnimation ):
					if( !Animate )
						skip = true;
					break;
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			TickAnimate( delta );
		}

		public Component_SpriteAnimationController GetAnimationController()
		{
			return GetComponent<Component_SpriteAnimationController>( onlyEnabledInHierarchy: true );
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
				}
			}
		}

		void TickAnimate( float delta )
		{
			if( Animate )
			{
				var controller = GetAnimationController();
				if( controller != null )
				{
					//update event animation
					if( eventAnimation != null )
						eventAnimationUpdateMethod?.Invoke();
					if( eventAnimation != null )
					{
						var current = controller.PlayAnimation.Value;
						if( current == eventAnimation && controller.CurrentAnimationTime == eventAnimation.Length )
							EventAnimationEnd();
					}

					//update controller

					Component_Animation animation = null;
					bool autoRewind = true;
					double speed = 1;

					//event animation
					if( eventAnimation != null )
					{
						animation = eventAnimation;
						autoRewind = false;
					}

					//current state animation
					if( animation == null && EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
					{
						//animation = FlyAnimation;
						//autoRewind = true;
					}

					if( animation == null )
					{
						animation = IdleAnimation;
						autoRewind = true;
					}

					//update controller
					controller.PlayAnimation = animation;
					controller.AutoRewind = autoRewind;
					controller.Speed = speed;
				}
			}
		}

		[Browsable( false )]
		public Component_Animation EventAnimation
		{
			get { return eventAnimation; }
		}

		[Browsable( false )]
		public Action EventAnimationUpdateMethod
		{
			get { return eventAnimationUpdateMethod; }
		}

		public void EventAnimationBegin( Component_Animation animation, Action updateMethod = null )
		{
			eventAnimation = animation;
			eventAnimationUpdateMethod = updateMethod;
		}

		public void EventAnimationEnd()
		{
			eventAnimation = null;
			eventAnimationUpdateMethod = null;
		}

	}
}
