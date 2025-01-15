// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.Text;
using System.Xml;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using NeoAxis;

namespace CommandLineTools
{
	public static class Compile
	{


		/*
Напиши парсер на C# формата файла .compile. Построчный, вначале префикс с двоеточием.

Пример файла:

Platform: Web
Defines: список дефайнов.
Source: "относительный путь к файлу .vcxproj" - берет список из vcxproj файла.
Source: "относительный путь к файлу/File.cpp"
Source: "относительный путь к файлу/File*.cpp" - можно использовать маску.
TempFolder: "относительный или полный путь к папке типа _build для .o и других файлов".
Output: "относительный или полный путь к выходному файлу".
		*/


		//!!!!сгенерил chatgpt


		public class CompileFileParser
		{
			public string CompileFilePath { get; private set; }
			public string Platform { get; private set; }
			public HashSet<string> Includes { get; private set; }
			public HashSet<string> Defines { get; private set; }
			public List<string> SourceFiles { get; private set; }
			public HashSet<string> ExcludeSources { get; private set; }
			public List<string> HeaderFolders { get; private set; }
			public string TempFolder { get; private set; }
			public string Output { get; private set; }
			public Dictionary<string, string> CompileFlags { get; private set; }
			public Dictionary<string, string> Compilers { get; private set; }

			/// <summary>
			/// Корневая директория, которая будет использоваться в качесте Working Directory для команды компилятора.
			/// Необходима, чтобы указывать относительные пути файлов и папок, а не полные, так как максимальная длина команды ограничена и необходимо по максимуму уменьшать длину команды.
			/// </summary>
			public string RootPath { get; private set; }

			public CompileFileParser()
			{
				Includes = new HashSet<string>();
				ExcludeSources = new HashSet<string>( StringComparer.CurrentCultureIgnoreCase );
				Defines = new HashSet<string>();
				SourceFiles = new List<string>();
				HeaderFolders = new List<string>();
				CompileFlags = new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase );
				Compilers = new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase );
			}

			public void Parse( string filePath )
			{
				CompileFilePath = filePath;
				RootPath = Path.GetFullPath( Path.GetDirectoryName( filePath ) );
				var ctx = new ParseContext( this, "" );
				ctx.Parse( Path.GetFileName( filePath ) );
			}

			public void Print()
			{
				Console.WriteLine( $"Platform: {Platform}" );
				Console.WriteLine( "Defines: " + string.Join( ", ", Defines ) );
				Console.WriteLine( "Source Files: " );
				foreach( var sourceFile in SourceFiles )
				{
					Console.WriteLine( $"- {sourceFile}" );
				}
				Console.WriteLine( "Header Folders: " );
				foreach( var headerFolder in HeaderFolders )
				{
					Console.WriteLine( $"- {headerFolder}" );
				}
				Console.WriteLine( $"Temp Folder: {TempFolder}" );
				Console.WriteLine( $"Output: {Output}" );
			}

			public string GetFullSourcePath( string path )
			{
				if( Path.IsPathRooted( path ) )
				{
					return Path.GetFullPath( path );
				}
				return Path.GetFullPath( Path.GetFullPath( Path.Combine( RootPath, path ) ) );
			}

			private struct ParseContext
			{
				private readonly CompileFileParser parser;

				/// <summary>
				/// Директория .compile файла, относительно корневой директории
				/// </summary>
				private string basePath;

				public ParseContext( CompileFileParser parser, string basePath )
				{
					this.parser = parser;
					this.basePath = basePath;
				}

				public void Parse( string filePath )
				{
					if( !Path.IsPathRooted( filePath ) )
					{
						filePath = Path.Join( basePath, filePath );
					}

					var fullPath = parser.GetFullSourcePath( Path.Join( basePath, filePath ) );
					if( !File.Exists( fullPath ) )
					{
						throw new FileNotFoundException( "Файл не найден.", filePath );
					}

					basePath = Path.GetDirectoryName( filePath );

					var lines = File.ReadAllLines( fullPath );
					foreach( var line in lines )
					{
						ParseLine( line );
					}
				}

