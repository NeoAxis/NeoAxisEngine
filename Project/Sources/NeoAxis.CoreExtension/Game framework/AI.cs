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

		public delegate void InteractionGetInfoEventDelegate( AI sender, GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info );
		public event InteractionGetInfoEventDelegate InteractionGetInfoEvent;

		public delegate void InteractionInputMessageEventDelegate( AI sender, GameMode gameMode, Component initiator, InputMessage message, ref bool handled );
		public event InteractionInputMessageEventDelegate InteractionInputMessageEvent;

		public delegate void InteractionEnterEventDelegate( AI sender, ObjectInteractionContext context );
		public event InteractionEnterEventDelegate InteractionEnterEvent;

		public delegate void InteractionExitEventDelegate( AI sender, ObjectInteractionContext context );
		public event InteractionExitEventDelegate InteractionExitEvent;

		public delegate void InteractionUpdateEventDelegate( AI sender, ObjectInteractionContext context );
		public event InteractionUpdateEventDelegate InteractionUpdateEvent;

		///////////////////////////////////////////////

		public virtual void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			InteractionGetInfoEvent?.Invoke( this, gameMode, initiator, ref info );
		}

		public virtual bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message )
		{
			var handled = false;
			InteractionInputMessageEvent?.Invoke( this, gameMode, initiator, message, ref handled );
			return handled;
		}

		public virtual void InteractionEnter( ObjectInteractionContext context )
		{
			InteractionEnterEvent?.Invoke( this, context );
		}

		public virtual void InteractionExit( ObjectInteractionContext context )
		{
			InteractionExitEvent?.Invoke( this, context );
		}

		public virtual void InteractionUpdate( ObjectInteractionContext context )
		{
			InteractionUpdateEvent?.Invoke( this, context );
		}
	}
}