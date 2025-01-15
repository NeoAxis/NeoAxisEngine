// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace NeoAxis.Networking
{
	/// <summary>
	/// A basic client to access cloud functions. Another way is using ClientNetworkService_CloudFunctions.
	/// </summary>
	public class CloudFunctionsClient : BasicServiceClient
	{
		static List<CloudFunctionsClient> instances;
		static CloudFunctionsClient firstInstance;

		string helloFromServerMessage;

		///////////////////////////////////////////////

		public class CloudFunctionsNode : BasicServiceNode
		{
			ClientNetworkService_CloudFunctions cloudFunctions;

			//

			public CloudFunctionsNode()
			{
				cloudFunctions = new ClientNetworkService_CloudFunctions();
				RegisterService( cloudFunctions );
			}

			public ClientNetworkService_CloudFunctions CloudFunctions
			{
				get { return cloudFunctions; }
			}
		}

		///////////////////////////////////////////////

		public class CreateResult
		{
			public CloudFunctionsClient Client;
			public string Error;
		}

		///////////////////////////////////////////////

		public class SaveStringResult
		{
			public string Error { get; set; }
		}

		///////////////////////////////////////////////

		public class LoadStringsResult
		{
			public string[] Values { get; set; }
			public string Error { get; set; }
		}

		///////////////////////////////////////////////

		public class LoadStringResult
		{
			public string Value { get; set; }
			public string Error { get; set; }
		}

		///////////////////////////////////////////////

		public class CallMethodResult<T>
		{
			public T Value { get; set; }
			public string Error { get; set; }
		}

		///////////////////////////////////////////////

		public class LoadFilesInfoResult
		{
			public FileItem[] Items { get; set; }
			public string Error { get; set; }

			public struct FileItem
			{
				public string FilePath { get; set; }
				public long Size { get; set; }
				//!!!!
				public string Hash { get; set; }
			}
		}

		///////////////////////////////////////////////

		public class LoadFilesResult
		{
			public string Error { get; set; }
		}

		///////////////////////////////////////////////

		static CloudFunctionsClient()
		{
			instances = new List<CloudFunctionsClient>();
		}

		CloudFunctionsClient( bool autoUpdate )
			: base( autoUpdate )
		{
			ServiceName = "CloudFunctions";
		}

		//!!!!need?
		public static CloudFunctionsClient[] GetInstances()
		{
			lock( instances )
				return instances.ToArray();
		}

		//!!!!need?
		public static CloudFunctionsClient FirstInstance
		{
			get { return firstInstance; }
		}

		public static async Task<CreateResult> CreateAsync( ConnectionSettingsClass connectionSettings, bool autoUpdate, bool connect )
		{
			var instance = new CloudFunctionsClient( autoUpdate );
			instance.ConnectionSettings = connectionSettings;

			lock( instances )
			{
				instances.Add( instance );
				firstInstance = instances.Count > 0 ? instances[ 0 ] : null;
			}

			if( connect )
			{
				var error = await instance.ReconnectAsync();
				if( !string.IsNullOrEmpty( error ) )
					return new CreateResult() { Error = error };
			}

			return new CreateResult() { Client = instance };
		}

		protected override BasicServiceNode OnCreateNetworkNode()
		{
			return new CloudFunctionsNode();
		}

		public new CloudFunctionsNode ConnectionNode
		{
			get { return (CloudFunctionsNode)base.ConnectionNode; }
		}

		public string HelloFromServerMessage
		{
			get { return helloFromServerMessage; }
		}

		////async?
		//public static CloudFunctions2 ConnectDirect( string serverAddress, int serverPort, string password/*, bool callUpdateFromInternalThread*/, out string error )
		//{
		//	error = "";

		//	zzz;//instances

		//	var instance = new CloudFunctions2();
		//	//instance.connectionType = ConnectionTypeEnum.Direct;
		//	//instance.serverAddress = serverAddress;
		//	//instance.serverPort = serverPort;
		//	//instance.password = password;
		//	instance.callUpdateFromInternalThread = callUpdateFromInternalThread;

		//	//connect to the server
		//	instance.connectionNode = CloudFunctionsClient2.BeginConnect( serverAddress, serverPort, password, instance, out error );
		//	if( instance.connectionNode == null )
		//		return null;
		//	//if( !CloudFunctionsClient2.BeginConnect( serverAddress, serverPort, password, instance, out error ) )
		//	//	return null;

		//	instance.PostConnect();

		//	return instance;
		//}

		////async?
		//public static CloudFunctions2 ConnectViaCloud( bool callUpdateFromInternalThread, out string error )
		//{
		//	error = "";

		//	var instance = new CloudFunctions2();
		//	//instance.connectionType = ConnectionTypeEnum.ViaCloud;
		//	instance.callUpdateFromInternalThread = callUpdateFromInternalThread;

		//	string serverAddress;
		//	int serverPort;
		//	string verificationCode;

		//	//request server address from the cloud service
		//	try
		//	{
		//		var requestResult = GeneralManagerFunctions.RequestService( "CloudFunctions" );
		//		if( !string.IsNullOrEmpty( requestResult.Error ) )
		//		{
		//			error = "Unable to get server address from the cloud service. " + requestResult.Error;
		//			return null;
		//		}

		//		serverAddress = requestResult.ServerAddress;
		//		serverPort = requestResult.ServerPort;
		//		verificationCode = requestResult.VerificationCode;
		//	}
		//	catch( Exception e )
		//	{
		//		error = e.Message;
		//		return null;
		//	}

		//	//connect to the server
		//	instance.connectionNode = CloudFunctionsClient2.BeginConnect( serverAddress, serverPort, verificationCode, instance, out error );
		//	if( instance.connectionNode == null )
		//		return null;
		//	//if( !CloudFunctionsClient2.BeginConnect( serverAddress, serverPort, verificationCode, instance, out error ) )
		//	//	return null;

		//	instance.PostConnect();

		//	return instance;
		//}

		//void PostConnect()
		//{
		//if( callUpdateFromInternalThread )
		//{
		//	backgroundThread = new Thread( delegate ()
		//	{
		//		while( true )
		//		{
		//			Update();
		//			Thread.Sleep( 0 );
		//		}
		//	} );
		//	backgroundThread.IsBackground = true;
		//	backgroundThread.Start();
		//}
		//}

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		//!!!!
		//public static void UpdateAll()
		//{
		//	foreach( var instance in GetInstances() )
		//		instance.Update();
		//}

		protected override void OnDestroy()
		{
			lock( instances )
			{
				instances.Remove( this );
				firstInstance = instances.Count > 0 ? instances[ 0 ] : null;
			}

			base.OnDestroy();
		}

		protected override void OnClient_ProtocolError( ClientNode sender, string message )
		{
			base.OnClient_ProtocolError( sender, message );

			if( ConnectionNode != null )
				ConnectionNode.CloudFunctions.ConnectionErrorReceived = ConnectionErrorReceived;
		}

		protected override void OnClient_ConnectionStatusChanged( ClientNode sender )
		{
			base.OnClient_ConnectionStatusChanged( sender );

			if( sender.Status == NetworkStatus.Disconnected )
			{
				if( !string.IsNullOrEmpty( sender.DisconnectionReason ) )
				{
					if( ConnectionNode != null )
						ConnectionNode.CloudFunctions.ConnectionErrorReceived = ConnectionErrorReceived;
				}
			}
		}

		protected override void OnMessages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			base.OnMessages_ReceiveMessageString( sender, message, data );

			if( message == "HelloFromServerMessage" )
				helloFromServerMessage = data;
		}

		//!!!!
		//public static void DestroyAll()
		//{
		//	foreach( var instance in GetInstances() )
		//		instance.Destroy();
		//}

		//!!!!about reconnect
		//!!!!проверять когда было последнее сообщение. еще много где может быть проблема с накапливанием неотправленных сообщений

		//public bool Connected
		//{
		//	get
		//	{
		//		return false;
		//	}
		//}

		///// <summary>
		///// Returns error.
		///// </summary>
		///// <returns></returns>
		//public async Task<string> ReconnectAsync()
		//{
		//	try
		//	{
		//		connectionNode?.Dispose();
		//		connectionNode = null;

		//		string serverAddress;
		//		int serverPort;
		//		string password = null;
		//		string verificationCode = null;

		//		if( connectionSettings.ConnectionType == ConnectionSettingsClass.ConnectionTypeEnum.Cloudbox )
		//		{
		//			//request access info from Cloudbox

		//			//request verification code from general manager to entering server manager
		//			var requestCodeResult = await GeneralManagerFunctions.RequestVerificationCodeToEnterProjectAsync( connectionSettings.ProjectID, "Service" );

		//			if( !string.IsNullOrEmpty( requestCodeResult.Error ) )
		//				throw new Exception( requestCodeResult.Error );

		//			//var requestResult = GeneralManagerFunctions.RequestService( "CloudFunctions" );
		//			//if( !string.IsNullOrEmpty( requestResult.Error ) )
		//			//{
		//			//	error = "Unable to get server address from the cloud service. " + requestResult.Error;
		//			//	return false;
		//			//}


		//			//!!!!port


		//			serverAddress = requestCodeResult.Data.GetAttribute( "ServerAddress" );
		//			serverPort = int.Parse( requestCodeResult.Data.GetAttribute( "ServerPort" ) );
		//			verificationCode = requestCodeResult.Data.GetAttribute( "VerificationCode" );
		//		}
		//		else
		//		{
		//			//connect direct by IP
		//			serverAddress = connectionSettings.ServerAddress;
		//			serverPort = connectionSettings.ServerPort;
		//			password = connectionSettings.Password;
		//		}

		//		var node = new Node();
		//		node.ProtocolError += Client_ProtocolError;
		//		node.ConnectionStatusChanged += Client_ConnectionStatusChanged;

		//		var rootBlock = new TextBlock();
		//		if( !string.IsNullOrEmpty( verificationCode ) )
		//			rootBlock.SetAttribute( "VerificationCode", verificationCode );
		//		if( !string.IsNullOrEmpty( password ) )
		//			rootBlock.SetAttribute( "Password", password );

		//		if( !node.BeginConnect( serverAddress, serverPort, EngineInfo.Version, rootBlock.DumpToString(), 100, out error ) )
		//		{
		//			node.Dispose();
		//			node = null;
		//			return false;
		//		}

		//		connectionNode = node;
		//		return true;
		//	}
		//	catch( Exception e )
		//	{
		//		error = e.Message;
		//		return false;
		//	}
		//}

		//public bool Reconnect( out string error )
		//{
		//	error = "";

		//	try
		//	{
		//		connectionNode?.Dispose();
		//		connectionNode = null;

		//		string serverAddress;
		//		int serverPort;
		//		string password = null;
		//		string verificationCode = null;

		//		if( connectionSettings.ConnectionType == ConnectionSettingsClass.ConnectionTypeEnum.Cloudbox )
		//		{
		//			//request access info from Cloudbox

		//			//request verification code from general manager to entering server manager
		//			var requestCodeResultTask = GeneralManagerFunctions.RequestVerificationCodeToEnterProjectAsync( connectionSettings.ProjectID, "Service" );
		//			var requestCodeResult = requestCodeResultTask.GetAwaiter().GetResult();

		//			if( !string.IsNullOrEmpty( requestCodeResult.Error ) )
		//				throw new Exception( requestCodeResult.Error );

		//			//var requestResult = GeneralManagerFunctions.RequestService( "CloudFunctions" );
		//			//if( !string.IsNullOrEmpty( requestResult.Error ) )
		//			//{
		//			//	error = "Unable to get server address from the cloud service. " + requestResult.Error;
		//			//	return false;
		//			//}


		//			//!!!!port


		//			serverAddress = requestCodeResult.Data.GetAttribute( "ServerAddress" );
		//			serverPort = int.Parse( requestCodeResult.Data.GetAttribute( "ServerPort" ) );
		//			verificationCode = requestCodeResult.Data.GetAttribute( "VerificationCode" );
		//		}
		//		else
		//		{
		//			//connect direct by IP
		//			serverAddress = connectionSettings.ServerAddress;
		//			serverPort = connectionSettings.ServerPort;
		//			password = connectionSettings.Password;
		//		}

		//		var node = new Node();
		//		node.ProtocolError += Client_ProtocolError;
		//		node.ConnectionStatusChanged += Client_ConnectionStatusChanged;

		//		var rootBlock = new TextBlock();
		//		if( !string.IsNullOrEmpty( verificationCode ) )
		//			rootBlock.SetAttribute( "VerificationCode", verificationCode );
		//		if( !string.IsNullOrEmpty( password ) )
		//			rootBlock.SetAttribute( "Password", password );

		//		if( !node.BeginConnect( serverAddress, serverPort, EngineInfo.Version, rootBlock.DumpToString(), 100, out error ) )
		//		{
		//			node.Dispose();
		//			node = null;
		//			return false;
		//		}

		//		connectionNode = node;
		//		return true;
		//	}
		//	catch( Exception e )
		//	{
		//		error = e.Message;
		//		return false;
		//	}
		//}


		///////////////////////////////////////////////
		//SaveString, LoadString

		static SaveStringResult Convert( ClientNetworkService_CloudFunctions.SaveStringResult value )
		{
			return new SaveStringResult { Error = value.Error };
		}

		static LoadStringsResult Convert( ClientNetworkService_CloudFunctions.LoadStringResult value )
		{
			return new LoadStringsResult { Error = value.Error, Values = value.Values };
		}

		public async Task<SaveStringResult> SaveStringsAsync( string[] keys, string[] values, CancellationToken cancellationToken = default )
		{
			return Convert( await ConnectionNode.CloudFunctions.SaveStringsAsync( keys, values, cancellationToken ) );
		}

		public async Task<SaveStringResult> SaveStringsAsync( ICollection<(string, string)> pairs, CancellationToken cancellationToken = default )
		{
			var keys = new string[ pairs.Count ];
			var values = new string[ pairs.Count ];
			var counter = 0;
			foreach( var pair in pairs )
			{
				keys[ counter ] = pair.Item1;
				values[ counter ] = pair.Item2;
				counter++;
			}

			return Convert( await ConnectionNode.CloudFunctions.SaveStringsAsync( keys, values, cancellationToken ) );
		}

		public async Task<SaveStringResult> SaveStringAsync( string key, string value, CancellationToken cancellationToken = default )
		{
			return Convert( await ConnectionNode.CloudFunctions.SaveStringsAsync( new string[] { key }, new string[] { value }, cancellationToken ) );
		}

		public async Task<LoadStringsResult> LoadStringsAsync( string[] keys, CancellationToken cancellationToken = default )
		{
			return Convert( await ConnectionNode.CloudFunctions.LoadStringsAsync( keys, cancellationToken ) );
		}

		public async Task<LoadStringResult> LoadStringAsync( string key, CancellationToken cancellationToken = default )
		{
			var result = await ConnectionNode.CloudFunctions.LoadStringsAsync( new string[] { key }, cancellationToken );
			if( string.IsNullOrEmpty( result.Error ) )
				return new LoadStringResult { Value = result.Values[ 0 ] };
			else
				return new LoadStringResult { Error = result.Error };
		}


		///////////////////////////////////////////////
		//CallMethod

		static CallMethodResult<T> Convert<T>( ClientNetworkService_CloudFunctions.CallMethodResult<T> value )
		{
			return new CallMethodResult<T> { Error = value.Error, Value = value.Value };
		}

		public async Task<CallMethodResult<T>> CallMethodAsync<T>( string className, string methodName, object[] parameters, CancellationToken cancellationToken = default )
		{
			return Convert( await ConnectionNode.CloudFunctions.CallMethodAsync<T>( className, methodName, parameters, cancellationToken ) );
		}

		public async Task<CallMethodResult<T>> CallMethodAsync<T>( string className, string methodName, params object[] parameters )
		{
			return Convert( await ConnectionNode.CloudFunctions.CallMethodAsync<T>( className, methodName, parameters ) );
		}


		///////////////////////////////////////////////
		//SaveFile, LoadFile

		//!!!!how much downloaded can be calculated by comparing already downloaded files with LoadFilesInfoResult.
		//or add progress indicator

		//!!!!Save

		//!!!!MaxBytesPerSecond

		//public async Task<LoadFilesInfoResult> LoadFilesInfoAsync( string[] sourceFilePaths, string anyData = null, CancellationToken cancellationToken = default )
		//{
		//	return Convert( await ConnectionNode.CloudFunctions.LoadFilesInfoAsync( sourceFilePaths, anyData, cancellationToken ) );
		//}

		//public async Task<LoadFilesResult> LoadFilesAsync( string[] sourceFilePaths, string[] destinationFullPaths, long maxBytesPerSecond = 0, string anyData = null, CancellationToken cancellationToken = default )
		//{
		//	return Convert( await ConnectionNode.CloudFunctions.LoadFilesAsync( sourceFilePaths, destinationFullPaths, maxBytesPerSecond, anyData, cancellationToken ) );
		//}

		//public async Task<LoadFilesResult> LoadFileAsync( string sourceFilePath, string destinationFullPath, long maxBytesPerSecond = 0, string anyData = null, CancellationToken cancellationToken = default )
		//{
		//	return await LoadFilesAsync( new string[] { sourceFilePath }, new string[] { destinationFullPath }, maxBytesPerSecond, anyData, cancellationToken );
		//}
	}
}
