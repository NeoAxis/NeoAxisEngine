// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Threading.Tasks;
using NeoAxis.Player.Web;
using Internal;

namespace NeoAxis
{
	class PlatformSpecificUtilityWeb : PlatformSpecificUtility
	{
		public override string GetExecutableDirectoryPath()
		{
			//!!!!
			return "";
		}

		public override async Task<string> GetClipboardTextAsync()
		{
			return await Interop.GetClipboardTextAsync();
		}

		public override void SetClipboardText( string text )
		{
			Interop.SetClipboardText( text );
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
