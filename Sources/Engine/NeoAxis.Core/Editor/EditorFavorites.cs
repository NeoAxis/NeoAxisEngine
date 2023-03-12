#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Editor
{
	public static class EditorFavorites
	{
		public static bool AllowFavorites = true;

		[EngineConfig( "Editor", "Favorites" )]
		public static string Favorites = "";

		//

		internal static void Init()
		{
			EngineConfig.RegisterClassParameters( typeof( EditorFavorites ) );
		}

		public static void Add( string name )
		{
			if( !string.IsNullOrEmpty( name ) && !Contains( name ) )
			{
				if( !string.IsNullOrEmpty( Favorites ) )
					Favorites += "|";
				Favorites += name;
			}
		}

		public static void Remove( string name )
		{
			var names = Favorites.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries );

			Favorites = "";
			foreach( var name2 in names )
			{
				if( name2 != name )
				{
					if( !string.IsNullOrEmpty( Favorites ) )
						Favorites += "|";
					Favorites += name2;
				}
			}
		}

		public static bool Contains( string name )
		{
			var names = Favorites.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries );

			foreach( var name2 in names )
			{
				if( name2 == name )
					return true;
			}
			return false;
		}
	}
}

#endif