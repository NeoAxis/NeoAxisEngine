// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// An instance of a weapon.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Weapon\Weapon", 22101 )]
	[NewObjectDefaultName( "Weapon" )]
	public class Weapon : MeshInSpace, IGameFrameworkItem, InteractiveObject
	{
		static FastRandom staticRandom = null;

		WeaponType typeCached = new WeaponType();

		bool[] firing = new bool[ 3 ];
		float[] firingCurrentTime = new float[ 3 ];
		//network UserID
		long[] firingWhoFired = new long[ 3 ];

		//play one animation
		Animation playOneAnimation = null;
		float playOneAnimationSpeed = 1;
		float playOneAnimationRemainingTime;

		//optimization
		MeshInSpaceAnimationController animationControllerCached;

		/////////////////////////////////////////

		const string weaponTypeDefault = @"Content\Weapons\Default\Default Weapon.weapontype";

		/// <summary>
		/// The type of the weapon.
		/// </summary>
		[DefaultValueReference( weaponTypeDefault )]
		public Reference<WeaponType> WeaponType
		{
			get { if( _weaponType.BeginGet() ) WeaponType = _weaponType.Get( this ); return _weaponType.value; }
			set
			{
				if( _weaponType.BeginSet( ref value ) )
				{
					try
					{
						WeaponTypeChanged?.Invoke( this );

						//update cached type and mesh
						typeCached = _weaponType.value;
						if( typeCached == null )
							typeCached = new WeaponType();
						UpdateMesh();
					}
					finally { _weaponType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="WeaponType"/> property value changes.</summary>
		public event Action<Weapon> WeaponTypeChanged;
		ReferenceField<WeaponType> _weaponType = new Reference<WeaponType>( null, weaponTypeDefault );

		/// <summary>
		/// Whether to display initial position and direction of a bullet and places of hands.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugVisualization"/> property value changes.</summary>
		public event Action<Weapon> DebugVisualizationChanged;
		ReferenceField<bool> _debugVisualization = false;

		/////////////////////////////////////////

		[Browsable( false )]
		public WeaponType WeaponTypeCached
		{
			get { return typeCached; }
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//these properties are under control by the class
				case nameof( Mesh ):
					skip = true;
					break;
				}
			}
		}

		void UpdateMesh()
		{
			Mesh = typeCached.Mesh;

			//!!!!
			//Collision = 
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			animationControllerCached = null;

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				//update cached type and mesh
				typeCached = WeaponType.Value;
				if( typeCached == null )
					typeCached = new WeaponType();
				UpdateMesh();

				if( EngineApp.IsSimulation )
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
			return mode == 1 ? typeCached.Mode1Enabled.Value : typeCached.Mode2Enabled.Value;
		}

		public bool IsFiring( int mode )
		{
			return firing[ mode - 1 ];
		}

		public double GetFiringCurrentTime( int mode )
		{
			return firingCurrentTime[ mode - 1 ];
		}

		public long GetFiringWhoFired( int mode )
		{
			return firingWhoFired[ mode - 1 ];
		}

		protected virtual void OnCanFire( int mode, ref bool canFire )
		{
			if( ( mode == 1 ? typeCached.Mode1BulletType.Value : typeCached.Mode2BulletType.Value ) == null )
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

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void OnFire( int mode )
		{
			var bulletTypeReference = mode == 1 ? typeCached.Mode1BulletType : typeCached.Mode2BulletType;
			//var bulletType = mode == 1 ? typeCached.Mode1BulletType.Value : typeCached.Mode2BulletType.Value;
			if( bulletTypeReference.Value != null )
			{
				var bulletCount = mode == 1 ? typeCached.Mode1BulletCount.Value : typeCached.Mode2BulletCount.Value;
				var bulletSpeed = mode == 1 ? typeCached.Mode1BulletSpeed.Value : typeCached.Mode2BulletSpeed.Value;
				var dispersionAngle = mode == 1 ? typeCached.Mode1BulletDispersionAngle.Value : typeCached.Mode2BulletDispersionAngle.Value;

				for( int nCount = 0; nCount < bulletCount; nCount++ )
				{

					//!!!!event to create, to update

					//!!!!network: alot of transfer (GetByReference string)

					var component = ParentScene.CreateComponent<Bullet>( enabled: false );
					component.BulletType = bulletTypeReference;
					//component.BulletType = bulletType;
					////var component = Parent.CreateComponent( bulletType, enabled: false );

					var objectInSpace = component;// as ObjectInSpace;
					if( objectInSpace != null )
					{
						//creation parameters
						var transform = GetBulletWorldInitialTransform( mode );

						//dispersion angle
						if( dispersionAngle != new Degree( 0 ) )
						{
							if( staticRandom == null )
								staticRandom = new FastRandom();

							Matrix3F.FromRotateByX( staticRandom.NextFloat() * MathEx.PI * 2, out var matrix2 );
							Matrix3F.FromRotateByZ( staticRandom.NextFloat() * (float)dispersionAngle.InRadians(), out var matrix3 );
							Matrix3F.Multiply( ref matrix2, ref matrix3, out var matrix );
							matrix.ToQuaternion( out var rot );

							transform = transform.UpdateRotation( transform.Rotation * rot );
						}

						var velocity = transform.Rotation.GetForward() * bulletSpeed;

						//!!!!no sense now
						////configure transform and velocity
						//var collisionBody = objectInSpace.GetComponent<RigidBody>();
						//if( collisionBody != null )
						//{
						//	collisionBody.Transform = transform;
						//	collisionBody.LinearVelocity = velocity;
						//}
						//else
						objectInSpace.Transform = transform;

						var meshInSpace = objectInSpace as MeshInSpace;
						if( meshInSpace != null && meshInSpace.Collision )
							meshInSpace.PhysicalBodyLinearVelocity = velocity;

						//configure Bullet component
						var bullet = objectInSpace;// as Bullet;
						if( bullet != null )
						{
							////make a reference path from the root component (scene)
							bullet.WhoFired = GetFiringWhoFired( mode );//bullet.OriginalCreator = ReferenceUtility.MakeRootReference( this );
							bullet.SetLinearVelocity( velocity );
						}

						if( NetworkIsSingle )
							CreateMuzzleFlash( mode, transform );

						//!!!!maybe merge with FireSound message
						if( NetworkIsServer && ( mode == 1 ? typeCached.Mode1FireMuzzleFlashParticle : typeCached.Mode2FireMuzzleFlashParticle ).ReferenceOrValueSpecified )
						{
							var writer = BeginNetworkMessageToEveryone( "MuzzleFlash" );
							writer.WriteVariableInt32( mode );
							EndNetworkMessage();
						}

						////muzzle flash
						//var muzzleFlashParticle = mode == 1 ? typeCached.Mode1FireMuzzleFlashParticle.Value : typeCached.Mode2FireMuzzleFlashParticle.Value;
						//if( muzzleFlashParticle != null )
						//{
						//	var obj = Parent.CreateComponent<ParticleSystemInSpace>( enabled: false );
						//	obj.ParticleSystem = muzzleFlashParticle;
						//	obj.MergeSimulationSteps = 1;

						//	var totalFiringTime = mode == 1 ? typeCached.Mode1FiringTotalTime.Value : typeCached.Mode2FiringTotalTime.Value;
						//	obj.RemainingLifetime = totalFiringTime * 2;

						//	if( staticRandom == null )
						//		staticRandom = new FastRandom();

						//	obj.Transform = new Transform( transform.Position, transform.Rotation, obj.TransformV.Scale );


						//	//!!!!slowly GC update manually
						//	ObjectInSpaceUtility.Attach( this, obj, TransformOffset.ModeEnum.Elements );


						//	obj.Enabled = true;
						//}
					}

					component.Enabled = true;
				}
			}
		}

		void CreateMuzzleFlash( int mode, Transform transform )
		{
			var muzzleFlashParticle = mode == 1 ? typeCached.Mode1FireMuzzleFlashParticle.Value : typeCached.Mode2FireMuzzleFlashParticle.Value;
			if( muzzleFlashParticle != null )
			{
				var obj = Parent.CreateComponent<ParticleSystemInSpace>( enabled: false );
				obj.ParticleSystem = muzzleFlashParticle;
				obj.MergeSimulationSteps = 1;

				var totalFiringTime = mode == 1 ? typeCached.Mode1FiringTotalTime.Value : typeCached.Mode2FiringTotalTime.Value;
				obj.RemainingLifetime = totalFiringTime * 2;

				if( staticRandom == null )
					staticRandom = new FastRandom();

				obj.Transform = new Transform( transform.Position, transform.Rotation, obj.TransformV.Scale );


				//!!!!slowly GC update manually
				ObjectInSpaceUtility.Attach( this, obj, TransformOffset.ModeEnum.Elements );


				obj.Enabled = true;
			}
		}

		public delegate void FireDelegate( Weapon sender, int mode );
		public event FireDelegate Fire;

		public bool PerformFire( int mode )
		{
			if( !PerformCanFire( mode ) )
				return false;

			SoundPlay( mode == 1 ? typeCached.Mode1FireSound : typeCached.Mode2FireSound );
			if( NetworkIsServer && ( mode == 1 ? typeCached.Mode1FireSound : typeCached.Mode2FireSound ).ReferenceOrValueSpecified )
			{
				var writer = BeginNetworkMessageToEveryone( "FireSound" );
				writer.WriteVariableInt32( mode );
				EndNetworkMessage();
			}

			OnFire( mode );
			Fire?.Invoke( this, mode );

			return true;
		}

		public delegate void FiringBeginEndDelegate( Weapon sender, int mode );
		public event FiringBeginEndDelegate FiringBeginEvent;
		public event FiringBeginEndDelegate FiringEndEvent;

		public bool FiringBegin( int mode, long whoFired )
		{
			if( IsFiring( mode ) )
				return false;
			if( !PerformCanFire( mode ) )
				return false;

			firing[ mode - 1 ] = true;
			firingCurrentTime[ mode - 1 ] = 0;
			firingWhoFired[ mode - 1 ] = whoFired;

			StartPlayOneAnimation( mode == 1 ? typeCached.Mode1FireAnimation : typeCached.Mode2FireAnimation, mode == 1 ? typeCached.Mode1FireAnimationSpeed : typeCached.Mode2FireAnimationSpeed );
			SoundPlay( mode == 1 ? typeCached.Mode1FiringBeginSound : typeCached.Mode2FiringBeginSound );

			if( NetworkIsServer )
			{
				var writer = BeginNetworkMessageToEveryone( "FiringBegin" );
				writer.WriteVariableInt32( mode );
				EndNetworkMessage();
			}

			FiringBeginEvent?.Invoke( this, mode );

			return true;
		}

		public void FiringEnd( int mode )
		{
			if( !IsFiring( mode ) )
				return;

			firing[ mode - 1 ] = false;
			firingCurrentTime[ mode - 1 ] = 0;
			firingWhoFired[ mode - 1 ] = 0;

			//SoundPlay( mode == 1 ? Mode1FiringEndSound : Mode2FiringEndSound );
			FiringEndEvent?.Invoke( this, mode );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void Simulate( float delta )
		{
			for( int mode = 1; mode <= 2; mode++ )
			{
				if( IsFiring( mode ) )
				{
					var firingFireTime = mode == 1 ? typeCached.Mode1FiringFireTime : typeCached.Mode2FiringFireTime;
					var firingTotalTime = mode == 1 ? typeCached.Mode1FiringTotalTime : typeCached.Mode2FiringTotalTime;

					var before = GetFiringCurrentTime( mode ) < firingFireTime || ( GetFiringCurrentTime( mode ) == 0 && firingFireTime == 0 );
					firingCurrentTime[ mode - 1 ] += delta;
					var after = GetFiringCurrentTime( mode ) < firingFireTime;

					if( before != after )
						PerformFire( mode );

					if( GetFiringCurrentTime( mode ) >= firingTotalTime )
					{
						if( NetworkIsClient )
						{
							var writer = BeginNetworkMessageToServer( "FiringEnd" );
							if( writer != null )
							{
								writer.WriteVariableInt32( mode );
								EndNetworkMessage();
							}
						}
						else
							FiringEnd( mode );
					}
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			Simulate( Time.SimulationDelta );
		}

		//public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		//{
		//	if( Components.Count == 0 && !Mesh.ReferenceSpecified )
		//	{
		//		//Animation controller
		//		{
		//			var controller = CreateComponent<MeshInSpaceAnimationController>();
		//			controller.Name = "Mesh In Space Animation Controller";
		//			controller.InterpolationTime = 0.05;
		//		}
		//	}
		//}

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
				//disable taking from another character or vehicle
				if( Parent as Scene != null )
				{
					info = new InteractiveObjectObjectInfo();
					info.AllowInteract = true;//CanTake;
					info.SelectionTextInfo.Add( Name );
					info.SelectionTextInfo.Add( "Click mouse button to take. Press T to drop." );
				}
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

		public delegate void GetBulletWorldInitialTransformEventDelegate( Weapon sender, ref Transform transform );
		public event GetBulletWorldInitialTransformEventDelegate GetBulletWorldInitialTransformEvent;

		public virtual Transform GetBulletWorldInitialTransform( int mode )
		{
			var transform = mode == 1 ? typeCached.Mode1BulletTransform.Value : typeCached.Mode2BulletTransform.Value;

			var result = Transform.Value * transform;
			GetBulletWorldInitialTransformEvent?.Invoke( this, ref result );
			return result;
		}

		Transform GetHandTransform( HandEnum hand )
		{
			return hand == HandEnum.Left ? typeCached.LeftHandTransform.Value : typeCached.RightHandTransform.Value;
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

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				//var context2 = context.ObjectInSpaceRenderingContext;

				//bool show = ( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this ) || DebugVisualization;
				//if( show )
				if( DebugVisualization )
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
			if( animationControllerCached == null )
				animationControllerCached = GetComponent<MeshInSpaceAnimationController>( onlyEnabledInHierarchy: true );
			return animationControllerCached;
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
					float speed = 1;
					bool autoRewind = true;

					if( animation == null )
						animation = typeCached.IdleAnimation;

					//play one animation
					if( playOneAnimation != null )
					{
						animation = playOneAnimation;
						speed = playOneAnimationSpeed;
						autoRewind = false;
					}

					//!!!!GC

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
			playOneAnimationSpeed = (float)speed;
			if( playOneAnimation != null && playOneAnimationSpeed == 0 )
				playOneAnimationSpeed = 0.000001f;

			if( playOneAnimation != null )
			{
				playOneAnimationRemainingTime = (float)playOneAnimation.Length / playOneAnimationSpeed;

				var controller = GetAnimationController();
				if( controller != null && playOneAnimationRemainingTime > controller.InterpolationTime.Value )
					playOneAnimationRemainingTime -= (float)controller.InterpolationTime.Value;
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

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "FireSound" )
			{
				var mode = reader.ReadVariableInt32();
				if( !reader.Complete() )
					return false;
				SoundPlay( mode == 1 ? typeCached.Mode1FireSound : typeCached.Mode2FireSound );
			}
			else if( message == "FiringBegin" )
			{
				var mode = reader.ReadVariableInt32();
				if( !reader.Complete() )
					return false;
				StartPlayOneAnimation( mode == 1 ? typeCached.Mode1FireAnimation : typeCached.Mode2FireAnimation, mode == 1 ? typeCached.Mode1FireAnimationSpeed : typeCached.Mode2FireAnimationSpeed );
				SoundPlay( mode == 1 ? typeCached.Mode1FiringBeginSound : typeCached.Mode2FiringBeginSound );
			}
			else if( message == "MuzzleFlash" )
			{
				var mode = reader.ReadVariableInt32();
				if( !reader.Complete() )
					return false;
				var transform = GetBulletWorldInitialTransform( mode );
				CreateMuzzleFlash( mode, transform );
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
						var mode = reader.ReadVariableInt32();
						if( !reader.Complete() )
							return false;
						FiringBegin( mode, client.User.UserID );
					}
					else if( message == "FiringEnd" )
					{
						var mode = reader.ReadVariableInt32();
						if( !reader.Complete() )
							return false;
						FiringEnd( mode );
					}
				}
			}

			return true;
		}

		protected override void OnComponentRemoved( Component component )
		{
			base.OnComponentRemoved( component );

			if( animationControllerCached == component )
				animationControllerCached = null;
		}
	}
}
