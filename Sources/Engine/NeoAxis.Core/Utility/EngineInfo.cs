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

		static bool ModifiedVersionOfNeoAxisEngine = false;


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

		public static string OriginalName
		{
			get { return "NeoAxis Engine"; }
		}

		public static string NameWithoutVersion
		{
			get
			{
				var result = "";
				if( ModifiedVersionOfNeoAxisEngine )
					result += "Modified version of ";
				result += OriginalName;
				return result;
			}
		}

		public static string NameWithVersion
		{
			get
			{
				Version v = Assembly.GetExecutingAssembly().GetName().Version;
				return NameWithoutVersion + $" {v.Major}.{v.Minor} ({WWW})";
			}
		}

		public static Bitmap SplashLogoImage
		{
			get { return Properties.Resources.PoweredBy; }
		}
	}
}
