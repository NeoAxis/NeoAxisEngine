#if !NO_SERVER
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis.LiteDB;
using NeoAxis;
using NeoAxis.Networking;

namespace NeoAxis.CloudServer
{
	/// <summary>
	/// Internal transactions management class.
	/// </summary>
	public static class Transactions
	{
		public static int TransactionAnyDataMaxLength { get; set; } = 1000;

		static bool initialized;
		//public static ILiteCollection<Group> groupsCollection;
		//static object groupsCollectionNewItemLock = new object();

		//public static ILiteCollection<UserItem> usersCollection;
		//public static ConcurrentLockManager<long> usersCollectionLockManager = new ConcurrentLockManager<long>();

		public static ILiteCollection<Transaction> transactionsCollection;
		public static ConcurrentLockManager<long> transactionsCollectionLockManager = new ConcurrentLockManager<long>();
		static object transactionsCollectionNewItemLock = new object();

		///////////////////////////////////////////////

		//public class Group
		//{
		//	public long Id { get; set; }
		//	public DateTime CreationTime { get; set; }
		//	//!!!!Deleted? DeletionTime
		//	public long UserID { get; set; }
		//	public string Name { get; set; }
		//	public string AnyData { get; set; }
		//}

		///////////////////////////////////////////////

		//public class UserItem
		//{
		//	public long Id { get; set; }
		//	public string AnyData { get; set; }
		//}

		///////////////////////////////////////////////

		public class Transaction
		{
			public long Id { get; set; }
			public DateTime CreationTime { get; set; }
			public long UserID { get; set; }
			//public long GroupID { get; set; }
			//Type: Task ////Type: TopUp, Withdraw, Task
			public string Type { get; set; }
			public double Amount { get; set; }
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

			var database = CloudFunctionsServer.ServerNode?.CloudFunctions?.DatabaseImpl?.Database;
			if( database == null )
			{
				ServerLogs.Write( "Cloud Functions", "Transactions: Initialize: Database is not initialized." );
				Console.WriteLine( "Transactions: Initialize: Database is not initialized." );
				return;
			}

			//groupsCollection = database.GetCollection<Group>( "Groups" );
			//groupsCollection.EnsureIndex( x => x.CreationTime );
			//groupsCollection.EnsureIndex( x => x.UserID );

			transactionsCollection = database.GetCollection<Transaction>( "Transactions" );
			transactionsCollection.EnsureIndex( x => x.CreationTime );
			transactionsCollection.EnsureIndex( x => x.UserID );
			//transactionsCollection.EnsureIndex( x => x.GroupID );
			transactionsCollection.EnsureIndex( x => x.Type );
			transactionsCollection.EnsureIndex( x => x.AnyData );

			//usersCollection = database.GetCollection<UserItem>( "Users" );

			//register call methods 
			CloudFunctionsServer.ServerNode.CloudFunctions.RegisterCloudMethods( typeof( Transactions ), out var error );
			if( !string.IsNullOrEmpty( error ) )
			{
				ServerLogs.Write( "Cloud Functions", "Transactions: Initialize: RegisterCallMethods error. " + error );
				Console.WriteLine( "Transactions: Initialize: RegisterCallMethods error. " + error );
			}

			initialized = true;
		}

		///////////////////////////////////////////////
		//groups

		//public class GetGroupsResult
		//{
		//	public Group[] Groups;
		//	public string Error;
		//}

		//[CloudMethod]
		//public static GetGroupsResult GetGroups( long? userID = null )
		//{
		//	if( !initialized )
		//		return new GetGroupsResult { Error = "Transactions class is not initialized." };

		//	try
		//	{
		//		var queriesAnd = new List<BsonExpression>();
		//		if( userID != null )
		//			queriesAnd.Add( Query.EQ( "UserID", userID.Value ) );

		//		ILiteQueryable<Group> queryable;
		//		if( queriesAnd.Count > 1 )
		//			queryable = groupsCollection.Query().Where( Query.And( queriesAnd.ToArray() ) );
		//		else if( queriesAnd.Count == 1 )
		//			queryable = groupsCollection.Query().Where( queriesAnd[ 0 ] );
		//		else
		//			queryable = groupsCollection.Query();
		//		queryable = queryable.OrderBy( "CreationTime", Query.Ascending );

