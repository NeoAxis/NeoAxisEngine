// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using Internal.Lidgren.Network;

namespace NeoAxis.Networking
{
	public abstract class NetworkNode
	{
		internal const int maxServiceIdentifier = 255;

		ArrayDataReader receiveDataReader = new ArrayDataReader();

		bool disposed;

		//profiler
		ProfilerDataClass profilerData;

		///////////////////////////////////////////

		public sealed class ConnectedNode
		{
			NetworkNode owner;
			NetConnection connection;
			internal NetworkStatus status = NetworkStatus.Disconnected;
			StatisticsData statistics;
			string loginData;

			DateTime creationTime;
			double maxLifetime;

			//specific
			public long LoginDataUserID { get; set; }
			public string LoginDataUsername { get; set; }
			public object Tag { get; set; }

			ArrayDataWriter sendCachedDataWriter = new ArrayDataWriter();

			List<string> remoteServices;
			ReadOnlyCollection<string> remoteServicesAsReadOnly;

			internal float lastRoundtripTime;

			///////////////

			public sealed class StatisticsData
			{
				ConnectedNode owner;

				Group sentGroup = new Group();
				Group receivedGroup = new Group();
				Group sentGroupIncludeLibraryMessages = new Group();
				Group receivedGroupIncludeLibraryMessages = new Group();

				//

				class Group
				{
					public long bytesTotal;

					public float bytesPerSecond;
					public double bytesPerSecondUpdateTime;
					public long bytesPerSecondUpdateCount;
				}

				//

				internal StatisticsData( ConnectedNode owner )
				{
					this.owner = owner;
				}

				public long GetBytesReceived( bool includeLibraryMessages )
				{
					if( !includeLibraryMessages )
						Log.Fatal( "ConnectedNode: StatisticsData: GetBytesReceived: \"includeLibraryMessages = false\" is not supported." );

					Group group = includeLibraryMessages ? receivedGroupIncludeLibraryMessages : receivedGroup;
					NetConnection connection = owner.connection;
					if( connection != null )
						group.bytesTotal = connection.Statistics.ReceivedBytes;// GetBytesReceived( includeLibraryMessages );
					return group.bytesTotal;
				}

				public long GetBytesSent( bool includeLibraryMessages )
				{
					if( !includeLibraryMessages )
						Log.Fatal( "ConnectedNode: StatisticsData: GetBytesSent: \"includeLibraryMessages = false\" is not supported." );

					Group group = includeLibraryMessages ? sentGroupIncludeLibraryMessages : sentGroup;
					NetConnection connection = owner.connection;
					if( connection != null )
						group.bytesTotal = connection.Statistics.SentBytes;// GetBytesSent( includeLibraryMessages );
					return group.bytesTotal;
				}

				float GetBytesPerSecond( bool receive, bool includeLibraryMessages )
				{
					if( owner.connection == null )
						return 0;

					Group group;
					if( receive )
						group = includeLibraryMessages ? receivedGroupIncludeLibraryMessages : receivedGroup;
					else
						group = includeLibraryMessages ? sentGroupIncludeLibraryMessages : sentGroup;

					double now = NetTime.Now;

					double diff = now - group.bytesPerSecondUpdateTime;
					if( diff > .2f )
					{
						long bytesTotal = receive ? GetBytesReceived( includeLibraryMessages ) : GetBytesSent( includeLibraryMessages );

						if( diff > 5 )
							group.bytesPerSecondUpdateTime = 0;

						if( group.bytesPerSecondUpdateTime != 0 )
						{
							long bytesDiff = bytesTotal - group.bytesPerSecondUpdateCount;
							group.bytesPerSecond = (float)( (double)bytesDiff / diff );
						}

						group.bytesPerSecondUpdateTime = now;
						group.bytesPerSecondUpdateCount = bytesTotal;
					}

					return group.bytesPerSecond;
				}

				public float GetBytesReceivedPerSecond( bool includeLibraryMessages )
				{
					return GetBytesPerSecond( true, includeLibraryMessages );
				}

