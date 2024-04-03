// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Constraint_SixDOF_NewObjectSettings : NewObjectSettings
	{
		/// <summary>
		/// Enumerates the types of constraint link between two physical bodies.
		/// </summary>
		public enum InitialConfigurationEnum
		{
			Fixed,
			Hinge,
			Point,
			ConeTwist,
			Slider,
		}

		InitialConfigurationEnum initialConfiguration = InitialConfigurationEnum.Fixed;

		[DefaultValue( InitialConfigurationEnum.Fixed )]
		[Category( "Options" )]
		public InitialConfigurationEnum InitialConfiguration
		{
			get { return initialConfiguration; }
			set { initialConfiguration = value; }
		}

		public override bool Creation( NewObjectCell.ObjectCreationContext context )
		{
			var c = (Constraint_SixDOF)context.newObject;

			//!!!!Constraint: создание: если выделены тела, то констрейнт становится чилдом
			////get bodies from selected objects
			//{
			//	//!!!!пока так. надо брать из документа
			//	var selectedObjects = SettingsWindow.Instance._SelectedObjects;
			//	if( selectedObjects.Length == 2 )
			//	{
			//		var bodyA = selectedObjects[ 0 ] as RigidBody;
			//		var bodyB = selectedObjects[ 1 ] as RigidBody;
			//		if( bodyA != null && bodyB != null )
			//		{
			//			c.BodyA = new Reference<RigidBody>( null, ReferenceUtils.CalculateThisReference( c, bodyA ) );
			//			c.BodyB = new Reference<RigidBody>( null, ReferenceUtils.CalculateThisReference( c, bodyB ) );

			//			//!!!!можно луч от курсора учитывать
			//			var pos = ( bodyA.Transform.Value.Position + bodyB.Transform.Value.Position ) * 0.5;
			//			c.Transform = new Transform( pos, Quat.Identity );
			//		}
			//	}
			//}

			switch( InitialConfiguration )
			{
			case InitialConfigurationEnum.Hinge:
				c.AngularZAxis = PhysicsAxisMode.Free;
				break;

			case InitialConfigurationEnum.Point:
				c.AngularXAxis = PhysicsAxisMode.Free;
				c.AngularYAxis = PhysicsAxisMode.Free;
				c.AngularZAxis = PhysicsAxisMode.Free;
				break;

			case InitialConfigurationEnum.ConeTwist:
				//!!!!так?
				c.AngularXAxis = PhysicsAxisMode.Free;
				c.AngularYAxis = PhysicsAxisMode.Limited;
				c.AngularZAxis = PhysicsAxisMode.Limited;
				break;

			case InitialConfigurationEnum.Slider:
				c.LinearXAxis = PhysicsAxisMode.Free;
				break;
			}

			return base.Creation( context );
		}
	}
}