				private readonly void ParseLine( string line )
				{
					// Очищаем строку от пробелов и заменяем все символы на верхний регистр для упрощения проверки
					line = line.Trim();
					var parts = line.Split( new[] { ':' }, 2 );

					if( parts.Length < 2 ) return; // Если нет префикса, пропускаем строку

					var prefix = parts[ 0 ].Trim();
					var value = parts[ 1 ].Trim();


					//!!!!тут он лищние запятые ввел?

					string flags;
					switch( prefix.ToUpper() )
					{
					case "PLATFORM":
						parser.Platform = value;
						break;
					case "DEFINES":
						parser.Defines.UnionWith( value.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries )
							.Select( v => v.Trim( ' ', '"' ) ) );
						break;
					case "EXCLUDESOURCE":
						foreach( var v in EnumerateWildcardFiles( parser.GetFullSourcePath( PreparePath( value ) ) ) )
						{
							parser.ExcludeSources.Add( v.Replace( '\\', '/' ) );
						}
						break;
					case "SOURCE":
						var baseDir = parser.GetFullSourcePath( basePath );
						foreach( var v in EnumerateWildcardFiles( parser.GetFullSourcePath( PreparePath( value ) ) ) )
						{
							parser.SourceFiles.Add( PreparePath( Path.GetRelativePath( baseDir, v ) ) );
						}
						break;
					case "TEMPFOLDER":
						parser.TempFolder = PreparePath( value );
						break;
					case "OUTPUT":
						parser.Output = PreparePath( value );
						break;
					case "INCLUDE":
						value = parser.GetFullSourcePath( PreparePath( value ) );
						if( parser.Includes.Add( value.Replace( '\\', '/' ) ) )
						{
							var ctx = new ParseContext( parser, basePath );
							ctx.Parse( value );
						}
						break;
					case "HEADERFOLDER":
						parser.HeaderFolders.Add( PreparePath( value ) );
						break;
					default:
						if( prefix.EndsWith( "FLAGS", StringComparison.InvariantCultureIgnoreCase ) )
						{
							var ext = prefix.Substring( 0, prefix.Length - 5 ).ToLowerInvariant();
							if( !parser.CompileFlags.TryGetValue( ext, out flags ) )
							{
								flags = "";
							}
							parser.CompileFlags[ ext ] = flags + value;
						}
						if( prefix.EndsWith( "COMPILER", StringComparison.InvariantCultureIgnoreCase ) )
						{
							var ext = prefix.Substring( 0, prefix.Length - 8 ).ToLowerInvariant();
							parser.Compilers[ ext ] = value;
						}
						break;
					}
				}

				private readonly string PreparePath( string path )
				{
					path = path.Trim( ' ', '"' ); // Убираем кавычки и пробелы
					if( !Path.IsPathRooted( path ) ) path = Path.Join( basePath, path );
					return path.Replace( '\\', '/' );
				}

