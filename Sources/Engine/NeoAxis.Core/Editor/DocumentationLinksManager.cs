#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public static class DocumentationLinksManager
	{
		static Dictionary<Type, string> nameByType = new Dictionary<Type, string>();

		//

		public static void AddNameByType( Type type, string name )
		{
			nameByType[ type ] = name;
		}

		public static string GetNameForType( Type type )
		{
			if( nameByType.TryGetValue( type, out var name ) )
				return name;
			return "";
		}

		public static string GetFullLinkForType( Type type )
		{
			var name = GetNameForType( type );
			if( !string.IsNullOrEmpty( name ) )
			{
				var baseLink = name.Replace( ' ', '_' );
				return $"https://www.neoaxis.com/docs/html/{baseLink}.htm";
			}
			return "";
		}
	}
}

#endif