		//		var groups = queryable.ToArray();

		//		return new GetGroupsResult { Groups = groups };
		//	}
		//	catch( Exception e )
		//	{
		//		return new GetGroupsResult { Error = e.Message };
		//	}
		//}

		//public class NewGroupResult
		//{
		//	public Group Group;
		//	public string Error;
		//}

		//static long GetUniqueGroupID()
		//{
		//	var random = new FastRandom();
		//	for( var digits = 4; digits < 20; digits++ )
		//	{
		//		for( int attempts = 0; attempts < 10; attempts++ )
		//		{
		//			long id = random.Next( (long)Math.Pow( 10, digits - 1 ), (long)Math.Pow( 10, digits ) - 1 );
		//			if( groupsCollection.FindById( id ) == null )
		//				return id;
		//		}
		//	}
		//	return 0;
		//}

		//public static NewGroupResult NewGroup( long userID, string name, string anyData = null )
		//{
		//	if( !initialized )
		//		return new NewGroupResult { Error = "Transactions class is not initialized." };

		//	try
		//	{
		//		lock( groupsCollectionNewItemLock )
		//		{
		//			var group = new Group();
		//			group.Id = GetUniqueGroupID();
		//			group.CreationTime = DateTime.UtcNow;
		//			group.UserID = userID;
		//			group.Name = name ?? "";
		//			group.AnyData = anyData;
		//			groupsCollection.Insert( group );

		//			return new NewGroupResult { Group = group };
		//		}
		//	}
		//	catch( Exception e )
		//	{
		//		return new NewGroupResult { Error = e.Message };
		//	}
		//}

		//public static SimpleResult DeleteGroups( long[] ids )
		//{
		//	if( !initialized )
		//		return new SimpleResult { Error = "Transactions class is not initialized." };

		//	try
		//	{
		//		foreach( var id in ids )
		//			groupsCollection.Delete( id );

		//		return new SimpleResult();
		//	}
		//	catch( Exception e )
		//	{
		//		return new SimpleResult { Error = e.Message };
		//	}
		//}

		//public static SimpleResult DeleteGroup( long id )
		//{
		//	return DeleteGroups( new long[] { id } );
		//}

		///////////////////////////////////////////////
		//users

		//public class GetUserResult
		//{
		//	public UserItem User;
		//	public bool NotFound;
		//	public string Error;
		//}

		////!!!!forceRefreshBalance. from transactions can get balance

		//public static GetUserResult GetUser( long userID )//, bool forceRefreshBalance = false )
		//{
		//	if( !initialized )
		//		return new GetUserResult { Error = "Transactions class is not initialized." };

		//	try
		//	{
		//		var user = usersCollection.FindOne( Query.EQ( "_id", userID ) );
		//		if( user == null )
		//			return new GetUserResult { NotFound = true };
		//		return new GetUserResult { User = user };
		//	}
		//	catch( Exception e )
		//	{
		//		return new GetUserResult { Error = e.Message };
		//	}
		//}

		//[CloudMethod] //!!!!?
		//public static GetUserResult GetUser( ServerNetworkService_CloudFunctions.CallMethodContext context )
		//{
		//	return GetUser( context.Client.LoginDataUserID );
		//}

		public async static Task<double> GetUserReservedBalance( long userID )
		{
			var topUpWithdrawContribution = 0.0;
			var taskExpenses = 0.0;

			//get top up and withdraw transactions contribution
			{
				//in this call returns the list of transactions with the server. The user ID is detected from serverCheckCode, it is equal to projectUserID. Database request includes "userID == tx.SenderID || userID == tx.RecepientID"

				var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
				var projectUserID = CloudServerProcessUtility.CommandLineParameters.UserID;
				var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;
				using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 2, 0 ) );

				//!!!!maybe add function to general manager to get sum of payments
				//UserTransactionsGetSumProjectInAppPaymentsAsync

				var result = await CloudServiceFunctions.UserTransactionsGetAsync( "ProjectInAppPayment", projectID, "Completed", authToken: serverCheckCode, cancellationToken: cancellationToken.Token );
				if( !string.IsNullOrEmpty( result.Error ) )
					throw new Exception( result.Error );

