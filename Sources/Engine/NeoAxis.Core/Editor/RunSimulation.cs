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
using System.IO;

namespace NeoAxis.Editor
{
	//!!!!так? может не надо отдельный класс

	public static class RunSimulation
	{
		static List<Metadata.TypeInfo> supportedTypesByPlayer = new List<Metadata.TypeInfo>();
		//static List<string> supportedFileTypesByPlayer = new List<string>();

		/////////////////////////////////////////

		public enum RunMethod
		{
			DocumentWindowEditorProcess,
			//!!!!т.е. можно в одном процессе с редактором, а можно отдельно. в одном удобно, если быстро тестить
			//DocumentWindowSeparateProcess,

			Player,//Standalone NeoAxis.Standalone.exe

			//!!!!
			//Game, Project
		}

		/////////////////////////////////////////

		static RunSimulation()
		{
			supportedTypesByPlayer.Add( MetadataManager.GetTypeOfNetType( typeof( Component_Scene ) ) );
			supportedTypesByPlayer.Add( MetadataManager.GetTypeOfNetType( typeof( UIControl ) ) );
			//supportedFileTypesByPlayer.Add( "scene" );
			//supportedFileTypesByPlayer.Add( "ui" );
		}

		public static List<Metadata.TypeInfo> SupportedTypesByPlayer
		{
			get { return supportedTypesByPlayer; }
		}

		public static bool CheckTypeSupportedByPlayer( Metadata.TypeInfo type )
		{
			foreach( var t in supportedTypesByPlayer )
				if( t.IsAssignableFrom( type ) )
					return true;
			return false;
		}

		public static void Run( string realFileName, RunMethod runMethod )
		{
			string fileName = Path.Combine( VirtualFileSystem.Directories.Binaries, "NeoAxis.Player.exe" );

			string arguments = "";

			if( ProjectSettings.Get.RunSimulationInFullscreen.Value )
				arguments += "-fullscreen 1";
			else
				arguments += "-windowed 1";

			if( !string.IsNullOrEmpty( realFileName ) )
				arguments += string.Format( " -play \"{0}\"", realFileName );

			var runMapProcess = Process.Start( fileName, arguments );
		}
	}
}
