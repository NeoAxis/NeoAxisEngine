// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System.Text;
using System.Xml;
using System.Diagnostics;
using NeoAxis;

namespace CommandLineTools
{
	public static class Compile
	{
		public class LibCompiler
		{
			public readonly CompileFileParser Parser;
			public PlatformServer.ClientData ClientData;

			public readonly List<(string inPath, string outPath, string type)> CompileList;
			public readonly List<string> FolderIncludes;
			//public IReadOnlyDictionary<string, DateTime> CachedTimestamps;
			//public readonly Dictionary<string, string> CompilerPaths;

			string outputPath;
			string tempPath;
			//string emarPath;

			volatile bool isCancelled;

			//

			public LibCompiler( CompileFileParser parser, PlatformServer.ClientData clientData )
			{
				Parser = parser;
				ClientData = clientData;
				CompileList = new List<(string inPath, string outPath, string type)>();
				FolderIncludes = new List<string>();
				//CompilerPaths = new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase );
			}

			public async Task<bool> CompileAsync( /*bool forceRecompile,*/ bool singleTask )
			{
				Prepare();// forceRecompile );

				var hasChanges = await CompileSourcesAsync( singleTask );
				if( isCancelled )
					return false;

				if( hasChanges )
				{
					Console.WriteLine( "Making a library..." );
					await MakeLibAsync();
					if( isCancelled )
						return false;
					Console.WriteLine( "The library was created successfully." );

					//Console.WriteLine( "Saving the cache" );
					//SaveTimestamps();
					//SaveProperties();

					return true;
				}
				else
				{
					Console.WriteLine( "Nothing has changed, compilation is skipped" );
					return true;
				}
			}

			async Task<bool> CompileSourcesAsync( bool singleTask )
			{
				if( !Directory.Exists( tempPath ) )
					Directory.CreateDirectory( tempPath );

				var includes = string.Join( " ", FolderIncludes.Select( i => "-I" + PreparePath( i ) ) );
				var defines = string.Join( " ", Parser.Defines.Select( i => "-D" + PrepareArg( i ) ) );
				var args = $"{includes} {defines}";

				if( singleTask )
				{
					return await CompileBatchAsync( CompileList, args );
				}
				else
				{
					var procCount = Environment.ProcessorCount;
					var batches = CompileList
						.Select( ( x, i ) => new { Index = i, Value = x } )
						.GroupBy( x => x.Index % procCount )
						.Select( g => g.Select( x => x.Value ).ToList() ).ToList();
					var tasks = new Task<bool>[ batches.Count ];
					for( int i = 0; i < batches.Count; i++ )
					{
						tasks[ i ] = CompileBatchAsync( batches[ i ], args );
					}
					var hasChanges = await Task.WhenAll( tasks );

					return hasChanges.Contains( true );
				}
			}

