// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// A class for tracking changes to a virtual file system.
	/// </summary>
	public static class VirtualFileWatcher
	{
		static FileSystemWatcher systemWatcher;
		static List<FileSystemEventArgs> newEvents = new List<FileSystemEventArgs>();
		static object lockObject = new object();

		public delegate void UpdateDelegate( FileSystemEventArgs args );
		public static event UpdateDelegate Update;


		//

		public static void Init()
		{
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows || SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP || SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
			{
				systemWatcher = new FileSystemWatcher( VirtualFileSystem.Directories.Assets );
				//!!!!?
				systemWatcher.InternalBufferSize = 32768;
				systemWatcher.IncludeSubdirectories = true;
				systemWatcher.Created += fileSystemWatcher_Event;
				systemWatcher.Deleted += fileSystemWatcher_Event;
				systemWatcher.Renamed += fileSystemWatcher_Event;
				systemWatcher.Changed += fileSystemWatcher_Event;
				systemWatcher.EnableRaisingEvents = true;
			}
		}

		public static void Shutdown()
		{
			if( systemWatcher != null )
			{
				systemWatcher.Dispose();
				systemWatcher = null;
			}
		}

		public static bool Enabled
		{
			get { return systemWatcher.EnableRaisingEvents; }
			set { systemWatcher.EnableRaisingEvents = value; }
		}

		static void fileSystemWatcher_Event( object sender, FileSystemEventArgs e )
		{
			//different threads
			lock( lockObject )
			{
				newEvents.Add( e );
			}
		}

		static List<FileSystemEventArgs> GetNewEvents()
		{
			List<FileSystemEventArgs> list;

			lock( lockObject )
			{
				//copy to list (exclude repeats)
				list = new List<FileSystemEventArgs>( newEvents.Count );
				foreach( FileSystemEventArgs evt in newEvents )
				{
					//ignore repeats
					if( evt.ChangeType == WatcherChangeTypes.Changed )
					{
						int index = list.FindIndex( delegate ( FileSystemEventArgs evt2 )
						{
							return ( string.Compare( evt2.FullPath, evt.FullPath, true ) == 0 );
						} );
						if( index != -1 )
							continue;
					}

					list.Add( evt );
				}

				newEvents.Clear();
			}

			return list;
		}

		public static void ProcessEvents()
		{
			foreach( var args in GetNewEvents() )
				Update?.Invoke( args );
		}
	}
}
