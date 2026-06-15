// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System.Text;

namespace CommandLineTools
{
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