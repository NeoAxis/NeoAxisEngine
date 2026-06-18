// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace NeoAxis.Networking
{
	/// <summary>
	/// A basic client to access cloud functions service.
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
			ClientNetworkService_Users users;
			//ClientNetworkService_Chat chat;

			//

			public CloudFunctionsNode()
			{
				cloudFunctions = new ClientNetworkService_CloudFunctions();
				RegisterService( cloudFunctions );

				users = new ClientNetworkService_Users();
				RegisterService( users );

				//chat = new ClientNetworkService_Chat( users );
				//RegisterService( chat );
			}

			public ClientNetworkService_CloudFunctions CloudFunctions
			{
				get { return cloudFunctions; }
			}

			public ClientNetworkService_Users Users
			{
				get { return users; }
			}

			//public ClientNetworkService_Chat Chat
			//{
			//	get { return chat; }
			//}
		}

		///////////////////////////////////////////////

		public class CreateResult
		{
			public CloudFunctionsClient Client;
			public string Error;
		}

		///////////////////////////////////////////////

		static CloudFunctionsClient()
		{
			instances = new List<CloudFunctionsClient>();
		}

		public CloudFunctionsClient()
		{
			ServiceName = "CloudFunctions";
		}

		public static CloudFunctionsClient[] GetInstances()
		{
			lock( instances )
				return instances.ToArray();
		}

		public static CloudFunctionsClient FirstInstance
		{
			get { return firstInstance; }
		}

		public static async Task<CreateResult> CreateAsync( ConnectionSettingsClass connectionSettings, bool connect, CancellationToken cancellationToken = default )
		{
			var instance = (CloudFunctionsClient)connectionSettings.PreCreatedInstance;
			if( instance == null )
				instance = new CloudFunctionsClient();
			instance.ConnectionSettings = connectionSettings;

			lock( instances )
			{
				instances.Add( instance );
				firstInstance = instances.Count > 0 ? instances[ 0 ] : null;
			}

			if( connect )
			{
				var error = await instance.ReconnectAsync( cancellationToken );
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

		protected override void OnUpdate( DateTime utcNow )
		{
			base.OnUpdate( utcNow );
		}

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

		//public static void DestroyAll()
		//{
		//	foreach( var instance in GetInstances() )
		//		instance.Destroy();
		//}

		///////////////////////////////////////////////
		//SaveStrings, LoadStrings

		public async Task<ClientNetworkService_CloudFunctions.SaveStringsResult> SaveStringsAsync( string[] keys, string[] values, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.SaveStringsAsync( keys, values, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.SaveStringsResult> SaveStringsAsync( ICollection<(string, string)> pairs, CancellationToken cancellationToken = default )
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
			return await ConnectionNode.CloudFunctions.SaveStringsAsync( keys, values, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.SaveStringsResult> SaveStringAsync( string key, string value, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.SaveStringsAsync( new string[] { key }, new string[] { value }, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.LoadStringsResult> LoadStringsAsync( string[] keys, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.LoadStringsAsync( keys, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.LoadStringResult> LoadStringAsync( string key, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.LoadStringAsync( key, cancellationToken );
		}

		///////////////////////////////////////////////
		//GetCallMethodInfo

		public async Task<ClientNetworkService_CloudFunctions.GetCloudMethodInfoResult> GetCallMethodInfoAsync( string className, string methodName, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.GetCloudMethodInfoAsync( className, methodName, cancellationToken );
		}

		///////////////////////////////////////////////
		//GetCallMethods

		public async Task<ClientNetworkService_CloudFunctions.GetCloudMethodsResult> GetCallMethodsAsync( bool commandsOnly, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.GetCloudMethodsAsync( commandsOnly, cancellationToken );
		}

		///////////////////////////////////////////////
		//CallMethod

		//with return value

		public async Task<ClientNetworkService_CloudFunctions.CallMethodResult<T>> CallMethodAsync<T>( ClientNetworkService_CloudFunctions.CloudMethodInfo method, CancellationToken cancellationToken, params object[] parameters )
		{
			return await ConnectionNode.CloudFunctions.CallMethodAsync<T>( method, cancellationToken, parameters );
		}

		public async Task<ClientNetworkService_CloudFunctions.CallMethodResult<T>> CallMethodAsync<T>( string className, string methodName, CancellationToken cancellationToken, params object[] parameters )
		{
			return await ConnectionNode.CloudFunctions.CallMethodAsync<T>( className, methodName, cancellationToken, parameters );
		}

		///// <summary>
		///// Call method with default cancellation token specified in CallMethodDefaultCancellationTokenSource.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="method"></param>
		///// <param name="parameters"></param>
		///// <returns></returns>
		//public async Task<ClientNetworkService_CloudFunctions.CallMethodResult<T>> CallMethodAsync<T>( ClientNetworkService_CloudFunctions.CloudMethodInfo method, params object[] parameters )
		//{
		//	return await ConnectionNode.CloudFunctions.CallMethodAsync<T>( method, parameters );
		//}

		//without return value

		///// <summary>
		///// Call method with default cancellation token specified in CallMethodDefaultCancellationTokenSource.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="method"></param>
		///// <param name="parameters"></param>
		///// <returns></returns>
		//public async Task<ClientNetworkService_CloudFunctions.CallMethodResult<T>> CallMethodAsync<T>( string className, string methodName, params object[] parameters )
		//{
		//	return await ConnectionNode.CloudFunctions.CallMethodAsync<T>( className, methodName, parameters );
		//}

		public async Task<ClientNetworkService_CloudFunctions.CallMethodResultNoValue> CallMethodAsync( ClientNetworkService_CloudFunctions.CloudMethodInfo method, CancellationToken cancellationToken, params object[] parameters )
		{
			return await ConnectionNode.CloudFunctions.CallMethodAsync( method, cancellationToken, parameters );
		}

		public async Task<ClientNetworkService_CloudFunctions.CallMethodResultNoValue> CallMethodAsync( string className, string methodName, CancellationToken cancellationToken, params object[] parameters )
		{
			return await ConnectionNode.CloudFunctions.CallMethodAsync( className, methodName, cancellationToken, parameters );
		}

		///// <summary>
		///// Call method with default cancellation token specified in CallMethodDefaultCancellationTokenSource.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="method"></param>
		///// <param name="parameters"></param>
		///// <returns></returns>
		//public async Task<ClientNetworkService_CloudFunctions.CallMethodResultNoValue> CallMethodAsync( ClientNetworkService_CloudFunctions.CloudMethodInfo method, params object[] parameters )
		//{
		//	return await ConnectionNode.CloudFunctions.CallMethodAsync( method, parameters );
		//}

		///// <summary>
		///// Call method with default cancellation token specified in CallMethodDefaultCancellationTokenSource.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="method"></param>
		///// <param name="parameters"></param>
		///// <returns></returns>
		//public async Task<ClientNetworkService_CloudFunctions.CallMethodResultNoValue> CallMethodAsync( string className, string methodName, params object[] parameters )
		//{
		//	return await ConnectionNode.CloudFunctions.CallMethodAsync( className, methodName, parameters );
		//}

		///////////////////////////////////////////////
		//GetFilesInfo, GetFileInfo, GetDirectoryInfo
		//DownloadFiles, DownloadFile, DownloadDirectory, DownloadObjectsAsync
		//UploadFiles, UploadFile, CreateDirectory, UploadObjectsAsync
		//DeleteFiles, DeleteFile, DeleteDirectory, DeleteObjectsAsync

		public async Task<ClientNetworkService_CloudFunctions.GetFilesInfoResult> GetFilesInfoAsync( ClientNetworkService_CloudFunctions.DataSource source, string[] filePaths, string anyData = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.GetFilesInfoAsync( source, filePaths, anyData, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.GetFileInfoResult> GetFileInfoAsync( ClientNetworkService_CloudFunctions.DataSource source, string filePath, string anyData = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.GetFileInfoAsync( source, filePath, anyData, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.GetDirectoryInfoResult> GetDirectoryInfoAsync( ClientNetworkService_CloudFunctions.DataSource source, string directoryPath, string searchPattern, SearchOption searchOption, string anyData = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.GetDirectoryInfoAsync( source, directoryPath, searchPattern, searchOption, anyData, cancellationToken );
		}

		///////////////////////////////////////////////

		public async Task<ClientNetworkService_CloudFunctions.DownloadFilesResult> DownloadFilesAsync( ClientNetworkService_CloudFunctions.DataSource source, string[] sourceFilePaths, string[] targetFullPaths, bool skipDownloadIfUpToDate, string anyData = null, ClientNetworkService_CloudFunctions.DownloadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.DownloadFilesAsync( source, sourceFilePaths, targetFullPaths, skipDownloadIfUpToDate, anyData, progressCallback, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.DownloadFilesResult> DownloadFileAsync( ClientNetworkService_CloudFunctions.DataSource source, string sourceFilePath, string targetFullPath, bool skipDownloadIfUpToDate, string anyData = null, ClientNetworkService_CloudFunctions.DownloadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.DownloadFileAsync( source, sourceFilePath, targetFullPath, skipDownloadIfUpToDate, anyData, progressCallback, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.DownloadDirectoryResult> DownloadDirectoryAsync( ClientNetworkService_CloudFunctions.DataSource source, string sourceDirectoryPath, string targetFullPath, string searchPattern, SearchOption searchOption, bool skipDownloadIfUpToDate, bool deleteExcessEntries, string anyData = null, ClientNetworkService_CloudFunctions.DownloadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.DownloadDirectoryAsync( source, sourceDirectoryPath, targetFullPath, searchPattern, searchOption, skipDownloadIfUpToDate, deleteExcessEntries, anyData, progressCallback, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.DownloadDirectoryResult> DownloadObjectsAsync( ClientNetworkService_CloudFunctions.DataSource source, ClientNetworkService_CloudFunctions.DownloadObjectsItem[] objects, string[] targetFullPaths, bool skipDownloadIfUpToDate, bool deleteExcessEntries, string anyData = null, ClientNetworkService_CloudFunctions.DownloadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.DownloadObjectsAsync( source, objects, targetFullPaths, skipDownloadIfUpToDate, deleteExcessEntries, anyData, progressCallback, cancellationToken );
		}

		///////////////////////////////////////////////

		public async Task<ClientNetworkService_CloudFunctions.SimpleResult> UploadFilesAsync( ClientNetworkService_CloudFunctions.DataSource source, string[] sourceFullPaths, string[] targetFilePaths, string anyData = null, ClientNetworkService_CloudFunctions.UploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.UploadFilesAsync( source, sourceFullPaths, targetFilePaths, anyData, progressCallback, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.SimpleResult> UploadFileAsync( ClientNetworkService_CloudFunctions.DataSource source, string sourceFullPath, string targetFilePath, string anyData = null, ClientNetworkService_CloudFunctions.UploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.UploadFileAsync( source, sourceFullPath, targetFilePath, anyData, progressCallback, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.SimpleResult> UploadDirectoryAsync( ClientNetworkService_CloudFunctions.DataSource source, string sourceFullPath, string targetDirectoryName, SearchOption searchOption, string anyData = null, ClientNetworkService_CloudFunctions.UploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.UploadDirectoryAsync( source, sourceFullPath, targetDirectoryName, searchOption, anyData, progressCallback, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.SimpleResult> CreateDirectoryAsync( ClientNetworkService_CloudFunctions.DataSource source, string directoryPath, string anyData = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.CreateDirectoryAsync( source, directoryPath, anyData, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.SimpleResult> UploadObjectsAsync( ClientNetworkService_CloudFunctions.DataSource source, string[] sourceFullPaths, string[] targetFilePaths, SearchOption searchOption, string anyData = null, ClientNetworkService_CloudFunctions.UploadFilesProgressCallback progressCallback = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.UploadObjectsAsync( source, sourceFullPaths, targetFilePaths, searchOption, anyData, progressCallback, cancellationToken );
		}

		///////////////////////////////////////////////

		public async Task<ClientNetworkService_CloudFunctions.SimpleResult> DeleteFilesAsync( ClientNetworkService_CloudFunctions.DataSource source, string[] filePaths, string anyData = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.DeleteFilesAsync( source, filePaths, anyData, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.SimpleResult> DeleteFileAsync( ClientNetworkService_CloudFunctions.DataSource source, string filePath, string anyData = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.DeleteFileAsync( source, filePath, anyData, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.SimpleResult> DeleteDirectoryAsync( ClientNetworkService_CloudFunctions.DataSource source, string directoryPath, bool recursive, bool clear, string anyData = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.DeleteDirectoryAsync( source, directoryPath, recursive, clear, anyData, cancellationToken );
		}

		public async Task<ClientNetworkService_CloudFunctions.SimpleResult> DeleteObjectsAsync( ClientNetworkService_CloudFunctions.DataSource source, ClientNetworkService_CloudFunctions.DeleteObjectsItem[] objects, string anyData = null, CancellationToken cancellationToken = default )
		{
			return await ConnectionNode.CloudFunctions.DeleteObjectsAsync( source, objects, anyData, cancellationToken );
		}
	}
}
