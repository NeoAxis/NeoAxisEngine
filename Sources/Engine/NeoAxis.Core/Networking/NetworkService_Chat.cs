// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NeoAxis.Networking;

namespace NeoAxis
{
	/// <summary>
	/// Built-in chat service.
	/// </summary>
	public class ServerNetworkService_Chat : ServerService
	{
		ServerNetworkService_Users usersService;
		bool allowPrivateMessages;

		MessageType thisUserAddedToRoomToClient;
		MessageType thisUserRemovingFromRoomToClient;
		MessageType anotherUserAddedToRoomToClient;
		MessageType anotherUserRemovingFromRoomToClient;
		MessageType sayInRoomToServer;
		MessageType sayPrivateToServer;
		MessageType roomMessageToClient;
		MessageType privateMessageToClient;

		long messageIdCounter;
		long roomIdCounter;

		List<Room> rooms = new List<Room>();

		EConcurrentQueue<PrivateMessage> privateMessages = new EConcurrentQueue<PrivateMessage>();

		EConcurrentQueue<object> receivedMessagesToProcess = new EConcurrentQueue<object>();
		volatile Thread receivedMessagesToProcessThread;

		///////////////////////////////////////////

		public delegate void ProcessingReceivedMessageDelegate( ServerNetworkService_Chat sender, object message, ref bool skip );
		public event ProcessingReceivedMessageDelegate ProcessingReceivedMessage;

		public delegate void SendingMessageToClientDelegate( ServerNetworkService_Chat sender, ServerNetworkService_Users.UserInfo sendTo, object message, ref bool skip );
		public event SendingMessageToClientDelegate SendingMessageToClient;

		///////////////////////////////////////////

		public class RoomMessage
		{
			public long Id;
			public DateTime Time;
			public Room Room;
			public ServerNetworkService_Users.UserInfo User;
			public string Text;
			public string Language;
			public string AnyData;
		}

		///////////////////////////////////////////

		public class PrivateMessage
		{
			public long Id;
			public DateTime Time;
			public ServerNetworkService_Users.UserInfo FromUser;
			public ServerNetworkService_Users.UserInfo ToUser;
			public string Text;
			public string Language;
			public string AnyData;
		}

		///////////////////////////////////////////////

		public class Room
		{
			public long Id;
			public string Name;

			volatile Dictionary<long, ServerNetworkService_Users.UserInfo> usersDictionary = new Dictionary<long, ServerNetworkService_Users.UserInfo>();
			volatile ServerNetworkService_Users.UserInfo[] usersArray;
			//EConcurrentDictionary
			//public EConcurrentDictionary<long, ServerNetworkService_Users.UserInfo> Users = new EDictionary<long, ServerNetworkService_Users.UserInfo>();

			public EConcurrentQueue<RoomMessage> Messages = new EConcurrentQueue<RoomMessage>();

			/////////////////////

			public void AddUser( ServerNetworkService_Users.UserInfo user )
			{
				lock( usersDictionary )
				{
					usersDictionary[ user.UserID ] = user;
					usersArray = null;
				}
			}

			public void RemoveUser( ServerNetworkService_Users.UserInfo user )
			{
				lock( usersDictionary )
				{
					usersDictionary.Remove( user.UserID );
					usersArray = null;
				}
			}

			public ServerNetworkService_Users.UserInfo[] GetUsers()
			{
				var users = usersArray;
				if( users == null )
				{
					lock( usersDictionary )
					{
						users = usersDictionary.Values.ToArray();
						usersArray = users;
					}
				}
				return users;
			}
		}

		///////////////////////////////////////////

		public ServerNetworkService_Chat( ServerNetworkService_Users usersService )
			: base( "Chat", 3 )
		{
			this.usersService = usersService;

			//register message types
			thisUserAddedToRoomToClient = RegisterMessageType( "ThisUserAddedToRoomToClient", 1 );
			thisUserRemovingFromRoomToClient = RegisterMessageType( "ThisUserRemovingFromRoomToClient", 2 );
			anotherUserAddedToRoomToClient = RegisterMessageType( "AnotherUserAddedToRoomToClient", 3 );
			anotherUserRemovingFromRoomToClient = RegisterMessageType( "AnotherUserRemovingFromRoomToClient", 4 );
			sayInRoomToServer = RegisterMessageType( "SayInRoomToServer", 5, ReceiveMessage_SayInRoomToServer );
			sayPrivateToServer = RegisterMessageType( "SayPrivateToServer", 6, ReceiveMessage_SayPrivateToServer );
			roomMessageToClient = RegisterMessageType( "RoomMessageToClient", 7 );
			privateMessageToClient = RegisterMessageType( "PrivateMessageToClient", 8 );
		}

