// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis;
using NeoAxis.Networking;
#if !NO_SERVER
using NeoAxis.LiteDB;
#endif

namespace NeoAxis.CloudServer
{
	public static class Chats
	{
#if !NO_SERVER
		//settings
		public static int ChatMaxCountPerUser { get; set; } = 1000;
		public static int ChatNameMaxLength { get; set; } = 200;
		public static int ChatAnyDataMaxLength { get; set; } = 1000;
		public static int ChatMaxMessageCount { get; set; } = 10000;
		public static int MessageTextMaxLength { get; set; } = 10000;
		public static int MessageAttachmentsMaxTextLength { get; set; } = 1000;
		//public static int MessageAttachmentsMaxItemCount { get; set; } = 10;
		public static int MessageAnyDataMaxLength { get; set; } = 1000;
		public static int GetMessagesMaxCountLimit { get; set; } = int.MaxValue;

		static bool initialized;

		public static ILiteCollection<Chat> chatsCollection;
		public static ConcurrentLockManager<long> chatsCollectionLockManager = new ConcurrentLockManager<long>();
		static object chatsCollectionNewItemLock = new object();

		public static ILiteCollection<Message> messagesCollection;
		public static ConcurrentLockManager<long> messagesCollectionLockManager = new ConcurrentLockManager<long>();
		static object messagesCollectionNewItemLock = new object();
#endif

		///////////////////////////////////////////////

		public class Chat
		{
			//simple structure to support cloud method GetChats

			public long Id { get; set; }
			public DateTime CreationTime { get; set; }
			public string Status { get; set; } //"Open", "Closed", "Deleted" or owner defined
			public DateTime DeletionTime { get; set; }
			public long UserID { get; set; }
			public string Name { get; set; }
			public string Group { get; set; }
			public string AnyData { get; set; }
		}

		///////////////////////////////////////////////

		//for Matches and Technical Support no sense to have list of chat users.
		//when need?

		//!!!!//ChatUser

		///////////////////////////////////////////////

		public class Message
		{
			//simple structure to support cloud method GetMessages

			public long Id { get; set; }
			public DateTime CreationTime { get; set; }
			public string Status { get; set; } //"Enabled", "Deleted"
			public DateTime DeletionTime { get; set; }
			public long UserID { get; set; }
			public string Username { get; set; }
			public long ChatID { get; set; }
			public string Text { get; set; }
			public string Attachments { get; set; }
			public string AnyData { get; set; }
		}

		///////////////////////////////////////////////

#if !NO_SERVER
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
				ServerLogs.Write( "Cloud Functions", "ChatService: Initialize: Database is not initialized." );
				Console.WriteLine( "ChatService: Initialize: Database is not initialized." );
				return;
			}

			//get and configure Chats collection
			chatsCollection = database.GetCollection<Chat>( "Chats" );
			chatsCollection.EnsureIndex( x => x.CreationTime );
			chatsCollection.EnsureIndex( x => x.Status );
			chatsCollection.EnsureIndex( x => x.UserID );
			chatsCollection.EnsureIndex( x => x.Group );

			//get and configure Messages collection
			messagesCollection = database.GetCollection<Message>( "Messages" );
			messagesCollection.EnsureIndex( x => x.CreationTime );
			messagesCollection.EnsureIndex( x => x.Status );
			messagesCollection.EnsureIndex( x => x.UserID );
			messagesCollection.EnsureIndex( x => x.ChatID );

			//register cloud methods
			CloudFunctionsServer.ServerNode.CloudFunctions.RegisterCloudMethods( typeof( Chats ), out var error );
			if( !string.IsNullOrEmpty( error ) )
			{
				ServerLogs.Write( "Cloud Functions", "ChatService: Initialize: RegisterCloudMethods error. " + error );
				Console.WriteLine( "ChatService: Initialize: RegisterCloudMethods error. " + error );
			}