				foreach( var tx in result.Transactions )
				{
					if( tx.SenderID == userID && tx.RecepientID == projectUserID )
					{
						//this is transaction with top up reserved balance from this user to the project
						topUpWithdrawContribution += tx.Amount;
					}
					else if( tx.SenderID == projectUserID && tx.RecepientID == userID )
					{
						//this is transaction with withdraw reserved balance from the project to this user
						topUpWithdrawContribution -= tx.Amount;
					}
				}
			}

			//get task transactions expenses
			{

				//!!!!slowly when many transactions. merge old transactions into one?

				var getTransactionsResult = GetTransactions( userID: userID );
				if( !string.IsNullOrEmpty( getTransactionsResult.Error ) )
					throw new Exception( getTransactionsResult.Error );

				foreach( var tx in getTransactionsResult.Transactions )
				{
					if( tx.Type == "Task" )
						taskExpenses += tx.Amount;
				}
			}

			//calculate final value
			return topUpWithdrawContribution - taskExpenses;
		}

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public async static Task<double> GetUserReservedBalance( ServerNetworkService_CloudFunctions.CallMethodContext context )
		{
			//check initialized
			if( !initialized )
				throw new Exception( "Transactions service is not initialized." );

			var userID = context.Client.LoginDataUserID;
			return await GetUserReservedBalance( userID );
		}

		//[CloudMethod]
		//public static string GetUserAnyData( ServerNetworkService_CloudFunctions.CallMethodContext context )
		//{
		//	var getResult = GetUser( context );
		//	if( !string.IsNullOrEmpty( getResult.Error ) )
		//		throw new Exception( getResult.Error );
		//	return getResult.User != null ? getResult.User.AnyData : null;
		//}

		[CloudMethod( MaxCallPerClientPermit = 100 )]
		public async static Task<double> GetUserAccountBalance( ServerNetworkService_CloudFunctions.CallMethodContext context )
		{
			var clientData = CloudFunctionsServer.GetClientData( context.Client );
			if( clientData == null )
				throw new Exception( "Client data not found." );

			var verificationCode = clientData.VerificationCode;
			if( string.IsNullOrEmpty( verificationCode ) )
				throw new Exception( "Verification code is empty." ); //Maybe because used Direct connection instead of Cloud connection.

			var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
			using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
			var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;

			var result = await CloudServiceFunctions.AccessGetUserByVerificationCodeAsync( projectID, context.Client.UserRole, verificationCode, true, serverCheckCode, cancellationToken.Token );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );

			return result.Balance;
		}

		//public static void SetUserAnyData( long userID, string anyData )
		//{
		//	using( usersCollectionLockManager.LockDisposable( userID ) )
		//	{
		//		var user = usersCollection.FindById( userID );
		//		if( user != null )
		//		{
		//			user.AnyData = anyData;
		//			usersCollection.Update( user );
		//		}
		//		else
		//		{
		//			user = new UserItem();
		//			user.Id = userID;
		//			user.AnyData = anyData;
		//			usersCollection.Insert( user );
		//		}
		//	}
		//}

		///////////////////////////////////////////////
		//transactions

		public class GetTransactionsResult
		{
			public Transaction[] Transactions;
			public string Error;
		}

		public static GetTransactionsResult GetTransactions( long? transactionID = null, long? userID = null/*, long? groupID = null*/, string? type = null, string? anyData = null )
		{
			try
			{
				var queriesAnd = new List<BsonExpression>();
				if( transactionID != null )
					queriesAnd.Add( Query.EQ( "_id", transactionID.Value ) );
				if( userID != null )
					queriesAnd.Add( Query.EQ( "UserID", userID.Value ) );
				//if( groupID != null )
				//	queriesAnd.Add( Query.EQ( "GroupID", groupID ) );
				if( type != null )
					queriesAnd.Add( Query.EQ( "Type", type ) );
				if( anyData != null )
					queriesAnd.Add( Query.EQ( "AnyData", anyData ) );

				ILiteQueryable<Transaction> queryable;
				if( queriesAnd.Count > 1 )
					queryable = transactionsCollection.Query().Where( Query.And( queriesAnd.ToArray() ) );
				else if( queriesAnd.Count == 1 )
					queryable = transactionsCollection.Query().Where( queriesAnd[ 0 ] );
				else
					queryable = transactionsCollection.Query();
				queryable = queryable.OrderBy( "CreationTime", Query.Ascending );

				var transactions = queryable.ToArray();

				return new GetTransactionsResult { Transactions = transactions };
			}
			catch( Exception e )
			{
				return new GetTransactionsResult { Error = e.Message };
			}
		}

		public class NewTransactionResult
		{
			public Transaction Transaction;
			public string Error;
		}

		static long GetUniqueTransactionID()
		{
			var random = new FastRandom();
			for( var digits = 6; digits < 20; digits++ )
			{
				for( int attempts = 0; attempts < 10; attempts++ )
				{
					long id = random.Next( (long)Math.Pow( 10, digits - 1 ), (long)Math.Pow( 10, digits ) - 1 );
					if( transactionsCollection.FindById( id ) == null )
						return id;
				}
			}
			return 0;
		}

		public static NewTransactionResult NewTransaction( long userID/*, long groupID*/, string type, double amount = 0, string anyData = null )
		{
			//check initialized
			if( !initialized )
				return new NewTransactionResult { Error = "Transactions service is not initialized." };

			//check limits
			if( anyData != null && anyData.Length > TransactionAnyDataMaxLength )
				return new NewTransactionResult { Error = "Too big AnyData value length." };

			//create transaction
			try
			{
				lock( transactionsCollectionNewItemLock )
				{
					var tx = new Transaction();
					tx.Id = GetUniqueTransactionID();
					tx.UserID = userID;
					//tx.GroupID = groupID;
					tx.Type = type;
					tx.Amount = amount;
					tx.AnyData = anyData;
					transactionsCollection.Insert( tx );

					return new NewTransactionResult { Transaction = tx };
				}
			}
			catch( Exception e )
			{
				return new NewTransactionResult { Error = e.Message };
			}
		}

		//!!!!check. never used
		public static SimpleResult UpdateTransaction( long transactionID, string anyData = null )
		{
			//check initialized
			if( !initialized )
				return new SimpleResult { Error = "Transactions service is not initialized." };

			//update transaction
			using( transactionsCollectionLockManager.LockDisposable( transactionID ) )
			{
				var tx = transactionsCollection.FindById( transactionID );
				if( tx == null )
					return new SimpleResult { Error = "Transaction not found." };

				if( anyData != null )
				{
					if( anyData.Length > TransactionAnyDataMaxLength )
						return new SimpleResult { Error = "Too big AnyData value length." };
					tx.AnyData = anyData;
				}

				transactionsCollection.Update( tx );
			}

			return new SimpleResult();
		}

		[CloudMethod( MaxCallPerClientPermit = 10 )]
		public async static Task RequestTopUpReserve( ServerNetworkService_CloudFunctions.CallMethodContext context, double amount )
		{
			var userID = context.Client.LoginDataUserID;
			var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
			var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;
			using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );

			var result = await CloudServiceFunctions.UserTransactionsRequestProjectInAppPurchaseAsync( projectID, userID, amount, null, null, serverCheckCode, cancellationToken.Token );

			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );
		}

		[CloudMethod( MaxCallPerClientPermit = 10 )]
		public async static Task RequestWithdrawReserve( ServerNetworkService_CloudFunctions.CallMethodContext context, double amount )
		{
			var userID = context.Client.LoginDataUserID;

			var reservedBalance = await GetUserReservedBalance( context );
			if( reservedBalance < amount )
				throw new Exception( "Not enough reserved balance." );

			var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
			var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;
			using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );

			var result = await CloudServiceFunctions.UserTransactionsProjectInAppWithdrawAsync( projectID, userID, amount, null, null, serverCheckCode, cancellationToken.Token );
			if( !string.IsNullOrEmpty( result.Error ) )
				throw new Exception( result.Error );
		}

		public static void Update( DateTime utcNow )
		{
			if( !initialized )
				return;

		}
	}
}






