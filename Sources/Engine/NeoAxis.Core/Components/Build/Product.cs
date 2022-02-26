// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents the product build settings.
	/// </summary>
	[ResourceFileExtension( "product" )]
#if !DEPLOY
	[EditorControl( typeof( ProductEditor ) )]
	[SettingsCell( typeof( ProductSettingsCell ) )]
#endif
	public abstract class Product : Component
	{
		/// <summary>
		/// The position of the product in the product list for build.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Product" )]
		public Reference<double> SortOrder
		{
			get { if( _sortOrder.BeginGet() ) SortOrder = _sortOrder.Get( this ); return _sortOrder.value; }
			set { if( _sortOrder.BeginSet( ref value ) ) { try { SortOrderChanged?.Invoke( this ); } finally { _sortOrder.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SortOrder"/> property value changes.</summary>
		public event Action<Product> SortOrderChanged;
		ReferenceField<double> _sortOrder = 0.0;

		//!!!!make SelectFiles window
		/// <summary>
		/// The list of folders and files to add. It is specified by the list, list items are separated by return. The item can have a prefix 'exclude:' to remove selected path.
		/// </summary>
		[DefaultValue( pathsDefault )]
		[Category( "Files" )]
		[Editor( typeof( HCItemTextBoxDropMultiline ), typeof( object ) )]
		public Reference<string> Paths
		{
			get { if( _paths.BeginGet() ) Paths = _paths.Get( this ); return _paths.value; }
			set { if( _paths.BeginSet( ref value ) ) { try { PathsChanged?.Invoke( this ); } finally { _paths.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Paths"/> property value changes.</summary>
		public event Action<Product> PathsChanged;
		ReferenceField<string> _paths = pathsDefault;
		const string pathsDefault = "Assets\r\n\r\nexclude:Assets\\Base\\Build\r\nexclude:Assets\\Base\\Tools\r\nexclude:Assets\\Base\\Learning\r\nexclude:Assets\\Base\\Fonts\\FlowGraphEditor.ttf\r\nexclude:Assets\\Base\\Fonts\\FlowGraphEditor.ttf.settings";

		/// <summary>
		/// Whether to include the cache of auto-compressed images.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Files" )]
		public Reference<bool> FileCache
		{
			get { if( _fileCache.BeginGet() ) FileCache = _fileCache.Get( this ); return _fileCache.value; }
			set { if( _fileCache.BeginSet( ref value ) ) { try { FileCacheChanged?.Invoke( this ); } finally { _fileCache.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FileCache"/> property value changes.</summary>
		public event Action<Product> FileCacheChanged;
		ReferenceField<bool> _fileCache = true;

		/// <summary>
		/// Whether to include shader cache.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Files" )]
		public Reference<bool> ShaderCache
		{
			get { if( _shaderCache.BeginGet() ) ShaderCache = _shaderCache.Get( this ); return _shaderCache.value; }
			set { if( _shaderCache.BeginSet( ref value ) ) { try { ShaderCacheChanged?.Invoke( this ); } finally { _shaderCache.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShaderCache"/> property value changes.</summary>
		public event Action<Product> ShaderCacheChanged;
		ReferenceField<bool> _shaderCache = true;


		//!!!!code build events and build events like Visual Studio
		//event AfterCopyFiles
		//event BeforePackaging
		//EditorCommandLineTools


		[Browsable( false )]
		public abstract SystemSettings.Platform Platform
		{
			get;
		}

		public List<string> GetPlatformsExcludePaths()
		{
			var result = new List<string>();

			var path = PathUtility.Combine( VirtualFileSystem.Directories.Binaries, @"NeoAxis.Internal\Platforms" );
			foreach( var folder in Directory.GetDirectories( path ) )
			{
				if( Path.GetFileName( folder ) != Platform.ToString() )
					result.Add( folder );
			}

			return result;
		}

		//[Browsable( false )]
		//public string SourcePlatformFolder
		//{
		//	get { return Path.Combine( VirtualFileSystem.Directories.Binaries, "NeoAxis.Internal\\Platforms", Platform.ToString() ); }
		//}

		public void CopyFiles( string sourceFolder, string destFolder, ProductBuildInstance buildInstance, Range progressRange, string searchPattern )
		{
			Directory.CreateDirectory( destFolder );

			var allFiles = new DirectoryInfo( sourceFolder ).GetFiles( searchPattern, SearchOption.TopDirectoryOnly );

			long totalLength = allFiles.Sum( f => f.Length );

			long processedLength = 0;
			foreach( var fileInfo in allFiles )
			{
				if( File.Exists( fileInfo.FullName ) )
					File.Copy( fileInfo.FullName, fileInfo.FullName.Replace( sourceFolder, destFolder ), true );

				if( buildInstance.RequestCancel )
				{
					buildInstance.State = ProductBuildInstance.StateEnum.Cancelled;
					return;
				}

				processedLength += fileInfo.Length;
				buildInstance.SetProgressWithRange( (double)processedLength / (double)totalLength, progressRange );
			}
		}

		public void CopyFolder( string sourceFolder, string destFolder, ProductBuildInstance buildInstance, Range progressRange, IEnumerable<string> excludePaths = null )
		{
			if( !Directory.Exists( sourceFolder ) )
				return;

			Directory.CreateDirectory( destFolder );

			IEnumerable<FileInfo> allFiles = new DirectoryInfo( sourceFolder ).GetFiles( "*.*", SearchOption.AllDirectories ).ToList();
			IEnumerable<string> allDirs = Directory.GetDirectories( sourceFolder, "*", SearchOption.AllDirectories ).ToList();

			// filter if needed.
			if( excludePaths != null )
			{
				//!!!!good? норм если rooted пути в excludePaths
				allFiles = allFiles.Where( file => excludePaths.All( p => !file.FullName.Contains( p ) ) );
				allDirs = allDirs.Where( dir => excludePaths.All( p => !dir.Contains( p ) ) );
			}

			long totalLength = 0;
			foreach( var fileInfo in allFiles )
				totalLength += fileInfo.Length;

			foreach( string dirPath in allDirs )
			{
				if( Directory.Exists( dirPath ) )
					Directory.CreateDirectory( dirPath.Replace( sourceFolder, destFolder ) );
			}

			long processedLength = 0;
			foreach( var fileInfo in allFiles )
			{
				if( File.Exists( fileInfo.FullName ) )
					File.Copy( fileInfo.FullName, fileInfo.FullName.Replace( sourceFolder, destFolder ), true );

				if( buildInstance.RequestCancel )
				{
					buildInstance.State = ProductBuildInstance.StateEnum.Cancelled;
					return;
				}

				processedLength += fileInfo.Length;
				buildInstance.SetProgressWithRange( (double)processedLength / (double)totalLength, progressRange );
			}
		}

		//protected bool BuildCopyFiles( PackageBuildInstance buildInstance, Range progressRange )
		//{
		//	try
		//	{
		//		string sourcePath = VirtualFileSystem.Directories.Executables;

		//		string destBinariesDirectory = Path.Combine( buildInstance.DestinationFolder, "Files" );
		//		Directory.CreateDirectory( destBinariesDirectory );

		//		FileInfo[] allFiles = new DirectoryInfo( sourcePath ).GetFiles( "*.*", SearchOption.AllDirectories );

		//		long totalLength = 0;
		//		foreach( var fileInfo in allFiles )
		//			totalLength += fileInfo.Length;

		//		foreach( string dirPath in Directory.GetDirectories( sourcePath, "*", SearchOption.AllDirectories ) )
		//		{
		//			if( Directory.Exists( dirPath ) )
		//				Directory.CreateDirectory( dirPath.Replace( sourcePath, destBinariesDirectory ) );
		//		}

		//		long processedLength = 0;
		//		foreach( var fileInfo in allFiles )
		//		{
		//			if( File.Exists( fileInfo.FullName ) )
		//				File.Copy( fileInfo.FullName, fileInfo.FullName.Replace( sourcePath, destBinariesDirectory ), false );

		//			if( buildInstance.RequestCancel )
		//			{
		//				buildInstance.State = PackageBuildInstance.StateEnum.Cancelled;
		//				return false;
		//			}

		//			processedLength += fileInfo.Length;

		//			var progress = (double)processedLength / (double)totalLength;
		//			if( progress > 1 )
		//				progress = 1;
		//			var progress2 = progressRange.Minimum + progress * progressRange.Size;
		//			buildInstance.Progress = (float)progress2;
		//			//deployProgressBarValue = (int)( (double)processedLength / (double)totalLength * 100.0 );
		//			//if( deployProgressBarValue > 100 )
		//			//	deployProgressBarValue = 100;
		//		}

		//		//delete not needed files

		//		//!!!! delete Platforms makes sense only for Windows (not for UWP) !
		//		//TODO: need to extract method and use polymorphysm. (virtual/override)
		//		if( Platform != SystemSettings.Platform.UWP )
		//		{
		//			{
		//				//delete from Platforms

		//				string platformName = Platform.ToString();
		//				string platformsPath = Path.Combine( destBinariesDirectory, "NeoAxis.Internal\\Platforms" );

		//				foreach( var directory in Directory.GetDirectories( platformsPath ) )
		//				{
		//					if( Path.GetFileName( directory ) != platformName )
		//						Directory.Delete( directory, true );
		//				}
		//			}
		//		}
		//	}
		//	catch( Exception e )
		//	{
		//		buildInstance.Error = e.Message;
		//		buildInstance.State = PackageBuildInstance.StateEnum.Error;
		//		return false;
		//	}

		//	return true;
		//}

		public abstract void BuildFunction( ProductBuildInstance buildInstance );

		[Browsable( false )]
		public abstract bool SupportsBuildAndRun { get; }

		public bool CheckCancel( ProductBuildInstance buildInstance )
		{
			if( buildInstance.RequestCancel )
				buildInstance.State = ProductBuildInstance.StateEnum.Cancelled;
			return buildInstance.RequestCancel;
		}

		public void ShowSuccessScreenNotification()
		{
			ScreenNotifications.Show( EditorLocalization.Translate( "Backstage", "The product was built successfully." ) );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( ScreenLabel ):
					skip = true;
					break;
				}
			}
		}

		public void CopyIncludeExcludePaths( IEnumerable<string> paths, ProductBuildInstance buildInstance, Range progressRange )
		{
			var includePaths = new List<string>();
			var excludePathsRooted = new List<string>();
			foreach( var path in paths )
			{
				if( !path.Contains( "exclude:" ) )
					includePaths.Add( path );
				else
					excludePathsRooted.Add( Path.Combine( VirtualFileSystem.Directories.Project, path.Replace( "exclude:", "" ) ) );
			}

			if( includePaths.Count != 0 )
			{
				var percentStep = progressRange.Size / includePaths.Count;
				var currentPercent = progressRange.Minimum;

				foreach( var includePath in includePaths )
				{
					var sourcePath = Path.Combine( VirtualFileSystem.Directories.Project, includePath );
					var destPath = Path.Combine( buildInstance.DestinationFolder, includePath );

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
						CopyFolder( sourcePath, destPath, buildInstance, percentRange, excludePathsRooted );

					if( CheckCancel( buildInstance ) )
						return;

					currentPercent += percentStep;
				}
			}
		}

		protected virtual void OnGetPaths( List<string> paths )
		{
			//Paths
			foreach( var path in Paths.Value.Split( '\n', StringSplitOptions.RemoveEmptyEntries ) )
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
		}

		public List<string> GetPaths()
		{
			var paths = new List<string>();
			OnGetPaths( paths );

			//remove rooted paths
			var prefix = VirtualFileSystem.Directories.Project + Path.DirectorySeparatorChar;
			for( int n = 0; n < paths.Count; n++ )
				paths[ n ] = paths[ n ].Replace( prefix, "" );

			return paths;
		}

		[Browsable( false )]
		public virtual bool CanBuildFromThread
		{
			get { return true; }
		}
	}
}
