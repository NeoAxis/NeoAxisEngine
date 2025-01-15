// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis.Networking;

namespace NeoAxis
{
	/// <summary>
	/// A basic server service for string and binary messages.
	/// </summary>
	public class ServerNetworkService_Messages : ServerService
	{
		int receiveDataSizeLimit = 10 * 1024 * 1024;

		MessageType transferMessageStringType;
		MessageType transferMessageBinaryType;
		MessageType messageToAllClientsStringType;
		MessageType messageToAllClientsBinaryType;

		///////////////////////////////////////////

		public delegate void ReceiveMessageStringDelegate( ServerNetworkService_Messages sender, ServerNode.Client client, string message, string data );
		public event ReceiveMessageStringDelegate ReceiveMessageString;

		public delegate void ReceiveMessageBinaryDelegate( ServerNetworkService_Messages sender, ServerNode.Client client, string message, byte[] data );
		public event ReceiveMessageBinaryDelegate ReceiveMessageBinary;

		public delegate void ReceiveMessageToAllClientsStringDelegate( ServerNetworkService_Messages sender, ServerNode.Client client, string message, string data, ref bool handled );
		public event ReceiveMessageToAllClientsStringDelegate ReceiveMessageToAllClientsString;

		public delegate void ReceiveMessageToAllClientsBinaryDelegate( ServerNetworkService_Messages sender, ServerNode.Client client, string message, byte[] data, ref bool handled );
		public event ReceiveMessageToAllClientsBinaryDelegate ReceiveMessageToAllClientsBinary;

		///////////////////////////////////////////

		public ServerNetworkService_Messages()
			: base( "Messages", 1 )
		{
			//register message types
			transferMessageStringType = RegisterMessageType( "TransferMessageString", 1, ReceiveMessage_TransferMessageStringToServer );
			transferMessageBinaryType = RegisterMessageType( "TransferMessageBinary", 2, ReceiveMessage_TransferMessageBinaryToServer );
			messageToAllClientsStringType = RegisterMessageType( "MessageToAllClientsString", 3, ReceiveMessage_MessageToAllClientsStringToServer );
			messageToAllClientsBinaryType = RegisterMessageType( "MessageToAllClientsBinary", 4, ReceiveMessage_MessageToAllClientsBinaryToServer );
		}

		public int ReceiveDataSizeLimit
		{
			get { return receiveDataSizeLimit; }
			set { receiveDataSizeLimit = value; }
		}

		bool ReceiveMessage_TransferMessageStringToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			string message = reader.ReadString();
			string data = reader.ReadString();
			if( !reader.Complete() )
				return false;

			ReceiveMessageString?.Invoke( this, sender, message, data );

			return true;
		}

		bool ReceiveMessage_TransferMessageBinaryToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			string message = reader.ReadString();

			var dataSize = reader.ReadInt32();

			//!!!!where else
			if( dataSize > ReceiveDataSizeLimit )
			{
				error = $"The size of the data is too large. The maximum size is {ReceiveDataSizeLimit} bytes.";
				return false;
			}

			var data = new byte[ dataSize ];
			reader.ReadBuffer( data, 0, dataSize );
			if( !reader.Complete() )
				return false;

			ReceiveMessageBinary?.Invoke( this, sender, message, data );

