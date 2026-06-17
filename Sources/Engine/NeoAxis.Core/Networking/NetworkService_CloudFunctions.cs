// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152<ServerNetworkService_CloudFunctions> Commonwealth of Dominica.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis.Networking;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using System.Net.WebSockets;



#if !NO_SERVER
using NeoAxis.LiteDB;
using NeoAxis.LiteDB.Engine;
#endif

namespace NeoAxis
{
#if !NO_SERVER
	public class ServerNetworkService_CloudFunctions : ServerService
	{
		public int SaveStringMaxCountPerRequest { get; set; } = 1000;
		public int SaveStringMaxKeyLength { get; set; } = 1024;
		public int SaveStringMaxValueLength { get; set; } = 1 * 1024 * 1024; //1 MB

		public int CloudMethodDefaultMaxCallPerClientPermit { get; set; } = 1000;
		//public int GlobalCloudMethodMaxCallPerClientQueue { get; set; } = 0;
		public CloudMethodAttribute.LimitWindowMeasure CloudMethodDefaultLimitWindow { get; set; } = CloudMethodAttribute.LimitWindowMeasure.Minute;
		public int CloudMethodMaxInputParametersMaxBytes { get; set; } = 10 * 1024 * 1024; //10 MB
		public int CloudMethodMaxInputParametersMaxObjects { get; set; } = 100000;
		public int CloudMethodMaxTimeInMinutes { get; set; } = 10;

		public int FileRequestMaxItemCount { get; set; } = 1000;

		public int ReceiveMessageDownloadFilesContentMaxSize { get; set; } = 1 * 1024 * 1024;
		public int UploadFilesMaxBlockSize { get; set; } = 1 * 1024 * 1024;

		//!!!!not used
		//public int UploadFilesMaxQueueSize { get; set; } = 10;

		//

		MessageType saveStringMessage;
		MessageType loadStringMessage;
		MessageType stringAnswerMessage;
		MessageType callMethodMessage;
		MessageType callMethodAnswerMessage;
		MessageType getFilesInfoMessage;
		MessageType getFilesInfoAnswerMessage;
		MessageType getDirectoryInfoMessage;
		MessageType getDirectoryInfoAnswerMessage;
		MessageType downloadFileContentMessage;
		MessageType downloadFileContentAnswerMessage;
		MessageType uploadFileContentMessage;
		MessageType uploadFileContentAnswerMessage;
		MessageType createDirectoryMessage;
		MessageType createDirectoryAnswerMessage;
		MessageType deleteFilesMessage;
		MessageType deleteFilesAnswerMessage;
		MessageType deleteDirectoryMessage;
		MessageType deleteDirectoryAnswerMessage;
		MessageType cancelRequestMessage;
		MessageType storageDownloadFilesMessage;
		MessageType storageDownloadFilesAnswerMessage;
		MessageType storageUploadFilesMessage;
		MessageType storageUploadFilesAnswerMessage;
		MessageType getCloudMethodInfoMessage;
		MessageType getCloudMethodInfoAnswerMessage;
		MessageType getCloudMethodsMessage;
		MessageType getCloudMethodsAnswerMessage;
		MessageType getCommonInfoMessage;
		MessageType getCommonInfoAnswerMessage;

		string fullPathToDatabase;
		bool databaseReadOnly;
		DatabaseImplClass databaseImpl;
		string projectDirectory;

		ConcurrentDictionary<string, CloudMethodInfo> cloudMethods = new ConcurrentDictionary<string, CloudMethodInfo>();
		ConcurrentDictionary<int, CloudMethodInfo> cloudMethodById = new ConcurrentDictionary<int, CloudMethodInfo>();
		int cloudMethodIdCounter;

		ConcurrentDictionary<(ServerNode.Client, long), RequestToCancelItem> requestsToCancel = new ConcurrentDictionary<(ServerNode.Client, long), RequestToCancelItem>();
		DateTime requestsToCancelLastUpdateTime;

		ConcurrentDictionary<long, CancellationTokenSource> requestsCancellationTokens = new ConcurrentDictionary<long, CancellationTokenSource>();

		///////////////////////////////////////////

		//save, load strings

		public delegate void SaveStringEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, string[] keys, string[] values, ref bool allow, ref string error );
		public event SaveStringEventDelegate SaveStringEvent;

