// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for working with strings.
	/// </summary>
	public static class StringUtility
	{
		public static bool IsCorrectIdentifierName( string name )
		{
			if( string.IsNullOrEmpty( name ) )
				return false;

			{
				char c = name[ 0 ];
				if( !( ( c >= 'A' && c <= 'Z' ) || ( c >= 'a' && c <= 'z' ) ||
					c == '_' || c == '$' || c == '#' ) )
				{
					return false;
				}
			}

			for( int z = 1; z < name.Length; z++ )
			{
				char c = name[ z ];
				if( !( ( c >= 'A' && c <= 'Z' ) || ( c >= 'a' && c <= 'z' ) ||
					( c >= '0' && c <= '9' ) || c == '_' || c == '.' || c == '-' ) )
				{
					return false;
				}
			}
			return true;
		}

		public static string EncodeDelimiterFormatString( string text )
		{
			StringBuilder builder = new StringBuilder( "", text.Length + 2 );

			foreach( char c in text )
			{
				switch( c )
				{
				case '\n': builder.Append( "\\n" ); break;
				case '\r': builder.Append( "\\r" ); break;
				case '\t': builder.Append( "\\t" ); break;
				case '\'': builder.Append( "\\\'" ); break;
				case '\"': builder.Append( "\\\"" ); break;
				case '\\': builder.Append( "\\\\" ); break;

				default:
					if( (int)c < 32 || (int)c >= 127 )
						builder.Append( "\\x" + ( (int)c ).ToString( "x04" ) );
					else
						builder.Append( c );
					break;
				}
			}

			return builder.ToString();
		}

		internal static void DecodeDelimiterFormatString( StringBuilder outBuilder, string text )
		{
			for( int n = 0; n < text.Length; n++ )
			{
				char c = text[ n ];

				if( c == '\\' )
				{
					n++;

					char c2 = text[ n ];

					switch( c2 )
					{
					case 'n': outBuilder.Append( '\n' ); break;
					case 'r': outBuilder.Append( '\r' ); break;
					case 't': outBuilder.Append( '\t' ); break;
					case '\'': outBuilder.Append( '\'' ); break;
					case '"': outBuilder.Append( '"' ); break;
					case '\\': outBuilder.Append( '\\' ); break;

					case 'x':
						{
							if( n + 4 >= text.Length )
								throw new Exception( "Invalid string format" );

							int[] values = new int[ 4 ];
							for( int z = 0; z < 4; z++ )
							{
								char cc = text[ n + 1 + z ];

								if( cc >= '0' && cc <= '9' )
									values[ z ] = (int)cc - (int)'0';
								else if( cc >= 'a' && cc <= 'f' )
									values[ z ] = 10 + (int)cc - (int)'a';
								else if( cc >= 'A' && cc <= 'F' )
									values[ z ] = 10 + (int)cc - (int)'A';
								else
									throw new Exception( "Invalid string format" );
							}

							int unicodeChar = ( ( values[ 0 ] * 16 + values[ 1 ] ) * 16 +
								values[ 2 ] ) * 16 + values[ 3 ];

							outBuilder.Append( (char)unicodeChar );

							n += 4;
						}
						break;

					default: throw new Exception( "Invalid string format" );
					}
				}
				else
					outBuilder.Append( c );
			}
		}

		public static string DecodeDelimiterFormatString( string text )
		{
			StringBuilder builder = new StringBuilder( "", text.Length + 2 );
			DecodeDelimiterFormatString( builder, text );
			return builder.ToString();
		}

		public static string[] TextWordWrap( string text, int charactersPerLine )
		{
			List<string> strings = new List<string>();

			StringBuilder currentString = new StringBuilder();

			string[] words = text.Split( new char[] { ' ' } );
			foreach( string word in words )
			{
				if( currentString.Length + 1 + word.Length > charactersPerLine )
				{
					if( currentString.Length != 0 )
						strings.Add( currentString.ToString() );
					currentString.Length = 0;
				}
				if( currentString.Length != 0 )
					currentString.Append( " " );
				currentString.Append( word );
			}

			if( currentString.Length != 0 )
				strings.Add( currentString.ToString() );

			return strings.ToArray();
		}

		static unsafe int ParseInt( char* input, int len )
		{
			int pos = 0;           // read pointer position
			int part = 0;          // the current part (int, float and sci parts of the number)
			bool neg = false;      // true if part is a negative number

			//int* ret = stackalloc int[ 1 ];

			while( pos < len && ( *( input + pos ) > '9' || *( input + pos ) < '0' ) && *( input + pos ) != '-' )
				pos++;

			// sign
			if( *( input + pos ) == '-' )
			{
				neg = true;
				pos++;
			}

			// integer part
			while( pos < len && !( input[ pos ] > '9' || input[ pos ] < '0' ) )
				part = part * 10 + ( input[ pos++ ] - '0' );

			return neg ? ( part * -1 ) : part;
			//*ret = neg ? ( part * -1 ) : part;
			//return *ret;
		}

		public unsafe static bool StringToIntegers( string s, int[] result )
		//public unsafe static int[] StringToInts( string s, int length )
		{
			//int[] ints = new int[ length ];
			int index = 0;
			int startpos = 0;

			fixed( char* pStringBuffer = s )
			{
				fixed( int* pIntBuffer = result )//fixed ( int* pIntBuffer = ints )
				{
					for( int n = 0; n < s.Length; n++ )
					{
						if( s[ n ] == ' ' || n == s.Length - 1 )
						{
							if( n == s.Length - 1 )
								n++;

							if( index == result.Length )
								return false;

							// pIntBuffer[index++] = int.Parse(new string(pStringBuffer, startpos, n - startpos));
							pIntBuffer[ index++ ] = ParseInt( ( pStringBuffer + startpos ), n - startpos );
							startpos = n + 1;
						}
					}
				}
			}

			if( index != result.Length )
				return false;
			return true;

			//return ints;
		}

		public static string ToUpperFirstCharacter( string text )
		{
			if( text.Length > 0 )
				return char.ToUpper( text[ 0 ] ).ToString() + text.Substring( 1 );
			return text;
		}

		public static string ToLowerFirstCharacter( string text )
		{
			if( text.Length > 0 )
				return char.ToLower( text[ 0 ] ).ToString() + text.Substring( 1 );
			return text;
		}

		public static int GetStableHashCode( string str )
		{
			unchecked
			{
				int hash1 = 5381;
				int hash2 = hash1;

				for( int i = 0; i < str.Length && str[ i ] != '\0'; i += 2 )
				{
					hash1 = ( ( hash1 << 5 ) + hash1 ) ^ str[ i ];
					if( i == str.Length - 1 || str[ i + 1 ] == '\0' )
						break;
					hash2 = ( ( hash2 << 5 ) + hash2 ) ^ str[ i + 1 ];
				}

				return hash1 + ( hash2 * 1566083941 );
			}
		}

		public static string Construct( string value )
		{
			return value;
		}

		public static string EncodeToBase64URL( string value )
		{
			return Convert.ToBase64String( Encoding.UTF8.GetBytes( value ) ).Replace( "=", "" ).Replace( '+', '-' ).Replace( '/', '_' );
		}

		public static string DecodeFromBase64URL( string base64URL )
		{
			var v = base64URL.Replace( '-', '+' ).Replace( '_', '/' ).PadRight( 4 * ( ( base64URL.Length + 3 ) / 4 ), '=' );
			return Encoding.UTF8.GetString( Convert.FromBase64String( v ) );
		}
	}
}
