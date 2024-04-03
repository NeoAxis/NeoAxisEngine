#if CLOUD
#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeoAxis.Editor
{
	public class RepositoryFileWatcher
	{
		FileSystemWatcher systemWatcher;
		List<FileSystemEventArgs> newEvents = new List<FileSystemEventArgs>();
		object lockObject = new object();

		public delegate void UpdateDelegate( FileSystemEventArgs args );
		public event UpdateDelegate Update;


		//

		public void Init( string folder )
		{
			systemWatcher = new FileSystemWatcher( folder );
			systemWatcher.InternalBufferSize = 32768;
			systemWatcher.IncludeSubdirectories = true;
			systemWatcher.Created += fileSystemWatcher_Event;
			systemWatcher.Deleted += fileSystemWatcher_Event;
			systemWatcher.Renamed += fileSystemWatcher_Event;
			systemWatcher.Changed += fileSystemWatcher_Event;
			systemWatcher.EnableRaisingEvents = true;
		}

		public void Shutdown()
		{
			if( systemWatcher != null )
			{
				systemWatcher.Dispose();
				systemWatcher = null;
			}
		}

		public bool Enabled
		{
			get { return systemWatcher.EnableRaisingEvents; }
			set { systemWatcher.EnableRaisingEvents = value; }
		}

		void fileSystemWatcher_Event( object sender, FileSystemEventArgs e )
		{
			//different threads
			lock( lockObject )
			{
				newEvents.Add( e );
			}
		}

		List<FileSystemEventArgs> GetNewEvents()
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
							return string.Compare( evt2.FullPath, evt.FullPath, true ) == 0;
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

		public void ProcessEvents()
		{
			foreach( var args in GetNewEvents() )
				Update?.Invoke( args );
		}
	}

}
#endif
#endif