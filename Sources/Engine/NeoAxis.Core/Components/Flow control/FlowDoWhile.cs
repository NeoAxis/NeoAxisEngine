// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Component representing a do-while loop.
	/// </summary>
	public class FlowDoWhile : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
	{
		//Entry
		/// <summary>
		/// The input of the node.
		/// </summary>
		public FlowInput Entry
		{
			get { return new FlowInput( this, nameof( Entry ) ); }
		}

		//Condition
		ReferenceField<bool> _condition;
		/// <summary>
		/// The condition of the while statement.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> Condition
		{
			get
			{
				if( _condition.BeginGet() )
					Condition = _condition.Get( this );
				return _condition.value;
			}
			set
			{
				if( _condition.BeginSet( ref value ) )
				{
					try { ConditionChanged?.Invoke( this ); }
					finally { _condition.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Condition"/> property value changes.</summary>
		public event Action<FlowDoWhile> ConditionChanged;

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
		public event Action<FlowDoWhile> LoopBodyChanged;

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
		public event Action<FlowDoWhile> ExitChanged;

		/////////////////////////////////////////

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			bool isEntry = entryItem.FlowInput != null && entryItem.FlowInput.PropertyName == nameof( Entry );

			//bool first = true;
			//if( flow.InternalVariables.TryGetValue( this, out var value ) )
			//	first = (bool)value;

			if( isEntry || Condition )//if( first || Condition )
			{
				//flow.InternalVariables[ this ] = false;

				//reply after LoopBody
				flow.ExecutionStack.Push( new Flow.ExecutionStackItem( this ) );

				//go to LoopBody
				FlowInput next = LoopBody;
				if( next != null )
					flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
			}
			else
			{
				//flow.InternalVariables.Remove( this );

				//go to Exit
				FlowInput next = Exit;
				if( next != null )
					flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
			}
		}
	}
}
