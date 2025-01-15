// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !LIDGREN
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Internal.WebSocketSharp;

namespace NeoAxis.Networking
{
	public abstract class ClientNode
	{
		readonly static bool trace = false;
		internal const int maxServiceIdentifier = 255;

		bool disposed;

		//profiler
		ProfilerDataClass profilerData;

		string clientVersion = "";
		string loginData = "";
		string remoteServerName = "";

		//connection state
		NetworkStatus lastStatus;
		bool beginConnecting;
		bool failedToConnect;
		//ESet<string> serverServices = new ESet<string>();

		string disconnectionReason = "";
		double lastReceivedMessageTime;

		//services
		List<ClientService> services = new List<ClientService>();
		ClientService[] servicesByIdentifier = new ClientService[ maxServiceIdentifier + 1 ];
		ReadOnlyCollection<ClientService> servicesReadOnly;

		//client data
		internal WebSocket client;

		Thread thread;
		bool threadNeedExit;
		bool threadNeedExitSendClose;

		//limits. it is solved by timeout
		internal ConcurrentQueue<ReceivedMessage> receivedMessages = new ConcurrentQueue<ReceivedMessage>();
		internal ConcurrentQueue<ToProcessMessage> toProcessMessages = new ConcurrentQueue<ToProcessMessage>();

		//these values are changed from a background thread
		long dataMessagesReceivedCounter;
		long dataSizeReceivedCounter;
		uint dataMessagesReceivedChecksum;
		long dataMessagesSentCounter;
		long dataSizeSentCounter;
		uint dataMessagesSentChecksum;

		internal ArrayDataWriter accumulatedMessagesToSend = new ArrayDataWriter();

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
			profilerData.TimeStarted = DateTime.Now;
			profilerData.WorkingTime = workingTime;
			profilerData.SystemMessagesReceivedStartCounter = DataMessagesReceivedCounter;
			profilerData.SystemMessagesSentStartCounter = DataMessagesSentCounter;

			Log.Info( "Network profiler started." );
		}

