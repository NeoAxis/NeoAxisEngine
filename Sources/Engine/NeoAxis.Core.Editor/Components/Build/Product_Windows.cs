// Copyright 2006Ц2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NeoAxis.Editor;
using Internal;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Represents the product build settings for Windows.
	/// </summary>
	public class Product_Windows : Product
	{
		/// <summary>
		/// The target platform architecture.
		/// </summary>
		[Category( "Compilation" )]
		[DefaultValue( ProfileEnum.x64 )]
		public Reference<ProfileEnum> Profile
		{
			get { if( _profile.BeginGet() ) Profile = _profile.Get( this ); return _profile.value; }
			set { if( _profile.BeginSet( this, ref value ) ) { try { ProfileChanged?.Invoke( this ); } finally { _profile.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Profile"/> property value changes.</summary>
		public event Action<Product_Windows> ProfileChanged;
		ReferenceField<ProfileEnum> _profile = ProfileEnum.x64;

		/// <summary>
		/// The build configuration. Release enables AOT compilation.
		/// </summary>
		[Category( "Compilation" )]
		[DefaultValue( ConfigurationEnum.Release )]
		public Reference<ConfigurationEnum> Configuration
		{
			get { if( _configuration.BeginGet() ) Configuration = _configuration.Get( this ); return _configuration.value; }
			set { if( _configuration.BeginSet( this, ref value ) ) { try { ConfigurationChanged?.Invoke( this ); } finally { _configuration.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Configuration"/> property value changes.</summary>
		public event Action<Product_Windows> ConfigurationChanged;
		ReferenceField<ConfigurationEnum> _configuration = ConfigurationEnum.Release;

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
		public event Action<Product_Windows> DefineConstantsChanged;
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
		public event Action<Product_Windows> VerbosityLevelChanged;
		ReferenceField<VerbosityLevelEnum> _verbosityLevel = VerbosityLevelEnum.Minimal;

		/// <summary>
		/// The name of application executable file.
		/// </summary>
		[Category( "Compilation" )]
		[DefaultValue( "NeoAxis.Player" )]
		//[Category( "Product" )]
		public Reference<string> ExecutableName
		{
			get { if( _executableName.BeginGet() ) ExecutableName = _executableName.Get( this ); return _executableName.value; }
			set { if( _executableName.BeginSet( this, ref value ) ) { try { ExecutableNameChanged?.Invoke( this ); } finally { _executableName.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExecutableName"/> property value changes.</summary>
		public event Action<Product_Windows> ExecutableNameChanged;
		ReferenceField<string> _executableName = "NeoAxis.Player";

		/// <summary>
		/// Whether to include .NET runtime and assemblies to support compiling C# scripts in the built product.
		/// </summary>
		[Category( "Compilation" )]
		[DefaultValue( false )]
		public Reference<bool> CompilingScripts
		{
			get { if( _compilingScripts.BeginGet() ) CompilingScripts = _compilingScripts.Get( this ); return _compilingScripts.value; }
			set { if( _compilingScripts.BeginSet( this, ref value ) ) { try { CompilingScriptsChanged?.Invoke( this ); } finally { _compilingScripts.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CompilingScripts"/> property value changes.</summary>
		public event Action<Product_Windows> CompilingScriptsChanged;
		ReferenceField<bool> _compilingScripts = false;

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
		//public event Action<Product_Windows> EditorChanged;
		//ReferenceField<bool> _editor = false;

		/// <summary>
		/// Whether to include tools that are intended to import 3D models.
		/// </summary>
		[Category( "Windows" )]
		[DefaultValue( false )]
		public Reference<bool> ImportTools
		{
			get { if( _importTools.BeginGet() ) ImportTools = _importTools.Get( this ); return _importTools.value; }
			set { if( _importTools.BeginSet( this, ref value ) ) { try { ImportToolsChanged?.Invoke( this ); } finally { _importTools.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ImportTools"/> property value changes.</summary>
		public event Action<Product_Windows> ImportToolsChanged;
		ReferenceField<bool> _importTools = false;

		/// <summary>
		/// Whether to include tools that are intended to process environment cubemaps.
		/// </summary>
		[Category( "Windows" )]
		[DefaultValue( false )]
		public Reference<bool> CubemapProcessingTools
		{
			get { if( _cubemapProcessingTools.BeginGet() ) CubemapProcessingTools = _cubemapProcessingTools.Get( this ); return _cubemapProcessingTools.value; }
			set { if( _cubemapProcessingTools.BeginSet( this, ref value ) ) { try { CubemapProcessingToolsChanged?.Invoke( this ); } finally { _cubemapProcessingTools.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CubemapProcessingTools"/> property value changes.</summary>
		public event Action<Product_Windows> CubemapProcessingToolsChanged;
		ReferenceField<bool> _cubemapProcessingTools = false;

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
		////public event Action<Product_Windows> BuildToolsChanged;
		////ReferenceField<bool> _buildTools = false;

		/// <summary>
		/// Whether to include files for debugging (xml, pdb).
		/// </summary>
		[Category( "Windows" )]
		[DefaultValue( false )]
		public Reference<bool> DebugFiles
		{
			get { if( _debugFiles.BeginGet() ) DebugFiles = _debugFiles.Get( this ); return _debugFiles.value; }
			set { if( _debugFiles.BeginSet( this, ref value ) ) { try { DebugFilesChanged?.Invoke( this ); } finally { _debugFiles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugFiles"/> property value changes.</summary>
		public event Action<Product_Windows> DebugFilesChanged;
		ReferenceField<bool> _debugFiles = false;

		/// <summary>
		/// Whether to include files to support UIWebBrowser control.
		/// </summary>
		[Category( "Windows" )]
		[DisplayName( "UIWebBrowser" )]
		[DefaultValue( false )]
		public Reference<bool> UIWebBrowser
		{
			get { if( _uIWebBrowser.BeginGet() ) UIWebBrowser = _uIWebBrowser.Get( this ); return _uIWebBrowser.value; }
			set { if( _uIWebBrowser.BeginSet( this, ref value ) ) { try { UIWebBrowserChanged?.Invoke( this ); } finally { _uIWebBrowser.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UIWebBrowser"/> property value changes.</summary>
		public event Action<Product_Windows> UIWebBrowserChanged;
		ReferenceField<bool> _uIWebBrowser = false;

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
		//public event Action<Product_Windows> SatelliteResourceLanguagesChanged;
		//ReferenceField<bool> _satelliteResourceLanguages = false;

		///// <summary>
		///// Whether to include assemblies from .NET WindowsDesktop configuration (WinForms, WPF).
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> WindowsDesktopAssemblies
		//{
		//	get { if( _windowsDesktopAssemblies.BeginGet() ) WindowsDesktopAssemblies = _windowsDesktopAssemblies.Get( this ); return _windowsDesktopAssemblies.value; }
		//	set { if( _windowsDesktopAssemblies.BeginSet( this, ref value ) ) { try { WindowsDesktopAssembliesChanged?.Invoke( this ); } finally { _windowsDesktopAssemblies.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="WindowsDesktopAssemblies"/> property value changes.</summary>
		//public event Action<Product_Windows> WindowsDesktopAssembliesChanged;
		//ReferenceField<bool> _windowsDesktopAssemblies = false;

		/////////////////////////////////////////

		public enum ProfileEnum
		{
			x64,
			//ARM64,
		}

		/////////////////////////////////////////

		public override SystemSettings.Platform Platform
		{
			get { return SystemSettings.Platform.Windows; }
		}

		public override void BuildFunction( ProductBuildInstance buildInstance )
		{
			try
			{
				BuildProjects( buildInstance, new Range( 0, 0.5 ) );

				var paths = GetPaths();
				var obfuscate = Obfuscate.Value;
				CopyIncludeExcludePaths( paths, buildInstance, new Range( 0.5, 0.9 ) ); //new Range( 0, obfuscate ? 0.9 : 0.99 ) );

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

			//post build event
			if( !PeformPostBuild( buildInstance ) )
				return;
			if( CheckCancel( buildInstance ) )
				return;

			//done
			buildInstance.Progress = 1;
			buildInstance.State = ProductBuildInstance.StateEnum.Success;

			//run
			if( buildInstance.Run )
			{
				string executableFileName = Path.Combine( buildInstance.DestinationFolder, "Binaries", ExecutableName + ".exe" );
				Process.Start( new ProcessStartInfo( executableFileName, "" ) { UseShellExecute = true } );
			}

			if( CheckCancel( buildInstance ) )
				return;

			ShowSuccessScreenNotification();
		}

		[Browsable( false )]
		public override bool SupportsBuildAndRun
		{
			get { return true; }
		}

		void BuildProjects( ProductBuildInstance buildInstance, Range progressRange )
		{
			var dotnetExePath = Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Platforms\Windows\dotnet\dotnet.exe" );

			var destinationFolder = Path.Combine( buildInstance.DestinationFolder, "Binaries" );
			Directory.CreateDirectory( destinationFolder );

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

			var configuration = Configuration.Value.ToString();
			var verbosity = VerbosityLevel.Value.ToString().ToLower();

			buildInstance.Progress = (float)progressRange.Minimum;

			//build assembly for compiling scripts
			if( CompilingScripts )
			{
				buildInstance.ProgressText = "Building CompileScripts assembly...";

				var projectFullPath = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.Core.CompileScripts\NeoAxis.Core.CompileScripts.csproj" );
				var arguments = $"build \"{projectFullPath}\" --configuration {configuration} --output \"{destinationFolder}\" --verbosity {verbosity}";

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

				//var success = ProcessUtility.RunAndWait( dotnetExePath, arguments, out var result ) == 0;
				//if( !success )
				//{
				//	throw new Exception( $"Unable to compile project.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log." );
				//}
			}

			buildInstance.Progress = (float)MathEx.Lerp( progressRange.Minimum, progressRange.Maximum, 0.33 );

			//build Project assembly
			{
				buildInstance.ProgressText = "Building Project assembly...";

				var projectFullPath = Path.Combine( VirtualFileSystem.Directories.Project, @"Project.csproj" );
				var arguments = $"build \"{projectFullPath}\" --configuration {configuration} --output \"{destinationFolder}\" --verbosity {verbosity}";

				var defineConstants = DefineConstants.Value.Trim();
				if( !string.IsNullOrEmpty( defineConstants ) )
					arguments += $" -p:DefineConstants=\"{defineConstants}\"";

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

				//var success = ProcessUtility.RunAndWait( dotnetExePath, arguments, out var result ) == 0;
				//if( !success )
				//{
				//	throw new Exception( $"Unable to compile project.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log." );
				//}
			}

			buildInstance.Progress = (float)MathEx.Lerp( progressRange.Minimum, progressRange.Maximum, 0.66 );

			//build Player assembly
			{
				buildInstance.ProgressText = "Building Player assembly...";

				var projectFullPath = Path.Combine( VirtualFileSystem.Directories.Project, @"Sources\NeoAxis.Player\NeoAxis.Player.csproj" );
				var arguments = $"build \"{projectFullPath}\" --configuration {configuration}-Windows-{Profile.Value} --output \"{destinationFolder}\" --verbosity {verbosity}";

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

				//var success = ProcessUtility.RunAndWait( dotnetExePath, arguments, out var result ) == 0;
				//if( !success )
				//{
				//	throw new Exception( $"Unable to compile project.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log." );
				//}
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
			if( UIWebBrowser )
				paths.Add( @"Binaries\NeoAxis.Addon.UIWebBrowser.dll" );

			//exclude from Binaries
			{
				var sourceFolder = Path.Combine( VirtualFileSystem.Directories.Project, "Binaries" );

				var excludePaths = new List<string>();
				excludePaths.AddRange( GetPlatformsExcludePaths() );

				//if( !Editor )
				{
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tips" ) );
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Localization" ) );
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\PlatformTools" ) );
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\Obfuscar" ) );

					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.Editor.dll" ) );
					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.Editor.deps.json" ) );
					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.dll" ) );
					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.exe" ) );
					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.exe.config" ) );
					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.deps.json" ) );
					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.runtimeconfig.dev.json" ) );
					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.runtimeconfig.json" ) );
				}

				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.exe" ) );
				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.exe.config" ) );
				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.deps.json" ) );
				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.runtimeconfig.dev.json" ) );
				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.runtimeconfig.json" ) );

				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.exe" ) );
				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.exe.config" ) );
				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.deps.json" ) );
				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.runtimeconfig.dev.json" ) );
				//excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.runtimeconfig.json" ) );

				//excludePaths.Add( Path.Combine( sourceFolder, "_TestPlayerParameters.cmd" ) );

				if( !CompilingScripts ) //if( !Editor && !CompilingScripts )
				{
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\dotnet" ) );

					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.CompileScripts.dll" ) );
					//excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.CompileScripts.deps.json" ) );
				}

				////if( !BuildTools )
				////{
				////	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\BuildTools" ) );
				////	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\Framework" ) );
				////}

				if( !UIWebBrowser )
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\CefGlue" ) );

				if( !ImportTools )
				{
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\FbxWrapperNative.dll" ) );
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\assimp-vc143-mt.dll" ) );
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\CommandLineTools" ) );
				}

				if( !CubemapProcessingTools )
					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\Filament" ) );

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

				//if( !WindowsDesktopAssemblies )
				//{
				//	excludePaths.Add( Path.Combine( sourceFolder, "Accessibility.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "D3DCompiler_47_cor3.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "DirectWriteForwarder.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "Microsoft.Win32.Registry.AccessControl.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "Microsoft.Win32.SystemEvents.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "Microsoft.WindowsDesktop.App.deps.json" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "Microsoft.WindowsDesktop.App.runtimeconfig.json" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PenImc_cor3.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationCore.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemCore.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemData.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemDrawing.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemXml.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework-SystemXmlLinq.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Aero.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Aero2.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.AeroLite.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Classic.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Luna.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationFramework.Royale.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationNative_cor3.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "PresentationUI.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "ReachFramework.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.CodeDom.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Configuration.ConfigurationManager.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Design.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Diagnostics.EventLog.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Diagnostics.PerformanceCounter.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.DirectoryServices.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Drawing.Common.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Drawing.Design.dll" ) );
				//	//need//excludePaths.Add( Path.Combine( sourceFolder, "System.Drawing.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.IO.Packaging.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Printing.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Resources.Extensions.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Security.Cryptography.Pkcs.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Security.Cryptography.ProtectedData.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Security.Cryptography.Xml.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Security.Permissions.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Threading.AccessControl.dll" ) );

				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Controls.Ribbon.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Extensions.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Forms.Design.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Forms.Design.Editors.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Forms.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Input.Manipulations.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Windows.Presentation.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "System.Xaml.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "UIAutomationClient.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "UIAutomationClientSideProviders.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "UIAutomationProvider.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "UIAutomationTypes.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "vcruntime140_cor3.dll" ) );
				//	//need//excludePaths.Add( Path.Combine( sourceFolder, "WindowsBase.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "WindowsFormsIntegration.dll" ) );
				//	excludePaths.Add( Path.Combine( sourceFolder, "wpfgfx_cor3.dll" ) );
				//}

				foreach( var excludePath in excludePaths )
					paths.Add( "exclude:" + excludePath );
			}
		}










		//old:

		//public override void BuildFunction( ProductBuildInstance buildInstance )
		//{
		//	//copy files
		//	try
		//	{
		//		var paths = GetPaths();
		//		var obfuscate = Obfuscate.Value;
		//		CopyIncludeExcludePaths( paths, buildInstance, new Range( 0, obfuscate ? 0.9 : 0.99 ) );

		//		//obfuscate
		//		if( Obfuscate )
		//			DoObfuscation( buildInstance, new Range( 0.9, 0.99 ) );
		//	}
		//	catch( Exception e )
		//	{
		//		buildInstance.Error = e.Message;
		//		buildInstance.State = ProductBuildInstance.StateEnum.Error;
		//		return;
		//	}

		//	//List<string> folders = new List<string>();
		//	//folders.Add( "Assets" );
		//	//folders.Add( "Binaries" );
		//	//folders.Add( "Caches" );

		//	////copy files
		//	//try
		//	//{
		//	//	var percentStep = 0.99 / folders.Count;
		//	//	var currentPercent = 0.0;

		//	//	for( int nFolder = 0; nFolder < folders.Count; nFolder++ )
		//	//	{
		//	//		var folder = folders[ nFolder ];
		//	//		var sourceFolder = Path.Combine( VirtualFileSystem.Directories.Project, folder );
		//	//		var destFolder = Path.Combine( buildInstance.DestinationFolder, folder );

		//	//		//!!!!проценты от размера папки
		//	//		var percentRange = new Range( currentPercent, currentPercent + percentStep );

		//	//		bool skipDefaultBehavior = false;

		//	//		var assetsExcludePaths = new List<string>();
		//	//		{
		//	//			assetsExcludePaths.Add( Path.Combine( sourceFolder, @"Base\Tools" ) );
		//	//			assetsExcludePaths.Add( Path.Combine( sourceFolder, @"Base\Learning" ) );

		//	//			var excluded = ExcludedAssets.Value.Trim();
		//	//			if( !string.IsNullOrEmpty( excluded ) )
		//	//			{
		//	//				foreach( var v in excluded.Split( new char[] { ';', '\r', '\n' } ) )
		//	//				{
		//	//					var v2 = v.Trim( ' ', '\t' );
		//	//					if( !string.IsNullOrEmpty( v2 ) )
		//	//						assetsExcludePaths.Add( Path.Combine( sourceFolder, v2 ) );
		//	//				}
		//	//			}
		//	//		}

		//	//		if( folder == "Assets" )
		//	//		{
		//	//			var values = IncludedAssets.Value.Trim();
		//	//			if( !string.IsNullOrEmpty( values ) )
		//	//			{
		//	//				foreach( var v in values.Split( new char[] { ';', '\r', '\n' } ) )
		//	//				{
		//	//					var v2 = v.Trim( ' ', '\t' );
		//	//					if( !string.IsNullOrEmpty( v2 ) )
		//	//					{
		//	//						var sourceFolder2 = Path.Combine( VirtualFileSystem.Directories.Project, folder, v2 );
		//	//						var destFolder2 = Path.Combine( buildInstance.DestinationFolder, folder, v2 );
		//	//						var percentRange2 = new Range( percentRange.Minimum, percentRange.Minimum );

		//	//						CopyFolder( sourceFolder2, destFolder2, buildInstance, percentRange2, assetsExcludePaths );

		//	//						if( CheckCancel( buildInstance ) )
		//	//							return;
		//	//					}
		//	//				}

		//	//				skipDefaultBehavior = true;
		//	//			}
		//	//		}

		//	//		if( !skipDefaultBehavior )
		//	//		{
		//	//			var excludePaths = new List<string>();
		//	//			if( folder == "Binaries" )
		//	//			{
		//	//				excludePaths.AddRange( GetPlatformsExcludePaths() );

		//	//				excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tips" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Localization" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\PlatformTools" ) );

		//	//				excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.exe" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.exe.config" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.deps.json" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.runtimeconfig.dev.json" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Editor.runtimeconfig.json" ) );

		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.exe" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.exe.config" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.deps.json" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.runtimeconfig.dev.json" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWinForms.runtimeconfig.json" ) );

		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.exe" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.exe.config" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.deps.json" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.runtimeconfig.dev.json" ) );
		//	//				excludePaths.Add( Path.Combine( sourceFolder, "SampleWidgetWPF.runtimeconfig.json" ) );

		//	//				excludePaths.Add( Path.Combine( sourceFolder, "_TestPlayerParameters.cmd" ) );

		//	//				excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\dotnet" ) );
		//	//				//if( !BuildTools )
		//	//				//{
		//	//				//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\BuildTools" ) );
		//	//				//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\Framework" ) );
		//	//				//}

		//	//				if( !UIWebBrowser )
		//	//					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\CefGlue" ) );

		//	//				if( !DebugFiles )
		//	//				{
		//	//					foreach( var fileName in Directory.GetFiles( sourceFolder, "*.pdb", SearchOption.TopDirectoryOnly ) )
		//	//						excludePaths.Add( fileName );
		//	//					foreach( var fileName in Directory.GetFiles( sourceFolder, "*.xml", SearchOption.TopDirectoryOnly ) )
		//	//						excludePaths.Add( fileName );
		//	//					foreach( var fileName in Directory.GetFiles( sourceFolder, "*.mdb", SearchOption.TopDirectoryOnly ) )
		//	//						excludePaths.Add( fileName );
		//	//				}

		//	//				if( !ImportTools )
		//	//				{
		//	//					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\FbxWrapperNative.dll" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\assimp-vc141-mt.dll" ) );
		//	//				}

		//	//				if( !CubemapProcessingTools )
		//	//					excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\Filament" ) );

		//	//				if( !SatelliteResourceLanguages )
		//	//				{
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "cs" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "de" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "es" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "fr" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "it" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "ja" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "ko" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "pl" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "pt-BR" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "ru" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "tr" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "zh-Hans" ) );
		//	//					excludePaths.Add( Path.Combine( sourceFolder, "zh-Hant" ) );
		//	//				}

		//	//			}
		//	//			if( folder == "Caches" )
		//	//			{
		//	//				if( !ShaderCache )
		//	//					excludePaths.Add( @"Caches\ShaderCache" );
		//	//				if( !FileCache )
		//	//					excludePaths.Add( @"Caches\Files" );
		//	//			}
		//	//			if( folder == "Assets" )
		//	//				excludePaths.AddRange( assetsExcludePaths );

		//	//			CopyFolder( sourceFolder, destFolder, buildInstance, percentRange, excludePaths );

		//	//			if( CheckCancel( buildInstance ) )
		//	//				return;
		//	//		}

		//	//		currentPercent += percentStep;
		//	//	}
		//	//}
		//	//catch( Exception e )
		//	//{
		//	//	buildInstance.Error = e.Message;
		//	//	buildInstance.State = ProductBuildInstance.StateEnum.Error;
		//	//	return;
		//	//}

		//	////write Run.cmd
		//	//try
		//	//{
		//	//	string fileName = Path.Combine( buildInstance.DestinationFolder, "Run.cmd" );
		//	//	File.WriteAllText( fileName, Path.Combine( @"Binaries", ExecutableName + ".exe" ) );
		//	//}
		//	//catch( Exception e )
		//	//{
		//	//	buildInstance.Error = e.Message;
		//	//	buildInstance.State = ProductBuildInstance.StateEnum.Error;
		//	//	return;
		//	//}

		//	////write License.cert
		//	//if( EngineApp.IsProPlan )
		//	//{
		//	//	var realFileName = Path.Combine( buildInstance.DestinationFolder, "License.cert" );

		//	//	var date = DateTime.UtcNow;
		//	//	//add 10 years
		//	//	date = date.Add( new TimeSpan( 365 * 10, 0, 0, 0, 0 ) );

		//	//	if( LoginUtility.GetCurrentLicense( out var email, out _ ) )
		//	//	{
		//	//		if( !LoginUtility.SaveLicenseCertificate( realFileName, email, EngineInfo.Version, EngineApp.License, "", date, out string error ) )
		//	//		{
		//	//		}
		//	//	}
		//	//}

		//	//string destBinariesDirectory = Path.Combine( buildInstance.DestinationFolder, "Files" );

		//	////copy files
		//	//try
		//	//{
		//	//	var excludePaths = GetPlatformsExcludePaths();
		//	//	CopyFolder( VirtualFileSystem.Directories.Binaries, destBinariesDirectory, buildInstance, new Range( 0, 0.99 ), excludePaths );
		//	//}
		//	//catch( Exception e )
		//	//{
		//	//	buildInstance.Error = e.Message;
		//	//	buildInstance.State = ProductBuildInstance.StateEnum.Error;
		//	//	return;
		//	//}

		//	//post build event
		//	if( !PeformPostBuild( buildInstance ) )
		//		return;
		//	if( CheckCancel( buildInstance ) )
		//		return;

		//	//done
		//	buildInstance.Progress = 1;
		//	buildInstance.State = ProductBuildInstance.StateEnum.Success;

		//	//run
		//	if( buildInstance.Run )
		//	{
		//		string executableFileName = Path.Combine( buildInstance.DestinationFolder, "Binaries", ExecutableName + ".exe" );
		//		Process.Start( new ProcessStartInfo( executableFileName, "" ) { UseShellExecute = true } );
		//		//Process.Start( executableFileName, "" );
		//	}

		//	if( CheckCancel( buildInstance ) )
		//		return;

		//	ShowSuccessScreenNotification();
		//}

		//[Browsable( false )]
		//public override bool SupportsBuildAndRun
		//{
		//	get { return true; }
		//}

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

		//		if( !Editor )
		//		{
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tips" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Localization" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\PlatformTools" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\Obfuscar" ) );

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

		//		if( !Editor && !CompilingScripts )
		//		{
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\dotnet" ) );

		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.CompileScripts.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, "NeoAxis.Core.CompileScripts.deps.json" ) );
		//		}

		//		//if( !BuildTools )
		//		//{
		//		//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\BuildTools" ) );
		//		//	excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Tools\Framework" ) );
		//		//}

		//		if( !UIWebBrowser )
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\CefGlue" ) );

		//		if( !DebugFiles )
		//		{
		//			foreach( var fileName in Directory.GetFiles( sourceFolder, "*.pdb", SearchOption.TopDirectoryOnly ) )
		//				excludePaths.Add( fileName );
		//			foreach( var fileName in Directory.GetFiles( sourceFolder, "*.xml", SearchOption.TopDirectoryOnly ) )
		//				excludePaths.Add( fileName );
		//			foreach( var fileName in Directory.GetFiles( sourceFolder, "*.mdb", SearchOption.TopDirectoryOnly ) )
		//				excludePaths.Add( fileName );
		//		}

		//		if( !ImportTools )
		//		{
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\FbxWrapperNative.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\assimp-vc143-mt.dll" ) );
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\CommandLineTools" ) );
		//		}

		//		if( !CubemapProcessingTools )
		//			excludePaths.Add( Path.Combine( sourceFolder, @"NeoAxis.Internal\Platforms\Windows\Filament" ) );

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

		//		if( !WindowsDesktopAssemblies )
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
