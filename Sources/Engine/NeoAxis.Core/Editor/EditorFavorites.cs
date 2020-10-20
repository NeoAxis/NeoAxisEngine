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

		public static void Add( Metadata.TypeInfo type )
		{
			var typeName = type.Name;

			if( !string.IsNullOrEmpty( Favorites ) )
				Favorites += "|";
			Favorites += typeName;
		}

		public static void Remove( Metadata.TypeInfo type )
		{
			var typeNames = Favorites.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries );

			Favorites = "";
			foreach( var typeName2 in typeNames )
			{
				if( typeName2 != type.Name )
				{
					if( !string.IsNullOrEmpty( Favorites ) )
						Favorites += "|";
					Favorites += typeName2;
				}
			}
		}

		public static bool Contains( Metadata.TypeInfo type )
		{
			var typeNames = Favorites.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries );

			foreach( var typeName2 in typeNames )
			{
				if( typeName2 == type.Name )
					return true;
			}
			return false;
		}
	}
}
