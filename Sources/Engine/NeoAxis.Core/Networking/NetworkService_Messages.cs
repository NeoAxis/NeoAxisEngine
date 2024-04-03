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
		MessageType transferMessageBinaryType;

		///////////////////////////////////////////

		public delegate void ReceiveMessageDelegate( ServerNetworkService_Messages sender, NetworkNode.ConnectedNode connectedNode, string message, string data );
		public event ReceiveMessageDelegate ReceiveMessage;

		public delegate void ReceiveMessageBinaryDelegate( ServerNetworkService_Messages sender, NetworkNode.ConnectedNode connectedNode, string message, byte[] data );
		public event ReceiveMessageBinaryDelegate ReceiveMessageBinary;

		///////////////////////////////////////////

		public ServerNetworkService_Messages()
			: base( "Messages", 1 )
		{
			//register message types
			transferMessageType = RegisterMessageType( "TransferMessage", 1, ReceiveMessage_TransferMessageToServer );
			transferMessageBinaryType = RegisterMessageType( "TransferMessageBinary", 2, ReceiveMessage_TransferMessageBinaryToServer );
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

		bool ReceiveMessage_TransferMessageBinaryToServer( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			string message = reader.ReadString();
			var dataSize = reader.ReadInt32();

			//!!!!проверка на максимальный размер. в lidgren максимальный размер памяти для соединения. где еще

			var data = new byte[ dataSize ];
			reader.ReadBuffer( data, 0, dataSize );
			if( !reader.Complete() )
				return false;

			ReceiveMessageBinary?.Invoke( this, sender, message, data );

			return true;
		}

		public void SendToClient( NetworkNode.ConnectedNode connectedNode, string message, string data )
		{
			var writer = BeginMessage( connectedNode, transferMessageType );
			writer.Write( message );
			writer.Write( data );
			EndMessage();
		}

		public void SendToClient( NetworkNode.ConnectedNode connectedNode, string message, byte[] data )
		{
			var writer = BeginMessage( connectedNode, transferMessageBinaryType );
			writer.Write( message );
			writer.Write( data.Length );
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

		public void SendToAllClients( string message, byte[] data )
		{
			//!!!!broadcast
			//!!!!где еще. сцену раздавать всем броадкастом

			var writer = BeginMessageToEveryone( transferMessageBinaryType );
			writer.Write( message );
			writer.Write( data.Length );
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
		MessageType transferMessageBinaryType;

		///////////////////////////////////////////

		public delegate void ReceiveMessageDelegate( ClientNetworkService_Messages sender, string message, string data );
		public event ReceiveMessageDelegate ReceiveMessage;

		public delegate void ReceiveMessageBinaryDelegate( ClientNetworkService_Messages sender, string message, byte[] data );
		public event ReceiveMessageBinaryDelegate ReceiveMessageBinary;

		///////////////////////////////////////////

		public ClientNetworkService_Messages()
			: base( "Messages", 1 )
		{
			//register message types
			transferMessageType = RegisterMessageType( "TransferMessage", 1, ReceiveMessage_TransferMessageToClient );
			transferMessageBinaryType = RegisterMessageType( "TransferMessageBinary", 2, ReceiveMessage_TransferMessageBinaryToClient );
		}

		public void SendToServer( string message, string data )
		{
			var writer = BeginMessage( transferMessageType );
			writer.Write( message );
			writer.Write( data );
			EndMessage();
		}

		public void SendToServer( string message, byte[] data )
		{
			var writer = BeginMessage( transferMessageBinaryType );
			writer.Write( message );
			writer.Write( data.Length );
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

		bool ReceiveMessage_TransferMessageBinaryToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			string message = reader.ReadString();
			var dataSize = reader.ReadInt32();

			//!!!!проверка на максимальный размер. в lidgren максимальный размер памяти для соединения. где еще

			var data = new byte[ dataSize ];
			reader.ReadBuffer( data, 0, dataSize );
			if( !reader.Complete() )
				return false;

			ReceiveMessageBinary?.Invoke( this, message, data );

			return true;
		}
	}
}