// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using NeoAxis;

namespace Internal
{
	// see similar RoslynPad ScriptRunner class

	/// <summary>
	/// Script compilation and execution engine.
	/// </summary>
	public static class ScriptingCSharpEngine
	{
		static bool initialized;

		static ScriptCache scriptCache = new ScriptCache();

		static bool scriptCompilerRequested;
		static object getScriptCompilerLock = new object();
		//static Lazy<ScriptCompiler> scriptCompiler = new Lazy<ScriptCompiler>();

		//public static List<string> CSharpScriptReferenceAssemblies { get; } = new List<string>();
		//public static List<string> CSharpScriptUsingNamespaces { get; } = new List<string>();
		//internal static ScriptAssemblyNameResolver ScriptAssemblyNameResolver;

		static List<string> tempFilesToDelete = new List<string>();

		/////////////////////////////////////////

		public static void Init()
		{
			if( !initialized )
			{
				try
				{
					if( CanCompileScripts )
					{
						ScriptCompiler.ScriptAssemblyNameResolver = new ScriptAssemblyNameResolver();
						ScriptCompiler.ScriptAssemblyNameResolver.AddSearchDirectory( VirtualFileSystem.Directories.Binaries );

						{
							var netFolder = PathUtility.Combine( VirtualFileSystem.Directories.PlatformSpecific, @"dotnet\shared\Microsoft.NETCore.App" );

							var folderWithFiles = "";
							foreach( var folder in Directory.GetDirectories( netFolder ) )
							{
								if( Directory.GetFiles( folder ).Length != 0 )
								{
									folderWithFiles = folder;
									break;
								}
							}

							if( !string.IsNullOrEmpty( folderWithFiles ) )
								ScriptCompiler.ScriptAssemblyNameResolver.AddSearchDirectory( folderWithFiles );
						}

						InitReferenceAssemblies();
						InitUsingNamespaces();

						GetScriptCompiler().SettingsAddReferences( ScriptCompiler.CSharpScriptReferenceAssemblies );
						//ScriptCompiler.Settings = ScriptCompiler.Settings.AddReferences( CSharpScriptReferenceAssemblies );
					}

					scriptCache.Initialize();

					initialized = true;
				}
				catch( Exception e )
				{
					Log.Warning( "Scripting engine initialization failed. " + e.Message );
				}
			}
		}

		public static void WriteCharpScriptsCsFile()
		{
			try
			{
				//get scripts to compile
				var scriptsToCompile = scriptCache.GetScriptsToCompile();

				//generate code
				string script = "";
				if( scriptsToCompile.Count != 0 )
				{
					script = "#if DEPLOY\r\n";
					script += "namespace Scripts {\r\n";

					script += GetScriptCompiler().ScriptCodeGenerator_GenerateWrappedScript( scriptsToCompile, ScriptCompiler.CSharpScriptUsingNamespaces, null );
					//script += ScriptCodeGenerator.GenerateWrappedScript( scriptsToCompile, CSharpScriptUsingNamespaces, null );//, ContextType );

					script += "\r\n}\r\n";
					script += "#endif";
				}

				var path = scriptCache.GeneratedCSFileName;

				//create folder
				var directory = Path.GetDirectoryName( path );
				if( !Directory.Exists( directory ) )
					Directory.CreateDirectory( directory );

				//write file
				if( !File.Exists( path ) || File.ReadAllText( path, Encoding.UTF8 ) != script )
					File.WriteAllText( path, script, Encoding.UTF8 );
			}
			catch( Exception e )
			{
				Log.Warning( $"Unable to write \'{scriptCache.GeneratedCSFileName}\'. " + e.Message );
			}
		}

		public static void Shutdown()
		{
			if( !initialized )
				return;

			//write CSharpScripts.cs
			WriteCharpScriptsCsFile();

			try
			{
				scriptCache.Dispose();
				DeleteTempFiles();
			}
			catch( Exception e )
			{
				Log.Warning( "Scripting engine shutdown failed. " + e.Message );
			}
		}

