// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY

//using Microsoft.Build.Evaluation;
//using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using System.Linq;
//using System.Reflection;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;
using NeoAxis.Editor;
using System.Xml;
//using System.Linq.Expressions;

namespace NeoAxis
{
	/// <summary>
	/// An auxiliary class for working with C# project files.
	/// </summary>
	static class CSharpProjectFileUtility
	{
		const string projectAndSolutionName = "Project";

		static ESet<string> projectFileCSFiles;
		static ESet<string> projectFileCSFilesFullPaths;
		static List<string> projectFileReferences;

		//change watcher for Project.csproj
		static FileSystemWatcher systemWatcher;
		static bool needResetProjectData;

		static string lastCompilationDirectory = "";

		//const bool useManualParser = true;

		//public static string BuildConfiguration { get; set; }

		/////////////////////////////////////////

		public static void Init()
		{
			//BuildConfiguration = buildConfiguration;

			FileWatcherInit();

			DeleteTemporaryCompilationDirectory();
		}

		public static void Shutdown()
		{
			FileWatcherShutdown();

			DeleteTemporaryCompilationDirectory();
		}

		static void DeleteTemporaryCompilationDirectory()
		{
			try
			{
				var directory = GetProjectTemporaryDirectory( false );
				if( Directory.Exists( directory ) )
					Directory.Delete( directory, true );
			}
			catch { }
		}

		static string GetProjectAndSolutionName( bool clientDll )
		{
			var result = projectAndSolutionName;
			if( clientDll )
				result += ".Client";
			return result;
		}

		///////////////////////////////////////////////

		static string GetProjectCSProjFullPath( bool clientDll )
		{
			return VirtualPathUtility.GetRealPathByVirtual( $"project:{GetProjectAndSolutionName( clientDll )}.csproj" );
		}

		public static bool ProjectCSProjFileExists( bool clientDll )
		{
			return File.Exists( GetProjectCSProjFullPath( clientDll ) );
		}

		public static string GetProjectSlnFullPath( bool clientDll )
		{
			return VirtualPathUtility.GetRealPathByVirtual( $"project:{GetProjectAndSolutionName( clientDll )}.sln" );
		}

		//public static string GetProjectDllFullPath()
		//{
		//	return Path.Combine( VirtualFileSystem.Directories.Binaries, ProjectAndSolutionName + ".dll" );
		//}

		static void FileWatcherInit()
		{
			if( ProjectCSProjFileExists( false ) )
			{
				var path = GetProjectCSProjFullPath( false );
				systemWatcher = new FileSystemWatcher( Path.GetDirectoryName( path ), Path.GetFileName( path ) );
				//systemWatcher.InternalBufferSize = 32768;
				systemWatcher.IncludeSubdirectories = false;
				//systemWatcher.Created += fileSystemWatcher_Event;
				//systemWatcher.Deleted += fileSystemWatcher_Event;
				//systemWatcher.Renamed += fileSystemWatcher_Event;
				systemWatcher.Changed += delegate ( object sender, FileSystemEventArgs e )
				{
					//occurs from different threads
					needResetProjectData = true;
				};
				systemWatcher.EnableRaisingEvents = true;
			}
		}

		static void FileWatcherShutdown()
		{
			systemWatcher?.Dispose();
			systemWatcher = null;
		}

		static void CheckResetProjectData()
		{
			if( needResetProjectData )
			{
				projectFileCSFiles = null;
				projectFileCSFilesFullPaths = null;
				projectFileReferences = null;

				needResetProjectData = false;
			}
		}

