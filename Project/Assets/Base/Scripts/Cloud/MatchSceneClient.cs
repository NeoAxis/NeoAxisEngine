// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using NeoAxis;
using NeoAxis.Networking;

namespace Project
{
	/// <summary>
	/// A client class that synchronizes the match instance scene.
	/// </summary>
	public static class MatchSceneClient
	{
		static MatchSceneSyncClientNode clientNode;
		static string LastError { get; set; } = "";
		static bool subscribedToEvents;
		static string helloFromServerMessage;

		///////////////////////////////////////////////

		public class MatchSceneSyncClientNode : ClientNode
		{
			//services
			ClientNetworkService_CloudFunctions cloudFunctions;
			ClientNetworkService_Messages messages;
			ClientNetworkService_Users users;
			ClientNetworkService_Components components;

			//

			public MatchSceneSyncClientNode()
			{
				//register cloud functions service
				cloudFunctions = new ClientNetworkService_CloudFunctions();
				RegisterService( cloudFunctions );

				//register messages service
				messages = new ClientNetworkService_Messages();
				RegisterService( messages );

				//register users service
				users = new ClientNetworkService_Users();
				RegisterService( users );

				//register components service
				components = new ClientNetworkService_Components( users );
				RegisterService( components );
			}

			public ClientNetworkService_CloudFunctions CloudFunctions
			{
				get { return cloudFunctions; }
			}

			public ClientNetworkService_Messages Messages
			{
				get { return messages; }
			}

			public ClientNetworkService_Users Users
			{
				get { return users; }
			}

			public ClientNetworkService_Components Components
			{
				get { return components; }
			}
		}

		///////////////////////////////////////////////

		public static bool Created
		{
			get { return clientNode != null; }
		}

		public static MatchSceneSyncClientNode ConnectionNode
		{
			get { return clientNode; }
		}

		public static string HelloFromServerMessage
		{
			get { return helloFromServerMessage; }
		}

		static bool ConnectToServer( string serverAddress, int serverPort, string userPassword, out string error )
		{
			Destroy();

			error = "";

			if( !subscribedToEvents )
			{
				EngineApp.Tick += EngineApp_Tick;
				EngineApp.AppDestroy += EngineApp_AppDestroy;
				SimulationApp.MainViewportRenderUI += SimulationApp_MainViewportRenderUI;
				subscribedToEvents = true;
			}

			clientNode = new MatchSceneSyncClientNode();
			clientNode.ProtocolError += Client_ProtocolError;
			clientNode.ConnectionStatusChanged += Client_ConnectionStatusChanged;
			clientNode.Messages.ReceiveMessageString += Messages_ReceiveMessageString;
			clientNode.Components.SceneCreateBegin += Components_SceneCreateBegin;
			clientNode.Components.SceneCreateEnd += Components_SceneCreateEnd;
			clientNode.Components.SceneDestroy += Components_SceneDestroy;

			var block = new TextBlock();
			block.SetAttribute( "Password", userPassword );

			var loginData = block.DumpToString();

			if( !clientNode.BeginConnect( false, serverAddress, serverPort, serverPort + 1, EngineInfo.Version, loginData, out error ) )
			{
				Destroy();
				return false;
			}

			//!!!!not here?
			////configure the app
			//EngineApp.EnginePauseWhenApplicationIsNotActive = false;

			return true;
		}

		public static bool Connect( string serverAddress, int serverPort, string userPassword, out string error )
		{
			Destroy();

			if( !ConnectToServer( serverAddress, serverPort, userPassword, out error ) )
			{
				LastError = error;
				return false;
			}

			return true;
		}

		private static void Client_ProtocolError( ClientNode sender, string message )
		{
			Log.Warning( "MatchSceneClient: Protocol error: " + message );
		}

