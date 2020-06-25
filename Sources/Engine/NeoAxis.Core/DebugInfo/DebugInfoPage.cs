// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	//!!!!не компоненты?

	/// <summary>
	/// Represents a base class of info page for Debug Window of the editor.
	/// </summary>
	public abstract class DebugInfoPage
	{
		static List<DebugInfoPage> allPages = new List<DebugInfoPage>();

		public static List<DebugInfoPage> AllPages
		{
			get { return allPages; }
		}

		public override string ToString()
		{
			return Title;
		}

		public abstract string Title { get; }
		public abstract List<string> Content { get; }

		//public static string LinesToString( ICollection<string> lines )
		//{
		//	var b = new StringBuilder();
		//	bool first = true;
		//	foreach( var line in lines )
		//	{
		//		if( !first )
		//			b.Append( "\r\n" );
		//		b.Append( line );
		//		first = false;
		//	}
		//	return b.ToString();
		//}

		static DebugInfoPage()
		{
			AllPages.Add( new DebugInfoPage_Memory() );
			AllPages.Add( new DebugInfoPage_RenderResources() );
			AllPages.Add( new DebugInfoPage_RenderStatistics() );
			AllPages.Add( new DebugInfoPage_Sound() );
			//AllPages.Add( new DebugInfoPage_Scene() );

			//ломает звук. странно
			//AllPages.Add( new DebugInfoPage_Assemblies() );
			//AllPages.Add( new DebugInfoPage_DLLs() );
		}

		public static string Translate( string text )
		{
			return EditorLocalization.Translate( "DebugInfoWindow", text );
		}

	}
}
