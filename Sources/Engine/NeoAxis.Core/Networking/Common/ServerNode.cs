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
	/// Base class for a network server instance.
	/// </summary>
	public abstract class ServerNode
	{
		readonly static bool trace = false;
		const int maxServiceIdentifier = 255;
		const double sendPingIntervalInSeconds = 1;
		static string predefinedHost = "localhost";

		static List<ServerNode> instances = new List<ServerNode>();

		volatile bool disposed;

		//common
		public string ServerName { get; }
		public string ServerVersion { get; }
		public int MaxConnections { get; set; }
		public double DefaultMaxLifetime { get; set; }
		public double KeepAliveTime { get; set; }
		public bool AllowReconnect { get; set; }

		public int ReceiveMessageMaxInternalMessageSize { get; set; } = 11 * 1024 * 1024;
		public int ReceiveMessageMaxInternalQueueSize { get; set; } = 22 * 1024 * 1024;
		public int ReceiveMessageMaxInternalQueueSizeOfAllClients { get; set; } = 50 * 1024 * 1024;

		public int ReceiveMessageMaxMessageSize { get; set; } = 10 * 1024 * 1024;
		public long ReceiveMessageMaxBytesPerClientInMinute { get; set; }
		public int ReceiveMessageMaxMessagesPerClientInMinute { get; set; }

		public int SendMessageMaxSize { get; set; } = 10 * 1024 * 1024;

		/// <summary>
		/// UDP library logic update and send period, in seconds.
		/// </summary>
		public double UpdateTimeUdp { get; set; } = 0.015;

		//profiler
		ProfilerDataClass profilerData;

		//services
		List<ServerService> services = new List<ServerService>();
		ServerService[] servicesByIdentifier = new ServerService[ maxServiceIdentifier + 1 ];
		ReadOnlyCollection<ServerService> servicesReadOnly;

		//server data
		internal HttpListener webSocketListener;
		internal UdpServerListener udpListener;

		ConcurrentDictionary<Client, int> connectingClients = new ConcurrentDictionary<Client, int>();
		DateTime connectingClientsDeleteFreezedLastTime;
		DateTime connectingClientsUpdateLastTime;

		ConcurrentQueue<(Client, string)> disapprovedClosingClients = new ConcurrentQueue<(Client, string)>();
		ESet<Client> clients = new ESet<Client>();
		Client[] clientsArray;
		Dictionary<long, Client> clientByLoginDataUserID;
		ConcurrentQueue<Client.ReceivedMessage> receivedMessages = new ConcurrentQueue<Client.ReceivedMessage>();
		ConcurrentQueue<Client.ToProcessMessage> toProcessMessages = new ConcurrentQueue<Client.ToProcessMessage>();
		EConcurrentQueue<(Client, DateTime)> clientsMustNormalDisconnect = new EConcurrentQueue<(Client, DateTime)>();

		DateTime dropByMaxLifetimeLastTime;

		//background task
		Task backgroundTask;
		volatile bool backgroundTaskNeedExit;
		AsyncAutoResetEvent backgroundTaskSemaphore = new AsyncAutoResetEvent();

		//these values are changed from the background task
		internal long totalDataMessagesReceivedCounter;
		internal long totalDataSizeReceivedCounter;
		internal long totalDataMessagesSentCounter;
		internal long totalDataSizeSentCounter;

		//statistics
		double statisticsLastUpdateTime;
		long statisticsLastUpdateReceivedMessages;
		long statisticsLastUpdateReceivedSize;
		long statisticsLastUpdateSentMessages;
		long statisticsLastUpdateSentSize;
		double statisticsLastUpdateReceivedMessagesPerSecond;
		double statisticsLastUpdateReceivedSizePerSecond;
		double statisticsLastUpdateSentMessagesPerSecond;
		double statisticsLastUpdateSentSizePerSecond;

		DateTime sentPingLastTime;

		internal int receivedMessagesSizeTotal;

		ServerNetworkService_Internal networkServiceInternal;

		static object debugConnectionCounterLock = new object();
		static long debugConnectionCounter;

		///////////////////////////////////////////////

		public sealed class Client
		{
			ServerNode owner;

			IPEndPoint remoteEndPoint;

			volatile internal RealConnectionInstance realConnection;
			//internal HttpListenerWebSocketContext webSocketContext;
			//internal HttpListenerContext httpContext;
			internal CancellationTokenSource receiveAsyncCts;

			volatile internal bool insideReconnectingLoop;
			internal int insideReconnectingBegin;

			internal NetworkStatus status = NetworkStatus.Disconnected;
			DateTime noRealConnectionStartTime;

			string clientVersion;
			string loginData;

			DateTime creationTime;
			double maxLifetime;

			//specific
			public long LoginDataUserID { get; set; }
			public string LoginDataUsername { get; set; } = "";
			public object Tag { get; set; }
			public CloudUserRole UserRole { get; set; }

			internal bool? connectingApproved;
			internal string connectingRejectedReason;

			internal string reconnectToken;

			//remote services
			//List<string> remoteServices;
			//ReadOnlyCollection<string> remoteServicesAsReadOnly;

			//received messages queue
			internal int receivedMessagesSize;

			internal int toProcesssMessagesTotalBinaryDataSize;

			ArrayDataWriter accumulatedMessagesToSend = new ArrayDataWriter();

			//sent messages queue for reconnection support
			internal long sentMessagesQueueSize;
			internal EConcurrentQueue<SentMessage> sentMessages = new EConcurrentQueue<SentMessage>();

			internal uint receivedMessagesLastNumber;

			public ConcurrentDictionary<string, object> AnyData = new ConcurrentDictionary<string, object>();

			public NetworkAggregateConnectionStatistics AggregateConnectionStatistics;

#if !NO_SERVER
			//cloud functions specific
			internal ServerNetworkService_CloudFunctions.CallMethodLimiter callMethodLimiter;
#endif

			//per client limits
			public long ReceivedMessagesMaxBytesInMinute;
			public int ReceivedMessagesMaxMessagesInMinute;
			TrafficLimiter trafficLimiter;

			//roundtrip time measurement
			uint roundtripTimeMeasurementLastSentMessageNumber;
			List<RoundtripTimeMeasurement> roundtripTimeMeasurements = new List<RoundtripTimeMeasurement>();
			DateTime roundtripLastUtcTime;

			//these values are changed from the background task
			internal int dataMessagesReceivedCounter;
			internal long dataSizeReceivedCounter;
			internal uint dataMessagesReceivedChecksum;
			internal int dataMessagesSentCounter;
			internal long dataSizeSentCounter;
			internal uint dataMessagesSentChecksum;

			/////////////////////

			//public sealed class StatisticsData
			//{
			//	Client owner;

			//	Group sentGroup = new Group();
			//	Group receivedGroup = new Group();
			//	//Group sentGroupIncludeLibraryMessages = new Group();
			//	//Group receivedGroupIncludeLibraryMessages = new Group();

			//	//

			//	class Group
			//	{
			//		public long bytesTotal;

			//		public float bytesPerSecond;
			//		public double bytesPerSecondUpdateTime;
			//		public long bytesPerSecondUpdateCount;
			//	}

			//	//

			//	internal StatisticsData( Client owner )
			//	{
			//		this.owner = owner;
			//	}

			//	public long GetBytesReceived( bool includeLibraryMessages )
			//	{
			//		if( !includeLibraryMessages )
			//			Log.Fatal( "ConnectedNode: StatisticsData: GetBytesReceived: \"includeLibraryMessages = false\" is not supported." );


			//		return 0;

			//		//owner.connection.totalBytesReceived;


			//		//var targetGroup = includeLibraryMessages ? receivedGroupIncludeLibraryMessages : receivedGroup;

			//		//var connection = owner.connection;
			//		//if( connection != null )
			//		//	targetGroup.bytesTotal = connection.Statistics.ReceivedBytes;// GetBytesReceived( includeLibraryMessages );

			//		//return targetGroup.bytesTotal;



			//		//Group group = includeLibraryMessages ? receivedGroupIncludeLibraryMessages : receivedGroup;
			//		//NetConnection connection = owner.connection;
			//		//if( connection != null )
			//		//	group.bytesTotal = connection.Statistics.ReceivedBytes;// GetBytesReceived( includeLibraryMessages );

			//		//return group.bytesTotal;
			//	}

			//	public long GetBytesSent( bool includeLibraryMessages )
			//	{
			//		if( !includeLibraryMessages )
			//			Log.Fatal( "ConnectedNode: StatisticsData: GetBytesSent: \"includeLibraryMessages = false\" is not supported." );

			//		return 0;


			//		//Group group = includeLibraryMessages ? sentGroupIncludeLibraryMessages : sentGroup;
			//		//NetConnection connection = owner.connection;
			//		//if( connection != null )
			//		//	group.bytesTotal = connection.Statistics.SentBytes;// GetBytesSent( includeLibraryMessages );

			//		//return group.bytesTotal;
			//	}

			//	float GetBytesPerSecond( DateTime utcNow, bool receive )//, bool includeLibraryMessages )
			//	{
			//		if( owner.connection == null )
			//			return 0;

			//		return 0;

			//		//Group group;
			//		//if( receive )
			//		//	group = includeLibraryMessages ? receivedGroupIncludeLibraryMessages : receivedGroup;
			//		//else
			//		//	group = includeLibraryMessages ? sentGroupIncludeLibraryMessages : sentGroup;

			//		//double now = ( utcNow - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) ).TotalSeconds;
			//		////double now = NetTime.Now;

			//		//double diff = now - group.bytesPerSecondUpdateTime;
			//		//if( diff > .2f )
			//		//{
			//		//	long bytesTotal = receive ? GetBytesReceived( includeLibraryMessages ) : GetBytesSent( includeLibraryMessages );

			//		//	if( diff > 5 )
			//		//		group.bytesPerSecondUpdateTime = 0;

			//		//	if( group.bytesPerSecondUpdateTime != 0 )
			//		//	{
			//		//		long bytesDiff = bytesTotal - group.bytesPerSecondUpdateCount;
			//		//		group.bytesPerSecond = (float)( (double)bytesDiff / diff );
			//		//	}

			//		//	group.bytesPerSecondUpdateTime = now;
			//		//	group.bytesPerSecondUpdateCount = bytesTotal;
			//		//}

			//		//return group.bytesPerSecond;
			//	}

			//	public float GetBytesReceivedPerSecond( DateTime utcNow )//, bool includeLibraryMessages )
			//	{
			//		return GetBytesPerSecond( utcNow, true );//, includeLibraryMessages );
			//	}

			//	public float GetBytesSentPerSecond( DateTime utcNow )//, bool includeLibraryMessages )
			//	{
			//		return GetBytesPerSecond( utcNow, false );//, includeLibraryMessages );
			//	}
			//}

			/////////////////////

			public class ReceivedMessage
			{
				public Client Client;

				public byte[] DataBinary;

				public string CloseReason;
				public ConnectionCloseStatus CloseCode; //public ConnectionCloseStatus? CloseCode;
				public string ErrorMessage;
			}

			/////////////////////

			public class ToProcessMessage
			{
				public Client Client;

				public bool DataBinaryCommand;
				public byte[] DataBinaryArray;

				public bool CloseCommand;
				public ConnectionCloseStatus CloseStatusCode;
				public string CloseReason;

				public bool SendSettings;

				public bool ResendSentMessages;
			}

			/////////////////////

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

			/////////////////////

			public class RoundtripTimeMeasurement
			{
				public uint MessageNumber;
				public DateTime SendTime;
				public DateTime ReceiveTime;
			}

			/////////////////////

			internal Client( ServerNode owner, IPEndPoint remoteEndPoint, string clientVersion, string loginData )
			{
				this.owner = owner;
				this.remoteEndPoint = remoteEndPoint;
				this.clientVersion = clientVersion;
				this.loginData = loginData;
				creationTime = DateTime.UtcNow;

				//statistics = new StatisticsData( this );
			}

			public ServerNode Owner
			{
				get { return owner; }
			}

			public IPEndPoint RemoteEndPoint
			{
				get { return remoteEndPoint; }
			}

			public NetworkStatus Status
			{
				get { return status; }
			}

			public double GetNoRealConnectionTimeInSeconds( DateTime? utcNow = null )
			{
				if( noRealConnectionStartTime == default )
					return 0;
				var utcNow2 = utcNow ?? DateTime.UtcNow;
				return ( utcNow2 - noRealConnectionStartTime ).TotalSeconds;
			}

			public string ClientVersion
			{
				get { return clientVersion; }
			}

			public string LoginData
			{
				get { return loginData; }
			}

			public DateTime CreationTime
			{
				get { return creationTime; }
			}

			/// <summary>
			/// Set 0 for unlimited time.
			/// </summary>
			public double MaxLifetime
			{
				get { return maxLifetime; }
				set { maxLifetime = value; }
			}

			//public StatisticsData Statistics
			//{
			//	get { return statistics; }
			//}

			//internal void SetRemoteServices( List<string> remoteServices )
			//{
			//	this.remoteServices = remoteServices;
			//	remoteServicesAsReadOnly = new ReadOnlyCollection<string>( this.remoteServices );
			//}

			//public IList<string> RemoteServices
			//{
			//	get { return remoteServicesAsReadOnly; }
			//}

			public string GetAddressText()
			{
				return RemoteEndPoint.ToString();
			}

			public double GetCurrentLifetime( DateTime? utcNow = null )
			{
				var utcNow2 = utcNow ?? DateTime.UtcNow;
				return ( utcNow2 - CreationTime ).TotalSeconds;
			}

			static string ClampCloseReason( string reason )
			{
				var reasonClamped = reason;
				if( reasonClamped.Length > 110 )
					reasonClamped = reason.Substring( 0, 110 ) + "...";
				return reasonClamped;
			}

			internal async Task CloseAsync( ConnectionCloseStatus status, string rejectReason )
			{
				var webSocket = realConnection?.WebSocketContext?.WebSocket;
				if( webSocket != null )
				{
					using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					await webSocket.CloseAsync( (WebSocketCloseStatus)status, rejectReason != null ? ClampCloseReason( rejectReason ) : null, cts.Token );
				}

				var udpPeer = realConnection?.UdpPeer;
				if( udpPeer != null )
				{
					var text = rejectReason != null ? ClampCloseReason( rejectReason ) : "";
					udpPeer.Disconnect( Encoding.UTF8.GetBytes( text ) );
				}
			}

			static internal async Task CloseOutputAsync( WebSocket webSocket, NetPeer udpPeer, ConnectionCloseStatus status, string rejectReason )
			{
				//var webSocket = realConnection?.WebSocketContext?.WebSocket;
				if( webSocket != null )
				{
					using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					await webSocket.CloseOutputAsync( (WebSocketCloseStatus)status, rejectReason != null ? ClampCloseReason( rejectReason ) : null, cts.Token );
				}

				//var udpPeer = realConnection?.UdpPeer;
				if( udpPeer != null )
				{
					var text = rejectReason != null ? ClampCloseReason( rejectReason ) : "";
					udpPeer.Disconnect( Encoding.UTF8.GetBytes( text ) );
				}
			}

			internal void CloseUdp( /*ConnectionCloseStatus status,*/ string rejectReason )
			{
				var udpPeer = realConnection?.UdpPeer;
				if( udpPeer != null )
				{
					var text = rejectReason != null ? ClampCloseReason( rejectReason ) : "";
					udpPeer.Disconnect( Encoding.UTF8.GetBytes( text ) );
				}
			}

			internal void SetNoRealConnection( string hint )
			{
				if( trace )
					Log.Info( "ServerNode: SetNoRealConnection" );

				if( noRealConnectionStartTime == default )
					noRealConnectionStartTime = DateTime.UtcNow;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			internal void SendPingUpdate( DateTime utcNow )
			{
				if( Status == NetworkStatus.Connected )
				{
					lock( roundtripTimeMeasurements )
					{
						roundtripTimeMeasurementLastSentMessageNumber++;

						if( roundtripTimeMeasurements.Count > 3 )
							roundtripTimeMeasurements.RemoveAt( 0 );

						roundtripTimeMeasurements.Add( new RoundtripTimeMeasurement { SendTime = utcNow, MessageNumber = roundtripTimeMeasurementLastSentMessageNumber } );

						owner.networkServiceInternal.SendPingToClient( this, roundtripTimeMeasurementLastSentMessageNumber, receivedMessagesLastNumber );
					}
				}
			}

			[MethodImpl( (MethodImplOptions)512 )]
			internal void ProcessAccumulatedMessagesToSend()
			{
				if( accumulatedMessagesToSend.Length > 0 ) //fast check before lock
				{
					lock( accumulatedMessagesToSend )
					{
						if( accumulatedMessagesToSend.Length > 0 )
						{
							//!!!!
							//GC. if manage arrays by self, maybe need to worry about memory limitations
							//use array pool?
							var array = accumulatedMessagesToSend.ToArray();

							var overloadOfToProcessMessages = toProcesssMessagesTotalBinaryDataSize > owner.SendMessageMaxSize * 2;
							if( overloadOfToProcessMessages )
							{
								//send null array to indicate overload. it is processed as special case in SendingAndDisapprovedClosingClientsTask
								ToProcessMessagesEnqueue( new ToProcessMessage { DataBinaryCommand = true, DataBinaryArray = null } );
							}
							else
							{
								//add accumulated message to send queue
								ToProcessMessagesEnqueue( new ToProcessMessage { DataBinaryCommand = true, DataBinaryArray = array } );
								Interlocked.Add( ref toProcesssMessagesTotalBinaryDataSize, array.Length );
							}

							accumulatedMessagesToSend.Reset();
						}
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
					if( newCount > owner.SendMessageMaxSize )
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
				get { return roundtripLastUtcTime; }
			}

			public double GetRoundtripLastInSeconds( DateTime? utcNow = null )
			{
				var lastTime = RoundtripLastUtcTime;
				if( lastTime == default )
					return 0;
				var utcNow2 = utcNow ?? DateTime.UtcNow;
				return ( utcNow2 - lastTime ).TotalSeconds;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			internal async Task HandleConnectionWebSocketAsync()
			{
				var accumulatedBuffer = new ArrayDataWriter( 1024 );
				var buffer = new byte[ 1024 * 64 ];

				while( !owner.Disposed && !owner.backgroundTaskNeedExit )
				{
					var reconnect = false;

					try
					{
						while( true )
						{
							var webSocket = realConnection?.WebSocketContext?.WebSocket;
							if( webSocket == null || webSocket.State != WebSocketState.Open )
								break;

							//check for max received messages size
							if( receivedMessagesSize > owner.ReceiveMessageMaxInternalQueueSize )
							{
								await Task.Delay( 10 );
								continue;
							}
							if( owner.receivedMessagesSizeTotal > owner.ReceiveMessageMaxInternalQueueSizeOfAllClients )
							{
								await Task.Delay( 10 );
								continue;
							}

							//Console.WriteLine( $"INSIDE ReceiveAsync. Web socket state={webSocket?.State.ToString() ?? "(null)"}" );

							WebSocketReceiveResult result;

							//!!!!GC?
							receiveAsyncCts = new CancellationTokenSource();

							try
							{
								result = await webSocket.ReceiveAsync( new ArraySegment<byte>( buffer ), receiveAsyncCts.Token );
							}
							finally
							{
								try
								{
									receiveAsyncCts?.Dispose();
								}
								catch { }
								receiveAsyncCts = null;

								//Console.WriteLine( $"OUTSIDE ReceiveAsync. Web socket state={webSocket?.State.ToString() ?? "(null)"}" );
							}

							if( result.MessageType != WebSocketMessageType.Close )
							{
								accumulatedBuffer.Write( buffer, 0, result.Count );

								//check for max message size
								if( accumulatedBuffer.Length > owner.ReceiveMessageMaxInternalMessageSize )
								{
									var error = $"The size of the received message is too large. The maximum size is {owner.ReceiveMessageMaxInternalMessageSize} bytes.";
									OnClose( ConnectionCloseStatus.MessageTooBig, error );
									return;
								}

								if( result.EndOfMessage )
								{
									ProcessReceivedMessage( (ConnectionMessageType)result.MessageType, accumulatedBuffer.AsArraySegment() );
									accumulatedBuffer.Reset();
								}
							}
							else
							{
								//need?
								if( owner.AllowReconnect )
								{
									reconnect = true;
									break;
								}
								else
								{
									OnClose( ConnectionCloseStatus.NormalClosure, result.CloseStatusDescription );
									return;
								}
							}
						}
					}
					catch( Exception e )
					{
						//Console.WriteLine( "EXITED FROM ReceiveAsync" );

						//!!!!если reconnect то может статус 400 ставить

						//maybe exists better solution to detect normal close
						if( e.Message.Contains( "The remote party closed the WebSocket connection without completing the close handshake." ) && !owner.AllowReconnect )
						{
							//try
							//{
							//	var httpContext = realConnection?.HttpContext;
							//	if( httpContext != null )
							//	{
							//		httpContext.Response.StatusCode = 200;
							//		httpContext.Response.Close();
							//	}
							//}
							//catch { }

							OnClose( ConnectionCloseStatus.NormalClosure, null );
							return;
						}
						else
						{
							//try
							//{
							//	//Console.WriteLine( "400 exception: " + e.ToString() );

							//	var httpContext = realConnection?.HttpContext;
							//	if( httpContext != null )
							//	{
							//		httpContext.Response.StatusCode = 400;
							//		httpContext.Response.StatusDescription = e.Message;
							//		httpContext.Response.Close();
							//	}
							//}
							//catch { }

							if( owner.AllowReconnect )
								reconnect = true;
							else
							{
								OnClose( ConnectionCloseStatus.ProtocolError, e.Message );
								return;
							}
						}
					}

					if( realConnection?.WebSocketContext == null )
						await Task.Delay( 1000 );
					else
						await Task.Delay( 10 );

					//reconnect
					if( reconnect )
					{
						SetNoRealConnection( "1" );

						insideReconnectingLoop = true;

						//Console.WriteLine( "Inside reconnecting loop. Real state: " + realConnection?.WebSocketContext?.WebSocket.State.ToString() ?? "null" );

						//wait for establish a new connection
						try
						{
							do
							{
								try
								{
									//check for restored connection
									var webSocket = realConnection?.WebSocketContext?.WebSocket;
									if( webSocket != null && webSocket.State == WebSocketState.Open )
										break;

									var utcNow = DateTime.UtcNow;
									var noRealConnectionTime = GetNoRealConnectionTimeInSeconds( utcNow );

									//check time is out
									if( noRealConnectionTime >= owner.KeepAliveTime )
									{
										//time is out, close connection
										OnClose( ConnectionCloseStatus.NormalClosure, "Keep alive time is out." );
										//OnClose( ConnectionCloseStatus.NormalClosure, null );
										return;
									}

									//check disposed
									if( owner.disposed )
										return;
								}
								catch //( Exception e )
								{
									//Console.WriteLine( "Exception inside reconnect. Exception: " + e.Message );

									//OnClose( ConnectionCloseStatus.ProtocolError, e.Message );
									//return;
								}

								await Task.Delay( 1000 );

							} while( true );
						}
						finally
						{
							insideReconnectingLoop = false;
						}

						//Console.WriteLine( "Connection restored." );

						//connection restored now
						noRealConnectionStartTime = default;

						//resend queued messages
						ToProcessMessagesEnqueue( new ToProcessMessage { ResendSentMessages = true } );
					}
				}
			}

			internal void OnClose( ConnectionCloseStatus closeStatus, string statusDescription )
			{
				ToReceivedMessagesEnqueue( new ReceivedMessage { CloseReason = statusDescription ?? "", CloseCode = closeStatus } );

				if( trace )
				{
					var statusDescription2 = statusDescription ?? "(No status)";
					Log.Info( $"OnClose {RemoteEndPoint} {closeStatus} {statusDescription2}" );
				}
			}

			[MethodImpl( (MethodImplOptions)512 )]
			internal void ProcessReceivedMessage( ConnectionMessageType messageType, ArraySegment<byte> buffer )
			{
				//if( messageType == WebSocketMessageType.Text )
				//{
				//	//system commands

				//	if( trace )
				//		Log.Info( "OnMessage Text" );

				//	try
				//	{
				//		//? check for hard to parse text block. maybe then not use TextBlock. or use limits when parsing

				//		var text = Encoding.UTF8.GetString( buffer );
				//		if( text.Length > 1000 )
				//			throw new Exception( "The system message is more than 1000 characters." );

				//		var rootBlock = TextBlock.Parse( text, out var error );
				//		if( !string.IsNullOrEmpty( error ) )
				//			throw new Exception( error );

				//		var command = rootBlock.GetAttribute( "C" );

				//		throw new Exception( "Unknown command." );
				//	}
				//	catch( Exception e )
				//	{
				//		if( trace )
				//			Log.Info( "OnMessage Text Exception: " + e.Message );
				//		OnClose( ConnectionCloseStatus.ProtocolError, e.Message );
				//	}
				//}
				//else

				if( messageType == ConnectionMessageType.Binary )
				{
					//data command

					//check status
					if( status != NetworkStatus.Connected )
					{
						if( trace )
							Log.Info( "OnMessage Binary: The client is not in the Connected status." );
						OnClose( ConnectionCloseStatus.ProtocolError, "The client is not in the Connected status." );
						return;
					}

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

						//update statistics
						unchecked
						{
							//client data
							Interlocked.Increment( ref dataMessagesReceivedCounter );
							Interlocked.Add( ref dataSizeReceivedCounter, length );
							dataMessagesReceivedChecksum += ChecksumAppend( data );

							//all clients data
							Interlocked.Increment( ref owner.totalDataMessagesReceivedCounter );
							Interlocked.Add( ref owner.totalDataSizeReceivedCounter, length );
							AggregateConnectionStatistics?.AddReceived( length );
						}

						//debug logs
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

						//check traffic limit
						{
							var maxBytes = ReceivedMessagesMaxBytesInMinute != 0 ? ReceivedMessagesMaxBytesInMinute : owner.ReceiveMessageMaxBytesPerClientInMinute;
							var maxMessages = ReceivedMessagesMaxMessagesInMinute != 0 ? ReceivedMessagesMaxMessagesInMinute : owner.ReceiveMessageMaxMessagesPerClientInMinute;

							if( maxBytes != 0 || maxMessages != 0 )
							{
								if( trafficLimiter == null )
									trafficLimiter = new TrafficLimiter();

								var utcNow = DateTime.UtcNow;

								if( !trafficLimiter.Add( utcNow, maxBytes, maxMessages, length, out var error ) )
									throw new Exception( $"Traffic limit exceeded. " + error );
							}
						}

						//enqueue message
						ToReceivedMessagesEnqueue( new ReceivedMessage { DataBinary = data } );
						Interlocked.Add( ref receivedMessagesSize, length );
						Interlocked.Add( ref owner.receivedMessagesSizeTotal, length );

						////send back message number. it is used to confirm message received
						//if( owner.AllowReconnect )
						//	owner.networkServiceInternal.SendMessageProcessedToClient( this, receivedMessagesLastNumber );
					}
				}
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

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			internal void ToProcessMessagesEnqueue( ToProcessMessage message )
			{
				message.Client = this;
				owner.toProcessMessages.Enqueue( message );
				owner.backgroundTaskSemaphore.Set();
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			internal void ToReceivedMessagesEnqueue( ReceivedMessage message )
			{
				message.Client = this;
				owner.receivedMessages.Enqueue( message );

				//semaphore?
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

							if( item.ReceiveTime > roundtripLastUtcTime )
								roundtripLastUtcTime = item.ReceiveTime;

							break;
						}
					}
				}
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			internal void ProcessMessageProcessed( uint messageNumber )
			{
				//remove sentMessages messages up to processed message number
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
		}

		///////////////////////////////////////////////

		internal class RealConnectionInstance
		{
			//web socket connection
			public HttpListenerWebSocketContext WebSocketContext;
			public HttpListenerContext HttpContext;

			//udp connection
			public NetPeer UdpPeer;

			public long DebugRealConnectionCounter;
		}

		///////////////////////////////////////////////

		public class ProfilerDataClass
		{
			//const data
			public DateTime TimeStarted;
			public double WorkingTime;
			//from ClientNode:
			//public long SystemMessagesReceivedStartCounter;
			//public long SystemMessagesSentStartCounter;

			//dynamic data
			public long TotalReceivedMessages;
			public long TotalReceivedSize;
			public long TotalSentMessages; //using Interlocked
			public long TotalSentSize; //using Interlocked
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
					public long SentMessages; //using Interlocked
					public long SentSize; //using Interlocked

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

		///////////////////////////////////////////

		public class TrafficLimiter
		{
			Queue<(DateTime, int)> requests = new Queue<(DateTime, int)>();
			long requestsTotalSize;

			//

			public bool Add( DateTime utcNow, long maxBytes, int maxMessages, int messageSize, out string error )
			{
				//remove old requests
				while( requests.Count > 0 && ( utcNow - requests.Peek().Item1 ).TotalMinutes >= 1.0 )
				{
					var size = requests.Dequeue().Item2;
					requestsTotalSize -= size;
				}

				if( maxMessages != 0 && requests.Count > maxMessages )
				{
					error = $"Max messages per minute limit exceeded. Max messages={maxMessages}; Current messages={requests.Count}.";
					return false;
				}

				if( maxBytes != 0 && requestsTotalSize > maxBytes )
				{
					error = $"Max bytes per minute limit exceeded. Max bytes={maxBytes}; Current bytes={requestsTotalSize}.";
					return false;
				}

				//add new request
				//if( ( maxMessages == 0 || requests.Count < maxMessages ) && ( maxBytes == 0 || requestsTotalSize < maxBytes ) )
				{
					requests.Enqueue( (utcNow, messageSize) );
					requestsTotalSize += messageSize;
					error = null;
					return true;
				}

				//return false;
			}
		}

		///////////////////////////////////////////////

		internal class UdpServerListener : INetEventListener
		{
			public ServerNode Owner;
			public NetManager Server;

			//

			public void OnConnectionRequest( ConnectionRequest request )
			{
				//Console.WriteLine( "Server UDP: Peer connection request: " + request.RemoteEndPoint.ToString() );

				try
				{
					////get reconnect token from parameters
					//var reconnectToken = httpContext.Request.QueryString[ "reconnect_token" ];

					//if( !string.IsNullOrEmpty( reconnectToken ) )
					//{
					//}
					//else
					{
						//new connection

						//Console.WriteLine( "BeginListen: New connection." );

						if( Owner.GetClientsArray().Length >= Owner.MaxConnections )
							throw new Exception( $"The maximum connections limit has been reached, which is set at {Owner.MaxConnections}." );

						var userEndPoint = request.RemoteEndPoint;
						if( userEndPoint == null )
							throw new Exception( "UserEndPoint is null." );

						if( trace )
							Log.Info( "OnOpen UDP " + userEndPoint.ToString() );

						var welcomeBase64 = request.Data.GetString();
						if( string.IsNullOrEmpty( welcomeBase64 ) )
							throw new Exception( "Invalid welcome parameter." );

						var welcome = StringUtility.DecodeFromBase64URL( welcomeBase64 );
						if( welcome.Length > 300 )
							throw new Exception( "The welcome message is more than 300 characters." );

						var rootBlock = TextBlock.Parse( welcome, out var error );
						if( !string.IsNullOrEmpty( error ) )
							throw new Exception( error );

						var clientVersion = rootBlock.GetAttribute( "ClientVersion" );
						var loginData = rootBlock.GetAttribute( "LoginData" );

						var peer = request.Accept();

						var client = new Client( Owner, userEndPoint, clientVersion, loginData );
						peer.Tag = client;

						var instance = new RealConnectionInstance();
						instance.UdpPeer = peer;
						instance.DebugRealConnectionCounter = GetUniqueDebugConnectionCounter();
						client.realConnection = instance;

						client.status = NetworkStatus.Connecting;
						client.MaxLifetime = Owner.DefaultMaxLifetime;

						Owner.connectingClients[ client ] = 1;

						//start incoming connection approval process (cloud verification code)
						var incomingConnectionApprovalResult = new IncomingConnectionApproveResult( client );
						Owner.IncomingConnectionApproval?.Invoke( Owner, client, incomingConnectionApprovalResult );
					}
				}
				catch( Exception e )
				{
					request.Reject( Encoding.UTF8.GetBytes( e.Message ) );
				}
			}

			public void OnPeerConnected( NetPeer peer )
			{
				//Console.WriteLine( "Server UDP: Peer connected: " + peer );
			}

			public void OnPeerDisconnected( NetPeer peer, DisconnectInfo disconnectInfo )
			{
				//Console.WriteLine( "OnPeerDisconnected: " + peer + ", reason: " + disconnectInfo.Reason );
				//return;

				//get client
				var client = peer.Tag as Client;
				if( client == null )
					return;
				lock( Owner.clients )
				{
					if( !Owner.clients.Contains( client ) )
						return;
				}

				//!!!!read AdditionalData?

				client.OnClose( ConnectionCloseStatus.NormalClosure, disconnectInfo.Reason.ToString() );
			}

			public void OnNetworkError( IPEndPoint endPoint, SocketError socketErrorCode )
			{
			}

			public void OnNetworkReceive( NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod )
			{
				if( Owner.Disposed || Owner.backgroundTaskNeedExit )
					return;

				//get client
				var client = peer.Tag as Client;
				if( client == null )
					return;
				lock( Owner.clients )
				{
					if( !Owner.clients.Contains( client ) )
						return;
				}

				try
				{

					//the messages are processed immediately without queue. also can't change socket size in the library

					////check for max received messages size
					//if( client.receivedMessagesSize > Owner.ReceiveMessageMaxInternalQueueSize )
					//{
					//	await Task.Delay( 10 );
					//	continue;
					//}
					//if( Owner.receivedMessagesSizeTotal > Owner.ReceiveMessageMaxInternalQueueSizeOfAllClients )
					//{
					//	await Task.Delay( 10 );
					//	continue;
					//}


					//check for max message size
					if( reader.UserDataSize > Owner.ReceiveMessageMaxInternalMessageSize )
					{
						var error = $"The size of the received message is too large. The maximum size is {Owner.ReceiveMessageMaxInternalMessageSize} bytes.";
						client.OnClose( ConnectionCloseStatus.MessageTooBig, error );
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
						client.OnClose( ConnectionCloseStatus.ProtocolError, error );
						return;
					}

					var segment = new ArraySegment<byte>( reader.RawData, reader.UserDataOffset, reader.UserDataSize );
					client.ProcessReceivedMessage( messageType, segment );
				}
				catch( Exception e )
				{
					client.CloseUdp( /*ConnectionCloseStatus.ProtocolError,*/ e.Message );
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

		public delegate void ProtocolErrorDelegate( ServerNode sender, Client client, string message );
		/// <summary>
		/// Occurs when a protocol error is detected.
		/// </summary>
		public event ProtocolErrorDelegate ProtocolError;

		public delegate void IncomingConnectionApprovalDelegate( ServerNode sender, Client client, IncomingConnectionApproveResult approveResult );
		public event IncomingConnectionApprovalDelegate IncomingConnectionApproval;

		public delegate void ClientBeforeStatusChangeToConnectedDelegate( ServerNode sender, Client client );
		public event ClientBeforeStatusChangeToConnectedDelegate ClientBeforeStatusChangeToConnected;

		public delegate void ClientStatusChangedDelegate( ServerNode sender, Client client, string message );
		public event ClientStatusChangedDelegate ClientStatusChanged;

		///////////////////////////////////////////////

		public class IncomingConnectionApproveResult
		{
			Client client;

			internal IncomingConnectionApproveResult( Client client )
			{
				this.client = client;
			}

			public void Approve()
			{
				client.connectingApproved = true;
			}

			public void Reject( string reason )
			{
				client.connectingRejectedReason = reason;
				client.connectingApproved = false;
			}
		}

		///////////////////////////////////////////////

		public static string PredefinedHost
		{
			get { return predefinedHost; }
		}

		public static string BeginListenLastError { get; set; } = "";

		public static ServerNode[] GetInstances()
		{
			lock( instances )
				return instances.ToArray();
		}

		public void Update( DateTime utcNow )
		{
			OnUpdate( utcNow );
		}

		public bool Disposed
		{
			get { return disposed; }
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
			//from ClientNode:
			//profilerData.SystemMessagesReceivedStartCounter = DataMessagesReceivedCounter;
			//profilerData.SystemMessagesSentStartCounter = DataMessagesSentCounter;

			Log.Info( "Network profiler started." );
		}

		public void ProfilerStop( bool writeToLogs )
		{
			if( profilerData != null )
			{
				var workedTime = ( DateTime.UtcNow - ProfilerData.TimeStarted ).TotalSeconds;
				if( workedTime > 0 )
				{
					var workedTimeString = workedTime.ToString( "F1" );
					Log.Info( $"Network profiler stopped after {workedTimeString} seconds." );
				}
				else
					Log.Info( "Network profiler stopped." );

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
			var lines = new List<string>();

			lines.Add( "--------------------------------------------------------------" );
			lines.Add( string.Format( "Total received; {0}", StringUtility.FormatSize( profilerData.TotalReceivedSize ) ) );
			//lines.Add( string.Format( "Total received. Messages: {0}; Size: {1}", FormatCount( profilerData.TotalReceivedMessages ), FormatSize( profilerData.TotalReceivedSize ) ) );

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
			lines.Add( string.Format( "Total sent; Size: {0}", StringUtility.FormatSize( Interlocked.Read( ref profilerData.TotalSentSize ) ) ) );

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
							if( messageByTypeItem != null && Interlocked.Read( ref messageByTypeItem.SentMessages ) != 0 )
								messageByTypeItems.Add( (messageByTypeItem, messageTypeId) );
						}
					}

					CollectionUtility.MergeSort( messageByTypeItems, delegate ( (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item1, (ProfilerDataClass.ServiceItem.MessageTypeItem, int) item2 )
					{
						if( Interlocked.Read( ref item1.Item1.SentSize ) > Interlocked.Read( ref item2.Item1.SentSize ) )
							return -1;
						if( Interlocked.Read( ref item1.Item1.SentSize ) < Interlocked.Read( ref item2.Item1.SentSize ) )
							return 1;
						return 0;
					} );

					foreach( var messageByTypeItemPair in messageByTypeItems )
					{
						var messageByTypeItem = messageByTypeItemPair.Item1;
						var messageTypeId = messageByTypeItemPair.Item2;

						var messageType = service.GetMessageType( messageTypeId );

						lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( Interlocked.Read( ref messageByTypeItem.SentMessages ) ), StringUtility.FormatSize( Interlocked.Read( ref messageByTypeItem.SentSize ) ) ) );

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="serverVersion"></param>
		/// <param name="maxConnections"></param>
		/// <param name="defaultMaxLifetime">Set 0 for unlimited time.</param>
		/// <param name="noRealConnectionMaxTime">Set 0 to disable the internal reconnect functionality.</param>
		protected ServerNode( string serverName, string serverVersion, int maxConnections, double defaultMaxLifetime, double keepAliveTime, bool allowReconnect )
		{
			lock( instances )
				instances.Add( this );

			this.ServerName = serverName;
			this.ServerVersion = serverVersion;
			this.MaxConnections = maxConnections;
			this.DefaultMaxLifetime = defaultMaxLifetime;
			this.KeepAliveTime = keepAliveTime;
			this.AllowReconnect = allowReconnect;

			servicesReadOnly = new ReadOnlyCollection<ServerService>( services );

			networkServiceInternal = new ServerNetworkService_Internal();
			RegisterService( networkServiceInternal );
		}

		/// <summary>
		/// Starts listening for incoming WebSocket and UDP connections on the specified host and ports.
		/// </summary>
		/// <remarks>This method must be called before accepting client connections. If the server is already
		/// initialized or has been disposed, the method will not start listening. Only one call to BeginListen should be
		/// active at a time. If an error occurs during initialization, the error message will be set in the out parameter and
		/// the method will return false.</remarks>
		/// <param name="https">true to use HTTPS for WebSocket connections; otherwise, false to use HTTP.</param>
		/// <param name="host">The host name or IP address to bind the server to.</param>
		/// <param name="portWebSocket">The port number on which to listen for WebSocket connections. Specify 0 to disable WebSocket listening.</param>
		/// <param name="portUdp">The port number on which to listen for UDP connections. Specify 0 to disable UDP listening.</param>
		/// <param name="error">When this method returns, contains an error message if the server failed to start listening; otherwise, null.</param>
		/// <returns>true if the server successfully started listening; otherwise, false.</returns>
		public bool BeginListen( bool https, string host, int portWebSocket, int portUdp, out string error )
		{
			error = null;

			if( Disposed )
				Log.Fatal( "ServerNode: BeginListen: The server has been disposed." );
			if( webSocketListener != null )
				Log.Fatal( "ServerNode: BeginListen: The server is already initialized." );

			if( portWebSocket == 0 && portUdp == 0 )
			{
				error = "At least one of the ports must be specified to start listening.";
				return false;
			}

			//listen for web socket connections
			if( portWebSocket != 0 )
			{
				webSocketListener = new HttpListener();

				var prefix = https ? "https" : "http";
				var host2 = host;
				if( string.IsNullOrEmpty( host2 ) )
					host2 = PredefinedHost;
				webSocketListener.Prefixes.Add( $"{prefix}://{host2}:{portWebSocket}/service/" );

				try
				{
					//start WebSocket server
					webSocketListener.Start();
					if( !webSocketListener.IsListening )
						throw new Exception( "The WebSocket server is not listening." );

					//run task to receive messages
					TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Forever, "ServerNode: BeginListen WebSocket: Receive messages", async delegate ()
					{
						try
						{
							while( webSocketListener.IsListening )
							{
								var httpContext = await webSocketListener.GetContextAsync();
								if( httpContext.Request.IsWebSocketRequest )
								{
									try
									{
										//get reconnect token from parameters
										var reconnectToken = httpContext.Request.QueryString[ "reconnect_token" ];

										if( !string.IsNullOrEmpty( reconnectToken ) )
										{
											//reconnection
											//can be many requests to reconnect at same time (system caching, etc)

											//Console.WriteLine( "BeginListen: Reconnection started. Token: " + reconnectToken );

											//check for disabled reconnect
											if( !AllowReconnect )
											{
												httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
												httpContext.Response.Close();
												continue;
											}

											//get client by reconnect token
											Client client = null;
											foreach( var c in GetClientsArray() )
											{
												if( c.reconnectToken == reconnectToken )
												{
													client = c;
													break;
												}
											}

											//invalid reconnect token
											if( client == null )
											{
												//Console.WriteLine( "BeginListen: INVALID RECONNECT TOKEN: " + reconnectToken );

												httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
												httpContext.Response.Close();
												continue;
											}

											//exit from ReceiveAsync
											try
											{
												client.receiveAsyncCts?.Cancel();
											}
											catch { }
											client.receiveAsyncCts = null;

											try
											{
												// Ensure only one reconnection begin is processed at a time per client.
												// Atomically set 0 -> 1, if it was already 1 then reject.
												if( Interlocked.CompareExchange( ref client.insideReconnectingBegin, 1, 0 ) != 0 )
												{
													//Console.WriteLine( "BeginListen: Already reconnecting begin " );

													httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
													httpContext.Response.Close();
													continue;
												}

												//if( client.insideReconnectingBegin != 0 )
												//{
												//	httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
												//	httpContext.Response.Close();
												//	continue;
												//}
												//client.insideReconnectingBegin = 1;

												//delete previous connection
												try
												{
													//var currentHttpContext = client?.realConnection?.HttpContext;
													var webSocket = client?.realConnection?.WebSocketContext?.WebSocket;
													if( webSocket != null ) //if( currentHttpContext != null )
													{
														await Client.CloseOutputAsync( webSocket, null, ConnectionCloseStatus.NormalClosure, null );

														//currentHttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
														//currentHttpContext.Response.Close();

														//wait switch to reconnecting state
														await Task.Delay( 3000 );
													}
												}
												catch { }

												//check server is ready to reconnect (inside reconnecting loop)
												if( !client.insideReconnectingLoop )
												{
													httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
													httpContext.Response.Close();
													continue;
												}

												//accept new web socket
												var webSocketContext = await httpContext.AcceptWebSocketAsync( subProtocol: null );

												//assign new web socket to client
												var instance = new RealConnectionInstance();
												instance.WebSocketContext = webSocketContext;
												instance.HttpContext = httpContext;
												instance.DebugRealConnectionCounter = GetUniqueDebugConnectionCounter();
												client.realConnection = instance;
											}
											finally
											{
												Interlocked.Exchange( ref client.insideReconnectingBegin, 0 );
												//client.insideReconnectingBegin = 0;
											}

											//Console.WriteLine( "BeginListen: Reconnected. Real state: " + client.realConnection?.WebSocketContext?.WebSocket?.State.ToString() ?? "null" );

											//now client has a new real connection. HandleConnectionAsync will detect it and continue working
										}
										else
										{
											//new connection

											//Console.WriteLine( "BeginListen: New connection." );

											if( GetClientsArray().Length >= MaxConnections )
												throw new Exception( $"The maximum connections limit has been reached, which is set at {MaxConnections}." );

											var userEndPoint = httpContext.Request.RemoteEndPoint;
											if( userEndPoint == null )
												throw new Exception( "UserEndPoint is null." );

											if( trace )
												Log.Info( "OnOpen WebSocket " + userEndPoint.ToString() );

											var welcomeBase64 = httpContext.Request.QueryString[ "welcome" ];
											if( string.IsNullOrEmpty( welcomeBase64 ) )
												throw new Exception( "Invalid welcome parameter." );

											var welcome = StringUtility.DecodeFromBase64URL( welcomeBase64 );
											if( welcome.Length > 300 )
												throw new Exception( "The welcome message is more than 300 characters." );

											var rootBlock = TextBlock.Parse( welcome, out var error );
											if( !string.IsNullOrEmpty( error ) )
												throw new Exception( error );

											var clientVersion = rootBlock.GetAttribute( "ClientVersion" );
											var loginData = rootBlock.GetAttribute( "LoginData" );

											//var clientServices = new List<string>();
											//foreach( var block in rootBlock.Children )
											//{
											//	if( block.Name == "ClientService" )
											//	{
											//		var name = block.Data;
											//		if( string.IsNullOrEmpty( name ) )
											//			throw new Exception( "The remove service has no name." );
											//		clientServices.Add( name );
											//		if( clientServices.Count > 100 )
											//			throw new Exception( "More than 100 remote services." );
											//	}
											//}

											var webSocketContext = await httpContext.AcceptWebSocketAsync( subProtocol: null );

											var client = new Client( this, userEndPoint, clientVersion, loginData );

											var instance = new RealConnectionInstance();
											instance.WebSocketContext = webSocketContext;
											instance.HttpContext = httpContext;
											instance.DebugRealConnectionCounter = GetUniqueDebugConnectionCounter();
											client.realConnection = instance;

											client.status = NetworkStatus.Connecting;
											client.MaxLifetime = DefaultMaxLifetime;

											connectingClients[ client ] = 1;

											//start incoming connection approval process (cloud verification code)
											var incomingConnectionApprovalResult = new IncomingConnectionApproveResult( client );
											IncomingConnectionApproval?.Invoke( this, client, incomingConnectionApprovalResult );
										}
									}
									catch( Exception e )
									{
										//Console.WriteLine( "BeginListen exception 2: " + e.ToString() );

										try
										{
											httpContext.Response.StatusCode = 400;
											httpContext.Response.StatusDescription = e.Message;
											httpContext.Response.Close();
										}
										catch { }
									}
								}
								else
								{
									httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
									httpContext.Response.Close();
								}
							}
						}
						catch( Exception e )
						{
							Log.Warning( "BeginListen WebSocket inner exception: " + e.ToString() );

							//Console.WriteLine( "BeginListen inner exception: " + e.ToString() );
						}
					} );

					//backgroundTask = TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Forever, "ServerNode: BeginListen: SendingAndDisapprovedClosingClientsTask", SendingAndDisapprovedClosingClientsTask );
				}
				catch( Exception e )
				{
					//Console.WriteLine( "BeginListen WebSocket exception: " + e.ToString() );

					error = e.Message;
					BeginListenLastError = error;
					return false;
				}
			}

			//listen for udp connections
			if( portUdp != 0 )
			{
				try
				{
					//start UDP server

					var listener = new UdpServerListener();
					var server = new NetManager( listener );
					listener.Owner = this;
					listener.Server = server;
					udpListener = listener;

					server.AutoRecycle = true;
					server.MtuOverride = 1200;
					//MtuDiscovery. to change MtuDiscovery consider UpdateUdpMaxFragmentsCount
					UpdateUdpMaxFragmentsCount_DisconnectTimeout_UpdateTime();
					server.UnsyncedEvents = true;
					server.UnsyncedReceiveEvent = true;
					server.UnsyncedDeliveryEvent = true;
					server.UseNativeSockets = true;
					server.AllowPeerAddressChange = true; //to enable reconnect


					//on server need when P2P
					//public int ReconnectDelay = 500;
					//public int MaxConnectAttempts = 10;


					//!!!!
					//server.PacketPoolSize

					//!!!!
					//socket.ReceiveBufferSize = NetConstants.SocketBufferSize;
					//socket.SendBufferSize = NetConstants.SocketBufferSize;


					if( !server.Start( portUdp ) )
						throw new Exception( "The UDP server is not listening." );
				}
				catch( Exception e )
				{
					//Console.WriteLine( "BeginListen UDP exception: " + e.ToString() );

					error = e.Message;
					BeginListenLastError = error;
					return false;
				}
			}

			backgroundTask = TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Forever, "ServerNode: BeginListen: SendingAndDisapprovedClosingClientsTask", SendingAndDisapprovedClosingClientsTask );

			return true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		async Task SendingAndDisapprovedClosingClientsTask()
		{
			try
			{
				//can't use parallel
				var binaryDataMessagesWriter = new ArrayDataWriter();

				while( !Disposed && !backgroundTaskNeedExit )
				{
					//send messages
					while( toProcessMessages.TryDequeue( out var message ) )
					{
						var client = message.Client;
						var webSocket = client.realConnection?.WebSocketContext?.WebSocket;
						var udpPeer = client.realConnection?.UdpPeer;

						if( message.DataBinaryCommand )
						{
							var data = message.DataBinaryArray;

							//overload of toProcessMessages queue
							if( data == null )
							{
								client.OnClose( ConnectionCloseStatus.ProtocolError, "The process messages queue is overloaded." );
								continue;
							}

							Interlocked.Add( ref client.toProcesssMessagesTotalBinaryDataSize, -data.Length );

							try
							{
								unchecked
								{
									//client data
									Interlocked.Increment( ref client.dataMessagesSentCounter );
									Interlocked.Add( ref client.dataSizeSentCounter, data.Length );
									client.dataMessagesSentChecksum += ChecksumAppend( data );

									//all clients data
									Interlocked.Increment( ref totalDataMessagesSentCounter );
									Interlocked.Add( ref totalDataSizeSentCounter, data.Length );
									client.AggregateConnectionStatistics?.AddSent( data.Length );
								}

								//update headers
								unsafe
								{
									fixed( byte* p = data )
									{
										*(uint*)p = (uint)client.dataMessagesSentCounter;
										*(uint*)( p + 4 ) = client.dataMessagesSentChecksum;
									}
								}

								if( trace )
									Log.Info( $"Send Binary {data.Length} {client.dataMessagesSentCounter} {client.dataMessagesSentChecksum}" );

								//add to sent messages queue for reconnecting
								if( AllowReconnect && webSocket != null )
								{
									Interlocked.Add( ref client.sentMessagesQueueSize, data.Length );
									client.sentMessages.Enqueue( new Client.SentMessage( (uint)client.dataMessagesSentCounter, client.dataMessagesSentChecksum, message ) );

									//maybe SendMessageMaxSize * 2?

									//check limit. max buffer size is same as SendMessageMaxSize
									if( client.sentMessagesQueueSize > SendMessageMaxSize )
									{
										client.OnClose( ConnectionCloseStatus.ProtocolError, "The sent messages queue size exceeded the maximum allowed." );
										continue;
									}
								}

								if( webSocket != null )
								{
									using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
#if NETSTANDARD2_1
									await webSocket.SendAsync( data, WebSocketMessageType.Binary, true, cts.Token );
#else
									await webSocket.SendAsync( data, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage | WebSocketMessageFlags.DisableCompression, cts.Token );
#endif
								}
								else if( udpPeer != null )
									udpPeer.Send( data, 0, DeliveryMethod.ReliableOrdered );
							}
							catch( Exception e )
							{
								if( trace )
									Log.Info( "OnMessage Binary exception when sending binary data: " + e.Message );

								if( AllowReconnect && webSocket != null )
									client.SetNoRealConnection( "2 " + e.Message );// ToString() );
								else
								{
									client.OnClose( ConnectionCloseStatus.ProtocolError, "Unable to send the binary message. " + e.Message );
									//client.OnClose( ConnectionCloseStatus.ProtocolError, "Unable to send the binary message. " + e.ToString() );
								}

								continue;
							}
						}
						else if( message.CloseCommand )
						{
							//close connection
							client.OnClose( message.CloseStatusCode, message.CloseReason );
						}
						else if( message.SendSettings )
						{
							//send KeepAliveTime, ReconnectToken

							try
							{
								var rootBlock = new TextBlock();
								rootBlock.SetAttribute( "C", "Settings" );
								rootBlock.SetAttribute( "KeepAliveTime", KeepAliveTime.ToString() );
								if( AllowReconnect && webSocket != null && !string.IsNullOrEmpty( client.reconnectToken ) )
									rootBlock.SetAttribute( "ReconnectToken", client.reconnectToken );
								var text = rootBlock.DumpToString( false );

								if( trace )
									Log.Info( $"Send Text Settings" );

								var buffer2 = Encoding.UTF8.GetBytes( text );
								if( webSocket != null )
								{
									using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
#if NETSTANDARD2_1
									await webSocket.SendAsync( new ArraySegment<byte>( buffer2 ), WebSocketMessageType.Text, true, cts.Token );
#else
									await webSocket.SendAsync( new ArraySegment<byte>( buffer2 ), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage | WebSocketMessageFlags.DisableCompression, cts.Token );
#endif
								}
								else if( udpPeer != null )
									udpPeer.Send( new ArraySegment<byte>( buffer2 ), 1, DeliveryMethod.ReliableOrdered );
							}
							catch( Exception e )
							{
								if( trace )
									Log.Info( "OnMessage Binary exception when sending NoRealConnectionMaxTime command: " + e.Message );

								if( AllowReconnect && webSocket != null )
									client.SetNoRealConnection( "3 " + e.Message );
								else
									client.OnClose( ConnectionCloseStatus.ProtocolError, "Unable to send the NoRealConnectionMaxTime command message. " + e.Message );

								continue;
							}
						}
						else if( message.ResendSentMessages )
						{
							//resend sent messages after reconnection

							try
							{
								foreach( var sentMessage in client.sentMessages )
								{
									using var cts = new CancellationTokenSource( TimeSpan.FromSeconds( Math.Max( KeepAliveTime, 60 ) ) );
									if( webSocket != null )
									{
#if NETSTANDARD2_1
										await webSocket.SendAsync( sentMessage.Data.DataBinaryArray, WebSocketMessageType.Binary, true, cts.Token );
#else
										await webSocket.SendAsync( sentMessage.Data.DataBinaryArray, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage | WebSocketMessageFlags.DisableCompression, cts.Token );
#endif
									}
									else if( udpPeer != null )
										udpPeer.Send( sentMessage.Data.DataBinaryArray, 0, DeliveryMethod.ReliableOrdered );
								}
							}
							catch( Exception e )
							{
								if( trace )
									Log.Info( "OnMessage Binary exception when sending ResendSentMessages command: " + e.Message );

								if( AllowReconnect && webSocket != null )
									client.SetNoRealConnection( "4 " + e.Message );
								else
									client.OnClose( ConnectionCloseStatus.ProtocolError, "Unable to send the ResendSentMessages command message. " + e.Message );

								continue;
							}
						}

						if( Disposed || backgroundTaskNeedExit )
							return;
					}

					//normal closing with delay
					while( clientsMustNormalDisconnect.TryPeek( out var item ) )
					{
						var client = item.Item1;
						var disconnectTime = item.Item2;

						if( DateTime.UtcNow > disconnectTime )
						{
							clientsMustNormalDisconnect.TryDequeue( out _ );

							client.ToProcessMessagesEnqueue( new Client.ToProcessMessage { CloseCommand = true, CloseStatusCode = ConnectionCloseStatus.NormalClosure, CloseReason = "" } );
						}
						else
							break;
					}

					if( Disposed || backgroundTaskNeedExit )
						return;

					//clients to close (disaproved)
					while( disapprovedClosingClients.TryDequeue( out var pair ) )
					{
						var client = pair.Item1;
						var rejectReason = pair.Item2;
						try
						{
							await client.CloseAsync( ConnectionCloseStatus.ProtocolError, rejectReason );
						}
						catch { }
					}

					//wait for new messages
					await backgroundTaskSemaphore.WaitAsync();
				}
			}
			catch( Exception e )
			{
				Log.Warning( "SendingAndDisapprovedClosingClientsTask exception: " + e.ToString() );
				//Console.WriteLine( "SendingAndDisapprovedClosingClientsTask exception: " + e.ToString() );
			}
		}

		public virtual void Dispose()
		{
			//wait to exit from the task
			var backgroundTask2 = backgroundTask;
			if( backgroundTask2 != null )
			{
				backgroundTaskNeedExit = true;
				backgroundTaskSemaphore.Set();

				for( var counter = 0; !backgroundTask2.IsCompleted && counter < 500; counter++ )
					Thread.Sleep( 1 );

				backgroundTask = null;
			}

			//remove connected clients
			lock( clients )
			{
				while( clients.Count != 0 )
				{
					foreach( var node in GetClientsArray() )
						RemoveClient( node );
				}
			}

			//stop web socket server
			try
			{
				webSocketListener?.Stop();
			}
			catch { }
			webSocketListener = null;

			//stop udp server
			try
			{
				udpListener?.Server?.Stop();
			}
			catch { }
			udpListener = null;

			//dispose services
			foreach( var service in services.ToArray().GetReverse() )
				service.PerformDispose();
			//services.Clear();

			backgroundTaskSemaphore?.Dispose();

			lock( instances )
				instances.Remove( this );

			disposed = true;
		}

		protected virtual void OnUpdate( DateTime utcNow )
		{
			if( Disposed )
				return;

			//update udp server
			if( udpListener != null )
			{
				UpdateUdpMaxFragmentsCount_DisconnectTimeout_UpdateTime();
				udpListener?.Server.PollEvents();
			}

			//profiler
			{
				var profilerData2 = ProfilerData;
				if( profilerData2 != null )
				{
					var workedTime = utcNow - profilerData2.TimeStarted;
					if( workedTime.TotalSeconds >= profilerData2.WorkingTime )
						ProfilerStop( true );
				}
			}

			//delete freezed connecting clients
			if( ( utcNow - connectingClientsDeleteFreezedLastTime ).TotalSeconds > 1 )
			{
				connectingClientsDeleteFreezedLastTime = utcNow;

				if( connectingClients.Count > 0 )
				{
					foreach( var client in connectingClients.Keys )
					{
						if( ( utcNow - client.CreationTime ).TotalSeconds > 30 )
						{
							//disapprove by timeout
							connectingClients.TryRemove( client, out _ );

							var error = "Timeout when connecting.";
							if( !client.connectingApproved.HasValue && string.IsNullOrEmpty( client.connectingRejectedReason ) )
								error += " The connection was not approved or rejected in time.";

							disapprovedClosingClients.Enqueue( (client, error) );
							backgroundTaskSemaphore.Set();
						}
					}
				}
			}

			//process connecting clients
			if( ( utcNow - connectingClientsUpdateLastTime ).TotalSeconds > 0.2 )
			{
				connectingClientsUpdateLastTime = utcNow;

				if( connectingClients.Count > 0 )
				{
					foreach( var client in connectingClients.Keys )
					{
						if( client.connectingApproved.HasValue )
						{
							//remove from connecting clients
							connectingClients.TryRemove( client, out _ );

							if( client.connectingApproved.Value )
							{
								//approved

								//additional check for Connecting status
								if( client.status == NetworkStatus.Connecting )
								{
									//add to the list of connected clients
									lock( clients )
										clients.Add( client );
									clientsArray = null;
									clientByLoginDataUserID = null;

									//generate reconnect token
									if( AllowReconnect && client.realConnection?.WebSocketContext != null )
										client.reconnectToken = Guid.NewGuid().ToString();

									//can send settings via networkServiceInternal
									//send settings to the client before status change (KeepAliveTime, ReconnectToken)
									client.ToProcessMessagesEnqueue( new Client.ToProcessMessage { SendSettings = true } );

									//send data to client before status change. to do things before Connected status on the client
									ClientBeforeStatusChangeToConnected?.Invoke( this, client );

									//update status
									client.status = NetworkStatus.Connected;

									//set Connected status on the client
									networkServiceInternal.SendStatusConnectedToClient( client );

									//notify status change to do useful things after status is set to Connected on the client
									ClientStatusChanged?.Invoke( this, client, "" );

									//start handling connection. only for web socket
									if( client.realConnection?.WebSocketContext != null )
									{
										TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Forever, "ServerNode: OnUpdate: HandleConnectionAsync", async delegate
										{
											try
											{
												await client.HandleConnectionWebSocketAsync();
											}
											catch( Exception e )
											{
												if( trace )
													Log.Info( "HandleConnectionAsync exception: " + e.Message );
												try
												{
													client.OnClose( ConnectionCloseStatus.ProtocolError, e.Message );
												}
												catch { }
											}
										} );
									}
								}
							}
							else
							{
								//disapproved
								disapprovedClosingClients.Enqueue( (client, client.connectingRejectedReason) );
								backgroundTaskSemaphore.Set();
							}
						}
					}
				}
			}

			//process received messages
			while( receivedMessages.TryDequeue( out var message ) )
			{
				var client = message.Client;

				if( message.DataBinary != null )
				{
					//data binary message

					var data = message.DataBinary;
					var reader = new ArrayDataReader( data );

					Interlocked.Add( ref client.receivedMessagesSize, -data.Length );
					Interlocked.Add( ref receivedMessagesSizeTotal, -data.Length );

					while( reader.CurrentPosition < reader.EndPosition )
					{
						var length = (int)reader.ReadVariableUInt32();
						if( length > ReceiveMessageMaxMessageSize )
						{
							var reason = "OnMessage: Message size limit exceeded.";
							OnReceiveProtocolErrorInternal( client, reason );
							client.ToProcessMessagesEnqueue( new Client.ToProcessMessage { CloseCommand = true, CloseStatusCode = ConnectionCloseStatus.ProtocolError, CloseReason = reason } );
							break;
						}

						ProcessReceivedMessage( client, data, reader.CurrentPosition, length );
						reader.ReadSkip( length );

						if( reader.Overflow )
						{
							var reason = "OnMessage: Read overflow.";
							OnReceiveProtocolErrorInternal( client, reason );
							client.ToProcessMessagesEnqueue( new Client.ToProcessMessage { CloseCommand = true, CloseStatusCode = ConnectionCloseStatus.ProtocolError, CloseReason = reason } );
							break;
						}
					}
				}
				else if( message.CloseReason != null )
				{
					//close message

					//update status
					if( client.status != NetworkStatus.Disconnected )
					{
						client.status = NetworkStatus.Disconnected;
						ClientStatusChanged?.Invoke( this, client, message.CloseReason );
					}

					if( message.CloseCode == ConnectionCloseStatus.ProtocolError )
						OnReceiveProtocolErrorInternal( client, message.CloseReason );

					RemoveClient( client, message.CloseCode, message.CloseReason );
				}
				else if( message.ErrorMessage != null )
				{
					//error message

					//update status
					if( client.status != NetworkStatus.Disconnected )
					{
						client.status = NetworkStatus.Disconnected;
						ClientStatusChanged?.Invoke( this, client, message.ErrorMessage );
					}

					OnReceiveProtocolErrorInternal( client, message.ErrorMessage );

					RemoveClient( client, ConnectionCloseStatus.ProtocolError, message.ErrorMessage );
				}
			}

			//update services
			for( int n = 0; n < services.Count; n++ )
				services[ n ].OnUpdate();

			//updates
			{
				var clients = GetClientsArray();

				//!!!!maybe for many clients better send not all at once

				//send ping
				if( ( utcNow - sentPingLastTime ).TotalSeconds > sendPingIntervalInSeconds )
				{
					sentPingLastTime = utcNow;

					if( clients.Length < 64 )
					{
						foreach( var client in clients )
							client.SendPingUpdate( utcNow );
					}
					else
					{
						Parallel.ForEach( clients, delegate ( Client client )
						{
							client.SendPingUpdate( utcNow );
						} );
					}
				}

				//send accumulated messages
				if( clients.Length < 64 )
				{
					foreach( var client in clients )
						client.ProcessAccumulatedMessagesToSend();
				}
				else
				{
					Parallel.ForEach( clients, delegate ( Client client )
					{
						client.ProcessAccumulatedMessagesToSend();
					} );
				}

				//drop by
				DropByMaxLifetimeAndByKeepAliveTime( clients, utcNow );
			}
		}

		public void DisconnectClient( Client client, string reason = null )
		{
			client.ProcessAccumulatedMessagesToSend();

			if( string.IsNullOrEmpty( reason ) )
			{
				clientsMustNormalDisconnect.Enqueue( (client, DateTime.UtcNow + new TimeSpan( 0, 0, 2 )) );
				//client.mustNormalDisconnectTime = DateTime.UtcNow + new TimeSpan( 0, 0, 2 );
				backgroundTaskSemaphore.Set();
			}
			else
			{
				client.ToProcessMessagesEnqueue( new Client.ToProcessMessage { CloseCommand = true, CloseStatusCode = string.IsNullOrEmpty( reason ) ? ConnectionCloseStatus.NormalClosure : ConnectionCloseStatus.ProtocolError, CloseReason = reason ?? "" } );
			}
		}

		void RemoveClient( Client client, ConnectionCloseStatus closeCode, string closeReason )
		{
			var realConnection = client.realConnection;
			if( realConnection != null )
			{
				TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "ServerNode: RemoveClient: CloseOutputAsync", async delegate ()
				{
					try
					{
						await Client.CloseOutputAsync( realConnection.WebSocketContext?.WebSocket, realConnection.UdpPeer, closeCode, closeReason );
					}
					catch { }
				} );

				client.realConnection = null;
			}

			RemoveClient( client );

			//if( closeHttpContext )
			//{
			//	var httpContext = client.realConnection?.HttpContext;
			//	if( httpContext != null )
			//	{
			//		try
			//		{
			//			httpContext.Response.StatusCode = 400;
			//			httpContext.Response.Close();
			//		}
			//		catch { }

			//		client.realConnection = null;
			//	}
			//}
		}

		void RemoveClient( Client client )
		{
			lock( clients )
				clients.Remove( client );
			clientsArray = null;
			clientByLoginDataUserID = null;
		}

		public IList<ServerService> Services
		{
			get { return servicesReadOnly; }
		}

		protected void RegisterService( ServerService service )
		{
			if( service.owner != null )
				Log.Fatal( "ServerNode: RegisterService: Service is already registered." );
			if( service.Identifier < 0 )
				Log.Fatal( "ServerNode: RegisterService: Invalid service identifier. Identifier can not be zero or negative." );
			if( service.Identifier > maxServiceIdentifier )
				Log.Fatal( "ServerNode: RegisterService: Invalid service identifier. Maximum identifier is \"{0}\".", maxServiceIdentifier );

			//check for unique identifier
			{
				var checkService = GetService( service.Identifier );
				if( checkService != null )
					Log.Fatal( "ServerNode: RegisterService: Service with identifier \"{0}\" is already registered.", service.Identifier );
			}

			//check for unique name
			{
				var checkService = GetService( service.Name );
				if( checkService != null )
					Log.Fatal( "ServerNode: RegisterService: Service with name \"{0}\" is already registered.", service.Name );
			}

			service.owner = this;
			services.Add( service );
			servicesByIdentifier[ service.Identifier ] = service;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal ServerService GetService( int identifier )
		{
			if( identifier >= servicesByIdentifier.Length )
				return null;
			return servicesByIdentifier[ identifier ];
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal ServerService GetService( string name )
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
		public virtual T GetService<T>() where T : ServerService
		{
			for( int n = 0; n < services.Count; n++ )
			{
				var service = services[ n ];
				if( typeof( T ).IsAssignableFrom( service.GetType() ) )
					return (T)service;
			}
			return null;
		}

		//protected virtual void OnReceiveProtocolError( ConnectedNode sender, string message ) { }

		internal void OnReceiveProtocolErrorInternal( Client client, string message )
		{
			if( trace )
				Log.Info( $"OnReceiveProtocolErrorInternal: {message}" );

			if( !Disposed )
				ProtocolError?.Invoke( this, client, message );
			//OnReceiveProtocolError( sender, message );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Client[] GetClientsArray()
		{
			var array = clientsArray;
			if( array == null )
			{
				lock( clients )
					array = clients.ToArray();
				clientsArray = array;
			}
			return array;
		}

		public int ClientCount
		{
			get { return GetClientsArray().Length; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Client GetClientByLoginDataUserID( long userID )
		{
			//build dictionary
			var dictionary = clientByLoginDataUserID;
			if( dictionary == null )
			{
				var clientsArray = GetClientsArray();
				dictionary = new Dictionary<long, Client>( clientsArray.Length );
				foreach( var client in clientsArray )
				{
					if( client.LoginDataUserID != 0 )
						dictionary[ client.LoginDataUserID ] = client;
				}
				clientByLoginDataUserID = dictionary;
			}

			//try get
			{
				clientByLoginDataUserID.TryGetValue( userID, out var client );
				return client;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public List<Client> GetClientsByLoginDataUserID( IList<long> userIDs )
		{
			var clients = new List<Client>( userIDs.Count );
			foreach( var userID in userIDs )
			{
				var client = GetClientByLoginDataUserID( userID );
				if( client != null )
					clients.Add( client );
			}
			return clients;
		}

		void DropByMaxLifetimeAndByKeepAliveTime( Client[] clients, DateTime utcNow )
		{
			if( ( utcNow - dropByMaxLifetimeLastTime ).TotalSeconds > 1 )
			{
				dropByMaxLifetimeLastTime = utcNow;

				if( clients.Length < 64 )
				{
					foreach( var client in clients )
					{
						if( client.MaxLifetime > 0 && ( utcNow - client.CreationTime ).TotalSeconds > client.MaxLifetime )
						{
							client.ToProcessMessagesEnqueue( new Client.ToProcessMessage { CloseCommand = true, CloseStatusCode = ConnectionCloseStatus.EndpointUnavailable, CloseReason = "Max lifetime." } );
						}
						else if( client.GetRoundtripLastInSeconds( utcNow ) > KeepAliveTime )
						{
							client.ToProcessMessagesEnqueue( new Client.ToProcessMessage { CloseCommand = true, CloseStatusCode = ConnectionCloseStatus.EndpointUnavailable, CloseReason = "Keep alive time." } );
						}
					}
				}
				else
				{
					Parallel.ForEach( clients, delegate ( Client client )
					{
						if( client.MaxLifetime > 0 && ( utcNow - client.CreationTime ).TotalSeconds > client.MaxLifetime )
						{
							client.ToProcessMessagesEnqueue( new Client.ToProcessMessage { CloseCommand = true, CloseStatusCode = ConnectionCloseStatus.EndpointUnavailable, CloseReason = "Max lifetime." } );
						}
						else if( client.GetRoundtripLastInSeconds( utcNow ) > KeepAliveTime )
						{
							client.ToProcessMessagesEnqueue( new Client.ToProcessMessage { CloseCommand = true, CloseStatusCode = ConnectionCloseStatus.EndpointUnavailable, CloseReason = "Keep alive time." } );
						}
					} );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ProcessReceivedMessage( Client client, byte[] data, int position, int length )
		{
			var reader = new ArrayDataReader( data, position, length );

			var serviceIdentifier = reader.ReadByte();
			var messageIdentifier = reader.ReadByte();

			if( reader.Overflow )
			{
				OnReceiveProtocolErrorInternal( client, "Invalid message." );
				return;
			}

			//service message
			var service = GetService( serviceIdentifier );
			if( service == null )
			{
				//no such service
				return;
			}

			service.ProcessReceivedMessage( client, reader, length, messageIdentifier );
		}

		public long TotalDataMessagesReceivedCounter
		{
			get { return Interlocked.Read( ref totalDataMessagesReceivedCounter ); }
		}

		public long TotalDataSizeReceivedCounter
		{
			get { return Interlocked.Read( ref totalDataSizeReceivedCounter ); }
		}

		public long TotalDataMessagesSentCounter
		{
			get { return Interlocked.Read( ref totalDataMessagesSentCounter ); }
		}

		public long TotalDataSizeSentCounter
		{
			get { return Interlocked.Read( ref totalDataSizeSentCounter ); }
		}

		public void GetDataMessageStatistics( double updateTime, out double receivedMessages, out double receivedSize, out double sentMessages, out double sentSize )
		{
			receivedMessages = 0;
			receivedSize = 0;
			sentMessages = 0;
			sentSize = 0;

			double now = ( DateTime.UtcNow - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) ).TotalSeconds;

			double diff = now - statisticsLastUpdateTime;
			if( diff > updateTime )
			{
				statisticsLastUpdateTime = now;

				var receivedMessagesDiff = TotalDataMessagesReceivedCounter - statisticsLastUpdateReceivedMessages;
				var receivedSizeDiff = TotalDataSizeReceivedCounter - statisticsLastUpdateReceivedSize;
				var sentMessagesDiff = TotalDataMessagesSentCounter - statisticsLastUpdateSentMessages;
				var sentSizeDiff = TotalDataSizeSentCounter - statisticsLastUpdateSentSize;

				statisticsLastUpdateReceivedMessages = TotalDataMessagesReceivedCounter;
				statisticsLastUpdateReceivedSize = TotalDataSizeReceivedCounter;
				statisticsLastUpdateSentMessages = TotalDataMessagesSentCounter;
				statisticsLastUpdateSentSize = TotalDataSizeSentCounter;

				if( updateTime != 0 && diff < 5 )
				{
					statisticsLastUpdateReceivedMessagesPerSecond = receivedMessagesDiff / updateTime;
					statisticsLastUpdateReceivedSizePerSecond = receivedSizeDiff / updateTime;
					statisticsLastUpdateSentMessagesPerSecond = sentMessagesDiff / updateTime;
					statisticsLastUpdateSentSizePerSecond = sentSizeDiff / updateTime;
				}
				else
				{
					statisticsLastUpdateReceivedMessagesPerSecond = 0;
					statisticsLastUpdateReceivedSizePerSecond = 0;
					statisticsLastUpdateSentMessagesPerSecond = 0;
					statisticsLastUpdateSentSizePerSecond = 0;
				}
			}

			receivedMessages = statisticsLastUpdateReceivedMessagesPerSecond;
			receivedSize = statisticsLastUpdateReceivedSizePerSecond;
			sentMessages = statisticsLastUpdateSentMessagesPerSecond;
			sentSize = statisticsLastUpdateSentSizePerSecond;



			//Group group;
			//if( receive )
			//	group = includeLibraryMessages ? receivedGroupIncludeLibraryMessages : receivedGroup;
			//else
			//	group = includeLibraryMessages ? sentGroupIncludeLibraryMessages : sentGroup;

			//double now = ( utcNow - new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc ) ).TotalSeconds;
			////double now = NetTime.Now;

			//double diff = now - group.bytesPerSecondUpdateTime;
			//if( diff > .2f )
			//{
			//	long bytesTotal = receive ? GetBytesReceived( includeLibraryMessages ) : GetBytesSent( includeLibraryMessages );

			//	if( diff > 5 )
			//		group.bytesPerSecondUpdateTime = 0;

			//	if( group.bytesPerSecondUpdateTime != 0 )
			//	{
			//		long bytesDiff = bytesTotal - group.bytesPerSecondUpdateCount;
			//		group.bytesPerSecond = (float)( (double)bytesDiff / diff );
			//	}

			//	group.bytesPerSecondUpdateTime = now;
			//	group.bytesPerSecondUpdateCount = bytesTotal;
			//}

			//return group.bytesPerSecond;


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

		static long GetUniqueDebugConnectionCounter()
		{
			lock( debugConnectionCounterLock )
			{
				debugConnectionCounter++;
				return debugConnectionCounter;
			}
		}

		void UpdateUdpMaxFragmentsCount_DisconnectTimeout_UpdateTime()
		{
			var udpListener2 = udpListener;
			if( udpListener2 != null )
			{
				var server = udpListener2.Server;

				int mtu = server.MtuOverride;
				int headerSize = 16;
				int payloadPerFragment = mtu - headerSize;

				var maxMessageSize = Math.Max( ReceiveMessageMaxMessageSize, Math.Max( ReceiveMessageMaxInternalMessageSize, SendMessageMaxSize ) );
				server.MaxFragmentsCount = (ushort)( ( maxMessageSize + payloadPerFragment - 1 ) / payloadPerFragment );

				server.DisconnectTimeout = (int)( KeepAliveTime * 1000 );

				server.UpdateTime = (int)( UpdateTimeUdp * 1000 );
			}
		}
	}
}