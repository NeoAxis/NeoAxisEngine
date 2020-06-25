// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using NeoAxis.Input;

namespace NeoAxis
{
	/// <summary>
	/// An object to process input from a player.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Input Processing", -9998 )]
	public class Component_InputProcessing : Component
	{
		//user interaction
		//object lockerKeysMouse = new object();
		bool inputEnabled;
		bool[] keys;
		bool[] mouseButtons = new bool[ 5 ];
		Vector2 mousePosition = new Vector2( -10000, -10000 );
		bool mouseRelativeMode;

		bool[] joystickButtons = new bool[ 64 ];
		double[] joystickAxes = new double[ 32 ];
		JoystickPOVDirections[] joystickPOVs = new JoystickPOVDirections[ 16 ];
		Vector2[] joystickSliders = new Vector2[ 16 ];

		//Vector2 restoreMousePosAfterRelativeMode;
		//bool mouseRelativeModeSkipOneMouseMove;

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

		protected virtual void OnInputMessage( UIControl playScreen, Component_GameMode gameMode, InputMessage message ) { }

		public delegate void InputMessageEventDelegate( Component_InputProcessing sender, UIControl playScreen, Component_GameMode gameMode, InputMessage message );
		public event InputMessageEventDelegate InputMessageEvent;

		//!!!!protected virtual? event?
		void InputMessageBefore( UIControl playScreen, Component_GameMode gameMode, InputMessage message )
		{
			//input enabled changed
			{
				var m = message as InputMessageInputEnabledChanged;
				if( m != null )
					inputEnabled = m.Value;
			}

			//key down
			{
				var m = message as InputMessageKeyDown;
				if( m != null )
				{
					//lock( lockerKeysMouse )
					//{
					if( keys == null )
						keys = new bool[ GetEKeysMaxIndex() + 1 ];
					keys[ (int)m.Key ] = true;
					//}
				}
			}

			//key up
			{
				var m = message as InputMessageKeyUp;
				if( m != null )
				{
					//lock( lockerKeysMouse )
					//{
					if( keys == null )
						keys = new bool[ GetEKeysMaxIndex() + 1 ];

					if( keys[ (int)m.Key ] )
					{
						keys[ (int)m.Key ] = false;
					}
					//}
				}
			}

			//mouse button down
			{
				var m = message as InputMessageMouseButtonDown;
				if( m != null )
				{
					//lock( lockerKeysMouse )
					//{
					mouseButtons[ (int)m.Button ] = true;
					//}
				}
			}

			//mouse button up
			{
				var m = message as InputMessageMouseButtonUp;
				if( m != null )
				{
					//lock( lockerKeysMouse )
					//{
					mouseButtons[ (int)m.Button ] = false;
					//}
				}
			}

			//mouse move
			{
				var m = message as InputMessageMouseMove;
				if( m != null )
				{
					//lock( lockerKeysMouse )
					//{
					mousePosition = m.Position;
					//}
				}
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

			////special input device
			//{
			//	var m = message as InputMessageSpecialInputDevice;
			//	if( m != null )
			//	{
			//	}
			//}
		}

		public bool PerformMessage( UIControl playScreen, Component_GameMode gameMode, InputMessage message )
		{
			InputMessageBefore( playScreen, gameMode, message );
			if( message.Handled )
				return true;

			OnInputMessage( playScreen, gameMode, message );
			if( message.Handled )
				return true;

			InputMessageEvent?.Invoke( this, playScreen, gameMode, message );
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
			//lock( lockerKeysMouse )
			//{
			if( keys != null )
				return keys[ (int)key ];
			else
				return false;
			//}
		}

		//!!!!
		//public bool IsKeyLocked( EKeys key )
		//{
		//	if( key != EKeys.Insert && key != EKeys.NumLock && key != EKeys.Capital && key != EKeys.Scroll )
		//		Log.Fatal( "Viewport: IsKeyLocked: Invalid key value. Next keys can be checked by this method: EKeys.Insert, EKeys.NumLock, EKeys.Capital, EKeys.Scroll." );
		//	return EngineApp.platform.IsKeyLocked( key );
		//}

		public bool IsMouseButtonPressed( EMouseButtons button )
		{
			//lock( lockerKeysMouse )
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

	}
}
