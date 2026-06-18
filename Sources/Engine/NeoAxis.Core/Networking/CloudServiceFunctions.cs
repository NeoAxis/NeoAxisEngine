// Copyright 2006Ц2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeoAxis.Networking
{
	public static class CloudServiceFunctions
	{
		///////////////////////////////////////////////

		public class SimpleResult
		{
			public string Error;
		}

		///////////////////////////////////////////////

		public static string GetHttpURL()
		{
			var frontServer = NetworkCommonSettings.CloudServiceCurrentFrontServer;
			if( frontServer != null )
			{
				if( frontServer.HttpsPort == 443 )
					return $"https://{frontServer.Domain}";
				else
					return $"https://{frontServer.Domain}:{frontServer.HttpsPort}";
			}

			throw new Exception( "Front server is not selected." );

			//var prefix = "https://";
			//var address = NetworkCommonSettings.CloudServiceDomain;
			//var port = NetworkCommonSettings.CloudServiceHttpsPort;
			//return $"{prefix}{address}:{port}";
		}

		///////////////////////////////////////////////

		public class AccessGetUserByVerificationCodeResult
		{
			public long UserID;
			public string Username;
			public double Balance;
			public string Error;
		}

		public static async Task<AccessGetUserByVerificationCodeResult> AccessGetUserByVerificationCodeAsync( long projectID, CloudUserRole userRole, string verificationCode, bool getBalance, string serverCheckCodeToChangeServerLimits = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			command.FunctionName = "api/v1/access/get_user_by_verification_code";
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;
			if( !string.IsNullOrEmpty( serverCheckCodeToChangeServerLimits ) )
				command.ServerCheckCodeOrAccessToken = serverCheckCodeToChangeServerLimits;

			var block = new TextBlock();
			block.SetAttribute( "ProjectID", projectID.ToString() );
			block.SetAttribute( "UserRole", userRole.ToString() );
			block.SetAttribute( "VerificationCode", verificationCode );
			if( getBalance )
				block.SetAttribute( "GetBalance", "True" );

			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			var requestResult = await command.ExecuteAsync( cancellationToken );
			if( !string.IsNullOrEmpty( requestResult.Error ) )
				return new AccessGetUserByVerificationCodeResult() { Error = requestResult.Error };

			var requestBlock = requestResult.Data;
			long.TryParse( requestBlock.GetAttribute( "UserID" ), out var userID );
			double.TryParse( requestBlock.GetAttribute( "Balance" ), out var balance );

			return new AccessGetUserByVerificationCodeResult()
			{
				UserID = userID,
				Username = requestBlock.GetAttribute( "Username" ),
				Balance = balance,
			};
		}

		///////////////////////////////////////////////

		public class AccessRequestServiceResult
		{
			public string ServerAddress;
			public int ServerPort;
			public string VerificationCode;
			public string Error;
		}

		/// <summary>
		/// Requests a service server address and port.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="userRole">Developer, Player</param>
		/// <param name="projectID"></param>
		/// <returns></returns>
		public static async Task<AccessRequestServiceResult> AccessRequestServiceServerAsync( string service, CloudUserRole userRole, long? projectID = null, string authToken = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			command.FunctionName = "api/v1/access/request_server_access";
			if( !string.IsNullOrEmpty( authToken ) )
				command.ServerCheckCodeOrAccessToken = authToken;
			else
				command.RequireUserLogin = true;
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			var block = new TextBlock();
			block.SetAttribute( "Service", service );
			block.SetAttribute( "UserRole", userRole.ToString() );
			if( projectID != null )
				block.SetAttribute( "ProjectID", projectID.Value.ToString() );
			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			var requestResult = await command.ExecuteAsync( cancellationToken );
			if( !string.IsNullOrEmpty( requestResult.Error ) )
				return new AccessRequestServiceResult() { Error = requestResult.Error };

			var requestBlock = requestResult.Data;
			return new AccessRequestServiceResult()
			{
				ServerAddress = requestBlock.GetAttribute( "ServerAddress" ),
				ServerPort = int.Parse( requestBlock.GetAttribute( "ServerPort" ) ),
				VerificationCode = requestBlock.GetAttribute( "VerificationCode" ),
			};
		}

		///////////////////////////////////////////////

		public static async Task<CloudServiceExecuteCommand.ResultClass> ProjectUpdateFromServerAsync( /*long projectID*/ TextBlock block, string authToken = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			command.FunctionName = "api/v1/project/update_from_server";
			if( !string.IsNullOrEmpty( authToken ) )
				command.ServerCheckCodeOrAccessToken = authToken;
			else
				command.RequireUserLogin = true;
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			return await command.ExecuteAsync( cancellationToken );
		}

		///////////////////////////////////////////////

		public class UserTransactionItem
		{
			public long Id;
			public DateTime CreationTime;

			public long SenderID;
			public long RecepientID;
			public long AffiliateID;

			//AdminChangeBalance: change balance by admin.
			//TopUp: top up balance. NodeId, ServiceComission, NodeComission are used.
			//Withdraw: withdraw balance. NodeId, ServiceComission, NodeComission are used.
			//PaymentServer: оплата сервера (за аренду, за трафик, за хранение). NodeID is used. ProjectId is used (first project of the server). PaymentName: "Basic", "Additional traffic".
			//PaymentStorage: оплата хранилища (за трафик, за хранение).
			//ProjectEntryFee: за вход в проект (купить игру). ProjectID is used.
			//ProjectInAppPayment: in-app payments. purchase when amount > 0, withdraw when amount < 0. ProjectID, PaymentName are used.
			public string Type;

			public double Amount;

			public long ProjectID;
			public string ProjectIDs;
			public string ServerAddress;
			public string PaymentName; //"Main server", "Horizontal server", "Main server traffic", "Horizontal server traffic" or any name of in-app purchase

			//dynamic data
			public string Status; // "Pending", "Completed" ////, "Failed" delete when failed
			public string Description; //Any description, may be used by the node.
			public bool ProcessedByProject; //in app purchases
			public string ProcessedByProjectDetails; //in app purchases
		}

		public class UserTransactionsGetResult
		{
			public TextBlock Data;
			public List<UserTransactionItem> Transactions;
			public string Error;
		}

		static string ParseUserTransaction( TextBlock block, UserTransactionItem tx )
		{
			if( !long.TryParse( block.GetAttribute( "Id" ), out tx.Id ) )
				return "Can't parse Id.";

			if( long.TryParse( block.GetAttribute( "CreationTime" ), out var creationTimeTicks ) )
				tx.CreationTime = new DateTime( creationTimeTicks, DateTimeKind.Utc );

			if( block.AttributeExists( "SenderID" ) )
				long.TryParse( block.GetAttribute( "SenderID" ), out tx.SenderID );
			if( block.AttributeExists( "RecepientID" ) )
				long.TryParse( block.GetAttribute( "RecepientID" ), out tx.RecepientID );
			if( block.AttributeExists( "AffiliateID" ) )
				long.TryParse( block.GetAttribute( "AffiliateID" ), out tx.AffiliateID );

			tx.Type = block.GetAttribute( "Type" );
			double.TryParse( block.GetAttribute( "Amount" ), out tx.Amount );

			if( block.AttributeExists( "ProjectID" ) )
				long.TryParse( block.GetAttribute( "ProjectID" ), out tx.ProjectID );
			if( block.AttributeExists( "ProjectIDs" ) )
				tx.ProjectIDs = block.GetAttribute( "ProjectIDs" );
			if( block.AttributeExists( "ServerAddress" ) )
				tx.ServerAddress = block.GetAttribute( "ServerAddress" );
			if( block.AttributeExists( "PaymentName" ) )
				tx.PaymentName = block.GetAttribute( "PaymentName" );

			if( block.AttributeExists( "Status" ) )
				tx.Status = block.GetAttribute( "Status" );
			if( block.AttributeExists( "Description" ) )
				tx.Description = block.GetAttribute( "Description" );
			if( block.AttributeExists( "ProcessedByProject" ) )
				bool.TryParse( block.GetAttribute( "ProcessedByProject" ), out tx.ProcessedByProject );
			if( block.AttributeExists( "ProcessedByProjectDetails" ) )
				tx.ProcessedByProjectDetails = block.GetAttribute( "ProcessedByProjectDetails" );

			return null;
		}

		public static async Task<UserTransactionsGetResult> UserTransactionsGetAsync( string type = null, long? projectID = null, string status = "Completed", long? transactionID = null, string creationRequestID = null, int? getLatest = null, string authToken = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			command.FunctionName = "api/v1/user_transaction/get";
			if( !string.IsNullOrEmpty( authToken ) )
				command.ServerCheckCodeOrAccessToken = authToken;
			else
				command.RequireUserLogin = true;
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			var inputBlock = new TextBlock();
			if( type != null )
				inputBlock.SetAttribute( "Type", type );
			if( projectID != null )
				inputBlock.SetAttribute( "ProjectID", projectID.Value.ToString() );
			if( status != null )
				inputBlock.SetAttribute( "Status", status );
			if( !string.IsNullOrEmpty( creationRequestID ) )
				inputBlock.SetAttribute( "CreationRequestID", creationRequestID );
			if( transactionID != null )
				inputBlock.SetAttribute( "TransactionID", transactionID.ToString() );
			if( getLatest != null )
				inputBlock.SetAttribute( "GetLatest", getLatest.Value.ToString() );
			command.ContentData = Encoding.UTF8.GetBytes( inputBlock.DumpToString() );

			var result = await command.ExecuteAsync( cancellationToken );
			if( !string.IsNullOrEmpty( result.Error ) )
				return new UserTransactionsGetResult { Error = result.Error };

			var list = new List<UserTransactionItem>();
			foreach( var block in result.Data.Children )
			{
				if( block.Name == "Item" )
				{
					var tx = new UserTransactionItem();
					var error = ParseUserTransaction( block, tx );
					if( !string.IsNullOrEmpty( error ) )
						return new UserTransactionsGetResult { Error = error };
					list.Add( tx );
				}
			}
			return new UserTransactionsGetResult { Data = result.Data, Transactions = list };
		}

		public class UserTransactionsIsProjectEntryFeePaidResult
		{
			public bool Paid;
			public string Error;
		}

		public static async Task<UserTransactionsIsProjectEntryFeePaidResult> UserTransactionsIsProjectEntryFeePaidAsync( long userID, long projectID, string authToken = null, CancellationToken cancellationToken = default )
		{
			var paidCount = 0;
			{
				var result = await UserTransactionsGetAsync( "ProjectEntryFee", projectID, cancellationToken: cancellationToken );
				if( !string.IsNullOrEmpty( result.Error ) )
					return new UserTransactionsIsProjectEntryFeePaidResult() { Error = result.Error };
				foreach( var tx in result.Transactions )
				{
					if( tx.SenderID == userID )
						paidCount++;
				}
			}

			var cancelCount = 0;
			{
				var result = await UserTransactionsGetAsync( "ProjectEntryFeeCancel", projectID );
				if( !string.IsNullOrEmpty( result.Error ) )
					return new UserTransactionsIsProjectEntryFeePaidResult() { Error = result.Error };
				foreach( var tx in result.Transactions )
				{
					//if( tx.SenderID == userID )
					cancelCount++;
				}
			}

			return new UserTransactionsIsProjectEntryFeePaidResult() { Paid = paidCount > cancelCount };
		}

		public static async Task<CloudServiceExecuteCommand.ResultClass> UserTransactionsRequestProjectInAppPurchaseAsync( long projectID, long senderID, double amount, string description /*= null*/, string requestID = null, string authToken = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			command.FunctionName = "api/v1/user_transaction/request_project_in_app_purchase";
			if( !string.IsNullOrEmpty( authToken ) )
				command.ServerCheckCodeOrAccessToken = authToken;
			else
				command.RequireUserLogin = true;
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			var block = new TextBlock();
			block.SetAttribute( "ProjectID", projectID.ToString() );
			block.SetAttribute( "SenderID", senderID.ToString() );
			block.SetAttribute( "Amount", amount.ToString() );
			if( description != null )
				block.SetAttribute( "Description", description );

			var requestID2 = requestID;
			if( string.IsNullOrEmpty( requestID2 ) )
				requestID2 = Guid.NewGuid().ToString();
			block.SetAttribute( "RequestID", requestID2 );

			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			return await command.ExecuteAsync( cancellationToken );
		}

		public static async Task<CloudServiceExecuteCommand.ResultClass> UserTransactionsProjectInAppWithdrawAsync( long projectID, long recepientID, double amount, string description /*= null*/, string requestID = null, string authToken = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			command.FunctionName = "api/v1/user_transaction/project_in_app_withdraw";
			if( !string.IsNullOrEmpty( authToken ) )
				command.ServerCheckCodeOrAccessToken = authToken;
			else
				command.RequireUserLogin = true;
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			var block = new TextBlock();
			block.SetAttribute( "ProjectID", projectID.ToString() );
			block.SetAttribute( "RecepientID", recepientID.ToString() );
			block.SetAttribute( "Amount", amount.ToString() );
			if( description != null )
				block.SetAttribute( "Description", description );

			var requestID2 = requestID;
			if( string.IsNullOrEmpty( requestID2 ) )
				requestID2 = Guid.NewGuid().ToString();
			block.SetAttribute( "RequestID", requestID2 );

			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			return await command.ExecuteAsync( cancellationToken );
		}

		//public static async Task<CloudServiceExecuteCommand.ResultClass> UserTransactionsTransferToAnotherUserAsync( long recepientID, string currency, double amount, string description = null, string serverCheckCode = null, CancellationToken cancellationToken = default )
		//{
		//	var command = new CloudServiceExecuteCommand();
		//	command.FunctionName = "api/user_transactions_transfer_to_another_user";
		//	if( !string.IsNullOrEmpty( serverCheckCode ) )
		//		command.ServerCheckCode = serverCheckCode;
		//	else
		//		command.RequireUserLogin = true;

		//	command.AddParameter( "recepientID", recepientID.ToString(), false );
		//	command.AddParameter( "currency", currency, false );
		//	command.AddParameter( "amount", amount.ToString(), false );
		//	if( description != null )
		//		command.AddParameter( "description", description, true );

		//	return await command.ExecuteAsync( cancellationToken );
		//}

		//public static async Task<CloudServiceExecuteCommand.ResultClass> UserTransactionsUpdateAsync( long transactionId, string description = null/*, bool? processedByProject = null, string processedByProjectDetails = null*/, string serverCheckCode = null, CancellationToken cancellationToken = default )
		//{
		//	var command = new CloudServiceExecuteCommand();
		//	command.FunctionName = "api/v1/user_transaction/update";
		//	if( !string.IsNullOrEmpty( serverCheckCode ) )
		//		command.ServerCheckCode = serverCheckCode;
		//	else
		//		command.RequireUserLogin = true;
		//	command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

		//	var block = new TextBlock();
		//	block.SetAttribute( "TransactionID", transactionId.ToString() );
		//	if( description != null )
		//		block.SetAttribute( "Description", description );
		//	//if( processedByProject != null )
		//	//	block.SetAttribute( "ProcessedByProject", processedByProject.Value.ToString() );
		//	//if( processedByProjectDetails != null )
		//	//	block.SetAttribute( "ProcessedByProjectDetails", processedByProjectDetails );
		//	command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

		//	return await command.ExecuteAsync( cancellationToken );
		//}

		///////////////////////////////////////////////

		public class StorageGetInfoResult
		{
			public int Directories;
			public int Files;
			public long Size;
			public string Error;
		}

		public static async Task<StorageGetInfoResult> StorageGetInfoAsync( string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/storage/get_info";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new StorageGetInfoResult() { Error = executeResult.Error };

				var rootBlock = executeResult.Data;

				var result = new StorageGetInfoResult();
				int.TryParse( rootBlock.GetAttribute( "Directories" ), out result.Directories );
				int.TryParse( rootBlock.GetAttribute( "Files" ), out result.Files );
				long.TryParse( rootBlock.GetAttribute( "Size" ), out result.Size );
				return result;
			}
			catch( Exception e )
			{
				return new StorageGetInfoResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class StorageGetFilesInfoResult
		{
			public Item[] Items;
			public string Error;

			/////////////////////

			public struct Item : IEquatable<Item>
			{
				public bool Exists;
				public long Size;
				public DateTime LastModified;

				//

				public bool Equals( Item other )
				{
					return Exists == other.Exists && Size == other.Size && LastModified == other.LastModified;
				}

				public override bool Equals( object obj )
				{
					return obj is Item item && Equals( item );
				}

				public override int GetHashCode()
				{
					return HashCode.Combine( Exists, Size, LastModified );
				}

				public override string ToString()
				{
					return Exists + " " + Size.ToString();
				}
			}
		}

		public static async Task<StorageGetFilesInfoResult> StorageGetFilesInfoAsync( IList<string> storageFileNames, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/storage/get_files_info";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				for( int n = 0; n < storageFileNames.Count; n++ )
					block.SetAttribute( $"Name{n}", storageFileNames[ n ].Replace( '\\', '/' ) );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new StorageGetFilesInfoResult() { Error = executeResult.Error };

				var rootBlock = executeResult.Data;

				var result = new StorageGetFilesInfoResult();
				var resultItems = new List<StorageGetFilesInfoResult.Item>();

				foreach( var childItem in rootBlock.Children )
				{
					if( childItem.Name != "Item" )
						continue;

					var fileItem = new StorageGetFilesInfoResult.Item();
					long.TryParse( childItem.GetAttribute( "Size" ), out fileItem.Size );
					fileItem.Exists = fileItem.Size >= 0;
					if( long.TryParse( childItem.GetAttribute( "LastModified" ), out var lastModifiedTicks ) )
						fileItem.LastModified = new DateTime( lastModifiedTicks, DateTimeKind.Utc );

					resultItems.Add( fileItem );
				}

				if( storageFileNames.Count != resultItems.Count )
					return new StorageGetFilesInfoResult() { Error = "Invalid item count." };

				result.Items = resultItems.ToArray();

				return result;
			}
			catch( Exception e )
			{
				return new StorageGetFilesInfoResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class StorageGetFileInfoResult
		{
			public bool Exists;
			public long Size;
			public DateTime LastModified;
			public string Error;
		}

		public static async Task<StorageGetFileInfoResult> StorageGetFileInfoAsync( string storageFileName, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var getFilesInfoResult = await StorageGetFilesInfoAsync( new string[] { storageFileName }, authToken, cancellationToken );
				if( !string.IsNullOrEmpty( getFilesInfoResult.Error ) )
					return new StorageGetFileInfoResult() { Error = getFilesInfoResult.Error };

				var result = new StorageGetFileInfoResult();
				if( getFilesInfoResult.Items.Length == 1 )
				{
					var item = getFilesInfoResult.Items[ 0 ];
					result.Exists = item.Exists;
					result.Size = item.Size;
					result.LastModified = item.LastModified;
				}

				return result;
			}
			catch( Exception e )
			{
				return new StorageGetFileInfoResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class StorageGetDirectoryInfoResult
		{
			public bool Exists;
			public Item[] Items;
			public string Error;

			/////////////////////

			public struct Item : IEquatable<Item>
			{
				public string Name;
				public long Size;
				public DateTime LastModified;
				public bool IsDirectory;

				public bool Public;

				//

				public bool Equals( Item other )
				{
					return Name == other.Name && Size == other.Size && LastModified == other.LastModified && IsDirectory == other.IsDirectory && Public == other.Public;
				}

				public override bool Equals( object obj )
				{
					return obj is Item item && Equals( item );
				}

				public override int GetHashCode()
				{
					return HashCode.Combine( Name, Size, LastModified, IsDirectory, Public );
				}

				public override string ToString()
				{
					return Name + " " + Size.ToString();
				}
			}
		}

		public static async Task<StorageGetDirectoryInfoResult> StorageGetDirectoryInfoAsync( string storageFileName, string searchPattern, SearchOption searchOption, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/storage/get_directory_info";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "DirectoryName", storageFileName.Replace( '\\', '/' ) );
				if( searchPattern != null )
					block.SetAttribute( "SearchPattern", searchPattern );
				if( searchOption != SearchOption.TopDirectoryOnly )
					block.SetAttribute( "SearchOption", searchOption.ToString() );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new StorageGetDirectoryInfoResult() { Error = executeResult.Error };

				var rootBlock = executeResult.Data;

				var result = new StorageGetDirectoryInfoResult();
				bool.TryParse( rootBlock.GetAttribute( "Exists" ), out result.Exists );
				var resultItems = new List<StorageGetDirectoryInfoResult.Item>();

				foreach( var childItem in rootBlock.Children )
				{
					if( childItem.Name != "Item" )
						continue;

					var fileItem = new StorageGetDirectoryInfoResult.Item();
					fileItem.Name = childItem.GetAttribute( "Name" );
					long.TryParse( childItem.GetAttribute( "Size" ), out fileItem.Size );
					if( long.TryParse( childItem.GetAttribute( "LastModified" ), out var lastModifiedTicks ) )
						fileItem.LastModified = new DateTime( lastModifiedTicks, DateTimeKind.Utc );
					bool.TryParse( childItem.GetAttribute( "IsDirectory" ), out fileItem.IsDirectory );
					bool.TryParse( childItem.GetAttribute( "Public" ), out fileItem.Public );

					resultItems.Add( fileItem );
				}

				result.Items = resultItems.ToArray();

				return result;
			}
			catch( Exception e )
			{
				return new StorageGetDirectoryInfoResult() { Error = e.Message };
			}
		}

		public static async Task<SimpleResult> StorageCreateDirectoriesAsync( string[] directoryNames, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/storage/create_directories";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				for( int n = 0; n < directoryNames.Length; n++ )
					block.SetAttribute( $"DirectoryName{n}", directoryNames[ n ].Replace( '\\', '/' ) );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		public static async Task<SimpleResult> StorageCreateDirectoryAsync( string directoryName, string authToken = null, CancellationToken cancellationToken = default )
		{
			return await StorageCreateDirectoriesAsync( new string[] { directoryName }, authToken, cancellationToken );
		}

		///////////////////////////////////////////////

		public struct DeleteObjectsItem
		{
			public string Name;
			public bool IsDirectory;

			public DeleteObjectsItem( string path, bool isDirectory )
			{
				Name = path;
				IsDirectory = isDirectory;
			}
		}

		public static async Task<SimpleResult> StorageDeleteObjectsAsync( IList<DeleteObjectsItem> objects, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/storage/delete_objects";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				for( int n = 0; n < objects.Count; n++ )
				{
					var obj = objects[ n ];
					block.SetAttribute( $"Name{n}", obj.Name.Replace( '\\', '/' ) );
					if( obj.IsDirectory )
						block.SetAttribute( $"IsDirectory{n}", obj.IsDirectory.ToString() );
				}
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		public static async Task<SimpleResult> StorageDeleteDirectoryAsync( string storageDirectory, string authToken = null, CancellationToken cancellationToken = default )
		{
			var objects = new DeleteObjectsItem[] { new DeleteObjectsItem( storageDirectory, true ) };
			return await StorageDeleteObjectsAsync( objects, authToken, cancellationToken );
		}

		public static async Task<SimpleResult> StorageDeleteFilesAsync( string[] storageFileNames, string authToken = null, CancellationToken cancellationToken = default )
		{
			var objects = new DeleteObjectsItem[ storageFileNames.Length ];
			for( int n = 0; n < objects.Length; n++ )
				objects[ n ] = new DeleteObjectsItem( storageFileNames[ n ], false );
			return await StorageDeleteObjectsAsync( objects, authToken, cancellationToken );
		}

		public static async Task<SimpleResult> StorageDeleteFileAsync( string storageFileName, string authToken = null, CancellationToken cancellationToken = default )
		{
			var objects = new DeleteObjectsItem[] { new DeleteObjectsItem( storageFileName, false ) };
			return await StorageDeleteObjectsAsync( objects, authToken, cancellationToken );
		}

		///////////////////////////////////////////////

		public struct CopyObjectsItem
		{
			public string Name;
			public bool IsDirectory;
			public string TargetName;

			public CopyObjectsItem( string path, bool isDirectory, string targetName )
			{
				Name = path;
				IsDirectory = isDirectory;
				TargetName = targetName;
			}
		}

		public static async Task<SimpleResult> StorageCopyObjectsAsync( CopyObjectsItem[] objects, bool move, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/storage/copy_objects";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "Move", move.ToString() );
				for( int n = 0; n < objects.Length; n++ )
				{
					var obj = objects[ n ];
					block.SetAttribute( $"Name{n}", obj.Name.Replace( '\\', '/' ) );
					if( obj.IsDirectory )
						block.SetAttribute( $"IsDirectory{n}", obj.IsDirectory.ToString() );
					block.SetAttribute( $"TargetName{n}", obj.TargetName.Replace( '\\', '/' ) );
				}
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public struct MakePublicItem
		{
			public string Name;
			public bool IsDirectory;

			public MakePublicItem( string path, bool isDirectory )
			{
				Name = path;
				IsDirectory = isDirectory;
			}
		}

		public static async Task<SimpleResult> StorageMakePublicAsync( IList<MakePublicItem> objects, bool makePublic, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/storage/make_public";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "MakePublic", makePublic.ToString() );
				for( int n = 0; n < objects.Count; n++ )
				{
					var obj = objects[ n ];
					block.SetAttribute( $"Name{n}", obj.Name.Replace( '\\', '/' ) );
					if( obj.IsDirectory )
						block.SetAttribute( $"IsDirectory{n}", obj.IsDirectory.ToString() );
				}
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class StorageGetContentUrlsResult
		{
			public string[] Urls;
			public string Error;
		}

		public static async Task<StorageGetContentUrlsResult> StorageGetContentUrlsAsync( string[] storageFileNames, bool upload, bool makePublic = false, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/storage/get_content_urls";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "Upload", upload.ToString() );
				if( makePublic )
					block.SetAttribute( "MakePublic", "True" );

				for( int n = 0; n < storageFileNames.Length; n++ )
					block.SetAttribute( $"Name{n}", storageFileNames[ n ].Replace( '\\', '/' ) );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new StorageGetContentUrlsResult() { Error = executeResult.Error };

				var urls = new List<string>();
				for( int n = 0; ; n++ )
				{
					if( !executeResult.Data.AttributeExists( $"Url{n}" ) )
						break;
					var code = executeResult.Data.GetAttribute( $"Url{n}" );
					urls.Add( code );
				}

				return new StorageGetContentUrlsResult() { Urls = urls.ToArray() };
			}
			catch( Exception e )
			{
				return new StorageGetContentUrlsResult() { Error = e.Message };
			}
		}

		public class StorageGetContentUrlResult
		{
			public string Url;
			public string Error;
		}

		public static async Task<StorageGetContentUrlResult> StorageGetContentUrlAsync( string storageFileName, bool upload, bool makePublic = false, string authToken = null, CancellationToken cancellationToken = default )
		{
			var result = await StorageGetContentUrlsAsync( new string[] { storageFileName }, upload, makePublic, authToken, cancellationToken );
			if( !string.IsNullOrEmpty( result.Error ) )
				return new StorageGetContentUrlResult() { Error = result.Error };
			if( result.Urls.Length != 1 )
				return new StorageGetContentUrlResult() { Error = "Invalid url count." };
			return new StorageGetContentUrlResult() { Url = result.Urls[ 0 ] };
		}

		///////////////////////////////////////////////

		public class HorizontalServerNewResult
		{
			public string ServerAddress;
			public string Error;
		}

		public static async Task<HorizontalServerNewResult> HorizontalServerNewAsync( long projectID, string optionalRegion, string optionalConfiguration, string requestID = null, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/horizontal_server/new";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "ProjectID", projectID.ToString() );
				if( !string.IsNullOrEmpty( optionalRegion ) )
					block.SetAttribute( "Region", optionalRegion );
				if( !string.IsNullOrEmpty( optionalConfiguration ) )
					block.SetAttribute( "Configuration", optionalConfiguration );

				var requestID2 = requestID;
				if( string.IsNullOrEmpty( requestID2 ) )
					requestID2 = Guid.NewGuid().ToString();
				block.SetAttribute( "RequestID", requestID2 );

				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new HorizontalServerNewResult() { Error = executeResult.Error };

				var serverAddress = executeResult.Data.GetAttribute( "Address" );
				return new HorizontalServerNewResult() { ServerAddress = serverAddress };
			}
			catch( Exception e )
			{
				return new HorizontalServerNewResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public static async Task<SimpleResult> HorizontalServerDeleteAsync( long projectID, string serverAddress, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/horizontal_server/delete";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "ProjectID", projectID.ToString() );
				block.SetAttribute( "Address", serverAddress );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public static async Task<SimpleResult> HorizontalServerDeleteAllAsync( long projectID, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/horizontal_server/delete_all";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "ProjectID", projectID.ToString() );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public static async Task<SimpleResult> HorizontalServerResetAsync( long projectID, string serverAddress, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/horizontal_server/reset";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "ProjectID", projectID.ToString() );
				block.SetAttribute( "Address", serverAddress );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public static async Task<SimpleResult> HorizontalServerResetAllAsync( long projectID, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/horizontal_server/reset_all";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "ProjectID", projectID.ToString() );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public static async Task<SimpleResult> ServerRestartAsync( string address, string serverCheckCode = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( serverCheckCode ) )
					command.ServerCheckCodeOrAccessToken = serverCheckCode;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/server/restart";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "Address", address );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class ServerUpdateRootPasswordResult
		{
			public string Password;
			public string Error;
		}

		public static async Task<ServerUpdateRootPasswordResult> ServerUpdateRootPasswordAsync( string address, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/server/update_root_password";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;
				command.Timeout = 60000 * 5;

				var block = new TextBlock();
				block.SetAttribute( "Address", address );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new ServerUpdateRootPasswordResult() { Error = executeResult.Error };

				var password = executeResult.Data.GetAttribute( "Password" );
				return new ServerUpdateRootPasswordResult() { Password = password };
			}
			catch( Exception e )
			{
				return new ServerUpdateRootPasswordResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class ServerGetResult
		{
			public List<ServerItem> Servers;
			public string Error;

			/////////////////////

			public class ServerItem
			{
				//common data
				public string Address;
				public string Region;
				public string Configuration;
				public bool Horizontal;

				//data from the server
				public StatusEnum Status;
				public string ServerCheckCode;
				public string HorizontalServerBuildName;
				public int CPUUsage;
				public long MemoryInUse;
				public long MemoryCapacity;
				public long SwapInUse;
				public long SwapCapacity;
				public int GPUUsage;
				public long GPUMemoryInUse;
				public long GPUMemoryCapacity;
				public long DiskInUse;
				public long DiskCapacity;
				public long TrafficOutbound;
				public long TrafficInbound;
				public long TrafficOutboundSpeed;
				public long TrafficInboundSpeed;
				public ProjectItem[] Projects;

				//data from the provider
				public long TrafficOutboundUsed;
				public long TrafficOutboundQuota;
				public long TrafficOutboundBillable;

				////////////////////

				public enum StatusEnum
				{
					Invalid,
					Creating,
					Connected,
				}

				////////////////////

				public class ProjectItem
				{
					public long ProjectID;
					public string ProcessSummary;
					public int ProcessPort;
					public string ProcessSettings;

					TextBlock processSummaryAsTextBlock;

					//

					public TextBlock ProcessSummaryAsTextBlock
					{
						get
						{
							if( processSummaryAsTextBlock == null && !string.IsNullOrEmpty( ProcessSummary ) )
								processSummaryAsTextBlock = TextBlock.Parse( ProcessSummary, out _ );
							return processSummaryAsTextBlock;
						}
					}
				}

				////////////////////

				public int MemoryUsage
				{
					get
					{
						if( MemoryCapacity == 0 )
							return 0;
						return (int)( MemoryInUse * 100 / MemoryCapacity );
					}
				}

				public int SwapUsage
				{
					get
					{
						if( SwapCapacity == 0 )
							return 0;
						return (int)( SwapInUse * 100 / SwapCapacity );
					}
				}

				public int GPUMemoryUsage
				{
					get
					{
						if( GPUMemoryCapacity == 0 )
							return 0;
						return (int)( GPUMemoryInUse * 100 / GPUMemoryCapacity );
					}
				}

				public int DiskUsage
				{
					get
					{
						if( DiskCapacity == 0 )
							return 0;
						return (int)( DiskInUse * 100 / DiskCapacity );
					}
				}
			}
		}

		public static async Task<ServerGetResult> ServerGetAsync( long projectID, string optionalServerAddress, bool getTrafficInfoFromProvider, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/server/get";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var inputBlock = new TextBlock();
				inputBlock.SetAttribute( "ProjectID", projectID.ToString() );
				if( !string.IsNullOrEmpty( optionalServerAddress ) )
					inputBlock.SetAttribute( "Address", optionalServerAddress );
				if( getTrafficInfoFromProvider )
					inputBlock.SetAttribute( "GetTrafficInfoFromProvider", "True" );

				command.ContentData = Encoding.UTF8.GetBytes( inputBlock.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new ServerGetResult() { Error = executeResult.Error };

				var list = new List<ServerGetResult.ServerItem>();
				foreach( var serverBlock in executeResult.Data.Children )
				{
					if( serverBlock.Name == "Item" )
					{
						var server = new ServerGetResult.ServerItem();

						//common data
						server.Address = serverBlock.GetAttribute( "Address" );
						server.Region = serverBlock.GetAttribute( "Region" );
						server.Configuration = serverBlock.GetAttribute( "Configuration" );
						bool.TryParse( serverBlock.GetAttribute( "Horizontal", "False" ), out server.Horizontal );

						//data from the server

						Enum.TryParse( serverBlock.GetAttribute( "Status", "Invalid" ), out server.Status );

						server.ServerCheckCode = serverBlock.GetAttribute( "ServerCheckCode", null );
						server.HorizontalServerBuildName = serverBlock.GetAttribute( "HorizontalServerBuildName" );

						int.TryParse( serverBlock.GetAttribute( "CPUUsage", "0" ), out server.CPUUsage );

						long.TryParse( serverBlock.GetAttribute( "MemoryInUse", "0" ), out server.MemoryInUse );
						long.TryParse( serverBlock.GetAttribute( "MemoryCapacity", "0" ), out server.MemoryCapacity );
						long.TryParse( serverBlock.GetAttribute( "SwapInUse", "0" ), out server.SwapInUse );
						long.TryParse( serverBlock.GetAttribute( "SwapCapacity", "0" ), out server.SwapCapacity );

						int.TryParse( serverBlock.GetAttribute( "GPUUsage", "0" ), out server.GPUUsage );
						long.TryParse( serverBlock.GetAttribute( "GPUMemoryInUse", "0" ), out server.GPUMemoryInUse );
						long.TryParse( serverBlock.GetAttribute( "GPUMemoryCapacity", "0" ), out server.GPUMemoryCapacity );

						long.TryParse( serverBlock.GetAttribute( "DiskInUse" ), out server.DiskInUse );
						long.TryParse( serverBlock.GetAttribute( "DiskCapacity" ), out server.DiskCapacity );

						long.TryParse( serverBlock.GetAttribute( "TrafficOutbound", "0" ), out server.TrafficOutbound );
						long.TryParse( serverBlock.GetAttribute( "TrafficInbound", "0" ), out server.TrafficInbound );
						long.TryParse( serverBlock.GetAttribute( "TrafficOutboundSpeed", "0" ), out server.TrafficOutboundSpeed );
						long.TryParse( serverBlock.GetAttribute( "TrafficInboundSpeed", "0" ), out server.TrafficInboundSpeed );

						var projects = new List<ServerGetResult.ServerItem.ProjectItem>();
						foreach( var projectBlock in serverBlock.Children )
						{
							if( projectBlock.Name == "Project" )
							{
								var project = new ServerGetResult.ServerItem.ProjectItem();
								long.TryParse( projectBlock.GetAttribute( "ProjectID" ), out project.ProjectID );
								project.ProcessSummary = projectBlock.GetAttribute( "ProcessSummary" );
								int.TryParse( projectBlock.GetAttribute( "ProcessPort", "0" ), out project.ProcessPort );
								project.ProcessSettings = projectBlock.GetAttribute( "ProcessSettings" );
								projects.Add( project );
							}
						}
						server.Projects = projects.ToArray();

						//data from the provider
						long.TryParse( executeResult.Data.GetAttribute( "TrafficOutboundUsed", "0" ), out server.TrafficOutboundUsed );
						long.TryParse( executeResult.Data.GetAttribute( "TrafficOutboundQuota", "0" ), out server.TrafficOutboundQuota );
						long.TryParse( executeResult.Data.GetAttribute( "TrafficOutboundBillable", "0" ), out server.TrafficOutboundBillable );

						list.Add( server );
					}
				}
				return new ServerGetResult() { Servers = list };
			}
			catch( Exception e )
			{
				return new ServerGetResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class ProjectNewResult
		{
			public long ProjectID;
			public string Error;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="serverless"></param>
		/// <param name="region"></param>
		/// <param name="attachTo"></param>
		/// <param name="configuration"></param>
		/// <param name="templateOrBackup"></param>
		/// <param name="requestID">Use this ID to make guaranteed call of the method. Internally the project will not created twice for requests with equal request ID. The example of making request ID: var requestID = Guid.NewGuid().ToString()</param>
		/// <param name="authToken"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<ProjectNewResult> ProjectNewAsync( string name, bool serverless, string region, string attachTo, string configuration, string templateOrBackup, string requestID = null, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/project/new";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				if( !string.IsNullOrEmpty( name ) )
					block.SetAttribute( "Name", name.ToString() );
				block.SetAttribute( "Serverless", serverless.ToString() );
				if( !string.IsNullOrEmpty( region ) )
					block.SetAttribute( "Region", region.ToString() );
				if( !string.IsNullOrEmpty( attachTo ) )
					block.SetAttribute( "AttachTo", attachTo.ToString() );
				if( !string.IsNullOrEmpty( configuration ) )
					block.SetAttribute( "Configuration", configuration.ToString() );
				if( !string.IsNullOrEmpty( templateOrBackup ) )
					block.SetAttribute( "TemplateOrBackup", templateOrBackup.ToString() );

				var requestID2 = requestID;
				if( string.IsNullOrEmpty( requestID2 ) )
					requestID2 = Guid.NewGuid().ToString();
				block.SetAttribute( "RequestID", requestID2 );

				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new ProjectNewResult() { Error = executeResult.Error };

				long.TryParse( executeResult.Data.GetAttribute( "ProjectID" ), out var projectID );
				return new ProjectNewResult() { ProjectID = projectID };
			}
			catch( Exception e )
			{
				return new ProjectNewResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public static async Task<SimpleResult> ProjectDeleteAsync( long projectID, double deletionTime, bool cancel, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/project/delete";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "ProjectID", projectID.ToString() );
				if( deletionTime != 0 )
					block.SetAttribute( "DeletionTime", deletionTime.ToString() );
				if( cancel )
					block.SetAttribute( "Cancel", cancel.ToString() );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class ProjectGetResult
		{
			//!!!!not parsed data
			public TextBlock Data;
			public string Error;
		}

		public static async Task<ProjectGetResult> ProjectGetAsync( int skip = 0, int limit = int.MaxValue, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/project/get";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "Skip", skip.ToString() );
				block.SetAttribute( "Limit", limit.ToString() );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new ProjectGetResult() { Error = executeResult.Error };

				return new ProjectGetResult() { Data = executeResult.Data };
			}
			catch( Exception e )
			{
				return new ProjectGetResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class ProjectGetChangesResult
		{
			public TextBlock Data;
			public string Error;
		}

		public static async Task<ProjectGetChangesResult> ProjectGetChangesAsync( long projectID, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/project/get_changes";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "ProjectID", projectID.ToString() );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new ProjectGetChangesResult() { Error = executeResult.Error };

				return new ProjectGetChangesResult() { Data = executeResult.Data };
			}
			catch( Exception e )
			{
				return new ProjectGetChangesResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class AccessTokenGetResult
		{
			public TokenItem[] Tokens;
			public string Error;

			//

			public class TokenItem
			{
				public long Id;
				public DateTime CreationTime;
			}
		}

		public static async Task<AccessTokenGetResult> AccessTokenGetAsync( string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/access_token/get";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new AccessTokenGetResult() { Error = executeResult.Error };

				//write result
				{
					var list = new List<AccessTokenGetResult.TokenItem>();
					foreach( var tokenBlock in executeResult.Data.Children )
					{
						if( tokenBlock.Name == "Item" )
						{
							var item = new AccessTokenGetResult.TokenItem();
							long.TryParse( tokenBlock.GetAttribute( "Id" ), out item.Id );
							long.TryParse( tokenBlock.GetAttribute( "CreationTime" ), out var creationTime );
							item.CreationTime = new DateTime( creationTime, DateTimeKind.Utc );
							list.Add( item );
						}
					}
					return new AccessTokenGetResult() { Tokens = list.ToArray() };
				}
			}
			catch( Exception e )
			{
				return new AccessTokenGetResult() { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		public class AccessTokenNewResult
		{
			public long Id;
			public string Value;
			public string Error;
		}

		public static async Task<AccessTokenNewResult> AccessTokenNewAsync( string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/access_token/new";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				//block.SetAttribute( "Name", name );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new AccessTokenNewResult() { Error = executeResult.Error };

				long.TryParse( executeResult.Data.GetAttribute( "Id" ), out var id );
				var value = executeResult.Data.GetAttribute( "Value" );
				return new AccessTokenNewResult() { Id = id, Value = value };
			}
			catch( Exception e )
			{
				return new AccessTokenNewResult() { Error = e.Message };
			}
		}

		public static async Task<SimpleResult> AccessTokenDeleteAsync( long tokenID, string authToken = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var command = new CloudServiceExecuteCommand();
				if( !string.IsNullOrEmpty( authToken ) )
					command.ServerCheckCodeOrAccessToken = authToken;
				else
					command.RequireUserLogin = true;
				command.FunctionName = "api/v1/access_token/delete";
				command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

				var block = new TextBlock();
				block.SetAttribute( "Id", tokenID.ToString() );
				command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

				var executeResult = await command.ExecuteAsync( cancellationToken );
				if( !string.IsNullOrEmpty( executeResult.Error ) )
					return new SimpleResult() { Error = executeResult.Error };

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult() { Error = e.Message };
			}
		}

		public static async Task<CloudServiceExecuteCommand.ResultClass> ProductGetUserSpecificProductsAsync( string authToken = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			if( !string.IsNullOrEmpty( authToken ) )
				command.ServerCheckCodeOrAccessToken = authToken;
			else
				command.RequireUserLogin = true;
			command.FunctionName = "api/v1/product/get_user_specific_products";
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			var block = new TextBlock();
			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			return await command.ExecuteAsync( cancellationToken );
		}

		public static async Task<CloudServiceExecuteCommand.ResultClass> ProjectUpdateAsync( TextBlock block, string authToken = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			if( !string.IsNullOrEmpty( authToken ) )
				command.ServerCheckCodeOrAccessToken = authToken;
			else
				command.RequireUserLogin = true;
			command.FunctionName = "api/v1/project/update";
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			return await command.ExecuteAsync( cancellationToken );
		}

		public static async Task<CloudServiceExecuteCommand.ResultClass> ProjectUpdateSortIndexAsync( long projectID, int sortIndex, long anotherProjectID, int anotherProjectIDSortIndex, string authToken = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			if( !string.IsNullOrEmpty( authToken ) )
				command.ServerCheckCodeOrAccessToken = authToken;
			else
				command.RequireUserLogin = true;
			command.FunctionName = "api/v1/project/update_sort_index";
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			var block = new TextBlock();
			block.SetAttribute( "ID", projectID.ToString() );
			block.SetAttribute( "SortIndex", sortIndex.ToString() );
			block.SetAttribute( "AnotherProjectID", anotherProjectID.ToString() );
			block.SetAttribute( "AnotherProjectSortIndex", anotherProjectIDSortIndex.ToString() );
			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			return await command.ExecuteAsync( cancellationToken );
		}

		public static async Task<CloudServiceExecuteCommand.ResultClass> ProjectChangeServerAsync( long projectID, string targetServer, string authToken = null, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			if( !string.IsNullOrEmpty( authToken ) )
				command.ServerCheckCodeOrAccessToken = authToken;
			else
				command.RequireUserLogin = true;
			command.FunctionName = "api/v1/project/change_server";
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			var block = new TextBlock();
			block.SetAttribute( "ProjectID", projectID.ToString() );
			block.SetAttribute( "NewServerAddress", targetServer );
			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			return await command.ExecuteAsync( cancellationToken );
		}

		public static async Task<CloudServiceExecuteCommand.ResultClass> UserGetAsync( long userID = 0, CancellationToken cancellationToken = default )
		{
			var command = new CloudServiceExecuteCommand();
			command.FunctionName = "api/v1/user/get";
			command.RequireUserLogin = true;
			command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

			var block = new TextBlock();
			if( userID != 0 )
				block.SetAttribute( "UserID", userID.ToString() );
			command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

			return await command.ExecuteAsync( cancellationToken );
		}
	}
}