			async Task MakeLibAsync()
			{
				if( Parser.Platform == "Web" )
				{

					//!!!!? find . -name "*.o" -exec emar rcs libmylibrary.a {} +
					//!!!!? или типа -filelist

					//!!!!is not tested

					var appPath = Parser.PythonPath;// @"C:\emsdk\emsdk\python\3.13.3_64bit\python.exe";
					var emarArguments = "\"" + Parser.EmscriptenFolder + @"\upstream\emscripten\emar.py" + "\"";

					var outputPath = this.outputPath;
					if( File.Exists( outputPath ) )
						File.Delete( outputPath );
					else if( !Directory.Exists( Path.GetDirectoryName( outputPath ) ) )
						Directory.CreateDirectory( Path.GetDirectoryName( outputPath ) );

					const int maxCmdLen = 2000;
					var sb = new StringBuilder();
					int index = 0;

					outputPath = PreparePath( outputPath );
					sb.Append( $"rcs {outputPath}" );
					while( index < CompileList.Count )
					{
						var fileName = CompileList[ index ].outPath;
						if( sb.Length + fileName.Length > maxCmdLen )
						{
							await RunCmdAsync( appPath, emarArguments + " " + sb.ToString(), tempPath );
							if( isCancelled )
								return;
							Console.WriteLine( $"Done {Math.Floor( index * 100 / (float)CompileList.Count )}%" );

							sb.Clear();
							sb.Append( $"rcs {outputPath}" );
						}
						sb.Append( ' ' );
						sb.Append( fileName );
						index++;
					}
					await RunCmdAsync( appPath, emarArguments + " " + sb.ToString(), tempPath );
				}
				else if( /*Parser.Platform == "macOS" ||*/ Parser.Platform == "iOS" )
				{
					//write all .o files to list.txt
					var listTextFullPath = Path.Combine( tempPath, "list.txt" );
					using( var writer = new StreamWriter( listTextFullPath ) )
					{
						foreach( var info in CompileList )
							writer.WriteLine( info.outPath );
					}

					//compile to root of task directory

					if( ClientData != null )
					{
						var libFileName = Path.GetFileName( outputPath );
						var taskFullDirectory = PlatformServer.GetTaskFullPathDirectory( ClientData.TaskID );
						var libFullPath = Path.Combine( taskFullDirectory, libFileName );

						await RunCmdAsync( "libtool", $"-static -filelist {listTextFullPath} -o {libFullPath}", tempPath );
					}
					else
					{
						//no implementation
						Console.WriteLine( "Running libtool to create library: no client data, no implementation." );
						isCancelled = true;
					}
				}
				else if( Parser.Platform == "macOS" )
				{
					//write all .o files to list.txt
					var listTextFullPath = Path.Combine( tempPath, "list.txt" );
					using( var writer = new StreamWriter( listTextFullPath ) )
					{
						foreach( var info in CompileList )
							writer.WriteLine( info.outPath );
					}

					//compile to root of task directory

					if( ClientData != null )
					{
						var libFileName = Path.GetFileName( outputPath );
						var taskFullDirectory = PlatformServer.GetTaskFullPathDirectory( ClientData.TaskID );
						var libFullPath = Path.Combine( taskFullDirectory, libFileName );

						await RunCmdAsync( "clang++", $"-dynamiclib -filelist {listTextFullPath} -framework CoreFoundation -framework Foundation -framework AppKit -framework CoreGraphics -liconv -o {libFullPath}", tempPath );

						//await RunCmdAsync( "libtool", $"-static -filelist {listTextFullPath} -o {libFullPath}", tempPath );
					}
					else
					{
						//no implementation
						Console.WriteLine( "Running libtool to create library: no client data, no implementation." );
						isCancelled = true;
					}
				}
				else
				{
					Console.WriteLine( "Making library: Platform not supported: " + Parser.Platform );
				}
			}

			async Task<bool> CompileBatchAsync( List<(string inPath, string outPath, string type)> batch, string args )
			{
				bool hasChanges = false;
				foreach( var info in batch )
				{
					if( isCancelled )
						break;
					hasChanges |= await CompileFileAsync( info.inPath, info.outPath, info.type, args );
				}
				return hasChanges;
			}

			async Task<bool> CompileFileAsync( string inPath, string outPath, string type, string args )
			{

				//!!!!temp
				//Console.WriteLine( "COMPILE inPath: " + inPath );
				//Console.WriteLine( "COMPILE outPath: " + outPath );


				outPath = Parser.GetFullSourcePath( Path.Join( tempPath, outPath ) );
				//if( !ShouldCompileFile( inPath, outPath ) )
				//	return false;

				inPath = PreparePath( inPath );
				outPath = PreparePath( outPath );


				//!!!!temp
				//Console.WriteLine( "COMPILE outPath 2: " + outPath );


				var flags = Parser.CompileFlags[ type ];

				bool result;

				if( Parser.Platform == "Web" )
				{
					var appPath = Parser.PythonPath;// @"C:\emsdk\emsdk\python\3.13.3_64bit\python.exe";
					var arguments = "\"" + Parser.EmscriptenFolder + @"\upstream\emscripten\emcc.py" + "\"";
					arguments += $" {flags} {args} -o {outPath} -c -MD {inPath}";
					result = await RunCmdAsync( appPath, arguments, Parser.CompileFileDirectory );
				}
				else if( Parser.Platform == "macOS" || Parser.Platform == "iOS" )
				{
					var compiler = Path.GetExtension( inPath ).Equals( ".c", StringComparison.OrdinalIgnoreCase ) ? "clang" : "clang++";
					var arguments = $"{flags} {args} -o {outPath} -c -MD {inPath}";
					result = await RunCmdAsync( compiler, arguments, Parser.CompileFileDirectory );
				}
				else
				{
					Console.WriteLine( "Platform not supported: " + Parser.Platform );
					return false;
				}

				////var flags = Parser.CompileFlags[ type ];
				////var compiler = CompilerPaths[ type ];
				////var result = await RunCmdAsync( compiler, $"{flags} {args} -o {outPath} -c -MD {inPath}", Parser.RootPath );

				if( result )
				{
					var inPathFileName = Path.GetFileName( inPath );
					var outPathFileName = Path.GetFileName( outPath );

					Console.WriteLine( $"Compiled: {inPathFileName} as {outPathFileName}" );
					if( ClientData != null )
						PlatformServer.SendShowMessageToClient( ClientData, $"Compiled: {inPathFileName} as {outPathFileName}" );
				}

				return true;
			}

