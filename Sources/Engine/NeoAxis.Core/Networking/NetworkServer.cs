//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Text;
//using System.Net;
//using System.IO;
//using NeoAxis;


//using Lidgren.Network;

//namespace NeoAxis
//{
//	public abstract class NetworkServer : NetworkNode
//	{
//		string serverName;
//		string serverVersion;
//		int maxConnections;

//		//services
//		List<ServerNetworkService> services = new List<ServerNetworkService>();
//		ServerNetworkService[] servicesByIdentifier = new ServerNetworkService[
//			maxServiceIdentifier + 1 ];
//		ReadOnlyCollection<ServerNetworkService> servicesReadOnly;

//		//server data
//		internal NetServer server;
//		List<ConnectedNode> connectedNodes = new List<ConnectedNode>();
//		ReadOnlyCollection<ConnectedNode> connectedNodesReadOnly;

//		///////////////////////////////////////////

//		public delegate void ConnectedNodeConnectionStatusChangedDelegate( NetworkServer sender,
//			ConnectedNode connectedNode, NetworkConnectionStatuses status, string message );
//		public event ConnectedNodeConnectionStatusChangedDelegate ConnectedNodeConnectionStatusChanged;

//		///////////////////////////////////////////

//		protected NetworkServer( string serverName, string serverVersion, int maxConnections )
//		{
//			this.serverName = serverName;
//			this.serverVersion = serverVersion;
//			this.maxConnections = maxConnections;

//			servicesReadOnly = new ReadOnlyCollection<ServerNetworkService>( services );
//			connectedNodesReadOnly = new ReadOnlyCollection<ConnectedNode>( connectedNodes );
//		}

//		public bool BeginListen( int port, out string error )
//		{
//			error = null;

//			if( Disposed )
//				Log.Fatal( "NetworkServer: BeginListen: The server has been disposed." );
//			if( server != null )
//				Log.Fatal( "NetworkServer: BeginListen: The server is already initialized." );

//			//create a configuration for the server
//			NetPeerConfiguration config = new NetPeerConfiguration( "NeoAxis" );
//			config.MaximumConnections = maxConnections;
//			config.Port = port;
//			config.PingInterval = 1;

//			//!!!!!!new
//			//config.AutoExpandMTU = true;

//			//create server
//			server = new NetServer( config );
//			server.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.ConnectionApproval, true );
//			server.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.StatusChanged, true );
//			server.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.Data, true );
//			server.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.ConnectionLatencyUpdated, true );

//			//server.SimulatedLoss = 0.03f; // 3 %
//			//server.SimulatedMinimumLatency = 0.1f; // 100 ms
//			//server.SimulatedLatencyVariance = 0.05f; // 100-150 ms actually

//			//string lastCurrentDirectory = null;
//   //         if (SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
//   //             SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP)
//			//{
//			//	if( SystemSettings.NetRuntime == SystemSettings.NetRuntimeType.Mono )
//			//	{
//			//		lastCurrentDirectory = Directory.GetCurrentDirectory();
//			//		VirtualFileSystem.CorrectCurrentDirectory();
//			//	}
//			//}

//			try
//			{
//				//start listening for connections
//				server.Start();
//			}
//			catch( Exception e )
//			{
//				error = e.Message;
//				return false;
//			}
//			//finally
//			//{
//   //             if (SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
//   //                 SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP)
//			//	{
//			//		if( SystemSettings.NetRuntime == SystemSettings.NetRuntimeType.Mono )
//			//			Directory.SetCurrentDirectory( lastCurrentDirectory );
//			//	}
//			//}

//			return true;
//		}

//		public virtual void Dispose( string reason )
//		{
//			//dispose connected nodes
//			while( connectedNodes.Count != 0 )
//			{
//				ConnectedNode connectedNode = connectedNodes[ 0 ];
//				RemoveAndClearConnectedNode( connectedNode );
//			}

//			if( server != null )
//			{
//				server.Shutdown( reason );
//				server = null;
//			}

//			//dispose services
//			while( services.Count != 0 )
//			{
//				ServerNetworkService service = services[ services.Count - 1 ];
//				service.Dispose();
//				services.Remove( service );
//			}

//			base.NetworkNode_Dispose();
//		}

//		protected override void OnUpdate()
//		{
//			base.OnUpdate();

