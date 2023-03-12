#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.IO.Compression;
using System.Linq;
using System.Xml;

namespace NeoAxis.Editor
{
	static class EditorCommandLineTools
	{
		public static bool Process()
		{
			if( SystemSettings.CommandLineParameters.ContainsKey( "-prepareProductAssets" ) )
			{
				PrepareProductAssets.Process();
				return true;
			}

			if( SystemSettings.CommandLineParameters.ContainsKey( "-platformProjectPatch" ) )
			{
				PlatformProjectPatch.Process();
				return true;
			}

			return false;
		}

		///////////////////////////////////////////////

		static class PrepareProductAssets
		{
			static SystemSettings.Platform Platform;
			static string ProjectFolder;
			static string AssetsFolder;
			//static string Platform;
			static string DestinationFolder;
			static bool CompressData;
			static bool ShaderCache;
			static bool FileCache;

			static readonly DateTime setTimeToFilesInZip = new DateTime( 2001, 1, 1, 1, 1, 1 );

			//

			static void CreateDirectory( string path )
			{
				Directory.CreateDirectory( path );

				//Directory.SetCreationTime( path, setTimeToFilesInZip );
				//Directory.SetLastWriteTime( path, setTimeToFilesInZip );
				//Directory.SetLastAccessTime( path, setTimeToFilesInZip );
			}

			static void FileCopy( string sourceFileName, string destFileName, bool overwrite )
			{
				File.Copy( sourceFileName, destFileName, overwrite );

				//File.SetCreationTime( destFileName, setTimeToFilesInZip );
				//File.SetLastWriteTime( destFileName, setTimeToFilesInZip );
				//File.SetLastAccessTime( destFileName, setTimeToFilesInZip );
			}

			public static void CopyFolder( string sourceFolder, string destFolder, Range progressRange, IEnumerable<string> excludePaths = null )
			{
				if( !Directory.Exists( sourceFolder ) )
					return;

				CreateDirectory( destFolder );

				IEnumerable<FileInfo> allFiles = new DirectoryInfo( sourceFolder ).GetFiles( "*.*", SearchOption.AllDirectories ).ToList();
				IEnumerable<string> allDirs = Directory.GetDirectories( sourceFolder, "*", SearchOption.AllDirectories ).ToList();

				// filter if needed.
				if( excludePaths != null )
				{
					allFiles = allFiles.Where( file => excludePaths.All( p => !file.FullName.Contains( p ) ) );
					allDirs = allDirs.Where( dir => excludePaths.All( p => !dir.Contains( p ) ) );
				}

				long totalLength = 0;
				foreach( var fileInfo in allFiles )
					totalLength += fileInfo.Length;

				foreach( string dirPath in allDirs )
				{
					if( Directory.Exists( dirPath ) )
						CreateDirectory( dirPath.Replace( sourceFolder, destFolder ) );
				}

				long processedLength = 0;
				foreach( var fileInfo in allFiles )
				{
					if( File.Exists( fileInfo.FullName ) )
					{
						var destFileName = fileInfo.FullName.Replace( sourceFolder, destFolder );
						FileCopy( fileInfo.FullName, destFileName, true );
					}

					//if( buildInstance.RequestCancel )
					//{
					//	buildInstance.State = ProductBuildInstance.StateEnum.Cancelled;
					//	return;
					//}

					processedLength += fileInfo.Length;
					//buildInstance.SetProgressWithRange( (double)processedLength / (double)totalLength, progressRange );
				}
			}