			return true;
		}

		bool ReceiveMessage_MessageToAllClientsStringToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			string message = reader.ReadString();
			string data = reader.ReadString();
			if( !reader.Complete() )
				return false;

			var handled = false;
			ReceiveMessageToAllClientsString?.Invoke( this, sender, message, data, ref handled );

			if( !handled )
				SendToAllClients( message, data );

			return true;
		}

		bool ReceiveMessage_MessageToAllClientsBinaryToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			string message = reader.ReadString();
			var dataSize = reader.ReadInt32();

			if( dataSize > ReceiveDataSizeLimit )
			{
				error = $"The size of the data is too large. The maximum size is {ReceiveDataSizeLimit} bytes.";
				return false;
			}

			var data = new byte[ dataSize ];
			reader.ReadBuffer( data, 0, dataSize );
			if( !reader.Complete() )
				return false;

			var handled = false;
			ReceiveMessageToAllClientsBinary?.Invoke( this, sender, message, data, ref handled );

			if( !handled )
				SendToAllClients( message, data );

			return true;
		}

		public void SendToClient( ServerNode.Client client, string message, string data )
		{
			var writer = BeginMessage( client, transferMessageStringType );
			writer.Write( message );
			writer.Write( data );
			EndMessage();
		}

		public void SendToClient( ServerNode.Client client, string message, byte[] data )
		{
			var writer = BeginMessage( client, transferMessageBinaryType );
			writer.Write( message );
			writer.Write( data.Length );
			writer.Write( data );
			EndMessage();
		}

		public void SendToAllClients( string message, string data )
		{
			//!!!!broadcast
			//!!!!где еще. сцену раздавать всем броадкастом

			var writer = BeginMessageToEveryone( transferMessageStringType );
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

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// A basic client service for string and binary messages.
	/// </summary>
	public class ClientNetworkService_Messages : ClientService
	{
		MessageType transferMessageStringType;
		MessageType transferMessageBinaryType;
		MessageType messageToAllClientsStringType;
		MessageType messageToAllClientsBinaryType;

		///////////////////////////////////////////

		public delegate void ReceiveMessageStringDelegate( ClientNetworkService_Messages sender, string message, string data );
		public event ReceiveMessageStringDelegate ReceiveMessageString;

		public delegate void ReceiveMessageBinaryDelegate( ClientNetworkService_Messages sender, string message, byte[] data );
		public event ReceiveMessageBinaryDelegate ReceiveMessageBinary;

		///////////////////////////////////////////

		public ClientNetworkService_Messages()
			: base( "Messages", 1 )
		{
			//register message types
			transferMessageStringType = RegisterMessageType( "TransferMessageString", 1, ReceiveMessage_TransferMessageStringToClient );
			transferMessageBinaryType = RegisterMessageType( "TransferMessageBinary", 2, ReceiveMessage_TransferMessageBinaryToClient );
			messageToAllClientsStringType = RegisterMessageType( "MessageToAllClientsString", 3 );
			messageToAllClientsBinaryType = RegisterMessageType( "MessageToAllClientsBinary", 4 );
		}

		public void SendToServer( string message, string data )
		{
			var writer = BeginMessage( transferMessageStringType );
			writer.Write( message );
			writer.Write( data );
			EndMessage();
		}

		public void SendToServer( string message, byte[] data, int offset, int length )
		{
			var writer = BeginMessage( transferMessageBinaryType );
			writer.Write( message );
			writer.Write( length );
			writer.Write( data, offset, length );
			EndMessage();
		}

		public void SendToServer( string message, byte[] data )
		{
			SendToServer( message, data, 0, data.Length );
		}

		public void SendToAllClients( string message, string data )
		{
			var writer = BeginMessage( messageToAllClientsStringType );
			writer.Write( message );
			writer.Write( data );
			EndMessage();
		}

		public void SendToAllClients( string message, byte[] data, int offset, int length )
		{
			var writer = BeginMessage( messageToAllClientsBinaryType );
			writer.Write( message );
			writer.Write( length );
			writer.Write( data, offset, length );
			EndMessage();
		}

		public void SendToAllClients( string message, byte[] data )
		{
			SendToAllClients( message, data, 0, data.Length );
		}


		//public ArrayDataWriter SendToServerBinaryBegin( string message )
		//{
		//	var writer = BeginMessage( transferMessageBinaryType );
		//	writer.Write( message );
		//	return writer;
		//}

		//public void SendToServerBinaryEnd()
		//{
		//	EndMessage();
		//}

		bool ReceiveMessage_TransferMessageStringToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			string message = reader.ReadString();
			string data = reader.ReadString();
			if( !reader.Complete() )
				return false;

			ReceiveMessageString?.Invoke( this, message, data );

			return true;
		}

		bool ReceiveMessage_TransferMessageBinaryToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			string message = reader.ReadString();
			var dataSize = reader.ReadInt32();

			var data = new byte[ dataSize ];
			reader.ReadBuffer( data, 0, dataSize );
			if( !reader.Complete() )
				return false;

			ReceiveMessageBinary?.Invoke( this, message, data );

			return true;
		}
	}
}