// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Base class of all collision shapes.
	/// </summary>
	public abstract class CollisionShape : Component
	{
		double cachedVolumeNotScaledByParent = -1;

		/// <summary>
		/// The position, rotation and scale of the object relative to parent.
		/// </summary>
		[Serialize]
		[DefaultValue( Transform.IdentityAsString )]
		public Reference<Transform> LocalTransform
		{
			get { if( _localTransform.BeginGet() ) LocalTransform = _localTransform.Get( this ); return _localTransform.value; }
			set
			{
				if( _localTransform.BeginSet( this, ref value ) )
				{
					try
					{
						LocalTransformChanged?.Invoke( this );
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _localTransform.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LocalTransform"/> property value changes.</summary>
		public event Action<CollisionShape> LocalTransformChanged;
		ReferenceField<Transform> _localTransform = Transform.Identity;

		///////////////////////////////////////////////

		protected internal virtual void GetShapeKey( StringBuilder key )
		{
			key.Append( " lt " );
			var relativeTransform = LocalTransform.Value;
			if( !relativeTransform.IsIdentity )
				key.Append( relativeTransform.ToString() );
			else
				key.Append( '1' );
		}
		//protected internal abstract void GetShapeKey( StringBuilder key );

		protected internal abstract void CreateShape( Scene scene, IntPtr nativeShape, ref Vector3F position, ref QuaternionF rotation, ref Vector3F localScaling, ref Scene.PhysicsWorldClass.Shape.CollisionShapeData collisionShapeData );

		protected internal abstract void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered );

		protected override void OnAddedToParent()
		{
			base.OnAddedToParent();

			if( EnabledInHierarchy )
				RecreateBody();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			base.OnRemovedFromParent( oldParent );

			RecreateBody( oldParent );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			RecreateBody();
		}

		static RigidBody GetRigidBody( Component start )
		{
			var p = start;
			while( p != null )
			{
				var body = p as RigidBody;
				if( body != null )
					return body;
				p = p.Parent;
			}
			return null;
		}

		//!!!!так?
		[Browsable( false )]
		public RigidBody ParentRigidBody
		{
			get { return GetRigidBody( Parent ); }
		}

		public void RecreateBody( Component oldParent = null )
		{
			RigidBody body = GetRigidBody( oldParent != null ? oldParent : Parent );
			if( body != null )
				body.RecreateBody();
		}

		//!!!!
		[Browsable( false )]
		public double VolumeNotScaledByParent
		{
			get
			{
				if( cachedVolumeNotScaledByParent < 0 )
					cachedVolumeNotScaledByParent = OnCalculateVolume();
				return cachedVolumeNotScaledByParent;
			}
		}

		protected abstract double OnCalculateVolume();

		protected void NeedUpdateCachedVolume()
		{
			cachedVolumeNotScaledByParent = -1;
			//body.NeedUpdateCachedMass();
		}

		//!!!!?
		internal virtual Vector3 GetCenterOfMassPositionNotScaledByParent()
		{
			return Vector3.Zero;
		}
	}
}
