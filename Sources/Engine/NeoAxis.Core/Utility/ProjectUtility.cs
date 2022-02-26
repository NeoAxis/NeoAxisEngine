// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.IO;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class to work with the NeoAxis project.
	/// </summary>
	public static class ProjectUtility
	{
		public static void GetDefaultProjectPaths( out string projectDirectory, out string userSettingsDirectory )
		{
			projectDirectory = "";
			userSettingsDirectory = "";

			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
			{
				var dir = Path.GetDirectoryName( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) );
				//var dir = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), "Project" );

				if( Directory.Exists( dir ) )
				{
					projectDirectory = dir;
					//projectDirectory = Path.Combine( dir, "Data" );
					userSettingsDirectory = Path.Combine( dir, "User settings" );
				}
				else
				{
					Log.Fatal( $"Project directory is not exists \'{dir}\'." );
				}

				//var d = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), "Projects" );
				//if( Directory.Exists( d ) )
				//{
				//	string[] dirs = Directory.GetDirectories( d );

				//	//!!!!!
				//	if( dirs.Length != 1 )
				//		Log.Fatal( "impl: dirs.Length != 1." );

				//	string dir = dirs[ 0 ];
				//	dataDirectory = Path.Combine( dir, "Data" );
				//	userSettingsDirectory = Path.Combine( dir, "User settings" );
				//}
				//else
				//{
				//	//!!!!!
				//	Log.Fatal( "impl: no Projects directory." );
				//}
			}
			else if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
			{
				Log.Fatal( "ProjectUtility: GetDefaultProjectPaths: Must be never called." );
				//#if W__!!__INDOWS_UWP
				//				var appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

				//				//!!!!
				//				projectDirectory = appFolder;
				//				//if( Directory.Exists( appFolder ) )
				//				//	dataDirectory = Path.Combine( appFolder, "Data" );
				//				//else
				//				//	Log.Fatal( "ProjectUtils: GetDefaultProjectPaths: App folder not found: " + appFolder );

				//				var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
				//				if( Directory.Exists( localFolder ) )
				//					userSettingsDirectory = Path.Combine( localFolder, "User settings" );
				//				else
				//					Log.Fatal( "ProjectUtility: GetDefaultProjectPaths: Data folder not found: " + localFolder );
				//#endif
			}
			else
			{
				Log.Fatal( "ProjectUtility: GetDefaultProjectPaths: No implementation." );
			}
		}
	}
}

/*

namespace NeoAxis
{
	public static class ProjectUtils2
	{
		public static void GetDefaultProjectPaths( out string dataDirectory, out string userSettingsDirectory )
		{
			dataDirectory = GetDataFolder();
			userSettingsDirectory = GetSettingsFolder();
		}

		static string GetDataFolder()
		{
#if W___!!___INDOWS_UWP
			var appFolder = GetInstalledAppFolder();
#else
			var appFolder = GetLocalAppDataFolder();
#endif

			if( Directory.Exists( appFolder ) )
				return Path.Combine( appFolder, "Data" );
			else
				Log.Fatal( "App folder not found: " + appFolder );

			return "";
		}

		static string GetSettingsFolder()
		{
			var dataFolder = GetLocalAppDataFolder();
			if( Directory.Exists( dataFolder ) )
				return Path.Combine( dataFolder, "User settings" );
			else
				Log.Fatal( "Data folder not found: " + dataFolder );

			return "";
		}


		static string GetLocalAppDataFolder()
		{
#if W___!!___INDOWS_UWP
			Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
			return localFolder.Path;
#else
			var folder = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
			return Path.Combine( folder, "Project" );
#endif
		}

		static string GetInstalledAppFolder()
		{
#if W___!!___INDOWS_UWP
			var installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
			return installedLocation.Path;
#else
			return Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
#endif
		}
	}
}
*/
