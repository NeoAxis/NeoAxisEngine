// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace NeoAxis
{
	/// <summary>
	/// A 2D prismatic joint. This joint provides one degree of freedom: translation along an axis fixed in body A. Relative rotation is prevented.
	/// </summary>
	[NewObjectDefaultName( "Prismatic Constraint 2D" )]
	[AddToResourcesWindow( @"Base\2D\Prismatic Constraint 2D", -7994 )]
	public class Component_Constraint2D_Prismatic : Component_Constraint2D
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
		public event Action<Component_Constraint2D_Prismatic> LimitEnabledChanged;
		ReferenceField<bool> _limitEnabled = false;

		/// <summary>
		/// The lower constraint limit in world units (usually in meters).
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -100, 100 )]
		public Reference<double> LimitLow
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
							c.LowerLimit = (float)LimitLow;
					}
					finally { _limitLow.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LimitLow"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Prismatic> LimitLowChanged;
		ReferenceField<double> _limitLow = 0.0;

		/// <summary>
		/// The upper constraint limit in world units (usually in meters).
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( -100, 100 )]
		public Reference<double> LimitHigh
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
							c.UpperLimit = (float)LimitHigh;
					}
					finally { _limitHigh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LimitHigh"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Prismatic> LimitHighChanged;
		ReferenceField<double> _limitHigh = 0.0;

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
		public event Action<Component_Constraint2D_Prismatic> MotorEnabledChanged;
		ReferenceField<bool> _motorEnabled = false;

		/// <summary>
		/// The motor speed in world units (usually meters) per second.
		/// </summary>
		[Range( -2000, 2000 )]
		[DefaultValue( 0.0 )]
		public Reference<double> MotorSpeed
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
							c.MotorSpeed = (float)MotorSpeed;
					}
					finally { _motorSpeed.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotorSpeed"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Prismatic> MotorSpeedChanged;
		ReferenceField<double> _motorSpeed = 0.0;

		/// <summary>
		/// The maximum motor torque, usually in N.
		/// </summary>
		[Range( -1000, 1000 )]
		[DefaultValue( 0.0 )]
		public Reference<double> MotorMaxForce
		{
			get { if( _motorMaxForce.BeginGet() ) MotorMaxForce = _motorMaxForce.Get( this ); return _motorMaxForce.value; }
			set
			{
				if( _motorMaxForce.BeginSet( ref value ) )
				{
					try
					{
						MotorMaxForceChanged?.Invoke( this );
						var c = ConstraintRigid;
						if( c != null )
							c.MaxMotorForce = (float)MotorMaxForce;
					}
					finally { _motorMaxForce.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotorMaxForce"/> property value changes.</summary>
		public event Action<Component_Constraint2D_Prismatic> MotorMaxForceChanged;
		ReferenceField<double> _motorMaxForce = 0.0;

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
				case nameof( MotorMaxForce ):
					if( !MotorEnabled )
						skip = true;
					break;
				}
			}
		}

		[Browsable( false )]
		public new PrismaticJoint ConstraintRigid
		{
			get { return (PrismaticJoint)base.ConstraintRigid; }
		}

		protected override Joint OnCreateConstraint( Component_PhysicalBody2D creationBodyA, Component_PhysicalBody2D creationBodyB )
		{
			var anchor = Physics2DUtility.Convert( TransformV.Position.ToVector2() );

			var angle = -MathEx.DegreeToRadian( TransformV.Rotation.ToAngles().Yaw );
			var axis = Physics2DUtility.Convert( new Vector2( Math.Cos( angle ), Math.Sin( angle ) ) );

			return new PrismaticJoint( creationBodyA.Physics2DBody, creationBodyB.Physics2DBody, anchor, anchor, axis, true );
		}

		protected override void OnCreateConstraintApplyParameters()
		{
			base.OnCreateConstraintApplyParameters();

			var c = ConstraintRigid;

			//var angle = MathEx.DegreeToRadian( TransformV.Rotation.ToAngles().Yaw );
			c.ReferenceAngle = c.BodyB.Rotation - c.BodyA.Rotation;// - (float)angle;

			c.LimitEnabled = LimitEnabled;
			c.LowerLimit = (float)LimitLow;
			c.UpperLimit = (float)LimitHigh;

			c.MotorEnabled = MotorEnabled;
			c.MotorSpeed = (float)MotorSpeed;
			c.MaxMotorForce = (float)MotorMaxForce;
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

				double low = -1;
				double high = 1;
				if( LimitEnabled )
				{
					low = LimitLow;
					high = LimitHigh;
				}

				var scale = ( Math.Abs( low ) + Math.Abs( high ) ) / 20;

				var posLow = pos + tr.Rotation * new Vector3( low, 0, 0 );
				var posHigh = pos + tr.Rotation * new Vector3( high, 0, 0 );

				{
					var color = new ColorValue( 0, 0, 0 );
					renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
					renderer.AddLine( pos + tr.Rotation * new Vector3( low, 0, 0 ), pos + tr.Rotation * new Vector3( high, 0, 0 ) );
					verticesRendered += 2;
				}

				{
					var color = new ColorValue( 0, 0, 0, 0.4 );
					var p1 = pos + tr.Rotation * new Vector3( 0, scale, 0 );
					var p2 = pos + tr.Rotation * new Vector3( 0, -scale, 0 );
					renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
					renderer.AddLine( p1, p2 );
					verticesRendered += 2;
				}

				if( LimitEnabled )
				{
					var color = new ColorValue( 1, 0, 0 );
					renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
					renderer.AddLine(
						posLow + tr.Rotation * new Vector3( 0, scale, 0 ),
						posLow + tr.Rotation * new Vector3( 0, -scale, 0 ) );
					renderer.AddLine(
						posHigh + tr.Rotation * new Vector3( 0, scale, 0 ),
						posHigh + tr.Rotation * new Vector3( 0, -scale, 0 ) );
					verticesRendered += 4;
				}

				{
					var color = new ColorValue( 0, 0, 0 );

					var posCurrent = pos + tr.Rotation * new Vector3( ConstraintRigid.JointTranslation, 0, 0 );

					var p1 = posCurrent + tr.Rotation * new Vector3( 0, scale, 0 );
					var p2 = posCurrent + tr.Rotation * new Vector3( 0, -scale, 0 );
					renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
					renderer.AddLine( p1, p2 );
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
