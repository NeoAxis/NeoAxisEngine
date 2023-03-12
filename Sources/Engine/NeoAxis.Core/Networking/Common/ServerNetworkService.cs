// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Networking
{
	public abstract class ServerNetworkService : NetworkService
	{
		internal NetworkServerNode owner;

		//send data
		bool sendingData;
		List<NetworkNode.ConnectedNode> sendingDestinationNodes = new List<NetworkNode.ConnectedNode>();
		ArrayDataWriter sendDataWriter = new ArrayDataWriter();

		///////////////////////////////////////////

		protected ServerNetworkService( string name, byte identifier )
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

		protected ArrayDataWriter BeginMessage( IList<NetworkNode.ConnectedNode> recipients, byte messageID )
		{
			if( sendingData )
				Log.Fatal( "ServerNetworkService: BeginMessage: The message is already begun." );
			if( recipients == null )
				Log.Fatal( "ServerNetworkService: BeginMessage: recipients = null." );
			if( messageID <= 0 || messageID > 255 )
				Log.Fatal( "ServerNetworkService: BeginMessage: messageID <= 0 || messageID > 255." );
			//if( messageType == null )
			//	Log.Fatal( "ServerNetworkService: BeginMessage: messageType = null." );

			sendingData = true;
			for( int n = 0; n < recipients.Count; n++ )
				sendingDestinationNodes.Add( recipients[ n ] );
			sendDataWriter.Reset();

			sendDataWriter.Write( Identifier );//, NetworkDefines.bitsForServiceIdentifier );
			sendDataWriter.Write( messageID );// messageType.Identifier );//, NetworkDefines.bitsForServiceMessageTypeIdentifier );

			return sendDataWriter;
		}

		protected ArrayDataWriter BeginMessage( IList<NetworkNode.ConnectedNode> recipients, MessageType messageType )
		{
			return BeginMessage( recipients, messageType.Identifier );
		}

		protected ArrayDataWriter BeginMessage( NetworkNode.ConnectedNode recipient, byte messageID )
		{
			//!!!!не фаталы, мир не падает. везде так
			if( sendingData )
				Log.Fatal( "ServerNetworkService: BeginMessage: The message is already begun." );
			if( recipient == null )
				Log.Fatal( "ServerNetworkService: BeginMessage: recipient = null." );
			if( messageID <= 0 || messageID > 255 )
				Log.Fatal( "ServerNetworkService: BeginMessage: messageID <= 0 || messageID > 255." );
			//if( messageType == null )
			//	Log.Fatal( "ServerNetworkService: BeginMessage: messageType = null." );

			sendingData = true;
			sendingDestinationNodes.Add( recipient );
			sendDataWriter.Reset();

			sendDataWriter.Write( Identifier );//, NetworkDefines.bitsForServiceIdentifier );
			sendDataWriter.Write( messageID );// messageType.Identifier );//, NetworkDefines.bitsForServiceMessageTypeIdentifier );

			return sendDataWriter;
		}

		protected ArrayDataWriter BeginMessage( NetworkNode.ConnectedNode recipient, MessageType messageType )
		{
			return BeginMessage( recipient, messageType.Identifier );
		}

		protected ArrayDataWriter BeginMessageToEveryone( byte messageID )
		{
			//!!!!broadcast
			//!!!!where else. broadcast scene data

			return BeginMessage( owner.GetConnectedNodesArray(), messageID );
		}

		protected ArrayDataWriter BeginMessageToEveryone( MessageType messageType )
		{
			return BeginMessage( owner.GetConnectedNodesArray(), messageType );
		}

		protected ArrayDataWriter BeginMessage( byte messageID )
		{
			if( sendingData )
				Log.Fatal( "ServerNetworkService: BeginMessage: The message is already begun." );
			//if( recipients == null )
			//	Log.Fatal( "ServerNetworkService: BeginMessage: recipients = null." );
			if( messageID <= 0 || messageID > 255 )
				Log.Fatal( "ServerNetworkService: BeginMessage: messageID <= 0 || messageID > 255." );
			//if( messageType == null )
			//	Log.Fatal( "ServerNetworkService: BeginMessage: messageType = null." );

			sendingData = true;
			//for( int n = 0; n < recipients.Count; n++ )
			//	sendingDestinationNodes.Add( recipients[ n ] );
			sendDataWriter.Reset();

			sendDataWriter.Write( Identifier );//, NetworkDefines.bitsForServiceIdentifier );
			sendDataWriter.Write( messageID );// messageType.Identifier );//, NetworkDefines.bitsForServiceMessageTypeIdentifier );

			return sendDataWriter;
		}

		protected ArrayDataWriter BeginMessage( MessageType messageType )
		{
			return BeginMessage( messageType.Identifier );
		}

		protected void AddMessageRecipient( NetworkNode.ConnectedNode recipient )
		{
			if( !sendingData )
				Log.Fatal( "ServerNetworkService: AddMessageRecipient: The message is not begun." );

			sendingDestinationNodes.Add( recipient );
		}

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
				}
			}

			sendingData = false;
			sendingDestinationNodes.Clear();
			sendDataWriter.Reset();
		}
	}
}
