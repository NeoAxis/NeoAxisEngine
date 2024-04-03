#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis;
using NeoAxis.Networking;

namespace NeoAxis.Editor
{
	class EditorClientNode : NetworkClientNode
	{
		//services
		ClientNetworkService_Messages messages;
		ClientNetworkService_Users users;
		ClientNetworkService_Chat chat;
		ClientNetworkService_Components components;
		ClientNetworkService_FileSync fileSync;

		//

		public EditorClientNode()
		{
			//register messages service
			messages = new ClientNetworkService_Messages();
			RegisterService( messages );

			//register users service
			users = new ClientNetworkService_Users();
			RegisterService( users );

			//register chat service
			chat = new ClientNetworkService_Chat( users );
			RegisterService( chat );

			//register components service
			components = new ClientNetworkService_Components( users );
			RegisterService( components );

			fileSync = new ClientNetworkService_FileSync();
			RegisterService( fileSync );
		}

		public ClientNetworkService_Messages Messages
		{
			get { return messages; }
		}

		public ClientNetworkService_Users Users
		{
			get { return users; }
		}

		public ClientNetworkService_Chat Chat
		{
			get { return chat; }
		}

		public ClientNetworkService_Components Components
		{
			get { return components; }
		}

		public ClientNetworkService_FileSync FileSync
		{
			get { return fileSync; }
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class EditorClientAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			if( EngineApp.IsEditor )
			{
				EngineApp.AppCreateAfter += delegate ()
				{
					EditorClient.Init();
				};
			}
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The class for general management of the client.
	/// </summary>
	static class EditorClient
	{
		static bool initialized;
		static NetworkModeEnum networkMode;
		static string serverAddress = "";
		static int serverPort;

		static EditorClientNode client;
		//!!!!
		static string errorScreenMessage = "";

		/////////////////////////////////////////

		public enum NetworkModeEnum
		{
			CloudProject,
			Simple,
		}

		/////////////////////////////////////////

		public static bool Initialized
		{
			get { return initialized; }
		}

		static bool GetInitSettings( out string verificationCode )
		{
			verificationCode = "";

			if( SystemSettings.CommandLineParameters.TryGetValue( "-editorClient", out var parameter ) )
			{
				//run editor from Launcher

				//get network mode
				if( SystemSettings.CommandLineParameters.TryGetValue( "-networkMode", out var networkModeStr ) )
				{
					if( !Enum.TryParse( networkModeStr, out networkMode ) )
					{
						Log.Fatal( "EditorClient: Init: '-networkMode' unknown mode." );
						return false;
					}
				}

				//get serverAddress
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-serverAddress", out serverAddress ) )
				{
					Log.Fatal( "EditorClient: Init: '-serverAddress' is not specified." );
					return false;
				}

				//get serverPort
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-serverPort", out var serverPortStr ) )
				{
					Log.Fatal( "EditorClient: Init: '-serverPort' is not specified." );
					return false;
				}
				if( !int.TryParse( serverPortStr, out serverPort ) )
				{
					Log.Fatal( "EditorClient: Init: '-serverPort' invalid data." );
					return false;
				}

				//get verificationCode
				if( networkMode == NetworkModeEnum.CloudProject )
				{
					if( !SystemSettings.CommandLineParameters.TryGetValue( "-verificationCode", out verificationCode ) )
					{
						Log.Fatal( "EditorClient: Init: '-verificationCode' is not specified." );
						return false;
					}
				}

				return true;
			}
			else
			{
				//run editor directly NeoAxis.Editor.exe

				//use CloudProject.info
				if( EngineInfo.CloudProjectInfo != null )
				{
					var projectID = EngineInfo.CloudProjectInfo.ID;


					//!!!!

					//zzzzzz;//случайно не поломать C:\Dev


					networkMode = NetworkModeEnum.CloudProject;


					//static string serverAddress = "";
					//static int serverPort;


					//!!!!как-то показывать что коннектимся. возможность отменить


					//!!!!дожидаться async



					////update project files, get info to connect
					//var additionalDataFromServer = "";
					//{
					//	//request verification code from general manager to entering server manager						
					//	var requestCodeResult = await GeneralManagerFunctions.RequestVerificationCodeToEnterProjectAsync( projectID, "Edit" );
					//	if( !string.IsNullOrEmpty( requestCodeResult.Error ) )
					//	{
					//		ScreenMessages.Add( "Error:" + requestCodeResult.Error );
					//		return;
					//	}


					//	//connect to server manager, update files
					//	var block = requestCodeResult.Data;
					//	var code = block.GetAttribute( "Code" );
					//	var serverAddress = block.GetAttribute( "ServerAddress" );
					//	var enteringResult = await ProjectEnteringClientManager.BeginEnteringAsync( serverAddress, projectID, code, true, false, delegate ( string statusMessage )
					//	{
					//		enteringMessageBoxWindow.Window.MessageText = statusMessage + "...";
					//	}, null );

					//	if( enteringResult == null || !string.IsNullOrEmpty( enteringResult.Error ) )
					//	{
					//		if( enteringResult != null )
					//			ScreenMessages.Add( "Error:" + enteringResult.Error );
					//		return;
					//	}

					//	additionalDataFromServer = enteringResult.Data;
					//}

					//zzzzz;

					////parse additional data from server
					//int serverPort;
					//{
					//	var block = TextBlock.Parse( additionalDataFromServer, out var error );
					//	if( block == null )
					//	{
					//		ScreenMessages.Add( "Error:Unable to parse additional data from server. " + error );
					//		return;
					//	}

					//	serverPort = int.Parse( block.GetAttribute( "ServerPort", "0" ) );
					//}

				}
			}

			return false;
		}

		public static void Init()
		{
			if( !GetInitSettings( out var verificationCode ) )
				return;

			initialized = true;

			EngineApp.AppDestroy += EngineApp_AppDestroy;
			//SimulationApp.MainViewportRenderUI += SimulationApp_MainViewportRenderUI;


			//!!!!связь может прерываться, может не сразу подключиться. постоянный "Connecting..." как в ланчере


			if( !ConnectToServer( verificationCode, out var error ) )
			{
				//!!!!
				errorScreenMessage = error;
				return;
			}

			EngineApp.Tick += EngineApp_Tick;
		}

		static bool ConnectToServer( string verificationCode, out string error )
		{
			error = "";

			DestroyClient();

			client = new EditorClientNode();
			client.ProtocolError += Client_ProtocolError;
			client.ConnectionStatusChanged += Client_ConnectionStatusChanged;
			client.Messages.ReceiveMessage += Messages_ReceiveMessage;
			client.Components.SceneCreateBegin += Components_SceneCreateBegin;
			client.Components.SceneCreateEnd += Components_SceneCreateEnd;
			client.Components.SceneDestroy += Components_SceneDestroy;

			var block = new TextBlock();
			block.SetAttribute( "VerificationCode", verificationCode );
			var loginData = block.DumpToString();

			//!!!!connectionTimeout
			if( !client.BeginConnect( serverAddress, serverPort, EngineInfo.Version, loginData, 100, out error ) )
			{
				client.Dispose();
				client = null;
				return false;
			}

			return true;
		}

		private static void Client_ProtocolError( NetworkClientNode sender, string message )
		{
			Log.Warning( "EditorClient: Protocol error: " + message );
		}

		static void DestroyClient()
		{
			if( client != null )
			{
				client.ProtocolError -= Client_ProtocolError;
				client.ConnectionStatusChanged -= Client_ConnectionStatusChanged;
				client.Messages.ReceiveMessage -= Messages_ReceiveMessage;
				client.Components.SceneCreateBegin -= Components_SceneCreateBegin;
				client.Components.SceneCreateEnd -= Components_SceneCreateEnd;
				client.Components.SceneDestroy -= Components_SceneDestroy;

				client.Dispose();
				client = null;
			}
		}
		private static void EngineApp_AppDestroy()
		{
			DestroyClient();
		}

		private static void EngineApp_Tick( float delta )
		{
			client?.Update();
		}

		//private static void SimulationApp_MainViewportRenderUI()
		//{
		//	if( Initialized )
		//	{
		//		var viewport = SimulationApp.MainViewport;

		//		var lines = new List<string>();

		//		lines.Add( "Project client manager is initialized." );
		//		if( client != null )
		//		{
		//			lines.Add( "Project client is created." );
		//			lines.Add( "Connection status: " + client.Status.ToString() );

		//			var fileSyncStatus = client.FileSync.Status;
		//			lines.Add( "File sync status: " + fileSyncStatus.Status.ToString() );
		//			if( fileSyncStatus.Status == ClientNetworkService_FileSync.StatusEnum.Error )
		//				lines.Add( "File sync error: " + fileSyncStatus.Error );
		//		}

		//		if( !string.IsNullOrEmpty( errorScreenMessage ) )
		//		{
		//			lines.Add( "" );
		//			lines.Add( "Last error: " + errorScreenMessage );
		//		}

		//		CanvasRendererUtility.AddTextLinesWithShadow( viewport, lines, new Rectangle( 0.02, 0.02, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
		//	}
		//}

		static void Client_ConnectionStatusChanged( NetworkClientNode sender, NetworkStatus status )
		{
			//ScreenMessages.Add( string.Format( "Connection status changed: {0}", status.ToString() ) );

			switch( status )
			{
			case NetworkStatus.Connected:
				{

					//!!!!не обновлять если это dev
					//!!!!это sync в процессе работы
					//client.FileSync.StartUpdate();

				}
				break;

			case NetworkStatus.Connecting:
				break;

			case NetworkStatus.Disconnected:
				break;
			}
		}

		private static void Messages_ReceiveMessage( ClientNetworkService_Messages sender, string message, string data )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", message ) );

			if( message == "Verified" )
			{
			}
		}

		private static void Components_SceneCreateBegin( ClientNetworkService_Components sender, string sceneInfo )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", "SceneCreateBegin" ) );
		}

		private static void Components_SceneCreateEnd( ClientNetworkService_Components sender )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", "SceneCreateEnd" ) );
		}

		private static void Components_SceneDestroy( ClientNetworkService_Components sender )
		{
			//ScreenMessages.Add( string.Format( "Message from server: {0}", "SceneDestroy" ) );
		}

	}
}

#endif
#endif