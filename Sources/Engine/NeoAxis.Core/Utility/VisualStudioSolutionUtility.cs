#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Internal;

namespace NeoAxis
{
	/// <summary>
	/// A utility class to compile Visual Studio solution files.
	/// </summary>
	static class VisualStudioSolutionUtility
	{
		const string BuildConfiguration = "Release";
		const string Verbosity = "minimal";

		//

		public static bool BuildSolution( string solutionFullPath, string outputDirectoryOptional, bool rebuild )//, out string error )
		{
			//error = "";

			var solutionFileName = Path.GetFileName( solutionFullPath );

			string arguments;
			{
				//!!!!юзает последнюю версию фрейморка. например Preview
				//[-r | --runtime<RUNTIME_IDENTIFIER>]

				var builder = new StringBuilder();
				builder.Append( "build" );
				builder.Append( $" \"{solutionFullPath}\"" );

				builder.Append( $" --configuration {BuildConfiguration}" );
				builder.Append( $" --framework netcoreapp3.1" );

				if( !string.IsNullOrEmpty( outputDirectoryOptional ) )
					builder.Append( $" --output \"{outputDirectoryOptional}\"" );

				builder.Append( $" --verbosity {Verbosity}" );

				//!!!!optional? с опцией если нет файла, то ошибка при загрузке редактора
				//builder.Append( " --no-restore" );

				if( rebuild )
					builder.Append( " --no-incremental" );

				arguments = builder.ToString();
			}

			var dotnetExePath = Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Platforms\Windows\dotnet\dotnet.exe" );

			var success = ProcessUtility.RunAndWait( dotnetExePath, arguments, out var result ) == 0;
			result = result.Trim( new char[] { '\r', '\n' } );

			if( success )
			{
				//!!!!get compilation info?

				Log.Info( $"\'{solutionFileName}\' was built successfully." );
				return true;
			}
			else
			{
				if( EngineApp.IsSimulation )
				{
					var error = $"Unable to compile solution {solutionFileName}. Continue?\r\n\r\n{result}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log.";

					if( LogPlatformFunctionality.Instance.ShowMessageBox( error, "Error", EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
					{
						Process.GetCurrentProcess().Kill();
						Environment.Exit( 0 );
					}
				}
				else
				{
					var error = $"Unable to compile solution {solutionFileName}.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{dotnetExePath} {arguments}\r\n\r\nSee details in the log.";

					Log.Error( error );
				}

				return false;
			}
		}


		///// <summary>
		///// Compilation settings of a solution.
		///// </summary>
		//public class BuildConfig
		//{
		//	public string Verbosity = "minimal";
		//	public string BuildPlatform = "Any CPU";
		//	public string BuildConfiguration = "Release";

		//	//public bool CreateAppxPackage;
		//	//public string CreateAppxBundle = "Always";
		//	//public bool AppxPackageSigningEnabled;
		//	//public string AppxBundlePlatforms = "x64";

		//	zzzzzz;
		//	public string OutputDirectory;
		//	public string OutputAssemblyName;
		//}

		//static string GetDotNetExecutablePath()
		//{
		//	return Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Platforms\Windows\dotnet\dotnet.exe" );
		//}

		//public static string GetMSBuildFolderPath()
		//{
		//	//const bool InternalMsBuild = true;
		//	//if( InternalMsBuild )
		//	//{

		//	//!!!!

		//	//string path = Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Platforms\Windows\dotnet\sdk" );
		//	string path = Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Platforms\Windows\dotnet\sdk\3.1.302" );
		//	//string path = Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Tools\BuildTools\MSBuild\15.0\Bin" );

		//	if( !Directory.Exists( path ) )
		//	{
		//		Log.Fatal( $"Could not locate the MSBuild tools. Directory is not exists \'{path}\'." );
		//		//throw new Exception( "Could not locate the tools (MSBuild) path." );
		//	}
		//	return path;
		//	//}
		//	//else
		//	//{
		//	//	// installed tools search:

		//	//	string toolsPath = "";//ToolLocationHelper.GetPathToBuildToolsFile( "msbuild.exe", ToolLocationHelper.CurrentToolsVersion );
		//	//	if( string.IsNullOrEmpty( toolsPath ) )
		//	//	{
		//	//		toolsPath = PollForToolsPath().FirstOrDefault();
		//	//	}
		//	//	if( string.IsNullOrEmpty( toolsPath ) )
		//	//	{
		//	//		throw new Exception( "Could not locate the tools (MSBuild) path." );
		//	//	}
		//	//	return Path.GetDirectoryName( toolsPath );
		//	//}
		//}

		//static string GetTargetFrameworkRootPath()
		//{
		//	return Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Tools\Framework" );
		//}

		//static bool ExecuteMSBuild( string args, out string result )
		//{
		//	var msBuildExePath = Path.Combine( GetMSBuildPath(), "msbuild.exe" );

		//	return ProcessUtils.RunAndWait( msBuildExePath, args, out result ) == 0;

		//	//if( ProcessUtils.RunAndWait( msBuildExePath, args, out string result ) != 0 )
		//	//{
		//	//	xx xx;

		//	//	throw new Exception( string.Format( "Failed to build Visual Studio project using arguments '{0} {1}'.\nOutput:{2}\n", msBuildExePath, args, result ) );
		//	//	//throw new BuildException( string.Format( "Failed to build Visual Studio project using arguments '{0} {1}'.\nOutput:{2}\n", msBuildExePath, args, result ) );
		//	//}
		//	//return result;
		//}

		//public static bool RestoreNuGetPackets( string solutionDir, string productName, BuildConfig config )
		//{
		//	var fileName = $"{productName}.sln";

		//	//TODO: Kill Running App before build?

		//	// restore changes in msbuild 15.5 https://stackoverflow.com/questions/46773698/msbuild-15-nuget-restore-and-build

		//	string arguments;
		//	{
		//		var builder = new StringBuilder();
		//		builder.Append( $"\"{solutionDir}\\{fileName}\" /nologo /maxcpucount /t:Restore" );
		//		builder.Append( $" /p:SolutionDir=\"{solutionDir}\"" );
		//		builder.Append( $" /p:Configuration={config.BuildConfiguration}" );
		//		builder.Append( $" /p:Platform=\"{config.BuildPlatform}\"" );

		//		//const bool InternalNuGet = true;
		//		//if( InternalNuGet )
		//		//{
		//		//	string nuGetRestoreTargets = Path.Combine( ToolsetHelper.GetNuGetBuildPath(), "NuGet.targets" );
		//		//	args.Append( $" /p:NuGetRestoreTargets=\"{nuGetRestoreTargets}\"" );
		//		//}

		//		builder.Append( $" /Verbosity:{config.Verbosity}" );

		//		arguments = builder.ToString();
		//	}

		//	//!!!!
		//	Log.Fatal( "VisualStudioSolutionUtility: RestoreNuGetPackets: impl." );

		//	var msBuildExePath = Path.Combine( GetMSBuildFolderPath(), "msbuild.exe" );

		//	var success = ProcessUtility.RunAndWait( msBuildExePath, arguments, out var result ) == 0;
		//	result = result.Trim( new char[] { '\r', '\n' } );

		//	if( success )
		//	{
		//		// result ignored ?
		//		Log.Info( $"NuGet packages for \'{fileName}\' was restored successfully." );
		//	}
		//	else
		//	{
		//		var error = $"Unable to restore NuGet packages for \'{fileName}\'.\r\n\r\n" + result;
		//		Log.Error( result );
		//	}
		//	return success;
		//}

		//public static bool BuildSolution( string solutionDir, string productName, BuildConfig config, bool rebuild )
		//{
		//	var fileName = $"{productName}.sln";

		//	string arguments;
		//	{
		//		//!!!!юзает последнюю версию фрейморка. например Preview
		//		//[-r | --runtime<RUNTIME_IDENTIFIER>]

		//		var builder = new StringBuilder();
		//		builder.Append( "build" );
		//		zzzzzz;
		//		builder.Append( $" \"{solutionDir}\\{productName}.sln\"" );

		//		builder.Append( $" --configuration {config.BuildConfiguration}" );
		//		builder.Append( $" --framework netcoreapp3.1" );

		//		zzzzzz;
		//		if( !string.IsNullOrEmpty( config.OutputDirectory ) )
		//			builder.Append( $" --output \"{config.OutputDirectory}\"" );

		//		builder.Append( $" --verbosity {config.Verbosity}" );

		//		//!!!!optional?
		//		//builder.Append( " --no-restore" );

		//		if( rebuild )
		//			builder.Append( " --no-incremental" );

		//		//builder.Append( $" /p:Platform=\"{config.BuildPlatform}\"" );
		//		//if( !string.IsNullOrEmpty( config.OutputAssemblyName ) )
		//		//	builder.Append( $" /p:AssemblyName=\"{config.OutputAssemblyName}\"" );

		//		//builder.Append( $" /p:AppxPackage={config.CreateAppxPackage}" );
		//		//builder.Append( $" /p:AppxBundlePlatforms=\"{config.AppxBundlePlatforms}\"" );
		//		//builder.Append( $" /p:AppxBundle={config.CreateAppxBundle}" );
		//		//builder.Append( $" /p:AppxPackageSigningEnabled={config.AppxPackageSigningEnabled}" );

		//		//// instead of TargetFrameworkRootPath we can also use FrameworkPathOverride prop but only for .net 4.x building. 
		//		//// FrameworkPathOverride will not work for UWP app building.
		//		//string frameworkRootPath = GetTargetFrameworkRootPath();
		//		//if( Directory.Exists( frameworkRootPath ) )
		//		//	builder.Append( $" /p:TargetFrameworkRootPath=\"{frameworkRootPath}\"" );
		//		//else
		//		//	Log.Info( $"Framework Root Path '{frameworkRootPath}' not found. will be used Targeting Pack installed in system." );

		//		arguments = builder.ToString();
		//	}

		//	var msBuildExePath = GetDotNetExecutablePath();
		//	var success = ProcessUtility.RunAndWait( msBuildExePath, arguments, out var result ) == 0;

		//	//var msBuildExePath = Path.Combine( GetDotNetExecutablePath(), Path.Combine( GetMSBuildFolderPath(), "MSBuild.dll" ) );

		//	//var msBuildExePath = Path.Combine( GetMSBuildFolderPath(), "msbuild.exe" );
		//	//var success = ProcessUtility.RunAndWait( msBuildExePath, arguments, out var result ) == 0;

		//	result = result.Trim( new char[] { '\r', '\n' } );

		//	if( success )
		//	{
		//		//!!!!result ignored?

		//		Log.Info( $"\'{fileName}\' was built successfully." );
		//		return true;
		//	}
		//	else
		//	{
		//		if( EngineApp.IsSimulation )
		//		{
		//			var error = $"Unable to compile solution \'{fileName}\'. Continue?\r\n\r\n{result}\r\n\r\nCommand line:\r\n{msBuildExePath} {arguments}\r\n\r\nSee details in log.";

		//			if( LogPlatformFunctionality.Instance.ShowMessageBox( error, "Error", EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
		//			{
		//				Process.GetCurrentProcess().Kill();
		//				Environment.Exit( 0 );
		//			}
		//		}
		//		else
		//		{
		//			var error = $"Unable to compile solution \'{fileName}\'.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{msBuildExePath} {arguments}\r\n\r\nSee details in log.";
		//			Log.Error( error );
		//		}

		//		return false;
		//	}



		//	//var fileName = $"{productName}.sln";

		//	//string arguments;
		//	//{
		//	//	var builder = new StringBuilder();
		//	//	builder.Append( $"\"{solutionDir}\\{productName}.sln\" /nologo /maxcpucount /t:Build" );
		//	//	builder.Append( $" /p:SolutionDir=\"{solutionDir}\"" );
		//	//	if( !string.IsNullOrEmpty( config.OutDir ) )
		//	//		builder.Append( $" /p:OutDir=\"{config.OutDir}\"" );
		//	//	builder.Append( $" /p:Configuration={config.BuildConfiguration}" );
		//	//	builder.Append( $" /p:Platform=\"{config.BuildPlatform}\"" );
		//	//	if( !string.IsNullOrEmpty( config.OutputAssemblyName ) )
		//	//		builder.Append( $" /p:AssemblyName=\"{config.OutputAssemblyName}\"" );

		//	//	builder.Append( $" /p:AppxPackage={config.CreateAppxPackage}" );
		//	//	builder.Append( $" /p:AppxBundlePlatforms=\"{config.AppxBundlePlatforms}\"" );
		//	//	builder.Append( $" /p:AppxBundle={config.CreateAppxBundle}" );
		//	//	builder.Append( $" /p:AppxPackageSigningEnabled={config.AppxPackageSigningEnabled}" );

		//	//	// instead of TargetFrameworkRootPath we can also use FrameworkPathOverride prop but only for .net 4.x building. 
		//	//	// FrameworkPathOverride will not work for UWP app building.
		//	//	string frameworkRootPath = GetTargetFrameworkRootPath();
		//	//	if( Directory.Exists( frameworkRootPath ) )
		//	//		builder.Append( $" /p:TargetFrameworkRootPath=\"{frameworkRootPath}\"" );
		//	//	else
		//	//		Log.Info( $"Framework Root Path '{frameworkRootPath}' not found. will be used Targeting Pack installed in system." );

		//	//	builder.Append( $" /Verbosity:{config.Verbosity}" );

		//	//	if( rebuild )
		//	//		builder.Append( " /t:Rebuild" );

		//	//	//const bool PerformanceSummary = false;
		//	//	//if( PerformanceSummary )
		//	//	//	builder.Append( $" /clp:performancesummary" );

		//	//	arguments = builder.ToString();
		//	//}

		//	//var msBuildExePath = Path.Combine( GetDotNetExecutablePath(), Path.Combine( GetMSBuildFolderPath(), "MSBuild.dll" ) );
		//	//var success = ProcessUtility.RunAndWait( msBuildExePath, arguments, out var result ) == 0;

		//	////var msBuildExePath = Path.Combine( GetMSBuildFolderPath(), "msbuild.exe" );
		//	////var success = ProcessUtility.RunAndWait( msBuildExePath, arguments, out var result ) == 0;

		//	//result = result.Trim( new char[] { '\r', '\n' } );

		//	//if( success )
		//	//{
		//	//	//!!!!result ignored?

		//	//	Log.Info( $"\'{fileName}\' was built successfully." );
		//	//	return true;
		//	//}
		//	//else
		//	//{
		//	//	var error = $"Unable to compile solution \'{fileName}\'.\r\n\r\n{result}\r\n\r\nCommand line:\r\n{msBuildExePath} {arguments}\r\n\r\nSee details in log.";
		//	//	Log.Error( error );
		//	//	return false;
		//	//}
		//}

		////!!!!uy
		////!!!!тут ли
		//public static bool ReadSolutionData( /*string fileName, */out string error, out string[] projects )
		//{
		//	error = "";
		//	projects = null;

		//	//!!!!
		//	var path = Path.Combine( CSharpProjectFileUtility.ProjectDir, "Project.sln" );
		//	//var path = Path.Combine( CSharpProjectFileUtility.ProjectDir, "Project.sln" );
		//	if( File.Exists( path ) )
		//	{
		//		try
		//		{
		//			var workspace = MSBuildWorkspace.Create();
		//			var solution = workspace.OpenSolutionAsync( path ).Result;

		//			//foreach( var p in solution.Projects )
		//			//{
		//			//	Log.Info( p.ToString() );
		//			//}

		//		}
		//		catch( Exception e )
		//		{
		//			error = e.Message;
		//			return false;
		//		}

		//		//!!!!

		//		return true;
		//	}
		//	else
		//	{
		//		//!!!!
		//	}

		//	//var path = Path.Combine( CSharpProjectFileUtility.ProjectDir, "Project.sln" );
		//	//if( File.Exists( path ) )
		//	//{
		//	//	try
		//	//	{
		//	//		using( var projectCollection = CSharpProjectFileUtility.CreateProjectCollection() )
		//	//		{
		//	//			var solution = projectCollection.LoadProject( path );

		//	//			foreach( var p in solution.ProjectCollection.LoadedProjects )
		//	//			{
		//	//				Log.Info( p );
		//	//			}
		//	//		}

		//	//	}
		//	//	catch( Exception e )
		//	//	{
		//	//		error = e.Message;
		//	//		return false;
		//	//	}

		//	//	//!!!!

		//	//	return true;
		//	//}
		//	//else
		//	//{
		//	//	//!!!!
		//	//}

		//	//!!!!

		//	return false;
		//}

	}
}



/*

namespace NeoAxis
{
	public static class ProjectUtils2
	{
		public static void GetDefaultProjectPaths( out string dataDirectory, out string userSettingsDirectory )
		{
			dataDirectory = GetDataFolder();
			userSettingsDirectory = GetSettingsFolder();
		}

		static string GetDataFolder()
		{
#if W______INDOWS_UWP
			var appFolder = GetInstalledAppFolder();
#else
			var appFolder = GetLocalAppDataFolder();
#endif

			if( Directory.Exists( appFolder ) )
				return Path.Combine( appFolder, "Data" );
			else
				Log.Fatal( "App folder not found: " + appFolder );

			return "";
		}

		static string GetSettingsFolder()
		{
			var dataFolder = GetLocalAppDataFolder();
			if( Directory.Exists( dataFolder ) )
				return Path.Combine( dataFolder, "User settings" );
			else
				Log.Fatal( "Data folder not found: " + dataFolder );

			return "";
		}


		static string GetLocalAppDataFolder()
		{
#if W______INDOWS_UWP
			Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
			return localFolder.Path;
#else
			var folder = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
			return Path.Combine( folder, "Project" );
#endif
		}

		static string GetInstalledAppFolder()
		{
#if W______INDOWS_UWP
			var installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
			return installedLocation.Path;
#else
			return Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
#endif
		}
	}
}
*/
#endif