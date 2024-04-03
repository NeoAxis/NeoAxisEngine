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
	public class Weapon : MeshInSpace, ItemInterface, InteractiveObjectInterface
	{
		static FastRandom staticRandom = new FastRandom( 0 );

		WeaponType typeCached = new WeaponType();

		bool[] firing = new bool[ 3 ];
		float[] firingCurrentTime = new float[ 3 ];
		long[] firingWhoFired = new long[ 3 ];//network UserID
		bool[] firingMeleeCollided = new bool[ 3 ];

		//play one animation
		Animation playOneAnimation = null;
		float playOneAnimationSpeed = 1;
		float playOneAnimationRemainingTime;

		//optimization
		MeshInSpaceAnimationController animationControllerCached;

		/////////////////////////////////////////

		public const string WeaponTypeDefault = @"Content\Weapons\Default\Default Weapon.weapontype";

		/// <summary>
		/// The type of the weapon.
		/// </summary>
		[DefaultValueReference( WeaponTypeDefault )]
		public Reference<WeaponType> WeaponType
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
							typeCached = new WeaponType();

						//update mesh
						if( EnabledInHierarchyAndIsInstance )
							UpdateMesh();
					}
					finally { _weaponType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="WeaponType"/> property value changes.</summary>
		public event Action<Weapon> WeaponTypeChanged;
		ReferenceField<WeaponType> _weaponType = new Reference<WeaponType>( null, WeaponTypeDefault );

		/// <summary>
		/// Whether to display initial position and direction of a bullet and places of hands.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( this, ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugVisualization"/> property value changes.</summary>
		public event Action<Weapon> DebugVisualizationChanged;
		ReferenceField<bool> _debugVisualization = false;

		/////////////////////////////////////////

		[Browsable( false )]
		public WeaponType TypeCached
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
			Mesh = TypeCached.Mesh;

			//enable collision when collision definiation is exists and when not taken by character or other object
			var meshValue = Mesh.Value;

			//!!!!Parent as Character == null

			Collision = meshValue != null && meshValue.GetComponent<RigidBody>( "Collision Definition" ) != null && Parent as Character == null;
			//Collision = meshValue != null && meshValue.GetComponent<RigidBody>( "Collision Definition" ) != null && Parent as ObjectInSpace == null;
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

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			TickAnimate( delta );
		}

		public bool IsModeEnabled( int mode )
		{
			return mode == 1 ? TypeCached.Mode1Enabled.Value : TypeCached.Mode2Enabled.Value;
		}

		public bool IsFiring( int mode )
		{
			return firing[ mode - 1 ];
		}

		public bool IsFiringAnyMode()
		{
			return IsFiring( 1 ) || IsFiring( 2 );
		}

		public double GetFiringCurrentTime( int mode )
		{
			return firingCurrentTime[ mode - 1 ];
		}

		public long GetFiringWhoFired( int mode )
		{
			return firingWhoFired[ mode - 1 ];
		}

		public bool IsFiringMeleeCollided( int mode )
		{
			return firingMeleeCollided[ mode - 1 ];
		}

		protected virtual void OnCanFire( int mode, ref bool canFire )
		{
			if( TypeCached.WayToUse.Value == NeoAxis.WeaponType.WayToUseEnum.Rifle )
			{
				if( ( mode == 1 ? TypeCached.Mode1BulletType.Value : TypeCached.Mode2BulletType.Value ) == null )
					canFire = false;
			}
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
			var bulletTypeReference = mode == 1 ? TypeCached.Mode1BulletType : TypeCached.Mode2BulletType;
			//var bulletType = mode == 1 ? TypeCached.Mode1BulletType.Value : TypeCached.Mode2BulletType.Value;
			var bulletType = bulletTypeReference.Value;
			if( bulletType != null )
			{
				var bulletCount = mode == 1 ? TypeCached.Mode1BulletCount.Value : TypeCached.Mode2BulletCount.Value;
				var bulletSpeed = mode == 1 ? TypeCached.Mode1BulletSpeed.Value : TypeCached.Mode2BulletSpeed.Value;
				var dispersionAngle = mode == 1 ? TypeCached.Mode1BulletDispersionAngle.Value : TypeCached.Mode2BulletDispersionAngle.Value;


				var initialTransform = GetBulletWorldInitialTransform( mode );

				var initialTransformFixed = initialTransform;

				//fix direction for character in first person camera
				{
					var character = Parent as Character;
					if( character != null && character.CurrentLookToPosition.HasValue )
					{
						var rot = Quaternion.FromDirectionZAxisUp( character.CurrentLookToPosition.Value - initialTransform.Position );
						initialTransformFixed = initialTransformFixed.UpdateRotation( rot );
					}
				}


				if( !NetworkIsClient )
				{
					for( int nCount = 0; nCount < bulletCount; nCount++ )
					{

						//!!!!event to create, to update

						//!!!!network: alot of transfer (GetByReference string)

						var component = ParentScene.CreateComponent<Bullet>( enabled: false );
						component.BulletType = bulletTypeReference;
						//component.BulletType = bulletType;
						////var component = Parent.CreateComponent( bulletType, enabled: false );

						//set Collision
						if( !bulletType.existsCollisionCached.HasValue )
							bulletType.existsCollisionCached = bulletType.Mesh.Value?.GetComponent<RigidBody>( "Collision Definition" ) != null;
						if( bulletType.existsCollisionCached.Value )
							component.Collision = true;

						var objectInSpace = component;// as ObjectInSpace;
						if( objectInSpace != null )
						{
							//creation parameters
							var transform = initialTransformFixed;

							//dispersion angle
							if( dispersionAngle != new Degree( 0 ) )
							{
								Matrix3F.FromRotateByX( staticRandom.NextFloat() * MathEx.PI * 2, out var matrix2 );
								Matrix3F.FromRotateByZ( staticRandom.NextFloat() * (float)dispersionAngle.InRadians(), out var matrix3 );
								Matrix3F.Multiply( ref matrix2, ref matrix3, out var matrix );
								matrix.ToQuaternion( out var rot );

								transform = transform.UpdateRotation( transform.Rotation * rot );
							}

							var velocity = transform.Rotation.GetForward() * bulletSpeed;

							//no sense now
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
						}

						component.Enabled = true;
					}
				}

				if( NetworkIsSingle || NetworkIsClient )
					CreateMuzzleFlash( mode, initialTransform );
			}
		}

		void CreateMuzzleFlash( int mode, Transform transform )
		{
			var muzzleFlashParticle = mode == 1 ? TypeCached.Mode1FireMuzzleFlashParticle.Value : TypeCached.Mode2FireMuzzleFlashParticle.Value;
			if( muzzleFlashParticle != null )
			{
				var obj = Parent.CreateComponent<ParticleSystemInSpace>( enabled: false );
				obj.ParticleSystem = muzzleFlashParticle;
				obj.MergeSimulationSteps = 1;

				var totalFiringTime = mode == 1 ? TypeCached.Mode1FiringTotalTime.Value : TypeCached.Mode2FiringTotalTime.Value;
				obj.RemainingLifetime = totalFiringTime * 2;

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

			SoundPlay( mode == 1 ? TypeCached.Mode1FireSound : TypeCached.Mode2FireSound );

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
			firingMeleeCollided[ mode - 1 ] = false;

			StartPlayOneAnimation( mode == 1 ? TypeCached.Mode1FireAnimation : TypeCached.Mode2FireAnimation, mode == 1 ? TypeCached.Mode1FireAnimationSpeed : TypeCached.Mode2FireAnimationSpeed );
			SoundPlay( mode == 1 ? TypeCached.Mode1FiringBeginSound : TypeCached.Mode2FiringBeginSound );

			var character = Parent as Character;
			if( character != null )
				character.WeaponFiringBegin( this, mode );

			if( NetworkIsServer )
			{
				var writer = BeginNetworkMessageToEveryone( "FiringBegin" );
				writer.WriteVariableInt32( mode );
				EndNetworkMessage();
			}

			FiringBeginEvent?.Invoke( this, mode );

			return true;
		}

		//!!!!send often. need know network loop time to send only at end of firing. or send start and end
		public void FiringBeginClient( int mode )
		{
			//!!!!
			//if( IsFiring( mode ) )
			//	return;

			var writer = BeginNetworkMessageToServer( "FiringBeginFromClient" );
			if( writer != null )
			{
				writer.WriteVariableInt32( mode );
				EndNetworkMessage();
			}
		}

		public void FiringEnd( int mode )
		{
			if( !IsFiring( mode ) )
				return;

			firing[ mode - 1 ] = false;
			firingCurrentTime[ mode - 1 ] = 0;
			firingWhoFired[ mode - 1 ] = 0;
			firingMeleeCollided[ mode - 1 ] = false;

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
					var firingFireTime = mode == 1 ? TypeCached.Mode1FiringFireTime : TypeCached.Mode2FiringFireTime;
					var firingTotalTime = mode == 1 ? TypeCached.Mode1FiringTotalTime : TypeCached.Mode2FiringTotalTime;

					var before = GetFiringCurrentTime( mode ) < firingFireTime || ( GetFiringCurrentTime( mode ) == 0 && firingFireTime == 0 );
					firingCurrentTime[ mode - 1 ] += delta;
					var after = GetFiringCurrentTime( mode ) < firingFireTime;

					if( before != after )
						PerformFire( mode );

					if( GetFiringCurrentTime( mode ) >= firingTotalTime )
						FiringEnd( mode );


					//melee weapon
					if( TypeCached.WayToUse.Value == NeoAxis.WeaponType.WayToUseEnum.OneHandedMelee && IsFiring( mode ) && GetFiringCurrentTime( mode ) >= firingFireTime && !IsFiringMeleeCollided( mode ) )
					{
						var scene = ParentScene;
						var mesh = Mesh.Value;

						if( scene != null )
						{
							PhysicsVolumeTestItem volumeTestItem = null;
							Vector3 applyForceCenter = Vector3.Zero;
							Vector3 applyForceDirection = Vector3.Zero;

							var detectionMode = mode == 1 ? TypeCached.Mode1MeleeCollisionDetectionMethod.Value : TypeCached.Mode2MeleeCollisionDetectionMethod.Value;

							if( detectionMode == NeoAxis.WeaponType.MeleeCollisionDetectionMethodEnum.Mesh )
							{
								//mesh collision detection mode

								if( mesh?.Result != null )
								{
									var localBox = new Box( mesh.Result.SpaceBounds.BoundingBox );
									var worldBox = localBox * TransformV.ToMatrix4();
									volumeTestItem = new PhysicsVolumeTestItem( worldBox, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.OneForEach );

									applyForceCenter = worldBox.Center;
									//!!!!impl
									applyForceDirection = Vector3.Zero;
								}
							}
							else if( detectionMode == NeoAxis.WeaponType.MeleeCollisionDetectionMethodEnum.Volume )
							{
								//sphere volume collision detection mode

								var character = Parent as Character;
								if( character != null )
								{
									var localSphere = mode == 1 ? TypeCached.Mode1MeleeCollisionVolume.Value : TypeCached.Mode2MeleeCollisionVolume.Value;

									var center = character.TransformV * localSphere.Center;
									var radius = localSphere.Radius * character.GetScaleFactor();

									var sphere = new Sphere( center, radius );

									volumeTestItem = new PhysicsVolumeTestItem( sphere, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.OneForEach );

									applyForceCenter = center;
									applyForceDirection = character.TransformV.Rotation.GetForward();
								}
							}

							if( volumeTestItem != null )
							{
								scene.PhysicsVolumeTest( volumeTestItem );

								Scene.PhysicsWorldClass.Body collidedBody = null;
								IProcessDamage collidedObject = null;

								foreach( var resultItem in volumeTestItem.Result )
								{
									if( resultItem.Body.Owner == this || resultItem.Body.Owner == Parent )
										continue;

									collidedBody = resultItem.Body;

									var owner = resultItem.Body.Owner as Component;
									if( owner != null )
									{
										var processDamage = owner as IProcessDamage;
										if( processDamage == null )
											processDamage = owner.Parent as IProcessDamage;

										if( processDamage != null && processDamage != Parent )
										{
											collidedObject = processDamage;
											break;
										}
									}
								}

								if( collidedBody != null )
								{
									//collision sound
									SoundPlay( mode == 1 ? TypeCached.Mode1MeleeCollisionSound : TypeCached.Mode2MeleeCollisionSound );

									//impulse
									var impulse = mode == 1 ? TypeCached.Mode1MeleeImpulse.Value : TypeCached.Mode2MeleeImpulse.Value;
									if( applyForceDirection != Vector3.Zero && impulse != 0 )
									{
										var tr = new Matrix4( collidedBody.Rotation.ToMatrix3(), collidedBody.Position );
										tr.GetInverse( out var invertTr );
										var localPosition = invertTr * applyForceCenter;

										//!!!!impl affect characters

										collidedBody.ApplyForce( ( applyForceDirection * impulse ).ToVector3F(), localPosition.ToVector3F() );
									}

									//damage
									if( collidedObject != null )
									{
										var damage = mode == 1 ? TypeCached.Mode1MeleeDamage.Value : TypeCached.Mode2MeleeDamage.Value;
										if( damage != 0 )
											collidedObject.ProcessDamage( GetFiringWhoFired( mode ), (float)damage, null );
									}

									firingMeleeCollided[ mode - 1 ] = true;
								}
							}
						}
					}
				}
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

		public delegate void SoundPlayBeforeDelegate( Weapon sender, ref Sound sound, ref bool handled );
		public event SoundPlayBeforeDelegate SoundPlayBefore;

		public virtual void SoundPlay( Sound sound )
		{
			var handled = false;
			SoundPlayBefore?.Invoke( this, ref sound, ref handled );
			if( handled )
				return;

			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		protected override bool OnSpaceBoundsUpdateIncludeChildren()
		{
			return true;
		}

		public delegate void ObjectInteractionGetInfoEventDelegate( Weapon sender, GameMode gameMode, ref InteractiveObjectObjectInfo info );
		public event ObjectInteractionGetInfoEventDelegate ObjectInteractionGetInfoEvent;

		public virtual void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			//enable an interaction context to take the object by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character;
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
					var character = gameMode.ObjectControlledByPlayer.Value as Character;
					if( character != null && character.ItemCanTake( gameMode, this ) )
					{
						if( NetworkIsClient )
						{
							var activate = character.GetActiveItem() == null;
							character.ItemTakeAndActivateClient( this, activate );
						}
						else
						{
							if( character.ItemTake( gameMode, this ) )
							{
								if( character.GetActiveItem() == null )
									character.ItemActivate( gameMode, this );
							}
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

		public delegate void GetBulletWorldInitialTransformEventDelegate( Weapon sender, ref Transform transform );
		public event GetBulletWorldInitialTransformEventDelegate GetBulletWorldInitialTransformEvent;

		public virtual Transform GetBulletWorldInitialTransform( int mode )
		{
			var transform = mode == 1 ? TypeCached.Mode1BulletTransform.Value : TypeCached.Mode2BulletTransform.Value;

			//!!!!also add event for type?
			var result = Transform.Value * transform;
			GetBulletWorldInitialTransformEvent?.Invoke( this, ref result );
			return result;
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum && DebugVisualization )
			{
				var renderer = context.Owner.Simple3DRenderer;
				if( renderer != null )
				{
					if( TypeCached.WayToUse.Value == NeoAxis.WeaponType.WayToUseEnum.Rifle )
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

						{
							var tr = TransformV;

							renderer.SetColor( new ColorValue( 0, 1, 0 ) );
							if( TypeCached.PistolGripBottom.Value != TypeCached.PistolGripTop.Value )
								renderer.AddLine( tr * TypeCached.PistolGripBottom.Value, tr * TypeCached.PistolGripTop.Value );
							if( TypeCached.HandguardNear.Value != TypeCached.HandguardFar.Value )
								renderer.AddLine( tr * TypeCached.HandguardNear.Value, tr * TypeCached.HandguardFar.Value );
						}
					}
				}
			}
		}

		public MeshInSpaceAnimationController GetAnimationController( bool createInSimulationIfNotExists )
		{
			if( animationControllerCached == null )
			{
				animationControllerCached = GetComponent<MeshInSpaceAnimationController>();
				if( animationControllerCached == null && !NetworkIsClient && EngineApp.IsSimulation && createInSimulationIfNotExists )
				{
					animationControllerCached = CreateComponent<MeshInSpaceAnimationController>();
					animationControllerCached.Name = "Animation Controller";
				}
			}
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

			//get current animation

			MeshInSpaceAnimationController.AnimationStateClass state = null;

			var character = Parent as Character;
			if( character != null )
			{
				Animation animation = null;
				float speed = 1;
				bool autoRewind = true;

				if( animation == null )
					animation = TypeCached.IdleAnimation;

				//play one animation
				if( playOneAnimation != null )
				{
					animation = playOneAnimation;
					speed = playOneAnimationSpeed;
					autoRewind = false;
				}

				//!!!!GC

				state = new MeshInSpaceAnimationController.AnimationStateClass();
				state.Animations.Add( new MeshInSpaceAnimationController.AnimationStateClass.AnimationItem() { Animation = animation, Speed = speed, AutoRewind = autoRewind } );
			}

			//update controller
			var controller = GetAnimationController( state != null );
			controller?.SetAnimationState( state, true );
		}

		public delegate void StartPlayOneAnimationBeforeDelegate( Weapon sender, ref Animation animation, ref double speed, ref bool resetSameAnimation, ref bool handled );
		public event StartPlayOneAnimationBeforeDelegate StartPlayOneAnimationBefore;

		public virtual void StartPlayOneAnimation( Animation animation, double speed = 1.0, bool resetSameAnimation = false )
		{
			var handled = false;
			StartPlayOneAnimationBefore?.Invoke( this, ref animation, ref speed, ref resetSameAnimation, ref handled );
			if( handled )
				return;

			if( !resetSameAnimation && animation != null && playOneAnimation == animation && playOneAnimationSpeed == speed )
				return;

			playOneAnimation = animation;
			playOneAnimationSpeed = (float)speed;
			if( playOneAnimation != null && playOneAnimationSpeed == 0 )
				playOneAnimationSpeed = 0.000001f;

			if( playOneAnimation != null )
			{
				playOneAnimationRemainingTime = (float)playOneAnimation.Length / playOneAnimationSpeed;

				var controller = GetAnimationController( true );
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

			if( message == "FiringBegin" )
			{
				var mode = reader.ReadVariableInt32();
				if( !reader.Complete() )
					return false;
				FiringBegin( mode, 0 );
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
				if( networkLogic != null && GetAllParents().Contains( networkLogic.ServerGetObjectControlledByUser( client.User, true ) ) )
				{
					if( message == "FiringBeginFromClient" )
					{
						var mode = reader.ReadVariableInt32();
						if( !reader.Complete() )
							return false;
						FiringBegin( mode, client.User.UserID );
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

		public virtual void GetInventoryImage( out ImageComponent image, out object anyData )
		{
			image = TypeCached.InventoryImage;
			anyData = null;
		}

		[Browsable( false )]
		public Reference<double> ItemCount
		{
			get { return 1.0; }
			set { }
		}
	}
}
