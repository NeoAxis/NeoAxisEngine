// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Foundation;
using Internal;
using System.IO.Compression;

namespace NeoAxis.Player.iOS
{
	static class Engine
	{

		//!!!!impl. copied from Android


		//public static AppCompatActivity activity;

		//Thread engineMainThread;
		public volatile static bool engineInitialized;

		public struct TouchEventItem
		{
			//!!!!
			//public MotionEventActions Action;
			//public int ActionIndex;
			//public MotionEventActions ActionMasked;

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
			new PlatformFunctionalityIOS();
			EngineApp.ApplicationType = EngineApp.ApplicationTypeEnum.Simulation;

			//subscribe to log events
			Log.Handlers.InvisibleInfoHandler += Log_InvisibleInfoHandler;
			Log.Handlers.InfoHandler += Log_InfoHandler;
			Log.Handlers.WarningHandler += Log_WarningHandler;
			Log.Handlers.ErrorHandler += Log_ErrorHandler;

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
			AssemblyUtility.RegisterAssembly( typeof( Engine ).Assembly, "" );
			EngineApp.ProjectAssembly = typeof( Engine ).Assembly;

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
			Console.WriteLine( "Info: " + text );
			//global::Android.Util.Log.WriteLine( global::Android.Util.LogPriority.Debug, "MyApp", "Info: " + text );
		}

		static private void Log_InfoHandler( string text, ref bool dumpToLogFile )
		{
			Console.WriteLine( "Info: " + text );
			//global::Android.Util.Log.WriteLine( global::Android.Util.LogPriority.Debug, "MyApp", "Info: " + text );
		}

		static private void Log_WarningHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			Console.WriteLine( "Warning: " + text );
			//global::Android.Util.Log.WriteLine( global::Android.Util.LogPriority.Debug, "MyApp", "Warning: " + text );
		}