		public ServerNetworkService_Users UsersService
		{
			get { return usersService; }
		}

		public bool AllowPrivateMessages
		{
			get { return allowPrivateMessages; }
			set { allowPrivateMessages = value; }
		}

		void ReceivedMessagesToProcessThreadFunction()
		{
			while( !Owner.Disposed )
			{
				while( receivedMessagesToProcess.TryDequeue( out var message ) )
				{
					var skip = false;
					ProcessingReceivedMessage?.Invoke( this, message, ref skip );
					if( !skip )
					{
						var roomMessage = message as RoomMessage;
						if( roomMessage != null )
						{
							AddRoomMessage( roomMessage );
							continue;
						}

						var privateMessage = message as PrivateMessage;
						if( privateMessage != null )
						{
							AddPrivateMessage( privateMessage );
							continue;
						}
					}
				}

				Thread.Sleep( 1 );
			}
		}

		void CreateReceivedMessagesToProcessThread()
		{
			if( receivedMessagesToProcessThread == null )
			{
				var newThread = new Thread( ReceivedMessagesToProcessThreadFunction );
				newThread.IsBackground = true;
				if( Interlocked.CompareExchange( ref receivedMessagesToProcessThread, newThread, null ) == null )
					newThread.Start();
			}
		}

		bool ReceiveMessage_SayInRoomToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get source user
			var user = usersService.GetUser( sender );

			//get data of message
			var roomId = reader.ReadVariableInt64();
			var text = reader.ReadString() ?? string.Empty;
			var language = reader.ReadString();
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//!!!!check max sizes


			var room = GetRoom( roomId );
			if( room != null )
			{
				var message = new RoomMessage();
				message.Id = Interlocked.Increment( ref messageIdCounter );
				message.Time = DateTime.UtcNow;
				message.Room = room;
				message.User = user;
				message.Text = text;
				message.Language = language;
				message.AnyData = anyData;


				//!!!!max message count

				CreateReceivedMessagesToProcessThread();
				receivedMessagesToProcess.Enqueue( message );
			}

			return true;
		}

		bool ReceiveMessage_SayPrivateToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get source user
			var user = usersService.GetUser( sender );

			//get data of message
			var toUserId = reader.ReadVariableInt64();
			var text = reader.ReadString() ?? string.Empty;
			var language = reader.ReadString();
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//!!!!check max sizes


			if( AllowPrivateMessages )
			{
				var toUser = usersService.GetUser( toUserId );
				if( toUser != null )
				{
					var message = new PrivateMessage();
					message.Id = Interlocked.Increment( ref messageIdCounter );
					message.Time = DateTime.UtcNow;
					message.FromUser = user;
					message.ToUser = toUser;
					message.Text = text;
					message.Language = language;
					message.AnyData = anyData;


					//!!!!max message count

					CreateReceivedMessagesToProcessThread();
					receivedMessagesToProcess.Enqueue( message );
				}
			}

