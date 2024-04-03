// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if CLOUD
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NeoAxis.Networking
{
	public class CloudProjectEnteringClientNode : NetworkClientNode
	{
		//services
		ClientNetworkService_Messages messages;
		ClientNetworkService_FileSync fileSync;

		//

		public CloudProjectEnteringClientNode()
		{
			//register messages service
			messages = new ClientNetworkService_Messages();
			RegisterService( messages );

			//register file sync service
			fileSync = new ClientNetworkService_FileSync();
			RegisterService( fileSync );
		}

		public ClientNetworkService_Messages Messages
		{
			get { return messages; }
		}

		public ClientNetworkService_FileSync FileSync
		{
			get { return fileSync; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class CloudProjectEnteringClient
	{
		static bool Edit;
		static bool EnableFileSync;
		static FileSyncBeforeUpdateFilesDelegate FileSyncBeforeUpdateFilesCallback;
		static CloudProjectEnteringClientNode client;
		static string resultDataFromServer = "";
		static volatile ResultClass Result;
		static volatile bool cancelEntering;

		/////////////////////////////////////////

		public class ResultClass
		{
			public string Data;
			public string Error;

			public ResultClass( string data, string error )
			{
				Data = data;
				Error = error;
			}
		}

		/////////////////////////////////////////

		static bool ConnectToServer( string serverAddress, long projectID, string verificationCode, out string error )
		{
			error = "";

			DestroyClient();

			client = new CloudProjectEnteringClientNode();
			client.ProtocolError += Client_ProtocolError;
			client.ConnectionStatusChanged += Client_ConnectionStatusChanged;
			client.Messages.ReceiveMessage += Messages_ReceiveMessage;
			client.FileSync.StatusChanged += FileSync_StatusChanged;
			client.FileSync.BeforeUpdateFiles += FileSync_BeforeUpdateFiles;

			client.FileSync.DestinationFolder = CloudProjectCommon.GetAppProjectFolder( projectID, Edit );

			//client.FileSync.LocalFilesFolder = "";


			var block = new TextBlock();
			block.SetAttribute( "Project", projectID.ToString() );
			block.SetAttribute( "Edit", Edit.ToString() );
			block.SetAttribute( "VerificationCode", verificationCode );

			if( !client.BeginConnect( serverAddress, NetworkCommonSettings.ProjectEnteringPort, EngineInfo.Version, block.DumpToString(), 100, out error ) )
			{
				client.Dispose();
				client = null;
				return false;
			}

			return true;
		}

		static private void Client_ProtocolError( NetworkClientNode sender, string message )
		{
			Result = new ResultClass( "", "Protocol error. " + message );
		}

		static void DestroyClient()
		{
			if( client != null )
			{
				client.ProtocolError -= Client_ProtocolError;
				client.ConnectionStatusChanged -= Client_ConnectionStatusChanged;
				client.Messages.ReceiveMessage -= Messages_ReceiveMessage;
				client.FileSync.StatusChanged -= FileSync_StatusChanged;
				client.FileSync.BeforeUpdateFiles -= FileSync_BeforeUpdateFiles;

				client.Dispose();
				client = null;
			}

			resultDataFromServer = "";
		}

		public static void AppDestroy()
		{
			DestroyClient();
		}

		public static void Update()
		{
			client?.Update();
		}

		static void Client_ConnectionStatusChanged( NetworkClientNode sender, NetworkStatus status )
		{
			switch( status )
			{
			case NetworkStatus.Disconnected:
				if( Result == null && !string.IsNullOrEmpty( sender.DisconnectionReason ) )
					Result = new ResultClass( "", sender.DisconnectionReason );
				break;
			}
		}

		static private void Messages_ReceiveMessage( ClientNetworkService_Messages sender, string message, string data )
		{
			if( message == "Verified" )
			{
				resultDataFromServer = data;

				if( EnableFileSync )
				{
					if( client != null )
						client.FileSync.StartUpdate();
				}
				else
				{
					//done, now can start app
					Result = new ResultClass( resultDataFromServer, "" );
					client?.Dispose();
				}
			}
		}

		private static void FileSync_StatusChanged( ClientNetworkService_FileSync sender )
		{
			switch( sender.Status.Status )
			{
			case ClientNetworkService_FileSync.StatusEnum.Success:
				{
					//done, now can start app
					Result = new ResultClass( resultDataFromServer, "" );
					client.Dispose();
				}
				break;

			case ClientNetworkService_FileSync.StatusEnum.Error:
				Result = new ResultClass( "", sender.Status.Error );
				break;
			}
		}

		private static void FileSync_BeforeUpdateFiles( ClientNetworkService_FileSync sender, Dictionary<string, ClientNetworkService_FileSync.FileInfo> filesOnServer, ref string[] filesToDownload, ref string[] filesToDelete, ref bool cancel )
		{
			FileSyncBeforeUpdateFilesCallback?.Invoke( sender, filesOnServer, ref filesToDownload, ref filesToDelete, ref cancel );
		}

		public delegate void BeginEnteringAsyncStatusMessageDelegate( string statusMessage );
		public delegate void FileSyncBeforeUpdateFilesDelegate( ClientNetworkService_FileSync sender, Dictionary<string, ClientNetworkService_FileSync.FileInfo> filesOnServer, ref string[] filesToDownload, ref string[] filesToDelete, ref bool cancel );

		public static async Task<ResultClass> BeginEnteringAsync( string serverAddress, long projectID, string verificationCode, bool edit, bool enableFileSync, BeginEnteringAsyncStatusMessageDelegate statusMessageCallback, FileSyncBeforeUpdateFilesDelegate fileSyncBeforeUpdateFilesCallback )
		{
			Edit = edit;
			EnableFileSync = enableFileSync;
			FileSyncBeforeUpdateFilesCallback = fileSyncBeforeUpdateFilesCallback;
			Result = null;
			cancelEntering = false;

			statusMessageCallback?.Invoke( "Entering" );

			//create folder for project files
			try
			{
				var projectFolder = CloudProjectCommon.GetAppProjectFolder( projectID, edit );
				if( !Directory.Exists( projectFolder ) )
					Directory.CreateDirectory( projectFolder );
			}
			catch( Exception e )
			{
				return new ResultClass( "", e.Message );
			}


			//connect to server
			if( !ConnectToServer( serverAddress, projectID, verificationCode, out var error ) )
				return new ResultClass( "", error );

			while( Result == null && !cancelEntering )
			{
				await Task.Delay( 10 );

				if( client != null && client.FileSync.RemainsToUpdate != 0 )
				{
					var remainsToUpdate = client.FileSync.RemainsToUpdate / 1024 / 1024;
					statusMessageCallback?.Invoke( $"It remains to update {remainsToUpdate} megabytes" );
				}
			}

			//!!!!где еще такое вызывать
			DestroyClient();

			return cancelEntering ? null : Result;
		}

		public static void CancelEntering()
		{
			cancelEntering = true;
		}
	}
}
#endif