			static void CopyIncludeExcludePaths( IEnumerable<string> paths, Range progressRange )
			{
				var includePaths = new List<string>();
				var excludePathsRooted = new List<string>();
				foreach( var path in paths )
				{
					if( !path.Contains( "exclude:" ) )
						includePaths.Add( path );
					else
						excludePathsRooted.Add( Path.Combine( ProjectFolder, path.Replace( "exclude:", "" ) ) );
				}

				if( includePaths.Count != 0 )
				{
					var percentStep = progressRange.Size / includePaths.Count;
					var currentPercent = progressRange.Minimum;

					foreach( var includePath in includePaths )
					{
						var sourcePath = Path.Combine( ProjectFolder, includePath );
						var destPath = Path.Combine( DestinationFolder, includePath );

						//!!!!проценты от размера папки
						var percentRange = new Range( currentPercent, currentPercent + percentStep );

						if( File.Exists( sourcePath ) )
						{
							var directoryName = Path.GetDirectoryName( destPath );
							if( !Directory.Exists( directoryName ) )
								Directory.CreateDirectory( directoryName );
							File.Copy( sourcePath, destPath, true );
						}
						else
							CopyFolder( sourcePath, destPath, percentRange, excludePathsRooted );

						//if( CheckCancel( buildInstance ) )
						//	return;

						currentPercent += percentStep;
					}
				}
			}