//var now = DateTime.UtcNow;
//if( ( now - lastUpdateProcessTopUps ).TotalSeconds > 10 )
//{
//	lastUpdateProcessTopUps = now;

//	Task.Run( async delegate
//	{
//		var error = await TransactionsProcessTopUpsAsync();
//		if( !string.IsNullOrEmpty( error ) )
//		{
//			ServerLogs.Write( "Cloud Functions", "Transactions: Update: TransactionsProcessTopUpsAsync error. " + error );
//			Console.WriteLine( "Transactions: Update: TransactionsProcessTopUpsAsync error. " + error );
//		}
//	} );
//}


////update user balance
//if( amount != 0 )
//{
//	if( type == "TopUp" )
//		ChangeUserReservedBalance( userID, amount );
//	else //if( type == "Withdraw" )
//		ChangeUserReservedBalance( userID, -amount );
//}


//public static void ChangeUserReservedBalance( long userID, double delta )
//{
//	using( usersCollectionLockManager.LockDisposable( userID ) )
//	{
//		var user = usersCollection.FindById( userID );
//		if( user != null )
//		{
//			user.ReservedBalance += delta;
//			usersCollection.Update( user );
//		}
//		else
//		{
//			user = new UserItem();
//			user.Id = userID;
//			user.ReservedBalance = delta;
//			usersCollection.Insert( user );
//		}
//	}
//}


