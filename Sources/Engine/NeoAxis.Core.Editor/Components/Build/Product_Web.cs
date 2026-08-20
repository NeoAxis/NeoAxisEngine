// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using Internal;
using NeoAxis.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Represents the product build settings for Web.
	/// </summary>
	public class Product_Web : Product
	{
		static readonly DateTime setTimeToFilesInZip = new DateTime( 2001, 1, 1, 1, 1, 1 );

		//!!!!change later to Release

		/// <summary>
		/// The build configuration. Release enables AOT compilation.
		/// </summary>
		[Category( "Compilation" )]
		[DefaultValue( ConfigurationEnum.Debug )]
		public Reference<ConfigurationEnum> Configuration
		{
			get { if( _configuration.BeginGet() ) Configuration = _configuration.Get( this ); return _configuration.value; }
			set { if( _configuration.BeginSet( this, ref value ) ) { try { ConfigurationChanged?.Invoke( this ); } finally { _configuration.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Configuration"/> property value changes.</summary>
		public event Action<Product_Web> ConfigurationChanged;
		ReferenceField<ConfigurationEnum> _configuration = ConfigurationEnum.Debug;

		/// <summary>
		/// Define constants for Project assembly separated by semicolon. For example: "CLIENT;ANOTHER_CONSTANT".
		/// </summary>
		[Category( "Compilation" )]
		[DefaultValue( "" )]
		public Reference<string> DefineConstants
		{
			get { if( _defineConstants.BeginGet() ) DefineConstants = _defineConstants.Get( this ); return _defineConstants.value; }
			set { if( _defineConstants.BeginSet( this, ref value ) ) { try { DefineConstantsChanged?.Invoke( this ); } finally { _defineConstants.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DefineConstants"/> property value changes.</summary>
		public event Action<Product_Web> DefineConstantsChanged;
		ReferenceField<string> _defineConstants = "";

		/// <summary>
		/// The verbosity level of the build process. Minimal shows only essential information, while Normal provides more detailed output.
		/// </summary>
		[Category( "Compilation" )]
		[DefaultValue( VerbosityLevelEnum.Minimal )]
		public Reference<VerbosityLevelEnum> VerbosityLevel
		{
			get { if( _verbosityLevel.BeginGet() ) VerbosityLevel = _verbosityLevel.Get( this ); return _verbosityLevel.value; }
			set { if( _verbosityLevel.BeginSet( this, ref value ) ) { try { VerbosityLevelChanged?.Invoke( this ); } finally { _verbosityLevel.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VerbosityLevel"/> property value changes.</summary>
		public event Action<Product_Web> VerbosityLevelChanged;
		ReferenceField<VerbosityLevelEnum> _verbosityLevel = VerbosityLevelEnum.Minimal;

		/// <summary>
		/// Whether to compress Zip archive with the project data.
		/// </summary>
		[Category( "Web" )]
		[DefaultValue( true )]
		public Reference<bool> CompressData
		{
			get { if( _compressData.BeginGet() ) CompressData = _compressData.Get( this ); return _compressData.value; }
			set { if( _compressData.BeginSet( this, ref value ) ) { try { CompressDataChanged?.Invoke( this ); } finally { _compressData.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CompressData"/> property value changes.</summary>
		public event Action<Product_Web> CompressDataChanged;
		ReferenceField<bool> _compressData = true;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				//switch( p.Name )
				//{
				//case nameof( EntryPoint ):
				//	if( !PatchProjectFiles )
				//		skip = true;
				//	break;
				//}
			}
		}

		public override SystemSettings.Platform Platform
		{
			get { return SystemSettings.Platform.Web; }
		}

		[Browsable( false )]
		public override bool SupportsBuildAndRun
		{
			get { return false; }
		}

		public override void BuildFunction( ProductBuildInstance buildInstance )
		{
			var tempFolder = Path.Combine( buildInstance.DestinationFolder, "_Temp" );
			var publishFolder = Path.Combine( tempFolder, "Publish" );

			try
			{
				buildInstance.ProgressText = "Checking requirements...";

				var dotnetExePath = FindSystemDotnet();
				if( string.IsNullOrEmpty( dotnetExePath ) )
					throw new Exception( ".NET SDK is not found in the system. .NET 10 SDK is required to build for Web.\r\n\r\nDownload: https://dotnet.microsoft.com/download" );

				CheckBuildRequirements( dotnetExePath );

				PatchCSharpProjects( buildInstance );

				buildInstance.ProgressText = "Copying files...";
				CopyFilesToPackageFolder( buildInstance, tempFolder );
				buildInstance.Progress = 0.4f;
				if( CheckCancel( buildInstance ) )
					return;

				buildInstance.ProgressText = "Building projects...";
				BuildProjects( buildInstance, dotnetExePath, publishFolder, new Range( 0.4, 0.85 ) );
				if( CheckCancel( buildInstance ) )
					return;

				//the destination folder must contain the final build only
				CleanDestinationFolder( buildInstance );
				CollectPublishResult( buildInstance, publishFolder, new Range( 0.85, 0.95 ) );

				//the archive must be placed after publishing to override the one created by the prebuild event
				PlaceProjectArchive( buildInstance, tempFolder );

				if( Directory.Exists( tempFolder ) )
					Directory.Delete( tempFolder, true );

				buildInstance.Progress = 0.99f;
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
				var p1 = Path.Combine( VirtualFileSystem.Directories.Project, @"Project.Web.csproj" );
				var p2 = Path.Combine( VirtualFileSystem.Directories.Project, @"Project.csproj" );
				if( File.Exists( p1 ) )
				{
					if( !EditorAPI.EditorCommandLineTools_PlatformProjectPatch_Process( p1, p2, false, out var error, out _ ) )
						throw new Exception( error );
				}
			}

			{
				var p1 = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.Player.Web\NeoAxis.Player.Web.csproj" );
				var p2 = Path.Combine( VirtualFileSystem.Directories.Project, @"Project.csproj" );
				if( File.Exists( p1 ) )
				{
					if( !EditorAPI.EditorCommandLineTools_PlatformProjectPatch_Process( p1, p2, true, out var error, out _ ) )
						throw new Exception( error );
				}
			}
		}

		static string FindSystemDotnet()
		{
			var checkPaths = new List<string>();

			var programFiles = Environment.GetEnvironmentVariable( "ProgramFiles" );
			if( !string.IsNullOrEmpty( programFiles ) )
				checkPaths.Add( Path.Combine( programFiles, "dotnet", "dotnet.exe" ) );

			var pathVariable = Environment.GetEnvironmentVariable( "PATH" );
			if( !string.IsNullOrEmpty( pathVariable ) )
			{
				foreach( var folder in pathVariable.Split( Path.PathSeparator ) )
				{
					var folder2 = folder.Trim();
					if( folder2 != "" )
					{
						try
						{
							checkPaths.Add( Path.Combine( folder2, "dotnet.exe" ) );
						}
						catch { }
					}
				}
			}

			var dotnetRoot = Environment.GetEnvironmentVariable( "DOTNET_ROOT" );
			if( !string.IsNullOrEmpty( dotnetRoot ) )
				checkPaths.Add( Path.Combine( dotnetRoot, "dotnet.exe" ) );

			foreach( var path in checkPaths )
			{
				if( File.Exists( path ) )
					return path;
			}

			return null;
		}

		void CheckBuildRequirements( string dotnetExePath )
		{
			//check .NET SDK
			{
				if( ProcessUtility.RunAndWait( dotnetExePath, "--list-sdks", out var result ) != 0 )
					throw new Exception( "Unable to get the list of installed .NET SDKs.\r\n\r\n" + result );

				var found = false;
				foreach( var line in result.Split( '\n' ) )
				{
					if( line.TrimStart().StartsWith( "10." ) )
					{
						found = true;
						break;
					}
				}

				if( !found )
					throw new Exception( ".NET 10 SDK is required to build for Web, but it was not found in the system.\r\n\r\nInstalled SDKs:\r\n" + result + "\r\n\r\nDownload: https://dotnet.microsoft.com/download" );
			}

			//check wasm-tools workload. Required for AOT only
			if( Configuration.Value == ConfigurationEnum.Release )
			{
				ProcessUtility.RunAndWait( dotnetExePath, "workload list", out var result );

				if( result == null || !result.Contains( "wasm-tools" ) )
					throw new Exception( "The 'wasm-tools' workload is required to build with AOT compilation.\r\n\r\nRun the command as administrator:\r\ndotnet workload install wasm-tools\r\n\r\nOr select Debug configuration to build without AOT." );
			}
		}

		void BuildProjects( ProductBuildInstance buildInstance, string dotnetExePath, string publishFolder, Range progressRange )
		{
			buildInstance.Progress = (float)progressRange.Minimum;

			var release = Configuration.Value == ConfigurationEnum.Release;

			buildInstance.ProgressText = release
				? "Building projects Release (AOT compilation may take some time)..."
				: "Building projects Debug...";

			var projectFullPath = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.Player.Web\NeoAxis.Player.Web.csproj" );

			var arguments = $"publish \"{projectFullPath}\"";
			arguments += $" --configuration {( release ? "Release" : "Debug" )}";
			arguments += $" --output \"{publishFolder}\"";
			arguments += $" --verbosity {VerbosityLevel.Value.ToString().ToLower()}";
			arguments += " --nologo";
			arguments += " -tl:off";
			arguments += " -p:NeoAxisProductBuild=true";
			if( release )
				arguments += " -p:RunAOTCompilation=true -p:PublishTrimmed=true -p:TrimMode=partial";

			var defineConstants = DefineConstants.Value.Trim();
			if( !string.IsNullOrEmpty( defineConstants ) )
				arguments += $" -p:DefineConstants=\"{defineConstants}\"";

			void ErrorReceived( string text )
			{
				var text2 = text.Trim();
				if( !string.IsNullOrEmpty( text2 ) )
				{
					//add to build instance logs
					var buildInstance2 = buildInstance;
					if( buildInstance2 != null )
						buildInstance2.Logs += "Error: " + text2 + "\r\n";

					//write to log files
					Log.InvisibleInfo( "Build: Error: " + text2 );
				}
			}

			void OutputReceived( string text )
			{
				var text2 = text.Trim();

				//add to build instance logs
				var buildInstance2 = buildInstance;
				if( buildInstance2 != null )
					buildInstance2.Logs += text2 + "\r\n";

				//write to log files
				Log.InvisibleInfo( "Build: Output: " + text2 );
			}

			{
				var cts = new CancellationTokenSource();
				var runResultTask = ProcessUtility.RunAndWaitAsync( dotnetExePath, arguments, errorDataReceivedCallback: ErrorReceived, outputDataReceivedCallback: OutputReceived, cancellationToken: cts.Token );
				while( !runResultTask.IsCompleted )
				{
					if( buildInstance.RequestCancel )
						cts.Cancel();
					Thread.Sleep( 10 );
				}
				if( CheckCancel( buildInstance ) )
					return;
				var runResult = runResultTask.Result;
				if( runResult.ExitCode != 0 )
					throw new Exception( $"Unable to publish project.\r\n\r\n{runResult.Output}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log." );
			}

			buildInstance.Progress = (float)progressRange.Maximum;
		}

		void CollectPublishResult( ProductBuildInstance buildInstance, string publishFolder, Range progressRange )
		{
			buildInstance.ProgressText = "Collecting result...";
			var sourceFolder = Path.Combine( publishFolder, "wwwroot" );

			if( !Directory.Exists( sourceFolder ) )
				throw new Exception( $"The publish result folder is not found.\r\n\r\n{sourceFolder}" );

			CopyFolder( sourceFolder, buildInstance.DestinationFolder, buildInstance, progressRange );
		}

		void CleanDestinationFolder( ProductBuildInstance buildInstance )
		{
			var items = new string[] { "Assets", "Caches", "Binaries", "Sources", "Properties", "Build.Web.sln", "Project.Web.csproj" };

			foreach( var item in items )
			{
				var path = Path.Combine( buildInstance.DestinationFolder, item );

				if( Directory.Exists( path ) )
					Directory.Delete( path, true );
				else if( File.Exists( path ) )
					File.Delete( path );
			}
		}

		void PlaceProjectArchive( ProductBuildInstance buildInstance, string tempFolder )
		{
			var sourceFileName = Path.Combine( tempFolder, "Project.zip" );
			var destinationFileName = Path.Combine( buildInstance.DestinationFolder, @"Assets\Project.zip" );

			Directory.CreateDirectory( Path.GetDirectoryName( destinationFileName ) );

			File.Copy( sourceFileName, destinationFileName, true );
			File.Copy( sourceFileName + ".hash", destinationFileName + ".hash", true );
		}

		void CopyFilesToPackageFolder( ProductBuildInstance buildInstance, string tempFolder )
		{
			//copy files
			var copyPaths = GetPaths();

			//execute
			CopyIncludeExcludePaths( copyPaths, buildInstance, new Range( 0, 0.4 ) );

			var sourceBinariesPath = VirtualFileSystem.Directories.Binaries;
			string destBinariesPath = Path.Combine( buildInstance.DestinationFolder, "Binaries" );

			//copy NeoAxis.DefaultSettings.config
			Directory.CreateDirectory( Path.Combine( destBinariesPath, "NeoAxis.Internal" ) );
			File.Copy(
				Path.Combine( VirtualFileSystem.Directories.Binaries, "NeoAxis.Internal", "NeoAxis.DefaultSettings.config" ),
				Path.Combine( destBinariesPath, "NeoAxis.Internal", "NeoAxis.DefaultSettings.config" ), true );

			if( CheckCancel( buildInstance ) )
				return;

			//create Project.zip
			{
				buildInstance.ProgressText = "Creating Project.zip...";
				//the archive is placed into the destination after publishing, to not be overwritten by the prebuild event
				var destinationFileName = Path.Combine( tempFolder, "Project.zip" );
				Directory.CreateDirectory( tempFolder );

				var compressionLevel = CompressData.Value ? CompressionLevel.Optimal : CompressionLevel.NoCompression;

				if( File.Exists( destinationFileName ) )
					File.Delete( destinationFileName );

				var paths = new List<string>();
				paths.Add( Path.Combine( buildInstance.DestinationFolder, "Assets" ) );
				paths.Add( Path.Combine( buildInstance.DestinationFolder, @"Binaries\NeoAxis.Internal\NeoAxis.DefaultSettings.config" ) );
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

								if( CheckCancel( buildInstance ) )
									return;
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

							if( CheckCancel( buildInstance ) )
								return;
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
		}

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