			return true;
		}

		public Room CreateRoom( string name )
		{
			var room = new Room();
			room.Id = Interlocked.Increment( ref roomIdCounter );
			room.Name = name;
			lock( rooms )
				rooms.Add( room );
			return room;
		}

		public void DeleteRoom( Room room )
		{
			foreach( var user in room.GetUsers() )
				RemoveUserFromRoom( room, user );

			lock( rooms )
				rooms.Remove( room );
		}

		public Room GetRoom( long id )
		{
			//slowly? dictionary?
			lock( rooms )
				return rooms.FirstOrDefault( r => r.Id == id );
		}

		public Room GetRoom( string name )
		{
			//slowly? dictionary?
			lock( rooms )
				return rooms.FirstOrDefault( r => r.Name == name );
		}

		public Room[] GetRooms()
		{
			lock( rooms )
				return rooms.ToArray();
		}


		void SendRoomMessageToClient( ServerNetworkService_Users.UserInfo sendTo, RoomMessage message )
		{
			var m = BeginMessage( sendTo.Client, roomMessageToClient );
			m.Writer.WriteVariable( message.Id );
			m.Writer.Write( message.Time.Ticks );
			m.Writer.WriteVariable( message.Room.Id );
			m.Writer.WriteVariable( message.User.UserID );
			m.Writer.Write( message.Text );
			m.Writer.Write( message.Language );
			m.Writer.Write( message.AnyData );
			m.End();
		}

		void AddRoomMessage( RoomMessage message )
		{
			var room = message.Room;
			room.Messages.Enqueue( message );

			//send to clients
			var users = room.GetUsers();
			foreach( var user in users )
			{
				var skip = false;
				SendingMessageToClient?.Invoke( this, user, message, ref skip );
				if( !skip )
					SendRoomMessageToClient( user, message );
			}
		}

		void SendPrivateMessageToClient( ServerNetworkService_Users.UserInfo sendTo, PrivateMessage message )
		{
			var m = BeginMessage( sendTo.Client, privateMessageToClient );
			m.Writer.WriteVariable( message.Id );
			m.Writer.Write( message.Time.Ticks );
			m.Writer.WriteVariable( message.FromUser.UserID );
			m.Writer.WriteVariable( message.ToUser.UserID );
			m.Writer.Write( message.Text );
			m.Writer.Write( message.Language );
			m.Writer.Write( message.AnyData );
			m.End();
		}

		void AddPrivateMessage( PrivateMessage message )
		{
			privateMessages.Enqueue( message );

			//send to both users
			{
				var skip = false;
				SendingMessageToClient?.Invoke( this, message.FromUser, message, ref skip );
				if( !skip )
					SendPrivateMessageToClient( message.FromUser, message );
			}
			{
				var skip = false;
				SendingMessageToClient?.Invoke( this, message.ToUser, message, ref skip );
				if( !skip )
					SendPrivateMessageToClient( message.ToUser, message );
			}
		}


		public void AddUserToRoom( Room room, ServerNetworkService_Users.UserInfo user )
		{
			room.AddUser( user );

			var users = room.GetUsers();
			foreach( var user2 in users )
			{
				if( user == user2 )
				{
					//send info to this user about adding to the room
					var m = BeginMessage( user2.Client, thisUserAddedToRoomToClient );
					m.Writer.WriteVariable( room.Id );
					m.Writer.Write( room.Name );
					m.Writer.WriteVariable( users.Length );
					foreach( var user3 in users )
						m.Writer.WriteVariable( user3.UserID );
					m.End();
				}
				else
				{
					//send info to room users about the new user
					var m = BeginMessage( user2.Client, anotherUserAddedToRoomToClient );
					m.Writer.WriteVariable( room.Id );
					m.Writer.WriteVariable( user.UserID );
					m.End();
				}
			}
		}

		public void RemoveUserFromRoom( Room room, ServerNetworkService_Users.UserInfo user )
		{
			//send to room users
			foreach( var user2 in room.GetUsers() )
			{
				if( user == user2 )
				{
					//send info to this user
					var m = BeginMessage( user2.Client, thisUserRemovingFromRoomToClient );
					m.Writer.WriteVariable( room.Id );
					m.Writer.WriteVariable( user.UserID );
					m.End();
				}
				else
				{
					//send info to room users
					var m = BeginMessage( user2.Client, anotherUserRemovingFromRoomToClient );
					m.Writer.WriteVariable( room.Id );
					m.Writer.WriteVariable( user.UserID );
					m.End();
				}
			}

			room.RemoveUser( user );
		}

		public void RemoveUser( ServerNetworkService_Users.UserInfo user )
		{
			foreach( var room in GetRooms() )
				RemoveUserFromRoom( room, user );
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_Chat : ClientService
	{
		ClientNetworkService_Users usersService;

		MessageType thisUserAddedToRoomToClient;
		MessageType thisUserRemovingFromRoomToClient;
		MessageType anotherUserAddedToRoomToClient;
		MessageType anotherUserRemovingFromRoomToClient;
		MessageType sayInRoomToServer;
		MessageType sayPrivateToServer;
		MessageType roomMessageToClient;
		MessageType privateMessageToClient;

		public int MaxMessagesInRoom { get; set; } = 200;
		public int MaxPrivateMessages { get; set; } = 200;

		List<Room> rooms = new List<Room>();
		EConcurrentQueue<PrivateMessage> privateMessages = new EConcurrentQueue<PrivateMessage>();

		///////////////////////////////////////////

		public delegate void ThisUserAddedToRoomDelegate( ClientNetworkService_Chat sender, Room room );
		public event ThisUserAddedToRoomDelegate ThisUserAddedToRoom;

		public delegate void ThisUserRemovingFromRoomDelegate( ClientNetworkService_Chat sender, Room room );
		public event ThisUserRemovingFromRoomDelegate ThisUserRemovingFromRoom;

		public delegate void ThisUserRemovedFromRoomDelegate( ClientNetworkService_Chat sender, long roomID );
		public event ThisUserRemovedFromRoomDelegate ThisUserRemovedFromRoom;


		public delegate void AnotherUserAddedToRoomDelegate( ClientNetworkService_Chat sender, Room room, long userID );// ClientNetworkService_Users.UserInfo user );
		public event AnotherUserAddedToRoomDelegate AnotherUserAddedToRoom;

		public delegate void AnotherUserRemovedFromRoomDelegate( ClientNetworkService_Chat sender, Room room, long userID );// ClientNetworkService_Users.UserInfo user );
		public event AnotherUserRemovedFromRoomDelegate AnotherUserRemovedFromRoom;


		public delegate void ReceivedRoomMessageDelegate( ClientNetworkService_Chat sender, RoomMessage message );
		public event ReceivedRoomMessageDelegate ReceivedRoomMessage;

		public delegate void ReceivedPrivateMessageDelegate( ClientNetworkService_Chat sender, PrivateMessage message );
		public event ReceivedPrivateMessageDelegate ReceivedPrivateMessage;

		///////////////////////////////////////////

		public class RoomMessage
		{
			public long Id;
			public DateTime Time;
			public Room Room;
			public long UserID;
			//public ClientNetworkService_Users.UserInfo User;
			public string Text;
			public string Language;
			public string AnyData;

			public double ReceivedEngineTime;
		}

		///////////////////////////////////////////

		public class PrivateMessage
		{
			public long Id;
			public DateTime Time;
			public long FromUserID;
			public long ToUserID;
			//public ServerNetworkService_Users.UserInfo FromUser;
			//public ServerNetworkService_Users.UserInfo ToUser;
			public string Text;
			public string Language;
			public string AnyData;

			public double ReceivedEngineTime;
		}

		///////////////////////////////////////////

		public class Room
		{
			public long Id;
			public string Name;

			volatile ESet<long> usersSet = new ESet<long>();
			volatile long[] usersArray;
			//volatile Dictionary<long, ClientNetworkService_Users.UserInfo> usersDictionary = new Dictionary<long, ClientNetworkService_Users.UserInfo>();
			//volatile ClientNetworkService_Users.UserInfo[] usersArray;

			public EConcurrentQueue<RoomMessage> Messages = new EConcurrentQueue<RoomMessage>();
			volatile internal RoomMessage[] messagesArray;

			/////////////////////

			public void AddUser( long userID )
			{
				lock( usersSet )
				{
					usersSet.AddWithCheckAlreadyContained( userID );
					usersArray = null;
				}
			}

			public bool RemoveUser( long userID )
			{
				lock( usersSet )
				{
					var removed = usersSet.Remove( userID );
					usersArray = null;
					return removed;
				}
			}

			public long[] GetUsers()
			{
				var users = usersArray;
				if( users == null )
				{
					lock( usersSet )
					{
						users = usersSet.ToArray();
						usersArray = users;
					}
				}
				return users;
			}

			public RoomMessage[] MessagesArray
			{
				get
				{
					var result = messagesArray;
					if( result == null )
					{
						result = Messages.ToArray();
						messagesArray = result;
					}
					return result;
				}
			}

			//public void AddUser( ClientNetworkService_Users.UserInfo user )
			//{
			//	lock( usersSet )
			//	{
			//		usersSet[ user.UserID ] = user;
			//		usersArray = null;
			//	}
			//}

			//public void RemoveUser( ClientNetworkService_Users.UserInfo user )
			//{
			//	lock( usersSet )
			//	{
			//		usersSet.Remove( user.UserID );
			//		usersArray = null;
			//	}
			//}

			//public ClientNetworkService_Users.UserInfo[] GetUsers()
			//{
			//	var users = usersArray;
			//	if( users == null )
			//	{
			//		lock( usersSet )
			//		{
			//			users = usersSet.Values.ToArray();
			//			usersArray = users;
			//		}
			//	}
			//	return users;
			//}
		}

		///////////////////////////////////////////

		public ClientNetworkService_Chat( ClientNetworkService_Users usersService )
			: base( "Chat", 3 )
		{
			this.usersService = usersService;

			//register message types
			thisUserAddedToRoomToClient = RegisterMessageType( "ThisUserAddedToRoomToClient", 1, ReceiveMessage_ThisUserAddedToRoomToClient );
			thisUserRemovingFromRoomToClient = RegisterMessageType( "ThisUserRemovingFromRoomToClient", 2, ReceiveMessage_ThisUserRemovingFromRoomToClient );
			anotherUserAddedToRoomToClient = RegisterMessageType( "AnotherUserAddedToRoomToClient", 3, ReceiveMessage_AnotherUserAddedToRoomToClient );
			anotherUserRemovingFromRoomToClient = RegisterMessageType( "AnotherUserRemovingFromRoomToClient", 4, ReceiveMessage_AnotherUserRemovingFromRoomToClient );
			sayInRoomToServer = RegisterMessageType( "SayInRoomToServer", 5 );
			sayPrivateToServer = RegisterMessageType( "SayPrivateToServer", 6 );
			roomMessageToClient = RegisterMessageType( "RoomMessageToClient", 7, ReceiveMessage_RoomMessageToClient );
			privateMessageToClient = RegisterMessageType( "PrivateMessageToClient", 8, ReceiveMessage_PrivateMessageToClient );
		}

		public ClientNetworkService_Users UsersService
		{
			get { return usersService; }
		}

		public Room GetRoom( long id )
		{
			//slowly? dictionary?
			lock( rooms )
				return rooms.FirstOrDefault( r => r.Id == id );
		}

		public Room GetRoom( string name )
		{
			//slowly? dictionary?
			lock( rooms )
				return rooms.FirstOrDefault( r => r.Name == name );
		}

		public Room[] GetRooms()
		{
			lock( rooms )
				return rooms.ToArray();
		}

		bool ReceiveMessage_ThisUserAddedToRoomToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//send previous messages too?

			//get data from message
			var roomID = reader.ReadVariableInt64();
			var roomName = reader.ReadString() ?? string.Empty;
			var userCount = reader.ReadVariableInt32();
			var userIDs = new List<long>( userCount );
			for( int n = 0; n < userCount; n++ )
				userIDs.Add( reader.ReadVariableInt64() );
			if( !reader.Complete() )
				return false;

			var room = new Room();
			room.Id = roomID;
			room.Name = roomName;
			foreach( var userID in userIDs )
				room.AddUser( userID );
			lock( rooms )
				rooms.Add( room );

			ThisUserAddedToRoom?.Invoke( this, room );

			return true;
		}

		bool ReceiveMessage_ThisUserRemovingFromRoomToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var roomID = reader.ReadVariableInt64();
			var userID = reader.ReadVariableInt64();
			if( !reader.Complete() )
				return false;

			var room = GetRoom( roomID );
			if( room != null )
			{
				ThisUserRemovingFromRoom?.Invoke( this, room );

				lock( rooms )
					rooms.Remove( room );

				ThisUserRemovedFromRoom?.Invoke( this, room.Id );
			}

			return true;
		}

		bool ReceiveMessage_AnotherUserAddedToRoomToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var roomID = reader.ReadVariableInt64();
			var userID = reader.ReadVariableInt64();
			if( !reader.Complete() )
				return false;

			var room = GetRoom( roomID );
			if( room != null )
			{
				room.AddUser( userID );
				AnotherUserAddedToRoom?.Invoke( this, room, userID );
			}

			return true;
		}

		bool ReceiveMessage_AnotherUserRemovingFromRoomToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var roomID = reader.ReadVariableInt64();
			var userID = reader.ReadVariableInt64();
			if( !reader.Complete() )
				return false;

			var room = GetRoom( roomID );
			if( room != null )
			{
				if( room.RemoveUser( userID ) )
					AnotherUserRemovedFromRoom?.Invoke( this, room, userID );
			}

			return true;
		}

		bool ReceiveMessage_RoomMessageToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var messageId = reader.ReadVariableInt64();
			var time = new DateTime( reader.ReadInt64(), DateTimeKind.Utc );
			var roomID = reader.ReadVariableInt64();
			var userID = reader.ReadVariableInt64();
			var text = reader.ReadString() ?? string.Empty;
			var language = reader.ReadString();
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//process
			var room = GetRoom( roomID );
			if( room != null )
			{
				var message = new RoomMessage();
				message.Id = messageId;
				message.Time = time;
				message.Room = room;
				message.UserID = userID;
				//message.User = usersService.GetUser( userID );
				message.Text = text;
				message.Language = language;
				message.AnyData = anyData;
				message.ReceivedEngineTime = EngineApp.EngineTime;

				while( room.Messages.Count >= MaxMessagesInRoom )
					room.Messages.TryDequeue( out _ );
				room.Messages.Enqueue( message );
				room.messagesArray = null;

				ReceivedRoomMessage?.Invoke( this, message );
			}

			return true;
		}

		bool ReceiveMessage_PrivateMessageToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var messageId = reader.ReadVariableInt64();
			var time = new DateTime( reader.ReadInt64(), DateTimeKind.Utc );
			var fromUserID = reader.ReadVariableInt64();
			var toUserID = reader.ReadVariableInt64();
			var text = reader.ReadString() ?? string.Empty;
			var language = reader.ReadString();
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			var message = new PrivateMessage();
			message.Id = messageId;
			message.Time = time;
			message.FromUserID = fromUserID;
			message.ToUserID = toUserID;
			message.Text = text;
			message.Language = language;
			message.AnyData = anyData;

			while( privateMessages.Count >= MaxPrivateMessages )
				privateMessages.TryDequeue( out _ );
			privateMessages.Enqueue( message );

			ReceivedPrivateMessage?.Invoke( this, message );

			return true;
		}


		public void SayInRoom( Room room, string text, string language = null, string anyData = null )
		{
			var m = BeginMessage( sayInRoomToServer );
			m.Writer.WriteVariable( room.Id );
			m.Writer.Write( text );
			m.Writer.Write( language );
			m.Writer.Write( anyData );
			m.End();
		}

		public void SayPrivate( ClientNetworkService_Users.UserInfo user, string text, string language = null, string anyData = null )
		{
			var m = BeginMessage( sayPrivateToServer );
			m.Writer.WriteVariable( user.UserID );
			m.Writer.Write( text );
			m.Writer.Write( language );
			m.Writer.Write( anyData );
			m.End();
		}

		public RoomMessage GetLastRoomMessageFromUser( Room room, long userID ) //ClientNetworkService_Users.UserInfo user )
		{
			var messages = room.MessagesArray;

			for( int n = messages.Length - 1; n >= 0; n-- )
			{
				var message = messages[ n ];
				if( message.UserID == userID )
					return message;
			}

			return null;
		}
	}
}


