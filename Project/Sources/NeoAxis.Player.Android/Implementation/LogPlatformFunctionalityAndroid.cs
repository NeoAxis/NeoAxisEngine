// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace NeoAxis
{
	class LogPlatformFunctionalityAndroid : LogPlatformFunctionality
	{
		public LogPlatformFunctionalityAndroid()
		{
			SetInstance( this );
		}

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons = EMessageBoxButtons.OK )
		{
			//!!!!buttons, result

			Android.Util.Log.WriteLine( Android.Util.LogPriority.Debug, "MyApp", "MESSAGE:\r\n" + caption + ":" + text );

			return EDialogResult.OK;
		}
	}
}