//			//check if any messages has been received
//			NetIncomingMessage incomingMessage;
//			while( server != null && ( incomingMessage = server.ReadMessage() ) != null )
//			{
//				switch( incomingMessage.MessageType )
//				{
//				case NetIncomingMessageType.ConnectionApproval:
//					{
//						ConnectedNode connectedNode = GetConnectedNode( incomingMessage.SenderConnection );

//						//receive login name and password
//						string clientVersion = "";
//						string loginName = "";
//						string password = "";
//						List<string> remoteServices = new List<string>();
//						try
//						{
//							clientVersion = incomingMessage.ReadString();
//							loginName = incomingMessage.ReadString();
//							password = incomingMessage.ReadString();

//							int count = (int)incomingMessage.ReadVariableUInt32();
//							for( int n = 0; n < count; n++ )
//								remoteServices.Add( incomingMessage.ReadString() );
//						}
//						catch { }

//						//new connection
//						if( connectedNode == null )
//						{
//							//skip equal IPEndPoint. this is Lidgren bug?
//							{
//								bool skip = false;
//								foreach( ConnectedNode node in ConnectedNodes )
//								{
//									if( node.RemoteEndPoint.Address == incomingMessage.SenderConnection.RemoteEndpoint.Address &&
//										node.RemoteEndPoint.Port == incomingMessage.SenderConnection.RemoteEndpoint.Port )
//									{
//										skip = true;
//										break;
//									}
//								}
//								if( skip )
//									break;
//							}

//							connectedNode = new ConnectedNode( this, incomingMessage.SenderConnection, loginName );
//							connectedNode.SetRemoteServices( remoteServices );

//							connectedNodes.Add( connectedNode );
//							incomingMessage.SenderConnection.Tag = connectedNode;
//						}

//						string rejectReason = "";
//						if( OnIncomingConnectionApproval( connectedNode, clientVersion, loginName,
//							password, ref rejectReason ) )
//						{
//							//approve and send server information
//							NetOutgoingMessage message = server.CreateMessage();
//							message.Write( ServerName );

//							message.WriteVariableUInt32( (uint)Services.Count );
//							foreach( ServerNetworkService service in Services )
//								message.Write( service.Name );

//							incomingMessage.SenderConnection.Approve( message );
//						}
//						else
//						{
//							//disapprove
//							incomingMessage.SenderConnection.Deny( rejectReason );
//						}
//					}
//					break;

//				case NetIncomingMessageType.StatusChanged:
//					{
//						NetConnectionStatus status = (NetConnectionStatus)incomingMessage.ReadByte();
//						string message = incomingMessage.ReadString();

//						ConnectedNode connectedNode = GetConnectedNode( incomingMessage.SenderConnection );
//						if( connectedNode != null )
//						{
//							//update status
//							if( connectedNode.status != connectedNode.GetRealStatus() )
//							{
//								connectedNode.status = connectedNode.GetRealStatus();

//								//need message? equal to server DisconnectionReason?
//								OnConnectedNodeConnectionStatusChanged( connectedNode, connectedNode.Status,
//									message );
//							}

//							//disconnected
//							if( status == NetConnectionStatus.Disconnected )
//								RemoveAndClearConnectedNode( connectedNode );
//						}
//					}
//					break;

//				case NetIncomingMessageType.Data:
//					{
//						ConnectedNode connectedNode = GetConnectedNode( incomingMessage.SenderConnection );
//						if( connectedNode != null )
//						{
//							while( incomingMessage.Position < incomingMessage.LengthBits )
//							{
//								int bitLength = (int)incomingMessage.ReadVariableUInt32();
//								ProcessReceivedMessage( connectedNode, incomingMessage,
//									(int)incomingMessage.Position, bitLength );
//								incomingMessage.Position += bitLength;
//								incomingMessage.SkipPadBits();
//							}
//						}
//					}
//					break;

//				case NetIncomingMessageType.ConnectionLatencyUpdated:
//					{
//						float roundtripTime = incomingMessage.ReadSingle();

//						ConnectedNode connectedNode = GetConnectedNode( incomingMessage.SenderConnection );
//						if( connectedNode != null )
//							connectedNode.lastRoundtripTime = roundtripTime;
//					}
//					break;

//				}
//			}

//			//update services
//			for( int n = 0; n < services.Count; n++ )
//			{
//				NetworkService service = services[ n ];
//				service.OnUpdate();
//			}

