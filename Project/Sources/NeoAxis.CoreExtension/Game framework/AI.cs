// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Task-based artificial intelligence.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\AI", -9997 )]
	public class AI : Component
	{
		[Browsable( false )]
		public AITask CurrentTask
		{
			get { return GetComponent<AITask>( onlyEnabledInHierarchy: true ); }
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			CurrentTask?.PerformTaskSimulationStep();
		}

		public void ClearTaskQueue()
		{
			foreach( var task in GetComponents<AITask>().Reverse() )
				task.Dispose();
		}
	}
}