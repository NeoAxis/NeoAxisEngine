// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY

using Microsoft.Build.Evaluation;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeoAxis.Editor;
using System.Xml;
using System.Linq.Expressions;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for working with C# project files.
	/// </summary>
	public static class CSharpProjectFileUtility
	{
		static bool useManualParser = true;

		/////////////////////////////////////////

		public static string Name { get; set; } // Project and Solution Name.
		public static string ProjectDir { get; set; }
		public static string OutputAssemblyName { get; set; }
		public static string OutputDir { get; set; }
		//public static string BuildConfiguration { get; set; }

		public static ESet<string> projectFileCSFiles;
		public static ESet<string> projectFileCSFilesFullPaths;
		public static List<string> projectFileReferences;

		/////////////////////////////////////////

		static FileSystemWatcher systemWatcher;
		static bool needResetProjectData;

		/////////////////////////////////////////

		public static void Init( string name )
		{
			Init( name, VirtualFileSystem.Directories.Project, VirtualFileSystem.Directories.Binaries );
		}

		public static void Init( string name, string projectDir, string outputDir )
		{
			Init( name, projectDir, name, outputDir );//, "Release" );
		}

		public static void Init( string name, string projectDir, string outputAssemblyName, string outputDir )//, string buildConfiguration )
		{
			Name = name;
			ProjectDir = projectDir;
			OutputAssemblyName = outputAssemblyName;
			OutputDir = outputDir;
			//BuildConfiguration = buildConfiguration;

			CSharpProjectFileUtility.FileWatcherInit();
		}

		static IEnumerable<ProjectItem> GetProjectCSFiles( Project project )
		{
			foreach( var item in project.Items )
			{
				if( item.ItemType == "Compile" && item.UnevaluatedInclude.EndsWith( ".cs" ) )
					yield return item;
			}
		}

		static IEnumerable<string> GetProjectReferences( Project project )
		{
			foreach( var item in project.Items )
			{
				if( item.ItemType == "Reference" )
				{
					var name = item.UnevaluatedInclude;
					int index = name.IndexOf( ", " );
					if( index != -1 )
						name = name.Substring( 0, index );

					yield return name;
				}
			}
		}

		static ProjectItem GetProjectItemByFileName( Project project, string fileName )
		{
			return GetProjectCSFiles( project ).FirstOrDefault( item => item.UnevaluatedInclude == fileName );
		}

		static string GetProjectFileName()
		{
			return VirtualPathUtility.GetRealPathByVirtual( $"project:{Name}.csproj" );
		}

		public static bool ProjectFileExists()
		{
			return File.Exists( GetProjectFileName() );
		}


		static Dictionary<string, string> GetGlobalProperties( /*string projectPath,*/ string toolsPath )
		{
			//string solutionDir = Path.GetDirectoryName( projectPath );
			//string extensionsPath = Path.GetFullPath( Path.Combine( toolsPath, @"..\..\" ) );
			//string sdksPath = Path.Combine( extensionsPath, "Sdks" );
			string roslynTargetsPath = Path.Combine( toolsPath, "Roslyn" );

			//!!!!test
			//var sdksPath = @"F:\Dev5\Project\Binaries\NeoAxis.Internal\Platforms\Windows\dotnet\shared";
			//var extensionsPath = @"F:\Dev5\Project\Binaries\NeoAxis.Internal\Platforms\Windows\dotnet\shared\Microsoft.WindowsDesktop.App\3.1.6";

			return new Dictionary<string, string>
			{
				//{"UseLegacySdkResolver", "true" },

				//{ "SolutionDir", solutionDir },
				//{ "MSBuildExtensionsPath", extensionsPath },
				//{ "MSBuildSDKsPath", sdksPath },
				{ "RoslynTargetsPath", roslynTargetsPath }
			};
		}

		internal static ProjectCollection CreateProjectCollection()
		{
			//!!!!
			//var ss = ToolLocationHelper.GetPathToBuildToolsFile( "msbuild.exe", ToolLocationHelper.CurrentToolsVersion );
			//Log.Info( ss );

			string toolsPath = VisualStudioSolutionUtility.GetMSBuildFolderPath();
			var globalProperties = GetGlobalProperties( VisualStudioSolutionUtility.GetMSBuildFolderPath() );
			var projectCollection = new ProjectCollection( globalProperties );
			// change toolset to internal.

			//var toolsPath2 = ToolLocationHelper.GetPathToBuildTools( ToolLocationHelper.CurrentToolsVersion );
			//Log.Info( toolsPath2 );

			//!!!!
			////projectCollection.AddToolset( new Toolset( "3.1.302", toolsPath, projectCollection, string.Empty ) );
			projectCollection.AddToolset( new Toolset( ToolLocationHelper.CurrentToolsVersion, toolsPath, projectCollection, string.Empty ) );
			return projectCollection;
		}

		//!!!!
		//private const string MSBuildSDKsPath = nameof( MSBuildSDKsPath );

		public static ESet<string> GetProjectFileCSFiles( bool reload, bool getFullPaths )
		{
			CheckResetProjectData();

			if( useManualParser )
			{
				GetProjectFileData_ManualParser( reload );
			}
			else
			{
				if( projectFileCSFiles == null || reload )
				{
					projectFileCSFiles = new ESet<string>();
					projectFileCSFilesFullPaths = new ESet<string>();

					if( ProjectFileExists() )
					{
						//!!!!

						//var sdksPath = @"F:\Dev5\Project\Binaries\NeoAxis.Internal\Platforms\Windows\dotnet\sdk\3.1.302";

						//var oldMSBuildSDKsPath = Environment.GetEnvironmentVariable( MSBuildSDKsPath );
						//Environment.SetEnvironmentVariable( MSBuildSDKsPath, sdksPath );

						try
						{
							using( var projectCollection = CreateProjectCollection() )
							{
								Project project = projectCollection.LoadProject( GetProjectFileName() );

								foreach( var item in GetProjectCSFiles( project ) )
								{
									var name = item.UnevaluatedInclude;
									projectFileCSFiles.AddWithCheckAlreadyContained( name );
									projectFileCSFilesFullPaths.AddWithCheckAlreadyContained( Path.Combine( ProjectDir, name ) );
								}
							}
						}
						catch( Exception e )
						{
							//!!!!
							Log.Warning( $"Unable to read file \'{GetProjectFileName()}\'. Error: {e.Message}" );
						}
						finally
						{
							//!!!!
							//Environment.SetEnvironmentVariable( MSBuildSDKsPath, oldMSBuildSDKsPath );
						}
					}
				}
			}

			return getFullPaths ? projectFileCSFilesFullPaths : projectFileCSFiles;
		}

		public static List<string> GetProjectFileReferences( bool reload )
		{
			CheckResetProjectData();

			if( useManualParser )
			{
				GetProjectFileData_ManualParser( reload );
			}
			else
			{
				if( projectFileReferences == null || reload )
				{
					projectFileReferences = new List<string>();

					if( ProjectFileExists() )
					{
						try
						{
							using( var projectCollection = CreateProjectCollection() )
							{
								Project project = projectCollection.LoadProject( GetProjectFileName() );

								foreach( var item in GetProjectReferences( project ) )
									projectFileReferences.Add( item );
							}
						}
						catch( Exception e )
						{
							//!!!!
							Log.Warning( $"Unable to read file \'{GetProjectFileName()}\'. Error: {e.Message}" );
						}
					}
				}
			}

			return projectFileReferences;
		}

		//		public static void SaveProject()
		//		{
		//			using( var projectCollection = CreateProjectCollection() )
		//			{
		//				Project project = projectCollection.LoadProject( GetProjectFileName() );
		//				project.Save();
		//			}
		//		}

		public static bool UpdateProjectFile( ICollection<string> addFiles, ICollection<string> removeFiles, out string error )
		{
			error = "";

			if( !ProjectFileExists() )
			{
				error = $"Project file is not exists. Path: {GetProjectFileName()}.";
				return false;
			}

			bool wasUpdated = false;

			if( useManualParser )
			{
				if( !UpdateProjectFile_ManualParser( addFiles, removeFiles, out wasUpdated, out error ) )
					return false;
			}
			else
			{
				try
				{
					using( var projectCollection = CreateProjectCollection() )
					{
						var project = projectCollection.LoadProject( GetProjectFileName() );
						//var project = new Project( GetProjectFileName() );

						if( removeFiles != null )
						{
							foreach( var fileName in removeFiles )
							{
								var fixedPath = fileName;
								if( Path.IsPathRooted( fixedPath ) )
									fixedPath = fixedPath.Replace( ProjectDir + Path.DirectorySeparatorChar, "" );

								var item = GetProjectItemByFileName( project, fixedPath );
								if( item != null )
								{
									project.RemoveItem( item );
									wasUpdated = true;
								}

								//if( item == null )
								//{
								//	error = $"Item with name \'{fileName}\' is not found.";
								//	return false;
								//}

								//project.RemoveItem( item );
							}
						}

						if( addFiles != null )
						{
							foreach( var fileName in addFiles )
							{
								var fixedPath = fileName;
								if( Path.IsPathRooted( fixedPath ) )
									fixedPath = fixedPath.Replace( ProjectDir + Path.DirectorySeparatorChar, "" );

								project.AddItem( "Compile", fixedPath );
								wasUpdated = true;
							}
						}

						if( wasUpdated )
							project.Save();
					}
				}
				catch( Exception e )
				{
					//!!!!check
					error = e.Message;
					return false;
				}
			}

			if( wasUpdated )
			{
				//refresh cached data
				GetProjectFileCSFiles( true, false );

				//update project for C# editor
				if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
					EditorAssemblyInterface.Instance.UpdateProjectFileForCSharpEditor( addFiles, removeFiles );
			}

			return true;
		}

		/////////////////////////////////////////

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

					var path = fullPath.Replace( ProjectDir + Path.DirectorySeparatorChar, "" );
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

		/// <summary>
		/// Compile assembly with unique name (if locked). or skip compilation if up to date.
		/// </summary>
		/// <param name="rebuild"></param>
		/// <returns>unique name if default assembly locked</returns>
		public static string CompileIfRequired( bool rebuild, bool clearOutput )
		{
			/*
				Compilation algorithm when launching a player or editor:

				Clear all Project_#id#.dll (for example when you start the editor)

				If for Project.dll compilation is needed (dll outdated or missing):
					If Project.dll is locked:
						compile and return Project_#id#.dll for loading
					else:
						compile and return the Project.dll for loading
				else:
					return Project.dll for loading

				!!!! strange effect: 
					when you msbuild compile, unused Project_#id#.dll is deleted without any request.
					it's ok for us but it's unexpected.
			*/

			if( clearOutput )  // use regex ?
				ClearOutput( s => s.StartsWith( OutputAssemblyName + "_" ) && ( s.EndsWith( ".dll" ) || s.EndsWith( ".pdb" ) ) );

			string outputAssemblyName = OutputAssemblyName;

			if( rebuild || CompilationIsRequired() )
			{
				string fullPath = Path.Combine( OutputDir, outputAssemblyName + ".dll" );
				if( File.Exists( fullPath ) && IOUtility.IsFileLocked( fullPath ) )
					outputAssemblyName = Name + "_" + Process.GetCurrentProcess().Id;

				Compile( rebuild, outputAssemblyName );
			}

			return outputAssemblyName;
		}

		/// <summary>
		/// Compile assembly
		/// </summary>
		/// <param name="rebuild"></param>
		/// <param name="outputAssemblyNameOverride"></param>
		/// <returns></returns>
		public static bool Compile( bool rebuild, string outputAssemblyNameOverride = null, string outputDirOverride = null )
		{
			var config = new VisualStudioSolutionUtility.BuildConfig();

			config.BuildConfiguration = "Release";
			//config.BuildConfiguration = ProjectSettings.Get.CSharpEditorBuildConfiguration;
			//config.BuildConfiguration = BuildConfiguration;

			config.OutDir = outputDirOverride ?? OutputDir;
			config.OutputAssemblyName = outputAssemblyNameOverride ?? OutputAssemblyName;

			if( IOUtility.IsFileLocked( Path.Combine( config.OutDir, config.OutputAssemblyName + ".dll" ) ) )
			{
				Log.Error( $"\'{config.OutputAssemblyName}.dll\' compilation skipped because output file is locked." );
				return false; // throw exception or warn message ?
			}

			return VisualStudioSolutionUtility.BuildSolution( ProjectDir, Name, config, rebuild );
		}

		public static void ClearOutput( Func<string, bool> filter /*= null*/ )
		{
			//if( filter == null )
			//	filter = s => s.StartsWith( OutputAssemblyName ) && ( s.EndsWith( ".dll" ) || s.EndsWith( ".pdb" ) );

			foreach( var path in GetAllOutputAssemblies( OutputDir, filter ) )
			{
				if( !IOUtility.IsFileLocked( path ) )
					File.Delete( path );
			}
		}

		public static bool CompilationIsRequired()
		{
			var outputAssembly = Path.Combine( OutputDir, OutputAssemblyName + ".dll" );
			if( !File.Exists( outputAssembly ) )
				return true;

			return !IsOutputUpToDate( GetAllSolutionFiles( ProjectDir ), outputAssembly );
		}

		static bool IsOutputUpToDate( IEnumerable<string> inputSolutionFiles, string outputAssembly )
		{
			////!!!!
			//return false;
			var lastSolutionWriteTime = inputSolutionFiles.Max( f => File.GetLastWriteTime( f ) );
			//Debug.WriteLine( "LastWriteFile: " + inputSolutionFiles.OrderByDescending(
			//	f => File.GetLastWriteTime( f ) ).FirstOrDefault() );
			return File.GetLastWriteTime( outputAssembly ) > lastSolutionWriteTime;
		}

		static IEnumerable<string> GetAllSolutionFiles( string solutionDir )
		{
			//TODO: use Roslyn and parse Project to check real project items.
			// simple solution:
			return Directory.EnumerateFiles( solutionDir, "*.*", SearchOption.AllDirectories )
						.Where( s => s.EndsWith( ".sln" ) || s.EndsWith( ".csproj" ) || s.EndsWith( ".cs" ) );
		}

		static IEnumerable<string> GetAllOutputAssemblies( string solutionDir, Func<string, bool> filter )
		{
			return Directory.EnumerateFiles( solutionDir, "*.*", SearchOption.TopDirectoryOnly )
				.Select( Path.GetFileName )
				.Where( filter );
		}

		internal static string GetProjectTempDir()
		{
			string tempDir = Path.Combine( Path.GetTempPath(), "NeoAxisProjectTempDir" );
			if( !Directory.Exists( tempDir ) )
				Directory.CreateDirectory( tempDir );
			return tempDir;
		}

		///////////////////////////////////////////////

		static void GetProjectFileData_ManualParser( bool reload )
		{
			if( projectFileCSFiles == null || reload )
			{
				projectFileCSFiles = new ESet<string>();
				projectFileCSFilesFullPaths = new ESet<string>();
				projectFileReferences = new List<string>();

				if( ProjectFileExists() )
				{
					try
					{
						var xmldoc = new XmlDocument();
						xmldoc.Load( GetProjectFileName() );

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
								projectFileCSFilesFullPaths.AddWithCheckAlreadyContained( Path.Combine( ProjectDir, include ) );
							}
						}

					}
					catch( Exception e )
					{
						Log.Warning( $"Unable to read file \'{GetProjectFileName()}\'. Error: {e.Message}" );
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
				xmldoc.Load( GetProjectFileName() );

				if( removeFiles != null )
				{
					var removeFilesSet = new ESet<string>();
					foreach( var fileName in removeFiles )
					{
						var fixedPath = fileName;
						if( Path.IsPathRooted( fixedPath ) )
							fixedPath = fixedPath.Replace( ProjectDir + Path.DirectorySeparatorChar, "" );

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
							fixedPath = fixedPath.Replace( ProjectDir + Path.DirectorySeparatorChar, "" );

						var node = xmldoc.CreateNode( XmlNodeType.Element, "Compile", null );
						var includeAttribute = xmldoc.CreateAttribute( "Include" );
						includeAttribute.Value = fixedPath;
						node.Attributes.Append( includeAttribute );
						itemGroupNode.AppendChild( node );

						wasUpdated = true;
					}
				}

				if( wasUpdated )
					xmldoc.Save( GetProjectFileName() );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}

		///////////////////////////////////////////////

		static void FileWatcherInit()
		{
			if( ProjectFileExists() )
			{
				var path = GetProjectFileName();
				systemWatcher = new FileSystemWatcher( Path.GetDirectoryName( path ), Path.GetFileName( path ) );
				//systemWatcher.InternalBufferSize = 32768;
				systemWatcher.IncludeSubdirectories = false;
				//systemWatcher.Created += fileSystemWatcher_Event;
				//systemWatcher.Deleted += fileSystemWatcher_Event;
				//systemWatcher.Renamed += fileSystemWatcher_Event;
				systemWatcher.Changed += fileSystemWatcher_Event;
				systemWatcher.EnableRaisingEvents = true;
			}
		}

		internal static void FileWatcherShutdown()
		{
			systemWatcher?.Dispose();
			systemWatcher = null;
		}

		static void fileSystemWatcher_Event( object sender, FileSystemEventArgs e )
		{
			//occurs from different threads
			needResetProjectData = true;
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

	}
}

#else

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeoAxis.Editor;

namespace NeoAxis
{
	public static class CSharpProjectFileUtility
	{
		public static bool UpdateProjectFile( ICollection<string> addFiles, ICollection<string> removeFiles, out string error )
		{
			error = "";
			return false;
		}

		public static ESet<string> GetProjectFileCSFiles( bool reload, bool getFullPaths )
		{
			return new ESet<string>();
		}

		static void FileWatcherInit()
		{
		}

		internal static void FileWatcherShutdown()
		{
		}
	}
}

#endif
