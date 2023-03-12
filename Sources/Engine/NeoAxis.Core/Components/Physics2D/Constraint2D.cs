// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Internal.tainicom.Aether.Physics2D.Dynamics;
using Internal.tainicom.Aether.Physics2D.Dynamics.Joints;

namespace NeoAxis
{
	/// <summary>
	/// Base class for constraint link between two physical 2D bodies.
	/// </summary>
	public abstract class Constraint2D : ObjectInSpace, IPhysicalObject
	{
		bool created;
		PhysicalBody2D creationBodyA;
		PhysicalBody2D creationBodyB;
		Joint constraintRigid;
		Matrix4 constraintAFrame = Matrix4.Identity;
		//int softAnchorModeClosestNodeIndex = -1;
		////int closestFaceIdx = -1;

		bool duringCreateDestroy;
		bool setTransformWithoutUpdatingConstraint;

		/////////////////////////////////////////

		/// <summary>
		/// The first physical body used.
		/// </summary>
		[Category( "Constraint 2D" )]
		public Reference<PhysicalBody2D> BodyA
		{
			get { if( _bodyA.BeginGet() ) BodyA = _bodyA.Get( this ); return _bodyA.value; }
			set
			{
				if( _bodyA.BeginSet( ref value ) )
				{
					try
					{
						BodyAChanged?.Invoke( this );
						RecreateConstraint();
					}
					finally { _bodyA.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BodyA"/> property value changes.</summary>
		public event Action<Constraint2D> BodyAChanged;
		ReferenceField<PhysicalBody2D> _bodyA;

		/// <summary>
		/// The second physical body used.
		/// </summary>
		[Category( "Constraint 2D" )]
		public Reference<PhysicalBody2D> BodyB
		{
			get { if( _bodyB.BeginGet() ) BodyB = _bodyB.Get( this ); return _bodyB.value; }
			set
			{
				if( _bodyB.BeginSet( ref value ) )
				{
					try
					{
						BodyBChanged?.Invoke( this );
						RecreateConstraint();
					}
					finally { _bodyB.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="BodyB"/> property value changes.</summary>
		public event Action<Constraint2D> BodyBChanged;
		ReferenceField<PhysicalBody2D> _bodyB;

		/// <summary>
		/// Whether the collision detection is enabled between the linked physical bodies.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Constraint 2D" )]
		public Reference<bool> CollisionsBetweenLinkedBodies
		{
			get { if( _collisionsBetweenLinkedBodies.BeginGet() ) CollisionsBetweenLinkedBodies = _collisionsBetweenLinkedBodies.Get( this ); return _collisionsBetweenLinkedBodies.value; }
			set
			{
				if( _collisionsBetweenLinkedBodies.BeginSet( ref value ) )
				{
					try
					{
						CollisionsBetweenLinkedBodiesChanged?.Invoke( this );
						if( constraintRigid != null )
							constraintRigid.CollideConnected = _collisionsBetweenLinkedBodies.value;
					}
					finally { _collisionsBetweenLinkedBodies.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionsBetweenLinkedBodies"/> property value changes.</summary>
		public event Action<Constraint2D> CollisionsBetweenLinkedBodiesChanged;
		ReferenceField<bool> _collisionsBetweenLinkedBodies = true;

		/// <summary>
		/// The Breakpoint indicates the maximum value the joint error can be before it breaks.
		/// </summary>
		[DefaultValue( 100000000.0 )]
		[Category( "Constraint 2D" )]
		public Reference<double> Breakpoint
		{
			get { if( _breakpoint.BeginGet() ) Breakpoint = _breakpoint.Get( this ); return _breakpoint.value; }
			set
			{
				if( _breakpoint.BeginSet( ref value ) )
				{
					try
					{
						BreakpointChanged?.Invoke( this );
						if( constraintRigid != null )
							constraintRigid.Breakpoint = (float)Breakpoint;
					}
					finally { _breakpoint.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Breakpoint"/> property value changes.</summary>
		public event Action<Constraint2D> BreakpointChanged;
		ReferenceField<double> _breakpoint = 100000000.0;

		/////////////////////////////////////////

		World GetPhysicsWorldData( bool canInit )
		{
			var scene = ParentScene;
			if( scene != null )
				return scene.Physics2DGetWorld( canInit );
			return null;
		}

		protected abstract Joint OnCreateConstraint( PhysicalBody2D creationBodyA, PhysicalBody2D creationBodyB );

		protected virtual void OnCreateConstraintApplyParameters()
		{
			var c = ConstraintRigid;

			c.Breakpoint = (float)Breakpoint;
		}

		void CreateConstraint()
		{
			if( constraintRigid != null )
				Log.Fatal( "Constraint2D: CreateConstraint: constraintRigid != null." );
			if( !EnabledInHierarchy )
				Log.Fatal( "Constraint2D: CreateConstraint: !EnabledInHierarchy." );

			var physicsWorldData = GetPhysicsWorldData( true );
			if( physicsWorldData == null )
				return;

			duringCreateDestroy = true;
			try
			{
				var ba = BodyA.Value;
				var bb = BodyB.Value;

				//check bodies are enabled
				if( ba != null && ba.Physics2DBody == null )
					return;
				if( bb != null && bb.Physics2DBody == null )
					return;
				//check no bodies
				if( ba == null && bb == null )
					return;

				created = true;
				creationBodyA = ba;
				creationBodyB = bb;

				//transform without Scale
				var transform = Transform.Value;
				var t = transform.ToMatrix4();
				//var t = new Matrix4( transform.Rotation.ToMatrix3(), transform.Position );

				if( creationBodyA != null && creationBodyB != null )
				{
					constraintRigid = OnCreateConstraint( creationBodyA, creationBodyB );

					var bodyATransformFull = creationBodyA.Transform.Value;
					var bodyATransform = new Matrix4( bodyATransformFull.Rotation.ToMatrix3(), bodyATransformFull.Position );
					constraintAFrame = bodyATransform.GetInverse() * t;
				}
				//else
				//{
				//var body = creationBodyA ?? creationBodyB;

				//var bodyTransformFull = body.Transform.Value;
				//var bodyTransform = new Matrix4( bodyTransformFull.Rotation.ToMatrix3(), bodyTransformFull.Position );
				//constraintAFrame = bodyTransform.GetInverse() * t;

				//var mat = BulletPhysicsUtility.Convert( constraintAFrame );
				//constraintRigid = new Generic6DofSpring2Constraint( (RigidBody)body.BulletBody, mat );
				//}

				if( constraintRigid != null )
				{
					constraintRigid.CollideConnected = CollisionsBetweenLinkedBodies;
					OnCreateConstraintApplyParameters();
					constraintRigid.Tag = this;

					physicsWorldData.Add( constraintRigid );
				}
			}
			finally
			{
				duringCreateDestroy = false;
			}
		}

		public void DestroyConstraint()
		{
			duringCreateDestroy = true;

			var physicsWorldData = GetPhysicsWorldData( false );
			if( physicsWorldData != null )
			{
				if( constraintRigid != null )
				{
					if( physicsWorldData.JointList.Contains( constraintRigid ) )
						physicsWorldData.Remove( constraintRigid );
				}
			}

			created = false;
			creationBodyA = null;
			creationBodyB = null;
			constraintRigid = null;
			//softAnchorModeClosestNodeIndex = -1;

			duringCreateDestroy = false;
		}

		public void RecreateConstraint()
		{
			if( EnabledInHierarchy && !duringCreateDestroy )
			{
				DestroyConstraint();
				CreateConstraint();
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( GetPhysicsWorldData( true ) != null )
			{
				if( EnabledInHierarchy )
				{
					if( !created )
						CreateConstraint();
				}
				else
					DestroyConstraint();
			}
		}

		protected override void OnTransformChanged()
		{
			if( created && !setTransformWithoutUpdatingConstraint )
			{
				//recreate
				RecreateConstraint();
			}

			base.OnTransformChanged();
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() || !ParentScene.DisplayLabels )
				return false;
			return base.OnEnabledSelectionByCursor();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			//create when must be created. can happen after undo of deletion physical body.
			if( EnabledInHierarchy && !created && GetPhysicsWorldData( false ) != null )
				CreateConstraint();
		}

		public virtual void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered )
		{
			verticesRendered = 0;

			//var context2 = context.objectInSpaceRenderingContext;
			//var renderer = context.Owner.Simple3DRenderer;

			//if( !Broken )
			//{
			//	Vec3 anchor = Anchor;
			//	Vec3 halfDir = Axis.Direction * .5f;
			//	debugGeometry.AddLine( anchor - halfDir, anchor + halfDir );
			//}
		}

		public void SetTransformWithoutUpdatingConstraint( Transform transform )
		{
			try
			{
				setTransformWithoutUpdatingConstraint = true;
				Transform = transform;
			}
			finally
			{
				setTransformWithoutUpdatingConstraint = false;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//if( member is Metadata.Property )
			//{
			//switch( member.Name )
			//{
			//case nameof( BreakingImpulseThreshold ):
			//case nameof( OverrideNumberSolverIterations ):
			//	if( SoftBodyMode )
			//		skip = true;
			//	break;
			//}
			//}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Browsable( false )]
		public Joint ConstraintRigid
		{
			get { return constraintRigid; }
		}

		public abstract void UpdateDataFromPhysicsEngine();

		[Browsable( false )]
		protected Matrix4 ConstraintAFrame
		{
			get { return constraintAFrame; }
		}
	}
}
