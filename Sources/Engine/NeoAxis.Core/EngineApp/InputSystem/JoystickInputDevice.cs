// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	public abstract class JoystickInputDevice : InputDevice
	{
		Button[] buttons;
		//index - (int)JoystickButtons
		Button[] buttonsDictionary;

		Axis[] axes;
		//index - (int)JoystickAxes
		Axis[] axesDictionary;

		POV[] povs;
		//index - (int)JoystickPOVs
		POV[] povsDictionary;

		Slider[] sliders;
		//index - (int)JoystickSliders
		Slider[] slidersDictionary;

		ForceFeedbackController forceFeedbackController;

		bool deviceLost;

		///////////////////////////////////////////

		public delegate void DeviceEventDelegate( DeviceEvents name );
		public event DeviceEventDelegate DeviceEvent;

		///////////////////////////////////////////

		public sealed class Button
		{
			JoystickButtons name;
			int index;
			bool pressed;

			//

			public Button( JoystickButtons name, int index )
			{
				this.name = name;
				this.index = index;
			}

			public JoystickButtons Name
			{
				get { return name; }
			}

			public int Index
			{
				get { return index; }
			}

			public bool Pressed
			{
				get { return pressed; }
				set { pressed = value; }
			}

			public override string ToString()
			{
				return string.Format( "{0}, Pressed: {1}", name, pressed );
			}
		}

		///////////////////////////////////////////

		public sealed class Axis
		{
			JoystickAxes name;
			RangeF range;
			bool isForceFeedbackSupported;

			float value;

			//

			public Axis( JoystickAxes name, RangeF range, bool isForceFeedbackSupported )
			{
				this.name = name;
				this.range = range;
				this.isForceFeedbackSupported = isForceFeedbackSupported;
			}

			public JoystickAxes Name
			{
				get { return name; }
			}

			public RangeF Range
			{
				get { return range; }
			}

			public bool IsForceFeedbackSupported
			{
				get { return isForceFeedbackSupported; }
			}

			public float Value
			{
				get { return value; }
				set { this.value = value; }
			}

			public override string ToString()
			{
				return string.Format( "{0}, Value: {1}", name, value );
			}
		}

		///////////////////////////////////////////

		public sealed class POV
		{
			JoystickPOVs name;
			JoystickPOVDirections value = JoystickPOVDirections.Centered;

			//

			public POV( JoystickPOVs name )
			{
				this.name = name;
			}

			public JoystickPOVs Name
			{
				get { return name; }
			}

			public JoystickPOVDirections Value
			{
				get { return value; }
				set { this.value = value; }
			}

			public override string ToString()
			{
				return string.Format( "{0}, Value: {1}", name, value );
			}
		}

		///////////////////////////////////////////

		public sealed class Slider
		{
			JoystickSliders name;
			Vector2F value;

			//

			public Slider( JoystickSliders name )
			{
				this.name = name;
			}

			public JoystickSliders Name
			{
				get { return name; }
			}

			public Vector2F Value
			{
				get { return value; }
				set { this.value = value; }
			}

			public override string ToString()
			{
				return string.Format( "{0}, Value: {1}", name, value );
			}
		}

		///////////////////////////////////////////

		public enum DeviceEvents
		{
			DeviceLost,
			DeviceRestored,
		}

		///////////////////////////////////////////

		protected JoystickInputDevice( string name )
			: base( name )
		{
		}

		protected void InitDeviceData( Button[] buttons, Axis[] axes, POV[] povs, Slider[] sliders,
			ForceFeedbackController forceFeedbackController )
		{
			//buttons
			{
				foreach( Button button in buttons )
				{
					if( button == null )
					{
						Log.Fatal( "JoystickInputDevice: SetButtons: " +
							"Button in the array is not initialized." );
					}
				}

				this.buttons = buttons;

				//buttonsDictionary
				{
					int count = 0;
					foreach( Button button in buttons )
					{
						int key = (int)button.Name;
						if( key >= count )
							count = key + 1;
					}

					buttonsDictionary = new Button[ count ];

					foreach( Button button in buttons )
					{
						int key = (int)button.Name;
						buttonsDictionary[ key ] = button;
					}
				}
			}

			//axes
			{
				foreach( Axis axis in axes )
				{
					if( axis == null )
					{
						Log.Fatal( "JoystickInputDevice: SetAxes: " +
							"Axis in the array is not initialized." );
					}
				}

				this.axes = axes;

				//axesDictionary
				{
					int count = 0;
					foreach( Axis axis in axes )
					{
						int key = (int)axis.Name;
						if( key >= count )
							count = key + 1;
					}

					axesDictionary = new Axis[ count ];

					foreach( Axis axis in axes )
					{
						int key = (int)axis.Name;
						axesDictionary[ key ] = axis;
					}
				}
			}

			//povs
			{
				foreach( POV pov in povs )
				{
					if( pov == null )
					{
						Log.Fatal( "JoystickInputDevice: SetPOVs: " +
							"POV in the array is not initialized." );
					}
				}

				this.povs = povs;

				//povsDictionary
				{
					int count = 0;
					foreach( POV pov in povs )
					{
						int key = (int)pov.Name;
						if( key >= count )
							count = key + 1;
					}

					povsDictionary = new POV[ count ];

					foreach( POV pov in povs )
					{
						int key = (int)pov.Name;
						povsDictionary[ key ] = pov;
					}
				}
			}

			//sliders
			{
				foreach( Slider slider in sliders )
				{
					if( slider == null )
					{
						Log.Fatal( "JoystickInputDevice: SetSliders: " +
							"Slider in the array is not initialized." );
					}
				}

				this.sliders = sliders;

				//slidersDictionary
				{
					int count = 0;
					foreach( Slider slider in sliders )
					{
						int key = (int)slider.Name;
						if( key >= count )
							count = key + 1;
					}

					slidersDictionary = new Slider[ count ];

					foreach( Slider slider in sliders )
					{
						int key = (int)slider.Name;
						slidersDictionary[ key ] = slider;
					}
				}
			}

			//forceFeedbackController
			this.forceFeedbackController = forceFeedbackController;
		}

		public Button[] Buttons
		{
			get { return buttons; }
		}

		public Axis[] Axes
		{
			get { return axes; }
		}

		public POV[] POVs
		{
			get { return povs; }
		}

		public Slider[] Sliders
		{
			get { return sliders; }
		}

		public ForceFeedbackController ForceFeedbackController
		{
			get { return forceFeedbackController; }
		}

		public Button GetButtonByName( JoystickButtons button )
		{
			int key = (int)button;
			if( key >= buttonsDictionary.Length )
				return null;
			return buttonsDictionary[ key ];
		}

		public Axis GetAxisByName( JoystickAxes axis )
		{
			int key = (int)axis;
			if( key >= axesDictionary.Length )
				return null;
			return axesDictionary[ key ];
		}

		public POV GetPOVByName( JoystickPOVs pov )
		{
			int key = (int)pov;
			if( key >= povsDictionary.Length )
				return null;
			return povsDictionary[ key ];
		}

		public Slider GetSliderByName( JoystickSliders slider )
		{
			int key = (int)slider;
			if( key >= slidersDictionary.Length )
				return null;
			return slidersDictionary[ key ];
		}

		public bool IsButtonPressed( JoystickButtons button )
		{
			Button btn = GetButtonByName( button );
			if( btn == null )
				return false;
			return btn.Pressed;
		}

		public bool IsDeviceLost()
		{
			return deviceLost;
		}

		protected void DeviceLost()
		{
			deviceLost = true;

			Log.InvisibleInfo( "JoystickInputDevice: Device lost" );

			if( forceFeedbackController != null )
				forceFeedbackController.OnDeviceLost();

			if( DeviceEvent != null )
				DeviceEvent( DeviceEvents.DeviceLost );
		}

		protected void DeviceRestore()
		{
			deviceLost = false;

			Log.InvisibleInfo( "JoystickInputDevice: Device restored" );

			if( forceFeedbackController != null )
				forceFeedbackController.OnDeviceRestore();

			if( DeviceEvent != null )
				DeviceEvent( DeviceEvents.DeviceRestored );
		}

	}
}
