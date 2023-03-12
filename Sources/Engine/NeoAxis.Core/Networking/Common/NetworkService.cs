// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Networking
{
	public abstract class NetworkService
	{
		internal const int maxMessageTypeIdentifier = 255;// ( 1 << NetworkDefines.bitsForServiceMessageTypeIdentifier ) - 1;

		//

		internal NetworkNode baseOwner;

		//general
		string name;
		byte identifier;

		//message types
		Dictionary<string, MessageType> messageTypesByName = new Dictionary<string, MessageType>();
		List<MessageType> messageTypesByID = new List<MessageType>();

		///////////////////////////////////////////

		protected sealed class MessageType
		{
			string name;
			byte identifier;
			ReceiveHandlerDelegate receiveHandler;

			///////////////

			public delegate bool ReceiveHandlerDelegate( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage );

			///////////////

			internal MessageType( string name, byte identifier, ReceiveHandlerDelegate receiveHandler )
			{
				this.name = name;
				this.identifier = identifier;
				this.receiveHandler = receiveHandler;
			}

			public string Name
			{
				get { return name; }
			}

			public byte Identifier
			{
				get { return identifier; }
			}

			public ReceiveHandlerDelegate ReceiveHandler
			{
				get { return receiveHandler; }
			}
		}

		///////////////////////////////////////////

		protected NetworkService( string name, byte identifier )
		{
			this.name = name;
			this.identifier = identifier;
		}

		public string Name
		{
			get { return name; }
		}

		public byte Identifier
		{
			get { return identifier; }
		}

		protected virtual void OnDispose()
		{
			//baseOwner = null;
		}

		internal void PerformDispose()
		{
			OnDispose();
		}

		protected MessageType RegisterMessageType( string name, byte identifier, MessageType.ReceiveHandlerDelegate receiveHandler )
		{
			if( identifier == 0 )
				Log.Fatal( "NetworkService: RegisterMessageType: Invalid message type identifier. Identifier can not be zero." );

			if( identifier > maxMessageTypeIdentifier )
				Log.Fatal( "NetworkService: RegisterMessageType: Invalid message type identifier. Max identifier is \"{0}\".", maxMessageTypeIdentifier );

			if( GetMessageType( name ) != null )
				Log.Fatal( "NetworkService: RegisterMessageType: Message type \"{0}\" is already registered.", name );

			if( GetMessageType( identifier ) != null )
				Log.Fatal( "NetworkService: RegisterMessageType: Message type with identifier \"{0}\" is already registered.", identifier );

			var messageType = new MessageType( name, identifier, receiveHandler );
			messageTypesByName.Add( name, messageType );

			while( messageTypesByID.Count <= identifier )
				messageTypesByID.Add( null );
			messageTypesByID[ identifier ] = messageType;

			return messageType;
		}

		protected MessageType RegisterMessageType( string name, byte identifier )
		{
			return RegisterMessageType( name, identifier, null );
		}

		protected MessageType GetMessageType( string name )
		{
			MessageType messageType;
			if( !messageTypesByName.TryGetValue( name, out messageType ) )
				return null;
			return messageType;
		}

		protected MessageType GetMessageType( byte identifier )
		{
			if( identifier >= messageTypesByID.Count )
				return null;
			return messageTypesByID[ identifier ];
		}

		internal void ProcessReceivedMessage( NetworkNode.ConnectedNode connectedNode, ArrayDataReader reader )
		{
			//receive message identifier
			byte messageIdentifier = reader.ReadByte();// NetworkDefines.bitsForServiceMessageTypeIdentifier );

			MessageType messageType = GetMessageType( messageIdentifier );
			if( messageType == null )
			{
				//no such message type
				return;
			}

			if( messageType.ReceiveHandler == null )
			{
				//no receive handler
				return;
			}

			string additionalErrorMessage = null;
			if( !messageType.ReceiveHandler( connectedNode, messageType, reader, ref additionalErrorMessage ) )
			{
				string text = string.Format( "Invalid service message \'{0}\'.", messageType.Name );
				if( !string.IsNullOrEmpty( additionalErrorMessage ) )
					text += " " + additionalErrorMessage;
				baseOwner.OnReceiveProtocolErrorInternal( connectedNode, text );
				return;
			}
		}

		protected internal virtual void OnUpdate() { }
	}
}
