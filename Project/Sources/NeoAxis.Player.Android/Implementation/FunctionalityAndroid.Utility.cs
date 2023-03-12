// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using Internal;

namespace NeoAxis
{
	partial class PlatformFunctionalityAndroid : PlatformFunctionality
	{
		Stopwatch stopwatch;

		public override double GetSystemTime()
		{
			if( stopwatch == null )
			{
				stopwatch = new Stopwatch();
				stopwatch.Start();
			}

			return stopwatch.Elapsed.TotalSeconds;
		}

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
