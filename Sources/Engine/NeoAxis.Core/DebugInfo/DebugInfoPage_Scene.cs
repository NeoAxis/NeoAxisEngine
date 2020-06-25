//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;
//using System.Globalization;

//namespace NeoAxis
//{
//	/// <summary>
//	/// Represents a page with information about current scene for Debug Window of the editor.
//	/// </summary>
//	public class DebugInfoPage_Scene : DebugInfoPage
//	{
//		public override string Title
//		{
//			get { return "Scene"; }
//		}

//		public override List<string> Content
//		{
//			get
//			{
//				var lines = new List<string>();

//				NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;
//				nfi.NumberGroupSeparator = " ";

//				var document = EditorAPI.SelectedDocument;
//				if( document != null )
//				{
//					var scene = document.ResultObject as Component_Scene;
//					if( scene != null )
//					{
//						foreach( var groupOfObjects in scene.GetComponents<Component_GroupOfObjects>() )
//						{
//							lines.Add( groupOfObjects.ToString() );

//							foreach( var element in groupOfObjects.GetComponents<Component_GroupOfObjectsElement>() )
//							{
//								lines.Add( "  Element: " + element.ToString() );

//								//!!!!

//							}


//						}
//					}
//				}

//				return lines;
//			}
//		}
//	}
//}