				public float GetBytesSentPerSecond( bool includeLibraryMessages )
				{
					return GetBytesPerSecond( false, includeLibraryMessages );
				}
			}

			///////////////

			internal ConnectedNode( NetworkNode owner, NetConnection connection, string loginData )
			{
				this.owner = owner;
				this.connection = connection;
				this.loginData = loginData;
				creationTime = DateTime.Now;

				statistics = new StatisticsData( this );
			}

			public NetworkNode Owner
			{
				get { return owner; }
			}

			public IPEndPoint RemoteEndPoint
			{
				get
				{
					if( connection == null )
						return null;
					return connection.RemoteEndPoint;
				}
			}

			public /*internal*/ NetConnection Connection
			{
				get { return connection; }
			}

			internal void Clear()
			{
				owner = null;
				connection = null;
				status = NetworkStatus.Disconnected;
				loginData = "";
			}

			public NetworkStatus Status
			{
				get { return status; }
			}

			public string LoginData
			{
				get { return loginData; }
			}

			public DateTime CreationTime
			{
				get { return creationTime; }
			}

			public double MaxLifetime
			{
				get { return maxLifetime; }
				set { maxLifetime = value; }
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			internal NetworkStatus GetRealStatus()
			{
				if( connection != null )
				{
					switch( connection.Status )
					{
					case NetConnectionStatus.None:
					case NetConnectionStatus.InitiatedConnect:
					case NetConnectionStatus.RespondedConnect:
						return NetworkStatus.Connecting;

					case NetConnectionStatus.Connected:
						return NetworkStatus.Connected;

					case NetConnectionStatus.Disconnecting:
					case NetConnectionStatus.Disconnected:
						return NetworkStatus.Disconnected;
					}
				}
				return NetworkStatus.Disconnected;
			}

			//public object _GetLidgrenLibraryNetConnection()
			//{
			//	return connection;
			//}

			public StatisticsData Statistics
			{
				get { return statistics; }
			}

			[MethodImpl( (MethodImplOptions)512 )]
			internal void SendAccumulatedMessages( NetPeer netPeer )
			{
				//!!!!maybe can send same message to all clients with broadcasting

				if( sendCachedDataWriter.Length == 0 )
					return;

				if( connection != null && connection.Status == NetConnectionStatus.Connected )
				{
					int byteLength = sendCachedDataWriter.Length;

					//Log.Warning( "SendFlush: " + byteLength.ToString() );

#if !UWP
					var message = netPeer.CreateMessage( byteLength );
					message.Write( sendCachedDataWriter.Data, 0, byteLength );
					connection.SendMessage( message, NetDeliveryMethod.ReliableOrdered, 1 );

					//!!!!new, but maybe no sense
					connection.Peer?.FlushSendQueue();

#else
					Log.Fatal( "NetworkNode: SendAccumulatedMessages: impl." );
#endif
				}

				sendCachedDataWriter.Reset();
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			internal void AddDataForSending( ArrayDataWriter writer )
			{
				if( connection != null && connection.Status == NetConnectionStatus.Connected )
				{
					var bytesWritten = sendCachedDataWriter.WriteVariableUInt32( (uint)writer.Length );
					sendCachedDataWriter.Write( writer.Data, 0, writer.Length );

					if( owner.ProfilerData != null )
					{
						//owner.ProfilerData.TotalSentMessages++;
						owner.ProfilerData.TotalSentSize += bytesWritten + writer.Length;
					}
				}
			}

			public float LastRoundtripTime
			{
				get
				{
					if( connection == null )
						return 0;
					return lastRoundtripTime;
				}
			}

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
				string text;
				if( RemoteEndPoint != null )
					text = RemoteEndPoint.ToString();
				else
					text = "Unknown address";
				return text;
			}
		}

		///////////////////////////////////////////

		public class ProfilerDataClass
		{
			public DateTime TimeStarted;
			public float WorkingTime;

			//public long TotalReceivedMessages;
			public long TotalReceivedSize;
			//public long TotalSentMessages;
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

