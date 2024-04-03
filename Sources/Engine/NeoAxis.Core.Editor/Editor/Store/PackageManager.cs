#if !DEPLOY
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
	public static class PackageManager
	{
		static Dictionary<string, PackageInfo> cachedPackages = new Dictionary<string, PackageInfo>();
		static ESet<string> cachedInstalledPackages = new ESet<string>();

		/////////////////////////////////////////

		public class PackageInfo
		{
			public StoreManager.StoreItem/*.StoreImplementation */ Store;
			public string Identifier;

			public string FullFilePath;
			public string Title;
			public string Version;
			public string Author;
			public string ShortDescription;
			public string FullDescription;
			public string Permalink;
			public string Cost;
			public long Size;
			public string FreeDownload;
			public bool SecureDownload;
			public string Date;//string
			public string Files;
			public string Categories;
			public string Tags;
			public string Thumbnail;
			public int Triangles;
			public int Vertices;
			public bool Rigged;
			public int Animations;
			public StoreProductLicense License;

			bool updatedFromArchive;

			//

			public override string ToString()
			{
				return Identifier.Replace( '_', ' ' );
				//return Title;// + " " + Version;
			}

			public int CostNumber
			{
				get
				{
					var result = 0;
					if( !string.IsNullOrEmpty( Cost ) )
						int.TryParse( Cost, out result );
					return result;
				}
			}

			public void UpdateDataFromArchive()
			{
				if( !updatedFromArchive )
				{
					updatedFromArchive = true;

					if( !string.IsNullOrEmpty( FullFilePath ) && File.Exists( FullFilePath ) )
					{
						var info = ReadPackageArchiveInfo( FullFilePath, out _ );
						if( info != null )
						{
							if( string.IsNullOrEmpty( Files ) )
							{
								var files = "";
								foreach( var file in info.Files )
								{
									if( files != "" )
										files += "\r\n";
									files += file;
								}
								Files = files;
							}

							if( !string.IsNullOrEmpty( info.Title ) )
								Title = info.Title;
							if( !string.IsNullOrEmpty( info.Permalink ) )
								Permalink = info.Permalink;
							if( !string.IsNullOrEmpty( info.Thumbnail ) )
								Thumbnail = info.Thumbnail;
							if( info.Triangles != 0 )
								Triangles = info.Triangles;
							if( info.Vertices != 0 )
								Vertices = info.Vertices;
							if( info.Rigged )
								Rigged = info.Rigged;
							if( info.Animations != 0 )
								Animations = info.Animations;
							if( !string.IsNullOrEmpty( info.ShortDescription ) )
								ShortDescription = info.ShortDescription;
							if( !string.IsNullOrEmpty( info.FullDescription ) )
								FullDescription = info.FullDescription;
							if( !string.IsNullOrEmpty( info.Author ) )
								Author = info.Author;
							if( !string.IsNullOrEmpty( info.Cost ) )
								Cost = info.Cost;
							if( info.License != StoreProductLicense.None )
								License = info.License;
							if( !string.IsNullOrEmpty( info.Categories ) )
								Categories = info.Categories;
							if( !string.IsNullOrEmpty( info.Tags ) )
								Tags = info.Tags;
							if( !string.IsNullOrEmpty( info.Store ) )
								Store = StoreManager.GetStore( info.Store );
						}

						try
						{
							if( Size == 0 )
								Size = new FileInfo( FullFilePath ).Length;
						}
						catch { }
					}
				}
			}

			string[] cachedFiles;
			public string[] GetFiles()
			{
				if( cachedFiles == null && !string.IsNullOrEmpty( Files ) )
				{
					var filesString = Files.Trim( new char[] { ' ', '\r', '\n' } );
					cachedFiles = filesString.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );
				}
				if( cachedFiles != null )
					return cachedFiles;
				else
					return Array.Empty<string>();
			}

			public static PackageInfo Merge( PackageInfo p1, PackageInfo p2 )
			{
				var result = new PackageInfo();
				result.Store = p1.Store ?? p2.Store;
				result.Identifier = p1.Identifier;

				result.FullFilePath = p1.FullFilePath ?? p2.FullFilePath;
				result.Title = p1.Title ?? p2.Title;
				result.Version = p1.Version ?? p2.Version;
				result.Author = p1.Author ?? p2.Author;
				result.ShortDescription = p1.ShortDescription ?? p2.ShortDescription;
				result.FullDescription = p1.FullDescription ?? p2.FullDescription;
				result.Permalink = p1.Permalink ?? p2.Permalink;
				result.Cost = p1.Cost ?? p2.Cost;
				result.Size = p1.Size != 0 ? p1.Size : p2.Size;
				result.FreeDownload = p1.FreeDownload ?? p2.FreeDownload;
				result.SecureDownload = p1.SecureDownload || p2.SecureDownload;
				result.Date = p1.Date ?? p2.Date;
				result.Files = p1.Files ?? p2.Files;
				result.Categories = p1.Categories ?? p2.Categories;
				result.Tags = p1.Tags ?? p2.Tags;
				result.Thumbnail = p1.Thumbnail ?? p2.Thumbnail;
				result.Triangles = p1.Triangles != 0 ? p1.Triangles : p2.Triangles;
				result.Vertices = p1.Vertices != 0 ? p1.Vertices : p2.Vertices;
				result.Rigged = p1.Rigged || p2.Rigged;
				result.Animations = p1.Animations != 0 ? p1.Animations : p2.Animations;
				result.License = p1.License != StoreProductLicense.None ? p1.License : p2.License;

				return result;
			}

			public enum FileTypeToDrop
			{
				None,
				Mesh,
				Material,
				Environment,
				Surface,
			}

			public (FileTypeToDrop type, string file) GetFileToDrop()
			{
				var files = GetFiles();

				//Environment
				{
					var selectedFiles = new List<(ResourceManager.ResourceType, string)>();

					foreach( var file in files )
					{
						try
						{
							var type = ResourceManager.GetTypeByFileExtension( Path.GetExtension( file ) );
							//!!!!other types
							if( type != null && type.Name == "Sky" )
								selectedFiles.Add( (type, file) );
						}
						catch { }
					}

					if( selectedFiles.Count == 1 )
					{
						var realFileName = Path.Combine( VirtualFileSystem.Directories.Project, selectedFiles[ 0 ].Item2 );
						var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );

						if( !string.IsNullOrEmpty( virtualFileName ) )
							return (FileTypeToDrop.Environment, virtualFileName);
					}
				}

				//Surface
				{
					var selectedFiles = new List<(ResourceManager.ResourceType, string)>();

					foreach( var file in files )
					{
						try
						{
							var type = ResourceManager.GetTypeByFileExtension( Path.GetExtension( file ) );
							if( type != null && type.Name == "Surface" )
								selectedFiles.Add( (type, file) );
						}
						catch { }
					}

					if( selectedFiles.Count == 1 )
					{
						var realFileName = Path.Combine( VirtualFileSystem.Directories.Project, selectedFiles[ 0 ].Item2 );
						var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );

						if( !string.IsNullOrEmpty( virtualFileName ) )
							return (FileTypeToDrop.Surface, virtualFileName);
					}
				}

				//!!!!maybe check Models category. also can add support for droping weapons, characters and vehicles
				//disable dropping high level components
				{
					var containsHighLevelComponent = false;

					foreach( var file in files )
					{
						try
						{
							var type = ResourceManager.GetTypeByFileExtension( Path.GetExtension( file ) );
							if( type != null )
							{
								if( type.Name == "Weapon Type" || type.Name == "Character Type" || type.Name == "Fence Type" || type.Name == "Pipe Type" || type.Name == "Vehicle Type" )
									containsHighLevelComponent = true;
							}
						}
						catch { }
					}

					if( containsHighLevelComponent )
						return (FileTypeToDrop.None, "");
				}

				//Mesh
				{
					var selectedFiles = new List<(ResourceManager.ResourceType, string)>();

					foreach( var file in files )
					{
						try
						{
							var type = ResourceManager.GetTypeByFileExtension( Path.GetExtension( file ) );
							if( type != null && ( type.Name == "Import 3D" || type.Name == "Mesh" ) )
								selectedFiles.Add( (type, file) );
						}
						catch { }
					}

					if( selectedFiles.Count == 1 )
					{
						var type = selectedFiles[ 0 ].Item1;
						var realFileName = Path.Combine( VirtualFileSystem.Directories.Project, selectedFiles[ 0 ].Item2 );
						var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );

						if( !string.IsNullOrEmpty( virtualFileName ) )
						{
							if( type.Name == "Import 3D" )
								return (FileTypeToDrop.Mesh, virtualFileName + "|$Mesh");
							else
								return (FileTypeToDrop.Mesh, virtualFileName);
						}
					}
				}

				//Material
				{
					var selectedFiles = new List<(ResourceManager.ResourceType, string)>();

					foreach( var file in files )
					{
						try
						{
							var type = ResourceManager.GetTypeByFileExtension( Path.GetExtension( file ) );
							if( type != null && type.Name == "Material" )
								selectedFiles.Add( (type, file) );
						}
						catch { }
					}

					if( selectedFiles.Count == 1 )
					{
						var realFileName = Path.Combine( VirtualFileSystem.Directories.Project, selectedFiles[ 0 ].Item2 );
						var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( realFileName );

						if( !string.IsNullOrEmpty( virtualFileName ) )
							return (FileTypeToDrop.Material, virtualFileName);
					}
				}

				return (FileTypeToDrop.None, "");
			}

			public static string GetSizeAsString( long size )
			{
				if( size == 0 )
					return "";

				var s = size / 1024 / 1024;
				if( s >= 1 )
					return s.ToString() + " MB";
				else
				{
					s = size / 1024;
					return s.ToString() + " KB";
				}
			}

			public static string GetTrianglesVerticesAsString( int count )
			{
				if( count >= 1000000 )
				{
					var v = ( (double)count / 1000000 ).ToString( "F1" );
					return v.Replace( ".0", "" ) + "M";
				}

				if( count >= 1000 )
				{
					var v = ( (double)count / 1000 ).ToString( "F1" );
					return v.Replace( ".0", "" ) + "k";
				}

				return count.ToString();
			}

			public string GetTooltipDescription()
			{
				var d = Title;

				if( Triangles != 0 )
				{
					d += "\r\nTriangles " + GetTrianglesVerticesAsString( Triangles );
					d += ", Vertices " + GetTrianglesVerticesAsString( Vertices );
				}

				if( Rigged )
					d += "\r\nRigged";

				if( Animations != 0 )
					d += "\r\nAnimations " + Animations;

				if( Size != 0 )
					d += "\r\n" + GetSizeAsString( Size );

				if( License != StoreProductLicense.None && !License.ToString().Contains( "Paid" ) )
					d += "\r\nFree (" + EnumUtility.GetValueDisplayName( License ) + ")";
				else if( CostNumber > 0 || !string.IsNullOrEmpty( FreeDownload ) )
				{
					d += "\r\n";
					if( CostNumber > 0 )
						d += "$" + CostNumber.ToString();
					else
						d += "Free";

					if( License != StoreProductLicense.None )
						d += " (" + EnumUtility.GetValueDisplayName( License ) + ")";
				}

				if( !string.IsNullOrEmpty( ShortDescription ) )
					d += "\r\n\r\n" + ShortDescription;

				//no Description update in the list of ContentBrowser
				//var state = storesWindow.GetPackageState( packageId );
				////state
				//d += "\r\n";
				//if( state.Installed )
				//	d += "Installed";
				//else if( state.Downloading )
				//	d += "Downloading";
				//else if( state.Downloaded )
				//	d += "Downloaded";
				//else
				//	d += "Not downloaded";

				return d;
			}

			//public string[] GetCategories()
			//{
			//	//!!!!cache

			//	var result = new List<string>();
			//	foreach( var category2 in Categories.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ) )
			//		result.Add( category2.Trim() );
			//	return result.ToArray();
			//}

			//public ESet<string> GetCategoriesSet()
			//{
			//	//!!!!cache

			//	var result = new ESet<string>();
			//	foreach( var category in GetCategories() )
			//		result.AddWithCheckAlreadyContained( category );
			//	return result;
			//}
		}

		/////////////////////////////////////////

		public class PackageArchiveInfo
		{
			public string Title;
			//public string Version;
			public string Author = "";
			public string ShortDescription = "";
			public string FullDescription = "";
			public List<string> Files = new List<string>();
			public bool MustRestart = false;
			public string OpenAfterInstall = "";
			public string AddCSharpFilesToProject = "";
			public string Permalink = "";
			public string Thumbnail = "";
			public int Triangles;
			public int Vertices;
			public bool Rigged;
			public int Animations;
			public string Cost = "";
			public StoreProductLicense License;
			public string Categories = "";
			public string Tags = "";
			public string Store = "";
		}

		/////////////////////////////////////////

		public static string PackagesFolder
		{
			get { return Path.Combine( VirtualFileSystem.Directories.Project, "EnginePackages" ); }
		}

		public static Dictionary<string, PackageInfo> GetPackagesInfoByFileArchives( bool update )
		{
			if( update )
			{
				//Dictionary<string, PackageInfo> oldList = new Dictionary<string, PackageInfo>();
				//foreach( var package in cachedPackages )
				//	oldList[ package.Identifier ] = package;

				cachedPackages.Clear();

				var packages = new List<PackageInfo>();

				//for( int nExtension = 0; nExtension < 2; nExtension++ )
				//{
				var filter = "*.neoaxispackage";
				//var filter = nExtension == 0 ? "*.neoaxispackage" : "*.zip";

				if( Directory.Exists( PackagesFolder ) )
				{
					foreach( var fileName in Directory.GetFiles( PackagesFolder, filter ) )
					{
						var fileBase = Path.GetFileNameWithoutExtension( fileName );
						var strings = fileBase.Split( new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries );
						if( strings.Length >= 2 )
						{
							var identifier = strings[ 0 ];

							var package = new PackageInfo();
							//if( !oldList.TryGetValue( identifier, out var package ) )
							//	package = new PackageInfo();

							package.Identifier = identifier;
							package.FullFilePath = fileName;
							package.Title = identifier.Replace( '_', ' ' );
							package.Version = strings[ 1 ];

							packages.Add( package );
							//cachedPackages.Add( package );
						}
					}
				}
				//}

				CollectionUtility.MergeSort( packages, delegate ( PackageInfo p1, PackageInfo p2 )
				{
					return string.Compare( p1.Identifier, p2.Identifier );
				} );

				foreach( var package in packages )
					cachedPackages[ package.Identifier ] = package;
			}

			return cachedPackages;
		}

		public static ESet<string> GetInstalledPackages( bool update )
		{
			if( update )
			{
				cachedInstalledPackages.Clear();

				var fileName = Path.Combine( PackagesFolder, "PackagesState.txt" );
				if( File.Exists( fileName ) )
				{
					var block = TextBlockUtility.LoadFromRealFile( fileName, out _ );
					if( block != null )
					{
						foreach( var child in block.Children )
						{
							if( child.Name == "Package" )
								cachedInstalledPackages.AddWithCheckAlreadyContained( child.GetAttribute( "Name" ) );
						}
					}
				}
			}

			return cachedInstalledPackages;
		}

		public static bool IsInstalled( string packageName, bool update )
		{
			return GetInstalledPackages( update ).Contains( packageName );
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

						info.Title = block.GetAttribute( "Title" );
						info.Author = block.GetAttribute( "Author" );
						info.ShortDescription = block.GetAttribute( "Description" );
						info.FullDescription = block.GetAttribute( "FullDescription" );

						if( bool.TryParse( block.GetAttribute( "MustRestart" ), out var mustRestart ) )
							info.MustRestart = mustRestart;

						info.OpenAfterInstall = block.GetAttribute( "OpenAfterInstall" );
						info.AddCSharpFilesToProject = block.GetAttribute( "AddCSharpFilesToProject" );
						info.Permalink = block.GetAttribute( "Permalink" );
						info.Thumbnail = block.GetAttribute( "Thumbnail" );

						if( int.TryParse( block.GetAttribute( "Triangles" ), out var triangles ) )
							info.Triangles = triangles;

						if( int.TryParse( block.GetAttribute( "Vertices" ), out var vertices ) )
							info.Vertices = vertices;

						if( bool.TryParse( block.GetAttribute( "Rigged" ), out var rigged ) )
							info.Rigged = rigged;

						if( int.TryParse( block.GetAttribute( "Animations" ), out var animations ) )
							info.Animations = animations;

						info.Cost = block.GetAttribute( "Cost" );

						var license = block.GetAttribute( "License" ).Replace( " ", "" ).Replace( "-", "" );
						Enum.TryParse<StoreProductLicense>( license, out var value );
						info.License = value;

						info.Categories = block.GetAttribute( "Categories" );
						info.Tags = block.GetAttribute( "Tags" );
						info.Store = block.GetAttribute( "Store" );
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

		public static bool ReadPackageArchiveInfo_CheckOnly( string fullPath, out string error )
		{
			error = "";

			try
			{
				using( var archive = ZipFile.OpenRead( fullPath ) )
				{
					PackageArchiveInfo info = new PackageArchiveInfo();

					//read Package.info
					var infoFile = archive.GetEntry( "Package.info" );

					return infoFile != null;
				}
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
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

#endif