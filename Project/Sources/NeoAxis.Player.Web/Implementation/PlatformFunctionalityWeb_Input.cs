// Copyright 2006–2026 Ivan Efimov. All rights reserved.

namespace NeoAxis
{
	public partial class PlatformFunctionalityWeb
	{
		public override Vector2 CreatedWindow_GetMousePosition()
		{
			return Vector2.Zero;
		}

		public override void CreatedWindow_SetMousePosition( Vector2 value )
		{
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
			return false;
		}

		public override void CreatedWindow_OnMouseRelativeModeChange()
		{
		}

		public override void CreatedWindow_UpdateMouseRelativeMove( out Vector2 delta )
		{
			delta = Vector2F.Zero;
		}
	}
}
