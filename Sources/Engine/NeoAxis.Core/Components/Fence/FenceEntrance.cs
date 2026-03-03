// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents the functionality of a fence entrance.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Fence\Fence Entrance", 520 )]
	public class FenceEntrance : ObjectInSpace
	{
		//protected override void OnEnabledInSimulation()
		//{
		//	base.OnEnabledInSimulation();

		//	//detach dynamic physical bodies
		//	foreach( var body in GetComponents<RigidBody>( checkChildren: true ) )
		//	{
		//		if( body.MotionType.Value == RigidBody.MotionTypeEnum.Dynamic )
		//		{
		//			//reset reference
		//			if( body.Transform.ReferenceSpecified )
		//				body.Transform = body.Transform.Value;
		//		}

		//	}
		//}
	}
}