//	//bool ReceiveMessage_TextToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//	//{
//	//	//get source user
//	//	var fromUser = usersService.GetUser( sender );

//	//	//get data of message
//	//	var text = reader.ReadString();
//	//	var privateToUserID = reader.ReadInt64();
//	//	//long privateToUserID = (long)reader.ReadVariableUInt64();
//	//	if( !reader.Complete() )
//	//		return false;

//	//	//send text to the clients
//	//	if( privateToUserID != 0 )
//	//	{
//	//		//send text to the specific user

//	//		var privateToUser = usersService.GetUser( privateToUserID );
//	//		if( privateToUser != null )
//	//		{
//	//			SendText( fromUser, text, privateToUser );
//	//		}
//	//		else
//	//		{
//	//			//no user anymore
//	//		}
//	//	}
//	//	else
//	//	{
//	//		SendText( fromUser, text, null );
//	//	}

//	//	return true;
//	//}


//	////public void SayToAll( string text )
//	////{
//	////	var fromUser = users.ServerUser;
//	////	if( fromUser == null )
//	////		Log.Fatal( "ChatServerNetworkService: Say: Server user is not created." );
//	////	SendText( fromUser, text, null );
//	////}

//	////public void SayPrivate( string text, ServerNetworkService_Users.UserInfo toUser )
//	////{
//	////	var fromUser = users.ServerUser;
//	////	if( fromUser == null )
//	////		Log.Fatal( "ChatServerNetworkService: Say: Server user is not created." );
//	////	SendText( fromUser, text, toUser );
//	////}

