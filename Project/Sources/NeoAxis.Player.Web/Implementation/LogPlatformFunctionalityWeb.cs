// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using Internal;

namespace NeoAxis
{
	public partial class PlatformFunctionalityWeb
	{
		class LogPlatformFunctionalityWeb : LogPlatformFunctionality
		{
			public LogPlatformFunctionalityWeb()
			{
				SetInstance( this );
			}

			public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons = EMessageBoxButtons.OK )
			{
				//!!!!buttons, result

				//TODO: message box
				Console.WriteLine( "MESSAGE:\r\n" + caption + ":" + text );

				return EDialogResult.OK;
			}
		}
	}
}
