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
using Android.Views;
using Android.Widget;
using Internal;
using System.IO.Compression;

namespace NeoAxis.Player.Android
{
	static class Engine
	{
		public static MainActivity/*AppCompatActivity*/ activity;

		//Thread engineMainThread;
		public volatile static bool engineInitialized;

		public static Queue<InputEventItem> inputEventQueue = new Queue<InputEventItem>();
		//public static Queue<TouchEventItem> touchEventsQueue = new Queue<TouchEventItem>();
		//public static Queue<KeyDownEventItem> keyDownEventsQueue = new Queue<KeyDownEventItem>();

		static List<object> pointerIdentifiers = new List<object>();

		/////////////////////////////////////////

		public abstract class InputEventItem
		{
		}

		/////////////////////////////////////////

		public class TouchEventItem : InputEventItem
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

		/////////////////////////////////////////

		public class KeyDownEventItem : InputEventItem
		{
			public EKeys KeyCode;
			public char Character;
		}

		/////////////////////////////////////////

		public static void InitEngine()
		{
			try
			{
				Log.Handlers.InvisibleInfoHandler += Log_InvisibleInfoHandler;
				Log.Handlers.InfoHandler += Log_InfoHandler;
				Log.Handlers.WarningHandler += Log_WarningHandler;
				Log.Handlers.ErrorHandler += Log_ErrorHandler;

				//EngineApp.AppCreateBefore += EngineApp_AppCreateBefore;

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

				//register project assembly
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
			catch( Exception ex )
			{
				Log.Fatal( "Engine initialization failed: " + ex.Message );
				return;
			}
		}

		//never call
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

		//static private void EngineApp_AppCreateBefore()
		//{
		//	//preload Project.dll
		//	AssemblyUtility.RegisterAssembly( typeof( Project.SimulationApp ).Assembly, "" );
		//}

		static void UnzipFromStream( Stream zipStream, string outFolder )
		{
			// Plan (pseudocode):
			// - Ensure `outFolder` exists.
			// - Use `System.IO.Compression.ZipArchive` (modern API) over the input `zipStream`.
			// - For each entry:
			//   - Normalize entry path separators to '/' and reject unsafe paths:
			//     - empty entries are allowed (directories)
			//     - reject rooted paths, drive letters, and any path containing ".." segments
			//   - Combine with `outFolder`, get full path, and ensure it is still under `outFolder` (Zip Slip protection).
			//   - If directory entry: create directory.
			//   - Else:
			//     - create parent directory
			//     - extract by copying entry stream to a newly created file (overwrite).
			//     - (optional) set last write time if available.
			//
			// Notes:
			// - Works on modern Android (.NET for Android) without external zip libs.
			// - Keeps memory usage low (streaming copy).

			if( zipStream == null )
				throw new ArgumentNullException( nameof( zipStream ) );
			if( string.IsNullOrEmpty( outFolder ) )
				throw new ArgumentNullException( nameof( outFolder ) );

			Directory.CreateDirectory( outFolder );

			// Full path canonicalization base for traversal prevention.
			var outFolderFullPath = Path.GetFullPath( outFolder );
			if( !outFolderFullPath.EndsWith( Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal ) )
				outFolderFullPath += Path.DirectorySeparatorChar;

			// IMPORTANT: If this file doesn't already have the using, add at top:
			// using System.IO.Compression;

			using( var archive = new ZipArchive( zipStream, System.IO.Compression.ZipArchiveMode.Read, leaveOpen: true ) )
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


			//using( var zipInputStream = new ZipInputStream( zipStream ) )
			//{
			//	while( zipInputStream.GetNextEntry() is ZipEntry zipEntry )
			//	{
			//		var entryFileName = zipEntry.Name;
			//		// To remove the folder from the entry:
			//		//var entryFileName = Path.GetFileName(entryFileName);
			//		// Optionally match entrynames against a selection list here
			//		// to skip as desired.
			//		// The unpacked length is available in the zipEntry.Size property.

			//		// 4K is optimum
			//		var buffer = new byte[ 4096 ];

			//		// Manipulate the output filename here as desired.
			//		var fullZipToPath = Path.Combine( outFolder, entryFileName );
			//		var directoryName = Path.GetDirectoryName( fullZipToPath );
			//		if( directoryName.Length > 0 )
			//			Directory.CreateDirectory( directoryName );

			//		// Skip directory entry
			//		if( Path.GetFileName( fullZipToPath ).Length == 0 )
			//			continue;

			//		// Unzip file in buffered chunks. This is just as fast as unpacking
			//		// to a buffer the full size of the file, but does not waste memory.
			//		// The "using" will close the stream even if an exception occurs.
			//		using( FileStream streamWriter = File.Create( fullZipToPath ) )
			//		{
			//			ICSharpCode.SharpZipLib.Core.StreamUtils.Copy( zipInputStream, streamWriter, buffer );
			//		}
			//	}
			//}
		}

		static string ReadAssetText( string assetName )
		{
			// Plan (pseudocode):
			// - Open asset via AssetManager.
			// - Read its bytes into MemoryStream (avoid StreamReader/ReadToEnd edge cases with encoding/BOM/partial reads).
			// - Decode bytes:
			//   - If UTF-8 BOM present, skip BOM and decode UTF-8.
			//   - Else decode as UTF-8 without BOM.
			// - Normalize line endings/trimming is done by the caller.

			using( var stream = activity.Assets.Open( assetName ) )
			using( var ms = new MemoryStream() )
			{
				stream.CopyTo( ms );
				var bytes = ms.ToArray();

				// UTF-8 BOM: EF BB BF
				int offset = 0;
				if( bytes.Length >= 3 && bytes[ 0 ] == 0xEF && bytes[ 1 ] == 0xBB && bytes[ 2 ] == 0xBF )
					offset = 3;

				return Encoding.UTF8.GetString( bytes, offset, bytes.Length - offset );
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
				// Avoid StreamReader.ReadToEnd() crashing on some devices/encodings by reading bytes first.
				newProjectZipHash = ReadAssetText( "Project.zip.hash" );
				newProjectZipHash = newProjectZipHash.Replace( "\r", "" ).Replace( "\n", "" ).Trim();

				//using( var stream = activity.Assets.Open( "Project.zip.hash" ) )
				//using( var r = new StreamReader( stream ) )
				//	newProjectZipHash = r.ReadToEnd();
				//newProjectZipHash = newProjectZipHash.Replace( "\r", "" ).Replace( "\n", "" ).Trim();
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
				int maxDistance = viewport.SizeInPixels.MinComponent() / 30;

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

		static void ProcessInputEvent_Touch( TouchEventItem item )
		{
			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

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

		static void ProcessInputEvent_KeyDown( KeyDownEventItem item )
		{
			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

			if( item.KeyCode != EKeys.None )
			{
				var data = new KeyEvent( item.KeyCode );

				{
					var handled = false;
					viewport.PerformKeyDown( data, ref handled );
				}

				//!!!!

				{
					var handled = false;
					viewport.PerformKeyUp( data, ref handled );
				}
			}
			else
			{
				var data = new KeyPressEvent( item.Character );

				bool handled = false;
				viewport.PerformKeyPress( data, ref handled );
				//if( handled )
				//	return;
			}

			//!!!!

			//{
			//	var data = new KeyEvent( key );

			//	bool handled = false;
			//	viewport.PerformKeyUp( data, ref handled );
			//	//if( handled )
			//	//	return;
			//}



			//EKeys key = EKeys.None;

			//if( key == EKeys.None )
			//	return;

			//{
			//	var data = new KeyEvent( key );

			//	bool handled = false;
			//	viewport.PerformKeyDown( data, ref handled );
			//	//if( handled )
			//	//	return;
			//}

			////!!!!

			//{
			//	var data = new KeyEvent( key );

			//	bool handled = false;
			//	viewport.PerformKeyUp( data, ref handled );
			//	//if( handled )
			//	//	return;
			//}
		}

		static void ProcessInputEvent( InputEventItem item )
		{
			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
			if( viewport.SizeInPixels.X == 0 || viewport.SizeInPixels.Y == 0 )
				return;

			var touchItem = item as TouchEventItem;
			if( touchItem != null )
				ProcessInputEvent_Touch( touchItem );

			var keyDownItem = item as KeyDownEventItem;
			if( keyDownItem != null )
				ProcessInputEvent_KeyDown( keyDownItem );
		}

		public static void ProcessInputEvents()
		{
			lock( inputEventQueue )
			{
				while( inputEventQueue.Count != 0 )
				{
					var item = inputEventQueue.Dequeue();
					ProcessInputEvent( item );
				}
			}
		}
	}
}