#if !NO_SERVER
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Internal.LiteDB;
using NeoAxis;
using NeoAxis.Networking;

namespace NeoAxis.CloudServer
{
	public static class Matches
	{
		//settings
		public static bool MatchChatEnabled { get; set; }
		public static int MatchMaxCountPerUser { get; set; } = 1000;
		public static int MatchNameMaxLength { get; set; } = 200;
		public static int MatchAnyDataMaxLength { get; set; } = 1000;
		public static int MatchUserAnyDataMaxLength { get; set; } = 1000;

		static bool initialized;

		public static ILiteCollection<Match> matchesCollection;
		public static ConcurrentLockManager<long> matchesCollectionLockManager = new ConcurrentLockManager<long>();
		static object matchesCollectionNewItemLock = new object();

		public static ILiteCollection<MatchUser> matchUsersCollection;
		public static ConcurrentLockManager<long> matchUsersCollectionLockManager = new ConcurrentLockManager<long>();
		static object matchUsersCollectionNewItemLock = new object();

		///////////////////////////////////////////////

		public class Match
		{
			//simple structure to support cloud method GetMatches

			public long Id { get; set; }
			public DateTime CreationTime { get; set; }
			public string Status { get; set; } //"Lobby", "Play", "Deleted"
			public DateTime DeletionTime { get; set; }
			public long UserID { get; set; }
			public string Group { get; set; }
			public string Name { get; set; }
			public string AnyData { get; set; }

			public long ChatID { get; set; }
		}

		///////////////////////////////////////////////

		public class MatchUser
		{
			//simple structure to support cloud methods in the future

			public long Id { get; set; }
			public DateTime EnterTime { get; set; }
			public long MatchID { get; set; }
			public long UserID { get; set; }
			public string Username { get; set; }
			public string AnyData { get; set; }
		}

		///////////////////////////////////////////////

		public class SimpleResult
		{
			public string Error { get; set; }
		}

		///////////////////////////////////////////////

		public static void Initialize()
		{
			if( initialized )
				return;

			//check database is initialized
			var database = CloudFunctionsServer.ServerNode?.CloudFunctions?.DatabaseImpl?.Database;
			if( database == null )
			{
				ServerLogs.Write( "Cloud Functions", "Matches: Initialize: Database is not initialized." );
				Console.WriteLine( "Matches: Initialize: Database is not initialized." );
				return;
			}

			//get and configure Matches collection
			matchesCollection = database.GetCollection<Match>( "Matches" );
			matchesCollection.EnsureIndex( x => x.CreationTime );
			matchesCollection.EnsureIndex( x => x.Status );
			matchesCollection.EnsureIndex( x => x.UserID );
			matchesCollection.EnsureIndex( x => x.Group );
			matchesCollection.EnsureIndex( x => x.UserID );

			//get and configure MatchUsers collection
			matchUsersCollection = database.GetCollection<MatchUser>( "MatchUsers" );
			matchUsersCollection.EnsureIndex( x => x.MatchID );
			matchUsersCollection.EnsureIndex( x => x.UserID );

			//register cloud methods
			CloudFunctionsServer.ServerNode.CloudFunctions.RegisterCloudMethods( typeof( Matches ), out var error );
			if( !string.IsNullOrEmpty( error ) )
			{
				ServerLogs.Write( "Cloud Functions", "Matches: Initialize: RegisterCloudMethods error. " + error );
				Console.WriteLine( "Matches: Initialize: RegisterCloudMethods error. " + error );
			}

			initialized = true;
		}

		public static void Update( DateTime utcNow )
		{
			if( !initialized )
				return;

		}

		///////////////////////////////////////////////
		//matches


		//!!!!? optimized variant to return less amount of data in the list. return only IDs and names

		//!!!! pages
		//!!!!? DateTime? creationTimeStart = null, DateTime? creationTimeEnd = null


		public class GetMatchesResult
		{
			public Match[] Matches;
			public string Error;
		}

