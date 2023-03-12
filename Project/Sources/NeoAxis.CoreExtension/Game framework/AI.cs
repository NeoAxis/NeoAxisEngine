// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	public class AI : Component, InteractiveObject
	{
		[Browsable( false )]
		public AITask CurrentTask
		{
			get { return GetComponent<AITask>( onlyEnabledInHierarchy: true ); }
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//!!!!реже вызывать

			CurrentTask?.PerformTaskSimulationStep();
		}

		public void ClearTaskQueue()
		{
			if( Components.Count != 0 )
			{
				foreach( var task in GetComponents<AITask>().Reverse() )
					task.Dispose();
			}
		}

		public virtual void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			return false;
		}

		public virtual void ObjectInteractionEnter( ObjectInteractionContext context )
		{
		}

		public virtual void ObjectInteractionExit( ObjectInteractionContext context )
		{
		}

		public virtual void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
		}
	}
}