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

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class NetworkUtility
	{
		public static Metadata.GetMembersContext metadataGetMembersContextNoFilter = new Metadata.GetMembersContext( false );

		public static string FormatSize( long byteCount )
		{
			//copyright: from LiteDB
			var suf = new[] { "B", "KB", "MB", "GB", "TB" }; //Longs run out around EB
			if( byteCount == 0 ) return "0 " + suf[ 0 ];
			var bytes = Math.Abs( byteCount );
			var place = Convert.ToInt64( Math.Floor( Math.Log( bytes, 1024 ) ) );
			var num = Math.Round( bytes / Math.Pow( 1024, place ), 1 );
			return ( Math.Sign( byteCount ) * num ).ToString() + " " + suf[ place ];
		}
	}
}
