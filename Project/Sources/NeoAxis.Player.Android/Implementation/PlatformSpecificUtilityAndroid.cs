// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using Internal;
using Plugin.Clipboard;

namespace NeoAxis
{
	class PlatformSpecificUtilityAndroid : PlatformSpecificUtility
	{
		public PlatformSpecificUtilityAndroid()
		{
			SetInstance( this );
		}

		public override string GetExecutableDirectoryPath()
		{
			//!!!!
			return "";
		}

		public override IntPtr LoadLibrary( string path )
		{
			return IntPtr.Zero;
		}

		public override string GetClipboardText()
		{
			try
			{
				return CrossClipboard.Current.GetText();
			}
			catch
			{
				return "";
			}
		}

		public override void SetClipboardText( string text )
		{
			try
			{
				CrossClipboard.Current.SetText( text );
			}
			catch { }
		}
	}
}
