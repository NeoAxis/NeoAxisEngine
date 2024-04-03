// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Reflection;
using System.Drawing;

namespace NeoAxis
{
	/// <summary>
	/// Provides general information about the engine.
	/// </summary>
	public static class EngineInfo
	{
		public static string Version
		{
			get
			{
				Version v = Assembly.GetExecutingAssembly().GetName().Version;

				string ret = "";
				ret += v.Major.ToString();
				ret += ".";
				ret += v.Minor.ToString();
				if( v.Build != 0 )
					ret += "." + v.Build.ToString();
				if( v.Revision != 0 )
					ret += "." + v.Revision.ToString();
				return ret;
			}
		}

		public static string WWW
		{
			get { return "www.neoaxis.com"; }
		}

		public static string Copyright
		{
			get { return "Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica."; }
		}

		public static bool SpecialAppMode { get; set; } = false;
		public static string OriginalName { get; set; } = "NeoAxis Engine";

		public static string NameWithoutVersion
		{
			get
			{
				if( !string.IsNullOrEmpty( ReplaceNameWithoutVersion ) )
					return ReplaceNameWithoutVersion;

				return OriginalName;

				//var result = "";
				////if( ModifiedVersionOfNeoAxisEngine )
				////	result += "Modified version of ";
				//result += OriginalName;
				//return result;
			}
		}

		public static string NameWithVersion
		{
			get
			{
				if( !string.IsNullOrEmpty( ReplaceNameWithVersion ) )
					return ReplaceNameWithVersion;

				Version v = Assembly.GetExecutingAssembly().GetName().Version;
				var result = NameWithoutVersion + $" {v.Major}.{v.Minor}";
				//if( OriginalName == "NeoAxis Engine" )//if( !SpecialAppMode )
				//	result += $" ({WWW})";

				return result;
			}
		}

		//public static Bitmap GetSplashLogoImage( ProjectSettingsPage_General.EngineSplashScreenStyleEnum style )
		//{
		//	if( style == ProjectSettingsPage_General.EngineSplashScreenStyleEnum.WhiteBackground )
		//		return Properties.Resources.PoweredBy_WhiteBackground;
		//	else
		//		return Properties.Resources.PoweredBy_BlackBackground;
		//}

		public static string ReplaceNameWithoutVersion { get; set; } = "";
		public static string ReplaceNameWithVersion { get; set; } = "";

		//public static bool ExtendedEdition;

		public static string StoreAddress
		{
			get { return "https://store.neoaxis.com"; }
		}

		/////////////////////////////////////////

		public enum EngineModeEnum
		{
			Standalone,
			CloudServer,
			CloudClient,
		}

		/////////////////////////////////////////

		public class CloudProjectInfoClass
		{
			public long ID { get; }
			public string Name { get; }

			internal CloudProjectInfoClass( long id, string name )
			{
				ID = id;
				Name = name;
			}
		}

		/////////////////////////////////////////

		static EngineModeEnum engineMode;
		static CloudProjectInfoClass cloudProjectInfo;

		public static void SetEngineMode( EngineModeEnum engineMode, CloudProjectInfoClass cloudProjectInfo )
		{
			EngineInfo.engineMode = engineMode;
			EngineInfo.cloudProjectInfo = cloudProjectInfo;
		}

		public static CloudProjectInfoClass CloudProjectInfo
		{
			get { return cloudProjectInfo; }
		}

		public static EngineModeEnum EngineMode
		{
			get { return engineMode; }
		}
	}
}
