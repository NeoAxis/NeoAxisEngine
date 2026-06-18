// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeoAxis;

namespace NeoAxis.Networking
{
	public static class NetworkCommonSettings
	{
		//common network settings
		public static bool NetworkLogging { get; set; }

		//cloud service settings
		public static CloudServiceFrontServer CloudServiceCurrentFrontServer { get; set; }
		public static int CloudServiceExecuteCommandTimeout = 120000; //2 minutes is default timeout for cloud service commands

		///////////////////////////////////////////////

		public class CloudServiceFrontServer
		{
			public string Domain;
			//public string Address;
			public int HttpsPort;

			public CloudServiceFrontServer( string domain/*, string address*/, int httpsPort )
			{
				Domain = domain;
				//Address = address;
				HttpsPort = httpsPort;
			}
		}

		///////////////////////////////////////////////

		static NetworkCommonSettings()
		{
			//default front server
			CloudServiceCurrentFrontServer = new CloudServiceFrontServer( "cloud.neoaxis.com", 443 );
		}
	}
}