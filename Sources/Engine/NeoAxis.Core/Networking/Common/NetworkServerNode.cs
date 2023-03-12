// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Internal.Lidgren.Network;

namespace NeoAxis.Networking
{
	public abstract class NetworkServerNode : NetworkNode
	{
		string serverName;
		string serverVersion;
		int maxConnections;
		//double maxConnectionLifetime;
		//string serverPassword = "";

		//services
		List<ServerNetworkService> services = new List<ServerNetworkService>();
		ServerNetworkService[] servicesByIdentifier = new ServerNetworkService[ maxServiceIdentifier + 1 ];
		ReadOnlyCollection<ServerNetworkService> servicesReadOnly;

		//server data
		internal NetServer server;
		ESet<ConnectedNode> connectedNodes = new ESet<ConnectedNode>();//List<ConnectedNode> connectedNodes = new List<ConnectedNode>();
		ConnectedNode[] connectedNodesArray;
		//ReadOnlyCollection<ConnectedNode> connectedNodesReadOnly;

		DateTime dropNodesByConnectionMaxLifetimeLastTime;

		///////////////////////////////////////////

		public delegate void ProtocolErrorDelegate( NetworkServerNode sender, ConnectedNode connectedNode, string message );
		public event ProtocolErrorDelegate ProtocolError;

		public delegate void IncomingConnectionApprovalDelegate( ConnectedNode connectedNode, string clientVersion, string loginData,/* string loginName, string password,*/ /*ref bool approve, */ref string rejectReason );
		public event IncomingConnectionApprovalDelegate IncomingConnectionApproval;

		public delegate void ConnectedNodeConnectionStatusChangedDelegate( NetworkServerNode sender, ConnectedNode connectedNode, NetworkStatus status, string message );
		public event ConnectedNodeConnectionStatusChangedDelegate ConnectedNodeConnectionStatusChanged;

		///////////////////////////////////////////

		protected NetworkServerNode( string serverName, string serverVersion, int maxConnections )//, double maxConnectionLifetime )
		{
			this.serverName = serverName;
			this.serverVersion = serverVersion;
			this.maxConnections = maxConnections;
			//this.maxConnectionLifetime = maxConnectionLifetime;

			servicesReadOnly = new ReadOnlyCollection<ServerNetworkService>( services );
			//connectedNodesReadOnly = new ReadOnlyCollection<ConnectedNode>( connectedNodes );
		}

		public bool BeginListen( int port, float connectionTimeout, out string error )
		{
			error = null;

#if !UWP
			if( Disposed )
				Log.Fatal( "NetworkServer: BeginListen: The server has been disposed." );
			if( server != null )
				Log.Fatal( "NetworkServer: BeginListen: The server is already initialized." );

			//create a configuration for the server
			var config = new NetPeerConfiguration( "NeoAxis" );
			config.MaximumConnections = maxConnections;
			config.Port = port;
			//!!!!может больше
			config.PingInterval = 1;
			config.ConnectionTimeout = connectionTimeout;

			//!!!!
			config.ReceiveBufferSize = 131071 * 10;
			config.SendBufferSize = 131071 * 10;

			//!!!!!!new
			//config.AutoExpandMTU = true;

			//create server
			server = new NetServer( config );
			server.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.ConnectionApproval, true );
			server.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.StatusChanged, true );
			server.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.Data, true );
			server.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.ConnectionLatencyUpdated, true );

			//server.SimulatedLoss = 0.03f; // 3 %
			//server.SimulatedMinimumLatency = 0.1f; // 100 ms
			//server.SimulatedLatencyVariance = 0.05f; // 100-150 ms actually

			//string lastCurrentDirectory = null;
			//         if (SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
			//             SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP)
			//{
			//	if( SystemSettings.NetRuntime == SystemSettings.NetRuntimeType.Mono )
			//	{
			//		lastCurrentDirectory = Directory.GetCurrentDirectory();
			//		VirtualFileSystem.CorrectCurrentDirectory();
			//	}
			//}

			try
			{
				//start listening for connections
				server.Start();
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}
			//finally
			//{
			//             if (SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
			//                 SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP)
			//	{
			//		if( SystemSettings.NetRuntime == SystemSettings.NetRuntimeType.Mono )
			//			Directory.SetCurrentDirectory( lastCurrentDirectory );
			//	}
			//}

			return true;
