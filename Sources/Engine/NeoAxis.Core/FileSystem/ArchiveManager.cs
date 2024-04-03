// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace NeoAxis
{
	//!!!!public
	static class ArchiveManager
	{
		static List<BakingArchive> archives = new List<BakingArchive>();
		static Dictionary<string, BakingArchive> files = new Dictionary<string, BakingArchive>();

		/////////////////////////////////////////

		internal static bool Init()
		{
			//!!!!error handling

			try
			{
				foreach( var path in Directory.GetFiles( VirtualFileSystem.Directories.Assets, "*.neoaxisbaking", SearchOption.AllDirectories ) )
					LoadBakingFile( path );
			}
			catch( Exception e )
			{
				Log.Error( e.Message );
				return false;
			}

			return true;
		}

		/////////////////////////////////////////

		class BakingArchive
		{
			string fullPath;
			ZipArchive zipArchive;
			Dictionary<string, ZipArchiveEntry> entries = new Dictionary<string, ZipArchiveEntry>();

			//

			public BakingArchive( string path )
			{
				fullPath = PathUtility.NormalizePath( path );
				zipArchive = ZipFile.Open( path, ZipArchiveMode.Read );

				var virtualFolder = VirtualPathUtility.GetVirtualPathByReal( Path.GetDirectoryName( path ) );

				foreach( var entry in zipArchive.Entries )
				{
					var fileKey = VirtualPathUtility.NormalizePath( Path.Combine( virtualFolder, entry.FullName ) ).ToLower();

					entries[ fileKey ] = entry;
					files[ fileKey ] = this;
				}
			}

			public void Dispose()
			{
				var toRemove = new List<string>();
				foreach( var item in files )
				{
					if( item.Value == this )
						toRemove.Add( item.Key );
				}
				foreach( var file in toRemove )
					files.Remove( file );

				zipArchive?.Dispose();
				zipArchive = null;
			}

			public string FullPath
			{
				get { return fullPath; }
			}

			public VirtualFileStream FileOpen( string fileKey )
			{
				entries.TryGetValue( fileKey, out var entry );

				using( var stream = entry.Open() )
				{
					var bytes = new byte[ entry.Length ];
					IOUtility.ReadGuaranteed( stream, bytes );
					//stream.Read( bytes, 0, bytes.Length );

					//decode
					for( int n = 0; n < bytes.Length; n++ )
						bytes[ n ] = (byte)~bytes[ n ];

					return new MemoryVirtualFileStream( bytes );
				}
			}

			public long FileGetLength( string fileKey )
			{
				entries.TryGetValue( fileKey, out var entry );
				return entry.Length;
			}
		}

		/////////////////////////////////////////

		static BakingArchive GetArchive( string path )
		{
			var path2 = PathUtility.NormalizePath( path );

			foreach( var archive in archives )
			{
				if( string.Compare( path2, archive.FullPath, true ) == 0 )
					return archive;
			}
			return null;
		}

		public static void LoadBakingFile( string path )
		{
			if( GetArchive( path ) == null )
			{
				var archive = new BakingArchive( path );
				archives.Add( archive );
			}
		}

		public static void UnloadBakingFile( string path )
		{
			var archive = GetArchive( path );
			if( archive != null )
			{
				archive.Dispose();
				archives.Remove( archive );
			}
		}

		public static bool FileExists( string path )
		{
			if( archives.Count != 0 )
			{
				string fileKey = VirtualPathUtility.NormalizePath( path ).ToLower();
				return files.ContainsKey( fileKey );
			}
			return false;
		}

		public static (bool fileExists, VirtualFileStream stream) FileOpen( string path )
		{
			string fileKey = VirtualPathUtility.NormalizePath( path ).ToLower();
			if( files.TryGetValue( fileKey, out var archive ) )
				return (true, archive.FileOpen( fileKey ));
			else
				return (false, null);
		}

		public static (bool fileExists, long length) FileGetLength( string path )
		{
			string fileKey = VirtualPathUtility.NormalizePath( path ).ToLower();
			if( files.TryGetValue( fileKey, out var archive ) )
				return (true, archive.FileGetLength( fileKey ));
			else
				return (false, 0);
		}
	}
}
