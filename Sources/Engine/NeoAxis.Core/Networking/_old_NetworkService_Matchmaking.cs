//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using NeoAxis.Networking;

//namespace NeoAxis
//{
//	public class ServerNetworkService_Matchmaking : ServerService
//	{
//		ServerNetworkService_Users usersService;

//		MessageType matchesListToClient;
//		MessageType matchDetailedInfoToClient;
//		MessageType requestMatchesListToServer;
//		MessageType requestMatchCreateToServer;
//		MessageType requestMatchJoinToServer;
//		MessageType requestMatchLeaveToServer;
//		MessageType requestMatchFindToServer;
//		MessageType requestMatchNameChangeToServer;
//		MessageType requestMatchStatusChangeToServer;

//		//matches
//		long matchIdCounter;
//		//ESet? where else
//		List<Match> matches = new List<Match>();

//		//!!!!impl
//		public int MaxUsersInMatch { get; set; } = 10;

//		//!!!!impl
//		public double AutoDeleteMatchWithoutUsersSeconds { get; set; } = 60 * 10;// 10 minutes

//		//!!!!без овнера удалять? тоже с таймеров видимо

//		///////////////////////////////////////////

//		public delegate void MatchCreateBeforeDelegate( ServerNetworkService_Matchmaking sender, ServerNetworkService_Users.UserInfo user, string optionalName, bool join/*, string additionalInfo*/, ref bool allow );
//		public event MatchCreateBeforeDelegate MatchCreateBefore;

//		public delegate void MatchJoinBeforeDelegate( ServerNetworkService_Matchmaking sender, ServerNetworkService_Users.UserInfo user, Match match, ref bool allow );
//		public event MatchJoinBeforeDelegate MatchJoinBefore;

//		public delegate void MatchNameChangeBeforeDelegate( ServerNetworkService_Matchmaking sender, ServerNetworkService_Users.UserInfo user, Match match, string name, ref bool allow );
//		public event MatchNameChangeBeforeDelegate MatchNameChangeBefore;

//		public delegate void MatchStatusChangeBeforeDelegate( ServerNetworkService_Matchmaking sender, ServerNetworkService_Users.UserInfo user, Match match, Match.StatusEnum status, ref bool allow );
//		public event MatchStatusChangeBeforeDelegate MatchStatusChangeBefore;

//		public delegate void MatchFindDelegate( ServerNetworkService_Matchmaking sender, ServerNetworkService_Users.UserInfo user, string userData, ref Match match );
//		public event MatchFindDelegate MatchFind;

//		///////////////////////////////////////////////

//		public class Match
//		{
//			public long Id { get; internal set; }
//			long owner;
//			string name;
//			StatusEnum status;

//			//EConcurrentDictionary without locks
//			volatile Dictionary<long, ServerNetworkService_Users.UserInfo> usersDictionary = new Dictionary<long, ServerNetworkService_Users.UserInfo>();
//			volatile ServerNetworkService_Users.UserInfo[] usersArray;

//			//!!!!chat?

//			string details;
//			public object Tag { get; set; }

//			internal bool needSendChangesToClients;
//			internal DateTime lastSendChangesToClientsTime;

//			/////////////////////

//			public enum StatusEnum
//			{
//				Prepare,
//				Play,
//				Delete,
//			}

//			/////////////////////

//			public long Owner
//			{
//				get { return owner; }
//				set
//				{
//					if( owner == value )
//						return;
//					owner = value;
//					needSendChangesToClients = true;
//				}
//			}

//			public string Name
//			{
//				get { return name; }
//				set
//				{
//					if( name == value )
//						return;
//					name = value;
//					needSendChangesToClients = true;
//				}
//			}

//			public StatusEnum Status
//			{
//				get { return status; }
//				set
//				{
//					if( status == value )
//						return;
//					status = value;
//					needSendChangesToClients = true;
//				}
//			}

