// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Globalization;

namespace NeoAxis
{
	public partial class PlatformFunctionalityWeb
	{
		public override string[] GetNativeModuleNames()
		{
			return new string[ 0 ];
		}

		public override bool ShowMessageBoxYesNoQuestion( string text, string caption )
		{
			return false;
		}

		public override void GetSystemLanguage( out string name, out string englishName )
		{
			name = CultureInfo.CurrentUICulture.Name;
			englishName = CultureInfo.CurrentUICulture.EnglishName;
		}

		public override IntPtr CallPlatformSpecificMethod( string message, IntPtr param )
		{
			return IntPtr.Zero;
		}
	}
}
