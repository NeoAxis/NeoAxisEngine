// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis;

namespace NeoAxis.Networking
{
	public static class NetworkCommonSettings
	{
		static string cloudServiceAddress;
		//public const string CloudServiceFolder = "https://neoaxis.us-east-1.linodeobjects.com/";

		internal const int GeneralManagerHttpPort = 44317;
		public const int GeneralManagerClientPort = 56566;
		//public const string GeneralManagerURL = @"http://localhost:44317";
		//public const string GeneralManagerAddress = "localhost";
		//public const int GeneralManagerPort = 56566;

		public const int ProjectManagerTimeout = 10000;
		public const int ProjectCommitPort = 56568;
		public const int ProjectEnteringPort = 56569;

		//!!!!
		public const int P2PDefaultSeederPort = 56571;

		public static bool NetworkLogging { get; set; }

		///////////////////////////////////////////////

		public static string CloudServiceAddress
		{
			get
			{
				if( cloudServiceAddress == null )
				{
					//to change address add to NeoAxis.DefaultSettings.config:
					//CloudServiceAddress = "https://neoaxis.us-east-1.linodeobjects.com/"

					var result = VirtualFileSystem.DefaultSettingsConfig?.GetAttribute( "CloudServiceAddress" );
					if( string.IsNullOrEmpty( result ) )
						result = "https://neoaxis.us-east-1.linodeobjects.com/";

					cloudServiceAddress = result;
				}
				return cloudServiceAddress;
			}
		}
	}
}