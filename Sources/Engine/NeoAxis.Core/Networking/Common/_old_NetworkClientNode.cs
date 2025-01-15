// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if LIDGREN
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Internal.Lidgren.Network;

namespace NeoAxis.Networking
{
	public abstract class NetworkClientNode : NetworkNode
	{
		string loginData = "";
		string remoteServerName = "";

		string disconnectionReason = "";
		double lastReceivedMessageTime;

		//services
		List<ClientNetworkService> services = new List<ClientNetworkService>();
		ClientNetworkService[] servicesByIdentifier = new ClientNetworkService[ maxServiceIdentifier + 1 ];
		ReadOnlyCollection<ClientNetworkService> servicesReadOnly;

		//client data
		internal NetClient client;
		ConnectedNode serverConnectedNode;
		bool beginConnecting;

		///////////////////////////////////////////

		public delegate void ProtocolErrorDelegate( NetworkClientNode sender, string message );
		public event ProtocolErrorDelegate ProtocolError;

		public delegate void ConnectionStatusChangedDelegate( NetworkClientNode sender, NetworkStatus status );
		public event ConnectionStatusChangedDelegate ConnectionStatusChanged;

		///////////////////////////////////////////

		protected NetworkClientNode()
		{
			servicesReadOnly = new ReadOnlyCollection<ClientNetworkService>( services );
		}

		public bool BeginConnect( string host, int port, string clientVersion, string loginData, float connectionTimeout, out string error )
		{
			this.loginData = loginData;

			error = null;

#if !UWP
			if( Disposed )
				Log.Fatal( "NetworkClient: BeginConnect: The client has been disposed." );
			if( client != null )
				Log.Fatal( "NetworkClient: BeginConnect: The client is already initialized." );

			disconnectionReason = "";

			var config = new NetPeerConfiguration( "NeoAxis" );
			////maybe more
			//config.PingInterval = 1;
			config.ConnectionTimeout = connectionTimeout;

			config.ReceiveBufferSize = 131071 * 10;
			config.SendBufferSize = 131071 * 10;
			config.AutoExpandMTU = true;//with it much faster

			//m_useMessageRecycling = true;
			//m_recycledCacheMaxCount = 64;
			//m_resendHandshakeInterval = 3.0f;
			//m_maximumHandshakeAttempts = 5;
			//m_autoFlushSendQueue = true;
			//m_suppressUnreliableUnorderedAcks = false;


			client = new NetClient( config );

			//client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.DebugMessage, true );
			//client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.VerboseDebugMessage, true );
			//client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.WarningMessage, true );
			client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.ErrorMessage, true );
			client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.StatusChanged, true );
			client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.Data, true );
			client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.ConnectionLatencyUpdated, true );

			//client.SimulatedLoss = 0.03f; // 3 %
			//client.SimulatedMinimumLatency = 0.1f; // 100 ms
			//client.SimulatedLatencyVariance = 0.05f; // 100-150 ms actually

			//string lastCurrentDirectory = null;
			//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
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
				client.Start();

				var message = client.CreateMessage();
				message.Write( clientVersion );
				message.Write( loginData );
				//message.Write( loginName );
				//message.Write( password );

				message.WriteVariableUInt32( (uint)Services.Count );
				foreach( NetworkService service in Services )
					message.Write( service.Name );

				client.Connect( host, port, message );
			}
			catch( Exception e )
			{
				error = e.Message;
				if( e.InnerException != null )
					error = e.InnerException.Message;

				return false;
			}
			//finally
			//{
			//	if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows  ||
			//                 SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP)
			//	{
			//		if( SystemSettings.NetRuntime == SystemSettings.NetRuntimeType.Mono )
			//			Directory.SetCurrentDirectory( lastCurrentDirectory );
			//	}
			//}

			beginConnecting = true;

			return true;
#else
			error = "No network implementation for the platform.";
			return false;
