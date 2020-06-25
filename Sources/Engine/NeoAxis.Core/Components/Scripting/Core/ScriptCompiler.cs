// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
using System.Text;

namespace NeoAxis
{
	class ScriptCompiler
	{
		public static bool DebugBuild { get; set; } = true;
		public static ScriptOptions Settings { get; set; } = ScriptOptions.Default;

		//

		string GetScriptTempFileName()
		{
			return Path.Combine( Path.GetTempPath(), string.Format( "{0}.{1}.tmp", Process.GetCurrentProcess().Id, Guid.NewGuid() ) );
		}

		public Assembly CompileCode( string scriptText, /*string scriptFile, bool emitInMemory, */string assemblyFileName )
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

				var options = Settings;
				if( !DebugBuild )
				{
					// There is a Roslyn bug that prevents emitting debug symbols if file path is specified. And fails
					// the whole compilation:
					// It triggers "error CS8055: Cannot emit debug information for a source text without encoding."
					// Thus disable setting the source path until Roslyn is fixed.
					options = Settings.WithFilePath( scriptFile ?? tempScriptFile );
				}

				var compilation = CSharpScript.Create( scriptText, options ).GetCompilation();

				if( DebugBuild )
					compilation = compilation.WithOptions( compilation.Options
						.WithOptimizationLevel( OptimizationLevel.Debug )
						.WithOutputKind( OutputKind.DynamicallyLinkedLibrary ) );

				return EmitIL( compilation, /*emitInMemory, */assemblyFileName );
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

		Assembly EmitIL( Compilation compilation, /*bool emitInMemory, */string assemblyFileName )
		{
			var emitInMemory = string.IsNullOrEmpty( assemblyFileName );

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
						//use _assemblyLoader.LoadAssemblyFromStream see RoslynPad
						peStream.Seek( 0, SeekOrigin.Begin );
						if( DebugBuild )
						{
							pdbStream.Seek( 0, SeekOrigin.Begin );
							return AppDomain.CurrentDomain.Load( peStream.GetBuffer(), pdbStream.GetBuffer() );
						}

						return AppDomain.CurrentDomain.Load( peStream.GetBuffer() );
					}
					else
					{
						peStream.Seek( 0, SeekOrigin.Begin );
						CopyToFile( assemblyFileName, peStream );
						//CopyToFileAsync( assemblyPath, peStream ).Wait();

						if( DebugBuild )
						{
							pdbStream.Seek( 0, SeekOrigin.Begin );
							CopyToFile( Path.ChangeExtension( assemblyFileName, "pdb" ), pdbStream );
							//CopyToFileAsync( Path.ChangeExtension( assemblyPath, "pdb" ), pdbStream ).ConfigureAwait( false );
						}

						return Assembly.LoadFrom( assemblyFileName );
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