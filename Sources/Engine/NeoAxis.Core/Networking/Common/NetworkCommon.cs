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
}
