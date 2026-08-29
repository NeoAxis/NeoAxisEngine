// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NeoAxis.Editor;
using System.Text;
using Internal;

namespace NeoAxis
{
	/// <summary>
	/// Represents the product build settings for Linux.
	/// </summary>
	public class Product_Linux : Product
	{

		//!!!!
		//ImportTools
		//CubemapProcessingTools
		//UIWebBrowser

		//CompilingScripts


		//!!!!merge with all
		public enum ConfigurationEnum
		{
			Debug,
			Release,
		}

		//!!!!merge with all
		/// <summary>
		/// The build configuration.
		/// </summary>
		[Category( "Linux" )]
		[DefaultValue( ConfigurationEnum.Release )]
		public Reference<ConfigurationEnum> Configuration
		{
			get { if( _configuration.BeginGet() ) Configuration = _configuration.Get( this ); return _configuration.value; }
			set { if( _configuration.BeginSet( this, ref value ) ) { try { ConfigurationChanged?.Invoke( this ); } finally { _configuration.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Configuration"/> property value changes.</summary>
		public event Action<Product_Linux> ConfigurationChanged;
		ReferenceField<ConfigurationEnum> _configuration = ConfigurationEnum.Release;

		/// <summary>
		/// The target platform architecture.
		/// </summary>
		[DefaultValue( ProfileEnum.x64 )]
		public Reference<ProfileEnum> Profile
		{
			get { if( _profile.BeginGet() ) Profile = _profile.Get( this ); return _profile.value; }
			set { if( _profile.BeginSet( this, ref value ) ) { try { ProfileChanged?.Invoke( this ); } finally { _profile.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Profile"/> property value changes.</summary>
		public event Action<Product_Linux> ProfileChanged;
		ReferenceField<ProfileEnum> _profile = ProfileEnum.x64;

		/// <summary>
		/// Define constants for Project assembly separated by semicolon. For example: "CLIENT;ANOTHER_CONSTANT".
		/// </summary>
		[DefaultValue( "" )]
		public Reference<string> DefineConstants
		{
			get { if( _defineConstants.BeginGet() ) DefineConstants = _defineConstants.Get( this ); return _defineConstants.value; }
			set { if( _defineConstants.BeginSet( this, ref value ) ) { try { DefineConstantsChanged?.Invoke( this ); } finally { _defineConstants.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DefineConstants"/> property value changes.</summary>
		public event Action<Product_Linux> DefineConstantsChanged;
		ReferenceField<string> _defineConstants = "";

		///// <summary>
		///// The name of application executable file.
		///// </summary>
		//[DefaultValue( "NeoAxis.Player.Linux" )]
		////[Category( "Product" )]
		//public Reference<string> ExecutableName
		//{
		//	get { if( _executableName.BeginGet() ) ExecutableName = _executableName.Get( this ); return _executableName.value; }
		//	set { if( _executableName.BeginSet( this, ref value ) ) { try { ExecutableNameChanged?.Invoke( this ); } finally { _executableName.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ExecutableName"/> property value changes.</summary>
		//public event Action<Product_Linux> ExecutableNameChanged;
		//ReferenceField<string> _executableName = "NeoAxis.Player.Linux";

		//!!!!add dotnet runtime and test
		///// <summary>
		///// Whether to include .NET runtime and assemblies to support compiling C# scripts in the built product.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> CompilingScripts
		//{
		//	get { if( _compilingScripts.BeginGet() ) CompilingScripts = _compilingScripts.Get( this ); return _compilingScripts.value; }
		//	set { if( _compilingScripts.BeginSet( this, ref value ) ) { try { CompilingScriptsChanged?.Invoke( this ); } finally { _compilingScripts.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CompilingScripts"/> property value changes.</summary>
		//public event Action<Product_Linux> CompilingScriptsChanged;
		//ReferenceField<bool> _compilingScripts = false;

		//!!!!
		///// <summary>
		///// Whether to include tools that are intended to import 3D models.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> ImportTools
		//{
		//	get { if( _importTools.BeginGet() ) ImportTools = _importTools.Get( this ); return _importTools.value; }
		//	set { if( _importTools.BeginSet( this, ref value ) ) { try { ImportToolsChanged?.Invoke( this ); } finally { _importTools.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ImportTools"/> property value changes.</summary>
		//public event Action<Product_Linux> ImportToolsChanged;
		//ReferenceField<bool> _importTools = false;

		///// <summary>
		///// Whether to copy .NET 6 runtime from NeoAxis.Internal\Platforms\Linux\dotnet_x64 with overwriting files.
		///// </summary>
		//[DefaultValue( true )]
		//public Reference<bool> CopyDotNetRuntime
		//{
		//	get { if( _copyDotNetRuntime.BeginGet() ) CopyDotNetRuntime = _copyDotNetRuntime.Get( this ); return _copyDotNetRuntime.value; }
		//	set { if( _copyDotNetRuntime.BeginSet( this, ref value ) ) { try { CopyDotNetRuntimeChanged?.Invoke( this ); } finally { _copyDotNetRuntime.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CopyDotNetRuntime"/> property value changes.</summary>
		//public event Action<Product_Linux> CopyDotNetRuntimeChanged;
		//ReferenceField<bool> _copyDotNetRuntime = true;

		///// <summary>
		///// Whether to include NeoAxis Editor.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> Editor
		//{
		//	get { if( _editor.BeginGet() ) Editor = _editor.Get( this ); return _editor.value; }
		//	set { if( _editor.BeginSet( this, ref value ) ) { try { EditorChanged?.Invoke( this ); } finally { _editor.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Editor"/> property value changes.</summary>
		//public event Action<Product_Linux> EditorChanged;
		//ReferenceField<bool> _editor = false;

		///// <summary>
		///// Whether to include tools that are intended to import 3D models.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> ImportTools
		//{
		//	get { if( _importTools.BeginGet() ) ImportTools = _importTools.Get( this ); return _importTools.value; }
		//	set { if( _importTools.BeginSet( this, ref value ) ) { try { ImportToolsChanged?.Invoke( this ); } finally { _importTools.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ImportTools"/> property value changes.</summary>
		//public event Action<Product_Linux> ImportToolsChanged;
		//ReferenceField<bool> _importTools = false;

		///// <summary>
		///// Whether to include tools that are intended to process environment cubemaps.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> CubemapProcessingTools
		//{
		//	get { if( _cubemapProcessingTools.BeginGet() ) CubemapProcessingTools = _cubemapProcessingTools.Get( this ); return _cubemapProcessingTools.value; }
		//	set { if( _cubemapProcessingTools.BeginSet( this, ref value ) ) { try { CubemapProcessingToolsChanged?.Invoke( this ); } finally { _cubemapProcessingTools.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CubemapProcessingTools"/> property value changes.</summary>
		//public event Action<Product_Linux> CubemapProcessingToolsChanged;
		//ReferenceField<bool> _cubemapProcessingTools = false;

		/////// <summary>
		/////// Whether to include build tools that are intended to compile C# scripts.
		/////// </summary>
		////[DefaultValue( false )]
		////public Reference<bool> BuildTools
		////{
		////	get { if( _buildTools.BeginGet() ) BuildTools = _buildTools.Get( this ); return _buildTools.value; }
		////	set { if( _buildTools.BeginSet( this, ref value ) ) { try { BuildToolsChanged?.Invoke( this ); } finally { _buildTools.EndSet(); } } }
		////}
		/////// <summary>Occurs when the <see cref="BuildTools"/> property value changes.</summary>
		////public event Action<Product_Linux> BuildToolsChanged;
		////ReferenceField<bool> _buildTools = false;

		/// <summary>
		/// Whether to include files for debugging (xml, pdb).
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DebugFiles
		{
			get { if( _debugFiles.BeginGet() ) DebugFiles = _debugFiles.Get( this ); return _debugFiles.value; }
			set { if( _debugFiles.BeginSet( this, ref value ) ) { try { DebugFilesChanged?.Invoke( this ); } finally { _debugFiles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugFiles"/> property value changes.</summary>
		public event Action<Product_Linux> DebugFilesChanged;
		ReferenceField<bool> _debugFiles = false;

		///// <summary>
		///// Whether to include files to support UIWebBrowser control.
		///// </summary>
		//[DisplayName( "UIWebBrowser" )]
		//[DefaultValue( false )]
		//public Reference<bool> UIWebBrowser
		//{
		//	get { if( _uIWebBrowser.BeginGet() ) UIWebBrowser = _uIWebBrowser.Get( this ); return _uIWebBrowser.value; }
		//	set { if( _uIWebBrowser.BeginSet( this, ref value ) ) { try { UIWebBrowserChanged?.Invoke( this ); } finally { _uIWebBrowser.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="UIWebBrowser"/> property value changes.</summary>
		//public event Action<Product_Linux> UIWebBrowserChanged;
		//ReferenceField<bool> _uIWebBrowser = false;

		///// <summary>
		///// Whether to include localized assemblies of .NET.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> SatelliteResourceLanguages
		//{
		//	get { if( _satelliteResourceLanguages.BeginGet() ) SatelliteResourceLanguages = _satelliteResourceLanguages.Get( this ); return _satelliteResourceLanguages.value; }
		//	set { if( _satelliteResourceLanguages.BeginSet( this, ref value ) ) { try { SatelliteResourceLanguagesChanged?.Invoke( this ); } finally { _satelliteResourceLanguages.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SatelliteResourceLanguages"/> property value changes.</summary>
		//public event Action<Product_Linux> SatelliteResourceLanguagesChanged;
		//ReferenceField<bool> _satelliteResourceLanguages = false;

		/////////////////////////////////////////

		public enum ProfileEnum
		{
			x64,
			//ARM64,
		}

		/////////////////////////////////////////

		public override SystemSettings.Platform Platform
		{
			get { return SystemSettings.Platform.Linux; }
		}

		public override void BuildFunction( ProductBuildInstance buildInstance )
		{
			try
			{
				BuildProjects( buildInstance, new Range( 0, 0.5 ) );

				var paths = GetPaths();
				CopyIncludeExcludePaths( paths, buildInstance, new Range( 0.5, 0.9 ) ); //new Range( 0, 0.99 ) );

				//obfuscate
				if( Obfuscate )
					DoObfuscation( buildInstance, new Range( 0.9, 0.99 ) );
			}
			catch( Exception e )
			{
				buildInstance.Error = e.Message;
				buildInstance.State = ProductBuildInstance.StateEnum.Error;
				return;
			}

			////copy runtime
			//if( CopyDotNetRuntime )
			//{
			//	var sourcePath = Path.Combine( VirtualFileSystem.Directories.Project, "Binaries\\NeoAxis.Internal\\Platforms\\Linux\\dotnet_x64" );
			//	var destPath = Path.Combine( buildInstance.DestinationFolder, "Binaries" );

			//	CopyFolder( sourcePath, destPath, buildInstance, new Range( 0.9, 0.99 ) );

			//	//CopyFiles( zz, zzz, buildInstance, new Range( 0.9, 0.99 ), zzz );
			//}

			////write Run.cmd
			//try
			//{
			//	string fileName = Path.Combine( buildInstance.DestinationFolder, "Run.cmd" );
			//	File.WriteAllText( fileName, Path.Combine( @"Binaries", ExecutableName + ".exe" ) );
			//}
			//catch( Exception e )
			//{
			//	buildInstance.Error = e.Message;
			//	buildInstance.State = ProductBuildInstance.StateEnum.Error;
			//	return;
			//}

			//post build event
			if( !PeformPostBuild( buildInstance ) )
				return;
			if( CheckCancel( buildInstance ) )
				return;

			//done
			buildInstance.Progress = 1;
			buildInstance.State = ProductBuildInstance.StateEnum.Success;

			////run
			//if( buildInstance.Run )
			//{
			//	string executableFileName = Path.Combine( buildInstance.DestinationFolder, "Binaries", ExecutableName + ".exe" );
			//	Process.Start( new ProcessStartInfo( executableFileName, "" ) { UseShellExecute = true } );
			//	//Process.Start( executableFileName, "" );
			//}

			if( CheckCancel( buildInstance ) )
				return;

			ShowSuccessScreenNotification();
		}

		[Browsable( false )]
		public override bool SupportsBuildAndRun
		{
			get { return false; }
		}

		void BuildProjects( ProductBuildInstance buildInstance, Range progressRange )
		{
			var dotnetExePath = Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Platforms\Windows\dotnet\dotnet.exe" );

			var destinationFolder = Path.Combine( buildInstance.DestinationFolder, "Binaries" );
			Directory.CreateDirectory( destinationFolder );

			buildInstance.Progress = (float)progressRange.Minimum;

			//!!!!
			////build assembly for compiling scripts
			//if( CompilingScripts )
			//{
			//	var projectFullPath = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.Core.CompileScripts\NeoAxis.Core.CompileScripts.csproj" );
			//	var arguments = $"build \"{projectFullPath}\" --configuration Release --output \"{destinationFolder}\" --verbosity minimal";

			//	var success = ProcessUtility.RunAndWait( dotnetExePath, arguments, out var result ) == 0;
			//	if( !success )
			//	{
			//		throw new Exception( $"Unable to compile project.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log." );
			//	}
			//}

			buildInstance.Progress = (float)MathEx.Lerp( progressRange.Minimum, progressRange.Maximum, 0.33 );

			//build Project assembly
			{
				var projectFullPath = Path.Combine( VirtualFileSystem.Directories.Project, @"Project.csproj" );
				var arguments = $"build \"{projectFullPath}\" --configuration {Configuration.Value} --output \"{destinationFolder}\" --verbosity minimal";

				var defineConstants = DefineConstants.Value.Trim();
				if( !string.IsNullOrEmpty( defineConstants ) )
				{
					arguments += $" -p:DefineConstants=\"{defineConstants}\"";
					//arguments += " -p:DefineConstants=\"$(DefineConstants);CLIENT\"";
				}

				var success = ProcessUtility.RunAndWait( dotnetExePath, arguments, out var result ) == 0;
				if( !success )
				{
					throw new Exception( $"Unable to compile project.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log." );
				}
			}

			buildInstance.Progress = (float)MathEx.Lerp( progressRange.Minimum, progressRange.Maximum, 0.66 );

			//build Player assembly
			{
				var projectFullPath = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.Player\NeoAxis.Player.csproj" );
				var arguments = $"build \"{projectFullPath}\" --configuration {Configuration.Value}-Linux-{Profile.Value} --output \"{destinationFolder}\" --verbosity minimal";

				var success = ProcessUtility.RunAndWait( dotnetExePath, arguments, out var result ) == 0;
				if( !success )
				{
					throw new Exception( $"Unable to compile project.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log." );
				}
			}

			buildInstance.Progress = (float)progressRange.Maximum;

			//delete debug files
			if( !DebugFiles )
			{
				foreach( var fileName in Directory.GetFiles( destinationFolder, "*.pdb", SearchOption.TopDirectoryOnly ) )
					File.Delete( fileName );
				foreach( var fileName in Directory.GetFiles( destinationFolder, "*.xml", SearchOption.TopDirectoryOnly ) )
					File.Delete( fileName );
				foreach( var fileName in Directory.GetFiles( destinationFolder, "*.mdb", SearchOption.TopDirectoryOnly ) )
					File.Delete( fileName );
			}



			////var projectFullPath = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.Player\NeoAxis.Player.csproj" );

			////string arguments;
			////{
			////	var builder = new StringBuilder();
			////	builder.Append( "build" );
			////	//builder.Append( "publish" );
			////	builder.Append( $" \"{projectFullPath}\"" );

			////	builder.Append( $" -p:PublishProfile=FolderProfile-Linux-x64" );

			////	var configuration = "Release-Linux-x64";
			////	builder.Append( $" --configuration {configuration}" );

			////	builder.Append( $" --output \"{destinationFolder}\"" );

			////	const string Verbosity = "minimal";

			////	builder.Append( $" --verbosity {Verbosity}" );

			////	//builder.Append( $" --configuration {BuildConfiguration}" );
			////	//builder.Append( $" --output \"{destinationFolder}\"" );
			////	//builder.Append( $" --verbosity {Verbosity}" );

			////	arguments = builder.ToString();
			////}

			//////var sourceFolder = Path.Combine( VirtualFileSystem.Directories.Project, "Binaries" );
			////var dotnetExePath = Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Platforms\Windows\dotnet\dotnet.exe" );
			////var success = ProcessUtility.RunAndWait( dotnetExePath, arguments, out var result ) == 0;
			////if( !success )
			////{
			////	throw new Exception( $"Unable to compile project.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log." );
			////}

			////buildInstance.Progress = 0.6f;

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

			//Binaries

			paths.Add( @"Binaries\NeoAxis.Internal" );

			//exclude from Binaries
			{
				var sourceFolder = Path.Combine( VirtualFileSystem.Directories.Project, "Binaries" );

				var excludePaths = new List<string>();
				excludePaths.AddRange( GetPlatformsExcludePaths() );

				//if( !Editor )
				{
					//excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tips" ) );
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Localization" ) );
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\PlatformTools" ) );
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\Obfuscar" ) );
				}

				//if( !Editor )
				excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Linux\dotnet_x64" ) );

				////if( !BuildTools )
				////{
				////	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\BuildTools" ) );
				////	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\Framework" ) );
				////}

				//if( !UIWebBrowser )
				//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Linux\x64\CefGlue" ) );

				//if( !ImportTools )
				//{
				//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Linux\x64\FbxWrapperNative.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Linux\x64\assimp-vc141-mt.dll" ) );
				//}

				//if( !CubemapProcessingTools )
				excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\Filament" ) );

				//if( !SatelliteResourceLanguages )
				//{
				//	excludePaths.Add( Path.Combine( sourceFolder, "cs" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "de" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "es" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "fr" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "it" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "ja" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "ko" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "pl" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "pt-BR" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "ru" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "tr" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "zh-Hans" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "zh-Hant" ) );
				//}

				foreach( var excludePath in excludePaths )
					paths.Add( "exclude:" + excludePath );
			}
		}

		//protected override void OnGetPaths( List<string> paths )
		//{
		//	base.OnGetPaths( paths );
		//	//GetPathsFromPathsProperty( paths );
		//	//foreach( var path in Paths.Value.Split( '\n', StringSplitOptions.RemoveEmptyEntries ) )
		//	//{
		//	//	var path2 = path.Replace( "\r", "" ).Trim();
		//	//	if( path2 != "" )
		//	//		paths.Add( path2 );
		//	//}

		//	//Caches
		//	paths.Add( "Caches" );
		//	if( !ShaderCache )
		//		paths.Add( @"exclude:Caches\ShaderCache" );
		//	if( !FileCache )
		//		paths.Add( @"exclude:Caches\Files" );


		//	//Binaries

		//	paths.Add( "Binaries" );

		//	//exclude from Binaries
		//	{
		//		var sourceFolder = Path.Combine( VirtualFileSystem.Directories.Project, "Binaries" );

		//		var excludePaths = new List<string>();

		//		excludePaths.AddRange( GetPlatformsExcludePaths() );

		//		//if( !Editor )
		//		{
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tips" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Localization" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\PlatformTools" ) );

		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.Editor.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.Editor.deps.json" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.exe" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.exe.config" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.deps.json" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.runtimeconfig.dev.json" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.runtimeconfig.json" ) );

		//			//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.CoreExtension.Editor.dll" ) );
		//			//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.CoreExtension.Editor.deps.json" ) );
		//		}

		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.exe" ) );
		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.exe.config" ) );
		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.deps.json" ) );
		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.runtimeconfig.dev.json" ) );
		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.runtimeconfig.json" ) );

		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.exe" ) );
		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.exe.config" ) );
		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.deps.json" ) );
		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.runtimeconfig.dev.json" ) );
		//		excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.runtimeconfig.json" ) );

		//		excludePaths.Add( Path.Combine( sourceFolder, "_TestPlayerParameters.cmd" ) );


		//		excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.CompileScripts.dll" ) );
		//		excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.CompileScripts.deps.json" ) );


		//		//!!!!x64

		//		//if( !Editor )
		//		//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Linux\x64\dotnet" ) );

		//		////if( !BuildTools )
		//		////{
		//		////	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\BuildTools" ) );
		//		////	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\Framework" ) );
		//		////}

		//		//if( !UIWebBrowser )
		//		//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Linux\x64\CefGlue" ) );

		//		if( !DebugFiles )
		//		{
		//			foreach( var fileName in Directory.GetFiles( sourceFolder, "*.pdb", SearchOption.TopDirectoryOnly ) )
		//				excludePaths.Add( fileName );
		//			foreach( var fileName in Directory.GetFiles( sourceFolder, "*.xml", SearchOption.TopDirectoryOnly ) )
		//				excludePaths.Add( fileName );
		//			foreach( var fileName in Directory.GetFiles( sourceFolder, "*.mdb", SearchOption.TopDirectoryOnly ) )
		//				excludePaths.Add( fileName );
		//		}

		//		//if( !ImportTools )
		//		//{
		//		//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Linux\x64\FbxWrapperNative.dll" ) );
		//		//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Linux\x64\assimp-vc141-mt.dll" ) );
		//		//}

		//		//if( !CubemapProcessingTools )
		//		excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\Filament" ) );

		//		if( !SatelliteResourceLanguages )
		//		{
		//			excludePaths.Add( Path.Combine( sourceFolder, "cs" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "de" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "es" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "fr" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "it" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "ja" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "ko" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "pl" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "pt-BR" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "ru" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "tr" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "zh-Hans" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "zh-Hant" ) );
		//		}

		//		//if( !WindowsDesktopAssemblies )
		//		{
		//			excludePaths.Add( Path.Combine( sourceFolder, "Accessibility.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "D3DCompiler_47_cor3.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "DirectWriteForwarder.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "Microsoft.Win32.Registry.AccessControl.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "Microsoft.Win32.SystemEvents.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "Microsoft.WindowsDesktop.App.deps.json" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "Microsoft.WindowsDesktop.App.runtimeconfig.json" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PenImc_cor3.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationCore.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemCore.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemData.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemDrawing.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemXml.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemXmlLinq.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Aero.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Aero2.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.AeroLite.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Classic.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Luna.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Royale.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationNative_cor3.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "PresentationUI.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "ReachFramework.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.CodeDom.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Configuration.ConfigurationManager.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Design.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Diagnostics.EventLog.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Diagnostics.PerformanceCounter.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.DirectoryServices.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Drawing.Common.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Drawing.Design.dll" ) );
		//			//need//excludePaths.Add( Path.Combine( sourceFolder, "System.Drawing.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.IO.Packaging.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Printing.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Resources.Extensions.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Security.Cryptography.Pkcs.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Security.Cryptography.ProtectedData.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Security.Cryptography.Xml.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Security.Permissions.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Threading.AccessControl.dll" ) );

		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Controls.Ribbon.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Extensions.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Forms.Design.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Forms.Design.Editors.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Forms.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Input.Manipulations.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Presentation.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "System.Xaml.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "UIAutomationClient.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "UIAutomationClientSideProviders.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "UIAutomationProvider.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "UIAutomationTypes.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "vcruntime140_cor3.dll" ) );
		//			//need//excludePaths.Add( Path.Combine( sourceFolder, "WindowsBase.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "WindowsFormsIntegration.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "wpfgfx_cor3.dll" ) );
		//		}

		//		foreach( var excludePath in excludePaths )
		//			paths.Add( "exclude:" + excludePath );
		//	}
		//}

	}
}
