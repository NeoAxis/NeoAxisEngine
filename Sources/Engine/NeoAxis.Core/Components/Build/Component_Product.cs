// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Represents the product build settings.
	/// </summary>
	[ResourceFileExtension( "product" )]
	[EditorDocumentWindow( typeof( Component_Product_DocumentWindow ) )]
	public abstract class Component_Product : Component
	{
		/// <summary>
		/// The name of the product.
		/// </summary>
		[DefaultValue( "" )]
		[Category( "Product" )]
		public Reference<string> ProductName
		{
			get { if( _productName.BeginGet() ) ProductName = _productName.Get( this ); return _productName.value; }
			set { if( _productName.BeginSet( ref value ) ) { try { ProductNameChanged?.Invoke( this ); } finally { _productName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProductName"/> property value changes.</summary>
		public event Action<Component_Product> ProductNameChanged;
		ReferenceField<string> _productName = "";

		/// <summary>
		/// The value for determining the position of the product in the product list for build.
		/// </summary>
		[DefaultValue( 0 )]
		[Category( "Product" )]
		public Reference<int> SortOrderInEditor
		{
			get { if( _sortOrderInEditor.BeginGet() ) SortOrderInEditor = _sortOrderInEditor.Get( this ); return _sortOrderInEditor.value; }
			set { if( _sortOrderInEditor.BeginSet( ref value ) ) { try { SortOrderInEditorChanged?.Invoke( this ); } finally { _sortOrderInEditor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SortOrderInEditor"/> property value changes.</summary>
		public event Action<Component_Product> SortOrderInEditorChanged;
		ReferenceField<int> _sortOrderInEditor = 0;

		/// <summary>
		/// The name of application executable file.
		/// </summary>
		[DefaultValue( "NeoAxis.Player" )]
		[Category( "Product" )]
		public Reference<string> ExecutableName
		{
			get { if( _executableName.BeginGet() ) ExecutableName = _executableName.Get( this ); return _executableName.value; }
			set { if( _executableName.BeginSet( ref value ) ) { try { ExecutableNameChanged?.Invoke( this ); } finally { _executableName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExecutableName"/> property value changes.</summary>
		public event Action<Component_Product> ExecutableNameChanged;
		ReferenceField<string> _executableName = "NeoAxis.Player";

		/// <summary>
		/// The list of folders to add from Assets folder. It is specified by the list, list items are separated by semicolon. Example: "Base;Samples\Starter Content;Samples\Simple Game". When the value is empty, the entire Assets folder is included.
		/// </summary>
		[DefaultValue( "" )]
		[Category( "Files" )]
		public Reference<string> SelectedAssets
		{
			get { if( _selectedAssets.BeginGet() ) SelectedAssets = _selectedAssets.Get( this ); return _selectedAssets.value; }
			set { if( _selectedAssets.BeginSet( ref value ) ) { try { SelectedAssetsChanged?.Invoke( this ); } finally { _selectedAssets.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SelectedAssets"/> property value changes.</summary>
		public event Action<Component_Product> SelectedAssetsChanged;
		ReferenceField<string> _selectedAssets = "";

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
		public event Action<Component_Product> FileCacheChanged;
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
		public event Action<Component_Product> ShaderCacheChanged;
		ReferenceField<bool> _shaderCache = true;

		////Platform
		//ReferenceField<SystemSettings.Platform> _platform = SystemSettings.Platform.Windows;
		//[DefaultValue( SystemSettings.Platform.Windows )]
		//[Serialize]
		//public Reference<SystemSettings.Platform> Platform
		//{
		//	get
		//	{
		//		if( _platform.BeginGet() )
		//			Platform = _platform.Get( this );
		//		return _platform.value;
		//	}
		//	set
		//	{
		//		if( _platform.BeginSet( ref value ) )
		//		{
		//			try { PlatformChanged?.Invoke( this ); }
		//			finally { _platform.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Deployment> PlatformChanged;

		//!!!!
		//SpecialPlatformFolder

		////DestinationFolder
		//ReferenceField<string> _destinationFolder = "";
		//[DefaultValue( "" )]
		//[Serialize]
		//public Reference<string> DestinationFolder
		//{
		//	get
		//	{
		//		if( _destinationFolder.BeginGet() )
		//			DestinationFolder = _destinationFolder.Get( this );
		//		return _destinationFolder.value;
		//	}
		//	set
		//	{
		//		if( _destinationFolder.BeginSet( ref value ) )
		//		{
		//			try { DestinationFolderChanged?.Invoke( this ); }
		//			finally { _destinationFolder.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Deployment> DestinationFolderChanged;

		//!!!!event AfterCopyFiles
		//!!!!event BeforePackaging

		[Browsable( false )]
		public abstract SystemSettings.Platform Platform
		{
			get;
		}

		public List<string> GetPlatformsExcludePaths()
		{
			var result = new List<string>();

			var path = Path.Combine( VirtualFileSystem.Directories.Binaries, @"NeoAxis.Internal\Platforms" );
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
	}
}
