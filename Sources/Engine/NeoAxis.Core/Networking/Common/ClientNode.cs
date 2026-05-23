// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;

namespace NeoAxis.Networking
{
	/// <summary>
	/// Base class for a network client instance.
	/// </summary>
	public abstract class ClientNode
	{
		readonly static bool trace = false;
		const int maxServiceIdentifier = 255;
		const double sendPingIntervalInSeconds = 1;

		static List<ClientNode> instances = new List<ClientNode>();

		volatile bool disposed;

		//profiler
		ProfilerDataClass profilerData;

		string clientVersion = "";
		string loginData = "";
		public int SendMessageMaxSize { get; set; } = 10 * 1024 * 1024;
		public int ReceiveMessageMaxSize { get; set; } = 11 * 1024 * 1024;

		//settings

		/// <summary>
		/// Gets or sets the maximum amount of time, in seconds, to wait for a connection to be established. The connection time is duplicated when WebSocket and UDP enabled at same time.
		/// </summary>
		public double ConnectingMaxTimeInSeconds { get; set; } = 20;
		public bool AllowReconnectFromClient { get; set; }

		/// <summary>
		/// UDP library logic update and send period, in seconds.
		/// </summary>
		public double UpdateTimeUdp { get; set; } = 0.015;

		//synchronized from server
		double keepAliveTimeFromServer = 60;
		string reconnectTokenFromServer;

		//connection state
		volatile NetworkStatus status;
		ConnectionTypeEnum connectionType;

		internal DateTime noRealConnectionStartTime;

		string disconnectionReason = "";

		//services
		List<ClientService> services = new List<ClientService>();
		ClientService[] servicesByIdentifier = new ClientService[ maxServiceIdentifier + 1 ];
		ReadOnlyCollection<ClientService> servicesReadOnly;

		//client data
		bool clientConnectHttps;
		string clientConnectHost = "";
		int clientConnectPortWebSocket;
		string clientConnectAddressWebSocket = ""; //changed when reconnect
		volatile ClientWebSocket webSocketClient;
		int clientConnectPortUdp;
		volatile UdpClientListener udpListener;
		volatile NetPeer udpPeer;
		volatile bool connectionIsOld;

		//background task
		Task connectAndSendingTask;
		Task receiveMessagesTask;
		volatile bool backgroundTaskNeedExit;
		volatile bool backgroundTasksNeedExitSendClose;
		AsyncAutoResetEvent backgroundTaskSemaphore = new AsyncAutoResetEvent();

		//limits. it is solved by timeout
		ConcurrentQueue<ReceivedMessage> receivedMessages = new ConcurrentQueue<ReceivedMessage>();
		ConcurrentQueue<ToProcessMessage> toProcessMessages = new ConcurrentQueue<ToProcessMessage>();
		int toProcesssMessagesTotalBinaryDataSize;

		//these values are changed from the background task
		int dataMessagesReceivedCounter;
		long dataSizeReceivedCounter;
		uint dataMessagesReceivedChecksum;
		int dataMessagesSentCounter;
		long dataSizeSentCounter;
		uint dataMessagesSentChecksum;

		internal ArrayDataWriter accumulatedMessagesToSend = new ArrayDataWriter();

		//sent messages queue for reconnection support
		internal long sentMessagesQueueSize;
		internal EConcurrentQueue<SentMessage> sentMessages = new EConcurrentQueue<SentMessage>();

		internal uint receivedMessagesLastNumber;

		//roundtrip time measurement
		uint roundtripTimeMeasurementLastSentMessageNumber;
		List<RoundtripTimeMeasurement> roundtripTimeMeasurements = new List<RoundtripTimeMeasurement>();
		DateTime roundtripTimeMeasurementsLastReceiveTime;

		DateTime sentPingLastTime;

		ClientNetworkService_Internal networkServiceInternal;

		///////////////////////////////////////////////

		public class ProfilerDataClass
		{
			//const data
			public DateTime TimeStarted;
			public double WorkingTime;
			public long SystemMessagesReceivedStartCounter;
			public long SystemMessagesSentStartCounter;

			//dynamic data
			public long TotalReceivedMessages;
			public long TotalReceivedSize;
			public long TotalSentMessages;
			public long TotalSentSize;
			public List<ServiceItem> Services = new List<ServiceItem>();

			/////////////////////

			public class ServiceItem
			{
				public List<MessageTypeItem> MessagesByType = new List<MessageTypeItem>();

				//

				public class MessageTypeItem
				{
					public long ReceivedMessages;
					public long ReceivedSize;
					public long SentMessages;
					public long SentSize;

					public struct CustomData
					{
						public long Messages;
						public long Size;
					}
					public Dictionary<string, CustomData> ReceivedCustomData;
					public Dictionary<string, CustomData> SentCustomData;
				}

				//

				public MessageTypeItem GetMessageTypeItem( int identifier )
				{
					while( identifier >= MessagesByType.Count )
						MessagesByType.Add( null );
					var item = MessagesByType[ identifier ];
					if( item == null )
					{
						item = new MessageTypeItem();
						MessagesByType[ identifier ] = item;
					}
					return item;
				}
			}

			/////////////////////

			public ServiceItem GetServiceItem( int identifier )
			{
				while( identifier >= Services.Count )
					Services.Add( null );
				var item = Services[ identifier ];
				if( item == null )
				{
					item = new ServiceItem();
					Services[ identifier ] = item;
				}
				return item;
			}
		}

		///////////////////////////////////////////////

		public class ReceivedMessage
		{
			////public string DataString;
			public byte[] DataBinary;
			public string CloseReason;
			public ConnectionCloseStatus CloseCode;
			public string ErrorMessage;
		}

		///////////////////////////////////////////////

		public class ToProcessMessage
		{
			public bool DataBinaryCommand;
			public byte[] DataBinaryArray;

			public bool CloseCommand;
			public ConnectionCloseStatus CloseStatusCode;
			public string CloseReason;

			public bool ResendSentMessages;
		}

		///////////////////////////////////////////////

		public struct SentMessage
		{
			public uint MessageNumber;
			public uint Checksum;
			public ToProcessMessage Data;

			public SentMessage( uint messageNumber, uint checksum, ToProcessMessage data )
			{
				MessageNumber = messageNumber;
				Checksum = checksum;
				Data = data;
			}
		}

		///////////////////////////////////////////////

		public class RoundtripTimeMeasurement
		{
			public uint MessageNumber;
			public DateTime SendTime;
			public DateTime ReceiveTime;
		}

		///////////////////////////////////////////////

		class UdpClientListener : INetEventListener
		{
			public ClientNode Owner;
			public NetManager Client;

			//

			public void OnConnectionRequest( ConnectionRequest request )
			{
			}

			public void OnPeerConnected( NetPeer peer )
			{
				Owner.udpPeer = peer;
			}

			public void OnPeerDisconnected( NetPeer peer, DisconnectInfo disconnectInfo )
			{
				//Console.WriteLine( $"UDP OnPeerDisconnected: Reason={disconnectInfo.Reason}, SocketErrorCode={disconnectInfo.SocketErrorCode}" );
				//return;

				if( Owner.Disposed || Owner.backgroundTaskNeedExit )
					return;
				if( Owner.udpPeer == null )
					return;

				if( ReferenceEquals( Owner.udpPeer, peer ) )
					Owner.udpPeer = null;

				var reason = "";
				var data = disconnectInfo.AdditionalData;
				if( data != null && data.UserDataSize != 0 )
					reason = Encoding.UTF8.GetString( data.RawData, data.UserDataOffset, data.UserDataSize );

				var statusDescription = disconnectInfo.Reason.ToString();
				if( !string.IsNullOrEmpty( reason ) )
					statusDescription = reason;

				var closeStatus = disconnectInfo.Reason == DisconnectReason.InvalidProtocol ? ConnectionCloseStatus.ProtocolError : ConnectionCloseStatus.NormalClosure;

				Owner.OnClose( closeStatus, statusDescription );
			}

