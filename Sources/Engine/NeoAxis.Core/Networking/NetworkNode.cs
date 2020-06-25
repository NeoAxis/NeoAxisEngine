//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Text;
//using System.Net;

//using Lidgren.Network;

//namespace NeoAxis
//{
//	public abstract class NetworkNode
//	{
//		internal const int maxServiceIdentifier =
//			( 1 << NetworkDefines.bitsForServiceIdentifier ) - 1;

//		//

//		//receive data
//		ArrayDataReader receiveDataReader = new ArrayDataReader();

//		bool disposed;

//		///////////////////////////////////////////

//		public sealed class ConnectedNode
//		{
//			NetworkNode owner;
//			NetConnection connection;
//			internal NetworkConnectionStatuses status = NetworkConnectionStatuses.Disconnected;
//			StatisticsData statistics;
//			string loginName;

//			ArrayDataWriter sendCachedDataWriter = new ArrayDataWriter();

//			List<string> remoteServices;
//			ReadOnlyCollection<string> remoteServicesAsReadOnly;

//			internal float lastRoundtripTime;

//			///////////////

//			public sealed class StatisticsData
//			{
//				ConnectedNode owner;

//				Group sentGroup = new Group();
//				Group receivedGroup = new Group();
//				Group sentGroupIncludeLibraryMessages = new Group();
//				Group receivedGroupIncludeLibraryMessages = new Group();

//				//

//				class Group
//				{
//					public long bytesTotal;

//					public float bytesPerSecond;
//					public double bytesPerSecondUpdateTime;
//					public long bytesPerSecondUpdateCount;
//				}

//				//

//				internal StatisticsData( ConnectedNode owner )
//				{
//					this.owner = owner;
//				}

//				public long GetBytesReceived( bool includeLibraryMessages )
//				{
//					if( !includeLibraryMessages )
//						Log.Fatal( "ConnectedNode: StatisticsData: GetBytesReceived: \"includeLibraryMessages = false\" is not supported." );

//					Group group = includeLibraryMessages ? receivedGroupIncludeLibraryMessages :
//						receivedGroup;
//					NetConnection connection = owner.connection;
//					if( connection != null )
//						group.bytesTotal = connection.Statistics.ReceivedBytes;// GetBytesReceived( includeLibraryMessages );
//					return group.bytesTotal;
//				}

//				public long GetBytesSent( bool includeLibraryMessages )
//				{
//					if( !includeLibraryMessages )
//						Log.Fatal( "ConnectedNode: StatisticsData: GetBytesSent: \"includeLibraryMessages = false\" is not supported." );

//					Group group = includeLibraryMessages ? sentGroupIncludeLibraryMessages : sentGroup;
//					NetConnection connection = owner.connection;
//					if( connection != null )
//						group.bytesTotal = connection.Statistics.SentBytes;// GetBytesSent( includeLibraryMessages );
//					return group.bytesTotal;
//				}

//				float GetBytesPerSecond( bool receive, bool includeLibraryMessages )
//				{
//					if( owner.connection == null )
//						return 0;

//					Group group;
//					if( receive )
//					{
//						group = includeLibraryMessages ? receivedGroupIncludeLibraryMessages :
//							receivedGroup;
//					}
//					else
//						group = includeLibraryMessages ? sentGroupIncludeLibraryMessages : sentGroup;

//					double now = NetTime.Now;

//					double diff = now - group.bytesPerSecondUpdateTime;
//					if( diff > .2f )
//					{
//						long bytesTotal = receive ? GetBytesReceived( includeLibraryMessages ) :
//							GetBytesSent( includeLibraryMessages );

//						if( diff > 5 )
//							group.bytesPerSecondUpdateTime = 0;

//						if( group.bytesPerSecondUpdateTime != 0 )
//						{
//							long bytesDiff = bytesTotal - group.bytesPerSecondUpdateCount;
//							group.bytesPerSecond = (float)( (double)bytesDiff / diff );
//						}

//						group.bytesPerSecondUpdateTime = now;
//						group.bytesPerSecondUpdateCount = bytesTotal;
//					}

//					return group.bytesPerSecond;
//				}

//				public float GetBytesReceivedPerSecond( bool includeLibraryMessages )
//				{
//					return GetBytesPerSecond( true, includeLibraryMessages );
//				}

//				public float GetBytesSentPerSecond( bool includeLibraryMessages )
//				{
//					return GetBytesPerSecond( false, includeLibraryMessages );
//				}
//			}

//			///////////////

//			internal ConnectedNode( NetworkNode owner, NetConnection connection, string loginName )
//			{
//				this.owner = owner;
//				this.connection = connection;
//				this.loginName = loginName;

//				statistics = new StatisticsData( this );
//			}

