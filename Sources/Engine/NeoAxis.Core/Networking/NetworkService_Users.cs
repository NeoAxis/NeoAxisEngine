// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using NeoAxis.Networking;
using System.Collections.Concurrent;
using System.Threading;

namespace NeoAxis
{
	public class ServerNetworkService_Users : ServerService
	{
		MessageType addUserToClient;
		MessageType removeUserToClient;
		MessageType updateObjectControlledByPlayerToClient;

		ConcurrentDictionary<long, UserInfo> usersByID = new ConcurrentDictionary<long, UserInfo>();
		ConcurrentDictionary<ServerNode.Client, UserInfo> usersByClient = new ConcurrentDictionary<ServerNode.Client, UserInfo>();
		//UserInfo serverUser;

		long botIDCounter = 10000000000L;
		long botUniqueNameCounter = 1;
		long directConnectionUserCounter = 20000000000L;

		///////////////////////////////////////////

		public class UserInfo
		{
			ServerNode.Client client;
			long botUserID;
			string botUsername = "";

			//

			internal UserInfo( ServerNode.Client client )
			{
				this.client = client;
			}

			internal UserInfo( long botUserID, string botUsername )
			{
				this.botUserID = botUserID;
				this.botUsername = botUsername;
			}

			public long UserID
			{
				get { return client != null ? client.LoginDataUserID : botUserID; }
			}

			public string Username
			{
				get { return client != null ? client.LoginDataUsername : botUsername; }
			}

			public ServerNode.Client Client
			{
				get { return client; }
			}

			public override string ToString()
			{
				if( client != null )
					return $"{Username} ({client.GetAddressText()})";
				else
					return Username;
			}

			public bool Bot
			{
				get { return client == null; }
			}

			//custom data
			public object AnyData { get; set; }
			public string DirectServerAvatar { get; set; } = "";
			public string ReferenceToObjectControlledByPlayer { get; set; } = "";
		}

		///////////////////////////////////////////

		public delegate void AddRemoveUserDelegate( ServerNetworkService_Users sender, UserInfo user );
		public event AddRemoveUserDelegate UserAdded;
		public event AddRemoveUserDelegate UserRemoved;

		//public delegate void UpdateUserDelegate( ServerNetworkService_Users sender, UserInfo user, ref string name );
		//public event UpdateUserDelegate UpdateUserEvent;

		public delegate void GetShareUserWithAnotherEventDelegate( ServerNetworkService_Users sender, UserInfo tellToThisUser, UserInfo aboutThisUser, ref bool share );
		public event GetShareUserWithAnotherEventDelegate GetShareUserWithAnotherEvent;

		///////////////////////////////////////////

		public ServerNetworkService_Users()
			: base( "Users", 2 )
		{
			//register message types
			addUserToClient = RegisterMessageType( "AddUserToClient", 1 );
			removeUserToClient = RegisterMessageType( "RemoveUserToClient", 2 );
			updateObjectControlledByPlayerToClient = RegisterMessageType( "UpdateObjectControlledByPlayerToClient", 3 );
			//RegisterMessageType( "UpdateUserToClient", 3 );
		}

		protected override void OnDispose()
		{
			while( usersByID.Count != 0 )
			{
				var enumerator = usersByID.GetEnumerator();
				enumerator.MoveNext();
				RemoveUser( enumerator.Current.Value );
			}

			base.OnDispose();
		}

		public ICollection<UserInfo> Users
		{
			get { return usersByID.Values; }
		}

		public UserInfo GetUser( long userID )
		{
			if( usersByID.TryGetValue( userID, out var user ) )
				return user;
			return null;
		}

		public UserInfo GetUser( ServerNode.Client client )
		{
			if( usersByClient.TryGetValue( client, out var user ) )
				return user;
			return null;
		}

		//public UserInfo ServerUser
		//{
		//	get { return serverUser; }
		//}

		public bool GetShareUserWithAnother( UserInfo tellToThisUser, UserInfo aboutThisUser )
		{
			var share = true;
			GetShareUserWithAnotherEvent?.Invoke( this, tellToThisUser, aboutThisUser, ref share );
			return share;
		}

		public UserInfo AddUser( ServerNode.Client client )
		{
			if( GetUser( client ) != null )
				Log.Fatal( "ServerNetworkService_Users: AddUser: GetUser( client ) != null." );

			var newUser = new UserInfo( client );

			usersByID[ newUser.UserID ] = newUser;
			if( newUser.Client != null )
				usersByClient[ newUser.Client ] = newUser;

			//send event about new user to the all users
			foreach( var user in Users )
			{
				if( user.Client != null && ( user == newUser || GetShareUserWithAnother( user, newUser ) ) )
				{
					bool thisUserFlag = user == newUser;

					var m = BeginMessage( user.Client, addUserToClient );
					m.Writer.WriteVariableInt64( newUser.UserID ); //m.Writer.Write( newUser.UserID );
					m.Writer.Write( newUser.Username );
					m.Writer.Write( newUser.Bot );
					m.Writer.Write( thisUserFlag );
					//custom data
					m.Writer.Write( newUser.ReferenceToObjectControlledByPlayer );
					m.End();
				}
			}

			if( newUser.Client != null )
			{
				//send list of users to new user
				foreach( var user in Users )
				{
					if( user != newUser && GetShareUserWithAnother( newUser, user ) )
					{
						var m = BeginMessage( newUser.Client, addUserToClient );
						m.Writer.WriteVariableInt64( user.UserID );//m.Writer.Write( user.UserID );
						m.Writer.Write( user.Username );
						m.Writer.Write( user.Bot );
						//this user flag
						m.Writer.Write( false );
						//custom data
						m.Writer.Write( user.ReferenceToObjectControlledByPlayer );
						m.End();
					}
				}
			}

			UserAdded?.Invoke( this, newUser );

			return newUser;
		}