#else
			error = "No network implementation for the platform.";
			return false;
#endif
		}

		public virtual void Dispose( string reason )
		{
			//dispose connected nodes
			while( connectedNodes.Count != 0 )
			{
				foreach( var node in GetConnectedNodesArray() )
					RemoveAndClearConnectedNode( node );
			}

			if( server != null )
			{
				server.Shutdown( reason );
				server = null;
			}

			//dispose services
			while( services.Count != 0 )
			{
				var service = services[ services.Count - 1 ];
				service.PerformDispose();
				services.Remove( service );
			}

			base.NetworkNode_Dispose();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

#if !UWP
			//check if any messages has been received
			NetIncomingMessage incomingMessage;
			while( server != null && ( incomingMessage = server.ReadMessage() ) != null )
			{
				switch( incomingMessage.MessageType )
				{
				case NetIncomingMessageType.ConnectionApproval:
					{
						var connectedNode = GetConnectedNode( incomingMessage.SenderConnection );

						//receive login name and password
						var clientVersion = "";
						var loginData = "";
						//string loginName = "";
						//string password = "";
						var remoteServices = new List<string>();
						try
						{
							clientVersion = incomingMessage.ReadString();
							loginData = incomingMessage.ReadString();
							//loginName = incomingMessage.ReadString();
							//password = incomingMessage.ReadString();

							int count = (int)incomingMessage.ReadVariableUInt32();
							for( int n = 0; n < count; n++ )
								remoteServices.Add( incomingMessage.ReadString() );
						}
						catch { }

						//new connection
						if( connectedNode == null )
						{
							//!!!!
							////skip equal IPEndPoint. this is Lidgren bug?
							//{
							//	bool skip = false;
							//	foreach( var node in GetConnectedNodesArray() )
							//	{
							//		if( node.RemoteEndPoint.Address == incomingMessage.SenderConnection.RemoteEndPoint.Address && node.RemoteEndPoint.Port == incomingMessage.SenderConnection.RemoteEndPoint.Port )
							//		{
							//			skip = true;
							//			break;
							//		}
							//	}
							//	if( skip )
							//		break;
							//}

							connectedNode = new ConnectedNode( this, incomingMessage.SenderConnection, loginData );// loginName );
							connectedNode.SetRemoteServices( remoteServices );

							connectedNodes.Add( connectedNode );
							connectedNodesArray = null;
							incomingMessage.SenderConnection.Tag = connectedNode;
						}

						var rejectReason = "";
						if( OnIncomingConnectionApproval( connectedNode, clientVersion, loginData,/* loginName, password, */ref rejectReason ) )
						{
							//approve and send server information
							var message = server.CreateMessage();
							message.Write( ServerName );

							message.WriteVariableUInt32( (uint)Services.Count );
							foreach( var service in Services )
								message.Write( service.Name );

							incomingMessage.SenderConnection.Approve( message );
						}
						else
						{
							//disapprove
							incomingMessage.SenderConnection.Deny( rejectReason );
						}
					}
					break;

				case NetIncomingMessageType.StatusChanged:
					{
						var status = (NetConnectionStatus)incomingMessage.ReadByte();
						var message = incomingMessage.ReadString();

						var connectedNode = GetConnectedNode( incomingMessage.SenderConnection );
						if( connectedNode != null )
						{
							//update status
							if( connectedNode.status != connectedNode.GetRealStatus() )
							{
								connectedNode.status = connectedNode.GetRealStatus();

								//need message? equal to server DisconnectionReason?
								OnConnectedNodeConnectionStatusChanged( connectedNode, connectedNode.Status, message );
							}

							//disconnected
							if( status == NetConnectionStatus.Disconnected )
								RemoveAndClearConnectedNode( connectedNode );
						}
					}
					break;

				case NetIncomingMessageType.Data:
					{
						var connectedNode = GetConnectedNode( incomingMessage.SenderConnection );
						if( connectedNode != null )
						{
							if( incomingMessage.Position % 8 != 0 )
							{
								OnReceiveProtocolErrorInternal( connectedNode, "incomingMessage.Position % 8 != 0." );
								break;
							}
							if( incomingMessage.LengthBits % 8 != 0 )
							{
								OnReceiveProtocolErrorInternal( connectedNode, "incomingMessage.LengthBits % 8 != 0." );
								break;
							}

							while( incomingMessage.Position < incomingMessage.LengthBits )
							{
								int byteLength = (int)incomingMessage.ReadVariableUInt32();
								ProcessReceivedMessage( connectedNode, incomingMessage, (int)incomingMessage.Position / 8, byteLength );
								incomingMessage.Position += byteLength * 8;

								//int bitLength = (int)incomingMessage.ReadVariableUInt32();
								//ProcessReceivedMessage( connectedNode, incomingMessage, (int)incomingMessage.Position, bitLength );
								//incomingMessage.Position += bitLength;
								//incomingMessage.SkipPadBits();
							}
						}
					}
					break;

				case NetIncomingMessageType.ConnectionLatencyUpdated:
					{
						var roundtripTime = incomingMessage.ReadSingle();

						var connectedNode = GetConnectedNode( incomingMessage.SenderConnection );
						if( connectedNode != null )
							connectedNode.lastRoundtripTime = roundtripTime;
					}
					break;

				}
			}

			//update services
			for( int n = 0; n < services.Count; n++ )
			{
				var service = services[ n ];
				service.OnUpdate();
			}

			SendAccumulatedMessages();

			DropNodesByConnectionMaxLifetime();
#endif
		}

		protected virtual bool OnIncomingConnectionApproval( ConnectedNode connectedNode, string clientVersion, string loginData, /*string loginName, string password, */ref string rejectReason )
		{
			//if( ServerVersion != clientVersion )
			//{
			//	rejectReason = string.Format( "Invalid client version. Server: {0}, Client: {1}", ServerVersion, clientVersion );
			//	return false;
			//}

			if( string.IsNullOrEmpty( loginData ) )
			{
				rejectReason = "Empty login data.";
				return false;
			}

			//if( ServerPassword != password )
			//{
			//	rejectReason = string.Format( "Invalid password." );
			//	return false;
			//}

			IncomingConnectionApproval?.Invoke( connectedNode, clientVersion, loginData,/* loginName, password,*/ ref rejectReason );
			if( !string.IsNullOrEmpty( rejectReason ) )
				return false;

			return true;
		}

		protected virtual void OnConnectedNodeConnectionStatusChanged( ConnectedNode connectedNode, NetworkStatus status, string message )
		{
			ConnectedNodeConnectionStatusChanged?.Invoke( this, connectedNode, status, message );
		}

		//public IList<ConnectedNode> ConnectedNodes
		//{
		//	get { return connectedNodesReadOnly; }
		//}

		ConnectedNode GetConnectedNode( NetConnection netConnection )
		{
			return (ConnectedNode)netConnection.Tag;
		}

		public void DisconnectConnectedNode( ConnectedNode connectedNode, string reason )//, float lingerSeconds )
		{
			connectedNode.Connection?.Disconnect( reason );// lingerSeconds );
		}

		void RemoveAndClearConnectedNode( ConnectedNode connectedNode )
		{
			connectedNodes.Remove( connectedNode );
			connectedNodesArray = null;
			if( connectedNode.Connection != null )
				connectedNode.Connection.Tag = null;
			connectedNode.Clear();
		}

		//public object _GetLidgrenLibraryNetServer()
		//{
		//	return server;
		//}

		public IList<ServerNetworkService> Services
		{
			get { return servicesReadOnly; }
		}

		protected void RegisterService( ServerNetworkService service )
		{
			if( service.owner != null )
				Log.Fatal( "NetworkServer: RegisterService: Service is already registered." );

			if( service.Identifier == 0 )
				Log.Fatal( "NetworkServer: RegisterService: Invalid service identifier. Identifier can not be zero." );

			if( service.Identifier > maxServiceIdentifier )
				Log.Fatal( "NetworkServer: RegisterService: Invalid service identifier. Maximum identifier is \"{0}\".", maxServiceIdentifier );

			//check for unique identifier
			{
				NetworkService checkService = GetService( service.Identifier );
				if( checkService != null )
					Log.Fatal( "NetworkServer: RegisterService: Service with identifier \"{0}\" is already registered.", service.Identifier );
			}

			//check for unique name
			{
				NetworkService checkService = GetService( service.Name );
				if( checkService != null )
					Log.Fatal( "NetworkServer: RegisterService: Service with name \"{0}\" is already registered.", service.Name );
			}

			service.baseOwner = this;
			service.owner = this;
			services.Add( service );
			servicesByIdentifier[ service.Identifier ] = service;
		}

		internal override NetworkService GetService( byte identifier )
		{
			if( identifier >= servicesByIdentifier.Length )
				return null;
			return servicesByIdentifier[ identifier ];
		}

		internal override NetworkService GetService( string name )
		{
			foreach( ServerNetworkService service in services )
			{
				if( service.Name == name )
					return service;
			}
			return null;
		}

		//protected virtual void OnReceiveProtocolError( ConnectedNode sender, string message ) { }

		internal override void OnReceiveProtocolErrorInternal( ConnectedNode connectedNode, string message )
		{
			ProtocolError?.Invoke( this, connectedNode, message );

			//OnReceiveProtocolError( sender, message );
		}

		public string ServerName
		{
			get { return serverName; }
		}

		public string ServerVersion
		{
			get { return serverVersion; }
		}

		//public string ServerPassword
		//{
		//	get { return serverPassword; }
		//	set { serverPassword = value; }
		//}

		public ConnectedNode[] GetConnectedNodesArray()
		{
			if( connectedNodesArray == null )
				connectedNodesArray = connectedNodes.ToArray();
			return connectedNodesArray;
		}

		public int GetConnectedNodesCount()
		{
			return connectedNodes.Count;
		}

		public void SendAccumulatedMessages()
		{
			if( server != null )
			{
				foreach( var node in GetConnectedNodesArray() )
					node.SendAccumulatedMessages( server );
			}
		}

		void DropNodesByConnectionMaxLifetime()
		{
			if( server != null )
			{
				var now = DateTime.Now;

				if( ( now - dropNodesByConnectionMaxLifetimeLastTime ).TotalSeconds > 1 )
				{
					dropNodesByConnectionMaxLifetimeLastTime = now;

					foreach( var connectedNode in GetConnectedNodesArray() )
					{
						if( connectedNode.MaxLifetime > 0 && ( now - connectedNode.CreationTime ).TotalSeconds > connectedNode.MaxLifetime )
							DisconnectConnectedNode( connectedNode, "Max lifetime" );
					}
				}
			}

			//if( server != null && maxConnectionLifetime > 0 )
			//{
			//	var now = DateTime.Now;

			//	if( ( now - dropNodesByMaxConnectionLifetimeLastTime ).TotalSeconds > 1 )
			//	{
			//		dropNodesByMaxConnectionLifetimeLastTime = now;

			//		foreach( var node in GetConnectedNodesArray() )
			//		{
			//			if( ( now - node.CreationTime ).TotalSeconds > maxConnectionLifetime )
			//				DisconnectConnectedNode( node, "Max connection lifetime" );
			//		}
			//	}
			//}
		}

	}
}