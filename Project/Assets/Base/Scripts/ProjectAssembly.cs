// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	public class ProjectAssembly : AssemblyUtility.AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
			{
				EngineApp.AppCreateBefore += SimulationApp.EngineApp_AppCreateBefore;
				EngineApp.AppCreateAfter += SimulationApp.EngineApp_AppCreateAfter;
				EngineApp.AppDestroy += SimulationApp.EngineApp_AppDestroy;
			}
		}
	}
}
