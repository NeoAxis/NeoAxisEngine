// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Internal;

namespace NeoAxis.Player.Web
{
	static class Engine
	{
		public static Queue<InputEventItem> inputEventQueue = new Queue<InputEventItem>();

		/////////////////////////////////////////

		public abstract class InputEventItem
		{
		}

		public enum InputModifiers
		{
			Shift = 1,
			Ctrl = 2,
			Alt = 4,
			Meta = 8
		}

		public enum ActionEnum
		{
			Down,
			Up,
			Move,
			Wheel,
		}

		/////////////////////////////////////////

		public class TouchEventItem : InputEventItem
		{
			public int Id;
			public Vector2F Position;
			public ActionEnum Action;
		}

		/////////////////////////////////////////

		public class MouseEventItem : InputEventItem
		{
			public EMouseButtons Button;
			public ActionEnum Action;
			public Vector2F Vector;
		}

		/////////////////////////////////////////

		public class KeyEventItem : InputEventItem
		{
			public EKeys Code;
			public KeyEventType Type;
			public char Character;
		}

		public enum KeyEventType
		{
			Up,
			Down,
			Repeat
		}

		/////////////////////////////////////////

		public static async Task<bool> InitEngine( HttpClient client )
		{
			new PlatformFunctionalityWeb();
			EngineApp.ApplicationType = EngineApp.ApplicationTypeEnum.Simulation;

			//subscribe to log events
			Log.Handlers.InvisibleInfoHandler += Log_InvisibleInfoHandler;
			Log.Handlers.InfoHandler += Log_InfoHandler;
			Log.Handlers.WarningHandler += Log_WarningHandler;
			Log.Handlers.ErrorHandler += Log_ErrorHandler;

			var projectDir = await ExtractProjectZip( client );

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
				return false;

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
				return false;
			}
			return true;
		}

		public static void ProcessInputEvents()
		{
			while( inputEventQueue.Count != 0 )
			{
				var item = inputEventQueue.Dequeue();
				ProcessInputEvent( item );
			}
		}

		static private void Log_InvisibleInfoHandler( string text, ref bool dumpToLogFile )
		{
			Console.WriteLine( "[MyApp] Info: {0}", text );
		}

		static private void Log_InfoHandler( string text, ref bool dumpToLogFile )
		{
			Console.WriteLine( "[MyApp] Info: {0}", text );
		}