			//bool ShouldCompileFile( string inPath, string outPath )
			//{
			//	if( CheckFileChanged( inPath ) )
			//		return true;

			//	if( !File.Exists( outPath ) )
			//		return true;

			//	var depFile = Path.ChangeExtension( outPath, ".d" );
			//	if( !File.Exists( depFile ) )
			//		return true;

			//	foreach( var line in File.ReadAllLines( depFile ) )
			//	{
			//		var str = line.Trim();
			//		if( str.EndsWith( '\\' ) )
			//			str = str.Substring( 0, str.Length - 1 ).Trim();
			//		foreach( var arg in CommandLineParser.SplitCommandLineIntoArguments( str, true ) )
			//		{
			//			var sourcePath = arg.Trim();
			//			if( sourcePath.EndsWith( ':' ) )
			//				continue;

			//			if( CheckFileChanged( sourcePath ) )
			//				return true;
			//		}
			//	}
			//	return false;
			//}

			//bool CheckFileChanged( string path )
			//{
			//	path = Parser.GetFullSourcePath( path ).Replace( '\\', '/' );

			//	if( !File.Exists( path ) )
			//		return true;

			//	if( !CachedTimestamps.TryGetValue( path, out var timestamp ) )
			//		return true;

			//	var lastWriteTime = File.GetLastWriteTime( path );
			//	return timestamp != lastWriteTime;
			//}

			async Task<bool> RunCmdAsync( string appPath, string arguments, string workingDirectory )
			{
				using Process process = new();
				process.StartInfo = new ProcessStartInfo
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					WorkingDirectory = workingDirectory,
					UseShellExecute = false,
					FileName = appPath,
					Arguments = arguments,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};

				process.OutputDataReceived += ( s, ea ) =>
				{
					if( !string.IsNullOrEmpty( ea.Data ) )
						Console.WriteLine( ea.Data );
				};

				process.ErrorDataReceived += ( s, ea ) =>
				{
					if( !string.IsNullOrEmpty( ea.Data ) )
					{
						Console.WriteLine( "ERROR: " + ea.Data );
						if( ClientData != null )
							PlatformServer.SendShowMessageToClient( ClientData, "ERROR: " + ea.Data );
					}
				};

				try
				{
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();

					await process.WaitForExitAsync();

					if( process.ExitCode != 0 )
					{
						isCancelled = true;
						return false;
					}

					return true;
				}
				catch( Exception e )
				{
					Console.WriteLine( e );
					isCancelled = true;
					return false;
				}
			}

			//void SaveProperties()
			//{
			//	var fullPath = Path.Combine( tempPath, "cache", "properties" );
			//	if( !Directory.Exists( Path.GetDirectoryName( fullPath ) ) )
			//		Directory.CreateDirectory( Path.GetDirectoryName( fullPath ) );

			//	var sb = new StringBuilder();

			//	sb.Append( "Defines: " );
			//	sb.AppendLine( string.Join( Path.PathSeparator, Parser.Defines ) );

			//	foreach( var pair in Parser.CompileFlags )
			//	{
			//		sb.Append( pair.Key );
			//		sb.Append( "Flags: " );
			//		sb.AppendLine( pair.Value );
			//	}

			//	//foreach( var pair in Parser.Compilers )
			//	//{
			//	//	sb.Append( pair.Key );
			//	//	sb.Append( "Compiler: " );
			//	//	sb.AppendLine( pair.Value );
			//	//}

			//	File.WriteAllText( fullPath, sb.ToString() );
			//}

			//void SaveTimestamps()
			//{
			//	var sb = new StringBuilder();
			//	var processed = new HashSet<string>( StringComparer.CurrentCultureIgnoreCase );
			//	foreach( var info in CompileList )
			//	{
			//		var depFile = Path.ChangeExtension( Path.Combine( tempPath, info.outPath ), ".d" );
			//		if( !File.Exists( depFile ) )
			//		{
			//			Console.WriteLine( "Unable to find dependencies file: " + depFile );
			//			continue;
			//		}

