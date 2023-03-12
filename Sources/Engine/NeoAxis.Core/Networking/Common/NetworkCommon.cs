// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public enum NetworkStatus
	{
		Disconnected,
		Connecting,
		Connected,
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	[AttributeUsage( /*AttributeTargets.Class |*/ AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false )]
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
}
