// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Task-based artificial intelligence.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\AI", -9997 )]
	public class Component_AI : Component
	{
		[Browsable( false )]
		public Component_AITask CurrentTask
		{
			get { return GetComponent<Component_AITask>( onlyEnabledInHierarchy: true ); }
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			CurrentTask?.PerformTaskSimulationStep();
		}

		public void ClearTaskQueue()
		{
			foreach( var task in GetComponents<Component_AITask>().Reverse() )
				task.Dispose();
		}
	}
}