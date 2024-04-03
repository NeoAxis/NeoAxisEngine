#if UWP
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Internal.Lidgren.Network
{
	public enum NetConnectionStatus
	{
		None,
		InitiatedConnect,
		RespondedConnect,
		Connected,
		Disconnecting,
		Disconnected,
	}

	public class NetConnection
	{
		public IPEndPoint RemoteEndPoint
		{
			get { return null; }
		}

		public NetConnectionStatus Status
		{
			get { return NetConnectionStatus.None; }
		}

		public object Tag { get; set; }

		public void Disconnect( string reason )
		{
		}

		public class StatisticsClass
		{
			public long ReceivedBytes
			{
				get { return 0; }
			}

			public long SentBytes
			{
				get { return 0; }
			}
		}

		public StatisticsClass Statistics
		{
			get { return null; }
		}
	}

	class NetPeer
	{
		public void Disconnect( string reason )
		{
		}

		public void Shutdown( string reason )
		{
		}
	}

	class NetClient : NetPeer
	{
	}

	class NetServer : NetPeer
	{
	}

	class NetTime
	{
		public static double Now
		{
			get { return 0; }
		}
	}

	class NetIncomingMessage
	{
		public byte[] PeekDataBuffer()
		{
			return null;
		}
	}
}
#endif