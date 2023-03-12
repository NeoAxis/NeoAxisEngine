// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ICSharpCode.SharpZipLib.Zip;
using Java.Nio;
using Javax.Microedition.Khronos.Opengles;
using Internal;

namespace NeoAxis.Player.Android
{
	static class Engine
	{
		public static AppCompatActivity activity;

		//Thread engineMainThread;
		public volatile static bool engineInitialized;

		public struct TouchEventItem
		{
			public MotionEventActions Action;
			public int ActionIndex;
			public MotionEventActions ActionMasked;

			public Vector2F[] PointersPosition;
			public int[] PointersId;

			//it can't work. MotionEvent properties become invalid when OnTouch is ended
			//public View View;
			//public MotionEvent MotionEvent;
		}
		public static Queue<TouchEventItem> touchEventsQueue = new Queue<TouchEventItem>();

		static List<object> pointerIdentifiers = new List<object>();

		/////////////////////////////////////////

		public static void InitEngine()
		{
			Log.Handlers.InvisibleInfoHandler += Log_InvisibleInfoHandler;
			Log.Handlers.InfoHandler += Log_InfoHandler;
			Log.Handlers.WarningHandler += Log_WarningHandler;
			Log.Handlers.ErrorHandler += Log_ErrorHandler;

			EngineApp.AppCreateBefore += EngineApp_AppCreateBefore;

			new PlatformFunctionalityAndroid();
			EngineApp.ApplicationType = EngineApp.ApplicationTypeEnum.Simulation;

			ExtractProjectZip( out var projectDir );

			//get project's directories
			string projectDirectory = "";
			string userSettingsDirectory = "";
			string binariesDirectory = "";
			{
				//!!!!
				projectDirectory = projectDir;
				userSettingsDirectory = Path.Combine( projectDirectory, "User settings" );
				binariesDirectory = Path.Combine( projectDirectory, "Binaries" );
			}

			if( !VirtualFileSystem.Init( "user:Logs/Player.log", true, projectDirectory, userSettingsDirectory, binariesDirectory ) )
				return;

			//configure general settings
			EngineApp.InitSettings.ConfigVirtualFileName = "user:Configs/Player.config";

			//these parameters are enabled by default
			//EngineApp.EnginePauseWhenApplicationIsNotActive = false;
			//EngineApp.InitSettings.AllowJoysticksAndSpecialInputDevices = false;

			//specify Project assembly for scripts
			EngineApp.ProjectAssembly = typeof( Project.SimulationApp ).Assembly;

			//init engine application
			EngineApp.Init();

			if( !EngineApp.Create() )
			{
				Log.Fatal( "EngineApp.Create() failed." );
				return;
			}
		}

		//!!!!never call
		public static void ShutdownEngine()
		{
			EngineApp.Shutdown();
			Log.DumpToFile( "Program END\r\n" );
			VirtualFileSystem.Shutdown();
		}

		static private void Log_InvisibleInfoHandler( string text, ref bool dumpToLogFile )
		{
			global::Android.Util.Log.WriteLine( global::Android.Util.LogPriority.Debug, "MyApp", "Info: " + text );
		}

		static private void Log_InfoHandler( string text, ref bool dumpToLogFile )
		{
			global::Android.Util.Log.WriteLine( global::Android.Util.LogPriority.Debug, "MyApp", "Info: " + text );
		}

		static private void Log_WarningHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			global::Android.Util.Log.WriteLine( global::Android.Util.LogPriority.Debug, "MyApp", "Warning: " + text );
		}

