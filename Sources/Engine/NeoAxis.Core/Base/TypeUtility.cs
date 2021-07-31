// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Helper class for working with engine types.
	/// </summary>
	public static class TypeUtility
	{
		static List<string> prefixesToRemove = new List<string>();
		static Dictionary<string, string> wordsToReplace = new Dictionary<string, string>();

		//

		public static List<string> PrefixesToRemove
		{
			get { return prefixesToRemove; }
		}

		public static Dictionary<string, string> WordsToReplace
		{
			get { return wordsToReplace; }
		}

		public static string DisplayNameAddSpaces( string str )
		{
			if( str.Contains( " " ) )
				return str;

			//!!!!
			if( str == "iOS" )
				return str;


			var str2 = str.Replace( '_', ' ' ).Trim();

			if( !str2.Any( char.IsLower ) )
				return str2;


			StringBuilder withSpaces = new StringBuilder();

			char prev = ' ';
			for( int n = 0; n < str2.Length; n++ )
			{
				var c = str2[ n ];

				if( n != 0 )
				{
					if( c >= 'A' && c <= 'Z' && !( prev >= 'A' && prev <= 'Z' ) )
						withSpaces.Append( " " );
				}

				withSpaces.Append( c );

				prev = c;
			}

			string[] words = withSpaces.ToString().Split( new char[] { ' ' } );

			StringBuilder result = new StringBuilder();
			for( int n = 0; n < words.Length; n++ )
			{
				var word = words[ n ];

				if( n > 0 && n < words.Length - 1 )
				{
					if( wordsToReplace.TryGetValue( word, out string newWord ) )
						word = newWord;
				}

				if( result.Length != 0 )
					result.Append( ' ' );
				result.Append( word );
			}

			return result.ToString();
		}

		static TypeUtility()
		{
			prefixesToRemove.Add( "NeoAxis." );
			prefixesToRemove.Add( "Component_" );
			prefixesToRemove.Add( "FlowGraphNode_" );
			prefixesToRemove.Add( "RenderingEffect_" );
			prefixesToRemove.Add( "UI" );
			prefixesToRemove.Add( "GroupOfObjectsElement_" );
			prefixesToRemove.Add( "SurfaceElement_" );
			prefixesToRemove.Add( "MeshGeometry_" );
			prefixesToRemove.Add( "MeshModifier_" );

			List<string> toLowerWords = new List<string>();
			foreach( var s in toLowerWords )
				wordsToReplace.Add( s, s.ToLower() );
		}

		public static string GetUserFriendlyNameForInstanceOfType( Type type )
		{
			//get from attribute 
			var ar = type.GetCustomAttributes( typeof( NewObjectDefaultNameAttribute ), true );
			if( ar.Length != 0 )
				return ( (NewObjectDefaultNameAttribute)ar[ 0 ] ).Name;
			//var ar = type.GetCustomAttributes( typeof( Metadata.UserFriendlyNameForInstanceOfTypeAttribute ), true );
			//if( ar.Length != 0 )
			//	return ( (Metadata.UserFriendlyNameForInstanceOfTypeAttribute)ar[ 0 ] ).Name;

			//go to process

			string str = type.Name;

			//remove prefixes
			foreach( var prefix in prefixesToRemove )
			{
				if( str.Length > prefix.Length && str.Substring( 0, prefix.Length ) == prefix )
					str = str.Substring( prefix.Length );
			}

			str = str.Replace( '_', ' ' ).Trim();

			if( !str.Contains( " " ) )
				str = DisplayNameAddSpaces( str );

			return str;
		}

		public static string GetUserFriendlyCategory( Metadata.Member member )
		{
			var ar = member.GetCustomAttributes( typeof( CategoryAttribute ), true );
			if( ar.Length != 0 )
				return ( (CategoryAttribute)ar[ 0 ] ).Category;

			var category = member.Owner.ToString();

			{
				int index = category.LastIndexOf( '_' );
				if( index != -1 )
					category = category.Substring( index + 1 );
			}

			foreach( var prefix in prefixesToRemove )
			{
				if( category.Length > prefix.Length && category.Substring( 0, prefix.Length ) == prefix )
					category = category.Substring( prefix.Length );
			}

			category = DisplayNameAddSpaces( category );

			return category;
		}
	}
}
