// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoAxis.Editor;
using System.Net.NetworkInformation;
//!!!!
#if !DEPLOY
using System.Management;
#endif

namespace NeoAxis
{
	static class LoginUtility
	{
		const string registryPath = @"SOFTWARE\NeoAxis";

		static volatile string requestedFullLicenseInfo_License = "";
		static volatile string requestedFullLicenseInfo_TokenTransactions = "";
		static volatile string requestedFullLicenseInfo_Error = "";
		//static double licenseInfoLastUpdateTime;
		//static bool licenseInfoPro;

		static string machineId;

		//

		public static bool GetCurrentLicense( out string email, out string hash )
		{
#if !DEPLOY
			try
			{
				//opening the subkey  
				RegistryKey key = Registry.CurrentUser.OpenSubKey( registryPath );

				//if it does exist, retrieve the stored values  
				if( key != null )
				{
					email = ( key.GetValue( "LoginEmail" ) ?? "" ).ToString();
					var p = key.GetValue( "LoginHash" );
					if( p != null )
						hash = EncryptDecrypt( p.ToString() );
					else
						hash = "";
					//hash = ( key.GetValue( "LoginHash" ) ?? "" ).ToString();
					key.Close();
					return true;
				}
			}
			catch { }
#endif

			email = "";
			hash = "";
			return false;
		}

		static string EncryptDecrypt( string input )
		{
			char[] key = { 'K', 'C', 'Q' }; //Any chars will work, in an array of any size
			char[] output = new char[ input.Length ];

			for( int i = 0; i < input.Length; i++ )
				output[ i ] = (char)( input[ i ] ^ key[ i % key.Length ] );

			return new string( output );
		}

		public static void SetCurrentLicense( string email, string password )
		{
#if !DEPLOY
			try
			{
				var key = Registry.CurrentUser.CreateSubKey( registryPath );

				key.SetValue( "LoginEmail", email );
				key.SetValue( "LoginHash", EncryptDecrypt( password ) );
				key.Close();
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return;
			}

			RequestFullLicenseInfo();
#endif
		}

		static void ThreadGetLicense( object param )
		{
			try
			{
				var param2 = (Dictionary<string, string>)param;
				var email = param2[ "Email" ];
				var hash = param2[ "Hash" ];

				var email64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( email.ToLower() ) ).Replace( "=", "" );
				var hash64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( hash ) ).Replace( "=", "" );

				string data = $"email={email64}&hash={hash64}";
				byte[] dataStream = Encoding.UTF8.GetBytes( data );

				{
					WebRequest request = WebRequest.Create( @"https://www.neoaxis.com/api/get_license" );
					request.Method = "POST";
					request.ContentType = "application/x-www-form-urlencoded";
					request.ContentLength = dataStream.Length;
					Stream newStream = request.GetRequestStream();
					newStream.Write( dataStream, 0, dataStream.Length );
					newStream.Close();

					string text;
					using( var response = (HttpWebResponse)request.GetResponse() )
					using( var stream = response.GetResponseStream() )
					using( var reader = new StreamReader( stream ) )
						text = reader.ReadToEnd();

					requestedFullLicenseInfo_Error = "";
					if( text.Contains( "Pro" ) )
						requestedFullLicenseInfo_License = "Pro";
					else if( text.Contains( "Personal" ) )
						requestedFullLicenseInfo_License = "Personal";
					else if( !string.IsNullOrEmpty( text ) )
						requestedFullLicenseInfo_License = text;
					else
					{
						requestedFullLicenseInfo_License = "";
						requestedFullLicenseInfo_Error = "Invalid username or password.";
					}
				}

				{
					WebRequest request = WebRequest.Create( @"https://www.neoaxis.com/api/get_token_transactions" );
					request.Method = "POST";
					request.ContentType = "application/x-www-form-urlencoded";
					request.ContentLength = dataStream.Length;
					Stream newStream = request.GetRequestStream();
					newStream.Write( dataStream, 0, dataStream.Length );
					newStream.Close();

					string text;
					using( var response = (HttpWebResponse)request.GetResponse() )
					using( var stream = response.GetResponseStream() )
					using( var reader = new StreamReader( stream ) )
						text = reader.ReadToEnd();

					requestedFullLicenseInfo_TokenTransactions = text;
				}