		static void InitReferenceAssemblies()
		{
			foreach( var b in VirtualFileSystem.DefaultSettingsConfig.Children )
			{
				if( string.Compare( b.Name, "CSharpScriptReferenceAssembly", true ) == 0 )
				{
					string name = b.GetAttribute( "Name" );
					if( !string.IsNullOrEmpty( name ) )
					{
						try
						{
							string fullPath = ScriptCompiler.ScriptAssemblyNameResolver.Resolve( name );
							if( File.Exists( fullPath ) )
								ScriptCompiler.CSharpScriptReferenceAssemblies.Add( fullPath );
						}
						catch { }
					}
				}
			}
		}

		static void InitUsingNamespaces()
		{
			foreach( var b in VirtualFileSystem.DefaultSettingsConfig.Children )
			{
				if( string.Compare( b.Name, "CSharpScriptUsingNamespace", true ) == 0 )
				{
					string value = b.GetAttribute( "Value" );
					if( !string.IsNullOrEmpty( value ) )
						ScriptCompiler.CSharpScriptUsingNamespaces.Add( value );
				}
			}
		}

		internal static CompiledScript GetOrCompileScript( string script, out string error )
		{
			Init();

			return scriptCache.GetOrCompileScript( script, out error );
		}

		public static bool CanCompileScripts
		{
			get
			{
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.UWP )
					return false;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
					return false;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.iOS )
					return false;
				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Web )
					return false;

				if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Windows )
				{
					if( SystemSettings.CommandLineParameters.TryGetValue( "-client", out _ ) )
						return false;
					if( SystemSettings.CommandLineParameters.TryGetValue( "-server", out _ ) )
						return false;

					var dotnetDirectoryPath = Path.Combine( VirtualFileSystem.Directories.EngineInternal, @"Platforms\Windows\dotnet" );
					if( !Directory.Exists( dotnetDirectoryPath ) )
						return false;


					//!!!!check NeoAxis.Core.CompileScripts.dll existence and loadability

				}

				return true;
			}
		}

		public static void AddTempFileToDelete( string file )
		{
			tempFilesToDelete.Add( file );
		}

		static void DeleteTempFiles()
		{
			foreach( string file in tempFilesToDelete )
			{
				if( File.Exists( file ) )
					File.Delete( file );
			}
		}

		public static ScriptCompiler GetScriptCompiler()
		{
			try
			{
				lock( getScriptCompilerLock )
				{
					if( !scriptCompilerRequested )
					{
						scriptCompilerRequested = true;
						var assembly = AssemblyUtility.LoadAssemblyByRealFileName( "NeoAxis.Core.CompileScripts.dll", false, true );
					}
				}
				return ScriptCompiler.Instance;

				//return scriptCompiler.Value;
			}
			catch( Exception e )
			{
				if( e.Message.Contains( "System.Collections.Immutable" ) ||
				   ( e.InnerException != null && e.InnerException.Message.Contains( "System.Collections.Immutable" ) ) )
					throw new Exception( "File loading error. Make sure 'Microsoft.CodeAnalysis.CSharp.Scripting' package is installed.", e );
				else
					throw;
			}
		}

		public static Assembly CompileScriptsToAssembly( IEnumerable<string> scripts, string writeToDllOptional )
		{
			var scriptCompiler = GetScriptCompiler();

			string script = scriptCompiler.ScriptCodeGenerator_GenerateWrappedScript( scripts, ScriptCompiler.CSharpScriptUsingNamespaces, null );//, ContextType );
			return scriptCompiler.CompileCode( script, writeToDllOptional );

			//string script = ScriptCodeGenerator.GenerateWrappedScript( scripts, CSharpScriptUsingNamespaces, null );//, ContextType );
			//return ScriptCompiler.CompileCode( script, writeToDllOptional );
		}

		public static void CheckForSyntaxErrors( string code )
		{
			GetScriptCompiler().ScriptCodeGenerator_CheckForSyntaxErrors( code );
			//ScriptCodeGenerator.CheckForSyntaxErrors( code );
		}

		public static bool ScriptCacheCompile( out string error )
		{
			return scriptCache.Compile( out error );
		}
	}
}