			//		foreach( var line in File.ReadAllLines( depFile ) )
			//		{
			//			var str = line.Trim();
			//			if( str.EndsWith( '\\' ) )
			//				str = str.Substring( 0, str.Length - 1 ).Trim();
			//			foreach( var arg in CommandLineParser.SplitCommandLineIntoArguments( str, true ) )
			//			{
			//				var sourcePath = arg.Trim();
			//				if( sourcePath.EndsWith( ':' ) )
			//					continue;
			//				sourcePath = Parser.GetFullSourcePath( sourcePath ).Replace( '\\', '/' );
			//				if( processed.Add( sourcePath ) )
			//				{
			//					sb.Append( File.GetLastWriteTime( sourcePath ).ToBinary() );
			//					sb.Append( Path.PathSeparator );
			//					sb.AppendLine( sourcePath );
			//				}
			//			}
			//		}
			//	}

			//	var fullPath = Path.Combine( tempPath, "cache", "timestamps" );
			//	if( !Directory.Exists( Path.GetDirectoryName( fullPath ) ) )
			//		Directory.CreateDirectory( Path.GetDirectoryName( fullPath ) );

			//	File.WriteAllText( fullPath, sb.ToString() );
			//}

			void Prepare()// bool forceRecompile )
			{
				FindTools();

				outputPath = Parser.GetFullSourcePath( Parser.OutputFilePath );
				tempPath = Parser.GetFullSourcePath( Parser.TempFolder );

				//if( forceRecompile || IsPropertiesChanged() )
				{
					//// Изменились параметры компиляции, по этому необходимо перекомпилировать все файлы
					//CachedTimestamps = new Dictionary<string, DateTime>();

					//// Удаление файла с параметрами предыдущей компиляции, чтобы не возникало конфликтов, если компиляция была неожиданно приостановлена
					//var fullPath = Path.Combine( tempPath, "cache", "properties" );
					//if( File.Exists( fullPath ) )
					//	File.Delete( fullPath );

					//clear temp folder
					if( Directory.Exists( tempPath ) )
						IOUtility.ClearDirectory( tempPath );

					//delete output file
					if( File.Exists( outputPath ) )
						File.Delete( outputPath );
				}
				//else
				//{
				//	LoadTimestamps();
				//}

				Console.WriteLine( "Collecting sources" );
				var sourcesCollector = new SourcesCollectContext( this );
				sourcesCollector.Collect();
				Console.WriteLine( $"Found {CompileList.Count} sources" );
				if( ClientData != null )
					PlatformServer.SendShowMessageToClient( ClientData, $"Found {CompileList.Count} sources" );

				CollectFolderIncludes();
			}

			void FindTools()
			{
				if( Parser.Platform == "Web" )
				{
					var files = Directory.GetFiles( Parser.EmscriptenFolder, "python.exe", SearchOption.AllDirectories );
					if( files.Length == 0 )
						throw new Exception( "Unable to find python.exe in Emscripten folder" );
					Parser.PythonPath = files[ 0 ];
				}


				//foreach( var pair in Parser.Compilers )
				//{
				//	var path = FindExecutablePath( pair.Value );

				//	//!!!!temp
				//	if( string.IsNullOrEmpty( path ) )
				//		path = @"C:\emsdk\emsdk\bazel\emscripten_toolchain\" + pair.Value;

				//	if( string.IsNullOrEmpty( path ) )
				//		throw new Exception( "Unable to find " + pair.Value );
				//	CompilerPaths[ pair.Key ] = path;
				//}

				//{
				//	var emar = FindExecutablePath( "emar.bat" );

				//	//!!!!temp
				//	if( string.IsNullOrEmpty( emar ) )
				//		emar = @"C:\emsdk\emsdk\bazel\emscripten_toolchain\emar.bat";

				//	if( string.IsNullOrEmpty( emar ) )
				//		throw new Exception( "Unable to find emar" );
				//	emarPath = emar;
				//}
			}

			//bool IsPropertiesChanged()
			//{
			//	// Файл хранит в себе параметры, использованные в предыдущей компиляции
			//	var fullPath = Path.Combine( tempPath, "cache", "properties" );
			//	if( !File.Exists( fullPath ) )
			//		return true;
			//	var compileFlags = new Dictionary<string, string>();
			//	var compilers = new Dictionary<string, string>();
			//	foreach( var line in File.ReadAllLines( fullPath ) )
			//	{
			//		if( string.IsNullOrWhiteSpace( line ) ) continue;

