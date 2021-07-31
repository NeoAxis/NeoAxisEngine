// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// The component provides the enumeration of collection elements with specifying the interval of elements.
	/// </summary>
	public class Component_For : Component, IFlowExecutionComponent, IFlowGraphRepresentationData
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
		public Reference<IList> Collection
		{
			get { if( _collection.BeginGet() ) Collection = _collection.Get( this ); return _collection.value; }
			set { if( _collection.BeginSet( ref value ) ) { try { CollectionChanged?.Invoke( this ); } finally { _collection.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Collection"/> property value changes.</summary>
		public event Action<Component_For> CollectionChanged;
		ReferenceField<IList> _collection = null;

		/// <summary>
		/// The index of the collection element to start iteration from.
		/// </summary>
		[DefaultValue( 0 )]
		public Reference<int> FirstIndex
		{
			get { if( _firstIndex.BeginGet() ) FirstIndex = _firstIndex.Get( this ); return _firstIndex.value; }
			set { if( _firstIndex.BeginSet( ref value ) ) { try { FirstIndexChanged?.Invoke( this ); } finally { _firstIndex.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FirstIndex"/> property value changes.</summary>
		public event Action<Component_For> FirstIndexChanged;
		ReferenceField<int> _firstIndex = 0;

		/// <summary>
		/// The index of the collection element to end iteration with.
		/// </summary>
		[DefaultValue( 0 )]
		public Reference<int> LastIndex
		{
			get { if( _lastIndex.BeginGet() ) LastIndex = _lastIndex.Get( this ); return _lastIndex.value; }
			set { if( _lastIndex.BeginSet( ref value ) ) { try { LastIndexChanged?.Invoke( this ); } finally { _lastIndex.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LastIndex"/> property value changes.</summary>
		public event Action<Component_For> LastIndexChanged;
		ReferenceField<int> _lastIndex = 0;

		/// <summary>
		/// The action to perform for each collection element from the specified indices range.
		/// </summary>
		[DefaultValue( null )]
		public Reference<FlowInput> LoopBody
		{
			get { if( _loopBody.BeginGet() ) LoopBody = _loopBody.Get( this ); return _loopBody.value; }
			set { if( _loopBody.BeginSet( ref value ) ) { try { LoopBodyChanged?.Invoke( this ); } finally { _loopBody.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LoopBody"/> property value changes.</summary>
		public event Action<Component_For> LoopBodyChanged;
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
					return ( (ExecutionData)value ).current;
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
		public event Action<Component_For> ExitChanged;
		ReferenceField<FlowInput> _exit = null;

		/////////////////////////////////////////

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.Flow;
		}

		class ExecutionData
		{
			public IList collection;
			public int currentIndex;
			public object current;
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem, ref bool sleep )
		{
			bool isEntry = entryItem.FlowInput != null && entryItem.FlowInput.PropertyName == nameof( Entry );

			ExecutionData executionData = null;
			if( isEntry )
			{
				var collection = Collection.Value;
				if( collection != null )
				{
					executionData = new ExecutionData();
					executionData.collection = collection;
					executionData.currentIndex = FirstIndex;
				}
			}
			else
			{
				if( flow.InternalVariables.TryGetValue( this, out var value ) )
				{
					executionData = (ExecutionData)value;
					executionData.currentIndex++;
				}
			}

			//update Current
			if( executionData != null )
			{
				if( executionData.currentIndex >= 0 && executionData.currentIndex <= LastIndex.Value && executionData.currentIndex < executionData.collection.Count )
					executionData.current = executionData.collection[ executionData.currentIndex ];
				else
					executionData.current = null;
			}

			if( executionData != null && executionData.currentIndex >= 0 && executionData.currentIndex <= LastIndex.Value && executionData.currentIndex < executionData.collection.Count )
			{
				flow.InternalVariables[ this ] = executionData;

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
