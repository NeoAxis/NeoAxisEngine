// Copyright 2006–2026 Ivan Efimov. All rights reserved.

//IMPORTANT: Modifying or bypassing this part of the code is prohibited by the NeoAxis Engine Standard EULA.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis.Cloud;

namespace NeoAxis.Editor
{
	public static class LoginUtility
	{
		const string registryPath = @"HKEY_CURRENT_USER\SOFTWARE\NeoAxis";

		volatile static ReceivedInfoClass receivedInfo = new ReceivedInfoClass();

		//static long requestedFullLicenseInfo_UserID;
		//static string requestedFullLicenseInfo_EntityType = "";
		//static volatile string requestedFullLicenseInfo_Error = "";
		////static volatile string requestedFullLicenseInfo_License = "";

		volatile static int getLicenseAsyncCounter;

		///////////////////////////////////////////////

		public class ReceivedInfoClass
		{
			public long UserID;
			public string EntityType = "";
			public string Error = "";
		}

		///////////////////////////////////////////////

		public static bool GetCurrentLicense( out string email, out string hash )
		{
			try
			{
				email = PlatformSpecificRegistryUtility.GetRegistryValue( registryPath, "LoginEmail", "" ) as string;
				hash = PlatformSpecificRegistryUtility.GetRegistryValue( registryPath, "LoginHash", "" ) as string;
				if( !string.IsNullOrEmpty( hash ) )
					hash = EncryptDecrypt( hash );

				if( !string.IsNullOrEmpty( email ) && !string.IsNullOrEmpty( hash ) )
					return true;
			}
			catch { }

			email = "";
			hash = "";
			return false;
		}

		internal static string EncryptDecrypt( string input )
		{
			char[] key = { 'K', 'C', 'Q' }; //Any chars will work, in an array of any size
			char[] output = new char[ input.Length ];

			for( int i = 0; i < input.Length; i++ )
				output[ i ] = (char)( input[ i ] ^ key[ i % key.Length ] );

			return new string( output );
		}

		public static void SetCurrentLicense( string email, string hash, string hardwareFingerprintId )
		{
			try
			{
				PlatformSpecificRegistryUtility.SetRegistryValue( registryPath, "LoginEmail", email );
				PlatformSpecificRegistryUtility.SetRegistryValue( registryPath, "LoginHash", EncryptDecrypt( hash ) );
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return;
			}

			RequestInfo( hardwareFingerprintId );
		}

		async static Task GetLicenseAsync( object param )
		{
			try
			{
				var param2 = (Dictionary<string, string>)param;
				var email = param2[ "Email" ];
				var hash = param2[ "Hash" ];
				int.TryParse( param2[ "getLicenseAsyncCounter" ], out var getLicenseAsyncCounter2 );
				var hardwareFingerprintId = param2[ "HardwareFingerprintId" ];

				again:

				//get user
				CloudServiceExecuteCommand.ResultClass getUserResult;
				{
					using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					getUserResult = await CloudServiceFunctions.UserGetAsync( hardwareFingerprintId, cancellationToken: cts.Token );
					if( !string.IsNullOrEmpty( getUserResult.Error ) )
					{
						if( getLicenseAsyncCounter2 == getLicenseAsyncCounter )
						{
							receivedInfo = new ReceivedInfoClass() { Error = getUserResult.Error };
							//requestedFullLicenseInfo_UserID = 0; //requestedFullLicenseInfo_License = "";
							//requestedFullLicenseInfo_EntityType = "";
							//requestedFullLicenseInfo_Error = getUserResult.Error;

							//pause for 1 minute before next attempt
							await Task.Delay( 1000 * 60 );
							goto again;
						}

						return;
					}
				}

				var data = getUserResult.Data;

				if( !data.AttributeExists( "NotFound" ) )
				{
					long.TryParse( data.GetAttribute( "Id" ), out var userID );

					receivedInfo = new ReceivedInfoClass()
					{
						UserID = userID,
						EntityType = data.GetAttribute( "EntityType" )
					};

					//long.TryParse( data.GetAttribute( "Id" ), out requestedFullLicenseInfo_UserID );
					//requestedFullLicenseInfo_EntityType = data.GetAttribute( "EntityType" );

					////requestedFullLicenseInfo_License = "Dummy";
					//requestedFullLicenseInfo_Error = "";
				}
				else
				{
					receivedInfo = new ReceivedInfoClass() { Error = "Invalid username or password." };

					//requestedFullLicenseInfo_UserID = 0; //requestedFullLicenseInfo_License = "";
					//requestedFullLicenseInfo_EntityType = "";
					//requestedFullLicenseInfo_Error = "Invalid username or password.";
				}
			}
			catch { }
		}

		public static void RequestInfo( string hardwareFingerprintId )
		{
			receivedInfo = new ReceivedInfoClass();
			//requestedFullLicenseInfo_UserID = 0; //requestedFullLicenseInfo_License = "";
			//requestedFullLicenseInfo_EntityType = "";

			if( !GetCurrentLicense( out var email, out var hash ) )
				return;

			getLicenseAsyncCounter++;

			var param = new Dictionary<string, string>();
			param[ "Email" ] = email;
			param[ "Hash" ] = hash;
			param[ "getLicenseAsyncCounter" ] = getLicenseAsyncCounter.ToString();
			param[ "HardwareFingerprintId" ] = hardwareFingerprintId;

			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "LoginUtility: RequestInfo", async () => await GetLicenseAsync( param ) );
		}

		public static ReceivedInfoClass ReceivedInfo
		{
			get { return receivedInfo; }
		}

		//!!!!delete?
		public static bool GetRequestedInfo( out long userID, out string error )
		{
			var info = receivedInfo;
			if( info.UserID != 0 || !string.IsNullOrEmpty( info.Error ) )
			{
				userID = info.UserID;
				error = info.Error;
				return true;
			}
			else
			{
				userID = 0;
				error = "";
				return false;
			}

			//if( requestedFullLicenseInfo_UserID != 0 || !string.IsNullOrEmpty( requestedFullLicenseInfo_Error ) )
			//{
			//	userID = requestedFullLicenseInfo_UserID;
			//	error = requestedFullLicenseInfo_Error;
			//	return true;
			//}
			//else
			//{
			//	userID = 0;
			//	error = "";
			//	return false;
			//}
		}
	}
}