//			public ServerNetworkService_Users.UserInfo[] GetUsers()
//			{
//				var users = usersArray;
//				if( users == null )
//				{
//					lock( usersDictionary )
//					{
//						users = usersDictionary.Values.ToArray();
//						usersArray = users;
//					}
//				}
//				return users;
//			}

//			public void AddUser( ServerNetworkService_Users.UserInfo user )
//			{
//				lock( usersDictionary )
//				{
//					usersDictionary[ user.UserID ] = user;
//					usersArray = null;
//				}
//				needSendChangesToClients = true;
//			}

//			public void RemoveUser( ServerNetworkService_Users.UserInfo user )
//			{
//				lock( usersDictionary )
//				{
//					usersDictionary.Remove( user.UserID );
//					usersArray = null;
//				}
//				needSendChangesToClients = true;
//			}

//			public string Details
//			{
//				get { return details; }
//				set
//				{
//					if( details == value )
//						return;
//					details = value;
//					needSendChangesToClients = true;
//				}
//			}
//		}

//		///////////////////////////////////////////

//		public ServerNetworkService_Matchmaking( ServerNetworkService_Users usersService )
//			: base( "Matchmaking", 7 )
//		{
//			this.usersService = usersService;

//			//register message types
//			matchesListToClient = RegisterMessageType( "MatchesListToClient", 1 );
//			matchDetailedInfoToClient = RegisterMessageType( "MatchDetailedInfoToClient", 2 );
//			requestMatchesListToServer = RegisterMessageType( "RequestMatchesListToServer", 3, ReceiveMessage_RequestMatchesListToServer );
//			requestMatchCreateToServer = RegisterMessageType( "RequestMatchCreateToServer", 4, ReceiveMessage_RequestMatchCreateToServer );
//			requestMatchJoinToServer = RegisterMessageType( "RequestMatchJoinToServer", 5, ReceiveMessage_RequestMatchJoinToServer );
//			requestMatchLeaveToServer = RegisterMessageType( "RequestMatchLeaveToServer", 6, ReceiveMessage_RequestMatchLeaveToServer );
//			requestMatchFindToServer = RegisterMessageType( "RequestMatchFindToServer", 7, ReceiveMessage_RequestMatchFindToServer );
//			requestMatchNameChangeToServer = RegisterMessageType( "RequestMatchNameChangeToServer", 8, ReceiveMessage_RequestMatchNameChangeToServer );
//			requestMatchStatusChangeToServer = RegisterMessageType( "RequestMatchStatusChangeToServer", 9, ReceiveMessage_RequestMatchStatusChangeToServer );
//		}

//		public ServerNetworkService_Users UsersService
//		{
//			get { return usersService; }
//		}

//		public Match[] GetMatches()
//		{
//			lock( matches )
//				return matches.ToArray();
//		}

//		public Match GetMatch( long id )
//		{
//			lock( matches )
//				return matches.FirstOrDefault( r => r.Id == id );
//		}

//		public Match GetMatch( string name )
//		{
//			lock( matches )
//				return matches.FirstOrDefault( r => r.Name == name );
//		}

//		public Match MatchCreate( long owner, string name = null )
//		{
//			var name2 = name;
//			if( string.IsNullOrEmpty( name2 ) )
//			{
//				var matches = GetMatches();
//				var usedNames = new ESet<string>();
//				foreach( var m in matches )
//					usedNames.AddWithCheckAlreadyContained( m.Name );

//				var namePrefix = "New Match ";

//				int index = 1;
//				while( true )
//				{
//					name2 = namePrefix + index.ToString();
//					if( !usedNames.Contains( name2 ) )
//						break;
//					index++;
//				}
//			}

//			var match = new Match();
//			match.Id = Interlocked.Increment( ref matchIdCounter );
//			match.Owner = owner;
//			match.Name = name2;
//			lock( matches )
//				matches.Add( match );
//			return match;
//		}

//		public void MatchDelete( Match match )
//		{
//			match.Status = Match.StatusEnum.Delete;

