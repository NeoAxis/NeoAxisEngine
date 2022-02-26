// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using NeoAxis;

namespace Internal//NeoAxis
{
	/// <summary>
	/// Represents a base class of info page for Debug Window of the editor.
	/// </summary>
	public abstract class DebugInfo
	{
		static List<DebugInfo> allPages = new List<DebugInfo>();

		public static List<DebugInfo> AllPages
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

		static DebugInfo()
		{
			AllPages.Add( new DebugInfo_Memory() );
			AllPages.Add( new DebugInfo_RenderResources() );
			AllPages.Add( new DebugInfo_RenderStatistics() );
			AllPages.Add( new DebugInfo_Sound() );

			//AllPages.Add( new DebugInfoPage_Scene() );
			//AllPages.Add( new DebugInfoPage_Assemblies() );
			//AllPages.Add( new DebugInfoPage_DLLs() );
		}

		public static string Translate( string text )
		{
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
				return NeoAxis.Editor.EditorLocalization.Translate( "DebugInfoWindow", text );
			else
				return text;
		}

	}
}