		static private void Log_WarningHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			Console.WriteLine( "[MyApp] Warning: {0}", text );
		}

		static private void Log_ErrorHandler( string text, ref bool handled, ref bool dumpToLogFile )
		{
			Console.WriteLine( "[MyApp] Error: {0}", text );
		}

		static async Task<string> ExtractProjectZip( HttpClient client )
		{
			StartupTiming.CounterStart( "Extract project files from the archive" );

			string projectDir;
			{
				var storageDir = Environment.GetEnvironmentVariable( "HOME" );
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
				using( var stream = await DownloadFile( client, "Assets/Project.zip.hash" ) )
				using( var r = new StreamReader( stream ) )
					newProjectZipHash = r.ReadToEnd();
				newProjectZipHash = newProjectZipHash.Replace( "\r", "" ).Replace( "\n", "" ).Trim();
			}

			var alreadyExtracted = !string.IsNullOrEmpty( newProjectZipHash ) && newProjectZipHash == currentProjectZipHash;

			//extract Project.zip
			if( !alreadyExtracted )
			{
				//delete old directory
				if( Directory.Exists( projectDir ) )
					Directory.Delete( projectDir, true );

				//extract Project.zip
				{
					Directory.CreateDirectory( projectDir );

					using( var stream = await DownloadFile( client, "Assets/Project.zip" ) )
						await UnzipFromStream( stream, projectDir );

					//write Project.zip.hash
					File.WriteAllText( projectZipHashFileName, newProjectZipHash );
				}
			}

			StartupTiming.CounterEnd( "Extract project files from the archive", alreadyExtracted ? "The archive is already extracted." : "" );

			return projectDir;
		}

		static async Task UnzipFromStream( Stream zipStream, string outFolder )
		{
			using( var zipInputStream = new ZipArchive( zipStream, ZipArchiveMode.Read ) )
			{
				foreach( var zipEntry in zipInputStream.Entries )
				{
					var entryFileName = zipEntry.FullName;
					// To remove the folder from the entry:
					//var entryFileName = Path.GetFileName(entryFileName);
					// Optionally match entrynames against a selection list here
					// to skip as desired.
					// The unpacked length is available in the zipEntry.Size property.

					// Manipulate the output filename here as desired.
					var fullZipToPath = PathUtility.NormalizePath( Path.Combine( outFolder, entryFileName ) );
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
						using var stream = zipEntry.Open();
						await stream.CopyToAsync( streamWriter );
					}
				}
			}
		}

		static async Task<Stream> DownloadFile( HttpClient client, string path )
		{
			//TODO: caching options (dont cache, cache locally (i.e. without caching on intermediate servers), clear cache)
			var response = await client.GetAsync( new Uri( path, UriKind.Relative ) );
			if( !response.IsSuccessStatusCode )
				throw new Exception();
			return await response.Content.ReadAsStreamAsync();
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

			switch( item.Action )
			{
			case ActionEnum.Down:
				{
					var data = new TouchData();
					data.PointerIdentifier = item.Id;
					data.Action = TouchData.ActionEnum.Down;
					data.PositionInPixels = item.Position.ToVector2I();
					data.Position = data.PositionInPixels.ToVector2F() / viewport.SizeInPixels.ToVector2F();

					ProcessTouch( viewport, data );
				}
				break;

			case ActionEnum.Up:
				{
					var data = new TouchData();
					data.PointerIdentifier = item.Id;
					data.Action = TouchData.ActionEnum.Up;
					data.PositionInPixels = item.Position.ToVector2I();
					data.Position = data.PositionInPixels.ToVector2F() / viewport.SizeInPixels.ToVector2F();

					ProcessTouch( viewport, data );
				}
				break;

			case ActionEnum.Move:
				{
					var data = new TouchData();
					data.PointerIdentifier = item.Id;
					data.Action = TouchData.ActionEnum.Move;
					data.PositionInPixels = item.Position.ToVector2I();
					data.Position = data.PositionInPixels.ToVector2F() / viewport.SizeInPixels.ToVector2F();

					ProcessTouch( viewport, data );
				}
				break;
			}
		}

		static void ProcessInputEvent_Mouse( MouseEventItem item )
		{
			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

			switch( item.Action )
			{
			case ActionEnum.Down:
				{
					var handled = false;
					viewport.PerformMouseDown( item.Button, ref handled );
				}
				break;
			case ActionEnum.Up:
				{
					var handled = false;
					viewport.PerformMouseUp( item.Button, ref handled );
				}
				break;
			case ActionEnum.Move:
				{
					viewport.PerformMouseMove( item.Vector.ToVector2() / viewport.SizeInPixels.ToVector2() );
				}
				break;
			case ActionEnum.Wheel:
				{
					var handled = false;
					viewport.PerformMouseWheel( (int)item.Vector.Y, ref handled );
				}
				break;
			}
		}

		static void ProcessInputEvent_KeyDown( KeyEventItem item )
		{
			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];

			if( item.Code != EKeys.None )
			{
				var data = new KeyEvent( item.Code );

				switch( item.Type )
				{
				case KeyEventType.Down:
					{
						var handled = false;
						viewport.PerformKeyDown( data, ref handled );
					}
					break;
				case KeyEventType.Up:
					{
						var handled = false;
						viewport.PerformKeyUp( data, ref handled );
					}
					break;
				}
			}
			else if( item.Type != KeyEventType.Up )
			{
				var data = new KeyPressEvent( item.Character );

				bool handled = false;
				viewport.PerformKeyPress( data, ref handled );
			}
		}

		static void ProcessInputEvent( InputEventItem item )
		{
			var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
			if( viewport.SizeInPixels.X == 0 || viewport.SizeInPixels.Y == 0 )
				return;

			var touchItem = item as TouchEventItem;
			if( touchItem != null )
				ProcessInputEvent_Touch( touchItem );

			var mouseItem = item as MouseEventItem;
			if( mouseItem != null )
				ProcessInputEvent_Mouse( mouseItem );

			var keyDownItem = item as KeyEventItem;
			if( keyDownItem != null )
				ProcessInputEvent_KeyDown( keyDownItem );
		}
	}
}