			//		var separatorIndex = line.IndexOf( ':' );
			//		if( separatorIndex < 0 ) continue;

			//		var prefix = line.Substring( 0, separatorIndex ).Trim().ToUpper();
			//		var value = line.Substring( separatorIndex + 1 ).Trim();
			//		switch( prefix )
			//		{
			//		case "DEFINES":
			//			{
			//				var defines = value.Split( Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries );
			//				if( !Parser.Defines.SequenceEqual( defines ) )
			//					return true;
			//			}
			//			break;
			//		default:
			//			if( prefix.EndsWith( "FLAGS", StringComparison.InvariantCultureIgnoreCase ) )
			//			{
			//				var ext = prefix.Substring( 0, prefix.Length - 5 ).ToLowerInvariant();
			//				compileFlags[ ext ] = value;
			//			}
			//			if( prefix.EndsWith( "COMPILER", StringComparison.InvariantCultureIgnoreCase ) )
			//			{
			//				var ext = prefix.Substring( 0, prefix.Length - 8 ).ToLowerInvariant();
			//				compilers[ ext ] = value;
			//			}
			//			break;
			//		}
			//	}
			//	if( Parser.CompileFlags.Count != compileFlags.Count )
			//	{
			//		return true;
			//	}
			//	foreach( var pair in compileFlags )
			//	{
			//		if( !Parser.CompileFlags.TryGetValue( pair.Key, out var value ) || value != pair.Value )
			//			return true;
			//	}
			//	//if( Parser.Compilers.Count != compilers.Count )
			//	//	return true;
			//	//foreach( var pair in compilers )
			//	//{
			//	//	if( !Parser.Compilers.TryGetValue( pair.Key, out var value ) || value != pair.Value )
			//	//		return true;
			//	//}
			//	return false;
			//}

			//void LoadTimestamps()
			//{
			//	var timestamps = new Dictionary<string, DateTime>( StringComparer.CurrentCultureIgnoreCase );

			//	// Файл хранит в себе временные метки всех файлов
			//	var fullPath = Path.Combine( tempPath, "cache", "timestamps" );
			//	if( File.Exists( fullPath ) )
			//	{
			//		foreach( var line in File.ReadAllLines( fullPath ) )
			//		{
			//			if( string.IsNullOrWhiteSpace( line ) ) continue;
			//			try
			//			{
			//				var parts = line.Split( Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries );
			//				if( parts.Length != 2 ) continue;
			//				if( !long.TryParse( parts[ 0 ].Trim(), out var binaryTimestamp ) )
			//					return;
			//				var path = parts[ 1 ].Trim().Replace( '\\', '/' );
			//				timestamps[ path ] = DateTime.FromBinary( binaryTimestamp );
			//			}
			//			catch { }
			//		}
			//	}
			//	CachedTimestamps = timestamps;
			//}

			void CollectFolderIncludes()
			{
				var processedFolders = new HashSet<string>( StringComparer.CurrentCultureIgnoreCase );
				foreach( var path in Parser.HeaderFolders )
				{
					var fullPath = Parser.GetFullSourcePath( path );
					if( processedFolders.Add( fullPath.Replace( '\\', '/' ) ) )
						FolderIncludes.Add( path );
				}
			}

			static string PreparePath( string path )
			{
				if( path.Contains( ' ' ) )
					path = $"\"{path}\"";
				return path.Replace( '\\', '/' );
			}

			static string PrepareArg( string value )
			{
				if( value.Contains( ' ' ) )
					value = $"\"{value}\"";
				return value;
			}

			//static string? FindExecutablePath( string name )
			//{
			//	var enviromentPath = Environment.GetEnvironmentVariable( "PATH" );
			//	var paths = enviromentPath.Split( ';' );
			//	return paths.Select( x => Path.Combine( x, name ) ).Where( File.Exists ).FirstOrDefault();
			//}