		static private void Log_ErrorHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			Console.WriteLine( "Error: " + text );
			//global::Android.Util.Log.WriteLine( global::Android.Util.LogPriority.Debug, "MyApp", "Error: " + text );
		}

		static void UnzipFromStream( Stream zipStream, string outFolder )
		{
			if( zipStream == null )
				throw new ArgumentNullException( nameof( zipStream ) );
			if( string.IsNullOrEmpty( outFolder ) )
				throw new ArgumentNullException( nameof( outFolder ) );

			Directory.CreateDirectory( outFolder );

			// Full path canonicalization base for traversal prevention.
			var outFolderFullPath = Path.GetFullPath( outFolder );
			if( !outFolderFullPath.EndsWith( Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal ) )
				outFolderFullPath += Path.DirectorySeparatorChar;

			using( var archive = new ZipArchive( zipStream, ZipArchiveMode.Read, leaveOpen: true ) )
			{
				foreach( var entry in archive.Entries )
				{
					// Zip entries use '/' by convention.
					var entryName = entry.FullName?.Replace( '\\', '/' ) ?? string.Empty;

					// Skip completely empty names.
					if( entryName.Length == 0 )
						continue;

					// Directory entry detection (common zip convention).
					var isDirectory = entryName.EndsWith( "/", StringComparison.Ordinal );

					// Reject rooted paths and drive-letter paths.
					// Also reject any traversal segments.
					{
						// Trim trailing slash for segment checks.
						var checkName = isDirectory ? entryName.TrimEnd( '/' ) : entryName;

						// Rooted ("/x") or drive ("C:/x") checks.
						if( checkName.StartsWith( "/", StringComparison.Ordinal ) )
							throw new InvalidDataException( $"Invalid zip entry path: '{entry.FullName}'." );

						if( checkName.Length >= 2 && char.IsLetter( checkName[ 0 ] ) && checkName[ 1 ] == ':' )
							throw new InvalidDataException( $"Invalid zip entry path: '{entry.FullName}'." );

						var segments = checkName.Split( new[] { '/' }, StringSplitOptions.RemoveEmptyEntries );
						for( int i = 0; i < segments.Length; i++ )
						{
							var seg = segments[ i ];
							if( seg == "." || seg == ".." )
								throw new InvalidDataException( $"Invalid zip entry path: '{entry.FullName}'." );
						}
					}

					// Build destination path using platform separators.
					var destinationPath = Path.Combine( outFolder, entryName.Replace( '/', Path.DirectorySeparatorChar ) );

					// Zip Slip protection: ensure destination is within outFolder.
					var destinationFullPath = Path.GetFullPath( destinationPath );
					if( !destinationFullPath.StartsWith( outFolderFullPath, StringComparison.Ordinal ) )
						throw new InvalidDataException( $"Zip entry is outside target dir: '{entry.FullName}'." );

					if( isDirectory )
					{
						Directory.CreateDirectory( destinationFullPath );
						continue;
					}

					var parentDir = Path.GetDirectoryName( destinationFullPath );
					if( !string.IsNullOrEmpty( parentDir ) )
						Directory.CreateDirectory( parentDir );

					// Some zips might include entries with empty Name but non-empty FullName; treat as directory.
					if( string.IsNullOrEmpty( entry.Name ) )
					{
						Directory.CreateDirectory( destinationFullPath );
						continue;
					}

					// Extract file (overwrite).
					using( var entryStream = entry.Open() )
					using( var outStream = new FileStream(
						destinationFullPath,
						FileMode.Create,
						FileAccess.Write,
						FileShare.None,
						bufferSize: 81920,
						useAsync: false ) )
					{
						entryStream.CopyTo( outStream );
					}

					// Preserve timestamp when available (does not throw on all platforms).
					try
					{
						if( entry.LastWriteTime != default )
							File.SetLastWriteTime( destinationFullPath, entry.LastWriteTime.DateTime );
					}
					catch { }
				}
			}
		}

		static void ExtractProjectZip( out string projectDir )
		{
			StartupTiming.CounterStart( "Extract project files from the archive" );

			//string projectDir;
			{
				var myDocuments = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );// MyDocuments );
				projectDir = Path.Combine( myDocuments, "Project" );
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
				using( var stream = File.OpenRead( NSBundle.MainBundle.PathForResource( "Project.zip.hash", "" ) ) )
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

					using( var stream = File.OpenRead( NSBundle.MainBundle.PathForResource( "Project.zip", "" ) ) )
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

			//!!!!

			//switch( item.ActionMasked )//switch( item.Action )
			//{
			//case MotionEventActions.Down:
			//case MotionEventActions.PointerDown:
			//	{
			//		var index = item.ActionIndex;
			//		var id = item.PointersId[ index ];

			//		while( id >= pointerIdentifiers.Count )
			//			pointerIdentifiers.Add( null );
			//		pointerIdentifiers[ id ] = new object();


			//		var data = new TouchData();
			//		data.Action = TouchData.ActionEnum.Down;

			//		var position = item.PointersPosition[ index ];
			//		data.PositionInPixels = position.ToVector2I();
			//		data.Position = data.PositionInPixels.ToVector2F() / viewport.SizeInPixels.ToVector2F();

			//		data.PointerIdentifier = pointerIdentifiers[ id ];

			//		ProcessTouch( viewport, data );
			//	}
			//	break;

			//case MotionEventActions.Up:
			//case MotionEventActions.PointerUp:
			//	{
			//		var index = item.ActionIndex;
			//		var id = item.PointersId[ index ];

			//		var data = new TouchData();
			//		data.Action = TouchData.ActionEnum.Up;

			//		var position = item.PointersPosition[ index ];
			//		data.PositionInPixels = position.ToVector2I();
			//		data.Position = data.PositionInPixels.ToVector2F() / viewport.SizeInPixels.ToVector2F();

			//		data.PointerIdentifier = pointerIdentifiers[ id ];

			//		ProcessTouch( viewport, data );

			//		if( id < pointerIdentifiers.Count )
			//			pointerIdentifiers[ id ] = null;
			//		while( pointerIdentifiers.Count != 0 && pointerIdentifiers[ pointerIdentifiers.Count - 1 ] == null )
			//			pointerIdentifiers.RemoveAt( pointerIdentifiers.Count - 1 );
			//	}
			//	break;

			//case MotionEventActions.Move:
			//	for( int index = 0; index < item.PointersPosition.Length; index++ )
			//	{
			//		var id = item.PointersId[ index ];
			//		if( id < pointerIdentifiers.Count )
			//		{
			//			var data = new TouchData();
			//			data.Action = TouchData.ActionEnum.Move;

			//			var position = item.PointersPosition[ index ];
			//			data.PositionInPixels = position.ToVector2I();
			//			data.Position = data.PositionInPixels.ToVector2F() / viewport.SizeInPixels.ToVector2F();

			//			data.PointerIdentifier = pointerIdentifiers[ id ];

			//			ProcessTouch( viewport, data );
			//		}
			//	}
			//	break;

			//default:
			//	return;
			//}

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