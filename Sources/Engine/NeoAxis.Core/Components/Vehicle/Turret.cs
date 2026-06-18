// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// An instance of a turret.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Vehicle\Turret", 22010 )]
	[NewObjectDefaultName( "Turret" )]
	public class Turret : MeshInSpace
	{
		//Turret class is used to mark controllable components. To control by means physics constraints.
	}
}