		public static void Destroy()
		{
			if( clientNode != null )
			{
				clientNode.ProtocolError -= Client_ProtocolError;
				clientNode.ConnectionStatusChanged -= Client_ConnectionStatusChanged;
				clientNode.Messages.ReceiveMessageString -= Messages_ReceiveMessageString;
				clientNode.Components.SceneCreateBegin -= Components_SceneCreateBegin;
				clientNode.Components.SceneCreateEnd -= Components_SceneCreateEnd;
				clientNode.Components.SceneDestroy -= Components_SceneDestroy;

				clientNode.Dispose();
				clientNode = null;

				//!!!!not here?
				////restore the app settings
				//EngineApp.EnginePauseWhenApplicationIsNotActive = true;
			}

			if( subscribedToEvents )
			{
				EngineApp.Tick -= EngineApp_Tick;
				EngineApp.AppDestroy -= EngineApp_AppDestroy;
				SimulationApp.MainViewportRenderUI -= SimulationApp_MainViewportRenderUI;
				subscribedToEvents = false;
			}
		}

		private static void EngineApp_AppDestroy()
		{
			Destroy();
		}

		private static void EngineApp_Tick( float delta )
		{
			var utcNow = DateTime.UtcNow;
			clientNode?.Update( utcNow );
		}

		private static void SimulationApp_MainViewportRenderUI()
		{
			if( SimulationApp.NetworkLogging )
			{
				var viewport = SimulationApp.MainViewport;

				var lines = new List<string>();

				lines.Add( "MatchSceneClient is initialized." );
				if( clientNode != null )
				{
					lines.Add( "MatchSceneClient is created." );
					lines.Add( "Connection status: " + clientNode.Status.ToString() );
				}

				if( !string.IsNullOrEmpty( LastError ) )
				{
					lines.Add( "" );
					lines.Add( "Last error: " + LastError );
				}

				CanvasRendererUtility.AddTextLinesWithShadow( viewport, lines, new Rectangle( 0.02, 0.02, 1, 1 ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );
			}
		}

		static void Client_ConnectionStatusChanged( ClientNode sender )
		{
			//ScreenMessages.Add( string.Format( "GameInstanceSyncClient status changed: {0}", sender.Status.ToString() ) );
		}

		private static void Messages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			//ScreenMessages.Add( string.Format( "MatchSceneClient: Message from server: {0}", message ) );

			if( message == "HelloFromServerMessage" )
				helloFromServerMessage = data;
			if( message == "ScreenMessagesAdd" )
				ScreenMessages.Add( data );
		}

		private static void Components_SceneCreateBegin( ClientNetworkService_Components sender, string sceneInfo )
		{
			//ScreenMessages.Add( string.Format( "MatchSceneClient: Message from server: {0}", "SceneCreateBegin" ) );
		}

		private static void Components_SceneCreateEnd( ClientNetworkService_Components sender )
		{
			//ScreenMessages.Add( string.Format( "MatchSceneClient: Message from server: {0}", "SceneCreateEnd" ) );

			//set active scene to the app, load Play screen and scene screen
			var scene = clientNode.Components.Scene;
			if( scene != null )
				SimulationApp.NetworkClientSceneCreated( scene );
		}

		private static void Components_SceneDestroy( ClientNetworkService_Components sender, bool anotherSceneWillLoaded )
		{
			//ScreenMessages.Add( string.Format( "MatchSceneClient: Message from server: {0}", "SceneDestroy" ) );

			SimulationApp.NetworkClientSceneDestroyed( anotherSceneWillLoaded );
		}

		internal static void RegisterEngineConsoleCommands()
		{
			EngineConsole.AddCommand( "StartNetworkProfiler", delegate ( string arguments )
			{
				var workingTime = float.MaxValue;
				if( !string.IsNullOrEmpty( arguments ) && float.TryParse( arguments, out var v ) )
					workingTime = v;
				ConnectionNode?.ProfilerStart( workingTime );

			}, "Starts the network profiler. Optionally, you can specify working time in seconds." );

			EngineConsole.AddCommand( "StopNetworkProfiler", delegate ( string arguments )
			{
				ConnectionNode?.ProfilerStop( true );
			}, "Stops the network profiler." );
		}
	}
}
