// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Reflection;
using SharpBgfx;

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

			public static string Project
			{
				get { return project; }
			}

			public static string Assets
			{
				get { return assets; }
			}

			public static string UserSettings
			{
				get { return userSettings; }
			}

			public static string Binaries
			{
				get { return binaries; }
			}

			public static string EngineInternal
			{
				get { return engineInternal; }
			}

			public static string PlatformSpecific
			{
				get { return platformSpecific; }
			}
		}

		///////////////////////////////////////////

		/// <summary>
		/// Represents an item of preloadable file to memory.
		/// </summary>
		public class PreloadFileToMemoryItem
		{
			internal string path;
			volatile internal bool loaded;
			volatile internal string error = "";
			volatile internal byte[] data;

			public string Path
			{
				get { return path; }
			}

			public bool Loaded
			{
				get { return loaded; }
			}

			public string Error
			{
				get { return error; }
			}

			public byte[] Data
			{
				get { return data; }
			}
		}

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

		public static bool Init( string logFileName, bool setCurrentDirectory, string projectDirectory,
			string userSettingsDirectory, string overrideBinariesDirectory = null )
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

					Directories.assets = Path.Combine( Directories.project, "Assets" );
					if( string.IsNullOrEmpty( Directories.assets ) )
						Log.Fatal( "VirtualFileSystem: Init: Project Assets directory must be specified." );
					if( !Directory.Exists( Directories.assets ) )
						Log.Fatal( "VirtualFileSystem: Init: Specified project Assets directory is not exists." );

					if( string.IsNullOrEmpty( userSettingsDirectory ) )
						Log.Fatal( "VirtualFileSystem: Init: User settings directory must be specified." );
					if( !Path.IsPathRooted( userSettingsDirectory ) )
					{
						Log.Fatal( "VirtualFileSystem: Init: User settings directory path must be rooted." );
						return false;
					}
					//if( !Directory.Exists( userSettingsDirectory ) )
					//	Log.Fatal( "VirtualFileSystem: Init: Specified User settijngs directory is not exists." );
					userSettingsDirectory = VirtualPathUtility.NormalizePath( userSettingsDirectory );
					Directories.userSettings = userSettingsDirectory;

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

					//// UWP works with relative paths.
					// should Directories.engineInternal and Directories.platformSpecific be relative for UWP ?
					//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					//	Directories.engineInternal = "NeoAxis.Internal";
					//else
					Directories.engineInternal = Path.Combine( Directories.binaries, "NeoAxis.Internal" );

					Directories.platformSpecific = Path.Combine( Directories.engineInternal, Path.Combine( "Platforms", SystemSettings.CurrentPlatform.ToString() ) );

					//!!!!
					//!!!!ARM
					if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
						Directories.platformSpecific = Path.Combine( Directories.platformSpecific, "x64" );
				}

				//!!!!new
				CultureInfo.CurrentCulture = new CultureInfo( "en-US" );
				try
				{
					//!!!! deprecated. use CultureInfo.CurrentCulture https://github.com/dotnet/platform-compat/blob/master/docs/DE0008.md
					Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-US" );
				}
				catch { }

				//bool monoRuntime = Type.GetType( "Mono.Runtime", false ) != null;
				//if( monoRuntime )
				//	AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

				if( setCurrentDirectory )
					CorrectCurrentDirectory();

				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
					Editor.PackageManager.DeleteFilesAsStartup();

				NativeLibraryManager.PreLoadLibrary( "NeoAxisCoreNative" );

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

			//!!!!!
			//PackageManager.Shutdown();
			//ResourceTypes.Shutdown();
			initialized = false;
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
			//lock( lockObject )
			//{
			if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows ||
				SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
				Directory.SetCurrentDirectory( Directories.Binaries );
			//}
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

		static void InitDefaultSettingsConfig()
		{
			if( defaultSettingsConfig == null )
			{
				string realFileName = Path.Combine( Directories.Binaries, "NeoAxis.DefaultSettings.config" );
				if( File.Exists( realFileName ) )
					defaultSettingsConfig = TextBlockUtility.LoadFromRealFile( realFileName );
				else
				{
					Log.Warning( "VirtualFileSystem: InitDefaultSettingsConfig: \"NeoAxis.DefaultSettings.config\" is not exists." );
					defaultSettingsConfig = new TextBlock();
				}
			}
		}

		public static TextBlock DefaultSettingsConfig
		{
			get { return defaultSettingsConfig; }
			set { defaultSettingsConfig = value; }
		}

		public static void RegisterAssemblies_IncludingFromDefaultSettingConfig()
		{
			//NeoAxis.Core.dll
			AssemblyUtility.RegisterAssembly( Assembly.GetExecutingAssembly(), "" );

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
								AssemblyUtility.LoadAssemblyByRealFileName( fullPath, true );
						}
						else
						{
							string fullPath = Path.Combine( Directories.Binaries, name );
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
				EngineSettings.Init.RendererBackend = (RendererBackend)Enum.Parse( typeof( RendererBackend ), v );

			v = DefaultSettingsConfig.GetAttribute( "SimulationVSync" );
			if( !string.IsNullOrEmpty( v ) )
				EngineSettings.Init.SimulationVSync = bool.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "SimulationTripleBuffering" );
			if( !string.IsNullOrEmpty( v ) )
				EngineSettings.Init.SimulationTripleBuffering = bool.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "RendererReportDebugToLog" );
			if( !string.IsNullOrEmpty( v ) )
				EngineSettings.Init.RendererReportDebugToLog = (bool)SimpleTypes.ParseValue( typeof( bool ), v );

			v = DefaultSettingsConfig.GetAttribute( "UseShaderCache" );
			if( !string.IsNullOrEmpty( v ) )
				EngineSettings.Init.UseShaderCache = (bool)SimpleTypes.ParseValue( typeof( bool ), v );

			v = DefaultSettingsConfig.GetAttribute( "AnisotropicFiltering" );
			if( !string.IsNullOrEmpty( v ) )
				EngineSettings.Init.AnisotropicFiltering = (bool)SimpleTypes.ParseValue( typeof( bool ), v );

			v = DefaultSettingsConfig.GetAttribute( "SoundSystemDLL" );
			if( !string.IsNullOrEmpty( v ) )
				EngineSettings.Init.SoundSystemDLL = v;

			v = DefaultSettingsConfig.GetAttribute( "SoundMaxReal2DChannels" );
			if( !string.IsNullOrEmpty( v ) )
				EngineSettings.Init.SoundMaxReal2DChannels = int.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "SoundMaxReal3DChannels" );
			if( !string.IsNullOrEmpty( v ) )
				EngineSettings.Init.SoundMaxReal3DChannels = int.Parse( v );

			v = DefaultSettingsConfig.GetAttribute( "ScriptingCompileProjectSolutionAtStartup" );
			if( !string.IsNullOrEmpty( v ) )
				EngineSettings.Init.ScriptingCompileProjectSolutionAtStartup = bool.Parse( v );
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
	}
}