#endif
		}

		public virtual void Dispose()
		{
			if( serverConnectedNode != null )
			{
				serverConnectedNode.Clear();
				serverConnectedNode = null;
			}

			if( client != null )
			{
				client.Disconnect( "" );
				client.Shutdown( "" );
				client = null;
			}

			//dispose services
			while( services.Count != 0 )
			{
				var service = services[ services.Count - 1 ];
				service.PerformDispose();
				services.Remove( service );
			}

			beginConnecting = false;

			////clear?
			//loginData = "";
			//remoteServerName = "";

			base.NetworkNode_Dispose();
		}

		public NetworkStatus Status
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( beginConnecting )
					return NetworkStatus.Connecting;
				if( serverConnectedNode == null )
					return NetworkStatus.Disconnected;
				return serverConnectedNode.Status;
			}
		}

		public string LoginData
		{
			get { return loginData; }
		}

		public string RemoteServerName
		{
			get { return remoteServerName; }
		}

		public ConnectedNode ServerConnectedNode
		{
			get { return serverConnectedNode; }
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnUpdate()
		{
			base.OnUpdate();

#if !UWP
			//check if any messages has been received
			NetIncomingMessage incomingMessage;
			while( client != null && ( incomingMessage = client.ReadMessage() ) != null )
			{
				lastReceivedMessageTime = EngineApp.EngineTime;

				switch( incomingMessage.MessageType )
				{

				case NetIncomingMessageType.ErrorMessage:
					{
						var message = incomingMessage.ReadString();

						if( serverConnectedNode != null )
							serverConnectedNode.status = NetworkStatus.Disconnected;
						disconnectionReason = message;
						OnConnectionStatusChanged( NetworkStatus.Disconnected );
						Dispose();
					}
					break;

				case NetIncomingMessageType.StatusChanged:
					{
						var status = (NetConnectionStatus)incomingMessage.ReadByte();
						string message = incomingMessage.ReadString();

						switch( status )
						{
						case NetConnectionStatus.RespondedConnect:
						case NetConnectionStatus.Connected:
							{
								if( serverConnectedNode == null )
									serverConnectedNode = new ConnectedNode( this, client.ServerConnection, loginData );

								beginConnecting = false;

								NetworkStatus realStatus = serverConnectedNode.GetRealStatus();

								//update status
								if( serverConnectedNode.status != realStatus )
								{
									serverConnectedNode.status = realStatus;

									//receive server information
									if( serverConnectedNode.Status == NetworkStatus.Connected )
									{
										var hailMessage = serverConnectedNode.Connection.RemoteHailMessage;

										remoteServerName = hailMessage.ReadString();
										var remoteServices = new List<string>();
										int serviceCount = (int)hailMessage.ReadVariableUInt32();
										for( int n = 0; n < serviceCount; n++ )
											remoteServices.Add( hailMessage.ReadString() );
										serverConnectedNode.SetRemoteServices( remoteServices );

										hailMessage.Position = 0;
									}

									OnConnectionStatusChanged( Status );
								}

							}
							break;

						case NetConnectionStatus.Disconnecting:
						case NetConnectionStatus.Disconnected:
							{
								if( serverConnectedNode != null )
									serverConnectedNode.status = NetworkStatus.Disconnected;
								disconnectionReason = message;
								OnConnectionStatusChanged( NetworkStatus.Disconnected );
								Dispose();
							}
							break;
						}

					}
					break;

				case NetIncomingMessageType.Data:
					{
						if( incomingMessage.Position % 8 != 0 )
						{
							OnReceiveProtocolErrorInternal( null, "incomingMessage.Position % 8 != 0." );
							break;
						}
						if( incomingMessage.LengthBits % 8 != 0 )
						{
							OnReceiveProtocolErrorInternal( null, "incomingMessage.LengthBits % 8 != 0." );
							break;
						}

						//Log.Warning( "Data2: " + incomingMessage.Position.ToString() + " " + incomingMessage.LengthBits.ToString() );

						while( incomingMessage.Position < incomingMessage.LengthBits )
						{
							int byteLength = (int)incomingMessage.ReadVariableUInt32WithBytesReaded( out var bytesReaded );
							ProcessReceivedMessage( serverConnectedNode, incomingMessage, (int)incomingMessage.Position / 8, byteLength );
							incomingMessage.Position += byteLength * 8;

							if( ProfilerData != null )
							{
								//ProfilerData.TotalReceivedMessages++;
								ProfilerData.TotalReceivedSize += bytesReaded + byteLength;
							}

							//ProcessReceivedMessage( serverConnectedNode, incomingMessage, (int)incomingMessage.Position, bitLength );
							//incomingMessage.Position += bitLength;
							//incomingMessage.SkipPadBits();
						}
					}
					break;

				case NetIncomingMessageType.ConnectionLatencyUpdated:
					{
						float roundtripTime = incomingMessage.ReadSingle();

						if( serverConnectedNode != null )
							serverConnectedNode.lastRoundtripTime = roundtripTime;
					}
					break;
				}
			}

			//update services
			for( int n = 0; n < services.Count; n++ )
			{
				NetworkService service = services[ n ];
				service.OnUpdate();
			}

			SendAccumulatedMessages();
#endif
		}

		protected virtual void OnConnectionStatusChanged( NetworkStatus status )
		{
			ConnectionStatusChanged?.Invoke( this, status );
		}

		public object _GetLidgrenLibraryNetClient()
		{
			return client;
		}

		public IList<ClientNetworkService> Services
		{
			get { return servicesReadOnly; }
		}

		protected void RegisterService( ClientNetworkService service )
		{
			if( service.owner != null )
				Log.Fatal( "NetworkClientNode: RegisterService: Service is already registered." );

			if( service.Identifier == 0 )
				Log.Fatal( "NetworkClientNode: RegisterService: Invalid service identifier. Identifier can not be zero." );

			if( service.Identifier > maxServiceIdentifier )
				Log.Fatal( "NetworkClientNode: RegisterService: Invalid service identifier. Max identifier is \"{0}\".", maxServiceIdentifier );

			//check for unique identifier
			{
				NetworkService checkService = GetService( service.Identifier );
				if( checkService != null )
					Log.Fatal( "NetworkClientNode: RegisterService: Service with identifier \"{0}\" is already registered.", service.Identifier );
			}

			//check for unique name
			{
				NetworkService checkService = GetService( service.Name );
				if( checkService != null )
					Log.Fatal( "NetworkClientNode: RegisterService: Service with name \"{0}\" is already registered.", service.Name );
			}

			service.baseOwner = this;
			service.owner = this;
			services.Add( service );
			servicesByIdentifier[ service.Identifier ] = service;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal override NetworkService GetService( int identifier )
		{
			if( identifier >= servicesByIdentifier.Length )
				return null;
			return servicesByIdentifier[ identifier ];
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal override NetworkService GetService( string name )
		{
			for( int n = 0; n < services.Count; n++ )
			{
				var service = services[ n ];
				if( service.Name == name )
					return service;
			}
			return null;
		}

		//protected internal virtual void OnReceiveProtocolError( string message ) { }

		internal override void OnReceiveProtocolErrorInternal( ConnectedNode connectedNode, string message )
		{
			ProtocolError?.Invoke( this, message );
			//OnReceiveProtocolError( message );
		}

		public string DisconnectionReason
		{
			get { return disconnectionReason; }
		}

		public double LastReceivedMessageTime
		{
			get { return lastReceivedMessageTime; }
		}

		public void SendAccumulatedMessages()
		{
			if( client == null || serverConnectedNode == null )
				return;
			serverConnectedNode.SendAccumulatedMessages( client );
		}
	}
}
#endif