		public UserInfo AddUserBot( string username = "", object anyData = null )
		{
			var username2 = username;
			if( string.IsNullOrEmpty( username2 ) )
			{
				username2 = "Bot" + botUniqueNameCounter.ToString();
				botUniqueNameCounter++;
			}

			var userID = Interlocked.Increment( ref botIDCounter );
			//var userID = botIDCounter;
			//botIDCounter++;

			var newUser = new UserInfo( userID, username2 );
			newUser.AnyData = anyData;

			usersByID[ newUser.UserID ] = newUser;

			//send event about new user to the all users
			foreach( var user in Users )
			{
				if( user.Client != null && ( user == newUser || GetShareUserWithAnother( user, newUser ) ) )
				{
					bool thisUserFlag = user == newUser;

					var m = BeginMessage( user.Client, addUserToClient );
					m.Writer.WriteVariableInt64( newUser.UserID );//m.Writer.Write( newUser.UserID );
					m.Writer.Write( newUser.Username );
					m.Writer.Write( newUser.Bot );
					m.Writer.Write( thisUserFlag );
					m.End();
				}
			}

			UserAdded?.Invoke( this, newUser );

			return newUser;
		}

		//public UserInfo CreateClientUser( NetworkNode.ConnectedNode connectedNode )
		//{
		//	return CreateUser( connectedNode.LoginName, connectedNode );
		//}

		//public UserInfo CreateServerUser( string name )
		//{
		//	if( serverUser != null )
		//		Log.Fatal( "UserManagementServerNetworkService: CreateServerUser: Server user is already created." );

		//	serverUser = CreateUser( name, null );
		//	return serverUser;
		//}

		public void RemoveUser( UserInfo user )
		{
			//check already removed
			if( !usersByID.ContainsKey( user.UserID ) )
				return;

			//moved down
			//UserRemoved?.Invoke( this, user );

			//remove user
			usersByID.TryRemove( user.UserID, out _ );
			if( user.Client != null )
				usersByClient.TryRemove( user.Client, out _ );
			//if( serverUser == user )
			//	serverUser = null;

			UserRemoved?.Invoke( this, user );

			//send event to the all users
			foreach( var toUser in Users )
			{
				if( toUser.Client != null && ( toUser == user || GetShareUserWithAnother( toUser, user ) ) )
				{
					var m = BeginMessage( toUser.Client, removeUserToClient );
					m.Writer.WriteVariableInt64( user.UserID );//m.Writer.Write( user.UserID );
					m.End();
				}
			}
		}

		public long GetDirectConnectionFreeUserID()
		{
			return Interlocked.Increment( ref directConnectionUserCounter );

			//for( long userID = 1; ; userID++ )
			//{
			//	if( GetUser( userID ) == null )
			//		return userID;
			//}
		}

		public void UpdateObjectControlledByPlayerToClient( UserInfo user, string referenceToObjectControlledByPlayer )
		{
			//update on the server
			user.ReferenceToObjectControlledByPlayer = referenceToObjectControlledByPlayer;

			//send update to clients
			{
				//broadcast message? where else

				foreach( var toUser in Users )
				{
					if( toUser.Client != null && ( toUser == user || GetShareUserWithAnother( toUser, user ) ) )
					{
						var m = BeginMessage( toUser.Client, updateObjectControlledByPlayerToClient );
						m.Writer.WriteVariableInt64( user.UserID ); //m.Writer.Write( user.UserID );
						m.Writer.Write( referenceToObjectControlledByPlayer );
						m.End();
					}
				}
			}
		}

		//public void UpdateUser( UserInfo user, string name )
		//{
		//	if( !usersByIdentifier.ContainsValue( user ) )
		//		return;

		//	UpdateUserEvent?.Invoke( this, user, ref name );

		//	if( user.Name != name )
		//	{
		//		user.Name = name;

