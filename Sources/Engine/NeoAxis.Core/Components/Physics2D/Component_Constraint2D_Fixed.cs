// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A 2D joint hard constraints two bodies.
	/// </summary>
	[NewObjectDefaultName( "Fixed Constraint 2D" )]
	[AddToResourcesWindow( @"Base\2D\Fixed Constraint 2D", -7991 )]
	public class Component_Constraint2D_Fixed : Component_Constraint2D
	{
		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( CollisionsBetweenLinkedBodies ):
					skip = true;
					break;
				}
			}
		}

		[Browsable( false )]
		public new RevoluteJoint ConstraintRigid
		{
			get { return (RevoluteJoint)base.ConstraintRigid; }
		}

		protected override Joint OnCreateConstraint( Component_PhysicalBody2D creationBodyA, Component_PhysicalBody2D creationBodyB )
		{
			var anchor = Physics2DUtility.Convert( TransformV.Position.ToVector2() );
			return new RevoluteJoint( creationBodyA.Physics2DBody, creationBodyB.Physics2DBody, anchor, true );
		}

		protected override void OnCreateConstraintApplyParameters()
		{
			base.OnCreateConstraintApplyParameters();

			var c = ConstraintRigid;

			c.CollideConnected = false;
			c.LimitEnabled = true;
			c.LowerLimit = 0;
			c.UpperLimit = 0;
		}

		public override void UpdateDataFromPhysicsEngine()
		{
			if( ConstraintRigid != null )
			{
				Transform rigidBodyTransform;

				{
					var rigidBody = ConstraintRigid.BodyA;

					var position = Physics2DUtility.Convert( rigidBody.Position );
					var rotation = -rigidBody.Rotation;

					var oldT = Transform.Value;

					Matrix3F.FromRotateByZ( rotation, out var mat3 );
					var rot2 = mat3.ToQuaternion();
					//var rot2 = new Angles( 0, 0, MathEx.RadianToDegree( rot ) );

					rigidBodyTransform = new Transform( new Vector3( position.X, position.Y, oldT.Position.Z ), rot2 );
				}

				{
					var transformA = rigidBodyTransform.ToMatrix4() * ConstraintAFrame;
					transformA.Decompose( out Vector3 translation, out Matrix3 rotation, out Vector3 scale );
					var oldT = Transform.Value;
					var newT = new Transform( translation, rotation.ToQuaternion(), oldT.Scale );

					SetTransformWithoutUpdatingConstraint( newT );
				}
			}
		}

	}
}
