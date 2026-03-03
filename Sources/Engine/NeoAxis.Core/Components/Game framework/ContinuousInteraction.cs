// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents an interaction between two participants.
	/// </summary>
	public class ContinuousInteraction : Component
	{
		/// <summary>
		/// Easy access to current interaction from scripts.
		/// </summary>
		public static ContinuousInteraction Latest { get; set; }

		//

		/// <summary>
		/// Who creates the interaction.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component> Creator
		{
			get { if( _creator.BeginGet() ) Creator = _creator.Get( this ); return _creator.value; }
			set { if( _creator.BeginSet( this, ref value ) ) { try { CreatorChanged?.Invoke( this ); } finally { _creator.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Creator"/> property value changes.</summary>
		public event Action<ContinuousInteraction> CreatorChanged;
		ReferenceField<Component> _creator = null;

		//!!!!list?
		/// <summary>
		/// The second participant of the interaction.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component> SecondParticipant
		{
			get { if( _secondParticipant.BeginGet() ) SecondParticipant = _secondParticipant.Get( this ); return _secondParticipant.value; }
			set { if( _secondParticipant.BeginSet( this, ref value ) ) { try { SecondParticipantChanged?.Invoke( this ); } finally { _secondParticipant.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SecondParticipant"/> property value changes.</summary>
		public event Action<ContinuousInteraction> SecondParticipantChanged;
		ReferenceField<Component> _secondParticipant = null;

		public enum InteractionTypeEnum
		{
			Dialogue,
		}

		/// <summary>
		/// The type of the interaction.
		/// </summary>
		[DefaultValue( InteractionTypeEnum.Dialogue )]
		public Reference<InteractionTypeEnum> InteractionType
		{
			get { if( _interactionType.BeginGet() ) InteractionType = _interactionType.Get( this ); return _interactionType.value; }
			set { if( _interactionType.BeginSet( this, ref value ) ) { try { InteractionTypeChanged?.Invoke( this ); } finally { _interactionType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InteractionType"/> property value changes.</summary>
		public event Action<ContinuousInteraction> InteractionTypeChanged;
		ReferenceField<InteractionTypeEnum> _interactionType = InteractionTypeEnum.Dialogue;

		/// <summary>
		/// Last current message from the creator.
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> CurrentMessageFromCreator
		{
			get { if( _currentMessageFromCreator.BeginGet() ) CurrentMessageFromCreator = _currentMessageFromCreator.Get( this ); return _currentMessageFromCreator.value; }
			set { if( _currentMessageFromCreator.BeginSet( this, ref value ) ) { try { CurrentMessageFromCreatorChanged?.Invoke( this ); } finally { _currentMessageFromCreator.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurrentMessageFromCreator"/> property value changes.</summary>
		public event Action<ContinuousInteraction> CurrentMessageFromCreatorChanged;
		ReferenceField<string> _currentMessageFromCreator = "";

		public string CurrentMessageFromParticipant { get; set; } = "";
		public Flow DeleteWhenFlowEnded;

		///////////////////////////////////////////////

		public delegate void MessageFromParticipantEventDelegate( ContinuousInteraction sender/*, Component participant*/, string message );
		public event MessageFromParticipantEventDelegate MessageFromParticipantEvent;

		public void MessageFromParticipant( /*Component participant,*/ string message )
		{
			if( NetworkIsClient )
			{
				var m = BeginNetworkMessageToServer( "MessageFromParticipant" );
				if( m != null )
				{
					m.Writer.Write( message );
					m.End();
				}
			}
			else
			{
				Latest = this;
				CurrentMessageFromParticipant = message;
				MessageFromParticipantEvent?.Invoke( this, /*participant,*/ message );
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( DeleteWhenFlowEnded != null && DeleteWhenFlowEnded.Ended )
				RemoveFromParent( true );

			var creator = Creator.Value;
			if( creator == null || !creator.EnabledInHierarchy )
				RemoveFromParent( true );

			var secondParticipant = SecondParticipant.Value;
			if( secondParticipant == null || !secondParticipant.EnabledInHierarchy )
				RemoveFromParent( true );
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			if( message == "MessageFromParticipant" )
			{
				var messageText = reader.ReadString() ?? string.Empty;
				if( !reader.Complete() )
					return false;

				Latest = this;
				CurrentMessageFromParticipant = messageText;
				MessageFromParticipantEvent?.Invoke( this, /*participant,*/ messageText );
			}

			return true;
		}
	}
}