//			lock( matches )
//				matches.Remove( match );
//		}

//		bool ReceiveMessage_RequestMatchesListToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//		{
//			//!!!!send not often

//			if( !reader.Complete() )
//				return false;

//			var user = usersService.GetUser( sender );
//			if( user == null )
//				return true;

//			SendMatchesListToClient( user );

//			return true;
//		}

//		bool IsValidMatchName( string name, out string error )
//		{
//			if( string.IsNullOrEmpty( name ) )
//			{
//				error = "Match name cannot be empty.";
//				return false;
//			}
//			if( name.Length > 100 )
//			{
//				error = "Match name is too long.";
//				return false;
//			}

//			//!!!!more checks

//			error = null;
//			return true;
//		}

//		bool ReceiveMessage_RequestMatchCreateToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//		{
//			var optionalName = reader.ReadString();
//			var join = reader.ReadBoolean();
//			//var additionalInfo = reader.ReadString();
//			if( !reader.Complete() )
//				return false;

//			//get source user
//			var user = usersService.GetUser( sender );
//			if( user == null )
//				return true;

//			var allow = true;
//			MatchCreateBefore?.Invoke( this, user, optionalName, join, ref allow );
//			if( !allow )
//				return true;

//			//create a new match

//			string name = null;
//			if( IsValidMatchName( optionalName, out var error ) )
//				name = optionalName;

//			var match = MatchCreate( user.UserID, name );
//			if( join )
//				match.AddUser( user );

//			return true;
//		}

//		bool ReceiveMessage_RequestMatchJoinToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//		{
//			var matchID = reader.ReadVariableInt64();
//			if( !reader.Complete() )
//				return false;

//			//get source user
//			var user = usersService.GetUser( sender );
//			if( user == null )
//				return true;

//			//get match
//			var match = GetMatch( matchID );
//			if( match == null )
//				return true;

//			//check by event
//			var allow = true;
//			MatchJoinBefore?.Invoke( this, user, match, ref allow );
//			if( !allow )
//				return true;

//			//join
//			match.AddUser( user );

//			return true;
//		}

//		bool ReceiveMessage_RequestMatchLeaveToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//		{
//			var matchID = reader.ReadVariableInt64();
//			if( !reader.Complete() )
//				return false;

//			//get source user
//			var user = usersService.GetUser( sender );
//			if( user == null )
//				return true;

//			//get match
//			var match = GetMatch( matchID );
//			if( match == null )
//				return true;

//			//leave
//			match.RemoveUser( user );

//			return true;
//		}

//		bool ReceiveMessage_RequestMatchFindToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//		{
//			var anyData = reader.ReadString();
//			if( !reader.Complete() )
//				return false;

//			//get source user
//			var user = usersService.GetUser( sender );
//			if( user == null )
//				return true;

//			//!!!!impl: find some time, right now only checked available matches
//			//and create new

//			Match match = null;
//			MatchFind?.Invoke( this, user, anyData, ref match );

//			if( match == null )
//			{
//				//default behavior: join to not full match or create new one

//				foreach( var m in GetMatches() )
//				{
//					if( m.Status == Match.StatusEnum.Prepare && m.GetUsers().Length < MaxUsersInMatch )
//					{
//						match = m;
//						break;
//					}
//				}

//				if( match == null )
//					match = MatchCreate( user.UserID );
//			}

//			if( match == null )
//				return true;

//			match.AddUser( user );

//			return true;
//		}

//		bool ReceiveMessage_RequestMatchNameChangeToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//		{
//			var matchID = reader.ReadVariableInt64();
//			var name = reader.ReadString();
//			if( !reader.Complete() )
//				return false;

//			//get source user
//			var user = usersService.GetUser( sender );
//			if( user == null )
//				return true;

//			//get match
//			var match = GetMatch( matchID );
//			if( match == null )
//				return true;

//			if( !IsValidMatchName( name, out var error ) )
//				return true;

