// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis.Networking
{
	public abstract class ClientService
	{
		internal const int maxMessageTypeIdentifier = 15;

		//general
		internal ClientNode owner;
		string name;
		int identifier;

		//message types
		Dictionary<string, MessageType> messageTypesByName = new Dictionary<string, MessageType>();
		List<MessageType> messageTypesByID = new List<MessageType>();

		//send data
		bool sendingData;
		int sendingMessageID;
		ArrayDataWriter sendingDataWriter = new ArrayDataWriter();

		///////////////////////////////////////////////

		public sealed class MessageType
		{
			string name;
			int identifier;
			ReceiveHandlerDelegate receiveHandler;

			/////////////////////

			public delegate bool ReceiveHandlerDelegate( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage );

			/////////////////////

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

		///////////////////////////////////////////////

		protected ClientService( string name, int identifier )
		{
			this.name = name;
			this.identifier = identifier;
		}

		public ClientNode Owner
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

		public int SendingDataWriterLength
		{
			get { return sendingDataWriter.Length; }
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
		internal void ProcessReceivedMessage( ArrayDataReader reader, int lengthForProfiler, int messageIdentifier )
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
			if( !messageType.ReceiveHandler( messageType, reader, ref additionalErrorMessage ) )
			{
				var text = string.Format( "Invalid service message \"{0}\".", messageType.Name );
				if( !string.IsNullOrEmpty( additionalErrorMessage ) )
					text += " " + additionalErrorMessage;
				owner.OnReceiveProtocolErrorInternal( text );
				return;
			}
		}

		protected internal virtual void OnUpdate() { }

		[MethodImpl( (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( int messageID )
		{
			if( sendingData )
				Log.Fatal( "ClientService: BeginMessage: The message is already begun." );
			if( messageID <= 0 || messageID > 15 )
				Log.Fatal( "ClientService: BeginMessage: messageID <= 0 || messageID > 15." );

			sendingData = true;
			sendingMessageID = messageID;
			sendingDataWriter.Reset();

			sendingDataWriter.Write( (byte)( ( Identifier << 4 ) + messageID ) );

			return sendingDataWriter;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( MessageType messageType )
		{
			return BeginMessage( messageType.Identifier );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected void EndMessage()
		{
			if( !sendingData )
				Log.Fatal( "ClientService: EndMessage: The message is not begun." );

			if( owner.Status == NetworkStatus.Connected )
			{
				var writer = sendingDataWriter;

				var bytesWritten = owner.accumulatedMessagesToSend.WriteVariableUInt32( (uint)writer.Length );
				owner.accumulatedMessagesToSend.Write( writer.Data, 0, writer.Length );

				var profilerDataCached = owner.ProfilerData;
				if( profilerDataCached != null )
				{
					var serviceItem = profilerDataCached.GetServiceItem( Identifier );
					var messageTypeItem = serviceItem.GetMessageTypeItem( sendingMessageID );
					messageTypeItem.SentMessages++;
					messageTypeItem.SentSize += sendingDataWriter.Length;

					profilerDataCached.TotalSentMessages++;
					profilerDataCached.TotalSentSize += bytesWritten + writer.Length;
				}
			}

			sendingData = false;
			sendingMessageID = -1;
			sendingDataWriter.Reset();
		}

		//!!!!
		////how to parse. need collect the data array with BeginMessage separately from usual BeginMessage
		//public void SendMessageWithThreadingSupport( byte[] data )
		//{
		//	if( owner.Status == NetworkStatus.Connected )
		//		owner.toProcessMessages.Enqueue( new ClientNode.ToProcessMessage { DataBinary = true, DataBinaryArray = data } );
		//}
	}
}
