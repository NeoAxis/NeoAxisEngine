// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// The component for starting a new flow.
	/// </summary>
	public class Component_FlowStart : Component, IFlowGraphRepresentationData, IFlowExecutionComponent
	{
		//!!!!time period/interval
		//!!!!выполнять при EnabledInHierarchy
		//!!!!или еще как-то
		//bool Simulation
		//bool Editor

		/// <summary>
		/// The input of the node.
		/// </summary>
		public FlowInput Entry
		{
			get { return new FlowInput( this, nameof( Entry ) ); }
		}

		/// <summary>
		/// The exit of the node.
		/// </summary>
		[Serialize]
		public Reference<FlowInput> Exit
		{
			get { if( _exit.BeginGet() ) Exit = _exit.Get( this ); return _exit.value; }
			set { if( _exit.BeginSet( ref value ) ) { try { ExitChanged?.Invoke( this ); } finally { _exit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Exit"/> property value changes.</summary>
		public event Action<Component_FlowStart> ExitChanged;
		ReferenceField<FlowInput> _exit;

		/// <summary>
		/// A branch of the new flow.
		/// </summary>
		[Serialize]
		public Reference<FlowInput> NewFlow
		{
			get { if( _newFlow.BeginGet() ) NewFlow = _newFlow.Get( this ); return _newFlow.value; }
			set { if( _newFlow.BeginSet( ref value ) ) { try { NewFlowChanged?.Invoke( this ); } finally { _newFlow.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="NewFlow"/> property value changes.</summary>
		public event Action<Component_FlowStart> NewFlowChanged;
		ReferenceField<FlowInput> _newFlow;

		/////////////////////////////////////////

		public void GetFlowGraphRepresentationData( FlowGraphRepresentationData data )
		{
			data.NodeContentType = FlowGraphNodeContentType.FlowStart;
		}

		public void Start()
		{
			var flow = NewFlow.Value;
			if( flow != null )
				Flow.Execute( flow );
		}

		void IFlowExecutionComponent.FlowExecution( Flow flow, Flow.ExecutionStackItem entryItem )
		{
			Start();

			FlowInput next = Exit;
			if( next != null )
				flow.ExecutionStack.Push( new Flow.ExecutionStackItem( next ) );
		}
	}
}
