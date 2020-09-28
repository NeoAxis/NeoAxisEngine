// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LiteDB;

namespace NeoAxis
{
	class ScriptCache
	{
		Dictionary<string, CompiledScript> compiledScripts = new Dictionary<string, CompiledScript>();

		LiteDatabase database;
		Dictionary<string, Type> loadedAssemblyDllTypes;

		object lockObjectGetOrCompileScript = new object();

		/////////////////////////////////////////

		class DatabaseItem
		{
			public int Id { get; set; }

			//save in Base64 because bug in the LiteDB. trim \r\n at the end of text
			//script = script.Trim( new char[] { '\r', '\n' } );
			public string ScriptBase64 { get; set; }
		}

		/////////////////////////////////////////

		class DatabaseCompiledAssemblyDllItem
		{
			public int Id { get; set; }
			public string ApplicationType { get; set; }
		}

		/////////////////////////////////////////

		public ScriptCache()
		{
		}

		string CacheFolder
		{
			get { return PathUtility.Combine( VirtualFileSystem.Directories.Project, @"Caches\CSharpScripts" ); }
		}

		string DatabaseFileName
		{
			get { return Path.Combine( CacheFolder, "CSharpScripts.cache" ); }
		}

		string AssemblyFileName
		{
			get { return Path.Combine( CacheFolder, $"CSharpScripts_{EngineApp.ApplicationType}.dll" ); }
		}

		public string GeneratedCSFileName
		{
			get { return Path.Combine( CacheFolder, "CSharpScripts.cs" ); }
		}

		public List<string> GetScriptsToCompile()
		{
			var scriptsCollection = database.GetCollection<DatabaseItem>( "scripts" );
			return new List<string>( scriptsCollection.FindAll().Select( i => FromBase64( i.ScriptBase64 ) ) );
		}

		public void Initialize()
		{
			//create cache folder
			if( !Directory.Exists( CacheFolder ) )
				Directory.CreateDirectory( CacheFolder );

			////get file paths
			//string textCachePath = Path.Combine( CacheFolder, textCacheName );
			//string cacheAssemblyPath = Path.Combine( CacheFolder, cacheAssemblyName );

			//init cache database
			database = new LiteDatabase( DatabaseFileName );

			//recompile cache if need to update

			bool needCompile = false;

			if( ScriptingCSharpEngine.CanCompileScripts )
			{
				//check dll is not in the list of precompiled dlls
				var compiledDllsCollection = database.GetCollection<DatabaseCompiledAssemblyDllItem>( "compiledDlls" );
				if( compiledDllsCollection.FindOne( Query.EQ( "ApplicationType", EngineApp.ApplicationType.ToString() ) ) == null )
					needCompile = true;

				// if text cache exists and assembly is absent
				if( !File.Exists( AssemblyFileName ) )
					needCompile = true;

				// don't compile if assembly cache exist but locked.
				if( needCompile && File.Exists( AssemblyFileName ) && IOUtility.IsFileLocked( AssemblyFileName ) )
				{
					//Log.Info( "Script Cache can not be updated because locked" );
					needCompile = false;
				}
			}

			if( needCompile )
			{
				var scriptsToCompile = GetScriptsToCompile();
				if( scriptsToCompile.Count != 0 )
				{
					if( CompileAssemblyDll( scriptsToCompile ) )
					{
						//register dll as compiled
						var compiledDllsCollection = database.GetCollection<DatabaseCompiledAssemblyDllItem>( "compiledDlls" );
						try
						{
							if( compiledDllsCollection.FindOne( Query.EQ( "ApplicationType", EngineApp.ApplicationType.ToString() ) ) == null )
								compiledDllsCollection.Insert( new DatabaseCompiledAssemblyDllItem { ApplicationType = EngineApp.ApplicationType.ToString() } );
						}
						catch { }
					}
					else
					{
						//unable to compile

						//clear database
						database.DropCollection( "scripts" );
						database.DropCollection( "compiledDlls" );

						//delete dll if exists
						DeleteAssemblyDllFile();
					}
				}
				else
					DeleteAssemblyDllFile();
			}
			else
			{
#if DEPLOY
				//on UWP, Android scripts compiled inside Project.dll
				if( EngineApp.ProjectAssembly != null )
					FillLoadedAssemblyDllTypes( EngineApp.ProjectAssembly );
#else
				//try load dll
				if( File.Exists( AssemblyFileName ) )
				{
					if( !LoadAssemblyDll() )
						DeleteAssemblyDllFile();
				}
#endif
			}
		}

		public void Dispose()
		{
			database?.Dispose();
			database = null;
		}

