// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis.Networking;

namespace NeoAxis
{
	/// <summary>
	/// A basic server service to exchange by means simple text and binary messages.
	/// </summary>
	public class ServerNetworkService_Messages : ServerService
	{
		public int MaxMessageLength { get; set; } = 1024;
		public int MaxStringDataLength { get; set; } = 5 * 1024 * 1024;
		public int MaxBinaryDataSize { get; set; } = 10 * 1024 * 1024;

		MessageType transferMessageString;
		MessageType transferMessageBinary;
		MessageType messageToAllClientsString;
		MessageType messageToAllClientsBinary;

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
			transferMessageString = RegisterMessageType( "TransferMessageString", 1, ReceiveMessage_TransferMessageStringToServer );
			transferMessageBinary = RegisterMessageType( "TransferMessageBinary", 2, ReceiveMessage_TransferMessageBinaryToServer );
			messageToAllClientsString = RegisterMessageType( "MessageToAllClientsString", 3, ReceiveMessage_MessageToAllClientsStringToServer );
			messageToAllClientsBinary = RegisterMessageType( "MessageToAllClientsBinary", 4, ReceiveMessage_MessageToAllClientsBinaryToServer );
		}

		bool ReceiveMessage_TransferMessageStringToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var message = reader.ReadString() ?? "";
			if( message.Length > MaxMessageLength )
			{
				error = $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.";
				return false;
			}

			var data = reader.ReadString() ?? "";
			if( data.Length > MaxStringDataLength )
			{
				error = $"The length of the data is too long. The maximum length is {MaxStringDataLength} characters.";
				return false;
			}

			if( !reader.Complete() )
				return false;

			ReceiveMessageString?.Invoke( this, sender, message, data );

