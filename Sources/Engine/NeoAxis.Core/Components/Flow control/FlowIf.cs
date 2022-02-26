// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Component representing the if-statement.
	/// </summary>
	public class FlowIf : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
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
		/// The condition of the statement.
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
		public event Action<FlowIf> ConditionChanged;

		//True
		ReferenceField<FlowInput> _true;
		/// <summary>
		/// The output of the node if condition is true.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<FlowInput> True
		{
			get
			{
				if( _true.BeginGet() )
					True = _true.Get( this );
				return _true.value;
			}
			set
			{
				if( _true.BeginSet( ref value ) )
				{
					try { TrueChanged?.Invoke( this ); }
					finally { _true.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="True"/> property value changes.</summary>
		public event Action<FlowIf> TrueChanged;

		//False
		ReferenceField<FlowInput> _false;
		/// <summary>
		/// The output of the node if condition is false.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		public Reference<FlowInput> False
		{
			get
			{
				if( _false.BeginGet() )
					False = _false.Get( this );
				return _false.value;
			}
			set
			{
				if( _false.BeginSet( ref value ) )
				{
					try { FalseChanged?.Invoke( this ); }
					finally { _false.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="False"/> property value changes.</summary>
		public event Action<FlowIf> FalseChanged;

		/////////////////////////////////////////

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			FlowInput next;
			if( Condition )
				next = True;
			else
				next = False;

			if( next != null )
				flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
		}
	}
}
