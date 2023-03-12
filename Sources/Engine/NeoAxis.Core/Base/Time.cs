// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Class for working with time in the engine.
	/// </summary>
	public static class Time
	{
		static float simulationDeltaCached;

		/// <summary>
		/// Gets simulation time step.
		/// </summary>
		public static float SimulationDelta
		{
			get 
			{
				if( simulationDeltaCached == 0 )
					simulationDeltaCached = ProjectSettings.Get.General.SimulationStepsPerSecondInv;
				return simulationDeltaCached;
			}
		}

		/// <summary>
		/// Gets the current time in the engine.
		/// </summary>
		public static double Current
		{
			get { return EngineApp.EngineTime; }
		}
	}
}
