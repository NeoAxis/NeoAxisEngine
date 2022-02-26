// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace NeoAxis
{
	//!!!!!!

	//!!!!!threading

	internal sealed class LanguageManager
	{
		static char[] translateSplitCharacters = new char[] { '\\', '/' };

		//

		static string languagesDirectory = "Languages";
		static LanguageManager instance;

		public delegate void AlternativeLanguageFileLoaderDelegate( ref TextBlock generatedBlock,
			ref bool error );
		public static event AlternativeLanguageFileLoaderDelegate AlternativeLanguageFileLoaderHandler;

		string language;
		bool localizeEngine;
		string toolsUICulture = "en-US";
		List<string> initializedFileRedirections = new List<string>();
		TranslationGroup textTranslations = new TranslationGroup( "Root" );

		///////////////////////////////////////////

		public class TranslationGroup
		{
			string name;
			internal Dictionary<string, TranslationGroup> children;
			internal Dictionary<string, string> texts;

			internal TranslationGroup( string name )
			{
				this.name = name;
			}

			public string Name
			{
				get { return name; }
			}

			public ICollection<TranslationGroup> Children
			{
				get
				{
					if( children == null )
						return null;
					return new ReadOnlyICollection<TranslationGroup>( children.Values );
				}
			}

			public ICollection<KeyValuePair<string, string>> Texts
			{
				get
				{
					if( texts == null )
						return null;
					return new ReadOnlyICollection<KeyValuePair<string, string>>( texts );
				}
			}
		}

		///////////////////////////////////////////

		public static string LanguagesDirectory
		{
			get { return languagesDirectory; }
			set
			{
				if( instance != null )
					Log.Fatal( "LanguageManager: LanguagesDirectory: Unable to change languages directory after initialization." );
				languagesDirectory = value;
			}
		}

		LanguageManager() { }

		internal static bool Init( string language, bool localizeEngine )
		{
			Trace.Assert( instance == null );
			instance = new LanguageManager();
			if( !instance.InitInternal( language, localizeEngine ) )
			{
				Shutdown();
				return false;
			}
			return true;
		}

		internal static void Shutdown()
		{
			if( instance != null )
			{
				instance.ShutdownInternal();
				instance = null;
			}
		}

		public static LanguageManager Instance
		{
			get { return instance; }
		}

		void ParseTranslationBlockRecursive( TranslationGroup group, TextBlock groupBlock )
		{
			if( groupBlock.Children.Count != 0 )
				group.children = new Dictionary<string, TranslationGroup>( groupBlock.Children.Count );
			foreach( TextBlock childBlock in groupBlock.Children )
			{
				TranslationGroup childGroup = new TranslationGroup( childBlock.Name );
				group.children.Add( childBlock.Name, childGroup );
				ParseTranslationBlockRecursive( childGroup, childBlock );
			}

			if( groupBlock.Attributes.Count != 0 )
				group.texts = new Dictionary<string, string>( groupBlock.Attributes.Count );
			foreach( TextBlock.Attribute attribute in groupBlock.Attributes )
				group.texts.Add( attribute.Name, attribute.Value );
		}

		void ParseFileRedirectionsBlock( TextBlock block )
		{
			foreach( TextBlock.Attribute attribute in block.Attributes )
			{
				//!!!!!
				//VirtualFileSystem.AddFileRedirection( attribute.Name, attribute.Value );

				initializedFileRedirections.Add( attribute.Name );
			}
		}

		public void ParseLanguageTextBlock( TextBlock rootBlock )
		{
			TextBlock languageBlock = rootBlock.FindChild( "language" );
			if( languageBlock != null )
			{
				if( languageBlock.AttributeExists( "toolsUICulture" ) )
					toolsUICulture = languageBlock.GetAttribute( "toolsUICulture" );

				if( localizeEngine )
				{
					//translationTexts
					TextBlock textTranslationsBlock = languageBlock.FindChild( "textTranslations" );
					if( textTranslationsBlock != null )
						ParseTranslationBlockRecursive( textTranslations, textTranslationsBlock );

					//fileRedirections
					//!!!!!!?
					if( EngineApp.ApplicationType != EngineApp.ApplicationTypeEnum.Editor )
					{
						TextBlock fileRedirectionsBlock = languageBlock.FindChild( "fileRedirections" );
						if( fileRedirectionsBlock != null )
							ParseFileRedirectionsBlock( fileRedirectionsBlock );
					}
				}
			}
		}

		bool InitInternal( string language, bool localizeEngine )
		{
			this.language = language;
			this.localizeEngine = localizeEngine;

			//alternative loader
			TextBlock rootBlock = null;
			bool error2 = false;
			if( AlternativeLanguageFileLoaderHandler != null )
				AlternativeLanguageFileLoaderHandler( ref rootBlock, ref error2 );
			if( error2 )
				return false;

			//default loader
			if( rootBlock == null )
			{
				//!!!!локализация дополнений
				//!!!!!еще можно изменить Engine.language. сделать уровни видать.

				//!!!!!!				
				string fileName = Path.Combine( Path.Combine( LanguagesDirectory, language ), "Engine.language" );
				if( VirtualFile.Exists( fileName ) )
				{
					string error;
					rootBlock = TextBlockUtility.LoadFromVirtualFile( fileName, out error );
					if( rootBlock == null )
					{
						Log.Fatal( "Parsing file failed \"{0}\" ({1}).", fileName, error );
						return false;
					}
				}
			}

			if( rootBlock != null )
				ParseLanguageTextBlock( rootBlock );

			return true;
		}

		void ShutdownInternal()
		{
			//!!!!!
			//foreach( string originalFileName in initializedFileRedirections )
			//	VirtualFileSystem.RemoveFileRedirection( originalFileName );
			initializedFileRedirections.Clear();
		}

		public string Language
		{
			get { return language; }
		}

		public bool LocalizeEngine
		{
			get { return localizeEngine; }
		}

		public string ToolsUICulture
		{
			get { return toolsUICulture; }
		}

		public IList<string> InitializedFileRedirections
		{
			get { return new ReadOnlyCollection<string>( initializedFileRedirections ); }
		}

		public TranslationGroup TextTranslations
		{
			get { return textTranslations; }
		}

		Dictionary<string, string> GetTranslationTextDictionary( string groupsPath )
		{
			if( !string.IsNullOrEmpty( groupsPath ) )
			{
				if( textTranslations.children != null )
				{
					if( groupsPath.IndexOfAny( translateSplitCharacters ) != -1 )
					{
						string[] groupNames = groupsPath.Split( translateSplitCharacters );

						TranslationGroup group = textTranslations;
						foreach( string groupName in groupNames )
						{
							if( group.children == null )
								return null;
							TranslationGroup child;
							if( !group.children.TryGetValue( groupName, out child ) )
								return null;
							group = child;
						}
						return group.texts;
					}
					else
					{
						TranslationGroup group;
						if( textTranslations.children.TryGetValue( groupsPath, out group ) )
							return group.texts;
						else
							return null;
					}
				}
				else
					return null;
			}
			else
			{
				return textTranslations.texts;
			}
		}

		public delegate void OverrideTranslateDelegate( string groupsPath, string text, ref bool processed, ref string result );
		public static event OverrideTranslateDelegate OverrideTranslate;

		public string Translate( string groupsPath, string text )
		{
			if( !localizeEngine )
				return text;

			if( string.IsNullOrEmpty( text ) )
				return text;

			if( OverrideTranslate != null )
			{
				bool processed = false;
				string result = "";
				OverrideTranslate( groupsPath, text, ref processed, ref result );
				if( processed )
					return result;
			}

			Dictionary<string, string> texts = GetTranslationTextDictionary( groupsPath );
			if( texts == null )
				return text;

			string value;
			if( !texts.TryGetValue( text, out value ) )
				return text;
			return value;
		}
	}
}
