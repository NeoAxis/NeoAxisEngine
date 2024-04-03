// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoAxis.Networking
{
	public abstract class ServerNetworkService : NetworkService
	{
		internal NetworkServerNode owner;

		//send data
		bool sendingData;
		int sendingMessageID;
		List<NetworkNode.ConnectedNode> sendingDestinationNodes = new List<NetworkNode.ConnectedNode>();
		ArrayDataWriter sendDataWriter = new ArrayDataWriter();

		///////////////////////////////////////////

		protected ServerNetworkService( string name, int identifier )
			: base( name, identifier )
		{
		}

		public NetworkServerNode Owner
		{
			get { return owner; }
		}

		protected override void OnDispose()
		{
			//owner = null;
			base.OnDispose();
		}

		public bool SendingData
		{
			get { return sendingData; }
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( IList<NetworkNode.ConnectedNode> recipients, int messageID )
		{
			if( sendingData )
				Log.Fatal( "ServerNetworkService: BeginMessage: The message is already begun." );
			if( recipients == null )
				Log.Fatal( "ServerNetworkService: BeginMessage: recipients = null." );
			if( messageID <= 0 || messageID > 15 )
				Log.Fatal( "ServerNetworkService: BeginMessage: messageID <= 0 || messageID > 15." );

			sendingData = true;
			sendingMessageID = messageID;
			for( int n = 0; n < recipients.Count; n++ )
				sendingDestinationNodes.Add( recipients[ n ] );
			sendDataWriter.Reset();

			sendDataWriter.Write( (byte)( ( Identifier << 4 ) + messageID ) );
			//sendDataWriter.Write( Identifier );//, NetworkDefines.bitsForServiceIdentifier );
			//sendDataWriter.Write( messageID );// messageType.Identifier );//, NetworkDefines.bitsForServiceMessageTypeIdentifier );

			return sendDataWriter;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( IList<NetworkNode.ConnectedNode> recipients, MessageType messageType )
		{
			return BeginMessage( recipients, messageType.Identifier );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( NetworkNode.ConnectedNode recipient, int messageID )
		{
			//!!!!не фаталы, мир не падает. везде так
			if( sendingData )
				Log.Fatal( "ServerNetworkService: BeginMessage: The message is already begun." );
			if( recipient == null )
				Log.Fatal( "ServerNetworkService: BeginMessage: recipient = null." );
			if( messageID <= 0 || messageID > 15 )
				Log.Fatal( "ServerNetworkService: BeginMessage: messageID <= 0 || messageID > 15." );

			sendingData = true;
			sendingMessageID = messageID;
			sendingDestinationNodes.Add( recipient );
			sendDataWriter.Reset();

			sendDataWriter.Write( (byte)( ( Identifier << 4 ) + messageID ) );
			//sendDataWriter.Write( Identifier );//, NetworkDefines.bitsForServiceIdentifier );
			//sendDataWriter.Write( messageID );// messageType.Identifier );//, NetworkDefines.bitsForServiceMessageTypeIdentifier );

			return sendDataWriter;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( NetworkNode.ConnectedNode recipient, MessageType messageType )
		{
			return BeginMessage( recipient, messageType.Identifier );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessageToEveryone( int messageID )
		{
			//!!!!broadcast
			//!!!!where else. broadcast scene data

			return BeginMessage( owner.GetConnectedNodesArray(), messageID );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessageToEveryone( MessageType messageType )
		{
			return BeginMessage( owner.GetConnectedNodesArray(), messageType );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( int messageID )
		{
			if( sendingData )
				Log.Fatal( "ServerNetworkService: BeginMessage: The message is already begun." );
			//if( recipients == null )
			//	Log.Fatal( "ServerNetworkService: BeginMessage: recipients = null." );
			if( messageID <= 0 || messageID > 15 )
				Log.Fatal( "ServerNetworkService: BeginMessage: messageID <= 0 || messageID > 15." );

			sendingData = true;
			sendingMessageID = messageID;
			//for( int n = 0; n < recipients.Count; n++ )
			//	sendingDestinationNodes.Add( recipients[ n ] );
			sendDataWriter.Reset();

			sendDataWriter.Write( (byte)( ( Identifier << 4 ) + messageID ) );
			//sendDataWriter.Write( Identifier );//, NetworkDefines.bitsForServiceIdentifier );
			//sendDataWriter.Write( messageID );// messageType.Identifier );//, NetworkDefines.bitsForServiceMessageTypeIdentifier );

			return sendDataWriter;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected ArrayDataWriter BeginMessage( MessageType messageType )
		{
			return BeginMessage( messageType.Identifier );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		protected void AddMessageRecipient( NetworkNode.ConnectedNode recipient )
		{
			if( !sendingData )
				Log.Fatal( "ServerNetworkService: AddMessageRecipient: The message is not begun." );

			sendingDestinationNodes.Add( recipient );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected void EndMessage()
		{
			if( !sendingData )
				Log.Fatal( "ServerNetworkService: EndMessage: The message is not begun." );

			if( owner != null && owner.server != null )
			{
				for( int n = 0; n < sendingDestinationNodes.Count; n++ )
				{
					var connectedNode = sendingDestinationNodes[ n ];
					connectedNode.AddDataForSending( sendDataWriter );

					if( owner.ProfilerData != null )
					{
						var serviceItem = owner.ProfilerData.GetServiceItem( Identifier );
						var messageTypeItem = serviceItem.GetMessageTypeItem( sendingMessageID );
						messageTypeItem.SentMessages++;
						messageTypeItem.SentSize += sendDataWriter.Length;
					}
				}
			}

			sendingData = false;
			sendingMessageID = -1;
			sendingDestinationNodes.Clear();
			sendDataWriter.Reset();
		}
	}
}