		void AddCompiledScript( string script, CompiledScript compiledScript )
		{
			compiledScripts.Add( script, compiledScript );

			var scriptBase64 = ToBase64( script );

			var scriptsCollection = database.GetCollection<DatabaseItem>( "scripts" );
			if( !scriptsCollection.Exists( s => s.ScriptBase64 == scriptBase64 ) )
			{
				scriptsCollection.Insert( new DatabaseItem { ScriptBase64 = scriptBase64 } );

				//need recompile dlls
				database.DropCollection( "compiledDlls" );
			}
		}

		string ToBase64( string code )
		{
			byte[] bytes = Encoding.UTF8.GetBytes( code );
			return Convert.ToBase64String( bytes, Base64FormattingOptions.None );
		}

		string FromBase64( string codeInBase64 )
		{
			var bytes = Convert.FromBase64String( codeInBase64 );
			return Encoding.UTF8.GetString( bytes );
		}

		Type FindScriptTypeInLoadedAssemblyDll( string key )
		{
			if( loadedAssemblyDllTypes != null )
			{
				var base64 = ToBase64( key );
				if( loadedAssemblyDllTypes.TryGetValue( base64, out var type ) )
					return type;

				//var base64 = ScriptCodeGenerator.GetBase64( key );
				//foreach( var type in loadedAssemblyDll.GetTypes() )
				//{
				//	var attr = type.GetCustomAttribute<CSharpScriptGeneratedAttribute>();
				//	if( attr != null && attr.Key == base64 )
				//		return type;
				//}
			}
			return null;
		}

		CompiledScript GetCompiledScript( string script )
		{
			CompiledScript compiledScript;

			compiledScripts.TryGetValue( script, out compiledScript );
			if( compiledScript != null )
				return compiledScript;

			var scriptType = FindScriptTypeInLoadedAssemblyDll( script );
			if( scriptType != null )
			{
				compiledScript = CompiledScript.CreateFrom( scriptType );
				compiledScripts.Add( script, compiledScript );
				return compiledScript;
			}

			return null;
		}

		public CompiledScript GetOrCompileScript( string script, out string error )
		{
			if( string.IsNullOrEmpty( script ) )
				Log.Fatal( "ScriptCache: Get: \"script\" is empty." );

			error = "";

			lock( lockObjectGetOrCompileScript )
			{
				var compiledScript = GetCompiledScript( script );
				if( compiledScript != null )
					return compiledScript;

				if( !ScriptingCSharpEngine.CanCompileScripts )
				{
					error = "Unable to get compiled script. The script is not precompiled in the cache. Script compilation is not supported on the current platform, run scenes on dev machine to make the cache.";
					return null;
				}

				try
				{
					ScriptingCSharpEngine.CheckForSyntaxErrors( script );

					var assembly = ScriptingCSharpEngine.CompileScriptsToAssembly( new List<string> { script }, null );
					compiledScript = CompiledScript.CreateFrom( assembly );

					AddCompiledScript( script, compiledScript );

					return compiledScript;
				}
				catch( Exception ex )
				{
					error = ex.Message;
					return null;
				}
			}
		}

		//void Clear()
		//{
		//	compiledScripts.Clear();

		//	string cacheAssemblyPath = Path.Combine( CacheFolder, cacheAssemblyName );

		//	if( File.Exists( DatabaseFileName ) )
		//		File.Delete( DatabaseFileName );

		//	database.DropCollection( "scripts" );
		//}

		void FillLoadedAssemblyDllTypes( Assembly assembly )
		{
			try
			{
				loadedAssemblyDllTypes = new Dictionary<string, Type>();
				foreach( var type in assembly.GetTypes() )
				{
					var attr = type.GetCustomAttribute<CSharpScriptGeneratedAttribute>();
					if( attr != null )
						loadedAssemblyDllTypes[ attr.Key ] = type;
				}
			}
			catch { }
		}

		bool CompileAssemblyDll( List<string> scriptsToCompile )
		{
			try
			{
				var assembly = ScriptingCSharpEngine.CompileScriptsToAssembly( scriptsToCompile, AssemblyFileName );
				if( assembly != null )
					FillLoadedAssemblyDllTypes( assembly );
			}
			catch( Exception e )
			{
				Log.Info( $"Unable to compile \"{AssemblyFileName}\". " + e.Message );
				return false;
			}

			return true;
		}

		bool LoadAssemblyDll()
		{
			try
			{
				var assembly = Assembly.LoadFrom( AssemblyFileName );
				if( assembly != null )
					FillLoadedAssemblyDllTypes( assembly );
			}
			catch( Exception e )
			{
				Log.Info( $"Unable to load \"{AssemblyFileName}\". " + e.Message );
				return false;
			}

			return true;
		}

		void DeleteAssemblyDllFile()
		{
			if( File.Exists( AssemblyFileName ) )
			{
				try
				{
					File.Delete( AssemblyFileName );
				}
				catch { }
			}
		}
	}
}