//	//void SendText( ServerNetworkService_Users.UserInfo fromUser, string text, ServerNetworkService_Users.UserInfo privateToUser )
//	//{
//	//	ReceiveText?.Invoke( this, fromUser, text, null );

//	//	if( privateToUser != null )
//	//	{
//	//		if( privateToUser.Client != null )
//	//			SendTextToClient( privateToUser, fromUser, text );
//	//	}
//	//	else
//	//	{
//	//		foreach( var toUser in usersService.Users )
//	//		{
//	//			if( toUser.Client != null )
//	//				SendTextToClient( toUser, fromUser, text );
//	//		}
//	//	}
//	//}

//	//void SendTextToClient( ServerNetworkService_Users.UserInfo toUser, ServerNetworkService_Users.UserInfo fromUser, string text )
//	//{
//	//	var messageType = GetMessageType( "TextToClient" );
//	//	var m = BeginMessage( toUser.Client, messageType );
//	//	m.Writer.Write( fromUser.UserID );
//	//	//writer.WriteVariableUInt64( (ulong)fromUser.UserID );
//	//	m.Writer.Write( text );
//	//	m.End();
//	//}



//	//public class ServerNetworkService_Chat : ServerService
//	//{
//	//	ServerNetworkService_Users users;

//	//	///////////////////////////////////////////

