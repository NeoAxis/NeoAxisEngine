// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using NeoAxis.Networking;

namespace NeoAxis.Editor
{
	public static class LoginUtility
	{
		const string registryPath = @"HKEY_CURRENT_USER\SOFTWARE\NeoAxis";

		static long requestedFullLicenseInfo_UserID;
		//static volatile string requestedFullLicenseInfo_License = "";
		static volatile string requestedFullLicenseInfo_Error = "";

		//static string licenseCached;

		//

		public static bool GetCurrentLicense( out string email, out string hash )
		{
			try
			{
				email = PlatformSpecificUtility.Instance.GetRegistryValue( registryPath, "LoginEmail", "" ) as string;
				hash = PlatformSpecificUtility.Instance.GetRegistryValue( registryPath, "LoginHash", "" ) as string;
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

		public static void SetCurrentLicense( string email, string hash )
		{
			try
			{
				PlatformSpecificUtility.Instance.SetRegistryValue( registryPath, "LoginEmail", email );
				PlatformSpecificUtility.Instance.SetRegistryValue( registryPath, "LoginHash", EncryptDecrypt( hash ) );
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return;
			}

			RequestInfo();
		}

		//static async Task<CloudServiceExecuteCommand.ResultClass> UserGetAsync( long userID = 0 )
		//{
		//	var command = new CloudServiceExecuteCommand();
		//	command.FunctionName = "api/v1/user/get";
		//	command.RequireUserLogin = true;
		//	//if( userID != 0 )
		//	//	command.AddParameter( "moderator_get_user", userID.ToString(), true );
		//	command.RequestMethod = CloudServiceExecuteCommand.RequestMethodEnum.Post;

		//	var block = new TextBlock();
		//	if( userID != 0 )
		//		block.SetAttribute( "UserID", userID.ToString() );
		//	command.ContentData = Encoding.UTF8.GetBytes( block.DumpToString() );

		//	using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
		//	return await command.ExecuteAsync( cts.Token );
		//}

		async static Task GetLicenseAsync( object param )
		{
			try
			{
				var param2 = (Dictionary<string, string>)param;
				var email = param2[ "Email" ];
				var hash = param2[ "Hash" ];

				//get user
				CloudServiceExecuteCommand.ResultClass getUserResult;
				{
					using var cts = new CancellationTokenSource( new TimeSpan( 0, 1, 0 ) );
					getUserResult = await CloudServiceFunctions.UserGetAsync( cancellationToken: cts.Token );
					//getUserResult = await UserGetAsync();
					if( !string.IsNullOrEmpty( getUserResult.Error ) )
					{
						requestedFullLicenseInfo_UserID = 0; //requestedFullLicenseInfo_License = "";
						requestedFullLicenseInfo_Error = getUserResult.Error;
						//requestedFullLicenseInfo_Error = "Invalid username or password. " + getUserResult.Error;

						//requestedUserID = 0;
						//requestedLicense = "";
						//requestedEmail = "";
						//requestedUsername = "";
						//requestedError = getUserResult.Error;

						return;
					}
				}

				var data = getUserResult.Data;

				if( !data.AttributeExists( "NotFound" ) )
				{
					long.TryParse( data.GetAttribute( "Id" ), out requestedFullLicenseInfo_UserID );

					//requestedFullLicenseInfo_License = "Dummy";
					requestedFullLicenseInfo_Error = "";

					//long.TryParse( data.GetAttribute( "Id" ), out requestedUserID );
					//requestedEmail = data.GetAttribute( "Email" );
					//requestedUsername = data.GetAttribute( "Username" );
					//requestedLicense = "Dummy";
					//requestedError = "";
				}
				else
				{
					requestedFullLicenseInfo_UserID = 0; //requestedFullLicenseInfo_License = "";
					requestedFullLicenseInfo_Error = "Invalid username or password.";

					//requestedUserID = 0;
					//requestedLicense = "";
					//requestedEmail = "";
					//requestedUsername = "";
					//requestedError = "Invalid username or password.";
				}



				//var email64 = StringUtility.EncodeToBase64URL( email );
				//var hash64 = StringUtility.EncodeToBase64URL( hash );

				//string data = $"email={email64}&hash={hash64}";
				//byte[] dataStream = Encoding.UTF8.GetBytes( data );

				//{
				//	WebRequest request = WebRequest.Create( EngineInfo.StoreAddress + @"/api/get_user_info2/" );
				//	request.Method = "POST";
				//	request.ContentType = "application/x-www-form-urlencoded";
				//	request.ContentLength = dataStream.Length;
				//	Stream newStream = request.GetRequestStream();
				//	newStream.Write( dataStream, 0, dataStream.Length );
				//	newStream.Close();

				//	string xml;
				//	using( var response = (HttpWebResponse)request.GetResponse() )
				//	using( var stream = response.GetResponseStream() )
				//	using( var reader = new StreamReader( stream ) )
				//		xml = reader.ReadToEnd();

				//	if( !string.IsNullOrEmpty( xml ) )
				//	{
				//		var xDoc = new XmlDocument();
				//		xDoc.LoadXml( xml );

				//		requestedFullLicenseInfo_Error = "";

				//		if( xDoc.DocumentElement != null )
				//		{
				//			foreach( XmlNode child in xDoc.DocumentElement.ChildNodes )
				//			{
				//				if( child.Name == "license" )
				//					requestedFullLicenseInfo_License = child.InnerText;

				//				if( child.Name == "purchased_products" )
				//				{
				//					var products = child.InnerText.Trim().Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries );
				//					foreach( var product in products )
				//						requestedFullLicenseInfo_PurchasedProducts.AddWithCheckAlreadyContained( product );
				//				}
				//			}
				//		}
				//		//foreach( XmlNode child in xDoc.ChildNodes )
				//		//{
				//		//	if( child.Name == "license" )
				//		//		requestedFullLicenseInfo_License = child.InnerText;
				//		//}
				//	}
				//	else
				//	{
				//		requestedFullLicenseInfo_License = "";
				//		requestedFullLicenseInfo_Error = "Invalid username or password.";
				//	}
				//}


				//licenseCached = null;
			}
			catch//( Exception e )
			{
				//Log.Info( e.Message );
				//Log.Warning( e.Message );
			}
		}

		public static void RequestInfo()
		{
			requestedFullLicenseInfo_UserID = 0; //requestedFullLicenseInfo_License = "";

			if( !GetCurrentLicense( out var email, out var hash ) )
				return;

			var param = new Dictionary<string, string>();
			param[ "Email" ] = email;
			param[ "Hash" ] = hash;

			TaskUtility.Run( TaskUtility.TaskLifetimeEnum.Minutes, "LoginUtility: RequestInfo", async () => await GetLicenseAsync( param ) );
			//Task.Run( async () => await GetLicenseAsync( param ) );

			//var thread1 = new Thread( ThreadGetLicense );
			//thread1.IsBackground = true;
			//thread1.Start( param );
		}

		public static bool GetRequestedInfo( out long userID, out string error )
		{
			if( requestedFullLicenseInfo_UserID != 0 || !string.IsNullOrEmpty( requestedFullLicenseInfo_Error ) )
			{
				userID = requestedFullLicenseInfo_UserID;
				error = requestedFullLicenseInfo_Error;
				return true;
			}
			else
			{
				userID = 0;
				error = "";
				return false;
			}
		}

		//public static bool GetRequestedFullLicenseInfo( out string license, out string error )
		//{
		//	if( !string.IsNullOrEmpty( requestedFullLicenseInfo_License ) || !string.IsNullOrEmpty( requestedFullLicenseInfo_Error ) )
		//	{
		//		license = requestedFullLicenseInfo_License;
		//		error = requestedFullLicenseInfo_Error;
		//		return true;
		//	}
		//	else
		//	{
		//		license = "";
		//		error = "";
		//		return false;
		//	}
		//}

		//public static string GetLicenseCached()
		//{
		//	if( licenseCached == null )
		//	{
		//		licenseCached = "";
		//		if( GetCurrentLicense( out _, out _ ) )
		//		{
		//			if( GetRequestedFullLicenseInfo( out var license, out _, /*out _,*/ out _ ) )
		//				licenseCached = license;
		//		}
		//	}

		//	var result = licenseCached;
		//	if( result == null )
		//		result = "";
		//	return result;
		//}
	}
}