			static void CopyFilesToPackageFolder( string pathsParameter )// string selectedAssets, string excludedAssets )
			{
				//copy files
				{
					var paths = new List<string>();

					paths.Add( "Caches" );

					//Caches
					if( !ShaderCache )
						paths.Add( @"exclude:Caches\ShaderCache" );
					if( !FileCache )
						paths.Add( @"exclude:Caches\Files" );
					paths.Add( @"exclude:Caches\CSharpScripts\CSharpScripts.cache" );
					paths.Add( @"exclude:Caches\CSharpScripts\CSharpScripts.dll" );
					paths.Add( @"exclude:Caches\CSharpScripts\CSharpScripts.pdb" );

					//Paths
					foreach( var path in pathsParameter.Split( '\n', StringSplitOptions.RemoveEmptyEntries ) )
					{
						var path2 = path.Replace( "\r", "" );

						//comment support
						var index = path2.IndexOf( "//" );
						if( index != -1 )
							path2 = path2.Substring( 0, index );

						path2 = path2.Trim();

						if( path2 != "" )
							paths.Add( path2 );
					}
					//foreach( var path in pathsParameter.Split( '\n', StringSplitOptions.RemoveEmptyEntries ) )
					//{
					//	var path2 = path.Replace( "\r", "" ).Trim();
					//	if( path2 != "" )
					//		paths.Add( path2 );
					//}

					//remove rooted paths
					for( int n = 0; n < paths.Count; n++ )
						paths[ n ] = paths[ n ].Replace( ProjectFolder + Path.DirectorySeparatorChar, "" );

					//execute
					CopyIncludeExcludePaths( paths, new Range( 0, 0.4 ) );
				}

				////copy Assets
				//{
				//	string sourceAssetsPath = AssetsFolder;
				//	string destAssetsPath = Path.Combine( DestinationFolder, "Assets" );

				//	var assetsExcludePaths = new List<string>();
				//	{
				//		assetsExcludePaths.Add( Path.Combine( sourceAssetsPath, @"Base\Tools" ) );
				//		assetsExcludePaths.Add( Path.Combine( sourceAssetsPath, @"Base\Learning" ) );

				//		var excluded = excludedAssets.Trim();
				//		if( !string.IsNullOrEmpty( excluded ) )
				//		{
				//			foreach( var v in excluded.Split( new char[] { ';', '\r', '\n' } ) )
				//			{
				//				var v2 = v.Trim( ' ', '\t' );
				//				if( !string.IsNullOrEmpty( v2 ) )
				//					assetsExcludePaths.Add( Path.Combine( sourceAssetsPath, v2 ) );
				//			}
				//		}
				//	}

				//	bool skipDefaultBehavior = false;

				//	var values = selectedAssets.Trim();// SelectedAssets.Value.Trim();
				//	if( !string.IsNullOrEmpty( values ) )
				//	{
				//		foreach( var v in values.Split( new char[] { ';', '\r', '\n' } ) )
				//		{
				//			var v2 = v.Trim( ' ', '\t' );
				//			if( !string.IsNullOrEmpty( v2 ) )
				//			{
				//				var sourceFolder2 = Path.Combine( AssetsFolder, v2 );
				//				var destFolder2 = Path.Combine( DestinationFolder, "Assets", v2 );
				//				var percentRange2 = new Range( 0.0, 0.0 );

				//				CopyFolder( sourceFolder2, destFolder2, percentRange2, assetsExcludePaths );

				//				//if( CheckCancel( buildInstance ) )
				//				//	return;
				//			}
				//		}

				//		skipDefaultBehavior = true;
				//	}

				//	if( !skipDefaultBehavior )
				//		CopyFolder( AssetsFolder, destAssetsPath, new Range( 0.0, 0.3 ), assetsExcludePaths );
				//}

				////copy Caches
				//{
				//	string sourceCachesPath = Path.Combine( ProjectFolder, "Caches" );
				//	string destCachesPath = Path.Combine( DestinationFolder, "Caches" );

				//	var excludePaths = new List<string>();
				//	if( !ShaderCache )
				//		excludePaths.Add( @"Caches\ShaderCache" );
				//	if( !FileCache )
				//		excludePaths.Add( @"Caches\Files" );

				//	CopyFolder( sourceCachesPath, destCachesPath, new Range( 0.3, 0.4 ), excludePaths );
				//}


				//copy Binaries\NeoAxis.DefaultSettings.config
				{
					string sourceFolder = Path.Combine( ProjectFolder, "Binaries" );
					string destFolder = Path.Combine( DestinationFolder, "Binaries" );

					if( !Directory.Exists( destFolder ) )
						CreateDirectory( destFolder );

					FileCopy( Path.Combine( sourceFolder, "NeoAxis.DefaultSettings.config" ), Path.Combine( destFolder, "NeoAxis.DefaultSettings.config" ), true );
				}

				////copy Binaries
				//{
				//	string sourceFolder = Path.Combine( VirtualFileSystem.Directories.Project, "Binaries" );
				//	string destFolder = Path.Combine( DestinationFolder, "Binaries" );

				//	var excludePaths = new List<string>();
				//	excludePaths.AddRange( GetPlatformsExcludePaths() );

				//	CopyFolder( sourceFolder, destFolder, buildInstance, new Range( 0.4, 0.5 ), excludePaths );

				//	if( CheckCancel( buildInstance ) )
				//		return;
				//}

				////copy part of Sources
				//{
				//	var sourceFolder = Path.Combine( VirtualFileSystem.Directories.Project, "Sources" );
				//	string destFolder = Path.Combine( DestinationFolder, "Sources" );
				//	//copy Sources\Sources.Android.sln
				//	CopyFiles( sourceFolder, destFolder, buildInstance, new Range( 0.5, 0.6 ), "Sources.Android.sln" );
				//	//copy Sources\NeoAxis.Player.Android
				//	CopyFolder( Path.Combine( sourceFolder, "NeoAxis.Player.Android" ), Path.Combine( destFolder, "NeoAxis.Player.Android" ), buildInstance, new Range( 0.6, 0.7 ) );
				//}

				//create Project.zip
				{
					var destinationFileName = Path.Combine( DestinationFolder, "Project.zip" );// @"Sources\NeoAxis.Player.Android\Assets\Project.zip" );
					var compressionLevel = CompressData/* CompressData.Value*/ ? CompressionLevel.Optimal : CompressionLevel.NoCompression;

					if( File.Exists( destinationFileName ) )
						File.Delete( destinationFileName );

					var paths = new List<string>();
					paths.Add( Path.Combine( DestinationFolder, "Assets" ) );
					paths.Add( Path.Combine( DestinationFolder, @"Binaries\NeoAxis.DefaultSettings.config" ) );
					paths.Add( Path.Combine( DestinationFolder, "Caches" ) );

					using( var archive = ZipFile.Open( destinationFileName, ZipArchiveMode.Create ) )
					{
						foreach( var path in paths )
						{
							if( Directory.Exists( path ) )
							{
								foreach( var file in Directory.GetFiles( path, "*.*", SearchOption.AllDirectories ) )
								{
									//read
									var bytes = File.ReadAllBytes( file );

									//write
									var fileName = file.Substring( DestinationFolder.Length + 1 );
									var entry = archive.CreateEntry( fileName, compressionLevel );
									entry.LastWriteTime = new DateTimeOffset( setTimeToFilesInZip );
									using( var stream = entry.Open() )
										stream.Write( bytes, 0, bytes.Length );
								}
							}
							else if( File.Exists( path ) )
							{
								//read
								var bytes = File.ReadAllBytes( path );

								//write
								var fileName = path.Substring( DestinationFolder.Length + 1 );
								var entry = archive.CreateEntry( fileName, compressionLevel );
								entry.LastWriteTime = new DateTimeOffset( setTimeToFilesInZip );
								using( var stream = entry.Open() )
									stream.Write( bytes, 0, bytes.Length );
							}
						}
					}

					//create .hash file
					{
						string hashString = "";

						using( var stream = File.Open( destinationFileName, FileMode.Open ) )
						{
							using( var hashAlgorithm1 = System.Security.Cryptography.SHA1.Create() )
							{
								var hash = hashAlgorithm1.ComputeHash( stream );

								var sb = new StringBuilder( hash.Length * 2 );
								foreach( byte b in hash )
									sb.Append( b.ToString( "X2" ) );

								hashString = sb.ToString();
							}
						}

						var fileName = destinationFileName + ".hash";
						File.WriteAllText( fileName, hashString );
					}
				}

				//delete Assets, Caches, Binaries
				////delete Assets, Caches, Binaries\NeoAxis.Internal\Tips, Binaries\NeoAxis.Internal\Tools
				{
					var destFolder = Path.Combine( DestinationFolder, "Assets" );
					if( Directory.Exists( destFolder ) )
						Directory.Delete( destFolder, true );

					destFolder = Path.Combine( DestinationFolder, "Caches" );
					if( Directory.Exists( destFolder ) )
						Directory.Delete( destFolder, true );

					destFolder = Path.Combine( DestinationFolder, "Binaries" );
					if( Directory.Exists( destFolder ) )
						Directory.Delete( destFolder, true );

					//destFolder = Path.Combine( buildInstance.DestinationFolder, @"Binaries\NeoAxis.Internal\Tips" );
					//if( Directory.Exists( destFolder ) )
					//	Directory.Delete( destFolder, true );

					//destFolder = Path.Combine( buildInstance.DestinationFolder, @"Binaries\NeoAxis.Internal\Tools" );
					//if( Directory.Exists( destFolder ) )
					//	Directory.Delete( destFolder, true );
				}

			}