//to use parts of old code:
//public async static Task<string> TransactionsProcessTopUpsAsync()
//{
//	var projectID = CloudServerProcessUtility.CommandLineParameters.ProjectID;
//	var serverCheckCode = CloudServerProcessUtility.CommandLineParameters.ServerCheckCode;
//	using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 2, 0 ) );

//	var result = await GeneralManagerFunctions.UserTransactionsGetAsync( "ProjectInAppPayment", projectID, "Completed", false, serverCheckCode: serverCheckCode, cancellationToken: cancellationToken.Token );
//	//var result = await GeneralManagerFunctions.UserTransactionsGetProjectInAppPaymentsAsync( projectID, null, false, "Completed", serverCheckCode, cancellationToken.Token );

//	if( !string.IsNullOrEmpty( result.Error ) )
//		return result.Error;

//	foreach( var tx in result.Transactions )
//	{
//		//get Payments group or create it
//		Group paymentsGroup = null;
//		{
//			var getResult = GetGroups( tx.SenderID );
//			if( !string.IsNullOrEmpty( getResult.Error ) )
//				return getResult.Error;
//			paymentsGroup = getResult.Groups.FirstOrDefault( g => g.Name == "Payments" );
//		}
//		//GroupItem paymentsGroup = FindGroup( tx.SenderID, "Payments" ).Group;
//		if( paymentsGroup == null )
//		{
//			var addGroupResult = NewGroup( tx.SenderID, "Payments" );
//			if( !string.IsNullOrEmpty( addGroupResult.Error ) )
//				return addGroupResult.Error;
//			paymentsGroup = addGroupResult.Group;
//		}

//		//check that transaction is already added in the project
//		var getTransactionsResult = GetTransactions( null, tx.SenderID, paymentsGroup.Id, "TopUp", tx.Id.ToString() );
//		if( !string.IsNullOrEmpty( getTransactionsResult.Error ) )
//			return getTransactionsResult.Error;

//		//add transaction in the project with TopUp type
//		Transaction transaction;
//		if( getTransactionsResult.Transactions.Length == 0 )
//		{
//			var addResult = NewTransaction( tx.SenderID, paymentsGroup.Id, "TopUp", tx.Amount, tx.Id.ToString() );
//			if( !string.IsNullOrEmpty( addResult.Error ) )
//				return addResult.Error;
//			transaction = addResult.Transaction;
//		}
//		else
//			transaction = getTransactionsResult.Transactions[ 0 ];

//		//update transaction to processed state
//		using var cancellationToken2 = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
//		var result2 = await GeneralManagerFunctions.UserTransactionsUpdateAsync( tx.Id, null, true, transaction.Id.ToString(), serverCheckCode, cancellationToken2.Token );
//		if( !string.IsNullOrEmpty( result2.Error ) )
//			return result2.Error;
//	}

//	return null;
//}
#endif