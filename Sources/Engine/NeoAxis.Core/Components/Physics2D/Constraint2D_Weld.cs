// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Internal.tainicom.Aether.Physics2D.Dynamics.Joints;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A weld joint essentially glues two bodies together. The joint is soft constraint based, which means the two bodies will move relative to each other.
	/// </summary>
	[NewObjectDefaultName( "Weld Constraint 2D" )]
	[AddToResourcesWindow( @"Base\2D\Weld Constraint 2D", -7992 )]
	public class Constraint2D_Weld : Constraint2D
	{
		/// <summary>
		/// The frequency of the joint. A higher frequency means a stiffer joint, but a too high value can cause the joint to oscillate. Default is 0, which means the joint does no spring calculations.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> Frequency
		{
			get { if( _frequency.BeginGet() ) Frequency = _frequency.Get( this ); return _frequency.value; }
			set { if( _frequency.BeginSet( ref value ) ) { try { FrequencyChanged?.Invoke( this ); } finally { _frequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Frequency"/> property value changes.</summary>
		public event Action<Constraint2D_Weld> FrequencyChanged;
		ReferenceField<double> _frequency = 1.0;

		/// <summary>
		/// The damping on the joint. The damping is only used when the joint has a frequency (> 0).
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> DampingRatio
		{
			get { if( _dampingRatio.BeginGet() ) DampingRatio = _dampingRatio.Get( this ); return _dampingRatio.value; }
			set { if( _dampingRatio.BeginSet( ref value ) ) { try { DampingRatioChanged?.Invoke( this ); } finally { _dampingRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DampingRatio"/> property value changes.</summary>
		public event Action<Constraint2D_Weld> DampingRatioChanged;
		ReferenceField<double> _dampingRatio = 0.0;

		/////////////////////////////////////////

		[Browsable( false )]
		public new WeldJoint ConstraintRigid
		{
			get { return (WeldJoint)base.ConstraintRigid; }
		}

		protected override Joint OnCreateConstraint( PhysicalBody2D creationBodyA, PhysicalBody2D creationBodyB )
		{
			var anchor = Physics2DUtility.Convert( TransformV.Position.ToVector2() );
			return new WeldJoint( creationBodyA.Physics2DBody, creationBodyB.Physics2DBody, anchor, anchor, true );
		}

		protected override void OnCreateConstraintApplyParameters()
		{
			base.OnCreateConstraintApplyParameters();

			var c = ConstraintRigid;
			c.FrequencyHz = (float)Frequency;
			c.DampingRatio = (float)DampingRatio;
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
