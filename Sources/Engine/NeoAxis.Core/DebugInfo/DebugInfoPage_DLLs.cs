//// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.ComponentModel;
//using System.Collections.Generic;
//using System.Text;
//using System.IO;

//namespace NeoAxis
//{
//	public class DebugInfoPage_DLLs : DebugInfoPage
//	{
//		public override string Title
//		{
//			get { return "DLLs"; }
//		}

//		public override string Content
//		{
//			get
//			{
//				var lines = new List<string>();

//				string[] names = EngineApp.GetNativeModuleNames();
//				ArrayUtils.SelectionSort( names, delegate ( string s1, string s2 )
//				{
//					return string.Compare( s1, s2, true );
//				} );

//				foreach( string name in names )
//					lines.Add( string.Format( "{0} - {1}", Path.GetFileName( name ), name ) );

//				return LinesToString( lines );
//			}
//		}
//	}
//}
