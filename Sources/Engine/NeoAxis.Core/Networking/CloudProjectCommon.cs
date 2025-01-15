// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//#if CLOUD
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeoAxis.Networking
{
	public enum RepositorySyncMode
	{
		Synchronize,
		ServerOnly,
		//StorageOnly
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class CloudProjectProjectSettingsCache
	{
		public ESet<string> SkipPaths = new ESet<string>();
		public ESet<string> SkipFoldersWithName = new ESet<string>();
		public ESet<string> SkipFilesWithExtensionWhenPlay = new ESet<string>();
		public ESet<string> ClearFilesWithExtensionWhenPlay = new ESet<string>();

		//

		public CloudProjectProjectSettingsCache( string cloudProjectFolder )
		{

			//!!!!disabled
			var applyNeoAxisEngineSkipPaths = false;


			if( applyNeoAxisEngineSkipPaths )
			{
				var projectSettingsFullPath = Path.Combine( cloudProjectFolder, @"Project\Assets\Base\ProjectSettings.component" );

				var data = ProjectSettings.ReadParametersDirectlyByRealPath( projectSettingsFullPath, "Repository",
					new string[] {
						"SkipPaths",
						"SkipFoldersWithName",
						"SkipFilesWithExtensionWhenPlay",
						"ClearFilesWithExtensionWhenPlay" },
					new string[] {
						ProjectSettingsPage_Repository.SkipPathsDefault,
						ProjectSettingsPage_Repository.SkipFoldersWithNameDefault,
						ProjectSettingsPage_Repository.SkipFilesWithExtensionWhenPlayDefault,
						ProjectSettingsPage_Repository.ClearFilesWithExtensionWhenPlayDefault } );

				SkipPaths.AddRangeWithCheckAlreadyContained( data[ 0 ].Split( new char[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries ) );
				SkipFoldersWithName.AddRangeWithCheckAlreadyContained( data[ 1 ].Split( new char[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries ) );
				SkipFilesWithExtensionWhenPlay.AddRangeWithCheckAlreadyContained( data[ 2 ].Split( new char[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries ) );
				ClearFilesWithExtensionWhenPlay.AddRangeWithCheckAlreadyContained( data[ 3 ].Split( new char[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries ) );
			}

			////var skipPaths = ProjectSettings.ReadParameterDirectlyByRealPath( projectSettingsFullPath, "Repository", "SkipPaths", ProjectSettingsPage_Repository.SkipPathsDefault );
			////SkipPaths = new List<string>();
			////foreach( var path in skipPaths.Split( new char[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries ) )
			////	SkipPaths.Add( path );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class CloudProjectClientData
	{
		public long ProjectID;
		public bool Edit;
		public string VerificationCode;
		public string CloudProjectFolder;

		public bool Verified;
		//public long UserID;

		//cached project settings
		CloudProjectProjectSettingsCache cloudProjectProjectSettingsCache;

		//

		public CloudProjectProjectSettingsCache GetCloudProjectProjectSettingsCache()
		{
			if( cloudProjectProjectSettingsCache == null )
				cloudProjectProjectSettingsCache = new CloudProjectProjectSettingsCache( CloudProjectFolder );
			return cloudProjectProjectSettingsCache;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public static class CloudProjectCommon
	//{
	//	static string dataFolder;

	//	static CloudProjectCommon()
	//	{
	//		dataFolder = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "NeoAxis" );
	//	}

	//	public static string DataFolder
	//	{
	//		get { return dataFolder; }
	//	}

	//	public static string DataFolderProjects
	//	{
	//		get { return Path.Combine( dataFolder, "Projects" ); }
	//	}

	//	public static string DataFolderPackages
	//	{
	//		get { return Path.Combine( dataFolder, "Packages" ); }
	//	}

	//	public static string GetAppProjectFolder( long projectID )
	//	{
	//		return Path.Combine( DataFolderProjects, projectID.ToString() );
	//	}

	//	public static string GetAppProjectFolder( long projectID, bool edit )
	//	{
	//		return Path.Combine( DataFolderProjects, projectID.ToString(), edit ? "Edit" : "Play" );
	//	}
	//}
}
//#endif