// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoAxis
{
	static class ResourceUpdate
	{
		public static void Init()
		{
			VirtualFileWatcher.Update += VirtualFileWatcher_Update;
		}

		public static void Shutdown()
		{
			VirtualFileWatcher.Update -= VirtualFileWatcher_Update;
		}

		static void VirtualFileWatcher_Update( FileSystemEventArgs args )
		{
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
