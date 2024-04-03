// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis.Networking;

namespace NeoAxis
{
	public class ServerNetworkService_Chat : ServerNetworkService
	{
		ServerNetworkService_Users users;

		///////////////////////////////////////////

		public delegate void ReceiveTextDelegate( ServerNetworkService_Chat sender, ServerNetworkService_Users.UserInfo fromUser, string text, ServerNetworkService_Users.UserInfo privateToUser );
		public event ReceiveTextDelegate ReceiveText;

		///////////////////////////////////////////

		public ServerNetworkService_Chat( ServerNetworkService_Users users )
			: base( "Chat", 3 )
		{
			this.users = users;

			//register message types
			RegisterMessageType( "TextToServer", 1, ReceiveMessage_TextToServer );
			RegisterMessageType( "TextToClient", 2 );
		}

		bool ReceiveMessage_TextToServer( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get source user
			var fromUser = users.GetUser( sender );

			//get data of message
			var text = reader.ReadString();
			var privateToUserID = reader.ReadInt64();
			//long privateToUserID = (long)reader.ReadVariableUInt64();
			if( !reader.Complete() )
				return false;

			//send text to the clients
			if( privateToUserID != 0 )
			{
				//send text to the specific user

				var privateToUser = users.GetUser( privateToUserID );
				if( privateToUser != null )
				{
					SendText( fromUser, text, privateToUser );
				}
				else
				{
					//no user anymore
				}
			}
			else
			{
				SendText( fromUser, text, null );
			}

			return true;
		}

		//public void SayToAll( string text )
		//{
		//	var fromUser = users.ServerUser;
		//	if( fromUser == null )
		//		Log.Fatal( "ChatServerNetworkService: Say: Server user is not created." );
		//	SendText( fromUser, text, null );
		//}

		//public void SayPrivate( string text, ServerNetworkService_Users.UserInfo toUser )
		//{
		//	var fromUser = users.ServerUser;
		//	if( fromUser == null )
		//		Log.Fatal( "ChatServerNetworkService: Say: Server user is not created." );
		//	SendText( fromUser, text, toUser );
		//}

		void SendText( ServerNetworkService_Users.UserInfo fromUser, string text, ServerNetworkService_Users.UserInfo privateToUser )
		{
			ReceiveText?.Invoke( this, fromUser, text, null );

			if( privateToUser != null )
			{
				if( privateToUser.ConnectedNode != null )
					SendTextToClient( privateToUser, fromUser, text );
			}
			else
			{
				foreach( var toUser in users.Users )
				{
					if( toUser.ConnectedNode != null )
						SendTextToClient( toUser, fromUser, text );
				}
			}
		}

		void SendTextToClient( ServerNetworkService_Users.UserInfo toUser, ServerNetworkService_Users.UserInfo fromUser, string text )
		{
			var messageType = GetMessageType( "TextToClient" );
			var writer = BeginMessage( toUser.ConnectedNode, messageType );
			writer.Write( fromUser.UserID );
			//writer.WriteVariableUInt64( (ulong)fromUser.UserID );
			writer.Write( text );
			EndMessage();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_Chat : ClientNetworkService
	{
		ClientNetworkService_Users users;

		public const int MaxLastMessages = 200;
		LinkedList<LastMessage> lastMessages = new LinkedList<LastMessage>();
		//Queue<LastMessage> lastMessages = new Queue<LastMessage>();

		///////////////////////////////////////////

		public delegate void ReceiveTextDelegate( ClientNetworkService_Chat sender, ClientNetworkService_Users.UserInfo fromUser, string text );
		public event ReceiveTextDelegate ReceiveText;

		///////////////////////////////////////////

		public class LastMessage
		{
			ClientNetworkService_Users.UserInfo fromUser;
			string text;
			double time;

			//

			public LastMessage( ClientNetworkService_Users.UserInfo fromUser, string text, double time )
			{
				this.fromUser = fromUser;
				this.text = text;
				this.time = time;
			}

			public LastMessage()
			{
			}

			public ClientNetworkService_Users.UserInfo FromUser
			{
				get { return fromUser; }
			}

			public string Text
			{
				get { return text; }
			}

			public double Time
			{
				get { return time; }
			}
		}

		///////////////////////////////////////////

		public ClientNetworkService_Chat( ClientNetworkService_Users users )
			: base( "Chat", 3 )
		{
			this.users = users;

			//register message types
			RegisterMessageType( "TextToServer", 1 );
			RegisterMessageType( "TextToClient", 2, ReceiveMessage_TextToClient );
		}

		public void SayToEveryone( string text )
		{
			var messageType = GetMessageType( "TextToServer" );
			var writer = BeginMessage( messageType );
			writer.Write( text );
			writer.Write( (long)0 );
			//writer.WriteVariableUInt64( 0 );
			EndMessage();
		}

		public void SayPrivate( string text, ClientNetworkService_Users.UserInfo toUser )
		{
			var messageType = GetMessageType( "TextToServer" );
			var writer = BeginMessage( messageType );
			writer.Write( text );
			writer.Write( toUser.UserID );
			//!!!!везде где можно юзать Variable. но не везде можно
			//writer.WriteVariableUInt64( (ulong)toUser.UserID );
			EndMessage();
		}

		bool ReceiveMessage_TextToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var fromUserID = reader.ReadInt64();//var fromUserID = (long)reader.ReadVariableUInt64();
			var text = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//get user by identifier
			var fromUser = users.GetUser( fromUserID );
			if( fromUser == null )
			{
				//error. no such user.
				return true;
			}

			lastMessages.AddLast( new LastMessage( fromUser, text, EngineApp.EngineTime ) );
			if( lastMessages.Count > MaxLastMessages )
				lastMessages.RemoveFirst();
			//lastMessages.Enqueue( new LastMessage( fromUser, text ) );
			//if( lastMessages.Count > MaxLastMessages )
			//	lastMessages.Dequeue();

			ReceiveText?.Invoke( this, fromUser, text );

			return true;
		}

		public IReadOnlyCollection<LastMessage> LastMessages
		{
			get { return lastMessages; }
		}

		public LastMessage GetLastMessageFromUser( ClientNetworkService_Users.UserInfo fromUser )
		{
			foreach( var message in lastMessages.GetReverse() )
			{
				if( message.FromUser == fromUser )
					return message;
			}
			return null;
		}
	}
}