		public static GetMatchesResult GetMatches( long[] matches = null, long[] users = null, string[] groups = null, string[] statuses = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetMatchesResult { Error = "Match service is not initialized." };

				var queriesAnd = new List<BsonExpression>();

				if( matches != null && matches.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var matchID in matches )
						queriesOr.Add( Query.EQ( "_id", matchID ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}
				if( users != null && users.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var userID in users )
						queriesOr.Add( Query.EQ( "UserID", userID ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}
				if( groups != null && groups.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var group in groups )
						queriesOr.Add( Query.EQ( "Group", group ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}
				if( statuses != null && statuses.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var status in statuses )
						queriesOr.Add( Query.EQ( "Status", status ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}

				ILiteQueryable<Match> queryable;
				if( queriesAnd.Count > 1 )
					queryable = matchesCollection.Query().Where( Query.And( queriesAnd.ToArray() ) );
				else if( queriesAnd.Count == 1 )
					queryable = matchesCollection.Query().Where( queriesAnd[ 0 ] );
				else
					queryable = matchesCollection.Query();

				queryable = queryable.OrderBy( "CreationTime", Query.Ascending );

				return new GetMatchesResult() { Matches = queryable.ToArray() };
			}
			catch( Exception e )
			{
				return new GetMatchesResult { Error = e.Message };
			}
		}

		public delegate void GetMatchesCheckAccessRightsDelegate( long callerID, long[] matches, long[] users, string[] groups, string[] statuses, ref bool allow );
		public static event GetMatchesCheckAccessRightsDelegate GetMatchesCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 60 )]
		public static Match[] GetMatches( ServerNetworkService_CloudFunctions.CallMethodContext context, long[] matches, long[] users, string[] groups, string[] statuses )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Match service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );


			//!!!!impl public/private matches. Private only for friends or something like that.

			//!!!!or maybe also when has invitation


			var allowByOwner = users != null && users.Length == 1 && users[ 0 ] == callerID;
			var allowByLobbyStatus = statuses != null && statuses.Length == 1 && statuses[ 0 ] == "Lobby";
			var allow = allowByOwner || allowByLobbyStatus;

			GetMatchesCheckAccessRights?.Invoke( callerID, matches, users, groups, statuses, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//get matches
			var result = GetMatches( matches, users, groups, statuses );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );

			return result.Matches;
		}

		public class GetMatchCountResult
		{
			public int Count;
			public string Error;
		}

		public static GetMatchCountResult GetMatchCount( long[] matches = null, long[] users = null, string[] groups = null, string[] statuses = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetMatchCountResult { Error = "Match service is not initialized." };

				//query

				var queriesAnd = new List<BsonExpression>();

				if( matches != null && matches.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var matchID in matches )
						queriesOr.Add( Query.EQ( "_id", matchID ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}
				if( users != null && users.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var userID in users )
						queriesOr.Add( Query.EQ( "UserID", userID ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}
				if( groups != null && groups.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var group in groups )
						queriesOr.Add( Query.EQ( "Group", group ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}
				if( statuses != null && statuses.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var status in statuses )
						queriesOr.Add( Query.EQ( "Status", status ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}

				ILiteQueryable<Match> queryable;
				if( queriesAnd.Count > 1 )
					queryable = matchesCollection.Query().Where( Query.And( queriesAnd.ToArray() ) );
				else if( queriesAnd.Count == 1 )
					queryable = matchesCollection.Query().Where( queriesAnd[ 0 ] );
				else
					queryable = matchesCollection.Query();

				return new GetMatchCountResult() { Count = queryable.Count() };
			}
			catch( Exception e )
			{
				return new GetMatchCountResult { Error = e.Message };
			}
		}

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static int GetMatchCount( ServerNetworkService_CloudFunctions.CallMethodContext context, long[] matches, long[] users, string[] groups, string[] statuses )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Match service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//check permissions
			var allow = users != null && users.Length == 1 && users[ 0 ] == callerID;
			GetMatchesCheckAccessRights?.Invoke( callerID, matches, users, groups, statuses, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//get count
			var result = GetMatchCount( matches, users, groups, statuses );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );

			return result.Count;
		}

		public class GetMatchResult
		{
			public Match Match;
			public bool NotFound;
			public string Error;
		}

		public static GetMatchResult GetMatch( long matchID )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetMatchResult { Error = "Match service is not initialized." };

				var match = matchesCollection.FindById( matchID );
				if( match == null )
					return new GetMatchResult { NotFound = true };

				return new GetMatchResult() { Match = match };
			}
			catch( Exception e )
			{
				return new GetMatchResult { Error = e.Message };
			}
		}