//	//	public delegate void ReceiveTextDelegate( ServerNetworkService_Chat sender, ServerNetworkService_Users.UserInfo fromUser, string text, ServerNetworkService_Users.UserInfo privateToUser );
//	//	public event ReceiveTextDelegate ReceiveText;

//	//	///////////////////////////////////////////

//	//	public ServerNetworkService_Chat( ServerNetworkService_Users users )
//	//		: base( "Chat", 3 )
//	//	{
//	//		this.users = users;

//	//		//register message types
//	//		RegisterMessageType( "TextToServer", 1, ReceiveMessage_TextToServer );
//	//		RegisterMessageType( "TextToClient", 2 );
//	//	}

//	//	bool ReceiveMessage_TextToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//	//	{
//	//		//get source user
//	//		var fromUser = users.GetUser( sender );

//	//		//get data of message
//	//		var text = reader.ReadString();
//	//		var privateToUserID = reader.ReadInt64();
//	//		//long privateToUserID = (long)reader.ReadVariableUInt64();
//	//		if( !reader.Complete() )
//	//			return false;

//	//		//send text to the clients
//	//		if( privateToUserID != 0 )
//	//		{
//	//			//send text to the specific user

//	//			var privateToUser = users.GetUser( privateToUserID );
//	//			if( privateToUser != null )
//	//			{
//	//				SendText( fromUser, text, privateToUser );
//	//			}
//	//			else
//	//			{
//	//				//no user anymore
//	//			}
//	//		}
//	//		else
//	//		{
//	//			SendText( fromUser, text, null );
//	//		}