//			public NetworkNode Owner
//			{
//				get { return owner; }
//			}

//			public IPEndPoint RemoteEndPoint
//			{
//				get
//				{
//					if( connection == null )
//						return null;
//					return connection.RemoteEndpoint;
//				}
//			}

//			internal NetConnection Connection
//			{
//				get { return connection; }
//			}

//			internal void Clear()
//			{
//				owner = null;
//				connection = null;
//				status = NetworkConnectionStatuses.Disconnected;
//				loginName = "";
//			}

//			public NetworkConnectionStatuses Status
//			{
//				get { return status; }
//			}

//			public string LoginName
//			{
//				get { return loginName; }
//			}

//			internal NetworkConnectionStatuses GetRealStatus()
//			{
//				if( connection != null )
//				{
//					switch( connection.Status )
//					{
//					case NetConnectionStatus.None:
//					case NetConnectionStatus.InitiatedConnect:
//					case NetConnectionStatus.RespondedConnect:
//						return NetworkConnectionStatuses.Connecting;

//					case NetConnectionStatus.Connected:
//						return NetworkConnectionStatuses.Connected;

//					case NetConnectionStatus.Disconnecting:
//					case NetConnectionStatus.Disconnected:
//						return NetworkConnectionStatuses.Disconnected;
//					}
//				}
//				return NetworkConnectionStatuses.Disconnected;
//			}

//			public object _GetLidgrenLibraryNetConnection()
//			{
//				return connection;
//			}

//			public StatisticsData Statistics
//			{
//				get { return statistics; }
//			}

//			internal void SendFlush( NetPeer netPeer )
//			{
//				if( sendCachedDataWriter.BitLength == 0 )
//					return;

//				if( connection != null && connection.Status == NetConnectionStatus.Connected )
//				{
//					int byteLength = sendCachedDataWriter.GetByteLength();
//					if( byteLength * 8 != sendCachedDataWriter.BitLength )
//					{
//						Log.Fatal( "NetworkNode: ConnectedNode: SendFlush: byteLength * 8 != " +
//							"sendCachedDataWriter.BitLength." );
//					}

//					NetOutgoingMessage message = netPeer.CreateMessage( byteLength );
//					message.Write( sendCachedDataWriter.Data, 0, byteLength );
//					connection.SendMessage( message, NetDeliveryMethod.ReliableOrdered, 1 );
//				}

//				sendCachedDataWriter.Reset();
//			}

//			internal void AddDataForSending( ArrayDataWriter writer )
//			{
//				if( connection != null && connection.Status == NetConnectionStatus.Connected )
//				{
//					sendCachedDataWriter.WriteVariableUInt32( (uint)writer.BitLength );
//					sendCachedDataWriter.Write( writer.Data, 0, writer.GetByteLength() );
//				}
//			}

//			public float LastRoundtripTime
//			{
//				get
//				{
//					if( connection == null )
//						return 0;
//					return lastRoundtripTime;
//				}
//			}

//			internal void SetRemoteServices( List<string> remoteServices )
//			{
//				this.remoteServices = remoteServices;
//				remoteServicesAsReadOnly = new ReadOnlyCollection<string>( remoteServices );
//			}

//			public IList<string> RemoteServices
//			{
//				get { return remoteServicesAsReadOnly; }
//			}
//		}

//		///////////////////////////////////////////

//		protected NetworkNode()
//		{
//		}

//		internal void NetworkNode_Dispose()
//		{
//			disposed = true;
//		}

//		protected virtual void OnUpdate() { }

//		public void Update()
//		{
//			OnUpdate();
//		}

//		internal void ProcessReceivedMessage( ConnectedNode connectedNode, NetIncomingMessage incomingMessage,
//			int bitPosition, int bitLength )
//		{
//			receiveDataReader.Init( incomingMessage.PeekDataBuffer(), bitPosition, bitLength );

//			//receive service identifier
//			byte serviceIdentifier = receiveDataReader.ReadByte(
//				NetworkDefines.bitsForServiceIdentifier );
//			if( receiveDataReader.Overflow )
//			{
//				OnReceiveProtocolErrorInternal( connectedNode, "Invalid message" );
//				return;
//			}

//			//service message
//			NetworkService service = GetService( serviceIdentifier );
//			if( service == null )
//			{
//				//no such service
//				return;
//			}

//			service.ProcessReceivedMessage( connectedNode, receiveDataReader );
//		}

//		public bool Disposed
//		{
//			get { return disposed; }
//		}

//		internal abstract NetworkService GetService( byte identifier );
//		internal abstract NetworkService GetService( string name );

//		internal abstract void OnReceiveProtocolErrorInternal( ConnectedNode sender, string message );
//	}
//}
