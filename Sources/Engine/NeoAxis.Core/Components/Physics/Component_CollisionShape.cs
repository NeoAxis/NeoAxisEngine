// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using BulletSharp;

namespace NeoAxis
{
	/// <summary>
	/// Base class of all collision shapes.
	/// </summary>
	public abstract class Component_CollisionShape : Component
	{
		double cachedVolumeNotScaledByParent = -1;

		//!!!!все параметры добавить

		//!!!!name
		/// <summary>
		/// The position, rotation and scale of the object relative to parent.
		/// </summary>
		[Serialize]
		[DefaultValue( Transform.IdentityAsString )]
		public Reference<Transform> TransformRelativeToParent
		{
			get { if( _transformRelativeToParent.BeginGet() ) TransformRelativeToParent = _transformRelativeToParent.Get( this ); return _transformRelativeToParent.value; }
			set
			{
				if( _transformRelativeToParent.BeginSet( ref value ) )
				{
					try
					{
						TransformRelativeToParentChanged?.Invoke( this );
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _transformRelativeToParent.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="TransformRelativeToParent"/> property value changes.</summary>
		public event Action<Component_CollisionShape> TransformRelativeToParentChanged;
		ReferenceField<Transform> _transformRelativeToParent = Transform.Identity;


		protected internal abstract CollisionShape CreateShape();
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

		//!!!!так?
		static Component_RigidBody GetRigidBody( Component start )
		{
			//!!!!slowly?

			var p = start;
			while( p != null )
			{
				var body = p as Component_RigidBody;
				if( body != null )
					return body;
				p = p.Parent;
			}
			return null;
		}

		//!!!!так?
		[Browsable( false )]
		public Component_RigidBody ParentRigidBody
		{
			get { return GetRigidBody( Parent ); }
		}

		public void RecreateBody( Component oldParent = null )
		{
			Component_RigidBody body = GetRigidBody( oldParent != null ? oldParent : Parent );
			if( body != null )
				body.RecreateBody();
		}

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

		internal virtual Vector3 GetCenterOfMassPositionNotScaledByParent()
		{
			return Vector3.Zero;
		}
	}
}
