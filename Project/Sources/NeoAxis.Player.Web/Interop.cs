// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using Project;
using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace NeoAxis.Player.Web
{
	internal static partial class Interop
	{
		[JSImport( "initialize", "main.js" )]
		public static partial void Initialize();

		[JSExport]
		public static void OnKeyDown( int code, string character, int modifiers, bool keyLocked )
		{
			var item = new Engine.KeyEventItem();
			item.Type = Engine.KeyEventType.Down;
			item.Code = (EKeys)code;
			item.Modifiers = (Engine.InputModifiers)modifiers;
			item.KeyLocked = keyLocked;
			if( !string.IsNullOrEmpty( character ) && character.Length == 1 && !char.IsControl( character, 0 ) )
				item.Character = character[ 0 ];
			Engine.inputEventQueue.Enqueue( item );
		}

		[JSExport]
		public static void OnKeyUp( int code, int modifiers, bool keyLocked )
		{
			var item = new Engine.KeyEventItem();
			item.Type = Engine.KeyEventType.Up;
			item.Code = (EKeys)code;
			item.Modifiers = (Engine.InputModifiers)modifiers;
			item.KeyLocked = keyLocked;

			Engine.inputEventQueue.Enqueue( item );
		}

		[JSExport]
		public static void OnMouseMove( float x, float y )
		{
			Engine.inputEventQueue.Enqueue( new Engine.MouseEventItem()
			{
				Action = Engine.ActionEnum.Move,
				Vector = new Vector2F( x, y )
			} );
		}

		[JSExport]
		public static void OnMouseMoveRelative( float deltaX, float deltaY )
		{
			Engine.inputEventQueue.Enqueue( new Engine.MouseEventItem()
			{
				Action = Engine.ActionEnum.Move,
				Relative = true,
				Vector = new Vector2F( deltaX, deltaY )
			} );
		}

		[JSExport]
		public static void OnMouseRelativeModeChanged( bool enabled )
		{
			PlatformFunctionalityWeb.mouseRelativeModeActive = enabled;
			PlatformFunctionalityWeb.mouseRelativeModeDelta = Vector2F.Zero;
		}

		[JSExport]
		public static void OnMouseWheel( float deltaX, float deltaY )
		{
			Engine.inputEventQueue.Enqueue( new Engine.MouseEventItem()
			{
				Action = Engine.ActionEnum.Wheel,
				Vector = new Vector2F( deltaX, deltaY )
			} );
		}

		static bool ConvertMouseButton( int browserButton, out EMouseButtons button )
		{
			switch( browserButton )
			{
			case 0: button = EMouseButtons.Left; return true;
			case 1: button = EMouseButtons.Middle; return true;
			case 2: button = EMouseButtons.Right; return true;
			case 3: button = EMouseButtons.XButton1; return true;
			case 4: button = EMouseButtons.XButton2; return true;
			default: button = EMouseButtons.Left; return false;
			}
		}

		[JSExport]
		public static void OnMouseDown( int button, int modifiers )
		{
			if( ConvertMouseButton( button, out var button2 ) )
			{
				Engine.inputEventQueue.Enqueue( new Engine.MouseEventItem()
				{
					Action = Engine.ActionEnum.Down,
					Button = button2
				} );
			}
		}

		[JSExport]
		public static void OnMouseUp( int button, int modifiers )
		{
			if( ConvertMouseButton( button, out var button2 ) )
			{
				Engine.inputEventQueue.Enqueue( new Engine.MouseEventItem()
				{
					Action = Engine.ActionEnum.Up,
					Button = button2
				} );
			}
		}

		[JSExport]
		public static void OnMouseDoubleClick( int button, int modifiers )
		{
			if( ConvertMouseButton( button, out var button2 ) )
			{
				Engine.inputEventQueue.Enqueue( new Engine.MouseEventItem()
				{
					Action = Engine.ActionEnum.DoubleClick,
					Button = button2
				} );
			}
		}

		[JSExport]
		public static void OnTouchMove( int touchId, float x, float y )
		{
			Engine.inputEventQueue.Enqueue( new Engine.TouchEventItem()
			{
				Action = Engine.ActionEnum.Move,
				Id = touchId,
				Position = new Vector2F( x, y )
			} );
		}

		[JSExport]
		public static void OnTouchStart( int touchId, float x, float y, int modifiers )
		{
			Engine.inputEventQueue.Enqueue( new Engine.TouchEventItem()
			{
				Action = Engine.ActionEnum.Down,
				Id = touchId,
				Position = new Vector2F( x, y )
			} );
		}

		[JSExport]
		public static void OnTouchEnd( int touchId, float x, float y, int modifiers )
		{
			Engine.inputEventQueue.Enqueue( new Engine.TouchEventItem()
			{
				Action = Engine.ActionEnum.Up,
				Id = touchId,
				Position = new Vector2F( x, y )
			} );
		}

		[JSExport]
		public static void OnCanvasResize( float width, float height/*, float devicePixelRatio*/, bool fullscreenEnabled )
		{
			PlatformFunctionalityWeb.screenSize = new Vector2I( (int)width, (int)height );
			Program.surfaceResized = true;

			//update windowed mode in the engine
			EngineApp.SetWindowedMode( fullscreenEnabled ? WindowedModeEnum.Fullscreen : WindowedModeEnum.Windowed, EngineApp.WindowedModeSize, false );
			SimulationApp.WindowedMode = EngineApp.WindowedMode;
		}

		[JSExport]
		public static void SetRootUri( string uri )
		{
			Program.BaseAddress = new Uri( uri );
		}

		[JSImport( "setClipboardText", "main.js" )]
		public static partial void SetClipboardText( string text );

		[JSImport( "getClipboardTextAsync", "main.js" )]
		public static partial Task<string> GetClipboardTextAsync();

		[JSImport( "setFullscreenAsync", "main.js" )]
		public static partial Task SetFullscreenAsync( bool enable );

		[JSImport( "setMouseRelativeMode", "main.js" )]
		public static partial void SetMouseRelativeMode( bool enable );

		[JSImport( "hideLogo", "main.js" )]
		internal static partial void HideLogo();
	}
}