		static private void Log_ErrorHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			global::Android.Util.Log.WriteLine( global::Android.Util.LogPriority.Debug, "MyApp", "Error: " + text );
		}

		static private void EngineApp_AppCreateBefore()
		{
			//preload NeoAxis.CoreExtension.dll
			AssemblyUtility.RegisterAssembly( typeof( CanvasRendererUtility ).Assembly, "" );

#if EXTENDED
			//preload NeoAxis.Extended.dll
			AssemblyUtility.RegisterAssembly( typeof( Component_RenderTargetInSpace ).Assembly, "" );
#endif

			//preload Project.dll
			AssemblyUtility.RegisterAssembly( typeof( Project.SimulationApp ).Assembly, "" );
		}

		static void UnzipFromStream( Stream zipStream, string outFolder )
		{
			using( var zipInputStream = new ZipInputStream( zipStream ) )
			{
				while( zipInputStream.GetNextEntry() is ZipEntry zipEntry )
				{
					var entryFileName = zipEntry.Name;
					// To remove the folder from the entry:
					//var entryFileName = Path.GetFileName(entryFileName);
					// Optionally match entrynames against a selection list here
					// to skip as desired.
					// The unpacked length is available in the zipEntry.Size property.

					// 4K is optimum
					var buffer = new byte[ 4096 ];

					// Manipulate the output filename here as desired.
					var fullZipToPath = Path.Combine( outFolder, entryFileName );
					var directoryName = Path.GetDirectoryName( fullZipToPath );
					if( directoryName.Length > 0 )
						Directory.CreateDirectory( directoryName );

					// Skip directory entry
					if( Path.GetFileName( fullZipToPath ).Length == 0 )
						continue;

					// Unzip file in buffered chunks. This is just as fast as unpacking
					// to a buffer the full size of the file, but does not waste memory.
					// The "using" will close the stream even if an exception occurs.
					using( FileStream streamWriter = File.Create( fullZipToPath ) )
					{
						ICSharpCode.SharpZipLib.Core.StreamUtils.Copy( zipInputStream, streamWriter, buffer );
					}
				}
			}
		}

		static void ExtractProjectZip( out string projectDir )
		{
			StartupTiming.CounterStart( "Extract project files from the archive" );

			//string projectDir;
			{
				var storageDir = System.Environment.GetFolderPath( System.Environment.SpecialFolder.Personal );
				//var storageDir = global::Android.OS.Environment.GetExternalStoragePublicDirectory( global::Android.OS.Environment.DirectoryDownloads ).AbsolutePath;
				//var storageDir = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				projectDir = Path.Combine( storageDir, "Project" );
			}

			var projectZipHashFileName = Path.Combine( projectDir, "Project.zip.hash" );

			var currentProjectZipHash = "";
			{
				if( File.Exists( projectZipHashFileName ) )
				{
					currentProjectZipHash = File.ReadAllText( projectZipHashFileName );
					currentProjectZipHash = currentProjectZipHash.Replace( "\r", "" ).Replace( "\n", "" ).Trim();
				}
			}

			var newProjectZipHash = "";
			{
				using( var stream = activity.Assets.Open( "Project.zip.hash" ) )
				using( var r = new StreamReader( stream ) )
					newProjectZipHash = r.ReadToEnd();
				newProjectZipHash = newProjectZipHash.Replace( "\r", "" ).Replace( "\n", "" ).Trim();
			}

			var alreadyExtracted = !string.IsNullOrEmpty( newProjectZipHash ) && newProjectZipHash == currentProjectZipHash;

			//extract Project.zip
			if( !alreadyExtracted )
			{
				//global::Android.Manifest.Permission.WriteExternalStorage
				//global::Android.Support.V4.App.ActivityCompat.RequestPermissions( this, new string[] { global::Android.Manifest.Permission.WriteExternalStorage, global::Android.Manifest.Permission.WriteExternalStorage }, int xxx );
				//ActivityCompat.requestPermissions( this, new String[] { Manifest.permission.WRITE_EXTERNAL_STORAGE }, REQUEST_CODE );

				//delete old directory
				if( Directory.Exists( projectDir ) )
					Directory.Delete( projectDir, true );

				//extract Project.zip
				{
					Directory.CreateDirectory( projectDir );

					using( var stream = activity.Assets.Open( "Project.zip" ) )
						UnzipFromStream( stream, projectDir );

					//write Project.zip.hash
					File.WriteAllText( projectZipHashFileName, newProjectZipHash );
				}
			}

			StartupTiming.CounterEnd( "Extract project files from the archive", alreadyExtracted ? "The archive is already extracted." : "" );
		}

		static void ProcessTouch( Viewport viewport, TouchData data )
		{
			bool handled = false;
			viewport.PerformTouch( data, ref handled );
			if( handled )
				return;

			//requested actions
			if( data.TouchDownRequestToControlActions.Count != 0 )
			{

				//!!!!
				int maxDistance = viewport.SizeInPixels.MinComponent() / 20;


				var filtered = new List<TouchData.TouchDownRequestToProcessTouch>();
				foreach( var i in data.TouchDownRequestToControlActions )
				{
					if( i.DistanceInPixels <= maxDistance )
						filtered.Add( i );
				}

				//sort by priority and distance to the control
				CollectionUtility.SelectionSort( filtered,
					delegate ( TouchData.TouchDownRequestToProcessTouch item1, TouchData.TouchDownRequestToProcessTouch item2 )
					{
						if( item1.ProcessPriority > item2.ProcessPriority )
							return -1;
						else if( item1.ProcessPriority < item2.ProcessPriority )
							return 1;

						if( item1.DistanceInPixels < item2.DistanceInPixels )
							return -1;
						else if( item1.DistanceInPixels > item2.DistanceInPixels )
							return 1;

						return 0;
					} );

				if( filtered.Count != 0 )
				{
					var item2 = filtered[ 0 ];
					item2.Action( item2.Sender, data, item2.AnyData );
				}
			}
		}

		static void PerformTouch( TouchEventItem item )
		{
			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
			if( viewport.SizeInPixels.X == 0 || viewport.SizeInPixels.Y == 0 )
				return;

			switch( item.ActionMasked )//switch( item.Action )
			{
			case MotionEventActions.Down:
			case MotionEventActions.PointerDown:
				{
					var index = item.ActionIndex;
					var id = item.PointersId[ index ];

					while( id >= pointerIdentifiers.Count )
						pointerIdentifiers.Add( null );
					pointerIdentifiers[ id ] = new object();


					var data = new TouchData();
					data.Action = TouchData.ActionEnum.Down;

					var position = item.PointersPosition[ index ];
					data.PositionInPixels = position.ToVector2I();
					data.Position = data.PositionInPixels.ToVector2F() / viewport.SizeInPixels.ToVector2F();

					data.PointerIdentifier = pointerIdentifiers[ id ];

					ProcessTouch( viewport, data );
				}
				break;

			case MotionEventActions.Up:
			case MotionEventActions.PointerUp:
				{
					var index = item.ActionIndex;
					var id = item.PointersId[ index ];

					var data = new TouchData();
					data.Action = TouchData.ActionEnum.Up;

					var position = item.PointersPosition[ index ];
					data.PositionInPixels = position.ToVector2I();
					data.Position = data.PositionInPixels.ToVector2F() / viewport.SizeInPixels.ToVector2F();

					data.PointerIdentifier = pointerIdentifiers[ id ];

					ProcessTouch( viewport, data );

					if( id < pointerIdentifiers.Count )
						pointerIdentifiers[ id ] = null;
					while( pointerIdentifiers.Count != 0 && pointerIdentifiers[ pointerIdentifiers.Count - 1 ] == null )
						pointerIdentifiers.RemoveAt( pointerIdentifiers.Count - 1 );
				}
				break;

			case MotionEventActions.Move:
				for( int index = 0; index < item.PointersPosition.Length; index++ )
				{
					var id = item.PointersId[ index ];
					if( id < pointerIdentifiers.Count )
					{
						var data = new TouchData();
						data.Action = TouchData.ActionEnum.Move;

						var position = item.PointersPosition[ index ];
						data.PositionInPixels = position.ToVector2I();
						data.Position = data.PositionInPixels.ToVector2F() / viewport.SizeInPixels.ToVector2F();

						data.PointerIdentifier = pointerIdentifiers[ id ];

						ProcessTouch( viewport, data );
					}
				}
				break;
			}
		}

		public static void ProcessTouchEvents()
		{
			lock( touchEventsQueue )
			{
				while( touchEventsQueue.Count != 0 )
				{
					var item = touchEventsQueue.Dequeue();
					PerformTouch( item );
				}
			}
		}

	}
}