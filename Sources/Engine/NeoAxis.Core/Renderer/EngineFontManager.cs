// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Design;

namespace NeoAxis
{
	//!!!!!!threading:
	////!!!!not supported. need rewrite on components before
	//EngineThreading.CheckMainThread();

	static class EngineFontManager
	{
		//string defaultLanguage;

		////Key: fileName.ToLower().Replace( '/', '\\' )
		//Dictionary<string, EngineFontDefinition> fontDefinitions = new Dictionary<string, EngineFontDefinition>();

		////Key: string.Format( "{0} {1}", fileName.ToLower().Replace( '/', '\\' ), height);
		//Dictionary<string, EngineFont> fonts = new Dictionary<string, EngineFont>();

		static IntPtr freeTypeLibrary;

		//

		//public static string FontsDirectory
		//{
		//	get { return fontsDirectory; }
		//	set
		//	{
		//		if( instance != null )
		//			Log.Fatal( "FontManager: FontsDirectory: Unable to change fonts directory after initialization." );
		//		fontsDirectory = value;
		//	}
		//}

		//EngineFontManager() { }

		internal static void Init( string defaultLanguage )
		{
			InitInternal( defaultLanguage );
		}

		internal static void Shutdown()
		{
			ShutdownInternal();
		}

		static void InitInternal( string defaultLanguage )
		{
			//this.defaultLanguage = defaultLanguage;

			//EngineBackgroundTasks.Update += EngineBackgroundTasks_Update;
		}

		static void ShutdownInternal()
		{
			//DisposeFonts();

			ShutdownFreeTypeLibrary();

			//EngineBackgroundTasks.Update -= EngineBackgroundTasks_Update;
		}

		///// <summary>
		///// Gets an instance of the <see cref="EngineFontManager"/>.
		///// </summary>
		//public static EngineFontManager Instance
		//{
		//	get { return instance; }
		//}

		//public string DefaultLanguage
		//{
		//	get { return defaultLanguage; }
		//}

		//public ICollection<EngineFontDefinition> FontDefinitions
		//{
		//	get { return fontDefinitions.Values; }
		//}

		//public ICollection<EngineFont> Fonts
		//{
		//	get { return fonts.Values; }
		//}

		//public EngineFontDefinition LoadFontDefinition( string fileName )
		//{
		//	//!!!!not supported. need rewrite on components before
		//	EngineThreading.CheckMainThread();

		//	string key = fileName.ToLower().Replace( '/', '\\' );

		//	EngineFontDefinition definition;
		//	if( !fontDefinitions.TryGetValue( key, out definition ) )
		//	{
		//		string errorString;
		//		TextBlock block = TextBlockUtility.LoadFromVirtualFile( fileName, out errorString );
		//		if( block == null )
		//		{
		//			Log.Warning( errorString );
		//			return null;
		//		}

		//		TextBlock definitionBlock = block.FindChild( "fontDefinition" );
		//		if( definitionBlock == null )
		//		{
		//			Log.Warning( "FontManager: Invalid font definition file \"{0}\" " +
		//				"(No \"fontDefinition\" block).", fileName );
		//			return null;
		//		}

		//		string typeString = definitionBlock.GetAttribute( "type" );
		//		if( typeString == "" )
		//			typeString = "PrecompiledImage";

		//		if( typeString == "PrecompiledImage" )
		//		{
		//			definition = new PrecompiledImageFontDefinition( fileName );
		//		}
		//		else if( typeString == "TrueType" )
		//		{
		//			definition = new TrueTypeFontDefinition( fileName );
		//		}
		//		else
		//		{
		//			Log.Warning( "FontManager: Invalid font definition file \"{0}\" " +
		//				"(Unknown font type \"{1}\").", fileName, typeString );
		//			return null;
		//		}

		//		if( !definition.OnLoad( definitionBlock, out errorString ) )
		//		{
		//			Log.Warning( "FontManager: Invalid font definition file \"{0}\" ({1}).",
		//				fileName, errorString );
		//			return null;
		//		}
		//		fontDefinitions.Add( key, definition );
		//	}

		//	return definition;
		//}

		//public EngineFontDefinition GetFontDefinitionByFileName( string fileName )
		//{
		//	string key = fileName.ToLower().Replace( '/', '\\' );

		//	EngineFontDefinition definition;
		//	if( !fontDefinitions.TryGetValue( key, out definition ) )
		//		return null;
		//	return definition;
		//}

		//public bool ReloadFontDefinition( EngineFontDefinition definition )
		//{
		//	string errorString;
		//	TextBlock block = TextBlockUtility.LoadFromVirtualFile( definition.FileName,
		//		out errorString );
		//	if( block == null )
		//	{
		//		Log.Warning( errorString );
		//		return false;
		//	}

		//	TextBlock definitionBlock = block.FindChild( "fontDefinition" );
		//	if( definitionBlock == null )
		//	{
		//		Log.Warning( "FontManager: Invalid font definition file \"{0}\" " +
		//			"(No \"fontDefinition\" block).", definition.FileName );
		//		return false;
		//	}

