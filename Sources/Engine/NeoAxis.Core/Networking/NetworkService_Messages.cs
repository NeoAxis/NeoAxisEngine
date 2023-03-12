// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis.Networking;

namespace NeoAxis
{
	public class ServerNetworkService_Messages : ServerNetworkService
	{
		MessageType transferMessageType;

		///////////////////////////////////////////

		public delegate void ReceiveMessageDelegate( ServerNetworkService_Messages sender, NetworkNode.ConnectedNode connectedNode, string message, string data );
		public event ReceiveMessageDelegate ReceiveMessage;

		///////////////////////////////////////////

		public ServerNetworkService_Messages()
			: base( "Messages", 1 )
		{
			//register message types
			transferMessageType = RegisterMessageType( "TransferMessage", 1, ReceiveMessage_TransferMessageToServer );
		}

		bool ReceiveMessage_TransferMessageToServer( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			string message = reader.ReadString();
			string data = reader.ReadString();
			if( !reader.Complete() )
				return false;

			ReceiveMessage?.Invoke( this, sender, message, data );

			return true;
		}

		public void SendToClient( NetworkNode.ConnectedNode connectedNode, string message, string data )
		{
			var writer = BeginMessage( connectedNode, transferMessageType );
			writer.Write( message );
			writer.Write( data );
			EndMessage();
		}

		public void SendToAllClients( string message, string data )
		{
			//!!!!broadcast
			//!!!!где еще. сцену раздавать всем броадкастом

			var writer = BeginMessageToEveryone( transferMessageType );
			writer.Write( message );
			writer.Write( data );
			EndMessage();

			//foreach( NetworkNode.ConnectedNode connectedNode in Owner.ConnectedNodes )
			//	SendToClient( connectedNode, message, data );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_Messages : ClientNetworkService
	{
		MessageType transferMessageType;

		///////////////////////////////////////////

		public delegate void ReceiveMessageDelegate( ClientNetworkService_Messages sender, string message, string data );
		public event ReceiveMessageDelegate ReceiveMessage;

		///////////////////////////////////////////

		public ClientNetworkService_Messages()
			: base( "Messages", 1 )
		{
			//register message types
			transferMessageType = RegisterMessageType( "TransferMessage", 1, ReceiveMessage_TransferMessageToClient );
		}

		public void SendToServer( string message, string data )
		{
			var writer = BeginMessage( transferMessageType );
			writer.Write( message );
			writer.Write( data );
			EndMessage();
		}

		bool ReceiveMessage_TransferMessageToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			string message = reader.ReadString();
			string data = reader.ReadString();
			if( !reader.Complete() )
				return false;

			ReceiveMessage?.Invoke( this, message, data );

			return true;
		}
	}
}