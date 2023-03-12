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

	public class CloudProjectClientData
	{
		public long ProjectID;
		public bool Edit;
		public string VerificationCode;

		public bool Verified;
		//public long UserID;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class CloudProjectCommon
	{
		static string dataFolder;

		static CloudProjectCommon()
		{
			dataFolder = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "NeoAxis" );
		}

		public static string DataFolder
		{
			get { return dataFolder; }
		}

		public static string DataFolderProjects
		{
			get { return Path.Combine( dataFolder, "Projects" ); }
		}

		public static string DataFolderPackages
		{
			get { return Path.Combine( dataFolder, "Packages" ); }
		}

		public static string GetAppProjectFolder( long projectID, bool edit )
		{
			return Path.Combine( DataFolderProjects, projectID.ToString(), edit ? "Edit" : "Play" );
		}
	}
}
//#endif