			public static void Process()
			{
				Console.WriteLine( "NeoAxis.Editor.exe: PrepareProductAssets: Preparing Project.zip..." );


				if( !SystemSettings.CommandLineParameters.TryGetValue( "-prepareProductAssets", out var productFileName ) )
					return;

				if( !SystemSettings.CommandLineParameters.TryGetValue( "-destinationFolder", out var destinationFolder ) )
				{
					Log.Warning( "PrepareProductAssets: -destinationFolder is not specified." );
					return;
				}

				if( !SystemSettings.CommandLineParameters.TryGetValue( "-platform", out var platform ) )
				{
					Log.Warning( "PrepareProductAssets: -platform is not specified." );
					return;
				}
				if( !Enum.TryParse( platform, out Platform ) )
				{
					Log.Warning( "PrepareProductAssets: Unable to parse platform parameter value \'{0}\'.", platform );
					return;
				}

				ProjectFolder = Path.GetFullPath( Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), ".." ) );
				AssetsFolder = Path.Combine( ProjectFolder, "Assets" );
				var productFullPath = Path.Combine( AssetsFolder, productFileName );
				DestinationFolder = destinationFolder;


				var block = TextBlockUtility.LoadFromRealFile( productFullPath, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( $"Unable to load \'{productFullPath }\'. " + error );
					return;
				}

				//delete old Project.zip. Project.zip.hash

