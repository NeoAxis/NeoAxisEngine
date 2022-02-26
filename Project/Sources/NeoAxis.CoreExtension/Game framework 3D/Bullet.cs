// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A basic class for making bullets.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Bullet", -5000 )]
	[NewObjectDefaultName( "Bullet" )]
	public class Bullet : MeshInSpace
	{
		bool physicalBodyMode;
		bool firstSimulationStep = true;

		/// <summary>
		/// The gravity multiplier when the bullet has no physical body.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 2 )]
		public Reference<double> GravityFactorNoPhysicalBodyMode
		{
			get { if( _gravityFactorNoPhysicalBodyMode.BeginGet() ) GravityFactorNoPhysicalBodyMode = _gravityFactorNoPhysicalBodyMode.Get( this ); return _gravityFactorNoPhysicalBodyMode.value; }
			set { if( _gravityFactorNoPhysicalBodyMode.BeginSet( ref value ) ) { try { GravityFactorNoPhysicalBodyModeChanged?.Invoke( this ); } finally { _gravityFactorNoPhysicalBodyMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GravityFactorNoPhysicalBodyMode"/> property value changes.</summary>
		public event Action<Bullet> GravityFactorNoPhysicalBodyModeChanged;
		ReferenceField<double> _gravityFactorNoPhysicalBodyMode = 1.0;

		/// <summary>
		/// The mass when the bullet has no physical body. The mass is used for impulse calculation when hit.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> MassNoPhysicalBodyMode
		{
			get { if( _massNoPhysicalBodyMode.BeginGet() ) MassNoPhysicalBodyMode = _massNoPhysicalBodyMode.Get( this ); return _massNoPhysicalBodyMode.value; }
			set { if( _massNoPhysicalBodyMode.BeginSet( ref value ) ) { try { MassNoPhysicalBodyModeChanged?.Invoke( this ); } finally { _massNoPhysicalBodyMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MassNoPhysicalBodyMode"/> property value changes.</summary>
		public event Action<Bullet> MassNoPhysicalBodyModeChanged;
		ReferenceField<double> _massNoPhysicalBodyMode = 1.0;

		/// <summary>
		/// Whether the bullet must be destroyed on hit.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> DestroyOnHit
		{
			get { if( _destroyOnHit.BeginGet() ) DestroyOnHit = _destroyOnHit.Get( this ); return _destroyOnHit.value; }
			set { if( _destroyOnHit.BeginSet( ref value ) ) { try { DestroyOnHitChanged?.Invoke( this ); } finally { _destroyOnHit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DestroyOnHit"/> property value changes.</summary>
		public event Action<Bullet> DestroyOnHitChanged;
		ReferenceField<bool> _destroyOnHit = true;

		/// <summary>
		/// The list of objects to create when the bullet get a collision.
		/// </summary>
		[Serialize]
		[Cloneable]
		public ReferenceList<Metadata.TypeInfo> HitObjects
		{
			get { return _hitObjects; }
		}
		public delegate void HitObjectsChangedDelegate( Bullet sender );
		public event HitObjectsChangedDelegate HitObjectsChanged;
		ReferenceList<Metadata.TypeInfo> _hitObjects;

		/// <summary>
		/// The strength of the damage. Affects to Health parameter.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> HitDamage
		{
			get { if( _hitDamage.BeginGet() ) HitDamage = _hitDamage.Get( this ); return _hitDamage.value; }
			set { if( _hitDamage.BeginSet( ref value ) ) { try { HitDamageChanged?.Invoke( this ); } finally { _hitDamage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HitDamage"/> property value changes.</summary>
		public event Action<Bullet> HitDamageChanged;
		ReferenceField<double> _hitDamage = 0.0;

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

		///////////////////////////////////////////////

		/// <summary>
		/// The linear velocity when the bullet has no physical body.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> LinearVelocityNoPhysicalBodyMode
		{
			get { if( _linearVelocityNoPhysicalBodyMode.BeginGet() ) LinearVelocityNoPhysicalBodyMode = _linearVelocityNoPhysicalBodyMode.Get( this ); return _linearVelocityNoPhysicalBodyMode.value; }
			set { if( _linearVelocityNoPhysicalBodyMode.BeginSet( ref value ) ) { try { LinearVelocityNoPhysicalBodyModeChanged?.Invoke( this ); } finally { _linearVelocityNoPhysicalBodyMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearVelocityNoPhysicalBodyMode"/> property value changes.</summary>
		public event Action<Bullet> LinearVelocityNoPhysicalBodyModeChanged;
		ReferenceField<Vector3> _linearVelocityNoPhysicalBodyMode = Vector3.Zero;

		/// <summary>
		/// The creator of the bullet or the creator of a original creator.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( null )]
		public Reference<Component> OriginalCreator
		{
			get { if( _originalCreator.BeginGet() ) OriginalCreator = _originalCreator.Get( this ); return _originalCreator.value; }
			set { if( _originalCreator.BeginSet( ref value ) ) { try { OriginalCreatorChanged?.Invoke( this ); } finally { _originalCreator.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OriginalCreator"/> property value changes.</summary>
		public event Action<Bullet> OriginalCreatorChanged;
		ReferenceField<Component> _originalCreator = null;

		///// <summary>
		///// Time lived.
		///// </summary>
		//[Browsable( false )]
		//[DefaultValue( 0.0 )]
		//public Reference<double> Time
		//{
		//	get { if( _time.BeginGet() ) Time = _time.Get( this ); return _time.value; }
		//	set { if( _time.BeginSet( ref value ) ) { try { TimeChanged?.Invoke( this ); } finally { _time.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Time"/> property value changes.</summary>
		//public event Action<Bullet> TimeChanged;
		//ReferenceField<double> _time = 0.0;

		///////////////////////////////////////////////

		public Bullet()
		{
			_hitObjects = new ReferenceList<Metadata.TypeInfo>( this, () => HitObjectsChanged?.Invoke( this ) );
			//_lifetimeEndObjects = new ReferenceList<Metadata.TypeInfo>( this, () => LifetimeEndObjectsChanged?.Invoke( this ) );
		}

		////////////////

		protected virtual void OnHitBefore( PhysicalBody hitBody, Vector3 hitPosition, Vector3 hitNormal, object anyData, ref bool hitHandled ) { }
		public delegate void HitBeforeDelegate( Bullet sender, PhysicalBody hitBody, Vector3 hitPosition, Vector3 hitNormal, object anyData, ref bool hitHandled );
		public event HitBeforeDelegate HitBefore;
		//protected virtual void OnHitBefore( PhysicsRayTestItem.ResultItem rayCastResult, object anyData, ref bool hitHandled ) { }
		//public delegate void HitBeforeDelegate( Bullet sender, PhysicsRayTestItem.ResultItem rayCastResult, object anyData, ref bool hitHandled );
		//public event HitBeforeDelegate HitBefore;

		////////////////

		protected void OnHitObjectsBeforeCreate( bool[] allowCreateObjects ) { }
		public delegate void HitObjectsBeforeCreateDelegate( Bullet sender, bool[] allowCreateObjects );
		public event HitObjectsBeforeCreateDelegate HitObjectsBeforeCreate;

		////////////////

		protected virtual void OnHitObjectCreate( Component obj ) { }
		public delegate void HitObjectCreateDelegate( Bullet sender, Component obj );
		public event HitObjectCreateDelegate HitObjectCreate;

		////////////////

		protected virtual void OnHit( PhysicalBody hitBody, Vector3 hitPosition, Vector3 hitNormal, object anyData ) { }
		public delegate void HitDelegate( Bullet sender, PhysicalBody hitBody, Vector3 hitPosition, Vector3 hitNormal, object anyData );
		public event HitDelegate Hit;
		//protected virtual void OnHit( PhysicsRayTestItem.ResultItem rayCastResult, object anyData ) { }
		//public delegate void HitDelegate( Bullet sender, PhysicsRayTestItem.ResultItem rayCastResult, object anyData );
		//public event HitDelegate Hit;

		////////////////

		public void PerformHit( PhysicalBody hitBody, Vector3 hitPosition, Vector3 hitNormal, object anyData )
		//public void PerformHit( PhysicsRayTestItem.ResultItem rayCastResult, object anyData )
		{
			var handled = false;
			OnHitBefore( hitBody, hitPosition, hitNormal, anyData, ref handled );
			if( handled )
				return;
			HitBefore?.Invoke( this, hitBody, hitPosition, hitNormal, anyData, ref handled );
			if( handled )
				return;
			//var handled = false;
			//OnHitBefore( rayCastResult, anyData, ref handled );
			//if( handled )
			//	return;
			//HitBefore?.Invoke( this, rayCastResult, anyData, ref handled );
			//if( handled )
			//	return;

			FastRandom random = null;

			//HitObjectsBeforeCreate
			var allowCreateObjects = new bool[ HitObjects.Count ];
			for( int n = 0; n < allowCreateObjects.Length; n++ )
				allowCreateObjects[ n ] = true;
			OnHitObjectsBeforeCreate( allowCreateObjects );
			HitObjectsBeforeCreate?.Invoke( this, allowCreateObjects );

			var hitRigidBody = hitBody as RigidBody;
			//var hitBody = rayCastResult.Body as RigidBody;

			for( int nHitObject = 0; nHitObject < HitObjects.Count; nHitObject++ )
			{
				if( !allowCreateObjects[ nHitObject ] )
					continue;
				var type = HitObjects[ nHitObject ].Value;
				if( type == null )
					continue;

				if( MetadataManager.GetTypeOfNetType( typeof( Sound ) ).IsAssignableFrom( type ) )
				{
					//sound
					ParentScene?.SoundPlay( type.Name, TransformV.Position );
				}
				else
				{
					//create component

					Component obj = null;

					//create ParticleSystemInSpace
					if( MetadataManager.GetTypeOfNetType( typeof( ParticleSystem ) ).IsAssignableFrom( type ) )
					{
						var p = Parent.CreateComponent<ParticleSystemInSpace>( enabled: false );
						p.ParticleSystem = ReferenceUtility.MakeReference( type.Name );
						obj = p;
					}

					//default creation
					if( obj == null )
						obj = Parent.CreateComponent( type, enabled: false );


					//configure

					var objectInSpace = obj as ObjectInSpace;
					if( objectInSpace != null )
					{
						var bulletTransform = TransformV;
						objectInSpace.Transform = new Transform( bulletTransform.Position, bulletTransform.Rotation, objectInSpace.TransformV.Scale );

						//decal
						var decal = objectInSpace as Decal;
						if( decal != null )
						{
							//create only for rigid bodies, don't create for characters
							if( hitRigidBody != null && hitRigidBody.Parent as Character == null )
							{
								var fixedHitPosition = hitPosition;

								//use mesh geometry instead of collision body for greater accuracy
								if( hitRigidBody.Name == "Collision Body" )
								{
									var parent = hitRigidBody.Parent as MeshInSpace;
									if( parent != null )
									{
										var mesh = parent.Mesh.Value;
										if( mesh != null && mesh.Result != null )
										{
											var distance = 0.1;

											var ray = new Ray( hitPosition + hitNormal * distance, -hitNormal * distance * 2 );

											parent.TransformV.ToMatrix4().GetInverse( out var transInv );
											var localRay = transInv * ray;

											if( mesh.Result.RayCast( localRay, NeoAxis.Mesh.CompiledData.RayCastMode.Auto, true, out var scale, out _ ) )
												fixedHitPosition = ray.GetPointOnRay( scale );
										}
									}
								}


								if( random == null )
									random = new FastRandom();

								var rot = Quaternion.FromDirectionZAxisUp( -hitNormal );
								rot *= Quaternion.FromRotateByX( random.Next( 0, MathEx.PI * 2 ) );

								objectInSpace.Transform = new Transform( fixedHitPosition, rot, objectInSpace.TransformV.Scale );

								//attach to the body when it is dynamic
								if( hitRigidBody.MotionType.Value != RigidBody.MotionTypeEnum.Static )
								{
									//change parent
									obj.RemoveFromParent( false );
									hitRigidBody.AddComponent( obj );

									ObjectInSpaceUtility.Attach( hitRigidBody, objectInSpace, TransformOffset.ModeEnum.Matrix );
								}
							}
							else
								objectInSpace.Dispose();
						}

						//particle system
						var particleInSpace = objectInSpace as ParticleSystemInSpace;
						if( particleInSpace != null )
						{
							//if( rayCastResult != null )
							//{
							if( random == null )
								random = new FastRandom();

							var rot = Quaternion.FromDirectionZAxisUp( hitNormal );
							rot *= Quaternion.FromRotateByX( random.Next( 0, MathEx.PI * 2 ) );

							objectInSpace.Transform = new Transform( hitPosition, rot, objectInSpace.TransformV.Scale );
							//}
						}

						//explosion
						var explosion = objectInSpace as Explosion;
						if( explosion != null )
							explosion.OriginalCreator = OriginalCreator;
					}

					OnHitObjectCreate( obj );
					if( !obj.RemoveFromParentQueued && !obj.Disposed )
						HitObjectCreate?.Invoke( this, obj );

					if( !obj.RemoveFromParentQueued && !obj.Disposed )
						obj.Enabled = true;
				}
			}

			//add an impulse to the hit body
			if( !PhysicalBodyMode )//if( rayCastResult != null )
			{
				if( !PhysicalBodyMode && hitRigidBody != null && hitRigidBody.MotionType.Value == RigidBody.MotionTypeEnum.Dynamic )
				{
					var mass = MassNoPhysicalBodyMode.Value;
					if( mass > 0 )
					{
						//!!!!slowly
						hitRigidBody.TransformV.ToMatrix4().GetInverse( out var invertTr );
						var localPosition = invertTr * TransformV.Position;
						hitRigidBody.ApplyForce( LinearVelocityNoPhysicalBodyMode.Value/*GetLinearVelocity()*/ * mass, localPosition );
					}
				}
			}

			//damage
			if( hitBody != null )
			{
				var parent = hitBody.Parent;
				if( parent != null && parent is ObjectInSpace )
				{
					var health = parent.PropertyGet<double>( "Health" );
					if( health > 0 )
					{
						health -= HitDamage.Value;
						parent.PropertySet( "Health", health );

						//get a new value
						var newHealth = parent.PropertyGet<double>( "Health" );
						if( newHealth <= 0 )
						{
							//!!!!for character switch to ragdoll

							parent.RemoveFromParent( true );
						}
					}
				}
			}

			OnHit( hitBody, hitPosition, hitNormal, anyData );
			Hit?.Invoke( this, hitBody, hitPosition, hitNormal, anyData );
			//OnHit( rayCastResult, anyData );
			//Hit?.Invoke( this, rayCastResult, anyData );

			//destroy the bullet
			if( DestroyOnHit )
				RemoveFromParent( true );
		}

		////////////////

		public RigidBody GetRigidBody()
		{
			return GetComponent<RigidBody>();
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				var body = GetRigidBody();
				physicalBodyMode = body != null;

				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation && PhysicalBodyMode )
					body.ContactsCollect = true;
			}
		}

		[Browsable( false )]
		bool PhysicalBodyMode
		{
			get { return physicalBodyMode; }
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( PhysicalBodyMode )
				SimulationStepPhysicalBodyMode( NeoAxis.Time.SimulationDelta );
			else
				SimulationStepNoPhysicalBodyMode( NeoAxis.Time.SimulationDelta );

			firstSimulationStep = false;
		}

		protected virtual bool IsOutsideSceneBounds( Scene scene )
		{
			var position = TransformV.Position;

			var bounds = scene.BoundsWhenSimulationStarted;
			bounds.Expand( 100 );

			if( bounds.Contains( ref position ) )
				return false;

			return true;
		}

		protected virtual void SimulationStepNoPhysicalBodyMode( float delta )
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			//apply gravity to velocity
			var gravityFactor = GravityFactorNoPhysicalBodyMode.Value;
			if( gravityFactor != 0 )
				LinearVelocityNoPhysicalBodyMode += scene.Gravity.Value * gravityFactor * delta;

			var tr = Transform.Value;
			var pos = tr.Position;
			var rot = tr.Rotation;
			var scl = tr.Scale;

			var vector = LinearVelocityNoPhysicalBodyMode.Value * delta;

			//update rotation
			if( vector != Vector3.Zero )
				rot = Quaternion.LookAt( vector.GetNormalize(), rot.GetUp() );

			//prevent skipping fast bodies
			var backOffset = Vector3.Zero;
			if( !firstSimulationStep )
				backOffset = vector * 0.1;

			var ray = new Ray( pos - backOffset, vector + backOffset );

			var item = new PhysicsRayTestItem( ray, 1, -1, PhysicsRayTestItem.ModeEnum.OneClosestForEach );
			scene.PhysicsRayTest( item );

			var hitFound = false;

			foreach( var result in item.Result )
			{
				var rigidBody = result.Body as RigidBody;
				if( rigidBody != null )
				{
					//skip
					//which bodies to skip?

					//found hit
					hitFound = true;
					Transform = new Transform( result.Position, rot, scl );
					PerformHit( result.Body, result.Position, result.Normal, null );//PerformHit( result, null );
					break;
				}
			}

			if( !hitFound )
			{
				Transform = new Transform( ray.GetEndPoint(), rot, scl );

				//destroy when outside scene bounds
				if( IsOutsideSceneBounds( scene ) )
					RemoveFromParent( true );
			}
		}

		protected virtual void SimulationStepPhysicalBodyMode( float delta )
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;
			var body = GetRigidBody();
			if( body == null )
				return;

			var hitFound = false;

			var contactsData = body.ContactsData;
			if( contactsData != null && contactsData.Count != 0 )
			{
				for( int nContact = 0; nContact < contactsData.Count; nContact++ )
				{
					ref var item = ref contactsData.Data[ nContact ];

					//!!!!what to skip

					//found hit
					hitFound = true;

					Vector3 normal;
					var diff = body.TransformV.Position - item.PositionWorldOnB;
					if( diff != Vector3.Zero )
						normal = diff.GetNormalize();
					else
						normal = Vector3.ZAxis;

					PerformHit( item.BodyB, item.PositionWorldOnB, normal, null );

					//!!!!
					//break;
				}
			}

			if( !hitFound )
			{
				//!!!!check less often

				//destroy when outside scene bounds
				if( IsOutsideSceneBounds( scene ) )
					RemoveFromParent( true );
			}
		}

		//public Vector3 GetLinearVelocity()
		//{
		//	if( PhysicalBodyMode )
		//	{
		//		z но тут уже было столновение, скорость уже другая. получается нужно предыдущее юзать
		//		return Vector3.Zero;
		//	}
		//	else
		//		return LinearVelocityNoPhysicalBodyMode;
		//}

		public void SetLinearVelocity( Vector3 value )
		{
			if( PhysicalBodyMode )
			{
				var body = GetRigidBody();
				if( body != null )
					body.LinearVelocity = value;
			}
			else
				LinearVelocityNoPhysicalBodyMode = value;
		}

		//protected override void OnLifetimeEnd( ref bool allowDestroy )
		//{
		//	base.OnLifetimeEnd( ref allowDestroy );
		//}
	}
}
