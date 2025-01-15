// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !LIDGREN
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using Internal.WebSocketSharp;
using Internal.WebSocketSharp.Server;

namespace NeoAxis.Networking
{
	public abstract class ServerNode
	{
		readonly static bool trace = false;
		internal const int maxServiceIdentifier = 255;

		static List<ServerNode> instances = new List<ServerNode>();

		bool disposed;

		//general
		string serverName;
		string serverVersion;
		int maxConnections;
		double defaultMaxLifetime;
		int receiveDataMaxMessageSize = 10 * 1024 * 1024;

		//profiler
		ProfilerDataClass profilerData;

		//services
		List<ServerService> services = new List<ServerService>();
		ServerService[] servicesByIdentifier = new ServerService[ maxServiceIdentifier + 1 ];
		ReadOnlyCollection<ServerService> servicesReadOnly;

		//server data
		internal WebSocketServer server;
		ConcurrentQueue<Client> connectingClients = new ConcurrentQueue<Client>();
		ConcurrentQueue<(Client, string)> disapprovedClosingClients = new ConcurrentQueue<(Client, string)>();
		ESet<Client> clients = new ESet<Client>();
		Client[] clientsArray;

		DateTime dropByMaxLifetimeLastTime;

		Thread thread;
		bool threadNeedExit;

		//these values are changed from a background thread
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

		///////////////////////////////////////////////

		public sealed class Client
		{
			ServerNode owner;
			MyWebSocketBehavior connection;
			IPEndPoint remoteEndPoint;

			internal NetworkStatus lastStatus = NetworkStatus.Disconnected;
			string clientVersion;
			string loginData;

			DateTime creationTime;
			double maxLifetime;

			//specific
			public long LoginDataUserID { get; set; }
			public string LoginDataUsername { get; set; }
			public object Tag { get; set; }

			List<string> remoteServices;
			ReadOnlyCollection<string> remoteServicesAsReadOnly;

			//limits. it is solved by timeout
			internal ConcurrentQueue<ReceivedMessage> receivedMessages = new ConcurrentQueue<ReceivedMessage>();
			internal ConcurrentQueue<ToProcessMessage> toProcessMessages = new ConcurrentQueue<ToProcessMessage>();

			internal ArrayDataWriter accumulatedMessagesToSend = new ArrayDataWriter();

			//ArrayDataWriter sendCachedDataWriter = new ArrayDataWriter();
			//internal float lastRoundtripTime;

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
				//public string DataString;
				public byte[] DataBinary;
				public string CloseReason;
				public CloseStatusCode CloseCode;
				public string ErrorMessage;
			}

			/////////////////////

			public class ToProcessMessage
			{
				public string DataString;

				public bool DataBinary;
				public byte[] DataBinaryArray;
				//public ArraySegment<byte> DataBinarySegment;

				public bool Close;
				public CloseStatusCode CloseStatusCode;
				public string CloseReason;
			}

			/////////////////////

