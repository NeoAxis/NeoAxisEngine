// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;

namespace NeoAxis.Networking
{
	/// <summary>
	/// A basic client code for services.
	/// </summary>
	public abstract class BasicServiceClient
	{
		public double UpdateFrequency { get; set; } = 1.0 / 30.0;

		static ConcurrentBag<BasicServiceClient> autoUpdateInstances = new ConcurrentBag<BasicServiceClient>();
		//static List<BasicServiceClient> instances;

		static Thread autoUpdateInstancesThread;
		//static bool autoUpdateInstancesThreadNeedExit;

		bool autoUpdate;

		string serviceName;

		ConnectionSettingsClass connectionSettings;
		BasicServiceNode connectionNode;

		string connectionErrorReceived;

		DateTime updateLastTime;

		///////////////////////////////////////////////

		public class ConnectionSettingsClass
		{
			//initial settings
			public BasicServiceClient PreCreatedInstance;
			public bool AutoUpdate = true;
			public ConnectionTypeEnum ConnectionType;
			public CloudUserRole UserRole; //Developer, Player

			//both Direct and Cloud specific
			public int ServerPort;

			//Direct specific
			public string ServerAddress;
			//public int ServerPort;
			public string Password;

			//Cloud specific
			public long ProjectID;

			//protocol settings
			public bool ConnectWebSocket = true;
			public bool ConnectUDP;

			//advanced settings
			public double ConnectingMaxTimeInSeconds = 20;
			public bool AllowReconnect;

			//public string AnyData;

			/////////////////////

			public enum ConnectionTypeEnum
			{
				/// <summary>
				/// Direct connect to the server by means password.
				/// </summary>
				Direct,

				/// <summary>
				/// Connecting with login of current user in the launcher.
				/// </summary>
				Cloud,
			}

			/////////////////////

			public ConnectionSettingsClass()
			{
			}

			public static ConnectionSettingsClass CreateDirect( CloudUserRole userRole, string serverAddress, int serverPort, string password, bool allowReconnect )
			{
				var result = new ConnectionSettingsClass();
				result.ConnectionType = ConnectionTypeEnum.Direct;
				result.UserRole = userRole;
				result.ServerAddress = serverAddress;
				result.ServerPort = serverPort;
				result.Password = password;
				result.AllowReconnect = allowReconnect;
				return result;
			}

			public static ConnectionSettingsClass CreateCloud( CloudUserRole userRole, long projectID /*= 0*/, bool allowReconnect )
			{
				var result = new ConnectionSettingsClass();
				result.ConnectionType = ConnectionTypeEnum.Cloud;
				result.UserRole = userRole;
				result.ProjectID = projectID;
				result.AllowReconnect = allowReconnect;
				return result;
			}
		}

		///////////////////////////////////////////////

		public class BasicServiceNode : ClientNode
		{
			ClientNetworkService_Messages messages;

			//

			public BasicServiceNode()
			{
				messages = new ClientNetworkService_Messages();
				RegisterService( messages );
			}

			public ClientNetworkService_Messages Messages
			{
				get { return messages; }
			}
		}

		///////////////////////////////////////////////

		//public class CreateResult
		//{
		//	public BasicServiceClient Client;
		//	public string Error;
		//}

		///////////////////////////////////////////////

		//static BasicServiceClient()
		//{
		//	instances = new List<TranslateClient>();
		//}

		//public static TranslateClient[] GetInstances()
		//{
		//	lock( instances )
		//		return instances.ToArray();
		//}

		//public static async Task<CreateResult> CreateAsync( ConnectionSettingsClass connectionSettings, bool connect )
		//{
		//	var instance = new TranslateClient();
		//	instance.connectionSettings = connectionSettings;

		//	lock( instances )
		//	{
		//		instances.Add( instance );
		//	}

		//	if( connect )
		//	{
		//		var error = await instance.ReconnectAsync();
		//		if( !string.IsNullOrEmpty( error ) )
		//			return new CreateResult() { Error = error };
		//	}

		//	return new CreateResult() { Client = instance };
		//}

		protected BasicServiceClient()
		{
		}

		//protected BasicServiceClient( bool autoUpdate )
		//{
		//	this.autoUpdate = autoUpdate;

		//	if( autoUpdate )
		//	{
		//		autoUpdateInstances.Add( this );

