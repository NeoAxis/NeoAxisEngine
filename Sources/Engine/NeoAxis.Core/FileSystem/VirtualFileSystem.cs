// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Reflection;
using Internal;
using Internal.SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Class for the implementation of the virtual file system.
	/// </summary>
	public static class VirtualFileSystem
	{
		static internal bool initialized;
		static Thread mainThread;
		static TextBlock defaultSettingsConfig;
		static bool loggingFileOperations;

		static bool neoAxisCoreNativeLoaded;

		//internal static object lockObject = new object();

		//!!!!это чуть другое, чем 'user', т.к. 'user' - это что-то вроде конвертации из real в virtual.
		//тут же источники. кароч лучше на потом додумать
		//class SourceType;

		//!!!!
		////file redirection
		//static Dictionary<string, string> fileRedirections;

		//!!!!!
		////!!!!лучше кешировать на месте?
		////cache files
		//static ESet<string> fileTypesThatCanBeCached = new ESet<string>();
		//static Dictionary<string, byte[]> cachedVirtualFiles = new Dictionary<string, byte[]>();

		//!!!!!!было
		//static bool deployed;
		//static DeploymentParametersClass deploymentParameters;

		//!!!!!
		////Key: path in lower case
		//internal static Dictionary<string, PreloadFileToMemoryItem> preloadedFilesToMemory = new Dictionary<string, PreloadFileToMemoryItem>();

		///////////////////////////////////////////

		/// <summary>
		/// Provides files paths to engine and project folders.
		/// </summary>
		public static class Directories
		{
			internal static string project;
			internal static string assets;
			internal static string userSettings;
			internal static string binaries;
			internal static string engineInternal;
			internal static string platformSpecific;
			internal static string allFiles;

			//

			/// <summary>
			/// The full path to project folder. It is one level upper more than Assets folder.
			/// </summary>
			public static string Project
			{
				get { return project; }
			}

			/// <summary>
			/// The full path to Assets folder.
			/// </summary>
			public static string Assets
			{
				get { return assets; }
			}

			/// <summary>
			/// The full path to updatable user settings and logs.
			/// </summary>
			public static string UserSettings
			{
				get { return userSettings; }
			}

			/// <summary>
			/// The full path to managed binaries.
			/// </summary>
			public static string Binaries
			{
				get { return binaries; }
			}

			/// <summary>
			/// The full path to internal data and native binaries on some platforms.
			/// </summary>
			public static string EngineInternal
			{
				get { return engineInternal; }
			}

			/// <summary>
			/// The full path to native binaries of current platform.
			/// </summary>
			public static string PlatformSpecific
			{
				get { return platformSpecific; }
			}

			/// <summary>
			/// The full path to top folder, it includes Project and Sources folders. It is one level upper more than Project folder and two levels upper more than Assets folder.
			/// </summary>
			public static string AllFiles
			{
				get { return allFiles; }
			}
		}

		///////////////////////////////////////////

		///// <summary>
		///// Represents an item of preloadable file to memory.
		///// </summary>
		//public class PreloadFileToMemoryItem
		//{
		//	internal string path;
		//	volatile internal bool loaded;
		//	volatile internal string error = "";
		//	volatile internal byte[] data;

		//	public string Path
		//	{
		//		get { return path; }
		//	}

		//	public bool Loaded
		//	{
		//		get { return loaded; }
		//	}

		//	public string Error
		//	{
		//		get { return error; }
		//	}

		//	public byte[] Data
		//	{
		//		get { return data; }
		//	}
		//}

		///////////////////////////////////////////

		//!!!!это чуть другое, чем 'user', т.к. 'user' - это что-то вроде конвертации из real в virtual.
		//тут же источники. кароч лучше на потом додумать
		//public class SourceType
		//{
		//	string name;
		//	internal GetValueDelegate getValueFunction;
		//}

		///////////////////////////////////////////

		//!!!!
		public static string MakePathRelative( string path )
		{
			if( path.StartsWith( Directories.Binaries ) )
				path = path.Replace( Directories.Binaries, "" );

			if( path.StartsWith( "\\" ) )
				path = path.Remove( 0, 1 );
			return path;
		}

		public static bool Init( string logFileName, bool setCurrentDirectory, string projectDirectory, string userSettingsDirectory, string overrideBinariesDirectory = null )
		{
			if( initialized )
			{
				Log.Fatal( "VirtualFileSystem: Init: File system is already initialized." );
				return false;
			}

			//it can be already started
			StartupTiming.TotalStart();

			StartupTiming.CounterStart( "Initialize virtual file system" );
			try
			{

				mainThread = Thread.CurrentThread;

				//init directories
				{
					//project directory
					if( string.IsNullOrEmpty( projectDirectory ) )
						Log.Fatal( "VirtualFileSystem: Init: Project directory must be specified." );
					if( !Directory.Exists( projectDirectory ) )
						Log.Fatal( "VirtualFileSystem: Init: Specified project directory is not exists." );
					projectDirectory = VirtualPathUtility.NormalizePath( projectDirectory );
					Directories.project = projectDirectory;
					if( !Path.IsPathRooted( Directories.project ) )
					{
						Log.Fatal( "VirtualFileSystem: Init: Project directory path must be rooted." );
						return false;
					}

					//assets directory
					Directories.assets = Path.Combine( Directories.project, "Assets" );
					//if( string.IsNullOrEmpty( Directories.assets ) )
					//	Log.Fatal( "VirtualFileSystem: Init: Project Assets directory must be specified." );
					//if( !Directory.Exists( Directories.assets ) )
					//	Log.Fatal( "VirtualFileSystem: Init: Specified project Assets directory is not exists." );

					//user settings directory
					if( string.IsNullOrEmpty( userSettingsDirectory ) )
						Log.Fatal( "VirtualFileSystem: Init: User settings directory must be specified." );
					if( !Path.IsPathRooted( userSettingsDirectory ) )
					{
						Log.Fatal( "VirtualFileSystem: Init: User settings directory path must be rooted." );
						return false;
					}
					userSettingsDirectory = VirtualPathUtility.NormalizePath( userSettingsDirectory );
					Directories.userSettings = userSettingsDirectory;

					//binaries directory
					if( !string.IsNullOrEmpty( overrideBinariesDirectory ) && !Directory.Exists( overrideBinariesDirectory ) )
						Log.Fatal( "VirtualFileSystem: Init: Specified binaries directory is not exists." );
					if( string.IsNullOrEmpty( overrideBinariesDirectory ) )
						Directories.binaries = PlatformSpecificUtility.Instance.GetExecutableDirectoryPath();
					else
						Directories.binaries = overrideBinariesDirectory;
					Directories.binaries = VirtualPathUtility.NormalizePath( Directories.binaries );
					if( !Path.IsPathRooted( Directories.binaries ) )
					{
						Log.Fatal( "VirtualFileSystem: Init: Executables directory path must be rooted." );
						return false;
					}

					//engine internal directory
					//// UWP works with relative paths.
					// should Directories.engineInternal and Directories.platformSpecific be relative for UWP ?
					//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					//	Directories.engineInternal = "NeoAxis.Internal";
					//else
					Directories.engineInternal = Path.Combine( Directories.binaries, "NeoAxis.Internal" );

					//platform specific directory
					Directories.platformSpecific = Path.Combine( Directories.engineInternal, Path.Combine( "Platforms", SystemSettings.CurrentPlatform.ToString() ) );

					//!!!!
					//!!!!ARM
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
						Directories.platformSpecific = Path.Combine( Directories.platformSpecific, "x64" );

					//!!!!ARM
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux )
						Directories.platformSpecific = Path.Combine( Directories.platformSpecific, "x64" );

					Directories.allFiles = Path.GetDirectoryName( Directories.Project );
				}

				//!!!!new
				CultureInfo.CurrentCulture = new CultureInfo( "en-US" );
				try
				{
					//!!!! deprecated. use CultureInfo.CurrentCulture https://github.com/dotnet/platform-compat/blob/master/docs/DE0008.md
					Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-US" );
				}
				catch { }

				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
				//bool monoRuntime = Type.GetType( "Mono.Runtime", false ) != null;
				//if( monoRuntime )
				//	AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

				if( setCurrentDirectory )
					CorrectCurrentDirectory();

				var fatalOnLoadingCoreNative = SystemSettings.CurrentPlatform != SystemSettings.Platform.Linux;
				neoAxisCoreNativeLoaded = NativeUtility.PreloadLibrary( "libNeoAxisCoreNative", errorFatal: fatalOnLoadingCoreNative ) != IntPtr.Zero;
				//on some system native libraries always loaded
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android  || SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS  || SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
					neoAxisCoreNativeLoaded = true;

				InitDefaultSettingsConfig();

				ArchiveManager.Init();
				//if( !ArchiveManager.Init() )
				//{
				//	//ArchiveManager.Shutdown();
				//	return false;
				//}

				initialized = true;

				//InitDeploymentInfoAndUserDirectory();

				string realPath = null;
				if( !string.IsNullOrEmpty( logFileName ) )
					realPath = VirtualPathUtility.GetRealPathByVirtual( logFileName );
				Log.Init( Thread.CurrentThread, realPath );

				//!!!!
				//InitFileTypesThatCanBeCached();

				ResourceManager.Init();
				//RegisterAssemblies_IncludingFromDefaultSettingConfig();
				ParseSettingsFromDefaultSettingsConfig();
				//ResourceTypes.Init();

				//!!!!тут?
				VirtualFileWatcher.Init();

				ResourceUpdate.Init();

			}
			finally
			{
				StartupTiming.CounterEnd( "Initialize virtual file system" );
			}

			return true;
		}

		public static void Shutdown()
		{
			ResourceUpdate.Shutdown();

			//!!!!!еще раз. тут?
			ResourceManager.Shutdown();

			VirtualFileWatcher.Shutdown();
			CSharpProjectFileUtility.Shutdown();

			//!!!!!
			//PackageManager.Shutdown();
			//ResourceTypes.Shutdown();
			initialized = false;
		}

		static Assembly CurrentDomain_AssemblyResolve( object sender, ResolveEventArgs args )
		{
			//ReflectionOnlyAssemblyResolve event is also exists

			string assemblyName = args.Name;
			if( !string.IsNullOrEmpty( assemblyName ) )
			{
				var baseName = assemblyName.Substring( 0, assemblyName.IndexOf( ',' ) );

				string fileName = baseName + ".dll";
				string fullPath = Path.Combine( Directories.Binaries, fileName );
				if( File.Exists( fullPath ) )
					return AssemblyUtility.LoadAssemblyByRealFileName( fullPath, false, loadWithoutLocking: true );

				////Use Project.Client.dll instead of Project.dll
				//if( baseName == "Project" )
				//{
				//	fileName = baseName + ".Client.dll";
				//	fullPath = Path.Combine( Directories.Binaries, fileName );
				//	if( File.Exists( fullPath ) )
				//		return AssemblyUtility.LoadAssemblyByRealFileName( fullPath, false, loadWithoutLocking: true );
				//}
			}

			return null;
		}

		////for Mono Runtime
		//static Assembly CurrentDomain_AssemblyResolve( object sender, ResolveEventArgs args )
		//{
		//	string assemblyName = args.Name;
		//	string fileName = assemblyName.Substring( 0, assemblyName.IndexOf( ',' ) ) + ".dll";
		//	string fullPath = Path.Combine( Directories.Executables, fileName );
		//	return AssemblyUtils.LoadAssemblyByRealFileName( fullPath, false );
		//	//return FileSystemAssemblyUtils.LoadAssemblyByFileName( fullPath );
		//}

		/// <summary>
		/// Reset the current directory of the application.
		/// </summary>
		public static void CorrectCurrentDirectory()
		{
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
				SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP ||
				SystemSettings.CurrentPlatform == SystemSettings.Platform.Linux ||
				SystemSettings.CurrentPlatform == SystemSettings.Platform.macOS )
				Directory.SetCurrentDirectory( Directories.Binaries );
		}

		//!!!!!
		//public static void AddFileRedirection( string originalFileName, string newFileName )
		//{
		//	lock( lockObject )
		//	{
		//		if( fileRedirections == null )
		//			fileRedirections = new Dictionary<string, string>();

		//		string correctedOriginalFileName = VirtualPathUtils.NormalizePath( originalFileName ).ToLower();
		//		string correctedNewFileName = VirtualPathUtils.NormalizePath( newFileName );

		//		if( fileRedirections.ContainsKey( correctedOriginalFileName ) )
		//		{
		//			Log.Fatal( "VirtualFileSystem: AddFileRedirection: File redirection " +
		//				"is already exists \"{0}\".", correctedOriginalFileName );
		//		}

		//		fileRedirections.Add( correctedOriginalFileName, correctedNewFileName );
		//	}
		//}

		//!!!!!
		//public static void RemoveFileRedirection( string originalFileName )
		//{
		//	lock( lockObject )
		//	{
		//		string correctedOriginalFileName = VirtualPathUtils.NormalizePath( originalFileName ).ToLower();
		//		if( fileRedirections != null )
		//			fileRedirections.Remove( correctedOriginalFileName );
		//	}
		//}

		//!!!!!
		//internal static string GetRedirectedFileNameInternal( string originalFileName, bool pathAlreadyNormalized )
		//{
		//	lock( lockObject )
		//	{
		//		if( fileRedirections == null )
		//			return originalFileName;

		//		string correctedOriginalFileName = originalFileName;
		//		if( !pathAlreadyNormalized )
		//			correctedOriginalFileName = VirtualPathUtils.NormalizePath( correctedOriginalFileName );
		//		correctedOriginalFileName = correctedOriginalFileName.ToLower();
		//		//string correctedOriginalFileName = VirtualFileSystem.NormalizePath( originalFileName ).ToLower();

		//		string redirectedFileName;
		//		if( !fileRedirections.TryGetValue( correctedOriginalFileName, out redirectedFileName ) )
		//			return originalFileName;
		//		return redirectedFileName;
		//	}
		//}

		//!!!!!
		//public static string GetRedirectedFileName( string originalFileName )
		//{
		//	return GetRedirectedFileNameInternal( originalFileName, false );
		//}

		//!!!!!!
		//static void InitDeploymentInfoAndUserDirectory()
		//{
		//	string userDirectoryName = null;

		//	string configFileName = "Base/Constants/Deployment.config";

		//	deployed = false;

		//	if( VirtualFile.Exists( configFileName ) )
		//	{
		//		deployed = true;

		//		deploymentParameters = new DeploymentParametersClass();

		//		try
		//		{
		//			using( VirtualFileStream stream = VirtualFile.Open( configFileName ) )
		//			{
		//				using( StreamReader reader = new StreamReader( stream ) )
		//				{
		//					while( true )
		//					{
		//						string line = reader.ReadLine();
		//						if( line == null )
		//							break;
		//						line = line.Trim();

		//						if( line == "" )
		//							continue;
		//						if( line.Length >= 2 && line.Substring( 0, 2 ) == "//" )
		//							continue;

		//						int equalIndex = line.IndexOf( '=' );
		//						if( equalIndex != -1 )
		//						{
		//							string name = line.Substring( 0, equalIndex ).Trim();
		//							string value = line.Substring( equalIndex + 1 ).Trim();
		//							if( value != "" )
		//							{
		//								if( name == "userDirectory" )
		//									userDirectoryName = value;
		//								if( name == "defaultLanguage" )
		//									deploymentParameters.defaultLanguage = value;
		//							}
		//						}
		//					}
		//				}
		//			}
		//		}
		//		catch( Exception e )
		//		{
		//			Log.Fatal( "VirtualFileSystem: Loading file failed {0} ({1}).",
		//				configFileName, e.Message );
		//			return;
		//		}
		//	}

		//	//set user directory path if not initialized
		//	if( string.IsNullOrEmpty( userDirectoryPath ) )
		//	{
		//		if( !string.IsNullOrEmpty( userDirectoryName ) )
		//		{
		//			string systemDirectory = null;

		//			if( PlatformInfo.Platform == PlatformInfo.Platforms.Windows )
		//			{
		//				systemDirectory = Environment.GetFolderPath(
		//					Environment.SpecialFolder.LocalApplicationData );
		//			}
		//			else if( PlatformInfo.Platform == PlatformInfo.Platforms.MacOSX )
		//			{
		//				systemDirectory = Path.Combine(
		//					Environment.GetFolderPath( Environment.SpecialFolder.Personal ),
		//					"Library/Application Support" );
		//			}
		//			//else if( PlatformInfo.Platform == PlatformInfo.Platforms.Android )
		//			//{
		//			//   //!!!!!!!!temp?
		//			//   userDirectoryPath = Path.Combine( ExecutableDirectoryPath, "UserSettings" );
		//			//}
		//			else
		//			{
		//				Log.Fatal( "VirtualFileSystem: InitDeploymentInfoAndUserDirectory: Unknown platform." );
		//			}

		//			userDirectoryPath = Path.Combine( systemDirectory, userDirectoryName );
		//		}
		//		else
		//		{
		//			userDirectoryPath = Path.Combine( ExecutableDirectoryPath, "UserSettings" );
		//		}
		//	}

		//}

		//!!!!!

		////!!!!!!
		//static void InitFileTypesThatCanBeCached()
		//{
		//	//!!!!!а если менять хотят. по сути можно отслеживалку сделать, тогда вапще париться не о чем.
		//	//!!!!!!или метод для сброса кеша.
		//	fileTypesThatCanBeCached.Add( ".hlsl" );
		//	fileTypesThatCanBeCached.Add( ".shader" );
		//	//!!!!!
		//	//item1 = .shaderBaseExtension

		//	//string configFileName = "Base/Constants/FileSystem.config";
		//	//if( VirtualFile.Exists( configFileName ) )
		//	//{
		//	//	TextBlock block = TextBlockUtils.LoadFromVirtualFile( configFileName );
		//	//	if( block != null )
		//	//	{
		//	//		TextBlock cachingExtensionsBlock = block.FindChild( "cachingExtensions" );
		//	//		if( cachingExtensionsBlock != null )
		//	//		{
		//	//			foreach( TextBlock.Attribute attribute in cachingExtensionsBlock.Attributes )
		//	//			{
		//	//				string extension = attribute.Value;
		//	//				fileTypesThatCanBeCached.Add( extension, 0 );
		//	//			}
		//	//		}
		//	//	}
		//	//}
		//}

		//internal static bool IsFileCanBeCached( string path )
		//{
		//	//lock on top level

		//	if( VirtualPathUtils.IsUserDirectoryPath( path ) )
		//		return false;
		//	return fileTypesThatCanBeCached.Contains( Path.GetExtension( path ) );
		//}

		//internal static byte[] GetVirtualFileDataFromCache( string path )
		//{
		//	//lock on top level

		//	byte[] data;
		//	if( !cachedVirtualFiles.TryGetValue( path, out data ) )
		//		return null;
		//	return data;
		//}

		//internal static void AddVirtualFileToCache( string path, byte[] data )
		//{
		//	//lock on top level

		//	cachedVirtualFiles.Add( path, data );
		//}

		//public static ESet<string> FileTypesThatCanBeCached
		//{
		//	get { return fileTypesThatCanBeCached; }
		//}

		//public static ICollection<string> CachedVirtualFiles
		//{
		//	get { return cachedVirtualFiles.Keys; }
		//}

		//public static bool RemoveCachedFile( string path )
		//{
		//	lock( lockObject )
		//	{
		//		path = VirtualPathUtils.NormalizePath( path );
		//		return cachedVirtualFiles.Remove( path );
		//	}
		//}

		//public static void ClearAllCachedFiles()
		//{
		//	lock( lockObject )
		//	{
		//		cachedVirtualFiles.Clear();
		//	}
		//}

		//!!!!!
		//public static bool Deployed
		//{
		//	get { return deployed; }
		//}
		//public static DeploymentParametersClass DeploymentParameters
		//{
		//	get { return deploymentParameters; }
		//}

		public static bool LoggingFileOperations
		{
			get { return loggingFileOperations; }
			set { loggingFileOperations = value; }
		}

		//!!!!!

		//static void PreloadFileToMemoryFromBackgroundThread_Function( object data )
		//{
		//	PreloadFileToMemoryItem item = (PreloadFileToMemoryItem)data;

		//	try
		//	{
		//		using( VirtualFileStream stream = VirtualFile.Open( item.Path ) )
		//		{
		//			byte[] buffer = new byte[ stream.Length ];

		//			if( stream.Read( buffer, 0, buffer.Length ) != buffer.Length )
		//			{
		//				throw new Exception( "Unable to load all data." );
		//			}

		//			item.data = buffer;
		//			item.loaded = true;
		//		}
		//	}
		//	catch( Exception e )
		//	{
		//		item.error = e.Message;
		//	}
		//}

		//public static PreloadFileToMemoryItem PreloadFileToMemoryFromBackgroundThread( string path )
		//{
		//	lock( lockObject )
		//	{
		//		string pathLowerCase = path.ToLower();

		//		PreloadFileToMemoryItem item;
		//		if( preloadedFilesToMemory.TryGetValue( pathLowerCase, out item ) )
		//			return item;

		//		item = new PreloadFileToMemoryItem();
		//		item.path = path;
		//		preloadedFilesToMemory.Add( pathLowerCase, item );

		//		//start Task
		//		Task task = new Task( PreloadFileToMemoryFromBackgroundThread_Function, item );
		//		task.Start();

		//		return item;
		//	}
		//}

		//public static void UnloadPreloadedFileToMemory( string path )
		//{
		//	lock( lockObject )
		//	{
		//		string pathLowerCase = path.ToLower();
		//		preloadedFilesToMemory.Remove( pathLowerCase );
		//	}
		//}

		//public static void UnloadPreloadedFileToMemory( PreloadFileToMemoryItem item )
		//{
		//	lock( lockObject )
		//	{
		//		UnloadPreloadedFileToMemory( item.Path );
		//	}
		//}

		//!!!!!

		//public static ICollection<Package> Packages
		//{
		//	get { return PackageManager.Packages; }
		//}

		//public static Package GetPackage( string realFileName )
		//{
		//	lock( lockObject )
		//	{
		//		return PackageManager.GetPackage( realFileName );
		//	}
		//}

		//public delegate void PackageLoadingDelegate( string realFileName, bool loadInfoOnly, ref Package implemetation, ref string error );
		//public static event PackageLoadingDelegate PackageLoading;
		//internal static void CallPackageLoading( string realFileName, bool loadInfoOnly, ref Package implemetation, ref string error )
		//{
		//	if( PackageLoading != null )
		//		PackageLoading( realFileName, loadInfoOnly, ref implemetation, ref error );
		//}

		//public delegate void PackageLoadedDelegate( Package package );
		//public static event PackageLoadedDelegate PackageLoaded;
		//internal static void CallPackageLoaded( Package package )
		//{
		//	if( PackageLoaded != null )
		//		PackageLoaded( package );
		//}

		//public static Package.InfoClass LoadPackageInfo( string realFileName, out string error )
		//{
		//	lock( lockObject )
		//	{
		//		return PackageManager.LoadPackageInfo( realFileName, out error );
		//	}
		//}

		//public static Package LoadPackage( string realFileName, out string error )
		//{
		//	lock( lockObject )
		//	{
		//		return PackageManager.LoadPackage( realFileName, out error );
		//	}
		//}

		//!!!!!
		//public void UnloadPackage(Package package)
		//{
		//}

		public static string GetDefaultDefaultSettingsConfigText()
		{
			return "RendererBackend = Noop\r\nSoundSystem = Null\r\nSoundMaxReal2DChannels = 32\r\nSoundMaxReal3DChannels = 100\r\n\r\n//compile Project at start\r\n//ScriptingCompileProjectSolutionAtStartup = False\r\n\r\nAutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInEditor = 300\r\nAutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInSimulation = 300\r\n\r\n// The list of auto load assemblies. The developer can add the code to execute after loading the assembly by means making a class based on NeoAxis.AssemblyUtils.AssemblyRegistration inside the assembly.\r\nAutoLoadAssembly { Name = NeoAxis.Addon.*.dll }\r\n\r\n//already loaded\r\n//AutoLoadAssembly { Name = Project.dll }\r\n\r\nDisableAssemblyRegistration\r\n{\r\n\t// internally referenced\r\n\tAssembly { Name = RoslynPad.Common }\r\n\tAssembly { Name = NuGet.Versioning }\r\n\tAssembly { Name = NuGet.Protocol.Core.Types }\r\n\tAssembly { Name = NuGet.Configuration }\r\n\tAssembly { Name = NuGet.Common }\r\n\tAssembly { Name = NuGet.Packaging }\r\n\tAssembly { Name = NuGet.Packaging.Core }\r\n\tAssembly { Name = NuGet.Packaging.Core.Types }\r\n\tAssembly { Name = NuGet.Frameworks }\r\n\tAssembly { Name = NuGet.Protocol }\r\n\tAssembly { Name = Microsoft.CodeAnalysis.Workspaces }\r\n\tAssembly { Name = Microsoft.CodeAnalysis }\r\n\tAssembly { Name = Roslyn.Services.Editor.UnitTests }\r\n\tAssembly { Name = Microsoft.CodeAnalysis.Features }\r\n\tAssembly { Name = Microsoft.CodeAnalysis.Scripting }\r\n\tAssembly { Name = Microsoft.CodeAnalysis.CSharp }\r\n\tAssembly { Name = ICSharpCode.AvalonEdit }\r\n\tAssembly { Name = RoslynPad.Editor.Windows }\r\n\tAssembly { Name = RoslynPad.Roslyn }\r\n\t\r\n\t// disabled, but can be useful\r\n\tAssembly { Name = System.Windows.Forms }\r\n\tAssembly { Name = System.Security }\r\n\tAssembly { Name = System.Design }\r\n\tAssembly { Name = System.Web }\r\n\tAssembly { Name = System.Transactions }\r\n\tAssembly { Name = System.EnterpriseServices }\r\n\tAssembly { Name = System.Web.ApplicationServices }\r\n\tAssembly { Name = System.Web.Services }\r\n\tAssembly { Name = System.Drawing.Design }\r\n\tAssembly { Name = System.Reflection.Metadata }\r\n\tAssembly { Name = System.Windows.Input.Manipulations }\r\n\tAssembly { Name = UIAutomationTypes }\r\n\tAssembly { Name = PresentationFramework }\r\n\tAssembly { Name = ReachFramework }\r\n\tAssembly { Name = System.Printing }\t\r\n\tAssembly { Name = System.Xml }\r\n\tAssembly { Name = System.Xml.Linq }\r\n\tAssembly { Name = System.Net.Http }\r\n\tAssembly { Name = System.Configuration }\r\n\tAssembly { Name = WindowsBase }\r\n\tAssembly { Name = System.Xaml }\r\n\tAssembly { Name = PresentationCore }\r\n\tAssembly { Name = System.Security.Cryptography.Primitives }\r\n\tAssembly { Name = System.Net.Primitives }\r\n\tAssembly { Name = System.Net.Requests }\r\n\tAssembly { Name = System.Net.WebClient }\r\n\tAssembly { Name = System.Net.WebHeaderCollection }\r\n\tAssembly { Name = System.Security.Principal }\r\n\tAssembly { Name = System.Security.Cryptography.X509Certificates }\r\n\tAssembly { Name = System.Net.Security }\r\n\tAssembly { Name = System.Security.Cryptography.Encoding }\r\n\tAssembly { Name = System.Text.RegularExpressions }\r\n\tAssembly { Name = System.IO.FileSystem }\r\n\tAssembly { Name = System.IO.FileSystem.Watcher }\r\n\tAssembly { Name = System.Private.Uri }\r\n\tAssembly { Name = System.Private.Xml }\r\n\tAssembly { Name = System.ComponentModel.EventBasedAsync }\r\n\tAssembly { Name = Microsoft.CodeAnalysis.CSharp.Features }\r\n\tAssembly { Name = RoslynPad.Roslyn.Windows }\r\n\tAssembly { Name = System.Reactive.Linq }\r\n\tAssembly { Name = System.Reactive.Interfaces }\r\n\tAssembly { Name = System.Linq.Expressions }\r\n\tAssembly { Name = System.Reactive.Core }\r\n\t\r\n\tAssembly { Name = System.Security.Cryptography }\r\n\tAssembly { Name = System.Diagnostics.Process }\r\n\tAssembly { Name = System.Diagnostics.FileVersionInfo }\r\n\t\r\n\t// Android\r\n\tAssembly\r\n\t{\r\n\t\tPlatform = Android\r\n\t\tName = Mono.Android\r\n\t}\r\n\tAssembly { Name = Java.Interop }\r\n\tAssembly\r\n\t{\r\n\t\tPlatform = Android\r\n\t\tName = System.Core\r\n\t}\r\n\tAssembly\r\n\t{\r\n\t\tPlatform = Android\r\n\t\tName = System.Windows.Forms\r\n\t}\r\n}\r\n\r\nDisableNamespaceRegistration\r\n{\r\n\tNamespace { Name = Microsoft.Win32 }\r\n\tNamespace { Name = Microsoft.Win32.SafeHandles }\r\n\tNamespace { Name = Mono }\r\n\tNamespace { Name = System.Deployment.Internal }\r\n\tNamespace { Name = System.Configuration.Assemblies }\r\n\tNamespace { Name = System.Resources }\r\n\tNamespace { Name = System.IO.IsolatedStorage }\r\n\tNamespace { Name = System.Security }\r\n\tNamespace { Name = System.Security.Policy }\r\n\tNamespace { Name = System.Security.Permissions }\r\n\tNamespace { Name = System.Security.AccessControl }\r\n\tNamespace { Name = System.Security.Principal }\r\n\tNamespace { Name = System.Security.Claims }\r\n\tNamespace { Name = System.Security.Cryptography }\r\n\tNamespace { Name = System.Security.Cryptography.X509Certificates }\r\n\tNamespace { Name = System.Threading }\r\n\tNamespace { Name = System.Threading.Tasks }\r\n\tNamespace { Name = System.Threading.Tasks.Sources }\r\n\tNamespace { Name = System.Runtime }\r\n\tNamespace { Name = System.Runtime.Hosting }\r\n\tNamespace { Name = System.Runtime.Versioning }\r\n\tNamespace { Name = System.Runtime.Serialization }\r\n\tNamespace { Name = System.Runtime.Serialization.Formatters }\r\n\tNamespace { Name = System.Runtime.Serialization.Formatters.Binary }\r\n\tNamespace { Name = System.Runtime.Remoting }\r\n\tNamespace { Name = System.Runtime.Remoting.Services }\r\n\tNamespace { Name = System.Runtime.Remoting.Proxies }\r\n\tNamespace { Name = System.Runtime.Remoting.Lifetime }\r\n\tNamespace { Name = System.Runtime.Remoting.Contexts }\r\n\tNamespace { Name = System.Runtime.Remoting.Channels }\r\n\tNamespace { Name = System.Runtime.Remoting.Activation }\r\n\tNamespace { Name = System.Runtime.Remoting.Metadata }\r\n\tNamespace { Name = System.Runtime.Remoting.Metadata.W3cXsd2001 }\r\n\tNamespace { Name = System.Runtime.Remoting.Messaging }\r\n\tNamespace { Name = System.Runtime.ExceptionServices }\r\n\tNamespace { Name = System.Runtime.ConstrainedExecution }\r\n\tNamespace { Name = System.Runtime.InteropServices }\r\n\tNamespace { Name = System.Runtime.InteropServices.WindowsRuntime }\r\n\tNamespace { Name = System.Runtime.InteropServices.Expando }\r\n\tNamespace { Name = System.Runtime.InteropServices.ComTypes }\r\n\tNamespace { Name = System.Runtime.CompilerServices }\t\r\n\tNamespace { Name = System.Reflection }\r\n\tNamespace { Name = System.Reflection.Metadata }\r\n\tNamespace { Name = System.Reflection.Emit }\r\n\tNamespace { Name = System.Globalization }\r\n\tNamespace { Name = System.Diagnostics }\r\n\tNamespace { Name = System.Diagnostics.SymbolStore }\r\n\tNamespace { Name = System.Diagnostics.Tracing }\r\n\tNamespace { Name = System.Diagnostics.Contracts }\r\n\tNamespace { Name = System.Diagnostics.Contracts.Internal }\r\n\tNamespace { Name = System.Diagnostics.CodeAnalysis }\r\n\tNamespace { Name = System.Buffers }\r\n\tNamespace { Name = Internal.SharpBgfx }\r\n\tNamespace { Name = Internal.SharpBgfx.Common }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.Zip }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.Zip.Compression }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.Zip.Compression.Streams }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.Tar }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.Lzw }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.GZip }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.Encryption }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.Core }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.Checksum }\r\n\tNamespace { Name = ICSharpCode.SharpZipLib.BZip2 }\r\n\tNamespace { Name = System.Windows.Input }\r\n\tNamespace { Name = System.Windows.Markup }\r\n\tNamespace { Name = System.Timers }\r\n\tNamespace { Name = System.ComponentModel.Design }\r\n\tNamespace { Name = System.ComponentModel.Design.Serialization }\r\n\tNamespace { Name = System.Text.RegularExpressions }\r\n\tNamespace { Name = System.Security.Authentication }\r\n\tNamespace { Name = System.Security.Authentication.ExtendedProtection }\r\n\tNamespace { Name = System.CodeDom.Compiler }\r\n\tNamespace { Name = System.Net }\r\n\tNamespace { Name = System.Net.Mail }\r\n\tNamespace { Name = System.Net.Mime }\r\n\tNamespace { Name = System.Net.Sockets }\r\n\tNamespace { Name = System.Net.Security }\r\n\tNamespace { Name = System.Net.NetworkInformation }\r\n\tNamespace { Name = System.Net.Cache }\r\n\tNamespace { Name = System.Net.WebSockets }\r\n\tNamespace { Name = System.IO.Compression }\r\n\tNamespace { Name = Newtonsoft.Json }\r\n\tNamespace { Name = Newtonsoft.Json.Serialization }\r\n\tNamespace { Name = Newtonsoft.Json.Schema }\r\n\tNamespace { Name = Newtonsoft.Json.Linq }\r\n\tNamespace { Name = Newtonsoft.Json.Converters }\r\n\tNamespace { Name = Newtonsoft.Json.Bson }\r\n\tNamespace { Name = Internal.Net3dBool }\r\n\tNamespace { Name = Internal.LiteDB }\r\n\tNamespace { Name = Internal.LiteDB.Engine }\r\n\tNamespace { Name = Lidgren.Network }\r\n\tNamespace { Name = Internal.Fbx }\r\n\tNamespace { Name = Internal.Xilium.CefGlue }\r\n\tNamespace { Name = Internal.Xilium.CefGlue.Wrapper }\r\n\tNamespace { Name = Internal.Xilium.CefGlue.Platform.Windows }\r\n\tNamespace { Name = Internal.BulletSharp }\r\n\tNamespace { Name = Internal.BulletSharp.SoftBody }\r\n\tNamespace { Name = Internal.BulletSharp.Math }\r\n\tNamespace { Name = System.Drawing }\r\n\tNamespace { Name = System.Drawing.Printing }\r\n\tNamespace { Name = System.Drawing.Design }\r\n\tNamespace { Name = System.Drawing.Configuration }\r\n\tNamespace { Name = System.Drawing.Text }\r\n\tNamespace { Name = System.Drawing.Imaging }\r\n\tNamespace { Name = System.Drawing.Drawing2D }\r\n\tNamespace { Name = NeoAxis.Import }\r\n\tNamespace { Name = NeoAxis.Properties }\r\n\tNamespace { Name = NeoAxis.OggVorbisTheora }\r\n\tNamespace { Name = NeoAxis.Widget }\r\n\tNamespace { Name = Internal.SharpNav }\r\n\tNamespace { Name = Internal.SharpNav.Pathfinding }\r\n\tNamespace { Name = Internal.SharpNav.IO }\r\n\tNamespace { Name = Internal.SharpNav.IO.Json }\r\n\tNamespace { Name = Internal.SharpNav.IO.Binary }\r\n\tNamespace { Name = Internal.SharpNav.Geometry }\r\n\tNamespace { Name = Internal.SharpNav.Crowds }\r\n\tNamespace { Name = Internal.SharpNav.Collections }\r\n\tNamespace { Name = Internal.SharpNav.Collections.Generic }\r\n\tNamespace { Name = Internal.Assimp }\r\n\tNamespace { Name = Internal.Assimp.Unmanaged }\r\n\tNamespace { Name = Internal.Assimp.Configs }\r\n\tNamespace { Name = Microsoft.Xna.Framework }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Fluids }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Dynamics }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Dynamics.Joints }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Dynamics.Contacts }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Diagnostics }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Controllers }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Content }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Common }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Common.TextureTools }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Common.PolygonManipulation }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Common.PhysicsLogic }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Common.Maths }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Common.Decomposition }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Common.ConvexHull }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Collision }\r\n\tNamespace { Name = Internal.nkast.Aether.Physics2D.Collision.Shapes }\r\n\t\r\n\tNamespace { Name = System.Runtime.DesignerServices }\r\n\tNamespace { Name = Microsoft.VisualBasic }\r\n\tNamespace { Name = Microsoft.CSharp }\r\n\tNamespace { Name = System.Web }\r\n\tNamespace { Name = System.Configuration }\r\n\tNamespace { Name = System.Net.Configuration }\r\n\tNamespace { Name = System.Media }\r\n\tNamespace { Name = System.IO.Ports }\r\n\tNamespace { Name = System.Security.Authentication.ExtendedProtection.Configuration }\r\n\tNamespace { Name = System.CodeDom }\r\n\tNamespace { Name = System.Windows.Forms.VisualStyles }\r\n\tNamespace { Name = System.Windows.Forms.PropertyGridInternal }\r\n\tNamespace { Name = System.Windows.Forms.Design }\r\n\tNamespace { Name = System.Windows.Forms.ComponentModel.Com2Interop }\r\n\tNamespace { Name = System.Windows.Forms.Layout }\r\n\tNamespace { Name = System.Windows.Forms.Automation }\r\n\t\r\n\tNamespace { Name = Internal.Aga.Controls }\r\n\tNamespace { Name = Internal.Aga.Controls.Properties }\r\n\tNamespace { Name = Internal.Aga.Controls.Threading }\r\n\tNamespace { Name = Internal.Aga.Controls.Tree }\r\n\tNamespace { Name = Internal.Aga.Controls.Tree.NodeControls }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Toolkit }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Navigator }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Ribbon }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Workspace }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Docking }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Toolkit.Properties }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Navigator.Properties }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Ribbon.Properties }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Workspace.Properties }\r\n\tNamespace { Name = Internal.ComponentFactory.Krypton.Docking.Properties }\r\n}\r\n\r\nCSharpScriptReferenceAssembly { Name = NeoAxis.Core }\r\nCSharpScriptReferenceAssembly { Name = Project }\r\nCSharpScriptReferenceAssembly { Name = System.Diagnostics.Process }\r\n\r\nCSharpScriptUsingNamespace { Value = System }\r\nCSharpScriptUsingNamespace { Value = System.Text }\r\nCSharpScriptUsingNamespace { Value = System.Collections }\r\nCSharpScriptUsingNamespace { Value = System.Collections.Generic }\r\nCSharpScriptUsingNamespace { Value = System.Linq }\r\nCSharpScriptUsingNamespace { Value = System.IO }\r\nCSharpScriptUsingNamespace { Value = NeoAxis }\r\nCSharpScriptUsingNamespace { Value = NeoAxis.Editor }\r\nCSharpScriptUsingNamespace { Value = Project }\r\n\r\nPredefinedServer\r\n{\r\n\tAddress = localhost\r\n\tPort = 53000\r\n}\r\n";
		}

		static void InitDefaultSettingsConfig()
		{
			if( defaultSettingsConfig == null )
			{
				string realFileName = Path.Combine( Directories.Binaries, "NeoAxis.Internal", "NeoAxis.DefaultSettings.config" );
				if( File.Exists( realFileName ) )
					defaultSettingsConfig = TextBlockUtility.LoadFromRealFile( realFileName );
				else
				{
					//default config if file not found
					var text = GetDefaultDefaultSettingsConfigText();
					defaultSettingsConfig = TextBlock.Parse( text, out var error );
					if( !string.IsNullOrEmpty( error ) )
						Log.Warning( $"VirtualFileSystem: InitDefaultSettingsConfig: Error parsing default settings config: {error}" );

					//Log.Warning( $"VirtualFileSystem: InitDefaultSettingsConfig: \"{realFileName}\" is not exists." );
					//defaultSettingsConfig = new TextBlock();
				}
			}
		}

		public static TextBlock DefaultSettingsConfig
		{
			get { return defaultSettingsConfig; }
			set { defaultSettingsConfig = value; }
		}

		public static void RegisterAssembliesIncludingFromDefaultSettingConfig()
		{
			//NeoAxis.Core.dll
			AssemblyUtility.RegisterAssembly( Assembly.GetExecutingAssembly(), "" );

			//NeoAxis.Core.Editor.dll
			if( EngineApp.IsEditor )
			{
				string fullPath = Path.Combine( Directories.Binaries, "NeoAxis.Core.Editor.dll" );
				AssemblyUtility.LoadAssemblyByRealFileName( fullPath, true );
			}

			//auto load
			foreach( var b in DefaultSettingsConfig.Children )
			{
				if( string.Compare( b.Name, "AutoLoadAssembly", true ) == 0 )
				{
					string name = b.GetAttribute( "Name" );
					if( !string.IsNullOrEmpty( name ) )
					{
						var ext = Path.GetExtension( name ).ToLower();
						if( ext != ".dll" )
							name += ".dll";

						if( name.Contains( "*" ) )
						{
							var files = Directory.GetFiles( Directories.Binaries, name, SearchOption.TopDirectoryOnly );
							foreach( var fullPath in files )
							{
								//skip NeoAxis.CoreExtension.Editor.dll, addons for editor in simulation
								if( !EngineApp.IsEditor && Path.GetFileName( fullPath ).Contains( ".Editor" ) )
									continue;

								////skip addons for editor in simulation
								//if( !EngineApp.IsEditor && Path.GetFileName( fullPath ).Contains( "NeoAxis.Addon.Editor" ) )
								//	continue;

								AssemblyUtility.LoadAssemblyByRealFileName( fullPath, true );
							}
						}
						else
						{
							string fullPath = Path.Combine( Directories.Binaries, name );

							//skip NeoAxis.CoreExtension.Editor.dll, addons for editor in simulation
							if( EngineApp.IsEditor || !Path.GetFileName( fullPath ).Contains( ".Editor" ) )
								AssemblyUtility.LoadAssemblyByRealFileName( fullPath, true );
						}
					}
				}
			}
		}

		static void ParseSettingsFromDefaultSettingsConfig()
		{
			var v = DefaultSettingsConfig.GetAttribute( "RendererBackend" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.RendererBackend = (RendererBackend)Enum.Parse( typeof( RendererBackend ), v );

			v = DefaultSettingsConfig.GetAttribute( "SimulationVSync" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.SimulationVSync = bool.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "SimulationTripleBuffering" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.SimulationTripleBuffering = bool.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "RendererReportDebugToLog" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.RendererReportDebugToLog = (bool)SimpleTypes.ParseValue( typeof( bool ), v );

			v = DefaultSettingsConfig.GetAttribute( "UseShaderCache" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.UseShaderCache = (bool)SimpleTypes.ParseValue( typeof( bool ), v );

			v = DefaultSettingsConfig.GetAttribute( "RenderingScene" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.RenderingScene = (bool)SimpleTypes.ParseValue( typeof( bool ), v );

			//v = DefaultSettingsConfig.GetAttribute( "AnisotropicFiltering" );
			//if( !string.IsNullOrEmpty( v ) )
			//	EngineApp.InitSettings.AnisotropicFiltering = (bool)SimpleTypes.ParseValue( typeof( bool ), v );

			v = DefaultSettingsConfig.GetAttribute( "SoundSystem" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.SoundSystem = v;

			v = DefaultSettingsConfig.GetAttribute( "SoundMaxReal2DChannels" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.SoundMaxReal2DChannels = int.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "SoundMaxReal3DChannels" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.SoundMaxReal3DChannels = int.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "ScriptingCompileProjectSolutionAtStartup" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.ScriptingCompileProjectSolutionAtStartup = bool.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "AutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInEditor" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.AutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInEditor = double.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "AutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInSimulation" );
			if( !string.IsNullOrEmpty( v ) )
				EngineApp.InitSettings.AutoUnloadGpuResourcesNotUsedForLongTimeInSecondsInSimulation = double.Parse( v );
		}

		public static Thread MainThread
		{
			get { return mainThread; }
		}

		//!!!!new
		public static void SetMainThread( Thread value )
		{
			mainThread = value;
		}

		public static bool NeoAxisCoreNativeLoaded
		{
			get { return neoAxisCoreNativeLoaded; }
		}

		//!!!!new commented
		//static void InitCloudProjectInfo()
		//{
		//	var fullPath = Path.Combine( Path.GetDirectoryName( Directories.Project ), "CloudProject.info" );
		//	//var fullPath = Path.Combine( Directories.Project, "CloudProject.info" );
		//	if( File.Exists( fullPath ) )
		//	{
		//		var block = TextBlockUtility.LoadFromRealFile( fullPath );
		//		if( block != null )
		//		{
		//			if( !long.TryParse( block.GetAttribute( "ID" ), out var id ) )
		//			{
		//				Log.Warning( "VirtualFileSystem: InitCloudProjectInfo: Unable to parse project ID from \"CloudProject.info\"." );
		//				return;
		//			}

		//			var name = block.GetAttribute( "Name" );

		//			EngineInfo.SetEngineMode( EngineInfo.EngineModeEnum.CloudClient, new EngineInfo.CloudProjectInfoClass( id, name ) );
		//		}
		//	}
		//}
	}
}
