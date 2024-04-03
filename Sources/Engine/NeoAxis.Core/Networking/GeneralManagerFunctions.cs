// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//#if !DEPLOY
//using Microsoft.Win32;
//#endif

namespace NeoAxis.Networking
{
	public static class GeneralManagerFunctions
	{
		static bool? cloudServiceDeveloperLocalhostModeCached;

		//

		static bool GetCloudServiceDeveloperLocalhostMode()
		{
			try
			{
				int result = (int)PlatformSpecificUtility.Instance.GetRegistryValue( "HKEY_CURRENT_USER\\SOFTWARE\\NeoAxis", "CloudServiceDeveloperLocalhostMode", 0 );

				if( result == 1 )
					return true;
			}
			catch { }

			//const string registryPath = @"SOFTWARE\NeoAxis";

			//try
			//{
			//	using( var key = Registry.CurrentUser.OpenSubKey( registryPath ) )
			//	{
			//		if( key != null )
			//		{
			//			var v = key.GetValue( "CloudServiceDeveloperLocalhostMode" );
			//			if( v == null )
			//				return false;

			//			if( v.ToString() == "1" )
			//				return true;
			//		}
			//	}
			//}
			//catch { }

			return false;
		}

		public static bool IsCloudServiceDeveloperLocalhostMode()
		{
			if( !cloudServiceDeveloperLocalhostModeCached.HasValue )
				cloudServiceDeveloperLocalhostModeCached = GetCloudServiceDeveloperLocalhostMode();
			return cloudServiceDeveloperLocalhostModeCached.Value;
		}

		public static string RequestGeneralManagerAddress( out string error )
		{
			error = "";

			if( IsCloudServiceDeveloperLocalhostMode() )
				return "localhost";
			else
			{
				try
				{
					using( var client = new WebClient() )
					{
						var data = client.DownloadData( NetworkCommonSettings.CloudServiceAddress + "Launcher.txt" );
						//var data = client.DownloadData( "https://neoaxis.s3.amazonaws.com/Launcher.txt" );

						if( data != null && data.Length != 0 )
						{
							var str = Encoding.Default.GetString( data );

							var block = TextBlock.Parse( str, out error );
							if( block == null )
								return "";

							if( block != null )
							{
								return block.GetAttribute( "GeneralManagerAddress" );
							}
						}
					}
				}
				catch( Exception e )
				{
					error = e.Message;
				}

				return "";
			}
		}

		public static async Task<string> RequestGeneralManagerAddressAsync()
		{
			if( IsCloudServiceDeveloperLocalhostMode() )
				return "localhost";
			else
			{
				try
				{
					using( var client = new WebClient() )
					{
						var data = await client.DownloadDataTaskAsync( NetworkCommonSettings.CloudServiceAddress + "Launcher.txt" );
						//var data = await client.DownloadDataTaskAsync( "https://neoaxis.s3.amazonaws.com/Launcher.txt" );

						if( data != null && data.Length != 0 )
						{
							var str = Encoding.Default.GetString( data );

							var block = TextBlock.Parse( str, out var error );
							if( block != null )
							{
								return block.GetAttribute( "GeneralManagerAddress" );
							}
						}
					}
				}
				catch { }

				return "";
			}
		}

		public static async Task<GeneralManagerExecuteCommand.ResultClass> RequestVerificationCodeToEnterProjectAsync( long projectID, string purpose )
		{
			var command = new GeneralManagerExecuteCommand();
			command.FunctionName = "api/request_verification_code_to_enter_project";
			command.RequireUserLogin = true;
			command.Parameters.Add( ("project", projectID.ToString()) );
			command.Parameters.Add( ("purpose", purpose) );

			return await command.ExecuteAsync();
		}

		public static string GetHttpURL( string generalManagerAddress )
		{
			//!!!!
			var prefix = generalManagerAddress.Contains( "localhost" ) ? "http://" : "http://";
			//var prefix = generalManagerAddress.Contains( "localhost" ) ? "http://" : "https://";

			var port = NetworkCommonSettings.GeneralManagerHttpPort;
			return $"{prefix}{generalManagerAddress}:{port}";
		}
	}
}