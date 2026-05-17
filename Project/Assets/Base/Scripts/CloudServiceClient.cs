// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis;
using NeoAxis.Networking;

namespace Project
{
	/// <summary>
	/// The class for general management of the cloud client.
	/// </summary>
	public static class CloudServiceClient
	{
		static CloudFunctionsClient client;
		static long thisUserID;

		///////////////////////////////////////////////

		public class SimpleResult
		{
			public string Error;
		}

		///////////////////////////////////////////////

		public static CloudFunctionsClient Client
		{
			get { return client; }
		}

		public static long ThisUserID
		{
			get
			{
				if( thisUserID == 0 )
					thisUserID = client?.ConnectionNode?.Users?.ThisUser?.UserID ?? 0L;
				return thisUserID;
			}
		}

		public async static Task<SimpleResult> ConnectAsync( long projectID = 0L, bool allowWebSocket = true, bool allowUDP = false, CancellationToken cancellationToken = default )
		{
			//disconnect previous client
			Destroy();

			//connection settings
			var settings = BasicServiceClient.ConnectionSettingsClass.CreateCloud( CloudUserRole.Player, projectID, true );
			settings.ConnectWebSocket = allowWebSocket;
			settings.ConnectUDP = allowUDP;

			//create client and connect
			var createResult = await CloudFunctionsClient.CreateAsync( settings, true, cancellationToken );
			if( !string.IsNullOrEmpty( createResult.Error ) )
				return new SimpleResult() { Error = createResult.Error };

			client = createResult.Client;

			return new SimpleResult();
		}

		public static void Destroy()
		{
			client?.Destroy();
			client = null;
			thisUserID = 0;
		}
	}
}