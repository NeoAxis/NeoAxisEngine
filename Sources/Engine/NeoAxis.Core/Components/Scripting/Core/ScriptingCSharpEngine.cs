// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;

namespace NeoAxis
{
	// see similar RoslynPad ScriptRunner class

	/// <summary>
	/// Script compilation and execution engine.
	/// </summary>
	static class ScriptingCSharpEngine
	{
		static bool initialized;

		static ScriptCache scriptCache = new ScriptCache();

#if !NO_EMIT
		static Lazy<ScriptCompiler> scriptCompiler = new Lazy<ScriptCompiler>();

		////currently we support only one context type for all scripts.
		//public static Type ContextType { get; private set; } = typeof( CSharpScriptContext );
#endif

		public static List<string> CSharpScriptReferenceAssemblies { get; } = new List<string>();
		public static List<string> CSharpScriptUsingNamespaces { get; } = new List<string>();

		public static ScriptAssemblyNameResolver scriptAssemblyNameResolver;
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
						scriptAssemblyNameResolver = new ScriptAssemblyNameResolver();
						scriptAssemblyNameResolver.AddSearchDirectory( VirtualFileSystem.Directories.Binaries );

						{
							var folder = PathUtility.Combine( VirtualFileSystem.Directories.PlatformSpecific, @"dotnet\shared\Microsoft.WindowsDesktop.App\3.1.6" );
							if( Directory.Exists( folder ) )
								scriptAssemblyNameResolver.AddSearchDirectory( folder );
						}
						{
							var folder = PathUtility.Combine( VirtualFileSystem.Directories.PlatformSpecific, @"dotnet\shared\Microsoft.NETCore.App\3.1.6" );
							if( Directory.Exists( folder ) )
								scriptAssemblyNameResolver.AddSearchDirectory( folder );
						}
						//scriptAssemblyNameResolver.AddSearchDirectory( Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, @"dotnet\shared\Microsoft.WindowsDesktop.App\3.1.6" ) );
						//scriptAssemblyNameResolver.AddSearchDirectory( Path.Combine( VirtualFileSystem.Directories.PlatformSpecific, @"dotnet\shared\Microsoft.NETCore.App\3.1.6" ) );

						InitReferenceAssemblies();
						InitUsingNamespaces();

#if !NO_EMIT
						ScriptCompiler.Settings = ScriptCompiler.Settings.AddReferences( CSharpScriptReferenceAssemblies );
#endif
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

#if !NO_EMIT
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
					script += ScriptCodeGenerator.GenerateWrappedScript( scriptsToCompile, CSharpScriptUsingNamespaces, null );//, ContextType );
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
#endif

		public static void Shutdown()
		{
			if( !initialized )
				return;

#if !NO_EMIT
			//write CSharpScripts.cs
			WriteCharpScriptsCsFile();
#endif

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
						string fullPath = scriptAssemblyNameResolver.Resolve( name );
						if( File.Exists( fullPath ) )
							CSharpScriptReferenceAssemblies.Add( fullPath );
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
						CSharpScriptUsingNamespaces.Add( value );
				}
			}
		}

		public static CompiledScript GetOrCompileScript( string script, out string error )
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

#if !NO_EMIT
		public static ScriptCompiler ScriptCompiler
		{
			get
			{
				try
				{
					return scriptCompiler.Value;
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
		}
#endif

		public static Assembly CompileScriptsToAssembly( IEnumerable<string> scripts, string writeToDllOptional )
		{
#if !NO_EMIT
			string script = ScriptCodeGenerator.GenerateWrappedScript( scripts, CSharpScriptUsingNamespaces, null );//, ContextType );
			return ScriptCompiler.CompileCode( script, writeToDllOptional );
#else
			return null;
#endif
		}

		public static void CheckForSyntaxErrors( string code )
		{
#if !NO_EMIT
			ScriptCodeGenerator.CheckForSyntaxErrors( code );
#endif
		}
	}
}
