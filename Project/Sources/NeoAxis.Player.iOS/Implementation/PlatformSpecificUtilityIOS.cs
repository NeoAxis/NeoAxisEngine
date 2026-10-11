// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeoAxis
{
	class PlatformSpecificUtilityIOS : PlatformSpecificUtility
	{
		//public PlatformSpecificUtilityIOS()
		//{
		//	SetInstance( this );
		//}

		public override string GetExecutableDirectoryPath()
		{
			//!!!!
			return "";
		}

		public override EDialogResult ShowMessageBox( string text, string caption, EMessageBoxButtons buttons = EMessageBoxButtons.OK )
		{
			//!!!!buttons, result
			Console.WriteLine( "MESSAGE:\r\n" + caption + ":" + text );
			return EDialogResult.OK;
		}

		public async override Task<string> GetClipboardTextAsync()
		{
			//!!!!impl

			return "";
		}

		public override void SetClipboardText( string text )
		{
			//!!!!impl
		}
	}
}
