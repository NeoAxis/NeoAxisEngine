// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// The component provides the enumeration of collection elements.
	/// </summary>
	public class Component_ForEach : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
	{
		/// <summary>
		/// The input of the node.
		/// </summary>
		public FlowInput Entry
		{
			get { return new FlowInput( this, nameof( Entry ) ); }
		}

		/// <summary>
		/// The collection to iterate through.
		/// </summary>
		[DefaultValue( null )]
		public Reference<ICollection> Collection
		{
			get { if( _collection.BeginGet() ) Collection = _collection.Get( this ); return _collection.value; }
			set { if( _collection.BeginSet( ref value ) ) { try { CollectionChanged?.Invoke( this ); } finally { _collection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Collection"/> property value changes.</summary>
		public event Action<Component_ForEach> CollectionChanged;
		ReferenceField<ICollection> _collection = null;

		/// <summary>
		/// The action to perform for each collection element.
		/// </summary>
		[DefaultValue( null )]
		public Reference<FlowInput> LoopBody
		{
			get { if( _loopBody.BeginGet() ) LoopBody = _loopBody.Get( this ); return _loopBody.value; }
			set { if( _loopBody.BeginSet( ref value ) ) { try { LoopBodyChanged?.Invoke( this ); } finally { _loopBody.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LoopBody"/> property value changes.</summary>
		public event Action<Component_ForEach> LoopBodyChanged;
		ReferenceField<FlowInput> _loopBody = null;

		/// <summary>
		/// Current iteration's collection element.
		/// </summary>
		public object Current
		{
			get
			{
				//get value from current flow
				var currentFlow = Flow.CurrentFlow;
				if( currentFlow != null && currentFlow.InternalVariables.TryGetValue( this, out var value ) )
					return ( (IEnumerator)value ).Current;
				return null;
			}
		}

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
		public event Action<Component_ForEach> ExitChanged;
		ReferenceField<FlowInput> _exit = null;

		/////////////////////////////////////////

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem )
		{
			bool isEntry = entryItem.FlowInput != null && entryItem.FlowInput.PropertyName == nameof( Entry );

			IEnumerator enumerator = null;
			if( isEntry )
			{
				var collection = Collection.Value;
				if( collection != null )
					enumerator = collection.GetEnumerator();
			}
			else
			{
				if( flow.InternalVariables.TryGetValue( this, out var value ) )
					enumerator = (IEnumerator)value;
			}

			if( enumerator != null && enumerator.MoveNext() )
			{
				flow.InternalVariables[ this ] = enumerator;

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
