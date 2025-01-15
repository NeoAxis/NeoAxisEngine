// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using NeoAxis.Networking;

namespace NeoAxis
{
	public class ServerNetworkService_Users : ServerService
	{
		Dictionary<long, UserInfo> usersByID = new Dictionary<long, UserInfo>();
		Dictionary<ServerNode.Client, UserInfo> usersByClient = new Dictionary<ServerNode.Client, UserInfo>();
		//UserInfo serverUser;

		long botIDCounter = 10000000001L;
		long botUniqueNameCounter = 1;

		///////////////////////////////////////////

		public class UserInfo
		{
			//long userID;
			//string username;
			ServerNode.Client client;

			long botUserID;
			string botUsername = "";

			//

			internal UserInfo( /*long userID, string username, */ServerNode.Client client )
			{
				//this.userID = userID;
				//this.username = username;
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
				string address;
				if( client != null )
				{
					if( client.RemoteEndPoint != null )
						address = client.RemoteEndPoint.ToString();
					else
						address = "Unknown address";
				}
				else
					address = "No connection";

				return string.Format( "{0} ({1})", Username, address );


				//string ipAddressText;
				//if( connectedNode != null )
				//{
				//	IPAddress ipAddress = IPAddress.None;
				//	if( connectedNode.RemoteEndPoint != null )
				//		ipAddress = connectedNode.RemoteEndPoint.Address;
				//	ipAddressText = ipAddress.ToString();
				//}
				//else
				//	ipAddressText = "Local";
				//return string.Format( "{0} ({1})", Username, ipAddressText );
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

		///////////////////////////////////////////

		public ServerNetworkService_Users()
			: base( "Users", 2 )
		{
			//register message types
			RegisterMessageType( "AddUserToClient", 1 );
			RegisterMessageType( "RemoveUserToClient", 2 );
			RegisterMessageType( "UpdateObjectControlledByPlayerToClient", 3 );
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

		//public UserInfo ServerUser
		//{
		//	get { return serverUser; }
		//}

		public UserInfo GetUser( ServerNode.Client client )
		{
			if( usersByClient.TryGetValue( client, out var user ) )
				return user;
			return null;
		}

		//uint GetFreeUserIdentifier()
		//{
		//	uint identifier = 1;
		//	while( usersByIdentifier.ContainsKey( identifier ) )
		//		identifier++;
		//	return identifier;
		//}

		public UserInfo AddUser( ServerNode.Client client )
		//UserInfo CreateUser( string name, NetworkNode.ConnectedNode connectedNode )
		{
			//uint identifier = GetFreeUserIdentifier();

			var newUser = new UserInfo( /*identifier, name, */client );

			usersByID.Add( newUser.UserID, newUser );
			if( newUser.Client != null )
				usersByClient.Add( newUser.Client, newUser );

			{
				var messageType = GetMessageType( "AddUserToClient" );

				//!!!!необязательно всем

				//send event about new user to the all users
				foreach( var user in Users )
				{
					if( user.Client != null )
					{
						bool thisUserFlag = user == newUser;

						var writer = BeginMessage( user.Client, messageType );
						writer.Write( newUser.UserID );//writer.WriteVariableUInt64( (ulong)newUser.UserID );
						writer.Write( newUser.Username );
						writer.Write( newUser.Bot );
						writer.Write( thisUserFlag );

						//custom data
						writer.Write( newUser.ReferenceToObjectControlledByPlayer );

						EndMessage();
					}
				}

				//!!!!необязательно всем

				if( newUser.Client != null )
				{
					//send list of users to new user
					foreach( var user in Users )
					{
						if( user == newUser )
							continue;

						var writer = BeginMessage( newUser.Client, messageType );
						writer.Write( user.UserID );//writer.WriteVariableUInt64( (ulong)user.UserID );
						writer.Write( user.Username );
						writer.Write( user.Bot );
						writer.Write( false );//this user flag

						//custom data
						writer.Write( user.ReferenceToObjectControlledByPlayer );

						EndMessage();
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

			var userID = botIDCounter;
			botIDCounter++;

			var newUser = new UserInfo( userID, username2 );
			newUser.AnyData = anyData;

			usersByID.Add( newUser.UserID, newUser );

			{
				var messageType = GetMessageType( "AddUserToClient" );

				//!!!!необязательно всем

				//send event about new user to the all users
				foreach( var user in Users )
				{
					if( user.Client != null )
					{
						bool thisUserFlag = user == newUser;

						var writer = BeginMessage( user.Client, messageType );
						writer.Write( newUser.UserID );//writer.WriteVariableUInt64( (ulong)newUser.UserID );
						writer.Write( newUser.Username );
						writer.Write( newUser.Bot );
						writer.Write( thisUserFlag );
						EndMessage();
					}
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
			//if( !usersByID.ContainsValue( user ) )
			//	return;

			UserRemoved?.Invoke( this, user );

			//remove user
			usersByID.Remove( user.UserID );
			if( user.Client != null )
				usersByClient.Remove( user.Client );
			//if( serverUser == user )
			//	serverUser = null;


			//!!!!необязательно всем

			//send event to the all users
			{
				var messageType = GetMessageType( "RemoveUserToClient" );

				foreach( var toUser in Users )
				{
					if( toUser.Client != null )
					{
						var writer = BeginMessage( toUser.Client, messageType );
						writer.Write( user.UserID );//writer.WriteVariableUInt64( (ulong)user.UserID );
						EndMessage();
					}
				}
			}
		}

		public long GetFreeUserID()
		{
			for( long userID = 1; ; userID++ )
			{
				if( GetUser( userID ) == null )
					return userID;
			}
		}

		public void UpdateObjectControlledByPlayerToClient( UserInfo user, string referenceToObjectControlledByPlayer )
		{
			//update on the server
			user.ReferenceToObjectControlledByPlayer = referenceToObjectControlledByPlayer;

			//send update to clients
			{
				//!!!!можно отправлять не всем

				//!!!!всем сразу отправлять одним BeginMessage. где еще так

				var messageType = GetMessageType( "UpdateObjectControlledByPlayerToClient" );

				foreach( var toUser in Users )
				{
					if( toUser.Client != null )
					{
						var writer = BeginMessage( toUser.Client, messageType );
						writer.Write( user.UserID );
						writer.Write( referenceToObjectControlledByPlayer );
						EndMessage();
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

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_Users : ClientService
	{
		//!!!!не все могут быть. только те которые сервер дал
		//key: user identifier
		Dictionary<long, UserInfo> usersByID = new Dictionary<long, UserInfo>();
		UserInfo thisUser;

		///////////////////////////////////////////

		public class UserInfo
		{
			long userID;
			string username;
			bool bot;

			//custom data
			internal string referenceToObjectControlledByPlayer = "";

			//

			internal UserInfo( long userID, string username, bool bot )
			{
				this.userID = userID;
				this.username = username;
				this.bot = bot;
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
		public event AddRemoveUserDelegate AddUserEvent;
		public event AddRemoveUserDelegate RemoveUserEvent;
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
			var userID = reader.ReadInt64();//long userID = (long)reader.ReadVariableUInt64();
			var username = reader.ReadString();
			var bot = reader.ReadBoolean();
			bool thisUserFlag = reader.ReadBoolean();

			//custom data
			var referenceToObjectControlledByPlayer = reader.ReadString();

			if( !reader.Complete() )
				return false;

			var user = AddUser( userID, username, bot, thisUserFlag );
			user.referenceToObjectControlledByPlayer = referenceToObjectControlledByPlayer;

			return true;
		}

		bool ReceiveMessage_RemoveUserToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var userID = reader.ReadInt64();//long userID = (long)reader.ReadVariableUInt64();
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
			var user = new UserInfo( userID, username, bot );
			usersByID.Add( userID, user );

			if( thisUserFlag )
				thisUser = user;

			AddUserEvent?.Invoke( this, user );

			return user;
		}

		void RemoveUser( UserInfo user )
		{
			RemoveUserEvent?.Invoke( this, user );

			usersByID.Remove( user.UserID );

			if( thisUser == user )
				thisUser = null;
		}

		public UserInfo ThisUser
		{
			get { return thisUser; }
		}

		bool ReceiveMessage_UpdateObjectControlledByPlayerToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			//get data from message
			var userID = reader.ReadInt64();
			var referenceToObjectControlledByPlayer = reader.ReadString();
			if( !reader.Complete() )
				return false;

			if( usersByID.TryGetValue( userID, out var user ) )
				user.referenceToObjectControlledByPlayer = referenceToObjectControlledByPlayer;

			return true;
		}

		public UserInfo GetUserByObjectControlledByPlayer( string referenceToObjectControlledByPlayer )
		{
			//!!!!slowly?

			foreach( var user in usersByID.Values )
			{
				if( user.ReferenceToObjectControlledByPlayer == referenceToObjectControlledByPlayer )
					return user;
			}
			return null;
		}
	}
}