		public static GetMatchResult GetMatchByChatID( long chatID )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetMatchResult { Error = "Match service is not initialized." };

				var match = matchesCollection.FindOne( Query.EQ( "ChatID", chatID ) );
				if( match == null )
					return new GetMatchResult { NotFound = true };

				return new GetMatchResult() { Match = match };
			}
			catch( Exception e )
			{
				return new GetMatchResult { Error = e.Message };
			}
		}

		public delegate void GetMatchCheckAccessRightsDelegate( long callerID, long matchID, ref bool allow );
		public static event GetMatchCheckAccessRightsDelegate GetMatchCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static Match GetMatch( ServerNetworkService_CloudFunctions.CallMethodContext context, long matchID )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Match service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//get match
			var result = GetMatch( matchID );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );
			if( result.NotFound )
				throw new Exception( "Match not found." );

			//set default access rights by lobby status
			var allow = result.Match.Status == "Lobby";

			//set default access rights by user in the match
			if( !allow )
			{
				//get match user
				var getMatchUserResult = GetMatchUser( matchID, callerID );
				if( !string.IsNullOrEmpty( getMatchUserResult.Error ) )
					throw new Exception( getMatchUserResult.Error );

				allow = getMatchUserResult.User != null;
			}

			GetMatchCheckAccessRights?.Invoke( callerID, matchID, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			return result.Match;
		}

		static long GetUniqueMatchID()
		{
			var random = new FastRandom();
			for( var digits = 4; digits < 20; digits++ )
			{
				for( int attempts = 0; attempts < 10; attempts++ )
				{
					long id = random.Next( (long)Math.Pow( 10, digits - 1 ), (long)Math.Pow( 10, digits ) - 1 );
					if( matchesCollection.FindById( id ) == null )
						return id;
				}
			}
			return 0;
		}

		public delegate void NewMatchBeforeDelegate( long userID, ref string name, ref string anyData, ref string error );
		public static event NewMatchBeforeDelegate NewMatchBefore;

		public delegate void NewMatchAfterDelegate( Match match );
		public static event NewMatchAfterDelegate NewMatchAfter;

		public class NewMatchResult
		{
			public Match Match;
			public string Error;
		}

		public static NewMatchResult NewMatch( long userID, string name, string anyData = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new NewMatchResult { Error = "Match service is not initialized." };

				//check input
				if( MatchMaxCountPerUser >= 0 )
				{
					var getCountResult = GetMatchCount( users: new[] { userID } );
					if( !string.IsNullOrEmpty( getCountResult.Error ) )
						return new NewMatchResult { Error = getCountResult.Error };
					if( getCountResult.Count >= MatchMaxCountPerUser )
						return new NewMatchResult { Error = "The maximum number of matches has been reached." };
				}

				//check name
				if( name != null && name.Length > MatchNameMaxLength )
					return new NewMatchResult { Error = $"The match name length exceeds the maximum allowed. Limit: {MatchNameMaxLength} characters." };

				//check anyData
				if( anyData != null && anyData.Length > MatchAnyDataMaxLength )
					return new NewMatchResult { Error = $"The match anyData length exceeds the maximum allowed. Limit: {MatchAnyDataMaxLength} characters." };

				//set default name if not specified
				if( string.IsNullOrEmpty( name ) )
				{
					var defaultNames = new[]
					{
						"Fun Match", "Great Game", "Exciting Battle", "Epic Showdown", "Intense Competition",
						"Thrilling Encounter", "Legendary Clash", "Unforgettable Duel", "Fierce Struggle", "Memorable Contest",
						"Action-Packed Match", "Heart-Pounding Game", "Adrenaline-Fueled Battle", "Nail-Biting Showdown", "High-Stakes Competition",
						"Gripping Encounter", "Riveting Clash", "Dynamic Match", "Electrifying Game", "Pulse-Racing Battle",
						"Edge-of-Your-Seat Showdown", "Captivating Encounter", "Dramatic Clash", "Spectacular Contest", "Breathtaking Duel",
						"Monumental Struggle", "Unbelievable Match", "Incredible Game", "Fantastic Battle", "Amazing Showdown",
						"Tremendous Competition", "Marvelous Encounter", "Wonderful Clash", "Superb Duel", "Excellent Struggle",
						"Outstanding Contest", "Remarkable Match", "Extraordinary Game", "Phenomenal Battle", "Sensational Showdown",
						"Stunning Competition", "Astonishing Encounter", "Astounding Clash", "Mind-Blowing Duel", "Overwhelming Struggle",
						"Impressive Contest", "Formidable Match", "Powerful Game", "Dominant Battle", "Commanding Showdown",
						"Authoritative Competition", "Forceful Encounter", "Potent Clash", "Mighty Duel", "Robust Struggle",
						"Vigorous Contest", "Energetic Match", "Lively Game", "Spirited Battle", "Animated Showdown",
						"Vibrant Competition", "Dynamic Encounter", "Active Clash", "Zealous Duel", "Passionate Struggle",
						"Fervent Contest", "Ardent Match", "Fiery Game", "Blazing Battle", "Sizzling Showdown",
						"Scorching Competition", "Burning Encounter", "Flaming Clash", "Infernal Duel", "Volcanic Struggle",
						"Explosive Contest", "Thunderous Match", "Roaring Game", "Resounding Battle", "Echoing Showdown",
						"Reverberating Competition", "Resonant Encounter", "Powerful Clash", "Thunderous Duel", "Deafening Struggle",
						"Overpowering Contest", "Dominating Match", "Overwhelming Game", "Conquering Battle", "Victorious Showdown",
						"Triumphant Competition", "Winning Encounter", "Successful Clash", "Achieving Duel", "Accomplishing Struggle",
						"Masterful Contest", "Skillful Match", "Expert Game", "Proficient Battle", "Competent Showdown",
						"Capable Competition", "Adept Encounter", "Talented Clash", "Gifted Duel", "Brilliant Struggle",
						"Genius Contest", "Masterpiece Match", "Classic Game", "Timeless Battle", "Enduring Showdown"
					};

					var random = new FastRandom();
					name = defaultNames[ random.Next( 0, defaultNames.Length ) ] + " " + random.Next( 100, 999 ).ToString();
				}

				//event before new match
				string error = null;
				NewMatchBefore?.Invoke( userID, ref name, ref anyData, ref error );
				if( !string.IsNullOrEmpty( error ) )
					return new NewMatchResult { Error = error };

				//add to the database
				var match = new Match();
				lock( matchesCollectionNewItemLock )
				{
					match.Id = GetUniqueMatchID();
					match.CreationTime = DateTime.UtcNow;
					match.Status = "Lobby";
					match.UserID = userID;
					match.Name = name;
					match.AnyData = anyData;
					matchesCollection.Insert( match );

					//create chat for the match
					if( MatchChatEnabled )
					{
						var newChatResult = Chats.NewChat( userID, $"Match Chat: {match.Name}", anyData: $"MatchID={match.Id}" );
						if( !string.IsNullOrEmpty( newChatResult.Error ) )
						{
							//log error but do not prevent match creation
							ServerLogs.Write( "Cloud Functions", $"Matches: NewMatch: Create chat error. {newChatResult.Error}" );
							Console.WriteLine( $"Matches: NewMatch: Create chat error. {newChatResult.Error}" );
						}
						else
						{
							match.ChatID = newChatResult.Chat.Id;
							matchesCollection.Update( match );
						}
					}
				}

				//event after new match
				NewMatchAfter?.Invoke( match );

				return new NewMatchResult { Match = match };
			}
			catch( Exception e )
			{
				return new NewMatchResult { Error = e.Message };
			}
		}

		public delegate void NewMatchCheckAccessRightsDelegate( long callerID, ref string name, ref string anyData, ref bool allow );
		public static event NewMatchCheckAccessRightsDelegate NewMatchCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 5 )]
		public static long NewMatch( ServerNetworkService_CloudFunctions.CallMethodContext context, string name, string anyData )
		{
			//check initialized
			if( !initialized )
				throw new Exception( "Match service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new Exception( "Client is not logged in." );

			//check access rights
			var allow = true;
			NewMatchCheckAccessRights?.Invoke( callerID, ref name, ref anyData, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//call create match
			var result = NewMatch( callerID, name, anyData );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );

			var match = result.Match;

			//add match creator as match user
			{
				var username = context.Client.LoginDataUsername;
				if( string.IsNullOrEmpty( username ) )
					username = "(No name)";

				var addMatchUserResult = AddMatchUser( match.Id, callerID, username, null );
				if( !string.IsNullOrEmpty( addMatchUserResult.Error ) )
				{
					//log error but do not prevent match creation
					ServerLogs.Write( "Cloud Functions", $"Matches: NewMatch: Add match user error. {addMatchUserResult.Error}" );
					Console.WriteLine( $"Matches: NewMatch: Add match user error. {addMatchUserResult.Error}" );
				}
			}

			return result.Match.Id;
		}

		public delegate void UpdateMatchBeforeDelegate( long matchID, Match currentMatchData, ref string name, ref string status, ref string anyData, ref string error );
		public static event UpdateMatchBeforeDelegate UpdateMatchBefore;

		public delegate void UpdateMatchAfterDelegate( Match match, Match oldMatchData );
		public static event UpdateMatchAfterDelegate UpdateMatchAfter;

		public static SimpleResult UpdateMatch( long matchID, string status = null, string name = null, string anyData = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new SimpleResult { Error = "Match service is not initialized." };

				//check name
				if( name != null && name.Length > MatchNameMaxLength )
					return new SimpleResult() { Error = $"The match name length exceeds the maximum allowed. Limit: {MatchNameMaxLength} characters." };

				//check anyData
				if( anyData != null && anyData.Length > MatchAnyDataMaxLength )
					return new SimpleResult() { Error = $"The match anyData length exceeds the maximum allowed. Limit: {MatchAnyDataMaxLength} characters." };

				//get previous match data?
				var oldMatchData = matchesCollection.FindById( matchID );
				if( oldMatchData == null )
					return new SimpleResult { Error = "Match not found." };

				//event before update
				string error = null;
				UpdateMatchBefore?.Invoke( matchID, oldMatchData, ref name, ref status, ref anyData, ref error );
				if( !string.IsNullOrEmpty( error ) )
					return new SimpleResult { Error = error };

				//update in database
				Match match = null;
				using( matchesCollectionLockManager.LockDisposable( matchID ) )
				{
					match = matchesCollection.FindById( matchID );
					if( match == null )
						return new SimpleResult { Error = "Match not found." };

					if( status != null )
					{
						match.Status = status;
						if( status == "Deleted" && match.Status != "Deleted" )
							match.DeletionTime = DateTime.UtcNow;
					}
					if( name != null )
						match.Name = name;
					if( anyData != null )
						match.AnyData = anyData;

					matchesCollection.Update( match );
				}

				//delete chat if match is deleted
				if( match.ChatID != 0 && match.Status == "Deleted" )
				{
					var updateResult = Chats.UpdateChat( match.ChatID, status: "Deleted" );
					if( !string.IsNullOrEmpty( updateResult.Error ) )
					{
						ServerLogs.Write( "Cloud Functions", "Matches: UpdateMatch: Chats.UpdateChat error. " + updateResult.Error );
						Console.WriteLine( "Matches: UpdateMatch: Chats.UpdateChat error. " + updateResult.Error );
					}
				}

				//event after update
				UpdateMatchAfter?.Invoke( match, oldMatchData );

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult { Error = e.Message };
			}
		}

		public delegate void UpdateMatchCheckAccessRightsDelegate( long callerID, Match match, ref string status, ref string name, ref string anyData, ref bool allow );
		public static event UpdateMatchCheckAccessRightsDelegate UpdateMatchCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static void UpdateMatch( ServerNetworkService_CloudFunctions.CallMethodContext context, long matchID, string status, string name, string anyData )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Match service is not initialized." );

			//check input
			if( name != null && name.Length > MatchNameMaxLength )
				throw new Exception( $"The match name length exceeds the maximum allowed. Limit: {MatchNameMaxLength} characters." );
			if( anyData != null && anyData.Length > MatchAnyDataMaxLength )
				throw new Exception( $"The match anyData length exceeds the maximum allowed. Limit: {MatchAnyDataMaxLength} characters." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//get match
			var getMatchResult = GetMatch( matchID );
			if( !string.IsNullOrEmpty( getMatchResult.Error ) )
				throw new Exception( getMatchResult.Error );
			if( getMatchResult.NotFound )
				throw new Exception( "Match not found." );
			var match = getMatchResult.Match;

			//check access rights
			var allow = match.UserID == callerID;
			UpdateMatchCheckAccessRights?.Invoke( callerID, match, ref status, ref name, ref anyData, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//update
			var result = UpdateMatch( matchID, status, name, anyData );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );
		}

		///////////////////////////////////////////////
		// MatchUser management

		public class GetMatchUsersResult
		{
			public MatchUser[] Users;
			public string Error;
		}

		public static GetMatchUsersResult GetMatchUsers( long matchID )
		{
			try
			{
				if( !initialized )
					return new GetMatchUsersResult { Error = "Match service is not initialized." };

				var queryable = matchUsersCollection.Query().Where( Query.EQ( "MatchID", matchID ) );
				queryable = queryable.OrderBy( "EnterTime", Query.Ascending );
				var users = queryable.ToArray();

				return new GetMatchUsersResult { Users = users };
			}
			catch( Exception e )
			{
				return new GetMatchUsersResult { Error = e.Message };
			}
		}

		public class GetMatchUserResult
		{
			public MatchUser User;
			public bool NotFound;
			public string Error;
		}

		public static GetMatchUserResult GetMatchUser( long matchID, long userID )
		{
			try
			{
				if( !initialized )
					return new GetMatchUserResult { Error = "Match service is not initialized." };

				var matchUser = matchUsersCollection.Query()
					.Where( Query.And( Query.EQ( "MatchID", matchID ), Query.EQ( "UserID", userID ) ) )
					.FirstOrDefault();
				if( matchUser == null )
					return new GetMatchUserResult { NotFound = true };

				return new GetMatchUserResult { User = matchUser };
			}
			catch( Exception e )
			{
				return new GetMatchUserResult { Error = e.Message };
			}
		}

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static MatchUser GetMatchUserOfCaller( ServerNetworkService_CloudFunctions.CallMethodContext context, long matchID )
		{
			//check initialized
			if( !initialized )
				throw new Exception( "Match service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new Exception( "Client is not logged in." );

			//check match exists
			var getMatchResult = GetMatch( matchID );
			if( !string.IsNullOrEmpty( getMatchResult.Error ) )
				throw new Exception( getMatchResult.Error );
			if( getMatchResult.NotFound )
				throw new Exception( "Match not found." );

			//get match user
			var getMatchUserResult = GetMatchUser( matchID, callerID );
			if( !string.IsNullOrEmpty( getMatchUserResult.Error ) )
				throw new Exception( getMatchUserResult.Error );

			if( getMatchUserResult.NotFound )
			{
				//!!!!null is not supported
				return new MatchUser();
			}

			return getMatchUserResult.User;
		}

		static long GetUniqueMatchUserID()
		{
			var random = new FastRandom();
			for( var digits = 6; digits < 20; digits++ )
			{
				for( int attempts = 0; attempts < 10; attempts++ )
				{
					long id = random.Next( (long)Math.Pow( 10, digits - 1 ), (long)Math.Pow( 10, digits ) - 1 );
					if( matchUsersCollection.FindById( id ) == null )
						return id;
				}
			}
			return 0;
		}

		public delegate void NewMatchUserBeforeDelegate( long userID, ref string username, ref string anyData, ref string error );
		public static event NewMatchUserBeforeDelegate NewMatchUserBefore;

		public delegate void NewMatchUserAfterDelegate( MatchUser matchUser );
		public static event NewMatchUserAfterDelegate NewMatchUserAfter;

		public class AddMatchUserResult
		{
			public MatchUser User;
			public string Error;
		}

		public static AddMatchUserResult AddMatchUser( long matchID, long userID, string username, string anyData )
		{
			try
			{
				if( !initialized )
					return new AddMatchUserResult { Error = "Match service is not initialized." };

				if( anyData != null && anyData.Length > MatchUserAnyDataMaxLength )
					return new AddMatchUserResult { Error = $"The match user anyData length exceeds the maximum allowed. Limit: {MatchUserAnyDataMaxLength} characters." };

				string error = null;
				NewMatchUserBefore?.Invoke( userID, ref username, ref anyData, ref error );
				if( !string.IsNullOrEmpty( error ) )
					return new AddMatchUserResult { Error = error };

				var user = new MatchUser();
				lock( matchUsersCollectionNewItemLock )
				{
					user.Id = GetUniqueMatchUserID();
					user.EnterTime = DateTime.UtcNow;
					user.MatchID = matchID;
					user.UserID = userID;
					user.Username = username;
					user.AnyData = anyData;
					matchUsersCollection.Insert( user );
				}

				NewMatchUserAfter?.Invoke( user );

				return new AddMatchUserResult { User = user };
			}
			catch( Exception e )
			{
				return new AddMatchUserResult { Error = e.Message };
			}
		}

		public delegate void UpdateMatchUserBeforeDelegate( long matchUserID, ref string anyData, ref string error );
		public static event UpdateMatchUserBeforeDelegate UpdateMatchUserBefore;

		public delegate void UpdateMatchUserAfterDelegate( MatchUser matchUser );//, MatchUser oldMatchUser );
		public static event UpdateMatchUserAfterDelegate UpdateMatchUserAfter;

		public class UpdateMatchUserResult
		{
			public MatchUser User;
			public string Error;
		}

		public static UpdateMatchUserResult UpdateMatchUser( long matchUserID, string anyData = null )
		{
			try
			{
				if( !initialized )
					return new UpdateMatchUserResult { Error = "Match service is not initialized." };

				if( anyData != null && anyData.Length > MatchAnyDataMaxLength )
					return new UpdateMatchUserResult { Error = $"The match user anyData length exceeds the maximum allowed. Limit: {MatchAnyDataMaxLength} characters." };

				string error = null;
				UpdateMatchUserBefore?.Invoke( matchUserID, ref anyData, ref error );
				if( !string.IsNullOrEmpty( error ) )
					return new UpdateMatchUserResult { Error = error };

				MatchUser user = null;
				using( matchUsersCollectionLockManager.LockDisposable( matchUserID ) )
				{
					user = matchUsersCollection.FindById( matchUserID );
					if( user == null )
						return new UpdateMatchUserResult { Error = "Match user not found." };

					if( anyData != null )
						user.AnyData = anyData;

					matchUsersCollection.Update( user );
				}

				UpdateMatchUserAfter?.Invoke( user );

				return new UpdateMatchUserResult { User = user };
			}
			catch( Exception e )
			{
				return new UpdateMatchUserResult { Error = e.Message };
			}
		}

		public delegate void RemoveMatchUserBeforeDelegate( long matchUserID, ref string error );
		public static event RemoveMatchUserBeforeDelegate RemoveMatchUserBefore;

		public delegate void RemoveMatchUserAfterDelegate( MatchUser matchUser );//, MatchUser oldMatchUser );
		public static event RemoveMatchUserAfterDelegate RemoveMatchUserAfter;

		public static SimpleResult RemoveMatchUser( long matchUserID )
		{
			try
			{
				if( !initialized )
					return new SimpleResult { Error = "Match service is not initialized." };

				string error = null;
				RemoveMatchUserBefore?.Invoke( matchUserID, ref error );
				if( !string.IsNullOrEmpty( error ) )
					return new SimpleResult { Error = error };

				MatchUser user;
				using( matchUsersCollectionLockManager.LockDisposable( matchUserID ) )
				{
					user = matchUsersCollection.FindById( matchUserID );
					if( user == null )
						return new SimpleResult { Error = "Match user not found." };

					matchUsersCollection.Delete( matchUserID );
				}

				RemoveMatchUserAfter?.Invoke( user );

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult { Error = e.Message };
			}
		}

		///////////////////////////////////////////////
		// Additional Methods

		public delegate void EnterMatchCheckAccessRightsDelegate( long callerID, Match match, ref string anyData, ref bool allow );
		public static event EnterMatchCheckAccessRightsDelegate EnterMatchCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static long EnterMatch( ServerNetworkService_CloudFunctions.CallMethodContext context, long matchID, string anyData )
		{
			//check initialized
			if( !initialized )
				throw new Exception( "Match service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new Exception( "Client is not logged in." );

			//check match exists
			var getMatchResult = GetMatch( matchID );
			if( !string.IsNullOrEmpty( getMatchResult.Error ) )
				throw new Exception( getMatchResult.Error );
			if( getMatchResult.NotFound )
				throw new Exception( "Match not found." );
			var match = getMatchResult.Match;

			//check match status
			if( match.Status == "Deleted" )
				throw new Exception( "Match is deleted." );

			//check if user already joined
			var getMatchUserResult = GetMatchUser( matchID, callerID );
			if( !string.IsNullOrEmpty( getMatchUserResult.Error ) )
				throw new Exception( getMatchUserResult.Error );
			if( !getMatchUserResult.NotFound )
				throw new Exception( "User already entered the match." );

			//check permissions
			var allow = true;
			EnterMatchCheckAccessRights?.Invoke( callerID, match, ref anyData, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//add user to match
			var addMatchUserResult = AddMatchUser( matchID, callerID, context.Client.LoginDataUsername, anyData );
			if( !string.IsNullOrEmpty( addMatchUserResult.Error ) )
				throw new Exception( addMatchUserResult.Error );

			////add user to match chat
			//if( MatchChatEnabled && match.ChatID != 0 )
			//{

			//что с чатами. пермишены проверять или добавить юзеров в чатах
			//"к чату присоединился", "вышел из чата" и т.д.

			//var addChatUserResult = Chats.AddChatUser( match.ChatID, callerID, context.Client.LoginDataUsername, anyData: null );
			//if( !string.IsNullOrEmpty( addChatUserResult.Error ) )
			//{
			//	//log error but do not prevent joining the match
			//	ServerLogs.Write( "Cloud Functions", $"Matches: JoinMatch: Add user to chat error. {addChatUserResult.Error}" );
			//	Console.WriteLine( $"Matches: JoinMatch: Add user to chat error. {addChatUserResult.Error}" );
			//}

			//}

			return addMatchUserResult.User.Id;
		}

		public delegate void RemoveMatchUserCheckAccessRightsDelegate( long callerID, Match match, MatchUser matchUser, ref bool allow );
		public static event RemoveMatchUserCheckAccessRightsDelegate RemoveMatchUserCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static void RemoveMatchUser( ServerNetworkService_CloudFunctions.CallMethodContext context, long matchID, long userID )
		{
			//check initialized
			if( !initialized )
				throw new Exception( "Match service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new Exception( "Client is not logged in." );

			//get match
			var getMatchResult = GetMatch( matchID );
			if( !string.IsNullOrEmpty( getMatchResult.Error ) )
				throw new Exception( getMatchResult.Error );
			var match = getMatchResult.Match;

			//get match user
			var getMatchUserResult = GetMatchUser( matchID, userID );
			if( !string.IsNullOrEmpty( getMatchUserResult.Error ) )
				throw new Exception( getMatchUserResult.Error );
			if( getMatchUserResult.NotFound )
				throw new Exception( "User is not a member of the match." );
			var matchUser = getMatchUserResult.User;

			//check permissions
			var allow = callerID == userID || callerID == match.UserID;
			RemoveMatchUserCheckAccessRights?.Invoke( callerID, match, matchUser, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//remove user from match
			var result = RemoveMatchUser( matchUser.Id );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );
		}

		//[CloudMethod( MaxCallPerClientPermit = 100 )]
		//public static void LeaveMatch( ServerNetworkService_CloudFunctions.CallMethodContext context, long matchID )
		//{
		//	//check initialized
		//	if( !initialized )
		//		throw new Exception( "Match service is not initialized." );

		//	//get caller
		//	var callerID = context.Client.LoginDataUserID;
		//	if( callerID == 0 )
		//		throw new Exception( "Client is not logged in." );

		//	//find match user entry
		//	var matchUser = matchUsersCollection.Query()
		//		.Where( Query.And( Query.EQ( "MatchID", matchID ), Query.EQ( "UserID", callerID ) ) )
		//		.FirstOrDefault();
		//	if( matchUser == null )
		//		throw new Exception( "User is not a member of the match." );

		//	//remove user from match
		//	var result = RemoveMatchUser( matchUser.Id );
		//	if( !string.IsNullOrEmpty( result.Error ) )
		//		throw new Exception( result.Error );
		//}
	}
}
#endif