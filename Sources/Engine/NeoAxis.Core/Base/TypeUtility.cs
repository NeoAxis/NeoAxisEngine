// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A helper class for working with types of the engine.
	/// </summary>
	public static class TypeUtility
	{
		static List<string> prefixesToRemove = new List<string>();
		//static Dictionary<string, string> wordsToReplace = new Dictionary<string, string>();

		//

		public static ReadOnlyCollection<string> PrefixesToRemove
		{
			get { return prefixesToRemove.AsReadOnly(); }
		}

		//public static Dictionary<string, string> WordsToReplace
		//{
		//	get { return wordsToReplace; }
		//}

		public static string DisplayNameAddSpaces( string str )
		{
			return StringUtility.AddSpacesBetweenWords( str );
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

			//List<string> toLowerWords = new List<string>();
			//foreach( var s in toLowerWords )
			//	wordsToReplace.Add( s, s.ToLower() );
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
