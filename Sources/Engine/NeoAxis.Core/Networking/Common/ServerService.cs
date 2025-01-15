// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis.Networking
{
	public abstract class ServerService
	{
		internal const int maxMessageTypeIdentifier = 15;

		//general
		internal ServerNode owner;
		string name;
		int identifier;

		//message types
		Dictionary<string, MessageType> messageTypesByName = new Dictionary<string, MessageType>();
		List<MessageType> messageTypesByID = new List<MessageType>();

		//send data
		bool sendingData;
		int sendingMessageID;
		List<ServerNode.Client> sendingDestinationClients = new List<ServerNode.Client>();
		ArrayDataWriter sendDataWriter = new ArrayDataWriter();

		///////////////////////////////////////////

		public sealed class MessageType
		{
			string name;
			int identifier;
			ReceiveHandlerDelegate receiveHandler;

			///////////////

			public delegate bool ReceiveHandlerDelegate( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage );

			///////////////

			internal MessageType( string name, int identifier, ReceiveHandlerDelegate receiveHandler )
			{
				this.name = name;
				this.identifier = identifier;
				this.receiveHandler = receiveHandler;
			}

			public string Name
			{
				get { return name; }
			}

			public int Identifier
			{
				get { return identifier; }
			}

			public ReceiveHandlerDelegate ReceiveHandler
			{
				get { return receiveHandler; }
			}
		}

		///////////////////////////////////////////

		protected ServerService( string name, int identifier )
		{
			this.name = name;
			this.identifier = identifier;
		}

		public ServerNode Owner
		{
			get { return owner; }
		}

		public string Name
		{
			get { return name; }
		}

		public int Identifier
		{
			get { return identifier; }
		}

		public bool SendingData
		{
			get { return sendingData; }
		}

		protected virtual void OnDispose()
		{
		}

		internal void PerformDispose()
		{
			OnDispose();
		}

		protected MessageType RegisterMessageType( string name, int identifier, MessageType.ReceiveHandlerDelegate receiveHandler )
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

		protected MessageType RegisterMessageType( string name, int identifier )
		{
			return RegisterMessageType( name, identifier, null );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public MessageType GetMessageType( string name )
		{
			MessageType messageType;
			if( !messageTypesByName.TryGetValue( name, out messageType ) )
				return null;
			return messageType;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public MessageType GetMessageType( int identifier )
		{
			if( identifier >= messageTypesByID.Count )
				return null;
			return messageTypesByID[ identifier ];
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ProcessReceivedMessage( ServerNode.Client client, ArrayDataReader reader, int lengthForProfiler, int messageIdentifier )
		{
			var messageType = GetMessageType( messageIdentifier );
			if( messageType == null )
			{
				//no such message type
				return;
			}

			//some data can be received for not registered message types
			var profiledDataCached = owner.ProfilerData;
			if( profiledDataCached != null )
			{
				var serviceItem = profiledDataCached.GetServiceItem( Identifier );
				var messageTypeItem = serviceItem.GetMessageTypeItem( messageType.Identifier );
				messageTypeItem.ReceivedMessages++;
				messageTypeItem.ReceivedSize += lengthForProfiler;
			}

			if( messageType.ReceiveHandler == null )
			{
				//no receive handler
				return;
			}

			string additionalErrorMessage = null;
			if( !messageType.ReceiveHandler( client, messageType, reader, ref additionalErrorMessage ) )
			{
				var text = string.Format( "Invalid service message \"{0}\".", messageType.Name );
				if( !string.IsNullOrEmpty( additionalErrorMessage ) )
					text += " " + additionalErrorMessage;
				owner.OnReceiveProtocolErrorInternal( client, text );
				return;
			}
		}

		protected internal virtual void OnUpdate() { }

		[MethodImpl( (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( IList<ServerNode.Client> recipients, int messageID )
		{
			if( sendingData )
				Log.Fatal( "ServerService: BeginMessage: The message is already begun." );
			if( recipients == null )
				Log.Fatal( "ServerService: BeginMessage: recipients = null." );
			if( messageID <= 0 || messageID > 15 )
				Log.Fatal( "ServerService: BeginMessage: messageID <= 0 || messageID > 15." );

			sendingData = true;
			sendingMessageID = messageID;
			for( int n = 0; n < recipients.Count; n++ )
				sendingDestinationClients.Add( recipients[ n ] );
			sendDataWriter.Reset();

			sendDataWriter.Write( (byte)( ( Identifier << 4 ) + messageID ) );

			return sendDataWriter;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( IList<ServerNode.Client> recipients, MessageType messageType )
		{
			return BeginMessage( recipients, messageType.Identifier );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( ServerNode.Client recipient, int messageID )
		{
			//!!!!maybe no fatals
			if( sendingData )
				Log.Fatal( "ServerService: BeginMessage: The message is already begun." );
			if( recipient == null )
				Log.Fatal( "ServerService: BeginMessage: recipient = null." );
			if( messageID <= 0 || messageID > 15 )
				Log.Fatal( "ServerService: BeginMessage: messageID <= 0 || messageID > 15." );

			sendingData = true;
			sendingMessageID = messageID;
			sendingDestinationClients.Add( recipient );
			sendDataWriter.Reset();

			sendDataWriter.Write( (byte)( ( Identifier << 4 ) + messageID ) );

			return sendDataWriter;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( ServerNode.Client recipient, MessageType messageType )
		{
			return BeginMessage( recipient, messageType.Identifier );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessageToEveryone( int messageID )
		{
			//!!!!broadcast
			//!!!!where else. broadcast scene data

			return BeginMessage( owner.GetClientsArray(), messageID );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessageToEveryone( MessageType messageType )
		{
			return BeginMessage( owner.GetClientsArray(), messageType );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( int messageID )
		{
			if( sendingData )
				Log.Fatal( "ServerService: BeginMessage: The message is already begun." );
			//if( recipients == null )
			//	Log.Fatal( "ServerService: BeginMessage: recipients = null." );
			if( messageID <= 0 || messageID > 15 )
				Log.Fatal( "ServerService: BeginMessage: messageID <= 0 || messageID > 15." );

			sendingData = true;
			sendingMessageID = messageID;
			//for( int n = 0; n < recipients.Count; n++ )
			//	sendingDestinationNodes.Add( recipients[ n ] );
			sendDataWriter.Reset();

			sendDataWriter.Write( (byte)( ( Identifier << 4 ) + messageID ) );

			return sendDataWriter;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( MessageType messageType )
		{
			return BeginMessage( messageType.Identifier );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected void AddMessageRecipient( ServerNode.Client recipient )
		{
			if( !sendingData )
				Log.Fatal( "ServerService: AddMessageRecipient: The message is not begun." );

			sendingDestinationClients.Add( recipient );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected void EndMessage()
		{
			if( !sendingData )
				Log.Fatal( "ServerService: EndMessage: The message is not begun." );

			var server = owner?.server;
			if( server != null )
			{
				for( int n = 0; n < sendingDestinationClients.Count; n++ )
				{
					var client = sendingDestinationClients[ n ];

					if( client.Status == NetworkStatus.Connected )
					{
						var writer = sendDataWriter;

						var bytesWritten = client.accumulatedMessagesToSend.WriteVariableUInt32( (uint)writer.Length );
						client.accumulatedMessagesToSend.Write( writer.Data, 0, writer.Length );

						var profilerDataCached = owner.ProfilerData;
						if( profilerDataCached != null )
						{
							var serviceItem = profilerDataCached.GetServiceItem( Identifier );
							var messageTypeItem = serviceItem.GetMessageTypeItem( sendingMessageID );
							messageTypeItem.SentMessages++;
							messageTypeItem.SentSize += sendDataWriter.Length;

							profilerDataCached.TotalSentMessages++;
							profilerDataCached.TotalSentSize += bytesWritten + writer.Length;
						}


						//var writer2 = new ArrayDataWriter( 4 + writer.Length );
						//var bytesWritten = writer2.WriteVariableUInt32( (uint)writer.Length );
						//writer2.Write( writer.Data, 0, writer.Length );

						//var segment = new ArraySegment<byte>( writer2.Data, 0, writer2.Length );
						//client.toProcessMessages.Enqueue( new ServerNode.Client.ToProcessMessage { DataBinary = true, DataBinarySegment = segment } );

						//if( owner.ProfilerData != null )
						//{
						//	var serviceItem = owner.ProfilerData.GetServiceItem( Identifier );
						//	var messageTypeItem = serviceItem.GetMessageTypeItem( sendingMessageID );
						//	messageTypeItem.SentMessages++;
						//	messageTypeItem.SentSize += sendDataWriter.Length;

						//	//owner.ProfilerData.TotalSentMessages++;
						//	owner.ProfilerData.TotalSentSize += bytesWritten + writer.Length;
						//}
					}
				}
			}

			sendingData = false;
			sendingMessageID = -1;
			sendingDestinationClients.Clear();
			sendDataWriter.Reset();
		}

		//!!!!
		////how to parse. need collect the data array with BeginMessage separately from usual BeginMessage
		//public void SendMessageWithThreadingSupport( recepients, int messageID, ArrayDataWriter writer or ArraySegment[] )
		//{
		//}

	}
}
