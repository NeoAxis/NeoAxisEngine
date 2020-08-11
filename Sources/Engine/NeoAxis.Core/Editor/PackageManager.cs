// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace NeoAxis.Editor
{
	static class PackageManager
	{
		/////////////////////////////////////////

		public class PackageInfo
		{
			public string FullFilePath;
			public string Name;
			public string Version;

			//!!!!
			public string Author;
			public string Description;
			//public bool OnlyPro;
			public long Size;
			public string Download;
			public bool SecureDownload;
			//!!!!string
			public string Date;
			public string Files;

			public override string ToString()
			{
				return GetDisplayName();// + " " + Version;
			}

			public string GetDisplayName()
			{
				return Name.Replace( '_', ' ' );
			}
		}

		/////////////////////////////////////////

		public class PackageArchiveInfo
		{
			//public string Name;
			//public string Version;
			public string Author = "";
			public string Description = "";
			public List<string> Files = new List<string>();
			public bool MustRestart = false;
			public string OpenAfterInstall = "";
			public string AddCSharpFilesToProject = "";
		}

		/////////////////////////////////////////

		public static string PackagesFolder
		{
			get { return Path.Combine( VirtualFileSystem.Directories.Project, "EnginePackages" ); }
		}

		public static List<PackageInfo> GetPackagesInfo()
		{
			var result = new List<PackageInfo>();

			for( int nExtension = 0; nExtension < 2; nExtension++ )
			{
				var filter = nExtension == 0 ? "*.neoaxispackage" : "*.zip";

				foreach( var fileName in Directory.GetFiles( PackagesFolder, filter ) )
				{
					var fileBase = Path.GetFileNameWithoutExtension( fileName );
					var strings = fileBase.Split( new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries );
					if( strings.Length >= 2 )
					{
						var info = new PackageInfo();
						info.FullFilePath = fileName;
						info.Name = strings[ 0 ].Replace( '_', ' ' );
						info.Version = strings[ 1 ];
						result.Add( info );
					}
				}
			}

			CollectionUtility.MergeSort( result, delegate ( PackageInfo p1, PackageInfo p2 )
			{
				return string.Compare( p1.Name, p2.Name );
			} );

			return result;
		}

		public static bool IsInstalled( string packageName )
		{
			var fileName = Path.Combine( PackagesFolder, "PackagesState.txt" );
			if( File.Exists( fileName ) )
			{
				var block = TextBlockUtility.LoadFromRealFile( fileName, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					EditorMessageBox.ShowWarning( error );
					return false;
				}

				foreach( var child in block.Children )
				{
					if( child.Name == "Package" )
					{
						if( child.GetAttribute( "Name" ) == packageName )
							return true;
					}
				}
			}
			return false;
		}

		public static bool ChangeInstalledState( string packageName, bool installed )
		{
			var fileName = Path.Combine( PackagesFolder, "PackagesState.txt" );

			//load
			TextBlock block;
			if( File.Exists( fileName ) )
			{
				block = TextBlockUtility.LoadFromRealFile( fileName, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					EditorMessageBox.ShowWarning( error );
					return false;
				}
			}
			else
				block = new TextBlock();

			//update
			{
				foreach( var child in block.Children )
				{
					if( child.Name == "Package" )
					{
						if( child.GetAttribute( "Name" ) == packageName )
						{
							block.DeleteChild( child );
							break;
						}
					}
				}

				if( installed )
				{
					var child = block.AddChild( "Package" );
					child.SetAttribute( "Name", packageName );
				}
			}

			//save
			{
				if( !Directory.Exists( PackagesFolder ) )
					Directory.CreateDirectory( PackagesFolder );

				TextBlockUtility.SaveToRealFile( block, fileName, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					EditorMessageBox.ShowWarning( error );
					return false;
				}
			}

			return true;
		}

		public static PackageArchiveInfo ReadPackageArchiveInfo( string fullPath, out string error )
		{
			error = "";

			try
			{
				using( var archive = ZipFile.OpenRead( fullPath ) )
				{
					PackageArchiveInfo info = new PackageArchiveInfo();

					//read Package.info
					var infoFile = archive.GetEntry( "Package.info" );
					using( var reader = new StreamReader( infoFile.Open() ) )
					{
						var infoText = reader.ReadToEnd();

						var block = TextBlock.Parse( infoText, out error );
						if( block == null )
							return null;

						info.Author = block.GetAttribute( "Author" );
						info.Description = block.GetAttribute( "Description" );
						if( bool.TryParse( block.GetAttribute( "MustRestart" ), out var v ) )
							info.MustRestart = v;
						info.OpenAfterInstall = block.GetAttribute( "OpenAfterInstall" );
						info.AddCSharpFilesToProject = block.GetAttribute( "AddCSharpFilesToProject" );
					}

					//get list of files
					foreach( var entry in archive.Entries )
					{
						var fileName = entry.FullName;
						bool directory = fileName[ fileName.Length - 1 ] == '/';
						if( fileName != "Package.info" && !directory )
							info.Files.Add( fileName );
					}

					return info;
				}
			}
			catch( Exception e )
			{
				error = e.Message;
				return null;
			}
		}

		public static void DeleteFilesAsStartup()
		{
			try
			{
				var txtFileName = Path.Combine( PackagesFolder, "FilesToDeleteAtStartup.txt" );
				if( File.Exists( txtFileName ) )
				{
					var fileNames = File.ReadAllLines( txtFileName );

					foreach( var file in fileNames )
					{
						var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );

						var startTime = DateTime.Now;
						Exception e = null;
						do
						{
							try
							{
								if( File.Exists( fullName ) )
									File.Delete( fullName );
								e = null;
								break;
							}
							catch( Exception e2 )
							{
								e = e2;
							}

						} while( ( DateTime.Now - startTime ).TotalSeconds < 30 );

						if( e != null )
							throw e;
					}

					File.Delete( txtFileName );
				}
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
			}
		}

		public static void AddFilesToDeletionAtStartup( List<string> fileNames )
		{
			try
			{
				var txtFileName = Path.Combine( PackagesFolder, "FilesToDeleteAtStartup.txt" );
				foreach( var file in fileNames )
					File.AppendAllText( txtFileName, file + "\r\n" );
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowInfo( e.Message );
			}
		}
	}
}
