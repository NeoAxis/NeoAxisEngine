// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using NeoAxis;
using System.Text;

namespace CommandLineTools
{
	public class CompileFileParser
	{
		public string CompileFileFullPath;

		public string Platform;
		public HashSet<string> Includes = new HashSet<string>();
		public HashSet<string> Defines = new HashSet<string>();
		public List<string> SourceFiles = new List<string>();
		public HashSet<string> ExcludeSources = new HashSet<string>( StringComparer.CurrentCultureIgnoreCase );
		public List<string> HeaderFolders = new List<string>();
		public string TempFolder;
		public string EmscriptenFolder;
		public string OutputFilePath;
		public Dictionary<string, string> CompileFlags = new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase );
		//public Dictionary<string, string> Compilers = new Dictionary<string, string>( StringComparer.CurrentCultureIgnoreCase );
		public string PythonPath;

		//remote compilation
		public bool RemoteCompilation;
		public string Remote_SourceRootFolder;
		public HashSet<string> Remote_SourceFolders = new HashSet<string>();
		public HashSet<string> Remote_SourceFileExtensions = new HashSet<string>();

		//

		public CompileFileParser( string compileFileFullPath, bool remoteCompilation )
		{
			CompileFileFullPath = compileFileFullPath;
			RemoteCompilation = remoteCompilation;
		}

		public string CompileFileDirectory
		{
			get { return Path.GetDirectoryName( CompileFileFullPath ); }
		}

		public void Parse()
		{
			var context = new ParseContext( this );
			context.Parse();
		}

		public void Print()
		{
			Console.WriteLine( $"Platform: {Platform}" );
			Console.WriteLine( "Defines: " + string.Join( ", ", Defines ) );
			Console.WriteLine( "Source Files: " );
			foreach( var sourceFile in SourceFiles )
				Console.WriteLine( $"- {sourceFile}" );
			Console.WriteLine( "Header Folders: " );
			foreach( var headerFolder in HeaderFolders )
				Console.WriteLine( $"- {headerFolder}" );
			Console.WriteLine( $"Temp Folder: {TempFolder}" );
			Console.WriteLine( $"Emscripten Folder: {EmscriptenFolder}" );
			Console.WriteLine( $"Output: {OutputFilePath}" );

			//remote compilation
			if( RemoteCompilation )
			{
				Console.WriteLine( $"Source Root Folder: {Remote_SourceRootFolder}" );
				Console.WriteLine( "Source Folders: " );
				foreach( var sourceFolder in Remote_SourceFolders )
					Console.WriteLine( $"- {sourceFolder}" );
				Console.WriteLine( "Source File Extensions: " );
				foreach( var sourceFileExtension in Remote_SourceFileExtensions )
					Console.WriteLine( $"- {sourceFileExtension}" );
			}
		}

		public string GetFullSourcePath( string path )
		{
			if( Path.IsPathRooted( path ) )
				return Path.GetFullPath( path );
			return Path.GetFullPath( Path.Combine( CompileFileDirectory, path ) );
		}

		struct ParseContext
		{
			readonly CompileFileParser parser;

			//

			public ParseContext( CompileFileParser parser )
			{
				this.parser = parser;
			}

			public void Parse()
			{
				if( !File.Exists( parser.CompileFileFullPath ) )
					throw new FileNotFoundException( "File not found. " + parser.CompileFileFullPath );

				var lines = File.ReadAllLines( parser.CompileFileFullPath );
				foreach( var line in lines )
					ParseLine( line );
			}