//			var allow = match.Owner == user.UserID;
//			MatchNameChangeBefore?.Invoke( this, user, match, name, ref allow );
//			if( !allow )
//				return true;

//			//change the name
//			match.Name = name;

//			return true;
//		}

//		bool ReceiveMessage_RequestMatchStatusChangeToServer( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//		{
//			var matchID = reader.ReadVariableInt64();
//			var status = (Match.StatusEnum)reader.ReadVariableInt32();
//			if( !reader.Complete() )
//				return false;

//			//get source user
//			var user = usersService.GetUser( sender );
//			if( user == null )
//				return true;

//			//get match
//			var match = GetMatch( matchID );
//			if( match == null )
//				return true;

//			var allow = match.Owner == user.UserID;
//			MatchStatusChangeBefore?.Invoke( this, user, match, status, ref allow );
//			if( !allow )
//				return true;

//			match.Status = status;
//			if( match.Status == Match.StatusEnum.Delete )
//				MatchDelete( match );

//			return true;
//		}

//		void SendMatchesListToClient( ServerNetworkService_Users.UserInfo sendTo )
//		{
//			var m = BeginMessage( sendTo.Client, matchesListToClient );
//			var matches = GetMatches();
//			m.Writer.WriteVariable( matches.Length );
//			for( int nMatch = 0; nMatch < matches.Length; nMatch++ )
//			{
//				var match = matches[ nMatch ];
//				m.Writer.WriteVariable( match.Id );
//				m.Writer.WriteVariable( match.Owner );
//				m.Writer.Write( match.Name );
//				m.Writer.WriteVariableInt32( (int)match.Status );
//				m.Writer.WriteVariable( match.GetUsers().Length );
//			}
//			m.End();
//		}

//		void SendMatchDetailedInfoToClient( ServerNetworkService_Users.UserInfo[] sendTo, Match match )
//		{
//			var recepients = new List<ServerNode.Client>( sendTo.Length );
//			foreach( var user in sendTo )
//			{
//				if( user.Client != null )
//					recepients.Add( user.Client );
//			}

//			var m = BeginMessage( recepients, matchDetailedInfoToClient );
//			m.Writer.WriteVariable( match.Id );
//			m.Writer.WriteVariable( match.Owner );
//			m.Writer.Write( match.Name );
//			m.Writer.WriteVariableInt32( (int)match.Status );
//			var users = match.GetUsers();
//			m.Writer.WriteVariable( users.Length );
//			for( int n = 0; n < users.Length; n++ )
//				m.Writer.WriteVariable( users[ n ].UserID );
//			m.Writer.Write( match.Details );
//			m.End();
//		}

//		protected internal override void OnUpdate()
//		{
//			base.OnUpdate();

//			var now = DateTime.UtcNow;
//			foreach( var match in GetMatches() )
//			{
//				if( match.needSendChangesToClients && ( now - match.lastSendChangesToClientsTime ).TotalSeconds > 5 )
//				{
//					var users = match.GetUsers();
//					if( users.Length != 0 )
//						SendMatchDetailedInfoToClient( users, match );

//					match.lastSendChangesToClientsTime = now;
//					match.needSendChangesToClients = false;
//				}
//			}
//		}
//	}

//	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//	public class ClientNetworkService_Matchmaking : ClientService
//	{
//		ClientNetworkService_Users usersService;

//		MessageType matchesListToClient;
//		MessageType matchDetailedInfoToClient;
//		MessageType requestMatchesListToServer;
//		MessageType requestMatchCreateToServer;
//		MessageType requestMatchJoinToServer;
//		MessageType requestMatchLeaveToServer;
//		MessageType requestMatchFindToServer;
//		MessageType requestMatchNameChangeToServer;
//		MessageType requestMatchStatusChangeToServer;

//		//ESet? where else
//		Match[] matches = Array.Empty<Match>();
//		long thisUserMatchID;

//		///////////////////////////////////////////

