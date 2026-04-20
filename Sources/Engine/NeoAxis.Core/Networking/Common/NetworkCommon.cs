// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Threading;

namespace NeoAxis
{
	public enum NetworkStatus
	{
		Disconnected,
		Connecting,
		Connected,
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class NetworkAggregateConnectionStatistics
	{
		long sent;
		long received;

		public void AddSent( long value )
		{
			Interlocked.Add( ref sent, value );
		}

		public void AddReceived( long value )
		{
			Interlocked.Add( ref received, value );
		}

		public long GetSent( bool reset = false )
		{
			if( reset )
				return Interlocked.Exchange( ref sent, 0 );
			else
				return Interlocked.Read( ref sent );
		}

		public long GetReceived( bool reset = false )
		{
			if( reset )
				return Interlocked.Exchange( ref received, 0 );
			else
				return Interlocked.Read( ref received );
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false )]
	public class NetworkSynchronizeAttribute : Attribute
	{
		bool networkMode;

		public NetworkSynchronizeAttribute( bool networkMode )
		{
			this.networkMode = networkMode;
		}

		public bool NetworkMode
		{
			get { return networkMode; }
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class NetworkUtilityInternal
	{
		public static Metadata.GetMembersContext metadataGetMembersContextNoFilter = new Metadata.GetMembersContext( false );
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	namespace Networking
	{
		//copy of WebSocketCloseStatus
		/// <summary>
		/// Represents well known WebSocket close codes as defined in section 11.7 of the WebSocket protocol spec.
		/// </summary>
		public enum ConnectionCloseStatus
		{
			/// <summary>
			/// (1000) The connection has closed after the request was fulfilled.
			/// </summary>
			NormalClosure = 1000,

			/// <summary>
			/// (1001) Indicates an endpoint is being removed. Either the server or client will become unavailable.
			/// </summary>
			EndpointUnavailable = 1001,

			/// <summary>
			/// (1002) The client or server is terminating the connection because of a protocol error.
			/// </summary>
			ProtocolError = 1002,

			/// <summary>
			/// (1003) The client or server is terminating the connection because it cannot accept the data type it received.
			/// </summary>
			InvalidMessageType = 1003,

			/// <summary>
			/// No error specified.
			/// </summary>
			Empty = 1005,

			/// <summary>
			/// (1007) The client or server is terminating the connection because it has received data inconsistent with the message type.
			/// </summary>
			InvalidPayloadData = 1007,

			/// <summary>
			/// (1008) The connection will be closed because an endpoint has received a message that violates its policy.
			/// </summary>
			PolicyViolation = 1008,

			/// <summary>
			/// (1009) The connection will be closed because the message is too big.
			/// </summary>
			MessageTooBig = 1009,

			/// <summary>
			/// (1010) The client is terminating the connection because it expected the server to negotiate an extension.
			/// </summary>
			MandatoryExtension = 1010,

			/// <summary>
			/// (1011) The connection will be closed by the server because of an error on the server.
			/// </summary>
			InternalServerError = 1011
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	namespace Networking
	{
		//same as WebSocketMessageType
		/// <summary>
		/// Indicates the message type.
		/// </summary>
		public enum ConnectionMessageType
		{
			/// <summary>
			/// The message is clear text.
			/// </summary>
			Text = 0,

			/// <summary>
			/// The message is in binary format.
			/// </summary>
			Binary = 1,

			/// <summary>
			/// A receive has completed because a close message was received.
			/// </summary>
			Close = 2
		}
	}
}