			void ParseLine( string line )
			{
				//clear string from comments and replace all symbols to upper case for easier checking
				line = line.Trim();
				var parts = line.Split( new[] { ':' }, 2 );

				if( parts.Length < 2 )
					return;

				var prefix = parts[ 0 ].Trim();
				var value = parts[ 1 ].Trim();

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
					foreach( var v in EnumerateWildcardFiles( parser.GetFullSourcePath( TrimAndNormalizePath( value ) ) ) )
						parser.ExcludeSources.Add( v.Replace( '\\', '/' ) );
					break;
				case "SOURCE":
					foreach( var v in EnumerateWildcardFiles( parser.GetFullSourcePath( TrimAndNormalizePath( value ) ) ) )
						parser.SourceFiles.Add( TrimAndNormalizePath( Path.GetRelativePath( parser.CompileFileDirectory, v ) ) );
					break;
				case "TEMPFOLDER":
					parser.TempFolder = TrimAndNormalizePath( value );
					break;
				case "EMSCRIPTENFOLDER":
					parser.EmscriptenFolder = TrimAndNormalizePath( value );
					break;
				case "OUTPUT":
					parser.OutputFilePath = TrimAndNormalizePath( value );
					break;
				//!!!!not used now. maybe later
				//case "INCLUDE":
				//	value = parser.GetFullSourcePath( PreparePath( value ) );
				//	if( parser.Includes.Add( value.Replace( '\\', '/' ) ) )
				//	{
				//		var ctx = new ParseContext( parser, basePath );
				//		ctx.Parse( value );
				//	}
				//	break;
				case "HEADERFOLDER":
					parser.HeaderFolders.Add( TrimAndNormalizePath( value ) );
					break;

				//remote compilation
				case "SOURCEROOTFOLDER":
					parser.Remote_SourceRootFolder = Path.GetFullPath( Path.Combine( parser.CompileFileDirectory, TrimAndNormalizePath( value ) ) );
					//parser.SourceRootFolder = Path.GetFullPath( Path.Combine( parser.RootPath, TrimAndNormalizePath( value ) ) );
					break;
				case "SOURCEFOLDER":
					parser.Remote_SourceFolders.Add( TrimAndNormalizePath( value ) );
					break;
				case "SOURCEFILEEXTENSION":
					parser.Remote_SourceFileExtensions.Add( value );
					break;

				default:
					if( prefix.EndsWith( "FLAGS", StringComparison.InvariantCultureIgnoreCase ) )
					{
						var ext = prefix.Substring( 0, prefix.Length - 5 ).ToLowerInvariant();
						if( !parser.CompileFlags.TryGetValue( ext, out flags ) )
							flags = "";
						parser.CompileFlags[ ext ] = flags + value;
					}
					//if( prefix.EndsWith( "COMPILER", StringComparison.InvariantCultureIgnoreCase ) )
					//{
					//	var ext = prefix.Substring( 0, prefix.Length - 8 ).ToLowerInvariant();
					//	parser.Compilers[ ext ] = value;
					//}
					break;
				}
			}

			string TrimAndNormalizePath( string path )
			{
				return path.Trim( ' ', '"' ).Replace( '\\', '/' );
			}

			static IEnumerable<string> EnumerateWildcardFiles( string path )
			{
				int index = path.IndexOf( '*' );
				if( index < 0 )
					return Enumerable.Repeat( path, 1 );
				if( path.IndexOf( "**", index ) == index )
					return Directory.EnumerateFiles( path.Substring( 0, index ), Path.GetFileName( path ), SearchOption.AllDirectories );
				return Directory.EnumerateFiles( Path.GetDirectoryName( path ), Path.GetFileName( path ), SearchOption.TopDirectoryOnly );
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static class CommandLineParser
	{
		public static string[] SplitCommandLineIntoArguments( string commandLine, bool removeHashComments )
		{
			if( commandLine == null )
				throw new ArgumentNullException( nameof( commandLine ) );

			var args = new List<string>();
			var current = new StringBuilder();

			bool inQuotes = false;
			int backslashCount = 0;
			bool atStartOfToken = true;

			for( int i = 0; i < commandLine.Length; i++ )
			{
				char c = commandLine[ i ];

				if( c == '\\' )
				{
					backslashCount++;
					atStartOfToken = false;
					continue;
				}

				if( c == '"' )
				{
					// Поддержка стандартной Windows-логики экранирования кавычек слэшами
					current.Append( '\\', backslashCount / 2 );

					if( backslashCount % 2 == 0 )
					{
						inQuotes = !inQuotes;
					}
					else
					{
						current.Append( '"' );
					}

					backslashCount = 0;
					atStartOfToken = false;
					continue;
				}

				// Если перед этим были слэши — выводим их
				if( backslashCount > 0 )
				{
					current.Append( '\\', backslashCount );
					backslashCount = 0;
				}

				// Комментарий с # только в начале аргумента и только вне кавычек
				if( removeHashComments && !inQuotes && atStartOfToken && c == '#' )
				{
					break;
				}

				if( char.IsWhiteSpace( c ) && !inQuotes )
				{
					if( current.Length > 0 )
					{
						args.Add( current.ToString() );
						current.Clear();
						atStartOfToken = true;
					}

					continue;
				}

				current.Append( c );
				atStartOfToken = false;
			}

			if( backslashCount > 0 )
				current.Append( '\\', backslashCount );

			if( current.Length > 0 )
				args.Add( current.ToString() );

			return args.ToArray();
		}
	}
}