			readonly struct SourcesCollectContext
			{
				readonly LibCompiler compiler;
				readonly HashSet<string> processedFiles;
				readonly HashSet<string> usedFileNames;

				public SourcesCollectContext( LibCompiler compiler )
				{
					this.compiler = compiler;

					processedFiles = new( StringComparer.CurrentCultureIgnoreCase );
					usedFileNames = new( StringComparer.CurrentCultureIgnoreCase );
				}

				public void Collect()
				{
					foreach( var path in compiler.Parser.SourceFiles )
					{
						if( path.EndsWith( ".vcxproj", StringComparison.CurrentCultureIgnoreCase ) )
							ParseProject( path );
						else
							TryPickSource( path );
					}
				}

				void ParseProject( string path )
				{
					var fullPath = compiler.Parser.GetFullSourcePath( path );
					if( !processedFiles.Add( fullPath.Replace( '\\', '/' ) ) )
						return;

					if( !File.Exists( fullPath ) )
						throw new Exception( "File not found: " + fullPath );

					string basePath = Path.GetDirectoryName( path );

					var xmldoc2 = new XmlDocument();
					xmldoc2.Load( fullPath );

					var mgr2 = new XmlNamespaceManager( xmldoc2.NameTable );
					mgr2.AddNamespace( "df", xmldoc2.DocumentElement.NamespaceURI );//"http://schemas.microsoft.com/developer/msbuild/2003" );

					//ClCompile Include
					{
						var list = xmldoc2.SelectNodes( "//df:ClCompile", mgr2 );

						foreach( XmlNode node in list )
						{
							var attr = node.Attributes[ "Include" ];
							if( attr != null )
							{
								var includePath = attr.Value;
								if( !Path.IsPathRooted( path ) )
									includePath = Path.Join( basePath, includePath );
								TryPickSource( PathUtility.NormalizePath( includePath ) );
							}
						}
					}
				}

				void TryPickSource( string path )
				{
					var fullPath = compiler.Parser.GetFullSourcePath( path );
					if( !processedFiles.Add( fullPath.Replace( '\\', '/' ) ) )
						return;

					if( compiler.Parser.ExcludeSources.Contains( fullPath.Replace( '\\', '/' ) ) )
						return;


					////!!!!temp
					//if( compiler.Parser.Platform == "macOS" || compiler.Parser.Platform == "iOS" )
					//{
					//	if( !fullPath.Contains( "CRTMemoryManager.cpp" ) )
					//		return;
					//	//if( !fullPath.Contains( "MemoryManagerInternal.cpp" ) && !fullPath.Contains( "CRTMemoryManager.cpp" ) )
					//	//	return;
					//}


					if( !File.Exists( fullPath ) )
						throw new Exception( "File not found: " + fullPath );

					var ext = Path.GetExtension( path ).ToLower().Substring( 1 );
					if( compiler.Parser.CompileFlags.ContainsKey( ext ) )
						compiler.CompileList.Add( (fullPath, GetOutputName( fullPath ) + ".o", ext) );
					else
						throw new Exception( "Unknown file type: " + fullPath );
				}

				string GetOutputName( string filePath )
				{
					var name = Path.GetFileNameWithoutExtension( filePath );
					if( usedFileNames.Add( name ) )
						return name;

					//throw new Exception( "Duplicate file name found: " + name );
					var baseName = name;
					int counter = 1;
					do
					{
						name = $"{baseName}.{counter}";
						counter++;
					}
					while( !usedFileNames.Add( name ) );
					return name;
				}
			}
		}

		public static bool Process( PlatformServer.ClientData clientData )
		{
			Console.WriteLine( "CommandLineTools: Compile." );

			if( !SystemSettings.CommandLineParameters.TryGetValue( "-compile", out var compileFilePath ) )
				return false;

			//bool forceRecompile = false;
			//if( SystemSettings.CommandLineParameters.TryGetValue( "-forceRecompile", out var forceRecompileString ) )
			//{
			//	bool.TryParse( forceRecompileString, out forceRecompile );
			//	if( forceRecompileString == "1" )
			//		forceRecompile = true;
			//}

			bool singleTask = SystemSettings.CommandLineParameters.ContainsKey( "-no-tasks" );

			//always force recompile for now
			//var forceRecompile = true;

			var executableDirectory = AppContext.BaseDirectory;
			var compileFileFullPath = Path.GetFullPath( Path.Combine( executableDirectory, compileFilePath ) );

			try
			{
				var parser = new CompileFileParser( compileFileFullPath, false );
				parser.Parse();
				parser.Print();

				//if( forceRecompile )
				//	Console.WriteLine( "Compiling with the -forceRecompile flag, all previous compilation results will be discarded." );

				var compiler = new LibCompiler( parser, clientData );
				return compiler.CompileAsync( /*forceRecompile,*/ singleTask ).GetAwaiter().GetResult();
			}
			catch( Exception e )
			{
				Console.WriteLine( $"Error: {e.Message}" );
			}

			return false;
		}
	}
}