		//		if( autoUpdateInstancesThread == null )
		//		{
		//			autoUpdateInstancesThread = new Thread( AutoUpdateInstancesThreadFunction );
		//			autoUpdateInstancesThread.IsBackground = true;
		//			autoUpdateInstancesThread.Start();
		//		}
		//	}
		//}

		static void AutoUpdateInstancesThreadFunction( object param )
		{
			//maybe change to exit from thread when no instances

			//maybe use wait/trigger instead of Thread.Sleep

			while( true )
			{
				if( autoUpdateInstances.Count > 0 )
				{
					var utcNow = DateTime.UtcNow;
					foreach( var instance in autoUpdateInstances )
						instance.Update( utcNow );
					Thread.Sleep( 1 );
				}
				else
					Thread.Sleep( 10 );
			}
		}

		public bool AutoUpdate
		{
			get { return autoUpdate; }
		}

		public string ServiceName
		{
			get { return serviceName; }
			protected set { serviceName = value; }
		}

		public ConnectionSettingsClass ConnectionSettings
		{
			get { return connectionSettings; }
			protected set { connectionSettings = value; }
		}

		public BasicServiceNode ConnectionNode
		{
			get { return connectionNode; }
		}

		public NetworkStatus Status
		{
			get { return connectionNode?.Status ?? NetworkStatus.Disconnected; }
		}

		public double GetRoundtripLastInSeconds( DateTime? utcNow = null )
		{
			return connectionNode?.GetRoundtripLastInSeconds( utcNow ) ?? 0;
		}

		public string ConnectionErrorReceived
		{
			get { return connectionErrorReceived; }
		}

		public object AnyData { get; set; }

		protected abstract BasicServiceNode OnCreateNetworkNode();

		protected virtual void OnUpdate( DateTime utcNow ) { }

		public void Update( DateTime utcNow )
		{
			if( utcNow - updateLastTime > TimeSpan.FromSeconds( UpdateFrequency ) )
			{
				updateLastTime = utcNow;

				try
				{
					connectionNode?.Update( utcNow );
					OnUpdate( utcNow );
				}
				catch( Exception e )
				{
					Log.Fatal( "BasicServiceClient: Update: Exception: " + e.ToString() );
				}
			}
		}

		//public static void UpdateAll()
		//{
		//	foreach( var instance in GetInstances() )
		//		instance.Update();
		//}

		protected virtual void OnDestroy() { }

		public void Destroy()
		{
			OnDestroy();

			//lock( instances )
			//	instances.Remove( this );

			connectionNode?.Dispose();
			connectionNode = null;

			try
			{
				autoUpdateInstances.TryTake( out _ );
			}
			catch { }
		}

		//public static void DestroyAll()
		//{
		//	foreach( var instance in GetInstances() )
		//		instance.Destroy();
		//}

		public delegate void BeforeConnectDelegate( BasicServiceClient sender, BasicServiceNode node, TextBlock loginData );
		public event BeforeConnectDelegate BeforeConnect;

		protected virtual void OnBeforeConnect( BasicServiceNode node, TextBlock loginData ) { }

