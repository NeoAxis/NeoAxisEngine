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
using System.IO;

namespace NeoAxis.Editor
{
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
			supportedTypesByPlayer.Add( MetadataManager.GetTypeOfNetType( typeof( Scene ) ) );
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

			if( ProjectSettings.Get.General.RunPlayerFromEditorInFullscreen.Value )
				arguments += "-fullscreen 1";
			else
				arguments += "-windowed 1";

			if( !string.IsNullOrEmpty( realFileName ) )
				arguments += string.Format( " -play \"{0}\"", realFileName );

			Process.Start( new ProcessStartInfo( fileName, arguments ) { UseShellExecute = true } );
			//Process.Start( fileName, arguments );
		}

		public static void RunRenderVideoToFile( string realFileName, string destRealFileName, RunMethod runMethod, int framesPerSecond, double length, string camera, string renderingPipeline, Vector2I resolution, string format )
		{
			string fileName = Path.Combine( VirtualFileSystem.Directories.Binaries, "NeoAxis.Player.exe" );

			string arguments = "";

			//!!!!
			//if( ProjectSettings.Get.RunSimulationInFullscreen.Value )
			arguments += "-fullscreen 1";
			//else
			//	arguments += "-windowed 1";

			if( !string.IsNullOrEmpty( realFileName ) )
				arguments += string.Format( " -play \"{0}\"", realFileName );

			arguments += string.Format( " -renderVideoToFile \"{0}\"", destRealFileName );
			arguments += string.Format( " -framesPerSecond {0}", framesPerSecond );
			arguments += string.Format( " -length {0}", length );
			if( !string.IsNullOrEmpty( camera ) )
				arguments += string.Format( " -camera \"{0}\"", camera );
			if( !string.IsNullOrEmpty( renderingPipeline ) )
				arguments += string.Format( " -renderingPipeline \"{0}\"", renderingPipeline );
			arguments += string.Format( " -resolution \"{0}\"", resolution.ToString() );
			arguments += string.Format( " -format \"{0}\"", format );

			Process.Start( new ProcessStartInfo( fileName, arguments ) { UseShellExecute = true } );
			//Process.Start( fileName, arguments );
		}
	}
}

#endif