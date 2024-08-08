// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// A class for working with time in the engine.
	/// </summary>
	public static class Time
	{
		static float simulationDeltaCached;

		/// <summary>
		/// Gets the time step of the simulation.
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
		/// Gets the current time in the engine. The engine time is updated once before a simulation step or before a frame update if it is an editor.
		/// </summary>
		public static double Current
		{
			get { return EngineApp.EngineTime; }
		}
	}
}