			internal Client( ServerNode owner, MyWebSocketBehavior connection, IPEndPoint remoteEndPoint, string clientVersion, string loginData )
			{
				this.owner = owner;
				this.connection = connection;
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

			public MyWebSocketBehavior Connection
			{
				get { return connection; }
			}

			public NetworkStatus Status
			{
				get { return lastStatus; }
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

			//public float LastRoundtripTime
			//{
			//	get
			//	{
			//		if( connection == null )
			//			return 0;
			//		return lastRoundtripTime;
			//	}
			//}

			internal void SetRemoteServices( List<string> remoteServices )
			{
				this.remoteServices = remoteServices;
				remoteServicesAsReadOnly = new ReadOnlyCollection<string>( this.remoteServices );
			}

			public IList<string> RemoteServices
			{
				get { return remoteServicesAsReadOnly; }
			}

			public string GetAddressText()
			{
				return RemoteEndPoint.ToString();

				//string text;
				//if( RemoteEndPoint != null )
				//	text = RemoteEndPoint.ToString();
				//else
				//	text = "Unknown address";
				//return text;
			}

			public double GetCurrentLifetime()
			{
				return ( DateTime.UtcNow - CreationTime ).TotalSeconds;
			}
		}

		///////////////////////////////////////////////

		public class MyWebSocketBehavior : WebSocketBehavior
		{
			internal ServerNode owner;
			Client client;

			//these values are changed from a background thread
			internal long dataMessagesReceivedCounter;
			internal long dataSizeReceivedCounter;
			internal uint dataMessagesReceivedChecksum;
			internal long dataMessagesSentCounter;
			internal long dataSizeSentCounter;
			internal uint dataMessagesSentChecksum;

			/////////////////////

			public ServerNode Owner { get { return owner; } }

			protected override void OnOpen()
			{
				base.OnOpen();

				try
				{
					if( owner.GetClientsArray().Length >= owner.maxConnections )
						throw new Exception( $"The maximum connections limit has been reached, which is set at {owner.maxConnections}." );

					var userEndPoint = UserEndPoint;
					if( userEndPoint == null )
						throw new Exception( "UserEndPoint is null." );

					if( trace )
						Log.Info( "OnOpen " + userEndPoint.ToString() );

					var welcomeBase64 = QueryString.Get( "welcome" );
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

					client = new Client( owner, this, userEndPoint, clientVersion, loginData );
					client.lastStatus = NetworkStatus.Connecting;
					client.MaxLifetime = owner.DefaultMaxLifetime;

					owner.connectingClients.Enqueue( client );
				}
				catch( Exception e )
				{
					Close( CloseStatusCode.InvalidData, e.Message );
				}
			}

			protected override void OnClose( CloseEventArgs e )
			{
				base.OnClose( e );

				//try
				//{

				var reason = e.Reason ?? "";
				client.receivedMessages.Enqueue( new Client.ReceivedMessage { CloseReason = reason, CloseCode = (CloseStatusCode)e.Code } );

				if( trace )
					Log.Info( "OnClose " + client.RemoteEndPoint.ToString() + " " + reason );

				//}
				//catch( Exception ex )
				//{
				//	if( trace )
				//		Log.Info( "OnClose exception: " + ex.ToString() );
				//}

			}

			protected override void OnError( ErrorEventArgs e )
			{
				base.OnError( e );

				var message = e.Message ?? "";
				client.receivedMessages.Enqueue( new Client.ReceivedMessage { ErrorMessage = message } );

				if( trace )
					Log.Info( "OnError " + client.RemoteEndPoint.ToString() + " " + message );

				//!!!!need?
				try
				{
					Close( CloseStatusCode.ServerError, message );
				}
				catch { }
			}

			protected override void OnMessage( MessageEventArgs e )
			{
				base.OnMessage( e );

				if( e.IsPing )
				{
					if( trace )
						Log.Info( "OnMessage Ping" );
				}
				else if( e.IsText )
				{
					//system commands

					if( trace )
						Log.Info( "OnMessage Text" );

					try
					{
						var text = e.Data;

						//!!!!check for hard to parse text block. maybe then not use TextBlock
						//!!!!!or use limits when parsing

						if( text.Length > 1000 )
							throw new Exception( "The system message is more than 1000 characters." );

						var rootBlock = TextBlock.Parse( text, out var error );
						if( !string.IsNullOrEmpty( error ) )
							throw new Exception( error );

						var command = rootBlock.GetAttribute( "Command" );

						if( command == "Checksum" )
						{
							var counter = long.Parse( rootBlock.GetAttribute( "Counter" ) );
							var checksum = uint.Parse( rootBlock.GetAttribute( "Checksum" ) );

							if( DataMessagesReceivedCounter != counter || dataMessagesReceivedChecksum != checksum )
							{
								throw new Exception( $"Invalid checksum. {DataMessagesReceivedCounter} != {counter} || {dataMessagesReceivedChecksum} != {checksum}" );
							}
						}
						else
							throw new Exception( "Unknown command." );
					}
					catch( Exception ex )
					{
						try
						{
							Close( CloseStatusCode.ProtocolError, ex.Message );
						}
						catch { }
					}

					//client.receivedMessages.Enqueue( new Client.ReceivedMessage { DataString = e.Data } );

				}
				else if( e.IsBinary )
				{
					//data commands

					var data = e.RawData;

					unchecked
					{
						Interlocked.Increment( ref dataMessagesReceivedCounter ); //dataMessagesReceivedCounter++;
						Interlocked.Add( ref dataSizeReceivedCounter, data.Length );
						foreach( var b in data )
							dataMessagesReceivedChecksum += b;
						Interlocked.Increment( ref owner.totalDataMessagesReceivedCounter );
						Interlocked.Add( ref owner.totalDataSizeReceivedCounter, data.Length );
					}

					if( trace )
						Log.Info( $"OnMessage Binary {data.Length} {DataMessagesReceivedCounter} {dataMessagesReceivedChecksum}" );

					if( data.Length > owner.ReceiveDataMaxMessageSize )
					{
						var error = $"The size of the received message is too large. The maximum size is {owner.ReceiveDataMaxMessageSize} bytes.";
						try
						{
							Close( CloseStatusCode.ProtocolError, error );
						}
						catch { }
					}
					else
					{
						//totalBytesReceived += data.Length;

						//!!!!without copy?

						client.receivedMessages.Enqueue( new Client.ReceivedMessage { DataBinary = (byte[])data.Clone() } );
					}
				}
			}

			internal void Send2( string data )
			{
				Send( data );
			}

			internal void Send2( byte[] data )
			{
				Send( data );
			}

			internal void Close2( CloseStatusCode code, string reason )
			{
				Close( code, reason );
			}

			public long DataMessagesReceivedCounter
			{
				get { return Interlocked.Read( ref dataMessagesReceivedCounter ); }
			}

			public long DataSizeReceivedCounter
			{
				get { return Interlocked.Read( ref dataSizeReceivedCounter ); }
			}

			public long DataMessagesSentCounter
			{
				get { return Interlocked.Read( ref dataMessagesSentCounter ); }
			}

			public long DataSizeSentCounter
			{
				get { return Interlocked.Read( ref dataSizeSentCounter ); }
			}

			//!!!!
			//public void GetDataMessagesStatistics( DateTime utcNow, out double receivedMessagesPerSecond, out double reveivedSizePerSecond, out double sentMessagesPerSecond, out double sentSizePerSecond )
			//{
			//	receivedMessagesPerSecond = 0;
			//	reveivedSizePerSecond = 0;
			//	sentMessagesPerSecond = 0;
			//	sentSizePerSecond = 0;

			//}
		}

		///////////////////////////////////////////////

		public class ProfilerDataClass
		{
			//const data
			public DateTime TimeStarted;
			public double WorkingTime;
			//public long SystemMessagesReceivedStartCounter;
			//public long SystemMessagesSentStartCounter;

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

		public delegate void ProtocolErrorDelegate( ServerNode sender, Client client, string message );
		public event ProtocolErrorDelegate ProtocolError;

		public delegate void IncomingConnectionApprovalDelegate( ServerNode sender, Client client, ref string rejectReason );
		public event IncomingConnectionApprovalDelegate IncomingConnectionApproval;

		public delegate void ClientStatusChangedDelegate( ServerNode sender, Client client, string message );
		public event ClientStatusChangedDelegate ClientStatusChanged;

		///////////////////////////////////////////////

		public static ServerNode[] GetInstances()
		{
			lock( instances )
				return instances.ToArray();
		}

		public void Update()
		{
			OnUpdate();
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
			//profilerData.SystemMessagesReceivedStartCounter = DataMessagesReceivedCounter;
			//profilerData.SystemMessagesSentStartCounter = DataMessagesSentCounter;

			Log.Info( "Network profiler started." );
		}

		public void ProfilerStop( bool writeToLogs )
		{
			if( profilerData == null )
				return;

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

		static string FormatCount( long count )
		{
			return count.ToString( "N0" );
		}

		void DumpProfilerDataToLogs()
		{
			var lines = new List<string>();

			lines.Add( "--------------------------------------------------------------" );
			lines.Add( string.Format( "Total received; {0}", NetworkUtility.FormatSize( profilerData.TotalReceivedSize ) ) );
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

						lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.ReceivedMessages ), NetworkUtility.FormatSize( messageByTypeItem.ReceivedSize ) ) );

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
								lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), NetworkUtility.FormatSize( item.Item2.Size ) ) );
							}
						}
					}


					//for( int messageTypeId = 0; messageTypeId < serviceItem.MessagesByType.Count; messageTypeId++ )
					//{
					//	var messageType = service.GetMessageType( (byte)messageTypeId );
					//	if( messageType != null )
					//	{
					//		var messageByTypeItem = serviceItem.GetMessageTypeItem( messageTypeId );
					//		if( messageByTypeItem != null && messageByTypeItem.ReceivedMessages != 0 )
					//		{
					//			lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.ReceivedMessages ), FormatSize( messageByTypeItem.ReceivedSize ) ) );

					//			var customData = messageByTypeItem.ReceivedCustomData;
					//			if( customData != null )
					//			{
					//				var items = new List<(string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData)>( customData.Count );
					//				foreach( var item in customData )
					//					items.Add( (item.Key, item.Value) );

					//				CollectionUtility.MergeSort( items, delegate ( (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item1, (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item2 )
					//				{
					//					if( item1.Item2.Size > item2.Item2.Size )
					//						return -1;
					//					if( item1.Item2.Size < item2.Item2.Size )
					//						return 1;
					//					return 0;
					//				} );

					//				foreach( var item in items )
					//				{
					//					lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), FormatSize( item.Item2.Size ) ) );
					//				}
					//			}
					//		}
					//	}
					//}
				}
			}

			lines.Add( "--------------------------------------------------------------" );
			lines.Add( string.Format( "Total sent; Size: {0}", NetworkUtility.FormatSize( profilerData.TotalSentSize ) ) );
			//lines.Add( string.Format( "Total sent. Messages: {0}; Size: {1}", FormatCount( profilerData.TotalSentMessages ), FormatSize( profilerData.TotalSentSize ) ) );

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

						lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.SentMessages ), NetworkUtility.FormatSize( messageByTypeItem.SentSize ) ) );

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
								lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), NetworkUtility.FormatSize( item.Item2.Size ) ) );
							}
						}
					}


					//for( int messageTypeId = 0; messageTypeId < serviceItem.MessagesByType.Count; messageTypeId++ )
					//{
					//	var messageType = service.GetMessageType( messageTypeId );
					//	if( messageType != null )
					//	{
					//		var messageByTypeItem = serviceItem.GetMessageTypeItem( messageTypeId );
					//		if( messageByTypeItem != null && messageByTypeItem.SentMessages != 0 )
					//		{
					//			lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.SentMessages ), FormatSize( messageByTypeItem.SentSize ) ) );

					//			var customData = messageByTypeItem.SentCustomData;
					//			if( customData != null )
					//			{
					//				var items = new List<(string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData)>( customData.Count );
					//				foreach( var item in customData )
					//					items.Add( (item.Key, item.Value) );

					//				CollectionUtility.MergeSort( items, delegate ( (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item1, (string, ProfilerDataClass.ServiceItem.MessageTypeItem.CustomData) item2 )
					//				{
					//					if( item1.Item2.Size > item2.Item2.Size )
					//						return -1;
					//					if( item1.Item2.Size < item2.Item2.Size )
					//						return 1;
					//					return 0;
					//				} );

					//				foreach( var item in items )
					//				{
					//					lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), FormatSize( item.Item2.Size ) ) );
					//				}
					//			}
					//		}
					//	}
					//}
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
		protected ServerNode( string serverName, string serverVersion, int maxConnections, double defaultMaxLifetime )
		{
			lock( instances )
				instances.Add( this );

			this.serverName = serverName;
			this.serverVersion = serverVersion;
			this.maxConnections = maxConnections;
			this.defaultMaxLifetime = defaultMaxLifetime;

			servicesReadOnly = new ReadOnlyCollection<ServerService>( services );
		}

		public bool BeginListen( int port, double connectionTimeout, out string error )
		{
			error = null;

#if !UWP
			if( Disposed )
				Log.Fatal( "ServerNode: BeginListen: The server has been disposed." );
			if( server != null )
				Log.Fatal( "ServerNode: BeginListen: The server is already initialized." );


			thread = new Thread( ThreadFunction );
			thread.IsBackground = true;
			thread.Start();


			//!!!!bool secure

			//!!!!limit max message size?

			server = new WebSocketServer( port );
			server.WaitTime = TimeSpan.FromSeconds( connectionTimeout );
			server.KeepClean = true;

			server.AddWebSocketService( "/service", delegate ( MyWebSocketBehavior behavior )
			{
				behavior.owner = this;
			} );


			//server.Log.Level = LogLevel.Trace;

			//lidgren
			//config.ReceiveBufferSize = 131071 * 10;
			//config.SendBufferSize = 131071 * 10;
			//config.AutoExpandMTU = true;//with it much faster


			try
			{
				//start listening for connections
				server.Start();

				if( !server.IsListening )
					throw new Exception( "The server is not listening." );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
#else
			error = "No network implementation for the platform.";
			return false;
#endif
		}

		void ThreadFunction( object param )
		{

			//!!!!non thread variant

			//!!!!slowly? GC?


			while( !Disposed && !threadNeedExit )
			{
				foreach( var client in GetClientsArray() )
				{
					var connection = client.Connection;

					while( connection != null && client.toProcessMessages.TryDequeue( out var message ) )
					{
						//if( message.DataString != null )
						//{
						//	//try
						//	//{
						//	//	connection.Send2( message.DataString );
						//	//}
						//	//catch( Exception e )
						//	//{
						//	//	connection.Close2( CloseStatusCode.ProtocolError, "Unable to send the string message. " + e.Message );
						//	//}
						//}
						//else 
						if( message.DataBinary )
						{
							try
							{
								var data = message.DataBinaryArray;

								unchecked
								{
									Interlocked.Increment( ref connection.dataMessagesSentCounter ); //connection.dataMessagesSentCounter++;
									Interlocked.Add( ref connection.dataSizeSentCounter, data.Length );
									foreach( var b in data )
										connection.dataMessagesSentChecksum += b;
									Interlocked.Increment( ref totalDataMessagesSentCounter );
									Interlocked.Add( ref totalDataSizeSentCounter, data.Length );
								}

								if( trace )
								{
									Log.Info( $"Send Binary {data.Length} {connection.DataMessagesSentCounter} {connection.dataMessagesSentChecksum}" );
								}

								//!!!!GC inside
								connection.Send2( data );
							}
							catch( Exception e )
							{
								connection.Close2( CloseStatusCode.ProtocolError, "Unable to send the binary message. " + e.Message );
								break;
							}

							//send checksum
							if( connection.DataMessagesSentCounter % 100 == 0 )
							{
								try
								{
									var rootBlock = new TextBlock();
									rootBlock.SetAttribute( "Command", "Checksum" );
									rootBlock.SetAttribute( "Counter", connection.DataMessagesSentCounter.ToString() );
									rootBlock.SetAttribute( "Checksum", connection.dataMessagesSentChecksum.ToString() );
									var text = rootBlock.DumpToString( false );

									if( trace )
										Log.Info( $"Send Text Checksum {connection.DataMessagesSentCounter} {connection.dataMessagesSentChecksum}" );

									connection.Send2( text );
								}
								catch( Exception e )
								{
									connection.Close2( CloseStatusCode.ProtocolError, "Unable to send the checksum command message. " + e.Message );
									break;
								}
							}

						}
						else if( message.Close )
						{
							try
							{
								var reasonClamped = message.CloseReason;
								if( reasonClamped.Length > 110 )
									reasonClamped = message.CloseReason.Substring( 0, 110 ) + "...";

								connection.Close2( message.CloseStatusCode, reasonClamped );
							}
							catch { }
						}

						if( Disposed || threadNeedExit )
							break;
					}

					if( Disposed || threadNeedExit )
						break;
				}

				//clients to close (disaproved)
				while( disapprovedClosingClients.TryDequeue( out var pair ) )
				{
					var client = pair.Item1;
					var rejectReason = pair.Item2;

					try
					{
						client.Connection.Close2( CloseStatusCode.InvalidData, rejectReason );
					}
					catch { }
				}

				Thread.Sleep( 1 );
			}
		}

		public virtual void Dispose()// string reason )
		{
			//exit from the thread
			if( thread != null )
			{
				threadNeedExit = true;
				//!!!!how to make without pause?
				Thread.Sleep( 50 );
				thread = null;
			}

			//dispose connected nodes
			lock( clients )
			{
				while( clients.Count != 0 )
				{
					foreach( var node in GetClientsArray() )
						RemoveClient( node );
				}
			}

			if( server != null )
			{
				server.Stop();
				//server.Shutdown( reason );

				server = null;
			}

			//dispose services
			foreach( var service in services.ToArray().Reverse() )
				service.PerformDispose();
			//services.Clear();
			////while( services.Count != 0 )
			////{
			////	var service = services[ services.Count - 1 ];
			////	service.PerformDispose();
			////	services.Remove( service );
			////}

			lock( instances )
				instances.Remove( this );

			disposed = true;
		}

		protected virtual void OnUpdate()
		{
			if( ProfilerData != null )
			{
				var workedTime = DateTime.UtcNow - ProfilerData.TimeStarted;
				if( workedTime.TotalSeconds >= ProfilerData.WorkingTime )
					ProfilerStop( true );
			}

#if !UWP

			//process connecting clients
			while( connectingClients.TryDequeue( out var client ) )
			{
				var rejectReason = "";
				if( OnIncomingConnectionApproval( client, ref rejectReason ) )
				{
					//approve

					//add to the list of clients
					lock( clients )
						clients.Add( client );
					clientsArray = null;

					//update status
					if( client.lastStatus != NetworkStatus.Connected )
					{
						client.lastStatus = NetworkStatus.Connected;
						OnClientStatusChanged( client, "" );
					}
				}
				else
				{
					//disapprove
					disapprovedClosingClients.Enqueue( (client, rejectReason) );
				}
			}

			//process received messages
			foreach( var client in GetClientsArray() )
			{
				while( client.receivedMessages.TryDequeue( out var message ) )
				{
					//if( message.DataString != null )
					//{
					//	//system string message

					//	//try
					//	//{
					//	//}
					//	//catch( Exception ex )
					//	//{
					//	//}

					//}
					//else 
					if( message.DataBinary != null )
					{
						//data binary message

						var data = message.DataBinary;
						var reader = new ArrayDataReader( data );

						while( reader.CurrentPosition < reader.EndPosition )
						{
							var length = (int)reader.ReadVariableUInt32();
							ProcessReceivedMessage( client, data, reader.CurrentPosition, length );
							reader.ReadSkip( length );

							if( reader.Overflow )
							{
								var reason = "OnMessage: Read overflow.";
								OnReceiveProtocolErrorInternal( client, reason );
								client.toProcessMessages.Enqueue( new Client.ToProcessMessage { Close = true, CloseStatusCode = CloseStatusCode.ProtocolError, CloseReason = reason } );

								break;
							}
						}
					}
					else if( message.CloseReason != null )
					{
						//close message

						//update status
						if( client.lastStatus != NetworkStatus.Disconnected )
						{
							client.lastStatus = NetworkStatus.Disconnected;
							OnClientStatusChanged( client, message.CloseReason );
						}

						if( message.CloseCode == CloseStatusCode.ProtocolError )
							OnReceiveProtocolErrorInternal( client, message.CloseReason );

						//now disconnected
						RemoveClient( client );
					}
					else if( message.ErrorMessage != null )
					{
						//error message

						var text = message.ErrorMessage;

						OnReceiveProtocolErrorInternal( client, text );

						//update status
						if( client.lastStatus != NetworkStatus.Disconnected )
						{
							client.lastStatus = NetworkStatus.Disconnected;
							OnClientStatusChanged( client, text );
						}

						//now disconnected
						RemoveClient( client );
					}
				}
			}

			//update services
			for( int n = 0; n < services.Count; n++ )
				services[ n ].OnUpdate();

			//send accumulated messages
			foreach( var client in GetClientsArray() )
			{
				//!!!!parallel? where else

				if( client.accumulatedMessagesToSend.Length > 0 )
				{
					var array = client.accumulatedMessagesToSend.ToArray();
					client.toProcessMessages.Enqueue( new Client.ToProcessMessage { DataBinary = true, DataBinaryArray = array } );

					client.accumulatedMessagesToSend.Reset();
				}
			}

			DropByMaxLifetime();
#endif
		}

		protected virtual bool OnIncomingConnectionApproval( Client client, ref string rejectReason )
		{
			if( string.IsNullOrEmpty( client.LoginData ) )
			{
				rejectReason = "Empty login data.";
				return false;
			}

			if( !Disposed )
			{
				IncomingConnectionApproval?.Invoke( this, client, ref rejectReason );
				if( !string.IsNullOrEmpty( rejectReason ) )
					return false;
			}

			return true;
		}

		protected virtual void OnClientStatusChanged( Client client, string message )
		{
			if( !Disposed )
				ClientStatusChanged?.Invoke( this, client, message );
		}

		public void DisconnectClient( Client client, string reason )
		{

			//!!!!CloseStatusCode.Normal close?

			client.toProcessMessages.Enqueue( new Client.ToProcessMessage { Close = true, CloseStatusCode = CloseStatusCode.InvalidData, CloseReason = reason ?? "" } );
		}

		void RemoveClient( Client client )
		{
			lock( clients )
				clients.Remove( client );
			clientsArray = null;
		}

		public IList<ServerService> Services
		{
			get { return servicesReadOnly; }
		}

		protected void RegisterService( ServerService service )
		{
			if( service.owner != null )
				Log.Fatal( "ServerNode: RegisterService: Service is already registered." );
			if( service.Identifier == 0 )
				Log.Fatal( "ServerNode: RegisterService: Invalid service identifier. Identifier can not be zero." );
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

		//protected virtual void OnReceiveProtocolError( ConnectedNode sender, string message ) { }

		internal void OnReceiveProtocolErrorInternal( Client client, string message )
		{
			if( trace )
				Log.Info( $"OnReceiveProtocolErrorInternal: {message}" );

			if( !Disposed )
				ProtocolError?.Invoke( this, client, message );
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

		public int MaxConnections
		{
			get { return maxConnections; }
			set { maxConnections = value; }
		}

		public double DefaultMaxLifetime
		{
			get { return defaultMaxLifetime; }
			set { defaultMaxLifetime = value; }
		}

		public int ReceiveDataMaxMessageSize
		{
			get { return receiveDataMaxMessageSize; }
			set { receiveDataMaxMessageSize = value; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Client[] GetClientsArray()
		{
			var clientsArray2 = clientsArray;
			if( clientsArray2 == null )
			{
				lock( clients )
					clientsArray2 = clients.ToArray();
				clientsArray = clientsArray2;
			}
			return clientsArray2;
		}

		public int ClientCount
		{
			get { return GetClientsArray().Length; }
		}

		void DropByMaxLifetime()
		{
			if( server != null )
			{
				var now = DateTime.UtcNow;

				if( ( now - dropByMaxLifetimeLastTime ).TotalSeconds > 1 )
				{
					dropByMaxLifetimeLastTime = now;

					foreach( var client in GetClientsArray() )
					{
						if( client.MaxLifetime > 0 && ( now - client.CreationTime ).TotalSeconds > client.MaxLifetime )
						{
							client.toProcessMessages.Enqueue( new Client.ToProcessMessage { Close = true, CloseStatusCode = CloseStatusCode.Away, CloseReason = "Max lifetime." } );
						}
					}
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ProcessReceivedMessage( Client client, byte[] data, int position, int length )
		{
			var reader = new ArrayDataReader( data, position, length );

			var pair = reader.ReadByte();
			var serviceIdentifier = (byte)( pair >> 4 );
			var messageIdentifier = (byte)( pair & 15 );

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
	}
}
#endif
