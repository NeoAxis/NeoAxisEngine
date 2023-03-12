// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using Internal;

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
	}
}
