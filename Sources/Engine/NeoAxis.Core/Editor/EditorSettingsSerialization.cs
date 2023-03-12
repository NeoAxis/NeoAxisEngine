#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using Internal.ComponentFactory.Krypton.Ribbon;

namespace NeoAxis.Editor
{
	static class EditorSettingsSerialization
	{
		[EngineConfig( "Editor", "QATLocation" )]
		static QATLocation QATLocation = QATLocation.Above;
		[EngineConfig( "Editor", "RibbonMinimizedMode" )]
		static bool RibbonMinimizedMode;
		[EngineConfig( "Editor", "RibbonLastSelectedTabTypeByUser" )]
		static string RibbonLastSelectedTabTypeByUser = "";
		[EngineConfig( "Editor", "ShowTipsAsStartup" )]
		public static bool ShowTipsAsStartup = true;

		[EngineConfig( "Editor", "OpenFileAtStartup" )]
		public static string OpenFileAtStartup = "";

		//

		public static void Init()
		{
			EngineConfig.RegisterClassParameters( typeof( EditorSettingsSerialization ) );
			//EngineApp.Config.LoadEvent += Config_LoadEvent;
			//EngineApp.Config.SaveEvent += Config_SaveEvent;

			EditorForm.Instance.kryptonRibbon.QATLocation = QATLocation;
			EditorForm.Instance.kryptonRibbon.MinimizedMode = RibbonMinimizedMode;
			EditorForm.Instance.ribbonLastSelectedTabTypeByUser = RibbonLastSelectedTabTypeByUser;
		}

		public static void InitAfterFormLoad()
		{
		}

		public static void Dump()
		{
			if( !EditorForm.Instance.backstageMenu1.Visible )
				QATLocation = EditorForm.Instance.kryptonRibbon.QATLocation;
			RibbonMinimizedMode = EditorForm.Instance.kryptonRibbon.MinimizedMode;
			RibbonLastSelectedTabTypeByUser = EditorForm.Instance.ribbonLastSelectedTabTypeByUser;
		}

		//private static void Config_LoadEvent( EngineConfig config, TextBlock block, ref string error )
		//{
		//}

		//private static void Config_SaveEvent( EngineConfig config, TextBlock block )
		//{
		//}
	}
}

#endif