		public void ProfilerStop( bool writeToLogs )
		{
			if( profilerData == null )
				return;

			var workedTime = ( DateTime.Now - ProfilerData.TimeStarted ).TotalSeconds;
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

		static string FormatCount( long count )
		{
			return count.ToString( "N0" );
		}

		//static string FormatSize( long byteCount )
		//{
		//	//copyright: from LiteDB
		//	var suf = new[] { "B", "KB", "MB", "GB", "TB" }; //Longs run out around EB
		//	if( byteCount == 0 ) return "0 " + suf[ 0 ];
		//	var bytes = Math.Abs( byteCount );
		//	var place = Convert.ToInt64( Math.Floor( Math.Log( bytes, 1024 ) ) );
		//	var num = Math.Round( bytes / Math.Pow( 1024, place ), 1 );
		//	return ( Math.Sign( byteCount ) * num ).ToString() + " " + suf[ place ];
		//}

		void DumpProfilerDataToLogs()
		{
			var systemMessagesReceived = DataMessagesReceivedCounter - profilerData.SystemMessagesReceivedStartCounter;
			var systemMessagesSent = DataMessagesSentCounter - profilerData.SystemMessagesSentStartCounter;

			var lines = new List<string>();

			lines.Add( "--------------------------------------------------------------" );
			lines.Add( string.Format( "Total received; {0}", NetworkUtility.FormatSize( profilerData.TotalReceivedSize ) ) );
			//lines.Add( string.Format( "Total received. Messages: {0}; Size: {1}", FormatCount( profilerData.TotalReceivedMessages ), FormatSize( profilerData.TotalReceivedSize ) ) );

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

		public delegate void ProtocolErrorDelegate( ClientNode sender, string message );
		public event ProtocolErrorDelegate ProtocolError;

		public delegate void ConnectionStatusChangedDelegate( ClientNode sender );
		public event ConnectionStatusChangedDelegate ConnectionStatusChanged;

		///////////////////////////////////////////////

		public class ReceivedMessage
		{
			//public string DataString;
			public byte[] DataBinary;
			public string CloseReason;
			public CloseStatusCode CloseCode;
			public string ErrorMessage;
		}

		///////////////////////////////////////////////

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

		///////////////////////////////////////////////

		protected ClientNode()
		{
			servicesReadOnly = new ReadOnlyCollection<ClientService>( services );
		}

		public bool BeginConnect( string host, int port, string clientVersion, string loginData, double connectionTimeout, out string error )
		{
			this.clientVersion = clientVersion;
			this.loginData = loginData;

			error = null;

#if !UWP
			if( Disposed )
				Log.Fatal( "NetworkClient: BeginConnect: The client is disposed." );
			if( client != null )
				Log.Fatal( "NetworkClient: BeginConnect: The client is already initialized." );

			disconnectionReason = "";

			string welcomeBase64;
			{
				var rootBlock = new TextBlock();
				rootBlock.SetAttribute( "ClientVersion", clientVersion );
				rootBlock.SetAttribute( "LoginData", loginData );
				//foreach( var service in Services )
				//	rootBlock.AddChild( "ClientService", service.Name );
				var welcome = rootBlock.DumpToString( false );
				welcomeBase64 = StringUtility.EncodeToBase64URL( welcome );
			}

			client = new WebSocket( $"ws://{host}:{port}/service?welcome={welcomeBase64}" );
			client.WaitTime = TimeSpan.FromSeconds( connectionTimeout );

			client.OnOpen += Client_OnOpen;
			client.OnMessage += Client_OnMessage;
			client.OnClose += Client_OnClose;
			client.OnError += Client_OnError;

			lastStatus = NetworkStatus.Connecting;
			beginConnecting = true;
			failedToConnect = false;


			thread = new Thread( ThreadFunction );
			thread.IsBackground = true;
			thread.Start();


			//config.ReceiveBufferSize = 131071 * 10;
			//config.SendBufferSize = 131071 * 10;
			//config.AutoExpandMTU = true;





			//if( Disposed )
			//	Log.Fatal( "NetworkClient: BeginConnect: The client has been disposed." );
			//if( client != null )
			//	Log.Fatal( "NetworkClient: BeginConnect: The client is already initialized." );


			//disconnectionReason = "";

			//var config = new NetPeerConfiguration( "NeoAxis" );
			//config.ConnectionTimeout = connectionTimeout;

			//config.ReceiveBufferSize = 131071 * 10;
			//config.SendBufferSize = 131071 * 10;
			//config.AutoExpandMTU = true;

			//client = new NetClient( config );

			//client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.ErrorMessage, true );
			//client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.StatusChanged, true );
			//client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.Data, true );
			//client.Configuration.SetMessageTypeEnabled( NetIncomingMessageType.ConnectionLatencyUpdated, true );

			//try
			//{
			//	client.Start();

			//	var message = client.CreateMessage();
			//	message.Write( clientVersion );
			//	message.Write( loginData );

			//	message.WriteVariableUInt32( (uint)Services.Count );
			//	foreach( NetworkService service in Services )
			//		message.Write( service.Name );

			//	client.Connect( host, port, message );
			//}
			//catch( Exception e )
			//{
			//	error = e.Message;
			//	if( e.InnerException != null )
			//		error = e.InnerException.Message;

			//	return false;
			//}

			//beginConnecting = true;

			return true;
#else
			error = "No network implementation for the platform.";
			return false;
#endif
		}

		void ThreadFunction( object param )
		{

			//!!!!non thread variant. client.ConnectAsync();


			if( !beginConnecting || Disposed )
				return;

			try
			{
				client.Connect();
			}
			catch( Exception e )
			{

				//!!!!good to set disconnectReason?
				disconnectionReason = e.Message;
				failedToConnect = true;
				if( lastStatus != NetworkStatus.Disconnected )
				{
					lastStatus = NetworkStatus.Disconnected;
					OnConnectionStatusChanged();
				}
				return;
			}

			beginConnecting = false;
			if( lastStatus != NetworkStatus.Connected )
			{
				lastStatus = NetworkStatus.Connected;
				OnConnectionStatusChanged();
			}

			while( !Disposed && !threadNeedExit )
			{
				while( toProcessMessages.TryDequeue( out var message ) )
				{
					if( message.DataString != null )
					{
						try
						{
							client.Send( message.DataString );
						}
						catch( Exception e )
						{
							client.Close( CloseStatusCode.ProtocolError, "Unable to send the string message. " + e.Message );
						}
					}
					else if( message.DataBinary )
					{
						try
						{
							var data = message.DataBinaryArray;

							unchecked
							{
								Interlocked.Increment( ref dataMessagesSentCounter ); //dataMessagesSentCounter++;
								Interlocked.Add( ref dataSizeSentCounter, data.Length );
								foreach( var b in data )
									dataMessagesSentChecksum += b;
							}

							if( trace )
								Log.Info( $"Send Binary {data.Length} {DataMessagesSentCounter} {dataMessagesSentChecksum}" );

							//!!!!GC inside
							client.Send( data );



							//var segment = message.DataBinarySegment;
							//var stream = new System.IO.MemoryStream( segment.Array, segment.Offset, segment.Count );

							//unchecked
							//{
							//	dataMessagesSentCounter++;
							//	var array = segment.Array;
							//	var from = segment.Offset;
							//	var to = segment.Offset + segment.Count;
							//	for( int n = from; n < to; n++ )
							//		dataMessagesSentChecksum += array[ n ];
							//	//foreach( var b in data )
							//	//	dataMessagesSentChecksum += b;
							//}

							//if( trace )
							//	Log.Info( $"Send Binary {segment.Count} {dataMessagesSentCounter} {dataMessagesSentChecksum}" );

							//client.Send( stream );

							////!!!!from server
							////connection.totalBytesSent += message.DataBinarySegment.Count;



							////var data = message.DataBinarySegment.ToArray();

							////unchecked
							////{
							////	dataMessagesSentCounter++;
							////	foreach( var b in data )
							////		dataMessagesSentChecksum += b;
							////}

							////if( trace )
							////	Log.Info( $"Send Binary {data.Length} {dataMessagesSentCounter} {dataMessagesSentChecksum}" );

							////client.Send( data );
							//////client.Send( message.DataBinarySegment.ToArray() );

							//////!!!!from server
							//////connection.totalBytesSent += message.DataBinarySegment.Count;

						}
						catch( Exception e )
						{
							client.Close( CloseStatusCode.ProtocolError, "Unable to send the binary message. " + e.Message );
						}

						//send checksum
						if( DataMessagesSentCounter % 100 == 0 )
						{
							try
							{
								var rootBlock = new TextBlock();
								rootBlock.SetAttribute( "Command", "Checksum" );
								rootBlock.SetAttribute( "Counter", DataMessagesSentCounter.ToString() );
								rootBlock.SetAttribute( "Checksum", dataMessagesSentChecksum.ToString() );
								var text = rootBlock.DumpToString( false );

								if( trace )
									Log.Info( $"Send Text Checksum {DataMessagesSentCounter} {dataMessagesSentChecksum}" );

								client.Send( text );
							}
							catch( Exception e )
							{
								client.Close( CloseStatusCode.ProtocolError, "Unable to send the checksum command message. " + e.Message );
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

							client.Close( message.CloseStatusCode, reasonClamped );
						}
						catch { }
					}

					if( Disposed || threadNeedExit )
						break;
				}

				Thread.Sleep( 1 );
			}

			if( threadNeedExit && threadNeedExitSendClose )
			{
				try
				{
					client.Close();
				}
				catch { }
			}
		}

		private void Client_OnOpen( object sender, EventArgs e )
		{
			if( trace )
				Log.Info( "Client_OnOpen" );
		}

		private void Client_OnClose( object sender, CloseEventArgs e )
		{
			var reason = e.Reason ?? "";
			receivedMessages.Enqueue( new ReceivedMessage { CloseReason = reason, CloseCode = (CloseStatusCode)e.Code } );

			if( trace )
				Log.Info( "Client_OnClose " + reason + " " + ( (CloseStatusCode)e.Code ).ToString() );
		}

		private void Client_OnError( object sender, ErrorEventArgs e )
		{
			var message = e.Message ?? "";
			receivedMessages.Enqueue( new ReceivedMessage { ErrorMessage = message } );

			if( trace )
				Log.Info( "Client_OnError " + message );

			//!!!!need?
			try
			{
				client.Close( CloseStatusCode.ServerError, message );
			}
			catch { }
		}

		private void Client_OnMessage( object sender, MessageEventArgs e )
		{
			if( e.IsPing )
			{
				if( trace )
					Log.Info( "Client_OnMessage Ping" );
			}
			else if( e.IsText )
			{
				//system commands

				if( trace )
					Log.Info( "Client_OnMessage Text" );

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
						client.Close( CloseStatusCode.ProtocolError, ex.Message );
					}
					catch { }
				}

				//maybe parse part here
				//receivedMessages.Enqueue( new ReceivedMessage { DataString = e.Data } );
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
				}

				if( trace )
					Log.Info( $"Client_OnMessage Binary {data.Length} {DataMessagesReceivedCounter} {dataMessagesReceivedChecksum}" );

				//!!!!without copy?

				receivedMessages.Enqueue( new ReceivedMessage { DataBinary = (byte[])data.Clone() } );
			}
		}

