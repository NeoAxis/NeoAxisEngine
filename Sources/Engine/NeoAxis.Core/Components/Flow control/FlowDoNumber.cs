// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Component representing a loop running specified amount of time.
	/// </summary>
	public class FlowDoNumber : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
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
		/// The number of times to run.
		/// </summary>
		[DefaultValue( 1 )]
		public Reference<int> Number
		{
			get { if( _number.BeginGet() ) Number = _number.Get( this ); return _number.value; }
			set { if( _number.BeginSet( ref value ) ) { try { NumberChanged?.Invoke( this ); } finally { _number.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Number"/> property value changes.</summary>
		public event Action<FlowDoNumber> NumberChanged;
		ReferenceField<int> _number = 1;

		//LoopBody
		ReferenceField<FlowInput> _loopBody;
		/// <summary>
		/// The body of the while loop.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<FlowInput> LoopBody
		{
			get
			{
				if( _loopBody.BeginGet() )
					LoopBody = _loopBody.Get( this );
				return _loopBody.value;
			}
			set
			{
				if( _loopBody.BeginSet( ref value ) )
				{
					try { LoopBodyChanged?.Invoke( this ); }
					finally { _loopBody.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LoopBody"/> property value changes.</summary>
		public event Action<FlowDoNumber> LoopBodyChanged;

		/// <summary>
		/// Current iteration number.
		/// </summary>
		public int Counter
		{
			get
			{
				//get value from current flow
				var currentFlow = Flow.CurrentFlow;
				if( currentFlow != null && currentFlow.InternalVariables.TryGetValue( this, out var value ) )
					return (int)value;
				return -1;
			}
		}

		//Exit
		ReferenceField<FlowInput> _exit;
		/// <summary>
		/// The output of the node.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<FlowInput> Exit
		{
			get
			{
				if( _exit.BeginGet() )
					Exit = _exit.Get( this );
				return _exit.value;
			}
			set
			{
				if( _exit.BeginSet( ref value ) )
				{
					try { ExitChanged?.Invoke( this ); }
					finally { _exit.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Exit"/> property value changes.</summary>
		public event Action<FlowDoNumber> ExitChanged;

		/////////////////////////////////////////

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			bool isEntry = entryItem.FlowInput != null && entryItem.FlowInput.PropertyName == nameof( Entry );

			int number = -1;
			if( !isEntry )
			{
				if( flow.InternalVariables.TryGetValue( this, out var value ) )
					number = (int)value;
			}

			number++;

			if( number < Number.Value )
			{
				flow.InternalVariables[ this ] = number;

				//reply after LoopBody
				flow.ExecutionStack.Push( new Flow.ExecutionStackItem( this ) );

				//go to LoopBody
				FlowInput next = LoopBody;
				if( next != null )
					flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
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