		//	if( !definition.OnLoad( definitionBlock, out errorString ) )
		//	{
		//		Log.Warning( "FontManager: Invalid font definition file \"{0}\" ({1}).",
		//			definition.FileName, errorString );
		//		return false;
		//	}

		//	return true;
		//}

		//public bool SaveFontDefinition( EngineFontDefinition definition )
		//{
		//	string realFileName = VirtualPathUtility.GetRealPathByVirtual( definition.FileName );

		//	TextBlock rootBlock = new TextBlock();

		//	TextBlock block = rootBlock.AddChild( "fontDefinition" );
		//	definition.OnSave( block );

		//	try
		//	{
		//		using( StreamWriter writer = new StreamWriter( realFileName ) )
		//		{
		//			writer.Write( rootBlock.DumpToString() );
		//		}
		//	}
		//	catch( Exception ex )
		//	{
		//		Log.Error( "Unable to save file \"{0}\" ({1}).", realFileName, ex.Message );
		//		return false;
		//	}

		//	return true;
		//}

		////public void RemoveFontDefinitionFromCache( string fileName )
		////{
		////   string key = fileName.ToLower().Replace( '/', '\\' );
		////   fontDefinitions.Remove( key );
		////}

		//public EngineFont LoadFontByFileName( string fileName, float height )
		//{
		//	//!!!!not supported. need rewrite on components before
		//	EngineThreading.CheckMainThread();

		//	string key = string.Format( "{0} {1}", fileName.ToLower().Replace( '/', '\\' ), height );

		//	EngineFont font;
		//	if( !fonts.TryGetValue( key, out font ) )
		//	{
		//		EngineFontDefinition definition = LoadFontDefinition( fileName );
		//		if( definition == null )
		//			return null;

		//		string name = "_Invalid";
		//		string language = "English";

		//		string fileBase = Path.GetFileNameWithoutExtension( fileName );
		//		try
		//		{
		//			string[] strings = fileBase.Split( new char[] { '_' },
		//				StringSplitOptions.RemoveEmptyEntries );

		//			//!!!!!странно это всё
		//			if( strings.Length >= 1 )
		//				name = strings[ 0 ];
		//			if( strings.Length >= 2 )
		//				language = strings[ 1 ];
		//		}
		//		catch { }

		//		font = new EngineFont( name, language, height, definition );
		//		fonts.Add( key, font );
		//	}
		//	return font;
		//}

		///// <summary>Loads font or returns already loaded font.</summary>
		///// <param name="name">The font name.</param>
		///// <param name="language">The font language.</param>
		///// <param name="height">The font height.</param>
		///// <returns><see cref="EngineFont"/> if the font has been loaded; otherwise, <b>null</b>.</returns>
		//public EngineFont LoadFont( string name, string language, float height )
		//{
		//	string fileName = string.Format( "{0}{1}{2}_{3}.fontDefinition",
		//		FontsDirectory, Path.DirectorySeparatorChar, name, language );
		//	//string fileName = string.Format( "{0}{1}{2}_{3}.fontDefinition",
		//	//	FontsDirectory, Path.DirectorySeparatorChar, name, language );
		//	return LoadFontByFileName( fileName, height );
		//}

		///// <summary>Loads font or returns already loaded font.</summary>
		///// <param name="name">The font name.</param>
		///// <param name="height">The font height.</param>
		///// <returns><see cref="EngineFont"/> if the font has been loaded; otherwise, <b>null</b>.</returns>
		//public EngineFont LoadFont( string name, float height )
		//{
		//	return LoadFont( name, DefaultLanguage, height );
		//}

		//internal void RemoveFont( EngineFont font )
		//{
		//	foreach( KeyValuePair<string, EngineFont> pair in fonts )
		//	{
		//		if( pair.Value == font )
		//		{
		//			fonts.Remove( pair.Key );
		//			break;
		//		}
		//	}
		//}

		////need to call somewhere?
		///// <summary>Dispose all loaded fonts.</summary>
		//public void DisposeFonts()
		//{
		//	while( fonts.Count != 0 )
		//	{
		//		Dictionary<string, EngineFont>.Enumerator enumerator = fonts.GetEnumerator();
		//		enumerator.MoveNext();
		//		enumerator.Current.Value.Dispose();
		//	}
		//}

		static internal IntPtr GetOrInitFreeTypeLibrary()
		{
			if( freeTypeLibrary == IntPtr.Zero )
			{
				freeTypeLibrary = FreeType.Init();
				if( freeTypeLibrary == IntPtr.Zero )
					Log.Fatal( "FontManager: Cannot initialize FreeType library." );
			}

			return freeTypeLibrary;
		}

		static void ShutdownFreeTypeLibrary()
		{
			if( freeTypeLibrary != IntPtr.Zero )
			{
				FreeType.Shutdown( freeTypeLibrary );
				freeTypeLibrary = IntPtr.Zero;
			}
		}

		//!!!!
		//void CheckAndRemoveNotNeededVariants()
		//{
		//	foreach( EngineFont font in fonts.Values )
		//		font.CheckAndRemoveNotNeededVariants();
		//}

		////!!!!
		//static private void EngineBackgroundTasks_Update()
		//{
		//	//CheckAndRemoveNotNeededVariants();
		//}
	}
}