			initialized = true;
		}

		public static void Update( DateTime utcNow )
		{
			if( !initialized )
				return;

		}

		///////////////////////////////////////////////
		//chats


		//!!!!? optimized variant to return less amount of data in the list. return only IDs and names

		//!!!! pages
		//!!!!? DateTime? creationTimeStart = null, DateTime? creationTimeEnd = null


		public class GetChatsResult
		{
			public Chat[] Chats;
			public string Error;
		}

		public static GetChatsResult GetChats( long[] chats = null, long[] users = null, string[] groups = null, string[] statuses = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetChatsResult { Error = "Chat service is not initialized." };

				var queriesAnd = new List<BsonExpression>();

				if( chats != null && chats.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var chatID in chats )
						queriesOr.Add( Query.EQ( "_id", chatID ) );
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

				ILiteQueryable<Chat> queryable;
				if( queriesAnd.Count > 1 )
					queryable = chatsCollection.Query().Where( Query.And( queriesAnd.ToArray() ) );
				else if( queriesAnd.Count == 1 )
					queryable = chatsCollection.Query().Where( queriesAnd[ 0 ] );
				else
					queryable = chatsCollection.Query();

				queryable = queryable.OrderBy( "CreationTime", Query.Ascending );

				return new GetChatsResult() { Chats = queryable.ToArray() };
			}
			catch( Exception e )
			{
				return new GetChatsResult { Error = e.Message };
			}
		}

		public class GetChatResult
		{
			public Chat Chat;
			public bool NotFound;
			public string Error;
		}

		public static GetChatResult GetChat( long chatID )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetChatResult { Error = "Chat service is not initialized." };

				var chat = chatsCollection.FindById( chatID );
				if( chat == null )
					return new GetChatResult { NotFound = true };

				return new GetChatResult() { Chat = chat };

			}
			catch( Exception e )
			{
				return new GetChatResult { Error = e.Message };
			}
		}


		public delegate void GetChatsCheckAccessRightsDelegate( long callerID, long[] chats, long[] users, string[] groups, string[] statuses, ref bool allow );
		public static event GetChatsCheckAccessRightsDelegate GetChatsCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static Chat[] GetChats( ServerNetworkService_CloudFunctions.CallMethodContext context, long[] chats, long[] users, string[] groups, string[] statuses )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Chat service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//check permissions
			var allow = users != null && users.Length == 1 && users[ 0 ] == callerID;
			GetChatsCheckAccessRights?.Invoke( callerID, chats, users, groups, statuses, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//get chats
			var result = GetChats( chats, users, groups, statuses );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );

			return result.Chats;
		}

		public class GetChatCountResult
		{
			public int Count;
			public string Error;
		}

		public static GetChatCountResult GetChatCount( long[] chats = null, long[] users = null, string[] groups = null, string[] statuses = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetChatCountResult { Error = "Chat service is not initialized." };

				//query

				var queriesAnd = new List<BsonExpression>();

				if( chats != null && chats.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var chatID in chats )
						queriesOr.Add( Query.EQ( "_id", chatID ) );
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

				ILiteQueryable<Chat> queryable;
				if( queriesAnd.Count > 1 )
					queryable = chatsCollection.Query().Where( Query.And( queriesAnd.ToArray() ) );
				else if( queriesAnd.Count == 1 )
					queryable = chatsCollection.Query().Where( queriesAnd[ 0 ] );
				else
					queryable = chatsCollection.Query();

				return new GetChatCountResult() { Count = queryable.Count() };
			}
			catch( Exception e )
			{
				return new GetChatCountResult { Error = e.Message };
			}
		}

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static int GetChatCount( ServerNetworkService_CloudFunctions.CallMethodContext context, long[] chats, long[] users, string[] groups, string[] statuses )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Chat service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//check permissions
			var allow = users != null && users.Length == 1 && users[ 0 ] == callerID;
			GetChatsCheckAccessRights?.Invoke( callerID, chats, users, groups, statuses, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//get count
			var result = GetChatCount( chats, users, groups, statuses );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );

			return result.Count;
		}

		static long GetUniqueChatID()
		{
			var random = new FastRandom();
			for( var digits = 4; digits < 20; digits++ )
			{
				for( int attempts = 0; attempts < 10; attempts++ )
				{
					long id = random.Next( (long)Math.Pow( 10, digits - 1 ), (long)Math.Pow( 10, digits ) - 1 );
					if( chatsCollection.FindById( id ) == null )
						return id;
				}
			}
			return 0;
		}

		public delegate void NewChatBeforeDelegate( long userID, ref string name, ref string group, ref string anyData, ref string error );
		public static event NewChatBeforeDelegate NewChatBefore;

		public delegate void NewChatAfterDelegate( Chat chat );
		public static event NewChatAfterDelegate NewChatAfter;

		public class NewChatResult
		{
			public Chat Chat;
			public string Error;
		}

		public static NewChatResult NewChat( long userID, string name, string group = null, string anyData = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new NewChatResult { Error = "Chat service is not initialized." };

				//check input
				if( ChatMaxCountPerUser >= 0 )
				{
					var getCountResult = GetChatCount( users: new[] { userID } );
					if( !string.IsNullOrEmpty( getCountResult.Error ) )
						return new NewChatResult { Error = getCountResult.Error };
					if( getCountResult.Count >= ChatMaxCountPerUser )
						return new NewChatResult { Error = $"The maximum number of chats has been reached. Limit: {ChatMaxCountPerUser} chats." };
				}
				if( name.Length > ChatNameMaxLength )
					return new NewChatResult { Error = $"The chat name length exceeds the maximum allowed. Limit: {ChatNameMaxLength} characters." };
				if( anyData != null && anyData.Length > ChatAnyDataMaxLength )
					return new NewChatResult { Error = $"The chat anyData length exceeds the maximum allowed. Limit: {ChatAnyDataMaxLength} characters." };

				//event before new chat
				string error = null;
				NewChatBefore?.Invoke( userID, ref name, ref group, ref anyData, ref error );
				if( !string.IsNullOrEmpty( error ) )
					return new NewChatResult { Error = error };

				//add to the database
				var chat = new Chat();
				lock( chatsCollectionNewItemLock )
				{
					chat.Id = GetUniqueChatID();
					chat.CreationTime = DateTime.UtcNow;
					chat.Status = "Open";
					chat.UserID = userID;
					chat.Name = name;
					chat.Group = group;
					chat.AnyData = anyData;
					chatsCollection.Insert( chat );
				}

				//event after new chat
				NewChatAfter?.Invoke( chat );

				return new NewChatResult { Chat = chat };
			}
			catch( Exception e )
			{
				return new NewChatResult { Error = e.Message };
			}
		}

		public delegate void NewChatCheckAccessRightsDelegate( long callerID, ref string name, ref string group, ref string anyData, ref bool allow );
		public static event NewChatCheckAccessRightsDelegate NewChatCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static long NewChat( ServerNetworkService_CloudFunctions.CallMethodContext context, string name, string group, string anyData )
		{
			//check initialized
			if( !initialized )
				throw new Exception( "Chat service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new Exception( "Client is not logged in." );

			//check access rights
			var allow = true;
			NewChatCheckAccessRights?.Invoke( callerID, ref name, ref group, ref anyData, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//call create chat
			var result = NewChat( callerID, name, group, anyData );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );

			return result.Chat.Id;
		}

		public delegate void UpdateChatBeforeDelegate( long chatID, ref string name, ref string status, ref string anyData, ref string error );
		public static event UpdateChatBeforeDelegate UpdateChatBefore;

		public delegate void UpdateChatAfterDelegate( Chat chat );
		public static event UpdateChatAfterDelegate UpdateChatAfter;

		public static SimpleResult UpdateChat( long chatID, string status = null, string name = null, string anyData = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new SimpleResult { Error = "Chat service is not initialized." };

				//check input
				if( name != null && name.Length > ChatNameMaxLength )
					return new SimpleResult() { Error = $"The chat name length exceeds the maximum allowed. Limit: {ChatNameMaxLength} characters." };
				if( anyData != null && anyData.Length > ChatAnyDataMaxLength )
					return new SimpleResult() { Error = $"The chat anyData length exceeds the maximum allowed. Limit: {ChatAnyDataMaxLength} characters." };

				//event before update
				string error = null;
				UpdateChatBefore?.Invoke( chatID, ref name, ref status, ref anyData, ref error );
				if( !string.IsNullOrEmpty( error ) )
					return new SimpleResult { Error = error };

				Chat chat = null;
				using( chatsCollectionLockManager.LockDisposable( chatID ) )
				{
					chat = chatsCollection.FindById( chatID );
					if( chat == null )
						return new SimpleResult { Error = "Chat not found." };

					if( status != null )
					{
						var oldStatus = chat.Status;
						chat.Status = status;
						if( status == "Deleted" && oldStatus != "Deleted" )
							chat.DeletionTime = DateTime.UtcNow;
					}
					if( name != null )
						chat.Name = name;
					if( anyData != null )
						chat.AnyData = anyData;

					chatsCollection.Update( chat );
				}

				//event after update
				UpdateChatAfter?.Invoke( chat );

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult { Error = e.Message };
			}
		}

		public delegate void UpdateChatCheckAccessRightsDelegate( long callerID, Chat chat, ref string status, ref string name, ref string anyData, ref bool allow );
		public static event UpdateChatCheckAccessRightsDelegate UpdateChatCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static void UpdateChat( ServerNetworkService_CloudFunctions.CallMethodContext context, long chatID, string status, string name, string anyData )
		{
			try
			{
				//check initialized
				if( !initialized )
					throw new InvalidOperationException( "Chat service is not initialized." );

				//check input
				if( name != null && name.Length > ChatNameMaxLength )
					throw new Exception( $"The chat name length exceeds the maximum allowed. Limit: {ChatNameMaxLength} characters." );
				if( anyData != null && anyData.Length > ChatAnyDataMaxLength )
					throw new Exception( $"The chat anyData length exceeds the maximum allowed. Limit: {ChatAnyDataMaxLength} characters." );

				//get caller
				var callerID = context.Client.LoginDataUserID;
				if( callerID == 0 )
					throw new InvalidOperationException( "Client is not logged in." );

				//get chat
				var getChatResult = GetChat( chatID );
				if( !string.IsNullOrEmpty( getChatResult.Error ) )
					throw new Exception( getChatResult.Error );
				if( getChatResult.NotFound )
					throw new Exception( "Chat not found." );
				var chat = getChatResult.Chat;

				//check access rights
				var allow = chat.UserID == callerID;
				UpdateChatCheckAccessRights?.Invoke( callerID, chat, ref status, ref name, ref anyData, ref allow );
				if( !allow )
					throw new Exception( "Access denied." );

				//update
				var result = UpdateChat( chatID, status, name, anyData );
				if( !string.IsNullOrEmpty( result.Error ) )
					throw new Exception( result.Error );
			}
			catch( Exception e )
			{
				Console.WriteLine( "UpdateChat exception: " + e.ToString() );
				throw new Exception( e.ToString() );
			}
		}

		///////////////////////////////////////////////
		//messages


		//!!!!same as GetChats optimizations?


		public class GetMessagesResult
		{
			public Message[] Messages;
			public string Error;
		}

		public static GetMessagesResult GetMessages( long chatID, string[] statuses, DateTime timeFrom, DateTime timeTo, int maxCount = int.MaxValue, bool getFromEnd = false )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetMessagesResult() { Error = "Chat service is not initialized." };

				//query

				var queriesAnd = new List<BsonExpression>();

				//chatID
				queriesAnd.Add( Query.EQ( "ChatID", chatID ) );

				//statuses
				if( statuses != null && statuses.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var status in statuses )
						queriesOr.Add( Query.EQ( "Status", status ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}

				//timeFrom, timeEnd
				if( timeFrom != DateTime.MinValue )
					queriesAnd.Add( Query.GTE( "CreationTime", timeFrom ) );
				if( timeTo != DateTime.MinValue )
					queriesAnd.Add( Query.LTE( "CreationTime", timeTo ) );

				ILiteQueryable<Message> queryable;
				if( queriesAnd.Count > 1 )
					queryable = messagesCollection.Query().Where( Query.And( queriesAnd.ToArray() ) );
				else if( queriesAnd.Count == 1 )
					queryable = messagesCollection.Query().Where( queriesAnd[ 0 ] );
				else
					queryable = messagesCollection.Query();

				queryable = queryable.OrderBy( "CreationTime", Query.Ascending );

				//apply maxCount/getFromEnd
				if( maxCount != int.MaxValue )
				{
					if( getFromEnd )
					{
						var totalCount = queryable.Count();
						var skip = Math.Max( 0, totalCount - maxCount );
						var sliced = queryable.Offset( skip ).Limit( maxCount );
						return new GetMessagesResult { Messages = sliced.ToArray() };
					}
					else
					{
						var limited = queryable.Limit( maxCount );
						return new GetMessagesResult { Messages = limited.ToArray() };
					}
				}
				else
					return new GetMessagesResult { Messages = queryable.ToArray() };
			}
			catch( Exception e )
			{
				return new GetMessagesResult { Error = e.Message };
			}
		}

		public delegate void GetMessagesCheckAccessRightsDelegate( long callerID, Chat chat, ref string[] statuses, ref DateTime timeFrom, ref DateTime timeTo, ref int maxCount, ref bool getFromEnd, ref bool allow );
		//public delegate void GetMessagesCheckAccessRightsDelegate( long callerID, Chat chat, ref string[] statuses, ref bool allow );
		public static event GetMessagesCheckAccessRightsDelegate GetMessagesCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static Message[] GetMessages( ServerNetworkService_CloudFunctions.CallMethodContext context, long chatID, string[] statuses, DateTime timeFrom, DateTime timeTo, int maxCount, bool getFromEnd )
		//public static Message[] GetMessages( ServerNetworkService_CloudFunctions.CallMethodContext context, long chatID, string[] statuses )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Chat service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//check input
			if( maxCount <= 0 )
				throw new ArgumentOutOfRangeException( nameof( maxCount ), "maxCount must be greater than 0." );
			if( maxCount > GetMessagesMaxCountLimit )
				throw new ArgumentOutOfRangeException( nameof( maxCount ), $"maxCount must be between 0 and {GetMessagesMaxCountLimit}." );

			//get chat
			var getChatResult = GetChat( chatID );
			if( !string.IsNullOrEmpty( getChatResult.Error ) )
				throw new Exception( getChatResult.Error );
			if( getChatResult.NotFound )
				throw new Exception( "Chat not found." );
			var chat = getChatResult.Chat;

			//check permissions
			var allow = chat.UserID == callerID;
			GetMessagesCheckAccessRights?.Invoke( callerID, chat, ref statuses, ref timeFrom, ref timeTo, ref maxCount, ref getFromEnd, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//get messages
			var getMessagesResult = GetMessages( chatID, statuses, timeFrom, timeTo, maxCount, getFromEnd );
			if( !string.IsNullOrEmpty( getMessagesResult.Error ) )
				throw new Exception( getMessagesResult.Error );

			return getMessagesResult.Messages;
		}

		public class GetMessageCountResult
		{
			public int Count;
			public string Error;
		}

		public static GetMessageCountResult GetMessageCount( long chatID, string[] statuses = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetMessageCountResult() { Error = "Chat service is not initialized." };

				//query

				var queriesAnd = new List<BsonExpression>();
				queriesAnd.Add( Query.EQ( "ChatID", chatID ) );
				if( statuses != null && statuses.Length > 0 )
				{
					var queriesOr = new List<BsonExpression>();
					foreach( var status in statuses )
						queriesOr.Add( Query.EQ( "Status", status ) );
					queriesAnd.Add( queriesOr.Count > 1 ? Query.Or( queriesOr.ToArray() ) : queriesOr[ 0 ] );
				}

				ILiteQueryable<Message> queryable;
				if( queriesAnd.Count > 1 )
					queryable = messagesCollection.Query().Where( Query.And( queriesAnd.ToArray() ) );
				else if( queriesAnd.Count == 1 )
					queryable = messagesCollection.Query().Where( queriesAnd[ 0 ] );
				else
					queryable = messagesCollection.Query();

				return new GetMessageCountResult { Count = queryable.Count() };
			}
			catch( Exception e )
			{
				return new GetMessageCountResult { Error = e.Message };
			}
		}

		public class GetMessageResult
		{
			public Message Message;
			public bool NotFound;
			public string Error;
		}

		public static GetMessageResult GetMessage( long messageID )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new GetMessageResult() { Error = "Chat service is not initialized." };

				//query
				var message = messagesCollection.FindById( messageID );
				if( message == null )
					return new GetMessageResult { NotFound = true };

				return new GetMessageResult { Message = message };
			}
			catch( Exception e )
			{
				return new GetMessageResult { Error = e.Message };
			}
		}

		static long GetUniqueMessageID()
		{
			var random = new FastRandom();
			for( var digits = 6; digits < 20; digits++ )
			{
				for( int attempts = 0; attempts < 10; attempts++ )
				{
					long id = random.Next( (long)Math.Pow( 10, digits - 1 ), (long)Math.Pow( 10, digits ) - 1 );
					if( messagesCollection.FindById( id ) == null )
						return id;
				}
			}
			return 0;
		}

		public delegate void NewMessageBeforeDelegate( long userID, long chatID, ref string text, ref string attachments, ref string anyData, ref string error );
		public static event NewMessageBeforeDelegate NewMessageBefore;

		public delegate void NewMessageAfterDelegate( Message message );
		public static event NewMessageAfterDelegate NewMessageAfter;

		public class NewMessageResult
		{
			public Message Message;
			public string Error;
		}

		public static NewMessageResult NewMessage( long userID, string username, long chatID, string text, string attachments = null, string anyData = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new NewMessageResult { Error = "Chat service is not initialized." };

				//check input
				if( text.Length > MessageTextMaxLength )
					return new NewMessageResult() { Error = $"The message text length exceeds the maximum allowed. Limit: {MessageTextMaxLength} characters." };
				if( attachments != null && attachments.Length > MessageAttachmentsMaxTextLength )
					return new NewMessageResult() { Error = $"The message attachments length exceeds the maximum allowed. Limit: {MessageAttachmentsMaxTextLength} characters." };
				if( anyData != null && anyData.Length > MessageAnyDataMaxLength )
					return new NewMessageResult() { Error = $"The message anyData length exceeds the maximum allowed. Limit: {MessageAnyDataMaxLength} characters." };
				if( ChatMaxMessageCount >= 0 )
				{
					var getCountResult = GetMessageCount( chatID: chatID );
					if( !string.IsNullOrEmpty( getCountResult.Error ) )
						return new NewMessageResult { Error = getCountResult.Error };
					if( getCountResult.Count >= ChatMaxMessageCount )
						return new NewMessageResult { Error = $"The maximum number of messages in the chat has been reached. Limit: {ChatMaxMessageCount} messages." };
				}

				//event before new message
				var newMessageBefore = NewMessageBefore;
				if( newMessageBefore != null )
				{
					string error = null;
					newMessageBefore( userID, chatID, ref text, ref attachments, ref anyData, ref error );
					if( !string.IsNullOrEmpty( error ) )
						return new NewMessageResult { Error = error };
				}

				//add to the database
				var message = new Message();
				lock( messagesCollectionNewItemLock )
				{
					message.Id = GetUniqueMessageID();
					message.CreationTime = DateTime.UtcNow;
					message.Status = "Enabled";
					message.UserID = userID;
					message.Username = username;
					message.ChatID = chatID;
					message.Text = text;
					message.Attachments = attachments;
					message.AnyData = anyData;
					messagesCollection.Insert( message );
				}

				//event after new message
				NewMessageAfter?.Invoke( message );

				return new NewMessageResult { Message = message };
			}
			catch( Exception e )
			{
				return new NewMessageResult { Error = e.Message };
			}
		}

		public delegate void NewMessageCheckAccessRightsDelegate( long callerID, Chat chat, ref string text, ref string attachments, ref string anyData, ref bool allow );
		public static event NewMessageCheckAccessRightsDelegate NewMessageCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static long NewMessage( ServerNetworkService_CloudFunctions.CallMethodContext context, long chatID, string text, string attachments, string anyData )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Chat service is not initialized." );

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );
			var callerUsername = context.Client.LoginDataUsername;

			//get chat
			var getChatResult = GetChat( chatID );
			if( !string.IsNullOrEmpty( getChatResult.Error ) )
				throw new Exception( getChatResult.Error );
			if( getChatResult.NotFound )
				throw new Exception( "Chat not found." );
			var chat = getChatResult.Chat;

			//check chat status
			if( chat.Status == "Deleted" )
				throw new Exception( "Chat is deleted." );
			if( chat.Status == "Closed" )
				throw new Exception( "Chat is closed." );

			//check access rights
			var allow = chat.UserID == callerID;
			NewMessageCheckAccessRights?.Invoke( callerID, chat, ref text, ref attachments, ref anyData, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//new message
			var result = NewMessage( callerID, callerUsername, chatID, text, attachments, anyData );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );

			return result.Message.Id;
		}

		public delegate void UpdateMessageBeforeDelegate( ref long messageID, ref string status, ref string text, ref string attachments, ref string anyData, ref string error );
		public static event UpdateMessageBeforeDelegate UpdateMessageBefore;

		public delegate void UpdateMessageAfterDelegate( Message message );
		public static event UpdateMessageAfterDelegate UpdateMessageAfter;

		public static SimpleResult UpdateMessage( long messageID, string status = null, string text = null, string attachments = null, string anyData = null )
		{
			try
			{
				//check initialized
				if( !initialized )
					return new SimpleResult { Error = "Chat service is not initialized." };

				//check input
				if( text != null && text.Length > MessageTextMaxLength )
					return new SimpleResult() { Error = $"The message text length exceeds the maximum allowed. Limit: {MessageTextMaxLength} characters." };
				if( attachments != null && attachments.Length > MessageAttachmentsMaxTextLength )
					return new SimpleResult() { Error = $"The message attachments length exceeds the maximum allowed. Limit: {MessageAttachmentsMaxTextLength} characters." };
				if( anyData != null && anyData.Length > MessageAnyDataMaxLength )
					return new SimpleResult() { Error = $"The message anyData length exceeds the maximum allowed. Limit: {MessageAnyDataMaxLength} characters." };

				//event before update
				var updateMessageBefore = UpdateMessageBefore;
				if( updateMessageBefore != null )
				{
					string error = null;
					updateMessageBefore( ref messageID, ref status, ref text, ref attachments, ref anyData, ref error );
					if( !string.IsNullOrEmpty( error ) )
						return new SimpleResult { Error = error };
				}

				//update
				Message message;
				using( messagesCollectionLockManager.LockDisposable( messageID ) )
				{
					message = messagesCollection.FindById( messageID );
					if( message == null )
						return new SimpleResult { Error = "Message not found." };

					if( status != null )
					{
						var oldStatus = message.Status;
						message.Status = status;
						if( status == "Deleted" && oldStatus != "Deleted" )
							message.DeletionTime = DateTime.UtcNow;
					}
					if( text != null )
						message.Text = text;
					if( attachments != null )
						message.Attachments = attachments;
					if( anyData != null )
						message.AnyData = anyData;

					messagesCollection.Update( message );
				}

				//event after update
				UpdateMessageAfter?.Invoke( message );

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult { Error = e.Message };
			}
		}

		public delegate void UpdateMessageCheckAccessRightsDelegate( long callerID, Message message, ref string status, ref string text, ref string attachments, ref string anyData, ref bool allow );
		public static event UpdateMessageCheckAccessRightsDelegate UpdateMessageCheckAccessRights;

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public static void UpdateMessage( ServerNetworkService_CloudFunctions.CallMethodContext context, long messageID, string status, string text, string attachments, string anyData )
		{
			//check initialized
			if( !initialized )
				throw new InvalidOperationException( "Chat service is not initialized." );

			//check input already inside UpdateMessage

			//get caller
			var callerID = context.Client.LoginDataUserID;
			if( callerID == 0 )
				throw new InvalidOperationException( "Client is not logged in." );

			//get message
			var getMessageResult = GetMessage( messageID );
			if( !string.IsNullOrEmpty( getMessageResult.Error ) )
				throw new Exception( getMessageResult.Error );
			if( getMessageResult.NotFound )
				throw new Exception( "Message not found." );
			var message = getMessageResult.Message;

			//check access rights
			var allow = message.UserID == callerID;
			UpdateMessageCheckAccessRights?.Invoke( callerID, message, ref status, ref text, ref attachments, ref anyData, ref allow );
			if( !allow )
				throw new Exception( "Access denied." );

			//update
			var result = UpdateMessage( messageID, status, text, attachments, anyData );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );
		}

#endif
	}
}