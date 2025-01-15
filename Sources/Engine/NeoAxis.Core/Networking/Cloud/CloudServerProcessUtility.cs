// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NeoAxis;

namespace NeoAxis.Networking
{
	public static class CloudServerProcessUtility
	{
		static bool initialized;

		////////////////////////////////////////////////

		public static class CommandLineParameters
		{
			internal static string serverAddress = string.Empty;
			internal static int serverPort;
			internal static string serverPasswordHash;
			internal static long projectID;
			internal static string projectFolder;
			internal static string processSettings = string.Empty;
			internal static TextBlock processSettingsTextBlock;

			//

			public static string ServerAddress
			{
				get
				{
					Init();
					return serverAddress;
				}
			}

			public static int ServerPort
			{
				get
				{
					Init();
					return serverPort;
				}
			}

			public static string ServerPasswordHash
			{
				get
				{
					Init();
					return serverPasswordHash;
				}
			}

			public static long ProjectID
			{
				get
				{
					Init();
					return projectID;
				}
			}

			public static string ProjectFolder
			{
				get
				{
					Init();
					return projectFolder;
				}
			}

			public static string ProcessSettings
			{
				get
				{
					Init();
					return processSettings;
				}
			}

			public static TextBlock ProcessSettingsTextBlock
			{
				get
				{
					if( processSettingsTextBlock == null )
					{
						processSettingsTextBlock = TextBlock.Parse( ProcessSettings, out _ );
						if( processSettingsTextBlock == null )
							processSettingsTextBlock = new TextBlock();
					}
					return processSettingsTextBlock;
				}
			}
		}

		////////////////////////////////////////////////

		static void Init()
		{
			if( !initialized )
			{
				//get serverAddress
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-serverAddress", out CommandLineParameters.serverAddress ) )
				{
					//Logs.Write( "Common", "Init: '-serverAddress' is not specified." );
					//return false;
				}

				//get serverPort
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-serverPort", out var serverPortString ) )
				{
					//Logs.Write( "Common", "Init: '-serverPort' is not specified." );
					//return false;
				}
				if( !int.TryParse( serverPortString, out CommandLineParameters.serverPort ) )
				{
					//Logs.Write( "Common", "Init: '-serverPort' invalid data." );
					//return false;
				}

				//get serverPasswordHash
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-serverPasswordHash", out CommandLineParameters.serverPasswordHash ) )
				{
					//Logs.Write( "Common", "Init: '-serverPasswordHash' is not specified." );
					//return false;
				}

				//get projectID
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-projectID", out var projectIDString ) )
				{
					//Logs.Write( "Common", "Init: '-projectID' is not specified." );
					//return false;
				}
				if( !long.TryParse( projectIDString, out CommandLineParameters.projectID ) )
				{
					//Logs.Write( "Common", "Init: '-projectID' invalid data." );
					//return false;
				}

				//get projectFolder
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-projectFolder", out CommandLineParameters.projectFolder ) )
				{
					//Logs.Write( "Common", "Init: '-projectFolder' is not specified." );
					//return false;
				}

				//get processSettings
				if( SystemSettings.CommandLineParameters.TryGetValue( "-processSettings", out var processSettings ) )
				{
					try
					{
						CommandLineParameters.processSettings = Encoding.UTF8.GetString( Convert.FromBase64String( processSettings ) );
					}
					catch { }
				}

				initialized = true;
			}
		}

		public static string MessageToServerManagerTxtFullPath
		{
			get
			{
				return Path.Combine( CommandLineParameters.ProjectFolder, "Data", "MessageToServerManager.txt" );
			}
		}
	}
}