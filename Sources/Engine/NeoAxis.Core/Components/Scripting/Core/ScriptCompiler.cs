// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !NO_EMIT
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Runtime.Loader;
using System.Text;

namespace NeoAxis
{
	class ScriptCompiler
	{
		//!!!!can work when false?
		/*public */
		static bool DebugBuild { get; set; } = true;

		public static ScriptOptions Settings { get; set; } = ScriptOptions.Default;

		///////////////////////////////////////////////

		//// This is a collectible (unloadable) AssemblyLoadContext that loads the dependencies
		//// of the plugin from the plugin's binary directory.
		//class HostAssemblyLoadContext : AssemblyLoadContext
		//{
		//	// Resolver of the locations of the assemblies that are dependencies of the
		//	// main plugin assembly.
		//	private AssemblyDependencyResolver _resolver;

		//	public HostAssemblyLoadContext( string pluginPath ) : base( isCollectible: true )
		//	{
		//		_resolver = new AssemblyDependencyResolver( pluginPath );
		//	}

		//	// The Load method override causes all the dependencies present in the plugin's binary directory to get loaded
		//	// into the HostAssemblyLoadContext together with the plugin assembly itself.
		//	// NOTE: The Interface assembly must not be present in the plugin's binary directory, otherwise we would
		//	// end up with the assembly being loaded twice. Once in the default context and once in the HostAssemblyLoadContext.
		//	// The types present on the host and plugin side would then not match even though they would have the same names.
		//	protected override Assembly Load( AssemblyName name )
		//	{
		//		string assemblyPath = _resolver.ResolveAssemblyToPath( name );
		//		if( assemblyPath != null )
		//		{
		//			//Console.WriteLine( $"Loading assembly {assemblyPath} into the HostAssemblyLoadContext" );
		//			return LoadFromAssemblyPath( assemblyPath );
		//		}

		//		return null;
		//	}
		//}

		///////////////////////////////////////////////

		string GetScriptTempFileName()
		{
			return Path.Combine( Path.GetTempPath(), string.Format( "{0}.{1}.tmp", Process.GetCurrentProcess().Id, Guid.NewGuid() ) );
		}

		public Assembly CompileCode( string scriptText, string writeToDllOptional )
		{
			string scriptFile = null;

			//!!!!
			//slow first exec. ~3 sec
			// https://stackoverflow.com/questions/22974473/using-roslyn-emit-method-with-a-modulebuilder-instead-of-a-memorystream

			string tempScriptFile = null;
			try
			{
				if( DebugBuild )
				{
					// Excellent example of debugging support
					// http://www.michalkomorowski.com/2016/10/roslyn-how-to-create-custom-debuggable_27.html

					if( scriptFile == null )
					{
						tempScriptFile = GetScriptTempFileName();
						File.WriteAllText( tempScriptFile, scriptText, Encoding.UTF8 );
					}

					scriptText = $"#line 1 \"{scriptFile ?? tempScriptFile}\"{Environment.NewLine}" + scriptText;
				}

				//options
				var options = Settings;
				if( !DebugBuild )
				{
					// There is a Roslyn bug that prevents emitting debug symbols if file path is specified. And fails
					// the whole compilation:
					// It triggers "error CS8055: Cannot emit debug information for a source text without encoding."
					// Thus disable setting the source path until Roslyn is fixed.
					options = Settings.WithFilePath( scriptFile ?? tempScriptFile );
				}

				var compilation = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create( scriptText, options ).GetCompilation();

				if( DebugBuild )
				{
					compilation = compilation.WithOptions( compilation.Options
						.WithOptimizationLevel( OptimizationLevel.Debug )
						.WithOutputKind( OutputKind.DynamicallyLinkedLibrary ) );
				}

				return EmitIL( compilation, writeToDllOptional );
			}
			finally
			{
				if( DebugBuild )
					ScriptingCSharpEngine.AddTempFileToDelete( tempScriptFile );
				else
				{
					if( File.Exists( tempScriptFile ) )
						File.Delete( tempScriptFile );
				}
			}
		}

		Assembly EmitIL( Compilation compilation, string writeToDllOptional )
		{
			var emitInMemory = string.IsNullOrEmpty( writeToDllOptional );

			using( var peStream = new MemoryStream() )
			using( var pdbStream = new MemoryStream() )
			{
				var emitOptions = new EmitOptions( false, DebugInformationFormat.PortablePdb );

				EmitResult result;
				if( DebugBuild )
					result = compilation.Emit( peStream, pdbStream, options: emitOptions );
				else
					result = compilation.Emit( peStream );

				if( result.Success )
				{
					if( emitInMemory )
					{

						//!!!!to implement unloading need change ScriptCache


						//var context = new HostAssemblyLoadContext( VirtualFileSystem.Directories.Binaries );

						//Assembly assembly = null;

						////use _assemblyLoader.LoadAssemblyFromStream see RoslynPad
						//peStream.Seek( 0, SeekOrigin.Begin );
						//if( DebugBuild )
						//{
						//	pdbStream.Seek( 0, SeekOrigin.Begin );

						//	context.LoadFromStream( zzz );
						//	//return AppDomain.CurrentDomain.Load( peStream.GetBuffer(), pdbStream.GetBuffer() );
						//}
						//else
						//	return AppDomain.CurrentDomain.Load( peStream.GetBuffer() );

						//zzzzz;

						//return assembly;


						//use _assemblyLoader.LoadAssemblyFromStream see RoslynPad
						peStream.Seek( 0, SeekOrigin.Begin );
						if( DebugBuild )
						{
							pdbStream.Seek( 0, SeekOrigin.Begin );
							return AppDomain.CurrentDomain.Load( peStream.GetBuffer(), pdbStream.GetBuffer() );
						}
						else
							return AppDomain.CurrentDomain.Load( peStream.GetBuffer() );
					}
					else
					{
						peStream.Seek( 0, SeekOrigin.Begin );
						CopyToFile( writeToDllOptional, peStream );
						//CopyToFileAsync( assemblyPath, peStream ).Wait();

						if( DebugBuild )
						{
							pdbStream.Seek( 0, SeekOrigin.Begin );
							CopyToFile( Path.ChangeExtension( writeToDllOptional, "pdb" ), pdbStream );
							//CopyToFileAsync( Path.ChangeExtension( assemblyPath, "pdb" ), pdbStream ).ConfigureAwait( false );
						}

						return Assembly.LoadFrom( writeToDllOptional );
					}
				}
				else
				{
					throw new ScriptCompilerException( ScriptUtility.FormatCompilationError( result.Diagnostics, DebugBuild ) );
				}
			}
		}

		void CopyToFile( string path, Stream stream )
		{
			using( var fileStream = new FileStream( path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous ) )
				stream.CopyTo( fileStream );
		}
	}
}
#endif