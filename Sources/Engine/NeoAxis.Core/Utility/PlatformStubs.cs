// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;

#if ANDROID || UWP || IOS || WEB

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

	public class SuppressGCTransitionAttribute : Attribute
	{
	}
}

#if UWP || WEB
namespace System.Drawing
{
	public class Bitmap
	{
	}

	public class Icon : IDisposable
	{
		public void Dispose()
		{
		}
	}
}
#endif

namespace System.Windows.Forms
{
	public class Cursor
	{
	}
}

namespace System.Drawing.Design
{ 
}

#endif
