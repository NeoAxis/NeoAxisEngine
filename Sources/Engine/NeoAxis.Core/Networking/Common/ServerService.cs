// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace NeoAxis.Networking
{
	public abstract class ServerService
	{
		internal const int maxMessageTypeIdentifier = 255;

		//general
		internal ServerNode owner;
		string name;
		int identifier;

		//message types
		Dictionary<string, MessageType> messageTypesByName = new Dictionary<string, MessageType>();
		List<MessageType> messageTypesByID = new List<MessageType>();

		//optimization
		BeginMessageContext oneBeginMessage = new BeginMessageContext();

		///////////////////////////////////////////

		public sealed class MessageType
		{
			string name;
			int identifier;
			ReceiveHandlerDelegate receiveHandler;

			///////////////

			public delegate bool ReceiveHandlerDelegate( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error );

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
			if( identifier < 0 )
				Log.Fatal( "ServerService: Constructor: identifier < 0." );

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
			var profiledData2 = owner.ProfilerData;
			if( profiledData2 != null )
			{
				var serviceItem = profiledData2.GetServiceItem( Identifier );
				var messageTypeItem = serviceItem.GetMessageTypeItem( messageType.Identifier );
				messageTypeItem.ReceivedMessages++;
				messageTypeItem.ReceivedSize += lengthForProfiler;

				profiledData2.TotalReceivedMessages++;
				profiledData2.TotalReceivedSize += lengthForProfiler;
			}

			if( messageType.ReceiveHandler == null )
			{
				//no receive handler
				return;
			}

			try
			{
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
			catch( Exception e )
			{
				var text = string.Format( "Exception in service message \"{0}\". {1}", messageType.Name, e.ToString() );
				//var text = string.Format( "Exception in service message \"{0}\". {1}", messageType.Name, e.Message );
				owner.OnReceiveProtocolErrorInternal( client, text );
				return;
			}
		}

		protected internal virtual void OnUpdate() { }

		///////////////////////////////////////////////

		public class BeginMessageContext
		{
			public ServerService Owner;
			public int MessageID;
			public List<ServerNode.Client> Recipients = new List<ServerNode.Client>();
			public ArrayDataWriter Writer = new ArrayDataWriter( 128 );

			/////////////////////

			[MethodImpl( (MethodImplOptions)512 )]
			public void End()
			{
				var serverWebSocket = Owner.Owner?.webSocketListener;
				var serverUdp = Owner.Owner?.udpListener;
				if( serverWebSocket != null || serverUdp != null )
				{
					for( int n = 0; n < Recipients.Count; n++ )
					{
						var client = Recipients[ n ];

						if( client != null && client.Status != NetworkStatus.Disconnected ) //can send when Connecting status, to send before Connected status
						{
							var writer = Writer;

							//send accumulated message
							int bytesWritten = client.AddAccumulatedMessageToSend( writer );

							//profiler
							var profilerData2 = Owner.Owner.ProfilerData;
							if( profilerData2 != null )
							{
								var serviceItem = profilerData2.GetServiceItem( Owner.Identifier );
								var messageTypeItem = serviceItem.GetMessageTypeItem( MessageID );

								Interlocked.Increment( ref messageTypeItem.SentMessages );
								Interlocked.Add( ref messageTypeItem.SentSize, writer.Length );

								Interlocked.Increment( ref profilerData2.TotalSentMessages );
								Interlocked.Add( ref profilerData2.TotalSentSize, bytesWritten + writer.Length );
							}
						}
					}
				}

				var current = Owner.oneBeginMessage;
				if( current == null || Writer.Data.Length > current.Writer.Data.Length )
					Interlocked.CompareExchange( ref Owner.oneBeginMessage, this, current );
			}
		}

		///////////////////////////////////////////////

		[MethodImpl( (MethodImplOptions)512 )]
		public BeginMessageContext BeginMessage( int messageID )
		{
			if( messageID <= 0 || messageID > maxMessageTypeIdentifier )
				Log.Fatal( "ServerService: BeginMessage: messageID <= 0 || messageID > 255." );

			var m = Interlocked.Exchange( ref oneBeginMessage, null );
			if( m == null )
				m = new BeginMessageContext();

			m.Owner = this;
			m.MessageID = messageID;
			m.Recipients.Clear();
			m.Writer.Reset();
			m.Writer.Write( (byte)Identifier );
			m.Writer.Write( (byte)messageID );

			return m;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BeginMessageContext BeginMessage( MessageType messageType )
		{
			return BeginMessage( messageType.Identifier );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public BeginMessageContext BeginMessage( IList<ServerNode.Client> recipients, int messageID )
		{
			if( recipients == null )
				Log.Fatal( "ServerService: BeginMessage: recipients = null." );

			var m = BeginMessage( messageID );
			for( int n = 0; n < recipients.Count; n++ )
				m.Recipients.Add( recipients[ n ] );
			return m;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BeginMessageContext BeginMessage( IList<ServerNode.Client> recipients, MessageType messageType )
		{
			return BeginMessage( recipients, messageType.Identifier );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public BeginMessageContext BeginMessage( ServerNode.Client recipient, int messageID )
		{
			if( recipient == null )
				Log.Fatal( "ServerService: BeginMessage: recipient = null." );

			var m = BeginMessage( messageID );
			m.Recipients.Add( recipient );
			return m;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected BeginMessageContext BeginMessage( ServerNode.Client recipient, MessageType messageType )
		{
			return BeginMessage( recipient, messageType.Identifier );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BeginMessageContext BeginMessageToAll( int messageID )
		{
			//broadcast?
			//where else. broadcast scene data

			return BeginMessage( owner.GetClientsArray(), messageID );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public BeginMessageContext BeginMessageToAll( MessageType messageType )
		{
			return BeginMessage( owner.GetClientsArray(), messageType );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendMessage( IList<ServerNode.Client> recipients, MessageType messageType, ArraySegment<byte> data )
		{
			var m = BeginMessage( recipients, messageType );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendMessage( IList<ServerNode.Client> recipients, int messageID, ArraySegment<byte> data )
		{
			var m = BeginMessage( recipients, messageID );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendMessage( ServerNode.Client recipient, MessageType messageType, ArraySegment<byte> data )
		{
			var m = BeginMessage( recipient, messageType );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void SendMessage( ServerNode.Client recipient, int messageID, ArraySegment<byte> data )
		{
			var m = BeginMessage( recipient, messageID );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}
	}
}