			return true;
		}

		bool ReceiveMessage_TransferMessageBinaryToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			string message = reader.ReadString() ?? "";
			if( message.Length > MaxMessageLength )
			{
				error = $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.";
				return false;
			}

			var dataSize = reader.ReadInt32();
			if( dataSize > MaxBinaryDataSize )
			{
				error = $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.";
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
			var message = reader.ReadString() ?? "";
			if( message.Length > MaxMessageLength )
			{
				error = $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.";
				return false;
			}

			var data = reader.ReadString() ?? "";
			if( data.Length > MaxStringDataLength )
			{
				error = $"The length of the data is too long. The maximum length is {MaxStringDataLength} characters.";
				return false;
			}

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
			var message = reader.ReadString() ?? "";
			if( message.Length > MaxMessageLength )
			{
				error = $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.";
				return false;
			}

			var dataSize = reader.ReadInt32();
			if( dataSize > MaxBinaryDataSize )
			{
				error = $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.";
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
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Length > MaxStringDataLength )
				throw new ArgumentException( $"The length of the data is too long. The maximum length is {MaxStringDataLength} characters.", nameof( data ) );

			var m = BeginMessage( client, transferMessageString );
			m.Writer.Write( message );
			m.Writer.Write( data );
			m.End();
		}

		public void SendToClient( ServerNode.Client client, string message, ArraySegment<byte> data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Count > MaxBinaryDataSize )
				throw new ArgumentException( $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.", nameof( data ) );

			var m = BeginMessage( client, transferMessageBinary );
			m.Writer.Write( message );
			m.Writer.Write( data.Count );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}

		public void SendToClient( ServerNode.Client client, string message, byte[] data )
		{
			SendToClient( client, message, new ArraySegment<byte>( data ) );

			//if( message.Length > MaxMessageLength )
			//	throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			//if( data.Length > MaxBinaryDataSize )
			//	throw new ArgumentException( $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.", nameof( data ) );

			//var m = BeginMessage( client, transferMessageBinary );
			//m.Writer.Write( message );
			//m.Writer.Write( data.Length );
			//m.Writer.Write( data );
			//m.End();
		}

		//!!!!new
		public void SendToClients( IList<ServerNode.Client> clients, string message, string data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Length > MaxStringDataLength )
				throw new ArgumentException( $"The length of the data is too long. The maximum length is {MaxStringDataLength} characters.", nameof( data ) );

			var m = BeginMessage( clients, transferMessageString );
			m.Writer.Write( message );
			m.Writer.Write( data );
			m.End();
		}

		//!!!!new
		public void SendToClients( IList<ServerNode.Client> clients, string message, ArraySegment<byte> data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Count > MaxBinaryDataSize )
				throw new ArgumentException( $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.", nameof( data ) );

			var m = BeginMessage( clients, transferMessageBinary );
			m.Writer.Write( message );
			m.Writer.Write( data.Count );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}

		//!!!!new
		public void SendToClients( IList<ServerNode.Client> clients, string message, byte[] data )
		{
			SendToClients( clients, message, new ArraySegment<byte>( data ) );
		}

		public void SendToAllClients( string message, string data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Length > MaxStringDataLength )
				throw new ArgumentException( $"The length of the data is too long. The maximum length is {MaxStringDataLength} characters.", nameof( data ) );

			//!!!!broadcast? where else

			var m = BeginMessageToAll( transferMessageString );
			m.Writer.Write( message );
			m.Writer.Write( data );
			m.End();
		}

		public void SendToAllClients( string message, ArraySegment<byte> data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Count > MaxBinaryDataSize )
				throw new ArgumentException( $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.", nameof( data ) );

			//!!!!broadcast? where else

			var m = BeginMessageToAll( transferMessageBinary );
			m.Writer.Write( message );
			m.Writer.Write( data.Count );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}

		public void SendToAllClients( string message, byte[] data )
		{
			SendToAllClients( message, new ArraySegment<byte>( data ) );

			//if( message.Length > MaxMessageLength )
			//	throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			//if( data.Length > MaxBinaryDataSize )
			//	throw new ArgumentException( $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.", nameof( data ) );

			//var m = BeginMessageToAll( transferMessageBinary );
			//m.Writer.Write( message );
			//m.Writer.Write( data.Length );
			//m.Writer.Write( data );
			//m.End();
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// A basic client service for string and binary messages.
	/// </summary>
	public class ClientNetworkService_Messages : ClientService
	{
		public int MaxMessageLength { get; set; } = 1024;
		public int MaxStringDataLength { get; set; } = 5 * 1024 * 1024;
		public int MaxBinaryDataSize { get; set; } = 10 * 1024 * 1024;

		MessageType transferMessageString;
		MessageType transferMessageBinary;
		MessageType messageToAllClientsString;
		MessageType messageToAllClientsBinary;

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
			transferMessageString = RegisterMessageType( "TransferMessageString", 1, ReceiveMessage_TransferMessageStringToClient );
			transferMessageBinary = RegisterMessageType( "TransferMessageBinary", 2, ReceiveMessage_TransferMessageBinaryToClient );
			messageToAllClientsString = RegisterMessageType( "MessageToAllClientsString", 3 );
			messageToAllClientsBinary = RegisterMessageType( "MessageToAllClientsBinary", 4 );
		}

		bool ReceiveMessage_TransferMessageStringToClient( MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var message = reader.ReadString() ?? "";
			if( message.Length > MaxMessageLength )
			{
				error = $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.";
				return false;
			}

			var data = reader.ReadString() ?? "";
			if( data.Length > MaxStringDataLength )
			{
				error = $"The length of the data is too long. The maximum length is {MaxStringDataLength} characters.";
				return false;
			}

			if( !reader.Complete() )
				return false;

			ReceiveMessageString?.Invoke( this, message, data );

			return true;
		}

		bool ReceiveMessage_TransferMessageBinaryToClient( MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var message = reader.ReadString() ?? "";
			if( message.Length > MaxMessageLength )
			{
				error = $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.";
				return false;
			}

			var dataSize = reader.ReadInt32();
			if( dataSize > MaxBinaryDataSize )
			{
				error = $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.";
				return false;
			}

			var data = new byte[ dataSize ];
			reader.ReadBuffer( data, 0, dataSize );

			if( !reader.Complete() )
				return false;

			ReceiveMessageBinary?.Invoke( this, message, data );

			return true;
		}

		public void SendToServer( string message, string data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Length > MaxStringDataLength )
				throw new ArgumentException( $"The length of the data is too long. The maximum length is {MaxStringDataLength} characters.", nameof( data ) );

			var m = BeginMessage( transferMessageString );
			m.Writer.Write( message );
			m.Writer.Write( data );
			m.End();
		}

		public void SendToServer( string message, ArraySegment<byte> data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Count > MaxBinaryDataSize )
				throw new ArgumentException( $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.", nameof( data ) );

			var m = BeginMessage( transferMessageBinary );
			m.Writer.Write( message );
			m.Writer.Write( data.Count );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}

		public void SendToServer( string message, byte[] data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Length > MaxBinaryDataSize )
				throw new ArgumentException( $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.", nameof( data ) );

			var m = BeginMessage( transferMessageBinary );
			m.Writer.Write( message );
			m.Writer.Write( data.Length );
			m.Writer.Write( data );
			m.End();
		}

		public void SendToServerWithForwardToAllClients( string message, string data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Length > MaxStringDataLength )
				throw new ArgumentException( $"The length of the data is too long. The maximum length is {MaxStringDataLength} characters.", nameof( data ) );

			var m = BeginMessage( messageToAllClientsString );
			m.Writer.Write( message );
			m.Writer.Write( data );
			m.End();
		}

		public void SendToServerWithForwardToAllClients( string message, ArraySegment<byte> data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Count > MaxBinaryDataSize )
				throw new ArgumentException( $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.", nameof( data ) );

			var m = BeginMessage( messageToAllClientsBinary );
			m.Writer.Write( message );
			m.Writer.Write( data.Count );
			m.Writer.Write( data.Array, data.Offset, data.Count );
			m.End();
		}

		public void SendToServerWithForwardToAllClients( string message, byte[] data )
		{
			if( message.Length > MaxMessageLength )
				throw new ArgumentException( $"The length of the message is too long. The maximum length is {MaxMessageLength} characters.", nameof( message ) );
			if( data.Length > MaxBinaryDataSize )
				throw new ArgumentException( $"The size of the data is too large. The maximum size is {MaxBinaryDataSize} bytes.", nameof( data ) );

			var m = BeginMessage( messageToAllClientsBinary );
			m.Writer.Write( message );
			m.Writer.Write( data.Length );
			m.Writer.Write( data );
			m.End();
		}
	}
}