//		//!!!!

//		//public delegate void ThisUserAddedToMatchDelegate( ClientNetworkService_Matchmaking sender, Match match );
//		//public event ThisUserAddedToMatchDelegate ThisUserAddedToMatch;

//		///////////////////////////////////////////

//		public class Match
//		{
//			public long Id { get; internal set; }
//			public long Owner { get; internal set; }
//			public string Name { get; internal set; }
//			public StatusEnum Status { get; internal set; }
//			public int UserCount { get; internal set; }

//			//detailed info
//			internal volatile ESet<long> usersSet = new ESet<long>();
//			internal volatile long[] usersArray = Array.Empty<long>();

//			public string Details { get; internal set; }

//			/////////////////////

//			public enum StatusEnum
//			{
//				Prepare,
//				Play,
//				Delete,
//			}

//			/////////////////////

//			public ESet<long> UsersSet
//			{
//				get { return usersSet; }
//			}

//			public long[] UsersArray
//			{
//				get { return usersArray; }
//			}
//		}

//		///////////////////////////////////////////

//		public ClientNetworkService_Matchmaking( ClientNetworkService_Users usersService )
//			: base( "Matchmaking", 7 )
//		{
//			this.usersService = usersService;

//			//register message types
//			matchesListToClient = RegisterMessageType( "MatchesListToClient", 1, ReceiveMessage_MatchesListToClient );
//			matchDetailedInfoToClient = RegisterMessageType( "MatchDetailedInfoToClient", 2, ReceiveMessage_MatchDetailedInfoToClient );
//			requestMatchesListToServer = RegisterMessageType( "RequestMatchesListToServer", 3 );
//			requestMatchCreateToServer = RegisterMessageType( "RequestMatchCreateToServer", 4 );
//			requestMatchJoinToServer = RegisterMessageType( "RequestMatchJoinToServer", 5 );
//			requestMatchLeaveToServer = RegisterMessageType( "RequestMatchLeaveToServer", 6 );
//			requestMatchFindToServer = RegisterMessageType( "RequestMatchFindToServer", 7 );
//			requestMatchNameChangeToServer = RegisterMessageType( "RequestMatchNameChangeToServer", 8 );
//			requestMatchStatusChangeToServer = RegisterMessageType( "RequestMatchStatusChangeToServer", 9 );
//		}

//		public ClientNetworkService_Users UsersService
//		{
//			get { return usersService; }
//		}

//		public Match[] Matches
//		{
//			get { return matches; }
//		}

//		//public Match[] GetMatches()
//		//{
//		//	//lock( matches )
//		//	return matches;//.ToArray();
//		//}

//		public Match GetMatch( long id )
//		{
//			//lock( matches )
//			return matches.FirstOrDefault( r => r.Id == id );
//		}

//		public Match GetMatch( string name )
//		{
//			//lock( matches )
//			return matches.FirstOrDefault( r => r.Name == name );
//		}

//		public long ThisUserMatchID
//		{
//			get { return thisUserMatchID; }
//		}

//		bool ReceiveMessage_MatchesListToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//		{
//			//get data from the message

//			var matchesAdded = new ESet<long>();
//			var matchesData = new List<(long id, long owner, string name, Match.StatusEnum status, int userCount)>();

//			var matchCount = reader.ReadVariableInt32();
//			for( int nMatch = 0; nMatch < matchCount; nMatch++ )
//			{
//				var id = reader.ReadVariableInt64();
//				var owner = reader.ReadVariableInt64();
//				var name = reader.ReadString();
//				var status = (Match.StatusEnum)reader.ReadVariableInt32();
//				var userCount = reader.ReadVariableInt32();

//				matchesAdded.Add( id );
//				matchesData.Add( (id, owner, name, status, userCount) );
//			}

//			if( !reader.Complete() )
//				return false;

//			//update matches

//			//lock( matches )
//			{
//				var newList = new List<Match>( matchCount );