//	//		return true;
//	//	}

//	//	//public void SayToAll( string text )
//	//	//{
//	//	//	var fromUser = users.ServerUser;
//	//	//	if( fromUser == null )
//	//	//		Log.Fatal( "ChatServerNetworkService: Say: Server user is not created." );
//	//	//	SendText( fromUser, text, null );
//	//	//}

//	//	//public void SayPrivate( string text, ServerNetworkService_Users.UserInfo toUser )
//	//	//{
//	//	//	var fromUser = users.ServerUser;
//	//	//	if( fromUser == null )
//	//	//		Log.Fatal( "ChatServerNetworkService: Say: Server user is not created." );
//	//	//	SendText( fromUser, text, toUser );
//	//	//}

//	//	void SendText( ServerNetworkService_Users.UserInfo fromUser, string text, ServerNetworkService_Users.UserInfo privateToUser )
//	//	{
//	//		ReceiveText?.Invoke( this, fromUser, text, null );

//	//		if( privateToUser != null )
//	//		{
//	//			if( privateToUser.Client != null )
//	//				SendTextToClient( privateToUser, fromUser, text );
//	//		}
//	//		else
//	//		{
//	//			foreach( var toUser in users.Users )
//	//			{
//	//				if( toUser.Client != null )
//	//					SendTextToClient( toUser, fromUser, text );
//	//			}
//	//		}
//	//	}

//	//	void SendTextToClient( ServerNetworkService_Users.UserInfo toUser, ServerNetworkService_Users.UserInfo fromUser, string text )
//	//	{
//	//		var messageType = GetMessageType( "TextToClient" );
//	//		var m = BeginMessage( toUser.Client, messageType );
//	//		m.Writer.Write( fromUser.UserID );
//	//		//writer.WriteVariableUInt64( (ulong)fromUser.UserID );
//	//		m.Writer.Write( text );
//	//		m.End();
//	//	}
//	//}

