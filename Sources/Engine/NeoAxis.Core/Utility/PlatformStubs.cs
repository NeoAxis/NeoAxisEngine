// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;

#if ANDROID || UWP || IOS

namespace System.Configuration
{
	internal class ApplicationSettingsBase
	{
		public static object Synchronized( object parameter )
		{
			return null;
		}
	}
}

namespace System
{
	public class UserPreferenceChangedEventArgs
	{
	}
}

#endif