		public delegate void LoadStringEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, string[] keys, ref bool allow, ref string error );
		public event LoadStringEventDelegate LoadStringEvent;

		//call method
		public delegate void CallMethodEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, CloudMethodInfo method, object[] parameters, ref bool answerHandled, ref string error );
		public event CallMethodEventDelegate CallMethodEvent;


		//to configure file access

		public enum FileOperationAccess
		{
			Get,
			UploadFile,
			CreateDirectory,
			Delete,
		}

		public delegate void CheckFileAccessEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, FileSource source, string[] filePaths, string anyData, FileOperationAccess requiredAccess, ref bool allow, ref string error );
		public event CheckFileAccessEventDelegate CheckFileAccessEvent;


		//to override file operations

		public delegate void GetFilesInfoEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, FileSource source, string[] filePaths, string anyData, ref bool answerHandled, ref string error );
		public event GetFilesInfoEventDelegate GetFilesInfoEvent;

		public delegate void GetDirectoryInfoEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, FileSource source, ref string directoryPath, SearchOption searchOption, string anyData, ref bool answerHandled, ref string error );
		public event GetDirectoryInfoEventDelegate GetDirectoryInfoEvent;

		public delegate void DownloadFileContentEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, DownloadFileContentPart[] parts, string anyData, ref bool answerHandled, ref string error );
		public event DownloadFileContentEventDelegate DownloadFileContentEvent;

		public delegate void StorageDownloadFilesEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, string[] filePaths, string anyData, ref bool answerHandled, ref string error );
		public event StorageDownloadFilesEventDelegate StorageDownloadFilesEvent;

		public delegate void UploadFileContentEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, UploadFileContentPart[] parts, string anyData, ref bool answerHandled, ref string error );
		public event UploadFileContentEventDelegate UploadFileContentEvent;

		public delegate void StorageUploadFilesEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, string[] filePaths, string anyData, ref bool answerHandled, ref string error );
		public event StorageUploadFilesEventDelegate StorageUploadFilesEvent;

		public delegate void CreateDirectoryEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, FileSource source, ref string directoryPath, string anyData, ref bool answerHandled, ref string error );
		public event CreateDirectoryEventDelegate CreateDirectoryEvent;

		public delegate void DeleteFilesEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, FileSource source, string[] filePaths, string anyData, ref bool answerHandled, ref string error );
		public event DeleteFilesEventDelegate DeleteFilesEvent;

		public delegate void DeleteDirectoryEventDelegate( ServerNetworkService_CloudFunctions sender, ServerNode.Client client, long requestID, FileSource source, string directoryPath, bool recursive, bool clear, string anyData, ref bool answerHandled, ref string error );
		public event DeleteDirectoryEventDelegate DeleteDirectoryEvent;

		///////////////////////////////////////////

		public class DatabaseImplClass
		{
			ServerNetworkService_CloudFunctions owner;
			LiteDatabase database;
			ILiteCollection<StringItem> stringCollection;

			/////////////////////

			public class StringItem
			{
				public ObjectId Id { get; set; }
				//public int Id { get; set; }

				public string Key { get; set; }
				public string Value { get; set; }
			}

			/////////////////////

			public DatabaseImplClass( ServerNetworkService_CloudFunctions owner )
			{
				this.owner = owner;
			}

			public ServerNetworkService_CloudFunctions Owner
			{
				get { return owner; }
			}

			public LiteDatabase Database
			{
				get { return database; }
			}

			public bool Init( out string error )
			{
				error = "";

				try
				{
					var folder = Path.GetDirectoryName( owner.FullPathToDatabase );
					Directory.CreateDirectory( folder );

					//var fileName = Path.Combine( App.DataFolder, "Database.litedb" );
					var connection = "direct"; //"shared"
					var connectionString = $"Filename={owner.FullPathToDatabase};Connection={connection};Upgrade=true";
					if( owner.DatabaseReadOnly )
						connectionString += ";ReadOnly=true";

					int attemp = 0;
					again:
					try
					{
						database = new LiteDatabase( connectionString );

						stringCollection = database.GetCollection<StringItem>( "Strings" );
						stringCollection.EnsureIndex( "Key", true );
					}
					catch( Exception )
					{
						if( attemp < 3 )
						{
							attemp++;
							Thread.Sleep( 500 );
							goto again;
						}
						else
							throw;
					}
				}
				catch( Exception e )
				{
					error = e.Message;
					return false;
				}

				return true;
			}

			public void SaveString( string key, string text )
			{
				if( text != null )
				{
					var existingItem = stringCollection.FindOne( Query.EQ( "Key", key ) ); //FindOne( x => x.Key == key );
					if( existingItem != null )
					{
						existingItem.Value = text;
						stringCollection.Update( existingItem );
					}
					else
					{
						var newItem = new StringItem
						{
							Key = key,
							Value = text
						};
						stringCollection.Insert( newItem );
					}
				}
				else
				{
					var existingItem = stringCollection.FindOne( Query.EQ( "Key", key ) ); //FindOne( x => x.Key == key );
					if( existingItem != null )
						stringCollection.Delete( existingItem.Id );
				}
			}

			public void SaveStrings( string[] keys, string[] texts )
			{
				//optimizations?

				for( int n = 0; n < keys.Length; n++ )
					SaveString( keys[ n ], texts[ n ] );
			}

			public string LoadString( string key )
			{
				var item = stringCollection.FindOne( Query.EQ( "Key", key ) ); //FindOne( x => x.Key == key );
				return item?.Value;
			}

			public string[] LoadStrings( string[] keys )
			{
				//optimizations?

				var values = new string[ keys.Length ];
				for( int n = 0; n < values.Length; n++ )
					values[ n ] = LoadString( keys[ n ] );
				return values;
			}

			public int GetStringCount()
			{
				return stringCollection.Count();
			}

			public void ClearSaveStrings()
			{
				if( database == null )
					return;
				stringCollection.DeleteAll();
			}

			public void DeleteAll()
			{
				if( database == null )
					return;

				database.GetCollectionNames().ToList().ForEach( name =>
				{
					if( database.CollectionExists( name ) )
						database.DropCollection( name );
				} );
			}

			//public void Clear( bool clearSaveStringsOnly )
			//{
			//	if( database == null )
			//		return;

			//	if( clearSaveStringsOnly )
			//		stringCollection.DeleteAll();
			//	else
			//	{
			//		foreach( var name in database.GetCollectionNames() )
			//		{
			//			if( database.CollectionExists( name ) )
			//				database.GetCollection( name ).DeleteAll();
			//		}
			//	}
			//}

			//!!!!impl

			//public (string, DateTime)[] LoadStringsWithTime( string[] keys )
			//{
			//}

			//public string[] FindStringsByTime( DateTime from, DateTime to )
			//{
			//}

			//public void DeleteStringsByTime( DateTime from, DateTime to )
			//{
			//}
		}

		///////////////////////////////////////////

		public class CloudMethodInfo
		{
			public int Id;
			public string ClassName;
			public string MethodName;
			public string Key;
			public MethodInfo NetMethod;
			public ParameterInfo[] Parameters;
			public bool WithContextParameter;

			public ParameterInfo[] InputParameters;
			public ParameterInfo ReturnParameter;

			public CloudUserRole UserRole;
			public bool AddToCommands;
			public string Description;
			//public string AdditionalInfo;

			public int MaxCallPerClientPermit;
			////is not implemented
			//public int MaxCallPerClientQueue;
			public CloudMethodAttribute.LimitWindowMeasure LimitWindow = CloudMethodAttribute.LimitWindowMeasure.Minute;

			//

			public class ParameterInfo
			{
				public string Name;
				public bool IsReturn;
				public Type Type;
			}
		}

		///////////////////////////////////////////

		public enum FileSource
		{
			Project,
			Storage,
		}

		///////////////////////////////////////////

		public class CallMethodContext
		{
			internal ServerNetworkService_CloudFunctions cloudFunctions;
			internal ServerNode.Client client;
			internal CancellationToken cancellationToken;

			//

			public ServerNetworkService_CloudFunctions CloudFunctions
			{
				get { return cloudFunctions; }
			}

			public ServerNode.Client Client
			{
				get { return client; }
			}

			public CancellationToken CancellationToken
			{
				get { return cancellationToken; }
			}
		}

		///////////////////////////////////////////

		public class CallMethodLimiter
		{
			Dictionary<int, Queue<DateTime>> requests = new Dictionary<int, Queue<DateTime>>();

			//

			public bool AddCall( int defaultMaxCall, CloudMethodAttribute.LimitWindowMeasure defaultWindow, CloudMethodInfo method )
			{
				var maxCall = method.MaxCallPerClientPermit <= 0 ? defaultMaxCall : method.MaxCallPerClientPermit;
				var window = method.MaxCallPerClientPermit <= 0 ? defaultWindow : method.LimitWindow;

				if( !requests.TryGetValue( method.Id, out var queue ) )
				{
					queue = new Queue<DateTime>();
					requests[ method.Id ] = queue;
				}

				var now = DateTime.UtcNow;

				var totalSecondsLimit = 1.0;
				if( window == CloudMethodAttribute.LimitWindowMeasure.Minute )
					totalSecondsLimit = 60.0;
				else if( window == CloudMethodAttribute.LimitWindowMeasure.Hour )
					totalSecondsLimit = 60.0 * 60.0;

				//remove old requests
				while( queue.Count > 0 && ( now - queue.Peek() ).TotalSeconds >= totalSecondsLimit )
					queue.Dequeue();

				if( queue.Count < maxCall )
				{
					queue.Enqueue( now );
					return true;
				}

				return false;
			}
		}

		///////////////////////////////////////////

		public ServerNetworkService_CloudFunctions( string fullPathToDatabase, bool databaseReadOnly, string projectDirectory, out string error )
			: base( "CloudFunctions", 6 )
		{
			this.fullPathToDatabase = fullPathToDatabase;
			this.databaseReadOnly = databaseReadOnly;
			this.projectDirectory = projectDirectory;
			error = null;

			saveStringMessage = RegisterMessageType( "SaveString", 1, ReceiveMessage_SaveString );
			loadStringMessage = RegisterMessageType( "LoadString", 2, ReceiveMessage_LoadString );
			stringAnswerMessage = RegisterMessageType( "StringAnswer", 3 );
			callMethodMessage = RegisterMessageType( "CallMethod", 4, ReceiveMessage_CallMethod );
			callMethodAnswerMessage = RegisterMessageType( "CallMethodAnswer", 5 );
			getFilesInfoMessage = RegisterMessageType( "GetFilesInfo", 6, ReceiveMessage_GetFilesInfo );
			getFilesInfoAnswerMessage = RegisterMessageType( "GetFilesInfoAnswer", 7 );
			getDirectoryInfoMessage = RegisterMessageType( "GetDirectoryInfo", 8, ReceiveMessage_GetDirectoryInfo );
			getDirectoryInfoAnswerMessage = RegisterMessageType( "GetDirectoryInfoAnswer", 9 );
			downloadFileContentMessage = RegisterMessageType( "DownloadFileContent", 10, ReceiveMessage_DownloadFileContent );
			downloadFileContentAnswerMessage = RegisterMessageType( "DownloadFileContentAnswer", 11 );
			uploadFileContentMessage = RegisterMessageType( "UploadFileContent", 12, ReceiveMessage_UploadFileContent );
			uploadFileContentAnswerMessage = RegisterMessageType( "UploadFileContentAnswer", 13 );
			createDirectoryMessage = RegisterMessageType( "CreateDirectory", 14, ReceiveMessage_CreateDirectory );
			createDirectoryAnswerMessage = RegisterMessageType( "CreateDirectoryAnswer", 15 );
			deleteFilesMessage = RegisterMessageType( "DeleteFiles", 16, ReceiveMessage_DeleteFiles );
			deleteFilesAnswerMessage = RegisterMessageType( "DeleteFilesAnswer", 17 );
			deleteDirectoryMessage = RegisterMessageType( "DeleteDirectory", 18, ReceiveMessage_DeleteDirectory );
			deleteDirectoryAnswerMessage = RegisterMessageType( "DeleteDirectoryAnswer", 19 );
			cancelRequestMessage = RegisterMessageType( "CancelRequest", 20, ReceiveMessage_CancelRequest );
			storageDownloadFilesMessage = RegisterMessageType( "StorageDownloadFiles", 21, ReceiveMessage_StorageDownloadFiles );
			storageDownloadFilesAnswerMessage = RegisterMessageType( "StorageDownloadFilesAnswer", 22 );
			storageUploadFilesMessage = RegisterMessageType( "StorageUploadFiles", 23, ReceiveMessage_StorageUploadFiles );
			storageUploadFilesAnswerMessage = RegisterMessageType( "StorageUploadFilesAnswer", 24 );
			getCloudMethodInfoMessage = RegisterMessageType( "GetCloudMethodInfo", 25, ReceiveMessage_GetCloudMethodInfo );
			getCloudMethodInfoAnswerMessage = RegisterMessageType( "GetCloudMethodInfoAnswer", 26 );
			getCloudMethodsMessage = RegisterMessageType( "GetCloudMethods", 27, ReceiveMessage_GetCloudMethods );
			getCloudMethodsAnswerMessage = RegisterMessageType( "GetCloudMethodsAnswer", 28 );
			getCommonInfoMessage = RegisterMessageType( "GetCommonInfo", 29, ReceiveMessage_GetCommonInfo );
			getCommonInfoAnswerMessage = RegisterMessageType( "GetCommonInfoAnswer", 30 );

			if( !string.IsNullOrEmpty( FullPathToDatabase ) )
			{
				databaseImpl = new DatabaseImplClass( this );
				if( !databaseImpl.Init( out var error2 ) )
				{
					error = "Unable to initialize datatabase. " + error2;
					databaseImpl = null;
					return;
				}
			}
		}

		public string FullPathToDatabase
		{
			get { return fullPathToDatabase; }
		}

		public bool DatabaseReadOnly
		{
			get { return databaseReadOnly; }
		}

		public DatabaseImplClass DatabaseImpl
		{
			get { return databaseImpl; }
		}

		public string ProjectDirectory
		{
			get { return projectDirectory; }
		}

		class RequestToCancelItem
		{
			public long RequestID;
			public DateTime CreationTime;
		}

		public bool RemoveCancelledRequest( ServerNode.Client client, long requestID )
		{
			return requestsToCancel.TryRemove( (client, requestID), out _ );
		}

		void RemoveOldNotUsedRequestsToCancel( DateTime now )
		{
			foreach( var item in requestsToCancel.ToArray() )
			{
				var client = item.Key.Item1;
				var item2 = item.Value;

				if( ( now - item2.CreationTime ).TotalMinutes > 10 )
					requestsToCancel.TryRemove( item.Key, out _ );
			}
		}

		protected internal override void OnUpdate()
		{
			base.OnUpdate();

			var now = DateTime.UtcNow;
			if( ( now - requestsToCancelLastUpdateTime ).TotalSeconds > 30 )
			{
				RemoveOldNotUsedRequestsToCancel( now );
				requestsToCancelLastUpdateTime = now;
			}
		}

		///////////////////////////////////////////////

		public void SendStringAnswer( ServerNode.Client recepient, long requestID, string[] values, string error )
		{
			var m = BeginMessage( recepient, stringAnswerMessage );
			m.Writer.WriteVariable( requestID );
			if( values != null )
			{
				m.Writer.WriteVariableInt32( values.Length );
				for( int n = 0; n < values.Length; n++ )
					m.Writer.Write( values[ n ] );
			}
			else
				m.Writer.WriteVariableInt32( 0 );
			m.Writer.Write( error );
			m.End();
		}

		bool ReceiveMessage_SaveString( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			//read request ID
			var requestID = reader.ReadVariableInt64();

			//read count
			var count = reader.ReadVariableInt32();
			if( count > SaveStringMaxCountPerRequest )
			{
				error = $"String count limit is {SaveStringMaxCountPerRequest}.";
				return false;
			}

			//read keys and values
			var keys = new string[ count ];
			var valuesToSave = new string[ count ];
			for( int n = 0; n < count; n++ )
			{
				var key = reader.ReadString() ?? "";
				if( key.Length > SaveStringMaxKeyLength )
				{
					error = $"Key length limit is {SaveStringMaxKeyLength}.";
					return false;
				}
				var value = reader.ReadString();
				if( value != null && value.Length > SaveStringMaxValueLength )
				{
					error = $"Value length limit is {SaveStringMaxValueLength}.";
					return false;
				}

				keys[ n ] = key;
				valuesToSave[ n ] = value;
			}

			if( !reader.Complete() )
				return false;

			//if( sender.ReadOnly )
			//{
			//	SendStringAnswer( sender, requestID, null, "Access Denied: Read-only mode activated." );
			//	return true;
			//}

			//Task.Run( delegate ()
			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_SaveString", async delegate ()
			{
				try
				{
					var allow = false;
					string error2 = null;
					SaveStringEvent?.Invoke( this, sender, requestID, keys, valuesToSave, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( allow )
					{
						if( databaseImpl != null )
						{
							if( !databaseReadOnly )
							{
								databaseImpl.SaveStrings( keys, valuesToSave );
								SendStringAnswer( sender, requestID, null, null );
							}
							else
								SendStringAnswer( sender, requestID, null, "The database is read-only." );
						}
						else
							SendStringAnswer( sender, requestID, null, "The database is not enabled." );
					}
				}
				catch( Exception e )
				{
					SendStringAnswer( sender, requestID, null, e.Message );
				}
			} );

			return true;
		}

		bool ReceiveMessage_LoadString( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();

			var count = reader.ReadVariableInt32();
			if( count > SaveStringMaxCountPerRequest )
			{
				error = $"String count limit is {SaveStringMaxCountPerRequest}.";
				return false;
			}

			var keys = new string[ count ];
			for( int n = 0; n < count; n++ )
			{
				var key = reader.ReadString() ?? "";
				if( key.Length > SaveStringMaxKeyLength )
				{
					error = $"Key length limit is {SaveStringMaxKeyLength}.";
					return false;
				}
				keys[ n ] = key;
			}

			if( !reader.Complete() )
				return false;

			//Task.Run( delegate ()
			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_LoadString", async delegate ()
			{
				try
				{
					var allow = false;
					string error2 = null;
					LoadStringEvent?.Invoke( this, sender, requestID, keys, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( allow )
					{
						if( databaseImpl != null )
						{
							var valuesToLoad = databaseImpl.LoadStrings( keys );
							SendStringAnswer( sender, requestID, valuesToLoad, null );
						}
						else
							SendStringAnswer( sender, requestID, null, "Database is not enabled." );
					}
				}
				catch( Exception e )
				{
					SendStringAnswer( sender, requestID, null, e.Message );
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		bool CloudMethodGetParameterTypeSupported( Type parameterType, out string reason )
		{
			reason = null;

			if( parameterType.IsByRef )
			{
				reason = "By reference parameters are not supported.";
				return false;
			}

			if( ArrayDataWriter.TypeToWriteIsSupported( parameterType, 0 ) )
				return true;

			reason = "The type is not supported.";
			return false;
		}

		public bool RegisterCloudMethods( Type type, out string error )
		{
			error = "";

			var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;// BindingFlags.Instance;
			foreach( var netMethod in type.GetMethods( bindingFlags ) )
			{
				var attrib = netMethod.GetCustomAttribute<CloudMethodAttribute>();
				if( attrib != null )
				{
					var method = new CloudMethodInfo();
					method.ClassName = type.Name;
					method.MethodName = netMethod.Name;
					method.Id = Interlocked.Increment( ref cloudMethodIdCounter );
					method.Key = $"{type.Name}.{netMethod.Name}";
					method.NetMethod = netMethod;

					//get parameters from the method
					var parameters = new List<CloudMethodInfo.ParameterInfo>();

					var netParameters = netMethod.GetParameters();

					var withContextParameter = false;
					if( netParameters.Length >= 1 && typeof( CallMethodContext ).IsAssignableFrom( netParameters[ 0 ].ParameterType ) )
						withContextParameter = true;

					var indexOffset = withContextParameter ? 1 : 0;
					for( int n = indexOffset; n < netParameters.Length; n++ )
					{
						var netParameter = netParameters[ n ];
						var parameterType = netParameter.ParameterType;

						if( netParameter.IsOut )
						{
							error = "Out parameters are not supported.";
							return false;
						}

						if( !CloudMethodGetParameterTypeSupported( parameterType, out var reason ) )
						{
							error = $"Parameter type \"{parameterType.FullName}\" is not supported. " + reason;
							return false;
						}

						var p = new CloudMethodInfo.ParameterInfo();
						p.Name = netParameter.Name;
						p.Type = parameterType;
						parameters.Add( p );
					}

					if( netMethod.ReturnType != null && netMethod.ReturnType != typeof( void ) )
					{
						var parameterType = netMethod.ReturnType;

						//async method specific
						{
							if( parameterType == typeof( Task ) )
								parameterType = typeof( void );
							else if( parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof( Task<> ) )
								parameterType = parameterType.GetGenericArguments()[ 0 ];
						}

						if( parameterType != typeof( void ) )
						{
							if( !CloudMethodGetParameterTypeSupported( parameterType, out var reason ) )
							{
								error = $"Parameter type \"{parameterType.FullName}\" is not supported. " + reason;
								return false;
							}

							var p = new CloudMethodInfo.ParameterInfo();
							p.Name = "ReturnValue";
							p.IsReturn = true;
							p.Type = parameterType;
							parameters.Add( p );
						}
					}

					method.Parameters = parameters.ToArray();
					method.WithContextParameter = withContextParameter;

					method.InputParameters = method.Parameters.Where( p => !p.IsReturn ).ToArray();
					method.ReturnParameter = method.Parameters.FirstOrDefault( p => p.IsReturn );

					method.UserRole = attrib.UserRole;
					method.AddToCommands = attrib.AddToCommands;
					var descriptionAttribute = netMethod.GetCustomAttribute<DescriptionAttribute>();
					if( descriptionAttribute != null )
						method.Description = descriptionAttribute.Description;

					method.MaxCallPerClientPermit = attrib.MaxCallPerClientPermit;
					//method.MaxCallPerClientQueue = attrib.MaxCallPerClientQueue;
					method.LimitWindow = attrib.LimitWindow;

					cloudMethods[ method.Key ] = method;
					cloudMethodById[ method.Id ] = method;
				}
			}

			return true;
		}

		public CloudMethodInfo[] GetCloudMethods()
		{
			return cloudMethods.Values.ToArray();
		}

		public int CloudMethodCount
		{
			get { return cloudMethods.Count; }
		}

		///////////////////////////////////////////////

		public void SendGetCloudMethodInfoAnswer( ServerNode.Client recepient, long requestID, CloudMethodInfo method, string error )
		{
			var m = BeginMessage( recepient, getCloudMethodInfoAnswerMessage );
			m.Writer.WriteVariable( requestID );

			if( string.IsNullOrEmpty( error ) )
			{
				var methodData = new TextBlock();
				methodData.SetAttribute( "Id", method.Id.ToString() );
				for( int nParameter = 0; nParameter < method.Parameters.Length; nParameter++ )
				{
					var parameter = method.Parameters[ nParameter ];

					var parameterBlock = methodData.AddChild( "Parameter" );
					parameterBlock.SetAttribute( "Name", parameter.Name );
					parameterBlock.SetAttribute( "IsReturn", parameter.IsReturn.ToString() );
					parameterBlock.SetAttribute( "Type", parameter.Type.FullName );

					//if( parameter.CustomTypeProperties != null )
					//{
					//	for( int n = 0; n < parameter.CustomTypeProperties.Length; n++ )
					//	{
					//		var property = parameter.CustomTypeProperties[ n ];

					//		var customTypePropertyBlock = parameterBlock.AddChild( "CustomTypeProperty" );
					//		customTypePropertyBlock.SetAttribute( "Name", property.Name );
					//		if( property.PropertyType != null )
					//			customTypePropertyBlock.SetAttribute( "PropertyType", property.PropertyType.FullName );
					//		else if( property.FieldType != null )
					//			customTypePropertyBlock.SetAttribute( "FieldType", property.FieldType.FullName );
					//	}
					//}
				}

				if( method.AddToCommands )
					methodData.SetAttribute( "AddToCommands", method.AddToCommands.ToString() );
				if( !string.IsNullOrEmpty( method.Description ) )
					methodData.SetAttribute( "Description", method.Description );

				m.Writer.Write( methodData.DumpToString( false ) );
			}
			else
				m.Writer.Write( "" );

			m.Writer.Write( error );
			m.End();
		}

		///////////////////////////////////////////////

		static bool GetCloudMethodInfoIsValidName( string classNameOrMethodName )
		{
			if( string.IsNullOrEmpty( classNameOrMethodName ) )
				return false;
			for( int n = 0; n < classNameOrMethodName.Length; n++ )
			{
				var currentChar = classNameOrMethodName[ n ];
				if( !char.IsLetter( currentChar ) && !char.IsDigit( currentChar ) && currentChar != '_' )
					return false;
			}
			return true;
		}

		bool ReceiveMessage_GetCloudMethodInfo( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			var className = reader.ReadString() ?? "";
			if( !GetCloudMethodInfoIsValidName( className ) )
			{
				error = "Invalid class name.";
				return false;
			}
			var methodName = reader.ReadString() ?? "";
			if( !GetCloudMethodInfoIsValidName( methodName ) )
			{
				error = "Invalid method name.";
				return false;
			}

			//var parameterTypeCount = reader.ReadVariableInt32();
			//if( parameterTypeCount > 100 )
			//{
			//	error = "Parameter type count limit is 100.";
			//	return false;
			//}
			//var parameterTypeNames = new string[ parameterTypeCount ];
			//for( int n = 0; n < parameterTypeCount; n++ )
			//	parameterTypeNames[ n ] = reader.ReadString();

			if( !reader.Complete() )
				return false;

			var key = $"{className}.{methodName}";
			if( !cloudMethods.TryGetValue( key, out var method ) )
			{
				SendGetCloudMethodInfoAnswer( sender, requestID, null, $"Method not found \"{key}\"." );
				return true;
			}

			SendGetCloudMethodInfoAnswer( sender, requestID, method, null );
			return true;
		}

		///////////////////////////////////////////////

		public void SendGetCloudMethodsAnswer( ServerNode.Client recepient, long requestID, bool commandsOnly, string error )
		{
			var m = BeginMessage( recepient, getCloudMethodsAnswerMessage );
			m.Writer.WriteVariable( requestID );

			if( string.IsNullOrEmpty( error ) )
			{
				var methods = GetCloudMethods();
				if( commandsOnly )
					methods = methods.Where( m => m.AddToCommands ).ToArray();

				var rootBlock = new TextBlock();

				var classes = new EDictionary<string, List<CloudMethodInfo>>();
				foreach( var method in methods )
				{
					if( !classes.TryGetValue( method.ClassName, out var list ) )
					{
						list = new List<CloudMethodInfo>();
						classes[ method.ClassName ] = list;
					}
					list.Add( method );
				}

				foreach( var pair in classes )
				{
					var classBlock = rootBlock.AddChild( "Class" );
					classBlock.SetAttribute( "Name", pair.Key );
					foreach( var method in pair.Value )
					{
						var methodBlock = classBlock.AddChild( "Method" );
						methodBlock.SetAttribute( "Name", method.MethodName );
					}
				}

				m.Writer.Write( rootBlock.DumpToString( false ) );
			}
			else
				m.Writer.Write( "" );

			m.Writer.Write( error );
			m.End();
		}

		bool ReceiveMessage_GetCloudMethods( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			var commandsOnly = reader.ReadBoolean();
			if( !reader.Complete() )
				return false;

			SendGetCloudMethodsAnswer( sender, requestID, commandsOnly, null );
			return true;
		}

		///////////////////////////////////////////////

		public void SendCallMethodAnswer( ServerNode.Client recepient, long requestID, CloudMethodInfo method, object value, string error )
		{
			var m = BeginMessage( recepient, callMethodAnswerMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.WriteVariable( method.Id );
			writer.Write( error );
			if( string.IsNullOrEmpty( error ) )
			{
				if( method.ReturnParameter != null && value != null )
					writer.Write( method.ReturnParameter.Type, value );
			}
			m.End();
		}

		bool ReceiveMessage_CallMethod( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			//read request id
			var requestID = reader.ReadVariableInt64();

			//read method id
			var methodId = reader.ReadVariableInt();
			if( !cloudMethodById.TryGetValue( methodId, out var method ) )
				return false;

			//check input parameters size limit
			if( ( reader.EndPosition - reader.CurrentPosition ) > CloudMethodMaxInputParametersMaxBytes )
			{
				error = $"Input parameters size exceeds limit of {CloudMethodMaxInputParametersMaxBytes} bytes.";
				return false;
			}

			//read parameter count
			var parameterCount = reader.ReadVariableInt();
			if( parameterCount > 100 )
			{
				error = "Parameter count limit is 100.";
				return false;
			}

			//check parameter count
			var inputParameters = method.InputParameters;
			if( parameterCount != inputParameters.Length )
				return false;

			//read parameters
			var inputParameterValues = new object[ parameterCount ];
			try
			{
				var allocationStatistics = new ArrayDataReader.AllocationStatistics( CloudMethodMaxInputParametersMaxBytes, CloudMethodMaxInputParametersMaxObjects );
				for( int n = 0; n < inputParameters.Length; n++ )
					inputParameterValues[ n ] = reader.Read( inputParameters[ n ].Type, allocationStatistics );
			}
			catch( Exception e )
			{
				error = "Error reading input parameters. " + e.Message;
				return false;
			}

			if( !reader.Complete() )
				return false;

			//check user role
			if( sender.UserRole < method.UserRole )
			{
				SendCallMethodAnswer( sender, requestID, method, null, "Access Denied: Insufficient user role." );
				return true;
			}

			//if( sender.ReadOnly )
			//{
			//	SendCallMethodAnswer( sender, requestID, method, null, "Access Denied: Read-only mode activated." );
			//	return true;
			//}

			//check max call limits
			{
				if( sender.callMethodLimiter == null )
					sender.callMethodLimiter = new CallMethodLimiter();
				if( !sender.callMethodLimiter.AddCall( CloudMethodDefaultMaxCallPerClientPermit, CloudMethodDefaultLimitWindow, method ) )
				{
					SendCallMethodAnswer( sender, requestID, method, null, "Call limit exceeded." );
					return true;
				}
			}

			//run task for method call
			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_CallMethod", async delegate ()
			{
				try
				{
					var answerHandled = false;
					string error2 = null;
					CallMethodEvent?.Invoke( this, sender, requestID, method, inputParameterValues, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						using var cts = new CancellationTokenSource( new TimeSpan( 0, CloudMethodMaxTimeInMinutes, 0 ) );

						requestsCancellationTokens[ requestID ] = cts;
						try
						{
							var context = new CallMethodContext();
							context.cloudFunctions = this;
							context.client = sender;
							context.cancellationToken = cts.Token;

							var inputParameterValues2 = new object[ ( method.WithContextParameter ? 1 : 0 ) + inputParameterValues.Length ];
							if( method.WithContextParameter )
								inputParameterValues2[ 0 ] = context;

							var indexOffset = method.WithContextParameter ? 1 : 0;

							//Console.WriteLine( $"CallMethod: method={method.Key}, inputParameterValues.Length={inputParameterValues.Length}" );

							for( int nParam = 0; nParam < inputParameterValues.Length; nParam++ )
							{
								var demandedType = inputParameters[ nParam ].Type;
								var value = inputParameterValues[ nParam ];

								//Console.WriteLine( $"CallMethod param {nParam}: demanded={demandedType.FullName}, value type={(value != null ? value.GetType().FullName : "null")}, value={(value != null ? value.ToString() : "null")}" );

								if( value != null )
								{
									object newValue;

									if( demandedType == value.GetType() )
										newValue = value;
									else if( demandedType.IsAssignableFrom( value.GetType() ) )
										newValue = Convert.ChangeType( value, demandedType );
									else
									{
										//!!!!good?
										//convert via string
										newValue = SimpleTypes.ParseValue( demandedType, value.ToString() );
										//newParameters[ nParam ] = MetadataManager.AutoConvertValue( value, demandedType );
									}

									//check valid type
									if( newValue != null )
									{
										if( !demandedType.IsAssignableFrom( newValue.GetType() ) )
											throw new Exception( $"Unable to convert parameter \"{inputParameters[ nParam ].Name}\" to type \"{demandedType.FullName}\"." );
									}
									else
									{
										if( demandedType.IsValueType )
											throw new Exception( $"Unable to convert parameter \"{inputParameters[ nParam ].Name}\" to type \"{demandedType.FullName}\"." );
									}

									inputParameterValues2[ indexOffset + nParam ] = newValue;
								}
								else
								{
									//check valid type
									if( demandedType.IsValueType )
										throw new Exception( $"Unable to convert parameter \"{inputParameters[ nParam ].Name}\" to type \"{demandedType.FullName}\"." );
								}
							}

							var netMethod = method.NetMethod;
							var methodValue = netMethod.Invoke( null, inputParameterValues2 );

							Task task = methodValue as Task;
							if( task != null )
							{
								await task;

								// Check if the task's return type is Task<object> or Task
								if( netMethod.ReturnType.IsGenericType && netMethod.ReturnType.GetGenericTypeDefinition() == typeof( Task<> ) )
								{
									var resultProperty = netMethod.ReturnType.GetProperty( "Result" );
									methodValue = resultProperty?.GetValue( task );
								}
								else if( netMethod.ReturnType == typeof( Task ) )
									methodValue = null;
							}

							SendCallMethodAnswer( sender, requestID, method, methodValue, null );
						}
						finally
						{
							requestsCancellationTokens.TryRemove( requestID, out _ );
						}
					}
				}
				catch( Exception e )
				{
					var e2 = e.InnerException;
					if( e2 != null )
					{
						var message = e2.Message + "\r\n" + ( e2.StackTrace ?? e2.ToString() );
						SendCallMethodAnswer( sender, requestID, method, null, message );
					}
					else
					{
						var message = e.Message + "\r\n" + ( e.StackTrace ?? e.ToString() );
						SendCallMethodAnswer( sender, requestID, method, null, message );
					}
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		public void SendGetFilesInfoAnswer( ServerNode.Client recepient, long requestID, (long Size, DateTime LastModified, string Hash)[] values, string error )
		{
			var writer = new ArrayDataWriter( 1024 );
			writer.WriteVariable( requestID );
			if( values != null )
			{
				writer.WriteVariableInt32( values.Length );
				for( int n = 0; n < values.Length; n++ )
				{
					var item = values[ n ];
					writer.Write( item.Size );
					writer.Write( item.Hash );
					writer.Write( item.LastModified );
				}
			}
			else
				writer.WriteVariableInt32( 0 );
			writer.Write( error );

			SendMessage( recepient, getFilesInfoAnswerMessage, writer.AsArraySegment() );
		}

		public static bool IsValidVirtualPath( string filePath, bool isStoragePath )
		{
			if( string.IsNullOrEmpty( filePath ) )
				return false;

			if( filePath[ 0 ] == '.' || filePath[ 0 ] == '/' || filePath[ 0 ] == '\\' )
				return false;

			if( filePath.Contains( ".." ) )
				return false;

			var invalidPathChars = Path.GetInvalidPathChars();
			if( filePath.Any( c => invalidPathChars.Contains( c ) ) )
				return false;

			if( isStoragePath )
			{
				//Linode limits

				var lengthWithHash = 0;
				try
				{
					lengthWithHash = filePath.Length + 34 + Path.GetExtension( filePath ).Length;
				}
				catch { }

				if( lengthWithHash >= 1024 )
					return false;

				////-40 for hash suffixes in the storage. by idea can save hash names or make own storage service
				//var maxLength = 1024 - 40;
				//if( filePath.Length > maxLength )
				//	return false;
			}
			else
			{
				var maxLength = 4096;
				if( filePath.Length > maxLength )
					return false;
			}

			//try
			//{
			//	if( Path.GetFileName( filePath ).Length >= 255 )
			//		return false;
			//}
			//catch
			//{
			//	return false;
			//}

			return true;
		}

		bool ReceiveMessage_GetFilesInfo( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			var source = reader.ReadBoolean() ? FileSource.Storage : FileSource.Project;
			var count = reader.ReadVariableInt32();
			if( count > FileRequestMaxItemCount )
			{
				error = $"File count limit is {FileRequestMaxItemCount}.";
				return false;
			}
			var filePaths = new string[ count ];
			for( int n = 0; n < count; n++ )
				filePaths[ n ] = reader.ReadString() ?? "";

			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//fix paths
			for( int n = 0; n < filePaths.Length; n++ )
				filePaths[ n ] = PathUtility.NormalizePath( filePaths[ n ] );

			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_GetFilesInfo", async delegate ()
			{
				try
				{
					//check for invalid paths
					for( int n = 0; n < filePaths.Length; n++ )
					{
						if( !IsValidVirtualPath( filePaths[ n ], source == FileSource.Storage ) )
						{
							SendGetFilesInfoAnswer( sender, requestID, null, "Invalid file path." );
							return;
						}
					}

					var allow = false;
					string error2 = null;
					CheckFileAccessEvent?.Invoke( this, sender, requestID, source, filePaths, anyData, FileOperationAccess.Get, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );
					if( !allow )
						throw new Exception( "Access denied." );

					var answerHandled = false;
					GetFilesInfoEvent?.Invoke( this, sender, requestID, source, filePaths, anyData, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						if( source == FileSource.Project )
						{
							//Project source

							if( string.IsNullOrEmpty( ProjectDirectory ) )
								throw new Exception( "Project directory is not configured." );

							var resultValues = new (long Size, DateTime LastModified, string Hash)[ filePaths.Length ];

							for( int n = 0; n < resultValues.Length; n++ )
							{
								var sourceFilePath = filePaths[ n ];
								var fullPath = Path.Combine( ProjectDirectory, sourceFilePath );

								var fileInfo = new FileInfo( fullPath );
								//!!!!
								if( fileInfo.Exists )//&& string.Compare( fileInfo.Extension, ".hash", true ) != 0 )
								{

									//!!!!what about hashes

									var hash = "";// GetOrCalculateFileHash( fileInfo );
									resultValues[ n ] = (fileInfo.Length, fileInfo.LastWriteTimeUtc, hash);
								}
								else
									resultValues[ n ] = (-1, new DateTime(), "");
							}

							SendGetFilesInfoAnswer( sender, requestID, resultValues, null );
						}
						else
						{
							//Storage source

							using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );

							if( string.IsNullOrEmpty( CloudServerProcessUtility.CommandLineParameters.ServerCheckCode ) )
								throw new Exception( "Server check code is not configured." );
							var resultTask = CloudServiceFunctions.StorageGetFilesInfoAsync( filePaths, CloudServerProcessUtility.CommandLineParameters.ServerCheckCode, cancellationToken.Token );

							//var resultTask = CloudServerProcessUtility.StorageGetFilesInfoAsync( filePaths, cancellationToken.Token );

							while( !resultTask.IsCompleted )
							{
								if( sender.Status == NetworkStatus.Disconnected || RemoveCancelledRequest( sender, requestID ) )
									cancellationToken.Cancel();
								await Task.Delay( 1 );
							}
							var result = resultTask.Result;

							if( !string.IsNullOrEmpty( result.Error ) )
								throw new Exception( result.Error );
							if( result.Items.Length != filePaths.Length )
								throw new Exception( "Invalid result from the Storage." );

							var resultValues = new (long Size, DateTime LastModified, string Hash)[ filePaths.Length ];
							for( int n = 0; n < resultValues.Length; n++ )
							{
								var resultValue = result.Items[ n ];
								resultValues[ n ] = (resultValue.Size, resultValue.LastModified, "");
							}
							SendGetFilesInfoAnswer( sender, requestID, resultValues, null );
						}
					}
				}
				catch( Exception e )
				{
					SendGetFilesInfoAnswer( sender, requestID, null, e.Message );
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		public void SendGetDirectoryInfoAnswer( ServerNode.Client recepient, long requestID, List<(string Path, long Size, DateTime LastModifiedUtc, string Hash, bool IsDirectory)> values, string error )
		{
			var writer = new ArrayDataWriter( 1024 );
			writer.WriteVariable( requestID );
			if( values != null )
			{
				writer.WriteVariableInt32( values.Count );
				for( int n = 0; n < values.Count; n++ )
				{
					var item = values[ n ];
					writer.Write( item.Path );
					writer.WriteVariableInt64( item.Size );
					writer.Write( item.Hash );
					writer.Write( item.LastModifiedUtc );
					writer.Write( item.IsDirectory );
				}
			}
			else
				writer.WriteVariableInt32( 0 );
			writer.Write( error );
			SendMessage( recepient, getDirectoryInfoAnswerMessage, writer.AsArraySegment() );
		}

		bool ReceiveMessage_GetDirectoryInfo( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			var source = reader.ReadBoolean() ? FileSource.Storage : FileSource.Project;
			var sourcePath = reader.ReadString() ?? "";
			var searchPattern = reader.ReadString();
			var searchOption = reader.ReadBoolean() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//fix path
			sourcePath = PathUtility.NormalizePath( sourcePath );
			if( sourcePath.Length > 0 && sourcePath[ sourcePath.Length - 1 ] == Path.DirectorySeparatorChar )
				sourcePath = sourcePath.Substring( 0, sourcePath.Length - 1 );

			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_GetDirectoryInfo", async delegate ()
			{
				try
				{
					//check for invalid path
					if( !IsValidVirtualPath( sourcePath, source == FileSource.Storage ) )
					{
						SendGetDirectoryInfoAnswer( sender, requestID, null, "Invalid file path." );
						return;
					}

					var allow = false;
					string error2 = null;
					CheckFileAccessEvent?.Invoke( this, sender, requestID, source, new string[] { sourcePath }, anyData, FileOperationAccess.Get, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );
					if( !allow )
						throw new Exception( "Access denied." );

					var answerHandled = false;
					GetDirectoryInfoEvent?.Invoke( this, sender, requestID, source, ref sourcePath, searchOption, anyData, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						if( source == FileSource.Project )
						{
							//Project source

							if( string.IsNullOrEmpty( ProjectDirectory ) )
							{
								SendGetDirectoryInfoAnswer( sender, requestID, null, "Project directory is not configured." );
								return;
							}

							var directoryFullPath = Path.Combine( ProjectDirectory, sourcePath );

							var directoryInfo = new DirectoryInfo( directoryFullPath );
							if( directoryInfo.Exists )
							{
								var resultValues = new List<(string Path, long Size, DateTime LastModifiedUtc, string Hash, bool IsDirectory)>();

								foreach( var directoryInfo2 in directoryInfo.GetDirectories( "*", searchOption ) )
								{
									var virtualPath = Path.Combine( sourcePath, directoryInfo2.FullName.Substring( directoryFullPath.Length + 1 ) );
									resultValues.Add( (virtualPath, 0, directoryInfo2.LastWriteTimeUtc, "", true) );
								}

								foreach( var fileInfo in directoryInfo.GetFiles( "*", searchOption ) )
								{
									//!!!!
									var extension = fileInfo.Extension;
									//!!!!
									//if( string.Compare( extension, ".hash", true ) == 0 )
									//	continue;

									var virtualPath = Path.Combine( sourcePath, fileInfo.FullName.Substring( directoryFullPath.Length + 1 ) );


									//!!!!hashes


									var hash = "";// GetOrCalculateFileHash( fileInfo );

									resultValues.Add( (virtualPath, fileInfo.Length, fileInfo.LastWriteTimeUtc, hash, false) );
								}

								SendGetDirectoryInfoAnswer( sender, requestID, resultValues, null );
							}
							else
								SendGetDirectoryInfoAnswer( sender, requestID, null, $"The directory does not exist. \"{directoryFullPath}\"." );
						}
						else
						{
							//Storage source

							using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 2, 0 ) );

							if( string.IsNullOrEmpty( CloudServerProcessUtility.CommandLineParameters.ServerCheckCode ) )
								throw new Exception( "Server check code is not configured." );
							var resultTask = CloudServiceFunctions.StorageGetDirectoryInfoAsync( sourcePath, searchPattern, searchOption, CloudServerProcessUtility.CommandLineParameters.ServerCheckCode, cancellationToken.Token );
							//var resultTask = CloudServerProcessUtility.StorageGetDirectoryInfoAsync( sourcePath, searchOption, cancellationToken.Token );

							while( !resultTask.IsCompleted )
							{
								if( sender.Status == NetworkStatus.Disconnected || RemoveCancelledRequest( sender, requestID ) )
									cancellationToken.Cancel();
								await Task.Delay( 1 );
							}
							var result = resultTask.Result;

							if( !string.IsNullOrEmpty( result.Error ) )
								throw new Exception( result.Error );

							var resultValues = new List<(string Name, long Size, DateTime LastModifiedUtc, string Hash, bool IsDirectory)>( result.Items.Length );
							foreach( var file in result.Items )
								resultValues.Add( (file.Name, file.Size, file.LastModified, "", file.IsDirectory) );
							SendGetDirectoryInfoAnswer( sender, requestID, resultValues, null );
						}
					}
				}
				catch( Exception e )
				{
					SendGetDirectoryInfoAnswer( sender, requestID, null, e.Message );
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		public struct DownloadFileContentPart
		{
			public string FileName;
			public long PartStart;
			public long PartEnd;

			public int Size
			{
				get { return (int)( PartEnd - PartStart ); }
			}
		}

		void SendDownloadFileContentAnswerError( ServerNode.Client client, long requestID, string error )
		{
			var writer = new ArrayDataWriter();
			writer.WriteVariable( requestID );
			writer.WriteVariable( 0 );
			writer.Write( error );
			SendMessage( client, downloadFileContentAnswerMessage, writer.AsArraySegment() );
		}

		bool ReceiveMessage_DownloadFileContent( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			const int maxPartCountInGroup = 10000;

			var requestID = reader.ReadVariableInt64();
			//Console.WriteLine( $"ReceiveMessage_DownloadFileContent: " + requestID.ToString() );
			var partCount = reader.ReadVariableInt32();
			if( partCount > maxPartCountInGroup )
			{
				error = $"Part count limit is {maxPartCountInGroup}.";
				return false;
			}
			var parts = new DownloadFileContentPart[ partCount ];
			for( int n = 0; n < partCount; n++ )
			{
				ref var part = ref parts[ n ];
				part.FileName = reader.ReadString() ?? "";
				part.PartStart = reader.ReadVariableInt64();
				part.PartEnd = reader.ReadVariableInt64();
			}
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//fix paths
			for( int n = 0; n < partCount; n++ )
			{
				ref var part = ref parts[ n ];
				part.FileName = PathUtility.NormalizePath( part.FileName );
			}


			//tasks settings?

			//Task.Run( delegate ()
			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_DownloadFileContent", async delegate ()
			{
				try
				{
					if( string.IsNullOrEmpty( ProjectDirectory ) )
					{
						SendDownloadFileContentAnswerError( sender, requestID, "Project directory is not configured." );
						return;
					}

					//check for invalid paths and data
					for( int n = 0; n < partCount; n++ )
					{
						var part = parts[ n ]; //ref var part = ref parts[ n ];
						if( !IsValidVirtualPath( part.FileName, false ) )
						{
							SendDownloadFileContentAnswerError( sender, requestID, "Invalid file path." );
							return;
						}
						if( part.Size < 0 )
						{
							SendDownloadFileContentAnswerError( sender, requestID, "Invalid request data." );
							return;
						}
					}

					//check max data size
					int totalRequestedDataSize = 0;
					{
						for( int n = 0; n < partCount; n++ )
						{
							var part = parts[ n ]; //ref var part = ref parts[ n ];
							totalRequestedDataSize += part.Size;
						}
					}
					if( totalRequestedDataSize > ReceiveMessageDownloadFilesContentMaxSize )
					{
						SendDownloadFileContentAnswerError( sender, requestID, $"Max block size limit {ReceiveMessageDownloadFilesContentMaxSize}." );
						return;
					}

					var filePaths = new ESet<string>();
					foreach( var pair in parts )
						filePaths.AddWithCheckAlreadyContained( pair.FileName );

					var allow = false;
					string error2 = null;
					CheckFileAccessEvent?.Invoke( this, sender, requestID, FileSource.Project, filePaths.ToArray(), anyData, FileOperationAccess.Get, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );
					if( !allow )
						throw new Exception( "Access denied." );

					//update MaxLifetime
					if( sender.MaxLifetime != 0 )
					{
						var requiredMaxLifeTime = sender.GetCurrentLifetime() + 60 * 2;
						if( sender.MaxLifetime < requiredMaxLifeTime )
							sender.MaxLifetime = requiredMaxLifeTime;
					}

					var answerHandled = false;
					DownloadFileContentEvent?.Invoke( this, sender, requestID, parts, anyData, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						byte[] tempData = null;

						//!!!!GC
						var writer = new ArrayDataWriter( 1024 );
						writer.WriteVariable( requestID );
						writer.WriteVariable( partCount );
						for( int n = 0; n < partCount; n++ )
						{
							var part = parts[ n ];
							var partSize = part.Size;

							if( tempData == null || tempData.Length < partSize )
								tempData = new byte[ partSize ];
							var data = tempData;// new byte[ partSize ];
							{
								var fullPath = Path.Combine( ProjectDirectory, part.FileName );

								var fileInfo = new FileInfo( fullPath );
								if( fileInfo.Exists )
								{
									using( FileStream fs = new FileStream( fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
									{
										fs.Seek( part.PartStart, SeekOrigin.Begin );
										if( fs.Read( data, 0, partSize ) != partSize )
										{
											SendDownloadFileContentAnswerError( sender, requestID, "File reading failed." );
											return;
										}
									}
								}
								else
								{
									SendDownloadFileContentAnswerError( sender, requestID, "File not found." );
									return;
								}
							}

							writer.WriteVariable( partSize );
							writer.Write( data, 0, partSize );
						}
						writer.Write( "" );
						SendMessage( sender, downloadFileContentAnswerMessage, writer.AsArraySegment() );
						//Console.WriteLine( $"ReceiveMessage_DownloadFileContent: " + requestID.ToString() + " Sent" );
					}
				}
				catch( Exception e )
				{
					SendDownloadFileContentAnswerError( sender, requestID, e.Message );
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		public class UploadFileContentPart
		{
			public string FileName { get; set; }
			public long PartStart { get; set; }
			public long PartEnd { get; set; }
			public byte[] Data;

			public int Size
			{
				get { return (int)( PartEnd - PartStart ); }
			}
		}

		void SendUploadFileContentAnswer( ServerNode.Client client, long requestID, string error )
		{
			var writer = new ArrayDataWriter();
			writer.WriteVariable( requestID );
			writer.Write( error );
			SendMessage( client, uploadFileContentAnswerMessage, writer.AsArraySegment() );
		}

		bool ReceiveMessage_UploadFileContent( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			const int maxPartCountInGroup = 10000;

			var requestID = reader.ReadVariableInt64();
			var partCount = reader.ReadVariableInt32();
			if( partCount > maxPartCountInGroup )
			{
				error = $"Part count limit is {maxPartCountInGroup}.";
				return false;
			}

			long totalSize = 0L;

			var parts = new UploadFileContentPart[ partCount ];
			for( int n = 0; n < partCount; n++ )
			{
				var part = new UploadFileContentPart();

				part.FileName = reader.ReadString() ?? "";
				part.PartStart = reader.ReadVariableInt64();
				part.PartEnd = reader.ReadVariableInt64();

				if( totalSize + part.Size > UploadFilesMaxBlockSize )
				{
					error = $"Invalid message. totalSize + part.Size > UploadFilesMaxBlockSize. {UploadFilesMaxBlockSize}";
					return false;
				}
				if( part.Size < 0 )
				{
					error = $"Invalid message. part.Size < 0.";
					return false;
				}

				part.Data = new byte[ part.Size ];
				reader.ReadBuffer( part.Data, 0, part.Size );

				totalSize += part.Size;

				parts[ n ] = part;
			}

			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//if( sender.ReadOnly )
			//{
			//	SendUploadFileContentAnswer( sender, requestID, "Access Denied: Read-only mode activated." );
			//	return true;
			//}

			//fix paths
			for( int n = 0; n < partCount; n++ )
			{
				ref var part = ref parts[ n ];
				part.FileName = PathUtility.NormalizePath( part.FileName );
			}

			//Task.Run( delegate ()
			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_UploadFileContent", async delegate ()
			{
				try
				{
					if( string.IsNullOrEmpty( ProjectDirectory ) )
					{
						SendUploadFileContentAnswer( sender, requestID, "Project directory is not configured." );
						return;
					}

					//check for invalid paths and data
					for( int n = 0; n < partCount; n++ )
					{
						var part = parts[ n ]; //ref var part = ref parts[ n ];
						if( !IsValidVirtualPath( part.FileName, false ) )
						{
							SendUploadFileContentAnswer( sender, requestID, "Invalid file path." );
							return;
						}
						if( part.Size < 0 )
						{
							SendUploadFileContentAnswer( sender, requestID, "Invalid request data." );
							return;
						}
					}

					var filePaths = new ESet<string>();
					foreach( var part in parts )
						filePaths.AddWithCheckAlreadyContained( part.FileName );

					var allow = false;
					string error2 = null;
					CheckFileAccessEvent?.Invoke( this, sender, requestID, FileSource.Project, filePaths.ToArray(), anyData, FileOperationAccess.UploadFile, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );
					if( !allow )
						throw new Exception( "Access denied." );

					//update MaxLifetime
					if( sender.MaxLifetime != 0 )
					{
						var requiredMaxLifeTime = sender.GetCurrentLifetime() + 60 * 2;
						if( sender.MaxLifetime < requiredMaxLifeTime )
							sender.MaxLifetime = requiredMaxLifeTime;
					}

					var answerHandled = false;
					UploadFileContentEvent?.Invoke( this, sender, requestID, parts, anyData, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						for( int n = 0; n < parts.Length; n++ )
						{
							var part = parts[ n ];
							var fullPath = Path.Combine( ProjectDirectory, part.FileName );

							if( part.PartStart == 0 )
							{
								var directory = Path.GetDirectoryName( fullPath );
								if( !Directory.Exists( directory ) )
									Directory.CreateDirectory( directory );
								if( File.Exists( fullPath ) )
									File.Delete( fullPath );
								File.WriteAllBytes( fullPath, part.Data );
							}
							else
							{
								using( var stream = new FileStream( fullPath, FileMode.Append ) )
									stream.Write( part.Data, 0, part.Data.Length );
							}
						}

						SendUploadFileContentAnswer( sender, requestID, null );
					}
				}
				catch( Exception e )
				{
					SendUploadFileContentAnswer( sender, requestID, e.Message );
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		public void SendCreateDirectoryAnswer( ServerNode.Client recepient, long requestID, string error )
		{
			var writer = new ArrayDataWriter( 32 );
			writer.WriteVariable( requestID );
			writer.Write( error );
			SendMessage( recepient, createDirectoryAnswerMessage, writer.AsArraySegment() );
		}

		bool ReceiveMessage_CreateDirectory( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			var source = reader.ReadBoolean() ? FileSource.Storage : FileSource.Project;
			var directoryPath = reader.ReadString() ?? "";
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//if( sender.ReadOnly )
			//{
			//	SendCreateDirectoryAnswer( sender, requestID, "Access Denied: Read-only mode activated." );
			//	return true;
			//}

			//fix path
			directoryPath = PathUtility.NormalizePath( directoryPath );
			if( directoryPath.Length > 0 && directoryPath[ directoryPath.Length - 1 ] == Path.DirectorySeparatorChar )
				directoryPath = directoryPath.Substring( 0, directoryPath.Length - 1 );

			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_CreateDirectory", async delegate ()
			{
				try
				{
					//check for invalid path
					if( !IsValidVirtualPath( directoryPath, source == FileSource.Storage ) )
					{
						SendCreateDirectoryAnswer( sender, requestID, "Invalid file path." );
						return;
					}

					var allow = false;
					string error2 = null;
					CheckFileAccessEvent?.Invoke( this, sender, requestID, source, new string[] { directoryPath }, anyData, FileOperationAccess.CreateDirectory, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );
					if( !allow )
						throw new Exception( "Access denied." );

					var answerHandled = false;
					CreateDirectoryEvent?.Invoke( this, sender, requestID, source, ref directoryPath, anyData, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						if( source == FileSource.Project )
						{
							//Project source

							if( string.IsNullOrEmpty( ProjectDirectory ) )
							{
								SendCreateDirectoryAnswer( sender, requestID, "Project directory is not configured." );
								return;
							}

							var directoryFullPath = Path.Combine( ProjectDirectory, directoryPath );
							if( !Directory.Exists( directoryFullPath ) )
								Directory.CreateDirectory( directoryFullPath );

							SendCreateDirectoryAnswer( sender, requestID, null );
						}
						else
						{
							//Storage source

							using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );

							if( string.IsNullOrEmpty( CloudServerProcessUtility.CommandLineParameters.ServerCheckCode ) )
								throw new Exception( "Server check code is not configured." );
							var resultTask = CloudServiceFunctions.StorageCreateDirectoriesAsync( new[] { directoryPath }, CloudServerProcessUtility.CommandLineParameters.ServerCheckCode, cancellationToken.Token );
							//var resultTask = CloudServerProcessUtility.StorageCreateDirectoryAsync( directoryPath, cancellationToken.Token );

							while( !resultTask.IsCompleted )
							{
								if( sender.Status == NetworkStatus.Disconnected || RemoveCancelledRequest( sender, requestID ) )
									cancellationToken.Cancel();
								await Task.Delay( 1 );
							}
							var result = resultTask.Result;

							if( !string.IsNullOrEmpty( result.Error ) )
								throw new Exception( result.Error );

							SendCreateDirectoryAnswer( sender, requestID, null );
						}
					}
				}
				catch( Exception e )
				{
					SendCreateDirectoryAnswer( sender, requestID, e.Message );
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		public void SendDeleteFilesAnswer( ServerNode.Client recepient, long requestID, string error )
		{
			var writer = new ArrayDataWriter( 32 );
			writer.WriteVariable( requestID );
			writer.Write( error );
			SendMessage( recepient, deleteFilesAnswerMessage, writer.AsArraySegment() );
		}

		bool ReceiveMessage_DeleteFiles( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			var source = reader.ReadBoolean() ? FileSource.Storage : FileSource.Project;
			var count = reader.ReadVariableInt32();
			if( count > FileRequestMaxItemCount )
			{
				error = $"Delete files count limit is {FileRequestMaxItemCount}.";
				return false;
			}
			var filePaths = new string[ count ];
			for( int n = 0; n < count; n++ )
				filePaths[ n ] = reader.ReadString() ?? "";
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//if( sender.ReadOnly )
			//{
			//	SendDeleteFilesAnswer( sender, requestID, "Access Denied: Read-only mode activated." );
			//	return true;
			//}

			//fix paths
			for( int n = 0; n < filePaths.Length; n++ )
				filePaths[ n ] = PathUtility.NormalizePath( filePaths[ n ] );

			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_DeleteFiles", async delegate ()
			{
				try
				{
					//check for invalid paths
					for( int n = 0; n < filePaths.Length; n++ )
					{
						if( !IsValidVirtualPath( filePaths[ n ], source == FileSource.Storage ) )
						{
							SendDeleteFilesAnswer( sender, requestID, null );
							return;
						}
					}

					var allow = false;
					string error2 = null;
					CheckFileAccessEvent?.Invoke( this, sender, requestID, source, filePaths, anyData, FileOperationAccess.Delete, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );
					if( !allow )
						throw new Exception( "Access denied." );

					var answerHandled = false;
					DeleteFilesEvent?.Invoke( this, sender, requestID, source, filePaths, anyData, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						if( source == FileSource.Project )
						{
							//Project source

							if( string.IsNullOrEmpty( ProjectDirectory ) )
								throw new Exception( "Project directory is not configured." );

							for( int n = 0; n < filePaths.Length; n++ )
							{
								var sourceFilePath = filePaths[ n ];
								var fullPath = Path.Combine( ProjectDirectory, sourceFilePath );

								if( File.Exists( fullPath ) )
									File.Delete( fullPath );
							}

							SendDeleteFilesAnswer( sender, requestID, null );
						}
						else
						{
							//Storage source


							//!!!!
							//var deleteLocalFiles = false;


							using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 2, 0 ) );

							if( string.IsNullOrEmpty( CloudServerProcessUtility.CommandLineParameters.ServerCheckCode ) )
								throw new Exception( "Server check code is not configured." );

							var objects = new CloudServiceFunctions.DeleteObjectsItem[ filePaths.Length ];
							for( int n = 0; n < filePaths.Length; n++ )
								objects[ n ] = new CloudServiceFunctions.DeleteObjectsItem { Name = filePaths[ n ] };
							var resultTask = CloudServiceFunctions.StorageDeleteObjectsAsync( objects, CloudServerProcessUtility.CommandLineParameters.ServerCheckCode, cancellationToken.Token );

							//var resultTask = CloudServerProcessUtility.StorageDeleteFilesAsync( filePaths, deleteLocalFiles, cancellationToken.Token );

							while( !resultTask.IsCompleted )
							{
								if( sender.Status == NetworkStatus.Disconnected || RemoveCancelledRequest( sender, requestID ) )
									cancellationToken.Cancel();
								await Task.Delay( 1 );
							}
							var result = resultTask.Result;

							if( !string.IsNullOrEmpty( result.Error ) )
								throw new Exception( result.Error );

							SendDeleteFilesAnswer( sender, requestID, null );
						}
					}
				}
				catch( Exception e )
				{
					SendDeleteFilesAnswer( sender, requestID, e.Message );
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		public void SendDeleteDirectoryAnswer( ServerNode.Client recepient, long requestID, string error )
		{
			var writer = new ArrayDataWriter( 32 );
			writer.WriteVariable( requestID );
			writer.Write( error );
			SendMessage( recepient, deleteDirectoryAnswerMessage, writer.AsArraySegment() );
		}

		bool ReceiveMessage_DeleteDirectory( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			var source = reader.ReadBoolean() ? FileSource.Storage : FileSource.Project;
			var directoryPath = reader.ReadString() ?? "";
			var recursive = reader.ReadBoolean();
			var clear = reader.ReadBoolean();
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//if( sender.ReadOnly )
			//{
			//	SendDeleteDirectoryAnswer( sender, requestID, "Access Denied: Read-only mode activated." );
			//	return true;
			//}

			//fix path
			directoryPath = PathUtility.NormalizePath( directoryPath );
			if( directoryPath.Length > 0 && directoryPath[ directoryPath.Length - 1 ] == Path.DirectorySeparatorChar )
				directoryPath = directoryPath.Substring( 0, directoryPath.Length - 1 );

			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNetworkService_CloudFunctions: ReceiveMessage_DeleteDirectory", async delegate ()
			{
				try
				{
					//check for invalid path
					if( !IsValidVirtualPath( directoryPath, source == FileSource.Storage ) )
					{
						SendDeleteDirectoryAnswer( sender, requestID, "Invalid file path." );
						return;
					}

					var allow = false;
					string error2 = null;
					CheckFileAccessEvent?.Invoke( this, sender, requestID, source, new string[] { directoryPath }, anyData, FileOperationAccess.Delete, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );
					if( !allow )
						throw new Exception( "Access denied." );

					var answerHandled = false;
					DeleteDirectoryEvent?.Invoke( this, sender, requestID, source, directoryPath, recursive, clear, anyData, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						if( source == FileSource.Project )
						{
							//Project source

							if( string.IsNullOrEmpty( ProjectDirectory ) )
							{
								SendDeleteDirectoryAnswer( sender, requestID, "Project directory is not configured." );
								return;
							}

							var directoryFullPath = Path.Combine( ProjectDirectory, directoryPath );
							if( Directory.Exists( directoryFullPath ) )
							{
								if( clear )
									IOUtility.ClearDirectory( directoryFullPath );
								else
									Directory.Delete( directoryFullPath, recursive );
							}

							SendDeleteDirectoryAnswer( sender, requestID, null );
						}
						else
						{
							//Storage source

							using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 10, 0 ) );

							if( string.IsNullOrEmpty( CloudServerProcessUtility.CommandLineParameters.ServerCheckCode ) )
								throw new Exception( "Server check code is not configured." );

							var objects = new CloudServiceFunctions.DeleteObjectsItem[ 1 ];
							objects[ 0 ] = new CloudServiceFunctions.DeleteObjectsItem { Name = directoryPath, IsDirectory = true };
							var resultTask = CloudServiceFunctions.StorageDeleteObjectsAsync( objects, CloudServerProcessUtility.CommandLineParameters.ServerCheckCode, cancellationToken.Token );

							//var resultTask = CloudServerProcessUtility.StorageDeleteDirectoryAsync( directoryPath, recursive, clear, cancellationToken.Token );

							while( !resultTask.IsCompleted )
							{
								if( sender.Status == NetworkStatus.Disconnected || RemoveCancelledRequest( sender, requestID ) )
									cancellationToken.Cancel();
								await Task.Delay( 1 );
							}
							var result = resultTask.Result;

							if( !string.IsNullOrEmpty( result.Error ) )
								throw new Exception( result.Error );

							SendDeleteDirectoryAnswer( sender, requestID, null );
						}
					}
				}
				catch( Exception e )
				{
					SendDeleteDirectoryAnswer( sender, requestID, e.Message );
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		bool ReceiveMessage_CancelRequest( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			if( !reader.Complete() )
				return false;

			//add to cancel list
			var item = new RequestToCancelItem();
			item.RequestID = requestID;
			item.CreationTime = DateTime.UtcNow;
			requestsToCancel[ (sender, requestID) ] = item;

			//cancel token if exists
			if( requestsCancellationTokens.TryRemove( requestID, out var cancellationTokenSource ) )
				cancellationTokenSource.Cancel();

			return true;
		}

		///////////////////////////////////////////////

		public void SendStorageDownloadFilesAnswer( ServerNode.Client recepient, long requestID, string[] downloadUrls, string error )
		{
			var writer = new ArrayDataWriter( 128 );
			writer.WriteVariable( requestID );
			if( downloadUrls != null )
			{
				writer.WriteVariableInt32( downloadUrls.Length );
				for( int n = 0; n < downloadUrls.Length; n++ )
					writer.Write( downloadUrls[ n ] );
			}
			else
				writer.WriteVariableInt32( 0 );
			writer.Write( error );
			SendMessage( recepient, storageDownloadFilesAnswerMessage, writer.AsArraySegment() );
		}

		bool ReceiveMessage_StorageDownloadFiles( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			var count = reader.ReadVariableInt32();
			if( count > FileRequestMaxItemCount )
			{
				error = $"Get files count limit is {FileRequestMaxItemCount}.";
				return false;
			}
			var filePaths = new string[ count ];
			for( int n = 0; n < count; n++ )
				filePaths[ n ] = reader.ReadString() ?? "";
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//fix paths
			for( int n = 0; n < filePaths.Length; n++ )
				filePaths[ n ] = PathUtility.NormalizePath( filePaths[ n ] );

			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Hour, "ServerNetworkService_CloudFunctions: ReceiveMessage_StorageDownloadFiles", async delegate ()
			{
				try
				{
					//check for invalid paths
					for( int n = 0; n < filePaths.Length; n++ )
					{
						if( !IsValidVirtualPath( filePaths[ n ], true ) )
						{
							SendStorageDownloadFilesAnswer( sender, requestID, null, "Invalid file path." );
							return;
						}
					}

					var allow = false;
					string error2 = null;
					CheckFileAccessEvent?.Invoke( this, sender, requestID, FileSource.Storage, filePaths, anyData, FileOperationAccess.Get, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );
					if( !allow )
						throw new Exception( "Access denied." );

					var answerHandled = false;
					StorageDownloadFilesEvent?.Invoke( this, sender, requestID, filePaths, anyData, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );

						if( string.IsNullOrEmpty( CloudServerProcessUtility.CommandLineParameters.ServerCheckCode ) )
							throw new Exception( "Server check code is not configured." );
						var resultTask = CloudServiceFunctions.StorageGetContentUrlsAsync( filePaths, false, false, CloudServerProcessUtility.CommandLineParameters.ServerCheckCode, cancellationToken.Token );
						while( !resultTask.IsCompleted )
						{
							if( sender.Status == NetworkStatus.Disconnected || RemoveCancelledRequest( sender, requestID ) )
								cancellationToken.Cancel();
							await Task.Delay( 1 );
						}
						var result = resultTask.Result;

						if( !string.IsNullOrEmpty( result.Error ) )
							throw new Exception( result.Error );
						if( result.Urls.Length != filePaths.Length )
							throw new Exception( "Invalid result from the Storage." );

						SendStorageDownloadFilesAnswer( sender, requestID, resultTask.Result.Urls, null );


						//using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
						//var resultTask = CloudServerProcessUtility.StorageGetDownloadUrlsAsync( filePaths, cancellationToken.Token );
						//while( !resultTask.IsCompleted )
						//{
						//	if( sender.Status == NetworkStatus.Disconnected || RemoveCancelledRequest( sender, requestID ) )
						//		cancellationToken.Cancel();
						//	await Task.Delay( 1 );
						//}
						//var result = resultTask.Result;

						//if( !string.IsNullOrEmpty( result.Error ) )
						//	throw new Exception( result.Error );
						//if( result.DownloadUrls.Length != filePaths.Length )
						//	throw new Exception( "Invalid result from the Storage." );

						//SendStorageDownloadFilesAnswer( sender, requestID, resultTask.Result.DownloadUrls, null );
					}
				}
				catch( Exception e )
				{
					SendStorageDownloadFilesAnswer( sender, requestID, null, e.Message );
				}
			} );

			return true;
		}

		///////////////////////////////////////////////

		public void SendStorageUploadFilesAnswer( ServerNode.Client recepient, long requestID, string[] uploadUrls, string error )
		{
			var writer = new ArrayDataWriter( 128 );
			writer.WriteVariable( requestID );
			if( uploadUrls != null )
			{
				writer.WriteVariableInt32( uploadUrls.Length );
				for( int n = 0; n < uploadUrls.Length; n++ )
					writer.Write( uploadUrls[ n ] );
			}
			else
				writer.WriteVariableInt32( 0 );
			writer.Write( error );
			SendMessage( recepient, storageUploadFilesAnswerMessage, writer.AsArraySegment() );
		}

		bool ReceiveMessage_StorageUploadFiles( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			var requestID = reader.ReadVariableInt64();
			var count = reader.ReadVariableInt32();
			if( count > FileRequestMaxItemCount )
			{
				error = $"Get files count limit is {FileRequestMaxItemCount}.";
				return false;
			}
			var filePaths = new string[ count ];
			for( int n = 0; n < count; n++ )
				filePaths[ n ] = reader.ReadString() ?? "";
			var anyData = reader.ReadString();
			if( !reader.Complete() )
				return false;

			//if( sender.ReadOnly )
			//{
			//	SendStorageUploadFilesAnswer( sender, requestID, null, "Access Denied: Read-only mode activated." );
			//	return true;
			//}

			//fix paths
			for( int n = 0; n < filePaths.Length; n++ )
				filePaths[ n ] = PathUtility.NormalizePath( filePaths[ n ] );

			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Hour, "ServerNetworkService_CloudFunctions: ReceiveMessage_StorageUploadFiles", async delegate ()
			{
				try
				{
					//check for invalid paths
					for( int n = 0; n < filePaths.Length; n++ )
					{
						if( !IsValidVirtualPath( filePaths[ n ], true ) )
						{
							SendStorageUploadFilesAnswer( sender, requestID, null, "Invalid file path." );
							return;
						}
					}

					var allow = false;
					string error2 = null;
					CheckFileAccessEvent?.Invoke( this, sender, requestID, FileSource.Storage, filePaths, anyData, FileOperationAccess.UploadFile, ref allow, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );
					if( !allow )
						throw new Exception( "Access denied." );

					var answerHandled = false;
					StorageUploadFilesEvent?.Invoke( this, sender, requestID, filePaths, anyData, ref answerHandled, ref error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						throw new Exception( error2 );

					if( !answerHandled )
					{
						using var cancellationToken = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );

						if( string.IsNullOrEmpty( CloudServerProcessUtility.CommandLineParameters.ServerCheckCode ) )
							throw new Exception( "Server check code is not configured." );
						var resultTask = CloudServiceFunctions.StorageGetContentUrlsAsync( filePaths, true, false, CloudServerProcessUtility.CommandLineParameters.ServerCheckCode, cancellationToken.Token );
						while( !resultTask.IsCompleted )
						{
							if( sender.Status == NetworkStatus.Disconnected || RemoveCancelledRequest( sender, requestID ) )
								cancellationToken.Cancel();
							await Task.Delay( 1 );
						}
						var result = resultTask.Result;

						if( !string.IsNullOrEmpty( result.Error ) )
							throw new Exception( result.Error );
						if( result.Urls.Length != filePaths.Length )
							throw new Exception( "Invalid result from the Storage." );

						for( int n = 0; n < result.Urls.Length; n++ )
						{
							if( string.IsNullOrEmpty( result.Urls[ n ] ) )
								throw new Exception( "Empty upload url." );
						}

						SendStorageUploadFilesAnswer( sender, requestID, resultTask.Result.Urls, null );
					}
				}
				catch( Exception e )
				{
					SendStorageUploadFilesAnswer( sender, requestID, null, e.Message );
				}
			} );

			return true;
		}

		bool ReceiveMessage_GetCommonInfo( ServerNode.Client sender, MessageType messageType, ArrayDataReader reader, ref string error )
		{
			if( !reader.Complete() )
				return false;

			var block = new TextBlock();
			block.SetAttribute( "UploadFilesMaxBlockSize", UploadFilesMaxBlockSize.ToString() );
			block.SetAttribute( "ReceiveMessageDownloadFilesContentMaxSize", ReceiveMessageDownloadFilesContentMaxSize.ToString() );

			var m = BeginMessage( sender, getCommonInfoAnswerMessage );
			m.Writer.Write( block.DumpToString() );
			m.End();

			return true;
		}
	}
#endif

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ClientNetworkService_CloudFunctions : ClientService
	{
		public int DownloadFilesMaxBlockSize { get; set; } = 10 * 1024 * 1024;
		public int UploadFilesMaxBlockSize { get; set; } = 10 * 1024 * 1024;

		//!!!!not used
		//public int DownloadFilesMaxQueueSize { get; set; } = 10;

		MessageType saveStringMessage;
		MessageType loadStringMessage;
		MessageType stringAnswerMessage;
		MessageType callMethodMessage;
		MessageType callMethodAnswerMessage;
		MessageType getFilesInfoMessage;
		MessageType getFilesInfoAnswerMessage;
		MessageType getDirectoryInfoMessage;
		MessageType getDirectoryInfoAnswerMessage;
		MessageType downloadFileContentMessage;
		MessageType downloadFileContentAnswerMessage;
		MessageType uploadFileContentMessage;
		MessageType uploadFileContentAnswerMessage;
		MessageType createDirectoryMessage;
		MessageType createDirectoryAnswerMessage;
		MessageType deleteFilesMessage;
		MessageType deleteFilesAnswerMessage;
		MessageType deleteDirectoryMessage;
		MessageType deleteDirectoryAnswerMessage;
		MessageType cancelRequestMessage;
		MessageType storageDownloadFilesMessage;
		MessageType storageDownloadFilesAnswerMessage;
		MessageType storageUploadFilesMessage;
		MessageType storageUploadFilesAnswerMessage;
		MessageType getCloudMethodInfoMessage;
		MessageType getCloudMethodInfoAnswerMessage;
		MessageType getCloudMethodsMessage;
		MessageType getCloudMethodsAnswerMessage;
		MessageType getCommonInfoMessage;
		MessageType getCommonInfoAnswerMessage;

		long requestIdCounter;
		ConcurrentDictionary<long, AnswerItem> answers = new ConcurrentDictionary<long, AnswerItem>();
		DateTime answersLastOldRemoveTime;

		string connectionErrorReceived;

		ConcurrentDictionary<string, CloudMethodInfo> cloudMethods = new ConcurrentDictionary<string, CloudMethodInfo>();
		ConcurrentDictionary<int, CloudMethodInfo> cloudMethodById = new ConcurrentDictionary<int, CloudMethodInfo>();
		//ConcurrentHashSet<Type> callMethodKnownParameterTypes = new ConcurrentHashSet<Type>();

		//GetCallMethodsResult getCallMethodsResultCached;

		//support for speed limit
		//maxBytesPerSecond

		List<Assembly> registeredCloudMethodAssemblies = new List<Assembly>();

		volatile ServerCommonInfoClass serverCommonInfo;
		public ServerCommonInfoClass ServerCommonInfo { get { return serverCommonInfo; } }

		///////////////////////////////////////////////

		public class SimpleResult
		{
			public string Error { get; set; }
		}

		class AnswerItem
		{
			public string Error;
			public DateTime CreationTime;
		}

		///////////////////////////////////////////////

		//common info may be dynamic in the future
		public class ServerCommonInfoClass
		{
			public int UploadFilesMaxBlockSize { get; internal set; }
			public int ReceiveMessageDownloadFilesContentMaxSize { get; internal set; }
		}

		///////////////////////////////////////////////

		public ClientNetworkService_CloudFunctions()
			: base( "CloudFunctions", 6 )
		{
			saveStringMessage = RegisterMessageType( "SaveString", 1 );
			loadStringMessage = RegisterMessageType( "LoadString", 2 );
			stringAnswerMessage = RegisterMessageType( "StringAnswer", 3, ReceiveMessage_StringAnswer );
			callMethodMessage = RegisterMessageType( "CallMethod", 4 );
			callMethodAnswerMessage = RegisterMessageType( "CallMethodAnswer", 5, ReceiveMessage_CallMethodAnswer );
			getFilesInfoMessage = RegisterMessageType( "GetFilesInfo", 6 );
			getFilesInfoAnswerMessage = RegisterMessageType( "GetFilesInfoAnswer", 7, ReceiveMessage_GetFilesInfoAnswer );
			getDirectoryInfoMessage = RegisterMessageType( "GetDirectoryInfo", 8 );
			getDirectoryInfoAnswerMessage = RegisterMessageType( "GetDirectoryInfoAnswer", 9, ReceiveMessage_GetDirectoryInfoAnswer );
			downloadFileContentMessage = RegisterMessageType( "DownloadFileContent", 10 );
			downloadFileContentAnswerMessage = RegisterMessageType( "DownloadFileContentAnswer", 11, ReceiveMessage_DownloadFileContentAnswer );
			uploadFileContentMessage = RegisterMessageType( "UploadFileContent", 12 );
			uploadFileContentAnswerMessage = RegisterMessageType( "UploadFileContentAnswer", 13, ReceiveMessage_UploadFileContentAnswer );
			createDirectoryMessage = RegisterMessageType( "CreateDirectory", 14 );
			createDirectoryAnswerMessage = RegisterMessageType( "CreateDirectoryAnswer", 15, ReceiveMessage_CreateDirectoryAnswer );
			deleteFilesMessage = RegisterMessageType( "DeleteFiles", 16 );
			deleteFilesAnswerMessage = RegisterMessageType( "DeleteFilesAnswer", 17, ReceiveMessage_DeleteFilesAnswer );
			deleteDirectoryMessage = RegisterMessageType( "DeleteDirectory", 18 );
			deleteDirectoryAnswerMessage = RegisterMessageType( "DeleteDirectoryAnswer", 19, ReceiveMessage_DeleteDirectoryAnswer );
			cancelRequestMessage = RegisterMessageType( "CancelRequest", 20 );
			storageDownloadFilesMessage = RegisterMessageType( "StorageDownloadFiles", 21 );
			storageDownloadFilesAnswerMessage = RegisterMessageType( "StorageDownloadFilesAnswer", 22, ReceiveMessage_StorageDownloadFilesAnswer );
			storageUploadFilesMessage = RegisterMessageType( "StorageUploadFiles", 23 );
			storageUploadFilesAnswerMessage = RegisterMessageType( "StorageUploadFilesAnswer", 24, ReceiveMessage_StorageUploadFilesAnswer );
			getCloudMethodInfoMessage = RegisterMessageType( "GetCloudMethodInfo", 25 );
			getCloudMethodInfoAnswerMessage = RegisterMessageType( "GetCloudMethodInfoAnswer", 26, ReceiveMessage_GetCloudMethodInfoAnswer );
			getCloudMethodsMessage = RegisterMessageType( "GetCloudMethods", 27 );
			getCloudMethodsAnswerMessage = RegisterMessageType( "GetCloudMethodsAnswer", 28, ReceiveMessage_GetCloudMethodsAnswer );
			getCommonInfoMessage = RegisterMessageType( "GetCommonInfo", 29 );
			getCommonInfoAnswerMessage = RegisterMessageType( "GetCommonInfoAnswer", 30, ReceiveMessage_GetCommonInfoAnswer );

			//register NeoAxis.Core.dll
			RegisterAssemblyForCloudMethodTypes( typeof( Vector2 ).Assembly );
		}

		long GetRequestID()
		{
			return Interlocked.Increment( ref requestIdCounter );
		}

		public string ConnectionErrorReceived
		{
			get { return connectionErrorReceived; }
			set { connectionErrorReceived = value; }
		}

		protected internal override void OnUpdate( DateTime utcNow )
		{
			base.OnUpdate( utcNow );

			if( ( utcNow - answersLastOldRemoveTime ).TotalSeconds > 30 )
			{
				answersLastOldRemoveTime = utcNow;
				RemoveOldNotUsedAnswers( utcNow );
			}
		}

		void SendCancelRequest( long requestID )
		{
			var m = BeginMessage( cancelRequestMessage );
			m.Writer.WriteVariable( requestID );
			m.End();
		}

		void RemoveOldNotUsedAnswers( DateTime utcNow )
		{
			foreach( var pair in answers.ToArray() )
			{
				var requestID = pair.Key;
				var item = pair.Value;

				if( ( utcNow - item.CreationTime ).TotalMinutes > 10 )
					answers.Remove( requestID, out _ );
			}
		}

		T GetAnswerAndRemove<T>( long requestID ) where T : AnswerItem
		{
			if( answers.TryRemove( requestID, out var item ) )
				return item as T;
			return null;
		}

		///////////////////////////////////////////////

		class StringAnswerItem : AnswerItem
		{
			public string[] Values;
		}

		public class SaveStringsResult
		{
			public string Error { get; set; }
		}

		public class LoadStringsResult
		{
			public string[] Values { get; set; }
			public string Error { get; set; }
		}

		public class LoadStringResult
		{
			public string Value { get; set; }
			public string Error { get; set; }
		}

		bool ReceiveMessage_StringAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var count = reader.ReadVariableInt32();
			string[] values = null;
			if( count > 0 )
			{
				values = new string[ count ];
				for( int n = 0; n < count; n++ )
					values[ n ] = reader.ReadString();
			}
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				//sense?
				//var handled = false;
				//StringAnswerEvent?.Invoke( this, requestID, values, ref handled );

				//if( !handled )
				{
					var answerItem = new StringAnswerItem();
					answerItem.Values = values;
					answerItem.Error = error;
					answerItem.CreationTime = DateTime.UtcNow;
					answers[ requestID ] = answerItem;
				}
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		void SendSaveString( long requestID, string[] keys, string[] values )
		{
			var m = BeginMessage( saveStringMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.WriteVariableInt32( keys.Length );
			for( int n = 0; n < keys.Length; n++ )
			{
				writer.Write( keys[ n ] );
				writer.Write( values != null ? values[ n ] : null );
			}
			m.End();
		}

		void SendLoadString( long requestID, string[] keys )
		{
			var m = BeginMessage( loadStringMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.WriteVariableInt32( keys.Length );
			for( int n = 0; n < keys.Length; n++ )
				writer.Write( keys[ n ] );
			m.End();
		}

		/// <summary>
		/// Save string values by keys to the server's database. Gettings the updated data for next SaveStrings and LoadStrings calls are guaranteed when this method's task is completed.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="values"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<SaveStringsResult> SaveStringsAsync( string[] keys, string[] values, CancellationToken cancellationToken = default )
		{
			var requestID = GetRequestID();
			SendSaveString( requestID, keys, values );

			while( true )
			{
				var answer = GetAnswerAndRemove<StringAnswerItem>( requestID );
				if( answer != null )
					return new SaveStringsResult() { Error = answer.Error };

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
				{
					SendCancelRequest( requestID );
					return new SaveStringsResult { Error = "Operation was canceled." };
				}
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new SaveStringsResult() { Error = ConnectionErrorReceived };
			}
		}

		/// <summary>
		/// Save string values by keys to the server's database. Gettings the updated data for next SaveStrings and LoadStrings calls are guaranteed when this method's task is completed.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<SaveStringsResult> SaveStringAsync( string key, string value, CancellationToken cancellationToken = default )
		{
			return await SaveStringsAsync( new string[] { key }, new string[] { value }, cancellationToken );
		}

		/// <summary>
		/// Loads string values by keys from the server's database. Gettings the updated data for next SaveStrings and LoadStrings calls are guaranteed when this method's task is completed.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<LoadStringsResult> LoadStringsAsync( string[] keys, CancellationToken cancellationToken = default )
		{
			var requestID = GetRequestID();
			SendLoadString( requestID, keys );

			while( true )
			{
				var answer = GetAnswerAndRemove<StringAnswerItem>( requestID );
				if( answer != null )
					return new LoadStringsResult { Values = answer.Values, Error = answer.Error };

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
				{
					SendCancelRequest( requestID );
					return new LoadStringsResult { Error = "Operation was canceled." };
				}
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new LoadStringsResult { Error = ConnectionErrorReceived };
			}
		}

		/// <summary>
		/// Loads string values by keys from the server's database. Gettings the updated data for next SaveStrings and LoadStrings calls are guaranteed when this method's task is completed.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<LoadStringResult> LoadStringAsync( string key, CancellationToken cancellationToken = default )
		{
			var result = await LoadStringsAsync( new string[] { key }, cancellationToken );
			if( string.IsNullOrEmpty( result.Error ) )
				return new LoadStringResult { Value = result.Values[ 0 ] };
			else
				return new LoadStringResult { Error = result.Error };
		}

		///////////////////////////////////////////////

		class GetCloudMethodInfoAnswerItem : AnswerItem
		{
			public TextBlock MethodData;
		}

		public class CloudMethodInfo
		{
			public int Id;
			public string ClassName;
			public string MethodName;
			public ParameterInfo[] Parameters;

			public ParameterInfo[] InputParameters;
			public ParameterInfo ReturnParameter;

			public bool AddToCommands;
			public string Description;

			//

			public class ParameterInfo
			{
				public string Name;
				public bool IsReturn;

				public string TypeName;
				public Type Type;

				//public ArrayDataWriter.TypeToWriteCustomStructureProperty[] CustomTypeProperties;
			}
		}

		public class GetCloudMethodInfoResult
		{
			public CloudMethodInfo Method;
			public string Error;
		}

		//public CancellationTokenSource CallMethodDefaultCancellationTokenSource
		//{
		//	get { return callMethodDefaultCancellationTokenSource; }
		//	set { callMethodDefaultCancellationTokenSource = value; }
		//}

		bool ReceiveMessage_GetCloudMethodInfoAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			//var methodId = reader.ReadVariableInt32();
			var methodDataText = reader.ReadString();
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new GetCloudMethodInfoAnswerItem();
				//answerItem.Id = methodId;

				if( !string.IsNullOrEmpty( methodDataText ) )
				{
					var methodData = TextBlock.Parse( methodDataText, out error );
					if( !string.IsNullOrEmpty( error ) )
						throw new Exception( error );
					answerItem.MethodData = methodData;
				}

				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		void SendGetCloudMethodInfo( long requestID, string className, string methodName )
		{
			var m = BeginMessage( getCloudMethodInfoMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.Write( className );
			writer.Write( methodName );
			m.End();
		}

		public async Task<GetCloudMethodInfoResult> GetCloudMethodInfoAsync( string className, string methodName, CancellationToken cancellationToken = default )
		{
			try
			{
				var key = $"{className}_{methodName}";
				if( !cloudMethods.TryGetValue( key, out var method ) )
				{
					var requestID = GetRequestID();
					SendGetCloudMethodInfo( requestID, className, methodName );

					while( true )
					{
						var answer = GetAnswerAndRemove<GetCloudMethodInfoAnswerItem>( requestID );
						if( answer != null )
						{
							if( !string.IsNullOrEmpty( answer.Error ) )
								return new GetCloudMethodInfoResult { Error = answer.Error };
							else
							{
								method = new CloudMethodInfo();
								method.Id = int.Parse( answer.MethodData.GetAttribute( "Id" ) );
								//method.Id = answer.Id;
								method.ClassName = className;
								method.MethodName = methodName;

								var parameters = new List<CloudMethodInfo.ParameterInfo>();

								foreach( var parameterBlock in answer.MethodData.Children )
								{
									if( parameterBlock.Name == "Parameter" )
									{
										var parameter = new CloudMethodInfo.ParameterInfo();
										parameter.Name = parameterBlock.GetAttribute( "Name" );
										parameter.IsReturn = bool.Parse( parameterBlock.GetAttribute( "IsReturn" ) );
										parameter.TypeName = parameterBlock.GetAttribute( "Type" );

										parameter.Type = FindTypeForCloudMethod( parameter.TypeName );

										//var simpleTypeItem = SimpleTypes.GetTypeItem( parameter.TypeName );
										//if( simpleTypeItem != null )
										//{
										//	parameter.Type = simpleTypeItem.Type;
										//}
										//else
										//{
										//	var type = Type.GetType( parameter.TypeName );
										//	if( type != null )
										//		parameter.Type = type;
										//	else
										//	{
										//		//!!!!check
										//		type = typeof( Vector2 ).Assembly.GetType( parameter.TypeName );
										//		if( type != null )
										//			parameter.Type = type;
										//	}
										//}

										//if( parameter.Type == null )
										//	return new GetCallMethodInfoResult { Error = $"Type not found \"{parameter.TypeName}\"." };

										var customProperties = new List<ArrayDataWriter.TypeToWriteCustomStructureProperty>();

										foreach( var childBlock in parameterBlock.Children )
										{
											if( childBlock.Name == "CustomTypeProperty" )
											{
												var p = new ArrayDataWriter.TypeToWriteCustomStructureProperty();
												p.Name = childBlock.GetAttribute( "Name" );

												var fieldTypeName = childBlock.GetAttribute( "FieldType" );
												var propertyTypeName = childBlock.GetAttribute( "PropertyType" );

												if( !string.IsNullOrEmpty( fieldTypeName ) )
												{
													var type = FindTypeForCloudMethod( fieldTypeName );
													if( type == null )
														return new GetCloudMethodInfoResult { Error = $"Type not found \"{fieldTypeName}\"." };
													p.FieldType = type;

													//var type = Type.GetType( fieldTypeName );
													//if( type == null )
													//	type = typeof( Vector2 ).Assembly.GetType( fieldTypeName );
													//if( type == null )
													//	return new GetCallMethodInfoResult { Error = $"Type not found \"{fieldTypeName}\"." };
													//p.FieldType = type;
												}

												if( !string.IsNullOrEmpty( propertyTypeName ) )
												{
													var type = FindTypeForCloudMethod( propertyTypeName );
													if( type == null )
														return new GetCloudMethodInfoResult { Error = $"Type not found \"{propertyTypeName}\"." };
													p.PropertyType = type;

													//var type = Type.GetType( propertyTypeName );
													//if( type == null )
													//	type = typeof( Vector2 ).Assembly.GetType( propertyTypeName );
													//if( type == null )
													//	return new GetCallMethodInfoResult { Error = $"Type not found \"{propertyTypeName}\"." };
													//p.PropertyType = type;
												}

												if( p.FieldType != null || p.PropertyType != null )
													customProperties.Add( p );
											}
										}


										//var customProperties = new List<CallMethodInfo.CustomTypeProperty>();

										//foreach( var childBlock in parameterBlock.Children )
										//{
										//	if( childBlock.Name == "CustomTypeProperty" )
										//	{
										//		var p = new CallMethodInfo.CustomTypeProperty();
										//		p.Name = childBlock.GetAttribute( "Name" );
										//		p.FieldTypeName = childBlock.GetAttribute( "FieldType" );
										//		p.PropertyTypeName = childBlock.GetAttribute( "PropertyType" );

										//		if( !string.IsNullOrEmpty( p.FieldTypeName ) )
										//		{
										//			var type = Type.GetType( p.FieldTypeName );
										//			if( type == null )
										//			{
										//				//!!!!check
										//				type = typeof( Vector2 ).Assembly.GetType( p.FieldTypeName );
										//			}
										//			if( type == null )
										//				return new GetCallMethodInfoResult { Error = $"Type not found \"{p.FieldTypeName}\"." };
										//			p.FieldType = type;
										//		}

										//		if( !string.IsNullOrEmpty( p.PropertyTypeName ) )
										//		{
										//			var type = Type.GetType( p.PropertyTypeName );
										//			if( type == null )
										//			{
										//				//!!!!check
										//				type = typeof( Vector2 ).Assembly.GetType( p.PropertyTypeName );
										//			}
										//			if( type == null )
										//				return new GetCallMethodInfoResult { Error = $"Type not found \"{p.PropertyTypeName}\"." };
										//			p.PropertyType = type;
										//		}

										//		if( p.FieldType != null || p.PropertyType != null )
										//			customProperties.Add( p );
										//	}
										//}


										//if( customProperties.Count != 0 )
										//	parameter.CustomTypeProperties = customProperties.ToArray();
										//else
										//{
										if( parameter.Type == null )
											return new GetCloudMethodInfoResult { Error = $"Type not found \"{parameter.TypeName}\"." };
										//}

										parameters.Add( parameter );
									}
								}

								method.Parameters = parameters.ToArray();

								method.InputParameters = method.Parameters.Where( p => !p.IsReturn ).ToArray();
								method.ReturnParameter = method.Parameters.FirstOrDefault( p => p.IsReturn );

								method.AddToCommands = bool.Parse( answer.MethodData.GetAttribute( "AddToCommands", "False" ) );
								method.Description = answer.MethodData.GetAttribute( "Description" );

								cloudMethods[ key ] = method;
								cloudMethodById[ method.Id ] = method;

								break;
							}
						}

						await Task.Delay( 1 );
						if( cancellationToken.IsCancellationRequested )
						{
							SendCancelRequest( requestID );
							return new GetCloudMethodInfoResult { Error = "Operation was canceled." };
						}
						if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
							return new GetCloudMethodInfoResult { Error = ConnectionErrorReceived };
					}
				}

				return new GetCloudMethodInfoResult { Method = method };

			}
			catch( Exception e )
			{
				return new GetCloudMethodInfoResult { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		class GetCloudMethodsAnswerItem : AnswerItem
		{
			public TextBlock ClassesData;
		}

		public class GetCloudMethodsResult
		{
			public ClassInfo[] Classes;
			public string Error;

			/////////////////////

			public class ClassInfo
			{
				public string ClassName;
				public string[] MethodNames;
			}
		}

		bool ReceiveMessage_GetCloudMethodsAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var classesDataText = reader.ReadString();
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new GetCloudMethodsAnswerItem();

				if( !string.IsNullOrEmpty( classesDataText ) )
				{
					var classesData = TextBlock.Parse( classesDataText, out error );
					if( !string.IsNullOrEmpty( error ) )
						throw new Exception( error );
					answerItem.ClassesData = classesData;
				}

				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		void SendGetCloudMethods( long requestID, bool commandsOnly )
		{
			var m = BeginMessage( getCloudMethodsMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.Write( commandsOnly );
			m.End();
		}

		public async Task<GetCloudMethodsResult> GetCloudMethodsAsync( bool commandsOnly, CancellationToken cancellationToken = default )
		{
			try
			{
				var requestID = GetRequestID();
				SendGetCloudMethods( requestID, commandsOnly );

				List<GetCloudMethodsResult.ClassInfo> classes = null;

				while( true )
				{
					var answer = GetAnswerAndRemove<GetCloudMethodsAnswerItem>( requestID );
					if( answer != null )
					{
						if( !string.IsNullOrEmpty( answer.Error ) )
							return new GetCloudMethodsResult { Error = answer.Error };
						else
						{
							classes = new List<GetCloudMethodsResult.ClassInfo>();

							foreach( var classBlock in answer.ClassesData.Children )
							{
								if( classBlock.Name == "Class" )
								{
									var className = classBlock.GetAttribute( "Name" );
									if( string.IsNullOrEmpty( className ) )
										return new GetCloudMethodsResult { Error = "Class name is empty." };

									var methodNames = new List<string>();
									foreach( var methodBlock in classBlock.Children )
									{
										if( methodBlock.Name == "Method" )
										{
											var methodName = methodBlock.GetAttribute( "Name" );
											if( string.IsNullOrEmpty( methodName ) )
												return new GetCloudMethodsResult { Error = "Method name is empty." };
											methodNames.Add( methodName );
										}
									}

									var classInfo = new GetCloudMethodsResult.ClassInfo();
									classInfo.ClassName = className;
									classInfo.MethodNames = methodNames.ToArray();

									classes.Add( classInfo );
								}
							}

							break;
						}
					}

					await Task.Delay( 1 );
					if( cancellationToken.IsCancellationRequested )
					{
						SendCancelRequest( requestID );
						return new GetCloudMethodsResult { Error = "Operation was canceled." };
					}
					if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
						return new GetCloudMethodsResult { Error = ConnectionErrorReceived };
				}

				return new GetCloudMethodsResult { Classes = classes.ToArray() };
			}
			catch( Exception e )
			{
				return new GetCloudMethodsResult { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		class CallMethodAnswerItem : AnswerItem
		{
			public object ResultValue;
			//public object[] ResultCustomStructureValues;
		}

		public class CallMethodResult<T>
		{
			public T Value { get; set; }
			public string Error { get; set; }
		}

		public class CallMethodResultNoValue
		{
			public string Error { get; set; }
		}

		bool ReceiveMessage_CallMethodAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();

			var methodId = reader.ReadVariableInt();
			if( !cloudMethodById.TryGetValue( methodId, out var method ) )
			{
				additionalErrorMessage = "No method with specified id.";
				return false;
			}

			var error = reader.ReadString() ?? "";
			object resultValue = null;
			//object[] resultCustomStructureValues = null;
			if( string.IsNullOrEmpty( error ) )
			{
				if( method.ReturnParameter != null )
				{
					//if( method.ReturnParameter.CustomTypeProperties != null )
					//	resultCustomStructureValues = reader.ReadCustomStructureProperties( method.ReturnParameter.CustomTypeProperties );
					//else
					//{
					if( method.ReturnParameter.Type != null )
						resultValue = reader.Read( method.ReturnParameter.Type, null );
					//}
				}
			}

			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new CallMethodAnswerItem();
				answerItem.ResultValue = resultValue;
				//answerItem.ResultCustomStructureValues = resultCustomStructureValues;
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		void SendCallMethod( long requestID, CloudMethodInfo method, object[] inputParameterValues )
		{
			var m = BeginMessage( callMethodMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.WriteVariable( method.Id );
			writer.WriteVariable( inputParameterValues.Length );
			var inputParameters = method.InputParameters;
			for( int n = 0; n < inputParameters.Length; n++ )
			{
				var inputParameter = inputParameters[ n ];
				var inputParameterValue = inputParameterValues[ n ];

				//if( inputParameter.CustomTypeProperties != null )
				//{
				//	if( inputParameter.Type != null )
				//		writer.WriteCustomStructure( inputParameter.Type, inputParameterValue );
				//	else
				//		writer.WriteCustomStructure( inputParameterValue.GetType(), inputParameterValue );
				//}
				//else
				writer.Write( inputParameter.Type, inputParameterValue );
			}
			m.End();
		}

		//with return value

		public async Task<CallMethodResult<T>> CallMethodAsync<T>( CloudMethodInfo method, CancellationToken cancellationToken, params object[] parameters )
		{
			var inputParameterValues = parameters ?? Array.Empty<object>();

			var requestID = GetRequestID();
			SendCallMethod( requestID, method, inputParameterValues );

			while( true )
			{
				var answer = GetAnswerAndRemove<CallMethodAnswerItem>( requestID );
				if( answer != null )
				{
					if( !string.IsNullOrEmpty( answer.Error ) )
						return new CallMethodResult<T> { Error = answer.Error };
					else
					{
						//if( answer.ResultCustomStructureValues != null )
						//{
						//	try
						//	{
						//		var returnParameter = method.ReturnParameter;

						//		var valueType = typeof( T );
						//		var value = (T)valueType.InvokeMember( "", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, null, null );

						//		if( returnParameter.CustomTypeProperties.Length != answer.ResultCustomStructureValues.Length )
						//			return new CallMethodResult<T> { Error = "Invalid answer. returnParameter.CustomTypeProperties.Length != answer.ResultCustomStructureValues.Length." };

						//		for( int n = 0; n < returnParameter.CustomTypeProperties.Length; n++ )
						//		{
						//			var p = returnParameter.CustomTypeProperties[ n ];
						//			var resultCustomStructureValue = answer.ResultCustomStructureValues[ n ];

						//			if( p.FieldType != null )
						//			{
						//				var field = valueType.GetField( p.Name );
						//				if( field == null )
						//					return new CallMethodResult<T> { Error = $"No field with name \"{p.Name}\"." };
						//				field.SetValue( value, resultCustomStructureValue );
						//			}
						//			else if( p.PropertyType != null )
						//			{
						//				var property = valueType.GetProperty( p.Name );
						//				if( property == null )
						//					return new CallMethodResult<T> { Error = $"No property with name \"{p.Name}\"." };
						//				property.SetValue( value, resultCustomStructureValue );
						//			}
						//		}

						//		return new CallMethodResult<T> { Value = value };
						//	}
						//	catch( Exception e )
						//	{
						//		return new CallMethodResult<T> { Error = e.Message };
						//	}
						//}
						//else
						return new CallMethodResult<T> { Value = (T)answer.ResultValue };
					}
				}

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
				{
					SendCancelRequest( requestID );
					return new CallMethodResult<T> { Error = "Operation was canceled." };
				}
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new CallMethodResult<T>() { Error = ConnectionErrorReceived };
			}
		}

		public async Task<CallMethodResult<T>> CallMethodAsync<T>( string className, string methodName, CancellationToken cancellationToken, params object[] parameters )
		{
			var method = await GetCloudMethodInfoAsync( className, methodName, cancellationToken );
			if( !string.IsNullOrEmpty( method.Error ) )
				return new CallMethodResult<T> { Error = method.Error };

			return await CallMethodAsync<T>( method.Method, cancellationToken, parameters );
		}

		///// <summary>
		///// Call method with default cancellation token specified in CallMethodDefaultCancellationTokenSource.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="method"></param>
		///// <param name="parameters"></param>
		///// <returns></returns>
		//public async Task<CallMethodResult<T>> CallMethodAsync<T>( CloudMethodInfo method, params object[] parameters )
		//{
		//	return await CallMethodWithCancellationTokenAsync<T>( method, default, parameters );
		//	//return await CallMethodWithCancellationTokenAsync<T>( method, CallMethodDefaultCancellationTokenSource.Token, parameters );
		//}

		///// <summary>
		///// Call method with default cancellation token specified in CallMethodDefaultCancellationTokenSource.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="className"></param>
		///// <param name="methodName"></param>
		///// <param name="parameters"></param>
		///// <returns></returns>
		//public async Task<CallMethodResult<T>> CallMethodAsync<T>( string className, string methodName, params object[] parameters )
		//{
		//	return await CallMethodWithCancellationTokenAsync<T>( className, methodName, default, parameters );
		//	//return await CallMethodWithCancellationTokenAsync<T>( className, methodName, CallMethodDefaultCancellationTokenSource.Token, parameters );
		//}

		//without return value

		public async Task<CallMethodResultNoValue> CallMethodAsync( CloudMethodInfo method, CancellationToken cancellationToken, params object[] parameters )
		{
			var inputParameterValues = parameters ?? Array.Empty<object>();

			var requestID = GetRequestID();
			SendCallMethod( requestID, method, inputParameterValues );

			while( true )
			{
				var answer = GetAnswerAndRemove<CallMethodAnswerItem>( requestID );
				if( answer != null )
				{
					if( !string.IsNullOrEmpty( answer.Error ) )
						return new CallMethodResultNoValue { Error = answer.Error };
					else
						return new CallMethodResultNoValue();
				}

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
				{
					SendCancelRequest( requestID );
					return new CallMethodResultNoValue { Error = "Operation was canceled." };
				}
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new CallMethodResultNoValue() { Error = ConnectionErrorReceived };
			}
		}

		public async Task<CallMethodResultNoValue> CallMethodAsync( string className, string methodName, CancellationToken cancellationToken, params object[] parameters )
		{
			var method = await GetCloudMethodInfoAsync( className, methodName, cancellationToken );
			if( !string.IsNullOrEmpty( method.Error ) )
				return new CallMethodResultNoValue { Error = method.Error };

			return await CallMethodAsync( method.Method, cancellationToken, parameters );
		}

		///// <summary>
		///// Call method with default cancellation token specified in CallMethodDefaultCancellationTokenSource.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="method"></param>
		///// <param name="parameters"></param>
		///// <returns></returns>
		//public async Task<CallMethodResultNoValue> CallMethodAsync( CloudMethodInfo method, params object[] parameters )
		//{
		//	return await CallMethodWithCancellationTokenAsync( method, default, parameters );
		//	//return await CallMethodWithCancellationTokenAsync( method, CallMethodDefaultCancellationTokenSource.Token, parameters );
		//}

		///// <summary>
		///// Call method with default cancellation token specified in CallMethodDefaultCancellationTokenSource.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="className"></param>
		///// <param name="methodName"></param>
		///// <param name="parameters"></param>
		///// <returns></returns>
		//public async Task<CallMethodResultNoValue> CallMethodAsync( string className, string methodName, params object[] parameters )
		//{
		//	return await CallMethodWithCancellationTokenAsync( className, methodName, default, parameters );
		//	//return await CallMethodWithCancellationTokenAsync( className, methodName, CallMethodDefaultCancellationTokenSource.Token, parameters );
		//}




		//public async Task<CallMethodResult<T>> CallMethodAsync<T>( string className, string methodName, object[] parameters, CancellationToken cancellationToken = default )
		//{

		//	//!!!!error when not supported

		//	//!!!!optimize
		//	//cache names. transfer IDs

		//	//!!!!more parameter types. arrays, tuples


		//	var parameters2 = parameters;
		//	if( parameters2 == null )
		//		parameters2 = Array.Empty<object>();

		//	var requestID = GetRequestID();

		//	var m = BeginMessage( callMethodMessage );
		//	var writer = m.Writer;
		//	writer.WriteVariable( requestID );
		//	writer.Write( className );
		//	writer.Write( methodName );
		//	writer.WriteVariableInt32( parameters2.Length );
		//	for( int n = 0; n < parameters2.Length; n++ )
		//	{
		//		var p = parameters2[ n ];
		//		writer.Write( p != null ? p.ToString() : null );
		//	}
		//	m.End();


		//	while( true )
		//	{
		//		var answer = GetAnswerAndRemove<CallMethodAnswerItem>( requestID );
		//		if( answer != null )
		//		{
		//			if( string.IsNullOrEmpty( answer.Error ) )
		//			{
		//				var value = answer.Value;
		//				if( value != null )
		//				{
		//					if( SimpleTypes.TryParseValue<T>( value, out var resultValue, out var error ) )
		//						return new CallMethodResult<T> { Value = resultValue };
		//					else
		//						return new CallMethodResult<T> { Error = error };
		//				}
		//				else
		//					return default;
		//			}
		//			else
		//				return new CallMethodResult<T> { Error = answer.Error };
		//		}

		//		await Task.Delay( 1 );
		//		if( cancellationToken.IsCancellationRequested )
		//		{
		//			SendCancelRequest( requestID );
		//			return new CallMethodResult<T> { Error = "Operation was canceled." };
		//		}
		//		if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
		//			return new CallMethodResult<T>() { Error = ConnectionErrorReceived };
		//	}
		//}

		///////////////////////////////////////////////

		class GetFilesInfoAnswerItem : AnswerItem
		{
			public GetFilesInfoResult.FileItem[] GetFilesInfo_Files;
			public GetDirectoryInfoResult.Item[] GetDirectoryInfo_Files;

			//

			public bool IsValidGetFilesInfo( string[] paths )
			{
				return ( GetFilesInfo_Files != null && GetFilesInfo_Files.Length == paths.Length ) || !string.IsNullOrEmpty( Error );
			}

			public bool IsValidGetDirectoryInfo()
			{
				return GetDirectoryInfo_Files != null || !string.IsNullOrEmpty( Error );
			}
		}

		public class GetFilesInfoResult
		{
			public FileItem[] Files { get; set; }
			public string Error { get; set; }

			public struct FileItem
			{
				/// <summary>
				/// -1 when file is not exist.
				/// </summary>
				public long Size { get; set; }
				public DateTime LastModifiedUtc { get; set; }
				public string Hash { get; set; }

				public bool Exists
				{
					get { return Size >= 0; }
				}
			}
		}

		public class GetFileInfoResult
		{
			public long Size { get; set; }
			public DateTime LastModifiedUtc { get; set; }
			public string Hash { get; set; }
			public string Error { get; set; }
		}

		public class GetDirectoryInfoResult
		{
			public Item[] Items { get; set; }
			public string Error { get; set; }

			public struct Item
			{
				public string Path { get; set; }
				//string path { get; set; }
				public long Size { get; set; }
				public DateTime LastModifiedUtc { get; set; }
				public string Hash { get; set; }
				public bool IsDirectory { get; set; }

				public string PathNormalized
				{
					get { return PathUtility.NormalizePath( Path ); }
				}
			}
		}

		public enum DataSource
		{
			Project,
			Storage,
		}

		//public enum DataSourceWithTransferWay
		//{
		//	Project,
		//	//StorageDirect,
		//	StorageThroughServer,
		//	//StorageP2P,
		//}

		public class DownloadFilesResult
		{
			public FileItem[] Files;
			public bool WasAlreadyDownloaded;
			public string Error;

			public struct FileItem
			{
				public long Size { get; set; }
				public DateTime LastModifiedUtc { get; set; }
				public string Hash { get; set; }
			}
		}

		public class DownloadDirectoryResult
		{
			public Item[] Items { get; set; }
			public bool WasAlreadyDownloaded;
			public string Error { get; set; }

			public struct Item
			{
				public string Path { get; set; }
				public long Size { get; set; }
				public DateTime LastModifiedUtc { get; set; }
				public string Hash { get; set; }
				public bool IsDirectory { get; set; }
				public string FullPath { get; set; }

				public string PathNormalized
				{
					get { return PathUtility.NormalizePath( Path ); }
				}
			}
		}

		bool ReceiveMessage_GetFilesInfoAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var itemCount = reader.ReadVariableInt32();
			GetFilesInfoResult.FileItem[] files = null;
			if( itemCount > 0 )
			{
				files = new GetFilesInfoResult.FileItem[ itemCount ];
				for( int n = 0; n < itemCount; n++ )
				{
					var item = new GetFilesInfoResult.FileItem();
					item.Size = reader.ReadInt64();
					item.Hash = reader.ReadString() ?? "";
					item.LastModifiedUtc = reader.ReadDateTime();
					files[ n ] = item;
				}
			}
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new GetFilesInfoAnswerItem();
				answerItem.GetFilesInfo_Files = files;
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		bool ReceiveMessage_GetDirectoryInfoAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var itemCount = reader.ReadVariableInt32();
			GetDirectoryInfoResult.Item[] files = null;
			if( itemCount >= 0 )
			{
				files = new GetDirectoryInfoResult.Item[ itemCount ];
				for( int n = 0; n < itemCount; n++ )
				{
					var item = new GetDirectoryInfoResult.Item();
					item.Path = reader.ReadString() ?? ""; //item.Path = PathUtility.NormalizePath( reader.ReadString() );
					item.Size = reader.ReadVariableInt64();
					item.Hash = reader.ReadString() ?? "";
					item.LastModifiedUtc = reader.ReadDateTime();
					item.IsDirectory = reader.ReadBoolean();
					files[ n ] = item;
				}
			}
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new GetFilesInfoAnswerItem();
				answerItem.GetDirectoryInfo_Files = files;
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		void SendGetFilesInfo( long requestID, DataSource source, string[] paths, string anyData )
		{
			var m = BeginMessage( getFilesInfoMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.Write( source == DataSource.Storage );
			writer.WriteVariableInt32( paths.Length );
			for( int n = 0; n < paths.Length; n++ )
				writer.Write( paths[ n ] );
			writer.Write( anyData );
			m.End();
		}

		public async Task<GetFilesInfoResult> GetFilesInfoAsync( DataSource source, string[] filePaths, string anyData = null, CancellationToken cancellationToken = default )
		{
			var requestID = GetRequestID();
			SendGetFilesInfo( requestID, source, filePaths, anyData );

			while( true )
			{
				var answer = GetAnswerAndRemove<GetFilesInfoAnswerItem>( requestID );
				if( answer != null )
				{
					if( answer.IsValidGetFilesInfo( filePaths ) )
						return new GetFilesInfoResult { Files = answer.GetFilesInfo_Files, Error = answer.Error };
					else
						return new GetFilesInfoResult { Error = "Answer is not valid." };
				}

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
				{
					SendCancelRequest( requestID );
					return new GetFilesInfoResult { Error = "Operation was canceled." };
				}
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new GetFilesInfoResult() { Error = ConnectionErrorReceived };
			}
		}

		public async Task<GetFileInfoResult> GetFileInfoAsync( DataSource source, string filePath, string anyData = null, CancellationToken cancellationToken = default )
		{
			var result = await GetFilesInfoAsync( source, new string[] { filePath }, anyData, cancellationToken );
			if( string.IsNullOrEmpty( result.Error ) )
			{
				var file = result.Files[ 0 ];
				return new GetFileInfoResult { Size = file.Size, Hash = file.Hash };
			}
			else
				return new GetFileInfoResult { Error = result.Error };
		}

		void SendGetDirectoryInfo( long requestID, DataSource source, string sourcePath, string searchPattern, SearchOption searchOption, string anyData )
		{
			var m = BeginMessage( getDirectoryInfoMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.Write( source == DataSource.Storage );
			writer.Write( sourcePath );
			writer.Write( searchPattern );
			writer.Write( searchOption == SearchOption.AllDirectories );
			writer.Write( anyData );
			m.End();
		}

		public async Task<GetDirectoryInfoResult> GetDirectoryInfoAsync( DataSource source, string directoryPath, string searchPattern, SearchOption searchOption, string anyData = null, CancellationToken cancellationToken = default )
		{
			var requestID = GetRequestID();
			SendGetDirectoryInfo( requestID, source, directoryPath, searchPattern, searchOption, anyData );

			while( true )
			{
				var answer = GetAnswerAndRemove<GetFilesInfoAnswerItem>( requestID );
				if( answer != null )
				{
					if( answer.IsValidGetDirectoryInfo() )
						return new GetDirectoryInfoResult { Items = answer.GetDirectoryInfo_Files, Error = answer.Error };
					else
						return new GetDirectoryInfoResult { Error = "Answer is not valid." };
				}

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
				{
					SendCancelRequest( requestID );
					return new GetDirectoryInfoResult { Error = "Operation was canceled." };
				}
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new GetDirectoryInfoResult() { Error = ConnectionErrorReceived };
			}
		}

		///////////////////////////////////////////////

		class DownloadFileContentAnswerItem : AnswerItem
		{
			public Part[] Parts;

			public struct Part
			{
				public byte[] Data;
			}
		}

		bool ReceiveMessage_DownloadFileContentAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var maxBlockSizeLimit = DownloadFilesMaxBlockSize; // = 1 * 1024 * 1024;

			const int maxPartCountInGroup = 10000;


			var requestID = reader.ReadVariableInt64();
			var partCount = reader.ReadVariableInt32();
			if( partCount > maxPartCountInGroup )
			{
				additionalErrorMessage = $"Part count limit is {maxPartCountInGroup}.";
				return false;
			}

			var parts = new DownloadFileContentAnswerItem.Part[ partCount ];
			for( int n = 0; n < partCount; n++ )
			{
				var dataSize = reader.ReadVariableInt32();
				if( dataSize > maxBlockSizeLimit )
				{
					additionalErrorMessage = $"dataSize > maxBlockSizeLimit. {maxBlockSizeLimit}";
					return false;
				}
				var data = new byte[ dataSize ];
				reader.ReadBuffer( data, 0, data.Length );
				parts[ n ] = new DownloadFileContentAnswerItem.Part() { Data = data };
			}
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new DownloadFileContentAnswerItem();
				answerItem.Parts = parts;
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		struct DownloadPart
		{
			public string FileName;
			public long PartStart;
			public long PartEnd;
			public string TargetFullPath;

			public int Size
			{
				get { return (int)( PartEnd - PartStart ); }
			}
		}

		class DownloadPartGroup
		{
			public List<DownloadPart> Parts = new List<DownloadPart>();
			public int Size;
			public long RequestID;
		}

		void SendDownloadFileContent( long requestID, DownloadPart[] parts, string anyData )
		{
			var m = BeginMessage( downloadFileContentMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.WriteVariable( parts.Length );
			for( int n = 0; n < parts.Length; n++ )
			{
				var item = parts[ n ];
				writer.Write( item.FileName );
				writer.WriteVariable( item.PartStart );
				writer.WriteVariable( item.PartEnd );
			}
			//writer.WriteVariable( maxBytesPerSecond );
			writer.Write( anyData );
			m.End();
		}

		class StorageDownloadFilesAnswerItem : AnswerItem
		{
			public string[] DownloadUrls;
		}

		void SendStorageDownloadFiles( long requestID, string[] sourceFilePaths, string anyData )
		{
			var m = BeginMessage( storageDownloadFilesMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.WriteVariableInt32( sourceFilePaths.Length );
			for( int n = 0; n < sourceFilePaths.Length; n++ )
				writer.Write( sourceFilePaths[ n ] );
			writer.Write( anyData );
			m.End();
		}

		bool ReceiveMessage_StorageDownloadFilesAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var itemCount = reader.ReadVariableInt32();
			var downloadUrls = new string[ itemCount ];
			for( int n = 0; n < itemCount; n++ )
				downloadUrls[ n ] = reader.ReadString();
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new StorageDownloadFilesAnswerItem();
				answerItem.DownloadUrls = downloadUrls;
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		public delegate void DownloadFilesProgressCallback( long totalSize, int addedBytes, long totalDownloadedSize, int percentage );

		public async Task<DownloadFilesResult> DownloadFilesAsync( DataSource source, string[] sourceFilePaths, string[] targetFullPaths, bool skipDownloadIfUpToDate, string anyData = null, DownloadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			//get files info
			var getFilesResult = await GetFilesInfoAsync( source, sourceFilePaths, anyData, cancellationToken );
			if( !string.IsNullOrEmpty( getFilesResult.Error ) )
				return new DownloadFilesResult { Error = getFilesResult.Error };

			//check for file exists
			for( int n = 0; n < getFilesResult.Files.Length; n++ )
			{
				var fileItem = getFilesResult.Files[ n ];
				if( !fileItem.Exists )
					return new DownloadFilesResult { Error = $"File \"{sourceFilePaths[ n ]}\" is not exists." };
			}

			var totalSize = 0L;
			for( int n = 0; n < getFilesResult.Files.Length; n++ )
				totalSize += getFilesResult.Files[ n ].Size;

			//prepare result files info
			var resultFiles = new DownloadFilesResult.FileItem[ getFilesResult.Files.Length ];
			for( int n = 0; n < getFilesResult.Files.Length; n++ )
			{
				var fileItem = getFilesResult.Files[ n ];
				resultFiles[ n ] = new DownloadFilesResult.FileItem
				{
					Size = fileItem.Size,
					LastModifiedUtc = fileItem.LastModifiedUtc,
					Hash = fileItem.Hash,
				};
			}

			//compare
			bool[] toDownload = new bool[ sourceFilePaths.Length ];
			for( int n = 0; n < getFilesResult.Files.Length; n++ )
			{
				toDownload[ n ] = true;

				if( skipDownloadIfUpToDate )
				{
					string targetFullPath = targetFullPaths[ n ];
					var storageFileInfo = getFilesResult.Files[ n ];

					try
					{
						//!!!!hashes

						var fileInfo = new FileInfo( targetFullPath );
						if( fileInfo.Exists && fileInfo.Length == storageFileInfo.Size && fileInfo.LastWriteTimeUtc >= storageFileInfo.LastModifiedUtc )
							toDownload[ n ] = false;
					}
					catch { }
				}
			}

			//all files already downloaded
			if( toDownload.All( i => !i ) )
				return new DownloadFilesResult { Files = resultFiles, WasAlreadyDownloaded = true };


			if( source == DataSource.Storage )
			{
				var sourceFilePaths2 = new List<string>();
				var targetFullPaths2 = new List<string>();
				var getFileResult2 = new List<GetFilesInfoResult.FileItem>();
				for( int n = 0; n < toDownload.Length; n++ )
				{
					if( toDownload[ n ] )
					{
						sourceFilePaths2.Add( sourceFilePaths[ n ] );
						targetFullPaths2.Add( targetFullPaths[ n ] );
						getFileResult2.Add( getFilesResult.Files[ n ] );
					}
				}

				//send request to get download urls
				var requestID = GetRequestID();
				SendStorageDownloadFiles( requestID, sourceFilePaths2.ToArray(), anyData );

				while( true )
				{
					var answer = GetAnswerAndRemove<StorageDownloadFilesAnswerItem>( requestID );
					if( answer != null )
					{
						if( !string.IsNullOrEmpty( answer.Error ) )
							return new DownloadFilesResult { Error = answer.Error };
						if( answer.DownloadUrls.Length != sourceFilePaths2.Count )
							return new DownloadFilesResult { Error = "Invalid answer." };

						try
						{
							//start downloading by urls

							progressCallback?.Invoke( totalSize, 0, 0, 0 );
							var callbackProgressLastTotalProcessedSize = 0L;
							var callbackProgressLastPercentage = 0;

							var totalDownloadedSize = 0L;

							var totalSizeToDownload = 0L;
							for( int n = 0; n < sourceFilePaths2.Count; n++ )
								totalSizeToDownload += getFileResult2[ n ].Size;

							for( int n = 0; n < sourceFilePaths2.Count; n++ )
							{
								var sourceFilePath = sourceFilePaths2[ n ];
								var targetFullPath = targetFullPaths2[ n ];
								var getFileResult = getFileResult2[ n ];
								var downloadUrl = answer.DownloadUrls[ n ];

								void Progress( int downloadedIncrement, long totalDownloaded2, long totalSize2 )
								{
									totalDownloadedSize += downloadedIncrement;

									//downloading callback
									var percentage = (int)MathEx.Clamp( (double)totalDownloadedSize / Math.Max( totalSizeToDownload, 1 ) * 100, 0, 100 );
									//if( callbackProgressLastTotalDownloadedSize != totalDownloadedSize || callbackProgressLastPercentage != percentage )
									{
										progressCallback?.Invoke( totalSize, downloadedIncrement, totalDownloadedSize, percentage );
										callbackProgressLastTotalProcessedSize = totalDownloadedSize;
										callbackProgressLastPercentage = percentage;
									}
								}

								var downloadResult = await NetworkUtility.DownloadFileByUrlAsync( downloadUrl, targetFullPath, Progress, cancellationToken );
								if( !string.IsNullOrEmpty( downloadResult.Error ) )
									return new DownloadFilesResult() { Error = downloadResult.Error };
							}

							//compare file content? or hash

							if( totalSizeToDownload != totalDownloadedSize )
								return new DownloadFilesResult { Error = "Invalid total downloaded size." };

							{
								//downloading callback
								var percentage = (int)MathEx.Clamp( (double)totalDownloadedSize / Math.Max( totalSizeToDownload, 1 ) * 100, 0, 100 );
								if( callbackProgressLastTotalProcessedSize != totalDownloadedSize || callbackProgressLastPercentage != percentage )
								{
									progressCallback?.Invoke( totalSize, 0, totalDownloadedSize, percentage );
									callbackProgressLastTotalProcessedSize = totalDownloadedSize;
									callbackProgressLastPercentage = percentage;
								}
							}

							return new DownloadFilesResult { Files = resultFiles };
						}
						catch( Exception e )
						{
							return new DownloadFilesResult { Error = e.Message };
						}
					}

					await Task.Delay( 1 );
					if( cancellationToken.IsCancellationRequested )
					{
						SendCancelRequest( requestID );
						return new DownloadFilesResult { Error = "Operation was canceled." };
					}
					if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
						return new DownloadFilesResult() { Error = ConnectionErrorReceived };
				}
			}
			else
			{
				var maxBlockSize = DownloadFilesMaxBlockSize;
				{
					//get max block size from the server

					var serverCommonInfo = await GetServerCommonInfoAsync( cancellationToken );
					if( !string.IsNullOrEmpty( serverCommonInfo.Error ) )
						return new DownloadFilesResult { Error = serverCommonInfo.Error };

					//!!!!need 0.95?
					if( ServerCommonInfo.ReceiveMessageDownloadFilesContentMaxSize != 0 )
						maxBlockSize = Math.Min( maxBlockSize, (int)( ServerCommonInfo.ReceiveMessageDownloadFilesContentMaxSize * 0.95f ) );
					else
						maxBlockSize = Math.Min( maxBlockSize, 32768 * 16 );
				}
				//var maxQueueSize = 10 * 1024 * 1024;
				//var maxQueueSize = DownloadFilesMaxQueueSize;

				const int maxPartCountInGroup = 10000;

				var downloadParts = new List<DownloadPart>( toDownload.Length );

				for( int n = 0; n < toDownload.Length; n++ )
				{
					if( toDownload[ n ] )
					{
						var storageFileName = sourceFilePaths[ n ];
						var fullPath = targetFullPaths[ n ];
						var storageFileInfo = getFilesResult.Files[ n ];
						var size = storageFileInfo.Size;

						if( size > 0 )
						{
							//not zero size

							for( int from = 0; from < size; from += maxBlockSize )
							{
								long to = from + maxBlockSize;
								if( to > size )
									to = size;

								if( from != to )
								{
									var part = new DownloadPart();
									part.FileName = storageFileName;
									part.PartStart = from;
									part.PartEnd = to;
									part.TargetFullPath = fullPath;
									downloadParts.Add( part );
								}
							}
						}
						else
						{
							//zero size

							try
							{
								var directory = Path.GetDirectoryName( fullPath );
								if( !Directory.Exists( directory ) )
									Directory.CreateDirectory( directory );
								if( File.Exists( fullPath ) )
									File.Delete( fullPath );
								File.WriteAllBytes( fullPath, new byte[ 0 ] );
							}
							catch( Exception e )
							{
								return new DownloadFilesResult { Error = e.Message };
							}
						}
					}
				}

				var downloadGroups = new List<DownloadPartGroup>( downloadParts.Count );
				{
					var currentGroup = new DownloadPartGroup();

					for( int nPart = 0; nPart < downloadParts.Count; nPart++ )
					{
						var part = downloadParts[ nPart ];

						if( currentGroup.Size + part.Size > maxBlockSize || currentGroup.Parts.Count >= maxPartCountInGroup )
						{
							downloadGroups.Add( currentGroup );
							currentGroup = new DownloadPartGroup();
						}
						currentGroup.Parts.Add( part );
						currentGroup.Size += part.Size;
					}

					if( currentGroup.Parts.Count != 0 )
						downloadGroups.Add( currentGroup );
				}

				var downloadGroupsDataSize = 0L;
				foreach( var group in downloadGroups )
					downloadGroupsDataSize += group.Size;

				progressCallback?.Invoke( totalSize, 0, 0, 0 );
				var lastSentTotalDownloadedSize = 0L;
				var lastSentPercentage = 0;

				var totalDownloadedSize = 0L;

				foreach( var group in downloadGroups )
				{
					var requestID = GetRequestID();
					group.RequestID = requestID;
					var requestParts = group.Parts.ToArray();
					SendDownloadFileContent( requestID, requestParts, anyData );

					while( true )
					{
						var answer = GetAnswerAndRemove<DownloadFileContentAnswerItem>( requestID );
						if( answer != null )
						{
							if( !string.IsNullOrEmpty( answer.Error ) )
								return new DownloadFilesResult { Error = answer.Error };
							if( answer.Parts.Length != requestParts.Length )
								return new DownloadFilesResult { Error = "Invalid answer 1." };

							try
							{
								var addedSize = 0;

								for( int n = 0; n < requestParts.Length; n++ )
								{
									var requestPart = requestParts[ n ];
									var answerPart = answer.Parts[ n ];
									var fullPath = requestPart.TargetFullPath;

									if( ( requestPart.PartEnd - requestPart.PartStart ) != answerPart.Data.Length )
										return new DownloadFilesResult { Error = "Invalid answer 2." };

									if( requestPart.PartStart == 0 )
									{
										var directory = Path.GetDirectoryName( fullPath );
										if( !Directory.Exists( directory ) )
											Directory.CreateDirectory( directory );
										if( File.Exists( fullPath ) )
											File.Delete( fullPath );
										File.WriteAllBytes( fullPath, answerPart.Data );
									}
									else
									{
										using( var stream = new FileStream( fullPath, FileMode.Append ) )
											stream.Write( answerPart.Data, 0, answerPart.Data.Length );
									}

									totalDownloadedSize += requestPart.Size;
									addedSize += requestPart.Size;
								}

								//percentage callback
								var percentage = (int)MathEx.Clamp( (double)totalDownloadedSize / Math.Max( downloadGroupsDataSize, 1 ) * 100, 0, 100 );
								//if( callbackProgressLastTotalDownloadedSize != totalDownloadedSize || callbackProgressLastPercentage != percentage )
								{
									progressCallback?.Invoke( totalSize, addedSize, totalDownloadedSize, percentage );
									lastSentTotalDownloadedSize = totalDownloadedSize;
									lastSentPercentage = percentage;
								}

								//go to next group
								break;
							}
							catch( Exception e )
							{
								return new DownloadFilesResult { Error = e.Message };
							}
						}

						await Task.Delay( 1 );
						if( cancellationToken.IsCancellationRequested )
						{
							SendCancelRequest( requestID );
							return new DownloadFilesResult { Error = "Operation was canceled." };
						}
						if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
							return new DownloadFilesResult() { Error = ConnectionErrorReceived };
					}
				}

				return new DownloadFilesResult { Files = resultFiles };
			}
		}

		public async Task<DownloadFilesResult> DownloadFileAsync( DataSource source, string sourceFilePath, string targetFullPath, bool skipDownloadIfUpToDate, string anyData = null, DownloadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await DownloadFilesAsync( source, new string[] { sourceFilePath }, new string[] { targetFullPath }, skipDownloadIfUpToDate, anyData, progressCallback, cancellationToken );
		}

		public async Task<DownloadDirectoryResult> DownloadDirectoryAsync( DataSource source, string sourceDirectoryPath, string targetFullPath, string searchPattern, SearchOption searchOption, bool skipDownloadIfUpToDate, bool deleteExcessEntries, string anyData = null, DownloadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			var getDirectoryInfoResult = await GetDirectoryInfoAsync( source, sourceDirectoryPath, searchPattern, searchOption, anyData, cancellationToken );
			if( !string.IsNullOrEmpty( getDirectoryInfoResult.Error ) )
				return new DownloadDirectoryResult() { Error = getDirectoryInfoResult.Error };

			try
			{
				var filesResult = getDirectoryInfoResult.Items;

				var sourceDirectoryPath2 = PathUtility.NormalizePath( sourceDirectoryPath );
				if( sourceDirectoryPath2.Length != 0 && sourceDirectoryPath[ sourceDirectoryPath.Length - 1 ] != Path.DirectorySeparatorChar )
					sourceDirectoryPath2 += Path.DirectorySeparatorChar;

				var targetFullPaths = new string[ filesResult.Length ];
				for( int n = 0; n < filesResult.Length; n++ )
				{
					var serverFileInfo = filesResult[ n ];

					if( serverFileInfo.PathNormalized.IndexOf( sourceDirectoryPath2, StringComparison.OrdinalIgnoreCase ) != 0 )
						return new DownloadDirectoryResult() { Error = "Invalid result 1." };

					var localPath = serverFileInfo.PathNormalized.Substring( sourceDirectoryPath2.Length );
					if( string.IsNullOrEmpty( localPath ) )
						return new DownloadDirectoryResult() { Error = "Invalid result 2." };

					var targetFullPath2 = Path.Combine( targetFullPath, localPath );
					targetFullPaths[ n ] = targetFullPath2;
				}

				//compare
				bool[] toDownload = new bool[ filesResult.Length ];
				for( int n = 0; n < filesResult.Length; n++ )
				{
					var serverFileInfo = filesResult[ n ];
					var targetFullPath2 = targetFullPaths[ n ];

					if( !serverFileInfo.IsDirectory )
					{
						toDownload[ n ] = true;

						if( skipDownloadIfUpToDate )
						{
							try
							{
								//!!!!optional compare hashes. where save them, internal folder?

								//also can check hashes
								var fileInfo = new FileInfo( targetFullPath2 );
								if( fileInfo.Exists && fileInfo.Length == serverFileInfo.Size && fileInfo.LastWriteTimeUtc >= serverFileInfo.LastModifiedUtc )
									toDownload[ n ] = false;
							}
							catch { }
						}
					}
					else
						Directory.CreateDirectory( targetFullPath2 );
				}

				//download
				var wasAlreadyDownloaded = true;
				if( toDownload.Any( i => i ) )
				{
					wasAlreadyDownloaded = false;

					var sourceFilePaths2 = new List<string>();
					var targetFullPaths2 = new List<string>();

					for( int n = 0; n < filesResult.Length; n++ )
					{
						if( toDownload[ n ] )
						{
							var fileResult = filesResult[ n ];
							var targetFullPath2 = targetFullPaths[ n ];

							sourceFilePaths2.Add( fileResult.Path );
							targetFullPaths2.Add( targetFullPath2 );
						}
					}

					var downloadFilesResult = await DownloadFilesAsync( source, sourceFilePaths2.ToArray(), targetFullPaths2.ToArray(), false, anyData, progressCallback, cancellationToken );
					if( !string.IsNullOrEmpty( downloadFilesResult.Error ) )
						return new DownloadDirectoryResult { Error = downloadFilesResult.Error };
				}

				if( deleteExcessEntries )
				{
					var needFiles = new ESet<string>();
					foreach( var p in targetFullPaths )
						needFiles.AddWithCheckAlreadyContained( p.ToLower() );

					var directoryInfo = new DirectoryInfo( targetFullPath );
					if( directoryInfo.Exists )
					{
						//delete files
						foreach( var fileInfo in directoryInfo.GetFiles( "*.*", searchOption ) )
						{
							if( !needFiles.Contains( fileInfo.FullName.ToLower() ) )
							{
								try
								{
									if( File.Exists( fileInfo.FullName ) )
										File.Delete( fileInfo.FullName );
								}
								catch { }
							}
						}

						//delete empty directories
						IOUtility.DeleteEmptyDirectories( targetFullPath, searchOption, true );
					}
				}

				//get result
				{
					var result = new DownloadDirectoryResult();

					result.Items = new DownloadDirectoryResult.Item[ filesResult.Length ];
					for( int n = 0; n < filesResult.Length; n++ )
					{
						var fileResult = filesResult[ n ];

						var fileItem = new DownloadDirectoryResult.Item();
						fileItem.Path = fileResult.Path;
						fileItem.Size = fileResult.Size;
						fileItem.LastModifiedUtc = fileResult.LastModifiedUtc;
						fileItem.Hash = fileResult.Hash;
						fileItem.IsDirectory = fileResult.IsDirectory;
						fileItem.FullPath = targetFullPaths[ n ];
						result.Items[ n ] = fileItem;
					}

					result.WasAlreadyDownloaded = wasAlreadyDownloaded;
					return result;
				}
			}
			catch( Exception e )
			{
				return new DownloadDirectoryResult { Error = e.Message };
			}
		}

		public struct DownloadObjectsItem
		{
			public string Path;
			public bool IsDirectory;

			public DownloadObjectsItem( string path, bool isDirectory )
			{
				Path = path;
				IsDirectory = isDirectory;
			}
		}

		public async Task<DownloadDirectoryResult> DownloadObjectsAsync( DataSource source, DownloadObjectsItem[] objects, string[] targetFullPaths, bool skipDownloadIfUpToDate, bool deleteExcessEntries, string anyData = null, DownloadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var totalSize = 0L;
				if( progressCallback != null )
				{
					//directories
					for( int n = 0; n < objects.Length; n++ )
					{
						var obj = objects[ n ];

						if( obj.IsDirectory )
						{
							var result2 = await GetDirectoryInfoAsync( source, obj.Path, "*", SearchOption.AllDirectories, anyData, cancellationToken );
							if( !string.IsNullOrEmpty( result2.Error ) )
								return new DownloadDirectoryResult { Error = result2.Error };
							foreach( var file in result2.Items )
								totalSize += file.Size;
						}
					}

					//files

					var objects2 = new List<string>();
					var targetFullPaths2 = new List<string>();

					for( int n = 0; n < objects.Length; n++ )
					{
						var obj = objects[ n ];
						if( !obj.IsDirectory )
						{
							objects2.Add( obj.Path );
							targetFullPaths2.Add( targetFullPaths[ n ] );
						}
					}

					if( objects2.Count != 0 )
					{
						var result2 = await GetFilesInfoAsync( source, objects2.ToArray(), anyData, cancellationToken );
						if( !string.IsNullOrEmpty( result2.Error ) )
							return new DownloadDirectoryResult { Error = result2.Error };
						foreach( var file in result2.Files )
							totalSize += file.Size;
					}
				}

				var lastSendTotalDownloadedSize = 0L;
				var lastSentPercentage = 0;

				var totalDownloadedSize = 0L;

				void Progress( long totalSize2, int addedBytes, long totaldownloadedSize2, int percentage2 )
				{
					totalDownloadedSize += addedBytes;

					var percentage = (int)MathEx.Clamp( (double)totalDownloadedSize / Math.Max( totalSize, 1 ) * 100, 0, 100 );
					//if( lastSendTotalDownloadedSize != totalDownloadedSize || percentage != lastSentPercentage )
					{
						progressCallback?.Invoke( totalSize, addedBytes, totalDownloadedSize, percentage );
						lastSendTotalDownloadedSize = totalDownloadedSize;
						lastSentPercentage = percentage;
					}
				}


				var resultObjects = new List<DownloadDirectoryResult.Item>();
				var resultWasAlreadyDownloaded = true;


				//download directories
				for( int n = 0; n < objects.Length; n++ )
				{
					var obj = objects[ n ];
					var targetFullPath = targetFullPaths[ n ];

					if( obj.IsDirectory )
					{
						var result = await DownloadDirectoryAsync( source, obj.Path, targetFullPath, "*", SearchOption.AllDirectories, skipDownloadIfUpToDate, deleteExcessEntries, anyData, Progress, cancellationToken );
						if( !string.IsNullOrEmpty( result.Error ) )
							return new DownloadDirectoryResult { Error = result.Error };

						resultObjects.AddRange( result.Items );
						if( !result.WasAlreadyDownloaded )
							resultWasAlreadyDownloaded = false;
					}
				}

				//download files
				{
					var objects2 = new List<string>();
					var targetFullPaths2 = new List<string>();
					for( int n = 0; n < objects.Length; n++ )
					{
						var obj = objects[ n ];
						var targetFullPath = targetFullPaths[ n ];
						if( !obj.IsDirectory )
						{
							objects2.Add( obj.Path );
							targetFullPaths2.Add( targetFullPath );
						}
					}

					if( objects2.Count != 0 )
					{
						var result = await DownloadFilesAsync( source, objects2.ToArray(), targetFullPaths2.ToArray(), skipDownloadIfUpToDate, anyData, Progress, cancellationToken );
						if( !string.IsNullOrEmpty( result.Error ) )
							return new DownloadDirectoryResult { Error = result.Error };

						for( int n = 0; n < objects2.Count; n++ )
						{
							var resultItem = result.Files[ n ];
							var targetFullPath = targetFullPaths2[ n ];

							var fileItem = new DownloadDirectoryResult.Item();
							fileItem.Path = objects2[ n ];
							fileItem.Size = resultItem.Size;
							fileItem.LastModifiedUtc = resultItem.LastModifiedUtc;
							fileItem.Hash = resultItem.Hash;
							fileItem.FullPath = targetFullPath;

							resultObjects.Add( fileItem );
						}

						if( !result.WasAlreadyDownloaded )
							resultWasAlreadyDownloaded = false;
					}
				}

				return new DownloadDirectoryResult { Items = resultObjects.ToArray(), WasAlreadyDownloaded = resultWasAlreadyDownloaded };
			}
			catch( Exception e )
			{
				return new DownloadDirectoryResult { Error = e.Message };
			}
		}

		///////////////////////////////////////////////

		struct UploadPart
		{
			public string SourceFullPath;
			public string TargetFileName;
			public long PartStart;
			public long PartEnd;

			public int Size
			{
				get { return (int)( PartEnd - PartStart ); }
			}
		}

		class UploadPartGroup
		{
			public List<UploadPart> Parts = new List<UploadPart>();
			public int Size;
			public long RequestID;
		}

		class UploadFileContentAnswerItem : AnswerItem
		{
		}

		bool ReceiveMessage_UploadFileContentAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new UploadFileContentAnswerItem();
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		bool SendUploadFileContent( long requestID, UploadPartGroup group, string anyData, out string error )
		{
			error = null;

			byte[] tempData = null;

			var parts = group.Parts;

			var writer = new ArrayDataWriter( 1024 + group.Size );
			//var m = BeginMessage( uploadFileContentMessage );
			//var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.WriteVariable( parts.Count );
			for( int n = 0; n < parts.Count; n++ )
			{
				var part = parts[ n ];
				var partSize = part.Size;

				if( tempData == null || tempData.Length < partSize )
					tempData = new byte[ partSize ];
				var data = tempData;// new byte[ partSize ];
				{
					var fileInfo = new FileInfo( part.SourceFullPath );
					if( fileInfo.Exists )
					{
						if( partSize > 0 )
						{
							using( FileStream fs = new FileStream( part.SourceFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
							{
								fs.Seek( part.PartStart, SeekOrigin.Begin );
								if( fs.Read( data, 0, partSize ) != partSize )
								{
									error = "File reading failed.";
									return false;
								}
							}
						}
					}
					else
					{
						error = "File not found.";
						return false;
					}
				}

				writer.Write( part.TargetFileName );
				writer.WriteVariable( part.PartStart );
				writer.WriteVariable( part.PartEnd );
				writer.Write( data, 0, partSize );
			}
			//writer.WriteVariable( maxBytesPerSecond );
			writer.Write( anyData );
			SendMessage( uploadFileContentMessage, writer.AsArraySegment() );
			//m.End();

			return true;
		}

		class StorageUploadFilesAnswerItem : AnswerItem
		{
			public string[] UploadUrls;
		}

		void SendStorageUploadFiles( long requestID, string[] targetFilePaths, string anyData )
		{
			var m = BeginMessage( storageUploadFilesMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.WriteVariableInt32( targetFilePaths.Length );
			for( int n = 0; n < targetFilePaths.Length; n++ )
				writer.Write( targetFilePaths[ n ] );
			writer.Write( anyData );
			m.End();
		}

		bool ReceiveMessage_StorageUploadFilesAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var itemCount = reader.ReadVariableInt32();
			var uploadUrls = new string[ itemCount ];
			for( int n = 0; n < itemCount; n++ )
				uploadUrls[ n ] = reader.ReadString();
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new StorageUploadFilesAnswerItem();
				answerItem.UploadUrls = uploadUrls;
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		class StreamWithReadCounter : Stream
		{
			readonly FileStream _fileStream;
			readonly Action<int> _callback;
			bool _disposed;

			public StreamWithReadCounter( string filePath, Action<int> callback )
			{
				if( string.IsNullOrEmpty( filePath ) )
					throw new ArgumentNullException( nameof( filePath ) );

				_fileStream = new FileStream( filePath, FileMode.Open, FileAccess.Read );
				_callback = callback ?? throw new ArgumentNullException( nameof( callback ) );
			}

			public override int Read( byte[] buffer, int offset, int count )
			{
				int bytesRead = _fileStream.Read( buffer, offset, count );
				if( bytesRead > 0 )
					_callback( bytesRead );
				return bytesRead;
			}

			public override async Task<int> ReadAsync( byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken )
			{
				int bytesRead = await _fileStream.ReadAsync( buffer, offset, count, cancellationToken );
				if( bytesRead > 0 )
					_callback( bytesRead );
				return bytesRead;
			}

			public override long Seek( long offset, SeekOrigin origin )
			{
				return _fileStream.Seek( offset, origin );
			}

			public override void SetLength( long value )
			{
				throw new NotSupportedException( "Setting length is not supported." );
			}

			public override void Flush()
			{
				_fileStream.Flush();
			}

			public override long Length => _fileStream.Length;

			public override long Position
			{
				get => _fileStream.Position;
				set => _fileStream.Position = value;
			}

			protected override void Dispose( bool disposing )
			{
				if( !_disposed )
				{
					if( disposing )
					{
						_fileStream.Dispose();
					}
					_disposed = true;
				}
				base.Dispose( disposing );
			}

			public override void Write( byte[] buffer, int offset, int count )
			{
				throw new NotImplementedException();
			}

			public override bool CanRead => _fileStream.CanRead;
			public override bool CanSeek => _fileStream.CanSeek;
			public override bool CanWrite => false;
		}

		public delegate void UploadFilesProgressCallback( long totalSize, int addedBytes, long totalUploadedSize, int percentage );
		//public delegate void UploadFilesProgressCallback( int percentage );

		public async Task<SimpleResult> UploadFilesAsync( DataSource source, string[] sourceFullPaths, string[] targetFilePaths, string anyData = null, UploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			if( targetFilePaths.Length != sourceFullPaths.Length )
				return new SimpleResult() { Error = "targetFilePaths.Length != sourceFullPaths.Length." };

			if( source == DataSource.Storage )
			{
				//send request to get upload urls
				var requestID = GetRequestID();
				SendStorageUploadFiles( requestID, targetFilePaths, anyData );

				while( true )
				{
					var answer = GetAnswerAndRemove<StorageUploadFilesAnswerItem>( requestID );
					if( answer != null )
					{
						if( !string.IsNullOrEmpty( answer.Error ) )
							return new SimpleResult { Error = answer.Error };
						if( answer.UploadUrls.Length != sourceFullPaths.Length )
							return new SimpleResult { Error = "Invalid answer." };

						try
						{
							//start uploading by urls

							var totalUploadedSize = 0L;

							var sizes = new long[ targetFilePaths.Length ];
							var totalSizeToUpload = 0L;
							for( int n = 0; n < targetFilePaths.Length; n++ )
							{
								var sourceFullPath = sourceFullPaths[ n ];

								var fileInfo = new FileInfo( sourceFullPath );
								if( !fileInfo.Exists )
									return new SimpleResult { Error = $"File \"{sourceFullPath}\" not exists." };

								sizes[ n ] = fileInfo.Length;
								totalSizeToUpload += fileInfo.Length;
							}

							progressCallback?.Invoke( totalSizeToUpload, 0, 0, 0 );
							var callbackProgressLastTotalProcessedSize = 0L;
							var callbackProgressLastPercentage = 0;

							for( int n = 0; n < targetFilePaths.Length; n++ )
							{
								var targetFilePath = targetFilePaths[ n ];
								var sourceFullPath = sourceFullPaths[ n ];
								var size = sizes[ n ];
								var uploadUrl = answer.UploadUrls[ n ];

								void Progress( int uploadedIncrement, long totalUploaded, long totalSize )// int bytesRead )
								{
									totalUploadedSize += uploadedIncrement;// bytesRead;

									//percentage callback
									var percentage = (int)MathEx.Clamp( (double)totalUploadedSize / Math.Max( totalSizeToUpload, 1 ) * 100, 0, 100 );
									//if( callbackProgressLastTotalProcessedSize != totalUploadedSize || callbackProgressLastPercentage != percentage )
									{
										progressCallback?.Invoke( totalSizeToUpload, uploadedIncrement/*bytesRead*/, totalUploadedSize, percentage );
										callbackProgressLastTotalProcessedSize = totalUploadedSize;
										callbackProgressLastPercentage = percentage;
									}

									////percentage callback
									//var percentage = (int)MathEx.Clamp( (double)totalUploadedSize / Math.Max( totalSizeToUpload, 1 ) * 100, 0, 100 );
									//if( callbackProgressPercentageLastValue != percentage )
									//{
									//	progressCallback?.Invoke( percentage );
									//	callbackProgressPercentageLastValue = percentage;
									//}
								}

								var downloadResult = await NetworkUtility.UploadFileByUrlAsync( uploadUrl, sourceFullPath, true, Progress, cancellationToken );
								if( !string.IsNullOrEmpty( downloadResult.Error ) )
									return new SimpleResult() { Error = downloadResult.Error };


								//using( var fileStream = new StreamWithReadCounter( sourceFullPath, Progress ) )
								//{
								//	var content = new StreamContent( fileStream );
								//	var contentType = MimeTypeUtility.GetMimeType( Path.GetExtension( sourceFullPath ) );
								//	content.Headers.ContentType = new MediaTypeHeaderValue( contentType );

								//	var response = await httpClient.PutAsync( uploadUrl, content );
								//	response.EnsureSuccessStatusCode();
								//}


								//void Progress( int bytesRead )
								//{
								//	totalUploadedSize += bytesRead;

								//	//percentage callback
								//	var percentage = (int)MathEx.Clamp( (double)totalUploadedSize / Math.Max( totalSizeToUpload, 1 ) * 100, 0, 100 );
								//	//if( callbackProgressLastTotalProcessedSize != totalUploadedSize || callbackProgressLastPercentage != percentage )
								//	{
								//		progressCallback?.Invoke( totalSizeToUpload, bytesRead, totalUploadedSize, percentage );
								//		callbackProgressLastTotalProcessedSize = totalUploadedSize;
								//		callbackProgressLastPercentage = percentage;
								//	}

								//	////percentage callback
								//	//var percentage = (int)MathEx.Clamp( (double)totalUploadedSize / Math.Max( totalSizeToUpload, 1 ) * 100, 0, 100 );
								//	//if( callbackProgressPercentageLastValue != percentage )
								//	//{
								//	//	progressCallback?.Invoke( percentage );
								//	//	callbackProgressPercentageLastValue = percentage;
								//	//}
								//}

								////using( var fileStream = File.OpenRead( sourceFullPath ) )
								//using( var fileStream = new StreamWithReadCounter( sourceFullPath, Progress ) )
								//{
								//	var content = new StreamContent( fileStream );
								//	var contentType = MimeTypeUtility.GetMimeType( Path.GetExtension( sourceFullPath ) );
								//	content.Headers.ContentType = new MediaTypeHeaderValue( contentType );

								//	var response = await httpClient.PutAsync( uploadUrl, content );
								//	response.EnsureSuccessStatusCode();
								//}

							}

							//compare file content? or hash

							if( totalSizeToUpload != totalUploadedSize )
								return new SimpleResult { Error = $"Invalid total uploaded size. realTotalSizeToUpload {totalSizeToUpload}, totalUploadedSize {totalUploadedSize}" };

							//percentage callback
							if( totalSizeToUpload == 0 )
							{
								try
								{
									var percentage = 100;
									if( callbackProgressLastTotalProcessedSize != totalUploadedSize || callbackProgressLastPercentage != percentage )
									{
										progressCallback?.Invoke( totalSizeToUpload, 0, totalUploadedSize, percentage );
										callbackProgressLastTotalProcessedSize = totalUploadedSize;
										callbackProgressLastPercentage = percentage;
									}
								}
								catch( Exception e )
								{
									return new SimpleResult { Error = e.Message };
								}
							}

							//{
							//	//percentage callback
							//	var percentage = (int)MathEx.Clamp( (double)totalUploadedSize / Math.Max( totalSizeToUpload, 1 ) * 100, 0, 100 );
							//	if( callbackProgressLastTotalProcessedSize != totalUploadedSize || callbackProgressLastPercentage != percentage )
							//	{
							//		progressCallback?.Invoke( totalSizeToUpload, 0, totalUploadedSize, percentage );
							//		callbackProgressLastTotalProcessedSize = totalUploadedSize;
							//		callbackProgressLastPercentage = percentage;
							//	}

							//	////percentage callback
							//	//var percentage = (int)MathEx.Clamp( (double)totalUploadedSize / Math.Max( totalSizeToUpload, 1 ) * 100, 0, 100 );
							//	//if( callbackProgressPercentageLastValue != percentage )
							//	//{
							//	//	progressCallback?.Invoke( percentage );
							//	//	callbackProgressPercentageLastValue = percentage;
							//	//}
							//}

							return new SimpleResult();
						}
						catch( Exception e )
						{
							return new SimpleResult { Error = e.Message };
						}
					}

					await Task.Delay( 1 );
					if( cancellationToken.IsCancellationRequested )
					{
						SendCancelRequest( requestID );
						return new SimpleResult { Error = "Operation was canceled." };
					}
					if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
						return new SimpleResult() { Error = ConnectionErrorReceived };
				}
			}
			else
			{
				var maxBlockSize = UploadFilesMaxBlockSize;
				{
					//get max block size from the server

					var serverCommonInfo = await GetServerCommonInfoAsync( cancellationToken );
					if( !string.IsNullOrEmpty( serverCommonInfo.Error ) )
						return new SimpleResult { Error = serverCommonInfo.Error };

					//!!!!need 0.95?
					if( ServerCommonInfo.UploadFilesMaxBlockSize != 0 )
						maxBlockSize = Math.Min( maxBlockSize, (int)( ServerCommonInfo.UploadFilesMaxBlockSize * 0.95f ) );
					else
						maxBlockSize = Math.Min( maxBlockSize, 32768 * 16 );
				}
				//var maxQueueSize = 10 * 1024 * 1024;
				//var maxQueueSize = UploadFilesMaxQueueSize;

				const int maxPartCountInGroup = 10000;

				var uploadParts = new List<UploadPart>();

				for( int n = 0; n < sourceFullPaths.Length; n++ )
				{
					var sourceFullPath = sourceFullPaths[ n ];
					var targetFilePath = targetFilePaths[ n ];

					var size = 0L;
					try
					{
						var fileInfo = new FileInfo( sourceFullPath );
						if( !fileInfo.Exists )
							return new SimpleResult { Error = $"File \"{sourceFullPath}\" is not exists." };
						size = fileInfo.Length;

						//also can check hashes
						//if( fileInfo.Exists && fileInfo.Length == storageFileInfo.Size && fileInfo.LastWriteTimeUtc >= storageFileInfo.LastModifiedUtc )
					}
					catch( Exception e )
					{
						return new SimpleResult { Error = e.Message };
					}

					for( int from = 0; from < size; from += maxBlockSize )
					{
						long to = from + maxBlockSize;
						if( to > size )
							to = size;

						if( from != to )
						{
							var part = new UploadPart();
							part.SourceFullPath = sourceFullPath;
							part.TargetFileName = targetFilePath;
							part.PartStart = from;
							part.PartEnd = to;
							uploadParts.Add( part );
						}
					}

					if( size == 0 )
					{
						var part = new UploadPart();
						part.SourceFullPath = sourceFullPath;
						part.TargetFileName = targetFilePath;
						part.PartStart = 0;
						part.PartEnd = 0;
						uploadParts.Add( part );
					}
				}

				var uploadGroups = new List<UploadPartGroup>( uploadParts.Count );
				{
					var currentGroup = new UploadPartGroup();

					for( int nPart = 0; nPart < uploadParts.Count; nPart++ )
					{
						var part = uploadParts[ nPart ];

						if( currentGroup.Size + part.Size > maxBlockSize || currentGroup.Parts.Count >= maxPartCountInGroup )
						{
							uploadGroups.Add( currentGroup );
							currentGroup = new UploadPartGroup();
						}
						currentGroup.Parts.Add( part );
						currentGroup.Size += part.Size;
					}

					if( currentGroup.Parts.Count != 0 )
						uploadGroups.Add( currentGroup );
				}

				var uploadGroupsDataSize = 0L;
				foreach( var group in uploadGroups )
					uploadGroupsDataSize += group.Size;

				progressCallback?.Invoke( uploadGroupsDataSize, 0, 0, 0 );
				var callbackProgressLastTotalProcessedSize = 0L;
				var callbackProgressLastPercentage = 0;

				var totalUploadedSize = 0L;

				//!!!!maybe be improved to send two groups without waiting answer for first group

				foreach( var group in uploadGroups )
				{
					var requestID = GetRequestID();
					group.RequestID = requestID;
					SendUploadFileContent( requestID, group, anyData, out var error );
					if( !string.IsNullOrEmpty( error ) )
						return new SimpleResult { Error = error };

					while( true )
					{
						var answer = GetAnswerAndRemove<UploadFileContentAnswerItem>( requestID );
						if( answer != null )
						{
							if( !string.IsNullOrEmpty( answer.Error ) )
								return new SimpleResult { Error = answer.Error };

							totalUploadedSize += group.Size;

							try
							{
								//percentage callback
								var percentage = (int)MathEx.Clamp( (double)totalUploadedSize / Math.Max( uploadGroupsDataSize, 1 ) * 100, 0, 100 );
								//if( callbackProgressLastTotalProcessedSize != totalUploadedSize || callbackProgressLastPercentage != percentage )
								{
									progressCallback?.Invoke( uploadGroupsDataSize, group.Size, totalUploadedSize, percentage );
									callbackProgressLastTotalProcessedSize = totalUploadedSize;
									callbackProgressLastPercentage = percentage;
								}

								////percentage callback
								//var percentage = (int)MathEx.Clamp( (double)totalUploadedSize / Math.Max( uploadGroupsDataSize, 1 ) * 100, 0, 100 );
								//if( callbackProgressPercentageLastValue != percentage )
								//{
								//	progressCallback?.Invoke( percentage );
								//	callbackProgressPercentageLastValue = percentage;
								//}
							}
							catch( Exception e )
							{
								return new SimpleResult { Error = e.Message };
							}

							//go to next group
							break;
						}

						await Task.Delay( 1 );
						if( cancellationToken.IsCancellationRequested )
						{
							SendCancelRequest( requestID );
							return new SimpleResult { Error = "Operation was canceled." };
						}
						if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
							return new SimpleResult() { Error = ConnectionErrorReceived };
					}
				}

				//percentage callback
				if( uploadGroupsDataSize == 0 )
				{
					try
					{
						var percentage = 100;
						if( callbackProgressLastTotalProcessedSize != totalUploadedSize || callbackProgressLastPercentage != percentage )
						{
							progressCallback?.Invoke( uploadGroupsDataSize, 0, totalUploadedSize, percentage );
							callbackProgressLastTotalProcessedSize = totalUploadedSize;
							callbackProgressLastPercentage = percentage;
						}
					}
					catch( Exception e )
					{
						return new SimpleResult { Error = e.Message };
					}
				}

				return new SimpleResult();
			}
		}

		//was not wrapped
		public async Task<SimpleResult> UploadFileAsync( DataSource source, string sourceFullPath, string targetFilePath, string anyData = null, UploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await UploadFilesAsync( source, new string[] { sourceFullPath }, new string[] { targetFilePath }, anyData, progressCallback, cancellationToken );
		}

		///////////////////////////////////////////////

		class CreateDirectoryAnswerItem : AnswerItem
		{
		}

		bool ReceiveMessage_CreateDirectoryAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new CreateDirectoryAnswerItem();
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		void SendCreateDirectory( long requestID, DataSource source, string directoryPath, string anyData )
		{
			var m = BeginMessage( createDirectoryMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.Write( source == DataSource.Storage );
			writer.Write( directoryPath );
			writer.Write( anyData );
			m.End();
		}

		public async Task<SimpleResult> CreateDirectoryAsync( DataSource source, string directoryPath, string anyData = null, CancellationToken cancellationToken = default )
		{
			var requestID = GetRequestID();
			SendCreateDirectory( requestID, source, directoryPath, anyData );

			while( true )
			{
				var answer = GetAnswerAndRemove<CreateDirectoryAnswerItem>( requestID );
				if( answer != null )
					return new SimpleResult { Error = answer.Error };

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
				{
					SendCancelRequest( requestID );
					return new SimpleResult { Error = "Operation was canceled." };
				}
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new SimpleResult() { Error = ConnectionErrorReceived };
			}
		}

		public async Task<SimpleResult> UploadDirectoryAsync( DataSource source, string sourceFullPath, string targetDirectoryName, SearchOption searchOption, string anyData = null, UploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await UploadObjectsAsync( source, new string[] { sourceFullPath }, new string[] { targetDirectoryName }, searchOption, anyData, progressCallback, cancellationToken );

			//try
			//{
			//	// Get all directories in the source directory
			//	var directories = Directory.GetDirectories( sourceFullPath, "*", searchOption );
			//	var targetDirectories = new string[ directories.Length + 1 ];
			//	targetDirectories[ 0 ] = targetDirectoryName;

			//	for( int i = 0; i < directories.Length; i++ )
			//	{
			//		var relativePath = Path.GetRelativePath( sourceFullPath, directories[ i ] );
			//		targetDirectories[ i + 1 ] = Path.Combine( targetDirectoryName, relativePath );
			//	}

			//	// Get list of already created directories
			//	var existingDirectoriesResult = await GetDirectoryInfoAsync( source, targetDirectoryName, searchOption, anyData, cancellationToken );
			//	if( !string.IsNullOrEmpty( existingDirectoriesResult.Error ) )
			//		return new SimpleResult { Error = existingDirectoriesResult.Error };

			//	var existingDirectories = new HashSet<string>( existingDirectoriesResult.Files.Where( f => f.IsDirectory ).Select( f => f.Path ) );

			//	// Create the target directories
			//	foreach( var targetDirectory in targetDirectories )
			//	{
			//		if( !existingDirectories.Contains( targetDirectory ) )
			//		{
			//			var createDirResult = await CreateDirectoryAsync( source, targetDirectory, anyData, cancellationToken );
			//			if( !string.IsNullOrEmpty( createDirResult.Error ) )
			//				return createDirResult;
			//		}
			//	}

			//	// Get all files in the source directory
			//	var files = Directory.GetFiles( sourceFullPath, "*", searchOption );
			//	var targetFilePaths = new string[ files.Length ];

			//	for( int i = 0; i < files.Length; i++ )
			//	{
			//		var relativePath = Path.GetRelativePath( sourceFullPath, files[ i ] );
			//		targetFilePaths[ i ] = Path.Combine( targetDirectoryName, relativePath );
			//	}

			//	// Upload all files
			//	var uploadResult = await UploadFilesAsync2( source, files, targetFilePaths, anyData, progressCallback, cancellationToken );
			//	if( !string.IsNullOrEmpty( uploadResult.Error ) )
			//		return uploadResult;

			//	return new SimpleResult();
			//}
			//catch( Exception e )
			//{
			//	return new SimpleResult { Error = e.Message };
			//}


			////try
			////{
			////	// Create the target directory
			////	var createDirResult = await CreateDirectoryAsync( source, targetDirectoryName, anyData, cancellationToken );
			////	if( !string.IsNullOrEmpty( createDirResult.Error ) )
			////		return createDirResult;

			////	// Get all files in the source directory
			////	var files = Directory.GetFiles( sourceFullPath, "*", SearchOption.AllDirectories );
			////	var targetFilePaths = new string[ files.Length ];

			////	for( int i = 0; i < files.Length; i++ )
			////	{
			////		var relativePath = Path.GetRelativePath( sourceFullPath, files[ i ] );
			////		targetFilePaths[ i ] = Path.Combine( targetDirectoryName, relativePath );
			////	}

			////	// Upload all files
			////	var uploadResult = await UploadFilesAsync2( source, files, targetFilePaths, anyData, progressCallback, cancellationToken );
			////	if( !string.IsNullOrEmpty( uploadResult.Error ) )
			////		return uploadResult;

			////	return new SimpleResult();
			////}
			////catch( Exception e )
			////{
			////	return new SimpleResult { Error = e.Message };
			////}
		}

		//!!!!
		//static long CalculateTotalSize( string directoryPath, SearchOption searchOption )
		//{
		//	long totalSize = 0;
		//	var files = Directory.GetFiles( directoryPath, "*.*", searchOption );
		//	foreach( var file in files )
		//	{
		//		var fileInfo = new FileInfo( file );
		//		totalSize += fileInfo.Length;
		//	}
		//	return totalSize;
		//}

		//static List<string> GetDirectoryWithSearchOption( string directoryPath, SearchOption searchOption )
		//{
		//	var directories = new List<string>( Directory.GetDirectories( directoryPath, "*", searchOption ) );
		//	return directories;
		//}

		static bool IsDirectoryEmpty( string directoryPath )
		{
			return Directory.GetFiles( directoryPath ).Length == 0 && Directory.GetDirectories( directoryPath ).Length == 0;
		}

		static List<string> GetEmptyDirectories( DirectoryInfo directory, SearchOption searchOption )
		{
			var emptyDirectories = new List<string>();
			var directories = directory.GetDirectories( "*", searchOption );
			foreach( var dir in directories )
			{
				if( IsDirectoryEmpty( dir.FullName ) )
					emptyDirectories.Add( dir.FullName );
			}
			return emptyDirectories;
		}

		//public static void Main( string[] args )
		//{
		//	string directoryPath = @"C:\YourDirectoryPath";
		//	SearchOption searchOption = SearchOption.AllDirectories;

		//	long totalSize = CalculateTotalSize( directoryPath, searchOption );
		//	Console.WriteLine( $"Total Size: {totalSize} bytes" );

		//	var directories = GetDirectoryWithSearchOption( directoryPath, searchOption );
		//	Console.WriteLine( "Directories:" );
		//	foreach( var dir in directories )
		//	{
		//		Console.WriteLine( dir );
		//	}

		//	var emptyDirectories = GetEmptyDirectories( directoryPath, searchOption );
		//	Console.WriteLine( "Empty Directories:" );
		//	foreach( var emptyDir in emptyDirectories )
		//	{
		//		Console.WriteLine( emptyDir );
		//	}
		//}

		public async Task<SimpleResult> UploadObjectsAsync( DataSource source, string[] sourceFullPaths, string[] targetFilePaths, SearchOption searchOption, string anyData = null, UploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			try
			{
				var totalSizeToUpload = 0L;
				var filesToUploadSourceFullPaths = new List<string>();
				var filesToUploadTargetFilePaths = new List<string>();
				var emptyDirectoriesToCreate = new List<string>();

				{
					for( int i = 0; i < sourceFullPaths.Length; i++ )
					{
						var sourcePath = PathUtility.NormalizePath( sourceFullPaths[ i ] );
						var targetPath = PathUtility.NormalizePath( targetFilePaths[ i ] );

						var fileInfo = new FileInfo( sourcePath );
						if( fileInfo.Exists )
						{
							totalSizeToUpload += fileInfo.Length;
							filesToUploadSourceFullPaths.Add( sourcePath );
							filesToUploadTargetFilePaths.Add( targetPath );
						}
						else
						{
							var directoryInfo = new DirectoryInfo( sourcePath );
							if( directoryInfo.Exists )
							{
								var files = directoryInfo.GetFiles( "*.*", searchOption );
								foreach( var file in files )
								{
									totalSizeToUpload += file.Length;

									var relativePath = Path.GetRelativePath( sourcePath, file.FullName );
									var targetFilePath = Path.Combine( targetPath, relativePath );

									filesToUploadSourceFullPaths.Add( file.FullName );
									filesToUploadTargetFilePaths.Add( targetFilePath );
								}

								var emptyDirectories = GetEmptyDirectories( directoryInfo, searchOption );
								foreach( var emptyDir in emptyDirectories )
								{
									var relativePath = Path.GetRelativePath( sourcePath, emptyDir );
									var targetFilePath = Path.Combine( targetPath, relativePath );

									emptyDirectoriesToCreate.Add( targetFilePath );
								}
							}
							else
							{
								return new SimpleResult { Error = $"File or directory not exists. \"{sourcePath}\"." };
							}
						}
					}
				}

				if( filesToUploadSourceFullPaths.Count > 0 )
				{
					var result = await UploadFilesAsync( source, filesToUploadSourceFullPaths.ToArray(), filesToUploadTargetFilePaths.ToArray(), anyData, progressCallback, cancellationToken );
					if( !string.IsNullOrEmpty( result.Error ) )
						return result;
				}

				foreach( var directory in emptyDirectoriesToCreate )
				{
					var result = await CreateDirectoryAsync( source, directory, anyData, cancellationToken );
					if( !string.IsNullOrEmpty( result.Error ) )
						return result;
				}

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult { Error = e.Message };
			}
		}

		//public async Task<SimpleResult> UploadObjectsAsync( DataSource source, string[] sourceFullPaths, string[] targetFilePaths, string anyData = null, UploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		//{
		//	try
		//	{

		//		return new SimpleResult();



		//		//var sourceFullPathsFiles = new List<string>();
		//		//var targetFilePathsFiles = new List<string>();

		//		//for( int n = 0; n < sourceFullPaths.Length; n++ )
		//		//{
		//		//	var sourceFullPath = sourceFullPaths[ n ];
		//		//	var targetFilePath = targetFilePaths[ n ];

		//		//	var directoryInfo = new DirectoryInfo( sourceFullPath );
		//		//	if( directoryInfo.Exists )
		//		//	{

		//		//		//!!!!upload all, not just create

		//		//		//var result = await CreateDirectoryAsync( source, targetFilePath, anyData: anyData, cancellationToken: cancellationToken );
		//		//		//if( !string.IsNullOrEmpty( result.Error ) )
		//		//		//	return result;


		//		//	}
		//		//	else
		//		//	{
		//		//		sourceFullPathsFiles.Add( sourceFullPath );
		//		//		targetFilePathsFiles.Add( targetFilePath );


		//		//		//var fileInfo = new FileInfo( sourceFullPath );
		//		//		//if( fileInfo.Exists )
		//		//		//{
		//		//		//	var result = await UploadFileAsync( source, sourceFullPath, targetFilePath, anyData: anyData, progressCallback: progressCallback, cancellationToken: cancellationToken );
		//		//		//	if( !string.IsNullOrEmpty( result.Error ) )
		//		//		//		return result;
		//		//		//}
		//		//		//else
		//		//		//	return new SimpleResult { Error = $"File \"{sourceFullPath}\" not exists." };
		//		//	}


		//		//	//var fileInfo = new FileInfo( sourceFullPath );

		//		//	//if( fileInfo.Exists )
		//		//	//{
		//		//	//	var result = await UploadFileAsync( source, sourceFullPath, targetFilePath, anyData: anyData, progressCallback: progressCallback, cancellationToken: cancellationToken );
		//		//	//	if( !string.IsNullOrEmpty( result.Error ) )
		//		//	//		return result;
		//		//	//}
		//		//	//else
		//		//	//	return new SimpleResult { Error = $"File \"{sourceFullPath}\" not exists." };


		//		//}

		//		//if( sourceFullPathsFiles.Count != 0 )
		//		//{
		//		//	zzzzz;

		//		//	var result = await UploadFilesAsync( source, targetFilePathsFiles.ToArray(), sourceFullPathsFiles.ToArray(), anyData: anyData, progressCallback: progressCallback, cancellationToken: cancellationToken );
		//		//	if( !string.IsNullOrEmpty( result.Error ) )
		//		//		return result;
		//		//}



		//	}
		//	catch( Exception e )
		//	{
		//		return new SimpleResult { Error = e.Message };
		//	}
		//}

		///////////////////////////////////////////////

		class DeleteFilesAnswerItem : AnswerItem
		{
		}

		bool ReceiveMessage_DeleteFilesAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new DeleteFilesAnswerItem();
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		void SendDeleteFiles( long requestID, DataSource source, string[] filePaths, string anyData )
		{
			var m = BeginMessage( deleteFilesMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.Write( source == DataSource.Storage );
			writer.WriteVariableInt32( filePaths.Length );
			for( int n = 0; n < filePaths.Length; n++ )
				writer.Write( filePaths[ n ] );
			writer.Write( anyData );
			m.End();
		}

		public async Task<SimpleResult> DeleteFilesAsync( DataSource source, string[] filePaths, string anyData = null, CancellationToken cancellationToken = default )
		{
			var requestID = GetRequestID();
			SendDeleteFiles( requestID, source, filePaths, anyData );

			while( true )
			{
				var answer = GetAnswerAndRemove<DeleteFilesAnswerItem>( requestID );
				if( answer != null )
					return new SimpleResult { Error = answer.Error };

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
				{
					SendCancelRequest( requestID );
					return new SimpleResult { Error = "Operation was canceled." };
				}
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new SimpleResult() { Error = ConnectionErrorReceived };
			}
		}

		public async Task<SimpleResult> DeleteFileAsync( DataSource source, string filePath, string anyData = null, CancellationToken cancellationToken = default )
		{
			return await DeleteFilesAsync( source, new string[] { filePath }, anyData, cancellationToken );
		}

		///////////////////////////////////////////////

		class DeleteDirectoryAnswerItem : AnswerItem
		{
		}

		bool ReceiveMessage_DeleteDirectoryAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var requestID = reader.ReadVariableInt64();
			var error = reader.ReadString() ?? "";
			if( !reader.Complete() )
				return false;

			try
			{
				var answerItem = new DeleteDirectoryAnswerItem();
				answerItem.Error = error;
				answerItem.CreationTime = DateTime.UtcNow;
				answers[ requestID ] = answerItem;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		void SendDeleteDirectory( long requestID, DataSource source, string directoryPath, bool recursive, bool clear, string anyData )
		{
			var m = BeginMessage( deleteDirectoryMessage );
			var writer = m.Writer;
			writer.WriteVariable( requestID );
			writer.Write( source == DataSource.Storage );
			writer.Write( directoryPath );
			writer.Write( recursive );
			writer.Write( clear );
			writer.Write( anyData );
			m.End();
		}

		public async Task<SimpleResult> DeleteDirectoryAsync( DataSource source, string directoryPath, bool recursive, bool clear, string anyData = null, CancellationToken cancellationToken = default )
		{
			var requestID = GetRequestID();
			SendDeleteDirectory( requestID, source, directoryPath, recursive, clear, anyData );

			while( true )
			{
				var answer = GetAnswerAndRemove<DeleteDirectoryAnswerItem>( requestID );
				if( answer != null )
					return new SimpleResult { Error = answer.Error };

				await Task.Delay( 1 );
				if( cancellationToken.IsCancellationRequested )
				{
					SendCancelRequest( requestID );
					return new SimpleResult { Error = "Operation was canceled." };
				}
				if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
					return new SimpleResult() { Error = ConnectionErrorReceived };
			}
		}

		public struct DeleteObjectsItem
		{
			public string Path;
			public bool IsDirectory;

			public DeleteObjectsItem( string path, bool isDirectory )
			{
				Path = path;
				IsDirectory = isDirectory;
			}
		}

		public async Task<SimpleResult> DeleteObjectsAsync( DataSource source, DeleteObjectsItem[] objects, string anyData = null, CancellationToken cancellationToken = default )
		{
			try
			{
				//delete directories
				for( int n = 0; n < objects.Length; n++ )
				{
					var item = objects[ n ];
					if( item.IsDirectory )
					{
						var result = await DeleteDirectoryAsync( source, item.Path, true, true, anyData, cancellationToken );
						if( !string.IsNullOrEmpty( result.Error ) )
							return result;
					}
				}

				//delete files
				var files = objects.Where( i => !i.IsDirectory ).ToArray();
				if( files.Length != 0 )
				{
					var filePaths = files.Select( i => i.Path ).ToArray();
					var result = await DeleteFilesAsync( source, filePaths, anyData, cancellationToken );
					if( !string.IsNullOrEmpty( result.Error ) )
						return result;
				}

				return new SimpleResult();
			}
			catch( Exception e )
			{
				return new SimpleResult { Error = e.Message };
			}
		}

		public void RegisterAssemblyForCloudMethodTypes( Assembly assembly )
		{
			if( !registeredCloudMethodAssemblies.Contains( assembly ) )
				registeredCloudMethodAssemblies.Add( assembly );
		}

		Type FindTypeForCloudMethod( string typeName )
		{
			var simpleTypeItem = SimpleTypes.GetTypeItem( typeName );
			if( simpleTypeItem != null )
				return simpleTypeItem.Type;

			var type = Type.GetType( typeName );
			if( type != null )
				return type;

			foreach( var assembly in registeredCloudMethodAssemblies )
			{
				type = assembly.GetType( typeName, false, true );
				if( type != null )
					return type;
			}

			//type = typeof( Vector2 ).Assembly.GetType( typeName );
			//if( type != null )
			//	return type;

			return null;
		}

		bool ReceiveMessage_GetCommonInfoAnswer( MessageType messageType, ArrayDataReader reader, ref string additionalErrorMessage )
		{
			var blockString = reader.ReadString();
			if( !reader.Complete() )
				return false;

			try
			{
				var block = TextBlock.Parse( blockString, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					additionalErrorMessage = error;
					return false;
				}

				var info = new ServerCommonInfoClass();
				info.UploadFilesMaxBlockSize = int.Parse( block.GetAttribute( "UploadFilesMaxBlockSize", "0" ) );
				info.ReceiveMessageDownloadFilesContentMaxSize = int.Parse( block.GetAttribute( "ReceiveMessageDownloadFilesContentMaxSize", "0" ) );
				serverCommonInfo = info;
			}
			catch( Exception e )
			{
				additionalErrorMessage = e.Message;
				return false;
			}

			return true;
		}

		public async Task<SimpleResult> GetServerCommonInfoAsync( CancellationToken cancellationToken )
		{
			if( serverCommonInfo == null )
			{
				//request
				var m = BeginMessage( getCommonInfoMessage );
				m.End();

				//wait for answer
				while( serverCommonInfo == null )
				{
					await Task.Delay( 1 );
					if( cancellationToken.IsCancellationRequested )
						return new SimpleResult { Error = "Operation was canceled." };
					if( !string.IsNullOrEmpty( ConnectionErrorReceived ) )
						return new SimpleResult() { Error = ConnectionErrorReceived };
				}
			}

			return new SimpleResult();
		}
	}
}