//				for( int n = 0; n < matchCount; n++ )
//				{
//					var matchData = matchesData[ n ];

//					var match = GetMatch( matchData.id );
//					if( match == null )
//						match = new Match();

//					match.Id = matchData.id;
//					match.Owner = matchData.owner;
//					match.Name = matchData.name;
//					match.Status = matchData.status;
//					match.UserCount = matchData.userCount;

//					newList.Add( match );
//				}

//				matches = newList.ToArray();
//			}

//			return true;
//		}

//		bool ReceiveMessage_MatchDetailedInfoToClient( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
//		{
//			//get data from the message
//			var matchID = reader.ReadVariableInt64();
//			var owner = reader.ReadVariableInt64();
//			var name = reader.ReadString();
//			var status = (Match.StatusEnum)reader.ReadVariableInt32();
//			var userCount = reader.ReadVariableInt32();
//			var users = new List<long>( userCount );
//			for( int n = 0; n < userCount; n++ )
//				users.Add( reader.ReadVariableInt64() );
//			var details = reader.ReadString();

//			//may be useful:
//			//new joined users
//			//leaved users

//			if( !reader.Complete() )
//				return false;

//			//update match data
//			{
//				var match = GetMatch( matchID );
//				if( match == null )
//				{
//					match = new Match();
//					matches = CollectionUtility.Merge( matches, new Match[] { match } );
//				}
//				match.Owner = owner;
//				match.Name = name;
//				match.Status = status;
//				match.UserCount = userCount;
//				lock( match.usersSet )
//				{
//					match.usersSet.Clear();
//					match.usersSet.AddRangeWithCheckAlreadyContained( users );
//					match.usersArray = match.usersSet.ToArray();
//				}
//				match.Details = details;
//			}

//			//update currentMatchID
//			{
//				var newMatchID = 0L;
//				var thisUser = UsersService.ThisUser;
//				if( thisUser?.UserID != 0 )
//				{
//					foreach( var match in Matches )
//					{
//						if( match.UsersSet.Contains( thisUser.UserID ) )
//							newMatchID = thisUser.UserID;
//					}
//				}
//				thisUserMatchID = newMatchID;
//			}

//			return true;
//		}

//		public void RequestMatchesList()
//		{

//			//!!!!не часто вызывать, периодически

//			var m = BeginMessage( requestMatchesListToServer );
//			m.End();
//		}

//		public void RequestMatchCreate( string optionalName, bool join )//, string additionalInfo = null )
//		{
//			var m = BeginMessage( requestMatchCreateToServer );
//			m.Writer.Write( optionalName );
//			m.Writer.Write( join );
//			//m.Writer.Write( additionalInfo );
//			m.End();
//		}

//		public void RequestMatchJoin( long matchID )
//		{
//			var m = BeginMessage( requestMatchJoinToServer );
//			m.Writer.WriteVariableInt64( matchID );
//			m.End();
//		}

//		public void RequestMatchLeave( long matchID )
//		{
//			var m = BeginMessage( requestMatchLeaveToServer );
//			m.Writer.WriteVariableInt64( matchID );
//			m.End();
//		}

//		//!!!!impl: find some time, right now only checked available matches
//		public void RequestMatchFind( string anyData = null )
//		{
//			var m = BeginMessage( requestMatchFindToServer );
//			m.Writer.Write( anyData );
//			m.End();
//		}

//		public void RequestMatchNameChange( long matchID, string name )
//		{
//			var m = BeginMessage( requestMatchNameChangeToServer );
//			m.Writer.WriteVariableInt64( matchID );
//			m.Writer.Write( name );
//			m.End();
//		}

//		public void RequestMatchStatusChange( long matchID, Match.StatusEnum status )
//		{
//			var m = BeginMessage( requestMatchStatusChangeToServer );
//			m.Writer.WriteVariableInt64( matchID );
//			m.Writer.WriteVariableInt32( (int)status );
//			m.End();
//		}
//	}
//}