		static void GetProjectFileData_ManualParser( bool reload )
		{
			if( projectFileCSFiles == null || reload )
			{
				projectFileCSFiles = new ESet<string>();
				projectFileCSFilesFullPaths = new ESet<string>();
				projectFileReferences = new List<string>();

				if( ProjectCSProjFileExists( false ) )
				{
					try
					{
						var xmldoc = new XmlDocument();
						xmldoc.Load( GetProjectCSProjFullPath( false ) );

						{
							var list = xmldoc.SelectNodes( "//Reference" );
							foreach( XmlNode node in list )
							{
								var include = node.Attributes[ "Include" ].Value;
								projectFileReferences.Add( include );
							}
						}

						{
							var list = xmldoc.SelectNodes( "//PackageReference" );
							foreach( XmlNode node in list )
							{
								var include = node.Attributes[ "Include" ].Value;
								projectFileReferences.Add( include );
							}
						}

						{
							var list = xmldoc.SelectNodes( "//Compile" );
							foreach( XmlNode node in list )
							{
								var include = node.Attributes[ "Include" ].Value;
								projectFileCSFiles.AddWithCheckAlreadyContained( include );
								projectFileCSFilesFullPaths.AddWithCheckAlreadyContained( Path.Combine( VirtualFileSystem.Directories.Project, include ) );
							}
						}

					}
					catch( Exception e )
					{
						Log.Warning( $"Unable to read file \'{GetProjectCSProjFullPath( false )}\'. Error: {e.Message}" );
					}
				}
			}
		}

