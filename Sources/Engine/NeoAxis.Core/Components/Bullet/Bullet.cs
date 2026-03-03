// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// A basic class for simulating bullets.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Bullet\Bullet", 410 )]
	public class Bullet : MeshInSpace
	{
		static FastRandom staticRandom = new FastRandom( 0 );

		BulletType typeCached = new BulletType();
		bool firstSimulationStep = true;

		///////////////////////////////////////////////

		const string bulletTypeDefault = @"Content\Weapons\Default\Bullet 1\Default Bullet.bullettype";
		//const string bulletTypeDefault = @"Content\Bullets\Default\Default.bullettype";

		[DefaultValueReference( bulletTypeDefault )]
		public Reference<BulletType> BulletType
		{
			get { if( _bulletType.BeginGet() ) BulletType = _bulletType.Get( this ); return _bulletType.value; }
			set
			{
				if( _bulletType.BeginSet( this, ref value ) )
				{
					try
					{
						BulletTypeChanged?.Invoke( this );

						//update cached type
						typeCached = _bulletType.value;
						if( typeCached == null )
							typeCached = new BulletType();

						//update mesh
						if( EnabledInHierarchyAndIsInstance )
							UpdateMesh();
					}
					finally { _bulletType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BulletType"/> property value changes.</summary>
		public event Action<Bullet> BulletTypeChanged;
		ReferenceField<BulletType> _bulletType = new Reference<BulletType>( null, bulletTypeDefault );

		/// <summary>
		/// The linear velocity when the bullet has no physical body.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( "0 0 0" )]
		public Reference<Vector3> LinearVelocityNoPhysicalBodyMode
		{
			get { if( _linearVelocityNoPhysicalBodyMode.BeginGet() ) LinearVelocityNoPhysicalBodyMode = _linearVelocityNoPhysicalBodyMode.Get( this ); return _linearVelocityNoPhysicalBodyMode.value; }
			set { if( _linearVelocityNoPhysicalBodyMode.BeginSet( this, ref value ) ) { try { LinearVelocityNoPhysicalBodyModeChanged?.Invoke( this ); } finally { _linearVelocityNoPhysicalBodyMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LinearVelocityNoPhysicalBodyMode"/> property value changes.</summary>
		public event Action<Bullet> LinearVelocityNoPhysicalBodyModeChanged;
		ReferenceField<Vector3> _linearVelocityNoPhysicalBodyMode = Vector3.Zero;

		/// <summary>
		/// User ID who started firing.
		/// </summary>
		[DefaultValue( 0 )]
		public Reference<long> WhoFired
		{
			get { if( _whoFired.BeginGet() ) WhoFired = _whoFired.Get( this ); return _whoFired.value; }
			set { if( _whoFired.BeginSet( this, ref value ) ) { try { WhoFiredChanged?.Invoke( this ); } finally { _whoFired.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WhoFired"/> property value changes.</summary>
		public event Action<Bullet> WhoFiredChanged;
		ReferenceField<long> _whoFired = 0;

		///////////////////////////////////////////////

		public class HitData
		{
			public ObjectInSpace SceneObject;
			public Scene.PhysicsWorldClass.Body PhysicalBody;
			public Vector3 Position;
			public Vector3 Normal;
			public object AnyData;

			public bool Handled;
		}

		///////////////////////////////////////////////

		//protected virtual void OnHitBefore( HitData hit ) { }
		public delegate void HitBeforeDelegate( Bullet sender, HitData hit );
		public event HitBeforeDelegate HitBefore;

		///////////////////////////////////////////////

		//old
		//protected void OnHitObjectsBeforeCreate( bool[] allowCreateObjects ) { }
		//public delegate void HitObjectsBeforeCreateDelegate( Bullet sender, bool[] allowCreateObjects );
		//public event HitObjectsBeforeCreateDelegate HitObjectsBeforeCreate;

		////////////////

		//old
		//protected virtual void OnHitObjectCreate( Component obj ) { }
		//public delegate void HitObjectCreateDelegate( Bullet sender, Component obj );
		//public event HitObjectCreateDelegate HitObjectCreate;

		////////////////

		//protected virtual void OnHit( HitData hit ) { }
		public delegate void HitDelegate( Bullet sender, HitData hit );
		public event HitDelegate Hit;

		////////////////

		[Browsable( false )]
		public BulletType TypeCached
		{
			get { return typeCached; }
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void PerformHit( HitData hit )
		{
			//HitBefore
			TypeCached.PerformHitBefore( this, hit );
			if( hit.Handled )
				return;
			//OnHitBefore( hit );
			//if( hit.Handled )
			//	return;
			if( HitBefore != null )
			{
				HitBefore.Invoke( this, hit );
				if( hit.Handled )
					return;
			}

			//HitObjectsBeforeCreate
			////GC
			//var allowCreateObjects = new bool[ HitObjects.Count ];
			//for( int n = 0; n < allowCreateObjects.Length; n++ )
			//	allowCreateObjects[ n ] = true;
			//OnHitObjectsBeforeCreate( allowCreateObjects );
			//HitObjectsBeforeCreate?.Invoke( this, allowCreateObjects );

			var hitSceneObject = hit.SceneObject;

			var fixedHitPosition = hit.Position;

			//use mesh geometry instead of collision body for greater accuracy
			{
				MeshInSpace meshInSpace = null;
				if( hitSceneObject != null )
				{
					if( hitSceneObject is RigidBody && hitSceneObject.Name == "Collision Body" )
						meshInSpace = hitSceneObject.Parent as MeshInSpace;
					else if( hitSceneObject is MeshInSpace )
						meshInSpace = hitSceneObject as MeshInSpace;
				}
				else
				{
					//impl?
				}

				if( meshInSpace != null )
				{
					var mesh = meshInSpace.Mesh.Value;
					if( mesh != null && mesh.Result != null )
					{
						var distanceOffset = meshInSpace.SpaceBounds.BoundingSphere.Radius / 2;
						var ray = new Ray( hit.Position + hit.Normal * distanceOffset, -hit.Normal * distanceOffset * 2 );

						meshInSpace.TransformV.ToMatrix4().GetInverse( out var transInv );
						var localRay = transInv * ray;

						var rayCastResult = mesh.Result.RayCast( localRay, NeoAxis.Mesh.CompiledData.RayCastModes.Auto, true );
						if( rayCastResult != null )
						{
							fixedHitPosition = ray.GetPointOnRay( rayCastResult.Scale );
							//use normal
						}

						//if( mesh.Result.RayCast( localRay, NeoAxis.Mesh.CompiledData.RayCastMode.Auto, true, out var scale, out var normal, out _ ) )
						//{
						//	fixedHitPosition = ray.GetPointOnRay( scale );
						//	//use normal
						//}
					}
				}
			}

			//add an impulse to the hit body
			if( PhysicalBody == null && hit.PhysicalBody != null && hit.PhysicalBody.MotionType == PhysicsMotionType.Dynamic )
			{
				var mass = TypeCached.MassNoPhysicalBodyMode.Value;
				if( mass > 0 )
				{
					//!!!!slowly?
					var tr = new Matrix4( hit.PhysicalBody.Rotation.ToMatrix3(), hit.PhysicalBody.Position );
					tr.GetInverse( out var invertTr );
					var localPosition = invertTr * TransformV.Position;
					//!!!!need apply scale?
					hit.PhysicalBody.ApplyForce( ( LinearVelocityNoPhysicalBodyMode.Value * mass ).ToVector3F(), localPosition.ToVector3F() );
				}
			}

			//damage
			if( hit.SceneObject != null && TypeCached.HitDamage != 0 )
			{
				var processDamage = hit.SceneObject as IProcessDamage;
				if( processDamage == null )
					processDamage = hit.SceneObject.Parent as IProcessDamage;

				if( processDamage != null )
				{
					//!!!!maybe add 'hit' to the method. maybe to anyData

					processDamage.ProcessDamage( WhoFired, (float)TypeCached.HitDamage.Value, null );
					//processDamage.ProcessDamage( OriginalCreator.Value ?? this, HitDamage.Value, null );
				}

				////!!!!maybe use interface IHealth

				//var health = parent.PropertyGet<double>( "Health" );
				//if( health > 0 )
				//{
				//	health -= HitDamage.Value;
				//	parent.PropertySet( "Health", health );

				//	//get a new value
				//	var newHealth = parent.PropertyGet<double>( "Health" );
				//	if( newHealth <= 0 )
				//	{
				//		//!!!!for character switch to ragdoll

				//		parent.RemoveFromParent( true );
				//	}
				//}
			}

			//volume hit
			if( TypeCached.HitVolumeRadius > 0 )
			{
				//get affected bodies
				var sphere = new Sphere( fixedHitPosition, TypeCached.HitVolumeRadius );
				var volumeTestItem = new PhysicsVolumeTestItem( sphere, Vector3.Zero, PhysicsVolumeTestItem.ModeEnum.OneClosestForEach );
				ParentScene.PhysicsVolumeTest( volumeTestItem );

				//process affected bodies
				for( int n = 0; n < volumeTestItem.Result.Length; n++ )
				{
					ref var item = ref volumeTestItem.Result[ n ];

					item.Body.GetBounds( out var bodyBounds );

					var distance = bodyBounds.GetPointDistance( fixedHitPosition );
					if( distance < TypeCached.HitVolumeRadius )
					{
						var distanceFactor = Math.Cos( distance / TypeCached.HitVolumeRadius );
						//var distanceFactor = MathEx.Saturate( 1.0 - distance / TypeCached.HitVolumeRadius );

						if( distanceFactor > 0 )
						{
							//add damage
							if( TypeCached.HitVolumeDamage != 0 )
							{
								var obj = item.Body.Owner as ObjectInSpace;
								if( obj != null )
								{
									var damage = TypeCached.HitVolumeDamage * distanceFactor;

									var processDamage = obj as IProcessDamage;
									if( processDamage == null )
										processDamage = obj.Parent as IProcessDamage;

									if( processDamage != null )
										processDamage.ProcessDamage( WhoFired, (float)damage, null );
								}
							}

							//add impulse
							if( TypeCached.HitVolumeImpulse != 0 )
							{
								if( item.Body != null && item.Body.MotionType == PhysicsMotionType.Dynamic )
								{
									var center = bodyBounds.GetCenter();
									var direction = center - fixedHitPosition;
									if( direction == Vector3.Zero )
										direction.X = 0.0001;
									direction.Normalize();

									var force = direction * TypeCached.HitVolumeImpulse * distanceFactor;
									var localPosition = Vector3F.Zero;

									item.Body.ApplyForce( force.ToVector3F(), localPosition );


									////!!!!slowly?
									//var tr = new Matrix4( hit.PhysicalBody.Rotation.ToMatrix3(), hit.PhysicalBody.Position );
									//tr.GetInverse( out var invertTr );
									//var localPosition = invertTr * TransformV.Position;
									////!!!!need apply scale?
									//hit.PhysicalBody.ApplyForce( ( LinearVelocityNoPhysicalBodyMode.Value * mass ).ToVector3F(), localPosition.ToVector3F() );
								}
							}
						}
					}
				}
			}

			if( NetworkIsSingle )
				HitSoundParticleDecal( fixedHitPosition, hit.Normal, hit.PhysicalBody != null, hitSceneObject );

			////HitSound
			//if( NetworkIsSingle )
			//{
			//	ParentScene.SoundPlay( TypeCached.HitSound, TransformV.Position );

			//	//!!!!not good. don't transfer sound name it is already know
			//	//!!!!impl

			//	//var soundName = TypeCached.HitSound.GetByReference;
			//	//if( NetworkIsServer && !string.IsNullOrEmpty( soundName ) )
			//	//{
			//	//	var writer = BeginNetworkMessageToEveryone( "SoundHit" );

			//	//	//!!!!maybe send only index of HitObjects

			//	//	writer.Write( soundName );
			//	//	writer.Write( TransformV.Position );
			//	//	EndNetworkMessage();
			//	//}
			//}

			////HitParticle
			//if( NetworkIsSingle )
			//{
			//	var particleSystem = TypeCached.HitParticle.Value;
			//	if( particleSystem != null )
			//	{
			//		var obj = Parent.CreateComponent<ParticleSystemInSpace>( enabled: false, networkMode: NetworkModeEnum.False );
			//		//!!!!can be faster without reference, but then can't serialize in simulation
			//		obj.ParticleSystem = particleSystem;//p.ParticleSystem = TypeCached.HitParticle;
			//		obj.RemainingLifetime = TypeCached.HitParticleLifetime;

			//		if( staticRandom == null )
			//			staticRandom = new FastRandom();

			//		Quaternion rot;
			//		if( TypeCached.HitParticleApplyHitNormal )
			//			rot = Quaternion.FromDirectionZAxisUp( hit.Normal ) * Quaternion.FromRotateByX( staticRandom.Next( 0, MathEx.PI * 2 ) );
			//		else
			//			rot = Quaternion.Identity;
			//		obj.Transform = new Transform( hit.Position, rot, obj.TransformV.Scale );

			//		//var bulletTransform = TransformV;
			//		//var rot = TypeCached.HitParticleApplyBulletRotation ? bulletTransform.Rotation : Quaternion.Identity;
			//		//obj.Transform = new Transform( fixedHitPosition/*bulletTransform.Position*/, rot, obj.TransformV.Scale );

			//		obj.Enabled = true;
			//	}
			//}

			////HitDecal
			//if( NetworkIsSingle && TypeCached.HitDecal )
			//{
			//	//create only for rigid bodies, don't create for characters
			//	if( hit.PhysicalBody != null /*hitSceneObject != null */ && hitSceneObject as Character == null && hitSceneObject?.Parent as Character == null )
			//	{
			//		var obj = Parent.CreateComponent<Decal>( enabled: false, networkMode: NetworkModeEnum.False );

			//		//var bulletTransform = TransformV;
			//		obj.RemainingLifetime = TypeCached.HitDecalLifetime;
			//		obj.VisibilityDistanceFactor = TypeCached.HitDecalVisibilityDistanceFactor;
			//		obj.Material = TypeCached.HitDecalMaterial;

			//		if( staticRandom == null )
			//			staticRandom = new FastRandom();

			//		var rot = Quaternion.FromDirectionZAxisUp( -hit.Normal );
			//		rot *= Quaternion.FromRotateByX( staticRandom.Next( 0, MathEx.PI * 2 ) );

			//		obj.Transform = new Transform( fixedHitPosition, rot, TypeCached.HitDecalScale/*obj.TransformV.Scale*/ );

			//		//attach to the body when it is dynamic
			//		if( hitSceneObject != null )
			//		{
			//			//RigidBody
			//			var hitRigidBody = hitSceneObject as RigidBody;
			//			if( hitRigidBody != null && hitRigidBody.MotionType.Value != PhysicsMotionType.Static )
			//			{
			//				//change parent
			//				obj.RemoveFromParent( false );
			//				hitRigidBody.AddComponent( obj );

			//				ObjectInSpaceUtility.Attach( hitRigidBody, obj, TransformOffset.ModeEnum.Matrix );
			//			}

			//			//MeshInSpace
			//			var hitMeshInSpaceBody = hitSceneObject as MeshInSpace;
			//			if( hitMeshInSpaceBody?.PhysicalBody != null && hitMeshInSpaceBody.PhysicalBody.MotionType != PhysicsMotionType.Static )
			//			{
			//				//change parent
			//				obj.RemoveFromParent( false );
			//				hitMeshInSpaceBody.AddComponent( obj );

			//				ObjectInSpaceUtility.Attach( hitMeshInSpaceBody, obj, TransformOffset.ModeEnum.Matrix );
			//			}
			//		}

			//		obj.Enabled = true;
			//	}
			//}

			//send Hit to clients
			if( NetworkIsServer )
			{
				var m = BeginNetworkMessageToEveryone( "Hit" );
				if( m != null )
				{
					var writer = m.Writer;
					writer.Write( fixedHitPosition );
					writer.Write( hit.Normal.ToVector3F() );
					writer.Write( hit.PhysicalBody != null );
					writer.WriteVariableUInt64( (ulong)( hitSceneObject != null ? hitSceneObject.NetworkID : 0 ) );
					m.End();
				}
			}

			//OnHit( hit );
			//if( hit.Handled )
			//	return;
			if( Hit != null )
			{
				Hit.Invoke( this, hit );
				if( hit.Handled )
					return;
			}

			//destroy the bullet
			if( TypeCached.DestroyOnHit )
				RemoveFromParent( true );


			//old
			////HitObjects
			//for( int nHitObject = 0; nHitObject < TypeCached.HitObjects.Count; nHitObject++ )
			//{
			//	//if( !allowCreateObjects[ nHitObject ] )
			//	//	continue;
			//	var type = TypeCached.HitObjects[ nHitObject ].Value;
			//	if( type == null )
			//		continue;

			//	if( MetadataManager.GetTypeOfNetType( typeof( Sound ) ).IsAssignableFrom( type ) )
			//	{
			//		//sound
			//		ParentScene.SoundPlay( type.Name, TransformV.Position );
			//		if( NetworkIsServer && !string.IsNullOrEmpty( type.Name ) )
			//		{
			//			var writer = BeginNetworkMessageToEveryone( "SoundHit" );

			//			//!!!!maybe send only index of HitObjects

			//			writer.Write( type.Name );
			//			writer.Write( TransformV.Position );
			//			EndNetworkMessage();
			//		}
			//	}
			//	else
			//	{
			//		//create component

			//		Component obj = null;

			//		//create ParticleSystemInSpace
			//		if( MetadataManager.GetTypeOfNetType( typeof( ParticleSystem ) ).IsAssignableFrom( type ) )
			//		{
			//			var p = Parent.CreateComponent<ParticleSystemInSpace>( enabled: false );
			//			//!!!!faster without reference, but can't serialize
			//			p.ParticleSystem = ReferenceUtility.MakeReference( type.Name );
			//			obj = p;
			//		}

			//		//default creation
			//		if( obj == null )
			//		{
			//			//!!!!slowly

			//			obj = Parent.CreateComponent( type, enabled: false );
			//		}


			//		//configure

			//		var objectInSpace = obj as ObjectInSpace;
			//		if( objectInSpace != null )
			//		{
			//			var bulletTransform = TransformV;
			//			objectInSpace.Transform = new Transform( bulletTransform.Position, bulletTransform.Rotation, objectInSpace.TransformV.Scale );

			//			//decal
			//			var decal = objectInSpace as Decal;
			//			if( decal != null )
			//			{
			//				//create only for rigid bodies, don't create for characters
			//				if( hit.PhysicalBody != null /*hitSceneObject != null */ && hitSceneObject as Character == null && hitSceneObject?.Parent as Character == null )
			//				{
			//					//var fixedHitPosition = hit.Position;

			//					////use mesh geometry instead of collision body for greater accuracy
			//					//{
			//					//	MeshInSpace meshInSpace = null;
			//					//	if( hitSceneObject != null )
			//					//	{
			//					//		if( hitSceneObject is RigidBody && hitSceneObject.Name == "Collision Body" )
			//					//			meshInSpace = hitSceneObject.Parent as MeshInSpace;
			//					//		else if( hitSceneObject is MeshInSpace )
			//					//			meshInSpace = hitSceneObject as MeshInSpace;
			//					//	}
			//					//	else
			//					//	{
			//					//		//!!!!impl
			//					//	}

			//					//	if( meshInSpace != null )
			//					//	{
			//					//		var mesh = meshInSpace.Mesh.Value;
			//					//		if( mesh != null && mesh.Result != null )
			//					//		{
			//					//			var distanceOffset = meshInSpace.SpaceBounds.BoundingSphere.Radius / 2;
			//					//			var ray = new Ray( hit.Position + hit.Normal * distanceOffset, -hit.Normal * distanceOffset * 2 );

			//					//			meshInSpace.TransformV.ToMatrix4().GetInverse( out var transInv );
			//					//			var localRay = transInv * ray;

			//					//			var rayCastResult = mesh.Result.RayCast( localRay, NeoAxis.Mesh.CompiledData.RayCastModes.Auto, true );
			//					//			if( rayCastResult != null )
			//					//			{
			//					//				fixedHitPosition = ray.GetPointOnRay( rayCastResult.Scale );
			//					//				//use normal
			//					//			}

			//					//			//if( mesh.Result.RayCast( localRay, NeoAxis.Mesh.CompiledData.RayCastMode.Auto, true, out var scale, out var normal, out _ ) )
			//					//			//{
			//					//			//	fixedHitPosition = ray.GetPointOnRay( scale );
			//					//			//	//use normal
			//					//			//}
			//					//		}
			//					//	}
			//					//}

			//					if( staticRandom == null )
			//						staticRandom = new FastRandom();

			//					var rot = Quaternion.FromDirectionZAxisUp( -hit.Normal );
			//					rot *= Quaternion.FromRotateByX( staticRandom.Next( 0, MathEx.PI * 2 ) );

			//					objectInSpace.Transform = new Transform( fixedHitPosition, rot, objectInSpace.TransformV.Scale );

			//					//attach to the body when it is dynamic
			//					if( hitSceneObject != null )
			//					{
			//						//RigidBody
			//						var hitRigidBody = hitSceneObject as RigidBody;
			//						if( hitRigidBody != null && hitRigidBody.MotionType.Value != PhysicsMotionType.Static )
			//						{
			//							//change parent
			//							obj.RemoveFromParent( false );
			//							hitRigidBody.AddComponent( obj );

			//							ObjectInSpaceUtility.Attach( hitRigidBody, objectInSpace, TransformOffset.ModeEnum.Matrix );
			//						}

			//						//MeshInSpace
			//						var hitMeshInSpaceBody = hitSceneObject as MeshInSpace;
			//						if( hitMeshInSpaceBody?.PhysicalBody != null && hitMeshInSpaceBody.PhysicalBody.MotionType != PhysicsMotionType.Static )
			//						{
			//							//change parent
			//							obj.RemoveFromParent( false );
			//							hitMeshInSpaceBody.AddComponent( obj );

			//							ObjectInSpaceUtility.Attach( hitMeshInSpaceBody, objectInSpace, TransformOffset.ModeEnum.Matrix );
			//						}
			//					}
			//				}
			//				else
			//				{
			//					//!!!!slowly
			//					objectInSpace.Dispose();
			//				}
			//			}
			//			else if( objectInSpace is ParticleSystemInSpace )//particleInSpace )
			//			{
			//				//particle system
			//				//var particleInSpace = objectInSpace as ParticleSystemInSpace;
			//				//if( particleInSpace != null )
			//				//{

			//				if( staticRandom == null )
			//					staticRandom = new FastRandom();

			//				var rot = Quaternion.FromDirectionZAxisUp( hit.Normal );
			//				rot *= Quaternion.FromRotateByX( staticRandom.Next( 0, MathEx.PI * 2 ) );

			//				objectInSpace.Transform = new Transform( hit.Position, rot, objectInSpace.TransformV.Scale );

			//				//}
			//			}

			//			////explosion
			//			//var explosion = objectInSpace as Explosion;
			//			//if( explosion != null )
			//			//	explosion.WhoFired = WhoFired;//explosion.OriginalCreator = OriginalCreator;
			//		}

			//		//OnHitObjectCreate( obj );
			//		//if( !obj.RemoveFromParentQueued && !obj.Disposed )
			//		//	HitObjectCreate?.Invoke( this, obj );

			//		if( !obj.RemoveFromParentQueued && !obj.Disposed )
			//			obj.Enabled = true;
			//	}
			//}

		}

		void HitSoundParticleDecal( Vector3 fixedHitPosition, Vector3 hitNormal, bool hitHasPhysicalBody, ObjectInSpace hitSceneObject )
		{
			//HitSound
			ParentScene.SoundPlay( TypeCached.HitSound, fixedHitPosition );

			//HitParticle
			{
				//!!!!can be faster without reference, but then can't serialize in simulation
				//var particleSystemReference = TypeCached.HitParticle;//var particleSystem = TypeCached.HitParticle.Value;
				//if( particleSystemReference.Value != null )
				//obj.ParticleSystem = particleSystemReference;

				var particleSystem = TypeCached.HitParticle.Value;
				if( particleSystem != null )
				{
					var obj = Parent.CreateComponent<ParticleSystemInSpace>( enabled: false );
					obj.ParticleSystem = particleSystem;
					obj.RemainingLifetime = TypeCached.HitParticleLifetime;

					Quaternion rot;
					if( TypeCached.HitParticleApplyHitNormal )
						rot = Quaternion.FromDirectionZAxisUp( hitNormal ) * Quaternion.FromRotateByX( staticRandom.Next( MathEx.PI * 2 ) );
					else
						rot = Quaternion.Identity;
					obj.Transform = new Transform( fixedHitPosition, rot, obj.TransformV.Scale );

					//var bulletTransform = TransformV;
					//var rot = TypeCached.HitParticleApplyBulletRotation ? bulletTransform.Rotation : Quaternion.Identity;
					//obj.Transform = new Transform( fixedHitPosition/*bulletTransform.Position*/, rot, obj.TransformV.Scale );

					obj.Enabled = true;
				}
			}

			//HitDecal
			if( TypeCached.HitDecal )
			{
				//create only for rigid bodies, don't create for characters
				if( hitHasPhysicalBody && hitSceneObject as Character == null && hitSceneObject?.Parent as Character == null )
				{
					var obj = Parent.CreateComponent<Decal>( enabled: false );

					//var bulletTransform = TransformV;
					obj.RemainingLifetime = TypeCached.HitDecalLifetime;
					obj.VisibilityDistanceFactor = TypeCached.HitDecalVisibilityDistanceFactor;
					obj.Material = TypeCached.HitDecalMaterial;

					var rot = Quaternion.FromDirectionZAxisUp( -hitNormal );
					rot *= Quaternion.FromRotateByX( staticRandom.Next( MathEx.PI * 2 ) );

					obj.Transform = new Transform( fixedHitPosition, rot, TypeCached.HitDecalScale/*obj.TransformV.Scale*/ );

					//attach to the body when it is dynamic
					if( hitSceneObject != null )
					{
						//RigidBody
						var hitRigidBody = hitSceneObject as RigidBody;
						if( hitRigidBody != null && hitRigidBody.MotionType.Value != PhysicsMotionType.Static )
						{
							//change parent
							obj.RemoveFromParent( false );
							hitRigidBody.AddComponent( obj );

							ObjectInSpaceUtility.Attach( hitRigidBody, obj, TransformOffset.ModeEnum.Matrix );
						}

						//MeshInSpace
						var hitMeshInSpaceBody = hitSceneObject as MeshInSpace;
						if( hitMeshInSpaceBody?.PhysicalBody != null && hitMeshInSpaceBody.PhysicalBody.MotionType != PhysicsMotionType.Static )
						{
							//change parent
							obj.RemoveFromParent( false );
							hitMeshInSpaceBody.AddComponent( obj );

							ObjectInSpaceUtility.Attach( hitMeshInSpaceBody, obj, TransformOffset.ModeEnum.Matrix );
						}
					}

					obj.Enabled = true;
				}
			}
		}

		////////////////

		void UpdateMesh()
		{
			Mesh = TypeCached.Mesh;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			BulletType.Touch();

			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
				UpdateMesh();
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( PhysicalBody != null )
				SimulationStepPhysicalBodyMode( Time.SimulationDelta );
			else
				SimulationStepNoPhysicalBodyMode( Time.SimulationDelta );

			firstSimulationStep = false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected virtual bool IsOutsideSceneBounds( Scene scene )
		{
			var position = TransformV.Position;

			var bounds = scene.BoundsWhenSimulationStarted;
			bounds.Expand( 100 );

			if( bounds.Contains( ref position ) )
				return false;

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void SimulationStepNoPhysicalBodyMode( float delta )
		{
			var scene = ParentScene;
			if( scene == null )
				return;

			//apply gravity to velocity
			var gravityFactor = TypeCached.GravityFactorNoPhysicalBodyMode.Value;
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

			var rayTestItem = new PhysicsRayTestItem( ray, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.CalculateNormal );
			scene.PhysicsRayTest( rayTestItem );

			var hitFoundIndex = -1;

			if( rayTestItem.Result.Length != 0 )
			{
				hitFoundIndex = 0;

				//for( int n = 0; n < rayTestItem.Result.Length; /*n++*/ )//foreach( var resultItem in item.Result )
				//{
				//	ref var resultItem = ref rayTestItem.Result[ n ];

				//	//skip
				//	//which bodies to skip?
				//}
			}

			if( hitFoundIndex != -1 )
			{
				//hit

				ref var resultItem = ref rayTestItem.Result[ hitFoundIndex ];

				Transform = new Transform( resultItem.Position, rot, scl );

				var hit = new HitData();
				hit.PhysicalBody = resultItem.Body;
				hit.SceneObject = hit.PhysicalBody.Owner as ObjectInSpace;
				hit.Position = resultItem.Position;
				hit.Normal = resultItem.Normal;

				//it is possibly to clarity result by means ray cast on mesh. the example in PerformHit method

				PerformHit( hit );
			}
			else
			{
				//no hit

				Transform = new Transform( ray.GetEndPoint(), rot, scl );

				//!!!!check it not each update?

				//destroy when outside scene bounds
				if( IsOutsideSceneBounds( scene ) )
					RemoveFromParent( true );
			}


			//for( int n = 0; n < item.Result.Length; /*n++*/ )//foreach( var resultItem in item.Result )
			//{
			//	ref var resultItem = ref item.Result[ n ];

			//	//skip
			//	//which bodies to skip?

			//	//found hit
			//	hitFound = true;
			//	Transform = new Transform( resultItem.Position, rot, scl );

			//	var hit = new HitItem();
			//	hit.PhysicalBody = resultItem.Body;
			//	hit.SceneObject = hit.PhysicalBody.BodyOwner as ObjectInSpace;
			//	hit.Position = resultItem.Position;
			//	hit.Normal = resultItem.Normal;

			//	//it is possibly to clarity result by means ray cast on mesh

			//	PerformHit( hit );
			//	break;

			//	//var rigidBody = result.Body as RigidBody;
			//	//if( rigidBody != null )
			//	//{
			//	//	//skip
			//	//	//which bodies to skip?

			//	//	//found hit
			//	//	hitFound = true;
			//	//	Transform = new Transform( result.Position, rot, scl );

			//	//	PerformHit( result.Body, result.Position, result.Normal, null );//PerformHit( result, null );
			//	//	break;
			//	//}
			//}

			//if( !hitFound )
			//{
			//	Transform = new Transform( ray.GetEndPoint(), rot, scl );

			//	//destroy when outside scene bounds
			//	if( IsOutsideSceneBounds( scene ) )
			//		RemoveFromParent( true );
			//}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void SimulationStepPhysicalBodyMode( float delta )
		{
			var scene = ParentScene;
			if( scene == null )
				return;

			var hitFound = false;

			unsafe
			{
				PhysicalBody.GetContacts( out var itemCount, out var itemBuffer );
				if( itemCount != 0 )
				{
					//if( contacts.Count != 0 )//for( int nContact = 0; nContact < contacts.Count; nContact++ )
					//{

					ref var item = ref itemBuffer[ 0 ];

					var hit = new HitData();
					hit.PhysicalBody = scene.PhysicsWorld.GetBodyById( item.Body2ID );
					hit.SceneObject = hit.PhysicalBody.Owner as ObjectInSpace;
					hit.Position = item.GetContactPointOn2( 0 );
					hit.Normal = -item.Normal;
					PerformHit( hit );

					hitFound = true;

					//if( !EnabledInHierarchy || RemoveFromParentQueued )
					//	break;
					//}
				}
			}

			//var contacts = PhysicalBody.GetContacts();
			//if( contacts.Count != 0 )
			//{
			//	if( contacts.Count != 0 )//for( int nContact = 0; nContact < contacts.Count; nContact++ )
			//	{
			//		var nContact = 0;
			//		ref var contact = ref contacts.Array[ contacts.Offset + nContact ];//ref var contact = ref contacts[ nContact ];

			//		//!!!!
			//		Vector3 normal;
			//		var diff = PhysicalBody.Position - contact.WorldPositionOn2;
			//		if( diff != Vector3.Zero )
			//			normal = diff.GetNormalize();
			//		else
			//			normal = Vector3.ZAxis;

			//		var hit = new HitData();
			//		hit.PhysicalBody = contact.Body2;
			//		hit.SceneObject = hit.PhysicalBody.Owner as ObjectInSpace;
			//		hit.Position = contact.WorldPositionOn2;
			//		hit.Normal = normal;
			//		PerformHit( hit );

			//		hitFound = true;

			//		//if( !EnabledInHierarchy || RemoveFromParentQueued )
			//		//	break;
			//	}
			//}

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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SetLinearVelocity( Vector3 value )
		{
			PhysicalBodyLinearVelocity = value;
			LinearVelocityNoPhysicalBodyMode = value;
		}

		//protected override void OnLifetimeEnd( ref bool allowDestroy )
		//{
		//	base.OnLifetimeEnd( ref allowDestroy );
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "Hit" )
			{
				var fixedHitPosition = reader.ReadVector3();
				var hitNormal = reader.ReadVector3F();
				var hitHasPhysicalBody = reader.ReadBoolean();
				var hitSceneObjectNetworkID = (long)reader.ReadVariableUInt64();
				if( !reader.Complete() )
					return false;
				var hitSceneObject = ParentRoot.HierarchyController.GetComponentByNetworkID( hitSceneObjectNetworkID ) as ObjectInSpace;
				HitSoundParticleDecal( fixedHitPosition, hitNormal, hitHasPhysicalBody, hitSceneObject );
				return true;
			}

			//if( message == "SoundHit" )
			//{
			//	var soundName = reader.ReadString();
			//	var position = reader.ReadVector3();
			//	if( !reader.Complete() )
			//		return false;
			//	ParentScene?.SoundPlay( soundName, position );
			//}

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			if( PhysicalBody == null )
			{
				var result = MeshOutput?.Result;
				if( result != null )
				{
					if( result.MeshData.BillboardMode != 0 )
					{
						var tr = Transform.Value;
						var meshSphere = result.MeshData.SpaceBounds.BoundingSphere;
						newBounds = new SpaceBounds( new Sphere( tr.Position, meshSphere.Radius * tr.Scale.MaxComponent() ) );
					}
					else
					{
						var meshBoundLocal = result.SpaceBounds;
						var meshBoundsTransformed = SpaceBounds.Multiply( Transform, meshBoundLocal );
						newBounds = SpaceBounds.Merge( newBounds, meshBoundsTransformed );
					}

					//bounds prediction to skip small updates in future steps
					var realBounds = newBounds.BoundingBox;

					if( !SpaceBoundsOctreeOverride.HasValue || !SpaceBoundsOctreeOverride.Value.Contains( ref realBounds ) )
					{
						//calculated extended bounds. predict for 2-3 seconds

						var trPosition = TransformV.Position;

						var bTotal = realBounds;
						var b2 = new Bounds( trPosition );
						b2.Add( trPosition + LinearVelocityNoPhysicalBodyMode.Value * ( 2.0f + staticRandom.NextFloat() ) );
						b2.Expand( newBounds.BoundingSphere.Radius * 1.1 );
						bTotal.Add( ref b2 );

						SpaceBoundsOctreeOverride = bTotal;
					}
				}
			}
			else
				base.OnSpaceBoundsUpdate( ref newBounds );
		}
	}
}
