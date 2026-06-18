// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	public class ProjectAssembly : AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.IsSimulation )
			{
				EngineApp.AppCreateBefore += SimulationApp.EngineApp_AppCreateBefore;
				EngineApp.AppCreateAfter += SimulationApp.EngineApp_AppCreateAfter;
				EngineApp.AppDestroy += SimulationApp.EngineApp_AppDestroy;
			}
		}
	}
}
