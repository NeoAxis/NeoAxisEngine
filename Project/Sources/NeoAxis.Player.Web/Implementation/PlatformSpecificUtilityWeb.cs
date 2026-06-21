// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Threading.Tasks;

namespace NeoAxis
{
	internal class PlatformSpecificUtilityWeb : PlatformSpecificUtility
	{
		public PlatformSpecificUtilityWeb()
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

		public override async Task<string> GetClipboardTextAsync()
		{

			//!!!!impl
			//TODO: request copy from clipboard

			return "";
		}

		public override void SetClipboardText( string text )
		{

			//!!!!impl
			//TODO: request copy to clipboard

		}
	}
}
