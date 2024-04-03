// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Base class for making 2D weapons.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Weapon 2D\Weapon 2D", 23110 )]
	[NewObjectDefaultName( "Weapon 2D" )]
	public class Weapon2D : Sprite, ItemInterface, InteractiveObjectInterface
	{
		Weapon2DType typeCached = new Weapon2DType();

		bool firing;
		double firingCurrentTime;

		Animation eventAnimation;
		Action eventAnimationUpdateMethod;

		//optimization
		SpriteAnimationController animationControllerCached;

		/////////////////////////////////////////

		public const string WeaponTypeDefault = @"Content\Weapons 2D\Default\Default.weapon2dtype";

		/// <summary>
		/// The type of the weapon.
		/// </summary>
		[DefaultValueReference( WeaponTypeDefault )]
		public Reference<Weapon2DType> WeaponType
		{
			get { if( _weaponType.BeginGet() ) WeaponType = _weaponType.Get( this ); return _weaponType.value; }
			set
			{
				if( _weaponType.BeginSet( this, ref value ) )
				{
					try
					{
						WeaponTypeChanged?.Invoke( this );

						//update cached type
						typeCached = _weaponType.value;
						if( typeCached == null )
							typeCached = new Weapon2DType();

						//update mesh
						if( EnabledInHierarchyAndIsInstance )
							UpdateMesh();
					}
					finally { _weaponType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="WeaponType"/> property value changes.</summary>
		public event Action<Weapon2D> WeaponTypeChanged;
		ReferenceField<Weapon2DType> _weaponType = new Reference<Weapon2DType>( null, WeaponTypeDefault );

		/// <summary>
		/// Whether to display initial position and direction of a bullet.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Weapon 2D" )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( this, ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugVisualization"/> property value changes.</summary>
		public event Action<Weapon2D> DebugVisualizationChanged;
		ReferenceField<bool> _debugVisualization = false;

		/////////////////////////////////////////

		[Browsable( false )]
		public Weapon2DType TypeCached
		{
			get { return typeCached; }
		}

		void UpdateMesh()
		{
			//reset animation
			var controller = GetAnimationController();
			if( controller != null )
			{
				controller.PlayAnimation = null;
				controller.Speed = 1;
			}

			TickAnimate( 0 );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			animationControllerCached = null;
			WeaponType.Touch();

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				UpdateMesh();

				if( EngineApp.IsSimulation )
					TickAnimate( 0.001f );
			}
		}

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
			if( TypeCached.BulletType.Value == null )
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
			if( !NetworkIsClient )
			{
				var bulletType = TypeCached.BulletType.Value;
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
								collisionBody.LinearVelocity = ( initialTransform.Rotation.GetForward() * TypeCached.BulletSpeed ).ToVector2();
							}
							else
								bullet2.Transform = initialTransform;
						}

						bullet.Enabled = true;
					}
				}
			}
		}

		public delegate void FireDelegate( Weapon2D sender );
		public event FireDelegate Fire;

		public bool PerformFire()
		{
			if( !PerformCanFire() )
				return false;

			SoundPlay( TypeCached.FireSound );

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

			if( TypeCached.FireAnimation.Value != null )
			{
				EventAnimationBegin( TypeCached.FireAnimation, delegate ()
				{
					EventAnimationBegin( null );
				} );
			}

			SoundPlay( TypeCached.FiringBeginSound );

			if( NetworkIsServer )
			{
				BeginNetworkMessageToEveryone( "FiringBegin" );
				EndNetworkMessage();
			}

			FiringBeginEvent?.Invoke( this );
			return true;
		}

		//!!!!send often. need know network loop time to send only at end of firing. or send start and end
		public void FiringBeginClient()
		{
			//!!!!
			//if( Firing )
			//	return;

			BeginNetworkMessageToServer( "FiringBeginFromClient" );
			EndNetworkMessage();
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
				var before = firingCurrentTime < TypeCached.FiringFireTime || ( firingCurrentTime == 0 && TypeCached.FiringFireTime == 0 );
				firingCurrentTime += delta;
				var after = firingCurrentTime < TypeCached.FiringFireTime;

				if( before != after )
					PerformFire();

				if( firingCurrentTime >= TypeCached.FiringTotalTime )
					FiringEnd();
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			Simulate( Time.SimulationDelta );
		}

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			Simulate( Time.SimulationDelta );
		}

		public delegate void SoundPlayBeforeDelegate( Weapon2D sender, ref Sound sound, ref bool handled );
		public event SoundPlayBeforeDelegate SoundPlayBefore;

		public virtual void SoundPlay( Sound sound )
		{
			var handled = false;
			SoundPlayBefore?.Invoke( this, ref sound, ref handled );
			if( handled )
				return;

			ParentScene?.SoundPlay2D( sound );
		}

		protected override bool OnSpaceBoundsUpdateIncludeChildren()
		{
			return true;
		}

		public delegate void ObjectInteractionGetInfoEventDelegate( Weapon2D sender, GameMode gameMode, ref InteractiveObjectObjectInfo info );
		public event ObjectInteractionGetInfoEventDelegate ObjectInteractionGetInfoEvent;

		public virtual void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			//enable an interaction context to take the object by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character2D;
			if( character != null && character.ItemCanTake( gameMode, this ) )
			{
				info = new InteractiveObjectObjectInfo();
				info.AllowInteract = true;
				//info.Text.Add( "Take the item" );
				//info.Text.Add( Name );
				//info.Text.Add( $"Click to take. Press {gameMode.KeyDrop1.Value} to drop." );
			}
			ObjectInteractionGetInfoEvent?.Invoke( this, gameMode, ref info );
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var mouseDown = message as InputMessageMouseButtonDown;
			if( mouseDown != null )
			{
				if( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right )
				{
					//process an interaction context to take the object by a character
					var character = gameMode.ObjectControlledByPlayer.Value as Character2D;
					if( character != null && character.ItemCanTake( gameMode, this ) )
					{
						if( NetworkIsClient )
							character.ItemTakeAndActivateClient( this );
						else
						{
							if( character.ItemTake( gameMode, this ) )
								character.ItemActivate( gameMode, this );
						}
						return true;
					}
				}
			}

			return false;
		}

		public virtual void ObjectInteractionEnter( ObjectInteractionContext context )
		{
		}

		public virtual void ObjectInteractionExit( ObjectInteractionContext context )
		{
		}

		public virtual void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
		}

		public delegate void GetBulletWorldInitialTransformEventDelegate( Weapon2D sender, ref Transform transform );
		public event GetBulletWorldInitialTransformEventDelegate GetBulletWorldInitialTransformEvent;

		public virtual Transform GetBulletWorldInitialTransform()
		{
			var result = Transform.Value * TypeCached.BulletTransform.Value;
			GetBulletWorldInitialTransformEvent?.Invoke( this, ref result );
			return result;
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum && DebugVisualization )
			{
				var renderer = context.Owner.Simple3DRenderer;

				var tr = GetBulletWorldInitialTransform();
				var ray = new Ray( tr.Position, tr.Rotation.GetForward() * 0.1 );

				renderer.SetColor( new ColorValue( 1, 0, 0 ) );
				renderer.AddArrow( ray.Origin, ray.GetEndPoint() );
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			TickAnimate( delta );
		}

		public SpriteAnimationController GetAnimationController()
		{
			if( animationControllerCached == null )
			{
				animationControllerCached = GetComponent<SpriteAnimationController>();
				if( animationControllerCached == null && !NetworkIsClient )
				{
					animationControllerCached = CreateComponent<SpriteAnimationController>();
					animationControllerCached.Name = "Animation Controller";
				}
			}
			return animationControllerCached;
		}

		void TickAnimate( float delta )
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
					animation = TypeCached.IdleAnimation;
					autoRewind = true;
				}

				//update controller
				controller.PlayAnimation = animation;
				controller.AutoRewind = autoRewind;
				controller.Speed = speed;
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

			if( message == "FiringBegin" )
			{
				if( !reader.Complete() )
					return false;
				FiringBegin();
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
					if( message == "FiringBeginFromClient" )
					{
						if( !reader.Complete() )
							return false;
						FiringBegin();
					}
				}
			}

			return true;
		}

		public virtual void GetInventoryImage( out ImageComponent image, out object anyData )
		{
			image = typeCached.InventoryImage;
			anyData = null;
		}

		[Browsable( false )]
		public Reference<double> ItemCount
		{
			get { return 1.0; }
			set { }
		}

		protected override void OnComponentRemoved( Component component )
		{
			base.OnComponentRemoved( component );

			if( animationControllerCached == component )
				animationControllerCached = null;
		}
	}
}
