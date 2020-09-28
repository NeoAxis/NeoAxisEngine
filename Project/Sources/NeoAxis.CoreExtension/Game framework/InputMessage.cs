// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Input;

namespace NeoAxis
{
	public class InputMessage
	{
		public bool Handled { get; set; }
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class InputMessageKey : InputMessage
	{
	}

	public class InputMessageKeyDown : InputMessageKey
	{
		EKeys key;

		public InputMessageKeyDown( EKeys key )
		{
			this.key = key;
		}

		public EKeys Key { get { return key; } }
	}

	public class InputMessageKeyPress : InputMessageKey
	{
		char keyChar;

		public InputMessageKeyPress( char keyChar )
		{
			this.keyChar = keyChar;
		}

		public char KeyChar { get { return keyChar; } }
	}

	public class InputMessageKeyUp : InputMessageKey
	{
		EKeys key;

		public InputMessageKeyUp( EKeys key )
		{
			this.key = key;
		}

		public EKeys Key { get { return key; } }
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class InputMessageMouse : InputMessage
	{
	}

	public abstract class InputMessageMouseButton : InputMessageMouse
	{
		EMouseButtons button;

		protected InputMessageMouseButton( EMouseButtons button )
		{
			this.button = button;
		}

		public EMouseButtons Button { get { return button; } }
	}

	public class InputMessageMouseButtonDown : InputMessageMouseButton
	{
		public InputMessageMouseButtonDown( EMouseButtons button )
			: base( button )
		{
		}
	}

	public class InputMessageMouseButtonUp : InputMessageMouseButton
	{
		public InputMessageMouseButtonUp( EMouseButtons button )
			: base( button )
		{
		}
	}

	public class InputMessageMouseDoubleClick : InputMessageMouseButton
	{
		public InputMessageMouseDoubleClick( EMouseButtons button )
			: base( button )
		{
		}
	}

	public class InputMessageMouseMove : InputMessageMouse
	{
		Vector2 position;

		public InputMessageMouseMove( Vector2 position )
		{
			this.position = position;
		}

		public Vector2 Position { get { return position; } }
	}

	public class InputMessageMouseWheel : InputMessageMouse
	{
		int delta;

		public InputMessageMouseWheel( int delta )
		{
			this.delta = delta;
		}

		public int Delta { get { return delta; } }
	}

	public class InputMessageMouseRelativeModeChanged : InputMessageMouse
	{
		bool value;

		public InputMessageMouseRelativeModeChanged( bool value )
		{
			this.value = value;
		}

		public bool Value { get { return value; } }
	}

	public class InputMessageJoystick : InputMessage
	{
		JoystickInputEvent inputEvent;

		public InputMessageJoystick( JoystickInputEvent inputEvent )
		{
			this.inputEvent = inputEvent;
		}

		public JoystickInputEvent InputEvent { get { return inputEvent; } }
	}

	public class InputMessageTouch : InputMessage
	{
		TouchData touchEvent;

		public InputMessageTouch( TouchData touchEvent )
		{
			this.touchEvent = touchEvent;
		}

		public TouchData TouchEvent { get { return touchEvent; } }
	}

	public class InputMessageSpecialInputDevice : InputMessage
	{
		InputEvent inputEvent;

		public InputMessageSpecialInputDevice( InputEvent inputEvent )
		{
			this.inputEvent = inputEvent;
		}

		public InputEvent InputEvent { get { return inputEvent; } }
	}

	public class InputMessageInputEnabledChanged : InputMessageMouse
	{
		bool value;

		public InputMessageInputEnabledChanged( bool value )
		{
			this.value = value;
		}

		public bool Value { get { return value; } }
	}

}
