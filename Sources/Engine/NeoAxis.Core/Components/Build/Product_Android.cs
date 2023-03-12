// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using NeoAxis.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Represents the product build settings for Android.
	/// </summary>
	public class Product_Android : Product
	{
		static readonly DateTime setTimeToFilesInZip = new DateTime( 2001, 1, 1, 1, 1, 1 );

		///// <summary>
		///// The package name of the package.
		///// </summary>
		//[Category( "Product" )]
		//public string PackageName
		//{
		//	get
		//	{
		//		// PackageName in manifest must match pattern '[-.A-Za-z0-9]+'

		//		var result = ProductName.Value.Replace( " ", "" ).Replace( "_", "" );

		//		////check
		//		//if( !System.Text.RegularExpressions.Regex.IsMatch( packageName, "^[-.A-Za-z0-9]+$" ) )
		//		//	throw new Exception( "PackageName must match pattern '[-.A-Za-z0-9]+'" );

		//		return result;
		//	}
		//}

		//[Category( "Android" )]// and Run" )]
		//[DefaultValue( "x64" )]
		//public Reference<string> BuildPlatform
		//{
		//	get { if( _buildPlatform.BeginGet() ) BuildPlatform = _buildPlatform.Get( this ); return _buildPlatform.value; }
		//	set { if( _buildPlatform.BeginSet( ref value ) ) { try { BuildPlatformChanged?.Invoke( this ); } finally { _buildPlatform.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BuildPlatform"/> property value changes.</summary>
		//public event Action<Product_Android> BuildPlatformChanged;
		//ReferenceField<string> _buildPlatform = "x64";

		/// <summary>
		/// Whether to compress Zip archive with the project data.
		/// </summary>
		[Category( "Android" )]
		[DefaultValue( true )]
		public Reference<bool> CompressData
		{
			get { if( _compressData.BeginGet() ) CompressData = _compressData.Get( this ); return _compressData.value; }
			set { if( _compressData.BeginSet( ref value ) ) { try { CompressDataChanged?.Invoke( this ); } finally { _compressData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CompressData"/> property value changes.</summary>
		public event Action<Product_Android> CompressDataChanged;
		ReferenceField<bool> _compressData = true;

		//!!!!impl
		//[Category( "Android" )]
		//[DefaultValue( true )]
		//public Reference<bool> PatchProjectFiles
		//{
		//	get { if( _patchProjectFiles.BeginGet() ) PatchProjectFiles = _patchProjectFiles.Get( this ); return _patchProjectFiles.value; }
		//	set { if( _patchProjectFiles.BeginSet( ref value ) ) { try { PatchProjectFilesChanged?.Invoke( this ); } finally { _patchProjectFiles.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PatchProjectFiles"/> property value changes.</summary>
		//public event Action<Product_Android> PatchProjectFilesChanged;
		//ReferenceField<bool> _patchProjectFiles = true;

		///// <summary>
		///// The displayable name of the package.
		///// </summary>
		//[Category( "Android" )]
		//[DefaultValue( "NeoAxis.Player" )]
		//[Serialize]
		//public Reference<string> PackageDisplayName
		//{
		//	get { if( _packageDisplayName.BeginGet() ) PackageDisplayName = _packageDisplayName.Get( this ); return _packageDisplayName.value; }
		//	set { if( _packageDisplayName.BeginSet( ref value ) ) { try { PackageDisplayNameChanged?.Invoke( this ); } finally { _packageDisplayName.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PackageDisplayName"/> property value changes.</summary>
		//public event Action<Product_Android> PackageDisplayNameChanged;
		//ReferenceField<string> _packageDisplayName = "NeoAxis.Player";

		///// <summary>
		///// The version of the product package.
		///// </summary>
		//[Category( "Android" )]
		//[DefaultValue( "1.0.0.0" )]
		//[Serialize]
		//public Reference<string> PackageVersion
		//{
		//	get { if( _packageVersion.BeginGet() ) PackageVersion = _packageVersion.Get( this ); return _packageVersion.value; }
		//	set { if( _packageVersion.BeginSet( ref value ) ) { try { PackageVersionChanged?.Invoke( this ); } finally { _packageVersion.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PackageVersion"/> property value changes.</summary>
		//public event Action<Product_Android> PackageVersionChanged;
		//ReferenceField<string> _packageVersion = "1.0.0.0";

		///// <summary>
		///// The name of the package publisher.
		///// </summary>
		//[Category( "Android" )]
		//[DefaultValue( "CN=DefaultPublisher" )]
		//public Reference<string> PackagePublisher
		//{
		//	get { if( _packagePublisher.BeginGet() ) PackagePublisher = _packagePublisher.Get( this ); return _packagePublisher.value; }
		//	set { if( _packagePublisher.BeginSet( ref value ) ) { try { PackagePublisherChanged?.Invoke( this ); } finally { _packagePublisher.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PackagePublisher"/> property value changes.</summary>
		//public event Action<Product_Android> PackagePublisherChanged;
		//ReferenceField<string> _packagePublisher = "CN=DefaultPublisher";

		///// <summary>
		///// Displayed publisher name.
		///// </summary>
		//[Category( "Android" )]
		////[DisplayName( "PublisherDisplayName" )]
		//[DefaultValue( "Default Publisher" )]
		//[Serialize]
		//public Reference<string> PublisherDisplayName
		//{
		//	get { if( _publisherDisplayName.BeginGet() ) PublisherDisplayName = _publisherDisplayName.Get( this ); return _publisherDisplayName.value; }
		//	set { if( _publisherDisplayName.BeginSet( ref value ) ) { try { PublisherDisplayNameChanged?.Invoke( this ); } finally { _publisherDisplayName.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="PublisherDisplayName"/> property value changes.</summary>
		//public event Action<Product_Android> PublisherDisplayNameChanged;
		//ReferenceField<string> _publisherDisplayName = "Default Publisher";

		/////// <summary>
		/////// Whether to create Appx package.
		/////// </summary>
		////[Category( "Build" )]
		////[DefaultValue( false )]
		////[Serialize]
		////public Reference<bool> CreateAppxPackage
		////{
		////	get { if( _createAppxPackage.BeginGet() ) CreateAppxPackage = _createAppxPackage.Get( this ); return _createAppxPackage.value; }
		////	set { if( _createAppxPackage.BeginSet( ref value ) ) { try { CreateAppxPackageChanged?.Invoke( this ); } finally { _createAppxPackage.EndSet(); } } }
		////}
		/////// <summary>Occurs when the <see cref="CreateAppxPackage"/> property value changes.</summary>
		////public event Action<Product_Android> CreateAppxPackageChanged;
		////ReferenceField<bool> _createAppxPackage = false;

		/////// <summary>
		/////// Whether to create Appx bundle.
		/////// </summary>
		////[Category( "Build" )]
		////[DefaultValue( "Always" )]
		////[Serialize]
		////[Browsable( false )]
		////public string CreateAppxBundle { get; } = "Always";

		///////////////////////////////////////////
		//// app ui (windows Start menu, main window caption)

		//[Category( "Android" )]
		////[DisplayName( "DisplayName" )]
		//[DefaultValue( "NeoAxis Player" )]
		//[Serialize]
		//public Reference<string> AppDisplayName
		//{
		//	get { if( _appDisplayName.BeginGet() ) AppDisplayName = _appDisplayName.Get( this ); return _appDisplayName.value; }
		//	set { if( _appDisplayName.BeginSet( ref value ) ) { try { AppDisplayNameChanged?.Invoke( this ); } finally { _appDisplayName.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AppDisplayName"/> property value changes.</summary>
		//public event Action<Product_Android> AppDisplayNameChanged;
		//ReferenceField<string> _appDisplayName = "NeoAxis Player";

		///// <summary>
		///// The description of the product.
		///// </summary>
		//[Category( "Android" )]
		////[DisplayName( "Description" )]
		//[DefaultValue( "NeoAxis Application" )]
		//[Serialize]
		//public Reference<string> AppDescription
		//{
		//	get { if( _appDescription.BeginGet() ) AppDescription = _appDescription.Get( this ); return _appDescription.value; }
		//	set
		//	{
		//		if( string.IsNullOrWhiteSpace( value ) )
		//		{
		//			value = ProductName;
		//			//throw new Exception( "Description must not begin or end with whitespace" );
		//		}
		//		if( _appDescription.BeginSet( ref value ) ) { try { AppDescriptionChanged?.Invoke( this ); } finally { _appDescription.EndSet(); } }
		//	}
		//}
		///// <summary>Occurs when the <see cref="AppDescription"/> property value changes.</summary>
		//public event Action<Product_Android> AppDescriptionChanged;
		//ReferenceField<string> _appDescription = "NeoAxis Application";

		///// <summary>
		///// The application identifier.
		///// </summary>
		//[Category( "Android" )]
		//[DefaultValue( "App" )]
		//public Reference<string> ApplicationId
		//{
		//	get { if( _applicationId.BeginGet() ) ApplicationId = _applicationId.Get( this ); return _applicationId.value; }
		//	set { if( _applicationId.BeginSet( ref value ) ) { try { ApplicationIdChanged?.Invoke( this ); } finally { _applicationId.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ApplicationId"/> property value changes.</summary>
		//public event Action<Product_Android> ApplicationIdChanged;
		//ReferenceField<string> _applicationId = "App";

		///// <summary>
		///// The entry point of the product.
		///// </summary>
		//[Category( "Android" )]
		//[DefaultValue( "NeoAxis.Player.App" )]
		//public Reference<string> EntryPoint
		//{
		//	get { if( _entryPoint.BeginGet() ) EntryPoint = _entryPoint.Get( this ); return _entryPoint.value; }
		//	set { if( _entryPoint.BeginSet( ref value ) ) { try { EntryPointChanged?.Invoke( this ); } finally { _entryPoint.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="EntryPoint"/> property value changes.</summary>
		//public event Action<Product_Android> EntryPointChanged;
		//ReferenceField<string> _entryPoint = "NeoAxis.Player.App";

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				//switch( p.Name )
				//{
				//case nameof( PackageDisplayName ):
				//case nameof( PackageVersion ):
				//case nameof( PackagePublisher ):
				//case nameof( PublisherDisplayName ):
				//case nameof( AppDisplayName ):
				//case nameof( AppDescription ):
				//case nameof( ApplicationId ):
				//case nameof( EntryPoint ):
				//	if( !PatchProjectFiles )
				//		skip = true;
				//	break;
				//}
			}
		}

		public override SystemSettings.Platform Platform
		{
			get { return SystemSettings.Platform.Android; }
		}

		[Browsable( false )]
		public override bool SupportsBuildAndRun
		{
			get { return false; }
		}

		public override void BuildFunction( ProductBuildInstance buildInstance )
		{
			try
			{
				PatchCSharpProjects( buildInstance );

				CopyFilesToPackageFolder( buildInstance );
				buildInstance.Progress = 0.8f;

				if( CheckCancel( buildInstance ) )
					return;

				//if( PatchProjectFiles )
				//	DoPatchProjectFiles( buildInstance );
				//buildInstance.Progress = 0.9f;

				//if( CheckCancel( buildInstance ) )
				//	return;
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

			if( CheckCancel( buildInstance ) )
				return;

			ShowSuccessScreenNotification();
		}

		void PatchCSharpProjects( ProductBuildInstance buildInstance )
		{
			{
				var p1 = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.Android.csproj" );
				var p2 = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.CoreExtension\NeoAxis.CoreExtension.csproj" );
				if( !EditorCommandLineTools.PlatformProjectPatch.Process( p1, p2, out var error, out _ ) )
					throw new Exception( error );
			}

			{
				var p1 = Path.Combine( VirtualFileSystem.Directories.Project, @"Project.Android.csproj" );
				var p2 = Path.Combine( VirtualFileSystem.Directories.Project, @"Project.csproj" );
				if( !EditorCommandLineTools.PlatformProjectPatch.Process( p1, p2, out var error, out _ ) )
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

			//copy Build.Android.sln or Build.Android.Extended.sln
			if( File.Exists( Path.Combine( VirtualFileSystem.Directories.Project, "Build.Android.Extended.sln" ) ) )
			{
				CopyFiles( VirtualFileSystem.Directories.Project, buildInstance.DestinationFolder, buildInstance, new Range( 0.4, 0.4 ), "Build.Android.Extended.sln" );
			}
			else
			{
				CopyFiles( VirtualFileSystem.Directories.Project, buildInstance.DestinationFolder, buildInstance, new Range( 0.4, 0.4 ), "Build.Android.sln" );
			}

			//copy Project.Android.csproj
			CopyFiles( VirtualFileSystem.Directories.Project, buildInstance.DestinationFolder, buildInstance, new Range( 0.4, 0.4 ), "Project.Android.csproj" );

			//copy Properties
			CopyFolder( Path.Combine( VirtualFileSystem.Directories.Project, "Properties" ), Path.Combine( buildInstance.DestinationFolder, "Properties" ), buildInstance, new Range( 0.4, 0.4 ) );

			//copy part of Sources
			var sourceSourcesPath = Path.Combine( VirtualFileSystem.Directories.Project, "Sources" );
			string destSourcesPath = Path.Combine( buildInstance.DestinationFolder, "Sources" );

			//copy Sources\NeoAxis.Player.Android exclude Project.zip.hash, Project.zip
			{
				var excludePaths = new List<string>();
				excludePaths.Add( @"Sources\NeoAxis.Player.Android\Assets\Project.zip.hash" );
				excludePaths.Add( @"Sources\NeoAxis.Player.Android\Assets\Project.zip" );

				CopyFolder( Path.Combine( sourceSourcesPath, "NeoAxis.Player.Android" ), Path.Combine( destSourcesPath, "NeoAxis.Player.Android" ), buildInstance, new Range( 0.4, 0.45 ), excludePaths );
			}

			//copy Sources\NeoAxis.CoreExtension
			CopyFolder( Path.Combine( sourceSourcesPath, "NeoAxis.CoreExtension" ), Path.Combine( destSourcesPath, "NeoAxis.CoreExtension" ), buildInstance, new Range( 0.45, 0.5 ) );

			var sourceBinariesPath = VirtualFileSystem.Directories.Binaries;
			string destBinariesPath = Path.Combine( buildInstance.DestinationFolder, "Binaries" );

			var sourcePlatformFolder = Path.Combine( sourceBinariesPath, "NeoAxis.Internal\\Platforms", Platform.ToString() );
			var destPlatformFolder = Path.Combine( destBinariesPath, "NeoAxis.Internal\\Platforms", Platform.ToString() );

			////copy managed dll references from original folder
			//CopyFiles( VirtualFileSystem.Directories.Binaries, destBinariesPath, buildInstance, new Range( 0.5, 0.6 ), "*.dll" );
			//copy NeoAxis.DefaultSettings.config
			Directory.CreateDirectory( destBinariesPath );
			File.Copy(
				Path.Combine( VirtualFileSystem.Directories.Binaries, "NeoAxis.DefaultSettings.config" ),
				Path.Combine( destBinariesPath, "NeoAxis.DefaultSettings.config" ) );

			//!!!!unnecessary dlls are copied? we need a list of references?
			//copy managed dll references from Android folder
			CopyFiles(
				Path.Combine( sourcePlatformFolder, "Managed" ),
				Path.Combine( destPlatformFolder, "Managed" ), buildInstance, new Range( 0.6, 0.7 ), "*.dll" );

			if( CheckCancel( buildInstance ) )
				return;

			//create Project.zip
			{
				var destinationFileName = Path.Combine( buildInstance.DestinationFolder, @"Sources\NeoAxis.Player.Android\Assets\Project.zip" );
				var compressionLevel = CompressData.Value ? CompressionLevel.Optimal : CompressionLevel.NoCompression;

				if( File.Exists( destinationFileName ) )
					File.Delete( destinationFileName );

				var paths = new List<string>();
				paths.Add( Path.Combine( buildInstance.DestinationFolder, "Assets" ) );
				paths.Add( Path.Combine( buildInstance.DestinationFolder, @"Binaries\NeoAxis.DefaultSettings.config" ) );
				//!!!!without CSharpScripts
				paths.Add( Path.Combine( buildInstance.DestinationFolder, "Caches" ) );

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
								var fileName = file.Substring( buildInstance.DestinationFolder.Length + 1 );
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
							var fileName = path.Substring( buildInstance.DestinationFolder.Length + 1 );
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

			//delete Assets, Caches, Binaries\NeoAxis.Internal\Tips, Binaries\NeoAxis.Internal\Tools
			{
				//need for cs files
				//var destFolder = Path.Combine( buildInstance.DestinationFolder, "Assets" );
				//if( Directory.Exists( destFolder ) )
				//	Directory.Delete( destFolder, true );

				//need for cs files
				//destFolder = Path.Combine( buildInstance.DestinationFolder, "Caches" );
				//if( Directory.Exists( destFolder ) )
				//	Directory.Delete( destFolder, true );

				var destFolder = Path.Combine( buildInstance.DestinationFolder, @"Binaries\NeoAxis.Internal\Tips" );
				if( Directory.Exists( destFolder ) )
					Directory.Delete( destFolder, true );

				destFolder = Path.Combine( buildInstance.DestinationFolder, @"Binaries\NeoAxis.Internal\Tools" );
				if( Directory.Exists( destFolder ) )
					Directory.Delete( destFolder, true );
			}

		}

		//void DoPatchProjectFiles( ProductBuildInstance buildInstance )
		//{
		//!!!!

		//string destSourcesPath = Path.Combine( buildInstance.DestinationFolder, "Sources" );

		//PatchManifestFile( Path.Combine( destSourcesPath, "NeoAxis.Player.UWP\\Package.appxmanifest" ) );
		//PatchCSProjFile( Path.Combine( destSourcesPath, "NeoAxis.Player.UWP\\NeoAxis.Player.UWP.csproj" ) );
		//PatchAssemblyInfoFile( Path.Combine( destSourcesPath, "NeoAxis.Player.UWP\\Properties\\AssemblyInfo.cs" ) );
		//}

		//// The package manifest is an XML document that contains the info the system needs to deploy, display, or update a Windows app.
		//// https://docs.microsoft.com/en-us/uwp/schemas/appxpackage/appx-package-manifest
		//void PatchManifestFile( string manifestPath )
		//{
		//	string data = File.ReadAllText( manifestPath );

		//	data = data
		//		.Replace( "Name=\"NeoAxis.Player\"", "Name=\"" + PackageName + "\"" )
		//		.Replace( "Publisher=\"CN=DefaultPublisher\"", "Publisher=\"" + PackagePublisher + "\"" )
		//		.Replace( "Version=\"1.0.0.0\"", "Version=\"" + PackageVersion + "\"" )
		//		.Replace( "<DisplayName>NeoAxis.Player</DisplayName>", "<DisplayName>" + PackageDisplayName + "</DisplayName>" )
		//		.Replace( "<PublisherDisplayName>Default Publisher</PublisherDisplayName>", "<PublisherDisplayName>" + PublisherDisplayName + "</PublisherDisplayName>" )
		//		.Replace( "Id=\"App\"", "Id=\"" + ApplicationId + "\"" )
		//		//.Replace( "{Executable}", ExecutableName + ".exe" )
		//		.Replace( "EntryPoint=\"NeoAxis.Player.App\"", "EntryPoint=\"" + EntryPoint + "\"" )
		//		//.Replace( "{BackgroundColor}", BackgroundColor )
		//		.Replace( "DisplayName=\"NeoAxis Player\"", "DisplayName=\"" + AppDisplayName + "\"" )
		//		.Replace( "Description=\"NeoAxis Application\"", "Description=\"" + AppDescription + "\"" );

		//	File.WriteAllText( manifestPath, data );
		//}

		//void PatchCSProjFile( string path )
		//{
		//	string data = File.ReadAllText( path );
		//	data = data.Replace( "<AssemblyName>NeoAxis.Player</AssemblyName>", "<AssemblyName>" + ProductName + "</AssemblyName>" );
		//	File.WriteAllText( path, data );
		//}

		//void PatchAssemblyInfoFile( string path )
		//{
		//	string data = File.ReadAllText( path );
		//	data = data.Replace( "NeoAxis.Player.UWP", ProductName );
		//	File.WriteAllText( path, data );
		//}

		protected override void OnGetPaths( List<string> paths )
		{
			base.OnGetPaths( paths );

			//Caches
			paths.Add( "Caches" );
			if( !ShaderCache )
				paths.Add( @"exclude:Caches\ShaderCache" );
			if( !FileCache )
				paths.Add( @"exclude:Caches\Files" );
			paths.Add( @"exclude:Caches\CSharpScripts\CSharpScripts.cache" );
			paths.Add( @"exclude:Caches\CSharpScripts\CSharpScripts.dll" );
			paths.Add( @"exclude:Caches\CSharpScripts\CSharpScripts.pdb" );
		}
	}
}
#endif