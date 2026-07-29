// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using NeoAxis.Player.Web;

namespace NeoAxis
{
	public partial class PlatformFunctionalityWeb
	{
		public static Vector2F cursorPosition;

		public static Vector2F mouseRelativeModeDelta;

		public static bool mouseRelativeModeActive;

		public override Vector2 CreatedWindow_GetMousePosition()
		{
			var size = GetScreenSize();
			if( size.X <= 0 || size.Y <= 0 )
			return Vector2.Zero;
			return cursorPosition.ToVector2() / size.ToVector2();
		}

		public override void CreatedWindow_SetMousePosition( Vector2 value )
		{
			var size = GetScreenSize();
			cursorPosition = new Vector2F( (float)( value.X * size.X ), (float)( value.Y * size.Y ) );
		}

		public override void CreatedWindow_UpdateShowSystemCursor( bool forceUpdate )
		{
		}

		public override void CreatedWindow_UpdateSystemCursorFileName()
		{
		}

		public unsafe override bool InitDirectInputMouseDevice()
		{
			return false;
		}

		public override void ShutdownDirectInputMouseDevice()
		{
		}

		public unsafe override void CreatedWindow_UpdateInputDevices()
		{
		}

		public override bool IsKeyLocked( EKeys key )
		{
			lock( Engine.keyLockedStates )
			{
				if( Engine.keyLockedStates.TryGetValue( key, out var locked ) )
					return locked;
			}
			return false;
		}

		public override void CreatedWindow_OnMouseRelativeModeChange()
		{
			var renderTarget = RenderingSystem.ApplicationRenderTarget;
			if( renderTarget == null )
				return;

			var enable = renderTarget.Viewports[ 0 ].MouseRelativeMode;

			//drop whatever was accumulated before the switch, it belongs to the previous mode
			mouseRelativeModeDelta = Vector2F.Zero;

			Interop.SetMouseRelativeMode( enable );
		}

		public override void CreatedWindow_UpdateMouseRelativeMove( out Vector2 delta )
		{
			var size = GetScreenSize();
			if( size.X <= 0 || size.Y <= 0 )
			{
				delta = Vector2.Zero;
				return;
			}

			delta = mouseRelativeModeDelta.ToVector2() / size.ToVector2();
			mouseRelativeModeDelta = Vector2F.Zero;
		}
	}
}
