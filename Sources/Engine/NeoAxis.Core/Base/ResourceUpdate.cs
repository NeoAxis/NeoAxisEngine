// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace NeoAxis
{
	static class ResourceUpdate
	{
		//!!!!
		//!!!!когда отключать? какие типы отключать? опцинально дл€ симул€ции?
		//static bool enabled;


		internal static void Init()
		{
			VirtualFileWatcher.Update += VirtualFileWatcher_Update;
		}

		internal static void Shutdown()
		{
			VirtualFileWatcher.Update -= VirtualFileWatcher_Update;
		}

		private static void VirtualFileWatcher_Update( FileSystemEventArgs args )
		{
			//!!!!так?

			switch( args.ChangeType )
			{
			case WatcherChangeTypes.Created:
			case WatcherChangeTypes.Renamed:
			case WatcherChangeTypes.Changed:
				{
					var virtualPath = VirtualPathUtility.GetVirtualPathByReal( args.FullPath );
					if( !string.IsNullOrEmpty( virtualPath ) )
						ResourceManager.TryReloadResource( virtualPath );
				}
				break;

			case WatcherChangeTypes.Deleted:
				{
					//set FileWasDeleted flag
					var virtualPath = VirtualPathUtility.GetVirtualPathByReal( args.FullPath );
					if( !string.IsNullOrEmpty( virtualPath ) )
					{
						var resource = ResourceManager.GetByName( virtualPath );
						if( resource != null )
							resource.FileWasDeleted = true;
					}
				}
				break;
			}
		}
	}
}