				try
				{
					var f = Path.Combine( DestinationFolder, "Project.zip" );
					if( File.Exists( f ) )
						File.Delete( f );

					f = Path.Combine( DestinationFolder, "Project.zip.hash" );
					if( File.Exists( f ) )
						File.Delete( f );

					f = Path.Combine( DestinationFolder, "Assets" );
					if( Directory.Exists( f ) )
						Directory.Delete( f, true );

					f = Path.Combine( DestinationFolder, "Binaries" );
					if( Directory.Exists( f ) )
						Directory.Delete( f, true );

					f = Path.Combine( DestinationFolder, "Caches" );
					if( Directory.Exists( f ) )
						Directory.Delete( f, true );
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
					return;
				}

				//read options

				var childBlock = block.Children[ 0 ];

				var pathsParameter = childBlock.GetAttribute( "Paths" );
				//var selectedAssets = childBlock.GetAttribute( "SelectedAssets" );
				//var excludedAssets = childBlock.GetAttribute( "ExcludedAssets" );

				if( !bool.TryParse( childBlock.GetAttribute( "CompressData", "True" ), out CompressData ) )
					CompressData = true;
				if( !bool.TryParse( childBlock.GetAttribute( "FileCache", "True" ), out FileCache ) )
					FileCache = true;
				if( !bool.TryParse( childBlock.GetAttribute( "ShaderCache", "True" ), out ShaderCache ) )
					ShaderCache = true;

				//process

				try
				{
					CopyFilesToPackageFolder( pathsParameter );// selectedAssets, excludedAssets );
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
					return;
				}

