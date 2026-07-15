// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using Internal;

namespace NeoAxis
{
	/// <summary>
	/// A class for initialization Windows, macOS, Linux platforms.
	/// </summary>
	public static class Platforms
	{
		static bool initialized;

		//

		public static void Initialize()
		{
			if( initialized )
				return;
			initialized = true;

			switch( SystemSettings.CurrentPlatform )
			{
			case SystemSettings.Platform.Windows:
				new WindowsPlatformFunctionality();
				break;

			case SystemSettings.Platform.macOS:
				new MacOSPlatformFunctionality();
				break;

			case SystemSettings.Platform.Linux:
				new LinuxPlatformFunctionality();
				break;
			}
		}
	}
}
