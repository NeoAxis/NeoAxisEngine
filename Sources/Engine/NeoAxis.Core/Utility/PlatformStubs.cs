// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;


#if NETSTANDARD2_1 || UWP
namespace System
{
	public class SuppressGCTransitionAttribute : Attribute
	{
	}
}
#endif


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


#if !WEB && !ANDROID
namespace System
{
	public class UserPreferenceChangedEventArgs
	{
	}
}
#endif


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
