//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Text;

//using Lidgren.Network;

//namespace NeoAxis
//{
//	public abstract class ClientNetworkService : NetworkService
//	{
//		internal NetworkClient owner;

//		//send data
//		bool sendingData;
//		ArrayDataWriter sendDataWriter = new ArrayDataWriter();

//		///////////////////////////////////////////

//		protected ClientNetworkService( string name, byte identifier )
//			: base( name, identifier )
//		{
//		}

//		public NetworkClient Owner
//		{
//			get { return owner; }
//		}

//		protected internal override void Dispose()
//		{
//			owner = null;
//			base.Dispose();
//		}

//		protected ArrayDataWriter BeginMessage( MessageType messageType )
//		{
//			if( sendingData )
//				Log.Fatal( "ClientNetworkService: BeginMessage: The message is already begun." );
//			if( messageType == null )
//				Log.Fatal( "ClientNetworkService: BeginMessage: messageType = null." );

//			sendingData = true;
//			sendDataWriter.Reset();

//			sendDataWriter.Write( Identifier, NetworkDefines.bitsForServiceIdentifier );
//			sendDataWriter.Write( messageType.Identifier,
//				NetworkDefines.bitsForServiceMessageTypeIdentifier );

//			return sendDataWriter;
//		}

//		protected void EndMessage()
//		{
//			if( !sendingData )
//				Log.Fatal( "ClientNetworkService: EndMessage: The message is not begun." );

//			if( owner != null && owner.client != null )
//			{
//				NetworkNode.ConnectedNode connectedNode = Owner.ServerConnectedNode;
//				connectedNode.AddDataForSending( sendDataWriter );
//			}

//			sendingData = false;
//			sendDataWriter.Reset();
//		}
//	}
//}