		public virtual void Dispose()
		{
			//exit from the thread
			if( thread != null )
			{
				threadNeedExit = true;
				if( client != null && Status != NetworkStatus.Disconnected )
					threadNeedExitSendClose = true;
				//!!!!how to make without pause?
				Thread.Sleep( 50 );
				thread = null;
			}

			lastStatus = NetworkStatus.Disconnected;
			beginConnecting = false;

			//clear client field. the client must be already closed from the thread
			if( client != null )
			{
				try
				{
					client.Close();
				}
				catch { }
				client = null;
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

			disposed = true;
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

		public string RemoteServerName
		{
			get { return remoteServerName; }
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void OnUpdate()
		{
			if( ProfilerData != null )
			{
				var workedTime = ( DateTime.Now - ProfilerData.TimeStarted ).TotalSeconds;
				if( workedTime >= ProfilerData.WorkingTime )
					ProfilerStop( true );
			}

#if !UWP

			//process received messages
			while( receivedMessages.TryDequeue( out var message ) )
			{
				lastReceivedMessageTime = EngineApp.EngineTime;

				//if( message.DataString != null )
				//{
				//	//system string message

				//	try
				//	{
				//		if( message.DataString.Length > 1000 )
				//			throw new Exception( "The welcome message is more than 1000 characters." );

				//		var rootBlock = TextBlock.Parse( message.DataString, out var error );
				//		if( !string.IsNullOrEmpty( error ) )
				//			throw new Exception( error );

				//		var command = rootBlock.GetAttribute( "Command" );

				//		//throw new Exception( "Unknown command." );

				//	}
				//	catch( Exception ex )
				//	{
				//	}
				//}
				//else 
				if( message.DataBinary != null )
				{
					//data binary message

					var data = message.DataBinary;
					var reader = new ArrayDataReader( data );

					while( reader.CurrentPosition < reader.EndPosition )
					{
						var startPosition = reader.CurrentPosition;
						var length = (int)reader.ReadVariableUInt32();
						ProcessReceivedMessage( data, reader.CurrentPosition, length );
						reader.ReadSkip( length );
						var endPosition = reader.CurrentPosition;

						if( profilerData != null )
						{
							profilerData.TotalReceivedMessages++;
							profilerData.TotalReceivedSize += endPosition - startPosition;
						}

						if( reader.Overflow )
						{
							var reason = "OnMessage: Read overflow.";
							OnReceiveProtocolErrorInternal( reason );
							toProcessMessages.Enqueue( new ToProcessMessage { Close = true, CloseStatusCode = CloseStatusCode.ProtocolError, CloseReason = reason } );

							break;
						}
					}
				}
				else if( message.CloseReason != null )
				{
					//close message

					if( trace )
						Log.Info( "OnUpdate: Close. " + message.CloseReason + " " + message.CloseCode.ToString() );

					//update status

					beginConnecting = false;
					if( string.IsNullOrEmpty( disconnectionReason ) )
						disconnectionReason = message.CloseReason ?? "";

					if( lastStatus != NetworkStatus.Disconnected )
					{
						lastStatus = NetworkStatus.Disconnected;
						OnConnectionStatusChanged();
					}

					if( message.CloseCode == CloseStatusCode.ProtocolError )
						OnReceiveProtocolErrorInternal( disconnectionReason );

					//now disconnected
				}
				else if( message.ErrorMessage != null )
				{
					//error message

					var text = message.ErrorMessage;

					//update status
					beginConnecting = false;
					if( lastStatus != NetworkStatus.Disconnected )
					{
						lastStatus = NetworkStatus.Disconnected;
						OnConnectionStatusChanged();
					}

					OnReceiveProtocolErrorInternal( text );

					//now disconnected
				}
			}

			//update services
			for( int n = 0; n < services.Count; n++ )
				services[ n ].OnUpdate();

			//send accumulated messages
			if( accumulatedMessagesToSend.Length > 0 )
			{
				var array = accumulatedMessagesToSend.ToArray();
				toProcessMessages.Enqueue( new ToProcessMessage { DataBinary = true, DataBinaryArray = array } );
				accumulatedMessagesToSend.Reset();
			}

#endif
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
			if( service.Identifier == 0 )
				Log.Fatal( "ClientNode: RegisterService: Invalid service identifier. Identifier can not be zero." );
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

		//protected internal virtual void OnReceiveProtocolError( string message ) { }

		internal virtual void OnReceiveProtocolErrorInternal( string message )
		{
			if( trace )
				Log.Info( $"OnReceiveProtocolErrorInternal: {message}" );

			if( !Disposed )
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ProcessReceivedMessage( byte[] data, int position, int length )
		{
			var reader = new ArrayDataReader( data, position, length );

			var pair = reader.ReadByte();
			var serviceIdentifier = (byte)( pair >> 4 );
			var messageIdentifier = (byte)( pair & 15 );

			if( reader.Overflow )
			{
				OnReceiveProtocolErrorInternal( "Invalid message." );
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
	}
}
#endif