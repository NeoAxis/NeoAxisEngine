
//!!!!from 3.5:

//// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Engine.Utils;
//using Engine.MathEx;
//using DirectInputNativeWrapper;
//using System.Runtime.InteropServices;

//namespace Engine
//{
//	internal class MacOSXJoystickInputDevice : JoystickInputDevice
//	{
//		IntPtr nativeObject;

//		///////////////////////////////////////////////////////////////////////////////////////////////////////

//		struct Wrapper
//		{
//			public const string library = "MacAppNativeWrapper";
//			public const CallingConvention convention = CallingConvention.Cdecl;
//		}

//		struct MacAppNativeWrapper
//		{
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ShutdownDevice", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void ShutdownDevice( IntPtr nativeObject );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDeviceButtonCount", CallingConvention = Wrapper.convention )]
//			public unsafe static extern int GetDeviceButtonCount( IntPtr nativeObject );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDevicePOVCount", CallingConvention = Wrapper.convention )]
//			public unsafe static extern int GetDevicePOVCount( IntPtr nativeObject );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDeviceAxisCount", CallingConvention = Wrapper.convention )]
//			public unsafe static extern int GetDeviceAxisCount( IntPtr nativeObject );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDeviceAxisInfo", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void GetDeviceAxisInfo( IntPtr nativeObject, int axisIndex,
//				out JoystickAxes name, out float rangeMin, out float rangeMax );
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDeviceSliderCount", CallingConvention = Wrapper.convention )]
//			public unsafe static extern int GetDeviceSliderCount( IntPtr nativeObject );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_UpdateDeviceState", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool UpdateDeviceState( IntPtr nativeObject,
//				out ComponentTypes componentType, out int componentIndex, out float value1, out float value2 );
//		}

//		///////////////////////////////////////////////////////////////////////////////////////////////////////

//		enum ComponentTypes
//		{
//			Button,
//			POV,
//			Axis,
//			Slider,
//		}

//		///////////////////////////////////////////////////////////////////////////////////////////////////////

//		internal MacOSXJoystickInputDevice( string name, IntPtr nativeObject )
//			: base( name )
//		{
//			this.nativeObject = nativeObject;
//		}

//		internal unsafe bool Init()
//		{
//			//buttons
//			int buttonCount = MacAppNativeWrapper.GetDeviceButtonCount( nativeObject );
//			Button[] buttons = new Button[ buttonCount ];
//			for( int n = 0; n < buttons.Length; n++ )
//				buttons[ n ] = new Button( (JoystickButtons)n, n );

//			//povs
//			int povCount = MacAppNativeWrapper.GetDevicePOVCount( nativeObject );
//			POV[] povs = new POV[ povCount ];
//			for( int n = 0; n < povs.Length; n++ )
//				povs[ n ] = new JoystickInputDevice.POV( (JoystickPOVs)n );

//			//axes
//			int axisCount = MacAppNativeWrapper.GetDeviceAxisCount( nativeObject );
//			Axis[] axes = new Axis[ axisCount ];
//			for( int n = 0; n < axes.Length; n++ )
//			{
//				JoystickAxes axisName;
//				float rangeMin, rangeMax;
//				MacAppNativeWrapper.GetDeviceAxisInfo( nativeObject, n, out axisName, out rangeMin,
//					out rangeMax );
//				bool isForceFeedbackSupported = false;
//				axes[ n ] = new JoystickInputDevice.Axis( axisName, new Range( rangeMin, rangeMax ),
//					isForceFeedbackSupported );
//			}

//			//sliders
//			int sliderCount = MacAppNativeWrapper.GetDeviceSliderCount( nativeObject );
//			Slider[] sliders = new Slider[ sliderCount ];
//			for( int n = 0; n < sliders.Length; n++ )
//				sliders[ n ] = new JoystickInputDevice.Slider( (JoystickSliders)n );

//			ForceFeedbackController forceFeedbackController = null;

//			//initialize data
//			InitDeviceData( buttons, axes, povs, sliders, forceFeedbackController );

//			return true;
//		}

//		protected unsafe override void OnShutdown()
//		{
//			if( nativeObject != IntPtr.Zero )
//			{
//				MacAppNativeWrapper.ShutdownDevice( nativeObject );
//				nativeObject = IntPtr.Zero;
//			}
//		}

//		unsafe protected override void OnUpdateState()
//		{
//			ComponentTypes componentType;
//			int componentIndex;
//			float value1;
//			float value2;

//			while( MacAppNativeWrapper.UpdateDeviceState( nativeObject, out componentType, out componentIndex,
//				out value1, out value2 ) )
//			{
//				switch( componentType )
//				{

//				case ComponentTypes.Button:
//					{
//						bool pressed = value1 > .5f;

//						Button button = Buttons[ componentIndex ];
//						if( button.Pressed != pressed )
//						{
//							button.Pressed = pressed;
//							if( pressed )
//								InputDeviceManager.Instance.SendEvent( new JoystickButtonDownEvent( this, button ) );
//							else
//								InputDeviceManager.Instance.SendEvent( new JoystickButtonUpEvent( this, button ) );
//						}
//					}
//					break;

//				case ComponentTypes.POV:
//					{
//						int v = (int)value1;

//						JoystickPOVDirections value;
//						switch( v )
//						{
//						case 0: value = JoystickPOVDirections.North; break;
//						case 1: value = JoystickPOVDirections.North | JoystickPOVDirections.East; break;
//						case 2: value = JoystickPOVDirections.East; break;
//						case 3: value = JoystickPOVDirections.South | JoystickPOVDirections.East; break;
//						case 4: value = JoystickPOVDirections.South; break;
//						case 5: value = JoystickPOVDirections.South | JoystickPOVDirections.West; break;
//						case 6: value = JoystickPOVDirections.West; break;
//						case 7: value = JoystickPOVDirections.North | JoystickPOVDirections.West; break;
//						default: value = JoystickPOVDirections.Centered; break;
//						}

//						POV pov = POVs[ componentIndex ];
//						if( pov.Value != value )
//						{
//							pov.Value = value;
//							InputDeviceManager.Instance.SendEvent( new JoystickPOVChangedEvent( this, pov ) );
//						}
//					}
//					break;

//				case ComponentTypes.Axis:
//					{
//						float value = value1;

//						Axis axis = Axes[ componentIndex ];
//						if( axis.Value != value )
//						{
//							axis.Value = value;
//							InputDeviceManager.Instance.SendEvent( new JoystickAxisChangedEvent( this, axis ) );
//						}
//					}
//					break;

//				case ComponentTypes.Slider:
//					{
//						Vec2 value = new Vec2( value1, value2 );

//						Slider slider = Sliders[ componentIndex ];
//						if( slider.Value != value )
//						{
//							bool xEvent = slider.Value.X != value.X;
//							bool yEvent = slider.Value.Y != value.Y;
//							slider.Value = value;
//							if( xEvent )
//								InputDeviceManager.Instance.SendEvent( new JoystickSliderChangedEvent( this, slider, JoystickSliderAxes.X ) );
//							if( yEvent )
//								InputDeviceManager.Instance.SendEvent( new JoystickSliderChangedEvent( this, slider, JoystickSliderAxes.Y ) );
//						}
//					}
//					break;

//				}
//			}
//		}
//	}
//}
