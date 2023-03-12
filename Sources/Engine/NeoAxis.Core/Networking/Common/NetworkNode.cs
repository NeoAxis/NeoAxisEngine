// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Internal.Lidgren.Network;

namespace NeoAxis.Networking
{
	public abstract class NetworkNode
	{
		internal const int maxServiceIdentifier = 255;

		ArrayDataReader receiveDataReader = new ArrayDataReader();

		bool disposed;

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

					Group group = includeLibraryMessages ? receivedGroupIncludeLibraryMessages :
						receivedGroup;
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
					{
						group = includeLibraryMessages ? receivedGroupIncludeLibraryMessages :
							receivedGroup;
					}
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

			internal NetConnection Connection
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

			public object _GetLidgrenLibraryNetConnection()
			{
				return connection;
			}

			public StatisticsData Statistics
			{
				get { return statistics; }
			}

			internal void SendAccumulatedMessages( NetPeer netPeer )
			{
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
#else
					Log.Fatal( "NetworkNode: SendAccumulatedMessages: impl." );
#endif
				}

				sendCachedDataWriter.Reset();
			}

			internal void AddDataForSending( ArrayDataWriter writer )
			{
				if( connection != null && connection.Status == NetConnectionStatus.Connected )
				{
					//Log.Warning( "AddDataForSending: " + writer.CurrentLength.ToString() );

					sendCachedDataWriter.WriteVariableUInt32( (uint)writer.Length );
					sendCachedDataWriter.Write( writer.Data, 0, writer.Length );
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

		protected NetworkNode()
		{
		}

		internal void NetworkNode_Dispose()
		{
			disposed = true;
		}

		protected virtual void OnUpdate() { }

		public void Update()
		{
			OnUpdate();
		}

		internal void ProcessReceivedMessage( ConnectedNode connectedNode, NetIncomingMessage incomingMessage, int bytePosition, int byteLength )
		{
			receiveDataReader.Init( incomingMessage.PeekDataBuffer(), bytePosition, byteLength );

			//receive service identifier
			var serviceIdentifier = receiveDataReader.ReadByte();
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

			service.ProcessReceivedMessage( connectedNode, receiveDataReader );
		}

		public bool Disposed
		{
			get { return disposed; }
		}

		internal abstract NetworkService GetService( byte identifier );
		internal abstract NetworkService GetService( string name );

		internal abstract void OnReceiveProtocolErrorInternal( ConnectedNode connectedNode, string message );
	}
}