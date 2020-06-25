// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Input
{
	public abstract class JoystickInputEvent : InputEvent
	{
		JoystickInputDevice device;

		public JoystickInputEvent( InputDevice device )
			: base( device )
		{
			this.device = (JoystickInputDevice)device;
		}

		public new JoystickInputDevice Device
		{
			get { return device; }
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class JoystickButtonEvent : JoystickInputEvent
	{
		JoystickInputDevice.Button button;

		public JoystickButtonEvent( InputDevice device, JoystickInputDevice.Button button )
			: base( device )
		{
			this.button = button;
		}

		public JoystickInputDevice.Button Button
		{
			get { return button; }
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public class JoystickButtonDownEvent : JoystickButtonEvent
	{
		public JoystickButtonDownEvent( InputDevice device, JoystickInputDevice.Button button )
			: base( device, button )
		{
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public class JoystickButtonUpEvent : JoystickButtonEvent
	{
		public JoystickButtonUpEvent( InputDevice device, JoystickInputDevice.Button button )
			: base( device, button )
		{
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public class JoystickAxisChangedEvent : JoystickInputEvent
	{
		JoystickInputDevice.Axis axis;

		public JoystickAxisChangedEvent( InputDevice device, JoystickInputDevice.Axis axis )
			: base( device )
		{
			this.axis = axis;
		}

		public JoystickInputDevice.Axis Axis
		{
			get { return axis; }
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public class JoystickPOVChangedEvent : JoystickInputEvent
	{
		JoystickInputDevice.POV pov;

		public JoystickPOVChangedEvent( InputDevice device, JoystickInputDevice.POV pov )
			: base( device )
		{
			this.pov = pov;
		}

		public JoystickInputDevice.POV POV
		{
			get { return pov; }
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	public class JoystickSliderChangedEvent : JoystickInputEvent
	{
		JoystickInputDevice.Slider slider;
		JoystickSliderAxes axis;

		public JoystickSliderChangedEvent( InputDevice device, JoystickInputDevice.Slider slider, JoystickSliderAxes axis )
			: base( device )
		{
			this.slider = slider;
			this.axis = axis;
		}

		public JoystickInputDevice.Slider Slider
		{
			get { return slider; }
		}

		public JoystickSliderAxes Axis
		{
			get { return axis; }
		}
	}
}