//			SendFlush();
//		}

//		protected virtual bool OnIncomingConnectionApproval( NetworkNode.ConnectedNode connectedNode,
//			string clientVersion, string loginName, string password, ref string rejectReason )
//		{
//			if( string.IsNullOrEmpty( loginName ) )
//			{
//				rejectReason = "Invalid login name";
//				return false;
//			}

//			if( ServerVersion != clientVersion )
//			{
//				rejectReason = string.Format( "Invalid client version. Server: {0}, Client: {1}",
//					ServerVersion, clientVersion );
//				return false;
//			}

//			return true;
//		}

//		protected virtual void OnConnectedNodeConnectionStatusChanged( ConnectedNode connectedNode,
//			NetworkConnectionStatuses status, string message )
//		{
//			if( ConnectedNodeConnectionStatusChanged != null )
//				ConnectedNodeConnectionStatusChanged( this, connectedNode, status, message );
//		}

//		public IList<ConnectedNode> ConnectedNodes
//		{
//			get { return connectedNodesReadOnly; }
//		}

//		ConnectedNode GetConnectedNode( NetConnection netConnection )
//		{
//			return (ConnectedNode)netConnection.Tag;
//		}

//		public void DisconnectConnectedNode( ConnectedNode connectedNode, string reason,
//			float lingerSeconds )
//		{
//			if( connectedNode.Connection != null )
//				connectedNode.Connection.Disconnect( reason );// lingerSeconds );
//		}

//		void RemoveAndClearConnectedNode( ConnectedNode connectedNode )
//		{
//			connectedNodes.Remove( connectedNode );
//			connectedNode.Connection.Tag = null;
//			connectedNode.Clear();
//		}

//		public object _GetLidgrenLibraryNetServer()
//		{
//			return server;
//		}

//		public IList<ServerNetworkService> Services
//		{
//			get { return servicesReadOnly; }
//		}

//		protected void RegisterService( ServerNetworkService service )
//		{
//			if( service.owner != null )
//				Log.Fatal( "NetworkServer: RegisterService: Service is already registered." );

//			if( service.Identifier == 0 )
//			{
//				Log.Fatal( "NetworkServer: RegisterService: Invalid service identifier. " +
//					"Identifier can not be zero." );
//			}

//			if( service.Identifier > maxServiceIdentifier )
//			{
//				Log.Fatal( "NetworkServer: RegisterService: Invalid service identifier. " +
//					"Maximum identifier is \"{0}\".", maxServiceIdentifier );
//			}

//			//check for unique identifier
//			{
//				NetworkService checkService = GetService( service.Identifier );
//				if( checkService != null )
//				{
//					Log.Fatal( "NetworkServer: RegisterService: Service with identifier \"{0}\" " +
//						"is already registered.", service.Identifier );
//				}
//			}

//			//check for unique name
//			{
//				NetworkService checkService = GetService( service.Name );
//				if( checkService != null )
//				{
//					Log.Fatal( "NetworkServer: RegisterService: Service with name \"{0}\" " +
//						"is already registered.", service.Name );
//				}
//			}

//			service.baseOwner = this;
//			service.owner = this;
//			services.Add( service );
//			servicesByIdentifier[ service.Identifier ] = service;
//		}

//		internal override NetworkService GetService( byte identifier )
//		{
//			if( identifier >= servicesByIdentifier.Length )
//				return null;
//			return servicesByIdentifier[ identifier ];
//		}

//		internal override NetworkService GetService( string name )
//		{
//			foreach( ServerNetworkService service in services )
//			{
//				if( service.Name == name )
//					return service;
//			}
//			return null;
//		}

//		protected virtual void OnReceiveProtocolError( ConnectedNode sender, string message ) { }

//		internal override void OnReceiveProtocolErrorInternal( NetworkNode.ConnectedNode sender,
//			string message )
//		{
//			OnReceiveProtocolError( sender, message );
//		}

//		public string ServerName
//		{
//			get { return serverName; }
//		}

//		public string ServerVersion
//		{
//			get { return serverVersion; }
//		}

//		public void SendFlush()
//		{
//			if( server == null )
//				return;
//			for( int n = 0; n < connectedNodes.Count; n++ )
//				connectedNodes[ n ].SendFlush( server );
//		}
//	}
//}