		/// <summary>
		/// Returns error.
		/// </summary>
		/// <returns></returns>
		public async Task<string> ReconnectAsync( CancellationToken cancellationToken = default )
		{
			try
			{
				connectionNode?.Dispose();
				connectionNode = null;


				if( connectionSettings.AutoUpdate )
				{
					if( !autoUpdateInstances.Contains( this ) )
					{
						autoUpdateInstances.Add( this );

						if( autoUpdateInstancesThread == null )
						{
							autoUpdateInstancesThread = new Thread( AutoUpdateInstancesThreadFunction );
							autoUpdateInstancesThread.IsBackground = true;
							autoUpdateInstancesThread.Start();
						}
					}
				}

				string serverAddress;
				int serverPort;
				string password = null;
				string verificationCode = null;

				if( connectionSettings.ConnectionType == ConnectionSettingsClass.ConnectionTypeEnum.Cloud )
				{
					if( string.IsNullOrEmpty( ServiceName ) )
						return "ServiceName is not configured.";

					var projectID = connectionSettings.ProjectID;
					//get projectID from command line parameters or assembly file path
					if( projectID == 0 )
						projectID = CloudClientProcessUtility.ProjectID;
					if( projectID == 0 )
						return "ProjectID is not configured.";

					//request access info from cloud. get access data from general manager
					var requestCodeResult = await CloudServiceFunctions.AccessRequestServiceServerAsync( ServiceName, connectionSettings.UserRole, projectID, cancellationToken );
					if( !string.IsNullOrEmpty( requestCodeResult.Error ) )
						return "RequestService failed. " + requestCodeResult.Error;

					serverAddress = requestCodeResult.ServerAddress;
					serverPort = connectionSettings.ServerPort != 0 ? connectionSettings.ServerPort : requestCodeResult.ServerPort;
					verificationCode = requestCodeResult.VerificationCode;
				}
				else
				{
					//connect direct by IP
					serverAddress = connectionSettings.ServerAddress;
					serverPort = connectionSettings.ServerPort;
					password = connectionSettings.Password;
				}

				var node = OnCreateNetworkNode();

				//settings
				node.ConnectingMaxTimeInSeconds = connectionSettings.ConnectingMaxTimeInSeconds;
				node.AllowReconnectFromClient = connectionSettings.AllowReconnect;

				//subscribe to events
				node.ProtocolError += Client_ProtocolError;
				node.ConnectionStatusChanged += Client_ConnectionStatusChanged;
				node.Messages.ReceiveMessageString += Messages_ReceiveMessageString;
				node.Messages.ReceiveMessageBinary += Messages_ReceiveMessageBinary;

				var loginData = new TextBlock();
				loginData.SetAttribute( "UserRole", connectionSettings.UserRole.ToString() );
				if( !string.IsNullOrEmpty( verificationCode ) )
					loginData.SetAttribute( "VerificationCode", verificationCode );
				if( !string.IsNullOrEmpty( password ) )
					loginData.SetAttribute( "Password", password );
				OnBeforeConnect( node, loginData );
				BeforeConnect?.Invoke( this, node, loginData );

				//!!!!
				var https = false;

				var serverPortWebSocket = connectionSettings.ConnectWebSocket ? serverPort : 0;
				var serverPortUDP = connectionSettings.ConnectUDP ? serverPort + 1 : 0;

				if( !node.BeginConnect( https, serverAddress, serverPortWebSocket, serverPortUDP, EngineInfo.Version, loginData.DumpToString(), out var error ) )
				{
					node.Dispose();
					node = null;
					return error;
				}

				connectionNode = node;

				//wait for establishing connection
				while( ConnectionNode.Status == NetworkStatus.Connecting )
				{
					await Task.Delay( 1 );
					if( cancellationToken.IsCancellationRequested )
						break;
				}

				if( ConnectionNode.Status != NetworkStatus.Connected )
					return connectionErrorReceived ?? $"ConnectionNode.Status != NetworkStatus.Connected. Status: {ConnectionNode.Status}, {ConnectionNode.DisconnectionReason ?? ""}";
			}
			catch( Exception e )
			{
				return e.Message;
			}

			return null;
		}

		protected virtual void OnClient_ProtocolError( ClientNode sender, string message )
		{
		}

		private void Client_ProtocolError( ClientNode sender, string message )
		{
			connectionErrorReceived = "Protocol error: " + message;
			OnClient_ProtocolError( sender, message );
		}

		protected virtual void OnClient_ConnectionStatusChanged( ClientNode sender )
		{
		}

		void Client_ConnectionStatusChanged( ClientNode sender )
		{
			if( sender.Status == NetworkStatus.Disconnected )
			{
				if( !string.IsNullOrEmpty( sender.DisconnectionReason ) )
					connectionErrorReceived = sender.DisconnectionReason;
			}

			OnClient_ConnectionStatusChanged( sender );
		}

		protected virtual void OnMessages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			if( message == "Connected" )
				connectionNode.ReceivedMessageSetStatusConnected();
		}

		void Messages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			OnMessages_ReceiveMessageString( sender, message, data );
		}

		protected virtual void OnMessages_ReceiveMessageBinary( ClientNetworkService_Messages sender, string message, byte[] data )
		{
		}

		void Messages_ReceiveMessageBinary( ClientNetworkService_Messages sender, string message, byte[] data )
		{
			OnMessages_ReceiveMessageBinary( sender, message, data );
		}
	}
}