				//Log.Warning( "Done" );
			}
		}

		///////////////////////////////////////////////

		public static class PlatformProjectPatch
		{
			public static bool Process( string destFile, string baseProjectFileName, out string error, out bool changed )
			{
				if( !Path.IsPathRooted( destFile ) )
					destFile = Path.Combine( Directory.GetCurrentDirectory(), destFile );
				if( !Path.IsPathRooted( baseProjectFileName ) )
					baseProjectFileName = Path.Combine( Directory.GetCurrentDirectory(), baseProjectFileName );

				string sourceFolder = Path.GetDirectoryName( destFile );

				error = "";
				changed = false;

				if( !File.Exists( destFile ) )
				{
					error = "Target project file not exists. File: " + destFile;
					return false;
				}

				if( !File.Exists( baseProjectFileName ) )
				{
					error = "Base project file not exists. File: " + baseProjectFileName;
					return false;
				}

				var toInclude = new ESet<string>();
				{
					var xmldoc2 = new XmlDocument();
					xmldoc2.Load( baseProjectFileName );

					var mgr2 = new XmlNamespaceManager( xmldoc2.NameTable );
					mgr2.AddNamespace( "df", xmldoc2.DocumentElement.NamespaceURI );

					//EnableDefaultCompileItems
					{
						var defaultCompileItems = true;
						foreach( XmlNode node in xmldoc2.GetElementsByTagName( "EnableDefaultCompileItems" ) )
						{
							if( !string.IsNullOrEmpty( node.InnerText ) )
							{
								defaultCompileItems = bool.Parse( node.InnerText );
								break;
							}
						}

						if( defaultCompileItems )
						{
							foreach( var f in Directory.GetFiles( sourceFolder, "*.cs", SearchOption.AllDirectories ) )
							{
								var name = f.Replace( sourceFolder + "\\", "" );
								toInclude.AddWithCheckAlreadyContained( name );
							}
						}
					}

					//Compile Include
					{
						var list = xmldoc2.SelectNodes( "//df:Compile", mgr2 );
						foreach( XmlNode node in list )
						{
							var attr = node.Attributes[ "Include" ];
							if( attr != null )
							{
								var name = attr.Value;
								toInclude.AddWithCheckAlreadyContained( name );
							}
						}
					}

					//Compile Remove
					{
						var list = xmldoc2.SelectNodes( "//df:Compile", mgr2 );
						foreach( XmlNode node in list )
						{
							var attr = node.Attributes[ "Remove" ];
							if( attr != null )
							{
								var t = attr.Value;

								if( t.Length >= 2 && t[ t.Length - 2 ] == '*' && t[ t.Length - 1 ] == '*' )
								{
									var t2 = t.Substring( 0, t.Length - 2 );

									again:;
									foreach( var name in toInclude )
									{
										if( name.Length >= t2.Length && name.Substring( 0, t2.Length ) == t2 )
										{
											toInclude.Remove( name );
											goto again;
										}
									}
								}
								else
									toInclude.Remove( t );
							}
						}
					}

				}

				var xmldoc = new XmlDocument();
				xmldoc.Load( destFile );

				var mgr = new XmlNamespaceManager( xmldoc.NameTable );
				mgr.AddNamespace( "df", xmldoc.DocumentElement.NamespaceURI );

				//find parent to add

				XmlNode itemGroupNode = null;
				{
					var list = xmldoc.SelectNodes( "//df:Compile", mgr );
					//var list = xmldoc.SelectNodes( "//Compile" );
					foreach( XmlNode node in list )
					{
						itemGroupNode = node.ParentNode;
						break;
					}
				}

				var nodesChanged = false;

				//remove
				{
					var nodesToRemove = new List<XmlNode>();

					var list = xmldoc.SelectNodes( "//df:Compile", mgr );
					//var list = xmldoc.SelectNodes( "//Compile" );
					foreach( XmlNode node in list )
					{
						var attr = node.Attributes[ "Include" ];
						if( attr != null )
						{
							var name = attr.Value;

							if( toInclude.Contains( name ) )
								toInclude.Remove( name );
							else
								nodesToRemove.Add( node );
						}
					}

					foreach( var node in nodesToRemove.GetReverse() )
						node.ParentNode.RemoveChild( node );

					if( nodesToRemove.Count != 0 )
						nodesChanged = true;
				}

				//add
				foreach( var fileName in toInclude )
				{
					var node = xmldoc.CreateNode( XmlNodeType.Element, "Compile", null );
					var includeAttribute = xmldoc.CreateAttribute( "Include" );
					includeAttribute.Value = fileName;
					node.Attributes.Append( includeAttribute );
					itemGroupNode.AppendChild( node );

					nodesChanged = true;
				}

				//save

				if( nodesChanged )
				{
					var oldFile = File.ReadAllText( destFile, Encoding.UTF8 );

					var stream = new MemoryStream();
					xmldoc.Save( stream );
					stream.Seek( 0, SeekOrigin.Begin );
					var reader = new StreamReader( stream );//, Encoding.UTF8 );
					string text = reader.ReadToEnd();

					//remove xmlns=""
					text = text.Replace( " xmlns=\"\"", "" );

					if( oldFile != text )
					{
						File.WriteAllText( destFile, text );
						changed = true;
					}

					//var oldFile = File.ReadAllText( destFile, Encoding.UTF8 );

					////!!!!save once, not twice

					//xmldoc.Save( destFile );

					//var text = File.ReadAllText( destFile );
					////remove xmlns=""
					//text = text.Replace( " xmlns=\"\"", "" );

					//File.WriteAllText( destFile, text );

					//changed = oldFile != text;
				}

				return true;
			}

			public static void Process()
			{
				Console.WriteLine( "NeoAxis.Editor.exe: PlatformProjectPatch." );
				//Console.WriteLine();

				if( !SystemSettings.CommandLineParameters.TryGetValue( "-platformProjectPatch", out var destFile ) )
					return;
				if( !SystemSettings.CommandLineParameters.TryGetValue( "-baseProject", out var baseProjectFileName ) )
				{
					Log.Warning( "PlatformProjectPatch: -baseProject is not specified." );
					return;
				}

				if( !Process( destFile, baseProjectFileName, out var error, out var changed ) )
				{
					Log.Warning( error );
					return;
				}

				if( changed )
					Console.WriteLine( "Done. The file was changed. Need to rebuild the solution." );
				else
					Console.WriteLine( "Done. No changes." );
			}
		}
	}
}

#endif
#endif