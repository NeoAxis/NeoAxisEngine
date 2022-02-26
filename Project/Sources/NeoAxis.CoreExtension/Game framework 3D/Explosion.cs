// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A basic class for making explosions.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Explosion", -4000 )]
	[NewObjectDefaultName( "Explosion" )]
	public class Explosion : ObjectInSpace
	{
		/// <summary>
		/// The radius of an explosion effect.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set { if( _radius.BeginSet( ref value ) ) { try { RadiusChanged?.Invoke( this ); } finally { _radius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<Explosion> RadiusChanged;
		ReferenceField<double> _radius = 1.0;

		/// <summary>
		/// The strength of a physical effect.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> Impulse
		{
			get { if( _impulse.BeginGet() ) Impulse = _impulse.Get( this ); return _impulse.value; }
			set { if( _impulse.BeginSet( ref value ) ) { try { ImpulseChanged?.Invoke( this ); } finally { _impulse.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Impulse"/> property value changes.</summary>
		public event Action<Explosion> ImpulseChanged;
		ReferenceField<double> _impulse = 0.0;

		/// <summary>
		/// The strength of the damage. Affects to Health parameter.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> Damage
		{
			get { if( _damage.BeginGet() ) Damage = _damage.Get( this ); return _damage.value; }
			set { if( _damage.BeginSet( ref value ) ) { try { DamageChanged?.Invoke( this ); } finally { _damage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Damage"/> property value changes.</summary>
		public event Action<Explosion> DamageChanged;
		ReferenceField<double> _damage = 1.0;

		/// <summary>
		/// The list of objects to create when the explosion created in a simulation.
		/// </summary>
		[Serialize]
		[Cloneable]
		public ReferenceList<Metadata.TypeInfo> Objects
		{
			get { return _objects; }
		}
		public delegate void ObjectsChangedDelegate( Explosion sender );
		public event ObjectsChangedDelegate ObjectsChanged;
		ReferenceList<Metadata.TypeInfo> _objects;

		/// <summary>
		/// The creator of the explosion or the creator of a original creator.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( null )]
		public Reference<Component> OriginalCreator
		{
			get { if( _originalCreator.BeginGet() ) OriginalCreator = _originalCreator.Get( this ); return _originalCreator.value; }
			set { if( _originalCreator.BeginSet( ref value ) ) { try { OriginalCreatorChanged?.Invoke( this ); } finally { _originalCreator.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OriginalCreator"/> property value changes.</summary>
		public event Action<Explosion> OriginalCreatorChanged;
		ReferenceField<Component> _originalCreator = null;

		///////////////////////////////////////////////

		public Explosion()
		{
			_objects = new ReferenceList<Metadata.TypeInfo>( this, () => ObjectsChanged?.Invoke( this ) );
		}

		////////////////

		protected void OnObjectsBeforeCreate( bool[] allowCreateObjects ) { }
		public delegate void ObjectsBeforeCreateDelegate( Explosion sender, bool[] allowCreateObjects );
		public event ObjectsBeforeCreateDelegate ObjectsBeforeCreate;

		////////////////

		protected virtual void OnObjectCreate( Component obj ) { }
		public delegate void ObjectCreateDelegate( Explosion sender, Component obj );
		public event ObjectCreateDelegate ObjectCreate;

		////////////////

		protected virtual void OnExplode() { }
		public delegate void ExplodeDelegate( Explosion sender );
		public event ExplodeDelegate Explode;

		////////////////

		protected override void OnEnabledInSimulation()
		{
			base.OnEnabledInSimulation();

			PerformExplode();
		}

		void PerformExplode()
		{
			var scene = FindParent<Scene>();
			if( scene == null )
				return;

			var transform = TransformV;
			var radius = Radius.Value;
			var impulse = Impulse.Value;
			var damage = Damage.Value;

			if( radius > 0 )
			{
				var sphere = new Sphere( transform.Position, radius );
				var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );
				scene.GetObjectsInSpace( getObjectsItem );

				foreach( var item in getObjectsItem.Result )
				{
					if( impulse > 0 )
					{
						var body = item.Object as RigidBody;
						if( body != null && body.MotionType.Value == RigidBody.MotionTypeEnum.Dynamic )
						{
							var objectTransform = body.TransformV;

							//!!!!if object is big
							var diff = objectTransform.Position - transform.Position;
							if( diff == Vector3.Zero )
								diff = new Vector3( 0, 0, 0.001 );
							var direction = diff;
							var distance = direction.Normalize();

							if( distance < radius )
							{
								var factor = Math.Cos( distance / radius );
								body.ApplyForce( direction * impulse * factor, Vector3.Zero );
							}
						}
					}

					if( damage > 0 && item.Object != null )
					{
						var health = item.Object.PropertyGet<double>( "Health" );
						if( health > 0 )
						{
							var objectTransform = item.Object.TransformV;

							//!!!!if object is big
							var distance = ( objectTransform.Position - transform.Position ).Length();
							if( distance < radius )
							{
								var factor = Math.Cos( distance / radius );

								health -= damage * factor;
								item.Object.PropertySet( "Health", health );

								//get a new value
								var newHealth = item.Object.PropertyGet<double>( "Health" );
								if( newHealth <= 0 )
								{
									//!!!!for character switch to ragdoll

									item.Object.RemoveFromParent( true );
								}
							}
						}
					}
				}
			}

			//HitObjectsBeforeCreate
			var allowCreateObjects = new bool[ Objects.Count ];
			for( int n = 0; n < allowCreateObjects.Length; n++ )
				allowCreateObjects[ n ] = true;
			OnObjectsBeforeCreate( allowCreateObjects );
			ObjectsBeforeCreate?.Invoke( this, allowCreateObjects );

			for( int nObject = 0; nObject < Objects.Count; nObject++ )
			{
				if( !allowCreateObjects[ nObject ] )
					continue;
				var type = Objects[ nObject ].Value;
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
						objectInSpace.Transform = new Transform( transform.Position, transform.Rotation, objectInSpace.TransformV.Scale );

						////decal
						//var decal = objectInSpace as Decal;
						//if( decal != null )
						//{
						//}
					}

					OnObjectCreate( obj );
					if( !obj.RemoveFromParentQueued && !obj.Disposed )
						ObjectCreate?.Invoke( this, obj );

					if( !obj.RemoveFromParentQueued && !obj.Disposed )
						obj.Enabled = true;
				}
			}

			OnExplode();
			Explode?.Invoke( this );

			//destroy the explosion
			RemoveFromParent( true );
		}
	}
}