		static bool UpdateProjectFile_ManualParser( ICollection<string> addFiles, ICollection<string> removeFiles, out bool wasUpdated, out string error )
		{
			wasUpdated = false;
			error = "";

			try
			{
				var xmldoc = new XmlDocument();
				xmldoc.Load( GetProjectCSProjFullPath( false ) );

				if( removeFiles != null )
				{
					var removeFilesSet = new ESet<string>();
					foreach( var fileName in removeFiles )
					{
						var fixedPath = fileName;
						if( Path.IsPathRooted( fixedPath ) )
							fixedPath = fixedPath.Replace( VirtualFileSystem.Directories.Project + Path.DirectorySeparatorChar, "" );

						removeFilesSet.AddWithCheckAlreadyContained( fixedPath );
					}

					var nodesToRemove = new List<XmlNode>();

					var list = xmldoc.SelectNodes( "//Compile" );
					foreach( XmlNode node in list )
					{
						var name = node.Attributes[ "Include" ].Value;

						if( removeFilesSet.Contains( name ) )
						{
							nodesToRemove.Add( node );
							wasUpdated = true;
						}
					}

					foreach( var node in nodesToRemove.GetReverse() )
						node.ParentNode.RemoveChild( node );
				}

				if( addFiles != null )
				{
					XmlNode itemGroupNode = null;
					{
						var list = xmldoc.SelectNodes( "//Compile" );
						foreach( XmlNode node in list )
						{
							itemGroupNode = node.ParentNode;
							break;
						}
					}

					if( itemGroupNode == null )
					{
						XmlNode projectNode = null;
						{
							var list = xmldoc.SelectNodes( "//Project" );
							foreach( XmlNode node in list )
							{
								projectNode = node;
								break;
							}
						}

						if( projectNode == null )
							throw new Exception( "Project node not found." );

						itemGroupNode = xmldoc.CreateNode( XmlNodeType.Element, "ItemGroup", null );
						projectNode.AppendChild( itemGroupNode );
					}

					foreach( var fileName in addFiles )
					{
						var fixedPath = fileName;
						if( Path.IsPathRooted( fixedPath ) )
							fixedPath = fixedPath.Replace( VirtualFileSystem.Directories.Project + Path.DirectorySeparatorChar, "" );

						var node = xmldoc.CreateNode( XmlNodeType.Element, "Compile", null );
						var includeAttribute = xmldoc.CreateAttribute( "Include" );
						includeAttribute.Value = fixedPath;
						node.Attributes.Append( includeAttribute );
						itemGroupNode.AppendChild( node );

						wasUpdated = true;
					}
				}

				if( wasUpdated )
					xmldoc.Save( GetProjectCSProjFullPath( false ) );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}

		public static void CheckToRemoveNotExistsFilesFromProject()
		{
			var notExists = new ESet<string>();
			foreach( var path in GetProjectFileCSFiles( false, true ) )
			{
				if( !File.Exists( path ) )
					notExists.AddWithCheckAlreadyContained( path );
			}

			if( notExists.Count != 0 )
			{
				var text = EditorLocalization.Translate( "General", "Unable to compile Project.csproj. The project contains files which are not exists. Remove these files from the project?" ) + "\r\n";
				int counter = 0;
				foreach( var fullPath in notExists )
				{
					if( counter > 20 )
					{
						text += "\r\n...";
						break;
					}

					var path = fullPath.Replace( VirtualFileSystem.Directories.Project + Path.DirectorySeparatorChar, "" );
					text += "\r\n" + path;
					counter++;
				}

				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
				{
					if( UpdateProjectFile( null, notExists, out var error ) )
					{
						if( notExists.Count > 1 )
							Log.Info( EditorLocalization.Translate( "General", "Items have been removed from the Project.csproj." ) );
						else
							Log.Info( EditorLocalization.Translate( "General", "The item has been removed from the Project.csproj." ) );
					}
					else
						Log.Warning( error );
				}
			}
		}

		public static ESet<string> GetProjectFileCSFiles( bool reload, bool getFullPaths )
		{
			CheckResetProjectData();
			GetProjectFileData_ManualParser( reload );
			return getFullPaths ? projectFileCSFilesFullPaths : projectFileCSFiles;
		}

		public static List<string> GetProjectFileReferences( bool reload )
		{
			CheckResetProjectData();
			GetProjectFileData_ManualParser( reload );
			return projectFileReferences;
		}

		public static bool UpdateProjectFile( ICollection<string> addFiles, ICollection<string> removeFiles, out string error )
		{
			if( !ProjectCSProjFileExists( false ) )
			{
				error = $"Project file is not exists. Path: {GetProjectCSProjFullPath( false )}.";
				return false;
			}

			bool wasUpdated;

			if( !UpdateProjectFile_ManualParser( addFiles, removeFiles, out wasUpdated, out error ) )
				return false;

			if( wasUpdated )
			{
				//refresh cached data
				GetProjectFileCSFiles( true, false );

				//update project for C# editor
				if( EngineApp.IsEditor )
					EditorAssemblyInterface.Instance.UpdateProjectFileForCSharpEditor( addFiles, removeFiles );
			}

			return true;
		}

		static string GetProjectTemporaryDirectory( bool canCreate )
		{
			string directory = Path.Combine( Path.GetTempPath(), "NeoAxisProjectCompile" );
			if( canCreate && !Directory.Exists( directory ) )
				Directory.CreateDirectory( directory );
			return directory;
		}

		static List<string> GetProjectCompilationFileNames( bool clientDll )
		{
			var projectName = GetProjectAndSolutionName( clientDll );

			var files = new List<string>();
			files.Add( projectName + ".dll" );
			files.Add( projectName + ".pdb" );
			files.Add( projectName + ".deps.json" );
			return files;
		}

		static bool CanCompileToLastCompilationDirectory( bool clientDll )
		{
			var outputDirectory = lastCompilationDirectory;
			if( string.IsNullOrEmpty( outputDirectory ) )
				outputDirectory = VirtualFileSystem.Directories.Binaries;

			foreach( var file in GetProjectCompilationFileNames( clientDll ) )
			{
				var path = Path.Combine( outputDirectory, file );
				if( File.Exists( path ) && IOUtility.IsFileLocked( path ) )
					return false;
			}
			return true;
		}

		public static bool Compile( bool clientDll, bool rebuild, out string outputDllFilePath )//, out string error )
		{
			outputDllFilePath = "";
			//error = "";

			//get compilation directory
			if( !CanCompileToLastCompilationDirectory( clientDll ) )
			{
				//update last compilation directory

				try
				{
					var tempDirectory = GetProjectTemporaryDirectory( true );

					string directory;
					do
					{
						var guid = Guid.NewGuid().ToString();
						directory = Path.Combine( tempDirectory, guid );

					} while( Directory.Exists( directory ) );

					lastCompilationDirectory = directory;
				}
				catch
				{
					var error = "Unable to create temporary directory for compilation.";
					Log.Error( error );
					return false;
				}
			}

			//compile

			//can be empty
			var outputDirectoryOptional = lastCompilationDirectory;

			var outputDirectory = lastCompilationDirectory;
			if( string.IsNullOrEmpty( outputDirectory ) )
				outputDirectory = VirtualFileSystem.Directories.Binaries;

			if( !VisualStudioSolutionUtility.BuildSolution( GetProjectSlnFullPath( clientDll ), outputDirectoryOptional, rebuild ) )//, out error ) )
				return false;

			outputDllFilePath = Path.Combine( outputDirectory, GetProjectAndSolutionName( clientDll ) + ".dll" );

			return true;
		}

		//clientDll is always false
		public static void ClearAndCompileIfRequiredAtStart( bool clientDll )
		{
			//editor specific. delete Project.dll, Project.pdb, Project.deps.json
			if( EngineApp.IsEditor )
			{
				foreach( var file in GetProjectCompilationFileNames( clientDll ) )
				{
					var path = Path.Combine( VirtualFileSystem.Directories.Binaries, file );

					if( File.Exists( path ) && !IOUtility.IsFileLocked( path ) )
					{
						try
						{
							File.Delete( path );
						}
						catch { }
					}
				}
			}

			//compile
			if( CompilationIsRequired( clientDll ) )
				Compile( clientDll, false, out _ );
		}

		static List<string> GetAllSolutionFiles( bool clientDll )
		{
			var result = new List<string>();

			var projectName = GetProjectAndSolutionName( clientDll );

			//!!!!simple solution. checking only Project.csproj and only cs files. need to check additional cs projects
			result.Add( Path.Combine( VirtualFileSystem.Directories.Project, projectName + ".sln" ) );
			result.Add( Path.Combine( VirtualFileSystem.Directories.Project, projectName + ".csproj" ) );
			result.AddRange( GetProjectFileCSFiles( false, true ) );

			return result;
		}

		static bool IsOutputUpToDate( IEnumerable<string> inputSolutionFiles, string outputDllFileName )
		{
			try
			{
				var outputDllTime = File.GetLastWriteTime( outputDllFileName );
				foreach( var file in inputSolutionFiles )
				{
					//skip CSharpScripts.cs
					if( file.Contains( PathUtility.NormalizePath( @"Caches\CSharpScripts\CSharpScripts.cs" ) ) )
						continue;

					if( File.GetLastWriteTime( file ) > outputDllTime )
						return false;
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool CompilationIsRequired( bool clientDll )
		{
			var outputDirectory = lastCompilationDirectory;
			if( string.IsNullOrEmpty( outputDirectory ) )
				outputDirectory = VirtualFileSystem.Directories.Binaries;

			var outputDllFilePath = Path.Combine( outputDirectory, GetProjectAndSolutionName( clientDll ) + ".dll" );

			return !File.Exists( outputDllFilePath ) || !IsOutputUpToDate( GetAllSolutionFiles( clientDll ), outputDllFilePath );
		}


		//public static void Init( string name )
		//{
		//	Init( name, VirtualFileSystem.Directories.Project, VirtualFileSystem.Directories.Binaries );
		//}

		//public static void Init( string name, string projectDir, string outputDir )
		//{
		//	Init( name, projectDir, name, outputDir );//, "Release" );
		//}

		//public static void Init( string name, string projectDir, string outputAssemblyName, string outputDir )//, string buildConfiguration )
		//{
		//	Name = name;
		//	ProjectDir = projectDir;
		//	OutputAssemblyName = outputAssemblyName;
		//	OutputDir = outputDir;
		//	//BuildConfiguration = buildConfiguration;

		//	FileWatcherInit();
		//}

		//static IEnumerable<ProjectItem> GetProjectCSFiles( Project project )
		//{
		//	foreach( var item in project.Items )
		//	{
		//		if( item.ItemType == "Compile" && item.UnevaluatedInclude.EndsWith( ".cs" ) )
		//			yield return item;
		//	}
		//}

		//static IEnumerable<string> GetProjectReferences( Project project )
		//{
		//	foreach( var item in project.Items )
		//	{
		//		if( item.ItemType == "Reference" )
		//		{
		//			var name = item.UnevaluatedInclude;
		//			int index = name.IndexOf( ", " );
		//			if( index != -1 )
		//				name = name.Substring( 0, index );

		//			yield return name;
		//		}
		//	}
		//}

		//static ProjectItem GetProjectItemByFileName( Project project, string fileName )
		//{
		//	return GetProjectCSFiles( project ).FirstOrDefault( item => item.UnevaluatedInclude == fileName );
		//}

		//static Dictionary<string, string> GetGlobalProperties( /*string projectPath,*/ string toolsPath )
		//{
		//	//string solutionDir = Path.GetDirectoryName( projectPath );
		//	//string extensionsPath = Path.GetFullPath( Path.Combine( toolsPath, @"..\..\" ) );
		//	//string sdksPath = Path.Combine( extensionsPath, "Sdks" );
		//	string roslynTargetsPath = Path.Combine( toolsPath, "Roslyn" );

		//	//!!!!test
		//	//var sdksPath = @"F:\Dev5\Project\Binaries\NeoAxis.Internal\Platforms\Windows\dotnet\shared";
		//	//var extensionsPath = @"F:\Dev5\Project\Binaries\NeoAxis.Internal\Platforms\Windows\dotnet\shared\Microsoft.WindowsDesktop.App\3.1.6";

		//	return new Dictionary<string, string>
		//	{
		//		//{"UseLegacySdkResolver", "true" },

		//		//{ "SolutionDir", solutionDir },
		//		//{ "MSBuildExtensionsPath", extensionsPath },
		//		//{ "MSBuildSDKsPath", sdksPath },
		//		{ "RoslynTargetsPath", roslynTargetsPath }
		//	};
		//}

		//internal static ProjectCollection CreateProjectCollection()
		//{
		//	//!!!!
		//	//var ss = ToolLocationHelper.GetPathToBuildToolsFile( "msbuild.exe", ToolLocationHelper.CurrentToolsVersion );
		//	//Log.Info( ss );

		//	string toolsPath = VisualStudioSolutionUtility.GetMSBuildFolderPath();
		//	var globalProperties = GetGlobalProperties( VisualStudioSolutionUtility.GetMSBuildFolderPath() );
		//	var projectCollection = new ProjectCollection( globalProperties );
		//	// change toolset to internal.

		//	//var toolsPath2 = ToolLocationHelper.GetPathToBuildTools( ToolLocationHelper.CurrentToolsVersion );
		//	//Log.Info( toolsPath2 );

		//	//!!!!
		//	////projectCollection.AddToolset( new Toolset( "3.1.302", toolsPath, projectCollection, string.Empty ) );
		//	projectCollection.AddToolset( new Toolset( ToolLocationHelper.CurrentToolsVersion, toolsPath, projectCollection, string.Empty ) );
		//	return projectCollection;
		//}

		//!!!!
		//private const string MSBuildSDKsPath = nameof( MSBuildSDKsPath );

		//public static ESet<string> GetProjectFileCSFiles( bool reload, bool getFullPaths )
		//{
		//	CheckResetProjectData();

		//	//if( useManualParser )
		//	//{
		//	GetProjectFileData_ManualParser( reload );
		//	//}
		//	//else
		//	//{
		//	//	if( projectFileCSFiles == null || reload )
		//	//	{
		//	//		projectFileCSFiles = new ESet<string>();
		//	//		projectFileCSFilesFullPaths = new ESet<string>();

		//	//		if( ProjectFileExists() )
		//	//		{
		//	//			//!!!!

		//	//			//var sdksPath = @"F:\Dev5\Project\Binaries\NeoAxis.Internal\Platforms\Windows\dotnet\sdk\3.1.302";

		//	//			//var oldMSBuildSDKsPath = Environment.GetEnvironmentVariable( MSBuildSDKsPath );
		//	//			//Environment.SetEnvironmentVariable( MSBuildSDKsPath, sdksPath );

		//	//			try
		//	//			{
		//	//				using( var projectCollection = CreateProjectCollection() )
		//	//				{
		//	//					Project project = projectCollection.LoadProject( GetProjectFileName() );

		//	//					foreach( var item in GetProjectCSFiles( project ) )
		//	//					{
		//	//						var name = item.UnevaluatedInclude;
		//	//						projectFileCSFiles.AddWithCheckAlreadyContained( name );
		//	//						projectFileCSFilesFullPaths.AddWithCheckAlreadyContained( Path.Combine( ProjectDir, name ) );
		//	//					}
		//	//				}
		//	//			}
		//	//			catch( Exception e )
		//	//			{
		//	//				//!!!!
		//	//				Log.Warning( $"Unable to read file \'{GetProjectFileName()}\'. Error: {e.Message}" );
		//	//			}
		//	//			finally
		//	//			{
		//	//				//!!!!
		//	//				//Environment.SetEnvironmentVariable( MSBuildSDKsPath, oldMSBuildSDKsPath );
		//	//			}
		//	//		}
		//	//	}
		//	//}

		//	return getFullPaths ? projectFileCSFilesFullPaths : projectFileCSFiles;
		//}

		//public static List<string> GetProjectFileReferences( bool reload )
		//{
		//	CheckResetProjectData();

		//	//if( useManualParser )
		//	//{
		//	GetProjectFileData_ManualParser( reload );
		//	//}
		//	//else
		//	//{
		//	//	if( projectFileReferences == null || reload )
		//	//	{
		//	//		projectFileReferences = new List<string>();

		//	//		if( ProjectFileExists() )
		//	//		{
		//	//			try
		//	//			{
		//	//				using( var projectCollection = CreateProjectCollection() )
		//	//				{
		//	//					Project project = projectCollection.LoadProject( GetProjectFileName() );

		//	//					foreach( var item in GetProjectReferences( project ) )
		//	//						projectFileReferences.Add( item );
		//	//				}
		//	//			}
		//	//			catch( Exception e )
		//	//			{
		//	//				//!!!!
		//	//				Log.Warning( $"Unable to read file \'{GetProjectFileName()}\'. Error: {e.Message}" );
		//	//			}
		//	//		}
		//	//	}
		//	//}

		//	return projectFileReferences;
		//}

		////		public static void SaveProject()
		////		{
		////			using( var projectCollection = CreateProjectCollection() )
		////			{
		////				Project project = projectCollection.LoadProject( GetProjectFileName() );
		////				project.Save();
		////			}
		////		}

		//public static bool UpdateProjectFile( ICollection<string> addFiles, ICollection<string> removeFiles, out string error )
		//{
		//	//error = "";

		//	if( !ProjectFileExists() )
		//	{
		//		error = $"Project file is not exists. Path: {GetProjectFileName()}.";
		//		return false;
		//	}

		//	bool wasUpdated;

		//	//if( useManualParser )
		//	//{
		//	if( !UpdateProjectFile_ManualParser( addFiles, removeFiles, out wasUpdated, out error ) )
		//		return false;
		//	//}
		//	//else
		//	//{
		//	//	try
		//	//	{
		//	//		using( var projectCollection = CreateProjectCollection() )
		//	//		{
		//	//			var project = projectCollection.LoadProject( GetProjectFileName() );
		//	//			//var project = new Project( GetProjectFileName() );

		//	//			if( removeFiles != null )
		//	//			{
		//	//				foreach( var fileName in removeFiles )
		//	//				{
		//	//					var fixedPath = fileName;
		//	//					if( Path.IsPathRooted( fixedPath ) )
		//	//						fixedPath = fixedPath.Replace( ProjectDir + Path.DirectorySeparatorChar, "" );

		//	//					var item = GetProjectItemByFileName( project, fixedPath );
		//	//					if( item != null )
		//	//					{
		//	//						project.RemoveItem( item );
		//	//						wasUpdated = true;
		//	//					}

		//	//					//if( item == null )
		//	//					//{
		//	//					//	error = $"Item with name \'{fileName}\' is not found.";
		//	//					//	return false;
		//	//					//}

		//	//					//project.RemoveItem( item );
		//	//				}
		//	//			}

		//	//			if( addFiles != null )
		//	//			{
		//	//				foreach( var fileName in addFiles )
		//	//				{
		//	//					var fixedPath = fileName;
		//	//					if( Path.IsPathRooted( fixedPath ) )
		//	//						fixedPath = fixedPath.Replace( ProjectDir + Path.DirectorySeparatorChar, "" );

		//	//					project.AddItem( "Compile", fixedPath );
		//	//					wasUpdated = true;
		//	//				}
		//	//			}

		//	//			if( wasUpdated )
		//	//				project.Save();
		//	//		}
		//	//	}
		//	//	catch( Exception e )
		//	//	{
		//	//		//!!!!check
		//	//		error = e.Message;
		//	//		return false;
		//	//	}
		//	//}

		//	if( wasUpdated )
		//	{
		//		//refresh cached data
		//		GetProjectFileCSFiles( true, false );

		//		//update project for C# editor
		//		if( EngineApp.IsEditor )
		//			EditorAssemblyInterface.Instance.UpdateProjectFileForCSharpEditor( addFiles, removeFiles );
		//	}

		//	return true;
		//}


		//static IEnumerable<string> GetAllOutputAssemblies( string solutionDir, Func<string, bool> filter )
		//{
		//	return Directory.EnumerateFiles( solutionDir, "*.*", SearchOption.TopDirectoryOnly )
		//		.Select( Path.GetFileName )
		//		.Where( filter );
		//}

		//public static void ClearOutput( Func<string, bool> filter /*= null*/ )
		//{
		//	//if( filter == null )
		//	//	filter = s => s.StartsWith( OutputAssemblyName ) && ( s.EndsWith( ".dll" ) || s.EndsWith( ".pdb" ) );

		//	foreach( var path in GetAllOutputAssemblies( OutputDir, filter ) )
		//	{
		//		if( !IOUtility.IsFileLocked( path ) )
		//			File.Delete( path );
		//	}
		//}

		//public static string CompileIfRequired( bool rebuild, bool clearOutput )
		//{
		//	/*
		//		Compilation algorithm when launching a player or editor:

		//		Clear all Project_#id#.dll (for example when you start the editor)

		//		If for Project.dll compilation is needed (dll outdated or missing):
		//			If Project.dll is locked:
		//				compile and return Project_#id#.dll for loading
		//			else:
		//				compile and return the Project.dll for loading
		//		else:
		//			return Project.dll for loading

		//		!!!! strange effect: 
		//			when you msbuild compile, unused Project_#id#.dll is deleted without any request.
		//			it's ok for us but it's unexpected.
		//	*/

		//	if( clearOutput )  // use regex ?
		//		ClearOutput( s => s.StartsWith( OutputAssemblyName + "_" ) && ( s.EndsWith( ".dll" ) || s.EndsWith( ".pdb" ) ) );

		//	string outputAssemblyName = OutputAssemblyName;

		//	if( rebuild || CompilationIsRequired() )
		//	{
		//		string fullPath = Path.Combine( OutputDir, outputAssemblyName + ".dll" );
		//		if( File.Exists( fullPath ) && IOUtility.IsFileLocked( fullPath ) )
		//			outputAssemblyName = Name + "_" + Process.GetCurrentProcess().Id;

		//		Compile( rebuild, outputAssemblyName );
		//	}

		//	return outputAssemblyName;
		//}

		///// <summary>
		///// Compile assembly
		///// </summary>
		///// <param name="rebuild"></param>
		///// <param name="outputAssemblyNameOverride"></param>
		///// <returns></returns>
		//public static bool Compile( bool rebuild, string outputAssemblyNameOverride = null, string outputDirOverride = null )
		//{
		//	var config = new VisualStudioSolutionUtility.BuildConfig();

		//	config.BuildConfiguration = "Release";
		//	//config.BuildConfiguration = ProjectSettings.Get.CSharpEditorBuildConfiguration;
		//	//config.BuildConfiguration = BuildConfiguration;

		//	config.OutputDirectory = outputDirOverride ?? OutputDir;
		//	config.OutputAssemblyName = outputAssemblyNameOverride ?? OutputAssemblyName;

		//	if( IOUtility.IsFileLocked( Path.Combine( config.OutputDirectory, config.OutputAssemblyName + ".dll" ) ) )
		//	{
		//		Log.Error( $"\'{config.OutputAssemblyName}.dll\' compilation skipped because output file is locked." );
		//		return false; // throw exception or warn message ?
		//	}

		//	if( !VisualStudioSolutionUtility.BuildSolution( ProjectDir, Name, config, rebuild ) )
		//		return false;

		//	return true;
		//}

		//static IEnumerable<string> GetAllSolutionFiles( string solutionDir )
		//{
		//	//!!!!не добавленные не добавлять

		//	//TODO: use Roslyn and parse Project to check real project items.
		//	// simple solution:
		//	return Directory.EnumerateFiles( solutionDir, "*.*", SearchOption.AllDirectories )
		//		.Where( s => s.EndsWith( ".sln" ) || s.EndsWith( ".csproj" ) || s.EndsWith( ".cs" ) );
		//}

		//public static bool CompilationIsRequired()
		//{
		//	var outputAssembly = Path.Combine( OutputDir, OutputAssemblyName + ".dll" );
		//	if( !File.Exists( outputAssembly ) )
		//		return true;

		//	return !IsOutputUpToDate( GetAllSolutionFiles( ProjectDir ), outputAssembly );
		//}

		//static bool IsOutputUpToDate( IEnumerable<string> inputSolutionFiles, string outputAssembly )
		//{
		//	////!!!!
		//	//return false;
		//	var lastSolutionWriteTime = inputSolutionFiles.Max( f => File.GetLastWriteTime( f ) );
		//	//Debug.WriteLine( "LastWriteFile: " + inputSolutionFiles.OrderByDescending(
		//	//	f => File.GetLastWriteTime( f ) ).FirstOrDefault() );
		//	return File.GetLastWriteTime( outputAssembly ) > lastSolutionWriteTime;
		//}

		//internal static string GetProjectTempDir()
		//{
		//	string tempDir = Path.Combine( Path.GetTempPath(), "NeoAxisProjectTempDir" );
		//	if( !Directory.Exists( tempDir ) )
		//		Directory.CreateDirectory( tempDir );
		//	return tempDir;
		//}


	}
}

#else

using System;
using System.Collections.Generic;

namespace NeoAxis
{
	static class CSharpProjectFileUtility
	{
		public static void Init()
		{
		}

		public static void Shutdown()
		{
		}

		public static bool UpdateProjectFile( ICollection<string> addFiles, ICollection<string> removeFiles, out string error )
		{
			error = "";
			return false;
		}

		public static ESet<string> GetProjectFileCSFiles( bool reload, bool getFullPaths )
		{
			return new ESet<string>();
		}
	}
}

#endif
