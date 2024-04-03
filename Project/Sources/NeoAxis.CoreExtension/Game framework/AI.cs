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
	public class AI : Component, InteractiveObjectInterface
	{
		/// <summary>
		/// The text of the permanent message that is displayed above the controlled object.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> PermanentMessage
		{
			get { if( _permanentMessage.BeginGet() ) PermanentMessage = _permanentMessage.Get( this ); return _permanentMessage.value; }
			set { if( _permanentMessage.BeginSet( this, ref value ) ) { try { PermanentMessageChanged?.Invoke( this ); } finally { _permanentMessage.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PermanentMessage"/> property value changes.</summary>
		public event Action<AI> PermanentMessageChanged;
		ReferenceField<string> _permanentMessage = "";

		///////////////////////////////////////////////

		[Browsable( false )]
		public AITask CurrentTask
		{
			get { return GetComponent<AITask>( onlyEnabledInHierarchy: true ); }
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//!!!!maybe call rarely

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

		///////////////////////////////////////////////

		public delegate void ObjectInteractionGetInfoEventDelegate( AI sender, GameMode gameMode, ref InteractiveObjectObjectInfo info );
		public event ObjectInteractionGetInfoEventDelegate ObjectInteractionGetInfoEvent;

		public delegate void ObjectInteractionInputMessageEventDelegate( AI sender, GameMode gameMode, InputMessage message, ref bool handled );
		public event ObjectInteractionInputMessageEventDelegate ObjectInteractionInputMessageEvent;

		public delegate void ObjectInteractionEnterEventDelegate( AI sender, ObjectInteractionContext context );
		public event ObjectInteractionEnterEventDelegate ObjectInteractionEnterEvent;

		public delegate void ObjectInteractionExitEventDelegate( AI sender, ObjectInteractionContext context );
		public event ObjectInteractionExitEventDelegate ObjectInteractionExitEvent;

		public delegate void ObjectInteractionUpdateEventDelegate( AI sender, ObjectInteractionContext context );
		public event ObjectInteractionUpdateEventDelegate ObjectInteractionUpdateEvent;

		///////////////////////////////////////////////

		public virtual void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			ObjectInteractionGetInfoEvent?.Invoke( this, gameMode, ref info );
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var handled = false;
			ObjectInteractionInputMessageEvent?.Invoke( this, gameMode, message, ref handled );
			return handled;
		}

		public virtual void ObjectInteractionEnter( ObjectInteractionContext context )
		{
			ObjectInteractionEnterEvent?.Invoke( this, context );
		}

		public virtual void ObjectInteractionExit( ObjectInteractionContext context )
		{
			ObjectInteractionExitEvent?.Invoke( this, context );
		}

		public virtual void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
			ObjectInteractionUpdateEvent?.Invoke( this, context );
		}
	}
}