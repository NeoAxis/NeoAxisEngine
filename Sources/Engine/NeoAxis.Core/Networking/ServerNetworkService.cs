//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Text;

//using Lidgren.Network;

//namespace NeoAxis
//{
//	public abstract class ServerNetworkService : NetworkService
//	{
//		internal NetworkServer owner;

//		//send data
//		bool sendingData;
//		List<NetworkNode.ConnectedNode> sendingDestinationNodes =
//			new List<NetworkNode.ConnectedNode>();
//		ArrayDataWriter sendDataWriter = new ArrayDataWriter();

//		///////////////////////////////////////////

//		protected ServerNetworkService( string name, byte identifier )
//			: base( name, identifier )
//		{
//		}

//		public NetworkServer Owner
//		{
//			get { return owner; }
//		}

//		protected internal override void Dispose()
//		{
//			owner = null;
//			base.Dispose();
//		}

//		protected ArrayDataWriter BeginMessage( IList<NetworkNode.ConnectedNode> recipients,
//			MessageType messageType )
//		{
//			if( sendingData )
//				Log.Fatal( "ServerNetworkService: BeginMessage: The message is already begun." );
//			if( recipients == null )
//				Log.Fatal( "ServerNetworkService: BeginMessage: recipients = null." );
//			if( messageType == null )
//				Log.Fatal( "ServerNetworkService: BeginMessage: messageType = null." );

//			sendingData = true;
//			for( int n = 0; n < recipients.Count; n++ )
//				sendingDestinationNodes.Add( recipients[ n ] );
//			sendDataWriter.Reset();

//			sendDataWriter.Write( Identifier, NetworkDefines.bitsForServiceIdentifier );
//			sendDataWriter.Write( messageType.Identifier,
//				NetworkDefines.bitsForServiceMessageTypeIdentifier );

//			return sendDataWriter;
//		}

//		protected ArrayDataWriter BeginMessage( NetworkNode.ConnectedNode recipient,
//			MessageType messageType )
//		{
//			if( sendingData )
//				Log.Fatal( "ServerNetworkService: BeginMessage: The message is already begun." );
//			if( recipient == null )
//				Log.Fatal( "ServerNetworkService: BeginMessage: recipient = null." );
//			if( messageType == null )
//				Log.Fatal( "ServerNetworkService: BeginMessage: messageType = null." );

//			sendingData = true;
//			sendingDestinationNodes.Add( recipient );
//			sendDataWriter.Reset();

//			sendDataWriter.Write( Identifier, NetworkDefines.bitsForServiceIdentifier );
//			sendDataWriter.Write( messageType.Identifier,
//				NetworkDefines.bitsForServiceMessageTypeIdentifier );

//			return sendDataWriter;
//		}

//		protected ArrayDataWriter BeginMessageToAll( MessageType messageType )
//		{
//			return BeginMessage( owner.ConnectedNodes, messageType );
//		}

//		protected void EndMessage()
//		{
//			if( !sendingData )
//				Log.Fatal( "ServerNetworkService: EndMessage: The message is not begun." );

//			if( owner != null && owner.server != null )
//			{
//				for( int n = 0; n < sendingDestinationNodes.Count; n++ )
//				{
//					NetworkNode.ConnectedNode connectedNode = sendingDestinationNodes[ n ];
//					connectedNode.AddDataForSending( sendDataWriter );
//				}
//			}

//			sendingData = false;
//			sendingDestinationNodes.Clear();
//			sendDataWriter.Reset();
//		}
//	}
//}
