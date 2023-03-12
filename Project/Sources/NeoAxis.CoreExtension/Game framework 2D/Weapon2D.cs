// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Base class for making weapons.
	/// </summary>
	[AddToResourcesWindow( @"Base\2D\Weapon 2D", -6000 )]
	[NewObjectDefaultName( "Weapon 2D" )]
	public class Weapon2D : Sprite, IGameFrameworkItem2D, InteractiveObject
	{
		bool firing;
		double firingCurrentTime;

		Animation eventAnimation;
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
		public event Action<Weapon2D> FiringTotalTimeChanged;
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
		public event Action<Weapon2D> FiringFireTimeChanged;
		ReferenceField<double> _firingFireTime = 0.0;

		/// <summary>
		/// The type of the bullet.
		/// </summary>
		[DefaultValueReference( @"Content\2D\Weapon\Bullet\Default bullet 2D type.scene|$Bullet" )]
		[Category( "Weapon 2D" )]
		public Reference<Metadata.TypeInfo> BulletType
		{
			get { if( _bulletType.BeginGet() ) BulletType = _bulletType.Get( this ); return _bulletType.value; }
			set { if( _bulletType.BeginSet( ref value ) ) { try { BulletTypeChanged?.Invoke( this ); } finally { _bulletType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BulletType"/> property value changes.</summary>
		public event Action<Weapon2D> BulletTypeChanged;
		ReferenceField<Metadata.TypeInfo> _bulletType = new Reference<Metadata.TypeInfo>( null, @"Content\2D\Weapon\Bullet\Default bullet 2D type.scene|$Bullet" );

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
		public event Action<Weapon2D> BulletTransformChanged;
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
		public event Action<Weapon2D> BulletSpeedChanged;
		ReferenceField<double> _bulletSpeed = 15.0;

		/// <summary>
		/// The sound that is played when the firing begins.
		/// </summary>
		[DefaultValueReference( @"Content\2D\Weapon\Fire.ogg" )]
		[Category( "Weapon 2D" )]
		public Reference<Sound> FiringBeginSound
		{
			get { if( _firingBeginSound.BeginGet() ) FiringBeginSound = _firingBeginSound.Get( this ); return _firingBeginSound.value; }
			set { if( _firingBeginSound.BeginSet( ref value ) ) { try { FiringBeginSoundChanged?.Invoke( this ); } finally { _firingBeginSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FiringBeginSound"/> property value changes.</summary>
		public event Action<Weapon2D> FiringBeginSoundChanged;
		ReferenceField<Sound> _firingBeginSound = new Reference<Sound>( null, @"Content\2D\Weapon\Fire.ogg" );

		/// <summary>
		///The sound that is played when a shot occurs.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Weapon 2D" )]
		public Reference<Sound> FireSound
		{
			get { if( _fireSound.BeginGet() ) FireSound = _fireSound.Get( this ); return _fireSound.value; }
			set { if( _fireSound.BeginSet( ref value ) ) { try { FireSoundChanged?.Invoke( this ); } finally { _fireSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FireSound"/> property value changes.</summary>
		public event Action<Weapon2D> FireSoundChanged;
		ReferenceField<Sound> _fireSound = null;

		///// <summary>
		/////The sound that is played when the firing ends.
		///// </summary>
		//[DefaultValue( null )]
		//[Category( "Weapon 2D" )]
		//public Reference<Sound> FiringEndSound
		//{
		//	get { if( _firingEndSound.BeginGet() ) FiringEndSound = _firingEndSound.Get( this ); return _firingEndSound.value; }
		//	set { if( _firingEndSound.BeginSet( ref value ) ) { try { FiringEndSoundChanged?.Invoke( this ); } finally { _firingEndSound.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="FiringEndSound"/> property value changes.</summary>
		//public event Action<Weapon2D> FiringEndSoundChanged;
		//ReferenceField<Sound> _firingEndSound = null;

		/// <summary>
		/// Whether to display initial position and direction of a bullet.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Weapon 2D" )]
		public Reference<bool> AlwaysDisplayAdditionalInfo
		{
			get { if( _alwaysDisplayAdditionalInfo.BeginGet() ) AlwaysDisplayAdditionalInfo = _alwaysDisplayAdditionalInfo.Get( this ); return _alwaysDisplayAdditionalInfo.value; }
			set { if( _alwaysDisplayAdditionalInfo.BeginSet( ref value ) ) { try { AlwaysDisplayAdditionalInfoChanged?.Invoke( this ); } finally { _alwaysDisplayAdditionalInfo.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AlwaysDisplayAdditionalInfo"/> property value changes.</summary>
		public event Action<Weapon2D> AlwaysDisplayAdditionalInfoChanged;
		ReferenceField<bool> _alwaysDisplayAdditionalInfo = false;

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
		public event Action<Weapon2D> AnimateChanged;
		ReferenceField<bool> _animate = false;

		/// <summary>
		/// Animation of weapon at rest.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Animation> IdleAnimation
		{
			get { if( _idleAnimation.BeginGet() ) IdleAnimation = _idleAnimation.Get( this ); return _idleAnimation.value; }
			set { if( _idleAnimation.BeginSet( ref value ) ) { try { IdleAnimationChanged?.Invoke( this ); } finally { _idleAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IdleAnimation"/> property value changes.</summary>
		public event Action<Weapon2D> IdleAnimationChanged;
		ReferenceField<Animation> _idleAnimation = null;

		/// <summary>
		/// Fire animation of the weapon.
		/// </summary>
		[Category( "Animation" )]
		[DefaultValue( null )]
		public Reference<Animation> FireAnimation
		{
			get { if( _fireAnimation.BeginGet() ) FireAnimation = _fireAnimation.Get( this ); return _fireAnimation.value; }
			set { if( _fireAnimation.BeginSet( ref value ) ) { try { FireAnimationChanged?.Invoke( this ); } finally { _fireAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FireAnimation"/> property value changes.</summary>
		public event Action<Weapon2D> FireAnimationChanged;
		ReferenceField<Animation> _fireAnimation = null;

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

		public delegate void CanFireDelegate( Weapon2D sender, ref bool canFire );
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
				var scene = ParentScene;
				if( scene != null )
				{
					var bullet = scene.CreateComponent( bulletType, enabled: false, setUniqueName: true );

					var bullet2 = bullet as ObjectInSpace;
					if( bullet2 != null )
					{
						var initialTransform = GetBulletWorldInitialTransform();

						var collisionBody = bullet2.GetComponent( "Collision Body" ) as RigidBody2D;
						if( collisionBody != null )
						{
							collisionBody.Transform = initialTransform;
							collisionBody.LinearVelocity = ( initialTransform.Rotation.GetForward() * BulletSpeed ).ToVector2();
						}
						else
							bullet2.Transform = initialTransform;
					}

					bullet.Enabled = true;
				}
			}
		}

		public delegate void FireDelegate( Weapon2D sender );
		public event FireDelegate Fire;

		public bool PerformFire()
		{
			if( !PerformCanFire() )
				return false;

			SoundPlay( FireSound );
			if( NetworkIsServer && FireSound.ReferenceOrValueSpecified )
			{
				BeginNetworkMessageToEveryone( "FireSound" );
				EndNetworkMessage();
			}

			OnFire();
			Fire?.Invoke( this );

			return true;
		}

		//

		public event Action<Weapon2D> FiringBeginEvent;
		public event Action<Weapon2D> FiringEndEvent;

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

			SoundPlay( FiringBeginSound );

			if( NetworkIsServer )
			{
				BeginNetworkMessageToEveryone( "FiringBegin" );
				EndNetworkMessage();
			}

			FiringBeginEvent?.Invoke( this );
			return true;
		}

		public void FiringEnd()
		{
			if( !Firing )
				return;

			firing = false;
			firingCurrentTime = 0;
			//SoundPlay( FiringEndSound );
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
				{
					if( NetworkIsClient )
					{
						BeginNetworkMessageToServer( "FiringEnd" );
						EndNetworkMessage();
					}
					else
						FiringEnd();
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
			if( Components.Count == 0 )
			{
				var controller = CreateComponent<SpriteAnimationController>();
				controller.Name = "Sprite Animation Controller";

				Animate = true;
				IdleAnimation = ReferenceUtility.MakeReference( @"content\2D\Weapon\Animation Idle.component" );
				//FireAnimation = ReferenceUtility.MakeReference( @"content\2D\Penguin\Animation Fire.component" );
			}
		}

		public void SoundPlay( Sound sound )
		{
			ParentScene?.SoundPlay2D( sound );
		}

		protected override bool OnSpaceBoundsUpdateIncludeChildren()
		{
			return true;
		}

		public void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			//take by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character2D;
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
					var character = gameMode.ObjectControlledByPlayer.Value as Character2D;
					if( character != null && character.ItemGetEnabledFirst() == null )
					{
						if( NetworkIsClient )
						{
							var writer = character.BeginNetworkMessageToServer( "ItemTakeAndActivate" );
							if( writer != null )
							{
								writer.WriteVariableUInt64( (ulong)NetworkID );
								character.EndNetworkMessage();
							}
						}
						else
						{
							character.ItemTake( this );
							character.ItemActivate( this );
						}

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

		public delegate void GetBulletWorldInitialTransformEventDelegate( Weapon2D sender, ref Transform transform );
		public event GetBulletWorldInitialTransformEventDelegate GetBulletWorldInitialTransformEvent;

		public virtual Transform GetBulletWorldInitialTransform()
		{
			var result = Transform.Value * BulletTransform.Value;
			GetBulletWorldInitialTransformEvent?.Invoke( this, ref result );
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

		public SpriteAnimationController GetAnimationController()
		{
			return GetComponent<SpriteAnimationController>( onlyEnabledInHierarchy: true );
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

					Animation animation = null;
					bool autoRewind = true;
					double speed = 1;

					//event animation
					if( eventAnimation != null )
					{
						animation = eventAnimation;
						autoRewind = false;
					}

					//current state animation
					if( animation == null && EngineApp.IsSimulation )
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
		public Animation EventAnimation
		{
			get { return eventAnimation; }
		}

		[Browsable( false )]
		public Action EventAnimationUpdateMethod
		{
			get { return eventAnimationUpdateMethod; }
		}

		public void EventAnimationBegin( Animation animation, Action updateMethod = null )
		{
			eventAnimation = animation;
			eventAnimationUpdateMethod = updateMethod;
		}

		public void EventAnimationEnd()
		{
			eventAnimation = null;
			eventAnimationUpdateMethod = null;
		}

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "FireSound" )
			{
				if( !reader.Complete() )
					return false;
				SoundPlay( FireSound );
			}
			else if( message == "FiringBegin" )
			{
				if( !reader.Complete() )
					return false;

				if( FireAnimation.Value != null )
				{
					EventAnimationBegin( FireAnimation, delegate ()
					{
						EventAnimationBegin( null );
					} );
				}

				SoundPlay( FiringBeginSound );
			}

			return true;
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			if( Parent != null )
			{
				//security check the object is controlled by the player
				var networkLogic = NetworkLogicUtility.GetNetworkLogic( this );
				if( networkLogic != null && networkLogic.ServerGetObjectControlledByUser( client.User, true ) == Parent )
				{
					if( message == "FiringBegin" )
					{
						if( !reader.Complete() )
							return false;
						FiringBegin();
					}
					else if( message == "FiringEnd" )
					{
						if( !reader.Complete() )
							return false;
						FiringEnd();
					}
				}
			}

			return true;
		}
	}
}
