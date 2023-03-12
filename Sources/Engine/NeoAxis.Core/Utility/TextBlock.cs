// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// The class that allows to store the text information in the hierarchical form. Supports creation of children and attributes.
	/// </summary>
	public class TextBlock
	{
		TextBlock parent;
		string name;
		string data;

		//TO DO: create lists by request
		//!!!!!EDictionary? для атрибутов - норм вроде. но имена чилдов пересекаться.
		List<TextBlock> children = new List<TextBlock>();
		ReadOnlyCollection<TextBlock> childrenAsReadOnly;
		List<Attribute> attributes = new List<Attribute>();
		ReadOnlyCollection<Attribute> attributesAsReadOnly;

		//

		/// <summary>
		/// Defines <see cref="NeoAxis.TextBlock"/> attribute.
		/// </summary>
		public sealed class Attribute
		{
			internal string name;
			internal string value;

			internal Attribute() { }

			/// <summary>
			/// Gets the attribute name.
			/// </summary>
			public string Name
			{
				get { return name; }
			}

			/// <summary>
			/// Gets the attribute value.
			/// </summary>
			public string Value
			{
				get { return value; }
			}

			/// <summary>
			/// Returns a string that represents the current attribute.
			/// </summary>
			/// <returns>A string that represents the current attribute.</returns>
			public override string ToString()
			{
				return string.Format( "Name: \"{0}\", Value \"{1}\"", name, value );
			}
		}

		/// <summary>
		/// It is applied only to creation root blocks. Not for creation of children.
		/// </summary>
		/// <example>Example of creation of the block and filling by data.
		/// <code>
		/// TextBlock block = new TextBlock();
		/// TextBlock childBlock = block.AddChild( "childBlock", "child block data" );
		/// childBlock.SetAttribute( "attribute", "attribute value" );
		/// </code>
		/// </example>
		/// <seealso cref="NeoAxis.TextBlock.AddChild(string,string)"/>
		/// <seealso cref="NeoAxis.TextBlock.SetAttribute(string,string)"/>
		public TextBlock()
		{
			childrenAsReadOnly = new ReadOnlyCollection<TextBlock>( children );
			attributesAsReadOnly = new ReadOnlyCollection<Attribute>( attributes );
		}

		//Hierarchy

		/// <summary>
		/// Gets the parent block.
		/// </summary>
		public TextBlock Parent
		{
			get { return parent; }
		}

		/// <summary>
		/// Gets or set block name.
		/// </summary>
		public string Name
		{
			get { return name; }
			set
			{
				if( name == value )
					return;
				name = value;

				if( string.IsNullOrEmpty( name ) )
					throw new Exception( "set Name: \"name\" is null or empty." );
			}
		}

		/// <summary>
		/// Gets or set block string data.
		/// </summary>
		public string Data
		{
			get { return data; }
			set { data = value; }
		}

		/// <summary>
		/// Gets the children collection.
		/// </summary>
		public IList<TextBlock> Children
		{
			get { return childrenAsReadOnly; }
		}

		/// <summary>
		/// Finds child block by name.
		/// </summary>
		/// <param name="name">The block name.</param>
		/// <returns><see cref="NeoAxis.TextBlock"/> if the block has been exists; otherwise, <b>null</b>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public TextBlock FindChild( string name )
		{
			for( int n = 0; n < children.Count; n++ )
			{
				TextBlock child = children[ n ];
				if( child.Name == name )
					return child;
			}
			return null;
		}

		/// <summary>
		/// Creates the child block.
		/// </summary>
		/// <param name="name">The block name.</param>
		/// <param name="data">The block data string.</param>
		/// <returns>The child block.</returns>
		/// <remarks>
		/// Names of blocks can repeat.
		/// </remarks>
		[MethodImpl( (MethodImplOptions)512 )]
		public TextBlock AddChild( string name, string data = "" )
		{
			if( string.IsNullOrEmpty( name ) )
				throw new Exception( "AddChild: \"name\" is null or empty." );

			var child = new TextBlock();
			child.parent = this;
			child.name = name;
			child.data = data;
			children.Add( child );
			return child;
		}

		/// <summary>
		/// Adds an already created child block.
		/// </summary>
		/// <param name="child">The child block.</param>
		/// <returns></returns>
		[MethodImpl( (MethodImplOptions)512 )]
		public void AddChild( TextBlock child )
		{
			if( child.parent != null )
				throw new Exception( "AddChild: Unable to add. The block is already added to another block. child.Parent != null." );
			child.parent = this;
			children.Add( child );
		}

		/// <summary>
		/// Deletes child block.
		/// </summary>
		/// <param name="child">The child block.</param>
		[MethodImpl( (MethodImplOptions)512 )]
		public bool DeleteChild( TextBlock child )
		{
			var result = children.Remove( child );
			child.parent = null;
			//child.name = "";
			//child.data = "";
			//child.children = null;
			//child.attributes = null;
			return result;
		}

		/// <summary>
		/// Returns the attribute value by name.
		/// </summary>
		/// <param name="name">The attribute name.</param>
		/// <param name="defaultValue">Default value. If the attribute does not exist that this value will return.</param>
		/// <returns>The attribute value if the attribute exists; otherwise, default value.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public string GetAttribute( string name, string defaultValue = "" )
		{
			for( int n = 0; n < attributes.Count; n++ )
			{
				var attribute = attributes[ n ];
				if( attribute.Name == name )
					return attribute.Value;
			}
			return defaultValue;
		}

		///// <summary>
		///// Returns the attribute value by name.
		///// </summary>
		///// <param name="name">The attribute name.</param>
		///// <returns>The attribute value if the attribute exists; otherwise, empty string.</returns>
		//public string GetAttribute( string name )
		//{
		//	return GetAttribute( name, "" );
		//}

		/// <summary>
		/// Gets the attributes collection.
		/// </summary>
		public IList<Attribute> Attributes
		{
			get { return attributesAsReadOnly; }
		}

		/// <summary>
		/// Checks existence of attribute.
		/// </summary>
		/// <param name="name">The attribute name.</param>
		/// <returns><b>true</b> if the block exists; otherwise, <b>false</b>.</returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool AttributeExists( string name )
		{
			for( int n = 0; n < attributes.Count; n++ )
			{
				var attribute = attributes[ n ];
				if( attribute.Name == name )
					return true;
			}
			return false;
		}

		/// <summary>
		/// Sets attribute. If such already there is that rewrites him.
		/// </summary>
		/// <param name="name">The attribute name.</param>
		/// <param name="value">The attribute value.</param>
		[MethodImpl( (MethodImplOptions)512 )]
		public void SetAttribute( string name, string value )
		{
			if( string.IsNullOrEmpty( name ) )
				throw new Exception( "AddChild: \"name\" is null or empty." );
			if( value == null )
				throw new Exception( "AddChild: \"value\" is null." );

			for( int n = 0; n < attributes.Count; n++ )
			{
				Attribute attribute = attributes[ n ];
				if( attribute.Name == name )
				{
					attribute.value = value;
					return;
				}
			}
			var a = new Attribute();
			a.name = name;
			a.value = value;
			attributes.Add( a );
		}

		/// <summary>
		/// Deletes attribute if he exists.
		/// </summary>
		/// <param name="name">The attribute name.</param>
		[MethodImpl( (MethodImplOptions)512 )]
		public bool DeleteAttribute( string name )
		{
			for( int n = 0; n < attributes.Count; n++ )
			{
				if( name == attributes[ n ].name )
				{
					//Attribute attribute = attributes[ n ];
					//attribute.name = "";
					//attribute.value = "";
					attributes.RemoveAt( n );
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Deletes all attributes.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void DeleteAllAttributes()
		{
			attributes.Clear();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static string TabLevelToString( int level )
		{
			string str = "";
			for( int n = 0; n < level; n++ )
				str += "\t";
			return str;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static bool IsNeedQuotesForLexeme( string text, bool thisIsAttributeValue )
		{
			if( !thisIsAttributeValue )
			{
				foreach( char c in text )
				{
					bool good = ( c >= 'A' && c <= 'Z' ) || ( c >= 'a' && c <= 'z' ) ||
						( c >= '0' && c <= '9' ) || c == '_' || c == '.' || c == '#' || c == '$';
					if( !good )
						return true;
				}
				return false;
			}
			else
			{
				//add quotes for long string for faster parsing.
				if( text.Length > 1000 )
					return true;

				if( text.Length > 0 )
				{
					if( text[ 0 ] == ' ' || text[ text.Length - 1 ] == ' ' )
						return true;
				}

				foreach( char c in text )
				{
					bool good = ( c >= 'A' && c <= 'Z' ) || ( c >= 'a' && c <= 'z' ) ||
						( c >= '0' && c <= '9' ) || c == '_' || c == '#' || c == '$' || c == '.' ||
						c == ',' || c == '-' || c == '!' || c == '%' || c == '&' || c == '(' ||
						c == ')' || c == '*' || c == '+' || c == '?' || c == '[' || c == ']' ||
						c == '^' || c == '|' || c == '~' || c == ' ';

					if( !good )
						return true;
				}

				return false;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void DumpToString( StringBuilder builder, bool userFriendly, int tabLevel )
		{
			string tabPrefix = userFriendly ? TabLevelToString( tabLevel ) : null;

			if( !string.IsNullOrEmpty( Name ) )
			{
				{
					if( userFriendly )
						builder.Append( tabPrefix );

					if( IsNeedQuotesForLexeme( Name, false ) )
					{
						builder.Append( '\"' );
						StringUtility.EncodeDelimiterFormatString( builder, Name );
						builder.Append( '\"' );
					}
					else
						builder.Append( Name );
					//string v;
					//if( IsNeedQuotesForLexeme( Name, false ) )
					//	v = string.Format( "\"{0}\"", StringUtility.EncodeDelimiterFormatString( Name ) );
					//else
					//	v = Name;
					//builder.Append( v );
				}

				if( !string.IsNullOrEmpty( Data ) )
				{
					builder.Append( " " );

					if( IsNeedQuotesForLexeme( Data, false ) )
					{
						builder.Append( '\"' );
						StringUtility.EncodeDelimiterFormatString( builder, Data );
						builder.Append( '\"' );
					}
					else
						builder.Append( Data );
					//string v;
					//if( IsNeedQuotesForLexeme( Data, false ) )
					//	v = string.Format( "\"{0}\"", StringUtility.EncodeDelimiterFormatString( Data ) );
					//else
					//	v = Data;
					//builder.Append( v );
				}

				builder.Append( "\r\n" );
				if( userFriendly )
					builder.Append( tabPrefix );
				builder.Append( "{\r\n" );
			}

			for( int nAttribute = 0; nAttribute < attributes.Count; nAttribute++ ) //foreach( Attribute attribute in attributes )
			{
				var attribute = attributes[ nAttribute ];

				if( userFriendly )
				{
					builder.Append( tabPrefix );
					builder.Append( tabLevel != -1 ? "\t" : "" );
				}

				if( IsNeedQuotesForLexeme( attribute.Name, false ) )
				{
					builder.Append( '\"' );
					StringUtility.EncodeDelimiterFormatString( builder, attribute.Name );
					builder.Append( '\"' );
				}
				else
					builder.Append( attribute.Name );

				builder.Append( " = " );

				if( IsNeedQuotesForLexeme( attribute.Value, true ) )
				{
					builder.Append( '\"' );
					StringUtility.EncodeDelimiterFormatString( builder, attribute.Value );
					builder.Append( '\"' );
				}
				else
					builder.Append( attribute.Value );

				builder.Append( "\r\n" );


				//string name;
				//string value;

				//if( IsNeedQuotesForLexeme( attribute.Name, false ) )
				//	name = string.Format( "\"{0}\"", StringUtility.EncodeDelimiterFormatString( attribute.Name ) );
				//else
				//	name = attribute.Name;

				//if( IsNeedQuotesForLexeme( attribute.Value, true ) )
				//	value = string.Format( "\"{0}\"", StringUtility.EncodeDelimiterFormatString( attribute.Value ) );
				//else
				//	value = attribute.Value;

				//if( userFriendly )
				//{
				//	builder.Append( tabPrefix );
				//	builder.Append( tabLevel != -1 ? "\t" : "" );
				//}
				//builder.AppendFormat( "{0} = {1}\r\n", name, value );
			}

			for( int nChild = 0; nChild < children.Count; nChild++ )
				children[ nChild ].DumpToString( builder, userFriendly, tabLevel + 1 );
			//foreach( TextBlock child in children )
			//	child.DumpToString( builder, userFriendly, tabLevel + 1 );

			if( !string.IsNullOrEmpty( Name ) )
			{
				if( userFriendly )
					builder.Append( tabPrefix );
				builder.Append( "}\r\n" );
			}
		}

		/// <summary>
		/// Returns a string containing all data about the block and his children.
		/// </summary>
		/// <returns>A string containing all data about the block and his children.</returns>
		/// <remarks>
		/// This method is applied at preservation of data of the block in a file.
		/// </remarks>
		/// <example>Example of preservation of data of the block in a file.
		/// <code>
		/// TextBlock block = ...
		/// StreamWriter writer = new StreamWriter( fileName );
		/// writer.Write( block.DumpToString() );
		/// writer.Close();
		/// </code>
		/// </example>
		/// <seealso cref="NeoAxis.TextBlock.Parse(string,out string)"/>
		[MethodImpl( (MethodImplOptions)512 )]
		public string DumpToString( bool userFriendly = true )
		{
			var builder = new StringBuilder( 64 );
			DumpToString( builder, userFriendly, -1 );
			return builder.ToString();
		}

		/// <summary>
		/// Returns a string that represents the current text block.
		/// </summary>
		/// <returns>A string that represents the current text block.</returns>
		public override string ToString()
		{
			string text = string.Format( "Name: \"{0}\"", name );
			if( !string.IsNullOrEmpty( data ) )
				text += string.Format( ", Data: \"{0}\"", data );
			return text;
		}

		/// <summary>
		/// Parses the text with data of the block and his children.
		/// </summary>
		/// <param name="str">The data string.</param>
		/// <param name="error">The information on an error.</param>
		/// <returns><see cref="NeoAxis.TextBlock"/> if the block has been parsed; otherwise, <b>null</b>.</returns>
		/// <seealso cref="NeoAxis.TextBlock.DumpToString()"/>
		/// <remarks>
		/// For convenience of loading of blocks there is auxiliary class <see cref="NeoAxis.TextBlockUtility"/>.
		/// </remarks>
		/// <example>Example of loading of data of the block from a stream.
		/// <code>
		/// FileStream stream = ...;
		/// StreamReader streamReader = new StreamReader( stream );
		/// string error;
		/// TextBlock block = TextBlock.Parse( streamReader.ReadToEnd(), out error );
		/// streamReader.Dispose();
		/// </code>
		/// </example>
		[MethodImpl( (MethodImplOptions)512 )]
		public static TextBlock Parse( string str, out string error )
		{
			var parser = new TextBlockParser();
			return parser.Parse( str, out error );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	class TextBlockParser
	{
		string streamString;
		int streamStringLength;
		int streamPosition;
		string error;
		int linePosition;
		TextBlock root;

		StringBuilder lexStringBuilder;
		bool lexStringBuilderInUse;

		bool StreamEOF
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return streamPosition >= streamStringLength; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		bool StreamReadChar( out char character )
		{
			if( StreamEOF )
			{
				character = (char)0;
				return false;
			}
			character = streamString[ streamPosition ];
			streamPosition++;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void StreamSeek( int position )
		{
			streamPosition = position;
		}

		void Error( string str )
		{
			if( error == null )
				error = string.Format( "{0} (line - {1})", str, linePosition );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		string GetLexeme( bool stopOnlyAtSeparatorOrQuotes, out bool intoQuotes )
		{
			intoQuotes = false;

			if( lexStringBuilderInUse )
				throw new Exception( "GetLexeme: lexStringBuilderInUse == True." );
			var lex = lexStringBuilder;
			lexStringBuilderInUse = true;
			//StringBuilder lex = new StringBuilder( 32 );
			lex.Length = 0;

			try
			{

				while( true )
				{
					char c;
					if( !StreamReadChar( out c ) )
					{
						if( StreamEOF )
							return lex.ToString().Trim();
						Error( "Unexpected end of file" );
						return "";
					}

					//comments
					if( c == '/' )
					{
						char cc;
						if( !StreamReadChar( out cc ) )
						{
							Error( "Unexpected end of file" );
							return "";
						}

						if( cc == '/' )
						{
							while( true )
							{
								if( !StreamReadChar( out c ) )
								{
									if( StreamEOF )
									{
										c = '\n';
										break;
									}
									Error( "Unexpected end of file" );
									return "";
								}
								if( c == '\n' )
									break;
							}
						}
						else if( cc == '*' )
						{
							char oldChar = (char)0;

							while( true )
							{
								if( !StreamReadChar( out c ) )
								{
									if( StreamEOF )
									{
										c = ';';
										break;
									}
									Error( "Unexpected end of file" );
									return "";
								}

								if( c == '\n' )
									linePosition++;

								if( oldChar == '*' && c == '/' )
								{
									c = ';';
									break;
								}

								oldChar = c;
							}
						}
						else
						{
							StreamSeek( streamPosition - 1 );
						}
					}

					if( c == '\n' )
						linePosition++;
					else if( c == '=' || c == '{' || c == '}' )
					{
						if( lex.Length != 0 )
						{
							StreamSeek( streamPosition - 1 );
							return lex.ToString().Trim();
						}
						return c.ToString();
					}

					if( ( !stopOnlyAtSeparatorOrQuotes && ( c <= 32 || c == ';' ) ) ||
						( stopOnlyAtSeparatorOrQuotes && ( c == '\n' || c == '\r' || c == ';' ) ) )
					{
						if( lex.Length != 0 || stopOnlyAtSeparatorOrQuotes )
							return lex.ToString().Trim();
						continue;
					}

					if( c == '"' )
					{
						if( lex.Length != 0 )
						{
							StreamSeek( streamPosition - 1 );
							return lex.ToString().Trim();
						}

						//quotes
						while( true )
						{
							if( !StreamReadChar( out c ) )
							{
								Error( "Unexpected end of file" );
								return "";
							}
							if( c == '\n' )
								linePosition++;
							else if( c == '\\' )
							{
								char c2;
								if( !StreamReadChar( out c2 ) )
								{
									Error( "Unexpected end of file" );
									return "";
								}

								string ss = "\\" + c2;
								if( c2 == 'x' )
								{
									for( int z = 0; z < 4; z++ )
									{
										if( !StreamReadChar( out c2 ) )
										{
											Error( "Unexpected end of file" );
											return "";
										}
										ss += c2;
									}
								}
								StringUtility.DecodeDelimiterFormatString( lex, ss );
								//lex.Append( StringUtils.DecodeDelimiterFormatString( ss ) );
								continue;
							}
							else if( c == '"' )
							{
								intoQuotes = true;
								return lex.ToString();
							}
							lex.Append( c );
						}

					}

					if( lex.Length == 0 && ( c == ' ' || c == '\t' ) )
						continue;

					lex.Append( c );
				}
			}
			finally
			{
				lexStringBuilderInUse = false;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		string GetLexeme( bool stopOnlyAtSeparatorOrQuotes )
		{
			bool intoQuotes;
			return GetLexeme( stopOnlyAtSeparatorOrQuotes, out intoQuotes );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		bool LoadChild( TextBlock child, bool ifEmptyLexReturnTrue )
		{
			while( true )
			{
				bool lexIntoQuotes;
				string lex = GetLexeme( false, out lexIntoQuotes );
				if( lex.Length == 0 )//if( lex == "" )
				{
					if( ifEmptyLexReturnTrue )
						return true;

					Error( "Unexpected end of file" );
					return false;
				}

				if( lex == "}" )
					return true;

				string lex2 = GetLexeme( false );
				if( lex2.Length == 0 )//if( lex2 == "" )
				{
					Error( "Unexpected end of file" );
					return false;
				}

				if( lex2 == "=" )
				{
					string value = GetLexeme( true );
					child.SetAttribute( lex, value );
					continue;
				}

				if( lex2 == "{" )
				{
					TextBlock c = child.AddChild( lex );
					if( !LoadChild( c, false ) )
						return false;
					continue;
				}

				string lex3 = GetLexeme( false );
				if( lex3.Length == 0 )//if( lex3 == "" )
				{
					Error( "Unexpected end of file" );
					return false;
				}

				if( lex3 == "{" )
				{
					TextBlock c = child.AddChild( lex, lex2 );
					if( !LoadChild( c, false ) )
						return false;
					continue;
				}

				Error( "Invalid file format" );
				return false;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public TextBlock Parse( string str, out string errorString )
		{
			try
			{
				if( str == null )
				{
					errorString = "TextBlock: Parse: \"str\" is null.";
					return null;
				}

				streamString = str;
				streamStringLength = streamString.Length;
				streamPosition = 0;
				error = null;
				linePosition = 1;
				root = new TextBlock();
				lexStringBuilder = new StringBuilder( 128 );
				//lexStringBuilder = new StringBuilder( Math.Max( str.Length, 4 ) );

				bool ret = LoadChild( root, true );
				if( !ret )
				{
					errorString = error;
					return null;
				}
				errorString = "";
				return root;

			}
			catch( Exception e )
			{
				errorString = e.Message;
				return null;
			}
		}
	}
}
