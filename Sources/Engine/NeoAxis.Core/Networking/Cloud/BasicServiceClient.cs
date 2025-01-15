// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace NeoAxis.Networking
{
	/// <summary>
	/// A basic client code for services.
	/// </summary>
	public abstract class BasicServiceClient
	{
		static ConcurrentBag<BasicServiceClient> autoUpdateInstances = new ConcurrentBag<BasicServiceClient>();
		//static List<BasicServiceClient> instances;

		static Thread autoUpdateInstancesThread;
		//static bool autoUpdateInstancesThreadNeedExit;

		string serviceName;

		ConnectionSettingsClass connectionSettings;
		BasicServiceNode connectionNode;
		bool verified;

		string connectionErrorReceived;

		///////////////////////////////////////////////

		public class ConnectionSettingsClass
		{
			public ConnectionTypeEnum ConnectionType;

			//Direct specific
			public string ServerAddress;
			public int ServerPort;
			public string Password;

			//Cloudbox specific
			public long ProjectID;

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
				Cloudbox,
			}

			/////////////////////

			public ConnectionSettingsClass()
			{
			}

			public static ConnectionSettingsClass CreateDirect( string serverAddress, int serverPort, string password )
			{
				var result = new ConnectionSettingsClass();
				result.ConnectionType = ConnectionTypeEnum.Direct;
				result.ServerAddress = serverAddress;
				result.ServerPort = serverPort;
				result.Password = password;
				return result;
			}

			public static ConnectionSettingsClass CreateCloudbox( long projectID = 0 )
			{
				var result = new ConnectionSettingsClass();
				result.ConnectionType = ConnectionTypeEnum.Cloudbox;
				result.ProjectID = projectID;
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

		public class CreateResult
		{
			public BasicServiceClient Client;
			public string Error;
		}

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

		protected BasicServiceClient( bool autoUpdate )
		{
			if( autoUpdate )
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

		static void AutoUpdateInstancesThreadFunction( object param )
		{

			//!!!!
			while( true )// !Disposed && !threadNeedExit )
			{
				if( autoUpdateInstances.Count > 0 )
				{
					foreach( var instance in autoUpdateInstances.ToArray() )
						instance.Update();
					Thread.Sleep( 1 );
				}
				else
					Thread.Sleep( 10 );
			}
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

		public bool Verified
		{
			get { return verified; }
		}

		public string ConnectionErrorReceived
		{
			get { return connectionErrorReceived; }
		}

		protected abstract BasicServiceNode OnCreateNetworkNode();

		protected virtual void OnUpdate() { }

		public void Update()
		{
			connectionNode?.Update();

			OnUpdate();
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

		//!!!!about reconnect
		//!!!!проверять когда было последнее сообщение. еще много где может быть проблема с накапливанием неотправленных сообщений

		//public bool Connected
		//{
		//	get
		//	{
		//		return false;
		//	}
		//}

		/// <summary>
		/// Returns error.
		/// </summary>
		/// <returns></returns>
		public async Task<string> ReconnectAsync()
		{
			try
			{
				connectionNode?.Dispose();
				connectionNode = null;

				string serverAddress;
				int serverPort;
				string password = null;
				string verificationCode = null;

				if( connectionSettings.ConnectionType == ConnectionSettingsClass.ConnectionTypeEnum.Cloudbox )
				{
					if( string.IsNullOrEmpty( ServiceName ) )
						return "ServiceName is not configured.";

					//request access info from Cloudbox. get access data from general manager
					var requestCodeResult = await GeneralManagerFunctions.RequestServiceAsync( ServiceName, connectionSettings.ProjectID );
					if( !string.IsNullOrEmpty( requestCodeResult.Error ) )
						return "RequestService failed. " + requestCodeResult.Error;

					serverAddress = requestCodeResult.ServerAddress;
					serverPort = requestCodeResult.ServerPort;
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
				node.ProtocolError += Client_ProtocolError;
				node.ConnectionStatusChanged += Client_ConnectionStatusChanged;
				node.Messages.ReceiveMessageString += Messages_ReceiveMessageString;
				node.Messages.ReceiveMessageBinary += Messages_ReceiveMessageBinary;

				var rootBlock = new TextBlock();
				if( !string.IsNullOrEmpty( verificationCode ) )
					rootBlock.SetAttribute( "VerificationCode", verificationCode );
				if( !string.IsNullOrEmpty( password ) )
					rootBlock.SetAttribute( "Password", password );

				if( !node.BeginConnect( serverAddress, serverPort, EngineInfo.Version, rootBlock.DumpToString(), 30, out var error ) )
				{
					node.Dispose();
					node = null;
					return error;
				}

				connectionNode = node;

				//wait for establishing connection
				{
					while( ConnectionNode.Status == NetworkStatus.Connecting || ( ConnectionNode.Status == NetworkStatus.Connected && !Verified ) )
					{
						Update();
						await Task.Delay( 1 );
					}
					if( ConnectionNode.Status != NetworkStatus.Connected || !Verified )
						return connectionErrorReceived ?? "ConnectionNode.Status != NetworkStatus.Connected || !Verified";
				}
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
			//!!!!? reconnect, resend requests

			connectionErrorReceived = "Protocol error: " + message;

			OnClient_ProtocolError( sender, message );

			//Console.WriteLine( "BasicServiceClient: Protocol error: " + message );
		}

		protected virtual void OnClient_ConnectionStatusChanged( ClientNode sender )
		{
		}

		void Client_ConnectionStatusChanged( ClientNode sender )
		{
			if( sender.Status == NetworkStatus.Disconnected )
			{
				//!!!!? reconnect, resend requests

				if( !string.IsNullOrEmpty( sender.DisconnectionReason ) )
				{
					connectionErrorReceived = sender.DisconnectionReason;
				}
			}

			OnClient_ConnectionStatusChanged( sender );

			//Console.WriteLine( $"BasicServiceClient: Client_ConnectionStatusChanged " + status.ToString() );
			//Console.WriteLine( $"reason " + ( sender.DisconnectionReason ?? "" ) );
		}

		protected virtual void OnMessages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
		}

		void Messages_ReceiveMessageString( ClientNetworkService_Messages sender, string message, string data )
		{
			if( message == "Verified" )
				verified = true;

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
