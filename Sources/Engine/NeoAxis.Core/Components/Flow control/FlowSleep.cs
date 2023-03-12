// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// A flow control component representing the sleeping for the specified number of seconds.
	/// </summary>
	public class FlowSleep : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
	{
		//Entry
		/// <summary>
		/// The input of the node.
		/// </summary>
		public FlowInput Entry
		{
			get { return new FlowInput( this, nameof( Entry ) ); }
		}

		/// <summary>
		/// The amount of seconds to sleep.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> Seconds
		{
			get { if( _seconds.BeginGet() ) Seconds = _seconds.Get( this ); return _seconds.value; }
			set { if( _seconds.BeginSet( ref value ) ) { try { SecondsChanged?.Invoke( this ); } finally { _seconds.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Seconds"/> property value changes.</summary>
		public event Action<FlowSleep> SecondsChanged;
		ReferenceField<double> _seconds = 1.0;

		/// <summary>
		/// The output of the node.
		/// </summary>
		[DefaultValue( null )]
		public Reference<FlowInput> Exit
		{
			get { if( _exit.BeginGet() ) Exit = _exit.Get( this ); return _exit.value; }
			set { if( _exit.BeginSet( ref value ) ) { try { ExitChanged?.Invoke( this ); } finally { _exit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Exit"/> property value changes.</summary>
		public event Action<FlowSleep> ExitChanged;
		ReferenceField<FlowInput> _exit;

		/////////////////////////////////////////

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		class ExecutionData
		{
			public double totalTime;
			public double currentTime;
			public double lastSimulationTime;
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			bool isEntry = entryItem.FlowInput != null && entryItem.FlowInput.PropertyName == nameof( Entry );

			ExecutionData executionData = null;
			if( isEntry )
			{
				executionData = new ExecutionData();
				executionData.totalTime = Seconds;
			}
			else
			{
				if( flow.InternalVariables.TryGetValue( this, out var value ) )
				{
					executionData = (ExecutionData)value;

					double simulationTime;
					double delta;

					var controller = flow.Owner;
					if( controller != null )
					{
						simulationTime = controller.SimulationTime;
						delta = Time.SimulationDelta;
					}
					else
					{
						simulationTime = EngineApp.EngineTime;
						delta = Flow.globalSleepingFlowsTimeDelta;
					}

					if( simulationTime != executionData.lastSimulationTime )
					{
						executionData.currentTime += delta;
						executionData.lastSimulationTime = simulationTime;
					}
				}
			}

			if( executionData != null && executionData.currentTime < executionData.totalTime )
			{
				flow.InternalVariables[ this ] = executionData;

				//reply after sleeping
				flow.ExecutionStack.Push( new Flow.ExecutionStackItem( this ) );

				//add to the sleeping list
				if( flow.Owner != null )
					flow.Owner.AddSleepingFlow( flow );
				else
					Flow.AddGlobalSleepingFlow( flow );

				//to exit from execution stack loop
				sleep = true;
			}
			else
			{
				flow.InternalVariables.Remove( this );

				//go to Exit
				FlowInput next = Exit;
				if( next != null )
					flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
			}
		}
	}
}
