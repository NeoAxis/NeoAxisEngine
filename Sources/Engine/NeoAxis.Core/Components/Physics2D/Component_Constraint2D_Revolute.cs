// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A 2D revolute joint constrains to bodies to share a common point while they are free to rotate about the point.
	/// </summary>
	[NewObjectDefaultName( "Revolute Constraint 2D" )]
	[AddToResourcesWindow( @"Base\2D\Revolute Constraint 2D", -7995 )]
	public class Component_Constraint2D_Revolute : Component_Constraint2D
	{
		/// <summary>
		/// Whether the constraint limit is enabled.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> LimitEnabled
		{
			get { if( _limitEnabled.BeginGet() ) LimitEnabled = _limitEnabled.Get( this ); return _limitEnabled.value; }
			set
			{
				if( _limitEnabled.BeginSet( ref value ) )
				{
					try
					{
						LimitEnabledChanged?.Invoke( this );
						var c = ConstraintRigid;
						if( c != null )
							c.LimitEnabled = LimitEnabled.Value;
					}
					finally { _limitEnabled.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LimitEnabled"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Revolute> LimitEnabledChanged;
		ReferenceField<bool> _limitEnabled = false;

		/// <summary>
		/// The lower constraint limit in degrees.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -360, 360 )]
		public Reference<Degree> LimitLow
		{
			get { if( _limitLow.BeginGet() ) LimitLow = _limitLow.Get( this ); return _limitLow.value; }
			set
			{
				if( _limitLow.BeginSet( ref value ) )
				{
					try
					{
						LimitLowChanged?.Invoke( this );
						var c = ConstraintRigid;
						if( c != null )
							c.LowerLimit = (float)LimitLow.Value.InRadians();
					}
					finally { _limitLow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LimitLow"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Revolute> LimitLowChanged;
		ReferenceField<Degree> _limitLow = Degree.Zero;

		/// <summary>
		/// The upper constraint limit in degrees.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -360, 360 )]
		public Reference<Degree> LimitHigh
		{
			get { if( _limitHigh.BeginGet() ) LimitHigh = _limitHigh.Get( this ); return _limitHigh.value; }
			set
			{
				if( _limitHigh.BeginSet( ref value ) )
				{
					try
					{
						LimitHighChanged?.Invoke( this );
						var c = ConstraintRigid;
						if( c != null )
							c.UpperLimit = (float)LimitHigh.Value.InRadians();
					}
					finally { _limitHigh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LimitHigh"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Revolute> LimitHighChanged;
		ReferenceField<Degree> _limitHigh = Degree.Zero;

		/// <summary>
		/// Whether the constraint motor is enabled.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> MotorEnabled
		{
			get { if( _motorEnabled.BeginGet() ) MotorEnabled = _motorEnabled.Get( this ); return _motorEnabled.value; }
			set
			{
				if( _motorEnabled.BeginSet( ref value ) )
				{
					try
					{
						MotorEnabledChanged?.Invoke( this );
						var c = ConstraintRigid;
						if( c != null )
							c.MotorEnabled = MotorEnabled.Value;
					}
					finally { _motorEnabled.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotorEnabled"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Revolute> MotorEnabledChanged;
		ReferenceField<bool> _motorEnabled = false;

		/// <summary>
		/// The motor speed in degrees per second.
		/// </summary>
		[Range( -2000, 2000 )]
		[DefaultValue( 0.0 )]
		public Reference<Degree> MotorSpeed
		{
			get { if( _motorSpeed.BeginGet() ) MotorSpeed = _motorSpeed.Get( this ); return _motorSpeed.value; }
			set
			{
				if( _motorSpeed.BeginSet( ref value ) )
				{
					try
					{
						MotorSpeedChanged?.Invoke( this );
						var c = ConstraintRigid;
						if( c != null )
							c.MotorSpeed = (float)MotorSpeed.Value.InRadians();
					}
					finally { _motorSpeed.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotorSpeed"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Revolute> MotorSpeedChanged;
		ReferenceField<Degree> _motorSpeed = Degree.Zero;

		/// <summary>
		/// The maximum motor torque, usually in N-m.
		/// </summary>
		[Range( -1000, 1000 )]
		[DefaultValue( 0.0 )]
		public Reference<double> MotorMaxTorque
		{
			get { if( _motorMaxTorque.BeginGet() ) MotorMaxTorque = _motorMaxTorque.Get( this ); return _motorMaxTorque.value; }
			set
			{
				if( _motorMaxTorque.BeginSet( ref value ) )
				{
					try
					{
						MotorMaxTorqueChanged?.Invoke( this );
						var c = ConstraintRigid;
						if( c != null )
							c.MaxMotorTorque = (float)MotorMaxTorque.Value;
					}
					finally { _motorMaxTorque.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotorMaxTorque"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Revolute> MotorMaxTorqueChanged;
		ReferenceField<double> _motorMaxTorque = 0.0;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( LimitLow ):
				case nameof( LimitHigh ):
					if( !LimitEnabled )
						skip = true;
					break;

				case nameof( MotorSpeed ):
				case nameof( MotorMaxTorque ):
					if( !MotorEnabled )
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

			c.LimitEnabled = LimitEnabled;
			c.LowerLimit = (float)LimitLow.Value.InRadians();
			c.UpperLimit = (float)LimitHigh.Value.InRadians();

			c.MotorEnabled = MotorEnabled;
			c.MotorSpeed = (float)MotorSpeed.Value.InRadians();
			c.MaxMotorTorque = (float)MotorMaxTorque.Value;
		}

		public override void Render( ViewportRenderingContext context, out int verticesRendered )
		{
			base.Render( context, out verticesRendered );

			var context2 = context.objectInSpaceRenderingContext;
			var renderer = context.Owner.Simple3DRenderer;

			if( ConstraintRigid != null && renderer != null )
			{
				var tr = TransformV;
				var pos = tr.Position;

				{
					var color = new ColorValue( 0, 0, 0, 0.4 );
					renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
					renderer.AddLine( pos, pos + tr.Rotation * new Vector3( 1, 0, 0 ) );
					verticesRendered += 2;
				}

				if( LimitEnabled )
				{
					var color = new ColorValue( 1, 0, 0 );

					renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

					var r = tr.Rotation * Quaternion.FromRotateByZ( -ConstraintRigid.LowerLimit );
					renderer.AddLine( pos, pos + r * new Vector3( 1, 0, 0 ) );

					r = tr.Rotation * Quaternion.FromRotateByZ( -ConstraintRigid.UpperLimit );
					renderer.AddLine( pos, pos + r * new Vector3( 1, 0, 0 ) );

					verticesRendered += 4;
				}

				{
					var color = new ColorValue( 0, 0, 0 );

					var rotationOffset = Quaternion.FromRotateByZ( -ConstraintRigid.JointAngle );
					var rot = tr.Rotation * rotationOffset;

					renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
					renderer.AddLine( pos, pos + rot * new Vector3( 1, 0, 0 ) );
					verticesRendered += 2;
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