		//		//send event about update user to the all users
		//		MessageType messageType = GetMessageType( "UpdateUserToClient" );
		//		foreach( var user2 in Users )
		//		{
		//			if( user2.ConnectedNode != null )
		//			{
		//				var writer = BeginMessage( user2.ConnectedNode, messageType );
		//				writer.WriteVariableUInt32( user.Identifier );
		//				writer.Write( user.Name );
		//				EndMessage();
		//			}
		//		}
		//	}
		//}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_Users : ClientService
	{
		ConcurrentDictionary<long, UserInfo> usersByID = new ConcurrentDictionary<long, UserInfo>();
		UserInfo thisUser;

		///////////////////////////////////////////

		public class UserInfo
		{
			long userID;
			string username;
			bool bot;
			bool thisUserFlag;

			//custom data
			internal string referenceToObjectControlledByPlayer = "";

			//

			internal UserInfo( long userID, string username, bool bot, bool thisUserFlag )
			{
				this.userID = userID;
				this.username = username;
				this.bot = bot;
				this.thisUserFlag = thisUserFlag;
			}

			public long UserID
			{
				get { return userID; }
			}

			public string Username
			{
				get { return username; }
				//internal set { username = value; }
			}

			public bool Bot
			{
				get { return bot; }
			}

			public bool ThisUserFlag
			{
				get { return thisUserFlag; }
			}

			public override string ToString()
			{
				return Username;
			}

			public string ReferenceToObjectControlledByPlayer
			{
				get { return referenceToObjectControlledByPlayer; }
			}
		}

		///////////////////////////////////////////

		public delegate void AddRemoveUserDelegate( ClientNetworkService_Users sender, UserInfo user );
		public event AddRemoveUserDelegate UserAdded;
		public event AddRemoveUserDelegate UserRemoved;
		//public event AddRemoveUserDelegate UpdateUserEvent;

		///////////////////////////////////////////

		public ClientNetworkService_Users()
			: base( "Users", 2 )
		{
			//register message types
			RegisterMessageType( "AddUserToClient", 1, ReceiveMessage_AddUserToClient );
			RegisterMessageType( "RemoveUserToClient", 2, ReceiveMessage_RemoveUserToClient );
			RegisterMessageType( "UpdateObjectControlledByPlayerToClient", 3, ReceiveMessage_UpdateObjectControlledByPlayerToClient );

			//RegisterMessageType( "UpdateUserToClient", 3, ReceiveMessage_UpdateUserToClient );
		}

		protected override void OnDispose()
		{
			while( usersByID.Count != 0 )
			{
				var enumerator = usersByID.GetEnumerator();
				enumerator.MoveNext();
				RemoveUser( enumerator.Current.Value );
			}

			base.OnDispose();
		}

		public ICollection<UserInfo> Users
		{
			get { return usersByID.Values; }
		}

		public UserInfo GetUser( long userID )
		{
			if( usersByID.TryGetValue( userID, out var user ) )
				return user;
			return null;
		}

		bool ReceiveMessage_AddUserToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var userID = reader.ReadVariableInt64();
			var username = reader.ReadString() ?? string.Empty;
			var bot = reader.ReadBoolean();
			bool thisUserFlag = reader.ReadBoolean();

			//custom data
			var referenceToObjectControlledByPlayer = reader.ReadString() ?? string.Empty;

			if( !reader.Complete() )
				return false;

			var user = AddUser( userID, username, bot, thisUserFlag );
			user.referenceToObjectControlledByPlayer = referenceToObjectControlledByPlayer;

			return true;
		}

		bool ReceiveMessage_RemoveUserToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var userID = reader.ReadVariableInt64();
			if( !reader.Complete() )
				return false;

			if( usersByID.TryGetValue( userID, out var user ) )
				RemoveUser( user );

			return true;
		}

		//bool ReceiveMessage_UpdateUserToClient( NetworkNode.ConnectedNode sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		//{
		//	//get data from message
		//	uint identifier = reader.ReadVariableUInt32();
		//	string name = reader.ReadString();
		//	if( !reader.Complete() )
		//		return false;

		//	var user = GetUser( identifier );
		//	if( user != null )
		//	{
		//		user.Username = name;
		//		UpdateUserEvent?.Invoke( this, user );
		//	}

		//	return true;
		//}

		UserInfo AddUser( long userID, string username, bool bot, bool thisUserFlag )
		{
			var user = new UserInfo( userID, username, bot, thisUserFlag );
			usersByID[ userID ] = user;
			if( thisUserFlag )
				thisUser = user;

			UserAdded?.Invoke( this, user );

			return user;
		}

		void RemoveUser( UserInfo user )
		{
			usersByID.TryRemove( user.UserID, out _ );
			if( thisUser == user )
				thisUser = null;

			UserRemoved?.Invoke( this, user );
		}

		public UserInfo ThisUser
		{
			get { return thisUser; }
		}

		bool ReceiveMessage_UpdateObjectControlledByPlayerToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var userID = reader.ReadVariableInt64();
			var referenceToObjectControlledByPlayer = reader.ReadString() ?? string.Empty;
			if( !reader.Complete() )
				return false;

			if( usersByID.TryGetValue( userID, out var user ) )
				user.referenceToObjectControlledByPlayer = referenceToObjectControlledByPlayer;

			return true;
		}

		public UserInfo GetUserByObjectControlledByPlayer( string referenceToObjectControlledByPlayer )
		{
			//slowly?

			foreach( var user in usersByID.Values )
			{
				if( user.ReferenceToObjectControlledByPlayer == referenceToObjectControlledByPlayer )
					return user;
			}
			return null;
		}
	}
}