//	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//	//public class ClientNetworkService_Chat : ClientService
//	//{
//	//	ClientNetworkService_Users users;

//	//	public const int MaxLastMessages = 200;
//	//	LinkedList<LastMessage> lastMessages = new LinkedList<LastMessage>();
//	//	//Queue<LastMessage> lastMessages = new Queue<LastMessage>();

//	//	///////////////////////////////////////////

//	//	public delegate void ReceiveTextDelegate( ClientNetworkService_Chat sender, ClientNetworkService_Users.UserInfo fromUser, string text );
//	//	public event ReceiveTextDelegate ReceiveText;

//	//	///////////////////////////////////////////

//	//	public class LastMessage
//	//	{
//	//		ClientNetworkService_Users.UserInfo fromUser;
//	//		string text;
//	//		double time;

//	//		//

//	//		public LastMessage( ClientNetworkService_Users.UserInfo fromUser, string text, double time )
//	//		{
//	//			this.fromUser = fromUser;
//	//			this.text = text;
//	//			this.time = time;
//	//		}

//	//		public LastMessage()
//	//		{
//	//		}

//	//		public ClientNetworkService_Users.UserInfo FromUser
//	//		{
//	//			get { return fromUser; }
//	//		}

//	//		public string Text
//	//		{
//	//			get { return text; }
//	//		}

//	//		public double Time
//	//		{
//	//			get { return time; }
//	//		}
//	//	}

//	//	///////////////////////////////////////////

//	//	public ClientNetworkService_Chat( ClientNetworkService_Users users )
//	//		: base( "Chat", 3 )
//	//	{
//	//		this.users = users;

//	//		//register message types
//	//		RegisterMessageType( "TextToServer", 1 );
//	//		RegisterMessageType( "TextToClient", 2, ReceiveMessage_TextToClient );
//	//	}

//	//	public void SayToEveryone( string text )
//	//	{
//	//		var messageType = GetMessageType( "TextToServer" );
//	//		var m = BeginMessage( messageType );
//	//		m.Writer.Write( text );
//	//		m.Writer.Write( (long)0 );
//	//		//writer.WriteVariableUInt64( 0 );
//	//		m.End();
//	//	}

//	//	public void SayPrivate( string text, ClientNetworkService_Users.UserInfo toUser )
//	//	{
//	//		var messageType = GetMessageType( "TextToServer" );
//	//		var m = BeginMessage( messageType );
//	//		m.Writer.Write( text );
//	//		m.Writer.Write( toUser.UserID );
//	//		//!!!!везде где можно юзать Variable. но не везде можно
//	//		//writer.WriteVariableUInt64( (ulong)toUser.UserID );
//	//		m.End();
//	//	}

//	//	bool ReceiveMessage_TextToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//	//	{
//	//		//get data from message
//	//		var fromUserID = reader.ReadInt64();//var fromUserID = (long)reader.ReadVariableUInt64();
//	//		var text = reader.ReadString();
//	//		if( !reader.Complete() )
//	//			return false;

//	//		//get user by identifier
//	//		var fromUser = users.GetUser( fromUserID );
//	//		if( fromUser == null )
//	//		{
//	//			//error. no such user.
//	//			return true;
//	//		}

//	//		lastMessages.AddLast( new LastMessage( fromUser, text, EngineApp.EngineTime ) );
//	//		if( lastMessages.Count > MaxLastMessages )
//	//			lastMessages.RemoveFirst();
//	//		//lastMessages.Enqueue( new LastMessage( fromUser, text ) );
//	//		//if( lastMessages.Count > MaxLastMessages )
//	//		//	lastMessages.Dequeue();

//	//		ReceiveText?.Invoke( this, fromUser, text );

//	//		return true;
//	//	}

//	//	public IReadOnlyCollection<LastMessage> LastMessages
//	//	{
//	//		get { return lastMessages; }
//	//	}

//	//	public LastMessage GetLastMessageFromUser( ClientNetworkService_Users.UserInfo fromUser )
//	//	{
//	//		foreach( var message in lastMessages.GetReverse() )
//	//		{
//	//			if( message.FromUser == fromUser )
//	//				return message;
//	//		}
//	//		return null;
//	//	}
//	//}
//}