// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using NeoAxis.Editor;

namespace NeoAxis
{
	public static class EditorLocalization
	{
		static bool initialized;
		static string language;
		static bool writeNotFound;
		static Dictionary<string, string> data = new Dictionary<string, string>();
		static ESet<string> notFound = new ESet<string>();
		static bool wideLanguage;

		//

		internal static void Init( string language, bool writeNotFound )
		{
			if( !string.IsNullOrEmpty( language ) && language != "English" )
			{
				EditorLocalization.language = language;
				EditorLocalization.writeNotFound = writeNotFound;

				//parse basic txt file
				if( ParseFile( language, out var d, out _, out wideLanguage ) )
				{
					foreach( var pair in d )
						data[ pair.Key ] = pair.Value;
				}

				initialized = true;
			}
		}

		internal static void Shutdown()
		{
			if( initialized && writeNotFound )
			{
				var fileBase = Path.Combine( VirtualFileSystem.Directories.EngineInternal, "Localization", language );
				var pathNotFoundTxt = fileBase + "_NotFound.txt";

				EDictionary<string, string> newNotFound = null;
				Encoding encoding = null;
				if( File.Exists( pathNotFoundTxt ) )
					ParseFile( language + "_NotFound", out newNotFound, out encoding, out _ );

				if( newNotFound == null )
					newNotFound = new EDictionary<string, string>();

				foreach( var v in notFound )
				{
					if( !newNotFound.ContainsKey( v ) )
						newNotFound[ v ] = "";
				}

				try
				{
					if( File.Exists( pathNotFoundTxt ) )
						File.Delete( pathNotFoundTxt );

					var lines = new List<string>();
					foreach( var pair in newNotFound )
						lines.Add( pair.Key + "|" + pair.Value );

					lines.Sort();

					if( encoding != null )
						File.WriteAllLines( pathNotFoundTxt, lines, encoding );
					else
						File.WriteAllLines( pathNotFoundTxt, lines );
				}
				catch( Exception e )
				{
					Log.Warning( "EditorLocalization: Shutdown: " + e.Message );
				}
			}
		}

		public static bool Initialized
		{
			get { return initialized; }
		}

		public static string Language
		{
			get { return language; }
		}

		public static bool WriteNotFound
		{
			get { return writeNotFound; }
		}

		public static string Translate( string group, string text )
		{
			if( Initialized && text != null )
			{
				var text2 = text.Trim();
				if( text2 != "" )
				{
					var key = group + "|" + text2.Replace( "\n", "\\n" );

					if( data.TryGetValue( key, out var value ) )
						return value.Replace( "\\n", "\n" );

					if( writeNotFound )
						notFound.AddWithCheckAlreadyContained( key );
				}
			}

			return text;
		}

		public static bool ParseFile( string fileName, out EDictionary<string, string> resultData, out Encoding encoding, out bool wide )
		{
			resultData = null;
			encoding = null;
			wide = false;

			var fileBase = Path.Combine( VirtualFileSystem.Directories.EngineInternal, "Localization", fileName );
			var pathInfo = fileBase + ".info";
			var pathTxt = fileBase + ".txt";

			if( File.Exists( pathTxt ) )
			{
				try
				{
					string encodingName = null;
					int? encodingCodepage = null;

					if( File.Exists( pathInfo ) )
					{
						var block = TextBlockUtility.LoadFromRealFile( pathInfo );
						if( block != null )
						{
							//Encoding
							{
								var value = block.GetAttribute( "Encoding" );
								if( int.TryParse( value, out var codepage ) )
									encodingCodepage = codepage;
								else
									encodingName = value;
							}

							//WideLanguage
							{
								var value = block.GetAttribute( "WideLanguage" );
								if( !string.IsNullOrEmpty( value ) )
									wide = (bool)SimpleTypes.ParseValue( typeof( bool ), value );
							}
						}
					}

#if !DEPLOY
					if( encodingCodepage.HasValue )
						encoding = CodePagesEncodingProvider.Instance.GetEncoding( encodingCodepage.Value );
					else if( !string.IsNullOrEmpty( encodingName ) )
						encoding = CodePagesEncodingProvider.Instance.GetEncoding( encodingName );
#endif
					//if( encodingCodepage.HasValue )
					//	encoding = Encoding.GetEncoding( encodingCodepage.Value );
					//else if( !string.IsNullOrEmpty( encodingName ) )
					//	encoding = Encoding.GetEncoding( encodingName );

					string[] lines = null;
					if( encoding != null )
						lines = File.ReadAllLines( pathTxt, encoding );
					else
						lines = File.ReadAllLines( pathTxt );

					resultData = new EDictionary<string, string>();

					foreach( var line in lines )
					{
						if( !string.IsNullOrEmpty( line ) )
						{
							var strs = line.Split( new char[] { '|' } );
							if( strs.Length != 3 )
								throw new Exception( string.Format( "Invalid format for line \'{0}\'.", line ) );
							resultData[ strs[ 0 ] + "|" + strs[ 1 ] ] = strs[ 2 ];
						}
					}

					return true;
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
				}
			}

			return false;
		}

		public static void TranslateForm( string group, Control control )
		{
			if( Initialized )
			{
				foreach( var child in control.Controls )
				{
					//KryptonLabel
					{
						var child2 = child as KryptonLabel;
						if( child2 != null && !string.IsNullOrEmpty( child2.Text ) )
						{
							var text = child2.Text;

							if( text[ text.Length - 1 ] == ':' )
							{
								var t = text.Substring( 0, text.Length - 1 );
								child2.Text = Translate( group, t ) + ":";
							}
							else
								child2.Text = Translate( group, text );
						}
					}

					//Label
					{
						var child2 = child as Label;
						if( child2 != null && !string.IsNullOrEmpty( child2.Text ) )
						{
							var text = child2.Text;

							if( text.Length > 0 && text[ text.Length - 1 ] == ':' )
							{
								var t = text.Substring( 0, text.Length - 1 );
								child2.Text = Translate( group, t ) + ":";
							}
							else
								child2.Text = Translate( group, text );
						}
					}

					//LabelEx
					{
						var child2 = child as EngineLabel;
						if( child2 != null && !string.IsNullOrEmpty( child2.Text ) )
						{
							var text = child2.Text;

							if( text.Length > 0 && text[ text.Length - 1 ] == ':' )
							{
								var t = text.Substring( 0, text.Length - 1 );
								child2.Text = Translate( group, t ) + ":";
							}
							else
								child2.Text = Translate( group, text );
						}
					}

					//KryptonButton
					{
						var child2 = child as KryptonButton;
						if( child2 != null && !string.IsNullOrEmpty( child2.Text ) )
							child2.Text = Translate( group, child2.Text );
					}

					//KryptonCheckBox
					{
						var child2 = child as KryptonCheckBox;
						if( child2 != null && !string.IsNullOrEmpty( child2.Text ) )
							child2.Text = Translate( group, child2.Text );
					}
				}
			}
		}

		public static string TranslateLabel( string group, string text )
		{
			if( Initialized && text != null )
			{
				if( text.Length > 0 && text[ text.Length - 1 ] == ':' )
				{
					var t = text.Substring( 0, text.Length - 1 );
					return Translate( group, t ) + ":";
				}
				else
					return Translate( group, text );
			}
			return text;
		}

		public static bool WideLanguage
		{
			get { return Initialized && wideLanguage; }
		}
	}
}