			public void OnNetworkError( IPEndPoint endPoint, SocketError socketErrorCode )
			{
				//Console.WriteLine( $"UDP OnNetworkError: EndPoint={endPoint}, SocketErrorCode={socketErrorCode}" );
				//return;

				if( Owner.Disposed || Owner.backgroundTaskNeedExit || Owner.status == NetworkStatus.Disconnected )
					return;

				var reason = $"UDP network error: {socketErrorCode}.";
				Owner.OnClose( ConnectionCloseStatus.ProtocolError, reason );
			}

			public void OnNetworkReceive( NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod )
			{
				if( Owner.Disposed || Owner.backgroundTaskNeedExit || Owner.status == NetworkStatus.Disconnected )
					return;

				try
				{
					//check for max receive message size
					if( reader.UserDataSize > Owner.ReceiveMessageMaxSize )
					{
						//exceed max message size
						Owner.OnClose( ConnectionCloseStatus.MessageTooBig, "The received message size exceeded the maximum allowed." );
						return;
					}

					ConnectionMessageType messageType;
					if( channelNumber == 0 )
						messageType = ConnectionMessageType.Binary;
					else if( channelNumber == 1 )
						messageType = ConnectionMessageType.Text;
					else
					{
						var error = $"Invalid channel number: {channelNumber}.";
						Owner.OnClose( ConnectionCloseStatus.ProtocolError, error );
						return;
					}

					var segment = new ArraySegment<byte>( reader.RawData, reader.UserDataOffset, reader.UserDataSize );
					Owner.ProcessReceivedMessage( messageType, segment );
				}
				catch( Exception e )
				{
					if( trace )
						Log.Info( "OnNetworkReceive exception when processing received message: " + e.ToString() );

					Owner.OnClose( ConnectionCloseStatus.ProtocolError, "Unable to process received message. " + e.Message );
				}
			}

			public void OnNetworkReceiveUnconnected( IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType )
			{
			}

			public void OnNetworkLatencyUpdate( NetPeer peer, int latency )
			{
			}

			//void INetEventListener.OnMessageDelivered( NetPeer peer, object userData )
			//{
			//}

			//void INetEventListener.OnNtpResponse( NtpPacket packet )
			//{
			//}

			//void INetEventListener.OnPeerAddressChanged( NetPeer peer, IPEndPoint previousAddress )
			//{
			//}
		}

		///////////////////////////////////////////////

		public enum ConnectionTypeEnum
		{
			None,
			WebSocket,
			UDP,
		}

		///////////////////////////////////////////////

		public delegate void ProtocolErrorDelegate( ClientNode sender, string message );
		public event ProtocolErrorDelegate ProtocolError;

		public delegate void ConnectionStatusChangedDelegate( ClientNode sender );
		public event ConnectionStatusChangedDelegate ConnectionStatusChanged;

		///////////////////////////////////////////////

		public static ClientNode[] GetInstances()
		{
			lock( instances )
				return instances.ToArray();
		}

		///////////////////////////////////////////////

		public virtual void Dispose()
		{
			//DisposeWebSocketClient();
			//DisposeUdpClient();

			//wait to exit from background tasks
			{
				var connectAndSendingTask2 = connectAndSendingTask;
				var receiveMessagesTask2 = receiveMessagesTask;
				if( connectAndSendingTask2 != null || receiveMessagesTask2 != null )
				{
					backgroundTaskNeedExit = true;
					if( ( webSocketClient != null || udpListener != null ) && Status != NetworkStatus.Disconnected )
						backgroundTasksNeedExitSendClose = true;
					backgroundTaskSemaphore.Set();

					if( connectAndSendingTask2 != null )
						for( var counter = 0; ( !connectAndSendingTask2.IsCompleted || backgroundTasksNeedExitSendClose ) && counter < 500; counter++ )
							Thread.Sleep( 1 );
					if( receiveMessagesTask2 != null )
						for( var counter = 0; !receiveMessagesTask2.IsCompleted && counter < 500; counter++ )
							Thread.Sleep( 1 );

					connectAndSendingTask = null;
					receiveMessagesTask = null;
				}
			}

			status = NetworkStatus.Disconnected;

			DisposeWebSocketClient();
			DisposeUdpClient();

			//dispose services
			foreach( var service in services.ToArray().GetReverse() )
				service.PerformDispose();
			//services.Clear();

			backgroundTaskSemaphore?.Dispose();

			lock( instances )
				instances.Remove( this );

			disposed = true;
		}

		public bool Disposed
		{
			get { return disposed; }
		}

		public void Update( DateTime utcNow )
		{
			OnUpdate( utcNow );
		}

		public ProfilerDataClass ProfilerData
		{
			get { return profilerData; }
		}

		public void ProfilerStart( double workingTime )
		{
			ProfilerStop( false );
			profilerData = new ProfilerDataClass();
			profilerData.TimeStarted = DateTime.UtcNow;
			profilerData.WorkingTime = workingTime;
			profilerData.SystemMessagesReceivedStartCounter = DataMessagesReceivedCounter;
			profilerData.SystemMessagesSentStartCounter = DataMessagesSentCounter;

			Log.Info( "Network profiler started." );
		}

		public void ProfilerStop( bool writeToLogs )
		{
			if( profilerData != null )
			{
				var workedTime = ( DateTime.UtcNow - profilerData.TimeStarted ).TotalSeconds;
				if( workedTime > 0 )
				{
					var workedTimeString = workedTime.ToString( "F1" );
					Log.Info( $"Network profiler stopped after {workedTimeString} seconds." );
				}
				else
					Log.Info( $"Network profiler stopped." );

				if( writeToLogs )
					DumpProfilerDataToLogs();

				profilerData = null;
			}
		}

		static string FormatCount( long count )
		{
			return count.ToString( "N0" );
		}

		void DumpProfilerDataToLogs()
		{
			var systemMessagesReceived = DataMessagesReceivedCounter - profilerData.SystemMessagesReceivedStartCounter;
			var systemMessagesSent = DataMessagesSentCounter - profilerData.SystemMessagesSentStartCounter;

			var lines = new List<string>();

			lines.Add( "--------------------------------------------------------------" );
			lines.Add( string.Format( "Total received; {0}", StringUtility.FormatSize( profilerData.TotalReceivedSize ) ) );

			lines.Add( string.Format( "System messages received; {0}", systemMessagesReceived ) );

			for( int serviceId = 0; serviceId < profilerData.Services.Count; serviceId++ )
			{
				var serviceItem = profilerData.Services[ serviceId ];
				if( serviceItem != null )
				{
					var service = GetService( serviceId );
					lines.Add( string.Format( "> {0}", service.Name ) );

					var messageByTypeItems = new List<(ProfilerDataClass.ServiceItem.MessageTypeItem, int)>();

					for( int messageTypeId = 0; messageTypeId < serviceItem.MessagesByType.Count; messageTypeId++ )
					{
						var messageType = service.GetMessageType( messageTypeId );
						if( messageType != null )
						{
							var messageByTypeItem = serviceItem.GetMessageTypeItem( messageTypeId );
							if( messageByTypeItem != null && messageByTypeItem.ReceivedMessages != 0 )
								messageByTypeItems.Add( (messageByTypeItem, messageTypeId) );
						}
					}

					CollectionUtility.MergeSort( messageByTypeItems, delegate ( (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item1, (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item2 )
					{
						if( item1.Item1.ReceivedSize > item2.Item1.ReceivedSize )
							return -1;
						if( item1.Item1.ReceivedSize < item2.Item1.ReceivedSize )
							return 1;
						return 0;
					} );

					foreach( var messageByTypeItemPair in messageByTypeItems )
					{
						var messageByTypeItem = messageByTypeItemPair.Item1;
						var messageTypeId = messageByTypeItemPair.Item2;

						var messageType = service.GetMessageType( messageTypeId );

						lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.ReceivedMessages ), StringUtility.FormatSize( messageByTypeItem.ReceivedSize ) ) );

						var customData = messageByTypeItem.ReceivedCustomData;
						if( customData != null )
						{
							var items = new List<(string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData)>( customData.Count );
							foreach( var item in customData )
								items.Add( (item.Key, item.Value) );

							CollectionUtility.MergeSort( items, delegate ( (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item1, (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item2 )
							{
								if( item1.Item2.Size > item2.Item2.Size )
									return -1;
								if( item1.Item2.Size < item2.Item2.Size )
									return 1;
								return 0;
							} );

							foreach( var item in items )
							{
								lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), StringUtility.FormatSize( item.Item2.Size ) ) );
							}
						}
					}
				}
			}

