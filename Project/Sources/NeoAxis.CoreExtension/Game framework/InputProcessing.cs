// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// An object to process input from a player.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Input Processing", -9998 )]
	public class InputProcessing : Component
	{
		bool inputEnabled;

		bool[] keys;

		//!!!!memory. maybe allocate memory per request

		bool[] mouseButtons = new bool[ 5 ];
		Vector2 mousePosition = new Vector2( -10000, -10000 );
		bool mouseRelativeMode;

		bool[] joystickButtons = new bool[ 64 ];
		double[] joystickAxes = new double[ 32 ];
		JoystickPOVDirections[] joystickPOVs = new JoystickPOVDirections[ 16 ];
		Vector2[] joystickSliders = new Vector2[ 16 ];

		List<TouchPointerData> touchPointers = new List<TouchPointerData>();
		Vector2[] touchSliders = new Vector2[ 4 ];

		/////////////////////////////////////////

		public class TouchPointerData
		{
			public Vector2 Position;
			public object PointerIdentifier;

			public TouchPointerData()
			{
			}

			public TouchPointerData( Vector2 position, object pointerIdentifier )
			{
				Position = position;
				PointerIdentifier = pointerIdentifier;
			}
		}

		/////////////////////////////////////////

		static int GetEKeysMaxIndex()
		{
			int maxIndex = 0;
			foreach( EKeys eKey in Enum.GetValues( typeof( EKeys ) ) )
			{
				int index = (int)eKey;
				if( index > maxIndex )
					maxIndex = index;
			}
			return maxIndex;
		}

		protected virtual void OnInputMessage( GameMode gameMode, InputMessage message ) { }

		public delegate void InputMessageEventDelegate( InputProcessing sender, GameMode gameMode, InputMessage message );
		public event InputMessageEventDelegate InputMessageEvent;

		public void KeyMouseTouchUpAll()
		{
			if( keys != null )
			{
				for( int n = 0; n < keys.Length; n++ )
					keys[ n ] = false;
			}
			for( int n = 0; n < mouseButtons.Length; n++ )
				mouseButtons[ n ] = false;
			for( int n = 0; n < joystickButtons.Length; n++ )
				joystickButtons[ n ] = false;
			for( int n = 0; n < joystickAxes.Length; n++ )
				joystickAxes[ n ] = 0;
			for( int n = 0; n < joystickPOVs.Length; n++ )
				joystickPOVs[ n ] = JoystickPOVDirections.Centered;
			for( int n = 0; n < joystickSliders.Length; n++ )
				joystickSliders[ n ] = Vector2.Zero;
			touchPointers.Clear();
			for( int n = 0; n < touchSliders.Length; n++ )
				touchSliders[ n ] = Vector2.Zero;
		}

		void InputMessageBefore( GameMode gameMode, InputMessage message )
		{
			//input enabled changed
			{
				var m = message as InputMessageInputEnabledChanged;
				if( m != null )
				{
					inputEnabled = m.Value;

					if( !inputEnabled )
						KeyMouseTouchUpAll();
				}
			}

			//key down
			{
				var m = message as InputMessageKeyDown;
				if( m != null )
				{
					if( keys == null )
						keys = new bool[ GetEKeysMaxIndex() + 1 ];
					keys[ (int)m.Key ] = true;
				}
			}

			//key up
			{
				var m = message as InputMessageKeyUp;
				if( m != null )
				{
					if( keys == null )
						keys = new bool[ GetEKeysMaxIndex() + 1 ];
					keys[ (int)m.Key ] = false;
				}
			}

			//mouse button down
			{
				var m = message as InputMessageMouseButtonDown;
				if( m != null )
					mouseButtons[ (int)m.Button ] = true;
			}

			//mouse button up
			{
				var m = message as InputMessageMouseButtonUp;
				if( m != null )
					mouseButtons[ (int)m.Button ] = false;
			}

			//mouse move
			{
				var m = message as InputMessageMouseMove;
				if( m != null )
					mousePosition = m.Position;
			}

			//mouse relative mode
			{
				var m = message as InputMessageMouseRelativeModeChanged;
				if( m != null )
					mouseRelativeMode = m.Value;
			}

			//joystick
			{
				var m = message as InputMessageJoystick;
				if( m != null )
				{
					//button down
					{
						var m2 = m.InputEvent as JoystickButtonDownEvent;
						if( m2 != null )
						{
							var index = m2.Button.Index;
							if( index >= 0 && index < joystickButtons.Length )
								joystickButtons[ index ] = true;
						}
					}

					//button up
					{
						var m2 = m.InputEvent as JoystickButtonUpEvent;
						if( m2 != null )
						{
							var index = m2.Button.Index;
							if( index >= 0 && index < joystickButtons.Length )
								joystickButtons[ index ] = false;
						}
					}

					//axis
					{
						var m2 = m.InputEvent as JoystickAxisChangedEvent;
						if( m2 != null )
						{
							var index = (int)m2.Axis.Name;
							if( index >= 0 && index < joystickAxes.Length )
								joystickAxes[ index ] = m2.Axis.Value;
						}
					}

					//POV
					{
						var m2 = m.InputEvent as JoystickPOVChangedEvent;
						if( m2 != null )
						{
							var index = (int)m2.POV.Name;
							if( index >= 0 && index < joystickPOVs.Length )
								joystickPOVs[ index ] = m2.POV.Value;
						}
					}

					//slider
					{
						var m2 = m.InputEvent as JoystickSliderChangedEvent;
						if( m2 != null )
						{
							var index = (int)m2.Slider.Name;
							if( index >= 0 && index < joystickPOVs.Length )
								joystickSliders[ index ] = m2.Slider.Value;
						}
					}
				}
			}

			//touch pointers
			{
				var m2 = message as InputMessageTouch;
				if( m2 != null )
				{
					var touch = m2.TouchEvent;

					switch( touch.Action )
					{
					case TouchData.ActionEnum.Down:
						{
							var data = GetTouchPointerByIdentifier( touch.PointerIdentifier );
							//if already down, but it is not a normal behavior
							if( data != null )
								data.Position = touch.Position;
							else
							{
								data = new TouchPointerData( touch.Position, touch.PointerIdentifier );
								touchPointers.Add( data );
							}
						}
						break;

					case TouchData.ActionEnum.Up:
						{
							for( int n = touchPointers.Count - 1; n >= 0; n-- )
							{
								var data = touchPointers[ n ];
								if( ReferenceEquals( data.PointerIdentifier, touch.PointerIdentifier ) )
									touchPointers.RemoveAt( n );
							}
						}
						break;

					case TouchData.ActionEnum.Move:
						{
							var data = GetTouchPointerByIdentifier( touch.PointerIdentifier );
							if( data != null )
								data.Position = touch.Position;
						}
						break;
					}
				}
			}

			//touch slider
			{
				var m2 = message as InputMessageTouchSliderChanged;
				if( m2 != null )
				{
					var index = m2.Slider;
					if( index >= 0 && index < touchSliders.Length )
						touchSliders[ index ] = m2.Value;
				}
			}

			////special input device
			//{
			//	var m = message as InputMessageSpecialInputDevice;
			//	if( m != null )
			//	{
			//	}
			//}
		}

		public bool PerformMessage( GameMode gameMode, InputMessage message )
		{
			InputMessageBefore( gameMode, message );
			if( message.Handled )
				return true;

			OnInputMessage( gameMode, message );
			if( message.Handled )
				return true;

			InputMessageEvent?.Invoke( this, gameMode, message );
			if( message.Handled )
				return true;

			return false;
		}

		[Browsable( false )]
		public bool InputEnabled
		{
			get { return inputEnabled; }
		}

		public bool IsKeyPressed( EKeys key )
		{
			if( keys != null )
				return keys[ (int)key ];
			else
				return false;
		}

		public bool IsMouseButtonPressed( EMouseButtons button )
		{
			return mouseButtons[ (int)button ];
		}

		[Browsable( false )]
		public Vector2 MousePosition
		{
			get { return mousePosition; }
		}

		[Browsable( false )]
		public bool MouseRelativeMode
		{
			get { return mouseRelativeMode; }
		}

		[Browsable( false )]
		public bool[] JoystickButtons
		{
			get { return joystickButtons; }
		}

		[Browsable( false )]
		public double[] JoystickAxes
		{
			get { return joystickAxes; }
		}

		[Browsable( false )]
		public JoystickPOVDirections[] JoystickPOVs
		{
			get { return joystickPOVs; }
		}

		[Browsable( false )]
		public Vector2[] JoystickSliders
		{
			get { return joystickSliders; }
		}

		public bool IsJoystickButtonPressed( JoystickButtons button )
		{
			var index = (int)button;
			if( index >= 0 && index < joystickButtons.Length )
				return joystickButtons[ index ];
			return false;
		}

		public double GetJoystickAxis( JoystickAxes axis )
		{
			var index = (int)axis;
			if( index >= 0 && index < joystickAxes.Length )
				return joystickAxes[ index ];
			return 0;
		}

		public JoystickPOVDirections GetJoystickPOV( JoystickPOVs pov )
		{
			var index = (int)pov;
			if( index >= 0 && index < joystickPOVs.Length )
				return joystickPOVs[ index ];
			return 0;
		}

		public Vector2 GetJoystickSlider( JoystickSliders slider )
		{
			var index = (int)slider;
			if( index >= 0 && index < joystickSliders.Length )
				return joystickSliders[ index ];
			return Vector2.Zero;
		}

		[Browsable( false )]
		public IList<TouchPointerData> TouchPointers
		{
			get { return touchPointers; }
		}

		public TouchPointerData GetTouchPointerByIdentifier( object pointerIdentifier )
		{
			for( int n = 0; n < touchPointers.Count; n++ )
			{
				var data = touchPointers[ n ];
				if( ReferenceEquals( data.PointerIdentifier, pointerIdentifier ) )
					return data;
			}
			return null;
		}

		[Browsable( false )]
		public Vector2[] TouchSliders
		{
			get { return touchSliders; }
		}
	}
}
