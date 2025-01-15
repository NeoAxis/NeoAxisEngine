#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NeoAxis.Networking;

namespace NeoAxis.Editor
{
	class CloudProjectCommitClientNode : ClientNode
	{
		//services
		ClientNetworkService_Messages messages;
		ClientNetworkService_Commit commit;

		//

		public CloudProjectCommitClientNode()
		{
			//register messages service
			messages = new ClientNetworkService_Messages();
			RegisterService( messages );

			//register messages service
			commit = new ClientNetworkService_Commit();
			RegisterService( commit );
		}

		public ClientNetworkService_Messages Messages
		{
			get { return messages; }
		}

		public ClientNetworkService_Commit Commit
		{
			get { return commit; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class CloudProjectCommitClient
	{
		static CloudProjectCommitClientNode client;
		//static string resultDataFromServer = "";
		static volatile ResultClass Result;
		static volatile bool cancelCommit;

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

		static bool ConnectToServer( string serverAddress, long projectID, string verificationCode, ICollection<ClientNetworkService_Commit.FileItem> filesToCommit, out string error )
		{
			error = "";

			DestroyClient();

			client = new CloudProjectCommitClientNode();
			client.ProtocolError += Client_ProtocolError;
			client.ConnectionStatusChanged += Client_ConnectionStatusChanged;
			client.Messages.ReceiveMessageString += Messages_ReceiveMessage;
			client.Commit.StatusChanged += Commit_StatusChanged;

			client.Commit.SetFilesToCommit( filesToCommit );

			var block = new TextBlock();
			block.SetAttribute( "Project", projectID.ToString() );
			block.SetAttribute( "VerificationCode", verificationCode );

			if( !client.BeginConnect( serverAddress, NetworkCommonSettings.ProjectCommitPort, EngineInfo.Version, block.DumpToString(), 30, out error ) )
			{
				client.Dispose();
				client = null;
				return false;
			}

			return true;
		}

		static private void Client_ProtocolError( ClientNode sender, string message )
		{
			Result = new ResultClass( "", "Protocol error. " + message );
		}

		static void DestroyClient()
		{
			if( client != null )
			{
				client.ProtocolError -= Client_ProtocolError;
				client.ConnectionStatusChanged -= Client_ConnectionStatusChanged;
				client.Messages.ReceiveMessageString -= Messages_ReceiveMessage;
				client.Commit.StatusChanged -= Commit_StatusChanged;

				client.Dispose();
				client = null;
			}
		}

		public static void AppDestroy()
		{
			DestroyClient();
		}

		public static void Update()
		{
			client?.Update();
		}

		private static void Commit_StatusChanged( ClientNetworkService_Commit sender )
		{
			switch( sender.Status.Status )
			{
			case ClientNetworkService_Commit.StatusEnum.Success:
				{
					//done, now can start app
					Result = new ResultClass( ""/*resultDataFromServer*/, "" );
					client.Dispose();
				}
				break;

			case ClientNetworkService_Commit.StatusEnum.Error:
				Result = new ResultClass( "", sender.Status.Error );
				break;
			}
		}

		static void Client_ConnectionStatusChanged( ClientNode sender )//, NetworkStatus status )
		{
			switch( sender.Status )
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
				client?.Commit.BeginUpload();
		}

		public delegate void BeginCommitAsyncStatusMessageDelegate( string statusMessage );

		public static async Task<ResultClass> BeginCommitAsync( string serverAddress, long projectID, string verificationCode, ICollection<ClientNetworkService_Commit.FileItem> filesToCommit, BeginCommitAsyncStatusMessageDelegate statusMessageCallback )
		{
			Result = null;
			cancelCommit = false;

			statusMessageCallback?.Invoke( "Preparing" );

			//connect to server
			if( !ConnectToServer( serverAddress, projectID, verificationCode, filesToCommit, out var error ) )
				return new ResultClass( "", error );

			while( Result == null && !cancelCommit )
			{
				await Task.Delay( 10 );

				if( client != null && client.Commit.GetRemainsCount() != 0 )
				{
					var remainsToUpdate = client.Commit.GetRemainsSize() / 1024 / 1024;
					statusMessageCallback?.Invoke( $"It remains to commit {remainsToUpdate} megabytes" );
				}
			}

			//!!!!где еще такое вызывать
			DestroyClient();

			return cancelCommit ? null : Result;
		}

		public static void CancelCommit()
		{
			cancelCommit = true;
		}
	}
}

#endif
#endif