				//SaveLicenseCertificate2();
				EngineApp.needReadLicenseCertificate = true;
			}
			catch { }
		}

		public static void RequestFullLicenseInfo()
		{
			requestedFullLicenseInfo_License = "";
			requestedFullLicenseInfo_TokenTransactions = "";

			if( !GetCurrentLicense( out var email, out var hash ) )
				return;

			var param = new Dictionary<string, string>();
			param[ "Email" ] = email;
			param[ "Hash" ] = hash;

			var thread1 = new Thread( ThreadGetLicense );
			thread1.Start( param );
		}

		public static bool GetRequestedFullLicenseInfo( out string license, out string tokenTransactions, out string error )
		{
			if( !string.IsNullOrEmpty( requestedFullLicenseInfo_License ) || !string.IsNullOrEmpty( requestedFullLicenseInfo_Error ) )
			{
				license = requestedFullLicenseInfo_License;
				tokenTransactions = requestedFullLicenseInfo_TokenTransactions;
				error = requestedFullLicenseInfo_Error;
				//pro = requestedFullLicenseInfo_License.Contains( "Pro" );
				return true;
			}
			else
			{
				license = "";
				tokenTransactions = "";
				error = "";
				//pro = false;
				return false;
			}
		}

		public static bool SaveLicenseCertificate( string realFileName, string email, string engineVersion, string license, string machineId, DateTime expirationDate, out string error )
		{
			//!!!!на сервере генерировать

			var email2 = email.ToLower();

			//!!!!не так. GetHashCode может поменяться
			//CRC32
			var verification = "Check";
			//var verification = email2.GetHashCode() ^ engineVersion.GetHashCode() ^ license.GetHashCode() ^ expirationDate.GetHashCode();

			string str = "NeoAxis Certificate version 1";
			str += "\n" + email2;
			str += "\n" + engineVersion;
			str += "\n" + license;
			str += "\n" + machineId;
			str += "\n" + expirationDate.ToString( "MM/dd/yyyy" );
			str += "\n" + verification;

			var base64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( str ) );

			try
			{
				File.WriteAllText( realFileName, base64 );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			error = "";
			return true;
		}

		public static string GetMachineId()
		{
#if !DEPLOY
			if( string.IsNullOrEmpty( machineId ) )
			{
				try
				{
					var mc = new ManagementClass( "win32_processor" );
					foreach( ManagementObject mo in mc.GetInstances() )
					{
						var cpuInfo = mo.Properties[ "processorID" ].Value.ToString();
						machineId = cpuInfo;
						break;
					}
				}
				catch { }
			}
#endif

			return machineId;
		}

		//static PhysicalAddress GetMacAddress2()
		//{
		//	foreach( NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces() )
		//	{
		//		// Only consider Ethernet network interfaces
		//		if( nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet && nic.OperationalStatus == OperationalStatus.Up )
		//			return nic.GetPhysicalAddress();
		//	}
		//	return null;
		//}

		//public static string GetMacAddress()
		//{
		//	try
		//	{
		//		return GetMacAddress2().ToString();
		//	}
		//	catch
		//	{
		//		return "";
		//	}
		//}

		//static bool SaveLicenseCertificate2()
		//{
		//	var fileName = Path.Combine( VirtualFileSystem.Directories.Project, "License.cert" );

		//	if( File.Exists( fileName ) )
		//		File.Delete( fileName );

		//	if( !GetCurrentLicense( out var email, out _ ) )
		//		return false;
		//	if( !GetRequestedFullLicenseInfo( out var license, out var error2 ) )
		//		return false;

		//	var date = DateTime.UtcNow;
		//	//add 5 days
		//	date = date.Add( new TimeSpan( 5, 0, 0, 0, 0 ) );

		//	if( !SaveLicenseCertificate( fileName, email, EngineInfo.Version, license, GetMachineId(), date, out var error ) )
		//	{
		//		EditorMessageBox.ShowWarning( "Unable to save 'License.cert'. " + error );
		//		return false;
		//	}

		//	return true;
		//}

		//public static bool ReadLicenseCertificate( string realFileName, out string error, out string email, out string engineVersion, out string license, out string machineId, out DateTime expirationDate )
		//{
		//	error = "";

		//	try
		//	{
		//		var base64 = File.ReadAllText( realFileName );
		//		var text = Encoding.UTF8.GetString( Convert.FromBase64String( base64 ) );
		//		var lines = text.Split( new char[] { '\n' }, StringSplitOptions.None );

		//		var format = lines[ 0 ];

		//		email = lines[ 1 ];
		//		engineVersion = lines[ 2 ];
		//		license = lines[ 3 ];
		//		machineId = lines[ 4 ];
		//		expirationDate = DateTime.ParseExact( lines[ 5 ], "MM/dd/yyyy", null );

		//		var verification = lines[ 6 ];

		//		//!!!!verification
		//		//!!!!!error

		//		return true;
		//	}
		//	catch( Exception e )
		//	{
		//		error = e.Message;
		//		email = "";
		//		expirationDate = new DateTime( 0 );
		//		engineVersion = "";
		//		license = "";
		//		machineId = "";
		//		return false;
		//	}
		//}

	}
}
