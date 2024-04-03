//#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeoAxis.Editor
{
	public static class EditorLocalization
	{
		public static string Translate( string group, string text )
		{
			if( EditorAssemblyInterface.Instance != null )
				return EditorAssemblyInterface.Instance.Translate( group, text );
			return text;
		}
	}
}
//#endif