				private static IEnumerable<string> EnumerateWildcardFiles( string path )
				{
					int index = path.IndexOf( '*' );
					if( index < 0 )
					{
						return Enumerable.Repeat( path, 1 );
					}
					if( path.IndexOf( "**", index ) == index )
					{
						return Directory.EnumerateFiles( path.Substring( 0, index ), Path.GetFileName( path ), SearchOption.AllDirectories );
					}
					return Directory.EnumerateFiles( Path.GetDirectoryName( path ), Path.GetFileName( path ), SearchOption.TopDirectoryOnly );
				}
			}
		}

		class LibCompiler
		{
			public CompileFileParser Parser { get; private set; }
			public List<(string inPath, string outPath, string type)> CompileList { get; private set; }
			public List<string> FolderIncludes { get; private set; }
			public IReadOnlyDictionary<string, DateTime> CachedTimestamps { get; private set; }
			public Dictionary<string, string> CompilerPaths { get; private set; }

			private string outputPath;
			private string tmpPath;
			private string emarPath;

			private volatile bool isCancelled = false;

			public LibCompiler( CompileFileParser parser )
			{
				Parser = parser;
				CompileList = new List<(string inPath, string outPath, string type)>();
				FolderIncludes = new List<string>();
				CompilerPaths = new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase );
			}

			public async Task<bool> Compile( bool forceRecompile, bool singleThread )
			{
				Prepare( forceRecompile );

				var hasChanges = await CompileSources( singleThread );
				if( isCancelled ) return false;

				if( hasChanges )
				{
					Console.WriteLine( "Archiving the library" );
					await MakeLib();

					if( isCancelled )
					{
						return false;
					}

					Console.WriteLine( "Saving the cache" );
					SaveTimestamps();
					SaveProperties();
					return true;
				}
				else
				{
					Console.WriteLine( "Nothing has changed, compilation is skipped" );
					return true;
				}
			}

			private async Task<bool> CompileSources( bool singleThread )
			{
				if( !Directory.Exists( tmpPath ) ) Directory.CreateDirectory( tmpPath );

				var includes = string.Join( " ", FolderIncludes.Select( i => "-I" + PreparePath( i ) ) );
				var defines = string.Join( " ", Parser.Defines.Select( i => "-D" + PrepareArg( i ) ) );
				var args = $"{includes} {defines}";

				if( singleThread )
				{
					return await CompileBatch( CompileList, args );
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
						tasks[ i ] = CompileBatch( batches[ i ], args );
					}
					var hasChanges = await Task.WhenAll( tasks );

					return hasChanges.Contains( true );
				}
			}

			private async Task MakeLib()
			{
				var outputPath = this.outputPath;
				if( File.Exists( outputPath ) )
				{
					File.Delete( outputPath );
				}
				else if( !Directory.Exists( Path.GetDirectoryName( outputPath ) ) )
				{
					Directory.CreateDirectory( Path.GetDirectoryName( outputPath ) );
				}

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
						await RunCmd( emarPath, sb.ToString(), tmpPath );
						if( isCancelled ) return;
						Console.WriteLine( $"Done {Math.Floor( index * 100 / (float)CompileList.Count )}%" );

						sb.Clear();
						sb.Append( $"rcs {outputPath}" );
					}
					sb.Append( ' ' );
					sb.Append( fileName );
					index++;
				}
				await RunCmd( emarPath, sb.ToString(), tmpPath );
			}

			private async Task<bool> CompileBatch( List<(string inPath, string outPath, string type)> batch, string args )
			{
				bool hasChanges = false;
				foreach( var info in batch )
				{
					if( isCancelled ) break;
					hasChanges |= await CompileFile( info.inPath, info.outPath, info.type, args );
				}
				return hasChanges;
			}

			private async Task<bool> CompileFile( string inPath, string outPath, string type, string args )
			{
				outPath = Parser.GetFullSourcePath( Path.Join( tmpPath, outPath ) );
				if( !ShouldCompileFile( inPath, outPath ) )
				{
					return false;
				}

				inPath = PreparePath( inPath );
				outPath = PreparePath( outPath );

				var flags = Parser.CompileFlags[ type ];
				var compiler = CompilerPaths[ type ];
				var result = await RunCmd( compiler, $"{flags} {args} -o {outPath} -c -MD {inPath}", Parser.RootPath );

				if( result ) Console.WriteLine( $"Compiled {inPath} as {outPath}" );

				return true;
			}

			private bool ShouldCompileFile( string inPath, string outPath )
			{
				if( CheckFileChanged( inPath ) )
				{
					return true;
				}

				if( !File.Exists( outPath ) )
				{
					return true;
				}

				var depFile = Path.ChangeExtension( outPath, ".d" );
				if( !File.Exists( depFile ) )
				{
					return true;
				}

				foreach( var line in File.ReadAllLines( depFile ) )
				{
					var str = line.Trim();
					if( str.EndsWith( '\\' ) )
					{
						str = str.Substring( 0, str.Length - 1 ).Trim();
					}
					foreach( var arg in CommandLineParser.SplitCommandLineIntoArguments( str, true ) )
					{
						var sourcePath = arg.Trim();
						if( sourcePath.EndsWith( ':' ) )
						{
							continue;
						}

						if( CheckFileChanged( sourcePath ) )
						{
							return true;
						}
					}
				}
				return false;
			}

			private bool CheckFileChanged( string path )
			{
				path = Parser.GetFullSourcePath( path ).Replace( '\\', '/' );

				if( !File.Exists( path ) )
				{
					return true;
				}

				if( !CachedTimestamps.TryGetValue( path, out var timestamp ) )
				{
					return true;
				}

				var lastWriteTime = File.GetLastWriteTime( path );
				return timestamp != lastWriteTime;
			}

			private async Task<bool> RunCmd( string prog, string args, string workingDir )
			{
				using Process process = new();
				process.EnableRaisingEvents = true;
				process.StartInfo = new()
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					WorkingDirectory = workingDir,
					FileName = prog,
					Arguments = args
				};

				var tcs = new TaskCompletionSource();
				process.Exited += ( sender, args ) =>
				{
					tcs.SetResult();
					process.Dispose();
				};

				process.OutputDataReceived += ( s, ea ) => Console.WriteLine( ea.Data );
				process.ErrorDataReceived += ( s, ea ) => Console.WriteLine( "[ERROR] " + ea.Data );
				try
				{
					process.Start();

					await tcs.Task;

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
				}
				return false;
			}

			private void SaveProperties()
			{
				var fullPath = Path.Combine( tmpPath, "cache", "properties" );
				if( !Directory.Exists( Path.GetDirectoryName( fullPath ) ) )
				{
					Directory.CreateDirectory( Path.GetDirectoryName( fullPath ) );
				}

				var sb = new StringBuilder();

				sb.Append( "Defines: " );
				sb.AppendLine( string.Join( Path.PathSeparator, Parser.Defines ) );

				foreach( var pair in Parser.CompileFlags )
				{
					sb.Append( pair.Key );
					sb.Append( "Flags: " );
					sb.AppendLine( pair.Value );
				}

				foreach( var pair in Parser.Compilers )
				{
					sb.Append( pair.Key );
					sb.Append( "Compiler: " );
					sb.AppendLine( pair.Value );
				}

				File.WriteAllText( fullPath, sb.ToString() );
			}

			private void SaveTimestamps()
			{
				var sb = new StringBuilder();
				var processed = new HashSet<string>( StringComparer.CurrentCultureIgnoreCase );
				foreach( var info in CompileList )
				{
					var depFile = Path.ChangeExtension( Path.Combine( tmpPath, info.outPath ), ".d" );
					if( !File.Exists( depFile ) )
					{
						Console.WriteLine( "Unable to find dependencies file: " + depFile );
						continue;
					}

					foreach( var line in File.ReadAllLines( depFile ) )
					{
						var str = line.Trim();
						if( str.EndsWith( '\\' ) )
						{
							str = str.Substring( 0, str.Length - 1 ).Trim();
						}
						foreach( var arg in CommandLineParser.SplitCommandLineIntoArguments( str, true ) )
						{
							var sourcePath = arg.Trim();
							if( sourcePath.EndsWith( ':' ) )
							{
								continue;
							}
							sourcePath = Parser.GetFullSourcePath( sourcePath ).Replace( '\\', '/' );
							if( processed.Add( sourcePath ) )
							{
								sb.Append( File.GetLastWriteTime( sourcePath ).ToBinary() );
								sb.Append( Path.PathSeparator );
								sb.AppendLine( sourcePath );
							}
						}
					}
				}

				var fullPath = Path.Combine( tmpPath, "cache", "timestamps" );
				if( !Directory.Exists( Path.GetDirectoryName( fullPath ) ) )
				{
					Directory.CreateDirectory( Path.GetDirectoryName( fullPath ) );
				}

				File.WriteAllText( fullPath, sb.ToString() );
			}

			private void Prepare( bool forceRecompile )
			{
				FindTools();

				outputPath = Parser.GetFullSourcePath( Parser.Output );
				tmpPath = Parser.GetFullSourcePath( Parser.TempFolder );

				if( forceRecompile || IsPropertiesChanged() )
				{
					// Изменились параметры компиляции, по этому необходимо перекомпилировать все файлы
					CachedTimestamps = new Dictionary<string, DateTime>();

					// Удаление файла с параметрами предыдущей компиляции, чтобы не возникало конфликтов, если компиляция была неожиданно приостановлена
					var fullPath = Path.Combine( tmpPath, "cache", "properties" );
					if( File.Exists( fullPath ) ) File.Delete( fullPath );
				}
				else
				{
					LoadTimestamps();
				}

				Console.WriteLine( "Collecting sources" );
				var sourcesCollector = new SourcesCollectContext( this );
				sourcesCollector.Collect();
				Console.WriteLine( $"Found {CompileList.Count} sources" );

				CollectFolderIncludes();
			}

			private void FindTools()//TODO: names are different for linux
			{
				foreach( var pair in Parser.Compilers )
				{
					var path = FindExecutablePath( pair.Value );
					if( string.IsNullOrEmpty( path ) ) throw new Exception( "Unable to find " + pair.Value );
					CompilerPaths[ pair.Key ] = path;
				}

				var emar = FindExecutablePath( "emar.bat" );
				if( string.IsNullOrEmpty( emar ) ) throw new Exception( "Unable to find emar" );
				emarPath = emar;
			}

			private bool IsPropertiesChanged()
			{
				// Файл хранит в себе параметры, использованные в предыдущей компиляции
				var fullPath = Path.Combine( tmpPath, "cache", "properties" );
				if( !File.Exists( fullPath ) )
				{
					return true;
				}
				var compileFlags = new Dictionary<string, string>();
				var compilers = new Dictionary<string, string>();
				foreach( var line in File.ReadAllLines( fullPath ) )
				{
					if( string.IsNullOrWhiteSpace( line ) ) continue;

					var separatorIndex = line.IndexOf( ':' );
					if( separatorIndex < 0 ) continue;

					var prefix = line.Substring( 0, separatorIndex ).Trim().ToUpper();
					var value = line.Substring( separatorIndex + 1 ).Trim();
					switch( prefix )
					{
					case "DEFINES":
						{
							var defines = value.Split( Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries );
							if( !Parser.Defines.SequenceEqual( defines ) )
							{
								return true;
							}
						}
						break;
					default:
						if( prefix.EndsWith( "FLAGS", StringComparison.InvariantCultureIgnoreCase ) )
						{
							var ext = prefix.Substring( 0, prefix.Length - 5 ).ToLowerInvariant();
							compileFlags[ ext ] = value;
						}
						if( prefix.EndsWith( "COMPILER", StringComparison.InvariantCultureIgnoreCase ) )
						{
							var ext = prefix.Substring( 0, prefix.Length - 8 ).ToLowerInvariant();
							compilers[ ext ] = value;
						}
						break;
					}
				}
				if( Parser.CompileFlags.Count != compileFlags.Count )
				{
					return true;
				}
				foreach( var pair in compileFlags )
				{
					if( !Parser.CompileFlags.TryGetValue( pair.Key, out var value ) || value != pair.Value )
					{
						return true;
					}
				}
				if( Parser.Compilers.Count != compilers.Count )
				{
					return true;
				}
				foreach( var pair in compilers )
				{
					if( !Parser.Compilers.TryGetValue( pair.Key, out var value ) || value != pair.Value )
					{
						return true;
					}
				}
				return false;
			}

			private void LoadTimestamps()
			{
				var timestamps = new Dictionary<string, DateTime>( StringComparer.CurrentCultureIgnoreCase );

				// Файл хранит в себе временные метки всех файлов
				var fullPath = Path.Combine( tmpPath, "cache", "timestamps" );
				if( File.Exists( fullPath ) )
				{
					foreach( var line in File.ReadAllLines( fullPath ) )
					{
						if( string.IsNullOrWhiteSpace( line ) ) continue;
						try
						{
							var parts = line.Split( Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries );
							if( parts.Length != 2 ) continue;
							if( !long.TryParse( parts[ 0 ].Trim(), out var binaryTimestamp ) ) return;
							var path = parts[ 1 ].Trim().Replace( '\\', '/' );
							timestamps[ path ] = DateTime.FromBinary( binaryTimestamp );
						}
						catch { }
					}
				}
				CachedTimestamps = timestamps;
			}

			private void CollectFolderIncludes()
			{
				var processedFolders = new HashSet<string>( StringComparer.CurrentCultureIgnoreCase );
				foreach( var path in Parser.HeaderFolders )
				{
					var fullPath = Parser.GetFullSourcePath( path );
					if( processedFolders.Add( fullPath.Replace( '\\', '/' ) ) )
					{
						FolderIncludes.Add( path );
					}
				}
			}

			private static string PreparePath( string path )
			{
				if( path.Contains( ' ' ) ) path = $"\"{path}\"";
				return path.Replace( '\\', '/' );
			}

			private static string PrepareArg( string value )
			{
				if( value.Contains( ' ' ) ) value = $"\"{value}\"";
				return value;
			}

			private static string? FindExecutablePath( string name )
			{
				var enviromentPath = Environment.GetEnvironmentVariable( "PATH" );
				var paths = enviromentPath.Split( ';' );
				return paths.Select( x => Path.Combine( x, name ) ).Where( File.Exists ).FirstOrDefault();
			}

			private readonly struct SourcesCollectContext
			{
				private readonly LibCompiler compiler;
				private readonly HashSet<string> processedFiles;
				private readonly HashSet<string> usedFileNames;

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
						{
							ParseProject( path );
						}
						else
						{
							TryPickSource( path );
						}
					}
				}

				private void ParseProject( string path )
				{
					var fullPath = compiler.Parser.GetFullSourcePath( path );
					if( !processedFiles.Add( fullPath.Replace( '\\', '/' ) ) )
					{
						return;
					}

					if( !File.Exists( fullPath ) ) throw new Exception( "File not found: " + fullPath );

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
								{
									includePath = Path.Join( basePath, includePath );
								}
								TryPickSource( includePath );
							}
						}
					}
				}

				private void TryPickSource( string path )
				{
					var fullPath = compiler.Parser.GetFullSourcePath( path );
					if( !processedFiles.Add( fullPath.Replace( '\\', '/' ) ) )
					{
						return;
					}

					if( compiler.Parser.ExcludeSources.Contains( fullPath.Replace( '\\', '/' ) ) )
					{
						return;
					}

					if( !File.Exists( fullPath ) ) throw new Exception( "File not found: " + fullPath );
					var ext = Path.GetExtension( path ).ToLower().Substring( 1 );
					if( compiler.Parser.CompileFlags.ContainsKey( ext ) )
					{
						compiler.CompileList.Add( (fullPath, GetOutputName( fullPath ) + ".o", ext) );
					}
					else
					{
						throw new Exception( "Unknown file type: " + fullPath );
					}
				}

				private string GetOutputName( string filePath )
				{
					var name = Path.GetFileNameWithoutExtension( filePath );
					if( usedFileNames.Add( name ) ) return name;

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

		public static bool Process()
		{
			Console.WriteLine( "CommandLineTools: Compile." );

			if( !SystemSettings.CommandLineParameters.TryGetValue( "-compile", out var compileFilePath ) )
				return false;

			SystemSettings.CommandLineParameters.TryGetValue( "-buildServer", out var buildServer );

			//!!!!check update
			bool forceRecompile = false;
			if( SystemSettings.CommandLineParameters.TryGetValue( "-forceRecompile", out var forceRecompileString ) )
			{
				bool.TryParse( forceRecompileString, out forceRecompile );
				if( forceRecompileString == "1" )
					forceRecompile = true;
			}
			//bool forceRecompile = SystemSettings.CommandLineParameters.ContainsKey( "-force" );

			bool singleThread = SystemSettings.CommandLineParameters.ContainsKey( "-no-tasks" );

			try
			{
				var parser = new CompileFileParser();
				parser.Parse( compileFilePath );
				parser.Print();

				if( forceRecompile )
					Console.WriteLine( "Compiling with the -forceRecompile flag, all previous compilation results will be discarded." );

				if( !string.IsNullOrEmpty( buildServer ) )
				{
					//!!!!update networking
					//var task = CompileOnBuildServer.Process( parser );
					//task.Wait();
					//return task.Result;

					////return CompileOnBuildServer.Process( parser );
				}
				else
				{
					var compiler = new LibCompiler( parser );

					var task = compiler.Compile( forceRecompile, singleThread );
					task.Wait();
					return task.Result;
				}
			}
			catch( Exception ex )
			{
				Console.WriteLine( $"Error: {ex.Message}" );
			}

			return false;
		}
	}
}