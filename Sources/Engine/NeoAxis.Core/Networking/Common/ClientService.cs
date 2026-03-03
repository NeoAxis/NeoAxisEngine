// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace NeoAxis.Networking
{
	public abstract class ClientService
	{
		internal const int maxMessageTypeIdentifier = 255;

		//general
		internal ClientNode owner;
		string name;
		int identifier;

		//message types
		Dictionary<string, MessageType> messageTypesByName = new Dictionary<string, MessageType>();
		List<MessageType> messageTypesByID = new List<MessageType>();

		//optimization
		BeginMessageContext oneBeginMessage = new BeginMessageContext();

		///////////////////////////////////////////////

		public sealed class MessageType
		{
			string name;
			int identifier;
			ReceiveHandlerDelegate receiveHandler;

			/////////////////////

			public delegate bool ReceiveHandlerDelegate( MessageType messageType, ArrayDataReader reader, ref string error );

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

		public class BeginMessageContext
		{
			public ClientService Owner;
			public int MessageID;
			public ArrayDataWriter Writer = new ArrayDataWriter( 128 );
			public string MessageForProfiler;

			/////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			public void End()
			{
				var clientNode = Owner.Owner;
				if( clientNode != null && clientNode.Status == NetworkStatus.Connected )
				{
					var writer = Writer;

					//send accumulated message
					int bytesWritten = clientNode.AddAccumulatedMessageToSend( writer );

					//profiler
					var profilerDataCached = clientNode.ProfilerData;
					if( profilerDataCached != null )
					{
						var serviceItem = profilerDataCached.GetServiceItem( Owner.Identifier );
						var messageTypeItem = serviceItem.GetMessageTypeItem( MessageID );

						Interlocked.Increment( ref messageTypeItem.SentMessages );
						Interlocked.Add( ref messageTypeItem.SentSize, writer.Length );

						Interlocked.Increment( ref profilerDataCached.TotalSentMessages );
						Interlocked.Add( ref profilerDataCached.TotalSentSize, bytesWritten + writer.Length );

						//custom message
						if( !string.IsNullOrEmpty( MessageForProfiler ) )
						{
							//!!!!threading

							try
							{
								if( messageTypeItem.SentCustomData == null )
									messageTypeItem.SentCustomData = new Dictionary<string, ClientNode.ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData>();

								messageTypeItem.SentCustomData.TryGetValue( MessageForProfiler, out var item );
								item.Messages++;
								item.Size += writer.Length;// SendingDataWriterLength;
								messageTypeItem.SentCustomData[ MessageForProfiler ] = item;
							}
							catch { }
						}
					}
				}

				var current = Owner.oneBeginMessage;
				if( current == null || Writer.Data.Length > current.Writer.Data.Length )
					Interlocked.CompareExchange( ref Owner.oneBeginMessage, this, current );
			}
		}

		///////////////////////////////////////////////

		protected ClientService( string name, int identifier )
		{
			if( identifier < 0 )
				Log.Fatal( "ClientService: Constructor: identifier < 0." );

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

		protected internal virtual void OnUpdate( DateTime utcNow ) { }

		[MethodImpl( (MethodImplOptions)512 )]
		protected BeginMessageContext BeginMessage( int messageID )
		{
			if( messageID <= 0 || messageID > maxMessageTypeIdentifier )
				Log.Fatal( "ClientService: BeginMessage: messageID <= 0 || messageID > 255." );

			var m = Interlocked.Exchange( ref oneBeginMessage, null );
			if( m == null )
				m = new BeginMessageContext();

			m.Owner = this;
			m.MessageID = messageID;
			m.Writer.Reset();
			m.MessageForProfiler = null;
			m.Writer.Write( (byte)Identifier );
			m.Writer.Write( (byte)messageID );

			return m;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected BeginMessageContext BeginMessage( MessageType messageType )
		{
			return BeginMessage( messageType.Identifier );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendMessage( MessageType messageType, ArraySegment<byte> data )
		{
			var m = BeginMessage( messageType );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendMessage( int messageID, ArraySegment<byte> data )
		{
			var m = BeginMessage( messageID );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}
	}
}