			lines.Add( "--------------------------------------------------------------" );
			lines.Add( string.Format( "Total sent; Size: {0}", StringUtility.FormatSize( profilerData.TotalSentSize ) ) );

			lines.Add( string.Format( "System messages sent; {0}", systemMessagesSent ) );

			for( int serviceId = 0; serviceId < profilerData.Services.Count; serviceId++ )
			{
				var serviceItem = profilerData.Services[ serviceId ];
				if( serviceItem != null )
				{
					var service = GetService( serviceId );
					lines.Add( string.Format( "> {0}", service.Name ) );

					var messageByTypeItems = new List<(ProfilerDataClass.ServiceItem.MessageTypeItem, int)>();

					for( int messageTypeId = 0; messageTypeId < serviceItem.MessagesByType.Count; messageTypeId++ )
					{
						var messageType = service.GetMessageType( messageTypeId );
						if( messageType != null )
						{
							var messageByTypeItem = serviceItem.GetMessageTypeItem( messageTypeId );
							if( messageByTypeItem != null && messageByTypeItem.SentMessages != 0 )
								messageByTypeItems.Add( (messageByTypeItem, messageTypeId) );
						}
					}

					CollectionUtility.MergeSort( messageByTypeItems, delegate ( (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item1, (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item2 )
					{
						if( item1.Item1.SentSize > item2.Item1.SentSize )
							return -1;
						if( item1.Item1.SentSize < item2.Item1.SentSize )
							return 1;
						return 0;
					} );

					foreach( var messageByTypeItemPair in messageByTypeItems )
					{
						var messageByTypeItem = messageByTypeItemPair.Item1;
						var messageTypeId = messageByTypeItemPair.Item2;

						var messageType = service.GetMessageType( messageTypeId );

						lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.SentMessages ), StringUtility.FormatSize( messageByTypeItem.SentSize ) ) );

						var customData = messageByTypeItem.SentCustomData;
						if( customData != null )
						{
							var items = new List<(string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData)>( customData.Count );
							foreach( var item in customData )
								items.Add( (item.Key, item.Value) );

							CollectionUtility.MergeSort( items, delegate ( (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item1, (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item2 )
							{
								if( item1.Item2.Size > item2.Item2.Size )
									return -1;
								if( item1.Item2.Size < item2.Item2.Size )
									return 1;
								return 0;
							} );

							foreach( var item in items )
							{
								lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), StringUtility.FormatSize( item.Item2.Size ) ) );
							}
						}
					}
				}
			}

			lines.Add( "--------------------------------------------------------------" );

			var result = "";
			foreach( var line in lines )
			{
				if( result != "" )
					result += "\r\n";
				result += line;
			}
			Log.Info( result );
		}

		///////////////////////////////////////////////

		protected ClientNode()
		{
			lock( instances )
				instances.Add( this );

			servicesReadOnly = new ReadOnlyCollection<ClientService>( services );

			networkServiceInternal = new ClientNetworkService_Internal();
			RegisterService( networkServiceInternal );
		}

		/// <summary>
		/// Begins an asynchronous connection to the specified server using WebSocket and UDP ports.
		/// </summary>
		/// <param name="https">true to use HTTPS for the connection; otherwise, false.</param>
		/// <param name="host">The hostname or IP address of the server to connect to. Cannot be null or empty.</param>
		/// <param name="portWebSocket">The port number to use for the WebSocket connection. Must be a valid port number or 0.</param>
		/// <param name="portUdp">The port number to use for the UDP connection. Must be a valid port number or 0.</param>
		/// <param name="clientVersion">The version string identifying the client. Used for compatibility checks with the server.</param>
		/// <param name="loginData">The login data or credentials to send to the server during connection initialization.</param>
		/// <param name="error">When this method returns, contains an error message if the connection could not be started; otherwise, null.</param>
		/// <returns>true if the connection process was successfully started; otherwise, false.</returns>
		public bool BeginConnect( bool https, string host, int portWebSocket, int portUdp, string clientVersion, string loginData, out string error )
		{
			this.clientVersion = clientVersion;
			this.loginData = loginData;

			if( Disposed )
				Log.Fatal( "NetworkClient: BeginConnect: The client is disposed." );
			if( webSocketClient != null )
				Log.Fatal( "NetworkClient: BeginConnect: The client is already initialized." );
			if( string.IsNullOrEmpty( host ) )
				Log.Fatal( "NetworkClient: BeginConnect: \"host\" is empty." );
			if( portWebSocket == 0 && portUdp == 0 )
				Log.Fatal( "NetworkClient: BeginConnect: Both WebSocket and UDP ports cannot be 0." );

			disconnectionReason = "";

			string welcomeBase64;
			{
				var rootBlock = new TextBlock();
				rootBlock.SetAttribute( "ClientVersion", clientVersion );
				rootBlock.SetAttribute( "LoginData", loginData );
				var welcome = rootBlock.DumpToString( false );
				welcomeBase64 = StringUtility.EncodeToBase64URL( welcome );
			}

			clientConnectHttps = https;
			clientConnectHost = host;
			clientConnectPortWebSocket = portWebSocket;

			//web socket client
			if( portWebSocket != 0 )
			{
				var prefix = https ? "wss" : "ws";
				clientConnectAddressWebSocket = $"{prefix}://{host}:{portWebSocket}/service/?welcome={welcomeBase64}";

				try
				{
					webSocketClient = new ClientWebSocket();

					////? disable compression
					//webSocketClient.Options.DangerousDeflateOptions = new WebSocketDeflateOptions
					//{
					//	ClientContextTakeover = false,
					//	ServerContextTakeover = false,
					//	//ClientMaxWindowBits = 15,
					//	//ServerMaxWindowBits = 15
					//};

					////enable compression
					//webSocketClient.Options.DangerousDeflateOptions = new WebSocketDeflateOptions
					//{
					//	ClientContextTakeover = true,
					//	ServerContextTakeover = true,
					//	//ClientMaxWindowBits = 15,
					//	//ServerMaxWindowBits = 15
					//};
				}
				catch( Exception e )
				{
					error = e.Message;
					return false;
				}
			}

			//udp client
			if( portUdp != 0 )
			{
				try
				{
					clientConnectPortUdp = portUdp;

					var listener = new UdpClientListener();
					var client = new NetManager( listener );
					listener.Owner = this;
					listener.Client = client;
					udpListener = listener;

					client.AutoRecycle = true;
					client.MtuOverride = 1200;
					//MtuDiscovery. to change MtuDiscovery consider UpdateUdpMaxFragmentsCount
					UpdateUdpMaxFragmentsCount_DisconnectTimeout_UpdateTime();
					client.UnsyncedEvents = true;
					client.UnsyncedReceiveEvent = true;
					client.UnsyncedDeliveryEvent = true;
					//UseNativeSockets


					//!!!!
					//server.PacketPoolSize

					//!!!!
					//socket.ReceiveBufferSize = NetConstants.SocketBufferSize;
					//socket.SendBufferSize = NetConstants.SocketBufferSize;


					client.ReconnectDelay = 2000;
					client.MaxConnectAttempts = Math.Max( (int)( ConnectingMaxTimeInSeconds * 1000 / client.ReconnectDelay ), 1 );

					if( !client.Start( portUdp ) )
						throw new Exception( "The UDP client is not started." );
				}
				catch( Exception e )
				{
					error = e.Message;
					return false;
				}
			}

			status = NetworkStatus.Connecting;

			connectAndSendingTask = TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Forever, "ClientNode: BeginConnect: ConnectAndSendingTask", ConnectAndSendingTask );

			error = null;
			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		async Task ConnectAndSendingTask()
		{
			//first connect

			//try connect by mean UDP
			var udpConnected = false;
			if( clientConnectPortUdp != 0 )
			{
				connectionType = ConnectionTypeEnum.UDP;

				string welcomeBase64;
				{
					var rootBlock = new TextBlock();
					rootBlock.SetAttribute( "ClientVersion", clientVersion );
					rootBlock.SetAttribute( "LoginData", loginData );
					var welcome = rootBlock.DumpToString( false );
					welcomeBase64 = StringUtility.EncodeToBase64URL( welcome );
				}

				var connectPeer = udpListener?.Client.Connect( clientConnectHost, clientConnectPortUdp, welcomeBase64 );
				if( connectPeer == null )
				{
					if( clientConnectPortWebSocket == 0 )
					{
						disconnectionReason = "UDP connection request was rejected immediately.";
						if( status != NetworkStatus.Disconnected )
						{
							status = NetworkStatus.Disconnected;
							OnConnectionStatusChanged();
						}
						return;
					}
				}
				else
				{
					while( true )
					{
						//check for connection established
						if( udpPeer != null )
						{
							//connection established
							udpConnected = true;
							DisposeWebSocketClient();
							break;
						}

						//check client disposed
						if( Disposed || backgroundTaskNeedExit )
						{
							if( string.IsNullOrEmpty( disconnectionReason ) )
								disconnectionReason = "Connection attempt canceled.";
							return;
						}

						//check for connection is not established
						if( connectPeer.ConnectionState == ConnectionState.Disconnected )
						{
							if( clientConnectPortWebSocket == 0 )
							{
								disconnectionReason = "Connection attempt timed out.";
								if( status != NetworkStatus.Disconnected )
								{
									status = NetworkStatus.Disconnected;
									OnConnectionStatusChanged();
								}
								return;
							}
							else
								break;
						}

						await Task.Delay( 5 );
					}
				}
			}

			//try connect by mean web socket
			if( !udpConnected && clientConnectPortWebSocket != 0 )
			{
				connectionType = ConnectionTypeEnum.WebSocket;

				var firstDisconnectionReason = "";
				var connectAttemptsStartTime = DateTime.UtcNow;

				while( true )
				{
					try
					{
						var remainingTime = ConnectingMaxTimeInSeconds - ( DateTime.UtcNow - connectAttemptsStartTime ).TotalSeconds;
						if( remainingTime < 1 )
							remainingTime = 1;

						using var cts = new CancellationTokenSource( TimeSpan.FromSeconds( remainingTime ) );
						await webSocketClient.ConnectAsync( new Uri( clientConnectAddressWebSocket ), cts.Token );

						//connection established
						break;
					}
					catch( Exception e )
					{
						if( string.IsNullOrEmpty( firstDisconnectionReason ) )
							firstDisconnectionReason = "ConnectAsync exception: " + e.ToString(); //e.Message;

						//check client disposed
						if( Disposed || backgroundTaskNeedExit )
						{
							disconnectionReason = firstDisconnectionReason;
							return;
						}

						//check for connection is not established
						var utcNow = DateTime.UtcNow;
						if( ( utcNow - connectAttemptsStartTime ).TotalSeconds > ConnectingMaxTimeInSeconds )
						{
							disconnectionReason = firstDisconnectionReason;
							if( status != NetworkStatus.Disconnected )
							{
								status = NetworkStatus.Disconnected;
								OnConnectionStatusChanged();
							}
							return;
						}

						await Task.Delay( 1000 );
					}
				}
			}

			//start receiving messages task for web socket after connection established
			if( ConnectionType == ConnectionTypeEnum.WebSocket )
			{
				receiveMessagesTask = TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Forever, "ClientNode: BeginConnect: ReceiveMessagesTask", ReceiveMessagesTaskWebSocketAsync );
			}

			try
			{
				//process messages
				while( !Disposed && !backgroundTaskNeedExit )
				{
					//send messages
					while( toProcessMessages.TryDequeue( out var message ) )
					{
						if( message.DataBinaryCommand )
						{
							//send data message

							var data = message.DataBinaryArray;

							//overload of toProcessMessages queue
							if( data == null )
							{
								OnClose( ConnectionCloseStatus.ProtocolError, "The process messages queue is overloaded." );
								return;
							}

							//decrease total size of toProcessMessages queue
							Interlocked.Add( ref toProcesssMessagesTotalBinaryDataSize, -data.Length );

							try
							{
								//update counters and checksum
								unchecked
								{
									Interlocked.Increment( ref dataMessagesSentCounter );
									Interlocked.Add( ref dataSizeSentCounter, data.Length );
									dataMessagesSentChecksum += ChecksumAppend( data );
								}

								//update headers
								unsafe
								{
									fixed( byte* p = data )
									{
										*(uint*)p = (uint)dataMessagesSentCounter;
										*(uint*)( p + 4 ) = dataMessagesSentChecksum;
									}
								}

								if( trace )
									Log.Info( $"Send Binary {data.Length} {dataMessagesSentCounter} {dataMessagesSentChecksum}" );

								//add to sent messages queue for reconnecting
								if( AllowReconnect && ConnectionType == ConnectionTypeEnum.WebSocket )
								{
									Interlocked.Add( ref sentMessagesQueueSize, data.Length );
									sentMessages.Enqueue( new SentMessage( (uint)dataMessagesSentCounter, dataMessagesSentChecksum, message ) );

									//maybe SendMessageMaxSize * 2?

									//check limit. max buffer size is same as SendMessageMaxSize
									if( sentMessagesQueueSize > SendMessageMaxSize )
									{
										OnClose( ConnectionCloseStatus.ProtocolError, "The sent messages queue size exceeded the maximum allowed." );
										return;
									}
								}

								//Log.Info( "SendAsync Before." );

								//try to send message
								var webSocketClient2 = webSocketClient;
								if( webSocketClient2 != null )
								{
									using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
#if NETSTANDARD2_1
									await webSocketClient2.SendAsync( data, WebSocketMessageType.Binary, true, cts.Token );
#else
									await webSocketClient2.SendAsync( data, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage | WebSocketMessageFlags.DisableCompression, cts.Token );
#endif
								}

								var udpPeer2 = udpPeer;
								if( udpPeer2 != null )
									udpPeer2.Send( data, 0, DeliveryMethod.ReliableOrdered );
							}
							catch( Exception e )
							{
								if( trace )
									Log.Info( "OnMessage Binary exception when sending data message: " + e.Message );

								if( AllowReconnect && ConnectionType == ConnectionTypeEnum.WebSocket )
								{
									SetConnectionIsOldAndDisposeWebSocketClient();
									break;
								}
								else
								{
									OnClose( ConnectionCloseStatus.ProtocolError, "Unable to send the binary message. " + e.Message );
									//OnClose( ConnectionCloseStatus.ProtocolError, "Unable to send the binary message. " + e.ToString() );
									return;
								}
							}
						}
						else if( message.CloseCommand )
						{
							//receive close message
							OnClose( message.CloseStatusCode, message.CloseReason );
							return;
						}
						else if( message.ResendSentMessages )
						{
							//resend sent messages after reconnection

							try
							{
								foreach( var sentMessage in sentMessages )
								{
									//Log.Info( "SendAsync Before (SentMessages)." );

									var webSocketClient2 = webSocketClient;
									if( webSocketClient2 != null )
									{
										using var cts = new CancellationTokenSource( TimeSpan.FromSeconds( Math.Max( KeepAliveTime, 60 ) ) );
#if NETSTANDARD2_1
										await webSocketClient2.SendAsync( sentMessage.Data.DataBinaryArray, WebSocketMessageType.Binary, true, cts.Token );
#else
										await webSocketClient2.SendAsync( sentMessage.Data.DataBinaryArray, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage | WebSocketMessageFlags.DisableCompression, cts.Token );
#endif
									}

									var udpPeer2 = udpPeer;
									if( udpPeer2 != null )
										udpPeer2.Send( sentMessage.Data.DataBinaryArray, 0, DeliveryMethod.ReliableOrdered );

									//Log.Info( "SendAsync After (SentMessages)." );
								}
							}
							catch( Exception e )
							{
								if( trace )
									Log.Info( "OnMessage Binary exception when sending ResendSentMessages command: " + e.Message );

								if( AllowReconnect && ConnectionType == ConnectionTypeEnum.WebSocket )
								{
									SetConnectionIsOldAndDisposeWebSocketClient();
									break;
								}
								else
								{
									OnClose( ConnectionCloseStatus.ProtocolError, "Unable to send the ResendSentMessages command message. " + e.Message );
									return;
								}
							}
						}

						if( Disposed || backgroundTaskNeedExit )
							break;
					}

					//try to reconnect
					if( connectionIsOld && AllowReconnect && ConnectionType == ConnectionTypeEnum.WebSocket )
					{
						while( !Disposed && !backgroundTaskNeedExit )
						{
							try
							{
								//Log.Info( "Request reconnect. Token: " + reconnectTokenFromServer );

								clientConnectAddressWebSocket = $"ws://{clientConnectHost}:{clientConnectPortWebSocket}/service/?reconnect_token={reconnectTokenFromServer}";

								webSocketClient = new ClientWebSocket();

								////? disable compression
								//webSocketClient.Options.DangerousDeflateOptions = new WebSocketDeflateOptions
								//{
								//	ClientContextTakeover = false,
								//	ServerContextTakeover = false,
								//	//ClientMaxWindowBits = 15,
								//	//ServerMaxWindowBits = 15
								//};

								////enable compression
								//webSocketClient.Options.DangerousDeflateOptions = new WebSocketDeflateOptions
								//{
								//	ClientContextTakeover = true,
								//	ServerContextTakeover = true,
								//	//ClientMaxWindowBits = 15,
								//	//ServerMaxWindowBits = 15
								//};

								using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
								await webSocketClient.ConnectAsync( new Uri( clientConnectAddressWebSocket ), cts.Token );

								//Log.Info( "Request reconnect 2." );

								//connection restored now
								noRealConnectionStartTime = default;
								connectionIsOld = false;

								//resend queued messages
								ToProcessMessagesEnqueue( new ToProcessMessage { ResendSentMessages = true } );

								//exit from the reconnect loop
								break;
							}
							catch( Exception e )
							{
								if( trace )
									Log.Info( "Exception when reconnecting: " + e.Message );
							}

							var noRealConnectionStartTime2 = noRealConnectionStartTime;
							if( noRealConnectionStartTime2 != default )
							{
								var utcNow = DateTime.UtcNow;
								if( ( utcNow - noRealConnectionStartTime2 ).TotalSeconds >= KeepAliveTime )
								{
									//time is out, close connection
									OnClose( ConnectionCloseStatus.NormalClosure, null );
									return;
								}
							}

							await Task.Delay( 5000 );
						}
					}

					if( Disposed || backgroundTaskNeedExit )
						break;

					//wait for new messages to send or reconnect
					await backgroundTaskSemaphore.WaitAsync();
				}
			}
			catch( Exception e )
			{
				Log.Warning( "ClientNode: ConnectAndSendingTask exception: " + e.ToString() );
			}

			//send close before dispose
			if( backgroundTaskNeedExit && backgroundTasksNeedExitSendClose )
			{
				try
				{
					await CloseAsync( ConnectionCloseStatus.NormalClosure, null );
				}
				catch { }
			}
		}

		void OnClose( ConnectionCloseStatus closeStatus, string statusDescription )
		{
			receivedMessages.Enqueue( new ReceivedMessage { CloseReason = statusDescription, CloseCode = closeStatus } );

			if( trace )
			{
				var statusDescription2 = statusDescription ?? "(No status)";
				Log.Info( $"OnClose {closeStatus} {statusDescription2}" );
			}
		}

		void ProcessReceivedMessage( ConnectionMessageType messageType, ArraySegment<byte> buffer )
		{
			if( messageType == ConnectionMessageType.Text )
			{
				//system commands

				if( trace )
					Log.Info( "OnMessage Text" );

				try
				{
					var text = Encoding.UTF8.GetString( buffer );
					if( text.Length > 1000 )
						throw new Exception( "The system message is more than 1000 characters." );

					var rootBlock = TextBlock.Parse( text, out var error );
					if( !string.IsNullOrEmpty( error ) )
						throw new Exception( error );

					var command = rootBlock.GetAttribute( "C" );

					if( command == "Settings" )
					{
						if( double.TryParse( rootBlock.GetAttribute( "KeepAliveTime", "60" ), out var keepAliveTime2 ) )
						{
							keepAliveTimeFromServer = keepAliveTime2;

							var udpListener2 = udpListener;
							if( udpListener2 != null )
								udpListener2.Client.DisconnectTimeout = (int)( KeepAliveTime * 1000 );
						}
						if( rootBlock.AttributeExists( "ReconnectToken" ) )
							reconnectTokenFromServer = rootBlock.GetAttribute( "ReconnectToken" );

						if( trace )
							Log.Info( $"OnMessage Text Settings; KeepAliveTime={KeepAliveTime}; AllowReconnect={AllowReconnect}" );
					}
					else
						throw new Exception( "Unknown command." );
				}
				catch( Exception e )
				{
					if( trace )
						Log.Info( "OnMessage Text Exception: " + e.Message );
					OnClose( ConnectionCloseStatus.ProtocolError, e.Message );
					return;
				}
			}
			else if( messageType == ConnectionMessageType.Binary )
			{
				//data commands

				if( buffer.Count < 8 )
				{
					if( trace )
						Log.Info( "OnMessage Binary: The binary message size is less than 8 bytes." );
					OnClose( ConnectionCloseStatus.ProtocolError, "The binary message size is less than 8 bytes." );
					return;
				}

				var reader = new ArrayDataReader( buffer.Array, buffer.Offset, buffer.Count );
				var messageNumber = reader.ReadUInt32();
				var checksum = reader.ReadUInt32();

				var length = buffer.Count - 8;

				//!!!!need copy data because buffer will be reused. can make pool of buffers. limit the total size of buffers.
				//GC. if manage arrays by self, maybe need to worry about memory limitations
				//use array pool?
				var data = new byte[ length ];
				reader.ReadBuffer( data, 0, length );

				if( reader.Overflow )
				{
					if( trace )
						Log.Info( "OnMessage Binary: Invalid binary message. reader.Overflow." );
					OnClose( ConnectionCloseStatus.ProtocolError, "Invalid binary message." );
					return;
				}

				if( messageNumber == receivedMessagesLastNumber + 1 )
				{
					receivedMessagesLastNumber++;

					unchecked
					{
						Interlocked.Increment( ref dataMessagesReceivedCounter );
						Interlocked.Add( ref dataSizeReceivedCounter, length );
						dataMessagesReceivedChecksum += ChecksumAppend( data );
					}

					if( trace )
						Log.Info( $"OnMessage Binary {length} {DataMessagesReceivedCounter} {dataMessagesReceivedChecksum}" );

					//compare checksums
					if( DataMessagesReceivedCounter != messageNumber || dataMessagesReceivedChecksum != checksum )
					{
						if( trace )
							Log.Info( $"OnMessage Binary: Invalid checksum. {DataMessagesReceivedCounter} != {messageNumber} || {dataMessagesReceivedChecksum} != {checksum}" );
						OnClose( ConnectionCloseStatus.ProtocolError, $"Invalid checksum. {DataMessagesReceivedCounter} != {messageNumber} || {dataMessagesReceivedChecksum} != {checksum}" );
						return;
					}

					//enqueue message
					receivedMessages.Enqueue( new ReceivedMessage { DataBinary = data } );

					////send back message number. it is used to confirm message received
					//if( AllowReconnect )
					//	networkServiceInternal.SendMessageProcessedToServer( receivedMessagesLastNumber );
				}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		async Task ReceiveMessagesTaskWebSocketAsync()
		{
			while( !Disposed && !backgroundTaskNeedExit && status != NetworkStatus.Disconnected )
			{
				try
				{
					var accumulatedBuffer = new ArrayDataWriter( 1024 );
					var buffer = new byte[ 1024 * 64 ];

					while( true )
					{
						var webSocketClient2 = webSocketClient;
						if( webSocketClient2 == null )
							break;
						if( webSocketClient2.State == WebSocketState.None || webSocketClient2.State == WebSocketState.Connecting )
							break;

						var result = await webSocketClient2.ReceiveAsync( new ArraySegment<byte>( buffer ), CancellationToken.None );

						if( result.MessageType != WebSocketMessageType.Close )
						{
							//check for max receive message size
							if( accumulatedBuffer.Length + result.Count > ReceiveMessageMaxSize )
							{
								//exceed max message size
								OnClose( ConnectionCloseStatus.MessageTooBig, "The received message size exceeded the maximum allowed." );
								break;
							}

							accumulatedBuffer.Write( buffer, 0, result.Count );

							if( result.EndOfMessage )
							{
								ProcessReceivedMessage( (ConnectionMessageType)result.MessageType, accumulatedBuffer.AsArraySegment() );
								accumulatedBuffer.Reset();
							}
						}
						else
						{
							//with status description can't reconnect, because server connection is closed with error reason (not just disconnection)
							if( AllowReconnect && string.IsNullOrEmpty( result.CloseStatusDescription ) )
								SetConnectionIsOldAndDisposeWebSocketClient();
							else
								OnClose( ConnectionCloseStatus.NormalClosure, result.CloseStatusDescription );
							break;
						}
					}
				}
				catch( Exception e )
				{
					if( AllowReconnect )
						SetConnectionIsOldAndDisposeWebSocketClient();
					else
					{
						//!!!!maybe exists better solution to detect normal close

						if( e.Message.Contains( "The remote party closed the WebSocket connection without completing the close handshake." ) )
							OnClose( ConnectionCloseStatus.NormalClosure, null );
						else
							OnClose( ConnectionCloseStatus.ProtocolError, "Unable to receive messages. " + e.Message );
					}
				}

				if( webSocketClient == null )
					await Task.Delay( 5000 );
				else
					await Task.Delay( 10 );
			}
		}

		public NetworkStatus Status
		{
			get { return status; }
		}

		public ConnectionTypeEnum ConnectionType
		{
			get { return connectionType; }
		}

		public string ClientVersion
		{
			get { return clientVersion; }
		}

		public string LoginData
		{
			get { return loginData; }
		}

		public double KeepAliveTime
		{
			get { return keepAliveTimeFromServer; }
		}

		public bool AllowReconnect
		{
			get { return AllowReconnectFromClient && !string.IsNullOrEmpty( reconnectTokenFromServer ); }
		}

		void SendPingUpdate( DateTime utcNow )
		{
			if( Status == NetworkStatus.Connected )
			{
				lock( roundtripTimeMeasurements )
				{
					roundtripTimeMeasurementLastSentMessageNumber++;

					if( roundtripTimeMeasurements.Count > 3 )
						roundtripTimeMeasurements.RemoveAt( 0 );

					roundtripTimeMeasurements.Add( new RoundtripTimeMeasurement { SendTime = utcNow, MessageNumber = roundtripTimeMeasurementLastSentMessageNumber } );

					networkServiceInternal.SendPingToServer( roundtripTimeMeasurementLastSentMessageNumber, receivedMessagesLastNumber );
				}
			}
		}

		void ProcessAccumulatedMessagesToSend()
		{
			lock( accumulatedMessagesToSend )
			{
				if( accumulatedMessagesToSend.Length > 0 )
				{
					//!!!!
					//GC. if manage arrays by self, maybe need to worry about memory limitations
					//use array pool?
					var array = accumulatedMessagesToSend.ToArray();

					var overloadOfToProcessMessages = toProcesssMessagesTotalBinaryDataSize > SendMessageMaxSize * 2;
					if( overloadOfToProcessMessages )
						ToProcessMessagesEnqueue( new ToProcessMessage { DataBinaryCommand = true, DataBinaryArray = null } );
					else
					{
						ToProcessMessagesEnqueue( new ToProcessMessage { DataBinaryCommand = true, DataBinaryArray = array } );
						Interlocked.Add( ref toProcesssMessagesTotalBinaryDataSize, array.Length );
					}

					accumulatedMessagesToSend.Reset();
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal int AddAccumulatedMessageToSend( ArrayDataWriter writer )
		{
			int bytesWritten;

			lock( accumulatedMessagesToSend )
			{
				var newCount = accumulatedMessagesToSend.Length + writer.Length + 4;
				if( newCount > SendMessageMaxSize )
					ProcessAccumulatedMessagesToSend();

				//add headers (message number, checksum)
				if( accumulatedMessagesToSend.Length == 0 )
				{
					accumulatedMessagesToSend.Write( 0 );
					accumulatedMessagesToSend.Write( 0 );
				}

				bytesWritten = accumulatedMessagesToSend.WriteVariableUInt32( (uint)writer.Length );
				accumulatedMessagesToSend.Write( writer.Data, 0, writer.Length );
			}

			return bytesWritten;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void OnUpdate( DateTime utcNow )
		{
			if( Disposed )
				return;

			//update udp client
			if( udpListener != null )
			{
				UpdateUdpMaxFragmentsCount_DisconnectTimeout_UpdateTime();
				udpListener?.Client.PollEvents();
			}

			//stop profiler if time is out
			{
				var profilerData2 = ProfilerData;
				if( profilerData2 != null )
				{
					var workedTime = ( utcNow - profilerData2.TimeStarted ).TotalSeconds;
					if( workedTime >= profilerData2.WorkingTime )
						ProfilerStop( true );
				}
			}

			//process received messages
			while( receivedMessages.TryDequeue( out var message ) )
			{
				if( message.DataBinary != null )
				{
					//data binary message

					var data = message.DataBinary;
					var reader = new ArrayDataReader( data );

					while( reader.CurrentPosition < reader.EndPosition )
					{
						//read and process message
						var startPosition = reader.CurrentPosition;
						var length = (int)reader.ReadVariableUInt32();
						ProcessReceivedMessage( data, reader.CurrentPosition, length );
						reader.ReadSkip( length );
						var endPosition = reader.CurrentPosition;

						//check overflow
						if( reader.Overflow )
						{
							var reason = "OnMessage: Read overflow.";
							OnReceiveProtocolErrorInternal( reason );
							ToProcessMessagesEnqueue( new ToProcessMessage { CloseCommand = true, CloseStatusCode = ConnectionCloseStatus.ProtocolError, CloseReason = reason } );
							break;
						}

						//update profiler
						var profilerData2 = profilerData;
						if( profilerData2 != null )
						{
							profilerData2.TotalReceivedMessages++;
							profilerData2.TotalReceivedSize += endPosition - startPosition;
						}
					}
				}
				else if( message.CloseReason != null )
				{
					//close message

					if( trace )
						Log.Info( "OnUpdate: Close. " + message.CloseReason + " " + message.CloseCode.ToString() );

					//set disconnection reason
					if( string.IsNullOrEmpty( disconnectionReason ) )
						disconnectionReason = message.CloseReason ?? "";

					//update status
					if( status != NetworkStatus.Disconnected )
					{
						status = NetworkStatus.Disconnected;
						OnConnectionStatusChanged();
					}

					if( message.CloseCode == ConnectionCloseStatus.ProtocolError )
						OnReceiveProtocolErrorInternal( disconnectionReason );

					TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ClientNode: OnUpdate: CloseAsync: Close message", async delegate ()
					{
						try
						{
							await CloseAsync( message.CloseCode, disconnectionReason );
						}
						catch { }
					} );
				}
				else if( message.ErrorMessage != null )
				{
					//error message

					if( trace )
						Log.Info( "OnUpdate: Error: " + message.ErrorMessage );

					//update status
					if( status != NetworkStatus.Disconnected )
					{
						status = NetworkStatus.Disconnected;
						OnConnectionStatusChanged();
					}

					OnReceiveProtocolErrorInternal( message.ErrorMessage );

					TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ClientNode: OnUpdate: CloseAsync: Error message", async delegate ()
					{
						try
						{
							await CloseAsync( ConnectionCloseStatus.ProtocolError, disconnectionReason );
						}
						catch { }
					} );
				}
			}

			//check for closed connection. drop connection if closed or no real connection long time
			if( status != NetworkStatus.Disconnected && status != NetworkStatus.Connecting ) //if( status != NetworkStatus.Disconnected )
			{
				var webSocketClient2 = webSocketClient;
				var udpPeer2 = udpPeer;

				if( ( webSocketClient2 == null || webSocketClient2.State == WebSocketState.Aborted || webSocketClient2.State == WebSocketState.Closed ) && ( udpPeer2 == null || ( udpPeer2.ConnectionState & ConnectionState.Disconnected ) != 0 ) )
				{
					var allowReconnect = AllowReconnect && ConnectionType == ConnectionTypeEnum.WebSocket;
					if( !allowReconnect || GetRoundtripLastInSeconds( utcNow ) > KeepAliveTime )
					{
						status = NetworkStatus.Disconnected;
						if( string.IsNullOrEmpty( disconnectionReason ) )
							disconnectionReason = "Connection is closed.";
						OnConnectionStatusChanged();
					}
				}
			}

			//update services
			for( int n = 0; n < services.Count; n++ )
				services[ n ].OnUpdate( utcNow );

			//send ping
			if( ( utcNow - sentPingLastTime ).TotalSeconds > sendPingIntervalInSeconds )
			{
				sentPingLastTime = utcNow;
				SendPingUpdate( utcNow );
			}

			//send accumulated messages
			ProcessAccumulatedMessagesToSend();
		}

		protected virtual void OnConnectionStatusChanged()
		{
			ConnectionStatusChanged?.Invoke( this );
		}

		public IList<ClientService> Services
		{
			get { return servicesReadOnly; }
		}

		protected void RegisterService( ClientService service )
		{
			if( service.owner != null )
				Log.Fatal( "ClientNode: RegisterService: Service is already registered." );
			if( service.Identifier < 0 )
				Log.Fatal( "ClientNode: RegisterService: Invalid service identifier. Identifier can not be zero or negative." );
			if( service.Identifier > maxServiceIdentifier )
				Log.Fatal( "ClientNode: RegisterService: Invalid service identifier. Max identifier is \"{0}\".", maxServiceIdentifier );

			//check for unique identifier
			{
				var checkService = GetService( service.Identifier );
				if( checkService != null )
					Log.Fatal( "ClientNode: RegisterService: Service with identifier \"{0}\" is already registered.", service.Identifier );
			}

			//check for unique name
			{
				var checkService = GetService( service.Name );
				if( checkService != null )
					Log.Fatal( "ClientNode: RegisterService: Service with name \"{0}\" is already registered.", service.Name );
			}

			service.owner = this;
			services.Add( service );
			servicesByIdentifier[ service.Identifier ] = service;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ClientService GetService( int identifier )
		{
			if( identifier >= servicesByIdentifier.Length )
				return null;
			return servicesByIdentifier[ identifier ];
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public virtual ClientService GetService( string name )
		{
			for( int n = 0; n < services.Count; n++ )
			{
				var service = services[ n ];
				if( service.Name == name )
					return service;
			}
			return null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public virtual T GetService<T>() where T : ClientService
		{
			for( int n = 0; n < services.Count; n++ )
			{
				var service = services[ n ];
				if( typeof( T ).IsAssignableFrom( service.GetType() ) )
					return (T)service;
			}
			return null;
		}

		internal virtual void OnReceiveProtocolErrorInternal( string message )
		{
			if( trace )
				Log.Info( $"OnReceiveProtocolErrorInternal: {message}" );

			if( !Disposed )
				ProtocolError?.Invoke( this, message );
		}

		public string DisconnectionReason
		{
			get { return disconnectionReason; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void ProcessReceivedMessage( byte[] data, int position, int length )
		{
			var reader = new ArrayDataReader( data, position, length );

			var serviceIdentifier = reader.ReadByte();
			var messageIdentifier = reader.ReadByte();

			if( reader.Overflow )
			{
				//OnReceiveProtocolErrorInternal( "Invalid message." );
				return;
			}

			//service message
			var service = GetService( serviceIdentifier );
			if( service == null )
			{
				//no such service
				return;
			}

			service.ProcessReceivedMessage( reader, length, messageIdentifier );
		}

		public int DataMessagesReceivedCounter
		{
			get { return dataMessagesReceivedCounter; }
		}

		public long DataSizeReceivedCounter
		{
			get { return Interlocked.Read( ref dataSizeReceivedCounter ); }
		}

		public int DataMessagesSentCounter
		{
			get { return dataMessagesSentCounter; }
		}

		public long DataSizeSentCounter
		{
			get { return Interlocked.Read( ref dataSizeSentCounter ); }
		}

		public bool ClientConnectHttps
		{
			get { return clientConnectHttps; }
		}

		public string ClientConnectHost
		{
			get { return clientConnectHost; }
		}

		public int ClientConnectPortWebSocket
		{
			get { return clientConnectPortWebSocket; }
		}

		public int ClientConnectPortUdp
		{
			get { return clientConnectPortUdp; }
		}

		//changed when reconnect
		//public string ClientConnectAddress
		//{
		//	get { return clientConnectAddress; }
		//}

		static string ClampCloseReason( string reason )
		{
			var reasonClamped = reason;
			if( reasonClamped.Length > 110 )
				reasonClamped = reason.Substring( 0, 110 ) + "...";
			return reasonClamped;
		}

		async Task CloseAsync( ConnectionCloseStatus status, string rejectReason )
		{
			var webSocketClient2 = webSocketClient;
			if( webSocketClient2 != null )
			{
				if( string.IsNullOrEmpty( disconnectionReason ) )
					disconnectionReason = rejectReason ?? "";

				using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
				await webSocketClient2.CloseOutputAsync( (WebSocketCloseStatus)status, rejectReason != null ? ClampCloseReason( rejectReason ) : null, cts.Token );
				//await webSocketClient2.CloseAsync( (WebSocketCloseStatus)status, rejectReason != null ? ClampCloseReason( rejectReason ) : null, cts.Token );
				backgroundTasksNeedExitSendClose = false;
			}

			var udpListener2 = udpListener;
			var udpPeer2 = udpPeer;
			if( udpListener2 != null && udpPeer2 != null )
			{
				if( string.IsNullOrEmpty( disconnectionReason ) )
					disconnectionReason = rejectReason ?? "";

				var text = rejectReason != null ? ClampCloseReason( rejectReason ) : "";
				udpListener2.Client.DisconnectPeer( udpPeer2, Encoding.UTF8.GetBytes( text ) );
				backgroundTasksNeedExitSendClose = false;
			}
		}

		void DisposeWebSocketClient()
		{
			if( webSocketClient != null )
			{
				try
				{
					webSocketClient?.Dispose();
				}
				catch { }
				webSocketClient = null;
			}
		}

		void DisposeUdpClient()
		{
			if( udpListener != null )
			{
				try
				{
					udpListener?.Client.Stop( false );
				}
				catch { }
				udpListener = null;
			}
		}

		void SetConnectionIsOldAndDisposeWebSocketClient()
		{
			if( !connectionIsOld )
			{
				if( trace )
					Log.Info( "ClientNode: SetConnectionIsOldAndDisposeClient: Start reconnecting..." );

				DisposeWebSocketClient();
				if( noRealConnectionStartTime == default )
					noRealConnectionStartTime = DateTime.UtcNow;
				connectionIsOld = true;
				backgroundTaskSemaphore.Set();
			}
		}

		public double RoundtripSizeLast
		{
			get
			{
				lock( roundtripTimeMeasurements )
				{
					for( int i = roundtripTimeMeasurements.Count - 1; i >= 0; i-- )
					{
						var item = roundtripTimeMeasurements[ i ];
						if( item.ReceiveTime != default )
							return ( item.ReceiveTime - item.SendTime ).TotalSeconds;
					}
					return double.MaxValue;
				}
			}
		}

		public double RoundtripSizeAverage
		{
			get
			{
				var total = 0.0;
				var count = 0;
				lock( roundtripTimeMeasurements )
				{
					for( int i = 0; i < roundtripTimeMeasurements.Count; i++ )
					{
						var item = roundtripTimeMeasurements[ i ];
						if( item.ReceiveTime != default )
						{
							var time = ( item.ReceiveTime - item.SendTime ).TotalSeconds;
							total += time;
							count++;
						}
					}
					if( count == 0 )
						return 0;
					return total / count;
				}
			}
		}

		public DateTime RoundtripLastUtcTime
		{
			get { return roundtripTimeMeasurementsLastReceiveTime; }
		}

		public double GetRoundtripLastInSeconds( DateTime? utcNow = null )
		{
			var lastTime = RoundtripLastUtcTime;
			if( lastTime == default )
				return 0;
			var utcNow2 = utcNow ?? DateTime.UtcNow;
			return ( utcNow2 - lastTime ).TotalSeconds;
		}

		internal void ToProcessMessagesEnqueue( ToProcessMessage message )
		{
			toProcessMessages.Enqueue( message );
			backgroundTaskSemaphore.Set();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static uint ChecksumAppend( ReadOnlySpan<byte> data )
		{
			unchecked
			{
				unsafe
				{
					fixed( byte* pData = data )
					{
						//use optimized sum of 8 byte longs and bytes at the end
						ulong result = 0;

						ulong* pULong = (ulong*)pData;
						int longCount = data.Length / 8;
						for( int n = 0; n < longCount; n++ )
							result += pULong[ n ];

						int byteCount = data.Length & 7;
						byte* pByte = (byte*)( pULong + longCount );
						for( int n = 0; n < byteCount; n++ )
							result += pByte[ n ];

						//Fold 64-bit sum into 32-bit checksum (mod 2^32)
						return (uint)result + (uint)( result >> 32 );
					}
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ProcessPong( uint messageNumber )
		{
			lock( roundtripTimeMeasurements )
			{
				for( int n = 0; n < roundtripTimeMeasurements.Count; n++ )
				{
					var item = roundtripTimeMeasurements[ n ];
					if( item.MessageNumber == messageNumber )
					{
						item.ReceiveTime = DateTime.UtcNow;

						if( item.ReceiveTime > roundtripTimeMeasurementsLastReceiveTime )
							roundtripTimeMeasurementsLastReceiveTime = item.ReceiveTime;

						break;
					}
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ProcessMessageProcessed( uint messageNumber )
		{
			//remove client.sentMessages messages up to processed message number
			while( sentMessages.TryPeek( out var message ) )
			{
				if( message.MessageNumber <= messageNumber )
				{
					Interlocked.Add( ref sentMessagesQueueSize, -message.Data.DataBinaryArray.Length );
					sentMessages.TryDequeue( out _ );
				}
				else
					break;
			}
		}

		internal void ReceivedMessageSetStatusConnected()
		{
			if( status != NetworkStatus.Connected )
			{
				status = NetworkStatus.Connected;
				OnConnectionStatusChanged();
			}
		}

		//public string _GetInternalState()
		//{
		//	var webSocketClient2 = webSocketClient;
		//	if( webSocketClient2 != null )
		//		return webSocketClient2.State.ToString();
		//	else
		//		return "No socket client";
		//}

		public int ClientConnectPort
		{
			get { return ConnectionType == ConnectionTypeEnum.WebSocket ? ClientConnectPortWebSocket : ClientConnectPortUdp; }
		}

		void UpdateUdpMaxFragmentsCount_DisconnectTimeout_UpdateTime()
		{
			var udpListener2 = udpListener;
			if( udpListener2 != null )
			{
				var server = udpListener2.Client;

				int mtu = server.MtuOverride;
				int headerSize = 16;
				int payloadPerFragment = mtu - headerSize;

				var maxMessageSize = Math.Max( SendMessageMaxSize, ReceiveMessageMaxSize );
				server.MaxFragmentsCount = (ushort)( ( maxMessageSize + payloadPerFragment - 1 ) / payloadPerFragment );

				server.DisconnectTimeout = (int)( KeepAliveTime * 1000 );

				server.UpdateTime = (int)( UpdateTimeUdp * 1000 );
			}
		}
	}
}