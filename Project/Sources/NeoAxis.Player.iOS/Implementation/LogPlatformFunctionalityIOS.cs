// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Internal;

namespace NeoAxis
{
	class LogPlatformFunctionalityIOS : LogPlatformFunctionality
	{
		public LogPlatformFunctionalityIOS()
		{
			SetInstance( this );
		}

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons = EMessageBoxButtons.OK )
		{
			//!!!!buttons, result

			Console.WriteLine( "MESSAGE:\r\n" + caption + ":" + text );
			//Android.Util.Log.WriteLine( Android.Util.LogPriority.Debug, "MyApp", "MESSAGE:\r\n" + caption + ":" + text );

			return EDialogResult.OK;
		}
	}
}
