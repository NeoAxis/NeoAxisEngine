// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using Internal.tainicom.Aether.Physics2D.Dynamics.Joints;

namespace NeoAxis
{
	/// <summary>
	/// A 2D distance joint rains two points on two bodies to remain at a fixed distance from each other.
	/// </summary>
	[NewObjectDefaultName( "Distance Constraint 2D" )]
	[AddToResourcesWindow( @"Base\2D\Distance Constraint 2D", -7993 )]
	public class Constraint2D_Distance : Constraint2D
	{
		/// <summary>
		/// The mass-spring-damper frequency in Hertz. A value of 0 disables softness.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> Frequency
		{
			get { if( _frequency.BeginGet() ) Frequency = _frequency.Get( this ); return _frequency.value; }
			set { if( _frequency.BeginSet( ref value ) ) { try { FrequencyChanged?.Invoke( this ); } finally { _frequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Frequency"/> property value changes.</summary>
		public event Action<Constraint2D_Distance> FrequencyChanged;
		ReferenceField<double> _frequency = 0.0;

		/// <summary>
		/// The damping ratio. 0 = no damping, 1 = critical damping.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> DampingRatio
		{
			get { if( _dampingRatio.BeginGet() ) DampingRatio = _dampingRatio.Get( this ); return _dampingRatio.value; }
			set { if( _dampingRatio.BeginSet( ref value ) ) { try { DampingRatioChanged?.Invoke( this ); } finally { _dampingRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DampingRatio"/> property value changes.</summary>
		public event Action<Constraint2D_Distance> DampingRatioChanged;
		ReferenceField<double> _dampingRatio = 0.0;

		/////////////////////////////////////////

		[Browsable( false )]
		public new DistanceJoint ConstraintRigid
		{
			get { return (DistanceJoint)base.ConstraintRigid; }
		}

		protected override Joint OnCreateConstraint( PhysicalBody2D creationBodyA, PhysicalBody2D creationBodyB )
		{
			var anchor1 = Physics2DUtility.Convert( TransformV.Position.ToVector2() );
			var anchor2 = Physics2DUtility.Convert( ( TransformV * new Vector3( 1, 0, 0 ) ).ToVector2() );
			return new DistanceJoint( creationBodyA.Physics2DBody, creationBodyB.Physics2DBody, anchor1, anchor2, true );
		}

		protected override void OnCreateConstraintApplyParameters()
		{
			base.OnCreateConstraintApplyParameters();

			var c = ConstraintRigid;
			c.Frequency = (float)Frequency;
			c.DampingRatio = (float)DampingRatio;
		}

		public override void Render( ViewportRenderingContext context, out int verticesRendered )
		{
			base.Render( context, out verticesRendered );

			var context2 = context.ObjectInSpaceRenderingContext;
			var renderer = context.Owner.Simple3DRenderer;

			if( /*ConstraintRigid != null && */renderer != null )
			{
				var tr = TransformV;
				var pos = tr.Position;

				{
					var color = new ColorValue( 0, 0, 0 );
					renderer.SetColor( color, color * ProjectSettings.Get.General.HiddenByOtherObjectsColorMultiplier );

					renderer.AddLine( pos, tr * new Vector3( 1, 0, 0 ) );
					verticesRendered += 2;

					var point1 = tr.Position;
					var point2 = tr * new Vector3( 1, 0, 0 );

					var dimensions = new Vector2( tr.Scale.X / 10, tr.Scale.X / 10 );

					renderer.AddEllipse( dimensions, 16, Matrix4.FromTranslate( point1 ), true );
					renderer.AddEllipse( dimensions, 16, Matrix4.FromTranslate( point2 ), true );

					verticesRendered += 17 * 2;
				}

			}
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
					var anchorA = new Vector3( Physics2DUtility.Convert( ConstraintRigid.WorldAnchorA ), 0 );
					var anchorB = new Vector3( Physics2DUtility.Convert( ConstraintRigid.WorldAnchorB ), 0 );
					var rot = Quaternion.FromDirectionZAxisUp( ( anchorB - anchorA ).GetNormalize() );

					var transformA = rigidBodyTransform.ToMatrix4() * ConstraintAFrame;
					transformA.Decompose( out Vector3 translation, out Matrix3 rotation, out Vector3 scale );
					var oldT = Transform.Value;
					var newT = new Transform( translation, rot, oldT.Scale );
					//var newT = new Transform( translation, rotation.ToQuaternion(), oldT.Scale );

					SetTransformWithoutUpdatingConstraint( newT );
				}
			}
		}
	}
}
