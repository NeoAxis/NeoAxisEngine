// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public interface IProcessDamage
	{
		void ProcessDamage( long whoFired, float damage, object anyData );
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public enum PhysicsModeEnum
	{
		None,
		//Kinematic,
		Basic
	}
}
