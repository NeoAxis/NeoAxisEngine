// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using tainicom.Aether.Physics2D.Dynamics;

namespace NeoAxis
{
	/// <summary>
	/// Base class of all 2D collision shapes.
	/// </summary>
	public abstract class Component_CollisionShape2D : Component
	{
		double cachedAreaNotScaledByParent = -1;

		/////////////////////////////////////////

		/// <summary>
		/// The position, rotation and scale of the object relative to parent.
		/// </summary>
		[Category( "Collision Shape 2D" )]
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
		public event Action<Component_CollisionShape2D> TransformRelativeToParentChanged;
		ReferenceField<Transform> _transformRelativeToParent = Transform.Identity;

		/// <summary>
		/// The amount of friction applied on the shape.
		/// </summary>
		[DefaultValue( 0.2 )]
		[Category( "Collision Shape 2D" )]
		[Range( 0, 1 )]
		public Reference<double> Friction
		{
			get { if( _friction.BeginGet() ) Friction = _friction.Get( this ); return _friction.value; }
			set
			{
				if( _friction.BeginSet( ref value ) )
				{
					try
					{
						FrictionChanged?.Invoke( this );
						//!!!!no sense to recreate body. where else
						if( EnabledInHierarchy )
						{
							//var body = Parent as Component_RigidBody2D;
							//if( body != null && body.Physics2DBody != null )
							//{
							//	body.Physics2DBody.FixtureList
							//}
							//else
							RecreateBody();
						}
					}
					finally { _friction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Friction"/> property value changes.</summary>
		public event Action<Component_CollisionShape2D> FrictionChanged;
		ReferenceField<double> _friction = 0.2;

		/// <summary>
		/// The ratio of the final relative velocity to initial relative velocity of the shape after collision.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Collision Shape 2D" )]
		[Range( 0, 2 )]
		public Reference<double> Restitution
		{
			get { if( _restitution.BeginGet() ) Restitution = _restitution.Get( this ); return _restitution.value; }
			set
			{
				if( _restitution.BeginSet( ref value ) )
				{
					try
					{
						RestitutionChanged?.Invoke( this );
						//!!!!no sense to recreate body. where else
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _restitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Restitution"/> property value changes.</summary>
		public event Action<Component_CollisionShape2D> RestitutionChanged;
		ReferenceField<double> _restitution = 0.0;

		/// <summary>
		/// The collision categories this fixture is a part of.
		/// </summary>
		[DefaultValue( Category.Category1 )]
		[Category( "Collision Shape 2D" )]
		public Reference<Category> CollisionCategories
		{
			get { if( _collisionCategories.BeginGet() ) CollisionCategories = _collisionCategories.Get( this ); return _collisionCategories.value; }
			set
			{
				if( _collisionCategories.BeginSet( ref value ) )
				{
					try
					{
						CollisionCategoriesChanged?.Invoke( this );
						//!!!!no sense to recreate body. where else
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _collisionCategories.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionCategories"/> property value changes.</summary>
		public event Action<Component_CollisionShape2D> CollisionCategoriesChanged;
		ReferenceField<Category> _collisionCategories = Category.Category1;

		/// <summary>
		/// The collision mask bits. This states the categories that this fixture would accept for collision.
		/// </summary>
		[DefaultValue( Category.All )]
		[Category( "Collision Shape 2D" )]
		public Reference<Category> CollidesWith
		{
			get { if( _collidesWith.BeginGet() ) CollidesWith = _collidesWith.Get( this ); return _collidesWith.value; }
			set
			{
				if( _collidesWith.BeginSet( ref value ) )
				{
					try
					{
						CollidesWithChanged?.Invoke( this );
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _collidesWith.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollidesWith"/> property value changes.</summary>
		public event Action<Component_CollisionShape2D> CollidesWithChanged;
		ReferenceField<Category> _collidesWith = Category.All;

		/// <summary>
		/// Collision groups allow a certain group of objects to never collide or always collide. Zero means no collision group. Non-zero group filtering always wins against the mask bits.
		/// </summary>
		[DefaultValue( 0 )]
		[Category( "Collision Shape 2D" )]
		public Reference<int> CollisionGroup
		{
			get { if( _collisionGroup.BeginGet() ) CollisionGroup = _collisionGroup.Get( this ); return _collisionGroup.value; }
			set
			{
				if( _collisionGroup.BeginSet( ref value ) )
				{
					try
					{
						CollisionGroupChanged?.Invoke( this );
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _collisionGroup.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionGroup"/> property value changes.</summary>
		public event Action<Component_CollisionShape2D> CollisionGroupChanged;
		ReferenceField<int> _collisionGroup = 0;

		/////////////////////////////////////////

		protected internal abstract IList<Fixture> CreateShape( Body body, Transform shapeTransform, List<Vector2> rigidBodyLocalPoints );
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

		static Component_RigidBody2D GetRigidBody( Component start )
		{
			var p = start;
			while( p != null )
			{
				var body = p as Component_RigidBody2D;
				if( body != null )
					return body;
				p = p.Parent;
			}
			return null;
		}

		[Browsable( false )]
		public Component_RigidBody2D ParentRigidBody
		{
			get { return GetRigidBody( Parent ); }
		}

		public void RecreateBody( Component oldParent = null )
		{
			var body = GetRigidBody( oldParent != null ? oldParent : Parent );
			if( body != null )
				body.RecreateBody();
		}

		[Browsable( false )]
		public double AreaNotScaledByParent
		{
			get
			{
				if( cachedAreaNotScaledByParent < 0 )
					cachedAreaNotScaledByParent = OnCalculateArea();
				return cachedAreaNotScaledByParent;
			}
		}

		protected abstract double OnCalculateArea();

		protected void NeedUpdateCachedArea()
		{
			cachedAreaNotScaledByParent = -1;
			//body.NeedUpdateCachedMass();
		}

		//internal virtual Vector2 GetCenterOfMassPositionNotScaledByParent()
		//{
		//	return Vector2.Zero;
		//}
	}
}