		///////////////////////////////////////////

		protected NetworkNode()
		{
		}

		internal void NetworkNode_Dispose()
		{
			disposed = true;
		}

		protected virtual void OnUpdate()
		{
			if( ProfilerData != null )
			{
				var workedTime = DateTime.Now - ProfilerData.TimeStarted;
				if( workedTime.TotalSeconds >= ProfilerData.WorkingTime )
					ProfilerStop( true );
			}
		}

		public void Update()
		{
			OnUpdate();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal void ProcessReceivedMessage( ConnectedNode connectedNode, NetIncomingMessage incomingMessage, int bytePosition, int byteLength )
		{
			receiveDataReader.Init( incomingMessage.PeekDataBuffer(), bytePosition, byteLength );

			var pair = receiveDataReader.ReadByte();
			var serviceIdentifier = (byte)( pair >> 4 );
			var messageIdentifier = (byte)( pair & 15 );

			//receive service identifier
			//var serviceIdentifier = receiveDataReader.ReadByte();
			if( receiveDataReader.Overflow )
			{
				OnReceiveProtocolErrorInternal( connectedNode, "Invalid message." );
				return;
			}

			//service message
			var service = GetService( serviceIdentifier );
			if( service == null )
			{
				//no such service
				return;
			}

			service.ProcessReceivedMessage( connectedNode, receiveDataReader, byteLength, messageIdentifier );
		}

		public bool Disposed
		{
			get { return disposed; }
		}

		internal abstract NetworkService GetService( int identifier );
		internal abstract NetworkService GetService( string name );

		internal abstract void OnReceiveProtocolErrorInternal( ConnectedNode connectedNode, string message );

		public ProfilerDataClass ProfilerData
		{
			get { return profilerData; }
		}

		public void ProfilerStart( float workingTime )
		{
			ProfilerStop( false );
			profilerData = new ProfilerDataClass();
			profilerData.TimeStarted = DateTime.Now;
			profilerData.WorkingTime = workingTime;

			Log.Info( "Network profiler started." );
		}

		public void ProfilerStop( bool writeToLogs )
		{
			if( profilerData == null )
				return;
			Log.Info( "Network profiler stopped." );
			if( writeToLogs )
				DumpProfilerDataToLogs();
			profilerData = null;
		}

		static string FormatCount( long count )
		{
			return count.ToString( "N0" );
		}

		static string FormatSize( long byteCount )
		{
			//copyright: from LiteDB
			var suf = new[] { "B", "KB", "MB", "GB", "TB" }; //Longs run out around EB
			if( byteCount == 0 ) return "0 " + suf[ 0 ];
			var bytes = Math.Abs( byteCount );
			var place = Convert.ToInt64( Math.Floor( Math.Log( bytes, 1024 ) ) );
			var num = Math.Round( bytes / Math.Pow( 1024, place ), 1 );
			return ( Math.Sign( byteCount ) * num ).ToString() + " " + suf[ place ];
		}

		void DumpProfilerDataToLogs()
		{
			var lines = new List<string>();

			lines.Add( "--------------------------------------------------------------" );
			lines.Add( string.Format( "Total received; {0}", FormatSize( profilerData.TotalReceivedSize ) ) );
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

						lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.ReceivedMessages ), FormatSize( messageByTypeItem.ReceivedSize ) ) );

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
								lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), FormatSize( item.Item2.Size ) ) );
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
			lines.Add( string.Format( "Total sent; Size: {0}", FormatSize( profilerData.TotalSentSize ) ) );
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

						lines.Add( string.Format( "> > {0}; Messages: {1}; Size: {2}", messageType.Name, FormatCount( messageByTypeItem.SentMessages ), FormatSize( messageByTypeItem.SentSize ) ) );

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
								lines.Add( string.Format( "> > > {0}; Messages: {1}; Size: {2}", item.Item1, FormatCount( item.Item2.Messages ), FormatSize( item.Item2.Size ) ) );
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
	}
}