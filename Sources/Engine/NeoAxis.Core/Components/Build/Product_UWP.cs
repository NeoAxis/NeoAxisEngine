// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using NeoAxis.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents the product build settings for UWP.
	/// </summary>
	public class Product_UWP : Product
	{
		/// <summary>
		/// The package name of the package.
		/// </summary>
		[Category( "Product" )]
		public string PackageName
		{
			get
			{
				// PackageName in manifest must match pattern '[-.A-Za-z0-9]+'

				var result = Name/*ProductName.Value*/.Replace( " ", "" ).Replace( "_", "" );

				////check
				//if( !System.Text.RegularExpressions.Regex.IsMatch( packageName, "^[-.A-Za-z0-9]+$" ) )
				//	throw new Exception( "PackageName must match pattern '[-.A-Za-z0-9]+'" );

				return result;
			}
		}

		[Category( "Universal Windows Platform" )]// and Run" )]
		[DefaultValue( "x64" )]
		public Reference<string> BuildPlatform
		{
			get { if( _buildPlatform.BeginGet() ) BuildPlatform = _buildPlatform.Get( this ); return _buildPlatform.value; }
			set { if( _buildPlatform.BeginSet( this, ref value ) ) { try { BuildPlatformChanged?.Invoke( this ); } finally { _buildPlatform.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BuildPlatform"/> property value changes.</summary>
		public event Action<Product_UWP> BuildPlatformChanged;
		ReferenceField<string> _buildPlatform = "x64";

		[Category( "Universal Windows Platform" )]
		[DefaultValue( true )]
		public Reference<bool> PatchProjectFiles
		{
			get { if( _patchProjectFiles.BeginGet() ) PatchProjectFiles = _patchProjectFiles.Get( this ); return _patchProjectFiles.value; }
			set { if( _patchProjectFiles.BeginSet( this, ref value ) ) { try { PatchProjectFilesChanged?.Invoke( this ); } finally { _patchProjectFiles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PatchProjectFiles"/> property value changes.</summary>
		public event Action<Product_UWP> PatchProjectFilesChanged;
		ReferenceField<bool> _patchProjectFiles = true;

		/// <summary>
		/// The displayable name of the package.
		/// </summary>
		[Category( "Universal Windows Platform" )]
		[DefaultValue( "NeoAxis.Player" )]
		[Serialize]
		public Reference<string> PackageDisplayName
		{
			get { if( _packageDisplayName.BeginGet() ) PackageDisplayName = _packageDisplayName.Get( this ); return _packageDisplayName.value; }
			set { if( _packageDisplayName.BeginSet( this, ref value ) ) { try { PackageDisplayNameChanged?.Invoke( this ); } finally { _packageDisplayName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PackageDisplayName"/> property value changes.</summary>
		public event Action<Product_UWP> PackageDisplayNameChanged;
		ReferenceField<string> _packageDisplayName = "NeoAxis.Player";

		/// <summary>
		/// The version of the product package.
		/// </summary>
		[Category( "Universal Windows Platform" )]
		[DefaultValue( "1.0.0.0" )]
		[Serialize]
		public Reference<string> PackageVersion
		{
			get { if( _packageVersion.BeginGet() ) PackageVersion = _packageVersion.Get( this ); return _packageVersion.value; }
			set { if( _packageVersion.BeginSet( this, ref value ) ) { try { PackageVersionChanged?.Invoke( this ); } finally { _packageVersion.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PackageVersion"/> property value changes.</summary>
		public event Action<Product_UWP> PackageVersionChanged;
		ReferenceField<string> _packageVersion = "1.0.0.0";

		/// <summary>
		/// The name of the package publisher.
		/// </summary>
		[Category( "Universal Windows Platform" )]
		[DefaultValue( "CN=DefaultPublisher" )]
		public Reference<string> PackagePublisher
		{
			get { if( _packagePublisher.BeginGet() ) PackagePublisher = _packagePublisher.Get( this ); return _packagePublisher.value; }
			set { if( _packagePublisher.BeginSet( this, ref value ) ) { try { PackagePublisherChanged?.Invoke( this ); } finally { _packagePublisher.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PackagePublisher"/> property value changes.</summary>
		public event Action<Product_UWP> PackagePublisherChanged;
		ReferenceField<string> _packagePublisher = "CN=DefaultPublisher";

		////!!!!странное свойство, меняется из кода
		////TODO: make prop read only in grid !
		///// <summary>
		///// The name of the package publisher.
		///// </summary>
		//[Category( "Product" )]
		////[DisplayName( "Publisher" )]
		//[ReadOnly( true )]
		//public string PackagePublisher { get; private set; }

		///// <summary>
		///// The package publisher certificate file.
		///// </summary>
		//[Category( "Build" )]
		////[DisplayName( "PublisherCertificate" )]
		//[DefaultValue( "" )]
		//[Serialize]
		//public Reference<string> PublisherCertificateFileOverride
		//{
		//	get { if( _publisherCertificateFileOverride.BeginGet() ) PublisherCertificateFileOverride = _publisherCertificateFileOverride.Get( this ); return _publisherCertificateFileOverride.value; }
		//	set { if( _publisherCertificateFileOverride.BeginSet( this, ref value ) ) { try { PublisherCertificateFileOverrideChanged?.Invoke( this ); } finally { _publisherCertificateFileOverride.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PublisherCertificateFileOverride"/> property value changes.</summary>
		//public event Action<Product_UWP> PublisherCertificateFileOverrideChanged;
		////TODO: need file selector.
		//ReferenceField<string> _publisherCertificateFileOverride = "";

		//string GetPublisherCertificateFile()
		//{
		//	string publisherCertificateFileDefault = @"ProjectTemplate\ProductName\TemporaryKey.pfx";

		//	return string.IsNullOrEmpty( PublisherCertificateFileOverride ) ? publisherCertificateFileDefault : PublisherCertificateFileOverride.Value;
		//}

		/// <summary>
		/// Displayed publisher name.
		/// </summary>
		[Category( "Universal Windows Platform" )]
		//[DisplayName( "PublisherDisplayName" )]
		[DefaultValue( "Default Publisher" )]
		[Serialize]
		public Reference<string> PublisherDisplayName
		{
			get { if( _publisherDisplayName.BeginGet() ) PublisherDisplayName = _publisherDisplayName.Get( this ); return _publisherDisplayName.value; }
			set { if( _publisherDisplayName.BeginSet( this, ref value ) ) { try { PublisherDisplayNameChanged?.Invoke( this ); } finally { _publisherDisplayName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PublisherDisplayName"/> property value changes.</summary>
		public event Action<Product_UWP> PublisherDisplayNameChanged;
		ReferenceField<string> _publisherDisplayName = "Default Publisher";

		///// <summary>
		///// Whether to create Appx package.
		///// </summary>
		//[Category( "Build" )]
		//[DefaultValue( false )]
		//[Serialize]
		//public Reference<bool> CreateAppxPackage
		//{
		//	get { if( _createAppxPackage.BeginGet() ) CreateAppxPackage = _createAppxPackage.Get( this ); return _createAppxPackage.value; }
		//	set { if( _createAppxPackage.BeginSet( this, ref value ) ) { try { CreateAppxPackageChanged?.Invoke( this ); } finally { _createAppxPackage.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CreateAppxPackage"/> property value changes.</summary>
		//public event Action<Product_UWP> CreateAppxPackageChanged;
		//ReferenceField<bool> _createAppxPackage = false;

		///// <summary>
		///// Whether to create Appx bundle.
		///// </summary>
		//[Category( "Build" )]
		//[DefaultValue( "Always" )]
		//[Serialize]
		//[Browsable( false )]
		//public string CreateAppxBundle { get; } = "Always";

		/////////////////////////////////////////
		// app ui (windows Start menu, main window caption)

		[Category( "Universal Windows Platform" )]
		//[DisplayName( "DisplayName" )]
		[DefaultValue( "NeoAxis Player" )]
		[Serialize]
		public Reference<string> AppDisplayName
		{
			get { if( _appDisplayName.BeginGet() ) AppDisplayName = _appDisplayName.Get( this ); return _appDisplayName.value; }
			set { if( _appDisplayName.BeginSet( this, ref value ) ) { try { AppDisplayNameChanged?.Invoke( this ); } finally { _appDisplayName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AppDisplayName"/> property value changes.</summary>
		public event Action<Product_UWP> AppDisplayNameChanged;
		ReferenceField<string> _appDisplayName = "NeoAxis Player";

		/// <summary>
		/// The description of the product.
		/// </summary>
		[Category( "Universal Windows Platform" )]
		//[DisplayName( "Description" )]
		[DefaultValue( "NeoAxis Application" )]
		[Serialize]
		public Reference<string> AppDescription
		{
			get { if( _appDescription.BeginGet() ) AppDescription = _appDescription.Get( this ); return _appDescription.value; }
			set
			{
				//if( string.IsNullOrWhiteSpace( value ) )
				//{
				//	value = Name;//ProductName;
				//				 //throw new Exception( "Description must not begin or end with whitespace" );
				//}
				if( _appDescription.BeginSet( this, ref value ) ) { try { AppDescriptionChanged?.Invoke( this ); } finally { _appDescription.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="AppDescription"/> property value changes.</summary>
		public event Action<Product_UWP> AppDescriptionChanged;
		ReferenceField<string> _appDescription = "NeoAxis Application";

		/// <summary>
		/// The application identifier.
		/// </summary>
		[Category( "Universal Windows Platform" )]
		[DefaultValue( "App" )]
		public Reference<string> ApplicationId
		{
			get { if( _applicationId.BeginGet() ) ApplicationId = _applicationId.Get( this ); return _applicationId.value; }
			set { if( _applicationId.BeginSet( this, ref value ) ) { try { ApplicationIdChanged?.Invoke( this ); } finally { _applicationId.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ApplicationId"/> property value changes.</summary>
		public event Action<Product_UWP> ApplicationIdChanged;
		ReferenceField<string> _applicationId = "App";

		/// <summary>
		/// The entry point of the product.
		/// </summary>
		[Category( "Universal Windows Platform" )]
		[DefaultValue( "NeoAxis.Player.App" )]
		public Reference<string> EntryPoint
		{
			get { if( _entryPoint.BeginGet() ) EntryPoint = _entryPoint.Get( this ); return _entryPoint.value; }
			set { if( _entryPoint.BeginSet( this, ref value ) ) { try { EntryPointChanged?.Invoke( this ); } finally { _entryPoint.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EntryPoint"/> property value changes.</summary>
		public event Action<Product_UWP> EntryPointChanged;
		ReferenceField<string> _entryPoint = "NeoAxis.Player.App";

		///// <summary>
		///// The background color of the application.
		///// </summary>
		//[Category( "Application" )]
		//[DefaultValue( "transparent" )]
		//public Reference<string> BackgroundColor
		//{
		//	get { if( _backgroundColor.BeginGet() ) BackgroundColor = _backgroundColor.Get( this ); return _backgroundColor.value; }
		//	set { if( _backgroundColor.BeginSet( this, ref value ) ) { try { BackgroundColorChanged?.Invoke( this ); } finally { _backgroundColor.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BackgroundColor"/> property value changes.</summary>
		//public event Action<Product_UWP> BackgroundColorChanged;
		//ReferenceField<string> _backgroundColor = "transparent";

		//[Category( "Application" )]
		//public Reference<string> RotationPreference { get; } = ""; //TODO: implement this.

		//[Category( "Application" )]
		//[DefaultValue( "" )]
		//public Reference<string> Capabilities
		//{
		//	get { if( _capabilities.BeginGet() ) Capabilities = _capabilities.Get( this ); return _capabilities.value; }
		//	set { if( _capabilities.BeginSet( this, ref value ) ) { try { CapabilitiesChanged?.Invoke( this ); } finally { _capabilities.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Capabilities"/> property value changes.</summary>
		//public event Action<Product_UWP> CapabilitiesChanged;
		//ReferenceField<string> _capabilities = "";

		//[Category( "ContentUris" )]
		//[Browsable( false )]
		//public Reference<string> ApplicationContentUriRules { get; set; } = ""; //TODO: implement this.

		/////////////////////////////////////////
		// visual assets

		///// <summary>
		///// When used, overrides Assets folder path.
		///// </summary>
		//[DefaultValue( "" )]
		//[Category( "Assets" )]
		////[DisplayName( "AssetsFolder" )]
		//[Serialize]
		//public Reference<string> AssetsFolderOverride
		//{
		//	get { if( _assetsFolderOverride.BeginGet() ) AssetsFolderOverride = _assetsFolderOverride.Get( this ); return _assetsFolderOverride.value; }
		//	set { if( _assetsFolderOverride.BeginSet( this, ref value ) ) { try { AssetsFolderChanged?.Invoke( this ); } finally { _assetsFolderOverride.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AssetsFolder"/> property value changes.</summary>
		//public event Action<Product_UWP> AssetsFolderChanged;
		//ReferenceField<string> _assetsFolderOverride = "";
		//string _assetsFolderDefault = @"ProjectTemplate\ProductName\Assets";

		//string GetAssetsFolder()
		//{
		//	return string.IsNullOrEmpty( AssetsFolderOverride ) ? _assetsFolderDefault : AssetsFolderOverride.Value;
		//}

		/////////////////////////////////////////
		// build and run

		//[Category( "Build and Run" )]
		//[DefaultValue( "Release" )]
		//public Reference<string> BuildConfiguration
		//{
		//	get { if( _buildConfiguration.BeginGet() ) BuildConfiguration = _buildConfiguration.Get( this ); return _buildConfiguration.value; }
		//	set { if( _buildConfiguration.BeginSet( this, ref value ) ) { try { BuildConfigurationChanged?.Invoke( this ); } finally { _buildConfiguration.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BuildConfiguration"/> property value changes.</summary>
		//public event Action<Product_UWP> BuildConfigurationChanged;
		//ReferenceField<string> _buildConfiguration = "Release";

		//[Category( "Build and Run" )]
		//public string BuildConfiguration { get; } = "Debug";//TODO: implement this.

		//!!!!does not work
		//[Category( "Build and Run" )]
		//public bool RunOnLocalMachine { get; } = true; //TODO: implement this.

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( PackageDisplayName ):
				case nameof( PackageVersion ):
				case nameof( PackagePublisher ):
				case nameof( PublisherDisplayName ):
				case nameof( AppDisplayName ):
				case nameof( AppDescription ):
				case nameof( ApplicationId ):
				case nameof( EntryPoint ):
					if( !PatchProjectFiles )
						skip = true;
					break;
				}
			}
		}

		public override SystemSettings.Platform Platform
		{
			get { return SystemSettings.Platform.UWP; }
		}

		[Browsable( false )]
		public override bool SupportsBuildAndRun
		{
			get { return false; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//OnPublisherCertificateChanged();
		}

		public override void BuildFunction( ProductBuildInstance buildInstance )
		{
			// To build UWP app, Universal Windows SDK should be installed.
			// To register app Visual Studio should be installed.
			//TODO: check this before build / deploy

			try
			{
				PatchCSharpProjects( buildInstance );

				CopyFilesToPackageFolder( buildInstance );
				buildInstance.Progress = 0.8f;

				if( CheckCancel( buildInstance ) )
					return;

				if( PatchProjectFiles )
					DoPatchProjectFiles( buildInstance );
				buildInstance.Progress = 0.9f;

				if( CheckCancel( buildInstance ) )
					return;

				//!!!!does not work
				//if( buildInstance.Run )
				//{
				//	if( !BuildProject( buildInstance.DestinationFolder ) )
				//	{
				//		// exit if building failed.
				//		//TODO: pass error messsage to this point.
				//		buildInstance.Error = "Project building failed. See details in log.";
				//		buildInstance.State = ProductBuildInstance.StateEnum.Error;
				//		return;
				//	}

				//	buildInstance.Progress = 0.7f;

				//	if( CheckCancel( buildInstance ) ) return;

				//	RegisterLayout( buildInstance.DestinationFolder );

				//	if( CheckCancel( buildInstance ) ) return;
				//}

			}
			catch( Exception e )
			{
				buildInstance.Error = e.Message;
				buildInstance.State = ProductBuildInstance.StateEnum.Error;
				return;
			}

			//post build event
			if( !PeformPostBuild( buildInstance ) )
				return;
			if( CheckCancel( buildInstance ) )
				return;

			//done
			buildInstance.Progress = 1;
			buildInstance.State = ProductBuildInstance.StateEnum.Success;

			//!!!!does not work
			//try
			//{
			//	if( buildInstance.Run )
			//		Run();
			//}
			//catch( Exception e )
			//{
			//	Log.Warning( "Application run failed: " + e.Message ); // or Error ?
			//}

			if( CheckCancel( buildInstance ) )
				return;

			ShowSuccessScreenNotification();
		}

		//string GetComponentFolder()
		//{
		//	var realPath = VirtualPathUtility.GetRealPathByVirtual( HierarchyController.CreatedByResource.Owner.Name );
		//	return Path.GetDirectoryName( realPath );
		//	//var path = HierarchyController.CreatedByResource.Owner.Name;
		//	//var fullPath = Path.Combine( VirtualFileSystem.Directories.ProjectData, path );
		//	//return Path.GetDirectoryName( fullPath );
		//}

		//string GetTemplateFolder()
		//{
		//	return Path.Combine( GetComponentFolder(), "ProjectTemplate" );
		//}

		void PatchCSharpProjects( ProductBuildInstance buildInstance )
		{
			{
				var p1 = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.UWP.csproj" );
				var p2 = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.csproj" );
				if( !EditorAPI.EditorCommandLineTools_PlatformProjectPatch_Process( p1, p2, out var error, out _ ) )
					throw new Exception( error );
			}

			{
				var p1 = Path.Combine( VirtualFileSystem.Directories.Project, @"Project.UWP.csproj" );
				var p2 = Path.Combine( VirtualFileSystem.Directories.Project, @"Project.csproj" );
				if( !EditorAPI.EditorCommandLineTools_PlatformProjectPatch_Process( p1, p2, out var error, out _ ) )
					throw new Exception( error );
			}
		}

		void CopyFilesToPackageFolder( ProductBuildInstance buildInstance )
		{
			//copy files
			try
			{
				var paths = GetPaths();

				//var paths = new List<string>();

				//paths.Add( "Caches" );

				////Caches
				//if( !ShaderCache )
				//	paths.Add( @"exclude:Caches\ShaderCache" );
				//if( !FileCache )
				//	paths.Add( @"exclude:Caches\Files" );

				////Paths
				//foreach( var path in Paths.Value.Split( '\n', StringSplitOptions.RemoveEmptyEntries ) )
				//{
				//	var path2 = path.Replace( "\r", "" ).Trim();
				//	if( path2 != "" )
				//		paths.Add( path2 );
				//}

				////remove rooted paths
				//for( int n = 0; n < paths.Count; n++ )
				//	paths[ n ] = paths[ n ].Replace( VirtualFileSystem.Directories.Project + Path.DirectorySeparatorChar, "" );

				//execute
				CopyIncludeExcludePaths( paths, buildInstance, new Range( 0, 0.4 ) );
			}
			catch( Exception e )
			{
				buildInstance.Error = e.Message;
				buildInstance.State = ProductBuildInstance.StateEnum.Error;
				return;
			}

			////copy Assets
			//{
			//	string sourceAssetsPath = VirtualFileSystem.Directories.Assets;
			//	string destAssetsPath = Path.Combine( buildInstance.DestinationFolder, "Assets" );

			//	var assetsExcludePaths = new List<string>();
			//	{
			//		assetsExcludePaths.Add( Path.Combine( sourceAssetsPath, @"Base\Tools" ) );
			//		assetsExcludePaths.Add( Path.Combine( sourceAssetsPath, @"Base\Learning" ) );

			//		var excluded = ExcludedAssets.Value.Trim();
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

			//	var values = Paths.Value.Trim();
			//	if( !string.IsNullOrEmpty( values ) )
			//	{
			//		foreach( var v in values.Split( new char[] { ';', '\r', '\n' } ) )
			//		{
			//			var v2 = v.Trim( ' ', '\t' );
			//			if( !string.IsNullOrEmpty( v2 ) )
			//			{
			//				var sourceFolder2 = Path.Combine( VirtualFileSystem.Directories.Project, "Assets", v2 );
			//				var destFolder2 = Path.Combine( buildInstance.DestinationFolder, "Assets", v2 );
			//				var percentRange2 = new Range( 0.0, 0.0 );

			//				CopyFolder( sourceFolder2, destFolder2, buildInstance, percentRange2, assetsExcludePaths );

			//				if( CheckCancel( buildInstance ) )
			//					return;
			//			}
			//		}

			//		skipDefaultBehavior = true;
			//	}

			//	if( !skipDefaultBehavior )
			//		CopyFolder( VirtualFileSystem.Directories.Assets, destAssetsPath, buildInstance, new Range( 0.0, 0.3 ), assetsExcludePaths );
			//}

			////copy Caches
			//{
			//	string sourceCachesPath = Path.Combine( VirtualFileSystem.Directories.Project, "Caches" );
			//	string destCachesPath = Path.Combine( buildInstance.DestinationFolder, "Caches" );

			//	var excludePaths = new List<string>();
			//	if( !ShaderCache )
			//		excludePaths.Add( @"Caches\ShaderCache" );
			//	if( !FileCache )
			//		excludePaths.Add( @"Caches\Files" );

			//	CopyFolder( sourceCachesPath, destCachesPath, buildInstance, new Range( 0.3, 0.4 ), excludePaths );

			//	if( CheckCancel( buildInstance ) )
			//		return;
			//}

			//copy Build.UWP.sln or Build.UWP.Extended.sln
			if( File.Exists( Path.Combine( VirtualFileSystem.Directories.Project, "Build.UWP.Extended.sln" ) ) )
			{
				CopyFiles( VirtualFileSystem.Directories.Project, buildInstance.DestinationFolder, buildInstance, new Range( 0.4, 0.4 ), "Build.UWP.Extended.sln" );
			}
			else
			{
				CopyFiles( VirtualFileSystem.Directories.Project, buildInstance.DestinationFolder, buildInstance, new Range( 0.4, 0.4 ), "Build.UWP.sln" );
			}

			//copy Project.UWP.csproj
			CopyFiles( VirtualFileSystem.Directories.Project, buildInstance.DestinationFolder, buildInstance, new Range( 0.4, 0.4 ), "Project.UWP.csproj" );

			//copy Properties
			CopyFolder( Path.Combine( VirtualFileSystem.Directories.Project, "Properties" ), Path.Combine( buildInstance.DestinationFolder, "Properties" ), buildInstance, new Range( 0.4, 0.4 ) );

			//copy part of Sources
			var sourceSourcesPath = Path.Combine( VirtualFileSystem.Directories.Project, "Sources" );
			string destSourcesPath = Path.Combine( buildInstance.DestinationFolder, "Sources" );

			//copy Sources\NeoAxis.Player.UWP
			CopyFolder( Path.Combine( sourceSourcesPath, "NeoAxis.Player.UWP" ), Path.Combine( destSourcesPath, "NeoAxis.Player.UWP" ), buildInstance, new Range( 0.4, 0.45 ) );
			//copy Sources\NeoAxis.CoreExtension
			CopyFolder( Path.Combine( sourceSourcesPath, "NeoAxis.CoreExtension" ), Path.Combine( destSourcesPath, "NeoAxis.CoreExtension" ), buildInstance, new Range( 0.45, 0.5 ) );

			var sourceBinariesPath = VirtualFileSystem.Directories.Binaries;
			string destBinariesPath = Path.Combine( buildInstance.DestinationFolder, "Binaries" );

			var sourcePlatformFolder = Path.Combine( sourceBinariesPath, "NeoAxis.Internal\\Platforms", Platform.ToString() );
			var destPlatformFolder = Path.Combine( destBinariesPath, "NeoAxis.Internal\\Platforms", Platform.ToString() );

			//copy managed dll references from original folder
			CopyFiles( VirtualFileSystem.Directories.Binaries, destBinariesPath, buildInstance, new Range( 0.5, 0.6 ), "*.dll" );
			//copy NeoAxis.DefaultSettings.config
			Directory.CreateDirectory( Path.Combine( destBinariesPath, "NeoAxis.Internal" ) );
			File.Copy(
				Path.Combine( VirtualFileSystem.Directories.Binaries, "NeoAxis.Internal", "NeoAxis.DefaultSettings.config" ),
				Path.Combine( destBinariesPath, "NeoAxis.Internal", "NeoAxis.DefaultSettings.config" ) );

			//!!!!unnecessary dlls are copied? we need a list of references?
			//copy managed dll references from UWP folder
			CopyFiles(
				Path.Combine( sourcePlatformFolder, "Managed" ),
				Path.Combine( destPlatformFolder, "Managed" ), buildInstance, new Range( 0.6, 0.7 ), "*.dll" );

			if( CheckCancel( buildInstance ) )
				return;

			//copy native dlls
			CopyFolder(
				Path.Combine( sourcePlatformFolder, BuildPlatform ),
				Path.Combine( destPlatformFolder, BuildPlatform ), buildInstance, new Range( 0.7, 0.8 ) );


			////copy assets
			//if( !string.IsNullOrEmpty( AssetsFolderOverride ) )
			//{
			//	CopyFolder( Path.Combine( GetComponentFolder(), AssetsFolderOverride ), Path.Combine( buildInstance.ProductFolder, "Assets" ), buildInstance, new Range( 0.6, 0.7 ) );
			//}

			//TODO: need to more carefully prepare a Data folder for deploy.
			//if( Directory.Exists( Path.Combine( destAssetsPath, "obj" ) ) )
			//	Directory.Delete( Path.Combine( destAssetsPath, "obj" ), true );
			//if( Directory.Exists( Path.Combine( destAssetsPath, "Base\\Build" ) ) )
			//	Directory.Delete( Path.Combine( destAssetsPath, "Base\\Build" ), true );
		}

		void DoPatchProjectFiles( ProductBuildInstance buildInstance )
		{
			string destSourcesPath = Path.Combine( buildInstance.DestinationFolder, "Sources" );

			PatchManifestFile( Path.Combine( destSourcesPath, "NeoAxis.Player.UWP\\Package.appxmanifest" ) );
			PatchCSProjFile( Path.Combine( destSourcesPath, "NeoAxis.Player.UWP\\NeoAxis.Player.UWP.csproj" ) );
			PatchAssemblyInfoFile( Path.Combine( destSourcesPath, "NeoAxis.Player.UWP\\Properties\\AssemblyInfo.cs" ) );


			//CopyFolder( GetTemplateFolder(), buildInstance.DestinationFolder, buildInstance, new Range( 0, 0.2 ) );

			//File.Move(
			//	Path.Combine( buildInstance.DestinationFolder, "ProductName.sln" ),
			//	Path.Combine( buildInstance.DestinationFolder, ProductName + ".sln" ) );

			//Directory.Move( Path.Combine( buildInstance.DestinationFolder, "ProductName" ), buildInstance.ProductFolder );

			//File.Move(
			//	Path.Combine( buildInstance.ProductFolder, "ProductName.csproj" ),
			//	Path.Combine( buildInstance.ProductFolder, ProductName + ".csproj" ) );

			//PatchSolutionFile( Path.Combine( buildInstance.DestinationFolder, ProductName + ".sln" ) );
		}

		// The package manifest is an XML document that contains the info the system needs to deploy, display, or update a Windows app.
		// https://docs.microsoft.com/en-us/uwp/schemas/appxpackage/appx-package-manifest
		void PatchManifestFile( string manifestPath )
		{
			string data = File.ReadAllText( manifestPath );

			var appDescription = AppDescription.Value;
			if( string.IsNullOrWhiteSpace( appDescription ) )
				appDescription = Name;

			data = data
				.Replace( "Name=\"NeoAxis.Player\"", "Name=\"" + PackageName + "\"" )
				.Replace( "Publisher=\"CN=DefaultPublisher\"", "Publisher=\"" + PackagePublisher + "\"" )
				.Replace( "Version=\"1.0.0.0\"", "Version=\"" + PackageVersion + "\"" )
				.Replace( "<DisplayName>NeoAxis.Player</DisplayName>", "<DisplayName>" + PackageDisplayName + "</DisplayName>" )
				.Replace( "<PublisherDisplayName>Default Publisher</PublisherDisplayName>", "<PublisherDisplayName>" + PublisherDisplayName + "</PublisherDisplayName>" )
				.Replace( "Id=\"App\"", "Id=\"" + ApplicationId + "\"" )
				//.Replace( "{Executable}", ExecutableName + ".exe" )
				.Replace( "EntryPoint=\"NeoAxis.Player.App\"", "EntryPoint=\"" + EntryPoint + "\"" )
				//.Replace( "{BackgroundColor}", BackgroundColor )
				.Replace( "DisplayName=\"NeoAxis Player\"", "DisplayName=\"" + AppDisplayName + "\"" )
				.Replace( "Description=\"NeoAxis Application\"", "Description=\"" + appDescription + "\"" );

			//data = data
			//	.Replace( "{PackageName}", PackageName )
			//	.Replace( "{PackagePublisher}", PackagePublisher )
			//	.Replace( "{PackageVersion}", PackageVersion )
			//	.Replace( "{PackageDisplayName}", PackageDisplayName )
			//	.Replace( "{PublisherDisplayName}", PublisherDisplayName )
			//	.Replace( "{ApplicationId}", ApplicationId )
			//	.Replace( "{Executable}", ExecutableName + ".exe" )
			//	.Replace( "{EntryPoint}", EntryPoint )
			//	.Replace( "{BackgroundColor}", BackgroundColor )
			//	.Replace( "{AppDisplayName}", AppDisplayName )
			//	.Replace( "{AppDescription}", AppDescription )
			//	.Replace( "{Capabilities}", Capabilities );

			File.WriteAllText( manifestPath, data );
		}

		//void PatchSolutionFile( string path )
		//{
		//	string data = File.ReadAllText( path );
		//	data = data.Replace( "{ProductName}", ProductName );
		//	File.WriteAllText( path, data );
		//}

		void PatchCSProjFile( string path )
		{
			string data = File.ReadAllText( path );
			data = data.Replace( "<AssemblyName>NeoAxis.Player</AssemblyName>", "<AssemblyName>" + Name/*ProductName*/ + "</AssemblyName>" );
			File.WriteAllText( path, data );
		}

		void PatchAssemblyInfoFile( string path )
		{
			string data = File.ReadAllText( path );
			data = data.Replace( "NeoAxis.Player.UWP", Name );// ProductName );
			File.WriteAllText( path, data );
		}


		//bool BuildProject( string destFolder )
		//{
		//	var config = new VisualStudioSolutionUtility.BuildConfig()
		//	{
		//		BuildPlatform = BuildPlatform,
		//		BuildConfiguration = BuildConfiguration,
		//		AppxPackageSigningEnabled = true,
		//		CreateAppxPackage = CreateAppxPackage,
		//		CreateAppxBundle = CreateAppxBundle,
		//		AppxBundlePlatforms = "x64",
		//		OutputAssemblyName = ProductName,
		//		OutDir = string.Empty
		//	};

		//	//!!!!?
		//	var success = VisualStudioSolutionUtility.RestoreNuGetPackets( destFolder, ProductName, config );
		//	if( !success )
		//	{
		//		//TODO: should we try to build if !success ?
		//	}

		//	return VisualStudioSolutionUtility.BuildSolution( destFolder, ProductName, config, false );
		//}

		//void RegisterLayout( string destFolder )
		//{
		//	RegisterLayout( destFolder, ProductName, BuildConfiguration, BuildPlatform );
		//}

		//!!!!does not work
		//void Run()
		//{
		//	if( RunOnLocalMachine )
		//		RunAppOnLocalMachine();
		//	else
		//		throw new NotImplementedException();
		//}

		//!!!!does not work
		//void RunAppOnLocalMachine()
		//{
		//	string familyName = UWPPackageHelper.GetPackageFamilyName( PackageName, PackagePublisher );
		//	string appUserModelId = familyName + "!" + ApplicationId;

		//	Process.Start( @"shell:appsFolder\" + appUserModelId, "" );

		//	// alt way:
		//	//var appActiveManager = new ApplicationActivationManager();
		//	//appActiveManager.ActivateApplication( appUserModelId, null, ActivateOptionsEnum.NoErrorUI, out uint pid );
		//}

		/* 
		 * Alternative way:
		 * 
		 * please add this two references to project:
		 * 
			<Reference Include="System.Runtime.WindowsRuntime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL">
			  <SpecificVersion>False</SpecificVersion>
			  <HintPath>..\..\..\..\..\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll</HintPath>
			</Reference>
			<Reference Include="Windows, Version=255.255.255.255, Culture=neutral, processorArchitecture=MSIL">
			  <SpecificVersion>False</SpecificVersion>
			  <HintPath>..\..\..\..\..\Program Files (x86)\Windows Kits\10\UnionMetadata\Facade\Windows.WinMD</HintPath>
			</Reference>

			async Task<bool> RunAppAsync( string packageFamilyName )
			{
				var packageManager = new PackageManager();

				var package = packageManager.FindPackagesForUser( "", packageFamilyName ).FirstOrDefault();

			    //alt:
				//var package = packageManager.FindPackages( "NeoAxis.Player", "CN=nviko" ).FirstOrDefault();

				if( package == null )
				{
					Log.Warning( "Package with name '" + packageFamilyName + "' not found !" );
					return false;
				}

				var entries = await package.GetAppListEntriesAsync();
				var entry = entries.FirstOrDefault();
				return await entry.LaunchAsync();
			}

		*/

		//void OnPublisherCertificateChanged()
		//{
		//	try
		//	{
		//		string certPath = Path.Combine( GetComponentFolder(), GetPublisherCertificateFile() );
		//		var cert = CertificateHelper.Load( certPath );

		//		PackagePublisher = cert.Issuer;
		//	}

		//	catch( DirectoryNotFoundException )
		//	{
		//		Log.Error( "Error: The directory specified could not be found." );
		//	}
		//	catch( IOException )
		//	{
		//		Log.Error( "Error: A file in the directory could not be accessed." );
		//	}
		//	catch( NullReferenceException )
		//	{
		//		Log.Error( "File must be a .cer ot .pfx file. Program does not have access to that type of file." );
		//	}
		//}

		//// visual studio preinstalled required.
		//static void RegisterLayout( string destFolder, string productName, string buildConfiguration, string buildPlatform )
		//{
		//	//const bool DebugLogFile = false;

		//	var vars = new Dictionary<string, string>() { { "VsPromptUninstallNonVsPackage", "1" } };
		//	var deploy = new StringBuilder();
		//	deploy.Append( $"\"{destFolder}\\{productName}.sln\" /deploy" );
		//	deploy.Append( $" \"{buildConfiguration}|{buildPlatform}\"" );
		//	//if( DebugLogFile )
		//	//	deploy.Append( $" /out \"{destFolder}\\devenv.log\"" );
		//	//TODO: use async?
		//	string result = ExecuteVisualStudio( deploy.ToString(), vars );
		//	Log.Info( result );
		//}

		//		static string ExecuteVisualStudio( string args, IDictionary<string, string> environmentVariables )
		//		{
		//			//!!!!
		//#if !W__!!__INDOWS_UWP
		//			string devEnvExePath = Path.Combine( UWPVisualStudioTools.GetVisualStudioInstallPath(), "devenv.exe" );

		//			if( ProcessUtility.RunAndWait( devEnvExePath, args, out string result, environmentVariables ) != 0 )
		//			{
		//				throw new Exception( string.Format( "Failed to build Visual Studio project using arguments '{0} {1}'.\nOutput:{2}\n", devEnvExePath, args, result ) );
		//			}
		//			return result;

		//			//!!!!
		//#else
		//			return "";
		//#endif
		//		}

		protected override void OnGetPaths( List<string> paths )
		{
			base.OnGetPaths( paths );
			////Paths
			//foreach( var path in Paths.Value.Split( '\n', StringSplitOptions.RemoveEmptyEntries ) )
			//{
			//	var path2 = path.Replace( "\r", "" ).Trim();
			//	if( path2 != "" )
			//		paths.Add( path2 );
			//}

			//Caches
			paths.Add( "Caches" );
			if( !ShaderCache )
				paths.Add( @"exclude:Caches\ShaderCache" );
			if( !FileCache )
				paths.Add( @"exclude:Caches\Files" );
		}
	}
}
#endif