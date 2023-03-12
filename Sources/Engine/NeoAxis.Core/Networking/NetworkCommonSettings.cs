// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis;

namespace NeoAxis.Networking
{
	public static class NetworkCommonSettings
	{
//#if CLOUD
		//!!!!
		public const string GeneralManagerURL = @"http://localhost:44317";
		public const string GeneralManagerAddress = "localhost";
		public const int GeneralManagerPort = 56566;

		public const int ProjectManagerTimeout = 10000;
		public const int ProjectCommitPort = 56568;
		public const int ProjectEnteringPort = 56569;
//#endif

		public static bool NetworkLogging { get; set; }
	}
}