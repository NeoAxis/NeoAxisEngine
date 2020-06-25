using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.IO;
using System.Threading.Tasks;
//using Windows.Management.Deployment;
using System.Threading;
//using Windows.Foundation;
using System.Security.Cryptography.X509Certificates;

namespace NeoAxis
{
	class CertificateHelper
	{
		internal static X509Certificate2 Load( string path )
		{
			X509Certificate2 cert = new X509Certificate2();
			//Create X509Certificate2 object from .cer .pfx file.
			byte[] rawData = ReadFile( path );
			cert.Import( rawData );

			////Print to console information contained in the certificate.
			//Log.Info( "{0}Subject: {1}{0}", Environment.NewLine, cert.Subject );
			//Log.Info( "{0}Issuer: {1}{0}", Environment.NewLine, cert.Issuer );
			//Log.Info( "{0}Version: {1}{0}", Environment.NewLine, cert.Version );
			//Log.Info( "{0}Valid Date: {1}{0}", Environment.NewLine, cert.NotBefore );
			//Log.Info( "{0}Expiry Date: {1}{0}", Environment.NewLine, cert.NotAfter );
			//Log.Info( "{0}Thumbprint: {1}{0}", Environment.NewLine, cert.Thumbprint );
			//Log.Info( "{0}Serial Number: {1}{0}", Environment.NewLine, cert.SerialNumber );
			//Log.Info( "{0}Friendly Name: {1}{0}", Environment.NewLine, cert.PublicKey.Oid.FriendlyName );
			//Log.Info( "{0}Public Key Format: {1}{0}", Environment.NewLine, cert.PublicKey.EncodedKeyValue.Format( true ) );
			//Log.Info( "{0}Raw Data Length: {1}{0}", Environment.NewLine, cert.RawData.Length );
			//Log.Info( "{0}Certificate to string: {1}{0}", Environment.NewLine, cert.ToString( true ) );

			//Log.Info( "{0}Certificate to XML String: {1}{0}", Environment.NewLine, cert.PublicKey.Key.ToXmlString( false ) );

			return cert;
		}

		//TODO: implement this.
		internal static X509Certificate2 Create( string path, string password )
		{
			X509Certificate2 cert = new X509Certificate2( path, password );
			return cert;
		}

		internal static void AddToStore( X509Certificate2 certificate )
		{
			//Add the certificate to a X509Store.
			X509Store store = new X509Store();
			store.Open( OpenFlags.MaxAllowed );
			store.Add( certificate );
			store.Close();
		}

		private static byte[] ReadFile( string fileName )
		{
			FileStream f = new FileStream( fileName, FileMode.Open, FileAccess.Read );
			int size = (int)f.Length;
			byte[] data = new byte[size];
			size = f.Read( data, 0, size );
			f.Close();
			return data;
		}
	}

	internal static class UWPPackageHelper
	{
		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4 )]
		private class PACKAGE_ID
		{
			public uint reserved;
			public uint processorArchitecture;
			public ulong version;
			public string name;
			public string publisher;
			public string resourceId;
			public string publisherId;
		}

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true )]
		private static extern uint PackageFamilyNameFromId( PACKAGE_ID packageId, ref uint packageFamilyNameLength, StringBuilder packageFamilyName );


		internal static string GetPackageFamilyName( string packageName, string publisherId )
		{
			string packageFamilyName = null;
			PACKAGE_ID packageId = new PACKAGE_ID
			{
				name = packageName,
				publisher = publisherId
			};
			uint packageFamilyNameLength = 0;
			//First get the length of the Package Name -> Pass NULL as Output Buffer
			if( PackageFamilyNameFromId( packageId, ref packageFamilyNameLength, null ) == 122 ) //ERROR_INSUFFICIENT_BUFFER
			{
				StringBuilder packageFamilyNameBuilder = new StringBuilder( (int)packageFamilyNameLength );
				if( PackageFamilyNameFromId( packageId, ref packageFamilyNameLength, packageFamilyNameBuilder ) == 0 )
					packageFamilyName = packageFamilyNameBuilder.ToString();
			}
			return packageFamilyName;
		}

		//internal static void RemovePackage( string packageFullName, out bool cancel )
		//{
		//	cancel = false;

		//	PackageManager packageManager = new PackageManager();

		//	var deploymentOperation = packageManager.RemovePackageAsync( packageFullName );

		//	// this event will be signaled when the deployment operation has completed.
		//	ManualResetEvent opCompletedEvent = new ManualResetEvent( false );

		//	deploymentOperation.Completed = ( depProgress, status ) => { opCompletedEvent.Set(); };

		//	Console.WriteLine( "Removing package {0}", packageFullName );

		//	Console.WriteLine( "Waiting for removal to complete..." );

		//	opCompletedEvent.WaitOne();

		//	if( deploymentOperation.Status == AsyncStatus.Error )
		//	{
		//		DeploymentResult deploymentResult = deploymentOperation.GetResults();
		//		Log.Info( "Removal Error: {0}", deploymentOperation.ErrorCode );
		//		Log.Info( "Detailed Error Text: {0}", deploymentResult.ErrorText );

		//		throw new Exception( deploymentResult.ErrorText );
		//	}
		//	else if( deploymentOperation.Status == AsyncStatus.Canceled )
		//	{
		//		Log.Info( "Removal Canceled" );
		//		cancel = true;
		//	}
		//	else if( deploymentOperation.Status == AsyncStatus.Completed )
		//	{
		//		Log.Info( "Removal succeeded!" );
		//	}
		//	else
		//	{
		//		Log.Info( "Removal status unknown" );
		//	}
		//}

		//internal static void RegisterPackage( string manifestPath, out bool cancel )
		//{
		//	cancel = false;

		//	PackageManager packageManager = new PackageManager();

		//	var deploymentOperation = packageManager.RegisterPackageAsync( new Uri( manifestPath ),
		//		null, DeploymentOptions.DevelopmentMode );

		//	// this event will be signaled when the deployment operation has completed.
		//	ManualResetEvent opCompletedEvent = new ManualResetEvent( false );

		//	deploymentOperation.Completed = ( depProgress, status ) => { opCompletedEvent.Set(); };

		//	Log.Info( "Registering package {0}", manifestPath );

		//	Log.Info( "Waiting for registration to complete..." );

		//	opCompletedEvent.WaitOne();

		//	if( deploymentOperation.Status == AsyncStatus.Error )
		//	{
		//		DeploymentResult deploymentResult = deploymentOperation.GetResults();
		//		Log.Info( "Registration Error: {0}", deploymentOperation.ErrorCode );
		//		Log.Info( "Detailed Error Text: {0}", deploymentResult.ErrorText );

		//		throw new Exception( deploymentResult.ErrorText );
		//	}
		//	else if( deploymentOperation.Status == AsyncStatus.Canceled )
		//	{
		//		Log.Info( "Registration Canceled" );
		//		cancel = true;
		//	}
		//	else if( deploymentOperation.Status == AsyncStatus.Completed )
		//	{
		//		Log.Info( "Registration succeeded!" );
		//	}
		//	else
		//	{
		//		Log.Info( "Registration status unknown" );
		//	}
		//}
	}
}
